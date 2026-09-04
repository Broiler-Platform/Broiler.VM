// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           15
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>An enumerator over the property names <c>for…in</c> visits.</summary>
/// <remarks>
/// It is an object because the operand stack holds values and a value that is not a primitive is
/// an object. Guest code can never reach it: nothing puts it in a property and the two opcodes
/// that touch it are the only ones that accept it.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5A6690
// Broiler-Human:        PENDING
internal sealed class JsEnumerator : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A6428A
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<string> keys;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=98C321
    // Broiler-Human:        PENDING
    private int at;

    /// <summary>Creates an enumerator over an already-collected key list.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BE0557
    // Broiler-Human:        PENDING
    internal JsEnumerator(System.Collections.Generic.List<string> names)
        : base(null, "Enumerator") => keys = names;

    /// <summary>Yields the next name, or answers that there is none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=083E20
    // Broiler-Human:        PENDING
    internal bool TryNext(out string key)
    {
        if (at >= keys.Count)
        {
            key = string.Empty;
            return false;
        }

        key = keys[at++];
        return true;
    }
}

/// <summary>
/// One realm: the global object, the intrinsics, and the factories the engine builds values with.
/// </summary>
/// <remarks>
/// <para>
/// <b>The intrinsics are built in dependency order and the order is load-bearing.</b>
/// <c>Object.prototype</c> is the root of every chain, <c>Function.prototype</c> is itself a
/// function whose prototype is <c>Object.prototype</c>, and every other prototype hangs off those
/// two. Building them in any other order produces objects whose chains end in nothing, and the
/// failure surfaces much later as a missing <c>toString</c>.
/// </para>
/// <para>
/// <b>A realm is per-instance.</b> Two instances over one verified handle each build their own,
/// which is what makes them unable to observe each other. It is also what makes the conformance
/// harness's "a fresh realm per test" requirement satisfiable without a new process.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3283F6
    // Broiler-Human:        PENDING
    private readonly JsEngine engine;

    /// <summary>Builds a realm on <paramref name="owner"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6AAA1A
    // Broiler-Human:        PENDING
    internal JsRealm(JsEngine owner)
    {
        engine = owner;

        ObjectPrototype = new JsObject(null);
        FunctionPrototype = new JsNativeFunction(
            ObjectPrototype, string.Empty, 0, static (_, _, _) => JsValue.Undefined);

        GlobalObject = new JsObject(ObjectPrototype, "global");

        ArrayPrototype = new JsArray(ObjectPrototype);
        StringPrototype = new JsPrimitiveWrapper(ObjectPrototype, "String", JsValue.String(string.Empty));
        NumberPrototype = new JsPrimitiveWrapper(ObjectPrototype, "Number", JsValue.Number(0));
        BooleanPrototype = new JsPrimitiveWrapper(ObjectPrototype, "Boolean", JsValue.False);
        ErrorPrototype = new JsObject(ObjectPrototype, "Error");
        DatePrototype = new JsObject(ObjectPrototype, "Date");
        RegExpPrototype = new JsObject(ObjectPrototype, "RegExp");

        SetupObject();
        SetupFunction();
        SetupArray();
        SetupString();
        SetupNumber();
        SetupBoolean();
        SetupError();
        SetupMath();
        SetupJson();
        SetupDate();
        SetupRegExp();
        SetupGlobal();

        // LAST, BECAUSE IT READS `Function` BACK OFF THE GLOBAL. `%GeneratorFunction%` inherits
        // from the `Function` constructor the way the specification says it does, and that
        // constructor is published by SetupFunction into the global object above.
        SetupGenerator();
    }

    /// <summary>The realm's global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=962FD4
    // Broiler-Human:        PENDING
    internal JsObject GlobalObject { get; }

    /// <summary><c>Object.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=26D261
    // Broiler-Human:        PENDING
    internal JsObject ObjectPrototype { get; }

    /// <summary><c>Function.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=83CBFF
    // Broiler-Human:        PENDING
    internal JsObject FunctionPrototype { get; }

    /// <summary><c>Array.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=09FBFD
    // Broiler-Human:        PENDING
    internal JsObject ArrayPrototype { get; }

    /// <summary><c>String.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2C71AA
    // Broiler-Human:        PENDING
    internal JsObject StringPrototype { get; }

    /// <summary><c>Number.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6C2277
    // Broiler-Human:        PENDING
    internal JsObject NumberPrototype { get; }

    /// <summary><c>Boolean.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=49B304
    // Broiler-Human:        PENDING
    internal JsObject BooleanPrototype { get; }

    /// <summary><c>Error.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B09800
    // Broiler-Human:        PENDING
    internal JsObject ErrorPrototype { get; }

    /// <summary><c>Date.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=21F6B6
    // Broiler-Human:        PENDING
    internal JsObject DatePrototype { get; }

    /// <summary><c>RegExp.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E63611
    // Broiler-Human:        PENDING
    internal JsObject RegExpPrototype { get; }

    /// <summary>The Error constructors, by name, so a thrown intrinsic reaches its own identity.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60A934
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<string, JsNativeFunction> ErrorConstructors { get; } =
        new(System.StringComparer.Ordinal);

    /// <summary>Builds a built-in function object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=87A99A
    // Broiler-Human:        PENDING
    internal JsNativeFunction Native(string name, int arity, JsNativeBody body) =>
        new(FunctionPrototype, name, arity, body);

    /// <summary>Defines a built-in method on <paramref name="host"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FF73D9
    // Broiler-Human:        PENDING
    internal void Method(JsObject host, string name, int arity, JsNativeBody body) =>
        host.SetOwnProperty(
            name,
            JsProperty.Data(
                JsValue.Object(Native(name, arity, body)),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

    /// <summary>Defines a built-in constructor on the global object and links its prototype.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0B795C
    // Broiler-Human:        PENDING
    internal JsNativeFunction Constructor(
        string name, int arity, JsObject prototype, JsNativeBody call, JsNativeBody construct)
    {
        var function = new JsNativeFunction(FunctionPrototype, name, arity, call, construct);

        function.SetOwnProperty(
            "prototype", JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));

        prototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(
                JsValue.Object(function),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        GlobalObject.SetOwnProperty(
            name,
            JsProperty.Data(
                JsValue.Object(function),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        return function;
    }

    /// <summary>A fresh Array holding <paramref name="values"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BE50B5
    // Broiler-Human:        PENDING
    internal JsArray NewArray(System.Collections.Generic.IEnumerable<JsValue> values)
    {
        var array = new JsArray(ArrayPrototype);

        foreach (var value in values)
        {
            array.Push(value);
        }

        return array;
    }

    /// <summary>An empty Array.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=127267
    // Broiler-Human:        PENDING
    internal JsArray NewArray() => new(ArrayPrototype);

    /// <summary>A fresh Error of the named intrinsic kind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BFBD54
    // Broiler-Human:        PENDING
    internal JsValue CreateError(string kind, string message)
    {
        var prototype = ErrorConstructors.TryGetValue(kind, out var constructor)
            ? engine.GetProperty(JsValue.Object(constructor), "prototype")
            : JsValue.Object(ErrorPrototype);

        var error = new JsObject(
            prototype.IsObject ? prototype.AsObject() : ErrorPrototype, "Error");

        error.SetOwnProperty(
            "message",
            JsProperty.Data(JsValue.String(message), JsPropertyAttributes.BuiltIn));

        return JsValue.Object(error);
    }

    /// <summary>Wraps a String primitive so a property access on it has an object to read.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A29652
    // Broiler-Human:        PENDING
    internal JsObject WrapString(string value) =>
        new JsPrimitiveWrapper(StringPrototype, "String", JsValue.String(value));

    /// <summary>Builds the <c>arguments</c> object of a frame.</summary>
    /// <remarks>
    /// It is UNMAPPED: writing <c>arguments[0]</c> does not write the first parameter. The mapped
    /// form is observable and this is a declared deviation rather than an oversight - the mapping
    /// only exists in sloppy-mode functions with simple parameter lists, and nothing this profile
    /// is built to run depends on it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=47016A
    // Broiler-Human:        PENDING
    internal JsObject CreateArguments(JsValue[] arguments, JsScriptFunction? callee)
    {
        var value = new JsObject(ObjectPrototype, "Arguments");

        for (var at = 0; at < arguments.Length; at++)
        {
            value.SetOwnProperty(
                JsNumberFormat.ToUintString((uint)at),
                JsProperty.Data(arguments[at], JsPropertyAttributes.Default));
        }

        value.SetOwnProperty(
            "length",
            JsProperty.Data(JsValue.Number(arguments.Length), JsPropertyAttributes.BuiltIn));

        if (callee is not null)
        {
            value.SetOwnProperty(
                "callee", JsProperty.Data(JsValue.Object(callee), JsPropertyAttributes.BuiltIn));
        }

        return value;
    }

    /// <summary>Builds a closure over a code unit.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0C1A55
    // Broiler-Human:        PENDING
    internal JsObject CreateClosure(
        JsProgram program, int unit, JsEnvironment environment, JsValue lexicalThis)
    {
        // A GENERATOR FUNCTION INHERITS FROM `%GeneratorFunction.prototype%` AND NOT FROM
        // `Function.prototype`, and it is two hops rather than one to the latter. That is what
        // makes `Object.getPrototypeOf(g).constructor.name` answer `GeneratorFunction`, and it is
        // also what carries the `[object GeneratorFunction]` this realm reports through
        // `[[Class]]` in the absence of a Symbol to hang a tag on.
        var isGenerator = program.Functions[unit].IsGenerator;

        var function = new JsScriptFunction(
            isGenerator ? GeneratorFunctionPrototype : FunctionPrototype, program, unit, environment)
        {
            LexicalThis = lexicalThis,
        };

        if (isGenerator)
        {
            function.ClassName = "GeneratorFunction";
        }

        function.SetOwnProperty(
            "length",
            JsProperty.Data(
                JsValue.Number(program.Functions[unit].ParameterCount),
                JsPropertyAttributes.Configurable));

        function.SetOwnProperty(
            "name",
            JsProperty.Data(
                JsValue.String(program.Functions[unit].Name), JsPropertyAttributes.Configurable));

        if (isGenerator)
        {
            // A GENERATOR FUNCTION'S `prototype` HAS NO `constructor` BACK-LINK, and giving it one
            // would be the natural mistake: nothing is ever constructed from it, so there is
            // nothing for a back-link to name. What a program reads as `g.prototype.constructor`
            // is INHERITED from `%GeneratorPrototype%` and is an object rather than a function.
            // The property is writable and neither enumerable nor configurable, which is the one
            // descriptor shape a generator function's `prototype` has.
            function.SetOwnProperty(
                "prototype",
                JsProperty.Data(
                    JsValue.Object(new JsObject(GeneratorPrototype, "Generator")),
                    JsPropertyAttributes.Writable));

            return function;
        }

        if (function.IsConstructor)
        {
            // EVERY ORDINARY FUNCTION GETS A FRESH `prototype` WITH A `constructor` BACK-LINK.
            // The conformance harness compares `thrown.constructor` against the constructor it
            // expects, so an object created by `new` has to be able to name what made it.
            var prototype = new JsObject(ObjectPrototype);

            prototype.SetOwnProperty(
                "constructor",
                JsProperty.Data(JsValue.Object(function), JsPropertyAttributes.BuiltIn));

            function.SetOwnProperty(
                "prototype",
                JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.Writable));
        }

        return function;
    }

    /// <summary>
    /// Collects the names <c>for…in</c> visits: own and inherited enumerable string keys, each
    /// once, shadowed names excluded.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=53AD7C
    // Broiler-Human:        PENDING
    internal JsEnumerator CreateEnumerator(JsEngine owner, JsValue target)
    {
        var names = new System.Collections.Generic.List<string>();

        if (target.IsNullish)
        {
            return new JsEnumerator(names);
        }

        var seen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);
        var current = target.IsObject ? target.AsObject() : owner.ToObject(target);

        while (current is not null)
        {
            foreach (var key in current.OwnPropertyNames())
            {
                if (!seen.Add(key))
                {
                    continue;
                }

                if (current.TryGetOwnProperty(key, out var property) && property.Enumerable)
                {
                    names.Add(key);
                }
            }

            current = current.Prototype;
        }

        return new JsEnumerator(names);
    }

    /// <summary>The engine this realm belongs to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=96CC19
    // Broiler-Human:        PENDING
    internal JsEngine Engine => engine;
}
