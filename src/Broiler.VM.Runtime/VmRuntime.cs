namespace Broiler.VM;

/// <summary>
/// The execution domain and the unit of isolation: one catalog, one set of ceilings, one bound
/// capability table, and the operations that run under them.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Verify"/> is the one verification entry point in the whole core. Its parameter set is
/// exactly the descriptor, the complete byte range and a cancellation token, and nothing else, so an
/// incremental or streaming form cannot arrive as a quiet widening of a signature - adding one is a
/// numbered amendment. It takes no execution slot, because verifying without running is an ordinary
/// use of the surface rather than a second, tool-shaped API.
/// </para>
/// <para>
/// No member returns a task or any other awaitable, and no type here implements a completion
/// interface. Contract version 1 admits no asynchronous runtime creation, verification or
/// invocation: a profile that must wait suspends, and the host resumes it.
/// </para>
/// </remarks>
public sealed partial class VmRuntime : System.IDisposable
{
    private readonly object gate = new();
    private readonly VmCatalog catalog;
    private readonly VmRuntimeCreationOptions options;
    private readonly VmBudgetLevel runtimeLevel;
    private readonly VmAggregateBudget? parent;
    private readonly VmGuestLoadBounds guestLoadBounds;
    private readonly System.Collections.Generic.Dictionary<string, VmProfileRuntimeState> profiles = new(System.StringComparer.Ordinal);
    private readonly System.Collections.Generic.List<VmInstanceImplementation> instances = new();
    private readonly System.Collections.Generic.Dictionary<ulong, VmOperation> suspended = new();

    private VmRuntimeState state = VmRuntimeState.Ready;
    private int inCapabilityDepth;
    private int entryDepth;
    private int activeVerifications;

    private VmRuntime(
        VmObjectId objectId,
        VmCatalog catalog,
        VmRuntimeCreationOptions options,
        VmBudgetLevel runtimeLevel,
        VmAggregateBudget? parent,
        VmGuestLoadBounds guestLoadBounds)
    {
        ObjectId = objectId;
        this.catalog = catalog;
        this.options = options;
        this.runtimeLevel = runtimeLevel;
        this.parent = parent;
        this.guestLoadBounds = guestLoadBounds;
    }

    /// <summary>This runtime's identity.</summary>
    public VmObjectId ObjectId { get; }

    /// <summary>Its current state.</summary>
    public VmRuntimeState State
    {
        get
        {
            lock (gate)
            {
                return state;
            }
        }
    }

    /// <summary>The catalog this runtime was created over.</summary>
    public VmCatalog Catalog => catalog;

    /// <summary>Whether this runtime enables external suspension.</summary>
    public VmExternalSuspensionMode ExternalSuspension => options.ExternalSuspension;

