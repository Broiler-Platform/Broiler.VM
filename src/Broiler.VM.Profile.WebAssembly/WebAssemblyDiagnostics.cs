// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           76
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  1/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// This profile's own stable diagnostic codes. The core attaches no meaning to any of them.
/// </summary>
/// <remarks>
/// <para>
/// <b>The registry is bound in both directions or it is not a registry.</b> Every code this
/// assembly can emit is a member below, and every member below is reachable from a decode path in
/// <see cref="WasmDecoder"/>, a validation path in <see cref="WasmValidator"/>, or the reserved
/// path in <see cref="WebAssemblyVerifier"/>. The numbers are grouped by the pass that emits them
/// so that a reader can tell from a code alone which pass refused an artifact, and a code is never
/// reused for a different meaning: a rejection whose meaning changes takes a new number and the old
/// one is retired, because a corpus entry that recorded a code has dated it.
/// </para>
/// <para>
/// <b>The two thousand and the twenty-one hundred through twenty-six hundred bands are decoding;
/// the twenty-seven hundred and twenty-eight hundred bands are validation.</b> The split is
/// observable and it is meant to be: decoding completes before validation begins at module
/// granularity, so a module that is both malformed and invalid answers with a code below 2700, and
/// a corpus entry that recorded a 2700-band code for a module a decoder should have refused has
/// recorded the phases fused.
/// </para>
/// <para>
/// <b>Every code carries a core reason and a byte position, and the two categories do not mix.</b>
/// A code here always accompanies <c>InvalidArtifact</c>. A breach of an effective ceiling carries
/// no code at all: it is a resource exhaustion naming a dimension and a scope, and giving it a
/// diagnostic code would invite a reader to treat a host's spending decision as a property of the
/// module.
/// </para>
/// <para>
/// <b>The 2900 band is reserved and is not a decode diagnostic.</b> It is the answer a defect in
/// this assembly produces, and reaching it is a bug to be fixed rather than a rejection route: the
/// core deliberately does not catch a verifier's exception, so an escape would surface as a crash
/// in the caller rather than as an answer, and converting one into a refusal is the only way this
/// verifier can be total.
/// </para>
/// <para>
/// <b>It is an enum rather than a class of constants</b>, for the reason the JavaScript profile's
/// registry records: a closed vocabulary is one reviewable thing, and a class of constants would
/// be several dozen separately assessed fixed values saying the same thing worse.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=11871F
// Broiler-Human:        PENDING
public enum WebAssemblyDiagnosticCode
{
    // ---- 2000: the preamble, before any section is framed ---------------------------------

    /// <summary>The first four bytes are not the format's magic.</summary>
    WrongMagic = 2001,

    /// <summary>The four bytes after the magic are not a binary format version this build reads.</summary>
    UnsupportedBinaryVersion = 2002,

    /// <summary>The artifact descriptor names a format version outside this profile's range.</summary>
    UnsupportedArtifactFormatVersion = 2003,

    /// <summary>The artifact descriptor names a feature manifest this verifier does not accept.</summary>
    UnacceptedFeatureManifest = 2004,

    // ---- 2100: section framing ------------------------------------------------------------

    /// <summary>A byte where a section identifier is expected names no section.</summary>
    UnknownSectionId = 2101,

    /// <summary>A section appears earlier than the canonical order allows.</summary>
    SectionOutOfOrder = 2102,

    /// <summary>A non-custom section appears twice.</summary>
    DuplicateSection = 2103,

    /// <summary>A section body did not consume exactly its declared length.</summary>
    SectionLengthMismatch = 2104,

    /// <summary>A tag section is present, and no feature manifest here admits one.</summary>
    TagSectionNotAdmitted = 2105,

    /// <summary>The payload ended where the format says more bytes follow.</summary>
    Truncated = 2106,

    /// <summary>
    /// The bounded reader refused a framing operation for a reason the profile did not name first.
    /// </summary>
    /// <remarks>
    /// Every framing failure this decoder can predict carries its own code above. This one is what
    /// remains: a reader-level malformation on a path where the profile has nothing more precise to
    /// say than that the framing broke.
    /// </remarks>
    ReaderMalformedEncoding = 2107,

    // ---- 2200: variable-length integers ----------------------------------------------------

    /// <summary>An integer's encoding runs past the byte budget its width allows.</summary>
    IntegerRepresentationTooLong = 2201,

    /// <summary>An unsigned integer's terminal byte sets bits above the integer's width.</summary>
    IntegerTooLarge = 2202,

    /// <summary>A signed integer's terminal byte is not a correct sign extension.</summary>
    IntegerSignExtensionInvalid = 2203,

    // ---- 2300: the type grammar -------------------------------------------------------------

