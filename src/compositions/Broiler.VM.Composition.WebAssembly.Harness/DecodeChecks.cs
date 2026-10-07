using Broiler.VM.Abstractions;
using Broiler.VM.Profile.WebAssembly;
using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// What a translation reads each byte of a module for, and that nothing it does throws across the
/// retained corpus: two clauses of WA-3's exit gate, "each module is decoded at most once during
/// verification, asserted by a case" and "the validator throws on nothing across the whole corpus".
/// </summary>
/// <remarks>
/// <para>
/// <b>Read from outside, through the allowance.</b> Under the universal bytecode the core verifies an
/// artifact and never reads the module; the translator is where the module is decoded. Every byte
/// the decoder or the validator steps over is charged to the verification's work allowance, one unit
/// a byte, as the core's bounded reader charges it. What a translation charges is read the way a
/// host meets it: the smallest work allowance it completes under, found by bisection. Two modules
/// differing only in the length of one part are measured, and the difference is what that part's
/// extra bytes cost.
/// </para>
/// <para>
/// <b>A section's bytes are decoded once.</b> A custom section's payload is stepped over by the
/// decoder and read by nothing else, so its extra bytes cost one unit each: a module decoded twice
/// would pay two.
/// </para>
/// <para>
/// <b>A function body's instructions are read three times, and two of the reads are charged.</b> The
/// decoder copies a body's instructions into a buffer of their own, charged a unit a byte, and does
/// not decode them. The validator decodes them as it validates them, charged a unit a byte. And the
/// lowering walks them again to write the artifact's rows, reading every immediate again, with a
/// meter that admits every charge because the validator paid for the bytes. So the extra bytes of a
/// body cost two units each, and this check holds them to at least two and fewer than three: a pass
/// that stopped reading them, or a third charged pass, fails it. The lowering's walk is not observable
/// here, since it charges nothing, and it is the reason this check does not claim that a body is
/// decoded once. Whether a lowering's walk counts as a decode under the gate's clause is the gate
/// owner's to rule.
/// </para>
/// <para>
/// <b>Throws on nothing.</b> The translator answers any exception that escapes it as the reserved
/// defect code, so an exception is observable as that code. This check translates every module the
/// retained corpus holds, of either provenance, and asserts that none answers it.
/// </para>
/// </remarks>
internal static class DecodeChecks
{
    private const string Caller = "composition-wasm-harness://decoding";

    /// <summary>How many bytes longer the second module of each pair is.</summary>
    private const int Extra = 100_000;

    internal static int Report(VmRuntime runtime, string? corpus, bool verbose)
    {
        var checks = new List<(string Name, bool Passed, string Detail)>
        {
            Charged(runtime, "decode-a-custom-sections-payload-is-charged-once-a-byte", CustomSection, 1),
            Charged(runtime, "decode-a-function-bodys-instructions-are-charged-twice-a-byte-the-copy-and-the-validation", Body, 2),
        };

        if (corpus is not null)
        {
            checks.Add(NoDefect(runtime, corpus));
        }

        var failed = 0;
        Console.WriteLine($"# decoding: {checks.Count} checks");

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

        Console.WriteLine($"# decoding: {checks.Count - failed} of {checks.Count} checks passed");
        return failed;
    }

    /// <summary>A module whose last section is a custom section named "x" carrying <paramref name="payload"/> zero bytes.</summary>
    private static byte[] CustomSection(int payload) =>
        [.. CorpusStore.FunctionModule([], [], []), .. CorpusStore.Section(0, [0x01, (byte)'x', .. new byte[payload]])];

    /// <summary>A module of one function whose body is <paramref name="nops"/> nops.</summary>
    private static byte[] Body(int nops) => CorpusStore.FunctionModule([], [], [.. Enumerable.Repeat((byte)0x01, nops)]);

