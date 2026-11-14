// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   16
// Annotated:        16/16
// Exempt:           4
// Human-reviewed:   0/16
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  3/10 max
// Unverified:       16
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;
using NumericLiteral = Broiler.ECMAScript.Syntax.NumericLiteral;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// Builds this family's <see cref="JsSyntax"/> tree from the shared syntax library's tree, reproducing
/// every anchor and offset the wide parser gave (JSD-0083, decision D15; milestone SJL-13.3, step 1).
/// </summary>
/// <remarks>
/// <para>
/// <b>The lowering is unchanged, so the tree it reads must be the one it read.</b> Every node carries
/// the line and column the wide parser gave it - the first token of its production, which is where the
/// library's full span starts - and the facts the wide parser recorded beside the shape: a function's
/// and a class's source offsets, a dotted member's name span, a string's raw text, a template's cooked
/// and raw chunks, a numeric key's spelling by <c>NumberText</c>, a parenthesised name as an assignment
/// target, an array or object literal's trailing comma by the wide parser's reading, the directive
/// prologue apart from the body, and a function declaration that is an <c>if</c> clause in a block of
/// its own. Where the library's tree does not carry an anchor - the program's first token, the <c>*</c>
/// of a namespace import and the <c>as</c> of a re-exported namespace - it is read from the module's
/// retained tokens, or from the first statement of a script, which is where that token stands.
/// </para>
/// <para>
/// <b>The admission walk is made before the tree is adapted</b> (decision D18; SJL-13.3, step 2): it walks
/// the shared tree itself, and a unit it refuses is not adapted. Each admitted BigInt's value is the walk's,
/// and so is whether the unit awaits at its top level.
/// </para>
/// <para>
/// <b>It lives for one parse and holds no shared state</b> (rule N12).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=818D37
// Broiler-Falsified-If: for a source the wide parser accepted, a node, a field, a span or an offset of the tree built here differs from the one the wide parser built at step 1's base
// Broiler-Human:        PENDING
internal sealed partial class JsFrontendAdapter
{
    /// <summary>The unit's source text, which the raw text of a string literal is cut from.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A42375
    // Broiler-Human:        PENDING
    private readonly string text;

    /// <summary>The library's answer: its tree, its tokens for a module, and its declaration tables.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=472BF9
    // Broiler-Human:        PENDING
    private readonly ParseResult result;

    /// <summary>The strictness the library decided for each function and for the program.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=3AEB66
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.IReadOnlyDictionary<SyntaxNode, bool> strictness;

    /// <summary>Each admitted BigInt literal's value, a key's included, which the admission walk recorded.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=728EFC
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.IReadOnlyDictionary<NumericLiteral, System.Numerics.BigInteger> bigInts;

    /// <summary>
    /// Adapts the library's accepted parse of <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The unit's source text.</param>
    /// <param name="parsed">The library's answer, which reported nothing.</param>
    /// <param name="bigIntValues">The admission walk's values, of a unit it refused nothing in.</param>
    /// <param name="strictness">The strictness the library decided for each function and for the program.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=0234CA
    // Broiler-Human:        PENDING
    internal JsFrontendAdapter(
        string source,
        ParseResult parsed,
        System.Collections.Generic.IReadOnlyDictionary<NumericLiteral, System.Numerics.BigInteger> bigIntValues,
        System.Collections.Generic.IReadOnlyDictionary<SyntaxNode, bool> strictness)
    {
        text = source;
        result = parsed;
        bigInts = bigIntValues;
        this.strictness = strictness;
    }

    /// <summary>The program: its directive prologue, its statements and its strictness.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CACD00
    // Broiler-Human:        PENDING
    internal JsProgramNode Program()
    {
        var tree = result.Tree!;
        var body = new System.Collections.Generic.List<JsStatement>(tree.Body.Count);
        var directives = Prologue(tree.Body, body);

        return new JsProgramNode(JsTreeFacts.ProgramSpan(tree, result.Tokens, text), directives, body, Strict(tree));
    }

    /// <summary>
    /// Splits a body's directive prologue from its statements as the wide parser did: the leading run
    /// of statements that are a string literal alone, each a directive, and everything after them the
    /// body, adapted into <paramref name="body"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E2A141
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsStringLiteral> Prologue(
        System.Collections.Generic.IReadOnlyList<Statement> statements,
        System.Collections.Generic.List<JsStatement> body)
    {
        var directives = new System.Collections.Generic.List<JsStringLiteral>();
        var at = 0;

        var prologue = JsTreeFacts.PrologueLength(statements);

        for (; at < prologue; at++)
        {
            directives.Add(String((StringLiteral)((ExpressionStatement)statements[at]).Expression));
        }

        for (; at < statements.Count; at++)
        {
            body.Add(Statement(statements[at]));
        }

        return directives;
    }