    /// <summary>
    /// Creates a runtime. It never throws: every failure a host or a composition can cause is a
    /// returned result.
    /// </summary>
    public static VmRuntimeCreationResult Create(VmCatalog catalog, VmRuntimeCreationOptions options)
    {
        if (catalog is null)
        {
            throw new System.ArgumentNullException(nameof(catalog));
        }

        if (options is null)
        {
            throw new System.ArgumentNullException(nameof(options));
        }

        var objectId = VmObjectId.Mint();

        var baseline = VmDiagnostics.Create(
            VmStage.RuntimeCreation, VmOutcome.None, VmReason.None, objectId,
            VmInitiator.Caller, VmAttemptedCall.None);

        if (options.MaxSuspendedResidency <= System.TimeSpan.Zero ||
            options.MaxSuspendedResidency == System.Threading.Timeout.InfiniteTimeSpan)
        {
            return VmRuntimeCreationResult.HostFailure(
                VmReason.SuspendedResidencyUnbounded,
                baseline.WithOutcome(VmStage.RuntimeCreation, VmOutcome.HostFailure, VmReason.SuspendedResidencyUnbounded, VmInitiator.Caller));
        }

        if (options.MaxLiveSuspendedOperations <= 0)
        {
            return VmRuntimeCreationResult.HostFailure(
                VmReason.SuspendedOperationLimitUnbounded,
                baseline.WithOutcome(VmStage.RuntimeCreation, VmOutcome.HostFailure, VmReason.SuspendedOperationLimitUnbounded, VmInitiator.Caller));
        }

        if (!VmCeilingResolution.TryResolve(catalog, options, out var ceilings, out var failure))
        {
            return VmRuntimeCreationResult.HostFailure(
                failure,
                baseline.WithOutcome(VmStage.RuntimeCreation, VmOutcome.HostFailure, failure, VmInitiator.Caller));
        }

        if (!TryResolveGuestLoadBounds(catalog, options, out var guestBounds, out var boundsFailure))
        {
            return VmRuntimeCreationResult.HostFailure(
                boundsFailure,
                baseline.WithOutcome(VmStage.RuntimeCreation, VmOutcome.HostFailure, boundsFailure, VmInitiator.Caller));
        }

        // The parent is asked last, after every composition-shaped defect has been ruled out, so a
        // host debugging its wiring does not have to spend parent allowance to see the next error.
        if (options.AggregateBudget is not null && !options.AggregateBudget.TryAdmitRuntime(out var admission))
        {
            var outcome = admission is VmReason.AggregateBudgetDisposed
                ? VmOutcome.InvalidState
                : VmOutcome.ResourceExhaustion;

            var diagnostics = baseline
                .WithOutcome(VmStage.RuntimeCreation, outcome, admission, VmInitiator.Caller)
                .WithExhaustion(VmBudgetDimension.LiveRuntimes, VmBudgetScope.Aggregate);

            return outcome is VmOutcome.InvalidState
                ? VmRuntimeCreationResult.InvalidState(admission, diagnostics)
                : VmRuntimeCreationResult.ResourceExhaustion(admission, diagnostics);
        }

        var runtime = new VmRuntime(
            objectId,
            catalog,
            options,
            new VmBudgetLevel(VmBudgetScope.Runtime, ceilings),
            options.AggregateBudget,
            guestBounds);

        if (!runtime.TryBindCapabilities(out var bindingFailure, out var bindingCapability))
        {
            options.AggregateBudget?.ReleaseRuntime();

            return VmRuntimeCreationResult.HostFailure(
                bindingFailure,
                baseline
                    .WithOutcome(VmStage.RuntimeCreation, VmOutcome.HostFailure, bindingFailure, VmInitiator.Caller)
                    .WithCapability(bindingCapability, 0, default));
        }

        return VmRuntimeCreationResult.Normal(
            runtime,
            baseline.WithOutcome(VmStage.RuntimeCreation, VmOutcome.Normal, VmReason.NormalCompleted, VmInitiator.Caller));
    }

    /// <summary>
    /// Verifies caller-owned bytes into an immutable, profile-bound handle.
    /// </summary>
    /// <remarks>
    /// The handle owns a snapshot or a fully decoded form of the payload, so mutating, disposing or
    /// concurrently overwriting the caller's buffer afterwards cannot change what was verified.
    /// </remarks>
    public VmVerificationResult Verify(
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        System.Threading.CancellationToken cancellationToken)
    {
        var baseline = VmDiagnostics
            .Create(VmStage.Verification, VmOutcome.None, VmReason.None, ObjectId, VmInitiator.Caller, VmAttemptedCall.Verify)
            .WithArtifact(default, (ulong)payload.Length, descriptor.CallerIdentity);

        if (!TryBeginCall(out var stateFailure))
        {
            return VmVerificationResult.InvalidState(
                stateFailure,
                Invalid(baseline, VmStage.Verification, stateFailure, VmObjectKind.Runtime, VmAttemptedCall.Verify));
        }

        try
        {
            return VerifyCore(in descriptor, payload, cancellationToken, baseline, VmArtifactOrigin.Caller, null);
        }
        finally
        {
            EndCall();
        }
    }

    /// <summary>Instantiates a verified artifact into profile-owned mutable state.</summary>
    public VmInstantiationResult Instantiate(
        VmVerifiedArtifact artifact,
        System.Threading.CancellationToken cancellationToken)
    {
        var baseline = VmDiagnostics.Create(
            VmStage.Instantiation, VmOutcome.None, VmReason.None, ObjectId,
            VmInitiator.Caller, VmAttemptedCall.Instantiate);

        if (artifact is null)
        {
            throw new System.ArgumentNullException(nameof(artifact));
        }

        if (!TryBeginCall(out var stateFailure))
        {
            return VmInstantiationResult.InvalidState(
                stateFailure,
                Invalid(baseline, VmStage.Instantiation, stateFailure, VmObjectKind.Runtime, VmAttemptedCall.Instantiate));
        }

        try
        {
            return VmInstantiation.Run(this, artifact, cancellationToken, baseline);
        }
        finally
        {
            EndCall();
        }
    }

