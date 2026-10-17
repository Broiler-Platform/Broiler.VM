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
// Security risk:    High
// Criteria:         2/2
// Resource impact:  4/10 max
// Unverified:       19
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript;
using Broiler.ECMAScript.Numbers;
using Broiler.ECMAScript.Text;
using Broiler.Unicode.Algorithms;

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=36D659
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
                builder.Append((char)IntegerConversions.ToUint16(engine.ToNumber(argument)));
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
                        JsNumberFormat.NumberToString(code) + " is not a valid code point");
                }

                // A SURROGATE CODE POINT IS A LEGAL ARGUMENT AND ONE CODE UNIT. The platform's
                // converter refuses one - it encodes SCALAR values, and a surrogate is not one -
                // and the exception it raises is not a JavaScript error: it ended the whole
                // invocation as a contract violation. The language says `fromCodePoint(0xD800)` is
                // a String of length one holding that unit, which is the same thing
                // `fromCharCode(0xD800)` answers.
                if (code >= 0xD800 && code <= 0xDFFF)
                {
                    builder.Append((char)code);
                    continue;
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
            return JsValue.String(StringThisValue(engine, thisValue, "toString"));
        });

        // `normalize` IS THE UNICODE NORMALIZATION OF THE STRING'S CODE POINTS, read from the
        // Unicode 17.0.0 tables JSD-0031 pins and never from the platform: the platform's
        // `String.Normalize` returns its input unchanged under globalization-invariant mode and
        // throws on a lone surrogate the language requires to pass through (rule N23).
        //
        // The order is the specification's: the receiver is coerced first, then the form, and an
        // unknown form is a `RangeError` before any text is looked at. The work itself is
        // `NormalizeText`'s, which charges for what it scans and what it builds.
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

            return JsValue.String(NormalizeText(engine, text, compose: form is "NFC" or "NFKC", compatibility: form is "NFKC" or "NFKD"));
        });

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.String(StringThisValue(engine, thisValue, "valueOf"));
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

            // A CODE POINT IS NOT A CODE UNIT. `codePointAt` is the method that exists to say so:
            // a high surrogate followed by a low one is one astral character, and reporting the
            // surrogate is what `charCodeAt` is for. The reading is the shared library's CodePointAt
            // (JSD-0070).
            return JsValue.Number(Utf16.CodePointAt(text, (int)position).CodePoint);
        });

        Method(prototype, "at", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            var relative = engine.ToInteger(ArgOfString(arguments, 0));
            StringCharge(engine, 1);

            return RelativeIndex.TryElement(relative, text.Length, out var at)
                ? JsValue.String(text[(int)at].ToString())
                : JsValue.Undefined;
        });

        // A STRING IS A SEQUENCE OF UTF-16 UNITS AND NOT OF TEXT, and these two are where the
        // language admits it. A lone surrogate is a legal String and an illegal scalar sequence, so
        // a program handing one to anything that encodes - a URL, a file, a socket - has to know
        // first, and until these existed the only way to ask was to write the surrogate arithmetic
        // by hand.
        Method(prototype, "isWellFormed", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var text = StringThis(engine, thisValue);

            // The shared IsStringWellFormedUnicode charges the text's length and one more, first.
            return JsValue.Boolean(Utf16.IsWellFormed(text, engine.WorkCharge));
        });

        Method(prototype, "toWellFormed", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var text = StringThis(engine, thisValue);

            // THE REPLACEMENT IS ONE UNIT PER LONE SURROGATE, so the answer has the same length as
            // the receiver, and a well-formed receiver is the answer itself. The shared toWellFormed
            // charges the text's length and one more, first.
            return JsValue.String(Utf16.ToWellFormed(text, engine.WorkCharge));
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
                : IntegerConversions.ToIntegerOrInfinity(requested);

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
            RegExpRefuseAsSearchString(engine, ArgOfString(arguments, 0), "includes");
            var search = engine.ToStringValue(ArgOfString(arguments, 0));
            var start = StringBoundIndex(engine.ToInteger(ArgOfString(arguments, 1)), text.Length);
            StringCharge(engine, text.Length + search.Length);
            return JsValue.Boolean(text.IndexOf(search, start, System.StringComparison.Ordinal) >= 0);
        });

        Method(prototype, "startsWith", 1, static (engine, thisValue, arguments) =>
        {
            var text = StringThis(engine, thisValue);
            RegExpRefuseAsSearchString(engine, ArgOfString(arguments, 0), "startsWith");
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
            RegExpRefuseAsSearchString(engine, ArgOfString(arguments, 0), "endsWith");
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
            var from = (int)RelativeIndex.Clamped(engine.ToInteger(ArgOfString(arguments, 0)), text.Length);
            var endArgument = ArgOfString(arguments, 1);

            var to = endArgument.Type == JsType.Undefined
                ? text.Length
                : (int)RelativeIndex.Clamped(engine.ToInteger(endArgument), text.Length);

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
            var start = (double)RelativeIndex.Clamped(engine.ToInteger(ArgOfString(arguments, 0)), text.Length);

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
            JsValue.String(TransformCase(engine, StringThis(engine, thisValue), arguments, true)));

        Method(prototype, "toLocaleLowerCase", 0, static (engine, thisValue, arguments) =>
            JsValue.String(TransformCase(engine, StringThis(engine, thisValue), arguments, false)));

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

        // ANNEX B'S TWO OLD NAMES ARE THE SAME FUNCTION OBJECTS AS THE NEW ONES, as the language
        // says: `trimLeft === trimStart` and `trimLeft.name` is "trimStart" (JSP-7, JSC-239).
        StringAlias(prototype, "trimLeft", "trimStart");
        StringAlias(prototype, "trimRight", "trimEnd");

        // ANNEX B'S HTML METHODS, every one of them (JSP-7, JSC-239). The realm admits the whole of
        // Annex B.2 rather than the part of it someone happened to write, which is the rule
        // section 6 of the roadmap now states; each is `CreateHTML` with its tag and attribute.
        foreach (var (name, tag, attribute) in new (string, string, string)[]
        {
            ("anchor", "a", "name"), ("big", "big", ""), ("blink", "blink", ""), ("bold", "b", ""),
            ("fixed", "tt", ""), ("fontcolor", "font", "color"), ("fontsize", "font", "size"),
            ("italics", "i", ""), ("link", "a", "href"), ("small", "small", ""),
            ("strike", "strike", ""), ("sub", "sub", ""), ("sup", "sup", ""),
        })
        {
            Method(prototype, name, attribute.Length == 0 ? 0 : 1, (engine, thisValue, arguments) =>
                JsValue.String(StringCreateHtml(engine, thisValue, tag, attribute, ArgOfString(arguments, 0))));
        }

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

            // WITH `Intl`, ECMA-402 s20.1.1: A COLLATOR FROM THE LOCALES AND OPTIONS, made by the
            // realm's own %Intl.Collator% whatever the global binding now holds (JSD-0043).
            if (engine.Realm.CollatorConstructor is { } constructor)
            {
                var made = NewCollator(
                    engine,
                    JsValue.Object(constructor),
                    [ArgOfString(arguments, 1), ArgOfString(arguments, 2)]);

                return JsValue.Number(((JsCollatorObject)made.AsObject()).Collator.Compare(engine, text, other));
            }

            // THE ORDER IS ORDINAL OVER THE CANONICAL DECOMPOSITIONS. The specification allows any
            // order that is consistent and total, and requires one more thing ECMA-262 states
            // without ECMA-402: canonically equivalent strings compare as 0. Comparing NFD forms
            // gives exactly that, since two strings are canonically equivalent when their NFD
            // forms are equal; until 2026-10-03 the raw code units were compared, and `'\u00e4'` and
            // `'a\u0308'` were unequal (JSD-0027 N3). Two strings already equal skip the work, and
            // the comparison stays reproducible across hosts: NFD is the pinned tables' own.
            if (string.Equals(text, other, System.StringComparison.Ordinal))
            {
                return JsValue.Number(0);
            }

            var left = NormalizeText(engine, text, compose: false, compatibility: false);
            var right = NormalizeText(engine, other, compose: false, compatibility: false);
            return JsValue.Number(System.Math.Sign(string.CompareOrdinal(left, right)));
        });
    }

    /// <summary>
    /// The Unicode normalization of <paramref name="text"/>'s code points: NFD, NFKD with
    /// <paramref name="compatibility"/>, and NFC or NFKC with <paramref name="compose"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The algorithm is the Unicode area's owner's since SJL-7.4</b> (JSD-0074; JSD-0075):
    /// <c>UnicodeNormalization.Transform</c>, UAX #15 over the tables this family embeds, which rule N22
    /// holds to the owner's generator. A lone surrogate is read as the code point it is - class 0, no
    /// mapping, no composite - so it passes through where the language says it does. Until SJL-7.4 the
    /// passes were this profile's own (<c>IsNormalizedAlready</c>, <c>OrderCanonically</c>,
    /// <c>ComposeCanonically</c> and <c>WriteCodePoints</c>), over its own reading of the same tables.
    /// </para>
    /// <para>
    /// <b>The charge is this engine's, in its units</b> (correspondence row N7, JSD-0074's decision 4):
    /// the text's length, and then, unless the text is in the form already, its length plus three units
    /// per code point of its full decomposition, each charged through this engine's meter before the
    /// work it pays for. A text in the form is answered as itself after the first charge, which is the
    /// fast path ASCII always had.
    /// </para>
    /// <para>
    /// <b>The ceiling is checked on the result</b>, the <c>RangeError</c> <c>repeat</c> gives past
    /// <see cref="StringLengthCeiling"/> UTF-16 units. This corrects row N6: the profile's own passes
    /// checked the decomposition, so an NFC or NFKC result that would fit could be refused. It also
    /// refuses a text that is in the form already and longer than the ceiling, which the profile's
    /// quick check returned as it came (JSD-0075).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=0BE9D3
    // Broiler-Falsified-If: a guest string makes normalize allocate or loop over an expansion it was not charged for, or answer other than the pinned NormalizationTest.txt vectors
    // Broiler-Human:        PENDING
    internal static string NormalizeText(JsEngine engine, string text, bool compose, bool compatibility)
    {
        var form = compose
            ? compatibility ? NormalForm.Nfkc : NormalForm.Nfc
            : compatibility ? NormalForm.Nfkd : NormalForm.Nfd;

        return JsUnicode.Normalization.Transform(text, form, StringLengthCeiling, engine.UnicodeCharge, out var result) == UnicodeStatus.Success
            ? result!
            : throw engine.Error("RangeError", "Invalid string length");
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
    /// Defines <paramref name="alias"/> on <paramref name="host"/> as the very function object
    /// <paramref name="name"/> already holds.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=91ADF7
    // Broiler-Human:        PENDING
    private static void StringAlias(JsObject host, string alias, string name)
    {
        _ = host.TryGetOwnProperty(name, out var existing);

        host.SetOwnProperty(
            alias,
            JsProperty.Data(existing.Value, JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>
    /// Annex B's <c>CreateHTML</c>: the receiver's text wrapped in <paramref name="tag"/>, with
    /// <paramref name="attribute"/> set to <paramref name="value"/> when there is one.
    /// </summary>
    /// <remarks>
    /// The receiver is converted before the value, and a <c>"</c> in the value is written as
    /// <c>&amp;quot;</c>, which is the whole of the escaping the language does: nothing else in either
    /// text is touched.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0AD7F3
    // Broiler-Human:        PENDING
    private static string StringCreateHtml(
        JsEngine engine, JsValue thisValue, string tag, string attribute, JsValue value)
    {
        var text = StringThis(engine, thisValue);
        var opened = new System.Text.StringBuilder().Append('<').Append(tag);

        if (attribute.Length != 0)
        {
            var written = engine.ToStringValue(value);
            StringCharge(engine, written.Length);

            opened
                .Append(' ')
                .Append(attribute)
                .Append("=\"")
                .Append(written.Replace("\"", "&quot;", System.StringComparison.Ordinal))
                .Append('"');
        }

        StringCharge(engine, text.Length);
        return opened.Append('>').Append(text).Append("</").Append(tag).Append('>').ToString();
    }

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

    /// <summary>
    /// The Unicode Default Case Conversion over the pinned tables: full mappings, and Final_Sigma
    /// for GREEK CAPITAL LETTER SIGMA when lowering. No language is applied.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=06E58D
    // Broiler-Human:        PENDING
    private static string StringChangeCase(JsEngine engine, string text, bool upper) =>
        ChangeCase(engine, text, upper, CaseLanguage.Default);

    /// <summary>
    /// ECMA-402's <c>TransformCase</c> where the realm has <c>Intl</c>: the first requested locale, and
    /// the language rules of SpecialCasing.txt when it is Turkish, Azeri or Lithuanian; without
    /// <c>Intl</c>, the locale is ignored as JSD-0027 section 1 states.
    /// </summary>
    /// <remarks>
    /// The locale's resolution is this profile's, as it was; the mapping is the owner's since SJL-7.4
    /// (JSD-0074, decision 8): <c>tr</c> and <c>az</c> are <c>CaseLanguage.Turkic</c>, <c>lt</c> is
    /// <c>CaseLanguage.Lithuanian</c>, and anything else, <c>und</c> included, the default.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s20.1.2.1; IP=Low; Security=Medium; Resources=3; Fingerprint=84DDA4
    // Broiler-Human:        PENDING
    private static string TransformCase(JsEngine engine, string text, JsValue[] arguments, bool upper)
    {
        if (engine.Realm.CollatorConstructor is null)
        {
            return StringChangeCase(engine, text, upper);
        }

        var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
        var locale = JsLocaleTag.Parse(requested.Count != 0 ? requested[0] : DefaultLocale)!;
        var language = LookupMatchingLocale(["az", "lt", "tr"], [locale.WithoutUnicodeExtension()])?.Locale ?? "und";

        return ChangeCase(engine, text, upper, language switch
        {
            "tr" or "az" => CaseLanguage.Turkic,
            "lt" => CaseLanguage.Lithuanian,
            _ => CaseLanguage.Default,
        });
    }

    /// <summary>
    /// The full case mapping of <paramref name="text"/> in <paramref name="language"/>, by the Unicode
    /// area's owner (<c>UnicodeCaseMapping</c>, JSD-0074), charged in this engine's units.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The charge is the one this profile always made</b> (correspondence row C13; JSD-0074,
    /// decision 4). The owner charges the default pass through this engine's meter: the length of its
    /// input, each mapping's length in code points, and the code units each Final_Sigma test reads. The
    /// language pass's extra input length is charged here, before the call and only for a language,
    /// which is the split JSD-0065's decision 2 made for number parsing: until SJL-7.4 this profile's
    /// <c>LanguageCasing</c> charged it and then handed its output to the default pass, as the owner
    /// does.
    /// </para>
    /// <para>
    /// <b>The ceiling is checked on the whole result</b>, the <c>RangeError</c> <c>repeat</c> gives
    /// past <see cref="StringLengthCeiling"/> UTF-16 units. This corrects row C12: the profile's own
    /// conversion checked it only after a mapping was appended, and not at all in the language pass. A
    /// text casing does not change and that is longer than the ceiling is refused too (JSD-0075).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=58BAEA
    // Broiler-Falsified-If: a case conversion charges a total, or exhausts at a point, other than this profile's own conversion did for the same text before SJL-7.4
    // Broiler-Human:        PENDING
    private static string ChangeCase(JsEngine engine, string text, bool upper, CaseLanguage language)
    {
        if (language != CaseLanguage.Default)
        {
            StringCharge(engine, text.Length);
        }

        var status = upper
            ? JsUnicode.Casing.ToUpperCase(text, language, StringLengthCeiling, engine.UnicodeCharge, out var result)
            : JsUnicode.Casing.ToLowerCase(text, language, StringLengthCeiling, engine.UnicodeCharge, out result);

        return status == UnicodeStatus.Success
            ? result!
            : throw engine.Error("RangeError", "Invalid string length");
    }

    /// <summary>Trims the ECMAScript whitespace set from one or both ends.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6F7755
    // Broiler-Human:        PENDING
    private static string StringTrimEnds(JsEngine engine, string text, bool head, bool tail)
    {
        StringCharge(engine, text.Length);
        var from = 0;
        var to = text.Length;

        if (head)
        {
            while (from < to && NumberParsing.IsStrWhiteSpaceChar(text[from]))
            {
                from++;
            }
        }

        if (tail)
        {
            while (to > from && NumberParsing.IsStrWhiteSpaceChar(text[to - 1]))
            {
                to--;
            }
        }

        return from == 0 && to == text.Length ? text : text[from..to];
    }

    /// <summary>The shared body of <c>padStart</c> and <c>padEnd</c>.</summary>
    /// <remarks>
    /// <b>The filler is converted before the ceiling is consulted</b>, as StringPaddingBuiltinsImpl
    /// converts it before StringPad, and an empty filler answers the receiver whatever the length
    /// (JSD-0070, corrections P1 and P2): until SJL-5.4 a length past the ceiling was refused first, so
    /// a filler's <c>toString</c> was never called and <c>"x".padEnd(2 ** 24 + 1, "")</c> threw. The
    /// builder is the shared StringPad under this family's ceiling, which charges the early answer
    /// max(1, length) and a built String its length.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=413A4D
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

        var fillArgument = ArgOfString(arguments, 1);

        var filler = fillArgument.Type == JsType.Undefined
            ? " "
            : engine.ToStringValue(fillArgument);

        // ToLength's ceiling, 2^53 - 1: the target is past the receiver's length, so it is positive.
        var maxLength = target >= 9007199254740991d ? 9007199254740991L : (long)target;
        var status = StringPadding.Pad(
            text,
            maxLength,
            filler,
            atStart ? PadPlacement.Start : PadPlacement.End,
            new LengthLimit(StringLengthCeiling),
            out var padded,
            engine.WorkCharge);

        return status == ResultStatus.Completed
            ? JsValue.String(padded)
            : throw engine.Error("RangeError", "Invalid string length");
    }

    /// <summary>
    /// The shared body of <c>replace</c> and <c>replaceAll</c>, over a string pattern.
    /// </summary>
    /// <remarks>
    /// The pattern is a string and never a RegExp: this file is built before <c>RegExp</c> exists,
    /// and an object pattern converts through <c>ToString</c> rather than matching. That is a
    /// declared deviation - <c>"a1".replace(/\d/, "x")</c> here looks for the literal characters of
    /// the regular expression's source and finds nothing. <c>SetupRegExp</c> replaces both methods
    /// with versions that dispatch through <c>Symbol.replace</c> first, and <c>replaceAll</c> still
    /// arrives here when its pattern does not answer that Symbol. <b>A replacement that is not callable
    /// is converted before the search</b>, as the language's step 7 does, so its <c>toString</c> runs
    /// once even when nothing matches (JSD-0070, correction S1); its template is parsed once by the
    /// shared parser and its parts are evaluated at each match.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=332A6C
    // Broiler-Human:        PENDING
    private static JsValue StringReplaceWith(
        JsEngine engine, JsValue thisValue, JsValue[] arguments, bool all)
    {
        var text = StringThis(engine, thisValue);
        var pattern = engine.ToStringValue(ArgOfString(arguments, 0));
        var replacement = ArgOfString(arguments, 1);
        var callable = replacement.IsObject && replacement.AsObject().IsCallable;
        var template = callable ? string.Empty : engine.ToStringValue(replacement);
        StringCharge(engine, text.Length + pattern.Length);

        var found = text.IndexOf(pattern, System.StringComparison.Ordinal);

        if (found < 0)
        {
            return JsValue.String(text);
        }

        ReplacementPart[] parts = [];

        if (!callable)
        {
            StringCharge(engine, template.Length);

            // A string pattern has no captures and no groups, so only `$$`, `$&`, `` $` `` and `$'`
            // are substitutions; the parse is charged above, as the template's length.
            parts = ReplacementTemplate.Parse(template, 0, false);
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
                StringSubstitute(builder, parts, template, pattern, text, found);
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
    /// Evaluates a replacement template's parts against one match of a string pattern, which has no
    /// captures and no groups: <c>$$</c> is one dollar, <c>$&amp;</c> the match, <c>$`</c> what came
    /// before it and <c>$'</c> what came after; every other part is the template's own text.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CD3EC1
    // Broiler-Human:        PENDING
    private static void StringSubstitute(
        System.Text.StringBuilder builder,
        ReplacementPart[] parts,
        string template,
        string matched,
        string text,
        int position)
    {
        foreach (var part in parts)
        {
            switch (part.Kind)
            {
                case ReplacementPartKind.Dollar:
                    builder.Append('$');
                    break;

                case ReplacementPartKind.Match:
                    builder.Append(matched);
                    break;

                case ReplacementPartKind.Before:
                    builder.Append(text, 0, position);
                    break;

                case ReplacementPartKind.After:
                    builder.Append(text, position + matched.Length, text.Length - position - matched.Length);
                    break;

                default:
                    builder.Append(template, part.Start, part.Length);
                    break;
            }
        }
    }

    /// <summary>
    /// The specification's <c>thisStringValue</c>: a String, or the String a String wrapper holds,
    /// and a <c>TypeError</c> for anything else.
    /// </summary>
    /// <remarks>
    /// <b><c>toString</c> and <c>valueOf</c> do not coerce</b>, unlike every other member of
    /// <c>String.prototype</c>: an object with its own <c>toString</c> is not a String. Until
    /// 2026-10-03 both coerced their receiver (JSC-252).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1BB13F
    // Broiler-Human:        PENDING
    private static string StringThisValue(JsEngine engine, JsValue value, string member)
    {
        if (value.IsString)
        {
            return value.AsString();
        }

        if (value.AsObjectOrNull() is JsPrimitiveWrapper wrapper && wrapper.Primitive.IsString)
        {
            return wrapper.Primitive.AsString();
        }

        throw engine.Error(
            "TypeError", "String.prototype." + member + " requires that 'this' be a String");
    }
}
