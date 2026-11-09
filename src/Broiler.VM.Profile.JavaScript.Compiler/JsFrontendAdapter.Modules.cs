// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           0
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    High
// Criteria:         5/5
// Resource impact:  3/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>The module declarations of <see cref="JsFrontendAdapter"/>.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=818D37
// Broiler-Falsified-If: for a module declaration the wide parser accepted, a specifier, a name, an attribute, a declaration or a span built here differs from the wide parser's
// Broiler-Human:        PENDING
internal sealed partial class JsFrontendAdapter
{
    /// <summary>
    /// An <c>import</c> declaration: the default binding, then the namespace binding at its <c>*</c> or
    /// the named bindings, the specifier and the attributes, which an import always has, if none.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=99CFC9
    // Broiler-Falsified-If: an import's specifiers, their order, names or spans, its specifier or its attributes differ from the wide parser's
    // Broiler-Human:        PENDING
    private JsStatement Import(ImportDeclaration import)
    {
        var specifiers = new System.Collections.Generic.List<JsImportSpecifier>();

        if (import.DefaultBinding is { } binding)
        {
            specifiers.Add(new JsImportSpecifier(At(binding), "default", binding.Name, Namespace: false));
        }

        if (import.NamespaceBinding is { } space)
        {
            specifiers.Add(new JsImportSpecifier(TokenBefore(space, TokenKind.Star), string.Empty, space.Name, Namespace: true));
        }

        if (import.NamedImports is { } named)
        {
            foreach (var specifier in named)
            {
                var local = specifier.Local.Name;
                var imported = specifier.Imported is null ? local : ExportName(specifier.Imported);
                specifiers.Add(new JsImportSpecifier(At(specifier), imported, local, Namespace: false));
            }
        }

        Admission.ModuleSpecifier(import.Source);

        return new JsImportDeclaration(At(import), import.Source.Value, specifiers, Attributes(import.Attributes));
    }

    /// <summary>
    /// An <c>export</c> of a declaration, whose names it publishes as themselves at the declaration's
    /// span, or of a clause, with or without a <c>from</c>; a clause without one has no attributes.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=805320
    // Broiler-Falsified-If: an export's kind, specifiers, their order, names or spans, its declaration, its specifier or its attributes differ from the wide parser's
    // Broiler-Human:        PENDING
    private JsStatement ExportNamed(ExportNamedDeclaration export)
    {
        var span = At(export);
        var specifiers = new System.Collections.Generic.List<JsExportSpecifier>();

        if (export.Declaration is { } declared)
        {
            var at = At(declared);
            var declaration = Statement(declared);

            switch (declaration)
            {
                case JsClassDeclaration { Class.Name: var name }:
                    specifiers.Add(new JsExportSpecifier(at, name, name));
                    break;
                case JsFunctionDeclaration { Function.Name: var name }:
                    specifiers.Add(new JsExportSpecifier(at, name, name));
                    break;
                case JsVariableStatement variables:
                    foreach (var declarator in variables.Declarators)
                    {
                        if (declarator.Pattern is null)
                        {
                            specifiers.Add(new JsExportSpecifier(at, declarator.Name, declarator.Name));
                            continue;
                        }

                        var bound = new System.Collections.Generic.List<string>();
                        PatternNames(declarator.Pattern, bound);

                        foreach (var bind in bound)
                        {
                            specifiers.Add(new JsExportSpecifier(at, bind, bind));
                        }
                    }

                    break;
                default:
                    throw Unexpected(declared, "an exported declaration");
            }

            return new JsExportDeclaration(span, JsExportKind.Declaration, string.Empty, specifiers, declaration, null);
        }

        foreach (var specifier in export.Specifiers)
        {
            var local = ExportName(specifier.Local);
            var exported = specifier.Exported is null ? local : ExportName(specifier.Exported);
            specifiers.Add(new JsExportSpecifier(At(specifier), local, exported));
        }

        if (export.Source is { } source)
        {
            Admission.ModuleSpecifier(source);
            return new JsExportDeclaration(
                span, JsExportKind.Named, source.Value, specifiers, null, null, Attributes(export.Attributes));
        }

        return new JsExportDeclaration(span, JsExportKind.Named, string.Empty, specifiers, null, null, null);
    }

    /// <summary>
    /// <c>export default</c>: of a class or a function, named <c>default</c> where it has no name of its
    /// own, or of an expression, bound to <see cref="JsModuleNames.DefaultBindingName"/>; the specifier
    /// stands at the <c>export</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=30724D
    // Broiler-Falsified-If: a default export's specifier, its declaration's name or its value differs from the wide parser's
    // Broiler-Human:        PENDING
    private JsStatement ExportDefault(ExportDefaultDeclaration export)
    {
        var span = At(export);

        switch (export.Declaration)
        {
            case ClassDeclaration declared:
            {
                var at = At(declared);
                var node = Class(declared.Class, "default");
                return new JsExportDeclaration(
                    span,
                    JsExportKind.Default,
                    string.Empty,
                    new System.Collections.Generic.List<JsExportSpecifier> { new(span, node.Name, "default") },
                    new JsClassDeclaration(at, node),
                    null);
            }

            case FunctionDeclaration declared:
            {
                var at = At(declared);
                var node = Function(declared.Function, at, "default");
                return new JsExportDeclaration(
                    span,
                    JsExportKind.Default,
                    string.Empty,
                    new System.Collections.Generic.List<JsExportSpecifier> { new(span, node.Name, "default") },
                    new JsFunctionDeclaration(at, node),
                    null);
            }

            case Expression value:
                return new JsExportDeclaration(
                    span,
                    JsExportKind.Default,
                    string.Empty,
                    new System.Collections.Generic.List<JsExportSpecifier> { new(span, JsModuleNames.DefaultBindingName, "default") },
                    null,
                    Expression(value));
            default:
                throw Unexpected(export.Declaration, "a default export");
        }
    }

