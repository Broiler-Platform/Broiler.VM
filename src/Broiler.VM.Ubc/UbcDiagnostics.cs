// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   1
// Annotated:        1/1
// Exempt:           56
// Human-reviewed:   0/1
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  0/10 max
// Unverified:       1
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Ubc;

/// <summary>
/// The universal bytecode's own stable diagnostic codes: every refusal the container's framing, the
/// walk and the form layer's structural checks give. The core attaches no meaning to any of them.
/// </summary>
/// <remarks>
/// <para>
/// <b>The published registry is <c>docs/ubc/diagnostics/registry.txt</c></b>, and rule U8 holds it to
/// this vocabulary in both directions: every member has exactly one row carrying its name, its number
/// and the one core reason its every emission carries, and every row is reachable from a named entry
/// of the retained corpus <c>src/tests/corpus/ubc-1</c> or is one of the defensive rows the rule
/// itself lists.
/// </para>
/// <para>
/// The numbers are the 3000 range, grouped by the stage that refuses, so that a reader can tell from
/// the code alone which pass refused an artifact. A family's hook answers with codes of its own
/// registry, outside this range; the JavaScript and WebAssembly profiles' registries use the 1000 and
/// 2000 ranges. A code is never reused for another meaning.
/// </para>
/// <para>
/// It is an enumeration rather than a class of constants for the reason the profiles' registries give:
/// a registry is a closed vocabulary, and the assurance system treats a vocabulary as one reviewable
/// thing.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=4503D5
// Broiler-Falsified-If: a member is emitted with two different core reasons, or a refusal of the walk carries no member of this vocabulary
// Broiler-Human:        PENDING
public enum UbcDiagnosticCode
{
    // ---- 3000: the header, before any section is framed -----------------------------------

    /// <summary>The first four bytes are not <c>"BUBC"</c>.</summary>
    WrongMagic = 3001,

    /// <summary>The payload's format version disagrees with the artifact descriptor's.</summary>
    DescriptorFormatVersionMismatch = 3002,

    /// <summary>The format version is not one this build defines.</summary>
    UnsupportedFormatVersion = 3003,

    /// <summary>An identity field is empty, longer than the format admits, or holds a byte its grammar refuses.</summary>
    MalformedIdentity = 3004,

    /// <summary>The header's profile identity is not the descriptor's.</summary>
    DescriptorProfileMismatch = 3005,

    /// <summary>The header's feature manifest is not the artifact descriptor's.</summary>
    DescriptorManifestMismatch = 3006,

    /// <summary>The header's feature manifest names no table of the composed family.</summary>
    ManifestNotComposed = 3007,

    /// <summary>The header's form is not one this image composes.</summary>
    FormNotComposed = 3008,

    // ---- 3100: section framing ------------------------------------------------------------

    /// <summary>A section kind format version 1 does not define.</summary>
    UnknownSectionKind = 3101,

    /// <summary>Sections are out of order, or one kind appears twice.</summary>
    SectionOrder = 3102,

    /// <summary>A required section is absent.</summary>
    MissingSection = 3103,

    /// <summary>Bytes remain after the last declared section.</summary>
    TrailingBytes = 3104,

    /// <summary>A section body did not consume exactly its declared length.</summary>
    SectionLengthMismatch = 3105,

    // ---- 3200: the Families section and FamilyData -----------------------------------------

    /// <summary>A family slot is zero or above the highest slot, or the slots are not strictly ascending.</summary>
    FamilySlotInvalid = 3201,

    /// <summary>A Families row names a family this image does not compose.</summary>
    FamilyNotComposed = 3202,

    /// <summary>A family's table version is not the version of the table its manifest selects.</summary>
    FamilyTableVersionMismatch = 3203,

    /// <summary>A family's selecting manifest is not the header's.</summary>
    FamilyManifestMismatch = 3204,

    /// <summary>A FamilyData section belongs to a slot the Families section does not declare.</summary>
    FamilyDataForUndeclaredSlot = 3205,

    /// <summary>The Families section declares a family more than once; an artifact has at most one family.</summary>
    DuplicateFamily = 3206,

    // ---- 3300: Types and Units -------------------------------------------------------------

    /// <summary>A slot-type byte format version 1 does not define.</summary>
    UnknownSlotType = 3301,

    /// <summary>A unit names a signature the Types section does not have.</summary>
    TypeIndexOutOfRange = 3302,

    /// <summary>A unit names a family slot the Families section does not declare.</summary>
    UnitFamilyUndeclared = 3303,

    /// <summary>A unit sets a reserved flag bit, or a family-defined bit without a family.</summary>
    ReservedUnitFlag = 3304,

    /// <summary>The units do not tile the Code section in order with no gap, or a unit is empty.</summary>
    CodeNotTiled = 3305,

