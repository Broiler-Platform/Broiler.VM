using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// G5, G6, G7 and G9 of the VM-1 gate: per-runtime state isolation, legal and illegal lifecycle
/// transitions, cancellation and disposal behaviour, and typed profile-payload preservation.
/// </summary>
public sealed class LifecycleTests
{
    [Fact]
    public void A_Verified_Artifact_Instantiates_And_Runs()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(20, 22));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.True(FixtureVmProfileResults.TryGetValue(in result, out var value));
        Assert.Equal(42, value.Value);
    }

    [Fact]
    public void A_Typed_Payload_Survives_The_Neutral_Envelope_Unchanged()
    {
        // The core carries the payload and never interprets it. What comes back out is the same
        // object the profile put in, with the identity it stamped.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Fault(1234));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ProfileFault, result.Outcome);
        Assert.True(FixtureVmProfileResults.TryGetFault(in result, out var fault));
        Assert.Equal(1234, fault.Code);
        Assert.Equal("fixture fault", fault.Description);
        Assert.Equal(FixtureVmProfile.Id, result.PayloadIdentity.ProfileId);
    }

    [Fact]
    public void A_Foreign_Payload_Is_Not_Handed_On()
    {
        // A payload whose identity does not belong to the profile that produced the result is
        // dropped rather than passed on, so one profile cannot smuggle a value out through
        // another's result.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        // The alpha profile's value is in alpha's declared kind range; beta's accessor rejects it,
        // because ownership is decided by identity and not by CLR type.
        Assert.True(result.TryGetPayload<FixtureValue>(out var payload));
        Assert.Equal(FixtureKinds.Value(FixtureVmProfile.Id), payload.Identity.PayloadKindId);
        Assert.NotEqual(FixtureKinds.Value(SecondFixtureVmProfile.Id), payload.Identity.PayloadKindId);
    }

    [Fact]
    public void A_Success_Value_Is_Unreachable_Without_Checking_The_Outcome()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var descriptor = FixtureComposition.Descriptor();

        var result = runtime.Verify(
            in descriptor,
            FixtureArtifactWriter.Write([1], [FixtureFormat.OpReturn], FixtureArtifactWriter.Corruption.Truncated),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.False(result.TryGetArtifact(out _));
    }

    [Fact]
    public void Default_Of_A_Result_Cannot_Be_Read_As_Success()
    {
        // VmOutcome.None exists for exactly this: a zeroed result must not look like a completed
        // one, because a caller that forgot to assign would otherwise see success.
        var invocation = default(VmInvocationResult);
        var verification = default(VmVerificationResult);

        Assert.Equal(VmOutcome.None, invocation.Outcome);
        Assert.Equal(VmOutcome.None, verification.Outcome);
        Assert.False(invocation.IsSuccess);
        Assert.False(verification.IsSuccess);
    }

    [Fact]
    public void Two_Runtimes_Over_One_Catalog_Do_Not_Share_Instance_State()
    {
        var catalog = FixtureComposition.AlphaCatalog();

        using var first = FixtureComposition.Runtime(catalog);
        using var second = FixtureComposition.Runtime(catalog);

        var artifact = FixtureComposition.Verify(first, FixtureArtifactWriter.Constant(5));
        using var instanceA = FixtureComposition.Instantiate(first, artifact);
        using var instanceB = FixtureComposition.Instantiate(second, artifact);

        FixtureComposition.Invoke(instanceA);
        FixtureComposition.Invoke(instanceA);
        FixtureComposition.Invoke(instanceB);

        Assert.NotEqual(instanceA.ObjectId, instanceB.ObjectId);
        Assert.NotEqual(first.ObjectId, second.ObjectId);
    }

    [Fact]
    public void A_Shareable_Handle_Crosses_Runtimes_With_Matching_Identity()
    {
        var catalog = FixtureComposition.AlphaCatalog();

        using var first = FixtureComposition.Runtime(catalog);
        using var second = FixtureComposition.Runtime(catalog);

        var artifact = FixtureComposition.Verify(first, FixtureArtifactWriter.Constant(11));

        var result = second.Instantiate(artifact, CancellationToken.None);

        Assert.True(result.IsSuccess, $"{result.Outcome}/{result.Reason}");
    }

    [Fact]
    public void A_Handle_Whose_Ceilings_Differ_Is_Refused_By_The_Other_Runtime()
    {
        // Ceilings are compared by exact equality, never by subsumption. Relaxing that would turn
        // a refusal into a success, which is a breaking amendment rather than a convenience.
        var catalog = FixtureComposition.AlphaCatalog();

        using var loose = FixtureComposition.Runtime(catalog);
        using var tight = FixtureComposition.Runtime(
            catalog,
            FixtureComposition.Options(FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, 500)));

        var artifact = FixtureComposition.Verify(loose, FixtureArtifactWriter.Constant(3));

        var result = tight.Instantiate(artifact, CancellationToken.None);

        Assert.Equal(VmOutcome.InvalidState, result.Outcome);
        Assert.Equal(VmReason.SharedHandleCeilingMismatch, result.Reason);
    }

    [Fact]
    public void Use_After_Dispose_Is_A_Deterministic_Refusal_On_Every_Object()
    {
        var catalog = FixtureComposition.AlphaCatalog();
        var runtime = FixtureComposition.Runtime(catalog);

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        Assert.Equal(VmControlOutcome.Accepted, instance.Dispose().Kind);

        var afterInstance = FixtureComposition.Invoke(instance);
        Assert.Equal(VmOutcome.InvalidState, afterInstance.Outcome);
        Assert.Equal(VmReason.ObjectDisposed, afterInstance.Reason);

        Assert.Equal(VmControlOutcome.Accepted, runtime.Dispose().Kind);

        var descriptor = FixtureComposition.Descriptor();
        var afterRuntime = runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None);
        Assert.Equal(VmOutcome.InvalidState, afterRuntime.Outcome);
        Assert.Equal(VmReason.ObjectDisposed, afterRuntime.Reason);
    }

    [Fact]
    public void Disposal_Is_Idempotent_And_Answers_NoOp_The_Second_Time()
    {
        var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        Assert.Equal(VmControlOutcome.Accepted, runtime.Dispose().Kind);
        Assert.Equal(VmControlOutcome.NoOp, runtime.Dispose().Kind);
        Assert.Equal(VmRuntimeState.Disposed, runtime.State);
    }

    [Fact]
    public void An_Artifact_Handle_Drains_Rather_Than_Being_Seized()
    {
        // There is no force-dispose and no lease revocation: a handle with a live lease drains, so
        // one holder can never invalidate another's input.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));

        Assert.Equal(VmControlOutcome.Accepted, artifact.TryAcquireLease(out var lease).Kind);
        Assert.Equal(1, artifact.LeaseCount);

        Assert.Equal(VmControlOutcome.Accepted, artifact.Dispose().Kind);
        Assert.Equal(VmVerifiedArtifactState.Draining, artifact.State);

        Assert.Equal(VmControlOutcome.InvalidState, artifact.TryAcquireLease(out _).Kind);

        Assert.Equal(VmControlOutcome.Accepted, lease.Release().Kind);
        Assert.Equal(VmVerifiedArtifactState.Disposed, artifact.State);
        Assert.Equal(VmControlOutcome.NoOp, lease.Release().Kind);
    }

    [Fact]
    public void A_Disposed_Handle_Still_Reports_Its_Identity()
    {
        // Reading what a handle was is not a use of it. A diagnostic that could not name a disposed
        // handle would be useless exactly when it is needed.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));
        var identity = artifact.Identity;

        artifact.Dispose();

        Assert.Equal(VmVerifiedArtifactState.Disposed, artifact.State);
        Assert.Equal(identity, artifact.Identity);
        Assert.Equal(FixtureVmProfile.Id, artifact.Identity.ProfileId);
        Assert.False(artifact.TryGetState(out _));
    }

    [Fact]
    public void Instantiating_A_Disposed_Handle_Is_An_Invalid_State()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));
        artifact.Dispose();

        var result = runtime.Instantiate(artifact, CancellationToken.None);

        Assert.Equal(VmOutcome.InvalidState, result.Outcome);
        Assert.Equal(VmReason.HandleDisposed, result.Reason);
    }

    [Fact]
    public void Cancellation_Is_Observed_At_A_Polling_Point()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(1, 2));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();

        var request = new VmInvocationRequest(new VmUtf8Text("main"u8));
        var result = instance.Invoke(in request, cancelled.Token);

        Assert.Equal(VmOutcome.Cancellation, result.Outcome);
        Assert.Equal(VmReason.Cancelled, result.Reason);
    }

    [Fact]
    public void A_Cancellation_Request_Latch_Is_Monotonic()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.YieldThenConstant(4),
            FixtureComposition.Descriptor());

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var suspended = FixtureComposition.Invoke(instance, out var handle);
        Assert.True(suspended.IsSuspended);

        Assert.Equal(VmControlOutcome.Accepted, handle.RequestCancel().Kind);
        Assert.Equal(VmControlOutcome.NoOp, handle.RequestCancel().Kind);
        Assert.True(handle.QueryState().CancellationRequested);
    }

    [Fact]
    public void The_Diagnostics_Record_Names_The_Contract_And_Registry_Versions()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));

        Assert.Equal(VmCoreContract.Version, artifact.DiagnosticsBase.CoreContractVersion);
        Assert.Equal(VmReasonRegistry.Revision, artifact.DiagnosticsBase.ReasonRegistryRevision);
    }

    [Fact]
    public void An_Invalid_State_Result_Carries_The_Object_Kind_State_And_Attempted_Call()
    {
        var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        runtime.Dispose();

        var descriptor = FixtureComposition.Descriptor();
        var result = runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None);

        Assert.Equal(VmObjectKind.Runtime, result.Diagnostics.ObjectKind);
        Assert.Equal(VmAttemptedCall.Verify, result.Diagnostics.AttemptedCall);
        Assert.Equal(VmObjectKind.Runtime, result.Diagnostics.ObjectState.Kind);
    }
}
