using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule W4: the WebAssembly profile reads its own integers, and compares a declared count with its
/// ceiling in one place.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why the profile declines the core's readers.</b> The core's bounded reader offers three
/// variable-length readers, <c>TryReadVarUInt32</c>, <c>TryReadVarUInt64</c> and
/// <c>TryReadDeclaredCount</c>, and all three refuse an over-long encoding. This format admits one: a
/// single-pass compiler pads a section length to five bytes, and the retained corpus's canonical module
/// carries exactly that. So the profile reads every integer with its own <c>WasmLeb128</c>. A call to
/// one of the core's readers slipped back in would refuse modules the format calls well formed, and
/// nothing but the corpus entries that happen to pad that integer would notice. WA-2's gate asks for a
/// scan that says so.
/// </para>
/// <para>
/// <b>Declining <c>TryReadDeclaredCount</c> costs the comparison it made</b>, and the profile makes it
/// again in <c>WasmLeb128.TryReadBoundedCount</c>: a count read from the payload is compared with the
/// effective declared-count ceiling and charged before any caller can loop on it or size a buffer from
/// it. The same gate asks that there be no second implementation of that comparison. The rule reads
/// "the comparison" as a relational expression one of whose operands names <c>MaxDeclaredCount</c>, and
/// it lists the places that may hold one: <c>TryReadBoundedCount</c> itself, and the decoder's
/// <c>TryReadLocals</c>. The second compares a different quantity, a body's local runs added together,
/// each run already read through the first. A third place is an edit to this file.
/// </para>
/// <para>
/// <b>Stated limits.</b> The reading is syntax, as every source rule here is. A ceiling passed to a
/// helper by value - the validator hands <c>MaxDeclaredCount</c> to its buffer-growth helper, which
/// compares its own parameter - is not a comparison of a declared count, and is not read as one. That
/// every count the profile reads goes through <c>TryReadBoundedCount</c> is not asserted here: the
/// corpus's ceiling entries show it for the counts they reach.
/// </para>
/// </remarks>
internal static class WebAssemblyReaderRules
{
    /// <summary>The core's canonical variable-length readers, none of which the profile may call.</summary>
    internal static readonly string[] CanonicalReaders = ["TryReadVarUInt32", "TryReadVarUInt64", "TryReadDeclaredCount"];

    /// <summary>The member of the core's bounds a declared count is compared with.</summary>
    internal const string CountCeiling = "MaxDeclaredCount";

    /// <summary>The members allowed to compare a count with the ceiling, by file and member, and why.</summary>
    internal static readonly (string File, string Member, string Why)[] Comparisons =
    [
        ("src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs", "TryReadBoundedCount",
            "the one reader of a count from the payload, compared and charged before it is returned"),
        ("src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs", "TryReadLocals",
            "the total of a body's local runs, each run read through TryReadBoundedCount first"),
    ];

    /// <summary>
    /// W4 over <paramref name="files"/>: every use of a canonical reader, every count-ceiling
    /// comparison outside the listed members, and every listed member that no longer holds one.
    /// </summary>
    internal static IEnumerable<string> W4(IReadOnlyList<AssuranceSourceFile> files)
    {
        if (files.Count == 0)
        {
            yield return $"no source file of {WebAssemblyFamilyRules.ProfileAssembly} was read, so nothing was scanned";
            yield break;
        }

        var found = new HashSet<(string File, string Member)>();

        foreach (var file in files)
        {
            var root = file.Tree.GetRoot();

            foreach (var name in root.DescendantNodes().OfType<SimpleNameSyntax>()
                         .Where(static name => CanonicalReaders.Contains(name.Identifier.ValueText, StringComparer.Ordinal)))
            {
                yield return
                    $"{file.RelativePath}({Line(file, name)}) names the core's canonical reader {name.Identifier.ValueText}, " +
                    "which refuses the padded encodings this format admits";
            }

            foreach (var comparison in root.DescendantNodes().OfType<BinaryExpressionSyntax>()
                         .Where(static binary => binary.Kind() is SyntaxKind.GreaterThanExpression or SyntaxKind.GreaterThanOrEqualExpression
                             or SyntaxKind.LessThanExpression or SyntaxKind.LessThanOrEqualExpression)
                         .Where(static binary => NamesCeiling(binary.Left) || NamesCeiling(binary.Right)))
            {
                var member = comparison.Ancestors().OfType<MemberDeclarationSyntax>().FirstOrDefault() switch
                {
                    MethodDeclarationSyntax method => method.Identifier.ValueText,
                    PropertyDeclarationSyntax property => property.Identifier.ValueText,
                    _ => "(no member)",
                };

                if (Comparisons.Any(listed => listed.File == file.RelativePath && listed.Member == member))
                {
                    found.Add((file.RelativePath, member));
                    continue;
                }

                yield return
                    $"{file.RelativePath}({Line(file, comparison)}) compares with {CountCeiling} in {member}, a second " +
                    "implementation of the count-bound comparison the rule does not list";
            }
        }

        foreach (var (file, member, _) in Comparisons.Where(listed => !found.Contains((listed.File, listed.Member))))
        {
            yield return $"the rule lists {member} in {file} as comparing a count with {CountCeiling}, and it does not";
        }

        static bool NamesCeiling(ExpressionSyntax operand) =>
            operand.DescendantNodesAndSelf().OfType<SimpleNameSyntax>()
                .Any(static name => name.Identifier.ValueText == CountCeiling);

        static int Line(AssuranceSourceFile file, SyntaxNode node) =>
            file.Tree.GetLineSpan(node.Span).StartLinePosition.Line + 1;
    }
}
