// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           0
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The String constructor, its <c>fromCharCode</c> static, and every method on
/// <c>String.prototype</c> this profile admits.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every method here charges fuel for the string it scans or produces.</b> A String method is
/// the cheapest way a guest can ask for unbounded work - <c>"x".repeat(1e9)</c> and
/// <c>s.split("")</c> are each one call - so the charge is proportional to the characters touched
/// rather than one unit per call, and the two methods that can name a length before producing it
/// refuse a result past <see cref="StringLengthCeiling"/> outright.
/// </para>
/// <para>
/// <b>The receiver is coerced, never assumed.</b> <c>String.prototype.slice.call(42)</c> is legal
/// JavaScript, so every body starts at <see cref="StringThis"/>, which accepts a String primitive,
/// a boxed String, or anything else that has a <c>ToString</c> - and throws for <c>null</c> and
/// <c>undefined</c>, which is the one case the specification makes an error rather than a
/// conversion.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>
    /// The longest string <c>repeat</c>, <c>padStart</c> and <c>padEnd</c> will materialise.
    /// </summary>
    /// <remarks>
    /// The specification's ceiling is 2^53-1, which no implementation can honour and which this one
    /// would meet by exhausting the host's memory rather than the guest's allowance. Sixteen million
    /// characters is thirty-two megabytes of UTF-16 and is a declared deviation: a program that asks
    /// for more gets the same <c>RangeError</c> the specification gives for a length it cannot
    /// represent.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3C0D2E
    // Broiler-Human:        PENDING
    private const int StringLengthCeiling = 1 << 24;

    /// <summary>Builds <c>String</c>, <c>String.fromCharCode</c> and <c>String.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=623D89
    // Broiler-Human:        PENDING
    private void SetupString()
    {
        var prototype = StringPrototype;

        var constructor = Constructor(
            "String",
            1,
            prototype,
            static (engine, thisValue, arguments) =>
            {
                _ = thisValue;

                if (arguments.Length == 0)
                {
                    return JsValue.String(string.Empty);
                }

                // `String(symbol)` IS THE ONE EXPLICIT COERCION A SYMBOL ALLOWS, and it is here
                // rather than in `ToString` on purpose: every implicit path must keep throwing, so
                // the exception is spelled out at the one call site the language exempts.
                if (arguments[0].IsSymbol)
                {
                    return JsValue.String(arguments[0].AsSymbol().Rendered);
                }

                var text = engine.ToStringValue(ArgOfString(arguments, 0));
                StringCharge(engine, text.Length);
                return JsValue.String(text);
            },
            static (engine, thisValue, arguments) =>
            {
                _ = thisValue;

                var text = arguments.Length == 0
                    ? string.Empty
                    : engine.ToStringValue(ArgOfString(arguments, 0));

                StringCharge(engine, text.Length);

                return JsValue.Object(
                    new JsPrimitiveWrapper(
                        engine.Realm.StringPrototype, "String", JsValue.String(text)));
            });

        Method(constructor, "fromCharCode", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            StringCharge(engine, arguments.Length);
            var builder = new System.Text.StringBuilder(arguments.Length);

            foreach (var argument in arguments)
            {
                builder.Append((char)(ushort)engine.ToUint32(argument));
            }

            return JsValue.String(builder.ToString());
        });

        Method(constructor, "fromCodePoint", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            StringCharge(engine, arguments.Length);
            var builder = new System.Text.StringBuilder(arguments.Length);

            foreach (var argument in arguments)
            {
                var code = engine.ToNumber(argument);

                // THE RANGE CHECK IS THE WHOLE DIFFERENCE FROM `fromCharCode`, which truncates to
                // sixteen bits and cannot fail. A code point outside the Unicode range, or one that
                // is not an integer, is a `RangeError` naming the value rather than a replacement
                // character a caller would carry around without knowing.
                if (code != System.Math.Floor(code) || code < 0 || code > 0x10FFFF ||
                    double.IsNaN(code))
                {
                    return engine.ThrowRangeError(
                        JsNumberFormat.ToJsString(code) + " is not a valid code point");
                }

                builder.Append(char.ConvertFromUtf32((int)code));
            }

            return JsValue.String(builder.ToString());
        });

        // `String.raw` IS THE TAG A TAGGED TEMPLATE EXISTS FOR, and it is written against the
        // public shape of a template's strings object rather than against anything the lowering
        // knows: `raw` and `length` read off the first argument, the substitutions after it. So a
        // hand-built object works exactly as a template site does, which is what the specification
        // says and what a conformance suite checks first.
        Method(constructor, "raw", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var cooked = ArgOfString(arguments, 0);
            var raw = engine.GetProperty(cooked, "raw");
            var length = engine.ToInteger(engine.GetProperty(raw, "length"));
            var builder = new System.Text.StringBuilder();

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);
                builder.Append(engine.ToStringValue(engine.GetIndexed(raw, JsValue.Number(at))));

                if (at + 1 < length && at + 1 < arguments.Length)
                {
                    builder.Append(engine.ToStringValue(arguments[(int)at + 1]));
                }
            }

            StringCharge(engine, builder.Length);
            return JsValue.String(builder.ToString());
        });

        Method(prototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.String(StringThis(engine, thisValue));
        });

        // `normalize` EXISTS, VALIDATES ITS FORM, AND REFUSES THE CASE IT CANNOT ANSWER.
        //
        // Every composition here runs with globalization-invariant mode on, and in that mode the
        // platform's own `String.Normalize` RETURNS THE INPUT UNCHANGED and reports that it is
        // already normalized. That is not an approximation, it is a wrong answer that looks like a
        // right one - the exact shape this profile refused for regular expressions and for `Date`.
        // Implementing normalization properly needs the Unicode decomposition and composition
        // tables, which is a data set this component does not hold.
        //
        // So: the four forms are validated, because an unknown form is a `RangeError` in every
        // engine and that clause is answerable without any table. A string that is entirely ASCII
        // is returned unchanged, because all four forms are the identity over ASCII and that is
        // provable rather than assumed. Anything else is refused BY NAME, so a program that needs
        // real normalization is told so rather than handed its input back.
        Method(prototype, "normalize", 0, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var stated = ArgOfString(arguments, 0);

            var form = stated.Type == JsType.Undefined
                ? "NFC"
                : engine.ToStringValue(stated);

            if (form is not ("NFC" or "NFD" or "NFKC" or "NFKD"))
            {
                return engine.ThrowRangeError(
                    "the normalization form must be one of NFC, NFD, NFKC and NFKD");
            }

            StringCharge(engine, text.Length);

            foreach (var unit in text)
            {
                if (unit > 0x7F)
                {
                    return engine.ThrowTypeError(
                        "String.prototype.normalize is implemented for ASCII only in this build, " +
                        "and this string is not ASCII; the Unicode normalization tables are not " +
                        "held by this component");
                }
            }

            return JsValue.String(text);
        });

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.String(StringThis(engine, thisValue));
        });

        Method(prototype, "charAt", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var at = engine.ToInteger(ArgOfString(arguments, 0));
            StringCharge(engine, 1);

            return at < 0 || at >= text.Length
                ? JsValue.String(string.Empty)
                : JsValue.String(text[(int)at].ToString());
        });

        Method(prototype, "charCodeAt", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var at = engine.ToInteger(ArgOfString(arguments, 0));
            StringCharge(engine, 1);

            return at < 0 || at >= text.Length
                ? JsValue.Number(double.NaN)
                : JsValue.Number(text[(int)at]);
        });

        Method(prototype, "codePointAt", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var position = engine.ToInteger(ArgOfString(arguments, 0));
            StringCharge(engine, 1);

            if (position < 0 || position >= text.Length)
            {
                return JsValue.Undefined;
            }

            var at = (int)position;
            var first = text[at];

            // A CODE POINT IS NOT A CODE UNIT. `codePointAt` is the method that exists to say so:
            // a high surrogate followed by a low one is one astral character, and reporting the
            // surrogate is what `charCodeAt` is for.
            return char.IsHighSurrogate(first) &&
                at + 1 < text.Length &&
                char.IsLowSurrogate(text[at + 1])
                ? JsValue.Number(char.ConvertToUtf32(first, text[at + 1]))
                : JsValue.Number(first);
        });

        Method(prototype, "at", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var relative = engine.ToInteger(ArgOfString(arguments, 0));
            var at = relative < 0 ? text.Length + relative : relative;
            StringCharge(engine, 1);

            return at < 0 || at >= text.Length
                ? JsValue.Undefined
                : JsValue.String(text[(int)at].ToString());
        });

        Method(prototype, "indexOf", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var search = engine.ToStringValue(ArgOfString(arguments, 0));
            var start = StringBoundIndex(engine.ToInteger(ArgOfString(arguments, 1)), text.Length);
            StringCharge(engine, text.Length + search.Length);
            return JsValue.Number(text.IndexOf(search, start, System.StringComparison.Ordinal));
        });

        Method(prototype, "lastIndexOf", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var search = engine.ToStringValue(ArgOfString(arguments, 0));
            var requested = engine.ToNumber(ArgOfString(arguments, 1));

            // AN OMITTED POSITION IS +INFINITY AND NOT ZERO. `ToInteger` would turn the NaN an
            // omitted argument converts to into 0, which would make every `lastIndexOf` an
            // `indexOf` at position zero.
            var position = double.IsNaN(requested)
                ? double.PositiveInfinity
                : JsValue.ToInteger(requested);

            var start = StringBoundIndex(position, text.Length);
            StringCharge(engine, text.Length + search.Length);
            var limit = System.Math.Min(start, text.Length - search.Length);

            for (var at = limit; at >= 0; at--)
            {
                if (string.CompareOrdinal(text, at, search, 0, search.Length) == 0)
                {
                    return JsValue.Number(at);
                }
            }

            return JsValue.Number(-1);
        });

        Method(prototype, "includes", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var search = engine.ToStringValue(ArgOfString(arguments, 0));
            var start = StringBoundIndex(engine.ToInteger(ArgOfString(arguments, 1)), text.Length);
            StringCharge(engine, text.Length + search.Length);
            return JsValue.Boolean(text.IndexOf(search, start, System.StringComparison.Ordinal) >= 0);
        });

        Method(prototype, "startsWith", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var search = engine.ToStringValue(ArgOfString(arguments, 0));
            var start = StringBoundIndex(engine.ToInteger(ArgOfString(arguments, 1)), text.Length);
            StringCharge(engine, search.Length + 1);

            return start + search.Length > text.Length
                ? JsValue.False
                : JsValue.Boolean(string.CompareOrdinal(text, start, search, 0, search.Length) == 0);
        });

        Method(prototype, "endsWith", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var search = engine.ToStringValue(ArgOfString(arguments, 0));
            var endArgument = ArgOfString(arguments, 1);

            var end = endArgument.Type == JsType.Undefined
                ? text.Length
                : StringBoundIndex(engine.ToInteger(endArgument), text.Length);

            var start = end - search.Length;
            StringCharge(engine, search.Length + 1);

            return start < 0
                ? JsValue.False
                : JsValue.Boolean(string.CompareOrdinal(text, start, search, 0, search.Length) == 0);
        });

        Method(prototype, "slice", 2, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var from = StringRelativeIndex(engine.ToInteger(ArgOfString(arguments, 0)), text.Length);
            var endArgument = ArgOfString(arguments, 1);

            var to = endArgument.Type == JsType.Undefined
                ? text.Length
                : StringRelativeIndex(engine.ToInteger(endArgument), text.Length);

            return StringSlice(engine, text, from, to < from ? from : to);
        });

        Method(prototype, "substring", 2, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var first = StringBoundIndex(engine.ToInteger(ArgOfString(arguments, 0)), text.Length);
            var endArgument = ArgOfString(arguments, 1);

            var second = endArgument.Type == JsType.Undefined
                ? text.Length
                : StringBoundIndex(engine.ToInteger(endArgument), text.Length);

            // `substring` SWAPS a reversed pair where `slice` returns nothing. The two differ in
            // exactly this and in how they read a negative index, and a program that uses the wrong
            // one is silently right until an argument goes backwards.
            return first <= second
                ? StringSlice(engine, text, first, second)
                : StringSlice(engine, text, second, first);
        });

        Method(prototype, "substr", 2, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var requested = engine.ToInteger(ArgOfString(arguments, 0));

            var start = requested < 0
                ? System.Math.Max(text.Length + requested, 0)
                : System.Math.Min(requested, text.Length);

            var lengthArgument = ArgOfString(arguments, 1);

            var size = lengthArgument.Type == JsType.Undefined
                ? text.Length - start
                : engine.ToInteger(lengthArgument);

            size = System.Math.Min(System.Math.Max(size, 0), text.Length - start);
            var from = (int)start;
            return StringSlice(engine, text, from, from + (int)size);
        });

        Method(prototype, "toUpperCase", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.String(StringChangeCase(engine, StringThis(engine, thisValue), true));
        });

        Method(prototype, "toLowerCase", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.String(StringChangeCase(engine, StringThis(engine, thisValue), false));
        });

        Method(prototype, "toLocaleUpperCase", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.String(StringChangeCase(engine, StringThis(engine, thisValue), true));
        });

        Method(prototype, "toLocaleLowerCase", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.String(StringChangeCase(engine, StringThis(engine, thisValue), false));
        });

        Method(prototype, "trim", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.String(StringTrimEnds(engine, StringThis(engine, thisValue), true, true));
        });

        Method(prototype, "trimStart", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.String(StringTrimEnds(engine, StringThis(engine, thisValue), true, false));
        });

        Method(prototype, "trimEnd", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.String(StringTrimEnds(engine, StringThis(engine, thisValue), false, true));
        });

        Method(prototype, "split", 2, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var separatorArgument = ArgOfString(arguments, 0);
            var limitArgument = ArgOfString(arguments, 1);

            var limit = limitArgument.Type == JsType.Undefined
                ? uint.MaxValue
                : engine.ToUint32(limitArgument);

            var array = engine.Realm.NewArray();

            if (limit == 0)
            {
                return JsValue.Object(array);
            }

            if (separatorArgument.Type == JsType.Undefined)
            {
                StringCharge(engine, text.Length);
                array.Push(JsValue.String(text));
                return JsValue.Object(array);
            }

            var separator = engine.ToStringValue(separatorArgument);
            StringCharge(engine, text.Length + separator.Length);

            if (text.Length == 0)
            {
                // "".split("") IS THE EMPTY ARRAY and "".split(",") IS [""]. The empty separator
                // matches the empty string, and a match that consumes the whole subject leaves no
                // piece behind.
                if (separator.Length != 0)
                {
                    array.Push(JsValue.String(string.Empty));
                }

                return JsValue.Object(array);
            }

            if (separator.Length == 0)
            {
                for (var at = 0; at < text.Length && array.Length < limit; at++)
                {
                    array.Push(JsValue.String(text[at].ToString()));
                }

                return JsValue.Object(array);
            }

            var start = 0;

            while (true)
            {
                var found = text.IndexOf(separator, start, System.StringComparison.Ordinal);

                if (found < 0)
                {
                    break;
                }

                array.Push(JsValue.String(text[start..found]));

                if (array.Length >= limit)
                {
                    return JsValue.Object(array);
                }

                start = found + separator.Length;
            }

            array.Push(JsValue.String(text[start..]));
            return JsValue.Object(array);
        });

        Method(prototype, "concat", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            StringCharge(engine, text.Length);
            var builder = new System.Text.StringBuilder(text);

            foreach (var argument in arguments)
            {
                var piece = engine.ToStringValue(argument);
                StringCharge(engine, piece.Length);
                builder.Append(piece);
            }

            return JsValue.String(builder.ToString());
        });

        Method(prototype, "repeat", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var count = engine.ToInteger(ArgOfString(arguments, 0));

            if (count < 0 || double.IsPositiveInfinity(count))
            {
                throw engine.Error("RangeError", "Invalid count value");
            }

            if (count == 0 || text.Length == 0)
            {
                return JsValue.String(string.Empty);
            }

            var total = count * text.Length;

            if (total > StringLengthCeiling)
            {
                throw engine.Error("RangeError", "Invalid string length");
            }

            var size = (int)total;
            StringCharge(engine, size);
            var builder = new System.Text.StringBuilder(size);

            for (var turn = 0; turn < (int)count; turn++)
            {
                builder.Append(text);
            }

            return JsValue.String(builder.ToString());
        });

        Method(prototype, "padStart", 1, static (engine, thisValue, arguments) =>
            StringPadTo(engine, thisValue, arguments, true));

        Method(prototype, "padEnd", 1, static (engine, thisValue, arguments) =>
            StringPadTo(engine, thisValue, arguments, false));

        Method(prototype, "replace", 2, static (engine, thisValue, arguments) =>
            StringReplaceWith(engine, thisValue, arguments, false));

        Method(prototype, "replaceAll", 2, static (engine, thisValue, arguments) =>
            StringReplaceWith(engine, thisValue, arguments, true));

        Method(prototype, "localeCompare", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var other = engine.ToStringValue(ArgOfString(arguments, 0));
            StringCharge(engine, text.Length + other.Length);

            // THE COMPARISON IS ORDINAL, which is a declared deviation: the specification allows any
            // locale-sensitive order and requires only that the result be a consistent total order.
            // An ordinal one is consistent, reproducible across hosts, and the same order `<` uses.
            return JsValue.Number(System.Math.Sign(string.CompareOrdinal(text, other)));
        });
    }

    /// <summary>Reads one argument, which may not have been passed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=23C5BB
    // Broiler-Human:        PENDING
    private static JsValue ArgOfString(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>Charges fuel for work proportional to <paramref name="units"/> characters.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3A5162
    // Broiler-Human:        PENDING
    private static void StringCharge(JsEngine engine, int units) =>
        engine.Charge(units <= 0 ? 1UL : (ulong)units);

    /// <summary>
    /// The receiver of a <c>String.prototype</c> method, as a String.
    /// </summary>
    /// <remarks>
    /// The three cases are the specification's <c>RequireObjectCoercible</c> followed by
    /// <c>ToString</c>: <c>null</c> and <c>undefined</c> are a <c>TypeError</c>, a String primitive
    /// and a boxed String are themselves, and anything else converts.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=17E2F8
    // Broiler-Human:        PENDING
    private static string StringThis(JsEngine engine, JsValue value)
    {
        if (value.IsNullish)
        {
            throw engine.Error(
                "TypeError", "String.prototype method called on null or undefined");
        }

        if (value.IsString)
        {
            return value.AsString();
        }

        if (value.IsObject &&
            value.AsObject() is JsPrimitiveWrapper wrapper &&
            wrapper.Primitive.IsString)
        {
            return wrapper.Primitive.AsString();
        }

        return engine.ToStringValue(value);
    }

    /// <summary>Clamps an absolute position into <c>[0, length]</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=50210D
    // Broiler-Human:        PENDING
    private static int StringBoundIndex(double position, int length)
    {
        if (position <= 0)
        {
            return 0;
        }

        return position >= length ? length : (int)position;
    }

    /// <summary>Resolves a relative position, where a negative one counts from the end.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=33F84B
    // Broiler-Human:        PENDING
    private static int StringRelativeIndex(double position, int length)
    {
        if (position < 0)
        {
            var shifted = length + position;
            return shifted <= 0 ? 0 : (int)shifted;
        }

        return position >= length ? length : (int)position;
    }

    /// <summary>Takes <c>[from, to)</c> of <paramref name="text"/>, charging for what it copies.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=377A20
    // Broiler-Human:        PENDING
    private static JsValue StringSlice(JsEngine engine, string text, int from, int to)
    {
        var count = to - from;
        StringCharge(engine, count);

        return count <= 0
            ? JsValue.String(string.Empty)
            : JsValue.String(text[from..to]);
    }

    /// <summary>Maps case under the invariant culture, which is the only one this profile has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9A8E1A
    // Broiler-Human:        PENDING
    private static string StringChangeCase(JsEngine engine, string text, bool upper)
    {
        StringCharge(engine, text.Length);
        var info = System.Globalization.CultureInfo.InvariantCulture.TextInfo;
        return upper ? info.ToUpper(text) : info.ToLower(text);
    }

    /// <summary>Trims the ECMAScript whitespace set from one or both ends.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=66EDA3
    // Broiler-Human:        PENDING
    private static string StringTrimEnds(JsEngine engine, string text, bool head, bool tail)
    {
        StringCharge(engine, text.Length);
        var from = 0;
        var to = text.Length;

        if (head)
        {
            while (from < to && JsNumberFormat.IsWhiteSpace(text[from]))
            {
                from++;
            }
        }

        if (tail)
        {
            while (to > from && JsNumberFormat.IsWhiteSpace(text[to - 1]))
            {
                to--;
            }
        }

        return from == 0 && to == text.Length ? text : text[from..to];
    }

    /// <summary>The shared body of <c>padStart</c> and <c>padEnd</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=23990B
    // Broiler-Human:        PENDING
    private static JsValue StringPadTo(
        JsEngine engine, JsValue thisValue, JsValue[] arguments, bool atStart)
    {
        var text = StringThis(engine, thisValue);
        var target = engine.ToInteger(ArgOfString(arguments, 0));

        if (target <= text.Length)
        {
            StringCharge(engine, text.Length);
            return JsValue.String(text);
        }

        if (target > StringLengthCeiling)
        {
            throw engine.Error("RangeError", "Invalid string length");
        }

        var fillArgument = ArgOfString(arguments, 1);

        var filler = fillArgument.Type == JsType.Undefined
            ? " "
            : engine.ToStringValue(fillArgument);

        if (filler.Length == 0)
        {
            StringCharge(engine, text.Length);
            return JsValue.String(text);
        }

        var total = (int)target;
        StringCharge(engine, total);
        var builder = new System.Text.StringBuilder(total);

        if (!atStart)
        {
            builder.Append(text);
        }

        for (var written = 0; written < total - text.Length; written++)
        {
            builder.Append(filler[written % filler.Length]);
        }

        if (atStart)
        {
            builder.Append(text);
        }

        return JsValue.String(builder.ToString());
    }

    /// <summary>
    /// The shared body of <c>replace</c> and <c>replaceAll</c>, over a string pattern.
    /// </summary>
    /// <remarks>
    /// The pattern is a string and never a RegExp: this file is built before <c>RegExp</c> exists,
    /// and an object pattern converts through <c>ToString</c> rather than matching. That is a
    /// declared deviation - <c>"a1".replace(/\d/, "x")</c> here looks for the literal characters of
    /// the regular expression's source and finds nothing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3B939C
    // Broiler-Human:        PENDING
    private static JsValue StringReplaceWith(
        JsEngine engine, JsValue thisValue, JsValue[] arguments, bool all)
    {
        var text = StringThis(engine, thisValue);
        var pattern = engine.ToStringValue(ArgOfString(arguments, 0));
        var replacement = ArgOfString(arguments, 1);
        StringCharge(engine, text.Length + pattern.Length);

        var found = text.IndexOf(pattern, System.StringComparison.Ordinal);

        if (found < 0)
        {
            return JsValue.String(text);
        }

        var callable = replacement.IsObject && replacement.AsObject().IsCallable;
        var template = callable ? string.Empty : engine.ToStringValue(replacement);

        if (!callable)
        {
            StringCharge(engine, template.Length);
        }

        var builder = new System.Text.StringBuilder(text.Length);
        var copied = 0;

        while (true)
        {
            builder.Append(text, copied, found - copied);

            if (callable)
            {
                var produced = engine.Call(
                    replacement,
                    JsValue.Undefined,
                    [JsValue.String(pattern), JsValue.Number(found), JsValue.String(text)]);

                var rendered = engine.ToStringValue(produced);
                StringCharge(engine, rendered.Length);
                builder.Append(rendered);
            }
            else
            {
                builder.Append(StringExpandDollars(template, pattern, text, found));
            }

            copied = found + pattern.Length;

            if (!all)
            {
                break;
            }

            // AN EMPTY PATTERN ADVANCES BY ONE. Without that, `"abc".replaceAll("", "-")` matches
            // at position zero forever; with it, it matches at every boundary, which is the four
            // matches the specification's `advanceBy` produces.
            var from = found + (pattern.Length == 0 ? 1 : pattern.Length);

            if (from > text.Length)
            {
                break;
            }

            found = text.IndexOf(pattern, from, System.StringComparison.Ordinal);

            if (found < 0)
            {
                break;
            }

            StringCharge(engine, 1);
        }

        builder.Append(text, copied, text.Length - copied);
        return JsValue.String(builder.ToString());
    }

    /// <summary>
    /// Expands the dollar escapes a string replacement may carry.
    /// </summary>
    /// <remarks>
    /// <c>$$</c> is one dollar, <c>$&amp;</c> the match, <c>$`</c> what came before it and
    /// <c>$'</c> what came after. A dollar followed by anything else is a dollar: the specification
    /// leaves an unrecognised escape alone rather than dropping it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=55285F
    // Broiler-Human:        PENDING
    private static string StringExpandDollars(
        string template, string matched, string text, int position)
    {
        if (template.IndexOf('$', System.StringComparison.Ordinal) < 0)
        {
            return template;
        }

        var builder = new System.Text.StringBuilder(template.Length);

        for (var at = 0; at < template.Length; at++)
        {
            var character = template[at];

            if (character != '$' || at + 1 >= template.Length)
            {
                builder.Append(character);
                continue;
            }

            switch (template[at + 1])
            {
                case '$':
                    builder.Append('$');
                    at++;
                    break;

                case '&':
                    builder.Append(matched);
                    at++;
                    break;

                case '`':
                    builder.Append(text, 0, position);
                    at++;
                    break;

                case '\'':
                    builder.Append(
                        text,
                        position + matched.Length,
                        text.Length - position - matched.Length);

                    at++;
                    break;

                default:
                    builder.Append(character);
                    break;
            }
        }

        return builder.ToString();
    }
}
