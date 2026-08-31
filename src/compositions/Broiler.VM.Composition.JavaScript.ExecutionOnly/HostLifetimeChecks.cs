using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.ExecutionOnly;

/// <summary>
/// Two of JS-9's host-level exercises: sibling runtimes under one aggregate budget, and a soak
/// over recycled runtimes.
/// </summary>
/// <remarks>
/// <para>
/// <b>The core has tests of this shape and they are not this profile's evidence.</b> Update rule 6
/// of the status ledger says a core result never advances a row here, and it is not a courtesy:
/// the core's tests run over a fixture profile, and what these exercise is whether THIS profile's
/// verifier, executor and instance state behave under a shared parent and across recycling.
/// </para>
/// <para>
/// <b>One clause here is about what is NOT asserted.</b> When two siblings share a parent and the
/// parent runs out, which of them observes the exhaustion is a race, and a check that named a
/// winner would be a check that passes on one machine. What is asserted is the total.
/// </para>
/// </remarks>
internal static class HostLifetimeChecks
{
    /// <summary>How many create-verify-instantiate-invoke-dispose cycles the soak runs.</summary>
    /// <remarks>
    /// Stated rather than tuned. It is large enough that a per-cycle leak of anything measurable
    /// would show as growth well past the plateau band below, and small enough that the check
    /// costs a second. JS-9 owns choosing a soak budget; this is a recorded run and not one.
    /// </remarks>
    private const int SoakCycles = 2_000;

    /// <summary>
    /// The band a plateau must stay inside, as a multiple of the heap after the first cycles.
    /// </summary>
    /// <remarks>
    /// A managed heap does not return to a number; it returns to a range, and a check comparing
    /// two byte counts for equality would be a flake generator. Two is loose on purpose: what this
    /// is written to catch is unbounded growth across two thousand cycles, not a regression of a
    /// few kilobytes, and a tighter band would be a measurement claim JS-5 owns making.
    /// </remarks>
    private const double PlateauBand = 2.0;

    internal static IEnumerable<(string Name, bool Passed, string Detail)> Run(string directory)
    {
        var bytes = File.ReadAllBytes(Path.Combine(directory, "a-counting-loop.bjsb"));

        yield return SiblingsSpendOneTotal(bytes);
        yield return AParentWithLiveChildrenRefusesDisposal(bytes);
        yield return ASealedParentAdmitsNoFurtherRuntime();
        yield return RecycledRuntimesReachAPlateau(bytes);
    }

    /// <summary>
    /// Two runtimes under one parent together spend no more than the parent's allowance.
    /// </summary>
    /// <remarks>
    /// <b>Which sibling is refused is deliberately not asserted.</b> Both draw on one total and the
    /// order they reach it in is a race; naming a winner would be asserting a scheduler. What is
    /// asserted is that the total spent never passes the allowance and that at least one of them
    /// was refused, which is the property the shared parent exists to provide.
    /// </remarks>
    private static (string, bool, string) SiblingsSpendOneTotal(byte[] bytes)
    {
        using var parent = VmAggregateBudget.Create(AggregateCeilings(fuel: 4_000));

        using var first = Sibling(parent, out var firstFailure);
        using var second = Sibling(parent, out var secondFailure);

        if (first is null || second is null)
        {
            return (
                "two-siblings-under-one-parent-spend-one-total", false,
                $"a sibling was refused at creation: {firstFailure}/{secondFailure}");
        }

        var completed = 0;
        var refused = 0;

        for (var round = 0; round < 64; round++)
        {
            foreach (var runtime in new[] { first, second })
            {
                var outcome = RunOnce(runtime, bytes);

                completed += outcome == VmOutcome.Normal ? 1 : 0;
                refused += outcome == VmOutcome.ResourceExhaustion ? 1 : 0;
            }
        }

        var snapshot = parent.GetSnapshot();
        var spent = snapshot.Consumed(VmBudgetDimension.Fuel);
        var allowance = snapshot.EffectiveCeiling(VmBudgetDimension.Fuel);

        return (
            "two-siblings-under-one-parent-spend-one-total",
            spent <= allowance && refused > 0 && completed > 0,
            $"{completed} invocations completed and {refused} were refused across two siblings; " +
            $"the parent spent {spent} of {allowance} fuel. Which sibling was refused is not " +
            "asserted: both draw on one total and the order is a race");
    }

    /// <summary>Disposing a parent with a live child is refused rather than orphaning it.</summary>
    private static (string, bool, string) AParentWithLiveChildrenRefusesDisposal(byte[] bytes)
    {
        var parent = VmAggregateBudget.Create(AggregateCeilings(fuel: 1_000_000));
        var child = Sibling(parent, out var failure);

        if (child is null)
        {
            parent.Dispose();
            return ("a-parent-with-a-live-child-refuses-disposal", false, $"no child: {failure}");
        }

        RunOnce(child, bytes);

        var refused = parent.Dispose();
        var live = parent.LiveRuntimeCountValue;

        child.Dispose();
        var afterwards = parent.Dispose();

        return (
            "a-parent-with-a-live-child-refuses-disposal",
            !refused.IsSuccess && afterwards.IsSuccess,
            $"with {live} live child the parent answered {refused.Kind}/{refused.Reason}; " +
            $"after the child was disposed it answered {afterwards.Kind}");
    }

