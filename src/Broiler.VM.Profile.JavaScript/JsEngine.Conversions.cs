// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   21
// Annotated:        21/21
// Exempt:           0
// Human-reviewed:   0/21
// IP risk:          Low
// Security risk:    High
// Criteria:         9/9
// Resource impact:  5/10 max
// Unverified:       21
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    // ---- conversions ---------------------------------------------------------------------------

    /// <summary>The abstract operation <c>ToPrimitive</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B7C265
    // Broiler-Human:        PENDING
    internal JsValue ToPrimitive(JsValue value, string hint)
    {
        if (!value.IsObject)
        {
            return value;
        }

        // `Symbol.toPrimitive` COMES FIRST AND ANSWERS FOR THE WHOLE OPERATION. It is what makes a
        // Symbol refuse to become a String, and what lets a Date distinguish the three hints; a
        // conversion that consulted `valueOf` first would have already produced an answer before the
        // object could say it has none.
        if (TryGetSymbolMethod(value, Realm.ToPrimitiveSymbol, out var exotic))
        {
            var answered = Call(exotic, value, [JsValue.String(hint)]);

            if (!answered.IsObject)
            {
                return answered;
            }

            return ThrowTypeError("Cannot convert object to primitive value");
        }

        return OrdinaryToPrimitive(value, hint);
    }

    /// <summary>
    /// The abstract operation <c>OrdinaryToPrimitive</c>: the two methods, in the hint's order.
    /// </summary>
    /// <remarks>
    /// It is separate from <see cref="ToPrimitive"/> because an exotic
    /// <c>Symbol.toPrimitive</c> can need it: a Date's answers the <c>"default"</c> hint by asking
    /// for the <c>"string"</c> ordering, and an implementation that recursed into
    /// <see cref="ToPrimitive"/> to get it would find its own exotic method again.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=64D261
    // Broiler-Human:        PENDING
    internal JsValue OrdinaryToPrimitive(JsValue value, string hint)
    {
        var order = string.Equals(hint, "string", StringComparison.Ordinal)
            ? new[] { "toString", "valueOf" }
            : ["valueOf", "toString"];

        foreach (var name in order)
        {
            var method = GetProperty(value, name);

            if (method.IsObject && method.AsObject().IsCallable)
            {
                var result = Call(method, value, []);

                if (!result.IsObject)
                {
                    return result;
                }
            }
        }

        return ThrowTypeError("Cannot convert object to primitive value");
    }

    /// <summary>The abstract operation <c>ToNumber</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B3645E
    // Broiler-Human:        PENDING
    internal double ToNumber(JsValue value) => value.Type switch
    {
        JsType.Number => value.AsNumber(),
        JsType.Boolean => value.AsBoolean() ? 1 : 0,
        JsType.Undefined => double.NaN,
        JsType.Null => 0,
        JsType.String => ToNumberFromText(value.AsString()),

        // A SYMBOL HAS TO BE REFUSED HERE AND NOT LEFT TO THE ARM BELOW. `ToPrimitive` of a
        // primitive is that primitive, so a Symbol reaching the recursive arm converts to itself
        // for ever: the process dies of a stack overflow, which is the one failure this profile
        // may never produce. `ToString` already refuses a Symbol by name; this is the same refusal
        // on the other conversion, and the reason is the same - a Symbol is a key nobody can
        // forge, and a key that silently became a number would be forgeable by arithmetic.
        JsType.Symbol => ThrowTypeError("Cannot convert a Symbol value to a number").AsNumber(),

        // A BIGINT IS REFUSED BY `ToNumber` ITSELF, which is the specification's own answer and not
        // a placeholder: `ToNumber(1n)` is a TypeError. The operations that may turn a BigInt into
        // a Number - `Number(x)` alone - do so through `ToNumeric` and `JsBigInt.ToNumber`, and the
        // operators and comparisons that take either type convert with `ToNumeric` (JSeal B03-B05).
        // What stops here is unary `+`, `Math`, `isNaN` and every other operation that asks for a
        // Number, none of which may produce one from a BigInt. Leaving it to the arm below would
        // loop exactly as a Symbol did, because a BigInt is its own primitive.
        JsType.BigInt => ThrowTypeError("Cannot convert a BigInt value to a number").AsNumber(),
        _ => ToNumber(ToPrimitive(value, "number")),
    };

    /// <summary>Reads a number out of text, charging for the text it reads.</summary>
    /// <remarks>
    /// The conversion trims and scans the whole string, so its cost is the string's own length —
    /// which the guest chooses, and which was charged nothing until this existed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=07B791
    // Broiler-Falsified-If: a longer numeric string is read for the same charge as a shorter one
    // Broiler-Human:        PENDING
    private double ToNumberFromText(string text)
    {
        ChargeText(text.Length);
        return JsNumberFormat.ToNumber(text);
    }

    /// <summary>
    /// The abstract operation <c>ToNumeric</c>: a Number or a BigInt, from one conversion.
    /// </summary>
    /// <remarks>
    /// An object is converted to a primitive once, with the <c>"number"</c> hint, and a BigInt it
    /// answers is kept; anything else goes on to <see cref="ToNumber"/>, which is where a Symbol is
    /// refused. A value of either numeric type is returned as it is, so a Number operand costs one
    /// test more than it did.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=5C1B6B
    // Broiler-Falsified-If: an object operand is converted to a primitive twice, or a BigInt reaches ToNumber through this operation
    // Broiler-Human:        PENDING
    internal JsValue ToNumeric(JsValue value)
    {
        if (value.Type is JsType.Number or JsType.BigInt)
        {
            return value;
        }

        var primitive = value.IsObject ? ToPrimitive(value, "number") : value;
        return primitive.IsBigInt ? primitive : JsValue.Number(ToNumber(primitive));
    }

    /// <summary>The abstract operation <c>ToString</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C35FB5
    // Broiler-Human:        PENDING
    internal string ToStringValue(JsValue value) => value.Type switch
    {
        JsType.String => value.AsString(),
        JsType.Number => JsNumberFormat.ToJsString(value.AsNumber()),
        JsType.Boolean => value.AsBoolean() ? "true" : "false",
        JsType.Undefined => "undefined",
        JsType.Null => "null",

        // A SYMBOL DOES NOT COERCE, AND THAT IS THE WHOLE POINT OF THE TYPE. Every other primitive
        // has a String it turns into, so a Symbol that also had one would be usable everywhere a
        // String is - which is exactly what a key nobody can forge must not be. `String(symbol)`
        // and `symbol.toString()` are the explicit forms the language nonetheless provides, and
        // they go through the Symbol intrinsic rather than through here.
        JsType.Symbol => ThrowTypeError("Cannot convert a Symbol value to a string").AsString(),

        // A BIGINT HAS A STRING, AND IT IS EXACT: `BigInt::toString(x, 10)`, with no `n`. The
        // conversion is charged on the digits it can produce before it produces them.
        JsType.BigInt => BigIntText(value.AsBigInt()),
        _ => ToStringValue(ToPrimitive(value, "string")),
    };

    /// <summary>
    /// <c>BigInt::toString(x, 10)</c>, charged on the digits it can produce and on the square of the
    /// value's width, step by step (JSeal B03).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=E0D910
    // Broiler-Falsified-If: a wider BigInt is converted to text for the same charge as a narrower one
    // Broiler-Human:        PENDING
    private string BigIntText(JsBigInt value) => value.ToDecimalString(ChargeFuel);

    /// <summary>
    /// The abstract operation <c>ToBigInt</c>: a BigInt from a Boolean, a BigInt or a String, after
    /// one <c>ToPrimitive</c> with the <c>"number"</c> hint; anything else is refused as the
    /// specification refuses it.
    /// </summary>
    /// <remarks>
    /// <b>A Number is a TypeError here, not a conversion</b>: <c>BigInt.asIntN(8, 1)</c> must not
    /// quietly accept a value that might have lost its low bits on the way in. The one caller that
    /// takes a Number, the <c>BigInt</c> function, tests for it before calling this.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=66E934
    // Broiler-Falsified-If: a Number, undefined, null or a Symbol converts to a BigInt, or a String outside StringIntegerLiteral answers anything but a SyntaxError
    // Broiler-Human:        PENDING
    internal JsBigInt ToBigInt(JsValue value) => PrimitiveToBigInt(ToPrimitive(value, "number"));

    /// <summary><see cref="ToBigInt"/> after its <c>ToPrimitive</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=A8BFC5
    // Broiler-Falsified-If: a Number, undefined, null or a Symbol converts to a BigInt
    // Broiler-Human:        PENDING
    internal JsBigInt PrimitiveToBigInt(JsValue primitive)
    {
        return primitive.Type switch
        {
            JsType.BigInt => primitive.AsBigInt(),
            JsType.Boolean => primitive.AsBoolean() ? JsBigInt.One : JsBigInt.Zero,
            JsType.String => StringToBigIntOrThrow(primitive.AsString()),
            JsType.Number => throw Error("TypeError", "Cannot convert " + JsNumberFormat.ToJsString(primitive.AsNumber()) + " to a BigInt"),
            JsType.Symbol => throw Error("TypeError", "Cannot convert a Symbol value to a BigInt"),
            _ => throw Error("TypeError", "Cannot convert " + (primitive.Type == JsType.Null ? "null" : "undefined") + " to a BigInt"),
        };
    }

    /// <summary>
    /// <c>StringToBigInt</c> for a caller that needs a value: a SyntaxError for text outside the
    /// grammar, and the ceiling's RangeError for a value too wide for the realm.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=60A446
    // Broiler-Falsified-If: a text outside StringIntegerLiteral answers a value, or a value past the ceiling is answered
    // Broiler-Human:        PENDING
    internal JsBigInt StringToBigIntOrThrow(string text)
    {
        if (!JsBigInt.TryParseStringInteger(text, ChargeFuel, out var parsed, out _))
        {
            throw Error("SyntaxError", "Cannot convert " + (text.Length > 64 ? text[..64] + "..." : text) + " to a BigInt");
        }

        return parsed ?? BigIntResult(null).AsBigInt();
    }

    /// <summary>
    /// The abstract operation <c>NumberToBigInt</c>: the exact integer an integral Number is, and a
    /// RangeError for a fraction, a NaN or an infinity.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=AABAD1
    // Broiler-Falsified-If: a Number that is not an integer converts to a BigInt, or an integral one converts to any integer but its own
    // Broiler-Human:        PENDING
    internal JsBigInt NumberToBigInt(double number)
    {
        if (!double.IsFinite(number) || Math.Floor(number) != number)
        {
            throw Error("RangeError", "The number " + JsNumberFormat.ToJsString(number) + " cannot be converted to a BigInt because it is not an integer");
        }

        return JsBigInt.FromIntegralNumber(number);
    }

    /// <summary>
    /// The conversion every write into a typed-array element makes first: <c>ToBigInt</c> for a
    /// BigInt kind and <c>ToNumber</c> for every other, as the specification's
    /// <c>TypedArraySetElement</c> and the built-ins that follow it choose by the view's
    /// <c>[[ContentType]]</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The answer is a value the element write stores without converting again</b>, and it is
    /// always of the view's content type: a Number for a Number kind, a BigInt for a BigInt kind.
    /// Assigning <c>1</c> to a <c>BigInt64Array</c> element is therefore the <c>TypeError</c>
    /// <c>ToBigInt</c> owes a Number, and assigning <c>1n</c> to a <c>Float64Array</c> element the
    /// one <c>ToNumber</c> owes a BigInt; neither is silently converted the other way.
    /// </para>
    /// <para>
    /// <b>A BigInt wider than 64 bits is narrowed here, once, and charged for it</b>, because the
    /// narrowing reads every word of the value; the narrowed value writes the same eight bytes the
    /// wide one would, so a <c>fill</c> that stores it a million times masks it once.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=AB31B4
    // Broiler-Falsified-If: a BigInt kind accepts a Number, a Number kind accepts a BigInt, or either converts with the other content type's operation
    // Broiler-Human:        PENDING
    internal JsValue ToElementValue(JsElementKind kind, JsValue value)
    {
        if (!JsElements.HoldsBigInts(kind))
        {
            return JsValue.Number(ToNumber(value));
        }

        var integer = ToBigInt(value);

        if (integer.Bits <= 64)
        {
            return JsValue.BigInt(integer);
        }

        Charge(JsBigInt.LinearCost(integer.Words));
        return JsValue.BigInt(JsElements.Narrow(kind, integer));
    }

    /// <summary>
    /// The Number a BigInt stands for, <c>F(R(x))</c>: the nearest one, ties to even, charged on
    /// its width. <c>Number(x)</c> is the one operation that asks for it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=1D0F36
    // Broiler-Falsified-If: a BigInt converts to a Number other than the nearest one with ties to even
    // Broiler-Human:        PENDING
    internal double BigIntToNumber(JsBigInt value) => value.ToNumber(ChargeFuel);

    /// <summary>The abstract operation <c>ToObject</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D3B49B
    // Broiler-Human:        PENDING
    internal JsObject ToObject(JsValue value) => value.Type switch
    {
        JsType.Object => value.AsObject(),
        JsType.String => Realm.WrapString(value.AsString()),
        JsType.Number => new JsPrimitiveWrapper(Realm.NumberPrototype, "Number", value),
        JsType.Boolean => new JsPrimitiveWrapper(Realm.BooleanPrototype, "Boolean", value),
        JsType.Symbol => new JsPrimitiveWrapper(Realm.SymbolPrototype, "Symbol", value),

        // A BIGINT WRAPPER IS AN ORDINARY OBJECT WITH A [[BigIntData]] SLOT (JSeal B05): its
        // `Object.prototype.toString` tag comes from `BigInt.prototype[Symbol.toStringTag]` and not
        // from a class name, so the class is `Object`, as the specification's builtinTag is.
        JsType.BigInt => new JsPrimitiveWrapper(BigIntPrototypeOrRefuse(), "Object", value),
        _ => ThrowTypeError("Cannot convert undefined or null to object").AsObject(),
    };

    /// <summary>The abstract operation <c>ToInt32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1BA637
    // Broiler-Human:        PENDING
    internal int ToInt32(JsValue value) => JsValue.ToInt32(ToNumber(value));

    /// <summary>The abstract operation <c>ToUint32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=DBC89F
    // Broiler-Human:        PENDING
    internal uint ToUint32(JsValue value) => JsValue.ToUint32(ToNumber(value));

    /// <summary>The abstract operation <c>ToIntegerOrInfinity</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F80693
    // Broiler-Human:        PENDING
    internal double ToInteger(JsValue value) => JsValue.ToInteger(ToNumber(value));

    /// <summary>The abstract operation <c>ToPropertyKey</c>, over the string keys this surface has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7A65ED
    // Broiler-Human:        PENDING
    internal string ToPropertyKey(JsValue value) =>
        value.Type == JsType.String ? value.AsString() : ToStringValue(value);

    /// <summary>
    /// The abstract operation <c>ToPropertyKey</c> over both kinds of key, answering a String or a
    /// Symbol value.
    /// </summary>
    /// <remarks>
    /// <b>A Symbol survives, and so does a Symbol an object converts to.</b> <see cref="ToPropertyKey"/>
    /// answers a string and therefore refuses both, which is right only where the caller has already
    /// taken the Symbol path. This one is for a caller that has not: it runs <c>ToPrimitive</c> with
    /// the string hint exactly once, keeps a Symbol it produces, and converts anything else with
    /// <c>ToString</c>, which for a primitive runs no guest code.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=AD4C8B
    // Broiler-Human:        PENDING
    internal JsValue ToPropertyKeyValue(JsValue value)
    {
        if (value.IsSymbol || value.Type == JsType.String)
        {
            return value;
        }

        var primitive = ToPrimitive(value, "string");
        return primitive.IsSymbol ? primitive : JsValue.String(ToStringValue(primitive));
    }

    /// <summary>
    /// The one Symbol-keyed lookup the engine performs on its own behalf, for a well-known Symbol.
    /// </summary>
    /// <remarks>
    /// Every protocol the language expresses through a well-known Symbol — iteration, primitive
    /// coercion, instance testing — reads a method off a value and calls it. This is that read: it
    /// answers nothing when the property is absent or nullish, and a <c>TypeError</c> when it is
    /// present and not callable, which is what every one of those protocols says to do.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3B7FC6
    // Broiler-Human:        PENDING
    internal bool TryGetSymbolMethod(JsValue value, JsSymbol key, out JsValue method)
    {
        var found = GetSymbol(value, key);

        if (found.IsNullish)
        {
            method = JsValue.Undefined;
            return false;
        }

        if (!found.IsObject || !found.AsObject().IsCallable)
        {
            ThrowTypeError("a Symbol-keyed protocol member is not a function");
        }

        method = found;
        return true;
    }
}
