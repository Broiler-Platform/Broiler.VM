// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           21
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>Symbol</c> intrinsic, the well-known Symbols, and the iteration protocol they carry.
/// </summary>
/// <remarks>
/// <para>
/// <b>A Symbol is here because a protocol needs a key nobody can forge.</b> Iteration, primitive
/// coercion and instance testing are all "read this member off the value and call it", and if the
/// member were a String key then any object with that property would be claiming to implement the
/// protocol — including one that happened to have a property of that name for its own reasons. A
/// Symbol is a key whose identity is the object, so only a program handed the Symbol can install it.
/// </para>
/// <para>
/// <b>The well-known Symbols are per-realm here, and the specification says they are per-agent.</b>
/// Two realms in one process would disagree about <c>Symbol.iterator</c>, so an object built in one
/// would not iterate in the other. This profile builds one realm per instance and nothing crosses
/// between them — there is no way for a value to travel from one realm to another, because there is
/// no shape in which a host could carry one — so the difference is not observable. It is written
/// down because it stops being unobservable the day an agent model exists.
/// </para>
/// <para>
/// <b>The registry <c>Symbol.for</c> uses is per-realm for the same reason and with the same
/// consequence.</b> The specification's is a per-AGENT list, deliberately shared across realms, and
/// this one is not.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>Symbol.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=AA69B1
    // Broiler-Human:        PENDING
    internal JsObject SymbolPrototype { get; private set; } = null!;

    /// <summary>The Symbol every iterable answers its iterator factory under.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B2E21C
    // Broiler-Human:        PENDING
    internal JsSymbol IteratorSymbol { get; } = new("Symbol.iterator", described: true);

    /// <summary>The Symbol an asynchronous iterable would answer under.</summary>
    /// <remarks>
    /// It exists so that a program can test for it and find nothing installed, which is the truthful
    /// answer: this surface has no asynchronous iteration. A realm that omitted the Symbol entirely
    /// would make <c>Symbol.asyncIterator</c> a <c>TypeError</c> rather than a question.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B953B3
    // Broiler-Human:        PENDING
    internal JsSymbol AsyncIteratorSymbol { get; } = new("Symbol.asyncIterator", described: true);

    /// <summary>The Symbol <c>instanceof</c> consults before its ordinary behaviour.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FBA318
    // Broiler-Human:        PENDING
    internal JsSymbol HasInstanceSymbol { get; } = new("Symbol.hasInstance", described: true);

    /// <summary>The Symbol <c>ToPrimitive</c> consults before <c>valueOf</c> and <c>toString</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3B76E2
    // Broiler-Human:        PENDING
    internal JsSymbol ToPrimitiveSymbol { get; } = new("Symbol.toPrimitive", described: true);

    /// <summary>The Symbol <c>Object.prototype.toString</c> reads a class name from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D74BF7
    // Broiler-Human:        PENDING
    internal JsSymbol ToStringTagSymbol { get; } = new("Symbol.toStringTag", described: true);

    /// <summary>The Symbol <c>Array.prototype.concat</c> consults about flattening.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=ADC372
    // Broiler-Human:        PENDING
    internal JsSymbol IsConcatSpreadableSymbol { get; } = new("Symbol.isConcatSpreadable", described: true);

    /// <summary>The Symbol a built-in would read a derived constructor from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6F8447
    // Broiler-Human:        PENDING
    internal JsSymbol SpeciesSymbol { get; } = new("Symbol.species", described: true);

    /// <summary>The Symbol <c>with</c> would read a blocklist from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F98486
    // Broiler-Human:        PENDING
    internal JsSymbol UnscopablesSymbol { get; } = new("Symbol.unscopables", described: true);

    /// <summary>The Symbol a synchronous disposer is registered under.</summary>
    /// <remarks>
    /// <c>DisposableStack.prototype.use</c> reads it when a resource is registered, not when the
    /// stack is disposed, so a program that replaces the member afterwards does not change what
    /// runs. It is per-realm like every other well-known Symbol here, for the reason the header
    /// gives.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=43B309
    // Broiler-Human:        PENDING
    internal JsSymbol DisposeSymbol { get; } = new("Symbol.dispose", described: true);

    /// <summary>The Symbol an asynchronous disposer is registered under.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FCBDB0
    // Broiler-Human:        PENDING
    internal JsSymbol AsyncDisposeSymbol { get; } = new("Symbol.asyncDispose", described: true);

    /// <summary>The four Symbols the String methods would dispatch a pattern object through.</summary>
    /// <remarks>
    /// They exist and nothing installs them. <c>String.prototype.match</c> and its three siblings
    /// here read a RegExp directly rather than dispatching, which is the divergence
    /// <c>JsRealm.RegExp.cs</c>'s header records; the Symbols are minted anyway so that a program
    /// testing for them gets <c>undefined</c> rather than a <c>TypeError</c> on <c>Symbol.match</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B80358
    // Broiler-Human:        PENDING
    internal JsSymbol MatchSymbol { get; } = new("Symbol.match", described: true);

    /// <summary>The Symbol <c>String.prototype.replace</c> would dispatch through.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=501FB1
    // Broiler-Human:        PENDING
    internal JsSymbol ReplaceSymbol { get; } = new("Symbol.replace", described: true);

    /// <summary>The Symbol <c>String.prototype.search</c> would dispatch through.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F23166
    // Broiler-Human:        PENDING
    internal JsSymbol SearchSymbol { get; } = new("Symbol.search", described: true);

    /// <summary>The Symbol <c>String.prototype.split</c> would dispatch through.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=47AE98
    // Broiler-Human:        PENDING
    internal JsSymbol SplitSymbol { get; } = new("Symbol.split", described: true);

    /// <summary>The Symbol <c>String.prototype.matchAll</c> dispatches through.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1AAACF
    // Broiler-Human:        PENDING
    internal JsSymbol MatchAllSymbol { get; } = new("Symbol.matchAll", described: true);

    /// <summary>The prototype every built-in iterator this realm makes inherits from.</summary>
    /// <remarks>
    /// It carries the one member <c>%IteratorPrototype%</c> has to have: a
    /// <c>[Symbol.iterator]</c> answering itself, so that an iterator is also an iterable and
    /// <c>for (const x of someArray.values())</c> works.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=56785C
    // Broiler-Human:        PENDING
    internal JsObject IteratorPrototype { get; private set; } = null!;

    /// <summary>The registry <c>Symbol.for</c> and <c>Symbol.keyFor</c> share.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9F8017
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, JsSymbol> symbolRegistry =
        new(System.StringComparer.Ordinal);

    /// <summary>Builds the <c>Symbol</c> intrinsic and the iterators the realm's own types need.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B221B4
    // Broiler-Human:        PENDING
    private void SetupSymbol()
    {
        SymbolPrototype = new JsObject(ObjectPrototype, "Symbol");
        IteratorPrototype = new JsObject(ObjectPrototype);

        var constructor = Constructor(
            "Symbol",
            0,
            SymbolPrototype,
            static (engine, thisValue, arguments) =>
            {
                _ = thisValue;

                if (arguments.Length == 0 || arguments[0].Type == JsType.Undefined)
                {
                    return JsValue.Symbol(new JsSymbol(string.Empty, described: false));
                }

                return JsValue.Symbol(
                    new JsSymbol(engine.ToStringValue(arguments[0]), described: true));
            },

            // `new Symbol()` IS A TYPE ERROR, and the language is deliberate about it: a Symbol
            // object would be truthy, would compare by reference under `==`, and would be usable as
            // a key that is not the Symbol it wraps. The constructor exists so `Symbol.iterator`
            // has somewhere to live, and refuses to construct.
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Symbol is not a constructor"));

        constructor.DefineFrozen("iterator", JsValue.Symbol(IteratorSymbol));
        constructor.DefineFrozen("asyncIterator", JsValue.Symbol(AsyncIteratorSymbol));
        constructor.DefineFrozen("hasInstance", JsValue.Symbol(HasInstanceSymbol));
        constructor.DefineFrozen("toPrimitive", JsValue.Symbol(ToPrimitiveSymbol));
        constructor.DefineFrozen("toStringTag", JsValue.Symbol(ToStringTagSymbol));
        constructor.DefineFrozen("isConcatSpreadable", JsValue.Symbol(IsConcatSpreadableSymbol));
        constructor.DefineFrozen("species", JsValue.Symbol(SpeciesSymbol));
        constructor.DefineFrozen("unscopables", JsValue.Symbol(UnscopablesSymbol));
        constructor.DefineFrozen("match", JsValue.Symbol(MatchSymbol));
        constructor.DefineFrozen("replace", JsValue.Symbol(ReplaceSymbol));
        constructor.DefineFrozen("search", JsValue.Symbol(SearchSymbol));
        constructor.DefineFrozen("split", JsValue.Symbol(SplitSymbol));
        constructor.DefineFrozen("matchAll", JsValue.Symbol(MatchAllSymbol));
        constructor.DefineFrozen("dispose", JsValue.Symbol(DisposeSymbol));
        constructor.DefineFrozen("asyncDispose", JsValue.Symbol(AsyncDisposeSymbol));

        Method(constructor, "for", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var key = engine.ToStringValue(SymbolArgument(arguments, 0));
            var registry = engine.Realm.symbolRegistry;

            if (!registry.TryGetValue(key, out var found))
            {
                found = new JsSymbol(key, described: true);
                registry[key] = found;
            }

            return JsValue.Symbol(found);
        });

        Method(constructor, "keyFor", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var value = SymbolArgument(arguments, 0);

            if (!value.IsSymbol)
            {
                return engine.ThrowTypeError("Symbol.keyFor requires a Symbol");
            }

            foreach (var (key, candidate) in engine.Realm.symbolRegistry)
            {
                if (ReferenceEquals(candidate, value.AsSymbol()))
                {
                    return JsValue.String(key);
                }
            }

            return JsValue.Undefined;
        });

        Method(SymbolPrototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.String(SymbolOfThis(engine, thisValue, "toString").Rendered);
        });

        Method(SymbolPrototype, "valueOf", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.Symbol(SymbolOfThis(engine, thisValue, "valueOf"));
        });

        SymbolPrototype.SetOwnProperty(
            "description",
            JsProperty.Accessor(
                Native("get description", 0, static (engine, thisValue, arguments) =>
                {
                    _ = arguments;
                    var symbol = SymbolOfThis(engine, thisValue, "description");
                    return symbol.Described ? JsValue.String(symbol.Description) : JsValue.Undefined;
                }),
                null,
                JsPropertyAttributes.Configurable));

        SymbolPrototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Symbol"), JsPropertyAttributes.Configurable));

        // A Symbol REFUSES to become a String or a Number however it is asked, and this is the hook
        // that makes `symbol + ""` a TypeError rather than a concatenation.
        SymbolPrototype.SetOwnSymbol(
            ToPrimitiveSymbol,
            JsProperty.Data(
                JsValue.Object(Native("[Symbol.toPrimitive]", 1, static (engine, thisValue, arguments) =>
                {
                    _ = arguments;
                    return JsValue.Symbol(SymbolOfThis(engine, thisValue, "[Symbol.toPrimitive]"));
                })),
                JsPropertyAttributes.Configurable));

        // `%Iterator.prototype%` HAS NO `next` OF ITS OWN, and it used to carry one that threw. The
        // specification gives it none: every built-in iterator's `next` lives on its kind's own
        // prototype, below, and an object a program builds on `Iterator.prototype` supplies its
        // own. A throwing `next` here would have answered `"next" in Iterator.prototype` wrongly
        // and turned a helper's missing-`next` TypeError into a different one.
        SetupIteratorKinds();

        IteratorPrototype.SetOwnSymbol(
            IteratorSymbol,
            JsProperty.Data(
                JsValue.Object(Native("[Symbol.iterator]", 0, static (engine, thisValue, arguments) =>
                {
                    _ = engine;
                    _ = arguments;
                    return thisValue;
                })),
                JsPropertyAttributes.BuiltIn));

        SetupIterators();
        SetupDatePrimitive();
    }

    /// <summary>
    /// Installs <c>Date.prototype[Symbol.toPrimitive]</c>, which is what makes a Date add as text.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A Date is the only object in the language whose DEFAULT hint means "string".</b>
    /// <c>date + ""</c> and <c>"" + date</c> use hint <c>"default"</c>, and ordinary
    /// <c>OrdinaryToPrimitive</c> answers that hint with <c>valueOf</c> — so without this a Date
    /// concatenated with a string produced its epoch MILLISECONDS, a number that looks like an
    /// answer and is not the one every program expects. <c>date - 0</c> keeps the number, because
    /// subtraction asks with hint <c>"number"</c>.
    /// </para>
    /// <para>
    /// <b>It is here rather than in the Date setup because the realm builds Date before it has a
    /// Symbol to key this with</b>, which is the same join the collection iterators are in.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=13EB2A
    // Broiler-Human:        PENDING
    private void SetupDatePrimitive() =>
        DatePrototype.SetOwnSymbol(
            ToPrimitiveSymbol,
            JsProperty.Data(
                JsValue.Object(Native("[Symbol.toPrimitive]", 1, static (engine, thisValue, arguments) =>
                {
                    if (!thisValue.IsObject)
                    {
                        return engine.ThrowTypeError(
                            "Date.prototype[Symbol.toPrimitive] is not generic");
                    }

                    var hint = arguments.Length == 0
                        ? string.Empty
                        : engine.ToStringValue(arguments[0]);

                    return hint switch
                    {
                        "number" => engine.OrdinaryToPrimitive(thisValue, "number"),
                        "string" or "default" => engine.OrdinaryToPrimitive(thisValue, "string"),
                        _ => engine.ThrowTypeError("the hint must be one of default, number and string"),
                    };
                })),
                JsPropertyAttributes.Configurable));

    /// <summary>Installs <c>[Symbol.iterator]</c> on the types this realm can iterate.</summary>
    /// <remarks>
    /// <b>Every one of these is a walk over a snapshot-free live view</b>, because that is what the
    /// language says: an array grown during iteration is iterated further, and one shortened during
    /// iteration stops early. Copying the elements first would be easier and would be a different
    /// program.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F0BEE1
    // Broiler-Human:        PENDING
    private void SetupIterators()
    {
        // EACH OF THE THREE BEGINS WITH ToObject (ES2026 Array.prototype.entries, keys and values;
        // since 2026-09-22, JSeal VM-FIX-J): a nullish receiver is a TypeError when the method is
        // called, not an iterator whose first step fails, and a primitive is iterated as its
        // wrapper.
        var arrayValues = Native("values", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var target = JsValue.Object(engine.ToObject(thisValue));
            return JsValue.Object(engine.Realm.CreateIndexedIterator(target, IndexedIteratorKind.Value));
        });

        ArrayPrototype.SetOwnProperty(
            "values",
            JsProperty.Data(JsValue.Object(arrayValues), JsPropertyAttributes.BuiltIn));

        Method(ArrayPrototype, "keys", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var target = JsValue.Object(engine.ToObject(thisValue));
            return JsValue.Object(engine.Realm.CreateIndexedIterator(target, IndexedIteratorKind.Key));
        });

        Method(ArrayPrototype, "entries", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var target = JsValue.Object(engine.ToObject(thisValue));
            return JsValue.Object(engine.Realm.CreateIndexedIterator(target, IndexedIteratorKind.Entry));
        });

        ArrayPrototype.SetOwnSymbol(
            IteratorSymbol, JsProperty.Data(JsValue.Object(arrayValues), JsPropertyAttributes.BuiltIn));

        // ONE FUNCTION OBJECT UNDER BOTH KEYS AND IN THE FIELD, because
        // `Array.prototype.values === Array.prototype[Symbol.iterator]` is a fact programs test
        // for, and because an `arguments` object is given this same intrinsic rather than whatever
        // `Array.prototype` happens to carry when it is built.
        arrayIterator = JsValue.Object(arrayValues);

        // A STRING ITERATES BY CODE POINT AND NOT BY CODE UNIT, which is the one place the language
        // treats a surrogate pair as one thing. Every index this profile exposes elsewhere is a code
        // unit, so this is a deliberate difference rather than an inconsistency.
        StringPrototype.SetOwnSymbol(
            IteratorSymbol,
            JsProperty.Data(
                JsValue.Object(Native("[Symbol.iterator]", 0, static (engine, thisValue, arguments) =>
                {
                    _ = arguments;

                    if (thisValue.IsNullish)
                    {
                        return engine.ThrowTypeError("String.prototype[Symbol.iterator] called on null or undefined");
                    }

                    return JsValue.Object(
                        engine.Realm.CreateStringIterator(engine.ToStringValue(thisValue)));
                })),
                JsPropertyAttributes.BuiltIn));
    }

    /// <summary>What an indexed iterator yields.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E0FCFF
    // Broiler-Human:        PENDING
    internal enum IndexedIteratorKind
    {
        /// <summary>The index.</summary>
        Key = 0,

        /// <summary>The element.</summary>
        Value = 1,

        /// <summary>A two-element Array of the index and the element.</summary>
        Entry = 2,
    }

    /// <summary>Builds an iterator over anything with a <c>length</c> and indexed properties.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A2CBCE
    // Broiler-Human:        PENDING
    internal JsObject CreateIndexedIterator(JsValue target, IndexedIteratorKind kind)
    {
        var at = 0u;

        // THE STEP IS THE WHOLE OF WHAT DIFFERS BETWEEN KINDS; `next` itself is
        // `%ArrayIteratorPrototype%.next`, shared, and the brand check, the latch and the result
        // object are its. The length is read afresh at every step, so an array grown during
        // iteration is iterated further - until the first step that finds it exhausted, after
        // which the latch holds whatever is pushed later.
        return new JsBuiltinIterator(ArrayIteratorPrototype, "Array Iterator", engine =>
        {
            // A TYPED ARRAY IS MEASURED THROUGH ITS BUFFER, NOT ITS `length` PROPERTY, and a view
            // that is detached or out of bounds at a step is a TypeError at that step
            // (%ArrayIteratorPrototype%.next, since 2026-09-21, JSeal F06): ending the iteration
            // quietly would make a buffer shrunk under a `for...of` look like one that ended.
            double length;

            if (target.AsObjectOrNull() is JsTypedArray view)
            {
                if (view.IsOutOfBounds)
                {
                    throw engine.Error(
                        "TypeError", "Cannot iterate a typed array that is detached or out of bounds");
                }

                length = view.Length;
            }
            else
            {
                // `LengthOfArrayLike`, which is `ToLength`: a fractional length is truncated, so
                // `{ length: 2.7 }` yields two elements and not three (JSeal VM-FIX-J).
                length = ArrayLengthOf(engine, target);
            }

            if (at >= length)
            {
                return (false, JsValue.Undefined);
            }

            var index = at++;

            // A KEY ITERATOR NEVER READS THE ELEMENT: the specification's step yields the index
            // alone for `keys`, so a getter at that index is not run.
            if (kind is IndexedIteratorKind.Key)
            {
                return (true, JsValue.Number(index));
            }

            var element = engine.GetIndexed(target, JsValue.Number(index));

            return (
                true,
                kind is IndexedIteratorKind.Value
                    ? element
                    : JsValue.Object(engine.Realm.NewArray([JsValue.Number(index), element])));
        });
    }

    /// <summary>Builds an iterator over a String's code points.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2EF10A
    // Broiler-Human:        PENDING
    internal JsObject CreateStringIterator(string text)
    {
        var at = 0;

        return new JsBuiltinIterator(StringIteratorPrototype, "String Iterator", engine =>
        {
            if (at >= text.Length)
            {
                return (false, JsValue.Undefined);
            }

            var width = at + 1 < text.Length &&
                char.IsHighSurrogate(text[at]) &&
                char.IsLowSurrogate(text[at + 1])
                ? 2
                : 1;

            var slice = text.Substring(at, width);
            at += width;
            engine.Charge(1);
            return (true, JsValue.String(slice));
        });
    }

    /// <summary>Builds an iterator over a list this realm already has in hand.</summary>
    /// <remarks>
    /// <para>
    /// Used by the keyed collections, whose entries are a list the object owns rather than indexed
    /// properties. The list is the live one, so an entry added while iterating is reached.
    /// </para>
    /// <para>
    /// <b>THE READER SAYS WHERE THE CURSOR LANDS, and it has to.</b> This held the cursor and
    /// stepped it by one per call, which is right only if every position holds an entry. A table
    /// with a deleted entry in it does not: its reader walks past the tombstone to the next live
    /// slot, and a cursor stepped by one then re-read the slot the reader had already answered with
    /// — so a Map with one deleted entry yielded its last entry TWICE and a `for … of` over it saw
    /// one more element than the collection's own `size`.
    /// </para>
    /// <para>
    /// <b>Exhaustion is latched.</b> The language retires the iterator when it runs out — it drops
    /// the reference to what it was iterating — so an entry appended after that is not reached, and
    /// a cursor that merely sat at the end would have reached it. The latch is now the shared
    /// <c>next</c>'s, on <see cref="JsBuiltinIterator"/>, so every kind holds it alike.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A21613
    // Broiler-Human:        PENDING
    internal JsObject CreateListIterator(
        string tag, System.Func<int, (bool Found, JsValue Value, int Next)> read)
    {
        var at = 0;

        return new JsBuiltinIterator(IteratorKindPrototype(tag), tag, engine =>
        {
            engine.Charge(1);
            (var found, var value, at) = read(at);
            return (found, value);
        });
    }

    /// <summary>Reads one argument, or <c>undefined</c> when the caller omitted it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D2AB4A
    // Broiler-Human:        PENDING
    private static JsValue SymbolArgument(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>The specification's <c>thisSymbolValue</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=031483
    // Broiler-Human:        PENDING
    private static JsSymbol SymbolOfThis(JsEngine engine, JsValue value, string method)
    {
        if (value.IsSymbol)
        {
            return value.AsSymbol();
        }

        if (value.IsObject && value.AsObject() is JsPrimitiveWrapper wrapper && wrapper.Primitive.IsSymbol)
        {
            return wrapper.Primitive.AsSymbol();
        }

        throw engine.Error(
            "TypeError", "Symbol.prototype." + method + " requires that 'this' be a Symbol");
    }
}
