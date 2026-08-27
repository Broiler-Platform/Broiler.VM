namespace Broiler.VM.Fixtures;

/// <summary>
/// The fixture profile's verifier: a real decoder over a real format, driven through the core's
/// bounded reader.
/// </summary>
/// <remarks>
/// <para>
/// Every length, count and offset goes through <see cref="VmBoundedReader"/>, and the constant pool
/// is allocated through <see cref="VmBoundedAllocator"/>, so the checklist obligation that a
/// declared count above its bound fails <em>before</em> any allocation proportional to it is
/// demonstrated against a real reader rather than asserted.
/// </para>
/// <para>
/// It answers with one of the four classes a profile verifier may produce, and can never name
/// success as a category or an invalid state: those are core-owned, and keeping the two lists apart
/// by type is what stops a profile author believing the core will accept an outcome it cannot
/// represent.
/// </para>
/// </remarks>
public sealed class FixtureVmVerifier : IVmProfileVerifier
{
    private readonly bool chargesWork;

    /// <summary>Creates a verifier for <paramref name="profileId"/>.</summary>
    public FixtureVmVerifier(VmProfileId profileId, int semanticVersion, bool chargesWork = true)
    {
        ProfileId = profileId;
        VerifierSemanticVersion = semanticVersion;
        this.chargesWork = chargesWork;
    }

    /// <inheritdoc/>
    public VmProfileId ProfileId { get; }

    /// <inheritdoc/>
    public int BuiltAgainstCoreContractVersion => VmCoreContract.Version;

    /// <inheritdoc/>
    public int AuthoredCoreContractVersion => 1;

    /// <inheritdoc/>
    public int VerifierSemanticVersion { get; }

