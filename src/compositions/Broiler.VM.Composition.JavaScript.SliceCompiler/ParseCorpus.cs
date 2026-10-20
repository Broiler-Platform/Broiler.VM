// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;
using System.Formats.Tar;
using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The parse corpus's identity run: every input of a parse-corpus manifest, compiled in the four
/// sub-populations bundle sjl-12-001's rule names, with each compilation's outcome, its ordered
/// diagnostics and, for an accepted input, the SHA-256 and length of its artifact.
/// </summary>
/// <remarks>
/// <para>
/// <b>Written 2026-10-20 at SJL-12.4, before any change the rule judges.</b> The rule is
/// <c>docs/evidence/sjl-12-001/decision-rule-area.md</c>, parts B and C; the manifest's format is
/// fixed by the corpus's own README (format <c>broiler-parse-corpus 1</c>). The rule requires this
/// file to be unchanged between the corpus commit and the after commit, so everything a run's
/// answer depends on - which units a compilation holds, in which order, under which names - is
/// decided here and nowhere else, and the conformance root's choices are restated rather than
/// referenced (rule N13 forbids the reference, and a restatement cannot move under a run).
/// </para>
/// <para>
/// <b>The four sub-populations.</b>
/// B1, each input alone under its goal and each of its variants, in the bytecode form and in the
/// native form under <c>x86-64-sysv</c>: a script is one unit named <c>test</c> whose referrer is
/// its fixed name, strict by the unit's <c>ForceStrict</c> with its text unchanged; a module is the
/// graph the conformance root's resolution reaches from it; a JSON input is a JSON module.
/// B2, each test262 input as the conformance root composes it - <c>assert.js</c>, <c>sta.js</c>,
/// <c>doneprintHandle.js</c> for an asynchronous test and the test's includes, in that order, unless
/// the variant is raw, and then the test - in the bytecode form.
/// B3, each eval-family input compiled as eval code under each of the eight request-flag sets the
/// manifest's header lists, in the bytecode form.
/// B4, each input compiled by the slice front end, strict by the strict prologue in front of the
/// text, because the slice front end has no other way to be told.
/// </para>
/// <para>
/// <b>The names are fixed, and that is why they are virtual.</b> A module's key and a script's
/// referrer reach the artifact's bytes, so every test262 input is named under the manifest's
/// virtual root and every stored input under its stored root, and the suite is read straight out of
/// the pinned archive rather than from wherever it was extracted.
/// </para>
/// <para>
/// <b>It judges nothing.</b> It writes a report; <c>eng/compare-parse-corpus.py</c> joins two of them.
/// </para>
/// </remarks>
internal static class ParseCorpus
{
    /// <summary>The manifest format this mode reads, and no other.</summary>
    private const string ManifestFormat = "broiler-parse-corpus\t1";

    /// <summary>The manifest's column line, exactly.</summary>
    private const string ManifestColumns =
        "id\torigin\tpath\tsha256\tgoal\tvariants\tnegative\teval-family\tprovenance";

    /// <summary>The report format this mode writes.</summary>
    private const string ReportFormat = "broiler-vm-parse-corpus-report\t1";

    /// <summary>
    /// The conformance root's strict prologue (<c>Test262Adapter.StrictPrologue</c>), restated: B4
    /// composes its strict variant with it, and a manifest naming another one is refused.
    /// </summary>
    private const string StrictPrologue = "\"use strict\";\n";

    /// <summary>The backend B1's native form is emitted by, as the rule names it.</summary>
    private const string NativeBackend = JsNativeBackends.X64SystemV;

    /// <summary>The conformance root's cap on the modules one graph may reach (Test262Modules).</summary>
    private const int MaximumModules = 256;

    /// <summary>
    /// The stack each worker runs on. The module walk asks <see cref="JsCompiler.Requests"/> on the
    /// caller's stack, which is the one place a compilation of this run does not get the compiler's
    /// own declared stack; this one is larger than that, so a deep module cannot end the process.
    /// </summary>
    private const int WorkerStackBytes = 64 * 1024 * 1024;

    /// <summary>The four sub-populations, in report order.</summary>
    private static readonly string[] Populations = ["B1", "B2", "B3", "B4"];

    /// <summary>The flag sets of part B's B3, by name and value, as the rule fixes them.</summary>
    private static readonly (string Name, byte Value)[] RuleEvalFlags =
    [
        ("E0", 0), ("E1", 1), ("E2", 2), ("E3", 3), ("E4", 6), ("E5", 14), ("E6", 22), ("E7", 38),
    ];

    /// <summary>One row of the manifest.</summary>
    private sealed record Row(
        int Index,
        string Id,
        string Origin,
        string Path,
        string Sha256,
        string Goal,
        string[] Variants,
        bool EvalFamily)
    {
        /// <summary>Whether the input is one of the suite's files.</summary>
        internal bool IsTest262 => string.Equals(Origin, "test262", StringComparison.Ordinal);
    }

    /// <summary>What the manifest's header fixes.</summary>
    private sealed record Header(
        string Corpus,
        int Rows,
        string VirtualRoot,
        string StoredRoot,
        string StrictPrologue,
        (string Name, byte Value)[] EvalFlags,
        string ArchivePath,
        string ArchiveSha256);

    /// <summary>One compilation this run reports.</summary>
    private readonly record struct Item(int Row, string Population, string Variant, string Form)
    {
        internal string Key(IReadOnlyList<Row> rows) =>
            rows[Row].Id + "\t" + Population + "\t" + Variant + "\t" + Form;
    }

    /// <summary>What one compilation answered.</summary>
    /// <param name="Outcome">
    /// <c>accepted</c>, <c>refused</c>, <c>crashed</c> (an exception left the compiler), or
    /// <c>not-compiled</c> (the sub-population gives the input no compilation, and the note says why).
    /// </param>
    private sealed record Result(
        string Outcome, string Diagnostics, string Sha256, string Length, string Note)
    {
        internal string Columns => Outcome + "\t" + Diagnostics + "\t" + Sha256 + "\t" + Length + "\t" + Note;
    }

    /// <summary>The units of one compilation, or why the sub-population gives the input none.</summary>
    private sealed record Units(
        IReadOnlyList<JsScriptUnit> Scripts, IReadOnlyList<JsModuleUnit> Modules, string Failure);

    /// <summary>A module graph, or why the conformance root would not have compiled one.</summary>
    private sealed record Graph(IReadOnlyList<JsModuleUnit> Modules, string Failure);

