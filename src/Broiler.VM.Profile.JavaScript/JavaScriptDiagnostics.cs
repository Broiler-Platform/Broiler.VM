// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           62
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  1/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// This profile's own stable diagnostic codes. The core attaches no meaning to any of them.
/// </summary>
/// <remarks>
/// <para>
/// <b>The published registry is <c>docs/diagnostics/registry.txt</c></b> and decision JSD-0009
/// owns it. Every member below has a row there; every row there names a member here; each row
/// names the one core reason its emissions carry and the half of the registry it belongs to,
/// because a code carried by a rejection of ARTIFACT BYTES travels in a core result and a code
/// carried by a rejection of SOURCE never does. Every member below is in the core-result half,
/// because at this manifest there is no source rejection at all - the lowering is hand-written
/// and JS-3b is the milestone that mints the first embedder-seam code. Rules N5 through N8 hold
/// the two halves to the registry; nothing in this file is the authority on its own.
/// </para>
/// <para>
/// The numbers are grouped by the stage that emits them so that a reader can tell from the code
/// alone which pass refused an artifact. A code is never reused for a different meaning; a
/// rejection that changes meaning takes a new number and the old one is retired, because a corpus
/// entry that recorded a code has dated it.
/// </para>
/// <para>
/// <b>It is an enum rather than a class of constants, and that is a review decision.</b> A
/// registry is a closed vocabulary, and the assurance system's exemption predicate treats a
/// vocabulary as one reviewable thing: the declaration carries the assessment and every member is
/// covered by its fingerprint. A class of forty-five <c>const int</c> fields would instead be
/// forty-five separately assessed fixed values, each demanding its own two-line block, which the
/// predicate's own record calls a worse record than one block on the vocabulary.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=4E0E55
// Broiler-Human:        PENDING
public enum JavaScriptDiagnosticCode
{
    // ---- 1000: the header, before any section is framed ----------------------------------

    /// <summary>The first four bytes are not this format's magic.</summary>
    WrongMagic = 1001,

    /// <summary>The format version is outside the range this build defines.</summary>
    UnsupportedFormatVersion = 1002,

    /// <summary>The artifact descriptor's format version disagrees with the payload's.</summary>
    DescriptorFormatVersionMismatch = 1003,

    /// <summary>The feature-manifest identity in the payload is not one this descriptor accepts.</summary>
    UnsupportedFeatureManifest = 1004,

    /// <summary>The feature-manifest identity is longer than this format admits.</summary>
    ManifestIdTooLong = 1005,

    /// <summary>The artifact descriptor's manifest disagrees with the payload's.</summary>
    DescriptorManifestMismatch = 1006,

    // ---- 1100: section framing ------------------------------------------------------------

    /// <summary>A section kind this format version does not define.</summary>
    UnknownSectionKind = 1101,

    /// <summary>Sections are out of order, or one kind appears twice.</summary>
    SectionOrder = 1102,

    /// <summary>A required section is absent.</summary>
    MissingSection = 1103,

    /// <summary>Bytes remain after the last declared section.</summary>
    TrailingBytes = 1104,

    /// <summary>A section body did not consume exactly its declared length.</summary>
    SectionLengthMismatch = 1105,

    // ---- 1200: the limits section ---------------------------------------------------------

    /// <summary>A declared maximum is above this format's own structural ceiling for it.</summary>
    DeclaredMaximumTooLarge = 1201,

    /// <summary>The artifact declares a frame count the slice surface does not have.</summary>
    DeclaredFrameCount = 1202,

    // ---- 1300: the constant pool ----------------------------------------------------------

    /// <summary>A constant-pool tag this format version does not define.</summary>
    UnknownConstantTag = 1301,

    /// <summary>A Boolean constant's payload byte is neither 0 nor 1.</summary>
    MalformedBooleanConstant = 1302,

    /// <summary>The pool declares more constants than the limits section admits.</summary>
    ConstantCountExceedsDeclaredMaximum = 1303,

    /// <summary>
    /// The pool carries an interned name, which this format reserves and no manifest admits.
    /// </summary>
    InternedNameOutsideManifest = 1304,

    // ---- 1400: the code section -----------------------------------------------------------

