// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           28
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Critical
// Criteria:         24/24
// Resource impact:  5/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// One activation of a baseline-form code unit: everything a handler needs, held where the collector
/// can see it.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS IS WHERE THE EMITTED FRAME'S REFERENCES LIVE, AND THE EMITTED FRAME HOLDS NONE OF
/// THEM.</b> A baseline unit is entered with a frame of two integers - a table address and a
/// cookie - and every JavaScript value an instruction reads or writes is reached from here, by
/// managed code, after the cookie has been compared. The activation is rooted by the managed frame
/// that entered the emitted code and by the thread slot that frame set, both of which outlive the
/// emitted call, so the collector traces everything a handler can touch without knowing an emitted
/// frame exists.
/// </para>
/// <para>
/// <b>The entry values are kept exactly as the dispatch loop received them</b>, because a step is
/// that loop, run from the instruction it is handed, and reads the same parameters on every call: an
/// arm that reads the receiver, the arguments or the frame reads what the interpreter would have read.
/// </para>
/// <para>
/// <b>The thread slot is not an ambient holder, and the difference is its extent.</b> It is set by
/// the one method that enters emitted code, for exactly the duration of that call, and restored when
/// the call returns; it is read only by the handler wrapper, and only after the frame's cookie has
/// matched. Emitted code runs synchronously on the thread that entered it and never calls another
/// unit directly, so the innermost emitted frame on a thread always belongs to the activation in the
/// slot, and no other composition's code can observe it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=3; Fingerprint=7BCAA8
// Broiler-Falsified-If: a handler reaches an activation other than the one whose emitted frame is innermost on its thread, or an object a handler touches is reachable only from an emitted frame
// Broiler-Human:        PENDING
internal sealed unsafe class JsNativeActivation
{
    /// <summary>The last cookie handed out, process-wide.</summary>
    /// <remarks>
    /// <b>Incremented atomically and never reused</b>, so a frame carrying a cookie that matches an
    /// activation was built for that activation and for no earlier one that happened to occupy the
    /// same thread slot.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=2F3773
    // Broiler-Falsified-If: two activations are ever given the same cookie
    // Broiler-Human:        PENDING
    private static long cookies;

    /// <summary>The activation whose emitted code is innermost on this thread, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=1; Fingerprint=4421A1
    // Broiler-Falsified-If: this names an activation while no emitted code of that activation is on this thread's stack
    // Broiler-Human:        PENDING
    [System.ThreadStatic]
    private static JsNativeActivation? current;

    /// <summary>Creates the activation for one entry into a unit.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=3F3C6C
    // Broiler-Falsified-If: an activation is created with a cookie another activation already holds, or with entry values other than the ones the dispatch loop was called with
    // Broiler-Human:        PENDING
    internal JsNativeActivation(
        JsEngine engine,
        JsProgram program,
        int unitIndex,
        JsEnvironment? environment,
        JsValue thisValue,
        JsValue[] arguments,
        JsScriptFunction? self,
        JsValue newTarget,
        JsCell? thisBinding,
        JsFrame? frame)
    {
        Engine = engine;
        Program = program;
        UnitIndex = unitIndex;
        Code = program.Code;
        Cookie = System.Threading.Interlocked.Increment(ref cookies);
        Environment = environment;
        ThisValue = thisValue;
        Arguments = arguments;
        Self = self;
        NewTarget = newTarget;
        ThisBinding = thisBinding;
        Frame = frame;
    }

    /// <summary>The engine, and so the realm, the unit runs in.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=86B4E1
    // Broiler-Human:        PENDING
    internal readonly JsEngine Engine;

    /// <summary>The verified program the unit belongs to.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=265750
    // Broiler-Human:        PENDING
    internal readonly JsProgram Program;

    /// <summary>Which unit of <see cref="Program"/> is running.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=33D420
    // Broiler-Human:        PENDING
    internal readonly int UnitIndex;

    /// <summary>The program's code, which a handler reads its expected opcode from.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=975847
    // Broiler-Falsified-If: this is any array other than the program's own code section
    // Broiler-Human:        PENDING
    internal readonly byte[] Code;

    /// <summary>The identity a frame must carry for a handler to act on this activation.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=73BB14
    // Broiler-Falsified-If: a handler acts on this activation for a frame whose cookie differs
    // Broiler-Human:        PENDING
    internal readonly long Cookie;