    /// <summary>The front matter the conformance root reads, as much of it as composition needs.</summary>
    private sealed record Frontmatter(IReadOnlyList<string> Flags, IReadOnlyList<string> Includes);

    /// <summary>Everything one run reads, checked, and nothing it writes.</summary>
    private sealed class Inputs
    {
        internal required IReadOnlyList<Row> Rows { get; init; }

        internal required Header Header { get; init; }

        /// <summary>Each row's text, decoded as the conformance root decodes a file.</summary>
        internal required string[] Texts { get; init; }

        /// <summary>The suite's files under <c>test/</c> and <c>harness/</c>, by suite-relative path.</summary>
        internal required IReadOnlyDictionary<string, byte[]> Suite { get; init; }

        /// <summary>The stored inputs, by corpus-relative path, already checked against the manifest.</summary>
        internal required IReadOnlyDictionary<string, byte[]> Stored { get; init; }

        /// <summary>The fixed name of a row: the virtual root or the stored root, then its path.</summary>
        internal string NameOf(Row row) => (row.IsTest262 ? Header.VirtualRoot : Header.StoredRoot) + row.Path;

        /// <summary>
        /// Reads a fixed name as the conformance root reads a path, or answers false: a name under
        /// the virtual root is a file of the archived suite, a name under the stored root is a stored
        /// input the manifest lists, and no other name is a file.
        /// </summary>
        internal bool TryRead(string name, out string text)
        {
            var bytes = Bytes(name);
            text = bytes is null ? string.Empty : Decode(bytes);
            return bytes is not null;
        }

        /// <summary>Whether a fixed name is a file, in the sense <see cref="TryRead"/> gives it.</summary>
        internal bool Exists(string name) => Bytes(name) is not null;

        private byte[]? Bytes(string name)
        {
            byte[]? bytes = null;

            if (name.StartsWith(Header.VirtualRoot, StringComparison.Ordinal))
            {
                Suite.TryGetValue(name[Header.VirtualRoot.Length..], out bytes);
            }
            else if (name.StartsWith(Header.StoredRoot, StringComparison.Ordinal))
            {
                Stored.TryGetValue(name[Header.StoredRoot.Length..], out bytes);
            }

            return bytes;
        }
    }

    /// <summary>Runs the mode over the arguments that follow <c>--parse-corpus</c>.</summary>
    internal static int Run(string[] args)
    {
        if (!TryParseArguments(args, out var options, out var failure))
        {
            Console.WriteLine("broiler-js-slice-compiler: " + failure);
            Console.WriteLine(
                "usage: --parse-corpus <manifest> <report> [--workers <n>] [--twice-concurrent] " +
                "[--write-bytes <directory>] [--populations B1,B2,B3,B4] [--select <keys file>] " +
                "[--archive <test262 archive>]");
            return 2;
        }

        var started = System.Diagnostics.Stopwatch.GetTimestamp();

        if (!TryLoad(options, out var inputs, out var manifestSha256, out var archiveSha256, out failure))
        {
            Console.WriteLine("broiler-js-slice-compiler: --parse-corpus refused: " + failure);
            return 2;
        }

        var items = Plan(inputs, options.Populations);
        var selection = "all";

        if (options.Select is not null)
        {
            if (!TrySelect(options.Select, inputs.Rows, ref items, out selection, out failure))
            {
                Console.WriteLine("broiler-js-slice-compiler: --parse-corpus refused: " + failure);
                return 2;
            }
        }

        if (options.WriteBytes is not null)
        {
            Directory.CreateDirectory(options.WriteBytes);
        }

        var first = new Result[items.Count];
        var second = options.TwiceConcurrent ? new Result[items.Count] : null;

        Execute(inputs, items, options, first, second);

        WriteReport(
            options.Report, inputs, items, first, second, manifestSha256, archiveSha256, options, selection);

        var elapsed = System.Diagnostics.Stopwatch.GetElapsedTime(started);
        var crashed = first.Count(result => result.Outcome == "crashed") +
            (second?.Count(result => result.Outcome == "crashed") ?? 0);
        var disagreements = second is null
            ? 0
            : Enumerable.Range(0, items.Count).Count(index => first[index].Columns != second[index].Columns);

        foreach (var population in Populations)
        {
            var counts = Enumerable.Range(0, items.Count)
                .Where(index => items[index].Population == population)
                .GroupBy(index => first[index].Outcome, StringComparer.Ordinal)
                .OrderBy(group => group.Key, StringComparer.Ordinal)
                .Select(group => group.Key + " " + group.Count().ToString(CultureInfo.InvariantCulture));

            var line = string.Join(", ", counts);

            if (line.Length != 0)
            {
                Console.WriteLine($"broiler-js-slice-compiler: {population}: {line}");
            }
        }

        Console.WriteLine(
            $"broiler-js-slice-compiler: --parse-corpus reported {items.Count} compilations" +
            (second is null ? string.Empty : " twice, concurrently") +
            $" of {inputs.Rows.Count} inputs with {options.Workers} worker(s) in " +
            elapsed.TotalSeconds.ToString("F1", CultureInfo.InvariantCulture) + " s; " +
            $"{crashed} crashed" +
            (second is null ? string.Empty : $"; {disagreements} where the two compilations differ"));

        return 0;
    }

    /// <summary>The parsed command line.</summary>
    private sealed record Options(
        string Manifest,
        string Report,
        int Workers,
        bool TwiceConcurrent,
        string? WriteBytes,
        IReadOnlySet<string> Populations,
        string? Select,
        string? Archive);

