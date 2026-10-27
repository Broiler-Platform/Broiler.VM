// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;
using System.Diagnostics;
using System.Formats.Tar;
using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The wide path's source session, <c>--fuzz-wide</c>: the seeds a seed file names, read from the
/// parse corpus, mutated as <see cref="SourceFuzzing"/> mutates, and every mutant compiled through
/// <see cref="JsCompiler"/> on the stack it declares (SJL-13's tools commit, JSD-0083 decision D22).
/// </summary>
/// <remarks>
/// <para>
/// <b>Written 2026-10-27, at SJL-13's tools commit, before any change bundle <c>sjl-13-001</c>'s rule
/// judges.</b> No session reached the wide parser before it: the slice session reaches
/// <c>SliceTokenizer</c>, which was the wide path's tokenizer too until step 1 takes the wide path onto
/// the shared syntax library. Part G of the rule runs this session and the slice one at both steps,
/// over the seeds <c>docs/evidence/sjl-13-001/inputs/fuzz-seeds.txt</c> names, and the lane runs the
/// same file from the Native AOT image beside the slice session.
/// </para>
/// <para>
/// <b>The seed file fixes everything a session depends on</b>: the corpus by its manifest's SHA-256,
/// each seed by its identifier, path and SHA-256, its goal, its eval-flag sets, its count of mutants,
/// the mutator's seed and the wall bound. A session is a total function of that file and the corpus,
/// so it decides the same on a runner as on a workstation. Each seed's mutants are drawn from that seed
/// alone, in the file's order, from one mutator seeded once; no mutant becomes a further seed, so the
/// mutants a seed gets do not depend on what another seed's mutants answered.
/// </para>
/// <para>
/// <b>What a finding is</b>, as the seed file and the rule's part G state it: an exception that left the
/// compiler; a refusal with no diagnostic, or with a code the registry does not hold; one compilation
/// longer than the wall bound; and a process that ended without its exit code, which the caller sees
/// and this session cannot. A session reports every finding it meets, retains each mutant beside them
/// (up to a cap), and exits 1 when there was any. An accepted mutant is not verified: the rule's
/// reading of this session ends at the compiler's answer.
/// </para>
/// <para>
/// <b>The wall bound is enforced while a compilation runs, not read after it returns</b>, because a
/// compilation that never returns is the extreme case of one past the bound, and a check made after
/// the return would never be reached. Each compilation runs on a pool thread, which runs the compiler
/// on the stack the compiler declares, and the session waits for it for the wall bound and no longer.
/// A compilation still running at the bound is a finding, reported with its seed, mutant and context
/// and retained like any other, and it ends the session with exit code 1: the thread it runs on cannot
/// be stopped, and left running it would share the machine with every later compilation and move
/// their times. Both threads are background threads, so the process ends when the session returns.
/// </para>
/// </remarks>
internal static class WideFuzzing
{
    /// <summary>The seed file format this session reads, and no other.</summary>
    private const string SeedFormat = "broiler-vm-wide-fuzz-seeds\t1";

    /// <summary>The seed file's column line, exactly.</summary>
    private const string SeedColumns = "session\tid\tgoal\teval-flags\tmutants\tsha256\tpath";

    /// <summary>The parse corpus manifest format the seeds are read from.</summary>
    private const string ManifestFormat = "broiler-parse-corpus\t1";

    /// <summary>The registry, by its path from a checkout's root.</summary>
    private const string RegistryPath = "src/Broiler.VM.Profile.JavaScript/docs/diagnostics/registry.txt";

    /// <summary>How many findings' mutants a session writes out; every finding is still reported.</summary>
    private const int RetainedFindings = 32;

    /// <summary>One wide seed, as the seed file names it.</summary>
    private sealed record Seed(string Id, string Goal, string[] EvalFlags, int Mutants, string Sha256, string Path)
    {
        internal string Text { get; set; } = string.Empty;
    }

    /// <summary>What the seed file fixes for a session.</summary>
    private sealed record SeedFile(
        string Corpus, string CorpusSha256, int WallBoundMs, ulong MutatorSeed, IReadOnlyList<Seed> Seeds);

