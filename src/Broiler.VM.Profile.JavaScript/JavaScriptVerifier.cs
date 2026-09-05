// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   32
// Annotated:        32/32
// Exempt:           28
// Human-reviewed:   0/32
// IP risk:          Low
// Security risk:    High
// Criteria:         12/12
// Resource impact:  7/10 max
// Unverified:       32
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>One named entry point and the code offset it starts at.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=68B1D6
// Broiler-Human:        PENDING
public readonly struct JavaScriptEntryPoint
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0111F6
    // Broiler-Human:        PENDING
    internal JavaScriptEntryPoint(byte[] name, int codeOffset)
    {
        Name = name;
        CodeOffset = codeOffset;
    }

    /// <summary>The entry point's name, as the UTF-8 bytes the artifact carried.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8D0F79
    // Broiler-Human:        PENDING
    internal byte[] Name { get; }

    /// <summary>Where in the code section it starts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EBDD87
    // Broiler-Human:        PENDING
    public int CodeOffset { get; }
}

/// <summary>
/// The immutable decoded program a successful verification produces.
/// </summary>
/// <remarks>
/// <para>
/// Everything reachable from it is immutable once verification returns, which is what makes a
/// shareable handle safe for two runtimes reading it at once with no synchronisation between
/// them. No array is handed out; the executor reaches them through internal members and a
/// consumer sees counts.
/// </para>
/// <para>
/// <b>Nothing warmed, mutable or process-local is reachable from here</b>, and that is a property
/// of the design rather than of current contents: there is no inline-cache slot, no shape table,
/// no interned identity and no feedback vector, because the slice surface has no construct that
/// would want one. Invariant 7 is what that property is called, and JS-4 is where keeping it
/// stops being free.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=7FCF90
// Broiler-Falsified-If: anything reachable from this state can be mutated after verification returns, or two runtimes sharing one handle observe each other through it
// Broiler-Human:        PENDING
public sealed class JavaScriptProgram : IVmVerifiedState
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=914B29
    // Broiler-Human:        PENDING
    internal JavaScriptProgram(
        JavaScriptValue[] constants,
        byte[] code,
        JavaScriptEntryPoint[] entries,
        int maximumOperandStack,
        int localCount,
        int positionRowCount)
    {
        Constants = constants;
        Code = code;
        Entries = entries;
        MaximumOperandStack = maximumOperandStack;
        LocalCount = localCount;
        PositionRowCount = positionRowCount;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=25C081
    // Broiler-Human:        PENDING
    internal JavaScriptValue[] Constants { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0F8DCF
    // Broiler-Human:        PENDING
    internal byte[] Code { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F84F0B
    // Broiler-Human:        PENDING
    internal JavaScriptEntryPoint[] Entries { get; }

    /// <summary>
    /// The deepest the operand stack goes, computed at verification and stored here.
    /// </summary>
    /// <remarks>
    /// The executor sizes its stack from this and never from a number the payload chose. A
    /// declared maximum in the artifact is checked against it; it does not become it. That
    /// difference is the whole of why a fixed-size stack is safe here.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F5492F
    // Broiler-Human:        PENDING
    public int MaximumOperandStack { get; }

    /// <summary>How many local slots one frame has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D6473D
    // Broiler-Human:        PENDING
    public int LocalCount { get; }

    /// <summary>How many constants the pool holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7AB86D
    // Broiler-Human:        PENDING
    public int ConstantCount => Constants.Length;

    /// <summary>How many bytes the code section holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=83A2FC
    // Broiler-Human:        PENDING
    public int CodeLength => Code.Length;

    /// <summary>How many entry points the artifact declares.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EDA3C1
    // Broiler-Human:        PENDING
    public int EntryPointCount => Entries.Length;

    /// <summary>How many rows the canonical position table holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=79BFAA
    // Broiler-Human:        PENDING
    public int PositionRowCount { get; }

    /// <summary>Finds the entry point named by <paramref name="utf8Name"/>.</summary>
    /// <remarks>
    /// A linear scan over a table the format caps at a few hundred entries. A dictionary would be
    /// a mutable object reachable from a shared handle unless it were frozen, and freezing one is
    /// more machinery than a bounded scan.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A308EB
    // Broiler-Human:        PENDING
    internal bool TryFindEntry(System.ReadOnlySpan<byte> utf8Name, out int codeOffset)
    {
        for (var index = 0; index < Entries.Length; index++)
        {
            if (System.MemoryExtensions.SequenceEqual(
                System.MemoryExtensions.AsSpan(Entries[index].Name), utf8Name))
            {
                codeOffset = Entries[index].CodeOffset;
                return true;
            }
        }

        codeOffset = 0;
        return false;
    }
}

/// <summary>
/// The JavaScript profile's verifier: the one trust boundary, reached only through the core's one
/// verification entry point.
/// </summary>
/// <remarks>
/// <para>
/// <b>Verification is total.</b> It answers; it does not throw. Every rejection is one of the five
/// outcomes the core admits, carrying this profile's own diagnostic code and a position. An
/// exception escaping here would be a contract violation of this component and not a rejection.
/// </para>
/// <para>
/// <b>A structural check happens here or it does not happen.</b> No index check, stack-consistency
/// rule or boundary rule migrates into first execution. A late check reported as a language fault
/// would make a malformed artifact indistinguishable from a program that threw, and would hollow
/// out the corpus that is supposed to prove this boundary.
/// </para>
/// <para>
/// <b>Two outcome categories appear below and the split is the core's ruling, not a preference.</b>
/// A malformed or ill-typed artifact is an invalid artifact carrying a diagnostic code and a
/// position; a breach of an effective ceiling naming a budget dimension is a resource exhaustion
/// naming that dimension and its scope. Conflating them tells a caller its program is malformed
/// when the truth is that this host declined to spend the memory - and because every retained
/// corpus entry pins its observed triple, a miscategorised entry does not fail later, it passes
/// and records the wrong answer.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=6DB9F8
// Broiler-Falsified-If: any input makes Verify throw, or a check this class performs can be reached for the first time during execution
// Broiler-Human:        PENDING
public sealed class JavaScriptVerifier : IVmProfileVerifier
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DA99B9
    // Broiler-Human:        PENDING
    private readonly VmFeatureManifestId acceptedManifest;

    /// <summary>The optional surfaces the composition that built this descriptor admitted.</summary>
    /// <remarks>
    /// It is a field of the one verifier object rather than a parameter of the pass, because a
    /// composition's answer is fixed when it registers its descriptor and a verifier that could be
    /// asked twice could be given two answers.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=60EEF1
    // Broiler-Falsified-If: this set differs from the accepted feature manifests of the descriptor that carries this verifier
    // Broiler-Human:        PENDING
    private readonly System.Collections.Immutable.ImmutableArray<string> surfaces;

    /// <summary>Creates the verifier for one profile identity and its one accepted manifest.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2C702C
    // Broiler-Human:        PENDING
    public JavaScriptVerifier(
        VmProfileId profileId,
        VmFeatureManifestId manifest,
        System.Collections.Immutable.ImmutableArray<string> admittedSurfaces)
    {
        ProfileId = profileId;
        acceptedManifest = manifest;
        surfaces = admittedSurfaces;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CCA6CF
    // Broiler-Human:        PENDING
    public VmProfileId ProfileId { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=91327C
    // Broiler-Human:        PENDING
    public int BuiltAgainstCoreContractVersion => VmCoreContract.Version;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8BCF43
    // Broiler-Human:        PENDING
    public int AuthoredCoreContractVersion => 1;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=47E01B
    // Broiler-Human:        PENDING
    public int VerifierSemanticVersion => 1;

    /// <inheritdoc/>
    /// <remarks>
    /// The profile-identity check is the FIRST statement and the reader is not constructed before
    /// it, so an artifact naming a profile this verifier does not host is answered without
    /// examining a payload byte. That ordering is asserted by a named case rather than left to be
    /// read off this file.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=1557EA
    // Broiler-Falsified-If: a payload byte is read on a path that answers UnsupportedProfile
    // Broiler-Human:        PENDING
    public VmVerifierOutcome Verify(
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        System.Threading.CancellationToken cancellationToken)
    {
        if (descriptor.ProfileId != ProfileId)
        {
            return VmVerifierOutcome.UnsupportedProfile();
        }

        // TWO SURFACES, TWO FORMAT VERSIONS, ONE VERIFIER OBJECT. The descriptor names the version
        // the caller says these bytes are, and the payload names the version they actually are;
        // the version-2 pass checks the second against the first exactly as this one does. What is
        // decided here is only which pass reads them, and it is decided from the descriptor,
        // because reading a payload byte to find out how to read the payload is the one ordering
        // this component refuses to have.
        if (descriptor.FormatVersion == Format.JsFormat.FormatVersion)
        {
            return JsVerifier.Verify(in descriptor, payload, context, surfaces, cancellationToken);
        }

        var adapter = new JavaScriptReadAdapter(context.Meter);
        var bounds = JavaScriptReadAdapter.ToReadBounds(context.Ceilings.VerificationCeilings);
        var reader = new VmBoundedReader(payload, in bounds, adapter);

        return VerifyCore(in descriptor, ref reader, in bounds, adapter, cancellationToken);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A48C81
    // Broiler-Human:        PENDING
    private VmVerifierOutcome VerifyCore(
        in VmArtifactDescriptor descriptor,
        ref VmBoundedReader reader,
        in VmReadBounds bounds,
        JavaScriptReadAdapter adapter,
        System.Threading.CancellationToken cancellationToken)
    {
        if (!reader.TryReadBytes(4, out var magic))
        {
            return FromReader(ref reader, 0);
        }

        if (!System.MemoryExtensions.SequenceEqual(magic, JavaScriptFormat.Magic))
        {
            return Invalid(VmReason.MalformedEncoding, JavaScriptDiagnosticCode.WrongMagic, 0);
        }

        if (!reader.TryReadVarUInt32(out var formatVersion))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (formatVersion is < JavaScriptFormat.MinimumFormatVersion or > JavaScriptFormat.MaximumFormatVersion)
        {
            return Invalid(
                VmReason.UnsupportedProfileFormatVersion,
                JavaScriptDiagnosticCode.UnsupportedFormatVersion,
                reader.Position);
        }

        if (descriptor.FormatVersion != formatVersion)
        {
            return Invalid(
                VmReason.DescriptorMismatch,
                JavaScriptDiagnosticCode.DescriptorFormatVersionMismatch,
                reader.Position);
        }

        var manifestOutcome = ReadAndCheckManifest(in descriptor, ref reader);

        if (manifestOutcome.Category != VmOutcome.Normal)
        {
            return manifestOutcome;
        }

        var sections = new SectionSet();

        if (!reader.TryReadDeclaredCount(out var sectionCount))
        {
            return FromReader(ref reader, reader.Position);
        }

        var previousKind = 0u;

        for (var index = 0u; index < sectionCount; index++)
        {
            if (!adapter.Poll())
            {
                return Stopped(cancellationToken);
            }

            var sectionOutcome = ReadSection(
                ref reader, in bounds, adapter, ref previousKind, ref sections, index, cancellationToken);

            if (sectionOutcome.Category != VmOutcome.Normal)
            {
                return sectionOutcome;
            }
        }

        if (reader.Remaining != 0)
        {
            return Invalid(
                VmReason.InconsistentStructure, JavaScriptDiagnosticCode.TrailingBytes, reader.Position);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return VmVerifierOutcome.Cancellation();
        }

        return Link(ref sections, in bounds, adapter);
    }

    /// <summary>
    /// Reads the feature-manifest identity out of the payload and holds it to two things: the
    /// manifest this descriptor names, and the one this verifier accepts.
    /// </summary>
    /// <remarks>
    /// The two checks answer different questions and both are needed. The payload-against-verifier
    /// check says this build does not implement that surface; the payload-against-descriptor check
    /// says the caller mislabelled these bytes. Reporting either as the other would send a reader
    /// looking in the wrong place.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=5533C9
    // Broiler-Falsified-If: an artifact naming an unaccepted manifest verifies, or the two mismatches report the same diagnostic code
    // Broiler-Human:        PENDING
    private VmVerifierOutcome ReadAndCheckManifest(in VmArtifactDescriptor descriptor, ref VmBoundedReader reader)
    {
        var at = reader.Position;

        if (!reader.TryReadVarUInt32(out var manifestLength))
        {
            return FromReader(ref reader, at);
        }

        if (manifestLength > JavaScriptFormat.MaximumManifestIdBytes)
        {
            return Invalid(
                VmReason.InconsistentStructure, JavaScriptDiagnosticCode.ManifestIdTooLong, reader.Position);
        }

        if (!reader.TryReadBytes(manifestLength, out var manifestBytes))
        {
            return FromReader(ref reader, reader.Position);
        }

        // The identity is compared as UTF-8 bytes against the accepted identity's own UTF-8, so a
        // payload that is not valid UTF-8 simply fails to match rather than reaching a decoder
        // that could throw on it. A verifier that decoded first would have to answer for the
        // decoder's exceptions, which is a second failure mode for no gain.
        System.Span<byte> accepted = stackalloc byte[VmFeatureManifestId.MaximumLength];
        var acceptedLength = System.Text.Encoding.UTF8.GetBytes(acceptedManifest.AsSpan(), accepted);

        if (!System.MemoryExtensions.SequenceEqual(manifestBytes, accepted[..acceptedLength]))
        {
            return Invalid(
                VmReason.UnsupportedFeatureManifest,
                JavaScriptDiagnosticCode.UnsupportedFeatureManifest,
                reader.Position);
        }

        if (descriptor.FeatureManifestId != acceptedManifest)
        {
            return Invalid(
                VmReason.DescriptorMismatch,
                JavaScriptDiagnosticCode.DescriptorManifestMismatch,
                reader.Position);
        }

        return Ok;
    }

    /// <summary>Frames one section and parses its body into <paramref name="sections"/>.</summary>
    /// <remarks>
    /// Ascending and unique kinds are enforced here rather than after the loop, so a duplicate is
    /// refused before its body is read. An unknown kind is refused outright: skipping it would let
    /// one artifact carry content this verifier never looked at.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=2FA2BD
    // Broiler-Falsified-If: a section body is read before its kind's order and uniqueness are checked, or an unknown kind is skipped rather than refused
    // Broiler-Human:        PENDING
    private VmVerifierOutcome ReadSection(
        ref VmBoundedReader reader,
        in VmReadBounds bounds,
        JavaScriptReadAdapter adapter,
        ref uint previousKind,
        ref SectionSet sections,
        uint sectionIndex,
        System.Threading.CancellationToken cancellationToken)
    {
        var at = reader.Position;

        if (!reader.TryReadVarUInt32(out var kind))
        {
            return FromReader(ref reader, at);
        }

        if (!reader.TryReadVarUInt64(out var length))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (kind <= previousKind)
        {
            return Invalid(VmReason.InconsistentStructure, JavaScriptDiagnosticCode.SectionOrder, at);
        }

        if (!IsKnownSectionKind(kind))
        {
            return Invalid(VmReason.UnknownFeature, JavaScriptDiagnosticCode.UnknownSectionKind, at);
        }

        previousKind = kind;

        if (!reader.TryEnterSection(length, out var frame))
        {
            return FromReader(ref reader, reader.Position);
        }

        var body = (JavaScriptFormat.SectionKind)kind switch
        {
            JavaScriptFormat.SectionKind.Limits => ReadLimits(ref reader, ref sections),
            JavaScriptFormat.SectionKind.Constants => ReadConstants(ref reader, in bounds, adapter, ref sections, cancellationToken),
            JavaScriptFormat.SectionKind.Code => ReadCode(
                ref reader, in bounds, adapter, length, ref sections, sectionIndex),
            JavaScriptFormat.SectionKind.Entries => ReadEntries(ref reader, ref sections),
            JavaScriptFormat.SectionKind.ExceptionRegions => ReadReserved(
                ref reader, JavaScriptDiagnosticCode.ExceptionRegionOutsideManifest),
            JavaScriptFormat.SectionKind.SuspensionTargets => ReadReserved(
                ref reader, JavaScriptDiagnosticCode.SuspensionTargetOutsideManifest),
            _ => ReadPositions(ref reader, ref sections),
        };

        if (body.Category != VmOutcome.Normal)
        {
            return body;
        }

        if (!reader.TryExitSection(in frame))
        {
            // Consuming less than declared is as structural an error as consuming more: it means
            // the artifact and this verifier disagree about where the next section begins, which
            // is exactly the confusion framing exists to prevent.
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.SectionLengthMismatch,
                reader.Position);
        }

        return Ok;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2D8348
    // Broiler-Human:        PENDING
    private static bool IsKnownSectionKind(uint kind) => kind switch
    {
        (uint)JavaScriptFormat.SectionKind.Limits or
        (uint)JavaScriptFormat.SectionKind.Constants or
        (uint)JavaScriptFormat.SectionKind.Code or
        (uint)JavaScriptFormat.SectionKind.Entries or
        (uint)JavaScriptFormat.SectionKind.ExceptionRegions or
        (uint)JavaScriptFormat.SectionKind.SuspensionTargets or
        (uint)JavaScriptFormat.SectionKind.Positions => true,
        _ => false,
    };

    /// <summary>Reads the four declared maxima and holds each to this format's own ceiling.</summary>
    /// <remarks>
    /// The comparison happens here, before any of these numbers reaches a use. A declared maximum
    /// that sized something before it was compared would be a number from untrusted bytes deciding
    /// an allocation, which is the shape this whole boundary exists to prevent.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=521C64
    // Broiler-Falsified-If: a declared maximum is used before its ceiling comparison
    // Broiler-Human:        PENDING
    private VmVerifierOutcome ReadLimits(ref VmBoundedReader reader, ref SectionSet sections)
    {
        if (!reader.TryReadVarUInt32(out var operandStack) ||
            !reader.TryReadVarUInt32(out var locals) ||
            !reader.TryReadVarUInt32(out var frames) ||
            !reader.TryReadVarUInt32(out var constants))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (operandStack > JavaScriptFormat.CeilingOperandStack ||
            locals > JavaScriptFormat.CeilingLocals ||
            constants > JavaScriptFormat.CeilingConstants)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.DeclaredMaximumTooLarge,
                reader.Position);
        }

        if (frames != JavaScriptFormat.CeilingFrames)
        {
            // The slice has no functions, so it has exactly one frame. An artifact declaring two
            // is declaring a surface this manifest does not admit, and it is refused here rather
            // than at the first call that would have needed the second frame.
            return Invalid(
                VmReason.UnknownFeature, JavaScriptDiagnosticCode.DeclaredFrameCount, reader.Position);
        }

        sections.HasLimits = true;
        sections.MaxOperandStack = (int)operandStack;
        sections.LocalCount = (int)locals;
        sections.MaxConstants = (int)constants;
        return Ok;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=518AD1
    // Broiler-Human:        PENDING
    private VmVerifierOutcome ReadConstants(
        ref VmBoundedReader reader,
        in VmReadBounds bounds,
        JavaScriptReadAdapter adapter,
        ref SectionSet sections,
        System.Threading.CancellationToken cancellationToken)
    {
        if (!reader.TryReadDeclaredCount(out var count))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (!sections.HasLimits)
        {
            return Invalid(
                VmReason.InconsistentStructure, JavaScriptDiagnosticCode.MissingSection, reader.Position);
        }

        if (count > sections.MaxConstants)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.ConstantCountExceedsDeclaredMaximum,
                reader.Position);
        }

        if (!VmBoundedAllocator.TryAllocate<JavaScriptValue>(in bounds, adapter, count, out var constants))
        {
            // The guard refused before allocating, so a hostile count cost nothing proportional to
            // itself. The bytes were well formed and this host declined to spend the memory, which
            // is a resource answer and not a malformed one.
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact);
        }

        for (var index = 0u; index < count; index++)
        {
            if ((index & 0x3F) == 0 && cancellationToken.IsCancellationRequested)
            {
                return VmVerifierOutcome.Cancellation();
            }

            var at = reader.Position;

            if (!reader.TryReadByte(out var tag))
            {
                return FromReader(ref reader, at);
            }

            switch ((JavaScriptFormat.ConstantTag)tag)
            {
                case JavaScriptFormat.ConstantTag.Undefined:
                    constants[index] = JavaScriptValue.Undefined;
                    continue;

                case JavaScriptFormat.ConstantTag.Boolean:
                    if (!reader.TryReadByte(out var flag))
                    {
                        return FromReader(ref reader, reader.Position);
                    }

                    if (flag > 1)
                    {
                        return Invalid(
                            VmReason.MalformedEncoding,
                            JavaScriptDiagnosticCode.MalformedBooleanConstant,
                            reader.Position);
                    }

                    constants[index] = JavaScriptValue.Boolean(flag == 1);
                    continue;

                case JavaScriptFormat.ConstantTag.Number:
                    if (!reader.TryReadUInt64LittleEndian(out var bits))
                    {
                        return FromReader(ref reader, reader.Position);
                    }

                    constants[index] = JavaScriptValue.Number(System.BitConverter.Int64BitsToDouble((long)bits));
                    continue;

                case JavaScriptFormat.ConstantTag.InternedName:
                    // The format reserves it from version 1 and no manifest admits it. Refusing it
                    // HERE rather than at a first property access is what makes "a construct
                    // outside the declared manifest is rejected at verification" a rule with a
                    // case behind it.
                    return Invalid(
                        VmReason.UnknownFeature,
                        JavaScriptDiagnosticCode.InternedNameOutsideManifest,
                        at);

                default:
                    return Invalid(
                        VmReason.UnknownFeature, JavaScriptDiagnosticCode.UnknownConstantTag, at);
            }
        }

        sections.HasConstants = true;
        sections.Constants = constants;
        return Ok;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4B9AB8
    // Broiler-Human:        PENDING
    private VmVerifierOutcome ReadCode(
        ref VmBoundedReader reader,
        in VmReadBounds bounds,
        JavaScriptReadAdapter adapter,
        ulong declaredLength,
        ref SectionSet sections,
        uint sectionIndex)
    {
        if (declaredLength == 0)
        {
            return Invalid(VmReason.InconsistentStructure, JavaScriptDiagnosticCode.EmptyCode, reader.Position);
        }

        if (declaredLength > bounds.MaxArtifactBytes)
        {
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.ArtifactBytes, VmBudgetScope.Artifact);
        }

        if (!VmBoundedAllocator.TryAllocateExact<byte>(in bounds, adapter, declaredLength, out var code))
        {
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact);
        }

        if (!reader.TryReadBytes(declaredLength, out var body))
        {
            return FromReader(ref reader, reader.Position);
        }

        body.CopyTo(code);
        sections.HasCode = true;
        sections.Code = code;
        sections.CodeSectionIndex = (int)sectionIndex;
        return Ok;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FE3341
    // Broiler-Human:        PENDING
    private VmVerifierOutcome ReadEntries(ref VmBoundedReader reader, ref SectionSet sections)
    {
        if (!reader.TryReadDeclaredCount(out var count))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (count is 0 or > JavaScriptFormat.CeilingEntries)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                count == 0 ? JavaScriptDiagnosticCode.NoEntryPoint : JavaScriptDiagnosticCode.DeclaredMaximumTooLarge,
                reader.Position);
        }

        var entries = new JavaScriptEntryPoint[count];

        for (var index = 0u; index < count; index++)
        {
            var at = reader.Position;

            if (!reader.TryReadVarUInt32(out var nameLength))
            {
                return FromReader(ref reader, at);
            }

            if (nameLength is 0 or > JavaScriptFormat.MaximumEntryNameBytes)
            {
                return Invalid(
                    VmReason.InconsistentStructure, JavaScriptDiagnosticCode.MalformedEntryName, reader.Position);
            }

            if (!reader.TryReadBytes(nameLength, out var nameBytes))
            {
                return FromReader(ref reader, reader.Position);
            }

            if (!reader.TryReadVarUInt32(out var codeOffset))
            {
                return FromReader(ref reader, reader.Position);
            }

            entries[index] = new JavaScriptEntryPoint(nameBytes.ToArray(), (int)codeOffset);
        }

        for (var left = 0; left < entries.Length; left++)
        {
            for (var right = left + 1; right < entries.Length; right++)
            {
                if (System.MemoryExtensions.SequenceEqual(
                    System.MemoryExtensions.AsSpan(entries[left].Name),
                    System.MemoryExtensions.AsSpan(entries[right].Name)))
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.DuplicateEntryPoint,
                        reader.Position);
                }
            }
        }

        sections.HasEntries = true;
        sections.Entries = entries;
        return Ok;
    }

    /// <summary>
    /// Reads a section this format reserves and no manifest admits: it must declare nothing.
    /// </summary>
    /// <remarks>
    /// The section is parsed rather than skipped, which is the point of reserving it. An artifact
    /// that carries an exception region or a suspension target is refused at verification with a
    /// diagnostic naming which, so the format can grow into those sections at a later manifest
    /// without a format-version break and without ever having tolerated one silently.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=57B0F0
    // Broiler-Falsified-If: a reserved section carrying a non-zero count verifies
    // Broiler-Human:        PENDING
    private VmVerifierOutcome ReadReserved(ref VmBoundedReader reader, JavaScriptDiagnosticCode diagnosticCode)
    {
        var at = reader.Position;

        if (!reader.TryReadDeclaredCount(out var count))
        {
            return FromReader(ref reader, at);
        }

        return count == 0
            ? Ok
            : Invalid(VmReason.UnknownFeature, diagnosticCode, at);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EDFC3A
    // Broiler-Human:        PENDING
    private VmVerifierOutcome ReadPositions(ref VmBoundedReader reader, ref SectionSet sections)
    {
        if (!reader.TryReadDeclaredCount(out var count))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (count > JavaScriptFormat.CeilingPositions)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.DeclaredMaximumTooLarge,
                reader.Position);
        }

        var previousOffset = -1L;

        for (var index = 0u; index < count; index++)
        {
            var at = reader.Position;

            if (!reader.TryReadVarUInt32(out var offset) ||
                !reader.TryReadVarUInt32(out var line) ||
                !reader.TryReadVarUInt32(out var column))
            {
                return FromReader(ref reader, at);
            }

            if (offset <= previousOffset)
            {
                // Strictly ascending, so the table can be searched by a binary search that cannot
                // be confused by two rows claiming one offset, and so a stack trace names one
                // position rather than whichever row was found first.
                return Invalid(
                    VmReason.InconsistentStructure, JavaScriptDiagnosticCode.MalformedPositionRow, at);
            }

            // Zero is the published encoding's "not known" and one artifact may not mint it. A
            // table that could declare an unknown-looking position would be a table a consumer
            // trusts for the rows it wrote and cannot distinguish from the rows it did not. The
            // upper bound is the same clause from the other side: the coordinate is an int in the
            // core's record, so a value that does not survive the narrowing is refused rather
            // than wrapped into a negative line nobody wrote.
            if (line is 0 or > int.MaxValue || column is 0 or > int.MaxValue)
            {
                return Invalid(
                    VmReason.InconsistentStructure, JavaScriptDiagnosticCode.MalformedPositionRow, at);
            }

            previousOffset = offset;
            sections.PositionRows.Add(new JavaScriptPositionRow(offset, (int)line, (int)column));
        }

        sections.HasPositions = true;
        return Ok;
    }

    /// <summary>
    /// The whole-artifact stage: required sections present, then the code walked twice - once for
    /// instruction boundaries, once for control flow and operand-stack consistency.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The first walk establishes which offsets are instruction boundaries and that no instruction
    /// runs off the end. The second propagates an operand-stack height from every entry point and
    /// checks that every join agrees, that nothing underflows, that the height never passes the
    /// declared maximum, and that every reachable path ends in a return. At this surface every
    /// stack slot holds one JavaScript value, so the value state at a join IS the height; a later
    /// manifest with more than one static shape will need both.
    /// </para>
    /// <para>
    /// <b>Unreachable code is refused</b> rather than left unchecked. The alternative - validating
    /// reachable code and tolerating the rest - would leave bytes in a verified artifact that no
    /// check ever looked at, and "unreachable" would then be a claim about this verifier's own
    /// traversal rather than about the program.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=B856CD
    // Broiler-Falsified-If: an artifact admitted here contains a jump to a non-boundary, a join whose two heights differ, an unreachable instruction, or a path that reaches the end of the code without returning
    // Broiler-Human:        PENDING
    private VmVerifierOutcome Link(ref SectionSet sections, in VmReadBounds bounds, JavaScriptReadAdapter adapter)
    {
        if (!sections.HasLimits || !sections.HasCode || !sections.HasEntries)
        {
            return Invalid(VmReason.InconsistentStructure, JavaScriptDiagnosticCode.MissingSection, 0);
        }

        var code = sections.Code;
        var constants = sections.Constants;

        if (!adapter.TryChargeWork((ulong)code.Length))
        {
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);
        }

        // Three arrays over the code, and all three are charged. The entry map is the one added
        // at JS-3a, and it goes through the same allocator as the other two for the same reason
        // they do: an allocation proportional to an untrusted input that the meter never saw is a
        // budget this profile does not actually hold, whatever the ratio.
        if (!VmBoundedAllocator.TryAllocate<byte>(in bounds, adapter, (uint)code.Length, out var boundary) ||
            !VmBoundedAllocator.TryAllocate<int>(in bounds, adapter, (uint)code.Length, out var height) ||
            !VmBoundedAllocator.TryAllocate<byte>(in bounds, adapter, (uint)code.Length, out var isEntry))
        {
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact);
        }

        // Walk one: instruction boundaries. Every opcode must be one this version defines and
        // every operand must fit, so that walk two can treat an offset as a boundary without
        // decoding anything again.
        for (var offset = 0; offset < code.Length;)
        {
            if (!JavaScriptOpcodes.IsDefined(code[offset]))
            {
                return InvalidInCode(
                    VmReason.UnknownFeature, JavaScriptDiagnosticCode.UnknownOpcode, (ulong)offset, in sections);
            }

            var width = JavaScriptOpcodes.InstructionWidth((JavaScriptOpcode)code[offset]);

            if (offset + width > code.Length)
            {
                return InvalidInCode(
                    VmReason.Truncated,
                    JavaScriptDiagnosticCode.TruncatedInstruction,
                    (ulong)offset,
                    in sections);
            }

            boundary[offset] = 1;
            height[offset] = -1;
            offset += width;
        }

        foreach (var entry in sections.Entries)
        {
            if (entry.CodeOffset >= code.Length || boundary[entry.CodeOffset] == 0)
            {
                return InvalidInCode(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.EntryOffsetNotAnInstructionBoundary,
                    (ulong)entry.CodeOffset,
                    in sections);
            }
        }

        foreach (var row in sections.PositionRows)
        {
            if (row.Offset >= (uint)code.Length || boundary[row.Offset] == 0)
            {
                return InvalidInCode(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedPositionRow,
                    row.Offset,
                    in sections);
            }
        }

        var walk = Walk(in sections, code, boundary, height, isEntry, constants.Length);

        if (walk.Outcome.Category != VmOutcome.Normal)
        {
            return walk.Outcome;
        }

        for (var offset = 0; offset < code.Length; offset++)
        {
            if (boundary[offset] == 1 && height[offset] < 0)
            {
                return InvalidInCode(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.UnreachableCode,
                    (ulong)offset,
                    in sections);
            }
        }

        return VmVerifierOutcome.Verified(
            new JavaScriptProgram(
                constants,
                code,
                sections.Entries,
                walk.MaximumOperandStack,
                sections.LocalCount,
                sections.PositionRows.Count),
            VmArtifactSharing.Shareable);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F47A4B
    // Broiler-Human:        PENDING
    private (VmVerifierOutcome Outcome, int MaximumOperandStack) Walk(
        in SectionSet sections,
        byte[] code,
        byte[] boundary,
        int[] height,
        byte[] isEntry,
        int constantCount)
    {
        var pending = new System.Collections.Generic.Stack<(int Offset, int Height)>();

        // An entry point is entered with an empty operand stack, so any EDGE that reaches one
        // carrying operands is refused - and refused at the edge rather than at the join it would
        // otherwise cause. The distinction is not cosmetic: a join mismatch is reported by
        // whichever of the two arrivals the worklist happens to pop second, so a program's
        // diagnostic would depend on a traversal order no artifact can see. Checking the edge
        // makes the answer a property of the program.
        foreach (var entry in sections.Entries)
        {
            pending.Push((entry.CodeOffset, 0));
            isEntry[entry.CodeOffset] = 1;
        }

        var observed = 0;

        while (pending.Count > 0)
        {
            var (offset, incoming) = pending.Pop();

            if (height[offset] >= 0)
            {
                if (height[offset] != incoming)
                {
                    return (InvalidInCode(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.InconsistentStackHeightAtJoin,
                        (ulong)offset,
                        in sections), 0);
                }

                continue;
            }

            height[offset] = incoming;

            var opcode = (JavaScriptOpcode)code[offset];
            var width = JavaScriptOpcodes.InstructionWidth(opcode);
            var pop = JavaScriptOpcodes.PopCount(opcode);

            if (incoming < pop)
            {
                return (InvalidInCode(
                    VmReason.SemanticValidationFailed,
                    JavaScriptDiagnosticCode.OperandStackUnderflow,
                    (ulong)offset,
                    in sections), 0);
            }

            var after = incoming - pop + JavaScriptOpcodes.PushCount(opcode);

            if (after > sections.MaxOperandStack)
            {
                return (InvalidInCode(
                    VmReason.SemanticValidationFailed,
                    JavaScriptDiagnosticCode.OperandStackOverflow,
                    (ulong)offset,
                    in sections), 0);
            }

            observed = after > observed ? after : observed;

            var operand = OperandCheck(opcode, code, offset, constantCount, in sections);

            if (operand.Category != VmOutcome.Normal)
            {
                return (operand, 0);
            }

            if (opcode == JavaScriptOpcode.Return)
            {
                if (after != 0)
                {
                    // A return leaves exactly the completion value behind and nothing else, so the
                    // height after popping it is zero. Anything else is a lowering that lost track
                    // of its own stack, and admitting it would mean the executor could finish with
                    // values it never accounted for.
                    return (InvalidInCode(
                        VmReason.SemanticValidationFailed,
                        JavaScriptDiagnosticCode.ReturnStackNotExactlyOne,
                        (ulong)offset,
                        in sections), 0);
                }

                continue;
            }

            if (JavaScriptOpcodes.IsJump(opcode))
            {
                var displacement = System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(
                    System.MemoryExtensions.AsSpan(code, offset + 1, 4));

                var target = (long)offset + width + displacement;

                if (target < 0 || target >= code.Length || boundary[target] == 0)
                {
                    return (InvalidInCode(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.JumpTargetNotAnInstructionBoundary,
                        (ulong)offset,
                        in sections), 0);
                }

                if (isEntry[target] == 1 && after != 0)
                {
                    return (InvalidInCode(
                        VmReason.SemanticValidationFailed,
                        JavaScriptDiagnosticCode.EntryStackNotEmpty,
                        (ulong)target,
                        in sections), 0);
                }

                pending.Push(((int)target, after));
            }

            if (!JavaScriptOpcodes.FallsThrough(opcode))
            {
                continue;
            }

            var next = offset + width;

            if (next >= code.Length)
            {
                return (InvalidInCode(
                    VmReason.SemanticValidationFailed,
                    JavaScriptDiagnosticCode.FallsOffTheEnd,
                    (ulong)offset,
                    in sections), 0);
            }

            if (isEntry[next] == 1 && after != 0)
            {
                return (InvalidInCode(
                    VmReason.SemanticValidationFailed,
                    JavaScriptDiagnosticCode.EntryStackNotEmpty,
                    (ulong)next,
                    in sections), 0);
            }

            pending.Push((next, after));
        }

        return (Ok, observed);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D39432
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome OperandCheck(
        JavaScriptOpcode opcode,
        byte[] code,
        int offset,
        int constantCount,
        in SectionSet sections)
    {
        if (opcode == JavaScriptOpcode.LoadConstant)
        {
            var index = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(
                System.MemoryExtensions.AsSpan(code, offset + 1, 2));

            return index < constantCount
                ? Ok
                : InvalidInCode(
                    VmReason.SemanticValidationFailed,
                    JavaScriptDiagnosticCode.ConstantIndexOutOfRange,
                    (ulong)offset,
                    in sections);
        }

        if (opcode is JavaScriptOpcode.LoadLocal or JavaScriptOpcode.StoreLocal)
        {
            var index = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(
                System.MemoryExtensions.AsSpan(code, offset + 1, 2));

            return index < sections.LocalCount
                ? Ok
                : InvalidInCode(
                    VmReason.SemanticValidationFailed,
                    JavaScriptDiagnosticCode.LocalIndexOutOfRange,
                    (ulong)offset,
                    in sections);
        }

        return Ok;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2C1EB2
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome Ok => VmVerifierOutcome.Verified(EmptyState, VmArtifactSharing.Shareable);

    /// <summary>
    /// A sentinel success used to say "this stage found nothing wrong", never returned to the core.
    /// </summary>
    /// <remarks>
    /// The stage helpers answer with a <see cref="VmVerifierOutcome"/> so that a failure can carry
    /// its own reason, code and position rather than being flattened into a boolean and
    /// reconstructed by the caller. Only the final <c>Verified</c> in <see cref="Link"/> reaches
    /// the core, and it carries the real program.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7D454C
    // Broiler-Human:        PENDING
    private static readonly IVmVerifiedState EmptyState = new JavaScriptProgram(
        [], [], [], 0, 0, 0);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2442C7
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome Invalid(VmReason reason, JavaScriptDiagnosticCode code, ulong offset) =>
        VmVerifierOutcome.InvalidArtifact(reason, (int)code, At(offset));

    /// <summary>
    /// An invalid answer whose offset is an offset into the CODE SECTION rather than into the
    /// artifact, carrying the section it is in and the source position that covers it.
    /// </summary>
    /// <remarks>
    /// The two factories are separate rather than one with a flag because the difference is not a
    /// formatting choice: an offset resolved against the wrong frame names an unrelated byte, and
    /// the compiler is the party that should be refusing to mix them up. Every diagnostic the link
    /// and walk stages produce is of this kind, and every diagnostic the read stage produces is of
    /// the other.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=B800EB
    // Broiler-Falsified-If: a code-section offset is reported with the artifact-relative section index, or a read-stage offset with a section index
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome InvalidInCode(
        VmReason reason, JavaScriptDiagnosticCode code, ulong offset, in SectionSet sections) =>
        VmVerifierOutcome.InvalidArtifact(
            reason,
            (int)code,
            JavaScriptPosition.InCode(sections.CodeSectionIndex, offset, sections.PositionRows));

    /// <summary>
    /// What a refused poll means, which is not one thing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>Poll</c> is one combined check and its <see langword="false"/> has three causes:
    /// cancellation, a wall clock that has run out, and this profile exceeding its own declared
    /// uncharged-work bound. Answering <c>Cancellation</c> for all three - which is the obvious
    /// thing to write and which an earlier draft of this verifier did - reports a budget that ran
    /// out as a caller who changed their mind, and every corpus entry that pinned it would record
    /// the wrong triple.
    /// </para>
    /// <para>
    /// The token is the one cause a profile can tell apart, so it decides. Of the other two, the
    /// wall clock is the only budget <c>Poll</c> examines that this profile does not charge
    /// itself, so it is the honest name for what stopped, at the artifact scope this whole
    /// verification runs under. The third - a poll-bound breach - is a
    /// defect in this profile rather than a fact about the artifact; the core detects it and
    /// rewrites the answer, which is why this method does not try to claim it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=2FAC64
    // Broiler-Falsified-If: a wall-clock exhaustion during verification is reported as a cancellation, or a cancellation as a resource exhaustion
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome Stopped(System.Threading.CancellationToken cancellationToken) =>
        cancellationToken.IsCancellationRequested
            ? VmVerifierOutcome.Cancellation()
            : VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.WallClock, VmBudgetScope.Artifact);

    /// <summary>Maps the bounded reader's mechanism status onto the answers a verifier may give.</summary>
    /// <remarks>
    /// The bounded-reading assembly names no contract vocabulary, so this mapping is the profile's
    /// own. It is the visible price of keeping the mechanism free of the contract, and every arm
    /// of it decides between an invalid artifact and a resource exhaustion - which is the split a
    /// corpus entry pins.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B519B7
    // Broiler-Falsified-If: a ceiling breach is mapped onto an invalid artifact, or a framing failure onto a resource exhaustion
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome FromReader(ref VmBoundedReader reader, ulong position) =>
        reader.Status switch
        {
            VmBoundedReadStatus.Truncated =>
                Invalid(VmReason.Truncated, JavaScriptDiagnosticCode.Truncated, position),

            VmBoundedReadStatus.MalformedEncoding =>
                Invalid(VmReason.MalformedEncoding, JavaScriptDiagnosticCode.MalformedEncoding, position),

            VmBoundedReadStatus.DeclaredCountExceeded =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact),

            VmBoundedReadStatus.SectionCountExceeded =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.SectionCount, VmBudgetScope.Artifact),

            VmBoundedReadStatus.StructuralDepthExceeded =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.StructuralDepth, VmBudgetScope.Artifact),

            VmBoundedReadStatus.ArtifactBytesExceeded =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.ArtifactBytes, VmBudgetScope.Artifact),

            VmBoundedReadStatus.AllocationRefused =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact),

            VmBoundedReadStatus.WorkBudgetExhausted =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact),

            _ => Invalid(VmReason.InconsistentStructure, JavaScriptDiagnosticCode.ReaderStopped, position),
        };

    /// <summary>An artifact-relative position, which is what every read-stage refusal carries.</summary>
    /// <remarks>
    /// The encoding itself lives in <see cref="JavaScriptPosition"/> and is published in the
    /// diagnostic registry. This is the read stage's half of it: the reader is part-way through
    /// the framing when it stops, so there is no frame to name and the section index is <c>-1</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=300F5E
    // Broiler-Human:        PENDING
    private static VmSourcePosition At(ulong offset) => JavaScriptPosition.InArtifact(offset);

    /// <summary>What the section pass accumulates for the whole-artifact pass to link.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BB5E79
    // Broiler-Human:        PENDING
    private struct SectionSet
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=80E8B2
        // Broiler-Human:        PENDING
        public bool HasLimits;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D4A380
        // Broiler-Human:        PENDING
        public bool HasConstants;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A6E1EC
        // Broiler-Human:        PENDING
        public bool HasCode;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DD3254
        // Broiler-Human:        PENDING
        public bool HasEntries;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=98F171
        // Broiler-Human:        PENDING
        public bool HasPositions;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=81FF1A
        // Broiler-Human:        PENDING
        public int MaxOperandStack;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DFED22
        // Broiler-Human:        PENDING
        public int LocalCount;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=36F7E0
        // Broiler-Human:        PENDING
        public int MaxConstants;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1DCEF9
        // Broiler-Human:        PENDING
        public JavaScriptValue[] Constants;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4C52A1
        // Broiler-Human:        PENDING
        public byte[] Code;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=63AFA5
        // Broiler-Human:        PENDING
        public JavaScriptEntryPoint[] Entries;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=512D1C
        // Broiler-Human:        PENDING
        public System.Collections.Generic.List<JavaScriptPositionRow> PositionRows;

        /// <summary>
        /// The ordinal index of the code section's frame, or <c>-1</c> until it is framed.
        /// </summary>
        /// <remarks>
        /// The ordinal, not the section KIND. An artifact may omit a section this format defines,
        /// so the two are different numbers, and the core's position record wants the one that
        /// identifies a frame in THIS artifact.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D6697B
        // Broiler-Human:        PENDING
        public int CodeSectionIndex;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C1D971
        // Broiler-Human:        PENDING
        public SectionSet()
        {
            HasLimits = false;
            HasConstants = false;
            HasCode = false;
            HasEntries = false;
            HasPositions = false;
            MaxOperandStack = 0;
            LocalCount = 0;
            MaxConstants = 0;
            Constants = [];
            Code = [];
            Entries = [];
            PositionRows = [];
            CodeSectionIndex = JavaScriptPosition.OutsideAnySection;
        }
    }
}
