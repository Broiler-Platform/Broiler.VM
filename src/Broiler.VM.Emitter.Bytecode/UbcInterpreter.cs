// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   25
// Annotated:        25/25
// Exempt:           22
// Human-reviewed:   0/25
// IP risk:          Low
// Security risk:    Critical
// Criteria:         14/12
// Resource impact:  5/10 max
// Unverified:       25
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Immutable;
using Broiler.VM.Ubc;

namespace Broiler.VM.Emitter.Bytecode;

/// <summary>One heap frame: its unit, the instruction it is at, where its locals start, and where its results go.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C9372D
// Broiler-Human:        PENDING
internal struct UbcFrame
{
    /// <summary>The program the unit belongs to: the instance's, or one a guest load answered with.</summary>
    internal UbcVerifiedProgram Program;

    /// <summary>The unit.</summary>
    internal int Unit;

    /// <summary>The instruction the frame is at; for a caller, the instruction that called.</summary>
    internal int Index;

    /// <summary>The frame's first word local.</summary>
    internal int WordBase;

    /// <summary>The frame's first value local.</summary>
    internal int ValueBase;

    /// <summary>Where the frame's word results go when it returns.</summary>
    internal int ReturnWords;

    /// <summary>Where the frame's value results go when it returns.</summary>
    internal int ReturnValues;
}

/// <summary>
/// The meter the loop and the family's handlers charge through: fuel is counted toward the family's
/// uncharged-work bound here, so a handler's own charges and the loop's are one count and one poll
/// governs both.
/// </summary>
// Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=High; Resources=0; Fingerprint=622222
// Broiler-Falsified-If: fuel charged since the last poll can exceed the family's bound, or a handler's fuel charge escapes the count
// Broiler-Human:        PENDING
internal sealed class UbcLoopMeter : IVmMeter
{
    private readonly IVmMeter inner;
    private readonly ulong bound;
    private ulong since;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=547CDA
    // Broiler-Human:        PENDING
    internal UbcLoopMeter(IVmMeter inner, uint bound)
    {
        this.inner = inner;

        // A family that declares no bound still gets polled: cancellation must be observable.
        this.bound = bound == 0 ? 4096UL : bound;
    }

    /// <summary>True when the last refusal came from a poll rather than from a charge.</summary>
    internal bool Stopped { get; private set; }

    /// <summary>The meter the core handed the step.</summary>
    internal IVmMeter Inner => inner;

