namespace Broiler.VM;

/// <summary>
/// The core's mediation of a guest-initiated load: the only route by which an executing profile can
/// obtain further executable bytes.
/// </summary>
/// <remarks>
/// <para>
/// The composition, not the guest, decides whether this is possible at all. A profile declares that
/// it may request loads; the host either registers a typed artifact-provider capability or does not.
/// A composition that registers none refuses every request deterministically - which is how a
/// content policy forbidding dynamic evaluation is expressed as a contract outcome rather than as an
/// ad-hoc check inside an engine.
/// </para>
/// <para>
/// Every request is bounded in depth, fan-out and cumulative bytes, and every unit of nested work is
/// charged to the operation that made it. A nested load can exhaust an invocation; it can never
/// enlarge one. The bytes the provider returns become their own verified handle before anything in
/// them runs: nesting relaxes no bound, skips no descriptor match, and inherits no ceiling
/// implicitly.
/// </para>
/// <para>
/// <strong>Scope.</strong> One mediator object is handed to a declaring profile's executor when the
/// executor is created, and it answers only inside the dynamic extent of the step that supplied it.
/// The runtime opens the scope before calling the executor and closes it afterwards, so a profile
/// that stashes the mediator in a field and calls it later - from another step, or after the
/// operation ended - is refused rather than charged against whatever operation happens to be
/// running. Handing out a fresh object per step would leave the stale one able to answer.
/// </para>
/// </remarks>
internal sealed class VmArtifactLoadMediator : IVmArtifactLoadMediator
{
    private readonly VmRuntime runtime;
    private readonly VmProfileDescriptor profile;
    private readonly VmExecutionScope scope;
    private readonly object gate = new();

    private VmDiagnostics scopeBaseline;
    private int depth;
    private ulong fanOut;
    private ulong bytes;

    internal VmArtifactLoadMediator(VmRuntime runtime, VmProfileDescriptor profile, VmExecutionScope scope)
    {
        this.runtime = runtime;
        this.profile = profile;
        this.scope = scope;
    }

    /// <summary>
    /// Opens the mediator for one executor step, binding it to that step's meter so nested work is
    /// charged to the operation that will actually request it.
    /// </summary>
    internal void EnterScope(VmDiagnostics baseline)
    {
        lock (gate)
        {
            scopeBaseline = baseline;

            // Fan-out and cumulative bytes are per operation, so they reset with the scope. Depth
            // is not reset: it is a live nesting measure and is unwound as each request returns.
            fanOut = 0;
            bytes = 0;
        }
    }

    /// <inheritdoc/>
    public VmGuestLoadResult RequestLoad(scoped in VmArtifactRequest request)
    {
        VmMeter meter;
        VmDiagnostics identified;

        lock (gate)
        {
            var current = scope.Current;

            if (current is null)
            {
                var stale = VmDiagnostics
                    .Create(VmStage.GuestInitiatedLoad, VmOutcome.InvalidState, VmReason.MediatorOutOfScope,
                        runtime.ObjectId, VmInitiator.Guest, VmAttemptedCall.GuestLoad)
                    .WithObject(VmObjectKind.Operation, 0, VmAttemptedCall.GuestLoad);

                return VmGuestLoadResult.InvalidState(VmReason.MediatorOutOfScope, stale);
            }

            meter = current;
            identified = scopeBaseline
                .WithOutcome(VmStage.GuestInitiatedLoad, VmOutcome.None, VmReason.None, VmInitiator.Guest)
                .WithOperation(request.RequestingOperationId, request.RequestingOperationId, request.NestingDepth);
        }

        var provider = runtime.ProviderFor(profile);

        if (provider is null)
        {
            // The deterministic refusal. Registering no provider IS the content policy, so this is
            // the ordinary answer of a correctly configured composition, not an error condition.
            return VmGuestLoadResult.HostFailure(
                VmReason.ProviderNotRegistered,
                identified.WithOutcome(VmStage.GuestInitiatedLoad, VmOutcome.HostFailure, VmReason.ProviderNotRegistered, VmInitiator.Core));
        }

        var bounds = runtime.GuestLoadBounds;

        lock (gate)
        {
            if ((ulong)(depth + 1) > bounds.NestedLoadDepth)
            {
                return Exhausted(identified, VmBudgetDimension.NestedLoadDepth);
            }

            if (fanOut + 1 > bounds.NestedLoadFanOut)
            {
                return Exhausted(identified, VmBudgetDimension.NestedLoadFanOut);
            }
        }

        // Charged before the provider is asked, so a request that cannot be paid for never reaches
        // the host at all.
        if (!meter.TryCharge(VmBudgetDimension.NestedLoadFanOut, 1))
        {
            return Exhausted(identified, meter.FailedDimension, meter.FailedScope);
        }

        if (!meter.TryCharge(VmBudgetDimension.NestedLoadDepth, 1))
        {
            return Exhausted(identified, meter.FailedDimension, meter.FailedScope);
        }

        lock (gate)
        {
            depth++;
            fanOut++;
        }

        try
        {
            return Answer(provider, meter, in request, identified, bounds);
        }
        finally
        {
            lock (gate)
            {
                depth--;
            }

            meter.ReportReleased(VmBudgetDimension.NestedLoadDepth, 1);
        }
    }

