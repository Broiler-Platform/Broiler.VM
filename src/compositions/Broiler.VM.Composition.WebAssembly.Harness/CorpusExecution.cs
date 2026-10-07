using Broiler.VM.Abstractions;
using Broiler.VM.Profile.WebAssembly;
using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// Every function of every retained module that verifies, instantiated and called: that no exception
/// escapes execution across the corpus, one of WA-5's exit-gate clauses.
/// </summary>
/// <remarks>
/// <para>
/// <b>What an escaping exception looks like from outside.</b> The bytecode emitter runs the family's
/// handlers, and an exception that leaves one is caught by the emitter or by the core and answered as
/// a profile fault with the reason <c>ProfileContractViolation</c>. So "no exception escapes the
/// interpreter" is observable as no answer carrying that reason, at instantiation - which runs the
/// start function and the segments - or at any call.
/// </para>
/// <para>
/// <b>Every function, not every export.</b> Most retained modules that verify export nothing, so a
/// sweep of their exports would call almost nothing. This lane reads each module's type, function and
/// export sections itself, since the profile does not publish a module's function types, and writes the
/// module again with an export section naming every function it defines, in the place the section
/// order gives exports. The new module is translated and verified like any other, and each function
/// is called on a fresh instance with zero for every parameter, so a trap or an exhaustion in one
/// does not fault the instance the next is called on.
/// </para>
/// <para>
/// <b>What passes.</b> Every instantiation and every call answers something other than the contract
/// violation - a completion, a trap, an exhaustion - and at least one call completes, so a sweep that
/// reached nothing fails. The gate names the fuzz corpus too, and there is none yet; it is WA-9's.
/// </para>
/// </remarks>
internal static class CorpusExecution
{
    private const string Caller = "composition-wasm-harness://corpus-execution";

    /// <summary>The section identifiers in the order a module must carry them; custom sections go anywhere.</summary>
    private static readonly byte[] Order = [1, 2, 3, 4, 5, 6, 7, 8, 9, 12, 10, 11];

    private const byte ExportSection = 7;

    internal static int Report(VmRuntime runtime, string? corpus, bool verbose)
    {
        if (corpus is null)
        {
            return 0;
        }

        var check = Sweep(runtime, corpus);
        var failed = check.Passed ? 0 : 1;

        Console.WriteLine("# corpus execution: 1 checks");

        if (verbose || !check.Passed)
        {
            Console.WriteLine($"{(check.Passed ? "ok  " : "FAIL")} {check.Name}: {check.Detail}");
        }

        Console.WriteLine($"# corpus execution: {1 - failed} of 1 checks passed");
        return failed;
    }

