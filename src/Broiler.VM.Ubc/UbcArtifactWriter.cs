// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   43
// Annotated:        43/43
// Exempt:           10
// Human-reviewed:   0/43
// IP risk:          Low
// Security risk:    Low
// Criteria:         5/0
// Resource impact:  2/10 max
// Unverified:       43
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Broiler.VM.Ubc;

/// <summary>
/// Writes a universal bytecode container. <b>Deliberately unchecked</b>: it writes whatever it is
/// given, in the order it is given, so that a translator writes a well-formed artifact and a corpus
/// author writes a malformed one with the same tool.
/// </summary>
/// <remarks>
/// <para>
/// Nothing here validates. A section added twice is written twice, a kind added out of order is
/// written out of order, a raw section of an undefined kind is written, and a stated section count or
/// length that disagrees with what follows is written as stated. The walk is the only judge of what
/// was written, which is the position the JavaScript profile's artifact writers occupy for its
/// formats.
/// </para>
/// <para>
/// <see cref="Write(UbcArtifact)"/> is the one canonical writer: every section an artifact holds, in
/// kind order, each body as <see cref="UbcSectionEncoder"/> encodes it. Reading its output and writing
/// the result again yields the same bytes, which the codec's determinism check asserts.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Low; Resources=2; Fingerprint=C27007
// Broiler-Human:        PENDING
public sealed class UbcArtifactWriter
{
    private readonly UbcHeader header;
    private readonly List<(uint Kind, ulong StatedLength, byte[] Body, bool Explicit)> sections = new();
    private byte[] magic = UbcFormat.Magic.ToArray();
    private uint? sectionCount;
    private byte[] trailing = System.Array.Empty<byte>();

    /// <summary>A writer for an artifact with <paramref name="header"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D8AD2C
    // Broiler-Human:        PENDING
    public UbcArtifactWriter(UbcHeader header) => this.header = header ?? throw new System.ArgumentNullException(nameof(header));

    /// <summary>Replaces the four magic bytes, for a corpus entry that tests them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=66361B
    // Broiler-Human:        PENDING
    public UbcArtifactWriter WithMagic(System.ReadOnlySpan<byte> bytes)
    {
        magic = bytes.ToArray();
        return this;
    }

    /// <summary>States a section count other than the number of sections added.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B182D1
    // Broiler-Human:        PENDING
    public UbcArtifactWriter WithSectionCount(uint count)
    {
        sectionCount = count;
        return this;
    }

    /// <summary>Appends bytes after the last section.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=7E7C6D
    // Broiler-Human:        PENDING
    public UbcArtifactWriter WithTrailing(System.ReadOnlySpan<byte> bytes)
    {
        trailing = bytes.ToArray();
        return this;
    }

    /// <summary>Adds a section of any kind, defined or not, with its true length.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0B4263
    // Broiler-Human:        PENDING
    public UbcArtifactWriter RawSection(uint kind, System.ReadOnlySpan<byte> body)
    {
        sections.Add((kind, (ulong)body.Length, body.ToArray(), false));
        return this;
    }

    /// <summary>Adds a section whose stated length is <paramref name="statedLength"/>, whatever its body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1041D0
    // Broiler-Human:        PENDING
    public UbcArtifactWriter RawSectionWithLength(uint kind, ulong statedLength, System.ReadOnlySpan<byte> body)
    {
        sections.Add((kind, statedLength, body.ToArray(), true));
        return this;
    }

    /// <summary>Adds a Families section.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B68496
    // Broiler-Human:        PENDING
    public UbcArtifactWriter Families(IEnumerable<UbcFamilyEntry> rows) => RawSection((uint)UbcSectionKind.Families, UbcSectionEncoder.Families(rows));

    /// <summary>Adds a Types section.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D3D542
    // Broiler-Human:        PENDING
    public UbcArtifactWriter Types(IEnumerable<UbcSignature> rows) => RawSection((uint)UbcSectionKind.Types, UbcSectionEncoder.Types(rows));

