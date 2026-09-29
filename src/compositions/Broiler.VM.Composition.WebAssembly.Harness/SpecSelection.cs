using Broiler.VM.Profile.WebAssembly;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// Which scripts of the suite a run reads, stage by stage, and which of them one shard runs: the
/// selection pipeline and the sharding of roadmap section 15.
/// </summary>
/// <remarks>
/// <para>
/// <b>Selection is a recorded pipeline.</b> Discovery finds every script of the directory. Known-incorrect
/// exclusion removes the scripts a list names as wrong upstream. Scope filtering keeps the scripts the
/// feature manifest's scope manifest names. Per-file selectability removes a script the reader cannot
/// read at all. What is left is the selection, and its count is emitted before any shard is chosen, apart
/// from every shard's own count, so a merge can prove the shards covered the whole of it rather than a
/// part.
/// </para>
/// <para>
/// <b>The two lists name files, and a name the suite does not hold is a configuration failure.</b> A scope
/// manifest naming a script the directory does not contain is one of the roadmap's named failures; the
/// known-incorrect list is held to the same, so neither can go on excluding or admitting a file that is no
/// longer there. With no scope manifest, every candidate is in scope, and the run says so.
/// </para>
/// <para>
/// <b>Content-independent sharding.</b> A script's shard is the first eight bytes of the SHA-256 of its
/// normalized path - its name within the suite directory, which is how every answer line names it - read
/// big-endian, modulo the shard count. So a script's shard does not move when the selection around it
/// changes, and a shard's history stays comparable across runs.
/// </para>
/// </remarks>
internal static class SpecSelection
{
    /// <summary>One run's selection: the count after every stage, the scripts selected, and this shard's share.</summary>
    internal sealed record Selection(
        int Candidates,
        IReadOnlyList<string> KnownIncorrect,
        IReadOnlyList<string> OutOfScope,
        IReadOnlyList<(string Name, string Reason)> Unselectable,
        IReadOnlyList<(string Name, string Path)> Selected,
        int Shard,
        int Shards,
        IReadOnlyList<(string Name, string Path)> InShard,
        string Scope);

