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
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Function.prototype</c> - <c>call</c>, <c>apply</c>, <c>bind</c> and <c>toString</c> - and the
/// <c>Function</c> constructor, which exists so <c>typeof Function</c> answers and refuses so the
/// manifest's "no guest-initiated load" declaration is true of the running engine and not only of
/// the descriptor.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>What a call or a construction of <c>Function</c> is told, and why.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1F0493
    // Broiler-Human:        PENDING
    private const string FunctionConstructorRefusal =
        "Function: the broiler.javascript.wide manifest does not admit the Function constructor, " +
        "because this profile declares no guest-initiated load and cannot turn source into code " +
        "at run time";

    /// <summary>Builds <c>Function.prototype</c>'s members and the refused <c>Function</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F16C2D
    // Broiler-Human:        PENDING
    private void SetupFunction()
    {
        var functionPrototype = FunctionPrototype;

        // `Function.prototype` IS ITSELF A FUNCTION, and it is the one whose own `length` is 0 and
        // whose own `name` is the empty string. Both are non-writable, non-enumerable and
        // configurable - which is exactly what a descriptor query reports for them in every
        // engine, and what makes `delete Function.prototype.name` succeed and an assignment to it
        // silently do nothing.
        functionPrototype.SetOwnProperty(
            "length", JsProperty.Data(JsValue.Number(0), JsPropertyAttributes.Configurable));

        functionPrototype.SetOwnProperty(
            "name",
            JsProperty.Data(JsValue.String(string.Empty), JsPropertyAttributes.Configurable));

        Method(functionPrototype, "call", 1, static (engine, thisValue, arguments) =>
            engine.Call(
                thisValue,
                ArgOfFunction(arguments, 0),
                FunctionTailArguments(engine, arguments, 1)));

        Method(functionPrototype, "apply", 2, static (engine, thisValue, arguments) =>
        {
            // THE CALLABILITY CHECK COMES BEFORE THE ARGUMENT LIST IS READ. Reading the list goes
            // through `length` and the index properties, either of which may be an accessor, and a
            // getter that runs before the receiver was rejected is an observable difference rather
            // than an internal one.
            _ = FunctionCallableReceiver(engine, thisValue, "apply");

            return engine.Call(
                thisValue,
                ArgOfFunction(arguments, 0),
                FunctionSpreadArguments(engine, ArgOfFunction(arguments, 1)));
        });

        Method(functionPrototype, "bind", 1, (engine, thisValue, arguments) =>
        {
            var target = FunctionCallableReceiver(engine, thisValue, "bind");
            var boundThis = ArgOfFunction(arguments, 0);
            var boundArguments = FunctionTailArguments(engine, arguments, 1);

            // THE BOUND FUNCTION'S ARITY IS THE TARGET'S MINUS WHAT WAS ALREADY SUPPLIED, floored
            // at zero. The target's `length` is read as a property rather than taken from the
            // function object, because a redefined `length` is what the specification reads and a
            // bound-of-a-bound has to see the first binding's answer.
            var declared = engine.GetProperty(thisValue, "length");

            var remaining =
                (declared.IsNumber ? JsValue.ToInteger(declared.AsNumber()) : 0) -
                boundArguments.Length;

            var arity = remaining <= 0
                ? 0
                : remaining >= int.MaxValue ? int.MaxValue : (int)remaining;

            var targetName = engine.GetProperty(thisValue, "name");

            var bound = new JsBoundFunction(functionPrototype, target, boundThis, boundArguments)
            {
                FunctionName =
                    "bound " + (targetName.IsString ? targetName.AsString() : string.Empty),
                DeclaredArity = arity,
            };

            bound.SetOwnProperty(
                "length", JsProperty.Data(JsValue.Number(arity), JsPropertyAttributes.Configurable));

            bound.SetOwnProperty(
                "name",
                JsProperty.Data(
                    JsValue.String(bound.FunctionName), JsPropertyAttributes.Configurable));

            return JsValue.Object(bound);
        });

        Method(functionPrototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            var receiver = FunctionCallableReceiver(engine, thisValue, "toString");

            // NO SOURCE TEXT IS KEPT, so every function renders as a native one. This is a stated
            // approximation: the specification returns the source a function was defined from, and
            // an engine that executes verified bytecode has thrown that text away long before a
            // guest can ask for it.
            var name = receiver is JsFunction function ? function.FunctionName : string.Empty;
            return JsValue.String("function " + name + "() { [native code] }");
        });

        // THE CONSTRUCTOR EXISTS AND REFUSES. Omitting the binding would make `typeof Function`
        // answer "undefined", which is a different and untrue statement about the surface: the
        // intrinsic is there, the prototype hangs off it, and what this manifest declines is the
        // one thing it does - compiling a string into a function at run time.
        _ = Constructor(
            "Function",
            1,
            functionPrototype,
            static (engine, thisValue, arguments) => engine.ThrowTypeError(FunctionConstructorRefusal),
            static (engine, thisValue, arguments) => engine.ThrowTypeError(FunctionConstructorRefusal));
    }

    /// <summary>Reads one argument, or <c>undefined</c> when the caller omitted it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=664504
    // Broiler-Human:        PENDING
    private static JsValue ArgOfFunction(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>The receiver of a <c>Function.prototype</c> method, which has to be callable.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=731C9A
    // Broiler-Human:        PENDING
    private static JsObject FunctionCallableReceiver(JsEngine engine, JsValue value, string method)
    {
        var receiver = value.AsObjectOrNull();

        if (receiver is null || !receiver.IsCallable)
        {
            throw engine.Error(
                "TypeError",
                "Function.prototype." + method + " called on a value that is not a function");
        }

        return receiver;
    }

    /// <summary>Copies the arguments from <paramref name="from"/> onward into a fresh list.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1DD7B0
    // Broiler-Human:        PENDING
    private static JsValue[] FunctionTailArguments(JsEngine engine, JsValue[] arguments, int from)
    {
        if (arguments.Length <= from)
        {
            return System.Array.Empty<JsValue>();
        }

        var count = arguments.Length - from;
        engine.Charge((ulong)count);
        var tail = new JsValue[count];
        System.Array.Copy(arguments, from, tail, 0, count);
        return tail;
    }

    /// <summary>The specification's <c>CreateListFromArrayLike</c>, as <c>apply</c> needs it.</summary>
    /// <remarks>
    /// An <c>arguments</c> object, an Array and a plain object with a <c>length</c> all arrive here
    /// and none is special-cased: the list is read through <c>length</c> and the index properties,
    /// which is the only reading that works for all three.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=22953A
    // Broiler-Human:        PENDING
    private static JsValue[] FunctionSpreadArguments(JsEngine engine, JsValue source)
    {
        if (source.IsNullish)
        {
            return System.Array.Empty<JsValue>();
        }

        if (!source.IsObject)
        {
            throw engine.Error(
                "TypeError",
                "Function.prototype.apply expects an array-like second argument");
        }

        var count = engine.ToUint32(engine.GetProperty(source, "length"));

        if (count == 0)
        {
            return System.Array.Empty<JsValue>();
        }

        // ONE FUEL UNIT PER ELEMENT, CHARGED AS THE LIST GROWS RATHER THAN ONCE UP FRONT. The
        // `length` is guest-controlled and may be four billion; charging inside the loop instead of
        // allocating the whole array first is what makes an absurd `length` a spent allowance
        // rather than a spent heap.
        var collected = new System.Collections.Generic.List<JsValue>(count < 1024 ? (int)count : 1024);

        for (var at = 0u; at < count; at++)
        {
            engine.Charge(1);
            collected.Add(engine.GetIndexed(source, JsValue.Number(at)));
        }

        // The argument list is TRANSIENT: it lives for one call and is collected with it.
        // Reporting it as retained charged the LiveBytes CEILING, which is never released, so a
        // program calling `apply` in a loop reached a ceiling for memory it had already given
        // back - which the RayTrace benchmark did after printing its score. Transient allocation
        // is bounded by the fuel this call already charged.
        return collected.ToArray();
    }
}
