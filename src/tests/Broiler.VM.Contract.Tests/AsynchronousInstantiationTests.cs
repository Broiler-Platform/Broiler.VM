using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// Asynchronous instantiation, from the park to the one end of it that publishes an instance and
/// every end that does not.
/// </summary>
/// <remarks>
/// <para>
/// ADR 0009 admits the transition for a profile that declares it, and ADR 0004 fixes what an
/// instance does across it. It is published only when a resume completes the instantiation
/// normally. An instantiation that ends any other way, or is abandoned while suspended, goes to
/// Faulted and then Disposed, and nobody is ever handed it.
/// </para>
/// <para>
/// Until this suite existed the declared path had no test at all. A resumed instantiation was
/// published still holding the core's placeholder in place of the profile's state, so its first
/// invocation was a contract violation. One that ended otherwise left its pending instance
/// registered, with its lease and its retained bytes, until the runtime was disposed.
/// </para>
/// </remarks>
public sealed class AsynchronousInstantiationTests
{
    [Fact]
    public void A_Resumed_Instantiation_Publishes_An_Instance_That_Runs_Against_The_Profiles_Own_State()
    {
        // The fixture refuses to invoke against any state but its own, so an instance published
        // around the placeholder answers its first invocation as a contract violation. This one
        // runs the artifact and returns its constant.
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.CallsHostThenParksDuringInstantiation));

        using var runtime = FixtureComposition.Runtime(catalog);
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(7));
        var before = LiveBytes(runtime);

        var parked = runtime.Instantiate(artifact, CancellationToken.None);

        Assert.Equal(VmOutcome.Suspension, parked.Outcome);
        Assert.Equal(VmReason.InstantiationSuspended, parked.Reason);
        Assert.False(parked.TryGetInstance(out _));
        Assert.True(parked.TryGetSuspension(out var suspension));
        Assert.Equal(VmSuspensionOrigin.Instantiation, suspension.Origin);

        var resumed = runtime.Resume(suspension);

        Assert.Equal(VmOutcome.Normal, resumed.Outcome);
        Assert.True(resumed.TryGetInstance(out var instance));
        Assert.Equal(VmInstanceState.Live, instance.State);

        var invoked = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, invoked.Outcome);
        Assert.True(FixtureVmProfileResults.TryGetValue(in invoked, out var value));
        Assert.Equal(7, value.Value);

        // The retention made before the park belongs to the instance that was published, and goes
        // back when that instance is disposed.
        Assert.Equal(before + FixtureVmExecutor.InstantiationRetention, LiveBytes(runtime));

        instance.Dispose();

        Assert.Equal(before, LiveBytes(runtime));
    }

    [Fact]
    public void An_Instantiation_May_Park_Again_And_Publishes_Only_On_The_Resume_That_Completes_It()
    {
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.DeclaresAsynchronousInstantiation));

        using var runtime = FixtureComposition.Runtime(catalog);
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(3));

        Assert.True(runtime.Instantiate(artifact, CancellationToken.None).TryGetSuspension(out var first));

        var again = runtime.Resume(first);

        Assert.Equal(VmOutcome.Suspension, again.Outcome);
        Assert.False(again.TryGetInstance(out _));
        Assert.True(again.TryGetSuspension(out var second));

        var resumed = runtime.Resume(second);

        Assert.Equal(VmOutcome.Normal, resumed.Outcome);
        Assert.True(resumed.TryGetInstance(out var instance));

        using (instance)
        {
            Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);
        }
    }

    [Fact]
    public void A_Resumed_Instantiation_That_Completes_Without_Its_State_Is_A_Contract_Violation_And_Publishes_Nothing()
    {
        // Completed is the answer that finishes an invocation. Finishing an instantiation with it
        // hands over no state, and it was reported Normal with the placeholder instance published.
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.CompletesParkedInstantiationWithoutState));

        using var runtime = FixtureComposition.Runtime(catalog);
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));

        Assert.True(runtime.Instantiate(artifact, CancellationToken.None).TryGetSuspension(out var suspension));

        var resumed = runtime.Resume(suspension);

        Assert.Equal(VmOutcome.ProfileFault, resumed.Outcome);
        Assert.Equal(VmReason.ProfileContractViolation, resumed.Reason);
        Assert.False(resumed.TryGetInstance(out _));
        AssertLeaseReleased(artifact);
    }

    [Fact]
    public void A_Resume_That_Fails_For_A_Host_Failure_Leaves_No_Instance_Behind()
    {
        // The doubling capability completes while instantiating and fails terminally on the
        // resume's call, so the instantiation parks and its completion is refused.
        var calls = 0;

        VmHostCallOutcome SecondCallFails(ReadOnlySpan<long> arguments, out long result)
        {
            result = 0;

            if (Interlocked.Increment(ref calls) > 1)
            {
                throw new InvalidOperationException("host defect");
            }

            return VmHostCallOutcome.Completed;
        }

        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.CallsHostThenParksDuringInstantiation));

        using var runtime = FixtureComposition.Runtime(
            catalog, FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(SecondCallFails)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));
        var before = LiveBytes(runtime);

        Assert.True(runtime.Instantiate(artifact, CancellationToken.None).TryGetSuspension(out var suspension));
        Assert.Equal(before + FixtureVmExecutor.InstantiationRetention, LiveBytes(runtime));

        var resumed = runtime.Resume(suspension);

        Assert.Equal(VmOutcome.HostFailure, resumed.Outcome);
        Assert.Equal(VmReason.HostCapabilityFaulted, resumed.Reason);
        Assert.False(resumed.TryGetInstance(out _));
        Assert.Equal(before, LiveBytes(runtime));
        AssertLeaseReleased(artifact);
    }

    [Fact]
    public void A_Resume_That_Exhausts_Leaves_No_Instance_Behind()
    {
        // The instantiating step charges no fuel, so it parks under an instance Fuel ceiling the
        // resume's charge exceeds.
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.CallsHostThenParksDuringInstantiation));

        using var runtime = FixtureComposition.Runtime(catalog);
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));
        var before = LiveBytes(runtime);

        var parked = runtime.Instantiate(
            artifact,
            VmLimitOverrides.Of(VmBudgetDimension.Fuel, FixtureVmExecutor.InstantiationResumeCharge / 2),
            CancellationToken.None);

        Assert.True(parked.TryGetSuspension(out var suspension));

        var resumed = runtime.Resume(suspension);

        Assert.Equal(VmOutcome.ResourceExhaustion, resumed.Outcome);
        Assert.Equal(VmBudgetDimension.Fuel, resumed.Diagnostics.ExhaustedDimension);
        Assert.False(resumed.TryGetInstance(out _));
        Assert.Equal(before, LiveBytes(runtime));
        AssertLeaseReleased(artifact);
    }

    [Fact]
    public void A_Parked_Instantiation_Cancelled_Through_Its_Runtime_Is_Abandoned_When_Its_Resume_Is_Refused()
    {
        // A runtime-wide cancellation reaches every operation, the parked instantiation among them.
        // The resume reports the cancellation and ends the instantiation there: its continuation
        // is unwound and its pending instance disposed, since nobody holds either.
        FixtureVmExecutor? executor = null;

        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(
                FixtureVmProfileVariant.CallsHostThenParksDuringInstantiation,
                environmentObserver: null,
                executorObserver: created => executor = created));

        using var runtime = FixtureComposition.Runtime(catalog);
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));
        var before = LiveBytes(runtime);

        Assert.True(runtime.Instantiate(artifact, CancellationToken.None).TryGetSuspension(out var suspension));
        Assert.Equal(VmControlOutcome.Accepted, runtime.RequestCancel().Kind);

        var resumed = runtime.Resume(suspension);

        Assert.Equal(VmOutcome.Cancellation, resumed.Outcome);
        Assert.False(resumed.TryGetInstance(out _));
        Assert.Equal(1, executor!.UnwoundCount);
        Assert.Equal(before, LiveBytes(runtime));
        AssertLeaseReleased(artifact);
    }

    [Fact]
    public void A_Parked_Instantiation_That_Outstays_Its_Residency_Is_Abandoned_And_Leaves_No_Instance_Behind()
    {
        FixtureVmExecutor? executor = null;

        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(
                FixtureVmProfileVariant.CallsHostThenParksDuringInstantiation,
                environmentObserver: null,
                executorObserver: created => executor = created));

        using var runtime = FixtureComposition.Runtime(
            catalog, FixtureComposition.Options(maxSuspendedResidency: TimeSpan.FromMilliseconds(50)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));
        var before = LiveBytes(runtime);

        Assert.True(runtime.Instantiate(artifact, CancellationToken.None).TryGetSuspension(out var suspension));

        // Nobody resumes it. The residency bound is the host's own, enforced when it polls.
        Thread.Sleep(120);

        Assert.Equal(VmControlOutcome.Accepted, runtime.PollDeadlines().Kind);
        Assert.Equal(1, executor!.UnwoundCount);
        Assert.Equal(before, LiveBytes(runtime));
        AssertLeaseReleased(artifact);

        var late = runtime.Resume(suspension);

        Assert.Equal(VmOutcome.InvalidState, late.Outcome);
        Assert.Equal(VmReason.ResumeTokenConsumed, late.Reason);
    }

    private static ulong LiveBytes(VmRuntime runtime) =>
        runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes);

    /// <summary>
    /// A handle with no lease left is disposed at once, and one with a lease drains. The pending
    /// instance holds the only lease, and gives it back only when it is disposed.
    /// </summary>
    private static void AssertLeaseReleased(VmVerifiedArtifact artifact)
    {
        artifact.Dispose();

        Assert.Equal(VmVerifiedArtifactState.Disposed, artifact.State);
    }
}
