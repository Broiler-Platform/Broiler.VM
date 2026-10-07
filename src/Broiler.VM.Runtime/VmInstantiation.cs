// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           6
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Medium
// Criteria:         3/0
// Resource impact:  5/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Abstractions;

namespace Broiler.VM;

/// <summary>What the runtime hands an executor when it creates one.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C06550
// Broiler-Human:        PENDING
internal sealed class VmExecutionEnvironment : IVmExecutionEnvironment
{
    private readonly VmArtifactLoadMediator? mediator;

    internal VmExecutionEnvironment(
        VmProfileId profileId,
        IVmMeter meter,
        IVmHostCapabilityInvoker capabilities,
        VmArtifactLoadMediator? mediator)
    {
        ProfileId = profileId;
        Meter = meter;
        Capabilities = capabilities;
        this.mediator = mediator;
    }

    /// <inheritdoc/>
    public VmProfileId ProfileId { get; }

    /// <inheritdoc/>
    public IVmMeter Meter { get; }

    /// <inheritdoc/>
    public IVmHostCapabilityInvoker Capabilities { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0008; IP=Low; Security=Medium; Resources=0; Fingerprint=AE529C
    // Broiler-Human:        PENDING
    public bool TryGetArtifactLoadMediator(out IVmArtifactLoadMediator loadMediator)
    {
        loadMediator = mediator!;
        return mediator is not null;
    }
}

/// <summary>The instantiation stage.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C8EFA1
// Broiler-Human:        PENDING
internal static class VmInstantiation
{
    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=Medium; Resources=5; Fingerprint=EA76BF
    // Broiler-Falsified-If: an instantiation that publishes and parks nothing leaves its lease held, or what its instance level retained still counted at the runtime or aggregate level
    // Broiler-Human:        PENDING
    internal static VmInstantiationResult Run(
        VmRuntime runtime,
        VmVerifiedArtifact artifact,
        VmLimitOverrides limitOverrides,
        System.Threading.CancellationToken cancellationToken,
        VmDiagnostics baseline)
    {
        var identified = baseline.WithArtifact(
            artifact.ObjectId, artifact.ByteLength, artifact.DiagnosticsBase.CallerIdentity);

        // The clauses run in the frozen order, and the order is load-bearing: a disposed handle
        // offered to a runtime that also lacks its profile must report the disposal, not the
        // absence. Hoisting the catalog lookup ahead of the handle state made the answer depend on
        // which of two true things the code happened to look at first.
        if (!TryAdmitHandleState(artifact, out var stateFailure))
        {
            return VmInstantiationResult.InvalidState(
                stateFailure,
                VmRuntime.Invalid(identified, VmStage.Instantiation, stateFailure, VmObjectKind.VerifiedArtifact, VmAttemptedCall.Instantiate));
        }

        if (!runtime.TryGetDescriptor(artifact.Identity.ProfileId, out var profile))
        {
            return VmInstantiationResult.UnsupportedProfile(
                VmReason.ProfileNotInCatalog,
                identified.WithOutcome(VmStage.Instantiation, VmOutcome.UnsupportedProfile, VmReason.ProfileNotInCatalog, VmInitiator.Caller));
        }

        if (!TryAdmitSharing(runtime, artifact, profile, out var handleFailure))
        {
            return VmInstantiationResult.InvalidState(
                handleFailure,
                VmRuntime.Invalid(identified, VmStage.Instantiation, handleFailure, VmObjectKind.VerifiedArtifact, VmAttemptedCall.Instantiate));
        }

        // P3, and before the lease: an override the runtime is going to refuse should not first
        // pin a handle it will then have to release. The inherited value is the handle's own
        // materialized instantiation ceiling, so an override is measured against what verification
        // established rather than against the runtime's wider ceiling.
        if (!VmLimitPrecedence.TryApply(
                VmBudgetScope.Instance,
                VmRuntime.ToArray(artifact.Identity.EffectiveCeilings.InstantiationCeilings),
                limitOverrides,
                out var instanceCeilings,
                out var offending,
                out var overrideFailure))
        {
            // A host failure, emphatically not a resource exhaustion: nothing was exhausted, and
            // reporting a composition mistake as exhaustion is the same diagnostic error that
            // separating an unsupported profile from an invalid artifact exists to prevent. The
            // dimension travels in the one group that can name one; the category is what says
            // whether anything ran out.
            return VmInstantiationResult.HostFailure(
                overrideFailure,
                identified
                    .WithOutcome(VmStage.Instantiation, VmOutcome.HostFailure, overrideFailure, VmInitiator.Caller)
                    .WithExhaustion(offending, VmBudgetScope.Instance));
        }

        // The instance pins the handle for as long as it lives, so a concurrent disposal drains
        // rather than cutting the ground from under a live instance.
        if (artifact.TryAcquireLease(out var lease).Kind is not VmControlOutcome.Accepted)
        {
            return VmInstantiationResult.InvalidState(
                VmReason.HandleDraining,
                VmRuntime.Invalid(identified, VmStage.Instantiation, VmReason.HandleDraining, VmObjectKind.VerifiedArtifact, VmAttemptedCall.Instantiate));
        }

        var instanceLevel = new VmBudgetLevel(VmBudgetScope.Instance, instanceCeilings);
        var succeeded = false;

        try
        {
            return Instantiate(
                runtime, artifact, lease, profile, instanceLevel, cancellationToken, identified, ref succeeded);
        }
        finally
        {
            if (!succeeded)
            {
                // Nothing was published and nothing parked, so no instance will ever be disposed
                // around this level. What the step reported retained was committed at the runtime
                // and aggregate levels too, and left there it would stay counted for bytes nobody
                // holds: at the runtime until it is disposed, and at the aggregate for good, since
                // a disposing runtime gives back only what its instances held. Released first and
                // the lease last, the order an instance's own disposal uses.
                VmInstanceImplementation.ReleaseRetained(runtime, instanceLevel);
                lease.Release();
            }
        }
    }

