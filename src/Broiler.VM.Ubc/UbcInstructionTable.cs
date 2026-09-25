// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           42
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  1/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Immutable;

namespace Broiler.VM.Ubc;

/// <summary>The closed set of family-row kinds: what an emitter does with a family instruction.</summary>
/// <remarks>
/// For every kind the family's handler is the instruction's meaning. The kind tells an emitter how to
/// call it and what an answer may be, and tells the walk what the row may do to the control flow.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=FA82F5
// Broiler-Human:        PENDING
public enum UbcInstructionKind : byte
{
    /// <summary>The handler is called and answers a status; it may also throw or trap.</summary>
    Dynamic = 0,

    /// <summary>An emitter may execute the row's primitive itself, and must call the handler where it does not.</summary>
    Primitive = 1,

    /// <summary>The handler answers taken or not; the emitter transfers to the row's target or falls through.</summary>
    Branch = 2,

    /// <summary>The handler answers a call request, or completes the call itself.</summary>
    Call = 3,

    /// <summary>The handler may answer suspend; the emitter captures the frame. Admitted only in a suspendable unit.</summary>
    Suspend = 4,

    /// <summary>The handler answers threw; the emitter unwinds.</summary>
    Throw = 5,
}

/// <summary>One universal trap a primitive row can raise, mapped to a code of the family's trap vocabulary.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A07647
// Broiler-Human:        PENDING
public readonly struct UbcTrapMapping : System.IEquatable<UbcTrapMapping>
{
    /// <summary>Maps <paramref name="universal"/> to <paramref name="familyCode"/>.</summary>
    public UbcTrapMapping(UbcTrapCode universal, ushort familyCode)
    {
        Universal = universal;
        FamilyCode = familyCode;
    }

    /// <summary>The universal trap.</summary>
    public UbcTrapCode Universal { get; }

    /// <summary>The family's code for it.</summary>
    public ushort FamilyCode { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=6799A4
    // Broiler-Human:        PENDING
    public bool Equals(UbcTrapMapping other) => Universal == other.Universal && FamilyCode == other.FamilyCode;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UbcTrapMapping other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => System.HashCode.Combine(Universal, FamilyCode);

    /// <summary>Value equality.</summary>
    public static bool operator ==(UbcTrapMapping left, UbcTrapMapping right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=512124
    // Broiler-Human:        PENDING
    public static bool operator !=(UbcTrapMapping left, UbcTrapMapping right) => !left.Equals(right);
}

/// <summary>One row of a family's instruction table.</summary>
/// <remarks>
/// A row is data. The family's handler for it is the family's static dispatch
/// (<see cref="IUbcFamily"/>), reached by the family opcode byte this row describes; the row states
/// what that handler may do so that the walk, the interpreter and every encoder agree without asking
/// the family.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=9D85CA
// Broiler-Falsified-If: a row constructed here escapes the table's constructor checks, so an inconsistent row reaches the walk
// Broiler-Human:        PENDING
public sealed class UbcInstructionRow
{
    /// <summary>Describes one family instruction. <see cref="UbcInstructionTable.TryCreate"/> checks the whole row.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=EC4FFA
    // Broiler-Human:        PENDING
    public UbcInstructionRow(
        byte opcode,
        string mnemonic,
        UbcOperandShape shape,
        UbcEffect effect,
        UbcTarget target,
        UbcInstructionKind kind,
        bool isTerminal = false,
        uint cost = 1,
        UbcPrimitive? primitive = null,
        byte region = 0,
        ImmutableArray<UbcTrapMapping> traps = default)
    {
        Opcode = opcode;
        Mnemonic = mnemonic;
        Shape = shape;
        Effect = effect;
        Target = target;
        Kind = kind;
        IsTerminal = isTerminal;
        Cost = cost;
        Primitive = primitive;
        Region = region;
        Traps = traps.IsDefault ? ImmutableArray<UbcTrapMapping>.Empty : traps;
    }

    /// <summary>The family's opcode byte.</summary>
    public byte Opcode { get; }

    /// <summary>The mnemonic, with the family's own prefix, for diagnostics and disassembly.</summary>
    public string Mnemonic { get; }

    /// <summary>The operand shape, from the closed set.</summary>
    public UbcOperandShape Shape { get; }

    /// <summary>The stack effect, in the closed language.</summary>
    public UbcEffect Effect { get; }

    /// <summary>Whether the operand is a code target, and what the taken edge pushes.</summary>
    public UbcTarget Target { get; }

    /// <summary>What an emitter does with the row.</summary>
    public UbcInstructionKind Kind { get; }

    /// <summary>True when execution never falls through past the row.</summary>
    public bool IsTerminal { get; }

    /// <summary>The fuel charged before the row's effects. One unless the table says otherwise.</summary>
    public uint Cost { get; }

    /// <summary>For a primitive row, the entry of the primitive table the handler is asserted to equal.</summary>
    public UbcPrimitive? Primitive { get; }

    /// <summary>For a region primitive, the index of the family region it addresses.</summary>
    public byte Region { get; }

    /// <summary>For a primitive row, the family code of every universal trap the primitive can raise.</summary>
    public ImmutableArray<UbcTrapMapping> Traps { get; }
}

/// <summary>A kind of handler region, and what landing in one pushes.</summary>
/// <remarks>
/// When an exception lands in a region, the emitter truncates both planes to the region's entry
/// heights and the family pushes what its lowering expects - the exception for one kind, a completion
/// record for another. The walk must know those types to check the handler's entry state, so they are
/// table data rather than a family callback.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C111A0
// Broiler-Human:        PENDING
public sealed class UbcRegionKindRow
{
    /// <summary>A region kind: its byte, its name, and its landing pushes bottom to top.</summary>
    public UbcRegionKindRow(byte kind, string name, ImmutableArray<UbcSlotType> landingPushes)
    {
        Kind = kind;
        Name = name;
        LandingPushes = landingPushes;
    }

    /// <summary>The kind byte a Regions row carries.</summary>
    public byte Kind { get; }

    /// <summary>The name, for diagnostics.</summary>
    public string Name { get; }

    /// <summary>What the family pushes at a landing, bottom to top.</summary>
    public ImmutableArray<UbcSlotType> LandingPushes { get; }
}

/// <summary>A family region: named memory a region primitive addresses.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=565489
// Broiler-Human:        PENDING
public sealed class UbcRegionDeclaration
{
    /// <summary>A region: its index, which primitive rows name, and its name.</summary>
    public UbcRegionDeclaration(byte index, string name)
    {
        Index = index;
        Name = name;
    }

    /// <summary>The index a region primitive row names.</summary>
    public byte Index { get; }

    /// <summary>The name, for diagnostics.</summary>
    public string Name { get; }
}

/// <summary>One code of a family's trap vocabulary.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=3D4AAD
// Broiler-Human:        PENDING
public sealed class UbcFamilyTrap
{
    /// <summary>A trap code and its name.</summary>
    public UbcFamilyTrap(ushort code, string name)
    {
        Code = code;
        Name = name;
    }

    /// <summary>The code a <c>trap</c> instruction or a primitive's mapping names.</summary>
    public ushort Code { get; }

    /// <summary>The name, for diagnostics.</summary>
    public string Name { get; }
}

/// <summary>
/// One family's instruction table for one feature manifest: an immutable array of rows indexed by the
/// family's opcode byte, with the region kinds, regions and trap codes the rows name.
/// </summary>
/// <remarks>
/// <para>
/// A family registers one table per manifest it admits, and an artifact's Families row names the
/// manifest its table is selected by. A table admitted under one manifest is refused under another,
/// which is the programme's rule U5 stated as data: the version and the manifest are fields of the
/// table, and the walk compares both with the artifact's.
/// </para>
/// <para>
/// <see cref="TryCreate"/> checks every row against the schema and answers a defect naming the row
/// rather than throwing, because a table is a family's input to a composition root and a root that
/// cannot build one should say which row is wrong. This assembly declares no table of its own.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=42AD9F
// Broiler-Falsified-If: a table is created whose row violates the schema - a primitive whose signature is not the row's effect, a target on a non-U32 operand, a trap code outside the vocabulary
// Broiler-Human:        PENDING
public sealed class UbcInstructionTable
{
    private readonly UbcInstructionRow?[] rows;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D5A27B
    // Broiler-Human:        PENDING
    private UbcInstructionTable(
        string familyIdentity,
        uint tableVersion,
        VmFeatureManifestId manifest,
        UbcInstructionRow?[] rows,
        ImmutableArray<UbcInstructionRow> ordered,
        ImmutableArray<UbcRegionKindRow> regionKinds,
        ImmutableArray<UbcRegionDeclaration> regions,
        ImmutableArray<UbcFamilyTrap> traps,
        bool canonicaliseNaN)
    {
        FamilyIdentity = familyIdentity;
        TableVersion = tableVersion;
        Manifest = manifest;
        this.rows = rows;
        Rows = ordered;
        RegionKinds = regionKinds;
        Regions = regions;
        Traps = traps;
        CanonicaliseNaN = canonicaliseNaN;
    }

    /// <summary>The family's identity.</summary>
    public string FamilyIdentity { get; }

    /// <summary>The table version: it moves when the family's rows change.</summary>
    public uint TableVersion { get; }

    /// <summary>The feature manifest that selects this table.</summary>
    public VmFeatureManifestId Manifest { get; }

    /// <summary>Every row, in ascending opcode order.</summary>
    public ImmutableArray<UbcInstructionRow> Rows { get; }

    /// <summary>The region kinds a Regions row of this family may name.</summary>
    public ImmutableArray<UbcRegionKindRow> RegionKinds { get; }

    /// <summary>The regions a region primitive may address.</summary>
    public ImmutableArray<UbcRegionDeclaration> Regions { get; }

    /// <summary>The family's trap vocabulary.</summary>
    public ImmutableArray<UbcFamilyTrap> Traps { get; }

    /// <summary>
    /// True when a NaN result of every primitive <see cref="UbcPrimitives.Canonicalises"/> names is replaced
    /// by the canonical NaN. The sign-bit operations abs, neg and copysign, the reinterpretations and the
    /// region loads are unaffected under either setting: a NaN they answer keeps its payload, signalling
    /// or quiet.
    /// </summary>
    public bool CanonicaliseNaN { get; }

    /// <summary>The row of <paramref name="opcode"/>, or false when the table defines none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=61E7DC
    // Broiler-Falsified-If: an opcode the table does not define answers true
    // Broiler-Human:        PENDING
    public bool TryGetRow(byte opcode, out UbcInstructionRow row)
    {
        var found = rows[opcode];
        row = found!;
        return found is not null;
    }

    /// <summary>The region kind <paramref name="kind"/>, or false when the table defines none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F6D6AE
    // Broiler-Human:        PENDING
    public bool TryGetRegionKind(byte kind, out UbcRegionKindRow row)
    {
        foreach (var candidate in RegionKinds)
        {
            if (candidate.Kind == kind)
            {
                row = candidate;
                return true;
            }
        }

        row = null!;
        return false;
    }

    /// <summary>True when <paramref name="code"/> is a code of the family's trap vocabulary.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F19A10
    // Broiler-Human:        PENDING
    public bool DefinesTrap(ushort code)
    {
        foreach (var trap in Traps)
        {
            if (trap.Code == code)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Builds a table, or answers the first schema violation as <paramref name="defect"/>, naming the row.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=63FE0D
    // Broiler-Falsified-If: a row that breaks one of the rules listed in the method body is accepted, or a missing list or element throws rather than answering a defect
    // Broiler-Human:        PENDING
    public static bool TryCreate(
        string familyIdentity,
        uint tableVersion,
        VmFeatureManifestId manifest,
        System.Collections.Generic.IEnumerable<UbcInstructionRow> rows,
        System.Collections.Generic.IEnumerable<UbcRegionKindRow> regionKinds,
        System.Collections.Generic.IEnumerable<UbcRegionDeclaration> regions,
        System.Collections.Generic.IEnumerable<UbcFamilyTrap> traps,
        bool canonicaliseNaN,
        out UbcInstructionTable? table,
        out string? defect)
    {
        table = null;
        defect = null;

        if (!VmProfileId.TryParse(familyIdentity, out _))
        {
            defect = $"the family identity '{familyIdentity}' is not an identity the core's grammar admits";
            return false;
        }

        if (manifest.IsEmpty)
        {
            defect = "a table is selected by a feature manifest, and none was given";
            return false;
        }

        // A list or an element that is missing is a schema violation like any other, answered as a
        // defect rather than as the null dereference it would otherwise become.
        if (rows is null || regionKinds is null || regions is null || traps is null)
        {
            defect = "the rows, region kinds, regions and traps must each be given, even when empty; one is missing";
            return false;
        }

        var trapList = System.Linq.Enumerable.ToArray(traps);
        var trapCodes = new System.Collections.Generic.HashSet<ushort>();

        foreach (var trap in trapList)
        {
            if (trap is null)
            {
                defect = "a trap of the vocabulary is missing";
                return false;
            }

            if (!trapCodes.Add(trap.Code))
            {
                defect = $"trap code {trap.Code} is declared twice";
                return false;
            }
        }

        var regionList = System.Linq.Enumerable.ToArray(regions);
        var regionIndices = new System.Collections.Generic.HashSet<byte>();

        foreach (var region in regionList)
        {
            if (region is null)
            {
                defect = "a region declaration is missing";
                return false;
            }

            if (!regionIndices.Add(region.Index))
            {
                defect = $"region {region.Index} is declared twice";
                return false;
            }
        }

        var kindList = System.Linq.Enumerable.ToArray(regionKinds);
        var kindBytes = new System.Collections.Generic.HashSet<byte>();

        foreach (var kind in kindList)
        {
            if (kind is null)
            {
                defect = "a region kind is missing";
                return false;
            }

            if (!kindBytes.Add(kind.Kind))
            {
                defect = $"region kind {kind.Kind} is declared twice";
                return false;
            }

            if (kind.LandingPushes.IsDefault || !AllDefined(kind.LandingPushes))
            {
                defect = $"region kind {kind.Kind} names a landing push that is not a slot type";
                return false;
            }
        }

        var byOpcode = new UbcInstructionRow?[256];

        foreach (var row in rows)
        {
            if (row is null)
            {
                defect = "a row is missing";
                return false;
            }

            if (byOpcode[row.Opcode] is not null)
            {
                defect = $"opcode 0x{row.Opcode:X2} has two rows";
                return false;
            }

            var problem = CheckRow(row, trapCodes, regionIndices);

            if (problem is not null)
            {
                defect = $"row 0x{row.Opcode:X2} ({row.Mnemonic}): {problem}";
                return false;
            }

            byOpcode[row.Opcode] = row;
        }

        var ordered = ImmutableArray.CreateBuilder<UbcInstructionRow>();

        foreach (var row in byOpcode)
        {
            if (row is not null)
            {
                ordered.Add(row);
            }
        }

        table = new UbcInstructionTable(
            familyIdentity,
            tableVersion,
            manifest,
            byOpcode,
            ordered.ToImmutable(),
            ImmutableArray.Create(kindList),
            ImmutableArray.Create(regionList),
            ImmutableArray.Create(trapList),
            canonicaliseNaN);
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=BD778E
    // Broiler-Falsified-If: a primitive row's effect differs from its primitive's signature and the row is admitted, or a non-primitive row carries a primitive or a trap mapping
    // Broiler-Human:        PENDING
    private static string? CheckRow(
        UbcInstructionRow row,
        System.Collections.Generic.HashSet<ushort> trapCodes,
        System.Collections.Generic.HashSet<byte> regions)
    {
        if (string.IsNullOrEmpty(row.Mnemonic))
        {
            return "the mnemonic is empty";
        }

        if (!UbcOperandShapes.IsDefined(row.Shape))
        {
            return "the operand shape is not in the closed set";
        }

        if (row.Effect is null || row.Target is null)
        {
            return "the effect and the target must both be given";
        }

        if (row.Kind > UbcInstructionKind.Throw)
        {
            return "the kind is not in the closed set";
        }

        if (row.Cost == 0)
        {
            return "a row costs at least one fuel unit";
        }

        if (row.Effect.Form == UbcEffectForm.Counted && row.Shape is not (UbcOperandShape.U8 or UbcOperandShape.U16))
        {
            return "a counted effect needs a U8 or U16 operand, which is its count";
        }

        if (row.Target.IsCode && row.Shape != UbcOperandShape.U32)
        {
            return "a code target is carried in a U32 operand";
        }

        if (row.Kind == UbcInstructionKind.Branch && !row.Target.IsCode)
        {
            return "a branch row names a code target";
        }

        if (row.Kind != UbcInstructionKind.Branch && row.Target.IsCode)
        {
            return "only a branch row names a code target";
        }

        if (row.Kind == UbcInstructionKind.Branch && row.IsTerminal)
        {
            return "a branch row falls through when it is not taken, so it is not terminal";
        }

        if (row.Kind != UbcInstructionKind.Primitive)
        {
            if (row.Primitive is not null || !row.Traps.IsEmpty || row.Region != 0)
            {
                return "only a primitive row names a primitive, a region or trap codes";
            }

            return null;
        }

        return CheckPrimitive(row, trapCodes, regions);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=56F521
    // Broiler-Falsified-If: a primitive row is admitted without a mapping for a trap its primitive can raise, or with a region the table does not declare
    // Broiler-Human:        PENDING
    private static string? CheckPrimitive(
        UbcInstructionRow row,
        System.Collections.Generic.HashSet<ushort> trapCodes,
        System.Collections.Generic.HashSet<byte> regions)
    {
        if (row.Primitive is not { } primitive || !UbcPrimitives.IsDefined(primitive))
        {
            return "a primitive row names an entry of the primitive table";
        }

        if (row.Effect.Form != UbcEffectForm.Listed || row.IsTerminal)
        {
            return "a primitive row has a fixed effect and falls through";
        }

        if (primitive == UbcPrimitive.WordKeep)
        {
            var keeps = row.Effect.Pops.Length == 1 && row.Effect.Pushes.Length == 1
                && row.Effect.Pops[0] == row.Effect.Pushes[0] && row.Effect.Pops[0] != UbcSlotType.V;

            if (!keeps)
            {
                return "word.keep keeps one word: its effect is [w] -> [w] for one word type";
            }
        }
        else
        {
            UbcPrimitives.TryGetSignature(primitive, out var operands, out var results);

            if (!System.Linq.Enumerable.SequenceEqual(operands, row.Effect.Pops) ||
                !System.Linq.Enumerable.SequenceEqual(results, row.Effect.Pushes))
            {
                return "the effect is not the primitive's signature";
            }
        }

        if (UbcPrimitives.IsRegionAccess(primitive))
        {
            if (!regions.Contains(row.Region))
            {
                return "a region primitive names a region the table declares";
            }

            var offsetShape = primitive == UbcPrimitive.RegionSize
                ? row.Shape is UbcOperandShape.None or UbcOperandShape.U8
                : row.Shape is UbcOperandShape.U32 or UbcOperandShape.U8U32;

            if (!offsetShape)
            {
                return "a region access carries its static offset in a U32 operand or the second field of a U8U32, and a region size in none or a U8";
            }
        }
        else if (row.Region != 0)
        {
            return "only a region primitive names a region";
        }

        var needed = UbcPrimitives.TrapsOf(primitive);
        var mapped = UbcTrapCode.None;

        foreach (var mapping in row.Traps)
        {
            if ((needed & mapping.Universal) == 0 || System.Numerics.BitOperations.PopCount((uint)mapping.Universal) != 1)
            {
                return $"the primitive cannot raise {mapping.Universal}, so it is not mapped";
            }

            if ((mapped & mapping.Universal) != 0)
            {
                return $"{mapping.Universal} is mapped twice";
            }

            if (!trapCodes.Contains(mapping.FamilyCode))
            {
                return $"trap code {mapping.FamilyCode} is not in the table's trap vocabulary";
            }

            mapped |= mapping.Universal;
        }

        if (mapped != needed)
        {
            return $"every trap the primitive can raise is mapped to a family code; {needed & ~mapped} is not";
        }

        return null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B9E1F3
    // Broiler-Human:        PENDING
    private static bool AllDefined(ImmutableArray<UbcSlotType> types)
    {
        foreach (var type in types)
        {
            if (!UbcSlotTypes.IsDefined((byte)type))
            {
                return false;
            }
        }

        return true;
    }
}
