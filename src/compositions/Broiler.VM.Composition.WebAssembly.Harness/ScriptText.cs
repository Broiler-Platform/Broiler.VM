using System.Globalization;
using System.Numerics;
using System.Text;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>A script or a module text the reader cannot read, with where it stopped.</summary>
/// <remarks>
/// A defect of the reader and a text the specification calls malformed look the same from here, and
/// the script runner answers both as the reader's refusal rather than as anything the profile said.
/// </remarks>
internal sealed class ScriptReadException : Exception
{
    internal ScriptReadException(string message)
        : base(message)
    {
    }

    internal ScriptReadException(string message, int line)
        : base($"line {line.ToString(CultureInfo.InvariantCulture)}: {message}")
    {
    }
}

/// <summary>What one atom of the text format is.</summary>
internal enum AtomKind
{
    /// <summary>A keyword, a number or any other run of identifier characters not starting with a dollar sign.</summary>
    Word,

    /// <summary>An identifier: a run of identifier characters starting with a dollar sign.</summary>
    Id,

    /// <summary>A string, held as the bytes its escapes denote.</summary>
    String,
}

/// <summary>One node of a script: an atom, or a parenthesised list of nodes.</summary>
internal sealed class SExpr
{
    private SExpr(int line, List<SExpr>? items, AtomKind kind, string text, byte[]? bytes)
    {
        Line = line;
        Items = items;
        Kind = kind;
        Text = text;
        Bytes = bytes;
    }

    /// <summary>The source line the node starts on, from one.</summary>
    internal int Line { get; }

    /// <summary>The list's members, or null for an atom.</summary>
    internal List<SExpr>? Items { get; }

    /// <summary>Whether this node is a list.</summary>
    internal bool IsList => Items is not null;

    /// <summary>An atom's kind.</summary>
    internal AtomKind Kind { get; }

    /// <summary>An atom's text as written; a string's is its decoded bytes read as UTF-8, for messages only.</summary>
    internal string Text { get; }

    /// <summary>A string's bytes.</summary>
    internal byte[]? Bytes { get; }

    /// <summary>The keyword a list opens with, or null when it opens with anything else.</summary>
    internal string? Head =>
        Items is { Count: > 0 } items && !items[0].IsList && items[0].Kind is AtomKind.Word ? items[0].Text : null;

    internal static SExpr List(int line, List<SExpr> items) => new(line, items, AtomKind.Word, string.Empty, null);

    internal static SExpr Atom(int line, AtomKind kind, string text, byte[]? bytes = null) =>
        new(line, null, kind, text, bytes);

    /// <summary>Whether this is the keyword <paramref name="word"/>.</summary>
    internal bool IsWord(string word) =>
        !IsList && Kind is AtomKind.Word && string.Equals(Text, word, StringComparison.Ordinal);

    public override string ToString() => IsList ? $"({Head ?? "..."} ...)" : Text;
}

/// <summary>
/// Reads the text format's tokens into lists: the script reader's first stage.
/// </summary>
/// <remarks>
/// <para>
/// <b>It reads bytes, not characters.</b> A string may hold any bytes its escapes or its raw text
/// denote, and a module's names are compared as those bytes, so nothing here decodes the file as
/// text. Outside strings and comments the format is ASCII.
/// </para>
/// <para>
/// <b>A token is a maximal run of identifier characters</b>, as the specification's lexical grammar
/// has it, and whether it is a keyword, a number or something malformed is decided where it is used.
/// </para>
/// </remarks>
internal static class ScriptText
{
    /// <summary>Reads every top-level form of a script.</summary>
    internal static List<SExpr> Read(byte[] source)
    {
        var position = 0;
        var line = 1;
        var forms = new List<SExpr>();

        while (true)
        {
            SkipSpace(source, ref position, ref line);

            if (position >= source.Length)
            {
                return forms;
            }

            forms.Add(ReadNode(source, ref position, ref line));
        }
    }

    private static SExpr ReadNode(byte[] source, ref int position, ref int line)
    {
        var start = line;
        var current = source[position];

        if (current == (byte)'(')
        {
            position++;
            var items = new List<SExpr>();

            while (true)
            {
                SkipSpace(source, ref position, ref line);

                if (position >= source.Length)
                {
                    throw new ScriptReadException("unclosed list", start);
                }

                if (source[position] == (byte)')')
                {
                    position++;
                    return SExpr.List(start, items);
                }

                items.Add(ReadNode(source, ref position, ref line));
            }
        }

        if (current == (byte)')')
        {
            throw new ScriptReadException("unexpected closing parenthesis", start);
        }

        if (current == (byte)'"')
        {
            var bytes = ReadString(source, ref position, ref line);
            return SExpr.Atom(start, AtomKind.String, Encoding.UTF8.GetString(bytes), bytes);
        }

        var from = position;

        while (position < source.Length && IsIdChar(source[position]))
        {
            position++;
        }

        if (position == from)
        {
            throw new ScriptReadException($"unexpected byte 0x{current:X2}", start);
        }

        var text = Encoding.ASCII.GetString(source, from, position - from);
        return SExpr.Atom(start, text[0] == '$' ? AtomKind.Id : AtomKind.Word, text);
    }

