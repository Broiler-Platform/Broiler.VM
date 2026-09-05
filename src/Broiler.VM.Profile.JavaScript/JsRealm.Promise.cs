// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   28
// Annotated:        28/28
// Exempt:           6
// Human-reviewed:   0/28
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  4/10 max
// Unverified:       28
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8E809E
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

        SpeciesGetter(PromiseConstructor);

        SetupPromiseReactions();
        SetupPromiseCombinators();
    }

    /// <summary><c>then</c>, <c>catch</c> and <c>finally</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6CCEA2
    // Broiler-Human:        PENDING
    private void SetupPromiseReactions()
    {
        PromisePrototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Promise"), JsPropertyAttributes.Configurable));


        // THE RESULT IS BUILT BY THE SPECIES AND NOT BY THIS REALM'S `Promise`. A subclass that
        // declares `static get [Symbol.species]` decides what `p.then(f)` answers, which is the one
        // hook the language gives a library that wants its own promise type to survive a chain.
        // Where the species IS the intrinsic - which is nearly always - the internal path is taken
        // and no guest constructor runs.
        Method(PromisePrototype, "then", 2, (engine, thisValue, arguments) =>
        {
            var promise = PromiseThisPromise(engine, thisValue, "then");
            var species = PromiseSpeciesOf(engine, thisValue);
            var onFulfil = ArgOfPromise(arguments, 0);
            var onReject = ArgOfPromise(arguments, 1);

            if (ReferenceEquals(species.AsObjectOrNull(), PromiseConstructor))
            {
                return JsValue.Object(PromiseThen(engine, promise, onFulfil, onReject));
            }

            var capability = PromiseCapability(engine, species);

            PromiseAttach(
                engine,
                promise,
                new JsPromiseReaction(capability.Resolve, capability.Reject, onFulfil, true),
                new JsPromiseReaction(capability.Resolve, capability.Reject, onReject, false));

            return capability.Promise;
        });

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
            // THE SPECIES IS READ ONCE, HERE, and the handlers resolve through it. `finally` is
            // defined in terms of `PromiseResolve(C, result)`, so a subclass sees its own
            // constructor asked for the intermediate promise - which is what the suite counts when
            // it counts how many promises a `finally` built.
            var species = PromiseSpeciesOf(engine, thisValue);

            var pass = Native("", 1, (inner, ignored, passed) =>
            {
                var value = ArgOfPromise(passed, 0);
                var produced = inner.Call(onFinally, JsValue.Undefined, []);
                var waited = PromiseResolveThrough(inner, species, produced);

                return inner.Call(
                    inner.GetProperty(waited, "then"),
                    waited,
                    [JsValue.Object(Native("", 0, (deeper, alsoIgnored, none) => value))]);
            });

            var rethrow = Native("", 1, (inner, ignored, passed) =>
            {
                var reason = ArgOfPromise(passed, 0);
                var produced = inner.Call(onFinally, JsValue.Undefined, []);
                var waited = PromiseResolveThrough(inner, species, produced);

                return inner.Call(
                    inner.GetProperty(waited, "then"),
                    waited,
                    [
                        JsValue.Object(Native("", 0, (deeper, alsoIgnored, none) =>
                            throw new JsThrow(reason, deeper.Render(reason)))),
                    ]);
            });

            return engine.Call(
                thenFunction, thisValue, [JsValue.Object(pass), JsValue.Object(rethrow)]);
        });
    }

    /// <summary><c>resolve</c>, <c>reject</c>, <c>all</c>, <c>allSettled</c>, <c>race</c>, <c>any</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=3B9906
    // Broiler-Human:        PENDING
    private void SetupPromiseCombinators()
    {
        // THE RECEIVER IS THE CONSTRUCTOR, in these two and in the four below. `Promise.resolve`
        // called on a subclass answers an instance of the subclass, and called on something that is
        // not a constructor is a TypeError rather than a Promise - which is the difference between
        // a static that is inherited and one that is merely reachable.
        Method(PromiseConstructor, "resolve", 1, (engine, thisValue, arguments) =>
            PromiseResolveThrough(engine, thisValue, ArgOfPromise(arguments, 0)));

        Method(PromiseConstructor, "reject", 1, (engine, thisValue, arguments) =>
        {
            var capability = PromiseCapability(engine, thisValue);
            engine.Call(capability.Reject, JsValue.Undefined, [ArgOfPromise(arguments, 0)]);
            return capability.Promise;
        });

        // `try` RUNS ITS ARGUMENT NOW AND WRAPS WHATEVER HAPPENS, which is the one combinator that
        // takes a function rather than a list: it exists so that a synchronous throw and an
        // asynchronous rejection reach the same `catch`, without the caller writing the
        // `new Promise(r => r(f()))` dance that does it by accident.
        Method(PromiseConstructor, "try", 1, (engine, thisValue, arguments) =>
        {
            var capability = PromiseCapability(engine, thisValue);
            var callback = ArgOfPromise(arguments, 0);
            var rest = arguments.Length > 1 ? arguments[1..] : System.Array.Empty<JsValue>();

            try
            {
                if (!callback.IsObject || !callback.AsObject().IsCallable)
                {
                    throw engine.Error("TypeError", "Promise.try: the argument is not a function");
                }

                engine.Call(
                    capability.Resolve,
                    JsValue.Undefined,
                    [engine.Call(callback, JsValue.Undefined, rest)]);
            }
            catch (JsThrow thrown)
            {
                engine.Call(capability.Reject, JsValue.Undefined, [thrown.Value]);
            }

            return capability.Promise;
        });

        // THE CAPABILITY, HANDED TO THE PROGRAM. Before this existed a program that wanted to
        // settle a promise from outside wrote `let r; const p = new Promise(x => r = x)`, which
        // works and reads like a trick; this is the same three values with a name.
        Method(PromiseConstructor, "withResolvers", 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var capability = PromiseCapability(engine, thisValue);
            var record = new JsObject(ObjectPrototype);

            record.DefineOrdinary("promise", capability.Promise);
            record.DefineOrdinary("resolve", capability.Resolve);
            record.DefineOrdinary("reject", capability.Reject);
            return JsValue.Object(record);
        });

        // `all` RESOLVES WITH A DENSE ARRAY IN THE INPUT'S ORDER AND NOT IN SETTLEMENT ORDER, which
        // is why the slots are reserved before any element is waited on: the index a value belongs
        // at is decided when the walk reaches it and not when its promise settles.
        Method(PromiseConstructor, "all", 1, (engine, thisValue, arguments) =>
            PromiseCombine(engine, thisValue, ArgOfPromise(arguments, 0), JsPromiseCombination.All));

        Method(PromiseConstructor, "allSettled", 1, (engine, thisValue, arguments) =>
            PromiseCombine(engine, thisValue, ArgOfPromise(arguments, 0), JsPromiseCombination.AllSettled));

        // `race` OVER AN EMPTY ARRAY IS PENDING FOR EVER, and that is correct rather than an
        // oversight: there is no first settlement to adopt, so there is nothing the result could
        // settle to. `any` over an empty array rejects instead, because "none of them succeeded" is
        // a fact an empty set does establish.
        Method(PromiseConstructor, "race", 1, (engine, thisValue, arguments) =>
            PromiseCombine(engine, thisValue, ArgOfPromise(arguments, 0), JsPromiseCombination.Race));

        Method(PromiseConstructor, "any", 1, (engine, thisValue, arguments) =>
            PromiseCombine(engine, thisValue, ArgOfPromise(arguments, 0), JsPromiseCombination.Any));
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

    /// <summary>The specification's <c>SpeciesConstructor(promise, %Promise%)</c>.</summary>
    /// <remarks>
    /// <b>Two reads and two defaults.</b> A receiver with no <c>constructor</c> answers the
    /// intrinsic, and a constructor whose <c>Symbol.species</c> is <c>null</c> or <c>undefined</c>
    /// answers the intrinsic too - so an object that has deliberately opted out gets the ordinary
    /// promise rather than an error. Anything else must be a constructor, because the next thing
    /// that happens to it is a <c>new</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=1B9F23
    // Broiler-Human:        PENDING
    private JsValue PromiseSpeciesOf(JsEngine engine, JsValue receiver)
    {
        var constructor = engine.GetProperty(receiver, "constructor");

        if (constructor.Type == JsType.Undefined)
        {
            return JsValue.Object(PromiseConstructor);
        }

        if (!constructor.IsObject)
        {
            throw engine.Error("TypeError", "the receiver's `constructor` is not an object");
        }

        var species = engine.GetSymbol(constructor, SpeciesSymbol);

        if (species.IsNullish)
        {
            return JsValue.Object(PromiseConstructor);
        }

        if (!species.IsObject || !species.AsObject().IsConstructor)
        {
            throw engine.Error("TypeError", "the species is not a constructor");
        }

        return species;
    }

    /// <summary>Attaches a pair of reactions, scheduling at once when the promise has settled.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=E5B145
    // Broiler-Human:        PENDING
    private void PromiseAttach(
        JsEngine engine,
        JsPromiseObject promise,
        JsPromiseReaction whenFulfilled,
        JsPromiseReaction whenRejected)
    {
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
    }

    /// <summary>A promise and the two functions that settle it, as the language pairs them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=609421
    // Broiler-Human:        PENDING
    private readonly record struct JsPromiseCapability(JsValue Promise, JsValue Resolve, JsValue Reject);

    /// <summary>The specification's <c>NewPromiseCapability</c>, over any constructor.</summary>
    /// <remarks>
    /// <para>
    /// <b>Every static on <c>Promise</c> builds its answer through THIS and not through the
    /// intrinsic.</b> The receiver decides what is constructed - <c>Promise.all.call(C, xs)</c>
    /// answers a <c>C</c> - which is what makes the combinators inheritable by a subclass and what
    /// the pinned suite spends a large part of its Promise subtree checking. Reaching for
    /// <c>%Promise%</c> instead would answer the right VALUE with the wrong object, which is the
    /// kind of wrong that only shows up in somebody else's library.
    /// </para>
    /// <para>
    /// <b>The executor is called synchronously by the constructor and its two arguments are
    /// captured here.</b> A constructor that does not call it, or calls it twice, or hands it
    /// something that is not callable, is a <c>TypeError</c> - which is the check that makes a
    /// broken subclass fail at the point it broke rather than at the settlement nobody can trace
    /// back.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=55A6B9
    // Broiler-Human:        PENDING
    private JsPromiseCapability PromiseCapability(JsEngine engine, JsValue constructor)
    {
        if (!constructor.IsObject || !constructor.AsObject().IsConstructor)
        {
            throw engine.Error("TypeError", "PromiseCapability requires a constructor");
        }

        var resolve = JsValue.Undefined;
        var reject = JsValue.Undefined;

        var executor = Native("", 2, (inner, thisValue, arguments) =>
        {
            _ = thisValue;

            if (resolve.Type != JsType.Undefined || reject.Type != JsType.Undefined)
            {
                return inner.ThrowTypeError("the promise capability has already been settled");
            }

            resolve = ArgOfPromise(arguments, 0);
            reject = ArgOfPromise(arguments, 1);
            return JsValue.Undefined;
        });

        engine.Retain(PromiseReactionBytes);
        var promise = engine.Construct(constructor, [JsValue.Object(executor)], constructor);

        if (!resolve.IsObject || !resolve.AsObject().IsCallable ||
            !reject.IsObject || !reject.AsObject().IsCallable)
        {
            throw engine.Error("TypeError", "the promise constructor did not supply both resolvers");
        }

        return new JsPromiseCapability(promise, resolve, reject);
    }

    /// <summary>The specification's <c>PromiseResolve</c>, over any constructor.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=AE4023
    // Broiler-Human:        PENDING
    private JsValue PromiseResolveThrough(JsEngine engine, JsValue constructor, JsValue value)
    {
        // AN ALREADY-PROMISE OF THE SAME CONSTRUCTOR IS ANSWERED AS ITSELF, which is the identity
        // `Promise.resolve(p) === p` every program relies on. The test is on the value's own
        // `constructor` property rather than on its shape, because that is what the language reads
        // and a program may have changed it.
        if (value.AsObjectOrNull() is JsPromiseObject &&
            engine.GetProperty(value, "constructor").IsObject &&
            ReferenceEquals(
                engine.GetProperty(value, "constructor").AsObject(), constructor.AsObjectOrNull()))
        {
            return value;
        }

        var capability = PromiseCapability(engine, constructor);
        engine.Call(capability.Resolve, JsValue.Undefined, [value]);
        return capability.Promise;
    }

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6E50D5
    // Broiler-Human:        PENDING
    private void PromiseRun(JsEngine engine, JsPromiseReaction reaction, JsValue argument)
    {
        var handler = reaction.Handler;

        if (!handler.IsObject || !handler.AsObject().IsCallable)
        {
            if (reaction.OnFulfil)
            {
                PromiseReactionResolve(engine, reaction, argument);
            }
            else
            {
                PromiseReactionReject(engine, reaction, argument);
            }

            return;
        }

        try
        {
            PromiseReactionResolve(
                engine, reaction, engine.Call(handler, JsValue.Undefined, [argument]));
        }
        catch (JsThrow thrown)
        {
            PromiseReactionReject(engine, reaction, thrown.Value);
        }
    }

    /// <summary>Settles what a reaction owes, whichever of the two shapes it holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9BEC4D
    // Broiler-Human:        PENDING
    private void PromiseReactionResolve(
        JsEngine engine, JsPromiseReaction reaction, JsValue value)
    {
        if (reaction.Derived is { } derived)
        {
            PromiseResolveWith(engine, derived, value);
            return;
        }

        engine.Call(reaction.Resolve, JsValue.Undefined, [value]);
    }

    /// <summary>The rejecting half of the same.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F9E827
    // Broiler-Human:        PENDING
    private void PromiseReactionReject(
        JsEngine engine, JsPromiseReaction reaction, JsValue reason)
    {
        if (reaction.Derived is { } derived)
        {
            PromiseSettle(engine, derived, reason, JsPromiseState.Rejected);
            return;
        }

        engine.Call(reaction.Reject, JsValue.Undefined, [reason]);
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B41595
    // Broiler-Human:        PENDING
    private JsValue PromiseCombine(
        JsEngine engine, JsValue constructor, JsValue source, JsPromiseCombination shape)
    {
        // THE CAPABILITY IS BUILT BEFORE ANYTHING CAN GO WRONG, and a failure to build it THROWS
        // where every later failure rejects. That asymmetry is the specification's: without a
        // capability there is no promise to reject with, so there is nothing to answer but an
        // exception.
        var capability = PromiseCapability(engine, constructor);

        try
        {
            // THE ELEMENTS ARE RESOLVED THROUGH THE RECEIVER'S OWN `resolve` and waited on through
            // each element's own `then`, both read as properties and called as functions. A program
            // that overrides either sees its override used, and the suite counts those calls.
            var promiseResolve = engine.GetProperty(constructor, "resolve");

            if (!promiseResolve.IsObject || !promiseResolve.AsObject().IsCallable)
            {
                throw engine.Error("TypeError", "the constructor's `resolve` is not a function");
            }

            var collected = NewArray();
            var remaining = 1;
            var reached = 0;

            // ONE ELEMENT AT A TIME, and the iterator is CLOSED if anything in the body throws.
            // Reading the whole iterable first never returns over an iterator that does not end,
            // and the pinned suite's `invoke-then-error-close` family is exactly that: an infinite
            // iterator whose walk is supposed to stop at the first element whose `then` throws.
            CollectionEach(engine, source, item =>
            {
                var slot = (uint)reached;
                reached++;
                remaining++;
                engine.Charge(2);
                collected.Push(JsValue.Undefined);
                var element = engine.Call(promiseResolve, constructor, [item]);

                // ONE ELEMENT SETTLES ONCE, however many times its `then` calls back. A thenable
                // may call its `onFulfilled` twice - the language does not stop it - and without a
                // latch the second call decrements the counter again and settles the result while
                // elements are still outstanding. The latch is per element and shared by that
                // element's two handlers, which is what makes a fulfil after a reject a no-op too.
                var settled = new JsPromiseLatch();

                var onFulfil = Native("", 1, (inner, thisValue, arguments) =>
                {
                    var value = ArgOfPromise(arguments, 0);

                    if (settled.Latched)
                    {
                        return JsValue.Undefined;
                    }

                    settled.Latched = true;

                    switch (shape)
                    {
                        case JsPromiseCombination.All:
                            collected.SetIndex(slot, value);
                            remaining--;

                            if (remaining == 0)
                            {
                                inner.Call(
                                    capability.Resolve,
                                    JsValue.Undefined,
                                    [JsValue.Object(collected)]);
                            }

                            break;

                        case JsPromiseCombination.AllSettled:
                            collected.SetIndex(slot, PromiseOutcome(inner, true, value));
                            remaining--;

                            if (remaining == 0)
                            {
                                inner.Call(
                                    capability.Resolve,
                                    JsValue.Undefined,
                                    [JsValue.Object(collected)]);
                            }

                            break;

                        default:
                            inner.Call(capability.Resolve, JsValue.Undefined, [value]);
                            break;
                    }

                    return JsValue.Undefined;
                });

                var onReject = Native("", 1, (inner, thisValue, arguments) =>
                {
                    var reason = ArgOfPromise(arguments, 0);

                    if (settled.Latched)
                    {
                        return JsValue.Undefined;
                    }

                    settled.Latched = true;

                    switch (shape)
                    {
                        case JsPromiseCombination.AllSettled:
                            collected.SetIndex(slot, PromiseOutcome(inner, false, reason));
                            remaining--;

                            if (remaining == 0)
                            {
                                inner.Call(
                                    capability.Resolve,
                                    JsValue.Undefined,
                                    [JsValue.Object(collected)]);
                            }

                            break;

                        case JsPromiseCombination.Any:
                            collected.SetIndex(slot, reason);
                            remaining--;

                            if (remaining == 0)
                            {
                                inner.Call(
                                    capability.Reject,
                                    JsValue.Undefined,
                                    [PromiseAggregate(inner, collected)]);
                            }

                            break;

                        default:
                            inner.Call(capability.Reject, JsValue.Undefined, [reason]);
                            break;
                    }

                    return JsValue.Undefined;
                });

                var then = engine.GetProperty(element, "then");

                if (!then.IsObject || !then.AsObject().IsCallable)
                {
                    throw engine.Error("TypeError", "an element's `then` is not a function");
                }

                // `all` AND `race` HAND EVERY ELEMENT THE CAPABILITY'S OWN `reject`, one function
                // object for the whole call, and `race` hands them its `resolve` too. That is
                // observable - a test collects the handlers and compares them - and it follows from
                // the algorithm rather than being a saving: there is nothing per-element for those
                // sides to remember, so the specification passes the capability's function itself.
                var rejectSide = shape is JsPromiseCombination.All or JsPromiseCombination.Race
                    ? capability.Reject
                    : JsValue.Object(onReject);

                var fulfilSide = shape == JsPromiseCombination.Race
                    ? capability.Resolve
                    : JsValue.Object(onFulfil);

                _ = engine.Call(then, element, [fulfilSide, rejectSide]);
            });

            remaining--;

            if (remaining == 0)
            {
                switch (shape)
                {
                    case JsPromiseCombination.All:
                    case JsPromiseCombination.AllSettled:
                        engine.Call(
                            capability.Resolve, JsValue.Undefined, [JsValue.Object(collected)]);

                        break;

                    case JsPromiseCombination.Any:
                        engine.Call(
                            capability.Reject, JsValue.Undefined, [PromiseAggregate(engine, collected)]);

                        break;

                    default:
                        break;
                }
            }
        }
        catch (JsThrow thrown)
        {
            // `IfAbruptRejectPromise`: a combinator always ANSWERS a promise, so a caller may write
            // `Promise.all(x).catch(h)` without also wrapping the call in a `try`.
            engine.Call(capability.Reject, JsValue.Undefined, [thrown.Value]);
        }

        return capability.Promise;
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=DA11DE
    // Broiler-Human:        PENDING
    private JsValue PromiseAggregate(JsEngine engine, JsArray errors)
    {
        // THE REAL CONSTRUCTOR AND NOT AN OBJECT SHAPED LIKE ONE. This built an ordinary error
        // carrying the three properties, because the realm had no `AggregateError` when it was
        // written; a program testing `error instanceof AggregateError` - which is the whole point
        // of the type - got false, and `Object.getPrototypeOf(error)` answered `Error.prototype`.
        var constructor = ErrorConstructors["AggregateError"];

        return engine.Construct(
            JsValue.Object(constructor),
            [JsValue.Object(errors), JsValue.String("All promises were rejected")],
            JsValue.Object(constructor));
    }

    // ---- what an async function needs from this file ------------------------------------------
    //
    // THREE METHODS AND NOT A REIMPLEMENTATION. An async function's promise is an ordinary promise
    // of this realm: it settles through the same `PromiseResolveWith` and `PromiseSettle` a
    // `resolve` callback goes through, and its `await` attaches through the same `PromiseThen`
    // `p.then(f)` goes through. That is what makes an `await` and a `then` on the same promise
    // interleave in the order the specification fixes rather than in an order this profile
    // invented - and a second settle path here would have been exactly the way to get that wrong.

    /// <summary>The pending promise a call of an async function answers with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B3E5EC
    // Broiler-Human:        PENDING
    internal JsPromiseObject NewAsyncPromise() => new(PromisePrototype);

    /// <summary>
    /// Settles an async call's promise with what its body completed with.
    /// </summary>
    /// <remarks>
    /// <b>A RETURN resolves and does not fulfil, and the difference is a whole turn.</b>
    /// <c>return p</c> from an async function where <c>p</c> is a promise adopts <c>p</c>'s
    /// eventual state through the resolve procedure, so a caller awaiting the outer promise waits
    /// for the inner one; fulfilling with the promise as a VALUE would have handed the caller a
    /// promise where the language hands it the promise's result. A THROW rejects with the reason
    /// exactly as it stands, because a rejection reason is never adopted.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=316343
    // Broiler-Human:        PENDING
    internal void SettleAsyncPromise(
        JsEngine engine, JsPromiseObject promise, JsValue value, bool rejected)
    {
        if (rejected)
        {
            PromiseSettle(engine, promise, value, JsPromiseState.Rejected);
            return;
        }

        PromiseResolveWith(engine, promise, value);
    }

    /// <summary>
    /// Performs <c>Await</c>: resolves <paramref name="value"/> the way <c>Promise.resolve</c>
    /// does and registers <paramref name="resume"/> to run on whichever side it settles.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It goes through <c>PromiseResolve</c> and not through a shortcut for the non-promise
    /// case, and that is the ordering the whole family is graded on.</b> <c>await 0</c> is not
    /// free: it makes an already-fulfilled promise and attaches a reaction to it, which the queue
    /// runs on the next turn - so <c>async function f(){ print(1); await 0; print(3); } f();
    /// print(2);</c> prints 1, 2, 3. An implementation that noticed <c>0</c> was not a thenable
    /// and carried straight on would print 1, 3, 2 and would be the Zalgo defect
    /// <see cref="PromiseSchedule"/> exists to make uninspectable.
    /// </para>
    /// <para>
    /// <b>The two handlers are native functions rather than a reaction shape of their own</b>, so
    /// the reaction the queue runs is the same record a <c>then</c> makes and the ordering between
    /// an <c>await</c> and a <c>then</c> on one promise is decided by one queue and one list.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=DB2999
    // Broiler-Falsified-If: an `await` of a value that is not a promise continues without yielding to the job queue
    // Broiler-Human:        PENDING
    internal void AwaitOn(
        JsEngine engine, JsValue value, System.Action<JsEngine, JsValue, bool> resume)
    {
        var awaited = PromiseResolveValue(engine, value);

        var onFulfil = JsValue.Object(Native("", 1, (inner, thisValue, arguments) =>
        {
            resume(inner, ArgOfPromise(arguments, 0), false);
            return JsValue.Undefined;
        }));

        var onReject = JsValue.Object(Native("", 1, (inner, thisValue, arguments) =>
        {
            resume(inner, ArgOfPromise(arguments, 0), true);
            return JsValue.Undefined;
        }));

        _ = PromiseThen(engine, awaited, onFulfil, onReject);
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
