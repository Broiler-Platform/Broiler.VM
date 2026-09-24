// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   61
// Annotated:        61/61
// Exempt:           47
// Human-reviewed:   0/61
// IP risk:          Low
// Security risk:    High
// Criteria:         15/15
// Resource impact:  3/10 max
// Unverified:       61
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=911381
    // Broiler-Human:        PENDING
    internal static VmVerifierOutcome Verify(
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        System.Collections.Immutable.ImmutableArray<string> admittedSurfaces,
        IJsNativeEmitter? emitter,
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

        var manifest = ReadManifest(in descriptor, ref reader, state);

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

        return Link(state, adapter, context, admittedSurfaces, emitter, cancellationToken);
    }

    /// <summary>Reads the artifact's own manifest identity and rules on it.</summary>
    /// <remarks>
    /// <para>
    /// <b>TWO MANIFESTS ARE READ AT THIS FORMAT VERSION AND ONE OF THEM IS A NARROWING OF THE
    /// OTHER.</b> <c>broiler.javascript.wide</c> is the surface this format version was defined
    /// against; <c>broiler.javascript.numeric</c> is the numeric subset a whole artifact can be
    /// compiled from, and it is the same bytecode in the same sections with a smaller language
    /// behind it. One format version, two manifests, and no second verifier - which is the rule
    /// this profile already holds for its two format versions.
    /// </para>
    /// <para>
    /// <b>WHAT THIS PASS DOES NOT DO IS RE-DERIVE THE NUMERIC RESTRICTION, AND A READER IS OWED
    /// THAT PLAINLY.</b> The numeric manifest's exclusions are refusals of SOURCE: every construct
    /// outside it is refused by the front end, at compile time, by name, which roadmap section 6
    /// makes a first-class answer for a construct a front end can see. So an artifact naming this
    /// manifest and carrying an instruction the manifest excludes is not refused HERE; it is
    /// refused by the lowering that would have had to write it, and a payload that reached this
    /// verifier by another route is verified as the version-2 artifact it is. That is a real limit
    /// of this build and it is the reason the emitted-code sections carry the ordinary bytecode
    /// beside them: what a reader of a native payload is entitled to is re-emission equality
    /// against bytecode this verifier did check, not a promise about a manifest nothing recomputed.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=7CFF0B
    // Broiler-Falsified-If: an artifact naming a manifest this build does not accept is admitted, or the descriptor and the payload are allowed to name different ones
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadManifest(
        in VmArtifactDescriptor descriptor, ref VmBoundedReader reader, Sections state)
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

        if (!string.Equals(text, JsFormat.ManifestId, System.StringComparison.Ordinal) &&
            !string.Equals(text, JsNumericManifest.ManifestId, System.StringComparison.Ordinal))
        {
            return Invalid(
                VmReason.UnsupportedFeatureManifest,
                JavaScriptDiagnosticCode.UnsupportedFeatureManifest,
                reader.Position);
        }

        state.ManifestId = text;
        return VmVerifierOutcome.Verified(EmptyState.Instance, VmArtifactSharing.Shareable);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=EE5705
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

        if (kind is < 1 or > 15)
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
            JsFormat.SectionKind.Modules => ReadModules(ref reader, adapter, state),
            JsFormat.SectionKind.NativeCode => ReadNativeCode(ref reader, length, state),
            JsFormat.SectionKind.NativeSymbols => ReadNativeSymbols(ref reader, state),
            JsFormat.SectionKind.EvalScopes => ReadEvalScopes(ref reader, adapter, state),
            JsFormat.SectionKind.ScriptDeclarations => ReadScriptDeclarations(ref reader, state),
            JsFormat.SectionKind.ScriptReferrers => ReadScriptReferrers(ref reader, state),
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=CF70E7
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

                case JsFormat.ConstantTag.BigInt:
                {
                    var outcome = ReadBigInt(ref reader, state, out values[index]);

                    if (outcome.Category != VmOutcome.Normal)
                    {
                        return outcome;
                    }

                    break;
                }

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

    /// <summary>Reads one BigInt constant's payload, after its tag.</summary>
    /// <remarks>
    /// <para>
    /// <b>The numeric manifest refuses the tag outright</b>, as the unknown tag it has always been
    /// there: that manifest is Number values only, and its native form reads a constant as a double.
    /// Under the wide manifest the tag's admission depends on the surfaces, which are a later
    /// section, so the first BigInt constant's position is recorded and the link rules on it
    /// (decision JSD-0033).
    /// </para>
    /// <para>
    /// <b>The width is refused before the bytes are read</b>, and the encoding must be canonical, so
    /// one integer has one spelling and the decoding is linear in a payload the format bounds.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=07C246
    // Broiler-Falsified-If: a BigInt constant wider than the format ceiling, or spelled non-canonically, is admitted
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadBigInt(ref VmBoundedReader reader, Sections state, out JsValue value)
    {
        value = JsValue.Undefined;
        var at = reader.Position;

        if (state.ManifestId == JsNumericManifest.ManifestId)
        {
            return Invalid(VmReason.UnknownFeature, JavaScriptDiagnosticCode.UnknownConstantTag, at);
        }

        if (!reader.TryReadByte(out var sign) || !reader.TryReadVarUInt32(out var length))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (length > JsFormat.CeilingBigIntConstantBytes)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.DeclaredMaximumTooLarge,
                reader.Position);
        }

        if (!ReadRun(ref reader, length, out var magnitude))
        {
            return FromReader(ref reader, reader.Position);
        }

        // ONE INTEGER, ONE SPELLING: the sign is a flag, the most significant byte is not zero, and
        // zero is the empty magnitude with a clear sign - so `-0n`, which the language does not
        // have, cannot be written either.
        if (sign > 1 ||
            (magnitude.Length != 0 && magnitude[^1] == 0) ||
            (magnitude.Length == 0 && sign != 0))
        {
            return Invalid(
                VmReason.MalformedEncoding,
                JavaScriptDiagnosticCode.MalformedBigIntConstant,
                reader.Position);
        }

        state.BigIntConstantAt ??= at;

        value = JsValue.BigInt(JsBigInt.FromConstant(sign == 1, magnitude));
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

    /// <summary>Whether the module rows name <paramref name="unit"/> as a module's body.</summary>
    /// <remarks>
    /// Read off the rows rather than off a flag on the unit, because a flag would be a second place
    /// the same fact is stated and the two could disagree; the records are what make a unit a
    /// module's body, so they are what the question is put to.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=23291E
    // Broiler-Human:        PENDING
    private static bool IsModuleBody(Sections state, uint unit)
    {
        if (state.ModuleRows is not { } rows)
        {
            return false;
        }

        foreach (var row in rows)
        {
            if (row.UnitIndex == unit)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Reads the module records exactly as the payload declares them.</summary>
    /// <remarks>
    /// <b>Nothing is resolved here.</b> This pass answers only whether the bytes are a sequence of
    /// module rows; what a request names and what an export resolves to are questions about the
    /// whole graph, and asking them one row at a time would mean asking them against rows that had
    /// not been read yet.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F86BD3
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadModules(
        ref VmBoundedReader reader, JavaScriptReadAdapter adapter, Sections state)
    {
        if (!reader.TryReadDeclaredCount(out var count))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (count == 0 || count > JsFormat.CeilingModules)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedModuleRow,
                reader.Position);
        }

        var rows = new JsModuleRow[count];
        var imports = 0L;

        for (var index = 0u; index < count; index++)
        {
            if (!adapter.Poll())
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.ReaderStopped,
                    reader.Position);
            }

            if (!reader.TryReadVarUInt32(out var key) ||
                !reader.TryReadVarUInt32(out var unit) ||
                !reader.TryReadVarUInt32(out var initialiser))
            {
                return FromReader(ref reader, reader.Position);
            }

            if (!reader.TryReadDeclaredCount(out var requestCount) ||
                requestCount > JsFormat.CeilingModuleRequests)
            {
                return FromReader(ref reader, reader.Position);
            }

            var specifiers = new uint[requestCount];
            var requests = new uint[requestCount];

            for (var request = 0u; request < requestCount; request++)
            {
                if (!reader.TryReadVarUInt32(out specifiers[request]) ||
                    !reader.TryReadVarUInt32(out requests[request]))
                {
                    return FromReader(ref reader, reader.Position);
                }
            }

            if (!reader.TryReadDeclaredCount(out var importCount) ||
                importCount > JsFormat.CeilingImportEntries)
            {
                return FromReader(ref reader, reader.Position);
            }

            var importRows = new JsImportEntryRow[importCount];

            for (var entry = 0u; entry < importCount; entry++)
            {
                if (!reader.TryReadVarUInt32(out var request) ||
                    !reader.TryReadVarUInt32(out var name) ||
                    !reader.TryReadByte(out var kind))
                {
                    return FromReader(ref reader, reader.Position);
                }

                if (kind > (byte)JsFormat.ImportKind.Namespace)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        reader.Position);
                }

                importRows[entry] = new JsImportEntryRow(request, name, (JsFormat.ImportKind)kind);
            }

            imports += importRows.Length;

            if (imports > JsFormat.CeilingImportEntries)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedModuleRow,
                    reader.Position);
            }

            if (!reader.TryReadDeclaredCount(out var localCount) ||
                localCount > JsFormat.CeilingExportEntries)
            {
                return FromReader(ref reader, reader.Position);
            }

            var locals = new JsLocalExportRow[localCount];

            for (var entry = 0u; entry < localCount; entry++)
            {
                if (!reader.TryReadVarUInt32(out var name) || !reader.TryReadVarUInt32(out var slot))
                {
                    return FromReader(ref reader, reader.Position);
                }

                locals[entry] = new JsLocalExportRow(name, slot);
            }

            if (!reader.TryReadDeclaredCount(out var indirectCount) ||
                indirectCount > JsFormat.CeilingExportEntries)
            {
                return FromReader(ref reader, reader.Position);
            }

            var indirects = new JsIndirectExportRow[indirectCount];

            for (var entry = 0u; entry < indirectCount; entry++)
            {
                if (!reader.TryReadVarUInt32(out var name) ||
                    !reader.TryReadVarUInt32(out var request) ||
                    !reader.TryReadVarUInt32(out var importName) ||
                    !reader.TryReadByte(out var kind))
                {
                    return FromReader(ref reader, reader.Position);
                }

                if (kind > (byte)JsFormat.ImportKind.Namespace)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        reader.Position);
                }

                indirects[entry] =
                    new JsIndirectExportRow(name, request, importName, (JsFormat.ImportKind)kind);
            }

            if (!TryReadKeys(ref reader, JsFormat.CeilingModuleRequests, out var stars))
            {
                return stars is null
                    ? FromReader(ref reader, reader.Position)
                    : Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        reader.Position);
            }

            rows[index] = new JsModuleRow(
                key, unit, initialiser, specifiers, requests, importRows, locals, indirects, stars!);
        }

        state.ModuleRows = rows;
        state.ImportCount = (int)imports;
        return Ok;
    }

    /// <summary>
    /// Reads the emitted-code section: an architecture, a backend version, an alignment, a declared
    /// length, and the bytes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>NOTHING HERE DECODES AN INSTRUCTION AND NOTHING HERE EVER WILL.</b> The core's fourth
    /// invariant keeps it ignorant of any instruction set, and a profile verifier that read machine
    /// code would be a second disassembler with a second opinion. What this pass rules on is
    /// framing: whether the architecture is one this build names, whether the alignment is an
    /// alignment, and whether the length the backend declared is the length the section carries.
    /// </para>
    /// <para>
    /// <b>So a well-framed sequence of the WRONG instructions is accepted here, and that is stated
    /// rather than left to be discovered.</b> The answer to a wrong backend is re-emission equality
    /// against the bytecode the artifact also carries and a differential run of the two forms, and
    /// neither of those is a verification pass. A composition that runs emitted code is trusting a
    /// producer, not this method.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=E63499
    // Broiler-Falsified-If: a declared length that disagrees with the bytes present is accepted, or an architecture value this build cannot name is
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadNativeCode(
        ref VmBoundedReader reader, ulong length, Sections state)
    {
        if (length < 16 || length - 16 > JsFormat.CeilingNativeCodeBytes)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedNativeSection,
                reader.Position);
        }

        if (!TryReadFixedU32(ref reader, out var field) ||
            !TryReadFixedU32(ref reader, out var backendVersion) ||
            !TryReadFixedU32(ref reader, out var alignment) ||
            !TryReadFixedU32(ref reader, out var declared))
        {
            return FromReader(ref reader, reader.Position);
        }

        // AN ALIGNMENT OF ZERO IS NOT "NO ALIGNMENT" AND ONE THAT IS NOT A POWER OF TWO IS NOT AN
        // ALIGNMENT AT ALL. Both would be rounded silently by anything that used them, and a
        // rounding a producer did not ask for is a producer and a consumer disagreeing about where
        // a code unit begins.
        // THE FIRST FIELD IS AN ARCHITECTURE AND A FORM BYTE ABOVE IT (JSD-0035 section 9). A field
        // this build cannot split - a nonzero upper half, or a form byte naming neither the manifest's
        // own form nor the value form - is refused here as the malformed header it is.
        if (!JsNativeCodeHeader.TryUnpack(field, out var architecture, out var valueForm) ||
            architecture == JsNativeArchitecture.None ||
            architecture > JsNativeArchitecture.Arm64 ||
            alignment == 0 ||
            alignment > JsFormat.CeilingNativeCodeAlignment ||
            (alignment & (alignment - 1)) != 0 ||
            declared != length - 16)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedNativeSection,
                reader.Position);
        }

        if (!ReadRun(ref reader, declared, out var body))
        {
            return FromReader(ref reader, reader.Position);
        }

        state.NativeArchitecture = architecture;
        state.NativeValueForm = valueForm;
        state.NativeBackendVersion = backendVersion;
        state.NativeCodeAlignment = alignment;
        state.NativeCode = body;
        return Ok;
    }

    /// <summary>Reads the emitted-code symbol section: which code unit each run of bytes is.</summary>
    /// <remarks>
    /// <b>The rows must ascend by code unit and each offset must be inside the blob, and the
    /// ascent is what makes the runs tile it.</b> A symbol carries no length, so a run ends where
    /// the next one begins and the last ends at the blob; rows in any other order would leave the
    /// extent of a unit's code a question of which row a reader happened to look at next.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F1C392
    // Broiler-Falsified-If: an offset outside the emitted blob is accepted, or two rows naming one code unit are
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadNativeSymbols(ref VmBoundedReader reader, Sections state)
    {
        if (!TryReadFixedU32(ref reader, out var count))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (count > JsFormat.CeilingNativeSymbols)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.DeclaredMaximumTooLarge,
                reader.Position);
        }

        var rows = new JsNativeSymbolRow[count];
        var previousUnit = -1L;

        for (var index = 0u; index < count; index++)
        {
            if (!TryReadFixedU32(ref reader, out var unit) ||
                !TryReadFixedU32(ref reader, out var offset))
            {
                return FromReader(ref reader, reader.Position);
            }

            if (unit <= previousUnit)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedNativeSection,
                    reader.Position);
            }

            previousUnit = unit;
            rows[index] = new JsNativeSymbolRow(unit, offset);
        }

        state.NativeSymbols = rows;
        return Ok;
    }

    /// <summary>Reads one fixed-width little-endian <c>u32</c>.</summary>
    /// <remarks>
    /// <b>The emitted sections are the only ones of this format that use a fixed width, and the
    /// reason is on the writer.</b> A backend learns how long its output is only after it has
    /// written it, so the length is a field to be patched in place; a variable-length field cannot
    /// be patched without moving what follows it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0EEC3C
    // Broiler-Human:        PENDING
    private static bool TryReadFixedU32(ref VmBoundedReader reader, out uint value)
    {
        value = 0;

        if (!reader.TryReadBytes(4, out var bytes))
        {
            return false;
        }

        value = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes);
        return true;
    }

    /// <summary>Reads the eval scope map's three tables exactly as the payload declares them.</summary>
    /// <remarks>
    /// <b>Only the encoding is judged here</b>: counts within the ceiling, a kind, a flag byte and a
    /// refusal this build defines. What a row NAMES - a constant, a code unit, an offset, another
    /// row - is judged by <see cref="LinkEvalScopes"/>, after every table it could name has been
    /// read (JSeal V14, JSD-0026 step 2).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=D1DE97
    // Broiler-Falsified-If: a row of a kind, flag or refusal this build does not define is read as one it does
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadEvalScopes(
        ref VmBoundedReader reader, JavaScriptReadAdapter adapter, Sections state)
    {
        if (!reader.TryReadDeclaredCount(out var shapeCount))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (shapeCount > JsFormat.CeilingEvalScopeRows)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedEvalScopes,
                reader.Position);
        }

        var shapes = new JsEvalScopeRow[shapeCount];

        for (var index = 0u; index < shapeCount; index++)
        {
            if (!adapter.Poll())
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.ReaderStopped,
                    reader.Position);
            }

            if (!reader.TryReadByte(out var kind) ||
                !reader.TryReadVarUInt32(out var parent) ||
                !reader.TryReadDeclaredCount(out var nameCount))
            {
                return FromReader(ref reader, reader.Position);
            }

            if (kind is < (byte)JsFormat.EvalScopeKind.Function or > (byte)JsFormat.EvalScopeKind.Module ||
                nameCount > JsFormat.CeilingScopeSlots)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedEvalScopes,
                    reader.Position);
            }

            var names = new JsEvalNameRow[nameCount];

            for (var name = 0u; name < nameCount; name++)
            {
                if (!reader.TryReadVarUInt32(out var constant) ||
                    !reader.TryReadVarUInt32(out var slot) ||
                    !reader.TryReadByte(out var flags))
                {
                    return FromReader(ref reader, reader.Position);
                }

                // AN IMPORT IS IMMUTABLE AND BELONGS TO A MODULE'S ROW ALONE (JSeal V15-module): its
                // slot is an import entry, which means nothing in any other kind of record.
                if ((flags & ~JsFormat.EvalBindingFlagBits) != 0 ||
                    ((flags & JsFormat.EvalBindingFunctionName) != 0 &&
                     (flags & JsFormat.EvalBindingImmutable) == 0) ||
                    ((flags & JsFormat.EvalBindingImport) != 0 &&
                     (kind != (byte)JsFormat.EvalScopeKind.Module ||
                      (flags & JsFormat.EvalBindingImmutable) == 0)))
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedEvalScopes,
                        reader.Position);
                }

                names[name] = new JsEvalNameRow(constant, slot, flags);
            }

            shapes[index] = new JsEvalScopeRow((JsFormat.EvalScopeKind)kind, parent, names);
        }

        if (!reader.TryReadDeclaredCount(out var siteCount))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (siteCount > JsFormat.CeilingEvalScopeRows)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedEvalScopes,
                reader.Position);
        }

        var sites = new JsEvalSiteRow[siteCount];

        for (var index = 0u; index < siteCount; index++)
        {
            if (!reader.TryReadVarUInt32(out var unit) ||
                !reader.TryReadVarUInt32(out var offset) ||
                !reader.TryReadVarUInt32(out var scope) ||
                !reader.TryReadVarUInt32(out var depth) ||
                !reader.TryReadByte(out var flags))
            {
                return FromReader(ref reader, reader.Position);
            }

            if ((flags & ~(byte)JsFormat.EvalRequestFlagBits) != 0)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedEvalScopes,
                    reader.Position);
            }

            sites[index] = new JsEvalSiteRow(
                unit, offset, scope, depth, (JsFormat.EvalRequestFlags)flags);
        }

        if (!reader.TryReadDeclaredCount(out var declarationCount))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (declarationCount > JsFormat.CeilingEvalScopeRows)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedEvalScopes,
                reader.Position);
        }

        var declarations = new JsEvalDeclarationRow[declarationCount];

        for (var index = 0u; index < declarationCount; index++)
        {
            if (!reader.TryReadVarUInt32(out var unit) ||
                !reader.TryReadByte(out var flags) ||
                !reader.TryReadByte(out var refusal))
            {
                return FromReader(ref reader, reader.Position);
            }

            if ((flags & ~(byte)JsFormat.EvalRequestFlagBits) != 0 ||
                refusal > (byte)JsFormat.EvalRefusal.PrivateName)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedEvalScopes,
                    reader.Position);
            }

            // A NULL RUN IS A READER FAILURE AND AN EMPTY ONE A COUNT PAST THE CEILING, which is how
            // TryReadKeys tells the two apart for every caller.
            if (!TryReadKeys(ref reader, JsFormat.CeilingScopeSlots, out var varNames))
            {
                return varNames is null
                    ? FromReader(ref reader, reader.Position)
                    : Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedEvalScopes,
                        reader.Position);
            }

            if (!TryReadKeys(ref reader, JsFormat.CeilingScopeSlots, out var lexicalNames))
            {
                return lexicalNames is null
                    ? FromReader(ref reader, reader.Position)
                    : Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedEvalScopes,
                        reader.Position);
            }

            if (!TryReadKeys(ref reader, JsFormat.CeilingScopeSlots, out var functionNames))
            {
                return functionNames is null
                    ? FromReader(ref reader, reader.Position)
                    : Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedEvalScopes,
                        reader.Position);
            }

            if (!TryReadKeys(ref reader, JsFormat.CeilingScopeSlots, out var annexBNames))
            {
                return annexBNames is null
                    ? FromReader(ref reader, reader.Position)
                    : Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedEvalScopes,
                        reader.Position);
            }

            if (!TryReadKeys(ref reader, JsFormat.CeilingScopeSlots, out var privateNames))
            {
                return privateNames is null
                    ? FromReader(ref reader, reader.Position)
                    : Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedEvalScopes,
                        reader.Position);
            }

            declarations[index] = new JsEvalDeclarationRow(
                unit,
                (JsFormat.EvalRequestFlags)flags,
                (JsFormat.EvalRefusal)refusal,
                varNames!,
                lexicalNames!,
                functionNames!,
                annexBNames!,
                privateNames!);
        }

        state.EvalScopeRows = shapes;
        state.EvalSiteRows = sites;
        state.EvalDeclarationRows = declarations;
        return Ok;
    }

    /// <summary>Whether constant <paramref name="constant"/> exists and spells a non-empty name.</summary>
    /// <remarks>
    /// Stricter than an instruction's name operand, which may also be the empty String: a scope map
    /// row spells a binding, and no binding is spelled with nothing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=865AE1
    // Broiler-Human:        PENDING
    private static bool IsInternedName(Sections state, uint constant) =>
        constant < state.Constants!.Length && state.Names![constant].Length != 0;

    /// <summary>
    /// Whether <paramref name="offset"/> is an instruction boundary of <paramref name="unit"/> that
    /// holds a direct-eval call.
    /// </summary>
    /// <remarks>
    /// <b>A decode from the unit's first byte, not a reachability walk</b>: the lowering may leave a
    /// site behind a <c>return</c>, where the abstract pass never goes, and the question here is
    /// only whether the row names an instruction at all. Every instruction has a width its opcode
    /// alone decides, so the decode is exact; each step is charged.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=D3432C
    // Broiler-Falsified-If: it answers true for an offset inside an instruction, past the unit, or holding anything but CallEval or CallEvalSpread
    // Broiler-Human:        PENDING
    private static bool IsEvalCall(
        byte[] code, JsCodeUnit unit, uint offset, JavaScriptReadAdapter adapter)
    {
        var at = unit.CodeOffset;
        var end = unit.CodeOffset + unit.CodeLength;

        while (at < offset && at < end)
        {
            if (!adapter.TryChargeWork(1) || !JsOpcodes.IsDefined(code[at]))
            {
                return false;
            }

            at += (uint)JsOpcodes.InstructionWidth((JsOpcode)code[at]);
        }

        return at == offset && at < end &&
            (JsOpcode)code[at] is JsOpcode.CallEval or JsOpcode.CallEvalSpread;
    }

    /// <summary>Reads a counted run of unsigned integers, refusing one past a ceiling.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=38BC95
    // Broiler-Human:        PENDING
    private static bool TryReadKeys(ref VmBoundedReader reader, uint ceiling, out uint[]? values)
    {
        values = null;

        if (!reader.TryReadDeclaredCount(out var count))
        {
            return false;
        }

        if (count > ceiling)
        {
            values = [];
            return false;
        }

        var read = new uint[count];

        for (var index = 0u; index < count; index++)
        {
            if (!reader.TryReadVarUInt32(out read[index]))
            {
                return false;
            }
        }

        values = read;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5A6EF8
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome Link(
        Sections state,
        JavaScriptReadAdapter adapter,
        IVmVerificationContext context,
        System.Collections.Immutable.ImmutableArray<string> admittedSurfaces,
        IJsNativeEmitter? emitter,
        System.Threading.CancellationToken cancellationToken)
    {
        if (!state.SawLimits || !state.SawConstants || !state.SawCode ||
            !state.SawEntries || !state.SawFunctions)
        {
            return Invalid(
                VmReason.InconsistentStructure, JavaScriptDiagnosticCode.MissingSection, 0);
        }

        // A BIGINT CONSTANT IS ADMITTED ONLY BESIDE THE BIGINT SURFACE, and without it the answer
        // is the one every build before the tag gave: an unknown constant tag, at the constant. The
        // surfaces section follows the constants, which is why this is ruled on here and not there;
        // the composition's own admission of the surface was ruled on when that section was read.
        if (state.BigIntConstantAt is { } bigIntAt && !state.DeclaresBigInt())
        {
            return Invalid(
                VmReason.UnknownFeature,
                JavaScriptDiagnosticCode.UnknownConstantTag,
                bigIntAt);
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

            // AN ASYNC UNIT IS NOT A CONSTRUCTOR, AND NEITHER THE ARROW NOR THE GENERATOR IS ON
            // THE LIST. The arrow never was: an async ARROW is an ordinary arrow whose body may
            // suspend, and the executor enters it exactly as it enters any arrow - with the
            // lexical `this` and `new.target` its closure recorded. The GENERATOR was, and dropping
            // it is what admitted the async generator: the pair does not ask the executor to choose
            // between the generator driver and the async one, it names a THIRD driver whose caller
            // pulls with `next` and whose body settles the promise that pull answered.
            // `Generator | Arrow` stays refused by the check above, which is what keeps the one
            // combination the grammar has no production for out.
            //
            // A MODULE BODY IS THE ONE PROGRAM BODY THAT MAY BE ASYNC, and until the module goal
            // existed no program body could be. `ProgramBody | Async` was refused here because the
            // only program bodies were scripts, which have no driver and no caller to hand a
            // promise to; a module has both - it is entered by the linker, which holds the promise
            // its evaluation answers with and orders the graph on it. So the combination is
            // admitted exactly for a unit the module records name as a body, and refused everywhere
            // else, which is what keeps a script from claiming a driver nothing would supply.
            if ((unitFlags & JsFormat.FunctionFlags.Async) != 0 &&
                ((unitFlags & JsFormat.FunctionFlags.Constructible) != 0 ||
                    ((unitFlags & JsFormat.FunctionFlags.ProgramBody) != 0 &&
                        !IsModuleBody(state, (uint)index))))
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.AsyncFlagsInconsistent,
                    (ulong)index);
            }

            // AN EVAL-CODE UNIT IS AN EVALUATED PROGRAM'S ENTRY AND NOTHING ELSE. It is entered by
            // a direct evaluation with its caller's eval view and no arguments, so every flag that
            // would send it through another driver, give it parameters or make it a script or module
            // body contradicts the one bit that says what it is (JSeal V14).
            if ((unitFlags & JsFormat.FunctionFlags.EvalCode) != 0 &&
                ((unitFlags & (JsFormat.FunctionFlags.ProgramBody |
                    JsFormat.FunctionFlags.Arrow |
                    JsFormat.FunctionFlags.Constructible |
                    JsFormat.FunctionFlags.ClassConstructor |
                    JsFormat.FunctionFlags.DerivedConstructor |
                    JsFormat.FunctionFlags.BindsParameters |
                    JsFormat.FunctionFlags.UsesArguments |
                    JsFormat.FunctionFlags.Generator |
                    JsFormat.FunctionFlags.Async)) != 0 ||
                    row.ParameterCount != 0))
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedFunctionRow,
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

        var evalMap = LinkEvalScopes(state, units, adapter, out var linkedEvalMap);

        if (evalMap.Category != VmOutcome.Normal)
        {
            return evalMap;
        }

        var scriptDeclarations = LinkScriptDeclarations(state, units, adapter, out var linkedScripts);

        if (scriptDeclarations.Category != VmOutcome.Normal)
        {
            return scriptDeclarations;
        }

        var scriptReferrers = LinkScriptReferrers(state, units, adapter, out var linkedReferrers);

        if (scriptReferrers.Category != VmOutcome.Normal)
        {
            return scriptReferrers;
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
        var modules = System.Array.Empty<JsModuleRecord>();
        var bindings = System.Array.Empty<JsBinding>();

        // A MODULE SECTION AND A MODULE SURFACE ARE TWO DECLARATIONS AND EITHER WITHOUT THE OTHER
        // IS AN ARTIFACT THAT CONTRADICTS ITSELF. The section says the graph is here; the surface
        // says the composition was asked whether it admits one. Records without the declaration
        // would run a module graph in a composition that declined modules, and the declaration
        // without records would be a program declaring a surface it does not reach.
        if (state.DeclaresModules())
        {
            var linked = LinkModules(state, units, context, adapter, out modules, out bindings);

            if (linked.Category != VmOutcome.Normal)
            {
                return linked;
            }
        }
        else if (state.ModuleRows is not null)
        {
            return Invalid(
                VmReason.UnknownFeature,
                JavaScriptDiagnosticCode.ModuleSectionOutsideManifest,
                0);
        }

        var native = LinkNative(state, units, emitter);

        if (native.Category != VmOutcome.Normal)
        {
            return native;
        }

        var program = new JsProgram(
            state.Constants!,
            state.Names!,
            code,
            units,
            state.Regions,
            state.Entries,
            state.PositionRows,
            admittedSurfaces,
            modules,
            bindings,
            state.ManifestId,
            state.NativeArchitecture,
            state.NativeBackendVersion,
            state.NativeCodeAlignment,
            state.NativeCode ?? [],
            state.NativeSymbols ?? [],
            linkedEvalMap,
            linkedScripts,
            linkedReferrers,
            state.NativeValueForm);

        return VmVerifierOutcome.Verified(program, VmArtifactSharing.Shareable);
    }

    /// <summary>
    /// Holds the eval scope map to the code, the function table and itself, and builds what the
    /// executor reads (JSeal V14, JSD-0026 section 4).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>What the bytes can show is checked, and nothing else is claimed.</b> Every shape is a kind
    /// this build defines, a root has no parent and every other row has an EARLIER one, and every
    /// name is an interned name bound once per row. Every site is in its unit, on an instruction
    /// boundary holding a <see cref="JsOpcode.CallEval"/> or <see cref="JsOpcode.CallEvalSpread"/>,
    /// named once, strict exactly when its unit is, and its chain reaches its unit's own root - a
    /// function row for a function, a program row for a script body, an eval row for eval code - in
    /// exactly its declared depth, through rows that are blocks, catch clauses and <c>with</c>s. The
    /// depth itself is held to the abstract pass's by <see cref="Walker"/>. Every declaration row
    /// belongs to an eval-code unit, and every eval-code unit has exactly one.
    /// </para>
    /// <para>
    /// <b>A slot is not bounded here</b>, because the record a row describes may be a closure's
    /// outside the unit; the executor bounds it where it reads one. So a wrong map is a wrong answer
    /// or an internal defect, and never an unowned read.
    /// </para>
    /// <para>
    /// <b>Every unit of work is charged</b>: one per row and one per name, and one per instruction
    /// the boundary decode walks, so a map the size of the ceiling is paid for by the artifact that
    /// carries it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=BF07DB
    // Broiler-Falsified-If: an eval scope map row that names a non-name constant, a unit, an offset or a row it may not name reaches the executor
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome LinkEvalScopes(
        Sections state, JsCodeUnit[] units, JavaScriptReadAdapter adapter, out JsEvalMap? map)
    {
        map = null;

        foreach (var unit in units)
        {
            if ((unit.Flags & JsFormat.FunctionFlags.EvalCode) != 0)
            {
                state.HasEvalCode = true;
            }
        }

        if (state.EvalScopeRows is not { } rows)
        {
            // NO MAP AND AN EVAL-CODE UNIT is an evaluated program with no declaration row, which
            // the executor could not bind an answer to.
            return state.HasEvalCode
                ? Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedEvalScopes,
                    0)
                : Ok;
        }

        if (!state.DeclaresDynamic())
        {
            return Invalid(
                VmReason.UnknownFeature,
                JavaScriptDiagnosticCode.EvalScopesOutsideManifest,
                0);
        }

        var shapes = new JsEvalShape[rows.Length];

        for (var index = 0; index < rows.Length; index++)
        {
            var row = rows[index];
            var root = row.Kind is JsFormat.EvalScopeKind.Program or JsFormat.EvalScopeKind.Eval or
                JsFormat.EvalScopeKind.Module;

            if (!adapter.TryChargeWork(1 + (ulong)row.Names.Length))
            {
                return VmVerifierOutcome.ResourceExhaustion(
                    VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);
            }

            if (root != (row.Parent == 0) || (row.Parent != 0 && row.Parent - 1 >= (uint)index) ||
                (row.Kind == JsFormat.EvalScopeKind.With && row.Names.Length != 0))
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedEvalScopes,
                    (ulong)index);
            }

            var names = new System.Collections.Generic.Dictionary<
                string, (int Slot, bool Immutable, bool Lexical, bool Hidden, bool FunctionName, bool Import)>(
                row.Names.Length, System.StringComparer.Ordinal);

            foreach (var name in row.Names)
            {
                // AN IMPORT'S SLOT IS AN ENTRY OF THE ARTIFACT'S IMPORT TABLE, and it is bounded by
                // that table here, exactly as a `LoadImport` operand is (JSeal V15-module); every
                // other slot is the executor's to bound, since the record may be a closure's.
                var import = (name.Flags & JsFormat.EvalBindingImport) != 0;

                if (!IsInternedName(state, name.NameConstant) ||
                    (import ? name.Slot >= (uint)state.ImportCount : name.Slot > JsFormat.CeilingScopeSlots) ||
                    !names.TryAdd(
                        state.Names![name.NameConstant],
                        (
                            (int)name.Slot,
                            (name.Flags & JsFormat.EvalBindingImmutable) != 0,
                            (name.Flags & JsFormat.EvalBindingLexical) != 0,
                            (name.Flags & JsFormat.EvalBindingHidden) != 0,
                            (name.Flags & JsFormat.EvalBindingFunctionName) != 0,
                            import)))
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedEvalScopes,
                        (ulong)index);
                }
            }

            shapes[index] = new JsEvalShape(row.Kind, (int)row.Parent - 1, names);
        }

        var sites = new System.Collections.Generic.Dictionary<uint, JsEvalSite>(
            state.EvalSiteRows!.Length);

        var previous = -1L;

        foreach (var site in state.EvalSiteRows)
        {
            if (!adapter.TryChargeWork(1 + site.Depth))
            {
                return VmVerifierOutcome.ResourceExhaustion(
                    VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);
            }

            if (site.Offset <= previous || site.FunctionIndex >= units.Length ||
                site.Scope >= shapes.Length || site.Depth > MaxScopeDepth)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedEvalScopes,
                    site.Offset);
            }

            previous = site.Offset;
            var unit = units[site.FunctionIndex];
            var strict = (site.Flags & JsFormat.EvalRequestFlags.Strict) != 0;

            if (site.Offset < unit.CodeOffset ||
                site.Offset >= unit.CodeOffset + unit.CodeLength ||
                strict != unit.IsStrict ||
                !ReachesRoot(state, shapes, site, unit) ||
                !IsEvalCall(state.Code!, unit, site.Offset, adapter))
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedEvalScopes,
                    site.Offset);
            }

            sites.Add(site.Offset, new JsEvalSite((int)site.Scope, site.Flags));
            state.EvalSiteDepths[site.Offset] = site.Depth;
        }

        var declarations = new System.Collections.Generic.Dictionary<int, JsEvalDeclaration>(
            state.EvalDeclarationRows!.Length);

        foreach (var declaration in state.EvalDeclarationRows)
        {
            if (!adapter.TryChargeWork(
                    1 + (ulong)declaration.VarNameConstants.Length +
                    (ulong)declaration.LexicalNameConstants.Length +
                    (ulong)declaration.FunctionNameConstants.Length +
                    (ulong)declaration.AnnexBNameConstants.Length +
                    (ulong)declaration.PrivateNameConstants.Length))
            {
                return VmVerifierOutcome.ResourceExhaustion(
                    VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);
            }

            var named = System.Array.TrueForAll(
                    declaration.VarNameConstants, constant => IsInternedName(state, constant)) &&
                System.Array.TrueForAll(
                    declaration.LexicalNameConstants, constant => IsInternedName(state, constant)) &&
                System.Array.TrueForAll(
                    declaration.FunctionNameConstants, constant => IsInternedName(state, constant)) &&
                System.Array.TrueForAll(
                    declaration.AnnexBNameConstants, constant => IsInternedName(state, constant)) &&
                // A PRIVATE NAME IS SPELLED AS ITS SLOT, `##` and then the name, which no source can
                // write as an identifier - so a row cannot use this list to look up an ordinary
                // binding of its caller (JSeal V15-finish).
                System.Array.TrueForAll(
                    declaration.PrivateNameConstants,
                    constant => IsInternedName(state, constant) &&
                        state.Names![constant].Length > 2 &&
                        state.Names![constant].StartsWith("##", System.StringComparison.Ordinal));

            // A STRICT PROGRAM DECLARES NOTHING OUTSIDE ITSELF: its `var`s and functions are slots of
            // its own record, so a strict unit whose row names any is a row the executor would
            // instantiate into its caller's scope for code the language isolates (JSeal V15).
            var introduces = declaration.VarNameConstants.Length != 0 ||
                declaration.FunctionNameConstants.Length != 0 ||
                declaration.AnnexBNameConstants.Length != 0;

            if (declaration.FunctionIndex >= units.Length ||
                (units[declaration.FunctionIndex].Flags & JsFormat.FunctionFlags.EvalCode) == 0 ||
                // A STRICT REQUEST MAKES STRICT CODE, AND A SLOPPY ONE MAY STILL: the evaluated
                // source's own directive prologue can make it strict whatever its caller is.
                ((declaration.Flags & JsFormat.EvalRequestFlags.Strict) != 0 &&
                    !units[declaration.FunctionIndex].IsStrict) ||
                (introduces && units[declaration.FunctionIndex].IsStrict) ||
                !named ||
                !declarations.TryAdd(
                    (int)declaration.FunctionIndex,
                    new JsEvalDeclaration(
                        declaration.Flags,
                        declaration.Refusal,
                        Spelled(state, declaration.VarNameConstants),
                        Spelled(state, declaration.FunctionNameConstants),
                        Spelled(state, declaration.AnnexBNameConstants),
                        Spelled(state, declaration.PrivateNameConstants))))
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedEvalScopes,
                    declaration.FunctionIndex);
            }
        }

        for (var index = 0; index < units.Length; index++)
        {
            if ((units[index].Flags & JsFormat.FunctionFlags.EvalCode) != 0 &&
                !declarations.ContainsKey(index))
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedEvalScopes,
                    (ulong)index);
            }
        }

        map = new JsEvalMap(shapes, sites, declarations);
        return Ok;
    }

    /// <summary>Reads the script-declarations section (JSeal V15-host) into its rows.</summary>
    /// <remarks>
    /// Only the framing is judged here - counts within their ceilings, integers that decode; what a
    /// row may name is judged by <see cref="LinkScriptDeclarations"/>, once the function table and the
    /// pool exist.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=CA920C
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadScriptDeclarations(ref VmBoundedReader reader, Sections state)
    {
        if (!reader.TryReadDeclaredCount(out var rowCount))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (rowCount > JsFormat.CeilingFunctions)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedScriptDeclarations,
                reader.Position);
        }

        var rows = new JsScriptDeclarationRow[rowCount];

        for (var index = 0u; index < rowCount; index++)
        {
            if (!reader.TryReadVarUInt32(out var unit))
            {
                return FromReader(ref reader, reader.Position);
            }

            var runs = new uint[4][];

            for (var run = 0; run < runs.Length; run++)
            {
                // A NULL RUN IS A READER FAILURE AND AN EMPTY ONE A COUNT PAST THE CEILING, which is
                // how TryReadKeys tells the two apart for every caller.
                if (!TryReadKeys(ref reader, JsFormat.CeilingScopeSlots, out var names))
                {
                    return names is null
                        ? FromReader(ref reader, reader.Position)
                        : Invalid(
                            VmReason.InconsistentStructure,
                            JavaScriptDiagnosticCode.MalformedScriptDeclarations,
                            reader.Position);
                }

                runs[run] = names!;
            }

            rows[index] = new JsScriptDeclarationRow(unit, runs[0], runs[1], runs[2], runs[3]);
        }

        state.ScriptDeclarationRows = rows;
        return Ok;
    }

    /// <summary>
    /// Holds the script-declarations rows to the function table and the pool, and builds what the
    /// executor's global instantiation reads.
    /// </summary>
    /// <remarks>
    /// <b>A row belongs to a script body and to nothing else</b>: a program-body unit that is not
    /// eval code and is neither a module's body nor its initialiser, named once. Every name is an
    /// interned name, and a strict body carries no Annex B candidate. Each row is charged by the
    /// names it carries, so the link is metered like the rest of the pass.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=A345C7
    // Broiler-Falsified-If: a script-declarations row that names a unit other than a script body, or a constant that is not an interned name, reaches the executor
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome LinkScriptDeclarations(
        Sections state,
        JsCodeUnit[] units,
        JavaScriptReadAdapter adapter,
        out System.Collections.Generic.Dictionary<int, JsScriptDeclaration>? scripts)
    {
        scripts = null;

        if (state.ScriptDeclarationRows is not { } rows)
        {
            return Ok;
        }

        var linked = new System.Collections.Generic.Dictionary<int, JsScriptDeclaration>(rows.Length);

        foreach (var row in rows)
        {
            if (!adapter.TryChargeWork(
                    1 + (ulong)row.LexicalNameConstants.Length + (ulong)row.VarNameConstants.Length +
                    (ulong)row.FunctionNameConstants.Length + (ulong)row.AnnexBNameConstants.Length))
            {
                return VmVerifierOutcome.ResourceExhaustion(
                    VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);
            }

            var named = System.Array.TrueForAll(
                    row.LexicalNameConstants, constant => IsInternedName(state, constant)) &&
                System.Array.TrueForAll(row.VarNameConstants, constant => IsInternedName(state, constant)) &&
                System.Array.TrueForAll(
                    row.FunctionNameConstants, constant => IsInternedName(state, constant)) &&
                System.Array.TrueForAll(
                    row.AnnexBNameConstants, constant => IsInternedName(state, constant));

            if (row.FunctionIndex >= units.Length ||
                (units[row.FunctionIndex].Flags &
                    (JsFormat.FunctionFlags.ProgramBody | JsFormat.FunctionFlags.EvalCode)) !=
                    JsFormat.FunctionFlags.ProgramBody ||
                IsModuleUnit(state, row.FunctionIndex) ||
                (row.AnnexBNameConstants.Length != 0 && units[row.FunctionIndex].IsStrict) ||
                !named ||
                !linked.TryAdd(
                    (int)row.FunctionIndex,
                    new JsScriptDeclaration(
                        Spelled(state, row.LexicalNameConstants),
                        Spelled(state, row.VarNameConstants),
                        Spelled(state, row.FunctionNameConstants),
                        Spelled(state, row.AnnexBNameConstants))))
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedScriptDeclarations,
                    row.FunctionIndex);
            }
        }

        scripts = linked;
        return Ok;
    }

    /// <summary>Reads the script-referrers section (JSeal I12-upstream) into its rows.</summary>
    /// <remarks>
    /// Only the framing is judged here; what a row may name is judged by
    /// <see cref="LinkScriptReferrers"/>, once the function table and the pool exist.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D92BAD
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReadScriptReferrers(ref VmBoundedReader reader, Sections state)
    {
        if (!reader.TryReadDeclaredCount(out var rowCount))
        {
            return FromReader(ref reader, reader.Position);
        }

        if (rowCount > JsFormat.CeilingFunctions)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedScriptReferrers,
                reader.Position);
        }

        var rows = new (uint FunctionIndex, uint ReferrerConstant)[rowCount];

        for (var index = 0u; index < rowCount; index++)
        {
            if (!reader.TryReadVarUInt32(out var unit) || !reader.TryReadVarUInt32(out var referrer))
            {
                return FromReader(ref reader, reader.Position);
            }

            rows[index] = (unit, referrer);
        }

        state.ScriptReferrerRows = rows;
        return Ok;
    }

    /// <summary>
    /// Holds the script-referrers rows to the function table and the pool, and builds what the
    /// executor reads when code with no referrer of its own asks for the running script's.
    /// </summary>
    /// <remarks>
    /// <b>A row belongs to a script body and to nothing else</b>, as a script-declarations row does:
    /// a program-body unit that is not eval code and is neither a module's body nor its initialiser,
    /// named once, placed at a non-empty interned name. Each row is charged one unit of work.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=806351
    // Broiler-Falsified-If: a script-referrers row that names a unit other than a script body, or a constant that is not a non-empty interned name, reaches the executor
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome LinkScriptReferrers(
        Sections state,
        JsCodeUnit[] units,
        JavaScriptReadAdapter adapter,
        out System.Collections.Generic.Dictionary<int, string>? referrers)
    {
        referrers = null;

        if (state.ScriptReferrerRows is not { } rows)
        {
            return Ok;
        }

        var linked = new System.Collections.Generic.Dictionary<int, string>(rows.Length);

        foreach (var (function, referrer) in rows)
        {
            if (!adapter.TryChargeWork(1))
            {
                return VmVerifierOutcome.ResourceExhaustion(
                    VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);
            }

            if (function >= units.Length ||
                (units[function].Flags &
                    (JsFormat.FunctionFlags.ProgramBody | JsFormat.FunctionFlags.EvalCode)) !=
                    JsFormat.FunctionFlags.ProgramBody ||
                IsModuleUnit(state, function) ||
                !IsInternedName(state, referrer) ||
                state.Names![referrer].Length == 0 ||
                !linked.TryAdd((int)function, state.Names![referrer]))
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedScriptReferrers,
                    function);
            }
        }

        referrers = linked;
        return Ok;
    }

    /// <summary>Whether a module record names <paramref name="unit"/> as its body or its initialiser.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1741A3
    // Broiler-Human:        PENDING
    private static bool IsModuleUnit(Sections state, uint unit)
    {
        if (state.ModuleRows is not { } modules)
        {
            return false;
        }

        foreach (var module in modules)
        {
            if (module.UnitIndex == unit || module.InitialiserUnitIndex == unit)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>The names a run of interned-name constants spells, in order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FC066D
    // Broiler-Human:        PENDING
    private static string[] Spelled(Sections state, uint[] constants) =>
        constants.Length == 0
            ? []
            : System.Array.ConvertAll(constants, constant => state.Names![constant]);

    /// <summary>
    /// Whether a site's chain reaches its unit's own root in exactly its declared depth, through
    /// rows that a code unit pushes itself.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=0AFF44
    // Broiler-Falsified-If: it answers true for a chain whose row at the declared depth is not the root kind the unit's flags call for
    // Broiler-Human:        PENDING
    private static bool ReachesRoot(
        Sections state, JsEvalShape[] shapes, JsEvalSiteRow site, JsCodeUnit unit)
    {
        var row = (int)site.Scope;

        // A MODULE BODY'S SITE ENDS AT ITS MODULE'S ROW (JSeal V15-module), and a script body's at
        // a program row: the two roots end in the global scope through different records.
        var expected = (unit.Flags & JsFormat.FunctionFlags.EvalCode) != 0
            ? JsFormat.EvalScopeKind.Eval
            : (unit.Flags & JsFormat.FunctionFlags.ProgramBody) != 0
                ? IsModuleBody(state, site.FunctionIndex)
                    ? JsFormat.EvalScopeKind.Module
                    : JsFormat.EvalScopeKind.Program
                : JsFormat.EvalScopeKind.Function;

        for (var step = 0u; step < site.Depth; step++)
        {
            // A FUNCTION BODY'S OWN VARIABLE ENVIRONMENT is a record the unit pushes, and only ever
            // the outermost one, directly inside a function's own record (JSeal V15-finish).
            var body = shapes[row].Kind == JsFormat.EvalScopeKind.FunctionBody &&
                step == site.Depth - 1 && expected == JsFormat.EvalScopeKind.Function;

            if (!body && shapes[row].Kind is not (JsFormat.EvalScopeKind.Block or
                JsFormat.EvalScopeKind.Catch or JsFormat.EvalScopeKind.With))
            {
                return false;
            }

            row = shapes[row].Parent;
        }

        return shapes[row].Kind == expected;
    }

    /// <summary>
    /// Holds the emitted-code sections, the native surface and the function table to each other.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE SYMBOL TABLE MUST NAME EVERY CODE UNIT, AND THAT CLAUSE IS THE WHOLE OF "ONE FORM PER
    /// HANDLE" WRITTEN AS A STRUCTURAL CHECK.</b> A symbol table shorter than the function table
    /// would be an artifact in which some units have an emitted form and the rest do not - which is
    /// a per-unit choice of executor, made once at compile time rather than once per call but a
    /// per-unit choice all the same, and it is exactly what this profile's non-goal refuses. The
    /// numeric manifest exists so that a program admitted by it is compilable IN WHOLE; an artifact
    /// that emitted part of itself is an artifact whose producer did not believe that, and there is
    /// no honest thing for a verifier to do with it.
    /// </para>
    /// <para>
    /// <b>Three declarations and each without the others is an artifact contradicting itself.</b>
    /// The surface says the composition was asked whether it admits executable memory; the code
    /// section says the bytes are here; the symbol section says which unit each run belongs to. A
    /// surface with no bytes declares a capability nothing uses, bytes with no surface skip the
    /// question, and bytes with no symbols are a blob nothing can enter.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=4A67A6
    // Broiler-Falsified-If: an artifact whose symbol table names fewer units than the function table is admitted, or a symbol offset outside the emitted blob is
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome LinkNative(
        Sections state, JsCodeUnit[] units, IJsNativeEmitter? emitter)
    {
        var declared = state.DeclaresNative();
        var carries = state.NativeCode is not null || state.NativeSymbols is not null;

        if (carries && !declared)
        {
            return Invalid(
                VmReason.UnknownFeature,
                JavaScriptDiagnosticCode.NativeSectionOutsideManifest,
                0);
        }

        if (!declared)
        {
            return Ok;
        }

        if (state.NativeCode is not { Length: > 0 } blob || state.NativeSymbols is not { } symbols)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedNativeSection,
                0);
        }

        if (symbols.Length != units.Length)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedNativeSection,
                0);
        }

        for (var index = 0; index < symbols.Length; index++)
        {
            var row = symbols[index];

            // THE OFFSETS TILE THE BLOB IN THE FUNCTION TABLE'S OWN ORDER, which is the same shape
            // the bytecode ranges are already held to: a run carries no length of its own, so the
            // only thing that makes its extent a fact rather than an inference is that the next run
            // starts where it ends and the last ends at the blob. The first therefore starts at
            // zero, each is strictly past the one before it, and every one of them is inside.
            var expectedStart = index == 0 ? 0u : symbols[index - 1].Offset + 1;

            if (row.FunctionIndex != (uint)index ||
                row.Offset < expectedStart ||
                row.Offset >= (uint)blob.Length ||
                (row.Offset % state.NativeCodeAlignment) != 0)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedNativeSection,
                    (ulong)index);
            }
        }

        // THE TEMPLATE-CLOSURE SCAN RUNS HERE AND IT RUNS ALWAYS, which is a decision worth stating
        // where it is made. Re-emission equality below is strictly stronger where an emitter is in
        // the image - it reaches the GENERATOR and this reaches only the PAYLOAD - so the obvious
        // arrangement is to scan only where there is no emitter to re-emit with. That arrangement
        // would put the layer an execution-only composition depends on in the one configuration
        // nothing in this repository exercises: every lane here that compiles a native artifact
        // carries a backend, so the first real run of a scan skipped in that case would be in the
        // composition that has no other check. It runs first for the same reason the version check
        // comes before the byte comparison: a payload that is not code at all should be refused as
        // that rather than as a difference from what this image would have emitted.
        //
        // THE TABLE IS CHOSEN BY THE MANIFEST THE ARTIFACT DECLARES AND BY NOTHING THE PAYLOAD
        // SAYS. A numeric artifact carries computing templates and a wide one carries the baseline
        // form, and the manifest has already been read and held to the descriptor; a field of the
        // native section naming the form would be a second statement of that fact for a forged
        // payload to contradict.
        //
        // FOR THE x86-64 BASELINE FORM THE SCAN ALSO HOLDS EVERY UNIT BODY TO THE PARTITION'S LAYOUT,
        // so an image with no emitter refuses a baseline payload whose instructions are not the ones
        // this build would emit, and what it still trusts is the encoder's agreement with the table -
        // which the lane's closure and golden rows hold. Those clauses read the program, so the image
        // is built once, here, and the scan and re-emission read one projection of the artifact. They
        // hold instructions and not bytes: the padding's length is the padding clause's, and the
        // declared alignment is compared with this build's by re-emission alone. A wide artifact
        // declaring arm64 is judged against the arm64 numeric table and meets none of them; no host of
        // this build arms it.
        // THE VALUE FORM IS THE WIDE MANIFEST'S ALONE, AND x86-64'S ALONE (JSD-0035 section 1). A form
        // byte naming it beside the numeric manifest or an arm64 architecture declares a form no
        // backend of any build writes, so the header is refused as malformed before any table is
        // chosen for it.
        if (state.NativeValueForm &&
            (state.ManifestId == JsNumericManifest.ManifestId ||
                state.NativeArchitecture is not (JsNativeArchitecture.X64SystemV or JsNativeArchitecture.X64Windows)))
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedNativeSection,
                0);
        }

        var tier = state.ManifestId == JsNumericManifest.ManifestId
            ? JsNativeTier.Numeric
            : state.NativeValueForm ? JsNativeTier.Value : JsNativeTier.Baseline;

        var image = NativeImage(state, tier);

        var scan = JsNativeScan.Scan(
            state.NativeArchitecture, tier, blob, symbols, state.NativeCodeAlignment, image);

        if (!scan.Accepted)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.NativePayloadNotTemplateClosed,
                scan.Offset);
        }

        return ReEmit(state, blob, symbols, emitter, image);
    }

    /// <summary>
    /// The program an artifact's native payload was emitted from, projected from what this verifier read:
    /// the code, the function rows, the constant pool, and for the baseline form the exception regions.
    /// </summary>
    /// <remarks>
    /// <b>THE BASELINE FORM READS THE REGIONS AND THE NUMERIC FORM READS NONE</b>, so the image carries them
    /// only for the tier that needs them - projected from the regions this verifier read, in the order the
    /// artifact carries them, which is the order the lowering handed its backend. The scan's layout clauses
    /// and re-emission both read this one image.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=704E67
    // Broiler-Falsified-If: the image differs from the artifact's own code, function rows, constant pool, or - for the baseline tier - exception regions in their order, or its tier differs from the one the manifest selects
    // Broiler-Human:        PENDING
    private static JsNativeProgramImage NativeImage(Sections state, JsNativeTier tier)
    {
        var pool = state.Constants!;
        var values = new double[pool.Length];
        var numbers = new bool[pool.Length];

        for (var index = 0; index < pool.Length; index++)
        {
            numbers[index] = pool[index].IsNumber;
            values[index] = pool[index].IsNumber ? pool[index].AsNumber() : 0;
        }

        var image = new JsNativeProgramImage(
            state.Code!, state.FunctionRows!, values, numbers, state.DeclaredOperandStack);

        if (tier == JsNativeTier.Numeric)
        {
            return image;
        }

        var regions = new JsExceptionRegionRow[state.Regions.Length];

        for (var index = 0; index < regions.Length; index++)
        {
            var region = state.Regions[index];

            regions[index] = new JsExceptionRegionRow(
                region.Unit,
                region.TryStart,
                region.TryEnd,
                region.Handler,
                region.ScopeDepth,
                region.StackHeight,
                region.Kind);
        }

        return image with { Tier = tier, Regions = regions };
    }

    /// <summary>
    /// Recompiles the artifact's own bytecode and requires the emitted bytes to match, where this
    /// image has an emitter at all.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THIS IS THE ONE CHECK THAT REACHES THE GENERATOR, AND EVERY OTHER CHECK IN THIS FILE REACHES
    /// ONLY THE PAYLOAD.</b> Outside the x86-64 baseline form, a wrong backend produces a well-framed
    /// sequence of the WRONG instructions: the length agrees, the alignment agrees, every symbol is
    /// inside the blob, every instruction is one the template table admits, and the artifact answers a
    /// different number from the one the language says. No structural check ever written catches that,
    /// and this profile's verifier is not going to become a second disassembler with a second opinion.
    /// What it can do is run the SAME deterministic emitter over the SAME bytecode the artifact carries
    /// and require the same bytes, which reduces trusting the payload to trusting this image's own
    /// backend.
    /// </para>
    /// <para>
    /// <b>WHERE THERE IS NO EMITTER IN THE IMAGE, WHAT GOES UNCHECKED IS SAID HERE RATHER THAN LEFT TO
    /// BE DISCOVERED.</b> An execution-only image carries a verifier and an interpreter and no code
    /// generator by construction - that absence is what makes it the composition it is - so it cannot
    /// re-emit anything. Such an image admits a native payload on its framing and on the
    /// template-closure scan that runs before this in every image, which holds every instruction to the
    /// table and, for the x86-64 baseline form, every unit body to the layout of the artifact's own
    /// partition, instruction for instruction. What that leaves is the generator: outside the baseline
    /// form, which instructions a unit holds, and within it the padding's length and the declared
    /// alignment. For those it is trusting PROVENANCE: that whoever produced the artifact ran a backend
    /// it has no way to re-run. That is a weaker thing than verification, it is the honest description
    /// of what is happening, and a composition that runs emitted code in that position owes its users
    /// the sentence rather than a footnote.
    /// </para>
    /// <para>
    /// <b>The emitter's own version must be the one the artifact names, and the version check comes
    /// first.</b> Comparing bytes emitted by version two against bytes written by version one
    /// answers "not equal" for a payload that was correct when it was written, which is a refusal
    /// with the wrong reason attached; refusing on the version says the true thing.
    /// </para>
    /// <para>
    /// <b>The image it emits from is the one the scan read</b>, built once by the caller from what this
    /// verifier read.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=5982A5
    // Broiler-Falsified-If: an artifact whose emitted bytes differ from this image's own emission of its bytecode is admitted while an emitter is present
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome ReEmit(
        Sections state,
        byte[] blob,
        JsNativeSymbolRow[] symbols,
        IJsNativeEmitter? emitter,
        JsNativeProgramImage image)
    {
        if (emitter is null)
        {
            return Ok;
        }

        if (emitter.Architecture != state.NativeArchitecture ||
            emitter.SemanticVersion != state.NativeBackendVersion ||
            emitter.CodeAlignment != state.NativeCodeAlignment)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedNativeSection,
                0);
        }

        if (!emitter.TryEmit(image, out var emitted, out var table, out _) ||
            !System.MemoryExtensions.SequenceEqual(
                System.MemoryExtensions.AsSpan(emitted),
                System.MemoryExtensions.AsSpan(blob)) ||
            table.Length != symbols.Length)
        {
            return Invalid(
                VmReason.InconsistentStructure,
                JavaScriptDiagnosticCode.MalformedNativeSection,
                0);
        }

        for (var index = 0; index < table.Length; index++)
        {
            if (table[index] != symbols[index])
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedNativeSection,
                    (ulong)index);
            }
        }

        return Ok;
    }

    /// <summary>
    /// Turns declared module rows into a linked graph, executing nothing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The whole of module linking happens here, before the artifact is admitted.</b> Which
    /// module a request names, which slot of which module an imported name reads, whether two star
    /// re-exports supply the same name from different bindings, and whether an export resolution
    /// walks a cycle - all four are decidable from the rows alone, and answering them at the first
    /// import would mean an artifact that verified and then failed on its second instruction.
    /// </para>
    /// <para>
    /// <b>The resolution is a fixed point rather than a recursion, and the reason is the stack.</b>
    /// A recursive <c>ResolveExport</c> is the specification's shape and would nest once per link in
    /// a re-export chain, which an artifact controls; verification runs on the caller's thread, and
    /// this component's own rule is that a payload never chooses how deep the host recurses. So
    /// each module's export table is filled by repeated passes that stop when a pass changes
    /// nothing, and what is still unresolved afterwards is classified by a walk with a visited set.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=8255D8
    // Broiler-Falsified-If: linking recurses to a depth the payload chooses, or a cyclic export resolution is answered by spending an allowance
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome LinkModules(
        Sections state,
        JsCodeUnit[] units,
        IVmVerificationContext context,
        JavaScriptReadAdapter adapter,
        out JsModuleRecord[] modules,
        out JsBinding[] bindings)
    {
        modules = [];
        bindings = [];

        // THE RESOLVER IS CHECKED FIRST AND ON ITS OWN. Declining the surface is already answered
        // by the time this runs - an artifact declaring a surface the composition did not admit was
        // refused where the surfaces were read - so what is left to ask is the surface's one
        // question: a composition that admits modules and registers no resolver has said it will
        // run a graph and supplied no way to say what a specifier names. Every refusal below would
        // otherwise be a remark about an artifact it was never going to evaluate.
        if (!context.TryGetCapabilityDescriptor(
                JavaScriptProfile.ResolveCapability.CapabilityId,
                JavaScriptProfile.ResolveCapability.Version,
                out _))
        {
            return Invalid(
                VmReason.UnsupportedFeatureManifest,
                JavaScriptDiagnosticCode.ModuleResolverAbsent,
                0);
        }

        if (state.ModuleRows is not { Length: > 0 } rows)
        {
            return Invalid(
                VmReason.InconsistentStructure, JavaScriptDiagnosticCode.ModuleSectionMissing, 0);
        }

        var names = state.Names!;
        var keys = new string[rows.Length];

        for (var index = 0; index < rows.Length; index++)
        {
            var row = rows[index];

            if (row.KeyConstant >= names.Length || names[row.KeyConstant].Length == 0 ||
                row.UnitIndex >= units.Length || row.InitialiserUnitIndex >= units.Length)
            {
                return Invalid(
                    VmReason.InconsistentStructure,
                    JavaScriptDiagnosticCode.MalformedModuleRow,
                    (ulong)index);
            }

            keys[index] = names[row.KeyConstant];

            for (var earlier = 0; earlier < index; earlier++)
            {
                if (string.Equals(keys[earlier], keys[index], System.StringComparison.Ordinal))
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        (ulong)index);
                }
            }
        }

        var requests = new int[rows.Length][];

        for (var index = 0; index < rows.Length; index++)
        {
            var row = rows[index];
            requests[index] = new int[row.RequestKeyConstants.Length];

            for (var request = 0; request < row.RequestKeyConstants.Length; request++)
            {
                var constant = row.RequestKeyConstants[request];

                if (constant >= names.Length ||
                    row.RequestSpecifierConstants[request] >= names.Length ||
                    names[row.RequestSpecifierConstants[request]].Length == 0)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        (ulong)index);
                }

                var found = -1;

                for (var candidate = 0; candidate < keys.Length; candidate++)
                {
                    if (string.Equals(keys[candidate], names[constant], System.StringComparison.Ordinal))
                    {
                        found = candidate;
                        break;
                    }
                }

                if (found < 0)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.ModuleRequestUnresolved,
                        (ulong)index);
                }

                requests[index][request] = found;
            }
        }

        var tables = new System.Collections.Generic.Dictionary<string, ExportEntry>[rows.Length];
        var seeded = Seed(rows, requests, units, names, tables);

        if (seeded.Category != VmOutcome.Normal)
        {
            return seeded;
        }

        Settle(rows, requests, tables, adapter);

        var classified = Classify(rows, requests, tables);

        if (classified.Category != VmOutcome.Normal)
        {
            return classified;
        }

        // A RE-EXPORT IS CHECKED WHETHER OR NOT ANYTHING IMPORTS IT. `export { x } from './m'`
        // where `m` exports no `x` is a link failure of THIS module, and a graph in which nothing
        // happened to import that name would otherwise have verified with a re-export naming
        // nothing - which is the case a whole family of the conformance suite is about.
        for (var index = 0; index < rows.Length; index++)
        {
            foreach (var indirect in rows[index].IndirectExports)
            {
                var published = names[indirect.NameConstant];

                if (!tables[index].TryGetValue(published, out var entry))
                {
                    continue;
                }

                if (entry.State == ExportState.NotFound)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.ModuleExportNotFound,
                        (ulong)index);
                }

                if (entry.State == ExportState.Ambiguous)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.ModuleExportAmbiguous,
                        (ulong)index);
                }
            }
        }

        var records = new JsModuleRecord[rows.Length];
        var table = new System.Collections.Generic.List<JsBinding>(state.ImportCount);

        for (var index = 0; index < rows.Length; index++)
        {
            var row = rows[index];
            var exported = new System.Collections.Generic.List<string>(tables[index].Count);

            foreach (var pair in tables[index])
            {
                if (pair.Value.State == ExportState.Resolved)
                {
                    exported.Add(pair.Key);
                }
            }

            exported.Sort(System.StringComparer.Ordinal);
            var exportBindings = new JsBinding[exported.Count];

            for (var name = 0; name < exported.Count; name++)
            {
                exportBindings[name] = tables[index][exported[name]].Binding;
            }

            var specifiers = new string[row.RequestSpecifierConstants.Length];

            for (var request = 0; request < specifiers.Length; request++)
            {
                specifiers[request] = names[row.RequestSpecifierConstants[request]];
            }

            records[index] = new JsModuleRecord(
                keys[index],
                row.UnitIndex,
                row.InitialiserUnitIndex,
                specifiers,
                requests[index],
                exported.ToArray(),
                exportBindings);
        }

        for (var index = 0; index < rows.Length; index++)
        {
            foreach (var entry in rows[index].Imports)
            {
                if (entry.RequestIndex >= requests[index].Length)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        (ulong)index);
                }

                var target = requests[index][entry.RequestIndex];

                if (entry.Kind == JsFormat.ImportKind.Namespace)
                {
                    if (entry.NameConstant != 0)
                    {
                        return Invalid(
                            VmReason.InconsistentStructure,
                            JavaScriptDiagnosticCode.MalformedModuleRow,
                            (ulong)index);
                    }

                    table.Add(new JsBinding(target, 0, JsBindingKind.Namespace, keys[target]));
                    continue;
                }

                if (entry.NameConstant >= names.Length)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        (ulong)index);
                }

                var wanted = names[entry.NameConstant];

                if (!tables[target].TryGetValue(wanted, out var found) ||
                    found.State == ExportState.NotFound)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.ModuleExportNotFound,
                        (ulong)index);
                }

                if (found.State == ExportState.Ambiguous)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.ModuleExportAmbiguous,
                        (ulong)index);
                }

                table.Add(found.Binding);
            }
        }

        if (table.Count != state.ImportCount)
        {
            return Invalid(
                VmReason.InconsistentStructure, JavaScriptDiagnosticCode.MalformedModuleRow, 0);
        }

        modules = records;
        bindings = table.ToArray();
        return Ok;
    }

    /// <summary>Fills each module's table with what its own rows state, before any propagation.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DEAAE8
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome Seed(
        JsModuleRow[] rows,
        int[][] requests,
        JsCodeUnit[] units,
        string[] names,
        System.Collections.Generic.Dictionary<string, ExportEntry>[] tables)
    {
        for (var index = 0; index < rows.Length; index++)
        {
            var row = rows[index];
            var table = new System.Collections.Generic.Dictionary<string, ExportEntry>(
                System.StringComparer.Ordinal);

            tables[index] = table;

            foreach (var local in row.LocalExports)
            {
                if (local.NameConstant >= names.Length ||
                    local.Slot >= units[row.UnitIndex].ScopeSlots)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        (ulong)index);
                }

                var name = names[local.NameConstant];

                if (!table.TryAdd(
                        name,
                        new ExportEntry
                        {
                            State = ExportState.Resolved,
                            Binding = new JsBinding(index, (int)local.Slot, JsBindingKind.Slot, name),
                        }))
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        (ulong)index);
                }
            }

            foreach (var indirect in row.IndirectExports)
            {
                if (indirect.NameConstant >= names.Length ||
                    indirect.RequestIndex >= requests[index].Length)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        (ulong)index);
                }

                var target = requests[index][indirect.RequestIndex];
                var name = names[indirect.NameConstant];

                var entry = indirect.Kind == JsFormat.ImportKind.Namespace
                    ? new ExportEntry
                    {
                        State = ExportState.Resolved,
                        Binding = new JsBinding(
                            target, 0, JsBindingKind.Namespace, name),
                    }
                    : new ExportEntry
                    {
                        State = ExportState.Pending,
                        TargetModule = target,
                        TargetName = indirect.ImportNameConstant < names.Length
                            ? names[indirect.ImportNameConstant]
                            : string.Empty,
                    };

                if (indirect.Kind == JsFormat.ImportKind.Namespace
                    ? indirect.ImportNameConstant != 0
                    : entry.TargetName.Length == 0)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        (ulong)index);
                }

                if (!table.TryAdd(name, entry))
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        (ulong)index);
                }
            }

            foreach (var star in row.StarExportRequests)
            {
                if (star >= requests[index].Length)
                {
                    return Invalid(
                        VmReason.InconsistentStructure,
                        JavaScriptDiagnosticCode.MalformedModuleRow,
                        (ulong)index);
                }
            }
        }

        return Ok;
    }

    /// <summary>
    /// Propagates re-exports until a pass changes nothing, or until every module has had a turn.
    /// </summary>
    /// <remarks>
    /// The pass count is bounded by the module count because a chain of re-exports can be no longer
    /// than the graph; a cycle changes nothing after its first pass and stops the loop early, which
    /// is why a cyclic artifact costs a pass rather than an allowance.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=290727
    // Broiler-Human:        PENDING
    private static void Settle(
        JsModuleRow[] rows,
        int[][] requests,
        System.Collections.Generic.Dictionary<string, ExportEntry>[] tables,
        JavaScriptReadAdapter adapter)
    {
        for (var pass = 0; pass <= rows.Length; pass++)
        {
            var changed = false;

            for (var index = 0; index < rows.Length; index++)
            {
                if (!adapter.TryChargeWork((ulong)tables[index].Count + 1))
                {
                    return;
                }

                foreach (var name in Keys(tables[index]))
                {
                    var entry = tables[index][name];

                    if (entry.State != ExportState.Pending ||
                        !tables[entry.TargetModule].TryGetValue(entry.TargetName, out var found) ||
                        found.State == ExportState.Pending)
                    {
                        continue;
                    }

                    tables[index][name] = new ExportEntry
                    {
                        State = found.State,
                        Binding = found.Binding,
                        TargetModule = entry.TargetModule,
                        TargetName = entry.TargetName,
                    };

                    changed = true;
                }

                foreach (var star in rows[index].StarExportRequests)
                {
                    var target = requests[index][star];

                    foreach (var name in Keys(tables[target]))
                    {
                        // `default` IS NOT RE-EXPORTED BY A STAR, which is the one asymmetry in the
                        // form: `export * from './m'` republishes what `m` names and not what `m`
                        // is, so a default export stays reachable only through `m` itself.
                        if (string.Equals(name, "default", System.StringComparison.Ordinal) ||
                            tables[target][name].State != ExportState.Resolved)
                        {
                            continue;
                        }

                        var supplied = tables[target][name].Binding;

                        if (!tables[index].TryGetValue(name, out var existing))
                        {
                            tables[index][name] = new ExportEntry
                            {
                                State = ExportState.Resolved,
                                Binding = supplied,
                                FromStar = true,
                            };

                            changed = true;
                            continue;
                        }

                        // TWO STARS SUPPLYING ONE NAME IS AMBIGUOUS AND TWO SUPPLYING ONE BINDING IS
                        // NOT. A diamond in which both paths reach the same slot of the same module
                        // names one binding, and the language admits it; two different slots name
                        // two, and no read of that name could pick one.
                        if (existing.FromStar &&
                            existing.State == ExportState.Resolved &&
                            (existing.Binding.Module != supplied.Module ||
                                existing.Binding.Slot != supplied.Slot ||
                                existing.Binding.Kind != supplied.Kind))
                        {
                            tables[index][name] = new ExportEntry
                            {
                                State = ExportState.Ambiguous,
                                FromStar = true,
                            };

                            changed = true;
                        }
                    }
                }
            }

            if (!changed)
            {
                return;
            }
        }
    }

    /// <summary>
    /// Decides what every still-unresolved re-export is: a cycle, or a name nothing exports.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0D2F3A
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome Classify(
        JsModuleRow[] rows,
        int[][] requests,
        System.Collections.Generic.Dictionary<string, ExportEntry>[] tables)
    {
        _ = requests;

        for (var index = 0; index < rows.Length; index++)
        {
            foreach (var name in Keys(tables[index]))
            {
                if (tables[index][name].State != ExportState.Pending)
                {
                    continue;
                }

                var seen = new System.Collections.Generic.HashSet<(int Module, string Name)>();
                var module = index;
                var wanted = name;

                while (true)
                {
                    if (!seen.Add((module, wanted)))
                    {
                        return Invalid(
                            VmReason.InconsistentStructure,
                            JavaScriptDiagnosticCode.ModuleExportCircular,
                            (ulong)index);
                    }

                    if (!tables[module].TryGetValue(wanted, out var entry))
                    {
                        tables[index][name] = new ExportEntry { State = ExportState.NotFound };
                        break;
                    }

                    if (entry.State != ExportState.Pending)
                    {
                        tables[index][name] = entry;
                        break;
                    }

                    module = entry.TargetModule;
                    wanted = entry.TargetName;
                }
            }
        }

        return Ok;
    }

    /// <summary>A snapshot of a table's keys, so a pass may write the table while walking it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=01F1E0
    // Broiler-Human:        PENDING
    private static string[] Keys(
        System.Collections.Generic.Dictionary<string, ExportEntry> table)
    {
        var keys = new string[table.Count];
        table.Keys.CopyTo(keys, 0);
        return keys;
    }

    /// <summary>How far one exported name has got towards naming a binding.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=C47894
    // Broiler-Human:        PENDING
    private enum ExportState
    {
        /// <summary>It re-exports a name whose own resolution is not settled yet.</summary>
        Pending = 0,

        /// <summary>It names one binding.</summary>
        Resolved = 1,

        /// <summary>Two star re-exports supply it from different bindings.</summary>
        Ambiguous = 2,

        /// <summary>Nothing in the graph exports it.</summary>
        NotFound = 3,
    }

    /// <summary>One row of a module's export table while it is being settled.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E2D5BB
    // Broiler-Human:        PENDING
    private readonly struct ExportEntry
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=27F594
        // Broiler-Human:        PENDING
        internal ExportState State { get; init; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F1CBBA
        // Broiler-Human:        PENDING
        internal JsBinding Binding { get; init; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6ED800
        // Broiler-Human:        PENDING
        internal int TargetModule { get; init; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=C22B8A
        // Broiler-Human:        PENDING
        internal string TargetName { get; init; }

        /// <summary>Whether a star re-export supplied it, which is what makes ambiguity possible.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9ECA7B
        // Broiler-Human:        PENDING
        internal bool FromStar { get; init; }
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

        /// <summary>The feature manifest the payload's own header names.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=86259C
        // Broiler-Human:        PENDING
        internal string ManifestId { get; set; } = JsFormat.ManifestId;

        /// <summary>The instruction set and convention the emitted code section named.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6E316D
        // Broiler-Human:        PENDING
        internal JsNativeArchitecture NativeArchitecture { get; set; }

        /// <summary>The backend version the emitted code section named.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=68AC35
        // Broiler-Human:        PENDING
        internal uint NativeBackendVersion { get; set; }

        /// <summary>The alignment the emitted code section named.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F49964
        // Broiler-Human:        PENDING
        internal uint NativeCodeAlignment { get; set; }

        /// <summary>Whether the emitted code section's form byte named the value form.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=991AC8
        // Broiler-Falsified-If: this is true for a section whose form byte was zero, or false for one whose form byte named the value form
        // Broiler-Human:        PENDING
        internal bool NativeValueForm { get; set; }

        /// <summary>The emitted machine code, or null when the artifact carries none.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4D540B
        // Broiler-Human:        PENDING
        internal byte[]? NativeCode { get; set; }

        /// <summary>The emitted-code symbols, or null when the artifact carries none.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=494397
        // Broiler-Human:        PENDING
        internal JsNativeSymbolRow[]? NativeSymbols { get; set; }

        /// <summary>The module rows as the payload declares them, before any resolution.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=67DA18
        // Broiler-Human:        PENDING
        internal JsModuleRow[]? ModuleRows { get; set; }

        /// <summary>How many import entries the module rows declare between them.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A683AF
        // Broiler-Human:        PENDING
        internal int ImportCount { get; set; }

        /// <summary>Whether the artifact declared the module surface beside its manifest.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E754A2
        // Broiler-Human:        PENDING
        internal bool DeclaresModules()
        {
            foreach (var surface in Surfaces)
            {
                if (string.Equals(surface, JsSurfaces.Modules, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Where the first BigInt constant's payload begins, or null when the pool holds none.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F363E6
        // Broiler-Human:        PENDING
        internal ulong? BigIntConstantAt { get; set; }

        /// <summary>Whether the artifact declared the BigInt surface beside its manifest.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=357946
        // Broiler-Human:        PENDING
        internal bool DeclaresBigInt()
        {
            foreach (var surface in Surfaces)
            {
                if (string.Equals(surface, JsSurfaces.BigInt, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Whether the artifact declared the native surface beside its manifest.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0F78ED
        // Broiler-Human:        PENDING
        internal bool DeclaresNative()
        {
            foreach (var surface in Surfaces)
            {
                if (string.Equals(surface, JsSurfaces.Native, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Whether the artifact declared the dynamic surface beside its manifest.</summary>
        /// <remarks>
        /// A program declares this by reading one of the names the surface owns, and — since a
        /// dynamic <c>import()</c> may reach the mediator — by containing one of those.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=8EEDB8
        // Broiler-Human:        PENDING
        internal bool DeclaresDynamic()
        {
            foreach (var surface in Surfaces)
            {
                if (string.Equals(surface, JsSurfaces.Dynamic, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>The eval scope map's shapes as the payload declares them, or null without the section.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A77652
        // Broiler-Human:        PENDING
        internal JsEvalScopeRow[]? EvalScopeRows { get; set; }

        /// <summary>The eval scope map's sites as the payload declares them.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5AA912
        // Broiler-Human:        PENDING
        internal JsEvalSiteRow[]? EvalSiteRows { get; set; }

        /// <summary>The eval scope map's declaration rows as the payload declares them.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=C379BF
        // Broiler-Human:        PENDING
        internal JsEvalDeclarationRow[]? EvalDeclarationRows { get; set; }

        /// <summary>Each admitted site's declared depth, by code offset, for the abstract pass to hold.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4C367F
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.Dictionary<uint, uint> EvalSiteDepths { get; } = [];

        /// <summary>The script-declarations rows as the payload declares them, or null without the section.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A662B1
        // Broiler-Human:        PENDING
        internal JsScriptDeclarationRow[]? ScriptDeclarationRows { get; set; }

        /// <summary>The script-referrers rows as the payload declares them, or null without the section.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=06A473
        // Broiler-Human:        PENDING
        internal (uint FunctionIndex, uint ReferrerConstant)[]? ScriptReferrerRows { get; set; }

        /// <summary>Whether any code unit is flagged as eval code.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5C5485
        // Broiler-Human:        PENDING
        internal bool HasEvalCode { get; set; }

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

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FB800A
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

                    // A SITE'S DECLARED DEPTH IS THE DEPTH THIS PASS COMPUTES THERE, which is what ties
                    // the map's chain to the records the frame actually holds at the call: the rows
                    // inside the unit are the records the unit pushed, one each.
                    if (opcode is JsOpcode.CallEval or JsOpcode.CallEvalSpread &&
                        state.EvalSiteDepths.TryGetValue((uint)offset, out var declaredDepth) &&
                        declaredDepth != (uint)depth)
                    {
                        return Invalid(
                            VmReason.InconsistentStructure,
                            JavaScriptDiagnosticCode.MalformedEvalScopes,
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
                                JsOpcode.IterateCloseAsync or JsOpcode.DisposeStep => height - 1,
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

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=73F7C0
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
                case JsOpcode.ThrowImmutable:
                case JsOpcode.DeclareGlobalLet:
                case JsOpcode.DeclareGlobalConst:
                case JsOpcode.InitialiseGlobalLexical:
                case JsOpcode.DeleteGlobalBinding:
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

                // AN IMPORT READ IN AN ARTIFACT WITH NO IMPORTS IS REFUSED BY THE SAME CHECK.
                // The table is empty there, so every operand is out of range and the instruction
                // is unreachable without a further rule about which units may contain it.
                case JsOpcode.LoadImport:
                    return operand < state.ImportCount
                        ? Ok
                        : Invalid(
                            VmReason.InconsistentStructure,
                            JavaScriptDiagnosticCode.MalformedModuleRow,
                            (ulong)offset);

                // A DYNAMIC IMPORT NAMES THE DYNAMIC SURFACE, AND ONLY THAT ONE. Its specifier is a
                // value, so whether it names a module this artifact carries is not decidable here;
                // a call that finds one is answered from the artifact and a call that does not puts
                // the specifier to the mediator, which is the door `eval` goes through. So the
                // instruction is checked against the surface that door belongs to. The MODULE
                // surface is declared by carrying records and a script that writes `import()`
                // carries none - checking this instruction against it would refuse a program the
                // composition admits. A composition that declined the dynamic surface never reaches
                // this code: its artifact was refused where the surfaces were read, with
                // SurfaceOutsideComposition. What is left for here is an artifact that reached the
                // instruction without declaring it, which is a program buying a host round trip
                // with an opcode instead of with a declaration.
                case JsOpcode.ImportCall:
                    if (!state.DeclaresDynamic())
                    {
                        return Invalid(
                            VmReason.UnknownFeature,
                            JavaScriptDiagnosticCode.ImportCallOutsideManifest,
                            (ulong)offset);
                    }

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

                // AND `import.meta` NAMES ONE, because it is a module's own metadata and a script
                // has none. The operand indexes the module records rather than the constants, so
                // an artifact with no records has no valid operand for it at all.
                case JsOpcode.ImportMeta:
                    if (!state.DeclaresModules())
                    {
                        return Invalid(
                            VmReason.UnknownFeature,
                            JavaScriptDiagnosticCode.ImportCallOutsideManifest,
                            (ulong)offset);
                    }

                    return state.ModuleRows is not null && operand < state.ModuleRows.Length
                        ? Ok
                        : Invalid(
                            VmReason.InconsistentStructure,
                            JavaScriptDiagnosticCode.MalformedModuleRow,
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

                // AND ONLY A GENERATOR BODY MAY BE ENTERED IN TWO PIECES. The seam is checked
                // against the same flag as `Yield` and carries the same diagnostic, because it is
                // the same fact about the executor: the frame a call leaves suspended at this
                // instruction is the heap frame the generator arm allocated, and a unit without the
                // bit is entered on the native stack by a call that has nowhere to leave one. In a
                // unit that HAS the bit and does not bind its own parameters the instruction is
                // reachable and harmless - the executor never runs the prologue phase there and
                // walks straight through it - so the flag it is refused outside of is the only
                // flag it is checked against.
                case JsOpcode.EnterBody:
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

                // A DISPOSAL STEP IS THE FIRST HALF OF AN AWAIT, and it is refused where the await
                // would be, with the await's own code: its fall-through leaves the value an `Await`
                // is about to suspend on, and a unit that may not await has no driver to resume
                // the frame that reached it (JSD-0034).
                case JsOpcode.DisposeStep:
                    return (unit.Flags & JsFormat.FunctionFlags.Async) != 0
                        ? Ok
                        : Invalid(
                            VmReason.SemanticValidationFailed,
                            JavaScriptDiagnosticCode.AwaitOutsideAsync,
                            (ulong)offset);

                // A HINT OR A MODE THIS VERSION DOES NOT DEFINE IS AN UNKNOWN FEATURE, answered as
                // an undefined `NewClass` bit is: the byte names an instruction this reader knows
                // and asks it for behaviour it does not have - and `DisposeEnd`'s stack effect is
                // decided by the operand, so an undefined one has an effect nothing agreed on.
                case JsOpcode.DisposeAdd:
                case JsOpcode.DisposeEnd:
                    return operand <= 1
                        ? Ok
                        : Invalid(
                            VmReason.UnknownFeature,
                            JavaScriptDiagnosticCode.UnknownOpcode,
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

                // AN EVAL NAME INSTRUCTION BELONGS TO AN EVALUATED PROGRAM, and it is refused in any
                // artifact that is not one. Its high half is a count of records to the boundary, which
                // one byte bounds as the scope ceiling does; what the bytes cannot show is that the
                // record it reaches IS a boundary, which the executor checks and answers as an internal
                // defect rather than as a lookup (JSeal V14).
                case JsOpcode.LoadEvalName:
                case JsOpcode.LoadEvalNameOrUndefined:
                case JsOpcode.StoreEvalName:
                case JsOpcode.LoadEvalNameWithBase:
                case JsOpcode.DeleteEvalName:
                case JsOpcode.StoreEvalVariable:
                    if (!state.DeclaresDynamic())
                    {
                        return Invalid(
                            VmReason.UnknownFeature,
                            JavaScriptDiagnosticCode.EvalScopesOutsideManifest,
                            (ulong)offset);
                    }

                    if (!state.HasEvalCode)
                    {
                        return Invalid(
                            VmReason.InconsistentStructure,
                            JavaScriptDiagnosticCode.MalformedEvalScopes,
                            (ulong)offset);
                    }

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
