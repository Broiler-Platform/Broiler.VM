// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   29
// Annotated:        29/29
// Exempt:           21
// Human-reviewed:   0/29
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       29
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The format-version-2 verifier: structure, then linking, then one abstract pass per code unit.
/// </summary>
/// <remarks>
/// <para>
/// It has version 1's shape and version 1's discipline. Nothing is allocated from a declared count
/// before that count has been compared with a ceiling; every offset the payload states is checked
/// against a range this pass computed rather than against another number the payload stated; and
/// the operand-stack height the executor sizes its stack from is the height THIS pass computed,
/// never the one the artifact declared.
/// </para>
/// <para>
/// <b>What is new is that there are many code units and they share one code section.</b> Two
/// units whose declared ranges overlap would let a jump inside one land in the middle of the
/// other's instruction stream, so the ranges are required to be disjoint and ascending, and every
/// branch target is checked against the range of the unit that contains the branch - not against
/// the code section as a whole.
/// </para>
/// <para>
/// <b>An exception handler is a second entry into a unit</b>, at a stack height and a scope depth
/// the region declares. The abstract pass seeds those states exactly as it seeds a unit's entry,
/// so a handler whose declared height disagrees with what the code at it does is a join
/// disagreement and is refused, rather than an operand-stack corruption at the first throw.
/// </para>
/// <para>
/// <b>What a <c>with</c> body costs this pass is one thing and not the model.</b> An object
/// environment record is a scope like any other here: <see cref="JsOpcode.PushObjectScope"/> raises
/// the depth, <see cref="JsOpcode.PopScope"/> lowers it, and the two branches a dynamically
/// resolved name lowers to are held to the same join rule every other branch is. Every read, write
/// and deletion inside such a body is an ordinary property instruction, so this pass checks them.
/// <b>What it stops being able to check is which environment a NAME reaches</b>: outside a
/// <c>with</c> body a read names a slot and a depth this pass bounds, and inside one the answer
/// depends on what an object holds when the instruction runs. That is not a check this pass gave
/// up - it is a question the language stopped answering statically - and what keeps it harmless is
/// that <see cref="JsOpcode.ResolveName"/> can only ever answer with an OBJECT that is already on
/// the chain, never with a slot.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B42A78
// Broiler-Human:        PENDING
internal sealed class JsVerifier
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5BDA1A
    // Broiler-Human:        PENDING
    private const int MaxScopeDepth = (int)JsFormat.CeilingScopeDepth;


    /// <summary>Verifies a version-2 payload and produces the program the executor runs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DEEDF7
    // Broiler-Human:        PENDING
    internal static VmVerifierOutcome Verify(
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        System.Collections.Immutable.ImmutableArray<string> admittedSurfaces,
        System.Threading.CancellationToken cancellationToken)
    {
        var adapter = new JavaScriptReadAdapter(context.Meter);
        var bounds = JavaScriptReadAdapter.ToReadBounds(context.Ceilings.VerificationCeilings);
        var reader = new VmBoundedReader(payload, in bounds, adapter);
        var state = new Sections();

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

        // THE DESCRIPTOR-AGAINST-PAYLOAD CHECK COMES FIRST, and the order is what makes the
        // mismatch reachable at all. A build registering one format version can never observe it -
        // the core screens the descriptor against the registered range before this profile is
        // called, so a descriptor that got here already named the only version there is. With two
        // registered, a caller can present version-1 bytes under a version-2 descriptor, and
        // answering that with "unsupported version" would name the payload when the caller is what
        // is wrong.
        if (descriptor.FormatVersion != formatVersion)
        {
            return Invalid(
                VmReason.DescriptorMismatch,
                JavaScriptDiagnosticCode.DescriptorFormatVersionMismatch,
                reader.Position);
        }

        if (formatVersion != JsFormat.FormatVersion)
        {
            return Invalid(
                VmReason.UnsupportedProfileFormatVersion,
                JavaScriptDiagnosticCode.UnsupportedFormatVersion,
                reader.Position);
        }

        var manifest = ReadManifest(in descriptor, ref reader);

        if (manifest.Category != VmOutcome.Normal)
        {
            return manifest;
        }

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

            var outcome = ReadSection(ref reader, adapter, ref previousKind, state, admittedSurfaces);

            if (outcome.Category != VmOutcome.Normal)
            {
                return outcome;
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

        return Link(state, adapter, admittedSurfaces, cancellationToken);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BB9BC8
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadManifest(
        in VmArtifactDescriptor descriptor, ref VmBoundedReader reader)
    {
        if (!reader.TryReadVarUInt32(out var length))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (length > JavaScriptFormat.MaximumManifestIdBytes)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.ManifestIdTooLong,
                reader.Position);
        }

        if (!reader.TryReadBytes(length, out var bytes))
        {
            return FromReader(ref reader, reader.Position);
        }

        var text = System.Text.Encoding.UTF8.GetString(bytes);

        // The same ordering and the same reason as the format version above: with two accepted
        // manifests a caller can mislabel one as the other, and that is the caller's mistake and
        // not the payload's.
        if (!string.Equals(
                descriptor.FeatureManifestId.ToString(), text, System.StringComparison.Ordinal))
        {
            return Invalid(
                VmReason.DescriptorMismatch,
                JavaScriptDiagnosticCode.DescriptorManifestMismatch,
                reader.Position);
        }

        if (!string.Equals(text, JsFormat.ManifestId, System.StringComparison.Ordinal))
        {
            return Invalid(
                VmReason.UnsupportedFeatureManifest,
                JavaScriptDiagnosticCode.UnsupportedFeatureManifest,
                reader.Position);
        }

        return VmVerifierOutcome.Verified(EmptyState.Instance, VmArtifactSharing.Shareable);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=8C2539
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadSection(
        ref VmBoundedReader reader,
        JavaScriptReadAdapter adapter,
        ref uint previousKind,
        Sections state,
        System.Collections.Immutable.ImmutableArray<string> admittedSurfaces)
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

        if (kind is < 1 or > 9)
        {
            return Invalid(
                VmReason.UnknownFeature, JavaScriptDiagnosticCode.UnknownSectionKind, at);
        }

        if (kind <= previousKind)
        {
            return Invalid(VmReason.InconsistentStructure, JavaScriptDiagnosticCode.SectionOrder, at);
        }

        previousKind = kind;

        if (!reader.TryEnterSection(length, out var frame))
        {
            return FromReader(ref reader, reader.Position);
        }

        var outcome = (JsFormat.SectionKind)kind switch
        {
            JsFormat.SectionKind.Limits => ReadLimits(ref reader, state),
            JsFormat.SectionKind.Constants => ReadConstants(ref reader, adapter, state),
            JsFormat.SectionKind.Code => ReadCode(ref reader, length, state),
            JsFormat.SectionKind.Entries => ReadEntries(ref reader, state),
            JsFormat.SectionKind.ExceptionRegions => ReadRegions(ref reader, state),
            JsFormat.SectionKind.Positions => ReadPositions(ref reader, state),
            JsFormat.SectionKind.Functions => ReadFunctions(ref reader, state),
            JsFormat.SectionKind.Surfaces => ReadSurfaces(ref reader, state, admittedSurfaces),
            _ => Invalid(
                VmReason.UnknownFeature,
                JavaScriptDiagnosticCode.SuspensionTargetOutsideManifest,
                reader.Position),
        };

        if (outcome.Category != VmOutcome.Normal)
        {
            return outcome;
        }

        if (!reader.TryExitSection(in frame))
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.SectionLengthMismatch,
                reader.Position);
        }

        return Ok;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2D75FD
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadLimits(ref VmBoundedReader reader, Sections state)
    {
        if (!reader.TryReadVarUInt32(out var operandStack) ||
            !reader.TryReadVarUInt32(out var scopeSlots) ||
            !reader.TryReadVarUInt32(out var functions) ||
            !reader.TryReadVarUInt32(out var constants))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (operandStack > JsFormat.CeilingOperandStack ||
            scopeSlots > JsFormat.CeilingScopeSlots ||
            functions > JsFormat.CeilingFunctions ||
            constants > JsFormat.CeilingConstants)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.DeclaredMaximumTooLarge,
                reader.Position);
        }

        state.DeclaredOperandStack = operandStack;
        state.DeclaredScopeSlots = scopeSlots;
        state.DeclaredFunctions = functions;
        state.DeclaredConstants = constants;
        state.SawLimits = true;
        return Ok;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=58E734
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadConstants(
        ref VmBoundedReader reader, JavaScriptReadAdapter adapter, Sections state)
    {
        if (!reader.TryReadDeclaredCount(out var count))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (count > state.DeclaredConstants || count > JsFormat.CeilingConstants)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.ConstantCountExceedsDeclaredMaximum,
                reader.Position);
        }

        var values = new JsValue[count];
        var names = new string[count];

        for (var index = 0u; index < count; index++)
        {
            if (!adapter.Poll())
            {
                return Invalid(
                    VmReason.InconsistentStructure, JavaScriptDiagnosticCode.ReaderStopped, reader.Position);
            }

            if (!reader.TryReadByte(out var tag))
            {
                return FromReader(ref reader, reader.Position);
            }

            names[index] = string.Empty;

            switch ((JsFormat.ConstantTag)tag)
            {
                case JsFormat.ConstantTag.Undefined:
                    values[index] = JsValue.Undefined;
                    break;

                case JsFormat.ConstantTag.Null:
                    values[index] = JsValue.Null;
                    break;

                case JsFormat.ConstantTag.Boolean:
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

                    values[index] = JsValue.Boolean(flag == 1);
                    break;

                case JsFormat.ConstantTag.Number:
                    if (!reader.TryReadUInt64LittleEndian(out var bits))
                    {
                        return FromReader(ref reader, reader.Position);
                    }

                    values[index] = JsValue.Number(System.BitConverter.Int64BitsToDouble(unchecked((long)bits)));
                    break;

                case JsFormat.ConstantTag.InternedName:
                case JsFormat.ConstantTag.String:
                    if (!reader.TryReadVarUInt32(out var length))
                    {
                        return FromReader(ref reader, reader.Position);
                    }

                    if (!ReadRun(ref reader, length, out var text))
                    {
                        return FromReader(ref reader, reader.Position);
                    }

                    names[index] = Format.JsFormat.DecodeText(text);
                    values[index] = JsValue.String(names[index]);
                    break;

                default:
                    return Invalid(
                        VmReason.UnknownFeature,
                        JavaScriptDiagnosticCode.UnknownConstantTag,
                        reader.Position);
            }
        }

        state.Constants = values;
        state.Names = names;
        state.SawConstants = true;
        return Ok;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=712EA0
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadCode(ref VmBoundedReader reader, ulong length, Sections state)
    {
        if (length > JsFormat.CeilingCodeBytes)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.DeclaredMaximumTooLarge,
                reader.Position);
        }

        if (!ReadRun(ref reader, length, out var body))
        {
            return FromReader(ref reader, reader.Position);
        }

        state.Code = body;
        state.SawCode = true;
        return Ok;
    }

    /// <summary>
    /// Reads a run of bytes in windows no larger than the declared uncharged-work bound.
    /// </summary>
    /// <remarks>
    /// <b>One charge may not exceed the poll bound this profile declares.</b> The bounded reader
    /// charges one work unit per byte consumed and polls after each charge; a charge larger than
    /// the declared bound is a poll-bound violation, reported as an exhausted work allowance. So a
    /// code section is not one read of its whole length - it is a sequence of reads whose size is
    /// the bound. The alternative was to declare a bound as large as the artifact-bytes ceiling,
    /// which would have made the declaration true and the cancellation latency it promises
    /// meaningless.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=50642E
    // Broiler-Human:        PENDING
    private static bool ReadRun(ref VmBoundedReader reader, ulong length, out byte[] data)
    {
        data = length == 0 ? System.Array.Empty<byte>() : new byte[length];
        var written = 0;

        while ((ulong)written < length)
        {
            var window = System.Math.Min(ReadWindowBytes, length - (ulong)written);

            if (!reader.TryReadBytes(window, out var chunk))
            {
                data = System.Array.Empty<byte>();
                return false;
            }

            chunk.CopyTo(System.MemoryExtensions.AsSpan(data, written));
            written += (int)window;
        }

        return true;
    }

    /// <summary>The largest run this verifier reads in one charge.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3A8BAC
    // Broiler-Human:        PENDING
    private const ulong ReadWindowBytes = 32_768;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B4A2EC
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadEntries(ref VmBoundedReader reader, Sections state)
    {
        if (!reader.TryReadDeclaredCount(out var count))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (count > JsFormat.CeilingEntries)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.DeclaredMaximumTooLarge,
                reader.Position);
        }

        var entries = new JsEntry[count];

        for (var index = 0u; index < count; index++)
        {
            if (!reader.TryReadVarUInt32(out var nameLength))
            {
                return FromReader(ref reader, reader.Position);
            }

            if (nameLength is 0 or > JavaScriptFormat.MaximumEntryNameBytes)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedEntryName,
                    reader.Position);
            }

            if (!reader.TryReadBytes(nameLength, out var nameBytes))
            {
                return FromReader(ref reader, reader.Position);
            }

            if (!reader.TryReadVarUInt32(out var unit))
            {
                return FromReader(ref reader, reader.Position);
            }

            var name = System.Text.Encoding.UTF8.GetString(nameBytes);

            for (var earlier = 0u; earlier < index; earlier++)
            {
                if (string.Equals(entries[earlier].Name, name, System.StringComparison.Ordinal))
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.DuplicateEntryPoint,
                        reader.Position);
                }
            }

            entries[index] = new JsEntry(name, unit);
        }

        state.Entries = entries;
        state.SawEntries = true;
        return Ok;
    }

    /// <summary>
    /// Reads the optional surfaces this artifact declares, and refuses one the composition has not
    /// admitted.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is where a composition declining a surface refuses an artifact, and it is at
    /// verification rather than at run time on purpose.</b> Roadmap section 6 draws the
    /// distinction: a composition that declines a manifest produces an invalid artifact the guest
    /// never sees, and a composition that admits one while registering no provider produces a
    /// run-time refusal the guest may catch. Two outcomes, two catchabilities, and reading them off
    /// one behaviour is how a policy boundary quietly stops being one.
    /// </para>
    /// <para>
    /// <b>Two refusals rather than one, and the difference is who is wrong.</b> A name this build
    /// does not know is an artifact naming a surface nobody wrote; a name this build knows and this
    /// composition did not admit is an artifact naming a surface somebody declined. The first is a
    /// defect in whatever produced the bytes and the second is the composition doing its job, and a
    /// reader of a diagnostic code should not have to guess which happened.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2F6AFE
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadSurfaces(
        ref VmBoundedReader reader,
        Sections state,
        System.Collections.Immutable.ImmutableArray<string> admittedSurfaces)
    {
        if (!reader.TryReadDeclaredCount(out var count))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (count > JsFormat.CeilingSurfaces)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.DeclaredMaximumTooLarge,
                reader.Position);
        }

        var declared = new string[count];

        for (var index = 0u; index < count; index++)
        {
            if (!reader.TryReadVarUInt32(out var length))
            {
                return FromReader(ref reader, reader.Position);
            }

            if (length is 0 or > JavaScriptFormat.MaximumManifestIdBytes)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.ManifestIdTooLong,
                    reader.Position);
            }

            if (!reader.TryReadBytes(length, out var bytes))
            {
                return FromReader(ref reader, reader.Position);
            }

            var identity = System.Text.Encoding.UTF8.GetString(bytes);

            for (var earlier = 0u; earlier < index; earlier++)
            {
                if (string.Equals(declared[earlier], identity, System.StringComparison.Ordinal))
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.DuplicateSurface,
                        reader.Position);
                }
            }

            if (!JsSurfaces.IsKnown(identity))
            {
                return Invalid(
                    VmReason.UnknownFeature,
                    JavaScriptDiagnosticCode.UnknownSurface,
                    reader.Position);
            }

            if (!admittedSurfaces.Contains(identity))
            {
                return Invalid(
                    VmReason.UnsupportedFeatureManifest,
                    JavaScriptDiagnosticCode.SurfaceOutsideComposition,
                    reader.Position);
            }

            declared[index] = identity;
        }

        state.Surfaces = declared;
        return Ok;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9E02CD
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadRegions(ref VmBoundedReader reader, Sections state)
    {
        if (!reader.TryReadDeclaredCount(out var count))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (count > JsFormat.CeilingExceptionRegions)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.DeclaredMaximumTooLarge,
                reader.Position);
        }

        var regions = new JsRegion[count];

        for (var index = 0u; index < count; index++)
        {
            if (!reader.TryReadVarUInt32(out var unit) ||
                !reader.TryReadVarUInt32(out var tryStart) ||
                !reader.TryReadVarUInt32(out var tryEnd) ||
                !reader.TryReadVarUInt32(out var handler) ||
                !reader.TryReadVarUInt32(out var scopeDepth) ||
                !reader.TryReadVarUInt32(out var stackHeight) ||
                !reader.TryReadByte(out var kind))
            {
                return FromReader(ref reader, reader.Position);
            }

            if (kind > (byte)JsFormat.HandlerKind.Finally)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedExceptionRegion,
                    reader.Position);
            }

            regions[index] = new JsRegion(
                unit, tryStart, tryEnd, handler, scopeDepth, stackHeight, (JsFormat.HandlerKind)kind);
        }

        state.Regions = regions;
        return Ok;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=EA6D0B
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadPositions(ref VmBoundedReader reader, Sections state)
    {
        if (!reader.TryReadDeclaredCount(out var count))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (count > JsFormat.CeilingPositions)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.DeclaredMaximumTooLarge,
                reader.Position);
        }

        var previous = 0u;

        for (var index = 0u; index < count; index++)
        {
            if (!reader.TryReadVarUInt32(out var offset) ||
                !reader.TryReadVarUInt32(out var line) ||
                !reader.TryReadVarUInt32(out var column))
            {
                return FromReader(ref reader, reader.Position);
            }

            if ((index != 0 && offset < previous) || line == 0 || column == 0)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedPositionRow,
                    reader.Position);
            }

            previous = offset;
        }

        state.PositionRows = (int)count;
        return Ok;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=10947F
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadFunctions(ref VmBoundedReader reader, Sections state)
    {
        if (!reader.TryReadDeclaredCount(out var count))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (count == 0 || count > state.DeclaredFunctions || count > JsFormat.CeilingFunctions)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedFunctionRow,
                reader.Position);
        }

        var rows = new JsFunctionRow[count];

        for (var index = 0u; index < count; index++)
        {
            if (!reader.TryReadVarUInt32(out var name) ||
                !reader.TryReadVarUInt32(out var parameters) ||
                !reader.TryReadVarUInt32(out var slots) ||
                !reader.TryReadVarUInt32(out var stack) ||
                !reader.TryReadVarUInt32(out var offset) ||
                !reader.TryReadVarUInt32(out var length) ||
                !reader.TryReadVarUInt32(out var flags))
            {
                return FromReader(ref reader, reader.Position);
            }

            rows[index] = new JsFunctionRow(name, parameters, slots, stack, offset, length, flags);
        }

        state.FunctionRows = rows;
        state.SawFunctions = true;
        return Ok;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1067B2
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome Link(
        Sections state,
        JavaScriptReadAdapter adapter,
        System.Collections.Immutable.ImmutableArray<string> admittedSurfaces,
        System.Threading.CancellationToken cancellationToken)
    {
        if (!state.SawLimits || !state.SawConstants || !state.SawCode ||
            !state.SawEntries || !state.SawFunctions)
        {
            return Invalid(
                VmReason.InconsistentStructure, JavaScriptDiagnosticCode.MissingSection, 0);
        }

        var rows = state.FunctionRows!;
        var code = state.Code!;
        var units = new JsCodeUnit[rows.Length];
        var reached = 0u;

        for (var index = 0; index < rows.Length; index++)
        {
            var row = rows[index];

            // `ParameterCount` MEANS TWO THINGS AND ONLY ONE OF THEM IS A SLOT COUNT. Without
            // `BindsParameters` the frame copies that many arguments into slots zero upward, so it
            // must fit in the scope; with it, no copy happens and the figure is only the arity the
            // function reports as `length` - which a pattern with no bindings, `function f({}) {}`,
            // makes larger than the slots.
            var binds = ((JsFormat.FunctionFlags)row.Flags &
                JsFormat.FunctionFlags.BindsParameters) != 0;

            if (row.ScopeSlots > state.DeclaredScopeSlots ||
                row.MaxOperandStack > state.DeclaredOperandStack ||
                (!binds && row.ParameterCount > row.ScopeSlots) ||
                row.ParameterCount > JsFormat.CeilingCallArguments)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedFunctionRow,
                    (ulong)index);
            }

            if (row.NameConstant > state.Constants!.Length)
            {
                return Invalid(
                    VmReason.SemanticValidationFailed,
                    JavaScriptDiagnosticCode.ConstantIndexOutOfRange,
                    (ulong)index);
            }

            if (row.CodeLength == 0)
            {
                return Invalid(VmReason.InconsistentStructure, JavaScriptDiagnosticCode.EmptyCode, (ulong)index);
            }

            // A GENERATOR IS NONE OF THE OTHER THREE THINGS A FLAG CAN SAY IT IS. The executor
            // decides whether an invocation gets a heap frame from this bit alone, and each of the
            // three it is refused with here would have already sent the invocation somewhere else.
            var unitFlags = (JsFormat.FunctionFlags)row.Flags;

            if ((unitFlags & JsFormat.FunctionFlags.Generator) != 0 &&
                (unitFlags & (JsFormat.FunctionFlags.Arrow |
                    JsFormat.FunctionFlags.ProgramBody |
                    JsFormat.FunctionFlags.Constructible)) != 0)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.GeneratorFlagsInconsistent,
                    (ulong)index);
            }

            // AN ASYNC UNIT IS NEITHER THE PROGRAM BODY NOR A CONSTRUCTOR, AND NEITHER THE ARROW
            // NOR THE GENERATOR IS ON THE LIST. The arrow never was: an async ARROW is an ordinary
            // arrow whose body may suspend, and the executor enters it exactly as it enters any
            // arrow - with the lexical `this` and `new.target` its closure recorded. The GENERATOR
            // was, and dropping it is what admitted the async generator: the pair does not ask the
            // executor to choose between the generator driver and the async one, it names a THIRD
            // driver whose caller pulls with `next` and whose body settles the promise that pull
            // answered. `Generator | Arrow` stays refused by the check above, which is what keeps
            // the one combination the grammar has no production for out.
            if ((unitFlags & JsFormat.FunctionFlags.Async) != 0 &&
                (unitFlags & (JsFormat.FunctionFlags.ProgramBody |
                    JsFormat.FunctionFlags.Constructible)) != 0)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.AsyncFlagsInconsistent,
                    (ulong)index);
            }

            // DISJOINT AND ASCENDING, both. Two units whose ranges overlapped would let a branch
            // verified against one unit's range land inside the other's instruction stream, and
            // every check downstream of that is checking the wrong thing.
            if (row.CodeOffset != reached ||
                (ulong)row.CodeOffset + row.CodeLength > (ulong)code.Length)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.CodeUnitRangeInvalid,
                    (ulong)index);
            }

            reached = row.CodeOffset + row.CodeLength;

            units[index] = new JsCodeUnit(
                row.NameConstant == 0 ? string.Empty : state.Names![row.NameConstant - 1],
                row.ParameterCount,
                row.ScopeSlots,
                row.MaxOperandStack,
                row.CodeOffset,
                row.CodeLength,
                (JsFormat.FunctionFlags)row.Flags);
        }

        if (reached != code.Length)
        {
            return Invalid(
                VmReason.InconsistentStructure, JavaScriptDiagnosticCode.CodeUnitRangeInvalid, reached);
        }

        foreach (var entry in state.Entries!)
        {
            if (entry.Unit >= units.Length)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.FunctionIndexOutOfRange,
                    entry.Unit);
            }
        }

        if (state.Entries.Length == 0)
        {
            return Invalid(VmReason.InconsistentStructure, JavaScriptDiagnosticCode.NoEntryPoint, 0);
        }

        foreach (var region in state.Regions)
        {
            if (region.Unit >= units.Length)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.FunctionIndexOutOfRange,
                    region.Unit);
            }

            var unit = units[region.Unit];
            var start = unit.CodeOffset;
            var end = unit.CodeOffset + unit.CodeLength;

            if (region.TryStart < start || region.TryEnd > end || region.TryStart >= region.TryEnd ||
                region.Handler < start || region.Handler >= end ||
                region.ScopeDepth > MaxScopeDepth ||
                region.StackHeight > unit.MaxOperandStack)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedExceptionRegion,
                    region.Handler);
            }
        }

        var walker = new Walker(state, units, adapter);

        for (var index = 0; index < units.Length; index++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return VmVerifierOutcome.Cancellation();
            }

            var outcome = walker.Walk(index);

            if (outcome.Category != VmOutcome.Normal)
            {
                return outcome;
            }
        }

        // THE REALM IS BUILT FROM WHAT THE COMPOSITION ADMITS, NOT FROM WHAT THE ARTIFACT DECLARED.
        // The two sets are different questions and only one of them is a policy: the artifact's
        // declaration is what this pass has just refused an unadmitted entry of, and the
        // composition's is what the guest may find on the global object. Installing only what a
        // particular artifact declared would make `typeof Uint8Array` answer differently for two
        // programs a composition admits equally, which is a difference no embedder asked for.
        var program = new JsProgram(
            state.Constants!,
            state.Names!,
            code,
            units,
            state.Regions,
            state.Entries,
            state.PositionRows,
            admittedSurfaces);

        return VmVerifierOutcome.Verified(program, VmArtifactSharing.Shareable);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=658F98
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome Stopped(System.Threading.CancellationToken cancellationToken) =>
        cancellationToken.IsCancellationRequested
            ? VmVerifierOutcome.Cancellation()
            : VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B9CA4E
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome Invalid(
        VmReason reason, JavaScriptDiagnosticCode code, ulong position) =>
        VmVerifierOutcome.InvalidArtifact(reason, (int)code, JavaScriptPosition.InArtifact(position));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B519B7
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6EC9EF
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome Ok { get; } =
        VmVerifierOutcome.Verified(EmptyState.Instance, VmArtifactSharing.Shareable);

    /// <summary>A placeholder state for the intermediate "this step was fine" answer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4B7E71
    // Broiler-Human:        PENDING
    private sealed class EmptyState : IVmVerifiedState
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=CC4D0F
        // Broiler-Human:        PENDING
        internal static EmptyState Instance { get; } = new();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=736D60
    // Broiler-Human:        PENDING
    private sealed class Sections
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DD68D4
        // Broiler-Human:        PENDING
        internal uint DeclaredOperandStack { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0CDD56
        // Broiler-Human:        PENDING
        internal uint DeclaredScopeSlots { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=27A592
        // Broiler-Human:        PENDING
        internal uint DeclaredFunctions { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0C2C82
        // Broiler-Human:        PENDING
        internal uint DeclaredConstants { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=EB94E3
        // Broiler-Human:        PENDING
        internal JsValue[]? Constants { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FC3F02
        // Broiler-Human:        PENDING
        internal string[]? Names { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7960C7
        // Broiler-Human:        PENDING
        internal byte[]? Code { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5A86AC
        // Broiler-Human:        PENDING
        internal JsEntry[]? Entries { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FF5EAE
        // Broiler-Human:        PENDING
        internal JsFunctionRow[]? FunctionRows { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=24D1A1
        // Broiler-Human:        PENDING
        internal JsRegion[] Regions { get; set; } = [];

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7415BF
        // Broiler-Human:        PENDING
        internal int PositionRows { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9FAFA3
        // Broiler-Human:        PENDING
        internal string[] Surfaces { get; set; } = [];

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4F7CE5
        // Broiler-Human:        PENDING
        internal bool SawLimits { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=59535C
        // Broiler-Human:        PENDING
        internal bool SawConstants { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3770A5
        // Broiler-Human:        PENDING
        internal bool SawCode { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1439EE
        // Broiler-Human:        PENDING
        internal bool SawEntries { get; set; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=56B6A0
        // Broiler-Human:        PENDING
        internal bool SawFunctions { get; set; }
    }

    /// <summary>
    /// The abstract pass over one code unit: operand-stack height and scope depth at every offset.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=43A653
    // Broiler-Human:        PENDING
    private sealed class Walker(Sections state, JsCodeUnit[] units, JavaScriptReadAdapter adapter)
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=855AD6
        // Broiler-Human:        PENDING
        private const int Unvisited = -1;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5169B8
        // Broiler-Human:        PENDING
        private int[] heights = [];

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A1A8B7
        // Broiler-Human:        PENDING
        private int[] depths = [];

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D37B36
        // Broiler-Human:        PENDING
        internal VmVerifierOutcome Walk(int index)
        {
            var unit = units[index];
            var code = state.Code!;
            var start = (int)unit.CodeOffset;
            var end = start + (int)unit.CodeLength;
            var span = (int)unit.CodeLength;

            if (heights.Length < span)
            {
                heights = new int[System.Math.Max(span, 64)];
                depths = new int[heights.Length];
            }

            for (var at = 0; at < span; at++)
            {
                heights[at] = Unvisited;
                depths[at] = Unvisited;
            }

            var pending = new System.Collections.Generic.Stack<int>();

            heights[0] = 0;
            depths[0] = 0;
            pending.Push(start);

            foreach (var region in state.Regions)
            {
                if (region.Unit != index)
                {
                    continue;
                }

                var at = (int)region.Handler - start;
                var height = (int)region.StackHeight + 1;

                if (heights[at] == Unvisited)
                {
                    heights[at] = height;
                    depths[at] = (int)region.ScopeDepth;
                    pending.Push((int)region.Handler);
                }
                else if (heights[at] != height || depths[at] != (int)region.ScopeDepth)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.InconsistentStackHeightAtJoin,
                        region.Handler);
                }
            }

            var highest = 0;

            while (pending.Count != 0)
            {
                if (!adapter.TryChargeWork(1))
                {
                    return VmVerifierOutcome.ResourceExhaustion(
                        VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);
                }

                var offset = pending.Pop();

                while (true)
                {
                    if (offset >= end)
                    {
                        return Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.FallsOffTheEnd,
                            (ulong)offset);
                    }

                    var local = offset - start;
                    var height = heights[local];
                    var depth = depths[local];
                    var raw = code[offset];

                    if (!JsOpcodes.IsDefined(raw))
                    {
                        return Invalid(
                            VmReason.UnknownFeature,
                            JavaScriptDiagnosticCode.UnknownOpcode,
                            (ulong)offset);
                    }

                    var opcode = (JsOpcode)raw;
                    var width = JsOpcodes.InstructionWidth(opcode);

                    if (offset + width > end)
                    {
                        return Invalid(
                            VmReason.Truncated,
                            JavaScriptDiagnosticCode.TruncatedInstruction,
                            (ulong)offset);
                    }

                    var operand = Operand(code, offset, opcode);
                    var check = Check(unit, opcode, operand, offset);

                    if (check.Category != VmOutcome.Normal)
                    {
                        return check;
                    }

                    if (!JsOpcodes.TryDescribe(opcode, operand, out var pops, out var pushes))
                    {
                        return Invalid(
                            VmReason.UnknownFeature,
                            JavaScriptDiagnosticCode.UnknownOpcode,
                            (ulong)offset);
                    }

                    if (height < pops)
                    {
                        return Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.OperandStackUnderflow,
                            (ulong)offset);
                    }

                    var after = height - pops + pushes;

                    if (after > JsFormat.CeilingOperandStack)
                    {
                        return Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.OperandStackOverflow,
                            (ulong)offset);
                    }

                    if (opcode == JsOpcode.Pick && operand >= (uint)height)
                    {
                        return Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.OperandStackUnderflow,
                            (ulong)offset);
                    }

                    var afterDepth = depth;

                    switch (opcode)
                    {
                        // AN OBJECT ENVIRONMENT RECORD IS A SCOPE AND IS COUNTED AS ONE. It holds an
                        // object where a declarative record holds slots, and nothing about the
                        // abstract state distinguishes the two: a `with` body's exits - falling
                        // through, `break`, `continue`, `return` and an exception unwinding to a
                        // region - are checked against the same depth arithmetic every block gets.
                        case JsOpcode.PushScope:
                        case JsOpcode.PushObjectScope:
                            afterDepth = depth + 1;

                            if (afterDepth > MaxScopeDepth)
                            {
                                return Invalid(
                                    VmReason.InconsistentStructure,
                                    JavaScriptDiagnosticCode.ScopeDepthOutOfRange,
                                    (ulong)offset);
                            }

                            break;

                        case JsOpcode.PopScope:
                            if (depth == 0)
                            {
                                return Invalid(
                                    VmReason.InconsistentStructure,
                                    JavaScriptDiagnosticCode.ScopeDepthOutOfRange,
                                    (ulong)offset);
                            }

                            afterDepth = depth - 1;
                            break;

                        default:
                            break;
                    }

                    highest = System.Math.Max(highest, after);

                    if (JsOpcodes.HasCodeTarget(opcode))
                    {
                        // The four stepping opcodes have a different height on the taken branch than
                        // on the fall-through: a name, a value or a close result arrives only when
                        // there was one. `IterateAwaitStep` is two below rather than one, because
                        // it consumes the awaited step AND the record the step was taken from -
                        // and on the taken branch neither is replaced.
                        var targetHeight = opcode switch
                        {
                            JsOpcode.ForInNext or JsOpcode.IterateNext or
                                JsOpcode.IterateCloseAsync => height - 1,
                            JsOpcode.IterateAwaitStep => height - 2,
                            _ => after,
                        };
                        var seeded = Seed(
                            unit, code, (int)operand, targetHeight, afterDepth, pending, offset);

                        if (seeded.Category != VmOutcome.Normal)
                        {
                            return seeded;
                        }
                    }

                    if (JsOpcodes.IsTerminal(opcode))
                    {
                        if (opcode == JsOpcode.Return && height != 1)
                        {
                            return Invalid(
                                VmReason.SemanticValidationFailed,
                                JavaScriptDiagnosticCode.ReturnStackNotExactlyOne,
                                (ulong)offset);
                        }

                        break;
                    }

                    var next = offset + width;

                    if (next >= end)
                    {
                        return Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.FallsOffTheEnd,
                            (ulong)offset);
                    }

                    var nextLocal = next - start;

                    if (heights[nextLocal] == Unvisited)
                    {
                        heights[nextLocal] = after;
                        depths[nextLocal] = afterDepth;
                        offset = next;
                        continue;
                    }

                    if (heights[nextLocal] != after || depths[nextLocal] != afterDepth)
                    {
                        return Invalid(
                            VmReason.InconsistentStructure,
                            JavaScriptDiagnosticCode.InconsistentStackHeightAtJoin,
                            (ulong)next);
                    }

                    break;
                }
            }

            if (highest > unit.MaxOperandStack)
            {
                return Invalid(
                    VmReason.SemanticValidationFailed,
                    JavaScriptDiagnosticCode.OperandStackOverflow,
                    unit.CodeOffset);
            }

            // THE EXECUTOR SIZES ITS STACK FROM WHAT THIS PASS COMPUTED, never from what the
            // payload declared. The declared figure has been checked against it above and is not
            // needed again.
            unit.MaxOperandStack = (uint)highest;
            return Ok;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=C23B2D
        // Broiler-Human:        PENDING
        private VmVerifierOutcome Seed(
            JsCodeUnit unit,
            byte[] code,
            int target,
            int height,
            int depth,
            System.Collections.Generic.Stack<int> pending,
            int from)
        {
            var start = (int)unit.CodeOffset;
            var end = start + (int)unit.CodeLength;

            if (target < start || target >= end)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.JumpTargetNotAnInstructionBoundary,
                    (ulong)from);
            }

            if (height < 0)
            {
                return Invalid(
                    VmReason.SemanticValidationFailed,
                    JavaScriptDiagnosticCode.OperandStackUnderflow,
                    (ulong)from);
            }

            _ = code;
            var local = target - start;

            if (heights[local] == Unvisited)
            {
                heights[local] = height;
                depths[local] = depth;
                pending.Push(target);
                return Ok;
            }

            if (heights[local] != height || depths[local] != depth)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.InconsistentStackHeightAtJoin,
                    (ulong)target);
            }

            return Ok;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0DA715
        // Broiler-Human:        PENDING
        private VmVerifierOutcome Check(JsCodeUnit unit, JsOpcode opcode, uint operand, int offset)
        {
            switch (opcode)
            {
                case JsOpcode.LoadConstant:
                    return operand < state.Constants!.Length
                        ? Ok
                        : Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.ConstantIndexOutOfRange,
                            (ulong)offset);

                case JsOpcode.LoadGlobal:
                case JsOpcode.StoreGlobal:
                case JsOpcode.LoadGlobalOrUndefined:
                case JsOpcode.DeclareGlobal:
                case JsOpcode.GetProperty:
                case JsOpcode.SetProperty:
                case JsOpcode.DefineField:
                case JsOpcode.DeleteProperty:
                case JsOpcode.DefineGetter:
                case JsOpcode.DefineSetter:
                case JsOpcode.RequireCoercible:
                    if (operand >= state.Constants!.Length)
                    {
                        return Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.ConstantIndexOutOfRange,
                            (ulong)offset);
                    }

                    // A NAME OPERAND MUST NAME A NAME. Reading a Number constant as a property key
                    // would work by accident here and be a type confusion the first time somebody
                    // changed how a key is stored, so it is refused where it is representable.
                    return NamesAName(operand)
                        ? Ok
                        : Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.ConstantIndexOutOfRange,
                            (ulong)offset);

                // A PRIVATE NAME'S CONSTANT IS ITS DESCRIPTION AND NOTHING READS IT AS A KEY, but it
                // is checked as a name anyway: a Number constant there would be a description no
                // diagnostic could print, and the check costs one comparison at verification time
                // rather than a surprise at the first `TypeError` a private access reports.
                case JsOpcode.NewPrivateName:
                    if (operand >= state.Constants!.Length)
                    {
                        return Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.ConstantIndexOutOfRange,
                            (ulong)offset);
                    }

                    return NamesAName(operand)
                        ? Ok
                        : Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.ConstantIndexOutOfRange,
                            (ulong)offset);

                case JsOpcode.Closure:
                    return operand < units.Length
                        ? Ok
                        : Invalid(
                            VmReason.InconsistentStructure,
                            JavaScriptDiagnosticCode.FunctionIndexOutOfRange,
                            (ulong)offset);

                // ONLY A GENERATOR BODY MAY SUSPEND. The executor allocates the frame a suspension
                // saves itself into from the unit's flag, before a single instruction runs, so an
                // artifact that yields anywhere else is refused here rather than met by a null
                // frame in the middle of the dispatch loop.
                case JsOpcode.Yield:
                    return (unit.Flags & JsFormat.FunctionFlags.Generator) != 0
                        ? Ok
                        : Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.YieldOutsideGenerator,
                            (ulong)offset);

                // A DELEGATION IS CHECKED AGAINST THE GENERATOR FLAG AND NOTHING ELSE, exactly as
                // `Yield` is. The executor picks between two delegation loops on the ASYNC flag -
                // the synchronous one runs between two yields inside one entry into the dispatch
                // loop, the asynchronous one awaits every inner step and re-enters this instruction
                // to continue - and both are driven by a frame this flag already guarantees.
                case JsOpcode.YieldDelegate:
                    return (unit.Flags & JsFormat.FunctionFlags.Generator) != 0
                        ? Ok
                        : Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.YieldOutsideGenerator,
                            (ulong)offset);

                // THE `for await` HEAD IS CHECKED AGAINST THE FLAG ITS OWN `Await` IS CHECKED
                // AGAINST. Four of the five instructions would run perfectly well in an ordinary
                // function - each is a call on an iterator - and the answer would be a promise
                // nobody ever resolved rather than an error anybody could diagnose. Refusing the
                // whole sequence here is what makes "a `for await` head belongs to a body that may
                // await" a property of the format rather than of the lowering that emits one.
                case JsOpcode.IterateStartAsync:
                case JsOpcode.IterateNextAsync:
                case JsOpcode.IterateAwaitStep:
                case JsOpcode.IterateCloseAsync:
                case JsOpcode.IterateCloseCheck:
                    return (unit.Flags & JsFormat.FunctionFlags.Async) != 0
                        ? Ok
                        : Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.AsyncIterationOutsideAsync,
                            (ulong)offset);

                // AND ONLY AN ASYNC BODY MAY AWAIT, checked against the OTHER flag. Two bits and
                // two codes rather than one predicate over "may suspend", because the frame an
                // await suspends into is resumed by the job queue and the frame a yield suspends
                // into is resumed by the guest's own `next` - so a unit with the wrong bit would
                // be handed to a driver that has no way to reach it again, and an author told the
                // wrong bit is missing looks in the wrong place.
                case JsOpcode.Await:
                    return (unit.Flags & JsFormat.FunctionFlags.Async) != 0
                        ? Ok
                        : Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.AwaitOutsideAsync,
                            (ulong)offset);

                case JsOpcode.PushScope:
                case JsOpcode.CopyScope:
                    return operand <= JsFormat.CeilingScopeSlots
                        ? Ok
                        : Invalid(
                            VmReason.InconsistentStructure,
                            JavaScriptDiagnosticCode.ScopeDepthOutOfRange,
                            (ulong)offset);

                case JsOpcode.NewArray:
                    return operand <= JsFormat.CeilingOperandStack
                        ? Ok
                        : Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.OperandStackOverflow,
                            (ulong)offset);

                // AN OPERAND BIT THIS VERSION DOES NOT DEFINE IS AN UNKNOWN FEATURE, and it is
                // answered with the unknown-opcode reason for that reason: the byte names an
                // instruction this reader knows and asks it for behaviour this reader does not
                // have. A `NewClass` whose flags carried an undefined bit would also have a stack
                // effect nothing has agreed on, since the defined bit is what decides it.
                case JsOpcode.NewClass:
                    return operand <= JsOpcodes.ClassIsDerived
                        ? Ok
                        : Invalid(
                            VmReason.UnknownFeature,
                            JavaScriptDiagnosticCode.UnknownOpcode,
                            (ulong)offset);

                // A CLASS ELEMENT'S FLAGS ARE A SET WITH RULES BETWEEN ITS MEMBERS, and the rules
                // are checked rather than resolved. Each of the three pairs below names a bit
                // combination the executor has no behaviour for, and letting one through would mean
                // choosing an arm at run time for an encoding nothing agreed on: a static block
                // that lands on an instance has no `this` to run against; a getter that is also a
                // setter is one function asked to be two; and a public element reaching this
                // instruction at all is one `DefineMethod` should have defined, since only a
                // private element and a field are recorded rather than defined.
                case JsOpcode.DefineClassElement:
                {
                    var block = (operand & JsOpcodes.ElementIsBlock) != 0;
                    var accessor = operand & (JsOpcodes.ElementIsGetter | JsOpcodes.ElementIsSetter);
                    var method = (operand & JsOpcodes.ElementIsMethod) != 0;
                    var isPrivate = (operand & JsOpcodes.ElementIsPrivate) != 0;

                    var consistent = operand <= JsOpcodes.ElementBits &&
                        accessor != (JsOpcodes.ElementIsGetter | JsOpcodes.ElementIsSetter) &&
                        (!block || operand == (JsOpcodes.ElementIsBlock | JsOpcodes.ElementIsStatic)) &&
                        (accessor == 0 || (method && isPrivate)) &&
                        (!method || isPrivate);

                    return consistent
                        ? Ok
                        : Invalid(
                            VmReason.InconsistentStructure,
                            JavaScriptDiagnosticCode.ClassElementFlagsInconsistent,
                            (ulong)offset);
                }

                // A member is a getter, or a setter, or neither - never both. Resolving the pair
                // by precedence would give one encoding two readings.
                case JsOpcode.DefineMethod:
                    return operand <= JsOpcodes.MemberBits &&
                        (operand & (JsOpcodes.MemberIsGetter | JsOpcodes.MemberIsSetter)) !=
                            (JsOpcodes.MemberIsGetter | JsOpcodes.MemberIsSetter)
                        ? Ok
                        : Invalid(
                            VmReason.UnknownFeature,
                            JavaScriptDiagnosticCode.UnknownOpcode,
                            (ulong)offset);

                // A CALL'S ARGUMENT COUNT NEEDS NO CHECK, and saying so is better than a check
                // that cannot fail: the operand is one byte and the format's ceiling is 255, so
                // every encodable count is admissible. A branch here would be a row in the
                // registry no artifact could ever reach.
                case JsOpcode.LoadScoped:
                case JsOpcode.StoreScoped:
                case JsOpcode.InitialiseScoped:
                    // The slot half is bounded by the encoding and by the scope it lands in, which
                    // may be a closure's and therefore outside this unit. The executor bounds it
                    // there; what is checkable here is the depth half.
                    return (operand >> 16) <= MaxScopeDepth
                        ? Ok
                        : Invalid(
                            VmReason.InconsistentStructure,
                            JavaScriptDiagnosticCode.ScopeDepthOutOfRange,
                            (ulong)offset);

                // WHAT IS CHECKABLE HERE IS THE ENCODING AND NOT THE RESOLUTION. The low half must
                // name a name, exactly as every other name-carrying instruction's operand must, so
                // an artifact asking this instruction to search for a Number constant is refused
                // where it is representable. The high half needs no check, for the reason a call's
                // argument count needs none: it is one byte and the scope-depth ceiling is 255, so
                // every encodable bound is admissible.
                //
                // What this pass CANNOT check is the bound's CORRECTNESS - whether it stops at the
                // record the language's own scope rules stop at - because that is a fact about the
                // source the lowering read and not about the bytes. A bound that is too small
                // resolves fewer names dynamically and falls through to the static address, which is
                // the safe direction; a bound that is too large lets an outer `with` shadow a
                // binding, which is a wrong ANSWER and never a reachable slot, because the search
                // reads object records and a declarative record has no names in it to match.
                case JsOpcode.ResolveName:
                    return NamesAName(operand & 0xFFFF)
                        ? Ok
                        : Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.ConstantIndexOutOfRange,
                            (ulong)offset);

                default:
                    _ = unit;
                    return Ok;
            }
        }

        /// <summary>Whether constant <paramref name="operand"/> exists and is a name.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0AB6AF
        // Broiler-Human:        PENDING
        private bool NamesAName(uint operand) =>
            operand < state.Constants!.Length &&
            (state.Names![operand].Length != 0 || IsEmptyStringConstant(operand));

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3676ED
        // Broiler-Human:        PENDING
        private bool IsEmptyStringConstant(uint operand) =>
            state.Constants![operand].IsString && state.Constants[operand].AsString().Length == 0;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7F09E9
        // Broiler-Human:        PENDING
        private static uint Operand(byte[] code, int offset, JsOpcode opcode) =>
            JsOpcodes.Shape(opcode) switch
            {
                JsOperandShape.U8 => code[offset + 1],
                JsOperandShape.U16 => (uint)(code[offset + 1] | (code[offset + 2] << 8)),
                JsOperandShape.U32 => (uint)(
                    code[offset + 1] |
                    (code[offset + 2] << 8) |
                    (code[offset + 3] << 16) |
                    (code[offset + 4] << 24)),

                // The depth goes in the high half and the slot in the low half, so one unsigned
                // integer carries both and every caller unpacks it the same way.
                JsOperandShape.U8U16 => (uint)(
                    (code[offset + 1] << 16) | code[offset + 2] | (code[offset + 3] << 8)),

                _ => 0,
            };
    }
}
