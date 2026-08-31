// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   24
// Annotated:        24/24
// Exempt:           16
// Human-reviewed:   0/24
// IP risk:          Low
// Security risk:    High
// Criteria:         6/2
// Resource impact:  6/10 max
// Unverified:       24
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>The runtime's implementation of an instance.</summary>
/// <remarks>
/// <para>
/// One execution slot per instance. A second invocation while one is running is refused rather than
/// queued: queuing would be a scheduler, and the core does not schedule.
/// </para>
/// <para>
/// An instance is published only when instantiation completes normally, so there is no
/// half-instantiated instance a caller can obtain and then wonder about.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=091BAF
// Broiler-Human:        PENDING
internal sealed class VmInstanceImplementation : VmInstance
{
    private readonly object gate = new();
    private readonly VmRuntime runtime;
    private readonly VmProfileDescriptor profile;
    private readonly IVmProfileExecutor executor;
    private readonly IVmInstanceState state;
    private readonly VmBudgetLevel instanceLevel;
    private readonly VmDiagnostics baseline;
    private readonly VmExecutionScope scope;
    private readonly VmArtifactLoadMediator? mediator;
    private readonly VmArtifactLease lease;

    private VmInstanceState currentState = VmInstanceState.Live;
    private VmOperation? active;

    /// <summary>How many steps are inside the profile right now.</summary>
    /// <remarks>
    /// One instance admits one step at a time, so this is zero or one - but it is a count rather
    /// than a flag because disposal WAITS on it, and a flag would say whether a step is running
    /// while a count says when the last one left. Guarded by <c>gate</c> and pulsed on every
    /// decrement, so the waiter wakes as soon as the profile returns rather than at a poll.
    /// </remarks>
    private int stepsInFlight;

    /// <summary>
    /// Set when disposal gave up waiting for a step, so the step releases the lease as it leaves.
    /// </summary>
    /// <remarks>
    /// The alternative was to release the lease anyway, which is the use-after-dispose this whole
    /// mechanism exists to prevent: the verified state the executor is still reading would be
    /// disposed under it. Handing the release to the departing step is what ADR 0006's Draining
    /// state is for - the handle drains when its last lease goes, and here that is a moment later
    /// than disposal returns.
    /// </remarks>
    private bool releaseLeaseOnExit;

    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=Low; Resources=1; Fingerprint=17A728
    // Broiler-Human:        PENDING
    internal VmInstanceImplementation(
        VmRuntime runtime,
        VmProfileDescriptor profile,
        IVmProfileExecutor executor,
        IVmInstanceState state,
        VmBudgetLevel instanceLevel,
        VmDiagnostics baseline,
        VmExecutionScope scope,
        VmArtifactLoadMediator? mediator,
        VmArtifactLease lease)
    {
        this.runtime = runtime;
        this.profile = profile;
        this.executor = executor;
        this.state = state;
        this.instanceLevel = instanceLevel;
        this.baseline = baseline;
        this.scope = scope;
        this.mediator = mediator;
        this.lease = lease;
        Identity = VmObjectId.Mint();
    }

