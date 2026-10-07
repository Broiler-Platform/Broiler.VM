// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           0
// Human-reviewed:   0/14
// IP risk:          None
// Security risk:    High
// Criteria:         4/4
// Resource impact:  3/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // ---- statements ----------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F73A6A
    // Broiler-Human:        PENDING
    private void CompileStatements(
        System.Collections.Generic.IReadOnlyList<JsStatement> body, int completion)
    {
        foreach (var statement in body)
        {
            CompileStatement(statement, completion);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BD27A1
    // Broiler-Human:        PENDING
    private void CompileStatement(JsStatement statement, int completion)
    {
        Position(statement.Span);

        switch (statement)
        {
            case JsEmptyStatement:
            case JsDebuggerStatement:
                break;

            case JsExpressionStatement expression:
                CompileExpression(expression.Expression);

                if (completion >= 0)
                {
                    EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, completion);
                }
                else
                {
                    Emit(JsOpcode.Pop);
                }

                break;

            case JsVariableStatement variable:
                CompileVariable(variable);
                break;

            // A FUNCTION DECLARATION EMITS NOTHING WHERE IT STANDS, because everything it does was
            // done when the scope holding it was entered - except the one write Annex B adds, and
            // that write is here BECAUSE it belongs where the declaration stands. That is what
            // makes `if (x) { function f() { } }` leave `f` undefined when `x` is false, and what
            // makes the aliased value the block binding's CURRENT one rather than its first.
            case JsFunctionDeclaration declared:
                if (annexB.Contains(declared))
                {
                    EmitAnnexBAlias(declared.Function.Name);
                }

                break;

            case JsClassDeclaration declaration:
                CompileClassDeclaration(declaration);
                break;

            case JsBlockStatement block:
                CompileBlock(block, completion);
                break;

            case JsWithStatement scoped:
                CompileWith(scoped, completion);
                break;

            case JsIfStatement conditional:
                CompileIf(conditional, completion);
                break;

            case JsWhileStatement loop:
                CompileWhile(loop, completion, string.Empty);
                break;

            case JsDoWhileStatement loop:
                CompileDoWhile(loop, completion, string.Empty);
                break;

            case JsForStatement loop:
                CompileFor(loop, completion, string.Empty);
                break;

            case JsForInStatement loop:
                CompileForIn(loop, completion, string.Empty);
                break;

            case JsForOfStatement loop:
                CompileForOf(loop, completion, string.Empty);
                break;

            case JsBreakStatement jump:
                CompileJumpOut(jump.Span, jump.Label, wantsContinue: false);
                break;

            case JsContinueStatement jump:
                CompileJumpOut(jump.Span, jump.Label, wantsContinue: true);
                break;

            case JsReturnStatement returned:
                CompileReturn(returned);
                break;

            case JsThrowStatement thrown:
                CompileExpression(thrown.Value);
                Emit(JsOpcode.Throw);
                break;

            case JsTryStatement guarded:
                CompileTry(guarded, completion);
                break;

            case JsSwitchStatement switched:
                CompileSwitch(switched, completion, string.Empty);
                break;

            case JsLabelledStatement labelled:
                CompileLabelled(labelled, completion);
                break;

            default:
                Refuse(
                    statement.Span,
                    SliceSourceDiagnosticCode.ConstructOutsideManifest,
                    "this statement is not admitted by the declared feature manifest");

                break;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6B9B33
    // Broiler-Human:        PENDING
    private void CompileLabelled(JsLabelledStatement labelled, int completion)
    {
        switch (labelled.Body)
        {
            case JsWhileStatement loop:
                CompileWhile(loop, completion, labelled.Label);
                return;

            case JsDoWhileStatement loop:
                CompileDoWhile(loop, completion, labelled.Label);
                return;

            case JsForStatement loop:
                CompileFor(loop, completion, labelled.Label);
                return;

            case JsForInStatement loop:
                CompileForIn(loop, completion, labelled.Label);
                return;

            case JsForOfStatement loop:
                CompileForOf(loop, completion, labelled.Label);
                return;

            case JsSwitchStatement switched:
                CompileSwitch(switched, completion, labelled.Label);
                return;

            default:
            {
                var exit = new Exit(ExitKind.Label, labelled.Label, blockDepth)
                {
                    Break = NewLabel(),
                };

                exits.Add(exit);
                CompileStatement(labelled.Body, completion);
                exits.RemoveAt(exits.Count - 1);
                Mark(exit.Break!);
                return;
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=792901
    // Broiler-Human:        PENDING
    private void CompileVariable(JsVariableStatement variable)
    {
        foreach (var declarator in variable.Declarators)
        {
            if (declarator.Pattern is not null)
            {
                // A DESTRUCTURING DECLARATION ALWAYS HAS AN INITIALISER, whatever its keyword.
                // `var a;` is fine and `var [a];` is not, because there is nothing for the pattern
                // to take apart - and the grammar says so for `var` as loudly as for `const`.
                if (declarator.Initialiser is null)
                {
                    Refuse(
                        declarator.Span,
                        SliceSourceDiagnosticCode.ConstWithoutInitialiser,
                        "a destructuring declaration needs an initialiser");

                    continue;
                }

                if (variable.Kind != SliceDeclarationKind.Var)
                {
                    DeclarePatternNames(
                        declarator.Pattern, variable.Kind == SliceDeclarationKind.Const);
                }

                CompileExpression(declarator.Initialiser);

                BindPattern(
                    declarator.Pattern,
                    variable.Kind == SliceDeclarationKind.Var ? BindMode.Var : BindMode.Initialise);

                continue;
            }

            if (variable.Kind == SliceDeclarationKind.Const && declarator.Initialiser is null)
            {
                Refuse(
                    declarator.Span,
                    SliceSourceDiagnosticCode.ConstWithoutInitialiser,
                    "`const " + declarator.Name + "` needs an initialiser");

                continue;
            }

            // A RESOURCE IS REGISTERED BEFORE ITS BINDING LEAVES THE DEAD ZONE, which is the order
            // `InitializeBinding` gives the two: a value with no disposer is a `TypeError` raised
            // while the name is still unreadable, and the value stays on the stack across the
            // registration so the binding is written from the same one.
            if (variable.Using != JsUsing.None)
            {
                var resource = scope.Has(declarator.Name)
                    ? scope.SlotOf(declarator.Name)
                    : scope.Declare(declarator.Name, constant: true);

                CompileNamedValue(declarator.Initialiser!, declarator.Name);
                EmitRegistration(declarator.Span, variable.Using);
                EmitScoped(JsOpcode.InitialiseScoped, 0, resource);
                continue;
            }

            if (scope.Kind == ScopeKind.Program && blockDepth == 0)
            {
                // `let x;` STILL HAS TO RUN. A `var` with no initialiser writes nothing, because
                // hoisting already made the property and `undefined` is what it holds; a lexical
                // binding with no initialiser is in its dead zone until the declaration is REACHED,
                // and what reaches it is this instruction. Skipping it would leave `let x;` in a
                // dead zone for the rest of the program.
                if (programLexicals.ContainsKey(declarator.Name))
                {
                    if (declarator.Initialiser is null)
                    {
                        Emit(JsOpcode.LoadUndefined);
                    }
                    else
                    {
                        CompileNamedValue(declarator.Initialiser, declarator.Name);
                    }

                    Emit(JsOpcode.InitialiseGlobalLexical, InternedName(declarator.Name));
                    continue;
                }

                if (declarator.Initialiser is null)
                {
                    continue;
                }

                CompileNamedValue(declarator.Initialiser, declarator.Name);
                Emit(JsOpcode.StoreGlobal, InternedName(declarator.Name));
                continue;
            }

            // A `var` NAMES A BINDING THAT ALREADY EXISTS. Hoisting created it in the
            // enclosing function or program scope, so a declaration inside a block writes THAT
            // one; declaring it again here would create a second binding the rest of the function
            // cannot see.
            if (variable.Kind == SliceDeclarationKind.Var)
            {
                if (declarator.Initialiser is null)
                {
                    continue;
                }

                // IN A `with` BODY THE NAME IS RESOLVED BEFORE ITS INITIALISER RUNS, as an
                // assignment's is (14.3.2.1: ResolveBinding, then the initialiser, then PutValue),
                // so an initialiser that deletes the object's property still writes the object.
                // Until 2026-10-03 the name was resolved at the write (JSC-254).
                var initialiser = declarator.Initialiser;
                var initialised = declarator.Name;

                if (TryEmitShadowedReference(
                    declarator.Span, initialised, read: false, () => CompileNamedValue(initialiser, initialised)))
                {
                    Emit(JsOpcode.Pop);
                    continue;
                }

                CompileNamedValue(declarator.Initialiser, declarator.Name);
                StoreName(declarator.Span, declarator.Name);
                Emit(JsOpcode.Pop);
                continue;
            }

            var slot = scope.Has(declarator.Name)
                ? scope.SlotOf(declarator.Name)
                : scope.Declare(declarator.Name, variable.Kind == SliceDeclarationKind.Const);

            if (declarator.Initialiser is null)
            {
                Emit(JsOpcode.LoadUndefined);
            }
            else
            {
                CompileNamedValue(declarator.Initialiser, declarator.Name);
            }

            EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        }
    }

    /// <summary>
    /// Finds the block-level function declarations Annex B additionally binds in the var scope.
    /// </summary>
    /// <param name="body">The top level of one hoisting scope: a script, a module or a body.</param>
    /// <param name="blocked">
    /// The names a <c>var</c> of the same spelling could not be added to this scope for: its
    /// parameters, <c>arguments</c>, and every lexical name declared at its top level.
    /// </param>
    /// <param name="aliases">The names collected, in the order the declarations are written.</param>
    /// <remarks>
    /// <para>
    /// <b>Annex B's condition is a question about an EARLY ERROR and this is the whole of the
    /// question.</b> The clause admits a declaration when replacing it with <c>var F</c> would not
    /// have been a syntax error — so the walk carries the set of names that would have collided
    /// with such a <c>var</c>, adds each enclosing block's own lexical names to it on the way down,
    /// and admits a declaration exactly when its name is not in the set its ENCLOSING blocks give.
    /// A block's own lexical names are not in that set, because the declaration is one of them.
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
    /// <b>It stops where a hoisting scope stops.</b> A nested function, a class body and a method
    /// are scopes of their own and their declarations are their own <see cref="HoistFunction"/>'s
    /// to find; nothing here descends into one.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=8DEF5C
    // Broiler-Falsified-If: a declaration is admitted whose name a `var` of the same spelling could not be added under
    // Broiler-Human:        PENDING
    private void ScanAnnexB(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        System.Collections.Generic.HashSet<string> blocked,
        System.Collections.Generic.List<string> aliases)
    {
        foreach (var statement in body)
        {
            ScanAnnexBStatement(statement, blocked, aliases);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=643157
    // Broiler-Falsified-If: a statement that opens a lexical record forwards the enclosing blocking set unchanged
    // Broiler-Human:        PENDING
    private void ScanAnnexBStatement(
        JsStatement statement,
        System.Collections.Generic.HashSet<string> blocked,
        System.Collections.Generic.List<string> aliases)
    {
        switch (statement)
        {
            case JsBlockStatement block:
                ScanAnnexBBlock(block.Body, blocked, aliases);
                break;

            // THE CLAUSES OF A `switch` ARE ONE BLOCK, exactly as they are one record when the
            // lowering compiles them, so a declaration in one clause is blocked by a `let` in
            // another.
            case JsSwitchStatement switched:
            {
                var clauses = new System.Collections.Generic.List<JsStatement>();

                foreach (var clause in switched.Clauses)
                {
                    clauses.AddRange(clause.Body);
                }

                ScanAnnexBBlock(clauses, blocked, aliases);
                break;
            }

            case JsIfStatement conditional:
                ScanAnnexBStatement(conditional.Consequent, blocked, aliases);

                if (conditional.Alternate is not null)
                {
                    ScanAnnexBStatement(conditional.Alternate, blocked, aliases);
                }

                break;

            case JsWhileStatement loop:
                ScanAnnexBStatement(loop.Body, blocked, aliases);
                break;

            case JsDoWhileStatement loop:
                ScanAnnexBStatement(loop.Body, blocked, aliases);
                break;

            case JsForStatement loop:
            {
                var inner = blocked;

                if (loop.Initialiser is JsVariableStatement head &&
                    head.Kind != SliceDeclarationKind.Var)
                {
                    var names = new System.Collections.Generic.List<string>();

                    foreach (var declarator in head.Declarators)
                    {
                        CollectDeclaratorNames(declarator, names);
                    }

                    inner = Blocking(blocked, names);
                }

                ScanAnnexBStatement(loop.Body, inner, aliases);
                break;
            }

            case JsForInStatement loop:
                ScanAnnexBStatement(loop.Body, HeadBlocking(loop.Declaration, loop.Name, loop.Pattern, blocked), aliases);
                break;

            case JsForOfStatement loop:
                ScanAnnexBStatement(loop.Body, HeadBlocking(loop.Declaration, loop.Name, loop.Pattern, blocked), aliases);
                break;

            case JsTryStatement guarded:
            {
                ScanAnnexBBlock(guarded.Block.Body, blocked, aliases);

                if (guarded.Handler is not null)
                {
                    var inner = blocked;

                    if (guarded.CatchPattern is not null)
                    {
                        var names = new System.Collections.Generic.List<string>();
                        CollectPatternNames(guarded.CatchPattern, names);
                        inner = Blocking(blocked, names);
                    }

                    ScanAnnexBBlock(guarded.Handler.Body, inner, aliases);
                }

                if (guarded.Finaliser is not null)
                {
                    ScanAnnexBBlock(guarded.Finaliser.Body, blocked, aliases);
                }

                break;
            }

            case JsWithStatement scoped:
                ScanAnnexBStatement(scoped.Body, blocked, aliases);
                break;

            case JsLabelledStatement labelled:
                ScanAnnexBStatement(labelled.Body, blocked, aliases);
                break;

            default:
                break;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=CEAD6A
    // Broiler-Falsified-If: a block's own lexical names reach the test its own declarations are judged by
    // Broiler-Human:        PENDING
    private void ScanAnnexBBlock(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        System.Collections.Generic.HashSet<string> blocked,
        System.Collections.Generic.List<string> aliases)
    {
        foreach (var statement in body)
        {
            // ANNEX B EXTENDS `FunctionDeclaration` AND NOT THE THREE PRODUCTIONS THAT LOOK LIKE
            // IT. A generator, an async function and an async generator are separate productions of
            // the grammar, and the clause names only the first - so
            // `switch (0) { default: async function x() { } } x;` is the ReferenceError the
            // language says it is, in sloppy code as much as in strict. Admitting them here was a
            // web-compatibility extension nothing asked for, and it turned three conforming
            // refusals into programs that ran.
            if (Declared(statement) is { } declaration &&
                declaration.Function.Name.Length != 0 &&
                !declaration.Function.IsGenerator &&
                !declaration.Function.IsAsync &&
                !blocked.Contains(declaration.Function.Name))
            {
                annexB.Add(declaration);
                aliases.Add(declaration.Function.Name);
            }
        }

        var lexical = new System.Collections.Generic.List<(string Name, bool Constant)>();
        CollectLexical(body, lexical);
        var names = new System.Collections.Generic.List<string>(lexical.Count);

        foreach (var (name, _) in lexical)
        {
            names.Add(name);
        }

        ScanAnnexB(body, Blocking(blocked, names), aliases);
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

        var inner = new System.Collections.Generic.HashSet<string>(
            blocked, System.StringComparer.Ordinal);

        foreach (var name in names)
        {
            inner.Add(name);
        }

        return inner;
    }

    /// <summary>The blocking set a <c>for-in</c> or <c>for-of</c> head's own binding gives.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=13A949
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
        CollectHeadNames(name, pattern, names);
        return Blocking(blocked, names);
    }

    /// <summary>Copies a block's function binding into the <c>var</c>-scoped alias Annex B made.</summary>
    /// <remarks>
    /// <para>
    /// <b>The write cannot go through <see cref="StoreName"/>, and that is the whole difficulty of
    /// this lowering.</b> Name resolution answers with the NEAREST binding, and the nearest binding
    /// of this name is the block's own — the one being read. So the destination is named directly:
    /// the enclosing hoisting scope, reached by counting the records between it and here, or the
    /// global object where that scope is a script's.
    /// </para>
    /// <para>
    /// <b>Nothing is emitted where the alias has no destination.</b> A module is strict and never
    /// reaches here; a scope that somehow lacks the binding gets the read discarded rather than a
    /// write to whatever else answered, because a wrong write is worse than a missing one.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=84DFE0
    // Broiler-Falsified-If: the write lands on the block's own binding rather than the hoisting scope's
    // Broiler-Human:        PENDING
    private void EmitAnnexBAlias(string name)
    {
        if (!TryResolve(name, out var hops, out var slot, out _))
        {
            return;
        }

        EmitScoped(JsOpcode.LoadScoped, (byte)hops, slot);
        var target = VariableScope();

        if (target.Kind != ScopeKind.Program && target.TryGet(name, out var alias, out _))
        {
            EmitScoped(JsOpcode.StoreScoped, (byte)Hops(target), alias);
            return;
        }

        if (target.Kind == ScopeKind.Program)
        {
            Emit(JsOpcode.StoreGlobal, InternedName(name));
            return;
        }

        // A SLOPPY EVALUATION'S ALIAS IS ITS CALLER'S VARIABLE ENVIRONMENT'S, written there directly
        // and only when the evaluation's instantiation hoisted it (JSeal V15).
        if (target.Kind == ScopeKind.Eval && !strict)
        {
            EmitScoped(JsOpcode.StoreEvalVariable, (byte)Hops(target), InternedName(name));
            return;
        }

        Emit(JsOpcode.Pop);
    }

    /// <summary>Creates one record's lexical slots, all of them, before any of its code runs.</summary>
    /// <remarks>
    /// A repeated name is one slot and not two: the same name reaching here twice is either the
    /// <c>#switch</c> temporary a <c>switch</c> declares first or a duplicate the parser has
    /// already refused, and neither wants a second binding.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=907B3B
    // Broiler-Human:        PENDING
    private void DeclareLexical(
        System.Collections.Generic.IReadOnlyList<(string Name, bool Constant)> lexical)
    {
        foreach (var (name, constant) in lexical)
        {
            if (!scope.Has(name))
            {
                scope.Declare(name, constant);
            }
        }
    }

    /// <summary>
    /// Creates and initialises the function bindings one block's own statement list declares.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The block is a declaration instantiation of its own, and this is the whole of it.</b>
    /// Every function declared directly in the list gets its slot before any closure is built, so
    /// two functions in one block can call each other, and every closure is then built with the
    /// BLOCK's record current — which is the fact <see cref="CollectVarScope"/> used to destroy by
    /// hoisting the declaration out to the enclosing body <i>(JSC-138)</i>. A body that names a
    /// <c>let</c> of the same block now resolves it to a slot rather than to a global.
    /// </para>
    /// <para>
    /// <b>It runs at block ENTRY and not where the declaration stands</b>, which is what makes a
    /// call above the declaration work and what separates this from the assignment Annex B adds:
    /// the binding is complete before the first statement of the block, and the <c>var</c>-scoped
    /// alias — where there is one — is written later, by the declaration itself.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=15318F
    // Broiler-Human:        PENDING
    private void HoistBlockFunctions(System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
        System.Collections.Generic.List<JsFunctionDeclaration>? declared = null;

        foreach (var statement in body)
        {
            if (Declared(statement) is { } declaration && declaration.Function.Name.Length != 0)
            {
                (declared ??= []).Add(declaration);
            }
        }

        if (declared is null)
        {
            return;
        }

        var slots = new int[declared.Count];

        for (var index = 0; index < declared.Count; index++)
        {
            var name = declared[index].Function.Name;
            slots[index] = scope.Has(name) ? scope.SlotOf(name) : scope.Declare(name, constant: false);
        }

        for (var index = 0; index < declared.Count; index++)
        {
            Emit(JsOpcode.Closure, (ushort)CompileFunction(declared[index].Function));
            EmitScoped(JsOpcode.InitialiseScoped, 0, slots[index]);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D5AB9D
    // Broiler-Human:        PENDING
    private void CompileBlock(JsBlockStatement block, int completion)
    {
        var lexical = new System.Collections.Generic.List<(string Name, bool Constant)>();
        CollectLexical(block.Body, lexical);
        var pushed = lexical.Count != 0;
        var outer = scope;

        if (pushed)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var at = buffer.Code.Count;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((at + 1, scope));
            DeclareLexical(lexical);
            HoistBlockFunctions(block.Body);
        }

        CompileDisposing(block.Body, completion);

        if (pushed)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
        }
    }
}
