using System.Diagnostics;

namespace Broiler.VM.Bench.Host;

/// <summary>
/// One measurement: a candidate, the control it is compared against, and what a difference between
/// them would mean.
/// </summary>
/// <remarks>
/// <para>
/// The control is the load-bearing field. A number on its own is a property of this machine and
/// tells a profile author nothing; the same number beside a control that differs only in the core's
/// involvement is an attribution. Every control here is the SAME workload with the core's part
/// removed - the same bytes scanned without a verifier, the same dispatch loop with charging off,
/// the same artifact without the host call - so the difference is the core's cost and not a
/// comparison between two different programs.
/// </para>
/// <para>
/// <c>Unit</c> is what the difference is divided by to get a per-operation figure, which is the
/// gate's "per-operation attribution": a verification is attributed per byte, a host call per call,
/// an instruction per instruction.
/// </para>
/// <para>
/// <c>UpperBoundIfBelowNoise</c> separates two things a bare validity rule conflates. A difference
/// smaller than the A/A floor usually means the measurement is broken; it can also mean the
/// operation genuinely costs nothing measurable, which is a result and not a failure. Setting it
/// says the second is an admissible answer HERE, and the harness then publishes the noise floor as
/// an upper bound - "smaller than this" - rather than refusing. It is a claim made in advance and
/// restated in the register, because a flag set after seeing a disappointing number would be a way
/// to launder a broken lane into a null result.
/// </para>
/// </remarks>
internal sealed record Measurement(
    string Id,
    string What,
    string Unit,
    long UnitsPerIteration,
    Action Candidate,
    Action Control,
    int Iterations,
    Func<string?>? Condition = null,
    Action? Reset = null,
    bool UpperBoundIfBelowNoise = false);

/// <summary>What one lane of one measurement observed.</summary>
internal readonly record struct Lane(
    double NanosecondsPerIteration,
    long AllocatedBytesPerIteration,
    int Gen0,
    int Gen1,
    int Gen2);

/// <summary>
/// The harness: interleaved candidate and control, two identical lanes, every repetition retained.
/// </summary>
/// <remarks>
/// <para>
/// It is deliberately small enough to read. There is no pilot phase, no outlier policy and no
/// statistical model: a policy that discarded a repetition would be a judgement about which
/// measurements count, and the whole point of retaining every repetition is that the reader makes
/// that judgement rather than the tool.
/// </para>
/// <para>
/// <b>Interleaving.</b> Candidate and control alternate within a repetition rather than running in
/// two blocks, so a machine that gets slower - thermal throttling, another process arriving, a
/// migration between cores - slows both by roughly the same amount instead of slowing whichever
/// block ran last. Two blocks is how a drifting machine turns into a performance claim.
/// </para>
/// <para>
/// <b>The A/A lane.</b> The candidate is measured twice, identically, and the difference between
/// those two lanes is the noise floor of this machine on this workload. A candidate-versus-control
/// difference smaller than the A/A difference is not a measurement of anything, and the harness
/// says so rather than publishing it. This is the one judgement it makes.
/// </para>
/// </remarks>
internal static class Harness
{
    /// <summary>How many repetitions each lane runs. Every one is retained.</summary>
    internal const int Repetitions = 7;

    /// <summary>
    /// How many warmup iterations run before the first measured one.
    /// </summary>
    /// <remarks>
    /// Enough to reach steady state on the JIT lane, where a first call is a compilation. The
    /// Native AOT lane needs none and gets the same number anyway, because a harness that behaved
    /// differently per lane would make the two lanes incomparable - which is the only thing the two
    /// lanes exist to be.
    /// </remarks>
    internal const int WarmupIterations = 3;