    /// <summary>
    /// Whether the extra bytes of the longer module cost at least <paramref name="times"/> units each
    /// and fewer than one more.
    /// </summary>
    private static (string, bool, string) Charged(VmRuntime runtime, string name, Func<int, byte[]> module, ulong times)
    {
        var shorter = module(0);
        var longer = module(Extra);
        var descriptor = ModuleVerification.ArtifactDescriptor(WebAssemblyProfile.SliceManifest, Caller);
        var ceilings = ModuleVerification.Ceilings(runtime, in descriptor);

        if (Work(shorter, ceilings) is not { } fewer || Work(longer, ceilings) is not { } more)
        {
            return (name, false, "a module did not translate under the runtime's own work ceiling");
        }

        var attributed = more - fewer;
        var extra = (ulong)(longer.Length - shorter.Length);

        // The bounds are on the part's own extra bytes. The modules also differ by the few bytes by
        // which the longer one's lengths are encoded longer, each read once, which the upper bound's
        // whole extra unit a byte leaves room for.
        var passed = attributed >= times * Extra && attributed < (times + 1) * Extra;

        return (name, passed,
            $"{Text(extra)} more bytes, {Text(Extra)} of them the part's own, cost {Text(attributed)} more units of verifier work ({Text(fewer)} against {Text(more)})" +
            (passed ? string.Empty : $", expected at least {Text(times * Extra)} and fewer than {Text((times + 1) * Extra)}"));
    }

    /// <summary>The smallest verifier-work allowance <paramref name="module"/> translates under, or nothing when the ceiling given is too small.</summary>
    private static ulong? Work(byte[] module, VmLimitVector ceilings)
    {
        var high = ceilings[VmBudgetDimension.VerifierWork];

        if (!Translates(module, ceilings, high))
        {
            return null;
        }

        ulong low = 0;

        // The invariant: low does not translate and high does, so high ends as the smallest that does.
        while (high - low > 1)
        {
            var middle = low + ((high - low) / 2);

            if (Translates(module, ceilings, middle))
            {
                high = middle;
            }
            else
            {
                low = middle;
            }
        }

        return high;
    }

    private static bool Translates(byte[] module, VmLimitVector ceilings, ulong work)
    {
        var values = new ulong[VmBudgetDimensions.Count];

        foreach (var dimension in VmBudgetDimensions.All)
        {
            values[(int)dimension] = dimension is VmBudgetDimension.VerifierWork ? work : ceilings[dimension];
        }

        return VmLimitVector.TryCreate(values, out var vector) &&
            WasmTranslator.Translate(module, vector, CancellationToken.None).Succeeded;
    }

    /// <summary>No module of the retained corpus is answered with the defect code an escaping exception is answered with.</summary>
    private static (string, bool, string) NoDefect(VmRuntime runtime, string corpus)
    {
        const string Name = "decode-no-retained-module-is-answered-with-the-defect-code-an-exception-becomes";
        var descriptor = ModuleVerification.ArtifactDescriptor(WebAssemblyProfile.SliceManifest, Caller);
        var ceilings = ModuleVerification.Ceilings(runtime, in descriptor);
        var names = File.ReadAllLines(Path.Combine(corpus, CorpusStore.ManifestFileName))
            .Where(static line => line.Length > 0 && line[0] != '#')
            .Select(static line => line.Split('|')[0])
            .ToList();

        var defects = names
            .Where(entry => WasmTranslator.Translate(
                File.ReadAllBytes(Path.Combine(corpus, entry + CorpusStore.Extension)), ceilings, CancellationToken.None)
                .Code is WebAssemblyDiagnosticCode.VerifierDefect)
            .ToList();

        return (Name, names.Count > 0 && defects.Count == 0,
            defects.Count == 0
                ? $"{Text((ulong)names.Count)} modules translated, none answered {Text((ulong)WebAssemblyDiagnosticCode.VerifierDefect)}"
                : $"{string.Join(", ", defects)} answered {Text((ulong)WebAssemblyDiagnosticCode.VerifierDefect)}");
    }

    private static string Text(ulong value) => value.ToString(CultureInfo.InvariantCulture);
}
