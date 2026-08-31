using Broiler.VM;

namespace Broiler.VM.Sample.FeedConsumer;

/// <summary>
/// The profile's verifier: bytes in, an immutable decoded state out, or a deterministic refusal.
/// </summary>
/// <remarks>
/// <para>
/// Nothing this profile executes has skipped this method. That is the property the whole contract
/// is built around, and it is worth stating in a sample because it is what makes the rest of the
/// design make sense: the executor below does no validation at all, and it is allowed not to
/// because the state it receives can only have come from here.
/// </para>
/// <para>
/// Every length, count and offset goes through <see cref="VmBoundedReader"/>. A profile author is
/// free to parse bytes by hand and should not: the reader is where the checked arithmetic, the
/// canonical LEB128 form, the truncation checks and the allocation guard live, and a profile that
/// reimplements them reimplements the bugs too.
/// </para>
/// </remarks>
internal sealed class CounterVerifier : IVmProfileVerifier
{
    /// <inheritdoc/>
    public VmProfileId ProfileId => CounterProfile.Id;

    /// <inheritdoc/>
    public int BuiltAgainstCoreContractVersion => VmCoreContract.Version;

    /// <inheritdoc/>
    /// <remarks>
    /// What this profile was WRITTEN against, which is not the same question as what it was
    /// compiled against. They agree today. They stop agreeing the moment a new core contract
    /// version ships and this profile has not been re-read against it, and the core compares them
    /// so that the difference is a refusal rather than an assumption.
    /// </remarks>
    public int AuthoredCoreContractVersion => 1;

    /// <inheritdoc/>
    public int VerifierSemanticVersion => 1;

    /// <inheritdoc/>
    public VmVerifierOutcome Verify(
        in VmArtifactDescriptor descriptor,
        ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        CancellationToken cancellationToken)
    {
        // The four artifact-shaped ceilings, projected out of the effective policy the core
        // computed BEFORE calling this method. A profile never reads a host's configuration: it
        // reads what survived the intersection of host, profile and artifact.
        var limits = context.Ceilings.VerificationCeilings;

        var bounds = new VmReadBounds(
            limits[VmBudgetDimension.ArtifactBytes],
            limits[VmBudgetDimension.SectionCount],
            limits[VmBudgetDimension.DeclaredCount],
            limits[VmBudgetDimension.StructuralDepth]);

        var reader = new VmBoundedReader(payload, in bounds, new CounterReadMeter(context.Meter));

        if (!reader.TryReadBytes(4, out var magic))
        {
            return Fail(ref reader, position: 0);
        }

        if (!magic.SequenceEqual(CounterFormat.Magic))
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.MalformedEncoding,
                CounterFormat.NotThisFormat,
                new VmSourcePosition(-1, 0, 0, 0));
        }

        if (!reader.TryReadVarUInt32(out var formatVersion))
        {
            return Fail(ref reader, reader.Position);
        }

        if (formatVersion != CounterFormat.FormatVersion)
        {
            // An unknown version is refused deterministically and never guessed at. Interpreting
            // old bytes under new semantics is the failure mode versioning exists to prevent.
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.UnknownFormatVersion,
                CounterFormat.UnknownVersion,
                new VmSourcePosition(-1, reader.Position, 0, 0));
        }

        if (descriptor.FormatVersion != formatVersion)
        {
            // The caller labelled these bytes and the bytes disagree. That is the caller's mistake
            // and it gets a deterministic answer rather than a search for a decoder that accepts
            // them.
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.DescriptorMismatch,
                CounterFormat.DescriptorDisagrees,
                new VmSourcePosition(-1, reader.Position, 0, 0));
        }

        if (!reader.TryReadVarUInt64(out var start))
        {
            return Fail(ref reader, reader.Position);
        }

        // A declared count is read through the member that refuses it BEFORE returning it, so
        // nothing here can loop or size a buffer from a number that never passed its bound. This
        // profile allocates nothing proportional to the count, but it reads it this way anyway:
        // the day it does allocate, the guard is already in the right place.
        if (!reader.TryReadDeclaredCount(out var steps))
        {
            return Fail(ref reader, reader.Position);
        }

        if (reader.Remaining != 0)
        {
            // Trailing bytes are a rejection, not something to ignore. Ignoring them would let an
            // artifact carry content this verifier never looked at.
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.InconsistentStructure,
                CounterFormat.TrailingBytes,
                new VmSourcePosition(-1, reader.Position, 0, 0));
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return VmVerifierOutcome.Cancellation();
        }

        return VmVerifierOutcome.Verified(
            new CounterState(unchecked((long)start), steps), VmArtifactSharing.Shareable);
    }

    /// <summary>
    /// Turns the reader's own status into the outcome category it belongs to.
    /// </summary>
    /// <remarks>
    /// A reader that stopped because the budget ran out is a RESOURCE answer and one that stopped
    /// because the bytes were wrong is a VALIDITY answer, and collapsing them would tell a host
    /// that a well-formed artifact was malformed whenever a ceiling was tight.
    /// </remarks>
    private static VmVerifierOutcome Fail(ref VmBoundedReader reader, ulong position) =>
        reader.Status switch
        {
            VmBoundedReadStatus.WorkBudgetExhausted =>
                VmVerifierOutcome.ResourceExhaustion(
                    VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact),

            VmBoundedReadStatus.ArtifactBytesExceeded =>
                VmVerifierOutcome.ResourceExhaustion(
                    VmBudgetDimension.ArtifactBytes, VmBudgetScope.Artifact),

            VmBoundedReadStatus.DeclaredCountExceeded =>
                VmVerifierOutcome.ResourceExhaustion(
                    VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact),

            VmBoundedReadStatus.SectionCountExceeded =>
                VmVerifierOutcome.ResourceExhaustion(
                    VmBudgetDimension.SectionCount, VmBudgetScope.Artifact),

            VmBoundedReadStatus.StructuralDepthExceeded =>
                VmVerifierOutcome.ResourceExhaustion(
                    VmBudgetDimension.StructuralDepth, VmBudgetScope.Artifact),

            _ => VmVerifierOutcome.InvalidArtifact(
                VmReason.Truncated, CounterFormat.NotThisFormat,
                new VmSourcePosition(-1, position, 0, 0)),
        };
}

/// <summary>
/// The projection between the contract's metering surface and the bounded reader's.
/// </summary>
/// <remarks>
/// <c>Broiler.VM.Binary</c> names no contract vocabulary - bounded reading is mechanism and must
/// not acquire semantics - so its meter takes plain byte counts while the contract's takes budget
/// dimensions. Whoever holds both vocabularies performs the projection, and for a profile that is
/// these four lines. It is the visible price of keeping the mechanism assembly a graph sink, and a
/// sample is the right place to show that it is four lines and not a design problem.
/// </remarks>
internal sealed class CounterReadMeter : IVmBoundedAllocationMeter
{
    private readonly IVmMeter meter;

    internal CounterReadMeter(IVmMeter meter) => this.meter = meter;

    /// <inheritdoc/>
    public bool TryReserve(ulong byteCount) =>
        meter.TryCharge(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    public void Release(ulong byteCount) =>
        meter.ReportReleased(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    public bool TryChargeWork(ulong workUnits) =>
        meter.TryCharge(VmBudgetDimension.VerifierWork, workUnits);

    /// <inheritdoc/>
    public bool Poll() => meter.Poll();
}
