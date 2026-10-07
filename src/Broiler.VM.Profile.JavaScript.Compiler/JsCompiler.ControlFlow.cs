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
// Security risk:    Medium
// Criteria:         0/0
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

    /// <summary>Puts the script's completion value back to <c>undefined</c> before a statement.</summary>
    /// <remarks>
    /// <para>
    /// <b>Most statements do not simply LEAVE the completion value alone, and this is the whole of
    /// the difference.</b> The language's <c>UpdateEmpty(C, undefined)</c> appears on <c>if</c>,
    /// on every iteration statement, on <c>with</c>, on <c>try</c> and on <c>switch</c>: each of
    /// them answers with its own body's value, and with <c>undefined</c> — not with the value of
    /// whatever ran before it — when its body produced none. So <c>1; if (true) { }</c> is
    /// <c>undefined</c> and not <c>1</c>, <c>1; while (false) { }</c> is <c>undefined</c>, and
    /// <c>1; try { } finally { }</c> is <c>undefined</c>.
    /// </para>
    /// <para>
    /// <b>A BLOCK IS THE EXCEPTION, and it is the one every list of these gets wrong.</b>
    /// <c>Block : { }</c> answers <c>empty</c> and a statement list carries the value forward
    /// across an empty one, so <c>1; { }</c> is <c>1</c> — which is why the reset is written at the
    /// eight statements that own it rather than once in <see cref="CompileStatements"/>.
    /// </para>
    /// <para>
    /// <b>It is emitted once, before the statement, rather than tested afterwards.</b> A slot the
    /// body overwrites when it produces a value and leaves alone when it does not IS
    /// <c>UpdateEmpty</c>, so the whole rule costs two instructions at the head of the statement
    /// and nothing per iteration — and a <c>break</c> out of the middle of a loop body needs no
    /// arm of its own, because the slot already holds what the language says it holds.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1CB428
    // Broiler-Human:        PENDING
    private void ResetCompletion(int completion)
    {
        if (completion < 0)
        {
            return;
        }

        Emit(JsOpcode.LoadUndefined);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, completion);
    }

    /// <summary>Lowers <c>with</c>: one object environment record around one statement.</summary>
    /// <remarks>
    /// <para>
    /// <b>It is a block whose record holds an object, and every exit is the exits a block already
    /// has.</b> The depth counter rises with the record and falls with it, so
    /// <see cref="Unwrap"/> — which is what <c>break</c>, <c>continue</c> and <c>return</c> unwind
    /// through — discards it without knowing what kind of record it is, and an exception region
    /// opened inside the body records the depth WITH it and is truncated back to the same figure by
    /// the executor. Nothing about the object record needed its own unwinding path, which is the
    /// whole reason it is a record on the ordinary chain rather than a second one.
    /// </para>
    /// <para>
    /// <b>The scope it pushes declares nothing and cannot.</b> A <c>var</c> inside the body was
    /// hoisted to the enclosing function or to the global object before this ran, and a lexical
    /// declaration cannot be a <c>with</c> body at all — the parser refuses that as the syntax error
    /// the language calls it. So a name declared inside a <c>with</c> body is a name declared in the
    /// block inside it, which pushes a record of its own.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D51887
    // Broiler-Human:        PENDING
    private void CompileWith(JsWithStatement statement, int completion)
    {
        ResetCompletion(completion);
        CompileExpression(statement.Object);
        Emit(JsOpcode.PushObjectScope);

        var outer = scope;
        scope = new Scope(ScopeKind.With, outer);
        blockDepth++;

        CompileStatement(statement.Body, completion);

        Emit(JsOpcode.PopScope);
        blockDepth--;
        scope = outer;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=96D04B
    // Broiler-Human:        PENDING
    private void CompileIf(JsIfStatement conditional, int completion)
    {
        ResetCompletion(completion);
        CompileExpression(conditional.Test);
        var otherwise = NewLabel();
        Branch(JsOpcode.JumpIfFalse, otherwise);
        CompileStatement(conditional.Consequent, completion);

        if (conditional.Alternate is null)
        {
            Mark(otherwise);
            return;
        }

        var end = NewLabel();
        Branch(JsOpcode.Jump, end);
        Mark(otherwise);
        CompileStatement(conditional.Alternate, completion);
        Mark(end);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=30D799
    // Broiler-Human:        PENDING
    private void CompileWhile(JsWhileStatement loop, int completion, string label)
    {
        ResetCompletion(completion);
        var top = NewLabel();
        var exit = new Exit(ExitKind.Loop, label, blockDepth)
        {
            Break = NewLabel(),
            Continue = top,
        };

        Mark(top);
        CompileExpression(loop.Test);
        Branch(JsOpcode.JumpIfFalse, exit.Break!);
        exits.Add(exit);
        CompileStatement(loop.Body, completion);
        exits.RemoveAt(exits.Count - 1);
        Branch(JsOpcode.Jump, top);
        Mark(exit.Break!);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B76E55
    // Broiler-Human:        PENDING
    private void CompileDoWhile(JsDoWhileStatement loop, int completion, string label)
    {
        ResetCompletion(completion);
        var top = NewLabel();
        var exit = new Exit(ExitKind.Loop, label, blockDepth)
        {
            Break = NewLabel(),
            Continue = NewLabel(),
        };

        Mark(top);
        exits.Add(exit);
        CompileStatement(loop.Body, completion);
        exits.RemoveAt(exits.Count - 1);
        Mark(exit.Continue!);
        CompileExpression(loop.Test);
        Branch(JsOpcode.JumpIfTrue, top);
        Mark(exit.Break!);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5B4B2C
    // Broiler-Human:        PENDING
    private void CompileFor(JsForStatement loop, int completion, string label)
    {
        ResetCompletion(completion);
        var lexical = new System.Collections.Generic.List<string>();

        if (loop.Initialiser is JsVariableStatement head && head.Kind != SliceDeclarationKind.Var)
        {
            foreach (var declarator in head.Declarators)
            {
                CollectDeclaratorNames(declarator, lexical);
            }
        }

        var outer = scope;
        var pushed = lexical.Count != 0;

        if (pushed)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var headSite = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((headSite, scope));
        }

        // A RESOURCE DECLARED BY THE HEAD BELONGS TO THE WHOLE LOOP, not to one turn of it: the
        // head's bindings are constant, so the per-turn copies below share the one scope value, and
        // the scope is disposed once, after the loop - by a `break` falling out of it, by the test
        // failing, by a `return`, or by the handler when anything in the head or the loop throws.
        // A `continue` stays inside it and disposes nothing.
        var headDisposal = loop.Initialiser is JsVariableStatement { Using: not JsUsing.None } resources
            ? BeginDisposal(resources.Span, resources.Using == JsUsing.Async)
            : null;

        if (loop.Initialiser is JsVariableStatement declaration)
        {
            CompileVariable(declaration);
        }
        else if (loop.Initialiser is JsExpressionStatement expression)
        {
            CompileExpression(expression.Expression);
            Emit(JsOpcode.Pop);
        }

        // EACH TURN OF THE LOOP GETS ITS OWN BINDING, and WHERE the copy is made is the whole of
        // whether that works. The specification copies the environment once before the loop and
        // then again after each body and BEFORE the increment: a closure the body created keeps the
        // value the body saw, and the increment lands in the copy the next turn will use. Copying
        // before the body instead - the obvious placement - makes every closure see the value after
        // its own increment, which is the classic `for (let i …)` defect with the sign flipped.
        if (pushed)
        {
            var firstSite = buffer.Code.Count + 1;
            Emit(JsOpcode.CopyScope, (ushort)0);
            buffer.ScopeSites.Add((firstSite, scope));
        }

        var top = NewLabel();
        var exit = new Exit(ExitKind.Loop, label, blockDepth)
        {
            Break = NewLabel(),
            Continue = NewLabel(),
        };

        Mark(top);

        if (loop.Test is not null)
        {
            CompileExpression(loop.Test);
            Branch(JsOpcode.JumpIfFalse, exit.Break!);
        }

        exits.Add(exit);
        CompileStatement(loop.Body, completion);
        exits.RemoveAt(exits.Count - 1);
        Mark(exit.Continue!);

        if (pushed)
        {
            var copySite = buffer.Code.Count + 1;
            Emit(JsOpcode.CopyScope, (ushort)0);
            buffer.ScopeSites.Add((copySite, scope));
        }

        if (loop.Update is not null)
        {
            CompileExpression(loop.Update);
            Emit(JsOpcode.Pop);
        }

        Branch(JsOpcode.Jump, top);
        Mark(exit.Break!);

        if (headDisposal is not null)
        {
            EndDisposal(headDisposal);
        }

        if (pushed)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4DBD70
    // Broiler-Human:        PENDING
    private void CompileForIn(JsForInStatement loop, int completion, string label)
    {
        ResetCompletion(completion);

        // THE ENUMERATOR LIVES IN A SLOT, NOT ON THE OPERAND STACK. Every abrupt exit from the body
        // - `break`, `continue` to an outer loop, `return` - would otherwise have to know how many
        // operand-stack entries the enclosing loops are holding and pop exactly that many. One slot
        // costs an environment entry and removes the whole class of defect.
        var function = FunctionScope();
        var enumerator = function.Declare("#forin" + function.SlotCount, constant: false);

        // ANNEX B'S INITIALISER IS EVALUATED AND STORED ONCE, BEFORE THE OBJECT IS (JSP-7, JSC-239):
        // `for (var a = 0 in stored = a, o)` reads `a` as 0, and an anonymous function takes the
        // name. The parser admitted the form and this lowering dropped the value.
        if (loop.Initialiser is { } initialiser)
        {
            CompileNamedValue(initialiser, loop.Name);
            StoreName(loop.Span, loop.Name);
            Emit(JsOpcode.Pop);
        }

        var outer = scope;
        var pushed = loop.Declaration is SliceDeclarationKind.Let or SliceDeclarationKind.Const;
        var constant = loop.Declaration == SliceDeclarationKind.Const;

        // THE HEAD'S NAMES ARE IN THEIR DEAD ZONE WHILE THE OBJECT IS EVALUATED, as `for … of`'s
        // are: `for (let x in { x })` is a ReferenceError, not a read of the outer `x`. Until
        // 2026-10-03 the object was evaluated in the enclosing scope (JSC-246).
        if (pushed)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var headSite = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((headSite, scope));
            DeclareHead(loop.Name, loop.Pattern, constant);
        }

        CompileExpression(loop.Right);
        Emit(JsOpcode.ForInStart);

        if (pushed)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
        }

        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, enumerator);

        var loopScope = scope;

        if (pushed)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var bodySite = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((bodySite, scope));
            DeclareHead(loop.Name, loop.Pattern, constant);
            loopScope = scope;
        }

        var top = NewLabel();
        var exit = new Exit(ExitKind.Loop, label, blockDepth)
        {
            Break = NewLabel(),
            Continue = NewLabel(),
        };

        Mark(top);
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, enumerator);
        Branch(JsOpcode.ForInNext, exit.Break!);

        // EACH TURN BINDS A FRESH COPY, which is what a closure in the body captures: without it
        // every closure the loop made saw the last key. `for … of` has copied since it was
        // written; `for … in` did not until 2026-10-03 (JSC-246).
        if (pushed)
        {
            var copySite = buffer.Code.Count + 1;
            Emit(JsOpcode.CopyScope, (ushort)0);
            buffer.ScopeSites.Add((copySite, loopScope));
        }

        // A `for … in` HEAD MAY DESTRUCTURE THE KEY, which reads oddly and is ordinary grammar:
        // the value bound each turn is a String, so `for (const [a, b] in o)` takes its first two
        // characters apart. The lowering is the same one every other position uses.
        if (loop.Pattern is not null)
        {
            BindPattern(
                loop.Pattern,
                pushed ? BindMode.Initialise
                    : loop.Declaration == SliceDeclarationKind.Var ? BindMode.Var : BindMode.Assign);
        }
        else if (pushed)
        {
            EmitScoped(JsOpcode.InitialiseScoped, 0, scope.SlotOf(loop.Name));
        }
        else if (loop.Declaration == SliceDeclarationKind.Var)
        {
            StoreName(loop.Span, loop.Name);
            Emit(JsOpcode.Pop);
        }
        else if (loop.Target is not null)
        {
            CompileStoreTo(loop.Target);
            Emit(JsOpcode.Pop);
        }
        else
        {
            Emit(JsOpcode.Pop);
        }

        exits.Add(exit);
        CompileStatement(loop.Body, completion);
        exits.RemoveAt(exits.Count - 1);
        Mark(exit.Continue!);
        Branch(JsOpcode.Jump, top);
        Mark(exit.Break!);

        if (pushed)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
        }
    }

    /// <summary>
    /// Lowers <c>for … of</c>: the iteration protocol, and the <c>IteratorClose</c> every way out
    /// of it owes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The head's bindings exist, uninitialised, while the source expression is evaluated.</b>
    /// That is what makes <c>for (const x of x)</c> a <c>ReferenceError</c> rather than a read of
    /// whatever <c>x</c> meant outside, and it is why the head scope is pushed before the right-hand
    /// side and popped straight after it. The loop then pushes a SECOND scope, because the head's
    /// is gone by the time the first turn starts.
    /// </para>
    /// <para>
    /// <b>Every abrupt exit closes the iterator, and there are four of them.</b> Running out is the
    /// one that does not - the iterator said it was finished. <c>break</c> and a labelled
    /// <c>break</c> close it in <see cref="CompileJumpOut"/>, <c>return</c> closes it in
    /// <see cref="CompileReturn"/>, and a <c>throw</c> from the body reaches the handler this
    /// method installs. <c>continue</c> is the one that must NOT close it, which is why the exit
    /// record carries the slot rather than the loop emitting a close at its own bottom. A
    /// generator's forced return at a <c>yield</c> in the body is a fifth, and a synchronous loop
    /// closes for it from a second, finally-kind handler.
    /// </para>
    /// <para>
    /// <b>The per-iteration copy is what a closure in the body captures.</b> Without it every
    /// closure a loop created would share one binding and see the last value, which is the most
    /// reproduced defect in the language.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D778A7
    // Broiler-Human:        PENDING
    private void CompileForOf(JsForOfStatement loop, int completion, string label)
    {
        ResetCompletion(completion);
        var lexical = loop.Declaration is SliceDeclarationKind.Let or SliceDeclarationKind.Const;
        var constant = loop.Declaration == SliceDeclarationKind.Const;
        var outerDepth = blockDepth;
        var outer = scope;

        if (lexical)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var headSite = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((headSite, scope));
            DeclareHead(loop.Name, loop.Pattern, constant);
        }

        CompileExpression(loop.Right);

        // `for await` READS `Symbol.asyncIterator` AND FALLS BACK, and the falling back is the
        // operation rather than a courtesy: an Array has no `Symbol.asyncIterator`, so a loop that
        // refused anything without one would refuse `for await (const x of [p, q])`, which is the
        // case the statement exists for. What the fall-back builds awaits each VALUE, which is the
        // whole difference between iterating promises and iterating what they resolve to.
        Emit(loop.IsAwait ? JsOpcode.IterateStartAsync : JsOpcode.IterateStart);

        if (lexical)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
        }

        // The record lives in a slot of the function's own environment for the same reason the
        // `for … in` enumerator does: a `break` out of the body would otherwise have to know how
        // many operand-stack entries every enclosing loop is holding.
        var owner = FunctionScope();
        var record = owner.Declare("#forof" + owner.SlotCount, constant: false);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, record);

        var loopScope = scope;

        if (lexical)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var bodySite = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((bodySite, scope));
            DeclareHead(loop.Name, loop.Pattern, constant);
            loopScope = scope;
        }

        var guarded = buffer.Code.Count;
        var unwind = NewLabel();
        var level = ++regionLevel;

        // A GENERATOR'S FORCED RETURN CLOSES THE ITERATOR TOO, and passes every catch region over,
        // so the loop also records a finally region - second, so that a throw still finds the
        // catch region first, exactly as an array pattern records its two. `for await` records one
        // as well: its close suspends, so its handler parks the forced return in a slot across the
        // await, as a `finally` block's handler parks what it rethrows.
        var forced = NewLabel();

        buffer.PendingRegions.Add(
            new PendingRegion(guarded, forced, outerDepth, JsFormat.HandlerKind.Finally, Level: level));

        buffer.PendingRegions.Add(
            new PendingRegion(guarded, unwind, outerDepth, JsFormat.HandlerKind.Catch, Level: level));

        var top = NewLabel();
        var exit = new Exit(ExitKind.Loop, label, blockDepth)
        {
            Level = level,
            Break = NewLabel(),
            Continue = NewLabel(),
            IteratorSlot = record,
            IteratorIsAsync = loop.IsAwait,
        };

        Mark(top);
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);

        // THE STEP IS THREE INSTRUCTIONS WHERE THE SYNCHRONOUS ONE IS ONE, and the middle one is a
        // suspension. `next` is called, what it answered is awaited, and only then is the answer
        // asked whether it is done - so the record stays on the stack under the awaited value and
        // the pair is consumed together. Folding the three into one instruction would have needed
        // an instruction that suspends in the middle of itself.
        if (loop.IsAwait)
        {
            Emit(JsOpcode.IterateNextAsync);
            Emit(JsOpcode.Await);
            Branch(JsOpcode.IterateAwaitStep, exit.Break!);
        }
        else
        {
            Branch(JsOpcode.IterateNext, exit.Break!);
        }

        if (lexical)
        {
            var copySite = buffer.Code.Count + 1;
            Emit(JsOpcode.CopyScope, (ushort)0);
            buffer.ScopeSites.Add((copySite, loopScope));
        }

        // EACH TURN'S RESOURCE IS ITS OWN, disposed when the turn ends and before the iterator is
        // asked for the next value - or closed, on a `break` or a `return`, which unwind the turn's
        // scope first because it is the inner of the two exits. The scope value is made in the
        // turn's fresh copy of the body record, so no turn can reach another's.
        Exit? turnDisposal = null;

        if (loop.Using != JsUsing.None)
        {
            exits.Add(exit);
            turnDisposal = BeginDisposal(loop.Span, loop.Using == JsUsing.Async);
            EmitRegistration(loop.Span, loop.Using);
            EmitScoped(JsOpcode.InitialiseScoped, 0, scope.SlotOf(loop.Name));
        }
        else if (loop.Pattern is not null)
        {
            BindPattern(
                loop.Pattern,
                lexical ? BindMode.Initialise
                    : loop.Declaration == SliceDeclarationKind.Var ? BindMode.Var : BindMode.Assign);
        }
        else if (lexical)
        {
            EmitScoped(JsOpcode.InitialiseScoped, 0, scope.SlotOf(loop.Name));
        }
        else if (loop.Declaration == SliceDeclarationKind.Var)
        {
            StoreName(loop.Span, loop.Name);
            Emit(JsOpcode.Pop);
        }
        else if (loop.Target is not null)
        {
            CompileStoreTo(loop.Target);
            Emit(JsOpcode.Pop);
        }
        else
        {
            Emit(JsOpcode.Pop);
        }

        if (turnDisposal is null)
        {
            exits.Add(exit);
        }

        CompileStatement(loop.Body, completion);

        // THE TURN'S HANDLER IS EMITTED INSIDE THE LOOP'S OWN REGION, so what it re-raises reaches
        // the loop's handler and closes the iterator, which is the order the specification gives
        // the two: dispose the turn, then close under the completion that disposal produced.
        if (turnDisposal is not null)
        {
            EndDisposal(turnDisposal);
        }

        exits.RemoveAt(exits.Count - 1);
        Mark(exit.Continue!);
        Branch(JsOpcode.Jump, top);
        Mark(exit.Break!);
        buffer.CloseRegion(guarded, buffer.Code.Count);
        buffer.CloseRegion(guarded, buffer.Code.Count);
        regionLevel--;

        if (lexical)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
        }

        var after = NewLabel();
        Branch(JsOpcode.Jump, after);

        // THE HANDLER IS ENTERED WITH THE THROWN VALUE AND NOTHING ELSE, at the depth outside the
        // loop's own scope, because the executor trims the scope chain to the region's declared
        // depth before it lands here. It closes quietly and rethrows: an error the iterator's
        // `return` raises must not replace the one already travelling.
        Mark(unwind);

        if (loop.IsAwait)
        {
            CompileAsyncUnwind(record, outerDepth);
        }
        else
        {
            EmitScoped(JsOpcode.LoadScoped, (byte)outerDepth, record);
            Emit(JsOpcode.IterateClose, (byte)1);
            Emit(JsOpcode.Throw);
        }

        // THE FORCED RETURN CLOSES LOUDLY, because it is a return completion and not a throw: an
        // error the iterator's `return` raises replaces it, and `Throw` re-raises the parked return
        // when there is none.
        Mark(forced);
        buffer.Rejoin(1);

        if (loop.IsAwait)
        {
            CompileAsyncForcedClose(record, outerDepth);
        }
        else
        {
            EmitScoped(JsOpcode.LoadScoped, (byte)outerDepth, record);
            Emit(JsOpcode.IterateClose, (byte)0);
        }

        Emit(JsOpcode.Throw);
        Mark(after);
    }

    /// <summary>
    /// The <c>for await</c> handler: park the exception, close asynchronously, re-raise it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The exception is parked in a slot because the close SUSPENDS, and a value the operand
    /// stack holds is not what a re-entry finds.</b> The synchronous handler keeps the thrown value
    /// under the record and throws it three instructions later; here the close awaits, so the frame
    /// leaves and comes back at a height the handler no longer controls. A slot of the function's
    /// own environment is where every other value that has to outlive a jump already lives.
    /// </para>
    /// <para>
    /// <b>The close is wrapped in a region that SWALLOWS, and the swallowing is the specification's
    /// own.</b> <c>AsyncIteratorClose</c> under a throw completion reads <c>return</c>, calls it,
    /// awaits what it answered - and then discards every failure of those three, because the
    /// exception already travelling is the one the program is owed. Emitting a region rather than a
    /// quiet variant of <c>Await</c> is what keeps the suspension one opcode: the swallowing is
    /// control flow, and this format already expresses control flow.
    /// </para>
    /// <para>
    /// <b>The value check is NOT emitted here.</b> A <c>return</c> answering a primitive is a
    /// <c>TypeError</c> under a normal completion and is discarded under this one, which is why
    /// <c>IterateCloseCheck</c> appears on the <c>break</c> and <c>return</c> paths and not on this
    /// one.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2D2442
    // Broiler-Human:        PENDING
    private void CompileAsyncUnwind(int record, int outerDepth)
    {
        var owner = FunctionScope();
        var parked = owner.Declare("#forawait" + owner.SlotCount, constant: false);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)outerDepth, parked);

        var guarded = buffer.Code.Count;
        var swallow = NewLabel();
        var closed = NewLabel();
        var rethrow = NewLabel();

        buffer.PendingRegions.Add(
            new PendingRegion(guarded, swallow, outerDepth, JsFormat.HandlerKind.Catch));

        EmitScoped(JsOpcode.LoadScoped, (byte)outerDepth, record);
        Branch(JsOpcode.IterateCloseAsync, closed);
        Emit(JsOpcode.Await);
        Emit(JsOpcode.Pop);
        Mark(closed);
        buffer.CloseRegion(guarded, buffer.Code.Count);
        Branch(JsOpcode.Jump, rethrow);

        // THE HANDLER IS ENTERED WITH THE SWALLOWED VALUE AND NOTHING ELSE, at the height every
        // handler of this format is entered at, and it discards it. What is re-raised below is the
        // value that was parked, which is the exception the loop body actually threw.
        Mark(swallow);
        buffer.Rejoin(1);
        Emit(JsOpcode.Pop);
        Mark(rethrow);
        EmitScoped(JsOpcode.LoadScoped, (byte)outerDepth, parked);
        Emit(JsOpcode.Throw);
    }

    /// <summary>
    /// The <c>for await</c> handler a generator's forced return lands in: park the return, close
    /// asynchronously, leave the return on the stack for the caller's <c>Throw</c> to re-raise.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The return is parked in a slot because the close SUSPENDS</b>, for the reason
    /// <see cref="CompileAsyncUnwind"/> gives: a value the operand stack holds is not what the
    /// re-entry after the await finds. <c>try</c>'s <c>finally</c> handler parks what it rethrows
    /// the same way, which is why an <c>await</c> in a <c>finally</c> already survived a forced
    /// return and this loop did not.
    /// </para>
    /// <para>
    /// <b>The close is the loud one</b>: a return completion is not a throw, so
    /// <c>AsyncIteratorClose</c> lets a failure of <c>return</c>, of its await, or a primitive
    /// answer replace it - exactly the <c>break</c> path's <see cref="CompileAsyncClose"/>, with no
    /// swallowing region around it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2E93F5
    // Broiler-Human:        PENDING
    private void CompileAsyncForcedClose(int record, int outerDepth)
    {
        var owner = FunctionScope();
        var parked = owner.Declare("#forawait" + owner.SlotCount, constant: false);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)outerDepth, parked);

        var closed = NewLabel();
        EmitScoped(JsOpcode.LoadScoped, (byte)outerDepth, record);
        Branch(JsOpcode.IterateCloseAsync, closed);
        Emit(JsOpcode.Await);
        Emit(JsOpcode.IterateCloseCheck);
        Mark(closed);
        EmitScoped(JsOpcode.LoadScoped, (byte)outerDepth, parked);
    }

    /// <summary>
    /// <c>AsyncIteratorClose</c> under a normal or <c>break</c>-shaped completion, inline.
    /// </summary>
    /// <remarks>
    /// <b>Every failure propagates here, where the handler above discards them all.</b> That is the
    /// one difference between the two closes and it is the specification's: a <c>break</c> out of a
    /// <c>for await</c> whose iterator's <c>return</c> rejects rejects the enclosing async
    /// function, and a <c>throw</c> out of the same loop does not.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=36BF98
    // Broiler-Human:        PENDING
    private void CompileAsyncClose(int record)
    {
        var closed = NewLabel();
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
        Branch(JsOpcode.IterateCloseAsync, closed);
        Emit(JsOpcode.Await);
        Emit(JsOpcode.IterateCloseCheck);
        Mark(closed);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E0BD37
    // Broiler-Human:        PENDING
    private void DeclareHead(string name, JsPattern? pattern, bool constant)
    {
        if (pattern is null)
        {
            scope.Declare(name, constant);
            return;
        }

        DeclarePatternNames(pattern, constant);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E214E8
    // Broiler-Human:        PENDING
    private void CompileSwitch(JsSwitchStatement switched, int completion, string label)
    {
        ResetCompletion(completion);
        var outer = scope;
        scope = new Scope(ScopeKind.Block, outer);
        blockDepth++;
        var scopeSite = buffer.Code.Count + 1;
        Emit(JsOpcode.PushScope, (ushort)0);
        buffer.ScopeSites.Add((scopeSite, scope));

        var discriminant = scope.Declare("#switch", constant: false);
        CompileExpression(switched.Discriminant);
        EmitScoped(JsOpcode.InitialiseScoped, 0, discriminant);

        // THE CLAUSES SHARE ONE RECORD AND ONE DECLARATION INSTANTIATION. A function declared in
        // any clause is declared for the whole `switch`, and it is declared before the first
        // clause's own test expression runs - which is what makes
        // `switch (0) { case f(): break; default: function f() { } }` reach the declaration rather
        // than a global. So the clauses are ONE list here and not one hoisting each: hoisting them
        // separately would build the first clause's closure before the second clause's slot
        // existed, and a call across the two would resolve to a global again.
        var clauses = new System.Collections.Generic.List<JsStatement>();

        foreach (var clause in switched.Clauses)
        {
            clauses.AddRange(clause.Body);
        }

        var lexical = new System.Collections.Generic.List<(string Name, bool Constant)>();
        CollectLexical(clauses, lexical);
        DeclareLexical(lexical);
        HoistBlockFunctions(clauses);

        var exit = new Exit(ExitKind.Switch, label, blockDepth)
        {
            Break = NewLabel(),
        };

        var bodies = new Label[switched.Clauses.Count];

        for (var index = 0; index < bodies.Length; index++)
        {
            bodies[index] = NewLabel();
        }

        var defaultAt = -1;

        for (var index = 0; index < switched.Clauses.Count; index++)
        {
            var clause = switched.Clauses[index];

            if (clause.Test is null)
            {
                defaultAt = index;
                continue;
            }

            EmitScoped(JsOpcode.LoadScoped, 0, discriminant);
            CompileExpression(clause.Test);
            Emit(JsOpcode.StrictEquals);
            Branch(JsOpcode.JumpIfTrue, bodies[index]);
        }

        Branch(JsOpcode.Jump, defaultAt >= 0 ? bodies[defaultAt] : exit.Break!);
        exits.Add(exit);

        for (var index = 0; index < switched.Clauses.Count; index++)
        {
            Mark(bodies[index]);
            CompileStatements(switched.Clauses[index].Body, completion);
        }

        exits.RemoveAt(exits.Count - 1);
        Branch(JsOpcode.Jump, exit.Break!);
        Mark(exit.Break!);
        Emit(JsOpcode.PopScope);
        blockDepth--;
        scope = outer;
    }
}