    /// <summary>Adds a Units section.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=DDB88A
    // Broiler-Human:        PENDING
    public UbcArtifactWriter Units(IEnumerable<UbcUnit> rows) => RawSection((uint)UbcSectionKind.Units, UbcSectionEncoder.Units(rows));

    /// <summary>Adds a Code section.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=FA9ED0
    // Broiler-Human:        PENDING
    public UbcArtifactWriter Code(System.ReadOnlySpan<byte> code) => RawSection((uint)UbcSectionKind.Code, code);

    /// <summary>Adds a JumpTables section.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=09C47A
    // Broiler-Human:        PENDING
    public UbcArtifactWriter JumpTables(IEnumerable<UbcJumpTable> rows) => RawSection((uint)UbcSectionKind.JumpTables, UbcSectionEncoder.JumpTables(rows));

    /// <summary>Adds a Regions section.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A00667
    // Broiler-Human:        PENDING
    public UbcArtifactWriter Regions(IEnumerable<UbcRegion> rows) => RawSection((uint)UbcSectionKind.Regions, UbcSectionEncoder.Regions(rows));

    /// <summary>Adds an Entries section.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F0B6CE
    // Broiler-Human:        PENDING
    public UbcArtifactWriter Entries(IEnumerable<UbcEntry> rows) => RawSection((uint)UbcSectionKind.Entries, UbcSectionEncoder.Entries(rows));

    /// <summary>Adds a Positions section.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0C1896
    // Broiler-Human:        PENDING
    public UbcArtifactWriter Positions(IEnumerable<UbcPosition> rows) => RawSection((uint)UbcSectionKind.Positions, UbcSectionEncoder.Positions(rows));

    /// <summary>Adds the FamilyData section of <paramref name="slot"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=04843F
    // Broiler-Human:        PENDING
    public UbcArtifactWriter FamilyData(byte slot, System.ReadOnlySpan<byte> body) => RawSection(UbcFormat.FamilyDataKind(slot), body);

    /// <summary>Adds an Emission section.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E7783A
    // Broiler-Human:        PENDING
    public UbcArtifactWriter Emission(UbcEmission emission) => RawSection((uint)UbcSectionKind.Emission, UbcSectionEncoder.Emission(emission));

    /// <summary>The bytes: the header, then every section in the order added, then any trailing bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=937800
    // Broiler-Falsified-If: two calls on one writer answer different bytes
    // Broiler-Human:        PENDING
    public byte[] ToArray()
    {
        var output = new List<byte>();
        output.AddRange(magic);
        UbcSectionEncoder.WriteVarUInt(output, header.FormatVersion);
        UbcSectionEncoder.WriteIdentity(output, header.ProfileIdentity);
        UbcSectionEncoder.WriteIdentity(output, header.ManifestIdentity);
        UbcSectionEncoder.WriteIdentity(output, header.FormIdentity);
        UbcSectionEncoder.WriteIdentity(output, header.TranslatorIdentity);
        UbcSectionEncoder.WriteVarUInt(output, header.TranslatorVersion);
        UbcSectionEncoder.WriteVarUInt(output, sectionCount ?? (uint)sections.Count);

        foreach (var (kind, statedLength, body, _) in sections)
        {
            UbcSectionEncoder.WriteVarUInt(output, kind);
            UbcSectionEncoder.WriteVarUInt(output, statedLength);
            output.AddRange(body);
        }

        output.AddRange(trailing);
        return output.ToArray();
    }

    /// <summary>The canonical bytes of <paramref name="artifact"/>: every section it holds, in kind order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=12B54D
    // Broiler-Falsified-If: an artifact the reader decoded from canonical bytes is written back as different bytes
    // Broiler-Human:        PENDING
    public static byte[] Write(UbcArtifact artifact)
    {
        var writer = new UbcArtifactWriter(artifact.Header);

        if (artifact.HasSection(UbcSectionKind.Families))
        {
            writer.Families(artifact.Families);
        }

        writer.Types(artifact.Types).Units(artifact.Units).Code(artifact.Code.AsSpan());

        if (artifact.HasSection(UbcSectionKind.JumpTables))
        {
            writer.JumpTables(artifact.JumpTables);
        }

        if (artifact.HasSection(UbcSectionKind.Regions))
        {
            writer.Regions(artifact.Regions);
        }

        writer.Entries(artifact.Entries);

        if (artifact.HasSection(UbcSectionKind.Positions))
        {
            writer.Positions(artifact.Positions);
        }

        foreach (var data in artifact.FamilyData)
        {
            writer.FamilyData(data.Slot, data.Body.AsSpan());
        }

        if (artifact.Emission is { } emission)
        {
            writer.Emission(emission);
        }

        return writer.ToArray();
    }
}