    private static (string Name, bool Passed, string Detail) Sweep(VmRuntime runtime, string corpus)
    {
        const string Name = "execution-no-function-of-a-verified-retained-module-lets-an-exception-escape";
        var modules = 0;
        var calls = 0;
        var answers = new SortedDictionary<string, int>(StringComparer.Ordinal);
        var violations = new List<string>();

        foreach (var entry in VerifiedEntries(corpus))
        {
            var bytes = File.ReadAllBytes(Path.Combine(corpus, entry + CorpusStore.Extension));

            if (!TryExportEverything(bytes, out var module, out var signatures, out var failure))
            {
                violations.Add($"{entry}: {failure}");
                continue;
            }

            var verified = ModuleVerification.Verify(runtime, module, Caller, entry);

            if (!verified.TryGetArtifact(out var artifact))
            {
                violations.Add($"{entry}: rewritten, it answered {verified.Outcome}/{verified.Reason}");
                continue;
            }

            modules++;

            using (artifact)
            {
                // Instantiation runs the start function and the segments, so it is swept on its own even
                // for a module that defines no function to call.
                var instantiated = runtime.Instantiate(artifact, CancellationToken.None);
                Count(answers, "instantiation", instantiated.Outcome, instantiated.Reason);

                if (instantiated.Reason is VmReason.ProfileContractViolation)
                {
                    violations.Add($"{entry}: instantiation answered {instantiated.Outcome}/{instantiated.Reason}");
                }

                if (instantiated.TryGetInstance(out var first))
                {
                    first.Dispose();
                }
                else
                {
                    continue;
                }

                for (var function = 0; function < signatures.Count; function++)
                {
                    var fresh = runtime.Instantiate(artifact, CancellationToken.None);

                    if (!fresh.TryGetInstance(out var instance))
                    {
                        continue;
                    }

                    using (instance)
                    {
                        var name = $"f{function.ToString(CultureInfo.InvariantCulture)}";
                        var arguments = string.Concat(signatures[function].Select(Zero));
                        var entryPoint = $"{System.Text.Encoding.UTF8.GetByteCount(name).ToString(CultureInfo.InvariantCulture)}:{name}{arguments}";
                        var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(entryPoint)));
                        var answered = instance.Invoke(in request, CancellationToken.None);
                        calls++;
                        Count(answers, "call", answered.Outcome, answered.Reason);

                        if (answered.Reason is VmReason.ProfileContractViolation)
                        {
                            violations.Add($"{entry} {name}: {answered.Outcome}/{answered.Reason}");
                        }
                    }
                }
            }
        }

        var completed = answers.GetValueOrDefault($"call {VmOutcome.Normal}/{VmReason.NormalCompleted}");
        var passed = violations.Count == 0 && modules > 0 && completed > 0;

        return (Name, passed,
            $"{Text(modules)} modules, {Text(calls)} calls: " +
            string.Join(", ", answers.Select(static pair => $"{pair.Key} {Text(pair.Value)}")) +
            (violations.Count == 0 ? string.Empty : $"; {string.Join("; ", violations)}") +
            (completed > 0 ? string.Empty : "; no call completed"));
    }

    private static void Count(SortedDictionary<string, int> answers, string what, VmOutcome outcome, VmReason reason)
    {
        var key = $"{what} {outcome}/{reason}";
        answers[key] = answers.GetValueOrDefault(key) + 1;
    }

    /// <summary>The retained entries whose recorded answer is a verification.</summary>
    private static IEnumerable<string> VerifiedEntries(string corpus) =>
        File.ReadAllLines(Path.Combine(corpus, CorpusStore.ManifestFileName))
            .Where(static line => line.Length > 0 && line[0] != '#')
            .Select(static line => line.Split('|'))
            .Where(static columns => columns.Length > 3 && columns[3] == nameof(VmOutcome.Normal))
            .Select(static columns => columns[0]);

    /// <summary>A zero of <paramref name="type"/>, in the entry-point encoding.</summary>
    private static string Zero(byte type)
    {
        var (name, literal) = type switch
        {
            WasmAssembler.I32 => ("i32", "0"),
            0x7E => ("i64", "0"),
            0x7D => ("f32", "00000000"),
            _ => ("f64", "0000000000000000"),
        };

        return $"{name}:{literal.Length.ToString(CultureInfo.InvariantCulture)}:{literal}";
    }

    /// <summary>
    /// <paramref name="module"/> written again with one export section naming every function it
    /// defines, <c>f0</c> upward, and each function's parameter types.
    /// </summary>
    private static bool TryExportEverything(byte[] module, out byte[] rewritten, out List<byte[]> signatures, out string failure)
    {
        rewritten = [];
        signatures = [];
        var sections = new List<(byte Id, byte[] Body)>();
        var at = 8;

        while (at < module.Length)
        {
            var id = module[at++];

            if (!TryLeb(module, ref at, out var length) || at + (long)length > module.Length)
            {
                failure = "a section's length could not be read";
                return false;
            }

            sections.Add((id, module[at..(at + (int)length)]));
            at += (int)length;
        }

        var types = new List<byte[]>();
        var typeIndices = new List<uint>();

        foreach (var (id, body) in sections)
        {
            var cursor = 0;

            if (id == 1 && TryLeb(body, ref cursor, out var typeCount))
            {
                for (var type = 0u; type < typeCount; type++)
                {
                    // 0x60, the parameters, the results.
                    cursor++;
                    TryLeb(body, ref cursor, out var parameterCount);
                    types.Add(body[cursor..(cursor + (int)parameterCount)]);
                    cursor += (int)parameterCount;
                    TryLeb(body, ref cursor, out var resultCount);
                    cursor += (int)resultCount;
                }
            }
            else if (id == 3 && TryLeb(body, ref cursor, out var functionCount))
            {
                for (var function = 0u; function < functionCount; function++)
                {
                    TryLeb(body, ref cursor, out var typeIndex);
                    typeIndices.Add(typeIndex);
                }
            }
        }

        signatures = [.. typeIndices.Select(index => types[(int)index])];

        var exports = new List<byte>(WasmAssembler.Leb((uint)signatures.Count));

        for (var function = 0; function < signatures.Count; function++)
        {
            var name = System.Text.Encoding.UTF8.GetBytes($"f{function.ToString(CultureInfo.InvariantCulture)}");
            exports.AddRange(WasmAssembler.Leb((uint)name.Length));
            exports.AddRange(name);
            exports.Add(WasmAssembler.ExportFunction);
            exports.AddRange(WasmAssembler.Leb((uint)function));
        }

        // The export section goes before the first section that must follow it; an existing one is
        // replaced, and custom sections keep their places.
        var output = new List<byte>(module[..8]);
        var placed = false;

        foreach (var (id, body) in sections)
        {
            if (id == ExportSection)
            {
                continue;
            }

            if (!placed && id != 0 && Array.IndexOf(Order, id) > Array.IndexOf(Order, ExportSection))
            {
                output.AddRange(CorpusStore.Section(ExportSection, [.. exports]));
                placed = true;
            }

            output.AddRange(CorpusStore.Section(id, body));
        }

        if (!placed)
        {
            output.AddRange(CorpusStore.Section(ExportSection, [.. exports]));
        }

        rewritten = [.. output];
        failure = string.Empty;
        return true;
    }

    private static bool TryLeb(byte[] bytes, ref int at, out uint value)
    {
        value = 0;

        for (var shift = 0; shift < 35 && at < bytes.Length; shift += 7)
        {
            var current = bytes[at++];
            value |= (uint)(current & 0x7F) << shift;

            if ((current & 0x80) == 0)
            {
                return true;
            }
        }

        return false;
    }

    private static string Text(int value) => value.ToString(CultureInfo.InvariantCulture);
}
