using Broiler.VM;
using Broiler.VM.Fixtures;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Broiler.VM.Bench.Host;

/// <summary>
/// The baseline host: what the core costs a profile, measured against controls that differ only in
/// the core's involvement.
/// </summary>
/// <remarks>
/// <para>
/// It publishes what the core costs so that a profile can budget against it. **No language
/// performance claim follows from any figure here** - every measurement is of the core's own
/// overhead around a fixture profile whose executor is a toy, and a real language's cost is its
/// own. That sentence is in the gate, in the baseline register and here, because it is the one
/// misreading these numbers invite.
/// </para>
/// <para>
/// Each measurement's rule is predeclared in <c>docs/baselines.md</c> before any figure existed,
/// and rule L1 holds the two to each other in both directions: a figure nothing measured cannot be
/// published, and a measurement nobody declared cannot appear.
/// </para>
/// </remarks>
internal static class Program
{
    /// <summary>
    /// Where a lane's answer goes, so the answer is not dead code.
    /// </summary>
    /// <remarks>
    /// A lane whose result nothing reads is a lane the compiler is entitled to delete, and a
    /// deleted lane measures the loop. It matters most in the cheapest measurements, where the
    /// operation is a few comparisons and the difference between keeping and eliding it is the
    /// whole figure.
    /// </remarks>
    internal static long Sink;

    /// <summary>
    /// How many guest loads one invocation of the guest-load lane requests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The guest-load lane's shape is forced by a bound rather than chosen. Fan-out is charged at
    /// runtime scope as a lifetime total and the fixture profile's hard maximum for it is 64, which
    /// no host may raise - so no runtime anywhere admits more than 64 mediated loads in its life,
    /// and a lane of thousands cannot hold one runtime for the duration.
    /// </para>
    /// <para>
    /// So the runtime is rebuilt inside the timed region and the control rebuilds one too: a
    /// fixture cost present identically in both lanes cancels in the difference, which lets the
    /// measurement have as many iterations as it needs while every runtime it builds stays far
    /// inside its allowance.
    /// </para>
    /// <para>
    /// One, because a per-load figure taken over several is not a per-load figure here. The
    /// fan-out scaling series shows the cost of a load rising with the number of loads the same
    /// operation has already made, so an average over six would be a number belonging to no single
    /// load. The series is recorded separately and the measurement stays at the smallest honest
    /// unit.
    /// </para>
    /// </remarks>
    private const int LoadsPerInvocation = 1;

    /// <summary>Iterations of the guest-load lane, each one a whole runtime.</summary>
    private const int GuestLoadIterations = 300;

    /// <summary>The loads-per-operation the fan-out series walks.</summary>
    /// <remarks>
    /// It stops at eight because eight is the fixture profile's per-operation fan-out bound, and a
    /// ninth load in one operation is a refusal rather than a slower load.
    /// </remarks>
    private static readonly int[] FanOutSeries = [0, 1, 2, 3, 4, 6, 8];

    private const int FanOutIterations = 200;

    /// <summary>The nested artifact the provider answers with throughout.</summary>
    private static readonly byte[] NestedPayload = FixtureArtifactWriter.Constant(7);

    /// <summary>
    /// What a mediated load costs as a function of how many the same operation has already made.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A recorded series rather than a measurement, and the reason the guest-load measurement above
    /// is stated per single load. Mediation over one load costs nothing this harness can resolve
    /// against performing the same load directly; over several, the mediated lane pulls away from
    /// the direct one and each further load costs more than the last. A single "per load" figure
    /// averaged over six would therefore belong to no load at all.
    /// </para>
    /// <para>
    /// Each row is one whole runtime, so nothing here accumulates across rows, and both lanes
    /// verify the same nested bytes the same number of times. Publishing the series is what the
    /// gate means by funding optimization against a baseline: this repository does not act on it in
    /// VM-5, and anyone who later wants to has the numbers and the shape.
    /// </para>
    /// </remarks>
    private static readonly System.Threading.AsyncLocal<object?> DriftProbe = new();

    /// <summary>
    /// Whether an operation costs the same after seventy thousand runtimes as after none.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Every other measurement here divides by iterations and reports an average, which is exactly
    /// the shape that hides a cost growing with how much the process has already done. This one
    /// samples the same work at the start of the run and again at the end and compares the two, and
    /// it is the check that found the defect it now guards: a runtime's capability-depth
    /// async-local could never be set to null, so its entry stayed on the thread for the life of
    /// the process - one per runtime, released by nothing, not even disposal - and every later
    /// async-local write copied the lot.
    /// </para>
    /// <para>
    /// The cost of that was not subtle once something looked: the same instantiate-and-invoke went
    /// from 9,960 bytes to 1,188,872, a bare async-local write from 72 bytes to 393,072, and this
    /// whole benchmark from 43 seconds to 528. It was invisible to every average in the run.
    /// </para>
    /// </remarks>
    private static (long Invoke, long AsyncLocal) Independence(TextWriter writer, string label)
    {
        var catalog = DeclaringCatalog();
        var plain = Loading(0, load: false);
        var marker = new object();

        var invoke = Sample(() => Direct(catalog, NestedPayload, plain, 0));

        var ambient = Sample(() =>
        {
            DriftProbe.Value = marker;
            DriftProbe.Value = null;
        });

        writer.WriteLine(
            $"  independence {label} runtimes-created={RuntimesCreated} " +
            $"invoke-bytes={invoke.Bytes} asynclocal-bytes={ambient.Bytes}");

        return (invoke.Bytes, ambient.Bytes);
    }

