using Broiler.VM;
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
}