    /// <summary>
    /// Resumes a suspended operation. The single resume entry point: there is no second path, so
    /// there is no second admission check and no race between two of them.
    /// </summary>
    public VmResumeResult Resume(VmSuspension suspension)
    {
        if (suspension is null)
        {
            throw new System.ArgumentNullException(nameof(suspension));
        }

        var baseline = VmDiagnostics.Create(
            VmStage.Resume, VmOutcome.None, VmReason.None, ObjectId, VmInitiator.Caller, VmAttemptedCall.Resume);

        if (!TryBeginCall(out var stateFailure))
        {
            return VmResumeResult.InvalidState(
                suspension.SuspendedStage,
                stateFailure,
                Invalid(baseline, VmStage.Resume, stateFailure, VmObjectKind.Runtime, VmAttemptedCall.Resume));
        }

        try
        {
            if (!suspension.RuntimeId.Equals(ObjectId))
            {
                return VmResumeResult.InvalidState(
                    suspension.SuspendedStage,
                    VmReason.ForeignHandle,
                    Invalid(baseline, VmStage.Resume, VmReason.ForeignHandle, VmObjectKind.Suspension, VmAttemptedCall.Resume));
            }

            VmOperation? operation;

            lock (gate)
            {
                // Keyed by the operation, not by the suspension object. One operation may park
                // more than once over its life, and it is the operation that is resumed.
                if (!suspended.TryGetValue(OperationKey(suspension.OperationId), out operation))
                {
                    operation = null;
                }
            }

            if (operation is null)
            {
                return VmResumeResult.InvalidState(
                    suspension.SuspendedStage,
                    VmReason.ResumeTokenConsumed,
                    Invalid(baseline, VmStage.Resume, VmReason.ResumeTokenConsumed, VmObjectKind.Suspension, VmAttemptedCall.Resume));
            }

            if (!suspension.TryConsume())
            {
                return VmResumeResult.InvalidState(
                    suspension.SuspendedStage,
                    VmReason.ResumeTokenConsumed,
                    Invalid(baseline, VmStage.Resume, VmReason.ResumeTokenConsumed, VmObjectKind.Suspension, VmAttemptedCall.Resume));
            }

            lock (gate)
            {
                suspended.Remove(OperationKey(suspension.OperationId));
            }

            return operation.Resume(baseline);
        }
        finally
        {
            EndCall();
        }
    }

    /// <summary>
    /// Expires suspended operations that have outstayed their residency. Callable from any thread
    /// and never blocking.
    /// </summary>
    /// <remarks>
    /// A deadline the core enforced with its own timer would be a thread the runtime does not have
    /// and a policy the host did not choose. Instead the host polls, and the bound is enforced when
    /// it does.
    /// </remarks>
    public VmControlResult PollDeadlines()
    {
        System.Collections.Generic.List<VmOperation>? expired = null;

        lock (gate)
        {
            if (state is VmRuntimeState.Disposed)
            {
                return VmControlResult.InvalidState(VmReason.ObjectDisposed);
            }

            foreach (var pair in suspended)
            {
                if (pair.Value.HasOutstayed(options.MaxSuspendedResidency))
                {
                    (expired ??= new System.Collections.Generic.List<VmOperation>()).Add(pair.Value);
                }
            }

            if (expired is not null)
            {
                foreach (var operation in expired)
                {
                    suspended.Remove(operation.Key);
                }
            }
        }

        if (expired is null)
        {
            return VmControlResult.NoOp;
        }

        foreach (var operation in expired)
        {
            operation.Expire();
        }

        return VmControlResult.Accepted;
    }

    /// <summary>Requests cancellation of every operation in this runtime.</summary>
    public VmControlResult RequestCancel()
    {
        lock (gate)
        {
            if (state is VmRuntimeState.Disposed)
            {
                return VmControlResult.InvalidState(VmReason.ObjectDisposed);
            }

            foreach (var instance in instances)
            {
                instance.RequestCancel();
            }
        }

        return VmControlResult.Accepted;
    }

    /// <summary>Reads this runtime's consumption and remaining allowance.</summary>
    public VmBudgetSnapshot GetBudgetSnapshot()
    {
        lock (gate)
        {
            return runtimeLevel.Snapshot();
        }
    }

