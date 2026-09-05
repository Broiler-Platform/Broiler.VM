// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   110
// Annotated:        110/110
// Exempt:           14
// Human-reviewed:   0/110
// IP risk:          Low
// Security risk:    High
// Criteria:         15/15
// Resource impact:  7/10 max
// Unverified:       110
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The wide-surface engine: one realm, the abstract operations over it, and the dispatch loop.
/// </summary>
/// <remarks>
/// <para>
/// <b>One class holds the operations and the loop because they call each other in both
/// directions.</b> <c>ToPrimitive</c> calls <c>valueOf</c>, which may be a bytecode function, which
/// runs on the loop, which calls <c>ToPrimitive</c>. Splitting them would mean an interface between
/// two halves of one thing, and the interface would be a delegate field on each side.
/// </para>
/// <para>
/// <b>Fuel is charged per instruction and per call.</b> Not per second: two runs of the same
/// program on two machines stop at the same instruction. A built-in that does bounded work charges
/// once; a built-in whose work is proportional to an argument - sorting, joining, matching - charges
/// proportionally, so a program cannot buy unbounded work with one instruction.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3E740C
// Broiler-Human:        PENDING
internal sealed class JsEngine
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A41ED2
    // Broiler-Human:        PENDING
    private const int FuelPerInstruction = 1;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D9AC66
    // Broiler-Human:        PENDING
    private readonly IVmMeter meter;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F04339
    // Broiler-Human:        PENDING
    private readonly System.Threading.CancellationToken cancellation;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=60EEF1
    // Broiler-Human:        PENDING
    private readonly System.Collections.Immutable.ImmutableArray<string> surfaces;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=329437
    // Broiler-Human:        PENDING
    private int depth;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2AB034
    // Broiler-Human:        PENDING
    private ulong sinceLastPoll;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2E76EA
    // Broiler-Human:        PENDING
    private readonly IVmHostCapabilityInvoker? capabilities;

    /// <summary>Creates an engine over a fresh realm.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EFBE54
    // Broiler-Human:        PENDING
    internal JsEngine(
        IVmMeter contractMeter,
        System.Threading.CancellationToken token,
        IVmHostCapabilityInvoker? invoker = null,
        System.Collections.Immutable.ImmutableArray<string> admittedSurfaces = default)
    {
        meter = contractMeter;
        cancellation = token;
        capabilities = invoker;

        // THE SURFACE SET IS ASSIGNED BEFORE THE REALM IS BUILT AND NOT AFTER, because the realm's
        // constructor is what decides which intrinsics exist. A realm handed the set afterwards
        // would have to be able to grow a global, and a realm that can grow one is a realm whose
        // contents depend on when you looked.
        surfaces = admittedSurfaces.IsDefault
            ? System.Collections.Immutable.ImmutableArray<string>.Empty
            : admittedSurfaces;

        Realm = new JsRealm(this);
    }

    /// <summary>
    /// The jobs that have fallen due and not yet run: the microtask queue, in enqueue order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is a field of the engine and therefore of the realm, not of an invocation.</b> A
    /// promise resolved during one script whose reaction has not run yet is still owed when the
    /// next script starts, which is exactly what a host running several scripts in one realm needs;
    /// an invocation-scoped queue would silently drop it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B30BDD
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Queue<(JsValue Callable, JsValue[] Arguments)> jobs = new();

    /// <summary>Adds one job to the queue.</summary>
    /// <remarks>
    /// <b>Enqueueing is charged.</b> A program that enqueues without bound is a program that has
    /// bought unbounded future work with a bounded present, and the charge is what makes the queue
    /// a thing the allowance covers rather than a hole beside it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=93B789
    // Broiler-Human:        PENDING
    internal void EnqueueJob(JsValue callable, JsValue[] arguments)
    {
        Charge(1);
        Retain(64);
        jobs.Enqueue((callable, arguments));
    }

    /// <summary>Whether any job is waiting.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2DE9B8
    // Broiler-Human:        PENDING
    internal bool HasPendingJobs => jobs.Count != 0;

    /// <summary>
    /// Runs every job that is due, and every job those enqueue, until none is left.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The host decides when this happens and this profile never decides for it.</b> An
    /// embedding that runs one script and stops, one that runs several in one realm, and one that
    /// interleaves them with its own work all want different drain points, and a queue drained
    /// implicitly at a point nobody stated is a behaviour no embedder can reason about. The host
    /// asks by invoking the reserved entry point named in <see cref="JsExecution"/>.
    /// </para>
    /// <para>
    /// <b>A job that never settles is a resource exhaustion and not a hang.</b> A job may enqueue
    /// another job, so this loop is not bounded by the queue's length at entry; what bounds it is
    /// the allowance, charged per job here and per instruction inside each one. A program whose
    /// jobs enqueue jobs for ever spends its fuel and the operation ends naming <c>Fuel</c> —
    /// which is the same answer a program that loops for ever gets, and deliberately so.
    /// </para>
    /// <para>
    /// <b>A job that throws does not stop the drain.</b> The language says an unhandled rejection
    /// is the host's business and that the queue continues; the thrown value is carried out to the
    /// host through the return value rather than being swallowed, and the remaining jobs still run.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=541804
    // Broiler-Falsified-If: a job runs at a point the host did not ask for, or an endless queue is a hang rather than an exhaustion
    // Broiler-Human:        PENDING
    internal JsValue DrainJobs()
    {
        var first = JsValue.Undefined;
        var faulted = false;

        while (jobs.Count != 0)
        {
            Charge(1);
            var (callable, arguments) = jobs.Dequeue();

            try
            {
                _ = Call(callable, JsValue.Undefined, arguments);
            }
            catch (JsThrow thrown)
            {
                if (!faulted)
                {
                    faulted = true;
                    first = thrown.Value;
                }
            }
        }

        if (faulted)
        {
            throw new JsThrow(first, Render(first));
        }

        return JsValue.Undefined;
    }

    /// <summary>Whether the composition admitted the optional surface <paramref name="manifestId"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=486D64
    // Broiler-Human:        PENDING
    internal bool Admits(string manifestId) => surfaces.Contains(manifestId);

    /// <summary>
    /// The mediator this invocation may ask for further executable bytes through, or nothing.
    /// </summary>
    /// <remarks>
    /// <b>It is set per invocation and cleared after it, because that is the contract.</b> A
    /// mediator is valid only for the dynamic extent of the invocation that supplied it, and a
    /// profile that retained one and used it later would be naming a mediator the core reports as
    /// out of scope. Holding it on the engine rather than threading it through every frame is the
    /// only concession, and the clearing is what keeps it honest.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=67C7AE
    // Broiler-Falsified-If: a mediator is used outside the invocation that supplied it
    // Broiler-Human:        PENDING
    internal IVmArtifactLoadMediator? Loader { get; set; }

    /// <summary>
    /// Evaluates a String as a program, through the one route a guest may obtain code by.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Nothing here compiles anything.</b> The source becomes the opaque request payload of a
    /// guest-initiated load, the composition's registered artifact provider answers it with bytes,
    /// and the core verifies those bytes into their own immutable handle before a byte of them
    /// runs — under the requesting operation's remaining allowance, at a nesting depth the core
    /// counts. That is roadmap section 11's design and it is what keeps a compiler inside a
    /// composition's declared closure instead of inside this profile.
    /// </para>
    /// <para>
    /// <b>Two refusals a reader will meet and must not confuse.</b> A composition that DECLINES
    /// <c>broiler.javascript.dynamic</c> never gets here at all: its artifacts naming the surface
    /// were refused at verification, as an invalid artifact the guest never sees. A composition
    /// that admits the surface and registers NO provider gets here and is refused at run time, as
    /// an error the guest may catch. Section 6 draws exactly that distinction and this method is
    /// where the second half of it happens.
    /// </para>
    /// <para>
    /// <b>The direct form is admitted only where it means the same thing as the indirect one.</b>
    /// A direct <c>eval</c> evaluates in the CALLER's scope, and this profile resolves every name
    /// statically at lowering: the artifact the provider answers with was compiled without any
    /// knowledge of the frame that asked for it, so its free names reach the global object. At the
    /// top level of a script that is exactly right, because the caller's scope IS the global scope.
    /// Inside a function it is not, and rather than answer a program that reads a local with a
    /// global's value, this refuses by name and says why. That refusal is the published exclusion,
    /// not a defect to be discovered later.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=AF4B33
    // Broiler-Falsified-If: guest source becomes executable bytes without passing through the mediator
    // Broiler-Human:        PENDING
    internal JsValue Evaluate(JsValue[] arguments, bool direct, Format.JsFormat.FunctionFlags callerFlags)
    {
        var source = arguments.Length == 0 ? JsValue.Undefined : arguments[0];

        // `eval` of anything that is not a String answers it unchanged. It is the one coercion the
        // language deliberately does not do, so that `eval(someObject)` is a value and not a
        // program.
        if (!source.IsString)
        {
            return source;
        }

        if (direct && (callerFlags & Format.JsFormat.FunctionFlags.ProgramBody) == 0)
        {
            throw Error(
                "EvalError",
                "a direct eval inside a function is not admitted: this profile resolves every " +
                "name at lowering, so evaluated source cannot see the calling frame's bindings. " +
                "An indirect eval - (0, eval)(source) - evaluates in the global scope and is " +
                "admitted");
        }

        if (Loader is null)
        {
            throw Error(
                "EvalError",
                "this composition registered no artifact provider, so no source can become code");
        }

        var text = source.AsString();
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);

        // Proportional to the source, because a guest that could buy an unbounded compilation with
        // one instruction would have found the hole every budget dimension exists to close.
        Charge(1 + (ulong)bytes.Length);

        // The request carries the profile's identity, a nesting depth of one, and the source. The
        // core fills in and enforces everything else - the operation the work is charged to, the
        // remaining allowance, the real depth - which is why a profile may state a nesting depth
        // here without that being a way to relax one.
        var request = new VmArtifactRequest(
            JavaScriptProfile.Id,
            default,
            default,
            1,
            default,
            cancellation,
            new VmBytes(bytes));

        var loaded = Loader.RequestLoad(in request);

        // A PROVIDER REFUSAL IS A `SyntaxError` AND NOT AN `EvalError`, and the two are different
        // answers to different questions. `ProviderRefused` is what the mediator reports when the
        // provider it asked answered "this is not a program I will supply" - which, for the only
        // providers this profile's compositions register, means the front end refused the SOURCE.
        // The language says `eval` of source that is not a program throws a `SyntaxError`, and
        // programs test for it: a conformance case that asserts `assert.throws(SyntaxError, ...)`
        // over an evaluated string is checking the language and not this host's plumbing, and an
        // `EvalError` there failed a case whose subject this host answers correctly. Every OTHER
        // way a load can fail - no provider registered, a budget exhausted, the mediator out of
        // scope, a foreign artifact - is this host's own plumbing and keeps the `EvalError` it had.
        if (loaded.Reason == VmReason.ProviderRefused)
        {
            return ThrowSyntaxError("the evaluated source is not a program this profile admits");
        }

        if (loaded.Outcome != VmOutcome.Normal || !loaded.TryGetArtifact(out var artifact))
        {
            throw Error(
                "EvalError",
                "the artifact provider did not supply a program: " +
                    loaded.Outcome.ToString() + "/" + loaded.Reason.ToString());
        }

        if (!artifact.TryGetState(out var state) || state is not JsProgram evaluated)
        {
            throw Error("EvalError", "the artifact provider answered with a foreign program");
        }

        if (!evaluated.TryFindEntry("main", out var unit))
        {
            throw Error("EvalError", "the evaluated program declares no entry point");
        }

        // IT RUNS IN THIS REALM AND NOT IN A NEW ONE. The handle is a separate verified artifact -
        // its own constants, its own code, its own function table - but the global object it
        // reaches is this engine's, which is what makes `eval("var f = function () {}")` define
        // something the calling program can afterwards call.
        return RunEntry(evaluated, unit);
    }

    /// <summary>
    /// Writes one line of text to whatever the composition registered, or nowhere.
    /// </summary>
    /// <remarks>
    /// The two sinks are deliberately different things. <see cref="Output"/> is an in-process hook
    /// a test host sets to capture what a program printed; the capability is the host boundary a
    /// real composition registers. A program that prints reaches both when both exist and neither
    /// when neither does, and in no case does it reach a console this profile opened itself.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=5B318E
    // Broiler-Human:        PENDING
    internal void Write(string text)
    {
        Output?.Invoke(text);

        if (capabilities is null ||
            capabilities.BindingCount <= JavaScriptProfile.WriteBindingIndex ||
            !capabilities.IsBound(JavaScriptProfile.WriteBindingIndex))
        {
            return;
        }

        if (!meter.TryCharge(VmBudgetDimension.HostCalls, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the host-call allowance is spent");
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(text);

        capabilities.InvokeBytes(
            JavaScriptProfile.WriteBindingIndex, new VmBytes(bytes), out _);
    }

    /// <summary>The realm this engine runs in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B48D28
    // Broiler-Human:        PENDING
    internal JsRealm Realm { get; }

    /// <summary>Whatever the host wired to <c>print</c>, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EED672
    // Broiler-Human:        PENDING
    internal System.Action<string>? Output { get; set; }

    /// <summary>
    /// The deepest the call stack may go before the operation ends as a resource exhaustion.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the ORDINARY answer for a recursing program, and the budget ceiling is the
    /// tighter bound a host may impose.</b> The two were the other way round until 2026-09-04, and
    /// the reversal is a correction rather than a preference *(JSC-96)*: while the ceiling answered
    /// first, a stack overflow was a resource exhaustion no guest could see, so
    /// <c>try { recurse(); } catch (e) { }</c> — which a recursive descent probing its own depth, a
    /// benchmark sizing a workload and a conformance case asserting the error's type all write —
    /// never ran its own guard. <c>Maximum call stack size exceeded</c> is a catchable exception in
    /// every engine and it is one here.
    /// </para>
    /// <para>
    /// <b>It is a counted number, and the runtime's stack probe sits in front of it rather than
    /// beside it.</b> The probe answers whether there is room to do anything at all; when it says
    /// no, the operation ENDS, because building and dispatching an error object from there is what
    /// terminated the process at 3,000 frames *(JSC-85)*. The counted bound is reached with the
    /// probe still satisfied, so throwing from it is safe. Folding the two into one condition —
    /// which is what this was — gave the unsafe case's answer to the safe one.
    /// </para>
    /// <para>
    /// <b>The figure is MEASURED and not chosen</b>: <c>eng/measure-frame-cost.py</c> bisects the
    /// published binary against a recursion with no base case and finds that this interpreter
    /// survives 17,963 JavaScript calls on the sixty-four-megabyte stack
    /// <see cref="JsExecution"/> declares — 3,736 bytes of native stack per call, and the same
    /// figure whether the JavaScript frame is narrow or wide, because the operand stack and the
    /// environment are heap objects rather than stack ones. A guest `throw` unwinds from the same
    /// depth, which it did not before the executor caught by FILTER rather than by
    /// catch-and-rethrow *(JSC-97)*. This bound is set at 6,000, under a third of the measurement,
    /// with the margin for a call shape costing more than the measured one and for the frames the
    /// refusal's own error object needs.
    /// </para>
    /// <para>
    /// <b>BOTH HALVES OF THAT MEASUREMENT MOVE WHEN THE INSTRUCTION SET DOES, and re-measuring is
    /// not optional.</b> The per-call cost is the executor's own frame, which a switch sizes for
    /// the widest live set across all of its arms, so every bundle that adds cases to the dispatch
    /// loop grows it. Admitting spread, destructuring and <c>for … of</c> alongside classes grew it
    /// from 1,936 bytes to 3,158 - which put 6,000 frames past what the sixteen megabytes then
    /// declared held, so a runaway recursion terminated the process instead of throwing. That is
    /// JSC-85 exactly, reached by arithmetic rather than by a code change, and it is why the stack
    /// was re-measured and re-declared rather than this bound quietly lowered. Admitting the
    /// generator family grew it again, from 3,158 bytes to 3,463 - the two suspension arms and the
    /// heap frame they read - and the sixty-four megabytes still hold 19,377 calls, so the bound is
    /// still under a third of the capacity and the stack did not have to move. Admitting the ASYNC
    /// family and <c>with</c> grew it once more, from 3,463 bytes to 3,736 - one more suspension
    /// arm, the two locals the async driver carries across its own try, and the scope-chain walk a
    /// dynamic name resolution holds - and the capacity fell from 19,377 calls to 17,963, which is
    /// still more than twice the ceiling a host may be granted. The two families were measured
    /// apart before they were measured together, at 18,277 and 19,288 calls, and NEITHER of those
    /// figures describes the build that ships both: a per-frame cost is a property of the whole
    /// dispatch loop, so it is measured on the tree that has everything in it. The measurement
    /// was re-taken each time anyway, because a bound that is safe by arithmetic nobody re-did is a
    /// bound nobody knows is safe. Admitting the CLASS BODY - fields, static blocks, private names
    /// and a generator member - added six arms and grew it once more, from 3,736 bytes to
    /// <b>4,073</b>, and the capacity fell from 17,963 calls to <b>16,478</b>: 2.75 times this
    /// bound and 2.01 times the ceiling a host may be granted, so nothing had to move
    /// <i>(JSC-126)</i>.
    /// </para>
    /// <para>
    /// <b>AND THE TIME AFTER THAT, SOMETHING DID HAVE TO MOVE.</b> Asynchronous iteration adds five
    /// dispatch arms - the four steps of a <c>for await</c> head and the check its close owes - and
    /// grew the executor's frame from 4,073 bytes to <b>4,551</b>. On the sixty-four megabytes
    /// <see cref="JsExecution"/> then declared that is <b>14,737</b> calls, which is 1.80 times the
    /// ceiling a host may be granted: BELOW the factor of two the previous measurement already
    /// called the narrowest it had been, so the ordering this bound exists to guarantee stopped
    /// being guaranteed. The stack was raised to ninety-six megabytes and re-measured at
    /// <b>22,122</b> calls, which is 3.69 times this bound and 2.70 times that ceiling
    /// <i>(JSC-139)</i>. Raising the stack rather than lowering this bound is the same choice
    /// JSC-85 made and for the same reason: this bound is about what a program may do, and the
    /// stack is about what the machine can hold.
    /// </para>
    /// <para>
    /// <b>An <c>await</c>'s resumption does NOT stack, which is the one thing about this family
    /// that could have made the figure misleading.</b> A <c>yield*</c> chain holds one interpreter
    /// frame per level because each resumption is nested inside the last; an async chain does not,
    /// because every resumption starts from the job queue with the previous frame already returned.
    /// So an async function awaiting for ever spends <c>Fuel</c> and never the stack, and it is the
    /// SYNCHRONOUS part of an async body - a call before the first <c>await</c> - that this bound
    /// governs, exactly as it governs an ordinary call.
    /// </para>
    /// <para>
    /// <b>What a host can still do is narrow it.</b> The <c>CallDepth</c> budget is charged on every
    /// call and its exhaustion is an abort the guest cannot catch, which is what roadmap section 8
    /// asks for in those words. A host that wants a program refused at a hundred frames sets the
    /// ceiling there and gets it. What a host cannot do is widen past this bound, because this bound
    /// is about the native stack rather than about policy.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=321EE5
    // Broiler-Falsified-If: a program recursing past this bound terminates the process rather than throwing a catchable RangeError
    // Broiler-Human:        PENDING
    internal int MaximumCallDepth { get; set; } = 6000;

    /// <summary>Whether this engine is in the middle of reporting a call-depth refusal.</summary>
    /// <remarks>
    /// It exists so the refusal can allocate. Every other reading of it would be a reason to delete
    /// it, and the one that matters is in <see cref="Call"/>: without it the bound refuses the
    /// frames its own error object needs.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=DA5D1D
    // Broiler-Falsified-If: this stays set after the refusal has been thrown, so a later recursion is unbounded
    // Broiler-Human:        PENDING
    private bool reportingDepth;

    // ---- metering ------------------------------------------------------------------------------

    /// <summary>
    /// How much work this engine performs between two polls.
    /// </summary>
    /// <remarks>
    /// It is HALF the profile's declared cancellation poll bound, and the halving is what makes the
    /// declaration true rather than nearly true: a charge is added before the poll is considered,
    /// so the most work that can accumulate between two polls is one window plus the charge that
    /// crossed it. A single charge larger than a window is split, because a built-in charging
    /// proportionally to a megabyte-long string would otherwise breach the bound in one call - which
    /// is what the RegExp benchmark did.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=733ADC
    // Broiler-Human:        PENDING
    private const ulong PollWindow = 16_384;

    /// <summary>Charges fuel, aborting when the allowance is spent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=4AFB70
    // Broiler-Human:        PENDING
    internal void Charge(ulong units)
    {
        while (units > PollWindow)
        {
            ChargeOnce(PollWindow);
            units -= PollWindow;
        }

        ChargeOnce(units);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=8CD7C8
    // Broiler-Human:        PENDING
    private void ChargeOnce(ulong units)
    {
        if (!meter.TryCharge(VmBudgetDimension.Fuel, units))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the instruction allowance is spent");
        }

        sinceLastPoll += units;

        if (sinceLastPoll < PollWindow)
        {
            return;
        }

        sinceLastPoll = 0;

        if (cancellation.IsCancellationRequested)
        {
            throw new JsAbort(JsAbortKind.Cancelled, "cancellation was requested");
        }

        if (!meter.Poll())
        {
            throw new JsAbort(JsAbortKind.Exhausted, "a budget dimension was reached");
        }
    }

    /// <summary>Reports bytes an allocation retained, so LiveBytes stays a ceiling.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2C6F27
    // Broiler-Human:        PENDING
    internal void Retain(ulong bytes) =>
        meter.ReportRetained(VmBudgetDimension.LiveBytes, bytes);

    // ---- throwing ------------------------------------------------------------------------------

    /// <summary>Throws a <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D6FB0C
    // Broiler-Human:        PENDING
    internal JsValue ThrowTypeError(string message) => throw Error("TypeError", message);

    /// <summary>Throws a <c>RangeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=92484D
    // Broiler-Human:        PENDING
    internal JsValue ThrowRangeError(string message) => throw Error("RangeError", message);

    /// <summary>Throws a <c>ReferenceError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=97EC2F
    // Broiler-Human:        PENDING
    internal JsValue ThrowReferenceError(string message) => throw Error("ReferenceError", message);

    /// <summary>Throws a <c>SyntaxError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=173F3B
    // Broiler-Human:        PENDING
    internal JsValue ThrowSyntaxError(string message) => throw Error("SyntaxError", message);

    /// <summary>Builds a throw carrying a fresh Error of the named intrinsic kind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=66C1DC
    // Broiler-Human:        PENDING
    internal JsThrow Error(string kind, string message)
    {
        var error = Realm.CreateError(kind, message);
        return new JsThrow(error, kind + ": " + message);
    }

    // ---- conversions ---------------------------------------------------------------------------

    /// <summary>The abstract operation <c>ToPrimitive</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B7C265
    // Broiler-Human:        PENDING
    internal JsValue ToPrimitive(JsValue value, string hint)
    {
        if (!value.IsObject)
        {
            return value;
        }

        // `Symbol.toPrimitive` COMES FIRST AND ANSWERS FOR THE WHOLE OPERATION. It is what makes a
        // Symbol refuse to become a String, and what lets a Date distinguish the three hints; a
        // conversion that consulted `valueOf` first would have already produced an answer before the
        // object could say it has none.
        if (TryGetSymbolMethod(value, Realm.ToPrimitiveSymbol, out var exotic))
        {
            var answered = Call(exotic, value, [JsValue.String(hint)]);

            if (!answered.IsObject)
            {
                return answered;
            }

            return ThrowTypeError("Cannot convert object to primitive value");
        }

        return OrdinaryToPrimitive(value, hint);
    }

    /// <summary>
    /// The abstract operation <c>OrdinaryToPrimitive</c>: the two methods, in the hint's order.
    /// </summary>
    /// <remarks>
    /// It is separate from <see cref="ToPrimitive"/> because an exotic
    /// <c>Symbol.toPrimitive</c> can need it: a Date's answers the <c>"default"</c> hint by asking
    /// for the <c>"string"</c> ordering, and an implementation that recursed into
    /// <see cref="ToPrimitive"/> to get it would find its own exotic method again.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=5A1F89
    // Broiler-Human:        PENDING
    internal JsValue OrdinaryToPrimitive(JsValue value, string hint)
    {
        var order = string.Equals(hint, "string", System.StringComparison.Ordinal)
            ? new[] { "toString", "valueOf" }
            : ["valueOf", "toString"];

        foreach (var name in order)
        {
            var method = GetProperty(value, name);

            if (method.IsObject && method.AsObject().IsCallable)
            {
                var result = Call(method, value, System.Array.Empty<JsValue>());

                if (!result.IsObject)
                {
                    return result;
                }
            }
        }

        return ThrowTypeError("Cannot convert object to primitive value");
    }

    /// <summary>The abstract operation <c>ToNumber</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3320EE
    // Broiler-Human:        PENDING
    internal double ToNumber(JsValue value) => value.Type switch
    {
        JsType.Number => value.AsNumber(),
        JsType.Boolean => value.AsBoolean() ? 1 : 0,
        JsType.Undefined => double.NaN,
        JsType.Null => 0,
        JsType.String => JsNumberFormat.ToNumber(value.AsString()),

        // A SYMBOL HAS TO BE REFUSED HERE AND NOT LEFT TO THE ARM BELOW. `ToPrimitive` of a
        // primitive is that primitive, so a Symbol reaching the recursive arm converts to itself
        // for ever: the process dies of a stack overflow, which is the one failure this profile
        // may never produce. `ToString` already refuses a Symbol by name; this is the same refusal
        // on the other conversion, and the reason is the same - a Symbol is a key nobody can
        // forge, and a key that silently became a number would be forgeable by arithmetic.
        JsType.Symbol => ThrowTypeError("Cannot convert a Symbol value to a number").AsNumber(),
        _ => ToNumber(ToPrimitive(value, "number")),
    };

    /// <summary>The abstract operation <c>ToString</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=02CC42
    // Broiler-Human:        PENDING
    internal string ToStringValue(JsValue value) => value.Type switch
    {
        JsType.String => value.AsString(),
        JsType.Number => JsNumberFormat.ToJsString(value.AsNumber()),
        JsType.Boolean => value.AsBoolean() ? "true" : "false",
        JsType.Undefined => "undefined",
        JsType.Null => "null",

        // A SYMBOL DOES NOT COERCE, AND THAT IS THE WHOLE POINT OF THE TYPE. Every other primitive
        // has a String it turns into, so a Symbol that also had one would be usable everywhere a
        // String is - which is exactly what a key nobody can forge must not be. `String(symbol)`
        // and `symbol.toString()` are the explicit forms the language nonetheless provides, and
        // they go through the Symbol intrinsic rather than through here.
        JsType.Symbol => ThrowTypeError("Cannot convert a Symbol value to a string").AsString(),
        _ => ToStringValue(ToPrimitive(value, "string")),
    };

    /// <summary>The abstract operation <c>ToObject</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=76DF0A
    // Broiler-Human:        PENDING
    internal JsObject ToObject(JsValue value) => value.Type switch
    {
        JsType.Object => value.AsObject(),
        JsType.String => Realm.WrapString(value.AsString()),
        JsType.Number => new JsPrimitiveWrapper(Realm.NumberPrototype, "Number", value),
        JsType.Boolean => new JsPrimitiveWrapper(Realm.BooleanPrototype, "Boolean", value),
        JsType.Symbol => new JsPrimitiveWrapper(Realm.SymbolPrototype, "Symbol", value),
        _ => (JsObject)ThrowTypeError("Cannot convert undefined or null to object").AsObject(),
    };

    /// <summary>The abstract operation <c>ToInt32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1BA637
    // Broiler-Human:        PENDING
    internal int ToInt32(JsValue value) => JsValue.ToInt32(ToNumber(value));

    /// <summary>The abstract operation <c>ToUint32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=DBC89F
    // Broiler-Human:        PENDING
    internal uint ToUint32(JsValue value) => JsValue.ToUint32(ToNumber(value));

    /// <summary>The abstract operation <c>ToIntegerOrInfinity</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F80693
    // Broiler-Human:        PENDING
    internal double ToInteger(JsValue value) => JsValue.ToInteger(ToNumber(value));

    /// <summary>The abstract operation <c>ToPropertyKey</c>, over the string keys this surface has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7A65ED
    // Broiler-Human:        PENDING
    internal string ToPropertyKey(JsValue value) =>
        value.Type == JsType.String ? value.AsString() : ToStringValue(value);

    /// <summary>
    /// The one Symbol-keyed lookup the engine performs on its own behalf, for a well-known Symbol.
    /// </summary>
    /// <remarks>
    /// Every protocol the language expresses through a well-known Symbol — iteration, primitive
    /// coercion, instance testing — reads a method off a value and calls it. This is that read: it
    /// answers nothing when the property is absent or nullish, and a <c>TypeError</c> when it is
    /// present and not callable, which is what every one of those protocols says to do.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3B7FC6
    // Broiler-Human:        PENDING
    internal bool TryGetSymbolMethod(JsValue value, JsSymbol key, out JsValue method)
    {
        var found = GetSymbol(value, key);

        if (found.IsNullish)
        {
            method = JsValue.Undefined;
            return false;
        }

        if (!found.IsObject || !found.AsObject().IsCallable)
        {
            ThrowTypeError("a Symbol-keyed protocol member is not a function");
        }

        method = found;
        return true;
    }

    // ---- properties ----------------------------------------------------------------------------

    /// <summary>The prototype a primitive's property lookup starts from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=800C9F
    // Broiler-Human:        PENDING
    private JsObject? PrototypeFor(JsValue value) => value.Type switch
    {
        JsType.String => Realm.StringPrototype,
        JsType.Number => Realm.NumberPrototype,
        JsType.Boolean => Realm.BooleanPrototype,
        JsType.Symbol => Realm.SymbolPrototype,
        _ => null,
    };

    /// <summary>Reads a property off any value, walking the prototype chain.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=6EE048
    // Broiler-Human:        PENDING
    internal JsValue GetProperty(JsValue baseValue, string key)
    {
        if (baseValue.IsNullish)
        {
            return ThrowTypeError(
                "Cannot read properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") +
                " (reading '" + key + "')");
        }

        if (baseValue.IsString)
        {
            var text = baseValue.AsString();

            if (string.Equals(key, "length", System.StringComparison.Ordinal))
            {
                return JsValue.Number(text.Length);
            }

            if (JsObject.IsArrayIndex(key, out var at))
            {
                return at < text.Length
                    ? JsValue.String(text[(int)at].ToString())
                    : JsValue.Undefined;
            }
        }

        var start = baseValue.IsObject ? baseValue.AsObject() : PrototypeFor(baseValue);

        // AN INTEGER-INDEXED EXOTIC OBJECT DOES NOT INHERIT ITS INDICES, and that is a property of
        // [[Get]] rather than of [[GetOwnProperty]] — so it cannot be expressed by an override on
        // the object and has to be expressed here. Without it a realm in which somebody wrote
        // `Object.prototype[9] = 42` would answer 42 for `new Int32Array(3)[9]`, where the language
        // says `undefined`: the index is out of the view, and out of the view is the end of the
        // search rather than the start of a walk.
        if (start is JsTypedArray view && JsObject.IsArrayIndex(key, out _))
        {
            return view.TryGetOwnProperty(key, out var element) ? element.Value : JsValue.Undefined;
        }

        return start is null ? JsValue.Undefined : Lookup(start, key, baseValue);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F27834
    // Broiler-Human:        PENDING
    private JsValue Lookup(JsObject start, string key, JsValue receiver)
    {
        var current = start;

        while (current is not null)
        {
            if (current.TryGetOwnProperty(key, out var property))
            {
                if (!property.IsAccessor)
                {
                    return property.Value;
                }

                return property.Getter is null
                    ? JsValue.Undefined
                    : Call(JsValue.Object(property.Getter), receiver, System.Array.Empty<JsValue>());
            }

            current = current.Prototype;
        }

        return JsValue.Undefined;
    }

    /// <summary>Writes a property on any value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B5FA6D
    // Broiler-Human:        PENDING
    internal void SetProperty(JsValue baseValue, string key, JsValue value, bool strict)
    {
        if (baseValue.IsNullish)
        {
            ThrowTypeError(
                "Cannot set properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") +
                " (setting '" + key + "')");

            return;
        }

        var current = baseValue.IsObject ? baseValue.AsObject() : PrototypeFor(baseValue);
        var target = baseValue.AsObjectOrNull();

        // AN ARRAY'S `length` IS THE ONE PROPERTY WHOSE VALUE IS CHECKED BEFORE IT IS STORED, and
        // the check has to be here because it can THROW: `a.length = -1` is a RangeError in every
        // engine, and the object model has no engine to raise one with. What reaches the object is
        // the coerced number, so `a.length = "2"` sets two rather than nothing.
        if (target is JsArray sized && string.Equals(key, "length", System.StringComparison.Ordinal))
        {
            value = JsValue.Number(ArrayLengthOrRefuse(value));
        }

        // A WRITE PAST A CLOSED LENGTH IS A REFUSAL, and in strict code a refusal is a TypeError.
        // The object model drops the write silently because it cannot know the mode, so the mode's
        // half of the answer belongs here.
        if (target is JsArray fixedLength && !fixedLength.LengthWritable &&
            JsObject.IsArrayIndex(key, out var past) && past >= fixedLength.Length)
        {
            if (strict)
            {
                ThrowTypeError("Cannot add property " + key + ", object is not extensible");
            }

            return;
        }

        // THE ELEMENT CONVERSION IS THE ENGINE'S BECAUSE IT CAN RUN `valueOf`. The object model
        // stores an element without an engine to hand, so it can convert a primitive exactly and
        // nothing else; the language says a write to an integer-indexed element is `ToNumber` of
        // whatever was assigned, and `ToNumber` of an object is a call. Doing it here is also what
        // makes the write silently discarded when the index is out of the view or the buffer is
        // detached — after the conversion has happened, which is the order the specification asks
        // for and is observable through a `valueOf` with a side effect.
        if (target is JsTypedArray view && JsObject.IsArrayIndex(key, out var at))
        {
            var number = ToNumber(value);
            _ = view.TryWriteAt((int)at, number);
            return;
        }

        // A NAMESPACE REFUSES EVERY WRITE, and in strict code a refused write is a `TypeError`.
        // The walk below would find an export's own property, see it is writable - which it is,
        // and which a program can read off the descriptor - and let the assignment land in a copy
        // that no longer tracks the module's binding. Module code is always strict, so this is a
        // throw wherever an import is in scope; the sloppy branch is here for a namespace that
        // reached a script through a host.
        if (target is JsModuleNamespace)
        {
            if (strict)
            {
                ThrowTypeError(
                    "Cannot assign to '" + key + "' of a module namespace object");
            }

            return;
        }

        while (current is not null)
        {
            if (current.TryGetOwnProperty(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        if (strict)
                        {
                            ThrowTypeError("Cannot set property " + key + " which has only a getter");
                        }

                        return;
                    }

                    Call(JsValue.Object(property.Setter), baseValue, [value]);
                    return;
                }

                if (!property.Writable)
                {
                    if (strict)
                    {
                        ThrowTypeError("Cannot assign to read only property '" + key + "'");
                    }

                    return;
                }

                if (ReferenceEquals(current, target))
                {
                    property.Value = value;
                    target.SetOwnProperty(key, property);
                    return;
                }

                break;
            }

            current = current.Prototype;
        }

        if (target is null)
        {
            if (strict)
            {
                ThrowTypeError("Cannot create property '" + key + "' on a primitive");
            }

            return;
        }

        if (!target.Extensible)
        {
            if (strict)
            {
                ThrowTypeError("Cannot add property " + key + ", object is not extensible");
            }

            return;
        }

        target.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>An Array length, or the <c>RangeError</c> the language owes for anything else.</summary>
    /// <remarks>
    /// <b>The test is that the number survives the round trip</b>, which is the specification's own
    /// wording and is why <c>-1</c>, <c>1.5</c> and <c>NaN</c> are all refused while <c>"2"</c> is
    /// accepted: each of the three has a <c>ToUint32</c> that differs from its <c>ToNumber</c>, and
    /// the string does not.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4781FB
    // Broiler-Human:        PENDING
    internal uint ArrayLengthOrRefuse(JsValue value)
    {
        var number = ToNumber(value);
        var index = JsValue.ToUint32(number);

        if (index != number)
        {
            throw Error("RangeError", "Invalid array length");
        }

        return index;
    }

    /// <summary>Defines an own data property under a key that is not known until it is evaluated.</summary>
    /// <remarks>
    /// <b>A computed member of an object literal DEFINES and does not assign</b>, and the two differ
    /// wherever the chain has an opinion: <c>{ [k]: v }</c> with <c>k</c> of <c>"__proto__"</c> makes
    /// an own property called <c>__proto__</c>, where an assignment would have found the accessor on
    /// <c>Object.prototype</c> and moved the object's prototype instead. The same difference shows
    /// against any setter, and against a read-only property inherited from a frozen prototype.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=339011
    // Broiler-Human:        PENDING
    private void DefineByKey(JsObject host, JsValue key, JsValue value)
    {
        if (key.IsSymbol)
        {
            host.SetOwnSymbol(key.AsSymbol(), JsProperty.Data(value, JsPropertyAttributes.Default));
            return;
        }

        host.SetOwnProperty(ToPropertyKey(key), JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    // ---- the reflective forms of the two above -------------------------------------------------

    /// <summary>Reads a property off one object's chain with any accessor bound to another value.</summary>
    /// <remarks>
    /// <b>The chain that is walked and the <c>this</c> a getter sees are the same thing in every
    /// ordinary read</b>, because the base of the reference is both, and separating them is the
    /// entire reason <c>Reflect.get</c> takes a third argument. A program can run a getter it found
    /// on one object against an object that does not have it, which is how a class hierarchy reads
    /// an inherited accessor without inheriting.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D12778
    // Broiler-Human:        PENDING
    internal JsValue GetWithReceiver(JsObject target, string key, JsValue receiver)
    {
        if (target is JsTypedArray view && JsObject.IsArrayIndex(key, out _))
        {
            return view.TryGetOwnProperty(key, out var element) ? element.Value : JsValue.Undefined;
        }

        return Lookup(target, key, receiver);
    }

    /// <summary>The same read for a Symbol-keyed property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1E9870
    // Broiler-Human:        PENDING
    internal JsValue GetSymbolWithReceiver(JsObject target, JsSymbol key, JsValue receiver)
    {
        var current = target;

        while (current is not null)
        {
            if (current.TryGetOwnSymbol(key, out var property))
            {
                if (!property.IsAccessor)
                {
                    return property.Value;
                }

                return property.Getter is null
                    ? JsValue.Undefined
                    : Call(JsValue.Object(property.Getter), receiver, System.Array.Empty<JsValue>());
            }

            current = current.Prototype;
        }

        return JsValue.Undefined;
    }

    /// <summary>Writes through one object's chain, lands the write on another, and answers whether
    /// it took.</summary>
    /// <remarks>
    /// <para>
    /// <b>The answer is the point, and it is not the same question as "what does the property read
    /// back as".</b> A setter that discards what it was handed still took the write — the language
    /// says <c>[[Set]]</c> is true whenever a setter ran — and a read-back would call it a refusal.
    /// So this walks the chain itself rather than storing and looking.
    /// </para>
    /// <para>
    /// <b>The walk is over the target and the store is on the receiver</b>, which is what makes a
    /// data property found on a prototype shadow rather than overwrite. The two coincide for every
    /// call that does not name a receiver, which is nearly all of them.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=701115
    // Broiler-Human:        PENDING
    internal bool SetWithReceiver(JsObject target, string key, JsValue value, JsValue receiver)
    {
        if (target is JsTypedArray view && JsObject.IsArrayIndex(key, out var at))
        {
            _ = view.TryWriteAt((int)at, ToNumber(value));
            return true;
        }

        // A NAMESPACE REFUSES EVERY WRITE, INCLUDING ONE TO A NAME IT DOES NOT EXPORT. Its export
        // properties read back as writable - the language says so, and a program can see it in the
        // descriptor - so the walk below would find a writable data property and let the write
        // land, silently turning a live binding into a copy. The refusal is unconditional and is
        // the object's, not the property's, which is why it is decided before the walk begins.
        if (target is JsModuleNamespace)
        {
            return false;
        }

        var current = target;

        while (current is not null)
        {
            if (current.TryGetOwnProperty(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        return false;
                    }

                    Call(JsValue.Object(property.Setter), receiver, [value]);
                    return true;
                }

                if (!property.Writable)
                {
                    return false;
                }

                break;
            }

            current = current.Prototype;
        }

        return LandOnReceiver(receiver, key, value);
    }

    /// <summary>The same write for a Symbol-keyed property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2E77B3
    // Broiler-Human:        PENDING
    internal bool SetSymbolWithReceiver(JsObject target, JsSymbol key, JsValue value, JsValue receiver)
    {
        var current = target;

        while (current is not null)
        {
            if (current.TryGetOwnSymbol(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        return false;
                    }

                    Call(JsValue.Object(property.Setter), receiver, [value]);
                    return true;
                }

                if (!property.Writable)
                {
                    return false;
                }

                break;
            }

            current = current.Prototype;
        }

        if (!receiver.IsObject)
        {
            return false;
        }

        var holder = receiver.AsObject();

        if (holder.TryGetOwnSymbol(key, out var existing))
        {
            if (existing.IsAccessor || !existing.Writable)
            {
                return false;
            }

            existing.Value = value;
            holder.SetOwnSymbol(key, existing);
            return true;
        }

        if (!holder.Extensible)
        {
            return false;
        }

        holder.SetOwnSymbol(key, JsProperty.Data(value, JsPropertyAttributes.Default));
        return true;
    }

    /// <summary>Where a reflective write ends up: an own property of the receiver, or a refusal.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=974BD1
    // Broiler-Human:        PENDING
    private static bool LandOnReceiver(JsValue receiver, string key, JsValue value)
    {
        if (!receiver.IsObject)
        {
            return false;
        }

        var holder = receiver.AsObject();

        if (holder.TryGetOwnProperty(key, out var existing))
        {
            if (existing.IsAccessor || !existing.Writable)
            {
                return false;
            }

            existing.Value = value;
            holder.SetOwnProperty(key, existing);
            return true;
        }

        if (!holder.Extensible)
        {
            return false;
        }

        holder.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
        return true;
    }

    // ---- classes -------------------------------------------------------------------------------

    /// <summary>Reads a <c>this</c> that a derived constructor may not have bound yet.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7893CB
    // Broiler-Human:        PENDING
    private JsValue ThisBinding(JsCell binding)
    {
        if (binding.Value.IsEmpty)
        {
            ThrowReferenceError(
                "Must call super constructor in derived class before accessing 'this' or " +
                "returning from derived constructor");
        }

        return binding.Value;
    }

    /// <summary>
    /// Defines one method-shaped member, and gives it the home object that makes its <c>super</c>
    /// resolve.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The member's name is set here and not where the closure was made</b>, because a computed
    /// key is not known until now: <c>class C { [k]() { } }</c> has to report <c>k</c>'s value as
    /// the method's name, and an accessor reports <c>"get x"</c> rather than <c>"x"</c>. The code
    /// unit carries whatever the source spelled, which is right for the common case and empty for
    /// the computed one.
    /// </para>
    /// <para>
    /// A getter and a setter for one key are one property, so defining either keeps whichever half
    /// is already there - but only when what is already there is an accessor. A data property of
    /// the same name is replaced outright, which is what redeclaring it in a class body means.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=720A94
    // Broiler-Human:        PENDING
    private void DefineMember(JsObject host, string key, JsValue member, byte flags)
    {
        // DEFINING A MEMBER IS DefinePropertyOrThrow AND NOT AN UNCHECKED WRITE, and the one key
        // that can already be there and refuse is a class's own `prototype`: it is not
        // configurable, so `class C { static ['prototype']() { } }` is a TypeError rather than a
        // class whose `prototype` is a method. Every other key a member can name - `name`,
        // `length`, `constructor`, anything an object literal writes - is configurable, so this
        // refuses nothing a program is entitled to do.
        if (host.TryGetOwnProperty(key, out var standing) && !standing.Configurable)
        {
            ThrowTypeError("Cannot redefine property: " + key);
        }

        var getter = (flags & JsOpcodes.MemberIsGetter) != 0;
        var setter = (flags & JsOpcodes.MemberIsSetter) != 0;

        var attributes = JsPropertyAttributes.Configurable |
            ((flags & JsOpcodes.MemberIsEnumerable) != 0
                ? JsPropertyAttributes.Enumerable
                : JsPropertyAttributes.None);

        if (member.IsObject && member.AsObject() is JsScriptFunction bodied)
        {
            bodied.HomeObject = host;
            var label = getter ? "get " + key : setter ? "set " + key : key;
            bodied.FunctionName = label;

            bodied.SetOwnProperty(
                "name",
                JsProperty.Data(JsValue.String(label), JsPropertyAttributes.Configurable));
        }

        if (!getter && !setter)
        {
            host.SetOwnProperty(
                key, JsProperty.Data(member, attributes | JsPropertyAttributes.Writable));

            return;
        }

        host.TryGetOwnProperty(key, out var existing);
        var accessor = member.AsObjectOrNull();

        host.SetOwnProperty(
            key,
            JsProperty.Accessor(
                getter ? accessor : existing.IsAccessor ? existing.Getter : null,
                setter ? accessor : existing.IsAccessor ? existing.Setter : null,
                attributes));
    }

    /// <summary>Defines one member under a Symbol key, which a computed member may name.</summary>
    /// <remarks>
    /// <para>
    /// <b>It is a second method rather than a widened one for the same reason the reads are two
    /// walks</b>: the storage is a separate table, a String key and a Symbol key can never collide,
    /// and there is nothing for the two to agree about beyond the flags — which is why the flag
    /// decoding is the only text repeated here.
    /// </para>
    /// <para>
    /// <b>The function's name is the description in brackets</b>, which is what the language says
    /// and what makes <c>C.prototype[Symbol.iterator].name</c> answer
    /// <c>"[Symbol.iterator]"</c>. A Symbol with no description names an empty function, because
    /// there is nothing to put between the brackets.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EB5634
    // Broiler-Human:        PENDING
    private void DefineSymbolMember(JsObject host, JsSymbol key, JsValue member, byte flags)
    {
        if (host.TryGetOwnSymbol(key, out var standing) && !standing.Configurable)
        {
            ThrowTypeError("Cannot redefine property: " + key.Rendered);
        }

        var getter = (flags & JsOpcodes.MemberIsGetter) != 0;
        var setter = (flags & JsOpcodes.MemberIsSetter) != 0;

        var attributes = JsPropertyAttributes.Configurable |
            ((flags & JsOpcodes.MemberIsEnumerable) != 0
                ? JsPropertyAttributes.Enumerable
                : JsPropertyAttributes.None);

        if (member.IsObject && member.AsObject() is JsScriptFunction bodied)
        {
            bodied.HomeObject = host;
            var described = key.Described ? "[" + key.Description + "]" : string.Empty;
            var label = getter ? "get " + described : setter ? "set " + described : described;
            bodied.FunctionName = label;

            bodied.SetOwnProperty(
                "name",
                JsProperty.Data(JsValue.String(label), JsPropertyAttributes.Configurable));
        }

        if (!getter && !setter)
        {
            host.SetOwnSymbol(
                key, JsProperty.Data(member, attributes | JsPropertyAttributes.Writable));

            return;
        }

        host.TryGetOwnSymbol(key, out var existing);
        var accessor = member.AsObjectOrNull();

        host.SetOwnSymbol(
            key,
            JsProperty.Accessor(
                getter ? accessor : existing.IsAccessor ? existing.Getter : null,
                setter ? accessor : existing.IsAccessor ? existing.Setter : null,
                attributes));
    }

    /// <summary>Builds the object graph a class definition is.</summary>
    /// <remarks>
    /// <b>The heritage is validated before anything is built</b>, so a bad <c>extends</c> leaves no
    /// half-made class behind: a superclass that is neither <c>null</c> nor a constructor, or one
    /// whose <c>prototype</c> is a primitive, is a TypeError at the definition rather than a
    /// surprise at the first <c>new</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=42386B
    // Broiler-Human:        PENDING
    private JsValue BuildClass(JsValue constructor, bool derived, JsValue heritage)
    {
        Charge(8);
        var target = (JsScriptFunction)constructor.AsObject();
        JsObject? inherited = Realm.ObjectPrototype;
        JsObject? constructorParent = Realm.FunctionPrototype;

        if (derived && heritage.Type != JsType.Null)
        {
            if (!heritage.IsObject || !heritage.AsObject().IsConstructor)
            {
                return ThrowTypeError(
                    "Class extends value " + Describe(heritage) +
                    " is not a constructor or null");
            }

            var parentPrototype = GetProperty(heritage, "prototype");

            if (!parentPrototype.IsObject && parentPrototype.Type != JsType.Null)
            {
                return ThrowTypeError(
                    "Class extends value does not have valid prototype property " +
                    Describe(parentPrototype));
            }

            inherited = parentPrototype.AsObjectOrNull();
            constructorParent = heritage.AsObject();
        }
        else if (derived)
        {
            // `extends null` GIVES THE PROTOTYPE NO PROTOTYPE and leaves the constructor an
            // ordinary function object. The class is still DERIVED, which is why constructing one
            // fails: its `super()` has `Function.prototype` above it and that is not a constructor.
            inherited = null;
        }

        var prototype = new JsObject(inherited);

        prototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(
                constructor,
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        target.Prototype = constructorParent;
        target.HomeObject = prototype;

        target.SetOwnProperty(
            "prototype",
            JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));

        return constructor;
    }

    /// <summary>Records one class element on the constructor for later application.</summary>
    /// <remarks>
    /// <para>
    /// <b>The home object is given here and not when the element is applied</b>, because it is a
    /// property of the FUNCTION and the function is created once. A field initialiser's
    /// <c>super.x</c> reads through the class prototype for every instance, not through whatever
    /// object the initialiser happened to run against.
    /// </para>
    /// <para>
    /// <b>A second half of an accessor merges rather than appending</b>, and it merges only into an
    /// element of the same name in the same list. <c>get #a</c> and <c>set #a</c> declare one
    /// private name; two records would install two elements under one name and the second would
    /// hide the first.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=23C0ED
    // Broiler-Human:        PENDING
    private void RecordClassElement(
        JsScriptFunction target, JsObject prototype, JsValue key, JsValue body, byte flags)
    {
        Charge(4);
        var isStatic = (flags & JsOpcodes.ElementIsStatic) != 0;

        if (body.IsObject && body.AsObject() is JsScriptFunction bodied)
        {
            bodied.HomeObject = isStatic ? target : prototype;
        }

        var list = isStatic
            ? target.StaticElements ??= []
            : target.InstanceElements ??= [];

        if ((flags & JsOpcodes.ElementIsSetter) != 0)
        {
            foreach (var standing in list)
            {
                if (standing.Key.IsSymbol && key.IsSymbol &&
                    ReferenceEquals(standing.Key.AsSymbol(), key.AsSymbol()))
                {
                    standing.Setter = body;
                    standing.Flags |= JsOpcodes.ElementIsSetter;
                    return;
                }
            }

            list.Add(
                new JsClassElement { Key = key, Body = JsValue.Undefined, Setter = body, Flags = flags });

            return;
        }

        if ((flags & JsOpcodes.ElementIsGetter) != 0)
        {
            foreach (var standing in list)
            {
                if (standing.Key.IsSymbol && key.IsSymbol &&
                    ReferenceEquals(standing.Key.AsSymbol(), key.AsSymbol()))
                {
                    standing.Body = body;
                    standing.Flags |= JsOpcodes.ElementIsGetter;
                    return;
                }
            }
        }

        list.Add(new JsClassElement { Key = key, Body = body, Setter = JsValue.Undefined, Flags = flags });
    }

    /// <summary>Runs the elements a class body recorded against the constructor itself.</summary>
    /// <remarks>
    /// <b>The private methods go on in a pass of their own, before anything runs.</b> A static
    /// field's initialiser and a static block may both call a private static method, and the class
    /// body is entitled to write the method after them - <c>class C { static a = C.#m(); static
    /// #m() { return 1 } }</c> is an ordinary program. One pass in source order would have made
    /// that a <c>TypeError</c> about a method the class does have.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3723CE
    // Broiler-Human:        PENDING
    private void RunStaticElements(JsScriptFunction target)
    {
        if (target.StaticElements is not { } elements)
        {
            return;
        }

        ApplyClassElements(target, elements, methods: true);
        ApplyClassElements(target, elements, methods: false);
    }

    /// <summary>Gives one new instance the elements its class recorded.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C27CBF
    // Broiler-Human:        PENDING
    private void InitialiseInstanceElements(JsObject instance, JsScriptFunction constructor)
    {
        if (constructor.InstanceElements is not { } elements)
        {
            return;
        }

        ApplyClassElements(instance, elements, methods: true);
        ApplyClassElements(instance, elements, methods: false);
    }

    /// <summary>Applies one pass of a recorded element list to one object.</summary>
    /// <remarks>
    /// <b>A field's initialiser is CALLED and a method is not</b>, which is the whole of what the
    /// method bit decides here. The receiver of the call is the object being given the element, so
    /// <c>class C { x = this.y }</c> reads the instance and <c>class C { static x = this.name }</c>
    /// reads the constructor - one rule, two objects, decided by which list the element was in.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3FE4BF
    // Broiler-Human:        PENDING
    private void ApplyClassElements(
        JsObject target,
        System.Collections.Generic.List<JsClassElement> elements,
        bool methods)
    {
        var receiver = JsValue.Object(target);

        foreach (var element in elements)
        {
            var isMethod = (element.Flags & JsOpcodes.ElementIsMethod) != 0;

            if (isMethod != methods)
            {
                continue;
            }

            Charge(4);

            if (isMethod)
            {
                target.SetPrivate(element.Key.AsSymbol(), PrivateElementOf(element));
                continue;
            }

            if ((element.Flags & JsOpcodes.ElementIsBlock) != 0)
            {
                Call(element.Body, receiver, System.Array.Empty<JsValue>());
                continue;
            }

            var value = element.Body.Type == JsType.Undefined
                ? JsValue.Undefined
                : Call(element.Body, receiver, System.Array.Empty<JsValue>());

            if ((element.Flags & JsOpcodes.ElementIsPrivate) != 0)
            {
                var name = element.Key.AsSymbol();

                // A FIELD DECLARED TWICE ON ONE OBJECT IS A TypeError AND NOT A SECOND WRITE. It is
                // reachable without a duplicate in the source: `class C { #x = 1 }` whose
                // constructor returns an object it has already constructed would install `#x` on it
                // twice, and the language says the second attempt fails.
                if (target.HasPrivate(name))
                {
                    ThrowTypeError(
                        "Cannot initialize " + name.Description + " twice on the same object");
                }

                target.SetPrivate(
                    name, JsProperty.Data(value, JsPropertyAttributes.Writable));

                continue;
            }

            if (element.Key.IsSymbol)
            {
                target.SetOwnSymbol(
                    element.Key.AsSymbol(), JsProperty.Data(value, JsPropertyAttributes.Default));

                continue;
            }

            // A FIELD IS CreateDataPropertyOrThrow AND NOT AN ASSIGNMENT, which is what makes a
            // field shadow an inherited SETTER of the same name rather than calling it. An
            // assignment would have run the setter and defined nothing.
            DefineOwnDataProperty(target, ToPropertyKey(element.Key), value);
        }
    }

    /// <summary>Defines one own data property, refusing where the object already refuses.</summary>
    /// <remarks>
    /// <b>It is <c>CreateDataPropertyOrThrow</c> and not <c>SetOwnProperty</c>.</b> A field lands
    /// on an instance the class body has never seen frozen, so the refusal is rare - but it is
    /// reachable: a derived constructor may return a frozen object, and a class whose fields then
    /// fail to define must say so rather than produce an instance missing them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=640086
    // Broiler-Human:        PENDING
    private void DefineOwnDataProperty(JsObject target, string key, JsValue value)
    {
        if (target.TryGetOwnProperty(key, out var standing)
            ? !standing.Configurable
            : !target.Extensible)
        {
            ThrowTypeError("Cannot define property " + key + ", object is not extensible");
        }

        target.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>The private element one recorded method or accessor installs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=65257B
    // Broiler-Human:        PENDING
    private static JsProperty PrivateElementOf(JsClassElement element)
    {
        var accessor = element.Flags & (JsOpcodes.ElementIsGetter | JsOpcodes.ElementIsSetter);

        if (accessor == 0)
        {
            // NOT WRITABLE, which is what makes `this.#m = 1` a TypeError rather than a
            // replacement of the class's own method on one instance.
            return JsProperty.Data(element.Body, JsPropertyAttributes.None);
        }

        return JsProperty.Accessor(
            element.Body.AsObjectOrNull(),
            element.Setter.AsObjectOrNull(),
            JsPropertyAttributes.None);
    }

    /// <summary>Reads one private element, or says why there is none to read.</summary>
    /// <remarks>
    /// <b>The refusal names the private name and not the object</b>, because the object is usually
    /// the answer's subject rather than its cause: a method extracted from a class and called
    /// against something else meets this, and so does a brand check written as a read. Both are
    /// the same fact - this object was not constructed by the class that minted this name.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1C20CD
    // Broiler-Human:        PENDING
    private JsValue ReadPrivate(JsValue host, JsSymbol name)
    {
        if (!host.IsObject || !host.AsObject().TryGetPrivate(name, out var element))
        {
            return ThrowTypeError(
                "Cannot read private member " + name.Description +
                " from an object whose class did not declare it");
        }

        if (!element.IsAccessor)
        {
            return element.Value;
        }

        // A WRITE-ONLY PRIVATE ACCESSOR IS A TypeError WHEN READ, and not `undefined`. `set #a` on
        // its own declares a name with no getter, and the language refuses the read rather than
        // answering the absence the way a property with no getter does.
        return element.Getter is null
            ? ThrowTypeError("'" + name.Description + "' was defined without a getter")
            : Call(JsValue.Object(element.Getter), host, System.Array.Empty<JsValue>());
    }

    /// <summary>Writes one private element, or says why it cannot be written.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D2D724
    // Broiler-Human:        PENDING
    private void WritePrivate(JsValue host, JsSymbol name, JsValue value)
    {
        if (!host.IsObject || !host.AsObject().TryGetPrivate(name, out var element))
        {
            ThrowTypeError(
                "Cannot write private member " + name.Description +
                " to an object whose class did not declare it");

            return;
        }

        if (element.IsAccessor)
        {
            if (element.Setter is null)
            {
                ThrowTypeError("'" + name.Description + "' was defined without a setter");
                return;
            }

            Call(JsValue.Object(element.Setter), host, [value]);
            return;
        }

        if (!element.Writable)
        {
            ThrowTypeError("Cannot write to private method " + name.Description);
            return;
        }

        host.AsObject().SetPrivate(name, JsProperty.Data(value, JsPropertyAttributes.Writable));
    }

    /// <summary>Runs a <c>super()</c>: constructs the superclass and binds the result as <c>this</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3A0C9B
    // Broiler-Human:        PENDING
    private JsValue SuperConstruct(
        JsScriptFunction? active, JsCell? binding, JsValue newTarget, JsValue[] arguments)
    {
        if (active is null || binding is null)
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect, "a super call reached a frame with no this binding");
        }

        var parent = active.Prototype;

        if (parent is null || !parent.IsConstructor)
        {
            return ThrowTypeError(
                "Super constructor of " + active.FunctionName + " is not a constructor");
        }

        var constructed = Construct(JsValue.Object(parent), arguments, newTarget);

        // CALLING `super()` TWICE IS AN ERROR AND NOT A SECOND BINDING, and the check happens
        // AFTER the superclass has run rather than before. That order is the specification's and
        // it is observable: a second `super()` still constructs the superclass, with whatever the
        // superclass constructor does, and only the attempt to bind the result fails. Checking
        // first would have been the obvious encoding and it makes the superclass's side effects
        // disappear.
        if (!binding.Value.IsEmpty)
        {
            return ThrowReferenceError("Super constructor may only be called once");
        }

        binding.Value = constructed;

        // THE DERIVED CLASS'S OWN FIELDS GO ON HERE AND NOT IN ITS CONSTRUCTOR'S PROLOGUE. The
        // object did not exist until this instant - the BASE constructor made it - so this is the
        // first point at which a derived class has something to install its fields on, and it is
        // also why a field initialiser may read `this` while the constructor's first line may not.
        if (constructed.IsObject)
        {
            InitialiseInstanceElements(constructed.AsObject(), active);
        }

        return constructed;
    }

    /// <summary>The object a <c>super</c> lookup starts from.</summary>
    /// <remarks>
    /// <para>
    /// It is the home object's PROTOTYPE, and the receiver of the lookup is <c>this</c> - the pair
    /// that makes <c>super.m()</c> reach the parent's <c>m</c> and run it against the instance.
    /// Starting at the receiver's prototype instead would find the method itself and recur
    /// forever, which is the defect this design exists to make unrepresentable.
    /// </para>
    /// <para>
    /// <b>A home object with no prototype is a TypeError and not an <c>undefined</c>.</b> The
    /// specification requires the base to be object-coercible before it reads anything through it,
    /// so <c>super.x</c> inside a method of an object whose prototype is <c>null</c> fails the way
    /// <c>null.x</c> fails rather than quietly answering nothing.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=25892B
    // Broiler-Human:        PENDING
    private JsObject SuperBase(JsScriptFunction? active)
    {
        var home = active?.HomeObject;

        if (home is null)
        {
            ThrowTypeError("'super' keyword unexpected here");
        }

        if (home!.Prototype is null)
        {
            ThrowTypeError("Cannot read properties of null (reading a 'super' property)");
        }

        return home.Prototype!;
    }

    /// <summary>Writes a property through the active method's home object.</summary>
    /// <remarks>
    /// The chain the write consults starts above the home object and the write itself lands on
    /// <c>this</c>: an inherited setter runs with <c>this</c> as its receiver, an inherited
    /// non-writable data property refuses the write, and anything else creates or replaces an own
    /// property of the instance rather than touching the prototype it was found on.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A8CBB6
    // Broiler-Human:        PENDING
    private void SetSuper(
        JsObject start, JsValue receiver, string key, JsValue value, bool strict)
    {
        var current = start;

        while (current is not null)
        {
            if (current.TryGetOwnProperty(key, out var found))
            {
                if (found.IsAccessor)
                {
                    if (found.Setter is null)
                    {
                        if (strict)
                        {
                            ThrowTypeError(
                                "Cannot set property " + key + " which has only a getter");
                        }

                        return;
                    }

                    Call(JsValue.Object(found.Setter), receiver, [value]);
                    return;
                }

                if (!found.Writable)
                {
                    if (strict)
                    {
                        ThrowTypeError("Cannot assign to read only property '" + key + "'");
                    }

                    return;
                }

                break;
            }

            current = current.Prototype;
        }

        var instance = receiver.AsObjectOrNull();

        if (instance is null)
        {
            if (strict)
            {
                ThrowTypeError("Cannot create property '" + key + "' on a primitive");
            }

            return;
        }

        if (instance.TryGetOwnProperty(key, out var own) && !own.IsAccessor)
        {
            if (!own.Writable)
            {
                if (strict)
                {
                    ThrowTypeError("Cannot assign to read only property '" + key + "'");
                }

                return;
            }

            own.Value = value;
            instance.SetOwnProperty(key, own);
            return;
        }

        if (!instance.Extensible)
        {
            if (strict)
            {
                ThrowTypeError("Cannot add property " + key + ", object is not extensible");
            }

            return;
        }

        instance.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>The <c>in</c> operator's lookup: does any object in the chain have the key.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=806916
    // Broiler-Human:        PENDING
    internal bool HasProperty(JsObject start, string key)
    {
        // THE SAME EXOTIC RULE [[Get]] OBEYS, AND FOR THE SAME REASON. An integer-indexed object
        // does not inherit its indices, so `9 in new Int32Array(3)` is false whatever anybody put
        // on `Object.prototype`. Expressing it here rather than on the object is forced: the walk
        // is the engine's, not the object's.
        if (start is JsTypedArray view && JsObject.IsArrayIndex(key, out _))
        {
            return view.TryGetOwnProperty(key, out _);
        }

        // A NAMESPACE ANSWERS THIS FROM ITS EXPORT SET AND NEVER READS THE BINDING. The walk below
        // asks each object for the property, and a namespace answers that by reading through to the
        // module's slot - which throws for an export whose module has not run yet. `'x' in ns` is
        // true for such a name and `ns.x` is a `ReferenceError`; going through the walk would make
        // both the same throw. The Symbol table is still the base's, so `@@toStringTag in ns` is
        // answered below by the ordinary path.
        if (start is JsModuleNamespace names)
        {
            return names.Exports(key);
        }

        var current = start;

        while (current is not null)
        {
            if (current.TryGetOwnProperty(key, out _))
            {
                return true;
            }

            current = current.Prototype;
        }

        return false;
    }

    // ---- calling -------------------------------------------------------------------------------

    /// <summary>Calls <paramref name="callee"/>, whatever kind of callable it is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=BD3102
    // Broiler-Human:        PENDING
    internal JsValue Call(JsValue callee, JsValue thisValue, JsValue[] arguments)
    {
        if (!callee.IsObject || !callee.AsObject().IsCallable)
        {
            return ThrowTypeError(Describe(callee) + " is not a function");
        }

        Charge(4);

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
        if (!System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth backstop was reached");
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
            switch (callee.AsObject())
            {
                case JsNativeFunction native:
                    return native.Call(this, thisValue, arguments);

                case JsBoundFunction bound:
                    return Call(
                        JsValue.Object(bound.Target),
                        bound.BoundThis,
                        Concat(bound.BoundArguments, arguments));

                // A CLASS IS NOT CALLABLE AND THE REFUSAL BELONGS HERE. Every route into a
                // function - a call site, `Function.prototype.call`, a comparison function handed
                // to `sort` - arrives at this switch, and a guard inside the constructor's own
                // code would answer for none of them because the frame is never entered.
                case JsScriptFunction script when script.IsClassConstructor:
                    return ThrowTypeError(
                        "Class constructor " + script.FunctionName +
                        " cannot be invoked without 'new'");

                case JsScriptFunction script:
                    return Invoke(script, thisValue, arguments, JsValue.Undefined, null);

                default:
                    return ThrowTypeError("value is not a function");
            }
        }
        finally
        {
            depth--;
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }
    }

    /// <summary>Constructs with <paramref name="callee"/>, which is also the <c>new.target</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B91950
    // Broiler-Human:        PENDING
    internal JsValue Construct(JsValue callee, JsValue[] arguments) =>
        Construct(callee, arguments, callee);

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=920947
    // Broiler-Human:        PENDING
    internal JsValue Construct(JsValue callee, JsValue[] arguments, JsValue newTarget)
    {
        if (!callee.IsObject || !callee.AsObject().IsConstructor)
        {
            return ThrowTypeError(Describe(callee) + " is not a constructor");
        }

        Charge(8);
        var target = callee.AsObject();

        if (target is JsNativeFunction native)
        {
            var made = native.Construct(this, arguments);

            // A BUILT-IN REACHED THROUGH `super()` MUST STILL MAKE AN INSTANCE OF THE DERIVED
            // CLASS. `class Failure extends Error { }` is the case that matters: the built-in
            // builds the object, and it builds it against its own prototype because that is all a
            // C# body is given - so without this the instance would be an Error and not a Failure,
            // and `catch (e) { e instanceof Failure }` would be false for an object the program
            // just threw. The re-pointing is skipped when the built-in is what `new` named, which
            // is every ordinary construction.
            if (made.IsObject && !ReferenceEquals(newTarget.AsObjectOrNull(), target))
            {
                var wanted = GetProperty(newTarget, "prototype");

                if (wanted.IsObject)
                {
                    made.AsObject().Prototype = wanted.AsObject();
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
                prototype.IsObject ? prototype.AsObject() : Realm.ObjectPrototype);

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
        if (!System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth backstop was reached");
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=609082
    // Broiler-Human:        PENDING
    private static JsValue[] Concat(JsValue[] first, JsValue[] second)
    {
        if (first.Length == 0)
        {
            return second;
        }

        var joined = new JsValue[first.Length + second.Length];
        System.Array.Copy(first, joined, first.Length);
        System.Array.Copy(second, 0, joined, first.Length, second.Length);
        return joined;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=CD987E
    // Broiler-Human:        PENDING
    internal string Describe(JsValue value) => value.Type switch
    {
        JsType.Undefined => "undefined",
        JsType.Null => "null",
        JsType.String => "\"" + value.AsString() + "\"",
        JsType.Number => JsNumberFormat.ToJsString(value.AsNumber()),
        JsType.Boolean => value.AsBoolean() ? "true" : "false",

        // A SYMBOL NEEDS ITS OWN ARM AND THE DEFAULT WAS NOT ONE. This described anything that was
        // not one of the five above by asking it whether it was callable, which reads the value as
        // an OBJECT - and a Symbol is not one. So `Symbol()()` - a call of a Symbol, which every
        // engine answers with a TypeError - reached this while building that very TypeError's
        // message, failed the cast, and ended the whole invocation as a contract violation: an
        // internal fault, uncatchable, in place of the language's own error.
        JsType.Symbol => value.AsSymbol().Rendered,
        _ => value.IsObject && value.AsObject().IsCallable ? "function" : "object",
    };

    // ---- the iteration protocol ----------------------------------------------------------------

    /// <summary>
    /// The abstract operation <c>GetIterator</c>: the guest's own protocol, driven from here.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Nothing here shortcuts the protocol for a value this engine happens to recognise.</b>
    /// Spreading an Array calls <c>Array.prototype[Symbol.iterator]</c> and then calls the
    /// <c>next</c> that answered, every time - because a guest may replace either, and a program
    /// that does is entitled to see its own function run. A fast path over the dense elements would
    /// be faster and would answer the wrong thing for exactly the programs that would notice.
    /// </para>
    /// <para>
    /// <b>It reads <c>Symbol.iterator</c> and not a <c>length</c>.</b> An array-like and an
    /// iterable are different things, and a construct that fell back to indices when the Symbol was
    /// absent would iterate a plain object that happened to have a <c>length</c> — which the
    /// language refuses, loudly, and for good reason.
    /// </para>
    /// <para>
    /// <b>One helper for every construct that iterates</b> — spread, <c>for … of</c>, array
    /// destructuring, and any built-in taking an iterable. Each of those is the same three steps
    /// with a different thing done to the value, and writing them separately is how three of them
    /// end up agreeing and the fourth does not.
    /// </para>
    /// <para>
    /// <c>next</c> is read ONCE, here, and the record keeps it. The specification reads it at
    /// <c>GetIterator</c> and calls that same function at every step.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=77B2C3
    // Broiler-Human:        PENDING
    internal JsIteratorRecord GetIterator(JsValue iterable)
    {
        Charge(4);

        if (iterable.IsNullish)
        {
            ThrowTypeError(Describe(iterable) + " is not iterable");
        }

        if (!TryGetSymbolMethod(iterable, Realm.IteratorSymbol, out var method))
        {
            ThrowTypeError(Describe(iterable) + " is not iterable");
        }

        var iterator = Call(method, iterable, System.Array.Empty<JsValue>());

        if (!iterator.IsObject)
        {
            ThrowTypeError("The result of the iterator method is not an object");
        }

        return new JsIteratorRecord(iterator, GetProperty(iterator, "next"));
    }

    /// <summary>
    /// One step of <c>IteratorStep</c>: answers the next value, or that the iterator is finished.
    /// </summary>
    /// <remarks>
    /// <b>A record whose <c>next</c> threw is marked done before the exception leaves.</b> The
    /// specification does not close an iterator whose <c>next</c> failed - it has no reason to
    /// believe the object is in a state that can answer <c>return</c> - and the flag is what carries
    /// that decision to the <c>IterateClose</c> the lowering emits unconditionally.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=780D55
    // Broiler-Human:        PENDING
    internal bool TryIterateNext(JsIteratorRecord record, out JsValue value) =>
        TryIterateNext(record, System.Array.Empty<JsValue>(), out value, out _);

    /// <summary>
    /// The same step, with the argument <c>next</c> is called with and the value a DONE step
    /// carried.
    /// </summary>
    /// <remarks>
    /// <b>Both extras exist for <c>yield*</c> and for nothing else, which is why they are an
    /// overload rather than a second walk over the protocol.</b> A delegation forwards what
    /// <c>gen.next(v)</c> sent to the inner iterator's own <c>next</c>, and what <c>yield*</c>
    /// EVALUATES TO is the value of the step that reported done - which every other construct
    /// discards, and which a helper that only answered "finished" could not give back. The
    /// argument list is passed through rather than built here so that the ordinary overload calls
    /// <c>next</c> with NO arguments, exactly as <c>IteratorNext</c> does when it has no value to
    /// send: an iterator written in the guest can see the difference in its own
    /// <c>arguments.length</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=47A7F8
    // Broiler-Human:        PENDING
    internal bool TryIterateNext(
        JsIteratorRecord record,
        JsValue[] sent,
        out JsValue value,
        out JsValue completed,
        bool wantsCompleted = false)
    {
        value = JsValue.Undefined;
        completed = JsValue.Undefined;

        if (record.Done)
        {
            return false;
        }

        Charge(2);
        JsValue result;

        try
        {
            result = Call(record.Next, record.Iterator, sent);
        }
        catch (JsThrow)
        {
            record.Done = true;
            throw;
        }

        if (!result.IsObject)
        {
            record.Done = true;
            ThrowTypeError("Iterator result " + Describe(result) + " is not an object");
        }

        // THE `value` OF A DONE RESULT IS READ ONLY BY A CALLER THAT WANTS IT, which is one: the
        // `yield*` delegation, whose own value is the inner iterator's return value. Reading it for
        // everybody is observable through a getter - the pinned suite's set-like iterators count
        // exactly these reads - and a `for … of` that read it would be asking a question the
        // language does not ask.
        if (GetProperty(result, "done").ToBooleanValue())
        {
            record.Done = true;

            if (wantsCompleted)
            {
                completed = GetProperty(result, "value");
            }

            return false;
        }

        value = GetProperty(result, "value");
        return true;
    }

    /// <summary>
    /// <c>IteratorClose</c> under a normal or a <c>break</c>-shaped completion.
    /// </summary>
    /// <remarks>
    /// Errors from <c>return</c> propagate here, and a <c>return</c> answering a non-object is
    /// itself a <c>TypeError</c> - both of which a guest can observe and both of which a quiet close
    /// would swallow.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=48D728
    // Broiler-Human:        PENDING
    internal void CloseIterator(JsIteratorRecord record)
    {
        if (record.Done)
        {
            return;
        }

        record.Done = true;
        var method = GetProperty(record.Iterator, "return");

        if (method.IsNullish)
        {
            return;
        }

        if (!method.IsObject || !method.AsObject().IsCallable)
        {
            ThrowTypeError("The iterator's return is not a function");
        }

        var result = Call(method, record.Iterator, System.Array.Empty<JsValue>());

        if (!result.IsObject)
        {
            ThrowTypeError("The iterator's return answered " + Describe(result) + " and not an object");
        }
    }

    /// <summary>
    /// <c>IteratorClose</c> under a throw completion, which discards whatever <c>return</c> does.
    /// </summary>
    /// <remarks>
    /// <b>The exception already in flight is the one the program is owed.</b> A <c>for … of</c>
    /// body that throws still has to give the iterator its <c>return</c>, but replacing the body's
    /// exception with one the clean-up raised would report the second failure and lose the first.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7F0280
    // Broiler-Human:        PENDING
    internal void CloseIteratorQuietly(JsIteratorRecord record)
    {
        if (record.Done)
        {
            return;
        }

        record.Done = true;

        try
        {
            var method = GetProperty(record.Iterator, "return");

            if (method.IsObject && method.AsObject().IsCallable)
            {
                Call(method, record.Iterator, System.Array.Empty<JsValue>());
            }
        }
        catch (JsThrow)
        {
            // Deliberately swallowed: see the remark.
        }
    }

    /// <summary>Drains an iterable into a list, closing nothing because it ran to completion.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F6C82A
    // Broiler-Human:        PENDING
    internal void IterateInto(JsValue iterable, System.Collections.Generic.List<JsValue> into)
    {
        var record = GetIterator(iterable);

        while (TryIterateNext(record, out var element))
        {
            into.Add(element);
        }
    }

    /// <summary>Drains a record that is already open, which is what a rest element takes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=AA5907
    // Broiler-Human:        PENDING
    internal JsArray DrainIterator(JsIteratorRecord record)
    {
        var rest = Realm.NewArray();

        while (TryIterateNext(record, out var element))
        {
            rest.Push(element);
        }

        return rest;
    }

    /// <summary>
    /// The abstract operation <c>CopyDataProperties</c>, which is what object spread is.
    /// </summary>
    /// <remarks>
    /// <b>Own and enumerable, in the order the source yields them, and through the ordinary read
    /// path.</b> Reading through the property path is what makes a getter on the source run once
    /// and contribute its value, which is what the language says and what copying descriptors would
    /// not do. A <c>null</c> or <c>undefined</c> source contributes nothing rather than throwing -
    /// <c>{...null}</c> is an empty object.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=ADD0F8
    // Broiler-Human:        PENDING
    internal void CopyDataProperties(JsObject target, JsValue source)
    {
        if (source.IsNullish)
        {
            return;
        }

        var from = ToObject(source);

        foreach (var key in from.OwnPropertyNames())
        {
            Charge(1);

            if (!from.TryGetOwnProperty(key, out var property) || !property.Enumerable)
            {
                continue;
            }

            target.SetOwnProperty(
                key,
                JsProperty.Data(GetProperty(source, key), JsPropertyAttributes.Default));
        }
    }

    /// <summary>Runs a program's entry point and answers what it completed with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3D2B88
    // Broiler-Human:        PENDING
    internal JsValue RunEntry(JsProgram program, uint unit)
    {
        if (program.ModuleOfUnit[(int)unit] is var moduleIndex and >= 0)
        {
            return RunModuleGraph(program, moduleIndex);
        }

        var code = program.Functions[(int)unit];
        var environment = new JsEnvironment((int)code.ScopeSlots, null);

        return Execute(
            program,
            (int)unit,
            environment,
            JsValue.Object(Realm.GlobalObject),
            System.Array.Empty<JsValue>(),
            null,
            JsValue.Undefined,
            null,
            null);
    }

    // ---- modules -------------------------------------------------------------------------------

    /// <summary>The module instances of this realm, created the first time a module is entered.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3D208B
    // Broiler-Human:        PENDING
    private JsModuleInstance[]? modules;

    /// <summary>
    /// Links and evaluates the graph rooted at one module, and answers what its body completed with.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Three phases, and the order is the specification's rather than a convenience.</b> Every
    /// module's environment is created, then every module's declarations are initialised, then the
    /// bodies are evaluated in the order a depth-first walk from the root leaves them. Collapsing
    /// the second phase into the third is the change that breaks a legal cyclic program: the module
    /// that runs first calls a function of the one that has not, and that function has to already
    /// exist.
    /// </para>
    /// <para>
    /// <b>Linking happens once per realm and evaluation happens once per module.</b> A second
    /// invocation of the root entry point answers what the first completed with rather than running
    /// the graph again, which is what the language says of a module that is already evaluated.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=7F9904
    // Broiler-Falsified-If: a module body runs twice in one realm, or a module body runs before every module's declarations are initialised
    // Broiler-Human:        PENDING
    private JsValue RunModuleGraph(JsProgram program, int root)
    {
        if (modules is null)
        {
            modules = new JsModuleInstance[program.Modules.Length];

            for (var index = 0; index < modules.Length; index++)
            {
                var record = program.Modules[index];
                var slots = (int)program.Functions[(int)record.BodyUnit].ScopeSlots;
                modules[index] = new JsModuleInstance(new JsEnvironment(slots, null));
            }

            for (var index = 0; index < modules.Length; index++)
            {
                modules[index].Namespace =
                    new JsModuleNamespace(program.Modules[index], modules, this);
            }

            Confirm(program);

            for (var index = 0; index < modules.Length; index++)
            {
                Charge(FuelPerInstruction);

                Execute(
                    program,
                    (int)program.Modules[index].InitialiserUnit,
                    modules[index].Environment,
                    JsValue.Undefined,
                    System.Array.Empty<JsValue>(),
                    null,
                    JsValue.Undefined,
                    null,
                    null);

                modules[index].State = JsModuleState.Initialised;
            }
        }

        var order = new System.Collections.Generic.List<int>(program.Modules.Length);
        Order(program, root, order);
        Step(program, order, 0);
        return modules[root].Completion;
    }

    /// <summary>
    /// Puts the graph rooted at one module into the order its bodies must be evaluated in.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The walk ORDERS and does not evaluate, which is what lets a module await.</b> A walk that
    /// evaluated as it descended would hold the rest of the graph on the native stack, and a module
    /// that suspended half way would have to be resumed into a stack that no longer exists. An
    /// explicit order is a list a continuation can carry, so the module after an awaiting one is
    /// reached from a job rather than from a frame.
    /// </para>
    /// <para>
    /// A module already on the walk is skipped rather than descended into, which is what makes a
    /// cyclic import terminate here.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=8D99CC
    // Broiler-Human:        PENDING
    private void Order(JsProgram program, int index, System.Collections.Generic.List<int> order)
    {
        var instance = modules![index];

        if (instance.State is JsModuleState.Ordered or JsModuleState.Evaluating or
            JsModuleState.Evaluated)
        {
            return;
        }

        instance.State = JsModuleState.Ordered;
        Charge(FuelPerInstruction);

        foreach (var request in program.Modules[index].Requests)
        {
            Order(program, request, order);
        }

        order.Add(index);
    }

    /// <summary>
    /// Evaluates the ordered modules from <paramref name="at"/>, pausing where one awaits.
    /// </summary>
    /// <remarks>
    /// <b>A module that suspends does not hold the walk open - it schedules the rest of it.</b> The
    /// continuation is registered on the promise the module's own evaluation answers with, so the
    /// module after it runs when that settles and not before, which is the ordering guarantee
    /// top-level <c>await</c> exists to give. Nothing here drains the queue: the host does that, at
    /// a point the host chooses, exactly as it does for any other asynchronous program.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=F0048D
    // Broiler-Falsified-If: a module body runs before a module it requested has finished awaiting
    // Broiler-Human:        PENDING
    private void Step(JsProgram program, System.Collections.Generic.List<int> order, int at)
    {
        for (var position = at; position < order.Count; position++)
        {
            var index = order[position];
            var instance = modules![index];

            if (instance.State == JsModuleState.Evaluated)
            {
                continue;
            }

            instance.State = JsModuleState.Evaluating;
            var record = program.Modules[index];
            var unit = program.Functions[(int)record.BodyUnit];

            if (!unit.IsAsync)
            {
                instance.Completion = Execute(
                    program,
                    (int)record.BodyUnit,
                    instance.Environment,
                    JsValue.Undefined,
                    System.Array.Empty<JsValue>(),
                    null,
                    JsValue.Undefined,
                    null,
                    null);

                instance.State = JsModuleState.Evaluated;
                continue;
            }

            var promise = StartAsyncModule(program, record, instance);
            instance.State = JsModuleState.Evaluated;

            // A MODULE THAT RAN TO ITS END WITHOUT SUSPENDING NEEDS NO CONTINUATION, and taking
            // one anyway would put the rest of the graph on the job queue for no reason - which is
            // observable, because a reaction registered by the next module would then run first.
            if (promise.State != JsPromiseState.Pending)
            {
                continue;
            }

            var resume = position + 1;

            Realm.AwaitOn(
                this,
                JsValue.Object(promise),
                (engine, value, threw) =>
                {
                    if (threw)
                    {
                        throw new JsThrow(value, engine.Render(value));
                    }

                    engine.Step(program, order, resume);
                });

            return;
        }
    }

    /// <summary>Enters one module body as an async frame and answers the promise it settles.</summary>
    /// <remarks>
    /// The frame needs a function to name the unit it is running, and a module body is nobody's
    /// closure - so one is made over the module's own environment. It is never called and never
    /// reaches the guest; what it carries is the unit index and the scope chain the frame would
    /// otherwise have to be told twice.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=07269D
    // Broiler-Human:        PENDING
    private JsPromiseObject StartAsyncModule(
        JsProgram program, JsModuleRecord record, JsModuleInstance instance)
    {
        var body = new JsScriptFunction(
            Realm.FunctionPrototype, program, (int)record.BodyUnit, instance.Environment);

        var frame = new JsFrame(
            program,
            (int)record.BodyUnit,
            instance.Environment,
            JsValue.Undefined,
            System.Array.Empty<JsValue>(),
            body);

        Charge((frame.FrameBytes / 64) + 4);
        var call = new JsAsyncCall(frame, Realm.NewAsyncPromise());
        ResumeAsync(call, JsResumeMode.Next, JsValue.Undefined);
        return call.Promise;
    }

    /// <summary>
    /// Puts every module request to the composition, and refuses a graph it resolves differently.
    /// </summary>
    /// <remarks>
    /// <b>The profile never derives a key and this is why it does not have to.</b> The artifact
    /// states what its producer resolved each specifier to; this asks the composition whether that
    /// is its own answer, one request at a time, and a <c>Refused</c> ends the run by name. So a
    /// graph bundled under one host's resolution rules cannot be evaluated under another's, and no
    /// filesystem, URL scheme or search path is known to this component at all.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=4B22CD
    // Broiler-Falsified-If: a module request is honoured without the composition being asked, or a refusal is treated as an answer
    // Broiler-Human:        PENDING
    private void Confirm(JsProgram program)
    {
        if (capabilities is null ||
            capabilities.BindingCount <= JavaScriptProfile.ResolveBindingIndex ||
            !capabilities.IsBound(JavaScriptProfile.ResolveBindingIndex))
        {
            ThrowTypeError("this composition provides no module resolver");
            return;
        }

        foreach (var record in program.Modules)
        {
            for (var index = 0; index < record.Requests.Length; index++)
            {
                Charge(FuelPerInstruction);
                var target = program.Modules[record.Requests[index]];

                // THE FORMAT'S OWN TEXT ENCODING AND NOT THE PLATFORM'S. A module specifier is a
                // JavaScript String, so it may hold an unpaired surrogate that UTF-8 cannot carry -
                // and the platform's encoder answers that by THROWING, which turned a conformance
                // case about a lone surrogate in an export name into a harness crash. The format
                // already defines an encoding for exactly this and the request uses it.
                var request = JsFormat.EncodeText(
                    record.Key + "\0" + record.RequestSpecifiers[index] + "\0" + target.Key);

                if (!meter.TryCharge(VmBudgetDimension.HostCalls, 1))
                {
                    throw new JsAbort(JsAbortKind.Exhausted, "the host-call allowance is spent");
                }

                var outcome = capabilities.InvokeBytes(
                    JavaScriptProfile.ResolveBindingIndex, new VmBytes(request), out _);

                if (outcome != VmHostCallOutcome.Completed)
                {
                    ThrowTypeError(
                        "this composition does not resolve '" + record.RequestSpecifiers[index] +
                        "' from '" + record.Key + "' to '" + target.Key + "'");
                }
            }
        }
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C7B2C6
    // Broiler-Human:        PENDING
    private JsValue Invoke(
        JsScriptFunction function,
        JsValue thisValue,
        JsValue[] arguments,
        JsValue newTarget,
        JsCell? binding)
    {
        var program = function.Program;
        var unit = program.Functions[function.Unit];
        var environment = new JsEnvironment((int)unit.ScopeSlots, function.Environment);

        // A UNIT THAT BINDS ITS OWN PARAMETERS GETS NO COPY AT ALL, and the slots stay EMPTY. That
        // is not an optimisation: a default that reads a later parameter has to find a binding in
        // its temporal dead zone, and filling the slots with `undefined` here would turn that
        // ReferenceError into a silent `undefined`. For a simple parameter list `ParameterCount` is
        // both the arity and the copy count and this is the whole of parameter binding.
        if (!unit.BindsParameters)
        {
            var count = System.Math.Min(arguments.Length, (int)unit.ParameterCount);

            for (var at = 0; at < count; at++)
            {
                environment.Slots[at] = arguments[at];
            }

            for (var at = count; at < unit.ParameterCount; at++)
            {
                environment.Slots[at] = JsValue.Undefined;
            }
        }

        var receiver = unit.IsArrow
            ? function.LexicalThis
            : unit.IsStrict
                ? thisValue
                : thisValue.IsNullish
                    ? JsValue.Object(Realm.GlobalObject)
                    : thisValue.IsObject
                        ? thisValue
                        : JsValue.Object(ToObject(thisValue));

        // CALLING AN ASYNC GENERATOR FUNCTION RUNS NONE OF ITS BODY EITHER, and it is tested BEFORE
        // the two arms below because it carries both of their bits. What it answers is an async
        // generator object rather than a promise: a call of one starts nothing, and the first
        // `next` is what puts the body on the interpreter's stack - which is the generator's half
        // of the pair. Everything asynchronous about it is on the other side of that `next`.
        if (unit.IsGenerator && unit.IsAsync)
        {
            var body = new JsFrame(program, function.Unit, environment, receiver, arguments, function);

            Charge((body.FrameBytes / 64) + 4);
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
        return Execute(
            program,
            function.Unit,
            environment,
            receiver,
            arguments,
            function,
            unit.IsArrow ? function.LexicalNewTarget : newTarget,
            unit.IsArrow ? function.LexicalThisBinding : binding,
            null);
    }

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=984439
    // Broiler-Falsified-If: a generator resumed while its own body is running re-enters that body, or a completed generator runs any instruction
    // Broiler-Human:        PENDING
    internal JsValue ResumeGenerator(JsValue receiver, JsResumeMode mode, JsValue sent, string method)
    {
        if (receiver.AsObjectOrNull() is not JsGenerator generator)
        {
            return ThrowTypeError(
                "Generator.prototype." + method + " called on a value that is not a generator");
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
        if (depth >= MaximumCallDepth ||
            !System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
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
            var completed = Execute(
                frame.Program,
                frame.UnitIndex,
                null,
                frame.ThisValue,
                frame.Arguments,
                frame.Function,

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=99B3DF
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
        if (depth >= MaximumCallDepth ||
            !System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            call.Frame = null;
            Realm.SettleAsyncPromise(
                this,
                call.Promise,
                Error("RangeError", "Maximum call stack size exceeded").Value,
                rejected: true);

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
            var completed = Execute(
                body.Program,
                body.UnitIndex,
                null,
                body.ThisValue,
                body.Arguments,
                body.Function,
                body.NewTarget,
                body.ThisBinding,
                body);

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
            Realm.SettleAsyncPromise(this, call.Promise, outcome, rejected);
            return;
        }

        // THE AWAIT IS PERFORMED HERE AND NOT IN THE OPCODE, for the reason the `Yield` case hands
        // its value out rather than acting on it: the opcode's whole job is to leave the frame in a
        // resumable state, and everything that depends on WHO resumes belongs to the driver.
        Realm.AwaitOn(
            this,
            outcome,
            (engine, value, threw) => engine.ResumeAsync(
                call, threw ? JsResumeMode.Throw : JsResumeMode.Next, value));
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
    internal JsValue EnqueueAsyncGenerator(
        JsValue receiver, JsResumeMode mode, JsValue sent, string method)
    {
        if (receiver.AsObjectOrNull() is not JsAsyncGenerator generator)
        {
            return Realm.RejectedPromise(
                this,
                "AsyncGenerator.prototype." + method +
                " called on a value that is not an async generator");
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A0B79D
    // Broiler-Human:        PENDING
    private void ResumeAsyncGeneratorAtYield(
        JsAsyncGenerator generator, JsResumeMode mode, JsValue sent)
    {
        generator.State = JsAsyncGeneratorState.Executing;

        if (mode != JsResumeMode.Return)
        {
            ResumeAsyncGenerator(generator, mode, sent);
            return;
        }

        Realm.AwaitOn(
            this,
            sent,
            (engine, value, threw) => engine.ResumeAsyncGenerator(
                generator, threw ? JsResumeMode.Throw : JsResumeMode.Return, value));
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=30F939
    // Broiler-Falsified-If: an async generator whose body is on the interpreter's stack is resumed again, or an `await` inside an async generator body settles a request the way a `yield` does
    // Broiler-Human:        PENDING
    private void ResumeAsyncGenerator(
        JsAsyncGenerator generator, JsResumeMode mode, JsValue carried)
    {
        while (true)
        {
            if (generator.Frame is null || generator.Running)
            {
                return;
            }

            if (depth >= MaximumCallDepth ||
                !System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
            {
                generator.Frame = null;
                generator.State = JsAsyncGeneratorState.DrainingQueue;

                Realm.CompleteAsyncGeneratorStep(
                    this,
                    generator,
                    Error("RangeError", "Maximum call stack size exceeded").Value,
                    done: true,
                    rejected: true);

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
                outcome = Execute(
                    body.Program,
                    body.UnitIndex,
                    null,
                    body.ThisValue,
                    body.Arguments,
                    body.Function,

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
                Realm.AwaitOn(
                    this,
                    outcome,
                    (engine, value, threw) => engine.ResumeAsyncGenerator(
                        generator, threw ? JsResumeMode.Throw : JsResumeMode.Next, value));

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
            Realm.CompleteAsyncGeneratorStep(
                this,
                generator,
                request.Mode == JsResumeMode.Throw ? request.Value : JsValue.Undefined,
                done: true,
                rejected: request.Mode == JsResumeMode.Throw);
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
            Realm.AwaitOn(
                this,
                request.Value,
                (engine, value, threw) =>
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

    // ---- the loop ------------------------------------------------------------------------------

    /// <summary>
    /// The dispatch loop, over an ordinary frame or over a generator's heap-allocated one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A <see langword="null"/> <paramref name="frame"/> is the ordinary path and it is
    /// unchanged.</b> The operand stack, the scope chain, the height and the instruction pointer
    /// are locals exactly as they were; the frame is read once, at entry, and never looked at again
    /// unless the unit actually suspends. What an ordinary call now pays is that one test and the
    /// argument that carries it.
    /// </para>
    /// <para>
    /// <b>An abrupt resumption is raised at the top of the try, not at the suspension point.</b>
    /// <c>gen.throw</c> and <c>gen.return</c> re-enter at the instruction the frame suspended at
    /// and must be seen by whatever exception region encloses it - so the raise happens inside the
    /// same try the dispatch loop runs in, with <c>current</c> already set to that instruction. The
    /// existing region search then runs the same <c>catch</c> and <c>finally</c> blocks it would
    /// have run for a throw from the instruction itself, and no unwinding is reimplemented.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=764FB2
    // Broiler-Human:        PENDING
    private JsValue Execute(
        JsProgram program,
        int unitIndex,
        JsEnvironment? environment,
        JsValue thisValue,
        JsValue[] actualArguments,
        JsScriptFunction? self,
        JsValue newTarget,
        JsCell? thisBinding,
        JsFrame? frame)
    {
        var unit = program.Functions[unitIndex];
        var code = program.Code;
        var constants = program.Constants;
        var names = program.Names;
        var stack = frame is null ? new JsValue[unit.MaxOperandStack + 1] : frame.Stack;

        var scopes = frame is null
            ? new System.Collections.Generic.List<JsEnvironment>(4) { environment! }
            : frame.Scopes;

        var sp = frame is null ? 0 : frame.Sp;
        var pc = frame is null ? (int)unit.CodeOffset : frame.Pc;
        var strict = unit.IsStrict;
        var current = pc;
        JsRegion region = default;

        // THE FUNCTION `super` BELONGS TO IS NOT ALWAYS THE ONE RUNNING. An arrow has no `super`
        // of its own and reaches the enclosing method's, so both halves of `super` - the home
        // object a property starts from and the superclass a call constructs - are read from here
        // rather than from `self`.
        var active = self is null ? null : unit.IsArrow ? self.LexicalActiveFunction : self;

        // A DELEGATION RESUMES INSIDE ITS OWN OPCODE, whatever mode it resumes in: `return` and
        // `throw` arriving mid-`yield*` are forwarded to the inner iterator rather than raised
        // here, so only a plain `yield` reaches either of the two arms below.
        var abrupt = frame is { Started: true, Delegating: false } &&
            frame.ResumeMode != JsResumeMode.Next;

        // THE NORMAL RESUMPTION IS FINISHED HERE AND NOT IN THE OPCODE. The instruction that
        // suspended has already run its pop; what re-entry owes it is the push of the sent value
        // and the step past it, and doing that here keeps the `Yield` case a straight-line suspend.
        //
        // THE STEP IS THE WIDTH OF THE INSTRUCTION ACTUALLY AT THE POINTER, not of `Yield`. The two
        // suspensions this arm serves - `Yield` and `Await` - happen to be one byte each today, so
        // naming one of them worked; it would have gone on working right up until a suspension with
        // an operand was added, and then it would have resumed one byte into an instruction rather
        // than after it. Reading the byte costs nothing and cannot be wrong.
        if (frame is { Started: true, Delegating: false } && !abrupt)
        {
            stack[sp++] = frame.ResumeValue;
            pc += JsOpcodes.InstructionWidth((JsOpcode)code[pc]);
            current = pc;
        }

        while (true)
        {
            try
            {
                if (abrupt)
                {
                    abrupt = false;
                    current = pc;
                    var carried = frame!.ResumeValue;

                    if (frame.ResumeMode == JsResumeMode.Throw)
                    {
                        throw new JsThrow(carried, Render(carried));
                    }

                    throw new JsReturnSignal(carried);
                }

                while (true)
                {
                    current = pc;
                    Charge(FuelPerInstruction);
                    var opcode = (JsOpcode)code[pc];

                    switch (opcode)
                    {
                        case JsOpcode.Nop:
                            pc++;
                            break;

                        case JsOpcode.LoadUndefined:
                            stack[sp++] = JsValue.Undefined;
                            pc++;
                            break;

                        case JsOpcode.LoadNull:
                            stack[sp++] = JsValue.Null;
                            pc++;
                            break;

                        case JsOpcode.LoadTrue:
                            stack[sp++] = JsValue.True;
                            pc++;
                            break;

                        case JsOpcode.LoadFalse:
                            stack[sp++] = JsValue.False;
                            pc++;
                            break;

                        case JsOpcode.LoadConstant:
                            stack[sp++] = constants[U16(code, pc)];
                            pc += 3;
                            break;

                        case JsOpcode.LoadThis:
                            // A FRAME WITH A BINDING READS THE BINDING AND NOT THE VALUE IT WAS
                            // ENTERED WITH, because a derived constructor's `this` arrives part
                            // way through the frame and an arrow inside one has to see it when it
                            // does. Every other frame has no binding and reads what it was given.
                            stack[sp++] = thisBinding is null
                                ? thisValue
                                : ThisBinding(thisBinding);

                            pc++;
                            break;

                        case JsOpcode.NewArguments:
                            stack[sp++] = JsValue.Object(
                                Realm.CreateArguments(actualArguments, self, strict));

                            pc++;
                            break;

                        case JsOpcode.LoadNewTarget:

                            // `default` IS `undefined` HERE AND THAT IS DELIBERATE. An ordinary
                            // call passes nothing, and the value kind a `JsValue` defaults to is
                            // the uninitialised-binding marker, which no expression may produce -
                            // so the read normalises it rather than letting the marker escape onto
                            // the operand stack.
                            stack[sp++] = newTarget.IsEmpty ? JsValue.Undefined : newTarget;
                            pc++;
                            break;

                        case JsOpcode.LoadArgument:
                        {
                            var at = U16(code, pc);

                            stack[sp++] = at < actualArguments.Length
                                ? actualArguments[at]
                                : JsValue.Undefined;

                            pc += 3;
                            break;
                        }

                        case JsOpcode.RestArguments:
                        {
                            var from = U16(code, pc);
                            var rest = Realm.NewArray();

                            for (var at = from; at < actualArguments.Length; at++)
                            {
                                Charge(1);
                                rest.Push(actualArguments[at]);
                            }

                            stack[sp++] = JsValue.Object(rest);
                            pc += 3;
                            break;
                        }

                        case JsOpcode.LoadScoped:
                        {
                            var slot = Slot(scopes, code[pc + 1], U16(code, pc + 1), out var found);

                            if (!found)
                            {
                                throw new JsAbort(
                                    JsAbortKind.InternalDefect, "a scoped read named no slot");
                            }

                            if (slot.Slots[U16(code, pc + 1)].IsEmpty)
                            {
                                ThrowReferenceError("Cannot access a binding before initialisation");
                            }

                            stack[sp++] = slot.Slots[U16(code, pc + 1)];
                            pc += 4;
                            break;
                        }

                        case JsOpcode.StoreScoped:
                        {
                            var slot = Slot(scopes, code[pc + 1], U16(code, pc + 1), out var found);

                            if (!found)
                            {
                                throw new JsAbort(
                                    JsAbortKind.InternalDefect, "a scoped write named no slot");
                            }

                            var index = U16(code, pc + 1);

                            if (slot.Slots[index].IsEmpty)
                            {
                                ThrowReferenceError("Cannot access a binding before initialisation");
                            }

                            slot.Slots[index] = stack[--sp];
                            pc += 4;
                            break;
                        }

                        case JsOpcode.InitialiseScoped:
                        {
                            var slot = Slot(scopes, code[pc + 1], U16(code, pc + 1), out var found);

                            if (!found)
                            {
                                throw new JsAbort(
                                    JsAbortKind.InternalDefect, "a scoped initialiser named no slot");
                            }

                            slot.Slots[U16(code, pc + 1)] = stack[--sp];
                            pc += 4;
                            break;
                        }

                        case JsOpcode.LoadGlobal:
                        {
                            var name = names[U16(code, pc)];

                            if (!HasProperty(Realm.GlobalObject, name))
                            {
                                ThrowReferenceError(name + " is not defined");
                            }

                            stack[sp++] = GetProperty(JsValue.Object(Realm.GlobalObject), name);
                            pc += 3;
                            break;
                        }

                        case JsOpcode.LoadGlobalOrUndefined:
                        {
                            var name = names[U16(code, pc)];

                            stack[sp++] = HasProperty(Realm.GlobalObject, name)
                                ? GetProperty(JsValue.Object(Realm.GlobalObject), name)
                                : JsValue.Undefined;

                            pc += 3;
                            break;
                        }

                        case JsOpcode.StoreGlobal:
                        {
                            var target = names[U16(code, pc)];

                            // STRICT CODE MAY NOT CREATE A GLOBAL BY ASSIGNING TO A NAME NOBODY
                            // DECLARED, and that is the whole of what `"use strict"` buys a reader
                            // of an unfamiliar program. Sloppy code creates the property, which is
                            // what the arm below does; strict code gets the `ReferenceError` the
                            // language gives, because the alternative — a silent global — is the
                            // defect strict mode exists to make impossible.
                            if (strict && !HasProperty(Realm.GlobalObject, target))
                            {
                                sp--;
                                ThrowReferenceError(target + " is not defined");
                            }

                            SetProperty(
                                JsValue.Object(Realm.GlobalObject),
                                target,
                                stack[--sp],
                                strict);

                            pc += 3;
                            break;
                        }

                        case JsOpcode.DeclareGlobal:
                        {
                            var name = names[U16(code, pc)];

                            if (!Realm.GlobalObject.HasOwnProperty(name))
                            {
                                Realm.GlobalObject.SetOwnProperty(
                                    name,
                                    JsProperty.Data(
                                        JsValue.Undefined,
                                        JsPropertyAttributes.Writable | JsPropertyAttributes.Enumerable));
                            }

                            pc += 3;
                            break;
                        }

                        case JsOpcode.PushScope:
                            scopes.Add(new JsEnvironment(U16(code, pc), scopes[^1]));
                            pc += 3;
                            break;

                        case JsOpcode.PushObjectScope:
                            // The coercion is charged like the allocation it usually is: a String or
                            // a Number operand builds a wrapper object here, and `with (null)` is
                            // the TypeError `ToObject` already throws.
                            Charge(4);
                            scopes.Add(new JsEnvironment(ToObject(stack[--sp]), scopes[^1]));
                            pc++;
                            break;

                        case JsOpcode.ResolveName:
                            stack[sp++] = ResolveName(scopes, code[pc + 1], names[U16(code, pc + 1)]);
                            pc += 4;
                            break;

                        case JsOpcode.PopScope:
                            scopes.RemoveAt(scopes.Count - 1);
                            pc++;
                            break;

                        case JsOpcode.CopyScope:
                            scopes[^1] = scopes[^1].Copy(U16(code, pc));
                            pc += 3;
                            break;

                        case JsOpcode.NewObject:
                            stack[sp++] = JsValue.Object(new JsObject(Realm.ObjectPrototype));
                            pc++;
                            break;

                        case JsOpcode.NewArray:
                        {
                            var count = U16(code, pc);
                            var array = new JsArray(Realm.ArrayPrototype);

                            for (var at = 0; at < count; at++)
                            {
                                array.Push(stack[sp - count + at]);
                            }

                            sp -= count;
                            stack[sp++] = JsValue.Object(array);
                            pc += 3;
                            break;
                        }

                        case JsOpcode.GetProperty:
                        {
                            var target = stack[--sp];
                            stack[sp++] = GetProperty(target, names[U16(code, pc)]);
                            pc += 3;
                            break;
                        }

                        case JsOpcode.SetProperty:
                        {
                            var value = stack[--sp];
                            var target = stack[--sp];
                            SetProperty(target, names[U16(code, pc)], value, strict);
                            stack[sp++] = value;
                            pc += 3;
                            break;
                        }

                        case JsOpcode.GetIndex:
                        {
                            var key = stack[--sp];
                            var target = stack[--sp];
                            stack[sp++] = GetIndexed(target, key);
                            pc++;
                            break;
                        }

                        case JsOpcode.SetIndex:
                        {
                            var value = stack[--sp];
                            var key = stack[--sp];
                            var target = stack[--sp];
                            SetIndexed(target, key, value, strict);
                            stack[sp++] = value;
                            pc++;
                            break;
                        }

                        case JsOpcode.DefineField:
                            stack[sp - 2].AsObject().SetOwnProperty(
                                names[U16(code, pc)],
                                JsProperty.Data(stack[sp - 1], JsPropertyAttributes.Default));

                            sp--;
                            pc += 3;
                            break;

                        case JsOpcode.DefineIndexed:
                        {
                            var value = stack[--sp];
                            var key = stack[--sp];
                            DefineByKey(stack[sp - 1].AsObject(), key, value);
                            pc++;
                            break;
                        }

                        case JsOpcode.SetPrototypeLiteral:
                        {
                            var wanted = stack[--sp];

                            // A VALUE THAT IS NEITHER AN OBJECT NOR `null` IS IGNORED rather than
                            // refused: `{ __proto__: 5 }` is an ordinary object, and the member is
                            // dropped. No cycle is reachable here and no refusal is possible - the
                            // object is one this instruction sequence just built, so it is
                            // extensible and nothing else holds a reference through which it could
                            // appear in the chain being installed.
                            if (wanted.IsObject)
                            {
                                stack[sp - 1].AsObject().Prototype = wanted.AsObject();
                            }
                            else if (wanted.Type == JsType.Null)
                            {
                                stack[sp - 1].AsObject().Prototype = null;
                            }

                            pc++;
                            break;
                        }

                        case JsOpcode.DefineGetter:
                        case JsOpcode.DefineSetter:
                        {
                            var accessor = stack[--sp].AsObject();
                            var host = stack[sp - 1].AsObject();
                            var key = names[U16(code, pc)];
                            host.TryGetOwnProperty(key, out var existing);

                            host.SetOwnProperty(
                                key,
                                JsProperty.Accessor(
                                    opcode == JsOpcode.DefineGetter ? accessor : existing.Getter,
                                    opcode == JsOpcode.DefineSetter ? accessor : existing.Setter,
                                    JsPropertyAttributes.Enumerable | JsPropertyAttributes.Configurable));

                            pc += 3;
                            break;
                        }

                        case JsOpcode.ArrayAppend:
                        {
                            var element = stack[--sp];
                            var array = (JsArray)stack[sp - 1].AsObject();
                            Charge(1);
                            array.SetIndex(array.Length, element);
                            pc++;
                            break;
                        }

                        case JsOpcode.ArrayHoles:
                        {
                            var array = (JsArray)stack[sp - 1].AsObject();
                            array.SetLength(array.Length + U16(code, pc));
                            pc += 3;
                            break;
                        }

                        case JsOpcode.SpreadArray:
                        {
                            var source = stack[--sp];
                            var array = (JsArray)stack[sp - 1].AsObject();
                            var values = new System.Collections.Generic.List<JsValue>();
                            IterateInto(source, values);

                            foreach (var element in values)
                            {
                                Charge(1);
                                array.SetIndex(array.Length, element);
                            }

                            pc++;
                            break;
                        }

                        case JsOpcode.SpreadObject:
                        {
                            var source = stack[--sp];
                            CopyDataProperties(stack[sp - 1].AsObject(), source);
                            pc++;
                            break;
                        }

                        case JsOpcode.DeleteProperty:
                        {
                            var target = stack[--sp];
                            var key = names[U16(code, pc)];
                            var went = !target.IsObject || target.AsObject().DeleteOwnProperty(key);

                            // A REFUSED DELETE ANSWERS `false` IN SLOPPY CODE AND THROWS IN STRICT
                            // CODE, and the pair is the same rule the assignment above follows: an
                            // operation the object refused is reported as a value where a program
                            // may not have asked, and as an exception where it said it wanted to
                            // know. A `false` nobody reads is how a frozen property gets treated as
                            // deleted by the code after it.
                            if (!went && strict)
                            {
                                ThrowTypeError("Cannot delete property '" + key + "'");
                            }

                            stack[sp++] = JsValue.Boolean(went);
                            pc += 3;
                            break;
                        }

                        // A COMPUTED DELETE OBEYS THE SAME STRICT RULE THE STATIC ONE DOES, and it
                        // did not: `delete o.frozen` threw in strict code and `delete o["frozen"]`
                        // answered `false` and went on. The two are one operator written two ways,
                        // so a program that reached a refused delete through a computed key was
                        // told nothing and carried on as though the property were gone.
                        case JsOpcode.DeleteIndex:
                        {
                            var key = stack[--sp];
                            var target = stack[--sp];

                            var removed = !target.IsObject ||
                                (key.IsSymbol
                                    ? target.AsObject().DeleteOwnSymbol(key.AsSymbol())
                                    : target.AsObject().DeleteOwnProperty(ToPropertyKey(key)));

                            if (!removed && strict)
                            {
                                ThrowTypeError(
                                    "Cannot delete property '" +
                                    (key.IsSymbol
                                        ? "Symbol(" + key.AsSymbol().Description + ")"
                                        : ToPropertyKey(key)) +
                                    "'");
                            }

                            stack[sp++] = JsValue.Boolean(removed);
                            pc++;
                            break;
                        }

                        case JsOpcode.Closure:
                            stack[sp++] = JsValue.Object(
                                Realm.CreateClosure(
                                    program,
                                    U16(code, pc),
                                    scopes[^1],
                                    thisValue,
                                    thisBinding,
                                    newTarget,
                                    active));

                            pc += 3;
                            break;

                        // A COMPUTED KEY MAY BE A SYMBOL, and this is the instruction most likely
                        // to meet one: `class C { [Symbol.iterator]() {} }` is how a guest joins
                        // the iteration protocol, and converting the key to a String first threw
                        // the TypeError that conversion owes rather than defining the member.
                        case JsOpcode.DefineMethod:
                        {
                            var member = stack[--sp];
                            var key = stack[--sp];
                            var host = stack[sp - 1].AsObject();

                            if (key.IsSymbol)
                            {
                                DefineSymbolMember(host, key.AsSymbol(), member, code[pc + 1]);
                            }
                            else
                            {
                                DefineMember(host, ToPropertyKey(key), member, code[pc + 1]);
                            }

                            pc += 2;
                            break;
                        }

                        // THE ORDER OF THE THREE STEPS IS OBSERVABLE AND IS THE SPECIFICATION'S:
                        // the this binding is read, then the base is taken from the home object,
                        // and only then is the key converted. A key whose `toString` re-points the
                        // home object's prototype still reads through the prototype the reference
                        // was made against, and a derived constructor that has not called
                        // `super()` fails before the conversion runs at all.
                        case JsOpcode.LoadSuperProperty:
                        {
                            var receiver = thisBinding is null
                                ? thisValue
                                : ThisBinding(thisBinding);

                            var start = SuperBase(active);
                            var key = ToPropertyKey(stack[--sp]);
                            stack[sp++] = Lookup(start, key, receiver);
                            pc++;
                            break;
                        }

                        case JsOpcode.StoreSuperProperty:
                        {
                            var value = stack[--sp];

                            var receiver = thisBinding is null
                                ? thisValue
                                : ThisBinding(thisBinding);

                            var start = SuperBase(active);
                            var key = ToPropertyKey(stack[--sp]);
                            SetSuper(start, receiver, key, value, strict);
                            stack[sp++] = value;
                            pc++;
                            break;
                        }

                        case JsOpcode.NewClass:
                        {
                            var derived = (code[pc + 1] & JsOpcodes.ClassIsDerived) != 0;
                            var constructor = stack[--sp];
                            var heritage = derived ? stack[--sp] : JsValue.Undefined;
                            stack[sp++] = BuildClass(constructor, derived, heritage);
                            pc += 2;
                            break;
                        }

                        // THE PAIR UNDER THE KEY IS READ AND NOT POPPED, which is what lets a whole
                        // class body run over the one constructor-and-prototype pair the lowering
                        // loaded once. Which of the two is the home object is the static bit's
                        // answer and nothing else's.
                        case JsOpcode.DefineClassElement:
                        {
                            var elementFlags = code[pc + 1];
                            var body = stack[--sp];
                            var key = stack[--sp];

                            RecordClassElement(
                                (JsScriptFunction)stack[sp - 2].AsObject(),
                                stack[sp - 1].AsObject(),
                                key,
                                body,
                                elementFlags);

                            pc += 2;
                            break;
                        }

                        case JsOpcode.RunStaticElements:
                            RunStaticElements((JsScriptFunction)stack[sp - 1].AsObject());
                            pc++;
                            break;

                        case JsOpcode.NewPrivateName:
                            Charge(2);

                            stack[sp++] = JsValue.Symbol(
                                new JsSymbol(program.Constants[U16(code, pc)].AsString(), described: true)
                                {
                                    IsPrivateName = true,
                                });

                            pc += 3;
                            break;

                        case JsOpcode.LoadPrivate:
                        {
                            var name = stack[--sp].AsSymbol();
                            var host = stack[--sp];
                            stack[sp++] = ReadPrivate(host, name);
                            pc++;
                            break;
                        }

                        case JsOpcode.StorePrivate:
                        {
                            var written = stack[--sp];
                            var name = stack[--sp].AsSymbol();
                            var host = stack[--sp];
                            WritePrivate(host, name, written);
                            stack[sp++] = written;
                            pc++;
                            break;
                        }

                        // A NON-OBJECT IS A TypeError AND NOT A `false`. The form exists to ask a
                        // question `o.#x` would have thrown for, so answering `false` for a number
                        // reads like the right answer - and it is the ORDINARY `in` operator's
                        // answer that this follows instead: `"x" in 5` throws, and the private form
                        // is the same operator with a name the grammar spells differently. The
                        // opposite reading was written first and the comparison engine refused it.
                        case JsOpcode.HasPrivate:
                        {
                            var name = stack[--sp].AsSymbol();
                            var host = stack[--sp];

                            if (!host.IsObject)
                            {
                                ThrowTypeError(
                                    "Cannot use 'in' operator to search for '" + name.Description +
                                    "' in " + Describe(host));
                            }

                            stack[sp++] = JsValue.Boolean(host.AsObject().HasPrivate(name));
                            pc++;
                            break;
                        }

                        case JsOpcode.SuperCall:
                        {
                            var argc = code[pc + 1];
                            var passed = argc == 0 ? System.Array.Empty<JsValue>() : new JsValue[argc];

                            for (var at = argc - 1; at >= 0; at--)
                            {
                                passed[at] = stack[--sp];
                            }

                            stack[sp++] = SuperConstruct(active, thisBinding, newTarget, passed);
                            pc += 2;
                            break;
                        }

                        // THE ARGUMENT ARRAY CARRIES THE COUNT, and everything else this needs -
                        // the superclass, the `new.target`, the `this` slot it binds - comes from
                        // the frame exactly as SuperCall's does.
                        case JsOpcode.SuperCallSpread:
                            stack[sp - 1] = SuperConstruct(
                                active, thisBinding, newTarget, ArgumentsOf(stack[sp - 1]));

                            pc++;
                            break;

                        case JsOpcode.SuperCallForwarded:
                            stack[sp++] = SuperConstruct(
                                active, thisBinding, newTarget, actualArguments);

                            pc++;
                            break;

                        case JsOpcode.Call:
                        {
                            var argc = code[pc + 1];
                            var arguments = argc == 0 ? System.Array.Empty<JsValue>() : new JsValue[argc];

                            for (var at = argc - 1; at >= 0; at--)
                            {
                                arguments[at] = stack[--sp];
                            }

                            var receiver = stack[--sp];
                            var callee = stack[--sp];
                            stack[sp++] = Call(callee, receiver, arguments);
                            pc += 2;
                            break;
                        }

                        case JsOpcode.CallEval:
                        {
                            var argc = code[pc + 1];
                            var arguments = argc == 0 ? System.Array.Empty<JsValue>() : new JsValue[argc];

                            for (var at = argc - 1; at >= 0; at--)
                            {
                                arguments[at] = stack[--sp];
                            }

                            var receiver = stack[--sp];
                            var callee = stack[--sp];

                            // THE SPELLING SAYS DIRECT; THE VALUE DECIDES WHETHER IT IS. A program
                            // may assign to the global `eval`, and a call to whatever it now holds
                            // is an ordinary call however it is written.
                            stack[sp++] = Realm.IsEvalIntrinsic(callee)
                                ? Evaluate(arguments, direct: true, unit.Flags)
                                : Call(callee, receiver, arguments);

                            pc += 2;
                            break;
                        }

                        case JsOpcode.Construct:
                        {
                            var argc = code[pc + 1];
                            var arguments = argc == 0 ? System.Array.Empty<JsValue>() : new JsValue[argc];

                            for (var at = argc - 1; at >= 0; at--)
                            {
                                arguments[at] = stack[--sp];
                            }

                            var callee = stack[--sp];
                            stack[sp++] = Construct(callee, arguments);
                            pc += 2;
                            break;
                        }

                        case JsOpcode.CallSpread:
                        {
                            var spread = ArgumentsOf(stack[--sp]);
                            var receiver = stack[--sp];
                            var callee = stack[--sp];
                            stack[sp++] = Call(callee, receiver, spread);
                            pc++;
                            break;
                        }

                        case JsOpcode.ConstructSpread:
                        {
                            var spread = ArgumentsOf(stack[--sp]);
                            var callee = stack[--sp];
                            stack[sp++] = Construct(callee, spread);
                            pc++;
                            break;
                        }

                        case JsOpcode.Return:
                            return stack[--sp];

                        case JsOpcode.ReturnUndefined:
                            return JsValue.Undefined;

                        case JsOpcode.Add:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = Add(left, right);
                            pc++;
                            break;
                        }

                        case JsOpcode.Subtract:
                            Binary(stack, ref sp, static (a, b) => a - b, this);
                            pc++;
                            break;

                        case JsOpcode.Multiply:
                            Binary(stack, ref sp, static (a, b) => a * b, this);
                            pc++;
                            break;

                        case JsOpcode.Divide:
                            Binary(stack, ref sp, static (a, b) => a / b, this);
                            pc++;
                            break;

                        case JsOpcode.Remainder:
                            Binary(stack, ref sp, static (a, b) => a % b, this);
                            pc++;
                            break;

                        case JsOpcode.Exponent:
                            Binary(stack, ref sp, static (a, b) => System.Math.Pow(a, b), this);
                            pc++;
                            break;

                        case JsOpcode.Negate:
                            stack[sp - 1] = JsValue.Number(-ToNumber(stack[sp - 1]));
                            pc++;
                            break;

                        case JsOpcode.ToNumber:
                            stack[sp - 1] = JsValue.Number(ToNumber(stack[sp - 1]));
                            pc++;
                            break;

                        case JsOpcode.Not:
                            stack[sp - 1] = JsValue.Boolean(!stack[sp - 1].ToBooleanValue());
                            pc++;
                            break;

                        case JsOpcode.BitwiseNot:
                            stack[sp - 1] = JsValue.Number(~ToInt32(stack[sp - 1]));
                            pc++;
                            break;

                        case JsOpcode.LessThan:
                        case JsOpcode.LessThanOrEqual:
                        case JsOpcode.GreaterThan:
                        case JsOpcode.GreaterThanOrEqual:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(Relational(opcode, left, right));
                            pc++;
                            break;
                        }

                        case JsOpcode.StrictEquals:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(left.StrictlyEquals(right));
                            pc++;
                            break;
                        }

                        case JsOpcode.StrictNotEquals:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(!left.StrictlyEquals(right));
                            pc++;
                            break;
                        }

                        case JsOpcode.LooseEquals:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(LooselyEquals(left, right));
                            pc++;
                            break;
                        }

                        case JsOpcode.LooseNotEquals:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(!LooselyEquals(left, right));
                            pc++;
                            break;
                        }

                        case JsOpcode.BitwiseOr:
                        {
                            var right = ToInt32(stack[--sp]);
                            var left = ToInt32(stack[--sp]);
                            stack[sp++] = JsValue.Number(left | right);
                            pc++;
                            break;
                        }

                        case JsOpcode.BitwiseAnd:
                        {
                            var right = ToInt32(stack[--sp]);
                            var left = ToInt32(stack[--sp]);
                            stack[sp++] = JsValue.Number(left & right);
                            pc++;
                            break;
                        }

                        case JsOpcode.BitwiseXor:
                        {
                            var right = ToInt32(stack[--sp]);
                            var left = ToInt32(stack[--sp]);
                            stack[sp++] = JsValue.Number(left ^ right);
                            pc++;
                            break;
                        }

                        case JsOpcode.ShiftLeft:
                        {
                            var right = ToUint32(stack[--sp]) & 31;
                            var left = ToInt32(stack[--sp]);
                            stack[sp++] = JsValue.Number(left << (int)right);
                            pc++;
                            break;
                        }

                        case JsOpcode.ShiftRight:
                        {
                            var right = ToUint32(stack[--sp]) & 31;
                            var left = ToInt32(stack[--sp]);
                            stack[sp++] = JsValue.Number(left >> (int)right);
                            pc++;
                            break;
                        }

                        case JsOpcode.ShiftRightUnsigned:
                        {
                            var right = ToUint32(stack[--sp]) & 31;
                            var left = ToUint32(stack[--sp]);
                            stack[sp++] = JsValue.Number(left >> (int)right);
                            pc++;
                            break;
                        }

                        case JsOpcode.TypeOf:
                            stack[sp - 1] = JsValue.String(stack[sp - 1].TypeOf());
                            pc++;
                            break;

                        case JsOpcode.InstanceOf:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(InstanceOf(left, right));
                            pc++;
                            break;
                        }

                        case JsOpcode.In:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];

                            if (!right.IsObject)
                            {
                                ThrowTypeError("Cannot use 'in' operator to search for a key");
                            }

                            stack[sp++] = JsValue.Boolean(
                                left.IsSymbol
                                    ? HasSymbol(right.AsObject(), left.AsSymbol())
                                    : HasProperty(right.AsObject(), ToPropertyKey(left)));

                            pc++;
                            break;
                        }

                        case JsOpcode.Void:
                            stack[sp - 1] = JsValue.Undefined;
                            pc++;
                            break;

                        case JsOpcode.RequireCoercible:
                        {
                            var subject = stack[sp - 1];

                            if (subject.IsNullish)
                            {
                                var named = names[U16(code, pc)];

                                ThrowTypeError(
                                    named.Length == 0
                                        ? "Cannot destructure " + Describe(subject)
                                        : "Cannot destructure property '" + named + "' of " +
                                            Describe(subject));
                            }

                            pc += 3;
                            break;
                        }

                        case JsOpcode.Jump:
                            pc = (int)U32(code, pc);
                            break;

                        case JsOpcode.JumpIfFalse:
                            pc = !stack[--sp].ToBooleanValue() ? (int)U32(code, pc) : pc + 5;
                            break;

                        case JsOpcode.JumpIfTrue:
                            pc = stack[--sp].ToBooleanValue() ? (int)U32(code, pc) : pc + 5;
                            break;

                        case JsOpcode.Throw:
                        {
                            var thrown = stack[--sp];

                            // THE ONE VALUE THIS INSTRUCTION DOES NOT THROW. A `finally` that a
                            // forced return passed through re-raises what it parked, using this
                            // instruction because the lowering has no other; a forced return
                            // parked here comes back out as a forced return, so an outer `catch`
                            // still never sees it.
                            if (thrown.AsObjectOrNull() is JsForcedReturn forced)
                            {
                                throw new JsReturnSignal(forced.Value);
                            }

                            throw new JsThrow(thrown, Render(thrown));
                        }

                        case JsOpcode.ForInStart:
                        {
                            var target = stack[--sp];
                            stack[sp++] = JsValue.Object(Realm.CreateEnumerator(this, target));
                            pc++;
                            break;
                        }

                        case JsOpcode.ForInNext:
                        {
                            var enumerator = (JsEnumerator)stack[--sp].AsObject();

                            if (enumerator.TryNext(out var key))
                            {
                                stack[sp++] = JsValue.String(key);
                                pc += 5;
                            }
                            else
                            {
                                pc = (int)U32(code, pc);
                            }

                            break;
                        }

                        case JsOpcode.IterateStart:
                            stack[sp - 1] = JsValue.Object(GetIterator(stack[sp - 1]));
                            pc++;
                            break;

                        case JsOpcode.IterateNext:
                        {
                            var record = (JsIteratorRecord)stack[--sp].AsObject();

                            if (TryIterateNext(record, out var element))
                            {
                                stack[sp++] = element;
                                pc += 5;
                            }
                            else
                            {
                                pc = (int)U32(code, pc);
                            }

                            break;
                        }

                        case JsOpcode.IterateRest:
                            stack[sp - 1] = JsValue.Object(
                                DrainIterator((JsIteratorRecord)stack[sp - 1].AsObject()));

                            pc++;
                            break;

                        case JsOpcode.IterateClose:
                        {
                            var record = (JsIteratorRecord)stack[--sp].AsObject();

                            if (code[pc + 1] == 0)
                            {
                                CloseIterator(record);
                            }
                            else
                            {
                                CloseIteratorQuietly(record);
                            }

                            pc += 2;
                            break;
                        }

                        case JsOpcode.Yield:
                        {
                            // THE WHOLE OF SUSPENDING. The yielded value leaves on the return, the
                            // height and the pointer stay behind in the frame, and the pointer is
                            // left AT this instruction rather than after it - so a resumption that
                            // arrives abruptly raises its throw or its return at a point the
                            // enclosing exception regions actually cover. Nothing clears the
                            // delegation flag here because nothing can have set it: a suspended
                            // `yield*` always resumes at its own instruction, never at this one.
                            var yielded = stack[--sp];
                            frame!.Sp = sp;
                            frame.Pc = pc;
                            frame.Suspended = true;

                            // THE KIND IS RECORDED FOR THE ONE DRIVER THAT HAS TO ASK. A generator
                            // and an async function each suspend one way, so neither reads this;
                            // an async generator's body suspends both ways into one frame, and the
                            // two mean opposite things to whoever receives the value.
                            frame.Suspension = JsSuspension.Yield;
                            return yielded;
                        }

                        // AN IMPORT READ GOES TO THE EXPORTING ENVIRONMENT EVERY TIME. Nothing is
                        // cached here and nothing may be: a live binding is one whose later value
                        // is seen, so the only correct read is the one that happens now.
                        case JsOpcode.LoadImport:
                            stack[sp++] = JsModuleNamespace.Read(
                                program.ImportBindings[U16(code, pc)], modules!, this);

                            pc += 3;
                            break;

                        case JsOpcode.ThrowImmutable:
                            ThrowTypeError(
                                "Assignment to constant variable '" + names[U16(code, pc)] + "'");

                            break;

                        case JsOpcode.Await:
                        {
                            // THE SAME SUSPEND AS `Yield`, AND THE VALUE MEANS SOMETHING ELSE ON
                            // THE WAY OUT. What leaves on the return is what is being awaited
                            // rather than what is being yielded, and the driver that receives it
                            // resolves it and registers this frame's continuation. The pointer is
                            // left AT this instruction for the same reason: a rejected await
                            // re-enters abruptly and has to raise where the body's own exception
                            // regions cover it.
                            var awaited = stack[--sp];
                            frame!.Sp = sp;
                            frame.Pc = pc;
                            frame.Suspended = true;
                            frame.Suspension = JsSuspension.Await;
                            return awaited;
                        }

                        case JsOpcode.YieldDelegate:
                        {
                            // TWO DELEGATION LOOPS AND ONE INSTRUCTION, chosen by the unit's own
                            // flag rather than by anything on the stack. A synchronous delegation
                            // runs between two yields inside one entry into this loop; an
                            // asynchronous one leaves after every inner call and comes back at this
                            // same instruction, so it has five re-entry points where the
                            // synchronous one has one. Which of the two a body gets is fixed when
                            // it is verified and cannot change at run time.
                            var step = unit.IsAsync
                                ? DelegateAsync(frame!, stack, ref sp, pc)
                                : Delegate(frame!, stack, ref sp, pc);

                            if (frame!.Suspended)
                            {
                                return step;
                            }

                            stack[sp++] = step;
                            pc++;
                            break;
                        }

                        case JsOpcode.IterateStartAsync:
                            stack[sp - 1] = JsValue.Object(Realm.GetAsyncIterator(stack[sp - 1]));
                            pc++;
                            break;

                        case JsOpcode.IterateNextAsync:
                        {
                            // WHAT IS PUSHED IS UNAWAITED AND IS NOT A STEP. An async iterator's
                            // `next` answers a promise, and the instruction after this one is the
                            // `Await` that resolves it - so nothing here reads `done` or `value`,
                            // because neither exists yet.
                            var record = (JsIteratorRecord)stack[sp - 1].AsObject();
                            Charge(2);

                            // THE RECORD IS MARKED DONE FOR THE LENGTH OF THE STEP, and unmarked by
                            // `IterateAwaitStep` when the step turns out to have a value. A head
                            // step that fails - the call throwing, the promise rejecting, the
                            // answer not being an object - owes the iterator NO `return`: the
                            // specification propagates all of those with `?` and closes only for an
                            // abrupt binding or body. The loop's handler closes unconditionally, so
                            // the flag is what tells it which of the two happened, exactly as the
                            // synchronous `TryIterateNext` uses it.
                            record.Done = true;
                            stack[sp++] = Call(record.Next, record.Iterator, []);
                            pc++;
                            break;
                        }

                        case JsOpcode.IterateAwaitStep:
                        {
                            var step = stack[--sp];
                            var record = (JsIteratorRecord)stack[--sp].AsObject();

                            if (!step.IsObject)
                            {
                                ThrowTypeError(
                                    "Iterator result " + Describe(step) + " is not an object");
                            }

                            if (GetProperty(step, "done").ToBooleanValue())
                            {
                                pc = (int)U32(code, pc);
                                break;
                            }

                            // THE RECORD COMES BACK OFF DONE HERE, and only here. Everything from
                            // the call of `next` to this point is a head step the specification
                            // propagates without closing; from this point to the next call it is
                            // the loop body, whose abrupt exits owe the iterator its `return`.
                            record.Done = false;
                            stack[sp++] = GetProperty(step, "value");
                            pc += 5;
                            break;
                        }

                        case JsOpcode.IterateCloseAsync:
                        {
                            var record = (JsIteratorRecord)stack[--sp].AsObject();
                            var method = record.Done
                                ? JsValue.Undefined
                                : GetProperty(record.Iterator, "return");

                            // AN ITERATOR THAT IS DONE OR HAS NO `return` IS NOT AWAITED AT ALL,
                            // and the branch is what keeps that promise-free. `AsyncIteratorClose`
                            // returns its completion the moment it finds no `return` to call, so a
                            // lowering that awaited unconditionally would have spent a turn of the
                            // job queue on every `break` out of a loop over an Array.
                            if (method.IsNullish)
                            {
                                record.Done = true;
                                pc = (int)U32(code, pc);
                                break;
                            }

                            if (!method.IsObject || !method.AsObject().IsCallable)
                            {
                                ThrowTypeError("The iterator's return is not a function");
                            }

                            record.Done = true;
                            stack[sp++] = Call(method, record.Iterator, []);
                            pc += 5;
                            break;
                        }

                        case JsOpcode.IterateCloseCheck:
                        {
                            var answered = stack[--sp];

                            if (!answered.IsObject)
                            {
                                ThrowTypeError(
                                    "The iterator's return answered " + Describe(answered) +
                                    " and not an object");
                            }

                            pc++;
                            break;
                        }

                        case JsOpcode.Pop:
                            sp--;
                            pc++;
                            break;

                        case JsOpcode.Duplicate:
                            stack[sp] = stack[sp - 1];
                            sp++;
                            pc++;
                            break;

                        case JsOpcode.DuplicateTwo:
                            stack[sp] = stack[sp - 2];
                            stack[sp + 1] = stack[sp - 1];
                            sp += 2;
                            pc++;
                            break;

                        case JsOpcode.Swap:
                        {
                            (stack[sp - 1], stack[sp - 2]) = (stack[sp - 2], stack[sp - 1]);
                            pc++;
                            break;
                        }

                        case JsOpcode.Pick:
                            stack[sp] = stack[sp - 1 - code[pc + 1]];
                            sp++;
                            pc += 2;
                            break;

                        default:
                            throw new JsAbort(
                                JsAbortKind.InternalDefect, "a verified opcode had no case here");
                    }
                }
            }
            // A FILTER AND NOT A CATCH-AND-RETHROW, and the difference is a process termination.
            //
            // A frame with a `catch` that rethrows is entered during the SECOND pass: the runtime
            // runs the handler as a funclet above the current stack, and the rethrow starts a fresh
            // dispatch from there. A throw crossing a thousand of these accumulated a thousand
            // funclets and their dispatchers, and the process died - on a stack that holds eight
            // thousand ordinary calls. A guest `throw` from any depth past about five hundred was
            // fatal, whether or not the guest had a `catch` waiting for it.
            //
            // A FILTER runs in the FIRST pass, without unwinding and without a funclet per frame:
            // a frame with no region for this instruction answers false and is passed over, and
            // exactly one dispatch reaches the frame that has one. `TryFindHandler` is a pure
            // search, which is what a filter has to be.
            catch (JsThrow thrown) when (TryFindHandler(program, unitIndex, current, out region))
            {
                while (scopes.Count > region.ScopeDepth + 1)
                {
                    scopes.RemoveAt(scopes.Count - 1);
                }

                sp = (int)region.StackHeight;
                stack[sp++] = thrown.Value;
                pc = (int)region.Handler;
            }
            catch (JsReturnSignal forced)
            {
                // A FORCED RETURN RUNS EVERY `finally` AND NO `catch`. The search skips catch
                // regions rather than taking the innermost region of either kind, which is the one
                // place the two completions differ; taking the innermost would let
                // `try { yield } catch (e) {}` swallow a `gen.return()` as though somebody had
                // thrown, and the generator would carry on running instead of ending.
                if (!TryFindFinally(program, unitIndex, current, out var finaliser))
                {
                    throw;
                }

                while (scopes.Count > finaliser.ScopeDepth + 1)
                {
                    scopes.RemoveAt(scopes.Count - 1);
                }

                sp = (int)finaliser.StackHeight;
                stack[sp++] = JsValue.Object(new JsForcedReturn(forced.Value));
                pc = (int)finaliser.Handler;
            }
        }
    }

    /// <summary>
    /// The innermost <c>finally</c> region of <paramref name="unit"/> covering <paramref name="pc"/>.
    /// </summary>
    /// <remarks>
    /// The regions of one unit are recorded innermost-first by the lowering, so the first match in
    /// order is the innermost - the same property <see cref="TryFindHandler"/> relies on. What
    /// differs is only that a <c>catch</c> region is passed over rather than taken.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C8F9E6
    // Broiler-Human:        PENDING
    private static bool TryFindFinally(JsProgram program, int unit, int pc, out JsRegion region)
    {
        foreach (var candidate in program.Regions)
        {
            if (candidate.Unit == (uint)unit &&
                candidate.Kind == JsFormat.HandlerKind.Finally &&
                pc >= candidate.TryStart && pc < candidate.TryEnd)
            {
                region = candidate;
                return true;
            }
        }

        region = default;
        return false;
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=E53DC7
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
                // THE ONE PATH THAT IS THE ORDINARY PROTOCOL, so it is the ordinary helper: the
                // record's `next` is the function read once at acquisition, the sent value is
                // forwarded as its argument, and the inner iterator's own COMPLETION VALUE is what
                // `yield*` evaluates to - the half of delegation a loop written by hand forgets.
                if (!TryIterateNext(record, [sent], out var element, out var completed, wantsCompleted: true))
                {
                    frame.Delegate = null;
                    return completed;
                }

                frame.Delegating = true;
                frame.Sp = sp;
                frame.Pc = pc;
                frame.Suspended = true;
                return element;
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

        frame.Delegating = true;
        frame.Sp = sp;
        frame.Pc = pc;
        frame.Suspended = true;
        frame.Suspension = JsSuspension.Yield;
        return GetProperty(step, "value");
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

                    return SuspendDelegation(
                        frame,
                        Call(returner, record.Iterator, [carried]),
                        sp,
                        pc,
                        DelegateAwaitingReturn);
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

        return SuspendDelegation(
            frame, GetProperty(step, "value"), sp, pc, DelegateStepFresh);
    }

    /// <summary>Suspends the frame inside a delegation, at the stage it will re-enter at.</summary>
    /// <remarks>
    /// Stage zero is the yield of an inner value and every other stage is an await, which is the one
    /// place the <see cref="JsSuspension"/> the driver reads is decided for a delegation.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D2339C
    // Broiler-Human:        PENDING
    private static JsValue SuspendDelegation(
        JsFrame frame, JsValue value, int sp, int pc, int stage)
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

    /// <summary>The delegation is at its own yield, and a resumption is a request to forward.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=6D9F45
    // Broiler-Human:        PENDING
    private const int DelegateStepFresh = 0;

    /// <summary>Awaiting what the inner <c>next</c> or <c>throw</c> answered.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=6DFAAB
    // Broiler-Human:        PENDING
    private const int DelegateAwaitingStep = 1;

    /// <summary>Awaiting what the inner <c>return</c> answered.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EBD61E
    // Broiler-Human:        PENDING
    private const int DelegateAwaitingReturn = 2;

    /// <summary>Awaiting the close an inner iterator with no <c>throw</c> is given first.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3D1C9B
    // Broiler-Human:        PENDING
    private const int DelegateAwaitingClose = 3;

    /// <summary>Awaiting the resumption's own value, when the inner iterator has no <c>return</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3D5FDD
    // Broiler-Human:        PENDING
    private const int DelegateAwaitingReceived = 4;

    /// <summary>Unpacks the argument Array a spread call built into the array a call takes.</summary>
    /// <remarks>
    /// The Array was built by this lowering and nothing else can reach it, so the dense elements
    /// are read directly. It is the one place in the iteration work where that is honest: the
    /// protocol already ran, when the spread appended.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=73F0A8
    // Broiler-Human:        PENDING
    private JsValue[] ArgumentsOf(JsValue packed)
    {
        var array = (JsArray)packed.AsObject();
        var count = (int)array.Length;

        if (count == 0)
        {
            return System.Array.Empty<JsValue>();
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EAA946
    // Broiler-Human:        PENDING
    private static bool TryFindHandler(JsProgram program, int unit, int pc, out JsRegion region)
    {
        foreach (var candidate in program.Regions)
        {
            if (candidate.Unit == (uint)unit && pc >= candidate.TryStart && pc < candidate.TryEnd)
            {
                region = candidate;
                return true;
            }
        }

        region = default;
        return false;
    }

    /// <summary>
    /// The object environment record that binds <paramref name="name"/> within the innermost
    /// <paramref name="limit"/> records, or <c>undefined</c> when none of them does.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THIS IS THE ONLY LOOKUP BY NAME IN THE SCOPE CHAIN, AND IT CAN REACH NOTHING BUT AN
    /// OBJECT A <c>with</c> PUT THERE.</b> A declarative record is a <c>JsValue</c> array with no
    /// names in it, so this walk has nothing to compare a name against and skips it; a name the
    /// objects do not have therefore falls through to the <c>(depth, slot)</c> address the lowering
    /// computed from the language's own scope rules, and to nothing else. That is what stops a
    /// <c>with</c> body reaching an enclosing function's binding by naming it.
    /// </para>
    /// <para>
    /// <b><paramref name="limit"/> is the lowering's, and it is why an outer <c>with</c> cannot
    /// shadow an inner declaration.</b> It counts the records between the reference and the binding
    /// the lowering resolved, so a record past that binding is never asked. Walking the whole chain
    /// would make <c>with (a) { function f() { var x; with (b) { x } } }</c> read <c>a.x</c>.
    /// </para>
    /// <para>
    /// <b>Nothing here is cached, and that is the behaviour rather than a missing optimisation.</b>
    /// Each mention of a name inside a <c>with</c> body runs this walk again, so an object that
    /// gains the property between two reads is read from on the second and not on the first, and an
    /// object that loses it goes back to the enclosing binding. An implementation that remembered
    /// where a name resolved last time would answer the first reading for both.
    /// </para>
    /// <para>
    /// <b><c>Symbol.unscopables</c> is consulted per record and per name</b>, which is where the
    /// specification consults it - inside <c>HasBinding</c> - and is why a name listed truthily
    /// there does not stop the walk. It is a property read on the guest's object and can therefore
    /// run a getter, which is charged like any other call this walk makes.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=1AC0F4
    // Broiler-Falsified-If: this walk answers with anything but an object a `PushObjectScope` placed on the chain
    // Broiler-Human:        PENDING
    private JsValue ResolveName(
        System.Collections.Generic.List<JsEnvironment> scopes, int limit, string name)
    {
        var current = scopes[^1];

        for (var step = 0; step < limit && current is not null; step++)
        {
            Charge(1);

            if (current.Binding is { } bound && HasProperty(bound, name) && !Unscopable(bound, name))
            {
                return JsValue.Object(bound);
            }

            current = current.Parent;
        }

        return JsValue.Undefined;
    }

    /// <summary>Whether <paramref name="bound"/> hides <paramref name="name"/> from a <c>with</c>.</summary>
    /// <remarks>
    /// <b>The blocklist is read off the object every time, prototype chain included.</b> It is what
    /// lets an Array put <c>values</c>, <c>keys</c> and <c>flat</c> on
    /// <c>Array.prototype[Symbol.unscopables]</c> so that <c>with (someArray) { values }</c> reaches
    /// an outer <c>values</c> rather than the method - which is the whole reason the Symbol was
    /// added to the language, and is a compatibility rule rather than a nicety.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=47A445
    // Broiler-Human:        PENDING
    private bool Unscopable(JsObject bound, string name)
    {
        var blocked = GetSymbol(JsValue.Object(bound), Realm.UnscopablesSymbol);
        return blocked.IsObject && GetProperty(blocked, name).ToBooleanValue();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=6F7A88
    // Broiler-Human:        PENDING
    private static JsEnvironment Slot(
        System.Collections.Generic.List<JsEnvironment> scopes, int depth, int index, out bool found)
    {
        var current = scopes[^1];

        for (var step = 0; step < depth; step++)
        {
            if (current.Parent is null)
            {
                found = false;
                return current;
            }

            current = current.Parent;
        }

        found = index < current.Slots.Length;
        return current;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D7D299
    // Broiler-Human:        PENDING
    private static void Binary(
        JsValue[] stack, ref int sp, System.Func<double, double, double> operation, JsEngine engine)
    {
        var right = engine.ToNumber(stack[--sp]);
        var left = engine.ToNumber(stack[--sp]);
        stack[sp++] = JsValue.Number(operation(left, right));
    }

    /// <summary>The <c>+</c> operator, which is concatenation when either side is a String.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=414277
    // Broiler-Human:        PENDING
    internal JsValue Add(JsValue left, JsValue right)
    {
        var primitiveLeft = ToPrimitive(left, "default");
        var primitiveRight = ToPrimitive(right, "default");

        if (primitiveLeft.IsString || primitiveRight.IsString)
        {
            return JsValue.String(ToStringValue(primitiveLeft) + ToStringValue(primitiveRight));
        }

        return JsValue.Number(ToNumber(primitiveLeft) + ToNumber(primitiveRight));
    }

    /// <summary>The four relational operators, through one abstract comparison.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C40206
    // Broiler-Human:        PENDING
    internal bool Relational(JsOpcode opcode, JsValue left, JsValue right)
    {
        // THE ORDER OF EVALUATION IS THE SPECIFICATION'S: `<` and `<=` convert left first, `>` and
        // `>=` convert RIGHT first. It is observable through a valueOf with a side effect, and it
        // is the kind of thing only a conformance suite ever notices.
        JsValue first;
        JsValue second;

        if (opcode is JsOpcode.LessThan or JsOpcode.LessThanOrEqual)
        {
            first = ToPrimitive(left, "number");
            second = ToPrimitive(right, "number");
        }
        else
        {
            second = ToPrimitive(right, "number");
            first = ToPrimitive(left, "number");
        }

        if (first.IsString && second.IsString)
        {
            var order = string.CompareOrdinal(first.AsString(), second.AsString());

            return opcode switch
            {
                JsOpcode.LessThan => order < 0,
                JsOpcode.LessThanOrEqual => order <= 0,
                JsOpcode.GreaterThan => order > 0,
                _ => order >= 0,
            };
        }

        var a = ToNumber(first);
        var b = ToNumber(second);

        return opcode switch
        {
            JsOpcode.LessThan => a < b,
            JsOpcode.LessThanOrEqual => a <= b,
            JsOpcode.GreaterThan => a > b,
            _ => a >= b,
        };
    }

    /// <summary>The abstract equality comparison, <c>==</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=BED8C4
    // Broiler-Human:        PENDING
    internal bool LooselyEquals(JsValue left, JsValue right)
    {
        if (left.Type == right.Type)
        {
            return left.StrictlyEquals(right);
        }

        if (left.IsNullish && right.IsNullish)
        {
            return true;
        }

        if (left.IsNullish || right.IsNullish)
        {
            return false;
        }

        if (left.Type == JsType.Number && right.Type == JsType.String)
        {
            return left.AsNumber() == ToNumber(right);
        }

        if (left.Type == JsType.String && right.Type == JsType.Number)
        {
            return ToNumber(left) == right.AsNumber();
        }

        if (left.Type == JsType.Boolean)
        {
            return LooselyEquals(JsValue.Number(left.AsBoolean() ? 1 : 0), right);
        }

        if (right.Type == JsType.Boolean)
        {
            return LooselyEquals(left, JsValue.Number(right.AsBoolean() ? 1 : 0));
        }

        if (left.IsObject && right.Type is JsType.Number or JsType.String)
        {
            return LooselyEquals(ToPrimitive(left, "default"), right);
        }

        if (right.IsObject && left.Type is JsType.Number or JsType.String)
        {
            return LooselyEquals(left, ToPrimitive(right, "default"));
        }

        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D3DBFB
    // Broiler-Human:        PENDING
    private bool InstanceOf(JsValue left, JsValue right)
    {
        // `Symbol.hasInstance` COMES FIRST AND IS CONSULTED ON ANY OBJECT, callable or not. That
        // ordering is what lets a plain object answer `instanceof` at all, and checking callability
        // before it would refuse the one case the Symbol exists for.
        if (right.IsObject && TryGetSymbolMethod(right, Realm.HasInstanceSymbol, out var custom))
        {
            return Call(custom, right, [left]).ToBooleanValue();
        }

        if (!right.IsObject || !right.AsObject().IsCallable)
        {
            ThrowTypeError("Right-hand side of 'instanceof' is not callable");
        }

        if (right.AsObject() is JsBoundFunction bound)
        {
            return InstanceOf(left, JsValue.Object(bound.Target));
        }

        if (!left.IsObject)
        {
            return false;
        }

        var prototype = GetProperty(right, "prototype");

        if (!prototype.IsObject)
        {
            ThrowTypeError("Function has non-object prototype in instanceof");
        }

        var target = prototype.AsObject();
        var walk = left.AsObject().Prototype;

        while (walk is not null)
        {
            if (ReferenceEquals(walk, target))
            {
                return true;
            }

            walk = walk.Prototype;
        }

        return false;
    }

    /// <summary>Reads an indexed property, with the fast path an Array element deserves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B503B0
    // Broiler-Human:        PENDING
    internal JsValue GetIndexed(JsValue target, JsValue key)
    {
        if (target.IsObject && target.AsObject() is JsArray array && key.Type == JsType.Number)
        {
            var number = key.AsNumber();
            var at = (int)number;

            if (at == number && at >= 0 && at < array.DenseCount)
            {
                var element = array.DenseAt(at);

                // A HOLE IS NOT AN ANSWER. It may be a hole, or it may be a slot the array vacated
                // when the element was given attributes it could not carry, in which case the value
                // is in the ordinary map and the general path finds it.
                if (!element.IsEmpty)
                {
                    return element;
                }
            }
        }

        if (target.IsString && key.Type == JsType.Number)
        {
            var text = target.AsString();
            var number = key.AsNumber();
            var at = (int)number;

            if (at == number && at >= 0 && at < text.Length)
            {
                return JsValue.String(text[at].ToString());
            }
        }

        if (key.IsSymbol)
        {
            return GetSymbol(target, key.AsSymbol());
        }

        return GetProperty(target, ToPropertyKey(key));
    }

    /// <summary>Reads a Symbol-keyed property, walking the prototype chain the same way.</summary>
    /// <remarks>
    /// It is a second walk rather than a widened one because the key type differs all the way down:
    /// the storage is a separate table, and a String key and a Symbol key can never collide, so
    /// there is nothing for the two walks to agree about.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3D0E68
    // Broiler-Human:        PENDING
    internal JsValue GetSymbol(JsValue baseValue, JsSymbol key)
    {
        if (baseValue.IsNullish)
        {
            return ThrowTypeError(
                "Cannot read properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") +
                " (reading a Symbol-keyed property)");
        }

        var current = baseValue.IsObject ? baseValue.AsObject() : PrototypeFor(baseValue);

        while (current is not null)
        {
            if (current.TryGetOwnSymbol(key, out var property))
            {
                if (!property.IsAccessor)
                {
                    return property.Value;
                }

                return property.Getter is null
                    ? JsValue.Undefined
                    : Call(JsValue.Object(property.Getter), baseValue, System.Array.Empty<JsValue>());
            }

            current = current.Prototype;
        }

        return JsValue.Undefined;
    }

    /// <summary>Writes a Symbol-keyed property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=96CCA5
    // Broiler-Human:        PENDING
    internal void SetSymbol(JsValue baseValue, JsSymbol key, JsValue value, bool strict)
    {
        if (baseValue.IsNullish)
        {
            ThrowTypeError(
                "Cannot set properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") +
                " (setting a Symbol-keyed property)");

            return;
        }

        var target = baseValue.AsObjectOrNull();
        var current = target;

        while (current is not null)
        {
            if (current.TryGetOwnSymbol(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        if (strict)
                        {
                            ThrowTypeError("Cannot set a Symbol-keyed property which has only a getter");
                        }

                        return;
                    }

                    Call(JsValue.Object(property.Setter), baseValue, [value]);
                    return;
                }

                if (!property.Writable)
                {
                    if (strict)
                    {
                        ThrowTypeError("Cannot assign to a read only Symbol-keyed property");
                    }

                    return;
                }

                break;
            }

            current = current.Prototype;
        }

        if (target is null)
        {
            return;
        }

        target.SetOwnSymbol(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>Whether a Symbol-keyed property is reachable from <paramref name="start"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D5C766
    // Broiler-Human:        PENDING
    internal bool HasSymbol(JsObject start, JsSymbol key)
    {
        var current = start;

        while (current is not null)
        {
            if (current.TryGetOwnSymbol(key, out _))
            {
                return true;
            }

            current = current.Prototype;
        }

        return false;
    }

    /// <summary>Writes an indexed property, with the fast path an Array element deserves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=715A35
    // Broiler-Human:        PENDING
    internal void SetIndexed(JsValue target, JsValue key, JsValue value, bool strict)
    {
        if (target.IsObject && target.AsObject() is JsArray array && key.Type == JsType.Number)
        {
            var number = key.AsNumber();
            var at = (int)number;

            // THE FAST PATH IS FOR AN ELEMENT THAT IS STILL AN ELEMENT. A dense slot the array
            // vacated is a property with attributes living in the ordinary map, and writing the
            // slot would step straight over a `writable: false` that somebody asked for - which is
            // what made a frozen array assignable while reporting itself frozen. Appending is a
            // fast path too, and only while the array is extensible.
            if (at == number && at >= 0)
            {
                // A CLOSED LENGTH REFUSES BEFORE EITHER FAST PATH IS TAKEN, and in strict code the
                // refusal is a TypeError. The object model drops such a write silently because it
                // cannot know the mode; this is where the mode is known.
                if (!array.LengthWritable && at >= array.Length)
                {
                    if (strict)
                    {
                        ThrowTypeError(
                            "Cannot add property " + at.ToString(
                                System.Globalization.CultureInfo.InvariantCulture) +
                            ", object is not extensible");
                    }

                    return;
                }

                if (at < array.DenseCount)
                {
                    if (!array.DenseAt(at).IsEmpty)
                    {
                        array.SetIndex((uint)at, value);
                        return;
                    }
                }
                else if (array.Extensible && at == array.Length)
                {
                    array.SetIndex((uint)at, value);
                    return;
                }
            }
        }

        if (key.IsSymbol)
        {
            SetSymbol(target, key.AsSymbol(), value, strict);
            return;
        }

        SetProperty(target, ToPropertyKey(key), value, strict);
    }

    /// <summary>Renders a thrown value for a host that has to describe it in one line.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1C82D0
    // Broiler-Human:        PENDING
    internal string Render(JsValue value)
    {
        if (!value.IsObject)
        {
            return value.IsString ? value.AsString() : ToStringValue(value);
        }

        var name = GetProperty(value, "name");
        var message = GetProperty(value, "message");

        if (!name.IsNullish || !message.IsNullish)
        {
            var head = name.IsNullish ? "Error" : ToStringValue(name);
            var tail = message.IsNullish ? string.Empty : ToStringValue(message);
            return tail.Length == 0 ? head : head + ": " + tail;
        }

        return ToStringValue(value);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C54D2D
    // Broiler-Human:        PENDING
    private static ushort U16(byte[] code, int at) => (ushort)(code[at + 1] | (code[at + 2] << 8));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3B2766
    // Broiler-Human:        PENDING
    private static uint U32(byte[] code, int at) => (uint)(
        code[at + 1] | (code[at + 2] << 8) | (code[at + 3] << 16) | (code[at + 4] << 24));
}