    private VmGuestLoadResult Answer(
        IVmArtifactProvider provider,
        VmMeter meter,
        scoped in VmArtifactRequest request,
        VmDiagnostics identified,
        VmGuestLoadBounds bounds)
    {
        VmArtifactProviderAnswer answer;

        try
        {
            answer = provider.Answer(in request);
        }
        catch (System.OperationCanceledException)
        {
            return VmGuestLoadResult.Cancellation(
                VmReason.Cancelled,
                identified.WithOutcome(VmStage.GuestInitiatedLoad, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host));
        }
        catch (System.Exception)
        {
            // A provider that throws is a host fault, not a refusal. The difference matters: a
            // policy that refuses is working, and a provider that throws is broken.
            return VmGuestLoadResult.HostFailure(
                VmReason.HostCapabilityFaulted,
                identified.WithOutcome(VmStage.GuestInitiatedLoad, VmOutcome.HostFailure, VmReason.HostCapabilityFaulted, VmInitiator.Host));
        }

        switch (answer.Kind)
        {
            case VmArtifactProviderAnswerKind.Refused:
                return VmGuestLoadResult.HostFailure(
                    VmReason.ProviderRefused,
                    identified.WithOutcome(VmStage.GuestInitiatedLoad, VmOutcome.HostFailure, VmReason.ProviderRefused, VmInitiator.Host));

            case VmArtifactProviderAnswerKind.NotFound:
                return VmGuestLoadResult.HostFailure(
                    VmReason.ProviderArtifactNotFound,
                    identified.WithOutcome(VmStage.GuestInitiatedLoad, VmOutcome.HostFailure, VmReason.ProviderArtifactNotFound, VmInitiator.Host));
        }

        // A provider may only answer with an artifact of the profile that asked. Answering with
        // another profile's artifact would let one profile reach another through the host.
        if (!answer.Descriptor.ProfileId.Equals(request.RequestingProfileId))
        {
            return VmGuestLoadResult.HostFailure(
                VmReason.ProviderProfileMismatch,
                identified.WithOutcome(VmStage.GuestInitiatedLoad, VmOutcome.HostFailure, VmReason.ProviderProfileMismatch, VmInitiator.Host));
        }

        var length = (ulong)answer.Payload.Length;

        lock (gate)
        {
            if (bytes + length > bounds.NestedLoadBytes)
            {
                return Exhausted(identified, VmBudgetDimension.NestedLoadBytes);
            }

            bytes += length;
        }

        if (!meter.TryCharge(VmBudgetDimension.NestedLoadBytes, length))
        {
            return Exhausted(identified, meter.FailedDimension, meter.FailedScope);
        }

        // The nested bytes take the ordinary verification path, under the requesting operation's
        // remaining allowance rather than under a runtime ceiling.
        var verification = runtime.VerifyCore(
            answer.Descriptor,
            answer.Payload,
            request.CancellationToken,
            identified,
            VmArtifactOrigin.GuestInitiated,
            meter);

        return Project(verification, identified);
    }

    private static VmGuestLoadResult Project(VmVerificationResult verification, VmDiagnostics identified)
    {
        var diagnostics = verification.Diagnostics.WithOutcome(
            VmStage.GuestInitiatedLoad, verification.Outcome, verification.Reason, VmInitiator.Guest);

        switch (verification.Outcome)
        {
            case VmOutcome.Normal:
                verification.TryGetArtifact(out var artifact);
                return VmGuestLoadResult.Normal(artifact, diagnostics);

            case VmOutcome.UnsupportedProfile:
                return VmGuestLoadResult.UnsupportedProfile(verification.Reason, diagnostics);

            case VmOutcome.InvalidArtifact:
                return VmGuestLoadResult.InvalidArtifact(verification.Reason, diagnostics);

            case VmOutcome.Cancellation:
                return VmGuestLoadResult.Cancellation(verification.Reason, diagnostics);

            case VmOutcome.ResourceExhaustion:
                return VmGuestLoadResult.ResourceExhaustion(verification.Reason, diagnostics);

            default:
                return VmGuestLoadResult.InvalidState(
                    verification.Reason,
                    VmRuntime.Invalid(identified, VmStage.GuestInitiatedLoad, verification.Reason, VmObjectKind.Operation, VmAttemptedCall.GuestLoad));
        }
    }

    private static VmGuestLoadResult Exhausted(
        VmDiagnostics identified,
        VmBudgetDimension dimension,
        VmBudgetScope scope = VmBudgetScope.Invocation) =>
        VmGuestLoadResult.ResourceExhaustion(
            VmReason.CeilingReached,
            identified
                .WithOutcome(VmStage.GuestInitiatedLoad, VmOutcome.ResourceExhaustion, VmReason.CeilingReached, VmInitiator.Core)
                .WithExhaustion(dimension, scope));
}
