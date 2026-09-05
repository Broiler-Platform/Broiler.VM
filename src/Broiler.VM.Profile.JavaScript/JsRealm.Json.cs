// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   31
// Annotated:        31/31
// Exempt:           5
// Human-reviewed:   0/31
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       31
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>JSON</c> namespace object: a reader written from the grammar and a writer written from
/// <c>SerializeJSONProperty</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>The reader is a recursive-descent parser over the JSON grammar and not over JavaScript's.</b>
/// The two are close enough that reusing the script tokeniser looks free and is not: JSON has no
/// comments, no single-quoted strings, no trailing commas, no leading <c>+</c>, no <c>0x</c>, no
/// unquoted keys, and exactly four whitespace characters where the language has eleven. Every one of
/// those is something a lenient reader accepts and a conforming one must reject, and a lenient
/// <c>JSON.parse</c> is a security property lost rather than a convenience gained - it is the seam
/// where a document that one participant read as invalid and another read as data becomes a
/// disagreement about what was said.
/// </para>
/// <para>
/// <b>The writer's recursion carries the holder rather than the value.</b> The specification's
/// <c>SerializeJSONProperty(key, holder)</c> reads the value out of the holder itself, and it has to:
/// the replacer function is called with the holder as its receiver and the key as its first
/// argument, so a serialiser that had already extracted the value would have nothing left to pass.
/// The same shape is what makes <c>toJSON</c> observe the key it is being serialised under, which is
/// how <c>Date.prototype.toJSON</c> is reached and how an object can serialise differently depending
/// on where it sits.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>
    /// How deep either direction may nest before it stops rather than exhausting the CLR stack.
    /// </summary>
    /// <remarks>
    /// Both directions are recursive over a structure the guest chose the depth of, and the CLR has
    /// no recoverable stack overflow. A counted ceiling turns "the process dies" into "a RangeError
    /// is thrown", which is a difference the host can act on.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=68BDDE
    // Broiler-Human:        PENDING
    private const int JsonMaximumDepth = 512;

    /// <summary>The state one <c>JSON.stringify</c> call threads through its recursion.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F2B6A3
    // Broiler-Human:        PENDING
    private sealed class JsonWriter
    {
        /// <summary>The replacer, when it is callable; <c>undefined</c> otherwise.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=CBD8BB
        // Broiler-Human:        PENDING
        internal JsValue ReplacerFunction { get; set; } = JsValue.Undefined;

        /// <summary>The key allow-list an Array replacer produced, or nothing.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=339507
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<string>? PropertyList { get; set; }

        /// <summary>One level of indentation; empty when the output is not indented.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=ED61CE
        // Broiler-Human:        PENDING
        internal string Gap { get; set; } = string.Empty;

        /// <summary>The indentation in force at the current nesting level.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=10C1D6
        // Broiler-Human:        PENDING
        internal string Indent { get; set; } = string.Empty;

        /// <summary>The objects currently being serialised, innermost last.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A86FC1
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsObject> Stack { get; } = [];
    }

    /// <summary>Builds <c>JSON</c> and defines it on the global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DB1866
    // Broiler-Human:        PENDING
    private void SetupJson()
    {
        var json = new JsObject(ObjectPrototype, "JSON");

        Method(json, "parse", 2, (engine, thisValue, arguments) => JsonParseEntry(engine, arguments));
        Method(json, "stringify", 3, (engine, thisValue, arguments) => JsonStringifyEntry(engine, arguments));

        GlobalObject.DefineBuiltIn("JSON", JsValue.Object(json));
    }

    /// <summary>Reads one argument, which may not have been supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=307331
    // Broiler-Human:        PENDING
    private static JsValue JsonArgument(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    // ---- reading -----------------------------------------------------------------------------

    /// <summary><c>JSON.parse</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1335C9
    // Broiler-Human:        PENDING
    private JsValue JsonParseEntry(JsEngine engine, JsValue[] arguments)
    {
        var text = engine.ToStringValue(JsonArgument(arguments, 0));
        var at = 0;

        JsonSkipSpace(engine, text, ref at);
        var parsed = JsonParseValue(engine, text, ref at, 0);
        JsonSkipSpace(engine, text, ref at);

        if (at != text.Length)
        {
            throw JsonUnexpected(engine, text, at);
        }

        var reviver = JsonArgument(arguments, 1);

        if (!reviver.IsObject || !reviver.AsObject().IsCallable)
        {
            return parsed;
        }

        // The specification hands the reviver a synthetic holder whose one key is the empty string,
        // so the root value is revived by exactly the same rule as every value inside it.
        var root = new JsObject(ObjectPrototype);
        root.DefineOrdinary(string.Empty, parsed);
        return JsonInternalize(engine, root, string.Empty, reviver, 0);
    }

    /// <summary>Consumes the four characters JSON calls whitespace, and no others.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=C5FEB5
    // Broiler-Human:        PENDING
    private static void JsonSkipSpace(JsEngine engine, string text, ref int at)
    {
        var start = at;

        while (at < text.Length && text[at] is ' ' or '\t' or '\n' or '\r')
        {
            at++;
        }

        if (at > start)
        {
            engine.Charge((ulong)(at - start));
        }
    }

    /// <summary>Reads one JSON value, whatever kind it is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=32BA96
    // Broiler-Human:        PENDING
    private JsValue JsonParseValue(JsEngine engine, string text, ref int at, int depth)
    {
        engine.Charge(1);

        if (depth > JsonMaximumDepth)
        {
            throw engine.Error("RangeError", "Maximum JSON nesting depth exceeded");
        }

        if (at >= text.Length)
        {
            throw JsonUnexpected(engine, text, at);
        }

        switch (text[at])
        {
            case '{':
                return JsonParseObject(engine, text, ref at, depth);

            case '[':
                return JsonParseArray(engine, text, ref at, depth);

            case '"':
                return JsValue.String(JsonParseString(engine, text, ref at));

            case 't':
                return JsonParseKeyword(engine, text, ref at, "true", JsValue.True);

            case 'f':
                return JsonParseKeyword(engine, text, ref at, "false", JsValue.False);

            case 'n':
                return JsonParseKeyword(engine, text, ref at, "null", JsValue.Null);

            case '-':
                return JsonParseNumber(engine, text, ref at);

            default:
                if (text[at] is >= '0' and <= '9')
                {
                    return JsonParseNumber(engine, text, ref at);
                }

                throw JsonUnexpected(engine, text, at);
        }
    }

    /// <summary>Reads an object literal into an ordinary object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=32B99F
    // Broiler-Human:        PENDING
    private JsValue JsonParseObject(JsEngine engine, string text, ref int at, int depth)
    {
        at++;
        var target = new JsObject(ObjectPrototype);
        JsonSkipSpace(engine, text, ref at);

        if (at < text.Length && text[at] == '}')
        {
            at++;
            return JsValue.Object(target);
        }

        while (true)
        {
            JsonSkipSpace(engine, text, ref at);

            if (at >= text.Length || text[at] != '"')
            {
                throw JsonUnexpected(engine, text, at);
            }

            var key = JsonParseString(engine, text, ref at);
            JsonSkipSpace(engine, text, ref at);

            if (at >= text.Length || text[at] != ':')
            {
                throw JsonUnexpected(engine, text, at);
            }

            at++;
            engine.Charge(1);
            JsonSkipSpace(engine, text, ref at);
            var value = JsonParseValue(engine, text, ref at, depth + 1);

            // An unchecked define, which is what CreateDataProperty is here: a key of "__proto__"
            // becomes an own data property rather than reaching Object.prototype's setter, and a
            // repeated key replaces the earlier one in the earlier one's position.
            target.DefineOrdinary(key, value);
            JsonSkipSpace(engine, text, ref at);

            if (at < text.Length && text[at] == ',')
            {
                at++;
                engine.Charge(1);
                continue;
            }

            if (at < text.Length && text[at] == '}')
            {
                at++;
                return JsValue.Object(target);
            }

            throw JsonUnexpected(engine, text, at);
        }
    }

    /// <summary>Reads an array literal into an Array.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5B295D
    // Broiler-Human:        PENDING
    private JsValue JsonParseArray(JsEngine engine, string text, ref int at, int depth)
    {
        at++;
        var target = NewArray();
        JsonSkipSpace(engine, text, ref at);

        if (at < text.Length && text[at] == ']')
        {
            at++;
            return JsValue.Object(target);
        }

        while (true)
        {
            JsonSkipSpace(engine, text, ref at);
            target.Push(JsonParseValue(engine, text, ref at, depth + 1));
            JsonSkipSpace(engine, text, ref at);

            if (at < text.Length && text[at] == ',')
            {
                at++;
                engine.Charge(1);
                continue;
            }

            if (at < text.Length && text[at] == ']')
            {
                at++;
                return JsValue.Object(target);
            }

            throw JsonUnexpected(engine, text, at);
        }
    }

    /// <summary>Reads a string literal, resolving the escapes JSON has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=158C30
    // Broiler-Human:        PENDING
    private static string JsonParseString(JsEngine engine, string text, ref int at)
    {
        var start = at;
        at++;
        var built = new System.Text.StringBuilder();

        while (true)
        {
            if (at >= text.Length)
            {
                throw JsonSyntax(engine, "Unterminated string in JSON", at);
            }

            var character = text[at];

            if (character == '"')
            {
                at++;
                engine.Charge((ulong)(at - start));
                return built.ToString();
            }

            // A raw control character inside a string is the single most common way a hand-built
            // encoder produces something no conforming reader will take back.
            if (character < ' ')
            {
                throw JsonSyntax(engine, "Bad control character in JSON", at);
            }

            if (character != '\\')
            {
                built.Append(character);
                at++;
                continue;
            }

            at++;

            if (at >= text.Length)
            {
                throw JsonSyntax(engine, "Unexpected end of JSON input", at);
            }

            var escape = text[at];
            at++;

            switch (escape)
            {
                case '"':
                    built.Append('"');
                    break;

                case '\\':
                    built.Append('\\');
                    break;

                case '/':
                    built.Append('/');
                    break;

                case 'b':
                    built.Append('\b');
                    break;

                case 'f':
                    built.Append('\f');
                    break;

                case 'n':
                    built.Append('\n');
                    break;

                case 'r':
                    built.Append('\r');
                    break;

                case 't':
                    built.Append('\t');
                    break;

                case 'u':
                    built.Append(JsonParseHex(engine, text, ref at));
                    break;

                default:
                    throw JsonSyntax(engine, "Bad escaped character in JSON", at - 1);
            }
        }
    }

    /// <summary>Reads the four hexadecimal digits of a <c>\u</c> escape.</summary>
    /// <remarks>
    /// The result is one UTF-16 code unit and is not validated as a scalar value: <c>"\ud800"</c> is
    /// a well-formed JSON string holding a lone surrogate, and a String in this language is a
    /// sequence of code units rather than of scalars, so rejecting it would be inventing a rule.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=CB98BF
    // Broiler-Human:        PENDING
    private static char JsonParseHex(JsEngine engine, string text, ref int at)
    {
        if (at + 4 > text.Length)
        {
            throw JsonSyntax(engine, "Bad Unicode escape in JSON", at);
        }

        var unit = 0;

        for (var step = 0; step < 4; step++)
        {
            var digit = JsonHexDigit(text[at + step]);

            if (digit < 0)
            {
                throw JsonSyntax(engine, "Bad Unicode escape in JSON", at + step);
            }

            unit = (unit * 16) + digit;
        }

        at += 4;
        return (char)unit;
    }

    /// <summary>The value of one hexadecimal digit, or a negative number when it is not one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E6AE57
    // Broiler-Human:        PENDING
    private static int JsonHexDigit(char character) => character switch
    {
        >= '0' and <= '9' => character - '0',
        >= 'a' and <= 'f' => character - 'a' + 10,
        >= 'A' and <= 'F' => character - 'A' + 10,
        _ => -1,
    };

    /// <summary>Reads a number literal in the JSON grammar, which is narrower than the language's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7D68C4
    // Broiler-Human:        PENDING
    private static JsValue JsonParseNumber(JsEngine engine, string text, ref int at)
    {
        var start = at;

        if (at < text.Length && text[at] == '-')
        {
            at++;
        }

        if (at >= text.Length || text[at] is < '0' or > '9')
        {
            throw JsonUnexpected(engine, text, at);
        }

        // A leading zero admits no further digits: "01" is two tokens in JSON and therefore an
        // error, where the language would read it as an octal-looking decimal.
        if (text[at] == '0')
        {
            at++;
        }
        else
        {
            while (at < text.Length && text[at] is >= '0' and <= '9')
            {
                at++;
            }
        }

        if (at < text.Length && text[at] == '.')
        {
            at++;

            if (at >= text.Length || text[at] is < '0' or > '9')
            {
                throw JsonUnexpected(engine, text, at);
            }

            while (at < text.Length && text[at] is >= '0' and <= '9')
            {
                at++;
            }
        }

        if (at < text.Length && text[at] is 'e' or 'E')
        {
            at++;

            if (at < text.Length && text[at] is '+' or '-')
            {
                at++;
            }

            if (at >= text.Length || text[at] is < '0' or > '9')
            {
                throw JsonUnexpected(engine, text, at);
            }

            while (at < text.Length && text[at] is >= '0' and <= '9')
            {
                at++;
            }
        }

        engine.Charge((ulong)(at - start));
        return JsValue.Number(JsNumberFormat.ToNumber(text[start..at]));
    }

    /// <summary>Reads one of the three bare words.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=73F871
    // Broiler-Human:        PENDING
    private static JsValue JsonParseKeyword(
        JsEngine engine, string text, ref int at, string word, JsValue value)
    {
        if (at + word.Length > text.Length ||
            string.CompareOrdinal(text, at, word, 0, word.Length) != 0)
        {
            throw JsonUnexpected(engine, text, at);
        }

        at += word.Length;
        return value;
    }

    /// <summary>A SyntaxError naming where in the text the reader stopped.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DAE11A
    // Broiler-Human:        PENDING
    private static JsThrow JsonSyntax(JsEngine engine, string message, int at) =>
        engine.Error(
            "SyntaxError",
            message + " at position " + at.ToString(System.Globalization.CultureInfo.InvariantCulture));

    /// <summary>The error for a character the grammar has no place for.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=525CBA
    // Broiler-Human:        PENDING
    private static JsThrow JsonUnexpected(JsEngine engine, string text, int at) =>
        at >= text.Length
            ? JsonSyntax(engine, "Unexpected end of JSON input", at)
            : JsonSyntax(engine, "Unexpected token '" + text[at] + "' in JSON", at);

    /// <summary>The specification's <c>InternalizeJSONProperty</c>: bottom-up, holder-relative.</summary>
    /// <remarks>
    /// Children are revived before the parent is, so a reviver that rewrites a subtree sees the
    /// rewritten children rather than the raw ones. A reviver that returns <c>undefined</c> deletes
    /// the property, which is the only way it can remove one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B2A289
    // Broiler-Human:        PENDING
    private static JsValue JsonInternalize(
        JsEngine engine, JsObject holder, string key, JsValue reviver, int depth)
    {
        engine.Charge(1);

        if (depth > JsonMaximumDepth)
        {
            throw engine.Error("RangeError", "Maximum JSON nesting depth exceeded");
        }

        var value = engine.GetProperty(JsValue.Object(holder), key);

        if (value.IsObject)
        {
            var target = value.AsObject();

            // `IsArray` HERE TOO. A reviver may return a Proxy over an Array, and walking it as an
            // ordinary object would visit its own keys rather than its indices.
            if (ArrayIsArray(engine, target))
            {
                var length = ArrayLengthOf(engine, value);

                for (double at = 0; at < length; at++)
                {
                    engine.Charge(1);
                    JsonReviveInto(engine, target, ArrayKeyOf(at), reviver, depth);
                }
            }
            else
            {
                // The key list is a snapshot taken before the walk, exactly as
                // EnumerableOwnPropertyNames is: a reviver that adds keys does not extend the walk.
                foreach (var name in JsonOwnEnumerableKeys(engine, target))
                {
                    JsonReviveInto(engine, target, name, reviver, depth);
                }
            }
        }

        return engine.Call(reviver, JsValue.Object(holder), [JsValue.String(key), value]);
    }

    /// <summary>Revives one member in place, deleting it when the reviver returns <c>undefined</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=41BF0A
    // Broiler-Human:        PENDING
    private static void JsonReviveInto(
        JsEngine engine, JsObject target, string name, JsValue reviver, int depth)
    {
        var revived = JsonInternalize(engine, target, name, reviver, depth + 1);

        if (revived.Type == JsType.Undefined)
        {
            target.DeleteOwnProperty(name);
            return;
        }

        target.SetOwnProperty(name, JsProperty.Data(revived, JsPropertyAttributes.Default));
    }

    // ---- writing -----------------------------------------------------------------------------

    /// <summary><c>JSON.stringify</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=937510
    // Broiler-Human:        PENDING
    private JsValue JsonStringifyEntry(JsEngine engine, JsValue[] arguments)
    {
        var state = new JsonWriter();
        JsonApplyReplacer(engine, state, JsonArgument(arguments, 1));
        JsonApplySpace(engine, state, JsonArgument(arguments, 2));

        var holder = new JsObject(ObjectPrototype);
        holder.DefineOrdinary(string.Empty, JsonArgument(arguments, 0));
        var text = JsonSerialize(engine, state, string.Empty, holder);

        // `JSON.stringify(undefined)` is `undefined` and not the string "undefined". Returning the
        // string is the defect this branch exists to not have.
        if (text is null)
        {
            return JsValue.Undefined;
        }

        engine.Charge((ulong)text.Length);
        // Transient, like every other string this profile builds; see the note in
        // JsRealm.Function.cs. Fuel is what bounds it.
        return JsValue.String(text);
    }

    /// <summary>Reads the replacer argument into either a function or a key allow-list.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=16A326
    // Broiler-Human:        PENDING
    private static void JsonApplyReplacer(JsEngine engine, JsonWriter state, JsValue replacer)
    {
        if (!replacer.IsObject)
        {
            return;
        }

        var target = replacer.AsObject();

        if (target.IsCallable)
        {
            state.ReplacerFunction = replacer;
            return;
        }

        if (target is not JsArray list)
        {
            return;
        }

        var names = new System.Collections.Generic.List<string>();
        var seen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);
        var length = list.Length;

        for (uint at = 0; at < length; at++)
        {
            engine.Charge(1);
            var element = engine.GetIndexed(replacer, JsValue.Number(at));
            string? name = null;

            if (element.IsString)
            {
                name = element.AsString();
            }
            else if (element.IsNumber)
            {
                name = JsNumberFormat.ToJsString(element.AsNumber());
            }
            else if (element.IsObject && element.AsObject() is JsPrimitiveWrapper wrapper &&
                     (wrapper.Primitive.IsString || wrapper.Primitive.IsNumber))
            {
                name = engine.ToStringValue(element);
            }

            if (name is not null && seen.Add(name))
            {
                names.Add(name);
            }
        }

        state.PropertyList = names;
    }

    /// <summary>Reads the space argument into one level of indentation.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=ECC1E3
    // Broiler-Human:        PENDING
    private static void JsonApplySpace(JsEngine engine, JsonWriter state, JsValue space)
    {
        if (space.IsObject && space.AsObject() is JsPrimitiveWrapper wrapper)
        {
            if (wrapper.Primitive.IsNumber)
            {
                space = JsValue.Number(engine.ToNumber(space));
            }
            else if (wrapper.Primitive.IsString)
            {
                space = JsValue.String(engine.ToStringValue(space));
            }
        }

        if (space.IsNumber)
        {
            var count = (int)System.Math.Min(
                10, System.Math.Max(0, JsValue.ToInteger(space.AsNumber())));

            state.Gap = count > 0 ? new string(' ', count) : string.Empty;
            return;
        }

        if (space.IsString)
        {
            var text = space.AsString();
            state.Gap = text.Length <= 10 ? text : text[..10];
        }
    }

    /// <summary>
    /// The specification's <c>SerializeJSONProperty</c>, returning <see langword="null"/> for the
    /// values that have no JSON text at all.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A42B0C
    // Broiler-Human:        PENDING
    private static string? JsonSerialize(
        JsEngine engine, JsonWriter state, string key, JsObject holder)
    {
        var value = engine.GetProperty(JsValue.Object(holder), key);

        if (value.IsObject)
        {
            var toJson = engine.GetProperty(value, "toJSON");

            if (toJson.IsObject && toJson.AsObject().IsCallable)
            {
                value = engine.Call(toJson, value, [JsValue.String(key)]);
            }
        }

        if (state.ReplacerFunction.IsObject)
        {
            value = engine.Call(
                state.ReplacerFunction, JsValue.Object(holder), [JsValue.String(key), value]);
        }

        if (value.IsObject && value.AsObject() is JsPrimitiveWrapper wrapper)
        {
            if (wrapper.Primitive.IsNumber)
            {
                value = JsValue.Number(engine.ToNumber(value));
            }
            else if (wrapper.Primitive.IsString)
            {
                value = JsValue.String(engine.ToStringValue(value));
            }
            else if (wrapper.Primitive.Type == JsType.Boolean)
            {
                value = wrapper.Primitive;
            }
        }

        switch (value.Type)
        {
            case JsType.Null:
                return "null";

            case JsType.Boolean:
                return value.AsBoolean() ? "true" : "false";

            case JsType.String:
                return JsonQuote(engine, value.AsString());

            case JsType.Number:
            {
                var number = value.AsNumber();
                return double.IsFinite(number) ? JsNumberFormat.ToJsString(number) : "null";
            }

            default:
                break;
        }

        if (!value.IsObject)
        {
            return null;
        }

        var target = value.AsObject();

        if (target.IsCallable)
        {
            return null;
        }

        // `IsArray` AND NOT A TYPE TEST, because a Proxy over an Array must serialise as an Array.
        // A type test here wrote `{"0":1,"1":2}` for a proxied `[1, 2]` - valid JSON of the wrong
        // shape, which is the worst kind of wrong answer a serialiser can give.
        return ArrayIsArray(engine, target)
            ? JsonSerializeArray(engine, state, target)
            : JsonSerializeObject(engine, state, target);
    }

    /// <summary>The specification's <c>SerializeJSONObject</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5B77B9
    // Broiler-Human:        PENDING
    private static string JsonSerializeObject(JsEngine engine, JsonWriter state, JsObject value)
    {
        JsonEnter(engine, state, value);
        var stepback = state.Indent;
        state.Indent += state.Gap;

        var keys = state.PropertyList ?? JsonOwnEnumerableKeys(engine, value);
        var parts = new System.Collections.Generic.List<string>();
        var separator = state.Gap.Length == 0 ? ":" : ": ";

        foreach (var key in keys)
        {
            engine.Charge(1);
            var text = JsonSerialize(engine, state, key, value);

            if (text is not null)
            {
                parts.Add(JsonQuote(engine, key) + separator + text);
            }
        }

        var rendered = JsonJoin(state, stepback, parts, "{", "}");
        state.Indent = stepback;
        JsonLeave(state);
        return rendered;
    }

    /// <summary>The specification's <c>SerializeJSONArray</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7CEE41
    // Broiler-Human:        PENDING
    private static string JsonSerializeArray(JsEngine engine, JsonWriter state, JsObject value)
    {
        JsonEnter(engine, state, value);
        var stepback = state.Indent;
        state.Indent += state.Gap;

        var parts = new System.Collections.Generic.List<string>();

        // THE LENGTH IS READ AS A PROPERTY AND NOT OFF THE STORE, which is `LengthOfArrayLike` and
        // is what the specification says. It matters for a Proxy, whose `length` is whatever its
        // `get` trap answers and which has no store of its own to ask.
        var length = ArrayLengthOf(engine, JsValue.Object(value));

        for (double at = 0; at < length; at++)
        {
            engine.Charge(1);

            // In an Array, a value with no JSON text is `null` rather than a gap: an Array's shape
            // is its indices, and omitting one would renumber every element after it.
            parts.Add(JsonSerialize(engine, state, ArrayKeyOf(at), value) ?? "null");
        }

        var rendered = JsonJoin(state, stepback, parts, "[", "]");
        state.Indent = stepback;
        JsonLeave(state);
        return rendered;
    }

    /// <summary>Pushes one object onto the cycle stack, refusing a repeat.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F36AF0
    // Broiler-Human:        PENDING
    private static void JsonEnter(JsEngine engine, JsonWriter state, JsObject value)
    {
        foreach (var seen in state.Stack)
        {
            if (ReferenceEquals(seen, value))
            {
                throw engine.Error("TypeError", "Converting circular structure to JSON");
            }
        }

        if (state.Stack.Count >= JsonMaximumDepth)
        {
            throw engine.Error("RangeError", "Maximum JSON nesting depth exceeded");
        }

        state.Stack.Add(value);
    }

    /// <summary>Pops the innermost object off the cycle stack.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=727241
    // Broiler-Human:        PENDING
    private static void JsonLeave(JsonWriter state) =>
        state.Stack.RemoveAt(state.Stack.Count - 1);

    /// <summary>Joins the serialised members, indented or not.</summary>
    /// <remarks>
    /// An empty object or Array is <c>{}</c> and <c>[]</c> whatever the gap is: the specification
    /// puts the newline and the indentation between members, and there are none.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E82C2A
    // Broiler-Human:        PENDING
    private static string JsonJoin(
        JsonWriter state,
        string stepback,
        System.Collections.Generic.List<string> parts,
        string open,
        string close)
    {
        if (parts.Count == 0)
        {
            return open + close;
        }

        var text = new System.Text.StringBuilder();
        text.Append(open);

        if (state.Gap.Length == 0)
        {
            for (var at = 0; at < parts.Count; at++)
            {
                if (at > 0)
                {
                    text.Append(',');
                }

                text.Append(parts[at]);
            }
        }
        else
        {
            text.Append('\n');

            for (var at = 0; at < parts.Count; at++)
            {
                if (at > 0)
                {
                    text.Append(",\n");
                }

                text.Append(state.Indent).Append(parts[at]);
            }

            text.Append('\n').Append(stepback);
        }

        text.Append(close);
        return text.ToString();
    }

    /// <summary>Every own enumerable string key of <paramref name="value"/>, in specification order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=439337
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<string> JsonOwnEnumerableKeys(
        JsEngine engine, JsObject value)
    {
        var keys = new System.Collections.Generic.List<string>();

        foreach (var key in value.OwnPropertyNames())
        {
            engine.Charge(1);

            if (value.TryGetOwnProperty(key, out var property) && property.Enumerable)
            {
                keys.Add(key);
            }
        }

        return keys;
    }

    /// <summary>The specification's <c>QuoteJSONString</c>.</summary>
    /// <remarks>
    /// Every code unit below <c>0x20</c> that has no short escape, and every surrogate without its
    /// partner, becomes a <c>\u</c> escape. The lone-surrogate case is what makes the output
    /// well-formed UTF-16 text that any reader can take back: a bare <c>\ud800</c> in the output
    /// cannot be encoded as UTF-8 and would make the document unreadable to a consumer that decodes
    /// before it parses.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=C400F0
    // Broiler-Human:        PENDING
    private static string JsonQuote(JsEngine engine, string value)
    {
        engine.Charge((ulong)value.Length + 1);
        var text = new System.Text.StringBuilder(value.Length + 2);
        text.Append('"');

        for (var at = 0; at < value.Length; at++)
        {
            var character = value[at];

            switch (character)
            {
                case '\\':
                    text.Append("\\\\");
                    break;

                case '"':
                    text.Append("\\\"");
                    break;

                case '\b':
                    text.Append("\\b");
                    break;

                case '\f':
                    text.Append("\\f");
                    break;

                case '\n':
                    text.Append("\\n");
                    break;

                case '\r':
                    text.Append("\\r");
                    break;

                case '\t':
                    text.Append("\\t");
                    break;

                default:
                    if (character < ' ' || JsonIsLoneSurrogate(value, at))
                    {
                        text.Append("\\u").Append(
                            ((int)character).ToString(
                                "x4", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        text.Append(character);
                    }

                    break;
            }
        }

        text.Append('"');
        return text.ToString();
    }

    /// <summary>Whether the code unit at <paramref name="at"/> is a surrogate without its partner.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7E0A15
    // Broiler-Human:        PENDING
    private static bool JsonIsLoneSurrogate(string value, int at)
    {
        var character = value[at];

        if (char.IsHighSurrogate(character))
        {
            return at + 1 >= value.Length || !char.IsLowSurrogate(value[at + 1]);
        }

        if (char.IsLowSurrogate(character))
        {
            return at == 0 || !char.IsHighSurrogate(value[at - 1]);
        }

        return false;
    }
}
