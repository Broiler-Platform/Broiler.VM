namespace Com.Example.Calculator;

/// <summary>
/// Builds calculator artifacts.
/// </summary>
/// <remarks>
/// <para>
/// An encoder belongs beside the decoder that reads it, in the profile's own assembly. Putting it in
/// the composition root instead would give a host knowledge of a guest format it has no business
/// holding, and putting it in a shared helper would give two profiles one format.
/// </para>
/// <para>
/// It uses no Broiler type at all, which is the point worth noticing: writing an artifact is not a
/// core operation, so nothing in the core is involved in it. The core sees the bytes for the first
/// time when someone asks it to verify them.
/// </para>
/// </remarks>
public static class CalculatorArtifactWriter
{
    /// <summary>An artifact that evaluates to <paramref name="value"/>.</summary>
    public static byte[] Constant(long value) =>
        Write([value], [CalculatorFormat.TokenPush, 0, CalculatorFormat.TokenHalt]);

    /// <summary>An artifact that adds two operands.</summary>
    public static byte[] Sum(long left, long right) =>
        Write(
            [left, right],
            [
                CalculatorFormat.TokenPush, 0,
                CalculatorFormat.TokenPush, 1,
                CalculatorFormat.TokenAdd,
                CalculatorFormat.TokenHalt,
            ]);

    /// <summary>An artifact that multiplies two operands.</summary>
    public static byte[] Product(long left, long right) =>
        Write(
            [left, right],
            [
                CalculatorFormat.TokenPush, 0,
                CalculatorFormat.TokenPush, 1,
                CalculatorFormat.TokenMultiply,
                CalculatorFormat.TokenHalt,
            ]);

    /// <summary>An artifact that divides one operand by another, faulting when the divisor is zero.</summary>
    public static byte[] Quotient(long dividend, long divisor) =>
        Write(
            [dividend, divisor],
            [
                CalculatorFormat.TokenPush, 0,
                CalculatorFormat.TokenPush, 1,
                CalculatorFormat.TokenDivide,
                CalculatorFormat.TokenHalt,
            ]);

    /// <summary>An artifact whose evaluation stack reaches <paramref name="depth"/>.</summary>
    /// <remarks>
    /// A depth above the profile's own maximum is refused at verification, which is what makes the
    /// executor's fixed-size stack safe. Being able to ask for one is how that refusal gets tested.
    /// </remarks>
    public static byte[] DeepStack(int depth)
    {
        var tokens = new byte[(depth * 2) + 1];

        for (var index = 0; index < depth; index++)
        {
            tokens[index * 2] = CalculatorFormat.TokenPush;
            tokens[(index * 2) + 1] = 0;
        }

        tokens[^1] = CalculatorFormat.TokenHalt;
        return Write([1], tokens);
    }

    /// <summary>Writes an artifact from an operand pool and a token stream, checking neither.</summary>
    /// <remarks>
    /// Deliberately unchecked, so a test can write bytes the verifier must refuse. A writer that
    /// could only produce valid artifacts would make every rejection path unreachable.
    /// </remarks>
    public static byte[] Write(long[] operands, byte[] tokens)
    {
        var buffer = new System.Collections.Generic.List<byte>(32 + (operands.Length * 4) + tokens.Length);

        buffer.AddRange(CalculatorFormat.Magic.ToArray());
        WriteVarUInt(buffer, CalculatorFormat.FormatVersion);
        WriteVarUInt(buffer, (ulong)operands.Length);

        for (var index = 0; index < operands.Length; index++)
        {
            WriteVarUInt(buffer, Encode(operands[index]));
        }

        WriteVarUInt(buffer, (ulong)tokens.Length);
        buffer.AddRange(tokens);

        return buffer.ToArray();
    }

    /// <summary>Zigzags a signed operand, so a small negative costs one byte rather than ten.</summary>
    public static ulong Encode(long value) => (ulong)((value << 1) ^ (value >> 63));

    private static void WriteVarUInt(System.Collections.Generic.List<byte> buffer, ulong value)
    {
        while (value >= 0x80)
        {
            buffer.Add((byte)(value | 0x80));
            value >>= 7;
        }

        buffer.Add((byte)value);
    }
}
