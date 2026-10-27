// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The wide path's compile measurement, <c>--measure-compile</c>: one lane's turn at a set of
/// compile workloads, each compiled through <see cref="JsCompiler"/> with its control beside it,
/// writing each compilation's condition (its outcome and artifact hash) and its compile time (SJL-13's
/// tools commit, JSD-0083 decision D24).
/// </summary>
/// <remarks>
/// <para>
/// <b>Written 2026-10-27, at SJL-13's tools commit, before any change bundle <c>sjl-13-001</c>'s rule
/// judges.</b> This family had no compile-time harness. Part J of that rule, and part F of bundle
/// <c>sjl-13-002</c>'s, measure the wide path's compilation of the workloads
/// <c>docs/evidence/sjl-13-001/inputs/workloads</c> holds, in the core's discipline
/// (<c>docs/baselines.md</c> section 1); this mode is the timed child those parts run. Their arms are
/// builds of two commits, so no one process can hold two of them: the arms are interleaved by the
/// collector that runs this mode, each repetition starting one child per arm, and the A/A lane is the
/// after arm's binaries run a second time under another <c>--arm</c> name.
/// </para>
/// <para>
/// <b>What one child does, which is each of the core's five rules as far as one process can hold it.</b>
/// (1) Every workload is compiled beside its control, the same text as one block comment of the same
/// goal. (2) The two alternate inside the child: a workload, then its control, then the next workload.
/// (3) The lane's name is on every timed line, so the A/A lane is a name and not a second program.
/// (4) Every compilation it times is printed; nothing is dropped, averaged or judged here. (5) Every
/// timed compilation's own outcome and artifact hash are printed beside its time, so a lane that timed
/// a refusal says so in the same output; and the same condition is written by an untimed pass over
/// every file before the timed one and by another after it, the check before and after the lane.
/// </para>
/// <para>
/// <b>What is timed is the compilation and nothing around it</b>: one call of
/// <see cref="JsCompiler.Compile(IReadOnlyList{JsScriptUnit}, IReadOnlyList{JsModuleUnit}, JsCompileRequest)"/>
/// under the wide manifest in the bytecode form, which parses on the stack the compiler declares.
/// Process start, reading the file and hashing the artifact are outside it. Nothing is reset between
/// two compilations of one child.
/// </para>
/// <para>
/// <b>What is timed is a warm compiler: the child compiles every file once, untimed, before it times
/// any.</b> A child is one process, so under the JIT the first compilation that reaches a path of the
/// compiler's own code also pays for compiling that code. Timed cold, that cost would fall on whichever
/// workload came first to each path: a workload's time would depend on its place in the child, and its
/// control, which follows it and reaches less of the compiler, would run warm, so the workload minus
/// its control would charge the JIT to the workload and the pair would differ in more than one thing
/// (<c>docs/baselines.md</c> rule 1). Every repetition of the collector is a fresh child, so excluding
/// whole repetitions cannot remove that cost; the untimed pass does. Its condition lines, and those of
/// a closing untimed pass, are the checks before and after the lane of rule 5. The collector of the
/// measurement parts decision D24 names uses this mode as it is, warm; there is no cold mode. What a
/// warm compilation still pays is the runtime's own later tiering, which the collector holds to one
/// configuration through <c>--runtime</c>.
/// </para>
/// <para>
/// <b>The population is the directory's own statement of it.</b> Its <c>README.md</c> names each
/// workload, its control and its goal in one table row; a workload with no row, a row naming a file
/// that is not there, and a goal other than Script or Module are each refused before anything is
/// compiled. Each compilation names its unit by a fixed virtual path under <c>/workloads/</c>, because
/// a script's referrer and a module's key reach the artifact's bytes, and an artifact hash that
/// depended on where a checkout lies would be a different condition on every machine.
/// </para>
/// <para>
/// <b>It judges nothing.</b> No rule reads a time it writes but the measurement parts decision D24
/// names; the condition lines are the same on every run of one build over one directory, and the
/// collector compares them between arms. The exit code is 1 when any timed compilation was not
/// accepted, because the workloads are sources the wide path accepts and a timed refusal measures the
/// refusal path, and when a file's condition before, timed and after the lane is not one; the lines are
/// written either way.
/// </para>
/// </remarks>
internal static class CompileMeasurement
{
    /// <summary>The output format this mode writes.</summary>
    private const string Format = "broiler-vm-measure-compile\t1";

