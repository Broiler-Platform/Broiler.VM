// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           0
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  3/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>The functions, classes and patterns of <see cref="JsFrontendAdapter"/>.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=818D37
// Broiler-Falsified-If: for a function, a class or a pattern the wide parser accepted, a node, a field, a span or a source offset built here differs from the wide parser's
// Broiler-Human:        PENDING
internal sealed partial class JsFrontendAdapter
{
    /// <summary>
    /// A function that is not an arrow: a declaration, an expression or a method, named by its own
    /// binding or by <paramref name="name"/>, beginning at <paramref name="span"/>, and with the source
    /// offsets of its whole text.
    /// </summary>
    /// <param name="function">The library's function.</param>
    /// <param name="span">Where the wide parser began it: its declaration, its expression or its entry.</param>
    /// <param name="name">The name a method takes from its key, or the empty name.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=F9D520
    // Broiler-Falsified-If: a function's name, parameters, body, directives, strictness, kind or source offsets differ from the wide parser's
    // Broiler-Human:        PENDING
    private JsFunctionNode Function(FunctionNode function, SliceSourceSpan span, string name)
    {
        var parameters = Parameters(function.Parameters);
        var body = new System.Collections.Generic.List<JsStatement>(function.Body.Count);
        var directives = Prologue(function.Body, body);

        return Record(new JsFunctionNode(
            span,
            function.Name?.Name ?? name,
            parameters,
            body,
            IsArrow: false,
            Strict(function),
            directives,
            function.IsGenerator,
            function.IsAsync)
        {
            SourceStart = function.Span.Offset,
            SourceEnd = JsTreeFacts.EndOf(function),
        }, function);
    }

    /// <summary>
    /// An arrow function: its parameters, and its body, or the expression it returns as one
    /// <c>return</c> at the arrow's own span.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=854CF9
    // Broiler-Falsified-If: an arrow's parameters, body, directives, strictness, kind or source offsets differ from the wide parser's
    // Broiler-Human:        PENDING
    private JsFunctionNode Arrow(FunctionNode function, SliceSourceSpan span)
    {
        var parameters = Parameters(function.Parameters);
        var body = new System.Collections.Generic.List<JsStatement>();
        System.Collections.Generic.IReadOnlyList<JsStringLiteral> directives;

        if (function.ExpressionBody is { } value)
        {
            body.Add(new JsReturnStatement(span, Expression(value)));
            directives = [];
        }
        else
        {
            directives = Prologue(function.Body, body);
        }

        return Record(new JsFunctionNode(
            span,
            string.Empty,
            parameters,
            body,
            IsArrow: true,
            Strict(function),
            directives,
            IsGenerator: false,
            function.IsAsync)
        {
            SourceStart = function.Span.Offset,
            SourceEnd = JsTreeFacts.EndOf(function),
        }, function);
    }

    /// <summary>
    /// A formal parameter list: each parameter at its first token, its default, and the rest parameter
    /// at its <c>...</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5AB1DB
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsParameter> Parameters(
        System.Collections.Generic.IReadOnlyList<Pattern> parameters)
    {
        var all = new System.Collections.Generic.List<JsParameter>(parameters.Count);

        foreach (var parameter in parameters)
        {
            var span = JsTreeFacts.At(parameter);

            all.Add(parameter switch
            {
                RestElement rest => new JsParameter(span, Pattern(rest.Argument), null, IsRest: true),
                AssignmentPattern defaulted => new JsParameter(
                    span, Pattern(defaulted.Target), Expression(defaulted.Initializer), IsRest: false),
                _ => new JsParameter(span, Pattern(parameter), null, IsRest: false),
            });
        }

        return all;
    }

    /// <summary>
    /// A binding or assignment pattern: a name or a reference is a leaf, and each element and property
    /// value carries its default; a rest element is the pattern's rest.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=EFF750
    // Broiler-Falsified-If: a pattern's shape, keys, defaults, rest or spans differ from the wide parser's
    // Broiler-Human:        PENDING
    private JsPattern Pattern(Pattern pattern)
    {
        var span = JsTreeFacts.At(pattern);

        switch (pattern)
        {
            case BindingIdentifier identifier:
                return new JsTargetPattern(span, new JsIdentifier(span, identifier.Name));
            case TargetPattern target:
            {
                var leaf = Expression(target.Target);
                return new JsTargetPattern(leaf.Span, leaf);
            }

            case ArrayPattern array:
            {
                var elements = new System.Collections.Generic.List<JsPatternElement?>(array.Elements.Count);
                JsPattern? rest = null;

                foreach (var element in array.Elements)
                {
                    switch (element)
                    {
                        case null:
                            elements.Add(null);
                            break;
                        case RestElement spread:
                            rest = Pattern(spread.Argument);
                            break;
                        default:
                            elements.Add(Element(element));
                            break;
                    }
                }

                return new JsArrayPattern(span, elements, rest);
            }

            case ObjectPattern literal:
            {
                var properties = new System.Collections.Generic.List<JsPatternProperty>(literal.Properties.Count);
                JsPattern? rest = null;

                foreach (var property in literal.Properties)
                {
                    switch (property)
                    {
                        case RestElement spread:
                            rest = Pattern(spread.Argument);
                            break;
                        case PatternProperty entry:
                        {
                            var entrySpan = JsTreeFacts.At(entry);
                            var key = Key(entry.Key, out var computed);
                            properties.Add(new JsPatternProperty(entrySpan, key, computed, Element(entry.Value)));
                            break;
                        }

                        default:
                            throw JsTreeFacts.Unexpected(property, "an object pattern's property");
                    }
                }

                return new JsObjectPattern(span, properties, rest);
            }

            default:
                throw JsTreeFacts.Unexpected(pattern, "a pattern");
        }
    }

