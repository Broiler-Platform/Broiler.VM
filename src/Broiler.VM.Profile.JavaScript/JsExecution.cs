// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   21
// Annotated:        21/21
// Exempt:           17
// Human-reviewed:   0/21
// IP risk:          Low
// Security risk:    High
// Criteria:         9/9
// Resource impact:  6/10 max
// Unverified:       21
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>What a wide-surface entry point completed with.</summary>
/// <remarks>
/// It carries rendered text rather than the engine's own value type, and that is deliberate: a
/// host prints a completion value and branches on nothing about it, so exposing the value model
/// through the payload would put a representation this milestone calls provisional into the public
/// surface, where changing it would be a breaking change rather than an edit.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F93113
// Broiler-Human:        PENDING
public sealed class JsCompletion : IVmProfilePayload
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=79266F
    // Broiler-Human:        PENDING
    internal JsCompletion(VmProfileId profileId, string rendered, string typeOf)
    {
        Identity = new VmPayloadIdentity(profileId, JavaScriptProfile.WideCompletionKindId, 1);
        Value = rendered;
        TypeOf = typeOf;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AF660A
    // Broiler-Human:        PENDING
    public VmPayloadIdentity Identity { get; }

    /// <summary>The completion value, rendered the way the language renders it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7C00DF
    // Broiler-Human:        PENDING
    public string Value { get; }

    /// <summary>What <c>typeof</c> answers for the completion value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C2A992
    // Broiler-Human:        PENDING
    public string TypeOf { get; }
}

/// <summary>A JavaScript exception that reached the top of an entry point.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C353E6
// Broiler-Human:        PENDING
public sealed class JsUncaught : IVmProfilePayload
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BE664E
    // Broiler-Human:        PENDING
    internal JsUncaught(VmProfileId profileId, string rendered, string name)
    {
        Identity = new VmPayloadIdentity(profileId, JavaScriptProfile.WideFaultKindId, 1);
        Message = rendered;
        ErrorName = name;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AF660A
    // Broiler-Human:        PENDING
    public VmPayloadIdentity Identity { get; }

    /// <summary>One line describing what was thrown.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DFE792
    // Broiler-Human:        PENDING
    public string Message { get; }

    /// <summary>
    /// The <c>name</c> of the thrown value's constructor, which is what a conformance suite's
    /// negative expectation is matched against.
    /// </summary>
    /// <remarks>
    /// Empty when the thrown value is not an object with a constructor - throwing a string is
    /// legal and a runner has to be able to tell the two cases apart.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=05F22E
    // Broiler-Human:        PENDING
    public string ErrorName { get; }
}

/// <summary>What a host is told while an operation is parked between two job turns.</summary>
/// <remarks>
/// <b>It carries a count and nothing reachable.</b> A projection is handed to the host while the
/// operation is suspended, so anything in it that pointed back into the realm would be guest state
/// crossing the boundary at the one moment nothing is running to defend it. The number of jobs
/// still queued is what a host driving an event loop actually reads — whether to step again — and
/// it is a number rather than a handle.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=6E5458
// Broiler-Falsified-If: anything reachable from the realm is published through this payload
// Broiler-Human:        PENDING
public sealed class JsPause : IVmProfilePayload
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5F2BC7
    // Broiler-Human:        PENDING
    internal JsPause(VmProfileId profileId, int pendingJobs, string thrown)
    {
        Identity = new VmPayloadIdentity(profileId, JavaScriptProfile.WidePauseKindId, 1);
        PendingJobs = pendingJobs;
        Thrown = thrown;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AF660A
    // Broiler-Human:        PENDING
    public VmPayloadIdentity Identity { get; }

    /// <summary>How many jobs are still queued behind this pause.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D5A239
    // Broiler-Human:        PENDING
    public int PendingJobs { get; }

    /// <summary>
    /// What the job that just ran threw, rendered, or the empty string if it did not throw.
    /// </summary>
    /// <remarks>
    /// A drain folds every fault into the first and reports it once at the end. Stepping reports
    /// each one at the turn it happened, which is the difference a host stepping the queue is
    /// asking for; folding them here would lose every fault but one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F15B5B
    // Broiler-Human:        PENDING
    public string Thrown { get; }
}

