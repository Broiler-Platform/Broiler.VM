using Broiler.VM;

namespace Com.Example.Calculator;

/// <summary>The calculator profile's immutable decoded artifact.</summary>
/// <remarks>
/// Everything reachable from it is immutable once verification returns, which is what makes the
/// handle safe for unsynchronised concurrent readers in two runtimes at once. The arrays are never
/// handed out; only the counts are.
/// </remarks>
public sealed class CalculatorProgram : IVmVerifiedState
{
    internal CalculatorProgram(long[] operands, byte[] tokens, int maximumDepth)
    {
        Operands = operands;
        Tokens = tokens;
        MaximumDepth = maximumDepth;
    }

    internal long[] Operands { get; }

    internal byte[] Tokens { get; }

    /// <summary>How deep the evaluation stack goes, computed once at verification.</summary>
    public int MaximumDepth { get; }

    /// <summary>How many operands the pool holds.</summary>
    public int OperandCount => Operands.Length;

    /// <summary>How many tokens the program holds.</summary>
    public int TokenCount => Tokens.Length;
}

/// <summary>
/// The calculator profile's verifier: a decoder over its own format, driven through the core's
/// bounded reader.
/// </summary>
/// <remarks>
/// <para>
/// This is the whole of what a profile author writes against the public source contract. Every
/// length, count and offset goes through <see cref="VmBoundedReader"/>, the operand pool is
/// allocated through <see cref="VmBoundedAllocator"/>, and the four artifact-shaped ceilings are
/// projected out of the effective policy the core hands over - a projection this profile writes
/// itself, because the bounded-reading assembly deliberately names no contract vocabulary.
/// </para>
/// <para>
/// It reaches nothing in <c>Broiler.VM.Runtime</c>: it has no reference to that assembly, cannot
/// name a runtime, an instance or a catalog, and could not construct a verified handle if it wanted
/// to. That is the promise, and it is a property of the reference set rather than of good
/// behaviour.
/// </para>
/// <para>
/// Stack depth is computed at verification and stored, so execution never grows a stack it did not
/// already prove bounded. A program whose depth exceeds the profile's own maximum, or that
/// underflows, is refused before it can run rather than faulting halfway through.
/// </para>
/// </remarks>
public sealed class CalculatorVerifier : IVmProfileVerifier
{
    /// <summary>Creates the verifier for <paramref name="profileId"/>.</summary>
    public CalculatorVerifier(VmProfileId profileId) => ProfileId = profileId;

    /// <inheritdoc/>
    public VmProfileId ProfileId { get; }

    /// <inheritdoc/>
    public int BuiltAgainstCoreContractVersion => VmCoreContract.Version;

    /// <inheritdoc/>
    public int AuthoredCoreContractVersion => 1;

    /// <inheritdoc/>
    public int VerifierSemanticVersion => 1;

