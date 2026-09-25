using System.Collections.Immutable;
using System.Text;
using Broiler.VM.Ubc;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// A mutable description of one artifact, written into bytes by <see cref="UbcArtifactWriter"/>. A
/// corpus entry is usually a control's description with one thing changed, so every part is a plain
/// field a mutation can reach: a list that is <see langword="null"/> is a section that is absent, and
/// an empty list is a section present with no rows.
/// </summary>
internal sealed class UbcCorpusSpec
{
    private readonly List<byte> code = new();

    internal string Profile { get; set; } = UbcCorpusFamily.Identity;

    internal string Manifest { get; set; } = UbcCorpusFamily.BaseManifestText;

    internal string Form { get; set; } = UbcFormat.BytecodeForm;

    internal string Translator { get; set; } = "corpus";

    internal uint FormatVersion { get; set; } = UbcFormat.FormatVersion;

    internal uint TranslatorVersion { get; set; } = 1;

    internal List<UbcFamilyEntry>? Families { get; set; }

    internal List<UbcSignature>? Types { get; set; } = new();

    internal List<UbcUnit>? Units { get; set; } = new();

    /// <summary>The Code section's bytes; null leaves the section out.</summary>
    internal byte[]? CodeOverride { get; set; }

    internal bool OmitCode { get; set; }

    internal List<UbcJumpTable>? JumpTables { get; set; }

    internal List<UbcRegion>? Regions { get; set; }

    internal List<UbcEntry>? Entries { get; set; } = new();

    internal List<UbcPosition>? Positions { get; set; }

    internal SortedDictionary<byte, byte[]> FamilyData { get; } = new();

    internal UbcEmission? Emission { get; set; }

    /// <summary>Section bodies that replace the encoded one of their kind.</summary>
    internal Dictionary<uint, byte[]> Bodies { get; } = new();

    /// <summary>Stated lengths that replace a section's true length.</summary>
    internal Dictionary<uint, ulong> StatedLengths { get; } = new();

    /// <summary>Sections added beyond the ones the fields describe: kind, body, and where in the order.</summary>
    internal List<(int At, uint Kind, byte[] Body)> ExtraSections { get; } = new();

    /// <summary>A rearrangement of the section order, applied after every section is assembled.</summary>
    internal Func<List<UbcCorpusSection>, List<UbcCorpusSection>>? Rearrange { get; set; }

    internal byte[]? Magic { get; set; }

    internal uint? SectionCount { get; set; }

    internal byte[] Trailing { get; set; } = Array.Empty<byte>();

    /// <summary>The Code section's bytes as the units were added.</summary>
    internal byte[] Code => CodeOverride ?? code.ToArray();

    /// <summary>The Code offset the next unit's first instruction will have.</summary>
    internal uint NextCodeOffset => (uint)code.Count;

    internal int AddType(UbcSlotType[] parameters, UbcSlotType[] results)
    {
        Types!.Add(new UbcSignature([.. parameters], [.. results]));
        return Types.Count - 1;
    }

    /// <summary>
    /// Adds a unit whose code <paramref name="body"/> writes; the body answers the unit's landings. The
    /// builder's base offset is where the unit starts, so every offset it reports is absolute.
    /// </summary>
    internal int AddUnit(
        int type,
        byte slot,
        uint maxWords,
        uint maxValues,
        UbcUnitFlags flags,
        Func<UbcCodeBuilder, IEnumerable<uint>> body,
        params UbcLocalRun[] locals)
    {
        var builder = new UbcCodeBuilder((uint)code.Count);
        var landings = body(builder).ToArray();
        var bytes = builder.ToArray();

        Units!.Add(new UbcUnit((uint)type, slot, [.. locals], maxWords, maxValues, (uint)code.Count, (uint)bytes.Length, flags, [.. landings]));
        code.AddRange(bytes);
        return Units.Count - 1;
    }

    /// <summary>Appends raw bytes to the Code section outside any unit.</summary>
    internal void AppendCode(params byte[] bytes) => code.AddRange(bytes);

    internal void AddEntry(string name, int unit) => AddEntry(Encoding.UTF8.GetBytes(name), unit);

    internal void AddEntry(byte[] name, int unit) => Entries!.Add(new UbcEntry([.. name], (uint)unit));

