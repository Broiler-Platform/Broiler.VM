// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   30
// Annotated:        30/30
// Exempt:           10
// Human-reviewed:   0/30
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       30
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Generic;
using System.Globalization;
using Broiler.ECMAScript;
using Broiler.ECMAScript.Text;

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
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7AFE22
        // Broiler-Human:        PENDING
        internal List<string>? PropertyList { get; set; }

        /// <summary>One level of indentation; empty when the output is not indented.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=ED61CE
        // Broiler-Human:        PENDING
        internal string Gap { get; set; } = string.Empty;

        /// <summary>The indentation in force at the current nesting level.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=10C1D6
        // Broiler-Human:        PENDING
        internal string Indent { get; set; } = string.Empty;

        /// <summary>The objects currently being serialised, innermost last.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A5AB08
        // Broiler-Human:        PENDING
        internal List<JsObject> Stack { get; } = [];
    }

    /// <summary>Builds <c>JSON</c> and defines it on the global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B75A7D
    // Broiler-Human:        PENDING
    private void SetupJson()
    {
        var json = new JsObject(ObjectPrototype, "JSON");

        Method(json, "parse", 2, (engine, thisValue, arguments) => JsonParseEntry(engine, arguments));
        Method(json, "stringify", 3, (engine, thisValue, arguments) => JsonStringifyEntry(engine, arguments));
        Method(json, "rawJSON", 1, (engine, thisValue, arguments) => JsonRawJsonEntry(engine, arguments));
        Method(json, "isRawJSON", 1, static (engine, thisValue, arguments) =>
            JsValue.Boolean(JsonArgument(arguments, 0) is { IsObject: true } value && value.AsObject() is RawJsonObject));

        // `JSON[Symbol.toStringTag]` is "JSON", configurable and nothing else, as it is for
        // `Math` and `Reflect`.
        json.SetOwnSymbol(ToStringTagSymbol, JsProperty.Data(JsValue.String("JSON"), JsPropertyAttributes.Configurable));

        GlobalObject.DefineBuiltIn("JSON", JsValue.Object(json));
    }

    /// <summary>
    /// A raw JSON value: an ordinary object whose type is the specification's <c>[[IsRawJSON]]</c>
    /// internal slot.
    /// </summary>
    /// <remarks>
    /// <b>The brand is the CLR type and not a property</b>, because a property is something guest
    /// code can write: <c>{ rawJSON: "1" }</c>, a frozen null-prototype copy of one, an object
    /// inheriting from a real one, and a Proxy over a real one all read the same as the genuine
    /// article and none of them is it. Only <c>JSON.rawJSON</c> constructs this type, so only its
    /// validation stands between guest text and the verbatim output of <c>JSON.stringify</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=55D87A
    // Broiler-Human:        PENDING
    private sealed class RawJsonObject : JsObject
    {
        /// <summary>Creates the frozen, null-prototype object carrying <paramref name="text"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=574EEC
        // Broiler-Human:        PENDING
        internal RawJsonObject(string text) : base(null)
        {
            Text = text;
            SetOwnProperty("rawJSON", JsProperty.Data(JsValue.String(text), JsPropertyAttributes.Enumerable));
            Extensible = false;
        }

        /// <summary>
        /// The validated text, which is also the value of the object's own, non-writable,
        /// non-configurable <c>rawJSON</c> property and therefore cannot drift from it.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=37F0D2
        // Broiler-Human:        PENDING
        internal string Text { get; }
    }

    /// <summary><c>JSON.rawJSON</c>.</summary>
    /// <remarks>
    /// The first and last code-unit checks come before the parse and are what confine the result to a
    /// primitive: no permitted first character opens an object or an Array, and none of JSON's four
    /// whitespace characters may stand at either end, so the text is exactly one value with nothing
    /// around it. The parse itself is the ordinary reader, so a raw number or string is held to the
    /// same grammar <c>JSON.parse</c> is.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=12F642
    // Broiler-Human:        PENDING
    private JsValue JsonRawJsonEntry(JsEngine engine, JsValue[] arguments)
    {
        engine.Charge(1);
        var text = engine.ToStringValue(JsonArgument(arguments, 0));

        if (text.Length == 0)
        {
            throw JsonSyntax(engine, "Unexpected end of raw JSON text", 0);
        }

        if (!JsonRawMayStart(text[0]))
        {
            throw JsonSyntax(engine, "Raw JSON text may not start with '" + text[0] + "'", 0);
        }

        if (!JsonRawMayEnd(text[^1]))
        {
            throw JsonSyntax(engine, "Raw JSON text may not end with '" + text[^1] + "'", text.Length - 1);
        }

        // The value read is discarded: what the object carries is the text, and the reader is here
        // only to refuse text that is not one JSON primitive. Nothing it can reach creates an object,
        // because no character allowed first is `{` or `[`. What follows the value is refused where
        // the value ends, before any white space after it is skipped and charged.
        var reader = new JsonReader(text, new DepthLimit(JsonMaximumDepth), engine.WorkCharge);
        JsonRead(engine, ref reader, text);

        if (reader.TokenEnd != text.Length)
        {
            throw JsonUnexpected(engine, text, reader.TokenEnd);
        }

        return JsValue.Object(new RawJsonObject(text));
    }

    /// <summary>Whether raw JSON text may start with <paramref name="character"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BDC408
    // Broiler-Human:        PENDING
    private static bool JsonRawMayStart(char character) =>
        character is (>= 'a' and <= 'z') or (>= '0' and <= '9') or '"' or '-';

    /// <summary>Whether raw JSON text may end with <paramref name="character"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=632CCC
    // Broiler-Human:        PENDING
    private static bool JsonRawMayEnd(char character) =>
        character is (>= 'a' and <= 'z') or (>= '0' and <= '9') or '"';

    /// <summary>Reads one argument, which may not have been supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=307331
    // Broiler-Human:        PENDING
    private static JsValue JsonArgument(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    // ---- reading -----------------------------------------------------------------------------

    /// <summary><c>JSON.parse</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F560AE
    // Broiler-Human:        PENDING
    private JsValue JsonParseEntry(JsEngine engine, JsValue[] arguments)
    {
        var text = engine.ToStringValue(JsonArgument(arguments, 0));

        // THE PARSE IS RECORDED ONLY WHEN THERE IS A REVIVER TO HAND IT TO, which is the JSON Parse
        // Record the specification builds for one: the source text of every primitive value and the
        // record of every element and member, so a reviver is told what was written as well as what
        // it was read as (JSP-7, JSC-239). Reading the reviver first is unobservable: it is an
        // argument, and only its callability is asked.
        var reviver = JsonArgument(arguments, 1);
        var recording = reviver.IsObject && reviver.AsObject().IsCallable;

        // The shared reader checks the grammar and charges as this reader did (JSD-0071); the values,
        // the records and the errors are built here.
        var reader = new JsonReader(text, new DepthLimit(JsonMaximumDepth), engine.WorkCharge);
        JsonRead(engine, ref reader, text);
        var parsed = JsonBuild(engine, ref reader, text, recording, out var snapshot);

        // The end of the text, after nothing but white space: anything else is refused where it starts.
        JsonRead(engine, ref reader, text);

        if (!recording)
        {
            return parsed;
        }

        // The specification hands the reviver a synthetic holder whose one key is the empty string,
        // so the root value is revived by exactly the same rule as every value inside it.
        var root = new JsObject(ObjectPrototype);
        root.DefineOrdinary(string.Empty, parsed);
        return JsonInternalize(engine, root, string.Empty, reviver, 0, snapshot);
    }

    /// <summary>Reads the next token, or throws the error the reader stopped at.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=8BA23F
    // Broiler-Human:        PENDING
    private static void JsonRead(JsEngine engine, ref JsonReader reader, string text)
    {
        if (reader.Read() == ResultStatus.Completed)
        {
            return;
        }

        var failure = reader.Failure;

        throw failure.MessageId switch
        {
            // Both directions are recursive over a structure the guest chose the depth of; the
            // ceiling is a RangeError the host can act on rather than an exhausted CLR stack.
            "json.too-deep" => engine.Error("RangeError", "Maximum JSON nesting depth exceeded"),
            "json.unterminated-string" => JsonSyntax(engine, "Unterminated string in JSON", failure.Position),

            // A raw control character inside a string is the single most common way a hand-built
            // encoder produces something no conforming reader will take back.
            "json.control-character" => JsonSyntax(engine, "Bad control character in JSON", failure.Position),
            "json.bad-escape" => JsonSyntax(engine, "Bad escaped character in JSON", failure.Position),
            "json.bad-unicode-escape" => JsonSyntax(engine, "Bad Unicode escape in JSON", failure.Position),
            _ => JsonUnexpected(engine, text, failure.Position),
        };
    }

    /// <summary>Builds the value whose first token the reader has just read.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=C5B067
    // Broiler-Human:        PENDING
    private JsValue JsonBuild(JsEngine engine, ref JsonReader reader, string text, bool recording, out JsonRecord? record)
    {
        switch (reader.Token)
        {
            case JsonToken.BeginObject:
                return JsonBuildObject(engine, ref reader, text, recording, out record);

            case JsonToken.BeginArray:
                return JsonBuildArray(engine, ref reader, text, recording, out record);
        }

        // A string's escapes are resolved code unit by code unit, so `"\ud800"` is a well-formed JSON
        // string holding a lone surrogate: a String in this language is a sequence of code units
        // rather than of scalars, and rejecting it would be inventing a rule.
        var value = reader.Token switch
        {
            JsonToken.String => JsValue.String(reader.StringValue),
            JsonToken.Number => JsValue.Number(reader.NumberValue),
            JsonToken.True => JsValue.True,
            JsonToken.False => JsValue.False,
            _ => JsValue.Null,
        };

        // A PRIMITIVE'S RECORD IS ITS TEXT, exactly as written - the escapes of a string, the
        // exponent of a number, the sign of `-0` - with nothing around it. A container records its
        // members while it is read.
        record = recording
            ? new JsonRecord(value) { Source = text[reader.TokenStart..reader.TokenEnd] }
            : null;

        return value;
    }

    /// <summary>Builds an ordinary object from the members the reader reads.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=56CD6C
    // Broiler-Human:        PENDING
    private JsValue JsonBuildObject(JsEngine engine, ref JsonReader reader, string text, bool recording, out JsonRecord? record)
    {
        var target = new JsObject(ObjectPrototype);
        record = recording ? new JsonRecord(JsValue.Object(target)) { Entries = [] } : null;

        while (true)
        {
            JsonRead(engine, ref reader, text);

            if (reader.Token == JsonToken.EndObject)
            {
                return JsValue.Object(target);
            }

            var key = reader.StringValue;
            JsonRead(engine, ref reader, text);
            var value = JsonBuild(engine, ref reader, text, recording, out var member);

            // An unchecked define, which is what CreateDataProperty is here: a key of "__proto__"
            // becomes an own data property rather than reaching Object.prototype's setter, and a
            // repeated key replaces the earlier one in the earlier one's position. Its record is the
            // last one too, which is the member the specification's record names.
            target.DefineOrdinary(key, value);

            if (record?.Entries is { } entries)
            {
                entries[key] = member!;
            }
        }
    }

    /// <summary>Builds an Array from the elements the reader reads.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=89103F
    // Broiler-Human:        PENDING
    private JsValue JsonBuildArray(JsEngine engine, ref JsonReader reader, string text, bool recording, out JsonRecord? record)
    {
        var target = NewArray();
        record = recording ? new JsonRecord(JsValue.Object(target)) { Elements = [] } : null;

        while (true)
        {
            JsonRead(engine, ref reader, text);

            if (reader.Token == JsonToken.EndArray)
            {
                return JsValue.Object(target);
            }

            target.Push(JsonBuild(engine, ref reader, text, recording, out var element));
            record?.Elements!.Add(element!);
        }
    }

    /// <summary>A SyntaxError naming where in the text the reader stopped.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A3DF51
    // Broiler-Human:        PENDING
    private static JsThrow JsonSyntax(JsEngine engine, string message, int at) =>
        engine.Error("SyntaxError", message + " at position " + at.ToString(CultureInfo.InvariantCulture));

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A6E0FA
    // Broiler-Human:        PENDING
    private static JsValue JsonInternalize(JsEngine engine, JsObject holder, string key, JsValue reviver, int depth, JsonRecord? record)
    {
        engine.Charge(1);

        if (depth > JsonMaximumDepth)
        {
            throw engine.Error("RangeError", "Maximum JSON nesting depth exceeded");
        }

        var value = engine.GetProperty(JsValue.Object(holder), key);

        // THE THIRD ARGUMENT: a context object, holding the text a primitive was read from while the
        // value is still the one the parse produced (JSP-7, JSC-239). A value a reviver already
        // replaced has no source, and neither have its members.
        var context = new JsObject(engine.Realm.ObjectPrototype);

        if (record is not null && ObjectSameValue(record.Value, value))
        {
            if (!value.IsObject)
            {
                context.DefineOrdinary("source", JsValue.String(record.Source!));
            }
        }
        else
        {
            record = null;
        }

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

                    var element = record?.Elements is { } elements && at < elements.Count
                        ? elements[(int)at]
                        : null;

                    JsonReviveInto(engine, target, ArrayKeyOf(at), reviver, depth, element);
                }
            }
            else
            {
                // The key list is a snapshot taken before the walk, exactly as
                // EnumerableOwnPropertyNames is: a reviver that adds keys does not extend the walk.
                foreach (var name in JsonOwnEnumerableKeys(engine, target))
                {
                    var member = record?.Entries is { } entries && entries.TryGetValue(name, out var found)
                        ? found
                        : null;

                    JsonReviveInto(engine, target, name, reviver, depth, member);
                }
            }
        }

        return engine.Call(reviver, JsValue.Object(holder), [JsValue.String(key), value, JsValue.Object(context)]);
    }

    /// <summary>Revives one member in place, deleting it when the reviver returns <c>undefined</c>.</summary>
    /// <remarks>
    /// <b>The revived value is stored by <c>CreateDataProperty</c>, the target's own
    /// <c>[[DefineOwnProperty]]</c>, and a refusal is ignored.</b> The reviver can hand back any
    /// object as the holder of the next level, so the target may be a typed array - whose element
    /// write converts with the engine, ToBigInt for a BigInt kind, and throws a catchable
    /// <c>TypeError</c> for a Number - or a frozen object, which refuses. Storing through
    /// <see cref="JsObject.SetOwnProperty"/>, as this did, converted without the engine (ending the
    /// invocation for a Number bound for a BigInt element) and overwrote a non-writable property.
    /// (JSeal B07 review fix, 2026-09-22.) The removal is the holder's own <c>[[Delete]]</c>:
    /// <see cref="JsObject.DeleteOwnProperty"/> is virtual, so a Proxy holder's <c>deleteProperty</c>
    /// trap runs and its throw propagates, and a <c>false</c> answer is ignored, as
    /// <c>InternalizeJSONProperty</c> says (checked for JSeal VM-FIX-I).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D0AE81
    // Broiler-Human:        PENDING
    private static void JsonReviveInto(JsEngine engine, JsObject target, string name, JsValue reviver, int depth, JsonRecord? record)
    {
        var revived = JsonInternalize(engine, target, name, reviver, depth + 1, record);

        if (revived.Type == JsType.Undefined)
        {
            target.DeleteOwnProperty(name);
            return;
        }

        var fields = new ObjectDescriptorFields
        {
            HasValue = true,
            Value = revived,
            HasWritable = true,
            Writable = true,
            HasEnumerable = true,
            Enumerable = true,
            HasConfigurable = true,
            Configurable = true,
        };

        _ = ObjectDefineOwn(engine, target, JsValue.String(name), fields);
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9F005F
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

        // `IsArray` AND `LengthOfArrayLike`, not a type test and the stored length: a Proxy over an
        // Array is an allow-list too, and its `length` is read through its `get` trap like every
        // element after it.
        if (!ArrayIsArray(engine, target))
        {
            return;
        }

        var names = new List<string>();
        var seen = new HashSet<string>(System.StringComparer.Ordinal);
        var length = ArrayLengthOf(engine, replacer);

        for (double at = 0; at < length; at++)
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
                name = JsNumberFormat.NumberToString(element.AsNumber());
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=036E0F
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
            state.Gap = JsonText.GapFromNumber(space.AsNumber());
            return;
        }

        if (space.IsString)
        {
            state.Gap = JsonText.GapFromString(space.AsString());
        }
    }

    /// <summary>
    /// The specification's <c>SerializeJSONProperty</c>, returning <see langword="null"/> for the
    /// values that have no JSON text at all.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=407032
    // Broiler-Human:        PENDING
    private static string? JsonSerialize(JsEngine engine, JsonWriter state, string key, JsObject holder)
    {
        var value = engine.GetProperty(JsValue.Object(holder), key);

        // A BIGINT IS ASKED FOR `toJSON` AS AN OBJECT IS (JSeal B05), through `BigInt.prototype`, so a
        // program that installs `BigInt.prototype.toJSON` chooses how its BigInts serialise; one
        // that does not meets the TypeError below.
        if (value.IsObject || value.IsBigInt)
        {
            var toJson = engine.GetProperty(value, "toJSON");

            if (toJson.IsObject && toJson.AsObject().IsCallable)
            {
                value = engine.Call(toJson, value, [JsValue.String(key)]);
            }
        }

        if (state.ReplacerFunction.IsObject)
        {
            value = engine.Call(state.ReplacerFunction, JsValue.Object(holder), [JsValue.String(key), value]);
        }

        // A RAW VALUE IS EMITTED ONLY AFTER toJSON AND THE REPLACER HAVE HAD THEIR TURN, which is the
        // specification's order: a replacer sees the branded object and may substitute it, and a
        // toJSON may produce one. The check is on the type, so a Proxy over a raw value is an
        // ordinary object here and serialises its `rawJSON` member as a string.
        if (value.IsObject && value.AsObject() is RawJsonObject raw)
        {
            return raw.Text;
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
            else if (wrapper.Primitive.Type is JsType.Boolean or JsType.BigInt)
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
                return JsonText.Quote(value.AsString(), engine.WorkCharge);

            case JsType.Number:
                {
                    var number = value.AsNumber();
                    return double.IsFinite(number) ? JsNumberFormat.NumberToString(number) : "null";
                }

            // THE LANGUAGE THROWS A TypeError FOR A BIGINT HERE, after consulting `toJSON` and the
            // replacer and unwrapping a BigInt object. Falling through would answer "not
            // serialisable" and OMIT the value, a plausible wrong answer.
            case JsType.BigInt:
                throw engine.Error("TypeError", "Do not know how to serialize a BigInt");

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BBCDB6
    // Broiler-Human:        PENDING
    private static string JsonSerializeObject(JsEngine engine, JsonWriter state, JsObject value)
    {
        JsonEnter(engine, state, value);
        var stepback = state.Indent;
        state.Indent += state.Gap;

        var keys = state.PropertyList ?? JsonOwnEnumerableKeys(engine, value);
        var parts = new List<string>();
        var separator = state.Gap.Length == 0 ? ":" : ": ";

        foreach (var key in keys)
        {
            engine.Charge(1);
            var text = JsonSerialize(engine, state, key, value);

            if (text is not null)
            {
                parts.Add(JsonText.Quote(key, engine.WorkCharge) + separator + text);
            }
        }

        var rendered = JsonJoin(state, stepback, parts, "{", "}");
        state.Indent = stepback;
        JsonLeave(state);
        return rendered;
    }

    /// <summary>The specification's <c>SerializeJSONArray</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5A7C99
    // Broiler-Human:        PENDING
    private static string JsonSerializeArray(JsEngine engine, JsonWriter state, JsObject value)
    {
        JsonEnter(engine, state, value);
        var stepback = state.Indent;
        state.Indent += state.Gap;

        var parts = new List<string>();

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
    private static void JsonLeave(JsonWriter state) => state.Stack.RemoveAt(state.Stack.Count - 1);

    /// <summary>Joins the serialised members, indented or not.</summary>
    /// <remarks>
    /// An empty object or Array is <c>{}</c> and <c>[]</c> whatever the gap is: the specification
    /// puts the newline and the indentation between members, and there are none.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=403797
    // Broiler-Human:        PENDING
    private static string JsonJoin(JsonWriter state, string stepback, List<string> parts, string open, string close)
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2CC330
    // Broiler-Human:        PENDING
    private static List<string> JsonOwnEnumerableKeys(JsEngine engine, JsObject value)
    {
        var keys = new List<string>();

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

    /// <summary>
    /// The specification's JSON Parse Record: a value as the parse produced it, the text a primitive
    /// was read from, and the records of an Array's elements or an object's members.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=156748
    // Broiler-Human:        PENDING
    private sealed class JsonRecord(JsValue value)
    {
        /// <summary>The value the parse produced, which a reviver may since have replaced.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E50CDA
        // Broiler-Human:        PENDING
        internal JsValue Value { get; } = value;

        /// <summary>The text a primitive was read from, exactly as written.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=48F450
        // Broiler-Human:        PENDING
        internal string? Source { get; init; }

        /// <summary>An Array's elements' records, in order.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E4D0C9
        // Broiler-Human:        PENDING
        internal List<JsonRecord>? Elements { get; init; }

        /// <summary>An object's members' records, by key, the last of a repeated key kept.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1B41A6
        // Broiler-Human:        PENDING
        internal Dictionary<string, JsonRecord>? Entries { get; init; }
    }
}
