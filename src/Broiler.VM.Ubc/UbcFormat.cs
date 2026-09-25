// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           16
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Ubc;

/// <summary>
/// The fixed facts of the universal bytecode container: its magic, its format version, the identity
/// of the bytecode form, and the structural ceilings the format itself imposes before any budget does.
/// </summary>
/// <remarks>
/// <para>
/// The container is one byte string: a header, then framed sections in strictly ascending kind order
/// (<see cref="UbcSectionKind"/>). Every integer is a canonical variable-length integer read through
/// <c>Broiler.VM.Binary</c>'s bounded reader unless a field says fixed width; the layout of every
/// field is Appendix E of <c>docs/universal-bytecode.md</c>, restated where it is read in
/// <see cref="UbcArtifactReader"/>.
/// </para>
/// <para>
/// <b>The format version is the universal bytecode's own</b>, distinct from every family table
/// version, every emitter version and the core contract version. It moves only when the container,
/// the common family, the slot types, the operand shapes or the table schema change.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=B7B3FA
// Broiler-Human:        PENDING
public static class UbcFormat
{
    /// <summary>The first four bytes of every artifact: <c>"BUBC"</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=001202
    // Broiler-Human:        PENDING
    public static System.ReadOnlySpan<byte> Magic => "BUBC"u8;

    /// <summary>The one format version this build defines.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F181CE
    // Broiler-Human:        PENDING
    public const uint FormatVersion = 1;

    /// <summary>The form identity of an artifact that carries no emission: the bytecode form.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=8DD8B2
    // Broiler-Human:        PENDING
    public const string BytecodeForm = "bytecode";

    /// <summary>The highest family slot an artifact may declare; slot zero is the common family.</summary>
    /// <remarks>
    /// Fourteen, not the fifteen the concept's Appendix E counts. A family instruction of slot
    /// <c>s</c> is introduced by the byte <c>0xF0 + s</c>, and <c>0xF0 + 15</c> is <c>0xFF</c>, the
    /// reserved extended prefix, so a fifteenth slot could be declared and never used. Format
    /// version 1 admits slots 1 to 14 and refuses the rest.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F2E50F
    // Broiler-Human:        PENDING
    public const byte MaxFamilySlot = 14;

    /// <summary>The opcode byte that introduces an instruction of family slot one.</summary>
    /// <remarks>
    /// <c>0xF0 + s</c> introduces one instruction of family slot <c>s</c>. The byte <c>0xF0</c> itself
    /// would name slot zero, which is the common family and is never prefixed, so it is refused.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=908188
    // Broiler-Human:        PENDING
    public const byte FamilyPrefixBase = 0xF0;

    /// <summary>The reserved extended prefix. Format version 1 does not define it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8C4E35
    // Broiler-Human:        PENDING
    public const byte ExtendedPrefix = 0xFF;

    /// <summary>The longest identity string the header or a family row may carry, in bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9D1F7C
    // Broiler-Human:        PENDING
    public const int MaxIdentityBytes = 128;

    /// <summary>The longest entry-point name an Entries row may carry, in bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=081F6C
    // Broiler-Human:        PENDING
    public const int MaxEntryNameBytes = 1024;

    /// <summary>
    /// The most locals a unit may declare, parameters included. A local is named by a sixteen-bit
    /// operand, so a larger table could name locals no instruction can reach.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=29844D
    // Broiler-Human:        PENDING
    public const int MaxLocals = 65536;

    /// <summary>The highest operand height either plane of a unit may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0CBF1E
    // Broiler-Human:        PENDING
    public const int MaxHeight = 65535;

    /// <summary>The most jump tables an artifact may carry: a table is named by a sixteen-bit operand.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7B17D9
    // Broiler-Human:        PENDING
    public const int MaxJumpTables = 65536;

    /// <summary>The largest emission alignment an Emission section may state, in bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8BBA05
    // Broiler-Human:        PENDING
    public const uint MaxAlignment = 4096;

    /// <summary>The section kind of the FamilyData section of family slot <paramref name="slot"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=829B13
    // Broiler-Human:        PENDING
    public static byte FamilyDataKind(byte slot) => (byte)(UbcSectionKind.FamilyDataFirst + slot - 1);
}

/// <summary>The contract version of the universal bytecode, which the core never sees.</summary>
/// <remarks>
/// A family registration and an emitter each state the version they were built against, and
/// <see cref="UbcDescriptors"/> refuses to build a descriptor for one that differs from this
/// assembly's. The version is minted by a dated record in the core's ADR set (the programme's route
/// UBC-R9, at milestone UBC-9); until then it is 1 and nothing outside this repository depends on it.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=9349A5
// Broiler-Human:        PENDING
public static class UbcContract
{
    /// <summary>The universal bytecode contract version this assembly implements.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A5EC68
    // Broiler-Human:        PENDING
    public const int Version = 1;
}

/// <summary>The kinds of section a container may carry, in the order they must appear.</summary>
/// <remarks>
/// Kinds are strictly ascending and unique. A kind this enumeration does not name is refused: there
/// are no custom sections, and a family that needs one has its FamilyData section.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9C4F0C
// Broiler-Human:        PENDING
public enum UbcSectionKind : byte
{
    /// <summary>The families the code uses, one row per slot.</summary>
    Families = 1,

    /// <summary>Signatures: parameter and result slot types.</summary>
    Types = 2,

    /// <summary>One row per code unit.</summary>
    Units = 3,

    /// <summary>Every unit's instructions, back to back.</summary>
    Code = 4,

    /// <summary>The target rows of <c>jump_table</c>.</summary>
    JumpTables = 5,

    /// <summary>Handler regions.</summary>
    Regions = 6,

    /// <summary>Named entry points.</summary>
    Entries = 7,

    /// <summary>The diagnostic position table.</summary>
    Positions = 8,

    /// <summary>The first FamilyData section, belonging to family slot one.</summary>
    FamilyDataFirst = 9,

    /// <summary>The last FamilyData section, belonging to family slot fourteen.</summary>
    FamilyDataLast = 22,

    /// <summary>The emitted bytes of a native form. Absent in the bytecode form.</summary>
    Emission = 24,
}

/// <summary>The flags a Units row may carry.</summary>
/// <remarks>
/// Bits 2 to 7 are reserved and refused when set. Bits 8 to 15 belong to the unit's family, and are
/// refused on a unit that declares no family.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B3C071
// Broiler-Human:        PENDING
[System.Flags]
public enum UbcUnitFlags : ushort
{
    /// <summary>No flag.</summary>
    None = 0,

    /// <summary>The unit may execute a family instruction of the suspending kind.</summary>
    Suspendable = 1,

    /// <summary>The unit may be named by an Entries row.</summary>
    Entry = 2,

    /// <summary>The mask of the family-defined bits.</summary>
    FamilyDefined = 0xFF00,
}
