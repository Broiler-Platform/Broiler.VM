using System.Security.Cryptography;
using System.Text;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// A suite's identity: what it is called and which immutable revision of it was read.
/// </summary>
/// <param name="Name">The suite's name. Part of a floor's identity, so it is never blank.</param>
/// <param name="Revision">The revision, lower-case hexadecimal. Empty where nothing is pinned.</param>
/// <remarks>
/// <b>A branch name is not a pin, and neither is a directory path.</b> What makes a revision a pin
/// is that reading the suite twice either produces it twice or says so, which is why
/// <see cref="Suite.Resolve"/> re-derives it from the files it just read and compares, rather than
/// trusting what a caller passed.
/// </remarks>
internal sealed record SuiteRevision(string Name, string Revision)
{
    /// <summary>Whether a revision was pinned at all.</summary>
    internal bool IsPinned => Revision.Length != 0;

    /// <summary>The identity a report and a floor both write.</summary>
    public override string ToString() =>
        Name + "@" + (Revision.Length == 0 ? "unpinned" : Revision);
}

/// <summary>One file the suite read, and what it hashed to.</summary>
internal sealed record SuiteFile(string Path, string Sha256);

/// <summary>
/// Reading a suite: discovery, the pin, and the digest that verifies it.
/// </summary>
/// <remarks>
/// <para>
/// <b>There is no third-party suite in this checkout and this type does not fetch one.</b> It
/// takes a directory that already exists. Roadmap section 3's pin - retrieve, hash, archive - is a
/// human action nobody has performed, and what a run does when handed a directory with no revision
/// is report <see cref="ConfigurationFailure.MissingSuiteRevision"/>, which is a failure of that
/// run and not a smaller total.
/// </para>
/// <para>
/// <b>The suite this harness reads today is its own.</b> That is not a placeholder for a real one:
/// roadmap section 14 builds the harness against the smallest scoring target that exists rather
/// than after the language it will eventually score, and a fixture tree that drives this profile's
/// lowering, verifier and executor is that target. The reader is the one a pinned suite will be
/// pointed at, so the day a revision is retrieved nothing here is replaced.
/// </para>
/// </remarks>
internal static class Suite
{
    /// <summary>The file a suite directory declares its own revision in.</summary>
    internal const string PinFileName = "suite.pin";

    /// <summary>The extension a raw test's artifact carries.</summary>
    internal const string ArtifactExtension = ".bjsb";

    /// <summary>The extension a raw test's metadata sidecar carries.</summary>
    internal const string SidecarExtension = ".meta";

    /// <summary>Normalizes a path to the one spelling a shard hash and a report may use.</summary>
    /// <remarks>
    /// Sharding is a hash of this string, so two spellings of one path would put one test in two
    /// shards on two machines. Backslashes become slashes and a leading <c>./</c> is dropped;
    /// nothing else is touched, because a case fold would merge two files a case-sensitive
    /// filesystem keeps apart.
    /// </remarks>
    internal static string Normalize(string path)
    {
        var normalized = path.Replace('\\', '/');

        while (normalized.StartsWith("./", StringComparison.Ordinal))
        {
            normalized = normalized[2..];
        }

        return normalized.TrimStart('/');
    }

    /// <summary>The lower-case hexadecimal SHA-256 of some bytes.</summary>
    internal static string Sha256(ReadOnlySpan<byte> bytes) =>
        Convert.ToHexStringLower(SHA256.HashData(bytes));

    /// <summary>
    /// The revision a set of read files amounts to: one digest over every path and its content.
    /// </summary>
    /// <remarks>
    /// Over the PAIRS rather than over the concatenated bytes, so that renaming a file moves the
    /// revision. A digest of content alone is unchanged by a rename, and a rename is exactly what
    /// moves a test between shards.
    /// </remarks>
    internal static string Digest(IEnumerable<SuiteFile> files)
    {
        var text = new StringBuilder();

        foreach (var file in files.OrderBy(static file => file.Path, StringComparer.Ordinal))
        {
            text.Append(Normalize(file.Path)).Append('\n').Append(file.Sha256).Append('\n');
        }

        return Sha256(Encoding.UTF8.GetBytes(text.ToString()));
    }