    /// <inheritdoc/>
    public VmVerifierOutcome Verify(
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        System.Threading.CancellationToken cancellationToken)
    {
        var adapter = new FixtureBoundedReadAdapter(context.Meter);
        var bounds = FixtureBoundedReadAdapter.ToReadBounds(context.Ceilings.VerificationCeilings);
        var reader = new VmBoundedReader(payload, in bounds, adapter);

        if (!reader.TryReadBytes(4, out var magic))
        {
            return Fail(ref reader, position: 0);
        }

        if (!System.MemoryExtensions.SequenceEqual(magic, FixtureFormat.Magic))
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.MalformedEncoding, 1001, new VmSourcePosition(-1, 0, 0, 0));
        }

        if (!reader.TryReadVarUInt32(out var formatVersion))
        {
            return Fail(ref reader, reader.Position);
        }

        if (formatVersion != FixtureFormat.FormatVersion)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.UnknownFormatVersion, 1002, new VmSourcePosition(-1, reader.Position, 0, 0));
        }

        // The descriptor and the payload must agree. A caller that mislabels bytes gets this
        // profile's deterministic validation failure rather than a search for a decoder that
        // accepts them.
        if (descriptor.FormatVersion != formatVersion)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.DescriptorMismatch, 1003, new VmSourcePosition(-1, reader.Position, 0, 0));
        }

        if (!reader.TryReadDeclaredCount(out var sectionCount))
        {
            return Fail(ref reader, reader.Position);
        }

        long[]? constants = null;
        byte[]? code = null;

        for (var index = 0; index < sectionCount; index++)
        {
            if (!reader.TryReadByte(out var kind))
            {
                return Fail(ref reader, reader.Position);
            }

            if (!reader.TryReadVarUInt64(out var declaredLength))
            {
                return Fail(ref reader, reader.Position);
            }

            if (!reader.TryEnterSection(declaredLength, out var frame))
            {
                return Fail(ref reader, reader.Position);
            }

            switch (kind)
            {
                case FixtureFormat.SectionConstants:
                    if (!TryReadConstants(ref reader, in bounds, adapter, out constants, out var constantsRefused))
                    {
                        // An allocation the guard refused is a resource answer, not a malformed
                        // one: the bytes were fine and the budget was not.
                        return constantsRefused
                            ? VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact)
                            : Fail(ref reader, reader.Position);
                    }

                    break;

                case FixtureFormat.SectionCode:
                    if (!TryReadCode(ref reader, in bounds, adapter, out code, out var codeRefused))
                    {
                        return codeRefused
                            ? VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact)
                            : Fail(ref reader, reader.Position);
                    }

                    break;

                default:
                    // An unknown section is a deterministic rejection, not a skip. Skipping would
                    // let an artifact carry content this verifier never looked at.
                    return VmVerifierOutcome.InvalidArtifact(
                        VmReason.UnknownFeature, 1004, new VmSourcePosition(index, reader.Position, kind, 0));
            }

            if (!reader.TryExitSection(in frame))
            {
                return Fail(ref reader, reader.Position);
            }
        }

        if (constants is null || code is null)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.InconsistentStructure, 1005, new VmSourcePosition(-1, reader.Position, 0, 0));
        }

        if (chargesWork && !context.Meter.TryCharge(VmBudgetDimension.VerifierWork, (ulong)code.Length))
        {
            return VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return VmVerifierOutcome.Cancellation();
        }

        if (!Validate(code, constants.Length, out var badOffset))
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.SemanticValidationFailed, 1006, new VmSourcePosition(-1, (ulong)badOffset, 0, 0));
        }

        return VmVerifierOutcome.Verified(
            new FixtureVerifiedState(constants, code),
            VmArtifactSharing.Shareable);
    }

    /// <summary>
    /// Checks that every opcode is known, every operand is present, and every constant index is in
    /// range - so nothing unverified can be reached by the executor.
    /// </summary>
    private static bool Validate(byte[] code, int constantCount, out int badOffset)
    {
        var offset = 0;

        while (offset < code.Length)
        {
            var opcode = code[offset];
            offset++;

            switch (opcode)
            {
                case FixtureFormat.OpNop:
                case FixtureFormat.OpAdd:
                case FixtureFormat.OpSub:
                case FixtureFormat.OpMul:
                case FixtureFormat.OpYield:
                case FixtureFormat.OpReturn:
                    continue;

                case FixtureFormat.OpPushConst:
                case FixtureFormat.OpFault:
                case FixtureFormat.OpLoad:
                case FixtureFormat.OpSpin:
                case FixtureFormat.OpAllocate:
                case FixtureFormat.OpRetain:
                case FixtureFormat.OpRelease:
                    if (offset >= code.Length)
                    {
                        badOffset = offset;
                        return false;
                    }

                    if (code[offset] >= constantCount)
                    {
                        badOffset = offset;
                        return false;
                    }

                    offset++;
                    continue;

                case FixtureFormat.OpHostCall:
                    if (offset >= code.Length)
                    {
                        badOffset = offset;
                        return false;
                    }

                    offset++;
                    continue;

                default:
                    badOffset = offset - 1;
                    return false;
            }
        }

        badOffset = 0;
        return true;
    }

    private static bool TryReadConstants(
        ref VmBoundedReader reader,
        in VmReadBounds bounds,
        IVmBoundedAllocationMeter meter,
        out long[]? constants,
        out bool allocationRefused)
    {
        constants = null;
        allocationRefused = false;

        if (!reader.TryReadDeclaredCount(out var count))
        {
            return false;
        }

        // The guard refuses before allocating. A declared count above its bound therefore costs
        // nothing proportional to the number an attacker wrote down.
        if (!VmBoundedAllocator.TryAllocate<long>(in bounds, meter, count, out var buffer))
        {
            allocationRefused = true;
            return false;
        }

        for (var index = 0; index < count; index++)
        {
            if (!reader.TryReadVarUInt64(out var value))
            {
                return false;
            }

            buffer[index] = unchecked((long)value);
        }

        constants = buffer;
        return true;
    }

    private static bool TryReadCode(
        ref VmBoundedReader reader,
        in VmReadBounds bounds,
        IVmBoundedAllocationMeter meter,
        out byte[]? code,
        out bool allocationRefused)
    {
        code = null;
        allocationRefused = false;

        if (!reader.TryReadDeclaredCount(out var length))
        {
            return false;
        }

        if (!VmBoundedAllocator.TryAllocate<byte>(in bounds, meter, length, out var buffer))
        {
            allocationRefused = true;
            return false;
        }

        if (!reader.TryReadBytes(length, out var body))
        {
            return false;
        }

        body.CopyTo(buffer);
        code = buffer;
        return true;
    }

    /// <summary>
    /// Maps the reader's mechanism status onto the contract's categories. This mapping is the whole
    /// of "bounded reading is mechanism and must not acquire contract vocabulary".
    /// </summary>
    private static VmVerifierOutcome Fail(ref VmBoundedReader reader, ulong position) =>
        reader.Status switch
        {
            VmBoundedReadStatus.Truncated =>
                VmVerifierOutcome.InvalidArtifact(VmReason.Truncated, 2001, new VmSourcePosition(-1, position, 0, 0)),

            VmBoundedReadStatus.MalformedEncoding =>
                VmVerifierOutcome.InvalidArtifact(VmReason.MalformedEncoding, 2002, new VmSourcePosition(-1, position, 0, 0)),

            VmBoundedReadStatus.SectionCountExceeded =>
                VmVerifierOutcome.InvalidArtifact(VmReason.InconsistentStructure, 2003, new VmSourcePosition(-1, position, 0, 0)),

            VmBoundedReadStatus.StructuralDepthExceeded =>
                VmVerifierOutcome.InvalidArtifact(VmReason.InconsistentStructure, 2004, new VmSourcePosition(-1, position, 0, 0)),

            VmBoundedReadStatus.DeclaredCountExceeded =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact),

            VmBoundedReadStatus.ArtifactBytesExceeded =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.ArtifactBytes, VmBudgetScope.Artifact),

            VmBoundedReadStatus.AllocationRefused =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact),

            VmBoundedReadStatus.WorkBudgetExhausted =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact),

            _ => VmVerifierOutcome.InvalidArtifact(
                VmReason.InconsistentStructure, 2000, new VmSourcePosition(-1, position, 0, 0)),
        };
}
