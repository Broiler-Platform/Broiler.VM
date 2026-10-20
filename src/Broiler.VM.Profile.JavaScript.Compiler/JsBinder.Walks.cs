// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   36
// Annotated:        36/36
// Exempt:           9
// Human-reviewed:   0/36
// IP risk:          None
// Security risk:    High
// Criteria:         8/6
// Resource impact:  3/10 max
// Unverified:       36
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>The binder's walks, every one of them over an explicit stack.</summary>
/// <remarks>
/// <para>
/// <b>Two kinds of walk, and the difference is what decides an order.</b> The walk over the whole
/// unit meets every node once, with the strictness and the jump targets in force where the node
/// stands, and records each fact against the node it is about; no fact it records depends on the
/// order it meets nodes in, so it takes them in whatever order its stack gives. The walks that make
/// an ORDERED fact - the <c>var</c> names of a scope, the declarations of a list, the declarations
/// Annex B aliases - each visit their statements in source order, depth first, exactly as the
/// recursive collectors they replace did, so every list comes out as it always has.
/// </para>
/// <para>
/// <b>None of them recurses.</b> Each keeps its own stack of what is still to visit, so a source as
/// deep as the parser's tree-depth bound costs the walk heap and not native stack (decision D17).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=BE330C
// Broiler-Falsified-If: a walk of the binder recurses once per node, or an ordered fact comes out in an order other than the recursive collector it replaced gave
// Broiler-Human:        PENDING
internal sealed partial class JsBinder
{
    /// <summary>What kind of statement a <c>break</c> or <c>continue</c> may leave.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=1FD79B
    // Broiler-Human:        PENDING
    private enum ExitKind
    {
        /// <summary>A loop, which both may leave.</summary>
        Loop,

        /// <summary>A <c>switch</c>, which only <c>break</c> may leave.</summary>
        Switch,

        /// <summary>Any other labelled statement, which only a <c>break</c> naming it may leave.</summary>
        Label,
    }

