// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   30
// Annotated:        30/30
// Exempt:           0
// Human-reviewed:   0/30
// IP risk:          Low
// Security risk:    High
// Criteria:         7/7
// Resource impact:  1/10 max
// Unverified:       30
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;
using NumericLiteral = Broiler.ECMAScript.Syntax.NumericLiteral;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The facts this family reads off the shared syntax library's tree the way step 1's adapter read them:
/// where a node begins, the anchors the tree does not carry, a parenthesis seen through, the directive
/// prologue, a property key's spelling, a module's names, and the operators as this family's tokens
/// (JSD-0083, decisions D15 and D20; milestone SJL-13.3, step 2).
/// </summary>
/// <remarks>
/// <para>
/// <b>Each is a pure function of the tree and the unit's text, tokens and admission facts</b>, typed on
/// the shared nodes, so a reading the adapter made once is made at every site by the same code, and a
/// raw test of a shared type that stands for one of these readings can be found by its name. Nothing is
/// cached and nothing is held between calls (rule N12).
/// </para>
/// <para>
/// <b>A parenthesis is seen through by a loop</b>, never by recursion, so the depth a parenthesised
/// expression nests to costs no stack here (<see cref="CompilationStack"/>).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B90ED1
// Broiler-Falsified-If: for a source step 1's adapter accepted, a span, an anchor, a key, a name, a prologue length or an operator read here differs from the one the adapter gave
// Broiler-Human:        PENDING
internal static class JsTreeFacts
{
    /// <summary>The name an anonymous <c>export default</c> class or function takes.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=7FF186
    // Broiler-Human:        PENDING
    internal const string DefaultExportName = "default";

    // ---- positions ---------------------------------------------------------------------------------

    /// <summary>Where a node begins, as this family's positions record it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E9FC15
    // Broiler-Human:        PENDING
    internal static SliceSourceSpan At(SyntaxNode node) => new(node.Span.Line, node.Span.Column);

    /// <summary>The offset just past a node's last character.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=45DFE7
    // Broiler-Human:        PENDING
    internal static int EndOf(SyntaxNode node) => node.Span.Offset + node.Span.Length;

    /// <summary>
    /// The wide parser's program span: its first token, which the module's retained tokens give, and
    /// otherwise the first statement, or the end of the source where it has none.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8AFE7A
    // Broiler-Human:        PENDING
    internal static SliceSourceSpan ProgramSpan(
        ProgramNode tree, System.Collections.Generic.IReadOnlyList<Token> tokens, string text)
    {
        if (tokens.Count > 0)
        {
            return new SliceSourceSpan(tokens[0].Line, tokens[0].Column);
        }

        return tree.Body.Count != 0 ? At(tree.Body[0]) : EndOfSource(text);
    }

    /// <summary>
    /// The line and column just past the source's last character, which is where the wide parser's
    /// end-of-source token stood: every line terminator of the specification starts a line, and CRLF
    /// starts one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=876072
    // Broiler-Human:        PENDING
    internal static SliceSourceSpan EndOfSource(string text)
    {
        var line = 1;
        var column = 1;

        for (var at = 0; at < text.Length; at++)
        {
            var c = text[at];

            if (c == '\r' && at + 1 < text.Length && text[at + 1] == '\n')
            {
                continue;
            }

            if (c is '\n' or '\r' or '\u2028' or '\u2029')
            {
                line++;
                column = 1;
                continue;
            }

            column++;
        }

        return new SliceSourceSpan(line, column);
    }

    /// <summary>The last retained token of <paramref name="kind"/> before <paramref name="node"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=F3227E
    // Broiler-Human:        PENDING
    internal static SliceSourceSpan TokenBefore(
        System.Collections.Generic.IReadOnlyList<Token> tokens, SyntaxNode node, TokenKind kind) =>
        TokenBefore(tokens, node, token => token.Kind == kind);

    /// <summary>The last retained token that is the word <paramref name="word"/> before <paramref name="node"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=022070
    // Broiler-Human:        PENDING
    internal static SliceSourceSpan TokenBefore(
        System.Collections.Generic.IReadOnlyList<Token> tokens, SyntaxNode node, Word word) =>
        TokenBefore(tokens, node, token => token.Word == word && !token.IsEscaped);

