// Reflection probe over one variant's Broiler.VM.Runtime: exactness traces, a concurrent settle stress, a
// TryCharge microbenchmark, and two concurrency benchmarks.
// Usage: Probe <variant bin dir> [trace|stress|bench|concurrent-bench|concurrent-ambient|all]
//
// The bindings name the landed members: VmBudgetLevel.FuelPreAdmissions carries the runtime's table and
// VmFuelPreAdmissions.SettleAll empties it. A build with neither - the base build - binds nothing and every
// reader then reads the level directly, which is what the base build's readers do.
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

internal static class Program
{
    private const BindingFlags NP = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
    private const BindingFlags NS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    private static Assembly abs, rt;
    private static Type dimT, scopeT, levelT, meterT, iMeterT, snapT, execScopeT, ambientT;
    private static object fuel, callDepth, liveBytes;
    private static int count;
    private static MethodInfo settleAll, levelSnapshot, meterSnapshot, consumed, enter, leave;
    private static PropertyInfo preAdmissions;
    private static PropertyInfo failedDimension, failedScope, exhaustionObserved, pollBoundExceeded, unpolledExceeds;
    private static Func<object, ulong, bool> chargeFuel, chargeDepth;
    private static Func<object, bool> poll;
    private static Action<object, ulong> retain, release, releaseDepth;

    private static int Main(string[] args)
    {
        // A retained log is read on another machine. A figure whose decimal separator is this
        // workstation's language is a figure a reader has to guess at, so every number is written invariant.
        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        var bin = args[0];
        var mode = args.Length > 1 ? args[1] : "all";
        abs = Assembly.LoadFrom(Path.Combine(bin, "Broiler.VM.Abstractions.dll"));
        rt = Assembly.LoadFrom(Path.Combine(bin, "Broiler.VM.Runtime.dll"));
        dimT = Find("Broiler.VM.VmBudgetDimension");
        scopeT = Find("Broiler.VM.VmBudgetScope");
        levelT = Find("Broiler.VM.VmBudgetLevel");
        meterT = Find("Broiler.VM.VmMeter");
        iMeterT = Find("Broiler.VM.IVmMeter");
        snapT = Find("Broiler.VM.VmBudgetSnapshot");
        execScopeT = Find("Broiler.VM.VmExecutionScope");
        ambientT = Find("Broiler.VM.VmAmbientMeter");
        var dims = Find("Broiler.VM.VmBudgetDimensions");
        count = Convert.ToInt32((object)dims.GetField("Count", NS)?.GetValue(null) ?? dims.GetProperty("Count", NS).GetValue(null));
        fuel = Enum.Parse(dimT, "Fuel");
        callDepth = Enum.Parse(dimT, "CallDepth");
        liveBytes = Enum.Parse(dimT, "LiveBytes");
        preAdmissions = levelT.GetProperty("FuelPreAdmissions", NP);
        settleAll = rt.GetType("Broiler.VM.VmFuelPreAdmissions")?.GetMethod("SettleAll", NP);
        levelSnapshot = levelT.GetMethod("Snapshot", NP);
        meterSnapshot = meterT.GetMethod("Snapshot", NP);
        consumed = snapT.GetMethod("Consumed");
        enter = execScopeT.GetMethod("Enter", NP);
        leave = execScopeT.GetMethod("Leave", NP);
        failedDimension = meterT.GetProperty("FailedDimension", NP);
        failedScope = meterT.GetProperty("FailedScope", NP);
        exhaustionObserved = meterT.GetProperty("ExhaustionObserved", NP);
        pollBoundExceeded = meterT.GetProperty("PollBoundExceeded", NP);
        unpolledExceeds = meterT.GetProperty("UnpolledWorkExceedsBound", NP);
        chargeFuel = Charger(fuel);
        chargeDepth = Charger(callDepth);
        poll = Poller();
        retain = Reporter("ReportRetained", liveBytes);
        release = Reporter("ReportReleased", liveBytes);
        releaseDepth = Reporter("ReportReleased", callDepth);
        Console.WriteLine($"variant={Path.GetFileName(bin.TrimEnd('/', '\\'))} preAdmissionTable={(settleAll is not null && preAdmissions is not null)}");

        var failures = 0;

        if (mode is "trace" or "all")
        {
            foreach (var tight in new[] { "runtime", "instance", "invocation", "tie" })
            {
                foreach (var seed in new[] { 1, 2, 3 })
                {
                    Console.WriteLine(Sequential(tight, seed));
                }
            }

            foreach (var seed in new[] { 1, 2, 3 })
            {
                Console.WriteLine(TwoMeters(seed));
                Console.WriteLine(Handoff(seed));
            }
        }

        if (mode is "stress" or "all")
        {
            for (var round = 0; round < 5; round++)
            {
                failures += Stress(round, maxAmount: 1);
                failures += Stress(round, maxAmount: 3);
            }
        }

        if (mode is "bench" or "all")
        {
            Bench();
        }

        if (mode is "concurrent-bench" or "all")
        {
            Concurrent("concurrent-bench");
        }

        if (mode is "concurrent-ambient" or "all")
        {
            Concurrent("concurrent-ambient");
        }

        Console.WriteLine($"stress_failures={failures}");
        return failures == 0 ? 0 : 1;
    }

