using Broiler.VM.Abstractions;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// Reclamation: what a disposed instance gives back, and what it deliberately does not.
/// </summary>
/// <remarks>
/// The lifecycle promises that runtime, artifact and profile-owned state is reclaimed on dispose
/// and reaches a measured plateau under repeated load, run and evict cycles. A retained-bytes
/// report commits at the instance, runtime and aggregate levels alike, so dropping the instance
/// level alone would reclaim nothing outside it, and a host cycling instances would watch its
/// runtime climb toward a ceiling while nothing was actually held.
/// </remarks>
public sealed class ReclamationTests
{
    [Fact]
    public void A_Disposed_Instance_Gives_Back_The_Live_Bytes_It_Held()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.RetainThenRelease(0));

        var before = runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes);

        var instance = FixtureComposition.Instantiate(runtime, artifact);

        // Retain without releasing: the fixture reports 4096 bytes held and never gives them back
        // on its own, so only disposal can reclaim them.
        var retaining = FixtureComposition.Verify(
            runtime,
            FixtureArtifactWriter.Write(
                [4096],
                [FixtureFormat.OpRetain, 0, FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn]));

        using var holder = FixtureComposition.Instantiate(runtime, retaining);
        Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(holder).Outcome);

        var held = runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes);
        Assert.True(held >= before + 4096, $"expected the retention to be visible, saw {held}");

        holder.Dispose();

        var after = runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes);
        Assert.True(after < held, $"disposal reclaimed nothing: {held} before, {after} after");

        instance.Dispose();
    }

    [Fact]
    public void Repeated_Instantiate_Run_Dispose_Cycles_Reach_A_Plateau()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var payload = FixtureArtifactWriter.Write(
            [4096],
            [FixtureFormat.OpRetain, 0, FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn]);

        var artifact = FixtureComposition.Verify(runtime, payload);

        ulong afterFirst = 0;

        for (var cycle = 0; cycle < 8; cycle++)
        {
            var instance = FixtureComposition.Instantiate(runtime, artifact);
            Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);
            instance.Dispose();

            var live = runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes);

            if (cycle == 0)
            {
                afterFirst = live;
                continue;
            }

            // A plateau, not a climb. Without reclamation this would grow by 4096 per cycle and
            // would eventually refuse an instantiation that has nothing wrong with it.
            Assert.Equal(afterFirst, live);
        }
    }

    [Fact]
    public void An_Allowance_Is_Never_Given_Back_By_Disposal()
    {
        // The other half of the rule, and the more important one: fuel spent by an instance stays
        // spent. If disposal refunded an allowance, a guest could loop instantiate-run-dispose and
        // never exhaust anything.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Spin(1000));

        var instance = FixtureComposition.Instantiate(runtime, artifact);
        Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);

        var spent = runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.Fuel);
        Assert.True(spent >= 1000, $"expected the spin to be charged, saw {spent}");

        instance.Dispose();

        Assert.Equal(spent, runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.Fuel));
    }

    [Theory]
    [InlineData(FixtureVmProfileVariant.BreachesBoundDuringInstantiation)]
    [InlineData(FixtureVmProfileVariant.BreachesBoundThenPollsDuringInstantiation)]
    [InlineData(FixtureVmProfileVariant.BreachesBoundThenParksDuringInstantiation)]
    public void A_Refused_Instantiation_Gives_Back_The_Live_Bytes_It_Retained(FixtureVmProfileVariant variant)
    {
        // The profile reported bytes retained and was then refused for breaking its poll bound. No
        // instance was built around the instance level those bytes were committed through, so no
        // disposal will ever release them: the core has to, at the runtime and at the aggregate.
        using var parent = Parent();
        var catalog = FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(variant));
        var runtime = FixtureComposition.Runtime(catalog, FixtureComposition.Options(parent: parent));
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));

        var before = runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes);
        var parentBefore = parent.GetSnapshot().Consumed(VmBudgetDimension.LiveBytes);

        // Three times, so a leak would show as a climb and not only as an offset.
        for (var attempt = 0; attempt < 3; attempt++)
        {
            var result = runtime.Instantiate(artifact, CancellationToken.None);

            Assert.Equal(VmOutcome.ProfileFault, result.Outcome);
            Assert.Equal(VmReason.CancellationPollBoundExceeded, result.Reason);
            Assert.Equal(before, runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes));
        }

        // The aggregate is where a leak would outlive the runtime: disposing a runtime gives back
        // what its instances held, and there were none.
        runtime.Dispose();

        Assert.Equal(parentBefore, parent.GetSnapshot().Consumed(VmBudgetDimension.LiveBytes));
    }

    [Fact]
    public void An_Instantiation_Refused_For_Exhaustion_Gives_Back_The_Live_Bytes_It_Retained()
    {
        // The exhaustion path had the same gap as the poll-bound one: the retention before the
        // refused charge was committed and never released. The instance's Fuel ceiling is stated
        // below what the profile charges while instantiating, so the charge is refused.
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.BreachesBoundDuringInstantiation));

        using var runtime = FixtureComposition.Runtime(catalog);
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));

        var before = runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes);

        var result = runtime.Instantiate(
            artifact,
            VmLimitOverrides.Of(VmBudgetDimension.Fuel, FixtureVmExecutor.InstantiationCharge / 2),
            CancellationToken.None);

        Assert.Equal(VmOutcome.ResourceExhaustion, result.Outcome);
        Assert.Equal(VmBudgetDimension.Fuel, result.Diagnostics.ExhaustedDimension);
        Assert.Equal(before, runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes));
    }

    [Fact]
    public void An_Instantiation_Refused_For_A_Host_Failure_Gives_Back_The_Live_Bytes_It_Retained()
    {
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.CallsHostDuringInstantiation));

        // First the same profile with a handler that completes, so the retention is shown to be
        // real: the instance holds it until it is disposed.
        using (var runtime = FixtureComposition.Runtime(catalog))
        {
            var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));
            var before = runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes);

            var instance = FixtureComposition.Instantiate(runtime, artifact);

            Assert.Equal(
                before + FixtureVmExecutor.InstantiationRetention,
                runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes));

            instance.Dispose();
        }

        // Then with a handler that fails terminally: refused, and nothing left counted.
        using (var runtime = FixtureComposition.Runtime(
                   catalog,
                   FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(ThrowingHostCall))))
        {
            var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));
            var before = runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes);

            var result = runtime.Instantiate(artifact, CancellationToken.None);

            Assert.Equal(VmOutcome.HostFailure, result.Outcome);
            Assert.Equal(before, runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes));
        }
    }

    [Fact]
    public void A_Cancelled_Instantiation_Gives_Back_The_Live_Bytes_It_Retained()
    {
        // The profile retains, makes a call that completes, and answers that it created the
        // instance, under a token its caller had already cancelled. The answer is a cancellation,
        // so nothing holds the retention and nothing may stay counted for it.
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.CallsHostDuringInstantiation));

        using var runtime = FixtureComposition.Runtime(catalog);
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));

        var before = runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes);

        var result = runtime.Instantiate(artifact, new CancellationToken(canceled: true));

        Assert.Equal(VmOutcome.Cancellation, result.Outcome);
        Assert.Equal(before, runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.LiveBytes));
    }

    private static VmHostCallOutcome ThrowingHostCall(ReadOnlySpan<long> arguments, out long result)
    {
        result = 0;
        throw new InvalidOperationException("host defect");
    }

    private static VmAggregateBudget Parent()
    {
        var builder = System.Collections.Immutable.ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (!VmBudgetDimensions.CarriesAggregateScope(dimension))
            {
                continue;
            }

            builder.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.Value(dimension, 8),
                VmBudgetDimension.Fuel => VmCeilingSpec.Value(dimension, 10_000_000),
                VmBudgetDimension.HostCalls => VmCeilingSpec.Value(dimension, 1_000_000),
                _ => VmCeilingSpec.Value(dimension, 1_000_000_000),
            });
        }

        return VmAggregateBudget.Create(builder.ToImmutable());
    }
}