    private static void SkipSpace(byte[] source, ref int position, ref int line)
    {
        while (position < source.Length)
        {
            var current = source[position];

            if (current == (byte)'\n')
            {
                line++;
                position++;
            }
            else if (current <= 0x20)
            {
                position++;
            }
            else if (current == (byte)';' && position + 1 < source.Length && source[position + 1] == (byte)';')
            {
                while (position < source.Length && source[position] != (byte)'\n')
                {
                    position++;
                }
            }
            else if (current == (byte)'(' && position + 1 < source.Length && source[position + 1] == (byte)';')
            {
                SkipBlockComment(source, ref position, ref line);
            }
            else
            {
                return;
            }
        }
    }

    private static void SkipBlockComment(byte[] source, ref int position, ref int line)
    {
        var start = line;
        var depth = 0;

        while (position < source.Length)
        {
            if (source[position] == (byte)'(' && position + 1 < source.Length && source[position + 1] == (byte)';')
            {
                depth++;
                position += 2;
            }
            else if (source[position] == (byte)';' && position + 1 < source.Length && source[position + 1] == (byte)')')
            {
                depth--;
                position += 2;

                if (depth == 0)
                {
                    return;
                }
            }
            else
            {
                if (source[position] == (byte)'\n')
                {
                    line++;
                }

                position++;
            }
        }

        throw new ScriptReadException("unclosed block comment", start);
    }

    private static byte[] ReadString(byte[] source, ref int position, ref int line)
    {
        var start = line;
        var bytes = new List<byte>();
        position++;

        while (true)
        {
            if (position >= source.Length)
            {
                throw new ScriptReadException("unclosed string", start);
            }

            var current = source[position++];

            if (current == (byte)'"')
            {
                return [.. bytes];
            }

            if (current == (byte)'\n')
            {
                line++;
            }

            if (current != (byte)'\\')
            {
                bytes.Add(current);
                continue;
            }

            if (position >= source.Length)
            {
                throw new ScriptReadException("unclosed escape", start);
            }

            var escape = source[position++];

            switch (escape)
            {
                case (byte)'t': bytes.Add((byte)'\t'); break;
                case (byte)'n': bytes.Add((byte)'\n'); break;
                case (byte)'r': bytes.Add((byte)'\r'); break;
                case (byte)'"': bytes.Add((byte)'"'); break;
                case (byte)'\'': bytes.Add((byte)'\''); break;
                case (byte)'\\': bytes.Add((byte)'\\'); break;

                case (byte)'u':
                    if (position >= source.Length || source[position] != (byte)'{')
                    {
                        throw new ScriptReadException("malformed unicode escape", start);
                    }

                    position++;
                    var scalar = 0;
                    var digits = 0;

                    while (position < source.Length && source[position] != (byte)'}')
                    {
                        if (source[position] == (byte)'_')
                        {
                            position++;
                            continue;
                        }

                        scalar = checked((scalar * 16) + HexValue(source[position++], start));
                        digits++;
                    }

                    position++;

                    if (digits == 0 || scalar > 0x10FFFF || scalar is >= 0xD800 and <= 0xDFFF)
                    {
                        throw new ScriptReadException("malformed unicode escape", start);
                    }

                    bytes.AddRange(Encoding.UTF8.GetBytes(char.ConvertFromUtf32(scalar)));
                    break;

                default:
                    if (position >= source.Length)
                    {
                        throw new ScriptReadException("unclosed escape", start);
                    }

                    bytes.Add((byte)((HexValue(escape, start) * 16) + HexValue(source[position++], start)));
                    break;
            }
        }
    }

    private static int HexValue(byte digit, int line) => digit switch
    {
        >= (byte)'0' and <= (byte)'9' => digit - '0',
        >= (byte)'a' and <= (byte)'f' => digit - 'a' + 10,
        >= (byte)'A' and <= (byte)'F' => digit - 'A' + 10,
        _ => throw new ScriptReadException($"not a hexadecimal digit: 0x{digit:X2}", line),
    };