    /// <summary>A function type does not begin with the byte the format assigns it.</summary>
    MalformedFunctionTypeTag = 2301,

    /// <summary>A byte where a value type is expected names no value type.</summary>
    UnknownValueType = 2302,

    /// <summary>A value type the format defines and no manifest here admits.</summary>
    ValueTypeNotAdmitted = 2303,

    /// <summary>A limits flag byte is neither of the two the format defines.</summary>
    MalformedLimitsFlag = 2304,

    /// <summary>A declared minimum is above the declared maximum beside it.</summary>
    LimitsMinimumAboveMaximum = 2305,

    /// <summary>A memory declares more pages than a 32-bit address space holds.</summary>
    MemoryPagesAboveFormatMaximum = 2306,

    /// <summary>A table's element type is not one this format version admits.</summary>
    UnknownElementType = 2307,

    /// <summary>A global's mutability byte is neither zero nor one.</summary>
    MalformedMutabilityFlag = 2308,

    // ---- 2400: names, imports and exports ---------------------------------------------------

    /// <summary>A name is not well formed under the format's own UTF-8 rule.</summary>
    MalformedNameEncoding = 2401,

    /// <summary>A byte where an import or export kind is expected names no kind.</summary>
    UnknownExternalKind = 2402,

    /// <summary>The module declares an import, and no feature manifest here admits one.</summary>
    ImportNotAdmitted = 2403,

    // ---- 2500: the relationships between sections -------------------------------------------

    /// <summary>The function section and the code section declare different counts.</summary>
    FunctionAndCodeCountMismatch = 2501,

    /// <summary>The data count section and the data section declare different counts.</summary>
    DataCountMismatch = 2502,

    /// <summary>A function body did not consume exactly its declared size.</summary>
    FunctionBodyLengthMismatch = 2503,

    /// <summary>A module declares more than one memory, which this format version does not admit.</summary>
    MultipleMemoriesNotAdmitted = 2504,

    /// <summary>A module declares more than one table, which this format version does not admit.</summary>
    MultipleTablesNotAdmitted = 2505,

    // ---- 2600: constant expressions and segments --------------------------------------------

    /// <summary>An instruction appears in a constant expression that may not hold one.</summary>
    UnsupportedConstantExpressionOpcode = 2601,

    /// <summary>A constant expression does not end where the format says it must.</summary>
    ConstantExpressionNotTerminated = 2602,

    /// <summary>A segment uses an encoding form this format version does not define.</summary>
    UnsupportedSegmentKind = 2603,

    // ---- 2700: validation of the module's own index spaces ----------------------------------

    /// <summary>A function names a type index that addresses no declared type.</summary>
    FunctionTypeIndexOutOfRange = 2701,

    /// <summary>The start section names a function index that addresses no function.</summary>
    StartFunctionIndexOutOfRange = 2702,

    /// <summary>The start function takes a parameter or returns a result, and it may do neither.</summary>
    StartFunctionSignatureInvalid = 2703,

    /// <summary>An export names an index that addresses nothing in the space its kind selects.</summary>
    ExportIndexOutOfRange = 2704,

    /// <summary>Two exports publish the same name.</summary>
    DuplicateExportName = 2705,

    /// <summary>An element segment names a table index that addresses no table.</summary>
    ElementSegmentTableIndexOutOfRange = 2706,

    /// <summary>An element segment writes a function index that addresses no function.</summary>
    ElementSegmentFunctionIndexOutOfRange = 2707,

    /// <summary>A data segment names a memory index that addresses no memory.</summary>
    DataSegmentMemoryIndexOutOfRange = 2708,

    /// <summary>A constant expression has a type its position does not admit.</summary>
    ConstantExpressionTypeMismatch = 2709,

    /// <summary>
    /// A constant expression reads a global, and no global it may read has been declared.
    /// </summary>
    /// <remarks>
    /// A constant expression may read an IMPORTED global and no other, because the globals a module
    /// defines are themselves initialised by constant expressions and reading one would be reading a
    /// value that does not exist yet. No feature manifest here admits an import, so every
    /// <c>global.get</c> in a constant expression reaches this code.
    /// </remarks>
    ConstantExpressionGlobalUnavailable = 2710,

    // ---- 2800: validation of one function body ----------------------------------------------

    /// <summary>A byte where an instruction is expected names no instruction this surface admits.</summary>
    UnknownOpcode = 2801,

    /// <summary>An instruction was handed an operand of a type it does not take.</summary>
    OperandTypeMismatch = 2802,

    /// <summary>An instruction wanted an operand the enclosing block does not have.</summary>
    OperandStackUnderflow = 2803,

    /// <summary>A block ends without leaving the operands its result type declares.</summary>
    BlockResultTypeUnmet = 2804,

