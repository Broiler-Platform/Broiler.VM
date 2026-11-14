// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           0
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  3/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;
using NumericLiteral = Broiler.ECMAScript.Syntax.NumericLiteral;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>The expressions of <see cref="JsFrontendAdapter"/>.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=818D37
// Broiler-Falsified-If: for an expression the wide parser accepted, a node, a field or a span built here differs from the wide parser's
// Broiler-Human:        PENDING
internal sealed partial class JsFrontendAdapter
{
    /// <summary>One expression: a parenthesised one is the expression it holds, as the wide parser read it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=9C112D
    // Broiler-Falsified-If: an expression the wide parser accepted adapts to a different node, or a parenthesis survives as a node
    // Broiler-Human:        PENDING
    private JsExpression Expression(Expression expression)
    {
        var span = JsTreeFacts.At(expression);

        switch (expression)
        {
            case ParenthesizedExpression parenthesised:
                return Expression(JsTreeFacts.Unwrap(parenthesised));
            case Identifier identifier:
                return new JsIdentifier(span, identifier.Name);
            case ThisExpression:
                return new JsThisExpression(span);
            case NullLiteral:
                return new JsNullLiteral(span);
            case BooleanLiteral boolean:
                return new JsBooleanLiteral(span, boolean.Value);
            case NumericLiteral number:
                return Number(number);
            case StringLiteral literal:
                return String(literal);
            case RegularExpressionLiteral pattern:
                return new JsRegExpLiteral(span, pattern.Pattern, pattern.Flags);
            case TemplateLiteral template:
                return Template(template);
            case TaggedTemplateExpression tagged:
            {
                var tag = Expression(tagged.Tag);
                return new JsTaggedTemplate(span, tag, Template(tagged.Quasi));
            }

            case ArrayLiteral array:
                return Array(array);
            case ObjectLiteral literal:
                return Object(literal);
            case FunctionExpression function:
                return new JsFunctionExpression(span, Function(function.Function, span, string.Empty));
            case ArrowFunctionExpression arrow:
                return new JsFunctionExpression(span, Arrow(arrow.Function, span));
            case ClassExpression declared:
                return new JsClassExpression(span, Class(declared.Class, string.Empty));
            case MemberExpression member:
                return Member(member);
            case CallExpression call:
                return Call(call);
            case ChainExpression chain:
                return new JsChainExpression(span, Expression(chain.Expression));
            case NewExpression construct:
            {
                var callee = Expression(construct.Callee);
                return new JsNewExpression(span, callee, Arguments(construct.Arguments));
            }

            case MetaProperty meta:
                return meta.Meta.Name == "new"
                    ? new JsNewTargetExpression(span)
                    : new JsImportMeta(span);
            case ImportCall import:
            {
                var specifier = Expression(import.Source);
                return new JsImportCall(span, specifier, import.Options is null ? null : Expression(import.Options));
            }

            case UpdateExpression update:
                return new JsUpdateExpression(
                    span,
                    JsTreeFacts.Update(update.Operator),
                    Expression(update.Operand),
                    update.IsPrefix);
            case UnaryExpression unary:
                return new JsUnaryExpression(span, JsTreeFacts.Unary(unary.Operator), Expression(unary.Operand));
            case AwaitExpression awaited:
                return new JsAwaitExpression(span, Expression(awaited.Argument));
            case BinaryExpression binary:
            {
                var left = Expression(binary.Left);
                return new JsBinaryExpression(span, JsTreeFacts.Binary(binary.Operator), left, Expression(binary.Right));
            }

            case LogicalExpression logical:
            {
                var left = Expression(logical.Left);
                return new JsLogicalExpression(span, JsTreeFacts.Logical(logical.Operator), left, Expression(logical.Right));
            }

            case PrivateInExpression brand:
                return new JsPrivateInExpression(span, brand.Name.Name, Expression(brand.Right));
            case ConditionalExpression conditional:
            {
                var test = Expression(conditional.Test);
                var whenTrue = Expression(conditional.Consequent);
                return new JsConditionalExpression(span, test, whenTrue, Expression(conditional.Alternate));
            }

            case AssignmentExpression assignment:
                return Assignment(assignment);
            case SequenceExpression sequence:
            {
                var all = new System.Collections.Generic.List<JsExpression>(sequence.Expressions.Count);

                foreach (var item in sequence.Expressions)
                {
                    all.Add(Expression(item));
                }

                return new JsSequenceExpression(span, all);
            }

            case YieldExpression yield:
                return new JsYieldExpression(
                    span, yield.Argument is null ? null : Expression(yield.Argument), yield.IsDelegate);
            default:
                throw JsTreeFacts.Unexpected(expression, "an expression");
        }
    }