    private static bool TryParseArguments(string[] args, out Options options, out string failure)
    {
        options = null!;
        var positional = new List<string>();
        var workers = 0;
        var twice = false;
        string? writeBytes = null;
        string? select = null;
        string? archive = null;
        var populations = new HashSet<string>(Populations, StringComparer.Ordinal);

        for (var index = 0; index < args.Length; index++)
        {
            var argument = args[index];
            var hasValue = index + 1 < args.Length;

            switch (argument)
            {
                case "--workers" when hasValue:
                    if (!int.TryParse(
                            args[++index], NumberStyles.None, CultureInfo.InvariantCulture, out workers) ||
                        workers < 1 || workers > 256)
                    {
                        failure = "--workers takes a count from 1 to 256";
                        return false;
                    }

                    break;
                case "--twice-concurrent":
                    twice = true;
                    break;
                case "--write-bytes" when hasValue:
                    writeBytes = args[++index];
                    break;
                case "--select" when hasValue:
                    select = args[++index];
                    break;
                case "--archive" when hasValue:
                    archive = args[++index];
                    break;
                case "--populations" when hasValue:
                    populations = new HashSet<string>(
                        args[++index].Split(
                            ',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                        StringComparer.Ordinal);

                    if (populations.Count == 0 || !populations.IsSubsetOf(Populations))
                    {
                        failure = "--populations takes a comma-separated subset of B1, B2, B3 and B4";
                        return false;
                    }

                    break;
                default:
                    if (argument.StartsWith("--", StringComparison.Ordinal))
                    {
                        failure = $"`{argument}` is not an option of --parse-corpus, or lacks its value";
                        return false;
                    }

                    positional.Add(argument);
                    break;
            }
        }

        if (positional.Count != 2)
        {
            failure = "--parse-corpus takes a manifest and a report path";
            return false;
        }

        // RUN 3 OF PART C IS TWO WORKERS, EACH COMPILING EVERY INPUT, so the mode fixes the count
        // rather than accepting a third reading of "twice concurrently".
        if (twice && workers is not (0 or 2))
        {
            failure = "--twice-concurrent runs exactly two workers, one compilation of each input on each";
            return false;
        }

        options = new Options(
            positional[0],
            positional[1],
            twice ? 2 : Math.Max(workers, 1),
            twice,
            writeBytes,
            populations,
            select,
            archive);

        failure = string.Empty;
        return true;
    }

    /// <summary>Reads and checks the manifest, the archive and every input, or says why not.</summary>
    private static bool TryLoad(
        Options options,
        out Inputs inputs,
        out string manifestSha256,
        out string archiveSha256,
        out string failure)
    {
        inputs = null!;
        manifestSha256 = string.Empty;
        archiveSha256 = string.Empty;

        if (!File.Exists(options.Manifest))
        {
            failure = "no manifest at " + options.Manifest;
            return false;
        }

        var manifestBytes = File.ReadAllBytes(options.Manifest);
        manifestSha256 = Sha256(manifestBytes);

        if (!TryParseManifest(manifestBytes, out var header, out var rows, out failure))
        {
            return false;
        }

        var corpusDirectory = Path.GetDirectoryName(Path.GetFullPath(options.Manifest)) ?? ".";
        var texts = new string[rows.Count];
        var stored = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        var mismatches = new List<string>();

        foreach (var row in rows.Where(row => !row.IsTest262))
        {
            var file = Path.Combine(corpusDirectory, row.Path.Replace('/', Path.DirectorySeparatorChar));

            if (!File.Exists(file))
            {
                failure = $"{row.Id}: no stored input at {row.Path}";
                return false;
            }

            var bytes = File.ReadAllBytes(file);

            if (!string.Equals(Sha256(bytes), row.Sha256, StringComparison.Ordinal))
            {
                mismatches.Add(row.Id + " " + row.Path);
            }

            stored[row.Path] = bytes;
            texts[row.Index] = Decode(bytes);
        }

        IReadOnlyDictionary<string, byte[]> suite = new Dictionary<string, byte[]>(StringComparer.Ordinal);

        if (rows.Any(row => row.IsTest262))
        {
            var archive = options.Archive ?? FindArchive(corpusDirectory, header.ArchivePath);

            if (archive is null || !File.Exists(archive))
            {
                failure = "no test262 archive at " + (archive ?? header.ArchivePath) +
                    "; name it with --archive, or run from a checkout that holds it";
                return false;
            }

            archiveSha256 = Sha256(File.ReadAllBytes(archive));

            if (!string.Equals(archiveSha256, header.ArchiveSha256, StringComparison.Ordinal))
            {
                failure = $"the archive at {archive} hashes to {archiveSha256}, and the manifest pins " +
                    header.ArchiveSha256;
                return false;
            }

            suite = ReadSuite(archive);

            foreach (var row in rows.Where(row => row.IsTest262))
            {
                if (!suite.TryGetValue(row.Path, out var bytes))
                {
                    failure = $"{row.Id}: the archive holds no {row.Path}";
                    return false;
                }

                if (!string.Equals(Sha256(bytes), row.Sha256, StringComparison.Ordinal))
                {
                    mismatches.Add(row.Id + " " + row.Path);
                }

                texts[row.Index] = Decode(bytes);
            }
        }

        if (mismatches.Count != 0)
        {
            failure = $"{mismatches.Count} input(s) do not hash to the manifest's SHA-256, the first " +
                string.Join(", ", mismatches.Take(5));
            return false;
        }

        inputs = new Inputs
        {
            Rows = rows,
            Header = header,
            Texts = texts,
            Suite = suite,
            Stored = stored,
        };

        failure = string.Empty;
        return true;
    }

    /// <summary>
    /// Finds the archive the manifest names by its repository path, walking up from the corpus.
    /// </summary>
    private static string? FindArchive(string corpusDirectory, string repositoryPath)
    {
        var relative = repositoryPath.Replace('/', Path.DirectorySeparatorChar);

        for (var directory = new DirectoryInfo(corpusDirectory); directory is not null; directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, relative);

            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    /// <summary>
    /// Reads the suite's <c>test/</c> and <c>harness/</c> files out of the archive, keyed by their
    /// path under the archive's one top-level directory.
    /// </summary>
    private static Dictionary<string, byte[]> ReadSuite(string archive)
    {
        var files = new Dictionary<string, byte[]>(StringComparer.Ordinal);

        using var stream = File.OpenRead(archive);
        using var gzip = new GZipStream(stream, CompressionMode.Decompress);
        using var reader = new TarReader(gzip);

        while (reader.GetNextEntry() is { } entry)
        {
            if (entry.EntryType is not (TarEntryType.RegularFile or TarEntryType.V7RegularFile) ||
                entry.DataStream is null)
            {
                continue;
            }

            var slash = entry.Name.IndexOf('/', StringComparison.Ordinal);

            if (slash < 0)
            {
                continue;
            }

            var path = entry.Name[(slash + 1)..];

            if (!path.StartsWith("test/", StringComparison.Ordinal) &&
                !path.StartsWith("harness/", StringComparison.Ordinal))
            {
                continue;
            }

            using var copy = new MemoryStream();
            entry.DataStream.CopyTo(copy);
            files[path] = copy.ToArray();
        }

        return files;
    }

    /// <summary>Parses format 1 of the manifest, refusing anything it does not exactly hold.</summary>
    private static bool TryParseManifest(
        byte[] bytes, out Header header, out List<Row> rows, out string failure)
    {
        header = null!;
        rows = [];

        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
        {
            failure = "the manifest begins with a byte-order mark";
            return false;
        }

        string text;

        try
        {
            text = new UTF8Encoding(false, true).GetString(bytes);
        }
        catch (DecoderFallbackException)
        {
            failure = "the manifest is not UTF-8";
            return false;
        }

        if (text.Contains('\r', StringComparison.Ordinal) || !text.EndsWith('\n'))
        {
            failure = "the manifest's lines do not all end in LF";
            return false;
        }

        var lines = text[..^1].Split('\n');
        var fields = new Dictionary<string, List<string[]>>(StringComparer.Ordinal);
        var at = 0;

        for (; at < lines.Length && lines[at].StartsWith('#'); at++)
        {
            if (lines[at].StartsWith("#% ", StringComparison.Ordinal))
            {
                var parts = lines[at][3..].Split('\t');

                if (!fields.TryGetValue(parts[0], out var values))
                {
                    fields[parts[0]] = values = [];
                }

                values.Add(parts[1..]);
            }
        }

        string Single(string key) =>
            fields.TryGetValue(key, out var values) && values.Count == 1
                ? string.Join('\t', values[0])
                : string.Empty;

        if (!string.Equals(Single("format"), ManifestFormat, StringComparison.Ordinal))
        {
            failure = "the manifest is not format `" + ManifestFormat.Replace('\t', ' ') + "`";
            return false;
        }

        if (at >= lines.Length || !string.Equals(lines[at], ManifestColumns, StringComparison.Ordinal))
        {
            failure = "the manifest's column line is not the one format 1 fixes";
            return false;
        }

        if (!int.TryParse(Single("rows"), NumberStyles.None, CultureInfo.InvariantCulture, out var declared))
        {
            failure = "the manifest declares no row count";
            return false;
        }

        string prologue;

        try
        {
            using var document = JsonDocument.Parse(Single("strict-prologue"));
            prologue = document.RootElement.GetString() ?? string.Empty;
        }
        catch (JsonException)
        {
            prologue = string.Empty;
        }
        catch (InvalidOperationException)
        {
            prologue = string.Empty;
        }

        if (!string.Equals(prologue, StrictPrologue, StringComparison.Ordinal))
        {
            failure = "the manifest's strict prologue is not the conformance root's";
            return false;
        }

        var flagLines = fields.TryGetValue("eval-flags", out var flagValues) ? flagValues : [];
        var evalFlags = new List<(string Name, byte Value)>();

        foreach (var flag in flagLines)
        {
            if (flag.Length < 2 ||
                !byte.TryParse(flag[1], NumberStyles.None, CultureInfo.InvariantCulture, out var value))
            {
                failure = "an eval-flags line of the manifest is not `<set>\\t<value>\\t<names>`";
                return false;
            }

            evalFlags.Add((flag[0], value));
        }

        if (!evalFlags.SequenceEqual(RuleEvalFlags))
        {
            failure = "the manifest's eval flag sets are not the eight part B's B3 names";
            return false;
        }

        var archiveField =
            fields.TryGetValue("pin-test262-archive", out var archiveValues) && archiveValues.Count == 1
            ? archiveValues[0]
            : [];

        if (archiveField.Length != 2)
        {
            failure = "the manifest pins no test262 archive";
            return false;
        }

        var virtualRoot = Single("virtual-root");
        var storedRoot = Single("stored-root");

        if (!IsRoot(virtualRoot) || !IsRoot(storedRoot) ||
            virtualRoot.StartsWith(storedRoot, StringComparison.Ordinal) ||
            storedRoot.StartsWith(virtualRoot, StringComparison.Ordinal))
        {
            failure = "the manifest's virtual and stored roots are not two distinct `/name/` roots";
            return false;
        }

        header = new Header(
            Single("corpus"),
            declared,
            virtualRoot,
            storedRoot,
            prologue,
            [.. evalFlags],
            archiveField[0],
            archiveField[1]);

        var ids = new HashSet<string>(StringComparer.Ordinal);
        var paths = new HashSet<string>(StringComparer.Ordinal);

        for (at++; at < lines.Length; at++)
        {
            var cells = lines[at].Split('\t');

            if (cells.Length != 9 || cells.Any(cell => cell.Length == 0))
            {
                failure = $"line {at + 1} of the manifest is not nine non-empty fields";
                return false;
            }

            var row = new Row(
                rows.Count, cells[0], cells[1], cells[2], cells[3], cells[4],
                cells[5].Split(','), string.Equals(cells[7], "yes", StringComparison.Ordinal));

            if (!TryCheckRow(row, cells[7], out failure))
            {
                failure = $"line {at + 1} of the manifest: {failure}";
                return false;
            }

            if (!ids.Add(row.Id) || !paths.Add(row.Origin + "\t" + row.Path))
            {
                failure = $"line {at + 1} of the manifest repeats an id or an input";
                return false;
            }

            rows.Add(row);
        }

        if (rows.Count != declared)
        {
            failure = $"the manifest declares {declared} rows and holds {rows.Count}";
            return false;
        }

        failure = string.Empty;
        return true;
    }

    private static bool IsRoot(string root) =>
        root.Length > 2 && root[0] == '/' && root[^1] == '/' && root.IndexOf('/', 1) == root.Length - 1;

    /// <summary>Holds one row to the closed vocabularies format 1 fixes.</summary>
    private static bool TryCheckRow(Row row, string evalFamily, out string failure)
    {
        var prefix = row.Origin switch
        {
            "test262" => "test/",
            "broiler-vm" or "broiler-js" => "inputs/",
            _ => null,
        };

        failure = string.Empty;

        if (prefix is null)
        {
            failure = $"the origin `{row.Origin}` is not one format 1 knows";
        }
        else if (!row.Path.StartsWith(prefix, StringComparison.Ordinal) ||
            row.Path.Split('/').Any(segment => segment is "" or "." or ".."))
        {
            failure = $"the path `{row.Path}` is not a plain path under `{prefix}`";
        }
        else if (row.Sha256.Length != 64 ||
            row.Sha256.Any(c => c is not ((>= '0' and <= '9') or (>= 'a' and <= 'f'))))
        {
            failure = "the SHA-256 is not 64 lowercase hex digits";
        }
        else if (evalFamily is not ("yes" or "no") ||
            (row.EvalFamily && !string.Equals(row.Goal, "script", StringComparison.Ordinal)))
        {
            failure = "the eval-family column is not `yes` or `no`, or says `yes` of a goal that is not script";
        }
        else
        {
            var admitted = row.Goal switch
            {
                "script" => new[] { "strict", "sloppy", "raw" },
                "module" => ["module"],
                "json" => ["json"],
                _ => [],
            };

            if (admitted.Length == 0)
            {
                failure = $"the goal `{row.Goal}` is not one format 1 knows";
            }
            else if (row.Variants.Any(variant => !admitted.Contains(variant)) ||
                row.Variants.Distinct(StringComparer.Ordinal).Count() != row.Variants.Length)
            {
                failure = $"the variants `{string.Join(',', row.Variants)}` are not the goal's";
            }
        }

        return failure.Length == 0;
    }

    /// <summary>Every compilation of the run, in report order: by sub-population, then by row.</summary>
    private static List<Item> Plan(Inputs inputs, IReadOnlySet<string> populations)
    {
        var items = new List<Item>();

        foreach (var population in Populations.Where(populations.Contains))
        {
            foreach (var row in inputs.Rows)
            {
                switch (population)
                {
                    case "B1":
                        foreach (var variant in row.Variants)
                        {
                            items.Add(new Item(row.Index, population, variant, "bytecode"));
                            items.Add(new Item(row.Index, population, variant, "native"));
                        }

                        break;
                    case "B2" when row.IsTest262:
                        foreach (var variant in row.Variants)
                        {
                            items.Add(new Item(row.Index, population, variant, "bytecode"));
                        }

                        break;
                    case "B3" when row.EvalFamily:
                        foreach (var (name, _) in inputs.Header.EvalFlags)
                        {
                            items.Add(new Item(row.Index, population, name, "bytecode"));
                        }

                        break;
                    case "B4":
                        foreach (var variant in row.Variants)
                        {
                            items.Add(new Item(row.Index, population, variant, "slice"));
                        }

                        break;
                }
            }
        }

        return items;
    }

    /// <summary>Restricts the plan to the keys a file lists, one tab-separated key per line.</summary>
    private static bool TrySelect(
        string file, IReadOnlyList<Row> rows, ref List<Item> items, out string selection, out string failure)
    {
        selection = string.Empty;

        if (!File.Exists(file))
        {
            failure = "no selection at " + file;
            return false;
        }

        var bytes = File.ReadAllBytes(file);
        var wanted = new HashSet<string>(StringComparer.Ordinal);

        foreach (var line in Encoding.UTF8.GetString(bytes).Split('\n'))
        {
            var key = line.TrimEnd('\r');

            if (key.Length != 0 && !key.StartsWith('#'))
            {
                wanted.Add(string.Join('\t', key.Split('\t').Take(4)));
            }
        }

        var planned = items.Select(item => item.Key(rows)).ToHashSet(StringComparer.Ordinal);
        var unknown = wanted.Where(key => !planned.Contains(key)).ToList();

        if (unknown.Count != 0)
        {
            failure = $"{unknown.Count} selected key(s) name no compilation of this run, the first " +
                unknown[0].Replace('\t', ' ');
            return false;
        }

        items = items.Where(item => wanted.Contains(item.Key(rows))).ToList();
        selection = Sha256(bytes);
        failure = string.Empty;
        return true;
    }

    /// <summary>
    /// Runs every compilation: on <c>n</c> workers taking the next item each, or, twice
    /// concurrently, on two workers that start each item together.
    /// </summary>
    private static void Execute(Inputs inputs, List<Item> items, Options options, Result[] first, Result[]? second)
    {
        var workers = new List<Thread>();
        var done = 0;
        var step = Math.Max(items.Count / 20, 1);

        void Progress()
        {
            var count = Interlocked.Increment(ref done);

            if (count % step == 0)
            {
                Console.Error.WriteLine(
                    $"broiler-js-slice-compiler: --parse-corpus {count} of " +
                    $"{(second is null ? 1 : 2) * items.Count}");
            }
        }

        // ONE BARRIER, SO THE TWO COMPILATIONS OF AN INPUT OVERLAP. Two workers walking the list
        // independently would drift apart and compile most inputs at different moments, which is two
        // sequential runs on two threads and not the concurrency part C asks about. Each worker meets
        // the other before every item, so both compilations of one input start together.
        using var barrier = new Barrier(2);
        var next = -1;

        if (second is null)
        {
            for (var worker = 0; worker < options.Workers; worker++)
            {
                workers.Add(new Thread(
                    () =>
                    {
                        for (var index = Interlocked.Increment(ref next); index < items.Count;
                            index = Interlocked.Increment(ref next))
                        {
                            first[index] = Compile(inputs, items[index], options.WriteBytes, ".bin");
                            Progress();
                        }
                    },
                    WorkerStackBytes));
            }
        }
        else
        {
            foreach (var (results, suffix) in new[] { (first, ".bin"), (second, ".2.bin") })
            {
                workers.Add(new Thread(
                    () =>
                    {
                        for (var index = 0; index < items.Count; index++)
                        {
                            barrier.SignalAndWait();
                            results[index] = Compile(inputs, items[index], options.WriteBytes, suffix);
                            Progress();
                        }
                    },
                    WorkerStackBytes));
            }
        }

        foreach (var worker in workers)
        {
            worker.Start();
        }

        foreach (var worker in workers)
        {
            worker.Join();
        }
    }

    /// <summary>Compiles one item and answers what it produced; it never throws.</summary>
    private static Result Compile(Inputs inputs, Item item, string? writeBytes, string suffix)
    {
        var row = inputs.Rows[item.Row];
        var text = inputs.Texts[item.Row];

        try
        {
            switch (item.Population)
            {
                case "B1":
                    return Wide(
                        inputs, row, item, writeBytes, suffix, Alone(inputs, row, text, item.Variant),
                        item.Form == "native"
                            ? new JsCompileRequest(JsFeatureManifest.Wide, JsOutputForm.Native, NativeBackend)
                            : new JsCompileRequest(JsFeatureManifest.Wide, JsOutputForm.Bytecode));
                case "B2":
                    return Wide(
                        inputs, row, item, writeBytes, suffix, Composed(inputs, row, text, item.Variant),
                        new JsCompileRequest(JsFeatureManifest.Wide, JsOutputForm.Bytecode));
                case "B3":
                    var flags = inputs.Header.EvalFlags.First(set => set.Name == item.Variant).Value;
                    JsScriptUnit[] evaluated =
                        [new JsScriptUnit("eval", text, SliceParseOptions.Eval((JsFormat.EvalRequestFlags)flags))];

                    return Wide(
                        inputs, row, item, writeBytes, suffix, new Units(evaluated, [], string.Empty),
                        new JsCompileRequest(JsFeatureManifest.Wide, JsOutputForm.Bytecode));
                default:
                    return Slice(inputs, row, item, writeBytes, suffix, text);
            }
        }
        catch (Exception failure)
        {
            return new Result("crashed", "-", "-", "-", failure.GetType().FullName ?? failure.GetType().Name);
        }
    }

    /// <summary>B1's units: the input alone under its goal and variant.</summary>
    private static Units Alone(Inputs inputs, Row row, string text, string variant)
    {
        var name = inputs.NameOf(row);

        switch (variant)
        {
            case "module":
                var graph = LoadGraph(inputs, name, text, string.Empty);
                return new Units([], graph.Modules, graph.Failure);
            case "json":
                var key = JsFormat.TypedSpecifier(name, JsFormat.JsonModuleType);
                return new Units(
                    [],
                    [new JsModuleUnit(key, text, SliceParseOptions.Module) { Type = JsFormat.JsonModuleType }],
                    string.Empty);
            default:
                return new Units(
                    [new JsScriptUnit("test", text, SliceParseOptions.Script, variant == "strict", name)],
                    [],
                    string.Empty);
        }
    }

    /// <summary>
    /// B2's units: the conformance root's composition of a test (<c>Test262Run.RunVariant</c> at the
    /// corpus commit) - the harness scripts unless the variant is raw, then the test or its graph.
    /// </summary>
    /// <remarks>
    /// What decides whether the root RUNS a test - a proposed feature, the host's collection hook - is
    /// not composition, and is not applied: every test the root's front-matter reader reads is
    /// composed. A fixture is never run alone by the root and is reported as not compiled.
    /// </remarks>
    private static Units Composed(Inputs inputs, Row row, string text, string variant)
    {
        if (row.Path.Contains("_FIXTURE", StringComparison.Ordinal))
        {
            return new Units([], [], "fixture");
        }

        if (!TryReadFrontmatter(text, out var frontmatter))
        {
            return new Units([], [], "frontmatter");
        }

        var scripts = new List<JsScriptUnit>();

        if (!string.Equals(variant, "raw", StringComparison.Ordinal))
        {
            var names = new List<string> { "assert.js", "sta.js" };

            if (frontmatter.Flags.Contains("async", StringComparer.Ordinal))
            {
                names.Add("doneprintHandle.js");
            }

            names.AddRange(frontmatter.Includes);

            foreach (var harness in names)
            {
                if (!inputs.Suite.TryGetValue("harness/" + harness, out var bytes))
                {
                    return new Units([], [], "no-harness-file");
                }

                scripts.Add(new JsScriptUnit(
                    "harness" + scripts.Count.ToString(CultureInfo.InvariantCulture),
                    Decode(bytes),
                    SliceParseOptions.Script,
                    ForceStrict: false));
            }
        }

        var name = inputs.NameOf(row);

        if (string.Equals(variant, "module", StringComparison.Ordinal))
        {
            var graph = LoadGraph(inputs, name, text, string.Empty);
            return new Units(scripts, graph.Modules, graph.Failure);
        }

        scripts.Add(new JsScriptUnit("test", text, SliceParseOptions.Script, variant == "strict", name));
        return new Units(scripts, [], string.Empty);
    }

    /// <summary>Compiles units on the wide path and reports the answer.</summary>
    private static Result Wide(
        Inputs inputs,
        Row row,
        Item item,
        string? writeBytes,
        string suffix,
        Units units,
        JsCompileRequest request)
    {
        if (units.Failure.Length != 0)
        {
            return new Result("not-compiled", "-", "-", "-", units.Failure);
        }

        var compiled = JsCompiler.Compile(units.Scripts, units.Modules, request);
        var artifact = compiled.Succeeded ? compiled.Artifact : null;
        return Answer(row, item, writeBytes, suffix, artifact, compiled.Diagnostics);
    }

    /// <summary>B4: the slice front end, strict by the prologue, under the input's goal.</summary>
    private static Result Slice(Inputs inputs, Row row, Item item, string? writeBytes, string suffix, string text)
    {
        // THE SLICE FRONT END HAS NO READING OF A JSON DOCUMENT, so a JSON input is reported as not
        // compiled in B4 rather than left out: every row is reported in every sub-population it is in.
        if (string.Equals(item.Variant, "json", StringComparison.Ordinal))
        {
            return new Result("not-compiled", "-", "-", "-", "no-json-goal");
        }

        var compiled = SliceSourceCompiler.Compile(
            item.Variant == "strict" ? inputs.Header.StrictPrologue + text : text,
            item.Variant == "module" ? SliceParseOptions.Module : SliceParseOptions.Script);

        return Answer(row, item, writeBytes, suffix, compiled.Artifact, compiled.Diagnostics);
    }

    /// <summary>Reports one compilation's answer, and writes its bytes when the run was asked to.</summary>
    private static Result Answer(
        Row row,
        Item item,
        string? writeBytes,
        string suffix,
        byte[]? artifact,
        IReadOnlyList<SliceSourceDiagnostic> diagnostics)
    {
        var listed = diagnostics.Count == 0
            ? "-"
            : string.Join(
                ';',
                diagnostics.Select(diagnostic => string.Create(
                    CultureInfo.InvariantCulture,
                    $"{(int)diagnostic.Code}:{diagnostic.Line}:{diagnostic.Column}")));

        if (artifact is null)
        {
            return new Result("refused", listed, "-", "-", "-");
        }

        if (writeBytes is not null)
        {
            File.WriteAllBytes(
                Path.Combine(writeBytes, $"{row.Id}.{item.Population}.{item.Variant}.{item.Form}{suffix}"),
                artifact);
        }

        return new Result(
            "accepted", listed, Sha256(artifact), artifact.Length.ToString(CultureInfo.InvariantCulture), "-");
    }

    /// <summary>
    /// The graph the conformance root's resolution (<c>Test262Modules.Load</c>) reaches from one
    /// module, over fixed names: a relative or absolute specifier resolves against its referrer's
    /// directory, a typed request names a typed module, and a specifier naming nothing is carried
    /// unresolved, for the artifact to refuse.
    /// </summary>
    private static Graph LoadGraph(Inputs inputs, string rootName, string rootText, string rootType)
    {
        var root = JsFormat.TypedSpecifier(Normalise(rootName), rootType);
        var modules = new List<JsModuleUnit>();
        var texts = new Dictionary<string, string>(StringComparer.Ordinal) { [root] = rootText };
        var seen = new HashSet<string>(StringComparer.Ordinal) { root };
        var pending = new Queue<string>();
        pending.Enqueue(root);

        while (pending.Count != 0)
        {
            var key = pending.Dequeue();
            var path = JsFormat.SplitTypedSpecifier(key, out var type);

            if (modules.Count == MaximumModules)
            {
                return new Graph([], "graph-too-large");
            }

            if (!texts.TryGetValue(key, out var text))
            {
                if (!inputs.TryRead(path, out text))
                {
                    return new Graph([], "graph-unreadable");
                }

                texts[key] = text;
            }

            if (type.Length != 0)
            {
                modules.Add(new JsModuleUnit(key, text, SliceParseOptions.Module) { Type = type });
                continue;
            }

            var requests = JsCompiler.Requests(text, SliceParseOptions.Module);

            if (!requests.Succeeded)
            {
                modules.Add(new JsModuleUnit(key, text, SliceParseOptions.Module));
                continue;
            }

            var resolutions = new List<JsResolvedRequest>(requests.Specifiers.Count);

            foreach (var request in requests.Specifiers)
            {
                var specifier = JsFormat.SplitTypedSpecifier(request, out var requestType);
                var resolved = Resolve(key, specifier);

                if (resolved.Length == 0 || !inputs.Exists(resolved))
                {
                    resolutions.Add(new JsResolvedRequest(request, request));
                    continue;
                }

                var target = JsFormat.TypedSpecifier(resolved, requestType);
                resolutions.Add(new JsResolvedRequest(request, target));

                if (seen.Add(target))
                {
                    pending.Enqueue(target);
                }
            }

            modules.Add(new JsModuleUnit(key, text, SliceParseOptions.Module, resolutions));
        }

        return new Graph(modules, string.Empty);
    }

    /// <summary>The conformance root's resolution of a specifier against a referrer, over fixed names.</summary>
    private static string Resolve(string referrer, string specifier)
    {
        if (!specifier.StartsWith("./", StringComparison.Ordinal) &&
            !specifier.StartsWith("../", StringComparison.Ordinal) &&
            !specifier.StartsWith('/'))
        {
            return string.Empty;
        }

        if (specifier.StartsWith('/'))
        {
            return Normalise(specifier);
        }

        var slash = referrer.LastIndexOf('/');
        return Normalise((slash <= 0 ? "/" : referrer[..slash] + "/") + specifier);
    }

    /// <summary>
    /// An absolute <c>/</c>-separated name with <c>.</c>, <c>..</c> and empty segments resolved, as
    /// <c>Path.GetFullPath</c> answers for an absolute path on the rule's platform, and the same on
    /// every other.
    /// </summary>
    private static string Normalise(string name)
    {
        var segments = new List<string>();

        foreach (var segment in name.Split('/'))
        {
            if (segment is "" or ".")
            {
                continue;
            }

            if (segment == "..")
            {
                if (segments.Count != 0)
                {
                    segments.RemoveAt(segments.Count - 1);
                }

                continue;
            }

            segments.Add(segment);
        }

        return "/" + string.Join('/', segments);
    }

    /// <summary>Writes the report: a header, the column line, then one line per compilation.</summary>
    /// <remarks>
    /// <b>Nothing in it depends on the machine, the clock or the worker count</b>, so two runs with the
    /// same arguments over the same build write the same bytes; a path ending <c>.gz</c> is written
    /// compressed, and the comparison reads either.
    /// </remarks>
    private static void WriteReport(
        string path,
        Inputs inputs,
        List<Item> items,
        Result[] first,
        Result[]? second,
        string manifestSha256,
        string archiveSha256,
        Options options,
        string selection)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(path));

        if (directory is not null)
        {
            Directory.CreateDirectory(directory);
        }

        using var file = File.Create(path);
        using var body = path.EndsWith(".gz", StringComparison.Ordinal)
            ? new GZipStream(file, CompressionLevel.SmallestSize, leaveOpen: true)
            : (Stream)file;
        using var report = new StreamWriter(body, new UTF8Encoding(false), 1 << 20, leaveOpen: true);

        // LF, explicitly, on every platform, for the reason the corpus writer gives: the report is
        // hashed into a bundle and compared byte for byte.
        void Line(string text)
        {
            report.Write(text);
            report.Write('\n');
        }

        Line("#% format\t" + ReportFormat);
        Line("#% corpus\t" + inputs.Header.Corpus);
        Line("#% manifest-sha256\t" + manifestSha256);
        Line("#% manifest-rows\t" + inputs.Rows.Count.ToString(CultureInfo.InvariantCulture));
        Line("#% archive-sha256\t" + (archiveSha256.Length == 0 ? "-" : archiveSha256));
        Line("#% populations\t" + string.Join(',', Populations.Where(options.Populations.Contains)));
        Line("#% selection\t" + selection);
        Line("#% compilations\t" + items.Count.ToString(CultureInfo.InvariantCulture));
        Line("#% twice-concurrent\t" + (second is null ? "no" : "yes"));
        Line("#% native-backend\t" + NativeBackend);
        Line(
            "# B1 each input alone (strict: ForceStrict, text unchanged; a module with the graph the " +
            "conformance root's resolution reaches from it), bytecode and native; " +
            "B2 the conformance root's composition, bytecode; B3 eval code under E0-E7, bytecode; " +
            "B4 the slice front end (strict: the prologue before the text).");
        Line(
            "# diagnostics are code:line:column in the order the compiler reported them; " +
            "sha256 and length are the artifact's, for an accepted compilation.");
        Line(
            "id\tpopulation\tvariant\tform\toutcome\tdiagnostics\tsha256\tlength\tnote" +
            (second is null ? string.Empty : "\toutcome-2\tdiagnostics-2\tsha256-2\tlength-2\tnote-2"));

        for (var index = 0; index < items.Count; index++)
        {
            Line(
                items[index].Key(inputs.Rows) + "\t" + first[index].Columns +
                (second is null ? string.Empty : "\t" + second[index].Columns));
        }
    }

    /// <summary>
    /// The conformance root's front-matter reader (<c>Test262Metadata.TryRead</c>), restated for
    /// what composition reads: whether the block reads at all, its flags and its includes.
    /// </summary>
    /// <remarks>
    /// A block the root cannot read is a test the root does not compose, so every way its reader
    /// fails is kept: no block, an unclosed block, an indented line under no key, a line that is not
    /// <c>key: value</c>, a malformed list or negative expectation, no description, an unknown flag,
    /// and <c>onlyStrict</c> with <c>noStrict</c>.
    /// </remarks>
    private static bool TryReadFrontmatter(string text, out Frontmatter frontmatter)
    {
        frontmatter = new Frontmatter([], []);
        const string Open = "/*---";
        const string Close = "---*/";
        var open = text.IndexOf(Open, StringComparison.Ordinal);

        if (open < 0)
        {
            return false;
        }

        var close = text.IndexOf(Close, open + Open.Length, StringComparison.Ordinal);

        if (close < 0)
        {
            return false;
        }

        var lines = Dedent(
            text[(open + Open.Length)..close]
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace('\r', '\n')
                .Split('\n'));

        var description = string.Empty;
        var flags = new List<string>();
        var includes = new List<string>();

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];

            if (line.Trim().Length == 0)
            {
                continue;
            }

            if (Indent(line) > 0)
            {
                return false;
            }

            var colon = line.IndexOf(':', StringComparison.Ordinal);

            if (colon <= 0)
            {
                return false;
            }

            var key = line[..colon].Trim();
            var value = line[(colon + 1)..].Trim();

            switch (key)
            {
                case "description":
                    description = Scalar(lines, ref index, value);
                    break;
                case "flags":
                    if (!Sequence(lines, ref index, value, out flags))
                    {
                        return false;
                    }

                    break;
                case "features":
                    if (!Sequence(lines, ref index, value, out _))
                    {
                        return false;
                    }

                    break;
                case "includes":
                    if (!Sequence(lines, ref index, value, out includes))
                    {
                        return false;
                    }

                    break;
                case "negative":
                    if (!Negative(lines, ref index, value))
                    {
                        return false;
                    }

                    break;
                default:
                    _ = Scalar(lines, ref index, value);
                    break;
            }
        }

