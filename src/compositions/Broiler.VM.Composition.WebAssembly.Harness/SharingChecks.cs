using Broiler.VM;
using Broiler.VM.Profile.WebAssembly;
using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// Two runtimes executing one verified handle at once, with nothing between them: the concurrent half
/// of WA-5's handle invariant.
/// </summary>
/// <remarks>
/// <para>
/// <b>What the gate asks for.</b> "Two runtimes read one shareable handle concurrently with no
/// synchronisation", beside the structural scan that asserts nothing mutable is reachable from a
/// handle. The scan is rule W5, and it reads source. This lane runs the thing the scan is about: the
/// family declares its artifacts shareable, so a second runtime composed like the first admits the
/// first runtime's handle, and each runtime's instance of it is driven from a thread of its own.
/// </para>
/// <para>
/// <b>What sharing something would look like.</b> The module keeps a counter in a mutable global and
/// another in its memory, and each call of <c>step</c> advances both and answers the global. An
/// instance that shares nothing answers one, two, three and so on, whatever the other thread does,
/// and holds exactly as many in its memory as it was called. If the two instances shared a global, a
/// memory or any cache the handle held, the answers of one would skip the values the other took. A
/// data segment writes a seed the module never changes, and an instance made after both runs must
/// still read it and start its counters from zero: the handle itself was not written through.
/// </para>
/// <para>
/// <b>Concurrent, and shown to be.</b> The two threads start together and each records whether it
/// ever saw the other running, so the check fails if the runs happened not to overlap and proved
/// nothing. Nothing else passes between them: each thread has its own runtime and its own instance,
/// and the one object both read is the handle.
/// </para>
/// </remarks>
internal static class SharingChecks
{
    private const string Caller = "composition-wasm-harness://sharing";

    /// <summary>How many times each thread steps its instance.</summary>
    private const int Steps = 20_000;

    /// <summary>The value the data segment writes, which no function changes.</summary>
    private const int Seed = 7;

    internal static int Report(bool verbose)
    {
        var checks = Run();
        var failed = 0;

        Console.WriteLine($"# sharing: {checks.Count} checks");

        foreach (var (name, passed, detail) in checks)
        {
            if (!passed)
            {
                failed++;
            }

            if (verbose || !passed)
            {
                Console.WriteLine($"{(passed ? "ok  " : "FAIL")} {name}: {detail}");
            }
        }

        Console.WriteLine($"# sharing: {checks.Count - failed} of {checks.Count} checks passed");
        return failed;
    }

    private static List<(string Name, bool Passed, string Detail)> Run()
    {
        var checks = new List<(string, bool, string)>();
        using var first = Program.Runtime(out var failure);
        using var second = Program.Runtime(out var secondFailure);

        if (first is null || second is null)
        {
            checks.Add(("sharing: runtimes", false, $"{failure}{secondFailure}"));
            return checks;
        }

        var verified = ModuleVerification.Verify(first, Module(), Caller, "sharing");

        if (!verified.TryGetArtifact(out var handle))
        {
            checks.Add(("sharing: verification", false, $"{verified.Outcome}/{verified.Reason}"));
            return checks;
        }

        using (handle)
        {
            // The second runtime admits the first runtime's handle: the family declares its artifacts
            // shareable, and the two runtimes are composed alike.
            var admitted = second.Instantiate(handle, CancellationToken.None);

            if (!admitted.TryGetInstance(out var borrowed))
            {
                checks.Add(("sharing-a-second-runtime-instantiates-the-first-runtimes-handle", false,
                    $"{admitted.Outcome}/{admitted.Reason}"));
                return checks;
            }

            checks.Add(("sharing-a-second-runtime-instantiates-the-first-runtimes-handle", true,
                $"{admitted.Outcome}/{admitted.Reason}"));

            var own = first.Instantiate(handle, CancellationToken.None);

            if (!own.TryGetInstance(out var owned))
            {
                borrowed.Dispose();
                checks.Add(("sharing: the first runtime's instance", false, $"{own.Outcome}/{own.Reason}"));
                return checks;
            }

            using (owned)
            using (borrowed)
            {
                checks.Add(Concurrently(owned, borrowed));
            }

            // After both runs, a new instance of the handle starts where the module says it starts.
            var fresh = first.Instantiate(handle, CancellationToken.None);

            if (!fresh.TryGetInstance(out var after))
            {
                checks.Add(("sharing-an-instance-made-after-both-runs-starts-from-the-modules-state", false,
                    $"{fresh.Outcome}/{fresh.Reason}"));
                return checks;
            }

            using (after)
            {
                var seed = Answer(after, "4:seed");
                var cell = Answer(after, "4:cell");
                var step = Answer(after, "4:step");

                checks.Add(("sharing-an-instance-made-after-both-runs-starts-from-the-modules-state",
                    seed == Seed && cell == 0 && step == 1,
                    $"seed {Text(seed)}, cell {Text(cell)}, first step {Text(step)}; expected {Text(Seed)}, 0 and 1"));
            }
        }

        return checks;
    }

