// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// Stage JSB-1's standing check: the wide lowering's bytes are a function of its source alone, and
/// the lexical names a script or an evaluated program declares come out in the order the source
/// declares them (SJL-12.6 item 3, JSD-0080 decision D14).
/// </summary>
/// <remarks>
/// <para>
/// <b>They run beside "the lowering is deterministic" because that row is the slice front end's
/// half of the same claim</b>, and the wide front end is the one that wrote every artifact a backend
/// has consumed. Stage JSB-1's gate asks for exactly this: every source of the wide and module corpora
/// compiled twice through <see cref="JsCompiler"/> and compared byte for byte, where the slice
/// equivalent already runs, and the global lexical hoisting order derived from the source.
/// </para>
/// <para>
/// <b>Two rows, because each catches what the other cannot.</b> A lowering that emitted the global
/// lexical declarations in the reverse of source order would be perfectly deterministic, and only
/// the order row sees it; one that shuffled them anew for every compilation would, on a source with
/// one such name, be in order every time, and only a comparison of two compilations of a source with
/// many sees it. Bundle <c>sjl-12-001</c>'s part C watches both controls fail: K1 reverses the order
/// and K2 shuffles it per compilation.
/// </para>
/// </remarks>
internal static class WideLoweringChecks
{
    internal static IEnumerable<(string Name, bool Passed, string Detail)> Run()
    {
        yield return TheWideLoweringIsDeterministic();
        yield return TheWideLoweringDeclaresLexicalNamesInSourceOrder();
    }

    // ---- the probes ------------------------------------------------------------------------------
    //
    // EVERY NAME IS PLACED SO THAT NO ORDER BUT THE SOURCE'S PUTS THEM WHERE THEY ARE: not sorted, not
    // reversed, not by length, and with a `var` and a function among them, which the lowering declares
    // first and which must not move a lexical name. Destructuring is there because a pattern's names are
    // collected by a walk of their own, and a block's `let` because it is not a global one.

    /// <summary>A script whose top level declares lexical names, and the order it declares them in.</summary>
    private const string ScriptProbe =
        "var v = 0;\n" +
        "let m = 1;\n" +
        "const b = 2, [y, { k: e }] = [3, { k: 4 }];\n" +
        "function f() { return m + b; }\n" +
        "class Q {}\n" +
        "let { z, ...a } = { z: 5 };\n" +
        "{ let inner = 6; }\n" +
        "const w = 7;\n" +
        "let c;\n" +
        "class h {}\n" +
        "let x1 = 8, D = 9, r0 = 10;\n" +
        "const n = [m, b, y, e, z, a, w, c, x1, D, r0];\n";

    private static readonly (string Name, bool Constant)[] ScriptOrder =
    [
        ("m", false), ("b", true), ("y", true), ("e", true), ("Q", false), ("z", false), ("a", false),
        ("w", true), ("c", false), ("h", false), ("x1", false), ("D", false), ("r0", false), ("n", true),
    ];

    /// <summary>A second script, compiled after the first in one compilation.</summary>
    private const string SecondScriptProbe =
        "let t9 = 1;\nconst s = 2;\nclass K {}\nlet g4;\n";

    private static readonly (string Name, bool Constant)[] SecondScriptOrder =
        [("t9", false), ("s", true), ("K", false), ("g4", false)];

    /// <summary>
    /// An evaluated program whose top level declares lexical names, with a direct <c>eval</c> of its
    /// own so that the scope map writes a row for its record and the slots can be read.
    /// </summary>
    private const string EvalProbe =
        "let m = 1;\n" +
        "const b = 2;\n" +
        "var v = 3;\n" +
        "class Q {}\n" +
        "let [y, { k: e }] = [4, { k: 5 }];\n" +
        "function f() { return m; }\n" +
        "const w = 6;\n" +
        "let c;\n" +
        "eval('0');\n";

    private static readonly string[] EvalOrder = ["m", "b", "Q", "y", "e", "w", "c"];

    // ---- the determinism row -----------------------------------------------------------------------