    internal static void Run(Measurement measurement, TextWriter writer)
    {
        // The candidate must actually do what its name says, before anything is timed AND after
        // every timed lane. A measurement whose operation quietly failed is the most dangerous
        // output a harness can produce: it is fast, it is stable, and it is a number for the
        // refusal path. The per-declared-count measurement was exactly that on its first run.
        //
        // Checking only on entry is not enough, and finding that out is what shaped this member. A
        // budget dimension is a TOTAL rather than a per-operation limit, so a lane long enough to
        // be worth timing is long enough to spend one, and the lane that spends its allowance
        // half-way through starts from a candidate that works and finishes measuring a refusal. The
        // check therefore runs on both sides of every lane, where it costs one operation and
        // catches exactly that.
        Check(measurement, writer);
        Warm(measurement);

        var candidate = new Lane[Repetitions];
        var control = new Lane[Repetitions];
        var second = new Lane[Repetitions];

        for (var repetition = 0; repetition < Repetitions; repetition++)
        {
            // Interleaved, and in this order every time: the order itself is a variable, so it is
            // held constant rather than randomised. A/A being the third means any systematic
            // advantage to running first belongs to the candidate lane, which is the conservative
            // direction: it makes the A/A difference larger and the measurement harder to publish.
            //
            // The reset runs OUTSIDE every timed region, before each lane. A runtime's allowances
            // are totals rather than per-operation limits, so a lane long enough to be worth timing
            // is also long enough to spend one - and a spent runtime answers every later call with
            // a fast refusal, which is a stable, plausible and entirely wrong measurement.
            measurement.Reset?.Invoke();
            candidate[repetition] = Measure(measurement.Candidate, measurement.Iterations);
            Check(measurement, writer);

            measurement.Reset?.Invoke();
            control[repetition] = Measure(measurement.Control, measurement.Iterations);
            Check(measurement, writer);

            measurement.Reset?.Invoke();
            second[repetition] = Measure(measurement.Candidate, measurement.Iterations);
            Check(measurement, writer);
        }

        var candidateTime = Median(candidate);
        var controlTime = Median(control);
        var secondTime = Median(second);

        var difference = candidateTime - controlTime;
        var lane = Math.Abs(candidateTime - secondTime);

        // The per-operation attribution, which is what a profile author budgets against.
        var perUnit = measurement.UnitsPerIteration > 0
            ? difference / measurement.UnitsPerIteration
            : difference;

        var resolved = lane <= Math.Abs(difference);
        var valid = resolved || measurement.UpperBoundIfBelowNoise;

        // A null result carries the floor it was measured against, because "no difference" without
        // one says nothing: the reader needs to know how small a difference would still have been
        // seen.
        var bound = measurement.UnitsPerIteration > 0
            ? lane / measurement.UnitsPerIteration
            : lane;

        writer.WriteLine(
            $"measurement {measurement.Id} unit={measurement.Unit} " +
            $"candidate-ns={candidateTime:F1} control-ns={controlTime:F1} " +
            $"difference-ns={difference:F1} per-{measurement.Unit}-ns={perUnit:F4} " +
            $"aa-ns={lane:F1} valid={(resolved ? "yes" : valid ? "bound" : "no")} " +
            (resolved ? string.Empty : $"upper-bound-per-{measurement.Unit}-ns={bound:F4} ") +
            $"candidate-alloc={Median(candidate, static l => l.AllocatedBytesPerIteration):F0} " +
            $"control-alloc={Median(control, static l => l.AllocatedBytesPerIteration):F0} " +
            $"gen0={candidate[0].Gen0} gen1={candidate[0].Gen1} gen2={candidate[0].Gen2} " +
            $"iterations={measurement.Iterations} repetitions={Repetitions}");

        // Every repetition, retained. A summary is what a reader has to trust; the repetitions are
        // what a reader can check, and the spread between them is most of what a single figure
        // hides.
        for (var repetition = 0; repetition < Repetitions; repetition++)
        {
            writer.WriteLine(
                $"  rep {measurement.Id} {repetition} " +
                $"candidate-ns={candidate[repetition].NanosecondsPerIteration:F1} " +
                $"control-ns={control[repetition].NanosecondsPerIteration:F1} " +
                $"aa-ns={second[repetition].NanosecondsPerIteration:F1} " +
                $"candidate-alloc={candidate[repetition].AllocatedBytesPerIteration} " +
                $"control-alloc={control[repetition].AllocatedBytesPerIteration}");
        }
    }

