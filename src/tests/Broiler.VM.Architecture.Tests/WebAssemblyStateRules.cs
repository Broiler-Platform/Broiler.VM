using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule W5: nothing reachable from the WebAssembly profile's verified state is mutable.
/// </summary>
/// <remarks>
/// <para>
/// <b>The invariant, in the roadmap's words:</b> "everything reachable from a verified state must be
/// immutable once verification returns and safe for unsynchronised concurrent readers. Memories,
/// tables, globals, and every mutable cache belong to a store, never to a handle. Two runtimes sharing
/// one verified handle share nothing mutable." WA-5's exit gate names the structural scan that pins it,
/// and this is that scan.
/// </para>
/// <para>
/// <b>What the verified state is, since milestone UBC-4.</b> The core's handle wraps the universal
/// bytecode's verified program, and what that program keeps of this profile is the family state the
/// hook answers with when a verification ends: the module definitions, <c>WasmDefinitions</c>. So the
/// scan starts there and walks every type a stored member of it names, transitively, through the
/// profile's own declarations. A stored member is a field or an auto-implemented property; a property
/// with a body computes rather than keeps, and keeps nothing to scan.
/// </para>
/// <para>
/// <b>What a member may hold</b>: a built-in value type or a string, an enumeration, an
/// <c>ImmutableArray</c> or a <c>ReadOnlyMemory</c> of what may be held, another type the scan then walks,
/// or one of the types outside the assembly the rule lists. A field must be <c>readonly</c> and a
/// property must have no <c>set</c> accessor. An array, a list, a dictionary, a <c>Memory</c> or any
/// other type is reported, and so is a type the scan cannot find: the reading fails closed.
/// </para>
/// <para>
/// <b>Stated limits.</b> Syntax, not semantics. A <c>ReadOnlyMemory</c> may view an array another
/// holder writes to; the scan holds the type, and that the definitions' windows view the artifact's own
/// immutable bytes is the reader's claim, not this rule's. Static members are not reached from an
/// instance and are not scanned. The concurrent-read half of the gate's clause is not here.
/// </para>
/// </remarks>
internal static class WebAssemblyStateRules
{
    /// <summary>The types the verified state starts at, and why each is one.</summary>
    internal static readonly (string Type, string Why)[] Roots =
    [
        ("WasmDefinitions",
            "the family state the hook answers with when a verification ends, which the universal bytecode's verified program keeps and the core's handle wraps"),
    ];

    /// <summary>Generic types that are immutable when what they hold is.</summary>
    internal static readonly string[] ImmutableGenerics = ["ImmutableArray", "ReadOnlyMemory"];

    /// <summary>Types outside the profile assembly a stored member may name, and why each is immutable.</summary>
    internal static readonly (string Type, string Why)[] AdmittedExternal =
    [
        ("UbcPosition", "a sealed class of the universal bytecode whose four properties are get-only and set by its constructor"),
    ];

