// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           0
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Globalization;
using Broiler.ECMAScript.Numbers;

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

    /// <summary>Builds <c>Number</c> and <c>Number.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C68C64
    // Broiler-Human:        PENDING
    private void SetupNumber()
    {
        var prototype = NumberPrototype;

        var constructor = Constructor("Number", 1, prototype,
            static (engine, thisValue, arguments) => JsValue.Number(NumberFromArguments(engine, arguments)),
            (engine, thisValue, arguments) => JsValue.Object(new JsPrimitiveWrapper(prototype, "Number", JsValue.Number(NumberFromArguments(engine, arguments)))));

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

        // `Number.parseFloat` and `Number.parseInt` ARE the global functions (21.1.2.12 and 21.1.2.13),
        // which SetupGlobal writes into these two slots. The slots are made here, in their place in
        // the key order, holding nothing until then; until SJL-3.4 they held two functions of their
        // own that were overwritten unused.
        constructor.DefineOrdinary("parseFloat", JsValue.Undefined);
        constructor.DefineOrdinary("parseInt", JsValue.Undefined);

        Method(prototype, "toString", 1, static (engine, thisValue, arguments) =>
        {
            var value = NumberThisValue(engine, thisValue);
            var argument = ArgOfNumber(arguments, 0);
            var radix = argument.Type == JsType.Undefined ? 10.0 : engine.ToInteger(argument);

            if (radix is < 2 or > 36)
            {
                return engine.ThrowRangeError("toString() radix must be an integer between 2 and 36");
            }

            var text = NumberText.ToRadixString(value, (int)radix);
            engine.Charge((ulong)text.Length + 1);
            return JsValue.String(text);
        });

        // WITHOUT INTL, NO LOCALE: `toString` under another name, which is what the specification
        // permits an implementation without ECMA-402 to do. WITH IT, ECMA-402 s20.2.1: a number
        // format from the locales and options, made by the realm's own %Intl.NumberFormat%.
        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            var value = NumberThisValue(engine, thisValue);

            return engine.Realm.NumberFormatConstructor is null
                ? JsValue.String(JsNumberFormat.NumberToString(value))
                : JsValue.String(ToLocaleNumberString(engine, JsDecimal.FromNumber(value), arguments));
        });

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

            var text = NumberText.ToFixed(value, (int)digits);
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
                return JsValue.String(JsNumberFormat.NumberToString(value));
            }

            if (!automatic && digits is < 0 or > 100)
            {
                return engine.ThrowRangeError("toExponential() digits must be between 0 and 100");
            }

            var text = automatic ? NumberText.ToExponential(value) : NumberText.ToExponential(value, (int)digits);
            engine.Charge((ulong)text.Length + 1);
            return JsValue.String(text);
        });

        Method(prototype, "toPrecision", 1, static (engine, thisValue, arguments) =>
        {
            var value = NumberThisValue(engine, thisValue);
            var argument = ArgOfNumber(arguments, 0);

            if (argument.Type == JsType.Undefined)
            {
                return JsValue.String(JsNumberFormat.NumberToString(value));
            }

            var precision = engine.ToInteger(argument);

            if (!double.IsFinite(value))
            {
                return JsValue.String(JsNumberFormat.NumberToString(value));
            }

            if (precision is < 1 or > 100)
            {
                return engine.ThrowRangeError("toPrecision() precision must be between 1 and 100");
            }

            var text = NumberText.ToPrecision(value, (int)precision);
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
    /// <remarks>
    /// <b><c>ToNumeric</c> and not <c>ToNumber</c></b>, because this is the one operation the
    /// language lets turn a BigInt into a Number (JSeal B05): <c>Number(2n ** 53n + 1n)</c> is the
    /// nearest Number, ties to even, where <c>+(2n ** 53n + 1n)</c> is a TypeError.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0B48FC
    // Broiler-Human:        PENDING
    private static double NumberFromArguments(JsEngine engine, JsValue[] arguments)
    {
        if (arguments.Length == 0)
        {
            return 0;
        }

        var numeric = engine.ToNumeric(ArgOfNumber(arguments, 0));
        return numeric.IsBigInt ? engine.BigIntToNumber(numeric.AsBigInt()) : numeric.AsNumber();
    }

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
        throw engine.Error("TypeError", "Number.prototype method called on a value that is not a Number");
    }

    /// <summary>Whether <paramref name="value"/> is a finite integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F8A20A
    // Broiler-Human:        PENDING
    private static bool NumberIsIntegral(double value) =>
        double.IsFinite(value) && System.Math.Truncate(value) == value;
}