    /// <summary>
    /// The span of the last retained token before <paramref name="node"/> that <paramref name="matches"/>:
    /// an anchor the wide parser gave that the library's tree does not carry.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=798981
    // Broiler-Human:        PENDING
    internal static SliceSourceSpan TokenBefore(
        System.Collections.Generic.IReadOnlyList<Token> tokens, SyntaxNode node, System.Func<Token, bool> matches)
    {
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

    /// <summary>A node no reading here admits, which the library's grammar does not produce where it stands.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=9547A9
    // Broiler-Human:        PENDING
    internal static System.InvalidOperationException Unexpected(SyntaxNode node, string where) =>
        new("the shared tree holds a " + node.Kind + " at " + node.Span.Line + ":" + node.Span.Column +
            " where this compiler reads " + where);

    // ---- shapes ------------------------------------------------------------------------------------

    /// <summary>The expression a parenthesis holds, through every parenthesis around it, by a loop.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5116A5
    // Broiler-Falsified-If: a parenthesised expression is read as anything but the expression it holds, or seeing through one recurses
    // Broiler-Human:        PENDING
    internal static Expression Unwrap(Expression expression)
    {
        while (expression is ParenthesizedExpression parenthesised)
        {
            expression = parenthesised.Expression;
        }

        return expression;
    }

    /// <summary>
    /// How many statements a body's directive prologue holds, as the wide parser split it: the leading
    /// run of statements that are a string literal alone, unparenthesised.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=65881B
    // Broiler-Falsified-If: a statement the wide parser read as a directive is counted as a body statement, or one it read as a body statement is counted as a directive
    // Broiler-Human:        PENDING
    internal static int PrologueLength(System.Collections.Generic.IReadOnlyList<Statement> statements)
    {
        var at = 0;

        while (at < statements.Count && statements[at] is ExpressionStatement { Expression: StringLiteral })
        {
            at++;
        }

        return at;
    }

    /// <summary>Whether a pattern is a name alone, and that name: a binding, or a reference to a name, parenthesised or not.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=735EF3
    // Broiler-Human:        PENDING
    internal static bool IsSimpleName(SyntaxNode target, out string name)
    {
        switch (target)
        {
            case BindingIdentifier binding:
                name = binding.Name;
                return true;
            case TargetPattern { Target: var written } when Unwrap(written) is Identifier identifier:
                name = identifier.Name;
                return true;
            case Expression written when Unwrap(written) is Identifier identifier:
                name = identifier.Name;
                return true;
            default:
                name = string.Empty;
                return false;
        }
    }

    /// <summary>Whether an object literal's entry is written in method form: a method, a getter or a setter.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=82206C
    // Broiler-Human:        PENDING
    internal static bool IsMethodEntry(PropertyDefinition definition) =>
        definition.PropertyKind is PropertyKind.Method or PropertyKind.Get or PropertyKind.Set;

    /// <summary>
    /// Whether an <c>if</c> clause is a function declaration, which the wide parser read as a block of its
    /// own at the function's span (Annex B.3.3's FunctionDeclarations in IfStatement clauses).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=B2DDC7
    // Broiler-Human:        PENDING
    internal static bool IsIfClauseFunction(Statement clause) => clause is FunctionDeclaration;

    /// <summary>The name an <c>export default</c> class or function binds: its own, or <c>default</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=2B9DA4
    // Broiler-Human:        PENDING
    internal static string DefaultExportBinding(BindingIdentifier? own) => own?.Name ?? DefaultExportName;

    // ---- keys and names ----------------------------------------------------------------------------

    /// <summary>
    /// A property key: a name, a string's value, a number by <c>NumberText</c>, a BigInt by its decimal
    /// digits, a private name with its <c>#</c>, or the empty key and the expression of a computed one.
    /// </summary>
    /// <param name="key">The key as the tree holds it.</param>
    /// <param name="bigInts">The admission walk's values, a BigInt key's included.</param>
    /// <param name="computed">A computed key's expression, else <see langword="null"/>.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=17A295
    // Broiler-Falsified-If: a key's spelling differs from the wide parser's for any literal key
    // Broiler-Human:        PENDING
    internal static string Key(
        SyntaxNode key,
        System.Collections.Generic.IReadOnlyDictionary<NumericLiteral, System.Numerics.BigInteger> bigInts,
        out Expression? computed)
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
                return Broiler.ECMAScript.Numbers.NumberText.ToString(number.Value);
            case ComputedPropertyName name:
                computed = name.Expression;
                return string.Empty;
            default:
                throw Unexpected(key, "a property key");
        }
    }

    /// <summary>A module export name: an identifier's name, or a string's value.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=DA03A2
    // Broiler-Human:        PENDING
    internal static string ExportName(SyntaxNode name) => name switch
    {
        IdentifierName identifier => identifier.Name,
        StringLiteral literal => literal.Value,
        BindingIdentifier binding => binding.Name,
        _ => throw Unexpected(name, "a module export name"),
    };

    /// <summary>
    /// A module specifier, or the empty one where there is none: a missing <c>from</c> and <c>from ""</c>
    /// read alike, as the wide parser's tree held them.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=3C7AAF
    // Broiler-Falsified-If: a module specifier written as the empty string is read apart from a missing one, or either reads as anything but empty
    // Broiler-Human:        PENDING
    internal static string SourceOrNone(StringLiteral? source) => source?.Value ?? string.Empty;

    /// <summary>Every name a declaration's pattern binds, in source order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=CC4BCE
    // Broiler-Human:        PENDING
    internal static void PatternNames(Pattern pattern, System.Collections.Generic.List<string> names)
    {
        switch (pattern)
        {
            case BindingIdentifier identifier:
                names.Add(identifier.Name);
                return;
            case TargetPattern { Target: var written } when Unwrap(written) is Identifier identifier:
                names.Add(identifier.Name);
                return;
            case AssignmentPattern defaulted:
                PatternNames(defaulted.Target, names);
                return;
            case RestElement rest:
                PatternNames(rest.Argument, names);
                return;
            case ArrayPattern array:
                foreach (var element in array.Elements)
                {
                    if (element is not null)
                    {
                        PatternNames(element, names);
                    }
                }

                return;
            case ObjectPattern literal:
                foreach (var property in literal.Properties)
                {
                    switch (property)
                    {
                        case PatternProperty entry:
                            PatternNames(entry.Value, names);
                            break;
                        case RestElement rest:
                            PatternNames(rest.Argument, names);
                            break;
                    }
                }

                return;
            default:
                return;
        }
    }

    // ---- module declarations -----------------------------------------------------------------------

    /// <summary>
    /// An <c>import</c> declaration's bindings in the wide parser's order: the default binding, then the
    /// namespace binding at its <c>*</c> or the named bindings, each at its own span.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A88474
    // Broiler-Falsified-If: an import's bindings, their order, names or spans differ from the wide parser's
    // Broiler-Human:        PENDING
    internal static System.Collections.Generic.List<(SliceSourceSpan Span, string Imported, string Local, bool Namespace)> ImportBindings(
        ImportDeclaration import, System.Collections.Generic.IReadOnlyList<Token> tokens)
    {
        var bindings = new System.Collections.Generic.List<(SliceSourceSpan, string, string, bool)>();

        if (import.DefaultBinding is { } binding)
        {
            bindings.Add((At(binding), DefaultExportName, binding.Name, false));
        }

        if (import.NamespaceBinding is { } space)
        {
            bindings.Add((TokenBefore(tokens, space, TokenKind.Star), string.Empty, space.Name, true));
        }

        if (import.NamedImports is { } named)
        {
            foreach (var specifier in named)
            {
                var local = specifier.Local.Name;
                var imported = specifier.Imported is null ? local : ExportName(specifier.Imported);
                bindings.Add((At(specifier), imported, local, false));
            }
        }

        return bindings;
    }

    /// <summary>
    /// The names an exported declaration publishes, as themselves, at the declaration's span: a class's
    /// or a function's name, or every name a variable declaration binds, in source order.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=27D2ED
    // Broiler-Falsified-If: an exported declaration's names, their order or span differ from the wide parser's
    // Broiler-Human:        PENDING
    internal static System.Collections.Generic.List<(SliceSourceSpan Span, string Local, string Exported)> DeclarationExports(
        Statement declared)
    {
        var at = At(declared);
        var exports = new System.Collections.Generic.List<(SliceSourceSpan, string, string)>();

        switch (declared)
        {
            case ClassDeclaration { Class.Name: var name }:
                exports.Add((at, name?.Name ?? string.Empty, name?.Name ?? string.Empty));
                break;
            case FunctionDeclaration { Function.Name: var name }:
                exports.Add((at, name?.Name ?? string.Empty, name?.Name ?? string.Empty));
                break;
            case VariableDeclaration variables:
                foreach (var declarator in variables.Declarations)
                {
                    var bound = new System.Collections.Generic.List<string>();
                    PatternNames(declarator.Target, bound);

                    foreach (var bind in bound)
                    {
                        exports.Add((at, bind, bind));
                    }
                }

                break;
            default:
                throw Unexpected(declared, "an exported declaration");
        }

        return exports;
    }

    /// <summary>An export clause's names, each at its specifier: the local name, and the exported one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DEB75C
    // Broiler-Human:        PENDING
    internal static System.Collections.Generic.List<(SliceSourceSpan Span, string Local, string Exported)> ClauseExports(
        System.Collections.Generic.IReadOnlyList<ExportSpecifier> specifiers)
    {
        var exports = new System.Collections.Generic.List<(SliceSourceSpan, string, string)>(specifiers.Count);

        foreach (var specifier in specifiers)
        {
            var local = ExportName(specifier.Local);
            var exported = specifier.Exported is null ? local : ExportName(specifier.Exported);
            exports.Add((At(specifier), local, exported));
        }

        return exports;
    }

    /// <summary>An attribute clause's entries, each at its key; none is the empty clause.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=523E17
    // Broiler-Human:        PENDING
    internal static System.Collections.Generic.List<(SliceSourceSpan Span, string Key, string Value)> Attributes(
        System.Collections.Generic.IReadOnlyList<ImportAttribute>? attributes)
    {
        var all = new System.Collections.Generic.List<(SliceSourceSpan, string, string)>();

        if (attributes is null)
        {
            return all;
        }

        foreach (var attribute in attributes)
        {
            all.Add((At(attribute), ExportName(attribute.Key), attribute.Value.Value));
        }

        return all;
    }

    // ---- operators and kinds, as this family's tokens -----------------------------------------------

    /// <summary>A declaration's kind, and whether it registers its bindings for disposal.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=9D5BCB
    // Broiler-Human:        PENDING
    internal static (SliceDeclarationKind Kind, JsUsing Using) Kind(DeclarationKind kind) => kind switch
    {
        DeclarationKind.Var => (SliceDeclarationKind.Var, JsUsing.None),
        DeclarationKind.Let => (SliceDeclarationKind.Let, JsUsing.None),
        DeclarationKind.Const => (SliceDeclarationKind.Const, JsUsing.None),
        DeclarationKind.Using => (SliceDeclarationKind.Const, JsUsing.Sync),
        _ => (SliceDeclarationKind.Const, JsUsing.Async),
    };

    /// <summary>An update operator, as its token.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B312A0
    // Broiler-Human:        PENDING
    internal static SliceTokenKind Update(UpdateOperator op) =>
        op == UpdateOperator.Increment ? SliceTokenKind.PlusPlus : SliceTokenKind.MinusMinus;

    /// <summary>An assignment operator, as the token of its compound form.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=BC8AC9
    // Broiler-Human:        PENDING
    internal static SliceTokenKind Compound(AssignmentOperator op) => op switch
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=E53F88
    // Broiler-Human:        PENDING
    internal static SliceTokenKind Unary(UnaryOperator op) => op switch
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=97D6D8
    // Broiler-Human:        PENDING
    internal static SliceTokenKind Binary(BinaryOperator op) => op switch
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=9ECCBF
    // Broiler-Human:        PENDING
    internal static SliceTokenKind Logical(LogicalOperator op) => op switch
    {
        LogicalOperator.And => SliceTokenKind.AmpersandAmpersand,
        LogicalOperator.Or => SliceTokenKind.BarBar,
        _ => SliceTokenKind.QuestionQuestion,
    };
}