    /// <summary>How many runtimes this process has built, which is what the drift is measured against.</summary>
    private static long RuntimesCreated;

    private static void FanOut(TextWriter writer)
    {
        // Hoisted, and it matters more than it looks. Building a catalog means building a profile
        // descriptor - limit vectors, declaration matrix, capability imports - and calling it inside
        // the lambda put all of that into every iteration of both lanes, a megabyte an iteration
        // that swamped the loads being measured.
        var catalog = DeclaringCatalog();

        foreach (var loads in FanOutSeries)
        {
            var loading = Loading(loads, load: true);
            var plain = Loading(loads, load: false);

            var mediated = Sample(() => Mediated(catalog, NestedPayload, loading));
            var direct = Sample(() => Direct(catalog, NestedPayload, plain, loads));

            writer.WriteLine(
                $"  fan-out loads={loads} " +
                $"mediated-ns={mediated.Nanoseconds:F0} mediated-bytes={mediated.Bytes} " +
                $"direct-ns={direct.Nanoseconds:F0} direct-bytes={direct.Bytes} " +
                $"mediation-ns={mediated.Nanoseconds - direct.Nanoseconds:F0} " +
                $"mediation-bytes={mediated.Bytes - direct.Bytes}");
        }

        writer.WriteLine(
            $"fan-out series={string.Join(",", FanOutSeries)} " +
            $"iterations={FanOutIterations} note=mediation-cost-rises-with-loads-per-operation");
    }

    /// <summary>One sample of an action: its per-iteration time and its per-iteration allocation.</summary>
    /// <remarks>
    /// The settling collection is not optional here and finding that out cost a wrong conclusion.
    /// Without it the second lane pays for the first lane's garbage, and the series read as though
    /// the CONTROL grew with the load count too - which would have made the whole finding an
    /// artefact of the order the lanes happened to run in.
    /// </remarks>
    private static (double Nanoseconds, long Bytes) Sample(Action action)
    {
        for (var warm = 0; warm < 30; warm++)
        {
            action();
        }

        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);

        var before = GC.GetAllocatedBytesForCurrentThread();
        var start = Stopwatch.GetTimestamp();

        for (var iteration = 0; iteration < FanOutIterations; iteration++)
        {
            action();
        }

