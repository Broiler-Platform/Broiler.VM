namespace Broiler.VM;

/// <summary>
/// One unit of work inside a runtime: a verification, an instantiation, or an invocation, together
/// with everything that may happen to it while it runs.
/// </summary>
/// <remarks>
/// <para>
/// Resume is not a fourth kind of operation. At resume the operation that suspended continues,
/// keeping its identity, its budget remainder and its nested-load counters - a new operation would
/// mean a fresh allowance, which is exactly what a guest could then loop on.
/// </para>
/// <para>
/// The two latches - cancellation requested and external suspension requested - are monotonic and
/// are never cleared. A request that could be withdrawn would make "was it cancelled?" a question
/// with two answers depending on when it was asked.
/// </para>
/// </remarks>
internal sealed class VmOperation
{
    private readonly object gate = new();
    private readonly VmRuntime runtime;
    private readonly VmInstanceImplementation? instance;
    private readonly VmProfileDescriptor profile;
    private readonly VmMeter meter;
    private readonly System.Threading.CancellationTokenSource cancellation;
    private readonly System.Diagnostics.Stopwatch parkedFor = new();

    private IVmProfileContinuation? continuation;
    private VmSuspension? pending;
    private VmOperationState state = VmOperationState.Running;
    private VmSuspensionOrigin origin;
    private bool hasOrigin;
    private bool cancellationRequested;
    private bool externalSuspendRequested;

    internal VmOperation(
        VmRuntime runtime,
        VmInstanceImplementation? instance,
        VmProfileDescriptor profile,
        VmOperationKind kind,
        VmMeter meter,
        System.Threading.CancellationTokenSource cancellation,
        VmStage stage,
        VmDiagnostics baseline)
    {
        this.runtime = runtime;
        this.instance = instance;
        this.profile = profile;
        this.meter = meter;
        this.cancellation = cancellation;
        Kind = kind;
        Stage = stage;
        Baseline = baseline;
        ObjectId = VmObjectId.Mint();
        Key = unchecked((ulong)ObjectId.GetHashCode());
    }

    internal VmObjectId ObjectId { get; }

    internal ulong Key { get; }

    internal VmOperationKind Kind { get; }

    internal VmStage Stage { get; }

    internal VmDiagnostics Baseline { get; private set; }

    /// <summary>
    /// The host failure a capability produced that the profile did not convert, or
    /// <see cref="VmReason.None"/>.
    /// </summary>
    /// <remarks>
    /// A capability that declares <see cref="VmExceptionTranslation.TerminateOperation"/> means
    /// exactly that: the operation ends with the host failure, and the profile's own answer is
    /// discarded. Reporting it as a profile fault instead would bill a host defect to the guest and
    /// send a support case to the wrong owner. Where a capability declares an observable fault the
    /// profile is expected to convert it, and a converted outcome is a profile fault - that is a
    /// control-flow fact rather than a precedence question.
    /// </remarks>
    internal VmReason HostFailure { get; private set; } = VmReason.None;

    /// <summary>The capability the unconverted host failure came from.</summary>
    internal VmCapabilityId HostFailureCapability { get; private set; }

    /// <summary>Its declared version.</summary>
    internal int HostFailureCapabilityVersion { get; private set; }

    /// <summary>Latches an unconverted host failure. The first one wins; a later one is its echo.</summary>
    internal void LatchHostFailure(VmReason reason, VmCapabilityId capability, int version)
    {
        lock (gate)
        {
            if (HostFailure is not VmReason.None)
            {
                return;
            }

            HostFailure = reason;
            HostFailureCapability = capability;
            HostFailureCapabilityVersion = version;
        }
    }

    internal VmMeter Meter => meter;

    internal System.Threading.CancellationToken Token => cancellation.Token;

    internal VmOperationState State
    {
        get
        {
            lock (gate)
            {
                return state;
            }
        }
    }

    internal VmOperationStateSnapshot Snapshot()
    {
        lock (gate)
        {
            return new VmOperationStateSnapshot(
                ObjectId, Kind, state, origin, hasOrigin, cancellationRequested, externalSuspendRequested);
        }
    }

    /// <summary>Requests cancellation. Monotonic and idempotent.</summary>
    internal VmControlResult RequestCancel()
    {
        lock (gate)
        {
            if (state is VmOperationState.Completed or VmOperationState.Disposed)
            {
                return VmControlResult.InvalidState(VmReason.AlreadyCompleted);
            }

            if (cancellationRequested)
            {
                return VmControlResult.NoOp;
            }

            cancellationRequested = true;
        }

        cancellation.Cancel();
        return VmControlResult.Accepted;
    }

