// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   25
// Annotated:        25/25
// Exempt:           27
// Human-reviewed:   0/25
// IP risk:          Low
// Security risk:    Critical
// Criteria:         15/15
// Resource impact:  9/10 max
// Unverified:       25
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// One label on a frame's label stack: where a branch to it goes, how many values it carries, and
/// the operand height it truncates to.
/// </summary>
/// <remarks>
/// <b>A BRANCH IS A HEIGHT TRUNCATION PLUS A JUMP, AND THESE THREE NUMBERS ARE WHAT MAKES THAT
/// TRUE.</b> Nothing is searched for at run time: the target came out of the jump table validation
/// built, the arity came out of the block type validation read, and the height was recorded when the
/// block was entered. <see cref="IsLoop"/> is the fourth field because a branch to a loop re-enters
/// it and leaves its label standing, where a branch to anything else leaves the block and takes its
/// label with it.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A993B6
// Broiler-Falsified-If: a branch scans the body for a matching end rather than reading a target from here
// Broiler-Human:        PENDING
internal struct WasmLabel
{
    /// <summary>Where execution continues when this label is branched to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=541FB7
    // Broiler-Human:        PENDING
    public int Target;

    /// <summary>How many values a branch to this label carries across.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4CC92B
    // Broiler-Human:        PENDING
    public int Arity;

    /// <summary>The operand height, relative to the frame's base, that a branch truncates to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=258D84
    // Broiler-Human:        PENDING
    public int Height;

    /// <summary>Whether a branch to this label re-enters a loop rather than leaving a block.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B2A526
    // Broiler-Human:        PENDING
    public bool IsLoop;
}

/// <summary>
/// One activation: heap-allocated, so guest call depth never grows the CLR stack.
/// </summary>
/// <remarks>
/// <b>THE FRAME IS AN OBJECT AND NOT A CLR STACK FRAME, AND THAT IS THE ROW SECTION 14 FORCES.</b> A
/// frame model living on the CLR stack cannot later be moved to the heap without rewriting the
/// interpreter, so it is on the heap from the first line. Two consequences follow today, before
/// anything captures a frame: a guest cannot overflow the CLR stack however deep it calls, and the
/// only thing bounding call depth is the meter, which is why exhausting it is a resource exhaustion
/// naming a dimension rather than a process that stopped.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=7EAD83
// Broiler-Falsified-If: guest call depth grows the CLR stack, or a frame outlives the run that made it
// Broiler-Human:        PENDING
internal sealed class WasmFrame
{
    /// <summary>The body being executed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=38710E
    // Broiler-Human:        PENDING
    internal WasmFunctionBody Body = null!;

    /// <summary>The function's declared type, which fixes its parameter and result counts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B92A8D
    // Broiler-Human:        PENDING
    internal WasmFuncType Signature = null!;

    /// <summary>Which function this is, for a trap's position.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=64FB0C
    // Broiler-Human:        PENDING
    internal int FunctionIndex;

    /// <summary>Parameters then declared locals, the parameters copied in from the caller.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=71D853
    // Broiler-Human:        PENDING
    internal WasmValue[] Locals = [];

    /// <summary>This frame's label stack, sized from the bound validation computed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B83CC5
    // Broiler-Human:        PENDING
    internal WasmLabel[] Labels = [];

    /// <summary>How many labels are open.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D368C8
    // Broiler-Human:        PENDING
    internal int LabelCount;

    /// <summary>Where in the body execution stands.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4E6E02
    // Broiler-Human:        PENDING
    internal int Pc;

    /// <summary>Where this frame's operands begin on the shared stack.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=205A3C
    // Broiler-Human:        PENDING
    internal int StackBase;
}

