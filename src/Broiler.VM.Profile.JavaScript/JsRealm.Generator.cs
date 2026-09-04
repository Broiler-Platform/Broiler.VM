// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   19
// Annotated:        19/19
// Exempt:           0
// Human-reviewed:   0/19
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       19
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// An iterator over a source it reads LIVE: what a <c>yield*</c> over an Array or a String steps.
/// </summary>
/// <remarks>
/// <para>
/// <b>It holds the source and an index, and it copies nothing.</b> The obvious implementation
/// snapshots the elements into an array at construction, and it is wrong twice over. It is wrong
/// about the language - the specification's Array iterator re-reads <c>length</c> and the element
/// at each step, so an array a delegation appends to while it is being consumed is observed
/// growing - and it is wrong about this profile's own rules, because a snapshot of an array whose
/// declared <c>length</c> is a billion is a twenty-four-gigabyte allocation that no budget here
/// meters. A live index allocates nothing and cannot exceed what the source already is.
/// </para>
/// <para>
/// <b>A String is stepped by CODE POINT and not by code unit.</b> A surrogate pair is one step of
/// a String iterator and two of an index loop, and splitting one yields two halves of a character
/// neither of which is a character. An unpaired surrogate is its own step, which keeps the round
/// trip through <c>join</c> lossless.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
// Broiler-Human:        PENDING
internal sealed class JsSourceIterator : JsObject
{
    /// <summary>Creates an iterator over <paramref name="source"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsSourceIterator(JsObject? prototype, string className, JsValue source)
        : base(prototype, className) => Source = source;

    /// <summary>The array-like or the String being stepped.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsValue Source { get; }

    /// <summary>How far into the source the next step starts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal int At { get; set; }

    /// <summary>Whether the source is exhausted, so every further step answers done.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal bool Finished { get; set; }
}