    /// <summary>The environment the unit was entered with.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=081A05
    // Broiler-Human:        PENDING
    internal readonly JsEnvironment? Environment;

    /// <summary>The receiver the unit was entered with.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=C3E680
    // Broiler-Human:        PENDING
    internal readonly JsValue ThisValue;

    /// <summary>The actual arguments the unit was entered with.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A48201
    // Broiler-Human:        PENDING
    internal readonly JsValue[] Arguments;

    /// <summary>The closure being run, or nothing for a program body.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=F9A1B5
    // Broiler-Human:        PENDING
    internal readonly JsScriptFunction? Self;

    /// <summary>The <c>new.target</c> the unit was entered with.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=6546DB
    // Broiler-Human:        PENDING
    internal readonly JsValue NewTarget;

    /// <summary>The box a construction holds its <c>this</c> in, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=51EA57
    // Broiler-Human:        PENDING
    internal readonly JsCell? ThisBinding;

    /// <summary>The heap frame of a generator or async body, or nothing for an ordinary call.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7F5E13
    // Broiler-Human:        PENDING
    internal readonly JsFrame? Frame;

    /// <summary>The operand stack, set by the entry and shared by every step.</summary>
    /// <remarks>
    /// <b>These four are fields rather than properties because a step reads each of them for every
    /// step it runs</b>, and they are written by the dispatch loop's entry and step boundary only.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=5C6FE1
    // Broiler-Falsified-If: a step runs over a stack other than the one the entry built or borrowed from the frame
    // Broiler-Human:        PENDING
    internal JsValue[] Stack = [];

    /// <summary>The scope chain, set by the entry and shared by every step.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=0E6121
    // Broiler-Falsified-If: a step runs over a scope list other than the one the entry built or borrowed from the frame
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsEnvironment> Scopes = null!;

    /// <summary>The operand stack height at the step boundary.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=1FED1E
    // Broiler-Falsified-If: a step starts at a height other than the one the previous step or the entry stopped at
    // Broiler-Human:        PENDING
    internal int Sp;

    /// <summary>The instruction the next step must start at.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=1; Fingerprint=4E6E02
    // Broiler-Falsified-If: a handler starts a step at any offset other than this one
    // Broiler-Human:        PENDING
    internal int Pc;

    /// <summary>Whether the unit has left through a return or a suspension.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=05335F
    // Broiler-Falsified-If: a handler runs an instruction after the activation exited
    // Broiler-Human:        PENDING
    internal bool Exited;

    /// <summary>What the unit returned or suspended with.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=9AE3E3
    // Broiler-Human:        PENDING
    internal JsValue Result;

    /// <summary>The value-form segment this activation's region is in, or nothing outside the value form.</summary>
    /// <remarks>
    /// <b>Set by the value form's entry for exactly the life of its region</b>, and cleared when the region
    /// is closed, so a value step of an activation whose region is gone runs nothing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=61730A
    // Broiler-Falsified-If: a value step reads or writes words of a segment other than the one the activation's region was opened in, or runs after the region was closed
    // Broiler-Human:        PENDING
    internal JsValueSlab? Segment;

    /// <summary>The index of the activation's frame header in <see cref="Segment"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=A73741
    // Broiler-Falsified-If: this names any header other than the one the entry opened for this activation
    // Broiler-Human:        PENDING
    internal int SlabFrame;

    /// <summary>The value plan of the activation's unit, or nothing outside the value form.</summary>
    /// <remarks>
    /// <b>Set by the value form's entry with the region</b>: it says which region words are the arguments,
    /// the resident bindings and the operand stack, and the operand height before every instruction, which
    /// is how a helper knows the height inline code left, since inline code writes no managed state.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=3EC17C
    // Broiler-Falsified-If: a value step of this activation reads a plan other than the one of its own unit the template scan held the payload to
    // Broiler-Human:        PENDING
    internal Format.JsValueUnitPlan? Plan;

