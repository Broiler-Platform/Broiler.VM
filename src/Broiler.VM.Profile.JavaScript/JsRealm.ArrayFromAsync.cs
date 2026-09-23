// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           9
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  4/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Array.fromAsync</c>: the specification's built-in ASYNC function, written as a chain of
/// continuations over the realm's own <c>Await</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Nothing here waits.</b> The specification writes the method as one async closure with an
/// <c>Await</c> at every element; this file writes each stretch between two awaits as a method and
/// hands the next one to <see cref="AwaitOn"/> as the continuation. So every element costs at least
/// one turn of the job queue exactly as the language says, a host that never drains sees a pending
/// promise and nothing else, and no CLR thread ever blocks on a guest promise. The chain is not a
/// recursion either: each continuation runs in its own job, so a million-element input is a
/// million jobs, each charged, and never a deep stack.
/// </para>
/// <para>
/// <b>Every failure REJECTS, including the ones found before the first <c>await</c>.</b> A
/// non-callable mapper, a nullish input, a non-callable <c>Symbol.asyncIterator</c> and a
/// throwing <c>length</c> getter all happen synchronously inside the call, and all of them become
/// the answer's rejection rather than an exception at the call site - which is what an async
/// function does with its body's throw, and what lets a caller write <c>.catch</c> alone.
/// </para>
/// <para>
/// <b>An abandoned iterator is closed only where the specification says so.</b> A mapper that
/// throws or rejects, and a result object that refuses an element, owe the iterator its
/// <c>return</c> - awaited before the rejection, as <c>AsyncIteratorClose</c> requires. A
/// <c>next</c> that throws or rejects, a step that is not an object and a throwing <c>done</c> or
/// <c>value</c> do NOT: the iterator itself failed and is not asked to clean up after that. The
/// array-like path has no iterator and closes nothing.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>Installs <c>Array.fromAsync</c> on the <c>Array</c> constructor.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B07033
    // Broiler-Human:        PENDING
    private void SetupArrayFromAsync(JsObject constructor)
    {
        Method(constructor, "fromAsync", 1, (engine, thisValue, arguments) =>
        {
            var promise = NewAsyncPromise();
            engine.Charge(4);

            try
            {
                ArrayFromAsyncStart(engine, promise, thisValue, arguments);
            }
            catch (JsThrow thrown)
            {
                SettleAsyncPromise(engine, promise, thrown.Value, rejected: true);
            }

            return JsValue.Object(promise);
        });
    }

    /// <summary>
    /// The synchronous head of the method: the mapper check, the choice of input shape and the
    /// construction of the result, up to the first <c>Await</c>.
    /// </summary>
    /// <remarks>
    /// <b>The receiver is constructed AFTER the input is inspected</b>, with no argument for an
    /// iterable and with the length for an array-like, because that is the specification's order
    /// and a constructor that logs can see it. A receiver that is not a constructor gets an
    /// ordinary Array rather than a <c>TypeError</c>: the method is a generic factory.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C89977
    // Broiler-Human:        PENDING
    private void ArrayFromAsyncStart(
        JsEngine engine, JsPromiseObject promise, JsValue receiver, JsValue[] arguments)
    {
        var items = ArgOfArray(arguments, 0);
        var mapper = ArgOfArray(arguments, 1);

        if (mapper.Type != JsType.Undefined &&
            (!mapper.IsObject || !mapper.AsObject().IsCallable))
        {
            engine.ThrowTypeError("Array.fromAsync: the mapping function is not a function");
        }

        var constructs = receiver.IsObject && receiver.AsObject().IsConstructor;

        var run = new JsArrayFromAsyncRun(promise)
        {
            Mapper = mapper,
            ThisArg = ArgOfArray(arguments, 2),
        };

        // `GetMethod` FOR EACH SYMBOL, ONCE, AND THE ITERATOR IS MADE FROM THE METHOD THAT WAS
        // READ. Going through `GetIterator` would read `Symbol.iterator` a second time, which a
        // getter on it can count. The Symbols are this realm's own, whatever `Symbol.asyncIterator`
        // has since been reassigned to.
        JsIteratorRecord? record = null;

        if (engine.TryGetSymbolMethod(items, AsyncIteratorSymbol, out var asyncMethod))
        {
            record = ArrayFromAsyncIteratorOf(engine, items, asyncMethod);
        }
        else if (engine.TryGetSymbolMethod(items, IteratorSymbol, out var syncMethod))
        {
            record = CreateAsyncFromSyncIterator(ArrayFromAsyncIteratorOf(engine, items, syncMethod));
        }

        if (record is not null)
        {
            run.Record = record;
            run.Target = constructs
                ? engine.Construct(receiver, System.Array.Empty<JsValue>())
                : ArrayCreate(engine, 0);

            ArrayFromAsyncPull(engine, run);
            return;
        }

        // NEITHER PROTOCOL, SO AN ARRAY-LIKE - a number, a boolean and a plain object without a
        // `length` all land here and answer an empty result, as they do for `Array.from`.
        var source = JsValue.Object(engine.ToObject(items));
        var length = ArrayToLength(engine, engine.GetProperty(source, "length"));

        run.Source = source;
        run.Length = length;
        run.Target = constructs
            ? engine.Construct(receiver, [JsValue.Number(length)])
            : ArrayCreate(engine, length);

        ArrayFromAsyncTake(engine, run);
    }

    /// <summary>
    /// The specification's <c>GetIteratorFromMethod</c>: call the method already read, and read
    /// <c>next</c> once.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=D49D1D
    // Broiler-Human:        PENDING
    private static JsIteratorRecord ArrayFromAsyncIteratorOf(
        JsEngine engine, JsValue items, JsValue method)
    {
        engine.Charge(4);
        var iterator = engine.Call(method, items, System.Array.Empty<JsValue>());

        if (!iterator.IsObject)
        {
            engine.ThrowTypeError("The result of the iterator method is not an object");
        }

        return new JsIteratorRecord(iterator, engine.GetProperty(iterator, "next"));
    }

    /// <summary>
    /// One step over an iterator: call <c>next</c> with no argument and await what it answered.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=EE795C
    // Broiler-Human:        PENDING
    private void ArrayFromAsyncPull(JsEngine engine, JsArrayFromAsyncRun run)
    {
        engine.Charge(1);

        if (run.Index >= ArrayMaxSafeLength)
        {
            ArrayFromAsyncClose(
                engine, run, engine.Error("TypeError", "Array.fromAsync: too many elements").Value);

            return;
        }

        try
        {
            var record = run.Record!;
            var step = engine.Call(record.Next, record.Iterator, System.Array.Empty<JsValue>());

            AwaitOn(engine, step, (inner, value, rejected) =>
                ArrayFromAsyncStepped(inner, run, value, rejected));
        }
        catch (JsThrow thrown)
        {
            ArrayFromAsyncReject(engine, run, thrown.Value);
        }
    }

    /// <summary>
    /// The awaited step: finish on <c>done</c>, otherwise map the value (awaiting the mapper's
    /// answer) and store it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8383E4
    // Broiler-Human:        PENDING
    private void ArrayFromAsyncStepped(
        JsEngine engine, JsArrayFromAsyncRun run, JsValue step, bool rejected)
    {
        if (rejected)
        {
            ArrayFromAsyncReject(engine, run, step);
            return;
        }

        JsValue value;

        try
        {
            if (!step.IsObject)
            {
                engine.ThrowTypeError("Iterator result " + engine.Describe(step) + " is not an object");
            }

            if (engine.GetProperty(step, "done").ToBooleanValue())
            {
                ArrayFromAsyncFinish(engine, run);
                return;
            }

            value = engine.GetProperty(step, "value");
        }
        catch (JsThrow thrown)
        {
            ArrayFromAsyncReject(engine, run, thrown.Value);
            return;
        }

        if (!run.Mapper.IsObject)
        {
            ArrayFromAsyncStore(engine, run, value);
            return;
        }

        // FROM HERE A FAILURE CLOSES THE ITERATOR: the mapper's throw, its rejection, and a
        // failure to resolve what it answered are all `IfAbruptCloseAsyncIterator`.
        try
        {
            var mapped = engine.Call(run.Mapper, run.ThisArg, [value, JsValue.Number(run.Index)]);

            AwaitOn(engine, mapped, (inner, result, failed) =>
            {
                if (failed)
                {
                    ArrayFromAsyncClose(inner, run, result);
                }
                else
                {
                    ArrayFromAsyncStore(inner, run, result);
                }
            });
        }
        catch (JsThrow thrown)
        {
            ArrayFromAsyncClose(engine, run, thrown.Value);
        }
    }

    /// <summary>
    /// Defines one element on the result with <c>CreateDataPropertyOrThrow</c> and pulls the next;
    /// a refusal closes the iterator.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C969A8
    // Broiler-Human:        PENDING
    private void ArrayFromAsyncStore(JsEngine engine, JsArrayFromAsyncRun run, JsValue value)
    {
        try
        {
            ArrayCreateDataAt(engine, run.Target, run.Index, value);
        }
        catch (JsThrow thrown)
        {
            ArrayFromAsyncClose(engine, run, thrown.Value);
            return;
        }

        run.Index++;
        ArrayFromAsyncPull(engine, run);
    }

    /// <summary>
    /// The specification's <c>AsyncIteratorClose</c> under a throw completion: call
    /// <c>return</c>, await its answer, and reject with the ORIGINAL reason whatever happened.
    /// </summary>
    /// <remarks>
    /// <b>The rejection waits for the <c>return</c>.</b> An iterator whose <c>return</c> answers a
    /// promise is given the turns that promise takes before the caller hears about the failure,
    /// because the specification awaits it; rejecting at once would let the caller run while the
    /// iterator is still cleaning up. What <c>return</c> answered, or threw, is discarded: the
    /// failure the caller is owed is the one that made the method stop.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A385FD
    // Broiler-Human:        PENDING
    private void ArrayFromAsyncClose(JsEngine engine, JsArrayFromAsyncRun run, JsValue reason)
    {
        var record = run.Record!;
        record.Done = true;
        JsValue answered;

        try
        {
            var method = engine.GetProperty(record.Iterator, "return");

            if (!method.IsObject || !method.AsObject().IsCallable)
            {
                ArrayFromAsyncReject(engine, run, reason);
                return;
            }

            answered = engine.Call(method, record.Iterator, System.Array.Empty<JsValue>());
        }
        catch (JsThrow)
        {
            ArrayFromAsyncReject(engine, run, reason);
            return;
        }

        try
        {
            AwaitOn(engine, answered, (inner, _, _) => ArrayFromAsyncReject(inner, run, reason));
        }
        catch (JsThrow)
        {
            ArrayFromAsyncReject(engine, run, reason);
        }
    }

    /// <summary>
    /// One element of an array-like: read it, await it, map it (awaiting the answer), store it.
    /// </summary>
    /// <remarks>
    /// <b>Every element is awaited, mapped or not</b>, which is where this path differs from the
    /// iterator path: an async iterator's values are taken as they come, while an array-like's are
    /// resolved, so a thenable element is adopted. The length was read once, before the result was
    /// constructed, and a growing or shrinking source does not change it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=12C2FD
    // Broiler-Human:        PENDING
    private void ArrayFromAsyncTake(JsEngine engine, JsArrayFromAsyncRun run)
    {
        engine.Charge(1);

        try
        {
            if (run.Index >= run.Length)
            {
                ArrayFromAsyncFinish(engine, run);
                return;
            }

            var element = ArrayGetAt(engine, run.Source, run.Index);

            AwaitOn(engine, element, (inner, value, rejected) =>
                ArrayFromAsyncTaken(inner, run, value, rejected));
        }
        catch (JsThrow thrown)
        {
            ArrayFromAsyncReject(engine, run, thrown.Value);
        }
    }

    /// <summary>The awaited array-like element: map it if asked, then store it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=FAFD27
    // Broiler-Human:        PENDING
    private void ArrayFromAsyncTaken(
        JsEngine engine, JsArrayFromAsyncRun run, JsValue value, bool rejected)
    {
        if (rejected)
        {
            ArrayFromAsyncReject(engine, run, value);
            return;
        }

        if (!run.Mapper.IsObject)
        {
            ArrayFromAsyncPlace(engine, run, value);
            return;
        }

        try
        {
            var mapped = engine.Call(run.Mapper, run.ThisArg, [value, JsValue.Number(run.Index)]);

            AwaitOn(engine, mapped, (inner, result, failed) =>
            {
                if (failed)
                {
                    ArrayFromAsyncReject(inner, run, result);
                }
                else
                {
                    ArrayFromAsyncPlace(inner, run, result);
                }
            });
        }
        catch (JsThrow thrown)
        {
            ArrayFromAsyncReject(engine, run, thrown.Value);
        }
    }

    /// <summary>Defines one array-like element on the result and takes the next.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=93FCE4
    // Broiler-Human:        PENDING
    private void ArrayFromAsyncPlace(JsEngine engine, JsArrayFromAsyncRun run, JsValue value)
    {
        try
        {
            ArrayCreateDataAt(engine, run.Target, run.Index, value);
        }
        catch (JsThrow thrown)
        {
            ArrayFromAsyncReject(engine, run, thrown.Value);
            return;
        }

        run.Index++;
        ArrayFromAsyncTake(engine, run);
    }

    /// <summary>
    /// Writes the result's <c>length</c> strictly and resolves the answer with the result.
    /// </summary>
    /// <remarks>
    /// The answer is RESOLVED with the result, as an async function's return is, so a receiver
    /// whose construction produced a thenable is adopted rather than handed back.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=436D96
    // Broiler-Human:        PENDING
    private void ArrayFromAsyncFinish(JsEngine engine, JsArrayFromAsyncRun run)
    {
        try
        {
            ArrayWriteLength(engine, run.Target, run.Index);
        }
        catch (JsThrow thrown)
        {
            ArrayFromAsyncReject(engine, run, thrown.Value);
            return;
        }

        SettleAsyncPromise(engine, run.Promise, run.Target, rejected: false);
    }

    /// <summary>Rejects the answer with <paramref name="reason"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=DD0FC9
    // Broiler-Human:        PENDING
    private void ArrayFromAsyncReject(JsEngine engine, JsArrayFromAsyncRun run, JsValue reason) =>
        SettleAsyncPromise(engine, run.Promise, reason, rejected: true);
}