/// <summary>
/// The charge-and-poll pacing every execution path shares.
/// </summary>
/// <remarks>
/// <b>THE POLL GOES BEFORE THE CHARGE THAT WOULD CROSS THE BOUND, NOT AFTER A FIXED COUNT.</b> The
/// core measures uncharged work as fuel charged since the last poll and reports a profile fault when
/// it exceeds the declared bound, so a profile that polled every N instructions and then charged a
/// proportional cost would breach the bound with one instruction. <see cref="TryReserve"/> is how a
/// caller that is about to make a large proportional charge buys the headroom for it first.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=A58F73
// Broiler-Falsified-If: fuel charged between two polls can exceed the declared uncharged-work bound
// Broiler-Human:        PENDING
internal sealed class WasmPacing
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D9AC66
    // Broiler-Human:        PENDING
    private readonly IVmMeter meter;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=ACABAB
    // Broiler-Human:        PENDING
    private readonly ulong bound;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2AB034
    // Broiler-Human:        PENDING
    private ulong sinceLastPoll;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DA7E8B
    // Broiler-Human:        PENDING
    internal WasmPacing(IVmMeter contractMeter, ulong unchargedWorkBound)
    {
        meter = contractMeter;
        bound = unchargedWorkBound;
        Failure = WasmRunStatus.Completed;
    }

    /// <summary>The meter every charge goes through.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2CF889
    // Broiler-Human:        PENDING
    internal IVmMeter Meter => meter;

    /// <summary>Why the last refusal happened, when one did.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=076D1C
    // Broiler-Human:        PENDING
    internal WasmRunStatus Failure { get; private set; }

    /// <summary>Buys polling headroom for a charge of at most <paramref name="worstCase"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=624E86
    // Broiler-Falsified-If: it returns true while the bound could still be crossed by the charge it was asked about
    // Broiler-Human:        PENDING
    internal bool TryReserve(ulong worstCase)
    {
        if (sinceLastPoll + worstCase <= bound)
        {
            return true;
        }

        if (!meter.Poll())
        {
            // A refused poll is a cancellation or a wall-clock ceiling; the meter latched whichever
            // it was and the core's own precedence decides which the caller is told about.
            Failure = WasmRunStatus.Cancelled;
            return false;
        }

        sinceLastPoll = 0;
        return true;
    }

    /// <summary>Charges fuel, polling first if this charge would cross the bound.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=4DB319
    // Broiler-Falsified-If: a charge is committed without the poll that its size demanded
    // Broiler-Human:        PENDING
    internal bool TryCharge(ulong amount)
    {
        if (!TryReserve(amount))
        {
            return false;
        }

        if (!meter.TryCharge(VmBudgetDimension.Fuel, amount))
        {
            Failure = WasmRunStatus.Exhausted;
            return false;
        }

        sinceLastPoll += amount;
        return true;
    }

    /// <summary>Records fuel another party charged, so the poll bound stays honest.</summary>
    /// <remarks>
    /// A memory growth charges its own proportional fuel inside the memory, because the charge has
    /// to sit between the profile ceiling that refuses without spending and the allocation that must
    /// not happen unless the charge took. This is how that charge gets back into the pacing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6ABDC7
    // Broiler-Falsified-If: a charge made elsewhere never reaches this counter
    // Broiler-Human:        PENDING
    internal void Observe(ulong amount) => sinceLastPoll += amount;
}

