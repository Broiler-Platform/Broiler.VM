// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           5
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  4/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The intrinsics of asynchronous iteration: <c>%AsyncIteratorPrototype%</c>,
/// <c>%AsyncGeneratorPrototype%</c>, <c>%AsyncGeneratorFunction.prototype%</c>,
/// <c>%AsyncGeneratorFunction%</c> and <c>%AsyncFromSyncIteratorPrototype%</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>None of them is a global, and the first four are reached the way the generator intrinsics
/// are</b> — by walking up from an async generator function or from what one of its calls answered.
/// The fifth is reached by nothing at all: <c>%AsyncFromSyncIteratorPrototype%</c> has no name in
/// any realm and no <c>@@toStringTag</c>, and the only way a program observes it is by writing
/// <c>for await</c> over an object that has a <c>Symbol.iterator</c> and no
/// <c>Symbol.asyncIterator</c>, which is what builds one.
/// </para>
/// <para>
/// <b>The chain is four objects deep and every hop is load-bearing.</b> An async generator object
/// inherits from the function's own <c>prototype</c>, which inherits from
/// <c>%AsyncGeneratorPrototype%</c>, which inherits from <c>%AsyncIteratorPrototype%</c>, which is
/// where <c>[Symbol.asyncIterator]</c> answering <c>this</c> lives — so an async generator is an
/// async ITERABLE without <see cref="SetupAsyncGenerator"/> defining that Symbol anywhere near it,
/// exactly as a synchronous generator gets <c>[Symbol.iterator]</c> from
/// <c>%IteratorPrototype%</c>. Defining a second one on <c>%AsyncGeneratorPrototype%</c> would have
/// been a copy of a function programs compare for identity.
/// </para>
/// <para>
/// <b>The promise every method here answers is an ordinary promise of this realm, made by
/// <see cref="NewAsyncPromise"/> and settled through the same paths a <c>resolve</c> callback goes
/// through.</b> The specification takes these capabilities from the intrinsic <c>%Promise%</c>
/// rather than from the receiver's species, which is the one place async iteration is LESS generic
/// than <c>then</c> — and copying <c>then</c>'s species lookup here would have let a subclass
/// change what <c>next</c> answers, which no engine does.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>
    /// <c>%AsyncIteratorPrototype%</c>: where <c>[Symbol.asyncIterator]</c> answering itself lives.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=0E5AE2
    // Broiler-Human:        PENDING
    internal JsObject AsyncIteratorPrototype { get; private set; } = null!;

    /// <summary><c>%AsyncGeneratorPrototype%</c>: <c>next</c>, <c>return</c> and <c>throw</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=93D570
    // Broiler-Human:        PENDING
    internal JsObject AsyncGeneratorPrototype { get; private set; } = null!;

    /// <summary>
    /// <c>%AsyncGeneratorFunction.prototype%</c>: what every async generator function inherits from.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=16DF44
    // Broiler-Human:        PENDING
    internal JsObject AsyncGeneratorFunctionPrototype { get; private set; } = null!;

    /// <summary>
    /// <c>%AsyncFromSyncIteratorPrototype%</c>: what wraps a synchronous iterator for
    /// <c>for await</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=0AB28B
    // Broiler-Human:        PENDING
    private JsObject AsyncFromSyncIteratorPrototype { get; set; } = null!;

    /// <summary>Builds the asynchronous half of the iteration protocol.</summary>
    /// <remarks>
    /// <b>It is called from <see cref="SetupGenerator"/> for the reason
    /// <see cref="SetupAsyncFunction"/> is</b>: it needs the <c>Function</c> constructor
    /// <c>SetupGlobal</c> published, and one entry in the realm's setup list is one place to keep
    /// that ordering constraint rather than three.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=5B8C23
    // Broiler-Human:        PENDING
    private void SetupAsyncGenerator()
    {
        // ITS PROTOTYPE IS `Object.prototype` AND NOT `%IteratorPrototype%`, which is the one thing
        // about this object a reader is most likely to assume wrong. The two protocols are
        // disjoint: nothing that iterates asynchronously has a `Symbol.iterator`, and an async
        // generator that inherited `%IteratorPrototype%`'s would answer itself to `for … of` and
        // then fail on a `next` that returns a promise rather than a step.
        AsyncIteratorPrototype = new JsObject(ObjectPrototype);

        AsyncIteratorPrototype.SetOwnSymbol(
            AsyncIteratorSymbol,
            JsProperty.Data(
                JsValue.Object(Native("[Symbol.asyncIterator]", 0, static (engine, thisValue, arguments) =>
                {
                    _ = engine;
                    _ = arguments;
                    return thisValue;
                })),
                JsPropertyAttributes.BuiltIn));

        AsyncGeneratorPrototype = new JsObject(AsyncIteratorPrototype, "AsyncGenerator");
        AsyncGeneratorFunctionPrototype = new JsObject(FunctionPrototype, "AsyncGeneratorFunction");

        // THE BACK-LINK IS DEFINED FIRST, and the order is observable rather than cosmetic, exactly
        // as it is on `%GeneratorPrototype%`: `Object.getOwnPropertyNames` yields non-index keys in
        // creation order, and every engine answers `constructor,next,return,throw`.
        AsyncGeneratorPrototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(
                JsValue.Object(AsyncGeneratorFunctionPrototype), JsPropertyAttributes.Configurable));

        Method(AsyncGeneratorPrototype, "next", 1, static (engine, thisValue, arguments) =>
            engine.EnqueueAsyncGenerator(
                thisValue, JsResumeMode.Next, GeneratorArgument(arguments), "next"));

        Method(AsyncGeneratorPrototype, "return", 1, static (engine, thisValue, arguments) =>
            engine.EnqueueAsyncGenerator(
                thisValue, JsResumeMode.Return, GeneratorArgument(arguments), "return"));

        Method(AsyncGeneratorPrototype, "throw", 1, static (engine, thisValue, arguments) =>
            engine.EnqueueAsyncGenerator(
                thisValue, JsResumeMode.Throw, GeneratorArgument(arguments), "throw"));

        AsyncGeneratorPrototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("AsyncGenerator"), JsPropertyAttributes.Configurable));

        AsyncGeneratorFunctionPrototype.SetOwnProperty(
            "prototype",
            JsProperty.Data(
                JsValue.Object(AsyncGeneratorPrototype), JsPropertyAttributes.Configurable));

        AsyncGeneratorFunctionPrototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(
                JsValue.String("AsyncGeneratorFunction"), JsPropertyAttributes.Configurable));

        // THE CONSTRUCTOR EXISTS AND REFUSES, for the reason `Function`, `GeneratorFunction` and
        // `AsyncFunction` do. It is three hops off an async generator function and is a global
        // nowhere, but a program that walks there finds the intrinsic the specification says is
        // there rather than an absence whose cause it has to guess.
        var constructor = new JsNativeFunction(
            GlobalFunctionConstructor(),
            "AsyncGeneratorFunction",
            1,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError(AsyncGeneratorFunctionRefusal),
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError(AsyncGeneratorFunctionRefusal));

        constructor.SetOwnProperty(
            "prototype",
            JsProperty.Data(
                JsValue.Object(AsyncGeneratorFunctionPrototype), JsPropertyAttributes.None));

        AsyncGeneratorFunctionPrototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Configurable));

        SetupAsyncFromSyncIterator();
    }

    /// <summary>What a call or a construction of <c>AsyncGeneratorFunction</c> is told, and why.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B75E95
    // Broiler-Human:        PENDING
    private const string AsyncGeneratorFunctionRefusal =
        "AsyncGeneratorFunction: the broiler.javascript.wide manifest does not admit the " +
        "AsyncGeneratorFunction constructor, because this profile declares no guest-initiated " +
        "load and cannot turn source into code at run time";

    /// <summary>Builds an async generator object over a frame that has not started.</summary>
    /// <remarks>
    /// Its prototype is the function's own <c>prototype</c> property, read the way an ordinary
    /// <c>new</c> reads it, which is what <c>OrdinaryCreateFromConstructor</c> does and what makes
    /// <c>Object.getPrototypeOf(g()) === g.prototype</c> a fact about the program.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6D2BD2
    // Broiler-Human:        PENDING
    internal JsAsyncGenerator CreateAsyncGenerator(JsScriptFunction function, JsFrame frame)
    {
        var declared = engine.GetProperty(JsValue.Object(function), "prototype");

        return new JsAsyncGenerator(
            declared.IsObject ? declared.AsObject() : AsyncGeneratorPrototype, frame);
    }

    /// <summary>
    /// <c>GetIterator(obj, ~async~)</c>: the async iterator, or a synchronous one wrapped so that
    /// every value it answers has been awaited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The fall-back to <c>Symbol.iterator</c> is not a convenience, it is the operation.</b>
    /// <c>for await (const x of [p, q])</c> is the idiom the loop exists for, and an Array has no
    /// <c>Symbol.asyncIterator</c> — so a <c>for await</c> that refused anything without one would
    /// refuse the case every program writes. What the wrapper adds is the awaiting of each VALUE,
    /// which is the difference between iterating promises and iterating what they resolve to.
    /// </para>
    /// <para>
    /// <b>A value with NEITHER Symbol is not iterable at all</b>, and the message says so with the
    /// same words the synchronous <c>GetIterator</c> uses: an object that is missing both is
    /// missing the protocol rather than missing the asynchronous half of it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=EFF362
    // Broiler-Falsified-If: a `for await` over an object carrying `Symbol.asyncIterator` reaches the synchronous wrapper, or one over an Array of promises answers the promises rather than their values
    // Broiler-Human:        PENDING
    internal JsIteratorRecord GetAsyncIterator(JsValue iterable)
    {
        engine.Charge(4);

        if (iterable.IsNullish)
        {
            engine.ThrowTypeError(engine.Describe(iterable) + " is not async iterable");
        }

        if (engine.TryGetSymbolMethod(iterable, AsyncIteratorSymbol, out var method))
        {
            var iterator = engine.Call(method, iterable, System.Array.Empty<JsValue>());

            if (!iterator.IsObject)
            {
                engine.ThrowTypeError("The result of the async iterator method is not an object");
            }

            return new JsIteratorRecord(iterator, engine.GetProperty(iterator, "next"));
        }

        return CreateAsyncFromSyncIterator(engine.GetIterator(iterable));
    }

    /// <summary>Wraps a synchronous iterator record as an asynchronous one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=935FC5
    // Broiler-Human:        PENDING
    private JsIteratorRecord CreateAsyncFromSyncIterator(JsIteratorRecord synchronous)
    {
        var wrapper = new JsAsyncFromSyncIterator(AsyncFromSyncIteratorPrototype, synchronous);
        engine.Charge(4);

        return new JsIteratorRecord(
            JsValue.Object(wrapper), engine.GetProperty(JsValue.Object(wrapper), "next"));
    }

    /// <summary>
    /// Builds <c>%AsyncFromSyncIteratorPrototype%</c>: three methods, each answering a promise.
    /// </summary>
    /// <remarks>
    /// <b>Every one of the three ends in the same continuation, and the continuation is where the
    /// awaiting happens.</b> The synchronous step is taken FIRST and synchronously — that is what
    /// makes <c>for await</c> over an Array pull the next element before it waits — and only the
    /// <c>value</c> it answered is resolved and waited on. An implementation that awaited the whole
    /// step object instead would answer <c>{ value: promise }</c> unchanged, because a step object
    /// is not a thenable.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=FDB15D
    // Broiler-Human:        PENDING
    private void SetupAsyncFromSyncIterator()
    {
        AsyncFromSyncIteratorPrototype = new JsObject(AsyncIteratorPrototype);

        Method(AsyncFromSyncIteratorPrototype, "next", 1, static (engine, thisValue, arguments) =>
        {
            var realm = engine.Realm;

            if (thisValue.AsObjectOrNull() is not JsAsyncFromSyncIterator wrapper)
            {
                return realm.RejectedPromise(
                    engine, "next called on a value that is not an async-from-sync iterator");
            }

            var record = wrapper.Synchronous;

            try
            {
                var step = engine.Call(record.Next, record.Iterator, arguments);

                if (!step.IsObject)
                {
                    return engine.ThrowTypeError(
                        "Iterator result " + engine.Describe(step) + " is not an object");
                }

                return realm.AsyncFromSyncContinuation(engine, step, record, closeOnRejection: true);
            }
            catch (JsThrow thrown)
            {
                record.Done = true;
                return realm.RejectWith(engine, thrown.Value);
            }
        });

        Method(AsyncFromSyncIteratorPrototype, "return", 1, static (engine, thisValue, arguments) =>
        {
            var realm = engine.Realm;

            if (thisValue.AsObjectOrNull() is not JsAsyncFromSyncIterator wrapper)
            {
                return realm.RejectedPromise(
                    engine, "return called on a value that is not an async-from-sync iterator");
            }

            var record = wrapper.Synchronous;

            try
            {
                var returner = engine.GetProperty(record.Iterator, "return");

                // AN ITERATOR WITH NO `return` IS NOT AN ERROR AND IS NOT A NO-OP EITHER: the
                // wrapper answers a DONE step carrying the value it was given, because the caller
                // asked to stop and there is nothing to tell. Rejecting here would have made
                // `break` out of a `for await` over an Array throw.
                if (returner.IsNullish)
                {
                    record.Done = true;

                    return realm.FulfilledPromise(
                        engine,
                        JsValue.Object(realm.IteratorResult(GeneratorArgument(arguments), true)));
                }

                var step = engine.Call(returner, record.Iterator, arguments);

                if (!step.IsObject)
                {
                    return engine.ThrowTypeError(
                        "The iterator's return answered " + engine.Describe(step) +
                        " and not an object");
                }

                return realm.AsyncFromSyncContinuation(engine, step, record, closeOnRejection: false);
            }
            catch (JsThrow thrown)
            {
                return realm.RejectWith(engine, thrown.Value);
            }
        });

        Method(AsyncFromSyncIteratorPrototype, "throw", 1, static (engine, thisValue, arguments) =>
        {
            var realm = engine.Realm;

            if (thisValue.AsObjectOrNull() is not JsAsyncFromSyncIterator wrapper)
            {
                return realm.RejectedPromise(
                    engine, "throw called on a value that is not an async-from-sync iterator");
            }

            var record = wrapper.Synchronous;

            try
            {
                var thrower = engine.GetProperty(record.Iterator, "throw");

                // AN ITERATOR WITH NO `throw` IS CLOSED FIRST AND THE TYPE ERROR COMES SECOND,
                // which is the order the specification is explicit about: closing gives the
                // iterator its chance to clean up before it is told it violated the protocol, and
                // an error the close itself raises replaces the TypeError rather than being lost
                // behind it.
                if (thrower.IsNullish)
                {
                    engine.CloseIterator(record);

                    return engine.ThrowTypeError(
                        "The iterator does not provide a 'throw' method.");
                }

                var step = engine.Call(thrower, record.Iterator, arguments);

                if (!step.IsObject)
                {
                    return engine.ThrowTypeError(
                        "The iterator's throw answered " + engine.Describe(step) +
                        " and not an object");
                }

                return realm.AsyncFromSyncContinuation(engine, step, record, closeOnRejection: true);
            }
            catch (JsThrow thrown)
            {
                return realm.RejectWith(engine, thrown.Value);
            }
        });
    }

    /// <summary>
    /// <c>AsyncFromSyncIteratorContinuation</c>: read the step, wait for its value, re-package it.
    /// </summary>
    /// <remarks>
    /// <b><paramref name="closeOnRejection"/> is why the two rejection handlers differ, and it is
    /// not symmetry for its own sake.</b> A <c>next</c> whose value is a promise that REJECTS has
    /// abandoned the iteration part-way, so the synchronous iterator underneath is owed its
    /// <c>return</c>; a <c>return</c> whose value rejects is already closing it, and closing it a
    /// second time would call <c>return</c> twice on an iterator that has been told once.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=92389F
    // Broiler-Human:        PENDING
    private JsValue AsyncFromSyncContinuation(
        JsEngine owner, JsValue step, JsIteratorRecord record, bool closeOnRejection)
    {
        var done = owner.GetProperty(step, "done").ToBooleanValue();
        var value = owner.GetProperty(step, "value");

        if (done)
        {
            record.Done = true;
        }

        var wrapped = PromiseResolveValue(owner, value);

        var onFulfil = JsValue.Object(Native("", 1, (inner, thisValue, arguments) =>
        {
            _ = thisValue;
            return JsValue.Object(
                inner.Realm.IteratorResult(ArgOfPromise(arguments, 0), done));
        }));

        var onReject = !done && closeOnRejection
            ? JsValue.Object(Native("", 1, (inner, thisValue, arguments) =>
            {
                _ = thisValue;
                var reason = ArgOfPromise(arguments, 0);
                inner.CloseIteratorQuietly(record);
                throw new JsThrow(reason, inner.Render(reason));
            }))
            : JsValue.Undefined;

        return JsValue.Object(PromiseThen(owner, wrapped, onFulfil, onReject));
    }

    /// <summary>A promise already fulfilled with <paramref name="value"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=FD8AB5
    // Broiler-Human:        PENDING
    internal JsValue FulfilledPromise(JsEngine owner, JsValue value)
    {
        var promise = NewAsyncPromise();
        PromiseSettle(owner, promise, value, JsPromiseState.Fulfilled);
        return JsValue.Object(promise);
    }

    /// <summary>A promise already rejected with <paramref name="reason"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=EFB2D6
    // Broiler-Human:        PENDING
    internal JsValue RejectWith(JsEngine owner, JsValue reason)
    {
        var promise = NewAsyncPromise();
        PromiseSettle(owner, promise, reason, JsPromiseState.Rejected);
        return JsValue.Object(promise);
    }

    /// <summary>
    /// A promise already rejected with a <c>TypeError</c> carrying <paramref name="message"/>.
    /// </summary>
    /// <remarks>
    /// <b>It REJECTS where the synchronous protocol THROWS, and the difference is the whole shape of
    /// this family.</b> <c>Generator.prototype.next.call(1)</c> throws at the call site;
    /// <c>AsyncGenerator.prototype.next.call(1)</c> answers a promise that is already rejected,
    /// because the method's contract is that it answers a promise — and a caller writing
    /// <c>gen.next().catch(…)</c> would otherwise have to write a <c>try</c> around it as well.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=98AF4C
    // Broiler-Human:        PENDING
    internal JsValue RejectedPromise(JsEngine owner, string message) =>
        RejectWith(owner, owner.Error("TypeError", message).Value);

    /// <summary>Settles the promise one request is waiting on, with a step or with a rejection.</summary>
    /// <remarks>
    /// It is the specification's <c>AsyncGeneratorCompleteStep</c>, and the removal from the queue
    /// happens HERE rather than where the request was taken — which is what lets a <c>yield</c>
    /// answer the front request and then look at the queue again to see whether it may carry on
    /// without suspending.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C768FE
    // Broiler-Human:        PENDING
    internal void CompleteAsyncGeneratorStep(
        JsEngine owner, JsAsyncGenerator generator, JsValue value, bool done, bool rejected)
    {
        if (generator.Queue.Count == 0)
        {
            return;
        }

        var request = generator.Queue[0];
        generator.Queue.RemoveAt(0);

        if (rejected)
        {
            PromiseSettle(owner, request.Promise, value, JsPromiseState.Rejected);
            return;
        }

        // A STEP IS FULFILLED WITH AND NOT RESOLVED WITH, which is what keeps `yield somePromise`
        // from being unwrapped a second time. The value a `yield` carries has already been awaited
        // by the body; resolving the request's promise with the step object would be harmless, and
        // resolving it with the VALUE would adopt a thenable the program is entitled to see.
        PromiseSettle(
            owner,
            request.Promise,
            JsValue.Object(IteratorResult(value, done)),
            JsPromiseState.Fulfilled);
    }
}

/// <summary>
/// The object <c>CreateAsyncFromSyncIterator</c> makes: a synchronous iterator record, wearing the
/// asynchronous protocol.
/// </summary>
/// <remarks>
/// <b>It holds the RECORD and not the iterator object</b>, for the reason
/// <see cref="JsFrame.Delegate"/> does: the record carries the <c>next</c> that was read once when
/// the iterator was acquired, and the done flag that decides whether a close is owed. Holding the
/// object alone would re-read <c>next</c> at every step, which a program that replaces its own
/// <c>next</c> mid-loop can see.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7A8E77
// Broiler-Human:        PENDING
internal sealed class JsAsyncFromSyncIterator : JsObject
{
    /// <summary>Wraps one synchronous iterator record.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AF33B7
    // Broiler-Human:        PENDING
    internal JsAsyncFromSyncIterator(JsObject? prototype, JsIteratorRecord synchronous)
        : base(prototype, "AsyncFromSyncIterator") => Synchronous = synchronous;

    /// <summary>The record every one of the three methods drives.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AFCAB0
    // Broiler-Human:        PENDING
    internal JsIteratorRecord Synchronous { get; }
}
