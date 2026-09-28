// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   45
// Annotated:        45/45
// Exempt:           27
// Human-reviewed:   0/45
// IP risk:          Low
// Security risk:    Critical
// Criteria:         28/28
// Resource impact:  8/10 max
// Unverified:       45
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
/// <para>
/// <b>The work between two polls stays inside the bound.</b> Every byte consumed is charged as
/// verifier work before it is consumed, and no single read charges more than the read window - half
/// the uncharged-work bound and one unit - while the bounded reader polls at the other half, so a
/// read begun just short of a poll still ends inside the bound. A byte run longer than the window, a
/// function body, a data segment's contents, a name, a skipped custom section or a fixed-width
/// field alike, is read in pieces no longer than it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=8; Fingerprint=399155
// Broiler-Falsified-If: a buffer is sized from a count that has not cleared its ceiling, a ceiling breach is reported as a malformed artifact, or one read charges more work than the read window
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

    /// <summary>The most work one read charges; see <see cref="WasmReadAdapter.ReadWindow"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4C0F80
    // Broiler-Human:        PENDING
    private readonly ulong readWindow;

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

    /// <summary>
    /// The refusal a declared import earns, held until decoding completes; see
    /// <see cref="TryDecodeImportSection"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=670E55
    // Broiler-Human:        PENDING
    private VmVerifierOutcome unadmittedImport;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=872B04
    // Broiler-Human:        PENDING
    private bool declaresImport;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5E586D
    // Broiler-Human:        PENDING
    private uint declaredDataCount;

    /// <summary>
    /// Builds a decoder over one payload, under ceilings the caller has already materialized.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The bounds arrive as a parameter rather than being read here, because the ordering that
    /// matters - the ceilings are fixed before the first byte is examined - is a property of the
    /// translator's sequence and is stated there.
    /// </para>
    /// <para>
    /// <paramref name="pollGranularity"/> is the uncharged-work bound, and it is not handed to the
    /// reader as it is: the reader polls at the rest of the bound after one read window, and every
    /// read here charges at most one window, so the work charged between two polls never exceeds it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=765C18
    // Broiler-Falsified-If: any field is left uninitialised so a failed decode hands back an array nothing filled, or the reader is built polling at the whole bound rather than at the rest of it after one read window
    // Broiler-Human:        PENDING
    internal WasmDecoder(
        System.ReadOnlySpan<byte> payload,
        in VmReadBounds readBounds,
        WasmReadAdapter meter,
        ulong pollGranularity)
    {
        adapter = meter;
        bounds = readBounds;
        readWindow = WasmReadAdapter.ReadWindow(pollGranularity);
        reader = new VmBoundedReader(
            payload, in readBounds, meter, WasmReadAdapter.ReaderPollGranularity(pollGranularity));
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
        unadmittedImport = default;
        declaresImport = false;
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
    /// A custom section's name is read and held to the format's UTF-8 rule, and the rest of its body
    /// is stepped over without a byte of it being looked at. It is counted so that a caller can see
    /// it was there. It takes no position in the order and it may repeat.
    /// <i>(Corrected 2026-09-28. This paragraph read "A custom section is entered, skipped and exited
    /// without a byte of its body being looked at", and the decoder did that: a custom section whose
    /// name was not UTF-8, or whose name ran past the section, was accepted. The format makes the name
    /// part of the section's grammar, and the specification's own scripts refuse both as malformed.)</i>
    /// </para>
    /// <para>
    /// A declared import is refused only after the last section has been read and the sections agree,
    /// so a module malformed anywhere is answered as malformed. The refusal itself is made where the
    /// import section is read and held until then.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=6; Fingerprint=1F900B
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

        if (declaresImport)
        {
            outcome = unadmittedImport;
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
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=A74BC2
    // Broiler-Falsified-If: a payload whose first eight bytes are not the magic and version 1 reaches the section loop
    // Broiler-Human:        PENDING
    private bool TryReadPreamble()
    {
        System.Span<byte> magic = stackalloc byte[4];

        // The only refusal here that carries a position is a truncation, which the paced read makes
        // before consuming anything, so it is reported at offset zero as it always was.
        if (!TryReadPaced((ulong)magic.Length, magic))
        {
            return false;
        }

        if (!System.MemoryExtensions.SequenceEqual(magic, WasmFormat.Magic))
        {
            return Stop(Invalid(VmReason.MalformedEncoding, WebAssemblyDiagnosticCode.WrongMagic, 0));
        }

        // Four raw little-endian bytes rather than a variable-length integer: the version field is
        // fixed width in this format, and reading it as a varint would accept encodings the format
        // does not have.
        if (!TryReadFixedWidth(4, out var version))
        {
            return false;
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=5; Fingerprint=15EEDF
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

            if (!TryReadCustomSection(declaredLength))
            {
                return false;
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

    /// <summary>
    /// Reads a custom section's name, and steps over the rest of its body unread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The name is the one part of a custom section the format gives a grammar</b>, and it is a
    /// name like any other: a length and bytes that are well formed under the format's own UTF-8
    /// rule. What follows it is the producer's, and nothing here reads it.
    /// </para>
    /// <para>
    /// <b>A name that runs past its section is refused here, before the rest is stepped over.</b> The
    /// reader holds a section to its declared length only when the section is exited, so a name longer
    /// than the section reads the next section's bytes as its own, and what is left of the section
    /// would be a negative length. It is the same disagreement the exit reports, and it carries the
    /// same code.
    /// </para>
    /// <para>
    /// The rest is stepped over in pieces rather than skipped: the reader's skip charges the whole
    /// run in one charge, and a custom section is as long as its producer likes.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=3C1FC6
    // Broiler-Falsified-If: a custom section whose name is not well formed under this format's own UTF-8 rule, or whose name runs past the section, is accepted, or a byte after the name is examined
    // Broiler-Human:        PENDING
    private bool TryReadCustomSection(uint declaredLength)
    {
        var start = reader.Position;

        if (!TryReadName(out _))
        {
            return false;
        }

        var consumed = reader.Position - start;

        if (consumed > declaredLength)
        {
            return Stop(Invalid(
                VmReason.InconsistentStructure,
                WebAssemblyDiagnosticCode.SectionLengthMismatch,
                reader.Position));
        }

        return TryReadPaced(declaredLength - consumed, default);
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
    /// <para>
    /// <b>No feature manifest this profile accepts admits an import, and a manifest is refused
    /// rather than degraded.</b> A non-empty import section is an unadmitted feature rather than a
    /// malformation. An empty one is a legal encoding of a module that imports nothing, and it is
    /// accepted.
    /// </para>
    /// <para>
    /// <b>The refusal waits until the whole module has decoded, because a malformed module is
    /// malformed whatever this profile admits.</b> Every entry is read: the two names are held to the
    /// format's UTF-8 rule, the kind byte to the four kinds, and each kind's description to its own
    /// grammar, by the readers the module's own definitions use. Nothing read is kept. The section is
    /// then framed and exited like any other, and so is every section after it, so a malformation
    /// anywhere in the module is answered first. The refusal is made here and held, so it names this
    /// section and the position after the count, as it did when the count was all that was read.
    /// <i>(Corrected 2026-09-28. The section was refused as soon as its count was read, so an import
    /// whose name was not UTF-8, whose kind named nothing or whose description was malformed, or a
    /// module malformed after its imports, was answered as unadmitted. That told a caller nothing about
    /// a module the format calls malformed, which is what the specification's scripts ask.)</i>
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=87EB93
    // Broiler-Falsified-If: a module declaring an import verifies, the refusal is reported as a malformed artifact for a well-formed module, or a module malformed anywhere is refused as unadmitted
    // Broiler-Human:        PENDING
    private bool TryDecodeImportSection()
    {
        if (!TryReadCount(out var count))
        {
            return false;
        }

        var at = reader.Position;

        for (var index = 0u; index < count; index++)
        {
            itemOrdinal = (int)index;

            if (!TryReadName(out _) ||
                !TryReadName(out _) ||
                !TryReadByte(out var kind))
            {
                return false;
            }

            var described = (WasmExportKind)kind switch
            {
                WasmExportKind.Function => TryReadVarU32(out _),
                WasmExportKind.Table => TryReadTableType(out _),
                WasmExportKind.Memory => TryReadMemoryType(out _),
                WasmExportKind.Global => TryReadGlobalType(out _),
                _ => Stop(Invalid(
                    VmReason.MalformedEncoding,
                    WebAssemblyDiagnosticCode.UnknownExternalKind,
                    reader.Position - 1)),
            };

            if (!described)
            {
                return false;
            }
        }

        itemOrdinal = -1;

        if (count > 0)
        {
            unadmittedImport = Invalid(
                VmReason.UnknownFeature,
                WebAssemblyDiagnosticCode.ImportNotAdmitted,
                at);
            declaresImport = true;
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
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=4FB934
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

            if (!TryReadTableType(out var table))
            {
                return false;
            }

            decoded[index] = table;
        }

        tables = decoded;
        return true;
    }

    /// <summary>Reads a table type: the element type, then the limits.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=054FFF
    // Broiler-Human:        PENDING
    private bool TryReadTableType(out WasmTableType table)
    {
        table = default;

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

        table = new WasmTableType(WasmValueType.FuncRef, limits);
        return true;
    }

    /// <summary>Reads the linear memories the module defines.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=032266
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

            if (!TryReadMemoryType(out var memory))
            {
                return false;
            }

            decoded[index] = memory;
        }

        memories = decoded;
        return true;
    }

    /// <summary>Reads a memory type: limits no larger than a 32-bit address space holds.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=3B0387
    // Broiler-Falsified-If: a memory type declaring more pages than a 32-bit address space holds is read
    // Broiler-Human:        PENDING
    private bool TryReadMemoryType(out WasmMemoryType memory)
    {
        memory = default;

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

        memory = new WasmMemoryType(limits);
        return true;
    }

    /// <summary>Reads the globals and their initializing expressions.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=2; Fingerprint=6A7ADF
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

            if (!TryReadGlobalType(out var type) ||
                !TryReadConstantExpression(out var initializer))
            {
                return false;
            }

            decoded[index] = new WasmGlobal(type, initializer);
        }

        globals = decoded;
        return true;
    }

    /// <summary>Reads a global type: the value type, then a mutability byte of zero or one.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=9CC4CB
    // Broiler-Human:        PENDING
    private bool TryReadGlobalType(out WasmGlobalType type)
    {
        type = default;

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

        type = new WasmGlobalType(valueType, mutability == 1);
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
    /// to walk that is not a window onto the caller's payload - in pieces no longer than the read
    /// window, because a body is as long as its producer likes and one charge for the whole of it
    /// would carry the work between two polls past the uncharged-work bound.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=5; Fingerprint=58F41D
    // Broiler-Falsified-If: a body's byte count is taken from anywhere but its declared size, the expanded local count is not held to the declared-count ceiling, or a body's instruction bytes are read in one charge larger than the read window
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

            if (!TryReadPaced(codeLength, instructions))
            {
                return false;
            }

            decoded[index] = new WasmFunctionBody(locals, instructions);
        }

        bodies = decoded;
        return true;
    }

    /// <summary>Reads the data segments.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=3; Fingerprint=B0C933
    // Broiler-Falsified-If: a segment encoding form this format version does not define is decoded as though it were the classic form, or a segment's contents are read in one charge larger than the read window
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
                !TryAllocateValues<byte>(byteCount, out var contents) ||
                !TryReadPaced(byteCount, contents))
            {
                return false;
            }

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
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=4A96BB
    // Broiler-Falsified-If: the platform's UTF-8 decoder is consulted, the bytes are retained before the rule has admitted them, or the rule is applied to a piece of the name rather than to the whole of it
    // Broiler-Human:        PENDING
    private bool TryReadName(out byte[] name)
    {
        name = [];

        if (!TryReadCount(out var length) ||
            !TryAllocateValues<byte>(length, out var buffer) ||
            !TryReadPaced(length, buffer))
        {
            return false;
        }

        // THE RULE IS APPLIED TO THE WHOLE NAME, AFTER EVERY PIECE HAS ARRIVED. A piece boundary can
        // fall inside a multi-byte sequence, so a rule applied piece by piece would refuse a
        // well-formed name. The buffer is this member's own until the rule admits it.
        if (!WasmName.IsWellFormed(buffer))
        {
            return Stop(Invalid(
                VmReason.MalformedEncoding,
                WebAssemblyDiagnosticCode.MalformedNameEncoding,
                reader.Position - length));
        }

        name = buffer;
        return true;
    }

    /// <summary>Reads a constant expression in the only shape this format version admits.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=3; Fingerprint=81543C
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
                if (!TryReadFixedWidth(4, out var single))
                {
                    return false;
                }

                bits = single;
                break;

            case (byte)WasmOpcode.F64Const:
                if (!TryReadFixedWidth(8, out var doublePrecision))
                {
                    return false;
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

    /// <summary>
    /// Consumes <paramref name="length"/> bytes in pieces no longer than the read window, copying
    /// them into <paramref name="destination"/> unless it is empty, which steps over them unread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Each piece is charged before it is consumed and the reader polls between pieces</b>, so a
    /// run of any length keeps the work between two polls inside the uncharged-work bound. Every
    /// byte is charged exactly once, as the one read of the whole run charged it, so the verifier
    /// work a module costs does not change; only where the polls fall does.
    /// </para>
    /// <para>
    /// <b>An empty run and one the payload cannot hold are handed to the reader whole.</b> Neither
    /// charges a unit of work: the first consumes nothing, and the second is refused before any of it
    /// is consumed, exactly as one read of the whole length was - so no refusal position moves with
    /// the window.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=High; Resources=1; Fingerprint=7EF4EB
    // Broiler-Falsified-If: one piece charges more work than the read window, a run the payload cannot hold consumes bytes before it is refused, or a byte is copied past the destination
    // Broiler-Human:        PENDING
    private bool TryReadPaced(ulong length, scoped System.Span<byte> destination)
    {
        if (length == 0 || length > reader.Remaining)
        {
            return reader.TryReadBytes(length, out _) || Stop(FromReader(reader.Position));
        }

        var consumed = 0UL;

        while (consumed < length)
        {
            var piece = System.Math.Min(readWindow, length - consumed);

            if (!reader.TryReadBytes(piece, out var chunk))
            {
                return Stop(FromReader(reader.Position));
            }

            if (!destination.IsEmpty)
            {
                chunk.CopyTo(destination[(int)consumed..]);
            }

            consumed += piece;
        }

        return true;
    }

    /// <summary>
    /// Reads a little-endian fixed-width field - four bytes or eight - paced like any other run.
    /// </summary>
    /// <remarks>
    /// Eight bytes is inside the read window under any bound of fourteen or more, and this profile
    /// declares 65,536; the field is paced anyway so that no read in this decoder is the exception a
    /// smaller bound would find.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=96CDD8
    // Broiler-Falsified-If: the value is assembled other than little-endian, or from bytes the paced read did not consume
    // Broiler-Human:        PENDING
    private bool TryReadFixedWidth(int width, out ulong value)
    {
        value = 0;
        System.Span<byte> field = stackalloc byte[sizeof(ulong)];

        if (!TryReadPaced((ulong)width, field[..width]))
        {
            return false;
        }

        for (var index = width - 1; index >= 0; index--)
        {
            value = (value << 8) | field[index];
        }

        return true;
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
