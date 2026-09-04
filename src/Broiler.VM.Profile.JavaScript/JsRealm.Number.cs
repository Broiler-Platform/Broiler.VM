// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   24
// Annotated:        24/24
// Exempt:           0
// Human-reviewed:   0/24
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       24
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>Number</c> intrinsic: the constructor, its statics, and <c>Number.prototype</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>The platform formatter is used for its digits and not for its rounding.</b> On .NET Core the
/// <c>"F"</c> and <c>"E"</c> formats produce the exact decimal digits of a binary64 - which is what
/// <c>toFixed</c>, <c>toExponential</c> and <c>toPrecision</c> need and what a hand-rolled dtoa
/// would have to reproduce - but they break an exact halfway case to EVEN, and the specification
/// breaks it AWAY FROM ZERO. That is the whole of the difference and it is observable in three
/// digits: <c>(2.5).toFixed(0)</c> is <c>"3"</c> and .NET renders <c>2</c>,
/// <c>(2745).toPrecision(3)</c> is <c>"2.75e+3"</c> and .NET renders <c>2.74E+003</c>.
/// </para>
/// <para>
/// So a halfway case is detected exactly rather than guessed at. Writing the value as
/// <c>m x 2^p</c> with <c>m</c> odd, the value is exactly halfway at <c>d</c> decimal places iff
/// <c>p + d + 1 = 0</c> - and, when <c>d</c> is negative, iff <c>5^-d</c> also divides <c>m</c>.
/// Being a halfway case is also what makes the expansion terminate one digit later, so the branch
/// that fixes it re-renders with one more digit, drops the trailing <c>5</c> that is now known to
/// be exact, and increments. Everything else is the platform's digits placed by the
/// specification's rules.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>Two to the fifty-third, less one: the largest integer a binary64 holds alone.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AF3796
    // Broiler-Human:        PENDING
    private const double NumberMaxSafe = 9007199254740991.0;

    /// <summary>Two to the minus fifty-second: the gap between one and the next Number.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1A63BB
    // Broiler-Human:        PENDING
    private const double NumberEpsilonStep = 2.220446049250313E-16;

    /// <summary>The magnitude at which <c>toFixed</c> hands the value back to <c>ToString</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=453870
    // Broiler-Human:        PENDING
    private const double NumberFixedCeiling = 1e21;

    /// <summary>How many significant digits a binary64 ever needs to round-trip.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C9C3BE
    // Broiler-Human:        PENDING
    private const int NumberRoundTripDigits = 17;

    /// <summary>The largest <c>t</c> for which <c>5^t</c> can divide a fifty-three bit odd number.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8B7E2E
    // Broiler-Human:        PENDING
    private const int NumberFivePowerLimit = 22;

    /// <summary>The culture every conversion in this file uses, because none of them are localised.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=219406
    // Broiler-Human:        PENDING
    private static readonly System.Globalization.CultureInfo NumberCulture =
        System.Globalization.CultureInfo.InvariantCulture;

    /// <summary>Builds <c>Number</c> and <c>Number.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=84A0EC
    // Broiler-Human:        PENDING
    private void SetupNumber()
    {
        var prototype = NumberPrototype;

        var constructor = Constructor(
            "Number",
            1,
            prototype,
            static (engine, thisValue, arguments) =>
                JsValue.Number(NumberFromArguments(engine, arguments)),
            (engine, thisValue, arguments) =>
                JsValue.Object(
                    new JsPrimitiveWrapper(
                        prototype, "Number", JsValue.Number(NumberFromArguments(engine, arguments)))));

        constructor.DefineFrozen("MAX_SAFE_INTEGER", JsValue.Number(NumberMaxSafe));
        constructor.DefineFrozen("MIN_SAFE_INTEGER", JsValue.Number(-NumberMaxSafe));
        constructor.DefineFrozen("MAX_VALUE", JsValue.Number(double.MaxValue));
        constructor.DefineFrozen("MIN_VALUE", JsValue.Number(double.Epsilon));
        constructor.DefineFrozen("EPSILON", JsValue.Number(NumberEpsilonStep));
        constructor.DefineFrozen("POSITIVE_INFINITY", JsValue.Number(double.PositiveInfinity));
        constructor.DefineFrozen("NEGATIVE_INFINITY", JsValue.Number(double.NegativeInfinity));
        constructor.DefineFrozen("NaN", JsValue.Number(double.NaN));

        // THE FOUR PREDICATES DO NOT COERCE. `Number.isNaN("NaN")` is false where the global
        // `isNaN("NaN")` is true, and that difference is the only reason the static exists.
        Method(constructor, "isInteger", 1, static (engine, thisValue, arguments) =>
        {
            var value = ArgOfNumber(arguments, 0);
            return JsValue.Boolean(value.IsNumber && NumberIsIntegral(value.AsNumber()));
        });

        Method(constructor, "isSafeInteger", 1, static (engine, thisValue, arguments) =>
        {
            var value = ArgOfNumber(arguments, 0);

            return JsValue.Boolean(
                value.IsNumber &&
                NumberIsIntegral(value.AsNumber()) &&
                System.Math.Abs(value.AsNumber()) <= NumberMaxSafe);
        });

        Method(constructor, "isFinite", 1, static (engine, thisValue, arguments) =>
        {
            var value = ArgOfNumber(arguments, 0);
            return JsValue.Boolean(value.IsNumber && double.IsFinite(value.AsNumber()));
        });

        Method(constructor, "isNaN", 1, static (engine, thisValue, arguments) =>
        {
            var value = ArgOfNumber(arguments, 0);
            return JsValue.Boolean(value.IsNumber && double.IsNaN(value.AsNumber()));
        });

        Method(constructor, "parseFloat", 1, static (engine, thisValue, arguments) =>
        {
            var text = engine.ToStringValue(ArgOfNumber(arguments, 0));
            engine.Charge((ulong)text.Length + 1);
            return JsValue.Number(JsNumberFormat.ParseFloat(text));
        });

        Method(constructor, "parseInt", 2, static (engine, thisValue, arguments) =>
        {
            var text = engine.ToStringValue(ArgOfNumber(arguments, 0));
            var radix = engine.ToInt32(ArgOfNumber(arguments, 1));
            engine.Charge((ulong)text.Length + 1);
            return JsValue.Number(JsNumberFormat.ParseInt(text, radix));
        });

        Method(prototype, "toString", 1, static (engine, thisValue, arguments) =>
        {
            var value = NumberThisValue(engine, thisValue);
            var argument = ArgOfNumber(arguments, 0);
            var radix = argument.Type == JsType.Undefined ? 10.0 : engine.ToInteger(argument);

            if (radix is < 2 or > 36)
            {
                return engine.ThrowRangeError("toString() radix must be an integer between 2 and 36");
            }

            var text = JsNumberFormat.ToRadixString(value, (int)radix);
            engine.Charge((ulong)text.Length + 1);
            return JsValue.String(text);
        });

        // NO LOCALE. The manifest admits no Intl surface, so this is `toString` under another
        // name - which is exactly what the specification permits an implementation without one
        // to do, and is a declared limitation rather than a stub.
        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
            JsValue.String(JsNumberFormat.ToJsString(NumberThisValue(engine, thisValue))));

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) =>
            JsValue.Number(NumberThisValue(engine, thisValue)));

        Method(prototype, "toFixed", 1, static (engine, thisValue, arguments) =>
        {
            var value = NumberThisValue(engine, thisValue);
            var digits = engine.ToInteger(ArgOfNumber(arguments, 0));

            // The range is checked before the value is looked at, which is why
            // `(NaN).toFixed(101)` throws rather than answering "NaN".
            if (digits is < 0 or > 100)
            {
                return engine.ThrowRangeError("toFixed() digits must be between 0 and 100");
            }

            var text = NumberFixedText(value, (int)digits);
            engine.Charge((ulong)text.Length + 1);
            return JsValue.String(text);
        });

        Method(prototype, "toExponential", 1, static (engine, thisValue, arguments) =>
        {
            var value = NumberThisValue(engine, thisValue);
            var argument = ArgOfNumber(arguments, 0);
            var automatic = argument.Type == JsType.Undefined;
            var digits = automatic ? 0.0 : engine.ToInteger(argument);

            // Here the order is the other way round: a non-finite value answers before the range
            // is checked, so `(Infinity).toExponential(101)` is "Infinity" and not a RangeError.
            if (!double.IsFinite(value))
            {
                return JsValue.String(JsNumberFormat.ToJsString(value));
            }

            if (!automatic && digits is < 0 or > 100)
            {
                return engine.ThrowRangeError("toExponential() digits must be between 0 and 100");
            }

            var text = NumberExponentialText(value, automatic ? -1 : (int)digits);
            engine.Charge((ulong)text.Length + 1);
            return JsValue.String(text);
        });

        Method(prototype, "toPrecision", 1, static (engine, thisValue, arguments) =>
        {
            var value = NumberThisValue(engine, thisValue);
            var argument = ArgOfNumber(arguments, 0);

            if (argument.Type == JsType.Undefined)
            {
                return JsValue.String(JsNumberFormat.ToJsString(value));
            }

            var precision = engine.ToInteger(argument);

            if (!double.IsFinite(value))
            {
                return JsValue.String(JsNumberFormat.ToJsString(value));
            }

            if (precision is < 1 or > 100)
            {
                return engine.ThrowRangeError("toPrecision() precision must be between 1 and 100");
            }

            var text = NumberPrecisionText(value, (int)precision);
            engine.Charge((ulong)text.Length + 1);
            return JsValue.String(text);
        });
    }

    /// <summary>Reads one argument, which may not have been supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F35F7E
    // Broiler-Human:        PENDING
    private static JsValue ArgOfNumber(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>What <c>Number(...)</c> and <c>new Number(...)</c> both compute.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=51B42B
    // Broiler-Human:        PENDING
    private static double NumberFromArguments(JsEngine engine, JsValue[] arguments) =>
        arguments.Length == 0 ? 0 : engine.ToNumber(ArgOfNumber(arguments, 0));

    /// <summary>The specification's <c>thisNumberValue</c>: a Number, or a wrapper holding one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=558C25
    // Broiler-Human:        PENDING
    private static double NumberThisValue(JsEngine engine, JsValue value)
    {
        if (value.IsNumber)
        {
            return value.AsNumber();
        }

        if (value.AsObjectOrNull() is JsPrimitiveWrapper wrapper && wrapper.Primitive.IsNumber)
        {
            return wrapper.Primitive.AsNumber();
        }

        // Unlike String.prototype's methods, these do NOT coerce: the specification throws for
        // every receiver that is not a Number or a boxed one, and a conformance suite checks it.
        throw engine.Error(
            "TypeError", "Number.prototype method called on a value that is not a Number");
    }

    /// <summary>Whether <paramref name="value"/> is a finite integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F8A20A
    // Broiler-Human:        PENDING
    private static bool NumberIsIntegral(double value) =>
        double.IsFinite(value) && System.Math.Truncate(value) == value;

    /// <summary><c>Number.prototype.toFixed</c>, over an already-validated digit count.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=264A7C
    // Broiler-Human:        PENDING
    private static string NumberFixedText(double value, int digits)
    {
        if (double.IsNaN(value))
        {
            return "NaN";
        }

        var negative = value < 0;

        // Abs() rather than a negation, so that -0 loses its sign here and renders as "0" while
        // -0.0001 keeps the "-" the specification's `s` gives it and renders as "-0.00".
        var magnitude = System.Math.Abs(value);
        string text;

        if (magnitude >= NumberFixedCeiling)
        {
            // At and above 1e21 the answer is ToString(x), infinities included.
            text = JsNumberFormat.ToJsString(magnitude);
        }
        else if (NumberIsMidpoint(magnitude, digits))
        {
            var exact = magnitude.ToString("F" + (digits + 1).ToString(NumberCulture), NumberCulture);
            var kept = exact[..^1];

            if (digits == 0)
            {
                kept = kept[..^1];
            }

            text = NumberIncrementFixed(kept);
        }
        else
        {
            text = magnitude.ToString("F" + digits.ToString(NumberCulture), NumberCulture);
        }

        return negative ? "-" + text : text;
    }

    /// <summary><c>Number.prototype.toExponential</c>; <paramref name="digits"/> is -1 when absent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D34EF6
    // Broiler-Human:        PENDING
    private static string NumberExponentialText(double value, int digits)
    {
        var negative = value < 0;
        var magnitude = System.Math.Abs(value);
        string significant;
        int exponent;

        if (digits < 0)
        {
            significant = NumberShortestDigits(magnitude, out exponent);
        }
        else
        {
            NumberRoundSignificant(magnitude, digits + 1, out significant, out exponent);
        }

        var text = NumberPointAfterFirst(significant) + "e" + NumberExponentSuffix(exponent);
        return negative ? "-" + text : text;
    }

    /// <summary><c>Number.prototype.toPrecision</c>, over an already-validated precision.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=50E30F
    // Broiler-Human:        PENDING
    private static string NumberPrecisionText(double value, int precision)
    {
        var negative = value < 0;
        var magnitude = System.Math.Abs(value);
        NumberRoundSignificant(magnitude, precision, out var digits, out var exponent);
        string text;

        if (exponent < -6 || exponent >= precision)
        {
            text = NumberPointAfterFirst(digits) + "e" + NumberExponentSuffix(exponent);
        }
        else if (exponent == precision - 1)
        {
            text = digits;
        }
        else if (exponent >= 0)
        {
            text = digits[..(exponent + 1)] + "." + digits[(exponent + 1)..];
        }
        else
        {
            text = "0." + new string('0', -(exponent + 1)) + digits;
        }

        return negative ? "-" + text : text;
    }

    /// <summary>
    /// Rounds <paramref name="magnitude"/> to <paramref name="count"/> significant digits, breaking
    /// an exact halfway case away from zero.
    /// </summary>
    /// <remarks>
    /// <paramref name="digits"/> comes back with exactly <paramref name="count"/> characters and
    /// <paramref name="exponent"/> is the decimal exponent of the first of them, so the value is
    /// <c>0.digits x 10^(exponent + 1)</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EB6628
    // Broiler-Human:        PENDING
    private static void NumberRoundSignificant(
        double magnitude, int count, out string digits, out int exponent)
    {
        var rendered = magnitude.ToString("E" + (count - 1).ToString(NumberCulture), NumberCulture);
        NumberSplitRendered(rendered, out var mantissa, out exponent);
        digits = NumberWithoutPoint(mantissa);

        if (!NumberIsMidpoint(magnitude, count - 1 - exponent))
        {
            return;
        }

        // A halfway case terminates one digit past the cut, so THIS rendering is exact and its
        // last character is the '5' that .NET just broke to even and the language breaks upward.
        // It is also why the exponent is re-read here: the rounded rendering may have carried.
        var exact = magnitude.ToString("E" + count.ToString(NumberCulture), NumberCulture);
        NumberSplitRendered(exact, out var exactMantissa, out exponent);
        digits = NumberIncrementDigits(NumberWithoutPoint(exactMantissa)[..^1], out var carried);

        if (carried)
        {
            exponent++;
        }
    }

    /// <summary>The fewest significant digits that read back as <paramref name="magnitude"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1D9E23
    // Broiler-Human:        PENDING
    private static string NumberShortestDigits(double magnitude, out int exponent)
    {
        for (var count = 0; count < NumberRoundTripDigits; count++)
        {
            var candidate = magnitude.ToString("E" + count.ToString(NumberCulture), NumberCulture);

            if (double.Parse(
                    candidate, System.Globalization.NumberStyles.Float, NumberCulture) == magnitude)
            {
                NumberSplitRendered(candidate, out var mantissa, out exponent);
                return NumberWithoutPoint(mantissa);
            }
        }

        var widest = magnitude.ToString(
            "E" + (NumberRoundTripDigits - 1).ToString(NumberCulture), NumberCulture);

        NumberSplitRendered(widest, out var last, out exponent);
        return NumberWithoutPoint(last);
    }

    /// <summary>
    /// Whether <paramref name="magnitude"/> is exactly halfway between two values that
    /// <paramref name="decimals"/> decimal places apart can hold.
    /// </summary>
    /// <remarks>
    /// Writing the value as <c>m x 2^p</c> with <c>m</c> odd, twice the value scaled by
    /// <c>10^decimals</c> is <c>m x 5^decimals x 2^(p + decimals + 1)</c>. That is an odd integer -
    /// which is what halfway means - exactly when the power of two vanishes, and, for a negative
    /// <paramref name="decimals"/>, when the five it is divided by divides <c>m</c> as well.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=625238
    // Broiler-Human:        PENDING
    private static bool NumberIsMidpoint(double magnitude, int decimals)
    {
        if (magnitude == 0 || !double.IsFinite(magnitude))
        {
            return false;
        }

        var bits = System.BitConverter.DoubleToInt64Bits(magnitude);
        var biased = (int)((bits >> 52) & 0x7FF);
        var significand = bits & 0xFFFFFFFFFFFFFL;
        int exponent;

        if (biased == 0)
        {
            exponent = -1074;
        }
        else
        {
            significand |= 0x10000000000000L;
            exponent = biased - 1075;
        }

        if (significand == 0)
        {
            return false;
        }

        while ((significand & 1) == 0)
        {
            significand >>= 1;
            exponent++;
        }

        if (exponent + decimals + 1 != 0)
        {
            return false;
        }

        if (decimals >= 0)
        {
            return true;
        }

        var power = -decimals;

        if (power > NumberFivePowerLimit)
        {
            // 5^23 is larger than any odd significand a binary64 has, so nothing divides.
            return false;
        }

        long divisor = 1;

        for (var step = 0; step < power; step++)
        {
            divisor *= 5;
        }

        return significand % divisor == 0;
    }

    /// <summary>Splits a <c>"E"</c>-formatted rendering into its mantissa and its exponent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C44C92
    // Broiler-Human:        PENDING
    private static void NumberSplitRendered(string rendered, out string mantissa, out int exponent)
    {
        var atE = rendered.IndexOf('E', System.StringComparison.Ordinal);

        if (atE < 0)
        {
            mantissa = rendered;
            exponent = 0;
            return;
        }

        mantissa = rendered[..atE];
        exponent = int.Parse(rendered[(atE + 1)..], NumberCulture);
    }

    /// <summary>Drops the point from a mantissa the <c>"E"</c> format wrote.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1D0AA3
    // Broiler-Human:        PENDING
    private static string NumberWithoutPoint(string mantissa) =>
        mantissa.Length > 1 ? mantissa[..1] + mantissa[2..] : mantissa;

    /// <summary>Puts the point back after the first digit, which is where <c>d.ddd</c> wants it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=43BB3F
    // Broiler-Human:        PENDING
    private static string NumberPointAfterFirst(string digits) =>
        digits.Length > 1 ? digits[..1] + "." + digits[1..] : digits;

    /// <summary>The exponent part, with the sign the language writes and no zero padding.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=36B50F
    // Broiler-Human:        PENDING
    private static string NumberExponentSuffix(int exponent) =>
        (exponent < 0 ? "-" : "+") + System.Math.Abs(exponent).ToString(NumberCulture);

    /// <summary>Adds one to a digit string, reporting the carry that lengthened it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CAAF5D
    // Broiler-Human:        PENDING
    private static string NumberIncrementDigits(string digits, out bool carried)
    {
        var characters = digits.ToCharArray();

        for (var at = characters.Length - 1; at >= 0; at--)
        {
            if (characters[at] != '9')
            {
                characters[at]++;
                carried = false;
                return new string(characters);
            }

            characters[at] = '0';
        }

        // Every digit was a nine, so the count is kept and the exponent moves instead.
        carried = true;
        return "1" + new string('0', digits.Length - 1);
    }

    /// <summary>Adds one to the last place of a fixed-point string, carrying across the point.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0B9F07
    // Broiler-Human:        PENDING
    private static string NumberIncrementFixed(string text)
    {
        var characters = text.ToCharArray();

        for (var at = characters.Length - 1; at >= 0; at--)
        {
            if (characters[at] == '.')
            {
                continue;
            }

            if (characters[at] != '9')
            {
                characters[at]++;
                return new string(characters);
            }

            characters[at] = '0';
        }

        return "1" + new string(characters);
    }
}
