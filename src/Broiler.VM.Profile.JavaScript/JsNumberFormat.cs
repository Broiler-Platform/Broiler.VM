// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   19
// Annotated:        19/19
// Exempt:           0
// Human-reviewed:   0/19
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       19
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The two conversions between Number and String, written from the specification rather than
/// delegated to a platform formatter.
/// </summary>
/// <remarks>
/// <para>
/// <b>Neither direction is what .NET does.</b> <c>double.ToString("R")</c> answers <c>1E+21</c>
/// where the language says <c>1e+21</c>, <c>1E-07</c> where the language says <c>1e-7</c>, and
/// <c>5E-324</c> where the language says <c>5e-324</c>; and <c>double.Parse</c> accepts thousands
/// separators, rejects <c>0x10</c>, and has no opinion about a lone <c>Infinity</c>. Both are the
/// kind of difference no arithmetic corpus finds and every conformance suite does.
/// </para>
/// <para>
/// The platform is used for exactly one thing, and it is the thing it is good at: producing the
/// shortest decimal digit string that round-trips to the same double. That digit string is then
/// placed by the specification's own rules, which is where <c>k</c>, <c>n</c> and <c>s</c> below
/// come from - they are the specification's names for the digits, their count and the decimal
/// exponent.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=10C05B
// Broiler-Human:        PENDING
internal static class JsNumberFormat
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6F3573
    // Broiler-Human:        PENDING
    private static readonly string[] SmallIntegers = BuildSmallIntegers();

    /// <summary>The specification's <c>Number::toString</c> in radix 10.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=77D95D
    // Broiler-Human:        PENDING
    internal static string ToJsString(double value)
    {
        if (double.IsNaN(value))
        {
            return "NaN";
        }

        if (value == 0)
        {
            return "0";
        }

        if (double.IsPositiveInfinity(value))
        {
            return "Infinity";
        }

        if (double.IsNegativeInfinity(value))
        {
            return "-Infinity";
        }

        if (value < 0)
        {
            return "-" + ToJsString(-value);
        }

        if (value < SmallIntegers.Length && value == System.Math.Floor(value))
        {
            return SmallIntegers[(int)value];
        }

        Decompose(value, out var digits, out var exponent);
        return Place(digits, exponent);
    }

    /// <summary>Renders an unsigned integer, which is what an array index always is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=732E60
    // Broiler-Human:        PENDING
    internal static string ToUintString(uint value) =>
        value < SmallIntegers.Length
            ? SmallIntegers[(int)value]
            : value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>The specification's <c>Number::toString</c> in a radix other than 10.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=764D80
    // Broiler-Human:        PENDING
    internal static string ToRadixString(double value, int radix)
    {
        if (radix == 10)
        {
            return ToJsString(value);
        }

        if (double.IsNaN(value))
        {
            return "NaN";
        }

        if (double.IsInfinity(value))
        {
            return value > 0 ? "Infinity" : "-Infinity";
        }

        if (value == 0)
        {
            return "0";
        }

        var negative = value < 0;

        if (negative)
        {
            value = -value;
        }

        var integral = System.Math.Floor(value);
        var fraction = value - integral;

        // THE STOPPING RULE IS THE VALUE'S OWN PRECISION AND NOT A DIGIT COUNT.
        //
        // This loop used to stop after twenty digits, with a comment saying that twenty is past the
        // point where a binary64 fraction carries information in any radix. That is true of radix
        // 36, where a digit carries five and a sixth bits, and false of radix 3, where it carries
        // one and a half: `(0.1).toString(3)` needs thirty-four digits and got twenty, so the
        // answer was a PREFIX of the right one - the shape a truncation always has, and the reason
        // a fixed count cannot be right for a range of radices.
        //
        // `delta` is half the distance to the next representable double, scaled by the same radix
        // at every step. Digits are produced while the remaining fraction is larger than the
        // uncertainty in the value itself, so the expansion stops exactly where it stops saying
        // anything - and the half-way case rounds up and carries, which is why the digits are held
        // in a list rather than appended to a builder.
        var delta = System.Math.Max(
            0.5 * (System.Math.BitIncrement(value) - value), double.Epsilon);

        var head = new System.Collections.Generic.List<char>();

        if (integral == 0)
        {
            head.Add('0');
        }
        else
        {
            while (integral >= 1)
            {
                head.Add(Digit((int)(integral % radix)));
                integral = System.Math.Floor(integral / radix);
            }

            head.Reverse();
        }

        var tail = new System.Collections.Generic.List<char>();

        if (fraction >= delta)
        {
            while (true)
            {
                fraction *= radix;
                delta *= radix;
                var digit = (int)fraction;
                tail.Add(Digit(digit));
                fraction -= digit;

                if (fraction > 0.5 || (fraction == 0.5 && (digit & 1) != 0))
                {
                    if (fraction + delta > 1)
                    {
                        RoundUpRadix(head, tail, radix);
                        break;
                    }
                }

                if (fraction < delta)
                {
                    break;
                }
            }
        }

        var text = new System.Text.StringBuilder();
        text.Append(head.ToArray());

        if (tail.Count != 0)
        {
            text.Append('.').Append(tail.ToArray());
        }

        return negative ? "-" + text : text.ToString();
    }

    /// <summary>
    /// Adds one to the last digit produced, carrying through the fraction and into the integer.
    /// </summary>
    /// <remarks>
    /// <b>The carry has to be able to leave the fraction entirely</b>, which is the case a
    /// round-up written in place would get wrong: rounding the last digit of <c>0.ff</c> in radix
    /// 16 makes the answer <c>1</c>, not <c>0.100</c> and not <c>1.00</c>. So trailing digits that
    /// wrapped are dropped rather than written as zeroes, and a carry that runs off the front of
    /// the integer part prepends a digit.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E62621
    // Broiler-Human:        PENDING
    private static void RoundUpRadix(
        System.Collections.Generic.List<char> head,
        System.Collections.Generic.List<char> tail,
        int radix)
    {
        for (var at = tail.Count - 1; at >= 0; at--)
        {
            var digit = Value(tail[at]) + 1;

            if (digit < radix)
            {
                tail[at] = Digit(digit);
                return;
            }

            tail.RemoveAt(at);
        }

        for (var at = head.Count - 1; at >= 0; at--)
        {
            var digit = Value(head[at]) + 1;

            if (digit < radix)
            {
                head[at] = Digit(digit);
                return;
            }

            head[at] = '0';
        }

        head.Insert(0, '1');
    }

    /// <summary>The value a digit character carries, in any radix this accepts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=140182
    // Broiler-Human:        PENDING
    private static int Value(char digit) =>
        digit <= '9' ? digit - '0' : digit - 'a' + 10;

    /// <summary>The specification's <c>StringToNumber</c>, including the whitespace it trims.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E860DD
    // Broiler-Human:        PENDING
    internal static double ToNumber(string text)
    {
        var trimmed = Trim(text);

        if (trimmed.Length == 0)
        {
            return 0;
        }

        if (trimmed.Length > 2 && trimmed[0] == '0')
        {
            var radix = trimmed[1] switch
            {
                'x' or 'X' => 16,
                'o' or 'O' => 8,
                'b' or 'B' => 2,
                _ => 0,
            };

            if (radix != 0)
            {
                return ParseRadix(trimmed, 2, radix);
            }
        }

        var sign = 1.0;
        var at = 0;

        if (trimmed[0] is '+' or '-')
        {
            sign = trimmed[0] == '-' ? -1 : 1;
            at = 1;
        }

        if (string.CompareOrdinal(trimmed, at, "Infinity", 0, 8) == 0 && trimmed.Length - at == 8)
        {
            return sign * double.PositiveInfinity;
        }

        if (!IsDecimalLiteral(trimmed, at))
        {
            return double.NaN;
        }

        return double.TryParse(
            trimmed,
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out var parsed)
            ? parsed
            : double.NaN;
    }

    /// <summary>The global <c>parseInt</c>, which stops at the first character it cannot use.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9BE587
    // Broiler-Human:        PENDING
    internal static double ParseInt(string text, int radix)
    {
        var trimmed = Trim(text);
        var at = 0;
        var sign = 1.0;

        if (at < trimmed.Length && trimmed[at] is '+' or '-')
        {
            sign = trimmed[at] == '-' ? -1 : 1;
            at++;
        }

        if (radix is 0 or 16 &&
            at + 1 < trimmed.Length &&
            trimmed[at] == '0' &&
            trimmed[at + 1] is 'x' or 'X')
        {
            at += 2;
            radix = 16;
        }
        else if (radix == 0)
        {
            radix = 10;
        }

        if (radix is < 2 or > 36)
        {
            return double.NaN;
        }

        var start = at;
        var accumulated = 0.0;

        while (at < trimmed.Length)
        {
            var digit = DigitValue(trimmed[at]);

            if (digit < 0 || digit >= radix)
            {
                break;
            }

            accumulated = (accumulated * radix) + digit;
            at++;
        }

        return at == start ? double.NaN : sign * accumulated;
    }

    /// <summary>The global <c>parseFloat</c>, which reads the longest decimal prefix.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E73E6A
    // Broiler-Human:        PENDING
    internal static double ParseFloat(string text)
    {
        var trimmed = Trim(text);
        var at = 0;

        if (at < trimmed.Length && trimmed[at] is '+' or '-')
        {
            at++;
        }

        if (string.CompareOrdinal(trimmed, at, "Infinity", 0, System.Math.Min(8, trimmed.Length - at)) == 0 &&
            trimmed.Length - at >= 8)
        {
            return trimmed[0] == '-' ? double.NegativeInfinity : double.PositiveInfinity;
        }

        var digitsBefore = at;

        while (at < trimmed.Length && trimmed[at] is >= '0' and <= '9')
        {
            at++;
        }

        if (at < trimmed.Length && trimmed[at] == '.')
        {
            at++;

            while (at < trimmed.Length && trimmed[at] is >= '0' and <= '9')
            {
                at++;
            }
        }

        if (at == digitsBefore || (at == digitsBefore + 1 && trimmed[digitsBefore] == '.'))
        {
            return double.NaN;
        }

        var beforeExponent = at;

        if (at < trimmed.Length && trimmed[at] is 'e' or 'E')
        {
            at++;

            if (at < trimmed.Length && trimmed[at] is '+' or '-')
            {
                at++;
            }

            var exponentDigits = at;

            while (at < trimmed.Length && trimmed[at] is >= '0' and <= '9')
            {
                at++;
            }

            if (at == exponentDigits)
            {
                at = beforeExponent;
            }
        }

        return double.TryParse(
            System.MemoryExtensions.AsSpan(trimmed, 0, at),
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out var parsed)
            ? parsed
            : double.NaN;
    }

    /// <summary>Whether every character of <paramref name="text"/> is JavaScript whitespace.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=91F7DB
    // Broiler-Human:        PENDING
    internal static bool IsWhiteSpace(char character) =>
        character is ' ' or '\t' or '\n' or '\r' or '\u000B' or '\u000C' or
            '\u00A0' or '\uFEFF' or '\u2028' or '\u2029' ||
        System.Globalization.CharUnicodeInfo.GetUnicodeCategory(character) ==
            System.Globalization.UnicodeCategory.SpaceSeparator;

    /// <summary>Trims the whitespace the specification's numeric conversions trim.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2E25B8
    // Broiler-Human:        PENDING
    internal static string Trim(string text)
    {
        var start = 0;
        var end = text.Length;

        while (start < end && IsWhiteSpace(text[start]))
        {
            start++;
        }

        while (end > start && IsWhiteSpace(text[end - 1]))
        {
            end--;
        }

        return start == 0 && end == text.Length ? text : text[start..end];
    }

    /// <summary>Splits <paramref name="value"/> into the shortest round-tripping digits and an exponent.</summary>
    /// <remarks>
    /// <paramref name="digits"/> and <paramref name="exponent"/> are the specification's <c>s</c>
    /// and <c>n</c>: the value equals <c>s x 10^(n - k)</c>, where <c>k</c> is the digit count, and
    /// <c>s</c> has no leading and no trailing zero.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9B28CE
    // Broiler-Human:        PENDING
    private static void Decompose(double value, out string digits, out int exponent)
    {
        var rendered = value.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
        var mantissa = rendered;
        var scale = 0;
        var atE = rendered.IndexOf('E', System.StringComparison.Ordinal);

        if (atE >= 0)
        {
            mantissa = rendered[..atE];
            scale = int.Parse(
                rendered[(atE + 1)..], System.Globalization.CultureInfo.InvariantCulture);
        }

        var atDot = mantissa.IndexOf('.', System.StringComparison.Ordinal);
        string whole;
        string fraction;

        if (atDot >= 0)
        {
            whole = mantissa[..atDot];
            fraction = mantissa[(atDot + 1)..];
        }
        else
        {
            whole = mantissa;
            fraction = string.Empty;
        }

        var all = whole + fraction;
        exponent = whole.Length + scale;

        var first = 0;

        while (first < all.Length - 1 && all[first] == '0')
        {
            first++;
            exponent--;
        }

        var last = all.Length;

        while (last > first + 1 && all[last - 1] == '0')
        {
            last--;
        }

        digits = all[first..last];

        if (digits.Length == 1 && digits[0] == '0')
        {
            exponent = 1;
        }
    }

    /// <summary>Places the digits by the five cases of <c>Number::toString</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=21624D
    // Broiler-Human:        PENDING
    private static string Place(string digits, int exponent)
    {
        var count = digits.Length;

        if (count <= exponent && exponent <= 21)
        {
            return digits + new string('0', exponent - count);
        }

        if (exponent is > 0 and <= 21)
        {
            return digits[..exponent] + "." + digits[exponent..];
        }

        if (exponent is > -6 and <= 0)
        {
            return "0." + new string('0', -exponent) + digits;
        }

        var sign = exponent - 1 >= 0 ? "+" : "-";
        var magnitude = System.Math.Abs(exponent - 1)
            .ToString(System.Globalization.CultureInfo.InvariantCulture);

        return count == 1
            ? digits + "e" + sign + magnitude
            : digits[..1] + "." + digits[1..] + "e" + sign + magnitude;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0A158E
    // Broiler-Human:        PENDING
    private static bool IsDecimalLiteral(string text, int at)
    {
        var sawDigit = false;

        while (at < text.Length && text[at] is >= '0' and <= '9')
        {
            at++;
            sawDigit = true;
        }

        if (at < text.Length && text[at] == '.')
        {
            at++;

            while (at < text.Length && text[at] is >= '0' and <= '9')
            {
                at++;
                sawDigit = true;
            }
        }

        if (!sawDigit)
        {
            return false;
        }

        if (at < text.Length && text[at] is 'e' or 'E')
        {
            at++;

            if (at < text.Length && text[at] is '+' or '-')
            {
                at++;
            }

            var exponentDigits = false;

            while (at < text.Length && text[at] is >= '0' and <= '9')
            {
                at++;
                exponentDigits = true;
            }

            if (!exponentDigits)
            {
                return false;
            }
        }

        return at == text.Length;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=548C4B
    // Broiler-Human:        PENDING
    private static double ParseRadix(string text, int from, int radix)
    {
        if (from >= text.Length)
        {
            return double.NaN;
        }

        var accumulated = 0.0;

        for (var at = from; at < text.Length; at++)
        {
            var digit = DigitValue(text[at]);

            if (digit < 0 || digit >= radix)
            {
                return double.NaN;
            }

            accumulated = (accumulated * radix) + digit;
        }

        return accumulated;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BA921B
    // Broiler-Human:        PENDING
    private static int DigitValue(char character) => character switch
    {
        >= '0' and <= '9' => character - '0',
        >= 'a' and <= 'z' => character - 'a' + 10,
        >= 'A' and <= 'Z' => character - 'A' + 10,
        _ => -1,
    };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5D18E0
    // Broiler-Human:        PENDING
    private static char Digit(int value) =>
        (char)(value < 10 ? '0' + value : 'a' + (value - 10));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=561274
    // Broiler-Human:        PENDING
    private static string[] BuildSmallIntegers()
    {
        var table = new string[256];

        for (var at = 0; at < table.Length; at++)
        {
            table[at] = at.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        return table;
    }
}
