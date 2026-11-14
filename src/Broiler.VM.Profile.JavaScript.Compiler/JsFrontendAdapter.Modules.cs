// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           0
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    High
// Criteria:         5/5
// Resource impact:  3/10 max
// Unverified:       6
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=A46E3C
    // Broiler-Falsified-If: an import's specifiers, their order, names or spans, its specifier or its attributes differ from the wide parser's
    // Broiler-Human:        PENDING
    private JsStatement Import(ImportDeclaration import)
    {
        var specifiers = new System.Collections.Generic.List<JsImportSpecifier>();

        foreach (var (span, imported, local, space) in JsTreeFacts.ImportBindings(import, result.Tokens))
        {
            specifiers.Add(new JsImportSpecifier(span, imported, local, space));
        }

        return new JsImportDeclaration(JsTreeFacts.At(import), import.Source.Value, specifiers, Attributes(import.Attributes));
    }

    /// <summary>
    /// An <c>export</c> of a declaration, whose names it publishes as themselves at the declaration's
    /// span, or of a clause, with or without a <c>from</c>; a clause without one has no attributes.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=3F38C0
    // Broiler-Falsified-If: an export's kind, specifiers, their order, names or spans, its declaration, its specifier or its attributes differ from the wide parser's
    // Broiler-Human:        PENDING
    private JsStatement ExportNamed(ExportNamedDeclaration export)
    {
        var span = JsTreeFacts.At(export);
        var specifiers = new System.Collections.Generic.List<JsExportSpecifier>();

        if (export.Declaration is { } declared)
        {
            var declaration = Statement(declared);

            foreach (var (at, local, exported) in JsTreeFacts.DeclarationExports(declared))
            {
                specifiers.Add(new JsExportSpecifier(at, local, exported));
            }

            return new JsExportDeclaration(span, JsExportKind.Declaration, string.Empty, specifiers, declaration, null);
        }

        foreach (var (at, local, exported) in JsTreeFacts.ClauseExports(export.Specifiers))
        {
            specifiers.Add(new JsExportSpecifier(at, local, exported));
        }

        if (export.Source is { } source)
        {
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=44A3BB
    // Broiler-Falsified-If: a default export's specifier, its declaration's name or its value differs from the wide parser's
    // Broiler-Human:        PENDING
    private JsStatement ExportDefault(ExportDefaultDeclaration export)
    {
        var span = JsTreeFacts.At(export);

        switch (export.Declaration)
        {
            case ClassDeclaration declared:
            {
                var at = JsTreeFacts.At(declared);
                var node = Class(declared.Class, JsTreeFacts.DefaultExportName);
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
                var at = JsTreeFacts.At(declared);
                var node = Function(declared.Function, at, JsTreeFacts.DefaultExportName);
                return new JsExportDeclaration(
                    span,
                    JsExportKind.Default,
                    string.Empty,
                    new System.Collections.Generic.List<JsExportSpecifier> { new(span, node.Name, "default") },
                    Record(new JsFunctionDeclaration(at, node), declared),
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
                throw JsTreeFacts.Unexpected(export.Declaration, "a default export");
        }
    }

    /// <summary><c>export * from</c>, and <c>export * as name from</c>, whose specifier stands at its <c>as</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=E8E9CA
    // Broiler-Falsified-If: a re-export's specifier, its span, the requested specifier or the attributes differ from the wide parser's
    // Broiler-Human:        PENDING
    private JsStatement ExportAll(ExportAllDeclaration export)
    {
        var specifiers = new System.Collections.Generic.List<JsExportSpecifier>();

        if (export.Exported is { } exported)
        {
            specifiers.Add(new JsExportSpecifier(JsTreeFacts.TokenBefore(result.Tokens, exported, Word.As), JsModuleNames.StarName, JsTreeFacts.ExportName(exported)));
        }

        return new JsExportDeclaration(
            JsTreeFacts.At(export), JsExportKind.All, export.Source.Value, specifiers, null, null, Attributes(export.Attributes));
    }

    /// <summary>An attribute clause, each entry at its key: none is the empty clause.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=FC0FF7
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<JsImportAttribute> Attributes(
        System.Collections.Generic.IReadOnlyList<ImportAttribute>? attributes)
    {
        var all = new System.Collections.Generic.List<JsImportAttribute>();

        foreach (var (span, key, value) in JsTreeFacts.Attributes(attributes))
        {
            all.Add(new JsImportAttribute(span, key, value));
        }

        return all;
    }

}
