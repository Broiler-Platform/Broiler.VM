using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Soak.Host;

/// <summary>
/// The long-running lifecycle host: create, verify, instantiate, run, suspend, resume, cancel and
/// dispose, on several threads, for as long as it is asked, sampling what the process holds.
/// </summary>
/// <remarks>
/// <para>
/// It exists because a plateau is a measurement. The behavioural suite already asserts the METERED
/// plateau - the live-bytes counter returns to where it started - and that says the core's
/// accounting balances, which is a different claim from the process not growing. This one reports
/// the managed heap, the total allocated bytes and the working set at intervals, and says nothing
/// about whether the numbers are good.
/// </para>
/// <para>
/// It declares no threshold and passes no judgement, deliberately. Reading the samples is the
/// bundle's job and deciding whether a plateau is acceptable is a release decision; a host that
/// decided for itself would be a benchmark with an opinion, and baselines are VM-5's milestone. The
/// one thing it does judge is its own validity: a run whose operations did not actually happen is
/// reported as a failure, because a flat line from a host that ran nothing is the most misleading
/// output it could produce.
/// </para>
/// </remarks>
internal static class Program
{
    private static int Main(string[] args)
    {
        var cycles = Value(args, "--cycles", 2_000);
        var workers = Value(args, "--workers", 4);
        var sampleEvery = Value(args, "--sample-every", 250);

        Console.WriteLine(
            $"# broiler-vm-soak core-contract-version={VmCoreContract.Version} " +
            $"cycles={cycles} workers={workers} sample-every={sampleEvery}");

        Console.WriteLine(
            $"# gc server={System.Runtime.GCSettings.IsServerGC} " +
            $"latency={System.Runtime.GCSettings.LatencyMode}");

        var catalog = Catalog();
        var tally = new Tally();
        var samples = new List<Sample>();
        var started = DateTime.UtcNow;

        // One sampler thread, because sampling from the workers would make every figure a
        // measurement of whichever worker happened to reach the sample point first.
        using var stop = new CancellationTokenSource();

        var sampler = Task.Run(() =>
        {
            var index = 0;

            samples.Add(Sample.Take(0, TimeSpan.Zero));

            while (!stop.IsCancellationRequested)
            {
                Thread.Sleep(20);

                var done = Volatile.Read(ref tally.Cycles);

                if (done < (index + 1) * sampleEvery)
                {
                    continue;
                }

                // Advance past every point the workers crossed between two polls, and take ONE
                // sample. Emitting one per missed point would repeat the same measurement under
                // several cycle counts, which reads as a stall in the curve that never happened.
                while (done >= (index + 1) * sampleEvery)
                {
                    index++;
                }

                samples.Add(Sample.Take(done, DateTime.UtcNow - started));
            }
        });

        var work = new Task[workers];

        for (var worker = 0; worker < workers; worker++)
        {
            work[worker] = Task.Run(() => Soak(catalog, cycles / workers, tally));
        }

        Task.WaitAll(work);
        stop.Cancel();
        sampler.GetAwaiter().GetResult();

        // A final sample after a settling collection, which is the figure a plateau claim rests on:
        // what the process holds once nothing is running and everything reclaimable is reclaimed.
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);

        var settled = Sample.Take(Volatile.Read(ref tally.Cycles), DateTime.UtcNow - started);

        Console.WriteLine("cycles heap-bytes allocated-bytes working-set-bytes gen0 gen1 gen2 elapsed-ms");

        foreach (var sample in samples)
        {
            Console.WriteLine(sample.ToString());
        }

        Console.WriteLine(settled + " settled");

        Console.WriteLine(
            $"# totals cycles={tally.Cycles} runtimes={tally.Runtimes} invocations={tally.Invocations} " +
            $"suspensions={tally.Suspensions} resumptions={tally.Resumptions} " +
            $"cancellations={tally.Cancellations} refusals={tally.Refusals} faults={tally.Faults}");

