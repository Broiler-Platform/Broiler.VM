namespace Broiler.VM;

/// <summary>What the runtime hands an executor when it creates one.</summary>
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
    public bool TryGetArtifactLoadMediator(out IVmArtifactLoadMediator loadMediator)
    {
        loadMediator = mediator!;
        return mediator is not null;
    }
}

/// <summary>The instantiation stage.</summary>
internal static class VmInstantiation
{
    internal static VmInstantiationResult Run(
        VmRuntime runtime,
        VmVerifiedArtifact artifact,
        System.Threading.CancellationToken cancellationToken,
        VmDiagnostics baseline)
    {
        var identified = baseline.WithArtifact(
            artifact.ObjectId, artifact.ByteLength, artifact.DiagnosticsBase.CallerIdentity);

        if (!runtime.TryGetDescriptor(artifact.Identity.ProfileId, out var profile))
        {
            return VmInstantiationResult.UnsupportedProfile(
                VmReason.ProfileNotInCatalog,
                identified.WithOutcome(VmStage.Instantiation, VmOutcome.UnsupportedProfile, VmReason.ProfileNotInCatalog, VmInitiator.Caller));
        }

        if (!TryAdmitHandle(runtime, artifact, out var handleFailure))
        {
            return VmInstantiationResult.InvalidState(
                handleFailure,
                VmRuntime.Invalid(identified, VmStage.Instantiation, handleFailure, VmObjectKind.VerifiedArtifact, VmAttemptedCall.Instantiate));
        }

        if (!artifact.TryGetState(out _))
        {
            return VmInstantiationResult.InvalidState(
                VmReason.HandleDisposed,
                VmRuntime.Invalid(identified, VmStage.Instantiation, VmReason.HandleDisposed, VmObjectKind.VerifiedArtifact, VmAttemptedCall.Instantiate));
        }

        var profileState = runtime.GetProfileState(profile);

        var instanceLevel = new VmBudgetLevel(
            VmBudgetScope.Instance,
            VmRuntime.ToArray(artifact.Identity.EffectiveCeilings.InstantiationCeilings));

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

        // The scope is what the executor's meter, capability table and mediator resolve through,
        // and it answers only inside the dynamic extent of this step.
        profileState.Scope.Enter(meter);
        mediator?.EnterScope(identified);

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
        }

        if (meter.CancellationObserved)
        {
            return VmInstantiationResult.Cancellation(
                VmReason.Cancelled,
                identified.WithOutcome(VmStage.Instantiation, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host));
        }

        if (meter.ExhaustionObserved && step.Kind is not VmExecutionStepKind.Suspended)
        {
            return VmInstantiationResult.ResourceExhaustion(
                VmReason.AllowanceExhausted,
                identified
                    .WithOutcome(VmStage.Instantiation, VmOutcome.ResourceExhaustion, VmReason.AllowanceExhausted, VmInitiator.Core)
                    .WithExhaustion(meter.FailedDimension, meter.FailedScope));
        }

        switch (step.Kind)
        {
            case VmExecutionStepKind.Instantiated when step.State is not null:
            {
                var instance = new VmInstanceImplementation(
                    runtime, profile, executor, step.State, instanceLevel, identified,
                    profileState.Scope, mediator);

                runtime.RegisterInstance(instance);

                return VmInstantiationResult.Normal(
                    instance,
                    step.Payload,
                    identified.WithOutcome(VmStage.Instantiation, VmOutcome.Normal, VmReason.NormalCompleted, VmInitiator.Caller));
            }

            case VmExecutionStepKind.Suspended:
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

                return VmInstantiationResult.ProfileFault(
                    VmReason.ProfileContractViolation, null,
                    identified.WithOutcome(VmStage.Instantiation, VmOutcome.ProfileFault, VmReason.ProfileContractViolation, VmInitiator.Guest));

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
    private static bool TryAdmitHandle(VmRuntime runtime, VmVerifiedArtifact artifact, out VmReason failure)
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

        if (artifact.Sharing is not VmArtifactSharing.Shareable)
        {
            failure = VmReason.SharedHandleNotShareable;
            return false;
        }

        if (!runtime.TryGetDescriptor(artifact.Identity.ProfileId, out var profile))
        {
            failure = VmReason.ProfileNotInCatalog;
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

        var parentId = runtime.Parent?.Id.ObjectId ?? default;

        if (!artifact.AggregateBudgetId.Equals(parentId))
        {
            failure = VmReason.SharedHandleAggregateBudgetMismatch;
            return false;
        }

        return true;
    }

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