    /// <summary>Each instance stepped from a thread of its own, both at once.</summary>
    private static (string, bool, string) Concurrently(VmInstance first, VmInstance second)
    {
        const string Name = "sharing-two-runtimes-step-one-handle-at-once-and-share-nothing";
        var running = 0;
        using var start = new Barrier(2);

        Func<Outcome> Drive(VmInstance instance) => () =>
        {
            start.SignalAndWait();
            Interlocked.Increment(ref running);
            var overlapped = false;
            var wrong = 0;
            var firstWrong = string.Empty;

            for (var expected = 1; expected <= Steps; expected++)
            {
                overlapped |= Volatile.Read(ref running) == 2;
                var answered = Answer(instance, "4:step");

                if (answered != expected)
                {
                    if (wrong++ == 0)
                    {
                        firstWrong = $"step {Text(expected)} answered {Text(answered)}";
                    }
                }
            }

            Interlocked.Decrement(ref running);
            return new(wrong, firstWrong, Answer(instance, "4:cell"), overlapped);
        };

        var one = Task.Run(Drive(first));
        var two = Task.Run(Drive(second));
        Task.WaitAll(one, two);

        var results = new[] { one.Result, two.Result };
        var passed = results.All(static result => result.Wrong == 0 && result.Cell == Steps) &&
            results.Any(static result => result.Overlapped);

        return (Name, passed,
            string.Join("; ", results.Select((result, index) =>
                $"runtime {Text(index + 1)}: {Text(Steps - result.Wrong)} of {Text(Steps)} steps in order" +
                (result.Wrong == 0 ? string.Empty : $" ({result.FirstWrong})") +
                $", cell {Text(result.Cell)}")) +
            (results.Any(static result => result.Overlapped) ? "; the runs overlapped" : "; the runs did not overlap"));
    }

    private readonly record struct Outcome(int Wrong, string FirstWrong, long Cell, bool Overlapped);

    /// <summary>
    /// One memory with a seed at byte four, one mutable global, and three functions: <c>step</c>
    /// advances the global and the counter at byte zero and answers the global, <c>cell</c> answers the
    /// counter, and <c>seed</c> the seed.
    /// </summary>
    private static byte[] Module()
    {
        var assembler = new WasmAssembler();
        assembler.Memory(1, null);
        assembler.Data(4, [Seed, 0x00, 0x00, 0x00]);
        var counter = assembler.Global(WasmAssembler.I32, mutable: true, Instruction.I32Const(0));
        var nullary = assembler.Type([], [WasmAssembler.I32]);

        assembler.Export("step", WasmAssembler.ExportFunction, assembler.Function(nullary, [], Instruction.Cat(
            Instruction.GlobalGet(counter), Instruction.I32Const(1), [Instruction.I32Add], Instruction.GlobalSet(counter),
            Instruction.I32Const(0),
            Instruction.I32Const(0), Instruction.Load(0x28, 2, 0), Instruction.I32Const(1), [Instruction.I32Add],
            Instruction.Load(0x36, 2, 0),
            Instruction.GlobalGet(counter))));

        assembler.Export("cell", WasmAssembler.ExportFunction, assembler.Function(nullary, [], Instruction.Cat(
            Instruction.I32Const(0), Instruction.Load(0x28, 2, 0))));

        assembler.Export("seed", WasmAssembler.ExportFunction, assembler.Function(nullary, [], Instruction.Cat(
            Instruction.I32Const(4), Instruction.Load(0x28, 2, 0))));

        return assembler.Build();
    }

    /// <summary>The one i32 an entry answers, or minus one for any other answer.</summary>
    private static long Answer(VmInstance instance, string entry)
    {
        var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(entry)));
        var answered = instance.Invoke(in request, CancellationToken.None);

        return answered.Outcome is VmOutcome.Normal &&
            WebAssemblyProfile.TryGetResults(in answered, out var results) &&
            results.Count == 1 && results.TryGetValue(0, out var value) && value.Kind is WebAssemblyValueKind.I32
                ? value.AsInt32
                : -1;
    }

    private static string Text(long value) => value.ToString(CultureInfo.InvariantCulture);
}
