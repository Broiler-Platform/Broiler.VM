// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           0
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  3/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F0F1A5
    // Broiler-Human:        PENDING
    private void CompileTry(JsTryStatement guarded, int completion)
    {
        ResetCompletion(completion);
        var end = NewLabel();
        var hasFinally = guarded.Finaliser is not null;
        var tryStart = buffer.Code.Count;
        var level = ++regionLevel;

        if (hasFinally)
        {
            exits.Add(new Exit(ExitKind.Finally, string.Empty, blockDepth)
            {
                Finaliser = guarded.Finaliser,
                Guarded = tryStart,
                Level = level,
            });
        }

        if (guarded.Handler is not null)
        {
            var catchHandler = NewLabel();
            var afterCatch = NewLabel();
            buffer.PendingRegions.Add(
                new PendingRegion(
                    tryStart, catchHandler, blockDepth, JsFormat.HandlerKind.Catch, Level: level));

            CompileBlock(guarded.Block, completion);
            ProtectSomething(tryStart);
            buffer.CloseRegion(tryStart, buffer.Code.Count);
            Branch(JsOpcode.Jump, afterCatch);
            Mark(catchHandler);

            var outer = scope;
            scope = new Scope(ScopeKind.Block, outer) { IsCatch = true };
            blockDepth++;
            var scopeSite = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((scopeSite, scope));

            if (guarded.CatchPattern is not null)
            {
                DeclarePatternNames(guarded.CatchPattern, constant: false);
                BindPattern(guarded.CatchPattern, BindMode.Initialise);
            }
            else
            {
                var parameter = guarded.CatchParameter.Length == 0
                    ? scope.Declare("#caught", constant: false)
                    : scope.Declare(guarded.CatchParameter, constant: false);

                EmitScoped(JsOpcode.InitialiseScoped, 0, parameter);
            }

            // THE PARAMETERS ARE THE CATCH CLAUSE'S OWN BINDINGS AND EVERYTHING AFTER THEM IS THE
            // CATCH BLOCK'S: this lowering keeps both in one record, and a direct eval's conflict
            // walk exempts only the first half (Annex B.3.4, JSeal V15).
            scope.LexicalFrom = scope.SlotCount;

            // A PATTERN PARAMETER'S DEFAULTS CLOSE OVER THE PARAMETER SCOPE AND NOT THE BLOCK'S:
            // the catch Block is a scope of its own inside the parameter's (14.15.3), so a default
            // that closes over `x` sees the outer `x`, not a `let x` the block declares. Until
            // 2026-10-03 both lived in one record (JSC-254). A plain parameter has no default to
            // close over anything, so it keeps the one record.
            var blockScoped = guarded.CatchPattern is not null;
            var parameterScope = scope;

            if (blockScoped)
            {
                scope = new Scope(ScopeKind.Block, parameterScope);
                blockDepth++;
                var bodySite = buffer.Code.Count + 1;
                Emit(JsOpcode.PushScope, (ushort)0);
                buffer.ScopeSites.Add((bodySite, scope));
            }

            // THE BLOCK'S LEXICAL NAMES ARE HOISTED BEFORE ITS FIRST STATEMENT, as any Block's
            // are: a closure created above a `let` resolves to that `let`, in its dead zone, and
            // not to a name outside. Until 2026-10-03 a catch body declared each name only when
            // its declaration was reached (JSC-254).
            var catchLexical = new System.Collections.Generic.List<(string Name, bool Constant)>();
            CollectLexical(guarded.Handler.Body, catchLexical);
            DeclareLexical(catchLexical);
            HoistBlockFunctions(guarded.Handler.Body);

            // A catch body is a Block and disposes its own resources, inside the parameter's scope
            // and before control leaves the handler - not in whatever list encloses the `try`.
            CompileDisposing(guarded.Handler.Body, completion);

            if (blockScoped)
            {
                Emit(JsOpcode.PopScope);
                blockDepth--;
                scope = parameterScope;
            }

            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
            Mark(afterCatch);
        }
        else
        {
            CompileBlock(guarded.Block, completion);
        }

        if (!hasFinally)
        {
            regionLevel--;
            Mark(end);
            return;
        }

        var finallyStart = tryStart;
        var rethrow = NewLabel();
        buffer.PendingRegions.Add(
            new PendingRegion(
                finallyStart, rethrow, blockDepth, JsFormat.HandlerKind.Finally, Level: level));

        ProtectSomething(finallyStart);
        buffer.CloseRegion(finallyStart, buffer.Code.Count);
        regionLevel--;
        exits.RemoveAt(exits.Count - 1);

        // The normal path runs the finaliser inline and continues.
        //
        // A `finally` PRODUCES NO VALUE WHEN IT COMPLETES NORMALLY, which is why it is compiled
        // with no completion slot at all rather than with the enclosing script's. The language
        // takes the `try` statement's value from its BLOCK - or from its `catch` - and keeps that
        // value across the finaliser: `2; try { 3; } finally { }` is 3, and
        // `4; try { } catch (e) { } finally { 5; }` is undefined and not 5. Passing the slot down
        // let the finaliser's own statements overwrite an answer that was already settled.
        //
        // UNLESS THE FINALISER ITSELF COMPLETES ABRUPTLY. A `break` or `continue` out of a
        // `finally` replaces the `try`'s completion with its own, whose value is the finaliser's so
        // far - `try { 39 } finally { 42; break; }` is 42, and `finally { break; }` is undefined. So
        // the settled value is set aside, the finaliser writes the slot from undefined, and the
        // value set aside is put back only where the finaliser falls through. Until 2026-10-04 the
        // finaliser wrote nothing and a `break` out of it carried the `try`'s value (JSC-258).
        var settled = -1;

        if (completion >= 0)
        {
            var owner = FunctionScope();
            settled = owner.Declare("#held" + owner.SlotCount, constant: false);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, completion);
            EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, settled);
            ResetCompletion(completion);
        }

        CompileBlock(guarded.Finaliser!, completion);

        if (settled >= 0)
        {
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, settled);
            EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, completion);
        }

        Branch(JsOpcode.Jump, end);

        // The exceptional path stores the thrown value, runs the same block, and rethrows it.
        Mark(rethrow);
        var handlerOuter = scope;
        scope = new Scope(ScopeKind.Block, handlerOuter);
        blockDepth++;
        var site = buffer.Code.Count + 1;
        Emit(JsOpcode.PushScope, (ushort)0);
        buffer.ScopeSites.Add((site, scope));
        var pending = scope.Declare("#pending", constant: false);
        EmitScoped(JsOpcode.InitialiseScoped, 0, pending);
        ResetCompletion(completion);
        CompileBlock(guarded.Finaliser!, completion);
        EmitScoped(JsOpcode.LoadScoped, 0, pending);
        Emit(JsOpcode.PopScope);
        blockDepth--;
        scope = handlerOuter;
        Emit(JsOpcode.Throw);
        Mark(end);
    }

    /// <summary>
    /// Puts an instruction inside a protected range that would otherwise be empty.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b><c>try { } catch (e) { }</c> lowered to a region whose start equalled its end, and the
    /// verifier refused the artifact this lowering had just produced</b> — the same shape as
    /// [JSC-81], found the same way, by a program written to cover the surface rather than to
    /// confirm it. The verifier is right to refuse it: a region protecting no instruction is a
    /// region nothing can enter, and its handler is code the abstract pass would nonetheless seed
    /// as an entry, at a height nothing establishes.
    /// </para>
    /// <para>
    /// <b>So the lowering makes the range real rather than the verifier making the rule weaker.</b>
    /// The alternative considered was to emit no region and no handler for an empty block, and it
    /// is worse in a way that matters here: the handler's code would still be in the unit, reached
    /// by nothing, and an instruction stream carrying code no entry seeds is how unverified code
    /// gets into a verified artifact. A <c>Nop</c> costs one instruction in a block that had none,
    /// and only in that block — every non-empty <c>try</c> is unchanged.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=EAD16F
    // Broiler-Falsified-If: a region is emitted whose start offset equals its end offset
    // Broiler-Human:        PENDING
    private void ProtectSomething(int from)
    {
        if (buffer.Code.Count == from)
        {
            Emit(JsOpcode.Nop);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7E3B21
    // Broiler-Human:        PENDING
    private void CompileReturn(JsReturnStatement returned)
    {
        // THE QUESTION IS WHICH HOISTING SCOPE ENCLOSES THIS, NOT WHICH SCOPE IS CURRENT. A block
        // that declares a lexical name pushes a scope of its own, and so does a `with`, so asking
        // the current scope let `{ let a; return 1; }` at the top of a script through - and admitting
        // `with` would have added a second way in.
        // AND IT IS AN EARLY ERROR RATHER THAN A MANIFEST REFUSAL, which until 2026-09-05 it was
        // not. `2104` says this profile declines a construct it could otherwise run, and the
        // conformance runner reads the code: every case of a top-level `return` was reported as a
        // surface nobody had built and taken out of both the pass and the fail column, when what
        // the language says is that the source is not a program. `2101` is what a source wrong
        // about the LANGUAGE gets, exactly as `with` in strict code and a `super` property outside
        // a method do.
        // A module body is no more a function than a script is (16.2.1.1: ModuleItemList may not
        // contain a ReturnStatement); until 2026-10-03 a module's `return` ran (JSC-253).
        if (FunctionScope().Kind is ScopeKind.Program or ScopeKind.Eval or ScopeKind.Module)
        {
            Refuse(
                returned.Span,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "`return` has nothing to return from outside a function");

            return;
        }

        // A STATIC BLOCK COMPILES TO A FUNCTION AND IS NOT ONE, which is why the scope test above
        // does not catch this. The block's body is a code unit so that it can close over the class
        // scope and be called with the constructor as its `this`; nothing about it is a function a
        // program can return FROM, and the specification makes the word an early error there.
        if (insideStaticBlock)
        {
            Refuse(
                returned.Span,
                SliceSourceDiagnosticCode.IllegalBreak,
                "`return` has nothing to return from inside a class static block");

            return;
        }

        var unwinds = false;

        for (var index = exits.Count - 1; index >= 0 && !unwinds; index--)
        {
            unwinds = (exits[index].Kind == ExitKind.Finally && !exits[index].Running) ||
                exits[index].IteratorSlot >= 0 ||
                exits[index].Kind == ExitKind.Dispose;
        }

        if (!unwinds)
        {
            if (returned.Value is null)
            {
                Emit(JsOpcode.ReturnUndefined);
                return;
            }

            if (fieldValueName is { } named)
            {
                CompileNamedValue(returned.Value, named);
            }
            else
            {
                CompileExpression(returned.Value);
            }

            AwaitTheReturnedValue();
            Emit(JsOpcode.Return);
            return;
        }

        // A `return` that has to unwind must run every enclosing finaliser and close every
        // enclosing `for … of` iterator before it leaves, so the value is parked in a slot of the
        // function's own environment while they run. Returning it on the operand stack instead
        // would reach `Return` with more than one value the moment a finaliser pushed anything,
        // which is what the verifier refuses.
        var function = FunctionScope();
        var slot = function.Declare("#return" + function.SlotCount, constant: false);

        if (returned.Value is null)
        {
            Emit(JsOpcode.LoadUndefined);
        }
        else
        {
            CompileExpression(returned.Value);
            AwaitTheReturnedValue();
        }

        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, slot);
        var savedDepth = blockDepth;
        var savedScope = scope;
        var left = UnwindAbove(-1);

        Unwrap(0);
        EmitScoped(JsOpcode.LoadScoped, 0, slot);
        Emit(JsOpcode.Return);
        Left(left);
        blockDepth = savedDepth;
        scope = savedScope;
    }

    /// <summary>
    /// Awaits what a <c>return</c> is carrying, in the one body kind where the language does.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is the ASYNC GENERATOR and not the async function, and the asymmetry is the
    /// language's.</b> <c>ReturnStatement : return Expression</c> awaits its value when
    /// <c>GetGeneratorKind()</c> is <c>async</c>, and that answer is <c>async</c> only inside an
    /// async generator: an ordinary async function has no generator component at all. So
    /// <c>async function* g() { return Promise.resolve(1); }</c> completes with <c>1</c> where
    /// <c>async function f() { return Promise.resolve(1); }</c> completes with the promise - and the
    /// second is invisible, because the promise the CALL answered adopts it either way.
    /// </para>
    /// <para>
    /// <b>The await happens before the unwinding and not after it</b>, because it belongs to the
    /// evaluation of the return statement rather than to the completion travelling out. A
    /// <c>finally</c> that observes the world therefore sees it after the value has settled, which
    /// is the same ordering <c>gen.return(p)</c> has for the same reason.
    /// </para>
    /// <para>
    /// A <c>return</c> with no expression is not on this path at all: it completes with
    /// <c>undefined</c>, which the language does not await and which has nothing to wait for.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3768F5
    // Broiler-Human:        PENDING
    private void AwaitTheReturnedValue()
    {
        const JsFormat.FunctionFlags both =
            JsFormat.FunctionFlags.Async | JsFormat.FunctionFlags.Generator;

        if ((buffer.Flags & both) == both)
        {
            Emit(JsOpcode.Await);
        }
    }

    /// <summary>Runs one enclosing exit's finaliser, or closes its iterator, on the way out.</summary>
    /// <remarks>
    /// The scopes between here and the exit are discarded first, because a finaliser's body and an
    /// iterator's slot are both addressed relative to the depth the exit was created at.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=10C2A5
    // Broiler-Human:        PENDING
    private void Unwind(Exit exit)
    {
        // A RESOURCE SCOPE IS DISPOSED ON THE WAY PAST IT, innermost first like everything else
        // here, and under a normal completion: a `break`, `continue` or `return` is not a throw, so
        // the first disposer that throws becomes the exception and the jump never happens.
        if (exit.Kind == ExitKind.Dispose)
        {
            Unwrap(exit.Depth);
            EmitDisposal(exit, thrown: false);
            return;
        }

        if (exit.Kind == ExitKind.Finally)
        {
            if (exit.Running)
            {
                return;
            }

            Unwrap(exit.Depth);
            exit.Running = true;
            CompileBlock(exit.Finaliser!, -1);
            exit.Running = false;
            return;
        }

        if (exit.IteratorSlot < 0)
        {
            return;
        }

        Unwrap(exit.Depth);

        if (exit.IteratorIsAsync)
        {
            CompileAsyncClose(exit.IteratorSlot);
            return;
        }

        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, exit.IteratorSlot);
        Emit(JsOpcode.IterateClose, (byte)0);
    }

    /// <summary>
    /// Discards scopes down to <paramref name="depth"/>, at COMPILE time as well as at run time.
    /// </summary>
    /// <remarks>
    /// <b>The compile-time scope has to move with the counter, and it did not.</b> Every unwinding
    /// path emitted <c>PopScope</c> and decremented <c>blockDepth</c> while leaving <c>scope</c>
    /// pointing at the block it had just discarded - so a name a finaliser read afterwards was
    /// resolved one hop too far out. <c>function f() { try { throw x; } catch (e) { return 1; }
    /// finally { outer.push(1); } }</c> is the shortest witness: <c>outer</c> was resolved against
    /// the catch scope that had already been popped, and the finaliser read whatever slot of that
    /// index the grandparent environment held. It compiled, it verified, and it answered the wrong
    /// value - which is the failure mode a stack-height check cannot see.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A5998A
    // Broiler-Human:        PENDING
    private void Unwrap(int depth)
    {
        while (blockDepth > depth && scope.Parent is not null)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = scope.Parent;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=145379
    // Broiler-Human:        PENDING
    private void CompileJumpOut(SliceSourceSpan span, string label, bool wantsContinue)
    {
        var targetAt = -1;

        for (var index = exits.Count - 1; index >= 0; index--)
        {
            var candidate = exits[index];

            if (candidate.Kind == ExitKind.Finally)
            {
                continue;
            }

            if (label.Length != 0)
            {
                if (!string.Equals(candidate.Label, label, System.StringComparison.Ordinal))
                {
                    continue;
                }

                if (wantsContinue && candidate.Continue is null)
                {
                    continue;
                }

                targetAt = index;
                break;
            }

            if (wantsContinue)
            {
                if (candidate.Continue is null)
                {
                    continue;
                }

                targetAt = index;
                break;
            }

            if (candidate.Kind is ExitKind.Loop or ExitKind.Switch)
            {
                targetAt = index;
                break;
            }
        }

        if (targetAt < 0)
        {
            Refuse(
                span,
                wantsContinue
                    ? SliceSourceDiagnosticCode.IllegalContinue
                    : SliceSourceDiagnosticCode.IllegalBreak,
                wantsContinue
                    ? "`continue` names no enclosing loop"
                    : "`break` names no enclosing loop or switch");

            return;
        }

        var target = exits[targetAt];
        var savedDepth = blockDepth;
        var savedScope = scope;
        var closes = !wantsContinue && target.IteratorSlot >= 0;
        var left = UnwindAbove(targetAt, closes ? target : null);

        Unwrap(target.Depth);

        // `break` LEAVES THE LOOP AND `continue` DOES NOT, and that is the whole of why the target
        // is treated differently from the loops passed on the way. A `continue` that closed the
        // iterator would end the loop it was asked to keep going.
        if (closes)
        {
            // THE CLOSE RUNS ONCE THE BODY HAS COMPLETED, so the loop's own region, and every one
            // inside the loop, must not see what `return` throws.
            left.Add((buffer.Code.Count, target.Level));

            if (target.IteratorIsAsync)
            {
                CompileAsyncClose(target.IteratorSlot);
            }
            else
            {
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, target.IteratorSlot);
                Emit(JsOpcode.IterateClose, (byte)0);
            }
        }

        Branch(JsOpcode.Jump, wantsContinue ? target.Continue! : target.Break!);
        Left(left);
        blockDepth = savedDepth;
        scope = savedScope;
    }

    /// <summary>
    /// Unwinds every exit above <paramref name="stop"/>, innermost first, and answers where the
    /// unwinding of each statement that owns regions began.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Each start is where that statement has completed.</b> The code from there to the end of
    /// the jump is outside it, which <see cref="Left"/> records once the jump is written - not
    /// before, so that a region a finaliser's own body opens and closes inside the sequence is
    /// never split by the sequence around it.
    /// </para>
    /// <para>
    /// <b>A range that would lose every instruction to the holes gets a <c>Nop</c> first.</b> A
    /// <c>break</c> that is a <c>try</c> block's first statement starts leaving at the block's
    /// first offset, and a region that protected nothing would leave its handler as code no entry
    /// seeds - the same reason <see cref="ProtectSomething"/> exists.
    /// </para>
    /// </remarks>
    /// <param name="stop">The index of the exit that is not left, or -1 to leave them all.</param>
    /// <param name="alsoLeaving">The target whose own region is left too, when the jump closes it.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=8C6605
    // Broiler-Falsified-If: an exit that owns regions is unwound without its start being answered, or a region open at the jump is left with no instruction outside the answered starts
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<(int Start, int Level)> UnwindAbove(
        int stop, Exit? alsoLeaving = null)
    {
        var left = new System.Collections.Generic.List<(int Start, int Level)>();
        var leaves = alsoLeaving is not null;

        for (var index = exits.Count - 1; index > stop && !leaves; index--)
        {
            leaves = Leaves(exits[index]);
        }

        if (leaves)
        {
            var at = buffer.Code.Count;
            var empty = false;

            foreach (var pending in buffer.PendingRegions)
            {
                empty |= pending.TryStart == at;
            }

            for (var index = exits.Count - 1; index > stop; index--)
            {
                empty |= exits[index].Kind == ExitKind.Finally && exits[index].Guarded == at;
            }

            if (empty)
            {
                Emit(JsOpcode.Nop);
            }
        }

        for (var index = exits.Count - 1; index > stop; index--)
        {
            var exit = exits[index];
            var start = buffer.Code.Count;
            var owns = Leaves(exit);
            Unwind(exit);

            if (owns)
            {
                left.Add((start, exit.Level));
            }
        }

        return left;
    }

    /// <summary>Whether unwinding an exit leaves a statement that owns exception regions.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7254BE
    // Broiler-Human:        PENDING
    private static bool Leaves(Exit exit) =>
        exit.Level >= 0 && !(exit.Kind == ExitKind.Finally && exit.Running);

    /// <summary>
    /// Records, for each start <see cref="UnwindAbove"/> answered, that the code from there to the
    /// cursor has left that statement.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=26BD6C
    // Broiler-Human:        PENDING
    private void Left(System.Collections.Generic.List<(int Start, int Level)> left)
    {
        foreach (var (start, level) in left)
        {
            buffer.Leave(start, level);
        }
    }
}