        return (
            Stopwatch.GetElapsedTime(start).TotalNanoseconds() / FanOutIterations,
            (GC.GetAllocatedBytesForCurrentThread() - before) / FanOutIterations);
    }

    private static int Main(string[] args)
    {
        var started = Stopwatch.GetTimestamp();
        var writer = Console.Out;


        writer.WriteLine(
            $"# broiler-vm-bench core-contract-version={VmCoreContract.Version} " +
            $"repetitions={Harness.Repetitions} warmup={Harness.WarmupIterations}");

        writer.WriteLine(
            $"# gc server={System.Runtime.GCSettings.IsServerGC} " +
            $"latency={System.Runtime.GCSettings.LatencyMode} " +
            $"processors={Environment.ProcessorCount}");

        // Startup, measured before anything else has warmed anything: process start to the first
        // verified artifact, which is what a host embedding the core actually waits for.
        writer.WriteLine($"startup first-verification-ms={FirstVerification():F1}");

        Image(writer);

        var independenceBefore = Independence(writer, "before");

        var measurements = Build(writer);
        var invalid = 0;

        foreach (var measurement in measurements)
        {
            writer.WriteLine($"# {measurement.Id}: {measurement.What}");
            Harness.Run(measurement, writer);
        }

        // Re-read the harness's own verdicts from what it printed would be circular, so the same
        // validity rule is applied here over the measurements it just ran.
        invalid = 0;

        foreach (var measurement in measurements)
        {
            if (!Valid(measurement))
            {
                invalid++;
                writer.WriteLine($"# INVALID {measurement.Id}: the A/A lane difference exceeds the effect");
            }
        }

        FanOut(writer);

        var independenceAfter = Independence(writer, "after");

        // A quadruple is generous - the defect this guards multiplied the figure by a hundred and
        // twenty - and generous is right for a guard whose false positive would be a machine having
        // a bad minute.
        var independent =
            independenceAfter.Invoke <= (independenceBefore.Invoke * 4) &&
            independenceAfter.AsyncLocal <= (independenceBefore.AsyncLocal * 4);

        writer.WriteLine(
            $"independence held={(independent ? "yes" : "no")} " +
            $"invoke-bytes={independenceBefore.Invoke}->{independenceAfter.Invoke} " +
            $"asynclocal-bytes={independenceBefore.AsyncLocal}->{independenceAfter.AsyncLocal} " +
            $"runtimes-created={RuntimesCreated}");

        if (!independent)
        {
            invalid++;
            writer.WriteLine("# INVALID independence: per-operation cost grew with the number of runtimes created");
        }

        Plateau(writer);

        writer.WriteLine(
            $"# elapsed-ms={Stopwatch.GetElapsedTime(started).TotalMilliseconds:F0} " +
            $"measurements={measurements.Length} invalid={invalid}");

        writer.WriteLine(
            invalid == 0
                ? $"broiler-vm-bench: {measurements.Length} measurements, every A/A lane inside its effect"
                : $"broiler-vm-bench: {invalid} of {measurements.Length} measurements are INVALID");

        return invalid == 0 ? 0 : 1;
    }

    /// <summary>
    /// Re-runs one measurement's two identical lanes and answers whether the noise floor is smaller
    /// than the effect.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A second, independent check rather than a re-reading of the printed line. The printed verdict
    /// is for the reader; this one decides the exit code, and having them come from two separate
    /// runs is what stops a formatting bug from deciding whether a lane was valid.
    /// </para>
    /// <para>
    /// It repeats, for the same reason the harness does. A single unrepeated triple is a coin toss
    /// on any measurement whose effect is within an order of magnitude of one scheduler event, and
    /// it duly failed a measurement the harness had just called valid with an A/A lane fifty times
    /// inside its effect. A second opinion noisier than the first opinion is not a check.
    /// </para>
    /// </remarks>
    private static bool Valid(Measurement measurement)
    {
        const int Triples = 3;

        var first = new double[Triples];
        var control = new double[Triples];
        var second = new double[Triples];

        for (var triple = 0; triple < Triples; triple++)
        {
            // Same reset discipline as the harness. Without it this second opinion would run its
            // lanes against whatever allowance the first run left behind, which is the one state in
            // which every lane is fast and none of them measures the operation.
            measurement.Reset?.Invoke();
            first[triple] = Time(measurement.Candidate, measurement.Iterations);

            measurement.Reset?.Invoke();
            control[triple] = Time(measurement.Control, measurement.Iterations);

            measurement.Reset?.Invoke();
            second[triple] = Time(measurement.Candidate, measurement.Iterations);

            if (measurement.Condition?.Invoke() is { } problem)
            {
                throw new InvalidOperationException($"{measurement.Id} (revalidation): {problem}");
            }
        }

        if (measurement.UpperBoundIfBelowNoise)
        {
            // Declared in advance as a measurement whose answer may be "nothing measurable". The
            // harness publishes the floor as an upper bound; there is no second opinion to form
            // about a difference nobody claims to have resolved.
            return true;
        }

        var candidateTime = Middle(first);

        return Math.Abs(candidateTime - Middle(second)) <= Math.Abs(candidateTime - Middle(control));
    }

    private static double Middle(double[] values)
    {
        var ordered = values.OrderBy(static value => value).ToArray();

        return ordered.Length % 2 == 1
            ? ordered[ordered.Length / 2]
            : (ordered[(ordered.Length / 2) - 1] + ordered[ordered.Length / 2]) / 2;
    }

    private static double Time(Action action, int iterations)
    {
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        var start = Stopwatch.GetTimestamp();

        for (var iteration = 0; iteration < iterations; iteration++)
        {
            action();
        }

        return Stopwatch.GetElapsedTime(start).TotalNanoseconds() / iterations;
    }

    /// <summary>Process start to the first verified artifact, in milliseconds.</summary>
    private static double FirstVerification()
    {
        var startedAt = Process.GetCurrentProcess().StartTime.ToUniversalTime();

        using var runtime = Runtime(Catalog(profiles: 1));
        var descriptor = Descriptor();
        var result = runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None);

        result.TryGetArtifact(out var artifact);
        artifact?.Dispose();

        return (DateTime.UtcNow - startedAt).TotalMilliseconds;
    }

    /// <summary>
    /// How many mediated loads one runtime admits before an allowance stops it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A recorded figure that exists because the guest-load lane needed it. A mediated load leaves
    /// a nested verified handle behind for the requesting operation, and the allowances it charges
    /// are runtime-scope totals, so a lane sized like the other invocation lanes does not measure
    /// mediation - it measures mediation until the allowance runs out and refusal afterwards. The
    /// harness catches that now; this states the number the lane is sized against, so a reader can
    /// check the sizing rather than trust it.
    /// </para>
    /// <para>
    /// It is deterministic - Fuel, allocated bytes and nested-load counts are counted, not timed -
    /// so both lanes reach the same figure and the measurement stays comparable across them.
    /// </para>
    /// </remarks>
    private static (int Loads, string Limit) Headroom(byte[] nested, byte[] loading)
    {
        const int Ceiling = 1_000_000;

        using var runtime = Runtime(
            Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.DeclaresGuestLoads)),
            Provider(nested));

        var instance = Instance(runtime, loading);

        for (var load = 0; load < Ceiling; load++)
        {
            var result = Invoke(instance);

            if (result.Outcome is not VmOutcome.Normal)
            {
                return (load, $"{result.Outcome}/{result.Reason}/" +
                    $"{result.Diagnostics.ExhaustedDimension}/{result.Diagnostics.ExhaustedScope}");
            }
        }

        return (Ceiling, "none-reached");
    }

    /// <summary>
    /// What this deployment weighs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A recorded figure, not a measurement: there is no control a size could be attributed
    /// against, no A/A lane and nothing to repeat, and saying so in the output is better than
    /// dressing a file length up as an experiment.
    /// </para>
    /// <para>
    /// The two lanes report different things under the same name and the register says which is
    /// which. On the JIT lane <c>process</c> is the shared host that launched the app and
    /// <c>deployment</c> is what a consumer would copy; on the Native AOT lane the app IS the
    /// process image and the two converge. The core assemblies are reported separately, because
    /// they are the part a package would carry and the rest is the fixture profile and this
    /// harness.
    /// </para>
    /// </remarks>
    private static void Image(TextWriter writer)
    {
        var process = Environment.ProcessPath;
        var directory = AppContext.BaseDirectory;

        writer.WriteLine(
            $"image process-bytes={Length(process)} " +
            $"deployment-bytes={DirectoryBytes(directory)} " +
            $"core-bytes={CoreBytes(directory)} " +
            $"runtime-identifier={System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier} " +
            $"aot={!System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported}");
    }

    private static long Length(string? path) =>
        path is not null && File.Exists(path) ? new FileInfo(path).Length : 0;

    /// <summary>
    /// What the deployment directory itself weighs, not counting anything nested inside it.
    /// </summary>
    /// <remarks>
    /// Top level only, and the distinction is not pedantry: a build output directory acquires
    /// subdirectories that are not part of any deployment - a publish for another runtime
    /// identifier, most obviously - and summing them recursively reported this deployment as
    /// eighty-nine megabytes the first time an AOT publish had been run beside it. A deployment is
    /// the files the host loads, which are the files beside the entry point.
    /// </remarks>
    private static long DirectoryBytes(string directory) =>
        Directory.Exists(directory)
            ? Directory
                .EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly)
                .Sum(static file => new FileInfo(file).Length)
            : 0;

    /// <summary>
    /// The three packable assemblies, where they exist as separate files.
    /// </summary>
    /// <remarks>
    /// On the Native AOT lane they do not: everything is linked into one image and the figure is
    /// zero, which the register reads as "not separable" rather than as "nothing". Package sizes
    /// themselves belong to VM-6, which is where packages are produced at all.
    /// </remarks>
    private static long CoreBytes(string directory) =>
        Length(Path.Combine(directory, "Broiler.VM.Abstractions.dll")) +
        Length(Path.Combine(directory, "Broiler.VM.Binary.dll")) +
        Length(Path.Combine(directory, "Broiler.VM.Runtime.dll"));

    /// <summary>
    /// The resident-set plateau: whether a host that keeps using the core stops growing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Also a recorded figure rather than a measurement. VM-4's soak host declared a plateau in
    /// the managed heap, which is what the collector reports about itself; this one is the
    /// operating system's answer to the same question, and the two are worth having side by side
    /// because a managed heap that plateaus above a resident set that does not is exactly the
    /// shape a native leak takes.
    /// </para>
    /// <para>
    /// The claim is deliberately weak and checkable: the last round's resident set is not larger
    /// than the second round's by more than a stated slack. It is not an assertion that the number
    /// is small, which would be a property of this machine.
    /// </para>
    /// </remarks>
    private static void Plateau(TextWriter writer)
    {
        const int Rounds = 6;
        const int CyclesPerRound = 4_000;

        var resident = new long[Rounds];

        for (var round = 0; round < Rounds; round++)
        {
            for (var cycle = 0; cycle < CyclesPerRound; cycle++)
            {
                // A whole lifecycle each time - runtime, verification, instantiation, invocation,
                // disposal - because a plateau that only held for one stage would say nothing about
                // a host that uses all of them.
                using var runtime = Runtime(Catalog(profiles: 1));
                var instance = Instance(runtime, FixtureArtifactWriter.Constant(cycle));

                Invoke(instance);
                instance.Dispose();
            }

            GC.Collect(2, GCCollectionMode.Forced, blocking: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(2, GCCollectionMode.Forced, blocking: true);

            resident[round] = Resident();

            writer.WriteLine(
                $"  plateau round={round} cycles={(round + 1) * CyclesPerRound} " +
                $"resident-bytes={resident[round]} managed-bytes={GC.GetTotalMemory(forceFullCollection: false)}");
        }

        // The second round rather than the first, because the first still contains whatever the
        // process needed to reach steady state and comparing against it would flatter any result.
        var settled = resident[1];
        var last = resident[Rounds - 1];

        writer.WriteLine(
            $"plateau rounds={Rounds} cycles-per-round={CyclesPerRound} " +
            $"settled-bytes={settled} final-bytes={last} growth-bytes={last - settled} " +
            $"held={(last <= settled + (settled / 16) ? "yes" : "no")}");
    }

    private static long Resident()
    {
        using var process = Process.GetCurrentProcess();

        process.Refresh();
        return process.WorkingSet64;
    }

    /// <summary>
    /// The measurements, each with the control it is attributed against.
    /// </summary>
    /// <remarks>
    /// Every control is the same workload with the core's part removed. Where no such control
    /// exists - runtime creation has no non-core equivalent - the control is the degenerate form of
    /// the same operation and the difference is a MARGINAL cost, which the register says in the row
    /// rather than leaving to be inferred from the name.
    /// </remarks>
    private static Measurement[] Build(TextWriter writer)
    {
        const int LargePool = 4_000;
        const int SmallPool = 2_000;
        const int LoopInstructions = 256;


        var payload = ConstantPool(LargePool);
        var halfPool = ConstantPool(SmallPool);
        var loop = Loop(LoopInstructions);

        // The two host-call artifacts have the SAME instruction count and the same byte length: one
        // spends its middle instruction on a host call and the other on a second push. A control
        // one instruction shorter would attribute a dispatch iteration to the host call, which at
        // these magnitudes is a fifth of the answer.
        var hostCalling = FixtureArtifactWriter.Write(
            [21],
            [
                FixtureFormat.OpPushConst, 0,
                FixtureFormat.OpHostCall, FixtureHostCapabilities.DoubleBinding,
                FixtureFormat.OpReturn,
            ]);

        var noHostCall = FixtureArtifactWriter.Write(
            [21],
            [
                FixtureFormat.OpPushConst, 0,
                FixtureFormat.OpPushConst, 0,
                FixtureFormat.OpReturn,
            ]);

        // The two guest-load artifacts, again at matched instruction counts: one spends its first
        // instruction on a mediated load and the other on a second push.
        var mediatedLoad = Loading(LoadsPerInvocation, load: true);
        var noMediatedLoad = Loading(LoadsPerInvocation, load: false);

        // What the provider hands back, and what the control verifies for itself.
        var nested = NestedPayload;

        // Probed one load at a time, so the figure means loads and not invocations.
        var headroom = Headroom(nested, Loading(loads: 1, load: true));

        writer.WriteLine(
            $"headroom guest-loads-per-runtime={headroom.Loads} stopped-by={headroom.Limit} " +
            $"loads-per-runtime-in-lane={LoadsPerInvocation} lane-iterations={GuestLoadIterations}");

        if (headroom.Loads <= LoadsPerInvocation)
        {
            throw new InvalidOperationException(
                $"the guest-load lane asks one runtime for {LoadsPerInvocation} loads where it " +
                $"admits {headroom.Loads} ({headroom.Limit}), so it would time the refusal path");
        }

        var oneProfile = Catalog(profiles: 1);
        var twoProfiles = Catalog(profiles: 2);

        // Holders rather than locals, because EVERY lane long enough to be worth timing outlives an
        // allowance. A runtime's ceilings are totals: two hundred verifications of a
        // thirty-two-kilobyte pool spend sixty-four megabytes of AllocatedBytes, and twenty
        // thousand invocations of a two-hundred-and-fifty-six instruction loop spend more Fuel than
        // the profile's hard maximum admits. Both were found the same way - by the harness checking
        // that the operation still worked AFTER a lane, not only before it - and both are fixed the
        // same way, by starting each lane from a runtime that has spent nothing.
        var verifying = new RuntimeHolder(() => Runtime(oneProfile));

        var charging = new InstanceHolder(() => Runtime(oneProfile), Instance, loop);

        var free = new InstanceHolder(
            () => Runtime(Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.NonCharging))),
            Instance,
            loop);

        // No Reset on this one, deliberately. A whole run spends well under the profile's
        // million-host-call maximum, and a reset is not free: rebuilding the runtime between lanes
        // put a hundredfold more noise into this lane's A/A floor than the effect it was measuring.
        // A lane gets a reset when it needs one, and the check after every lane is what decides
        // whether it needs one rather than a guess made here.
        var hosting = new InstanceHolder(
            () => Runtime(oneProfile), Instance, hostCalling, noHostCall);

        var declaringCatalog = DeclaringCatalog();

        // One envelope, minted once. The projection lane reads the SAME result repeatedly rather
        // than invoking each time, because the projection is a handful of comparisons against an
        // invocation two orders of magnitude larger: measured through an invocation it would sit
        // below the A/A floor and be reported as no measurement at all. It holds its payload by
        // reference and outlives the runtime that produced it, so replacing that runtime later
        // leaves this result exactly as valid as it was.
        var envelope = Invoke(hosting[1]);

        var alpha = FixtureVmProfile.Id;

        return
        [
            new Measurement(
                "verify-throughput",
                "verification of a large artifact against a raw pass over the same bytes",
                "byte",
                payload.Length,
                () => Verify(verifying.Current, payload),
                () => Checksum(payload),
                Iterations: 200,
                () => VerifiesCleanly(verifying.Current, payload),
                verifying.Replace),

            new Measurement(
                "verify-per-declared-count",
                "verifying a four-thousand-constant pool against a two-thousand-constant one",
                "constant",
                LargePool - SmallPool,
                () => Verify(verifying.Current, payload),
                () => Verify(verifying.Current, halfPool),
                Iterations: 200,
                () => VerifiesCleanly(verifying.Current, payload) ?? VerifiesCleanly(verifying.Current, halfPool),
                verifying.Replace),

            new Measurement(
                "catalog-construction",
                "building a two-profile catalog against building the same two descriptors into an array",
                "profile",
                2,
                static () => VmCatalog.CreateBuilder()
                    .Add(FixtureVmProfile.Descriptor)
                    .Add(SecondFixtureVmProfile.Descriptor)
                    .Build(),
                static () => ImmutableArray.CreateBuilder<VmProfileDescriptor>(2)
                    .Also(FixtureVmProfile.Descriptor, SecondFixtureVmProfile.Descriptor),
                Iterations: 2_000),

            new Measurement(
                "catalog-lookup",
                "resolving a profile by identity against comparing the same identity",
                "lookup",
                1,
                () => twoProfiles.TryGetEntry(alpha, out _),
                () => alpha.Equals(SecondFixtureVmProfile.Id),
                Iterations: 200_000),

            new Measurement(
                "runtime-create-dispose",
                "creating and disposing a two-profile runtime against a one-profile runtime",
                "profile",
                1,
                () => Runtime(twoProfiles).Dispose(),
                () => Runtime(oneProfile).Dispose(),
                Iterations: 2_000),

            new Measurement(
                "meter-per-instruction",
                "one invocation with fuel charging against the same executor with charging off",
                "instruction",
                LoopInstructions,
                () => Invoke(charging[0]),
                () => Invoke(free[0]),
                Iterations: 20_000,
                () => RunsCleanly(charging[0]) ?? RunsCleanly(free[0]),
                () =>
                {
                    charging.Replace();
                    free.Replace();
                }),

            new Measurement(
                "host-call",
                "an artifact that makes one host call against the same shape without one",
                "call",
                1,
                () => Invoke(hosting[0]),
                () => Invoke(hosting[1]),
                Iterations: 20_000,
                () => RunsCleanly(hosting[0]) ?? RunsCleanly(hosting[1])),

            new Measurement(
                "diagnostics-capture",
                "building a fully identified diagnostics record against its minimal form",
                "record",
                1,
                static () => Identified(),
                static () => Minimal(),
                Iterations: 200_000),

            // The control is the same nested verification the host would have performed itself,
            // beside an invocation of matched shape - so the difference is the MEDIATION and not
            // the nested verification, which a caller pays either way. What the difference contains
            // is the provider dispatch, the request and answer marshalling, the depth and fan-out
            // accounting, the charge against the requesting operation, and the intersection of the
            // nested handle's ceilings with that operation's remaining allowance.
            new Measurement(
                "guest-load-mediation",
                "mediated guest-initiated loads against the same loads performed by the host itself",
                "load",
                LoadsPerInvocation,
                () => Mediated(declaringCatalog, nested, mediatedLoad),
                () => Direct(declaringCatalog, nested, noMediatedLoad, LoadsPerInvocation),
                Iterations: GuestLoadIterations,
                () => Mediates(declaringCatalog, nested, mediatedLoad),
                UpperBoundIfBelowNoise: true),

            // Reading a typed payload back out of the profile-neutral envelope: the identity check
            // the projection performs before it casts, against reading the envelope's category
            // without unwrapping anything. The difference is what payload projection costs a
            // consumer, which is the "envelope read" the gate asks for.
            new Measurement(
                "envelope-read",
                "projecting a typed payload out of a result against reading its category alone",
                "projection",
                1,
                () =>
                {
                    if (FixtureVmProfileResults.TryGetValue(in envelope, out _))
                    {
                        Sink++;
                    }
                },
                () =>
                {
                    if (envelope.IsSuccess)
                    {
                        Sink++;
                    }
                },
                Iterations: 200_000),
        ];
    }

    /// <summary>
    /// One runtime's worth of mediated loads: the candidate lane of the guest-load measurement.
    /// </summary>
    /// <remarks>
    /// The runtime, the verification and the instantiation are all inside the timed region, and all
    /// three appear identically in <see cref="Direct"/>. They are the fixture cost of getting to the
    /// thing being measured, and a fixture cost present in both lanes cancels in the difference.
    /// </remarks>
    private static void Mediated(VmCatalog catalog, byte[] nested, byte[] loading)
    {
        using var runtime = Runtime(catalog, Provider(nested));
        var instance = Instance(runtime, loading);

        Invoke(instance);
        instance.Dispose();
    }

    /// <summary>
    /// The control: the same loads, performed by the host itself rather than through the mediator.
    /// </summary>
    /// <remarks>
    /// The invocation runs an artifact of matched instruction count with its loads replaced by
    /// pushes, and the nested verifications the guest would have triggered are then performed
    /// directly. So both lanes verify the same nested bytes the same number of times, and the
    /// difference is what routing them through the mediator adds: the provider dispatch, the
    /// request and answer marshalling, the depth and fan-out accounting, the charge against the
    /// requesting operation, and the intersection of the nested handle's ceilings with that
    /// operation's remaining allowance.
    /// </remarks>
    private static void Direct(VmCatalog catalog, byte[] nested, byte[] plain, int loads)
    {
        using var runtime = Runtime(catalog, Provider(nested));
        var instance = Instance(runtime, plain);

        Invoke(instance);

        for (var load = 0; load < loads; load++)
        {
            Verify(runtime, nested);
        }

        instance.Dispose();
    }

    /// <summary>Whether the mediated lane really mediates, or why it does not.</summary>
    private static string? Mediates(VmCatalog catalog, byte[] nested, byte[] loading)
    {
        using var runtime = Runtime(catalog, Provider(nested));
        var instance = Instance(runtime, loading);

        try
        {
            return RunsCleanly(instance);
        }
        finally
        {
            instance.Dispose();
        }
    }

    /// <summary>
    /// An artifact that requests <paramref name="loads"/> guest loads, or pushes instead.
    /// </summary>
    /// <remarks>
    /// One builder for both, so the candidate and the control differ in one opcode and in nothing
    /// else - not in instruction count, not in constant-pool size, not in byte length. Building the
    /// control separately is how a control acquires a second difference nobody notices.
    /// </remarks>
    private static byte[] Loading(int loads, bool load)
    {
        var code = new List<byte>();

        for (var index = 0; index < loads; index++)
        {
            code.Add(load ? FixtureFormat.OpLoad : FixtureFormat.OpPushConst);
            code.Add(0);
        }

        code.Add(FixtureFormat.OpPushConst);
        code.Add(1);
        code.Add(FixtureFormat.OpReturn);

        return FixtureArtifactWriter.Write([1, 7], code.ToArray());
    }

    /// <summary>One verification, with its handle released immediately.</summary>
    private static void Verify(VmRuntime runtime, byte[] payload)
    {
        var descriptor = Descriptor();
        var result = runtime.Verify(in descriptor, payload, CancellationToken.None);

        result.TryGetArtifact(out var artifact);
        artifact?.Dispose();
    }

    /// <summary>
    /// Whether a verification of these bytes really succeeds, or why it does not.
    /// </summary>
    /// <remarks>
    /// Called once before a measurement is timed. It is not a check that the harness works; it is a
    /// check that the thing being timed is the thing being named.
    /// </remarks>
    private static string? VerifiesCleanly(VmRuntime runtime, byte[] payload)
    {
        var descriptor = Descriptor();
        var result = runtime.Verify(in descriptor, payload, CancellationToken.None);

        if (!result.TryGetArtifact(out var artifact))
        {
            return $"verification answered {result.Outcome}/{result.Reason} " +
                $"({result.Diagnostics.ExhaustedDimension}/{result.Diagnostics.ExhaustedScope})";
        }

        artifact.Dispose();
        return null;
    }

    /// <summary>Whether an invocation really completes, or why it does not.</summary>
    private static string? RunsCleanly(VmInstance instance)
    {
        var result = Invoke(instance);

        return result.Outcome is VmOutcome.Normal
            ? null
            : $"invocation answered {result.Outcome}/{result.Reason} " +
                $"({result.Diagnostics.ExhaustedDimension}/{result.Diagnostics.ExhaustedScope})";
    }

    /// <summary>
    /// One invocation of the fixture's single entry point.
    /// </summary>
    /// <remarks>
    /// The request is built inside rather than hoisted, because <c>VmInvocationRequest</c> is a ref
    /// struct and a lambda cannot close over one. Building it costs a span over a UTF-8 literal,
    /// which is the same cost in the candidate and in the control and therefore cancels.
    /// </remarks>
    private static VmInvocationResult Invoke(VmInstance instance)
    {
        var request = new VmInvocationRequest(new VmUtf8Text("main"u8));
        return instance.Invoke(in request, CancellationToken.None);
    }

    /// <summary>A fully identified diagnostics record: every group a real failure fills.</summary>
    private static VmDiagnostics Identified() =>
        VmDiagnostics
            .Create(VmStage.Verification, VmOutcome.InvalidArtifact, VmReason.Truncated,
                VmObjectId.Mint(), VmInitiator.Guest, VmAttemptedCall.Verify)
            .WithProfile(FixtureVmProfile.Id, FixtureFormat.FormatVersion, FixtureVmProfile.Manifest, 1)
            .WithArtifact(VmObjectId.Mint(), 4096, VmCallerIdentity.FromCanonicalIdentity("bench://artifact"))
            .WithPosition(new VmSourcePosition(0, 17, 0, 0), 1001);

    /// <summary>The minimal record: stage, outcome, reason and who asked.</summary>
    private static VmDiagnostics Minimal() =>
        VmDiagnostics.Create(VmStage.Verification, VmOutcome.InvalidArtifact, VmReason.Truncated,
            VmObjectId.Mint(), VmInitiator.Guest, VmAttemptedCall.Verify);

    /// <summary>
    /// The control for verification: the same bytes, read once, with no verifier.
    /// </summary>
    /// <remarks>
    /// It is a checksum rather than an empty loop because an empty loop over a span is not a pass
    /// over memory - the compiler is entitled to notice. Summing every byte forces the same reads
    /// the verifier performs and nothing else, which is the closest a control can get to "what this
    /// machine costs to look at these bytes".
    /// </remarks>
    private static long Checksum(byte[] payload)
    {
        long sum = 0;

        foreach (var value in payload)
        {
            sum += value;
        }

        return sum;
    }

    /// <summary>
    /// An artifact whose whole size is its constant pool.
    /// </summary>
    /// <remarks>
    /// A three-instruction program over a large pool, so verification cost is the pool and the
    /// executor contributes nothing worth measuring. Two of these at different sizes are what make
    /// the per-declared-count attribution possible: the difference between them is the pool and
    /// nothing else, because every other part of both artifacts is identical.
    /// </remarks>
    private static byte[] ConstantPool(int constants)
    {
        var pool = new long[constants];

        for (var index = 0; index < constants; index++)
        {
            pool[index] = index;
        }

        return FixtureArtifactWriter.Write(
            pool, [FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn]);
    }

    /// <summary>
    /// A program of a known instruction count, so per-instruction attribution is a division rather
    /// than an estimate.
    /// </summary>
    /// <remarks>
    /// Every instruction is a no-op: the dispatch loop's own work per instruction is what the
    /// metering measurement attributes against, and an instruction that computed something would
    /// put the computation in both the candidate and the control - where it cancels - while making
    /// the loop longer for no gain.
    /// </remarks>
    private static byte[] Loop(int instructions)
    {
        var code = new byte[instructions + 1];

        for (var index = 0; index < instructions; index++)
        {
            code[index] = FixtureFormat.OpNop;
        }

        code[instructions] = FixtureFormat.OpReturn;

        return FixtureArtifactWriter.Write([0], code);
    }

    private static VmCatalog Catalog(int profiles)
    {
        var builder = VmCatalog.CreateBuilder().Add(FixtureVmProfile.Descriptor);

        if (profiles > 1)
        {
            builder = builder.Add(SecondFixtureVmProfile.Descriptor);
        }

        return builder.Build();
    }

    /// <summary>The catalog holding the one profile that declares it may request loads.</summary>
    private static VmCatalog DeclaringCatalog() =>
        Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.DeclaresGuestLoads));

    private static VmCatalog Catalog(VmProfileDescriptor descriptor) =>
        VmCatalog.CreateBuilder().Add(descriptor).Build();

    /// <summary>The provider registration a guest-load composition adds to the value capabilities.</summary>
    private static IVmArtifactProvider Provider(byte[] answer) =>
        new FixtureArtifactProvider(FixtureVmProfile.Id, answer);

    private static VmRuntime Runtime(VmCatalog catalog) => Runtime(catalog, provider: null);

    private static VmRuntime Runtime(VmCatalog catalog, IVmArtifactProvider? provider)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.Value(dimension, ulong.MaxValue / 4));
        }

        var capabilities = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        capabilities.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Double, FixtureHostCapabilities.DoubleHandler));
        capabilities.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Throwing, FixtureHostCapabilities.ThrowingHandler));
        capabilities.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Refusing, FixtureHostCapabilities.RefusingHandler));

        if (provider is not null)
        {
            capabilities.Add(VmCapabilityRegistration.ArtifactProvider(
                FixtureHostCapabilities.Provider, provider));
        }

        // Ceilings far above anything these measurements reach, so no figure here is the cost of
        // approaching a limit. A measurement that exhausted its allowance halfway through would be
        // measuring the refusal path and calling it throughput.
        var options = new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(5),
            maxLiveSuspendedOperations: 8,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities.ToImmutable());

        RuntimesCreated++;
        var created = VmRuntime.Create(catalog, options);

        if (!created.TryGetRuntime(out var runtime))
        {
            throw new InvalidOperationException(
                $"runtime creation failed: {created.Outcome}/{created.Reason}");
        }

        return runtime;
    }

    private static VmInstance Instance(VmRuntime runtime, byte[] payload)
    {
        var descriptor = Descriptor();
        var verified = runtime.Verify(in descriptor, payload, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            throw new InvalidOperationException(
                $"verification failed: {verified.Outcome}/{verified.Reason}");
        }

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            throw new InvalidOperationException(
                $"instantiation failed: {instantiated.Outcome}/{instantiated.Reason}");
        }

        return instance;
    }

    private static VmArtifactDescriptor Descriptor() =>
        new(FixtureVmProfile.Id, FixtureFormat.FormatVersion, FixtureVmProfile.Manifest, default,
            VmCallerIdentity.FromCanonicalIdentity("bench://artifact"));
}

