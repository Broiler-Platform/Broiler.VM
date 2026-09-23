// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           9
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The iterator helpers on <c>%Iterator.prototype%</c>: the lazy ones, which answer an Iterator
/// Helper object, the terminal ones, which drain, and <c>Iterator.concat</c>, which is lazy too.
/// </summary>
/// <remarks>
/// <para>
/// <b>A lazy helper is a generator in the specification and a state machine here.</b> Its closure
/// has one suspension point - the <c>Yield</c> - so everything the closure keeps between two calls
/// of <c>next</c> is a handful of locals: a counter, a remaining limit, the inner iterator of a
/// <c>flatMap</c>. Each helper captures those in the step it hands <see cref="JsIteratorHelper"/>,
/// and the helper object carries the generator state the language makes observable:
/// <c>suspended-start</c>, <c>suspended-yield</c>, <c>executing</c> and <c>completed</c>. Nothing is
/// stepped before the first <c>next</c>, nothing is buffered, and an unbounded source is consumed
/// one element per call.
/// </para>
/// <para>
/// <b>Closing is the other half of the protocol and is where the precedence lives.</b> A callback
/// that throws closes the underlying iterator and the callback's exception wins over anything
/// <c>return</c> does (<see cref="JsEngine.CloseIteratorQuietly"/>). A short circuit - <c>some</c>
/// finding its value, <c>take</c> reaching its limit, a helper's own <c>return</c> - closes through
/// <see cref="JsEngine.CloseIterator"/>, whose failures DO propagate. An underlying <c>next</c> that
/// throws, or whose result is malformed, closes nothing: an iterator that failed is not asked to
/// clean up.
/// </para>
/// <para>
/// <b>Every step is metered.</b> Each call into the underlying iterator is charged by the engine's
/// own call path and its iteration step; <c>drop</c>'s skipping loop and <c>toArray</c>'s drain are
/// host loops, but they run one guest call per turn, so the fuel meter and cancellation stop them
/// the way they stop a guest <c>for</c> loop.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%IteratorHelperPrototype%</c>: where a lazy helper's <c>next</c> and <c>return</c> live.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A38C84
    // Broiler-Human:        PENDING
    internal JsObject IteratorHelperPrototype { get; private set; } = null!;

    /// <summary>Builds <c>%IteratorHelperPrototype%</c> and the eleven helper methods.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=C34395
    // Broiler-Human:        PENDING
    private void SetupIteratorHelpers()
    {
        IteratorHelperPrototype = new JsObject(IteratorPrototype);

        Method(IteratorHelperPrototype, "next", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return engine.Realm.IteratorHelperNext(engine, thisValue);
        });

        Method(IteratorHelperPrototype, "return", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return engine.Realm.IteratorHelperReturn(engine, thisValue);
        });

        IteratorHelperPrototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Iterator Helper"), JsPropertyAttributes.Configurable));

        SetupLazyHelpers();
        SetupTerminalHelpers();
    }

    /// <summary>Installs <c>map</c>, <c>filter</c>, <c>take</c>, <c>drop</c> and <c>flatMap</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DC2BC9
    // Broiler-Human:        PENDING
    private void SetupLazyHelpers()
    {
        Method(IteratorPrototype, "map", 1, static (engine, thisValue, arguments) =>
        {
            var mapper = IteratorArgument(arguments, 0);
            var iterated = HelperReceiver(engine, thisValue, mapper, "map");
            var counter = 0d;

            return JsValue.Object(engine.Realm.NewIteratorHelper(iterated, step =>
            {
                if (!step.TryIterateNext(iterated, out var value))
                {
                    return (false, JsValue.Undefined);
                }

                var mapped = CallClosing(step, iterated, mapper, [value, JsValue.Number(counter)]);
                counter++;
                return (true, mapped);
            }));
        });

        Method(IteratorPrototype, "filter", 1, static (engine, thisValue, arguments) =>
        {
            var predicate = IteratorArgument(arguments, 0);
            var iterated = HelperReceiver(engine, thisValue, predicate, "filter");
            var counter = 0d;

            return JsValue.Object(engine.Realm.NewIteratorHelper(iterated, step =>
            {
                while (step.TryIterateNext(iterated, out var value))
                {
                    var selected = CallClosing(
                        step, iterated, predicate, [value, JsValue.Number(counter)]);

                    counter++;

                    if (selected.ToBooleanValue())
                    {
                        return (true, value);
                    }
                }

                return (false, JsValue.Undefined);
            }));
        });

        Method(IteratorPrototype, "take", 1, static (engine, thisValue, arguments) =>
        {
            var remaining = HelperLimit(engine, thisValue, IteratorArgument(arguments, 0), "take");
            var iterated = GetIteratorDirect(engine, thisValue);

            return JsValue.Object(engine.Realm.NewIteratorHelper(iterated, step =>
            {
                // THE LIMIT REACHED IS A CLOSE, NOT JUST AN END. The underlying iterator has more
                // to give and is told it will not be asked, before this helper reports done - and
                // without a further `next`, which is what distinguishes it from running dry.
                if (remaining == 0)
                {
                    step.CloseIterator(iterated);
                    return (false, JsValue.Undefined);
                }

                if (!double.IsPositiveInfinity(remaining))
                {
                    remaining--;
                }

                return step.TryIterateNext(iterated, out var value)
                    ? (true, value)
                    : (false, JsValue.Undefined);
            }));
        });

        Method(IteratorPrototype, "drop", 1, static (engine, thisValue, arguments) =>
        {
            var remaining = HelperLimit(engine, thisValue, IteratorArgument(arguments, 0), "drop");
            var iterated = GetIteratorDirect(engine, thisValue);

            return JsValue.Object(engine.Realm.NewIteratorHelper(iterated, step =>
            {
                // THE SKIPPED ELEMENTS ARE STEPPED AND NOT READ: `IteratorStep` asks for `done` and
                // never for `value`, which a getter on the result can see. The loop is one guest
                // call per turn, so a skip of `Infinity` is bounded by the fuel meter.
                while (remaining > 0)
                {
                    if (!double.IsPositiveInfinity(remaining))
                    {
                        remaining--;
                    }

                    if (!IteratorStepOnly(step, iterated))
                    {
                        return (false, JsValue.Undefined);
                    }
                }

                return step.TryIterateNext(iterated, out var value)
                    ? (true, value)
                    : (false, JsValue.Undefined);
            }));
        });

        Method(IteratorPrototype, "flatMap", 1, static (engine, thisValue, arguments) =>
        {
            var mapper = IteratorArgument(arguments, 0);
            var iterated = HelperReceiver(engine, thisValue, mapper, "flatMap");
            var counter = 0d;
            JsIteratorRecord? inner = null;

            var helper = engine.Realm.NewIteratorHelper(iterated, step =>
            {
                while (true)
                {
                    if (inner is not null)
                    {
                        bool found;
                        JsValue value;

                        // AN INNER STEP THAT FAILS CLOSES THE OUTER ITERATOR, and not the inner
                        // one: the inner iterator is the one that failed.
                        try
                        {
                            found = step.TryIterateNext(inner, out value);
                        }
                        catch (JsThrow)
                        {
                            step.CloseIteratorQuietly(iterated);
                            throw;
                        }

                        if (found)
                        {
                            return (true, value);
                        }

                        inner = null;
                        counter++;
                    }

                    if (!step.TryIterateNext(iterated, out var outer))
                    {
                        return (false, JsValue.Undefined);
                    }

                    var mapped = CallClosing(step, iterated, mapper, [outer, JsValue.Number(counter)]);

                    try
                    {
                        inner = step.Realm.GetIteratorFlattenable(step, mapped, iterateStrings: false);
                    }
                    catch (JsThrow)
                    {
                        step.CloseIteratorQuietly(iterated);
                        throw;
                    }
                }
            });

            // A `return` WHILE SUSPENDED INSIDE AN INNER ITERATOR CLOSES BOTH, INNER FIRST. When
            // the inner close fails, its exception is the one reported and the outer iterator is
            // still closed, quietly; otherwise the outer close runs as an ordinary close whose own
            // failure propagates.
            helper.Abort = step =>
            {
                if (inner is not null)
                {
                    try
                    {
                        step.CloseIterator(inner);
                    }
                    catch (JsThrow)
                    {
                        step.CloseIteratorQuietly(iterated);
                        throw;
                    }
                }

                step.CloseIterator(iterated);
            };

            return JsValue.Object(helper);
        });
    }

    /// <summary>
    /// Installs <c>reduce</c>, <c>toArray</c>, <c>forEach</c>, <c>some</c>, <c>every</c> and
    /// <c>find</c>.
    /// </summary>
    /// <remarks>
    /// <b>They drain in place and allocate no helper.</b> <c>toArray</c> is the only one that keeps
    /// what it reads; it grows its Array one element per metered step, so a source larger than the
    /// allowance ends with the exhaustion the realm reports for any other runaway loop.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=341C4E
    // Broiler-Human:        PENDING
    private void SetupTerminalHelpers()
    {
        Method(IteratorPrototype, "reduce", 1, static (engine, thisValue, arguments) =>
        {
            var reducer = IteratorArgument(arguments, 0);
            var iterated = HelperReceiver(engine, thisValue, reducer, "reduce");
            JsValue accumulator;
            var counter = 0d;

            // "NOT PRESENT" IS AN ARGUMENT COUNT AND NOT A VALUE: `reduce(f, undefined)` starts
            // from `undefined`, while `reduce(f)` starts from the first element and refuses an
            // empty source.
            if (arguments.Length < 2)
            {
                if (!engine.TryIterateNext(iterated, out accumulator))
                {
                    return engine.ThrowTypeError("Reduce of empty iterator with no initial value");
                }

                counter = 1;
            }
            else
            {
                accumulator = arguments[1];
            }

            while (engine.TryIterateNext(iterated, out var value))
            {
                accumulator = CallClosing(
                    engine, iterated, reducer, [accumulator, value, JsValue.Number(counter)]);

                counter++;
            }

            return accumulator;
        });

        Method(IteratorPrototype, "toArray", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;

            if (!thisValue.IsObject)
            {
                return engine.ThrowTypeError("Iterator.prototype.toArray called on a value that is not an object");
            }

            var iterated = GetIteratorDirect(engine, thisValue);
            var items = engine.Realm.NewArray();

            while (engine.TryIterateNext(iterated, out var value))
            {
                engine.Charge(1);
                items.Push(value);
            }

            return JsValue.Object(items);
        });

        Method(IteratorPrototype, "forEach", 1, static (engine, thisValue, arguments) =>
        {
            var procedure = IteratorArgument(arguments, 0);
            var iterated = HelperReceiver(engine, thisValue, procedure, "forEach");
            var counter = 0d;

            while (engine.TryIterateNext(iterated, out var value))
            {
                CallClosing(engine, iterated, procedure, [value, JsValue.Number(counter)]);
                counter++;
            }

            return JsValue.Undefined;
        });

        Method(IteratorPrototype, "some", 1, static (engine, thisValue, arguments) =>
            ShortCircuit(engine, thisValue, IteratorArgument(arguments, 0), "some", stopWhen: true) is { } _
                ? JsValue.True
                : JsValue.False);

        Method(IteratorPrototype, "every", 1, static (engine, thisValue, arguments) =>
            ShortCircuit(engine, thisValue, IteratorArgument(arguments, 0), "every", stopWhen: false) is { } _
                ? JsValue.False
                : JsValue.True);

        Method(IteratorPrototype, "find", 1, static (engine, thisValue, arguments) =>
            ShortCircuit(engine, thisValue, IteratorArgument(arguments, 0), "find", stopWhen: true) is { } found
                ? found
                : JsValue.Undefined);
    }

    /// <summary>Installs <c>Iterator.concat</c>.</summary>
    /// <remarks>
    /// <b>Every argument is validated before anything is opened</b>: each must be an object with a
    /// callable <c>[Symbol.iterator]</c>, read once, here. The iterables are then opened one at a
    /// time, lazily, when the previous one is exhausted, and a <c>return</c> closes only the one
    /// that is open.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F0BE3E
    // Broiler-Human:        PENDING
    private void SetupIteratorConcat() =>
        Method(IteratorConstructor, "concat", 0, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var iterables = new (JsValue Method, JsValue Iterable)[arguments.Length];

            for (var at = 0; at < arguments.Length; at++)
            {
                engine.Charge(1);
                var item = arguments[at];

                if (!item.IsObject)
                {
                    return engine.ThrowTypeError("Iterator.concat requires iterable objects");
                }

                if (!engine.TryGetSymbolMethod(item, engine.Realm.IteratorSymbol, out var method))
                {
                    return engine.ThrowTypeError(engine.Describe(item) + " is not iterable");
                }

                iterables[at] = (method, item);
            }

            var next = 0;
            JsIteratorRecord? current = null;

            var helper = engine.Realm.NewIteratorHelper(null, step =>
            {
                while (true)
                {
                    if (current is null)
                    {
                        if (next >= iterables.Length)
                        {
                            return (false, JsValue.Undefined);
                        }

                        var (open, iterable) = iterables[next];
                        var iterator = step.Call(open, iterable, System.Array.Empty<JsValue>());

                        if (!iterator.IsObject)
                        {
                            step.ThrowTypeError("The result of the iterator method is not an object");
                        }

                        current = GetIteratorDirect(step, iterator);
                    }

                    if (step.TryIterateNext(current, out var value))
                    {
                        return (true, value);
                    }

                    current = null;
                    next++;
                }
            });

            helper.Abort = step =>
            {
                if (current is not null)
                {
                    step.CloseIterator(current);
                }
            };

            return JsValue.Object(helper);
        });

    /// <summary>Makes a lazy helper over <paramref name="iterated"/>, closing it on <c>return</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F56E42
    // Broiler-Human:        PENDING
    private JsIteratorHelper NewIteratorHelper(
        JsIteratorRecord? iterated, System.Func<JsEngine, (bool Found, JsValue Value)> step)
    {
        var helper = new JsIteratorHelper(IteratorHelperPrototype, iterated, step);

        if (iterated is not null)
        {
            helper.Abort = engine => engine.CloseIterator(iterated);
        }

        return helper;
    }

    /// <summary><c>%IteratorHelperPrototype%.next</c>: <c>GeneratorResume</c> with the helper brand.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=044DD9
    // Broiler-Human:        PENDING
    private JsValue IteratorHelperNext(JsEngine engine, JsValue thisValue)
    {
        var helper = HelperOf(engine, thisValue, "next");

        if (helper.State == JsIteratorHelperState.Executing)
        {
            return engine.ThrowTypeError("Iterator Helper is already running");
        }

        if (helper.State == JsIteratorHelperState.Completed)
        {
            return JsValue.Object(IteratorResult(JsValue.Undefined, done: true));
        }

        engine.Charge(1);
        helper.State = JsIteratorHelperState.Executing;
        (bool Found, JsValue Value) answer;

        try
        {
            answer = helper.Step(engine);
        }
        catch
        {
            helper.Complete();
            throw;
        }

        if (!answer.Found)
        {
            helper.Complete();
            return JsValue.Object(IteratorResult(JsValue.Undefined, done: true));
        }

        helper.State = JsIteratorHelperState.SuspendedYield;
        return JsValue.Object(IteratorResult(answer.Value, done: false));
    }

    /// <summary><c>%IteratorHelperPrototype%.return</c>.</summary>
    /// <remarks>
    /// <b>Before the first <c>next</c> there is no suspended closure to resume</b>, so the helper is
    /// completed first and the underlying iterator closed after - the order the specification gives,
    /// which a re-entrant <c>return</c> from inside that close can observe as an already-finished
    /// helper. After a <c>next</c> the closure is resumed with a return completion: the helper is
    /// <c>executing</c> while the underlying iterators close, and completed afterwards whatever they
    /// did.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=665F4A
    // Broiler-Human:        PENDING
    private JsValue IteratorHelperReturn(JsEngine engine, JsValue thisValue)
    {
        var helper = HelperOf(engine, thisValue, "return");

        if (helper.State == JsIteratorHelperState.SuspendedStart)
        {
            helper.Complete();

            if (helper.Underlying is { } underlying)
            {
                engine.CloseIterator(underlying);
            }

            return JsValue.Object(IteratorResult(JsValue.Undefined, done: true));
        }

        if (helper.State == JsIteratorHelperState.Executing)
        {
            return engine.ThrowTypeError("Iterator Helper is already running");
        }

        if (helper.State == JsIteratorHelperState.Completed)
        {
            return JsValue.Object(IteratorResult(JsValue.Undefined, done: true));
        }

        engine.Charge(1);
        helper.State = JsIteratorHelperState.Executing;

        try
        {
            helper.Abort?.Invoke(engine);
        }
        finally
        {
            helper.Complete();
        }

        return JsValue.Object(IteratorResult(JsValue.Undefined, done: true));
    }

    /// <summary>The helper behind a receiver, or the <c>TypeError</c> a wrong brand is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=164A74
    // Broiler-Human:        PENDING
    private static JsIteratorHelper HelperOf(JsEngine engine, JsValue thisValue, string method) =>
        thisValue.AsObjectOrNull() as JsIteratorHelper ??
        throw engine.Error(
            "TypeError", "%IteratorHelperPrototype%." + method + " called on an incompatible receiver");

    /// <summary>
    /// The receiver checks every callback-taking helper opens with, in the specification's order.
    /// </summary>
    /// <remarks>
    /// <b>The callback is checked BEFORE <c>next</c> is read</b>, and a callback that is not callable
    /// closes the receiver - which has not yet been asked for anything - so that a helper call that
    /// fails at the call site still releases what it was handed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=10B771
    // Broiler-Human:        PENDING
    private static JsIteratorRecord HelperReceiver(
        JsEngine engine, JsValue thisValue, JsValue callback, string method)
    {
        if (!thisValue.IsObject)
        {
            engine.ThrowTypeError(
                "Iterator.prototype." + method + " called on a value that is not an object");
        }

        if (!callback.IsObject || !callback.AsObject().IsCallable)
        {
            var error = engine.Error("TypeError", engine.Describe(callback) + " is not a function");
            engine.CloseIteratorQuietly(new JsIteratorRecord(thisValue, JsValue.Undefined));
            throw error;
        }

        return GetIteratorDirect(engine, thisValue);
    }

    /// <summary>
    /// <c>take</c>'s and <c>drop</c>'s limit: <c>ToNumber</c>, then <c>NaN</c> and negatives refused,
    /// each refusal closing the receiver.
    /// </summary>
    /// <remarks>
    /// <b>The limit is converted BEFORE <c>next</c> is read</b>, so a <c>valueOf</c> on the limit
    /// runs first, and a limit that throws closes the receiver with its own exception. Fractions
    /// truncate toward zero, so <c>-0.5</c> is a limit of zero rather than a negative one, and
    /// <c>Infinity</c> is kept as itself: an unbounded take and a drop of everything.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B54229
    // Broiler-Human:        PENDING
    private static double HelperLimit(JsEngine engine, JsValue thisValue, JsValue limit, string method)
    {
        if (!thisValue.IsObject)
        {
            engine.ThrowTypeError(
                "Iterator.prototype." + method + " called on a value that is not an object");
        }

        var receiver = new JsIteratorRecord(thisValue, JsValue.Undefined);
        double number;

        try
        {
            number = engine.ToNumber(limit);
        }
        catch (JsThrow)
        {
            engine.CloseIteratorQuietly(receiver);
            throw;
        }

        if (double.IsNaN(number))
        {
            var error = engine.Error("RangeError", method + " must be given a number, not NaN");
            engine.CloseIteratorQuietly(receiver);
            throw error;
        }

        var integer = JsValue.ToInteger(number);

        if (integer < 0)
        {
            var error = engine.Error("RangeError", method + " must be given a limit that is not negative");
            engine.CloseIteratorQuietly(receiver);
            throw error;
        }

        // `+ 0` folds a negative zero into zero, which is what `ToIntegerOrInfinity` answers.
        return integer + 0d;
    }

    /// <summary>
    /// <c>some</c>, <c>every</c> and <c>find</c>: the value at which the predicate first answers
    /// <paramref name="stopWhen"/>, having closed the iterator there, or <see langword="null"/> when
    /// the source ran dry first.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0EA30E
    // Broiler-Human:        PENDING
    private static JsValue? ShortCircuit(
        JsEngine engine, JsValue thisValue, JsValue predicate, string method, bool stopWhen)
    {
        var iterated = HelperReceiver(engine, thisValue, predicate, method);
        var counter = 0d;

        while (engine.TryIterateNext(iterated, out var value))
        {
            var answer = CallClosing(engine, iterated, predicate, [value, JsValue.Number(counter)]);

            if (answer.ToBooleanValue() == stopWhen)
            {
                // A SHORT CIRCUIT IS A NORMAL COMPLETION, so the close's own failure - a `return`
                // that throws, or answers a non-object - is what the call reports.
                engine.CloseIterator(iterated);
                return value;
            }

            counter++;
        }

        return null;
    }

    /// <summary>
    /// Calls a helper's callback; when it throws, closes <paramref name="iterated"/> quietly and
    /// lets the callback's exception through.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7B840F
    // Broiler-Human:        PENDING
    private static JsValue CallClosing(
        JsEngine engine, JsIteratorRecord iterated, JsValue callback, JsValue[] arguments)
    {
        try
        {
            return engine.Call(callback, JsValue.Undefined, arguments);
        }
        catch (JsThrow)
        {
            engine.CloseIteratorQuietly(iterated);
            throw;
        }
    }

    /// <summary>
    /// <c>IteratorStep</c> without the value: calls <c>next</c> and reads <c>done</c>, and nothing
    /// else.
    /// </summary>
    /// <remarks>
    /// Any failure marks the record done, so that nothing later asks a failed iterator to close.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=15729A
    // Broiler-Human:        PENDING
    private static bool IteratorStepOnly(JsEngine engine, JsIteratorRecord record)
    {
        if (record.Done)
        {
            return false;
        }

        engine.Charge(2);

        try
        {
            var result = engine.Call(record.Next, record.Iterator, System.Array.Empty<JsValue>());

            if (!result.IsObject)
            {
                engine.ThrowTypeError("Iterator result " + engine.Describe(result) + " is not an object");
            }

            if (engine.GetProperty(result, "done").ToBooleanValue())
            {
                record.Done = true;
                return false;
            }

            return true;
        }
        catch (JsThrow)
        {
            record.Done = true;
            throw;
        }
    }
}