    /// <summary>The virtual directory every workload's unit is named under.</summary>
    private const string VirtualRoot = "/workloads/";

    /// <summary>
    /// A row of the workloads README's table: the workload, its control (the same stem), and the goal.
    /// </summary>
    private static readonly Regex TableRow = new(
        @"^\| `(?<stem>[a-z0-9][a-z0-9-]*)\.js` \| `\k<stem>\.control\.js` \| (?<goal>[A-Za-z]+) \|",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

    /// <summary>A lane's name: letters, digits, hyphens and underscores, so a line stays one field.</summary>
    private static readonly Regex ArmName = new(
        "^[A-Za-z0-9][A-Za-z0-9_-]{0,31}$", RegexOptions.CultureInvariant);

    /// <summary>One workload: its stem and its goal, <c>script</c> or <c>module</c>.</summary>
    private sealed record Workload(string Stem, string Goal)
    {
        internal string File => Stem + ".js";

        internal string Control => Stem + ".control.js";
    }

    /// <summary>What one timed compilation answered.</summary>
    private sealed record Answer(string Outcome, string Diagnostics, string Sha256, string Length, double Milliseconds)
    {
        internal bool Accepted => Outcome == "accepted";
    }

    /// <summary>Runs the mode over the arguments that follow <c>--measure-compile</c>.</summary>
    internal static int Run(string[] args)
    {
        string? directory = null, arm = null;
        var only = new List<string>();
        var runtime = false;

        for (var index = 0; index < args.Length; index++)
        {
            var hasValue = index + 1 < args.Length;

            switch (args[index])
            {
                case "--arm" when hasValue && arm is null: arm = args[++index]; break;
                case "--only" when hasValue: only.Add(args[++index]); break;
                case "--runtime": runtime = true; break;
                default:
                    if (args[index].StartsWith("--", StringComparison.Ordinal) || directory is not null)
                    {
                        return Refuse($"`{args[index]}` is not an argument of --measure-compile, or lacks its value");
                    }

                    directory = args[index];
                    break;
            }
        }

        if (directory is null || arm is null)
        {
            return Refuse("--measure-compile takes a workloads directory and --arm <name>");
        }

        if (!ArmName.IsMatch(arm))
        {
            return Refuse($"the arm `{arm}` is not a name of letters, digits, hyphens and underscores");
        }

        if (!TryLoad(directory, out var workloads, out var failure))
        {
            return Refuse(failure);
        }

        if (only.Count != 0)
        {
            var unknown = only.FirstOrDefault(stem => workloads.All(workload => workload.Stem != stem));

            if (unknown is not null)
            {
                return Refuse($"--only {unknown} names no workload of {directory}");
            }

            workloads = workloads.Where(workload => only.Contains(workload.Stem, StringComparer.Ordinal)).ToList();
        }

        return Measure(directory, arm, workloads, runtime);
    }

    private static int Refuse(string why)
    {
        Console.WriteLine("broiler-js-measure-compile: refused: " + why);
        Console.WriteLine(
            "usage: --measure-compile <workloads directory> --arm <name> [--only <workload>]... [--runtime]");
        return 2;
    }

    /// <summary>
    /// Reads the README's table and holds the directory to it: every workload has a row, every row's
    /// two files exist, and every goal is Script or Module.
    /// </summary>
    private static bool TryLoad(string directory, out List<Workload> workloads, out string failure)
    {
        var named = new List<Workload>();
        workloads = named;
        var readme = Path.Combine(directory, "README.md");

        if (!File.Exists(readme))
        {
            failure = $"no README.md in {directory} to name the workloads and their goals";
            return false;
        }

        foreach (var line in File.ReadAllLines(readme))
        {
            var match = TableRow.Match(line);

            if (!match.Success)
            {
                continue;
            }

            var stem = match.Groups["stem"].Value;
            var goal = match.Groups["goal"].Value switch
            {
                "Script" => "script",
                "Module" => "module",
                var other => other,
            };

            if (goal is not ("script" or "module"))
            {
                failure = $"the README gives {stem}.js the goal `{goal}`, which is neither Script nor Module";
                return false;
            }

            if (named.Any(workload => workload.Stem == stem))
            {
                failure = $"the README names {stem}.js twice";
                return false;
            }

            named.Add(new Workload(stem, goal));
        }

        if (workloads.Count == 0)
        {
            failure = $"the README in {directory} has no row naming a workload, its control and its goal";
            return false;
        }

        foreach (var workload in workloads)
        {
            foreach (var file in new[] { workload.File, workload.Control })
            {
                if (!File.Exists(Path.Combine(directory, file)))
                {
                    failure = $"the README names {file}, and {directory} holds no such file";
                    return false;
                }
            }
        }

        var unnamed = Directory.EnumerateFiles(directory, "*.js")
            .Select(Path.GetFileName)
            .OfType<string>()
            .Where(name => !name.EndsWith(".control.js", StringComparison.Ordinal))
            .Where(name => named.All(workload => workload.File != name))
            .Order(StringComparer.Ordinal)
            .FirstOrDefault();

        if (unnamed is not null)
        {
            failure = $"{unnamed} is in {directory} and the README gives it no row";
            return false;
        }

        workloads.Sort(static (left, right) => string.CompareOrdinal(left.Stem, right.Stem));
        failure = string.Empty;
        return true;
    }

    /// <summary>The child itself: every workload, then its control, each timed and reported.</summary>
    private static int Measure(string directory, string arm, IReadOnlyList<Workload> workloads, bool runtime)
    {
        // LF, explicitly, on every platform: the collector retains these lines and compares the
        // condition lines byte for byte between arms.
        void Line(string text)
        {
            Console.Out.Write(text);
            Console.Out.Write('\n');
        }

        Line("#% format\t" + Format);
        Line("#% arm\t" + arm);
        Line("#% workloads\t" + workloads.Count.ToString(CultureInfo.InvariantCulture));
        Line(
            "# surface: JsCompiler.Compile, the wide manifest, the bytecode form, on the stack the compiler " +
            "declares; a script is one unit named test whose referrer is /workloads/<file>, a module one unit " +
            "keyed /workloads/<file>");
        Line(
            "# before and after <file> <goal> <outcome> <diagnostics> <artifact sha256> <artifact length>, " +
            "untimed, every file before the timed pass and again after it; source <file> <goal> <sha256> " +
            "<bytes>; condition <file> <goal> <outcome> <diagnostics> <artifact sha256> <artifact length>; " +
            "time <arm> <file> <compile milliseconds>");

        if (runtime)
        {
            Line(RuntimeLine());
        }

        // THE CHECK BEFORE THE LANE, UNTIMED, which is also what warms the compiler's code: every file
        // the timed pass compiles, in its order, once (docs/baselines.md rule 5, and the class's remarks).
        var before = Untimed("before");

        var refused = 0;
        var timed = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var workload in workloads)
        {
            // A WORKLOAD, THEN ITS CONTROL (docs/baselines.md rule 2), each read before its timed region
            // and reported after it.
            foreach (var file in new[] { workload.File, workload.Control })
            {
                var bytes = File.ReadAllBytes(Path.Combine(directory, file));
                var text = Decode(bytes);
                Line(string.Join(
                    '\t',
                    "source",
                    file,
                    workload.Goal,
                    Sha256(bytes),
                    bytes.Length.ToString(CultureInfo.InvariantCulture)));

                var answer = Compile(file, workload.Goal, text);

                if (!answer.Accepted)
                {
                    refused++;
                }

                timed[file] = Condition(file, workload.Goal, answer);
                Line("condition\t" + timed[file]);
                Line(string.Join(
                    '\t', "time", arm, file, answer.Milliseconds.ToString("F3", CultureInfo.InvariantCulture)));
            }
        }

        // THE CHECK AFTER THE LANE, UNTIMED, as the one before it.
        var after = Untimed("after");

        var moved = timed.Keys.Count(file => before[file] != timed[file] || after[file] != timed[file]);

        Line(
            $"# {(2 * workloads.Count).ToString(CultureInfo.InvariantCulture)} compilations timed, " +
            $"{refused.ToString(CultureInfo.InvariantCulture)} not accepted; " +
            $"{moved.ToString(CultureInfo.InvariantCulture)} files whose condition before, timed and after " +
            "is not one");

        return refused == 0 && moved == 0 ? 0 : 1;

        // One untimed compilation of every file, in the timed pass's order, each written as its condition.
        Dictionary<string, string> Untimed(string check)
        {
            var conditions = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var workload in workloads)
            {
                foreach (var file in new[] { workload.File, workload.Control })
                {
                    var text = Decode(File.ReadAllBytes(Path.Combine(directory, file)));
                    conditions[file] = Condition(file, workload.Goal, Compile(file, workload.Goal, text));
                    Line(check + "\t" + conditions[file]);
                }
            }

            return conditions;
        }
    }

    /// <summary>A compilation's condition: the file, its goal, the outcome, the diagnostics and the artifact.</summary>
    private static string Condition(string file, string goal, Answer answer) =>
        string.Join('\t', file, goal, answer.Outcome, answer.Diagnostics, answer.Sha256, answer.Length);

    /// <summary>
    /// Compiles one file under its goal and times the call; it never throws, and an exception that left
    /// the compiler is reported as the outcome <c>crashed</c>.
    /// </summary>
    private static Answer Compile(string file, string goal, string text)
    {
        var request = new JsCompileRequest(JsFeatureManifest.Wide, JsOutputForm.Bytecode);
        IReadOnlyList<JsScriptUnit> scripts = goal == "module"
            ? []
            : [new JsScriptUnit("test", text, SliceParseOptions.Script, ForceStrict: false, VirtualRoot + file)];
        IReadOnlyList<JsModuleUnit> modules = goal == "module"
            ? [new JsModuleUnit(VirtualRoot + file, text, SliceParseOptions.Module)]
            : [];

        JsCompilation compiled;
        var started = Stopwatch.GetTimestamp();

        try
        {
            compiled = JsCompiler.Compile(scripts, modules, request);
        }
        catch (Exception failure) when (failure is not OutOfMemoryException)
        {
            var lost = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
            return new Answer("crashed", failure.GetType().FullName ?? failure.GetType().Name, "-", "-", lost);
        }

        var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
        var diagnostics = compiled.Diagnostics.Count == 0
            ? "-"
            : string.Join(
                ';',
                compiled.Diagnostics.Select(diagnostic => string.Create(
                    CultureInfo.InvariantCulture,
                    $"{(int)diagnostic.Code}:{diagnostic.Line}:{diagnostic.Column}")));

        return compiled.Succeeded && compiled.Artifact is { } artifact
            ? new Answer(
                "accepted", diagnostics, Sha256(artifact), artifact.Length.ToString(CultureInfo.InvariantCulture), elapsed)
            : new Answer("refused", diagnostics, "-", "-", elapsed);
    }

    /// <summary>
    /// The runtime configuration this process got, in the command-line root's words
    /// (<c>--runtime</c> there), so that a collector holds every timed child to one line.
    /// </summary>
    /// <remarks>
    /// Restated rather than referenced: a composition root references no other. Tiering is read from the
    /// environment and from the runtime configuration both, because the runtime takes it from both, and
    /// neither is the tier state itself, which no runtime API reports.
    /// </remarks>
    private static string RuntimeLine() =>
        $"runtime-identifier={System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier} " +
        $"process-architecture={System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture} " +
        $"gc-server={System.Runtime.GCSettings.IsServerGC} " +
        $"gc-latency={System.Runtime.GCSettings.LatencyMode} " +
        $"tiered-compilation-env={Asked("TieredCompilation")} " +
        $"tiered-compilation-config={Configured("System.Runtime.TieredCompilation")} " +
        $"tiered-pgo-env={Asked("TieredPGO")} " +
        $"tiered-pgo-config={Configured("System.Runtime.TieredPGO")} " +
        $"dynamic-code={System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeCompiled} " +
        $"native-arming={(JsNativePage.Mapper is null ? "none" : "installed")}";

    /// <summary>What the environment asks of the runtime for one knob, or <c>unset</c>.</summary>
    private static string Asked(string knob)
    {
        var current = Environment.GetEnvironmentVariable("DOTNET_" + knob);

        if (!string.IsNullOrEmpty(current))
        {
            return current;
        }

        var legacy = Environment.GetEnvironmentVariable("COMPlus_" + knob);
        return string.IsNullOrEmpty(legacy) ? "unset" : legacy;
    }

    /// <summary>What the runtime configuration states for one switch, or <c>unset</c>.</summary>
    private static string Configured(string name) =>
        AppContext.GetData(name) is { } value ? value.ToString() ?? "unset" : "unset";

    /// <summary>Decodes a file as the conformance root does: UTF-8, a leading byte-order mark dropped.</summary>
    private static string Decode(byte[] bytes)
    {
        var start = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? 3 : 0;
        return Encoding.UTF8.GetString(bytes, start, bytes.Length - start);
    }

    private static string Sha256(byte[] bytes) => Convert.ToHexStringLower(SHA256.HashData(bytes));
}