    /// <summary>The shard a script belongs to among <paramref name="shards"/>.</summary>
    internal static int Bucket(string normalizedPath, int shards)
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedPath));
        var value = System.Buffers.Binary.BinaryPrimitives.ReadUInt64BigEndian(digest);
        return (int)(value % (ulong)shards);
    }

    /// <summary>Parses <c>i/n</c>, answering shard zero of one when there is nothing to parse.</summary>
    internal static bool TryParseShard(string? text, out int shard, out int shards)
    {
        shard = 0;
        shards = 1;

        if (text is null)
        {
            return true;
        }

        var slash = text.IndexOf('/', StringComparison.Ordinal);

        return slash > 0 &&
            int.TryParse(text[..slash], NumberStyles.None, CultureInfo.InvariantCulture, out shard) &&
            int.TryParse(text[(slash + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out shards) &&
            shards > 0 && shard < shards;
    }

    /// <summary>
    /// Runs the pipeline over <paramref name="directory"/>; answers the selection, or the configuration
    /// failure that stops it.
    /// </summary>
    internal static bool TrySelect(
        string directory, string? scopeFile, string? knownIncorrectFile, int shard, int shards,
        out Selection selection, out string failure)
    {
        selection = null!;

        // Discovery.
        var candidates = Directory.GetFiles(directory, "*.wast")
            .Select(static path => (Name: Path.GetFileName(path), Path: path))
            .OrderBy(static script => script.Name, StringComparer.Ordinal)
            .ToList();

        if (candidates.Count == 0)
        {
            failure = $"empty selection: {directory} holds no script";
            return false;
        }

        var names = candidates.Select(static candidate => candidate.Name).ToHashSet(StringComparer.Ordinal);

        // Known-incorrect exclusion.
        var knownIncorrect = new List<string>();

        if (knownIncorrectFile is not null)
        {
            if (!TryReadNames(knownIncorrectFile, "known-incorrect list", names, out var listed, out _, out failure))
            {
                return false;
            }

            knownIncorrect.AddRange(listed);
        }

        var remaining = candidates.Where(candidate => !knownIncorrect.Contains(candidate.Name, StringComparer.Ordinal)).ToList();

        // Scope filtering.
        var outOfScope = new List<string>();
        var scope = "no scope manifest: every candidate is in scope";

        if (scopeFile is not null)
        {
            if (!TryReadNames(scopeFile, "scope manifest", names, out var inScope, out var manifest, out failure))
            {
                return false;
            }

            var expected = WebAssemblyProfile.SliceManifest.ToString();

            if (!string.Equals(manifest, expected, StringComparison.Ordinal))
            {
                failure = $"scope manifest {scopeFile} is for the manifest {manifest ?? "(none named)"}, and this lane scores {expected}";
                return false;
            }

            outOfScope.AddRange(remaining.Where(candidate => !inScope.Contains(candidate.Name, StringComparer.Ordinal)).Select(static candidate => candidate.Name));
            remaining = [.. remaining.Where(candidate => inScope.Contains(candidate.Name, StringComparer.Ordinal))];
            scope = $"scope manifest {Path.GetFileName(scopeFile)} for {manifest}";
        }

        // Per-file selectability.
        var unselectable = new List<(string, string)>();
        var selected = new List<(string Name, string Path)>();

        foreach (var candidate in remaining)
        {
            try
            {
                ScriptText.Read(File.ReadAllBytes(candidate.Path));
                selected.Add(candidate);
            }
            catch (ScriptReadException refused)
            {
                unselectable.Add((candidate.Name, refused.Message));
            }
        }

        if (selected.Count == 0)
        {
            failure = $"empty selection: none of the {candidates.Count.ToString(CultureInfo.InvariantCulture)} scripts discovered survived the pipeline";
            return false;
        }

        var inShard = selected.Where(script => Bucket(script.Name, shards) == shard).ToList();

        selection = new Selection(candidates.Count, knownIncorrect, outOfScope, unselectable, selected, shard, shards, inShard, scope);
        failure = string.Empty;
        return true;
    }

    /// <summary>
    /// Reads a list of script names, one <c>file</c> line each, and an optional <c>manifest</c> line;
    /// refuses a name the suite does not hold, a name given twice, and any other line.
    /// </summary>
    internal static bool TryReadNames(
        string path, string what, IReadOnlySet<string> suite, out List<string> listed, out string? manifest, out string failure)
    {
        listed = [];
        manifest = null;

        if (!File.Exists(path))
        {
            failure = $"{what} {path} does not exist";
            return false;
        }

        foreach (var line in File.ReadAllLines(path).Where(static line => line.Length > 0 && line[0] != '#'))
        {
            var parts = line.Split(' ', 3);

            switch (parts[0])
            {
                case "manifest" when parts.Length >= 2:
                    manifest = parts[1];
                    break;

                case "file" when parts.Length >= 2:
                    if (!suite.Contains(parts[1]))
                    {
                        failure = $"a {what} naming a file the suite does not contain: {Path.GetFileName(path)} names {parts[1]}";
                        return false;
                    }

                    if (listed.Contains(parts[1], StringComparer.Ordinal))
                    {
                        failure = $"{what} {Path.GetFileName(path)} names {parts[1]} twice";
                        return false;
                    }

                    listed.Add(parts[1]);
                    break;

                default:
                    failure = $"{what} {Path.GetFileName(path)} holds the line \"{line}\", which is neither a manifest nor a file";
                    return false;
            }
        }

        failure = string.Empty;
        return true;
    }

    /// <summary>The pipeline's counts, one line a stage, and every script a stage removed, by name.</summary>
    internal static void Print(Selection selection)
    {
        Console.WriteLine(
            $"# spec: selection: {Text(selection.Candidates)} candidates, {Text(selection.KnownIncorrect.Count)} known incorrect, " +
            $"{Text(selection.OutOfScope.Count)} out of scope, {Text(selection.Unselectable.Count)} unselectable, " +
            $"{Text(selection.Selected.Count)} selected before sharding; {selection.Scope}");

        foreach (var name in selection.KnownIncorrect)
        {
            Console.WriteLine($"# spec: known incorrect {name}");
        }

        foreach (var name in selection.OutOfScope)
        {
            Console.WriteLine($"# spec: out of scope {name}");
        }

        foreach (var (name, reason) in selection.Unselectable)
        {
            Console.WriteLine($"# spec: unselectable {name}: {reason}");
        }

        Console.WriteLine(
            $"# spec: shard {Text(selection.Shard)} of {Text(selection.Shards)}: {Text(selection.InShard.Count)} scripts");
    }

    private static string Text(int value) => value.ToString(CultureInfo.InvariantCulture);
}