    /// <summary>A unit declares more locals, or a higher operand height, than the format admits.</summary>
    UnitDeclarationTooLarge = 3306,

    /// <summary>A unit's landings are not strictly ascending, or not exactly its resume points and handlers.</summary>
    LandingsInvalid = 3307,

    // ---- 3400: the code walk ---------------------------------------------------------------

    /// <summary>An unprefixed byte that names no common instruction, or a family opcode the selected table does not define.</summary>
    UnknownOpcode = 3401,

    /// <summary>The prefix byte <c>0xF0</c>, which would name the unprefixed common family, or the reserved extended prefix <c>0xFF</c>.</summary>
    ReservedPrefix = 3402,

    /// <summary>A family prefix for a slot other than the unit's own.</summary>
    ForeignFamilyInstruction = 3403,

    /// <summary>An instruction's operand runs past the unit's end.</summary>
    TruncatedInstruction = 3404,

    /// <summary>A code target, handler, landing or position is not an instruction boundary of its unit.</summary>
    NotAnInstructionBoundary = 3405,

    /// <summary>An instruction would pop more slots than the stack holds.</summary>
    StackUnderflow = 3406,

    /// <summary>An instruction's operand types are not the ones its effect requires.</summary>
    StackTypeMismatch = 3407,

    /// <summary>Two paths reach one instruction with different typed stacks.</summary>
    JoinMismatch = 3408,

    /// <summary>A plane's operand height exceeds the unit's declared maximum.</summary>
    HeightAboveDeclared = 3409,

    /// <summary>A path runs past the unit's last instruction.</summary>
    FallsOffTheEnd = 3410,

    /// <summary>An instruction no path reaches.</summary>
    UnreachableInstruction = 3411,

    /// <summary>A local index outside the unit's local table.</summary>
    LocalIndexOutOfRange = 3412,

    /// <summary>A <c>call</c> names a unit the artifact does not have.</summary>
    CallTargetOutOfRange = 3413,

    /// <summary>A <c>jump_table</c> names a table the artifact does not have, or one of another unit.</summary>
    JumpTableInvalid = 3414,

    /// <summary>A suspending family row in a unit not flagged suspendable.</summary>
    SuspendOutsideSuspendableUnit = 3415,

    /// <summary>A <c>trap</c> names a slot with no family, or a code the family's trap vocabulary does not define.</summary>
    TrapUndefined = 3416,

    /// <summary>A counted operand larger than any stack could supply.</summary>
    CountedOperandTooLarge = 3417,

    // ---- 3500: regions ---------------------------------------------------------------------

    /// <summary>A region names a unit the artifact does not have, or its range is empty or outside the unit.</summary>
    RegionRangeInvalid = 3501,

    /// <summary>Regions of one unit are not in nesting order, or overlap without nesting.</summary>
    RegionNesting = 3502,

    /// <summary>A region's entry heights are not a prefix the stack holds at every covered instruction.</summary>
    RegionEntryInconsistent = 3503,

    /// <summary>A region names a kind its unit's family does not define, or its unit has no family.</summary>
    RegionKindUndefined = 3504,

    // ---- 3600: jump tables, entries and positions -------------------------------------------

    /// <summary>A jump table names a unit the artifact does not have, or has no target.</summary>
    JumpTableMalformed = 3601,

    /// <summary>Two entries share one name, or a name is not valid UTF-8 or longer than the format admits.</summary>
    EntryNameInvalid = 3602,

    /// <summary>An entry names a unit the artifact does not have, or one not flagged as an entry.</summary>
    EntryUnitInvalid = 3603,

    /// <summary>A position names a unit the artifact does not have, or a coordinate outside the core's range.</summary>
    PositionInvalid = 3604,

    // ---- 3700: the form layer --------------------------------------------------------------
    //
    // One code at this contract version. An image composes the bytecode form alone, so the only form
    // check that can refuse is an Emission section where the bytecode form admits none; the checks of
    // emitted code a native form would need are the mechanism ADR 0013 did not admit, and they get
    // codes in the milestone that admits them.

    /// <summary>The bytecode form carries an Emission section.</summary>
    EmissionUnexpected = 3701,

    // ---- 3900: the bounded reader and the verifier itself -----------------------------------

    /// <summary>A read ran past the end of the payload.</summary>
    Truncated = 3901,

    /// <summary>A variable-length integer is not canonical, or a section's framing is inconsistent.</summary>
    MalformedEncoding = 3902,

    /// <summary>The bounded reader stopped for a status this build has no arm for. Defensive.</summary>
    ReaderStopped = 3903,

    /// <summary>
    /// The walk or the family's hook broke its own contract - a hook answering with a code of the
    /// universal range, or with no invalid-artifact reason - reported rather than thrown.
    /// </summary>
    VerifierDefect = 3904,
}
