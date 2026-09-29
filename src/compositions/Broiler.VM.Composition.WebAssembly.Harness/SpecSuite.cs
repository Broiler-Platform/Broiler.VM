using Broiler.VM;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// The specification's core test scripts through the core, one line per command: the lane bundle
/// UBC-4-005's population A is read by.
/// </summary>
/// <remarks>
/// <para>
/// <b>A pinned directory, or nothing.</b> Given <c>--expect</c>, the lane digests every file of the
/// directory it was handed, as the pin defines the digest, and refuses a directory that is not the one
/// the pin names before it reads a script. A run without a pin says it is unpinned, which no bundle
/// accepts.
/// </para>
/// <para>
/// <b>Every command is printed</b>, whatever its verdict, because the bundle compares answers line by
/// line. A failing verdict is not a failing run: a module that imports, which this profile cannot link,
/// fails its verdict in every run. The run fails only when the directory does not match its pin, or when
/// a command the floor says passes no longer does. <i>(Noted 2026-09-29: and, for WA-4's gate, when the
/// self-check or its negative control fails, and on a named configuration failure - an empty selection,
/// no executed tests, or a family that selected commands and executed none - which exits with four
/// rather than reporting a small total. Each assertion family's totals are printed in the roadmap's six
/// counts.)</i> <i>(Noted again 2026-09-29: the lane selects through the recorded pipeline of
/// <see cref="SpecSelection"/>, runs one shard of it with <c>--shard i/n</c>, and writes the shard's
/// report with <c>--report</c>; <c>--merge</c> reads a directory of reports, proves they cover the whole
/// selection (<see cref="SpecReport"/>), and holds the merged totals to the ratchet and the merged
/// failures to the queue (<see cref="FailureQueue"/>). The tooling's own regression suite,
/// <see cref="ToolingChecks"/>, runs before any shard and before any merge.)</i>
/// </para>
/// <para>
/// <b><c>--encode-to</c> reads and encodes, and runs nothing.</b> It writes every text module the
/// scripts define, a quoted one excepted, as the binary the reader would hand the core. The reader's
/// encoder can then be compared with another encoder's without any module being verified, instantiated
/// or invoked.
/// </para>
/// </remarks>
internal static class SpecSuite
{
    /// <summary>Runs the lane with the process's arguments.</summary>
    internal static int Run(string[] args)
    {
        var directory = Argument(args, "--spec");

        if (directory is null || !Directory.Exists(directory))
        {
            Console.WriteLine("broiler-wasm-harness: --spec needs the directory holding the scripts");
            return 2;
        }

        var pin = Argument(args, "--expect");
        var revision = "unpinned";

        if (pin is not null)
        {
            var (checkedRevision, failure) = CheckPin(directory, pin);

            if (failure is not null)
            {
                Console.WriteLine($"broiler-wasm-harness: {failure}");
                return 2;
            }

            revision = checkedRevision!;
        }

        var ratchet = Argument(args, "--ratchet");
        var writeRatchet = Argument(args, "--write-ratchet");

        // A ratchet is bound to the suite revision it was set under, so neither reading nor writing one
        // means anything for a directory the lane cannot name a revision for.
        if ((ratchet is not null || writeRatchet is not null) && pin is null)
        {
            Console.WriteLine("# spec: CONFIGURATION FAILURE missing suite revision: a ratchet is read or written only for a pinned directory, and no --expect was given");
            return 4;
        }

        var queue = Argument(args, "--queue");
        var writeQueue = Argument(args, "--write-queue");

        if ((queue is not null || writeQueue is not null) && pin is null)
        {
            Console.WriteLine("# spec: CONFIGURATION FAILURE missing suite revision: a failure queue is read or written only for a pinned directory, and no --expect was given");
            return 4;
        }

        if (!SpecSelection.TryParseShard(Argument(args, "--shard"), out var shard, out var shards))
        {
            Console.WriteLine("broiler-wasm-harness: --shard takes i/n, a shard index below a shard count of at least one");
            return 2;
        }

        // A RATCHET AND A QUEUE ARE WRITTEN FROM A WHOLE SELECTION, and a ratchet is held by one: one
        // shard's totals are part of a total, and the merge is where the whole is held.
        if (shards > 1 && (ratchet is not null || writeRatchet is not null || writeQueue is not null))
        {
            Console.WriteLine("# spec: CONFIGURATION FAILURE a ratchet is held, and a ratchet or a queue written, by a whole selection: run unsharded, or give them to --merge");
            return 4;
        }

        // A CONFIGURATION FAILURE IS A FAILURE, NOT A SMALL TOTAL. A directory holding no script would
        // otherwise report zero of everything and exit as cleanly as a full run.
        if (!SpecSelection.TrySelect(directory, Argument(args, "--scope"), Argument(args, "--known-incorrect"), shard, shards, out var selection, out var selectionFailure))
        {
            Console.WriteLine($"# spec: CONFIGURATION FAILURE {selectionFailure}");
            return 4;
        }

        var scripts = selection.InShard.ToList();
        var encodeTo = Argument(args, "--encode-to");

        if (encodeTo is not null)
        {
            return Encode([.. selection.Selected], encodeTo);
        }

        // THE HARNESS'S OWN REGRESSION SUITE, then the reader's self-check, before any shard starts.
        if (!ToolingChecks.Run())
        {
            Console.WriteLine("broiler-wasm-harness: the tooling's regression suite failed, so no script was run");
            return 3;
        }

        if (!SelfCheck())
        {
            Console.WriteLine("broiler-wasm-harness: the reader's self-check failed, so no script was run");
            return 3;
        }

        Console.WriteLine($"# spec: revision {revision}, {scripts.Count.ToString(CultureInfo.InvariantCulture)} scripts, each in a runtime of its own");
        Console.WriteLine($"# spec: effective limits {Limits()}");
        SpecSelection.Print(selection);

        var all = new List<ScriptCommand>();

        foreach (var (name, path) in scripts)
        {
            foreach (var command in ScriptRunner.Run(name, File.ReadAllBytes(path)))
            {
                Console.WriteLine(command);
                all.Add(command);
            }
        }

        foreach (var group in all.GroupBy(static command => command.Command).OrderBy(static group => group.Key, StringComparer.Ordinal))
        {
            Console.WriteLine(
                $"# spec: {group.Key}: {group.Count().ToString(CultureInfo.InvariantCulture)} commands, " +
                $"{Count(group, ScriptVerdict.Pass)} pass, {Count(group, ScriptVerdict.Fail)} fail, " +
                $"{Count(group, ScriptVerdict.Excluded)} excluded");
        }

        Console.WriteLine(
            $"# spec: {all.Count.ToString(CultureInfo.InvariantCulture)} commands, " +
            $"{Count(all, ScriptVerdict.Pass)} pass, {Count(all, ScriptVerdict.Fail)} fail, {Count(all, ScriptVerdict.Excluded)} excluded");

        var failures = Families(all, out var totals);

        var verdicts = Argument(args, "--write-verdicts");

        if (verdicts is not null)
        {
            File.WriteAllText(verdicts, Verdicts(revision, all), new UTF8Encoding(false));
            Console.WriteLine($"# spec: verdicts written to {verdicts}");
        }

        if (failures > 0)
        {
            return 4;
        }

        var floor = Argument(args, "--floor");
        var code = floor is null ? 0 : CheckFloor(floor, revision, all);

        var failing = scripts.ToDictionary(
            static script => script.Name,
            script => all.Count(command => command.File == script.Name && command.Verdict is ScriptVerdict.Fail),
            StringComparer.Ordinal);

        var report = Argument(args, "--report");

        if (report is not null)
        {
            File.WriteAllText(report, SpecReport.Write(Configuration(revision, selection), shard, scripts.Select(static script => script.Name), totals, failing), new UTF8Encoding(false));
            Console.WriteLine($"# spec: shard report written to {report}");
        }

        return Hold(code, revision, Limits(), totals, failing,
            selection.Selected.Select(static script => script.Name).ToHashSet(StringComparer.Ordinal),
            scripts.Select(static script => script.Name).ToHashSet(StringComparer.Ordinal),
            ratchet, writeRatchet, queue, writeQueue);
    }

