namespace Com.Example.Calculator;

/// <summary>
/// The calculator profile's artifact format: a header, a pool of signed operands, and a flat
/// program of one-byte tokens.
/// </summary>
/// <remarks>
/// <para>
/// It is deliberately unlike the core's own fixture format, and the difference is the point of the
/// milestone. The fixture uses length-framed sections and a stack machine with an operand byte per
/// instruction; this uses no framing at all, a zigzag signed encoding the fixture does not have,
/// and a token set that is not a subset of the fixture's. A second profile that quietly reused the
/// first one's shape would demonstrate that the core supports one format twice.
/// </para>
/// <para>
/// Nothing here is a core concept. The core neither names nor interprets a token, an operand or the
/// magic; it carries bytes to this profile's verifier and a typed payload back.
/// </para>
/// </remarks>
public static class CalculatorFormat
{
    /// <summary>The four magic bytes every calculator artifact starts with.</summary>
    public static System.ReadOnlySpan<byte> Magic => "CALC"u8;

    /// <summary>The only profile-format version this verifier accepts.</summary>
    public const uint FormatVersion = 1;

    /// <summary>Push operand <c>n</c>, where <c>n</c> is the following byte.</summary>
    public const byte TokenPush = 0x10;

    /// <summary>Pop two, push their sum.</summary>
    public const byte TokenAdd = 0x20;

    /// <summary>Pop two, push their product.</summary>
    public const byte TokenMultiply = 0x21;

    /// <summary>Pop one, push its negation.</summary>
    public const byte TokenNegate = 0x22;

    /// <summary>Pop two, push the first divided by the second. Division by zero is a language fault.</summary>
    public const byte TokenDivide = 0x23;

    /// <summary>Stop, leaving the top of the stack as the answer.</summary>
    public const byte TokenHalt = 0x30;

    /// <summary>The deepest the evaluation stack may grow. A program needing more is refused.</summary>
    public const int MaximumStackDepth = 32;
}