    /// <summary>A block ends leaving operands above the height it was entered at.</summary>
    BlockLeavesExtraOperands = 2805,

    /// <summary>An <c>else</c> appears where no <c>if</c> is open.</summary>
    ElseWithoutIf = 2806,

    /// <summary>An <c>if</c> declaring a result has no <c>else</c> arm to produce one.</summary>
    IfWithoutElseResultMismatch = 2807,

    /// <summary>A function body ends with a block still open.</summary>
    UnterminatedFunctionBody = 2808,

    /// <summary>A function body carries bytes after the <c>end</c> that closed it.</summary>
    InstructionsAfterFunctionEnd = 2809,

    /// <summary>A branch names a label depth no enclosing block provides.</summary>
    BranchDepthOutOfRange = 2810,

    /// <summary>Two labels of one <c>br_table</c> carry different numbers of values.</summary>
    BranchTableArityMismatch = 2811,

    /// <summary>An instruction names a local index the function does not have.</summary>
    LocalIndexOutOfRange = 2812,

    /// <summary>An instruction names a global index the module does not declare.</summary>
    GlobalIndexOutOfRange = 2813,

    /// <summary>A <c>global.set</c> names a global the module declared immutable.</summary>
    GlobalIsImmutable = 2814,

    /// <summary>A call names a function index that addresses no function.</summary>
    FunctionIndexOutOfRange = 2815,

    /// <summary>An instruction names a type index that addresses no declared type.</summary>
    TypeIndexOutOfRange = 2816,

    /// <summary>A memory instruction appears in a module that declares no memory.</summary>
    MemoryNotDeclared = 2817,

    /// <summary>A <c>call_indirect</c> appears in a module that declares no table.</summary>
    TableNotDeclared = 2818,

    /// <summary>
    /// A load or store declares an alignment above the natural alignment of its access width.
    /// </summary>
    /// <remarks>
    /// It is a VALIDATION rejection and not a runtime one. The immediate is a hint the format
    /// bounds rather than a fact the access depends on, so an over-aligned access is refused before
    /// anything runs rather than trapping when it is first reached.
    /// </remarks>
    AlignmentAboveNaturalAlignment = 2819,

    /// <summary>A block type uses the type-index form, which no manifest here admits.</summary>
    BlockTypeNotAdmitted = 2820,

    /// <summary>An immediate the format reserves as a zero byte carries something else.</summary>
    ReservedImmediateNotZero = 2821,

    /// <summary>
    /// A byte names an instruction a later specification defines and no manifest here admits.
    /// </summary>
    /// <remarks>
    /// It is separate from <see cref="UnknownOpcode"/> for the reason
    /// <see cref="ValueTypeNotAdmitted"/> is separate from <see cref="UnknownValueType"/>: a module
    /// compiled for the bulk-memory, vector, atomic, reference or sign-extension surfaces is a
    /// well-formed module this build does not admit, and telling its author it emitted rubbish would
    /// be false.
    /// </remarks>
    OpcodeNotAdmitted = 2822,

    // ---- 2900: reserved for a defect in this assembly ---------------------------------------

    /// <summary>
    /// Something in this assembly threw. It says nothing about the artifact.
    /// </summary>
    /// <remarks>
    /// It is emitted by the one catch that makes this verifier total, and it exists so that a
    /// defect here becomes a deterministic refusal instead of an exception escaping into a caller
    /// that has no way to tell it from a malicious module. A retained corpus entry recording this
    /// code is a bug report, never an expected answer.
    /// </remarks>
    VerifierDefect = 2901,

    /// <summary>
    /// The bounded reader stopped with a status this profile's mapping does not name.
    /// </summary>
    /// <remarks>
    /// It is in this band rather than in the framing band because it cannot be reached by any
    /// artifact: the mapping covers every status the core's reader defines, so arriving here means
    /// the core grew a status this profile has not been taught. Like the code above it, an entry
    /// recording it is a bug report rather than an expected answer.
    /// </remarks>
    ReaderStopped = 2902,

    // ---- 3000: execution, where the code travels in the payload rather than in the diagnostics --

    /// <summary>The <c>unreachable</c> instruction was executed.</summary>
    TrapUnreachable = 3001,

    /// <summary>An integer division or remainder had a zero divisor.</summary>
    TrapIntegerDivideByZero = 3002,

    /// <summary>A signed division overflowed, or a conversion was out of range.</summary>
    TrapIntegerOverflow = 3003,

    /// <summary>A float-to-integer conversion was handed a NaN.</summary>
    TrapInvalidConversionToInteger = 3004,

    /// <summary>A load or store addressed bytes outside the memory.</summary>
    TrapOutOfBoundsMemoryAccess = 3005,

    /// <summary>A table access addressed an entry outside the table.</summary>
    TrapOutOfBoundsTableAccess = 3006,