/// <summary>
/// The generator intrinsics: <c>%IteratorPrototype%</c>, <c>%GeneratorPrototype%</c>,
/// <c>%GeneratorFunction.prototype%</c> and the two iterator prototypes a <c>yield*</c> needs.
/// </summary>
/// <remarks>
/// <para>
/// <b>None of these is a global, in this realm or in any other.</b> The only way a program reaches
/// them is by walking up from a generator function or a generator object, which is exactly how the
/// specification arranges them, so building them here rather than in <c>SetupGlobal</c> is not a
/// hiding place - it is where they live.
/// </para>
/// <para>
/// <b>One declared divergence, and it is <c>Symbol</c>'s absence rather than this stage's
/// choice.</b> The specification puts <c>@@iterator</c> on <c>%IteratorPrototype%</c> and
/// <c>@@toStringTag</c> on <c>%GeneratorPrototype%</c>; this realm has no Symbol, so it has no key
/// to put either under. What a program can observe of them is preserved anyway:
/// <c>Object.prototype.toString</c> answers <c>[object Generator]</c> through the object's
/// <c>[[Class]]</c>, and the iteration protocol is reached through the engine rather than through a
/// property. The day <c>Symbol</c> exists, both become ordinary properties and nothing else here
/// moves.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%IteratorPrototype%</c>: the root every iterator's chain reaches.</summary>
    /// <remarks>
    /// It has no members in this realm, because its only member in the specification is
    /// <c>@@iterator</c>. It exists anyway, and is not skipped as an empty link, because the chain
    /// a program walks is observable: skipping it would make
    /// <c>Object.getPrototypeOf(Object.getPrototypeOf(g.prototype))</c> answer
    /// <c>Object.prototype</c>, which is one hop short of what every engine answers.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsObject IteratorPrototype { get; private set; } = null!;

    /// <summary><c>%GeneratorPrototype%</c>: where <c>next</c>, <c>return</c> and <c>throw</c> live.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsObject GeneratorPrototype { get; private set; } = null!;

    /// <summary><c>%GeneratorFunction.prototype%</c>: what every generator function inherits from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsObject GeneratorFunctionPrototype { get; private set; } = null!;

    /// <summary>The prototype of the iterator a <c>yield*</c> over an Array steps.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsObject ArrayIteratorPrototype { get; private set; } = null!;

    /// <summary>The prototype of the iterator a <c>yield*</c> over a String steps.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsObject StringIteratorPrototype { get; private set; } = null!;

    /// <summary>Builds the generator intrinsics and the two iterator prototypes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private void SetupGenerator()
    {
        IteratorPrototype = new JsObject(ObjectPrototype, "Iterator");
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

        GeneratorFunctionPrototype.SetOwnProperty(
            "prototype",
            JsProperty.Data(JsValue.Object(GeneratorPrototype), JsPropertyAttributes.Configurable));

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

        ArrayIteratorPrototype = new JsObject(IteratorPrototype, "Array Iterator");
        StringIteratorPrototype = new JsObject(IteratorPrototype, "String Iterator");

        // NEITHER ITERATOR PROTOTYPE HAS A `return`, and that is the specification's shape rather
        // than an omission. It is observable: a `gen.return()` that arrives while a `yield*` over
        // an array is suspended finds nothing to forward to, so the outer generator returns
        // immediately - which is what an engine does and what a probe over this family checks.
        Method(ArrayIteratorPrototype, "next", 0, static (engine, thisValue, arguments) =>
            ArrayIteratorNext(engine, thisValue));

        Method(StringIteratorPrototype, "next", 0, static (engine, thisValue, arguments) =>
            StringIteratorNext(engine, thisValue));
    }

    /// <summary>What a call or a construction of <c>GeneratorFunction</c> is told, and why.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private JsObject GlobalFunctionConstructor() =>
        GlobalObject.TryGetOwnProperty("Function", out var found) && found.Value.IsObject
            ? found.Value.AsObject()
            : FunctionPrototype;

    /// <summary>One step of an Array iterator: <c>length</c> and the element, both read now.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private static JsValue ArrayIteratorNext(JsEngine engine, JsValue thisValue)
    {
        if (thisValue.AsObjectOrNull() is not JsSourceIterator iterator)
        {
            return engine.ThrowTypeError(
                "Array Iterator.prototype.next called on an incompatible receiver");
        }

        engine.Charge(2);
        var length = engine.ToInteger(engine.GetProperty(iterator.Source, "length"));

        if (iterator.Finished || iterator.At >= length)
        {
            // ONCE DONE, ALWAYS DONE. An array that shrinks and then grows again would otherwise
            // restart an iterator that had already answered done, which is a step the language
            // does not have.
            iterator.Finished = true;
            return JsValue.Object(engine.Realm.IteratorResult(JsValue.Undefined, done: true));
        }

        var element = engine.GetIndexed(iterator.Source, JsValue.Number(iterator.At));
        iterator.At++;
        return JsValue.Object(engine.Realm.IteratorResult(element, done: false));
    }

    /// <summary>One step of a String iterator: the next whole code point.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private static JsValue StringIteratorNext(JsEngine engine, JsValue thisValue)
    {
        if (thisValue.AsObjectOrNull() is not JsSourceIterator iterator ||
            !iterator.Source.IsString)
        {
            return engine.ThrowTypeError(
                "String Iterator.prototype.next called on an incompatible receiver");
        }

        engine.Charge(2);
        var text = iterator.Source.AsString();

        if (iterator.At >= text.Length)
        {
            return JsValue.Object(engine.Realm.IteratorResult(JsValue.Undefined, done: true));
        }

        var width = char.IsHighSurrogate(text[iterator.At]) &&
            iterator.At + 1 < text.Length &&
            char.IsLowSurrogate(text[iterator.At + 1])
            ? 2
            : 1;

        var part = text.Substring(iterator.At, width);
        iterator.At += width;
        return JsValue.Object(engine.Realm.IteratorResult(JsValue.String(part), done: false));
    }

    /// <summary>Reads a resumption's one argument, or <c>undefined</c> when it was omitted.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private static JsValue GeneratorArgument(JsValue[] arguments) =>
        arguments.Length == 0 ? JsValue.Undefined : arguments[0];

    /// <summary>Builds the <c>{ value, done }</c> object every iteration step answers with.</summary>
    /// <remarks>
    /// Both properties are ordinary, writable, enumerable and configurable, which is what
    /// <c>CreateIterResultObject</c> says and what makes <c>JSON.stringify</c> of a step read the
    /// way a reader expects.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsGenerator CreateGenerator(JsScriptFunction function, JsFrame frame)
    {
        var declared = engine.GetProperty(JsValue.Object(function), "prototype");

        return new JsGenerator(
            declared.IsObject ? declared.AsObject() : GeneratorPrototype, frame);
    }
}
