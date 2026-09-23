// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   16
// Annotated:        16/16
// Exempt:           13
// Human-reviewed:   0/16
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       16
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>Iterator</c> global, <c>%Iterator.prototype%</c>'s own members, and one prototype per
/// built-in iterator kind.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every built-in iterator kind has a prototype of its own, and all of them inherit from ONE
/// <c>%Iterator.prototype%</c>.</b> This realm used to build each iterator as an ordinary object
/// carrying its own <c>next</c> closure and hanging directly off <c>%Iterator.prototype%</c>, which
/// collapsed the chain the specification gives: a property installed on
/// <c>Object.getPrototypeOf([].values())</c> reached every String, Map and Set iterator too, and a
/// <c>next</c> extracted from one kind worked on another. Now <c>next</c> lives on
/// <c>%ArrayIteratorPrototype%</c>, <c>%StringIteratorPrototype%</c>, <c>%MapIteratorPrototype%</c>,
/// <c>%SetIteratorPrototype%</c> and <c>%RegExpStringIteratorPrototype%</c>, each checks the brand of
/// its receiver, and the instances carry only their cursor.
/// </para>
/// <para>
/// <b>The kinds stay specialized.</b> Each iterator's step is still the closure its kind builds -
/// an indexed walk, a code-point walk, a slot walk - and only the part that was the same everywhere
/// (the brand check, the done latch, the result object) moved to one shared <c>next</c>.
/// </para>
/// <para>
/// <b>Two accessors on <c>%Iterator.prototype%</c> are deliberately odd.</b> <c>constructor</c> and
/// <c>[Symbol.toStringTag]</c> are accessor pairs rather than data properties, for web
/// compatibility, and their setter is <c>SetterThatIgnoresPrototypeProperties</c>: assigning through
/// an object that inherits them defines an own data property on that object, and assigning to
/// <c>%Iterator.prototype%</c> itself is the <c>TypeError</c> a non-writable property would be.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%ArrayIteratorPrototype%</c>, which the typed arrays' iterators share.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3FD61D
    // Broiler-Human:        PENDING
    internal JsObject ArrayIteratorPrototype { get; private set; } = null!;

    /// <summary>
    /// The intrinsic <c>%ArrayIteratorPrototype%.next</c>, whatever a program has since stored in
    /// that property.
    /// </summary>
    /// <remarks>
    /// A drain that stands in for the iteration protocol over a plain Array may do so only while
    /// the prototype's own <c>next</c> is still this function; see
    /// <see cref="ArrayIterationIsIntrinsic"/>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BB94D9
    // Broiler-Human:        PENDING
    internal JsValue IntrinsicArrayIteratorNext { get; private set; }

    /// <summary><c>%StringIteratorPrototype%</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=503181
    // Broiler-Human:        PENDING
    internal JsObject StringIteratorPrototype { get; private set; } = null!;

    /// <summary><c>%MapIteratorPrototype%</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=508DF3
    // Broiler-Human:        PENDING
    internal JsObject MapIteratorPrototype { get; private set; } = null!;

    /// <summary><c>%SetIteratorPrototype%</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2B49D3
    // Broiler-Human:        PENDING
    internal JsObject SetIteratorPrototype { get; private set; } = null!;

    /// <summary><c>%RegExpStringIteratorPrototype%</c>, which <c>matchAll</c> answers with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E79D3A
    // Broiler-Human:        PENDING
    internal JsObject RegExpStringIteratorPrototype { get; private set; } = null!;

    /// <summary><c>%WrapForValidIteratorPrototype%</c>: what <c>Iterator.from</c> wraps with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7B7C1D
    // Broiler-Human:        PENDING
    private JsObject WrapForValidIteratorPrototype { get; set; } = null!;

    /// <summary>The <c>Iterator</c> constructor, which a construction's new target is compared to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A5C91E
    // Broiler-Human:        PENDING
    private JsNativeFunction IteratorConstructor { get; set; } = null!;

    /// <summary>Builds the kind-specific iterator prototypes on <c>%Iterator.prototype%</c>.</summary>
    /// <remarks>
    /// It runs from the Symbol setup, straight after <c>%Iterator.prototype%</c> exists, because the
    /// Array and String iterators are installed there. The RegExp and keyed-collection iterators are
    /// only BUILT at run time, so their prototypes being made here, earlier than their owners' setup
    /// runs, orders nothing that matters.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DD9167
    // Broiler-Human:        PENDING
    private void SetupIteratorKinds()
    {
        ArrayIteratorPrototype = IteratorKind("Array Iterator");
        _ = ArrayIteratorPrototype.TryGetOwnProperty("next", out var arrayNext);
        IntrinsicArrayIteratorNext = arrayNext.Value;
        StringIteratorPrototype = IteratorKind("String Iterator");
        MapIteratorPrototype = IteratorKind("Map Iterator");
        SetIteratorPrototype = IteratorKind("Set Iterator");
        RegExpStringIteratorPrototype = IteratorKind("RegExp String Iterator");
    }

    /// <summary>One kind-specific prototype: a branded <c>next</c> and the kind's tag.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=CD2DCD
    // Broiler-Human:        PENDING
    private JsObject IteratorKind(string tag)
    {
        var prototype = new JsObject(IteratorPrototype);

        Method(prototype, "next", 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return engine.Realm.BuiltinIteratorNext(engine, thisValue, prototype, tag);
        });

        // NOT WRITABLE, NOT ENUMERABLE, CONFIGURABLE: the descriptor every kind's tag has, and the
        // one the realm's other tags already use.
        prototype.SetOwnSymbol(
            ToStringTagSymbol, JsProperty.Data(JsValue.String(tag), JsPropertyAttributes.Configurable));

        return prototype;
    }

    /// <summary>
    /// Whether iterating <paramref name="value"/> through <paramref name="method"/> is exactly this
    /// realm's own Array iteration, so a drain may read its elements instead of stepping it.
    /// </summary>
    /// <remarks>
    /// <b>Both halves of the protocol have to be the intrinsics.</b> The method found must be
    /// <c>Array.prototype.values</c>, and the iterator it makes reads its <c>next</c> from
    /// <c>%ArrayIteratorPrototype%</c>, which a program may replace: Test262
    /// <c>TypedArrayConstructors/ctors/object-arg/iterated-array-with-modified-array-iterator.js</c>
    /// replaces it and expects the typed array built from what the replacement answers. The check
    /// reads an ordinary object's own property and runs no guest code.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AD47F4
    // Broiler-Human:        PENDING
    internal bool ArrayIterationIsIntrinsic(JsValue value, JsValue method) =>
        value.AsObjectOrNull() is JsArray &&
        ReferenceEquals(method.AsObjectOrNull(), arrayIterator.AsObjectOrNull()) &&
        ArrayIteratorPrototype.TryGetOwnProperty("next", out var next) &&
        !next.IsAccessor &&
        ReferenceEquals(next.Value.AsObjectOrNull(), IntrinsicArrayIteratorNext.AsObjectOrNull());

    /// <summary>The kind prototype a list iterator with <paramref name="tag"/> belongs to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=315AFE
    // Broiler-Human:        PENDING
    private JsObject IteratorKindPrototype(string tag) => tag switch
    {
        "Map Iterator" => MapIteratorPrototype,
        "Set Iterator" => SetIteratorPrototype,
        "RegExp String Iterator" => RegExpStringIteratorPrototype,
        "String Iterator" => StringIteratorPrototype,
        _ => ArrayIteratorPrototype,
    };

    /// <summary>The shared <c>next</c> of every built-in iterator kind.</summary>
    /// <remarks>
    /// <para>
    /// <b>The brand is the prototype the iterator was MADE for</b>, not the one it currently has:
    /// <c>Object.setPrototypeOf</c> can move an Array iterator under the Map iterator prototype, and
    /// the specification's <c>GeneratorValidate</c> still answers by the internal brand.
    /// </para>
    /// <para>
    /// <b>These are generators in the specification, and three of their states are observable.</b>
    /// A step that throws or finds its source exhausted retires the iterator for good, and a step
    /// that re-enters its own <c>next</c> - a <c>length</c> getter, a custom <c>exec</c> - is the
    /// <c>TypeError</c> a running generator answers with.
    /// </para>
    /// <para>
    /// <b>The RegExp String Iterator is not a generator, and neither rule holds for it.</b> Its
    /// <c>next</c> keeps a <c>[[Done]]</c> slot that only a <c>null</c> match or a non-global
    /// matcher's one match sets, so a step that throws leaves it resumable and a custom <c>exec</c>
    /// that re-enters <c>next</c> is simply answered (<see cref="JsBuiltinIterator.Resumable"/>).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=791F0C
    // Broiler-Human:        PENDING
    private JsValue BuiltinIteratorNext(JsEngine engine, JsValue thisValue, JsObject brand, string tag)
    {
        if (thisValue.AsObjectOrNull() is not JsBuiltinIterator iterator ||
            !ReferenceEquals(iterator.Brand, brand))
        {
            return engine.ThrowTypeError(
                "%" + tag.Replace(" ", string.Empty, System.StringComparison.Ordinal) +
                "Prototype%.next called on an incompatible receiver");
        }

        if (iterator.Running && !iterator.Resumable)
        {
            return engine.ThrowTypeError(tag + " is already running");
        }

        engine.Charge(1);

        if (iterator.Step is not { } step)
        {
            return JsValue.Object(IteratorResult(JsValue.Undefined, done: true));
        }

        iterator.Running = !iterator.Resumable;
        (bool Found, JsValue Value) answer;

        try
        {
            answer = step(engine);
        }
        catch (JsThrow) when (!iterator.Resumable)
        {
            iterator.Step = null;
            throw;
        }
        finally
        {
            iterator.Running = false;
        }

        if (!answer.Found)
        {
            // THE STEP IS DROPPED, which is the latch and also lets go of what it was walking.
            iterator.Step = null;
            return JsValue.Object(IteratorResult(JsValue.Undefined, done: true));
        }

        return JsValue.Object(IteratorResult(answer.Value, done: false));
    }

    /// <summary>
    /// Builds the <c>Iterator</c> constructor, <c>Iterator.from</c>, <c>Iterator.concat</c> and the
    /// members of <c>%Iterator.prototype%</c> that are not helpers.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=8A20D9
    // Broiler-Human:        PENDING
    private void SetupIterator()
    {
        IteratorConstructor = Constructor(
            "Iterator",
            0,
            IteratorPrototype,
            static (engine, thisValue, arguments) =>
            {
                _ = thisValue;
                _ = arguments;
                return engine.ThrowTypeError("Constructor Iterator requires 'new'");
            },

            // ABSTRACT: `new Iterator()` IS A TypeError AND `new Sub()` IS NOT. The receiver slot
            // carries the new target on this path, and the engine re-points the instance to the new
            // target's own `prototype` after this answers - which is `OrdinaryCreateFromConstructor`
            // for a subclass, and leaves `%Iterator.prototype%` where that `prototype` is not an
            // object.
            static (engine, newTarget, arguments) =>
            {
                _ = arguments;

                if (newTarget.Type == JsType.Undefined ||
                    ReferenceEquals(newTarget.AsObjectOrNull(), engine.Realm.IteratorConstructor))
                {
                    return engine.ThrowTypeError("Abstract class Iterator not directly constructable");
                }

                return JsValue.Object(new JsObject(engine.Realm.IteratorPrototype));
            });

        // `constructor` IS AN ACCESSOR PAIR, which Constructor() above has just defined as the
        // ordinary data back-link every other prototype carries. It is replaced, not added to.
        IteratorPrototype.SetOwnProperty(
            "constructor",
            JsProperty.Accessor(
                Native("get constructor", 0, static (engine, thisValue, arguments) =>
                {
                    _ = thisValue;
                    _ = arguments;
                    return JsValue.Object(engine.Realm.IteratorConstructor);
                }),
                Native("set constructor", 1, static (engine, thisValue, arguments) =>
                {
                    engine.Realm.SetterIgnoringPrototype(
                        engine, thisValue, JsValue.String("constructor"), IteratorArgument(arguments, 0));

                    return JsValue.Undefined;
                }),
                JsPropertyAttributes.Configurable));

        IteratorPrototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Accessor(
                Native("get [Symbol.toStringTag]", 0, static (engine, thisValue, arguments) =>
                {
                    _ = engine;
                    _ = thisValue;
                    _ = arguments;
                    return JsValue.String("Iterator");
                }),
                Native("set [Symbol.toStringTag]", 1, static (engine, thisValue, arguments) =>
                {
                    engine.Realm.SetterIgnoringPrototype(
                        engine,
                        thisValue,
                        JsValue.Symbol(engine.Realm.ToStringTagSymbol),
                        IteratorArgument(arguments, 0));

                    return JsValue.Undefined;
                }),
                JsPropertyAttributes.Configurable));

        WrapForValidIteratorPrototype = new JsObject(IteratorPrototype);

        Method(WrapForValidIteratorPrototype, "next", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var record = WrappedRecord(engine, thisValue, "next");

            // THE `next` READ WHEN THE WRAPPER WAS MADE, called with no arguments. The wrapper's
            // own `next` forwards nothing, and what it answers is passed through unchecked: the
            // caller of a wrapper is the one that inspects the result.
            return engine.Call(record.Next, record.Iterator, System.Array.Empty<JsValue>());
        });

        Method(WrapForValidIteratorPrototype, "return", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var record = WrappedRecord(engine, thisValue, "return");
            var method = engine.GetProperty(record.Iterator, "return");

            if (method.IsNullish)
            {
                return JsValue.Object(engine.Realm.IteratorResult(JsValue.Undefined, done: true));
            }

            return engine.Call(method, record.Iterator, System.Array.Empty<JsValue>());
        });

        Method(IteratorConstructor, "from", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var realm = engine.Realm;
            var record = realm.GetIteratorFlattenable(
                engine, IteratorArgument(arguments, 0), iterateStrings: true);

            // `OrdinaryHasInstance(%Iterator%, …)`: an iterator that already inherits from
            // `%Iterator.prototype%` is answered as itself. The walk goes through each link's
            // `[[GetPrototypeOf]]`, so a proxy's trap runs, and it is metered because a trap can
            // invent a chain with no end.
            for (var link = record.Iterator.AsObject().Prototype; link is not null; link = link.Prototype)
            {
                engine.Charge(1);

                if (ReferenceEquals(link, realm.IteratorPrototype))
                {
                    return record.Iterator;
                }
            }

            return JsValue.Object(new JsWrappedIterator(realm.WrapForValidIteratorPrototype, record));
        });

        SetupIteratorConcat();
        SetupIteratorHelpers();
    }

    /// <summary>The record behind a <c>%WrapForValidIteratorPrototype%</c> receiver.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3814AB
    // Broiler-Human:        PENDING
    private static JsIteratorRecord WrappedRecord(JsEngine engine, JsValue thisValue, string method)
    {
        if (thisValue.AsObjectOrNull() is JsWrappedIterator wrapped)
        {
            return wrapped.Iterated;
        }

        throw engine.Error(
            "TypeError", "%WrapForValidIteratorPrototype%." + method + " called on an incompatible receiver");
    }

    /// <summary>
    /// <c>SetterThatIgnoresPrototypeProperties(this, %Iterator.prototype%, key, value)</c>.
    /// </summary>
    /// <remarks>
    /// An object that INHERITS the accessor gets an own data property, as if the accessor were a
    /// writable data property it could shadow; an object that already HAS one is assigned through
    /// the ordinary path, which may reach its own setter; and <c>%Iterator.prototype%</c> itself
    /// refuses, as a non-writable property would in strict code.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7190AB
    // Broiler-Human:        PENDING
    private void SetterIgnoringPrototype(JsEngine engine, JsValue thisValue, JsValue key, JsValue value)
    {
        if (!thisValue.IsObject)
        {
            engine.ThrowTypeError("the setter was called on a value that is not an object");
        }

        var target = thisValue.AsObject();

        if (ReferenceEquals(target, IteratorPrototype))
        {
            engine.ThrowTypeError("Cannot assign to read only property of Iterator.prototype");
        }

        var owned = key.IsSymbol
            ? target.TryGetOwnSymbol(key.AsSymbol(), out _)
            : target.TryGetOwnProperty(key.AsString(), out _);

        if (owned)
        {
            if (key.IsSymbol)
            {
                engine.SetSymbol(thisValue, key.AsSymbol(), value, strict: true);
            }
            else
            {
                engine.SetProperty(thisValue, key.AsString(), value, strict: true);
            }

            return;
        }

        // CreateDataPropertyOrThrow. A proxy's own [[DefineOwnProperty]] runs its trap and throws on
        // a refusal; an ordinary object refuses only by being non-extensible.
        if (target is not JsProxy && !target.Extensible)
        {
            engine.ThrowTypeError("Cannot define a property on an object that is not extensible");
        }

        var property = JsProperty.Data(value, JsPropertyAttributes.Default);

        if (key.IsSymbol)
        {
            target.SetOwnSymbol(key.AsSymbol(), property);
        }
        else
        {
            target.SetOwnProperty(key.AsString(), property);
        }
    }

    /// <summary><c>GetIteratorDirect</c>: the object itself, and its <c>next</c> read once.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5F7C07
    // Broiler-Human:        PENDING
    internal static JsIteratorRecord GetIteratorDirect(JsEngine engine, JsValue iterator) =>
        new(iterator, engine.GetProperty(iterator, "next"));

    /// <summary><c>GetIteratorFlattenable</c>, as <c>Iterator.from</c> and <c>flatMap</c> use it.</summary>
    /// <remarks>
    /// <b>An object with no <c>[Symbol.iterator]</c> is taken as an iterator already</b>, which is
    /// the difference between this and <c>GetIterator</c>. A String primitive is iterated only when
    /// <paramref name="iterateStrings"/> says so - <c>Iterator.from("ab")</c> walks code points,
    /// while a <c>flatMap</c> mapper answering a string is a <c>TypeError</c>, so that a mapper that
    /// returns words does not silently flatten them into characters.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A0B4D4
    // Broiler-Human:        PENDING
    internal JsIteratorRecord GetIteratorFlattenable(JsEngine engine, JsValue value, bool iterateStrings)
    {
        if (!value.IsObject && !(iterateStrings && value.Type == JsType.String))
        {
            engine.ThrowTypeError(engine.Describe(value) + " is not an iterator or iterable object");
        }

        var iterator = engine.TryGetSymbolMethod(value, IteratorSymbol, out var method)
            ? engine.Call(method, value, System.Array.Empty<JsValue>())
            : value;

        if (!iterator.IsObject)
        {
            engine.ThrowTypeError("The result of the iterator method is not an object");
        }

        return GetIteratorDirect(engine, iterator);
    }

    /// <summary>Reads one argument, or <c>undefined</c> when the caller omitted it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=77C1DC
    // Broiler-Human:        PENDING
    private static JsValue IteratorArgument(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;
}

