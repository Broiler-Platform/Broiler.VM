using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One piece of state in the lowering assembly that outlives a call.</summary>
/// <remarks>
/// <c>Kind</c> is what makes it one - a mutable static, a <c>[ThreadStatic]</c> field, or an
/// <c>AsyncLocal</c> - and the message a rule writes names it, because "there is ambient state
/// here" is not something a reader can act on and "this field is a mutable static" is.
/// </remarks>
internal sealed record AmbientStateSite(string File, string Member, string Kind);

/// <summary>
/// The scan behind rule N12: what in the lowering assembly could carry a parse's state out of it.
/// </summary>
/// <remarks>
/// <para>
/// <b>This exists because roadmap section 9 states the gate as a runtime test and a runtime test
/// is the weaker half.</b> The gate is two parses with different goals running concurrently, each
/// producing the goal-appropriate result, failing when the options are replaced by a shared
/// static. That test is real and the producer composition runs it - but it can only fail when a
/// shared static is actually reached by the two parses it runs, and a static reached by a third
/// construct nobody wrote a concurrent case for would sit there green. The scan has no such gap:
/// it asks what could hold state at all, over every declaration in the assembly.
/// </para>
/// <para>
/// <b>A readonly static is not ambient state and is not reported.</b> The tokenizer's punctuator
/// table and the validator's reserved-name list are both static and neither can carry anything out
/// of a parse, because nothing can write them. The subject is state a parse could mutate, which is
/// the shape the seed's async-local goal switch has and the shape this component removed.
/// </para>
/// </remarks>
internal static class FrontEndAmbientState
{
    /// <summary>The assembly this rule is about.</summary>
    internal const string Assembly = "Broiler.VM.Profile.JavaScript.Compiler";

    /// <summary>The two ambient-context types no front-end file may name.</summary>
    /// <remarks>
    /// Matched on the written type name rather than on a resolved symbol, for the reason every
    /// group N rule is over text: rule A11 keeps these assemblies out of this project's reference
    /// set, so there is no metadata to resolve against and this rule does not pretend there is.
    /// </remarks>
    internal static readonly string[] AmbientTypes = ["AsyncLocal", "ThreadLocal"];

    /// <summary>Every declaration in <paramref name="files"/> that could outlive a call.</summary>
    internal static IReadOnlyList<AmbientStateSite> Sites(IEnumerable<AssuranceSourceFile> files)
    {
        var found = new List<AmbientStateSite>();

        foreach (var file in files)
        {
            foreach (var field in file.Tree.GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>())
            {
                var modifiers = field.Modifiers.Select(static token => token.ValueText).ToArray();
                var isStatic = modifiers.Contains("static", StringComparer.Ordinal);
                var isConst = modifiers.Contains("const", StringComparer.Ordinal);
                var isReadOnly = modifiers.Contains("readonly", StringComparer.Ordinal);
                var type = field.Declaration.Type.ToString();

                foreach (var declared in field.Declaration.Variables)
                {
                    var name = declared.Identifier.ValueText;

                    if (HasAttribute(field.AttributeLists, "ThreadStatic"))
                    {
                        found.Add(new AmbientStateSite(file.RelativePath, name, "a [ThreadStatic] field"));
                        continue;
                    }

                    if (AmbientTypes.Any(ambient =>
                        type.StartsWith(ambient, StringComparison.Ordinal) ||
                        type.Contains("." + ambient, StringComparison.Ordinal)))
                    {
                        found.Add(new AmbientStateSite(
                            file.RelativePath, name, $"a field of ambient-context type {type}"));

                        continue;
                    }

                    if (isStatic && !isConst && !isReadOnly)
                    {
                        found.Add(new AmbientStateSite(
                            file.RelativePath, name, "a mutable static field"));
                    }
                }
            }

            // A static property with a setter is the same thing wearing an accessor, and leaving
            // it out would make the rule a rule about the `static` keyword rather than about state.
            foreach (var property in file.Tree.GetRoot().DescendantNodes()
                .OfType<PropertyDeclarationSyntax>())
            {
                var modifiers = property.Modifiers.Select(static token => token.ValueText).ToArray();

                if (!modifiers.Contains("static", StringComparer.Ordinal))
                {
                    continue;
                }

                var setter = property.AccessorList?.Accessors.Any(static accessor =>
                    accessor.IsKind(SyntaxKind.SetAccessorDeclaration) ||
                    accessor.IsKind(SyntaxKind.InitAccessorDeclaration)) ?? false;

                if (setter)
                {
                    found.Add(new AmbientStateSite(
                        file.RelativePath, property.Identifier.ValueText, "a settable static property"));
                }
            }
        }

        return found;
    }

    private static bool HasAttribute(SyntaxList<AttributeListSyntax> lists, string name) => lists
        .SelectMany(static list => list.Attributes)
        .Any(attribute => attribute.Name.ToString() is var written &&
            (written == name || written == name + "Attribute" ||
                written.EndsWith("." + name, StringComparison.Ordinal)));
}