    /// <summary>A sealed parent admits no further runtime, and says so rather than throwing.</summary>
    private static (string, bool, string) ASealedParentAdmitsNoFurtherRuntime()
    {
        using var parent = VmAggregateBudget.Create(AggregateCeilings(fuel: 1_000_000));

        var sealing = parent.Seal();
        var admitted = Sibling(parent, out var failure);
        var isSealed = parent.IsSealed;

        admitted?.Dispose();

        return (
            "a-sealed-parent-admits-no-further-runtime",
            sealing.IsSuccess && isSealed && admitted is null,
            $"sealing answered {sealing.Kind}; a runtime created afterwards was " +
            (admitted is null ? $"refused with {failure}" : "ADMITTED"));
    }

    /// <summary>
    /// Two thousand create-run-dispose cycles reach a heap plateau.
    /// </summary>
    /// <remarks>
    /// <b>What this can and cannot show.</b> It shows that recycling a runtime two thousand times
    /// does not grow the managed heap without bound, which is what a per-cycle leak looks like.
    /// It does not measure anything: the band is loose, the collection is forced, and a managed
    /// heap number on one machine is not a figure. JS-5 owns measurement and
    /// [section 17](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.gates.md) owns its rules.
    /// </remarks>
    private static (string, bool, string) RecycledRuntimesReachAPlateau(byte[] bytes)
    {
        var settled = 0L;
        var completed = 0;

        for (var cycle = 0; cycle < SoakCycles; cycle++)
        {
            using var runtime = Hosts.Runtime("default", out _);

            if (runtime is not null && RunOnce(runtime, bytes) == VmOutcome.Normal)
            {
                completed++;
            }

            // The band is measured from a heap that has already seen a hundred cycles, so what is
            // compared is steady state against steady state rather than steady state against a
            // process that has just started.
            if (cycle == 99)
            {
                settled = Collected();
            }
        }

        var finished = Collected();
        var grew = settled == 0 ? 0 : (double)finished / settled;

        return (
            "recycled-runtimes-reach-a-heap-plateau",
            completed == SoakCycles && settled > 0 && grew <= PlateauBand,
            $"{completed} of {SoakCycles} cycles completed; the heap went from {settled} bytes " +
            $"after 100 cycles to {finished} after {SoakCycles}, a factor of {grew:F2} against a " +
            $"band of {PlateauBand:F1}. This is a plateau check and not a measurement");
    }

    private static long Collected()
    {
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);

        return GC.GetTotalMemory(forceFullCollection: true);
    }

    private static VmRuntime? Sibling(VmAggregateBudget parent, out string failure)
    {
        // A child under a parent ADOPTS THE PARENT'S REMAINING on every dimension the parent
        // bounds, and profile defaults on the rest. Anything else is refused at creation with
        // ExceedsParentRemaining - which is the core doing its job, and which is how this check
        // first failed: a child asking for the profile's default fuel under a parent holding four
        // thousand is asking for more than the parent has.
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(VmBudgetDimensions.CarriesAggregateScope(dimension)
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var options = new VmRuntimeCreationOptions(
            aggregateBudget: parent,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: ImmutableArray<VmCapabilityRegistration>.Empty);

        var created = VmRuntime.Create(Hosts.Catalog(), options);

        if (created.TryGetRuntime(out var runtime))
        {
            failure = string.Empty;
            return runtime;
        }

        failure = $"{created.Outcome}/{created.Reason}";
        return null;
    }

    private static VmOutcome RunOnce(VmRuntime runtime, byte[] bytes)
    {
        var descriptor = Hosts.Descriptor("default");
        var verified = runtime.Verify(in descriptor, bytes, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return verified.Outcome;
        }

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return instantiated.Outcome;
        }

        var request = new VmInvocationRequest(new VmUtf8Text("main"u8));

        return instance.Invoke(in request, CancellationToken.None).Outcome;
    }

    /// <summary>
    /// An aggregate ceiling for every dimension that carries aggregate scope.
    /// </summary>
    /// <remarks>
    /// Every one of them explicitly, because the core refuses an omission rather than reading it as
    /// unbounded - and that refusal is the reason this helper exists rather than a vector with one
    /// interesting number in it.
    /// </remarks>
    private static ImmutableArray<VmCeilingSpec> AggregateCeilings(ulong fuel)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (!VmBudgetDimensions.CarriesAggregateScope(dimension))
            {
                continue;
            }

            ceilings.Add(VmCeilingSpec.Value(
                dimension,
                dimension == VmBudgetDimension.Fuel ? fuel : ulong.MaxValue / 4));
        }

        return ceilings.ToImmutable();
    }
}
