// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           1
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    High
// Criteria:         10/8
// Resource impact:  5/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Runtime.CompilerServices;
using Broiler.VM.Abstractions;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    // ---- calling -------------------------------------------------------------------------------

    /// <summary>Calls <paramref name="callee"/>, whatever kind of callable it is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=86C600
    // Broiler-Human:        PENDING
    internal JsValue Call(JsValue callee, JsValue thisValue, JsValue[] arguments)
    {
        if (!callee.IsObject || !callee.AsObject().IsCallable)
        {
            return ThrowTypeError(Describe(callee) + " is not a function");
        }

        EnterCall();

        try
        {
            switch (callee.AsObject())
            {
                // THE PROXY CASE IS FIRST BECAUSE IT IS NOT A FUNCTION. It has no bytecode and no
                // delegate; what it has is an `apply` trap, or a target to forward to. Reaching it
                // through this switch rather than at the call site is what makes every route into a
                // call - a call expression, `Function.prototype.call`, a comparator handed to
                // `sort`, an iterator's `next` - trap alike.
                case JsProxy proxy:
                    return proxy.ProxyCall(thisValue, arguments);

                // A BUILT-IN RUNS IN ITS OWN REALM (JSD-0030 SR-2): another realm's is entered for
                // the call, and the running one stays put for every call that does not cross.
                case JsNativeFunction native:
                    return ReferenceEquals(native.Realm, Realm)
                        ? native.Call(this, thisValue, arguments)
                        : CallInRealm(native, thisValue, arguments);

                case JsBoundFunction bound:
                    return Call(
                        JsValue.Object(bound.Target),
                        bound.BoundThis,
                        Concat(bound.BoundArguments, arguments));

                // A CLASS IS NOT CALLABLE AND THE REFUSAL BELONGS HERE. Every route into a
                // function - a call site, `Function.prototype.call`, a comparison function handed
                // to `sort` - arrives at this switch, and a guard inside the constructor's own
                // code would answer for none of them because the frame is never entered.
                //
                // The error is the CLASS's realm's (10.2.1 step 2: it is made in the callee's
                // context), so a class of another realm called without `new` throws that realm's
                // TypeError.
                case JsScriptFunction script when script.IsClassConstructor:
                    var outer = EnterRealm(script.Realm!);

                    try
                    {
                        return ThrowTypeError(
                            "Class constructor " + script.FunctionName +
                            " cannot be invoked without 'new'");
                    }
                    finally
                    {
                        LeaveRealm(outer);
                    }

                case JsScriptFunction script:
                    return Invoke(script, thisValue, arguments, JsValue.Undefined, null);

                default:
                    return ThrowTypeError("value is not a function");
            }
        }
        finally
        {
            LeaveCall();
        }
    }

    /// <summary>
    /// What every call does between knowing its callee is callable and entering it: its charge, the two
    /// backstops, and the depth it takes.
    /// </summary>
    /// <remarks>
    /// <b>ONE METHOD BECAUSE TWO PATHS MAKE A CALL</b>: <see cref="Call"/>, and the value form's direct call
    /// (JSD-0035 section 6, stage JSV-3), which makes exactly this call's checks, in this order, before it
    /// enters its callee's emitted code - so the charge, the stack probe, the <c>RangeError</c> at the
    /// counted bound and the call-depth ceiling are one text for both, and a value-form recursion answers
    /// the interpreter's error at the interpreter's depth.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=4854FB
    // Broiler-Falsified-If: a call that passes it has not been charged, has not been probed, is past the counted bound without a RangeError, or has not taken one depth that LeaveCall gives back
    // Broiler-Human:        PENDING
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnterCall()
    {
        Charge(CallCharge);
        EnterDepth();
    }

    /// <summary>
    /// The two backstops and the depth a call takes, after its charge: the stack probe, the counted bound's
    /// <c>RangeError</c>, and the call-depth ceiling.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=015600
    // Broiler-Falsified-If: a call that passes it has not been probed, is past the counted bound without a RangeError, or has not taken one depth that LeaveCall gives back
    // Broiler-Human:        PENDING
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnterDepth()
    {
        // THE TWO BACKSTOPS ARE DIFFERENT ANSWERS TO DIFFERENT QUESTIONS, and folding them into one
        // condition - which is what this was - cost the language its own error.
        //
        // The runtime's own stack probe answers "is there room to do ANYTHING here". When it says
        // no there is no safe action left: constructing an error object and dispatching it needs
        // stack the program has already spent, which is the process termination
        // [JSC-85](roadmap.corrections.md#jsc-85) recorded. So that case ends the operation.
        //
        // The counted bound answers a different question - "has this interpreter recursed further
        // than it will promise" - and it is reached with the stack probe still satisfied, so a
        // `RangeError` can be built and thrown. It MUST be thrown rather than aborted, because
        // `Maximum call stack size exceeded` is a catchable exception in every engine and real
        // programs catch it: a recursive descent that probes its own depth, a benchmark that sizes
        // a workload, a conformance case that asserts the error's type. An abort there is a
        // resource exhaustion no guest can see, and the guard the program wrote never runs.
        if (!RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            throw StackBackstopReached();
        }

        if (depth >= MaximumCallDepth && !reportingDepth)
        {
            // THE REPORT NEEDS FRAMES OF ITS OWN, and refusing them is what turned this bound into
            // a process termination *(JSC-85)*. Building the `RangeError` runs `CreateError`, which
            // constructs an object, which re-enters here at a depth already past the bound and
            // throws again — a recursion with no base case, inside the code that exists to refuse
            // one. The flag is the base case. It is cleared as the exception unwinds, so the guest's
            // own `catch` runs with the bound back in force.
            reportingDepth = true;

            try
            {
                ThrowRangeError("Maximum call stack size exceeded");
            }
            finally
            {
                reportingDepth = false;
            }
        }

        if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
        }

        depth++;
    }

    /// <summary>Gives back the depth <see cref="EnterCall"/> took, however the call ended.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0C6441
    // Broiler-Falsified-If: it is run other than once for each EnterCall that returned
    // Broiler-Human:        PENDING
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LeaveCall()
    {
        depth--;
        meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
    }

    /// <summary>Constructs with <paramref name="callee"/>, which is also the <c>new.target</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B91950
    // Broiler-Human:        PENDING
    internal JsValue Construct(JsValue callee, JsValue[] arguments) => Construct(callee, arguments, callee);

    /// <summary>Constructs with <paramref name="callee"/> on behalf of <paramref name="newTarget"/>.</summary>
    /// <remarks>
    /// <para>
    /// <b>The instance is made from <c>new.target</c>'s prototype and not from the callee's</b>,
    /// and the two differ exactly when a derived class calls up: <c>new C()</c> on a three-deep
    /// chain runs <c>A</c>'s constructor with <c>new.target</c> still <c>C</c>, so the object it
    /// creates is a <c>C</c>. Reading the callee's own prototype would have made every instance of
    /// a subclass an instance of its base.
    /// </para>
    /// <para>
    /// <b>A derived constructor is entered with NO instance at all.</b> Its <c>this</c> is
    /// whatever its <c>super()</c> eventually returns, so what is passed down is an empty box the
    /// <c>super()</c> fills; a frame that read it before then gets the <c>ReferenceError</c> the
    /// language promises rather than a half-built object.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=E11FE0
    // Broiler-Human:        PENDING
    internal JsValue Construct(JsValue callee, JsValue[] arguments, JsValue newTarget)
    {
        if (!callee.IsObject || !callee.AsObject().IsConstructor)
        {
            return ThrowTypeError(Describe(callee) + " is not a constructor");
        }

        Charge(8);
        var target = callee.AsObject();

        // THE `new.target` GOES THROUGH UNCHANGED AND IS NOT REPLACED BY THE PROXY. A `construct`
        // trap is handed whatever the construction named, which is what lets a proxied base class
        // build an instance of a derived one; substituting the proxy would make every such instance
        // an instance of the proxy's own target.
        if (target is JsProxy proxied)
        {
            return proxied.ProxyConstruct(arguments, newTarget);
        }

        if (target is JsNativeFunction native)
        {
            // THE NEW TARGET IS HANDED OVER RATHER THAN DROPPED. Every built-in in the profile
            // ignores the receiver slot on this path and goes on ignoring it; what changes is that
            // a body written by an embedder can now answer `new.target`, which is a question a
            // constructor is entitled to ask and had no way to.
            // A BUILT-IN REACHED THROUGH `super()` HIDES THE CONSTRUCTORS THAT REACHED IT from the
            // stack an error it makes captures, down to the one `new` named, as V8 does: the trace
            // of `new Failure()` starts where the program wrote that (JSD-0038).
            var outerSkip = stackSkipUntil;
            stackSkipUntil = ReferenceEquals(newTarget.AsObjectOrNull(), target)
                ? null
                : newTarget.AsObjectOrNull();

            JsValue made;

            try
            {
                made = ReferenceEquals(native.Realm, Realm)
                    ? native.Construct(this, arguments, newTarget)
                    : ConstructInRealm(native, arguments, newTarget);
            }
            finally
            {
                stackSkipUntil = outerSkip;
            }

            // A BUILT-IN REACHED THROUGH `super()` MUST STILL MAKE AN INSTANCE OF THE DERIVED
            // CLASS. `class Failure extends Error { }` is the case that matters: the built-in
            // builds the object, and it builds it against its own prototype because that is all a
            // C# body is given - so without this the instance would be an Error and not a Failure,
            // and `catch (e) { e instanceof Failure }` would be false for an object the program
            // just threw. The re-pointing is skipped when the built-in is what `new` named, which
            // is every ordinary construction, and when the built-in read `new.target` itself
            // (JsNativeFunction.BuildsFromNewTarget), because a second read is a second getter call.
            if (made.IsObject && !native.BuildsFromNewTarget &&
                !ReferenceEquals(newTarget.AsObjectOrNull(), target))
            {
                var wanted = GetProperty(newTarget, "prototype");

                if (wanted.IsObject)
                {
                    made.AsObject().Prototype = wanted.AsObject();
                }
                else if (made.AsObject().Prototype is { } built)
                {
                    // A `prototype` THAT IS NOT AN OBJECT ANSWERS THE INTRINSIC OF NEW.TARGET'S
                    // REALM, which for a new target of another realm is not the one the built-in
                    // used (JSD-0030 SR-7).
                    made.AsObject().Prototype =
                        FunctionRealm(newTarget).CounterpartOf(built, native.Realm!);
                }
                else
                {
                    FunctionRealm(newTarget);
                }
            }

            return made;
        }

        if (target is JsBoundFunction bound)
        {
            // A BOUND FUNCTION'S `new.target` FOLLOWS THROUGH TO ITS TARGET when the bound function
            // is the one being constructed, and is left alone otherwise - which is what makes
            // `new (D.bind(null))()` produce a `D`.
            return Construct(
                JsValue.Object(bound.Target),
                Concat(bound.BoundArguments, arguments),
                ReferenceEquals(newTarget.AsObjectOrNull(), bound)
                    ? JsValue.Object(bound.Target)
                    : newTarget);
        }

        var script = (JsScriptFunction)target;
        var derived = script.IsDerivedConstructor;
        JsObject? instance = null;

        if (!derived)
        {
            var prototype = GetProperty(newTarget, "prototype");

            instance = new JsObject(
                prototype.IsObject ? prototype.AsObject() : FunctionRealm(newTarget).ObjectPrototype);

            // THE FIELDS GO ON BEFORE THE BODY RUNS AND NOT AFTER IT, which is what makes
            // `class C { x = 1; constructor() { this.x += 1 } }` produce a `2`. The specification
            // puts this at the top of a BASE constructor's body evaluation; a derived one gets its
            // own fields when `super()` returns, because until then it has no object to give them
            // to.
            InitialiseInstanceElements(instance, script);
        }

        var binding = new JsCell
        {
            Value = derived ? JsValue.Empty : JsValue.Object(instance!),
        };

        // THE TWO BACKSTOPS ARE DIFFERENT ANSWERS TO DIFFERENT QUESTIONS, and folding them into one
        // condition - which is what this was - cost the language its own error.
        //
        // The runtime's own stack probe answers "is there room to do ANYTHING here". When it says
        // no there is no safe action left: constructing an error object and dispatching it needs
        // stack the program has already spent, which is the process termination
        // [JSC-85](roadmap.corrections.md#jsc-85) recorded. So that case ends the operation.
        //
        // The counted bound answers a different question - "has this interpreter recursed further
        // than it will promise" - and it is reached with the stack probe still satisfied, so a
        // `RangeError` can be built and thrown. It MUST be thrown rather than aborted, because
        // `Maximum call stack size exceeded` is a catchable exception in every engine and real
        // programs catch it: a recursive descent that probes its own depth, a benchmark that sizes
        // a workload, a conformance case that asserts the error's type. An abort there is a
        // resource exhaustion no guest can see, and the guard the program wrote never runs.
        if (!RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            throw StackBackstopReached();
        }

        if (depth >= MaximumCallDepth && !reportingDepth)
        {
            // THE REPORT NEEDS FRAMES OF ITS OWN, and refusing them is what turned this bound into
            // a process termination *(JSC-85)*. Building the `RangeError` runs `CreateError`, which
            // constructs an object, which re-enters here at a depth already past the bound and
            // throws again — a recursion with no base case, inside the code that exists to refuse
            // one. The flag is the base case. It is cleared as the exception unwinds, so the guest's
            // own `catch` runs with the bound back in force.
            reportingDepth = true;

            try
            {
                ThrowRangeError("Maximum call stack size exceeded");
            }
            finally
            {
                reportingDepth = false;
            }
        }

        if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
        }

        depth++;

        try
        {
            var returned = Invoke(script, binding.Value, arguments, newTarget, binding);

            // A CONSTRUCTOR THAT RETURNS AN OBJECT RETURNS THAT OBJECT, and one that returns
            // anything else returns the instance. Getting this backwards makes every factory
            // written as a constructor produce the wrong thing.
            if (returned.IsObject)
            {
                return returned;
            }

            if (!derived)
            {
                return JsValue.Object(instance!);
            }

            // A DERIVED CONSTRUCTOR IS HELD TO MORE THAN A BASE ONE. Returning a primitive other
            // than `undefined` is a TypeError rather than being ignored, and falling off the end
            // without having called `super()` is a ReferenceError rather than producing nothing -
            // which is the check that makes the whole temporal dead zone worth having.
            if (returned.Type != JsType.Undefined)
            {
                return ThrowTypeError(
                    "Derived constructors may only return object or undefined");
            }

            if (binding.Value.IsEmpty)
            {
                return ThrowReferenceError(
                    "Must call super constructor in derived class before accessing 'this' or " +
                    "returning from derived constructor");
            }

            return binding.Value;
        }
        finally
        {
            depth--;
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C521C4
    // Broiler-Human:        PENDING
    private static JsValue[] Concat(JsValue[] first, JsValue[] second)
    {
        if (first.Length == 0)
        {
            return second;
        }

        var joined = new JsValue[first.Length + second.Length];
        Array.Copy(first, joined, first.Length);
        Array.Copy(second, 0, joined, first.Length, second.Length);
        return joined;
    }

    /// <summary>Enters one bytecode function's frame.</summary>
    /// <param name="function">The closure being entered.</param>
    /// <param name="thisValue">The receiver the call site supplied.</param>
    /// <param name="arguments">The actual arguments.</param>
    /// <param name="newTarget">
    /// The constructor a <c>new</c> named, or <c>undefined</c> for an ordinary call.
    /// </param>
    /// <param name="binding">
    /// The box a construction holds its <c>this</c> in, or <see langword="null"/> for a call.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EA8CDE
    // Broiler-Human:        PENDING
    private JsValue Invoke(JsScriptFunction function, JsValue thisValue, JsValue[] arguments, JsValue newTarget, JsCell? binding)
    {
        var program = function.Program;
        var unit = program.Functions[function.Unit];
        var environment = CallEnvironment(function, unit, arguments);
        var receiver = CallReceiver(function, unit, thisValue);

        // CALLING AN ASYNC GENERATOR FUNCTION RUNS NONE OF ITS BODY EITHER, and it is tested BEFORE
        // the two arms below because it carries both of their bits. What it answers is an async
        // generator object rather than a promise: a call of one starts nothing, and the first
        // `next` is what puts the body on the interpreter's stack - which is the generator's half
        // of the pair. Everything asynchronous about it is on the other side of that `next`.
        //
        // ITS PARAMETER LIST IS BOUND AT THE CALL LIKE A PLAIN GENERATOR'S AND NOT LIKE AN ASYNC
        // FUNCTION'S. `EvaluateAsyncGeneratorBody` performs `FunctionDeclarationInstantiation` with
        // a `?` and then creates the object, so a default that throws throws SYNCHRONOUSLY and
        // there is no promise for it to reject - which is the opposite of the async arm below,
        // where the promise is made first and a failing default settles it *(corrected: JSC-159)*.
        if (unit.IsGenerator && unit.IsAsync)
        {
            var body = new JsFrame(program, function.Unit, environment, receiver, arguments, function);

            Charge((body.FrameBytes / 64) + 4);
            BindParameters(program, unit, function, body, receiver, arguments);
            return JsValue.Object(Realm.CreateAsyncGenerator(function, body));
        }

        // CALLING A GENERATOR FUNCTION RUNS NONE OF ITS BODY. The environment above is built and
        // the parameters are bound - both are observable, and both happen at the call - and then
        // the frame is put on the heap and handed back inside a generator object instead of being
        // interpreted. One bit test is what an ordinary call pays for that.
        if (unit.IsGenerator)
        {
            var frame = new JsFrame(program, function.Unit, environment, receiver, arguments, function);

            // THE FRAME IS CHARGED IN PROPORTION TO ITS SIZE, which is the rule this engine already
            // applies to a built-in whose work is proportional to an argument: one instruction may
            // not buy unbounded work. A generator over a unit verified to need a deep operand stack
            // costs more to build than one over a shallow unit, and a program that builds a million
            // of them has spent a million times that.
            Charge((frame.FrameBytes / 64) + 4);
            BindParameters(program, unit, function, frame, receiver, arguments);
            return JsValue.Object(Realm.CreateGenerator(function, frame));
        }

        // CALLING AN ASYNC FUNCTION RUNS ITS BODY, HERE, NOW, ON THIS NATIVE STACK. That is the
        // whole difference from the arm above and a program can see it on its first line:
        // `async function f(){ print(1); await 0; } f(); print(2)` prints 1 before 2, because the
        // body runs to its first `await` before the call returns. What the call returns is the
        // promise, which is already made and still pending unless the body finished without
        // awaiting at all.
        if (unit.IsAsync)
        {
            var frame = new JsFrame(program, function.Unit, environment, receiver, arguments, function)
            {
                // AN ASYNC ARROW TAKES ITS `new.target` AND ITS `this` BOX FROM WHERE IT WAS
                // WRITTEN, exactly as the ordinary path below does - but it has to take them onto
                // the FRAME, because the job that resumes this frame knows nothing about the call
                // site and cannot supply them a second time.
                NewTarget = unit.IsArrow ? function.LexicalNewTarget : newTarget,
                ThisBinding = unit.IsArrow ? function.LexicalThisBinding : binding,
            };

            Charge((frame.FrameBytes / 64) + 4);
            var call = new JsAsyncCall(frame, Realm.NewAsyncPromise());
            ResumeAsync(call, JsResumeMode.Next, JsValue.Undefined);
            return JsValue.Object(call.Promise);
        }

        // AN ARROW TAKES ALL THREE FROM WHERE IT WAS WRITTEN. It has no `this`, no `new.target`
        // and no `super` of its own, so what the call site supplies for any of them is discarded
        // here rather than being allowed to reach the frame.
        var result = ExecuteCall(function, unit, environment, receiver, arguments,
            unit.IsArrow ? function.LexicalNewTarget : newTarget,
            unit.IsArrow ? function.LexicalThisBinding : binding);

        // A TAIL CALL IS MADE HERE, AFTER ITS CALLER'S FRAME HAS GONE (JSC-256). The frame that made
        // it answered with no value and left the callee in `tail`; entering the callee from this loop
        // rather than from inside that frame is what keeps a strict tail recursion at one native
        // frame and one count of depth however deep it goes. Each turn is charged as a call is.
        while (tail.Callee is { } next)
        {
            var nextArguments = tail.Arguments ?? [];
            var nextThis = tail.Receiver;
            tail = default;
            Charge(CallCharge);

            var nextUnit = next.Program.Functions[next.Unit];
            result = ExecuteCall(next, nextUnit, CallEnvironment(next, nextUnit, nextArguments), CallReceiver(next, nextUnit, nextThis), nextArguments,
                nextUnit.IsArrow ? next.LexicalNewTarget : JsValue.Undefined,
                nextUnit.IsArrow ? next.LexicalThisBinding : null);
        }

        return result;
    }

    /// <summary>
    /// Runs one ordinary call frame, recording it while it runs when it is a sloppy plain
    /// function's, for the legacy <c>caller</c> and <c>arguments</c> (JSC-257).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=9A151E
    // Broiler-Falsified-If: a recorded frame outlives its call, or a frame that is not a sloppy plain function's is recorded
    // Broiler-Human:        PENDING
    private JsValue ExecuteCall(JsScriptFunction function, JsCodeUnit unit, JsEnvironment environment, JsValue receiver, JsValue[] arguments, JsValue newTarget, JsCell? binding)
    {
        if (!function.LegacyReflective)
        {
            return Execute(function.Program, function.Unit, environment, receiver, arguments, function,
                newTarget, binding, null, tailCalls: true);
        }

        legacyFrames.Add(new LegacyFrame(function, arguments, environment, depth));

        try
        {
            return Execute(function.Program, function.Unit, environment, receiver, arguments, function,
                newTarget, binding, null, tailCalls: true);
        }
        finally
        {
            legacyFrames.RemoveAt(legacyFrames.Count - 1);
        }
    }

    /// <summary>One running call of a sloppy plain function, and the depth it runs at.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=927B5B
    // Broiler-Human:        PENDING
    private readonly record struct LegacyFrame(JsScriptFunction Function, JsValue[] Arguments, JsEnvironment Environment, int Depth);

    /// <summary>The legacy <c>caller</c> of <paramref name="function"/>'s latest running call.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=68336F
    // Broiler-Falsified-If: it answers a strict function, a built-in, or a function that did not make the call
    // Broiler-Human:        PENDING
    internal JsValue LegacyCaller(JsScriptFunction function)
    {
        for (var at = legacyFrames.Count - 1; at >= 0; at--)
        {
            Charge(1);

            if (!ReferenceEquals(legacyFrames[at].Function, function))
            {
                continue;
            }

            return at > 0 && legacyFrames[at - 1].Depth == legacyFrames[at].Depth - 1
                ? JsValue.Object(legacyFrames[at - 1].Function)
                : JsValue.Null;
        }

        return JsValue.Null;
    }

    /// <summary>
    /// The legacy <c>arguments</c> of <paramref name="function"/>'s latest running call: an
    /// arguments object for it, or <c>null</c> when it is not running.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=A2A2A5
    // Broiler-Falsified-If: it answers an object for a function that is not running, or for a call other than its latest
    // Broiler-Human:        PENDING
    internal JsValue LegacyArguments(JsScriptFunction function)
    {
        for (var at = legacyFrames.Count - 1; at >= 0; at--)
        {
            Charge(1);
            var frame = legacyFrames[at];

            if (!ReferenceEquals(frame.Function, function))
            {
                continue;
            }

            var unit = function.Program.Functions[function.Unit];

            return JsValue.Object(
                Realm.CreateArguments(
                    frame.Arguments,
                    function,
                    false,
                    unit.BindsParameters ? null : frame.Environment,
                    (int)unit.ParameterCount));
        }

        return JsValue.Null;
    }

    /// <summary>
    /// Whether a call in the instruction at <paramref name="current"/>, whose next instruction is
    /// at <paramref name="next"/>, is in tail position of a frame that may make one, and to a
    /// callee that may be entered from the loop.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The language's tail positions are recognised from the code rather than marked in it</b>,
    /// so the format does not change. A call is in tail position when nothing of its frame runs
    /// after it: the instructions after it reach <c>Return</c> through jumps and scope pops alone,
    /// and no exception region covers it - a <c>try</c> would catch what the callee throws, and a
    /// <c>finally</c> would run after it returns. That is the edition's own reading: a call in a
    /// <c>finally</c> block or in a <c>catch</c> with no <c>finally</c> is in tail position, and a
    /// call in a <c>try</c> block is not.
    /// </para>
    /// <para>
    /// <b>Only a strict, ordinary call frame makes one</b>, and only the interpreter: the frame was
    /// entered by <see cref="Invoke"/> as a call (the caller passes that), it is not a generator's or
    /// an async function's, and it has no <c>new.target</c>. The callee is a function this engine
    /// enters the same way - not a class constructor, a generator or an async function, a bound
    /// function, a proxy or a built-in - so the loop that enters it needs nothing but its record.
    /// The emitted forms make every call as an ordinary call, which JSC-256 names.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=C72F91
    // Broiler-Falsified-If: a call is made from the loop while its caller's frame still has an instruction to run, a handler to land in, or a value to construct
    // Broiler-Human:        PENDING
    private static bool InTailPosition(JsProgram program, int unitIndex, int current, int next, JsValue callee)
    {
        if (!callee.IsObject ||
            callee.AsObject() is not JsScriptFunction { IsClassConstructor: false } target)
        {
            return false;
        }

        var entered = target.Program.Functions[target.Unit];

        if (entered.IsGenerator || entered.IsAsync)
        {
            return false;
        }

        var code = program.Code;

        for (var hops = 0; hops < 16; hops++)
        {
            switch ((JsOpcode)code[next])
            {
                case JsOpcode.Return:
                    return !TryFindHandler(program, unitIndex, current, out _);

                case JsOpcode.PopScope:
                    next += JsOpcodes.InstructionWidth(JsOpcode.PopScope);
                    continue;

                case JsOpcode.Jump:
                    next = (int)U32(code, next);
                    continue;

                default:
                    return false;
            }
        }

        return false;
    }


    /// <summary>The environment a call of <paramref name="function"/> enters with, its parameters copied in.</summary>
    /// <remarks>
    /// <b>SHARED WITH THE VALUE FORM'S DIRECT CALL</b> (stage JSV-3), which builds the callee's record exactly as
    /// <see cref="Invoke"/> does before it opens the callee's region.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=032BA1
    // Broiler-Falsified-If: a unit with a simple parameter list is entered with a parameter slot that is not its argument or undefined, or a unit that binds its own parameters with a slot that is not empty
    // Broiler-Human:        PENDING
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static JsEnvironment CallEnvironment(JsScriptFunction function, JsCodeUnit unit, JsValue[] arguments)
    {
        var environment = new JsEnvironment((int)unit.ScopeSlots, function.Environment);

        // A UNIT THAT BINDS ITS OWN PARAMETERS GETS NO COPY AT ALL, and the slots stay EMPTY. That
        // is not an optimisation: a default that reads a later parameter has to find a binding in
        // its temporal dead zone, and filling the slots with `undefined` here would turn that
        // ReferenceError into a silent `undefined`. For a simple parameter list `ParameterCount` is
        // both the arity and the copy count and this is the whole of parameter binding.
        if (!unit.BindsParameters)
        {
            var count = Math.Min(arguments.Length, (int)unit.ParameterCount);

            for (var at = 0; at < count; at++)
            {
                environment.Slots[at] = arguments[at];
            }

            for (var at = count; at < unit.ParameterCount; at++)
            {
                environment.Slots[at] = JsValue.Undefined;
            }
        }

        return environment;
    }

    /// <summary>The receiver a call of <paramref name="function"/> runs with: its own, or the lexical one of an arrow.</summary>
    /// <remarks><b>Shared with the value form's direct call</b>, for <see cref="CallEnvironment"/>'s reason.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=89873B
    // Broiler-Falsified-If: a sloppy non-arrow function is entered with a primitive or nullish receiver, or a strict or arrow one with any receiver but the one the call supplied or the lexical one
    // Broiler-Human:        PENDING
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private JsValue CallReceiver(JsScriptFunction function, JsCodeUnit unit, JsValue thisValue) =>
        unit.IsArrow
            ? function.LexicalThis
            : unit.IsStrict
                ? thisValue
                : thisValue.IsNullish
                    ? JsValue.Object(function.Realm!.GlobalObject)
                    : thisValue.IsObject
                        ? thisValue
                        : JsValue.Object(ToObjectIn(function.Realm!, thisValue));

    /// <summary>
    /// Runs a generator's parameter-binding prologue, at the call, and leaves the frame at the
    /// first instruction of its body.
    /// </summary>
    /// <param name="program">The program the unit lives in.</param>
    /// <param name="unit">The unit being entered.</param>
    /// <param name="function">The closure the call named.</param>
    /// <param name="frame">The heap frame the generator object will hold.</param>
    /// <param name="receiver">The receiver the body will see as <c>this</c>.</param>
    /// <param name="arguments">The actual arguments the prologue binds.</param>
    /// <remarks>
    /// <para>
    /// <b>Binding a parameter list is not part of a generator's body and never was, and the
    /// difference is visible on the first line of a program.</b> The specification runs
    /// <c>FunctionDeclarationInstantiation</c> before it creates the generator object, so
    /// <c>function* g([x = boom()]) {}; g([undefined])</c> throws where it is written - there is no
    /// generator to resume and no <c>next</c> for the failure to wait for. This engine ran the
    /// binding as the first instructions of the body instead, which for a generator meant the first
    /// <c>next</c>, and every destructuring parameter of every generator template in the
    /// conformance suite reported its error one call too late *(corrected: JSC-159)*.
    /// </para>
    /// <para>
    /// <b>Only a unit that BINDS its own parameters pays for this, and that is the same bit the
    /// frame already tests.</b> A simple parameter list is copied into slots by the caller above
    /// with no code at all, so there is nothing here to run and the entry is skipped: a generator
    /// over a simple list costs exactly what it cost before.
    /// </para>
    /// <para>
    /// <b>It is the ORDINARY dispatch loop over the ordinary frame, stopped by an instruction.</b>
    /// The alternative was a second interpreter, or a code unit of its own for the prologue - which
    /// would have needed a second set of slots, a second operand stack and a second set of
    /// exception regions to hold the same scope. What this needs instead is one flag on the frame
    /// and one seam in the stream, and the verifier checks the seam exactly as it checks a
    /// <c>yield</c>.
    /// </para>
    /// <para>
    /// <b>A prologue that ends anywhere but at the seam is an artifact this checkout cannot
    /// produce</b>, and it is refused rather than handed on: the frame would otherwise reach the
    /// generator object pointing at a suspension it has not started, which the first resumption
    /// would re-enter at an operand height the abstract pass never computed for it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=E1EDFC
    // Broiler-Falsified-If: a generator over a non-simple parameter list reports a binding failure at its first resumption rather than at its call
    // Broiler-Human:        PENDING
    private void BindParameters(JsProgram program, JsCodeUnit unit, JsScriptFunction function, JsFrame frame, JsValue receiver, JsValue[] arguments)
    {
        if (!unit.BindsParameters)
        {
            return;
        }

        frame.BindingParameters = true;

        // A GENERATOR BODY IS NEITHER AN ARROW NOR A CONSTRUCTOR - the verifier refuses both
        // pairings - so the two the resumption paths pass as literals are passed as literals here.
        _ = Execute(program, frame.UnitIndex, null, receiver, arguments, function, JsValue.Undefined, null, frame);

        if (frame.BindingParameters)
        {
            frame.BindingParameters = false;

            throw new JsAbort(JsAbortKind.InternalDefect, "a parameter-binding prologue did not reach the body it belongs to");
        }

        // THE FRAME IS NOT SUSPENDED IN THE SENSE THE DRIVERS MEAN. It stopped, and the state it
        // stopped in is `suspendedStart`: nothing has been yielded, `Started` is still false, and
        // the first resumption is the first entry into the body rather than a re-entry into an
        // instruction. Clearing the flag here keeps the two resumption paths reading it as they
        // already do.
        frame.Suspended = false;
    }

    /// <summary>Unpacks the argument Array a spread call built into the array a call takes.</summary>
    /// <remarks>
    /// The Array was built by this lowering and nothing else can reach it, so the dense elements
    /// are read directly. It is the one place in the iteration work where that is honest: the
    /// protocol already ran, when the spread appended.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=90FB8F
    // Broiler-Human:        PENDING
    private JsValue[] ArgumentsOf(JsValue packed)
    {
        var array = (JsArray)packed.AsObject();
        var count = (int)array.Length;

        if (count == 0)
        {
            return [];
        }

        Charge((ulong)count);
        var arguments = new JsValue[count];

        for (var at = 0; at < count; at++)
        {
            var element = at < array.DenseCount ? array.DenseAt(at) : JsValue.Undefined;
            arguments[at] = element.IsEmpty ? JsValue.Undefined : element;
        }

        return arguments;
    }
}
