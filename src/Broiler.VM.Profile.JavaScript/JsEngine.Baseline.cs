// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           2
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    Critical
// Criteria:         14/14
// Resource impact:  5/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <content>
/// The engine's half of the baseline native form: entering emitted code, and the page it runs from.
/// </content>
/// <remarks>
/// <b>IT IS A SEPARATE FILE BECAUSE IT IS THE ONE PLACE THE ENGINE LEAVES MANAGED CODE.</b> Everything
/// the form executes is the dispatch loop's own body; what this half adds is the call into the
/// mapped code, the thread slot that call sets, and the mapping itself - the small surface a reader
/// auditing where emitted code is entered has to read, kept apart from the seven thousand lines of
/// semantics it shares.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=5; Fingerprint=7BBE7E
// Broiler-Falsified-If: emitted code is entered from anywhere but this file
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{
    /// <summary>Runs one activation of a unit by entering its emitted code.</summary>
    /// <remarks>
    /// <para>
    /// <b>THE ENTRY RUNS IN MANAGED CODE BEFORE ANY EMITTED FRAME EXISTS.</b> The dispatch loop's own
    /// prologue builds or borrows the stack and the scopes, finishes a normal resumption and raises an
    /// abrupt one - so a <c>throw()</c> or a <c>return()</c> into a suspended body lands in its regions
    /// or propagates out of this method exactly as the interpreter's would, and the emitted code is
    /// entered only at an instruction the managed side chose.
    /// </para>
    /// <para>
    /// <b>THE ACTIVATION IS ROOTED HERE FOR THE WHOLE EMITTED CALL.</b> The frame handed to the emitted
    /// code carries a table address and a cookie and nothing the collector traces; the activation it
    /// names is kept alive by this frame and by the thread slot it sets, and the slot is restored when
    /// the call returns, whether it returned or not, so a nested activation's slot never outlives it.
    /// </para>
    /// <para>
    /// <b>A caught exception is raised again with a plain throw.</b> Capturing and replaying its
    /// dispatch information would append a trace per native level to an exception that crosses many,
    /// and no guest can observe the trace.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=5; Fingerprint=8C7329
    // Broiler-Falsified-If: emitted code runs while its activation or its page is unreachable from a managed root, or this answers a value for a status other than exit
    // Broiler-Human:        PENDING
    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private unsafe JsValue RunNative(
        JsProgram program,
        int unitIndex,
        JsEnvironment? environment,
        JsValue thisValue,
        JsValue[] actualArguments,
        JsScriptFunction? self,
        JsValue newTarget,
        JsCell? thisBinding,
        JsFrame? frame)
    {
        var page = NativePageOf(program) ??
            throw new JsAbort(JsAbortKind.InternalDefect, "emitted code could not be mapped");

        var act = new JsNativeActivation(
            this, program, unitIndex, environment, thisValue, actualArguments, self, newTarget,
            thisBinding, frame);

        _ = ExecuteCore<JsNativeEntry>(
            program, unitIndex, environment, thisValue, actualArguments, self, newTarget,
            thisBinding, frame, act);

        JsBaselineFrame native;
        native.Handlers = JsBaselineHandlers.Table;
        native.Cookie = act.Cookie;

        var previous = JsNativeActivation.Current;
        JsNativeActivation.Current = act;
        int status;

        try
        {
            var entry = (delegate* unmanaged<JsBaselineFrame*, int, int>)page.At(
                program.NativeSymbols[unitIndex].Offset);

            status = entry(&native, act.Pc);
        }
        finally
        {
            JsNativeActivation.Current = previous;
            System.GC.KeepAlive(page);
            System.GC.KeepAlive(act);
        }

        return status switch
        {
            (int)JsBaselineStatus.Exit => act.Result,
            (int)JsBaselineStatus.Threw => throw act.Pending!,
            _ => throw new JsAbort(
                JsAbortKind.InternalDefect, "emitted code answered status " + status),
        };
    }

    /// <summary>The value form's words: every value-form frame of this instance, or nothing in another form.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=431645
    // Broiler-Falsified-If: a value-form engine has none, or two engines share one
    // Broiler-Human:        PENDING
    internal JsValueStack? ValueStack { get; }

    /// <summary>The value form's handle table, rooted by <see cref="ValueStack"/>, or nothing in another form.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=582BBE
    // Broiler-Falsified-If: a value-form engine has none, two engines share one, or its compactions scan any words but this engine's value stack
    // Broiler-Human:        PENDING
    internal JsHandleTable? ValueHandles { get; }

    /// <summary>Runs one activation of a unit by entering its value-form emitted code.</summary>
    /// <remarks>
    /// <para>
    /// <b>IT IS <see cref="RunNative"/> WITH A REGION.</b> The dispatch loop's own prologue runs first, in
    /// managed code, exactly as for the baseline form - a normal resumption's push and an abrupt one's
    /// raise and landing included - and only then is a region opened on the instance's value stack, the
    /// activation's stack encoded into it and published, and the emitted code entered with a frame of a
    /// helper-table address and a cookie (JSD-0035 sections 3 and 5).
    /// </para>
    /// <para>
    /// <b>THE REGION CLOSES WHEN THE CALL RETURNS, WHETHER IT RETURNED OR NOT</b>, and every nested
    /// activation's region closes in its own call before this one's does, so the stack's frames are
    /// pushed and popped in call order. A region that cannot be opened answers the call-depth backstop:
    /// the value stack is bounded, and past its bound the instance has recursed further than any verified
    /// program the interpreter's call-depth ceiling admits.
    /// </para>
    /// <para>
    /// <b>A caught exception is raised again with a plain throw</b>, for RunNative's reason. A throw that
    /// crosses several value-form frames entered here is a managed rethrow per level; one that crosses
    /// direct calls is a status per level, landed at each call without a throw (JSD-0035 section 8).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=5; Fingerprint=22D6B8
    // Broiler-Falsified-If: emitted code runs while its activation or its page is unreachable from a managed root, runs with no region or with a region some other activation holds, a region outlives the call that opened it, or this answers a value for a status other than exit
    // Broiler-Human:        PENDING
    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private unsafe JsValue RunValue(
        JsProgram program,
        int unitIndex,
        JsEnvironment? environment,
        JsValue thisValue,
        JsValue[] actualArguments,
        JsScriptFunction? self,
        JsValue newTarget,
        JsCell? thisBinding,
        JsFrame? frame)
    {
        var page = NativePageOf(program) ??
            throw new JsAbort(JsAbortKind.InternalDefect, "emitted code could not be mapped");

        var act = OpenValue(
            program, unitIndex, environment, thisValue, actualArguments, self, newTarget, thisBinding, frame);

        try
        {
            // THE FRAME LIVES IN THIS METHOD'S FRAME FOR THE WHOLE EMITTED CALL, and names nothing the
            // collector traces: the region's address is stable for the region's life, because the segment is
            // a pinned array and the region is popped in the finally below, after the emitted code returned.
            JsValueFrame native;
            Frame(act, &native);

            var previous = JsNativeActivation.Current;
            JsNativeActivation.Current = act;
            int status;

            try
            {
                var entry = (delegate* unmanaged<JsValueFrame*, int, int>)page.At(
                    program.NativeSymbols[unitIndex].Offset);

                status = entry(&native, act.Pc);
            }
            finally
            {
                JsNativeActivation.Current = previous;
                System.GC.KeepAlive(page);
                System.GC.KeepAlive(act);
            }

            return status switch
            {
                (int)JsBaselineStatus.Exit => act.Result,
                (int)JsBaselineStatus.Threw => throw act.Pending!,
                JsValueAbi.Returned => Returned(act, native.Debt),
                _ => throw new JsAbort(
                    JsAbortKind.InternalDefect, "emitted code answered status " + status),
            };
        }
        finally
        {
            CloseValue(act);
        }
    }

    /// <summary>
    /// What an inline return left: the value in the region's first word, decoded before anything can compact,
    /// and the debt it ran up charged; recorded on the activation as the interpreter's arm records it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=696FCF
    // Broiler-Falsified-If: it answers any value but the decoding of the region's first word, decodes after a safepoint, or answers before the debt is charged
    // Broiler-Human:        PENDING
    internal JsValue Returned(JsNativeActivation act, long debt)
    {
        var value = JsValueWindows.Returned(act);
        ChargeDebt(debt);
        act.Exit(value);
        return value;
    }

    /// <summary>
    /// An activation of a value-form unit with its region open: the dispatch loop's prologue run, a region
    /// pushed on the value stack, and its words encoded and published.
    /// </summary>
    /// <remarks>
    /// <b>SHARED BY THE TWO WAYS A VALUE-FORM UNIT IS ENTERED</b>: from managed code, by
    /// <see cref="RunValue"/>, and from another unit's emitted code, by a direct call (stage JSV-3). A region
    /// that cannot be pushed is the call-depth backstop; one whose encoding fails is closed before the failure
    /// goes on.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=FB7536
    // Broiler-Falsified-If: it answers an activation whose region is not the top of the value stack, is not encoded and published at the activation's height, or whose plan is not its unit's; or a failure leaves a region pushed
    // Broiler-Human:        PENDING
    private JsNativeActivation OpenValue(
        JsProgram program,
        int unitIndex,
        JsEnvironment? environment,
        JsValue thisValue,
        JsValue[] actualArguments,
        JsScriptFunction? self,
        JsValue newTarget,
        JsCell? thisBinding,
        JsFrame? frame)
    {
        var stack = ValueStack ??
            throw new JsAbort(JsAbortKind.InternalDefect, "a value-form program reached an engine with no value stack");

        // THE PLAN IS THE ONE THE PAYLOAD WAS SCANNED AGAINST: which region words are the arguments, the
        // resident bindings and the operand stack, and the height before every instruction.
        var plan = program.ValuePlan(unitIndex) ??
            throw new JsAbort(JsAbortKind.InternalDefect, "a value-form unit has no value plan");

        var act = new JsNativeActivation(
            this, program, unitIndex, environment, thisValue, actualArguments, self, newTarget,
            thisBinding, frame);

        if (frame is null)
        {
            // AN ORDINARY ENTRY IS THE DISPATCH LOOP'S PROLOGUE FOR NO FRAME, and nothing more: a fresh operand
            // stack of the unit's verified depth, a scope list holding the entered environment, and the unit's
            // first instruction - which is exactly what the entry instantiation hands the activation when it
            // is given no frame, without the loop's frame of its own around it.
            var unit = program.Functions[unitIndex];
            act.Stack = new JsValue[unit.MaxOperandStack + 1];
            act.Scopes = new System.Collections.Generic.List<JsEnvironment>(4) { environment! };
            act.Sp = 0;
            act.Pc = (int)unit.CodeOffset;
        }
        else
        {
            _ = ExecuteCore<JsNativeEntry>(
                program, unitIndex, environment, thisValue, actualArguments, self, newTarget,
                thisBinding, frame, act);
        }

        if (!stack.TryPush(plan.StackBase + act.Stack.Length, out var segment, out var slabFrame))
        {
            throw StackBackstopReached();
        }

        act.Segment = segment;
        act.SlabFrame = slabFrame;
        act.Plan = plan;

        try
        {
            JsValueWindows.Open(act, ValueHandles!);
        }
        catch
        {
            CloseValue(act);
            throw;
        }

        return act;
    }

    /// <summary>Closes the region <see cref="OpenValue"/> opened: popped from the value stack, and forgotten by the activation.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=C5E740
    // Broiler-Falsified-If: a region is popped other than in the reverse order regions were pushed, or an activation keeps a segment or a plan after its region is closed
    // Broiler-Human:        PENDING
    private void CloseValue(JsNativeActivation act)
    {
        var segment = act.Segment ??
            throw new JsAbort(JsAbortKind.InternalDefect, "a value-form region was closed twice");

        act.Segment = null;
        act.Plan = null;
        ValueStack!.Pop(segment, act.SlabFrame);
    }

    /// <summary>Fills a frame context for an activation whose region is open: the table, the cookie, the region and no debt.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=22D8FA
    // Broiler-Falsified-If: the context names a cookie other than the activation's, a region other than its own first word, a debt other than zero, or an entry
    // Broiler-Human:        PENDING
    private static unsafe void Frame(JsNativeActivation act, JsValueFrame* frame)
    {
        frame->Helpers = JsValueHelpers.Table;
        frame->Cookie = act.Cookie;
        frame->Region = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(
            ref act.Segment!.Words[JsValueSlab.FirstWord(act.SlabFrame)]);
        frame->Debt = 0;
        frame->Entry = 0;
        frame->EntryPc = 0;
    }

    // ---- the direct call (JSD-0035 section 6, stage JSV-3) -------------------------------------------

    /// <summary>
    /// Whether the <c>Call</c> at <paramref name="pc"/> names a function the caller can call directly: a plain
    /// script function - not a generator, not async, not a class constructor - of the caller's own program.
    /// </summary>
    /// <remarks>
    /// <b>IT READS ONE WORD AND CHANGES NOTHING</b>, so a call it answers false for runs through the
    /// instruction's own helper exactly as it did before direct calls existed. The same program is the same
    /// payload, so the callee's entry is one the scan held to the same layout; a function of another program
    /// - a nested load's, another script's - is called through the helper.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=35645C
    // Broiler-Falsified-If: it answers true for a callee that is not a plain script function of the caller's program, or changes a word, a handle or the meter
    // Broiler-Human:        PENDING
    internal bool TryDirectCallee(JsNativeActivation caller, int pc, out JsScriptFunction function)
    {
        function = null!;
        var plan = caller.Plan;
        var segment = caller.Segment;

        if (plan is null || segment is null || ValueHandles is not { } handles)
        {
            return false;
        }

        var slot = plan.HeightAt(pc) - caller.Code[pc + 1] - 2;

        if (slot < 0)
        {
            return false;
        }

        var word = segment.Words[JsValueSlab.FirstWord(caller.SlabFrame) + plan.StackBase + slot];

        if (!JsWord.IsHandle(word) ||
            !JsWordCodec.TryDecode(word, handles, out var value) ||
            !value.IsObject ||
            value.AsObject() is not JsScriptFunction script ||
            !ReferenceEquals(script.Program, caller.Program) ||
            script.IsClassConstructor)
        {
            return false;
        }

        var unit = script.Program.Functions[script.Unit];

        if (unit.IsGenerator || unit.IsAsync)
        {
            return false;
        }

        function = script;
        return true;
    }

    /// <summary>
    /// Everything the interpreter does for a <c>Call</c> of a plain script function before it runs the callee,
    /// for a call the emitted code makes directly: the instruction's charge, <see cref="Call"/>'s checks,
    /// <see cref="Invoke"/>'s record and receiver, <see cref="Execute"/>'s referrer and <see cref="RunValue"/>'s
    /// region; the callee's context filled for the call.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>EACH STEP IS THE INTERPRETER'S OWN METHOD, IN THE INTERPRETER'S ORDER</b>, after the one charge
    /// <see cref="DirectCallCharge"/> names, which the caller makes with its debt; so the stack
    /// probe, the <c>RangeError</c> at the counted bound, the call-depth ceiling and the backstop are the
    /// interpreter's, at the same point in the same call. What the interpreter's call adds on the machine
    /// stack - the arm's frame, the call, the invocation and the entry - a direct call does not: the callee's
    /// emitted frame is all a level costs, which is why a recursion reaches the counted bound long before the
    /// machine stack.
    /// </para>
    /// <para>
    /// <b>A FAILURE LEAVES NOTHING TAKEN</b>: the depth, the referrer and the region are given back before it
    /// goes on, so the caller raises it at the call as the interpreter's arm would have met it.
    /// </para>
    /// </remarks>
    /// <param name="caller">The activation making the call, entered at the call's instruction.</param>
    /// <param name="pc">The call's instruction.</param>
    /// <param name="height">The operand height before it, whose top words the window decoded.</param>
    /// <param name="function">The callee, as <see cref="TryDirectCallee"/> answered it.</param>
    /// <param name="context">The callee context the call site reserved.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=0002A2
    // Broiler-Falsified-If: a direct call charges, checks, binds or opens other than the interpreter's call of the same function would, fills an entry other than the callee unit's in the caller's own payload, or a failure leaves a depth, a referrer or a region taken
    // Broiler-Human:        PENDING
    internal unsafe JsNativeActivation BeginDirectCall(
        JsNativeActivation caller, int pc, int height, JsScriptFunction function, JsValueFrame* context)
    {
        var stack = caller.Stack;
        var argc = caller.Code[pc + 1];
        var sp = height;
        var arguments = argc == 0 ? System.Array.Empty<JsValue>() : new JsValue[argc];

        for (var at = argc - 1; at >= 0; at--)
        {
            arguments[at] = stack[--sp];
        }

        var receiver = stack[--sp];

        // THE CHARGE WAS MADE BY THE CALLER, WITH ITS DEBT (DirectCallCharge); what the interpreter's call
        // makes after its charge follows.
        EnterDepth();

        try
        {
            var program = function.Program;
            var unit = program.Functions[function.Unit];
            var environment = CallEnvironment(function, unit, arguments);
            var thisValue = CallReceiver(function, unit, receiver);

            if ((program.NativeCode.Length != 0) != nativeForm || program.NativeValueForm != valueForm)
            {
                throw new JsAbort(
                    JsAbortKind.InternalDefect, "a program of the other output form reached this engine");
            }

            var page = NativePageOf(program) ??
                throw new JsAbort(JsAbortKind.InternalDefect, "emitted code could not be mapped");

            var outerReferrer = activeReferrer;
            activeReferrer = function.ScriptOrModule ?? string.Empty;

            try
            {
                var callee = OpenValue(
                    program,
                    function.Unit,
                    environment,
                    thisValue,
                    arguments,
                    function,
                    unit.IsArrow ? function.LexicalNewTarget : JsValue.Undefined,
                    unit.IsArrow ? function.LexicalThisBinding : null,
                    null);

                callee.Caller = caller;
                callee.CallerReferrer = outerReferrer;
                Frame(callee, context);
                context->Entry = page.At(program.NativeSymbols[function.Unit].Offset);
                context->EntryPc = callee.Pc;
                return callee;
            }
            catch
            {
                activeReferrer = outerReferrer;
                throw;
            }
        }
        catch
        {
            LeaveCall();
            throw;
        }
    }

    /// <summary>
    /// Everything the interpreter does after a directly called function's emitted code returned, before the
    /// caller goes on: the region closed, the referrer restored and the depth given back.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=7C9518
    // Broiler-Falsified-If: a direct callee's region, referrer or depth outlives its return, or is given back twice
    // Broiler-Human:        PENDING
    internal void EndDirectCall(JsNativeActivation callee)
    {
        try
        {
            CloseValue(callee);
        }
        finally
        {
            activeReferrer = callee.CallerReferrer ?? string.Empty;
            callee.Caller = null;
            LeaveCall();
        }
    }


    /// <summary>The armed mapping of <paramref name="program"/>'s emitted code, mapped on first use.</summary>
    /// <remarks>
    /// <para>
    /// <b>THE PAGE BELONGS TO THE PROGRAM AND NOT TO THE INSTANCE.</b> Closures, suspended frames,
    /// module graphs and queued jobs all hold the program they run, so a page owned by the program
    /// lives exactly as long as anything could still enter it - and a program a guest loads with
    /// <c>eval</c> or an import gets its own page that goes when it does, instead of one mapping per
    /// load kept for the life of the instance.
    /// </para>
    /// <para>
    /// <b>Two threads may map the same program at once and one of them wins.</b> The winner's page
    /// is published atomically; the loser releases its own and uses the winner's, so a program is
    /// never seen with two pages and a published page is never replaced.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=4; Fingerprint=D236B5
    // Broiler-Falsified-If: a page this returns is not armed, or a program's published page is replaced or released while the program is reachable
    // Broiler-Human:        PENDING
    internal JsNativePage? NativePageOf(JsProgram program)
    {
        var published = program.NativePage;

        if (published is not null)
        {
            return published;
        }

        var mapped = JsNativePage.TryMap(program.NativeCode);

        if (mapped is null)
        {
            return null;
        }

        if (!mapped.Arm())
        {
            mapped.Dispose();
            return null;
        }

        var raced = System.Threading.Interlocked.CompareExchange(ref program.NativePage, mapped, null);

        if (raced is not null)
        {
            mapped.Dispose();
            return raced;
        }

        return mapped;
    }

    /// <summary>
    /// Refuses a guest-loaded program whose output form is not this instance's, and maps one that is.
    /// </summary>
    /// <remarks>
    /// <b>A NESTED LOAD IS THE ONE ROUTE BY WHICH A SECOND FORM COULD ENTER A RUNNING INSTANCE</b>,
    /// because the composition's provider compiled it after instantiation checked the first. A
    /// mismatch is this profile's defect or a provider's, never the guest's, so it is an internal
    /// defect rather than a language error. The page is mapped here as well, so a program that
    /// cannot be mapped is refused where it was loaded rather than at its first call.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=3; Fingerprint=D4C7D8
    // Broiler-Falsified-If: a guest-loaded program of the other form, of another architecture, or of the numeric manifest runs in a baseline instance
    // Broiler-Human:        PENDING
    private void RequireInstanceForm(JsProgram loaded)
    {
        if (JsNativeExecution.CarriesEmittedCode(loaded) != nativeForm ||
            loaded.NativeValueForm != valueForm ||
            (nativeForm &&
                (loaded.NativeArchitecture != JsNativeExecution.HostArchitecture ||
                    string.Equals(
                        loaded.ManifestId, JsNumericManifest.ManifestId, System.StringComparison.Ordinal))))
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect,
                "a guest-loaded program's output form differs from its instance's form");
        }

        if (nativeForm && NativePageOf(loaded) is null)
        {
            throw new JsAbort(JsAbortKind.InternalDefect, "emitted code could not be mapped");
        }
    }
}