    /// <summary>
    /// The wide and module corpora, each source compiled four times - twice in turn, and twice at once
    /// on two threads - give one artifact.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The population is three sets, each named in the row's detail.</b>
    /// - <b>The retained corpus's wide and module rows</b>: every entry of
    ///   <c>src/tests/corpus/js-1/corpus.manifest</c> whose mode is <c>wide</c>, <c>wide-declining</c>,
    ///   <c>modules</c> or <c>modules-no-resolver</c>, REBUILT by the producer that writes them. Most of
    ///   them are malformed artifacts the producer assembles byte by byte, which pass through no
    ///   lowering, so comparing them says nothing about it; only the few it compiles from source go
    ///   through <see cref="JsCompiler"/>, and none of those declares two global lexical names.
    /// - <b>The stored inputs of the retained parse corpus</b>, <c>src/tests/corpus/parse-1/inputs/</c>:
    ///   each <c>.js</c> file under the script goal and each <c>.mjs</c> file under the module goal,
    ///   alone. They give the row its breadth: programs written for two engines' own tests, not for it.
    /// - <b>The order row's probes</b>, the two scripts as one compilation and the evaluated program
    ///   under both of its strictnesses.
    /// </para>
    /// <para>
    /// <b>The probes are what make a reordering visible here.</b> A shuffle of a script's global lexical
    /// declarations moves no byte of a script that declares one name or none, which is every row of the
    /// retained corpus and nearly every stored input of the parse corpus; the few stored inputs that
    /// declare two or three can shuffle back into one order. The first probe declares fourteen, so four
    /// compilations of it in one order are no accident: it is the probe, not the corpora, that makes a
    /// per-compilation shuffle - bundle <c>sjl-12-001</c>'s control K2 - fail this row.
    /// </para>
    /// <para>
    /// <b>Fresh compilations, four of each, and two of them at the same time.</b> Two in turn catch a
    /// lowering that carries state from one compilation into the next, or that draws on randomness of
    /// its own for each compilation, as an unseeded <see cref="System.Random"/> does - which is what K2
    /// patches in. Two at once on two threads catch one that shares state between compilations that
    /// overlap. A refused source must be refused all four times, and an accepted one must give one
    /// SHA-256 all four times.
    /// </para>
    /// <para>
    /// <b>What this row cannot catch is an order that varies only from one process to the next</b>, and
    /// it does not claim to. All four compilations run in this one process, so they share everything a
    /// process draws once - the seed of string hash codes above all, which decides the order of
    /// anything ordered by a hash code: a <c>Hashtable</c>, a <c>ConcurrentDictionary</c>, an
    /// <c>ImmutableHashSet</c>, a value from <see cref="HashCode"/>. (A <c>Dictionary</c> or a
    /// <c>HashSet</c> nothing is removed from enumerates in insertion order whatever the seed, which is
    /// why the two collections SJL-12.6 item 3 replaced never moved a byte.) That half of the claim
    /// belongs to the separate processes of bundle <c>sjl-12-001</c>'s part C - its runs 1 and 2 - and
    /// not to this row.
    /// </para>
    /// <para>
    /// <b>It fails on a population that is not there</b>, because agreement over nothing proves nothing:
    /// where the producer throws, where a wide or module row the retained manifest lists was not
    /// rebuilt, where a stored script or module the parse corpus's manifest lists was not compiled,
    /// where either manifest lists none, and where a probe is refused. Each such shortfall is named in
    /// the detail.
    /// </para>
    /// <para>
    /// <b>The corpora are read from the checkout</b>, from the first directory above the process's
    /// own or above the current one that holds them, as the fuel-parity twins are. A run that cannot
    /// find them fails rather than skipping a set.
    /// </para>
    /// </remarks>
    private static (string, bool, string) TheWideLoweringIsDeterministic()
    {
        const string Name = "the wide lowering is deterministic";

        var corpora = ReadCorpora();

        if (corpora is null)
        {
            return (
                Name,
                false,
                "no directory above this process's or the current one holds src/tests/corpus/js-1/corpus.manifest beside src/tests/corpus/parse-1's manifest and stored inputs");
        }

        var inputs = corpora.Inputs;
        var first = Answers(inputs);
        var second = Answers(inputs);

        Dictionary<string, string>? third = null;
        Dictionary<string, string>? fourth = null;

        // TWO THREADS STARTED TOGETHER, each compiling the whole population in the same order, so that
        // the two compilations of one source overlap or nearly do.
        var threads = new[]
        {
            new Thread(() => third = Answers(inputs)),
            new Thread(() => fourth = Answers(inputs)),
        };

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        var differed = new List<string>();
        var accepted = 0;

        foreach (var (key, answer) in first)
        {
            var same =
                second.TryGetValue(key, out var two) && string.Equals(answer, two, StringComparison.Ordinal) &&
                third is not null && third.TryGetValue(key, out var three) && string.Equals(answer, three, StringComparison.Ordinal) &&
                fourth is not null && fourth.TryGetValue(key, out var four) && string.Equals(answer, four, StringComparison.Ordinal);

            if (!same)
            {
                differed.Add(key);
            }
            else if (answer.StartsWith("bytes ", StringComparison.Ordinal))
            {
                accepted++;
            }
        }

        if (second.Count != first.Count || third?.Count != first.Count || fourth?.Count != first.Count)
        {
            differed.Add("the four passes did not answer for the same number of sources");
        }

        // THE POPULATION, BEFORE ITS AGREEMENT IS BELIEVED. Four passes that compiled nothing agree, and
        // so do four in which the producer threw the same way, so each set is held to what its manifest
        // lists and each probe must have been compiled.
        var shortfalls = new List<string>();

        foreach (var pass in new[] { first, second, third, fourth })
        {
            if (pass is not null && pass.TryGetValue(ProducerKey, out var threw))
            {
                shortfalls.Add("the retained corpus's producer " + threw);
                break;
            }
        }

        Hold(shortfalls, first, "js-1/", corpora.CorpusRows, "wide and module rows the retained corpus's manifest lists", "rebuilt");
        Hold(shortfalls, first, "parse-1/", corpora.StoredInputs, "stored scripts and modules the parse corpus's manifest lists", "compiled");

        foreach (var probe in ProbeKeys)
        {
            if (!first.TryGetValue(probe, out var answer) || !answer.StartsWith("bytes ", StringComparison.Ordinal))
            {
                shortfalls.Add($"the probe `{probe}` gave no artifact: {answer ?? "no answer"}");
            }
        }

        var corpusRows = first.Keys.Count(static key => key.StartsWith("js-1/", StringComparison.Ordinal));
        var stored = first.Keys.Count(static key => key.StartsWith("parse-1/", StringComparison.Ordinal));
        var probes = first.Keys.Count(static key => key.StartsWith("probe/", StringComparison.Ordinal));

        if (shortfalls.Count == 0 && differed.Count == 0)
        {
            return (
                Name,
                true,
                $"{first.Count} sources, each four times - twice in turn and twice at once on two threads: the retained corpus's {corpusRows} wide and module rows rebuilt by their producer, every one its manifest lists; the parse corpus's {stored} stored inputs compiled, every one its manifest lists; and {probes} lexical-order probes compiled, each accepted. {accepted} artifacts identical in all four, every refusal refused all four times");
        }

        var failures = new List<string>();

        if (shortfalls.Count != 0)
        {
            failures.Add("the population is short, so agreement over it proves nothing: " + string.Join("; ", shortfalls));
        }

        if (differed.Count != 0)
        {
            failures.Add(
                $"{differed.Count} of {first.Count} differed between the four compilations: " +
                string.Join(", ", differed.Take(8)) + (differed.Count > 8 ? ", ..." : string.Empty));
        }

        return (Name, false, string.Join(". ", failures));
    }

