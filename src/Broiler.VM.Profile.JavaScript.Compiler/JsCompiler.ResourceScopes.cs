// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           0
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  3/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // ---- resource scopes (JSeal F21-F22, JSD-0034) ------------------------------------------------

    /// <summary>
    /// Whether a statement list declares a resource directly, and whether any of them is awaited.
    /// </summary>
    /// <remarks>
    /// Only the list's own declarations count: a block inside it is a scope of its own and disposes
    /// its own resources, and the parser refuses a resource declaration wherever a label or a
    /// single-statement position would have hidden one from this walk.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=549314
    // Broiler-Human:        PENDING
    private static JsUsing UsingIn(
        System.Collections.Generic.IReadOnlyList<JsStatement> body, out SliceSourceSpan first)
    {
        var found = JsUsing.None;
        first = default;

        foreach (var statement in body)
        {
            if (statement is not JsVariableStatement { Using: not JsUsing.None } declaration)
            {
                continue;
            }

            if (found == JsUsing.None)
            {
                first = declaration.Span;
            }

            if (declaration.Using > found)
            {
                found = declaration.Using;
            }
        }

        return found;
    }

    /// <summary>
    /// Compiles a statement list, inside a resource scope when the list declares a resource.
    /// </summary>
    /// <remarks>
    /// <b>The scope is opened before the first statement and not at the first declaration</b>, so a
    /// list is one region with one handler however many declarations it has: the specification's
    /// <c>DisposeCapability</c> belongs to the environment the list runs in, and every resource the
    /// list registers joins the same stack in the order it was declared.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E0C0C5
    // Broiler-Human:        PENDING
    private void CompileDisposing(
        System.Collections.Generic.IReadOnlyList<JsStatement> body, int completion)
    {
        var declared = UsingIn(body, out var first);

        if (declared == JsUsing.None)
        {
            CompileStatements(body, completion);
            return;
        }

        var disposal = BeginDisposal(first, declared == JsUsing.Async);
        CompileStatements(body, completion);
        EndDisposal(disposal);
    }

    /// <summary>
    /// Opens a resource scope in the current compile-time scope: its slot, its value, its region
    /// and the exit every <c>break</c>, <c>continue</c> and <c>return</c> through it unwinds.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The region is a FINALLY region and not a catch region</b>, because a generator's forced
    /// return has to dispose the scope too: <c>gen.return()</c> runs every <c>finally</c> and no
    /// <c>catch</c>, and a resource the generator declared is released by the return exactly as
    /// <c>try { } finally { }</c> would release it. The handler tells the two apart by the value it
    /// is entered with, which is what <see cref="JsOpcode.DisposeFold"/> reads.
    /// </para>
    /// <para>
    /// <b>The slot's name begins with <c>#</c></b>, which no identifier the front end produces
    /// does, so no source can read or overwrite the scope - the same rule that keeps a
    /// <c>for … of</c> record and a private name out of guest hands.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=83FAFA
    // Broiler-Falsified-If: a resource registered in the list can be left without its scope being disposed, by falling off the end, a jump, a return, a throw or a forced return
    // Broiler-Human:        PENDING
    private Exit BeginDisposal(SliceSourceSpan span, bool isAsync)
    {
        var slot = scope.Declare("#dispose" + scope.SlotCount, constant: false);
        Emit(JsOpcode.DisposeScope);
        EmitScoped(JsOpcode.InitialiseScoped, 0, slot);

        var level = ++regionLevel;
        var exit = new Exit(ExitKind.Dispose, string.Empty, blockDepth)
        {
            Level = level,
            DisposalSlot = slot,
            DisposalScope = scope,
            DisposalIsAsync = isAsync,
            Guarded = buffer.Code.Count,
            DisposalHandler = NewLabel(),
            DisposalSpan = span,
        };

        buffer.PendingRegions.Add(
            new PendingRegion(
                exit.Guarded, exit.DisposalHandler!, blockDepth, JsFormat.HandlerKind.Finally, Level: level));

        exits.Add(exit);
        return exit;
    }

    /// <summary>
    /// Closes a resource scope: disposes it on the normal path, and emits the handler that disposes
    /// it for a throw or a forced return and then re-raises what the completion became.
    /// </summary>
    /// <remarks>
    /// <b>The normal path's disposal is OUTSIDE the region</b>, so a disposer that throws there is
    /// the list's exception and is not handed back to the list's own handler. A disposal inlined
    /// for a <c>break</c> or a <c>return</c> is kept out of it too, and out of every region inside
    /// the list, by the hole <see cref="UnitBuffer.Leave"/> records: what it throws belongs to the
    /// statements around the list, so a <c>finally</c> the jump already ran is not run again.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=FDF86E
    // Broiler-Falsified-If: the handler of a resource scope runs a disposer the normal path already ran, or re-raises anything other than the folded completion or the forced return it was entered with
    // Broiler-Human:        PENDING
    private void EndDisposal(Exit exit)
    {
        exits.Remove(exit);
        ProtectSomething(exit.Guarded);
        buffer.CloseRegion(exit.Guarded, buffer.Code.Count);
        regionLevel--;

        EmitDisposal(exit, thrown: false);
        var after = NewLabel();
        Branch(JsOpcode.Jump, after);

        // THE HANDLER IS ENTERED WITH THE THROWN VALUE AND NOTHING ELSE, at the scope's own depth,
        // because the region was opened after the scope's record was.
        Mark(exit.DisposalHandler!);
        buffer.Rejoin(1);
        EmitDisposal(exit, thrown: true);
        Mark(after);
    }

    /// <summary>
    /// Disposes a resource scope from the depth it was declared at: under a normal completion when
    /// <paramref name="thrown"/> is false, and with the value on the stack folded in first when it
    /// is true.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A synchronous scope is one instruction.</b> <see cref="JsOpcode.DisposeEnd"/> runs every
    /// entry newest first, folds each throw into the completion as a <c>SuppressedError</c>, and
    /// settles the completion - so nothing in a function that may not await ever suspends here.
    /// </para>
    /// <para>
    /// <b>An asynchronous scope is a loop around the ordinary <see cref="JsOpcode.Await"/></b>,
    /// because an await is a suspension and one instruction cannot suspend in the middle of itself.
    /// <see cref="JsOpcode.DisposeStep"/> runs entries until one owes an await and leaves what is
    /// owed; the await is guarded by a catch region whose handler folds a rejection into the scope's
    /// completion exactly as a synchronous throw is folded, and the loop asks again. Every await is
    /// therefore the realm's own, costing the turn the specification's <c>Await</c> costs.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=5B2B12
    // Broiler-Falsified-If: an asynchronous disposal is emitted into a unit that may not await, or a rejected disposer's reason escapes without being folded into the completion
    // Broiler-Human:        PENDING
    private void EmitDisposal(Exit exit, bool thrown)
    {
        var hops = (byte)Hops(exit.DisposalScope!);

        // WHAT DISPOSAL THROWS IS REPORTED AT THE DECLARATION THAT OPENED THE SCOPE, which is the
        // one place in the source that names the disposal; the end of the list has no token of
        // its own, and the last statement before it did not throw.
        Position(exit.DisposalSpan);

        if (thrown)
        {
            EmitScoped(JsOpcode.LoadScoped, hops, exit.DisposalSlot);
            Emit(JsOpcode.DisposeFold);
        }

        if (exit.DisposalIsAsync)
        {
            var height = buffer.Height;
            var step = NewLabel();
            var done = NewLabel();
            var rejected = NewLabel();

            Mark(step);
            EmitScoped(JsOpcode.LoadScoped, hops, exit.DisposalSlot);
            Branch(JsOpcode.DisposeStep, done);

            var awaited = buffer.Code.Count;
            buffer.PendingRegions.Add(
                new PendingRegion(awaited, rejected, blockDepth, JsFormat.HandlerKind.Catch, height));

            Emit(JsOpcode.Await);
            buffer.CloseRegion(awaited, buffer.Code.Count);
            Emit(JsOpcode.Pop);
            Branch(JsOpcode.Jump, step);

            Mark(rejected);
            buffer.Rejoin(height + 1);
            EmitScoped(JsOpcode.LoadScoped, hops, exit.DisposalSlot);
            Emit(JsOpcode.DisposeFold);
            Branch(JsOpcode.Jump, step);

            Mark(done);
            buffer.Rejoin(height);
        }

        EmitScoped(JsOpcode.LoadScoped, hops, exit.DisposalSlot);
        Emit(JsOpcode.DisposeEnd, (byte)(thrown ? 1 : 0));

        if (thrown)
        {
            Emit(JsOpcode.Throw);
        }
    }

    /// <summary>
    /// Registers the value on the stack with the innermost resource scope, leaving the value.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D9903B
    // Broiler-Human:        PENDING
    private void EmitRegistration(SliceSourceSpan span, JsUsing hint)
    {
        for (var index = exits.Count - 1; index >= 0; index--)
        {
            if (exits[index].Kind != ExitKind.Dispose)
            {
                continue;
            }

            Position(span);
            EmitScoped(
                JsOpcode.LoadScoped,
                (byte)Hops(exits[index].DisposalScope!),
                exits[index].DisposalSlot);

            Emit(JsOpcode.DisposeAdd, (byte)(hint == JsUsing.Async ? 1 : 0));
            return;
        }

        // THE PARSER ADMITS A RESOURCE ONLY IN A LIST THAT OPENS A SCOPE, so reaching here is a
        // front-end defect; it is refused rather than lowered as the `const` it resembles.
        Refuse(
            span,
            SliceSourceDiagnosticCode.ConstructOutsideManifest,
            "a `using` declaration outside a statement list that can dispose it");
    }
}