/// <summary>
/// The body encoding of every section the universal bytecode owns: the one place the writer's side
/// of Appendix E is stated. Public so that a corpus author can take a well-formed body and damage it.
/// </summary>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Low; Resources=1; Fingerprint=B0D207
// Broiler-Falsified-If: a body encoded here is read back by the reader as different rows
// Broiler-Human:        PENDING
public static class UbcSectionEncoder
{
    /// <summary>A canonical LEB128 encoding of <paramref name="value"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=6588EC
    // Broiler-Falsified-If: a value is written with a redundant continuation byte, so the reader's canonical check refuses it
    // Broiler-Human:        PENDING
    public static void WriteVarUInt(List<byte> output, ulong value)
    {
        do
        {
            var group = (byte)(value & 0x7F);
            value >>= 7;

            if (value != 0)
            {
                group |= 0x80;
            }

            output.Add(group);
        }
        while (value != 0);
    }

    /// <summary>An identity string: its UTF-8 length, then its bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=7C1670
    // Broiler-Human:        PENDING
    public static void WriteIdentity(List<byte> output, string identity)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(identity);
        WriteVarUInt(output, (ulong)bytes.Length);
        output.AddRange(bytes);
    }

    /// <summary>A Families body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CA4E57
    // Broiler-Human:        PENDING
    public static byte[] Families(IEnumerable<UbcFamilyEntry> rows)
    {
        var list = new List<UbcFamilyEntry>(rows);
        var output = new List<byte>();
        WriteVarUInt(output, (ulong)list.Count);

        foreach (var row in list)
        {
            WriteVarUInt(output, row.Slot);
            WriteIdentity(output, row.Identity);
            WriteVarUInt(output, row.TableVersion);
            WriteIdentity(output, row.Manifest);
        }

        return output.ToArray();
    }

    /// <summary>A Types body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=234EA5
    // Broiler-Human:        PENDING
    public static byte[] Types(IEnumerable<UbcSignature> rows)
    {
        var list = new List<UbcSignature>(rows);
        var output = new List<byte>();
        WriteVarUInt(output, (ulong)list.Count);

        foreach (var row in list)
        {
            WriteSlotTypes(output, row.Parameters);
            WriteSlotTypes(output, row.Results);
        }

        return output.ToArray();
    }

    /// <summary>A Units body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=90DC32
    // Broiler-Human:        PENDING
    public static byte[] Units(IEnumerable<UbcUnit> rows)
    {
        var list = new List<UbcUnit>(rows);
        var output = new List<byte>();
        WriteVarUInt(output, (ulong)list.Count);

        foreach (var row in list)
        {
            WriteVarUInt(output, row.TypeIndex);
            WriteVarUInt(output, row.FamilySlot);
            WriteVarUInt(output, (ulong)row.Locals.Length);

            foreach (var run in row.Locals)
            {
                WriteVarUInt(output, run.Count);
                output.Add((byte)run.Type);
            }

            WriteVarUInt(output, row.MaxWordHeight);
            WriteVarUInt(output, row.MaxValueHeight);
            WriteVarUInt(output, row.CodeOffset);
            WriteVarUInt(output, row.CodeLength);
            WriteVarUInt(output, (ulong)row.Flags);
            WriteOffsets(output, row.Landings);
        }

        return output.ToArray();
    }

    /// <summary>A JumpTables body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4C3B19
    // Broiler-Human:        PENDING
    public static byte[] JumpTables(IEnumerable<UbcJumpTable> rows)
    {
        var list = new List<UbcJumpTable>(rows);
        var output = new List<byte>();
        WriteVarUInt(output, (ulong)list.Count);

        foreach (var row in list)
        {
            WriteVarUInt(output, row.Unit);
            WriteOffsets(output, row.Targets);
        }

        return output.ToArray();
    }

    /// <summary>A Regions body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8EAA5B
    // Broiler-Human:        PENDING
    public static byte[] Regions(IEnumerable<UbcRegion> rows)
    {
        var list = new List<UbcRegion>(rows);
        var output = new List<byte>();
        WriteVarUInt(output, (ulong)list.Count);

        foreach (var row in list)
        {
            WriteVarUInt(output, row.Unit);
            WriteVarUInt(output, row.Start);
            WriteVarUInt(output, row.End);
            WriteVarUInt(output, row.Handler);
            WriteVarUInt(output, row.WordEntryHeight);
            WriteVarUInt(output, row.ValueEntryHeight);
            output.Add(row.Kind);
        }

        return output.ToArray();
    }

    /// <summary>An Entries body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ADF3C9
    // Broiler-Human:        PENDING
    public static byte[] Entries(IEnumerable<UbcEntry> rows)
    {
        var list = new List<UbcEntry>(rows);
        var output = new List<byte>();
        WriteVarUInt(output, (ulong)list.Count);

        foreach (var row in list)
        {
            WriteVarUInt(output, (ulong)row.Name.Length);
            output.AddRange(row.Name);
            WriteVarUInt(output, row.Unit);
        }

        return output.ToArray();
    }

    /// <summary>A Positions body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AD129C
    // Broiler-Human:        PENDING
    public static byte[] Positions(IEnumerable<UbcPosition> rows)
    {
        var list = new List<UbcPosition>(rows);
        var output = new List<byte>();
        WriteVarUInt(output, (ulong)list.Count);

        foreach (var row in list)
        {
            WriteVarUInt(output, row.Unit);
            WriteVarUInt(output, row.Offset);
            WriteVarUInt(output, (uint)row.Coordinate0);
            WriteVarUInt(output, (uint)row.Coordinate1);
        }

        return output.ToArray();
    }

    /// <summary>An Emission body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=607DE3
    // Broiler-Human:        PENDING
    public static byte[] Emission(UbcEmission emission)
    {
        var output = new List<byte>();
        WriteIdentity(output, emission.Form);
        WriteVarUInt(output, emission.EmitterVersion);
        WriteVarUInt(output, emission.Alignment);

        for (var index = 0; index < 8; index++)
        {
            output.Add((byte)(emission.ShapeHash >> (8 * index)));
        }

        var length = (uint)emission.Bytes.Length;

        for (var index = 0; index < 4; index++)
        {
            output.Add((byte)(length >> (8 * index)));
        }

        output.AddRange(emission.Bytes);
        WriteVarUInt(output, (ulong)emission.Symbols.Length);

        foreach (var symbol in emission.Symbols)
        {
            WriteVarUInt(output, symbol.Unit);
            WriteVarUInt(output, symbol.Offset);
        }

        return output.ToArray();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=3E94CC
    // Broiler-Human:        PENDING
    private static void WriteSlotTypes(List<byte> output, ImmutableArray<UbcSlotType> types)
    {
        WriteVarUInt(output, (ulong)types.Length);

        foreach (var type in types)
        {
            output.Add((byte)type);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=750302
    // Broiler-Human:        PENDING
    private static void WriteOffsets(List<byte> output, ImmutableArray<uint> offsets)
    {
        WriteVarUInt(output, (ulong)offsets.Length);

        foreach (var offset in offsets)
        {
            WriteVarUInt(output, offset);
        }
    }
}

/// <summary>
/// An instruction assembler for translators and tests: it encodes common and family instructions and
/// patches code targets to absolute offsets, and checks nothing a walk would check.
/// </summary>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Low; Resources=1; Fingerprint=A73B15
// Broiler-Falsified-If: an instruction is encoded with a width other than its table's, or a label is patched to an offset other than its mark plus the base
// Broiler-Human:        PENDING
public sealed class UbcCodeBuilder
{
    private readonly List<byte> bytes = new();
    private readonly List<int> marks = new();
    private readonly List<(int At, int Label)> fixups = new();

    /// <summary>A builder whose first byte will sit at <paramref name="baseOffset"/> in the Code section.</summary>
    public UbcCodeBuilder(uint baseOffset = 0) => BaseOffset = baseOffset;

    /// <summary>The Code-section offset of the builder's first byte.</summary>
    public uint BaseOffset { get; }

    /// <summary>The absolute offset the next instruction will have.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=645A8F
    // Broiler-Human:        PENDING
    public uint Offset => BaseOffset + (uint)bytes.Count;

    /// <summary>A new label, unmarked.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=BD3110
    // Broiler-Human:        PENDING
    public int NewLabel()
    {
        marks.Add(-1);
        return marks.Count - 1;
    }

    /// <summary>Marks <paramref name="label"/> at the current offset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=79C23C
    // Broiler-Human:        PENDING
    public UbcCodeBuilder Mark(int label)
    {
        marks[label] = bytes.Count;
        return this;
    }

    /// <summary>A common instruction; <paramref name="operand"/> is written at the row's operand width.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B9B42F
    // Broiler-Human:        PENDING
    public UbcCodeBuilder Emit(UbcOpcode opcode, ulong operand = 0)
    {
        var row = UbcOpcodes.Row(opcode);
        bytes.Add((byte)opcode);
        Append(operand, UbcOperandShapes.Width(row.Shape));
        return this;
    }

    /// <summary>A common instruction whose <c>U32</c> operand is <paramref name="label"/>'s absolute offset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=95671D
    // Broiler-Human:        PENDING
    public UbcCodeBuilder EmitTo(UbcOpcode opcode, int label)
    {
        // The width comes from the one table like every other instruction's: a row that takes no code
        // target is an author's mistake here, not a four-byte operand the table does not declare.
        if (!UbcOpcodes.Row(opcode).HasCodeTarget)
        {
            throw new System.ArgumentException($"{opcode} takes no code target", nameof(opcode));
        }

        bytes.Add((byte)opcode);
        fixups.Add((bytes.Count, label));
        Append(0, 4);
        return this;
    }

    /// <summary>A family instruction of <paramref name="slot"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=7527DA
    // Broiler-Human:        PENDING
    public UbcCodeBuilder EmitFamily(byte slot, byte opcode, UbcOperandShape shape, ulong operand = 0)
    {
        bytes.Add((byte)(UbcFormat.FamilyPrefixBase + slot));
        bytes.Add(opcode);
        Append(operand, UbcOperandShapes.Width(shape));
        return this;
    }

    /// <summary>A family instruction of <paramref name="slot"/> whose <c>U32</c> operand is <paramref name="label"/>'s offset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=500B77
    // Broiler-Human:        PENDING
    public UbcCodeBuilder EmitFamilyTo(byte slot, byte opcode, int label)
    {
        bytes.Add((byte)(UbcFormat.FamilyPrefixBase + slot));
        bytes.Add(opcode);
        fixups.Add((bytes.Count, label));
        Append(0, 4);
        return this;
    }

    /// <summary>Raw bytes, for a corpus entry that needs a byte no instruction encodes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AF7855
    // Broiler-Human:        PENDING
    public UbcCodeBuilder Raw(params byte[] raw)
    {
        bytes.AddRange(raw);
        return this;
    }

    /// <summary>The bytes, with every label patched. An unmarked label is an author's defect and throws.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=655750
    // Broiler-Human:        PENDING
    public byte[] ToArray()
    {
        var output = bytes.ToArray();

        foreach (var (at, label) in fixups)
        {
            if (marks[label] < 0)
            {
                throw new System.InvalidOperationException($"label {label} was never marked");
            }

            UbcOperandShapes.Write(System.MemoryExtensions.AsSpan(output, at, 4), BaseOffset + (uint)marks[label], 4);
        }

        return output;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=BAD2E6
    // Broiler-Human:        PENDING
    private void Append(ulong operand, int width)
    {
        for (var index = 0; index < width; index++)
        {
            bytes.Add((byte)(operand >> (8 * index)));
        }
    }
}