    /// <summary>
    /// One statement a jump inside it may leave, and the ones around it out to the function's edge.
    /// </summary>
    /// <remarks>
    /// <b>It is the lowering's exit stack as the tree gives it</b>, which is why a loop or a
    /// <c>switch</c> carries only the label written directly on it: <c>a: b: while (…)</c> is a label
    /// <c>a</c> around a loop labelled <c>b</c>, exactly as the lowering pushes the two.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=BA83A7
    // Broiler-Human:        PENDING
    private sealed class Exit(ExitKind kind, string label, JsStatement statement, Exit? outer)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=57323F
        // Broiler-Human:        PENDING
        internal ExitKind Kind { get; } = kind;

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=FFD2E9
        // Broiler-Human:        PENDING
        internal string Label { get; } = label;

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=12B1CD
        // Broiler-Human:        PENDING
        internal JsStatement Statement { get; } = statement;

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D6299A
        // Broiler-Human:        PENDING
        internal Exit? Outer { get; } = outer;
    }

    /// <summary>What is in force where a node stands: the strictness, and the exits a jump may take.</summary>
    /// <remarks>
    /// <b>A function starts a context of its own</b>: its strictness is its definer's or its own
    /// prologue's, and no exit outside it is one a jump inside it may take.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=94DADB
    // Broiler-Human:        PENDING
    private sealed class Context(bool strict, Exit? exits)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=57AC08
        // Broiler-Human:        PENDING
        internal bool Strict { get; } = strict;

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=075313
        // Broiler-Human:        PENDING
        internal Exit? Exits { get; } = exits;

        /// <summary>This context inside <paramref name="statement"/>, which a jump may leave.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=29B39C
        // Broiler-Human:        PENDING
        internal Context Inside(ExitKind kind, string label, JsStatement statement) =>
            new(Strict, new Exit(kind, label, statement, Exits));
    }

    /// <summary>One node still to visit, with what is in force where it stands.</summary>
    /// <param name="Node">The node.</param>
    /// <param name="Context">The strictness and exits in force there.</param>
    /// <param name="Label">
    /// The label written directly on it, when it is a loop or a <c>switch</c>; empty otherwise.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=BB960D
    // Broiler-Human:        PENDING
    private readonly record struct Pending(JsNode Node, Context Context, string Label);

    /// <summary>What a hoisting scope's walk is still to visit, for the <c>var</c> collection.</summary>
    /// <param name="Statement">The statement.</param>
    /// <param name="TopLevel">
    /// Whether it stands at the scope's own top level, looking through labels, where a function
    /// declaration is the scope's; below it, a declaration is its block's.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D0B1AE
    // Broiler-Human:        PENDING
    private readonly record struct VarPending(JsStatement Statement, bool TopLevel);

    /// <summary>What the Annex B walk is still to visit.</summary>
    /// <param name="Statement">A statement to look into, or <see langword="null"/> for a list.</param>
    /// <param name="List">A block's statement list, whose own declarations are judged first.</param>
    /// <param name="Blocked">
    /// The names a <c>var</c> of the same spelling could not be added under, where it stands.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A72E0F
    // Broiler-Human:        PENDING
    private readonly record struct AnnexBPending(
        JsStatement? Statement,
        System.Collections.Generic.IReadOnlyList<JsStatement>? List,
        System.Collections.Generic.HashSet<string> Blocked);

    /// <summary>The unit's own top level: what it declares, and what Annex B aliases there.</summary>
    /// <remarks>
    /// <b>A module's exported declarations are declarations of its top level</b>, so the walk reads
    /// each <c>export</c> that carries one as the declaration itself; a script holds none.
    /// <b>Only a script or a sloppy evaluated program owes an alias</b>, and the names that block one
    /// there are the top level's own lexical names - not its functions, which are <c>var</c>-scoped.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=447422
    // Broiler-Falsified-If: a script's or an evaluated program's var names, functions or Annex B aliases differ from what its hoisting declared before SJL-12.6 item 1, or a module's exported declarations are left out of its top level
    // Broiler-Human:        PENDING
    private HoistingScope BindProgram(JsProgramNode program, bool strict)
    {
        var body = new System.Collections.Generic.List<JsStatement>(program.Body.Count);

        foreach (var statement in program.Body)
        {
            body.Add(statement is JsExportDeclaration { Declaration: { } declared } ? declared : statement);
        }

        var topLevel = DeclarationsOf(body);
        var blocked = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        foreach (var declaration in topLevel.Lexical)
        {
            if (!declaration.IsFunction)
            {
                blocked.Add(declaration.Name);
            }
        }

        return Hoisted(body, strict, topLevel, blocked);
    }

    /// <summary>One function: its shape as the lowering decides it, and its body's declarations.</summary>
    /// <param name="function">The function.</param>
    /// <param name="definer">The strictness of the code the function is written in.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=FE2C59
    // Broiler-Falsified-If: a function's strictness, arguments object, variable environment or evaluation marks differ from the lowering's own reading of the same node
    // Broiler-Human:        PENDING
    private FunctionFacts BindFunction(JsFunctionNode function, bool definer)
    {
        var strict = definer || function.IsStrict;
        var simple = IsSimpleParameterList(function.Parameters);

        // A PARAMETER THAT BINDS `arguments` IS THE BINDING, and the object is not created at all;
        // parameter names are bound names, so `function f({ arguments }) {}` is that case too.
        var parameterNames = new System.Collections.Generic.List<string>();

        foreach (var parameter in function.Parameters)
        {
            PatternNames(parameter.Target, parameterNames);
        }

        var shadowedByParameter = false;

        foreach (var parameterName in parameterNames)
        {
            if (string.Equals(parameterName, "arguments", System.StringComparison.Ordinal))
            {
                shadowedByParameter = true;
                break;
            }
        }

        var usesArguments = !function.IsArrow && !shadowedByParameter && UsesArguments(function);
        var bodyMayEvaluate = !strict && MentionsEval(function.Body);

        var separateBody = !simple && ParametersHaveExpressions(function.Parameters) &&
            (ParametersMakeClosures(function.Parameters) || bodyMayEvaluate);

        var parametersMayEvaluate = !simple && !strict && ParametersMention(function.Parameters, "eval");

        // ANNEX B DECLINES A NAME THE PARAMETER LIST BINDS, `arguments`, AND EVERY LEXICAL NAME THE
        // BODY'S TOP LEVEL DECLARES - each is a name a `var` of the same spelling could not be added
        // under. `arguments` IS A PARAMETER NAME WHENEVER THE OBJECT EXISTS, and the language says so
        // in those words: function declaration instantiation appends it to `parameterNames` before
        // Annex B's clause reads them. Naming it whether or not the object is created keeps the answer
        // on the safe side of the rule - the extension is declined, and declining it changes nothing
        // about a program that has no such declaration in it. The lowering took the parameter names
        // off the function's record before anything else was declared in it, which held exactly these
        // names and the temporaries no source can spell.
        var topLevel = DeclarationsOf(function.Body);
        var blocked = new System.Collections.Generic.HashSet<string>(parameterNames, System.StringComparer.Ordinal)
        {
            "arguments",
        };

        foreach (var declaration in topLevel.Lexical)
        {
            if (!declaration.IsFunction)
            {
                blocked.Add(declaration.Name);
            }
        }

        return new FunctionFacts(
            Hoisted(function.Body, strict, topLevel, blocked),
            simple,
            usesArguments,
            bodyMayEvaluate,
            separateBody,
            parametersMayEvaluate);
    }

    /// <summary>A hoisting scope's facts, from its statement list and its blocking set.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7F811A
    // Broiler-Human:        PENDING
    private HoistingScope Hoisted(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        bool strict,
        Declarations topLevel,
        System.Collections.Generic.HashSet<string> blocked)
    {
        var names = new System.Collections.Generic.List<string>();
        var declared = new System.Collections.Generic.List<JsFunctionNode>();
        CollectVarScope(body, names, declared);

        var aliases = new System.Collections.Generic.List<string>();

        if (!strict)
        {
            ScanAnnexB(body, blocked, aliases);
        }

        return new HoistingScope(strict, topLevel, [.. names], [.. declared], [.. aliases]);
    }

    /// <summary>
    /// Walks the whole unit and records, against each node, the facts that are about it: each
    /// function's, each block's and <c>switch</c>'s declarations, each class body's private names, and
    /// each jump's target.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=340829
    // Broiler-Falsified-If: a function, block, switch, class body or jump the lowering can reach is not given its facts, or a node is walked under a strictness other than its definer's
    // Broiler-Human:        PENDING
    private void BindTree(JsProgramNode program, bool strict)
    {
        var pending = new System.Collections.Generic.Stack<Pending>();
        var top = new Context(strict, null);

        foreach (var statement in program.Body)
        {
            Push(pending, statement, top);
        }

        while (pending.Count != 0)
        {
            var (node, context, label) = pending.Pop();
            Visit(pending, node, context, label);
        }
    }

    /// <summary>Records one node's facts and queues the nodes inside it.</summary>
    /// <remarks>
    /// <b>It names every kind of node the tree has</b>, the leaves too, and fails on any other: a kind
    /// it does not know may hold a function, a class, a block or a <c>switch</c>, and passing over it
    /// would leave the lowering to ask about one the binder never saw.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=22B9E0
    // Broiler-Falsified-If: a node that holds a function, a class, a block, a switch or a jump is left unvisited, or a node of a kind the walk does not name passes it without failing
    // Broiler-Human:        PENDING
    private void Visit(
        System.Collections.Generic.Stack<Pending> pending, JsNode node, Context context, string label)
    {
        switch (node)
        {
            // A FUNCTION IS COMPILED UNDER ITS DEFINER'S STRICTNESS OR ITS OWN, and nothing a jump
            // inside it names lies outside it. Its parameter list is lowered inside the function, so
            // what the list holds is walked in the function's context too.
            case JsFunctionNode function:
            {
                var facts = BindFunction(function, context.Strict);
                functions[function] = facts;
                var inner = new Context(facts.Body.Strict, null);

                foreach (var parameter in function.Parameters)
                {
                    Push(pending, parameter.Target, inner);
                    Push(pending, parameter.Default, inner);
                }

                Push(pending, function.Body, inner);
                return;
            }

            // A CLASS BODY CHANGES NO STRICTNESS HERE, because the lowering's does not: its members
            // are compiled under their definer's strictness or their own, as any function is.
            case JsClassNode definition:
                privateNames[definition] = PrivateNamesOf(definition);
                Push(pending, definition.Heritage, context);

                foreach (var member in definition.Members)
                {
                    Push(pending, member.Computed, context);
                    Push(pending, member.Function, context);
                }

                return;

            case JsBlockStatement block:
                lists[block] = DeclarationsOf(block.Body);
                Push(pending, block.Body, context);
                return;

            // THE CLAUSES OF A `switch` ARE ONE STATEMENT LIST AND ONE RECORD, and its tests are
            // evaluated before the `switch` is a statement a `break` may leave.
            case JsSwitchStatement switched:
            {
                lists[switched] = DeclarationsOf(Clauses(switched));
                var inner = context.Inside(ExitKind.Switch, label, switched);
                Push(pending, switched.Discriminant, context);

                foreach (var clause in switched.Clauses)
                {
                    Push(pending, clause.Test, context);
                    Push(pending, clause.Body, inner);
                }

                return;
            }

            case JsWhileStatement loop:
                Push(pending, loop.Test, context);
                Push(pending, loop.Body, context.Inside(ExitKind.Loop, label, loop));
                return;

            case JsDoWhileStatement loop:
                Push(pending, loop.Body, context.Inside(ExitKind.Loop, label, loop));
                Push(pending, loop.Test, context);
                return;

            case JsForStatement loop:
                Push(pending, loop.Initialiser, context);
                Push(pending, loop.Test, context);
                Push(pending, loop.Update, context);
                Push(pending, loop.Body, context.Inside(ExitKind.Loop, label, loop));
                return;

            case JsForInStatement loop:
                Push(pending, loop.Target, context);
                Push(pending, loop.Pattern, context);
                Push(pending, loop.Initialiser, context);
                Push(pending, loop.Right, context);
                Push(pending, loop.Body, context.Inside(ExitKind.Loop, label, loop));
                return;

            case JsForOfStatement loop:
                Push(pending, loop.Target, context);
                Push(pending, loop.Pattern, context);
                Push(pending, loop.Right, context);
                Push(pending, loop.Body, context.Inside(ExitKind.Loop, label, loop));
                return;

            // A LABEL ON A LOOP OR A `switch` IS THAT STATEMENT'S OWN, and on anything else it makes a
            // statement a `break` naming it may leave - which is how the lowering reads one.
            case JsLabelledStatement labelled:
                if (labelled.Body is JsWhileStatement or JsDoWhileStatement or JsForStatement or
                    JsForInStatement or JsForOfStatement or JsSwitchStatement)
                {
                    pending.Push(new Pending(labelled.Body, context, labelled.Label));
                }
                else
                {
                    Push(
                        pending,
                        labelled.Body,
                        context.Inside(ExitKind.Label, labelled.Label, labelled));
                }

                return;

            case JsBreakStatement jump:
                jumps[jump] = Target(context.Exits, jump.Label, continues: false);
                return;

            case JsContinueStatement jump:
                jumps[jump] = Target(context.Exits, jump.Label, continues: true);
                return;

            case JsExpressionStatement statement:
                Push(pending, statement.Expression, context);
                return;

            case JsVariableStatement statement:
                foreach (var declarator in statement.Declarators)
                {
                    Push(pending, declarator.Pattern, context);
                    Push(pending, declarator.Initialiser, context);
                }

                return;

            case JsIfStatement statement:
                Push(pending, statement.Test, context);
                Push(pending, statement.Consequent, context);
                Push(pending, statement.Alternate, context);
                return;

            case JsWithStatement statement:
                Push(pending, statement.Object, context);
                Push(pending, statement.Body, context);
                return;

            case JsReturnStatement statement:
                Push(pending, statement.Value, context);
                return;

            case JsThrowStatement statement:
                Push(pending, statement.Value, context);
                return;

            case JsTryStatement statement:
                Push(pending, statement.Block, context);
                Push(pending, statement.CatchPattern, context);
                Push(pending, statement.Handler, context);
                Push(pending, statement.Finaliser, context);
                return;

            case JsFunctionDeclaration statement:
                Push(pending, statement.Function, context);
                return;

            case JsClassDeclaration statement:
                Push(pending, statement.Class, context);
                return;

            case JsExportDeclaration statement:
                Push(pending, statement.Declaration, context);
                Push(pending, statement.Default, context);
                return;

            case JsArrayLiteral expression:
                foreach (var element in expression.Elements)
                {
                    Push(pending, element, context);
                }

                return;

            case JsSpreadElement expression:
                Push(pending, expression.Argument, context);
                return;

            case JsObjectLiteral expression:
                foreach (var entry in expression.Entries)
                {
                    Push(pending, entry.Computed, context);
                    Push(pending, entry.Value, context);
                }

                return;

            case JsFunctionExpression expression:
                Push(pending, expression.Function, context);
                return;

            case JsYieldExpression expression:
                Push(pending, expression.Operand, context);
                return;

            case JsAwaitExpression expression:
                Push(pending, expression.Operand, context);
                return;

            case JsUnaryExpression expression:
                Push(pending, expression.Operand, context);
                return;

            case JsUpdateExpression expression:
                Push(pending, expression.Operand, context);
                return;

            case JsBinaryExpression expression:
                Push(pending, expression.Left, context);
                Push(pending, expression.Right, context);
                return;

            case JsLogicalExpression expression:
                Push(pending, expression.Left, context);
                Push(pending, expression.Right, context);
                return;

            case JsAssignmentExpression expression:
                Push(pending, expression.Target, context);
                Push(pending, expression.Value, context);
                return;

            case JsConditionalExpression expression:
                Push(pending, expression.Test, context);
                Push(pending, expression.WhenTrue, context);
                Push(pending, expression.WhenFalse, context);
                return;

            case JsMemberExpression expression:
                Push(pending, expression.Target, context);
                Push(pending, expression.Computed, context);
                return;

            case JsCallExpression expression:
                Push(pending, expression.Callee, context);

                foreach (var argument in expression.Arguments)
                {
                    Push(pending, argument, context);
                }

                return;

            case JsNewExpression expression:
                Push(pending, expression.Callee, context);

                foreach (var argument in expression.Arguments)
                {
                    Push(pending, argument, context);
                }

                return;

            case JsChainExpression expression:
                Push(pending, expression.Chain, context);
                return;

            case JsTemplateLiteral expression:
                foreach (var substitution in expression.Substitutions)
                {
                    Push(pending, substitution, context);
                }

                return;

            case JsTaggedTemplate expression:
                Push(pending, expression.Tag, context);
                Push(pending, expression.Quasi, context);
                return;

            case JsSequenceExpression expression:
                foreach (var inner in expression.Expressions)
                {
                    Push(pending, inner, context);
                }

                return;

            case JsDestructuringAssignment expression:
                Push(pending, expression.Target, context);
                Push(pending, expression.Value, context);
                return;

            case JsClassExpression expression:
                Push(pending, expression.Class, context);
                return;

            case JsSuperMemberExpression expression:
                Push(pending, expression.Computed, context);
                return;

            case JsPrivateMemberExpression expression:
                Push(pending, expression.Target, context);
                return;

            case JsPrivateInExpression expression:
                Push(pending, expression.Target, context);
                return;

            case JsSuperCallExpression expression:
                foreach (var argument in expression.Arguments)
                {
                    Push(pending, argument, context);
                }

                return;

            case JsImportCall expression:
                Push(pending, expression.Specifier, context);
                Push(pending, expression.Options, context);
                return;

            case JsTargetPattern pattern:
                Push(pending, pattern.Target, context);
                return;

            case JsArrayPattern pattern:
                foreach (var element in pattern.Elements)
                {
                    if (element is not null)
                    {
                        Push(pending, element.Target, context);
                        Push(pending, element.Default, context);
                    }
                }

                Push(pending, pattern.Rest, context);
                return;

            case JsObjectPattern pattern:
                foreach (var property in pattern.Properties)
                {
                    Push(pending, property.Computed, context);
                    Push(pending, property.Value.Target, context);
                    Push(pending, property.Value.Default, context);
                }

                Push(pending, pattern.Rest, context);
                return;

            // A LEAF: a literal, a name, `this`, `new.target`, `import.meta`, an empty or `debugger`
            // statement, an `import` declaration. None holds a node with a fact of its own.
            case JsNumberLiteral or JsBigIntLiteral or JsStringLiteral or JsBooleanLiteral or
                JsNullLiteral or JsRegExpLiteral or JsIdentifier or JsThisExpression or
                JsNewTargetExpression or JsImportMeta or JsEmptyStatement or JsDebuggerStatement or
                JsImportDeclaration:
                return;

            // EVERY KIND OF NODE IS NAMED ABOVE, AS ONE THAT HOLDS OTHERS OR AS A LEAF, so that a kind
            // added to the tree and not to this walk stops the compilation of the first unit that
            // holds one, naming it - rather than leaving whatever it holds without facts, which the
            // lowering would meet later as a function, block or `switch` nobody bound.
            default:
                throw new System.InvalidOperationException(
                    "the binder has no reading of " + node.GetType().Name + ", at " + Where(node));
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=402AA2
    // Broiler-Human:        PENDING
    private static void Push(System.Collections.Generic.Stack<Pending> pending, JsNode? node, Context context)
    {
        if (node is not null)
        {
            pending.Push(new Pending(node, context, string.Empty));
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=19E8D9
    // Broiler-Human:        PENDING
    private static void Push(
        System.Collections.Generic.Stack<Pending> pending,
        System.Collections.Generic.IReadOnlyList<JsStatement> statements,
        Context context)
    {
        foreach (var statement in statements)
        {
            pending.Push(new Pending(statement, context, string.Empty));
        }
    }

    /// <summary>
    /// The statement a jump leaves, searched from the innermost exit outward as the lowering's exit
    /// stack is.
    /// </summary>
    /// <remarks>
    /// A labelled jump takes the nearest exit carrying its label, skipping, for a <c>continue</c>, one
    /// that is not a loop's; an unlabelled <c>continue</c> takes the nearest loop, and an unlabelled
    /// <c>break</c> the nearest loop or <c>switch</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=3B0288
    // Broiler-Human:        PENDING
    private static JsStatement? Target(Exit? exits, string label, bool continues)
    {
        for (var exit = exits; exit is not null; exit = exit.Outer)
        {
            if (label.Length != 0)
            {
                if (string.Equals(exit.Label, label, System.StringComparison.Ordinal) &&
                    (!continues || exit.Kind == ExitKind.Loop))
                {
                    return exit.Statement;
                }

                continue;
            }

            if (exit.Kind == ExitKind.Loop || (!continues && exit.Kind == ExitKind.Switch))
            {
                return exit.Statement;
            }
        }

        return null;
    }

    /// <summary>A <c>switch</c>'s clauses' statements, as the one list they are.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=A9EA51
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<JsStatement> Clauses(JsSwitchStatement switched)
    {
        var clauses = new System.Collections.Generic.List<JsStatement>();

        foreach (var clause in switched.Clauses)
        {
            clauses.AddRange(clause.Body);
        }

        return clauses;
    }

    /// <summary>What one statement list declares where it stands, in source order.</summary>
    /// <remarks>
    /// <b>A CLASS AND A FUNCTION ARE LEXICAL DECLARATIONS OF A LIST TOO.</b> Leaving a class out would
    /// put its binding in whatever record happened to be current - in a loop body, a slot the next
    /// turn overwrites - and a function written in a block belongs to the block's record, looking
    /// through any labels on it, because the language's <c>LexicallyDeclaredNames</c> of a statement
    /// list looks through a <c>LabelledStatement</c> to the declaration under it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=2348DF
    // Broiler-Human:        PENDING
    private static Declarations DeclarationsOf(System.Collections.Generic.IReadOnlyList<JsStatement> list)
    {
        System.Collections.Generic.List<Declaration>? lexical = null;
        System.Collections.Generic.List<JsFunctionDeclaration>? declared = null;

        foreach (var statement in list)
        {
            if (statement is JsVariableStatement variable && variable.Kind != SliceDeclarationKind.Var)
            {
                var names = new System.Collections.Generic.List<string>();

                foreach (var declarator in variable.Declarators)
                {
                    DeclaratorNames(declarator, names);
                }

                foreach (var name in names)
                {
                    (lexical ??= []).Add(
                        new Declaration(name, variable.Kind == SliceDeclarationKind.Const, isFunction: false));
                }
            }

            if (statement is JsClassDeclaration definition && definition.Class.Name.Length != 0)
            {
                (lexical ??= []).Add(new Declaration(definition.Class.Name, constant: false, isFunction: false));
            }

            if (Declared(statement) is { } function && function.Function.Name.Length != 0)
            {
                (lexical ??= []).Add(new Declaration(function.Function.Name, constant: false, isFunction: true));
                (declared ??= []).Add(function);
            }
        }

        return new Declarations(
            lexical is null ? [] : [.. lexical],
            declared is null ? [] : [.. declared]);
    }

    /// <summary>The function declaration one statement is, looking through any labels on it.</summary>
    /// <remarks>
    /// <b>A label does not move the binding it labels</b>: <c>{ l: function f() { } }</c> declares
    /// <c>f</c> in the block exactly as <c>{ function f() { } }</c> does.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=B8CA09
    // Broiler-Human:        PENDING
    private static JsFunctionDeclaration? Declared(JsStatement statement)
    {
        while (statement is JsLabelledStatement labelled)
        {
            statement = labelled.Body;
        }

        return statement as JsFunctionDeclaration;
    }

    /// <summary>
    /// Collects the <c>var</c> names and the top-level function declarations of one hoisting scope.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It descends through blocks, loops, <c>try</c>, <c>with</c> and <c>switch</c> - a <c>var</c>
    /// anywhere inside a function belongs to that function - and stops at a nested function, which is
    /// a hoisting scope of its own.
    /// </para>
    /// <para>
    /// <b>A FUNCTION DECLARATION IS COLLECTED AT THE TOP LEVEL OF THE SCOPE AND NOWHERE BELOW IT</b>,
    /// which is the one respect in which it is unlike a <c>var</c>: written inside a block, it binds in
    /// that block. <b>A label is transparent to that rule and a block is not</b>, because the
    /// language's <c>TopLevelVarScopedDeclarations</c> looks through a labelled statement.
    /// </para>
    /// <para>
    /// <b>The statements are visited in source order, depth first</b>, as the recursive collector this
    /// replaced visited them: each statement's parts are pushed last part first, so the first part is
    /// the next one taken.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C37E1D
    // Broiler-Falsified-If: the var names or function declarations of a scope come out in an order, or with a multiplicity, other than the source's
    // Broiler-Human:        PENDING
    private static void CollectVarScope(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        System.Collections.Generic.List<string> names,
        System.Collections.Generic.List<JsFunctionNode> functions)
    {
        var pending = new System.Collections.Generic.Stack<VarPending>();
        PushVar(pending, body, topLevel: true);

        while (pending.Count != 0)
        {
            var (statement, topLevel) = pending.Pop();

            switch (statement)
            {
                case JsVariableStatement variable when variable.Kind == SliceDeclarationKind.Var:
                    foreach (var declarator in variable.Declarators)
                    {
                        DeclaratorNames(declarator, names);
                    }

                    break;

                case JsFunctionDeclaration declaration:
                    if (topLevel)
                    {
                        functions.Add(declaration.Function);
                    }

                    break;

                case JsBlockStatement block:
                    PushVar(pending, block.Body, topLevel: false);
                    break;

                // A `with` IS NOT A HOISTING SCOPE: `with (o) { var x; }` declares `x` in the
                // enclosing function, and an assignment inside the body still asks the object first.
                case JsWithStatement scoped:
                    pending.Push(new VarPending(scoped.Body, false));
                    break;

                case JsIfStatement conditional:
                    if (conditional.Alternate is not null)
                    {
                        pending.Push(new VarPending(conditional.Alternate, false));
                    }

                    pending.Push(new VarPending(conditional.Consequent, false));
                    break;

                case JsWhileStatement loop:
                    pending.Push(new VarPending(loop.Body, false));
                    break;

                case JsDoWhileStatement loop:
                    pending.Push(new VarPending(loop.Body, false));
                    break;

                case JsForStatement loop:
                    pending.Push(new VarPending(loop.Body, false));

                    if (loop.Initialiser is not null)
                    {
                        pending.Push(new VarPending(loop.Initialiser, false));
                    }

                    break;

                // THE HEAD'S NAMES COME BEFORE THE BODY'S, so they are taken here and the body is
                // left to be visited next.
                case JsForInStatement loop:
                    if (loop.Declaration == SliceDeclarationKind.Var)
                    {
                        HeadNames(loop.Name, loop.Pattern, names);
                    }

                    pending.Push(new VarPending(loop.Body, false));
                    break;

                case JsForOfStatement loop:
                    if (loop.Declaration == SliceDeclarationKind.Var)
                    {
                        HeadNames(loop.Name, loop.Pattern, names);
                    }

                    pending.Push(new VarPending(loop.Body, false));
                    break;

                case JsTryStatement guarded:
                    if (guarded.Finaliser is not null)
                    {
                        PushVar(pending, guarded.Finaliser.Body, topLevel: false);
                    }

                    if (guarded.Handler is not null)
                    {
                        PushVar(pending, guarded.Handler.Body, topLevel: false);
                    }

                    PushVar(pending, guarded.Block.Body, topLevel: false);
                    break;

                case JsSwitchStatement switched:
                    for (var at = switched.Clauses.Count - 1; at >= 0; at--)
                    {
                        PushVar(pending, switched.Clauses[at].Body, topLevel: false);
                    }

                    break;

                case JsLabelledStatement labelled:
                    pending.Push(new VarPending(labelled.Body, topLevel));
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>Queues a statement list so that its first statement is the next one taken.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=F209D2
    // Broiler-Human:        PENDING
    private static void PushVar(
        System.Collections.Generic.Stack<VarPending> pending,
        System.Collections.Generic.IReadOnlyList<JsStatement> statements,
        bool topLevel)
    {
        for (var at = statements.Count - 1; at >= 0; at--)
        {
            pending.Push(new VarPending(statements[at], topLevel));
        }
    }

    /// <summary>
    /// Finds the block-level function declarations Annex B additionally binds in the scope whose top
    /// level is <paramref name="body"/>, admits each, and collects its name.
    /// </summary>
    /// <param name="body">The top level of one hoisting scope.</param>
    /// <param name="blocked">
    /// The names a <c>var</c> of the same spelling could not be added to the scope for.
    /// </param>
    /// <param name="aliases">The names collected, in the order the declarations are written.</param>
    /// <remarks>
    /// <para>
    /// <b>Annex B's condition is a question about an EARLY ERROR and this is the whole of the
    /// question.</b> The clause admits a declaration when replacing it with <c>var F</c> would not
    /// have been a syntax error - so the walk carries the set of names that would have collided with
    /// such a <c>var</c>, adds each enclosing block's own lexical names to it on the way down, and
    /// admits a declaration exactly when its name is not in the set its ENCLOSING blocks give.
    /// </para>
    /// <para>
    /// <b>A simple catch parameter is the one binding that does not block, and a destructuring one
    /// does</b>: B.3.4 exempts <c>CatchParameter : BindingIdentifier</c> and nothing else. <b>Annex B
    /// extends <c>FunctionDeclaration</c> only</b>, so a generator or an async function is never
    /// admitted. <b>It stops where a hoisting scope stops</b>: a nested function, a class body and a
    /// method are scopes of their own.
    /// </para>
    /// <para>
    /// <b>A block's own declarations are judged before anything inside it</b>, and the statements of
    /// each list in source order, depth first: the order the recursive scan this replaced collected
    /// the names in, which is the order the lowering declares the aliases in.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=746203
    // Broiler-Falsified-If: a declaration is admitted whose name a `var` of the same spelling could not be added under, or the aliases come out in another order
    // Broiler-Human:        PENDING
    private void ScanAnnexB(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        System.Collections.Generic.HashSet<string> blocked,
        System.Collections.Generic.List<string> aliases)
    {
        var pending = new System.Collections.Generic.Stack<AnnexBPending>();
        PushAnnexB(pending, body, blocked);

        while (pending.Count != 0)
        {
            var (statement, list, inherited) = pending.Pop();

            if (list is not null)
            {
                foreach (var candidate in list)
                {
                    if (Declared(candidate) is { } declaration &&
                        declaration.Function.Name.Length != 0 &&
                        !declaration.Function.IsGenerator &&
                        !declaration.Function.IsAsync &&
                        !inherited.Contains(declaration.Function.Name))
                    {
                        annexB.Add(declaration);
                        aliases.Add(declaration.Function.Name);
                    }
                }

                var names = new System.Collections.Generic.List<string>();

                foreach (var declared in DeclarationsOf(list).Lexical)
                {
                    names.Add(declared.Name);
                }

                PushAnnexB(pending, list, Blocking(inherited, names));
                continue;
            }

            switch (statement)
            {
                case JsBlockStatement block:
                    pending.Push(new AnnexBPending(null, block.Body, inherited));
                    break;

                // THE CLAUSES OF A `switch` ARE ONE BLOCK, so a declaration in one clause is blocked
                // by a `let` in another.
                case JsSwitchStatement switched:
                    pending.Push(new AnnexBPending(null, Clauses(switched), inherited));
                    break;

                case JsIfStatement conditional:
                    if (conditional.Alternate is not null)
                    {
                        pending.Push(new AnnexBPending(conditional.Alternate, null, inherited));
                    }

                    pending.Push(new AnnexBPending(conditional.Consequent, null, inherited));
                    break;

                case JsWhileStatement loop:
                    pending.Push(new AnnexBPending(loop.Body, null, inherited));
                    break;

                case JsDoWhileStatement loop:
                    pending.Push(new AnnexBPending(loop.Body, null, inherited));
                    break;

                case JsForStatement loop:
                {
                    var inner = inherited;

                    if (loop.Initialiser is JsVariableStatement head && head.Kind != SliceDeclarationKind.Var)
                    {
                        var names = new System.Collections.Generic.List<string>();

                        foreach (var declarator in head.Declarators)
                        {
                            DeclaratorNames(declarator, names);
                        }

                        inner = Blocking(inherited, names);
                    }

                    pending.Push(new AnnexBPending(loop.Body, null, inner));
                    break;
                }

                case JsForInStatement loop:
                    pending.Push(new AnnexBPending(
                        loop.Body, null, HeadBlocking(loop.Declaration, loop.Name, loop.Pattern, inherited)));

                    break;

                case JsForOfStatement loop:
                    pending.Push(new AnnexBPending(
                        loop.Body, null, HeadBlocking(loop.Declaration, loop.Name, loop.Pattern, inherited)));

                    break;

                case JsTryStatement guarded:
                {
                    if (guarded.Finaliser is not null)
                    {
                        pending.Push(new AnnexBPending(null, guarded.Finaliser.Body, inherited));
                    }

                    if (guarded.Handler is not null)
                    {
                        var inner = inherited;

                        if (guarded.CatchPattern is not null)
                        {
                            var names = new System.Collections.Generic.List<string>();
                            PatternNames(guarded.CatchPattern, names);
                            inner = Blocking(inherited, names);
                        }

                        pending.Push(new AnnexBPending(null, guarded.Handler.Body, inner));
                    }

                    pending.Push(new AnnexBPending(null, guarded.Block.Body, inherited));
                    break;
                }

                case JsWithStatement scoped:
                    pending.Push(new AnnexBPending(scoped.Body, null, inherited));
                    break;

                case JsLabelledStatement labelled:
                    pending.Push(new AnnexBPending(labelled.Body, null, inherited));
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>Queues a statement list's statements so that its first is the next one taken.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=BAD504
    // Broiler-Human:        PENDING
    private static void PushAnnexB(
        System.Collections.Generic.Stack<AnnexBPending> pending,
        System.Collections.Generic.IReadOnlyList<JsStatement> statements,
        System.Collections.Generic.HashSet<string> blocked)
    {
        for (var at = statements.Count - 1; at >= 0; at--)
        {
            pending.Push(new AnnexBPending(statements[at], null, blocked));
        }
    }

    /// <summary>The blocking set one more record's names give, leaving the caller's alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=414085
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.HashSet<string> Blocking(
        System.Collections.Generic.HashSet<string> blocked,
        System.Collections.Generic.IReadOnlyList<string> names)
    {
        if (names.Count == 0)
        {
            return blocked;
        }

        var inner = new System.Collections.Generic.HashSet<string>(blocked, System.StringComparer.Ordinal);

        foreach (var name in names)
        {
            inner.Add(name);
        }

        return inner;
    }

    /// <summary>The blocking set a <c>for-in</c> or <c>for-of</c> head's own binding gives.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=26A2AF
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.HashSet<string> HeadBlocking(
        SliceDeclarationKind? declaration,
        string name,
        JsPattern? pattern,
        System.Collections.Generic.HashSet<string> blocked)
    {
        if (declaration is null or SliceDeclarationKind.Var)
        {
            return blocked;
        }

        var names = new System.Collections.Generic.List<string>();
        HeadNames(name, pattern, names);
        return Blocking(blocked, names);
    }

    /// <summary>Every private name one class body declares, in source order and once each.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=44F8F2
    // Broiler-Human:        PENDING
    private static string[] PrivateNamesOf(JsClassNode definition)
    {
        var found = new System.Collections.Generic.List<string>();

        foreach (var member in definition.Members)
        {
            if (member.IsPrivate && !found.Contains(member.Key))
            {
                found.Add(member.Key);
            }
        }

        return [.. found];
    }

    /// <summary>Every name one declarator introduces, whether it names one or destructures.</summary>
    /// <remarks>
    /// A syntactic answer about one node rather than a fact about the unit, so the lowering asks it
    /// too, where it allocates a loop head's or a module's bindings.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A6B0B2
    // Broiler-Human:        PENDING
    internal static void DeclaratorNames(JsDeclarator declarator, System.Collections.Generic.List<string> names) =>
        HeadNames(declarator.Name, declarator.Pattern, names);

    /// <summary>The name a binding head gives, or every name its pattern does.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=86C75E
    // Broiler-Human:        PENDING
    private static void HeadNames(string name, JsPattern? pattern, System.Collections.Generic.List<string> names)
    {
        if (pattern is null)
        {
            names.Add(name);
            return;
        }

        PatternNames(pattern, names);
    }

    /// <summary>Every name a binding pattern binds, in the order the pattern writes them.</summary>
    /// <remarks>
    /// Elements before the rest, properties before the rest, and each nested pattern's names where the
    /// pattern stands: the order of the recursive collector this replaced.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=5F0A16
    // Broiler-Falsified-If: the names of a pattern come out in an order other than the pattern writes them
    // Broiler-Human:        PENDING
    private static void PatternNames(JsPattern pattern, System.Collections.Generic.List<string> names)
    {
        var pending = new System.Collections.Generic.Stack<JsPattern>();
        pending.Push(pattern);

        while (pending.Count != 0)
        {
            switch (pending.Pop())
            {
                case JsTargetPattern { Target: JsIdentifier identifier }:
                    names.Add(identifier.Name);
                    break;

                case JsArrayPattern array:
                    if (array.Rest is not null)
                    {
                        pending.Push(array.Rest);
                    }

                    for (var at = array.Elements.Count - 1; at >= 0; at--)
                    {
                        if (array.Elements[at] is { } element)
                        {
                            pending.Push(element.Target);
                        }
                    }

                    break;

                case JsObjectPattern shape:
                    if (shape.Rest is not null)
                    {
                        pending.Push(shape.Rest);
                    }

                    for (var at = shape.Properties.Count - 1; at >= 0; at--)
                    {
                        pending.Push(shape.Properties[at].Value.Target);
                    }

                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>Whether every parameter is one name with no initialiser and no <c>...</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=8D6266
    // Broiler-Human:        PENDING
    private static bool IsSimpleParameterList(System.Collections.Generic.IReadOnlyList<JsParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (parameter.IsRest || parameter.Default is not null ||
                parameter.Target is not JsTargetPattern { Target: JsIdentifier })
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Whether a function's own code mentions <c>arguments</c> or <c>eval</c>, parameter defaults and
    /// patterns included.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The defaults are part of the scan because <c>function f(a = arguments[0]) {}</c> mentions the
    /// object only where a body-only walk never looks, and the object has to exist before the
    /// prologue that reads it runs.
    /// </para>
    /// <para>
    /// <b>A MENTION OF <c>eval</c> IS A POSSIBLE MENTION OF <c>arguments</c></b>, because a direct
    /// evaluation in the function's own code - or in an arrow nested in it through arrows only, which
    /// has no <c>arguments</c> of its own - reads the name through the scope map, and the map can only
    /// name a slot that exists (JSD-0026 step 4). A parameter list's evaluation does the same, and a
    /// <c>var arguments</c> it declares collides with the object (JSeal V15).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BC47D7
    // Broiler-Human:        PENDING
    private static bool UsesArguments(JsFunctionNode function)
    {
        foreach (var parameter in function.Parameters)
        {
            if (parameter.Default is not null &&
                (JsCompiler.Walk.Mentions(parameter.Default, "arguments") ||
                    JsCompiler.Walk.Mentions(parameter.Default, "eval")))
            {
                return true;
            }

            if (JsCompiler.Walk.Mentions(parameter.Target, "arguments") ||
                JsCompiler.Walk.Mentions(parameter.Target, "eval"))
            {
                return true;
            }
        }

        foreach (var statement in function.Body)
        {
            if (JsCompiler.Walk.Mentions(statement, "arguments") || JsCompiler.Walk.Mentions(statement, "eval"))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether a parameter list - defaults and patterns - mentions <paramref name="name"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=893942
    // Broiler-Human:        PENDING
    private static bool ParametersMention(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters, string name)
    {
        foreach (var parameter in parameters)
        {
            if ((parameter.Default is not null && JsCompiler.Walk.Mentions(parameter.Default, name)) ||
                JsCompiler.Walk.Mentions(parameter.Target, name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Whether a parameter list has expressions - a default, a pattern element's default or a computed
    /// key - which is the specification's <c>hasParameterExpressions</c> (JSeal V15-finish).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3B4CDC
    // Broiler-Human:        PENDING
    private static bool ParametersHaveExpressions(System.Collections.Generic.IReadOnlyList<JsParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (parameter.Default is not null || PatternHasExpression(parameter.Target))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether a binding pattern holds a default or a computed key anywhere in it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=963233
    // Broiler-Human:        PENDING
    private static bool PatternHasExpression(JsPattern pattern)
    {
        var pending = new System.Collections.Generic.Stack<JsPattern>();
        pending.Push(pattern);

        while (pending.Count != 0)
        {
            switch (pending.Pop())
            {
                case JsArrayPattern array:
                    foreach (var element in array.Elements)
                    {
                        if (element is null)
                        {
                            continue;
                        }

                        if (element.Default is not null)
                        {
                            return true;
                        }

                        pending.Push(element.Target);
                    }

                    if (array.Rest is not null)
                    {
                        pending.Push(array.Rest);
                    }

                    break;

                case JsObjectPattern shape:
                    foreach (var property in shape.Properties)
                    {
                        if (property.Computed is not null || property.Value.Default is not null)
                        {
                            return true;
                        }

                        pending.Push(property.Value.Target);
                    }

                    if (shape.Rest is not null)
                    {
                        pending.Push(shape.Rest);
                    }

                    break;

                default:
                    break;
            }
        }

        return false;
    }

    /// <summary>
    /// Whether a parameter list - defaults and patterns - may make a closure: a function, an arrow, a
    /// class or an object literal's method in its own syntax, or a mention of <c>eval</c>, whose source
    /// may make one (JSeal V15-finish).
    /// </summary>
    /// <remarks>
    /// Such a closure sees the parameters' record, and the specification keeps the body's declarations
    /// from it; a list that makes none cannot tell a body's variable environment from its own record.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3B4BA1
    // Broiler-Human:        PENDING
    private static bool ParametersMakeClosures(System.Collections.Generic.IReadOnlyList<JsParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            if ((parameter.Default is not null && JsCompiler.Walk.MakesClosure(parameter.Default)) ||
                JsCompiler.Walk.MakesClosure(parameter.Target))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether a body's own code - its arrows included - mentions the name <c>eval</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7184FD
    // Broiler-Human:        PENDING
    private static bool MentionsEval(System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
        foreach (var statement in body)
        {
            if (JsCompiler.Walk.Mentions(statement, "eval"))
            {
                return true;
            }
        }

        return false;
    }
}