    /// <summary>Whether the last step ended at a landing: a caught throw or a caught forced return.</summary>
    /// <remarks>
    /// <b>The dispatch loop sets it where it lands, and only outside the interpreter's own
    /// instantiation.</b> A landing truncates the operand stack to the region's height and pushes one
    /// value there, which may be below the instruction's own inputs, so a value step reads it to know
    /// which words the step wrote.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3B3559
    // Broiler-Falsified-If: a step that landed leaves this false, or one that did not land leaves it true
    // Broiler-Human:        PENDING
    internal bool Landed;

    /// <summary>The exception a step caught at the wrapper, for the entering frame to raise.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=AE8477
    // Broiler-Falsified-If: an exception a step caught is dropped rather than raised by the managed frame that entered the emitted code
    // Broiler-Human:        PENDING
    internal System.Exception? Pending;

    /// <summary>The activation whose emitted code called this one's directly, or nothing (stage JSV-3).</summary>
    /// <remarks>
    /// <b>IT IS WHAT ROOTS A DIRECT CALL'S CALLER</b>: the thread slot names the innermost activation, and each
    /// directly called one names the one below it, down to the activation <c>RunValue</c> entered, which that
    /// method's frame holds; the finish helper follows it back when the callee returns.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=275E20
    // Broiler-Falsified-If: a direct callee runs with no caller named, or names one other than the activation whose call site entered it
    // Broiler-Human:        PENDING
    internal JsNativeActivation? Caller;

    /// <summary>The script or module a direct call's caller was running, which its return restores.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3F30FA
    // Broiler-Human:        PENDING
    internal string? CallerReferrer;

    /// <summary>The activation whose emitted code is innermost on this thread.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=1; Fingerprint=859662
    // Broiler-Falsified-If: this is written anywhere but around the one call that enters emitted code, or is not restored when that call returns
    // Broiler-Human:        PENDING
    internal static JsNativeActivation? Current
    {
        get => current;
        set => current = value;
    }

    /// <summary>Records that the unit left, and with what.</summary>
    /// <remarks>
    /// <para>
    /// <b>It answers <c>default</c> because the dispatch loop's return value is not how a step's
    /// answer travels.</b> The value stays here, on the managed side, and the handler answers only
    /// the status that tells the emitted code to leave.
    /// </para>
    /// <para>
    /// <b>It is inlined, because its callers are the dispatch loop's leaving arms.</b> Two field
    /// writes behind a call cost the loop the argument registers and the call's own shadow space at
    /// every one of those arms, and that is paid out of the frame the block instantiation reserves.
    /// Inlined, the arms write the two fields where they stand.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=13132A
    // Broiler-Human:        PENDING
    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    internal JsValue Exit(JsValue value)
    {
        Exited = true;
        Result = value;
        return default;
    }

    /// <summary>
    /// Runs one step for the emitted code - one instruction, or a block of them - after checking that
    /// it starts at the instruction the managed side expects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THREE CHECKS STAND BETWEEN AN EMITTED CALL AND THE INTERPRETER, AND ANY ONE OF THEM FAILING
    /// RUNS NOTHING.</b> The frame's cookie must be the activation's, the offset the emitted code
    /// passes must be the one the previous step or the entry computed, and the byte at that offset
    /// must be the opcode this handler was built for, which for a block step is the opcode at the
    /// block's head. The template scan holds every call of an x86-64 baseline payload to a block head of
    /// the program's partition and to eight times the opcode there, in every image, so a verified payload
    /// does not reach these checks failing; they stay as defence in depth, against a defect in the engine
    /// or in the scan. A misplaced call therefore answers a defect, never a different JavaScript answer.
    /// </para>
    /// <para>
    /// <b>NOTHING CROSSES BACK INTO THE EMITTED CODE AS AN EXCEPTION.</b> A managed exception cannot
    /// unwind through a frame the runtime did not build, so every exception a step raises - a guest
    /// throw no region covers, a forced return no finally covers, an exhausted allowance, a
    /// cancellation, a stack backstop - is caught here, parked on the activation, and raised again
    /// by the managed frame that entered the emitted code once that code has returned. The catch
    /// completes before the emitted frames return, so handlers never accumulate across nested
    /// activations the way a rethrow per frame would.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=5; Fingerprint=EAD7FE
    // Broiler-Falsified-If: a step starts at an offset, with an opcode or for an activation other than what the managed side computed, or an exception escapes into emitted code
    // Broiler-Human:        PENDING
    internal static int Step<TMode>(JsBaselineFrame* frame, int pc, JsOpcode expected)
        where TMode : struct, IJsExecutionMode
    {
        var act = current;

        if (act is null ||
            frame is null ||
            act.Cookie != frame->Cookie ||
            act.Exited ||
            act.Pc != pc ||
            (uint)pc >= (uint)act.Code.Length ||
            act.Code[pc] != (byte)expected)
        {
            return (int)JsBaselineStatus.Defect;
        }

        try
        {
            _ = act.Engine.ExecuteCore<TMode>(
                act.Program,
                act.UnitIndex,
                act.Environment,
                act.ThisValue,
                act.Arguments,
                act.Self,
                act.NewTarget,
                act.ThisBinding,
                act.Frame,
                act);

            return act.Exited ? (int)JsBaselineStatus.Exit : act.Pc;
        }
        catch (System.Exception escaped)
        {
            act.Pending = escaped;
            return (int)JsBaselineStatus.Threw;
        }
    }