    /// <summary>A byte in the code section is not an opcode this format version defines.</summary>
    UnknownOpcode = 1401,

    /// <summary>An instruction's operand runs past the end of the code section.</summary>
    TruncatedInstruction = 1402,

    /// <summary>A jump displacement lands outside the code, or not on an instruction boundary.</summary>
    JumpTargetNotAnInstructionBoundary = 1403,

    /// <summary>Two paths reach one offset with different operand-stack heights.</summary>
    InconsistentStackHeightAtJoin = 1404,

    /// <summary>An instruction would pop from an empty operand stack.</summary>
    OperandStackUnderflow = 1405,

    /// <summary>The operand stack would grow past the declared maximum.</summary>
    OperandStackOverflow = 1406,

    /// <summary>A constant index addresses no pool entry.</summary>
    ConstantIndexOutOfRange = 1407,

    /// <summary>A local slot index is above the declared local count.</summary>
    LocalIndexOutOfRange = 1408,

    /// <summary>Control reaches the end of the code without a return.</summary>
    FallsOffTheEnd = 1409,

    /// <summary>The code section is empty.</summary>
    EmptyCode = 1410,

    /// <summary>
    /// An instruction is reachable from no entry point.
    /// </summary>
    /// <remarks>
    /// Unreachable code is refused rather than left unchecked. Tolerating it would leave bytes in
    /// a verified artifact that no check ever looked at, and "unreachable" would then be a claim
    /// about the verifier's own traversal rather than about the program.
    /// </remarks>
    UnreachableCode = 1411,

    /// <summary>A return would leave values on the operand stack besides the completion value.</summary>
    ReturnStackNotExactlyOne = 1412,

    // ---- 1500: entries, positions, and the reserved sections -------------------------------

    /// <summary>The artifact declares no entry point.</summary>
    NoEntryPoint = 1501,

    /// <summary>Two entry points carry the same name.</summary>
    DuplicateEntryPoint = 1502,

    /// <summary>An entry point's code offset is not an instruction boundary.</summary>
    EntryOffsetNotAnInstructionBoundary = 1503,

    /// <summary>An entry point's name is longer than this format admits, or is not valid UTF-8.</summary>
    MalformedEntryName = 1504,

    /// <summary>An entry point is reached with a non-empty operand stack.</summary>
    EntryStackNotEmpty = 1505,

    /// <summary>A position row's offset is not an instruction boundary, or rows are not ascending.</summary>
    MalformedPositionRow = 1506,

    /// <summary>
    /// The artifact declares an exception region, which this format reserves and no manifest
    /// admits.
    /// </summary>
    ExceptionRegionOutsideManifest = 1507,

    /// <summary>
    /// The artifact declares a suspension target, which this format reserves and no manifest
    /// admits.
    /// </summary>
    SuspensionTargetOutsideManifest = 1508,

    // ---- 1600: format version 2, which frames what version 1 declares and refuses -----------

    /// <summary>A code-unit row states a figure this format cannot represent.</summary>
    MalformedFunctionRow = 1601,

    /// <summary>An index names a code unit the functions section does not declare.</summary>
    FunctionIndexOutOfRange = 1602,

    /// <summary>
    /// A code unit's declared range is not the next disjoint run of the code section.
    /// </summary>
    /// <remarks>
    /// Overlapping ranges are what makes a per-unit branch check meaningless, so the ranges are
    /// required to tile the section exactly rather than merely to fit inside it.
    /// </remarks>
    CodeUnitRangeInvalid = 1603,

    /// <summary>A scope depth is past what the encoding or the structure allows.</summary>
    ScopeDepthOutOfRange = 1604,

    /// <summary>An exception region states a range, a handler or a kind this format refuses.</summary>
    MalformedExceptionRegion = 1605,

    /// <summary>The artifact declares one optional surface twice.</summary>
    DuplicateSurface = 1606,

    /// <summary>The artifact declares an optional surface this build does not implement.</summary>
    /// <remarks>
    /// It is a different failure from <see cref="SurfaceOutsideComposition"/> and the difference is
    /// who is wrong: this one says nobody wrote the surface, that one says somebody declined it.
    /// </remarks>
    UnknownSurface = 1607,

