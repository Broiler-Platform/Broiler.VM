using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule W6: the WebAssembly profile names no core outcome category where it executes, and no execution
/// step at all.
/// </summary>
/// <remarks>
/// <para>
/// <b>The invariant, in the roadmap's words:</b> "The executor answers in the core's vocabulary and no
/// other. Every step is one of the five execution-step kinds. Traps, uncaught exceptions, and link
/// errors are typed payloads this profile owns; no profile code names a core outcome category, and
/// adding an instruction never adds a core result case." WA-5's exit gate asks for the scan.
/// </para>
/// <para>
/// <b>Where it stands since milestone UBC-4.</b> The universal bytecode's emitter executes this
/// profile's modules and answers every step; the family's handlers answer it through the bytecode's
/// own status - next, trap, request, defect - and the emitter and the core make the step and its
/// category from that. So no source of the profile names <c>VmExecutionStep</c> or
/// <c>VmExecutionStepKind</c>, and none names <c>VmOutcome</c>, with one listed exception.
/// </para>
/// <para>
/// <b>The exception is the translator, and it is not the executor.</b> A translation answers in the
/// fields the core's verification of the bare module answered in, so a composition root prints what it
/// printed before the translator existed. It names the four categories a verification of this profile
/// answers - <c>Normal</c>, <c>InvalidArtifact</c>, <c>ResourceExhaustion</c> and <c>Cancellation</c>
/// - and no other: a translation that named a fault, an invalid state or a contract violation would be
/// the translator answering for execution. The file and the four members are listed here, and a fifth
/// is an edit to this file.
/// </para>
/// <para>
/// <b>Stated limits.</b> The reading is syntax. A name is matched by its identifier, whatever it is
/// qualified with, and a <c>using static</c> or an alias of one of the three types is itself reported,
/// because a bare member name after it could not be followed. Documentation comments are not read,
/// since they are not code. A category reached through <c>var</c> and a value the core handed back -
/// reading <c>result.Outcome</c> and passing it on - names no category and is not reported.
/// </para>
/// </remarks>
internal static class WebAssemblyVocabularyRules
{
    /// <summary>The core types that are the execution step, which no source of the profile may name.</summary>
    internal static readonly string[] StepTypes = ["VmExecutionStep", "VmExecutionStepKind"];

    /// <summary>The core's outcome categories.</summary>
    internal const string OutcomeType = "VmOutcome";

    /// <summary>The one file that may name <see cref="OutcomeType"/>, and why.</summary>
    internal const string TranslatorFile = "src/Broiler.VM.Profile.WebAssembly/WasmTranslator.cs";

    /// <summary>The categories a verification of this profile answers, which the translator may name.</summary>
    internal static readonly string[] VerificationCategories = ["Normal", "InvalidArtifact", "ResourceExhaustion", "Cancellation"];

    /// <summary>
    /// W6 over <paramref name="files"/>: every step type named, every outcome named outside the
    /// translator, every category the translator names past the four, and every <c>using static</c> or
    /// alias of the three types. <paramref name="admitted"/> is each category the translator names, as
    /// the rule found it.
    /// </summary>
    internal static IReadOnlyList<string> W6(IReadOnlyList<AssuranceSourceFile> files, out IReadOnlyList<string> admitted)
    {
        var messages = new List<string>();
        var found = new SortedSet<string>(StringComparer.Ordinal);
        admitted = [];

        if (files.Count == 0)
        {
            messages.Add($"no source file of {WebAssemblyFamilyRules.ProfileAssembly} was read, so nothing was scanned");
            return messages;
        }

        if (!files.Any(static file => file.RelativePath == TranslatorFile))
        {
            messages.Add($"the rule lists {TranslatorFile} as the one file that may name {OutcomeType}, and no source has that path");
        }

        foreach (var file in files)
        {
            var root = file.Tree.GetRoot();

            foreach (var directive in root.DescendantNodes().OfType<UsingDirectiveSyntax>()
                         .Where(static directive => directive.StaticKeyword != default || directive.Alias is not null)
                         .Where(directive => directive.NamespaceOrType.DescendantNodesAndSelf().OfType<SimpleNameSyntax>()
                             .Any(static name => IsGuarded(name.Identifier.ValueText))))
            {
                messages.Add(
                    $"{file.RelativePath}({Line(file, directive)}) {(directive.Alias is null ? "imports the members of" : "aliases")} " +
                    $"{directive.NamespaceOrType}, after which a name it brings could not be followed");
            }

            foreach (var name in root.DescendantNodes().OfType<IdentifierNameSyntax>())
            {
                var identifier = name.Identifier.ValueText;

                if (StepTypes.Contains(identifier, StringComparer.Ordinal))
                {
                    if (!InUsing(name))
                    {
                        messages.Add(
                            $"{file.RelativePath}({Line(file, name)}) names {identifier}, and the emitter, not the profile, answers the execution step");
                    }

                    continue;
                }

                if (identifier != OutcomeType || InUsing(name))
                {
                    continue;
                }

                var member = Member(name);

                if (file.RelativePath != TranslatorFile)
                {
                    messages.Add(
                        $"{file.RelativePath}({Line(file, name)}) names {OutcomeType}{(member is null ? string.Empty : "." + member)}, " +
                        "a core outcome category, outside the translator");
                    continue;
                }

                if (member is null)
                {
                    continue;
                }

                if (VerificationCategories.Contains(member, StringComparer.Ordinal))
                {
                    found.Add(member);
                    continue;
                }

                messages.Add(
                    $"{file.RelativePath}({Line(file, name)}) names {OutcomeType}.{member}, a category no verification of this " +
                    "profile answers, so the translator would be answering for execution");
            }
        }

        admitted = [.. found];
        return messages;

        static bool IsGuarded(string identifier) => identifier == OutcomeType || StepTypes.Contains(identifier, StringComparer.Ordinal);

        static bool InUsing(SyntaxNode node) => node.Ancestors().OfType<UsingDirectiveSyntax>().Any();

        static int Line(AssuranceSourceFile file, SyntaxNode node) =>
            file.Tree.GetLineSpan(node.Span).StartLinePosition.Line + 1;
    }

    /// <summary>The member named after the type, <c>VmOutcome.X</c> in an expression or a type position, or none.</summary>
    private static string? Member(IdentifierNameSyntax name)
    {
        // A qualified spelling, Broiler.VM.VmOutcome, is the right-hand side of its own access; the
        // member, if any, is named by the access around that.
        SyntaxNode type = name.Parent switch
        {
            MemberAccessExpressionSyntax access when access.Name == name => access,
            QualifiedNameSyntax qualified when qualified.Right == name => qualified,
            _ => name,
        };

        return type.Parent switch
        {
            MemberAccessExpressionSyntax access when access.Expression == type => access.Name.Identifier.ValueText,
            QualifiedNameSyntax qualified when qualified.Left == type => qualified.Right.Identifier.ValueText,
            _ => null,
        };
    }
}
