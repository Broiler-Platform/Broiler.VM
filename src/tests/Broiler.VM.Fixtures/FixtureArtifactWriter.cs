namespace Broiler.VM.Fixtures;

/// <summary>
/// Builds fixture artifacts, well formed and deliberately malformed.
/// </summary>
/// <remarks>
/// The malformed shapes exist so that the failure taxonomy is exercised against real bytes rather
/// than against a mocked verifier. A truncated artifact and an over-declared count fail in different
/// ways for different reasons, and a corpus that could not tell them apart would not be evidence of
/// anything.
/// </remarks>
public static class FixtureArtifactWriter
{
    /// <summary>Writes a well-formed artifact.</summary>
    public static byte[] Write(long[] constants, byte[] code) =>
        Build(constants, code, Corruption.None);

    /// <summary>Writes an artifact damaged in the named way.</summary>
    public static byte[] Write(long[] constants, byte[] code, Corruption corruption) =>
        Build(constants, code, corruption);

    /// <summary>The simplest artifact that returns a number.</summary>
    public static byte[] Constant(long value) =>
        Write([value], [FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn]);

    /// <summary>An artifact that adds two constants and returns the sum.</summary>
    public static byte[] Sum(long left, long right) =>
        Write(
            [left, right],
            [
                FixtureFormat.OpPushConst, 0,
                FixtureFormat.OpPushConst, 1,
                FixtureFormat.OpAdd,
                FixtureFormat.OpReturn,
            ]);

    /// <summary>An artifact that yields once and then returns a constant.</summary>
    public static byte[] YieldThenConstant(long value) =>
        Write(
            [value],
            [
                FixtureFormat.OpYield,
                FixtureFormat.OpPushConst, 0,
                FixtureFormat.OpReturn,
            ]);

    /// <summary>An artifact that faults with the given code.</summary>
    public static byte[] Fault(long code) =>
        Write([code], [FixtureFormat.OpFault, 0]);

    /// <summary>An artifact that requests one guest-initiated load and returns its result count.</summary>
    public static byte[] LoadThenConstant(long specifier, long value) =>
        Write(
            [specifier, value],
            [
                FixtureFormat.OpLoad, 0,
                FixtureFormat.OpPushConst, 1,
                FixtureFormat.OpReturn,
            ]);

    /// <summary>An artifact that calls host binding zero with one argument and returns the answer.</summary>
    public static byte[] HostCall(long argument, int bindingIndex) =>
        Write(
            [argument],
            [
                FixtureFormat.OpPushConst, 0,
                FixtureFormat.OpHostCall, (byte)bindingIndex,
                FixtureFormat.OpReturn,
            ]);

    /// <summary>An artifact that burns work without polling, breaking a declared poll bound.</summary>
    public static byte[] Spin(long units) =>
        Write([units], [FixtureFormat.OpSpin, 0, FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn]);

    /// <summary>An artifact that allocates from a declared count.</summary>
    public static byte[] Allocate(long elements) =>
        Write([elements], [FixtureFormat.OpAllocate, 0, FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn]);

    /// <summary>An artifact that retains and then releases a number of bytes.</summary>
    public static byte[] RetainThenRelease(long bytes) =>
        Write(
            [bytes],
            [
                FixtureFormat.OpRetain, 0,
                FixtureFormat.OpRelease, 0,
                FixtureFormat.OpPushConst, 0,
                FixtureFormat.OpReturn,
            ]);

    /// <summary>How an artifact is deliberately damaged.</summary>
    public enum Corruption
    {
        /// <summary>Not damaged.</summary>
        None = 0,

        /// <summary>The last byte is missing, so a section runs off the end.</summary>
        Truncated = 1,

        /// <summary>The magic number is wrong.</summary>
        BadMagic = 2,

        /// <summary>The format version is one the profile does not accept.</summary>
        UnknownFormatVersion = 3,

        /// <summary>The constant count is far larger than the pool that follows.</summary>
        OverDeclaredCount = 4,

        /// <summary>A section declares a length that does not match its body.</summary>
        SectionLengthMismatch = 5,

        /// <summary>More sections are declared than the artifact contains.</summary>
        OverDeclaredSectionCount = 6,

        /// <summary>A variable-length integer is encoded non-canonically.</summary>
        NonCanonicalVarInt = 7,
    }

    private static byte[] Build(long[] constants, byte[] code, Corruption corruption)
    {
        var buffer = new System.Collections.Generic.List<byte>(64);

        if (corruption is Corruption.BadMagic)
        {
            buffer.AddRange("XXXX"u8.ToArray());
        }
        else
        {
            buffer.AddRange(FixtureFormat.Magic.ToArray());
        }

        WriteVarUInt(buffer, corruption is Corruption.UnknownFormatVersion ? 99u : FixtureFormat.FormatVersion);

        WriteVarUInt(buffer, corruption is Corruption.OverDeclaredSectionCount ? 9u : 2u);

        var constantBody = new System.Collections.Generic.List<byte>(constants.Length * 2);

        WriteVarUInt(
            constantBody,
            corruption is Corruption.OverDeclaredCount ? 4_000_000_000u : (uint)constants.Length);

        foreach (var constant in constants)
        {
            if (corruption is Corruption.NonCanonicalVarInt)
            {
                // A value encoded with a redundant continuation group. Two encodings of one value
                // would make a byte-identical artifact check meaningless, so the reader rejects it.
                constantBody.Add(0x80);
                constantBody.Add(0x00);
                continue;
            }

            WriteVarUInt64(constantBody, unchecked((ulong)constant));
        }

        WriteSection(buffer, FixtureFormat.SectionConstants, constantBody, corruption);

        var codeBody = new System.Collections.Generic.List<byte>(code.Length + 4);
        WriteVarUInt(codeBody, (uint)code.Length);
        codeBody.AddRange(code);

        WriteSection(buffer, FixtureFormat.SectionCode, codeBody, corruption);

        var bytes = buffer.ToArray();

        if (corruption is Corruption.Truncated && bytes.Length > 1)
        {
            var truncated = new byte[bytes.Length - 1];
            System.Array.Copy(bytes, truncated, truncated.Length);
            return truncated;
        }

        return bytes;
    }

    private static void WriteSection(
        System.Collections.Generic.List<byte> buffer,
        byte kind,
        System.Collections.Generic.List<byte> body,
        Corruption corruption)
    {
        buffer.Add(kind);

        WriteVarUInt64(
            buffer,
            corruption is Corruption.SectionLengthMismatch
                ? (ulong)body.Count + 3
                : (ulong)body.Count);

        buffer.AddRange(body);
    }

    private static void WriteVarUInt(System.Collections.Generic.List<byte> buffer, uint value) =>
        WriteVarUInt64(buffer, value);

    private static void WriteVarUInt64(System.Collections.Generic.List<byte> buffer, ulong value)
    {
        while (true)
        {
            var group = (byte)(value & 0x7F);
            value >>= 7;

            if (value == 0)
            {
                buffer.Add(group);
                return;
            }

            buffer.Add((byte)(group | 0x80));
        }
    }
}
