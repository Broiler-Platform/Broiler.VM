// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           19
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       17
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
/// One iterator record: the iterator the guest handed over, its <c>next</c>, and whether it is done.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is an object for the same reason <see cref="JsEnumerator"/> is</b> - the operand stack
/// holds values - and guest code can never reach it, because nothing stores it in a property and
/// only the four <c>Iterate…</c> opcodes accept one.
/// </para>
/// <para>
/// <b>The done flag is the whole of why <c>IteratorClose</c> is safe to emit unconditionally.</b>
/// The specification says an iterator that has already reported completion, or whose <c>next</c>
/// threw, is not asked for its <c>return</c>; carrying that here rather than in the lowering means
/// a <c>break</c>, a rest element and an exhausted loop all emit the same instruction and the
/// record decides.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=87F67B
// Broiler-Human:        PENDING
internal sealed class JsIteratorRecord : JsObject
{
    /// <summary>Creates a record over an iterator object and the <c>next</c> read off it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=46425A
    // Broiler-Human:        PENDING
    internal JsIteratorRecord(JsValue iterator, JsValue next)
        : base(null, "Iterator")
    {
        Iterator = iterator;
        Next = next;
    }

    /// <summary>The iterator object the guest's <c>[Symbol.iterator]</c> answered with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DA1E9E
    // Broiler-Human:        PENDING
    internal JsValue Iterator { get; }

    /// <summary>
    /// The <c>next</c> method, read ONCE when the record was made.
    /// </summary>
    /// <remarks>
    /// The specification reads it once at <c>GetIterator</c> and calls that same function for every
    /// step, so an iterator that replaces its own <c>next</c> mid-loop does not change what the loop
    /// calls. Re-reading each turn is the obvious implementation and it is observably wrong.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6ED865
    // Broiler-Human:        PENDING
    internal JsValue Next { get; }

    /// <summary>Whether this iterator has finished, or failed, and is owed no <c>return</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=08F038
    // Broiler-Human:        PENDING
    internal bool Done { get; set; }
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

    /// <summary>
    /// The one <c>%Array.prototype.values%</c>, which <c>arguments</c> borrows.
    /// </summary>
    /// <remarks>
    /// <b>It is cached rather than read back off <c>Array.prototype</c></b>, because the
    /// specification gives an <c>arguments</c> object the INTRINSIC function: a guest that deletes
    /// or replaces <c>Array.prototype[Symbol.iterator]</c> does not thereby make its own
    /// <c>arguments</c> uniterable.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=460DFC
    // Broiler-Human:        PENDING
    private JsValue arrayIterator = JsValue.Undefined;

    /// <summary>Builds a realm on <paramref name="owner"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=ABFA90
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

        // THE OPTIONAL SURFACES ARE BUILT BEFORE THE GLOBAL SETUP AND ONLY WHEN ADMITTED. A
        // composition that declined one has already had every artifact declaring it refused at
        // verification, so nothing that runs here can reach these names; building them anyway would
        // put a global on the object that no admitted program may use and that a `typeof` would
        // nonetheless find.
        if (owner.Admits(Format.JsSurfaces.Binary))
        {
            SetupBinary();
        }

        if (owner.Admits(Format.JsSurfaces.Dynamic))
        {
            SetupDynamic();
        }

        SetupSymbol();
        SetupCollections();
        SetupCollectionIterators();
        SetupPromise();
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A3FB01
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

        // `arguments` IS ITERABLE, ON THE REAL `Symbol.iterator`, which is what makes
        // `f(...arguments)` and `[...arguments]` the ordinary forwarding idioms they are in the
        // wild. It gets `%Array.prototype.values%` because it is an array-like with a `length`,
        // which is all that iterator reads.
        value.SetOwnSymbol(
            IteratorSymbol, JsProperty.Data(arrayIterator, JsPropertyAttributes.BuiltIn));

        return value;
    }

    /// <summary>Builds a closure over a code unit.</summary>
    /// <param name="program">The verified program the body lives in.</param>
    /// <param name="unit">Which code unit the body is.</param>
    /// <param name="environment">The environment record the closure captures.</param>
    /// <param name="lexicalThis">The creating frame's <c>this</c>.</param>
    /// <param name="lexicalThisBinding">The creating frame's <c>this</c> box, when it has one.</param>
    /// <param name="lexicalNewTarget">The creating frame's <c>new.target</c>.</param>
    /// <param name="lexicalActive">The creating frame's active function.</param>
    /// <remarks>
    /// <b>The four lexical values are recorded only for an arrow</b>, because an arrow is the one
    /// unit that has none of them of its own: <c>this</c>, <c>new.target</c> and both halves of
    /// <c>super</c> come from where it was written rather than from how it is called. Recording
    /// them on every closure would cost nothing and mean nothing, and it would make a reader think
    /// an ordinary function consults them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7A987F
    // Broiler-Human:        PENDING
    internal JsObject CreateClosure(
        JsProgram program,
        int unit,
        JsEnvironment environment,
        JsValue lexicalThis,
        JsCell? lexicalThisBinding,
        JsValue lexicalNewTarget,
        JsScriptFunction? lexicalActive)
    {
        // A GENERATOR FUNCTION INHERITS FROM `%GeneratorFunction.prototype%` AND NOT FROM
        // `Function.prototype`, and it is two hops rather than one to the latter. That is what
        // makes `Object.getPrototypeOf(g).constructor.name` answer `GeneratorFunction`, and it is
        // what `[object GeneratorFunction]` is reported through.
        var isGenerator = program.Functions[unit].IsGenerator;

        // AN ASYNC FUNCTION INHERITS FROM `%AsyncFunction.prototype%` FOR THE SAME REASON, and an
        // async ARROW is an async function: the two bits are independent and the prototype is
        // decided by the async one alone, which is what makes
        // `Object.prototype.toString.call(async () => {})` answer `[object AsyncFunction]`.
        var isAsync = program.Functions[unit].IsAsync;

        var function = new JsScriptFunction(
            isGenerator ? GeneratorFunctionPrototype
                : isAsync ? AsyncFunctionPrototype
                : FunctionPrototype,
            program,
            unit,
            environment);

        if (program.Functions[unit].IsArrow)
        {
            function.LexicalThis = lexicalThis;
            function.LexicalThisBinding = lexicalThisBinding;
            function.LexicalNewTarget = lexicalNewTarget;
            function.LexicalActiveFunction = lexicalActive;
        }

        if (isGenerator)
        {
            function.ClassName = "GeneratorFunction";
        }
        else if (isAsync)
        {
            function.ClassName = "AsyncFunction";
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

        // A CLASS CONSTRUCTOR'S `prototype` IS NOT THIS ONE. It is built by the instruction that
        // builds the class, with the attributes the language gives it - not writable, not
        // enumerable, not configurable - and creating an ordinary one here first would be an
        // allocation nothing keeps and a writable property momentarily standing where a
        // non-writable one belongs.
        if (function.IsConstructor && !program.Functions[unit].IsClassConstructor)
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
