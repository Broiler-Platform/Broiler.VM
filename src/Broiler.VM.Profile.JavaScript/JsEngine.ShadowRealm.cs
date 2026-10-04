// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           0
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  3/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The engine's half of <c>ShadowRealm</c>: building a shadow realm, evaluating in it, and the
/// boundary every value crosses (JSD-0030 D3 to D8, under JSD-0040).
/// </summary>
/// <remarks>
/// <para>
/// <b>A shadow realm is an ordinary realm of the engine</b>, built from the same surface set as every
/// other, charged as <c>$262.createRealm</c> charges one, sharing the agent's Symbols and job queue,
/// and with no host surface told of it and no host member on its global (JSD-0030 D6, D7).
/// </para>
/// <para>
/// <b>Compilation goes where every compilation goes</b>: <c>evaluate</c> asks the engine's one
/// mediator for eval code, exactly as an indirect <c>eval</c> does, so a composition that refuses
/// guest compilation refuses it here and inside the shadow realm alike (JSD-0030 D5).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=7BBE7E
// Broiler-Falsified-If: a function runs in a ShadowRealm with a host member, or anything but a primitive or a wrapper crosses its boundary
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{
    /// <summary>
    /// The specification's <c>InitializeHostDefinedRealm</c> for a <c>ShadowRealm</c>: a new realm on
    /// this engine with nothing of a host's in it, charged before it is built.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=C4010D
    // Broiler-Falsified-If: a shadow realm is built uncharged, with a host member on its global, or from another surface set
    // Broiler-Human:        PENDING
    internal JsRealm CreateShadowRealm()
    {
        RetainOrAbort(RealmRetainedBytes);
        Charge(RealmFuel);
        return new JsRealm(this, shadow: true);
    }

    /// <summary>The specification's <c>PerformShadowRealmEval</c>.</summary>
    /// <remarks>
    /// <para>
    /// <b>The source is compiled in the caller's realm and run in the shadow realm</b>: a refusal, a
    /// source that does not parse and an early error are the caller's own errors, as the proposal has
    /// them raised before the evaluation's context is pushed; everything the evaluation itself throws
    /// - a global declaration check included - becomes a fresh <c>TypeError</c> of the caller's realm.
    /// </para>
    /// <para>
    /// <b>It is global eval code of the shadow realm</b>, sloppy, with the shadow realm's global as
    /// <c>this</c>: its lexical declarations are its own and its <c>var</c>s and functions become
    /// properties of that global, as an indirect <c>eval</c> there would make them.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=213CEA
    // Broiler-Falsified-If: source evaluated in a ShadowRealm compiles anywhere but through the engine's one mediator, or an exception object or a non-callable object crosses out of it
    // Broiler-Human:        PENDING
    internal JsValue ShadowRealmEvaluate(JsRealm evalRealm, string text)
    {
        var callerRealm = Realm;
        var (evaluated, unit, declaration) = LoadEvalCode(Format.JsFormat.EvalRequestFlags.None, text);

        if (declaration.PrivateNames.Length != 0)
        {
            ThrowSyntaxError(
                "the evaluated source names the private name " + declaration.PrivateNames[0][1..] +
                ", which no class encloses");
        }

        JsValue result;
        var view = JsEvalView.Global();
        var outer = EnterRealm(evalRealm);
        var threw = false;

        try
        {
            // ONE VIEW FOR THE DECLARATIONS AND THE BODY, as `EvaluateGlobal` has: the functions the
            // instantiation makes are bound through it when the body starts.
            if (declaration.Introduces)
            {
                InstantiateEvalDeclarations(view, null, declaration);
            }

            var code = evaluated.Functions[(int)unit];
            var boundary = new JsEnvironment((int)code.ScopeSlots, null, view);

            result = Execute(
                evaluated,
                (int)unit,
                boundary,
                JsValue.Object(evalRealm.GlobalObject),
                System.Array.Empty<JsValue>(),
                null,
                JsValue.Undefined,
                null,
                null,
                activeReferrer);
        }
        catch (JsThrow)
        {
            threw = true;
            result = JsValue.Undefined;
        }
        finally
        {
            LeaveRealm(outer);
        }

        // THE INNER EXCEPTION NEVER CROSSES (JSD-0030 D4): a fresh TypeError of the caller's realm,
        // whose message copies nothing the guest wrote.
        return threw
            ? ThrowTypeError("ShadowRealm.prototype.evaluate: the evaluation threw an exception")
            : GetWrappedValue(callerRealm, result);
    }

    /// <summary>
    /// The specification's <c>GetWrappedValue</c>: a primitive as itself, a callable as a new wrapped
    /// function of <paramref name="realm"/>, and anything else a <c>TypeError</c> of the running realm.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=31925B
    // Broiler-Falsified-If: a non-callable object crosses a ShadowRealm boundary, or a callable crosses as itself
    // Broiler-Human:        PENDING
    internal JsValue GetWrappedValue(JsRealm realm, JsValue value)
    {
        if (!value.IsObject)
        {
            return value;
        }

        if (!value.AsObject().IsCallable)
        {
            return ThrowTypeError("a ShadowRealm boundary admits only primitives and callables");
        }

        return JsValue.Object(WrappedFunctionCreate(realm, value.AsObject()));
    }

    /// <summary>
    /// The specification's <c>WrappedFunctionCreate</c>: a function of <paramref name="callerRealm"/>
    /// with a call and no construction, whose <c>length</c> and <c>name</c> are copied from
    /// <paramref name="target"/> as <c>CopyNameAndLength</c> copies them.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CE4D0D
    // Broiler-Human:        PENDING
    private JsNativeFunction WrappedFunctionCreate(JsRealm callerRealm, JsObject target)
    {
        Charge(1);
        JsNativeFunction? wrapped = null;

        wrapped = new JsNativeFunction(
            callerRealm,
            callerRealm.FunctionPrototype,
            string.Empty,
            0,
            (engine, thisValue, arguments) => engine.WrappedFunctionCall(target, thisValue, arguments));

        try
        {
            var length = 0.0;

            if (target.HasOwnProperty("length"))
            {
                var declared = GetProperty(JsValue.Object(target), "length");

                if (declared.IsNumber)
                {
                    var number = declared.AsNumber();

                    length = double.IsPositiveInfinity(number)
                        ? double.PositiveInfinity
                        : double.IsNegativeInfinity(number)
                            ? 0
                            : System.Math.Max(0, JsValue.ToInteger(number));
                }
            }

            wrapped.SetOwnProperty(
                "length", JsProperty.Data(JsValue.Number(length), JsPropertyAttributes.Configurable));

            var name = GetProperty(JsValue.Object(target), "name");
            wrapped.FunctionName = name.IsString ? name.AsString() : string.Empty;

            wrapped.SetOwnProperty(
                "name",
                JsProperty.Data(JsValue.String(wrapped.FunctionName), JsPropertyAttributes.Configurable));
        }
        catch (JsThrow)
        {
            ThrowTypeError("a ShadowRealm boundary could not copy the wrapped function's length and name");
        }

        return wrapped;
    }

    /// <summary>A wrapped function's <c>[[Call]]</c>, running in the wrapper's realm.</summary>
    /// <remarks>
    /// The arguments and the receiver cross inward, into the target's realm, and the result crosses
    /// back; every exception produced here is the wrapper's realm's, so one the target throws becomes
    /// a fresh <c>TypeError</c> of it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=DA03D8
    // Broiler-Falsified-If: a wrapped function hands its target an object, answers its caller an object, or lets the target's exception cross
    // Broiler-Human:        PENDING
    private JsValue WrappedFunctionCall(JsObject target, JsValue thisValue, JsValue[] arguments)
    {
        var callerRealm = Realm;
        var targetRealm = FunctionRealm(JsValue.Object(target));
        var wrappedArguments = new JsValue[arguments.Length];

        for (var at = 0; at < arguments.Length; at++)
        {
            wrappedArguments[at] = GetWrappedValue(targetRealm, arguments[at]);
        }

        var wrappedThis = GetWrappedValue(targetRealm, thisValue);
        JsValue result;

        try
        {
            result = Call(JsValue.Object(target), wrappedThis, wrappedArguments);
        }
        catch (JsThrow)
        {
            return ThrowTypeError("a wrapped function's target threw an exception");
        }

        return GetWrappedValue(callerRealm, result);
    }

    /// <summary>The specification's <c>ShadowRealmImportValue</c>.</summary>
    /// <remarks>
    /// The import is the engine's dynamic import, asked from the shadow realm against the running
    /// script or module, through the same mediator as every other; its rejection becomes a
    /// <c>TypeError</c> of the caller's realm through that realm's <c>%ThrowTypeError%</c>, and a
    /// fulfilment reads the export and wraps it into the caller's realm.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=31A99E
    // Broiler-Falsified-If: a module imported for a ShadowRealm loads anywhere but through the engine's one mediator, or its export crosses unwrapped
    // Broiler-Human:        PENDING
    internal JsValue ShadowRealmImportValue(JsRealm evalRealm, string specifier, string exportName)
    {
        var callerRealm = Realm;
        JsValue inner;
        var outer = EnterRealm(evalRealm);

        try
        {
            inner = DynamicImport(TopSite.Program!, activeReferrer, JsValue.String(specifier), JsValue.Undefined);
        }
        finally
        {
            LeaveRealm(outer);
        }

        var onFulfilled = callerRealm.Native(string.Empty, 1, (engine, thisValue, arguments) =>
        {
            var namespaceObject = arguments.Length == 0 ? JsValue.Undefined : arguments[0];

            if (!namespaceObject.IsObject || !engine.HasProperty(namespaceObject.AsObject(), exportName))
            {
                return engine.ThrowTypeError(
                    "ShadowRealm.prototype.importValue: the module has no export named " + exportName);
            }

            return engine.GetWrappedValue(callerRealm, engine.GetProperty(namespaceObject, exportName));
        });

        return JsValue.Object(callerRealm.PromiseThen(
            this,
            (JsPromiseObject)inner.AsObject(),
            JsValue.Object(onFulfilled),
            JsValue.Object(callerRealm.ThrowTypeErrorFunction)));
    }
}