    private static bool IsIdChar(byte current) =>
        current is >= (byte)'0' and <= (byte)'9'
            or >= (byte)'a' and <= (byte)'z'
            or >= (byte)'A' and <= (byte)'Z'
            or (byte)'!' or (byte)'#' or (byte)'$' or (byte)'%' or (byte)'&' or (byte)'\'' or (byte)'*'
            or (byte)'+' or (byte)'-' or (byte)'.' or (byte)'/' or (byte)':' or (byte)'<' or (byte)'='
            or (byte)'>' or (byte)'?' or (byte)'@' or (byte)'\\' or (byte)'^' or (byte)'_' or (byte)'`'
            or (byte)'|' or (byte)'~';
}

/// <summary>
/// The text format's numbers: integers into their bit patterns, and floats rounded exactly.
/// </summary>
/// <remarks>
/// <para>
/// <b>Floats are where a reader goes wrong quietly</b>, so neither kind is rounded through the other.
/// A hexadecimal float is converted here, exactly, with ties to even and gradual underflow. A decimal
/// one goes to the runtime's parser for its own width, which has rounded correctly at any length
/// since .NET Core 3.0. Going through a double for a single would round twice, and the suite has
/// literals that tell the two apart.
/// </para>
/// <para>
/// <b>An integer is accepted over the union of its signed and unsigned ranges</b>, as the format
/// allows, and is stored as the bits it denotes.
/// </para>
/// </remarks>
internal static class ScriptNumbers
{
    /// <summary>A 32-bit integer literal's bits.</summary>
    internal static uint I32(string text, int line) =>
        (uint)(ulong)Integer(text, 32, line);

    /// <summary>A 64-bit integer literal's bits.</summary>
    internal static ulong I64(string text, int line) =>
        (ulong)Integer(text, 64, line);

    /// <summary>An unsigned 32-bit literal: an index, an offset, an alignment or a limit.</summary>
    internal static uint U32(string text, int line)
    {
        if (text.Length == 0 || text[0] is '+' or '-')
        {
            throw new ScriptReadException($"not an unsigned integer: {text}", line);
        }

        var value = Magnitude(text, line);

        if (value > uint.MaxValue)
        {
            throw new ScriptReadException($"integer out of range: {text}", line);
        }

        return (uint)value;
    }

    /// <summary>Whether a word is an unsigned integer, so an index may be told from a keyword.</summary>
    internal static bool IsUnsigned(string text) =>
        text.Length > 0 && char.IsAsciiDigit(text[0]);

    /// <summary>A 32-bit float literal's bits.</summary>
    internal static uint F32(string text, int line) =>
        (uint)Float(text, 8, 23, line);

    /// <summary>A 64-bit float literal's bits.</summary>
    internal static ulong F64(string text, int line) =>
        (ulong)Float(text, 11, 52, line);

    private static BigInteger Integer(string text, int bits, int line)
    {
        var negative = text.StartsWith('-');
        var body = text.Length > 0 && text[0] is '+' or '-' ? text[1..] : text;
        var magnitude = Magnitude(body, line);
        var modulus = BigInteger.One << bits;

        if (negative)
        {
            if (magnitude > (modulus >> 1))
            {
                throw new ScriptReadException($"integer out of range: {text}", line);
            }

            return (modulus - magnitude) % modulus;
        }

        if (magnitude >= modulus)
        {
            throw new ScriptReadException($"integer out of range: {text}", line);
        }

        return magnitude;
    }

    private static BigInteger Magnitude(string text, int line)
    {
        var hex = text.StartsWith("0x", StringComparison.Ordinal);
        var digits = (hex ? text[2..] : text).Replace("_", string.Empty, StringComparison.Ordinal);

        if (digits.Length == 0)
        {
            throw new ScriptReadException($"not an integer: {text}", line);
        }

        var value = BigInteger.Zero;

        foreach (var digit in digits)
        {
            var next = hex ? HexDigit(digit) : digit is >= '0' and <= '9' ? digit - '0' : -1;

            if (next < 0)
            {
                throw new ScriptReadException($"not an integer: {text}", line);
            }

            value = (value * (hex ? 16 : 10)) + next;
        }

        return value;
    }

    private static int HexDigit(char digit) => digit switch
    {
        >= '0' and <= '9' => digit - '0',
        >= 'a' and <= 'f' => digit - 'a' + 10,
        >= 'A' and <= 'F' => digit - 'A' + 10,
        _ => -1,
    };

