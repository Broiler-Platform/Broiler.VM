using Broiler.VM;
using Broiler.VM.Profile.JavaScript;

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

        var controls = entries.Count(entry => entry.Outcome == "Normal");

        Console.WriteLine(
            $"broiler-js-slice-compiler: wrote {entries.Length} entries " +
            $"({controls} of them well-formed controls) to {directory}");

        return 0;
    }

    /// <summary>Runs the claims that need a second profile in the catalog.</summary>
    private static int RunChecks(bool verbose)
    {
        var checks = CrossProfileChecks.Run();
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
                ? $"broiler-js-slice-compiler: {checks.Length} cross-profile checks passed"
                : $"broiler-js-slice-compiler: {failed} of {checks.Length} cross-profile checks FAILED");

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