    /// <summary>
    /// Runs one instruction for value-form emitted code, over the decoding of its input words, after the
    /// checks <see cref="Step{TMode}"/> makes (JSD-0035 section 5).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE PROGRAM COUNTER IS CHECKED AGAINST THE PLAN, NOT AGAINST THE LAST STEP.</b> Inline templates
    /// run instructions no managed code sees, so the offset the emitted code passes is not the one the last
    /// helper answered; it must be an instruction start of the activation's own unit that the plan reached,
    /// the byte there must be this helper's opcode, and the activation must hold an open region. Any one of
    /// them failing runs nothing and answers a defect.
    /// </para>
    /// <para>
    /// <b>THE DEBT IS CHARGED BEFORE ANYTHING RUNS</b>: the pure instructions the emitted code ran since the
    /// last settlement are charged to the meter with the existing charge, and an allowance that cannot take
    /// them ends the operation here, before the helper's own instruction - which is where the interpreter
    /// would have run out, give or take pure instructions nothing observes (JSD-0035 section 7). Then the
    /// window is decoded from the region, the per-opcode instantiation of the dispatch loop runs the one
    /// instruction exactly as the interpreter would, and what it left is encoded back and published.
    /// </para>
    /// <para>
    /// <b>NOTHING CROSSES BACK INTO THE EMITTED CODE AS AN EXCEPTION</b>, for Step's reason: a defect the
    /// codec reports, a guest throw no region covers and an exhausted allowance are all parked on the
    /// activation and raised by the managed frame that entered the emitted code.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=5; Fingerprint=D0E8EF
    // Broiler-Falsified-If: a value step starts at an offset that is not a reached instruction start of its activation's unit, with an opcode or for an activation other than the emitted code's own, runs with no open region, runs before its debt is charged, leaves a word the arm wrote unencoded or unpublished, or lets an exception escape into emitted code
    // Broiler-Human:        PENDING
    internal static int StepValue<TMode>(JsValueFrame* frame, int pc, JsOpcode expected)
        where TMode : struct, IJsExecutionMode
    {
        var act = current;

        if (act is null ||
            frame is null ||
            act.Cookie != frame->Cookie ||
            act.Exited ||
            (uint)pc >= (uint)act.Code.Length ||
            act.Code[pc] != (byte)expected ||
            act.Segment is null ||
            act.Plan is null)
        {
            return (int)JsBaselineStatus.Defect;
        }

        if (act.Plan.HeightAt(pc) < 0)
        {
            return (int)JsBaselineStatus.Defect;
        }

        try
        {
            var debt = frame->Debt;
            frame->Debt = 0;
            act.Engine.ChargeDebt(debt);

            var pops = JsValueWindows.Enter(act, pc, out var height);
            act.Landed = false;

            _ = act.Engine.ExecuteCore<TMode>(
                act.Program,
                act.UnitIndex,
                act.Environment,
                act.ThisValue,
                act.Arguments,
                act.Self,
                act.NewTarget,
                act.ThisBinding,
                act.Frame,
                act);

            if (act.Exited)
            {
                // A FRAME THAT SUSPENDED TAKES ITS RESIDENT WORDS WITH IT (stage JSV-4): its stack is already
                // the frame's, decoded whole by the window, and the codec puts the rest into its records.
                if (act.Frame is { Suspended: true })
                {
                    JsValueWindows.Suspend(act, pc);
                }

                return (int)JsBaselineStatus.Exit;
            }

            JsValueWindows.Leave(act, pc, height, pops);
            return act.Pc;
        }
        catch (System.Exception escaped)
        {
            act.Pending = escaped;
            return (int)JsBaselineStatus.Threw;
        }
    }