    /// <summary>Runs the profile's instantiation step and maps what it answered.</summary>
    /// <remarks>
    /// <para>
    /// The mapping follows the one precedence order every stage uses: cancellation, then resource
    /// exhaustion, then a host failure the profile did not convert, then a profile that broke its
    /// declared uncharged-work bound, then the step's own answer. Cancellation is read from the
    /// token as well as from the meter, as invocation and resume read it: a profile that never
    /// polled did not observe it, and the caller that cancelled is still not handed an instance.
    /// </para>
    /// <para>
    /// Instantiation is an operation like any other, and the step runs with it in scope, so a
    /// capability that fails terminally while the profile instantiates is latched onto it as it
    /// would be during an invocation. The profile's own answer is then discarded, whatever it was:
    /// a host defect is not billed to the guest because it happened before the guest had an
    /// instance. It is the same operation a declared asynchronous instantiation parks, so a resumed
    /// instantiation keeps its identity and its nested-load counters, as a resumed invocation does.
    /// </para>
    /// <para>
    /// A breach of the bound is found by either of the meter's two readings - a poll that found it
    /// and latched, or work still unpolled past the bound when the step returned - and is answered
    /// as a profile fault naming it, whatever the step said. So a profile that answered its refused
    /// poll as a cancellation is not reported as cancelled, and one that breached and then answered
    /// <c>Instantiated</c> publishes nothing: its start function is guest code, and an instance
    /// whose profile did not keep its latency promise is not handed to a caller any more than an
    /// invocation that did not keep it completes.
    /// </para>
    /// <para>
    /// A continuation parked by a step refused for cancellation, a host failure or a breach is
    /// unwound, as every other refused parking here is. What a refused step reported retained is
    /// given back by the caller, which owns the instance level for exactly that reason.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0004; IP=Low; Security=Medium; Resources=5; Fingerprint=F2101D
    // Broiler-Falsified-If: a step whose caller cancelled, during which a terminating capability failed, or whose meter latched or left unpolled a poll-bound breach, is answered as anything but that cancellation, that host failure or a profile fault naming the breach where nothing ranked above it applies, or publishes an instance
    // Broiler-Human:        PENDING
    private static VmInstantiationResult Instantiate(
        VmRuntime runtime,
        VmVerifiedArtifact artifact,
        VmArtifactLease lease,
        VmProfileDescriptor profile,
        VmBudgetLevel instanceLevel,
        System.Threading.CancellationToken cancellationToken,
        VmDiagnostics identified,
        ref bool succeeded)
    {
        var profileState = runtime.GetProfileState(profile);

        var invocationLevel = new VmBudgetLevel(
            VmBudgetScope.Invocation, instanceLevel.CeilingsCopy());

        var linked = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var meter = new VmMeter(
            runtime.Gate, invocationLevel, instanceLevel, runtime.RuntimeLevel, runtime.Parent,
            profile.MaxUnchargedWork, linked.Token);

        if (profile.GuestInitiatedLoads.Kind is VmDeclaration.Declared)
        {
            profileState.Mediator ??= new VmArtifactLoadMediator(runtime, profile, profileState.Scope);
        }

        var mediator = profileState.Mediator;

        // The environment is per profile per runtime, and everything operation-shaped inside it
        // resolves through the scope. Capturing this operation's meter here would charge every
        // later invocation against the instantiation that happened to create the executor.
        var environment = new VmExecutionEnvironment(
            profile.ProfileId,
            new VmAmbientMeter(profileState.Scope),
            new VmAmbientCapabilityInvoker(profileState.Bindings, runtime, profileState.Scope),
            mediator);

        if (!profileState.TryGetExecutor(environment, out var executor, out var executorFailure))
        {
            return VmInstantiationResult.ProfileFault(
                executorFailure, null,
                identified.WithOutcome(VmStage.Instantiation, VmOutcome.ProfileFault, executorFailure, VmInitiator.Guest));
        }

        VmExecutionStep step;

        // Created before the step rather than only when it parks: a host failure latches onto the
        // operation in scope, and a step run with none in scope had its host failures dropped.
        var operation = new VmOperation(
            runtime, null, profile, VmOperationKind.Instantiate, meter, linked,
            VmStage.Instantiation, identified);

        // The scope is what the executor's meter, capability table and mediator resolve through,
        // and it answers only inside the dynamic extent of this step.
        profileState.Scope.Enter(meter, operation);

        // Keyed on the operation's own identity, which is fresh for every instantiation - two
        // instantiations are two operations, and they no more share a fan-out allowance than two
        // invocations do - and which a resume of a parked instantiation enters with again. A
        // separately minted key reset the counters at that first resume, handing a profile that
        // parks between loads a second allowance.
        mediator?.EnterScope(identified, operation.ObjectId);

        try
        {
            step = executor.Instantiate(artifact, linked.Token);
        }
        catch (System.OperationCanceledException)
        {
            return VmInstantiationResult.Cancellation(
                VmReason.Cancelled,
                identified.WithOutcome(VmStage.Instantiation, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host));
        }
        catch (System.Exception)
        {
            return VmInstantiationResult.ProfileFault(
                VmReason.ProfileContractViolation, null,
                identified.WithOutcome(VmStage.Instantiation, VmOutcome.ProfileFault, VmReason.ProfileContractViolation, VmInitiator.Guest));
        }
        finally
        {
            profileState.Scope.Leave();

            // As on the invocation and resume paths: what the step spent from a block is committed
            // before the outcome is mapped. The poll-bound reading below settles for itself as well,
            // and this settle also hands the fuel back to the levels rather than leaving it in a
            // table slot another operation could have used.
            meter.SettlePreAdmittedFuel();
        }

        // The token as well as the meter, as on the invocation and resume paths: a profile that
        // never polled did not observe the cancellation, but the caller cancelled all the same and
        // is owed that answer rather than an instance it no longer asked for.
        if (meter.CancellationObserved || linked.Token.IsCancellationRequested)
        {
            if (step.Kind is VmExecutionStepKind.Suspended && step.Continuation is not null)
            {
                Abandon(profile, runtime, executor, step.Continuation);
            }

            return VmInstantiationResult.Cancellation(
                VmReason.Cancelled,
                identified.WithOutcome(VmStage.Instantiation, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host));
        }

        if (meter.ExhaustionObserved && step.Kind is not VmExecutionStepKind.Suspended)
        {
            return VmInstantiationResult.ResourceExhaustion(
                VmMeter.ReasonFor(meter.FailedDimension),
                identified
                    .WithOutcome(VmStage.Instantiation, VmOutcome.ResourceExhaustion, VmMeter.ReasonFor(meter.FailedDimension), VmInitiator.Core)
                    .WithExhaustion(meter.FailedDimension, meter.FailedScope));
        }

        if (operation.HostFailure is not VmReason.None)
        {
            if (step.Kind is VmExecutionStepKind.Suspended && step.Continuation is not null)
            {
                Abandon(profile, runtime, executor, step.Continuation);
            }

            return VmInstantiationResult.HostFailure(
                operation.HostFailure,
                identified
                    .WithOutcome(VmStage.Instantiation, VmOutcome.HostFailure, operation.HostFailure, VmInitiator.Host)
                    .WithCapability(operation.HostFailureCapability, operation.HostFailureCapabilityVersion, default));
        }

        if (meter.PollBoundExceeded || meter.UnpolledWorkExceedsBound)
        {
            if (step.Kind is VmExecutionStepKind.Suspended && step.Continuation is not null)
            {
                Abandon(profile, runtime, executor, step.Continuation);
            }

            return VmInstantiationResult.ProfileFault(
                VmReason.CancellationPollBoundExceeded, step.Payload,
                identified.WithOutcome(VmStage.Instantiation, VmOutcome.ProfileFault, VmReason.CancellationPollBoundExceeded, VmInitiator.Guest));
        }

        switch (step.Kind)
        {
            case VmExecutionStepKind.Instantiated when step.State is not null:
            {
                var instance = new VmInstanceImplementation(
                    runtime, profile, executor, step.State, instanceLevel, identified,
                    profileState.Scope, mediator, lease);

                if (!runtime.RegisterInstance(instance))
                {
                    // The runtime began disposing while this instantiation was inside the profile.
                    // The instance exists and holds a lease and an allowance, and nothing else can
                    // reach it, so this is the only place it can be given back.
                    instance.Dispose();

                    return VmInstantiationResult.InvalidState(
                        VmReason.ObjectDisposing,
                        VmRuntime.Invalid(
                            identified, VmStage.Instantiation, VmReason.ObjectDisposing,
                            VmObjectKind.Runtime, VmAttemptedCall.Instantiate));
                }

                succeeded = true;

                return VmInstantiationResult.Normal(
                    instance,
                    step.Payload,
                    identified.WithOutcome(VmStage.Instantiation, VmOutcome.Normal, VmReason.NormalCompleted, VmInitiator.Caller));
            }

            case VmExecutionStepKind.Suspended:
            {
                // Asynchronous instantiation exists as a transition in contract version 1 and is
                // gated on the descriptor declaring it. A profile that parks here without having
                // declared it is refused after a bounded abandon, not silently accommodated.
                if (profile.AsynchronousInstantiation is not VmDeclaration.Declared)
                {
                    if (step.Continuation is not null)
                    {
                        Abandon(profile, runtime, executor, step.Continuation);
                    }

                    return VmInstantiationResult.InvalidState(
                        VmReason.UndeclaredAsynchronousInstantiation,
                        VmRuntime.Invalid(
                            identified, VmStage.Instantiation, VmReason.UndeclaredAsynchronousInstantiation,
                            VmObjectKind.Operation, VmAttemptedCall.Instantiate));
                }

                if (step.Continuation is null)
                {
                    return VmInstantiationResult.ProfileFault(
                        VmReason.ProfileContractViolation, null,
                        identified.WithOutcome(VmStage.Instantiation, VmOutcome.ProfileFault, VmReason.ProfileContractViolation, VmInitiator.Guest));
                }

                // The declared case parks. The instance is NOT published - an instance exists only
                // when instantiation completes normally - so what the caller receives is the
                // resumption object and nothing else. Both halves of the pairing are made before the
                // operation parks, so nothing that finds it parked finds it without its instance.
                var pending = new VmInstanceImplementation(
                    runtime, profile, executor, PlaceholderState.Instance, instanceLevel, identified,
                    profileState.Scope, mediator, lease);

                operation.AttachInstance(pending);
                pending.ParkInstantiation(operation);

                if (!operation.TryPark(
                        VmSuspensionOrigin.Instantiation, step.Continuation, step.Payload,
                        out var suspension, out var parkFailure))
                {
                    Abandon(profile, runtime, executor, step.Continuation);

                    return VmInstantiationResult.InvalidState(
                        parkFailure,
                        VmRuntime.Invalid(
                            identified, VmStage.Instantiation, parkFailure,
                            VmObjectKind.Operation, VmAttemptedCall.Instantiate));
                }

                if (!runtime.RegisterInstance(pending))
                {
                    // Same race, on the asynchronous-instantiation path: the operation is parked
                    // and the placeholder instance is live, and neither is reachable by anyone but
                    // this frame.
                    operation.Abandon(VmReason.ObjectDisposing);
                    pending.Dispose();

                    return VmInstantiationResult.InvalidState(
                        VmReason.ObjectDisposing,
                        VmRuntime.Invalid(
                            identified, VmStage.Instantiation, VmReason.ObjectDisposing,
                            VmObjectKind.Runtime, VmAttemptedCall.Instantiate));
                }
                succeeded = true;

                return VmInstantiationResult.Suspension(
                    suspension,
                    step.Payload,
                    identified.WithOutcome(VmStage.Instantiation, VmOutcome.Suspension, VmReason.InstantiationSuspended, VmInitiator.Guest));
            }

            case VmExecutionStepKind.Faulted:
                return VmInstantiationResult.ProfileFault(
                    VmReason.ProfileFaultUnspecified, step.Payload,
                    identified.WithOutcome(VmStage.Instantiation, VmOutcome.ProfileFault, VmReason.ProfileFaultUnspecified, VmInitiator.Guest));

            case VmExecutionStepKind.ContractViolation:
                return VmInstantiationResult.ProfileFault(
                    step.Reason, null,
                    identified.WithOutcome(VmStage.Instantiation, VmOutcome.ProfileFault, step.Reason, VmInitiator.Guest));

            default:
                return VmInstantiationResult.ProfileFault(
                    VmReason.ProfileContractViolation, null,
                    identified.WithOutcome(VmStage.Instantiation, VmOutcome.ProfileFault, VmReason.ProfileContractViolation, VmInitiator.Guest));
        }
    }