/// <summary>
/// A replaceable runtime, so a lane can start from a fresh allowance without the timed action
/// knowing.
/// </summary>
/// <remarks>
/// The alternative was to create a runtime inside the timed action, which would put two microseconds
/// of construction into every iteration of a measurement whose whole point is the microseconds - and
/// into the candidate only, since the control is not a runtime operation at all.
/// </remarks>
internal sealed class RuntimeHolder(Func<VmRuntime> factory)
{
    private VmRuntime current = factory();

    internal VmRuntime Current => current;

    internal void Replace()
    {
        current.Dispose();
        current = factory();
    }
}

/// <summary>
/// A replaceable runtime together with the instances bound to it.
/// </summary>
/// <remarks>
/// An instance outlives no runtime, so a lane that has to start from a fresh allowance has to
/// rebuild both. Verifying and instantiating inside the timed action instead would put a
/// verification into every iteration of a measurement whose whole subject is what happens after
/// one.
/// </remarks>
internal sealed class InstanceHolder
{
    private readonly Func<VmRuntime> factory;
    private readonly Func<VmRuntime, byte[], VmInstance> bind;
    private readonly byte[][] payloads;

    private VmRuntime runtime;
    private VmInstance[] instances;

    internal InstanceHolder(
        Func<VmRuntime> factory,
        Func<VmRuntime, byte[], VmInstance> bind,
        params byte[][] payloads)
    {
        this.factory = factory;
        this.bind = bind;
        this.payloads = payloads;

        runtime = factory();
        instances = Bind();
    }