    private static Type Find(string name) => abs.GetType(name) ?? rt.GetType(name) ?? throw new InvalidOperationException(name);

    private static Func<object, ulong, bool> Charger(object dimension)
    {
        var m = Expression.Parameter(typeof(object));
        var a = Expression.Parameter(typeof(ulong));
        var call = Expression.Call(Expression.Convert(m, iMeterT), iMeterT.GetMethod("TryCharge"), Expression.Constant(dimension, dimT), a);
        return Expression.Lambda<Func<object, ulong, bool>>(call, m, a).Compile();
    }

    private static Func<object, bool> Poller()
    {
        var m = Expression.Parameter(typeof(object));
        return Expression.Lambda<Func<object, bool>>(Expression.Call(Expression.Convert(m, iMeterT), iMeterT.GetMethod("Poll")), m).Compile();
    }

    private static Action<object, ulong> Reporter(string name, object dimension)
    {
        var m = Expression.Parameter(typeof(object));
        var a = Expression.Parameter(typeof(ulong));
        var call = Expression.Call(Expression.Convert(m, iMeterT), iMeterT.GetMethod(name), Expression.Constant(dimension, dimT), a);
        return Expression.Lambda<Action<object, ulong>>(call, m, a).Compile();
    }

    private static ulong[] Ceilings(ulong fuelCeiling)
    {
        var ceilings = new ulong[count];
        Array.Fill(ceilings, ulong.MaxValue);
        ceilings[Convert.ToInt32(fuel)] = fuelCeiling;
        return ceilings;
    }

    private static object Level(string scope, ulong fuelCeiling) =>
        Activator.CreateInstance(levelT, NP, null, new object[] { Enum.Parse(scopeT, scope), Ceilings(fuelCeiling) }, null);

    private static object Meter(object gate, object invocation, object instance, object runtime, ulong pollBound) =>
        meterT.GetConstructors(NP).Single().Invoke(new object[] { gate, invocation, instance, runtime, null, pollBound, CancellationToken.None });

    // What VmRuntime.GetBudgetSnapshot does: settle every holder of a block against this runtime, then read
    // the level. On a build with no table there is nothing to settle and the read is the whole of it.
    private static ulong LevelFuel(object gate, object runtimeLevel, object level)
    {
        lock (gate)
        {
            if (settleAll is not null && preAdmissions?.GetValue(runtimeLevel) is object table)
            {
                settleAll.Invoke(table, null);
            }

            return (ulong)consumed.Invoke(levelSnapshot.Invoke(level, null), new[] { fuel });
        }
    }

    private static ulong InvocationFuel(object meter) => (ulong)consumed.Invoke(meterSnapshot.Invoke(meter, null), new[] { fuel });

    private static string State(object meter) =>
        $"{failedDimension.GetValue(meter)}/{failedScope.GetValue(meter)} exh={exhaustionObserved.GetValue(meter)} pbe={pollBoundExceeded.GetValue(meter)}";

    private sealed class Rng(int seed)
    {
        private ulong state = (ulong)seed * 0x9E3779B97F4A7C15UL + 1;

        internal int Next(int bound)
        {
            state ^= state << 13;
            state ^= state >> 7;
            state ^= state << 17;
            return (int)(state % (ulong)bound);
        }
    }