    /// <summary>One manifest row, as much of it as a seed is checked against.</summary>
    private sealed record ManifestRow(string Origin, string Path, string Sha256, string Goal, bool EvalFamily);

    /// <summary>Runs the session over the arguments that follow <c>--fuzz-wide</c>.</summary>
    internal static int Run(string[] args)
    {
        string? directory = null, seeds = null, archive = null, findings = null, registry = null, journal = null;

        for (var index = 0; index < args.Length; index++)
        {
            var hasValue = index + 1 < args.Length;

            switch (args[index])
            {
                case "--seeds" when hasValue: seeds = args[++index]; break;
                case "--archive" when hasValue: archive = args[++index]; break;
                case "--findings" when hasValue: findings = args[++index]; break;
                case "--registry" when hasValue: registry = args[++index]; break;
                case "--journal" when hasValue: journal = args[++index]; break;
                default:
                    if (args[index].StartsWith("--", StringComparison.Ordinal) || directory is not null)
                    {
                        return Refuse($"`{args[index]}` is not an argument of --fuzz-wide, or lacks its value");
                    }

                    directory = args[index];
                    break;
            }
        }

        if (directory is null || seeds is null)
        {
            return Refuse("--fuzz-wide takes a parse corpus directory and --seeds <file>");
        }

        if (!TryLoad(directory, seeds, archive, registry, out var file, out var registered, out var failure))
        {
            return Refuse(failure);
        }

        var corpusDirectory = Path.GetFullPath(directory);
        findings ??= Path.Combine(
            Path.GetDirectoryName(corpusDirectory) ?? ".",
            Path.GetFileName(corpusDirectory) + "-wide-fuzz-findings");

        return Session(file, registered, findings, journal);
    }

    private static int Refuse(string why)
    {
        Console.WriteLine("broiler-js-wide-fuzz: refused: " + why);
        Console.WriteLine(
            "usage: --fuzz-wide <parse corpus directory> --seeds <seed file> [--archive <test262 archive>] " +
            "[--registry <registry.txt>] [--findings <directory>] [--journal <file>]");
        return 2;
    }

