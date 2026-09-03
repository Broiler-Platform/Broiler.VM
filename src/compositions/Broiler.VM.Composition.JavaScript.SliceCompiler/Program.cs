using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;

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

            Console.WriteLine("usage: --write <directory> | --checks [--verbose] | --closure");

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

        manifest.Append("# broiler.javascript.slice retained corpus, format version 1").Append(Eol);
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

    /// <summary>Runs the claims that need a neighbour profile, and the claims about the front end.</summary>
    private static int RunChecks(bool verbose)
    {
        var checks = CrossProfileChecks.Run().Concat(SourceFrontEndChecks.Run()).ToArray();
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
