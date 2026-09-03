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
    /// costs well under a second. JS-9 owns choosing a soak budget; this is a recorded run and not
    /// one. It was raised from two thousand when the baseline moved to the midpoint below: the
    /// comparison spans half the run, so a longer run is what keeps a slow leak visible, and ten
    /// thousand cycles cost about 290ms on the published Native AOT image.
    /// </remarks>
    private const int SoakCycles = 10_000;

    /// <summary>
    /// How many points of the heap curve the soak records beside its two endpoints.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THIS COUNT IS LOAD-BEARING AND MUST NOT BE TRIMMED TO SAVE TIME.</b> It was added to
    /// diagnose a failure and it turned out to remove it, which is the whole finding: with two
    /// forced collections - one at the midpoint, one at the end - the final reading included heap
    /// the collector had not returned, and how much that was scaled with how much the process had
    /// allocated BEFORE the soak ever started. Growing the retained corpus from 66 entries to 91
    /// was enough to push it past the band on macOS under Native AOT, on both architectures,
    /// byte-identically across three runs, while win-x64, linux-x64 and Android stayed flat.
    /// Sampling sixteen times keeps the heap trimmed through the run and the false signal is gone.
    /// </para>
    /// <para>
    /// <b>So the check was measuring the wrong thing, and more sampling makes it stricter rather
    /// than kinder.</b> What it read was how much heap the collector had not yet returned, which
    /// is a fact about the process's history and not about recycling a runtime; a real per-cycle
    /// leak grows LIVE bytes, which every one of these readings sees however often they are taken.
    /// A future reader who deletes the samples to save two collections' worth of time reintroduces
    /// a signal that was never about leaking.
    /// </para>
    /// <para>
    /// Each sample costs two blocking gen-2 collections, so this is a real cost and not a free
    /// one; sixteen of them over ten thousand cycles is a fraction of a second.
    /// </para>
    /// </remarks>
    // Broiler-Falsified-If: reducing this count changes whether the check passes on unchanged code
    private const int Samples = 16;

    /// <summary>
    /// The band a plateau must stay inside, as a multiple of the heap at the midpoint of the run.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A managed heap does not return to a number; it returns to a range, and a check comparing
    /// two byte counts for equality would be a flake generator. What this is written to catch is
    /// unbounded growth across the run, not a regression of a few kilobytes.
    /// </para>
    /// <para>
    /// <b>It was 2.0, and 2.0 is unreachable once the baseline is the midpoint.</b> A per-cycle
    /// leak of L bytes over N cycles reads (B + N*L) / (B + N*L/2), which rises towards 2.0 as the
    /// leak grows and never arrives: a band of exactly 2.0 cannot be exceeded by ANY linear leak,
    /// so the check would have been structurally incapable of failing for the reason it exists.
    /// That was found by the negative control below rather than by reasoning, which is the whole
    /// argument for keeping one.
    /// </para>
    /// <para>
    /// <b>1.20 is derived from both ends and not chosen.</b> The measured steady state is 0.97
    /// under Native AOT, 0.95 under JIT and 0.93 to 0.94 trimmed, reproducible to the hundredth
    /// across repeated runs; an injected retention of 64 bytes per cycle reads 1.75. The band sits
    /// between them with margin on both sides. Solving the expression above, it fails once total
    /// retained bytes pass half the settled heap - about 8 bytes per cycle at this run length,
    /// which is a far smaller leak than the old band could see.
    /// </para>
    /// <para>
    /// <b>This is a tightening and not a widening.</b> A band is an envelope, and the measurement
    /// rules forbid widening one after seeing a candidate because a wider envelope hides the next
    /// real defect. Narrowing one is the opposite move and needs its own justification, which is
    /// the two measurements above.
    /// </para>
    /// </remarks>
    private const double PlateauBand = 1.20;

    /// <summary>
    /// The host-level exercises: three aggregate-budget properties and the soak's heap plateau.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The plateau was briefly opt-in and is not any more, and the reason it stopped being so
    /// is measured.</b> It was excluded from the CI lane on the ground that a heap reading on an
    /// ephemeral shared runner can be attributed to nothing. That was written against the OLD
    /// check, which compared an absolute reading at cycle 99 against one at the end - and it was
    /// right about absolute readings. The corrected check compares two readings in ONE process on
    /// ONE machine, which is a ratio.
    /// </para>
    /// <para>
    /// <b>Asked of runners rather than argued.</b> Twenty-seven samples over three dispatched runs,
    /// three attempts each on three operating systems: 0.97 on linux-x64, 0.97 on win-x64, 0.90 on
    /// osx-arm64, every one of them passing, with no run-to-run variation at the hundredth. The
    /// absolute heaps differed enormously across those platforms - about 153KB, 158KB and 388KB -
    /// while the ratio did not move. That is the whole case: what a shared runner makes
    /// unattributable is the byte count, and this check no longer reads one.
    /// </para>
    /// <para>
    /// <b>Those readings retain nothing and are rationale rather than evidence.</b> They were taken
    /// in dispatched lane runs, which collect no bundle and keep no configuration, so they are why
    /// the band above is what it is and they are cited as evidence by no bundle and by no row of
    /// the profile's ledger - which under its update rule 10 may carry no figure without a retained
    /// record behind it. What the checkout holds is the check, the band, and the control that makes
    /// it fail.
    /// </para>
    /// </remarks>
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
    /// The soak's cycles of create, run and dispose reach a heap plateau.
    /// </summary>
    /// <remarks>
    /// <b>What this can and cannot show.</b> It shows that recycling a runtime for the cycle count
    /// above does not grow the managed heap without bound, which is what a per-cycle leak looks like.
    /// It does not measure anything: the band is loose, the collection is forced, and a managed
    /// heap number on one machine is not a figure. JS-5 owns measurement and
    /// [section 17](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.gates.md) owns its rules.
    /// </remarks>
    private static (string, bool, string) RecycledRuntimesReachAPlateau(byte[] bytes)
    {
        var settled = 0L;
        var completed = 0;
        var curve = new List<(int Cycle, long Bytes)>();

        for (var cycle = 0; cycle < SoakCycles; cycle++)
        {
            using var runtime = Hosts.Runtime("default", out _);

            if (runtime is not null && RunOnce(runtime, bytes) == VmOutcome.Normal)
            {
                completed++;
            }

            // BOTH READINGS ARE LATE, AND THAT IS THE WHOLE POINT. The baseline is the midpoint
            // of the run, not an early fixed cycle, so warm-up is excluded by CONSTRUCTION in
            // every publish mode rather than by a constant that happens to suit one of them.
            //
            // The earlier version sampled at cycle 99 and reasoned that a hundred cycles was
            // enough to reach steady state. That is true under JIT and false under Native AOT,
            // and it made this check FAIL on correct code: measured on win-x64, the AOT heap is
            // still climbing at cycle 99 and does not settle until about cycle 1,000, so the
            // baseline was a cold reading and the ratio was 2.30 against a band of 2.0. Under
            // JIT the runtime's own allocation front-loads, the heap is already at steady state
            // by cycle 99, and the same check read 0.95. Same code, same corpus, same cycles.
            //
            // The diagnosis is recorded rather than summarised, because "we moved a threshold
            // after it went red" is what this would look like from outside: the growth is
            // ONE-TIME AND BOUNDED, not per-cycle. Running 2,000, 8,000 and 16,000 cycles
            // produced a final heap of 158,096 bytes in all three - identical to the byte, eight
            // times the work - which is the shape of warm-up and not of a leak. Sampling every
            // 500 cycles out to 20,000 showed one step and then a heap that did not move for
            // 19,500 consecutive cycles.
            //
            // THE BAND WAS NOT WIDENED - it was TIGHTENED, from 2.0 to the constant above.
            // Widening was the available shortcut and is the one thing the measurement rules
            // forbid, because a wider band hides the next real leak as happily as this false one.
            // Moving the baseline to the midpoint would have made 2.0 structurally unfailable: a
            // per-cycle leak L over N cycles reads (B + N*L) / (B + N*L/2), which rises towards
            // 2.0 and never reaches it. Sensitivity comes from the tighter band and the longer
            // run together, and the constant's own remarks derive it from both ends. A negative
            // control injects a per-cycle retention, which is what found that 2.0 could not fail.
            // THE CURVE IS RECORDED AND NOT ONLY ITS ENDPOINTS. Two numbers cannot tell warm-up
            // from a leak - a heap that climbs once and settles and a heap that climbs for ever
            // read the same at two points - and that ambiguity is what made the previous reading
            // of this check take a diagnosis rather than a glance. The samples are cheap, they
            // are printed only when the check fails or the caller asked, and a reader who can see
            // the shape does not have to be told what it was.
            if (cycle % (SoakCycles / Samples) == 0 || cycle == SoakCycles / 2)
            {
                var reading = Collected();
                curve.Add((cycle, reading));

                if (cycle == SoakCycles / 2)
                {
                    settled = reading;
                }
            }
        }

        var finished = Collected();
        curve.Add((SoakCycles, finished));
        var grew = settled == 0 ? 0 : (double)finished / settled;

        return (
            "recycled-runtimes-reach-a-heap-plateau",
            completed == SoakCycles && settled > 0 && grew <= PlateauBand,
            $"{completed} of {SoakCycles} cycles completed; the heap went from {settled} bytes " +
            $"at the midpoint to {finished} at the end, a factor of {grew:F2} against a " +
            $"band of {PlateauBand:F1}. Both readings are after warm-up, which is what makes them " +
            "comparable in every publish mode. This is a plateau check and not a measurement. " +
            "The curve, as cycle=bytes: " +
            string.Join(" ", curve.Select(static point => $"{point.Cycle}={point.Bytes}")));
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
