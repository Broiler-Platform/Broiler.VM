using Broiler.VM;
using Broiler.VM.Fixtures;
using System.Collections.Immutable;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// A shared aggregate budget is honoured by concurrent runtimes rather than multiplied by them.
/// </summary>
/// <remarks>
/// The gate states it as one sentence and it is the whole point of an aggregate: a host that puts
/// eight runtimes under one budget has bought one budget, not eight. The interesting case is
/// contention - eight threads spending at once against one counter - because that is where a
/// read-then-write would let the total overrun and where nothing single-threaded would notice.
/// </remarks>
public sealed class AggregateBudgetConcurrencyTests
{
    /// <summary>
    /// Eight runtimes spending concurrently against one parent spend the parent's total once.
    /// </summary>
    [Fact]
    public void Concurrent_Runtimes_Spend_One_Shared_Total()
    {
        const int Runtimes = 8;
        const int Cycles = 20;
        const ulong SpinPerCycle = 100;

        // Deliberately far more than the work below needs, so the assertion is about the SUM being
        // shared rather than about anyone being refused.
        var parent = Parent(fuel: 10_000_000);
        var catalog = FixtureComposition.AlphaCatalog();

        Parallel.For(0, Runtimes, _ =>
        {
            using var runtime = FixtureComposition.Runtime(
                catalog, FixtureComposition.Options(parent: parent));

            var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Spin((long)SpinPerCycle));

            for (var cycle = 0; cycle < Cycles; cycle++)
            {
                var instance = FixtureComposition.Instantiate(runtime, artifact);
                Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);
                instance.Dispose();
            }

            artifact.Dispose();
        });

        var spent = parent.GetSnapshot().Consumed(VmBudgetDimension.Fuel);

        // The floor is what makes this a shared total rather than a per-runtime one: eight
        // runtimes' spins are all charged to one parent, so the parent must have seen at least
        // eight times one runtime's work. A parent that answered a per-runtime figure - or one
        // whose increments raced and were lost - would sit below it.
        var floor = Runtimes * Cycles * SpinPerCycle;

        Assert.True(
            spent >= floor,
            $"the parent recorded {spent} where at least {floor} was spent against it");

        parent.Dispose();
    }

    /// <summary>
    /// A parent's allowance is exhausted once between its runtimes, not once each.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the multiplication the gate forbids, made observable. Each runtime is created with
    /// an explicit ceiling that fits inside the parent, several race for what is left, and the
    /// total actually spent cannot exceed what one budget allowed however the interleaving fell
    /// out.
    /// </para>
    /// <para>
    /// The refusal arrives at CREATION rather than at execution, and that is the design rather than
    /// an accident of the numbers: a per-runtime ceiling may never exceed the parent's remaining
    /// allowance, so the moment the parent cannot cover another runtime's stated ceiling, no
    /// further runtime exists to refuse anything. A host that wants more runtimes than that gives
    /// each of them a smaller ceiling.
    /// </para>
    /// </remarks>
    [Fact]
    public void An_Exhausted_Parent_Refuses_Every_Runtime_Under_It()
    {
        const int Attempts = 8;
        const ulong PerRuntimeCeiling = 4_000;
        const ulong SpinPerCycle = 900;

        var parent = Parent(fuel: 20_000);
        var catalog = FixtureComposition.AlphaCatalog();
        var ceilings = FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, PerRuntimeCeiling);

        var admitted = 0;
        var refusedAtCreation = 0;
        var exhausted = 0;

        Parallel.For(0, Attempts, _ =>
        {
            var created = VmRuntime.Create(
                catalog, FixtureComposition.Options(ceilings: ceilings, parent: parent));

            if (!created.TryGetRuntime(out var runtime))
            {
                Interlocked.Increment(ref refusedAtCreation);

                Assert.Equal(VmOutcome.HostFailure, created.Outcome);
                Assert.Equal(VmReason.ExceedsParentRemaining, created.Reason);
                return;
            }

            Interlocked.Increment(ref admitted);

            using (runtime)
            {
                var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Spin((long)SpinPerCycle));

                for (var cycle = 0; cycle < 8; cycle++)
                {
                    var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

                    if (!instantiated.TryGetInstance(out var instance))
                    {
                        Interlocked.Increment(ref exhausted);
                        continue;
                    }

                    var result = FixtureComposition.Invoke(instance);
                    instance.Dispose();

                    if (result.Outcome is not VmOutcome.Normal)
                    {
                        Interlocked.Increment(ref exhausted);
                        Assert.Equal(VmOutcome.ResourceExhaustion, result.Outcome);
                    }
                }

                artifact.Dispose();
            }
        });

        Assert.True(admitted > 0, "no runtime was admitted, so nothing was tested");
        Assert.True(
            refusedAtCreation > 0 || exhausted > 0,
            "nothing was refused, so the parent's allowance was never actually reached");

        var snapshot = parent.GetSnapshot();
        var spent = snapshot.Consumed(VmBudgetDimension.Fuel);
        var ceiling = snapshot.EffectiveCeiling(VmBudgetDimension.Fuel);

        // The claim, in one line: whatever the interleaving, the parent never let more be spent
        // than it had. Runtimes each believing they had the whole allowance would show up here as a
        // total several times the ceiling.
        Assert.True(spent <= ceiling, $"the parent's allowance was overspent: {spent} of {ceiling}");

        parent.Dispose();
    }

    /// <summary>
    /// A runtime whose stated ceiling the parent can no longer cover is refused, and the refusal
    /// names the parent rather than the profile.
    /// </summary>
    [Fact]
    public void A_Spent_Parent_Admits_No_Further_Runtime()
    {
        const ulong PerRuntimeCeiling = 3_000;

        var parent = Parent(fuel: 5_000);
        var catalog = FixtureComposition.AlphaCatalog();
        var ceilings = FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, PerRuntimeCeiling);

        using (var runtime = FixtureComposition.Runtime(
                   catalog, FixtureComposition.Options(ceilings: ceilings, parent: parent)))
        {
            var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Spin(2_500));
            var instance = FixtureComposition.Instantiate(runtime, artifact);
            FixtureComposition.Invoke(instance);
            instance.Dispose();
            artifact.Dispose();
        }

        // The first runtime spent most of it, so a second of the same shape no longer fits.
        var created = VmRuntime.Create(
            catalog, FixtureComposition.Options(ceilings: ceilings, parent: parent));

        Assert.False(created.IsSuccess, "a spent parent admitted another runtime");
        Assert.Equal(VmOutcome.HostFailure, created.Outcome);
        Assert.Equal(VmReason.ExceedsParentRemaining, created.Reason);

        parent.Dispose();
    }

    /// <summary>
    /// Sealing a parent from one thread while others are creating runtimes refuses cleanly.
    /// </summary>
    /// <remarks>
    /// Sealing is a host control, and the host that seals is not the host that is mid-creation. What
    /// must not happen is a runtime that is half-admitted: either it was created before the seal and
    /// is a live runtime the parent counts, or it was refused.
    /// </remarks>
    [Fact]
    public async Task Sealing_A_Parent_Under_Contention_Admits_Or_Refuses_And_Never_Both()
    {
        var parent = Parent(fuel: 10_000_000);
        var catalog = FixtureComposition.AlphaCatalog();
        var created = new List<VmRuntime>();
        var refusals = 0;

        var creators = Enumerable.Range(0, 8).Select(_ => Task.Run(() =>
        {
            var result = VmRuntime.Create(catalog, FixtureComposition.Options(parent: parent));

            if (result.TryGetRuntime(out var runtime))
            {
                lock (created)
                {
                    created.Add(runtime);
                }

                return;
            }

            Interlocked.Increment(ref refusals);
            Assert.True(
                result.Outcome is VmOutcome.InvalidState or VmOutcome.ResourceExhaustion,
                $"a refusal under a sealing parent answered {result.Outcome}/{result.Reason}");
        })).ToArray();

        parent.Seal();
        await Task.WhenAll(creators);

        // Every attempt did exactly one of the two things, and the parent's own count agrees with
        // the number that succeeded.
        Assert.Equal(8, created.Count + refusals);
        Assert.Equal(created.Count, parent.LiveRuntimeCountValue);

        foreach (var runtime in created)
        {
            runtime.Dispose();
        }

        Assert.Equal(0, parent.LiveRuntimeCountValue);
        parent.Dispose();
    }

    /// <summary>
    /// A parent with a stated fuel allowance and room for every runtime these tests create.
    /// </summary>
    /// <remarks>
    /// Every aggregate dimension is explicit, because omission is what creation refuses: there is
    /// no parent to adopt from and no profile default that applies to a budget several profiles
    /// share.
    /// </remarks>
    private static VmAggregateBudget Parent(ulong fuel)
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
                VmBudgetDimension.Fuel => VmCeilingSpec.Value(dimension, fuel),
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.Value(dimension, 64),
                _ => VmCeilingSpec.Value(dimension, 1_000_000_000),
            });
        }

        return VmAggregateBudget.Create(builder.ToImmutable());
    }
}