    /// <summary>
    /// Charges the debt value-form emitted code carried to a debt test, and answers the offset it resumes at
    /// (JSD-0035 section 7, clause V5).
    /// </summary>
    /// <remarks>
    /// <b>IT RUNS NO INSTRUCTION AND TOUCHES NO WORD.</b> It makes the value step's checks - the cookie, a
    /// live activation, an offset that is a reached instruction start of its unit, an open region - and then
    /// charges the debt, which is where cancellation and the wall clock are polled on a loop of pure
    /// instructions; an allowance that cannot take it is parked as the exhaustion it is.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=2A7F11
    // Broiler-Falsified-If: a settlement answers an offset other than the one it was handed, runs an instruction, writes a word, or leaves a debt it was handed uncharged without parking the refusal
    // Broiler-Human:        PENDING
    internal static int SettleValue(JsValueFrame* frame, int pc)
    {
        var act = current;

        if (act is null ||
            frame is null ||
            act.Cookie != frame->Cookie ||
            act.Exited ||
            act.Segment is null ||
            act.Plan is null)
        {
            return (int)JsBaselineStatus.Defect;
        }

        if (act.Plan.HeightAt(pc) < 0)
        {
            return (int)JsBaselineStatus.Defect;
        }

        try
        {
            var debt = frame->Debt;
            frame->Debt = 0;
            act.Engine.ChargeDebt(debt);
            return pc;
        }
        catch (System.Exception escaped)
        {
            act.Pending = escaped;
            return (int)JsBaselineStatus.Threw;
        }
    }

    /// <summary>
    /// The call-prepare helper's work at a direct call site: the <c>Call</c> run through its own helper, or a
    /// direct call prepared - the callee's activation made, its region opened, its context filled - and the
    /// thread slot moved to it (JSD-0035 section 6, stage JSV-3).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>IT MAKES <see cref="StepValue{TMode}"/>'S CHECKS FIRST</b>, and a callee that is not a plain script
    /// function of the caller's program is the ordinary helper's to call, through <see cref="StepValue{TMode}"/>
    /// itself, with nothing done before it. A direct call charges the debt, decodes the call's window as that
    /// step would, and then does exactly what the interpreter's <c>Call</c> does before it runs a callee.
    /// </para>
    /// <para>
    /// <b>WHAT FAILS BEFORE THE CALLEE RUNS IS RAISED AT THE CALL</b>, so the <c>RangeError</c> of a recursion
    /// past the counted bound lands in the caller's own regions exactly as the interpreter's does, and what
    /// lands nowhere is parked for the caller's caller.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=F68147
    // Broiler-Falsified-If: a direct call is prepared for a call site whose checks fail, for a callee the ordinary helper should call, without its debt charged, or answers the direct call with the thread slot naming anything but the prepared callee; or anything but the direct call escapes into emitted code
    // Broiler-Human:        PENDING
    internal static int PrepareCall<TMode>(JsValueFrame* frame, int pc, JsValueFrame* context)
        where TMode : struct, IJsExecutionMode
    {
        var act = current;

        if (act is null ||
            frame is null ||
            context is null ||
            act.Cookie != frame->Cookie ||
            act.Exited ||
            (uint)pc >= (uint)act.Code.Length ||
            act.Code[pc] != (byte)JsOpcode.Call ||
            act.Segment is null ||
            act.Plan is null)
        {
            return (int)JsBaselineStatus.Defect;
        }

        if (act.Plan.HeightAt(pc) < 0)
        {
            return (int)JsBaselineStatus.Defect;
        }

        if (!act.Engine.TryDirectCallee(act, pc, out var function))
        {
            return StepValue<TMode>(frame, pc, JsOpcode.Call);
        }

        var height = -1;
        var pops = 0;

        try
        {
            var debt = frame->Debt;
            frame->Debt = 0;
            act.Engine.ChargeDebt(debt + JsEngine.DirectCallCharge);

            pops = JsValueWindows.Enter(act, pc, out height);
            act.Landed = false;

            current = act.Engine.BeginDirectCall(act, pc, height, function, context);
            return JsValueAbi.DirectCall;
        }
        catch (System.Exception escaped)
        {
            return RaiseAt(act, pc, height, pops, escaped);
        }
    }

