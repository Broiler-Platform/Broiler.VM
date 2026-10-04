// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           2
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  3/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>ShadowRealm</c> intrinsic: a constructor whose instances own a realm of their own, and the
/// two methods that reach into it (JSD-0030 SR-3 and SR-5, under JSD-0040).
/// </summary>
/// <remarks>
/// <para>
/// <b>It is built only where the composition admitted <c>broiler.javascript.shadowrealm</c></b>, which
/// a descriptor admits only beside <c>broiler.javascript.dynamic</c> (JSD-0030 section 8): a realm
/// that cannot compile source has no way to evaluate anything in a child, so it has no
/// <c>ShadowRealm</c> global and <c>typeof ShadowRealm</c> answers <c>"undefined"</c>.
/// </para>
/// <para>
/// <b>Only primitives and callables cross</b>, through the engine's <c>GetWrappedValue</c>
/// (JSD-0030 D4): a callable as a new wrapped function every time, anything else as a
/// <c>TypeError</c> of the realm that is running, and an exception as a fresh <c>TypeError</c> of the
/// caller's realm whose message copies nothing the guest wrote.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=60DD8D
// Broiler-Falsified-If: a function runs in a ShadowRealm with a host member, or anything but a primitive or a wrapper crosses its boundary
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>ShadowRealm.prototype</c>, where the surface was admitted.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6F937D
    // Broiler-Human:        PENDING
    internal JsObject? ShadowRealmPrototype { get; private set; }

    /// <summary>Builds the <c>ShadowRealm</c> constructor and its prototype.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=91450B
    // Broiler-Falsified-If: a realm builds ShadowRealm without the dynamic surface, or its evaluate compiles outside the mediator
    // Broiler-Human:        PENDING
    private void SetupShadowRealm()
    {
        var prototype = new JsObject(ObjectPrototype);
        ShadowRealmPrototype = prototype;

        var constructor = Constructor(
            "ShadowRealm",
            0,
            prototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor ShadowRealm requires 'new'"),

            // THE PROTOTYPE IS READ FIRST, as `OrdinaryCreateFromConstructor` reads it before the
            // realm is built, and from `new.target`'s realm when its `prototype` is not an object.
            static (engine, newTarget, arguments) =>
            {
                var made = engine.PrototypeFromConstructor(newTarget, engine.Realm.ShadowRealmPrototype!);
                return JsValue.Object(new JsShadowRealmObject(made, engine.CreateShadowRealm()));
            });

        constructor.BuildsFromNewTarget = true;

        Method(prototype, "evaluate", 1, static (engine, thisValue, arguments) =>
        {
            var shadow = ShadowRealmOfThis(engine, thisValue, "evaluate");
            var source = arguments.Length == 0 ? JsValue.Undefined : arguments[0];

            if (!source.IsString)
            {
                return engine.ThrowTypeError("ShadowRealm.prototype.evaluate: the source text is not a String");
            }

            return engine.ShadowRealmEvaluate(shadow.Inner, source.AsString());
        });

        Method(prototype, "importValue", 2, static (engine, thisValue, arguments) =>
        {
            var shadow = ShadowRealmOfThis(engine, thisValue, "importValue");
            var specifier = engine.ToStringValue(arguments.Length == 0 ? JsValue.Undefined : arguments[0]);
            var exportName = arguments.Length < 2 ? JsValue.Undefined : arguments[1];

            if (!exportName.IsString)
            {
                return engine.ThrowTypeError("ShadowRealm.prototype.importValue: the export name is not a String");
            }

            return engine.ShadowRealmImportValue(shadow.Inner, specifier, exportName.AsString());
        });

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("ShadowRealm"), JsPropertyAttributes.Configurable));
    }

    /// <summary>The specification's <c>ValidateShadowRealmObject</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EFCC94
    // Broiler-Human:        PENDING
    private static JsShadowRealmObject ShadowRealmOfThis(JsEngine engine, JsValue thisValue, string method) =>
        thisValue.AsObjectOrNull() as JsShadowRealmObject ??
        throw engine.Error(
            "TypeError", "ShadowRealm.prototype." + method + " requires that 'this' be a ShadowRealm");
}

/// <summary>A <c>ShadowRealm</c> instance: an ordinary object with a <c>[[ShadowRealm]]</c> slot.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0787BB
// Broiler-Human:        PENDING
internal sealed class JsShadowRealmObject : JsObject
{
    /// <summary>Creates an instance over the realm it owns.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C96010
    // Broiler-Human:        PENDING
    internal JsShadowRealmObject(JsObject prototype, JsRealm inner)
        : base(prototype) => Inner = inner;

    /// <summary>The instance's <c>[[ShadowRealm]]</c>: the realm its <c>evaluate</c> runs source in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1EFEBD
    // Broiler-Human:        PENDING
    internal JsRealm Inner { get; }
}