    /// <summary>The key a pass answers under where the retained corpus's producer threw.</summary>
    private const string ProducerKey = "js-1 producer";

    /// <summary>The keys the order row's probes answer under.</summary>
    private static readonly string[] ProbeKeys = ["probe/two scripts", "probe/eval, sloppy", "probe/eval, strict"];

    /// <summary>
    /// Adds a shortfall where a manifest lists nothing for a set, or where a source it lists has no
    /// answer in the pass.
    /// </summary>
    private static void Hold(
        List<string> shortfalls,
        Dictionary<string, string> pass,
        string prefix,
        IReadOnlyList<string> listed,
        string what,
        string done)
    {
        if (listed.Count == 0)
        {
            shortfalls.Add($"there are no {what}");
            return;
        }

        var missing = listed.Where(name => !pass.ContainsKey(prefix + name)).ToArray();

        if (missing.Length != 0)
        {
            shortfalls.Add(
                $"{missing.Length} of the {listed.Count} {what} were not {done}: " +
                string.Join(", ", missing.Take(4)) + (missing.Length > 4 ? ", ..." : string.Empty));
        }
    }

    /// <summary>Whether a retained corpus row's mode is one the wide front end's rows carry.</summary>
    private static bool IsWideMode(string mode) =>
        mode is "wide" or "wide-declining" or "modules" or "modules-no-resolver";