    internal VmRuntime Runtime => runtime;

    internal VmInstance this[int index] => instances[index];

    internal void Replace()
    {
        // The runtime's own disposal drains and releases the instances it owns, so they are not
        // disposed individually here: doing both would be the second disposal of an object the
        // runtime is already draining, and idempotent disposal makes that harmless rather than
        // correct.
        runtime.Dispose();
        runtime = factory();
        instances = Bind();
    }

    private VmInstance[] Bind()
    {
        var bound = new VmInstance[payloads.Length];

        for (var index = 0; index < payloads.Length; index++)
        {
            bound[index] = bind(runtime, payloads[index]);
        }

        return bound;
    }
}

/// <summary>A builder terminator, so the catalog control ends in the same shape as the candidate.</summary>
/// <remarks>
/// The control must actually build something: a builder nobody drains is an allocation the
/// collector never sees, and comparing it against a catalog that really was constructed would
/// attribute the difference to the core.
/// </remarks>
internal static class BuilderExtensions
{
    internal static ImmutableArray<VmProfileDescriptor> Also(
        this ImmutableArray<VmProfileDescriptor>.Builder builder,
        VmProfileDescriptor first,
        VmProfileDescriptor second)
    {
        builder.Add(first);
        builder.Add(second);
        return builder.ToImmutable();
    }
}
