// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   20
// Annotated:        20/20
// Exempt:           78
// Human-reviewed:   0/20
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       20
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Immutable;

namespace Broiler.VM.Ubc;

/// <summary>
/// A container, decoded: every section the universal bytecode owns, as rows, and every FamilyData
/// section as the opaque bytes it framed.
/// </summary>
/// <remarks>
/// <para>
/// A value of this type says only that its bytes were well framed. <see cref="UbcArtifactReader"/>
/// produces it after the framing checks, and nothing about the rows' mutual consistency - whether a
/// unit's type index exists, whether the code is well typed - is established until
/// <see cref="UbcVerifier"/> has walked it. <see cref="UbcArtifactWriter"/> accepts it too, and writes
/// whatever it holds, which is how a corpus author writes a malformed artifact.
/// </para>
/// <para>
/// Every collection is immutable, so an artifact the walk admitted can be shared between instances and
/// threads without a copy.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=908095
// Broiler-Human:        PENDING
public sealed class UbcArtifact
{
    /// <summary>An artifact of the given parts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2CC003
    // Broiler-Human:        PENDING
    public UbcArtifact(
        UbcHeader header,
        ImmutableArray<UbcFamilyEntry> families,
        ImmutableArray<UbcSignature> types,
        ImmutableArray<UbcUnit> units,
        ImmutableArray<byte> code,
        ImmutableArray<UbcJumpTable> jumpTables,
        ImmutableArray<UbcRegion> regions,
        ImmutableArray<UbcEntry> entries,
        ImmutableArray<UbcPosition> positions,
        ImmutableArray<UbcFamilyData> familyData,
        UbcEmission? emission,
        uint presentSections = 0)
    {
        Header = header;
        Families = families;
        Types = types;
        Units = units;
        Code = code;
        JumpTables = jumpTables;
        Regions = regions;
        Entries = entries;
        Positions = positions;
        FamilyData = familyData;
        Emission = emission;
        PresentSections = presentSections != 0 ? presentSections : Derive();
    }

    /// <summary>
    /// A bit per section kind present, bit <c>k</c> for kind <c>k</c>. The reader records what it read,
    /// so that an optional section present with no rows is written back present; an artifact built by
    /// hand derives it from its non-empty collections and the required sections.
    /// </summary>
    public uint PresentSections { get; }

    /// <summary>True when the artifact carries a section of <paramref name="kind"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=2A80EE
    // Broiler-Human:        PENDING
    public bool HasSection(UbcSectionKind kind) => (PresentSections & (1u << (int)kind)) != 0;

    /// <summary>The header.</summary>
    public UbcHeader Header { get; }

    /// <summary>The Families section's rows, in slot order.</summary>
    public ImmutableArray<UbcFamilyEntry> Families { get; }

    /// <summary>The Types section's signatures.</summary>
    public ImmutableArray<UbcSignature> Types { get; }

    /// <summary>The Units section's rows.</summary>
    public ImmutableArray<UbcUnit> Units { get; }

    /// <summary>The Code section's bytes.</summary>
    public ImmutableArray<byte> Code { get; }

    /// <summary>The JumpTables section's rows.</summary>
    public ImmutableArray<UbcJumpTable> JumpTables { get; }

    /// <summary>The Regions section's rows, per unit innermost first.</summary>
    public ImmutableArray<UbcRegion> Regions { get; }

    /// <summary>The Entries section's rows.</summary>
    public ImmutableArray<UbcEntry> Entries { get; }

    /// <summary>The Positions section's rows.</summary>
    public ImmutableArray<UbcPosition> Positions { get; }

    /// <summary>The FamilyData sections, in slot order.</summary>
    public ImmutableArray<UbcFamilyData> FamilyData { get; }