    /// <inheritdoc/>
    public VmVerifierOutcome Verify(
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        System.Threading.CancellationToken cancellationToken)
    {
        var meter = new CalculatorReadAdapter(context.Meter);
        var bounds = CalculatorReadAdapter.ToReadBounds(context.Ceilings.VerificationCeilings);
        var reader = new VmBoundedReader(payload, in bounds, meter);

        if (!reader.TryReadBytes(4, out var magic))
        {
            return Failed(ref reader, 0);
        }

        if (!System.MemoryExtensions.SequenceEqual(magic, CalculatorFormat.Magic))
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.MalformedEncoding, DiagnosticCodes.WrongMagic, At(0));
        }

        if (!reader.TryReadVarUInt32(out var formatVersion))
        {
            return Failed(ref reader, reader.Position);
        }

        if (formatVersion != CalculatorFormat.FormatVersion)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.UnknownFormatVersion, DiagnosticCodes.UnknownFormatVersion, At(reader.Position));
        }

        // The descriptor and the payload must agree. A caller that mislabels bytes gets this
        // profile's deterministic validation failure rather than a search for a decoder that
        // accepts them.
        if (descriptor.FormatVersion != formatVersion)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.DescriptorMismatch, DiagnosticCodes.DescriptorMismatch, At(reader.Position));
        }

        if (!reader.TryReadDeclaredCount(out var operandCount))
        {
            return Failed(ref reader, reader.Position);
        }

        if (!VmBoundedAllocator.TryAllocate<long>(in bounds, meter, operandCount, out var operands))
        {
            // The guard refused before allocating, so a hostile count cost nothing proportional to
            // itself. The bytes were fine and the budget was not, which is a resource answer.
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact);
        }

        for (var index = 0; index < operandCount; index++)
        {
            if (!reader.TryReadVarUInt64(out var encoded))
            {
                return Failed(ref reader, reader.Position);
            }

            // Zigzag, so a small negative operand costs one byte rather than ten. The core has no
            // opinion about this: it is the profile's own encoding of its own value model.
            operands[index] = (long)(encoded >> 1) ^ -(long)(encoded & 1);
        }

        if (!reader.TryReadDeclaredCount(out var tokenCount))
        {
            return Failed(ref reader, reader.Position);
        }

        if (!VmBoundedAllocator.TryAllocate<byte>(in bounds, meter, tokenCount, out var tokens))
        {
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact);
        }

        if (!reader.TryReadBytes(tokenCount, out var body))
        {
            return Failed(ref reader, reader.Position);
        }

        body.CopyTo(tokens);

        if (reader.Remaining != 0)
        {
            // Trailing bytes are a structural error and not something to ignore. Ignoring them
            // would let one artifact carry content this verifier never looked at.
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.InconsistentStructure, DiagnosticCodes.TrailingBytes, At(reader.Position));
        }

        if (!reader.TryChargeWork(tokenCount))
        {
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return VmVerifierOutcome.Cancellation();
        }

        if (!TryValidate(tokens, operands.Length, out var depth, out var badOffset, out var reason))
        {
            return VmVerifierOutcome.InvalidArtifact(reason, DiagnosticCodes.SemanticFailure, At((ulong)badOffset));
        }

        return VmVerifierOutcome.Verified(
            new CalculatorProgram(operands, tokens, depth), VmArtifactSharing.Shareable);
    }

    /// <summary>
    /// Walks the program once, checking that every token is known, every operand index is in range,
    /// the stack never underflows, and its depth stays inside the profile's own maximum.
    /// </summary>
    /// <remarks>
    /// The depth it computes is returned and stored on the verified state, so the executor allocates
    /// a stack of a size verification already proved sufficient. An executor that sized its stack
    /// from a number the payload chose would be sizing it from untrusted data after verification,
    /// which is the shape this whole boundary exists to prevent.
    /// </remarks>
    private static bool TryValidate(
        byte[] tokens,
        int operandCount,
        out int maximumDepth,
        out int badOffset,
        out VmReason reason)
    {
        maximumDepth = 0;
        badOffset = 0;
        reason = VmReason.SemanticValidationFailed;

        var depth = 0;
        var offset = 0;
        var halted = false;

        while (offset < tokens.Length)
        {
            var token = tokens[offset];
            badOffset = offset;
            offset++;

            switch (token)
            {
                case CalculatorFormat.TokenPush:
                    if (offset >= tokens.Length || tokens[offset] >= operandCount)
                    {
                        badOffset = offset;
                        return false;
                    }

                    offset++;
                    depth++;

                    if (depth > CalculatorFormat.MaximumStackDepth)
                    {
                        return false;
                    }

                    maximumDepth = depth > maximumDepth ? depth : maximumDepth;
                    continue;

                case CalculatorFormat.TokenAdd:
                case CalculatorFormat.TokenMultiply:
                case CalculatorFormat.TokenDivide:
                    if (depth < 2)
                    {
                        return false;
                    }

                    depth--;
                    continue;

                case CalculatorFormat.TokenNegate:
                    if (depth < 1)
                    {
                        return false;
                    }

                    continue;

                case CalculatorFormat.TokenHalt:
                    if (depth < 1 || offset != tokens.Length)
                    {
                        // Halt is the last token and leaves exactly the answer. A program with
                        // anything after it has code the executor would never reach, which is a
                        // structural error rather than a harmless tail.
                        return false;
                    }

                    halted = true;
                    continue;

                default:
                    reason = VmReason.UnknownFeature;
                    badOffset = offset - 1;
                    return false;
            }
        }

        if (!halted)
        {
            reason = VmReason.Truncated;
            badOffset = tokens.Length;
            return false;
        }

        return true;
    }

    /// <summary>Maps the reader's mechanism status onto the four answers a verifier may give.</summary>
    /// <remarks>
    /// The bounded reader names no contract vocabulary, so this mapping is the profile's own. It is
    /// the visible price of keeping the mechanism assembly free of the contract, and it is four
    /// lines.
    /// </remarks>
    private static VmVerifierOutcome Failed(ref VmBoundedReader reader, ulong position) =>
        reader.Status switch
        {
            VmBoundedReadStatus.Truncated =>
                VmVerifierOutcome.InvalidArtifact(VmReason.Truncated, DiagnosticCodes.Truncated, At(position)),

            VmBoundedReadStatus.MalformedEncoding =>
                VmVerifierOutcome.InvalidArtifact(VmReason.MalformedEncoding, DiagnosticCodes.MalformedEncoding, At(position)),

            VmBoundedReadStatus.DeclaredCountExceeded =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact),

            VmBoundedReadStatus.ArtifactBytesExceeded =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.ArtifactBytes, VmBudgetScope.Artifact),

            VmBoundedReadStatus.AllocationRefused =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact),

            VmBoundedReadStatus.WorkBudgetExhausted =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact),

            _ => VmVerifierOutcome.InvalidArtifact(
                VmReason.InconsistentStructure, DiagnosticCodes.ReaderStopped, At(position)),
        };

    private static VmSourcePosition At(ulong offset) => new(-1, offset, 0, 0);

    /// <summary>This profile's own stable diagnostic codes. The core attaches no meaning to them.</summary>
    internal static class DiagnosticCodes
    {
        internal const int WrongMagic = 7001;
        internal const int UnknownFormatVersion = 7002;
        internal const int DescriptorMismatch = 7003;
        internal const int TrailingBytes = 7004;
        internal const int SemanticFailure = 7005;
        internal const int Truncated = 7101;
        internal const int MalformedEncoding = 7102;
        internal const int ReaderStopped = 7103;
    }
}

/// <summary>
/// The projection every profile writes between the contract's metering surface and the bounded
/// reader's.
/// </summary>
/// <remarks>
/// <c>Broiler.VM.Binary</c> names no contract vocabulary, so the party holding both performs the
/// projection. For this profile that is these four lines and one mapping from a limit vector onto
/// four numbers - which is also the whole of what a profile author has to understand about the
/// arrangement.
/// </remarks>
public sealed class CalculatorReadAdapter : IVmBoundedAllocationMeter
{
    private readonly IVmMeter meter;

    /// <summary>Wraps the contract meter the core supplied.</summary>
    public CalculatorReadAdapter(IVmMeter contractMeter) => meter = contractMeter;

    /// <summary>Projects the four artifact-shaped ceilings out of an effective limit vector.</summary>
    public static VmReadBounds ToReadBounds(VmLimitVector limits) =>
        new(
            limits[VmBudgetDimension.ArtifactBytes],
            limits[VmBudgetDimension.SectionCount],
            limits[VmBudgetDimension.DeclaredCount],
            limits[VmBudgetDimension.StructuralDepth]);

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