    /// <summary>
    /// One pass over the population: for each source, <c>bytes</c> and the artifact's SHA-256, or
    /// <c>refused</c> and the first diagnostic's code, keyed by where the source comes from.
    /// </summary>
    private static Dictionary<string, string> Answers(IReadOnlyList<(string Key, string Text)> inputs)
    {
        var answers = new Dictionary<string, string>(StringComparer.Ordinal);

        // THE RETAINED CORPUS'S WIDE AND MODULE ROWS, REBUILT. The producer throws where a source it
        // retains does not compile. That is recorded under a key of its own, which the row reads as a
        // population that is not there: four passes that all threw the same way agree about nothing.
        try
        {
            foreach (var entry in CorpusBuilder.Build())
            {
                if (IsWideMode(entry.Mode))
                {
                    answers["js-1/" + entry.Name] = "bytes " + Sha256(entry.Bytes);
                }
            }
        }
        catch (Exception failure)
        {
            answers[ProducerKey] = "threw " + failure.GetType().Name + ": " + failure.Message;
        }

        foreach (var (key, text) in inputs)
        {
            answers["parse-1/" + key] = Answer(() =>
                key.EndsWith(".mjs", StringComparison.Ordinal)
                    ? JsCompiler.Compile([], [new JsModuleUnit("mem:/" + key, text, SliceParseOptions.Module, [])])
                    : JsCompiler.Compile([new JsScriptUnit("main", text, SliceParseOptions.Script)]));
        }

        answers[ProbeKeys[0]] = Answer(CompileScriptProbes);
        answers[ProbeKeys[1]] = Answer(() => CompileEvalProbe(JsFormat.EvalRequestFlags.None));
        answers[ProbeKeys[2]] = Answer(() => CompileEvalProbe(JsFormat.EvalRequestFlags.Strict));

        return answers;
    }

    /// <summary>What one compilation answered, as a line two passes can compare.</summary>
    private static string Answer(Func<JsCompilation> compile)
    {
        try
        {
            var compiled = compile();

            return compiled.Succeeded && compiled.Artifact is not null
                ? "bytes " + Sha256(compiled.Artifact)
                : "refused " + (compiled.Diagnostics.Count == 0 ? "with no diagnostic" : compiled.Diagnostics[0].Code.ToString());
        }
        catch (Exception failure)
        {
            return "threw " + failure.GetType().Name;
        }
    }

    /// <summary>What the determinism row reads from the checkout, and what the two manifests list.</summary>
    /// <param name="Inputs">
    /// The parse corpus's stored inputs, script and module, keyed by their path under <c>inputs/</c>
    /// with <c>/</c> separators, in ordinal order.
    /// </param>
    /// <param name="CorpusRows">The names of the retained corpus's wide and module rows, as its manifest lists them.</param>
    /// <param name="StoredInputs">
    /// The stored scripts and modules the parse corpus's manifest lists, keyed as <paramref name="Inputs"/> is.
    /// </param>
    private sealed record Corpora(
        IReadOnlyList<(string Key, string Text)> Inputs,
        IReadOnlyList<string> CorpusRows,
        IReadOnlyList<string> StoredInputs);

    /// <summary>
    /// The two retained corpora, from the first directory above the process's own or above the current
    /// one that holds both, or <see langword="null"/> where none does.
    /// </summary>
    private static Corpora? ReadCorpora()
    {
        foreach (var start in new[] { AppContext.BaseDirectory, Environment.CurrentDirectory })
        {
            for (var directory = new DirectoryInfo(start); directory is not null; directory = directory.Parent)
            {
                var corpus = Path.Combine(directory.FullName, "src", "tests", "corpus");
                var retained = Path.Combine(corpus, "js-1", "corpus.manifest");
                var parse = Path.Combine(corpus, "parse-1");

                if (!File.Exists(retained) ||
                    !File.Exists(Path.Combine(parse, "corpus.manifest")) ||
                    !Directory.Exists(Path.Combine(parse, "inputs")))
                {
                    continue;
                }

                var root = Path.Combine(parse, "inputs");

                var inputs = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                    .Where(static path => IsScriptOrModule(path))
                    .Select(path => (
                        Key: Path.GetRelativePath(root, path).Replace(Path.DirectorySeparatorChar, '/'),
                        Text: File.ReadAllText(path)))
                    .OrderBy(static input => input.Key, StringComparer.Ordinal)
                    .ToArray();

                // THE RETAINED CORPUS'S MANIFEST: a row per line, `|`-separated, its name first and its
                // mode third; every line that begins with `#` is a comment, its column header among them.
                var corpusRows = File.ReadLines(retained)
                    .Where(static line => line.Length != 0 && !line.StartsWith('#'))
                    .Select(static line => line.Split('|'))
                    .Where(static cells => cells.Length > 2 && IsWideMode(cells[2]))
                    .Select(static cells => cells[0])
                    .ToArray();

                return new Corpora(inputs, corpusRows, StoredInputs(Path.Combine(parse, "corpus.manifest")));
            }
        }

        return null;
    }