/// <summary>Where a parked wide-surface operation resumes.</summary>
/// <remarks>
/// <para>
/// <b>It names the instance and carries no frame, because the queue is the instance's.</b> The
/// continuation exists to say <i>which</i> parked operation this is, not to hold what it was doing:
/// a job turn begins and ends at the queue, so between two turns there is nothing on any stack to
/// capture. That is what makes this pause cost no thread — roadmap section 12's requirement — and
/// it is why the continuation is a token rather than a frame.
/// </para>
/// <para>
/// <b>Single use is the core's to enforce and this does not second-guess it.</b> A second resume, a
/// resume after cancellation or disposal, and a resume presented to a runtime that does not own the
/// continuation are all refused before this profile is reached. What this type adds is the instance
/// check the core cannot make: a continuation from one instance presented against another is this
/// profile's own confusion to catch.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=231B03
// Broiler-Falsified-If: a continuation is honoured against an instance that did not produce it
// Broiler-Human:        PENDING
internal sealed class JsContinuation : IVmProfileContinuation
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7ABACC
    // Broiler-Human:        PENDING
    internal JsContinuation(JsInstance instance, int turn)
    {
        Instance = instance;
        Turn = turn;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F89838
    // Broiler-Human:        PENDING
    internal JsInstance Instance { get; }

    /// <summary>Which turn of the stepping this continuation resumes into.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E8AD94
    // Broiler-Human:        PENDING
    internal int Turn { get; }
}