    /// <summary>
    /// A numeric literal, or a BigInt literal with the value the admission walk recorded for it; a unit
    /// the walk refused any BigInt in is not adapted.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E39C75
    // Broiler-Falsified-If: a numeric literal's value or legacy flag differs from the wide parser's, or a BigInt literal's value differs from the one the walk recorded for it
    // Broiler-Human:        PENDING
    private JsExpression Number(NumericLiteral number)
    {
        var span = JsTreeFacts.At(number);

        if ((number.Flags & NumericLiteralFlags.BigInt) != 0)
        {
            return new JsBigIntLiteral(span, bigInts[number]);
        }

        return new JsNumberLiteral(
            span,
            number.Value,
            (number.Flags & (NumericLiteralFlags.LegacyOctal | NumericLiteralFlags.NonOctalDecimal)) != 0);
    }

    /// <summary>A string literal, with its raw text, quotes included, which a directive is told apart by.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=08D3DF
    // Broiler-Human:        PENDING
    private JsStringLiteral String(StringLiteral literal) =>
        new(JsTreeFacts.At(literal), literal.Value, text.Substring(literal.Span.Offset, literal.Span.Length));

    /// <summary>A template: its cooked and raw chunks, and its substitutions between them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1293AC
    // Broiler-Human:        PENDING
    private JsTemplateLiteral Template(TemplateLiteral template)
    {
        var cooked = new System.Collections.Generic.List<string?>(template.Quasis.Count);
        var raw = new System.Collections.Generic.List<string>(template.Quasis.Count);
        var substitutions = new System.Collections.Generic.List<JsExpression>(template.Expressions.Count);

        for (var which = 0; which < template.Quasis.Count; which++)
        {
            cooked.Add(template.Quasis[which].Cooked);
            raw.Add(template.Quasis[which].Raw);

            if (which < template.Expressions.Count)
            {
                substitutions.Add(Expression(template.Expressions[which]));
            }
        }

        return new JsTemplateLiteral(JsTreeFacts.At(template), cooked, raw, substitutions);
    }

    /// <summary>
    /// An array literal: a hole is <see langword="null"/>, and the trailing comma is the wide parser's
    /// reading, whether a comma followed the last element that is not a hole.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DE9267
    // Broiler-Human:        PENDING
    private JsExpression Array(ArrayLiteral array)
    {
        var elements = new System.Collections.Generic.List<JsExpression?>(array.Elements.Count);
        var anyElement = false;

        foreach (var element in array.Elements)
        {
            switch (element)
            {
                case null:
                    elements.Add(null);
                    break;
                case SpreadElement spread:
                    anyElement = true;
                    elements.Add(new JsSpreadElement(JsTreeFacts.At(spread), Expression(spread.Argument)));
                    break;
                default:
                    anyElement = true;
                    elements.Add(Expression(element));
                    break;
            }
        }

        var trailing = elements.Count != 0 && elements[^1] is null ? anyElement : array.TrailingComma;

        return new JsArrayLiteral(JsTreeFacts.At(array), elements, trailing);
    }

    /// <summary>An object literal and its entries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8028B4
    // Broiler-Human:        PENDING
    private JsExpression Object(ObjectLiteral literal)
    {
        var entries = new System.Collections.Generic.List<JsObjectEntry>(literal.Properties.Count);

        foreach (var property in literal.Properties)
        {
            var span = JsTreeFacts.At(property);

            switch (property)
            {
                case SpreadElement spread:
                    entries.Add(new JsObjectEntry(span, JsPropertyKind.Spread, string.Empty, null, Expression(spread.Argument)));
                    break;
                case PropertyDefinition definition:
                    entries.Add(Entry(definition, span));
                    break;
                default:
                    throw JsTreeFacts.Unexpected(property, "an object literal's entry");
            }
        }

        return new JsObjectLiteral(JsTreeFacts.At(literal), entries, literal.TrailingComma);
    }