    internal VmObjectId Identity { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=00165A
    // Broiler-Human:        PENDING
    public override VmObjectId ObjectId => Identity;

    /// <inheritdoc/>
    public override VmProfileId ProfileId => profile.ProfileId;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=Low; Resources=0; Fingerprint=D103C9
    // Broiler-Falsified-If: an instance whose instantiation is still parked reports Live rather than Instantiating or Suspended
    // Broiler-Human:        PENDING
    public override VmInstanceState State
    {
        get
        {
            lock (gate)
            {
                return currentState;
            }
        }
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=Medium; Resources=5; Fingerprint=BAA561
    // Broiler-Human:        PENDING
    public override VmInvocationResult Invoke(
        in VmInvocationRequest request,
        System.Threading.CancellationToken cancellationToken) =>
        Invoke(in request, VmLimitOverrides.None, cancellationToken, out _);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=Medium; Resources=5; Fingerprint=D380BC
    // Broiler-Human:        PENDING
    public override VmInvocationResult Invoke(
        in VmInvocationRequest request,
        System.Threading.CancellationToken cancellationToken,
        out VmOperationControlHandle controlHandle) =>
        Invoke(in request, VmLimitOverrides.None, cancellationToken, out controlHandle);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=5; Fingerprint=ED65D9
    // Broiler-Human:        PENDING
    public override VmInvocationResult Invoke(
        in VmInvocationRequest request,
        VmLimitOverrides limitOverrides,
        System.Threading.CancellationToken cancellationToken) =>
        Invoke(in request, limitOverrides, cancellationToken, out _);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=5; Fingerprint=A72EEA
    // Broiler-Falsified-If: a refused override leaves the operation running, or one dimension of it applied
    // Broiler-Human:        PENDING
    public override VmInvocationResult Invoke(
        in VmInvocationRequest request,
        VmLimitOverrides limitOverrides,
        System.Threading.CancellationToken cancellationToken,
        out VmOperationControlHandle controlHandle)
    {
        controlHandle = null!;

        var identified = baseline.WithOutcome(VmStage.Invocation, VmOutcome.None, VmReason.None, VmInitiator.Caller);

        if (!runtime.TryBeginCall(out var runtimeFailure))
        {
            return VmInvocationResult.InvalidState(
                runtimeFailure,
                VmRuntime.Invalid(identified, VmStage.Invocation, runtimeFailure, VmObjectKind.Runtime, VmAttemptedCall.Invoke));
        }

        try
        {
            VmOperation operation;

            lock (gate)
            {
                if (!TryAdmit(out var instanceFailure))
                {
                    return VmInvocationResult.InvalidState(
                        instanceFailure,
                        VmRuntime.Invalid(identified, VmStage.Invocation, instanceFailure, VmObjectKind.Instance, VmAttemptedCall.Invoke));
                }

                // P4, inside the admission lock and before the operation exists: a refused
                // override must leave no operation, no linked token source and no meter behind.
                if (!VmLimitPrecedence.TryApply(
                        VmBudgetScope.Invocation,
                        instanceLevel.CeilingsCopy(),
                        limitOverrides,
                        out var operationCeilings,
                        out var offending,
                        out var overrideFailure))
                {
                    return VmInvocationResult.HostFailure(
                        overrideFailure,
                        identified
                            .WithOutcome(VmStage.Invocation, VmOutcome.HostFailure, overrideFailure, VmInitiator.Caller)
                            .WithExhaustion(offending, VmBudgetScope.Invocation));
                }

                var linked = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                var invocationLevel = new VmBudgetLevel(VmBudgetScope.Invocation, operationCeilings);

                var meter = new VmMeter(
                    runtime.Gate, invocationLevel, instanceLevel, runtime.RuntimeLevel, runtime.Parent,
                    profile.MaxUnchargedWork, linked.Token);

                operation = new VmOperation(
                    runtime, this, profile, VmOperationKind.Invoke, meter, linked,
                    VmStage.Invocation, identified);

                active = operation;
                currentState = VmInstanceState.Executing;
                stepsInFlight++;
            }

            controlHandle = new VmOperationControlHandleImplementation(operation);

            try
            {
                return RunInvocation(operation, in request);
            }
            finally
            {
                LeaveStep();
            }
        }
        finally
        {
            runtime.EndCall();
        }
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=Medium; Resources=0; Fingerprint=C860A4
    // Broiler-Human:        PENDING
    public override VmControlResult RequestCancel()
    {
        VmOperation? operation;

        lock (gate)
        {
            operation = active;
        }

        return operation?.RequestCancel() ?? VmControlResult.NoOp;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=Medium; Resources=2; Fingerprint=2C9D03
    // Broiler-Human:        PENDING
    public override VmControlResult Dispose() => Dispose(runtime.Options.DisposeDrainBudget);

    /// <summary>
    /// Disposes the instance, waiting up to <paramref name="drainBudget"/> for a step that is
    /// inside the profile to leave it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The wait is the point, and its bound is the point of the bound. Disposal that returned while
    /// the executor was still running would release the artifact lease and give back the instance's
    /// retained bytes under a profile that is still reading the verified state - a use-after-dispose
    /// the core would have performed on itself, on a path no single-threaded test can reach. So
    /// disposal cancels the operation, then waits for the step to return.
    /// </para>
    /// <para>
    /// It cannot wait forever: a profile that ignores its cancellation token would otherwise wedge
    /// the disposing thread, which ADR 0004 names as a release blocker. The wait is therefore
    /// bounded by the host's own <c>DisposeDrainBudget</c> - the core's bound on its own waiting,
    /// which is a promise the core can keep, unlike a bound on the profile's work. When it expires
    /// the instance is disposed anyway and the lease release is handed to the departing step, so
    /// the handle drains a moment later instead of being disposed under its reader.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=High; Resources=6; Fingerprint=3B4D81
    // Broiler-Falsified-If: disposal returns while stepsInFlight is above zero and still releases the lease
    // Broiler-Human:        PENDING
    internal VmControlResult Dispose(System.TimeSpan drainBudget)
    {
        VmOperation? operation;
        bool drained;

        lock (gate)
        {
            if (currentState is VmInstanceState.Disposed)
            {
                return VmControlResult.NoOp;
            }

            if (currentState is VmInstanceState.Disposing)
            {
                return VmControlResult.NoOp;
            }

            currentState = VmInstanceState.Disposing;
            operation = active;
            active = null;
        }

        // Cancellation first, outside the lock, because the step that must observe it may be inside
        // the profile on another thread and the lock is what its exit path needs.
        operation?.Abandon(VmReason.Cancelled);

        lock (gate)
        {
            var deadline = System.DateTime.UtcNow + drainBudget;

            while (stepsInFlight > 0)
            {
                var remaining = deadline - System.DateTime.UtcNow;

                if (remaining <= System.TimeSpan.Zero || !System.Threading.Monitor.Wait(gate, remaining))
                {
                    break;
                }
            }

            drained = stepsInFlight == 0;
            releaseLeaseOnExit = !drained;
        }

        ReleaseRetained();

        lock (gate)
        {
            currentState = VmInstanceState.Disposed;
        }

        // The handle is pinned for as long as the instance lives, so releasing the lease is the
        // last thing disposal does. A handle whose last lease goes with its last instance completes
        // its drain here rather than being left half-disposed - unless a step outstayed the drain
        // budget, in which case the lease is that step's to release as it leaves.
        if (drained)
        {
            lease.Release();
        }

        runtime.ForgetInstance(this);
        return VmControlResult.Accepted;
    }

    /// <summary>
    /// Records that a step has left the profile, and wakes a disposal that is waiting for it.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=High; Resources=2; Fingerprint=E8A45C
    // Broiler-Falsified-If: a step returns without decrementing, so a later disposal waits its whole budget
    // Broiler-Human:        PENDING
    private void LeaveStep()
    {
        var release = false;

        lock (gate)
        {
            if (stepsInFlight > 0)
            {
                stepsInFlight--;
            }

            if (stepsInFlight == 0 && releaseLeaseOnExit)
            {
                releaseLeaseOnExit = false;
                release = true;
            }

            System.Threading.Monitor.PulseAll(gate);
        }

        if (release)
        {
            // The disposal that gave up waiting left this behind. Releasing it here rather than
            // there is what keeps the verified state alive for exactly as long as the profile was
            // still reading it.
            lease.Release();
        }
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0009; IP=Low; Security=Medium; Resources=5; Fingerprint=51DB74
    // Broiler-Human:        PENDING
    internal VmResumeResult ResumeOperation(VmOperation operation, IVmProfileContinuation continuation)
    {
        lock (gate)
        {
            if (currentState is VmInstanceState.Disposed or VmInstanceState.Disposing)
            {
                return VmResumeResult.InvalidState(
                    operation.Stage,
                    VmReason.ObjectDisposed,
                    VmRuntime.Invalid(operation.Baseline, VmStage.Resume, VmReason.ObjectDisposed, VmObjectKind.Instance, VmAttemptedCall.Resume));
            }

            active = operation;
            currentState = VmInstanceState.Executing;
            stepsInFlight++;
        }

        try
        {
            return RunResume(operation, continuation);
        }
        finally
        {
            LeaveStep();
        }
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0009; IP=Low; Security=Medium; Resources=5; Fingerprint=A3C1CE
    // Broiler-Human:        PENDING
    private VmResumeResult RunResume(VmOperation operation, IVmProfileContinuation continuation)
    {
        VmExecutionStep step;

        // The scope answers only inside the dynamic extent of this step. A profile that stashed its
        // meter or its mediator during an earlier step and uses it now is refused rather than
        // charged against whatever operation happens to be running.
        scope.Enter(operation.Meter, operation);
        mediator?.EnterScope(operation.Baseline, operation.ObjectId);

        try
        {
            step = executor.Resume(state, continuation, operation.Token);
        }
        catch (System.OperationCanceledException)
        {
            return Finish(operation, VmResumeResult.Cancellation(
                operation.Stage, VmReason.Cancelled,
                operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host)));
        }
        catch (System.Exception)
        {
            return Finish(operation, VmResumeResult.ProfileFault(
                operation.Stage, VmReason.ProfileContractViolation, null,
                operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.ProfileFault, VmReason.ProfileContractViolation, VmInitiator.Guest)));
        }
        finally
        {
            scope.Leave();
        }

        return Finish(operation, MapResume(operation, step));
    }

    /// <summary>
    /// Gives back the live bytes this instance still holds, at every scope that recorded them.
    /// </summary>
    /// <remarks>
    /// A retained-bytes report commits at the instance, runtime and aggregate levels alike. Dropping
    /// the instance level on dispose therefore reclaims nothing outside it, and a host running
    /// repeated load-run-evict cycles would watch the runtime and its parent climb toward their
    /// ceilings while nothing was actually retained. The memory plateau the lifecycle promises is
    /// this method: the ceiling-class dimensions are released, and the allowance-class ones are
    /// deliberately not, because an allowance never refunds.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=F871C8
    // Broiler-Human:        PENDING
    private void ReleaseRetained()
    {
        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (VmBudgetDimensions.ClassOf(dimension) is not VmBudgetClass.Ceiling)
            {
                continue;
            }

            ulong retained;

            lock (runtime.Gate)
            {
                // What this level holds is exactly what the parent accepted: a retention the
                // parent refused was never committed here, so releasing this amount credits the
                // parent precisely what it was debited.
                retained = instanceLevel.Consumed(dimension);

                if (retained == 0)
                {
                    continue;
                }

                instanceLevel.Release(dimension, retained);
                runtime.RuntimeLevel.Release(dimension, retained);
            }

            runtime.Parent?.Release(dimension, retained);
        }
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0009; IP=Low; Security=Medium; Resources=4; Fingerprint=1103B7
    // Broiler-Human:        PENDING
    internal void Unwind(IVmProfileContinuation continuation)
    {
        // The tighter of the profile's declared abandon budget and the runtime's unwind budget. A
        // profile that declares none leaves the entry point empty and its continuation is dropped
        // deterministically.
        var allowance = System.Math.Min(profile.AbandonBudget, runtime.Options.UnwindBudget);

        try
        {
            executor.Unwind(continuation, allowance);
        }
        catch (System.Exception)
        {
            // A profile that throws while unwinding has already been abandoned. There is nobody
            // left to report to, and letting it escape would turn one abandoned operation into a
            // failed disposal for the whole runtime.
        }
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=Medium; Resources=5; Fingerprint=5D2AA9
    // Broiler-Human:        PENDING
    private VmInvocationResult RunInvocation(VmOperation operation, in VmInvocationRequest request)
    {
        VmExecutionStep step;

        scope.Enter(operation.Meter, operation);
        mediator?.EnterScope(operation.Baseline, operation.ObjectId);

        try
        {
            step = executor.Invoke(state, in request, operation.Token);
        }
        catch (System.OperationCanceledException)
        {
            return Finish(operation, VmInvocationResult.Cancellation(
                VmReason.Cancelled,
                operation.Baseline.WithOutcome(VmStage.Invocation, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host)));
        }
        catch (System.Exception)
        {
            return Finish(operation, VmInvocationResult.ProfileFault(
                VmReason.ProfileContractViolation, null,
                operation.Baseline.WithOutcome(VmStage.Invocation, VmOutcome.ProfileFault, VmReason.ProfileContractViolation, VmInitiator.Guest)));
        }
        finally
        {
            scope.Leave();
        }

        return Finish(operation, MapInvocation(operation, step));
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0005; IP=Low; Security=Medium; Resources=1; Fingerprint=832E40
    // Broiler-Human:        PENDING
    private VmInvocationResult MapInvocation(VmOperation operation, VmExecutionStep step)
    {
        // The frozen precedence order, which is one order for every stage: invalid state,
        // cancellation, unsupported profile, invalid artifact, resource exhaustion, host failure,
        // profile fault, suspension, normal. Cancellation ranks above exhaustion and both rank
        // above a profile fault, so a profile that also overran its declared poll bound is still
        // reported as cancelled or exhausted - reporting the poll-bound breach first would blame
        // the profile for a condition it did not cause and would drop the exhaustion dimension and
        // scope from the diagnostics entirely.
        // The token as well as the meter: a profile that never polled did not observe the
        // cancellation, but the operation was cancelled all the same, and the caller is owed that
        // answer rather than a complaint about the profile's polling.
        if (operation.Meter.CancellationObserved || operation.Token.IsCancellationRequested)
        {
            return VmInvocationResult.Cancellation(
                VmReason.Cancelled,
                operation.Baseline.WithOutcome(VmStage.Invocation, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host));
        }

        if (operation.Meter.ExhaustionObserved && step.Kind is not VmExecutionStepKind.Suspended)
        {
            var exhausted = VmMeter.ReasonFor(operation.Meter.FailedDimension);

            return VmInvocationResult.ResourceExhaustion(
                exhausted,
                operation.Baseline
                    .WithOutcome(VmStage.Invocation, VmOutcome.ResourceExhaustion, exhausted, VmInitiator.Core)
                    .WithExhaustion(operation.Meter.FailedDimension, operation.Meter.FailedScope));
        }

        if (operation.HostFailure is not VmReason.None)
        {
            // A host capability the profile did not convert. Ranked above a profile fault because
            // it is a host defect, and billing it to the guest would send a support case to the
            // wrong owner.
            return VmInvocationResult.HostFailure(
                operation.HostFailure,
                operation.Baseline
                    .WithOutcome(VmStage.Invocation, VmOutcome.HostFailure, operation.HostFailure, VmInitiator.Host)
                    .WithCapability(operation.HostFailureCapability, operation.HostFailureCapabilityVersion, default));
        }

        if (operation.Meter.PollBoundExceeded || operation.Meter.UnpolledWorkExceedsBound)
        {
            return VmInvocationResult.ProfileFault(
                VmReason.CancellationPollBoundExceeded, ValidatePayload(step.Payload),
                operation.Baseline.WithOutcome(VmStage.Invocation, VmOutcome.ProfileFault, VmReason.CancellationPollBoundExceeded, VmInitiator.Guest));
        }

        switch (step.Kind)
        {
            case VmExecutionStepKind.Suspended when step.Continuation is not null:
            {
                var origin = operation.ExternalSuspendRequested
                    ? VmSuspensionOrigin.External
                    : VmSuspensionOrigin.Guest;

                if (!operation.TryPark(origin, step.Continuation, ValidatePayload(step.Payload), out var suspension, out var parkFailure))
                {
                    return VmInvocationResult.InvalidState(
                        parkFailure,
                        VmRuntime.Invalid(operation.Baseline, VmStage.Invocation, parkFailure, VmObjectKind.Operation, VmAttemptedCall.Invoke));
                }

                lock (gate)
                {
                    currentState = VmInstanceState.Suspended;
                }

                return VmInvocationResult.Suspension(
                    origin is VmSuspensionOrigin.External ? null : suspension,
                    // An external suspension carries origin and identity only. What a paused
                    // profile exposes is the profile's own surface, and a host that paused an
                    // operation from outside did not ask the guest for a projection.
                    origin is VmSuspensionOrigin.External ? null : ValidatePayload(step.Payload),
                    origin is VmSuspensionOrigin.External ? VmReason.ExternallySuspended : VmReason.GuestSuspended,
                    operation.Baseline.WithOutcome(
                        VmStage.Invocation, VmOutcome.Suspension,
                        origin is VmSuspensionOrigin.External ? VmReason.ExternallySuspended : VmReason.GuestSuspended,
                        origin is VmSuspensionOrigin.External ? VmInitiator.Host : VmInitiator.Guest));
            }

            case VmExecutionStepKind.Faulted:
                return VmInvocationResult.ProfileFault(
                    VmReason.ProfileFaultUnspecified, ValidatePayload(step.Payload),
                    operation.Baseline.WithOutcome(VmStage.Invocation, VmOutcome.ProfileFault, VmReason.ProfileFaultUnspecified, VmInitiator.Guest));

            case VmExecutionStepKind.ContractViolation:
                return VmInvocationResult.ProfileFault(
                    step.Reason, null,
                    operation.Baseline.WithOutcome(VmStage.Invocation, VmOutcome.ProfileFault, step.Reason, VmInitiator.Guest));

            case VmExecutionStepKind.Completed:
                return VmInvocationResult.Normal(
                    ValidatePayload(step.Payload),
                    operation.Baseline.WithOutcome(VmStage.Invocation, VmOutcome.Normal, VmReason.NormalCompleted, VmInitiator.Caller));

            default:
                return VmInvocationResult.ProfileFault(
                    VmReason.ProfileContractViolation, null,
                    operation.Baseline.WithOutcome(VmStage.Invocation, VmOutcome.ProfileFault, VmReason.ProfileContractViolation, VmInitiator.Guest));
        }
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0005; IP=Low; Security=Medium; Resources=1; Fingerprint=5DE8C1
    // Broiler-Human:        PENDING
    private VmResumeResult MapResume(VmOperation operation, VmExecutionStep step)
    {
        // The same frozen precedence as the invocation stage; it is one order for every stage.
        if (operation.Meter.CancellationObserved || operation.Token.IsCancellationRequested)
        {
            return VmResumeResult.Cancellation(
                operation.Stage, VmReason.Cancelled,
                operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host));
        }

        if (operation.Meter.ExhaustionObserved && step.Kind is not VmExecutionStepKind.Suspended)
        {
            return VmResumeResult.ResourceExhaustion(
                operation.Stage, VmMeter.ReasonFor(operation.Meter.FailedDimension),
                operation.Baseline
                    .WithOutcome(VmStage.Resume, VmOutcome.ResourceExhaustion, VmMeter.ReasonFor(operation.Meter.FailedDimension), VmInitiator.Core)
                    .WithExhaustion(operation.Meter.FailedDimension, operation.Meter.FailedScope));
        }

        if (operation.HostFailure is not VmReason.None)
        {
            return VmResumeResult.HostFailure(
                operation.Stage, operation.HostFailure,
                operation.Baseline
                    .WithOutcome(VmStage.Resume, VmOutcome.HostFailure, operation.HostFailure, VmInitiator.Host)
                    .WithCapability(operation.HostFailureCapability, operation.HostFailureCapabilityVersion, default));
        }

        if (operation.Meter.PollBoundExceeded || operation.Meter.UnpolledWorkExceedsBound)
        {
            return VmResumeResult.ProfileFault(
                operation.Stage, VmReason.CancellationPollBoundExceeded, ValidatePayload(step.Payload),
                operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.ProfileFault, VmReason.CancellationPollBoundExceeded, VmInitiator.Guest));
        }

        switch (step.Kind)
        {
            case VmExecutionStepKind.Suspended when step.Continuation is not null:
            {
                var origin = operation.ExternalSuspendRequested
                    ? VmSuspensionOrigin.External
                    : VmSuspensionOrigin.Guest;

                if (!operation.TryPark(origin, step.Continuation, ValidatePayload(step.Payload), out var suspension, out var parkFailure))
                {
                    return VmResumeResult.InvalidState(
                        operation.Stage, parkFailure,
                        VmRuntime.Invalid(operation.Baseline, VmStage.Resume, parkFailure, VmObjectKind.Operation, VmAttemptedCall.Resume));
                }

                return VmResumeResult.Suspension(
                    operation.Stage,
                    origin is VmSuspensionOrigin.External ? null : suspension,
                    origin is VmSuspensionOrigin.External ? null : ValidatePayload(step.Payload),
                    origin is VmSuspensionOrigin.External ? VmReason.ExternallySuspended : VmReason.GuestSuspended,
                    operation.Baseline.WithOutcome(
                        VmStage.Resume, VmOutcome.Suspension,
                        origin is VmSuspensionOrigin.External ? VmReason.ExternallySuspended : VmReason.GuestSuspended,
                        origin is VmSuspensionOrigin.External ? VmInitiator.Host : VmInitiator.Guest));
            }

            case VmExecutionStepKind.Faulted:
                return VmResumeResult.ProfileFault(
                    operation.Stage, VmReason.ProfileFaultUnspecified, ValidatePayload(step.Payload),
                    operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.ProfileFault, VmReason.ProfileFaultUnspecified, VmInitiator.Guest));

            case VmExecutionStepKind.ContractViolation:
                return VmResumeResult.ProfileFault(
                    operation.Stage, step.Reason, null,
                    operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.ProfileFault, step.Reason, VmInitiator.Guest));

            case VmExecutionStepKind.Suspended:
                // Suspended with no continuation. There is nothing to resume from, so reporting
                // success would hand the caller a completed operation whose profile state was
                // abandoned mid-step.
                return VmResumeResult.ProfileFault(
                    operation.Stage, VmReason.ProfileContractViolation, null,
                    operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.ProfileFault, VmReason.ProfileContractViolation, VmInitiator.Guest));

            default:
                return VmResumeResult.Normal(
                    operation.Stage,
                    operation.Stage is VmStage.Instantiation ? this : null,
                    ValidatePayload(step.Payload),
                    operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.Normal, VmReason.NormalCompleted, VmInitiator.Caller));
        }
    }

    /// <summary>
    /// Checks that a payload belongs to the profile that produced the result and lies inside its
    /// declared kind range.
    /// </summary>
    /// <remarks>
    /// This is the whole of what the core does with a payload. It never calls a member on it, never
    /// pattern-matches its concrete type, and never stores, clones, pools or serialises it - a
    /// foreign payload is dropped rather than passed on, so a profile cannot smuggle one profile's
    /// value out through another profile's result.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0005; IP=Low; Security=Medium; Resources=0; Fingerprint=833B3C
    // Broiler-Human:        PENDING
    private IVmProfilePayload? ValidatePayload(IVmProfilePayload? payload)
    {
        if (payload is null)
        {
            return null;
        }

        var identity = payload.Identity;

        if (!identity.ProfileId.Equals(profile.ProfileId) ||
            !profile.PayloadKindIdRange.Contains(identity.PayloadKindId))
        {
            return null;
        }

        return payload;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=124DB5
    // Broiler-Human:        PENDING
    private VmInvocationResult Finish(VmOperation operation, VmInvocationResult result)
    {
        Settle(operation, result.Outcome);
        return result;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=FDD7DE
    // Broiler-Human:        PENDING
    private VmResumeResult Finish(VmOperation operation, VmResumeResult result)
    {
        Settle(operation, result.Outcome);
        return result;
    }

    /// <summary>
    /// Applies the mandatory outcome-to-instance-state mapping.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The mapping is frozen and admits no implementation freedom. Exhaustion, cancellation and
    /// host failure all move the instance to <see cref="VmInstanceState.Faulted"/>, <em>always</em>:
    /// the profile stack was abandoned at an arbitrary point, so it has no owner-visible state, and
    /// declaring it usable would make every later isolation and use-after-dispose claim
    /// meaningless. Only a language fault consults the profile, because recoverability is a
    /// language property - a trap and a caught exception differ - and a core-wide answer would
    /// silently pick one language's.
    /// </para>
    /// <para>
    /// An invalid state leaves the instance unchanged, because the call never entered the profile.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=Medium; Resources=0; Fingerprint=4158B6
    // Broiler-Falsified-If: Suspension reaches the switch, or exhaustion, cancellation or a host failure leaves it unfaulted
    // Broiler-Human:        PENDING
    private void Settle(VmOperation operation, VmOutcome outcome)
    {
        if (outcome is VmOutcome.Suspension)
        {
            return;
        }

        operation.Complete();

        lock (gate)
        {
            if (currentState is VmInstanceState.Disposed or VmInstanceState.Disposing)
            {
                return;
            }

            active = null;

            switch (outcome)
            {
                case VmOutcome.InvalidState or VmOutcome.UnsupportedProfile:
                    // Unchanged. The call never reached the profile, so nothing about the instance
                    // has become untrue.
                    return;

                case VmOutcome.ResourceExhaustion or VmOutcome.Cancellation or VmOutcome.HostFailure:
                    currentState = VmInstanceState.Faulted;
                    return;

                case VmOutcome.ProfileFault:
                    currentState = profile.FaultRecovery is VmFaultRecovery.InstanceFatal
                        ? VmInstanceState.Faulted
                        : VmInstanceState.Live;
                    return;

                default:
                    currentState = VmInstanceState.Live;
                    return;
            }
        }
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=Medium; Resources=0; Fingerprint=D4AF06
    // Broiler-Falsified-If: a Faulted instance admits anything beyond disposal and a diagnostics read
    // Broiler-Human:        PENDING
    private bool TryAdmit(out VmReason failure)
    {
        switch (currentState)
        {
            case VmInstanceState.Disposed:
                failure = VmReason.ObjectDisposed;
                return false;

            case VmInstanceState.Disposing:
                failure = VmReason.ObjectDisposing;
                return false;

            case VmInstanceState.Faulted:
                failure = VmReason.TerminalFault;
                return false;

            case VmInstanceState.Executing:
                failure = VmReason.ReentrancyRefused;
                return false;

            case VmInstanceState.Suspended:
                failure = VmReason.WrongState;
                return false;

            default:
                failure = VmReason.None;
                return true;
        }
    }
}