    /// <summary>The Emission section, present only in a native form.</summary>
    public UbcEmission? Emission { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A3F3D8
    // Broiler-Human:        PENDING
    private uint Derive()
    {
        var present = (1u << (int)UbcSectionKind.Types) | (1u << (int)UbcSectionKind.Units)
            | (1u << (int)UbcSectionKind.Code) | (1u << (int)UbcSectionKind.Entries);

        if (!Families.IsDefaultOrEmpty) { present |= 1u << (int)UbcSectionKind.Families; }
        if (!JumpTables.IsDefaultOrEmpty) { present |= 1u << (int)UbcSectionKind.JumpTables; }
        if (!Regions.IsDefaultOrEmpty) { present |= 1u << (int)UbcSectionKind.Regions; }
        if (!Positions.IsDefaultOrEmpty) { present |= 1u << (int)UbcSectionKind.Positions; }
        if (Emission is not null) { present |= 1u << (int)UbcSectionKind.Emission; }

        if (!FamilyData.IsDefault)
        {
            foreach (var data in FamilyData)
            {
                present |= 1u << UbcFormat.FamilyDataKind(data.Slot);
            }
        }

        return present;
    }
}

/// <summary>The header: what the artifact is, what it was lowered from, and which form it is in.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=EA2433
// Broiler-Human:        PENDING
public sealed class UbcHeader
{
    /// <summary>A header of the given fields.</summary>
    public UbcHeader(
        uint formatVersion,
        string profileIdentity,
        string manifestIdentity,
        string formIdentity,
        string translatorIdentity,
        uint translatorVersion)
    {
        FormatVersion = formatVersion;
        ProfileIdentity = profileIdentity;
        ManifestIdentity = manifestIdentity;
        FormIdentity = formIdentity;
        TranslatorIdentity = translatorIdentity;
        TranslatorVersion = translatorVersion;
    }

    /// <summary>The universal bytecode format version the artifact is written in.</summary>
    public uint FormatVersion { get; }

    /// <summary>The identity of the language profile the artifact was lowered from.</summary>
    public string ProfileIdentity { get; }

    /// <summary>The identity of the feature manifest the artifact was lowered under.</summary>
    public string ManifestIdentity { get; }

    /// <summary><see cref="UbcFormat.BytecodeForm"/>, or the identity of the emitter form that produced the Emission section.</summary>
    public string FormIdentity { get; }

    /// <summary>The identity of the translator that wrote the artifact.</summary>
    public string TranslatorIdentity { get; }

    /// <summary>The translator's semantic version.</summary>
    public uint TranslatorVersion { get; }
}

/// <summary>A Families row: which family a slot's prefix byte introduces.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4A86E5
// Broiler-Human:        PENDING
public sealed class UbcFamilyEntry
{
    /// <summary>A Families row.</summary>
    public UbcFamilyEntry(byte slot, string identity, uint tableVersion, string manifest)
    {
        Slot = slot;
        Identity = identity;
        TableVersion = tableVersion;
        Manifest = manifest;
    }

    /// <summary>The slot, one to fourteen.</summary>
    public byte Slot { get; }

    /// <summary>The family's identity.</summary>
    public string Identity { get; }

    /// <summary>The version of the family table the code was written against.</summary>
    public uint TableVersion { get; }

    /// <summary>The feature manifest that selects that table.</summary>
    public string Manifest { get; }
}

/// <summary>A Types row: a signature.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=05BA5F
// Broiler-Human:        PENDING
public sealed class UbcSignature
{
    /// <summary>A signature.</summary>
    public UbcSignature(ImmutableArray<UbcSlotType> parameters, ImmutableArray<UbcSlotType> results)
    {
        Parameters = parameters;
        Results = results;
    }

    /// <summary>The parameter types, in order: the first parameters of a unit's locals.</summary>
    public ImmutableArray<UbcSlotType> Parameters { get; }