    /// <summary>
    /// The artifact declares an optional surface this composition did not admit.
    /// </summary>
    /// <remarks>
    /// This is the manifest boundary as a policy boundary, refused at verification, which roadmap
    /// section 6 distinguishes by name from the run-time refusal a composition that admits a
    /// surface and registers no provider produces.
    /// </remarks>
    SurfaceOutsideComposition = 1608,

    /// <summary>
    /// A suspension instruction appears in a code unit that is not a generator body.
    /// </summary>
    /// <remarks>
    /// The executor gives a generator invocation a heap-allocated frame and an ordinary one none,
    /// so a <c>Yield</c> anywhere else would suspend a frame that does not exist. It is refused
    /// here rather than answered there, because "the frame is null" is not a diagnosis a payload
    /// author can act on.
    /// </remarks>
    YieldOutsideGenerator = 1609,

    /// <summary>
    /// A code-unit row combines the generator flag with a flag that contradicts it.
    /// </summary>
    /// <remarks>
    /// A generator is neither an arrow, nor the program body, nor a constructor. Each pairing would
    /// send the executor down a path the other flag already claimed - an arrow's <c>this</c>, the
    /// program body's entry, or <c>new</c> - and none of them has an answer for a unit that
    /// suspends.
    /// </remarks>
    GeneratorFlagsInconsistent = 1610,

    /// <summary>
    /// An <c>Await</c> instruction appears in a code unit that is not an async function body.
    /// </summary>
    /// <remarks>
    /// It is a code of its own rather than a second use of
    /// <see cref="YieldOutsideGenerator"/>, because the two name different missing FLAGS and a
    /// payload author acts on the flag. An artifact that awaits in a generator body is not an
    /// artifact that yields outside one, and telling it the latter would send its author to
    /// exactly the wrong bit.
    /// </remarks>
    AwaitOutsideAsync = 1611,

    /// <summary>
    /// A code-unit row combines the async flag with a flag that contradicts it.
    /// </summary>
    /// <remarks>
    /// An async function is neither the program body, nor a constructor, nor a generator. The
    /// third is the one worth stating: this profile admits no async generator, and a unit claiming
    /// both bits would be asking the executor to pick a driver - suspended-start with a caller
    /// pulling it, or running-start with the job queue pushing it - where the format offers no way
    /// to say which. An async ARROW is not on the list, because an arrow with a suspendable body
    /// is exactly what <c>async () =&gt; { await x; }</c> is.
    /// </remarks>
    AsyncFlagsInconsistent = 1612,

    /// <summary>
    /// A <c>DefineClassElement</c> operand carries a bit set the instruction has no reading for.
    /// </summary>
    /// <remarks>
    /// <b>It is a code of its own rather than a second use of <see cref="UnknownOpcode"/></b>,
    /// because every bit in the operand IS defined by this format version: what is wrong is the
    /// combination, and an author told "unknown feature" would go looking for a version of the
    /// format that has one. A static block that is not static, a getter that is also a setter, and
    /// a public element on an instruction that only records private ones are the three shapes it
    /// answers for, and each is an encoding the executor would otherwise have to pick an arm for by
    /// precedence.
    /// </remarks>
    ClassElementFlagsInconsistent = 1621,

    // ---- 1613: the module goal ---------------------------------------------------------------
    //
    // Out of numeric order beside the block above, and deliberately: these were minted while 1621
    // was, and packing them after it would have put a module refusal in the middle of the
    // structural block it has nothing to do with. Every code here is about a relation BETWEEN
    // modules - what a request names, what an export resolves to, whether the composition admits
    // the surface at all - and none of them can be stated about one row.

    /// <summary>
    /// The artifact carries module records and declares no module surface beside its manifest.
    /// </summary>
    ModuleSectionOutsideManifest = 1613,

    /// <summary>A module record states an index, a count or a slot this format refuses.</summary>
    MalformedModuleRow = 1614,

    /// <summary>A module requests a key no module of this artifact carries.</summary>
    /// <remarks>
    /// <b>Resolution happened before the bytes were written and this is the check that it was
    /// complete.</b> The composition turns a specifier into a key and supplies the module under
    /// that key; an artifact whose request matches no key is one whose producer resolved a
    /// specifier and then did not carry what it resolved to, which no amount of executing would
    /// discover any earlier.
    /// </remarks>
    ModuleRequestUnresolved = 1615,

