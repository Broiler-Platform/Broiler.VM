using Broiler.VM;
using Broiler.VM.Profile.WebAssembly;
using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// What <c>memory.grow</c> is charged, measured against its declared function: the one proportional
/// family WA-5 ships.
/// </summary>
/// <remarks>
/// <para>
/// <b>What the gate asks for.</b> Roadmap section 9 names the families this profile will ever charge
/// proportionally and holds each to a declared monotone non-decreasing charging function, a
/// granularity, and a fixture with an unsimplified control. WA-5's exit gate, as correction WAC-21
/// scopes it, asks for a fixture for each family the milestone ships, which is <c>memory.grow</c> alone,
/// and names a growth over a large delta as the negative control a flat charge fails. Until this file
/// no fixture existed.
/// </para>
/// <para>
/// <b>The declared function is decision WAD-0001's.</b> A growth by <i>n</i> pages charges <i>n</i> units
/// of fuel, one a page, and the <i>n</i> pages' bytes as allocated and as retained, all before it
/// allocates. The granularity is one page, so the floor is the function itself. The floors below are
/// that declaration written down, not what was measured: a floor set to the measurement would pass by
/// construction.
/// </para>
/// <para>
/// <b>The charge is read off the budget, against a control that is the same call.</b> Each magnitude
/// runs in a runtime of its own, because the allocations of all of them together pass what one
/// runtime's default allocation ceiling admits. Each instance holds one page, and the same exported
/// function is called with <i>n</i> and, on another instance, with zero. A growth of zero runs the
/// same instructions and dispatches the same rows, and the specification defines it as reading the
/// size, so it charges nothing proportional. The difference between the two calls is what the
/// growth is charged. The magnitudes double, so the shape is a comparison between neighbours: a
/// charge that rises by less than half again when the delta doubles is reported, and a flat charge
/// fails at the first doubling. The largest delta is the gate's negative control.
/// </para>
/// <para>
/// <b>What the declared function does not cover.</b> WAD-0001's growth allocates a new array of the
/// whole new size and copies the old contents into it. The copy grows with the memory's current size,
/// and the declared function charges nothing for it. So a growth by one page is charged the same
/// from a memory of one page as from one of several hundred. The page ceiling bounds it: no single
/// copy exceeds that ceiling's sixty-four mebibytes, and no memory grows more than that ceiling's
/// number of times. This lane prints that measurement beside its checks and does not judge it. Whether
/// the charge should cover the copy is a question for WAD-0001's charge row, which is a taken
/// decision, and not for a fixture.
/// </para>
/// </remarks>
internal static class ProportionalityChecks
{
    private const string Caller = "composition-wasm-harness://proportionality";

    /// <summary>The bytes of one page, which the format fixes.</summary>
    private const ulong Page = 65_536;

    /// <summary>The deltas measured, in pages; the last is the gate's large-delta negative control.</summary>
    private static readonly int[] Deltas = [1, 2, 4, 8, 16, 32, 64, 128, 256, 512];

    /// <summary>The current sizes a growth of one page is measured from, for the copy the function does not cover.</summary>
    private static readonly uint[] CurrentSizes = [1, 256];

    /// <summary>What one call spent, read off its runtime's budget.</summary>
    private readonly record struct Spent(ulong Fuel, ulong Allocated, ulong Live);

    internal static int Report(bool verbose)
    {
        var checks = new List<(string Name, bool Passed, string Detail)> { Grow() };
        var failed = 0;

        Console.WriteLine($"# proportionality: {checks.Count} checks");

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

        Console.WriteLine($"# proportionality: {checks.Count - failed} of {checks.Count} checks passed");
        Console.WriteLine($"# proportionality, not declared: {Copy()}");
        return failed;
    }

    /// <summary>The fixture: every delta against its control, judged against the floor and the shape.</summary>
    private static (string, bool, string) Grow()
    {
        const string Name = "memory.grow is charged proportionally to its delta";
        var attributed = new Spent[Deltas.Length];

        for (var index = 0; index < Deltas.Length; index++)
        {
            var delta = Deltas[index];

            if (!TryAttribute(1, (uint)delta, out attributed[index], out var failure))
            {
                return (Name, false, $"at n={delta.ToString(CultureInfo.InvariantCulture)} {failure}");
            }
        }

        var complaints = new List<string>();

        for (var index = 0; index < Deltas.Length; index++)
        {
            var delta = (ulong)Deltas[index];
            var at = $"at n={delta.ToString(CultureInfo.InvariantCulture)}";

            if (attributed[index].Fuel < delta)
            {
                complaints.Add($"{at} the fuel {Text(attributed[index].Fuel)} is below the declared floor {Text(delta)}");
            }

            if (attributed[index].Allocated < delta * Page)
            {
                complaints.Add($"{at} the allocated bytes {Text(attributed[index].Allocated)} are below the declared {Text(delta * Page)}");
            }

            if (attributed[index].Live < delta * Page)
            {
                complaints.Add($"{at} the live bytes {Text(attributed[index].Live)} are below the declared {Text(delta * Page)}");
            }

            if (index == 0)
            {
                continue;
            }

            // MONOTONE, AND MORE THAN MONOTONE. A flat charge is monotone; what tells a proportional
            // charge from a flat one is that doubling the delta costs materially more.
            var previous = attributed[index - 1].Fuel;
            var current = attributed[index].Fuel;

            if (current < previous)
            {
                complaints.Add($"the fuel fell from {Text(previous)} to {Text(current)} between n={Text((ulong)Deltas[index - 1])} and n={Text(delta)}");
            }
            else if (current * 2 < previous * 3)
            {
                complaints.Add($"doubling n from {Text((ulong)Deltas[index - 1])} to {Text(delta)} took the fuel only from {Text(previous)} to {Text(current)}");
            }
        }

        var series = string.Join(", ", Enumerable.Range(0, Deltas.Length).Select(index =>
            $"n={Text((ulong)Deltas[index])}:{Text(attributed[index].Fuel)}"));

        return (Name, complaints.Count == 0,
            complaints.Count == 0
                ? $"declared one unit of fuel a page added, granularity one page, and the pages' bytes allocated and retained; fuel over grow(0) at {series}"
                : string.Join("; ", complaints) + $" (fuel over grow(0) at {series})");
    }

