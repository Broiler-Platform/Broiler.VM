// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           2
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The intrinsics of the two function kinds whose body may suspend:
/// <c>%GeneratorPrototype%</c>, <c>%GeneratorFunction.prototype%</c>, <c>%GeneratorFunction%</c>,
/// <c>%AsyncFunction.prototype%</c> and <c>%AsyncFunction%</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>None of these is a global, in this realm or in any other.</b> The only way a program reaches
/// them is by walking up from a generator function, a generator object or an async function, which
/// is exactly how the specification arranges them, so building them here rather than in
/// <c>SetupGlobal</c> is not a hiding place - it is where they live.
/// </para>
/// <para>
/// <b>An async function's intrinsic is one object where a generator's is three, and the asymmetry
/// is the language's.</b> A generator function has a <c>prototype</c> whose objects are what its
/// calls return, and those objects need <c>next</c>, <c>return</c> and <c>throw</c>. An async
/// function's call returns an ordinary <c>Promise</c> of this realm, so there is nothing for an
/// <c>%AsyncFunctionPrototype%</c> to be the prototype OF: the intrinsic exists only so that
/// <c>Object.getPrototypeOf(async function(){})</c> answers something other than
/// <c>Function.prototype</c> and <c>Object.prototype.toString</c> answers
/// <c>[object AsyncFunction]</c>. Giving it a <c>prototype</c> property would have been the natural
/// mistake and would make <c>new (async function(){})</c> look constructible.
/// </para>
/// <para>
/// <b>The iteration protocol is NOT built here, and that is the whole of what this file is not.</b>
/// <c>%IteratorPrototype%</c>, the Array and String iterators and the <c>{ value, done }</c> step
/// object are the realm's, keyed on the REAL <c>Symbol.iterator</c> and
/// <c>Symbol.toStringTag</c>, and a generator is wired onto those rather than onto a private
/// arrangement of its own. A second <c>%IteratorPrototype%</c> here would have made
/// <c>Object.getPrototypeOf(Object.getPrototypeOf(g()))</c> answer an object no other iterator in
/// the realm reaches, and <c>for (const x of g())</c> would have found no <c>Symbol.iterator</c>
/// to call.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%GeneratorPrototype%</c>: where <c>next</c>, <c>return</c> and <c>throw</c> live.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A989DE
    // Broiler-Human:        PENDING
    internal JsObject GeneratorPrototype { get; private set; } = null!;

    /// <summary><c>%GeneratorFunction.prototype%</c>: what every generator function inherits from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=359E82
    // Broiler-Human:        PENDING
    internal JsObject GeneratorFunctionPrototype { get; private set; } = null!;

    /// <summary>Builds the generator intrinsics on top of the realm's own iteration protocol.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=558980
    // Broiler-Human:        PENDING
    private void SetupGenerator()
    {
        GeneratorPrototype = new JsObject(IteratorPrototype, "Generator");
        GeneratorFunctionPrototype = new JsObject(FunctionPrototype, "GeneratorFunction");

        // THE BACK-LINK IS DEFINED FIRST, and the order is observable rather than cosmetic:
        // `Object.getOwnPropertyNames` yields non-index keys in creation order, so defining the
        // three methods first would answer `next,return,throw,constructor` where every engine
        // answers `constructor,next,return,throw`. Neither back-link is writable, which is also
        // what a descriptor query reports for them. A generator's `constructor` is an ORDINARY
        // OBJECT rather than a function - it is `%GeneratorFunction.prototype%` - and a reader who
        // assumed otherwise would call it and get a type error from the language.
        GeneratorPrototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(
                JsValue.Object(GeneratorFunctionPrototype), JsPropertyAttributes.Configurable));

        Method(GeneratorPrototype, "next", 1, static (engine, thisValue, arguments) =>
            engine.ResumeGenerator(
                thisValue, JsResumeMode.Next, GeneratorArgument(arguments), "next"));

        Method(GeneratorPrototype, "return", 1, static (engine, thisValue, arguments) =>
            engine.ResumeGenerator(
                thisValue, JsResumeMode.Return, GeneratorArgument(arguments), "return"));

        Method(GeneratorPrototype, "throw", 1, static (engine, thisValue, arguments) =>
            engine.ResumeGenerator(
                thisValue, JsResumeMode.Throw, GeneratorArgument(arguments), "throw"));

        // A GENERATOR IS AN ITERABLE ITERATOR THROUGH THE REAL SYMBOL, and inheriting the one
        // `%IteratorPrototype%` carries is not enough to say so: the specification puts
        // `@@toStringTag` on THIS object, and `Object.prototype.toString.call(g())` reads the tag
        // before it reads any `[[Class]]`. The `@@iterator` below is inherited rather than
        // redefined - it already answers `this` - and defining a second one here would have been
        // a copy of a function programs compare for identity.
        GeneratorPrototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Generator"), JsPropertyAttributes.Configurable));

        GeneratorFunctionPrototype.SetOwnProperty(
            "prototype",
            JsProperty.Data(JsValue.Object(GeneratorPrototype), JsPropertyAttributes.Configurable));

        GeneratorFunctionPrototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(
                JsValue.String("GeneratorFunction"), JsPropertyAttributes.Configurable));

        // THE CONSTRUCTOR EXISTS AND REFUSES, for the reason `Function` does. Reaching it needs two
        // hops off a generator function and it is not a global anywhere, but a program that gets
        // there finds the intrinsic the specification says is there, and what this manifest
        // declines is the one thing it does: turning source into code at run time.
        var constructor = new JsNativeFunction(
            GlobalFunctionConstructor(),
            "GeneratorFunction",
            1,
            static (engine, thisValue, arguments) => engine.ThrowTypeError(GeneratorFunctionRefusal),
            static (engine, thisValue, arguments) => engine.ThrowTypeError(GeneratorFunctionRefusal));

        constructor.SetOwnProperty(
            "prototype",
            JsProperty.Data(
                JsValue.Object(GeneratorFunctionPrototype), JsPropertyAttributes.None));

        GeneratorFunctionPrototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Configurable));

        SetupAsyncFunction();
    }

    /// <summary><c>%AsyncFunction.prototype%</c>: what every async function inherits from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsObject AsyncFunctionPrototype { get; private set; } = null!;

    /// <summary>Builds the async-function intrinsics.</summary>
    /// <remarks>
    /// <b>It is called from <see cref="SetupGenerator"/> and not from the realm's setup list</b>,
    /// because it needs the same one thing that does: the <c>Function</c> constructor that
    /// <c>SetupGlobal</c> has already published. Two entries in the list would have been two places
    /// to keep that ordering constraint in, and the second one is the one that would drift.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private void SetupAsyncFunction()
    {
        AsyncFunctionPrototype = new JsObject(FunctionPrototype, "AsyncFunction");

        AsyncFunctionPrototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("AsyncFunction"), JsPropertyAttributes.Configurable));

        // THE CONSTRUCTOR EXISTS AND REFUSES, for the reason `Function` and `GeneratorFunction` do.
        // What this manifest declines is the one thing it does - turning source into code at run
        // time - and a program that walks two hops off an async function finds the intrinsic the
        // specification says is there rather than an absence it has to guess the cause of.
        var constructor = new JsNativeFunction(
            GlobalFunctionConstructor(),
            "AsyncFunction",
            1,
            static (engine, thisValue, arguments) => engine.ThrowTypeError(AsyncFunctionRefusal),
            static (engine, thisValue, arguments) => engine.ThrowTypeError(AsyncFunctionRefusal));

        constructor.SetOwnProperty(
            "prototype",
            JsProperty.Data(JsValue.Object(AsyncFunctionPrototype), JsPropertyAttributes.None));

        AsyncFunctionPrototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Configurable));
    }

    /// <summary>What a call or a construction of <c>AsyncFunction</c> is told, and why.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private const string AsyncFunctionRefusal =
        "AsyncFunction: the broiler.javascript.wide manifest does not admit the AsyncFunction " +
        "constructor, because this profile declares no guest-initiated load and cannot turn " +
        "source into code at run time";

    /// <summary>What a call or a construction of <c>GeneratorFunction</c> is told, and why.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=31EF54
    // Broiler-Human:        PENDING
    private const string GeneratorFunctionRefusal =
        "GeneratorFunction: the broiler.javascript.wide manifest does not admit the " +
        "GeneratorFunction constructor, because this profile declares no guest-initiated load and " +
        "cannot turn source into code at run time";

    /// <summary>
    /// The realm's <c>Function</c> constructor, which <c>%GeneratorFunction%</c> inherits from.
    /// </summary>
    /// <remarks>
    /// It is read back off the global object rather than kept in a field, because <c>Function</c>
    /// is already the one intrinsic this realm publishes under that name and a second reference to
    /// it would be a second thing to keep in step.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=24C92C
    // Broiler-Human:        PENDING
    private JsObject GlobalFunctionConstructor() =>
        GlobalObject.TryGetOwnProperty("Function", out var found) && found.Value.IsObject
            ? found.Value.AsObject()
            : FunctionPrototype;

    /// <summary>Reads a resumption's one argument, or <c>undefined</c> when it was omitted.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2D44B6
    // Broiler-Human:        PENDING
    private static JsValue GeneratorArgument(JsValue[] arguments) =>
        arguments.Length == 0 ? JsValue.Undefined : arguments[0];

    /// <summary>Builds the <c>{ value, done }</c> object every iteration step answers with.</summary>
    /// <remarks>
    /// Both properties are ordinary, writable, enumerable and configurable, which is what
    /// <c>CreateIterResultObject</c> says and what makes <c>JSON.stringify</c> of a step read the
    /// way a reader expects.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E88AE9
    // Broiler-Human:        PENDING
    internal JsObject IteratorResult(JsValue value, bool done)
    {
        var result = new JsObject(ObjectPrototype);
        result.DefineOrdinary("value", value);
        result.DefineOrdinary("done", JsValue.Boolean(done));
        return result;
    }

    /// <summary>Builds a generator object over a frame that has not started.</summary>
    /// <remarks>
    /// <b>Its prototype is the generator function's own <c>prototype</c> property, read the way an
    /// ordinary <c>new</c> reads it</b>, so a program that replaced that property sees the
    /// replacement - which is what the specification's <c>OrdinaryCreateFromConstructor</c> does
    /// and what makes <c>Object.getPrototypeOf(g()) === g.prototype</c> a fact about the program
    /// rather than about this implementation.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2EB509
    // Broiler-Human:        PENDING
    internal JsGenerator CreateGenerator(JsScriptFunction function, JsFrame frame)
    {
        var declared = engine.GetProperty(JsValue.Object(function), "prototype");

        return new JsGenerator(
            declared.IsObject ? declared.AsObject() : GeneratorPrototype, frame);
    }
}
