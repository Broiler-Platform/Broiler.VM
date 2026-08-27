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

    private VmInstanceState currentState = VmInstanceState.Live;
    private VmOperation? active;

    internal VmInstanceImplementation(
        VmRuntime runtime,
        VmProfileDescriptor profile,
        IVmProfileExecutor executor,
        IVmInstanceState state,
        VmBudgetLevel instanceLevel,
        VmDiagnostics baseline,
        VmExecutionScope scope,
        VmArtifactLoadMediator? mediator)
    {
        this.runtime = runtime;
        this.profile = profile;
        this.executor = executor;
        this.state = state;
        this.instanceLevel = instanceLevel;
        this.baseline = baseline;
        this.scope = scope;
        this.mediator = mediator;
        Identity = VmObjectId.Mint();
    }

    internal VmObjectId Identity { get; }

    /// <inheritdoc/>
    public override VmObjectId ObjectId => Identity;

    /// <inheritdoc/>
    public override VmProfileId ProfileId => profile.ProfileId;

    /// <inheritdoc/>
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
    public override VmInvocationResult Invoke(
        in VmInvocationRequest request,
        System.Threading.CancellationToken cancellationToken) =>
        Invoke(in request, cancellationToken, out _);

    /// <inheritdoc/>
    public override VmInvocationResult Invoke(
        in VmInvocationRequest request,
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

                var linked = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                var invocationLevel = new VmBudgetLevel(
                    VmBudgetScope.Invocation, instanceLevel.CeilingsCopy());

                var meter = new VmMeter(
                    runtime.Gate, invocationLevel, instanceLevel, runtime.RuntimeLevel, runtime.Parent,
                    profile.MaxUnchargedWork, linked.Token);

                operation = new VmOperation(
                    runtime, this, profile, VmOperationKind.Invoke, meter, linked,
                    VmStage.Invocation, identified);

                active = operation;
                currentState = VmInstanceState.Executing;
            }

            controlHandle = new VmOperationControlHandleImplementation(operation);

            return RunInvocation(operation, in request);
        }
        finally
        {
            runtime.EndCall();
        }
    }

    /// <inheritdoc/>
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
    public override VmControlResult Dispose()
    {
        VmOperation? operation;

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

        operation?.Abandon(VmReason.Cancelled);

        lock (gate)
        {
            currentState = VmInstanceState.Disposed;
        }

        // Live bytes the profile reported retained are given back on dispose, so repeated
        // load-run-evict cycles reach a plateau instead of climbing.
        runtime.ForgetInstance(this);
        return VmControlResult.Accepted;
    }

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
        }

        VmExecutionStep step;

        // The scope answers only inside the dynamic extent of this step. A profile that stashed its
        // meter or its mediator during an earlier step and uses it now is refused rather than
        // charged against whatever operation happens to be running.
        scope.Enter(operation.Meter);
        mediator?.EnterScope(operation.Baseline);

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

    private VmInvocationResult RunInvocation(VmOperation operation, in VmInvocationRequest request)
    {
        VmExecutionStep step;

        scope.Enter(operation.Meter);
        mediator?.EnterScope(operation.Baseline);

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

    private VmInvocationResult MapInvocation(VmOperation operation, VmExecutionStep step)
    {
        // Precedence, outermost cause first. A profile that exceeded its declared poll bound broke
        // its own promise about cancellation latency, so that is reported before anything else. A
        // profile that stopped because Poll said stop obeyed cancellation. A profile that stopped
        // because a charge was refused ran out of budget. Only what is left is the profile's own
        // answer.
        if (operation.Meter.PollBoundExceeded || operation.Meter.UnpolledWorkExceedsBound)
        {
            return VmInvocationResult.ProfileFault(
                VmReason.CancellationPollBoundExceeded, step.Payload,
                operation.Baseline.WithOutcome(VmStage.Invocation, VmOutcome.ProfileFault, VmReason.CancellationPollBoundExceeded, VmInitiator.Guest));
        }

        if (operation.Meter.CancellationObserved)
        {
            return VmInvocationResult.Cancellation(
                VmReason.Cancelled,
                operation.Baseline.WithOutcome(VmStage.Invocation, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host));
        }

        if (operation.Meter.ExhaustionObserved && step.Kind is not VmExecutionStepKind.Suspended)
        {
            return VmInvocationResult.ResourceExhaustion(
                VmReason.AllowanceExhausted,
                operation.Baseline
                    .WithOutcome(VmStage.Invocation, VmOutcome.ResourceExhaustion, VmReason.AllowanceExhausted, VmInitiator.Core)
                    .WithExhaustion(operation.Meter.FailedDimension, operation.Meter.FailedScope));
        }

        switch (step.Kind)
        {
            case VmExecutionStepKind.Suspended when step.Continuation is not null:
            {
                var origin = operation.ExternalSuspendRequested
                    ? VmSuspensionOrigin.External
                    : VmSuspensionOrigin.Guest;

                if (!operation.TryPark(origin, step.Continuation, step.Payload, out var suspension, out var parkFailure))
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
                    step.Payload,
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

    private VmResumeResult MapResume(VmOperation operation, VmExecutionStep step)
    {
        if (operation.Meter.PollBoundExceeded || operation.Meter.UnpolledWorkExceedsBound)
        {
            return VmResumeResult.ProfileFault(
                operation.Stage, VmReason.CancellationPollBoundExceeded, step.Payload,
                operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.ProfileFault, VmReason.CancellationPollBoundExceeded, VmInitiator.Guest));
        }

        if (operation.Meter.CancellationObserved)
        {
            return VmResumeResult.Cancellation(
                operation.Stage, VmReason.Cancelled,
                operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host));
        }

        if (operation.Meter.ExhaustionObserved && step.Kind is not VmExecutionStepKind.Suspended)
        {
            return VmResumeResult.ResourceExhaustion(
                operation.Stage, VmReason.AllowanceExhausted,
                operation.Baseline
                    .WithOutcome(VmStage.Resume, VmOutcome.ResourceExhaustion, VmReason.AllowanceExhausted, VmInitiator.Core)
                    .WithExhaustion(operation.Meter.FailedDimension, operation.Meter.FailedScope));
        }

        switch (step.Kind)
        {
            case VmExecutionStepKind.Suspended when step.Continuation is not null:
            {
                var origin = operation.ExternalSuspendRequested
                    ? VmSuspensionOrigin.External
                    : VmSuspensionOrigin.Guest;

                if (!operation.TryPark(origin, step.Continuation, step.Payload, out var suspension, out var parkFailure))
                {
                    return VmResumeResult.InvalidState(
                        operation.Stage, parkFailure,
                        VmRuntime.Invalid(operation.Baseline, VmStage.Resume, parkFailure, VmObjectKind.Operation, VmAttemptedCall.Resume));
                }

                return VmResumeResult.Suspension(
                    operation.Stage,
                    origin is VmSuspensionOrigin.External ? null : suspension,
                    step.Payload,
                    origin is VmSuspensionOrigin.External ? VmReason.ExternallySuspended : VmReason.GuestSuspended,
                    operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.Suspension, VmReason.GuestSuspended, VmInitiator.Guest));
            }

            case VmExecutionStepKind.Faulted:
                return VmResumeResult.ProfileFault(
                    operation.Stage, VmReason.ProfileFaultUnspecified, ValidatePayload(step.Payload),
                    operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.ProfileFault, VmReason.ProfileFaultUnspecified, VmInitiator.Guest));

            case VmExecutionStepKind.ContractViolation:
                return VmResumeResult.ProfileFault(
                    operation.Stage, step.Reason, null,
                    operation.Baseline.WithOutcome(VmStage.Resume, VmOutcome.ProfileFault, step.Reason, VmInitiator.Guest));

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

    private VmInvocationResult Finish(VmOperation operation, VmInvocationResult result)
    {
        Settle(operation, result.Outcome);
        return result;
    }

    private VmResumeResult Finish(VmOperation operation, VmResumeResult result)
    {
        Settle(operation, result.Outcome);
        return result;
    }

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

            // The outcome-to-instance-state mapping. Whether a language fault kills the instance is
            // the profile's declaration, and it is the only thing that changes this mapping.
            currentState = outcome is VmOutcome.ProfileFault && profile.FaultRecovery is VmFaultRecovery.InstanceFatal
                ? VmInstanceState.Faulted
                : VmInstanceState.Live;
        }
    }

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