/// <summary>
/// The interpreter: a switch over the code bytes with an explicit program counter, a heap frame
/// stack, and a per-frame label stack.
/// </summary>
/// <remarks>
/// <para>
/// <b>NOTHING HERE THROWS AND NOTHING HERE CATCHES.</b> A trap is a return code, an exhausted budget
/// is a return code, and a cancellation is a return code. The only exceptions this loop could raise
/// are the ones a defect in it would raise - an index outside an array, a null body - and the
/// executor's own wrapper turns those into a contract violation rather than letting them reach the
/// core.
/// </para>
/// <para>
/// <b>Every branch is a truncation and a jump, and every jump target came from validation.</b> The
/// bytes of a body carry no back pointer, so an interpreter without a resolved jump table would walk
/// forward over the remaining instructions - decoding immediates to skip them correctly - every time
/// a branch was taken, turning a loop with a conditional exit into a quadratic walk a guest chooses
/// the cost of. Validation already walked the body once and already knew which opening instruction
/// each <c>end</c> closed, so the pairing is read here rather than recomputed.
/// </para>
/// <para>
/// <b>Dead code is never entered, which is why this loop does not implement it.</b> Every route into
/// unreachable code passes through a branch, a return, or the <c>unreachable</c> instruction; the
/// first two jump over it and the third traps. The validator type-checks it polymorphically because
/// the specification says a module is invalid if it does not check out; the interpreter does not
/// execute it because nothing can reach it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=8; Fingerprint=03489B
// Broiler-Falsified-If: any input makes a member here throw, or a trap leaves as anything but a return code, or an operand is read past the height validation computed
// Broiler-Human:        PENDING
internal sealed class WasmInterpreter
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9E32C1
    // Broiler-Human:        PENDING
    private readonly WasmModule module;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C6A8CC
    // Broiler-Human:        PENDING
    private readonly WasmStore store;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2D75B2
    // Broiler-Human:        PENDING
    private readonly WasmPacing pacing;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=06622E
    // Broiler-Human:        PENDING
    private WasmValue[] stack;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=75ECBA
    // Broiler-Human:        PENDING
    private WasmFrame[] frames;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4CE248
    // Broiler-Human:        PENDING
    private int top;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F0A4BB
    // Broiler-Human:        PENDING
    private int frameCount;

    /// <summary>Builds an interpreter over one instance's module and store.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=48BD3D
    // Broiler-Human:        PENDING
    internal WasmInterpreter(WasmModule instantiated, WasmStore allocated, WasmPacing pace)
    {
        module = instantiated;
        store = allocated;
        pacing = pace;
        stack = new WasmValue[InitialStackSlots];
        frames = new WasmFrame[InitialFrames];
        TrapKind = WasmTrapKind.Unreachable;
        TrapFunctionIndex = -1;
        TrapOffset = -1;
    }

    /// <summary>How many operand slots the first stack holds before it has to grow.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=17AD62
    // Broiler-Human:        PENDING
    private const int InitialStackSlots = 256;

    /// <summary>How many frames the first frame stack holds before it has to grow.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AADA3A
    // Broiler-Human:        PENDING
    private const int InitialFrames = 32;

    /// <summary>Which trap ended the run, when one did.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=FEF59D
    // Broiler-Human:        PENDING
    internal WasmTrapKind TrapKind { get; private set; }

    /// <summary>Which function the trap happened in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=549FFB
    // Broiler-Human:        PENDING
    internal int TrapFunctionIndex { get; private set; }

    /// <summary>The trapping instruction's offset inside its own body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7AD518
    // Broiler-Human:        PENDING
    internal int TrapOffset { get; private set; }

    /// <summary>
    /// Calls one function with the arguments given and answers what the run ended as.
    /// </summary>
    /// <remarks>
    /// On a completed run the results are the bottom <paramref name="resultCount"/> slots of this
    /// interpreter's own stack, which <see cref="ResultAt"/> reads. They are left there rather than
    /// copied out so that the caller decides what shape to build from them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=9994B7
    // Broiler-Falsified-If: it answers completed while the operand stack does not hold exactly the declared results
    // Broiler-Human:        PENDING
    internal WasmRunStatus Call(
        int functionIndex, System.ReadOnlySpan<WasmValue> arguments, out int resultCount)
    {
        resultCount = 0;
        top = 0;
        frameCount = 0;

        EnsureStack(arguments.Length);

        for (var index = 0; index < arguments.Length; index++)
        {
            stack[top++] = arguments[index];
        }

        if (!TryPushFrame(functionIndex, out var pushFailure))
        {
            return pushFailure;
        }

        var status = Run();

        if (status is WasmRunStatus.Completed)
        {
            resultCount = top;
            return status;
        }

        // A run that did not complete leaves its activations standing, and the depth they charged
        // has to come back before the next one starts: a trap is not a leak.
        while (frameCount > 0)
        {
            frameCount--;
            pacing.Meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }

        return status;
    }

    /// <summary>Reads one of the results a completed run left behind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AC79FD
    // Broiler-Human:        PENDING
    internal WasmValue ResultAt(int index) => stack[index];

    /// <summary>Grows the operand stack to hold at least this many slots.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=981C65
    // Broiler-Human:        PENDING
    private void EnsureStack(int slots)
    {
        if (stack.Length >= slots)
        {
            return;
        }

        var grown = stack.Length;

        while (grown < slots)
        {
            grown *= 2;
        }

        System.Array.Resize(ref stack, grown);
    }

    /// <summary>
    /// Pushes one activation, moving its arguments off the caller's stack into its locals.
    /// </summary>
    /// <remarks>
    /// <b>THE CALL-DEPTH CHARGE IS THE ONLY THING BOUNDING RECURSION HERE.</b> Nothing about a guest
    /// call reaches the CLR stack, so a refused charge is what stops a runaway - and a refused charge
    /// latches on the meter, so the core reports it as a resource exhaustion naming call depth and a
    /// scope rather than as a trap or as a process that stopped.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=CBF744
    // Broiler-Falsified-If: a frame is pushed without a call-depth charge taking, or arguments are read below the caller's own base
    // Broiler-Human:        PENDING
    private bool TryPushFrame(int functionIndex, out WasmRunStatus failure)
    {
        failure = WasmRunStatus.Completed;

        if (!pacing.Meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            failure = WasmRunStatus.Exhausted;
            return false;
        }

        var body = module.Bodies[functionIndex];
        var signature = module.Types[(int)module.FunctionTypeIndices[functionIndex]];
        var parameterCount = signature.ParameterCount;

        if (top - parameterCount < 0)
        {
            failure = WasmRunStatus.Defect;
            return false;
        }

        if (frameCount == frames.Length)
        {
            System.Array.Resize(ref frames, frames.Length * 2);
        }

        var frame = frames[frameCount] ??= new WasmFrame();

        frame.Body = body;
        frame.Signature = signature;
        frame.FunctionIndex = functionIndex;
        frame.Pc = 0;

        var localCount = parameterCount + body.LocalCount;

        if (frame.Locals.Length < localCount)
        {
            frame.Locals = new WasmValue[localCount];
        }

        top -= parameterCount;

        for (var index = 0; index < parameterCount; index++)
        {
            frame.Locals[index] = stack[top + index];
        }

        for (var index = parameterCount; index < localCount; index++)
        {
            frame.Locals[index] = WasmValue.Zero;
        }

        frame.StackBase = top;
        EnsureStack(top + body.MaxOperandStack + 1);

        if (frame.Labels.Length < body.MaxLabelDepth)
        {
            frame.Labels = new WasmLabel[body.MaxLabelDepth];
        }

        frame.Labels[0] = new WasmLabel
        {
            Target = body.CodeLength,
            Arity = signature.ResultCount,
            Height = 0,
            IsLoop = false,
        };

        frame.LabelCount = 1;
        frameCount++;
        return true;
    }

    /// <summary>
    /// Pops the top activation, leaving its declared results on the caller's stack.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=99DB03
    // Broiler-Falsified-If: it leaves anything on the stack other than exactly the declared results
    // Broiler-Human:        PENDING
    private void PopFrame(WasmFrame frame)
    {
        var results = frame.Signature.ResultCount;
        var from = top - results;

        for (var index = 0; index < results; index++)
        {
            stack[frame.StackBase + index] = stack[from + index];
        }

        top = frame.StackBase + results;
        frameCount--;
        pacing.Meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
    }

    /// <summary>Records a trap and answers with the status that carries it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4E23B3
    // Broiler-Human:        PENDING
    private WasmRunStatus Trapped(WasmTrapKind kind, WasmFrame frame, int at)
    {
        TrapKind = kind;
        TrapFunctionIndex = frame.FunctionIndex;
        TrapOffset = at;
        return WasmRunStatus.Trapped;
    }

    /// <summary>The dispatch loop.</summary>
    /// <remarks>
    /// The outer loop owns the activation and the inner loop owns the program counter. A call ends
    /// the inner loop so the outer one picks the new activation up; a return ends it so the outer one
    /// picks the caller back up; everything else stays inside it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=9; Fingerprint=599992
    // Broiler-Falsified-If: an opcode validation admits is not handled here, or a handled opcode reads a different immediate shape than validation read
    // Broiler-Human:        PENDING
    private WasmRunStatus Run()
    {
        while (frameCount > 0)
        {
            var frame = frames[frameCount - 1];
            var code = frame.Body.Code;
            var pc = frame.Pc;
            var running = true;

            while (running)
            {
                if (pc >= code.Length)
                {
                    PopFrame(frame);
                    running = false;
                    continue;
                }

                if (!pacing.TryCharge(1))
                {
                    frame.Pc = pc;
                    return pacing.Failure;
                }

                var at = pc;
                var opcode = code[pc];
                pc++;

                switch ((WasmOpcode)opcode)
                {
                    case WasmOpcode.Unreachable:
                        frame.Pc = pc;
                        return Trapped(WasmTrapKind.Unreachable, frame, at);

                    case WasmOpcode.Nop:
                        continue;

                    case WasmOpcode.Block:
                    case WasmOpcode.Loop:
                    case WasmOpcode.If:
                    {
                        SkipVarInt(code, ref pc);

                        if (!frame.Body.TryFindJumpTarget(at, out var target))
                        {
                            return WasmRunStatus.Defect;
                        }

                        if ((WasmOpcode)opcode is WasmOpcode.If)
                        {
                            var condition = stack[--top].I32;

                            if (condition == 0 && target.ElseOffset < 0)
                            {
                                pc = target.EndOffset;
                                continue;
                            }

                            PushLabel(frame, target.EndOffset, target.LabelArity, false);
                            pc = condition != 0 ? pc : target.ElseOffset;
                            continue;
                        }

                        if ((WasmOpcode)opcode is WasmOpcode.Loop)
                        {
                            PushLabel(frame, pc, 0, true);
                            continue;
                        }

                        PushLabel(frame, target.EndOffset, target.LabelArity, false);
                        continue;
                    }

                    case WasmOpcode.Else:
                    {
                        // The then-arm ran to its end, so the alternative is skipped and the block's
                        // label goes with it.
                        if (!frame.Body.TryFindJumpTarget(at, out var target))
                        {
                            return WasmRunStatus.Defect;
                        }

                        frame.LabelCount--;
                        pc = target.EndOffset;
                        continue;
                    }

                    case WasmOpcode.End:
                        frame.LabelCount--;
                        continue;

                    case WasmOpcode.Br:
                    {
                        var depth = ReadU32(code, ref pc);
                        Branch(frame, (int)depth, ref pc);
                        continue;
                    }

                    case WasmOpcode.BrIf:
                    {
                        var depth = ReadU32(code, ref pc);

                        if (stack[--top].I32 != 0)
                        {
                            Branch(frame, (int)depth, ref pc);
                        }

                        continue;
                    }

                    case WasmOpcode.BrTable:
                    {
                        var count = ReadU32(code, ref pc);
                        var chosen = stack[--top].I32;
                        var taken = uint.MaxValue;

                        for (var index = 0u; index <= count; index++)
                        {
                            var label = ReadU32(code, ref pc);

                            if (index == (uint)chosen || index == count && taken == uint.MaxValue)
                            {
                                taken = label;
                            }
                        }

                        Branch(frame, (int)taken, ref pc);
                        continue;
                    }

                    case WasmOpcode.Return:
                        Branch(frame, frame.LabelCount - 1, ref pc);
                        continue;

                    case WasmOpcode.Call:
                    {
                        var callee = ReadU32(code, ref pc);
                        frame.Pc = pc;

                        if (!TryPushFrame((int)callee, out var failure))
                        {
                            return failure;
                        }

                        running = false;
                        continue;
                    }

                    case WasmOpcode.CallIndirect:
                    {
                        var declaredType = ReadU32(code, ref pc);
                        SkipVarInt(code, ref pc);

                        var slot = (uint)stack[--top].I32;
                        frame.Pc = pc;

                        if (store.Tables.Length == 0)
                        {
                            return WasmRunStatus.Defect;
                        }

                        if (!store.Tables[0].TryRead(slot, out var callee))
                        {
                            return Trapped(WasmTrapKind.OutOfBoundsTableAccess, frame, at);
                        }

                        if (callee == WasmTableInstance.NullFunctionReference)
                        {
                            return Trapped(WasmTrapKind.UninitializedElement, frame, at);
                        }

                        if (module.FunctionTypeIndices[callee] != declaredType)
                        {
                            return Trapped(WasmTrapKind.IndirectCallTypeMismatch, frame, at);
                        }

                        if (!TryPushFrame(callee, out var indirectFailure))
                        {
                            return indirectFailure;
                        }

                        running = false;
                        continue;
                    }

                    case WasmOpcode.Drop:
                        top--;
                        continue;

                    case WasmOpcode.Select:
                    {
                        var condition = stack[--top].I32;
                        var alternative = stack[--top];
                        var consequent = stack[--top];
                        stack[top++] = condition != 0 ? consequent : alternative;
                        continue;
                    }

                    case WasmOpcode.LocalGet:
                        stack[top++] = frame.Locals[ReadU32(code, ref pc)];
                        continue;

                    case WasmOpcode.LocalSet:
                        frame.Locals[ReadU32(code, ref pc)] = stack[--top];
                        continue;

                    case WasmOpcode.LocalTee:
                        frame.Locals[ReadU32(code, ref pc)] = stack[top - 1];
                        continue;

                    case WasmOpcode.GlobalGet:
                        stack[top++] = store.Globals[ReadU32(code, ref pc)];
                        continue;

                    case WasmOpcode.GlobalSet:
                        store.Globals[ReadU32(code, ref pc)] = stack[--top];
                        continue;

                    case WasmOpcode.MemorySize:
                        pc++;
                        stack[top++] = WasmValue.FromI32((int)store.Memories[0].PageCount);
                        continue;

                    case WasmOpcode.MemoryGrow:
                    {
                        pc++;

                        // Buy the poll headroom the growth's own proportional charge could need,
                        // before that charge is made inside the memory.
                        if (!pacing.TryReserve(WasmMemoryInstance.ProfileMaximumPages))
                        {
                            frame.Pc = pc;
                            return pacing.Failure;
                        }

                        var delta = (uint)stack[--top].I32;
                        var answer = store.Memories[0].Grow(delta, pacing.Meter);

                        if (answer != WasmMemoryInstance.GrowthRefused)
                        {
                            pacing.Observe(delta);
                        }

                        stack[top++] = WasmValue.FromI32((int)answer);
                        continue;
                    }

                    case WasmOpcode.I32Const:
                        stack[top++] = WasmValue.FromI32((int)ReadS64(code, ref pc));
                        continue;

                    case WasmOpcode.I64Const:
                        stack[top++] = WasmValue.FromI64(ReadS64(code, ref pc));
                        continue;

                    case WasmOpcode.F32Const:
                        stack[top++] = WasmValue.FromBits(ReadFixed(code, ref pc, 4));
                        continue;

                    case WasmOpcode.F64Const:
                        stack[top++] = WasmValue.FromBits(ReadFixed(code, ref pc, 8));
                        continue;

                    default:
                    {
                        WasmTrapKind? trap;

                        if (opcode is >= 0x28 and <= 0x3E)
                        {
                            trap = MemoryAccess(opcode, code, ref pc);
                        }
                        else if (!WasmReferenceNumerics.TryNumeric(opcode, stack, ref top, out trap))
                        {
                            // Validation admitted an opcode this loop does not implement, which is
                            // this assembly disagreeing with itself rather than anything an artifact
                            // did. It is reported as a defect and never as a trap.
                            frame.Pc = pc;
                            return WasmRunStatus.Defect;
                        }

                        if (trap is not null)
                        {
                            frame.Pc = pc;
                            return Trapped(trap.Value, frame, at);
                        }

                        continue;
                    }
                }
            }
        }

        return WasmRunStatus.Completed;
    }

    /// <summary>Opens a label at the current operand height.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=09A979
    // Broiler-Human:        PENDING
    private void PushLabel(WasmFrame frame, int target, int arity, bool isLoop)
    {
        frame.Labels[frame.LabelCount++] = new WasmLabel
        {
            Target = target,
            Arity = arity,
            Height = top - frame.StackBase,
            IsLoop = isLoop,
        };
    }

    /// <summary>
    /// Takes a branch: truncate the operand stack to the label's height, carry the label's values
    /// across, close the labels the branch left, and jump.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=A49C19
    // Broiler-Falsified-If: a branch to a loop closes the loop's own label, or a branch leaves values below the label's height
    // Broiler-Human:        PENDING
    private void Branch(WasmFrame frame, int depth, ref int pc)
    {
        var index = frame.LabelCount - 1 - depth;
        var label = frame.Labels[index];
        var from = top - label.Arity;
        var to = frame.StackBase + label.Height;

        for (var slot = 0; slot < label.Arity; slot++)
        {
            stack[to + slot] = stack[from + slot];
        }

        top = to + label.Arity;
        frame.LabelCount = label.IsLoop ? index + 1 : index;
        pc = label.Target;
    }

    /// <summary>
    /// Executes one load or store, answering the trap it raised or nothing.
    /// </summary>
    /// <remarks>
    /// The alignment immediate is read and discarded: validation already refused an alignment above
    /// the natural one, and the specification gives alignment no semantics beyond that - a hint an
    /// implementation may ignore, which this one does.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=5; Fingerprint=666BB8
    // Broiler-Falsified-If: an effective address is computed in 32-bit arithmetic, or a width here disagrees with the one validation typed
    // Broiler-Human:        PENDING
    private WasmTrapKind? MemoryAccess(byte opcode, System.ReadOnlySpan<byte> code, ref int pc)
    {
        SkipVarInt(code, ref pc);
        var offset = ReadU32(code, ref pc);
        var memory = store.Memories[0];

        if (opcode >= 0x36)
        {
            var value = stack[--top];
            var address = (ulong)(uint)stack[--top].I32 + offset;

            var width = opcode switch
            {
                0x3A or 0x3C => 1,
                0x3B or 0x3D => 2,
                0x36 or 0x38 or 0x3E => 4,
                _ => 8,
            };

            return memory.TryStore(address, width, value.Low)
                ? null
                : WasmTrapKind.OutOfBoundsMemoryAccess;
        }

        var loadAddress = (ulong)(uint)stack[--top].I32 + offset;

        var loadWidth = opcode switch
        {
            0x2C or 0x2D or 0x30 or 0x31 => 1,
            0x2E or 0x2F or 0x32 or 0x33 => 2,
            0x28 or 0x2A or 0x34 or 0x35 => 4,
            _ => 8,
        };

        if (!memory.TryLoad(loadAddress, loadWidth, out var bits))
        {
            return WasmTrapKind.OutOfBoundsMemoryAccess;
        }

        stack[top++] = opcode switch
        {
            0x2C => WasmValue.FromI32((sbyte)bits),
            0x2D => WasmValue.FromI32((byte)bits),
            0x2E => WasmValue.FromI32((short)bits),
            0x2F => WasmValue.FromI32((ushort)bits),
            0x30 => WasmValue.FromI64((sbyte)bits),
            0x31 => WasmValue.FromI64((byte)bits),
            0x32 => WasmValue.FromI64((short)bits),
            0x33 => WasmValue.FromI64((ushort)bits),
            0x34 => WasmValue.FromI64((int)(uint)bits),
            0x35 => WasmValue.FromI64((long)(uint)bits),
            _ => WasmValue.FromBits(bits),
        };

        return null;
    }

    /// <summary>Reads an unsigned variable-length integer out of validated code.</summary>
    /// <remarks>
    /// The bytes were validated, so this reader answers rather than refusing; what it will not do is
    /// read past the body's end or shift past the width, because a defect in this assembly must fail
    /// as a wrong answer rather than as an out-of-range access.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=BE279D
    // Broiler-Falsified-If: it reads past the end of the body, or shifts a payload byte past the width
    // Broiler-Human:        PENDING
    private static uint ReadU32(System.ReadOnlySpan<byte> code, ref int pc)
    {
        var result = 0u;
        var shift = 0;

        while (pc < code.Length)
        {
            var current = code[pc];
            pc++;

            if (shift < 32)
            {
                result |= (uint)(current & 0x7F) << shift;
            }

            shift += 7;

            if ((current & 0x80) == 0)
            {
                break;
            }
        }

        return result;
    }

    /// <summary>Reads a signed variable-length integer out of validated code.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=FA830B
    // Broiler-Falsified-If: it reads past the end of the body, or sign-extends from the wrong bit
    // Broiler-Human:        PENDING
    private static long ReadS64(System.ReadOnlySpan<byte> code, ref int pc)
    {
        var result = 0L;
        var shift = 0;
        byte current = 0;

        while (pc < code.Length)
        {
            current = code[pc];
            pc++;

            if (shift < 64)
            {
                result |= (long)(current & 0x7F) << shift;
            }

            shift += 7;

            if ((current & 0x80) == 0)
            {
                break;
            }
        }

        if (shift < 64 && (current & 0x40) != 0)
        {
            result |= -1L << shift;
        }

        return result;
    }

    /// <summary>Reads a fixed-width little-endian immediate out of validated code.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E91E47
    // Broiler-Human:        PENDING
    private static ulong ReadFixed(System.ReadOnlySpan<byte> code, ref int pc, int width)
    {
        var bits = 0UL;

        for (var index = 0; index < width && pc < code.Length; index++)
        {
            bits |= (ulong)code[pc] << (index * 8);
            pc++;
        }

        return bits;
    }

    /// <summary>Steps past a variable-length immediate whose value is not needed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0A54EE
    // Broiler-Human:        PENDING
    private static void SkipVarInt(System.ReadOnlySpan<byte> code, ref int pc)
    {
        while (pc < code.Length)
        {
            var current = code[pc];
            pc++;

            if ((current & 0x80) == 0)
            {
                break;
            }
        }
    }
}