    /// <summary>The row the earlier specification revision's name for that access would carry.</summary>
    TrapUndefinedElement = 3007,

    /// <summary>An indirect call reached a function of the wrong signature.</summary>
    TrapIndirectCallTypeMismatch = 3008,

    /// <summary>An indirect call reached a null table entry.</summary>
    TrapUninitializedElement = 3009,
}

/// <summary>
/// The one mapping from a mechanism's latched status onto the answers a verifier may give.
/// </summary>
/// <remarks>
/// <para>
/// <b>IT IS ONE MAPPING BECAUSE TWO WOULD DISAGREE.</b> Both passes of this verifier read bytes
/// through the core's bounded reader and both read this profile's own variable-length integers, so
/// both have to decide, for every way those can stop, whether the answer is an invalid artifact
/// carrying a diagnostic code or a resource exhaustion naming a dimension and a scope. That split
/// is the one a retained corpus entry pins for ever, so a second copy of it - written months apart,
/// in a file whose author was thinking about something else - is how a decoder and a validator come
/// to give different answers for the same latched status.
/// </para>
/// <para>
/// The position arrives as a parameter rather than being computed here because where a failure IS
/// belongs to the pass that found it: the decoder locates it by section ordinal and vector index,
/// and the validator by section identifier and function index. What is shared is the category, and
/// only the category.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0005; IP=Low; Security=High; Resources=1; Fingerprint=686934
// Broiler-Falsified-If: a ceiling breach is mapped onto an invalid artifact, or a framing failure onto a resource exhaustion
// Broiler-Human:        PENDING
internal static class WasmRefusal
{
    /// <summary>An invalid-artifact answer carrying this profile's code and a position.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=653767
    // Broiler-Human:        PENDING
    internal static VmVerifierOutcome Invalid(
        VmReason reason, WebAssemblyDiagnosticCode code, VmSourcePosition position) =>
        VmVerifierOutcome.InvalidArtifact(reason, (int)code, position);

    /// <summary>Maps the bounded reader's mechanism status onto a verifier's answer.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0005; IP=Low; Security=High; Resources=1; Fingerprint=A30C39
    // Broiler-Falsified-If: a ceiling breach is mapped onto an invalid artifact, or a framing failure onto a resource exhaustion
    // Broiler-Human:        PENDING
    internal static VmVerifierOutcome FromReader(
        VmBoundedReadStatus status, VmSourcePosition position) => status switch
    {
        VmBoundedReadStatus.Truncated =>
            Invalid(VmReason.Truncated, WebAssemblyDiagnosticCode.Truncated, position),

        VmBoundedReadStatus.MalformedEncoding =>
            Invalid(
                VmReason.MalformedEncoding,
                WebAssemblyDiagnosticCode.ReaderMalformedEncoding,
                position),

        VmBoundedReadStatus.DeclaredCountExceeded =>
            VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact),

        VmBoundedReadStatus.SectionCountExceeded =>
            VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.SectionCount, VmBudgetScope.Artifact),

        VmBoundedReadStatus.StructuralDepthExceeded =>
            VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.StructuralDepth, VmBudgetScope.Artifact),

        VmBoundedReadStatus.ArtifactBytesExceeded =>
            VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.ArtifactBytes, VmBudgetScope.Artifact),

        VmBoundedReadStatus.AllocationRefused =>
            VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact),

        VmBoundedReadStatus.WorkBudgetExhausted =>
            VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact),

        _ => Invalid(
            VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.ReaderStopped, position),
    };

    /// <summary>Maps a variable-length integer refusal onto a verifier's answer.</summary>
    /// <remarks>
    /// The three malformation arms are separate codes because the specification's own suite
    /// separates them, and a reader collapsing them would record one answer for three fixtures.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=F268C0
    // Broiler-Falsified-If: two of the three integer malformations report the same diagnostic code
    // Broiler-Human:        PENDING
    internal static VmVerifierOutcome FromVarInt(
        WasmVarIntStatus status,
        VmBoundedReadStatus readerStatus,
        VmSourcePosition position) => status switch
    {
        WasmVarIntStatus.ByteBudgetExceeded => Invalid(
            VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.IntegerRepresentationTooLong,
            position),

        WasmVarIntStatus.UnusedBitsSet => Invalid(
            VmReason.MalformedEncoding, WebAssemblyDiagnosticCode.IntegerTooLarge, position),

        WasmVarIntStatus.SignExtensionInvalid => Invalid(
            VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.IntegerSignExtensionInvalid,
            position),

        WasmVarIntStatus.DeclaredCountExhausted => VmVerifierOutcome.ResourceExhaustion(
            VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact),

        _ => FromReader(readerStatus, position),
    };
}
