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

using Broiler.ECMAScript.Syntax;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // ---- statements ----------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E79616
    // Broiler-Human:        PENDING
    private void CompileStatements(
        System.Collections.Generic.IReadOnlyList<Statement> body, int completion, int first = 0)
    {
        for (var at = first; at < body.Count; at++)
        {
            CompileStatement(body[at], completion);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AE49FD
    // Broiler-Human:        PENDING
    private void CompileStatement(Statement statement, int completion)
    {
        Position(JsTreeFacts.At(statement));

        switch (statement)
        {
            case EmptyStatement:
            case DebuggerStatement:
                break;

            case ExpressionStatement expression:
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

            case VariableDeclaration variable:
                CompileVariable(variable);
                break;

            // A FUNCTION DECLARATION EMITS NOTHING WHERE IT STANDS, because everything it does was
            // done when the scope holding it was entered - except the one write Annex B adds, and
            // that write is here BECAUSE it belongs where the declaration stands. That is what
            // makes `if (x) { function f() { } }` leave `f` undefined when `x` is false, and what
            // makes the aliased value the block binding's CURRENT one rather than its first.
            case FunctionDeclaration declared:
                if (binder.AdmitsAnnexB(declared))
                {
                    EmitAnnexBAlias(NameOf(declared.Function));
                }

                break;

            case ClassDeclaration declaration:
                CompileClassDeclaration(declaration);
                break;

            case BlockStatement block:
                CompileBlock(block, block.Body, completion);
                break;

            case WithStatement scoped:
                CompileWith(scoped, completion);
                break;

            case IfStatement conditional:
                CompileIf(conditional, completion);
                break;

            case WhileStatement loop:
                CompileWhile(loop, completion, string.Empty);
                break;

            case DoWhileStatement loop:
                CompileDoWhile(loop, completion, string.Empty);
                break;

            case ForStatement loop:
                CompileFor(loop, completion, string.Empty);
                break;

            case ForInStatement loop:
                CompileForIn(loop, completion, string.Empty);
                break;

            case ForOfStatement loop:
                CompileForOf(loop, completion, string.Empty);
                break;

            case BreakStatement jump:
                CompileJumpOut(JsTreeFacts.At(jump), jump.Label?.Name ?? string.Empty, wantsContinue: false);
                break;

            case ContinueStatement jump:
                CompileJumpOut(JsTreeFacts.At(jump), jump.Label?.Name ?? string.Empty, wantsContinue: true);
                break;

            case ReturnStatement returned:
                CompileReturn(JsTreeFacts.At(returned), returned.Argument);
                break;

            case ThrowStatement thrown:
                CompileExpression(thrown.Argument);
                Emit(JsOpcode.Throw);
                break;

            case TryStatement guarded:
                CompileTry(guarded, completion);
                break;

            case SwitchStatement switched:
                CompileSwitch(switched, completion, string.Empty);
                break;

            case LabelledStatement labelled:
                CompileLabelled(labelled, completion);
                break;

            default:
                Refuse(
                    JsTreeFacts.At(statement),
                    SliceSourceDiagnosticCode.ConstructTheLoweringHasNoArmFor,
                    "this statement is not admitted by the declared feature manifest");

                break;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9C1DD3
    // Broiler-Human:        PENDING
    private void CompileLabelled(LabelledStatement labelled, int completion)
    {
        var label = labelled.Label.Name;

        switch (labelled.Body)
        {
            case WhileStatement loop:
                CompileWhile(loop, completion, label);
                return;

            case DoWhileStatement loop:
                CompileDoWhile(loop, completion, label);
                return;

            case ForStatement loop:
                CompileFor(loop, completion, label);
                return;

            case ForInStatement loop:
                CompileForIn(loop, completion, label);
                return;

            case ForOfStatement loop:
                CompileForOf(loop, completion, label);
                return;

            case SwitchStatement switched:
                CompileSwitch(switched, completion, label);
                return;

            default:
            {
                var exit = new Exit(ExitKind.Label, label, blockDepth)
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4D6F56
    // Broiler-Human:        PENDING
    private void CompileVariable(VariableDeclaration variable)
    {
        var (kind, disposal) = (JsTreeFacts.Kind(variable.DeclarationKind).Kind, JsTreeFacts.Disposal(variable.DeclarationKind));

        foreach (var declarator in variable.Declarations)
        {
            var span = JsTreeFacts.At(declarator);
            var name = declarator.Target is BindingIdentifier binding ? binding.Name : string.Empty;
            var initialiser = declarator.Initializer;

            if (declarator.Target is not BindingIdentifier)
            {
                // A DESTRUCTURING DECLARATION ALWAYS HAS AN INITIALISER, whatever its keyword.
                // `var a;` is fine and `var [a];` is not, because there is nothing for the pattern
                // to take apart - and the grammar says so for `var` as loudly as for `const`.
                if (initialiser is null)
                {
                    throw BinderAdmitted(span, "a destructuring declaration with no initialiser");
                }

                if (kind != SliceDeclarationKind.Var)
                {
                    DeclarePatternNames(
                        declarator.Target, kind == SliceDeclarationKind.Const);
                }

                CompileExpression(initialiser);

                BindPattern(
                    declarator.Target,
                    kind == SliceDeclarationKind.Var ? BindMode.Var : BindMode.Initialise);

                continue;
            }

            if (kind == SliceDeclarationKind.Const && initialiser is null)
            {
                throw BinderAdmitted(span, "a `const` with no initialiser");
            }

            // A RESOURCE IS REGISTERED BEFORE ITS BINDING LEAVES THE DEAD ZONE, which is the order
            // `InitializeBinding` gives the two: a value with no disposer is a `TypeError` raised
            // while the name is still unreadable, and the value stays on the stack across the
            // registration so the binding is written from the same one.
            if (disposal != JsDisposal.None)
            {
                var resource = scope.Has(name)
                    ? scope.SlotOf(name)
                    : scope.Declare(name, constant: true);

                CompileNamedValue(initialiser!, name);
                EmitRegistration(span, disposal);
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
                if (programLexicals.ContainsKey(name))
                {
                    if (initialiser is null)
                    {
                        Emit(JsOpcode.LoadUndefined);
                    }
                    else
                    {
                        CompileNamedValue(initialiser, name);
                    }

                    Emit(JsOpcode.InitialiseGlobalLexical, InternedName(name));
                    continue;
                }

                if (initialiser is null)
                {
                    continue;
                }

                CompileNamedValue(initialiser, name);
                Emit(JsOpcode.StoreGlobal, InternedName(name));
                continue;
            }

            // A `var` NAMES A BINDING THAT ALREADY EXISTS. Hoisting created it in the
            // enclosing function or program scope, so a declaration inside a block writes THAT
            // one; declaring it again here would create a second binding the rest of the function
            // cannot see.
            if (kind == SliceDeclarationKind.Var)
            {
                if (initialiser is null)
                {
                    continue;
                }

                // IN A `with` BODY THE NAME IS RESOLVED BEFORE ITS INITIALISER RUNS, as an
                // assignment's is (14.3.2.1: ResolveBinding, then the initialiser, then PutValue),
                // so an initialiser that deletes the object's property still writes the object.
                // Until 2026-10-03 the name was resolved at the write (JSC-254).
                var initialised = name;

                if (TryEmitShadowedReference(
                    span, initialised, read: false, () => CompileNamedValue(initialiser, initialised)))
                {
                    Emit(JsOpcode.Pop);
                    continue;
                }

                CompileNamedValue(initialiser, name);
                StoreName(span, name);
                Emit(JsOpcode.Pop);
                continue;
            }

            var slot = scope.Has(name)
                ? scope.SlotOf(name)
                : scope.Declare(name, kind == SliceDeclarationKind.Const);

            if (initialiser is null)
            {
                Emit(JsOpcode.LoadUndefined);
            }
            else
            {
                CompileNamedValue(initialiser, name);
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
    /// BLOCK's record current — which is the fact the <c>var</c> collection used to destroy by
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=03C424
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
            var name = NameOf(declared[index].Function);
            slots[index] = scope.Has(name) ? scope.SlotOf(name) : scope.Declare(name, constant: false);
        }

        for (var index = 0; index < declared.Count; index++)
        {
            Emit(JsOpcode.Closure, (ushort)CompileFunction(declared[index].Function, JsTreeFacts.At(declared[index]), NameOf(declared[index].Function)));
            EmitScoped(JsOpcode.InitialiseScoped, 0, slots[index]);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=25CF5F
    // Broiler-Human:        PENDING
    private void CompileBlock(SyntaxNode block, System.Collections.Generic.IReadOnlyList<Statement> body, int completion)
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

        CompileDisposing(body, completion);

        if (pushed)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
        }
    }
}