    /// <summary>An import or a re-export names an export the exporting module does not have.</summary>
    ModuleExportNotFound = 1616,

    /// <summary>Two star re-exports supply the same name from different bindings.</summary>
    ModuleExportAmbiguous = 1617,

    /// <summary>
    /// Resolving an export re-entered the module and name it started from: a cyclic import.
    /// </summary>
    /// <remarks>
    /// <b>This is what a cyclic import costs, and it costs a named refusal rather than a budget.</b>
    /// A cycle in the module GRAPH is ordinary and runs; a cycle in an export RESOLUTION -
    /// <c>a</c> re-exporting a name from <c>b</c> while <c>b</c> re-exports it from <c>a</c> -
    /// names a binding that exists nowhere, and a resolver that followed it would walk the cycle
    /// until an allowance ran out. The walk carries the pairs it has visited and refuses on
    /// re-entry, so the answer is this code and never an exhaustion.
    /// </remarks>
    ModuleExportCircular = 1618,

    /// <summary>
    /// The artifact declares the module surface and the composition registered no resolver.
    /// </summary>
    /// <remarks>
    /// <b>Admitting the surface and answering its one question are two different acts, and this is
    /// the second.</b> A composition declines the surface by not admitting it, and is told so with
    /// <see cref="SurfaceOutsideComposition"/>; a composition that admits it and registers no
    /// resolver has said it will run modules and supplied no way to say what a specifier names.
    /// Both are refused at verification, and a reader holding one of the two codes knows which of
    /// the two things is missing.
    /// </remarks>
    ModuleResolverAbsent = 1619,

    /// <summary>The artifact declares the module surface and carries no module records.</summary>
    ModuleSectionMissing = 1620,

    // ---- 1900: the bounded reader's own statuses, mapped -----------------------------------

    /// <summary>The payload ended inside a value the reader was part-way through.</summary>
    Truncated = 1901,

    /// <summary>A variable-length integer is malformed or overlong.</summary>
    MalformedEncoding = 1902,

    /// <summary>The reader stopped for a status this profile has no more specific answer for.</summary>
    ReaderStopped = 1903,
}

/// <summary>
/// The projection between the contract's metering surface and the bounded reader's, and the one
/// between an effective limit vector and the four artifact-shaped ceilings.
/// </summary>
/// <remarks>
/// <c>Broiler.VM.Binary</c> names no contract vocabulary, so the party holding both performs the
/// projection. Writing it is this profile's work and not the core's, which is the whole of what
/// the seventh core-facing type amounts to.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1DD7A4
// Broiler-Falsified-If: a charge made through this adapter reaches a dimension other than the one named, or a released byte count is charged rather than released
// Broiler-Human:        PENDING
public sealed class JavaScriptReadAdapter : IVmBoundedAllocationMeter
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D9AC66
    // Broiler-Human:        PENDING
    private readonly IVmMeter meter;

    /// <summary>Wraps the contract meter the core supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9141C7
    // Broiler-Human:        PENDING
    public JavaScriptReadAdapter(IVmMeter contractMeter) => meter = contractMeter;

    /// <summary>Projects the four artifact-shaped ceilings out of an effective limit vector.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8F35EC
    // Broiler-Human:        PENDING
    public static VmReadBounds ToReadBounds(VmLimitVector limits) =>
        new(
            limits[VmBudgetDimension.ArtifactBytes],
            limits[VmBudgetDimension.SectionCount],
            limits[VmBudgetDimension.DeclaredCount],
            limits[VmBudgetDimension.StructuralDepth]);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FFFA26
    // Broiler-Human:        PENDING
    public bool TryReserve(ulong byteCount) =>
        meter.TryCharge(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E74062
    // Broiler-Human:        PENDING
    public void Release(ulong byteCount) =>
        meter.ReportReleased(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4E1E45
    // Broiler-Human:        PENDING
    public bool TryChargeWork(ulong workUnits) =>
        meter.TryCharge(VmBudgetDimension.VerifierWork, workUnits);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=06A1AC
    // Broiler-Human:        PENDING
    public bool Poll() => meter.Poll();
}
