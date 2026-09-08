// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Text;

namespace Broiler.VM.Composition.PolyglotCli;

/// <summary>Which composed profile a file is for.</summary>
/// <remarks>
/// <b>There are two and there is no third value meaning "unknown".</b> A file this host cannot
/// route does not get a lane it might later be given; it gets a refusal naming what it is and what
/// the two lanes accept. An enumeration with an <c>Unknown</c> member would have let a routing
/// mistake travel into a front end and come back as a decode failure, which names the symptom and
/// hides the decision.
/// </remarks>
internal enum Lane
{
    /// <summary>The <c>broiler.javascript</c> profile: source text in, bytecode out, run.</summary>
    JavaScript,

    /// <summary>The <c>broiler.webassembly</c> profile: a module's bytes verbatim, verified and run.</summary>
    WebAssembly,
}

/// <summary>One file this host was asked to handle, routed, read or refused.</summary>
/// <param name="Path">The path as the caller named it, with separators normalized for reporting.</param>
/// <param name="Lane">The profile this file is for. Meaningless when <paramref name="Problem"/> is set.</param>
/// <param name="Text">The source, for a JavaScript file. Empty otherwise.</param>
/// <param name="Bytes">The module, for a WebAssembly file. Empty otherwise.</param>
/// <param name="Problem">Why the file cannot be handled, or empty where it can.</param>
/// <param name="Status">Which kind of refusal <paramref name="Problem"/> is.</param>
internal sealed record InputFile(
    string Path, Lane Lane, string Text, byte[] Bytes, string Problem, RunStatus Status);

/// <summary>
/// Turning command-line paths into routed inputs, which is the step a single-profile host skips.
/// </summary>
/// <remarks>
/// <para>
/// <b>MOST OF THIS FILE IS COPIED FROM
/// <c>Broiler.VM.Composition.JavaScript.Cli/SourceFiles.cs</c>, and that is said here rather than
/// left to be discovered.</b> The byte-order-mark strip, the strict UTF-8 decode that refuses
/// rather than substituting, the recursive directory sweep, the ordinal sort and the treatment of
/// a named path that is not there are all decisions that root took, argued for in its own remarks,
/// and exercised by <c>eng/run-cli-acceptance.py</c> over the binary a person would run. Rules A11
/// and A12 leave such code nowhere else - a shared library would be a project outside
/// <c>src/compositions/</c> referencing a profile assembly, which A11 forbids, and a composition
/// root referencing another composition root is not the reference set A12 admits - so a second
/// end-user host copies rather than references, and the copy is the normal shape here.
/// </para>
/// <para>
/// <b>WHAT IS NEW IS THE ROUTING, AND IT IS DECIDED TWICE.</b> The extension says which profile a
/// file claims to be for; the first four bytes say whether the file agrees. A <c>.wasm</c> file
/// that does not open with the module preamble is REFUSED rather than handed to a decoder, and a
/// <c>.js</c> file that does open with it is refused rather than compiled as text - because a host
/// that routed on a name alone would answer a mislabelled file with a decode diagnostic about byte
/// zero, which names the symptom and hides the mistake. Both directions are checked, because a
/// check in one direction only would leave the more likely mistake - a module saved under the
/// wrong name - reported as a syntax error on line 1.
/// </para>
/// </remarks>
internal static class Inputs
{
    /// <summary>The extension a script is presented under.</summary>
    private const string ScriptExtension = ".js";

    /// <summary>
    /// The extension that makes a file a module without the option being passed.
    /// </summary>
    /// <remarks>
    /// <b>The goal is a property of how a source is PRESENTED, and a file name is one way of
    /// presenting it.</b> The same characters are a legal script and a legal module with different
    /// meanings, so something has to say which, and it cannot be the text.
    /// </remarks>
    internal const string ModuleExtension = ".mjs";

    /// <summary>The extension a WebAssembly module is presented under.</summary>
    private const string WasmExtension = ".wasm";

    /// <summary>
    /// The four bytes every WebAssembly module opens with, and the whole of the content check.
    /// </summary>
    /// <remarks>
    /// <b>The preamble's VERSION word is deliberately NOT checked here.</b> Refusing a version is
    /// the profile's decision and it answers with a diagnostic the profile owns; a host that
    /// pre-empted it would be a second reader of the same field, able to disagree with the one
    /// that owns it. What this host checks is only whether the file is a module at all, which is
    /// the question routing asks and the profile never gets to.
    /// </remarks>
    private static readonly byte[] WasmMagic = [0x00, 0x61, 0x73, 0x6D];