/// <summary>The four generator states an Iterator Helper passes through.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E76F44
// Broiler-Human:        PENDING
internal enum JsIteratorHelperState
{
    /// <summary>Made, and not yet asked for anything.</summary>
    SuspendedStart = 0,

    /// <summary>Has answered at least one value and is waiting to be asked again.</summary>
    SuspendedYield = 1,

    /// <summary>Inside a step; a <c>next</c> or <c>return</c> now is a re-entry.</summary>
    Executing = 2,

    /// <summary>Finished, by exhaustion, failure or <c>return</c>, and never resumed again.</summary>
    Completed = 3,
}

/// <summary>An Iterator Helper object: a lazy helper's state, its step and its close.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FD027E
// Broiler-Human:        PENDING
internal sealed class JsIteratorHelper : JsObject
{
    /// <summary>Creates a helper in <c>suspended-start</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A56B9C
    // Broiler-Human:        PENDING
    internal JsIteratorHelper(
        JsObject prototype,
        JsIteratorRecord? underlying,
        System.Func<JsEngine, (bool Found, JsValue Value)> step)
        : base(prototype, "Iterator Helper")
    {
        Underlying = underlying;
        Step = step;
    }

    /// <summary>The generator state.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3F49CD
    // Broiler-Human:        PENDING
    internal JsIteratorHelperState State { get; set; }

    /// <summary>
    /// The <c>[[UnderlyingIterators]]</c> slot's one entry, or <see langword="null"/> for
    /// <c>Iterator.concat</c>, whose list is empty.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=8240FD
    // Broiler-Human:        PENDING
    internal JsIteratorRecord? Underlying { get; }

    /// <summary>Resumes the closure normally: the next value, or that it has finished.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B082C4
    // Broiler-Human:        PENDING
    internal System.Func<JsEngine, (bool Found, JsValue Value)> Step { get; private set; }

    /// <summary>Resumes the closure with a return completion from <c>suspended-yield</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=14B471
    // Broiler-Human:        PENDING
    internal System.Action<JsEngine>? Abort { get; set; }

    /// <summary>Enters <c>completed</c> and lets go of everything the closure held.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F00910
    // Broiler-Human:        PENDING
    internal void Complete()
    {
        State = JsIteratorHelperState.Completed;
        Step = static _ => (false, JsValue.Undefined);
        Abort = null;
    }
}