    /// <summary>
    /// Merges the shard reports in a directory, proves they cover the whole selection, and holds the merged
    /// totals to a ratchet and the merged failures to a queue.
    /// </summary>
    internal static int Merge(string[] args)
    {
        var directory = Argument(args, "--merge");

        if (directory is null || !Directory.Exists(directory))
        {
            Console.WriteLine("broiler-wasm-harness: --merge needs the directory holding the shard reports");
            return 2;
        }

        if (!ToolingChecks.Run())
        {
            Console.WriteLine("broiler-wasm-harness: the tooling's regression suite failed, so nothing was merged");
            return 3;
        }

        var reports = Directory.GetFiles(directory, "*.report")
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(static path => (Source: Path.GetFileName(path), Text: File.ReadAllText(path)))
            .ToList();

        var failures = new List<string>();
        var (families, failing, scripts, configuration) = SpecReport.Merge(reports, failures);

        foreach (var failure in failures)
        {
            Console.WriteLine($"# merge: CONFIGURATION FAILURE {failure}");
        }

        if (failures.Count > 0)
        {
            return 4;
        }

        Console.WriteLine(
            $"# merge: {reports.Count.ToString(CultureInfo.InvariantCulture)} shard reports of {configuration["shards"]}, revision {configuration["revision"]}, " +
            $"{scripts.Count.ToString(CultureInfo.InvariantCulture)} scripts of the {configuration["selected"]} selected before sharding: the whole selection");
        Console.WriteLine($"# merge: effective limits {configuration["limits"]}");

        foreach (var (family, _) in AssertionFamilies)
        {
            var totals = families.GetValueOrDefault(family);
            Console.WriteLine(
                $"# merge: family {family}: selected {totals.Selected.ToString(CultureInfo.InvariantCulture)}, executed {totals.Executed.ToString(CultureInfo.InvariantCulture)}, " +
                $"passed {totals.Passed.ToString(CultureInfo.InvariantCulture)}, failed {totals.Failed.ToString(CultureInfo.InvariantCulture)}, " +
                $"skipped {totals.Skipped.ToString(CultureInfo.InvariantCulture)}, timed out 0");
        }

        return Hold(0, configuration["revision"], configuration["limits"], families, failing, scripts, scripts,
            Argument(args, "--ratchet"), Argument(args, "--write-ratchet"), Argument(args, "--queue"), Argument(args, "--write-queue"));
    }

