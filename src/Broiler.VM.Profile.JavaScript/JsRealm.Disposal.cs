// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   36
// Annotated:        36/36
// Exempt:           21
// Human-reviewed:   0/36
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  3/10 max
// Unverified:       36
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The runtime half of explicit resource management: <c>DisposableStack</c>,
/// <c>AsyncDisposableStack</c>, and the two disposers the iterator prototypes carry.
/// </summary>
/// <remarks>
/// <para>
/// <b>None of this is syntax.</b> <c>using</c> and <c>await using</c> are not parsed here, and
/// nothing in this file makes them so; what is here is the part of the proposal a program can reach
/// through ordinary calls. The pinned edition has none of it (decision JSD-0019 records that), so the
/// oracle is the pinned Test262 subtree the suite files under <c>explicit-resource-management</c>.
/// </para>
/// <para>
/// <b>A disposal method is captured when the resource is REGISTERED, not when the stack is
/// disposed.</b> That is the specification's <c>CreateDisposableResource</c>, and it is observable:
/// a program that replaces <c>obj[Symbol.dispose]</c> after <c>stack.use(obj)</c> still gets the
/// method it registered, and a getter on the member runs once, at <c>use</c>.
/// </para>
/// <para>
/// <b>A stack is marked disposed BEFORE its first entry runs, and the entries are detached from it
/// at the same moment.</b> That is what makes reentry harmless: a disposer that calls
/// <c>dispose()</c> again finds a disposed stack and returns, one that calls <c>use</c> or
/// <c>move</c> gets the <c>ReferenceError</c> a disposed stack answers, and no entry can be reached
/// twice because the list it lived in is no longer the stack's.
/// </para>
/// <para>
/// <b>Unwinding is metered and never catches a stop.</b> Each entry is charged as it is unwound and
/// each registration as it is made, and only a guest <c>throw</c> is folded into the completion; an
/// allowance that runs out or a cancellation mid-unwind ends the run exactly as it would anywhere
/// else, with the remaining entries not run. The asynchronous form awaits through the realm's own
/// <c>Await</c>, so each await is a promise reaction on the ordinary job queue and nothing here
/// blocks or runs a guest function outside a normal turn.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>What one registered resource is charged against the live-bytes ceiling.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DDFBEC
    // Broiler-Human:        PENDING
    private const ulong DisposableResourceBytes = 64;

    /// <summary><c>DisposableStack.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4E2698
    // Broiler-Human:        PENDING
    internal JsObject DisposableStackPrototype { get; private set; } = null!;

    /// <summary><c>AsyncDisposableStack.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=215586
    // Broiler-Human:        PENDING
    internal JsObject AsyncDisposableStackPrototype { get; private set; } = null!;

    /// <summary>Builds both stacks and the two iterator disposers.</summary>
    /// <remarks>
    /// It runs after the promise intrinsics and after <c>%AsyncIteratorPrototype%</c> exist, because
    /// the asynchronous stack settles through the first and the second is where one disposer goes.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4C5764
    // Broiler-Human:        PENDING
    private void SetupDisposal()
    {
        DisposableStackPrototype = new JsObject(ObjectPrototype);
        AsyncDisposableStackPrototype = new JsObject(ObjectPrototype);

        SetupDisposableStack();
        SetupAsyncDisposableStack();
        SetupIteratorDisposers();
    }

    /// <summary>Builds <c>DisposableStack</c> and its prototype.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=EF6A55
    // Broiler-Human:        PENDING
    private void SetupDisposableStack()
    {
        var prototype = DisposableStackPrototype;

        // THE INSTANCE IS BUILT ON THE INTRINSIC PROTOTYPE and the engine re-points it at
        // `new.target.prototype` afterwards when a subclass is what was newed, which is how every
        // built-in constructor here meets `OrdinaryCreateFromConstructor`.
        _ = Constructor(
            "DisposableStack",
            0,
            prototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor DisposableStack requires 'new'"),
            (engine, thisValue, arguments) =>
            {
                engine.Charge(2);
                return JsValue.Object(new JsDisposableStack(prototype, asynchronous: false));
            });

        DisposalAccessor(prototype, asynchronous: false);

        Method(prototype, "use", 1, (engine, thisValue, arguments) =>
        {
            var stack = DisposalThis(engine, thisValue, asynchronous: false, "use");
            var value = DisposalArg(arguments, 0);
            DisposalAdd(engine, stack, value);
            return value;
        });

        Method(prototype, "adopt", 2, (engine, thisValue, arguments) =>
        {
            var stack = DisposalThis(engine, thisValue, asynchronous: false, "adopt");
            var value = DisposalArg(arguments, 0);
            DisposalAdopt(engine, stack, value, DisposalArg(arguments, 1), "adopt");
            return value;
        });

        Method(prototype, "defer", 1, (engine, thisValue, arguments) =>
        {
            var stack = DisposalThis(engine, thisValue, asynchronous: false, "defer");
            DisposalDefer(engine, stack, DisposalArg(arguments, 0), "defer");
            return JsValue.Undefined;
        });

        Method(prototype, "move", 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var stack = DisposalThis(engine, thisValue, asynchronous: false, "move");
            return JsValue.Object(DisposalMove(engine, stack, DisposableStackPrototype));
        });

        var dispose = Native("dispose", 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var stack = DisposalThis(engine, thisValue, asynchronous: false, "dispose");

            if (stack.Disposed)
            {
                return JsValue.Undefined;
            }

            var entries = DisposalDetach(stack);
            var threw = false;
            var completion = JsValue.Undefined;

            for (var at = entries.Count - 1; at >= 0; at--)
            {
                engine.Charge(1);
                var entry = entries[at];

                try
                {
                    _ = engine.Call(entry.Method, entry.Value, []);
                }
                catch (JsThrow thrown)
                {
                    completion = threw
                        ? NewSuppressedError(engine, thrown.Value, completion)
                        : thrown.Value;

                    threw = true;
                }
            }

            if (threw)
            {
                throw new JsThrow(completion, engine.Render(completion));
            }

            return JsValue.Undefined;
        });

        // `[Symbol.dispose]` IS THE SAME FUNCTION OBJECT AS `dispose`, not a second function that
        // behaves the same: the specification says so and test262 compares them by identity.
        prototype.SetOwnProperty(
            "dispose",
            JsProperty.Data(
                JsValue.Object(dispose),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        prototype.SetOwnSymbol(
            DisposeSymbol,
            JsProperty.Data(
                JsValue.Object(dispose),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("DisposableStack"), JsPropertyAttributes.Configurable));
    }

    /// <summary>Builds <c>AsyncDisposableStack</c> and its prototype.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3BB403
    // Broiler-Human:        PENDING
    private void SetupAsyncDisposableStack()
    {
        var prototype = AsyncDisposableStackPrototype;

        _ = Constructor(
            "AsyncDisposableStack",
            0,
            prototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor AsyncDisposableStack requires 'new'"),
            (engine, thisValue, arguments) =>
            {
                engine.Charge(2);
                return JsValue.Object(new JsDisposableStack(prototype, asynchronous: true));
            });

        DisposalAccessor(prototype, asynchronous: true);

        Method(prototype, "use", 1, (engine, thisValue, arguments) =>
        {
            var stack = DisposalThis(engine, thisValue, asynchronous: true, "use");
            var value = DisposalArg(arguments, 0);
            DisposalAdd(engine, stack, value);
            return value;
        });

        Method(prototype, "adopt", 2, (engine, thisValue, arguments) =>
        {
            var stack = DisposalThis(engine, thisValue, asynchronous: true, "adopt");
            var value = DisposalArg(arguments, 0);
            DisposalAdopt(engine, stack, value, DisposalArg(arguments, 1), "adopt");
            return value;
        });

        Method(prototype, "defer", 1, (engine, thisValue, arguments) =>
        {
            var stack = DisposalThis(engine, thisValue, asynchronous: true, "defer");
            DisposalDefer(engine, stack, DisposalArg(arguments, 0), "defer");
            return JsValue.Undefined;
        });

        Method(prototype, "move", 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var stack = DisposalThis(engine, thisValue, asynchronous: true, "move");
            return JsValue.Object(DisposalMove(engine, stack, AsyncDisposableStackPrototype));
        });

        var disposeAsync = Native("disposeAsync", 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var promise = NewAsyncPromise();
            engine.Charge(2);

            // A RECEIVER THAT IS NOT AN ASYNCHRONOUS STACK REJECTS, IT DOES NOT THROW: the method
            // always answers a promise, so `stack.disposeAsync().catch(h)` sees every failure.
            if (thisValue.AsObjectOrNull() is not JsDisposableStack { Asynchronous: true } stack)
            {
                SettleAsyncPromise(
                    engine,
                    promise,
                    CreateError(
                        "TypeError",
                        "AsyncDisposableStack.prototype.disposeAsync called on an incompatible receiver"),
                    rejected: true);

                return JsValue.Object(promise);
            }

            if (stack.Disposed)
            {
                SettleAsyncPromise(engine, promise, JsValue.Undefined, rejected: false);
                return JsValue.Object(promise);
            }

            new JsDisposalRun(this, promise, DisposalDetach(stack)).Continue(engine);
            return JsValue.Object(promise);
        });

        prototype.SetOwnProperty(
            "disposeAsync",
            JsProperty.Data(
                JsValue.Object(disposeAsync),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        prototype.SetOwnSymbol(
            AsyncDisposeSymbol,
            JsProperty.Data(
                JsValue.Object(disposeAsync),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(
                JsValue.String("AsyncDisposableStack"), JsPropertyAttributes.Configurable));
    }

    /// <summary>
    /// Installs <c>%IteratorPrototype%[@@dispose]</c> and
    /// <c>%AsyncIteratorPrototype%[@@asyncDispose]</c>.
    /// </summary>
    /// <remarks>
    /// Both close the iterator through its own <c>return</c> when it has one and do nothing when it
    /// has none. The asynchronous one always answers a promise and discards what <c>return</c>
    /// settled with, so a failure to read or call <c>return</c> is a rejection and a success is
    /// <c>undefined</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0FA021
    // Broiler-Human:        PENDING
    private void SetupIteratorDisposers()
    {
        IteratorPrototype.SetOwnSymbol(
            DisposeSymbol,
            JsProperty.Data(
                JsValue.Object(Native("[Symbol.dispose]", 0, static (engine, thisValue, arguments) =>
                {
                    _ = arguments;
                    var close = engine.GetProperty(thisValue, "return");

                    if (close.IsNullish)
                    {
                        return JsValue.Undefined;
                    }

                    if (!close.IsObject || !close.AsObject().IsCallable)
                    {
                        return engine.ThrowTypeError("the iterator's return is not a function");
                    }

                    _ = engine.Call(close, thisValue, []);
                    return JsValue.Undefined;
                })),
                JsPropertyAttributes.BuiltIn));

        AsyncIteratorPrototype.SetOwnSymbol(
            AsyncDisposeSymbol,
            JsProperty.Data(
                JsValue.Object(Native("[Symbol.asyncDispose]", 0, (engine, thisValue, arguments) =>
                {
                    _ = arguments;
                    var promise = NewAsyncPromise();
                    engine.Charge(2);

                    try
                    {
                        var close = engine.GetProperty(thisValue, "return");

                        if (close.IsNullish)
                        {
                            SettleAsyncPromise(engine, promise, JsValue.Undefined, rejected: false);
                            return JsValue.Object(promise);
                        }

                        if (!close.IsObject || !close.AsObject().IsCallable)
                        {
                            return engine.ThrowTypeError("the iterator's return is not a function");
                        }

                        var result = engine.Call(close, thisValue, [JsValue.Undefined]);

                        // THE RESULT IS AWAITED AND THEN DROPPED. `AwaitOn` is the realm's
                        // `PromiseResolve` followed by a reaction pair, which is the
                        // specification's `PerformPromiseThen` with an unwrap that answers
                        // `undefined`.
                        AwaitOn(engine, result, (inner, value, rejected) =>
                            SettleAsyncPromise(
                                inner, promise, rejected ? value : JsValue.Undefined, rejected));
                    }
                    catch (JsThrow thrown)
                    {
                        SettleAsyncPromise(engine, promise, thrown.Value, rejected: true);
                    }

                    return JsValue.Object(promise);
                })),
                JsPropertyAttributes.BuiltIn));
    }

    /// <summary>Installs the <c>disposed</c> getter, which answers the stack's state.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4F0004
    // Broiler-Human:        PENDING
    private void DisposalAccessor(JsObject prototype, bool asynchronous) =>
        prototype.SetOwnProperty(
            "disposed",
            JsProperty.Accessor(
                Native("get disposed", 0, (engine, thisValue, arguments) =>
                {
                    _ = arguments;

                    if (thisValue.AsObjectOrNull() is not JsDisposableStack stack ||
                        stack.Asynchronous != asynchronous)
                    {
                        return engine.ThrowTypeError(
                            "get disposed called on an incompatible receiver");
                    }

                    return stack.Disposed ? JsValue.True : JsValue.False;
                }),
                null,
                JsPropertyAttributes.Configurable));

    /// <summary>
    /// The receiver check every stack method starts with: the right kind of stack, and not
    /// disposed.
    /// </summary>
    /// <remarks>
    /// The two kinds are different internal slots in the specification, so a synchronous stack's
    /// <c>use</c> refuses an asynchronous stack as its receiver and the other way round. A disposed
    /// stack answers <c>ReferenceError</c>, which is the error the specification chose for "this
    /// name no longer refers to anything usable".
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F62FA5
    // Broiler-Human:        PENDING
    private static JsDisposableStack DisposalThis(
        JsEngine engine, JsValue thisValue, bool asynchronous, string method)
    {
        if (thisValue.AsObjectOrNull() is not JsDisposableStack stack ||
            stack.Asynchronous != asynchronous)
        {
            throw engine.Error(
                "TypeError",
                (asynchronous ? "AsyncDisposableStack" : "DisposableStack") + ".prototype." + method +
                " called on an incompatible receiver");
        }

        if (method != "dispose" && stack.Disposed)
        {
            throw engine.Error(
                "ReferenceError",
                (asynchronous ? "AsyncDisposableStack" : "DisposableStack") + ".prototype." + method +
                " called on a disposed stack");
        }

        return stack;
    }

    /// <summary>Reads one argument, which may not have been supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=185FC5
    // Broiler-Human:        PENDING
    private static JsValue DisposalArg(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>Appends one resource, charging it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=043002
    // Broiler-Human:        PENDING
    private static void DisposalPush(JsEngine engine, JsDisposableStack stack, JsDisposableResource resource)
    {
        engine.Charge(1);
        engine.Retain(DisposableResourceBytes);
        stack.Resources.Add(resource);
    }

    /// <summary>
    /// <c>use(value)</c>: the specification's <c>AddDisposableResource</c> without a method.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A nullish value is accepted and means different things to the two stacks.</b> A
    /// synchronous stack records nothing. An asynchronous one records an entry with no method,
    /// because the specification uses it to promise one <c>Await</c> at the end of disposal even
    /// when nothing else awaited; skipping it would let <c>disposeAsync()</c> settle a turn early.
    /// </para>
    /// <para>
    /// <b>An asynchronous stack falls back to <c>Symbol.dispose</c></b> when the value has no
    /// <c>Symbol.asyncDispose</c>, and wraps what it finds so that the synchronous method's result
    /// is never awaited and its throw becomes a rejection. Only the fallback is read when the first
    /// member is absent, and each member is read exactly once.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=AABE0A
    // Broiler-Human:        PENDING
    private void DisposalAdd(JsEngine engine, JsDisposableStack stack, JsValue value)
    {
        if (value.IsNullish)
        {
            if (stack.Asynchronous)
            {
                DisposalPush(engine, stack, new JsDisposableResource(JsValue.Undefined, JsValue.Undefined));
            }

            return;
        }

        DisposalPush(
            engine, stack, new JsDisposableResource(value, DisposalMethod(engine, value, stack.Asynchronous)));
    }

    /// <summary>
    /// The specification's <c>GetDisposeMethod</c> for a value that is not nullish, with the two
    /// <c>TypeError</c>s <c>CreateDisposableResource</c> adds: a value that is not an object, and an
    /// object with no method of either kind.
    /// </summary>
    /// <remarks>
    /// Shared by both stacks and by <c>using</c> declarations, so the member read, the fallback and
    /// the wrapper are one piece of code whichever surface registered the resource.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D15876
    // Broiler-Human:        PENDING
    private JsValue DisposalMethod(JsEngine engine, JsValue value, bool asynchronous)
    {
        if (!value.IsObject)
        {
            throw engine.Error("TypeError", "a disposable resource must be an object");
        }

        JsValue method;

        if (asynchronous)
        {
            if (!engine.TryGetSymbolMethod(value, AsyncDisposeSymbol, out method) &&
                engine.TryGetSymbolMethod(value, DisposeSymbol, out var synchronous))
            {
                method = DisposalFromSync(synchronous);
            }
        }
        else
        {
            _ = engine.TryGetSymbolMethod(value, DisposeSymbol, out method);
        }

        if (method.Type == JsType.Undefined)
        {
            throw engine.Error(
                "TypeError",
                asynchronous
                    ? "the resource has neither Symbol.asyncDispose nor Symbol.dispose"
                    : "the resource has no Symbol.dispose");
        }

        return method;
    }

    /// <summary>
    /// The specification's wrapper around a synchronous <c>Symbol.dispose</c> an asynchronous stack
    /// fell back to.
    /// </summary>
    /// <remarks>
    /// It calls the method on the resource, discards what it answered, and answers a promise: fulfilled
    /// with <c>undefined</c>, or rejected with what the method threw. It is not reachable by guest
    /// code - nothing stores it where a program can read it - so its name and length are not
    /// observable.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2D40DC
    // Broiler-Human:        PENDING
    private JsValue DisposalFromSync(JsValue synchronous) =>
        JsValue.Object(Native(string.Empty, 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var promise = NewAsyncPromise();

            try
            {
                _ = engine.Call(synchronous, thisValue, []);
                SettleAsyncPromise(engine, promise, JsValue.Undefined, rejected: false);
            }
            catch (JsThrow thrown)
            {
                SettleAsyncPromise(engine, promise, thrown.Value, rejected: true);
            }

            return JsValue.Object(promise);
        }));

    /// <summary><c>adopt(value, onDispose)</c>: registers a closure calling the callback with the value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=AF9263
    // Broiler-Human:        PENDING
    private void DisposalAdopt(
        JsEngine engine, JsDisposableStack stack, JsValue value, JsValue onDispose, string method)
    {
        if (!onDispose.IsObject || !onDispose.AsObject().IsCallable)
        {
            throw engine.Error("TypeError", method + ": the disposal callback is not a function");
        }

        // THE CALLBACK IS CALLED WITH NO RECEIVER AND THE VALUE AS ITS ONE ARGUMENT, and what it
        // answers is passed through: an asynchronous stack awaits it, a synchronous one drops it.
        var closure = Native(string.Empty, 0, (inner, thisValue, arguments) =>
        {
            _ = thisValue;
            _ = arguments;
            return inner.Call(onDispose, JsValue.Undefined, [value]);
        });

        DisposalPush(engine, stack, new JsDisposableResource(JsValue.Undefined, JsValue.Object(closure)));
    }

    /// <summary><c>defer(onDispose)</c>: registers the callback itself, called with nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=8E3A8D
    // Broiler-Human:        PENDING
    private static void DisposalDefer(
        JsEngine engine, JsDisposableStack stack, JsValue onDispose, string method)
    {
        if (!onDispose.IsObject || !onDispose.AsObject().IsCallable)
        {
            throw engine.Error("TypeError", method + ": the disposal callback is not a function");
        }

        DisposalPush(engine, stack, new JsDisposableResource(JsValue.Undefined, onDispose));
    }

    /// <summary>
    /// <c>move()</c>: a new stack of the INTRINSIC kind takes the entries and the old one is
    /// disposed without running any of them.
    /// </summary>
    /// <remarks>
    /// The new stack is built on the intrinsic prototype whatever the receiver's subclass, which is
    /// what the specification says and what test262 checks: <c>move</c> does not consult a species.
    /// The transfer is the list itself, so moving a stack of any size costs one charge.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=CAEDD5
    // Broiler-Human:        PENDING
    private static JsDisposableStack DisposalMove(
        JsEngine engine, JsDisposableStack stack, JsObject prototype)
    {
        engine.Charge(2);

        var moved = new JsDisposableStack(prototype, stack.Asynchronous);
        moved.Resources = stack.Resources;
        stack.Resources = [];
        stack.Disposed = true;
        return moved;
    }

    /// <summary>Marks a stack disposed and takes its entries away from it, in one step.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=CF7D49
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<JsDisposableResource> DisposalDetach(
        JsDisposableStack stack)
    {
        var entries = stack.Resources;
        stack.Resources = [];
        stack.Disposed = true;
        return entries;
    }

    // ---- resource scopes: `using` and `await using` (JSeal F21-F22, JSD-0034) ---------------------

    /// <summary>
    /// <c>AddDisposableResource</c> for a declaration: registers <paramref name="value"/> with the
    /// scope under the declaration's hint, capturing its method now.
    /// </summary>
    /// <remarks>
    /// A nullish value is skipped under the synchronous hint and recorded with no method under the
    /// asynchronous one, which is the record the specification keeps so that leaving the scope
    /// still awaits once. Everything else goes through the same <see cref="DisposalMethod"/> the
    /// stacks use, so a declaration and <c>stack.use</c> read the same members in the same order.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=939B46
    // Broiler-Human:        PENDING
    internal void DisposeScopeAdd(JsEngine engine, JsDisposeScope scope, JsValue value, bool asynchronous)
    {
        if (value.IsNullish)
        {
            if (asynchronous)
            {
                DisposeScopePush(
                    engine, scope, new JsScopedResource(JsValue.Undefined, JsValue.Undefined, Asynchronous: true));
            }

            return;
        }

        var method = DisposalMethod(engine, value, asynchronous);
        DisposeScopePush(engine, scope, new JsScopedResource(value, method, asynchronous));
    }

    /// <summary>Appends one resource to a scope, charging it as a stack entry is charged.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0C3AFA
    // Broiler-Human:        PENDING
    private static void DisposeScopePush(JsEngine engine, JsDisposeScope scope, JsScopedResource resource)
    {
        engine.Charge(1);
        engine.Retain(DisposableResourceBytes);
        scope.Resources.Add(resource);
    }

    /// <summary>
    /// Folds a value that left a resource scope abruptly into the scope's completion.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A forced return is not a throw and is not folded.</b> A generator's <c>return()</c>
    /// reaches the scope's handler as the wrapper the executor passes through <c>finally</c>
    /// regions; it is kept beside the completion, handed back when disposal throws nothing, and
    /// discarded when a disposer throws, because a return overtaken by an exception is that
    /// exception.
    /// </para>
    /// <para>
    /// <b>Every other value is a throw</b>: the first becomes the completion, and each later one
    /// becomes a <c>SuppressedError</c> whose <c>error</c> is the new value and whose
    /// <c>suppressed</c> is the completion so far - the specification's <c>DisposeResources</c>
    /// step, with the payloads kept as they are.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=6F7B69
    // Broiler-Falsified-If: a second throw during one scope's disposal replaces the first instead of suppressing it, or a forced return is reported as a thrown value
    // Broiler-Human:        PENDING
    internal void DisposeScopeFold(JsEngine engine, JsDisposeScope scope, JsValue thrown)
    {
        if (thrown.AsObjectOrNull() is JsForcedReturn)
        {
            scope.Forced = thrown;
            return;
        }

        scope.Completion = scope.Threw ? NewSuppressedError(engine, thrown, scope.Completion) : thrown;
        scope.Threw = true;
    }

    /// <summary>
    /// Unwinds an asynchronous scope until something must be awaited, answering it, or answers
    /// false when the scope is spent.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is <c>DisposeResources</c> with its awaits cut out and handed to the caller.</b> The
    /// lowering awaits what this answers with the ordinary <c>Await</c>, folds a rejection with
    /// <see cref="DisposeScopeFold"/>, and asks again; the position, the completion and the two flags
    /// live on the scope, so each entry is visited once however the suspensions interleave.
    /// </para>
    /// <para>
    /// <b>The two flags are the specification's <c>needsAwait</c> and <c>hasAwaited</c></b>, with
    /// the same effect as in <c>disposeAsync()</c>: an entry recorded from a nullish
    /// <c>await using</c> owes one <c>Await(undefined)</c>, paid before the next synchronous entry
    /// runs or at the end, and only if nothing awaited since. A synchronous entry of a scope that
    /// also holds asynchronous ones is called and never awaited.
    /// </para>
    /// <para>
    /// Each entry is charged as it is reached, and only a guest throw is folded; an allowance or a
    /// cancellation that stops the run leaves the remaining entries not run.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F3F6F6
    // Broiler-Falsified-If: an entry of one scope is called twice, a synchronous entry's result is awaited, or a nullish await using owes more or fewer awaits than DisposeResources performs
    // Broiler-Human:        PENDING
    internal bool DisposeScopeStep(JsEngine engine, JsDisposeScope scope, out JsValue awaited)
    {
        scope.Start();

        while (scope.Next >= 0)
        {
            engine.Charge(1);
            var entry = scope.Resources[scope.Next];

            if (!entry.Asynchronous && scope.NeedsAwait && !scope.HasAwaited)
            {
                scope.NeedsAwait = false;
                awaited = JsValue.Undefined;
                return true;
            }

            scope.Next--;

            if (entry.Method.Type == JsType.Undefined)
            {
                scope.NeedsAwait = true;
                continue;
            }

            JsValue result;

            try
            {
                result = engine.Call(entry.Method, entry.Value, []);
            }
            catch (JsThrow thrown)
            {
                DisposeScopeFold(engine, scope, thrown.Value);
                continue;
            }

            if (entry.Asynchronous)
            {
                scope.HasAwaited = true;
                awaited = result;
                return true;
            }
        }

        if (scope.NeedsAwait && !scope.HasAwaited)
        {
            scope.NeedsAwait = false;
            awaited = JsValue.Undefined;
            return true;
        }

        awaited = JsValue.Undefined;
        return false;
    }

    /// <summary>
    /// Runs what a scope has left without awaiting, and settles its completion: re-raises it, or
    /// answers it for the lowering's <c>Throw</c> when <paramref name="reraise"/> is set.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>For a synchronous scope this IS <c>DisposeResources</c></b>: every entry newest first,
    /// each throw folded. An asynchronous scope reaches it with nothing left, because its step loop
    /// ran first; only an artifact the lowering did not write could reach it with asynchronous
    /// entries still registered, and those are then called and not awaited.
    /// </para>
    /// <para>
    /// <b>The completion is forgotten as it is settled</b>, and that is what makes a disposal
    /// inlined inside the scope's own region harmless: a throw it raises re-enters the scope's
    /// handler, which folds it into an empty completion, finds no entry left, and re-raises the
    /// same value rather than suppressing it under itself.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=2AD8C7
    // Broiler-Falsified-If: a scope whose disposal threw completes normally, or a scope disposed twice runs any entry twice
    // Broiler-Human:        PENDING
    internal JsValue DisposeScopeEnd(JsEngine engine, JsDisposeScope scope, bool reraise)
    {
        scope.Start();

        while (scope.Next >= 0)
        {
            engine.Charge(1);
            var entry = scope.Resources[scope.Next--];

            if (entry.Method.Type == JsType.Undefined)
            {
                continue;
            }

            try
            {
                _ = engine.Call(entry.Method, entry.Value, []);
            }
            catch (JsThrow thrown)
            {
                DisposeScopeFold(engine, scope, thrown.Value);
            }
        }

        var threw = scope.Threw;
        var completion = scope.Completion;
        var forced = scope.Forced;
        scope.Threw = false;
        scope.Completion = JsValue.Undefined;
        scope.Forced = JsValue.Undefined;
        scope.NeedsAwait = false;

        if (reraise)
        {
            return threw ? completion : forced;
        }

        if (threw)
        {
            throw new JsThrow(completion, engine.Render(completion));
        }

        return JsValue.Undefined;
    }

    /// <summary>
    /// One <c>disposeAsync()</c> in progress: the specification's <c>DisposeResources</c> with its
    /// awaits, written as a continuation that the promise reactions resume.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every await is <see cref="AwaitOn"/></b>, so it costs the same turn an <c>await</c> in
    /// guest code costs and the order against a program's own <c>then</c> chains is the order the
    /// specification fixes. The run holds its own position, so each entry is visited once however
    /// the reactions interleave with other work.
    /// </para>
    /// <para>
    /// <b>The two flags are the specification's <c>needsAwait</c> and <c>hasAwaited</c>.</b> An
    /// entry registered from <c>null</c> or <c>undefined</c> has no method and sets the first; any
    /// entry whose method was called sets the second. One <c>Await(undefined)</c> is owed at the end
    /// when the first is set and the second is not, and only then.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=E99113
    // Broiler-Falsified-If: an entry of one disposeAsync() run is called twice, or the run settles its promise before every entry's awaited result has settled
    // Broiler-Human:        PENDING
    private sealed class JsDisposalRun
    {
        private readonly JsRealm realm;
        private readonly JsPromiseObject promise;
        private readonly System.Collections.Generic.List<JsDisposableResource> entries;
        private int next;
        private bool needsAwait;
        private bool hasAwaited;
        private bool threw;
        private JsValue completion = JsValue.Undefined;

        /// <summary>Creates a run over entries already detached from their stack.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1F6B23
        // Broiler-Human:        PENDING
        internal JsDisposalRun(
            JsRealm owner,
            JsPromiseObject settles,
            System.Collections.Generic.List<JsDisposableResource> detached)
        {
            realm = owner;
            promise = settles;
            entries = detached;
            next = detached.Count - 1;
        }

        /// <summary>
        /// Unwinds from the current position until an entry must be awaited or the list is spent.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=A97F1B
        // Broiler-Falsified-If: a guest throw from a disposer escapes disposeAsync() synchronously instead of rejecting its promise
        // Broiler-Human:        PENDING
        internal void Continue(JsEngine engine)
        {
            while (next >= 0)
            {
                engine.Charge(1);
                var entry = entries[next--];

                if (entry.Method.Type == JsType.Undefined)
                {
                    needsAwait = true;
                    continue;
                }

                JsValue result;

                try
                {
                    result = engine.Call(entry.Method, entry.Value, []);
                }
                catch (JsThrow thrown)
                {
                    Fold(engine, thrown.Value);
                    continue;
                }

                hasAwaited = true;

                if (Await(engine, result, resumeAtEnd: false))
                {
                    return;
                }
            }

            if (needsAwait && !hasAwaited)
            {
                hasAwaited = true;

                if (Await(engine, JsValue.Undefined, resumeAtEnd: true))
                {
                    return;
                }
            }

            Finish(engine);
        }

        /// <summary>
        /// Awaits <paramref name="value"/> and resumes the run, answering whether the await was
        /// registered; a throw from resolving the value is folded and the caller carries on.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BDDD8C
        // Broiler-Human:        PENDING
        private bool Await(JsEngine engine, JsValue value, bool resumeAtEnd)
        {
            try
            {
                realm.AwaitOn(engine, value, (inner, settled, rejected) =>
                {
                    if (rejected)
                    {
                        Fold(inner, settled);
                    }

                    if (resumeAtEnd)
                    {
                        Finish(inner);
                    }
                    else
                    {
                        Continue(inner);
                    }
                });

                return true;
            }
            catch (JsThrow thrown)
            {
                Fold(engine, thrown.Value);
                return false;
            }
        }

        /// <summary>Combines one more throw into the completion, as a SuppressedError when one is in flight.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6470BF
        // Broiler-Human:        PENDING
        private void Fold(JsEngine engine, JsValue thrown)
        {
            completion = threw ? realm.NewSuppressedError(engine, thrown, completion) : thrown;
            threw = true;
        }

        /// <summary>Settles the run's promise with the completion.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1C9833
        // Broiler-Human:        PENDING
        private void Finish(JsEngine engine) =>
            realm.SettleAsyncPromise(engine, promise, completion, rejected: threw);
    }
}

/// <summary>One registered resource: the value a disposer is called on, and the disposer.</summary>
/// <remarks>
/// A method of <c>undefined</c> is the asynchronous stack's record of a nullish <c>use</c>, which owes
/// an await and calls nothing.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F84EF1
// Broiler-Human:        PENDING
internal readonly record struct JsDisposableResource(JsValue Value, JsValue Method);

/// <summary>A <c>DisposableStack</c> or an <c>AsyncDisposableStack</c>.</summary>
/// <remarks>
/// One class for both because the two differ in how they unwind and in nothing they store; the flag
/// is the internal slot that tells them apart, and every method checks it, so neither kind is
/// accepted as the receiver of the other's methods.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9972BE
// Broiler-Human:        PENDING
internal sealed class JsDisposableStack : JsObject
{
    /// <summary>Creates a pending, empty stack.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=35F23D
    // Broiler-Human:        PENDING
    internal JsDisposableStack(JsObject? prototype, bool asynchronous)
        : base(prototype, asynchronous ? "AsyncDisposableStack" : "DisposableStack") =>
        Asynchronous = asynchronous;

    /// <summary>Whether this is an <c>AsyncDisposableStack</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E3E8DC
    // Broiler-Human:        PENDING
    internal bool Asynchronous { get; }

    /// <summary>Whether the stack has been disposed or moved from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=43E7A3
    // Broiler-Human:        PENDING
    internal bool Disposed { get; set; }

    /// <summary>The registered resources, oldest first.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=25E022
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsDisposableResource> Resources { get; set; } = [];
}

/// <summary>One resource a declaration registered, with the hint it was declared under.</summary>
/// <remarks>
/// A method of <c>undefined</c> is a nullish <c>await using</c>, which owes an await and calls nothing.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9FBEA8
// Broiler-Human:        PENDING
internal readonly record struct JsScopedResource(JsValue Value, JsValue Method, bool Asynchronous);

/// <summary>
/// The disposal state of one statement list that declared a resource: the specification's
/// <c>DisposeCapability</c>, plus the completion its disposal is building.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is an object only so that a scope slot can hold it</b>, and it has no prototype, no
/// properties and no reachable name: the lowering keeps it in a slot whose name begins with
/// <c>#</c> and passes it to nothing but the five disposal instructions, exactly as a
/// <c>for … of</c> iterator record is kept.
/// </para>
/// <para>
/// <b>Unwinding starts once and moves one way.</b> The first step or end fixes the position at the
/// newest entry, every entry is passed once, and a scope disposed again - by its own handler after
/// an inlined disposal threw - finds nothing left to run.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=14A8D9
// Broiler-Human:        PENDING
internal sealed class JsDisposeScope : JsObject
{
    /// <summary>Creates an empty scope.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=73808D
    // Broiler-Human:        PENDING
    internal JsDisposeScope()
        : base(null, "DisposeScope")
    {
    }

    /// <summary>The registered resources, oldest first.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=EE3B94
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsScopedResource> Resources { get; } = [];

    /// <summary>The index of the next entry to unwind, or -1 when none is left.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=97E3B0
    // Broiler-Human:        PENDING
    internal int Next { get; set; } = -1;

    /// <summary>Whether unwinding has begun.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0513F6
    // Broiler-Human:        PENDING
    internal bool Started { get; private set; }

    /// <summary>The specification's <c>needsAwait</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=51B3C4
    // Broiler-Human:        PENDING
    internal bool NeedsAwait { get; set; }

    /// <summary>The specification's <c>hasAwaited</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=AE6B2A
    // Broiler-Human:        PENDING
    internal bool HasAwaited { get; set; }

    /// <summary>Whether the completion so far is a throw.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A3F941
    // Broiler-Human:        PENDING
    internal bool Threw { get; set; }

    /// <summary>The thrown value of the completion so far, when <see cref="Threw"/> is set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=62D6FD
    // Broiler-Human:        PENDING
    internal JsValue Completion { get; set; } = JsValue.Undefined;

    /// <summary>A generator's forced return that entered the scope's handler, or undefined.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=62DEF4
    // Broiler-Human:        PENDING
    internal JsValue Forced { get; set; } = JsValue.Undefined;

    /// <summary>Fixes the unwinding position at the newest entry, the first time it is asked.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=15437E
    // Broiler-Human:        PENDING
    internal void Start()
    {
        if (Started)
        {
            return;
        }

        Started = true;
        Next = Resources.Count - 1;
    }

    /// <summary>The scope a disposal instruction was handed.</summary>
    /// <remarks>
    /// Only the lowering puts a value where these instructions read one, from a slot no source can
    /// name, so anything else is a defect of the artifact and ends the invocation as one rather than
    /// being read as a scope.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=119C32
    // Broiler-Falsified-If: a value that is not a disposal scope is treated as one
    // Broiler-Human:        PENDING
    internal static JsDisposeScope From(JsValue value) =>
        value.AsObjectOrNull() as JsDisposeScope ??
        throw new JsAbort(JsAbortKind.InternalDefect, "a disposal instruction was handed no disposal scope");
}