    /// <summary>One element of an array pattern, or one property value of an object pattern, with its default.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=164F8D
    // Broiler-Human:        PENDING
    private JsPatternElement Element(Pattern element)
    {
        switch (element)
        {
            case AssignmentPattern defaulted:
                return new JsPatternElement(JsTreeFacts.At(defaulted), Pattern(defaulted.Target), Expression(defaulted.Initializer));
            case TargetPattern:
            {
                // A reference's element begins where the reference does, inside any parentheses around
                // it, as the wide parser's cover grammar read the expression it reinterpreted.
                var leaf = Pattern(element);
                return new JsPatternElement(leaf.Span, leaf, null);
            }

            default:
                return new JsPatternElement(JsTreeFacts.At(element), Pattern(element), null);
        }
    }

    /// <summary>
    /// A class: its name, or <paramref name="name"/> where it has none, its heritage, its members in
    /// source order, and the source offsets of its whole text.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=80DAFE
    // Broiler-Falsified-If: a class's name, heritage, members, keys, kinds, functions or source offsets differ from the wide parser's
    // Broiler-Human:        PENDING
    private JsClassNode Class(ClassNode declared, string name)
    {
        var span = JsTreeFacts.At(declared);
        var heritage = declared.Heritage is null ? null : Expression(declared.Heritage);
        var members = new System.Collections.Generic.List<JsClassMember>(declared.Elements.Count);

        foreach (var element in declared.Elements)
        {
            members.Add(ClassMember(element));
        }

        return Record(new JsClassNode(span, declared.Name?.Name ?? name, heritage, declared.Heritage is not null, members)
        {
            SourceStart = declared.Span.Offset,
            SourceEnd = JsTreeFacts.EndOf(declared),
        }, declared);
    }

    /// <summary>
    /// One member of a class body: a method, getter or setter, whose function begins where the member
    /// does and whose text begins after <c>static</c>; a field, whose initialiser is a strict function
    /// returning it at the member's span; or a static block, a strict function of its own.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=6ED4E1
    // Broiler-Falsified-If: a member's kind, key, privacy, staticness, function or source offsets differ from the wide parser's
    // Broiler-Human:        PENDING
    private JsClassMember ClassMember(SyntaxNode element)
    {
        var span = JsTreeFacts.At(element);

        switch (element)
        {
            case MethodDefinition method:
            {
                var key = Key(method.Key, out var computed);
                var kind = method.MethodKind switch
                {
                    MethodKind.Get => JsMethodKind.Get,
                    MethodKind.Set => JsMethodKind.Set,
                    _ => JsMethodKind.Method,
                };

                return new JsClassMember(
                    span, kind, method.IsStatic, key, computed, method.Key is PrivateIdentifier,
                    Function(method.Function, span, key));
            }

            case FieldDefinition field:
            {
                var key = Key(field.Key, out var computed);
                JsFunctionNode? initialiser = null;

                if (field.Value is { } value)
                {
                    initialiser = Record(new JsFunctionNode(
                        span,
                        key,
                        System.Array.Empty<JsParameter>(),
                        [new JsReturnStatement(span, Expression(value))],
                        IsArrow: false,
                        IsStrict: true,
                        System.Array.Empty<JsStringLiteral>()), field);
                }

                return new JsClassMember(
                    span, JsMethodKind.Field, field.IsStatic, key, computed, field.Key is PrivateIdentifier, initialiser);
            }

            case StaticBlock block:
            {
                var body = new System.Collections.Generic.List<JsStatement>(block.Body.Count);
                var directives = Prologue(block.Body, body);

                var function = Record(new JsFunctionNode(
                    span,
                    string.Empty,
                    new System.Collections.Generic.List<JsParameter>(),
                    body,
                    IsArrow: false,
                    IsStrict: true,
                    directives)
                {
                    SourceStart = block.Span.Offset,
                    SourceEnd = JsTreeFacts.EndOf(block),
                }, block);

                return new JsClassMember(
                    span, JsMethodKind.StaticBlock, IsStatic: true, string.Empty, null, IsPrivate: false, function);
            }

            default:
                throw JsTreeFacts.Unexpected(element, "a class member");
        }
    }
}