    /// <summary>
    /// The call-finish helper's work after a directly called function's emitted code returned: the thread slot
    /// moved back to the caller, the callee's region closed, and its answer pushed or its exception raised at
    /// the call (stage JSV-3).
    /// </summary>
    /// <remarks>
    /// <b>THE CALLEE IS THE THREAD SLOT'S, AND ITS CALLER IS THE ONE IT NAMES</b>: each must carry the cookie
    /// of the context it was handed, and the caller must still be at the call's instruction, or nothing runs
    /// and the answer is a defect. A returned value is written where the interpreter's arm writes it, and the
    /// caller goes on at the instruction after the call.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=C2D3A2
    // Broiler-Falsified-If: it acts for a callee or a caller whose cookie is not its context's, leaves the thread slot naming the callee, leaves the callee's region, depth or referrer taken, writes the answer anywhere but the call's own output slot, or lets an exception escape into emitted code
    // Broiler-Human:        PENDING
    internal static int FinishCall(JsValueFrame* frame, int pc, JsValueFrame* context, int status)
    {
        var callee = current;

        if (callee is null ||
            frame is null ||
            context is null ||
            callee.Cookie != context->Cookie ||
            callee.Caller is not { } act ||
            act.Cookie != frame->Cookie ||
            act.Exited ||
            act.Pc != pc ||
            act.Plan is null ||
            act.Segment is null)
        {
            return (int)JsBaselineStatus.Defect;
        }

        current = act;

        var height = act.Plan.HeightAt(pc);
        var pops = act.Code[pc + 1] + 2;

        try
        {
            var debt = context->Debt;
            context->Debt = 0;

            try
            {
                // AN INLINE RETURN LEFT ITS VALUE IN THE CALLEE'S FIRST WORD, which is read before the region
                // closes and before anything can compact, and its debt, which is charged before the caller
                // sees the value.
                if (status == JsValueAbi.Returned)
                {
                    _ = act.Engine.Returned(callee, debt);
                    status = (int)JsBaselineStatus.Exit;
                }
                else
                {
                    act.Engine.ChargeDebt(debt);
                }
            }
            finally
            {
                act.Engine.EndDirectCall(callee);
            }

            if (status == (int)JsBaselineStatus.Exit && callee.Exited)
            {
                var slot = height - pops;
                act.Stack[slot] = callee.Result;
                act.Sp = slot + 1;
                act.Pc = pc + 2;
                act.Landed = false;
                JsValueWindows.Leave(act, pc, height, pops);
                return act.Pc;
            }

            return status == (int)JsBaselineStatus.Threw
                ? RaiseAt(act, pc, height, pops, callee.Pending ??
                    new JsAbort(JsAbortKind.InternalDefect, "a direct callee threw nothing"))
                : (int)JsBaselineStatus.Defect;
        }
        catch (System.Exception escaped)
        {
            act.Pending = escaped;
            return (int)JsBaselineStatus.Threw;
        }
    }

    /// <summary>
    /// Raises an exception at a direct call's instruction: landed in the caller's own region the interpreter's
    /// filter would pick, through the dispatch loop's own landing and with no throw, or parked for the caller's
    /// caller as a status (JSD-0035 section 8, stage JSV-4).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=8E4FA9
    // Broiler-Falsified-If: an exception lands where the interpreter's filter would not land it, one it would land is parked, it is thrown again to land, or a landing's words are left unencoded or unpublished
    // Broiler-Human:        PENDING
    private static int RaiseAt(JsNativeActivation act, int pc, int height, int pops, System.Exception raised)
    {
        try
        {
            if (height < 0 || !JsEngine.TryLand(act, pc, raised))
            {
                act.Pending = raised;
                return (int)JsBaselineStatus.Threw;
            }

            JsValueWindows.Leave(act, pc, height, pops);
            return act.Pc;
        }
        catch (System.Exception escaped)
        {
            act.Pending = escaped;
            return (int)JsBaselineStatus.Threw;
        }
    }
}