    /// <summary>
    /// The cross-runtime sharing predicate, applied when a handle from another runtime is offered.
    /// </summary>
    /// <remarks>
    /// Every clause is checked in the frozen order, so a handle failing two of them always reports
    /// the same one. Ceilings are compared by exact equality rather than by subsumption: relaxing
    /// that would turn a refusal into a success, which is a breaking amendment.
    /// </remarks>
    /// <summary>
    /// Clauses 0 and 1: the handle's own state, checked before anything about the composition.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0006 s3; IP=Low; Security=Medium; Resources=0; Fingerprint=87809B
    // Broiler-Human:        PENDING
    private static bool TryAdmitHandleState(VmVerifiedArtifact artifact, out VmReason failure)
    {
        failure = VmReason.None;

        if (artifact.State is VmVerifiedArtifactState.Disposed)
        {
            failure = VmReason.HandleDisposed;
            return false;
        }

        if (artifact.State is VmVerifiedArtifactState.Draining)
        {
            failure = VmReason.HandleDraining;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Clauses 3 to 11: the cross-runtime sharing predicate, in the frozen clause order.
    /// </summary>
    /// <remarks>
    /// The identity clauses are evaluated before the sharing declaration, so a handle that fails
    /// several of them always reports the same one - the first in the frozen order. Evaluating the
    /// declaration first made every mismatch report as "not shareable", which is true of the
    /// profile but says nothing about why this handle was refused.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0006 s4; IP=Low; Security=Medium; Resources=1; Fingerprint=078B3F
    // Broiler-Human:        PENDING
    private static bool TryAdmitSharing(
        VmRuntime runtime,
        VmVerifiedArtifact artifact,
        VmProfileDescriptor profile,
        out VmReason failure)
    {
        failure = VmReason.None;

        if (artifact.OwningRuntimeId.Equals(runtime.ObjectId))
        {
            return true;
        }

        // A guest-initiated handle is never shareable: its ceilings came from one operation's
        // remainder, which means nothing in another runtime.
        if (artifact.Origin is VmArtifactOrigin.GuestInitiated)
        {
            failure = VmReason.NestedHandleNotShareable;
            return false;
        }

        // The receiving runtime's own materialized ceilings, not the handle's: comparing a handle
        // against itself would make clause 8 unfalsifiable.
        var receivingCeilings = VmLimitVector.Intersect(
            runtime.RuntimeLevel.AsCeilingVector(), profile.ProfileHardMaxima);

        var receiving = new VmVerifiedArtifactIdentity(
            profile.ProfileId,
            profile.DescriptorRevision,
            artifact.Identity.AcceptedProfileFormatVersion,
            artifact.Identity.ManifestId,
            profile.ConformanceManifestVersion,
            profile.Verifier.VerifierSemanticVersion,
            VmCoreContract.Version,
            new VmEffectiveCeilings(receivingCeilings, receivingCeilings),
            runtime.GetProfileState(profile).Assumptions);

        var mismatch = artifact.Identity.FirstMismatch(receiving);

        if (mismatch is not VmReason.None)
        {
            failure = mismatch;
            return false;
        }

        // Clause 10 last among the identity clauses: the declaration is a property of the profile,
        // and reporting it ahead of a genuine identity difference would hide which one applied.
        if (artifact.Sharing is not VmArtifactSharing.Shareable)
        {
            failure = VmReason.SharedHandleNotShareable;
            return false;
        }

        var parentId = runtime.Parent?.Id.ObjectId ?? default;

        if (!artifact.AggregateBudgetId.Equals(parentId))
        {
            failure = VmReason.SharedHandleAggregateBudgetMismatch;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Stands in for instance state while an asynchronous instantiation is still parked.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The instance is not published to the caller until instantiation completes normally, so this
    /// is never handed to anyone: it exists so the parked operation has an instance to be resumed
    /// against. It is also what the profile's <c>Resume</c> is handed for that instance, and the
    /// profile has nothing to read in it - its own partial state travels in its continuation.
    /// </para>
    /// <para>
    /// The profile supplies its real state by completing the resumed instantiation with
    /// <c>Instantiated</c>, and the pending instance takes that state before it is published. A
    /// resumed instantiation that completes with anything else publishes nothing, and its pending
    /// instance is disposed.
    /// </para>
    /// <para>
    /// <i>(Corrected 2026-09-28. This read "The profile supplies its real state when it
    /// completes", and nothing implemented it: a resumed instantiation that completed was published
    /// still holding this placeholder, and every later invocation handed it to the profile.)</i>
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5323E5
    // Broiler-Falsified-If: an instance published to a caller, by instantiation or by a resume that completed one, still holds the placeholder
    // Broiler-Human:        PENDING
    private sealed class PlaceholderState : IVmInstanceState
    {
        internal static PlaceholderState Instance { get; } = new();
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0009; IP=Low; Security=Medium; Resources=4; Fingerprint=ED427A
    // Broiler-Human:        PENDING
    private static void Abandon(
        VmProfileDescriptor profile,
        VmRuntime runtime,
        IVmProfileExecutor executor,
        IVmProfileContinuation continuation)
    {
        var allowance = System.Math.Min(profile.AbandonBudget, runtime.Options.UnwindBudget);

        try
        {
            executor.Unwind(continuation, allowance);
        }
        catch (System.Exception)
        {
            // Already abandoned; there is nobody left to report to.
        }
    }
}
