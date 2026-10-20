// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           0
// Human-reviewed:   0/9
// IP risk:          None
// Security risk:    High
// Criteria:         1/1
// Resource impact:  3/10 max
// Unverified:       9
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4700BC
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
                if (binder.AdmitsAnnexB(declared))
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=D2B45A
    // Broiler-Human:        PENDING
    private void DeclareLexical(System.Collections.Generic.IReadOnlyList<JsBinder.Declaration> lexical)
    {
        foreach (var declaration in lexical)
        {
            if (!scope.Has(declaration.Name))
            {
                scope.Declare(declaration.Name, declaration.Constant);
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=62B9A3
    // Broiler-Human:        PENDING
    private void HoistBlockFunctions(JsBinder.Declarations declarations)
    {
        var declared = declarations.Functions;

        if (declared.Count == 0)
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=825F2F
    // Broiler-Human:        PENDING
    private void CompileBlock(JsBlockStatement block, int completion)
    {
        var declarations = binder.Block(block);
        var pushed = declarations.Lexical.Count != 0;
        var outer = scope;

        if (pushed)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var at = buffer.Code.Count;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((at + 1, scope));
            DeclareLexical(declarations.Lexical);
            HoistBlockFunctions(declarations);
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