    /// <summary>A run's configuration, as its shard report states it.</summary>
    private static Dictionary<string, string> Configuration(string revision, SpecSelection.Selection selection) =>
        new(StringComparer.Ordinal)
        {
            ["report"] = "1",
            ["revision"] = revision,
            ["manifest"] = Broiler.VM.Profile.WebAssembly.WebAssemblyProfile.SliceManifest.ToString(),
            ["limits"] = Limits(),
            ["scope"] = selection.Scope,
            ["candidates"] = selection.Candidates.ToString(CultureInfo.InvariantCulture),
            ["known-incorrect"] = selection.KnownIncorrect.Count.ToString(CultureInfo.InvariantCulture),
            ["out-of-scope"] = selection.OutOfScope.Count.ToString(CultureInfo.InvariantCulture),
            ["unselectable"] = selection.Unselectable.Count.ToString(CultureInfo.InvariantCulture),
            ["selected"] = selection.Selected.Count.ToString(CultureInfo.InvariantCulture),
            ["shards"] = selection.Shards.ToString(CultureInfo.InvariantCulture),
        };

    /// <summary>Holds totals to a ratchet and failures to a queue, and writes either; answers the exit code.</summary>
    private static int Hold(
        int code, string revision, string limits,
        IReadOnlyDictionary<string, FamilyTotals> totals, IReadOnlyDictionary<string, int> failing,
        IReadOnlySet<string> selected, IReadOnlySet<string> ran,
        string? ratchet, string? writeRatchet, string? queue, string? writeQueue)
    {
        if (ratchet is not null)
        {
            code = Math.Max(code, Ratchet.Check(ratchet, revision, limits, totals));
        }

        if (writeRatchet is not null)
        {
            code = Math.Max(code, Ratchet.Write(writeRatchet, revision, limits, totals));
        }

        if (queue is not null)
        {
            if (!File.Exists(queue))
            {
                Console.WriteLine($"# spec: CONFIGURATION FAILURE queue {queue}: no such file");
                return 4;
            }

            if (!FailureQueue.TryParse(File.ReadAllText(queue), out var parsed, out var failure))
            {
                Console.WriteLine($"# spec: CONFIGURATION FAILURE queue {queue}: {failure}");
                return 4;
            }

            var violations = FailureQueue.Check(parsed, revision, selected, ran, failing);

            foreach (var violation in violations)
            {
                Console.WriteLine($"# spec: QUEUE {violation}");
            }

            var judged = parsed.Entries.Keys.Count(ran.Contains);
            Console.WriteLine(
                $"# spec: queue {queue}: {judged.ToString(CultureInfo.InvariantCulture)} of {parsed.Entries.Count.ToString(CultureInfo.InvariantCulture)} entries judged by this run; " +
                (violations.Count == 0 ? "every one confirmed, and no failure unlisted" : $"{violations.Count.ToString(CultureInfo.InvariantCulture)} disagreements"));
            code = Math.Max(code, violations.Count == 0 ? 0 : 1);
        }

        if (writeQueue is not null)
        {
            File.WriteAllText(writeQueue, FailureQueue.Write(revision, failing), new UTF8Encoding(false));
            Console.WriteLine($"# spec: failure queue written to {writeQueue}");
        }

        return code;
    }

