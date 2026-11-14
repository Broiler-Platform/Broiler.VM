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
// Criteria:         1/1
// Resource impact:  3/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=0A4526
    // Broiler-Human:        PENDING
    private void CompileCall(CallExpression call)
    {
        var callee = JsTreeFacts.Unwrap(call.Callee);

        // `super.m()` IS THE ONE CALL WHOSE CALLEE AND RECEIVER COME FROM DIFFERENT PLACES. The
        // function is found through the method's home object and the receiver is `this`, so the
        // ordinary member path - which duplicates the base and uses it for both - would have
        // called the inherited method against the prototype instead of against the instance.
        if (callee is MemberExpression { Object: SuperExpression } inherited)
        {
            CompileSuperKey(inherited);
            Emit(JsOpcode.LoadSuperProperty);
            Emit(JsOpcode.LoadThis);
            CompileArguments(call);
            return;
        }
        EmitCallee(call.Callee);

        // A DIRECT `eval` IS A FACT ABOUT THE SPELLING, and this is the only place that fact
        // still exists. `eval(s)` and `(0, eval)(s)` reach the same function object with the same
        // receiver and the same arguments; the language says the first evaluates in the caller's
        // scope and the second in the global one, and no executor can recover the difference from
        // the operand stack. So the lowering says it, with an opcode whose stack effect is the
        // ordinary call's - which is what lets the verifier check it while knowing nothing about
        // what it means.
        //
        // A LOCALLY BOUND `eval` IS THIS TOO, and the executor's identity check decides. The
        // language's test is the reference's NAME and the called value's identity with %eval% -
        // not where the name resolved - so `var eval = globalThis.eval; eval(s)` is a direct
        // evaluation in the function's scope. Until JSeal V15 a name that resolved to a slot was
        // lowered as an ordinary `Call`, which made that program a silent global evaluation whose
        // `var` became a property of the global object. A `with` record between the call and the
        // binding needed the same answer for the same reason (JSD-0026 step 1).
        var direct = callee is Identifier named &&
            string.Equals(named.Name, "eval", System.StringComparison.Ordinal);

        CompileArguments(call, direct);
    }

    /// <summary>Pushes a callee and the receiver the calling convention wants above it.</summary>
    /// <remarks>
    /// <b>Which receiver a call gets is decided by the SPELLING of its callee and by nothing
    /// else.</b> <c>o.f()</c> passes <c>o</c> and <c>(o.f)()</c> passes it too, while <c>(0,
    /// o.f)()</c> and a tagged template whose tag is parenthesised pass <c>undefined</c> - the
    /// difference being whether the callee expression is still a reference when the call reaches
    /// it. This is the one place that decision is made, so an ordinary call and a tagged template
    /// cannot drift apart on it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=234CC8
    // Broiler-Human:        PENDING
    private void EmitCallee(Expression written)
    {
        var callee = JsTreeFacts.Unwrap(written);

        if (callee is MemberExpression { Object: not SuperExpression, Property: not PrivateIdentifier } member)
        {
            CompileExpression(member.Object);
            Emit(JsOpcode.Duplicate);

            if (JsTreeFacts.Computed(member) is null)
            {
                Emit(JsOpcode.GetProperty, InternedName(JsTreeFacts.PropertyName(member)));
            }
            else
            {
                CompileExpression(JsTreeFacts.Computed(member)!);
                Emit(JsOpcode.GetIndex);
            }

            // The receiver is under the callee and the calling convention wants it above, so one
            // exchange turns [receiver, callee] into [callee, receiver].
            Emit(JsOpcode.Swap);
            return;
        }

        // `o.#m()` IS CALLED AGAINST `o` EXACTLY AS `o.m()` IS. The private element is found in a
        // different table and the calling convention is the same one, so the shape below is the
        // member arm's with the private instruction where the property read was.
        if (callee is MemberExpression { Property: PrivateIdentifier } privateCallee)
        {
            CompileExpression(privateCallee.Object);
            Emit(JsOpcode.Duplicate);
            EmitPrivateName(JsTreeFacts.At(privateCallee), JsTreeFacts.PropertyName(privateCallee));
            Emit(JsOpcode.LoadPrivate);
            Emit(JsOpcode.Swap);
            return;
        }

        // A CALLEE RESOLVED THROUGH AN OBJECT ENVIRONMENT RECORD IS CALLED AGAINST THAT OBJECT.
        // `with (o) { f() }` runs `f` with `o` as its `this` and `with ({}) { f() }` runs it with
        // `undefined`, and the difference is decided by which of the two branches the search took -
        // which is why the receiver is produced by the same lowering that produced the callee
        // rather than pushed after it.
        if (callee is Identifier bare && Shadowable(bare.Name, out var limit))
        {
            EmitDynamicName(bare.Name, limit, wantsBase: true, orUndefined: false);
            return;
        }

        // A CALLEE EVAL CODE DOES NOT BIND MAY BE A CALLER'S `with` OBJECT'S METHOD, and then it is
        // called against that object - so the name is resolved with its base rather than read and
        // called against `undefined` (JSD-0026 section 5).
        if (callee is Identifier free && !Resolvable(free.Name) && TryEvalHops(out var evalHops))
        {
            EmitEvalName(JsOpcode.LoadEvalNameWithBase, evalHops, free.Name);
            return;
        }

        // A PARENTHESISED CHAIN KEEPS ITS BASE AS THE RECEIVER, as `(o.f)()` does (JSC-258).
        if (callee is ChainExpression chain && JsTreeFacts.Unwrap(chain.Expression) is MemberExpression { Object: not SuperExpression })
        {
            EmitChainReceiver(EmitChainCalleeValue(callee, NewLabel(), shortIsTrue: false));
            return;
        }

        CompileExpression(callee);
        Emit(JsOpcode.LoadUndefined);
    }

    // ---- optional chains -----------------------------------------------------------------------

    /// <summary>Lowers one whole optional chain.</summary>
    /// <remarks>
    /// <para>
    /// <b>One merge label for the chain, and every short circuit in it jumps there.</b> That is
    /// the shape the construct demands: in <c>a?.b.c.d</c> a nullish <c>a</c> must skip <c>.c</c>
    /// and <c>.d</c> as well, so the target of the jump is past the LAST link and not past the
    /// next one - a position only this method knows, because only this method knows where the
    /// chain ends.
    /// </para>
    /// <para>
    /// <b>Both paths reach the merge holding exactly one value, and that is the whole of what the
    /// verifier checks.</b> Each guard knows how many values the chain has pushed at the point it
    /// tests - one for a plain access, two for a call whose receiver is already under its callee -
    /// and pops exactly that many before pushing the answer. A guard that miscounted would reach
    /// the merge at the wrong height and be refused with an inconsistent join, which is the good
    /// failure: it is caught by the verifier rather than by the value being wrong at run time.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=838E5B
    // Broiler-Falsified-If: a link after a short-circuited one is evaluated, or the two paths meet at different heights
    // Broiler-Human:        PENDING
    private void CompileChain(ChainExpression chain)
    {
        var end = NewLabel();
        EmitChainLink(chain.Expression, end, shortIsTrue: false);
        Mark(end);
    }

    /// <summary>Emits one link of a chain, leaving its value on the operand stack.</summary>
    /// <remarks>
    /// The recursion descends the chain to its head and builds back up, which is the order the
    /// language evaluates in: the base, then the key or the arguments, and never the second when
    /// the first declined. Anything that is not a member or a call is the head, and is compiled as
    /// the ordinary expression it is - which is how a parenthesised chain inside another one keeps
    /// its own merge.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B3F5EB
    // Broiler-Human:        PENDING
    private void EmitChainLink(Expression written, Label end, bool shortIsTrue)
    {
        var node = JsTreeFacts.Unwrap(written);

        switch (node)
        {
            case MemberExpression { Property: PrivateIdentifier } privateAccess:
            {
                EmitChainLink(privateAccess.Object, end, shortIsTrue);

                if (privateAccess.IsOptional)
                {
                    EmitNullishGuard(end, held: 1, shortIsTrue);
                }

                EmitPrivateName(JsTreeFacts.At(privateAccess), JsTreeFacts.PropertyName(privateAccess));
                Emit(JsOpcode.LoadPrivate);
                return;
            }

            case MemberExpression { Object: not SuperExpression, Property: not PrivateIdentifier } member:
            {
                EmitChainLink(member.Object, end, shortIsTrue);

                if (member.IsOptional)
                {
                    EmitNullishGuard(end, held: 1, shortIsTrue);
                }

                if (JsTreeFacts.Computed(member) is null)
                {
                    Emit(JsOpcode.GetProperty, InternedName(JsTreeFacts.PropertyName(member)));
                }
                else
                {
                    CompileExpression(JsTreeFacts.Computed(member)!);
                    Emit(JsOpcode.GetIndex);
                }

                return;
            }

            case CallExpression { Callee: not SuperExpression } call when JsTreeFacts.Unwrap(call.Callee) is MemberExpression { Object: not SuperExpression, Property: not PrivateIdentifier } callee:
            {
                EmitChainLink(callee.Object, end, shortIsTrue);

                if (callee.IsOptional)
                {
                    EmitNullishGuard(end, held: 1, shortIsTrue);
                }

                Emit(JsOpcode.Duplicate);

                if (JsTreeFacts.Computed(callee) is null)
                {
                    Emit(JsOpcode.GetProperty, InternedName(JsTreeFacts.PropertyName(callee)));
                }
                else
                {
                    CompileExpression(JsTreeFacts.Computed(callee)!);
                    Emit(JsOpcode.GetIndex);
                }

                // THE RECEIVER IS STILL UNDER THE CALLEE HERE, which is why the guard says two.
                // Testing after the exchange would need to reach under the top of the stack for
                // the value being tested, and testing before it is simply the same test one
                // instruction earlier.
                if (call.IsOptional)
                {
                    EmitNullishGuard(end, held: 2, shortIsTrue);
                }

                Emit(JsOpcode.Swap);
                CompileArguments(call);
                return;
            }

            // `super.m?.()` IS CALLED AGAINST `this`, as `super.m()` is: the function is found
            // through the home object and the receiver is the method's own. Until 2026-10-04 the
            // optional spelling reached the arm below and called it against `undefined` (JSC-258).
            case CallExpression { Callee: not SuperExpression } call when JsTreeFacts.Unwrap(call.Callee) is MemberExpression { Object: SuperExpression } inherited:
            {
                CompileSuperKey(inherited);
                Emit(JsOpcode.LoadSuperProperty);

                if (call.IsOptional)
                {
                    EmitNullishGuard(end, held: 1, shortIsTrue);
                }

                Emit(JsOpcode.LoadThis);
                CompileArguments(call);
                return;
            }

            case CallExpression { Callee: not SuperExpression } call:
            {
                var receiver = EmitChainCalleeValue(call.Callee, end, shortIsTrue);

                if (call.IsOptional)
                {
                    EmitNullishGuard(end, held: 1, shortIsTrue);
                }

                EmitChainReceiver(receiver);
                CompileArguments(call);
                return;
            }

            default:
                CompileExpression(node);
                return;
        }
    }

    /// <summary>
    /// Pushes a chain call's callee, and answers the slot its receiver was kept in, or -1 when the
    /// callee has none.
    /// </summary>
    /// <remarks>
    /// <b>A parenthesised chain is still a reference, so it keeps its base as the receiver.</b>
    /// <c>(a?.b)()</c> calls <c>a.b</c> against <c>a</c>, as <c>(a.b)()</c> does: the parentheses
    /// change nothing about the reference a member access evaluates to, and an optional link only
    /// adds the short circuit. The base is kept in a temporary that is cleared first, so the call
    /// after a short circuit is made against <c>undefined</c> - and throws, because its callee is
    /// <c>undefined</c> too. Until 2026-10-04 such a call was always made against
    /// <c>undefined</c> (JSC-258).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=786EEB
    // Broiler-Human:        PENDING
    private int EmitChainCalleeValue(Expression callee, Label end, bool shortIsTrue)
    {
        if (JsTreeFacts.Unwrap(callee) is not ChainExpression parenthesised ||
            JsTreeFacts.Unwrap(parenthesised.Expression) is not MemberExpression { Object: not SuperExpression } link)
        {
            EmitChainLink(callee, end, shortIsTrue);
            return -1;
        }

        var owner = FunctionScope();
        var kept = owner.Declare("#held" + owner.SlotCount, constant: false);
        Emit(JsOpcode.LoadUndefined);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);

        var inner = NewLabel();

        switch (link)
        {
            case MemberExpression { Property: PrivateIdentifier } privateAccess:
                EmitChainLink(privateAccess.Object, inner, shortIsTrue: false);

                if (privateAccess.IsOptional)
                {
                    EmitNullishGuard(inner, held: 1, shortIsTrue: false);
                }

                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);
                EmitPrivateName(JsTreeFacts.At(privateAccess), JsTreeFacts.PropertyName(privateAccess));
                Emit(JsOpcode.LoadPrivate);
                break;

            case MemberExpression { Object: not SuperExpression, Property: not PrivateIdentifier } member:
                EmitChainLink(member.Object, inner, shortIsTrue: false);

                if (member.IsOptional)
                {
                    EmitNullishGuard(inner, held: 1, shortIsTrue: false);
                }

                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);

                if (JsTreeFacts.Computed(member) is null)
                {
                    Emit(JsOpcode.GetProperty, InternedName(JsTreeFacts.PropertyName(member)));
                }
                else
                {
                    CompileExpression(JsTreeFacts.Computed(member)!);
                    Emit(JsOpcode.GetIndex);
                }

                break;
        }

        Mark(inner);
        return kept;
    }

    /// <summary>Pushes the receiver <see cref="EmitChainCalleeValue"/> kept, or <c>undefined</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A367EB
    // Broiler-Human:        PENDING
    private void EmitChainReceiver(int kept)
    {
        if (kept < 0)
        {
            Emit(JsOpcode.LoadUndefined);
            return;
        }

        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, kept);
    }

    /// <summary>Emits a call's arguments and the call instruction that consumes them.</summary>
    /// <param name="call">The call, whose callee and receiver are already on the stack.</param>
    /// <param name="direct">
    /// Whether the callee was spelled as the bare name <c>eval</c>, which is the one thing about a
    /// call that only its SPELLING knows. A call inside an optional chain never passes it:
    /// <c>eval?.(s)</c> is INDIRECT in the language, which is the same answer <c>(0, eval)(s)</c>
    /// gets.
    /// </param>
    /// <remarks>
    /// <b>Every call in this lowering ends here, and that is the point.</b> An ordinary call, a
    /// <c>super.m()</c>, an optional call and an optional member call each push their callee and
    /// receiver differently and then all want the same thing done with the argument list - so the
    /// spread test, the 255 ceiling and the choice of instruction live in one place. They did not,
    /// and a spread argument reached a lowering that had never heard of one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=585CA1
    // Broiler-Human:        PENDING
    private void CompileArguments(CallExpression call, bool direct = false)
    {
        // A SPREAD MAKES THE COUNT A RUN-TIME QUANTITY, so the arguments travel as one Array and
        // the instruction that takes them has a fixed stack effect again. A direct `eval` spelled
        // with a spread KEEPS its directness, which it did not until JSeal V14: this lowering
        // emitted `CallSpread` for it on the grounds that no program in the corpus used the
        // spelling, and the pinned suite's `expressions/call/eval-spread*` cases do. The ordinary
        // call made the evaluation a silent global one; `CallEvalSpread` gets the answer
        // `CallEval` gets at the same site, which is a direct evaluation or the explicit refusal
        // (JSD-0026 step 1).
        if (CarriesArgumentsInAnArray(call.Arguments))
        {
            CompileArgumentArray(call.Arguments);

            if (direct)
            {
                RecordEvalSite();
            }

            PositionCall(call);
            Emit(direct ? JsOpcode.CallEvalSpread : JsOpcode.CallSpread);
            return;
        }

        foreach (var argument in call.Arguments)
        {
            CompileExpression(argument);
        }

        if (direct)
        {
            RecordEvalSite();
        }

        // THE CALL IS PLACED AGAIN, after its arguments placed themselves, so the frame that made it
        // is shown at the call in an error's stack: at the method's name for a method call, and at
        // its own start otherwise (JSD-0038).
        PositionCall(call);

        Emit(
            direct ? JsOpcode.CallEval : JsOpcode.Call,
            (byte)System.Math.Min(call.Arguments.Count, 255));
    }

    /// <summary>Short-circuits the whole chain when the value on top is <c>null</c> or <c>undefined</c>.</summary>
    /// <param name="end">Where the chain's two paths meet.</param>
    /// <param name="held">
    /// How many values this chain has on the operand stack right now, the tested one included. It
    /// is the count the short-circuit path pops, and it is passed rather than computed because the
    /// caller is the only thing that knows it.
    /// </param>
    /// <param name="shortIsTrue">
    /// Whether the short circuit answers <c>true</c> rather than <c>undefined</c>, which is what
    /// <c>delete a?.b</c> needs: deleting through a chain that declined to run succeeded, and the
    /// language says so.
    /// </param>
    /// <summary>
    /// Replaces the value on top of the stack with whether it is <c>undefined</c> or <c>null</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the test <c>??</c>, <c>??=</c> and optional chaining make, and it is not
    /// <c>== null</c>.</b> It was lowered as <c>LoadNull; LooseEquals</c>, which is the same
    /// question for every value but one: an object with an <c>[[IsHTMLDDA]]</c> slot is loosely
    /// equal to <c>null</c> (Annex B.3.6.2), and the language asks <c>??</c> and <c>?.</c> whether
    /// the value IS <c>undefined</c> or <c>null</c>, so <c>document.all ?? 1</c> is
    /// <c>document.all</c>. Once a host could make such an object, the old lowering answered
    /// <c>1</c>.
    /// </para>
    /// <para>
    /// <b>Two strict comparisons and a third between their answers</b>, so no opcode was added: a
    /// value is never both, so the two answers differ exactly when one of them is <c>true</c>. None
    /// of the three can run guest code, and the stack is one value in, one value out, as before.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=2226F5
    // Broiler-Human:        PENDING
    private void EmitIsNullish()
    {
        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.LoadUndefined);
        Emit(JsOpcode.StrictEquals);
        Emit(JsOpcode.Swap);
        Emit(JsOpcode.LoadNull);
        Emit(JsOpcode.StrictEquals);
        Emit(JsOpcode.StrictNotEquals);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=53E7A5
    // Broiler-Human:        PENDING
    private void EmitNullishGuard(Label end, int held, bool shortIsTrue)
    {
        // WHAT THE CHAIN IS HOLDING WHEN THE TEST RUNS IS WHAT IT IS STILL HOLDING IF THE TEST
        // SAYS NO, and the straight-line stack model cannot see that, because the path it walks
        // from here is the one that throws the holdings away. It is told afterwards.
        var live = buffer.Height;

        var target = NewLabel();
        Emit(JsOpcode.Duplicate);
        EmitIsNullish();
        Branch(JsOpcode.JumpIfFalse, target);

        for (var index = 0; index < held; index++)
        {
            Emit(JsOpcode.Pop);
        }

        Emit(shortIsTrue ? JsOpcode.LoadTrue : JsOpcode.LoadUndefined);
        Branch(JsOpcode.Jump, end);
        Mark(target);
        buffer.Rejoin(live);
    }

    /// <summary>Pushes the key half of a <c>super</c> property access.</summary>
    /// <remarks>
    /// <para>
    /// An access where the language has no <c>super</c> to start it from is an early error the
    /// binder refuses before any of the unit is lowered (SJL-12.6 item 2); the lowering only asserts
    /// it (<see cref="BinderAdmitted"/>).
    /// </para>
    /// <para>
    /// The key is pushed as a VALUE rather than encoded as a name operand, so that
    /// <c>super.x</c> and <c>super[e]</c> are one instruction with one stack effect. A pair of
    /// instructions would have bought nothing: the named form is a constant load either way.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=342A80
    // Broiler-Human:        PENDING
    private void CompileSuperKey(MemberExpression member)
    {
        if (!insideMethod)
        {
            throw BinderAdmitted(JsTreeFacts.At(member), "a `super` property outside a method");
        }

        if (JsTreeFacts.Computed(member) is null)
        {
            Emit(JsOpcode.LoadConstant, StringConstant(JsTreeFacts.PropertyName(member)));
            return;
        }

        // THE RECEIVER IS READ BEFORE THE KEY EXPRESSION, and the read is emitted for its effect
        // alone. `super[f()]` in a derived constructor that has not called `super()` yet must be a
        // ReferenceError about `this` and must not run `f` at all - the specification takes the
        // this binding first and the key second. Leaving the read to the instruction that consumes
        // the key would have run `f` first and reported whatever `f` did.
        Emit(JsOpcode.LoadThis);
        Emit(JsOpcode.Pop);
        CompileExpression(JsTreeFacts.Computed(member)!);
    }

    /// <summary>
    /// Evaluates a <c>super</c> property reference: the this binding, then the key, which is left
    /// on the stack.
    /// </summary>
    /// <remarks>
    /// <b>The this binding is read for a named key too</b>, because the reference is made from it:
    /// a destructuring target <c>super.x</c> in a derived constructor before <c>super()</c> throws
    /// its <c>ReferenceError</c> when the target is evaluated and not when it is written.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=139122
    // Broiler-Human:        PENDING
    private void EmitSuperReference(MemberExpression member)
    {
        if (JsTreeFacts.Computed(member) is null)
        {
            Emit(JsOpcode.LoadThis);
            Emit(JsOpcode.Pop);
        }

        CompileSuperKey(member);
    }

    /// <summary>
    /// Reads the <c>super</c> property whose key <see cref="CompileSuperKey"/> pushed, leaving the
    /// key under the value for the write that follows.
    /// </summary>
    /// <remarks>
    /// <b>A computed key is converted by the read and kept converted</b>, so a compound, update or
    /// logical assignment observes its <c>toString</c> once (JSC-236). A named key is a String
    /// constant whose conversion nothing can observe, and keeps the two instructions it always had,
    /// so a program with no computed <c>super</c> write lowers to the bytes it lowered to before.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8C9D36
    // Broiler-Human:        PENDING
    private void EmitSuperReadKeepingKey(MemberExpression member)
    {
        if (JsTreeFacts.Computed(member) is null)
        {
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.LoadSuperProperty);
            return;
        }

        Emit(JsOpcode.LoadSuperPropertyKeepKey);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1601D7
    // Broiler-Human:        PENDING
    private void CompileSuperCall(CallExpression call)
    {
        if (!insideDerivedConstructor)
        {
            throw BinderAdmitted(JsTreeFacts.At(call), "a `super` call outside a derived constructor");
        }

        // `super(...args)` IS NOT `f.apply`-SHAPED AND MUST NOT BORROW CallSpread. That
        // instruction takes a callee and a receiver off the operand stack; a super call takes the
        // superclass and the `new.target` from the FRAME, and there is nothing beneath the
        // argument Array for CallSpread to pop. The two families meeting is what
        // `SuperCallSpread` is for.
        if (CarriesArgumentsInAnArray(call.Arguments))
        {
            CompileArgumentArray(call.Arguments);
            Position(JsTreeFacts.At(call));
            Emit(JsOpcode.SuperCallSpread);
            return;
        }

        foreach (var argument in call.Arguments)
        {
            CompileExpression(argument);
        }

        Position(JsTreeFacts.At(call));
        Emit(JsOpcode.SuperCall, (byte)call.Arguments.Count);
    }
}
