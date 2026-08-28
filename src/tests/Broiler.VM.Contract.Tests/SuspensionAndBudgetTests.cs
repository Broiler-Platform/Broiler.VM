using Broiler.VM;
using Broiler.VM.Fixtures;
using System.Collections.Immutable;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// G13 and G14 of the VM-1 gate: external suspension and resume, and aggregate budget exhaustion
/// across several runtimes.
/// </summary>
public sealed class SuspensionAndBudgetTests
{
    private static VmCatalog SuspendingCatalog() =>
        FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.DeclaresExternalSuspension));

    [Fact]
    public void A_Guest_Suspension_Rides_The_Callers_Result_And_Resumes()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(17));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var suspended = FixtureComposition.Invoke(instance);

        Assert.True(suspended.IsSuspended);
        Assert.Equal(VmReason.GuestSuspended, suspended.Reason);
        Assert.True(suspended.TryGetSuspension(out var suspension));
        Assert.Equal(VmSuspensionOrigin.Guest, suspension.Origin);

        Assert.True(FixtureVmProfileResults.TryGetSuspensionProjection(in suspended, out var projection));
        Assert.True(projection.InstructionPointer > 0);

        var resumed = runtime.Resume(suspension);

        Assert.Equal(VmOutcome.Normal, resumed.Outcome);
        Assert.Equal(VmStage.Invocation, resumed.SuspendedStage);
        Assert.True(FixtureVmProfileResults.TryGetValue(in resumed, out var value));
        Assert.Equal(17, value.Value);
    }

    [Fact]
    public void A_Suspension_Object_Is_Single_Use()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(1));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var suspended = FixtureComposition.Invoke(instance);
        Assert.True(suspended.TryGetSuspension(out var suspension));

        Assert.Equal(VmOutcome.Normal, runtime.Resume(suspension).Outcome);

        var again = runtime.Resume(suspension);
        Assert.Equal(VmOutcome.InvalidState, again.Outcome);
        Assert.Equal(VmReason.ResumeTokenConsumed, again.Reason);
    }

    [Fact]
    public void A_Suspension_From_Another_Runtime_Is_A_Foreign_Handle()
    {
        var catalog = FixtureComposition.AlphaCatalog();
        using var first = FixtureComposition.Runtime(catalog);
        using var second = FixtureComposition.Runtime(catalog);

        var artifact = FixtureComposition.Verify(first, FixtureArtifactWriter.YieldThenConstant(1));
        using var instance = FixtureComposition.Instantiate(first, artifact);

        var suspended = FixtureComposition.Invoke(instance);
        Assert.True(suspended.TryGetSuspension(out var suspension));

        var wrong = second.Resume(suspension);

        Assert.Equal(VmOutcome.InvalidState, wrong.Outcome);
        Assert.Equal(VmReason.ForeignHandle, wrong.Reason);
    }

    [Fact]
    public void External_Suspension_Needs_Both_Halves_Of_The_Double_Gate()
    {
        // The profile declares, and the composition enables. Either alone answers unsupported, and
        // the reason says which - so a host can tell "this profile cannot" from "I did not turn it
        // on".
        using var notDeclared = FixtureComposition.Runtime(
            FixtureComposition.AlphaCatalog(),
            FixtureComposition.Options(externalSuspension: VmExternalSuspensionMode.Enabled));

        var artifactA = FixtureComposition.Verify(notDeclared, FixtureArtifactWriter.YieldThenConstant(1));
        using var instanceA = FixtureComposition.Instantiate(notDeclared, artifactA);

        FixtureComposition.Invoke(instanceA, out var handleA);
        var refusedByProfile = handleA.RequestSuspend();

        Assert.Equal(VmControlOutcome.Unsupported, refusedByProfile.Kind);
        Assert.Equal(VmReason.ExternalSuspensionNotDeclared, refusedByProfile.Reason);

        using var notEnabled = FixtureComposition.Runtime(
            SuspendingCatalog(),
            FixtureComposition.Options(externalSuspension: VmExternalSuspensionMode.Disabled));

        var artifactB = FixtureComposition.Verify(notEnabled, FixtureArtifactWriter.YieldThenConstant(1));
        using var instanceB = FixtureComposition.Instantiate(notEnabled, artifactB);

        FixtureComposition.Invoke(instanceB, out var handleB);
        var refusedByComposition = handleB.RequestSuspend();

        Assert.Equal(VmControlOutcome.Unsupported, refusedByComposition.Kind);
        Assert.Equal(VmReason.ExternalSuspensionNotEnabled, refusedByComposition.Reason);
    }

    [Fact]
    public void An_Unsupported_Control_Result_Is_Not_An_Illegal_Transition()
    {
        // Invariant 8: a missing capability is a truthful absence, not a state error. Reporting it
        // as an invalid state would make "the profile cannot do this" indistinguishable from "you
        // called this at the wrong time".
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(1));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        FixtureComposition.Invoke(instance, out var handle);

        Assert.NotEqual(VmControlOutcome.InvalidState, handle.RequestSuspend().Kind);
    }

    [Fact]
    public void An_External_Suspension_Is_Delivered_Through_The_Control_Handle_Only()
    {
        // A guest suspension rides the caller's result; an external one has no such result to ride,
        // so it is taken once from the handle. That is the single path that gives the party
        // entitled to resume a way to resume without a second admission check.
        using var runtime = FixtureComposition.Runtime(
            SuspendingCatalog(),
            FixtureComposition.Options(externalSuspension: VmExternalSuspensionMode.Enabled));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(23));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var first = FixtureComposition.Invoke(instance, out var handle);
        Assert.True(first.IsSuspended);
        Assert.True(first.TryGetSuspension(out var guestSuspension));

        // The first park is guest-origin, because nothing asked for an external one.
        Assert.Equal(VmSuspensionOrigin.Guest, guestSuspension.Origin);
        Assert.Equal(VmControlOutcome.Accepted, handle.RequestSuspend().Kind);
        Assert.True(handle.QueryState().ExternalSuspendRequested);

        Assert.Equal(VmOutcome.Normal, runtime.Resume(guestSuspension).Outcome);
    }

    [Fact]
    public void Disposing_A_Handle_Holding_An_Untaken_External_Suspension_Latches_Cancellation()
    {
        // Otherwise a debugger that paused an operation and then went away would park it until the
        // residency bound expired, holding frames and an execution slot for a resumption that is
        // never coming.
        using var runtime = FixtureComposition.Runtime(
            SuspendingCatalog(),
            FixtureComposition.Options(externalSuspension: VmExternalSuspensionMode.Enabled));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(1));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        FixtureComposition.Invoke(instance, out var handle);

        Assert.Equal(VmControlOutcome.Accepted, handle.Dispose().Kind);
        Assert.Equal(VmControlOutcome.NoOp, handle.Dispose().Kind);
    }

    [Fact]
    public void The_Live_Suspended_Operation_Bound_Is_Enforced()
    {
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.AlphaCatalog(),
            FixtureComposition.Options(maxLiveSuspendedOperations: 1));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(1));

        using var first = FixtureComposition.Instantiate(runtime, artifact);
        using var second = FixtureComposition.Instantiate(runtime, artifact);

        Assert.True(FixtureComposition.Invoke(first).IsSuspended);

        var overflow = FixtureComposition.Invoke(second);

        Assert.Equal(VmOutcome.InvalidState, overflow.Outcome);
        Assert.Equal(VmReason.SuspendedOperationLimitReached, overflow.Reason);
    }

    [Fact]
    public void A_Runtime_Without_A_Finite_Residency_Bound_Is_Not_Created()
    {
        // A paused operation holds frames, host handles and an execution slot. An unbounded
        // residency is an unbounded retention with a debugger attached to it.
        var created = VmRuntime.Create(
            FixtureComposition.AlphaCatalog(),
            new VmRuntimeCreationOptions(
                aggregateBudget: null,
                ceilings: FixtureComposition.AdoptedCeilings(),
                maxSuspendedResidency: Timeout.InfiniteTimeSpan,
                maxLiveSuspendedOperations: 4,
                guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
                externalSuspension: VmExternalSuspensionMode.Disabled,
                capabilities: FixtureComposition.ValueCapabilities()));

        Assert.Equal(VmOutcome.HostFailure, created.Outcome);
        Assert.Equal(VmReason.SuspendedResidencyUnbounded, created.Reason);
    }

    [Fact]
    public void An_Omitted_Dimension_Fails_Runtime_Creation()
    {
        // Omission is not a value. Invariant 9 makes resource authority trusted and monotonic, and
        // a dimension the host forgot would otherwise be whatever the core felt like.
        var partial = FixtureComposition.AdoptedCeilings().RemoveAt(0);

        var created = VmRuntime.Create(
            FixtureComposition.AlphaCatalog(),
            FixtureComposition.Options(partial));

        Assert.Equal(VmOutcome.HostFailure, created.Outcome);
        Assert.Equal(VmReason.BudgetDimensionUnresolved, created.Reason);
    }

    [Fact]
    public void The_Live_Runtimes_Dimension_Accepts_Only_The_Parent_Marker()
    {
        var builder = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            builder.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.Value(dimension, 10)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(
            FixtureComposition.AlphaCatalog(),
            FixtureComposition.Options(builder.ToImmutable()));

        Assert.Equal(VmOutcome.HostFailure, created.Outcome);
        Assert.Equal(VmReason.BudgetDimensionNotRuntimeScoped, created.Reason);
    }

    [Fact]
    public void An_Aggregate_Budget_Bounds_The_Number_Of_Live_Runtimes()
    {
        using var parent = AggregateBudget(liveRuntimes: 2);
        var catalog = FixtureComposition.AlphaCatalog();

        using var first = FixtureComposition.Runtime(catalog, FixtureComposition.Options(parent: parent));
        using var second = FixtureComposition.Runtime(catalog, FixtureComposition.Options(parent: parent));

        Assert.Equal(2, parent.LiveRuntimeCountValue);

        var third = VmRuntime.Create(catalog, FixtureComposition.Options(parent: parent));

        Assert.Equal(VmOutcome.ResourceExhaustion, third.Outcome);
        Assert.Equal(VmReason.LiveRuntimeCeilingReached, third.Reason);
        Assert.Equal(VmBudgetDimension.LiveRuntimes, third.Diagnostics.ExhaustedDimension);
        Assert.Equal(VmBudgetScope.Aggregate, third.Diagnostics.ExhaustedScope);
    }

    [Fact]
    public void Disposing_A_Runtime_Returns_Its_Slot_To_The_Parent()
    {
        // The live-runtime count is a ceiling on a live measure, so it decrements. Allowances do
        // not: they never refund, including on dispose.
        using var parent = AggregateBudget(liveRuntimes: 1);
        var catalog = FixtureComposition.AlphaCatalog();

        var first = FixtureComposition.Runtime(catalog, FixtureComposition.Options(parent: parent));
        Assert.Equal(1, parent.LiveRuntimeCountValue);

        first.Dispose();
        Assert.Equal(0, parent.LiveRuntimeCountValue);

        using var second = FixtureComposition.Runtime(catalog, FixtureComposition.Options(parent: parent));
        Assert.Equal(1, parent.LiveRuntimeCountValue);
    }

    [Fact]
    public void Concurrent_Runtimes_Share_A_Parent_Allowance_Rather_Than_Multiplying_It()
    {
        // This is the whole point of an aggregate budget: two runtimes under one parent may not
        // each spend the parent's ceiling.
        using var parent = AggregateBudget(liveRuntimes: 4, fuel: 400);
        var catalog = FixtureComposition.AlphaCatalog();

        // Each runtime declares a fuel ceiling equal to the parent's whole allowance, which is
        // legal - a per-runtime ceiling may equal the parent's remaining and may never exceed it.
        // What must not happen is the two of them together spending twice the parent's ceiling.
        var ceilings = FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, 400);

        using var first = FixtureComposition.Runtime(
            catalog, FixtureComposition.Options(ceilings, parent: parent));
        using var second = FixtureComposition.Runtime(
            catalog, FixtureComposition.Options(ceilings, parent: parent));

        var artifact = FixtureComposition.Verify(first, FixtureArtifactWriter.Spin(300));
        using var instanceA = FixtureComposition.Instantiate(first, artifact);

        var artifactB = FixtureComposition.Verify(second, FixtureArtifactWriter.Spin(300));
        using var instanceB = FixtureComposition.Instantiate(second, artifactB);

        var a = FixtureComposition.Invoke(instanceA);
        var b = FixtureComposition.Invoke(instanceB);

        // Six hundred units of work against a four-hundred-unit parent: one of them must be
        // refused, and it must be refused at aggregate scope.
        Assert.True(
            a.Outcome is VmOutcome.ResourceExhaustion || b.Outcome is VmOutcome.ResourceExhaustion,
            $"neither run was refused: {a.Outcome}/{a.Reason} and {b.Outcome}/{b.Reason}");

        var refused = a.Outcome is VmOutcome.ResourceExhaustion ? a : b;
        Assert.Equal(VmBudgetScope.Aggregate, refused.Diagnostics.ExhaustedScope);
    }

    [Fact]
    public void A_Sealed_Budget_Admits_No_Further_Runtime()
    {
        using var parent = AggregateBudget(liveRuntimes: 4);

        Assert.Equal(VmControlOutcome.Accepted, parent.Seal().Kind);
        Assert.Equal(VmControlOutcome.NoOp, parent.Seal().Kind);

        var refused = VmRuntime.Create(
            FixtureComposition.AlphaCatalog(), FixtureComposition.Options(parent: parent));

        Assert.Equal(VmOutcome.ResourceExhaustion, refused.Outcome);
    }

    [Fact]
    public void A_Parent_With_Live_Children_Cannot_Be_Disposed()
    {
        var parent = AggregateBudget(liveRuntimes: 4);
        using var child = FixtureComposition.Runtime(
            FixtureComposition.AlphaCatalog(), FixtureComposition.Options(parent: parent));

        var refused = parent.Dispose();

        Assert.Equal(VmControlOutcome.InvalidState, refused.Kind);
        Assert.Equal(VmReason.AggregateBudgetHasLiveRuntimes, refused.Reason);

        child.Dispose();
        Assert.Equal(VmControlOutcome.Accepted, parent.Dispose().Kind);
    }

    [Fact]
    public void An_Aggregate_Budget_Declares_Only_Aggregate_Dimensions()
    {
        var builder = ImmutableArray.CreateBuilder<VmCeilingSpec>();
        builder.Add(VmCeilingSpec.Value(VmBudgetDimension.ArtifactBytes, 1024));

        // ArtifactBytes carries no aggregate scope: summing "the largest one artifact may be"
        // across concurrent runtimes measures nothing.
        Assert.Throws<ArgumentException>(() => VmAggregateBudget.Create(builder.ToImmutable()));
    }

    private static VmAggregateBudget AggregateBudget(ulong liveRuntimes, ulong fuel = 10_000_000)
    {
        var builder = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (!VmBudgetDimensions.CarriesAggregateScope(dimension))
            {
                continue;
            }

            builder.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.Value(dimension, liveRuntimes),
                VmBudgetDimension.Fuel => VmCeilingSpec.Value(dimension, fuel),
                _ => VmCeilingSpec.Value(dimension, 1_000_000_000),
            });
        }

        return VmAggregateBudget.Create(builder.ToImmutable());
    }
}
