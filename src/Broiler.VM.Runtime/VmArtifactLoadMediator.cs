// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           11
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    Medium
// Criteria:         1/0
// Resource impact:  7/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

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
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C62B33
// Broiler-Human:        PENDING
internal sealed class VmArtifactLoadMediator : IVmArtifactLoadMediator
{
    private readonly VmRuntime runtime;
    private readonly VmProfileDescriptor profile;
    private readonly VmExecutionScope scope;
    private readonly object gate = new();

    private VmDiagnostics scopeBaseline;
    private VmObjectId currentOperation;
    private int depth;
    private ulong fanOut;
    private ulong bytes;
    private ulong verifierWork;

    internal VmArtifactLoadMediator(VmRuntime runtime, VmProfileDescriptor profile, VmExecutionScope scope)
    {
        this.runtime = runtime;
        this.profile = profile;
        this.scope = scope;
    }

    /// <summary>
    /// Opens the mediator for one step of the named operation, binding it to that step's meter so
    /// nested work is charged to the operation that will actually request it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The operation's identity is a required argument and there is deliberately no overload that
    /// omits it. There was one, and every call site used it: it passed <c>default</c>, every step
    /// therefore compared equal to the last, and the reset below never ran once. The counters it
    /// guards are the fan-out, cumulative-bytes and nested-verifier-work bounds, so what was
    /// documented as a per-operation bound behaved as a lifetime bound on a mediator shared by
    /// every instance of one profile in one runtime - a profile could request its fan-out limit
    /// worth of loads in total and never another. Removing the overload is what makes that
    /// unrepeatable; a caller with no operation of its own mints an identity rather than passing
    /// nothing.
    /// </para>
    /// <para>
    /// VM-5's baseline work found this: the guest-load lane measured how many mediated loads one
    /// runtime admits and got the fan-out limit instead of a number in the thousands.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0008; IP=Low; Security=Medium; Resources=1; Fingerprint=20F004
    // Broiler-Falsified-If: two steps of two different operations share a fan-out, byte or verifier-work count
    // Broiler-Human:        PENDING
    internal void EnterScope(VmDiagnostics baseline, VmObjectId operationId)
    {
        lock (gate)
        {
            scopeBaseline = baseline;

            // Deliberately NOT reset here. Fan-out and cumulative bytes are per operation, and a
            // resumed operation is the same operation - it keeps its identity, its budget remainder
            // and its nested-load counters. Resetting on every step would let a profile that yields
            // between loads have as many as it liked.
            if (!operationId.Equals(currentOperation))
            {
                currentOperation = operationId;
                fanOut = 0;
                bytes = 0;
                verifierWork = 0;
            }
        }
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0008; IP=Low; Security=Medium; Resources=7; Fingerprint=C04C88
    // Broiler-Human:        PENDING
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
                return Exhausted(meter, identified, VmBudgetDimension.NestedLoadDepth);
            }

            if (fanOut + 1 > bounds.NestedLoadFanOut)
            {
                return Exhausted(meter, identified, VmBudgetDimension.NestedLoadFanOut);
            }