    /// <summary>The built-in types a stored member may hold.</summary>
    private static readonly string[] BuiltIn =
        ["bool", "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "char", "string"];

    /// <summary>W5 over <paramref name="files"/>, from <paramref name="roots"/>: every mutable thing a stored member keeps.</summary>
    internal static IEnumerable<string> W5(IReadOnlyList<AssuranceSourceFile> files, IReadOnlyList<string> roots, out IReadOnlyList<string> walked)
    {
        var declared = new Dictionary<string, (AssuranceSourceFile File, BaseTypeDeclarationSyntax Declaration)>(StringComparer.Ordinal);

        foreach (var file in files)
        {
            foreach (var declaration in file.Tree.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
            {
                declared.TryAdd(declaration.Identifier.ValueText, (file, declaration));
            }
        }

        var messages = new List<string>();
        var seen = new List<string>();
        var queue = new Queue<string>();

        foreach (var root in roots)
        {
            if (!declared.ContainsKey(root))
            {
                messages.Add($"the rule starts at {root}, and no source of {WebAssemblyFamilyRules.ProfileAssembly} declares it");
                continue;
            }

            queue.Enqueue(root);
        }

        while (queue.Count > 0)
        {
            var name = queue.Dequeue();

            if (seen.Contains(name, StringComparer.Ordinal))
            {
                continue;
            }

            seen.Add(name);
            var (file, declaration) = declared[name];

            if (declaration is not TypeDeclarationSyntax type)
            {
                continue;
            }

            foreach (var member in type.Members)
            {
                if (member.Modifiers.Any(SyntaxKind.StaticKeyword) || member.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    continue;
                }

                switch (member)
                {
                    case FieldDeclarationSyntax field:
                        foreach (var variable in field.Declaration.Variables)
                        {
                            var where = $"{file.RelativePath}({Line(file, variable)}) {name}.{variable.Identifier.ValueText}";

                            if (!field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
                            {
                                messages.Add($"{where} is a field that is not readonly");
                            }

                            Hold(field.Declaration.Type, where, declared, queue, messages);
                        }

                        break;

                    case PropertyDeclarationSyntax property when property.AccessorList is { } accessors:
                    {
                        var where = $"{file.RelativePath}({Line(file, property)}) {name}.{property.Identifier.ValueText}";

                        if (accessors.Accessors.Any(static accessor => accessor.IsKind(SyntaxKind.SetAccessorDeclaration)))
                        {
                            messages.Add($"{where} is a property with a setter");
                        }

                        // An auto-implemented property keeps what it names; one with bodies computes it.
                        if (accessors.Accessors.All(static accessor => accessor.Body is null && accessor.ExpressionBody is null))
                        {
                            Hold(property.Type, where, declared, queue, messages);
                        }

                        break;
                    }
                }
            }
        }

        walked = seen;
        return messages;

        static int Line(AssuranceSourceFile file, SyntaxNode node) => file.Tree.GetLineSpan(node.Span).StartLinePosition.Line + 1;
    }

    /// <summary>Holds one stored member's type to what may be kept, and queues the profile's own types it names.</summary>
    private static void Hold(
        TypeSyntax type,
        string where,
        IReadOnlyDictionary<string, (AssuranceSourceFile File, BaseTypeDeclarationSyntax Declaration)> declared,
        Queue<string> queue,
        List<string> messages)
    {
        switch (type)
        {
            case PredefinedTypeSyntax predefined when BuiltIn.Contains(predefined.Keyword.ValueText, StringComparer.Ordinal):
                return;

            case NullableTypeSyntax nullable:
                Hold(nullable.ElementType, where, declared, queue, messages);
                return;

            case ArrayTypeSyntax:
                messages.Add($"{where} keeps an array, which any holder of it can write");
                return;

            case QualifiedNameSyntax qualified:
                Hold(qualified.Right, where, declared, queue, messages);
                return;

            case GenericNameSyntax generic:
                if (!ImmutableGenerics.Contains(generic.Identifier.ValueText, StringComparer.Ordinal))
                {
                    messages.Add($"{where} keeps a {generic.Identifier.ValueText}, a generic type the rule does not admit as immutable");
                    return;
                }

                foreach (var argument in generic.TypeArgumentList.Arguments)
                {
                    Hold(argument, where, declared, queue, messages);
                }

                return;

            case IdentifierNameSyntax identifier:
            {
                var name = identifier.Identifier.ValueText;

                if (declared.TryGetValue(name, out var found))
                {
                    if (found.Declaration is not EnumDeclarationSyntax)
                    {
                        queue.Enqueue(name);
                    }

                    return;
                }

                if (AdmittedExternal.Any(admitted => admitted.Type == name))
                {
                    return;
                }

                messages.Add($"{where} keeps a {name}, a type the scan cannot see and the rule does not admit");
                return;
            }

            default:
                messages.Add($"{where} keeps a {type}, which the scan cannot read");
                return;
        }
    }
}