    /// <summary>Declares the corpus family in <paramref name="slot"/>.</summary>
    internal UbcCorpusSpec WithFamily(
        byte slot = 1,
        string identity = UbcCorpusFamily.Identity,
        uint version = UbcCorpusFamily.BaseTableVersion,
        string? manifest = null)
    {
        Families ??= new();
        Families.Add(new UbcFamilyEntry(slot, identity, version, manifest ?? Manifest));
        return this;
    }

    /// <summary>Replaces one unit row with <paramref name="change"/> applied.</summary>
    internal void ChangeUnit(int index, Func<UbcUnit, UbcUnit> change) => Units![index] = change(Units[index]);

    /// <summary>Every section in the order it will be written.</summary>
    internal List<UbcCorpusSection> Sections()
    {
        var sections = new List<UbcCorpusSection>();

        void Add(UbcSectionKind kind, byte[]? encoded)
        {
            if (encoded is not null)
            {
                sections.Add(Section((uint)kind, encoded));
            }
        }

        Add(UbcSectionKind.Families, Families is null ? null : UbcSectionEncoder.Families(Families));
        Add(UbcSectionKind.Types, Types is null ? null : UbcSectionEncoder.Types(Types));
        Add(UbcSectionKind.Units, Units is null ? null : UbcSectionEncoder.Units(Units));
        Add(UbcSectionKind.Code, OmitCode ? null : Code);
        Add(UbcSectionKind.JumpTables, JumpTables is null ? null : UbcSectionEncoder.JumpTables(JumpTables));
        Add(UbcSectionKind.Regions, Regions is null ? null : UbcSectionEncoder.Regions(Regions));
        Add(UbcSectionKind.Entries, Entries is null ? null : UbcSectionEncoder.Entries(Entries));
        Add(UbcSectionKind.Positions, Positions is null ? null : UbcSectionEncoder.Positions(Positions));

        foreach (var (slot, body) in FamilyData)
        {
            sections.Add(Section(UbcFormat.FamilyDataKind(slot), body));
        }

        if (Emission is not null)
        {
            sections.Add(Section((uint)UbcSectionKind.Emission, UbcSectionEncoder.Emission(Emission)));
        }

        foreach (var (at, kind, body) in ExtraSections)
        {
            sections.Insert(Math.Min(at, sections.Count), new UbcCorpusSection(kind, body, (ulong)body.Length));
        }

        return Rearrange is null ? sections : Rearrange(sections);
    }

    /// <summary>The bytes, and where each part of them lies.</summary>
    internal UbcCorpusBuilt Build()
    {
        var sections = Sections();
        var header = new UbcHeader(FormatVersion, Profile, Manifest, Form, Translator, TranslatorVersion);
        var count = SectionCount ?? (uint)sections.Count;

        UbcArtifactWriter Writer(int sectionsWritten)
        {
            var writer = new UbcArtifactWriter(header).WithSectionCount(count);

            if (Magic is not null)
            {
                writer.WithMagic(Magic);
            }

            for (var index = 0; index < sectionsWritten; index++)
            {
                var section = sections[index];
                writer.RawSectionWithLength(section.Kind, section.StatedLength, section.Body);
            }

            return writer;
        }

        var starts = new ulong[sections.Count];
        var bodies = new ulong[sections.Count];

        for (var index = 0; index < sections.Count; index++)
        {
            starts[index] = (ulong)Writer(index).ToArray().Length;
            bodies[index] = starts[index] + (ulong)VarUIntLength(sections[index].Kind) + (ulong)VarUIntLength(sections[index].StatedLength);
        }

        var bytes = Writer(sections.Count).WithTrailing(Trailing).ToArray();
        var identities = new ulong[4];
        var offset = (ulong)(Magic?.Length ?? 4) + (ulong)VarUIntLength(FormatVersion);
        var strings = new[] { Profile, Manifest, Form, Translator };

        for (var index = 0; index < 4; index++)
        {
            identities[index] = offset;
            var length = Encoding.UTF8.GetByteCount(strings[index]);
            offset += (ulong)VarUIntLength((ulong)length) + (ulong)length;
        }

        return new UbcCorpusBuilt(bytes, sections.Select(static section => section.Kind).ToArray(), starts, bodies, identities);
    }