    /// <summary><c>export * from</c>, and <c>export * as name from</c>, whose specifier stands at its <c>as</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=AC7DFB
    // Broiler-Falsified-If: a re-export's specifier, its span, the requested specifier or the attributes differ from the wide parser's
    // Broiler-Human:        PENDING
    private JsStatement ExportAll(ExportAllDeclaration export)
    {
        var specifiers = new System.Collections.Generic.List<JsExportSpecifier>();

        if (export.Exported is { } exported)
        {
            specifiers.Add(new JsExportSpecifier(TokenBefore(exported, Word.As), JsModuleNames.StarName, ExportName(exported)));
        }

        Admission.ModuleSpecifier(export.Source);

        return new JsExportDeclaration(
            At(export), JsExportKind.All, export.Source.Value, specifiers, null, null, Attributes(export.Attributes));
    }

    /// <summary>A module export name: an identifier's name, or a string's value.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=8D6096
    // Broiler-Human:        PENDING
    private static string ExportName(SyntaxNode name) => name switch
    {
        IdentifierName identifier => identifier.Name,
        StringLiteral literal => literal.Value,
        BindingIdentifier binding => binding.Name,
        _ => throw Unexpected(name, "a module export name"),
    };

    /// <summary>An attribute clause, each entry at its key: none is the empty clause.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=DF022F
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<JsImportAttribute> Attributes(
        System.Collections.Generic.IReadOnlyList<ImportAttribute>? attributes)
    {
        var all = new System.Collections.Generic.List<JsImportAttribute>();

        if (attributes is null)
        {
            return all;
        }

        foreach (var attribute in attributes)
        {
            all.Add(new JsImportAttribute(At(attribute), ExportName(attribute.Key), attribute.Value.Value));
        }

        return all;
    }

    /// <summary>Every name a declaration's pattern binds, in source order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=D61F0A
    // Broiler-Human:        PENDING
    private static void PatternNames(JsPattern pattern, System.Collections.Generic.List<string> names)
    {
        switch (pattern)
        {
            case JsTargetPattern { Target: JsIdentifier identifier }:
                names.Add(identifier.Name);
                return;
            case JsArrayPattern array:
                foreach (var element in array.Elements)
                {
                    if (element is not null)
                    {
                        PatternNames(element.Target, names);
                    }
                }

                if (array.Rest is not null)
                {
                    PatternNames(array.Rest, names);
                }

                return;
            case JsObjectPattern literal:
                foreach (var property in literal.Properties)
                {
                    PatternNames(property.Value.Target, names);
                }

                if (literal.Rest is not null)
                {
                    PatternNames(literal.Rest, names);
                }

                return;
            default:
                return;
        }
    }

    /// <summary>The last retained token of <paramref name="kind"/> before <paramref name="node"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=3ABDB2
    // Broiler-Human:        PENDING
    private SliceSourceSpan TokenBefore(SyntaxNode node, TokenKind kind) =>
        TokenBefore(node, token => token.Kind == kind);

    /// <summary>The last retained token that is the word <paramref name="word"/> before <paramref name="node"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=893970
    // Broiler-Human:        PENDING
    private SliceSourceSpan TokenBefore(SyntaxNode node, Word word) =>
        TokenBefore(node, token => token.Word == word && !token.IsEscaped);

    /// <summary>
    /// The span of the last retained token before <paramref name="node"/> that <paramref name="matches"/>:
    /// an anchor the wide parser gave that the library's tree does not carry.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=5CF764
    // Broiler-Human:        PENDING
    private SliceSourceSpan TokenBefore(SyntaxNode node, System.Func<Token, bool> matches)
    {
        var tokens = result.Tokens ?? throw Unexpected(node, "a module's retained tokens");
        var low = 0;
        var high = tokens.Count;

        while (low < high)
        {
            var middle = (low + high) / 2;

            if (tokens[middle].Start < node.Span.Offset)
            {
                low = middle + 1;
            }
            else
            {
                high = middle;
            }
        }

        for (var at = low - 1; at >= 0; at--)
        {
            if (matches(tokens[at]))
            {
                return new SliceSourceSpan(tokens[at].Line, tokens[at].Column);
            }
        }

        throw Unexpected(node, "a module declaration with the token before it");
    }
}