    /// <summary>One assertion family's totals, in the roadmap's counts; timed out is zero by construction.</summary>
    internal readonly record struct FamilyTotals(int Selected, int Executed, int Passed, int Failed, int Skipped);

    /// <summary>The assertion families the roadmap names, and the commands each is made of.</summary>
    private static readonly (string Family, string[] Commands)[] AssertionFamilies =
    [
        ("malformed", ["assert_malformed"]),
        ("invalid", ["assert_invalid"]),
        ("unlinkable", ["assert_unlinkable"]),
        ("uninstantiable", ["assert_uninstantiable"]),
        ("trap", ["assert_trap"]),
        ("exception", ["assert_exception"]),
        ("exhaustion", ["assert_exhaustion"]),
        ("return", ["assert_return", "assert_return_canonical_nan", "assert_return_arithmetic_nan"]),
    ];

    /// <summary>
    /// Prints each assertion family's totals in the shape the roadmap asks for, and every configuration
    /// failure the run shows; answers how many there were.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Six counts a family, never a percentage.</b> Selected is every command of the family's kinds;
    /// executed is those the lane scored; skipped is those it excluded, a quoted text module being the
    /// one class today; passed and failed split the executed. Timed out is zero by construction: the
    /// lane runs no timer, and every command is bounded by its runtime's budget, whose breach is an
    /// answer and not a timeout. A family the revision holds no command of reports zeros.
    /// </para>
    /// <para>
    /// <b>The configuration failures a run can show.</b> The roadmap's closed set is inconsistent shard
    /// configuration, a missing suite revision, an empty selection, no executed tests, and a scope
    /// manifest naming a file the suite does not contain. This lane runs no shards and reads no scope
    /// manifest, so the first and the last cannot arise; an empty selection is refused before any
    /// script is read, and a missing revision where a ratchet needs one is refused with it. Here are the
    /// other two: a run that executed nothing, and a family that selected commands and executed none -
    /// which is the roadmap's "a named configuration failure, not a small total".
    /// </para>
    /// </remarks>
    private static int Families(List<ScriptCommand> all, out Dictionary<string, FamilyTotals> totals)
    {
        var failures = 0;
        totals = new Dictionary<string, FamilyTotals>(StringComparer.Ordinal);

        foreach (var (family, kinds) in AssertionFamilies)
        {
            var selected = all.Where(command => kinds.Contains(command.Command, StringComparer.Ordinal)).ToList();
            var passed = selected.Count(static command => command.Verdict is ScriptVerdict.Pass);
            var failed = selected.Count(static command => command.Verdict is ScriptVerdict.Fail);
            var skipped = selected.Count(static command => command.Verdict is ScriptVerdict.Excluded);
            var executed = passed + failed;
            totals[family] = new FamilyTotals(selected.Count, executed, passed, failed, skipped);

            Console.WriteLine(
                $"# spec: family {family}: selected {Text(selected.Count)}, executed {Text(executed)}, passed {Text(passed)}, " +
                $"failed {Text(failed)}, skipped {Text(skipped)}, timed out 0");

            if (selected.Count > 0 && executed == 0)
            {
                Console.WriteLine($"# spec: CONFIGURATION FAILURE family {family} selected {Text(selected.Count)} commands and executed none");
                failures++;
            }
        }

        if (all.All(static command => command.Verdict is ScriptVerdict.Excluded))
        {
            Console.WriteLine($"# spec: CONFIGURATION FAILURE no executed tests: {Text(all.Count)} commands read, none scored");
            failures++;
        }

        return failures;

        static string Text(int value) => value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// A script this root wrote, every command of which declares its verdict, run before any script of
    /// the suite: the reader has to answer each one as declared.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Half of its commands are declared to fail</b>: a wrong value, a wrong trap message, a global
    /// read, a missing export. A reader whose verdicts passed everything would pass the other half, and
    /// these catch it. Every command kind the suite uses is here at least once, and so is each way a
    /// module can end: instantiated, refused at verification, trapped at instantiation, unlinkable and
    /// excluded. It holds no suite material.
    /// </para>
    /// <para>
    /// <b>Each module assertion is here passing and failing, by what the refusal says.</b> A module
    /// that is both malformed and invalid - a type index that addresses nothing, then a section
    /// identifier that names nothing - has to pass as malformed and fail as invalid; a scorer that did
    /// not tell them apart passes it as both. An invalid module fails as malformed, a malformed one fails
    /// as invalid, and a feature this profile does not admit fails as either. A malformation inside a
    /// function body - <c>memory.size</c> with a reserved byte that is not zero - is found by the
    /// validator and passes as malformed, because the refusal's reason, not the pass, is what says so.
    /// A module that imports, which verification refuses, fails as unlinkable, and one whose data segment
    /// does not fit its memory passes.
    /// <i>(Added 2026-09-28, with the scoring those fixtures pin. The import was declared to pass as
    /// unlinkable, and the scoring passed it.)</i>
    /// </para>
    /// <para>
    /// <b>A module written as bare fields</b>, with no <c>module</c> around them, is read as one module
    /// and instantiated, and the assertion after it reads its export. The script grammar allows it and
    /// the suite's <c>inline-module.wast</c> is written that way; a reader taking each field for a command
    /// of its own refuses both. <i>(Added 2026-09-29.)</i>
    /// </para>
    /// </remarks>
    private static bool SelfCheck()
    {
        const string Script = """
            (module $M
              (func (export "add") (param i32 i32) (result i32) (i32.add (local.get 0) (local.get 1)))
              (func (export "div") (param i32 i32) (result i32) (i32.div_s (local.get 0) (local.get 1)))
              (func (export "nan") (result f32) (f32.div (f32.const 0) (f32.const 0)))
              (func $deep (export "deep") (result i32) (call $deep))
              (global (export "g") i32 (i32.const 7)))
            (register "M" $M)
            (assert_return (invoke "add" (i32.const 40) (i32.const 2)) (i32.const 42))
            (assert_return (invoke "add" (i32.const 40) (i32.const 2)) (i32.const 43))
            (assert_trap (invoke "div" (i32.const 1) (i32.const 0)) "integer divide by zero")
            (assert_trap (invoke "div" (i32.const 1) (i32.const 0)) "unreachable")
            (assert_return_canonical_nan (invoke "nan"))
            (assert_return_arithmetic_nan (invoke "add" (i32.const 1) (i32.const 1)))
            (assert_return (get "g") (i32.const 7))
            (assert_return (invoke "absent"))
            (invoke $M "add" (i32.const 1) (i32.const 2))
            (assert_invalid (module (func (result i32))) "type mismatch")
            (assert_invalid (module (func (result i32) (i32.const 0))) "type mismatch")
            (assert_malformed (module binary "\00asm" "\02\00\00\00") "unknown binary version")
            (assert_malformed (module quote "(func") "unexpected end")
            (assert_unlinkable (module (import "nowhere" "f" (func))) "unknown import")
            (assert_unlinkable (module (memory 0) (data (i32.const 0) "a")) "data segment does not fit")
            (assert_malformed (module binary "\00asm" "\01\00\00\00" "\03\02\01\05" "\0e\00") "malformed section id")
            (assert_invalid (module binary "\00asm" "\01\00\00\00" "\03\02\01\05" "\0e\00") "unknown type")
            (assert_malformed (module (func (result i32))) "type mismatch")
            (assert_invalid (module binary "\00asm" "\02\00\00\00") "unknown binary version")
            (assert_invalid (module (memory 0) (memory 0)) "multiple memories")
            (assert_malformed (module binary "\00asm" "\01\00\00\00" "\01\04\01\60\00\00" "\03\02\01\00" "\05\03\01\00\00" "\0a\07\01\05\00\3f\01\1a\0b") "zero byte expected")
            (assert_trap (module (func $start unreachable) (start $start)) "unreachable")
            (module (func (export "one") (result i32) (i32.const 1)))
            (assert_return (invoke "one") (i32.const 1))
            (assert_exhaustion (invoke $M "deep") "call stack exhausted")
            (func (export "two") (result i32) (i32.const 2))
            (assert_return (invoke "two") (i32.const 2))
            """;

        ScriptVerdict[] declared =
        [
            ScriptVerdict.Pass, ScriptVerdict.Pass, ScriptVerdict.Fail, ScriptVerdict.Pass, ScriptVerdict.Fail,
            ScriptVerdict.Pass, ScriptVerdict.Fail, ScriptVerdict.Fail, ScriptVerdict.Fail, ScriptVerdict.Pass,
            ScriptVerdict.Pass, ScriptVerdict.Fail, ScriptVerdict.Pass, ScriptVerdict.Excluded, ScriptVerdict.Fail,
            ScriptVerdict.Pass, ScriptVerdict.Pass, ScriptVerdict.Fail, ScriptVerdict.Fail, ScriptVerdict.Fail,
            ScriptVerdict.Fail, ScriptVerdict.Pass, ScriptVerdict.Pass, ScriptVerdict.Pass, ScriptVerdict.Pass,
            ScriptVerdict.Pass, ScriptVerdict.Pass, ScriptVerdict.Pass,
        ];

        var commands = ScriptRunner.Run("self-check.wast", Encoding.UTF8.GetBytes(Script));
        var passed = Disagreements(commands, declared, print: true) == 0;

        Console.WriteLine($"# self-check: {commands.Count.ToString(CultureInfo.InvariantCulture)} commands for {declared.Length.ToString(CultureInfo.InvariantCulture)} declared verdicts, {(passed ? "every one as declared" : "NOT as declared")}");

        if (!passed)
        {
            return false;
        }

        // THE NEGATIVE CONTROL: a scoring regression injected, observed and reverted. The scorer is made
        // to pass a malformed or an invalid assertion on any refusal, as it did until 2026-09-28, and the
        // same script run again must disagree with its declared verdicts; the regression is reverted
        // before any script of the suite is read, whatever happens.
        int regressed;
        ScriptRunner.ScoresAnyRefusal = true;

        try
        {
            regressed = Disagreements(ScriptRunner.Run("self-check.wast", Encoding.UTF8.GetBytes(Script)), declared, print: false);
        }
        finally
        {
            ScriptRunner.ScoresAnyRefusal = false;
        }

        Console.WriteLine(
            $"# self-check control: a scorer passing any refusal as malformed or invalid disagrees with {regressed.ToString(CultureInfo.InvariantCulture)} " +
            $"declared verdicts{(regressed > 0 ? ", and it is reverted" : ", so the self-check could not see the regression")}");

        return regressed > 0;
    }

    /// <summary>How many commands answer other than declared, a missing or an extra command each counting once.</summary>
    private static int Disagreements(List<ScriptCommand> commands, ScriptVerdict[] declared, bool print)
    {
        var disagreements = Math.Abs(commands.Count - declared.Length);

        for (var index = 0; index < commands.Count; index++)
        {
            var expected = index < declared.Length ? declared[index] : (ScriptVerdict?)null;
            var agrees = expected == commands[index].Verdict;

            if (!agrees && index < declared.Length)
            {
                disagreements++;
            }

            if (print)
            {
                Console.WriteLine($"# self-check {(agrees ? "ok  " : "FAIL")} {commands[index]} (declared {expected?.ToString().ToLowerInvariant() ?? "nothing"})");
            }
        }

        return disagreements;
    }

    /// <summary>Digests the directory as the pin defines it and compares the digest, the count and the revision.</summary>
    private static (string? Revision, string? Failure) CheckPin(string directory, string pinFile)
    {
        var fields = File.ReadAllLines(pinFile)
            .Where(static line => line.Length > 0 && line[0] != '#')
            .Select(static line => line.Split(' ', 2))
            .Where(static parts => parts.Length == 2)
            .ToDictionary(static parts => parts[0], static parts => parts[1], StringComparer.Ordinal);

        if (!fields.TryGetValue("revision", out var revision) ||
            !fields.TryGetValue("content-sha256", out var expected) ||
            !fields.TryGetValue("files", out var count))
        {
            return (null, $"the pin {pinFile} does not name a revision, a content digest and a file count");
        }

        var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
            .Select(path => (Path: Path.GetRelativePath(directory, path).Replace('\\', '/'), Hash: Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)))))
            .OrderBy(static file => file.Path, StringComparer.Ordinal)
            .ToList();

        var text = new StringBuilder();

        foreach (var (path, hash) in files)
        {
            text.Append(path).Append('\n').Append(hash).Append('\n');
        }

        var digest = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString())));

        if (!string.Equals(files.Count.ToString(CultureInfo.InvariantCulture), count, StringComparison.Ordinal) ||
            !string.Equals(digest, expected, StringComparison.Ordinal))
        {
            return (null,
                $"the directory {directory} is not the pinned suite: {files.Count.ToString(CultureInfo.InvariantCulture)} files digesting to {digest}, " +
                $"where the pin names {count} files digesting to {expected}");
        }

        return (revision, null);
    }

    private static string Limits()
    {
        using var runtime = ScriptRunner.CreateRuntime(out var failure) ??
            throw new InvalidOperationException(failure);

        var snapshot = runtime.GetBudgetSnapshot();

        return string.Join(" ", VmBudgetDimensions.All.ToArray().Select(dimension =>
            $"{dimension}={snapshot.EffectiveCeiling(dimension).ToString(CultureInfo.InvariantCulture)}"));
    }

    private static int Encode(List<(string Name, string Path)> scripts, string into)
    {
        Directory.CreateDirectory(into);
        var written = 0;
        var refused = 0;

        foreach (var (name, path) in scripts)
        {
            var commands = ScriptText.Read(File.ReadAllBytes(path));
            var ordinal = 0;

            foreach (var command in commands)
            {
                ordinal++;

                foreach (var module in Modules(command))
                {
                    var file = Path.Combine(into, $"{name}.{ordinal.ToString(CultureInfo.InvariantCulture)}.wasm");

                    try
                    {
                        File.WriteAllBytes(file, TextModule.Encode(module));
                        written++;
                    }
                    catch (ScriptReadException failure)
                    {
                        File.WriteAllText(file + ".refused", failure.Message);
                        refused++;
                    }
                }
            }
        }

        Console.WriteLine($"# spec: {written.ToString(CultureInfo.InvariantCulture)} text modules encoded, {refused.ToString(CultureInfo.InvariantCulture)} refused by the reader, nothing run");
        return 0;
    }

    /// <summary>The text modules a command defines: its own, or the one an assertion wraps; never a quoted or binary one.</summary>
    private static IEnumerable<SExpr> Modules(SExpr command)
    {
        var module = command.Head is "module" ? command
            : command.Head is "assert_malformed" or "assert_invalid" or "assert_unlinkable" or "assert_trap" && command.Items!.Count > 1 && command.Items[1].Head is "module" ? command.Items[1]
            : null;

        if (module is null || module.Items!.Any(static item => item.IsWord("binary") || item.IsWord("quote")))
        {
            yield break;
        }

        yield return module;
    }

    private static string Verdicts(string revision, List<ScriptCommand> commands)
    {
        var text = new StringBuilder();
        text.Append("# revision ").Append(revision).Append('\n');

        foreach (var command in commands)
        {
            text.Append(command.Id).Append(' ').Append(command.Verdict.ToString().ToLowerInvariant()).Append('\n');
        }

        return text.ToString();
    }

    /// <summary>Fails the run for every command the floor records as passing that does not pass now.</summary>
    private static int CheckFloor(string floor, string revision, List<ScriptCommand> commands)
    {
        var lines = File.ReadAllLines(floor);
        var header = lines.FirstOrDefault(static line => line.StartsWith("# revision ", StringComparison.Ordinal));

        if (header is null || !string.Equals(header["# revision ".Length..], revision, StringComparison.Ordinal))
        {
            Console.WriteLine($"# spec: FLOOR the floor {floor} was not set under revision {revision}, and a floor is never compared across revisions");
            return 1;
        }

        var now = commands.ToDictionary(static command => command.Id, static command => command.Verdict, StringComparer.Ordinal);
        var regressions = 0;
        var held = 0;

        foreach (var line in lines.Where(static line => line.Length > 0 && line[0] != '#'))
        {
            var parts = line.Split(' ');

            if (parts.Length < 2 || !string.Equals(parts[1], "pass", StringComparison.Ordinal))
            {
                continue;
            }

            held++;

            if (!now.TryGetValue(parts[0], out var verdict) || verdict is not ScriptVerdict.Pass)
            {
                Console.WriteLine($"# spec: FLOOR {parts[0]} passes in the floor and not now");
                regressions++;
            }
        }

        Console.WriteLine($"# spec: floor {floor}: {held.ToString(CultureInfo.InvariantCulture)} passing commands held, {regressions.ToString(CultureInfo.InvariantCulture)} regressed");
        return regressions == 0 ? 0 : 1;
    }

    private static string Count(IEnumerable<ScriptCommand> commands, ScriptVerdict verdict) =>
        commands.Count(command => command.Verdict == verdict).ToString(CultureInfo.InvariantCulture);

    private static string? Argument(string[] args, string name)
    {
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (string.Equals(args[index], name, StringComparison.Ordinal))
            {
                return args[index + 1];
            }
        }

        return null;
    }
}