    /// <summary>
    /// One entry of an object literal: a value, a shorthand, or a method, getter or setter, whose
    /// function is named by its key and begins where the entry does.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=10ABAD
    // Broiler-Human:        PENDING
    private JsObjectEntry Entry(PropertyDefinition definition, SliceSourceSpan span)
    {
        var key = Key(definition.Key, out var computed);

        switch (definition.PropertyKind)
        {
            case PropertyKind.Shorthand:
                return new JsObjectEntry(span, JsPropertyKind.Init, key, computed, new JsIdentifier(span, key), Shorthand: true);
            case PropertyKind.Init:
                return new JsObjectEntry(span, JsPropertyKind.Init, key, computed, Expression(definition.Value));
            default:
            {
                var function = definition.Value is FunctionExpression expression
                    ? expression.Function
                    : throw JsTreeFacts.Unexpected(definition.Value, "a method's function");

                var kind = definition.PropertyKind switch
                {
                    PropertyKind.Get => JsPropertyKind.Get,
                    PropertyKind.Set => JsPropertyKind.Set,
                    _ => JsPropertyKind.Init,
                };

                return new JsObjectEntry(
                    span,
                    kind,
                    key,
                    computed,
                    new JsFunctionExpression(span, Function(function, span, key)),
                    IsMethod: true);
            }
        }
    }

    /// <summary>
    /// A property key: a name, a string's value, a number by <c>NumberText</c>, a BigInt by its decimal
    /// digits, a private name with its <c>#</c>, or the empty key and the expression of a computed one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=02659B
    // Broiler-Falsified-If: a key's spelling differs from the wide parser's for any literal key
    // Broiler-Human:        PENDING
    private string Key(SyntaxNode key, out JsExpression? computed)
    {
        var spelled = JsTreeFacts.Key(key, bigInts, out var written);
        computed = written is null ? null : Expression(written);
        return spelled;
    }

    /// <summary>
    /// A member access: <c>super.x</c> and <c>super[x]</c>, a private name, a computed key, or a dotted
    /// name with the span of the name after the dot.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CFFF4E
    // Broiler-Human:        PENDING
    private JsExpression Member(MemberExpression member)
    {
        var span = JsTreeFacts.At(member);

        if (member.Object is SuperExpression)
        {
            return member.IsComputed
                ? new JsSuperMemberExpression(span, string.Empty, Expression((Expression)member.Property))
                : new JsSuperMemberExpression(span, ((IdentifierName)member.Property).Name, null);
        }

        var target = Expression(member.Object);

        return member.Property switch
        {
            PrivateIdentifier name => new JsPrivateMemberExpression(span, target, name.Name, member.IsOptional),
            IdentifierName name => new JsMemberExpression(span, target, name.Name, null, member.IsOptional)
            {
                NameSpan = JsTreeFacts.At(name),
            },
            Expression key => new JsMemberExpression(span, target, string.Empty, Expression(key), member.IsOptional),
            _ => throw JsTreeFacts.Unexpected(member.Property, "a member's property"),
        };
    }

    /// <summary>A call, or <c>super(...)</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8DE011
    // Broiler-Human:        PENDING
    private JsExpression Call(CallExpression call)
    {
        var span = JsTreeFacts.At(call);

        if (call.Callee is SuperExpression)
        {
            return new JsSuperCallExpression(span, Arguments(call.Arguments));
        }

        var callee = Expression(call.Callee);

        return new JsCallExpression(span, callee, Arguments(call.Arguments), call.IsOptional);
    }

    /// <summary>An argument list, a spread argument at its <c>...</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E1D772
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsExpression> Arguments(
        System.Collections.Generic.IReadOnlyList<Expression> arguments)
    {
        var all = new System.Collections.Generic.List<JsExpression>(arguments.Count);

        foreach (var argument in arguments)
        {
            all.Add(argument is SpreadElement spread
                ? new JsSpreadElement(JsTreeFacts.At(spread), Expression(spread.Argument))
                : Expression(argument));
        }

        return all;
    }

    /// <summary>
    /// An assignment: to a pattern, a destructuring assignment; otherwise to a reference, whose name
    /// written in parentheses is recorded as the wide parser recorded it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2A8BFC
    // Broiler-Human:        PENDING
    private JsExpression Assignment(AssignmentExpression assignment)
    {
        var span = JsTreeFacts.At(assignment);

        if (assignment.Target is ArrayPattern or ObjectPattern)
        {
            var pattern = Pattern((Pattern)assignment.Target);
            return new JsDestructuringAssignment(span, pattern, Expression(assignment.Value));
        }

        var written = assignment.Target switch
        {
            TargetPattern target => target.Target,
            Expression expression => expression,
            _ => throw JsTreeFacts.Unexpected(assignment.Target, "an assignment's target"),
        };

        var target2 = Expression(written);
        var value = Expression(assignment.Value);

        return new JsAssignmentExpression(span, JsTreeFacts.Compound(assignment.Operator), target2, value)
        {
            ParenthesisedTarget = written is ParenthesizedExpression && target2 is JsIdentifier,
        };
    }

}