    internal byte[] Bytes() => Build().Bytes;

    private UbcCorpusSection Section(uint kind, byte[] encoded)
    {
        var body = Bodies.TryGetValue(kind, out var replaced) ? replaced : encoded;
        var stated = StatedLengths.TryGetValue(kind, out var length) ? length : (ulong)body.Length;
        return new UbcCorpusSection(kind, body, stated);
    }

    internal static int VarUIntLength(ulong value)
    {
        var length = 1;

        while (value >= 0x80)
        {
            value >>= 7;
            length++;
        }

        return length;
    }
}

/// <summary>One section as it will be written: its kind, its body and the length it states.</summary>
internal sealed record UbcCorpusSection(uint Kind, byte[] Body, ulong StatedLength);

/// <summary>An artifact's bytes and the offsets a pinned position is computed from.</summary>
internal sealed class UbcCorpusBuilt
{
    private readonly uint[] kinds;
    private readonly ulong[] starts;
    private readonly ulong[] bodies;
    private readonly ulong[] identities;

    internal UbcCorpusBuilt(byte[] bytes, uint[] kinds, ulong[] starts, ulong[] bodies, ulong[] identities)
    {
        Bytes = bytes;
        this.kinds = kinds;
        this.starts = starts;
        this.bodies = bodies;
        this.identities = identities;
    }

    internal byte[] Bytes { get; }

    internal ulong Length => (ulong)Bytes.Length;

    /// <summary>The offset of the header's identity <paramref name="index"/>: profile, manifest, form, translator.</summary>
    internal ulong IdentityAt(int index) => identities[index];

    /// <summary>The offset of the kind byte of the <paramref name="index"/>th section written.</summary>
    internal ulong SectionAt(int index) => starts[index];

    /// <summary>The offset of the first section of <paramref name="kind"/>.</summary>
    internal ulong SectionOf(UbcSectionKind kind) => starts[Array.IndexOf(kinds, (uint)kind)];

    /// <summary>The offset of the first byte of the body of the first section of <paramref name="kind"/>.</summary>
    internal ulong BodyOf(UbcSectionKind kind) => bodies[Array.IndexOf(kinds, (uint)kind)];

    /// <summary>The offset of the first byte of the body of the <paramref name="index"/>th section written.</summary>
    internal ulong BodyAt(int index) => bodies[index];
}

/// <summary>Family instructions for <see cref="UbcCodeBuilder"/>, their shapes read from the corpus family's wide table.</summary>
internal static class UbcCorpusCode
{
    private static readonly UbcInstructionTable Wide = UbcCorpusFamily.Tables()[1];

    /// <summary>A family instruction of <paramref name="slot"/>, its operand written at its row's width.</summary>
    internal static UbcCodeBuilder F(this UbcCodeBuilder builder, byte opcode, ulong operand = 0, byte slot = 1)
    {
        Wide.TryGetRow(opcode, out var row);
        return builder.EmitFamily(slot, opcode, row?.Shape ?? UbcOperandShape.None, operand);
    }

    /// <summary>A branch row of <paramref name="slot"/> whose target is <paramref name="label"/>.</summary>
    internal static UbcCodeBuilder FTo(this UbcCodeBuilder builder, byte opcode, int label, byte slot = 1) =>
        builder.EmitFamilyTo(slot, opcode, label);

    /// <summary>Marks <paramref name="label"/> here and answers its absolute offset.</summary>
    internal static uint Here(this UbcCodeBuilder builder, int label)
    {
        var at = builder.Offset;
        builder.Mark(label);
        return at;
    }

    internal static ulong Bits(float value) => BitConverter.SingleToUInt32Bits(value);

    internal static ulong Bits(double value) => BitConverter.DoubleToUInt64Bits(value);

    /// <summary>A two-field operand: <paramref name="first"/> in the low field, <paramref name="second"/> after it.</summary>
    internal static ulong Pair(UbcOperandShape shape, ulong first, ulong second) => shape switch
    {
        UbcOperandShape.U8U8 or UbcOperandShape.U8U16 or UbcOperandShape.U8U32 => first | (second << 8),
        UbcOperandShape.U16U16 => first | (second << 16),
        _ => throw new ArgumentOutOfRangeException(nameof(shape)),
    };
}
