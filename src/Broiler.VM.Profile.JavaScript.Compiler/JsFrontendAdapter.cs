// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   22
// Annotated:        22/22
// Exempt:           6
// Human-reviewed:   0/22
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  3/10 max
// Unverified:       22
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;

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
/// <b>The admission walk is made as the tree is adapted</b> (decision D18): every node is visited once,
/// after a parse that reported nothing, and the walk's refusals are collected in
/// <see cref="Admission"/> in source order; a unit with any is refused and its tree not used.
/// </para>
/// <para>
/// <b>Whether a module awaits at its top level is the wide parser's rule</b> (decision D20): an
/// <c>await</c> expression, a <c>for await</c> or an <c>await using</c> that no function other than an
/// arrow encloses, which is what <c>SawTopLevelAwait</c> answered.
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7D1C9D
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<SyntaxNode, bool> strictness =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

    /// <summary>How many functions other than arrows enclose the node being adapted.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=837D17
    // Broiler-Human:        PENDING
    private int functionDepth;

    /// <summary>
    /// Adapts the library's accepted parse of <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The unit's source text.</param>
    /// <param name="parsed">The library's answer, which reported nothing.</param>
    /// <param name="options">The unit's options: eval code inside a function starts one function deep.</param>
    /// <param name="admitsBigInt">Whether the request's manifest admits BigInt literals.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=2FBBBD
    // Broiler-Human:        PENDING
    internal JsFrontendAdapter(string source, ParseResult parsed, SliceParseOptions options, bool admitsBigInt)
    {
        text = source;
        result = parsed;
        Admission = new JsAdmissionWalk(admitsBigInt);
        functionDepth = options.IsEval && (options.EvalFlags & Format.JsFormat.EvalRequestFlags.InFunction) != 0
            ? 1
            : 0;

        var tables = parsed.Declarations ?? throw new System.InvalidOperationException(
            "the library answered an accepted parse with no declaration tables");

        foreach (var facts in tables.Functions)
        {
            strictness[facts.Node] = facts.IsStrict;
        }
    }

    /// <summary>The admission walk's refusals, in source order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=31409F
    // Broiler-Human:        PENDING
    internal JsAdmissionWalk Admission { get; }

    /// <summary>Whether the unit awaits outside every function, by the wide parser's rule.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=ACE465
    // Broiler-Human:        PENDING
    internal bool SawTopLevelAwait { get; private set; }

    /// <summary>The program: its directive prologue, its statements and its strictness.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E5AA76
    // Broiler-Human:        PENDING
    internal JsProgramNode Program()
    {
        var tree = result.Tree!;
        var body = new System.Collections.Generic.List<JsStatement>(tree.Body.Count);
        var directives = Prologue(tree.Body, body);

        return new JsProgramNode(ProgramSpan(tree), directives, body, Strict(tree));
    }

    /// <summary>
    /// The wide parser's program span: its first token, which the module's retained tokens give, and
    /// otherwise the first statement, or the end of the source where it has none.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BF7B3C
    // Broiler-Human:        PENDING
    private SliceSourceSpan ProgramSpan(ProgramNode tree)
    {
        if (result.Tokens is { Count: > 0 } tokens)
        {
            return new SliceSourceSpan(tokens[0].Line, tokens[0].Column);
        }

        return tree.Body.Count != 0 ? At(tree.Body[0]) : EndOfSource();
    }

    /// <summary>
    /// The line and column just past the source's last character, which is where the wide parser's
    /// end-of-source token stood: every line terminator of the specification starts a line, and CRLF
    /// starts one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=55E8C3
    // Broiler-Human:        PENDING
    private SliceSourceSpan EndOfSource()
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

    /// <summary>
    /// Splits a body's directive prologue from its statements as the wide parser did: the leading run
    /// of statements that are a string literal alone, each a directive, and everything after them the
    /// body, adapted into <paramref name="body"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=144C89
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsStringLiteral> Prologue(
        System.Collections.Generic.IReadOnlyList<Statement> statements,
        System.Collections.Generic.List<JsStatement> body)
    {
        var directives = new System.Collections.Generic.List<JsStringLiteral>();
        var at = 0;

        while (at < statements.Count && statements[at] is ExpressionStatement { Expression: StringLiteral literal })
        {
            directives.Add(String(literal));
            at++;
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

    /// <summary>Where a node begins, as this family's tree records it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=F25A44
    // Broiler-Human:        PENDING
    private static SliceSourceSpan At(SyntaxNode node) => new(node.Span.Line, node.Span.Column);

    /// <summary>The offset just past a node's last character.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=73B89C
    // Broiler-Human:        PENDING
    private static int EndOf(SyntaxNode node) => node.Span.Offset + node.Span.Length;

    /// <summary>A node no arm of this adapter reads, which the library's grammar does not produce here.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=17CF82
    // Broiler-Human:        PENDING
    private static System.InvalidOperationException Unexpected(SyntaxNode node, string where) =>
        new("the shared tree holds a " + node.Kind + " at " + node.Span.Line + ":" + node.Span.Column +
            " where this adapter reads " + where);

    // ---- statements -----------------------------------------------------------------------------

    /// <summary>One statement.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=AF80FA
    // Broiler-Human:        PENDING
    private JsStatement Statement(Statement statement)
    {
        var span = At(statement);

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
                throw Unexpected(statement, "a statement");
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7AECF6
    // Broiler-Human:        PENDING
    private JsBlockStatement Block(BlockStatement block)
    {
        var body = new System.Collections.Generic.List<JsStatement>(block.Body.Count);

        foreach (var statement in block.Body)
        {
            body.Add(Statement(statement));
        }

        return new JsBlockStatement(At(block), body);
    }

    /// <summary>
    /// A variable statement: a <c>using</c> or an <c>await using</c> declaration is a <c>const</c> one
    /// with its disposal recorded, and an <c>await using</c> outside every function is a top-level await.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=90FCC4
    // Broiler-Human:        PENDING
    private JsVariableStatement Variables(VariableDeclaration declaration)
    {
        var (kind, resource) = Kind(declaration.DeclarationKind);

        if (resource == JsUsing.Async)
        {
            SawTopLevelAwait |= functionDepth == 0;
        }

        return new JsVariableStatement(At(declaration), kind, Declarators(declaration), resource);
    }

    /// <summary>A declaration's kind, and whether it registers its bindings for disposal.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=73F32E
    // Broiler-Human:        PENDING
    private static (SliceDeclarationKind Kind, JsUsing Using) Kind(DeclarationKind kind) => kind switch
    {
        DeclarationKind.Var => (SliceDeclarationKind.Var, JsUsing.None),
        DeclarationKind.Let => (SliceDeclarationKind.Let, JsUsing.None),
        DeclarationKind.Const => (SliceDeclarationKind.Const, JsUsing.None),
        DeclarationKind.Using => (SliceDeclarationKind.Const, JsUsing.Sync),
        _ => (SliceDeclarationKind.Const, JsUsing.Async),
    };

    /// <summary>A declaration's declarators: a name, or a pattern, and the initialiser.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=6F1070
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsDeclarator> Declarators(VariableDeclaration declaration)
    {
        var declarators = new System.Collections.Generic.List<JsDeclarator>(declaration.Declarations.Count);

        foreach (var declarator in declaration.Declarations)
        {
            var pattern = declarator.Target is BindingIdentifier ? null : Pattern(declarator.Target);
            var name = declarator.Target is BindingIdentifier identifier ? identifier.Name : string.Empty;

            declarators.Add(new JsDeclarator(
                At(declarator),
                name,
                pattern,
                declarator.Initializer is null ? null : Expression(declarator.Initializer)));
        }

        return declarators;
    }

    /// <summary><c>for (init; test; update) body</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CCC61B
    // Broiler-Human:        PENDING
    private JsStatement For(ForStatement loop)
    {
        JsStatement? initialiser = loop.Init switch
        {
            null => null,
            VariableDeclaration declaration => Variables(declaration),
            Expression expression => new JsExpressionStatement(At(expression), Expression(expression)),
            _ => throw Unexpected(loop.Init, "a for head"),
        };

        var test = loop.Test is null ? null : Expression(loop.Test);
        var update = loop.Update is null ? null : Expression(loop.Update);

        return new JsForStatement(At(loop), initialiser, test, update, Statement(loop.Body));
    }

    /// <summary>
    /// The left side of a <c>for … in</c> or <c>for … of</c> head: a declaration's kind, name, pattern and
    /// Annex B initialiser, or an assignment target, a pattern when it destructures.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1E72B4
    // Broiler-Human:        PENDING
    private (SliceDeclarationKind? Kind, string Name, JsPattern? Pattern, JsExpression? Target, JsExpression? Initialiser, JsUsing Using)
        Head(SyntaxNode left)
    {
        switch (left)
        {
            case VariableDeclaration declaration:
            {
                var (kind, resource) = Kind(declaration.DeclarationKind);

                if (resource == JsUsing.Async)
                {
                    SawTopLevelAwait |= functionDepth == 0;
                }

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
                throw Unexpected(left, "a for-in or for-of head");
        }
    }

    /// <summary><c>for (left in right) body</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EDC2D7
    // Broiler-Human:        PENDING
    private JsStatement ForIn(ForInStatement loop)
    {
        var head = Head(loop.Left);
        var right = Expression(loop.Right);

        return new JsForInStatement(
            At(loop), head.Kind, head.Name, head.Pattern, head.Target, right, Statement(loop.Body), head.Initialiser);
    }

    /// <summary><c>for (left of right) body</c>, and <c>for await</c>, a top-level await outside every function.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0F26CE
    // Broiler-Human:        PENDING
    private JsStatement ForOf(ForOfStatement loop)
    {
        if (loop.IsAwait)
        {
            SawTopLevelAwait |= functionDepth == 0;
        }

        var head = Head(loop.Left);
        var right = Expression(loop.Right);

        return new JsForOfStatement(
            At(loop), head.Kind, head.Name, head.Pattern, head.Target, right, Statement(loop.Body), loop.IsAwait,
            head.Using);
    }

    /// <summary><c>try</c>: the block, the catch parameter's name or pattern, the handler and the finaliser.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=1F1871
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

        return new JsTryStatement(At(attempt), block, parameter, pattern, handler, finaliser);
    }

    /// <summary><c>switch</c> and its clauses.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=96CA87
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

            clauses.Add(new JsSwitchClause(At(clause), test, body));
        }

        return new JsSwitchStatement(At(choice), discriminant, clauses);
    }
}