    /// <summary>
    /// Resolves a suite's revision: what the pin file declares, checked against what was read.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Three answers, deliberately different. A directory with no pin file resolves to an UNPINNED
    /// revision, which every caller that needs one reports as
    /// <see cref="ConfigurationFailure.MissingSuiteRevision"/>. A pin that agrees with the files
    /// resolves to itself. A pin that disagrees is refused rather than replaced: a suite that moved
    /// under a pin somebody wrote is not a suite whose pin can be believed.
    /// </para>
    /// <para>
    /// The last case is the one worth having. It is how a fixture edited without its pin being
    /// updated is caught, and it is the same discipline the retained corpus applies to its bytes.
    /// </para>
    /// </remarks>
    internal static SuiteRevision Resolve(
        string root,
        string defaultName,
        IEnumerable<SuiteFile> files,
        out string failure)
    {
        var observed = Digest(files);
        var pinPath = Path.Combine(root, PinFileName);

        if (!File.Exists(pinPath))
        {
            failure = string.Empty;
            return new SuiteRevision(defaultName, string.Empty);
        }

        var name = defaultName;
        var declared = string.Empty;

        foreach (var line in File.ReadAllLines(pinPath))
        {
            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var parts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                continue;
            }

            if (string.Equals(parts[0], "suite", StringComparison.Ordinal))
            {
                name = parts[1];
            }
            else if (string.Equals(parts[0], "revision", StringComparison.Ordinal))
            {
                declared = parts[1];
            }
        }

        if (declared.Length == 0)
        {
            failure = $"{PinFileName} in {root} declares no revision";
            return new SuiteRevision(name, string.Empty);
        }

        if (!string.Equals(declared, observed, StringComparison.Ordinal))
        {
            failure =
                $"{PinFileName} in {root} pins {declared} and the files read amount to {observed}: " +
                "the suite moved under its pin";

            return new SuiteRevision(name, string.Empty);
        }

        failure = string.Empty;
        return new SuiteRevision(name, declared);
    }

    /// <summary>
    /// Every file the suite holds, with its hash: the input the pin is computed over.
    /// </summary>
    /// <remarks>
    /// Everything under the root and not only the scored tests, so that editing a self-check
    /// fixture, a known-incorrect entry or a raw test's sidecar moves the revision too. A pin over
    /// part of a suite is a pin somebody can edit around.
    /// </remarks>
    internal static IReadOnlyList<SuiteFile> Files(string root)
    {
        var files = new List<SuiteFile>();

        foreach (var file in Directory
                     .EnumerateFiles(root, "*", SearchOption.AllDirectories)
                     .OrderBy(static path => path, StringComparer.Ordinal))
        {
            var relative = Normalize(Path.GetRelativePath(root, file));

            // The pin is not part of what it pins. Including it would make the digest a function
            // of itself, which no value can satisfy.
            if (string.Equals(relative, PinFileName, StringComparison.Ordinal))
            {
                continue;
            }

            files.Add(new SuiteFile(relative, Sha256(File.ReadAllBytes(file))));
        }

        return files;
    }

    /// <summary>
    /// Reads every test under a directory, in path order, with the unreadable ones named.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Two shapes and one reader. A <c>.js</c> file is a source test and carries its own metadata
    /// block; a <c>.bjsb</c> artifact is a raw test and carries a <c>.bjsb.meta</c> sidecar with
    /// the same block in it. One metadata format for both is what keeps a raw test as readable as
    /// a source one - and it is what lets the raw flag be checked rather than assumed: the flag is
    /// required exactly where bytes exist and refused everywhere else.
    /// </para>
    /// <para>
    /// A file that is not a readable test is <b>reported and not skipped</b>. Discovery that
    /// silently dropped what it could not parse would make a mis-authored fixture
    /// indistinguishable from a fixture nobody wrote, and the candidate count - which the merge
    /// uses to prove the shards covered the selection - would move without anything saying why.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<ConformanceTest> Read(string directory, out IReadOnlyList<string> unreadable)
    {
        var tests = new List<ConformanceTest>();
        var failures = new List<string>();

        if (!Directory.Exists(directory))
        {
            unreadable = [$"{directory} does not exist, so it holds no test"];
            return tests;
        }

        foreach (var file in Directory
                     .EnumerateFiles(directory, "*", SearchOption.AllDirectories)
                     .OrderBy(static path => path, StringComparer.Ordinal))
        {
            var relative = Normalize(Path.GetRelativePath(directory, file));

            if (relative.EndsWith(".js", StringComparison.Ordinal))
            {
                if (TestMetadata.TryRead(relative, File.ReadAllText(file), null, out var test, out var why))
                {
                    tests.Add(test);
                }
                else
                {
                    failures.Add(why);
                }

                continue;
            }

            if (!relative.EndsWith(ArtifactExtension + SidecarExtension, StringComparison.Ordinal))
            {
                continue;
            }

            var artifactPath = file[..^SidecarExtension.Length];
            var artifactRelative = Normalize(Path.GetRelativePath(directory, artifactPath));

            if (!File.Exists(artifactPath))
            {
                failures.Add($"{relative} describes {artifactRelative}, which is not beside it");
                continue;
            }

            if (TestMetadata.TryRead(
                    artifactRelative,
                    File.ReadAllText(file),
                    File.ReadAllBytes(artifactPath),
                    out var raw,
                    out var complaint))
            {
                tests.Add(raw);
            }
            else
            {
                failures.Add(complaint);
            }
        }

        unreadable = failures;
        return tests;
    }
}