            // The cumulative nested verifier-work bound, checked before the provider is asked.
            // It is a separate bound from the operation's own verifier-work allowance and it is
            // the tighter of the two by construction, which is what makes it worth having: an
            // operation may spend its whole allowance on the artifact it was given, and only this
            // bounds how much of it the loads it requests may consume.
            if (verifierWork >= bounds.VerifierWork)
            {
                return Exhausted(meter, identified, VmBudgetDimension.VerifierWork);
            }
        }

        // Charged before the provider is asked, so a request that cannot be paid for never reaches
        // the host at all.
        if (!meter.TryCharge(VmBudgetDimension.NestedLoadFanOut, 1))
        {
            return Exhausted(meter, identified, meter.FailedDimension, meter.FailedScope);
        }

        if (!meter.TryCharge(VmBudgetDimension.NestedLoadDepth, 1))
        {
            return Exhausted(meter, identified, meter.FailedDimension, meter.FailedScope);
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

    // Broiler-AI:           Origin=AI; Spec=ADR-0008; IP=Low; Security=Medium; Resources=7; Fingerprint=9DC2AB
    // Broiler-Human:        PENDING
    private VmGuestLoadResult Answer(
        IVmArtifactProvider provider,
        VmMeter meter,
        scoped in VmArtifactRequest request,
        VmDiagnostics identified,
        VmGuestLoadBounds bounds)
    {
        // A provider call is a host call: it is charged like one and it runs inside the capability
        // boundary like one. Without the charge a guest could drive an unbounded number of provider
        // requests against a runtime whose host-call allowance was exhausted; without the boundary
        // the mandatory non-reentrancy of a provider was enforced nowhere.
        if (!meter.TryCharge(VmBudgetDimension.HostCalls, 1))
        {
            return Exhausted(meter, identified, meter.FailedDimension, meter.FailedScope);
        }

        VmArtifactProviderAnswer answer;

        runtime.EnterProviderCall();

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
        finally
        {
            runtime.LeaveProviderCall();
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
        // another profile's artifact would let one profile reach another through the host. This is
        // a provider contract breach rather than a composition gap, so it stays a host failure -
        // an identity the catalog simply lacks is reported by the ordinary verification path below,
        // which answers unsupported profile.
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
                return Exhausted(meter, identified, VmBudgetDimension.NestedLoadBytes);
            }

            bytes += length;
        }

        if (!meter.TryCharge(VmBudgetDimension.NestedLoadBytes, length))
        {
            return Exhausted(meter, identified, meter.FailedDimension, meter.FailedScope);
        }

        // The nested bytes take the ordinary verification path, under the requesting operation's
        // remaining allowance rather than under a runtime ceiling.
        var before = meter.RemainingSnapshot[VmBudgetDimension.VerifierWork];

        var verification = runtime.VerifyCore(
            answer.Descriptor,
            answer.Payload,
            request.CancellationToken,
            identified,
            VmArtifactOrigin.GuestInitiated,
            meter);

        // Measured rather than predicted, because how much verifier work an artifact costs is a
        // property of the artifact and the profile's verifier, and the core cannot know it in
        // advance. The pre-check above refuses a request once nothing is left; this one is what
        // makes the total a bound rather than a suggestion, and it terminates the operation whose
        // loads spent it.
        var after = meter.RemainingSnapshot[VmBudgetDimension.VerifierWork];
        var spent = before > after ? before - after : 0;

        lock (gate)
        {
            verifierWork = verifierWork > ulong.MaxValue - spent ? ulong.MaxValue : verifierWork + spent;

            if (verifierWork > bounds.VerifierWork)
            {
                return Exhausted(meter, identified, VmBudgetDimension.VerifierWork);
            }
        }

        return Project(verification, identified);
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0008; IP=Low; Security=Low; Resources=1; Fingerprint=6DF904
    // Broiler-Human:        PENDING
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

    /// <summary>
    /// Refuses a nested load for want of allowance, and latches the refusal onto the requesting
    /// operation.
    /// </summary>
    /// <remarks>
    /// The latch is the point. A nested exhaustion is terminal for the operation that caused it, and
    /// the profile is obliged to convert it rather than swallow it - but a profile that ignores the
    /// returned result and keeps running would otherwise complete normally, because the mediator's
    /// own bound checks fire before any meter charge and so leave no trace for the caller's result
    /// to pick up. Latching makes the conversion obligation enforced rather than trusted.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0008; IP=Low; Security=Medium; Resources=0; Fingerprint=64C2D0
    // Broiler-Human:        PENDING
    private static VmGuestLoadResult Exhausted(
        VmMeter meter,
        VmDiagnostics identified,
        VmBudgetDimension dimension,
        VmBudgetScope scope = VmBudgetScope.Invocation)
    {
        meter.LatchNestedRefusal(dimension, scope);

        return VmGuestLoadResult.ResourceExhaustion(
            VmMeter.ReasonFor(dimension),
            identified
                .WithOutcome(VmStage.GuestInitiatedLoad, VmOutcome.ResourceExhaustion, VmReason.CeilingReached, VmInitiator.Core)
                .WithExhaustion(dimension, scope));
    }
}
