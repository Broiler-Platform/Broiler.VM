// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           0
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>Boolean</c> intrinsic: the constructor and the two methods on <c>Boolean.prototype</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>The two call forms differ in kind, not in degree.</b> <c>Boolean(v)</c> is the language's
/// <c>ToBoolean</c> spelled as a function and answers a primitive; <c>new Boolean(v)</c> answers an
/// object, and an object is truthy, so <c>new Boolean(false)</c> passes an <c>if</c>. That is the
/// most-reported wart of the wrapper types and reproducing it is the job.
/// </para>
/// <para>
/// <b><c>Boolean.prototype</c> is itself a wrapper.</b> The realm builds it as a
/// <see cref="JsPrimitiveWrapper"/> over <see cref="JsValue.False"/>, so the receiver coercion the
/// two methods share accepts the prototype object as well as any instance, and
/// <c>Boolean.prototype.toString()</c> answers <c>"false"</c> rather than throwing.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=99C8B8
    // Broiler-Human:        PENDING
    private void SetupBoolean()
    {
        Constructor(
            "Boolean",
            1,
            BooleanPrototype,
            static (engine, thisValue, arguments) =>
                JsValue.Boolean(ArgOfBoolean(arguments, 0).ToBooleanValue()),
            static (engine, thisValue, arguments) => JsValue.Object(
                new JsPrimitiveWrapper(
                    engine.Realm.BooleanPrototype,
                    "Boolean",
                    JsValue.Boolean(ArgOfBoolean(arguments, 0).ToBooleanValue()))));

        Method(
            BooleanPrototype,
            "toString",
            0,
            static (engine, thisValue, arguments) =>
                JsValue.String(BooleanOfThis(engine, thisValue, "toString") ? "true" : "false"));

        Method(
            BooleanPrototype,
            "valueOf",
            0,
            static (engine, thisValue, arguments) =>
                JsValue.Boolean(BooleanOfThis(engine, thisValue, "valueOf")));
    }

    /// <summary>The argument at <paramref name="at"/>, or <c>undefined</c> when there is none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=47D10E
    // Broiler-Human:        PENDING
    private static JsValue ArgOfBoolean(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>
    /// The specification's <c>thisBooleanValue</c>: the Boolean a <c>Boolean.prototype</c> method's
    /// receiver stands for.
    /// </summary>
    /// <remarks>
    /// A Boolean primitive answers itself and a wrapper answers what it boxes. Everything else -
    /// <c>undefined</c>, <c>null</c>, a Number, a String, a plain object, a wrapper over some other
    /// primitive - is a <c>TypeError</c> and not a coercion, which is what distinguishes these two
    /// methods from the ones that take <c>ToString</c> of whatever they are handed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D92BEF
    // Broiler-Human:        PENDING
    private static bool BooleanOfThis(JsEngine engine, JsValue value, string method)
    {
        if (value.Type == JsType.Boolean)
        {
            return value.AsBoolean();
        }

        if (value.IsObject &&
            value.AsObject() is JsPrimitiveWrapper wrapper &&
            wrapper.Primitive.Type == JsType.Boolean)
        {
            return wrapper.Primitive.AsBoolean();
        }

        throw engine.Error(
            "TypeError", "Boolean.prototype." + method + " requires that 'this' be a Boolean");
    }
}
