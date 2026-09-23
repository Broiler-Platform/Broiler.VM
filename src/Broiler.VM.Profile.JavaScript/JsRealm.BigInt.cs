// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           1
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  3/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>BigInt</c> intrinsic: the conversion function, <c>asIntN</c> and <c>asUintN</c>, and the
/// three methods and the tag on <c>BigInt.prototype</c> (JSeal B05, decision JSD-0033 section 7).
/// </summary>
/// <remarks>
/// <para>
/// <b>It is built only when the composition admits <c>broiler.javascript.bigint</c>.</b> A
/// composition that declines the surface refuses every artifact holding a BigInt constant or naming
/// this global at verification, so its realms hold no BigInt at all and a <c>typeof BigInt</c>
/// there answers <c>"undefined"</c> - a question answered, as for the binary surface's globals.
/// </para>
/// <para>
/// <b><c>BigInt</c> converts and does not construct.</b> <c>BigInt(v)</c> takes an integral Number
/// exactly and anything else through <c>ToBigInt</c>; <c>new BigInt(v)</c> is a TypeError before
/// its argument is looked at, as <c>new Symbol()</c> is. A BigInt object exists only through
/// <c>Object(1n)</c> and every other <c>ToObject</c>, and its class is <c>Object</c>: its
/// <c>Object.prototype.toString</c> tag is the prototype's <c>Symbol.toStringTag</c>.
/// </para>
/// <para>
/// <b><c>BigInt.prototype</c> is an ordinary object, not a BigInt object</b>, which is where it
/// differs from <c>Number.prototype</c> and <c>Boolean.prototype</c>: <c>BigInt.prototype.valueOf()</c>
/// is a TypeError, as the specification's <c>thisBigIntValue</c> makes it.
/// </para>
/// <para>
/// <b><c>toLocaleString</c> answers the decimal digits.</b> This realm has no <c>Intl</c>
/// (decision JSD-0027), so the method is the specification's implementation-defined form with no
/// locale data, as <c>Number.prototype.toLocaleString</c> already is here.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>
    /// <c>BigInt.prototype</c>, or <see langword="null"/> in a realm whose composition declined the
    /// BigInt surface.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=CA85ED
    // Broiler-Human:        PENDING
    internal JsObject? BigIntPrototype { get; private set; }

    /// <summary>Builds <c>BigInt</c>, its two statics and <c>BigInt.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D3D1FA
    // Broiler-Human:        PENDING
    private void SetupBigInt()
    {
        var prototype = new JsObject(ObjectPrototype);
        BigIntPrototype = prototype;

        var constructor = Constructor(
            "BigInt",
            1,
            prototype,
            static (engine, thisValue, arguments) =>
            {
                _ = thisValue;

                // THE NUMBER TEST COMES AFTER THE ONE ToPrimitive, so an object whose `valueOf`
                // answers `1` is `1n`, and a Number anywhere else in the language is refused.
                var primitive = engine.ToPrimitive(ArgOfBigInt(arguments, 0), "number");

                return JsValue.BigInt(
                    primitive.IsNumber
                        ? engine.NumberToBigInt(primitive.AsNumber())
                        : engine.PrimitiveToBigInt(primitive));
            },
            static (engine, thisValue, arguments) =>
            {
                _ = thisValue;
                _ = arguments;
                return engine.ThrowTypeError("BigInt is not a constructor");
            });

        Method(constructor, "asIntN", 2, static (engine, thisValue, arguments) =>
            BigIntWrap(engine, arguments, signed: true));

        Method(constructor, "asUintN", 2, static (engine, thisValue, arguments) =>
            BigIntWrap(engine, arguments, signed: false));

        Method(prototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            var value = BigIntOfThis(engine, thisValue, "toString");
            var radixArgument = ArgOfBigInt(arguments, 0);
            var radix = 10.0;

            if (radixArgument.Type != JsType.Undefined)
            {
                radix = engine.ToInteger(radixArgument);

                if (radix < 2 || radix > 36)
                {
                    return engine.ThrowRangeError("toString() radix must be between 2 and 36");
                }
            }

            return JsValue.String(value.ToRadixString((int)radix, engine.ChargeFuel));
        });

        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.String(
                BigIntOfThis(engine, thisValue, "toLocaleString").ToDecimalString(engine.ChargeFuel));
        });

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.BigInt(BigIntOfThis(engine, thisValue, "valueOf"));
        });

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("BigInt"), JsPropertyAttributes.Configurable));
    }

    /// <summary>
    /// <c>BigInt.asIntN</c> or <c>BigInt.asUintN</c>: <c>ToIndex</c> of the width, then
    /// <c>ToBigInt</c> of the value, in that order, then the internal operation (JSeal B04).
    /// </summary>
    /// <remarks>
    /// A width is at most <c>2 ** 53 - 1</c> after <c>ToIndex</c>; an unsigned result wider than the
    /// realm's ceiling - a negative value at a width past it - is the ceiling's RangeError, since no
    /// value in the realm can hold it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=2F9DBC
    // Broiler-Falsified-If: the value is converted before the width, a Number value is accepted, or a result past MaximumBits is answered
    // Broiler-Human:        PENDING
    private static JsValue BigIntWrap(JsEngine engine, JsValue[] arguments, bool signed)
    {
        var bits = (ulong)BinaryToIndex(engine, ArgOfBigInt(arguments, 0), "bits");
        var value = engine.ToBigInt(ArgOfBigInt(arguments, 1));

        var result = signed
            ? JsBigInt.AsIntN(bits, value, engine.ChargeFuel)
            : JsBigInt.AsUintN(bits, value, engine.ChargeFuel);

        return engine.BigIntResult(result);
    }

    /// <summary>The argument at <paramref name="at"/>, or <c>undefined</c> when there is none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=6271AA
    // Broiler-Human:        PENDING
    private static JsValue ArgOfBigInt(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>
    /// The specification's <c>thisBigIntValue</c>: the BigInt a <c>BigInt.prototype</c> method's
    /// receiver stands for.
    /// </summary>
    /// <remarks>
    /// A BigInt answers itself and a BigInt object answers what it boxes. Everything else -
    /// <c>BigInt.prototype</c> itself included - is a TypeError and not a coercion.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3D47B1
    // Broiler-Human:        PENDING
    private static JsBigInt BigIntOfThis(JsEngine engine, JsValue value, string method)
    {
        if (value.IsBigInt)
        {
            return value.AsBigInt();
        }

        if (value.IsObject &&
            value.AsObject() is JsPrimitiveWrapper wrapper &&
            wrapper.Primitive.IsBigInt)
        {
            return wrapper.Primitive.AsBigInt();
        }

        throw engine.Error(
            "TypeError", "BigInt.prototype." + method + " requires that 'this' be a BigInt");
    }
}