    // One action of the shared mix; appends what the meter answered.
    private static long Act(object meter, Rng rng, StringBuilder log)
    {
        var x = rng.Next(100);

        if (x < 80)
        {
            var amount = x < 60 ? 1UL : x < 75 ? (ulong)(2 + rng.Next(7)) : (ulong)(100 + rng.Next(200));
            var ok = chargeFuel(meter, amount);
            log.Append(ok ? '+' : '-').Append(amount).Append(';');
            return ok ? (long)amount : 0;
        }

        if (x < 87)
        {
            log.Append('P').Append(poll(meter) ? 1 : 0).Append(';');
        }
        else if (x < 94)
        {
            log.Append('D').Append(chargeDepth(meter, 1) ? 1 : 0).Append(';');
            releaseDepth(meter, 1);
        }
        else
        {
            retain(meter, 16);
            release(meter, 16);
            log.Append('R');
        }

        return 0;
    }

    private static string Hash(StringBuilder log) => Convert.ToHexString(SHA256.HashData(Encoding.ASCII.GetBytes(log.ToString())))[..16];

    private static string Sequential(string tight, int seed)
    {
        const ulong Big = 10_000_000, Small = 150_000;
        var gate = new object();
        var r = Level("Runtime", tight is "runtime" or "tie" ? Small : Big);
        var i = Level("Instance", tight is "instance" or "tie" ? Small : Big);
        var v = Level("Invocation", tight == "invocation" ? Small : Big);
        var m = Meter(gate, v, i, r, 20_000);
        var rng = new Rng(seed);
        var log = new StringBuilder();
        long admitted = 0;

        for (var step = 0; step < 40_000; step++)
        {
            if (step == 5000)
            {
                var burst = 0;

                for (var k = 0; k < 20_001; k++)
                {
                    burst += chargeFuel(m, 1) ? 1 : 0;
                }

                admitted += burst;
                log.Append("B").Append(burst).Append(';').Append("P").Append(poll(m) ? 1 : 0).Append(';');
            }

            admitted += Act(m, rng, log);

            if (step % 997 == 0)
            {
                log.Append('S').Append(LevelFuel(gate, r, r)).Append(',').Append(LevelFuel(gate, r, i)).Append(',').Append(InvocationFuel(m)).Append(';');
            }
        }

        log.Append("U").Append(unpolledExceeds.GetValue(m));
        return $"S1 tight={tight} seed={seed} hash={Hash(log)} admitted={admitted} {State(m)} consumed r/i/v={LevelFuel(gate, r, r)}/{LevelFuel(gate, r, i)}/{InvocationFuel(m)}";
    }

    // Two meters of one runtime, interleaved on one thread, with no settle between them other than their own.
    private static string TwoMeters(int seed)
    {
        var gate = new object();
        var r = Level("Runtime", 150_000);
        var ia = Level("Instance", 10_000_000);
        var ib = Level("Instance", 10_000_000);
        var a = Meter(gate, Level("Invocation", 10_000_000), ia, r, 1_000_000);
        var b = Meter(gate, Level("Invocation", 10_000_000), ib, r, 1_000_000);
        var rng = new Rng(seed);
        var log = new StringBuilder();

        for (var step = 0; step < 40_000; step++)
        {
            var which = rng.Next(100) < 50 ? a : b;
            log.Append(which == a ? 'a' : 'b');

            // Runs, so a meter keeps a credit across several charges before the other takes it back.
            var run = 1 + rng.Next(40);

            for (var k = 0; k < run; k++)
            {
                Act(which, rng, log);
            }
        }

        return $"S2 seed={seed} hash={Hash(log)} a={State(a)} b={State(b)} consumed r={LevelFuel(gate, r, r)} ia={LevelFuel(gate, r, ia)} ib={LevelFuel(gate, r, ib)} va={InvocationFuel(a)} vb={InvocationFuel(b)}";
    }

    // An invocation meter ends holding a credit; the next invocation's meter on the same instance takes over.
    private static string Handoff(int seed)
    {
        var gate = new object();
        var r = Level("Runtime", 150_000);
        var i = Level("Instance", 120_000);
        var rng = new Rng(seed);
        var log = new StringBuilder();
        var meters = new List<object>();

        for (var invocation = 0; invocation < 12; invocation++)
        {
            var m = Meter(gate, Level("Invocation", 30_000), i, r, 1_000_000);
            meters.Add(m);

            for (var step = 0; step < 2500; step++)
            {
                Act(m, rng, log);
            }

            log.Append('|');
        }

        var states = string.Join(" ", meters.Select(State));
        var invocations = string.Join(",", meters.Select(InvocationFuel));
        return $"S3 seed={seed} hash={Hash(log)} consumed r={LevelFuel(gate, r, r)} i={LevelFuel(gate, r, i)} v={invocations} states={Hash(new StringBuilder(states))}";
    }