/// <summary>
/// One built-in iterator - Array, String, Map, Set or RegExp String - and the cursor it walks with.
/// </summary>
/// <remarks>
/// <b>The brand and the step are internal slots; everything a program sees is on the prototype.</b>
/// Guest code reaches the object and never the delegate, which is called only by the kind's own
/// <c>next</c> after the brand check.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4FFE28
// Broiler-Human:        PENDING
internal sealed class JsBuiltinIterator : JsObject
{
    /// <summary>Creates an iterator of one kind over the step that kind supplies.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=05CAF9
    // Broiler-Human:        PENDING
    internal JsBuiltinIterator(
        JsObject brand, string tag, System.Func<JsEngine, (bool Found, JsValue Value)> step)
        : base(brand, tag)
    {
        Brand = brand;
        Step = step;
    }

    /// <summary>The kind prototype this iterator was made for, which its <c>next</c> checks.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=AB0B65
    // Broiler-Human:        PENDING
    internal JsObject Brand { get; }

    /// <summary>The next step, or <see langword="null"/> once the iterator is finished.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=C2B2D5
    // Broiler-Human:        PENDING
    internal System.Func<JsEngine, (bool Found, JsValue Value)>? Step { get; set; }

    /// <summary>Whether a step is in progress, so that re-entry is refused.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=832C69
    // Broiler-Human:        PENDING
    internal bool Running { get; set; }

    /// <summary>
    /// Whether this kind keeps a <c>[[Done]]</c> slot instead of generator states: a throwing step
    /// does not retire it and re-entry is not refused. Only the RegExp String Iterator is.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3DAF2A
    // Broiler-Human:        PENDING
    internal bool Resumable { get; init; }
}

/// <summary>An object <c>Iterator.from</c> answered with: a record wrapped to inherit the helpers.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BA94A8
// Broiler-Human:        PENDING
internal sealed class JsWrappedIterator : JsObject
{
    /// <summary>Wraps one record.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B88759
    // Broiler-Human:        PENDING
    internal JsWrappedIterator(JsObject prototype, JsIteratorRecord iterated)
        : base(prototype) => Iterated = iterated;

    /// <summary>The <c>[[Iterated]]</c> slot: the iterator and the <c>next</c> read off it once.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=24E801
    // Broiler-Human:        PENDING
    internal JsIteratorRecord Iterated { get; }
}
