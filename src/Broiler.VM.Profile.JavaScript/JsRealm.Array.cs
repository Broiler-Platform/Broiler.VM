// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   38
// Annotated:        38/38
// Exempt:           1
// Human-reviewed:   0/38
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  4/10 max
// Unverified:       38
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>Array</c> constructor and <c>Array.prototype</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every prototype method here is generic over array-likes.</b> The specification defines them
/// over an object with a <c>length</c> and index properties rather than over the Array exotic
/// object, and guest code observes the difference the moment it writes
/// <c>Array.prototype.push.call(objectWithLength, item)</c> - a shape that jQuery, `arguments`
/// handling and every "array-like" helper in the wild depend on. So the loops go through
/// <c>GetIndexed</c>, <c>SetIndexed</c> and the <c>length</c> property and never through
/// <see cref="JsArray"/>'s dense list, and the dense list is reached only by the fast paths the
/// engine already puts behind those calls.
/// </para>
/// <para>
/// <b>A hole is not a stored <c>undefined</c>.</b> <c>forEach</c>, <c>map</c>, <c>filter</c>,
/// <c>some</c> and <c>every</c> skip an index the object has no property for; <c>find</c>,
/// <c>findIndex</c> and <c>includes</c> visit it and see <c>undefined</c>. One helper,
/// <see cref="ArrayHasAt"/>, is what the whole distinction rests on, so it is written once.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>
    /// The intrinsic <c>%Array.prototype.toString%</c>, which <c>%TypedArray%.prototype.toString</c>
    /// is the same function object as.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=985EF4
    // Broiler-Human:        PENDING
    internal JsValue IntrinsicArrayToString { get; private set; }

    /// <summary>Builds <c>Array</c>, its statics and <c>Array.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6B7E6A
    // Broiler-Human:        PENDING
    private void SetupArray()
    {
        var constructor = Constructor("Array", 1, ArrayPrototype, ArrayBuild, ArrayBuild);
        SpeciesGetter(constructor);

        Method(constructor, "isArray", 1, (engine, thisValue, arguments) =>
            JsValue.Boolean(ArrayIsArray(engine, ArgOfArray(arguments, 0).AsObjectOrNull())));

        // THE RESULT IS BUILT BY THE RECEIVER WHEN IT IS A CONSTRUCTOR, as `Array.from`'s is:
        // `Construct(C, « len »)`, then `CreateDataPropertyOrThrow` for each item and a strict `Set`
        // of `length` (ES2026 23.1.2.3). A subclass gets an instance of itself, and a receiver whose
        // result refuses a definition or the length is a `TypeError` rather than a quiet Array.
        Method(constructor, "of", 0, (engine, thisValue, arguments) =>
        {
            double length = arguments.Length;

            var result = thisValue.IsObject && thisValue.AsObject().IsConstructor
                ? engine.Construct(thisValue, [JsValue.Number(length)])
                : ArrayCreate(engine, length);

            for (var at = 0; at < arguments.Length; at++)
            {
                engine.Charge(1);
                ArrayCreateDataAt(engine, result, at, arguments[at]);
            }

            ArrayWriteLength(engine, result, length);
            return result;
        });

        // THE ITERATOR COMES FIRST AND THE ARRAY-LIKE READING IS THE FALLBACK, which is the order
        // the specification gives and the order that decides what `Array.from` of a string is.
        //
        // This comment used to say that iterables were out of this profile's scope, and while that
        // was true the fallback was the whole operation. It stopped being true when JSW-6 gave the
        // realm `Symbol` and an iteration protocol, and the difference is observable in the first
        // case anybody tries: a String is BOTH iterable and array-like, its iterator yields CODE
        // POINTS and its indices are CODE UNITS, so `Array.from("\u{1F600}")` is one element by the
        // iterator and two by the length. A Set or a Map has no `length` at all and produced an
        // empty Array - a wrong answer that looked like an empty collection.
        Method(constructor, "from", 1, (engine, thisValue, arguments) =>
        {
            var items = ArgOfArray(arguments, 0);

            if (items.IsNullish)
            {
                return engine.ThrowTypeError("Array.from requires an array-like object");
            }

            var mapper = ArgOfArray(arguments, 1);

            if (mapper.Type != JsType.Undefined &&
                (!mapper.IsObject || !mapper.AsObject().IsCallable))
            {
                return engine.ThrowTypeError("Array.from: the mapping function is not a function");
            }

            var thisArg = ArgOfArray(arguments, 2);

            // THE RESULT IS BUILT BY THE RECEIVER WHEN THE RECEIVER IS A CONSTRUCTOR, and it is
            // filled with `CreateDataPropertyOrThrow` and given its `length` with a strict `Set`.
            // A subclass of Array, or any constructor `Array.from` was borrowed onto, gets an
            // instance of itself; a result it refuses to extend is a `TypeError` rather than a
            // silently short one.
            var constructs = thisValue.IsObject && thisValue.AsObject().IsConstructor;

            if (engine.TryGetSymbolMethod(items, IteratorSymbol, out var method))
            {
                var result = constructs
                    ? engine.Construct(thisValue, System.Array.Empty<JsValue>())
                    : JsValue.Object(NewArray());

                var iterator = engine.GetIteratorFromMethod(items, method);
                double index = 0;

                while (engine.TryIterateNext(iterator, out var yielded))
                {
                    engine.Charge(1);

                    // A THROW FROM THE MAPPER OR FROM THE DEFINITION CLOSES THE ITERATOR, which
                    // is `IfAbruptCloseIterator`; one from the step itself has already marked the
                    // record done and is not closed.
                    try
                    {
                        if (index >= ArrayMaxSafeLength)
                        {
                            engine.ThrowTypeError("Array.from: the result would be too long");
                        }

                        var element = mapper.IsObject
                            ? engine.Call(mapper, thisArg, [yielded, JsValue.Number(index)])
                            : yielded;

                        ArrayCreateDataAt(engine, result, index, element);
                    }
                    catch (JsThrow)
                    {
                        engine.CloseIteratorQuietly(iterator);
                        throw;
                    }

                    index++;
                }

                ArrayWriteLength(engine, result, index);
                return result;
            }

            var source = ArrayReceiver(engine, items);
            var length = ArrayLengthOf(engine, source);

            var target = constructs
                ? engine.Construct(thisValue, [JsValue.Number(length)])
                : ArrayCreate(engine, length);

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);
                var element = ArrayGetAt(engine, source, at);

                if (mapper.IsObject)
                {
                    element = engine.Call(mapper, thisArg, [element, JsValue.Number(at)]);
                }

                ArrayCreateDataAt(engine, target, at, element);
            }

            ArrayWriteLength(engine, target, length);
            return target;
        });

        SetupArrayFromAsync(constructor);
        SetupArrayMutators();
        SetupArrayReaders();
        SetupArrayIteration();
        SetupArrayLaterAdditions();
        SetupArrayUnscopables();
    }

    /// <summary>
    /// Installs <c>Array.prototype[Symbol.unscopables]</c>, the blocklist a <c>with</c> obeys.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This list is a compatibility rule and not a nicety, and it is the reason the Symbol
    /// exists.</b> Adding <c>values</c> to <c>Array.prototype</c> broke pages that wrote
    /// <c>with (someArray) { values }</c> against an outer variable of that name, so the language
    /// made the newer members invisible to <c>with</c>. A realm without the list answers those
    /// programs with a method where they expect their own variable.
    /// </para>
    /// <para>
    /// <b>Every name the specification lists is listed, whether or not this realm has the member.</b>
    /// The list is a fixed property of <c>Array.prototype</c> and a program may read it; trimming it
    /// to what this realm implements would make <c>Object.keys(Array.prototype[Symbol.unscopables])</c>
    /// a report about this build rather than about the language.
    /// </para>
    /// <para>
    /// <b>Its prototype is null</b>, which is what the specification asks for and what stops a name
    /// like <c>toString</c> — inherited from <c>Object.prototype</c> and truthy — from being
    /// unscopable by accident.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=98F923
    // Broiler-Human:        PENDING
    private void SetupArrayUnscopables()
    {
        var blocked = new JsObject(null);

        foreach (var name in ArrayUnscopableNames)
        {
            blocked.SetOwnProperty(name, JsProperty.Data(JsValue.True, JsPropertyAttributes.Default));
        }

        ArrayPrototype.SetOwnSymbol(
            UnscopablesSymbol,
            JsProperty.Data(JsValue.Object(blocked), JsPropertyAttributes.Configurable));
    }

    /// <summary>The members of <c>Array.prototype</c> a <c>with</c> statement does not bind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FBB4CC
    // Broiler-Human:        PENDING
    private static readonly string[] ArrayUnscopableNames =
    [
        "at", "copyWithin", "entries", "fill", "find", "findIndex", "findLast", "findLastIndex",
        "flat", "flatMap", "includes", "keys", "toReversed", "toSorted", "toSpliced", "values",
    ];

    /// <summary>
    /// The prototype methods the language added after the ones above, and which real programs use
    /// as though they had always been there.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>They are here because a differential probe found them absent, not because a list was
    /// worked through.</b> Each was a <c>TypeError</c> naming a method that is not a function -
    /// which is the least useful refusal this realm can produce, because it says nothing about
    /// whether the manifest declines the surface or the realm simply has not got there yet.
    /// </para>
    /// <para>
    /// <b>The four change-by-copy methods answer a plain Array and not the receiver's species.</b>
    /// That is what the specification says for them, and it is the one place in this file where a
    /// generic method deliberately does not preserve the receiver's kind: <c>toSorted</c> called on
    /// an array-like produces an Array, because the operation's whole point is that the receiver is
    /// left alone.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7E4066
    // Broiler-Human:        PENDING
    private void SetupArrayLaterAdditions()
    {
        Method(ArrayPrototype, "at", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var relative = engine.ToInteger(ArgOfArray(arguments, 0));
            var at = relative < 0 ? length + relative : relative;

            return at < 0 || at >= length ? JsValue.Undefined : ArrayGetAt(engine, target, at);
        });

        Method(ArrayPrototype, "findLast", 1, (engine, thisValue, arguments) =>
            ArrayFindFromEnd(engine, thisValue, arguments, wantIndex: false));

        Method(ArrayPrototype, "findLastIndex", 1, (engine, thisValue, arguments) =>
            ArrayFindFromEnd(engine, thisValue, arguments, wantIndex: true));

        Method(ArrayPrototype, "flat", 0, (engine, thisValue, arguments) =>
        {
            // THE LENGTH IS READ BEFORE THE DEPTH IS COERCED, and the species is consulted after
            // both: that is the order the specification gives and the one a getter can observe.
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var stated = ArgOfArray(arguments, 0);
            var depth = stated.Type == JsType.Undefined ? 1 : engine.ToInteger(stated);
            var resultValue = ArraySpeciesCreate(engine, target, 0);

            _ = ArrayFlattenInto(
                engine, target, length, depth, resultValue, 0, JsValue.Undefined, JsValue.Undefined);

            return resultValue;
        });

        Method(ArrayPrototype, "flatMap", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var callback = ArrayCallbackOf(engine, arguments, "flatMap");
            var resultValue = ArraySpeciesCreate(engine, target, 0);

            _ = ArrayFlattenInto(
                engine, target, length, 1, resultValue, 0, callback, ArgOfArray(arguments, 1));

            return resultValue;
        });

        Method(ArrayPrototype, "copyWithin", 2, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var to = ArrayRelative(engine, ArgOfArray(arguments, 0), length);
            var from = ArrayRelative(engine, ArgOfArray(arguments, 1), length);
            var end = ArgOfArray(arguments, 2);

            var final = end.Type == JsType.Undefined
                ? length
                : ArrayRelative(engine, end, length);

            var count = System.Math.Min(final - from, length - to);

            // THE DIRECTION IS DECIDED BY THE OVERLAP AND NOT BY TASTE. Copying forwards over a
            // region that overlaps ahead of itself would read a slot this same call had already
            // written, which is the difference between a copy and a repeating fill.
            var step = 1.0;

            if (from < to && to < from + count)
            {
                step = -1;
                from += count - 1;
                to += count - 1;
            }

            while (count > 0)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, from))
                {
                    ArraySetAt(engine, target, to, ArrayGetAt(engine, target, from));
                }
                else
                {
                    ArrayDeleteAt(engine, target, to);
                }

                from += step;
                to += step;
                count--;
            }

            return target;
        });

        SetupArrayChangeByCopy();
    }

    /// <summary>
    /// <c>toSorted</c>, <c>toReversed</c>, <c>toSpliced</c> and <c>with</c>: the four that answer a
    /// new Array and leave the receiver alone.
    /// </summary>
    /// <remarks>
    /// <b>A hole becomes a stored <c>undefined</c> in all four</b>, which is the one thing about
    /// them a reader is likely to get wrong. They read every index from 0 to <c>length</c> rather
    /// than the indices the receiver has, so <c>[,1].toReversed()</c> is a dense two-element Array
    /// and not a copy of the holes. The specification is explicit about it and the reason is that a
    /// copy which preserved holes would have to be an Array exotic object built by a different
    /// path than the one that produces every other result here.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0D8B2B
    // Broiler-Human:        PENDING
    private void SetupArrayChangeByCopy()
    {
        Method(ArrayPrototype, "toReversed", 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            ArrayRefuseUncreatable(engine, length);
            var result = NewArray();

            for (double at = length - 1; at >= 0; at--)
            {
                engine.Charge(1);
                result.Push(ArrayGetAt(engine, target, at));
            }

            return JsValue.Object(result);
        });

        Method(ArrayPrototype, "toSorted", 1, (engine, thisValue, arguments) =>
        {
            var comparator = ArgOfArray(arguments, 0);

            if (comparator.Type != JsType.Undefined &&
                (!comparator.IsObject || !comparator.AsObject().IsCallable))
            {
                return engine.ThrowTypeError("Array.prototype.toSorted: the comparator is not a function");
            }

            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            ArrayRefuseUncreatable(engine, length);
            var items = new System.Collections.Generic.List<JsValue>();
            double undefinedCount = 0;

            // A HOLE AND AN `undefined` ARE THE SAME THING TO THIS ONE, unlike everywhere else in
            // this file: the copy is dense, so both become an `undefined` sorted to the end. The
            // sort itself is `sort`'s, so a comparator sees the same order from either.
            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);
                var element = ArrayGetAt(engine, target, at);

                if (element.Type == JsType.Undefined)
                {
                    undefinedCount++;
                }
                else
                {
                    items.Add(element);
                }
            }

            if (items.Count > 1)
            {
                var buffer = new System.Collections.Generic.List<JsValue>(items);
                ArrayMergeSort(engine, comparator, items, buffer, 0, items.Count);
            }

            var result = NewArray();

            foreach (var value in items)
            {
                result.Push(value);
            }

            for (double at = 0; at < undefinedCount; at++)
            {
                result.Push(JsValue.Undefined);
            }

            return JsValue.Object(result);
        });

        Method(ArrayPrototype, "with", 2, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var relative = engine.ToInteger(ArgOfArray(arguments, 0));
            var at = relative < 0 ? length + relative : relative;

            if (at < 0 || at >= length)
            {
                return engine.ThrowRangeError("Array.prototype.with: the index is out of range");
            }

            ArrayRefuseUncreatable(engine, length);
            var replacement = ArgOfArray(arguments, 1);
            var result = NewArray();

            for (double index = 0; index < length; index++)
            {
                engine.Charge(1);

                result.Push(index == at ? replacement : ArrayGetAt(engine, target, index));
            }

            return JsValue.Object(result);
        });

        Method(ArrayPrototype, "toSpliced", 2, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var start = ArrayRelative(engine, ArgOfArray(arguments, 0), length);
            var removed = ArraySpliceCount(engine, arguments, length, start);
            var inserted = arguments.Length > 2 ? arguments.Length - 2 : 0;
            var newLength = (length - removed) + inserted;
            ArrayRefuseUnsafeLength(engine, newLength, "toSpliced");
            ArrayRefuseUncreatable(engine, newLength);
            var result = NewArray();

            for (double at = 0; at < start; at++)
            {
                engine.Charge(1);
                result.Push(ArrayGetAt(engine, target, at));
            }

            for (var extra = 2; extra < arguments.Length; extra++)
            {
                result.Push(arguments[extra]);
            }

            for (var at = start + removed; at < length; at++)
            {
                engine.Charge(1);
                result.Push(ArrayGetAt(engine, target, at));
            }

            return JsValue.Object(result);
        });
    }

    /// <summary>The walk <c>findLast</c> and <c>findLastIndex</c> share.</summary>
    /// <remarks>
    /// Both visit a hole and see <c>undefined</c>, exactly as <c>find</c> and <c>findIndex</c> do,
    /// so the pair that reads from the end is the same operation with the loop reversed and not a
    /// second reading of what an absent index means.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=530C09
    // Broiler-Human:        PENDING
    private static JsValue ArrayFindFromEnd(
        JsEngine engine, JsValue thisValue, JsValue[] arguments, bool wantIndex)
    {
        var target = ArrayReceiver(engine, thisValue);
        var length = ArrayLengthOf(engine, target);
        var callback = ArrayCallbackOf(engine, arguments, wantIndex ? "findLastIndex" : "findLast");
        var thisArg = ArgOfArray(arguments, 1);

        for (var at = length - 1; at >= 0; at--)
        {
            engine.Charge(1);
            var element = ArrayGetAt(engine, target, at);
            var answered = engine.Call(callback, thisArg, [element, JsValue.Number(at), target]);

            if (answered.ToBooleanValue())
            {
                return wantIndex ? JsValue.Number(at) : element;
            }
        }

        return wantIndex ? JsValue.Number(-1) : JsValue.Undefined;
    }

    /// <summary>
    /// The flattening walk <c>flat</c> and <c>flatMap</c> share: the specification's
    /// <c>FlattenIntoArray</c>, answering the next index to write.
    /// </summary>
    /// <remarks>
    /// <b>It recurses on the CLR stack and the depth a guest can ask for is unbounded</b>, so the
    /// recursion is charged per element rather than per array: a guest handing this a
    /// deeply self-nesting structure meets the instruction budget on the way down. The alternative
    /// - an explicit worklist - would be immune to that, and would also make the mapper's argument
    /// order harder to keep right for no behaviour a program can see.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=871EB9
    // Broiler-Human:        PENDING
    private static double ArrayFlattenInto(
        JsEngine engine,
        JsValue source,
        double length,
        double depth,
        JsValue into,
        double written,
        JsValue mapper,
        JsValue thisArg)
    {
        for (double at = 0; at < length; at++)
        {
            engine.Charge(1);

            if (!ArrayHasAt(engine, source, at))
            {
                continue;
            }

            var element = ArrayGetAt(engine, source, at);

            if (mapper.IsObject)
            {
                element = engine.Call(mapper, thisArg, [element, JsValue.Number(at), source]);
            }

            // THE SAME `IsArray` THE SPREAD USES, and it looks through a Proxy for the same reason.
            if (depth > 0 && ArrayIsArray(engine, element.AsObjectOrNull()))
            {
                written = ArrayFlattenInto(
                    engine,
                    element,
                    ArrayLengthOf(engine, element),
                    depth - 1,
                    into,
                    written,
                    JsValue.Undefined,
                    JsValue.Undefined);

                continue;
            }

            if (written >= ArrayMaxSafeLength)
            {
                throw engine.Error(
                    "TypeError", "the flattened Array would exceed the maximum length");
            }

            ArrayCreateDataAt(engine, into, written, element);
            written++;
        }

        return written;
    }

    /// <summary><c>push</c>, <c>pop</c>, <c>shift</c>, <c>unshift</c>, <c>splice</c>, <c>reverse</c>, <c>fill</c>, <c>sort</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1A6693
    // Broiler-Human:        PENDING
    private void SetupArrayMutators()
    {
        Method(ArrayPrototype, "push", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            ArrayRefuseUnsafeLength(engine, length + arguments.Length, "push");

            for (var at = 0; at < arguments.Length; at++)
            {
                engine.Charge(1);
                ArraySetAt(engine, target, length, arguments[at]);
                length++;
            }

            // THE LENGTH IS WRITTEN BACK EVEN WHEN NOTHING WAS PUSHED, because the receiver may be
            // an ordinary object whose `length` is a string or a fraction, and the specification
            // says a push normalises it.
            ArrayWriteLength(engine, target, length);
            return JsValue.Number(length);
        });

        Method(ArrayPrototype, "pop", 0, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);

            if (length == 0)
            {
                ArrayWriteLength(engine, target, 0);
                return JsValue.Undefined;
            }

            var last = length - 1;
            var element = ArrayGetAt(engine, target, last);
            ArrayDeleteAt(engine, target, last);
            ArrayWriteLength(engine, target, last);
            return element;
        });

        Method(ArrayPrototype, "shift", 0, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);

            if (length == 0)
            {
                ArrayWriteLength(engine, target, 0);
                return JsValue.Undefined;
            }

            var first = ArrayGetAt(engine, target, 0);

            for (double at = 1; at < length; at++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, at))
                {
                    ArraySetAt(engine, target, at - 1, ArrayGetAt(engine, target, at));
                }
                else
                {
                    ArrayDeleteAt(engine, target, at - 1);
                }
            }

            ArrayDeleteAt(engine, target, length - 1);
            ArrayWriteLength(engine, target, length - 1);
            return first;
        });

        Method(ArrayPrototype, "unshift", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var count = arguments.Length;

            if (count > 0)
            {
                ArrayRefuseUnsafeLength(engine, length + count, "unshift");

                for (var at = length; at > 0; at--)
                {
                    engine.Charge(1);
                    var from = at - 1;
                    var to = from + count;

                    if (ArrayHasAt(engine, target, from))
                    {
                        ArraySetAt(engine, target, to, ArrayGetAt(engine, target, from));
                    }
                    else
                    {
                        ArrayDeleteAt(engine, target, to);
                    }
                }

                for (var at = 0; at < count; at++)
                {
                    engine.Charge(1);
                    ArraySetAt(engine, target, at, arguments[at]);
                }
            }

            ArrayWriteLength(engine, target, length + count);
            return JsValue.Number(length + count);
        });

        Method(ArrayPrototype, "splice", 2, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var start = ArrayRelative(engine, ArgOfArray(arguments, 0), length);
            var deleteCount = ArraySpliceCount(engine, arguments, length, start);
            var itemCount = arguments.Length > 2 ? arguments.Length - 2 : 0;
            ArrayRefuseUnsafeLength(engine, (length - deleteCount) + itemCount, "splice");

            // THE REMOVED ELEMENTS ARE COLLECTED INTO THE SPECIES BEFORE THE RECEIVER IS TOUCHED, so a
            // species result that refuses a definition throws with the receiver still intact.
            var removedValue = ArraySpeciesCreate(engine, target, deleteCount);

            for (double at = 0; at < deleteCount; at++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, start + at))
                {
                    ArrayCreateDataAt(
                        engine, removedValue, at, ArrayGetAt(engine, target, start + at));
                }
            }

            ArrayWriteLength(engine, removedValue, deleteCount);

            if (itemCount < deleteCount)
            {
                for (var at = start; at < length - deleteCount; at++)
                {
                    engine.Charge(1);
                    ArrayShiftOne(engine, target, at + deleteCount, at + itemCount);
                }

                for (var at = length; at > (length - deleteCount) + itemCount; at--)
                {
                    engine.Charge(1);
                    ArrayDeleteAt(engine, target, at - 1);
                }
            }
            else if (itemCount > deleteCount)
            {
                for (var at = length - deleteCount; at > start; at--)
                {
                    engine.Charge(1);
                    ArrayShiftOne(engine, target, (at + deleteCount) - 1, (at + itemCount) - 1);
                }
            }

            for (var at = 0; at < itemCount; at++)
            {
                engine.Charge(1);
                ArraySetAt(engine, target, start + at, arguments[at + 2]);
            }

            ArrayWriteLength(engine, target, (length - deleteCount) + itemCount);
            return removedValue;
        });

        Method(ArrayPrototype, "reverse", 0, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var middle = System.Math.Floor(length / 2);

            for (double lower = 0; lower < middle; lower++)
            {
                engine.Charge(1);
                var upper = (length - lower) - 1;
                var lowerExists = ArrayHasAt(engine, target, lower);
                var lowerValue = lowerExists ? ArrayGetAt(engine, target, lower) : JsValue.Undefined;
                var upperExists = ArrayHasAt(engine, target, upper);
                var upperValue = upperExists ? ArrayGetAt(engine, target, upper) : JsValue.Undefined;

                // TWO HOLES ARE LEFT ALONE. The specification deletes only in the mixed cases, and a
                // Proxy's `deleteProperty` trap sees the difference.
                if (!lowerExists && !upperExists)
                {
                    continue;
                }

                if (upperExists)
                {
                    ArraySetAt(engine, target, lower, upperValue);
                }
                else
                {
                    ArrayDeleteAt(engine, target, lower);
                }

                if (lowerExists)
                {
                    ArraySetAt(engine, target, upper, lowerValue);
                }
                else
                {
                    ArrayDeleteAt(engine, target, upper);
                }
            }

            return target;
        });

        Method(ArrayPrototype, "fill", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var value = ArgOfArray(arguments, 0);
            var start = ArrayRelative(engine, ArgOfArray(arguments, 1), length);
            var stop = ArgOfArray(arguments, 2).Type == JsType.Undefined
                ? length
                : ArrayRelative(engine, ArgOfArray(arguments, 2), length);

            for (var at = start; at < stop; at++)
            {
                engine.Charge(1);
                ArraySetAt(engine, target, at, value);
            }

            return target;
        });

        Method(ArrayPrototype, "sort", 1, (engine, thisValue, arguments) =>
        {
            var comparator = ArgOfArray(arguments, 0);

            if (comparator.Type != JsType.Undefined &&
                (!comparator.IsObject || !comparator.AsObject().IsCallable))
            {
                return engine.ThrowTypeError(
                    "the comparison function must be either a function or undefined");
            }

            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var items = new System.Collections.Generic.List<JsValue>();
            double undefinedCount = 0;

            // MATERIALISE FIRST. A comparator can read, write, delete and grow the receiver while
            // the sort runs, and a sort that read the receiver as it went would compare values that
            // no longer exist. Reading everything up front makes the comparator's side effects
            // visible afterwards rather than able to corrupt the ordering.
            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);

                if (!ArrayHasAt(engine, target, at))
                {
                    continue;
                }

                var element = ArrayGetAt(engine, target, at);

                if (element.Type == JsType.Undefined)
                {
                    undefinedCount++;
                }
                else
                {
                    items.Add(element);
                }
            }

            if (items.Count > 1)
            {
                var buffer = new System.Collections.Generic.List<JsValue>(items);
                ArrayMergeSort(engine, comparator, items, buffer, 0, items.Count);
            }

            double written = 0;

            for (var at = 0; at < items.Count; at++)
            {
                engine.Charge(1);
                ArraySetAt(engine, target, written, items[at]);
                written++;
            }

            for (double at = 0; at < undefinedCount; at++)
            {
                engine.Charge(1);
                ArraySetAt(engine, target, written, JsValue.Undefined);
                written++;
            }

            for (var at = written; at < length; at++)
            {
                engine.Charge(1);
                ArrayDeleteAt(engine, target, at);
            }

            return target;
        });
    }

    /// <summary><c>slice</c>, <c>concat</c>, <c>join</c>, <c>toString</c> and the three searches.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B4E0A5
    // Broiler-Human:        PENDING
    private void SetupArrayReaders()
    {
        Method(ArrayPrototype, "slice", 2, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var start = ArrayRelative(engine, ArgOfArray(arguments, 0), length);
            var stop = ArgOfArray(arguments, 1).Type == JsType.Undefined
                ? length
                : ArrayRelative(engine, ArgOfArray(arguments, 1), length);

            // THE SPECIES IS CONSULTED AFTER BOTH BOUNDS ARE COERCED, which is the order a guest sees
            // through a `valueOf` on either bound and a getter on `constructor`.
            var resultValue = ArraySpeciesCreate(engine, target, stop > start ? stop - start : 0);
            double written = 0;

            for (var at = start; at < stop; at++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, at))
                {
                    ArrayCreateDataAt(engine, resultValue, written, ArrayGetAt(engine, target, at));
                }

                written++;
            }

            ArrayWriteLength(engine, resultValue, written);
            return resultValue;
        });

        Method(ArrayPrototype, "concat", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var resultValue = ArraySpeciesCreate(engine, target, 0);
            var written = ArrayConcatOne(engine, resultValue, 0, target);

            for (var at = 0; at < arguments.Length; at++)
            {
                written = ArrayConcatOne(engine, resultValue, written, arguments[at]);
            }

            ArrayWriteLength(engine, resultValue, written);
            return resultValue;
        });

        Method(ArrayPrototype, "join", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var separatorValue = ArgOfArray(arguments, 0);
            var separator = separatorValue.Type == JsType.Undefined
                ? ","
                : engine.ToStringValue(separatorValue);

            var text = new System.Text.StringBuilder();

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);

                if (at > 0)
                {
                    text.Append(separator);
                }

                // `undefined` AND `null` RENDER AS NOTHING, which is why [1, null, 2].join() is
                // "1,,2" and not "1,null,2". A hole reads as undefined here and so renders the
                // same way, which is the one place the two are indistinguishable on purpose.
                var element = ArrayGetAt(engine, target, at);

                if (!element.IsNullish)
                {
                    text.Append(engine.ToStringValue(element));
                }
            }

            return JsValue.String(text.ToString());
        });

        Method(ArrayPrototype, "toString", 0, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var join = engine.GetProperty(target, "join");

            if (join.IsObject && join.AsObject().IsCallable)
            {
                return engine.Call(join, target, System.Array.Empty<JsValue>());
            }

            // A RECEIVER WHOSE `join` IS NOT CALLABLE FALLS BACK TO THE INTRINSIC
            // %Object.prototype.toString%, which is what makes Array.prototype.toString.call({})
            // answer "[object Object]" rather than throwing - and it is the intrinsic, not whatever
            // Object.prototype holds now, so deleting or replacing that property changes nothing.
            return engine.Call(IntrinsicObjectToString, target, System.Array.Empty<JsValue>());
        });

        _ = ArrayPrototype.TryGetOwnProperty("toString", out var installed);
        IntrinsicArrayToString = installed.Value;

        // EACH ELEMENT'S OWN `toLocaleString`, and not `join`: ECMA-262 specifies this method even
        // without ECMA-402, as `Invoke(element, "toLocaleString")` for every element that is not
        // nullish, with no arguments and `,` as the separator. Inheriting Object.prototype's, which
        // is what this prototype did before, called `join` and so each element's `toString`
        // (decision JSD-0027, follow-up N1). A non-callable method is a TypeError from `Call`.
        Method(ArrayPrototype, "toLocaleString", 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var text = new System.Text.StringBuilder();

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);

                if (at > 0)
                {
                    text.Append(',');
                }

                var element = ArrayGetAt(engine, target, at);

                if (!element.IsNullish)
                {
                    var method = engine.GetProperty(element, "toLocaleString");

                    text.Append(engine.ToStringValue(
                        engine.Call(method, element, System.Array.Empty<JsValue>())));
                }
            }

            return JsValue.String(text.ToString());
        });

        Method(ArrayPrototype, "indexOf", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);

            if (length == 0)
            {
                return JsValue.Number(-1);
            }

            var wanted = ArgOfArray(arguments, 0);
            var from = engine.ToInteger(ArgOfArray(arguments, 1));

            if (from >= length)
            {
                return JsValue.Number(-1);
            }

            var at = from >= 0 ? from : length + from;

            if (at < 0)
            {
                at = 0;
            }

            for (; at < length; at++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, at) &&
                    ArrayGetAt(engine, target, at).StrictlyEquals(wanted))
                {
                    return JsValue.Number(at);
                }
            }

            return JsValue.Number(-1);
        });

        Method(ArrayPrototype, "lastIndexOf", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);

            if (length == 0)
            {
                return JsValue.Number(-1);
            }

            var wanted = ArgOfArray(arguments, 0);
            var from = arguments.Length > 1
                ? engine.ToInteger(arguments[1])
                : length - 1;

            var at = from >= 0
                ? System.Math.Min(from, length - 1)
                : length + from;

            for (; at >= 0; at--)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, at) &&
                    ArrayGetAt(engine, target, at).StrictlyEquals(wanted))
                {
                    return JsValue.Number(at);
                }
            }

            return JsValue.Number(-1);
        });

        // SameValueZero, NOT ===. That single difference is the whole reason `includes` exists
        // beside `indexOf`: [NaN].includes(NaN) is true and [NaN].indexOf(NaN) is -1.
        Method(ArrayPrototype, "includes", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);

            if (length == 0)
            {
                return JsValue.False;
            }

            var wanted = ArgOfArray(arguments, 0);
            var from = engine.ToInteger(ArgOfArray(arguments, 1));
            var at = from >= 0 ? from : length + from;

            if (at < 0)
            {
                at = 0;
            }

            for (; at < length; at++)
            {
                engine.Charge(1);

                if (ArrayGetAt(engine, target, at).SameValueZero(wanted))
                {
                    return JsValue.True;
                }
            }

            return JsValue.False;
        });
    }

    /// <summary>The callback-taking methods.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=421761
    // Broiler-Human:        PENDING
    private void SetupArrayIteration()
    {
        Method(ArrayPrototype, "forEach", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var callback = ArrayCallbackOf(engine, arguments, "forEach");
            var thisArg = ArgOfArray(arguments, 1);

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, at))
                {
                    _ = ArrayInvoke(engine, callback, thisArg, target, at);
                }
            }

            return JsValue.Undefined;
        });

        Method(ArrayPrototype, "map", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var callback = ArrayCallbackOf(engine, arguments, "map");
            var thisArg = ArgOfArray(arguments, 1);

            // THE RESULT IS THE RECEIVER'S SPECIES, sized to the receiver's length, and its elements
            // are DEFINED rather than assigned. No `length` is written afterwards: the constructor
            // was told the length, and a species result that is not an Array has no reason to be
            // handed one it did not ask for.
            var resultValue = ArraySpeciesCreate(engine, target, length);

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, at))
                {
                    ArrayCreateDataAt(
                        engine, resultValue, at, ArrayInvoke(engine, callback, thisArg, target, at));
                }
            }

            return resultValue;
        });

        Method(ArrayPrototype, "filter", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var callback = ArrayCallbackOf(engine, arguments, "filter");
            var thisArg = ArgOfArray(arguments, 1);
            var resultValue = ArraySpeciesCreate(engine, target, 0);
            double written = 0;

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);

                if (!ArrayHasAt(engine, target, at))
                {
                    continue;
                }

                var element = ArrayGetAt(engine, target, at);
                var kept = engine.Call(
                    callback, thisArg, [element, JsValue.Number(at), target]);

                if (kept.ToBooleanValue())
                {
                    ArrayCreateDataAt(engine, resultValue, written, element);
                    written++;
                }
            }

            return resultValue;
        });

        Method(ArrayPrototype, "some", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var callback = ArrayCallbackOf(engine, arguments, "some");
            var thisArg = ArgOfArray(arguments, 1);

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, at) &&
                    ArrayInvoke(engine, callback, thisArg, target, at).ToBooleanValue())
                {
                    return JsValue.True;
                }
            }

            return JsValue.False;
        });

        Method(ArrayPrototype, "every", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var callback = ArrayCallbackOf(engine, arguments, "every");
            var thisArg = ArgOfArray(arguments, 1);

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, at) &&
                    !ArrayInvoke(engine, callback, thisArg, target, at).ToBooleanValue())
                {
                    return JsValue.False;
                }
            }

            return JsValue.True;
        });

        // `find` AND `findIndex` DO NOT SKIP HOLES. They were specified after the others and
        // deliberately visit every index, so [ , 1].find(x => x === undefined) finds the hole.
        Method(ArrayPrototype, "find", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var callback = ArrayCallbackOf(engine, arguments, "find");
            var thisArg = ArgOfArray(arguments, 1);

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);
                var element = ArrayGetAt(engine, target, at);
                var found = engine.Call(
                    callback, thisArg, [element, JsValue.Number(at), target]);

                if (found.ToBooleanValue())
                {
                    return element;
                }
            }

            return JsValue.Undefined;
        });

        Method(ArrayPrototype, "findIndex", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var callback = ArrayCallbackOf(engine, arguments, "findIndex");
            var thisArg = ArgOfArray(arguments, 1);

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);
                var element = ArrayGetAt(engine, target, at);
                var found = engine.Call(
                    callback, thisArg, [element, JsValue.Number(at), target]);

                if (found.ToBooleanValue())
                {
                    return JsValue.Number(at);
                }
            }

            return JsValue.Number(-1);
        });

        Method(ArrayPrototype, "reduce", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var callback = ArrayCallbackOf(engine, arguments, "reduce");
            double at = 0;
            JsValue accumulated;

            if (arguments.Length > 1)
            {
                accumulated = arguments[1];
            }
            else
            {
                var seeded = false;
                accumulated = JsValue.Undefined;

                while (at < length)
                {
                    engine.Charge(1);

                    if (ArrayHasAt(engine, target, at))
                    {
                        accumulated = ArrayGetAt(engine, target, at);
                        at++;
                        seeded = true;
                        break;
                    }

                    at++;
                }

                if (!seeded)
                {
                    return engine.ThrowTypeError("Reduce of empty array with no initial value");
                }
            }

            for (; at < length; at++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, at))
                {
                    accumulated = engine.Call(
                        callback,
                        JsValue.Undefined,
                        [accumulated, ArrayGetAt(engine, target, at), JsValue.Number(at), target]);
                }
            }

            return accumulated;
        });

        Method(ArrayPrototype, "reduceRight", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var callback = ArrayCallbackOf(engine, arguments, "reduceRight");
            var at = length - 1;
            JsValue accumulated;

            if (arguments.Length > 1)
            {
                accumulated = arguments[1];
            }
            else
            {
                var seeded = false;
                accumulated = JsValue.Undefined;

                while (at >= 0)
                {
                    engine.Charge(1);

                    if (ArrayHasAt(engine, target, at))
                    {
                        accumulated = ArrayGetAt(engine, target, at);
                        at--;
                        seeded = true;
                        break;
                    }

                    at--;
                }

                if (!seeded)
                {
                    return engine.ThrowTypeError("Reduce of empty array with no initial value");
                }
            }

            for (; at >= 0; at--)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, at))
                {
                    accumulated = engine.Call(
                        callback,
                        JsValue.Undefined,
                        [accumulated, ArrayGetAt(engine, target, at), JsValue.Number(at), target]);
                }
            }

            return accumulated;
        });
    }

    /// <summary>The <c>Array</c> constructor, which behaves the same called and constructed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=848D4D
    // Broiler-Human:        PENDING
    private JsValue ArrayBuild(JsEngine engine, JsValue thisValue, JsValue[] arguments)
    {
        var array = NewArray();

        // ONE NUMERIC ARGUMENT IS A LENGTH AND NOT AN ELEMENT. Array(3) is three holes and
        // Array("3") is one string, and a number that is not a uint32 is a RangeError rather than
        // a truncation - Array(-1) and Array(1.5) both throw.
        if (arguments.Length == 1 && arguments[0].IsNumber)
        {
            var requested = arguments[0].AsNumber();
            var length = JsValue.ToUint32(requested);

            if (length != requested)
            {
                return engine.ThrowRangeError("Invalid array length");
            }

            array.SetLength(length);
            return JsValue.Object(array);
        }

        for (var at = 0; at < arguments.Length; at++)
        {
            engine.Charge(1);
            array.Push(arguments[at]);
        }

        return JsValue.Object(array);
    }

    /// <summary>Reads argument <paramref name="at"/>, which may not have been supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=837A74
    // Broiler-Human:        PENDING
    private static JsValue ArgOfArray(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>The specification's <c>IsArray</c>, which looks THROUGH a Proxy.</summary>
    /// <remarks>
    /// <b>It is not a type test, and treating it as one is what made <c>Array.isArray</c> answer
    /// <c>false</c> for a proxy over an Array.</b> The predicate is about the target at the end of a
    /// chain of proxies, because a proxy over an Array must behave as an Array everywhere the
    /// language branches on this — <c>Array.prototype.concat</c>'s flattening,
    /// <c>Object.prototype.toString</c>'s tag, and <c>JSON.stringify</c>'s choice between a list and
    /// an object all ask it. A revoked proxy has no target to look through and is the one input this
    /// refuses rather than answering <c>false</c> about.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=40A5B4
    // Broiler-Human:        PENDING
    private static bool ArrayIsArray(JsEngine engine, JsObject? value)
    {
        while (value is JsProxy proxy)
        {
            engine.Charge(1);

            if (proxy.Target is null)
            {
                engine.ThrowTypeError("Array.isArray called on a revoked Proxy");
            }

            value = proxy.Target;
        }

        return value is JsArray;
    }

    /// <summary>The receiver an <c>Array.prototype</c> method operates on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=63CD90
    // Broiler-Human:        PENDING
    private static JsValue ArrayReceiver(JsEngine engine, JsValue value)
    {
        if (value.IsNullish)
        {
            return engine.ThrowTypeError(
                "Array.prototype method called on null or undefined");
        }

        return value.IsObject ? value : JsValue.Object(engine.ToObject(value));
    }

    /// <summary>
    /// The specification's <c>LengthOfArrayLike</c>: the receiver's <c>length</c> through
    /// <c>ToLength</c>, an integer in [0, 2^53-1].
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is <c>ToLength</c> and not <c>ToUint32</c></b>, and the two disagree on exactly the
    /// lengths a program can reach with an array-like: <c>-1</c> is 0 and not 4294967295, and 2^32
    /// is 2^32 and not 0. Reading the uint32 made <c>push</c> on <c>{ length: -5 }</c> write at index
    /// 4294967291 and every method treat an object of length 2^32 as empty.
    /// </para>
    /// <para>
    /// <b>A length past 2^32 does not make any loop here unmetered.</b> Every method that walks the
    /// indices charges each one it visits, so an array-like claiming 2^53-1 elements meets the
    /// instruction budget; the methods that would have to build an Array that long refuse first
    /// with the <c>RangeError</c> <c>ArrayCreate</c> owes, and the ones that would grow the receiver
    /// past 2^53-1 refuse with the <c>TypeError</c> the specification names before they write.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4138B7
    // Broiler-Human:        PENDING
    private static double ArrayLengthOf(JsEngine engine, JsValue target) =>
        ArrayToLength(engine, engine.GetProperty(target, "length"));

    /// <summary>The property key index <paramref name="at"/> is named by.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=34AD56
    // Broiler-Human:        PENDING
    private static string ArrayKeyOf(double at) =>
        at >= 0 && at < 4294967296.0 && at == System.Math.Floor(at)
            ? JsNumberFormat.ToUintString((uint)at)
            : JsNumberFormat.ToJsString(at);

    /// <summary>Whether the receiver has a property at index <paramref name="at"/>: the hole test.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E72B6F
    // Broiler-Human:        PENDING
    private static bool ArrayHasAt(JsEngine engine, JsValue target, double at)
    {
        var host = target.AsObjectOrNull();
        return host is not null && engine.HasProperty(host, ArrayKeyOf(at));
    }

    /// <summary>Reads index <paramref name="at"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=538CCD
    // Broiler-Human:        PENDING
    private static JsValue ArrayGetAt(JsEngine engine, JsValue target, double at) =>
        engine.GetIndexed(target, JsValue.Number(at));

    /// <summary>Writes index <paramref name="at"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=09CE6D
    // Broiler-Human:        PENDING
    private static void ArraySetAt(JsEngine engine, JsValue target, double at, JsValue value) =>
        engine.SetIndexed(target, JsValue.Number(at), value, true);

    /// <summary>Removes index <paramref name="at"/>, leaving a hole.</summary>
    /// <remarks>
    /// <b>It is <c>DeletePropertyOrThrow</c>, as every mutator here names it.</b> A sealed or
    /// frozen receiver, or one element made non-configurable, refuses the deletion, and the method
    /// throws there rather than going on to shorten a <c>length</c> over an element it could not
    /// remove.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=399716
    // Broiler-Human:        PENDING
    private static void ArrayDeleteAt(JsEngine engine, JsValue target, double at)
    {
        var host = target.AsObjectOrNull();

        if (host is not null && !host.DeleteOwnProperty(ArrayKeyOf(at)))
        {
            throw engine.Error("TypeError", "Cannot delete property " + ArrayKeyOf(at));
        }
    }

    /// <summary>Writes the receiver's <c>length</c> back.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=01DBE3
    // Broiler-Human:        PENDING
    /// <remarks>
    /// <b>Strict, which is the specification's own choice for these built-ins and not a policy this
    /// file adds.</b> Every mutator here performs <c>Set(O, key, value, true)</c>, so a receiver
    /// that refuses the write — a frozen Array, or one whose <c>length</c> has been closed — makes
    /// the call throw rather than answer a new length that is not the one it has.
    /// </remarks>
    private static void ArrayWriteLength(JsEngine engine, JsValue target, double length) =>
        engine.SetProperty(target, "length", JsValue.Number(length), true);

    /// <summary>The relative-index rule a negative or out-of-range bound is clamped by.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=206341
    // Broiler-Human:        PENDING
    private static double ArrayRelative(JsEngine engine, JsValue value, double length)
    {
        var relative = engine.ToInteger(value);

        if (relative < 0)
        {
            relative += length;
            return relative < 0 ? 0 : relative;
        }

        return relative > length ? length : relative;
    }

    /// <summary>How many elements a <c>splice</c> removes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=812676
    // Broiler-Human:        PENDING
    private static double ArraySpliceCount(
        JsEngine engine, JsValue[] arguments, double length, double start)
    {
        if (arguments.Length == 0)
        {
            return 0;
        }

        var available = length - start;

        if (arguments.Length == 1)
        {
            return available;
        }

        var requested = engine.ToInteger(arguments[1]);

        if (requested < 0)
        {
            return 0;
        }

        return requested > available ? available : requested;
    }

    /// <summary>Moves one element during a <c>splice</c>, propagating the hole when there is one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1199C1
    // Broiler-Human:        PENDING
    private static void ArrayShiftOne(JsEngine engine, JsValue target, double from, double to)
    {
        if (ArrayHasAt(engine, target, from))
        {
            ArraySetAt(engine, target, to, ArrayGetAt(engine, target, from));
        }
        else
        {
            ArrayDeleteAt(engine, target, to);
        }
    }

    /// <summary>Appends one <c>concat</c> operand, spreading it when it is concat-spreadable.</summary>
    /// <remarks>
    /// <b>The length a spread reads is <c>ToLength</c>, as every length here is.</b> An operand that
    /// spreads only because it said so is an arbitrary array-like, and its <c>length</c> may be
    /// negative, fractional or past 2^32; the specification clamps it to [0, 2^53-1] and refuses a
    /// result that would pass that ceiling before a single element is copied. Each index the spread visits is charged, holes included, so a huge sparse length meets
    /// the instruction budget rather than an unmetered scan.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=CC5F47
    // Broiler-Human:        PENDING
    private static double ArrayConcatOne(
        JsEngine engine, JsValue result, double at, JsValue item)
    {
        engine.Charge(1);

        // THE SYMBOL DECIDES FIRST AND `IsArray` IS ONLY THE FALLBACK. An array-like that sets
        // `Symbol.isConcatSpreadable` spreads, an Array that clears it is appended whole, and with
        // no answer either way only a real Array - looked at THROUGH a Proxy - spreads, which is
        // what keeps `[].concat(arguments)` a one-element Array.
        if (ArrayIsConcatSpreadable(engine, item))
        {
            var length = ArrayToLength(engine, engine.GetProperty(item, "length"));

            if (at + length > ArrayMaxSafeLength)
            {
                throw engine.Error(
                    "TypeError", "the concatenated Array would exceed the maximum length");
            }

            for (double index = 0; index < length; index++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, item, index))
                {
                    ArrayCreateDataAt(engine, result, at, ArrayGetAt(engine, item, index));
                }

                at++;
            }

            return at;
        }

        if (at >= ArrayMaxSafeLength)
        {
            throw engine.Error(
                "TypeError", "the concatenated Array would exceed the maximum length");
        }

        ArrayCreateDataAt(engine, result, at, item);
        return at + 1;
    }

    /// <summary>The specification's <c>IsConcatSpreadable</c>.</summary>
    /// <remarks>
    /// <b>One read of the symbol per operand, through the ordinary <c>[[Get]]</c></b>, so an
    /// inherited flag counts, a getter runs exactly once and its exception propagates, and a Proxy's
    /// <c>get</c> trap is asked. A primitive operand is never spread: <c>concat</c> does not box its
    /// arguments, so a flag on <c>String.prototype</c> does not make a string spread.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=ABA5EA
    // Broiler-Human:        PENDING
    private static bool ArrayIsConcatSpreadable(JsEngine engine, JsValue item)
    {
        if (!item.IsObject)
        {
            return false;
        }

        var spreadable = engine.GetSymbol(item, engine.Realm.IsConcatSpreadableSymbol);

        if (spreadable.Type != JsType.Undefined)
        {
            return spreadable.ToBooleanValue();
        }

        return ArrayIsArray(engine, item.AsObject());
    }

    /// <summary>The largest length an array-like may have: 2^53-1.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E932DB
    // Broiler-Human:        PENDING
    private const double ArrayMaxSafeLength = 9007199254740991.0;

    /// <summary>The specification's <c>ToLength</c>: an integer clamped to [0, 2^53-1].</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FD062F
    // Broiler-Human:        PENDING
    private static double ArrayToLength(JsEngine engine, JsValue value)
    {
        var length = engine.ToInteger(value);

        if (length <= 0)
        {
            return 0;
        }

        return length > ArrayMaxSafeLength ? ArrayMaxSafeLength : length;
    }

    /// <summary>
    /// The specification's <c>ArraySpeciesCreate</c>: the object an allocating method builds its
    /// answer in, chosen by the receiver's <c>constructor[Symbol.species]</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Only an Array receiver has a species.</b> A plain array-like - including one with a
    /// <c>constructor</c> of its own - gets an ordinary Array, and its <c>constructor</c> is never
    /// read. <c>IsArray</c> looks through a Proxy, so a proxy over an Array is asked for its
    /// <c>constructor</c> through its <c>get</c> trap.
    /// </para>
    /// <para>
    /// <b>Two reads and two defaults, then a construction.</b> An undefined <c>constructor</c>, or a
    /// species that is <c>null</c> or <c>undefined</c>, answers an ordinary Array; a
    /// <c>constructor</c> that is a primitive other than undefined, or a species that cannot be
    /// constructed, is a <c>TypeError</c>. Anything else is constructed through
    /// <see cref="JsEngine.Construct(JsValue, JsValue[])"/> with the length as its one argument, so
    /// the construction is charged, bounded by the call depth and throws what the guest threw -
    /// exactly as a <c>new</c> written in the program would.
    /// </para>
    /// <para>
    /// <b>The cross-realm step is vacuous here.</b> The specification replaces ANOTHER realm's
    /// <c>%Array%</c> with undefined; an engine in this profile hosts one realm and
    /// <c>$262.createRealm</c> refuses, so no constructor can come from another realm to be
    /// replaced. The Test262 cases that need a second realm stay failing rather than being
    /// counted as covered.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=09DE0F
    // Broiler-Human:        PENDING
    private JsValue ArraySpeciesCreate(JsEngine engine, JsValue original, double length)
    {
        // A NEGATIVE ZERO IS HANDED ON AS ZERO: `splice(0, -0)` must not show a species
        // constructor a `-0` it can tell apart with `Object.is`.
        if (length == 0)
        {
            length = 0;
        }

        if (!ArrayIsArray(engine, original.AsObjectOrNull()))
        {
            return ArrayCreate(engine, length);
        }

        var constructor = engine.GetProperty(original, "constructor");

        if (constructor.IsObject)
        {
            constructor = engine.GetSymbol(constructor, SpeciesSymbol);

            if (constructor.Type == JsType.Null)
            {
                constructor = JsValue.Undefined;
            }
        }

        if (constructor.Type == JsType.Undefined)
        {
            return ArrayCreate(engine, length);
        }

        if (!constructor.IsObject || !constructor.AsObject().IsConstructor)
        {
            return engine.ThrowTypeError("the Array species is not a constructor");
        }

        return engine.Construct(constructor, [JsValue.Number(length)]);
    }

    /// <summary>
    /// The <c>RangeError</c> <c>ArrayCreate</c> owes for a length no Array can have: past 2^32-1.
    /// </summary>
    /// <remarks>
    /// The change-by-copy methods and <c>Array.from</c> build their answer by appending rather than
    /// through <see cref="ArrayCreate"/>, and they ask this first, so an array-like claiming 2^32
    /// elements or more is refused before a single element is read rather than copied until the
    /// instruction budget runs out.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=C43651
    // Broiler-Human:        PENDING
    private static void ArrayRefuseUncreatable(JsEngine engine, double length)
    {
        if (length > uint.MaxValue)
        {
            engine.ThrowRangeError("Invalid array length");
        }
    }

    /// <summary>
    /// The <c>TypeError</c> a method owes when the length it would leave behind passes 2^53-1.
    /// </summary>
    /// <remarks>
    /// Asked before anything is written, which is the specification's order: <c>push</c>,
    /// <c>unshift</c>, <c>splice</c> and <c>toSpliced</c> refuse with the receiver untouched. The
    /// new length is computed as removal first and insertion second, so no intermediate sum passes
    /// 2^53 and rounds back under the ceiling.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F9D11C
    // Broiler-Human:        PENDING
    private static void ArrayRefuseUnsafeLength(JsEngine engine, double length, string method)
    {
        if (length > ArrayMaxSafeLength)
        {
            engine.ThrowTypeError(
                "Array.prototype." + method + ": the new length would exceed 2^53-1");
        }
    }

    /// <summary>The specification's <c>ArrayCreate</c>: a fresh Array of the given length.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A0C0EF
    // Broiler-Human:        PENDING
    private JsValue ArrayCreate(JsEngine engine, double length)
    {
        ArrayRefuseUncreatable(engine, length);
        var array = NewArray();

        if (length > 0)
        {
            array.SetLength((uint)length);
        }

        return JsValue.Object(array);
    }

    /// <summary>
    /// The specification's <c>CreateDataPropertyOrThrow</c> at index <paramref name="at"/>: the way
    /// an allocating method fills the object <see cref="ArraySpeciesCreate"/> answered.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A definition and not an assignment.</b> A species result is an arbitrary object, and an
    /// assignment would run an inherited or own setter at that index, or be swallowed by a
    /// read-only one; a definition does neither. It replaces a configurable property of either kind
    /// with a plain writable, enumerable, configurable data property.
    /// </para>
    /// <para>
    /// <b>The definition is the RESULT's own <c>[[DefineOwnProperty]]</c></b>, the one
    /// <c>Object.defineProperty</c> reaches, so an exotic result validates it the way the language
    /// says: a typed array too short for the index refuses it, a String object refuses its own
    /// characters, and a Proxy is asked through its <c>defineProperty</c> trap with the complete
    /// four-field descriptor. Each refusal is a <c>TypeError</c>, where the store underneath would
    /// have ignored the write silently. Writing the store directly, as this used to, skipped every
    /// exotic object's own rules.
    /// </para>
    /// <para>
    /// <b>A plain Array keeps a direct path because it is observably the same.</b> Its refusals are
    /// the ordinary object's - a non-configurable property already there, a non-extensible Array
    /// without one - and the Array's own: an ARRAY INDEX at or past a length that has been closed.
    /// A key of 2^32-1 or more is not an array index and cannot move the length, so it is not
    /// refused for that reason.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0D928E
    // Broiler-Human:        PENDING
    private static void ArrayCreateDataAt(JsEngine engine, JsValue target, double at, JsValue value)
    {
        var host = target.AsObject();
        var key = ArrayKeyOf(at);

        if (host is not JsArray array)
        {
            var fields = new ObjectDescriptorFields
            {
                HasValue = true,
                Value = value,
                HasWritable = true,
                Writable = true,
                HasEnumerable = true,
                Enumerable = true,
                HasConfigurable = true,
                Configurable = true,
            };

            ObjectApplyDescriptorAt(engine, host, JsValue.String(key), fields);
            return;
        }

        var refused = array.TryGetOwnProperty(key, out var current)
            ? !current.Configurable
            : !array.Extensible;

        if (!refused && !array.LengthWritable &&
            JsObject.IsArrayIndex(key, out var index) && index >= array.Length)
        {
            refused = true;
        }

        if (refused)
        {
            engine.ThrowTypeError("Cannot define property " + key + " on the Array method's result");
        }

        array.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>The callback an iteration method was given, which has to be callable.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=98C83A
    // Broiler-Human:        PENDING
    private static JsValue ArrayCallbackOf(JsEngine engine, JsValue[] arguments, string method)
    {
        var callback = ArgOfArray(arguments, 0);

        if (!callback.IsObject || !callback.AsObject().IsCallable)
        {
            return engine.ThrowTypeError(
                "Array.prototype." + method + " requires a callback function");
        }

        return callback;
    }

    /// <summary>Calls a callback with the specification's <c>(element, index, object)</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1F8A59
    // Broiler-Human:        PENDING
    private static JsValue ArrayInvoke(
        JsEngine engine, JsValue callback, JsValue thisArg, JsValue target, double at) =>
        engine.Call(
            callback, thisArg, [ArrayGetAt(engine, target, at), JsValue.Number(at), target]);

    /// <summary>One comparison, by the comparator when there is one and by ToString when there is not.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F0E9DE
    // Broiler-Human:        PENDING
    private static int ArrayCompareValues(
        JsEngine engine, JsValue comparator, JsValue left, JsValue right)
    {
        engine.Charge(1);

        if (comparator.IsObject)
        {
            var ordering = engine.ToNumber(engine.Call(comparator, JsValue.Undefined, [left, right]));

            if (double.IsNaN(ordering) || ordering == 0)
            {
                return 0;
            }

            return ordering < 0 ? -1 : 1;
        }

        // THE DEFAULT ORDER IS OVER STRINGS AND NOT OVER NUMBERS, which is why [1, 10, 2].sort()
        // is [1, 10, 2]. Reproducing that is the point; "fixing" it would be a different language.
        var difference = string.CompareOrdinal(
            engine.ToStringValue(left), engine.ToStringValue(right));

        if (difference == 0)
        {
            return 0;
        }

        return difference < 0 ? -1 : 1;
    }

    /// <summary>A stable merge sort of <paramref name="items"/> over the half-open range.</summary>
    /// <remarks>
    /// Stable because the merge takes from the left run on a tie, and stability is observable:
    /// sorting records by one field has to leave equal records in the order they were in, and
    /// <c>List&lt;T&gt;.Sort</c> - an introsort - does not promise that.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=449AD0
    // Broiler-Human:        PENDING
    private static void ArrayMergeSort(
        JsEngine engine,
        JsValue comparator,
        System.Collections.Generic.List<JsValue> items,
        System.Collections.Generic.List<JsValue> buffer,
        int from,
        int to)
    {
        if (to - from < 2)
        {
            return;
        }

        var middle = from + ((to - from) / 2);
        ArrayMergeSort(engine, comparator, items, buffer, from, middle);
        ArrayMergeSort(engine, comparator, items, buffer, middle, to);

        var left = from;
        var right = middle;
        var written = from;

        while (left < middle && right < to)
        {
            buffer[written++] = ArrayCompareValues(engine, comparator, items[left], items[right]) <= 0
                ? items[left++]
                : items[right++];
        }

        while (left < middle)
        {
            buffer[written++] = items[left++];
        }

        while (right < to)
        {
            buffer[written++] = items[right++];
        }

        for (var at = from; at < to; at++)
        {
            items[at] = buffer[at];
        }
    }
}
