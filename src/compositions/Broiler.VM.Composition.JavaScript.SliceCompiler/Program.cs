using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The producer composition: it lowers the slice programs and writes the retained corpus.
/// </summary>
/// <remarks>
/// <para>
/// Its closure carries a lowering, which is what makes it a different composition from its
/// execution-only sibling rather than a second mode of one. No publish of this root is evidence
/// for that one, and the two are separate projects so that the distinction is a reference set
/// rather than a promise.
/// </para>
/// <para>
/// <b>It verifies nothing and judges nothing.</b> It writes bytes and the expectations recorded
/// against them; whether those expectations hold is the execution-only root's replay to answer.
/// A producer that checked its own output would be a verifier with a schedule attached, and the
/// two halves agreeing because one asked the other is exactly the shape the corpus exists to
/// avoid.
/// </para>
/// </remarks>
internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length >= 2 && string.Equals(args[0], "--write", StringComparison.Ordinal))
            {
                return Write(args[1]);
            }

            if (args.Contains("--closure", StringComparer.Ordinal))
            {
                return ReportClosure();
            }

            if (args.Contains("--checks", StringComparer.Ordinal))
            {
                return RunChecks(args.Contains("--verbose", StringComparer.Ordinal));
            }

            // WHAT THE REALM ADMITS, ASKED OF THE REALM. A document's list of absent globals is
            // checked against the answer this mode publishes rather than against another document,
            // which is the whole reason the mode exists: bundle JS-4-001's list was true and not
            // exhaustive, and a later reader mistaking one for the other is how a gap survives.
            if (args.Contains("--globals", StringComparer.Ordinal))
            {
                return RealmGlobals.Run(Argument(args, "--write"));
            }

            if (args.Length >= 2 && string.Equals(args[0], "--census", StringComparison.Ordinal))
            {
                return Census(args[1..]);
            }

            // THE SOURCE SURFACE'S OWN SESSION, and it lives here rather than beside the
            // artifact one because it needs a compiler. The execution-only root carries no
            // lowering by construction, so a session over source could not run there at all -
            // which is the same reason every claim in SourceFrontEndChecks is in this root.
            if (args.Length >= 2 && string.Equals(args[0], "--fuzz", StringComparison.Ordinal))
            {
                return SourceFuzzing.Run(
                    args[1],
                    Unsigned(Argument(args, "--seed"), 1),
                    (int)Unsigned(Argument(args, "--iterations"), 2000));
            }

            Console.WriteLine(
                "usage: --write <directory> | --checks [--verbose] | --closure | --globals " +
                "[--write <file>] | " +
                "--census <directory> [<directory> ...] | " +
                "--fuzz <source directory> [--seed <n>] [--iterations <n>]");

            return 2;
        }
        catch (Exception failure)
        {
            Console.WriteLine(
                $"broiler-js-slice-compiler: unhandled {failure.GetType().Name}: {failure.Message}");

            return 2;
        }
    }

    /// <summary>Lowers every corpus entry, writes its bytes, and records what it is expected to do.</summary>
    /// <remarks>
    /// Each entry is one file plus one manifest row carrying the SHA-256 of that file. The replay
    /// re-hashes what it reads, so a corpus whose bytes changed without its manifest changing is a
    /// failure rather than a quiet drift - which is the whole reason the hash is recorded rather
    /// than the file trusted.
    /// </remarks>
    private static int Write(string directory)
    {
        Directory.CreateDirectory(directory);

        var entries = CorpusBuilder.Build();
        var manifest = new System.Text.StringBuilder();

        // LF, explicitly, on every platform. The manifest is a retained artefact whose bytes are
        // hashed into an evidence bundle and whose repository form .gitattributes pins to LF, so a
        // producer that emitted the platform's newline would write a file that differs from the one
        // a fresh checkout holds - and the bundle's hash would then record the machine rather than
        // the corpus.
        const string Eol = "\n";

        manifest.Append(
                "# broiler.javascript retained corpus, feature manifests slice and wide, " +
                "format versions 1 and 2")
            .Append(Eol);
        manifest
            .Append("# name|sha256|mode|outcome|reason|diagnostic|completion|position|dimension|scope")
            .Append(Eol);
        manifest
            .Append("# position is sectionIndex:byteOffset:coordinate0:coordinate1, or - where the row ")
            .Append("pins no position")
            .Append(Eol);
        manifest
            .Append("# dimension and scope are the budget dimension a resource exhaustion named and ")
            .Append("the scope that refused, or - where the row is not an exhaustion")
            .Append(Eol);

        foreach (var entry in entries)
        {
            var path = Path.Combine(directory, entry.Name + ".bjsb");
            File.WriteAllBytes(path, entry.Bytes);

            manifest.Append(entry.Name).Append('|')
                .Append(Sha256(entry.Bytes)).Append('|')
                .Append(entry.Mode).Append('|')
                .Append(entry.Outcome).Append('|')
                .Append(entry.Reason).Append('|')
                .Append(entry.DiagnosticCode.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Append('|')
                .Append(entry.Completion).Append('|')
                .Append(entry.Position).Append('|')
                .Append(entry.Dimension).Append('|')
                .Append(entry.Scope)
                .Append(Eol);
        }

        File.WriteAllText(Path.Combine(directory, "corpus.manifest"), manifest.ToString());

        WriteSourceCorpus(directory);

        var controls = entries.Count(entry => entry.Outcome == "Normal");
        var compiled = SliceSourcePrograms.Accepted.Length;

        Console.WriteLine(
            $"broiler-js-slice-compiler: wrote {entries.Length} entries " +
            $"({controls} of them well-formed controls, {compiled} of those compiled from " +
            $"retained source) to {directory}");

        return 0;
    }

    /// <summary>
    /// Writes the source corpus: the text of every program the front end was asked about, and one
    /// manifest recording what each is expected to answer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the retained artefact the seam half of the diagnostic registry is bound to.</b>
    /// The artifact corpus binds every <c>core-result</c> code to a named entry, and the binding is
    /// what stops the registry from being a file that agrees with itself; a seam code has no
    /// artifact and would have had no such binding. This manifest is the equivalent: one row per
    /// source, naming the code it must be refused with, read off disk by the rule and by nothing
    /// that produced it.
    /// </para>
    /// <para>
    /// <b>Three refused sources are recorded and not retained</b>, and the manifest says which.
    /// They are the format's own ceilings - more locals than the frame admits, more constants than
    /// the pool admits, a deeper operand stack than the format admits - and reaching one takes tens
    /// of thousands of declarations. A file of that size is a file nobody reads, and a generator of
    /// three lines is something anybody can check, so the generator is the retained thing and the
    /// manifest names it.
    /// </para>
    /// </remarks>
    private static void WriteSourceCorpus(string directory)
    {
        const string Eol = "\n";
        const int RetainedSourceLimit = 4_096;

        var root = Path.Combine(directory, "source");
        var accepted = Path.Combine(root, "accepted");
        var refused = Path.Combine(root, "refused");

        Directory.CreateDirectory(accepted);
        Directory.CreateDirectory(refused);

        var manifest = new System.Text.StringBuilder();
        manifest.Append("# broiler.javascript.slice retained source corpus").Append(Eol);
        manifest.Append("# kind|name|sha256|answer|retained").Append(Eol);
        manifest
            .Append("# answer is the completion value an accepted source runs to, or the ")
            .Append("embedder-seam diagnostic a refused source is refused with")
            .Append(Eol);
        manifest
            .Append("# retained is `file` where the source text is beside this manifest, and ")
            .Append("`generated` where it is produced by a named member of SliceSourcePrograms")
            .Append(Eol);

        foreach (var program in SliceSourcePrograms.Accepted)
        {
            var text = Normalise(program.Source);
            File.WriteAllText(Path.Combine(accepted, program.Name + ".js"), text);

            manifest.Append("accepted|").Append(program.Name).Append('|')
                .Append(Sha256(System.Text.Encoding.UTF8.GetBytes(text))).Append('|')
                .Append(program.Completion).Append("|file").Append(Eol);
        }

        foreach (var program in SliceSourcePrograms.Refused)
        {
            var text = Normalise(program.Source);
            var retain = text.Length <= RetainedSourceLimit;

            if (retain)
            {
                File.WriteAllText(Path.Combine(refused, program.Name + ".js"), text);
            }

            manifest.Append("refused|").Append(program.Name).Append('|')
                .Append(Sha256(System.Text.Encoding.UTF8.GetBytes(text))).Append('|')
                .Append(program.Code).Append('|')
                .Append(retain ? "file" : "generated").Append(Eol);
        }

        // THE MODULE-GOAL REFUSALS ARE RETAINED BESIDE THE OTHERS AND UNDER THE SAME KIND, because
        // the registry's `source` reachability is a claim that a named retained source is refused
        // with a code, and it is the same claim whichever goal the source was presented under. The
        // extension is what records the goal: a `.mjs` here is read back as module source, which is
        // the same convention the CLI composition applies to a path it is handed.
        foreach (var program in SliceSourcePrograms.RefusedModules)
        {
            var text = Normalise(program.Source);
            var extension = program.Options.Goal == SliceGoal.Module ? ".mjs" : ".js";
            File.WriteAllText(Path.Combine(refused, program.Name + extension), text);

            manifest.Append("refused|").Append(program.Name).Append('|')
                .Append(Sha256(System.Text.Encoding.UTF8.GetBytes(text))).Append('|')
                .Append(program.Code).Append("|file").Append(Eol);
        }

        File.WriteAllText(Path.Combine(root, "source.manifest"), manifest.ToString());
    }

    /// <summary>LF endings and one trailing newline, on every platform.</summary>
    /// <remarks>
    /// The same reason the corpus manifest states: these files are hashed into an evidence bundle
    /// and their repository form is pinned to LF, so a producer emitting the platform's newline
    /// would write a file that differs from the one a fresh checkout holds.
    /// </remarks>
    private static string Normalise(string source) =>
        source.Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd('\n') + "\n";

    /// <summary>
    /// Counts what the JavaScript under the given directories is made of.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This measures the distance between this profile and real code, and it advances
    /// nothing.</b> It is not the conformance oracle: roadmap section 14's harness needs a pinned
    /// suite revision, content-independent sharding, a self-check that proves a failing test comes
    /// back as a failure, per-host-mode totals and a ratchet, and this has none of them and claims
    /// none of them. What it produces is a ranked census of constructs, which is an input to a
    /// scope decision rather than a score.
    /// </para>
    /// <para>
    /// <b>It reads whatever directory it is pointed at and ingests nothing.</b> No third-party
    /// source enters this repository through it. The suite and the benchmark whose measurement
    /// motivated it are separately licensed material whose retrieval, hashing and archiving is a
    /// human action that has not happened, so this takes a path and keeps no copy.
    /// </para>
    /// </remarks>
    private static int Census(string[] directories)
    {
        var paths = new List<string>();

        foreach (var directory in directories)
        {
            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"broiler-js-slice-compiler: no directory at {directory}");

                return 2;
            }

            paths.AddRange(Directory.EnumerateFiles(directory, "*.js", SearchOption.AllDirectories));
        }

        paths.Sort(StringComparer.Ordinal);

        var census = SliceConstructCensus.Take(paths.Select(File.ReadAllText));

        Console.WriteLine(
            $"# broiler.javascript.slice construct census over {census.FilesRead} files");
        Console.WriteLine(
            $"# {census.FilesParsed} parsed, " +
            $"{census.FilesRead - census.FilesParsed - census.FilesThatFaulted} did not, " +
            $"and {census.FilesCompiled} contain nothing outside the declared manifest");

        // A fault is a defect in the front end rather than a property of a source, so it is
        // reported on its own line and never folded into the parse failures.
        if (census.FilesThatFaulted > 0)
        {
            Console.WriteLine(
                $"# {census.FilesThatFaulted} threw out of the front end, which is a defect in it");
        }
        Console.WriteLine("# construct|files|occurrences");

        foreach (var entry in census.Files.OrderByDescending(entry => entry.Value)
            .ThenBy(entry => entry.Key.ToString(), StringComparer.Ordinal))
        {
            var occurrences = census.Occurrences.TryGetValue(entry.Key, out var total) ? total : 0;

            Console.WriteLine($"{entry.Key}|{entry.Value}|{occurrences}");
        }

        if (census.ParseFailures.Count > 0)
        {
            Console.WriteLine("# the sources that did not parse, by the first refusal each got");

            foreach (var failure in census.ParseFailures.OrderByDescending(entry => entry.Value))
            {
                Console.WriteLine($"{failure.Key}|{failure.Value}");
            }
        }

        Curve(paths, census);
        return 0;
    }

    /// <summary>
    /// How far the manifest has to grow before any of these files becomes admissible.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The ranked list above says what is needed; it does not say what buying any of it would
    /// get.</b> A construct at the top of that list appears in nearly every file, and admitting it
    /// alone typically admits none of them, because those same files each need eleven other things
    /// as well. The number a scope decision actually wants is the one below: admit the first k
    /// constructs of the ranking, and how many whole files can this profile then compile?
    /// </para>
    /// <para>
    /// <b>It is a curve along ONE ranking and not a smallest set.</b> The order is the census's own
    /// - by how many files need a construct - which is a reasonable order to buy things in and is
    /// not the cheapest set that admits some file. Reading it as "the minimum needed" would
    /// overstate what the ranking proves; the distribution printed beside it is what bounds the
    /// nearest file.
    /// </para>
    /// <para>
    /// <b>It re-reads each source rather than growing the census record.</b> Per-file need sets are
    /// a list as long as the corpus - fifty thousand of them for a real suite - and the census
    /// returns aggregates precisely so that it does not carry one. Paying a second parse here keeps
    /// that property where it belongs.
    /// </para>
    /// </remarks>
    private static void Curve(IReadOnlyList<string> paths, SliceCensus census)
    {
        var needs = new List<HashSet<SliceConstructKind>>();

        foreach (var path in paths)
        {
            var one = SliceConstructCensus.Take([File.ReadAllText(path)]);

            // A source that did not parse has no need set - not an empty one. Counting it as
            // needing nothing would put every unparsed file in the admissible column.
            if (one.FilesParsed == 1)
            {
                needs.Add([.. one.Files.Keys]);
            }
        }

        Console.WriteLine("# how many constructs a file needs, and how many files need that many");
        Console.WriteLine("# constructs-needed|files");

        foreach (var group in needs.GroupBy(need => need.Count).OrderBy(group => group.Key))
        {
            Console.WriteLine($"{group.Key}|{group.Count()}");
        }

        var ranked = census.Files
            .OrderByDescending(entry => entry.Value)
            .ThenBy(entry => entry.Key.ToString(), StringComparer.Ordinal)
            .Select(entry => entry.Key)
            .ToArray();

        Console.WriteLine("# admitting the ranked constructs in order, and what each one buys");
        Console.WriteLine("# rank|construct|files-newly-admissible|files-admissible-in-total");

        var admitted = new HashSet<SliceConstructKind>();
        var previous = needs.Count(need => need.Count == 0);

        for (var rank = 0; rank < ranked.Length; rank++)
        {
            admitted.Add(ranked[rank]);
            var now = needs.Count(need => need.IsSubsetOf(admitted));

            // Only the ranks that move the number. A row per construct would be a column of
            // repeated zeroes with the four rows that matter buried in it.
            if (now != previous)
            {
                Console.WriteLine($"{rank + 1}|{ranked[rank]}|{now - previous}|{now}");
            }

            previous = now;
        }

        Console.WriteLine(
            $"# {previous} of {needs.Count} parsed files are admissible once all " +
            $"{ranked.Length} ranked constructs are admitted");
    }

    /// <summary>Runs the claims that need a neighbour profile, and the claims about the front end.</summary>
    private static int RunChecks(bool verbose)
    {
        var checks = CrossProfileChecks.Run()
            .Concat(SourceFrontEndChecks.Run())
            .Concat(SurfaceChecks.Run())
            .ToArray();
        var failed = 0;

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

        Console.WriteLine(
            failed == 0
                ? $"broiler-js-slice-compiler: {checks.Length} checks passed"
                : $"broiler-js-slice-compiler: {failed} of {checks.Length} checks FAILED");

        return failed == 0 ? 0 : 1;
    }

    private static string Sha256(byte[] bytes) =>
        Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(bytes));

    /// <summary>
    /// Prints what this composition is: the profile it names and the fact that it carries a
    /// lowering.
    /// </summary>
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

    private static ulong Unsigned(string? text, ulong fallback) =>
        text is not null && ulong.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var value)
            ? value
            : fallback;

    private static int ReportClosure()
    {
        Console.WriteLine($"# broiler-vm-composition core-contract-version={VmCoreContract.Version}");
        Console.WriteLine("composition Broiler.VM.Composition.JavaScript.SliceCompiler");
        Console.WriteLine("label narrow-runtime-compiler-shaped");
        Console.WriteLine("carries-lowering yes");
        Console.WriteLine("profiles 1");
        Console.WriteLine(
            string.Join(
                ' ',
                "profile",
                JavaScriptProfile.Id,
                JavaScriptProfile.Descriptor.PackageIdentity.PackageId,
                JavaScriptProfile.Descriptor.DescriptorRevision,
                JavaScriptProfile.Descriptor.HostCapabilityDescriptors.Length));

        return 0;
    }
}
