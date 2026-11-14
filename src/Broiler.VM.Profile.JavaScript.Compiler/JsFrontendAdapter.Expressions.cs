// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           0
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  3/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Numbers;
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=BB2DD0
    // Broiler-Falsified-If: an expression the wide parser accepted adapts to a different node, or a parenthesis survives as a node
    // Broiler-Human:        PENDING
    private JsExpression Expression(Expression expression)
    {
        var span = At(expression);

        switch (expression)
        {
            case ParenthesizedExpression parenthesised:
                return Expression(parenthesised.Expression);
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
                    update.Operator == UpdateOperator.Increment ? SliceTokenKind.PlusPlus : SliceTokenKind.MinusMinus,
                    Expression(update.Operand),
                    update.IsPrefix);
            case UnaryExpression unary:
                return new JsUnaryExpression(span, Unary(unary.Operator), Expression(unary.Operand));
            case AwaitExpression awaited:
                return new JsAwaitExpression(span, Expression(awaited.Argument));
            case BinaryExpression binary:
            {
                var left = Expression(binary.Left);
                return new JsBinaryExpression(span, Binary(binary.Operator), left, Expression(binary.Right));
            }

            case LogicalExpression logical:
            {
                var left = Expression(logical.Left);
                return new JsLogicalExpression(span, Logical(logical.Operator), left, Expression(logical.Right));
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
                throw Unexpected(expression, "an expression");
        }
    }

    /// <summary>
    /// A numeric literal, or a BigInt literal with the value the admission walk recorded for it; a unit
    /// the walk refused any BigInt in is not adapted.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=CF30CC
    // Broiler-Falsified-If: a numeric literal's value or legacy flag differs from the wide parser's, or a BigInt literal's value differs from the one the walk recorded for it
    // Broiler-Human:        PENDING
    private JsExpression Number(NumericLiteral number)
    {
        var span = At(number);

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1BDEC1
    // Broiler-Human:        PENDING
    private JsStringLiteral String(StringLiteral literal) =>
        new(At(literal), literal.Value, text.Substring(literal.Span.Offset, literal.Span.Length));

    /// <summary>A template: its cooked and raw chunks, and its substitutions between them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=609E02
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

        return new JsTemplateLiteral(At(template), cooked, raw, substitutions);
    }

    /// <summary>
    /// An array literal: a hole is <see langword="null"/>, and the trailing comma is the wide parser's
    /// reading, whether a comma followed the last element that is not a hole.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7C95CC
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
                    elements.Add(new JsSpreadElement(At(spread), Expression(spread.Argument)));
                    break;
                default:
                    anyElement = true;
                    elements.Add(Expression(element));
                    break;
            }
        }

        var trailing = elements.Count != 0 && elements[^1] is null ? anyElement : array.TrailingComma;

        return new JsArrayLiteral(At(array), elements, trailing);
    }

    /// <summary>An object literal and its entries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B5F412
    // Broiler-Human:        PENDING
    private JsExpression Object(ObjectLiteral literal)
    {
        var entries = new System.Collections.Generic.List<JsObjectEntry>(literal.Properties.Count);

        foreach (var property in literal.Properties)
        {
            var span = At(property);

            switch (property)
            {
                case SpreadElement spread:
                    entries.Add(new JsObjectEntry(span, JsPropertyKind.Spread, string.Empty, null, Expression(spread.Argument)));
                    break;
                case PropertyDefinition definition:
                    entries.Add(Entry(definition, span));
                    break;
                default:
                    throw Unexpected(property, "an object literal's entry");
            }
        }

        return new JsObjectLiteral(At(literal), entries, literal.TrailingComma);
    }

    /// <summary>
    /// One entry of an object literal: a value, a shorthand, or a method, getter or setter, whose
    /// function is named by its key and begins where the entry does.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=351616
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
                    : throw Unexpected(definition.Value, "a method's function");

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=71A963
    // Broiler-Falsified-If: a key's spelling differs from the wide parser's for any literal key
    // Broiler-Human:        PENDING
    private string Key(SyntaxNode key, out JsExpression? computed)
    {
        computed = null;

        switch (key)
        {
            case IdentifierName name:
                return name.Name;
            case PrivateIdentifier name:
                return name.Name;
            case StringLiteral literal:
                return literal.Value;
            case NumericLiteral number when (number.Flags & NumericLiteralFlags.BigInt) != 0:
                return bigInts[number].ToString(System.Globalization.CultureInfo.InvariantCulture);
            case NumericLiteral number:
                return NumberText.ToString(number.Value);
            case ComputedPropertyName name:
                computed = Expression(name.Expression);
                return string.Empty;
            default:
                throw Unexpected(key, "a property key");
        }
    }

    /// <summary>
    /// A member access: <c>super.x</c> and <c>super[x]</c>, a private name, a computed key, or a dotted
    /// name with the span of the name after the dot.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6C0882
    // Broiler-Human:        PENDING
    private JsExpression Member(MemberExpression member)
    {
        var span = At(member);

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
                NameSpan = At(name),
            },
            Expression key => new JsMemberExpression(span, target, string.Empty, Expression(key), member.IsOptional),
            _ => throw Unexpected(member.Property, "a member's property"),
        };
    }

    /// <summary>A call, or <c>super(...)</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=07E310
    // Broiler-Human:        PENDING
    private JsExpression Call(CallExpression call)
    {
        var span = At(call);

        if (call.Callee is SuperExpression)
        {
            return new JsSuperCallExpression(span, Arguments(call.Arguments));
        }

        var callee = Expression(call.Callee);

        return new JsCallExpression(span, callee, Arguments(call.Arguments), call.IsOptional);
    }

    /// <summary>An argument list, a spread argument at its <c>...</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A3952A
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsExpression> Arguments(
        System.Collections.Generic.IReadOnlyList<Expression> arguments)
    {
        var all = new System.Collections.Generic.List<JsExpression>(arguments.Count);

        foreach (var argument in arguments)
        {
            all.Add(argument is SpreadElement spread
                ? new JsSpreadElement(At(spread), Expression(spread.Argument))
                : Expression(argument));
        }

        return all;
    }

    /// <summary>
    /// An assignment: to a pattern, a destructuring assignment; otherwise to a reference, whose name
    /// written in parentheses is recorded as the wide parser recorded it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C1269D
    // Broiler-Human:        PENDING
    private JsExpression Assignment(AssignmentExpression assignment)
    {
        var span = At(assignment);

        if (assignment.Target is ArrayPattern or ObjectPattern)
        {
            var pattern = Pattern((Pattern)assignment.Target);
            return new JsDestructuringAssignment(span, pattern, Expression(assignment.Value));
        }

        var written = assignment.Target switch
        {
            TargetPattern target => target.Target,
            Expression expression => expression,
            _ => throw Unexpected(assignment.Target, "an assignment's target"),
        };

        var target2 = Expression(written);
        var value = Expression(assignment.Value);

        return new JsAssignmentExpression(span, Compound(assignment.Operator), target2, value)
        {
            ParenthesisedTarget = written is ParenthesizedExpression && target2 is JsIdentifier,
        };
    }

    /// <summary>An assignment operator, as the token of its compound form.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0118D7
    // Broiler-Human:        PENDING
    private static SliceTokenKind Compound(AssignmentOperator op) => op switch
    {
        AssignmentOperator.Assign => SliceTokenKind.Equals,
        AssignmentOperator.AddAssign => SliceTokenKind.Plus,
        AssignmentOperator.SubtractAssign => SliceTokenKind.Minus,
        AssignmentOperator.MultiplyAssign => SliceTokenKind.Star,
        AssignmentOperator.DivideAssign => SliceTokenKind.Slash,
        AssignmentOperator.RemainderAssign => SliceTokenKind.Percent,
        AssignmentOperator.LeftShiftAssign => SliceTokenKind.LessThanLessThan,
        AssignmentOperator.SignedRightShiftAssign => SliceTokenKind.GreaterThanGreaterThan,
        AssignmentOperator.UnsignedRightShiftAssign => SliceTokenKind.GreaterThanGreaterThanGreaterThan,
        AssignmentOperator.BitwiseAndAssign => SliceTokenKind.Ampersand,
        AssignmentOperator.BitwiseOrAssign => SliceTokenKind.Bar,
        AssignmentOperator.BitwiseXorAssign => SliceTokenKind.Caret,
        AssignmentOperator.ExponentAssign => SliceTokenKind.StarStar,
        AssignmentOperator.AndAssign => SliceTokenKind.AmpersandAmpersand,
        AssignmentOperator.OrAssign => SliceTokenKind.BarBar,
        AssignmentOperator.CoalesceAssign => SliceTokenKind.QuestionQuestion,
        _ => throw new System.ArgumentOutOfRangeException(nameof(op), op, "an assignment operator"),
    };

    /// <summary>A unary operator, as its token.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=DB4652
    // Broiler-Human:        PENDING
    private static SliceTokenKind Unary(UnaryOperator op) => op switch
    {
        UnaryOperator.Delete => SliceTokenKind.Delete,
        UnaryOperator.Void => SliceTokenKind.Void,
        UnaryOperator.Typeof => SliceTokenKind.Typeof,
        UnaryOperator.Plus => SliceTokenKind.Plus,
        UnaryOperator.Minus => SliceTokenKind.Minus,
        UnaryOperator.BitwiseNot => SliceTokenKind.Tilde,
        UnaryOperator.LogicalNot => SliceTokenKind.Bang,
        _ => throw new System.ArgumentOutOfRangeException(nameof(op), op, "a unary operator"),
    };

    /// <summary>A binary operator, as its token.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=349C88
    // Broiler-Human:        PENDING
    private static SliceTokenKind Binary(BinaryOperator op) => op switch
    {
        BinaryOperator.Exponent => SliceTokenKind.StarStar,
        BinaryOperator.Multiply => SliceTokenKind.Star,
        BinaryOperator.Divide => SliceTokenKind.Slash,
        BinaryOperator.Remainder => SliceTokenKind.Percent,
        BinaryOperator.Add => SliceTokenKind.Plus,
        BinaryOperator.Subtract => SliceTokenKind.Minus,
        BinaryOperator.LeftShift => SliceTokenKind.LessThanLessThan,
        BinaryOperator.SignedRightShift => SliceTokenKind.GreaterThanGreaterThan,
        BinaryOperator.UnsignedRightShift => SliceTokenKind.GreaterThanGreaterThanGreaterThan,
        BinaryOperator.LessThan => SliceTokenKind.LessThan,
        BinaryOperator.GreaterThan => SliceTokenKind.GreaterThan,
        BinaryOperator.LessThanOrEqual => SliceTokenKind.LessThanEquals,
        BinaryOperator.GreaterThanOrEqual => SliceTokenKind.GreaterThanEquals,
        BinaryOperator.Instanceof => SliceTokenKind.Instanceof,
        BinaryOperator.In => SliceTokenKind.In,
        BinaryOperator.Equal => SliceTokenKind.EqualsEquals,
        BinaryOperator.NotEqual => SliceTokenKind.BangEquals,
        BinaryOperator.StrictEqual => SliceTokenKind.EqualsEqualsEquals,
        BinaryOperator.StrictNotEqual => SliceTokenKind.BangEqualsEquals,
        BinaryOperator.BitwiseAnd => SliceTokenKind.Ampersand,
        BinaryOperator.BitwiseXor => SliceTokenKind.Caret,
        BinaryOperator.BitwiseOr => SliceTokenKind.Bar,
        _ => throw new System.ArgumentOutOfRangeException(nameof(op), op, "a binary operator"),
    };

    /// <summary>A logical operator, as its token.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D8DA74
    // Broiler-Human:        PENDING
    private static SliceTokenKind Logical(LogicalOperator op) => op switch
    {
        LogicalOperator.And => SliceTokenKind.AmpersandAmpersand,
        LogicalOperator.Or => SliceTokenKind.BarBar,
        _ => SliceTokenKind.QuestionQuestion,
    };
}
