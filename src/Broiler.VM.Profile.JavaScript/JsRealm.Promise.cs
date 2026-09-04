// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           6
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  4/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Promise</c>: the constructor, the prototype, and the five combinators.
/// </summary>
/// <remarks>
/// <para>
/// <b>EVERY REACTION RUNS AS A JOB. NOTHING IN THIS FILE CALLS A HANDLER SYNCHRONOUSLY, EVER.</b>
/// That is the one rule the whole type is built on and the one a reader should check first:
/// <c>Promise.resolve(1).then(f); print("sync")</c> prints <c>sync</c> and only then runs
/// <c>f</c>, whether the promise was already settled, settled during the call, or settled a
/// thousand instructions later. An implementation that took a short cut for the already-settled
/// case - "there is nothing to wait for, so call it now" - would be the classic Zalgo defect: a
/// callback that sometimes runs before and sometimes after the code that registered it, so that
/// every invariant the caller establishes on the line below is sometimes established too late.
/// The short cut is not taken, and <see cref="PromiseSchedule"/> is the single choke point through
/// which that is enforceable by inspection.
/// </para>
/// <para>
/// <b>Nothing here drains the queue, and that is also deliberate.</b> Reactions land in
/// <see cref="JsEngine.EnqueueJob"/> and stay there until the HOST invokes the reserved drain entry
/// point. A profile that drained implicitly - at the end of a script, say - would be choosing a
/// point no embedder stated, and an embedder that interleaves scripts with its own work could not
/// reason about when guest code ran. The consequence is worth stating plainly: on a host that never
/// drains, every <c>then</c> in a program is registered and none of them runs, and the program's
/// synchronous half completes normally. That is not a hang and not a leak; it is the host declining
/// to give the guest another turn.
/// </para>
/// <para>
/// <b>An unhandled rejection is reported NOWHERE.</b> The specification makes it the host's
/// business - <c>HostPromiseRejectionTracker</c> - and this profile has no such hook: the core's
/// result envelope carries a completion or a fault, and a promise nobody handled is neither. So
/// <c>[[PromiseIsHandled]]</c> is not tracked, there is no <c>unhandledrejection</c>, and a
/// rejected promise with no handler is silent. A rejection that reaches a DRAIN with no handler is
/// equally silent; the only thing the drain surfaces is a throw out of a job body itself.
/// </para>
/// <para>
/// <b>Subclassing is not honoured.</b> The specification threads <c>this</c> through
/// <c>then</c>, <c>all</c> and <c>resolve</c> as a constructor <c>C</c> and builds the derived
/// promise with <c>NewPromiseCapability(C)</c>, so that a subclass of <c>Promise</c> yields
/// subclass instances. Everything here builds a plain <c>Promise</c> on this realm's own
/// prototype. <c>Symbol.species</c>, which is the other half of that machinery, does not exist in
/// this realm at all. A program that subclasses <c>Promise</c> gets working promises of the wrong
/// class rather than broken ones, and that is the declared trade.
/// </para>
/// <para>
/// <b>The combinators take an ARRAY-LIKE, for the same reason the collections do</b> - see
/// <c>JsRealm.Collections.cs</c> - because <c>GetIterator</c> needs a well-known Symbol this realm
/// has not wired to them. <c>Promise.all([a, b])</c> works; <c>Promise.all(someSet)</c> rejects
/// with a <c>TypeError</c>, which is what a conforming engine does for a non-iterable and what
/// this one does for a non-array-like. The set of arguments that behave differently is exactly
/// "iterables that are not array-like" and "array-likes that are not iterable".
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>What one recorded reaction is charged against the live-bytes ceiling.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=372897
    // Broiler-Human:        PENDING
    private const ulong PromiseReactionBytes = 128;

    /// <summary><c>Promise.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=927C72
    // Broiler-Human:        PENDING
    internal JsObject PromisePrototype { get; private set; } = null!;

    /// <summary>
    /// The <c>Promise</c> constructor, held because <c>Promise.resolve</c> must be able to ask
    /// whether a value's <c>constructor</c> is this one before handing it straight back.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B608E6
    // Broiler-Human:        PENDING
    internal JsNativeFunction PromiseConstructor { get; private set; } = null!;

    /// <summary>Builds <c>Promise</c>, its prototype and its statics.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=45334E
    // Broiler-Human:        PENDING
    private void SetupPromise()
    {
        PromisePrototype = new JsObject(ObjectPrototype, "Promise");

        PromiseConstructor = Constructor(
            "Promise",
            1,
            PromisePrototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Promise constructor cannot be invoked without 'new'"),
            (engine, thisValue, arguments) =>
            {
                var executor = ArgOfPromise(arguments, 0);

                if (!executor.IsObject || !executor.AsObject().IsCallable)
                {
                    throw engine.Error("TypeError", "Promise resolver is not a function");
                }

                var promise = new JsPromiseObject(PromisePrototype);
                engine.Charge(4);
                PromiseResolvers(engine, promise, out var resolve, out var reject);

                // THE EXECUTOR RUNS SYNCHRONOUSLY AND ITS THROW BECOMES A REJECTION. Both halves
                // are the specification's: `new Promise(f)` has already called `f` by the time it
                // returns, and a throw out of `f` after it called `resolve` is SWALLOWED rather
                // than turning a resolved promise into a rejected one - which is what routing the
                // throw through the same latched `reject` gives, for free.
                try
                {
                    _ = engine.Call(executor, JsValue.Undefined, [resolve, reject]);
                }
                catch (JsThrow thrown)
                {
                    _ = engine.Call(reject, JsValue.Undefined, [thrown.Value]);
                }

                return JsValue.Object(promise);
            });

        SetupPromiseReactions();
        SetupPromiseCombinators();
    }

    /// <summary><c>then</c>, <c>catch</c> and <c>finally</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=E50BFF
    // Broiler-Human:        PENDING
    private void SetupPromiseReactions()
    {
        Method(PromisePrototype, "then", 2, (engine, thisValue, arguments) =>
            JsValue.Object(
                PromiseThen(
                    engine,
                    PromiseThisPromise(engine, thisValue, "then"),
                    ArgOfPromise(arguments, 0),
                    ArgOfPromise(arguments, 1))));

        // `catch` AND `finally` GO BACK THROUGH THE RECEIVER'S OWN `then` RATHER THAN CALLING THE
        // INTERNAL ONE. The specification defines both by Invoke(promise, "then", ...), so a
        // program that replaced `then` on an instance or on the prototype sees its replacement
        // used - which is a thing test suites and instrumentation libraries actually do.
        Method(PromisePrototype, "catch", 1, static (engine, thisValue, arguments) =>
            engine.Call(
                engine.GetProperty(thisValue, "then"),
                thisValue,
                [JsValue.Undefined, ArgOfPromise(arguments, 0)]));

        Method(PromisePrototype, "finally", 1, (engine, thisValue, arguments) =>
        {
            var onFinally = ArgOfPromise(arguments, 0);
            var thenFunction = engine.GetProperty(thisValue, "then");

            // A NON-CALLABLE ARGUMENT IS PASSED THROUGH TO `then` AS BOTH HANDLERS, which makes it
            // a no-op that preserves the settlement rather than an error. That is the
            // specification's wording and it is what keeps `p.finally(undefined)` harmless.
            if (!onFinally.IsObject || !onFinally.AsObject().IsCallable)
            {
                return engine.Call(thenFunction, thisValue, [onFinally, onFinally]);
            }

            // THE OUTCOME OF `onFinally` IS WAITED ON AND THEN DISCARDED. `p.finally(f)` settles
            // exactly as `p` did, but not until whatever `f` returned has settled: a `finally` that
            // returns a promise delays the chain, and a `finally` that throws replaces the
            // settlement with its own rejection. Anything simpler - calling `f` and ignoring what
            // it gave back - makes `finally` useless for the one thing it exists for, which is
            // releasing a resource before the value moves on.
            var pass = Native("", 1, (inner, ignored, passed) =>
            {
                var value = ArgOfPromise(passed, 0);
                var produced = inner.Call(onFinally, JsValue.Undefined, []);

                return JsValue.Object(
                    PromiseThen(
                        inner,
                        PromiseResolveValue(inner, produced),
                        JsValue.Object(Native("", 0, (deeper, alsoIgnored, none) => value)),
                        JsValue.Undefined));
            });

            var rethrow = Native("", 1, (inner, ignored, passed) =>
            {
                var reason = ArgOfPromise(passed, 0);
                var produced = inner.Call(onFinally, JsValue.Undefined, []);

                return JsValue.Object(
                    PromiseThen(
                        inner,
                        PromiseResolveValue(inner, produced),
                        JsValue.Object(Native("", 0, (deeper, alsoIgnored, none) =>
                            throw new JsThrow(reason, deeper.Render(reason)))),
                        JsValue.Undefined));
            });

            return engine.Call(
                thenFunction, thisValue, [JsValue.Object(pass), JsValue.Object(rethrow)]);
        });
    }

    /// <summary><c>resolve</c>, <c>reject</c>, <c>all</c>, <c>allSettled</c>, <c>race</c>, <c>any</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=5A97C5
    // Broiler-Human:        PENDING
    private void SetupPromiseCombinators()
    {
        Method(PromiseConstructor, "resolve", 1, (engine, thisValue, arguments) =>
            JsValue.Object(PromiseResolveValue(engine, ArgOfPromise(arguments, 0))));

        Method(PromiseConstructor, "reject", 1, (engine, thisValue, arguments) =>
        {
            var promise = new JsPromiseObject(PromisePrototype);
            PromiseSettle(engine, promise, ArgOfPromise(arguments, 0), JsPromiseState.Rejected);
            return JsValue.Object(promise);
        });

        // `all` RESOLVES WITH A DENSE ARRAY IN THE INPUT'S ORDER AND NOT IN SETTLEMENT ORDER, which
        // is why the slots are reserved before any element is waited on: the index a value belongs
        // at is decided when the walk reaches it and not when its promise settles.
        Method(PromiseConstructor, "all", 1, (engine, thisValue, arguments) =>
            PromiseCombine(engine, ArgOfPromise(arguments, 0), JsPromiseCombination.All));

        Method(PromiseConstructor, "allSettled", 1, (engine, thisValue, arguments) =>
            PromiseCombine(engine, ArgOfPromise(arguments, 0), JsPromiseCombination.AllSettled));

        // `race` OVER AN EMPTY ARRAY IS PENDING FOR EVER, and that is correct rather than an
        // oversight: there is no first settlement to adopt, so there is nothing the result could
        // settle to. `any` over an empty array rejects instead, because "none of them succeeded" is
        // a fact an empty set does establish.
        Method(PromiseConstructor, "race", 1, (engine, thisValue, arguments) =>
            PromiseCombine(engine, ArgOfPromise(arguments, 0), JsPromiseCombination.Race));

        Method(PromiseConstructor, "any", 1, (engine, thisValue, arguments) =>
            PromiseCombine(engine, ArgOfPromise(arguments, 0), JsPromiseCombination.Any));
    }

    /// <summary>Reads argument <paramref name="at"/>, which may not have been supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6338B6
    // Broiler-Human:        PENDING
    private static JsValue ArgOfPromise(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>The receiver as a Promise, or a <c>TypeError</c> naming the method.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F206C0
    // Broiler-Human:        PENDING
    private static JsPromiseObject PromiseThisPromise(
        JsEngine engine, JsValue thisValue, string method) =>
        thisValue.AsObjectOrNull() as JsPromiseObject ??
            throw engine.Error(
                "TypeError", "Promise.prototype." + method + " called on an incompatible receiver");

    /// <summary>
    /// Builds the <c>resolve</c>/<c>reject</c> pair for <paramref name="promise"/>, sharing one
    /// latch.
    /// </summary>
    /// <remarks>
    /// The pair is what the executor is handed and what a thenable adoption is handed, and each
    /// call site gets a FRESH pair with a fresh latch. Reusing one pair per promise would make the
    /// adoption job's <c>resolve</c> already latched by the executor's, so a promise resolved with
    /// a thenable could never settle.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B3D392
    // Broiler-Human:        PENDING
    private void PromiseResolvers(
        JsEngine engine, JsPromiseObject promise, out JsValue resolve, out JsValue reject)
    {
        var latch = new JsPromiseLatch();
        engine.Retain(PromiseReactionBytes);

        resolve = JsValue.Object(Native("", 1, (inner, thisValue, arguments) =>
        {
            if (latch.Latched)
            {
                return JsValue.Undefined;
            }

            latch.Latched = true;
            PromiseResolveWith(inner, promise, ArgOfPromise(arguments, 0));
            return JsValue.Undefined;
        }));

        reject = JsValue.Object(Native("", 1, (inner, thisValue, arguments) =>
        {
            if (latch.Latched)
            {
                return JsValue.Undefined;
            }

            latch.Latched = true;
            PromiseSettle(inner, promise, ArgOfPromise(arguments, 0), JsPromiseState.Rejected);
            return JsValue.Undefined;
        }));
    }

    /// <summary>
    /// The specification's resolve procedure: a thenable is ADOPTED through a job and everything
    /// else fulfils.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Three cases and each of them is load-bearing.</b> Resolving a promise with ITSELF is a
    /// <c>TypeError</c> rejection rather than a hang, because the alternative is a promise waiting
    /// on itself for ever with nothing to report. Resolving with an object whose <c>then</c> is
    /// callable adopts that object's eventual state - which is what makes any library's
    /// hand-rolled thenable interoperate with this one - and does so through a JOB, so a thenable
    /// costs one extra turn exactly as it does everywhere else. Resolving with anything else,
    /// including a non-callable <c>then</c>, fulfils with the value as it is.
    /// </para>
    /// <para>
    /// <b>The <c>then</c> property is read ONCE and the read can throw.</b> A getter on <c>then</c>
    /// is guest code, it runs here, and if it throws the promise rejects with what it threw rather
    /// than the throw escaping into whatever built-in happened to be resolving. Reading it twice -
    /// once to test and once to call - would let a getter answer differently each time and hand the
    /// adoption a function the test never saw.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8A770B
    // Broiler-Human:        PENDING
    private void PromiseResolveWith(JsEngine engine, JsPromiseObject promise, JsValue value)
    {
        if (ReferenceEquals(value.AsObjectOrNull(), promise))
        {
            PromiseSettle(
                engine,
                promise,
                CreateError("TypeError", "Chaining cycle detected for promise"),
                JsPromiseState.Rejected);

            return;
        }

        if (!value.IsObject)
        {
            PromiseSettle(engine, promise, value, JsPromiseState.Fulfilled);
            return;
        }

        JsValue thenFunction;

        try
        {
            thenFunction = engine.GetProperty(value, "then");
        }
        catch (JsThrow thrown)
        {
            PromiseSettle(engine, promise, thrown.Value, JsPromiseState.Rejected);
            return;
        }

        if (!thenFunction.IsObject || !thenFunction.AsObject().IsCallable)
        {
            PromiseSettle(engine, promise, value, JsPromiseState.Fulfilled);
            return;
        }

        engine.EnqueueJob(
            JsValue.Object(Native("", 0, (inner, thisValue, arguments) =>
            {
                PromiseResolvers(inner, promise, out var resolve, out var reject);

                try
                {
                    _ = inner.Call(thenFunction, value, [resolve, reject]);
                }
                catch (JsThrow thrown)
                {
                    _ = inner.Call(reject, JsValue.Undefined, [thrown.Value]);
                }

                return JsValue.Undefined;
            })),
            []);
    }

    /// <summary>Settles a pending promise and schedules everything waiting on that side.</summary>
    /// <remarks>
    /// A promise that is already settled is left alone in silence, which is what makes every
    /// second call on a latched pair, every late <c>reject</c> from a badly written executor and
    /// every extra settlement from a <c>race</c> a no-op rather than an error.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=14505C
    // Broiler-Human:        PENDING
    private void PromiseSettle(
        JsEngine engine, JsPromiseObject promise, JsValue value, JsPromiseState state)
    {
        if (promise.State != JsPromiseState.Pending)
        {
            return;
        }

        promise.State = state;
        promise.Result = value;

        var waiting = state == JsPromiseState.Fulfilled
            ? promise.FulfilReactions
            : promise.RejectReactions;

        foreach (var reaction in waiting)
        {
            PromiseSchedule(engine, reaction, value);
        }

        // BOTH LISTS ARE RELEASED, not just the one that ran. The losing side can never fire, and a
        // settled promise that still held it would keep every rejection handler a program ever
        // attached to a long-lived resolved promise alive for the life of the realm.
        promise.FulfilReactions.Clear();
        promise.RejectReactions.Clear();
    }

    /// <summary>
    /// Puts one reaction on the job queue. THE ONLY PLACE A REACTION IS EVER SCHEDULED.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Every path that could run a handler - a settlement with reactions waiting, a <c>then</c> on
    /// an already-settled promise, a combinator's per-element hook - arrives here, and here there
    /// is exactly one thing to do with it: hand it to the engine's queue. That is what makes the
    /// asynchrony checkable by reading one method rather than by auditing every branch that settles
    /// a promise.
    /// </para>
    /// <para>
    /// The job is a fresh native function closing over the reaction and its argument, so the queue
    /// stays a queue of callables and needs no promise-shaped entry of its own. One small object
    /// per reaction is the cost, and it is charged: <see cref="JsEngine.EnqueueJob"/> takes fuel and
    /// reports retention, so a program that schedules without bound spends its allowance rather
    /// than the host's memory.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=C89A73
    // Broiler-Falsified-If: a promise reaction runs before the synchronous continuation of whatever settled or observed the promise
    // Broiler-Human:        PENDING
    private void PromiseSchedule(JsEngine engine, JsPromiseReaction reaction, JsValue argument) =>
        engine.EnqueueJob(
            JsValue.Object(Native("", 0, (inner, thisValue, arguments) =>
            {
                PromiseRun(inner, reaction, argument);
                return JsValue.Undefined;
            })),
            []);

    /// <summary>Runs one reaction, inside its job.</summary>
    /// <remarks>
    /// A handler that is not callable is the PASS-THROUGH: the fulfil side resolves the derived
    /// promise with the value and the reject side rejects it with the reason, which is what makes
    /// <c>p.then(null).then(f)</c> reach <c>f</c> and <c>p.catch(g)</c> forward a fulfilment past
    /// <c>g</c> untouched.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=7EC4E4
    // Broiler-Human:        PENDING
    private void PromiseRun(JsEngine engine, JsPromiseReaction reaction, JsValue argument)
    {
        var handler = reaction.Handler;

        if (!handler.IsObject || !handler.AsObject().IsCallable)
        {
            if (reaction.OnFulfil)
            {
                PromiseResolveWith(engine, reaction.Derived, argument);
            }
            else
            {
                PromiseSettle(engine, reaction.Derived, argument, JsPromiseState.Rejected);
            }

            return;
        }

        try
        {
            var produced = engine.Call(handler, JsValue.Undefined, [argument]);
            PromiseResolveWith(engine, reaction.Derived, produced);
        }
        catch (JsThrow thrown)
        {
            PromiseSettle(engine, reaction.Derived, thrown.Value, JsPromiseState.Rejected);
        }
    }

    /// <summary>
    /// Attaches two handlers and answers the derived promise, scheduling at once when the receiver
    /// has already settled.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8BE273
    // Broiler-Human:        PENDING
    private JsPromiseObject PromiseThen(
        JsEngine engine, JsPromiseObject promise, JsValue onFulfil, JsValue onReject)
    {
        var derived = new JsPromiseObject(PromisePrototype);
        var whenFulfilled = new JsPromiseReaction(derived, onFulfil, true);
        var whenRejected = new JsPromiseReaction(derived, onReject, false);

        engine.Charge(2);

        switch (promise.State)
        {
            case JsPromiseState.Pending:
                promise.FulfilReactions.Add(whenFulfilled);
                promise.RejectReactions.Add(whenRejected);
                engine.Retain(PromiseReactionBytes);
                break;

            case JsPromiseState.Fulfilled:
                PromiseSchedule(engine, whenFulfilled, promise.Result);
                break;

            default:
                PromiseSchedule(engine, whenRejected, promise.Result);
                break;
        }

        return derived;
    }

    /// <summary>
    /// The specification's <c>PromiseResolve</c>: a promise of this realm is handed straight back,
    /// anything else is wrapped.
    /// </summary>
    /// <remarks>
    /// The identity test is on the value's <c>constructor</c> and not merely on its C# type,
    /// because the specification's is: a promise whose <c>constructor</c> was reassigned is no
    /// longer one this operation may pass through, and wrapping it instead is both what a
    /// conforming engine does and the conservative answer.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=3E8D44
    // Broiler-Human:        PENDING
    private JsPromiseObject PromiseResolveValue(JsEngine engine, JsValue value)
    {
        if (value.AsObjectOrNull() is JsPromiseObject already &&
            ReferenceEquals(
                engine.GetProperty(value, "constructor").AsObjectOrNull(), PromiseConstructor))
        {
            return already;
        }

        var made = new JsPromiseObject(PromisePrototype);
        engine.Charge(2);
        PromiseResolveWith(engine, made, value);
        return made;
    }

    /// <summary>
    /// The four combinators, which differ only in what one element's settlement does.
    /// </summary>
    /// <remarks>
    /// <para>
    /// They are one method because they are one algorithm: read the array-like, reserve a result
    /// slot per element, resolve every element to a promise, attach a pair of hooks, and count
    /// down. Writing them four times would mean four copies of the counting - which is the part
    /// with the off-by-one in it - and four chances for one of them to settle on the wrong tick.
    /// </para>
    /// <para>
    /// <b>The counter starts one HIGH and is decremented after the loop.</b> Without that, an input
    /// whose first element was already settled would drive the count to zero mid-walk and settle
    /// the result before the remaining elements were even attached. The extra count is released
    /// once, after every element is in flight, and it is what makes the empty-input cases land on
    /// the right answer as well.
    /// </para>
    /// <para>
    /// <b>A failure to read the input REJECTS rather than throws.</b> The specification's
    /// <c>IfAbruptRejectPromise</c> says a combinator always answers a promise, so a caller may
    /// write <c>Promise.all(x).catch(h)</c> without also wrapping the call in a <c>try</c>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=470C14
    // Broiler-Human:        PENDING
    private JsValue PromiseCombine(JsEngine engine, JsValue source, JsPromiseCombination shape)
    {
        var result = new JsPromiseObject(PromisePrototype);

        try
        {
            var items = CollectionElements(engine, source);
            var collected = NewArray();
            var remaining = items.Count + 1;

            for (var at = 0; at < items.Count; at++)
            {
                engine.Charge(2);
                collected.Push(JsValue.Undefined);
            }

            for (var at = 0; at < items.Count; at++)
            {
                var slot = (uint)at;
                var element = PromiseResolveValue(engine, items[at]);

                var onFulfil = Native("", 1, (inner, thisValue, arguments) =>
                {
                    var value = ArgOfPromise(arguments, 0);

                    switch (shape)
                    {
                        case JsPromiseCombination.All:
                            collected.SetIndex(slot, value);
                            remaining--;

                            if (remaining == 0)
                            {
                                PromiseSettle(
                                    inner,
                                    result,
                                    JsValue.Object(collected),
                                    JsPromiseState.Fulfilled);
                            }

                            break;

                        case JsPromiseCombination.AllSettled:
                            collected.SetIndex(slot, PromiseOutcome(inner, true, value));
                            remaining--;

                            if (remaining == 0)
                            {
                                PromiseSettle(
                                    inner,
                                    result,
                                    JsValue.Object(collected),
                                    JsPromiseState.Fulfilled);
                            }

                            break;

                        default:
                            PromiseSettle(inner, result, value, JsPromiseState.Fulfilled);
                            break;
                    }

                    return JsValue.Undefined;
                });

                var onReject = Native("", 1, (inner, thisValue, arguments) =>
                {
                    var reason = ArgOfPromise(arguments, 0);

                    switch (shape)
                    {
                        case JsPromiseCombination.AllSettled:
                            collected.SetIndex(slot, PromiseOutcome(inner, false, reason));
                            remaining--;

                            if (remaining == 0)
                            {
                                PromiseSettle(
                                    inner,
                                    result,
                                    JsValue.Object(collected),
                                    JsPromiseState.Fulfilled);
                            }

                            break;

                        case JsPromiseCombination.Any:
                            collected.SetIndex(slot, reason);
                            remaining--;

                            if (remaining == 0)
                            {
                                PromiseSettle(
                                    inner,
                                    result,
                                    PromiseAggregate(collected),
                                    JsPromiseState.Rejected);
                            }

                            break;

                        default:
                            PromiseSettle(inner, result, reason, JsPromiseState.Rejected);
                            break;
                    }

                    return JsValue.Undefined;
                });

                _ = PromiseThen(engine, element, JsValue.Object(onFulfil), JsValue.Object(onReject));
            }

            remaining--;

            if (remaining == 0)
            {
                switch (shape)
                {
                    case JsPromiseCombination.All:
                    case JsPromiseCombination.AllSettled:
                        PromiseSettle(
                            engine, result, JsValue.Object(collected), JsPromiseState.Fulfilled);

                        break;

                    case JsPromiseCombination.Any:
                        PromiseSettle(
                            engine, result, PromiseAggregate(collected), JsPromiseState.Rejected);

                        break;

                    default:
                        break;
                }
            }
        }
        catch (JsThrow thrown)
        {
            PromiseSettle(engine, result, thrown.Value, JsPromiseState.Rejected);
        }

        return JsValue.Object(result);
    }

    /// <summary>One <c>allSettled</c> record.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=54A1AA
    // Broiler-Human:        PENDING
    private JsValue PromiseOutcome(JsEngine engine, bool fulfilled, JsValue value)
    {
        var record = new JsObject(ObjectPrototype);
        engine.Charge(4);

        record.DefineOrdinary("status", JsValue.String(fulfilled ? "fulfilled" : "rejected"));
        record.DefineOrdinary(fulfilled ? "value" : "reason", value);
        return JsValue.Object(record);
    }

    /// <summary>
    /// The error <c>Promise.any</c> rejects with when every element rejected.
    /// </summary>
    /// <remarks>
    /// <b>There is no <c>AggregateError</c> CONSTRUCTOR in this realm, so this is an Error shaped
    /// like one.</b> It inherits from <c>Error.prototype</c>, carries an own <c>name</c> of
    /// <c>"AggregateError"</c>, the specification's message, and the <c>errors</c> array in input
    /// order. Everything a program reads off the value - <c>name</c>, <c>message</c>,
    /// <c>errors</c>, <c>instanceof Error</c> - answers as it would elsewhere; the one thing that
    /// does not work is <c>instanceof AggregateError</c>, because the global is absent and naming
    /// it is a <c>ReferenceError</c>. Adding the constructor belongs with the Error intrinsics in
    /// <c>JsRealm.Error.cs</c> rather than here, where it would be a second, divergent Error
    /// hierarchy built by the promise code.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=D365AF
    // Broiler-Human:        PENDING
    private JsValue PromiseAggregate(JsArray errors)
    {
        var error = new JsObject(ErrorPrototype, "Error");

        error.DefineBuiltIn("name", JsValue.String("AggregateError"));
        error.DefineBuiltIn("message", JsValue.String("All promises were rejected"));
        error.DefineBuiltIn("errors", JsValue.Object(errors));
        return JsValue.Object(error);
    }
}

/// <summary>Which combinator a <c>PromiseCombine</c> walk is performing.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=35DF05
// Broiler-Human:        PENDING
internal enum JsPromiseCombination : byte
{
    /// <summary>Fulfil with every value, or reject with the first reason.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8BA425
    // Broiler-Human:        PENDING
    All = 0,

    /// <summary>Fulfil with one outcome record per element, never reject.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=EC637A
    // Broiler-Human:        PENDING
    AllSettled = 1,

    /// <summary>Adopt the first settlement of either kind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6CF65A
    // Broiler-Human:        PENDING
    Race = 2,

    /// <summary>Fulfil with the first value, or reject with every reason at once.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6AE2A6
    // Broiler-Human:        PENDING
    Any = 3,
}