    /// <summary>
    /// Disposes the runtime. Idempotent, and terminal: every later call returns an invalid state.
    /// </summary>
    /// <remarks>
    /// Suspended operations are unwound through the profile's own terminal-unwind entry point under
    /// the tighter of its declared abandon budget and the runtime's unwind budget, so a parked
    /// operation can never block disposal indefinitely.
    /// </remarks>
    public VmControlResult Dispose()
    {
        System.Collections.Generic.List<VmOperation> parked;
        System.Collections.Generic.List<VmInstanceImplementation> live;

        lock (gate)
        {
            if (state is VmRuntimeState.Disposed)
            {
                return VmControlResult.NoOp;
            }

            if (state is VmRuntimeState.Disposing)
            {
                return VmControlResult.NoOp;
            }

            state = VmRuntimeState.Disposing;
            parked = new System.Collections.Generic.List<VmOperation>(suspended.Values);
            suspended.Clear();
            live = new System.Collections.Generic.List<VmInstanceImplementation>(instances);
        }

        foreach (var operation in parked)
        {
            operation.Abandon();
        }

        foreach (var instance in live)
        {
            instance.Dispose();
        }

        lock (gate)
        {
            instances.Clear();
            profiles.Clear();
            state = VmRuntimeState.Disposed;
        }

        parent?.ReleaseRuntime();
        return VmControlResult.Accepted;
    }

    /// <inheritdoc/>
    void System.IDisposable.Dispose() => Dispose();

    // ---- internals ------------------------------------------------------------------------

    internal object Gate => gate;

    internal VmRuntimeCreationOptions Options => options;

    internal VmAggregateBudget? Parent => parent;

    internal VmBudgetLevel RuntimeLevel => runtimeLevel;

    internal VmGuestLoadBounds GuestLoadBounds => guestLoadBounds;

    internal void Poison()
    {
        lock (gate)
        {
            if (state is VmRuntimeState.Ready)
            {
                state = VmRuntimeState.Poisoned;
            }
        }
    }

    internal void EnterCapability(VmCapabilityReentrancy reentrancy)
    {
        if (reentrancy is VmCapabilityReentrancy.NonReentrant)
        {
            System.Threading.Interlocked.Increment(ref inCapabilityDepth);
        }
    }

    internal void LeaveCapability(VmCapabilityReentrancy reentrancy)
    {
        if (reentrancy is VmCapabilityReentrancy.NonReentrant)
        {
            System.Threading.Interlocked.Decrement(ref inCapabilityDepth);
        }
    }

    internal bool TryBeginCall(out VmReason failure)
    {
        lock (gate)
        {
            switch (state)
            {
                case VmRuntimeState.Disposed:
                    failure = VmReason.ObjectDisposed;
                    return false;

                case VmRuntimeState.Disposing:
                    failure = VmReason.ObjectDisposing;
                    return false;

                case VmRuntimeState.Poisoned:
                    failure = VmReason.TerminalFault;
                    return false;
            }

            // A public call reached from inside a non-reentrant host capability is refused rather
            // than admitted: the capability declared that it would not re-enter, and enforcing that
            // is the difference between a declaration and a comment.
            if (System.Threading.Volatile.Read(ref inCapabilityDepth) > 0)
            {
                failure = VmReason.ReentrantRuntimeCallFromCapability;
                return false;
            }

            entryDepth++;
            failure = VmReason.None;
            return true;
        }
    }

    internal void EndCall()
    {
        lock (gate)
        {
            if (entryDepth > 0)
            {
                entryDepth--;
            }
        }
    }

    internal bool TryEnterVerification()
    {
        lock (gate)
        {
            if (activeVerifications >= options.MaxConcurrentVerifications)
            {
                return false;
            }

            activeVerifications++;
            return true;
        }
    }

    internal void ExitVerification()
    {
        lock (gate)
        {
            if (activeVerifications > 0)
            {
                activeVerifications--;
            }
        }
    }

    internal void RegisterInstance(VmInstanceImplementation instance)
    {
        lock (gate)
        {
            instances.Add(instance);
        }
    }

    internal void ForgetInstance(VmInstanceImplementation instance)
    {
        lock (gate)
        {
            instances.Remove(instance);
        }
    }

    internal bool TryPark(VmOperation operation, out VmReason failure)
    {
        lock (gate)
        {
            if (suspended.Count >= options.MaxLiveSuspendedOperations)
            {
                failure = VmReason.SuspendedOperationLimitReached;
                return false;
            }

            suspended[operation.Key] = operation;
            failure = VmReason.None;
            return true;
        }
    }

    internal void Unpark(VmOperation operation)
    {
        lock (gate)
        {
            suspended.Remove(operation.Key);
        }
    }

    internal VmProfileRuntimeState GetProfileState(VmProfileDescriptor descriptor)
    {
        lock (gate)
        {
            var key = descriptor.ProfileId.ToString();

            if (!profiles.TryGetValue(key, out var profileState))
            {
                profileState = new VmProfileRuntimeState(descriptor);
                profiles[key] = profileState;
            }

            return profileState;
        }
    }