    /// <summary>The strictness the library decided for a function or the program.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=1F2663
    // Broiler-Human:        PENDING
    private bool Strict(SyntaxNode node) =>
        strictness.TryGetValue(node, out var strict)
            ? strict
            : throw new System.InvalidOperationException(
                "the library's declaration tables hold no facts for the " + node.Kind + " at " +
                node.Span.Line + ":" + node.Span.Column);

    // ---- statements -----------------------------------------------------------------------------

    /// <summary>One statement.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D9341D
    // Broiler-Human:        PENDING
    private JsStatement Statement(Statement statement)
    {
        var span = JsTreeFacts.At(statement);

        switch (statement)
        {
            case EmptyStatement:
                return new JsEmptyStatement(span);
            case BlockStatement block:
                return Block(block);
            case VariableDeclaration declaration:
                return Variables(declaration);
            case ExpressionStatement expression:
                return new JsExpressionStatement(span, Expression(expression.Expression));
            case IfStatement branch:
                return new JsIfStatement(
                    span,
                    Expression(branch.Test),
                    Clause(Statement(branch.Consequent)),
                    branch.Alternate is null ? null : Clause(Statement(branch.Alternate)));
            case WithStatement with:
                return new JsWithStatement(span, Expression(with.Object), Statement(with.Body));
            case WhileStatement loop:
                return new JsWhileStatement(span, Expression(loop.Test), Statement(loop.Body));
            case DoWhileStatement loop:
            {
                var body = Statement(loop.Body);
                return new JsDoWhileStatement(span, body, Expression(loop.Test));
            }

            case ForStatement loop:
                return For(loop);
            case ForInStatement loop:
                return ForIn(loop);
            case ForOfStatement loop:
                return ForOf(loop);
            case BreakStatement jump:
                return new JsBreakStatement(span, jump.Label?.Name ?? string.Empty);
            case ContinueStatement jump:
                return new JsContinueStatement(span, jump.Label?.Name ?? string.Empty);
            case ReturnStatement exit:
                return new JsReturnStatement(span, exit.Argument is null ? null : Expression(exit.Argument));
            case ThrowStatement exit:
                return new JsThrowStatement(span, Expression(exit.Argument));
            case TryStatement attempt:
                return Try(attempt);
            case SwitchStatement choice:
                return Switch(choice);
            case LabelledStatement labelled:
                return new JsLabelledStatement(span, labelled.Label.Name, Statement(labelled.Body));
            case DebuggerStatement:
                return new JsDebuggerStatement(span);
            case FunctionDeclaration declared:
                return new JsFunctionDeclaration(span, Function(declared.Function, span, string.Empty));
            case ClassDeclaration declared:
                return new JsClassDeclaration(span, Class(declared.Class, string.Empty));
            case ImportDeclaration import:
                return Import(import);
            case ExportNamedDeclaration export:
                return ExportNamed(export);
            case ExportDefaultDeclaration export:
                return ExportDefault(export);
            case ExportAllDeclaration export:
                return ExportAll(export);
            default:
                throw JsTreeFacts.Unexpected(statement, "a statement");
        }
    }

    /// <summary>
    /// An <c>if</c> clause: a function declaration there is a block of its own, at the function's span,
    /// as the wide parser wrapped it (Annex B.3.3's FunctionDeclarations in IfStatement clauses).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E259A6
    // Broiler-Human:        PENDING
    private static JsStatement Clause(JsStatement statement) =>
        statement is JsFunctionDeclaration
            ? new JsBlockStatement(statement.Span, [statement])
            : statement;

    /// <summary>A braced statement list.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=5F17E6
    // Broiler-Human:        PENDING
    private JsBlockStatement Block(BlockStatement block)
    {
        var body = new System.Collections.Generic.List<JsStatement>(block.Body.Count);

        foreach (var statement in block.Body)
        {
            body.Add(Statement(statement));
        }

        return new JsBlockStatement(JsTreeFacts.At(block), body);
    }

    /// <summary>
    /// A variable statement: a <c>using</c> or an <c>await using</c> declaration is a <c>const</c> one
    /// with its disposal recorded.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5203E4
    // Broiler-Human:        PENDING
    private JsVariableStatement Variables(VariableDeclaration declaration)
    {
        var (kind, resource) = JsTreeFacts.Kind(declaration.DeclarationKind);

        return new JsVariableStatement(JsTreeFacts.At(declaration), kind, Declarators(declaration), resource);
    }

    /// <summary>A declaration's declarators: a name, or a pattern, and the initialiser.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=BD077C
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsDeclarator> Declarators(VariableDeclaration declaration)
    {
        var declarators = new System.Collections.Generic.List<JsDeclarator>(declaration.Declarations.Count);

        foreach (var declarator in declaration.Declarations)
        {
            var pattern = declarator.Target is BindingIdentifier ? null : Pattern(declarator.Target);
            var name = declarator.Target is BindingIdentifier identifier ? identifier.Name : string.Empty;

            declarators.Add(new JsDeclarator(
                JsTreeFacts.At(declarator),
                name,
                pattern,
                declarator.Initializer is null ? null : Expression(declarator.Initializer)));
        }

        return declarators;
    }