    /// <summary>
    /// Asserts that the measurement's operation still does what its name says, or stops the run.
    /// </summary>
    /// <remarks>
    /// It throws rather than marking the measurement invalid, because an operation that stopped
    /// working mid-lane has already corrupted the repetitions this run would otherwise print, and a
    /// harness that printed them beside a warning would be inviting someone to read them anyway.
    /// </remarks>
    private static void Check(Measurement measurement, TextWriter writer)
    {
        var problem = measurement.Condition?.Invoke();

        if (problem is null)
        {
            return;
        }

        writer.WriteLine($"measurement {measurement.Id} REFUSED: {problem}");
        throw new InvalidOperationException($"{measurement.Id}: {problem}");
    }

    private static void Warm(Measurement measurement)
    {
        measurement.Reset?.Invoke();

        for (var iteration = 0; iteration < WarmupIterations; iteration++)
        {
            measurement.Candidate();
            measurement.Control();
        }
    }

    /// <summary>
    /// Runs one action <paramref name="iterations"/> times and reports the per-iteration cost.
    /// </summary>
    /// <remarks>
    /// A settling collection before each lane, so a lane does not pay for the previous lane's
    /// garbage; the allocation figure is read from the thread's own allocation counter rather than
    /// from the heap size, because the heap size is what the collector felt like doing.
    /// </remarks>
    private static Lane Measure(Action action, int iterations)
    {
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();

        var gen0 = GC.CollectionCount(0);
        var gen1 = GC.CollectionCount(1);
        var gen2 = GC.CollectionCount(2);
        var allocated = GC.GetAllocatedBytesForCurrentThread();

        var stopwatch = Stopwatch.StartNew();

        for (var iteration = 0; iteration < iterations; iteration++)
        {
            action();
        }

        stopwatch.Stop();

        var elapsed = stopwatch.Elapsed.TotalNanoseconds() / iterations;
        var bytes = (GC.GetAllocatedBytesForCurrentThread() - allocated) / iterations;

        return new Lane(
            elapsed,
            bytes,
            GC.CollectionCount(0) - gen0,
            GC.CollectionCount(1) - gen1,
            GC.CollectionCount(2) - gen2);
    }

    /// <summary>
    /// The median of the repetitions, which is the one summary this harness computes.
    /// </summary>
    /// <remarks>
    /// A median rather than a mean, because a single interruption - a collection, a scheduler
    /// decision, another process - moves a mean and does not move a median, and neither is a
    /// property of the code. The repetitions are printed beside it so the choice is visible rather
    /// than load-bearing.
    /// </remarks>
    private static double Median(Lane[] lanes) =>
        Median(lanes, static lane => lane.NanosecondsPerIteration);

    private static double Median<T>(Lane[] lanes, Func<Lane, T> select)
        where T : IConvertible
    {
        var values = lanes
            .Select(lane => select(lane).ToDouble(System.Globalization.CultureInfo.InvariantCulture))
            .OrderBy(static value => value)
            .ToArray();

        return values.Length % 2 == 1
            ? values[values.Length / 2]
            : (values[(values.Length / 2) - 1] + values[values.Length / 2]) / 2;
    }
}

/// <summary>A nanosecond view of a timespan, so the harness never rounds through milliseconds.</summary>
internal static class TimeSpanExtensions
{
    internal static double TotalNanoseconds(this TimeSpan span) =>
        span.Ticks * (1_000_000_000.0 / TimeSpan.TicksPerSecond);
}