    internal bool TryGetDescriptor(VmProfileId profileId, out VmProfileDescriptor descriptor) =>
        catalog.TryGetDescriptor(profileId, out descriptor);

    internal VmCapabilityBinding[] BindingsFor(VmProfileDescriptor descriptor) =>
        GetProfileState(descriptor).Bindings;

    internal IVmArtifactProvider? ProviderFor(VmProfileDescriptor descriptor) =>
        GetProfileState(descriptor).Provider;

    internal static ulong OperationKey(VmObjectId operationId) =>
        unchecked((ulong)operationId.GetHashCode());

    internal static VmDiagnostics Invalid(
        VmDiagnostics baseline,
        VmStage stage,
        VmReason reason,
        VmObjectKind kind,
        VmAttemptedCall attempted) =>
        baseline
            .WithOutcome(stage, VmOutcome.InvalidState, reason, VmInitiator.Caller)
            .WithObject(kind, 0, attempted);

    private bool TryBindCapabilities(out VmReason failure, out VmCapabilityId capability)
    {
        failure = VmReason.None;
        capability = default;

        var registrations = options.Capabilities.IsDefault
            ? System.Collections.Immutable.ImmutableArray<VmCapabilityRegistration>.Empty
            : options.Capabilities;

        var providers = 0;

        foreach (var registration in registrations)
        {
            if (registration.Kind is VmCapabilityKind.ArtifactProvider)
            {
                providers++;
            }
        }

        // At most one artifact provider per runtime. Two would make "which provider answered" a
        // question the charging and audit rules have no way to answer.
        if (providers > 1)
        {
            failure = VmReason.DuplicateArtifactProvider;
            return false;
        }

        foreach (var descriptor in catalog.Descriptors)
        {
            var profileState = GetProfileState(descriptor);

            if (!profileState.TryBind(registrations, out failure, out capability))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryResolveGuestLoadBounds(
        VmCatalog catalog,
        VmRuntimeCreationOptions options,
        out VmGuestLoadBounds bounds,
        out VmReason failure)
    {
        failure = VmReason.None;
        bounds = VmGuestLoadBounds.None;

        var declaring = 0;
        var profileMaxima = VmGuestLoadBounds.None;

        foreach (var descriptor in catalog.Descriptors)
        {
            if (descriptor.GuestInitiatedLoads.Kind is not VmDeclaration.Declared)
            {
                continue;
            }

            declaring++;
            profileMaxima = declaring == 1
                ? descriptor.GuestInitiatedLoads.ProfileHardMaxima
                : Tighten(profileMaxima, descriptor.GuestInitiatedLoads.ProfileHardMaxima);
        }

        var registersProvider = false;

        if (!options.Capabilities.IsDefault)
        {
            foreach (var registration in options.Capabilities)
            {
                if (registration.Kind is VmCapabilityKind.ArtifactProvider)
                {
                    registersProvider = true;
                }
            }
        }

        if (declaring == 0)
        {
            return true;
        }

        if (options.GuestLoadBounds.AdoptsProfileMaxima)
        {
            bounds = profileMaxima;
            return true;
        }

        var stated = options.GuestLoadBounds.Bounds;

        if (!stated.IsFinite || !stated.IsPositive)
        {
            failure = VmReason.GuestLoadBoundsNotConfigured;
            return false;
        }

        // A composition may tighten a profile's declared maxima and may never loosen them: the
        // profile's number is a hard maximum, not a suggestion.
        if (stated.NestedLoadDepth > profileMaxima.NestedLoadDepth ||
            stated.NestedLoadFanOut > profileMaxima.NestedLoadFanOut ||
            stated.NestedLoadBytes > profileMaxima.NestedLoadBytes ||
            stated.VerifierWork > profileMaxima.VerifierWork)
        {
            failure = VmReason.GuestLoadBoundExceedsProfileMaximum;
            return false;
        }

        bounds = stated;
        _ = registersProvider;
        return true;
    }

    private static VmGuestLoadBounds Tighten(VmGuestLoadBounds left, VmGuestLoadBounds right) =>
        new(
            System.Math.Min(left.NestedLoadDepth, right.NestedLoadDepth),
            System.Math.Min(left.NestedLoadFanOut, right.NestedLoadFanOut),
            System.Math.Min(left.NestedLoadBytes, right.NestedLoadBytes),
            System.Math.Min(left.VerifierWork, right.VerifierWork));
}
