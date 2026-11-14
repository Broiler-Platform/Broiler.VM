// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   63
// Annotated:        63/63
// Exempt:           26
// Human-reviewed:   0/63
// IP risk:          Low
// Security risk:    High
// Criteria:         11/8
// Resource impact:  3/10 max
// Unverified:       63
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>The binder's walks, every one of them over an explicit stack.</summary>
/// <remarks>
/// <para>
/// <b>Two kinds of walk, and every one of them in source order.</b> The walk over the whole unit
/// meets every node once, with the strictness and the jump targets in force where the node stands,
/// and records each fact against the node it is about. No fact it records depends on the order it
/// meets nodes in, and it meets them in source order, depth first, all the same: SJL-12.6 item 2
/// moves the lowering's early errors into this pass, and they are to be reported in source order
/// (JSD-0080 decision D12). The walks that make an ORDERED fact - the <c>var</c> names of a scope,
/// the declarations of a list, the declarations Annex B aliases - each visit their statements in
/// source order, depth first, exactly as the recursive collectors they replace did, so every list
/// comes out as it always has.
/// </para>
/// <para>
/// <b>None of them recurses.</b> Each keeps its own stack of what is still to visit, so a source as
/// deep as the parser's tree-depth bound costs the walk heap and not native stack (decision D17).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=BE330C
// Broiler-Falsified-If: a walk of the binder recurses once per node, meets a node before one the source writes ahead of it, or an ordered fact comes out in an order other than the recursive collector it replaced gave
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=2E7309
    // Broiler-Human:        PENDING
    private sealed class Exit(ExitKind kind, string label, Statement statement, Exit? outer)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=57323F
        // Broiler-Human:        PENDING
        internal ExitKind Kind { get; } = kind;

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=FFD2E9
        // Broiler-Human:        PENDING
        internal string Label { get; } = label;

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D1B868
        // Broiler-Human:        PENDING
        internal Statement Statement { get; } = statement;

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D6299A
        // Broiler-Human:        PENDING
        internal Exit? Outer { get; } = outer;
    }

    /// <summary>What the code where a node stands is inside of, as the lowering's own flags say it.</summary>
    /// <remarks>
    /// <b>Each is the reading the lowering takes of the same position</b>, because each decides an
    /// early error the lowering used to raise from its own flags while it emitted (SJL-12.6 item 2):
    /// an arrow takes its definer's <see cref="Method"/> and <see cref="Derived"/> and every other
    /// function sets its own. <i>(Revised 2026-11-09: the flags <c>Function</c>, <c>Body</c>,
    /// <c>StaticBlock</c>, <c>Async</c> and <c>Disposing</c> left with the checks that read them, the
    /// correspondence's B03 to B05, B10 and B14, which are the shared library's refusals from SJL-13.3's
    /// step 1 (JSD-0083, decision D20). The remark read "an arrow takes its definer's
    /// <c>Method</c>, <c>Derived</c> and <c>Function</c> and every other function sets its own, while
    /// <c>StaticBlock</c> belongs to a static block's own body and nothing nested in it. <c>Async</c>
    /// and <c>Disposing</c> belong to one unit each, an arrow's
    /// included: the first is the unit's async flag, which the lowering takes from the function alone,
    /// and for a module's top level from its parser's answer to whether the module awaits there; the
    /// second a resource scope on the unit's exit stack, which every unit starts empty.")</i>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=7D4CB5
    // Broiler-Human:        PENDING
    [System.Flags]
    private enum Frame
    {
        /// <summary>None of what follows.</summary>
        None = 0,

        /// <summary>
        /// Code the lowering never lowers - a function a sloppy evaluated program declares again later
        /// - so nothing here is reported: the lowering would not have reported it either.
        /// </summary>
        Silent = 1,

        /// <summary>A method's code, or an arrow's in one, where <c>super.x</c> resolves.</summary>
        Method = 2,

        /// <summary>A derived constructor's code, or an arrow's in one, where <c>super()</c> resolves.</summary>
        Derived = 4,

        /// <summary>
        /// Code where <c>arguments</c> names a binding the unit itself makes, which only an evaluated
        /// program inside a class field initialiser asks about.
        /// </summary>
        ArgumentsBound = 64,
    }

    /// <summary>The private names one class body declares, and the bodies around it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=EE9740
    // Broiler-Human:        PENDING
    private sealed class PrivateScope(string[] names, PrivateScope? outer)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=F02656
        // Broiler-Human:        PENDING
        internal string[] Names { get; } = names;

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=5B981A
        // Broiler-Human:        PENDING
        internal PrivateScope? Outer { get; } = outer;
    }

    /// <summary>
    /// What is in force where a node stands: the strictness, the exits a jump may take, what the code
    /// is inside of, and the private names in scope.
    /// </summary>
    /// <remarks>
    /// <b>A function starts a context of its own</b>: its strictness is its definer's or its own
    /// prologue's, and no exit outside it is one a jump inside it may take.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7494C0
    // Broiler-Human:        PENDING
    private sealed class Context(bool strict, Exit? exits, Frame frame, PrivateScope? privates)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=57AC08
        // Broiler-Human:        PENDING
        internal bool Strict { get; } = strict;

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=075313
        // Broiler-Human:        PENDING
        internal Exit? Exits { get; } = exits;

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B928EB
        // Broiler-Human:        PENDING
        internal Frame Frame { get; } = frame;

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=DA2340
        // Broiler-Human:        PENDING
        internal PrivateScope? Privates { get; } = privates;

        /// <summary>Whether the code here is inside of <paramref name="frame"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=48A071
        // Broiler-Human:        PENDING
        internal bool Is(Frame frame) => (Frame & frame) != 0;

        /// <summary>This context inside <paramref name="statement"/>, which a jump may leave.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=BF7844
        // Broiler-Human:        PENDING
        internal Context Inside(ExitKind kind, string label, Statement statement) =>
            new(Strict, new Exit(kind, label, statement, Exits), Frame, Privates);

        /// <summary>This context, inside of <paramref name="frame"/> too.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=1A0664
        // Broiler-Human:        PENDING
        internal Context And(Frame frame) =>
            (Frame & frame) == frame ? this : new(Strict, Exits, Frame | frame, Privates);

        /// <summary>This context inside a class body that declares the private names <paramref name="names"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=E1697B
        // Broiler-Human:        PENDING
        internal Context WithPrivateNames(string[] names) =>
            names.Length == 0 ? this : new(Strict, Exits, Frame, new PrivateScope(names, Privates));
    }

    /// <summary>What a parent asks of a child node beside its context.</summary>
    /// <remarks>
    /// <b>The same node means different things to the lowering in different places</b>: a function
    /// is a method or not by where it is written, and a pattern's leaf is a binding, a <c>var</c>'s
    /// write or an assignment's by the statement it is in.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=1B98BD
    // Broiler-Human:        PENDING
    private enum Use
    {
        /// <summary>Evaluated where it stands: an expression or a statement, or an ordinary function.</summary>
        Value,

        /// <summary>A method: a class member's, a constructor of a base class, an object literal's.</summary>
        Method,

        /// <summary>The constructor of a class with a heritage.</summary>
        Derived,

        /// <summary>A class static block's body.</summary>
        StaticBlock,

        /// <summary>A pattern that initialises the bindings it names: <c>let</c>, <c>const</c>, a parameter, a catch.</summary>
        Initialise,

        /// <summary>A pattern that writes the <c>var</c> bindings hoisting made.</summary>
        Var,

        /// <summary>A pattern that assigns through whatever references its leaves are.</summary>
        Assign,

        /// <summary>
        /// A function declaration that is an <c>if</c> clause, which the wide parser read as a block of its
        /// own holding it; the block's facts are keyed by the declaration (SJL-13.3, step 2).
        /// </summary>
        Clause,
    }

    /// <summary>One node still to visit, with what is in force where it stands.</summary>
    /// <param name="Node">The node.</param>
    /// <param name="Context">The strictness, exits and frames in force there.</param>
    /// <param name="Label">
    /// The label written directly on it, when it is a loop or a <c>switch</c>; empty otherwise.
    /// </param>
    /// <param name="Use">What its parent asks of it.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=ADB0F3
    // Broiler-Human:        PENDING
    private readonly record struct Pending(SyntaxNode Node, Context Context, string Label, Use Use = Use.Value);

    /// <summary>What a hoisting scope's walk is still to visit, for the <c>var</c> collection.</summary>
    /// <param name="Statement">The statement.</param>
    /// <param name="TopLevel">
    /// Whether it stands at the scope's own top level, looking through labels, where a function
    /// declaration is the scope's; below it, a declaration is its block's.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=896045
    // Broiler-Human:        PENDING
    private readonly record struct VarPending(Statement Statement, bool TopLevel);

    /// <summary>What the Annex B walk is still to visit.</summary>
    /// <param name="Statement">A statement to look into, or <see langword="null"/> for a list.</param>
    /// <param name="List">A block's statement list, whose own declarations are judged first.</param>
    /// <param name="Blocked">
    /// The names a <c>var</c> of the same spelling could not be added under, where it stands.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B30C4B
    // Broiler-Human:        PENDING
    private readonly record struct AnnexBPending(
        Statement? Statement,
        System.Collections.Generic.IReadOnlyList<Statement>? List,
        System.Collections.Generic.HashSet<string> Blocked);

    /// <summary>The unit's own top level: what it declares, and what Annex B aliases there.</summary>
    /// <remarks>
    /// <b>A module's exported declarations are declarations of its top level</b>, so the walk reads
    /// each <c>export</c> that carries one as the declaration itself, through the lowering's one
    /// reading of that (<see cref="JsCompiler.Unwrapped"/>); a script holds none.
    /// <b>Only a script or a sloppy evaluated program owes an alias</b>, and the names that block one
    /// there are the top level's own lexical names - not its functions, which are <c>var</c>-scoped.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=E70217
    // Broiler-Falsified-If: a script's or an evaluated program's var names, functions or Annex B aliases differ from what its hoisting declared before SJL-12.6 item 1, or a module's exported declarations are left out of its top level
    // Broiler-Human:        PENDING
    private HoistingScope BindProgram(ProgramNode program, bool strict)
    {
        var body = JsTreeFacts.Unwrapped(program.Body);
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=AE4A04
    // Broiler-Falsified-If: a function's strictness, arguments object, variable environment or evaluation marks differ from the lowering's own reading of the same node
    // Broiler-Human:        PENDING
    private FunctionFacts BindFunction(SyntaxNode function, bool definer)
    {
        var strict = definer || OwnStrictness(function);
        var parameterList = ParametersOf(function);
        var statements = BodyOf(function, out var value);
        var simple = IsSimpleParameterList(parameterList);

        // A PARAMETER THAT BINDS `arguments` IS THE BINDING, and the object is not created at all;
        // parameter names are bound names, so `function f({ arguments }) {}` is that case too.
        var parameterNames = new System.Collections.Generic.List<string>();

        foreach (var parameter in parameterList)
        {
            PatternNames(Parameter(parameter, out _), parameterNames);
        }

        var shadowedByParameter = false;

        foreach (var parameterName in parameterNames)
        {
            if (IsArguments(parameterName))
            {
                shadowedByParameter = true;
                break;
            }
        }

        var usesArguments = !IsArrow(function) && !shadowedByParameter && UsesArguments(parameterList, statements, value);
        var bodyMayEvaluate = !strict && MentionsEval(statements, value);

        var separateBody = !simple && ParametersHaveExpressions(parameterList) &&
            (ParametersMakeClosures(parameterList) || bodyMayEvaluate);

        var parametersMayEvaluate = !simple && !strict && ParametersMention(parameterList, "eval");

        // ANNEX B DECLINES A NAME THE PARAMETER LIST BINDS, `arguments`, AND EVERY LEXICAL NAME THE
        // BODY'S TOP LEVEL DECLARES - each is a name a `var` of the same spelling could not be added
        // under. `arguments` IS A PARAMETER NAME WHENEVER THE OBJECT EXISTS, and the language says so
        // in those words: function declaration instantiation appends it to `parameterNames` before
        // Annex B's clause reads them. Naming it whether or not the object is created keeps the answer
        // on the safe side of the rule - the extension is declined, and declining it changes nothing
        // about a program that has no such declaration in it. The lowering took the parameter names
        // off the function's record before anything else was declared in it, which held exactly these
        // names and the temporaries no source can spell.
        var topLevel = DeclarationsOf(statements);
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
            Hoisted(statements, strict, topLevel, blocked),
            simple,
            usesArguments,
            bodyMayEvaluate,
            separateBody,
            parametersMayEvaluate);
    }

    /// <summary>A hoisting scope's facts, from its statement list and its blocking set.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=B60454
    // Broiler-Human:        PENDING
    private HoistingScope Hoisted(
        System.Collections.Generic.IReadOnlyList<Statement> body,
        bool strict,
        Declarations topLevel,
        System.Collections.Generic.HashSet<string> blocked)
    {
        var names = new System.Collections.Generic.List<string>();
        var declared = new System.Collections.Generic.List<FunctionNode>();
        CollectVarScope(body, names, declared);

        var aliases = new System.Collections.Generic.List<string>();

        if (!strict)
        {
            ScanAnnexB(body, blocked, aliases);
        }

        return new HoistingScope(strict, topLevel, [.. names], [.. declared], [.. aliases]);
    }

    /// <summary>
    /// Walks the whole unit and records, against each node, the facts that are about it - each
    /// function's, each block's and <c>switch</c>'s declarations, each class body's private names, each
    /// <c>catch</c> parameter's and lexical loop head's names, and each jump's target - and finds, where
    /// it meets them, the early errors the lowering used to raise while it emitted.
    /// </summary>
    /// <remarks>
    /// <b>In source order, depth first.</b> <see cref="Visit"/> lists the nodes inside the one it
    /// visits in the order the source writes them, and the list goes onto the stack last first, so
    /// the first of them is the next node taken.
    /// </remarks>
    /// <param name="program">The unit's tree.</param>
    /// <param name="strict">The unit's strictness.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=C2DD36
    // Broiler-Falsified-If: a function, block, switch, class body or jump the lowering can reach is not given its facts, a node is walked under a strictness other than its definer's, or a node is met before one the source writes ahead of it
    // Broiler-Human:        PENDING
    private void BindTree(ProgramNode program, bool strict)
    {
        var frame = Frame.None;

        // EVAL CODE'S OWN TOP LEVEL IS WHERE ITS CALLER STANDS, as far as the caller's request says:
        // the lowering takes `super` and `super()` there from the request's flags. *(Revised
        // 2026-11-09: a module's top level is no longer walked as an async unit or a resource scope,
        // and eval code's no longer as a function's, because the checks that read those frames, the
        // correspondence's B05, B10 and B14, are the shared library's refusals from SJL-13.3's step 1
        // (JSD-0083, decision D20). The comments read "A MODULE'S TOP LEVEL IS A UNIT OF ITS OWN: an
        // async one when the module awaits there, which is the parser's reading the lowering takes too,
        // and inside a resource scope when its own list declares a resource (below). A script's top
        // level and an evaluated program's are neither." and "the lowering takes `super`, `super()` and
        // `new.target` there from the request's flags.")*
        if (unit.IsEval)
        {
            var flags = unit.EvalFlags;

            if ((flags & Format.JsFormat.EvalRequestFlags.InMethod) != 0)
            {
                frame |= Frame.Method;
            }

            if ((flags & Format.JsFormat.EvalRequestFlags.InDerivedConstructor) != 0)
            {
                frame |= Frame.Derived;
            }

            if (EvaluatedInField && EvalRootDeclaresArguments(strict))
            {
                frame |= Frame.ArgumentsBound;
            }

            if (!strict)
            {
                FindOverriddenFunctions();
            }
        }

        var pending = new System.Collections.Generic.Stack<Pending>();
        var inside = new System.Collections.Generic.List<Pending>();
        var root = new Context(strict, null, frame, null);
        Push(inside, program.Body, root);
        Queue(pending, inside);

        while (pending.Count != 0)
        {
            Visit(inside, pending.Pop());
            Queue(pending, inside);
        }
    }

    /// <summary>
    /// Moves the nodes one visit listed onto the stack, last first, so that the first is the next one
    /// taken, and empties the list for the next visit.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=81F396
    // Broiler-Human:        PENDING
    private static void Queue(
        System.Collections.Generic.Stack<Pending> pending, System.Collections.Generic.List<Pending> inside)
    {
        for (var at = inside.Count - 1; at >= 0; at--)
        {
            pending.Push(inside[at]);
        }

        inside.Clear();
    }

    /// <summary>
    /// Records one node's facts, finds the early errors that are about it, and lists the nodes inside
    /// it, in source order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It names every kind of node the tree has</b>, the leaves too, and fails on any other: a kind
    /// it does not know may hold a function, a class, a block or a <c>switch</c>, and passing over it
    /// would leave the lowering to ask about one the binder never saw.
    /// </para>
    /// <para>
    /// <b>Each early error is the lowering's, with its code, its message and its place</b> (SJL-12.6
    /// item 2, JSD-0080 decisions D10 and D12), found from the same reading of the position the
    /// lowering's own flags gave it. A construct the lowering refused and lowered no further is still
    /// walked into, so what it held is reported too; code the lowering never lowers at all is walked
    /// <see cref="Frame.Silent"/>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=BE7EF3
    // Broiler-Falsified-If: a node that holds a function, a class, a block, a switch or a jump is left unvisited, a node of a kind the walk does not name passes it without failing, or an early error is found where the lowering lowered the node without refusing it
    // Broiler-Human:        PENDING
    private void Visit(System.Collections.Generic.List<Pending> pending, Pending visiting)
    {
        var (node, context, label, use) = visiting;

        switch (node)
        {
            // A FUNCTION IS COMPILED UNDER ITS DEFINER'S STRICTNESS OR ITS OWN, and nothing a jump
            // inside it names lies outside it. Its parameter list is lowered inside the function, so
            // what the list holds is walked in the function's context too.
            case FunctionNode or FieldDefinition or StaticBlock:
                VisitFunction(pending, node, context, use);
                return;

            // A PARENTHESIS IS THE EXPRESSION IT HOLDS, as the wide parser's tree held it.
            case ParenthesizedExpression parenthesised:
                Push(pending, JsTreeFacts.Unwrap(parenthesised), context);
                return;

            // A CLASS BODY CHANGES NO STRICTNESS HERE, because the lowering's does not: its members
            // are compiled under their definer's strictness or their own, as any function is. ITS
            // HERITAGE IS EVALUATED IN THE PRIVATE ENVIRONMENT AROUND IT, and its keys and members in
            // its own, which the lowering declares after compiling the heritage.
            case ClassNode definition:
            {
                var names = PrivateNamesOf(definition);
                privateNames[definition] = names;
                Push(pending, definition.Heritage, context);

                var inner = context.WithPrivateNames(names);
                var constructor = JsTreeFacts.FindConstructor(definition, parse.BigInts)?.Function;

                foreach (var member in definition.Elements)
                {
                    RefuseArgumentsIn(member, context);
                    Push(pending, ComputedKey(member), inner);

                    if (MemberFunction(member) is { } body)
                    {
                        pending.Add(new Pending(
                            body, inner, string.Empty, MemberUse(member, body, constructor, definition.Heritage is not null)));
                    }
                }

                return;
            }

            case BlockStatement block:
            {
                var declarations = DeclarationsOf(block.Body);
                lists[block] = declarations;
                Push(pending, block.Body, InsideRecord(context, declarations.Lexical));

                return;
            }

            // AN `if` CLAUSE THAT IS A FUNCTION DECLARATION IS A BLOCK HOLDING IT, as the wide parser
            // wrapped it, and the block's facts are the declaration's (SJL-13.3, step 2).
            case FunctionDeclaration clause when use == Use.Clause:
            {
                System.Collections.Generic.IReadOnlyList<Statement> list = [clause];
                var declarations = DeclarationsOf(list);
                lists[clause] = declarations;
                pending.Add(new Pending(clause, InsideRecord(context, declarations.Lexical), string.Empty));

                return;
            }

            // THE CLAUSES OF A `switch` ARE ONE STATEMENT LIST AND ONE RECORD, and its tests are
            // evaluated before the `switch` is a statement a `break` may leave. The record's names
            // are declared after the discriminant is lowered and before the first test is.
            case SwitchStatement switched:
            {
                var declarations = DeclarationsOf(Clauses(switched));
                lists[switched] = declarations;
                var declared = InsideRecord(context, declarations.Lexical);
                var inner = declared.Inside(ExitKind.Switch, label, switched);
                Push(pending, switched.Discriminant, context);

                foreach (var clause in switched.Cases)
                {
                    Push(pending, clause.Test, declared);
                    Push(pending, clause.Consequent, inner);
                }

                return;
            }

            case WhileStatement loop:
                Push(pending, loop.Test, context);
                Push(pending, loop.Body, context.Inside(ExitKind.Loop, label, loop));
                return;

            case DoWhileStatement loop:
                Push(pending, loop.Body, context.Inside(ExitKind.Loop, label, loop));
                Push(pending, loop.Test, context);
                return;

            // A LEXICAL HEAD'S NAMES ARE DECLARED BEFORE ITS INITIALISER RUNS, and its test, its
            // update and its body see them. The head's fact is recorded and then read back, so
            // the early errors decided under it - which `arguments` the unit binds - are decided
            // from the fact itself (SJL-12.6 item 2), as they are for a catch parameter's. *(Revised
            // 2026-11-09: the walk no longer marks the loop as a resource scope, because the check that
            // read it, the correspondence's B14, is the shared library's refusal from SJL-13.3's step 1
            // (JSD-0083, decision D20); the comment went on "A RESOURCE THE HEAD DECLARES BELONGS TO THE
            // WHOLE LOOP, whose scope the lowering opens before the head's declaration and closes after
            // the loop.")*
            case ForStatement loop:
            {
                heads[loop] = ForHeadOf(loop);
                var inner = InsideRecord(context, LoopHead(loop));
                Push(pending, loop.Init, inner);
                Push(pending, loop.Test, inner);
                Push(pending, loop.Update, inner);
                Push(pending, loop.Body, inner.Inside(ExitKind.Loop, label, loop));
                return;
            }

            case ForInStatement loop:
            {
                var each = JsTreeFacts.Head(loop.Left);
                heads[loop] = EachHeadOf(each.Declaration, each.Name, each.Pattern);
                var inner = InsideRecord(context, LoopHead(loop));
                Head(pending, each.Declaration, each.Name, each.Pattern, each.Target, resource: false, context, inner);
                Push(pending, each.Initialiser, context);

                // ANNEX B'S INITIALISER IS WRITTEN THROUGH THE NAME before the object is evaluated.
                if (each.Initialiser is not null)
                {
                    Referenced(context, each.Name);
                }

                Push(pending, loop.Right, inner);
                Push(pending, loop.Body, inner.Inside(ExitKind.Loop, label, loop));
                return;
            }

            // *(Revised 2026-11-09: the walk no longer marks a turn as a resource scope, because the
            // check that read it, the correspondence's B14, is the shared library's refusal from
            // SJL-13.3's step 1 (JSD-0083, decision D20); the comment read "EACH TURN'S RESOURCE IS ITS
            // OWN, in a scope the lowering opens around the turn's body.")*
            case ForOfStatement loop:
            {
                var each = JsTreeFacts.Head(loop.Left);
                heads[loop] = EachHeadOf(each.Declaration, each.Name, each.Pattern);
                var inner = InsideRecord(context, LoopHead(loop));
                Head(pending, each.Declaration, each.Name, each.Pattern, each.Target, each.Disposal != JsDisposal.None, context, inner);
                Push(pending, loop.Right, inner);
                Push(pending, loop.Body, inner.Inside(ExitKind.Loop, label, loop));
                return;
            }

            // A LABEL ON A LOOP OR A `switch` IS THAT STATEMENT'S OWN, and on anything else it makes a
            // statement a `break` naming it may leave - which is how the lowering reads one.
            case LabelledStatement labelled:
                if (labelled.Body is WhileStatement or DoWhileStatement or ForStatement or
                    ForInStatement or ForOfStatement or SwitchStatement)
                {
                    pending.Add(new Pending(labelled.Body, context, labelled.Label.Name));
                }
                else
                {
                    Push(
                        pending,
                        labelled.Body,
                        context.Inside(ExitKind.Label, labelled.Label.Name, labelled));
                }

                return;

            // A JUMP WITH NOWHERE TO GO IS THE ONE PLACE THE JUMP TARGET IS READ, through the fact
            // the walk has just recorded: the lowering keeps finding its targets on its own exit
            // stack, because a `finally` body it emits at a jump holds statements the jump is
            // leaving.
            case BreakStatement jump:
            {
                jumps[jump] = Target(context.Exits, jump.Label?.Name ?? string.Empty, continues: false);

                if (JumpTarget(jump) is null)
                {
                    Report(
                        context,
                        JsTreeFacts.At(jump),
                        SliceSourceDiagnosticCode.IllegalBreak,
                        "`break` names no enclosing loop or switch");
                }

                return;
            }

            case ContinueStatement jump:
            {
                jumps[jump] = Target(context.Exits, jump.Label?.Name ?? string.Empty, continues: true);

                if (JumpTarget(jump) is null)
                {
                    Report(
                        context,
                        JsTreeFacts.At(jump),
                        SliceSourceDiagnosticCode.IllegalContinue,
                        "`continue` names no enclosing loop");
                }

                return;
            }

            case ExpressionStatement statement:
                Push(pending, statement.Expression, context);
                return;

            case VariableDeclaration statement:
                VisitDeclarators(pending, statement, context);
                return;

            case IfStatement statement:
                Push(pending, statement.Test, context);
                Clause(pending, statement.Consequent, context);
                Clause(pending, statement.Alternate, context);
                return;

            case WithStatement statement:
                Push(pending, statement.Object, context);
                Push(pending, statement.Body, context);
                return;

            // A `return` outside a function's own code, or in a class static block, is the shared
            // library's refusal from SJL-13.3's step 1, before the binder runs (JSD-0083, decision D20,
            // the correspondence's B03 and B04). *(Revised 2026-11-09: the binder refused both here; the
            // comment read "A `return` HAS SOMETHING TO RETURN FROM ONLY IN A FUNCTION'S OWN CODE, and a
            // class static block compiles to a function and is not one: the word is an early error there
            // too, under the code the catalogue records for it (2218 outside a function and 2219 in a
            // static block from registry revision 19, JSD-0083 decision D12; JSD-0080 decisions D12 and
            // D13).")*
            case ReturnStatement statement:
                Push(pending, statement.Argument, context);
                return;

            case ThrowStatement statement:
                Push(pending, statement.Argument, context);
                return;

            // A CATCH PARAMETER'S NAMES ARE THE BLOCK'S AND ITS OWN DEFAULTS', and nothing outside.
            case TryStatement statement:
            {
                catches[statement] = CatchNamesOf(statement);
                var caught = InsideRecord(context, CatchParameters(statement));
                Push(pending, statement.Block, context);

                if (CatchPattern(statement) is { } pattern)
                {
                    pending.Add(new Pending(pattern, caught, string.Empty, Use.Initialise));
                }

                Push(pending, statement.Handler?.Body, caught);
                Push(pending, statement.Finalizer, context);
                return;
            }

            case FunctionDeclaration statement:
                Push(pending, statement.Function, context);
                return;

            case ClassDeclaration statement:
                Push(pending, statement.Class, context);
                return;

            case ExportNamedDeclaration statement:
                Push(pending, statement.Declaration, context);
                return;

            case ExportDefaultDeclaration statement:
                Push(pending, statement.Declaration, context);
                return;

            case ArrayLiteral expression:
                foreach (var element in expression.Elements)
                {
                    Push(pending, element, context);
                }

                return;

            case SpreadElement expression:
                Push(pending, expression.Argument, context);
                return;

            case ObjectLiteral expression:
                VisitObject(pending, expression, context);
                return;

            // A NAMED FUNCTION EXPRESSION'S OWN NAME IS A BINDING OF A RECORD AROUND THE FUNCTION.
            case FunctionExpression expression:
                Push(
                    pending,
                    expression.Function,
                    IsArguments(expression.Function.Name?.Name ?? string.Empty)
                        ? InsideRecord(context, bindsArguments: true)
                        : context);

                return;

            case ArrowFunctionExpression expression:
                Push(pending, expression.Function, context);
                return;

            case YieldExpression expression:
                Push(pending, expression.Argument, context);
                return;

            // `await` outside an async unit, `delete` of a private reference and an update of what is
            // not a reference are the shared library's refusals from SJL-13.3's step 1, before the
            // binder runs (JSD-0083, decision D20, the correspondence's B05 to B07). *(Revised
            // 2026-11-09: the binder refused all three here; the comment on the first read "`await`
            // WAITS ONLY IN A UNIT THE LOWERING EMITS AS AN ASYNC ONE, and anywhere else it is refused
            // under the code the catalogue records for it (2104; JSD-0080 decisions D12 and D13). A
            // template substitution's own parse let one into the parameters of an arrow that is not
            // async, whose unit is the arrow's, until SJL-12.6 item 4 (decision D15) had the parser read
            // a substitution in its parameter list and refuse the `await` itself (2101). No other way to
            // this refusal is known, so no source the parser admits reaches it now; it stays as the
            // rule's place.")*
            case AwaitExpression expression:
                Push(pending, expression.Argument, context);
                return;

            case UnaryExpression expression:
                Push(pending, expression.Operand, context);
                return;

            case UpdateExpression expression:
                Push(pending, expression.Operand, context);
                return;

            case BinaryExpression expression:
                Push(pending, expression.Left, context);
                Push(pending, expression.Right, context);
                return;

            case LogicalExpression expression:
                Push(pending, expression.Left, context);
                Push(pending, expression.Right, context);
                return;

            // AN ASSIGNMENT TO A PATTERN IS A DESTRUCTURING ONE, its leaves assigned through.
            case AssignmentExpression { Target: ArrayPattern or ObjectPattern } expression:
                pending.Add(new Pending(expression.Target, context, string.Empty, Use.Assign));
                Push(pending, expression.Value, context);
                return;

            case AssignmentExpression expression:
                VisitAssignment(pending, expression, context);
                return;

            case ConditionalExpression expression:
                Push(pending, expression.Test, context);
                Push(pending, expression.Consequent, context);
                Push(pending, expression.Alternate, context);
                return;

            case MemberExpression { Object: SuperExpression } expression:
                if (!context.Is(Frame.Method))
                {
                    Report(
                        context,
                        JsTreeFacts.At(expression),
                        SliceSourceDiagnosticCode.MisplacedSuper,
                        "`super` is only admitted inside a method");
                }

                Push(pending, expression.IsComputed ? expression.Property : null, context);
                return;

            case MemberExpression { Property: PrivateIdentifier name } expression:
                ResolvePrivate(context, JsTreeFacts.At(expression), name.Name);
                Push(pending, expression.Object, context);
                return;

            case MemberExpression expression:
                Push(pending, expression.Object, context);
                Push(pending, expression.IsComputed ? expression.Property : null, context);
                return;

            case PrivateInExpression expression:
                ResolvePrivate(context, JsTreeFacts.At(expression), expression.Name.Name);
                Push(pending, expression.Right, context);
                return;

            case CallExpression { Callee: SuperExpression } expression:
                if (!context.Is(Frame.Derived))
                {
                    Report(
                        context,
                        JsTreeFacts.At(expression),
                        SliceSourceDiagnosticCode.MisplacedSuper,
                        "a `super` call is only admitted in the constructor of a class with a heritage");
                }

                foreach (var argument in expression.Arguments)
                {
                    Push(pending, argument, context);
                }

                return;

            case CallExpression expression:
                Push(pending, expression.Callee, context);

                foreach (var argument in expression.Arguments)
                {
                    Push(pending, argument, context);
                }

                return;

            case NewExpression expression:
                Push(pending, expression.Callee, context);

                foreach (var argument in expression.Arguments)
                {
                    Push(pending, argument, context);
                }

                return;

            case ChainExpression expression:
                Push(pending, expression.Expression, context);
                return;

            case TemplateLiteral expression:
                foreach (var substitution in expression.Expressions)
                {
                    Push(pending, substitution, context);
                }

                return;

            case TaggedTemplateExpression expression:
                Push(pending, expression.Tag, context);
                Push(pending, expression.Quasi, context);
                return;

            case SequenceExpression expression:
                foreach (var inner in expression.Expressions)
                {
                    Push(pending, inner, context);
                }

                return;

            case ClassExpression expression:
                Push(pending, expression.Class, context);
                return;

            case ImportCall expression:
                Push(pending, expression.Source, context);
                Push(pending, expression.Options, context);
                return;

            case TargetPattern pattern:
                VisitLeaf(pending, JsTreeFacts.Unwrap(pattern.Target), context, use);
                return;

            // A PATTERN'S NAME, PUSHED AS THE ADAPTED TREE PUSHED ITS LEAF'S NAME EXPRESSION, IS A REFERENCE.
            case BindingIdentifier pattern when use == Use.Value:
                Referenced(context, pattern.Name);
                return;

            case BindingIdentifier pattern:
                VisitLeaf(pending, pattern, context, use);
                return;

            // AN ELEMENT'S TARGET, THEN ITS DEFAULT, and the rest after every element, as the adapted
            // tree's pattern held them.
            case ArrayPattern pattern:
                foreach (var element in pattern.Elements)
                {
                    switch (element)
                    {
                        case null:
                            break;
                        case RestElement rest:
                            pending.Add(new Pending(rest.Argument, context, string.Empty, use));
                            break;
                        case AssignmentPattern defaulted:
                            pending.Add(new Pending(defaulted.Target, context, string.Empty, use));
                            Push(pending, defaulted.Initializer, context);
                            break;
                        default:
                            pending.Add(new Pending(element, context, string.Empty, use));
                            break;
                    }
                }

                return;

            case ObjectPattern pattern:
                foreach (var property in pattern.Properties)
                {
                    switch (property)
                    {
                        case RestElement rest:
                            pending.Add(new Pending(rest.Argument, context, string.Empty, use));
                            break;
                        case PatternProperty entry:
                            Push(pending, entry.Key is ComputedPropertyName key ? key.Expression : null, context);

                            if (entry.Value is AssignmentPattern defaulted)
                            {
                                pending.Add(new Pending(defaulted.Target, context, string.Empty, use));
                                Push(pending, defaulted.Initializer, context);
                            }
                            else
                            {
                                pending.Add(new Pending(entry.Value, context, string.Empty, use));
                            }

                            break;
                    }
                }

                return;

            // A NAME IN AN EXPRESSION IS A REFERENCE THE LOWERING RESOLVES, and in eval code inside a
            // field initialiser one that reaches the caller as `arguments` is an early error.
            case Identifier expression:
                Referenced(context, expression.Name);
                return;

            // A LEAF: a literal, `this`, an empty or `debugger` statement, an `import` declaration,
            // `new.target`, `import.meta` and a regular-expression literal. None holds a node with a
            // fact or an early error of its own: `new.target` outside a function, `import.meta` outside
            // a module and a regular expression's validity are the shared library's refusals from
            // SJL-13.3's step 1, the last through this family's validator (JSD-0083, decisions D7 and
            // D20, the correspondence's B10, B11 and B25). *(Revised 2026-11-09: the binder checked the
            // three here; the comment read "A LEAF: a literal, `this`, an empty or `debugger` statement,
            // an `import` declaration. None holds a node with a fact or an early error of its own.")*
            case NumericLiteral or StringLiteral or BooleanLiteral or
                NullLiteral or ThisExpression or EmptyStatement or DebuggerStatement or
                ImportDeclaration or ExportAllDeclaration or MetaProperty or RegularExpressionLiteral:
                return;

            // EVERY KIND OF NODE IS NAMED ABOVE, AS ONE THAT HOLDS OTHERS OR AS A LEAF, so that a kind
            // added to the tree and not to this walk stops the compilation of the first unit that
            // holds one, naming it - rather than leaving whatever it holds without facts, which the
            // lowering would meet later as a function, block or `switch` nobody bound.
            default:
                throw new System.InvalidOperationException(
                    "the binder has no reading of " + node.Kind + ", at " + Where(node));
        }
    }

    /// <summary>One function: its facts, and its parameters and body under what it is inside of.</summary>
    /// <remarks>
    /// <b>An arrow takes its definer's frames and every other function sets its own</b>, which is the
    /// whole of what <c>super</c> and <c>super()</c> mean inside an arrow - the lowering's own rule.
    /// <i>(Revised 2026-11-09: a function is no longer walked with a function, body, static-block, async
    /// or resource-scope frame, because the checks that read them, the correspondence's B03 to B05, B10
    /// and B14, are the shared library's refusals from SJL-13.3's step 1 (JSD-0083, decision D20). The
    /// remark read "which is the whole of what <c>super</c>, <c>super()</c> and <c>new.target</c> mean
    /// inside an arrow - the lowering's own rule. A static block's frame is its own body's alone.
    /// <b>Every function is a unit of its own</b>, an arrow included: an async one only when it is async
    /// itself, and with no resource scope until its body's own list declares a resource.")</i>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=BC8776
    // Broiler-Falsified-If: a function's code is walked as a method's or a derived constructor's where the lowering compiles it otherwise, or a function the lowering never compiles is walked as one it does
    // Broiler-Human:        PENDING
    private void VisitFunction(
        System.Collections.Generic.List<Pending> pending, SyntaxNode function, Context context, Use use)
    {
        var facts = BindFunction(function, context.Strict);
        functions[function] = facts;

        var frame = context.Frame & (Frame.Silent | Frame.ArgumentsBound);

        if (IsArrow(function))
        {
            frame |= context.Frame & (Frame.Method | Frame.Derived);
        }
        else
        {
            if (use is Use.Method or Use.Derived or Use.StaticBlock)
            {
                frame |= Frame.Method;
            }

            if (use == Use.Derived)
            {
                frame |= Frame.Derived;
            }
        }

        // A DECLARATION A SLOPPY EVALUATED PROGRAM DECLARES AGAIN LATER IS NEVER COMPILED: the
        // lowering instantiates the last declaration of each name and no other.
        if (function is FunctionNode declared && overridden.Contains(declared))
        {
            frame |= Frame.Silent;
        }

        var parameters = new Context(facts.Body.Strict, null, frame, context.Privates);
        var body = parameters;

        // WHERE `arguments` RESOLVES INSIDE THE FUNCTION, only an evaluated program in a field
        // initialiser asks: the parameters see the parameters and the arguments object, and the body
        // sees those and everything the body declares.
        if (EvaluatedInField)
        {
            parameters = InsideRecord(parameters, facts.UsesArguments || ParametersBindArguments(function));
            body = InsideRecord(parameters, BodyDeclaresArguments(facts));
        }

        foreach (var parameter in ParametersOf(function))
        {
            pending.Add(new Pending(Parameter(parameter, out var initialiser), parameters, string.Empty, Use.Initialise));
            Push(pending, initialiser, parameters);
        }

        // A CONCISE BODY AND A FIELD'S INITIALISER ARE THE ONE VALUE THE WIDE PARSER RETURNED.
        Push(pending, BodyOf(function, out var value), body);
        Push(pending, value, body);
    }

    /// <summary>One declaration statement's declarators, each with the early errors of its own.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=718CB8
    // Broiler-Human:        PENDING
    private void VisitDeclarators(
        System.Collections.Generic.List<Pending> pending, VariableDeclaration statement, Context context)
    {
        var mode = statement.DeclarationKind == DeclarationKind.Var ? Use.Var : Use.Initialise;

        foreach (var declarator in statement.Declarations)
        {
            // A destructuring declaration without an initialiser, a `const` without one, and a `using`
            // declaration outside a list that disposes it are the shared library's refusals from
            // SJL-13.3's step 1, before the binder runs (JSD-0083, decision D20, the correspondence's B12
            // to B14). *(Revised 2026-11-09: the binder refused the three here first; the comments read
            // "A DESTRUCTURING DECLARATION ALWAYS HAS AN INITIALISER, whatever its keyword, and a `const`
            // that names one does too." and "A RESOURCE IS REGISTERED WITH THE SCOPE AROUND IT, and where
            // the lowering opened none in the unit it refused the declaration under the code the catalogue
            // records for it (2104; JSD-0080 decisions D12 and D13). The parser refuses every such place
            // first today.")*
            if (declarator.Target is BindingIdentifier name && mode == Use.Var && declarator.Initializer is not null)
            {
                Referenced(context, name.Name);
            }

            if (declarator.Target is not BindingIdentifier)
            {
                pending.Add(new Pending(declarator.Target, context, string.Empty, mode));
            }

            Push(pending, declarator.Initializer, context);
        }
    }

    /// <summary>One object literal's entries, each as the lowering reads it.</summary>
    /// <remarks>
    /// A <c>__proto__: v</c> written a second time sets the prototype twice; an entry in method form is
    /// a method, whose <c>super</c> resolves. <i>(Revised 2026-11-09: a <c>{ a = 1 }</c> entry outside a
    /// pattern is the shared library's refusal from SJL-13.3's step 1, before the binder runs (JSD-0083,
    /// decision D20, the correspondence's B15), and the binder no longer checks it; the remark read
    /// "A <c>{ a = 1 }</c> entry that reached the lowering was never a pattern; a <c>__proto__: v</c>
    /// written a second time sets the prototype twice; an entry in method form is a method, whose
    /// <c>super</c> resolves.")</i>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=55E477
    // Broiler-Human:        PENDING
    private void VisitObject(
        System.Collections.Generic.List<Pending> pending, ObjectLiteral literal, Context context)
    {
        var prototyped = false;

        foreach (var property in literal.Properties)
        {
            if (property is SpreadElement spread)
            {
                Push(pending, spread.Argument, context);
                continue;
            }

            var entry = (PropertyDefinition)property;
            Push(pending, entry.Key is ComputedPropertyName key ? key.Expression : null, context);

            if (JsTreeFacts.IsMethodEntry(entry) && entry.Value is FunctionExpression method)
            {
                pending.Add(new Pending(method.Function, context, string.Empty, Use.Method));
                continue;
            }

            if (!JsTreeFacts.IsMethodEntry(entry) && entry.Key is not ComputedPropertyName &&
                entry.PropertyKind != PropertyKind.Shorthand &&
                string.Equals(JsTreeFacts.Key(entry.Key, parse.BigInts, out _), "__proto__", System.StringComparison.Ordinal))
            {
                if (prototyped)
                {
                    Report(
                        context,
                        JsTreeFacts.At(entry),
                        SliceSourceDiagnosticCode.DuplicateProtoProperty,
                        "an object literal may set `__proto__` once");
                }

                prototyped = true;
            }

            Push(pending, entry.Value, context);
        }
    }

    /// <summary>One assignment: its target, then its value, each walked for the facts it holds.</summary>
    /// <remarks>
    /// <i>(Revised 2026-11-09: a target the lowering cannot write through, a logical assignment's
    /// included, is the shared library's refusal from SJL-13.3's step 1, before the binder runs
    /// (JSD-0083, decision D20, the correspondence's B17 and B18), and the binder no longer checks it.
    /// The summary read "One assignment, with the target the lowering can write through or the
    /// refusal." and the remark "<b>A logical assignment writes through a name or a property and
    /// nothing else</b>: any other target is refused under the code the catalogue records for it (2104;
    /// JSD-0080 decisions D12 and D13). The lowering lowered neither half after refusing it; both are
    /// walked all the same. The parser refuses every such target first today, under 2205.")</i>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=B1200E
    // Broiler-Human:        PENDING
    private void VisitAssignment(
        System.Collections.Generic.List<Pending> pending, AssignmentExpression assignment, Context context)
    {
        var target = assignment.Target is TargetPattern leaf ? leaf.Target : assignment.Target;

        Push(pending, target, context);
        Push(pending, assignment.Value, context);
    }

    /// <summary>
    /// One leaf of a pattern: a binding a declaration initialises, a <c>var</c> it writes, or a
    /// reference an assignment writes through.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=405300
    // Broiler-Falsified-If: a leaf is read as a binding where the lowering writes through it, or as a reference where it initialises one
    // Broiler-Human:        PENDING
    private void VisitLeaf(
        System.Collections.Generic.List<Pending> pending, SyntaxNode leaf, Context context, Use use)
    {
        switch (use)
        {
            case Use.Initialise or Use.Var:
                // A `var`'s leaf is written through its name; a binding initialised is no reference. A
                // leaf that is not a name is the shared library's refusal from SJL-13.3's step 1 (JSD-0083,
                // decision D20, the correspondence's B19). *(Revised 2026-11-09: the binder refused it
                // here first.)*
                if (use == Use.Var)
                {
                    Push(pending, leaf, context);
                }

                return;

            case Use.Assign:
                // A leaf that is not a reference is the shared library's refusal from SJL-13.3's step 1
                // (JSD-0083, decision D20, the correspondence's B20). *(Revised 2026-11-09: the binder
                // refused it here first.)*
                Push(pending, leaf, context);
                return;

            default:
                throw new System.InvalidOperationException(
                    "the binder met a pattern no declaration or assignment holds, at " + Where(leaf));
        }
    }

    /// <summary>
    /// A <c>for-in</c> or <c>for-of</c> head: what it binds, writes or assigns through each turn.
    /// </summary>
    /// <remarks>
    /// A lexical head initialises its names in the turn's record; a <c>var</c> head writes through its
    /// name; any other head is an assignment target, lowered as an assignment's is. A resource head
    /// registers its value instead and writes nothing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=AFDC1B
    // Broiler-Human:        PENDING
    private void Head(
        System.Collections.Generic.List<Pending> pending,
        SliceDeclarationKind? declaration,
        string name,
        Pattern? pattern,
        Expression? target,
        bool resource,
        Context context,
        Context inner)
    {
        var lexical = declaration is SliceDeclarationKind.Let or SliceDeclarationKind.Const;

        if (target is not null)
        {
            if (!resource && pattern is null && !lexical &&
                declaration != SliceDeclarationKind.Var && !IsReference(target, context.Strict))
            {
                Report(
                    context,
                    JsTreeFacts.At(target),
                    SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                    "the left-hand side of an assignment is not a reference");
            }

            Push(pending, target, context);
        }

        if (pattern is not null)
        {
            pending.Add(new Pending(
                pattern,
                lexical ? inner : context,
                string.Empty,
                lexical ? Use.Initialise : declaration == SliceDeclarationKind.Var ? Use.Var : Use.Assign));
        }
        else if (!resource && declaration == SliceDeclarationKind.Var)
        {
            Referenced(context, name);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=5E06B2
    // Broiler-Human:        PENDING
    private static void Push(System.Collections.Generic.List<Pending> pending, SyntaxNode? node, Context context)
    {
        if (node is not null)
        {
            pending.Add(new Pending(node, context, string.Empty));
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0BEB8A
    // Broiler-Human:        PENDING
    private static void Push(
        System.Collections.Generic.List<Pending> pending,
        System.Collections.Generic.IReadOnlyList<Statement> statements,
        Context context)
    {
        foreach (var statement in statements)
        {
            pending.Add(new Pending(statement, context, string.Empty));
        }
    }

    /// <summary>
    /// An <c>if</c> clause: a function declaration there is the block the wide parser made of it, visited
    /// as that block first (SJL-13.3, step 2).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=26627A
    // Broiler-Human:        PENDING
    private static void Clause(System.Collections.Generic.List<Pending> pending, Statement? clause, Context context)
    {
        if (clause is not null)
        {
            pending.Add(new Pending(clause, context, string.Empty, JsTreeFacts.IsIfClauseFunction(clause) ? Use.Clause : Use.Value));
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=3263AB
    // Broiler-Human:        PENDING
    private static Statement? Target(Exit? exits, string label, bool continues)
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=2A00EB
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<Statement> Clauses(SwitchStatement switched)
    {
        var clauses = new System.Collections.Generic.List<Statement>();

        foreach (var clause in switched.Cases)
        {
            clauses.AddRange(clause.Consequent);
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E0EE15
    // Broiler-Human:        PENDING
    private Declarations DeclarationsOf(System.Collections.Generic.IReadOnlyList<Statement> list)
    {
        System.Collections.Generic.List<Declaration>? lexical = null;
        System.Collections.Generic.List<FunctionDeclaration>? declared = null;

        foreach (var statement in list)
        {
            if (statement is VariableDeclaration variable && variable.DeclarationKind != DeclarationKind.Var)
            {
                var names = new System.Collections.Generic.List<string>();

                foreach (var declarator in variable.Declarations)
                {
                    DeclaratorNames(declarator, names);
                }

                var constant = JsTreeFacts.Kind(variable.DeclarationKind).Kind == SliceDeclarationKind.Const;

                foreach (var name in names)
                {
                    (lexical ??= []).Add(
                        new Declaration(name, constant, isFunction: false));
                }
            }

            if (statement is ClassDeclaration definition && NameOf(definition.Class) is { Length: not 0 } className)
            {
                (lexical ??= []).Add(new Declaration(className, constant: false, isFunction: false));
            }

            if (Declared(statement) is { } function && NameOf(function.Function) is { Length: not 0 } functionName)
            {
                (lexical ??= []).Add(new Declaration(functionName, constant: false, isFunction: true));
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A99B51
    // Broiler-Human:        PENDING
    private static FunctionDeclaration? Declared(Statement statement)
    {
        while (statement is LabelledStatement labelled)
        {
            statement = labelled.Body;
        }

        return statement as FunctionDeclaration;
    }

    /// <summary>
    /// Collects the <c>var</c> names and the top-level function declarations of one hoisting scope.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It descends through blocks, loops, <c>try</c>, <c>with</c> and <c>switch</c> - a <c>var</c>
    /// anywhere inside a function belongs to that function - and stops at a nested function, which is
    /// a hoisting scope of its own. A collector that stopped at a block would answer that
    /// <c>if (x) { var y; }</c> declares nothing.
    /// </para>
    /// <para>
    /// <b>A FUNCTION DECLARATION IS COLLECTED AT THE TOP LEVEL OF THE SCOPE AND NOWHERE BELOW IT</b>,
    /// which is the one respect in which it is unlike a <c>var</c>: written inside a block, it binds in
    /// that block. Collecting it here anyway is what made
    /// <c>{ let n = 0; function fn() { n += 1; } fn(); }</c> a <c>ReferenceError</c>, because the
    /// closure was then built against a scope chain the block's record was not yet on
    /// <i>(JSC-138)</i>; below the top level the block owns the declaration.
    /// </para>
    /// <para>
    /// <b>A label is transparent to that rule and a block is not</b>, because the language's
    /// <c>TopLevelVarScopedDeclarations</c> looks through a labelled statement:
    /// <c>l: function f() { }</c> at the top level of a body declares <c>f</c> in the body, and
    /// <c>{ l: function f() { } }</c> declares it in the block.
    /// </para>
    /// <para>
    /// <b>The statements are visited in source order, depth first</b>, as the recursive collector this
    /// replaced visited them: each statement's parts are pushed last part first, so the first part is
    /// the next one taken.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=91E6EA
    // Broiler-Falsified-If: the var names or function declarations of a scope come out in an order, or with a multiplicity, other than the source's, or a function declared below the scope's top level, labels aside, is collected as the scope's
    // Broiler-Human:        PENDING
    private static void CollectVarScope(
        System.Collections.Generic.IReadOnlyList<Statement> body,
        System.Collections.Generic.List<string> names,
        System.Collections.Generic.List<FunctionNode> functions)
    {
        var pending = new System.Collections.Generic.Stack<VarPending>();
        PushVar(pending, body, topLevel: true);

        while (pending.Count != 0)
        {
            var (statement, topLevel) = pending.Pop();

            switch (statement)
            {
                case VariableDeclaration variable when variable.DeclarationKind == DeclarationKind.Var:
                    foreach (var declarator in variable.Declarations)
                    {
                        DeclaratorNames(declarator, names);
                    }

                    break;

                case FunctionDeclaration declaration:
                    if (topLevel)
                    {
                        functions.Add(declaration.Function);
                    }

                    break;

                case BlockStatement block:
                    PushVar(pending, block.Body, topLevel: false);
                    break;

                // A `with` IS NOT A HOISTING SCOPE: `with (o) { var x; }` declares `x` in the
                // enclosing function, and an assignment inside the body still asks the object first.
                case WithStatement scoped:
                    pending.Push(new VarPending(scoped.Body, false));
                    break;

                case IfStatement conditional:
                    if (conditional.Alternate is not null)
                    {
                        pending.Push(new VarPending(conditional.Alternate, false));
                    }

                    pending.Push(new VarPending(conditional.Consequent, false));
                    break;

                case WhileStatement loop:
                    pending.Push(new VarPending(loop.Body, false));
                    break;

                case DoWhileStatement loop:
                    pending.Push(new VarPending(loop.Body, false));
                    break;

                case ForStatement loop:
                    pending.Push(new VarPending(loop.Body, false));

                    if (loop.Init is Statement initialiser)
                    {
                        pending.Push(new VarPending(initialiser, false));
                    }

                    break;

                // THE HEAD'S NAMES COME BEFORE THE BODY'S, so they are taken here and the body is
                // left to be visited next.
                case ForInStatement loop:
                {
                    var each = JsTreeFacts.Head(loop.Left);

                    if (each.Declaration == SliceDeclarationKind.Var)
                    {
                        HeadNames(each.Name, each.Pattern, names);
                    }

                    pending.Push(new VarPending(loop.Body, false));
                    break;
                }

                case ForOfStatement loop:
                {
                    var each = JsTreeFacts.Head(loop.Left);

                    if (each.Declaration == SliceDeclarationKind.Var)
                    {
                        HeadNames(each.Name, each.Pattern, names);
                    }

                    pending.Push(new VarPending(loop.Body, false));
                    break;
                }

                case TryStatement guarded:
                    if (guarded.Finalizer is not null)
                    {
                        PushVar(pending, guarded.Finalizer.Body, topLevel: false);
                    }

                    if (guarded.Handler is not null)
                    {
                        PushVar(pending, guarded.Handler.Body.Body, topLevel: false);
                    }

                    PushVar(pending, guarded.Block.Body, topLevel: false);
                    break;

                case SwitchStatement switched:
                    for (var at = switched.Cases.Count - 1; at >= 0; at--)
                    {
                        PushVar(pending, switched.Cases[at].Consequent, topLevel: false);
                    }

                    break;

                case LabelledStatement labelled:
                    pending.Push(new VarPending(labelled.Body, topLevel));
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>Queues a statement list so that its first statement is the next one taken.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=D13C05
    // Broiler-Human:        PENDING
    private static void PushVar(
        System.Collections.Generic.Stack<VarPending> pending,
        System.Collections.Generic.IReadOnlyList<Statement> statements,
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
    /// admits a declaration exactly when its name is not in the set its ENCLOSING blocks give. A
    /// block's own lexical names are not in that set, because the declaration is one of them: judged
    /// against them, <c>{ function f() { } }</c> would block itself.
    /// </para>
    /// <para>
    /// <b>Every statement that opens a lexical record adds that record's names for what is inside
    /// it</b>: a block, a <c>switch</c>'s clauses, which are one block, so a declaration in one clause
    /// is blocked by a <c>let</c> in another; a <c>let</c> or <c>const</c> head of a <c>for</c>,
    /// <c>for-in</c> or <c>for-of</c>; and a catch parameter that destructures.
    /// </para>
    /// <para>
    /// <b>A simple catch parameter is the one binding that does not block, and a destructuring one
    /// does.</b> That asymmetry is not this walk's invention: B.3.4 exempts
    /// <c>CatchParameter : BindingIdentifier</c> from the rule that forbids a <c>var</c> of the
    /// parameter's name in the catch block, and exempts nothing else. So
    /// <c>catch (f) { { function f() { } } }</c> writes the alias and
    /// <c>catch ({ f }) { { function f() { } } }</c> does not.
    /// </para>
    /// <para>
    /// <b>Annex B extends <c>FunctionDeclaration</c> and not the three productions that look like
    /// it.</b> A generator, an async function and an async generator are separate productions of the
    /// grammar, and the clause names only the first - so
    /// <c>switch (0) { default: async function x() { } } x;</c> is the <c>ReferenceError</c> the
    /// language says it is, in sloppy code as much as in strict. Admitting them was a
    /// web-compatibility extension nothing asked for, and it turned three conforming refusals into
    /// programs that ran.
    /// </para>
    /// <para>
    /// <b>It stops where a hoisting scope stops.</b> A nested function, a class body and a method are
    /// scopes of their own, and their declarations are their own scan's to find; nothing here
    /// descends into one.
    /// </para>
    /// <para>
    /// <b>A block's own declarations are judged before anything inside it</b>, and the statements of
    /// each list in source order, depth first: the order the recursive scan this replaced collected
    /// the names in, which is the order the lowering declares the aliases in.
    /// </para>
    /// <para>
    /// <b>It collects each list's declarations itself</b>, though the walk over the unit records the
    /// same declarations for every block and <c>switch</c>: a scope is scanned when that walk meets the
    /// function or the unit, before it has reached any list inside, so its table holds none of them
    /// yet. A list is read twice and no answer differs.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=6DF92E
    // Broiler-Falsified-If: a declaration is admitted whose name a `var` of the same spelling could not be added under; a list's own lexical names reach the set its own declarations are judged by; a block, a switch, a lexical loop head or a destructuring catch parameter leaves its names out of the set what is inside it is judged by; or the aliases come out in another order
    // Broiler-Human:        PENDING
    private void ScanAnnexB(
        System.Collections.Generic.IReadOnlyList<Statement> body,
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
                        NameOf(declaration.Function) is { Length: not 0 } name &&
                        !declaration.Function.IsGenerator &&
                        !declaration.Function.IsAsync &&
                        !inherited.Contains(name))
                    {
                        annexB.Add(declaration);
                        aliases.Add(name);
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
                case BlockStatement block:
                    pending.Push(new AnnexBPending(null, block.Body, inherited));
                    break;

                // THE CLAUSES OF A `switch` ARE ONE BLOCK, so a declaration in one clause is blocked
                // by a `let` in another.
                case SwitchStatement switched:
                    pending.Push(new AnnexBPending(null, Clauses(switched), inherited));
                    break;

                // AN `if` CLAUSE THAT IS A FUNCTION DECLARATION IS THE BLOCK THE WIDE PARSER MADE OF IT.
                case IfStatement conditional:
                    if (conditional.Alternate is not null)
                    {
                        pending.Push(AnnexBClause(conditional.Alternate, inherited));
                    }

                    pending.Push(AnnexBClause(conditional.Consequent, inherited));
                    break;

                case WhileStatement loop:
                    pending.Push(new AnnexBPending(loop.Body, null, inherited));
                    break;

                case DoWhileStatement loop:
                    pending.Push(new AnnexBPending(loop.Body, null, inherited));
                    break;

                case ForStatement loop:
                {
                    var inner = inherited;

                    if (loop.Init is VariableDeclaration head && head.DeclarationKind != DeclarationKind.Var)
                    {
                        var names = new System.Collections.Generic.List<string>();

                        foreach (var declarator in head.Declarations)
                        {
                            DeclaratorNames(declarator, names);
                        }

                        inner = Blocking(inherited, names);
                    }

                    pending.Push(new AnnexBPending(loop.Body, null, inner));
                    break;
                }

                case ForInStatement loop:
                {
                    var each = JsTreeFacts.Head(loop.Left);
                    pending.Push(new AnnexBPending(
                        loop.Body, null, HeadBlocking(each.Declaration, each.Name, each.Pattern, inherited)));

                    break;
                }

                case ForOfStatement loop:
                {
                    var each = JsTreeFacts.Head(loop.Left);
                    pending.Push(new AnnexBPending(
                        loop.Body, null, HeadBlocking(each.Declaration, each.Name, each.Pattern, inherited)));

                    break;
                }

                case TryStatement guarded:
                {
                    if (guarded.Finalizer is not null)
                    {
                        pending.Push(new AnnexBPending(null, guarded.Finalizer.Body, inherited));
                    }

                    if (guarded.Handler is not null)
                    {
                        var inner = inherited;

                        if (CatchPattern(guarded) is { } caught)
                        {
                            var names = new System.Collections.Generic.List<string>();
                            PatternNames(caught, names);
                            inner = Blocking(inherited, names);
                        }

                        pending.Push(new AnnexBPending(null, guarded.Handler.Body.Body, inner));
                    }

                    pending.Push(new AnnexBPending(null, guarded.Block.Body, inherited));
                    break;
                }

                case WithStatement scoped:
                    pending.Push(new AnnexBPending(scoped.Body, null, inherited));
                    break;

                case LabelledStatement labelled:
                    pending.Push(new AnnexBPending(labelled.Body, null, inherited));
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>Queues a statement list's statements so that its first is the next one taken.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=6D7ACE
    // Broiler-Human:        PENDING
    private static void PushAnnexB(
        System.Collections.Generic.Stack<AnnexBPending> pending,
        System.Collections.Generic.IReadOnlyList<Statement> statements,
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=344B34
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.HashSet<string> HeadBlocking(
        SliceDeclarationKind? declaration,
        string name,
        Pattern? pattern,
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=C52C35
    // Broiler-Human:        PENDING
    private static string[] PrivateNamesOf(ClassNode definition)
    {
        var found = new System.Collections.Generic.List<string>();

        foreach (var member in definition.Elements)
        {
            var key = member switch
            {
                MethodDefinition { Key: PrivateIdentifier name } => name.Name,
                FieldDefinition { Key: PrivateIdentifier name } => name.Name,
                _ => null,
            };

            if (key is not null && !found.Contains(key))
            {
                found.Add(key);
            }
        }

        return [.. found];
    }

    /// <summary>The names a <c>for</c> head declares with <c>let</c> or <c>const</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=10AB21
    // Broiler-Human:        PENDING
    private static string[] ForHeadOf(ForStatement loop)
    {
        if (loop.Init is not VariableDeclaration head || head.DeclarationKind == DeclarationKind.Var)
        {
            return [];
        }

        var names = new System.Collections.Generic.List<string>();

        foreach (var declarator in head.Declarations)
        {
            DeclaratorNames(declarator, names);
        }

        return [.. names];
    }

    /// <summary>The names a <c>for-in</c> or <c>for-of</c> head declares with <c>let</c> or <c>const</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=DA45DC
    // Broiler-Human:        PENDING
    private static string[] EachHeadOf(SliceDeclarationKind? declaration, string name, Pattern? pattern)
    {
        if (declaration is not (SliceDeclarationKind.Let or SliceDeclarationKind.Const))
        {
            return [];
        }

        var names = new System.Collections.Generic.List<string>();
        HeadNames(name, pattern, names);
        return [.. names];
    }

    /// <summary>The names a <c>catch</c> parameter binds.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=952A1E
    // Broiler-Human:        PENDING
    private static string[] CatchNamesOf(TryStatement guarded)
    {
        if (CatchPattern(guarded) is { } caught)
        {
            var names = new System.Collections.Generic.List<string>();
            PatternNames(caught, names);
            return [.. names];
        }

        return guarded.Handler?.Parameter is BindingIdentifier parameter
            ? [parameter.Name]
            : [];
    }

    /// <summary>A catch clause's pattern, when its parameter is not a name alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=4DC761
    // Broiler-Human:        PENDING
    private static Pattern? CatchPattern(TryStatement guarded) =>
        guarded.Handler?.Parameter is { } parameter and not BindingIdentifier ? parameter : null;

    /// <summary>A module's top-level lexical declarations, an exported one in the export's place.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=6EF80F
    // Broiler-Human:        PENDING
    private ModuleDeclaration[] ModuleDeclarationsOf(System.Collections.Generic.IReadOnlyList<Statement> body)
    {
        var declared = new System.Collections.Generic.List<ModuleDeclaration>();

        foreach (var statement in JsTreeFacts.Unwrapped(body))
        {
            if (statement is ClassDeclaration definition && NameOf(definition.Class) is { Length: not 0 } className)
            {
                declared.Add(new ModuleDeclaration(className, false, JsTreeFacts.At(definition), true));
                continue;
            }

            if (statement is not VariableDeclaration variable || variable.DeclarationKind == DeclarationKind.Var)
            {
                continue;
            }

            var constant = JsTreeFacts.Kind(variable.DeclarationKind).Kind == SliceDeclarationKind.Const;

            foreach (var declarator in variable.Declarations)
            {
                var names = new System.Collections.Generic.List<string>();
                DeclaratorNames(declarator, names);

                foreach (var name in names)
                {
                    declared.Add(new ModuleDeclaration(
                        name, constant, JsTreeFacts.At(declarator), false));
                }
            }
        }

        return [.. declared];
    }

    /// <summary>Every name one declarator introduces, whether it names one or destructures.</summary>
    /// <remarks>
    /// A syntactic answer about one node rather than a fact about the unit, so the lowering asks it
    /// too, where it allocates a loop head's or a module's bindings.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=FE2E51
    // Broiler-Human:        PENDING
    internal static void DeclaratorNames(VariableDeclarator declarator, System.Collections.Generic.List<string> names) =>
        HeadNames(
            declarator.Target is BindingIdentifier name ? name.Name : string.Empty,
            declarator.Target is BindingIdentifier ? null : declarator.Target,
            names);

    /// <summary>The name a binding head gives, or every name its pattern does.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7BCCDA
    // Broiler-Human:        PENDING
    private static void HeadNames(string name, Pattern? pattern, System.Collections.Generic.List<string> names)
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=976FAD
    // Broiler-Falsified-If: the names of a pattern come out in an order other than the pattern writes them
    // Broiler-Human:        PENDING
    private static void PatternNames(Pattern pattern, System.Collections.Generic.List<string> names)
    {
        var pending = new System.Collections.Generic.Stack<Pattern>();
        pending.Push(pattern);

        while (pending.Count != 0)
        {
            switch (pending.Pop())
            {
                case BindingIdentifier identifier:
                    names.Add(identifier.Name);
                    break;

                case TargetPattern leaf when JsTreeFacts.Unwrap(leaf.Target) is Identifier identifier:
                    names.Add(identifier.Name);
                    break;

                // AN ELEMENT'S DEFAULT IS NOT A NAME; ITS TARGET'S ARE, and a rest's after every element.
                case AssignmentPattern defaulted:
                    pending.Push(defaulted.Target);
                    break;

                case RestElement rest:
                    pending.Push(rest.Argument);
                    break;

                case ArrayPattern array:
                    for (var at = array.Elements.Count - 1; at >= 0; at--)
                    {
                        if (array.Elements[at] is { } element)
                        {
                            pending.Push(element);
                        }
                    }

                    break;

                case ObjectPattern shape:
                    for (var at = shape.Properties.Count - 1; at >= 0; at--)
                    {
                        switch (shape.Properties[at])
                        {
                            case PatternProperty property:
                                pending.Push(property.Value);
                                break;
                            case RestElement rest:
                                pending.Push(rest.Argument);
                                break;
                        }
                    }

                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>Whether every parameter is one name with no initialiser and no <c>...</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=62D506
    // Broiler-Human:        PENDING
    private static bool IsSimpleParameterList(System.Collections.Generic.IReadOnlyList<Pattern> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (parameter is not BindingIdentifier)
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5B891C
    // Broiler-Human:        PENDING
    private static bool UsesArguments(
        System.Collections.Generic.IReadOnlyList<Pattern> parameters,
        System.Collections.Generic.IReadOnlyList<Statement> body,
        Expression? value)
    {
        foreach (var parameter in parameters)
        {
            var target = Parameter(parameter, out var initialiser);

            if (initialiser is not null &&
                (JsCompiler.Walk.Mentions(initialiser, "arguments") ||
                    JsCompiler.Walk.Mentions(initialiser, "eval")))
            {
                return true;
            }

            if (JsCompiler.Walk.Mentions(target, "arguments") ||
                JsCompiler.Walk.Mentions(target, "eval"))
            {
                return true;
            }
        }

        foreach (var statement in body)
        {
            if (JsCompiler.Walk.Mentions(statement, "arguments") || JsCompiler.Walk.Mentions(statement, "eval"))
            {
                return true;
            }
        }

        return value is not null &&
            (JsCompiler.Walk.Mentions(value, "arguments") || JsCompiler.Walk.Mentions(value, "eval"));
    }

    /// <summary>Whether a parameter list - defaults and patterns - mentions <paramref name="name"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=173901
    // Broiler-Human:        PENDING
    private static bool ParametersMention(
        System.Collections.Generic.IReadOnlyList<Pattern> parameters, string name)
    {
        foreach (var parameter in parameters)
        {
            var target = Parameter(parameter, out var initialiser);

            if ((initialiser is not null && JsCompiler.Walk.Mentions(initialiser, name)) ||
                JsCompiler.Walk.Mentions(target, name))
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=4ACE1A
    // Broiler-Human:        PENDING
    private static bool ParametersHaveExpressions(System.Collections.Generic.IReadOnlyList<Pattern> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (parameter is AssignmentPattern || PatternHasExpression(Parameter(parameter, out _)))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether a binding pattern holds a default or a computed key anywhere in it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=647892
    // Broiler-Human:        PENDING
    private static bool PatternHasExpression(Pattern pattern)
    {
        var pending = new System.Collections.Generic.Stack<Pattern>();
        pending.Push(pattern);

        while (pending.Count != 0)
        {
            switch (pending.Pop())
            {
                case ArrayPattern array:
                    foreach (var element in array.Elements)
                    {
                        switch (element)
                        {
                            case null:
                                continue;
                            case AssignmentPattern:
                                return true;
                            case RestElement rest:
                                pending.Push(rest.Argument);
                                break;
                            default:
                                pending.Push(element);
                                break;
                        }
                    }

                    break;

                case ObjectPattern shape:
                    foreach (var member in shape.Properties)
                    {
                        switch (member)
                        {
                            case PatternProperty property:
                                if (property.Key is ComputedPropertyName || property.Value is AssignmentPattern)
                                {
                                    return true;
                                }

                                pending.Push(property.Value);
                                break;
                            case RestElement rest:
                                pending.Push(rest.Argument);
                                break;
                        }
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=DB35B0
    // Broiler-Human:        PENDING
    private static bool ParametersMakeClosures(System.Collections.Generic.IReadOnlyList<Pattern> parameters)
    {
        foreach (var parameter in parameters)
        {
            var target = Parameter(parameter, out var initialiser);

            if ((initialiser is not null && JsCompiler.Walk.MakesClosure(initialiser)) ||
                JsCompiler.Walk.MakesClosure(target))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether a body's own code - its arrows included - mentions the name <c>eval</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=CC50C4
    // Broiler-Human:        PENDING
    private static bool MentionsEval(System.Collections.Generic.IReadOnlyList<Statement> body, Expression? value)
    {
        foreach (var statement in body)
        {
            if (JsCompiler.Walk.Mentions(statement, "eval"))
            {
                return true;
            }
        }

        return value is not null && JsCompiler.Walk.Mentions(value, "eval");
    }

    // ---- what a function is, on the shared tree (SJL-13.3, step 2) -------------------------------

    /// <summary>
    /// A parameter's target, and its default where it has one; a rest parameter's target is what it binds.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A141A8
    // Broiler-Human:        PENDING
    private static Pattern Parameter(Pattern parameter, out Expression? initialiser)
    {
        switch (parameter)
        {
            case AssignmentPattern defaulted:
                initialiser = defaulted.Initializer;
                return defaulted.Target;
            case RestElement rest:
                initialiser = null;
                return rest.Argument;
            default:
                initialiser = null;
                return parameter;
        }
    }

    /// <summary>A function's formal parameters: none for a field's initialiser or a static block.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A0BEA4
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.IReadOnlyList<Pattern> ParametersOf(SyntaxNode function) =>
        function is FunctionNode node ? node.Parameters : [];

    /// <summary>
    /// A function's statements, and the one value a concise arrow or a field's initialiser returns, which
    /// the wide parser's tree held as a <c>return</c> of it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=C39083
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.IReadOnlyList<Statement> BodyOf(SyntaxNode function, out Expression? value)
    {
        switch (function)
        {
            case FunctionNode node:
                value = node.ExpressionBody;
                return node.Body;
            case FieldDefinition field:
                value = field.Value;
                return [];
            case StaticBlock block:
                value = null;
                return block.Body;
            default:
                throw JsTreeFacts.Unexpected(function, "a function");
        }
    }

    /// <summary>Whether a function is an arrow.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0AEA9F
    // Broiler-Human:        PENDING
    private static bool IsArrow(SyntaxNode function) => function is FunctionNode { IsArrow: true };

    /// <summary>
    /// A function's own strictness, as the library decided it; a field's initialiser and a static block are
    /// strict code, as all of a class is.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=D490B4
    // Broiler-Falsified-If: a function the library judged sloppy is bound strict, one it judged strict is bound sloppy, or a field's initialiser or a static block is bound sloppy
    // Broiler-Human:        PENDING
    private bool OwnStrictness(SyntaxNode function) =>
        function is not FunctionNode ||
        (parse.Strictness.TryGetValue(function, out var strict)
            ? strict
            : throw new System.InvalidOperationException(
                "the library's declaration tables hold no facts for the " + function.Kind + " at " + Where(function)));

    /// <summary>A class member's computed key expression, else <see langword="null"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=535612
    // Broiler-Human:        PENDING
    private static Expression? ComputedKey(SyntaxNode member) => member switch
    {
        MethodDefinition { Key: ComputedPropertyName key } => key.Expression,
        FieldDefinition { Key: ComputedPropertyName key } => key.Expression,
        _ => null,
    };

    /// <summary>
    /// A class member's function as the wide parser's tree held it: a method's, a field's initialiser,
    /// which is the field itself, or a static block; none for a field with no initialiser.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=8F2536
    // Broiler-Human:        PENDING
    private static SyntaxNode? MemberFunction(SyntaxNode member) => member switch
    {
        MethodDefinition method => method.Function,
        FieldDefinition { Value: not null } field => field,
        StaticBlock block => block,
        _ => null,
    };

    /// <summary>
    /// An <c>if</c> clause for the Annex B walk: a function declaration there is the one-statement list
    /// the wide parser's block held.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=67FF6B
    // Broiler-Human:        PENDING
    private static AnnexBPending AnnexBClause(Statement clause, System.Collections.Generic.HashSet<string> inherited) =>
        JsTreeFacts.IsIfClauseFunction(clause)
            ? new AnnexBPending(null, [clause], inherited)
            : new AnnexBPending(clause, null, inherited);
}