    /// <summary>
    /// Requests an external suspension, subject to the double gate: the profile must declare it and
    /// the runtime must enable it.
    /// </summary>
    internal VmControlResult RequestSuspend()
    {
        if (profile.ExternalSuspension is not VmDeclaration.Declared)
        {
            return VmControlResult.Unsupported(VmReason.ExternalSuspensionNotDeclared);
        }

        if (runtime.ExternalSuspension is not VmExternalSuspensionMode.Enabled)
        {
            return VmControlResult.Unsupported(VmReason.ExternalSuspensionNotEnabled);
        }

        lock (gate)
        {
            if (state is VmOperationState.Completed or VmOperationState.Disposed)
            {
                return VmControlResult.InvalidState(VmReason.AlreadyCompleted);
            }

            if (externalSuspendRequested)
            {
                return VmControlResult.NoOp;
            }

            externalSuspendRequested = true;
            return VmControlResult.Accepted;
        }
    }

    internal bool ExternalSuspendRequested
    {
        get
        {
            lock (gate)
            {
                return externalSuspendRequested;
            }
        }
    }

    /// <summary>Parks the operation, minting the single-use resumption object.</summary>
    internal bool TryPark(
        VmSuspensionOrigin suspensionOrigin,
        IVmProfileContinuation profileContinuation,
        IVmProfilePayload? projection,
        out VmSuspension suspension,
        out VmReason failure)
    {
        suspension = null!;

        lock (gate)
        {
            if (state is not VmOperationState.Running)
            {
                failure = VmReason.WrongState;
                return false;
            }
        }

        if (!runtime.TryPark(this, out failure))
        {
            return false;
        }

        lock (gate)
        {
            continuation = profileContinuation;
            origin = suspensionOrigin;
            hasOrigin = true;
            state = suspensionOrigin is VmSuspensionOrigin.External
                ? VmOperationState.SuspendedByHost
                : VmOperationState.SuspendedByGuest;

            suspension = VmSuspension.Create(
                VmObjectId.Mint(), ObjectId, runtime.ObjectId, suspensionOrigin, Stage, projection);

            // An external suspension is delivered once, through the control handle, and never on
            // the caller's result: the caller did not ask for it and has no business resuming it.
            pending = suspensionOrigin is VmSuspensionOrigin.External ? suspension : null;
        }

        meter.PauseWallClock();
        parkedFor.Restart();
        failure = VmReason.None;
        return true;
    }

    /// <summary>Hands the resumption object to whoever holds the control handle, once.</summary>
    internal VmControlResult TryTakeSuspension(out VmSuspension suspension)
    {
        lock (gate)
        {
            if (pending is null)
            {
                suspension = null!;
                return state is VmOperationState.SuspendedByHost
                    ? VmControlResult.InvalidState(VmReason.ResumeTokenConsumed)
                    : VmControlResult.InvalidState(VmReason.WrongState);
            }

            suspension = pending;
            pending = null;
            return VmControlResult.Accepted;
        }
    }

    internal bool HasUntakenExternalSuspension
    {
        get
        {
            lock (gate)
            {
                return pending is not null;
            }
        }
    }

    internal bool HasOutstayed(System.TimeSpan residency)
    {
        lock (gate)
        {
            if (state is not (VmOperationState.SuspendedByGuest or VmOperationState.SuspendedByHost))
            {
                return false;
            }
        }

        return parkedFor.Elapsed > residency;
    }

    /// <summary>Resumes the parked operation through the profile's own continuation.</summary>
    internal VmResumeResult Resume(VmDiagnostics baseline)
    {
        IVmProfileContinuation? resumed;

        lock (gate)
        {
            if (state is not (VmOperationState.SuspendedByGuest or VmOperationState.SuspendedByHost))
            {
                return VmResumeResult.InvalidState(
                    Stage,
                    VmReason.WrongState,
                    baseline
                        .WithOutcome(VmStage.Resume, VmOutcome.InvalidState, VmReason.WrongState, VmInitiator.Caller)
                        .WithObject(VmObjectKind.Operation, (int)state, VmAttemptedCall.Resume));
            }

            // The cancellation latch is monotonic, so an operation cancelled while parked stays
            // cancelled. Resuming it would re-enter profile state that has already been abandoned.
            if (cancellationRequested)
            {
                return VmResumeResult.Cancellation(
                    Stage,
                    VmReason.Cancelled,
                    baseline.WithOutcome(VmStage.Resume, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host));
            }

            resumed = continuation;
            continuation = null;
            pending = null;
            state = VmOperationState.Running;
        }

        parkedFor.Stop();
        meter.ResumeWallClock();
        Baseline = baseline.WithOperation(ObjectId, default, 0);

        if (resumed is null || instance is null)
        {
            return VmResumeResult.InvalidState(
                Stage,
                VmReason.WrongState,
                Baseline
                    .WithOutcome(VmStage.Resume, VmOutcome.InvalidState, VmReason.WrongState, VmInitiator.Caller)
                    .WithObject(VmObjectKind.Operation, (int)VmOperationState.Running, VmAttemptedCall.Resume));
        }

        return instance.ResumeOperation(this, resumed);
    }