/// <summary>
/// One instance of a wide-surface program: a realm, and the engine that runs in it.
/// </summary>
/// <remarks>
/// <b>The realm outlives one invocation, and that is what makes several scripts one program.</b>
/// The conformance suite requires its harness files to be separate scripts evaluated in the test's
/// realm; the artifact carries one entry point per script and the host invokes them in order
/// against this one instance, so the second sees what the first declared. A fresh instance is a
/// fresh realm.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=792E06
// Broiler-Human:        PENDING
internal sealed class JsInstance : IVmInstanceState
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E31EEC
    // Broiler-Human:        PENDING
    internal JsInstance(JsProgram program, JsEngine engine, IVmExecutionEnvironment environment)
    {
        Program = program;
        Engine = engine;
        Environment = environment;
    }

    /// <summary>The environment the instance was created against.</summary>
    /// <remarks>
    /// It is held for one thing only: asking, at the start of each invocation, for the artifact
    /// load mediator that invocation may use. The mediator itself is never held across
    /// invocations — the contract says one is valid for the dynamic extent of the invocation that
    /// supplied it, and a profile holding a stale one would be naming a mediator the core reports
    /// as out of scope.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=1C7767
    // Broiler-Falsified-If: this environment is asked for a mediator outside an invocation it supplied one for
    // Broiler-Human:        PENDING
    internal IVmExecutionEnvironment Environment { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9D1393
    // Broiler-Human:        PENDING
    internal JsProgram Program { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=295C08
    // Broiler-Human:        PENDING
    internal JsEngine Engine { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=218248
    // Broiler-Human:        PENDING
    internal int InvocationCount { get; set; }
}

/// <summary>The wide surface's half of the executor: instantiate, invoke, and report.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E1ACB5
// Broiler-Human:        PENDING
internal static class JsExecution
{
    /// <summary>Builds an instance and its realm.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=809277
    // Broiler-Human:        PENDING
    internal static VmExecutionStep Instantiate(
        JsProgram program,
        IVmExecutionEnvironment environment,
        System.Threading.CancellationToken cancellationToken)
    {
        if (!environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))
        {
            return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
        }

        var engine = new JsEngine(
            environment.Meter,
            cancellationToken,
            environment.Capabilities,
            program.AdmittedSurfaces);
        var instance = new JsInstance(program, engine, environment);

        // A realm is the largest thing this profile retains, and it is retained rather than
        // consumed: it lives for the instance's lifetime and is released with it.
        environment.Meter.ReportRetained(VmBudgetDimension.LiveBytes, 262_144);

        return VmExecutionStep.Instantiated(instance, null);
    }

    /// <summary>The reserved entry-point name a host drains the job queue by invoking.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E1285C
    // Broiler-Human:        PENDING
    internal const string DrainEntryPoint = JavaScriptProfile.DrainEntryPoint;

    /// <summary>The reserved entry-point name a host steps the job queue by invoking.</summary>
    /// <remarks>
    /// <para>
    /// <b>The second of the two reserved names, and the routing decision roadmap section 12 leaves
    /// to this milestone.</b> Section 12 warns against routing every microtask through a core
    /// suspension — the suspended-operation limit would then govern a page rather than a pathology
    /// — so the routing here is not <i>every pause</i> but <i>the pause a host asked for</i>: a
    /// drain is still one operation running the queue to exhaustion, and a step is a host saying it
    /// wants the turn as its unit. A composition that never invokes this name creates no
    /// suspension at all, which is what keeps the live-suspension count a property of the embedding
    /// rather than of the program.
    /// </para>
    /// <para>
    /// <b>A generator's <c>yield</c> and an <c>await</c> stay where they were.</b> They suspend on a
    /// heap frame inside one operation and no core suspension is created for them, which is what
    /// section 12's first row says and what JSW-8 built. Nothing here changes that; what this adds
    /// is a pause the <b>host</b> owns, between turns rather than inside one.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=815A20
    // Broiler-Falsified-If: a guest pause creates a core suspension, or a step runs more than one job
    // Broiler-Human:        PENDING
    internal const string StepEntryPoint = JavaScriptProfile.StepEntryPoint;

    /// <summary>Runs one due job and parks if the queue still holds anything.</summary>
    /// <remarks>
    /// A step over an empty queue completes rather than parking, so a host may step until it is
    /// told there is nothing left without having to ask first — and so that a program with no jobs
    /// at all never creates a suspension.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=B5AF9E
    // Broiler-Falsified-If: a step parks with an empty queue, or completes with a job still due
    // Broiler-Human:        PENDING
    private static VmExecutionStep StepJobs(VmProfileId profileId, JsInstance instance, int turn)
    {
        instance.Engine.Loader =
            instance.Environment.TryGetArtifactLoadMediator(out var mediator) ? mediator : null;

        try
        {
            var thrown = RunOneJobOnGuestStack(instance);

            if (!instance.Engine.HasPendingJobs)
            {
                return VmExecutionStep.Completed(
                    new JsCompletion(profileId, thrown, thrown.Length == 0 ? "undefined" : "object"));
            }

            return VmExecutionStep.Suspended(
                new JsContinuation(instance, turn + 1),
                new JsPause(profileId, instance.Engine.PendingJobCount, thrown));
        }
        catch (JsAbort abort)
        {
            return abort.Kind switch
            {
                JsAbortKind.Cancelled => VmExecutionStep.ContractViolation(VmReason.Cancelled),
                JsAbortKind.Exhausted => VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted),
                _ => VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation),
            };
        }
        catch (System.InsufficientExecutionStackException)
        {
            return VmExecutionStep.Faulted(
                new JsUncaught(profileId, "Maximum call stack size exceeded", "RangeError"));
        }
        finally
        {
            instance.Engine.Loader = null;
        }
    }

    /// <summary>Resumes a parked stepping operation into its next turn.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=5DF4EF
    // Broiler-Falsified-If: a continuation is honoured against an instance that did not produce it
    // Broiler-Human:        PENDING
    internal static VmExecutionStep Resume(
        VmProfileId profileId, IVmInstanceState state, IVmProfileContinuation continuation)
    {
        if (state is not JsInstance instance ||
            continuation is not JsContinuation parked ||
            !ReferenceEquals(parked.Instance, instance))
        {
            // THE CORE HAS ALREADY REFUSED A FOREIGN RUNTIME, A SECOND RESUME AND A RESUME AFTER
            // DISPOSAL. What is left for this profile to catch is a continuation of ITS OWN kind
            // presented against a different instance of the same runtime, which the core cannot see
            // because both objects are this profile's.
            return VmExecutionStep.ContractViolation(VmReason.ForeignPayload);
        }

        instance.InvocationCount++;
        return StepJobs(profileId, instance, parked.Turn);
    }

    /// <summary>Abandons a parked stepping operation, running none of what it had queued.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=CE785F
    // Broiler-Falsified-If: guest code runs during an unwind
    // Broiler-Human:        PENDING
    internal static void Unwind(IVmProfileContinuation continuation)
    {
        if (continuation is JsContinuation parked)
        {
            _ = parked.Instance.Engine.DropPendingJobs();
        }
    }

    /// <summary>Runs every due job on the guest stack and reports what happened.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BF301D
    // Broiler-Human:        PENDING
    private static VmExecutionStep DrainJobs(VmProfileId profileId, JsInstance instance)
    {
        instance.InvocationCount++;

        instance.Engine.Loader =
            instance.Environment.TryGetArtifactLoadMediator(out var mediator) ? mediator : null;

        try
        {
            var value = RunOnGuestStack(instance, unit: null);

            return VmExecutionStep.Completed(
                new JsCompletion(profileId, instance.Engine.ToStringValue(value), value.TypeOf()));
        }
        catch (JsThrow thrown)
        {
            return VmExecutionStep.Faulted(
                new JsUncaught(
                    profileId, instance.Engine.Render(thrown.Value), ConstructorName(instance, thrown.Value)));
        }
        catch (JsAbort abort)
        {
            return abort.Kind switch
            {
                JsAbortKind.Cancelled => VmExecutionStep.ContractViolation(VmReason.Cancelled),
                JsAbortKind.Exhausted => VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted),
                _ => VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation),
            };
        }
        catch (System.InsufficientExecutionStackException)
        {
            return VmExecutionStep.Faulted(
                new JsUncaught(profileId, "Maximum call stack size exceeded", "RangeError"));
        }
        finally
        {
            instance.Engine.Loader = null;
        }
    }

    /// <summary>Runs one entry point against an existing realm.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DB0FB0
    // Broiler-Human:        PENDING
    internal static VmExecutionStep Invoke(
        VmProfileId profileId, JsInstance instance, in VmInvocationRequest request)
    {
        var name = System.Text.Encoding.UTF8.GetString(request.EntryPoint.Utf8);

        // THE ONE ENTRY POINT NAME THIS PROFILE OWNS, AND THE POINT AT WHICH JOBS RUN.
        //
        // A queue drained implicitly at a point nobody stated is a behaviour no embedder can reason
        // about: one host runs a script and stops, another runs several in one realm, a third
        // interleaves them with work of its own, and each wants a different moment. So the profile
        // never chooses - the host asks, by invoking this name, and gets the drain as an ordinary
        // invocation with an ordinary result. A program that throws inside a job faults this
        // invocation rather than the one that enqueued it, which is what makes the two
        // distinguishable in a transcript.
        //
        // The name cannot collide with a script's, because a script entry point is named by
        // whatever compiled it and `#` begins no JavaScript identifier.
        if (string.Equals(name, DrainEntryPoint, System.StringComparison.Ordinal))
        {
            return DrainJobs(profileId, instance);
        }

        if (string.Equals(name, StepEntryPoint, System.StringComparison.Ordinal))
        {
            instance.InvocationCount++;
            return StepJobs(profileId, instance, 0);
        }

        if (!instance.Program.TryFindEntry(name, out var unit))
        {
            return VmExecutionStep.Faulted(
                new JsUncaught(profileId, "entry point is not defined", "ReferenceError"));
        }

        instance.InvocationCount++;

        // THE MEDIATOR IS TAKEN AT THE START OF THE INVOCATION AND DROPPED AT ITS END, which is
        // exactly the extent the contract gives it. A composition that registered no artifact
        // provider yields nothing here, and the difference between that and a composition that
        // declined the dynamic surface is the whole of what roadmap section 6 distinguishes: this
        // one is a run-time refusal the guest may catch, the other was an invalid artifact.
        instance.Engine.Loader =
            instance.Environment.TryGetArtifactLoadMediator(out var mediator) ? mediator : null;

        try
        {
            var value = RunOnGuestStack(instance, unit);

            return VmExecutionStep.Completed(
                new JsCompletion(
                    profileId, instance.Engine.ToStringValue(value), value.TypeOf()));
        }
        catch (JsThrow thrown)
        {
            return VmExecutionStep.Faulted(
                new JsUncaught(
                    profileId, instance.Engine.Render(thrown.Value), ConstructorName(instance, thrown.Value)));
        }
        catch (JsAbort abort)
        {
            return abort.Kind switch
            {
                JsAbortKind.Cancelled => VmExecutionStep.ContractViolation(VmReason.Cancelled),
                JsAbortKind.Exhausted => VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted),
                _ => VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation),
            };
        }
        catch (System.InsufficientExecutionStackException)
        {
            return VmExecutionStep.Faulted(
                new JsUncaught(profileId, "Maximum call stack size exceeded", "RangeError"));
        }
        finally
        {
            instance.Engine.Loader = null;
        }
    }

    /// <summary>
    /// How much native stack one guest invocation gets.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The profile chooses the stack the guest runs on, because otherwise the call-depth ceiling
    /// means a different thing on every host.</b> This interpreter recurses once per JavaScript
    /// call, and a frame of it is over a kilobyte; on the one-megabyte stack a Windows process
    /// hands its main thread, a program recursing 776 deep ended the PROCESS rather than the
    /// operation - and a stack overflow is the one failure the CLR cannot turn into an exception,
    /// so no meter, no ceiling and no probe can report it after the fact.
    /// </para>
    /// <para>
    /// <b>Ninety-six megabytes is MEASURED against the declared ceiling and not chosen.</b>
    /// <c>eng/measure-frame-cost.py</c> bisects the published binary and reports how deep a
    /// recursion this stack holds; dividing the stack by that depth gives the cost of one guest
    /// call, which after the async family and <c>with</c> joined the instruction set is
    /// <b>3,736 bytes</b> - the executor's own frame having grown as the set did, from 3,179
    /// through 3,463 to this. Sixteen
    /// megabytes held 5,278 calls, which is BELOW
    /// <see cref="JsEngine.MaximumCallDepth"/>, so a runaway recursion reached the stack before it
    /// reached the bound and terminated the process - which is JSC-85 exactly, and is the failure
    /// this figure exists to prevent.
    /// </para>
    /// <para>
    /// <b>THIS FIGURE MOVED, and what moved it is the only reason it ever should.</b> Sixty-four
    /// megabytes held 17,963 calls until the class body's six dispatch arms took the executor's own
    /// frame to 4,073 bytes and the capacity to 16,478 <i>(JSC-126)</i>, which was 2.01 times the
    /// call-depth maximum the descriptor lets a host grant - the narrowest that margin had ever
    /// been. Asynchronous iteration added five more arms and took the frame to <b>4,551</b> bytes,
    /// at which sixty-four megabytes holds <b>14,737</b> calls: 1.80 times the grantable ceiling,
    /// BELOW the factor of two, and therefore a stack that no longer keeps the ordering the ceiling
    /// depends on. Ninety-six megabytes holds <b>22,122</b>, which is 2.70 times that ceiling and
    /// 3.69 times <see cref="JsEngine.MaximumCallDepth"/>. Both figures are measured on a build with
    /// the engine's bound and the profile's call-depth maximum lifted, because a bisection that
    /// stops at a declared bound reports the promise and not the capacity <i>(JSC-139)</i>.
    /// </para>
    /// <para>
    /// <b>A SECOND RUNTIME IDENTIFIER HAS NOW BEEN MEASURED, and it costs more per frame than the
    /// first.</b> On <c>linux-x64</c> ninety-six megabytes holds <b>19,756</b> calls, which is
    /// <b>5,095 bytes</b> a call against the 4,551 above. The ordering the ceiling depends on still
    /// holds — 2.41 times the grantable maximum and 3.29 times
    /// <see cref="JsEngine.MaximumCallDepth"/>, both above the factor of two — so nothing here
    /// moves; what changes is that the margin is now known on two machines rather than assumed to
    /// be one number. The returning and throwing shapes agreed exactly, which is the property
    /// JSC-97 left behind and the one a second platform could have broken
    /// <i>(JSC-192)</i>.
    /// </para>
    /// <para>
    /// The room this leaves is what it always left: the built-ins that recurse in C# without going
    /// through a call - a comparison function driving a sort, a cycle-free walk of a deep object in
    /// JSON - along with the stack a host has already used before it reached this profile.
    /// </para>
    /// <para>
    /// <b>The figure is a ceiling on ADDRESS SPACE and not on memory.</b> A thread's stack is
    /// reserved when the thread is made and committed a page at a time as it is used, so a program
    /// that never recurses pays for none of it - which is what makes measuring against the declared
    /// maximum the right conservatism rather than an expensive one.
    /// </para>
    /// <para>
    /// <b>A thread per invocation rather than one per instance.</b> An instance can outlive many
    /// invocations, and a thread parked between them would be a resource this profile holds while
    /// doing nothing; a fresh one costs a fraction of a millisecond against a program that runs for
    /// milliseconds at least, and it starts every invocation with the same stack whatever the
    /// previous one did.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=525EF4
    // Broiler-Human:        PENDING
    private const int GuestStackBytes = 96 * 1024 * 1024;

    /// <summary>Runs one entry point on a thread whose stack this profile declared.</summary>
    /// <remarks>
    /// Whatever the guest raises is carried back and rethrown here, so the caller sees the same
    /// exception it would have seen had the interpreter run on its own thread. The profile declares
    /// <c>Agile</c> thread affinity, so the core pins no operation to a thread, and the one host
    /// capability this profile imports declares caller-thread affinity - which this satisfies: the
    /// thread that calls it is the thread the guest is running on.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=CDA795
    // Broiler-Falsified-If: guest code runs on the caller's stack, or an exception the guest raised does not reach the caller
    // Broiler-Human:        PENDING
    private static JsValue RunOnGuestStack(JsInstance instance, uint? unit)
    {
        var completed = JsValue.Undefined;
        System.Runtime.ExceptionServices.ExceptionDispatchInfo? raised = null;

        var worker = new System.Threading.Thread(
            () =>
            {
                try
                {
                    // A DRAIN RUNS ON THE SAME STACK A SCRIPT DOES. A job is guest code and can
                    // recurse exactly as guest code does, so running it on the caller's stack would
                    // reintroduce the process termination JSC-79 records.
                    completed = unit is { } entry
                        ? instance.Engine.RunEntry(instance.Program, entry)
                        : instance.Engine.DrainJobs();
                }
                catch (System.Exception failure)
                {
                    raised = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure);
                }
            },
            GuestStackBytes)
        {
            IsBackground = true,
            Name = "broiler-js-guest",
        };

        worker.Start();
        worker.Join();
        raised?.Throw();
        return completed;
    }

    /// <summary>Runs one job on a thread whose stack this profile declared.</summary>
    /// <remarks>
    /// A job is guest code and recurses exactly as guest code does, so it gets the same stack a
    /// script gets. What it returns is the rendering of whatever it threw, or the empty string —
    /// carried out rather than raised, because a job that throws does not stop the stepping any
    /// more than it stops a drain.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=6D52BB
    // Broiler-Falsified-If: a job runs on the caller's stack, or a job that throws ends the stepping
    // Broiler-Human:        PENDING
    private static string RunOneJobOnGuestStack(JsInstance instance)
    {
        var rendered = string.Empty;
        System.Runtime.ExceptionServices.ExceptionDispatchInfo? raised = null;

        var worker = new System.Threading.Thread(
            () =>
            {
                try
                {
                    if (instance.Engine.StepOneJob(out var thrown))
                    {
                        rendered = instance.Engine.Render(thrown);
                    }
                }
                catch (System.Exception failure)
                {
                    raised = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure);
                }
            },
            GuestStackBytes)
        {
            IsBackground = true,
            Name = "broiler-js-guest",
        };

        worker.Start();
        worker.Join();
        raised?.Throw();
        return rendered;
    }

    /// <summary>
    /// The name of the thrown value's constructor, which is what a negative expectation matches.
    /// </summary>
    /// <remarks>
    /// It is read through the ordinary property path rather than off a C# type, because the value
    /// a program throws may be an object of its own whose constructor is a guest function - which
    /// is exactly the case the conformance suite's own <c>Test262Error</c> is.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=4CF0BB
    // Broiler-Human:        PENDING
    private static string ConstructorName(JsInstance instance, JsValue value)
    {
        if (!value.IsObject)
        {
            return string.Empty;
        }

        try
        {
            var constructor = instance.Engine.GetProperty(value, "constructor");

            if (!constructor.IsObject)
            {
                return string.Empty;
            }

            var name = instance.Engine.GetProperty(constructor, "name");
            return name.IsString ? name.AsString() : string.Empty;
        }
        catch (JsThrow)
        {
            return string.Empty;
        }
        catch (JsAbort)
        {
            return string.Empty;
        }
    }
}