    /// <summary>A float literal's bits in a format of the given exponent and fraction widths.</summary>
    private static BigInteger Float(string text, int exponentBits, int fractionBits, int line)
    {
        var negative = text.StartsWith('-');
        var body = text.Length > 0 && text[0] is '+' or '-' ? text[1..] : text;
        var sign = negative ? BigInteger.One << (exponentBits + fractionBits) : BigInteger.Zero;
        var infinity = ((BigInteger.One << exponentBits) - 1) << fractionBits;

        if (string.Equals(body, "inf", StringComparison.Ordinal))
        {
            return sign | infinity;
        }

        if (string.Equals(body, "nan", StringComparison.Ordinal))
        {
            return sign | infinity | (BigInteger.One << (fractionBits - 1));
        }

        if (body.StartsWith("nan:0x", StringComparison.Ordinal))
        {
            var payload = Magnitude(body[4..], line);

            if (payload.IsZero || payload >= (BigInteger.One << fractionBits))
            {
                throw new ScriptReadException($"NaN payload out of range: {text}", line);
            }

            return sign | infinity | payload;
        }

        var digits = body.Replace("_", string.Empty, StringComparison.Ordinal);

        if (digits.StartsWith("0x", StringComparison.Ordinal))
        {
            return sign | HexFloat(digits[2..], exponentBits, fractionBits, text, line);
        }

        // A decimal literal: the runtime's parser for this width, which rounds correctly at any
        // length. The sign is parsed with it so that a negative zero stays negative.
        var signed = negative ? "-" + digits : digits;

        if (fractionBits == 23)
        {
            if (!float.TryParse(signed, NumberStyles.Float, CultureInfo.InvariantCulture, out var single) ||
                float.IsInfinity(single))
            {
                throw new ScriptReadException($"not a finite float: {text}", line);
            }

            return BitConverter.SingleToUInt32Bits(single);
        }

        if (!double.TryParse(signed, NumberStyles.Float, CultureInfo.InvariantCulture, out var wide) ||
            double.IsInfinity(wide))
        {
            throw new ScriptReadException($"not a finite float: {text}", line);
        }

        return BitConverter.DoubleToUInt64Bits(wide);
    }

    /// <summary>
    /// A hexadecimal float, unsigned, rounded to the nearest representable value with ties to even,
    /// through the subnormals; a value that rounds past the largest finite one is refused.
    /// </summary>
    private static BigInteger HexFloat(string digits, int exponentBits, int fractionBits, string text, int line)
    {
        var exponentAt = digits.IndexOfAny(['p', 'P']);
        var mantissaText = exponentAt < 0 ? digits : digits[..exponentAt];
        var binaryExponent = exponentAt < 0 ? 0 : int.Parse(digits[(exponentAt + 1)..], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
        var point = mantissaText.IndexOf('.', StringComparison.Ordinal);
        var whole = point < 0 ? mantissaText : mantissaText[..point];
        var fraction = point < 0 ? string.Empty : mantissaText[(point + 1)..];

        if (whole.Length == 0 && fraction.Length == 0)
        {
            throw new ScriptReadException($"not a float: {text}", line);
        }

        var mantissa = BigInteger.Zero;

        foreach (var digit in whole + fraction)
        {
            var value = HexDigit(digit);

            if (value < 0)
            {
                throw new ScriptReadException($"not a float: {text}", line);
            }

            mantissa = (mantissa * 16) + value;
        }

        if (mantissa.IsZero)
        {
            return BigInteger.Zero;
        }

        // value = mantissa * 2^scale
        var scale = binaryExponent - (4 * fraction.Length);
        var precision = fractionBits + 1;
        var bias = (1 << (exponentBits - 1)) - 1;
        var minimumExponent = 1 - bias;
        var leading = (int)mantissa.GetBitLength() - 1 + scale;

        // The exponent of the value's quantum: the precision's last bit for a normal value, the
        // subnormals' fixed quantum below the smallest normal exponent.
        var quantumExponent = Math.Max(leading, minimumExponent) - (precision - 1);
        var shift = quantumExponent - scale;
        BigInteger significand;

        if (shift <= 0)
        {
            significand = mantissa << -shift;
        }
        else
        {
            significand = mantissa >> shift;
            var remainder = mantissa - (significand << shift);
            var half = BigInteger.One << (shift - 1);

            if (remainder > half || (remainder == half && !significand.IsEven))
            {
                significand += 1;
            }
        }

        // Rounding up can carry into a new leading bit: the quantum doubles and the value stays exact.
        if (significand.GetBitLength() > precision)
        {
            significand >>= 1;
            quantumExponent++;
        }

        if (significand.GetBitLength() < precision)
        {
            // Subnormal: the exponent field is zero, and the significand's bits are the fraction's.
            return significand;
        }

        var unbiased = quantumExponent + (precision - 1);

        if (unbiased > bias)
        {
            throw new ScriptReadException($"float out of range: {text}", line);
        }

        var exponentField = new BigInteger(unbiased + bias);
        var fractionMask = (BigInteger.One << fractionBits) - 1;
        return (exponentField << fractionBits) | (significand & fractionMask);
    }
}