    /// <summary>
    /// The stored scripts and modules the parse corpus's manifest lists: each row whose path lies under
    /// <c>inputs/</c> and names a <c>.js</c> or <c>.mjs</c> file, keyed by its path under <c>inputs/</c>.
    /// </summary>
    /// <remarks>
    /// The manifest is tab-separated; its lines that begin with <c>#</c> are its header and comments,
    /// and its first other line names the columns, which is where <c>path</c> is found.
    /// </remarks>
    private static string[] StoredInputs(string manifest)
    {
        const string Under = "inputs/";
        var listed = new List<string>();
        var column = -1;

        foreach (var line in File.ReadLines(manifest))
        {
            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            var cells = line.Split('\t');

            if (column < 0)
            {
                column = Array.IndexOf(cells, "path");

                if (column < 0)
                {
                    return [];
                }

                continue;
            }

            if (cells.Length > column &&
                cells[column].StartsWith(Under, StringComparison.Ordinal) &&
                IsScriptOrModule(cells[column]))
            {
                listed.Add(cells[column][Under.Length..]);
            }
        }

        return [.. listed];
    }

    private static bool IsScriptOrModule(string path) =>
        path.EndsWith(".js", StringComparison.Ordinal) || path.EndsWith(".mjs", StringComparison.Ordinal);

    // ---- the order row -------------------------------------------------------------------------------

    /// <summary>
    /// The global lexical declarations, the script-declarations row and an evaluated program's lexical
    /// slots and declaration row all name the lexical names in the order the source declares them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Read off the artifact, because the artifact is the claim.</b> The row decodes the bytes the
    /// compiler wrote: the constant pool for the names, the code section for every
    /// <see cref="JsOpcode.DeclareGlobalLet"/> and <see cref="JsOpcode.DeclareGlobalConst"/> in it, the
    /// script-declarations section, and the eval scope map's shape and declaration rows.
    /// </para>
    /// <para>
    /// <b>Four facts, each read separately.</b> For two scripts compiled as one compilation: the
    /// declaring instructions name each script's lexical names, with the right kind, in its source order,
    /// the first script's before the second's; and each script's row of the script-declarations section
    /// lists its own names in the same order. For an evaluated program, sloppy and strict: its declaration
    /// row lists its lexical names in source order, and the scope map's row for its record gives them
    /// ascending slots in the same order - which is the order the lowering declared them in.
    /// </para>
    /// </remarks>
    private static (string, bool, string) TheWideLoweringDeclaresLexicalNamesInSourceOrder()
    {
        const string Name = "the wide lowering declares lexical names in source order";
        var failed = new List<string>();

        var scripts = CompileScriptProbes();

        if (!scripts.Succeeded || scripts.Artifact is null)
        {
            failed.Add("the script probes were refused: " + string.Join(" | ", scripts.Diagnostics.Take(3)));
        }
        else
        {
            ReadScripts(scripts.Artifact, failed);
        }

        foreach (var (flags, label) in new[]
        {
            (JsFormat.EvalRequestFlags.None, "sloppy"),
            (JsFormat.EvalRequestFlags.Strict, "strict"),
        })
        {
            var evaluated = CompileEvalProbe(flags);

            if (!evaluated.Succeeded || evaluated.Artifact is null)
            {
                failed.Add($"the {label} eval probe was refused: " + string.Join(" | ", evaluated.Diagnostics.Take(3)));
                continue;
            }

            ReadEval(evaluated.Artifact, label, failed);
        }

        return (
            Name,
            failed.Count == 0,
            failed.Count == 0
                ? $"two scripts' {ScriptOrder.Length + SecondScriptOrder.Length} global lexical declarations and both script-declarations rows, and an evaluated program's {EvalOrder.Length} lexical slots and declaration row, sloppy and strict, all in source order"
                : string.Join("; ", failed));
    }