        string[] known =
        [
            "onlyStrict", "noStrict", "module", "raw", "async", "generated", "CanBlockIsFalse",
            "CanBlockIsTrue", "non-deterministic",
        ];

        if (description.Length == 0 ||
            flags.Any(flag => !known.Contains(flag, StringComparer.Ordinal)) ||
            (flags.Contains("onlyStrict", StringComparer.Ordinal) &&
                flags.Contains("noStrict", StringComparer.Ordinal)))
        {
            return false;
        }

        frontmatter = new Frontmatter(flags, includes);
        return true;
    }

    private static string[] Dedent(string[] lines)
    {
        var shared = int.MaxValue;

        foreach (var line in lines)
        {
            if (line.Trim().Length != 0)
            {
                shared = Math.Min(shared, Indent(line));
            }
        }

        if (shared is 0 or int.MaxValue)
        {
            return lines;
        }

        for (var index = 0; index < lines.Length; index++)
        {
            lines[index] = lines[index].Length <= shared ? string.Empty : lines[index][shared..];
        }

        return lines;
    }

    private static int Indent(string line)
    {
        var count = 0;

        while (count < line.Length && line[count] == ' ')
        {
            count++;
        }

        return count;
    }

    private static string Scalar(string[] lines, ref int index, string value)
    {
        var block = value is ">" or "|" or ">-" or "|-";

        if (value.Length != 0 && !block)
        {
            return value;
        }

        var parts = new List<string>();

        while (index + 1 < lines.Length)
        {
            var next = lines[index + 1];

            if (next.Trim().Length == 0)
            {
                index++;
                continue;
            }

            if (Indent(next) == 0)
            {
                break;
            }

            parts.Add(next.Trim());
            index++;
        }

        return string.Join(' ', parts);
    }

    private static bool Sequence(string[] lines, ref int index, string value, out List<string> items)
    {
        items = [];
        var trimmed = value.Trim();

        if (trimmed.StartsWith('['))
        {
            if (!trimmed.EndsWith(']'))
            {
                return false;
            }

            items.AddRange(Split(trimmed[1..^1]));
            return true;
        }

        if (trimmed.Length != 0)
        {
            items.AddRange(Split(trimmed));
            return true;
        }

        while (index + 1 < lines.Length)
        {
            var next = lines[index + 1];

            if (next.Trim().Length == 0)
            {
                index++;
                continue;
            }

            if (Indent(next) == 0)
            {
                break;
            }

            var item = next.Trim();

            if (!item.StartsWith("- ", StringComparison.Ordinal))
            {
                return false;
            }

            items.Add(item[2..].Trim());
            index++;
        }

        return true;
    }

    private static bool Negative(string[] lines, ref int index, string value)
    {
        if (value.Trim().Length != 0)
        {
            return false;
        }

        var phase = string.Empty;
        var type = string.Empty;

        while (index + 1 < lines.Length)
        {
            var next = lines[index + 1];

            if (next.Trim().Length == 0)
            {
                index++;
                continue;
            }

            if (Indent(next) == 0)
            {
                break;
            }

            var line = next.Trim();
            var colon = line.IndexOf(':', StringComparison.Ordinal);

            if (colon <= 0)
            {
                return false;
            }

            switch (line[..colon].Trim())
            {
                case "phase":
                    phase = line[(colon + 1)..].Trim();
                    break;
                case "type":
                    type = line[(colon + 1)..].Trim();
                    break;
                default:
                    return false;
            }

            index++;
        }

        return phase.Length != 0 && type.Length != 0 && phase is "parse" or "resolution" or "runtime";
    }

    private static IEnumerable<string> Split(string value) =>
        value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    /// <summary>Decodes a file as the conformance root does: UTF-8, a leading byte-order mark dropped.</summary>
    private static string Decode(byte[] bytes)
    {
        var start = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? 3 : 0;
        return Encoding.UTF8.GetString(bytes, start, bytes.Length - start);
    }

    private static string Sha256(byte[] bytes) => Convert.ToHexStringLower(SHA256.HashData(bytes));
}
