// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   23
// Annotated:        23/23
// Exempt:           0
// Human-reviewed:   0/23
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       23
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
    /// <summary>Builds <c>Array</c>, its statics and <c>Array.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=745D42
    // Broiler-Human:        PENDING
    private void SetupArray()
    {
        var constructor = Constructor("Array", 1, ArrayPrototype, ArrayBuild, ArrayBuild);

        Method(constructor, "isArray", 1, (engine, thisValue, arguments) =>
            JsValue.Boolean(ArgOfArray(arguments, 0).AsObjectOrNull() is JsArray));

        Method(constructor, "of", 0, (engine, thisValue, arguments) =>
        {
            var result = NewArray();

            for (var at = 0; at < arguments.Length; at++)
            {
                engine.Charge(1);
                result.Push(arguments[at]);
            }

            return JsValue.Object(result);
        });

        // ARRAY-LIKES ONLY: `length` plus indices. Iterables are out of this profile's scope, so a
        // Set or a generator reaches this as an object with no `length` and produces an empty
        // Array rather than a wrong one.
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
            var source = ArrayReceiver(engine, items);
            var length = ArrayLengthOf(engine, source);
            var result = NewArray();

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);
                var element = ArrayGetAt(engine, source, at);

                if (mapper.IsObject)
                {
                    element = engine.Call(mapper, thisArg, [element, JsValue.Number(at)]);
                }

                result.Push(element);
            }

            return JsValue.Object(result);
        });

        SetupArrayMutators();
        SetupArrayReaders();
        SetupArrayIteration();
    }

    /// <summary><c>push</c>, <c>pop</c>, <c>shift</c>, <c>unshift</c>, <c>splice</c>, <c>reverse</c>, <c>fill</c>, <c>sort</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0AB412
    // Broiler-Human:        PENDING
    private void SetupArrayMutators()
    {
        Method(ArrayPrototype, "push", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);

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
            ArrayDeleteAt(target, last);
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
                    ArrayDeleteAt(target, at - 1);
                }
            }

            ArrayDeleteAt(target, length - 1);
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
                        ArrayDeleteAt(target, to);
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
            var removed = NewArray();
            var removedValue = JsValue.Object(removed);

            for (double at = 0; at < deleteCount; at++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, start + at))
                {
                    ArraySetAt(engine, removedValue, at, ArrayGetAt(engine, target, start + at));
                }
            }

            ArrayWriteLength(engine, removedValue, deleteCount);
            var itemCount = arguments.Length > 2 ? arguments.Length - 2 : 0;

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
                    ArrayDeleteAt(target, at - 1);
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

                if (upperExists)
                {
                    ArraySetAt(engine, target, lower, upperValue);
                }
                else
                {
                    ArrayDeleteAt(target, lower);
                }

                if (lowerExists)
                {
                    ArraySetAt(engine, target, upper, lowerValue);
                }
                else
                {
                    ArrayDeleteAt(target, upper);
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
                ArrayDeleteAt(target, at);
            }

            return target;
        });
    }

    /// <summary><c>slice</c>, <c>concat</c>, <c>join</c>, <c>toString</c> and the three searches.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=18A99B
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

            var result = NewArray();
            var resultValue = JsValue.Object(result);
            double written = 0;

            for (var at = start; at < stop; at++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, at))
                {
                    ArraySetAt(engine, resultValue, written, ArrayGetAt(engine, target, at));
                }

                written++;
            }

            ArrayWriteLength(engine, resultValue, written);
            return resultValue;
        });

        Method(ArrayPrototype, "concat", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var result = NewArray();
            var resultValue = JsValue.Object(result);
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

            // A RECEIVER WHOSE `join` IS NOT CALLABLE FALLS BACK TO Object.prototype.toString,
            // which is what makes Array.prototype.toString.call({}) answer "[object Object]"
            // rather than throwing.
            var fallback = engine.GetProperty(JsValue.Object(ObjectPrototype), "toString");
            return engine.Call(fallback, target, System.Array.Empty<JsValue>());
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=090DF4
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
            var result = NewArray();
            var resultValue = JsValue.Object(result);

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, target, at))
                {
                    ArraySetAt(
                        engine, resultValue, at, ArrayInvoke(engine, callback, thisArg, target, at));
                }
            }

            ArrayWriteLength(engine, resultValue, length);
            return resultValue;
        });

        Method(ArrayPrototype, "filter", 1, (engine, thisValue, arguments) =>
        {
            var target = ArrayReceiver(engine, thisValue);
            var length = ArrayLengthOf(engine, target);
            var callback = ArrayCallbackOf(engine, arguments, "filter");
            var thisArg = ArgOfArray(arguments, 1);
            var result = NewArray();

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
                    result.Push(element);
                }
            }

            return JsValue.Object(result);
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

    /// <summary>The receiver's <c>length</c>, as the uint32 the specification clamps it to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0D26E1
    // Broiler-Human:        PENDING
    private static double ArrayLengthOf(JsEngine engine, JsValue target) =>
        engine.ToUint32(engine.GetProperty(target, "length"));

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B5AC03
    // Broiler-Human:        PENDING
    private static void ArraySetAt(JsEngine engine, JsValue target, double at, JsValue value) =>
        engine.SetIndexed(target, JsValue.Number(at), value, false);

    /// <summary>Removes index <paramref name="at"/>, leaving a hole.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FEFCDF
    // Broiler-Human:        PENDING
    private static void ArrayDeleteAt(JsValue target, double at)
    {
        var host = target.AsObjectOrNull();

        if (host is not null)
        {
            _ = host.DeleteOwnProperty(ArrayKeyOf(at));
        }
    }

    /// <summary>Writes the receiver's <c>length</c> back.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FA6D19
    // Broiler-Human:        PENDING
    private static void ArrayWriteLength(JsEngine engine, JsValue target, double length) =>
        engine.SetProperty(target, "length", JsValue.Number(length), false);

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FA07EA
    // Broiler-Human:        PENDING
    private static void ArrayShiftOne(JsEngine engine, JsValue target, double from, double to)
    {
        if (ArrayHasAt(engine, target, from))
        {
            ArraySetAt(engine, target, to, ArrayGetAt(engine, target, from));
        }
        else
        {
            ArrayDeleteAt(target, to);
        }
    }

    /// <summary>Appends one <c>concat</c> operand, spreading it when it is an Array.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=383E99
    // Broiler-Human:        PENDING
    private static double ArrayConcatOne(
        JsEngine engine, JsValue result, double at, JsValue item)
    {
        engine.Charge(1);

        // ONLY A REAL ARRAY SPREADS. An array-like with a length is appended whole, which is what
        // makes [].concat(arguments) a one-element Array in every engine.
        if (item.AsObjectOrNull() is JsArray)
        {
            var length = ArrayLengthOf(engine, item);

            for (double index = 0; index < length; index++)
            {
                engine.Charge(1);

                if (ArrayHasAt(engine, item, index))
                {
                    ArraySetAt(engine, result, at, ArrayGetAt(engine, item, index));
                }

                at++;
            }

            return at;
        }

        ArraySetAt(engine, result, at, item);
        return at + 1;
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
