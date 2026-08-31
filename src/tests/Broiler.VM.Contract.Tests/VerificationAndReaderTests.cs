using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The verification boundary and the bounded-reading primitives underneath it: the failure taxonomy
/// is stable, no partial state escapes a failed verification, and a declared count above its bound
/// is refused before anything proportional to it is allocated.
/// </summary>
public sealed class VerificationAndReaderTests
{
    [Theory]
    [InlineData(FixtureArtifactWriter.Corruption.Truncated)]
    [InlineData(FixtureArtifactWriter.Corruption.BadMagic)]
    [InlineData(FixtureArtifactWriter.Corruption.UnknownFormatVersion)]
    [InlineData(FixtureArtifactWriter.Corruption.SectionLengthMismatch)]
    [InlineData(FixtureArtifactWriter.Corruption.NonCanonicalVarInt)]
    public void A_Corrupt_Artifact_Fails_Before_Execution_And_Produces_No_Handle(
        FixtureArtifactWriter.Corruption corruption)
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var descriptor = FixtureComposition.Descriptor();

        var payload = FixtureArtifactWriter.Write(
            [1], [FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn], corruption);

        var result = runtime.Verify(in descriptor, payload, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.False(result.TryGetArtifact(out _));
        Assert.Equal(VmOutcome.InvalidArtifact, result.Outcome);
    }

    [Fact]
    public void An_Over_Declared_Count_Is_A_Resource_Answer_Not_A_Malformed_One()
    {
        // The bytes were fine and the budget was not. Reporting an over-declared count as a
        // corrupt file would tell a host to look at the wrong thing.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var descriptor = FixtureComposition.Descriptor();

        var payload = FixtureArtifactWriter.Write(
            [1],
            [FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn],
            FixtureArtifactWriter.Corruption.OverDeclaredCount);

        var result = runtime.Verify(in descriptor, payload, CancellationToken.None);

        Assert.Equal(VmOutcome.ResourceExhaustion, result.Outcome);
        Assert.Equal(VmBudgetDimension.DeclaredCount, result.Diagnostics.ExhaustedDimension);
    }

