// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   30
// Annotated:        30/30
// Exempt:           14
// Human-reviewed:   0/30
// IP risk:          Low
// Security risk:    High
// Criteria:         6/4
// Resource impact:  3/10 max
// Unverified:       30
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>What an Intl mathematical value is: a finite decimal, or one of ECMA-402's three others.</summary>
// Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.16; IP=Low; Security=Low; Resources=0; Fingerprint=D4A489
// Broiler-Human:        PENDING
internal enum JsDecimalKind
{
    /// <summary>A finite value, negative zero among them.</summary>
    Finite,

    /// <summary><c>not-a-number</c>.</summary>
    NaN,

    /// <summary><c>positive-infinity</c>.</summary>
    PositiveInfinity,

    /// <summary><c>negative-infinity</c>.</summary>
    NegativeInfinity,
}

/// <summary>ECMA-402's unsigned rounding modes (table 22 of the pinned edition).</summary>
// Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.17; IP=Low; Security=Low; Resources=0; Fingerprint=485763
// Broiler-Human:        PENDING
internal enum JsUnsignedRounding
{
    /// <summary>Toward zero.</summary>
    Zero,

    /// <summary>Away from zero.</summary>
    Infinity,

    /// <summary>To the nearer, ties toward zero.</summary>
    HalfZero,

    /// <summary>To the nearer, ties away from zero.</summary>
    HalfInfinity,

    /// <summary>To the nearer, ties to the even one.</summary>
    HalfEven,
}

