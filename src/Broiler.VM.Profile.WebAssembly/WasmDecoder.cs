// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   39
// Annotated:        39/39
// Exempt:           24
// Human-reviewed:   0/39
// IP risk:          Low
// Security risk:    Critical
// Criteria:         24/24
// Resource impact:  8/10 max
// Unverified:       39
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>One run of locals in a function body: a repeat count and the type repeated.</summary>
/// <remarks>
/// It is an unmanaged pair so the run vector can be sized through the core's bounded allocator
/// before it is filled, which is the only sanctioned route from an untrusted count to a buffer.
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=4FE791
// Broiler-Human:        PENDING
internal struct WasmLocalRun
{
    /// <summary>How many locals of the type follow.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=13A9DA
    // Broiler-Human:        PENDING
    public uint Count;

    /// <summary>The type repeated.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4BCF45
    // Broiler-Human:        PENDING
    public WasmValueType Type;
}

/// <summary>
/// The section loop: the first of the two passes a payload is put through, and the only one
/// that reads its framing.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS DECODES AND DOES NOT VALIDATE.</b> It answers whether the bytes are a well-formed
/// encoding of a module - the preamble, the section framing, the canonical order, the vector
/// grammars, the variable-length integers, the names - and it answers nothing about whether the
/// module makes sense. No index is checked against the space it addresses, no instruction byte is
/// read inside a function body, no operand stack is simulated and no block is matched to its end.
/// Those belong to <see cref="WasmValidator"/>, which stands BESIDE this type rather than inside
/// it, and the separation is observable rather than stylistic: decoding completes before validation
/// begins at module granularity, so a module that is both malformed and invalid is reported
/// malformed. A decoder that read an operand type to decide how to frame a section would have
/// fused the two passes, and this one never reads a byte of a function body.
/// <i>(Corrected 2026-09-08. The summary above read "The section loop: the whole of what this
/// build does to a payload", which this paragraph has contradicted since the validator landed
/// beside it: a payload is put through two passes and this is the first. The superseded reading is
/// quoted rather than deleted, because the sentence a reader quotes is the summary.)</i>
/// </para>
/// <para>
/// <b>Every failure is one of two categories and the split is the core's ruling.</b> A malformation
/// is an invalid artifact carrying this profile's diagnostic code and a byte position. A breach of
/// an effective ceiling - artifact bytes, section count, declared count, structural depth, or the
/// allocation and work allowances - is a resource exhaustion naming one dimension and one scope,
/// and it carries no diagnostic code, because it is a statement about what this host declined to
/// spend rather than about the module. Conflating them tells a caller its module is malformed when
/// it is not, and because a retained corpus entry pins the triple it observed, an entry recorded
/// under the wrong category passes forever and records the wrong answer.
/// </para>
/// <para>
/// <b>It is a ref struct because the reader is one</b>, and the reader is one because it holds a
/// span. That is not an inconvenience to be worked around: it is what makes it impossible for a
/// decoded module to keep a window onto the caller's bytes, so everything retained here is copied
/// through the bounded allocator into an array this module owns.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=8; Fingerprint=399155
// Broiler-Falsified-If: a buffer is sized from a count that has not cleared its ceiling, or a ceiling breach is reported as a malformed artifact
// Broiler-Human:        PENDING
internal ref struct WasmDecoder
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DFC99B
    // Broiler-Human:        PENDING
    private VmBoundedReader reader;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=92AD83
    // Broiler-Human:        PENDING
    private readonly WasmReadAdapter adapter;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B8F815
    // Broiler-Human:        PENDING
    private readonly VmReadBounds bounds;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5677B3
    // Broiler-Human:        PENDING
    private VmVerifierOutcome refusal;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FC639E
    // Broiler-Human:        PENDING
    private int sectionOrdinal;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D3DEE5
    // Broiler-Human:        PENDING
    private int sectionIdentifier;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FB6F93
    // Broiler-Human:        PENDING
    private int itemOrdinal;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=880625
    // Broiler-Human:        PENDING
    private int highestOrderRank;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F03A52
    // Broiler-Human:        PENDING
    private uint seenSections;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C7ECFA
    // Broiler-Human:        PENDING
    private int customSections;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=59592C
    // Broiler-Human:        PENDING
    private WasmFuncType[] types;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A4DF28
    // Broiler-Human:        PENDING
    private uint[] functionTypeIndices;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=903064
    // Broiler-Human:        PENDING
    private WasmTableType[] tables;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=50E076
    // Broiler-Human:        PENDING
    private WasmMemoryType[] memories;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3003E7
    // Broiler-Human:        PENDING
    private WasmGlobal[] globals;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4F80CC
    // Broiler-Human:        PENDING
    private WasmExport[] exports;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BE6A39
    // Broiler-Human:        PENDING
    private WasmElementSegment[] elements;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B261DE
    // Broiler-Human:        PENDING
    private WasmDataSegment[] data;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B0876C
    // Broiler-Human:        PENDING
    private WasmFunctionBody[] bodies;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=86C5BC
    // Broiler-Human:        PENDING
    private long startFunctionIndex;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DB1815
    // Broiler-Human:        PENDING
    private bool declaresDataCount;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5E586D
    // Broiler-Human:        PENDING
    private uint declaredDataCount;

    /// <summary>
    /// Builds a decoder over one payload, under ceilings the caller has already materialized.
    /// </summary>
    /// <remarks>
    /// The bounds arrive as a parameter rather than being read here, because the ordering that
    /// matters - the ceilings are fixed before the first byte is examined - is a property of the
    /// verifier's sequence and is stated there.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=7A55F8
    // Broiler-Falsified-If: any field is left uninitialised so a failed decode hands back an array nothing filled
    // Broiler-Human:        PENDING
    internal WasmDecoder(
        System.ReadOnlySpan<byte> payload,
        in VmReadBounds readBounds,
        WasmReadAdapter meter,
        ulong pollGranularity)
    {
        adapter = meter;
        bounds = readBounds;
        reader = new VmBoundedReader(payload, in readBounds, meter, pollGranularity);
        refusal = default;
        sectionOrdinal = -1;
        sectionIdentifier = -1;
        itemOrdinal = -1;
        highestOrderRank = 0;
        seenSections = 0;
        customSections = 0;
        types = [];
        functionTypeIndices = [];
        tables = [];
        memories = [];
        globals = [];
        exports = [];
        elements = [];
        data = [];
        bodies = [];
        startFunctionIndex = -1;
        declaresDataCount = false;
        declaredDataCount = 0;
    }

    /// <summary>
    /// Reads the whole payload, answering either a module or the one refusal that stopped it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The loop is: the section identifier byte, the section length as this profile's own unsigned
    /// variable-length integer, the framing entry, the body, and the framing exit. The exit is not
    /// decoration - it is where a body that consumed less than its declared length is caught, which
    /// is as much a structural error as one that consumed more, because it means the artifact and
    /// this decoder disagree about where the next section starts.
    /// </para>
    /// <para>
    /// A custom section is entered, skipped and exited without a byte of its body being looked at,
    /// and it is counted so that a caller can see it was there. It takes no position in the order
    /// and it may repeat.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=6; Fingerprint=E26D08
    // Broiler-Falsified-If: a module is returned while any section body did not consume exactly its declared length, or a non-custom section repeated or ran out of canonical order
    // Broiler-Human:        PENDING
    internal bool TryDecode(out WasmModule? module, out VmVerifierOutcome outcome)
    {
        module = null;
        outcome = default;

        if (!TryReadPreamble())
        {
            outcome = refusal;
            return false;
        }

        while (reader.Remaining > 0)
        {
            if (!TryReadOneSection())
            {
                outcome = refusal;
                return false;
            }
        }

        sectionOrdinal = -1;
        sectionIdentifier = -1;
        itemOrdinal = -1;

        if (!TryCheckSectionAgreement())
        {
            outcome = refusal;
            return false;
        }

        module = new WasmModule(
            types,
            functionTypeIndices,
            tables,
            memories,
            globals,
            exports,
            elements,
            data,
            bodies,
            startFunctionIndex,
            declaresDataCount,
            customSections);

        return true;
    }

    /// <summary>Reads the magic and the binary format version.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=BED23F
    // Broiler-Falsified-If: a payload whose first eight bytes are not the magic and version 1 reaches the section loop
    // Broiler-Human:        PENDING
    private bool TryReadPreamble()
    {
        if (!reader.TryReadBytes(4, out var magic))
        {
            return Stop(FromReader(0));
        }

        if (!System.MemoryExtensions.SequenceEqual(magic, WasmFormat.Magic))
        {
            return Stop(Invalid(VmReason.MalformedEncoding, WebAssemblyDiagnosticCode.WrongMagic, 0));
        }

        // Four raw little-endian bytes rather than a variable-length integer: the version field is
        // fixed width in this format, and reading it as a varint would accept encodings the format
        // does not have.
        if (!reader.TryReadUInt32LittleEndian(out var version))
        {
            return Stop(FromReader(reader.Position));
        }

        if (version != WasmFormat.BinaryVersion)
        {
            return Stop(Invalid(
                VmReason.UnknownFormatVersion,
                WebAssemblyDiagnosticCode.UnsupportedBinaryVersion,
                reader.Position));
        }

        return true;
    }

    /// <summary>Reads one section: its identifier, its length, its framing and its body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=5; Fingerprint=5D296A
    // Broiler-Falsified-If: the order and duplicate rules are applied after the section body is decoded rather than before
    // Broiler-Human:        PENDING
    private bool TryReadOneSection()
    {
        var at = reader.Position;

        if (!reader.TryReadByte(out var identifier))
        {
            return Stop(FromReader(at));
        }

        if (!WasmFormat.IsDefinedSectionId(identifier))
        {
            return Stop(Invalid(
                VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.UnknownSectionId, at));
        }

        var id = (WasmSectionId)identifier;

        // THE ORDER AND DUPLICATE RULES ARE APPLIED HERE, BEFORE THE FRAMING IS ENTERED, so a
        // section that may not appear at all is refused before it has charged the section-count
        // ceiling or authorised a single allocation.
        if (id != WasmSectionId.Custom && !TryAdmitSectionPosition(id, at))
        {
            return false;
        }

        if (!WasmLeb128.TryReadVarU32(ref reader, out var declaredLength, out var status))
        {
            return Stop(FromVarInt(status, at));
        }

        if (!reader.TryEnterSection(declaredLength, out var frame))
        {
            return Stop(FromReader(reader.Position));
        }

        sectionOrdinal++;
        sectionIdentifier = identifier;
        itemOrdinal = -1;

        if (id == WasmSectionId.Custom)
        {
            customSections++;

            if (!reader.TrySkipSectionBody(in frame))
            {
                return Stop(FromReader(reader.Position));
            }
        }
        else if (!TryDecodeSectionBody(id))
        {
            return false;
        }

        if (!reader.TryExitSection(in frame))
        {
            // The reader answers a framing disagreement as a malformed encoding, which is true but
            // says nothing. This is the one place the profile knows what it means: the body and its
            // declared length do not agree.
            return Stop(reader.Status == VmBoundedReadStatus.MalformedEncoding
                ? Invalid(
                    VmReason.InconsistentStructure,
                    WebAssemblyDiagnosticCode.SectionLengthMismatch,
                    reader.Position)
                : FromReader(reader.Position));
        }

        sectionIdentifier = -1;
        itemOrdinal = -1;
        return true;
    }

    /// <summary>Holds one non-custom section to the canonical order and to appearing once.</summary>
    /// <remarks>
    /// The comparison is against the order table and never against the identifier values, because
    /// the two disagree in two places the format defines and in one more the format reserves.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=DA3A9F
    // Broiler-Falsified-If: it decides order by comparing section identifiers rather than order ranks
    // Broiler-Human:        PENDING
    private bool TryAdmitSectionPosition(WasmSectionId id, ulong at)
    {
        var bit = 1u << (int)id;

        if ((seenSections & bit) != 0)
        {
            return Stop(Invalid(
                VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.DuplicateSection, at));
        }

        var rank = WasmFormat.OrderRankOf(id);

        if (rank <= highestOrderRank)
        {
            return Stop(Invalid(
                VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.SectionOutOfOrder, at));
        }

        seenSections |= bit;
        highestOrderRank = rank;
        return true;
    }

    /// <summary>Dispatches one non-custom section to the grammar that reads it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5F3DBE
    // Broiler-Human:        PENDING
    private bool TryDecodeSectionBody(WasmSectionId id) => id switch
    {
        WasmSectionId.Type => TryDecodeTypeSection(),
        WasmSectionId.Import => TryDecodeImportSection(),
        WasmSectionId.Function => TryDecodeFunctionSection(),
        WasmSectionId.Table => TryDecodeTableSection(),
        WasmSectionId.Memory => TryDecodeMemorySection(),
        WasmSectionId.Global => TryDecodeGlobalSection(),
        WasmSectionId.Export => TryDecodeExportSection(),
        WasmSectionId.Start => TryDecodeStartSection(),
        WasmSectionId.Element => TryDecodeElementSection(),
        WasmSectionId.Code => TryDecodeCodeSection(),
        WasmSectionId.Data => TryDecodeDataSection(),
        WasmSectionId.DataCount => TryDecodeDataCountSection(),
        _ => Stop(Invalid(
            VmReason.UnknownFeature,
            WebAssemblyDiagnosticCode.TagSectionNotAdmitted,
            reader.Position)),
    };

    /// <summary>Reads the function types.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=3; Fingerprint=8D16A4
    // Broiler-Falsified-If: a parameter or result vector is allocated from a count that has not cleared the declared-count ceiling
    // Broiler-Human:        PENDING
    private bool TryDecodeTypeSection()
    {
        if (!TryReadCount(out var count) ||
            !TryAllocateReferences<WasmFuncType>(count, out var decoded))
        {
            return false;
        }

        for (var index = 0u; index < count; index++)
        {
            itemOrdinal = (int)index;

            if (!TryReadByte(out var tag))
            {
                return false;
            }

            if (tag != WasmTypeGrammar.FunctionTypeTag)
            {
                return Stop(Invalid(
                    VmReason.MalformedEncoding,
                    WebAssemblyDiagnosticCode.MalformedFunctionTypeTag,
                    reader.Position - 1));
            }

            if (!TryReadValueTypeVector(out var parameters) ||
                !TryReadValueTypeVector(out var results))
            {
                return false;
            }

            decoded[index] = new WasmFuncType(parameters, results);
        }

        types = decoded;
        return true;
    }

    /// <summary>
    /// Reads the import section, and refuses a module that declares an import.
    /// </summary>
    /// <remarks>
    /// <b>No feature manifest this profile accepts admits an import, and a manifest is refused
    /// rather than degraded.</b> So the count is read - the grammar is exercised and the framing is
    /// held to its declared length either way - and a non-empty one is an unadmitted feature rather
    /// than a malformation. An empty import section is a legal encoding of a module that imports
    /// nothing, and it is accepted.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=14DFDD
    // Broiler-Falsified-If: a module declaring an import verifies, or the refusal is reported as a malformed artifact
    // Broiler-Human:        PENDING
    private bool TryDecodeImportSection()
    {
        if (!TryReadCount(out var count))
        {
            return false;
        }

        if (count > 0)
        {
            return Stop(Invalid(
                VmReason.UnknownFeature,
                WebAssemblyDiagnosticCode.ImportNotAdmitted,
                reader.Position));
        }

        return true;
    }

    /// <summary>Reads one type index per locally defined function.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=2; Fingerprint=22632B
    // Broiler-Human:        PENDING
    private bool TryDecodeFunctionSection()
    {
        if (!TryReadCount(out var count) ||
            !TryAllocateValues<uint>(count, out var decoded))
        {
            return false;
        }

        for (var index = 0u; index < count; index++)
        {
            itemOrdinal = (int)index;

            if (!TryReadVarU32(out var typeIndex))
            {
                return false;
            }

            decoded[index] = typeIndex;
        }

        functionTypeIndices = decoded;
        return true;
    }

    /// <summary>Reads the tables the module defines.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=3A047B
    // Broiler-Falsified-If: a second table is accepted at a format version that defines only one
    // Broiler-Human:        PENDING
    private bool TryDecodeTableSection()
    {
        if (!TryReadCount(out var count))
        {
            return false;
        }

        if (count > 1)
        {
            return Stop(Invalid(
                VmReason.UnknownFeature,
                WebAssemblyDiagnosticCode.MultipleTablesNotAdmitted,
                reader.Position));
        }

        if (!TryAllocateValues<WasmTableType>(count, out var decoded))
        {
            return false;
        }

        for (var index = 0u; index < count; index++)
        {
            itemOrdinal = (int)index;

            if (!TryReadByte(out var elementType))
            {
                return false;
            }

            if (elementType != (byte)WasmValueType.FuncRef)
            {
                return Stop(Invalid(
                    VmReason.MalformedEncoding,
                    WebAssemblyDiagnosticCode.UnknownElementType,
                    reader.Position - 1));
            }

            if (!TryReadLimits(out var limits))
            {
                return false;
            }

            decoded[index] = new WasmTableType(WasmValueType.FuncRef, limits);
        }

        tables = decoded;
        return true;
    }

    /// <summary>Reads the linear memories the module defines.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=BC38E0
    // Broiler-Falsified-If: a memory declaring more pages than a 32-bit address space holds is accepted
    // Broiler-Human:        PENDING
    private bool TryDecodeMemorySection()
    {
        if (!TryReadCount(out var count))
        {
            return false;
        }

        if (count > 1)
        {
            return Stop(Invalid(
                VmReason.UnknownFeature,
                WebAssemblyDiagnosticCode.MultipleMemoriesNotAdmitted,
                reader.Position));
        }

        if (!TryAllocateValues<WasmMemoryType>(count, out var decoded))
        {
            return false;
        }

        for (var index = 0u; index < count; index++)
        {
            itemOrdinal = (int)index;

            if (!TryReadLimits(out var limits))
            {
                return false;
            }

            if (limits.Minimum > WasmTypeGrammar.MaximumMemoryPages ||
                (limits.HasMaximum && limits.Maximum > WasmTypeGrammar.MaximumMemoryPages))
            {
                return Stop(Invalid(
                    VmReason.InconsistentStructure,
                    WebAssemblyDiagnosticCode.MemoryPagesAboveFormatMaximum,
                    reader.Position));
            }

            decoded[index] = new WasmMemoryType(limits);
        }

        memories = decoded;
        return true;
    }

    /// <summary>Reads the globals and their initializing expressions.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=2; Fingerprint=BE7723
    // Broiler-Human:        PENDING
    private bool TryDecodeGlobalSection()
    {
        if (!TryReadCount(out var count) ||
            !TryAllocateReferences<WasmGlobal>(count, out var decoded))
        {
            return false;
        }

        for (var index = 0u; index < count; index++)
        {
            itemOrdinal = (int)index;

            if (!TryReadValueType(out var valueType) ||
                !TryReadByte(out var mutability))
            {
                return false;
            }

            if (mutability > 1)
            {
                return Stop(Invalid(
                    VmReason.MalformedEncoding,
                    WebAssemblyDiagnosticCode.MalformedMutabilityFlag,
                    reader.Position - 1));
            }

            if (!TryReadConstantExpression(out var initializer))
            {
                return false;
            }

            decoded[index] = new WasmGlobal(
                new WasmGlobalType(valueType, mutability == 1), initializer);
        }

        globals = decoded;
        return true;
    }

    /// <summary>Reads the names the module publishes.</summary>
    /// <remarks>
    /// A duplicate export name is a validity rule rather than a well-formedness one, so it is not
    /// checked here. That is a deliberate omission and not an oversight: detecting it needs an
    /// index over a guest-controlled vector, and building one belongs beside the rest of validation
    /// where its cost is charged with everything else's.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=54567B
    // Broiler-Falsified-If: a name that is not well formed under this format's own UTF-8 rule is accepted
    // Broiler-Human:        PENDING
    private bool TryDecodeExportSection()
    {
        if (!TryReadCount(out var count) ||
            !TryAllocateReferences<WasmExport>(count, out var decoded))
        {
            return false;
        }

        for (var index = 0u; index < count; index++)
        {
            itemOrdinal = (int)index;

            if (!TryReadName(out var name) ||
                !TryReadByte(out var kind))
            {
                return false;
            }

            if (kind > (byte)WasmExportKind.Global)
            {
                return Stop(Invalid(
                    VmReason.MalformedEncoding,
                    WebAssemblyDiagnosticCode.UnknownExternalKind,
                    reader.Position - 1));
            }

            if (!TryReadVarU32(out var entityIndex))
            {
                return false;
            }

            decoded[index] = new WasmExport(name, (WasmExportKind)kind, entityIndex);
        }

        exports = decoded;
        return true;
    }

    /// <summary>Reads the start function's index.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=F35E1E
    // Broiler-Human:        PENDING
    private bool TryDecodeStartSection()
    {
        if (!TryReadVarU32(out var index))
        {
            return false;
        }

        startFunctionIndex = index;
        return true;
    }

    /// <summary>Reads the element segments.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=3; Fingerprint=07BA66
    // Broiler-Falsified-If: a segment encoding form this format version does not define is decoded as though it were the classic form
    // Broiler-Human:        PENDING
    private bool TryDecodeElementSection()
    {
        if (!TryReadCount(out var count) ||
            !TryAllocateReferences<WasmElementSegment>(count, out var decoded))
        {
            return false;
        }

        for (var index = 0u; index < count; index++)
        {
            itemOrdinal = (int)index;

            // THE FIRST FIELD IS A TABLE INDEX AT THIS FORMAT VERSION AND A SEGMENT KIND AT A LATER
            // ONE. Reading a non-zero value as a table index would decode a passive or declarative
            // segment as an active one and get a different module out of the same bytes, so
            // anything but zero is refused as a form this build does not define.
            if (!TryReadVarU32(out var tableIndex))
            {
                return false;
            }

            if (tableIndex != 0)
            {
                return Stop(Invalid(
                    VmReason.UnknownFeature,
                    WebAssemblyDiagnosticCode.UnsupportedSegmentKind,
                    reader.Position));
            }

            if (!TryReadConstantExpression(out var offset) ||
                !TryReadCount(out var entryCount) ||
                !TryAllocateValues<uint>(entryCount, out var entries))
            {
                return false;
            }

            for (var entry = 0u; entry < entryCount; entry++)
            {
                if (!TryReadVarU32(out var functionIndex))
                {
                    return false;
                }

                entries[entry] = functionIndex;
            }

            decoded[index] = new WasmElementSegment(tableIndex, offset, entries);
        }

        elements = decoded;
        return true;
    }

    /// <summary>Reads the function bodies.</summary>
    /// <remarks>
    /// <para>
    /// <b>A function body is not entered as a section.</b> The section-count ceiling bounds the
    /// sections of a module, and a body is not one; entering one per function would spend that
    /// ceiling on a number the code section chooses. So the body's declared size is turned into an
    /// expected end position and the position is compared afterwards, which is the same check
    /// without the ceiling confusion.
    /// </para>
    /// <para>
    /// <b>The instruction bytes are copied and not read.</b> Nothing here looks at an opcode. The
    /// bytes are copied into an array the module owns so that the validator has something immutable
    /// to walk that is not a window onto the caller's payload.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=5; Fingerprint=4E1120
    // Broiler-Falsified-If: a body's byte count is taken from anywhere but its declared size, or the expanded local count is not held to the declared-count ceiling
    // Broiler-Human:        PENDING
    private bool TryDecodeCodeSection()
    {
        if (!TryReadCount(out var count) ||
            !TryAllocateReferences<WasmFunctionBody>(count, out var decoded))
        {
            return false;
        }

        for (var index = 0u; index < count; index++)
        {
            itemOrdinal = (int)index;

            if (!TryReadVarU32(out var bodySize))
            {
                return false;
            }

            if (bodySize > reader.Remaining)
            {
                return Stop(Invalid(
                    VmReason.Truncated,
                    WebAssemblyDiagnosticCode.FunctionBodyLengthMismatch,
                    reader.Position));
            }

            var bodyEnd = reader.Position + bodySize;

            if (!TryReadLocals(out var locals))
            {
                return false;
            }

            if (reader.Position > bodyEnd)
            {
                return Stop(Invalid(
                    VmReason.InconsistentStructure,
                    WebAssemblyDiagnosticCode.FunctionBodyLengthMismatch,
                    reader.Position));
            }

            var codeLength = bodyEnd - reader.Position;

            if (!VmBoundedAllocator.TryAllocateExact<byte>(
                in bounds, adapter, codeLength, out var instructions))
            {
                return Stop(VmVerifierOutcome.ResourceExhaustion(
                    VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact));
            }

            if (!reader.TryReadBytes(codeLength, out var window))
            {
                return Stop(FromReader(reader.Position));
            }

            window.CopyTo(System.MemoryExtensions.AsSpan(instructions));

            decoded[index] = new WasmFunctionBody(locals, instructions);
        }

        bodies = decoded;
        return true;
    }

    /// <summary>Reads the data segments.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=3; Fingerprint=077959
    // Broiler-Falsified-If: a segment encoding form this format version does not define is decoded as though it were the classic form
    // Broiler-Human:        PENDING
    private bool TryDecodeDataSection()
    {
        if (!TryReadCount(out var count) ||
            !TryAllocateReferences<WasmDataSegment>(count, out var decoded))
        {
            return false;
        }

        for (var index = 0u; index < count; index++)
        {
            itemOrdinal = (int)index;

            if (!TryReadVarU32(out var memoryIndex))
            {
                return false;
            }

            if (memoryIndex != 0)
            {
                return Stop(Invalid(
                    VmReason.UnknownFeature,
                    WebAssemblyDiagnosticCode.UnsupportedSegmentKind,
                    reader.Position));
            }

            if (!TryReadConstantExpression(out var offset) ||
                !TryReadCount(out var byteCount) ||
                !TryAllocateValues<byte>(byteCount, out var contents))
            {
                return false;
            }

            if (!reader.TryReadBytes(byteCount, out var window))
            {
                return Stop(FromReader(reader.Position));
            }

            window.CopyTo(System.MemoryExtensions.AsSpan(contents));
            decoded[index] = new WasmDataSegment(memoryIndex, offset, contents);
        }

        data = decoded;
        return true;
    }

    /// <summary>Reads the declared data segment count.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=EC6DB5
    // Broiler-Human:        PENDING
    private bool TryDecodeDataCountSection()
    {
        if (!TryReadVarU32(out var count))
        {
            return false;
        }

        declaresDataCount = true;
        declaredDataCount = count;
        return true;
    }

    /// <summary>
    /// The two relationships between sections this decoder is responsible for.
    /// </summary>
    /// <remarks>
    /// Both are properties of the encoding rather than of the module's meaning, which is why they
    /// are here and why every other cross-section rule is not: a function without a body and a data
    /// count that disagrees with the data section are disagreements between two parts of one
    /// artifact, and an index that addresses nothing is a statement about the module.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=AEF1C9
    // Broiler-Falsified-If: a module whose function and code counts differ is decoded successfully
    // Broiler-Human:        PENDING
    private bool TryCheckSectionAgreement()
    {
        if (functionTypeIndices.Length != bodies.Length)
        {
            return Stop(Invalid(
                VmReason.InconsistentStructure,
                WebAssemblyDiagnosticCode.FunctionAndCodeCountMismatch,
                reader.Position));
        }

        if (declaresDataCount && declaredDataCount != (uint)data.Length)
        {
            return Stop(Invalid(
                VmReason.InconsistentStructure,
                WebAssemblyDiagnosticCode.DataCountMismatch,
                reader.Position));
        }

        return true;
    }

    /// <summary>Reads a vector of value types.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=D547B9
    // Broiler-Falsified-If: the vector is sized before its count has cleared the declared-count ceiling
    // Broiler-Human:        PENDING
    private bool TryReadValueTypeVector(out WasmValueType[] vector)
    {
        vector = [];

        if (!TryReadCount(out var count) ||
            !TryAllocateValues<WasmValueType>(count, out var decoded))
        {
            return false;
        }

        for (var index = 0u; index < count; index++)
        {
            if (!TryReadValueType(out var valueType))
            {
                return false;
            }

            decoded[index] = valueType;
        }

        vector = decoded;
        return true;
    }

    /// <summary>Reads one value type byte, separating an unknown one from an unadmitted one.</summary>
    /// <remarks>
    /// The two answers are different facts. A byte naming no value type is a malformed encoding; a
    /// byte naming the vector or reference types is a well-formed encoding of a feature no manifest
    /// here accepts, and reporting it as malformed would tell a caller its toolchain emitted rubbish
    /// when it emitted a surface this build does not implement.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=6D564B
    // Broiler-Falsified-If: an unadmitted value type and an undefined byte produce the same diagnostic code
    // Broiler-Human:        PENDING
    private bool TryReadValueType(out WasmValueType valueType)
    {
        valueType = WasmValueType.I32;

        if (!TryReadByte(out var encoded))
        {
            return false;
        }

        if (!WasmTypeGrammar.IsAnyValueType(encoded))
        {
            return Stop(Invalid(
                VmReason.MalformedEncoding,
                WebAssemblyDiagnosticCode.UnknownValueType,
                reader.Position - 1));
        }

        if (!WasmTypeGrammar.IsNumericValueType(encoded))
        {
            return Stop(Invalid(
                VmReason.UnknownFeature,
                WebAssemblyDiagnosticCode.ValueTypeNotAdmitted,
                reader.Position - 1));
        }

        valueType = (WasmValueType)encoded;
        return true;
    }

    /// <summary>Reads a resizable limit.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=ADE89C
    // Broiler-Falsified-If: a limit whose minimum is above its maximum is accepted
    // Broiler-Human:        PENDING
    private bool TryReadLimits(out WasmLimits limits)
    {
        limits = default;

        if (!TryReadByte(out var flag))
        {
            return false;
        }

        if (flag is not (WasmTypeGrammar.LimitsMinimumOnly or WasmTypeGrammar.LimitsMinimumAndMaximum))
        {
            return Stop(Invalid(
                VmReason.MalformedEncoding,
                WebAssemblyDiagnosticCode.MalformedLimitsFlag,
                reader.Position - 1));
        }

        if (!TryReadVarU32(out var minimum))
        {
            return false;
        }

        if (flag == WasmTypeGrammar.LimitsMinimumOnly)
        {
            limits = new WasmLimits(minimum, 0, false);
            return true;
        }

        if (!TryReadVarU32(out var maximum))
        {
            return false;
        }

        if (minimum > maximum)
        {
            return Stop(Invalid(
                VmReason.InconsistentStructure,
                WebAssemblyDiagnosticCode.LimitsMinimumAboveMaximum,
                reader.Position));
        }

        limits = new WasmLimits(minimum, maximum, true);
        return true;
    }

    /// <summary>Reads a name: a byte vector held to this format's own UTF-8 rule.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=1BD6B8
    // Broiler-Falsified-If: the platform's UTF-8 decoder is consulted, or the bytes are retained before the rule has admitted them
    // Broiler-Human:        PENDING
    private bool TryReadName(out byte[] name)
    {
        name = [];

        if (!TryReadCount(out var length) ||
            !TryAllocateValues<byte>(length, out var buffer))
        {
            return false;
        }

        if (!reader.TryReadBytes(length, out var window))
        {
            return Stop(FromReader(reader.Position));
        }

        if (!WasmName.IsWellFormed(window))
        {
            return Stop(Invalid(
                VmReason.MalformedEncoding,
                WebAssemblyDiagnosticCode.MalformedNameEncoding,
                reader.Position - length));
        }

        window.CopyTo(System.MemoryExtensions.AsSpan(buffer));
        name = buffer;
        return true;
    }

    /// <summary>Reads a constant expression in the only shape this format version admits.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=3; Fingerprint=C2310C
    // Broiler-Falsified-If: an expression not closed by the end opcode is accepted, or an instruction outside the constant set is decoded
    // Broiler-Human:        PENDING
    private bool TryReadConstantExpression(out WasmConstantExpression expression)
    {
        expression = default;

        if (!TryReadByte(out var opcode))
        {
            return false;
        }

        ulong bits;

        switch (opcode)
        {
            case (byte)WasmOpcode.I32Const:
                if (!TryReadVarS32(out var narrow))
                {
                    return false;
                }

                bits = (ulong)(long)narrow;
                break;

            case (byte)WasmOpcode.I64Const:
                if (!TryReadVarS64(out var wide))
                {
                    return false;
                }

                bits = (ulong)wide;
                break;

            case (byte)WasmOpcode.F32Const:
                if (!reader.TryReadUInt32LittleEndian(out var single))
                {
                    return Stop(FromReader(reader.Position));
                }

                bits = single;
                break;

            case (byte)WasmOpcode.F64Const:
                if (!reader.TryReadUInt64LittleEndian(out var doublePrecision))
                {
                    return Stop(FromReader(reader.Position));
                }

                bits = doublePrecision;
                break;

            case (byte)WasmOpcode.GlobalGet:
                if (!TryReadVarU32(out var globalIndex))
                {
                    return false;
                }

                bits = globalIndex;
                break;

            default:
                return Stop(Invalid(
                    VmReason.UnknownFeature,
                    WebAssemblyDiagnosticCode.UnsupportedConstantExpressionOpcode,
                    reader.Position - 1));
        }

        if (!TryReadByte(out var terminator))
        {
            return false;
        }

        if (terminator != WasmFormat.EndOpcode)
        {
            return Stop(Invalid(
                VmReason.InconsistentStructure,
                WebAssemblyDiagnosticCode.ConstantExpressionNotTerminated,
                reader.Position - 1));
        }

        expression = new WasmConstantExpression(opcode, bits);
        return true;
    }

    /// <summary>Reads a body's local declarations and expands them.</summary>
    /// <remarks>
    /// The runs are read into a bounded vector first, and their total is accumulated and held to the
    /// declared-count ceiling before the expanded array is sized. A decoder that expanded as it went
    /// would let a body of six bytes ask for a billion locals one run at a time.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=3; Fingerprint=A839E9
    // Broiler-Falsified-If: the expanded array is sized before the accumulated total has cleared the declared-count ceiling
    // Broiler-Human:        PENDING
    private bool TryReadLocals(out WasmValueType[] locals)
    {
        locals = [];

        if (!TryReadCount(out var runCount) ||
            !TryAllocateValues<WasmLocalRun>(runCount, out var runs))
        {
            return false;
        }

        ulong total = 0;

        for (var index = 0u; index < runCount; index++)
        {
            if (!TryReadCount(out var repeats) ||
                !TryReadValueType(out var valueType))
            {
                return false;
            }

            total += repeats;

            if (total > reader.Bounds.MaxDeclaredCount || total > uint.MaxValue)
            {
                return Stop(VmVerifierOutcome.ResourceExhaustion(
                    VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact));
            }

            runs[index].Count = repeats;
            runs[index].Type = valueType;
        }

        if (!TryAllocateValues<WasmValueType>((uint)total, out var expanded))
        {
            return false;
        }

        var written = 0;

        for (var index = 0u; index < runCount; index++)
        {
            for (var repeat = 0u; repeat < runs[index].Count; repeat++)
            {
                expanded[written] = runs[index].Type;
                written++;
            }
        }

        locals = expanded;
        return true;
    }

    /// <summary>Reads one byte, converting a reader refusal into this decoder's answer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8E773C
    // Broiler-Human:        PENDING
    private bool TryReadByte(out byte value)
    {
        if (reader.TryReadByte(out value))
        {
            return true;
        }

        return Stop(FromReader(reader.Position));
    }

    /// <summary>Reads an unsigned 32-bit variable-length integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=156A4E
    // Broiler-Human:        PENDING
    private bool TryReadVarU32(out uint value)
    {
        var at = reader.Position;

        if (WasmLeb128.TryReadVarU32(ref reader, out value, out var status))
        {
            return true;
        }

        return Stop(FromVarInt(status, at));
    }

    /// <summary>Reads a signed 32-bit variable-length integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=240209
    // Broiler-Human:        PENDING
    private bool TryReadVarS32(out int value)
    {
        var at = reader.Position;

        if (WasmLeb128.TryReadVarS32(ref reader, out value, out var status))
        {
            return true;
        }

        return Stop(FromVarInt(status, at));
    }

    /// <summary>Reads a signed 64-bit variable-length integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=699245
    // Broiler-Human:        PENDING
    private bool TryReadVarS64(out long value)
    {
        var at = reader.Position;

        if (WasmLeb128.TryReadVarS64(ref reader, out value, out var status))
        {
            return true;
        }

        return Stop(FromVarInt(status, at));
    }

    /// <summary>Reads a vector length that has cleared the declared-count ceiling.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D4C69D
    // Broiler-Falsified-If: it answers true for a count that has not been both compared and charged
    // Broiler-Human:        PENDING
    private bool TryReadCount(out uint count)
    {
        var at = reader.Position;

        if (WasmLeb128.TryReadBoundedCount(ref reader, adapter, out count, out var status))
        {
            return true;
        }

        return Stop(FromVarInt(status, at));
    }

    /// <summary>Sizes an unmanaged vector through the core's bounded allocator.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3E19A9
    // Broiler-Falsified-If: an array is created on a path where the allocator refused
    // Broiler-Human:        PENDING
    private bool TryAllocateValues<TElement>(uint count, out TElement[] buffer)
        where TElement : unmanaged
    {
        if (VmBoundedAllocator.TryAllocate<TElement>(in bounds, adapter, count, out buffer))
        {
            return true;
        }

        return Stop(VmVerifierOutcome.ResourceExhaustion(
            VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact));
    }

    /// <summary>
    /// Sizes a reference vector, reserving its bytes against the meter before it exists.
    /// </summary>
    /// <remarks>
    /// The core's allocator takes unmanaged elements only, because it computes a size from a known
    /// element width. A reference array's width is the pointer width, so the same discipline is
    /// applied here by hand: the count has already cleared its ceiling, the byte count is computed
    /// with checked arithmetic, the bytes are reserved, and only then is the array created.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=Critical; Resources=2; Fingerprint=155D19
    // Broiler-Falsified-If: the array is created before the reservation returns true, or the byte-count product is unchecked
    // Broiler-Human:        PENDING
    private bool TryAllocateReferences<TElement>(uint count, out TElement[] buffer)
        where TElement : class
    {
        buffer = [];

        if (count == 0)
        {
            return true;
        }

        if (count > int.MaxValue)
        {
            return Stop(VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact));
        }

        ulong byteCount;

        try
        {
            checked
            {
                byteCount = (ulong)count * (ulong)System.IntPtr.Size;
            }
        }
        catch (System.OverflowException)
        {
            return Stop(VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact));
        }

        if (byteCount > bounds.MaxArtifactBytes || !adapter.TryReserve(byteCount))
        {
            return Stop(VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact));
        }

        try
        {
            buffer = new TElement[count];
        }
        catch (System.OutOfMemoryException)
        {
            adapter.Release(byteCount);

            return Stop(VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact));
        }

        return true;
    }

    /// <summary>Records the refusal and answers false, which is the only way this decoder stops.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=349598
    // Broiler-Human:        PENDING
    private bool Stop(VmVerifierOutcome outcome)
    {
        refusal = outcome;
        return false;
    }

    /// <summary>An invalid-artifact answer carrying this profile's code and a byte position.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=680E03
    // Broiler-Human:        PENDING
    private VmVerifierOutcome Invalid(VmReason reason, WebAssemblyDiagnosticCode code, ulong offset) =>
        WasmRefusal.Invalid(reason, code, At(offset));

    /// <summary>
    /// The position encoding this profile publishes.
    /// </summary>
    /// <remarks>
    /// The section index is the section's ordinal in the module, or minus one outside every section.
    /// The first profile coordinate is the section identifier and the second is the index inside the
    /// section's own vector, each minus one where it does not apply. The core never parses, orders
    /// or compares any of it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AD6768
    // Broiler-Human:        PENDING
    private VmSourcePosition At(ulong offset) =>
        new(sectionOrdinal, offset, sectionIdentifier, itemOrdinal);

    /// <summary>Maps the bounded reader's mechanism status onto the answers a verifier may give.</summary>
    /// <remarks>
    /// The mapping itself is <see cref="WasmRefusal"/>'s, shared with the validation pass, because
    /// two copies of the invalid-artifact-or-resource-exhaustion split would eventually disagree.
    /// What this member adds is where the failure is, which is this pass's own knowledge.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0005; IP=Low; Security=Medium; Resources=1; Fingerprint=5C6656
    // Broiler-Human:        PENDING
    private VmVerifierOutcome FromReader(ulong offset) =>
        WasmRefusal.FromReader(reader.Status, At(offset));

    /// <summary>Maps a variable-length integer refusal onto the answers a verifier may give.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3C8F49
    // Broiler-Human:        PENDING
    private VmVerifierOutcome FromVarInt(WasmVarIntStatus status, ulong offset) =>
        WasmRefusal.FromVarInt(status, reader.Status, At(offset));
}
