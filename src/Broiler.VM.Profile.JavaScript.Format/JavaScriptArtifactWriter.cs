// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           3
// Human-reviewed:   0/17
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The encoder for format version 1.
/// </summary>
/// <remarks>
/// <para>
/// It sits beside the decoder it feeds, in the format assembly, because an encoder and a decoder
/// that disagree about a width are a defect neither of them can find alone. Nothing here is a
/// core operation and no Broiler contract type appears in this file: writing an artifact is not
/// something the core participates in, and it sees the bytes for the first time when someone asks
/// it to verify them.
/// </para>
/// <para>
/// <b>It is deliberately unchecked.</b> Every section body is handed in as bytes, every count is
/// written as given, and no method here refuses to emit something the verifier will reject. A
/// writer that could only produce valid artifacts would make every rejection path in the verifier
/// unreachable, and a retained malformed corpus would have nothing to retain.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=2B216E
// Broiler-Human:        PENDING
public static class JavaScriptArtifactWriter
{
    /// <summary>One section, as a kind and an already-encoded body.</summary>
    /// <remarks>
    /// The body is opaque here on purpose. A corpus entry that needs a truncated constant pool or
    /// a code section whose last instruction runs off the end writes those bytes itself, and this
    /// writer frames them without an opinion.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=05287A
    // Broiler-Human:        PENDING
    public readonly struct Section
    {
        /// <summary>Creates a section of <paramref name="kind"/> carrying <paramref name="body"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=2336CD
        // Broiler-Human:        PENDING
        public Section(JavaScriptFormat.SectionKind kind, byte[] body)
        {
            Kind = kind;
            Body = body;
        }

        /// <summary>Which section this is.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=60C0BA
        // Broiler-Human:        PENDING
        public JavaScriptFormat.SectionKind Kind { get; }

        /// <summary>Its already-encoded body.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=C9FD1F
        // Broiler-Human:        PENDING
        public byte[] Body { get; }
    }

    /// <summary>
    /// Frames a whole artifact: magic, format version, feature-manifest identity, section count,
    /// then each section as a kind, a length and a body.
    /// </summary>
    /// <remarks>
    /// The declared section count is a parameter rather than <c>sections.Length</c> so that a
    /// corpus entry can declare a count that disagrees with what follows. That disagreement is one
    /// of the framing failures the verifier has to answer for, and it cannot be tested through a
    /// writer that computes the count itself.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=EF30A6
    // Broiler-Human:        PENDING
    public static byte[] Write(
        string manifestId,
        Section[] sections,
        uint formatVersion = JavaScriptFormat.FormatVersion,
        byte[]? magic = null,
        uint? declaredSectionCount = null)
    {
        var buffer = new System.Collections.Generic.List<byte>(256);

        buffer.AddRange(magic ?? JavaScriptFormat.Magic.ToArray());
        WriteVarUInt(buffer, formatVersion);

        var manifestBytes = System.Text.Encoding.UTF8.GetBytes(manifestId);
        WriteVarUInt(buffer, (ulong)manifestBytes.Length);
        buffer.AddRange(manifestBytes);

        WriteVarUInt(buffer, declaredSectionCount ?? (ulong)sections.Length);

        foreach (var section in sections)
        {
            WriteVarUInt(buffer, (ulong)section.Kind);
            WriteVarUInt(buffer, (ulong)section.Body.Length);
            buffer.AddRange(section.Body);
        }

        return buffer.ToArray();
    }

    /// <summary>Encodes a limits section body.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=3FFDF3
    // Broiler-Human:        PENDING
    public static byte[] Limits(uint maxOperandStack, uint maxLocals, uint maxFrames, uint maxConstants)
    {
        var buffer = new System.Collections.Generic.List<byte>(8);
        WriteVarUInt(buffer, maxOperandStack);
        WriteVarUInt(buffer, maxLocals);
        WriteVarUInt(buffer, maxFrames);
        WriteVarUInt(buffer, maxConstants);
        return buffer.ToArray();
    }

    /// <summary>
    /// Encodes a constant-pool body from already-encoded entries.
    /// </summary>
    /// <remarks>
    /// <paramref name="declaredCount"/> is separate from <paramref name="entries"/> for the same
    /// reason the section count is: a pool that declares more entries than it carries is a
    /// truncation the verifier must answer for, and a writer that derived the count could not
    /// produce one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=154570
    // Broiler-Human:        PENDING
    public static byte[] Constants(byte[][] entries, uint? declaredCount = null)
    {
        var buffer = new System.Collections.Generic.List<byte>(16 + (entries.Length * 9));
        WriteVarUInt(buffer, declaredCount ?? (ulong)entries.Length);

        foreach (var entry in entries)
        {
            buffer.AddRange(entry);
        }

        return buffer.ToArray();
    }

    /// <summary>One <c>undefined</c> constant-pool entry.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=00A942
    // Broiler-Human:        PENDING
    public static byte[] UndefinedConstant() => [(byte)JavaScriptFormat.ConstantTag.Undefined];

    /// <summary>One Boolean constant-pool entry. The payload byte is written as given.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=F6251C
    // Broiler-Human:        PENDING
    public static byte[] BooleanConstant(byte payload) =>
        [(byte)JavaScriptFormat.ConstantTag.Boolean, payload];

