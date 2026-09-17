// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           22
// Human-reviewed:   0/4
// IP risk:          None
// Security risk:    Critical
// Criteria:         14/14
// Resource impact:  5/10 max
// Unverified:       4
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

    /// <summary>The exception a step caught at the wrapper, for the entering frame to raise.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=AE8477
    // Broiler-Falsified-If: an exception a step caught is dropped rather than raised by the managed frame that entered the emitted code
    // Broiler-Human:        PENDING
    internal System.Exception? Pending;

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
    /// <b>It answers <c>default</c> because the dispatch loop's return value is not how a step's
    /// answer travels.</b> The value stays here, on the managed side, and the handler answers only
    /// the status that tells the emitted code to leave.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E56FC1
    // Broiler-Human:        PENDING
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
}