    /// <summary>A growth of one page from each current size, against a growth of none from the same size.</summary>
    private static string Copy()
    {
        var measured = new List<string>();

        foreach (var pages in CurrentSizes)
        {
            measured.Add(TryAttribute(pages, 1, out var spent, out var failure)
                ? $"grow(1) from {Text(pages)} page{(pages == 1 ? string.Empty : "s")} is charged {Text(spent.Fuel)} fuel"
                : $"grow(1) from {Text(pages)} pages was not measured: {failure}");
        }

        return string.Join(", ", measured) + "; the copy of the current contents is charged by no declared function";
    }

    /// <summary>
    /// What a growth by <paramref name="delta"/> from a memory of <paramref name="pages"/> spends over a
    /// growth by none, each on a fresh instance in a runtime of their own.
    /// </summary>
    private static bool TryAttribute(uint pages, uint delta, out Spent attributed, out string failure)
    {
        attributed = default;
        using var runtime = Program.Runtime(out failure);

        if (runtime is null)
        {
            return false;
        }

        var assembler = new WasmAssembler();
        assembler.Memory(pages, null);
        var unary = assembler.Type([WasmAssembler.I32], [WasmAssembler.I32]);
        assembler.Export("grow", WasmAssembler.ExportFunction, assembler.Function(
            unary, [], Instruction.Cat(Instruction.LocalGet(0), [Instruction.MemoryGrow], [0x00])));

        var verified = ModuleVerification.Verify(
            runtime, assembler.Build(), Caller, $"grow-{pages.ToString(CultureInfo.InvariantCulture)}-by-{delta.ToString(CultureInfo.InvariantCulture)}");

        if (!verified.TryGetArtifact(out var artifact))
        {
            failure = $"verification {verified.Outcome}/{verified.Reason}";
            return false;
        }

        using (artifact)
        {
            if (!TryCall(runtime, artifact, 0, pages, out var control, out failure) ||
                !TryCall(runtime, artifact, delta, pages, out var candidate, out failure))
            {
                return false;
            }

            if (candidate.Fuel < control.Fuel || candidate.Allocated < control.Allocated || candidate.Live < control.Live)
            {
                failure = $"the growth spent less than its control: {candidate} against {control}";
                return false;
            }

            attributed = new(candidate.Fuel - control.Fuel, candidate.Allocated - control.Allocated, candidate.Live - control.Live);
            return true;
        }
    }

    /// <summary>One instance, one call of <c>grow</c>, which must answer the old size; what the call spent.</summary>
    private static bool TryCall(VmRuntime runtime, VmVerifiedArtifact artifact, uint delta, uint pages, out Spent spent, out string failure)
    {
        spent = default;
        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            failure = $"instantiation {instantiated.Outcome}/{instantiated.Reason}";
            return false;
        }

        using (instance)
        {
            var before = Read(runtime);
            var entry = $"4:grow{Argument(delta)}";
            var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(entry)));
            var answered = instance.Invoke(in request, CancellationToken.None);
            var after = Read(runtime);

            if (answered.Outcome is not VmOutcome.Normal ||
                !WebAssemblyProfile.TryGetResults(in answered, out var results) ||
                results.Count != 1 || !results.TryGetValue(0, out var value) ||
                value.Kind is not WebAssemblyValueKind.I32 || value.AsInt32 != (int)pages)
            {
                failure = $"grow({Text(delta)}) answered {answered.Outcome}/{answered.Reason}, not the old size {Text(pages)}";
                return false;
            }

            spent = new(after.Fuel - before.Fuel, after.Allocated - before.Allocated, after.Live - before.Live);
            failure = string.Empty;
            return true;
        }
    }

    private static Spent Read(VmRuntime runtime)
    {
        var snapshot = runtime.GetBudgetSnapshot();

        return new(
            snapshot.Consumed(VmBudgetDimension.Fuel),
            snapshot.Consumed(VmBudgetDimension.AllocatedBytes),
            snapshot.Consumed(VmBudgetDimension.LiveBytes));
    }

    private static string Argument(uint value)
    {
        var literal = value.ToString(CultureInfo.InvariantCulture);
        return $"i32:{literal.Length.ToString(CultureInfo.InvariantCulture)}:{literal}";
    }

    private static string Text(ulong value) => value.ToString(CultureInfo.InvariantCulture);
}