    /// <summary>The two script probes' declaring instructions and script-declarations rows.</summary>
    private static void ReadScripts(byte[] bytes, List<string> failed)
    {
        if (!Artifact.TryRead(bytes, out var artifact, out var why))
        {
            failed.Add("the script probes' artifact could not be read: " + why);
            return;
        }

        try
        {
            var declared = string.Join(" ", artifact.GlobalLexicalDeclarations().Select(Render));
            var expected = string.Join(" ", ScriptOrder.Concat(SecondScriptOrder).Select(Render));

            if (!string.Equals(declared, expected, StringComparison.Ordinal))
            {
                failed.Add($"the declaring instructions name `{declared}` where the source declares `{expected}`");
            }

            var rows = artifact.ScriptDeclarationLexicalNames();

            if (rows.Count != 2 ||
                !rows[0].SequenceEqual(ScriptOrder.Select(static name => name.Name)) ||
                !rows[1].SequenceEqual(SecondScriptOrder.Select(static name => name.Name)))
            {
                failed.Add(
                    "the script-declarations rows list `" + string.Join(" | ", rows.Select(static row => string.Join(" ", row))) +
                    "` where the sources declare `" + string.Join(" ", ScriptOrder.Select(static name => name.Name)) +
                    " | " + string.Join(" ", SecondScriptOrder.Select(static name => name.Name)) + "`");
            }
        }
        catch (FormatException failure)
        {
            failed.Add("the script probes' artifact could not be read: " + failure.Message);
        }
    }

    /// <summary>One eval probe's declaration row and the slots its record gives its lexical names.</summary>
    private static void ReadEval(byte[] bytes, string label, List<string> failed)
    {
        if (!Artifact.TryRead(bytes, out var artifact, out var why))
        {
            failed.Add($"the {label} eval probe's artifact could not be read: " + why);
            return;
        }

        try
        {
            var rows = artifact.EvalDeclarationLexicalNames();

            if (rows.Count != 1 || !rows[0].SequenceEqual(EvalOrder))
            {
                failed.Add(
                    $"the {label} evaluated program's declaration row lists `" +
                    string.Join(" | ", rows.Select(static row => string.Join(" ", row))) +
                    "` where the source declares `" + string.Join(" ", EvalOrder) + "`");
            }

            // THE SLOTS, read from the scope map's row for the evaluated program's own record: the one
            // row of the eval kind. Its names are written sorted, so the order is the slots'.
            var slotted = artifact.EvalScopeSlots()
                .Where(static name => Array.IndexOf(EvalOrder, name.Name) >= 0)
                .OrderBy(static name => name.Slot)
                .Select(static name => name.Name)
                .ToArray();

            if (!slotted.SequenceEqual(EvalOrder))
            {
                failed.Add(
                    $"the {label} evaluated program's record gives its lexical names slots in the order `" +
                    string.Join(" ", slotted) + "` where the source declares `" + string.Join(" ", EvalOrder) + "`");
            }
        }
        catch (FormatException failure)
        {
            failed.Add($"the {label} eval probe's artifact could not be read: " + failure.Message);
        }
    }

    private static string Render((string Name, bool Constant) name) =>
        (name.Constant ? "const " : "let ") + name.Name;

    private static JsCompilation CompileScriptProbes() =>
        JsCompiler.Compile(
        [
            new JsScriptUnit("first", ScriptProbe, SliceParseOptions.Script),
            new JsScriptUnit("second", SecondScriptProbe, SliceParseOptions.Script),
        ]);

    private static JsCompilation CompileEvalProbe(JsFormat.EvalRequestFlags flags) =>
        JsCompiler.Compile([new JsScriptUnit("eval", EvalProbe, SliceParseOptions.Eval(flags))]);