    /// <summary><c>for (init; test; update) body</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=423D01
    // Broiler-Human:        PENDING
    private JsStatement For(ForStatement loop)
    {
        JsStatement? initialiser = loop.Init switch
        {
            null => null,
            VariableDeclaration declaration => Variables(declaration),
            Expression expression => new JsExpressionStatement(JsTreeFacts.At(expression), Expression(expression)),
            _ => throw JsTreeFacts.Unexpected(loop.Init, "a for head"),
        };

        var test = loop.Test is null ? null : Expression(loop.Test);
        var update = loop.Update is null ? null : Expression(loop.Update);

        return new JsForStatement(JsTreeFacts.At(loop), initialiser, test, update, Statement(loop.Body));
    }

    /// <summary>
    /// The left side of a <c>for … in</c> or <c>for … of</c> head: a declaration's kind, name, pattern and
    /// Annex B initialiser, or an assignment target, a pattern when it destructures.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8CC1A8
    // Broiler-Human:        PENDING
    private (SliceDeclarationKind? Kind, string Name, JsPattern? Pattern, JsExpression? Target, JsExpression? Initialiser, JsUsing Using)
        Head(SyntaxNode left)
    {
        switch (left)
        {
            case VariableDeclaration declaration:
            {
                var (kind, resource) = JsTreeFacts.Kind(declaration.DeclarationKind);
                var declarator = Declarators(declaration)[0];
                return (kind, declarator.Name, declarator.Pattern, null, declarator.Initialiser, resource);
            }

            case ArrayPattern or ObjectPattern:
                return (null, string.Empty, Pattern((Pattern)left), null, null, JsUsing.None);
            case TargetPattern target:
                return (null, string.Empty, null, Expression(target.Target), null, JsUsing.None);
            case Expression expression:
                return (null, string.Empty, null, Expression(expression), null, JsUsing.None);
            default:
                throw JsTreeFacts.Unexpected(left, "a for-in or for-of head");
        }
    }

    /// <summary><c>for (left in right) body</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=885E59
    // Broiler-Human:        PENDING
    private JsStatement ForIn(ForInStatement loop)
    {
        var head = Head(loop.Left);
        var right = Expression(loop.Right);

        return new JsForInStatement(
            JsTreeFacts.At(loop), head.Kind, head.Name, head.Pattern, head.Target, right, Statement(loop.Body), head.Initialiser);
    }

    /// <summary><c>for (left of right) body</c>, and <c>for await</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=980E25
    // Broiler-Human:        PENDING
    private JsStatement ForOf(ForOfStatement loop)
    {
        var head = Head(loop.Left);
        var right = Expression(loop.Right);

        return new JsForOfStatement(
            JsTreeFacts.At(loop), head.Kind, head.Name, head.Pattern, head.Target, right, Statement(loop.Body), loop.IsAwait,
            head.Using);
    }

    /// <summary><c>try</c>: the block, the catch parameter's name or pattern, the handler and the finaliser.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=1F1B54
    // Broiler-Human:        PENDING
    private JsStatement Try(TryStatement attempt)
    {
        var block = Block(attempt.Block);
        var parameter = string.Empty;
        JsPattern? pattern = null;
        JsBlockStatement? handler = null;

        if (attempt.Handler is { } clause)
        {
            switch (clause.Parameter)
            {
                case null:
                    break;
                case BindingIdentifier identifier:
                    parameter = identifier.Name;
                    break;
                default:
                    pattern = Pattern(clause.Parameter);
                    break;
            }

            handler = Block(clause.Body);
        }

        var finaliser = attempt.Finalizer is null ? null : Block(attempt.Finalizer);

        return new JsTryStatement(JsTreeFacts.At(attempt), block, parameter, pattern, handler, finaliser);
    }

    /// <summary><c>switch</c> and its clauses.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=901388
    // Broiler-Human:        PENDING
    private JsStatement Switch(SwitchStatement choice)
    {
        var discriminant = Expression(choice.Discriminant);
        var clauses = new System.Collections.Generic.List<JsSwitchClause>(choice.Cases.Count);

        foreach (var clause in choice.Cases)
        {
            var test = clause.Test is null ? null : Expression(clause.Test);
            var body = new System.Collections.Generic.List<JsStatement>(clause.Consequent.Count);

            foreach (var statement in clause.Consequent)
            {
                body.Add(Statement(statement));
            }

            clauses.Add(new JsSwitchClause(JsTreeFacts.At(clause), test, body));
        }

        return new JsSwitchStatement(JsTreeFacts.At(choice), discriminant, clauses);
    }
}
