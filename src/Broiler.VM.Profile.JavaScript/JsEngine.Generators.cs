// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  5/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Runtime.CompilerServices;
using Broiler.VM.Abstractions;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    // ---- generators ----------------------------------------------------------------------------

    /// <summary>
    /// The one entry every resumption goes through: <c>next</c>, <c>return</c> and <c>throw</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The state is decided before the frame is touched.</b> Four states and three methods make
    /// twelve cases, and eleven of them answer without running a single instruction: a completed
    /// generator answers or rethrows, a generator that has not started swallows a <c>return</c> and
    /// rethrows a <c>throw</c> without ever entering its body, and one that is already on the
    /// interpreter's stack is a <c>TypeError</c>. The twelfth is the resumption.
    /// </para>
    /// <para>
    /// <b>A resumption is charged like a call, because it IS one - a second interpreter frame on
    /// the same native stack.</b> Fuel covers the re-entry, so driving a generator a million steps
    /// cannot buy a million frame switches for nothing; and the CALL-DEPTH dimension covers the
    /// frame, which is what makes a <c>yield*</c> chain thousands deep end in a named exhaustion
    /// rather than in a stack overflow. It is charged here and not left to the <c>next</c> call
    /// that reached this method, because that call's frame returns as soon as the generator
    /// suspends and this one does not: a delegation chain holds one of each per level, so counting
    /// only the call would say a chain is half as deep as it is - and the measured difference is
    /// the difference between an answer and a terminated process.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=9607B1
    // Broiler-Falsified-If: a generator resumed while its own body is running re-enters that body, or a completed generator runs any instruction
    // Broiler-Human:        PENDING
    internal JsValue ResumeGenerator(JsValue receiver, JsResumeMode mode, JsValue sent, string method)
    {
        if (receiver.AsObjectOrNull() is not JsGenerator generator)
        {
            return ThrowTypeError("Generator.prototype." + method + " called on a value that is not a generator");
        }

        if (generator.State == JsGeneratorState.Executing)
        {
            return ThrowTypeError("Generator is already running");
        }

        if (generator.State == JsGeneratorState.Completed || generator.Frame is null)
        {
            return mode switch
            {
                JsResumeMode.Throw => throw new JsThrow(sent, Render(sent)),
                JsResumeMode.Return => JsValue.Object(Realm.IteratorResult(sent, done: true)),
                _ => JsValue.Object(Realm.IteratorResult(JsValue.Undefined, done: true)),
            };
        }

        // A GENERATOR THAT HAS NOT STARTED HAS NO `try` TO RUN, so an abrupt resumption completes
        // it where it stands. Resuming into the body first and then unwinding would run the
        // parameter bindings' side effects a second time, which nothing in the language asks for.
        if (generator.State == JsGeneratorState.SuspendedStart && mode != JsResumeMode.Next)
        {
            CompleteGenerator(generator);

            return mode == JsResumeMode.Throw
                ? throw new JsThrow(sent, Render(sent))
                : JsValue.Object(Realm.IteratorResult(sent, done: true));
        }

        Charge(4);

        // THE DEPTH IS TAKEN BEFORE THE STATE MOVES, so a resumption refused for depth leaves the
        // generator suspended and resumable rather than half-entered. A generator that could not
        // be resumed because the stack was full has not run any of its body, and completing it
        // would be a stronger claim than what happened.
        if (depth >= MaximumCallDepth || !RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            return ThrowRangeError("Maximum call stack size exceeded");
        }

        if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
        }

        depth++;
        var frame = generator.Frame;
        frame.ResumeMode = mode;
        frame.ResumeValue = sent;
        frame.Suspended = false;
        generator.State = JsGeneratorState.Executing;

        try
        {
            var completed = Execute(frame.Program, frame.UnitIndex, null, frame.ThisValue, frame.Arguments, frame.Function,

                // A GENERATOR BODY IS NEITHER AN ARROW NOR A CONSTRUCTOR - the verifier refuses
                // both pairings - so its `new.target` is `undefined` and it has no `this` box to
                // read through. Passing the two explicitly rather than threading them through the
                // frame keeps the suspended state to what an instruction boundary actually needs.
                JsValue.Undefined,
                null,
                frame);

            if (frame.Suspended)
            {
                generator.State = JsGeneratorState.SuspendedYield;
                frame.Started = true;

                if (!frame.DelegatedResult.IsEmpty)
                {
                    var delegated = frame.DelegatedResult;
                    frame.DelegatedResult = JsValue.Empty;
                    return delegated;
                }

                return JsValue.Object(Realm.IteratorResult(completed, done: false));
            }

            CompleteGenerator(generator);
            return JsValue.Object(Realm.IteratorResult(completed, done: true));
        }
        catch (JsReturnSignal forced)
        {
            // THE RETURN THE `finally` BLOCKS DID NOT OVERRIDE. It reaches here having run every
            // enclosing finaliser on the way out, which is the whole reason it travels as an
            // exception rather than as a returned flag.
            return JsValue.Object(Realm.IteratorResult(forced.Value, done: true));
        }
        finally
        {
            // ANY OTHER WAY OUT OF THE BODY COMPLETES THE GENERATOR - a throw the body did not
            // catch, an allowance spent mid-instruction, a stack the runtime could not grow. The
            // test is on the STATE rather than on the exception type, because a catch clause per
            // type is a list that a new type is added to by forgetting: the one that got away
            // would leave a generator reading `already running` for the rest of the program, and
            // every later resumption of it would be a TypeError with no cause a reader could find.
            if (generator.State == JsGeneratorState.Executing)
            {
                CompleteGenerator(generator);
            }

            depth--;
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }
    }

    /// <summary>
    /// Retires a generator: no frame, no state to resume, and the operand stack let go of.
    /// </summary>
    /// <remarks>
    /// <b>Dropping the frame reference is the point and not the tidiness.</b> A completed generator
    /// object may stay reachable for the rest of the program - somebody kept the variable - and
    /// without this it would keep its operand-stack array, its scope chain and everything those
    /// reach alive with it. Clearing the field is what makes exhausting a generator release what it
    /// was holding, at the instant it is exhausted rather than at the collector's convenience.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=5D1433
    // Broiler-Human:        PENDING
    private static void CompleteGenerator(JsGenerator generator)
    {
        generator.Frame = null;
        generator.State = JsGeneratorState.Completed;
    }

    /// <summary>
    /// One turn of a <c>yield*</c>: step the inner iterator, or forward an abrupt resumption to it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is the specification's delegation loop, entered afresh at every resumption.</b> The
    /// iterator record lives in the frame, so the loop's whole state between two resumptions is
    /// "which record" and "still delegating" - and re-entering at the same instruction is what lets
    /// a <c>return</c> or a <c>throw</c> that arrives mid-delegation be handed to the inner
    /// iterator rather than raised in the outer body.
    /// </para>
    /// <para>
    /// <b>The source is acquired through <see cref="GetIterator"/> and stepped through
    /// <see cref="TryIterateNext(JsIteratorRecord, JsValue[], out JsValue, out JsValue)"/>, which
    /// is what makes <c>yield*</c> WORK OVER ANYTHING.</b> The
    /// operand is read for its <c>Symbol.iterator</c> exactly as a spread or a <c>for … of</c>
    /// reads it, so an Array, a String, a Map, a Set, another generator and a plain object that
    /// defines the Symbol itself are all delegated over by one path. Recognising the realm's own
    /// iterables by TYPE instead - which is what a realm without a Symbol has to do - refused every
    /// object a guest made iterable, and refused it as "not iterable", which is a claim about the
    /// program rather than about the engine.
    /// </para>
    /// <para>
    /// <b>The two missing-method cases are where an engine is most often wrong.</b> An inner
    /// iterator with no <c>return</c> - which is every Array and String iterator - does not swallow
    /// the outer <c>return</c>: the outer generator returns, running its own finalisers. An inner
    /// iterator with no <c>throw</c> is closed first and then the delegation raises a
    /// <c>TypeError</c>, so a program that throws into one is told what is missing rather than
    /// silently getting its own exception back.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=7787AD
    // Broiler-Falsified-If: a `return` or a `throw` that arrives while a `yield*` is suspended is not offered to the inner iterator first
    // Broiler-Human:        PENDING
    private JsValue Delegate(JsFrame frame, JsValue[] stack, ref int sp, int pc)
    {
        if (!frame.Delegating)
        {
            frame.Delegate = GetIterator(stack[--sp]);
            frame.ResumeMode = JsResumeMode.Next;
            frame.ResumeValue = JsValue.Undefined;
        }

        // THE FLAG IS CLEARED ON THE WAY IN AND SET AGAIN ONLY BY AN ACTUAL SUSPENSION, so every
        // other way out of this method ends the delegation - including the ways that leave by
        // throwing. Clearing it only on the paths that return normally left it set when an inner
        // `throw` method threw and the outer body caught: the next resumption would then re-enter
        // a delegation that no longer existed, at an instruction that was no longer a `yield*`.
        frame.Delegating = false;
        var record = frame.Delegate!;
        var mode = frame.ResumeMode;
        var sent = frame.ResumeValue;
        frame.ResumeMode = JsResumeMode.Next;
        frame.ResumeValue = JsValue.Undefined;
        JsValue step;

        switch (mode)
        {
            case JsResumeMode.Throw:
                {
                    var thrower = GetProperty(record.Iterator, "throw");

                    if (!thrower.IsObject || !thrower.AsObject().IsCallable)
                    {
                        // CLOSED UNDER A NORMAL COMPLETION AND NOT A QUIET ONE, which is what the
                        // specification asks for here: an error the inner `return` raises while it is
                        // being cleaned up is the error the program is owed, and only if it raises
                        // none does the missing `throw` become the TypeError below.
                        CloseIterator(record);
                        return ThrowTypeError("The iterator does not provide a 'throw' method.");
                    }

                    step = Call(thrower, record.Iterator, [sent]);
                    break;
                }

            case JsResumeMode.Return:
                {
                    var returner = GetProperty(record.Iterator, "return");

                    // AN INNER ITERATOR WITH NO `return` DOES NOT SWALLOW THE OUTER ONE. Every Array
                    // and String iterator is in this case, so it is the common one rather than the
                    // exotic one: the outer generator returns, and its own finalisers run on the way.
                    if (!returner.IsObject || !returner.AsObject().IsCallable)
                    {
                        record.Done = true;
                        throw new JsReturnSignal(sent);
                    }

                    step = Call(returner, record.Iterator, [sent]);

                    if (!step.IsObject)
                    {
                        return ThrowTypeError("iterator result is not an object");
                    }

                    if (GetProperty(step, "done").ToBooleanValue())
                    {
                        record.Done = true;
                        frame.Delegate = null;
                        throw new JsReturnSignal(GetProperty(step, "value"));
                    }

                    break;
                }

            default:
                {
                    // THE ORDINARY PROTOCOL: the record's `next`, read once at acquisition, is called
                    // with the sent value, and the inner iterator's own COMPLETION VALUE is what
                    // `yield*` evaluates to - the half of delegation a loop written by hand forgets.
                    Charge(1);
                    step = Call(record.Next, record.Iterator, [sent]);
                    break;
                }
        }

        if (!step.IsObject)
        {
            return ThrowTypeError("iterator result is not an object");
        }

        if (GetProperty(step, "done").ToBooleanValue())
        {
            record.Done = true;
            frame.Delegate = null;
            return GetProperty(step, "value");
        }

        // THE INNER RESULT OBJECT IS YIELDED AS IT IS (27.5.3.4: GeneratorYield(innerResult)), so
        // its `value` is not read here and the caller receives the very object the inner iterator
        // answered. Until 2026-10-03 the value was read and wrapped in a new result, which a getter
        // on `value` could count (JSC-254).
        frame.DelegatedResult = step;
        frame.Delegating = true;
        frame.Sp = sp;
        frame.Pc = pc;
        frame.Suspended = true;
        frame.Suspension = JsSuspension.Yield;
        return JsValue.Undefined;
    }

    /// <summary>Suspends the frame inside a delegation, at the stage it will re-enter at.</summary>
    /// <remarks>
    /// Stage zero is the yield of an inner value and every other stage is an await, which is the one
    /// place the <see cref="JsSuspension"/> the driver reads is decided for a delegation.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D2339C
    // Broiler-Human:        PENDING
    private static JsValue SuspendDelegation(JsFrame frame, JsValue value, int sp, int pc, int stage)
    {
        frame.Delegating = true;
        frame.DelegateStage = stage;
        frame.Sp = sp;
        frame.Pc = pc;
        frame.Suspended = true;

        frame.Suspension = stage == DelegateStepFresh
            ? JsSuspension.Yield
            : JsSuspension.Await;

        return value;
    }
}