    [Fact]
    public void A_Cancelled_Verification_Is_Cancellation_Not_Resource_Exhaustion()
    {
        // The bounded reader folds three causes into one status: a refused work charge, a
        // cancellation observed at a poll, and a poll-bound violation all surface to the profile
        // as WorkBudgetExhausted, so a verifier answers ResourceExhaustion for all three. Only the
        // meter knows which happened. If the core reports the verifier's attribution unchanged, a
        // host that cancelled is told its artifact was too expensive - and a malformed corpus
        // labelled by category and reason cannot tell a cancellation from a real exhaustion.
        // The cancellation must arrive *during* the verifier's read. A token already cancelled on
        // entry is answered before the profile is called at all, so it cannot witness this.
        using var cancellation = new CancellationTokenSource();

        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorForVerifierHook(
                FixtureVmProfileVariant.Conforming,
                cancellation.Cancel)));

        var descriptor = FixtureComposition.Descriptor();

        var payload = FixtureArtifactWriter.Write(
            [1], [FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn]);

        var result = runtime.Verify(in descriptor, payload, cancellation.Token);

        Assert.Equal(VmOutcome.Cancellation, result.Outcome);
        Assert.Equal(VmReason.Cancelled, result.Reason);
        Assert.False(result.TryGetArtifact(out _));
    }

    [Fact]
    public void An_Unknown_Profile_Is_Not_An_Invalid_Artifact()
    {
        // Conflating the two misreports a composition mistake as a corrupt file, which is the most
        // likely diagnostic error for a single-profile product.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var descriptor = FixtureComposition.Descriptor(
            SecondFixtureVmProfile.Id, SecondFixtureVmProfile.Manifest);

        var result = runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None);

        Assert.Equal(VmOutcome.UnsupportedProfile, result.Outcome);
        Assert.Equal(VmReason.ProfileNotInCatalog, result.Reason);
        Assert.Equal(SecondFixtureVmProfile.Id, result.Diagnostics.ProfileId);
    }

    [Fact]
    public void An_Unsupported_Format_Version_Is_An_Invalid_Artifact()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var descriptor = FixtureComposition.Descriptor(formatVersion: 7);

        var result = runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None);

        Assert.Equal(VmOutcome.InvalidArtifact, result.Outcome);
        Assert.Equal(VmReason.UnsupportedProfileFormatVersion, result.Reason);
    }

    [Fact]
    public void An_Unaccepted_Feature_Manifest_Is_An_Invalid_Artifact()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var descriptor = FixtureComposition.Descriptor(
            manifest: VmFeatureManifestId.Parse("Broiler.VM.Fixture.Alpha.Unknown"));

        var result = runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None);

        Assert.Equal(VmOutcome.InvalidArtifact, result.Outcome);
        Assert.Equal(VmReason.UnsupportedFeatureManifest, result.Reason);
    }

    [Fact]
    public void A_Malformed_Descriptor_Is_Rejected_Before_The_Bytes_Are_Looked_At()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var descriptor = new VmArtifactDescriptor(
            default, FixtureFormat.FormatVersion, FixtureVmProfile.Manifest, default, VmCallerIdentity.None);

        var result = runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None);

        Assert.Equal(VmOutcome.InvalidArtifact, result.Outcome);
        Assert.Equal(VmReason.MalformedArtifactDescriptor, result.Reason);
    }

    [Fact]
    public void Mutating_The_Callers_Buffer_After_Verification_Changes_Nothing()
    {
        // The handle owns a decoded form, so later mutation of the caller's bytes cannot affect
        // verified instructions. This is invariant 3's snapshot rule, checked rather than assumed.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var payload = FixtureArtifactWriter.Sum(20, 22);
        var artifact = FixtureComposition.Verify(runtime, payload);

        Array.Clear(payload);

        using var instance = FixtureComposition.Instantiate(runtime, artifact);
        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.True(FixtureVmProfileResults.TryGetValue(in result, out var value));
        Assert.Equal(42, value.Value);
    }

    [Fact]
    public void The_Verified_Handle_Records_Its_Full_Identity()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));

        var identity = artifact.Identity;

        Assert.Equal(FixtureVmProfile.Id, identity.ProfileId);
        Assert.Equal(1, identity.DescriptorRevision);
        Assert.Equal(FixtureFormat.FormatVersion, identity.AcceptedProfileFormatVersion);
        Assert.Equal(FixtureVmProfile.Manifest, identity.ManifestId);
        Assert.Equal(VmCoreContract.Version, identity.CoreContractVersion);
        Assert.False(identity.HostSignatureAssumptions.IsDefaultOrEmpty);
    }

    [Fact]
    public void The_Bounded_Reader_Refuses_A_Declared_Count_Before_Allocating()
    {
        // The order is the whole point: the count is checked against its bound first, so a hostile
        // number costs nothing proportional to itself.
        var meter = new CountingMeter();
        var bounds = new VmReadBounds(1024, 8, 4, 4);

        var payload = new byte[] { 0xFF, 0xFF, 0xFF, 0x7F };
        var reader = new VmBoundedReader(payload, in bounds, meter);

        Assert.False(reader.TryReadDeclaredCount(out var count));
        Assert.Equal(VmBoundedReadStatus.DeclaredCountExceeded, reader.Status);
        Assert.Equal(0u, count);
        Assert.Equal(0ul, meter.Reserved);
    }

    [Fact]
    public void The_Bounded_Allocator_Refuses_Before_Allocating()
    {
        var meter = new CountingMeter();
        var bounds = new VmReadBounds(1024, 8, 4, 4);

        Assert.False(VmBoundedAllocator.TryAllocate<long>(in bounds, meter, 1_000_000, out var buffer));
        Assert.Empty(buffer);
        Assert.Equal(0ul, meter.Reserved);
    }

    [Fact]
    public void A_Non_Canonical_Variable_Length_Integer_Is_Refused()
    {
        // Two encodings of one value would make a byte-identical artifact check meaningless and
        // would let a payload carry a value past a length check that read it differently.
        var meter = new CountingMeter();
        var bounds = new VmReadBounds(1024, 8, 1024, 4);

        var reader = new VmBoundedReader(new byte[] { 0x80, 0x00 }, in bounds, meter);

        Assert.False(reader.TryReadVarUInt32(out _));
        Assert.Equal(VmBoundedReadStatus.MalformedEncoding, reader.Status);
    }

    [Fact]
    public void A_Spent_Reader_Stays_Spent()
    {
        // One failure cannot be stepped past by a caller that ignored a return value.
        var meter = new CountingMeter();
        var bounds = new VmReadBounds(1024, 8, 1024, 4);

        var reader = new VmBoundedReader(new byte[] { 0x01 }, in bounds, meter);

        Assert.True(reader.TryReadByte(out _));
        Assert.False(reader.TryReadByte(out _));
        Assert.Equal(VmBoundedReadStatus.Truncated, reader.Status);

        Assert.False(reader.TryReadVarUInt32(out _));
        Assert.Equal(VmBoundedReadStatus.Truncated, reader.Status);
    }

    [Fact]
    public void A_Section_Must_Consume_Exactly_What_It_Declared()
    {
        // Consuming less than declared is as much a structural error as consuming more: it means
        // the artifact and the verifier disagree about where the next section starts.
        var meter = new CountingMeter();
        var bounds = new VmReadBounds(1024, 8, 1024, 4);

        var reader = new VmBoundedReader(new byte[] { 1, 2, 3, 4 }, in bounds, meter);

        Assert.True(reader.TryEnterSection(4, out var frame));
        Assert.True(reader.TryReadByte(out _));
        Assert.False(reader.TryExitSection(in frame));
        Assert.Equal(VmBoundedReadStatus.MalformedEncoding, reader.Status);
    }

    [Fact]
    public void Section_Nesting_Beyond_The_Bound_Is_Refused()
    {
        var meter = new CountingMeter();
        var bounds = new VmReadBounds(1024, 8, 1024, 2);

        var reader = new VmBoundedReader(new byte[16], in bounds, meter);

        Assert.True(reader.TryEnterSection(8, out _));
        Assert.True(reader.TryEnterSection(4, out _));
        Assert.False(reader.TryEnterSection(2, out _));
        Assert.Equal(VmBoundedReadStatus.StructuralDepthExceeded, reader.Status);
    }

    [Fact]
    public void A_Payload_Larger_Than_The_Artifact_Bound_Is_Refused_At_Construction()
    {
        var meter = new CountingMeter();
        var bounds = new VmReadBounds(4, 8, 1024, 4);

        var reader = new VmBoundedReader(new byte[16], in bounds, meter);

        Assert.Equal(VmBoundedReadStatus.ArtifactBytesExceeded, reader.Status);
        Assert.False(reader.TryReadByte(out _));
    }

    [Fact]
    public void A_Reader_Polls_Once_Per_Byte_By_Default()
    {
        // The behaviour every existing caller has. It is recorded so the granularity overload
        // cannot change it by accident.
        var meter = new CountingMeter();
        var bounds = new VmReadBounds(1024, 8, 1024, 4);

        var reader = new VmBoundedReader(new byte[16], in bounds, meter);

        for (var i = 0; i < 16; i++)
        {
            Assert.True(reader.TryReadByte(out _));
        }

        Assert.Equal(16, meter.Charges);
        Assert.Equal(16, meter.Polls);
    }

    [Fact]
    public void A_Granularity_Batches_The_Poll_And_Never_The_Charge()
    {
        // A poll takes the meter's lock and reads a clock, and the contract bounds cancellation
        // latency by the profile's declared uncharged-work bound rather than by one byte - so
        // polling per byte is a cost with no promise behind it. The charge is a different thing:
        // work is charged before it is done, and batching that would let a refused budget be
        // stepped past. This asserts both halves at once.
        var meter = new CountingMeter();
        var bounds = new VmReadBounds(1024, 8, 1024, 4);

        var reader = new VmBoundedReader(new byte[16], in bounds, meter, pollGranularity: 4);

        for (var i = 0; i < 16; i++)
        {
            Assert.True(reader.TryReadByte(out _));
        }

        Assert.Equal(16, meter.Charges);
        Assert.Equal(4, meter.Polls);
    }

    [Fact]
    public void A_Refused_Poll_Stops_The_Reader_Even_When_Batched()
    {
        // Batching may delay a cancellation by up to the granularity. It may not lose one.
        var meter = new CountingMeter { RefusePollAfter = 1 };
        var bounds = new VmReadBounds(1024, 8, 1024, 4);

        var reader = new VmBoundedReader(new byte[16], in bounds, meter, pollGranularity: 4);

        var consumed = 0;
        while (reader.TryReadByte(out _))
        {
            consumed++;
        }

        // Seven, not eight: the poll that refuses belongs to the eighth byte, and a byte whose
        // charge-and-poll did not both succeed is not consumed. So the refusal is observed at the
        // boundary it was batched to, and the byte it stopped on stays unread.
        Assert.Equal(VmBoundedReadStatus.WorkBudgetExhausted, reader.Status);
        Assert.Equal(7, consumed);
        Assert.Equal(2, meter.Polls);
    }

    private sealed class CountingMeter : IVmBoundedAllocationMeter
    {
        internal ulong Reserved { get; private set; }

        internal int Charges { get; private set; }

        internal int Polls { get; private set; }

        internal int RefusePollAfter { get; init; } = int.MaxValue;

        public bool TryReserve(ulong byteCount)
        {
            Reserved += byteCount;
            return true;
        }

        public void Release(ulong byteCount) => Reserved -= byteCount;

        public bool TryChargeWork(ulong workUnits)
        {
            Charges++;
            return true;
        }

        public bool Poll()
        {
            Polls++;
            return Polls <= RefusePollAfter;
        }
    }
}
