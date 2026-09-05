// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           2
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>Proxy</c> constructor and the three pieces of the realm a <see cref="JsProxy"/> borrows.
/// </summary>
/// <remarks>
/// <para>
/// <b><c>Proxy</c> is a constructor with no <c>prototype</c>, and both halves of that are
/// deliberate.</b> It is not callable without <c>new</c> — there is no useful thing a plain call
/// could return, since a proxy is an object identity rather than a conversion — and it has no
/// <c>prototype</c> property because a proxy does not inherit from a <c>Proxy.prototype</c>: its
/// prototype is its target's, answered through the <c>getPrototypeOf</c> trap. A
/// <c>Proxy.prototype</c> would be an object nothing is ever an instance of, and
/// <c>p instanceof Proxy</c> would then throw rather than be meaningless.
/// </para>
/// <para>
/// <b>The proxy itself is built with <see cref="JsRealm"/> and not with the engine</b>, because it
/// has to keep one: every trap is a call into guest code, and the object model's virtuals have no
/// engine parameter to pass one through. <see cref="JsProxy"/> explains why the realm is the thing
/// held rather than the engine.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>Builds <c>Proxy</c> and defines it on the global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=75A1AE
    // Broiler-Human:        PENDING
    private void SetupProxy()
    {
        var constructor = new JsNativeFunction(
            FunctionPrototype,
            "Proxy",
            2,
            static (engine, thisValue, arguments) =>
            {
                _ = thisValue;
                _ = arguments;

                // A PLAIN CALL IS REFUSED BEFORE THE ARGUMENTS ARE EVEN LOOKED AT, which is what
                // makes `Proxy({}, {})` a TypeError rather than a proxy nobody can reach. The
                // language marks this class of built-in by having no [[Call]] at all; this profile
                // gives every native both entry points, so the refusal is written out.
                return engine.ThrowTypeError("Constructor Proxy requires 'new'");
            },
            (engine, thisValue, arguments) =>
            {
                _ = thisValue;
                return JsValue.Object(
                    ProxyCreate(engine, ArgOfProxy(arguments, 0), ArgOfProxy(arguments, 1)));
            });

        GlobalObject.SetOwnProperty(
            "Proxy",
            JsProperty.Data(
                JsValue.Object(constructor),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        Method(constructor, "revocable", 2, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var made = ProxyCreate(engine, ArgOfProxy(arguments, 0), ArgOfProxy(arguments, 1));
            var pair = new JsObject(ObjectPrototype);

            // THE REVOKER IS ANONYMOUS AND TAKES NOTHING, and it closes over the proxy rather than
            // reading it back off the pair - so a program that reassigns `r.proxy` before calling
            // `r.revoke()` still revokes the proxy it was given. Calling it twice is not an error:
            // the second call finds the target already gone and has nothing left to do.
            var revoker = Native(string.Empty, 0, (_, _, _) =>
            {
                made.Revoke();
                return JsValue.Undefined;
            });

            pair.DefineOrdinary("proxy", JsValue.Object(made));
            pair.DefineOrdinary("revoke", JsValue.Object(revoker));
            return JsValue.Object(pair);
        });
    }

    /// <summary>The specification's <c>ProxyCreate</c>: both arguments are objects or neither.</summary>
    /// <remarks>
    /// <b>A proxy over a proxy is ordinary and is not special-cased.</b> The target may be any
    /// object at all, including a revoked proxy — which is refused not here but at the first
    /// internal method, because refusing at creation would make the check depend on when the
    /// revocation happened rather than on when the operation did.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1062ED
    // Broiler-Human:        PENDING
    private JsProxy ProxyCreate(JsEngine engine, JsValue target, JsValue handler)
    {
        if (!target.IsObject)
        {
            engine.ThrowTypeError("Cannot create proxy with a non-object as target");
        }

        if (!handler.IsObject)
        {
            engine.ThrowTypeError("Cannot create proxy with a non-object as handler");
        }

        return new JsProxy(this, target.AsObject(), handler.AsObject());
    }

    /// <summary>One argument, or <c>undefined</c> where there is none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=70A5C4
    // Broiler-Human:        PENDING
    private static JsValue ArgOfProxy(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>
    /// The specification's <c>ToPropertyDescriptor</c>, for the one caller outside this class.
    /// </summary>
    /// <remarks>
    /// The reading is the <c>Object</c> statics' own, which is the point: a descriptor a trap
    /// answers with and a descriptor <c>Object.defineProperty</c> is handed are the same kind of
    /// object and must be read by the same code, or a handler could describe a property in a shape
    /// only one of the two would accept.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=87AB76
    // Broiler-Human:        PENDING
    internal static ObjectDescriptorFields DescriptorFieldsOf(JsEngine engine, JsValue value) =>
        ObjectToDescriptorFields(engine, value);

    /// <summary>
    /// The specification's <c>FromPropertyDescriptor</c> over a descriptor that may be partial.
    /// </summary>
    /// <remarks>
    /// <b>It emits the fields the descriptor CARRIED and not the four a property has</b>, which is
    /// the difference from <see cref="ObjectDescriptorFor"/> beside it. That one describes a
    /// property, which always has all four; this one re-materialises a descriptor a program wrote,
    /// and <c>Object.defineProperty(p, "x", { value: 1 })</c> has to reach the <c>defineProperty</c>
    /// trap as an object with one key — a handler forwarding to <c>Reflect.defineProperty</c> would
    /// otherwise turn a partial redefinition into a total one, silently clearing the three
    /// attributes the caller never mentioned.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=496243
    // Broiler-Human:        PENDING
    internal JsObject DescriptorObjectOfFields(ObjectDescriptorFields fields)
    {
        var result = new JsObject(ObjectPrototype);

        if (fields.HasValue)
        {
            result.DefineOrdinary("value", fields.Value);
        }

        if (fields.HasWritable)
        {
            result.DefineOrdinary("writable", JsValue.Boolean(fields.Writable));
        }

        if (fields.HasGet)
        {
            result.DefineOrdinary(
                "get", fields.Getter is null ? JsValue.Undefined : JsValue.Object(fields.Getter));
        }

        if (fields.HasSet)
        {
            result.DefineOrdinary(
                "set", fields.Setter is null ? JsValue.Undefined : JsValue.Object(fields.Setter));
        }

        if (fields.HasEnumerable)
        {
            result.DefineOrdinary("enumerable", JsValue.Boolean(fields.Enumerable));
        }

        if (fields.HasConfigurable)
        {
            result.DefineOrdinary("configurable", JsValue.Boolean(fields.Configurable));
        }

        return result;
    }

    /// <summary>The specification's <c>SameValue</c>, likewise shared.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F4F7BF
    // Broiler-Human:        PENDING
    internal static bool SameValueOf(JsValue left, JsValue right) => ObjectSameValue(left, right);

    /// <summary>Which of the two non-callable tags a proxy reports.</summary>
    /// <remarks>
    /// It is `IsArray` and it is the realm's because that predicate lives beside <c>Array</c>; a
    /// proxy over an Array is an Array here for the same reason it is one to
    /// <c>Array.prototype.concat</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=919F8E
    // Broiler-Human:        PENDING
    internal string ProxyArrayTag(JsProxy proxy) =>
        ArrayIsArray(engine, proxy) ? "Array" : "Object";
}