/// <summary>
/// The state one <c>Array.fromAsync</c> call carries from each continuation to the next: what the
/// async closure would have kept in its locals.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=341151
// Broiler-Human:        PENDING
internal sealed class JsArrayFromAsyncRun
{
    /// <summary>Starts the state for one call, answering through <paramref name="promise"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=097A87
    // Broiler-Human:        PENDING
    internal JsArrayFromAsyncRun(JsPromiseObject promise) => Promise = promise;

    /// <summary>The promise the call answered, settled exactly once at the end of the chain.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8A7CF2
    // Broiler-Human:        PENDING
    internal JsPromiseObject Promise { get; }

    /// <summary>The mapper, or <c>undefined</c> when there is none.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=36813A
    // Broiler-Human:        PENDING
    internal JsValue Mapper { get; init; }

    /// <summary>The <c>this</c> the mapper is called with.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FAA64E
    // Broiler-Human:        PENDING
    internal JsValue ThisArg { get; init; }

    /// <summary>The object being filled: the receiver's construction, or an ordinary Array.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2B0AC4
    // Broiler-Human:        PENDING
    internal JsValue Target { get; set; }

    /// <summary>The async iterator record, or null on the array-like path.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AFE86D
    // Broiler-Human:        PENDING
    internal JsIteratorRecord? Record { get; set; }

    /// <summary>The array-like source, on the array-like path.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E7204A
    // Broiler-Human:        PENDING
    internal JsValue Source { get; set; }

    /// <summary>The array-like's length, read once.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A44326
    // Broiler-Human:        PENDING
    internal double Length { get; set; }

    /// <summary>The index the next element is stored at, and the count stored so far.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FEF573
    // Broiler-Human:        PENDING
    internal double Index { get; set; }
}