    private static int Stress(int round, int maxAmount)
    {
        const int Threads = 4;
        const ulong Ceiling = 2_000_000;
        var gate = new object();
        var r = Level("Runtime", Ceiling);
        var meters = Enumerable.Range(0, Threads)
            .Select(_ => Meter(gate, Level("Invocation", ulong.MaxValue), Level("Instance", ulong.MaxValue), r, 1_000_000))
            .ToArray();
        var admitted = new long[Threads];
        var done = 0;
        var observerFailures = 0;
        ulong observed = 0;

        var observer = new Thread(() =>
        {
            ulong last = 0;

            while (Volatile.Read(ref done) == 0)
            {
                var now = LevelFuel(gate, r, r);

                if (now < last || now > Ceiling)
                {
                    Interlocked.Increment(ref observerFailures);
                }

                last = now;
                observed++;
            }
        });

        var workers = Enumerable.Range(0, Threads).Select(t => new Thread(() =>
        {
            var rng = new Rng(round * 31 + t + 7);
            long mine = 0;
            var calls = 0;

            while (true)
            {
                var amount = (ulong)(1 + rng.Next(maxAmount));

                if (!chargeFuel(meters[t], amount))
                {
                    break;
                }

                mine += (long)amount;

                if (++calls % 1024 == 0)
                {
                    poll(meters[t]);
                }
            }

            admitted[t] = mine;
        })).ToArray();

        observer.Start();

        foreach (var worker in workers)
        {
            worker.Start();
        }

        foreach (var worker in workers)
        {
            worker.Join();
        }

        Volatile.Write(ref done, 1);
        observer.Join();

        var total = (ulong)admitted.Sum();
        var runtimeConsumed = LevelFuel(gate, r, r);
        var perMeterOk = Enumerable.Range(0, Threads).All(t => InvocationFuel(meters[t]) == (ulong)admitted[t]);
        var scopesOk = meters.All(m => State(m).StartsWith("Fuel/Runtime exh=True", StringComparison.Ordinal));
        var exactOk = maxAmount == 1 ? total == Ceiling : total <= Ceiling && Ceiling - total < (ulong)maxAmount;
        var ok = runtimeConsumed == total && perMeterOk && scopesOk && exactOk && observerFailures == 0;
        Console.WriteLine($"STRESS round={round} maxAmount={maxAmount} {(ok ? "PASS" : "FAIL")} admitted={total} runtime={runtimeConsumed} perMeter={perMeterOk} scopes={scopesOk} exact={exactOk} observerReads={observed} observerFailures={observerFailures}");
        return ok ? 0 : 1;
    }

    private static Func<object, long, long> Loop()
    {
        var m = Expression.Parameter(typeof(object));
        var n = Expression.Parameter(typeof(long));
        var typed = Expression.Variable(iMeterT);
        var i = Expression.Variable(typeof(long));
        var ok = Expression.Variable(typeof(long));
        var brk = Expression.Label(typeof(long));
        var tryCharge = iMeterT.GetMethod("TryCharge");
        var pollMethod = iMeterT.GetMethod("Poll");

        // for (i = 0; i < n; i++) { if (meter.TryCharge(Fuel, 1)) ok++; if ((i & 16383) == 16383) meter.Poll(); }
        var body = Expression.Block(
            new[] { typed, i, ok },
            Expression.Assign(typed, Expression.Convert(m, iMeterT)),
            Expression.Assign(i, Expression.Constant(0L)),
            Expression.Assign(ok, Expression.Constant(0L)),
            Expression.Loop(
                Expression.IfThenElse(
                    Expression.LessThan(i, n),
                    Expression.Block(
                        Expression.IfThen(
                            Expression.Call(typed, tryCharge, Expression.Constant(fuel, dimT), Expression.Constant(1UL)),
                            Expression.PreIncrementAssign(ok)),
                        Expression.IfThen(
                            Expression.Equal(Expression.And(i, Expression.Constant(16383L)), Expression.Constant(16383L)),
                            Expression.Call(typed, pollMethod)),
                        Expression.PreIncrementAssign(i)),
                    Expression.Break(brk, ok)),
                brk));
        return Expression.Lambda<Func<object, long, long>>(body, m, n).Compile();
    }