    /// <summary>The result types, in order, bottom to top.</summary>
    public ImmutableArray<UbcSlotType> Results { get; }
}

/// <summary>A run of locals of one type.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=5CDEAD
// Broiler-Human:        PENDING
public readonly struct UbcLocalRun : System.IEquatable<UbcLocalRun>
{
    /// <summary>A run of <paramref name="count"/> locals of <paramref name="type"/>.</summary>
    public UbcLocalRun(uint count, UbcSlotType type)
    {
        Count = count;
        Type = type;
    }

    /// <summary>How many locals the run declares.</summary>
    public uint Count { get; }

    /// <summary>Their type.</summary>
    public UbcSlotType Type { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E1AA3D
    // Broiler-Human:        PENDING
    public bool Equals(UbcLocalRun other) => Count == other.Count && Type == other.Type;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UbcLocalRun other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => System.HashCode.Combine(Count, Type);

    /// <summary>Value equality.</summary>
    public static bool operator ==(UbcLocalRun left, UbcLocalRun right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=EEBAD9
    // Broiler-Human:        PENDING
    public static bool operator !=(UbcLocalRun left, UbcLocalRun right) => !left.Equals(right);
}

/// <summary>A Units row: one code unit.</summary>
/// <remarks>
/// A unit's locals are its signature's parameters, in order, followed by the runs this row declares.
/// A unit is single-family: every family instruction in it belongs to <see cref="FamilySlot"/>.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=BF7574
// Broiler-Human:        PENDING
public sealed class UbcUnit
{
    /// <summary>A Units row.</summary>
    public UbcUnit(
        uint typeIndex,
        byte familySlot,
        ImmutableArray<UbcLocalRun> locals,
        uint maxWordHeight,
        uint maxValueHeight,
        uint codeOffset,
        uint codeLength,
        UbcUnitFlags flags,
        ImmutableArray<uint> landings)
    {
        TypeIndex = typeIndex;
        FamilySlot = familySlot;
        Locals = locals;
        MaxWordHeight = maxWordHeight;
        MaxValueHeight = maxValueHeight;
        CodeOffset = codeOffset;
        CodeLength = codeLength;
        Flags = flags;
        Landings = landings;
    }

    /// <summary>The index of the unit's signature in the Types section.</summary>
    public uint TypeIndex { get; }

    /// <summary>The family slot of every family instruction in the unit, or zero when it has none.</summary>
    public byte FamilySlot { get; }

    /// <summary>The declared locals beyond the parameters, as runs.</summary>
    public ImmutableArray<UbcLocalRun> Locals { get; }

    /// <summary>The declared maximum operand height of the word plane.</summary>
    public uint MaxWordHeight { get; }

    /// <summary>The declared maximum operand height of the value plane.</summary>
    public uint MaxValueHeight { get; }

    /// <summary>The absolute offset of the unit's first instruction in the Code section.</summary>
    public uint CodeOffset { get; }

    /// <summary>The length of the unit's code, in bytes.</summary>
    public uint CodeLength { get; }

    /// <summary>The unit's flags.</summary>
    public UbcUnitFlags Flags { get; }

    /// <summary>The unit's landing offsets, ascending: every resume point and every region handler.</summary>
    public ImmutableArray<uint> Landings { get; }
}

/// <summary>A JumpTables row: the targets of one <c>jump_table</c>, the last being the default.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=10641C
// Broiler-Human:        PENDING
public sealed class UbcJumpTable
{
    /// <summary>A JumpTables row.</summary>
    public UbcJumpTable(uint unit, ImmutableArray<uint> targets)
    {
        Unit = unit;
        Targets = targets;
    }

    /// <summary>The unit whose instructions the targets are.</summary>
    public uint Unit { get; }

    /// <summary>The absolute targets; the last is the default.</summary>
    public ImmutableArray<uint> Targets { get; }
}

/// <summary>A Regions row: a handler region of one unit.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=79DC2F
// Broiler-Human:        PENDING
public sealed class UbcRegion
{
    /// <summary>A Regions row.</summary>
    public UbcRegion(uint unit, uint start, uint end, uint handler, uint wordEntryHeight, uint valueEntryHeight, byte kind)
    {
        Unit = unit;
        Start = start;
        End = end;
        Handler = handler;
        WordEntryHeight = wordEntryHeight;
        ValueEntryHeight = valueEntryHeight;
        Kind = kind;
    }

    /// <summary>The unit the region belongs to.</summary>
    public uint Unit { get; }

    /// <summary>The absolute offset of the first covered instruction.</summary>
    public uint Start { get; }

    /// <summary>The absolute offset one past the last covered instruction.</summary>
    public uint End { get; }

    /// <summary>The absolute offset of the handler's first instruction.</summary>
    public uint Handler { get; }

    /// <summary>The word-plane operand height the landing truncates to.</summary>
    public uint WordEntryHeight { get; }

    /// <summary>The value-plane operand height the landing truncates to.</summary>
    public uint ValueEntryHeight { get; }

    /// <summary>The family's region kind.</summary>
    public byte Kind { get; }
}

/// <summary>An Entries row: a named entry point.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0DCB8D
// Broiler-Human:        PENDING
public sealed class UbcEntry
{
    /// <summary>An Entries row.</summary>
    public UbcEntry(ImmutableArray<byte> name, uint unit)
    {
        Name = name;
        Unit = unit;
    }

    /// <summary>The name, exactly the core's entry-point bytes.</summary>
    public ImmutableArray<byte> Name { get; }

    /// <summary>The unit it enters.</summary>
    public uint Unit { get; }
}

/// <summary>A Positions row: a source coordinate for one instruction.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=5BC03B
// Broiler-Human:        PENDING
public sealed class UbcPosition
{
    /// <summary>A Positions row.</summary>
    public UbcPosition(uint unit, uint offset, int coordinate0, int coordinate1)
    {
        Unit = unit;
        Offset = offset;
        Coordinate0 = coordinate0;
        Coordinate1 = coordinate1;
    }

    /// <summary>The unit.</summary>
    public uint Unit { get; }

    /// <summary>The absolute offset of the instruction.</summary>
    public uint Offset { get; }

    /// <summary>The core position record's first profile coordinate, as the family populates it.</summary>
    public int Coordinate0 { get; }

    /// <summary>The core position record's second profile coordinate.</summary>
    public int Coordinate1 { get; }
}

/// <summary>A FamilyData section: opaque to the universal bytecode, read by the family's hook and runtime.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=6EF174
// Broiler-Human:        PENDING
public sealed class UbcFamilyData
{
    /// <summary>A FamilyData section of <paramref name="slot"/>.</summary>
    public UbcFamilyData(byte slot, ImmutableArray<byte> body)
    {
        Slot = slot;
        Body = body;
    }

    /// <summary>The family slot the section belongs to.</summary>
    public byte Slot { get; }

    /// <summary>The section's bytes.</summary>
    public ImmutableArray<byte> Body { get; }
}

/// <summary>An Emission section: the emitted bytes of a native form.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=167FF5
// Broiler-Human:        PENDING
public sealed class UbcEmission
{
    /// <summary>An Emission section.</summary>
    public UbcEmission(
        string form,
        uint emitterVersion,
        uint alignment,
        ulong shapeHash,
        ImmutableArray<byte> bytes,
        ImmutableArray<UbcSymbol> symbols)
    {
        Form = form;
        EmitterVersion = emitterVersion;
        Alignment = alignment;
        ShapeHash = shapeHash;
        Bytes = bytes;
        Symbols = symbols;
    }

    /// <summary>The form identity.</summary>
    public string Form { get; }

    /// <summary>The emitter's semantic version.</summary>
    public uint EmitterVersion { get; }

    /// <summary>The alignment of every unit's code, in bytes.</summary>
    public uint Alignment { get; }

    /// <summary>The hash of the family handler-table shape the bytes were emitted against.</summary>
    public ulong ShapeHash { get; }

    /// <summary>The emitted bytes.</summary>
    public ImmutableArray<byte> Bytes { get; }

    /// <summary>One symbol per unit, offsets ascending.</summary>
    public ImmutableArray<UbcSymbol> Symbols { get; }
}

/// <summary>An Emission symbol: where a unit's emitted code begins.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=3BF07B
// Broiler-Human:        PENDING
public readonly struct UbcSymbol : System.IEquatable<UbcSymbol>
{
    /// <summary>A symbol.</summary>
    public UbcSymbol(uint unit, uint offset)
    {
        Unit = unit;
        Offset = offset;
    }

    /// <summary>The unit.</summary>
    public uint Unit { get; }

    /// <summary>The offset of its code in the emitted bytes.</summary>
    public uint Offset { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=716FD6
    // Broiler-Human:        PENDING
    public bool Equals(UbcSymbol other) => Unit == other.Unit && Offset == other.Offset;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UbcSymbol other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => System.HashCode.Combine(Unit, Offset);

    /// <summary>Value equality.</summary>
    public static bool operator ==(UbcSymbol left, UbcSymbol right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=130265
    // Broiler-Human:        PENDING
    public static bool operator !=(UbcSymbol left, UbcSymbol right) => !left.Equals(right);
}