    /// <summary>The session itself, over seeds already read and checked.</summary>
    private static int Session(SeedFile file, IReadOnlySet<int> registered, string findingsDirectory, string? journalPath)
    {
        var mutants = file.Seeds.Sum(static seed => seed.Mutants);
        var compilations = file.Seeds.Sum(static seed => seed.Mutants * (1 + seed.EvalFlags.Length));

        Console.WriteLine(
            $"broiler-js-wide-fuzz: corpus {file.Corpus} {file.CorpusSha256}, mutator seed " +
            $"{file.MutatorSeed.ToString(CultureInfo.InvariantCulture)}, {file.Seeds.Count} seeds, " +
            $"{mutants.ToString(CultureInfo.InvariantCulture)} mutants, " +
            $"{compilations.ToString(CultureInfo.InvariantCulture)} compilations, wall bound " +
            $"{file.WallBoundMs.ToString(CultureInfo.InvariantCulture)} ms; surface: JsCompiler.Compile " +
            "on the stack it declares, under each seed's goal and, for an eval-family seed, as eval code " +
            "under each flag set the file names");

        using var journal = journalPath is null
            ? null
            : new StreamWriter(journalPath, append: false, new UTF8Encoding(false)) { AutoFlush = true };

        var mutator = new SourceMutator(file.MutatorSeed);
        var answers = new Dictionary<string, int>(StringComparer.Ordinal);
        var found = 0;
        var retained = 0;
        var slowest = TimeSpan.Zero;
        var slowestAt = string.Empty;
        var started = Stopwatch.GetTimestamp();
        var seedsAccepted = 0;
        var seedsRefused = new List<string>();

        foreach (var seed in file.Seeds)
        {
            // THE SEED ITSELF, ONCE, UNDER ITS GOAL: a reading of the file, not a mutant. Every wide seed
            // was chosen as one the base accepts, so a refused seed says the corpus moved under it; the
            // line is printed and decides nothing.
            var own = CompileWithin(seed.Text, seed.Goal, string.Empty, file.WallBoundMs);

            if (own is null)
            {
                return PastTheWallBound(
                    "seed " + seed.Id + " " + seed.Goal, seed.Text, $"wide-fuzz-{seed.Id}-seed-{seed.Goal}.js");
            }

            if (own.Accepted)
            {
                seedsAccepted++;
            }
            else
            {
                seedsRefused.Add(seed.Id + " " + own.Answer);
            }

            var accepted = 0;
            var refused = 0;

            for (var number = 0; number < seed.Mutants; number++)
            {
                var mutant = mutator.Next([seed.Text]);

                foreach (var context in Contexts(seed))
                {
                    var key = seed.Id + "\t" + number.ToString(CultureInfo.InvariantCulture) + "\t" + context;
                    journal?.WriteLine("begin\t" + key);
                    var observed = CompileWithin(mutant, seed.Goal, context, file.WallBoundMs);

                    if (observed is null)
                    {
                        journal?.WriteLine("stop\t" + key + "\tpast the wall bound");
                        return PastTheWallBound(
                            key.Replace('\t', ' '),
                            mutant,
                            $"wide-fuzz-{seed.Id}-mutant{number.ToString(CultureInfo.InvariantCulture)}-{context}.js");
                    }

                    journal?.WriteLine("end\t" + key + "\t" + observed.Answer);

                    if (observed.Elapsed > slowest)
                    {
                        slowest = observed.Elapsed;
                        slowestAt = key.Replace('\t', ' ');
                    }

                    var why = WhyItIsAFinding(observed, registered, file.WallBoundMs);

                    if (why.Length != 0)
                    {
                        found++;
                        Console.WriteLine($"broiler-js-wide-fuzz: FINDING at {key.Replace('\t', ' ')}: {why}");

                        if (retained < RetainedFindings)
                        {
                            Directory.CreateDirectory(findingsDirectory);
                            var path = Path.Combine(
                                findingsDirectory,
                                $"wide-fuzz-{seed.Id}-mutant{number.ToString(CultureInfo.InvariantCulture)}-{context}.js");
                            File.WriteAllText(path, mutant);
                            Console.WriteLine("broiler-js-wide-fuzz: retained " + path);
                            retained++;
                        }
                    }

                    if (observed.Accepted)
                    {
                        accepted++;
                    }
                    else
                    {
                        refused++;
                    }

                    answers[observed.Answer] = answers.GetValueOrDefault(observed.Answer) + 1;
                }
            }

            Console.WriteLine(
                $"  {seed.Id} {seed.Goal}{(seed.EvalFlags.Length == 0 ? string.Empty : " and eval code under " + string.Join(',', seed.EvalFlags))}: " +
                $"{seed.Mutants.ToString(CultureInfo.InvariantCulture)} mutants, " +
                $"{accepted.ToString(CultureInfo.InvariantCulture)} accepted, " +
                $"{refused.ToString(CultureInfo.InvariantCulture)} refused");
        }

        Console.WriteLine(
            $"broiler-js-wide-fuzz: the seeds themselves, unmutated: {seedsAccepted.ToString(CultureInfo.InvariantCulture)} " +
            $"of {file.Seeds.Count.ToString(CultureInfo.InvariantCulture)} accepted" +
            (seedsRefused.Count == 0 ? string.Empty : "; refused: " + string.Join("; ", seedsRefused)));

        Console.WriteLine("broiler-js-wide-fuzz: answers over every compilation of a mutant");

        foreach (var (answer, count) in answers
            .OrderByDescending(static entry => entry.Value)
            .ThenBy(static entry => entry.Key, StringComparer.Ordinal))
        {
            Console.WriteLine($"  {count.ToString(CultureInfo.InvariantCulture),8}  {answer}");
        }

        // THE ONLY LINE THAT DEPENDS ON THE MACHINE, and it is the last one, so that every line above it
        // is the same on every run of one file over one corpus.
        Console.WriteLine(
            "broiler-js-wide-fuzz: elapsed " +
            Stopwatch.GetElapsedTime(started).TotalSeconds.ToString("F1", CultureInfo.InvariantCulture) +
            " s; the slowest compilation " +
            slowest.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture) + " ms, at " + slowestAt);