/// <summary>
/// An exact decimal value, the "Intl mathematical value" ECMA-402's number formatting works on: a
/// sign, the significant digits and where the decimal point falls among them (JSD-0044).
/// </summary>
/// <remarks>
/// <para>
/// <b>Digits, not a binary integer.</b> The value is <c>0.Digits * 10^Point</c>, with no leading or
/// trailing zero in <see cref="Digits"/>. Every operation ECMA-402 asks of it - scaling by a power
/// of ten, rounding at a decimal position to a multiple of an increment, and writing it out - is a
/// walk over that string, linear in its length, so a BigInt of a million bits formats in time
/// proportional to its digits and charges for each.
/// </para>
/// <para>
/// <b>The increment needs only the last four digits</b>: every increment ECMA-402 admits divides
/// 10,000, so the remainder of a count of units by the increment, and the parity of the quotient,
/// are read from at most five trailing digits.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5; IP=Low; Security=High; Resources=3; Fingerprint=0C7B76
// Broiler-Falsified-If: a value rounds to a decimal other than the one ECMA-402's ToRawFixed or ToRawPrecision selects for its rounding mode, or the work is not linear in the digits
// Broiler-Human:        PENDING
internal readonly struct JsDecimal
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D9CFA4
    // Broiler-Human:        PENDING
    private JsDecimal(JsDecimalKind kind, bool negative, string digits, int point)
    {
        Kind = kind;
        Negative = negative;
        Digits = digits;
        Point = point;
    }

    internal JsDecimalKind Kind { get; }

    /// <summary>The sign of a finite value, negative zero's included.</summary>
    internal bool Negative { get; }

    /// <summary>The significant digits, without leading or trailing zeros; empty for zero.</summary>
    internal string Digits { get; }

    /// <summary>Where the decimal point falls: the value is <c>0.Digits * 10^Point</c>.</summary>
    internal int Point { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=046CB1
    // Broiler-Human:        PENDING
    internal static JsDecimal NaN => new(JsDecimalKind.NaN, false, string.Empty, 0);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=37D5C0
    // Broiler-Human:        PENDING
    internal static JsDecimal PositiveInfinity => new(JsDecimalKind.PositiveInfinity, false, string.Empty, 0);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E7E130
    // Broiler-Human:        PENDING
    internal static JsDecimal NegativeInfinity => new(JsDecimalKind.NegativeInfinity, true, string.Empty, 0);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E83549
    // Broiler-Human:        PENDING
    internal static JsDecimal Zero => new(JsDecimalKind.Finite, false, string.Empty, 0);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4B2BF6
    // Broiler-Human:        PENDING
    internal static JsDecimal NegativeZero => new(JsDecimalKind.Finite, true, string.Empty, 0);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=077D2E
    // Broiler-Human:        PENDING
    internal bool IsFinite => Kind == JsDecimalKind.Finite;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D37727
    // Broiler-Human:        PENDING
    internal bool IsZero => Kind == JsDecimalKind.Finite && Digits.Length == 0;

    /// <summary>The power of ten of the leading digit, <c>floor(log10(|x|))</c>, for a nonzero value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=26D5C2
    // Broiler-Human:        PENDING
    internal int Magnitude => Point - 1;

    /// <summary>A finite value from digits that may carry leading and trailing zeros.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DC50EE
    // Broiler-Human:        PENDING
    internal static JsDecimal Finite(bool negative, string digits, int point)
    {
        var first = 0;

        while (first < digits.Length && digits[first] == '0')
        {
            first++;
        }

        var last = digits.Length;

        while (last > first && digits[last - 1] == '0')
        {
            last--;
        }

        return first == last
            ? new JsDecimal(JsDecimalKind.Finite, negative, string.Empty, 0)
            : new JsDecimal(JsDecimalKind.Finite, negative, digits[first..last], point - first);
    }

    /// <summary>The value without its sign.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CEBB05
    // Broiler-Human:        PENDING
    internal JsDecimal Abs() => new(Kind == JsDecimalKind.NegativeInfinity ? JsDecimalKind.PositiveInfinity : Kind, false, Digits, Point);

    /// <summary>The value with its sign set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4BA31D
    // Broiler-Human:        PENDING
    internal JsDecimal WithSign(bool negative) =>
        Kind switch
        {
            JsDecimalKind.Finite => new JsDecimal(Kind, negative, Digits, Point),
            JsDecimalKind.PositiveInfinity or JsDecimalKind.NegativeInfinity =>
                negative ? NegativeInfinity : PositiveInfinity,
            _ => this,
        };

    /// <summary>The value times <c>10^power</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=44AF16
    // Broiler-Human:        PENDING
    internal JsDecimal Scale(int power) => IsZero || !IsFinite ? this : new JsDecimal(Kind, Negative, Digits, Point + power);

    /// <summary>Whether two values are the same value: kind, sign and digits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A49E0D
    // Broiler-Human:        PENDING
    internal bool SameAs(JsDecimal other) =>
        Kind == other.Kind && Negative == other.Negative && Point == other.Point && string.Equals(Digits, other.Digits, System.StringComparison.Ordinal);

    /// <summary>The value a Number is: its shortest round-trip decimal, as ECMA-402's ToIntlMathematicalValue reads it.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.16; IP=Low; Security=Low; Resources=1; Fingerprint=677AAD
    // Broiler-Human:        PENDING
    internal static JsDecimal FromNumber(double value)
    {
        if (double.IsNaN(value))
        {
            return NaN;
        }

        if (double.IsInfinity(value))
        {
            return value > 0 ? PositiveInfinity : NegativeInfinity;
        }

        if (value == 0)
        {
            return double.IsNegative(value) ? NegativeZero : Zero;
        }

        return ParseDecimal(JsNumberFormat.ToJsString(value)) ?? NaN;
    }

    /// <summary>The value a BigInt is, exactly.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BA1271
    // Broiler-Human:        PENDING
    internal static JsDecimal FromBigInt(JsBigInt value, System.Action<ulong> charge)
    {
        var text = value.ToDecimalString(charge);
        var negative = text.StartsWith('-');
        var digits = negative ? text[1..] : text;
        return Finite(negative, digits, digits.Length);
    }

    /// <summary>
    /// ECMA-402's ToIntlMathematicalValue for a String: its StringNumericLiteral read exactly, then
    /// held to the Number range as RoundMVResult rounds it.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.16; IP=Low; Security=High; Resources=3; Fingerprint=7BF49B
    // Broiler-Falsified-If: a text outside StringNumericLiteral answers other than not-a-number, or a value that rounds to an infinity or a zero as a Number is kept as written
    // Broiler-Human:        PENDING
    internal static JsDecimal FromString(string text, System.Action<ulong> charge)
    {
        charge((ulong)text.Length / 8 + 1);
        var trimmed = JsNumberFormat.Trim(text);

        if (trimmed.Length == 0)
        {
            return Zero;
        }

        switch (trimmed)
        {
            case "Infinity":
            case "+Infinity":
                return PositiveInfinity;
            case "-Infinity":
                return NegativeInfinity;
        }

        if (trimmed.Length > 2 && trimmed[0] == '0' && (trimmed[1] | 0x20) is 'x' or 'o' or 'b')
        {
            if (!JsBigInt.TryParseStringInteger(trimmed, charge, out var integer, out var tooWide))
            {
                return tooWide != 0 ? PositiveInfinity : NaN;
            }

            return FromBigInt(integer!, charge).HeldToNumberRange();
        }

        return ParseDecimal(trimmed)?.HeldToNumberRange() ?? NaN;
    }

    /// <summary>
    /// A StrDecimalLiteral: an optional sign, digits with an optional point, and an optional
    /// exponent; nothing else.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-262 s7.1.4.1; IP=Low; Security=High; Resources=2; Fingerprint=B61AEA
    // Broiler-Falsified-If: a text that is not a StrDecimalLiteral parses, or one that is reads as another value
    // Broiler-Human:        PENDING
    private static JsDecimal? ParseDecimal(string text)
    {
        var at = 0;
        var negative = false;

        if (at < text.Length && text[at] is '+' or '-')
        {
            negative = text[at] == '-';
            at++;
        }

        var digits = new System.Text.StringBuilder();
        var point = 0;
        var seenPoint = false;
        var anyDigit = false;

        for (; at < text.Length; at++)
        {
            var c = text[at];

            if (char.IsAsciiDigit(c))
            {
                anyDigit = true;
                digits.Append(c);

                if (!seenPoint)
                {
                    point++;
                }
            }
            else if (c == '.' && !seenPoint)
            {
                seenPoint = true;
            }
            else
            {
                break;
            }
        }

        if (!anyDigit)
        {
            return null;
        }

        long exponent = 0;

        if (at < text.Length && text[at] is 'e' or 'E')
        {
            at++;
            var exponentNegative = false;

            if (at < text.Length && text[at] is '+' or '-')
            {
                exponentNegative = text[at] == '-';
                at++;
            }

            var exponentDigits = 0;

            for (; at < text.Length && char.IsAsciiDigit(text[at]); at++)
            {
                exponentDigits++;

                // AN EXPONENT PAST A BILLION is past every Number's range in either direction, and is
                // held there rather than read further.
                if (exponent < 1_000_000_000)
                {
                    exponent = (exponent * 10) + (text[at] - '0');
                }
            }

            if (exponentDigits == 0)
            {
                return null;
            }

            if (exponentNegative)
            {
                exponent = -exponent;
            }
        }

        if (at != text.Length)
        {
            return null;
        }

        return Finite(negative, digits.ToString(), (int)System.Math.Clamp(point + exponent, -2_000_000_000L, 2_000_000_000L));
    }

    /// <summary>
    /// RoundMVResult's test: a value whose Number would be an infinity is that infinity, and one whose
    /// Number would be a zero is that zero; any other is kept exactly.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.16 step 9; IP=Low; Security=Medium; Resources=1; Fingerprint=1C6FFF
    // Broiler-Human:        PENDING
    private JsDecimal HeldToNumberRange()
    {
        if (!IsFinite || IsZero)
        {
            return this;
        }

        if (Magnitude > 309)
        {
            return Negative ? NegativeInfinity : PositiveInfinity;
        }

        if (Magnitude < -330)
        {
            return Negative ? NegativeZero : Zero;
        }

        // NEAR EITHER EDGE, THE NUMBER DECIDES: the digits are cut at 800 with a sticky 1 standing for
        // any that were cut, which keeps a correctly rounded parse on the same side of every tie.
        var shown = Digits.Length > 800 ? Digits[..800] + "1" : Digits;
        var number = double.Parse("0." + shown + "e" + Point.ToString(System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture);

        if (double.IsInfinity(number))
        {
            return Negative ? NegativeInfinity : PositiveInfinity;
        }

        return number == 0 ? (Negative ? NegativeZero : Zero) : this;
    }

    /// <summary>
    /// The count of units of <c>10^unit</c>, a multiple of <paramref name="increment"/>, that the
    /// rounding mode chooses for this nonnegative value: ECMA-402's choice between r1 and r2 in
    /// ToRawFixed and ToRawPrecision, through ApplyUnsignedRoundingMode.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.18; IP=Low; Security=High; Resources=3; Fingerprint=55ECF7
    // Broiler-Falsified-If: a tie, a value just above or just below a tie, or an exact multiple is rounded otherwise than ApplyUnsignedRoundingMode states, for any admitted increment
    // Broiler-Human:        PENDING
    internal string RoundToUnits(int unit, int increment, JsUnsignedRounding mode)
    {
        if (IsZero)
        {
            return "0";
        }

        // THE UNITS AT OR ABOVE THE POSITION, AND WHAT IS LEFT BELOW IT.
        var above = (long)Point - unit;
        string units;
        int tail;

        if (above <= 0)
        {
            units = "0";

            // EVERY DIGIT IS BELOW THE POSITION: the first one a zero when the point is further down.
            tail = above < 0 ? 1 : Classify(Digits);
        }
        else if (above < Digits.Length)
        {
            units = Digits[..(int)above];
            tail = Classify(Digits[(int)above..]);
        }
        else
        {
            units = Digits + new string('0', (int)(above - Digits.Length));
            tail = 0;
        }

        // THE REMAINDER BY THE INCREMENT AND THE QUOTIENT'S PARITY, from the last five digits:
        // twice every admitted increment divides 100,000.
        var low = int.Parse(units.Length > 5 ? units[^5..] : units, System.Globalization.CultureInfo.InvariantCulture);
        var remainder = low % increment;
        var odd = (low % (2 * increment)) >= increment;

        bool up;

        if (remainder == 0 && tail == 0)
        {
            up = false;
        }
        else
        {
            var half = 2 * remainder > increment ? 1
                : 2 * remainder == increment ? (tail == 0 ? 0 : 1)
                : 2 * remainder == increment - 1 ? tail switch { 0 or 1 => -1, 2 => 0, _ => 1 }
                : -1;

            up = mode switch
            {
                JsUnsignedRounding.Zero => false,
                JsUnsignedRounding.Infinity => true,
                _ when half < 0 => false,
                _ when half > 0 => true,
                JsUnsignedRounding.HalfZero => false,
                JsUnsignedRounding.HalfInfinity => true,
                _ => odd,
            };
        }

        var result = Subtract(units, remainder);
        return up ? Add(result, increment) : result;
    }

    /// <summary>A tail of digits, with no trailing zero, against one half: 0 none, 1 below, 2 exactly, 3 above.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F48FE4
    // Broiler-Human:        PENDING
    private static int Classify(string tail) =>
        tail.Length == 0 ? 0
        : tail[0] < '5' ? 1
        : tail[0] > '5' ? 3
        : tail.Length == 1 ? 2 : 3;

    /// <summary>A digit string minus a small nonnegative number no greater than it, without leading zeros.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C367C6
    // Broiler-Human:        PENDING
    private static string Subtract(string digits, int value)
    {
        if (value == 0)
        {
            return digits.TrimStart('0') is { Length: > 0 } trimmed ? trimmed : "0";
        }

        var chars = digits.ToCharArray();
        var borrow = value;

        for (var at = chars.Length - 1; at >= 0 && borrow != 0; at--)
        {
            var d = chars[at] - '0' - (borrow % 10);
            borrow /= 10;

            if (d < 0)
            {
                d += 10;
                borrow++;
            }

            chars[at] = (char)('0' + d);
        }

        var text = new string(chars).TrimStart('0');
        return text.Length == 0 ? "0" : text;
    }

    /// <summary>A digit string plus a small nonnegative number, without leading zeros.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A63E16
    // Broiler-Human:        PENDING
    private static string Add(string digits, int value)
    {
        var chars = new System.Collections.Generic.List<char>(digits.Length + 5);
        chars.AddRange(digits);
        var carry = value;

        for (var at = chars.Count - 1; at >= 0 && carry != 0; at--)
        {
            var d = chars[at] - '0' + (carry % 10);
            carry /= 10;

            if (d >= 10)
            {
                d -= 10;
                carry++;
            }

            chars[at] = (char)('0' + d);
        }

        while (carry != 0)
        {
            chars.Insert(0, (char)('0' + (carry % 10)));
            carry /= 10;
        }

        var text = new string([.. chars]).TrimStart('0');
        return text.Length == 0 ? "0" : text;
    }
}

/// <summary>What ToRawFixed and ToRawPrecision answer.</summary>
// Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.8; IP=Low; Security=Low; Resources=0; Fingerprint=FB470A
// Broiler-Human:        PENDING
internal readonly record struct JsRawFormat(string Formatted, JsDecimal Rounded, int IntegerDigits, int RoundingMagnitude);

/// <summary>ECMA-402's raw decimal formats of a nonnegative value.</summary>
// Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.8, s16.5.9; IP=Low; Security=Medium; Resources=2; Fingerprint=6A98D1
// Broiler-Human:        PENDING
internal static class JsRawDecimal
{
    /// <summary>ECMA-402's ToRawFixed.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.9; IP=Low; Security=Medium; Resources=2; Fingerprint=F77F74
    // Broiler-Falsified-If: the string or the rounded value differs from ToRawFixed's for a value, a digit range, an increment and a mode
    // Broiler-Human:        PENDING
    internal static JsRawFormat ToRawFixed(JsDecimal x, int minFraction, int maxFraction, int increment, JsUnsignedRounding mode)
    {
        var f = maxFraction;
        var n = x.RoundToUnits(-f, increment, mode);
        var rounded = JsDecimal.Finite(false, n, n.Length - f);
        var m = n;
        int integerDigits;

        if (f != 0)
        {
            var k = m.Length;

            if (k <= f)
            {
                m = new string('0', f + 1 - k) + m;
                k = f + 1;
            }

            m = m[..(k - f)] + "." + m[(k - f)..];
            integerDigits = k - f;
        }
        else
        {
            integerDigits = m.Length;
        }

        m = Cut(m, maxFraction - minFraction);
        return new JsRawFormat(m, rounded, integerDigits, -f);
    }

    /// <summary>ECMA-402's ToRawPrecision.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.8; IP=Low; Security=Medium; Resources=2; Fingerprint=9D31D9
    // Broiler-Falsified-If: the string or the rounded value differs from ToRawPrecision's for a value, a precision range and a mode
    // Broiler-Human:        PENDING
    internal static JsRawFormat ToRawPrecision(JsDecimal x, int minPrecision, int maxPrecision, JsUnsignedRounding mode)
    {
        var p = maxPrecision;
        string n;
        int e;
        JsDecimal rounded;

        if (x.IsZero)
        {
            n = new string('0', p);
            e = 0;
            rounded = JsDecimal.Zero;
        }
        else
        {
            e = x.Magnitude;
            n = x.RoundToUnits(e - p + 1, 1, mode);

            // ROUNDING UP CARRIED INTO A NEW DIGIT: the same value one magnitude higher.
            if (n.Length > p)
            {
                e++;
                n = n[..p];
            }

            rounded = JsDecimal.Finite(false, n, e + 1);
        }

        string m;
        int integerDigits;

        if (e >= p - 1)
        {
            m = n + new string('0', e - p + 1);
            integerDigits = e + 1;
        }
        else if (e >= 0)
        {
            m = n[..(e + 1)] + "." + n[(e + 1)..];
            integerDigits = e + 1;
        }
        else
        {
            m = "0." + new string('0', -(e + 1)) + n;
            integerDigits = 1;
        }

        if (m.Contains('.') && maxPrecision > minPrecision)
        {
            m = Cut(m, maxPrecision - minPrecision);
        }

        return new JsRawFormat(m, rounded, integerDigits, e - p + 1);
    }

    /// <summary>Removes up to <paramref name="cut"/> trailing zeros after a point, then a trailing point.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=692384
    // Broiler-Human:        PENDING
    private static string Cut(string m, int cut)
    {
        var end = m.Length;

        if (m.Contains('.'))
        {
            while (cut > 0 && end > 0 && m[end - 1] == '0')
            {
                end--;
                cut--;
            }

            if (end > 0 && m[end - 1] == '.')
            {
                end--;
            }
        }

        return m[..end];
    }
}
