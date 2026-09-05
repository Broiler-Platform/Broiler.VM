using System.Text;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>One file this host was asked to run, read or refused.</summary>
/// <param name="Path">The path as the caller named it, with separators normalized for reporting.</param>
/// <param name="Text">The source, or empty where it could not be read.</param>
/// <param name="Unreadable">Why the file is not source, or empty where it is.</param>
internal sealed record SourceFile(string Path, string Text, string Unreadable);

/// <summary>
/// Turning command-line paths into source text, which is the one thing a harness never had to do.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every sibling root in this checkout gets its input from inside its own image</b> - a
/// programmatic builder, an embedded resource, a fixture tree it also wrote. This root is handed a
/// path by a person, and that is the whole difference between a demonstration and the
/// <c>narrow-runtime-compiler</c> composition. It is also where two problems arrive that no
/// fixture ever had: a byte-order mark, and bytes that are not UTF-8.
/// </para>
/// <para>
/// <b>Neither is papered over.</b> A leading byte-order mark is removed, because the language
/// defines U+FEFF as format-control whitespace a source text may open with, and a tokenizer handed
/// it would refuse an ordinary file saved by an ordinary editor. Bytes that are not valid UTF-8
/// are REFUSED rather than decoded with replacement characters: a replacement character changes
/// the program, and a host that silently ran a different program than the file contains is worse
/// than one that declines.
/// </para>
/// </remarks>
internal static class SourceFiles
{
    /// <summary>The extension this host recognises when it sweeps a directory.</summary>
    private const string Extension = ".js";

    /// <summary>
    /// The extension that makes a file a module without the option being passed.
    /// </summary>
    /// <remarks>
    /// <b>The goal is a property of how a source is PRESENTED, and a file name is one way of
    /// presenting it.</b> The same characters are a legal script and a legal module with different
    /// meanings - <c>this</c> at the top level, whether a declaration reaches the global object,
    /// whether <c>await</c> is a name - so something has to say which, and it cannot be the text.
    /// The option says it explicitly; this extension says it by convention, the way every host that
    /// reads files from a disk has settled on.
    /// </remarks>
    internal const string ModuleExtension = ".mjs";

    /// <summary>Whether this path is presented as a module by its name alone.</summary>
    internal static bool IsModulePath(string path) =>
        path.EndsWith(ModuleExtension, StringComparison.Ordinal);

    /// <summary>A decoder that refuses rather than substituting.</summary>
    /// <remarks>
    /// <c>throwOnInvalidBytes</c> is the whole point. <see cref="Encoding.UTF8"/> replaces bad
    /// bytes with U+FFFD, which would hand the tokenizer a character the file does not contain.
    /// </remarks>
    private static readonly UTF8Encoding Strict = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    /// <summary>
    /// Expands the caller's paths into the files to run, in a stable order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A path naming a directory is swept for <c>.js</c> files, recursively. That is not what a
    /// conventional host does with a directory, and it is here for a stated reason: the corpora
    /// this profile's scope is argued from are directory trees of thousands of files, and the
    /// alternative is a shell able to expand a recursive glob, which is not every shell.
    /// </para>
    /// <para>
    /// <b>The order is ordinal by full path and does not depend on the filesystem.</b> Two runs of
    /// the same tree report the same rows in the same order, which is what makes two transcripts
    /// comparable.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<string> Expand(IEnumerable<string> paths, out IReadOnlyList<string> missing)
    {
        var found = new List<string>();
        var absent = new List<string>();

        foreach (var path in paths)
        {
            if (Directory.Exists(path))
            {
                found.AddRange(Directory.EnumerateFiles(path, "*" + Extension, SearchOption.AllDirectories));
                found.AddRange(
                    Directory.EnumerateFiles(path, "*" + ModuleExtension, SearchOption.AllDirectories));

                continue;
            }

            if (File.Exists(path))
            {
                found.Add(path);
                continue;
            }

            // A NAMED PATH THAT IS NOT THERE IS AN ERROR AND NOT AN EMPTY SET. A host that shrugged
            // would exit zero on a mistyped filename, which is the one outcome a script driving it
            // cannot recover from.
            absent.Add(path);
        }

        missing = absent;
        found.Sort(StringComparer.Ordinal);
        return found;
    }

    /// <summary>Reads one file as source, or says why it is not source.</summary>
    internal static SourceFile Read(string path)
    {
        var shown = path.Replace('\\', '/');

        byte[] bytes;

        try
        {
            bytes = File.ReadAllBytes(path);
        }
        catch (IOException failure)
        {
            return new SourceFile(shown, string.Empty, failure.Message);
        }
        catch (UnauthorizedAccessException failure)
        {
            return new SourceFile(shown, string.Empty, failure.Message);
        }

        var start = 0;

        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
        {
            start = 3;
        }

        try
        {
            return new SourceFile(shown, Strict.GetString(bytes, start, bytes.Length - start), string.Empty);
        }
        catch (DecoderFallbackException failure)
        {
            return new SourceFile(
                shown,
                string.Empty,
                "not valid UTF-8" +
                    (failure.Index >= 0 ? $" at byte {failure.Index + start}" : string.Empty) +
                    ": a host that decoded it with replacement characters would run a program the " +
                    "file does not contain");
        }
    }
}