        if (found != 0)
        {
            Console.WriteLine(
                $"broiler-js-wide-fuzz: {found.ToString(CultureInfo.InvariantCulture)} finding(s), " +
                $"{retained.ToString(CultureInfo.InvariantCulture)} retained under {findingsDirectory}");
            return 1;
        }

        Console.WriteLine("broiler-js-wide-fuzz: no finding");
        return 0;

        // A COMPILATION STILL RUNNING AT THE WALL BOUND: reported and retained as every finding is, and
        // the last thing the session does, for the reason the class's remarks give.
        int PastTheWallBound(string at, string text, string name)
        {
            found++;
            Console.WriteLine(
                $"broiler-js-wide-fuzz: FINDING at {at}: the compilation had not returned at the wall bound " +
                $"of {file.WallBoundMs.ToString(CultureInfo.InvariantCulture)} ms");

            if (retained < RetainedFindings)
            {
                Directory.CreateDirectory(findingsDirectory);
                var path = Path.Combine(findingsDirectory, name);
                File.WriteAllText(path, text);
                Console.WriteLine("broiler-js-wide-fuzz: retained " + path);
                retained++;
            }

            Console.WriteLine(
                "broiler-js-wide-fuzz: the session stops at the first compilation past the wall bound, whose " +
                $"thread cannot be stopped; {found.ToString(CultureInfo.InvariantCulture)} finding(s), " +
                $"{retained.ToString(CultureInfo.InvariantCulture)} retained under {findingsDirectory}");
            return 1;
        }
    }

    /// <summary>The compilations one mutant gets: its seed's goal, then each eval-flag set named.</summary>
    private static IEnumerable<string> Contexts(Seed seed)
    {
        yield return seed.Goal;

        foreach (var flags in seed.EvalFlags)
        {
            yield return flags;
        }
    }

    /// <summary>What one compilation answered, and how long it took.</summary>
    private sealed record Observation(
        bool Accepted,
        string Answer,
        IReadOnlyList<SliceSourceDiagnostic> Diagnostics,
        string Escaped,
        TimeSpan Elapsed);

    /// <summary>
    /// The flag values of part B's B3, by name, as the corpus manifest's header defines them; the
    /// manifest is checked to define exactly these before a session starts.
    /// </summary>
    private static readonly (string Name, byte Value)[] EvalFlagSets =
    [
        ("E0", 0), ("E1", 1), ("E2", 2), ("E3", 3), ("E4", 6), ("E5", 14), ("E6", 22), ("E7", 38),
    ];

    /// <summary>
    /// <see cref="Compile"/> on a pool thread, waited for for the wall bound: its observation, or
    /// <see langword="null"/> when the compilation had not returned by then.
    /// </summary>
    private static Observation? CompileWithin(string text, string goal, string context, int wallBoundMs)
    {
        var running = Task.Run(() => Compile(text, goal, context));
        return running.Wait(wallBoundMs) ? running.Result : null;
    }

    /// <summary>
    /// Compiles one text through <see cref="JsCompiler"/>: a script, a module, or eval code under a
    /// named flag set. It never throws: an exception that leaves the compiler is the observation.
    /// </summary>
    /// <remarks>
    /// <b>The units are named as part B names them</b>: a script is one unit named <c>test</c>, sloppy,
    /// its text unchanged; a module is one unit alone, its requests left unresolved, which the
    /// compilation carries for a link to refuse; eval code is one unit named <c>eval</c> under the flag
    /// set. <see cref="OutOfMemoryException"/> is not caught: it says nothing about the front end.
    /// </remarks>
    private static Observation Compile(string text, string goal, string context)
    {
        var started = Stopwatch.GetTimestamp();

        try
        {
            var request = new JsCompileRequest(JsFeatureManifest.Wide, JsOutputForm.Bytecode);
            JsCompilation compiled;

            if (context.Length != 0 && context != goal)
            {
                var flags = EvalFlagSets.First(set => set.Name == context).Value;
                compiled = JsCompiler.Compile(
                    [new JsScriptUnit("eval", text, SliceParseOptions.Eval((JsFormat.EvalRequestFlags)flags))],
                    [],
                    request);
            }
            else if (goal == "module")
            {
                compiled = JsCompiler.Compile(
                    [], [new JsModuleUnit("/parse-1/wide-fuzz.mjs", text, SliceParseOptions.Module)], request);
            }
            else
            {
                compiled = JsCompiler.Compile(
                    [new JsScriptUnit("test", text, SliceParseOptions.Script)], [], request);
            }

            var elapsed = Stopwatch.GetElapsedTime(started);
            var accepted = compiled.Succeeded && compiled.Artifact is not null;

            return new Observation(
                accepted,
                accepted
                    ? "accepted"
                    : "refused:" + (compiled.Diagnostics.Count == 0 ? "(no diagnostic)" : compiled.Diagnostics[0].Code.ToString()),
                compiled.Diagnostics,
                string.Empty,
                elapsed);
        }
        catch (Exception failure) when (failure is not OutOfMemoryException)
        {
            return new Observation(
                false,
                "escaped:" + failure.GetType().Name,
                [],
                failure.GetType().FullName + ": " + failure.Message,
                Stopwatch.GetElapsedTime(started));
        }
    }

    /// <summary>Why an observation is a finding, or empty when it is an answer.</summary>
    private static string WhyItIsAFinding(Observation observed, IReadOnlySet<int> registered, int wallBoundMs)
    {
        if (observed.Escaped.Length != 0)
        {
            return "the compiler threw instead of answering: " + observed.Escaped;
        }

        if (observed.Elapsed.TotalMilliseconds > wallBoundMs)
        {
            return $"the compilation took {observed.Elapsed.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture)} ms, " +
                $"past the wall bound of {wallBoundMs.ToString(CultureInfo.InvariantCulture)} ms";
        }

        if (observed.Accepted)
        {
            return string.Empty;
        }

        if (observed.Diagnostics.Count == 0)
        {
            return "the compiler refused with no diagnostic";
        }

        foreach (var diagnostic in observed.Diagnostics)
        {
            if (!registered.Contains((int)diagnostic.Code))
            {
                return $"a refusal with {(int)diagnostic.Code}, a code the registry does not hold: {diagnostic}";
            }
        }

        return string.Empty;
    }

    /// <summary>Reads and checks the seed file, the corpus manifest, every seed and the registry.</summary>
    private static bool TryLoad(
        string directory,
        string seedsPath,
        string? archivePath,
        string? registryPath,
        out SeedFile file,
        out IReadOnlySet<int> registered,
        out string failure)
    {
        file = null!;
        registered = null!;

        if (!File.Exists(seedsPath))
        {
            failure = "no seed file at " + seedsPath;
            return false;
        }

        if (!TryParseSeeds(File.ReadAllBytes(seedsPath), out file, out failure))
        {
            failure = seedsPath + ": " + failure;
            return false;
        }

        var manifestPath = Path.Combine(directory, "corpus.manifest");

        if (!File.Exists(manifestPath))
        {
            failure = "no corpus manifest at " + manifestPath;
            return false;
        }

        var manifestBytes = File.ReadAllBytes(manifestPath);
        var manifestSha256 = Sha256(manifestBytes);

        if (!string.Equals(manifestSha256, file.CorpusSha256, StringComparison.Ordinal))
        {
            failure = $"the corpus manifest hashes to {manifestSha256}, and the seed file names {file.CorpusSha256}";
            return false;
        }

        if (!TryParseManifest(manifestBytes, file.Corpus, out var rows, out var archive, out failure))
        {
            failure = manifestPath + ": " + failure;
            return false;
        }

        foreach (var seed in file.Seeds)
        {
            if (!rows.TryGetValue(seed.Id, out var row))
            {
                failure = $"{seed.Id}: the corpus manifest has no such input";
                return false;
            }

            if (row.Path != seed.Path || row.Sha256 != seed.Sha256 || row.Goal != seed.Goal)
            {
                failure = $"{seed.Id}: the seed file names {seed.Goal} {seed.Path} {seed.Sha256}, and the manifest " +
                    $"{row.Goal} {row.Path} {row.Sha256}";
                return false;
            }

            if (seed.EvalFlags.Length != 0 && !row.EvalFamily)
            {
                failure = $"{seed.Id}: eval-flag sets are named for an input the manifest does not mark eval-family";
                return false;
            }
        }

        var corpusDirectory = Path.GetFullPath(directory);
        var wanted = file.Seeds.Where(seed => rows[seed.Id].Origin == "test262")
            .Select(static seed => seed.Path).ToHashSet(StringComparer.Ordinal);
        var suite = new Dictionary<string, byte[]>(StringComparer.Ordinal);

        if (wanted.Count != 0)
        {
            var found = archivePath ?? FindUpwards(corpusDirectory, archive.Path);

            if (found is null || !File.Exists(found))
            {
                failure = "no test262 archive at " + (found ?? archive.Path) +
                    "; name it with --archive, or run from a checkout that holds it";
                return false;
            }

            var archiveSha256 = Sha256(File.ReadAllBytes(found));

            if (archiveSha256 != archive.Sha256)
            {
                failure = $"the archive at {found} hashes to {archiveSha256}, and the manifest pins {archive.Sha256}";
                return false;
            }

            ReadFromArchive(found, wanted, suite);
        }

        foreach (var seed in file.Seeds)
        {
            byte[] bytes;

            if (rows[seed.Id].Origin == "test262")
            {
                if (!suite.TryGetValue(seed.Path, out bytes!))
                {
                    failure = $"{seed.Id}: the archive holds no {seed.Path}";
                    return false;
                }
            }
            else
            {
                var stored = Path.Combine(corpusDirectory, seed.Path.Replace('/', Path.DirectorySeparatorChar));

                if (!File.Exists(stored))
                {
                    failure = $"{seed.Id}: no stored input at {seed.Path}";
                    return false;
                }

                bytes = File.ReadAllBytes(stored);
            }

            if (Sha256(bytes) != seed.Sha256)
            {
                failure = $"{seed.Id}: {seed.Path} does not hash to {seed.Sha256}";
                return false;
            }

            seed.Text = Decode(bytes);
        }

        var registryFile = registryPath ?? FindUpwards(corpusDirectory, RegistryPath);

        if (registryFile is null || !File.Exists(registryFile))
        {
            failure = "no registry at " + (registryFile ?? RegistryPath) +
                "; name it with --registry, or run from a checkout that holds it";
            return false;
        }

        var codes = new HashSet<int>();

        foreach (var line in File.ReadAllLines(registryFile))
        {
            var cells = line.Split('|');

            if (!line.StartsWith('#') && cells.Length == 8 && cells[2] == "embedder-seam" &&
                int.TryParse(cells[0], NumberStyles.None, CultureInfo.InvariantCulture, out var code))
            {
                codes.Add(code);
            }
        }

        if (codes.Count == 0)
        {
            failure = registryFile + " holds no embedder-seam code";
            return false;
        }

        registered = codes;
        failure = string.Empty;
        return true;
    }

    /// <summary>Parses format 1 of the seed file, refusing anything it does not exactly hold.</summary>
    private static bool TryParseSeeds(byte[] bytes, out SeedFile file, out string failure)
    {
        file = null!;

        if (!TryText(bytes, out var lines, out failure))
        {
            return false;
        }

        var fields = Header(lines, out var at);

        if (!fields.TryGetValue("format", out var format) || format != SeedFormat)
        {
            failure = "the seed file is not format `" + SeedFormat.Replace('\t', ' ') + "`";
            return false;
        }

        var corpus = fields.GetValueOrDefault("corpus", string.Empty).Split('\t');

        if (corpus.Length != 2 || corpus[1].Length != 64 || !IsHex(corpus[1]))
        {
            failure = "the seed file names no corpus and its manifest's SHA-256";
            return false;
        }

        if (!int.TryParse(fields.GetValueOrDefault("wall-bound-ms"), NumberStyles.None, CultureInfo.InvariantCulture, out var wall) ||
            wall < 1)
        {
            failure = "the seed file states no wall bound";
            return false;
        }

        if (!ulong.TryParse(fields.GetValueOrDefault("wide-mutator-seed"), NumberStyles.None, CultureInfo.InvariantCulture, out var mutatorSeed))
        {
            failure = "the seed file states no wide-mutator-seed";
            return false;
        }

        if (at >= lines.Length || lines[at] != SeedColumns)
        {
            failure = "the seed file's column line is not the one format 1 fixes";
            return false;
        }

        var seeds = new List<Seed>();
        var ids = new HashSet<string>(StringComparer.Ordinal);

        for (at++; at < lines.Length; at++)
        {
            var cells = lines[at].Split('\t');

            if (cells.Length != 7 || cells.Any(static cell => cell.Length == 0))
            {
                failure = $"line {at + 1} is not seven non-empty fields";
                return false;
            }

            if (cells[0] == "slice")
            {
                // The slice session's lines are --fuzz's (its --seed and its --iterations), not this mode's.
                continue;
            }

            if (cells[0] != "wide" || cells[1].Length == 0 || !ids.Add(cells[1]))
            {
                failure = $"line {at + 1}: the session is neither wide nor slice, or the id repeats";
                return false;
            }

            var flags = cells[3] == "-" ? [] : cells[3].Split(',');

            if (cells[2] is not ("script" or "module") ||
                flags.Any(name => EvalFlagSets.All(set => set.Name != name)) ||
                flags.Distinct(StringComparer.Ordinal).Count() != flags.Length ||
                (flags.Length != 0 && cells[2] != "script") ||
                !int.TryParse(cells[4], NumberStyles.None, CultureInfo.InvariantCulture, out var count) || count < 1 ||
                cells[5].Length != 64 || !IsHex(cells[5]))
            {
                failure = $"line {at + 1}: the goal, the eval-flag sets, the count of mutants or the SHA-256 is not " +
                    "one format 1 admits";
                return false;
            }

            seeds.Add(new Seed(cells[1], cells[2], flags, count, cells[5], cells[6]));
        }

        if (seeds.Count == 0)
        {
            failure = "the seed file names no wide seed";
            return false;
        }

        file = new SeedFile(corpus[0], corpus[1], wall, mutatorSeed, seeds);
        failure = string.Empty;
        return true;
    }

    /// <summary>
    /// Reads the corpus manifest as much as a seed is checked against: its rows by identifier, and the
    /// archive it pins.
    /// </summary>
    private static bool TryParseManifest(
        byte[] bytes,
        string corpus,
        out Dictionary<string, ManifestRow> rows,
        out (string Path, string Sha256) archive,
        out string failure)
    {
        rows = new Dictionary<string, ManifestRow>(StringComparer.Ordinal);
        archive = default;

        if (!TryText(bytes, out var lines, out failure))
        {
            return false;
        }

        var flags = new List<(string Name, byte Value)>();

        foreach (var line in lines.TakeWhile(static line => line.StartsWith('#')))
        {
            if (line.StartsWith("#% eval-flags\t", StringComparison.Ordinal))
            {
                var parts = line.Split('\t');

                if (parts.Length >= 3 && byte.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out var value))
                {
                    flags.Add((parts[1], value));
                }
            }
        }

        var fields = Header(lines, out var at);

        if (fields.GetValueOrDefault("format") != ManifestFormat || fields.GetValueOrDefault("corpus") != corpus)
        {
            failure = $"the manifest is not format `{ManifestFormat.Replace('\t', ' ')}` of corpus {corpus}";
            return false;
        }

        // THE FLAG SETS ARE READ HERE AND CHECKED AGAINST THE EIGHT THIS MODE COMPILES UNDER, so that a
        // manifest defining E4 as another set cannot have its eval-family seeds compiled under this one.
        if (!flags.SequenceEqual(EvalFlagSets))
        {
            failure = "the manifest's eval flag sets are not the eight part B's B3 names";
            return false;
        }

        var pin = fields.GetValueOrDefault("pin-test262-archive", string.Empty).Split('\t');

        if (pin.Length != 2)
        {
            failure = "the manifest pins no test262 archive";
            return false;
        }

        archive = (pin[0], pin[1]);

        for (at++; at < lines.Length; at++)
        {
            var cells = lines[at].Split('\t');

            if (cells.Length != 9)
            {
                failure = $"line {at + 1} is not nine fields";
                return false;
            }

            rows[cells[0]] = new ManifestRow(cells[1], cells[2], cells[3], cells[4], cells[7] == "yes");
        }

        failure = string.Empty;
        return true;
    }

    /// <summary>The <c>#% key\tvalue</c> header lines, and where the header ends.</summary>
    private static Dictionary<string, string> Header(string[] lines, out int end)
    {
        var fields = new Dictionary<string, string>(StringComparer.Ordinal);
        end = 0;

        for (; end < lines.Length && lines[end].StartsWith('#'); end++)
        {
            if (lines[end].StartsWith("#% ", StringComparison.Ordinal))
            {
                var tab = lines[end].IndexOf('\t', StringComparison.Ordinal);

                if (tab > 3)
                {
                    fields.TryAdd(lines[end][3..tab], lines[end][(tab + 1)..]);
                }
            }
        }

        return fields;
    }

    /// <summary>UTF-8 without a byte-order mark, LF line ends, the last line ended.</summary>
    private static bool TryText(byte[] bytes, out string[] lines, out string failure)
    {
        lines = [];

        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
        {
            failure = "it begins with a byte-order mark";
            return false;
        }

        string text;

        try
        {
            text = new UTF8Encoding(false, true).GetString(bytes);
        }
        catch (DecoderFallbackException)
        {
            failure = "it is not UTF-8";
            return false;
        }

        if (text.Contains('\r', StringComparison.Ordinal) || !text.EndsWith('\n'))
        {
            failure = "its lines do not all end in LF";
            return false;
        }

        lines = text[..^1].Split('\n');
        failure = string.Empty;
        return true;
    }

    /// <summary>Reads the named suite files out of the archive, by their path under its top directory.</summary>
    private static void ReadFromArchive(string archive, IReadOnlySet<string> wanted, Dictionary<string, byte[]> files)
    {
        using var stream = File.OpenRead(archive);
        using var gzip = new GZipStream(stream, CompressionMode.Decompress);
        using var reader = new TarReader(gzip);

        while (files.Count < wanted.Count && reader.GetNextEntry() is { } entry)
        {
            if (entry.EntryType is not (TarEntryType.RegularFile or TarEntryType.V7RegularFile) ||
                entry.DataStream is null)
            {
                continue;
            }

            var slash = entry.Name.IndexOf('/', StringComparison.Ordinal);

            if (slash < 0 || !wanted.Contains(entry.Name[(slash + 1)..]))
            {
                continue;
            }

            using var copy = new MemoryStream();
            entry.DataStream.CopyTo(copy);
            files[entry.Name[(slash + 1)..]] = copy.ToArray();
        }
    }

    /// <summary>Finds a checkout-relative path by walking up from a directory.</summary>
    private static string? FindUpwards(string start, string relative)
    {
        for (var directory = new DirectoryInfo(start); directory is not null; directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, relative.Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    /// <summary>Decodes a file as the conformance root does: UTF-8, a leading byte-order mark dropped.</summary>
    private static string Decode(byte[] bytes)
    {
        var start = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? 3 : 0;
        return Encoding.UTF8.GetString(bytes, start, bytes.Length - start);
    }

    private static bool IsHex(string text) => text.All(static c => c is (>= '0' and <= '9') or (>= 'a' and <= 'f'));

    private static string Sha256(byte[] bytes) => Convert.ToHexStringLower(SHA256.HashData(bytes));
}