    private static void Bench()
    {
        const long N = 100_000_000;
        var loop = Loop();
        var gate = new object();
        var r = Level("Runtime", ulong.MaxValue / 2);
        var direct = Meter(gate, Level("Invocation", ulong.MaxValue / 2), Level("Instance", ulong.MaxValue / 2), r, 0);
        var scope = Activator.CreateInstance(execScopeT, nonPublic: true);
        var ambientMeter = Meter(gate, Level("Invocation", ulong.MaxValue / 2), Level("Instance", ulong.MaxValue / 2), r, 0);
        var ambient = ambientT.GetConstructors(NP).Single().Invoke(new[] { scope });
        enter.Invoke(scope, new object[] { ambientMeter, null });

        foreach (var (name, target) in new[] { ("direct", direct), ("ambient", ambient) })
        {
            loop(target, 20_000_000);
            var best = double.MaxValue;

            for (var rep = 0; rep < 3; rep++)
            {
                var sw = Stopwatch.StartNew();
                var admittedCount = loop(target, N);
                sw.Stop();

                if (admittedCount != N)
                {
                    throw new InvalidOperationException($"{name}: admitted {admittedCount} of {N}");
                }

                best = Math.Min(best, sw.Elapsed.TotalMilliseconds * 1e6 / N);
            }

            Console.WriteLine($"BENCH {name} ns_per_charge_min_of_3={best:F2}");
        }
    }

    // for (i = 0; i < n; i++) { if (!meter.TryCharge(Fuel, 1)) break; ok++; if ((i & 16383) == 16383) meter.Poll(); }
    // The poll every 16,384 charges is the profile's position, and the break is what a finite ceiling does to a
    // thread: the cell runs until every thread has been refused once.
    private static Func<object, long, long> BreakingLoop()
    {
        var m = Expression.Parameter(typeof(object));
        var n = Expression.Parameter(typeof(long));
        var typed = Expression.Variable(iMeterT);
        var i = Expression.Variable(typeof(long));
        var ok = Expression.Variable(typeof(long));
        var brk = Expression.Label(typeof(long));
        var tryCharge = iMeterT.GetMethod("TryCharge");
        var pollMethod = iMeterT.GetMethod("Poll");

        var body = Expression.Block(
            new[] { typed, i, ok },
            Expression.Assign(typed, Expression.Convert(m, iMeterT)),
            Expression.Assign(i, Expression.Constant(0L)),
            Expression.Assign(ok, Expression.Constant(0L)),
            Expression.Loop(
                Expression.IfThenElse(
                    Expression.LessThan(i, n),
                    Expression.Block(
                        Expression.IfThen(
                            Expression.Not(Expression.Call(typed, tryCharge, Expression.Constant(fuel, dimT), Expression.Constant(1UL))),
                            Expression.Break(brk, ok)),
                        Expression.PreIncrementAssign(ok),
                        Expression.IfThen(
                            Expression.Equal(Expression.And(i, Expression.Constant(16383L)), Expression.Constant(16383L)),
                            Expression.Call(typed, pollMethod)),
                        Expression.PreIncrementAssign(i)),
                    Expression.Break(brk, ok)),
                brk));
        return Expression.Lambda<Func<object, long, long>>(body, m, n).Compile();
    }

    // The two concurrency cells. Threads 5 and 8 exceed the table's capacity of 4 on purpose, and the two
    // finite ceilings are at and just above the block cap, where sizing a block as the whole remainder would
    // make two holders settle each other on nearly every charge.
    private static void Concurrent(string mode)
    {
        var loop = BreakingLoop();

        // name, ceiling, rounds per repetition, charges per thread per round.
        var ceilings = new (string Name, ulong Value, int Rounds, long PerThread)[]
        {
            ("unbounded", ulong.MaxValue, 3, 4_000_000L),
            ("2^20", 1UL << 20, 12, long.MaxValue / 4),
            ("4x2^20", 4UL << 20, 4, long.MaxValue / 4),
        };

        foreach (var threads in new[] { 1, 2, 4, 5, 8 })
        {
            foreach (var ceiling in ceilings)
            {
                Console.WriteLine(Cell(mode, loop, threads, ceiling.Name, ceiling.Value, ceiling.Rounds, ceiling.PerThread));
            }
        }
    }

    private sealed class Cell_State
    {
        internal object Gate;
        internal object Runtime;
        internal object[] Targets;   // what each thread charges through
        internal object[] Meters;    // each thread's own meter
        internal object Scope;       // the one execution scope, in the ambient mode
        internal MethodInfo Enter;
        internal MethodInfo Leave;
        internal bool Ambient;
    }