        // The validity check, and the only judgement this host makes. A run that completed no
        // cycles, never suspended or never cancelled would produce a beautifully flat line that
        // measured nothing, and reporting that as a plateau would be the worst outcome available.
        var valid =
            tally.Cycles >= cycles - workers &&
            tally.Invocations >= tally.Cycles &&
            tally.Suspensions > 0 &&
            tally.Resumptions > 0 &&
            tally.Cancellations > 0 &&
            tally.Faults == 0 &&
            tally.Refusals == 0 &&
            tally.Runtimes > 1 &&
            samples.Count > 1;

        Console.WriteLine(
            valid
                ? $"broiler-vm-soak: {tally.Cycles} cycles completed, {samples.Count} samples retained"
                : "broiler-vm-soak: INVALID RUN - the workload did not exercise what it claims to");

        return valid ? 0 : 1;
    }

    /// <summary>
    /// One worker's share of the cycles: the whole lifecycle, including the paths that fail.
    /// </summary>
    /// <remarks>
    /// Every fourth cycle suspends and resumes, every seventh is cancelled mid-flight, and every
    /// eleventh abandons its pause and lets residency expiry collect it. Those are the paths that
    /// leak if anything does: a continuation nobody resumed, an operation nobody completed, an
    /// instance whose lease was never released.
    /// </remarks>
    private static void Soak(VmCatalog catalog, int cycles, Tally tally)
    {
        // A runtime's ceilings are a TOTAL allowance, not a per-operation one, so one runtime
        // running forever does not measure a plateau - it measures the moment its fuel ran out and
        // every later cycle was refused. The first soak run did exactly that: it asked for 400,000
        // cycles, completed 161,616, and the rest were refusals nobody was counting.
        //
        // Recycling is also the more honest workload. The gate asks for create-verify-instantiate-
        // run-suspend-resume-cancel-dispose LOOPS, and runtime creation and disposal are two of
        // those verbs; a soak that created one runtime and never disposed it would be exercising
        // six of the eight.
        const int CyclesPerRuntime = 4_000;

        var descriptor = Descriptor();
        var plain = FixtureArtifactWriter.Sum(20, 22);
        var yielding = FixtureArtifactWriter.YieldThenConstant(17);

        VmRuntime? runtime = null;

        for (var cycle = 0; cycle < cycles; cycle++)
        {
            if (runtime is null || cycle % CyclesPerRuntime == 0)
            {
                runtime?.Dispose();
                runtime = Runtime(catalog);
                Interlocked.Increment(ref tally.Runtimes);
            }

            // Every fourth cycle parks, and every seventh of the rest is cancelled. The two are
            // deliberately not the same cycles: a suspension that is also cancelled exercises one
            // path twice and the other never.
            var parks = cycle % 4 == 0;
            var cancels = !parks && cycle % 7 == 0;
            var payload = parks ? yielding : plain;

            var verified = runtime.Verify(in descriptor, payload, CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                // Counted rather than skipped. A refusal is a legitimate answer - a runtime whose
                // allowance is spent refuses everything - but a soak that silently skipped them
                // would report a flat line drawn by a runtime that stopped working.
                Interlocked.Increment(ref tally.Refusals);
                continue;
            }

            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                Interlocked.Increment(ref tally.Refusals);
                artifact.Dispose();
                continue;
            }

            var request = new VmInvocationRequest(new VmUtf8Text("main"u8));

            // A cancelled token rather than a race: the cancellation path is exercised on a cycle
            // the run chose, every time, on every machine. Racing a cancel against a fast
            // invocation would exercise it sometimes, which is a different and much weaker thing to
            // measure over a long run.
            using var cancellation = new CancellationTokenSource();

            if (cancels)
            {
                cancellation.Cancel();
            }

            var result = instance.Invoke(in request, cancellation.Token);
            Interlocked.Increment(ref tally.Invocations);

            if (result.TryGetSuspension(out var suspension))
            {
                Interlocked.Increment(ref tally.Suspensions);

                if (cycle % 11 != 0)
                {
                    var resumed = runtime.Resume(suspension);
                    Interlocked.Increment(ref tally.Resumptions);

                    if (resumed.Outcome is not VmOutcome.Normal)
                    {
                        Interlocked.Increment(ref tally.Faults);
                    }
                }

                // Every eleventh pause is simply walked away from, which is the abandoning client
                // the gate names. Residency expiry is what ends it.
                runtime.PollDeadlines();
            }
            else if (result.Outcome is VmOutcome.Cancellation)
            {
                Interlocked.Increment(ref tally.Cancellations);
            }
            else if (result.Outcome is not VmOutcome.Normal)
            {
                Interlocked.Increment(ref tally.Faults);
            }

            instance.Dispose();
            artifact.Dispose();
            Interlocked.Increment(ref tally.Cycles);
        }

        runtime?.Dispose();
    }

    private static VmCatalog Catalog() =>
        VmCatalog.CreateBuilder()
            .Add(FixtureVmProfile.Descriptor)
            .Add(SecondFixtureVmProfile.Descriptor)
            .Build();

    private static VmRuntime Runtime(VmCatalog catalog)
    {
        var ceilings = System.Collections.Immutable.ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var capabilities = System.Collections.Immutable.ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        capabilities.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Double, FixtureHostCapabilities.DoubleHandler));
        capabilities.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Throwing, FixtureHostCapabilities.ThrowingHandler));
        capabilities.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Refusing, FixtureHostCapabilities.RefusingHandler));

        var options = new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMilliseconds(50),
            maxLiveSuspendedOperations: 4,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities.ToImmutable());

        var created = VmRuntime.Create(catalog, options);

        if (!created.TryGetRuntime(out var runtime))
        {
            throw new InvalidOperationException(
                $"runtime creation failed: {created.Outcome}/{created.Reason}");
        }

        return runtime;
    }

    private static VmArtifactDescriptor Descriptor() =>
        new(FixtureVmProfile.Id, FixtureFormat.FormatVersion, FixtureVmProfile.Manifest, default,
            VmCallerIdentity.FromCanonicalIdentity("soak://artifact"));

    private static int Value(string[] arguments, string name, int fallback)
    {
        for (var index = 0; index + 1 < arguments.Length; index++)
        {
            if (string.Equals(arguments[index], name, StringComparison.Ordinal) &&
                int.TryParse(arguments[index + 1], out var parsed) &&
                parsed > 0)
            {
                return parsed;
            }
        }

        return fallback;
    }

    /// <summary>What the run did, so a flat line can be told apart from an empty one.</summary>
    private sealed class Tally
    {
        internal int Cycles;
        internal int Runtimes;
        internal int Refusals;
        internal int Invocations;
        internal int Suspensions;
        internal int Resumptions;
        internal int Cancellations;
        internal int Faults;
    }

    /// <summary>One measurement of what the process holds.</summary>
    private readonly record struct Sample(
        int Cycles,
        long HeapBytes,
        long AllocatedBytes,
        long WorkingSetBytes,
        int Gen0,
        int Gen1,
        int Gen2,
        long ElapsedMilliseconds)
    {
        internal static Sample Take(int cycles, TimeSpan elapsed) =>
            new(
                cycles,
                GC.GetTotalMemory(forceFullCollection: false),
                GC.GetTotalAllocatedBytes(precise: false),
                Environment.WorkingSet,
                GC.CollectionCount(0),
                GC.CollectionCount(1),
                GC.CollectionCount(2),
                (long)elapsed.TotalMilliseconds);

        public override string ToString() =>
            $"{Cycles} {HeapBytes} {AllocatedBytes} {WorkingSetBytes} {Gen0} {Gen1} {Gen2} {ElapsedMilliseconds}";
    }
}
