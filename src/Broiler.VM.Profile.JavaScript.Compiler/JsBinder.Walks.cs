// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   54
// Annotated:        54/54
// Exempt:           30
// Human-reviewed:   0/54
// IP risk:          None
// Security risk:    High
// Criteria:         10/7
// Resource impact:  3/10 max
// Unverified:       54
//
// GENERATED - DO NOT EDIT MANUALLY

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

    /// <summary>What the code where a node stands is inside of, as the lowering's own flags say it.</summary>
    /// <remarks>
    /// <b>Each is the reading the lowering takes of the same position</b>, because each decides an
    /// early error the lowering used to raise from its own flags while it emitted (SJL-12.6 item 2):
    /// an arrow takes its definer's <see cref="Method"/>, <see cref="Derived"/> and
    /// <see cref="Function"/> and every other function sets its own, while
    /// <see cref="StaticBlock"/> belongs to a static block's own body and nothing nested in it.
    /// <see cref="Async"/> and <see cref="Disposing"/> belong to one unit each, an arrow's included:
    /// the first is the unit's async flag, which the lowering takes from the function alone, and the
    /// second a resource scope on the unit's exit stack, which every unit starts empty.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=287354
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

        /// <summary>A function's code, or an arrow's in one, where <c>new.target</c> resolves.</summary>
        Function = 8,

        /// <summary>
        /// Any function's code, an arrow's included, where <c>return</c> has something to return from.
        /// </summary>
        Body = 16,

        /// <summary>A class static block's own body, where a <c>return</c> is an early error.</summary>
        StaticBlock = 32,

        /// <summary>
        /// Code where <c>arguments</c> names a binding the unit itself makes, which only an evaluated
        /// program inside a class field initialiser asks about.
        /// </summary>
        ArgumentsBound = 64,

        /// <summary>
        /// Code of a unit the lowering emits as an async one, where <c>await</c> has something to wait
        /// in: an async function's own code, and a module's top level when the module awaits there.
        /// </summary>
        Async = 128,

        /// <summary>
        /// Code inside a statement list, or a <c>for</c> head, that the lowering opened a resource scope
        /// for in the same unit, where a <c>using</c> declaration has a scope to be disposed with.
        /// </summary>
        Disposing = 256,
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
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=9197DB
        // Broiler-Human:        PENDING
        internal Context Inside(ExitKind kind, string label, JsStatement statement) =>
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=BD87FF
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
    }

    /// <summary>One node still to visit, with what is in force where it stands.</summary>
    /// <param name="Node">The node.</param>
    /// <param name="Context">The strictness, exits and frames in force there.</param>
    /// <param name="Label">
    /// The label written directly on it, when it is a loop or a <c>switch</c>; empty otherwise.
    /// </param>
    /// <param name="Use">What its parent asks of it.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=51F3C3
    // Broiler-Human:        PENDING
    private readonly record struct Pending(JsNode Node, Context Context, string Label, Use Use = Use.Value);

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
    /// each <c>export</c> that carries one as the declaration itself, through the lowering's one
    /// reading of that (<see cref="JsCompiler.Unwrapped"/>); a script holds none.
    /// <b>Only a script or a sloppy evaluated program owes an alias</b>, and the names that block one
    /// there are the top level's own lexical names - not its functions, which are <c>var</c>-scoped.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=3244EA
    // Broiler-Falsified-If: a script's or an evaluated program's var names, functions or Annex B aliases differ from what its hoisting declared before SJL-12.6 item 1, or a module's exported declarations are left out of its top level
    // Broiler-Human:        PENDING
    private HoistingScope BindProgram(JsProgramNode program, bool strict)
    {
        var body = JsCompiler.Unwrapped(program.Body);
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=5318CE
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
            if (IsArguments(parameterName))
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
    /// <param name="topLevelAwait">Whether a module awaits at its own top level, as its parser saw.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=10C8E1
    // Broiler-Falsified-If: a function, block, switch, class body or jump the lowering can reach is not given its facts, a node is walked under a strictness other than its definer's, or a node is met before one the source writes ahead of it
    // Broiler-Human:        PENDING
    private void BindTree(JsProgramNode program, bool strict, bool topLevelAwait)
    {
        var frame = Frame.None;

        // A MODULE'S TOP LEVEL IS A UNIT OF ITS OWN: an async one when the module awaits there, which
        // is the parser's reading the lowering takes too, and inside a resource scope when its own
        // list declares a resource. A script's top level and an evaluated program's are neither.
        if (unit.Goal == SliceGoal.Module)
        {
            if (topLevelAwait)
            {
                frame |= Frame.Async;
            }

            if (JsCompiler.UsingIn(program.Body, out _) != JsUsing.None)
            {
                frame |= Frame.Disposing;
            }
        }

        // EVAL CODE'S OWN TOP LEVEL IS WHERE ITS CALLER STANDS, as far as the caller's request says:
        // the lowering takes `super`, `super()` and `new.target` there from the request's flags.
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

            if ((flags & Format.JsFormat.EvalRequestFlags.InFunction) != 0)
            {
                frame |= Frame.Function;
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
        Push(inside, program.Body, new Context(strict, null, frame, null));
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=F38111
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
            case JsFunctionNode function:
                VisitFunction(pending, function, context, use);
                return;

            // A CLASS BODY CHANGES NO STRICTNESS HERE, because the lowering's does not: its members
            // are compiled under their definer's strictness or their own, as any function is. ITS
            // HERITAGE IS EVALUATED IN THE PRIVATE ENVIRONMENT AROUND IT, and its keys and members in
            // its own, which the lowering declares after compiling the heritage.
            case JsClassNode definition:
            {
                var names = PrivateNamesOf(definition);
                privateNames[definition] = names;
                Push(pending, definition.Heritage, context);

                var inner = context.WithPrivateNames(names);
                var constructor = JsCompiler.FindConstructor(definition);

                foreach (var member in definition.Members)
                {
                    RefuseArgumentsIn(member, context);
                    Push(pending, member.Computed, inner);

                    if (member.Function is { } body)
                    {
                        pending.Add(new Pending(
                            body, inner, string.Empty, MemberUse(member, body, constructor, definition.HasHeritage)));
                    }
                }

                return;
            }

            case JsBlockStatement block:
            {
                var declarations = DeclarationsOf(block.Body);
                lists[block] = declarations;
                Push(
                    pending,
                    block.Body,
                    ResourceScoped(InsideRecord(context, declarations.Lexical), block.Body));

                return;
            }

            // THE CLAUSES OF A `switch` ARE ONE STATEMENT LIST AND ONE RECORD, and its tests are
            // evaluated before the `switch` is a statement a `break` may leave. The record's names
            // are declared after the discriminant is lowered and before the first test is.
            case JsSwitchStatement switched:
            {
                var declarations = DeclarationsOf(Clauses(switched));
                lists[switched] = declarations;
                var declared = InsideRecord(context, declarations.Lexical);
                var inner = declared.Inside(ExitKind.Switch, label, switched);
                Push(pending, switched.Discriminant, context);

                foreach (var clause in switched.Clauses)
                {
                    Push(pending, clause.Test, declared);
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

            // A LEXICAL HEAD'S NAMES ARE DECLARED BEFORE ITS INITIALISER RUNS, and its test, its
            // update and its body see them. The head's fact is recorded and then read back, so
            // the early errors decided under it - which `arguments` the unit binds - are decided
            // from the fact itself (SJL-12.6 item 2), as they are for a catch parameter's. A RESOURCE
            // THE HEAD DECLARES BELONGS TO THE WHOLE LOOP, whose scope the lowering opens before the
            // head's declaration and closes after the loop.
            case JsForStatement loop:
            {
                heads[loop] = ForHeadOf(loop);
                var inner = InsideRecord(context, LoopHead(loop));

                if (loop.Initialiser is JsVariableStatement { Using: not JsUsing.None })
                {
                    inner = inner.And(Frame.Disposing);
                }

                Push(pending, loop.Initialiser, inner);
                Push(pending, loop.Test, inner);
                Push(pending, loop.Update, inner);
                Push(pending, loop.Body, inner.Inside(ExitKind.Loop, label, loop));
                return;
            }

            case JsForInStatement loop:
            {
                heads[loop] = EachHeadOf(loop.Declaration, loop.Name, loop.Pattern);
                var inner = InsideRecord(context, LoopHead(loop));
                Head(pending, loop.Declaration, loop.Name, loop.Pattern, loop.Target, JsUsing.None, context, inner);
                Push(pending, loop.Initialiser, context);

                // ANNEX B'S INITIALISER IS WRITTEN THROUGH THE NAME before the object is evaluated.
                if (loop.Initialiser is not null)
                {
                    Referenced(context, loop.Name);
                }

                Push(pending, loop.Right, inner);
                Push(pending, loop.Body, inner.Inside(ExitKind.Loop, label, loop));
                return;
            }

            // EACH TURN'S RESOURCE IS ITS OWN, in a scope the lowering opens around the turn's body.
            case JsForOfStatement loop:
            {
                heads[loop] = EachHeadOf(loop.Declaration, loop.Name, loop.Pattern);
                var inner = InsideRecord(context, LoopHead(loop));
                var turn = loop.Using == JsUsing.None ? inner : inner.And(Frame.Disposing);
                Head(pending, loop.Declaration, loop.Name, loop.Pattern, loop.Target, loop.Using, context, inner);
                Push(pending, loop.Right, inner);
                Push(pending, loop.Body, turn.Inside(ExitKind.Loop, label, loop));
                return;
            }

            // A LABEL ON A LOOP OR A `switch` IS THAT STATEMENT'S OWN, and on anything else it makes a
            // statement a `break` naming it may leave - which is how the lowering reads one.
            case JsLabelledStatement labelled:
                if (labelled.Body is JsWhileStatement or JsDoWhileStatement or JsForStatement or
                    JsForInStatement or JsForOfStatement or JsSwitchStatement)
                {
                    pending.Add(new Pending(labelled.Body, context, labelled.Label));
                }
                else
                {
                    Push(
                        pending,
                        labelled.Body,
                        context.Inside(ExitKind.Label, labelled.Label, labelled));
                }

                return;

            // A JUMP WITH NOWHERE TO GO IS THE ONE PLACE THE JUMP TARGET IS READ, through the fact
            // the walk has just recorded: the lowering keeps finding its targets on its own exit
            // stack, because a `finally` body it emits at a jump holds statements the jump is
            // leaving.
            case JsBreakStatement jump:
            {
                jumps[jump] = Target(context.Exits, jump.Label, continues: false);

                if (JumpTarget(jump) is null)
                {
                    Report(
                        context,
                        jump.Span,
                        SliceSourceDiagnosticCode.IllegalBreak,
                        "`break` names no enclosing loop or switch");
                }

                return;
            }

            case JsContinueStatement jump:
            {
                jumps[jump] = Target(context.Exits, jump.Label, continues: true);

                if (JumpTarget(jump) is null)
                {
                    Report(
                        context,
                        jump.Span,
                        SliceSourceDiagnosticCode.IllegalContinue,
                        "`continue` names no enclosing loop");
                }

                return;
            }

            case JsExpressionStatement statement:
                Push(pending, statement.Expression, context);
                return;

            case JsVariableStatement statement:
                VisitDeclarators(pending, statement, context);
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

            // A `return` HAS SOMETHING TO RETURN FROM ONLY IN A FUNCTION'S OWN CODE, and a class
            // static block compiles to a function and is not one: the word is an early error there
            // too, under the code the catalogue records for it (2207; JSD-0080 decisions D12 and
            // D13). The lowering lowered nothing of the value after refusing either; the value is
            // walked all the same, so what it holds is reported too.
            case JsReturnStatement statement:
                if (!context.Is(Frame.Body))
                {
                    Report(
                        context,
                        statement.Span,
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "`return` has nothing to return from outside a function");
                }
                else if (context.Is(Frame.StaticBlock))
                {
                    Report(
                        context,
                        statement.Span,
                        SliceSourceDiagnosticCode.IllegalBreak,
                        "`return` has nothing to return from inside a class static block");
                }

                Push(pending, statement.Value, context);
                return;

            case JsThrowStatement statement:
                Push(pending, statement.Value, context);
                return;

            // A CATCH PARAMETER'S NAMES ARE THE BLOCK'S AND ITS OWN DEFAULTS', and nothing outside.
            case JsTryStatement statement:
            {
                catches[statement] = CatchNamesOf(statement);
                var caught = InsideRecord(context, CatchParameters(statement));
                Push(pending, statement.Block, context);

                if (statement.CatchPattern is not null)
                {
                    pending.Add(new Pending(statement.CatchPattern, caught, string.Empty, Use.Initialise));
                }

                Push(pending, statement.Handler, caught);
                Push(pending, statement.Finaliser, context);
                return;
            }

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
                VisitObject(pending, expression, context);
                return;

            // A NAMED FUNCTION EXPRESSION'S OWN NAME IS A BINDING OF A RECORD AROUND THE FUNCTION.
            case JsFunctionExpression expression:
                Push(
                    pending,
                    expression.Function,
                    !expression.Function.IsArrow && IsArguments(expression.Function.Name)
                        ? InsideRecord(context, bindsArguments: true)
                        : context);

                return;

            case JsYieldExpression expression:
                Push(pending, expression.Operand, context);
                return;

            // `await` WAITS ONLY IN A UNIT THE LOWERING EMITS AS AN ASYNC ONE, and anywhere else it is
            // refused under the code the catalogue records for it (2104; JSD-0080 decisions D12 and
            // D13). A template substitution's own parse lets one into the parameters of an arrow that
            // is not async, whose unit is the arrow's.
            case JsAwaitExpression expression:
                if (!context.Is(Frame.Async))
                {
                    Report(
                        context,
                        expression.Span,
                        SliceSourceDiagnosticCode.ConstructOutsideManifest,
                        "`await` is only admitted inside an async function");
                }

                Push(pending, expression.Operand, context);
                return;

            case JsUnaryExpression expression:
                if (expression.Operator == SliceTokenKind.Delete && expression.Operand is JsPrivateMemberExpression)
                {
                    Report(
                        context,
                        expression.Span,
                        SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                        "a private element cannot be deleted");
                }

                Push(pending, expression.Operand, context);
                return;

            case JsUpdateExpression expression:
                if (!IsReference(expression.Operand, context.Strict))
                {
                    Report(
                        context,
                        expression.Span,
                        SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                        "the operand of an update operator is not a reference");
                }

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
                VisitAssignment(pending, expression, context);
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
                pending.Add(new Pending(expression.Target, context, string.Empty, Use.Assign));
                Push(pending, expression.Value, context);
                return;

            case JsClassExpression expression:
                Push(pending, expression.Class, context);
                return;

            case JsSuperMemberExpression expression:
                if (!context.Is(Frame.Method))
                {
                    Report(
                        context,
                        expression.Span,
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "`super` is only admitted inside a method");
                }

                Push(pending, expression.Computed, context);
                return;

            case JsPrivateMemberExpression expression:
                ResolvePrivate(context, expression.Span, expression.Name);
                Push(pending, expression.Target, context);
                return;

            case JsPrivateInExpression expression:
                ResolvePrivate(context, expression.Span, expression.Name);
                Push(pending, expression.Target, context);
                return;

            case JsSuperCallExpression expression:
                if (!context.Is(Frame.Derived))
                {
                    Report(
                        context,
                        expression.Span,
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "a `super` call is only admitted in the constructor of a class with a heritage");
                }

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
                VisitLeaf(pending, pattern, context, use);
                return;

            case JsArrayPattern pattern:
                foreach (var element in pattern.Elements)
                {
                    if (element is not null)
                    {
                        pending.Add(new Pending(element.Target, context, string.Empty, use));
                        Push(pending, element.Default, context);
                    }
                }

                if (pattern.Rest is not null)
                {
                    pending.Add(new Pending(pattern.Rest, context, string.Empty, use));
                }

                return;

            case JsObjectPattern pattern:
                foreach (var property in pattern.Properties)
                {
                    Push(pending, property.Computed, context);
                    pending.Add(new Pending(property.Value.Target, context, string.Empty, use));
                    Push(pending, property.Value.Default, context);
                }

                if (pattern.Rest is not null)
                {
                    pending.Add(new Pending(pattern.Rest, context, string.Empty, use));
                }

                return;

            case JsNewTargetExpression expression:
                if (!context.Is(Frame.Function))
                {
                    Report(
                        context,
                        expression.Span,
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "`new.target` is only admitted inside a function");
                }

                return;

            case JsImportMeta expression:
                if (unit.Goal != SliceGoal.Module)
                {
                    Report(
                        context,
                        expression.Span,
                        SliceSourceDiagnosticCode.ImportMetaOutsideModuleGoal,
                        "`import.meta` is only admitted inside a module");
                }

                return;

            case JsRegExpLiteral expression:
                CheckRegExp(context, expression);
                return;

            // A NAME IN AN EXPRESSION IS A REFERENCE THE LOWERING RESOLVES, and in eval code inside a
            // field initialiser one that reaches the caller as `arguments` is an early error.
            case JsIdentifier expression:
                Referenced(context, expression.Name);
                return;

            // A LEAF: a literal, `this`, an empty or `debugger` statement, an `import` declaration.
            // None holds a node with a fact or an early error of its own.
            case JsNumberLiteral or JsBigIntLiteral or JsStringLiteral or JsBooleanLiteral or
                JsNullLiteral or JsThisExpression or JsEmptyStatement or JsDebuggerStatement or
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

    /// <summary>One function: its facts, and its parameters and body under what it is inside of.</summary>
    /// <remarks>
    /// <b>An arrow takes its definer's frames and every other function sets its own</b>, which is the
    /// whole of what <c>super</c>, <c>super()</c> and <c>new.target</c> mean inside an arrow - the
    /// lowering's own rule. A static block's frame is its own body's alone. <b>Every function is a unit
    /// of its own</b>, an arrow included: an async one only when it is async itself, and with no
    /// resource scope until its body's own list declares a resource.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=F0ABC5
    // Broiler-Falsified-If: a function's code is walked as a method's, a derived constructor's or a function's where the lowering compiles it otherwise, or a function the lowering never compiles is walked as one it does
    // Broiler-Human:        PENDING
    private void VisitFunction(
        System.Collections.Generic.List<Pending> pending, JsFunctionNode function, Context context, Use use)
    {
        var facts = BindFunction(function, context.Strict);
        functions[function] = facts;

        var frame = (context.Frame & (Frame.Silent | Frame.ArgumentsBound)) | Frame.Body;

        if (function.IsArrow)
        {
            frame |= context.Frame & (Frame.Method | Frame.Derived | Frame.Function);
        }
        else
        {
            frame |= Frame.Function;

            if (use is Use.Method or Use.Derived or Use.StaticBlock)
            {
                frame |= Frame.Method;
            }

            if (use == Use.Derived)
            {
                frame |= Frame.Derived;
            }
        }

        if (use == Use.StaticBlock)
        {
            frame |= Frame.StaticBlock;
        }

        if (function.IsAsync)
        {
            frame |= Frame.Async;
        }

        // A DECLARATION A SLOPPY EVALUATED PROGRAM DECLARES AGAIN LATER IS NEVER COMPILED: the
        // lowering instantiates the last declaration of each name and no other.
        if (overridden.Contains(function))
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

        foreach (var parameter in function.Parameters)
        {
            pending.Add(new Pending(parameter.Target, parameters, string.Empty, Use.Initialise));
            Push(pending, parameter.Default, parameters);
        }

        Push(pending, function.Body, ResourceScoped(body, function.Body));
    }

    /// <summary>
    /// This context inside a statement list, and inside the resource scope the lowering opens around
    /// the list when the list itself declares a resource.
    /// </summary>
    /// <remarks>
    /// <b>The test is the lowering's own</b> (<see cref="JsCompiler.UsingIn"/>): only the list's own
    /// declarations count, and a block inside it opens a scope of its own.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=018728
    // Broiler-Human:        PENDING
    private static Context ResourceScoped(
        Context context, System.Collections.Generic.IReadOnlyList<JsStatement> list) =>
        JsCompiler.UsingIn(list, out _) == JsUsing.None ? context : context.And(Frame.Disposing);

    /// <summary>One declaration statement's declarators, each with the early errors of its own.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=399BCD
    // Broiler-Human:        PENDING
    private void VisitDeclarators(
        System.Collections.Generic.List<Pending> pending, JsVariableStatement statement, Context context)
    {
        var mode = statement.Kind == SliceDeclarationKind.Var ? Use.Var : Use.Initialise;

        foreach (var declarator in statement.Declarators)
        {
            // A DESTRUCTURING DECLARATION ALWAYS HAS AN INITIALISER, whatever its keyword, and a
            // `const` that names one does too.
            if (declarator.Pattern is not null && declarator.Initialiser is null)
            {
                Report(
                    context,
                    declarator.Span,
                    SliceSourceDiagnosticCode.ConstWithoutInitialiser,
                    "a destructuring declaration needs an initialiser");
            }
            else if (declarator.Pattern is null &&
                statement.Kind == SliceDeclarationKind.Const &&
                declarator.Initialiser is null)
            {
                Report(
                    context,
                    declarator.Span,
                    SliceSourceDiagnosticCode.ConstWithoutInitialiser,
                    "`const " + declarator.Name + "` needs an initialiser");
            }
            else if (declarator.Pattern is null && statement.Using != JsUsing.None && !context.Is(Frame.Disposing))
            {
                // A RESOURCE IS REGISTERED WITH THE SCOPE AROUND IT, and where the lowering opened none
                // in the unit it refused the declaration under the code the catalogue records for it
                // (2104; JSD-0080 decisions D12 and D13). The parser refuses every such place first today.
                Report(
                    context,
                    declarator.Span,
                    SliceSourceDiagnosticCode.ConstructOutsideManifest,
                    "a `using` declaration outside a statement list that can dispose it");
            }
            else if (declarator.Pattern is null && mode == Use.Var && declarator.Initialiser is not null)
            {
                Referenced(context, declarator.Name);
            }

            if (declarator.Pattern is not null)
            {
                pending.Add(new Pending(declarator.Pattern, context, string.Empty, mode));
            }

            Push(pending, declarator.Initialiser, context);
        }
    }

    /// <summary>One object literal's entries, each as the lowering reads it.</summary>
    /// <remarks>
    /// A <c>{ a = 1 }</c> entry that reached the lowering was never a pattern; a <c>__proto__: v</c>
    /// written a second time sets the prototype twice; an entry in method form is a method, whose
    /// <c>super</c> resolves.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=84B66F
    // Broiler-Human:        PENDING
    private void VisitObject(
        System.Collections.Generic.List<Pending> pending, JsObjectLiteral literal, Context context)
    {
        var prototyped = false;

        foreach (var entry in literal.Entries)
        {
            Push(pending, entry.Computed, context);

            if (entry.Cover)
            {
                Report(
                    context,
                    entry.Span,
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    "`=` is not an object literal entry outside a destructuring assignment");

                Push(pending, entry.Value, context);
                continue;
            }

            if (entry.Kind != JsPropertyKind.Spread && entry.IsMethod && entry.Value is JsFunctionExpression method)
            {
                pending.Add(new Pending(method.Function, context, string.Empty, Use.Method));
                continue;
            }

            if (entry.Kind != JsPropertyKind.Spread && !entry.IsMethod && entry.Computed is null &&
                !entry.Shorthand && string.Equals(entry.Key, "__proto__", System.StringComparison.Ordinal))
            {
                if (prototyped)
                {
                    Report(
                        context,
                        entry.Span,
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "an object literal may set `__proto__` once");
                }

                prototyped = true;
            }

            Push(pending, entry.Value, context);
        }
    }

    /// <summary>One assignment, with the target the lowering can write through or the refusal.</summary>
    /// <remarks>
    /// <b>A logical assignment writes through a name or a property and nothing else</b>: any other
    /// target is refused under the code the catalogue records for it (2104; JSD-0080 decisions D12 and
    /// D13). The lowering lowered neither half after refusing it; both are walked all the same. The
    /// parser refuses every such target first today, under 2205.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=AF90E9
    // Broiler-Human:        PENDING
    private void VisitAssignment(
        System.Collections.Generic.List<Pending> pending, JsAssignmentExpression assignment, Context context)
    {
        var target = assignment.Target;

        if (assignment.Operator is SliceTokenKind.AmpersandAmpersand or SliceTokenKind.BarBar or
            SliceTokenKind.QuestionQuestion)
        {
            if (target is not (JsMemberExpression or JsPrivateMemberExpression or JsSuperMemberExpression or
                JsIdentifier))
            {
                Report(
                    context,
                    assignment.Span,
                    SliceSourceDiagnosticCode.ConstructOutsideManifest,
                    "a logical assignment to a target that is neither a name nor a property is not admitted");
            }

            Push(pending, target, context);
            Push(pending, assignment.Value, context);
            return;
        }

        if (!IsReference(target, context.Strict))
        {
            Report(
                context,
                assignment.Span,
                SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                "the left-hand side of an assignment is not a reference");
        }

        Push(pending, target, context);
        Push(pending, assignment.Value, context);
    }

    /// <summary>
    /// One leaf of a pattern: a binding a declaration initialises, a <c>var</c> it writes, or a
    /// reference an assignment writes through.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=16EE09
    // Broiler-Falsified-If: a leaf is read as a binding where the lowering writes through it, or as a reference where it initialises one
    // Broiler-Human:        PENDING
    private void VisitLeaf(
        System.Collections.Generic.List<Pending> pending, JsTargetPattern leaf, Context context, Use use)
    {
        switch (use)
        {
            case Use.Initialise or Use.Var:
                if (leaf.Target is not JsIdentifier)
                {
                    Report(
                        context,
                        leaf.Target.Span,
                        SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                        "a declaration's pattern binds names, and this is not one");

                    Push(pending, leaf.Target, context);
                    return;
                }

                // A `var`'s leaf is written through its name; a binding initialised is no reference.
                if (use == Use.Var)
                {
                    Push(pending, leaf.Target, context);
                }

                return;

            case Use.Assign:
                if (!IsReference(leaf.Target, context.Strict))
                {
                    Report(
                        context,
                        leaf.Target.Span,
                        SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                        "the left-hand side of an assignment is not a reference");
                }

                Push(pending, leaf.Target, context);
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=72EF1C
    // Broiler-Human:        PENDING
    private void Head(
        System.Collections.Generic.List<Pending> pending,
        SliceDeclarationKind? declaration,
        string name,
        JsPattern? pattern,
        JsExpression? target,
        JsUsing resource,
        Context context,
        Context inner)
    {
        var lexical = declaration is SliceDeclarationKind.Let or SliceDeclarationKind.Const;

        if (target is not null)
        {
            if (resource == JsUsing.None && pattern is null && !lexical &&
                declaration != SliceDeclarationKind.Var && !IsReference(target, context.Strict))
            {
                Report(
                    context,
                    target.Span,
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
        else if (resource == JsUsing.None && declaration == SliceDeclarationKind.Var)
        {
            Referenced(context, name);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=085910
    // Broiler-Human:        PENDING
    private static void Push(System.Collections.Generic.List<Pending> pending, JsNode? node, Context context)
    {
        if (node is not null)
        {
            pending.Add(new Pending(node, context, string.Empty));
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=9B6598
    // Broiler-Human:        PENDING
    private static void Push(
        System.Collections.Generic.List<Pending> pending,
        System.Collections.Generic.IReadOnlyList<JsStatement> statements,
        Context context)
    {
        foreach (var statement in statements)
        {
            pending.Add(new Pending(statement, context, string.Empty));
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C37E1D
    // Broiler-Falsified-If: the var names or function declarations of a scope come out in an order, or with a multiplicity, other than the source's, or a function declared below the scope's top level, labels aside, is collected as the scope's
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=746203
    // Broiler-Falsified-If: a declaration is admitted whose name a `var` of the same spelling could not be added under; a list's own lexical names reach the set its own declarations are judged by; a block, a switch, a lexical loop head or a destructuring catch parameter leaves its names out of the set what is inside it is judged by; or the aliases come out in another order
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

    /// <summary>The names a <c>for</c> head declares with <c>let</c> or <c>const</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=3EA16A
    // Broiler-Human:        PENDING
    private static string[] ForHeadOf(JsForStatement loop)
    {
        if (loop.Initialiser is not JsVariableStatement head || head.Kind == SliceDeclarationKind.Var)
        {
            return [];
        }

        var names = new System.Collections.Generic.List<string>();

        foreach (var declarator in head.Declarators)
        {
            DeclaratorNames(declarator, names);
        }

        return [.. names];
    }

    /// <summary>The names a <c>for-in</c> or <c>for-of</c> head declares with <c>let</c> or <c>const</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=1FEBAE
    // Broiler-Human:        PENDING
    private static string[] EachHeadOf(SliceDeclarationKind? declaration, string name, JsPattern? pattern)
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=AB66D5
    // Broiler-Human:        PENDING
    private static string[] CatchNamesOf(JsTryStatement guarded)
    {
        if (guarded.CatchPattern is not null)
        {
            var names = new System.Collections.Generic.List<string>();
            PatternNames(guarded.CatchPattern, names);
            return [.. names];
        }

        return guarded.Handler is not null && guarded.CatchParameter.Length != 0
            ? [guarded.CatchParameter]
            : [];
    }

    /// <summary>A module's top-level lexical declarations, an exported one in the export's place.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=6EFF9A
    // Broiler-Human:        PENDING
    private static ModuleDeclaration[] ModuleDeclarationsOf(System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
        var declared = new System.Collections.Generic.List<ModuleDeclaration>();

        foreach (var statement in JsCompiler.Unwrapped(body))
        {
            if (statement is JsClassDeclaration definition && definition.Class.Name.Length != 0)
            {
                declared.Add(new ModuleDeclaration(definition.Class.Name, false, definition.Span, true));
                continue;
            }

            if (statement is not JsVariableStatement variable || variable.Kind == SliceDeclarationKind.Var)
            {
                continue;
            }

            foreach (var declarator in variable.Declarators)
            {
                var names = new System.Collections.Generic.List<string>();
                DeclaratorNames(declarator, names);

                foreach (var name in names)
                {
                    declared.Add(new ModuleDeclaration(
                        name, variable.Kind == SliceDeclarationKind.Const, declarator.Span, false));
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