    /// <summary>A decoder that refuses rather than substituting.</summary>
    /// <remarks>
    /// <c>throwOnInvalidBytes</c> is the whole point. <see cref="Encoding.UTF8"/> replaces bad
    /// bytes with U+FFFD, which would hand the tokenizer a character the file does not contain.
    /// </remarks>
    private static readonly UTF8Encoding Strict =
        new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    /// <summary>Whether this path is presented as a module by its name alone.</summary>
    internal static bool IsModulePath(string path) =>
        path.EndsWith(ModuleExtension, StringComparison.Ordinal);

    /// <summary>
    /// Expands the caller's paths into the files to handle, in a stable order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A path naming a directory is swept for <c>.js</c>, <c>.mjs</c> and <c>.wasm</c> files,
    /// recursively. That is not what a conventional host does with a directory, and it is here for
    /// the reason the JavaScript host gives: the corpora this repository argues its scope from are
    /// directory trees of thousands of files, and the alternative is a shell able to expand a
    /// recursive glob, which is not every shell.
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
                foreach (var extension in new[] { ScriptExtension, ModuleExtension, WasmExtension })
                {
                    found.AddRange(
                        Directory.EnumerateFiles(path, "*" + extension, SearchOption.AllDirectories));
                }

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

    /// <summary>Reads one file and routes it, or says why it can be neither read nor routed.</summary>
    internal static InputFile Read(string path)
    {
        var shown = path.Replace('\\', '/');

        byte[] bytes;

        try
        {
            bytes = File.ReadAllBytes(path);
        }
        catch (IOException failure)
        {
            return Refused(shown, failure.Message, RunStatus.Unreadable);
        }
        catch (UnauthorizedAccessException failure)
        {
            return Refused(shown, failure.Message, RunStatus.Unreadable);
        }

        var looksLikeAModule = StartsWithTheModulePreamble(bytes);

        if (path.EndsWith(WasmExtension, StringComparison.Ordinal))
        {
            return looksLikeAModule
                ? new InputFile(
                    shown, Lane.WebAssembly, string.Empty, bytes, string.Empty, RunStatus.Completed)
                : Refused(
                    shown,
                    "is named `.wasm` and does not open with the WebAssembly preamble " +
                    "00 61 73 6D, so this host declines to hand it to a decoder that would " +
                    "report a byte-zero failure instead of a mislabelled file",
                    RunStatus.Unroutable);
        }

        if (path.EndsWith(ScriptExtension, StringComparison.Ordinal) ||
            path.EndsWith(ModuleExtension, StringComparison.Ordinal))
        {
            if (looksLikeAModule)
            {
                return Refused(
                    shown,
                    "opens with the WebAssembly preamble 00 61 73 6D and is named as JavaScript " +
                    "source, so this host declines to route it; rename it `.wasm` if that is what " +
                    "it is",
                    RunStatus.Unroutable);
            }

            var start = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF
                ? 3
                : 0;

            try
            {
                return new InputFile(
                    shown,
                    Lane.JavaScript,
                    Strict.GetString(bytes, start, bytes.Length - start),
                    [],
                    string.Empty,
                    RunStatus.Completed);
            }
            catch (DecoderFallbackException failure)
            {
                return Refused(
                    shown,
                    "not valid UTF-8" +
                        (failure.Index >= 0 ? $" at byte {failure.Index + start}" : string.Empty) +
                        ": a host that decoded it with replacement characters would run a program " +
                        "the file does not contain",
                    RunStatus.Unreadable);
            }
        }

        return Refused(
            shown,
            "names no profile this host composes; it routes `.js` and `.mjs` to " +
            "broiler.javascript and `.wasm` to broiler.webassembly, and nothing else",
            RunStatus.Unroutable);
    }

    private static InputFile Refused(string shown, string problem, RunStatus status) =>
        new(shown, Lane.JavaScript, string.Empty, [], problem, status);

    private static bool StartsWithTheModulePreamble(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= WasmMagic.Length && bytes[..WasmMagic.Length].SequenceEqual(WasmMagic);
}