    private static string Cell(string mode, Func<object, long, long> loop, int threads, string ceilingName, ulong ceiling, int rounds, long perThread)
    {
        var ambient = mode == "concurrent-ambient";
        var admitted = new long[threads];
        Cell_State state = null;
        var phase = 0;
        var go = -1;
        var ready = 0;
        var done = 0;
        var faults = 0;

        var workers = Enumerable.Range(0, threads).Select(t => new Thread(() =>
        {
            var mine = 0;

            while (true)
            {
                mine++;

                while (Volatile.Read(ref phase) < mine)
                {
                    Thread.SpinWait(64);
                }

                if (Volatile.Read(ref phase) == int.MaxValue)
                {
                    return;
                }

                var local = state;

                try
                {
                    if (local.Ambient)
                    {
                        // Each thread enters the one scope with its own meter, so the scope's resolution sees
                        // as many execution contexts at once as there are threads.
                        local.Enter.Invoke(local.Scope, new object[] { local.Meters[t], null });
                    }
                }
                catch (Exception)
                {
                    Interlocked.Increment(ref faults);
                }

                Interlocked.Decrement(ref ready);

                while (Volatile.Read(ref go) < mine)
                {
                    Thread.SpinWait(64);
                }

                admitted[t] = loop(local.Targets[t], perThread);
                Interlocked.Decrement(ref done);

                if (local.Ambient)
                {
                    // Outside the timed region. Without it every round leaves another live AsyncLocal in this
                    // thread's context, and the lookup the mode measures would get slower round by round for a
                    // reason that has nothing to do with either build.
                    local.Leave.Invoke(local.Scope, null);
                }
            }
        })
        { IsBackground = true }).ToArray();

        foreach (var worker in workers)
        {
            worker.Start();
        }

        Cell_State Build()
        {
            var gate = new object();
            var runtime = Level("Runtime", ceiling);
            var meters = new object[threads];
            var targets = new object[threads];
            object scope = null;

            for (var t = 0; t < threads; t++)
            {
                meters[t] = Meter(gate, Level("Invocation", ulong.MaxValue), Level("Instance", ulong.MaxValue), runtime, 65_536);
            }

            if (ambient)
            {
                scope = Activator.CreateInstance(execScopeT, nonPublic: true);
                var one = ambientT.GetConstructors(NP).Single().Invoke(new[] { scope });

                for (var t = 0; t < threads; t++)
                {
                    targets[t] = one;   // one ambient meter, shared by every thread
                }
            }
            else
            {
                for (var t = 0; t < threads; t++)
                {
                    targets[t] = meters[t];
                }
            }

            return new Cell_State { Gate = gate, Runtime = runtime, Targets = targets, Meters = meters, Scope = scope, Enter = enter, Leave = leave, Ambient = ambient };
        }

        double Round()
        {
            state = Build();
            Array.Clear(admitted);
            Volatile.Write(ref ready, threads);
            Volatile.Write(ref done, threads);
            var p = phase + 1;
            Volatile.Write(ref phase, p);

            while (Volatile.Read(ref ready) > 0)
            {
                Thread.SpinWait(64);
            }

            var sw = Stopwatch.StartNew();
            Volatile.Write(ref go, p);

            while (Volatile.Read(ref done) > 0)
            {
                Thread.SpinWait(64);
            }

            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        // A warm-up repetition, not counted: the first round pays for the JIT of the loop and of every
        // reflection-bound path it reaches.
        Round();

        var best = double.MaxValue;
        long bestAdmitted = 0;
        var exact = true;
        var consumedMatches = true;

        for (var rep = 0; rep < 3; rep++)
        {
            double elapsed = 0;
            long total = 0;

            for (var round = 0; round < rounds; round++)
            {
                elapsed += Round();
                var roundAdmitted = admitted.Sum();
                total += roundAdmitted;

                if (ceiling != ulong.MaxValue)
                {
                    exact &= (ulong)roundAdmitted == ceiling;
                }
                else
                {
                    exact &= roundAdmitted == threads * perThread;
                }

                consumedMatches &= LevelFuel(state.Gate, state.Runtime, state.Runtime) == (ulong)roundAdmitted;
            }

            var ns = elapsed * 1e6 / total;

            if (ns < best)
            {
                best = ns;
                bestAdmitted = total;
            }
        }

        Volatile.Write(ref phase, int.MaxValue);

        foreach (var worker in workers)
        {
            worker.Join(5000);
        }

        return $"CONC mode={mode} threads={threads} ceiling={ceilingName} ns_per_admitted_charge_min_of_3={best:F2} " +
            $"admitted_per_rep={bestAdmitted} rounds={rounds} exact={exact} runtime_consumed_matches={consumedMatches} faults={faults}";
    }
}