    private static string Sha256(byte[] bytes) =>
        Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(bytes));

    /// <summary>
    /// The parts of one version-2 artifact the order row reads, decoded from its bytes and from
    /// nothing else.
    /// </summary>
    /// <remarks>
    /// It reads what the format writes - <see cref="JavaScriptArtifactWriter"/>'s framing and
    /// <see cref="JsArtifactWriter"/>'s section bodies - and refuses anything else by name rather than
    /// guessing, so a change to the format shows as an unreadable artifact here and not as an order.
    /// </remarks>
    private sealed class Artifact
    {
        private readonly Dictionary<uint, byte[]> sections;
        private readonly string?[] names;

        private Artifact(Dictionary<uint, byte[]> sections, string?[] names)
        {
            this.sections = sections;
            this.names = names;
        }

        internal static bool TryRead(byte[] bytes, out Artifact artifact, out string why)
        {
            artifact = null!;

            try
            {
                var reader = new Reader(bytes);

                if (!reader.Bytes(JavaScriptFormat.Magic.Length).SequenceEqual(JavaScriptFormat.Magic.ToArray()))
                {
                    why = "it does not begin with the format's magic";
                    return false;
                }

                _ = reader.VarUInt();
                _ = reader.Bytes(reader.Count());

                var sections = new Dictionary<uint, byte[]>();
                var count = reader.Count();

                for (var index = 0; index < count; index++)
                {
                    var kind = (uint)reader.Index();
                    var body = reader.Bytes(reader.Count());

                    if (!sections.TryAdd(kind, body))
                    {
                        why = $"section {kind} appears twice";
                        return false;
                    }
                }

                if (!sections.TryGetValue((uint)JsFormat.SectionKind.Constants, out var constants))
                {
                    why = "it has no constant pool";
                    return false;
                }

                var pool = new Reader(constants);
                var names = new string?[pool.Count()];

                for (var index = 0; index < names.Length; index++)
                {
                    switch ((JsFormat.ConstantTag)pool.Byte())
                    {
                        case JsFormat.ConstantTag.Undefined or JsFormat.ConstantTag.Null:
                            break;
                        case JsFormat.ConstantTag.Boolean:
                            _ = pool.Byte();
                            break;
                        case JsFormat.ConstantTag.Number:
                            _ = pool.Bytes(8);
                            break;
                        case JsFormat.ConstantTag.InternedName:
                            names[index] = JsFormat.DecodeText(pool.Bytes(pool.Count()));
                            break;
                        case JsFormat.ConstantTag.String:
                            _ = pool.Bytes(pool.Count());
                            break;
                        case JsFormat.ConstantTag.BigInt:
                            _ = pool.Byte();
                            _ = pool.Bytes(pool.Count());
                            break;
                        default:
                            why = $"constant {index} carries a tag this reader does not know";
                            return false;
                    }
                }

                artifact = new Artifact(sections, names);
                why = string.Empty;
                return true;
            }
            catch (FormatException failure)
            {
                why = failure.Message;
                return false;
            }
        }

        /// <summary>
        /// Every <see cref="JsOpcode.DeclareGlobalLet"/> and <see cref="JsOpcode.DeclareGlobalConst"/>
        /// in the code section, in the order the instructions stand, with the name each declares.
        /// </summary>
        /// <remarks>
        /// The code section is every code unit's instructions back to back, so a walk from its first
        /// byte by each instruction's width meets every instruction once. Only a script body's prologue
        /// declares a global lexical name, so the walk needs no unit boundaries.
        /// </remarks>
        internal List<(string Name, bool Constant)> GlobalLexicalDeclarations()
        {
            var declared = new List<(string Name, bool Constant)>();
            var code = sections.TryGetValue((uint)JsFormat.SectionKind.Code, out var body) ? body : [];

            for (var at = 0; at < code.Length;)
            {
                var opcode = (JsOpcode)code[at];

                if (!JsOpcodes.IsDefined(code[at]) || at + JsOpcodes.InstructionWidth(opcode) > code.Length)
                {
                    throw new FormatException($"the byte at code offset {at} begins no whole instruction");
                }

                if (opcode is JsOpcode.DeclareGlobalLet or JsOpcode.DeclareGlobalConst)
                {
                    declared.Add((Name(code[at + 1] | (code[at + 2] << 8)), opcode == JsOpcode.DeclareGlobalConst));
                }

                at += JsOpcodes.InstructionWidth(opcode);
            }

            return declared;
        }

        /// <summary>Each script-declarations row's lexical names, row by row.</summary>
        internal List<string[]> ScriptDeclarationLexicalNames()
        {
            var rows = new List<string[]>();

            if (!sections.TryGetValue((uint)JsFormat.SectionKind.ScriptDeclarations, out var body))
            {
                return rows;
            }

            var reader = new Reader(body);
            var count = reader.Count();

            for (var row = 0; row < count; row++)
            {
                _ = reader.VarUInt();
                rows.Add(Names(reader));

                for (var list = 0; list < 3; list++)
                {
                    _ = Names(reader);
                }
            }

            return rows;
        }

        /// <summary>Each eval declaration row's lexical names, row by row.</summary>
        internal List<string[]> EvalDeclarationLexicalNames()
        {
            var rows = new List<string[]>();
            var reader = EvalScopes(out _);

            if (reader is null)
            {
                return rows;
            }

            var count = reader.Count();

            for (var row = 0; row < count; row++)
            {
                _ = reader.VarUInt();
                _ = reader.Byte();
                _ = reader.Byte();
                _ = Names(reader);
                rows.Add(Names(reader));

                for (var list = 0; list < 3; list++)
                {
                    _ = Names(reader);
                }
            }

            return rows;
        }

        /// <summary>The names and slots of every scope-map row of the eval kind.</summary>
        internal List<(string Name, ulong Slot)> EvalScopeSlots()
        {
            _ = EvalScopes(out var slots);
            return slots;
        }

        /// <summary>
        /// Reads the eval scope map's shapes and sites, keeping the eval rows' names and slots, and
        /// answers a reader standing at its declaration rows.
        /// </summary>
        private Reader? EvalScopes(out List<(string Name, ulong Slot)> slots)
        {
            slots = [];

            if (!sections.TryGetValue((uint)JsFormat.SectionKind.EvalScopes, out var body))
            {
                return null;
            }

            var reader = new Reader(body);
            var shapes = reader.Count();

            for (var shape = 0; shape < shapes; shape++)
            {
                var kind = (JsFormat.EvalScopeKind)reader.Byte();
                _ = reader.VarUInt();
                var count = reader.Count();

                for (var name = 0; name < count; name++)
                {
                    var constant = reader.Index();
                    var slot = reader.VarUInt();
                    _ = reader.Byte();

                    if (kind == JsFormat.EvalScopeKind.Eval)
                    {
                        slots.Add((Name(constant), slot));
                    }
                }
            }

            var sites = reader.Count();

            for (var site = 0; site < sites; site++)
            {
                _ = reader.VarUInt();
                _ = reader.VarUInt();
                _ = reader.VarUInt();
                _ = reader.VarUInt();
                _ = reader.Byte();
            }

            return reader;
        }

        private string[] Names(Reader reader)
        {
            var read = new string[reader.Count()];

            for (var index = 0; index < read.Length; index++)
            {
                read[index] = Name(reader.Index());
            }

            return read;
        }

        private string Name(int constant) =>
            constant >= 0 && constant < names.Length && names[constant] is { } name
                ? name
                : throw new FormatException($"constant {constant} is not an interned name");
    }

    /// <summary>A cursor over one byte run, refusing to read past its end.</summary>
    /// <remarks>
    /// <b>Every failure is a <see cref="FormatException"/></b>, which is all the order row catches, so
    /// a malformed artifact fails that row by name and never stops the run. A count or a length is
    /// therefore bounded by the bytes left before anything is allocated for it - each item a count
    /// announces takes at least one byte - and an index by what an <see cref="int"/> holds, rather
    /// than cast and left to overflow.
    /// </remarks>
    private sealed class Reader(byte[] bytes)
    {
        private int at;

        internal byte Byte() =>
            at < bytes.Length ? bytes[at++] : throw new FormatException("a run ended early");

        /// <summary>A count of items, or a length in bytes, that the rest of the run can hold.</summary>
        internal int Count()
        {
            var count = VarUInt();

            return count <= (ulong)(bytes.Length - at)
                ? (int)count
                : throw new FormatException($"a count of {count} runs past the {bytes.Length - at} bytes left");
        }

        /// <summary>An index, which the reader looks up and range-checks where it is used.</summary>
        internal int Index()
        {
            var index = VarUInt();

            return index <= int.MaxValue
                ? (int)index
                : throw new FormatException($"an index of {index} is past any this reader holds");
        }

        internal byte[] Bytes(int count)
        {
            if (count < 0 || count > bytes.Length - at)
            {
                throw new FormatException("a run ended early");
            }

            var read = bytes.AsSpan(at, count).ToArray();
            at += count;
            return read;
        }

        internal ulong VarUInt()
        {
            var value = 0ul;

            for (var shift = 0; shift < 64; shift += 7)
            {
                var next = Byte();
                value |= (ulong)(next & 0x7F) << shift;

                if ((next & 0x80) == 0)
                {
                    return value;
                }
            }

            throw new FormatException("a variable-length integer runs past sixty-four bits");
        }
    }
}