    /// <summary>One Number constant-pool entry, IEEE 754 binary64, little-endian.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=199220
    // Broiler-Human:        PENDING
    public static byte[] NumberConstant(double value)
    {
        var bytes = new byte[9];
        bytes[0] = (byte)JavaScriptFormat.ConstantTag.Number;
        System.Buffers.Binary.BinaryPrimitives.WriteDoubleLittleEndian(
            System.MemoryExtensions.AsSpan(bytes, 1), value);
        return bytes;
    }

    /// <summary>
    /// One interned-name constant-pool entry: the tag this format reserves and no manifest admits.
    /// </summary>
    /// <remarks>
    /// It exists so that "a construct outside the declared manifest is refused at verification and
    /// not at first execution" has something to refuse. Without a way to write one, that rule
    /// would be a sentence.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=6052BB
    // Broiler-Human:        PENDING
    public static byte[] InternedNameConstant(string name)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(name);
        var buffer = new System.Collections.Generic.List<byte>(bytes.Length + 6)
        {
            (byte)JavaScriptFormat.ConstantTag.InternedName,
        };

        WriteVarUInt(buffer, (ulong)bytes.Length);
        buffer.AddRange(bytes);
        return buffer.ToArray();
    }

    /// <summary>Encodes an entry-point table body.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=34F9EE
    // Broiler-Human:        PENDING
    public static byte[] Entries((string Name, uint CodeOffset)[] entries, uint? declaredCount = null)
    {
        var buffer = new System.Collections.Generic.List<byte>(16 + (entries.Length * 16));
        WriteVarUInt(buffer, declaredCount ?? (ulong)entries.Length);

        foreach (var (name, codeOffset) in entries)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(name);
            WriteVarUInt(buffer, (ulong)bytes.Length);
            buffer.AddRange(bytes);
            WriteVarUInt(buffer, codeOffset);
        }

        return buffer.ToArray();
    }

    /// <summary>Encodes a position-table body.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=21DDC1
    // Broiler-Human:        PENDING
    public static byte[] Positions((uint Offset, uint Line, uint Column)[] rows, uint? declaredCount = null)
    {
        var buffer = new System.Collections.Generic.List<byte>(8 + (rows.Length * 6));
        WriteVarUInt(buffer, declaredCount ?? (ulong)rows.Length);

        foreach (var (offset, line, column) in rows)
        {
            WriteVarUInt(buffer, offset);
            WriteVarUInt(buffer, line);
            WriteVarUInt(buffer, column);
        }

        return buffer.ToArray();
    }

    /// <summary>Encodes a count-only body: the shape both reserved sections take at version 1.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=F67FC3
    // Broiler-Human:        PENDING
    public static byte[] Count(uint count)
    {
        var buffer = new System.Collections.Generic.List<byte>(5);
        WriteVarUInt(buffer, count);
        return buffer.ToArray();
    }

    /// <summary>Appends one operand-free instruction.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=02E965
    // Broiler-Human:        PENDING
    public static void Emit(System.Collections.Generic.List<byte> code, JavaScriptOpcode opcode) =>
        code.Add((byte)opcode);

    /// <summary>Appends one instruction carrying a <c>u16</c> slot index, little-endian.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=EDF94B
    // Broiler-Human:        PENDING
    public static void Emit(System.Collections.Generic.List<byte> code, JavaScriptOpcode opcode, ushort index)
    {
        code.Add((byte)opcode);
        code.Add((byte)(index & 0xFF));
        code.Add((byte)(index >> 8));
    }

    /// <summary>
    /// Appends one jump carrying a signed 32-bit displacement, and answers where the displacement
    /// was written so a forward jump can be patched once its target is known.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=09126B
    // Broiler-Human:        PENDING
    public static int EmitJump(
        System.Collections.Generic.List<byte> code,
        JavaScriptOpcode opcode,
        int displacement = 0)
    {
        code.Add((byte)opcode);
        var at = code.Count;

        for (var shift = 0; shift < 32; shift += 8)
        {
            code.Add((byte)((displacement >> shift) & 0xFF));
        }

        return at;
    }

    /// <summary>Overwrites a displacement written earlier by <see cref="EmitJump"/>.</summary>
    /// <remarks>
    /// The displacement is relative to the offset of the instruction FOLLOWING the jump, which is
    /// the convention the verifier and the executor both read it under. Stating it in one place
    /// and computing it in another is how the two ends of a jump stop agreeing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=55F769
    // Broiler-Human:        PENDING
    public static void PatchJump(System.Collections.Generic.List<byte> code, int at, int target)
    {
        var displacement = target - (at + 4);

        for (var shift = 0; shift < 32; shift += 8)
        {
            code[at + (shift / 8)] = (byte)((displacement >> shift) & 0xFF);
        }
    }

    /// <summary>Writes an unsigned value in the core reader's variable-length encoding.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=92CB5F
    // Broiler-Human:        PENDING
    public static void WriteVarUInt(System.Collections.Generic.List<byte> buffer, ulong value)
    {
        while (value >= 0x80)
        {
            buffer.Add((byte)(value | 0x80));
            value >>= 7;
        }

        buffer.Add((byte)value);
    }
}
