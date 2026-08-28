using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The product source set the Broiler Code Assurance system covers: every C# file compiled into
/// one of the three product assemblies, parsed once.
/// </summary>
/// <remarks>
/// <para>
/// The set is derived from <see cref="ComponentGraph.Projects"/> rather than listed, so a new
/// product file is in scope the moment it exists. Test-only projects and witness inputs are out
/// of scope: the policy is about what ships.
/// </para>
/// <para>
/// Each file records the newline it uses and is read as UTF-8. The generator writes back with the
/// same newline and without a byte-order mark, so a regeneration that changes nothing is a
/// byte-identical file rather than a whitespace diff.
/// </para>
/// <para>
/// <b>The parse resolves the build's conditional symbols.</b> Parsing with
/// <c>CSharpParseOptions.Default</c> defines no preprocessor symbol at all, so every region the
/// real build COMPILES - a <c>#if NET10_0_OR_GREATER</c> block, say - parses as disabled text,
/// which is trivia, which the fingerprint excludes by construction. A declaration inside such a
/// region would then ship without being scanned, annotated or fingerprinted, and a body could be
/// swapped for one that ships while the fingerprint binding the review sat still. The symbol set
/// below is a superset of what the build defines for <c>net10.0</c> in either configuration, so a
/// region the build enables is enabled here too. Rule J6 is the other half of that defence and
/// does not depend on this one: it forbids a preprocessor directive in a covered file outright.
/// </para>
/// </remarks>
internal static class AssuranceSources
{
    /// <summary>The three assemblies the component publishes, and therefore covers.</summary>
    internal static readonly string[] CoveredAssemblies =
        ["Broiler.VM.Abstractions", "Broiler.VM.Binary", "Broiler.VM.Runtime"];

    /// <summary>
    /// The conditional symbols the parse defines: a superset of what the SDK defines for
    /// <c>net10.0</c>, in both the Debug and the Release configuration.
    /// </summary>
    /// <remarks>
    /// A superset rather than the exact set, deliberately. The purpose is that no region the build
    /// compiles is invisible to the scanner, and defining a symbol the build does not define costs
    /// only that a region the build discards would be scanned - which over-includes, the bias this
    /// whole system is written with. <c>DEBUG</c> and <c>RELEASE</c> are both here for that reason:
    /// the gate is documented to run in Release and a reviewer may run either.
    /// </remarks>
    internal static readonly string[] PreprocessorSymbols =
    [
        "NET",
        "NETCOREAPP",
        "NETCOREAPP1_0_OR_GREATER", "NETCOREAPP1_1_OR_GREATER", "NETCOREAPP2_0_OR_GREATER",
        "NETCOREAPP2_1_OR_GREATER", "NETCOREAPP2_2_OR_GREATER", "NETCOREAPP3_0_OR_GREATER",
        "NETCOREAPP3_1_OR_GREATER",
        "NET5_0_OR_GREATER", "NET6_0_OR_GREATER", "NET7_0_OR_GREATER", "NET8_0_OR_GREATER",
        "NET9_0_OR_GREATER", "NET10_0_OR_GREATER",
        "NET10_0",
        "DEBUG", "RELEASE", "TRACE",
    ];

    /// <summary>
    /// The parse options every read of a covered file, a witness or a probe goes through, so that
    /// what the scanner sees is what the compiler sees.
    /// </summary>
    internal static readonly CSharpParseOptions ParseOptions = CSharpParseOptions.Default
        .WithLanguageVersion(LanguageVersion.Latest)
        .WithPreprocessorSymbols(PreprocessorSymbols);

    /// <summary>Parses one source text under <see cref="ParseOptions"/>. The only parse there is.</summary>
    internal static SyntaxTree Parse(string text, string path) =>
        CSharpSyntaxTree.ParseText(text, ParseOptions, path: path);

    /// <summary>Every covered source file, ordered by path so every generated artefact is stable.</summary>
    internal static IReadOnlyList<AssuranceSourceFile> Files { get; } = Load();

    /// <summary>UTF-8 with no byte-order mark. The component has none and must not acquire one.</summary>
    internal static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    internal static AssuranceSourceFile File(string relativePath) =>
        Files.SingleOrDefault(file => string.Equals(file.RelativePath, relativePath, StringComparison.Ordinal))
        ?? throw new InvalidOperationException($"No covered product source file at {relativePath}.");