    /// <summary>
    /// Latches an expired suspension as cancelled. A parked operation that outstayed its residency
    /// is not left parked: the bound exists so that disposal is never blocked indefinitely.
    /// </summary>
    internal void Expire()
    {
        Abandon(VmReason.SuspendedResidencyExpired);
    }

    /// <summary>Latches an abandoned suspension as cancelled and unwinds the profile.</summary>
    internal void Abandon() => Abandon(VmReason.ExternalSuspensionAbandoned);

    internal void Abandon(VmReason reason)
    {
        IVmProfileContinuation? dropped;

        lock (gate)
        {
            if (state is VmOperationState.Completed or VmOperationState.Disposed or VmOperationState.Orphaned)
            {
                return;
            }

            dropped = continuation;
            continuation = null;
            pending = null;
            state = VmOperationState.Orphaned;
            cancellationRequested = true;
            AbandonReason = reason;
        }

        cancellation.Cancel();

        // Unpark, always. An abandoned operation is terminal, and leaving it in the runtime's
        // suspended set would consume a live-suspended slot for the life of the runtime while
        // pinning its meter and its instance - a leak invisible to any test that never disposes
        // a suspended instance.
        runtime.Unpark(this);

        if (dropped is not null)
        {
            instance?.Unwind(dropped);
        }
    }

    internal VmReason AbandonReason { get; private set; } = VmReason.None;

    internal void Complete()
    {
        lock (gate)
        {
            if (state is VmOperationState.Disposed)
            {
                return;
            }

            state = VmOperationState.Completed;
        }

        runtime.Unpark(this);
    }
}

/// <summary>The control surface handed to whoever started an operation.</summary>
/// <remarks>
/// Authority is possession. The core authenticates nobody and defines no principal, permission,
/// claim or policy: whoever holds this handle may control the operation, and the caller that
/// received it decides who else gets it.
/// </remarks>
internal sealed class VmOperationControlHandleImplementation : VmOperationControlHandle
{
    private readonly VmOperation operation;
    private int disposed;

    internal VmOperationControlHandleImplementation(VmOperation operation) => this.operation = operation;

    /// <inheritdoc/>
    public override VmControlResult RequestSuspend() =>
        IsDisposed ? VmControlResult.InvalidState(VmReason.ObjectDisposed) : operation.RequestSuspend();

    /// <inheritdoc/>
    public override VmControlResult RequestCancel() =>
        IsDisposed ? VmControlResult.InvalidState(VmReason.ObjectDisposed) : operation.RequestCancel();

    /// <inheritdoc/>
    public override VmOperationStateSnapshot QueryState() => operation.Snapshot();

    /// <inheritdoc/>
    public override VmControlResult TryTakeSuspension(out VmSuspension suspension)
    {
        if (IsDisposed)
        {
            suspension = null!;
            return VmControlResult.InvalidState(VmReason.ObjectDisposed);
        }

        return operation.TryTakeSuspension(out suspension);
    }

    /// <inheritdoc/>
    public override VmControlResult Dispose()
    {
        if (System.Threading.Interlocked.Exchange(ref disposed, 1) != 0)
        {
            return VmControlResult.NoOp;
        }

        // A handle disposed while still holding an untaken external suspension latches the
        // operation cancelled. Otherwise a debugger that paused an operation and then went away
        // would park it until the residency bound expired, holding frames and an execution slot
        // for a resumption that is never coming.
        if (operation.HasUntakenExternalSuspension)
        {
            operation.Abandon(VmReason.ExternalSuspensionAbandoned);
            return VmControlResult.Accepted;
        }

        return VmControlResult.Accepted;
    }

    private bool IsDisposed => System.Threading.Volatile.Read(ref disposed) != 0;
}