    /// <summary>Charges <paramref name="amount"/> of fuel, polling whenever the count reaches the bound.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=106A66
    // Broiler-Falsified-If: a charge is split so that work since the last poll passes the bound, or a refused piece is answered as spent
    // Broiler-Human:        PENDING
    internal bool Spend(ulong amount)
    {
        while (amount > 0)
        {
            if (since == bound && !Poll())
            {
                return false;
            }

            var piece = System.Math.Min(amount, bound - since);

            if (!inner.TryCharge(VmBudgetDimension.Fuel, piece))
            {
                Stopped = false;
                return false;
            }

            since += piece;
            amount -= piece;
        }

        return true;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=78EF54
    // Broiler-Falsified-If: fuel a handler charges reaches the core's meter without being counted toward the bound
    // Broiler-Human:        PENDING
    public bool TryCharge(VmBudgetDimension dimension, ulong amount)
    {
        if (dimension == VmBudgetDimension.Fuel)
        {
            return Spend(amount);
        }

        var charged = inner.TryCharge(dimension, amount);
        Stopped = false;
        return charged;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1AB442
    // Broiler-Human:        PENDING
    public bool Poll()
    {
        since = 0;

        if (inner.Poll())
        {
            return true;
        }

        Stopped = true;
        return false;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=2DD6F6
    // Broiler-Human:        PENDING
    public void ReportRetained(VmBudgetDimension dimension, ulong amount) => inner.ReportRetained(dimension, amount);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=25888A
    // Broiler-Human:        PENDING
    public void ReportReleased(VmBudgetDimension dimension, ulong amount) => inner.ReportReleased(dimension, amount);
}

/// <summary>
/// The dispatch loop: one per operation, generic over the family so the loop specialises per family
/// and reaches the family's handlers through static members with no interface dispatch.
/// </summary>
/// <remarks>
/// <para>
/// <b>One CLR frame whatever the guest's depth.</b> Frames are <see cref="UbcFrame"/> values in an
/// array the operation owns; a call pushes one and a return pops one, and <c>CallDepth</c> is charged
/// per push and released per pop, so a guest recursion is refused by the meter and never by the
/// process's stack.
/// </para>
/// <para>
/// <b>The heights are the walk's.</b> Every instruction carries its planes' operand heights, so the
/// loop keeps no stack pointer: a frame's operand top at an instruction is its stack base plus that
/// instruction's height. A handler writes its outputs at the argument bases and cannot move a top.
/// </para>
/// <para>
/// <b>Fuel before effects.</b> Every instruction charges its row's cost before anything it does, the
/// common rows one each; a frame's entry charges its unit's <see cref="UbcUnitCode.FrameFuel"/> with
/// it, and unwinding a unit per region it looks at and per frame it leaves. The loop polls at the
/// family's declared bound.
/// </para>
/// <para>
/// <b>A parked operation holds no depth.</b> A suspension gives its frames' <c>CallDepth</c> back
/// and a resumption charges it again before any frame stands, because the core may drop a parked
/// continuation outside any step, where nothing could give it back.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=5; Fingerprint=2D9DF7
// Broiler-Falsified-If: an instruction's effect happens before its fuel is charged, a frame is pushed without CallDepth being charged, or a slot is read or written outside the heights the walk proved
// Broiler-Human:        PENDING
internal sealed class UbcInterpreter<TFamily>
    where TFamily : struct, IUbcFamily
{
    // The bytes one heap frame occupies: a program reference and five integers, padded.
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=37D7BA
    // Broiler-Human:        PENDING
    private const int FrameBytes = 32;

    private readonly UbcInstance instance;
    private readonly UbcLoopMeter meter;
    private readonly IVmHostCapabilityInvoker capabilities;
    private ulong[] words = System.Array.Empty<ulong>();
    private IUbcValuePlane values = null!;
    private UbcFrame[] frames = System.Array.Empty<UbcFrame>();
    private int depth;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=FD4A38
    // Broiler-Human:        PENDING
    internal UbcInterpreter(UbcInstance instance, IVmMeter meter, IVmHostCapabilityInvoker capabilities, uint pollBound)
    {
        this.instance = instance;
        this.meter = new UbcLoopMeter(meter, pollBound);
        this.capabilities = capabilities;
    }

    /// <summary>Runs the entry unit <paramref name="unit"/>, its parameters bound by the family from the entry's name.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=AE42D7
    // Broiler-Falsified-If: an entry unit runs with locals the family did not bind left holding a previous operation's values
    // Broiler-Human:        PENDING
    internal VmExecutionStep Start(int unit, System.ReadOnlySpan<byte> name)
    {
        // The count of work since the last poll starts from a poll of its own: whatever the core's
        // meter already holds is not this loop's to know.
        if (!meter.Poll())
        {
            return Stop();
        }

        values = TFamily.CreateValuePlane(0);

        if (!Enter(instance.Program, unit, 0, 0, 0, 0, copyParameters: false))
        {
            return Finish(Stop());
        }

        var activation = Activation();
        activation.Program = instance.Program;
        activation.Unit = unit;
        activation.WordArgs = 0;
        activation.ValueArgs = 0;

        if (!TFamily.BindParameters(ref activation, name))
        {
            return Finish(VmExecutionStep.Faulted(TFamily.EntryRefused(instance.FamilyState, name)));
        }

        return Run();
    }

    /// <summary>Restores a suspended operation's frames and planes and continues after the suspending row.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=CC284E
    // Broiler-Falsified-If: a resumption continues with a plane other than the one captured, at an instruction other than the one after the suspending row, or with a frame standing whose depth is not charged
    // Broiler-Human:        PENDING
    internal VmExecutionStep Resume(UbcContinuation continuation)
    {
        // The operation's meter still holds the fuel the suspended step spent since its last poll;
        // this step's count starts from a poll, so the two are never added unseen.
        if (!meter.Poll())
        {
            return Stop();
        }

        // The suspension gave the frames' depth back; they are charged again before any of them
        // stands, and from here every exit that ends the operation gives it back through Finish.
        if (!meter.TryCharge(VmBudgetDimension.CallDepth, (ulong)continuation.Depth))
        {
            return Stop();
        }

        depth = continuation.Depth;
        var length = System.Math.Max(depth, 4);

        if (!meter.Inner.TryCharge(VmBudgetDimension.AllocatedBytes, (ulong)length * FrameBytes) || !meter.Spend((ulong)depth))
        {
            return Finish(Stop());
        }

        frames = new UbcFrame[length];
        System.Array.Copy(continuation.Frames, frames, depth);

        var top = frames[depth - 1];
        var code = top.Program.Units[top.Unit];
        values = TFamily.CreateValuePlane(0);

        // Every frame's extent, not the top's alone: a caller below may reach higher than its callee
        // once the callee returns, and the planes of an uninterrupted run were grown frame by frame.
        var wordExtent = 0;
        var valueExtent = 0;

        for (var frame = 0; frame < depth; frame++)
        {
            var unit = frames[frame].Program.Units[frames[frame].Unit];
            wordExtent = System.Math.Max(wordExtent, frames[frame].WordBase + unit.WordLocals + (int)unit.Unit.MaxWordHeight + 1);
            valueExtent = System.Math.Max(valueExtent, frames[frame].ValueBase + unit.ValueLocals + (int)unit.Unit.MaxValueHeight + 1);
        }

        if (!Grow(wordExtent, valueExtent))
        {
            return Finish(Stop());
        }

        System.Array.Copy(continuation.Words, words, continuation.Words.Length);
        TFamily.RestoreValues(values, 0, continuation.Values);

        var activation = Activation();
        activation.Program = top.Program;
        activation.Unit = top.Unit;
        activation.Pc = code.Instructions[top.Index].Pc;
        activation.WordBase = top.WordBase;
        activation.ValueBase = top.ValueBase;
        activation.WordArgs = continuation.Words.Length;
        activation.ValueArgs = continuation.ValueCount;
        activation.Pending = continuation.Reason;
        TFamily.OnResume(ref activation, continuation.Reason);

        frames[depth - 1].Index = continuation.ResumeIndex;
        return Run();
    }

    /// <summary>The loop.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=5; Fingerprint=B82492
    // Broiler-Falsified-If: a common row is executed with another meaning than Appendix A gives it, or a family status its row's kind does not admit is acted on rather than answered as a contract violation
    // Broiler-Human:        PENDING
    private VmExecutionStep Run()
    {
        var activation = Activation();

    Reload:
        var frame = frames[depth - 1];
        var program = frame.Program;
        var units = program.Units;
        var code = units[frame.Unit];
        var instructions = code.Instructions;
        var stackWords = frame.WordBase + code.WordLocals;
        var stackValues = frame.ValueBase + code.ValueLocals;
        var index = frame.Index;

        while (true)
        {
            ref readonly var ins = ref instructions.ItemRef(index);
            var row = ins.Row;

            if (!meter.Spend(row?.Cost ?? 1U))
            {
                return Finish(Stop());
            }

            var wordTop = stackWords + ins.WordHeight;
            var valueTop = stackValues + ins.ValueHeight;

            if (row is not null)
            {
                activation.Program = program;
                activation.Unit = frame.Unit;
                activation.Pc = ins.Pc;
                activation.WordBase = frame.WordBase;
                activation.ValueBase = frame.ValueBase;
                activation.WordArgs = wordTop - ins.WordPops;
                activation.ValueArgs = valueTop - ins.ValuePops;
                activation.Words = words;
                activation.Values = values;

                if (row.Kind == UbcInstructionKind.Primitive && row.Primitive is { } primitive && !UbcPrimitives.IsRegionAccess(primitive))
                {
                    var args = activation.WordArgs;
                    var result = UbcPrimitives.Evaluate(primitive, words[args], ins.WordPops > 1 ? words[args + 1] : 0, program.Table.CanonicaliseNaN);

                    if (result.Trap != UbcTrapCode.None)
                    {
                        return Finish(Trapped(ref activation, program, row, result.Trap));
                    }

                    words[args] = result.Bits;
                    index = ins.Next;
                    continue;
                }

                // Outputs a handler did not set must not be read from an earlier instruction.
                activation.CallRequest = new UbcCallRequest(-1);
                activation.Pending = null;

                var status = TFamily.Handle(ref activation, ins.Opcode, ins.Operand);

                if (!ReferenceEquals(activation.Words, words) || !ReferenceEquals(activation.Values, values))
                {
                    return Finish(VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation));
                }

                switch (status.Kind)
                {
                    case UbcStatusKind.Next when !row.IsTerminal:
                        Discard(activation.ValueArgs + ins.ValuePushes, ins.ValuePops - ins.ValuePushes);
                        index = ins.Next;
                        continue;

                    case UbcStatusKind.Taken when row.Kind == UbcInstructionKind.Branch:
                        Discard(activation.ValueArgs + ins.TakenValuePushes, ins.ValuePops - ins.TakenValuePushes);
                        index = ins.Target;
                        continue;

                    case UbcStatusKind.Request when row.Kind == UbcInstructionKind.Call && !row.IsTerminal:
                    {
                        var callee = activation.CallRequest.Unit;
                        var calleeProgram = activation.CallRequest.Program ?? program;

                        // The family boundary is not crossed by a call: a program of another family is
                        // the handler's defect, and so is a unit the program does not have.
                        if (!string.Equals(calleeProgram.Table.FamilyIdentity, instance.Program.Table.FamilyIdentity, System.StringComparison.Ordinal) ||
                            (uint)callee >= (uint)calleeProgram.Units.Length ||
                            !Fits(row, in ins, calleeProgram.Units[callee]))
                        {
                            return Finish(VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation));
                        }

                        frames[depth - 1].Index = index;

                        // The callee's frame starts above the row's whole input region, whose top slots
                        // are its parameters; its results replace the region.
                        if (!Enter(
                                calleeProgram,
                                callee,
                                wordTop,
                                valueTop,
                                wordTop - ins.WordPops,
                                valueTop - ins.ValuePops,
                                copyParameters: true))
                        {
                            return Finish(Stop());
                        }

                        activation.Words = words;
                        activation.Values = values;
                        goto Reload;
                    }

                    case UbcStatusKind.Suspend when row.Kind == UbcInstructionKind.Suspend && !row.IsTerminal:
                        frames[depth - 1].Index = index;
                        return Suspend(ref activation, ins.Next);

                    case UbcStatusKind.Threw when row.Kind != UbcInstructionKind.Primitive:
                        frames[depth - 1].Index = index;

                        switch (Unwind(ref activation, activation.Pending))
                        {
                            case Unwound.Uncaught:
                                return Finish(VmExecutionStep.Faulted(TFamily.Uncaught(ref activation)));
                            case Unwound.Stopped:
                                return Finish(Stop());
                        }

                        goto Reload;

                    case UbcStatusKind.Trap when program.Table.DefinesTrap(status.Code):
                        return Finish(VmExecutionStep.Faulted(TFamily.Fault(ref activation, program.FamilySlot, status.Code)));

                    default:
                        return Finish(VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation));
                }
            }

            switch ((UbcOpcode)ins.Opcode)
            {
                case UbcOpcode.Nop:
                    index = ins.Next;
                    break;

                case UbcOpcode.Trap:
                {
                    var shape = UbcOpcodes.Row(UbcOpcode.Trap).Shape;
                    activation.Program = program;
                    activation.Unit = frame.Unit;
                    activation.Pc = ins.Pc;
                    activation.WordArgs = wordTop;
                    activation.ValueArgs = valueTop;
                    return Finish(VmExecutionStep.Faulted(TFamily.Fault(
                        ref activation,
                        (byte)UbcOperandShapes.First(shape, ins.Operand),
                        (ushort)UbcOperandShapes.Second(shape, ins.Operand))));
                }

                case UbcOpcode.Jump:
                    index = ins.Target;
                    break;

                case UbcOpcode.JumpIfZero:
                    index = (uint)words[wordTop - 1] == 0 ? ins.Target : ins.Next;
                    break;

                case UbcOpcode.JumpIfNonZero:
                    index = (uint)words[wordTop - 1] != 0 ? ins.Target : ins.Next;
                    break;

                case UbcOpcode.JumpTable:
                {
                    var targets = program.JumpTableTargets[ins.Index];
                    var selector = (uint)words[wordTop - 1];
                    index = targets[selector < (uint)targets.Length ? (int)selector : targets.Length - 1];
                    break;
                }

                case UbcOpcode.Return:
                {
                    var resultWords = ins.WordPops;
                    var resultValues = ins.ValuePops;
                    var toWords = frame.ReturnWords;
                    var toValues = frame.ReturnValues;

                    System.Array.Copy(words, wordTop - resultWords, words, toWords, resultWords);

                    for (var slot = 0; slot < resultValues; slot++)
                    {
                        values.Copy(valueTop - resultValues + slot, toValues + slot);
                    }

                    Discard(toValues + resultValues, valueTop - (toValues + resultValues));
                    depth--;
                    meter.ReportReleased(VmBudgetDimension.CallDepth, 1);

                    if (depth == 0)
                    {
                        activation.Program = program;
                        activation.Unit = frame.Unit;
                        activation.Pc = ins.Pc;
                        activation.WordArgs = toWords;
                        activation.ValueArgs = toValues;
                        activation.Words = words;
                        activation.Values = values;
                        return VmExecutionStep.Completed(TFamily.Completion(ref activation));
                    }

                    ref var caller = ref frames[depth - 1];
                    caller.Index = caller.Program.Units[caller.Unit].Instructions[caller.Index].Next;
                    goto Reload;
                }

                case UbcOpcode.Call:
                {
                    var target = units[ins.Index];
                    frames[depth - 1].Index = index;

                    // The callee's frame starts above the caller's stack, whose top slots are the
                    // arguments; its results replace them.
                    if (!Enter(
                            program,
                            ins.Index,
                            wordTop,
                            valueTop,
                            wordTop - target.ParameterWords,
                            valueTop - target.ParameterValues,
                            copyParameters: true))
                    {
                        return Finish(Stop());
                    }

                    activation.Words = words;
                    activation.Values = values;
                    goto Reload;
                }

                case UbcOpcode.Drop:
                    Discard(valueTop - ins.ValuePops, ins.ValuePops);
                    index = ins.Next;
                    break;

                case UbcOpcode.Dup:
                case UbcOpcode.Dup2:
                    for (var slot = 0; slot < ins.WordPops; slot++)
                    {
                        words[wordTop + slot] = words[wordTop - ins.WordPops + slot];
                    }

                    for (var slot = 0; slot < ins.ValuePops; slot++)
                    {
                        values.Copy(valueTop - ins.ValuePops + slot, valueTop + slot);
                    }

                    index = ins.Next;
                    break;

                case UbcOpcode.Swap:
                    if (ins.WordPops == 2)
                    {
                        (words[wordTop - 1], words[wordTop - 2]) = (words[wordTop - 2], words[wordTop - 1]);
                    }
                    else if (ins.ValuePops == 2)
                    {
                        // The plane has a spare slot above every frame's highest height for this.
                        values.Copy(valueTop - 1, valueTop);
                        values.Copy(valueTop - 2, valueTop - 1);
                        values.Copy(valueTop, valueTop - 2);
                        values.Clear(valueTop, 1);
                    }

                    index = ins.Next;
                    break;

                case UbcOpcode.Pick:
                    if (ins.Plane == UbcPlane.Word)
                    {
                        words[wordTop] = words[wordTop - 1 - ins.Index];
                    }
                    else
                    {
                        values.Copy(valueTop - 1 - ins.Index, valueTop);
                    }

                    index = ins.Next;
                    break;

                case UbcOpcode.Select:
                {
                    var keepDeeper = (uint)words[wordTop - 1] != 0;

                    if (ins.ValuePops == 2)
                    {
                        if (!keepDeeper)
                        {
                            values.Copy(valueTop - 1, valueTop - 2);
                        }

                        values.Clear(valueTop - 1, 1);
                    }
                    else if (!keepDeeper)
                    {
                        words[wordTop - 3] = words[wordTop - 2];
                    }

                    index = ins.Next;
                    break;
                }

                case UbcOpcode.Squash:
                {
                    var keptWords = ins.WordPushes;
                    var droppedWords = ins.WordPops - keptWords;
                    System.Array.Copy(words, wordTop - keptWords, words, wordTop - keptWords - droppedWords, keptWords);

                    var keptValues = ins.ValuePushes;
                    var droppedValues = ins.ValuePops - keptValues;

                    for (var slot = 0; slot < keptValues; slot++)
                    {
                        values.Copy(valueTop - keptValues + slot, valueTop - keptValues - droppedValues + slot);
                    }

                    Discard(valueTop - droppedValues, droppedValues);
                    index = ins.Next;
                    break;
                }

                case UbcOpcode.LocalGet:
                    if (ins.Plane == UbcPlane.Word)
                    {
                        words[wordTop] = words[frame.WordBase + ins.Index];
                    }
                    else
                    {
                        values.Copy(frame.ValueBase + ins.Index, valueTop);
                    }

                    index = ins.Next;
                    break;

                case UbcOpcode.LocalSet:
                case UbcOpcode.LocalTee:
                    if (ins.Plane == UbcPlane.Word)
                    {
                        words[frame.WordBase + ins.Index] = words[wordTop - 1];
                    }
                    else
                    {
                        values.Copy(valueTop - 1, frame.ValueBase + ins.Index);

                        if (ins.Opcode == (byte)UbcOpcode.LocalSet)
                        {
                            values.Clear(valueTop - 1, 1);
                        }
                    }

                    index = ins.Next;
                    break;

                case UbcOpcode.ConstI32:
                case UbcOpcode.ConstI64:
                case UbcOpcode.ConstF32:
                case UbcOpcode.ConstF64:
                    words[wordTop] = ins.Operand;
                    index = ins.Next;
                    break;

                default:
                    return Finish(VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation));
            }
        }
    }

    /// <summary>
    /// Pushes a frame for <paramref name="unit"/> whose locals start at the given bases, charging
    /// <c>CallDepth</c> and the unit's <see cref="UbcUnitCode.FrameFuel"/> first and growing the planes
    /// to the callee's declared extent.
    /// </summary>
    /// <remarks>
    /// For a call the parameters are the slots just below the bases, and they are copied into the
    /// callee's first locals, word parameters copied and value parameters plane-copied, as Appendix A's
    /// call row says: the caller's slots stay as they were, so a region of the caller whose entry
    /// heights reach the arguments finds them unchanged when it lands, whatever the callee did to its
    /// parameters. The entry unit's parameters are the family's to bind.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=3D3B8E
    // Broiler-Falsified-If: a frame is pushed without its CallDepth and fuel charged first, a callee's parameter local is the caller's own slot, or a callee's non-parameter locals start holding anything but zero and the family's empty value
    // Broiler-Human:        PENDING
    private bool Enter(UbcVerifiedProgram program, int unit, int wordBase, int valueBase, int returnWords, int returnValues, bool copyParameters)
    {
        if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            return false;
        }

        var code = program.Units[unit];

        if (!meter.Spend(code.FrameFuel))
        {
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
            return false;
        }

        if (depth == frames.Length)
        {
            var length = System.Math.Max(4, frames.Length * 2);

            if (!meter.TryCharge(VmBudgetDimension.AllocatedBytes, (ulong)(length - frames.Length) * FrameBytes))
            {
                meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
                return false;
            }

            System.Array.Resize(ref frames, length);
        }

        if (!Grow(wordBase + code.WordLocals + (int)code.Unit.MaxWordHeight + 1,
                  valueBase + code.ValueLocals + (int)code.Unit.MaxValueHeight + 1))
        {
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
            return false;
        }

        if (copyParameters)
        {
            System.Array.Copy(words, wordBase - code.ParameterWords, words, wordBase, code.ParameterWords);

            for (var slot = 0; slot < code.ParameterValues; slot++)
            {
                values.Copy(valueBase - code.ParameterValues + slot, valueBase + slot);
            }
        }

        System.Array.Clear(words, wordBase + code.ParameterWords, code.WordLocals - code.ParameterWords);
        values.Clear(valueBase + code.ParameterValues, code.ValueLocals - code.ParameterValues);

        frames[depth++] = new UbcFrame
        {
            Program = program,
            Unit = unit,
            Index = 0,
            WordBase = wordBase,
            ValueBase = valueBase,
            ReturnWords = returnWords,
            ReturnValues = returnValues,
        };
        return true;
    }

    /// <summary>Grows each plane to at least the given capacity, charging the bytes before allocating them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=294E27
    // Broiler-Falsified-If: a plane grows by bytes that were not charged to the allocated-bytes allowance first
    // Broiler-Human:        PENDING
    private bool Grow(int wordCapacity, int valueCapacity)
    {
        if (wordCapacity > words.Length)
        {
            var length = System.Math.Max(wordCapacity, System.Math.Min(words.Length * 2, int.MaxValue / 16));

            if (!meter.TryCharge(VmBudgetDimension.AllocatedBytes, (ulong)(length - words.Length) * sizeof(ulong)))
            {
                return false;
            }

            System.Array.Resize(ref words, length);
        }

        if (valueCapacity > values.Capacity)
        {
            var length = System.Math.Max(valueCapacity, System.Math.Min(values.Capacity * 2, int.MaxValue / 16));

            if (!meter.TryCharge(VmBudgetDimension.AllocatedBytes, (ulong)(length - values.Capacity) * (ulong)values.ValueBytes))
            {
                return false;
            }

            values.Resize(length);
        }

        return true;
    }

    /// <summary>
    /// Unwinds an exception in flight: searches the current frame's regions innermost first, lands in
    /// the first that covers the instruction, and otherwise pops the frame and searches its caller at
    /// the instruction that called. Answers false when no frame catches it.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=3620FA
    // Broiler-Falsified-If: an outer region lands before an inner one covering the same instruction, or a landing leaves either plane above the region's entry height
    // Broiler-Human:        PENDING
    private Unwound Unwind(ref UbcActivation activation, object? pending)
    {
        while (depth > 0)
        {
            ref var frame = ref frames[depth - 1];
            var code = frame.Program.Units[frame.Unit];
            var stackWords = frame.WordBase + code.WordLocals;
            var stackValues = frame.ValueBase + code.ValueLocals;

            // The search is work the guest caused: a fuel unit per region looked at and per frame
            // left, charged before it is done, so one throw through a deep stack of wide units is
            // not bought for the one unit its row costs.
            if (!meter.Spend((ulong)code.Regions.Length + 1))
            {
                return Unwound.Stopped;
            }

            // The most the frame's stack can hold here: the instruction's own height and whatever its
            // row may have written above its arguments. Every such slot was pushed by a row that paid
            // for it, and the locals by the frame's entry, so what is cleared below was paid for.
            ref readonly var at = ref code.Instructions.ItemRef(frame.Index);
            var live = stackValues + at.ValueHeight + at.ValuePushes;

            foreach (var region in code.Regions)
            {
                if (!region.Covers(frame.Index))
                {
                    continue;
                }

                var valueTop = stackValues + region.ValueEntryHeight;
                Discard(valueTop, live - valueTop);

                activation.Program = frame.Program;
                activation.Unit = frame.Unit;
                activation.Pc = code.Instructions[region.Handler].Pc;
                activation.WordBase = frame.WordBase;
                activation.ValueBase = frame.ValueBase;
                activation.WordArgs = stackWords + region.WordEntryHeight;
                activation.ValueArgs = valueTop;
                activation.Pending = pending;
                TFamily.OnLand(ref activation, region.Kind);

                frame.Index = region.Handler;
                return Unwound.Landed;
            }

            Discard(frame.ValueBase, live - frame.ValueBase);
            depth--;
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }

        activation.WordArgs = 0;
        activation.ValueArgs = 0;
        activation.Pending = pending;
        return Unwound.Uncaught;
    }

    /// <summary>How an unwinding ended.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=73CB24
    // Broiler-Human:        PENDING
    private enum Unwound
    {
        /// <summary>A region caught the exception; the loop continues at its handler.</summary>
        Landed,

        /// <summary>No frame caught it.</summary>
        Uncaught,

        /// <summary>The search's fuel was refused or a poll said stop.</summary>
        Stopped,
    }

    /// <summary>Captures every frame and both planes below the suspending row's arguments, through the family's codec.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=F91B21
    // Broiler-Falsified-If: a slot below the arguments is missing from the continuation, the capture's bytes are not charged, or a parked continuation still holds CallDepth
    // Broiler-Human:        PENDING
    private VmExecutionStep Suspend(ref UbcActivation activation, int resumeIndex)
    {
        var wordCount = activation.WordArgs;
        var valueCount = activation.ValueArgs;

        if (activation.Pending is not { } reason)
        {
            // A suspension names its reason; a handler that answered suspend without one broke its contract.
            return Finish(VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation));
        }

        // The capture is charged whole before any of it is made: the words, the frames, and the
        // family's record of the values, at the plane's own bytes per value.
        var captured = ((ulong)wordCount * sizeof(ulong)) + ((ulong)depth * FrameBytes) + ((ulong)valueCount * (ulong)values.ValueBytes);

        if (!meter.TryCharge(VmBudgetDimension.AllocatedBytes, captured))
        {
            return Finish(Stop());
        }

        var savedWords = new ulong[wordCount];
        System.Array.Copy(words, savedWords, wordCount);
        var savedFrames = new UbcFrame[depth];
        System.Array.Copy(frames, savedFrames, depth);
        var savedValues = TFamily.CaptureValues(values, 0, valueCount);

        var continuation = new UbcContinuation(instance, savedFrames, depth, savedWords, savedValues, valueCount, resumeIndex, reason);
        var projection = TFamily.SuspendProjection(ref activation);

        // The parked frames give their depth back now: the core may drop the continuation outside any
        // step, where no release could reach the meter, and a resumption charges it again.
        return Finish(VmExecutionStep.Suspended(continuation, projection));
    }

    /// <summary>A primitive's trap, answered with the family's code for it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=93F04E
    // Broiler-Falsified-If: a trap is answered with a family code other than the row's mapping for it
    // Broiler-Human:        PENDING
    private static VmExecutionStep Trapped(ref UbcActivation activation, UbcVerifiedProgram program, UbcInstructionRow row, UbcTrapCode trap)
    {
        foreach (var mapping in row.Traps)
        {
            if (mapping.Universal == trap)
            {
                return VmExecutionStep.Faulted(TFamily.Fault(ref activation, program.FamilySlot, mapping.FamilyCode));
            }
        }

        // The table's constructor proved every trap the primitive can raise is mapped.
        return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
    }

    /// <summary>
    /// A call request fits when the callee's parameters are the top slots of the row's input region,
    /// type for type, and its results are exactly the row's pushes.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=F8A5E6
    // Broiler-Falsified-If: a request is performed whose callee would read a slot of another type than the walk proved, or return other slots than the row pushes
    // Broiler-Human:        PENDING
    private static bool Fits(UbcInstructionRow row, in UbcInstruction ins, UbcUnitCode callee)
    {
        var effect = row.Effect;
        var parameters = callee.Signature.Parameters;
        var region = ins.WordPops + ins.ValuePops;

        if (!System.Linq.Enumerable.SequenceEqual(callee.Signature.Results, effect.Pushes) || parameters.Length > region)
        {
            return false;
        }

        for (var slot = 0; slot < parameters.Length; slot++)
        {
            var position = region - parameters.Length + slot;
            var type = position < effect.Pops.Length ? effect.Pops[position] : effect.Repeated;

            if (type != parameters[slot])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Resets values the stack no longer holds, so the collector does not keep them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=9E24C3
    // Broiler-Human:        PENDING
    private void Discard(int start, int count)
    {
        if (count > 0)
        {
            values.Clear(start, count);
        }
    }

    /// <summary>The step a refused charge or a false poll ends in; the core names what happened from its latches.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A02F9E
    // Broiler-Human:        PENDING
    private VmExecutionStep Stop() =>
        VmExecutionStep.ContractViolation(meter.Stopped ? VmReason.Cancelled : VmReason.AllowanceExhausted);

    /// <summary>Releases the depth of every frame still standing, for a step that ends the operation.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4BCCA6
    // Broiler-Falsified-If: an ended operation leaves CallDepth charged for a frame that no longer exists
    // Broiler-Human:        PENDING
    private VmExecutionStep Finish(VmExecutionStep step)
    {
        if (depth > 0)
        {
            meter.ReportReleased(VmBudgetDimension.CallDepth, (ulong)depth);
            depth = 0;
        }

        return step;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=426DC5
    // Broiler-Human:        PENDING
    private UbcActivation Activation() => new()
    {
        InstanceState = instance.FamilyState,
        Meter = meter,
        Capabilities = capabilities,
        Words = words,
        Values = values,
    };
}
