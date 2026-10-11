// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           0
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  5/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Runtime.CompilerServices;
using Broiler.VM.Abstractions;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    // ---- async functions -----------------------------------------------------------------------

    /// <summary>
    /// Runs one async call's body until it suspends, returns or throws, and settles its promise
    /// when it does either of the last two.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is the WHOLE driver, and it is called from exactly two places</b>: the call that
    /// started the function, and the promise reaction an <c>await</c> registered. There is no
    /// third, which is what makes "the body runs at the call and then only on a job" a property a
    /// reader can check by looking at the callers rather than by tracing the queue.
    /// </para>
    /// <para>
    /// <b>A resumption is charged like a call, because it IS one.</b> Fuel covers the re-entry so
    /// that a program awaiting in a loop cannot buy frame switches for nothing, and the CALL-DEPTH
    /// dimension covers the frame so that an async function awaiting another async function
    /// thousands deep ends in a named exhaustion rather than a stack overflow. Both are the
    /// argument <see cref="ResumeGenerator"/> already makes, and it applies here more strongly: a
    /// job that resumes a frame which awaits again enqueues another job, so a program that awaits
    /// for ever spends its allowance on a queue that never empties and the drain ends naming
    /// <c>Fuel</c> — never as a hang.
    /// </para>
    /// <para>
    /// <b>The exhaustion path settles nothing, deliberately.</b> When the allowance is spent
    /// mid-body the abort travels out to the host as a contract violation, and rejecting the
    /// promise on the way would be manufacturing a guest-visible outcome for an operation the host
    /// is being told did not complete. The frame is dropped, so nothing is retained by a call that
    /// can never run again.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=80C159
    // Broiler-Falsified-If: an async call whose body is already on the interpreter's stack is resumed again, or a program that awaits without end is a hang rather than an exhaustion
    // Broiler-Human:        PENDING
    private void ResumeAsync(JsAsyncCall call, JsResumeMode mode, JsValue carried)
    {
        // A CALL THAT HAS NO FRAME HAS ALREADY SETTLED, and a second resumption of it is silently
        // dropped rather than answered. It is reachable only through a thenable whose `then` calls
        // its callback twice, which the promise machinery's own latch already stops - this is the
        // second lock on the same door, because the cost of being wrong here is two interpreters
        // walking one operand stack rather than a diagnosable error.
        if (call.Frame is null || call.Running)
        {
            return;
        }

        // A RESUMPTION REFUSED FOR DEPTH REJECTS, WHERE A GENERATOR'S WOULD MERELY REFUSE. Nothing
        // will ever come back to ask again: the reaction that carried this resumption has already
        // run, so a call left suspended here is a promise pending for ever, which is the hang the
        // whole metering model exists to prevent. It is taken before any state moves, so the
        // rejection is the only thing that happened.
        if (depth >= MaximumCallDepth || !RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            call.Frame = null;
            Realm.SettleAsyncPromise(this, call.Promise, Error("RangeError", "Maximum call stack size exceeded").Value, rejected: true);

            return;
        }

        Charge(4);

        if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
        }

        depth++;
        call.Running = true;
        var body = call.Frame;
        body.ResumeMode = mode;
        body.ResumeValue = carried;
        body.Suspended = false;

        // WHAT THE BODY DID, DECIDED INSIDE THE FRAME AND ACTED ON OUTSIDE IT. Settling a promise
        // and registering an await both run guest code - a reaction handler, a `then` getter - and
        // running that while this frame still counts against the call-depth dimension would charge
        // a continuation for a frame that has already finished. Recording the outcome in two locals
        // and acting after the `finally` is what keeps the accounting honest, and it is also what
        // stops a thenable that resumes synchronously from meeting its own frame still marked
        // running.
        var outcome = JsValue.Undefined;
        var settled = false;
        var rejected = false;

        try
        {
            var completed = Execute(body.Program, body.UnitIndex, null, body.ThisValue, body.Arguments, body.Function, body.NewTarget, body.ThisBinding, body);

            outcome = completed;

            if (body.Suspended)
            {
                body.Started = true;
            }
            else
            {
                settled = true;
                call.Frame = null;
            }
        }
        catch (JsThrow thrown)
        {
            // THE BODY'S OWN `catch` AND `finally` HAVE ALREADY RUN by the time this is reached:
            // an abrupt resumption is raised at the suspension point inside the dispatch loop's
            // try, so the unit's exception regions saw it first. What arrives here is what the body
            // did not handle, and rejecting with it is the whole of `async` error propagation.
            outcome = thrown.Value;
            settled = true;
            rejected = true;
            call.Frame = null;
        }
        catch
        {
            // AN ALLOWANCE SPENT MID-BODY SETTLES NOTHING, deliberately. The abort travels out to
            // the host as a contract violation, and manufacturing a rejection on the way would be
            // giving the guest an outcome for an operation the host is being told did not complete.
            // The frame is dropped, so a call that can never run again retains nothing.
            call.Frame = null;
            throw;
        }
        finally
        {
            call.Running = false;
            depth--;
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }

        if (settled)
        {
            Realm.SettleAsyncPromise(
                this,
                call.Promise,
                call.DiscardsCompletion && !rejected ? JsValue.Undefined : outcome,
                rejected);

            return;
        }

        // THE AWAIT IS PERFORMED HERE AND NOT IN THE OPCODE, for the reason the `Yield` case hands
        // its value out rather than acting on it: the opcode's whole job is to leave the frame in a
        // resumable state, and everything that depends on WHO resumes belongs to the driver.
        Realm.AwaitOn(this, outcome, (engine, value, threw) => engine.ResumeAsync(call, threw ? JsResumeMode.Throw : JsResumeMode.Next, value));
    }

    // ---- async generators ----------------------------------------------------------------------
    //
    // THE THIRD DRIVER, AND IT IS NOT THE OTHER TWO STACKED. A generator is pulled by its caller and
    // answers at once; an async function is pushed by the job queue and answers a promise nobody
    // asked twice for. An async generator is pulled AND answers later, which is a combination
    // neither of the two above has a place to put: between the pull and the answer, another pull can
    // arrive. The queue below is where those go, and everything else in this section exists to keep
    // it answered in order.

    /// <summary>
    /// The one entry every <c>next</c>, <c>return</c> and <c>throw</c> on an async generator goes
    /// through: it answers a promise, and it may start nothing at all.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every path answers a PROMISE, including the paths that are errors.</b> A receiver that is
    /// not an async generator is a rejected promise where the synchronous family throws, because
    /// the method's whole contract is that it answers something with a <c>then</c> - and a caller
    /// writing <c>gen.next().catch(f)</c> would otherwise also need a <c>try</c> around the call.
    /// </para>
    /// <para>
    /// <b>The three methods differ in what a generator that is not suspended does with them, and
    /// that is the only place they differ.</b> <c>next</c> on a completed generator answers a done
    /// step without queueing anything; <c>throw</c> on one that has not started completes it and
    /// rejects, running none of its body, for the reason the synchronous family does not enter a
    /// body that has no <c>try</c> in it yet; <c>return</c> queues even on a completed generator,
    /// because its value is AWAITED before it is answered and the awaiting has to happen somewhere.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=CB6E30
    // Broiler-Falsified-If: a call of `next`, `return` or `throw` on an async generator answers anything but a promise, or two calls made before the first settles are answered out of order
    // Broiler-Human:        PENDING
    internal JsValue EnqueueAsyncGenerator(JsValue receiver, JsResumeMode mode, JsValue sent, string method)
    {
        if (receiver.AsObjectOrNull() is not JsAsyncGenerator generator)
        {
            return Realm.RejectedPromise(this, "AsyncGenerator.prototype." + method + " called on a value that is not an async generator");
        }

        Charge(4);
        var promise = Realm.NewAsyncPromise();

        // A `throw` INTO A GENERATOR THAT HAS NOT STARTED COMPLETES IT WHERE IT STANDS. There is no
        // `try` in a body none of whose instructions have run, so entering it to unwind would run
        // the parameter bindings' side effects a second time for nothing.
        if (mode == JsResumeMode.Throw && generator.State == JsAsyncGeneratorState.SuspendedStart)
        {
            generator.Frame = null;
            generator.State = JsAsyncGeneratorState.Completed;
        }

        var state = generator.State;

        if (mode == JsResumeMode.Next && state == JsAsyncGeneratorState.Completed)
        {
            return Realm.FulfilledPromise(
                this, JsValue.Object(Realm.IteratorResult(JsValue.Undefined, done: true)));
        }

        if (mode == JsResumeMode.Throw && state == JsAsyncGeneratorState.Completed)
        {
            return Realm.RejectWith(this, sent);
        }

        generator.Queue.Add(new JsAsyncGeneratorRequest(mode, sent, promise));

        switch (state)
        {
            case JsAsyncGeneratorState.SuspendedStart when mode == JsResumeMode.Return:
            case JsAsyncGeneratorState.Completed:

                // THE BODY IS NOT ENTERED AND THE VALUE IS STILL AWAITED. `agen().return(p)` where
                // `p` is a promise answers `{ value: <what p resolved to>, done: true }`, which is
                // the one thing a `return` does that a `next` on the same generator does not - and
                // it is why this request is queued rather than answered here.
                generator.Frame = null;
                generator.State = JsAsyncGeneratorState.DrainingQueue;
                AwaitAsyncGeneratorReturn(generator);
                break;

            case JsAsyncGeneratorState.SuspendedStart:
                generator.State = JsAsyncGeneratorState.Executing;
                ResumeAsyncGenerator(generator, mode, sent);
                break;

            case JsAsyncGeneratorState.SuspendedYield:
                ResumeAsyncGeneratorAtYield(generator, mode, sent);
                break;

            default:

                // EXECUTING OR DRAINING: the request is in the queue and something already running
                // will reach it. Doing anything else here is what would re-enter a running frame.
                break;
        }

        return JsValue.Object(promise);
    }

    /// <summary>
    /// Resumes a generator suspended at a <c>yield</c>, awaiting a <c>return</c>'s value first.
    /// </summary>
    /// <remarks>
    /// <b>It is <c>AsyncGeneratorUnwrapYieldResumption</c>, and the await it performs is invisible
    /// in the source.</b> <c>gen.return(p)</c> while the body sits at a <c>yield</c> waits for
    /// <c>p</c> before the body's own <c>finally</c> blocks run, so a finaliser that observes the
    /// world sees it after <c>p</c> settled rather than before. A <c>p</c> that REJECTS turns the
    /// return into a throw at the same suspension point, which is why the callback below chooses
    /// between two modes rather than always raising a return.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A675D5
    // Broiler-Human:        PENDING
    private void ResumeAsyncGeneratorAtYield(JsAsyncGenerator generator, JsResumeMode mode, JsValue sent)
    {
        generator.State = JsAsyncGeneratorState.Executing;

        if (mode != JsResumeMode.Return)
        {
            ResumeAsyncGenerator(generator, mode, sent);
            return;
        }

        // The Await is the generator's own, at its `yield`: a value whose PromiseResolve throws -
        // a promise with a throwing `constructor` getter - throws THERE, where the body's `catch`
        // can see it (27.6.3.7 step 7). Until 2026-10-03 the throw escaped to the caller of
        // `return` and the body never ran (JSC-252).
        JsThrow? immediate = null;

        try
        {
            Realm.AwaitOn(this, sent, (engine, value, threw)
                => engine.ResumeAsyncGenerator(generator, threw ? JsResumeMode.Throw : JsResumeMode.Return, value));
        }
        catch (JsThrow thrown)
        {
            immediate = thrown;
        }

        if (immediate is not null)
        {
            ResumeAsyncGenerator(generator, JsResumeMode.Throw, immediate.Value);
        }
    }

    /// <summary>
    /// Runs one async generator's body until it yields, awaits, returns or throws, and answers the
    /// requests it settles on the way.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is a LOOP and not a recursion, and the difference is a process termination.</b> A
    /// <c>yield</c> answers the request at the front of the queue and then looks at the queue
    /// again: if another request is already waiting the specification says execution continues
    /// WITHOUT suspending, so the body is resumed straight away. Writing that as a call back into
    /// this method cost one native frame per queued request, and a program that calls <c>next</c> a
    /// hundred thousand times before awaiting any of the promises would have ended in a stack
    /// overflow rather than in an answer - on a stack that holds a few thousand ordinary calls.
    /// </para>
    /// <para>
    /// <b>The two suspensions are told apart by the frame and never by the value.</b>
    /// <see cref="JsFrame.Suspension"/> is written by the instruction that left the loop, so an
    /// <c>await</c> whose operand happens to be an iteration step and a <c>yield</c> whose operand
    /// happens to be a promise are not confusable. Guessing from the value is the defect this field
    /// exists to make unwritable.
    /// </para>
    /// <para>
    /// <b>A resumption is charged like a call, because it IS one</b>, exactly as
    /// <see cref="ResumeGenerator"/> and <see cref="ResumeAsync"/> argue: fuel for the re-entry so
    /// that driving a generator cannot buy frame switches for nothing, and the call-depth dimension
    /// for the frame so that an async generator awaiting another one thousands deep ends in a named
    /// exhaustion rather than a stack overflow.
    /// </para>
    /// <para>
    /// <b>A depth refusal REJECTS where a synchronous generator's would merely refuse</b>, for the
    /// reason <see cref="ResumeAsync"/> gives: nothing will come back to ask again, so a request
    /// left unanswered is a promise pending for ever.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=3FCF57
    // Broiler-Falsified-If: an async generator whose body is on the interpreter's stack is resumed again, or an `await` inside an async generator body settles a request the way a `yield` does
    // Broiler-Human:        PENDING
    private void ResumeAsyncGenerator(JsAsyncGenerator generator, JsResumeMode mode, JsValue carried)
    {
        while (true)
        {
            if (generator.Frame is null || generator.Running)
            {
                return;
            }

            if (depth >= MaximumCallDepth || !RuntimeHelpers.TryEnsureSufficientExecutionStack())
            {
                generator.Frame = null;
                generator.State = JsAsyncGeneratorState.DrainingQueue;

                Realm.CompleteAsyncGeneratorStep(this, generator, Error("RangeError", "Maximum call stack size exceeded").Value, done: true, rejected: true);

                DrainAsyncGeneratorQueue(generator);
                return;
            }

            Charge(4);

            if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
            {
                throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
            }

            depth++;
            generator.Running = true;
            var body = generator.Frame;
            body.ResumeMode = mode;
            body.ResumeValue = carried;
            body.Suspended = false;
            body.Suspension = JsSuspension.None;

            // WHAT THE BODY DID, DECIDED INSIDE THE FRAME AND ACTED ON OUTSIDE IT, for the reason
            // `ResumeAsync` records: settling a promise runs guest code, and running it while this
            // frame still counts against the call-depth dimension would charge a continuation for a
            // frame that has already finished.
            var outcome = JsValue.Undefined;
            var suspension = JsSuspension.None;
            var finished = false;
            var rejected = false;

            try
            {
                outcome = Execute(body.Program, body.UnitIndex, null, body.ThisValue, body.Arguments, body.Function,

                    // AN ASYNC GENERATOR BODY IS NEITHER AN ARROW NOR A CONSTRUCTOR - the verifier
                    // refuses `Generator | Arrow` and `Async | Constructible` - so its `new.target`
                    // is `undefined` and it has no `this` box to read through.
                    JsValue.Undefined,
                    null,
                    body);

                if (body.Suspended)
                {
                    body.Started = true;
                    suspension = body.Suspension;
                }
                else
                {
                    finished = true;
                    generator.Frame = null;
                }
            }
            catch (JsReturnSignal forced)
            {
                // THE RETURN THE `finally` BLOCKS DID NOT OVERRIDE, arriving here having run every
                // enclosing finaliser - and it completes the generator NORMALLY, with the value it
                // carries. `return` is not an error, and rejecting for one would turn
                // `gen.return(1)` into a rejection every consumer would have to catch.
                outcome = forced.Value;
                finished = true;
                generator.Frame = null;
            }
            catch (JsThrow thrown)
            {
                outcome = thrown.Value;
                finished = true;
                rejected = true;
                generator.Frame = null;
            }
            catch
            {
                // AN ALLOWANCE SPENT MID-BODY SETTLES NOTHING, deliberately, exactly as it does for
                // an async call: the abort travels out to the host as a contract violation, and
                // manufacturing a rejection on the way would hand the guest an outcome for an
                // operation the host is being told did not complete.
                generator.Frame = null;
                throw;
            }
            finally
            {
                generator.Running = false;
                depth--;
                meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
            }

            if (finished)
            {
                generator.State = JsAsyncGeneratorState.DrainingQueue;
                Realm.CompleteAsyncGeneratorStep(this, generator, outcome, true, rejected);
                DrainAsyncGeneratorQueue(generator);
                return;
            }

            if (suspension == JsSuspension.Await)
            {
                // THE STATE STAYS `Executing` ACROSS THE AWAIT, and that is what makes a `next`
                // arriving mid-await a queued request rather than a second entry into the body.
                Realm.AwaitOn(this, outcome, (engine, value, threw)
                    => engine.ResumeAsyncGenerator(generator, threw ? JsResumeMode.Throw : JsResumeMode.Next, value));

                return;
            }

            Realm.CompleteAsyncGeneratorStep(this, generator, outcome, false, false);

            if (generator.Queue.Count == 0)
            {
                generator.State = JsAsyncGeneratorState.SuspendedYield;
                return;
            }

            var waiting = generator.Queue[0];

            // A QUEUED `return` IS THE ONE CONTINUATION THAT CANNOT STAY IN THIS LOOP, because its
            // value has to be awaited before the body is re-entered. Everything else carries on
            // round, which is what "execution continues without suspending" means.
            if (waiting.Mode == JsResumeMode.Return)
            {
                ResumeAsyncGeneratorAtYield(generator, waiting.Mode, waiting.Value);
                return;
            }

            mode = waiting.Mode;
            carried = waiting.Value;
        }
    }

    /// <summary>
    /// Answers the requests an async generator's body will never reach, oldest first.
    /// </summary>
    /// <remarks>
    /// <b>It stops at the first <c>return</c> rather than answering it here</b>, because a
    /// <c>return</c>'s value is awaited and the rest of the queue has to wait behind it: answering
    /// the requests after it first would deliver them out of order. What resumes the drain is the
    /// job <see cref="AwaitAsyncGeneratorReturn"/> registers.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1D9150
    // Broiler-Human:        PENDING
    private void DrainAsyncGeneratorQueue(JsAsyncGenerator generator)
    {
        while (generator.Queue.Count != 0)
        {
            var request = generator.Queue[0];

            if (request.Mode == JsResumeMode.Return)
            {
                AwaitAsyncGeneratorReturn(generator);
                return;
            }

            // A `next` AFTER THE BODY FINISHED IS A DONE STEP CARRYING `undefined`, and NOT the
            // value the body completed with: that value was the answer to the request that was at
            // the front when the body finished, and handing it to every later `next` would repeat
            // a return value the language returns once.
            Realm.CompleteAsyncGeneratorStep(this, generator, request.Mode == JsResumeMode.Throw ? request.Value : JsValue.Undefined, done: true, rejected: request.Mode == JsResumeMode.Throw);
        }

        generator.State = JsAsyncGeneratorState.Completed;
    }

    /// <summary>Awaits the value a queued <c>return</c> carries, then answers it and drains on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C28516
    // Broiler-Human:        PENDING
    private void AwaitAsyncGeneratorReturn(JsAsyncGenerator generator)
    {
        if (generator.Queue.Count == 0)
        {
            generator.State = JsAsyncGeneratorState.Completed;
            return;
        }

        var request = generator.Queue[0];

        try
        {
            Realm.AwaitOn(this, request.Value, (engine, value, threw) =>
                {
                    engine.Realm.CompleteAsyncGeneratorStep(engine, generator, value, true, threw);
                    engine.DrainAsyncGeneratorQueue(generator);
                });
        }
        catch (JsThrow thrown)
        {
            // RESOLVING THE VALUE READ GUEST CODE AND IT THREW - a `constructor` getter on a
            // thenable is enough. The request is answered with that failure rather than left
            // pending, and the drain goes on: a queue that stopped here would be a set of promises
            // nothing could ever settle.
            Realm.CompleteAsyncGeneratorStep(this, generator, thrown.Value, true, rejected: true);
            DrainAsyncGeneratorQueue(generator);
        }
    }

    /// <summary>
    /// One turn of an ASYNC <c>yield*</c>: the same delegation loop, with every inner step awaited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is a separate method from <see cref="Delegate"/> and not a flag through it, because
    /// the two differ in the one thing a method body cannot parameterise: WHERE IT MAY STOP.</b>
    /// A synchronous delegation runs from one <c>yield</c> to the next inside a single entry into
    /// the dispatch loop; an asynchronous one leaves the loop after every inner call, waits for a
    /// promise, and comes back at the same instruction. Threading a boolean through the
    /// synchronous version would have meant a suspension point in the middle of each of its four
    /// branches, and the branch that forgot one would have carried on synchronously with a promise
    /// in its hand.
    /// </para>
    /// <para>
    /// <b>Five re-entry points and one instruction, which is what
    /// <see cref="JsFrame.DelegateStage"/> exists to distinguish.</b> Stage zero is the ordinary
    /// one - the inner value has been yielded out and the caller has answered - and the other four
    /// are awaits: of what <c>next</c> or <c>throw</c> answered, of what <c>return</c> answered, of
    /// the close performed when the inner iterator has no <c>throw</c>, and of the RESUMPTION's own
    /// value when it has no <c>return</c>. That last one is the await a reader does not expect and
    /// the language performs twice: once on the way in, at the yield, and again here.
    /// </para>
    /// <para>
    /// <b>An await that REJECTS ends the delegation and raises in the outer body.</b> That is the
    /// difference between a rejection arriving here and a <c>throw</c> arriving at the yield: the
    /// second is a request from the consumer, which the inner iterator is offered first; the first
    /// is the inner iterator's own step having failed, and there is nothing left to offer it to.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=197F48
    // Broiler-Falsified-If: an inner step of an async `yield*` reaches the outer body unawaited, or a `return` or a `throw` that arrives while one is suspended is not offered to the inner iterator first
    // Broiler-Human:        PENDING
    private JsValue DelegateAsync(JsFrame frame, JsValue[] stack, ref int sp, int pc)
    {
        if (!frame.Delegating)
        {
            frame.Delegate = Realm.GetAsyncIterator(stack[--sp]);
            frame.ResumeMode = JsResumeMode.Next;
            frame.ResumeValue = JsValue.Undefined;
            frame.DelegateStage = DelegateStepFresh;
        }

        // THE FLAG IS CLEARED ON THE WAY IN AND SET AGAIN ONLY BY AN ACTUAL SUSPENSION, for the
        // reason `Delegate` records: every other way out of this method ends the delegation,
        // including the ways that leave by throwing.
        frame.Delegating = false;
        var record = frame.Delegate!;
        var stage = frame.DelegateStage;
        var mode = frame.ResumeMode;
        var carried = frame.ResumeValue;
        frame.ResumeMode = JsResumeMode.Next;
        frame.ResumeValue = JsValue.Undefined;
        frame.DelegateStage = DelegateStepFresh;
        JsValue step;

        if (stage != DelegateStepFresh)
        {
            if (mode == JsResumeMode.Throw)
            {
                record.Done = true;
                frame.Delegate = null;
                throw new JsThrow(carried, Render(carried));
            }

            switch (stage)
            {
                case DelegateAwaitingClose:

                    // THE CLOSE HAPPENED AND ITS ANSWER IS CHECKED, and then the protocol
                    // violation is reported anyway. The inner iterator was given its chance to
                    // clean up because the `throw` it is about to be told it does not implement
                    // ends the delegation; an error the clean-up raised has already left above.
                    frame.Delegate = null;

                    return carried.IsObject
                        ? ThrowTypeError("The iterator does not provide a 'throw' method.")
                        : ThrowTypeError("iterator result is not an object");

                case DelegateAwaitingReceived:
                    frame.Delegate = null;
                    throw new JsReturnSignal(carried);

                default:
                    step = carried;
                    break;
            }
        }
        else
        {
            switch (mode)
            {
                case JsResumeMode.Throw:
                    {
                        var thrower = GetProperty(record.Iterator, "throw");

                        if (thrower.IsNullish)
                        {
                            var closer = GetProperty(record.Iterator, "return");

                            if (closer.IsNullish)
                            {
                                record.Done = true;
                                frame.Delegate = null;
                                return ThrowTypeError("The iterator does not provide a 'throw' method.");
                            }

                            if (!closer.IsObject || !closer.AsObject().IsCallable)
                            {
                                frame.Delegate = null;
                                return ThrowTypeError("The iterator's return is not a function");
                            }

                            record.Done = true;

                            return SuspendDelegation(
                                frame,
                                Call(closer, record.Iterator, []),
                                sp,
                                pc,
                                DelegateAwaitingClose);
                        }

                        if (!thrower.IsObject || !thrower.AsObject().IsCallable)
                        {
                            frame.Delegate = null;
                            return ThrowTypeError("The iterator's throw is not a function");
                        }

                        return SuspendDelegation(
                            frame,
                            Call(thrower, record.Iterator, [carried]),
                            sp,
                            pc,
                            DelegateAwaitingStep);
                    }

                case JsResumeMode.Return:
                    {
                        var returner = GetProperty(record.Iterator, "return");

                        // AN INNER ITERATOR WITH NO `return` DOES NOT SWALLOW THE OUTER ONE, and the
                        // outer one's value is AWAITED A SECOND TIME on the way out. It was awaited
                        // once at the yield, by the unwrapping every async resumption goes through,
                        // and the language awaits it again here - which a program counting turns of
                        // the job queue can see, and which is why it is a suspension rather than a
                        // return raised from this line.
                        if (returner.IsNullish)
                        {
                            record.Done = true;

                            return SuspendDelegation(
                                frame, carried, sp, pc, DelegateAwaitingReceived);
                        }

                        if (!returner.IsObject || !returner.AsObject().IsCallable)
                        {
                            frame.Delegate = null;
                            return ThrowTypeError("The iterator's return is not a function");
                        }

                        return SuspendDelegation(frame, Call(returner, record.Iterator, [carried]), sp, pc, DelegateAwaitingReturn);
                    }

                default:
                    Charge(2);

                    // THE RECORD IS MARKED DONE FOR THE LENGTH OF THE STEP, exactly as the
                    // `for await` head marks it: an inner step that fails owes the inner iterator
                    // no `return`, and the mark is what stops one being sent.
                    record.Done = true;

                    return SuspendDelegation(
                        frame,
                        Call(record.Next, record.Iterator, [carried]),
                        sp,
                        pc,
                        DelegateAwaitingStep);
            }
        }

        if (!step.IsObject)
        {
            frame.Delegate = null;
            return ThrowTypeError("iterator result is not an object");
        }

        if (GetProperty(step, "done").ToBooleanValue())
        {
            record.Done = true;
            frame.Delegate = null;
            var completed = GetProperty(step, "value");

            // A DELEGATION THAT ENDED INSIDE A `return` IS STILL RETURNING. The inner iterator
            // answered a done step, so the value it carries is what the OUTER generator completes
            // with - not what the `yield*` evaluates to, because nothing is going to evaluate it.
            if (stage == DelegateAwaitingReturn)
            {
                throw new JsReturnSignal(completed);
            }

            return completed;
        }

        record.Done = false;

        return SuspendDelegation(frame, GetProperty(step, "value"), sp, pc, DelegateStepFresh);
    }
}