    private static IReadOnlyList<AssuranceSourceFile> Load() =>
        ComponentGraph.Projects
            .Where(static project => project.IsProduct)
            .Where(static project => CoveredAssemblies.Contains(project.AssemblyName, StringComparer.Ordinal))
            .SelectMany(static project => Directory
                .EnumerateFiles(Path.GetDirectoryName(project.Path)!, "*.cs", SearchOption.AllDirectories)
                .Where(path => !IsProjectBuildOutput(Path.GetDirectoryName(project.Path)!, path))
                .Select(path => ReadFile(path, project.AssemblyName)))
            .OrderBy(static file => file.RelativePath, StringComparer.Ordinal)
            .ToArray();

    /// <summary>
    /// The same file with different content, reparsed.
    /// </summary>
    /// <remarks>
    /// The generator refreshes the annotations and then derives the file's summary from what it
    /// just wrote, not from what it read. Without that step one generation would not be a fixed
    /// point: the summary counts states, a state depends on the fingerprint, and the fingerprint
    /// is what the same pass is filling in - so the first run would publish the state the tree was
    /// in before it ran.
    /// </remarks>
    internal static AssuranceSourceFile WithText(AssuranceSourceFile file, string text) =>
        file with { Text = text, Tree = Parse(text, file.FullPath) };

    /// <summary>Reads and parses one file. Internal so a re-read can check idempotence.</summary>
    internal static AssuranceSourceFile ReadFile(string path, string assembly)
    {
        var text = System.IO.File.ReadAllText(path);

        return new AssuranceSourceFile(
            FullPath: path,
            RelativePath: Path.GetRelativePath(ComponentGraph.Root, path).Replace('\\', '/'),
            Assembly: assembly,
            Text: text,
            NewLine: text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n",
            Tree: Parse(text, path));
    }

    /// <summary>
    /// Every preprocessor directive in a covered file, named with the line it sits on.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A directive is trivia. It is therefore excluded from the fingerprint by construction, which
    /// makes it the one thing a covered file can carry that changes what ships without changing
    /// what any review is bound to. A conditional region is the sharp case - a body inside
    /// <c>#if NET10_0_OR_GREATER</c> compiles, and under a parse that does not define the symbol it
    /// is not even a declaration - but the rule is about every directive, because a
    /// <c>#pragma warning disable</c> or a <c>#nullable disable</c> changes what the compiler
    /// checks and appears in no fingerprint either.
    /// </para>
    /// <para>
    /// This is one of two independent defences and neither is the other's fallback:
    /// <see cref="ParseOptions"/> resolves the build's symbols so that a region which does ship is
    /// scanned, and this rule refuses the directive outright so that no covered file has one.
    /// </para>
    /// </remarks>
    internal static List<string> DirectiveViolations(IEnumerable<AssuranceSourceFile> files) => files
        .SelectMany(static file => file.Tree.GetRoot()
            .DescendantNodesAndSelf(descendIntoTrivia: true)
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.DirectiveTriviaSyntax>()
            .Select(directive =>
                $"{file.RelativePath}" +
                $"({file.Tree.GetLineSpan(directive.Span).StartLinePosition.Line + 1}): " +
                $"carries the preprocessor directive '{directive.ToString().Trim()}', and a covered " +
                "file carries none - a directive is trivia and no fingerprint records it"))
        .ToList();

    /// <summary>
    /// True when a file sits in the build output a project actually produces: the <c>bin</c> or
    /// <c>obj</c> directory at the PROJECT ROOT, and nowhere else.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An earlier revision dropped any path with a segment named <c>bin</c> or <c>obj</c> anywhere
    /// beneath the project, which does not agree with the compiler. The SDK's
    /// <c>DefaultItemExcludes</c> removes <c>bin/**</c> and <c>obj/**</c> relative to the project
    /// root only, so a real product file at <c>Internal/obj/VmHiddenGate.cs</c> compiles into the
    /// shipped assembly - and under the old test it was scanned by nothing. Every J rule then read
    /// a corpus the compiler did not: no unannotated unit to report, no reviewer identifier to
    /// find, no header to regenerate.
    /// </para>
    /// <para>
    /// The comparison is on the path prefix rather than on segment names, so a directory whose name
    /// merely CONTAINS the word is not matched, and the boundary is a directory separator.
    /// </para>
    /// </remarks>
    internal static bool IsProjectBuildOutput(string projectDirectory, string path)
    {
        var relative = Path.GetRelativePath(projectDirectory, path);

        if (relative.StartsWith("..", StringComparison.Ordinal) || Path.IsPathRooted(relative))
        {
            return false;
        }

        var segments = relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        return segments.Length > 1 &&
            (string.Equals(segments[0], "bin", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(segments[0], "obj", StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>One covered product source file, with the parse the scanner and fingerprinter share.</summary>
internal sealed record AssuranceSourceFile(
    string FullPath,
    string RelativePath,
    string Assembly,
    string Text,
    string NewLine,
    SyntaxTree Tree);
