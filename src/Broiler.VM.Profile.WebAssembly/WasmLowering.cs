// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   51
// Annotated:        51/51
// Exempt:           40
// Human-reviewed:   0/51
// IP risk:          Low
// Security risk:    Critical
// Criteria:         25/23
// Resource impact:  5/10 max
// Unverified:       51
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Ubc;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The lowering of a validated WebAssembly module into a universal bytecode artifact of the
/// WebAssembly family: one unit per function, the structured control flow as jumps, the module's
/// definitions as the family's FamilyData section.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE WALK OVER EACH BODY IS THE VALIDATOR'S, REPEATED WITH TYPES INSTEAD OF CHECKS.</b> The
/// validator hands over each body's resolved block targets and nothing of its operand stack, so the
/// lowering tracks the typed stack again, from the function's entry, over the code the validator has
/// already admitted. Every immediate is read again with this profile's own padding-tolerant readers,
/// because a module may pad an integer within its byte budget and the core's canonical readers would
/// refuse it; nothing a body says is trusted that the validator did not first check.
/// </para>
/// <para>
/// <b>Structured instructions emit nothing</b>, and a stack of control frames stands for them: each
/// frame's label, the typed stack at its entry, the values a branch to it carries and whether any
/// branch reached it. A <c>loop</c>'s label is the offset of what follows it; every other label is
/// marked where its <c>end</c> is. An <c>if</c> is a <c>jump_if_zero</c> to its alternative or its
/// end, and an <c>else</c> a <c>jump</c> to the end when the consequent falls into it.
/// </para>
/// <para>
/// <b>A branch drops what lies between the label's entry height and the values it carries</b> with
/// <c>squash</c>, chained two hundred and fifty-five slots at a time, before its <c>jump</c>; a branch
/// to the function's own label is a <c>return</c>, which drops whatever lies under the results. A
/// conditional branch that has nothing to drop is one <c>jump_if_nonzero</c> to the label; any other,
/// and every target of a <c>br_table</c> that has something to drop or that is the function's label,
/// goes through a trampoline after the body - the <c>squash</c> and <c>jump</c>, or the
/// <c>return</c> - keyed by the label and by the typed stack at the site, so two sites share one only
/// when they reach it with one stack, and the walk's rule that every arrival at an instruction brings
/// one typed stack holds for it. Trampolines follow the body in the order they were first used.
/// </para>
/// <para>
/// <b>Code no execution reaches is not emitted</b>, because the walk refuses an unreachable
/// instruction: after <c>unreachable</c>, <c>br</c>, <c>br_table</c> or <c>return</c> the lowering
/// steps to the enclosing frame's <c>else</c> or <c>end</c> by the validator's resolved offsets, and
/// an <c>end</c> nothing branches to and nothing falls into leaves the code after it unreachable too.
/// </para>
/// <para>
/// <b>What it writes, in the container's order:</b> the header; one family row, slot one; the module's
/// types one for one, never deduplicated, because <c>call_indirect</c> compares type indices; one unit
/// per function in function-index order, its declared locals re-encoded as runs, the exact greatest
/// word height its emitted code reaches, the entry flag when the function is exported; the code, every
/// unit built at its absolute offset so jump targets, jump tables and positions are all absolute; one
/// jump table per <c>br_table</c> in site order; an empty Entries section, because exports are found by
/// the family through its definitions; one Positions row per emitted row that can trap, naming the
/// function and the instruction's offset inside its body, in ascending order; and the module's
/// definitions.
/// </para>
/// <para>
/// <b>What the universal bytecode cannot hold is refused here</b>, with this profile's codes in the
/// translation band: more locals than a unit declares, a stack above the height a unit declares, more
/// branch tables than an artifact names, an immediate its operand field cannot hold, a branch carrying
/// more values than one <c>squash</c> keeps. An artifact larger than the artifact-bytes ceiling is
/// refused as the core would refuse it, as an exhaustion of that ceiling at artifact scope, before it
/// is allocated whole.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=5; Fingerprint=FB896C
// Broiler-Falsified-If: an artifact is written whose walk refuses it for a module the validator admitted, whose execution answers differently from the bare-module interpreter's other than for the float comparisons, or two translations of one module differ in a byte
// Broiler-Human:        PENDING
internal sealed class WasmLowering
{
    /// <summary>The most slots one <c>squash</c> drops or keeps: each count is one byte.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=20709D
    // Broiler-Human:        PENDING
    private const int SquashField = 255;

    /// <summary>How many instructions the lowering steps between two looks at the cancellation token.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C323FC
    // Broiler-Human:        PENDING
    private const int InstructionsBetweenPolls = 4096;

    private readonly WasmModule module;
    private readonly ulong byteLimit;
    private readonly System.Threading.CancellationToken cancellationToken;
    private readonly List<byte> code = new();
    private readonly List<UbcUnit> units = new();
    private readonly List<UbcJumpTable?> tables = new();
    private readonly List<UbcPosition> positions = new();
    private readonly TypedStack stacks = new();
    private readonly List<Frame> frames = new();
    private readonly List<long> marks = new();
    private readonly Dictionary<long, int> trampolineLabels = new();
    private readonly List<Trampoline> trampolines = new();
    private readonly List<(int Table, int[] Labels)> unitTables = new();
    private UbcCodeBuilder builder = new();
    private WasmTranslation? refusal;
    private int function;
    private int stack;
    private int maxHeight;
    private int steps;
    private bool reachable;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=532F01
    // Broiler-Human:        PENDING
    private WasmLowering(WasmModule module, ulong byteLimit, System.Threading.CancellationToken cancellationToken)
    {
        this.module = module;
        this.byteLimit = byteLimit;
        this.cancellationToken = cancellationToken;
    }

    /// <summary>What a frame of the control stack stands for.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AB6E44
    // Broiler-Human:        PENDING
    private enum FrameKind : byte
    {
        Function,
        Block,
        Loop,
        If,
        Else,
    }

    /// <summary>
    /// Lowers <paramref name="module"/>, which the validator admitted, into an artifact no larger than
    /// <paramref name="byteLimit"/> bytes, and answers it or the refusal.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=5; Fingerprint=3BE11D
    // Broiler-Falsified-If: a translation is answered when a body was refused, or the answered artifact exceeds the byte limit
    // Broiler-Human:        PENDING
    internal static WasmTranslation Lower(WasmModule module, ulong byteLimit, System.Threading.CancellationToken cancellationToken)
    {
        var lowering = new WasmLowering(module, byteLimit, cancellationToken);
        return lowering.Run() ?? lowering.refusal ?? WasmTranslation.Defect();
    }

    /// <summary>Lowers every body, then writes the artifact; null with the refusal set when a body is refused.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=4; Fingerprint=926133
    // Broiler-Falsified-If: the sections are written in another order than the container's, a unit is written out of function-index order, or the artifact is answered past the byte limit
    // Broiler-Human:        PENDING
    private WasmTranslation? Run()
    {
        var exported = new bool[module.FunctionCount];

        foreach (var export in module.Exports)
        {
            if (export.Kind is WasmExportKind.Function && export.EntityIndex < (uint)exported.Length)
            {
                exported[export.EntityIndex] = true;
            }
        }

        for (var index = 0; index < module.FunctionCount; index++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return WasmTranslation.Cancelled();
            }

            if (!LowerBody(index, exported[index]))
            {
                return null;
            }
        }

        var types = new UbcSignature[module.TypeCount];

        for (var index = 0; index < types.Length; index++)
        {
            types[index] = new UbcSignature(Slots(module.Types[index].Parameters), Slots(module.Types[index].Results));
        }

        var jumpTables = new UbcJumpTable[tables.Count];

        for (var index = 0; index < jumpTables.Length; index++)
        {
            jumpTables[index] = tables[index] ?? throw new System.InvalidOperationException("a jump table was never resolved");
        }

        var writer = new UbcArtifactWriter(new UbcHeader(
                UbcFormat.FormatVersion,
                WasmFamilyTable.Identity,
                WasmFamilyTable.ManifestIdentity,
                UbcFormat.BytecodeForm,
                WasmTranslator.TranslatorIdentity,
                WasmTranslator.TranslatorVersion))
            .Families([new UbcFamilyEntry(WasmFamilyTable.Slot, WasmFamilyTable.Identity, WasmFamilyTable.Table.TableVersion, WasmFamilyTable.ManifestIdentity)])
            .Types(types)
            .Units(units)
            .Code(System.Runtime.InteropServices.CollectionsMarshal.AsSpan(code));

        if (jumpTables.Length > 0)
        {
            writer.JumpTables(jumpTables);
        }

        writer.Entries([]);

        if (positions.Count > 0)
        {
            writer.Positions(positions);
        }

        writer.FamilyData(WasmFamilyTable.Slot, WasmFamilyData.Write(Definitions()).AsSpan());

        var bytes = writer.ToArray();

        if ((ulong)bytes.Length > byteLimit)
        {
            return TooLarge();
        }

        return WasmTranslation.Translated(System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(bytes), module);
    }

    /// <summary>The module's definitions, as the family's FamilyData section carries them.</summary>
    /// <remarks>
    /// A thirty-two-bit global's initial bits are its constant's low word, as the bare-module executor
    /// evaluated the constant, and a segment's offset is its constant's thirty-two-bit value read as
    /// unsigned; exports keep the order the module declares them in.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=2; Fingerprint=433FE8
    // Broiler-Falsified-If: a definition is written with another value than the bare-module executor evaluated for the same module, or an export's order moves
    // Broiler-Human:        PENDING
    private WasmDefinitions Definitions()
    {
        var globals = ImmutableArray.CreateBuilder<WasmGlobalDefinition>(module.GlobalCount);

        foreach (var global in module.Globals)
        {
            var kind = KindOf(global.Type.ValueType);
            var bits = kind is WasmGlobalKind.I32 or WasmGlobalKind.F32 ? (uint)global.Initializer.Bits : global.Initializer.Bits;
            globals.Add(new WasmGlobalDefinition(kind, global.Type.IsMutable, bits));
        }

        var elements = ImmutableArray.CreateBuilder<WasmElementDefinition>(module.ElementSegmentCount);

        foreach (var segment in module.Elements)
        {
            elements.Add(new WasmElementDefinition((uint)segment.Offset.Bits, ImmutableArray.Create(segment.Entries.ToArray())));
        }

        var data = ImmutableArray.CreateBuilder<WasmDataDefinition>(module.DataSegmentCount);

        foreach (var segment in module.Data)
        {
            data.Add(new WasmDataDefinition((uint)segment.Offset.Bits, segment.Contents.ToArray()));
        }

        var exports = ImmutableArray.CreateBuilder<WasmExportDefinition>(module.ExportCount);

        foreach (var export in module.Exports)
        {
            exports.Add(new WasmExportDefinition(export.Name.ToArray(), export.Kind, export.EntityIndex));
        }

        var memory = module.MemoryCount > 0 ? module.Memories[0].Limits : default;
        var table = module.TableCount > 0 ? module.Tables[0].Limits : default;

        return new WasmDefinitions(
            module.MemoryCount > 0,
            memory,
            module.TableCount > 0,
            table,
            globals.MoveToImmutable(),
            elements.MoveToImmutable(),
            data.MoveToImmutable(),
            exports.MoveToImmutable(),
            module.StartFunctionIndex < 0 ? WasmDefinitions.NoStart : (uint)module.StartFunctionIndex);
    }

    // ---- one body ---------------------------------------------------------------------------------

    /// <summary>Lowers one function's body into one unit at the Code section's current end.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=4; Fingerprint=7B112D
    // Broiler-Falsified-If: a unit declares a height other than the greatest its code reaches, locals other than the body's, a code offset other than where its code begins, or a jump table or position that is not absolute
    // Broiler-Human:        PENDING
    private bool LowerBody(int index, bool exported)
    {
        var body = module.Bodies[index];
        var typeIndex = module.FunctionTypeIndices[index];
        var signature = module.Types[(int)typeIndex];

        function = index;

        // A local is named by a sixteen-bit operand, so a unit declares at most that many, parameters
        // included.
        if ((ulong)signature.ParameterCount + (ulong)body.LocalCount > UbcFormat.MaxLocals)
        {
            return Refuse(WebAssemblyDiagnosticCode.TranslationLocalsAboveMaximum, 0);
        }

        var baseOffset = (uint)code.Count;
        builder = new UbcCodeBuilder(baseOffset);
        stacks.Clear();
        frames.Clear();
        marks.Clear();
        trampolineLabels.Clear();
        trampolines.Clear();
        unitTables.Clear();
        stack = TypedStack.Empty;
        maxHeight = 0;
        reachable = true;

        if (!LowerCode(body.Code, signature))
        {
            return false;
        }

        // The trampolines, after the body, in the order the sites first used them.
        foreach (var trampoline in trampolines)
        {
            Mark(trampoline.Label);

            if (trampoline.Target.Kind is FrameKind.Function)
            {
                builder.Emit(UbcOpcode.Return);
            }
            else
            {
                Squash(trampoline.Stack, trampoline.Target);
                builder.EmitTo(UbcOpcode.Jump, trampoline.Target.Label);
            }

            if (builder.Offset > byteLimit)
            {
                return TooLargeInBody();
            }
        }

        // Every label is marked now, so each of this unit's jump tables resolves to absolute targets.
        foreach (var (table, labels) in unitTables)
        {
            var targets = new uint[labels.Length];

            for (var at = 0; at < labels.Length; at++)
            {
                targets[at] = (uint)marks[labels[at]];
            }

            tables[table] = new UbcJumpTable((uint)index, System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(targets));
        }

        var emitted = builder.ToArray();
        code.AddRange(emitted);

        units.Add(new UbcUnit(
            typeIndex,
            WasmFamilyTable.Slot,
            Runs(body.Locals),
            (uint)maxHeight,
            0,
            baseOffset,
            (uint)emitted.Length,
            exported ? UbcUnitFlags.Entry : UbcUnitFlags.None,
            ImmutableArray<uint>.Empty));

        return true;
    }

    /// <summary>Walks one body's instructions, emitting what each reachable one lowers to.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=4; Fingerprint=18716E
    // Broiler-Falsified-If: an instruction after a terminal one is emitted before a label another instruction reaches, a body is left with a frame open, or an immediate is read with a reader other than this profile's padding-tolerant one
    // Broiler-Human:        PENDING
    private bool LowerCode(System.ReadOnlySpan<byte> instructions, WasmFuncType signature)
    {
        var bounds = new VmReadBounds((ulong)instructions.Length, 1, ulong.MaxValue, 1);
        var reader = new VmBoundedReader(instructions, in bounds, new ReadMeter(cancellationToken), WebAssemblyProfile.MaxUnchargedWork);

        frames.Add(new Frame(FrameKind.Function, -1, -1, TypedStack.Empty, Slots(signature.Results), -1, instructions.Length - 1));

        while (frames.Count > 0)
        {
            if (++steps % InstructionsBetweenPolls == 0 && cancellationToken.IsCancellationRequested)
            {
                refusal = WasmTranslation.Cancelled();
                return false;
            }

            if (!reachable && !SkipToFrameEdge(ref reader))
            {
                return false;
            }

            var at = (int)reader.Position;

            if (!reader.TryReadByte(out var opcode))
            {
                return Unreadable();
            }

            if (!Step(ref reader, opcode, at, signature))
            {
                return false;
            }

            if (builder.Offset > byteLimit)
            {
                return TooLargeInBody();
            }
        }

        return reader.Remaining == 0 || Unreadable();
    }

    /// <summary>
    /// Steps over unreachable code to the enclosing frame's <c>else</c>, when an <c>if</c>'s consequent
    /// is dead and it has one, or to its <c>end</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=712B25
    // Broiler-Falsified-If: the step lands anywhere but the enclosing frame's else or end, or moves backwards
    // Broiler-Human:        PENDING
    private bool SkipToFrameEdge(ref VmBoundedReader reader)
    {
        var frame = frames[^1];
        var edge = frame.Kind is FrameKind.If && frame.ElseAt >= 0 ? frame.ElseAt : frame.EndAt;
        var distance = (long)edge - (long)reader.Position;

        if (distance < 0)
        {
            return Unreadable();
        }

        return distance == 0 || reader.TryReadBytes((ulong)distance, out _) || Unreadable();
    }

    /// <summary>Lowers one instruction.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=3; Fingerprint=240D19
    // Broiler-Falsified-If: an instruction lowers to rows whose effect on the typed stack differs from the instruction's, or a row that can trap is emitted without its position
    // Broiler-Human:        PENDING
    private bool Step(ref VmBoundedReader reader, byte opcode, int at, WasmFuncType signature)
    {
        switch ((WasmOpcode)opcode)
        {
            case WasmOpcode.Unreachable:
                Position(at);
                builder.Emit(UbcOpcode.Trap);
                reachable = false;
                return true;

            case WasmOpcode.Nop:
                return true;

            case WasmOpcode.Block:
            case WasmOpcode.Loop:
            case WasmOpcode.If:
                return Open(ref reader, (WasmOpcode)opcode, at);

            case WasmOpcode.Else:
                return Else();

            case WasmOpcode.End:
                return End();

            case WasmOpcode.Br:
                return U32(ref reader, out var depth) && Branch(depth, at);

            case WasmOpcode.BrIf:
                return U32(ref reader, out var conditional) && BranchIf(conditional, at);

            case WasmOpcode.BrTable:
                return BranchTable(ref reader, at);

            case WasmOpcode.Return:
                builder.Emit(UbcOpcode.Return);
                reachable = false;
                return true;

            case WasmOpcode.Call:
            {
                if (!U32(ref reader, out var callee))
                {
                    return false;
                }

                var type = module.Types[(int)module.FunctionTypeIndices[(int)callee]];
                builder.Emit(UbcOpcode.Call, callee);
                stack = stacks.Pop(stack, type.ParameterCount);
                return PushAll(Slots(type.Results), at);
            }

            case WasmOpcode.CallIndirect:
            {
                if (!U32(ref reader, out var typeIndex) || !reader.TryReadByte(out _))
                {
                    return Unreadable();
                }

                var row = FamilyRow(WasmFamilyTable.CallIndirect);
                var type = module.Types[(int)typeIndex];
                Position(at);
                builder.EmitFamily(WasmFamilyTable.Slot, WasmFamilyTable.CallIndirect, row.Shape, typeIndex);
                stack = stacks.Pop(stack, row.Effect.Pops.Length + type.ParameterCount);
                return PushAll(Slots(type.Results), at);
            }

            case WasmOpcode.Drop:
                builder.Emit(UbcOpcode.Drop);
                stack = stacks.Pop(stack, 1);
                return true;

            case WasmOpcode.Select:
                // The condition and the shallower candidate go; the deeper candidate's slot stays.
                builder.Emit(UbcOpcode.Select);
                stack = stacks.Pop(stack, 2);
                return true;

            case WasmOpcode.LocalGet:
            case WasmOpcode.LocalSet:
            case WasmOpcode.LocalTee:
                return Local(ref reader, (WasmOpcode)opcode, at, signature);

            case WasmOpcode.GlobalGet:
            case WasmOpcode.GlobalSet:
                return Global(ref reader, (WasmOpcode)opcode, at);

            case WasmOpcode.MemorySize:
            case WasmOpcode.MemoryGrow:
            {
                // The reserved byte the validator required to be zero is the row's operand.
                if (!reader.TryReadByte(out var reserved))
                {
                    return Unreadable();
                }

                return FamilyRowAt(opcode, reserved, at);
            }

            case WasmOpcode.I32Const:
            {
                // Read signed within the thirty-two-bit budget, narrowed to its word as the bare
                // interpreter narrowed it.
                if (!WasmLeb128.TryReadVarS32(ref reader, out var value, out _))
                {
                    return Unreadable();
                }

                builder.Emit(UbcOpcode.ConstI32, (uint)value);
                return Listed(UbcOpcode.ConstI32, at);
            }

            case WasmOpcode.I64Const:
            {
                if (!WasmLeb128.TryReadVarS64(ref reader, out var value, out _))
                {
                    return Unreadable();
                }

                builder.Emit(UbcOpcode.ConstI64, (ulong)value);
                return Listed(UbcOpcode.ConstI64, at);
            }

            case WasmOpcode.F32Const:
            {
                if (!reader.TryReadUInt32LittleEndian(out var bits))
                {
                    return Unreadable();
                }

                builder.Emit(UbcOpcode.ConstF32, bits);
                return Listed(UbcOpcode.ConstF32, at);
            }

            case WasmOpcode.F64Const:
            {
                if (!reader.TryReadUInt64LittleEndian(out var bits))
                {
                    return Unreadable();
                }

                builder.Emit(UbcOpcode.ConstF64, bits);
                return Listed(UbcOpcode.ConstF64, at);
            }

            default:
                if (opcode is >= WasmFamilyTable.FirstAccess and <= WasmFamilyTable.LastAccess)
                {
                    // The alignment exponent and the static offset, in the row's two fields.
                    if (!U32(ref reader, out var alignment) || !U32(ref reader, out var offset))
                    {
                        return false;
                    }

                    if (alignment > byte.MaxValue)
                    {
                        return Refuse(WebAssemblyDiagnosticCode.TranslationOperandOutOfRange, at);
                    }

                    return FamilyRowAt(opcode, alignment | ((ulong)offset << 8), at);
                }

                if (opcode is >= WasmFamilyTable.FirstNumeric and <= WasmFamilyTable.LastNumeric)
                {
                    return FamilyRowAt(opcode, 0, at);
                }

                // The validator refused every other byte, so reaching here is this assembly
                // disagreeing with itself.
                return Unreadable();
        }
    }

    /// <summary>Opens a block, a loop or a conditional; nothing is emitted but a conditional's test.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2730BE
    // Broiler-Falsified-If: a loop's label is marked anywhere but where its body begins, or a conditional's test jumps anywhere but its alternative or its end
    // Broiler-Human:        PENDING
    private bool Open(ref VmBoundedReader reader, WasmOpcode opcode, int at)
    {
        // The block type, read as the signed thirty-three-bit form it is encoded in, which the
        // validator has proved is the empty type or one value type.
        if (!WasmLeb128.TryReadVarS33(ref reader, out var encoded, out _) ||
            !module.Bodies[function].TryFindJumpTarget(at, out var target))
        {
            return Unreadable();
        }

        var tag = (byte)(encoded & 0x7F);
        var results = tag == WasmTypeGrammar.EmptyBlockType
            ? ImmutableArray<UbcSlotType>.Empty
            : ImmutableArray.Create(SlotOf((WasmValueType)tag));

        var elseAt = target.ElseOffset >= 0 ? target.ElseOffset - 1 : -1;
        var endAt = target.EndOffset - 1;

        switch (opcode)
        {
            case WasmOpcode.Loop:
            {
                var label = NewLabel();
                Mark(label);
                frames.Add(new Frame(FrameKind.Loop, label, -1, stack, results, -1, endAt));
                return true;
            }

            case WasmOpcode.If:
            {
                stack = stacks.Pop(stack, UbcOpcodes.Row(UbcOpcode.JumpIfZero).Pops.Length);
                var alternative = NewLabel();
                builder.EmitTo(UbcOpcode.JumpIfZero, alternative);
                frames.Add(new Frame(FrameKind.If, NewLabel(), alternative, stack, results, elseAt, endAt));
                return true;
            }

            default:
                frames.Add(new Frame(FrameKind.Block, NewLabel(), -1, stack, results, -1, endAt));
                return true;
        }
    }

    /// <summary>Ends a conditional's consequent and begins its alternative.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=CA7C14
    // Broiler-Falsified-If: a consequent that falls into the else is not given a jump to the end, or the alternative begins with any stack but the conditional's entry
    // Broiler-Human:        PENDING
    private bool Else()
    {
        var frame = frames[^1];

        if (frame.Kind is not FrameKind.If)
        {
            return Unreadable();
        }

        if (reachable)
        {
            builder.EmitTo(UbcOpcode.Jump, frame.Label);
            frame.Branched = true;
        }

        Mark(frame.ElseLabel);
        frame.Kind = FrameKind.Else;
        stack = frame.Entry;
        reachable = true;
        return true;
    }

    /// <summary>Ends the innermost frame: marks its label where branches land, and decides whether what follows is reachable.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=9B6075
    // Broiler-Falsified-If: code after an end is emitted when nothing falls into the end and nothing branches to it, or a reachable end leaves any stack but the frame's entry and its results
    // Broiler-Human:        PENDING
    private bool End()
    {
        var frame = frames[^1];
        frames.RemoveAt(frames.Count - 1);

        bool reached;

        switch (frame.Kind)
        {
            case FrameKind.Function:
                // The function's own end: its results leave by a return when it is reached.
                if (reachable)
                {
                    builder.Emit(UbcOpcode.Return);
                }

                reachable = false;
                return true;

            case FrameKind.Loop:
                // A branch to a loop lands at its start, so only falling in reaches its end.
                reached = reachable;
                break;

            case FrameKind.If:
                // With no alternative, the test's jump lands here with the entry stack.
                Mark(frame.ElseLabel);
                Mark(frame.Label);
                reached = true;
                break;

            default:
                reached = reachable || frame.Branched;

                if (reached)
                {
                    Mark(frame.Label);
                }

                break;
        }

        reachable = reached;

        if (reached)
        {
            stack = frame.Entry;
            return PushAll(frame.Results, frame.EndAt);
        }

        return true;
    }

    /// <summary>An unconditional branch: the drop in place, then the jump, or a return from the function.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=BC5896
    // Broiler-Falsified-If: a branch arrives at its label with any slot above the label's entry but the values it carries
    // Broiler-Human:        PENDING
    private bool Branch(uint depth, int at)
    {
        var target = frames[frames.Count - 1 - (int)depth];
        reachable = false;

        if (target.Kind is FrameKind.Function)
        {
            builder.Emit(UbcOpcode.Return);
            return true;
        }

        if (target.LabelArity > SquashField)
        {
            return Refuse(WebAssemblyDiagnosticCode.TranslationSquashKeepAboveMaximum, at);
        }

        Squash(stack, target);
        builder.EmitTo(UbcOpcode.Jump, target.Label);
        Reached(target);
        return true;
    }

    /// <summary>A conditional branch: one <c>jump_if_nonzero</c>, to the label or to a trampoline.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=A1400D
    // Broiler-Falsified-If: the branch jumps straight to a label while slots must be dropped, or to a trampoline another site reaches with another stack
    // Broiler-Human:        PENDING
    private bool BranchIf(uint depth, int at)
    {
        stack = stacks.Pop(stack, UbcOpcodes.Row(UbcOpcode.JumpIfNonZero).Pops.Length);

        if (!BranchLabel(frames[frames.Count - 1 - (int)depth], at, out var label))
        {
            return false;
        }

        builder.EmitTo(UbcOpcode.JumpIfNonZero, label);
        return true;
    }

    /// <summary>A branch through a label vector: one jump table, its rows the labels in order and the default last.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=2; Fingerprint=724886
    // Broiler-Falsified-If: a table's rows are in another order than the vector's with the default last, or a table is named by an index its sixteen-bit operand cannot hold
    // Broiler-Human:        PENDING
    private bool BranchTable(ref VmBoundedReader reader, int at)
    {
        if (!U32(ref reader, out var count))
        {
            return false;
        }

        // The validator bounded and charged the count, so its labels are there to be read.
        var depths = new uint[(long)count + 1];

        for (var index = 0; index < depths.Length; index++)
        {
            if (!U32(ref reader, out depths[index]))
            {
                return false;
            }
        }

        if (tables.Count >= UbcFormat.MaxJumpTables)
        {
            return Refuse(WebAssemblyDiagnosticCode.TranslationJumpTablesAboveMaximum, at);
        }

        stack = stacks.Pop(stack, UbcOpcodes.Row(UbcOpcode.JumpTable).Pops.Length);

        var labels = new int[depths.Length];

        for (var index = 0; index < depths.Length; index++)
        {
            if (!BranchLabel(frames[frames.Count - 1 - (int)depths[index]], at, out labels[index]))
            {
                return false;
            }
        }

        var table = tables.Count;
        tables.Add(null);
        unitTables.Add((table, labels));
        builder.Emit(UbcOpcode.JumpTable, (ulong)table);
        reachable = false;
        return true;
    }

    /// <summary>
    /// Where a branch from the current stack to <paramref name="target"/> jumps: the label itself when
    /// nothing is to be dropped and the target is not the function, else the trampoline for the label
    /// and this stack.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=7578AC
    // Broiler-Falsified-If: two sites with different typed stacks share a trampoline, or a site jumps straight to a label it reaches with slots to drop
    // Broiler-Human:        PENDING
    private bool BranchLabel(Frame target, int at, out int label)
    {
        label = -1;

        if (target.LabelArity > SquashField)
        {
            return Refuse(WebAssemblyDiagnosticCode.TranslationSquashKeepAboveMaximum, at);
        }

        Reached(target);

        if (target.Kind is not FrameKind.Function &&
            stacks.Height(stack) - stacks.Height(target.Entry) == target.LabelArity)
        {
            label = target.Label;
            return true;
        }

        // The function's label has no builder label of its own, so its key is minus one, which no
        // other label is.
        var key = ((long)target.Label << 32) | (uint)stack;

        if (!trampolineLabels.TryGetValue(key, out label))
        {
            label = NewLabel();
            trampolineLabels.Add(key, label);
            trampolines.Add(new Trampoline(label, target, stack));
        }

        return true;
    }

    /// <summary>
    /// Drops the slots between <paramref name="target"/>'s entry and the values a branch to it carries,
    /// from <paramref name="from"/>, in pieces one <c>squash</c> can state; the branch site has already
    /// refused a count of carried values its keep byte cannot hold.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=916AA4
    // Broiler-Falsified-If: the pieces drop more or fewer slots than lie between the entry and the carried values, or a piece states a count its byte cannot hold
    // Broiler-Human:        PENDING
    private void Squash(int from, Frame target)
    {
        var keep = target.LabelArity;
        var drop = stacks.Height(from) - stacks.Height(target.Entry) - keep;

        while (drop > 0)
        {
            var piece = System.Math.Min(drop, SquashField);
            builder.Emit(UbcOpcode.Squash, (uint)piece | ((ulong)(uint)keep << 8));
            drop -= piece;
        }
    }

    /// <summary>Records that a branch reaches <paramref name="target"/>'s label, which makes a block's end reachable.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=99A902
    // Broiler-Human:        PENDING
    private static void Reached(Frame target) => target.Branched = true;

    /// <summary>A local access: the common row, typed by the parameter or the declared local it names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=049CE6
    // Broiler-Falsified-If: a local is typed as anything but its parameter's or its declaration's type, or an index is written past its sixteen-bit field
    // Broiler-Human:        PENDING
    private bool Local(ref VmBoundedReader reader, WasmOpcode opcode, int at, WasmFuncType signature)
    {
        if (!U32(ref reader, out var index))
        {
            return false;
        }

        if (index > ushort.MaxValue)
        {
            return Refuse(WebAssemblyDiagnosticCode.TranslationOperandOutOfRange, at);
        }

        var type = index < (uint)signature.ParameterCount
            ? signature.Parameters[(int)index]
            : module.Bodies[function].Locals[(int)(index - (uint)signature.ParameterCount)];

        switch (opcode)
        {
            case WasmOpcode.LocalGet:
                builder.Emit(UbcOpcode.LocalGet, index);
                return Push(SlotOf(type), at);

            case WasmOpcode.LocalSet:
                builder.Emit(UbcOpcode.LocalSet, index);
                stack = stacks.Pop(stack, 1);
                return true;

            default:
                builder.Emit(UbcOpcode.LocalTee, index);
                return true;
        }
    }

    /// <summary>A global access: the family's row for the global's type.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=F81D13
    // Broiler-Falsified-If: a global is read or written through the row of another type than its own
    // Broiler-Human:        PENDING
    private bool Global(ref VmBoundedReader reader, WasmOpcode opcode, int at)
    {
        if (!U32(ref reader, out var index))
        {
            return false;
        }

        var kind = KindOf(module.Globals[(int)index].Type.ValueType);
        var row = (byte)(WasmFamilyTable.FirstGlobal + (2 * (int)kind) + (opcode is WasmOpcode.GlobalSet ? 1 : 0));
        return FamilyRowAt(row, index, at);
    }

    /// <summary>
    /// A row of the family's table with a listed effect: emitted with its operand at the row's shape,
    /// with a position when the row maps a trap, and applied to the typed stack as its effect says.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=81FC50
    // Broiler-Falsified-If: a row that maps a trap is emitted without a position, or the typed stack moves otherwise than the row's effect
    // Broiler-Human:        PENDING
    private bool FamilyRowAt(byte opcode, ulong operand, int at)
    {
        var row = FamilyRow(opcode);

        if (!row.Traps.IsDefaultOrEmpty)
        {
            Position(at);
        }

        builder.EmitFamily(WasmFamilyTable.Slot, opcode, row.Shape, operand);
        stack = stacks.Pop(stack, row.Effect.Pops.Length);
        return PushAll(row.Effect.Pushes, at);
    }

    /// <summary>A common row with a listed effect, applied to the typed stack as the common table states it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9DF25D
    // Broiler-Human:        PENDING
    private bool Listed(UbcOpcode opcode, int at)
    {
        var row = UbcOpcodes.Row(opcode);
        stack = stacks.Pop(stack, row.Pops.Length);
        return PushAll(row.Pushes, at);
    }

    /// <summary>The family table's row of <paramref name="opcode"/>; the table defines every byte this lowering emits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=31A57C
    // Broiler-Human:        PENDING
    private static UbcInstructionRow FamilyRow(byte opcode) =>
        WasmFamilyTable.Table.TryGetRow(opcode, out var row)
            ? row
            : throw new System.InvalidOperationException($"the WebAssembly family table has no row 0x{opcode:X2}");

    // ---- the typed stack ---------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=90A9AC
    // Broiler-Human:        PENDING
    private bool PushAll(ImmutableArray<UbcSlotType> types, int at)
    {
        foreach (var type in types)
        {
            if (!Push(type, at))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Pushes one slot, keeping the greatest height, and refuses a height a unit cannot declare.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=22E46C
    // Broiler-Falsified-If: a height above the format's greatest is admitted, or the unit's declared height is lower than a height its code reaches
    // Broiler-Human:        PENDING
    private bool Push(UbcSlotType type, int at)
    {
        stack = stacks.Push(stack, type);
        var height = stacks.Height(stack);

        if (height > UbcFormat.MaxHeight)
        {
            return Refuse(WebAssemblyDiagnosticCode.TranslationOperandHeightAboveMaximum, at);
        }

        maxHeight = System.Math.Max(maxHeight, height);
        return true;
    }

    // ---- labels, positions and refusals ------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=94A3E1
    // Broiler-Human:        PENDING
    private int NewLabel()
    {
        marks.Add(-1);
        return builder.NewLabel();
    }

    /// <summary>Marks <paramref name="label"/> where the next instruction goes, and remembers that absolute offset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7307D4
    // Broiler-Human:        PENDING
    private void Mark(int label)
    {
        builder.Mark(label);
        marks[label] = builder.Offset;
    }

    /// <summary>A Positions row for the instruction about to be emitted: its absolute offset, the function and the body-relative offset.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=C89882
    // Broiler-Falsified-If: a row names an offset other than the absolute one the instruction is emitted at
    // Broiler-Human:        PENDING
    private void Position(int at) =>
        positions.Add(new UbcPosition((uint)function, builder.Offset, function, at));

    /// <summary>Reads an unsigned thirty-two-bit immediate with this profile's padding-tolerant reader.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=6ECB43
    // Broiler-Human:        PENDING
    private bool U32(ref VmBoundedReader reader, out uint value) =>
        WasmLeb128.TryReadVarU32(ref reader, out value, out _) || Unreadable();

    /// <summary>
    /// A body the validator admitted could not be read again: a cancellation noticed by the reader's
    /// poll, or this assembly disagreeing with itself.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0F0F05
    // Broiler-Human:        PENDING
    private bool Unreadable()
    {
        refusal = cancellationToken.IsCancellationRequested ? WasmTranslation.Cancelled() : WasmTranslation.Defect();
        return false;
    }

    /// <summary>A module the universal bytecode cannot hold, refused where the validator places its own refusals in a body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=CF3AC5
    // Broiler-Human:        PENDING
    private bool Refuse(WebAssemblyDiagnosticCode code, int at)
    {
        refusal = WasmTranslation.Invalid(
            VmReason.UnknownFeature,
            code,
            new VmSourcePosition(sectionIndex: -1, byteOffset: (ulong)at, profileCoordinate0: (int)WasmSectionId.Code, profileCoordinate1: function));
        return false;
    }

    /// <summary>The artifact would pass the artifact-bytes ceiling the core verifies it under: its exhaustion at artifact scope.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A21735
    // Broiler-Human:        PENDING
    private static WasmTranslation TooLarge() =>
        WasmTranslation.Exhausted(VmReason.CeilingReached, VmBudgetDimension.ArtifactBytes, VmBudgetScope.Artifact);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=7BD2DD
    // Broiler-Human:        PENDING
    private bool TooLargeInBody()
    {
        refusal = TooLarge();
        return false;
    }

    // ---- the vocabulary ------------------------------------------------------------------------------

    /// <summary>The word slot a value type is held in; the validator admitted no other value type.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=18FD31
    // Broiler-Human:        PENDING
    private static UbcSlotType SlotOf(WasmValueType type) => type switch
    {
        WasmValueType.I32 => UbcSlotType.I32,
        WasmValueType.I64 => UbcSlotType.I64,
        WasmValueType.F32 => UbcSlotType.F32,
        WasmValueType.F64 => UbcSlotType.F64,
        _ => throw new System.InvalidOperationException($"no word slot holds value type 0x{(byte)type:X2}"),
    };

    /// <summary>The module definitions' type byte for a global of <paramref name="type"/>.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=762064
    // Broiler-Human:        PENDING
    private static WasmGlobalKind KindOf(WasmValueType type) => type switch
    {
        WasmValueType.I32 => WasmGlobalKind.I32,
        WasmValueType.I64 => WasmGlobalKind.I64,
        WasmValueType.F32 => WasmGlobalKind.F32,
        WasmValueType.F64 => WasmGlobalKind.F64,
        _ => throw new System.InvalidOperationException($"no global holds value type 0x{(byte)type:X2}"),
    };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=405AEC
    // Broiler-Human:        PENDING
    private static ImmutableArray<UbcSlotType> Slots(System.ReadOnlySpan<WasmValueType> types)
    {
        var slots = new UbcSlotType[types.Length];

        for (var index = 0; index < slots.Length; index++)
        {
            slots[index] = SlotOf(types[index]);
        }

        return System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(slots);
    }

    /// <summary>The body's declared locals, parameters excluded as the decoder already excluded them, as runs of one type.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E3F13A
    // Broiler-Falsified-If: the runs expand to other types, in another order or in another number than the body's locals
    // Broiler-Human:        PENDING
    private static ImmutableArray<UbcLocalRun> Runs(System.ReadOnlySpan<WasmValueType> locals)
    {
        var runs = ImmutableArray.CreateBuilder<UbcLocalRun>();
        var at = 0;

        while (at < locals.Length)
        {
            var type = locals[at];
            var start = at;

            while (at < locals.Length && locals[at] == type)
            {
                at++;
            }

            runs.Add(new UbcLocalRun((uint)(at - start), SlotOf(type)));
        }

        return runs.ToImmutable();
    }

    /// <summary>One frame of the control stack.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1470A7
    // Broiler-Human:        PENDING
    private sealed class Frame(
        FrameKind kind, int label, int elseLabel, int entry, ImmutableArray<UbcSlotType> results, int elseAt, int endAt)
    {
        /// <summary>What the frame stands for; a conditional's becomes an alternative at its <c>else</c>.</summary>
        internal FrameKind Kind { get; set; } = kind;

        /// <summary>The label a branch to the frame jumps to: a loop's start, anything else's end; none for the function.</summary>
        internal int Label { get; } = label;

        /// <summary>A conditional's alternative, or its end when it has none: where its test jumps.</summary>
        internal int ElseLabel { get; } = elseLabel;

        /// <summary>The typed stack at the frame's entry, a conditional's test already taken.</summary>
        internal int Entry { get; } = entry;

        /// <summary>The values the frame leaves when it ends.</summary>
        internal ImmutableArray<UbcSlotType> Results { get; } = results;

        /// <summary>The offset of a conditional's <c>else</c> in the body, or minus one.</summary>
        internal int ElseAt { get; } = elseAt;

        /// <summary>The offset of the frame's <c>end</c> in the body.</summary>
        internal int EndAt { get; } = endAt;

        /// <summary>Whether a branch reached the frame's label, or a consequent jumped to its end.</summary>
        internal bool Branched { get; set; }

        /// <summary>How many values a branch to the frame carries: none to a loop, its results to anything else.</summary>
        // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=0; Fingerprint=45F18E
        // Broiler-Falsified-If: a branch to a loop is said to carry a value, or a branch to a block fewer than its results
        // Broiler-Human:        PENDING
        internal int LabelArity => Kind is FrameKind.Loop ? 0 : Results.Length;
    }

    /// <summary>A trampoline: its label, the frame it branches to and the typed stack every site reaches it with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=317CBA
    // Broiler-Human:        PENDING
    private sealed record Trampoline(int Label, Frame Target, int Stack);

    /// <summary>
    /// Typed stacks as shared, interned nodes: a node is a slot type over the node beneath it, and two
    /// stacks of the same types, bottom to top, are one node, so comparing two stacks is comparing two
    /// integers.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BBC357
    // Broiler-Falsified-If: two stacks of different types or heights are one node, or two of the same types are two
    // Broiler-Human:        PENDING
    private sealed class TypedStack
    {
        /// <summary>The empty stack.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=BC9006
        // Broiler-Human:        PENDING
        internal const int Empty = 0;

        private readonly List<int> below = new();
        private readonly List<int> heights = new();
        private readonly Dictionary<long, int> interned = new();

        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=09EBCD
        // Broiler-Human:        PENDING
        internal TypedStack() => Clear();

        /// <summary>Forgets every node but the empty stack.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0BC866
        // Broiler-Human:        PENDING
        internal void Clear()
        {
            below.Clear();
            heights.Clear();
            interned.Clear();
            below.Add(-1);
            heights.Add(0);
        }

        /// <summary>The node of <paramref name="type"/> over <paramref name="node"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=D2A56B
        // Broiler-Falsified-If: a push answers a node whose slot beneath or whose type is not the one asked for
        // Broiler-Human:        PENDING
        internal int Push(int node, UbcSlotType type)
        {
            var key = ((long)node << 8) | (byte)type;

            if (!interned.TryGetValue(key, out var pushed))
            {
                pushed = below.Count;
                below.Add(node);
                heights.Add(heights[node] + 1);
                interned.Add(key, pushed);
            }

            return pushed;
        }

        /// <summary>The node <paramref name="count"/> slots beneath <paramref name="node"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7EED75
        // Broiler-Human:        PENDING
        internal int Pop(int node, int count)
        {
            for (var index = 0; index < count; index++)
            {
                node = below[node];
            }

            return node;
        }

        /// <summary>How many slots <paramref name="node"/> holds.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=83DDDE
        // Broiler-Human:        PENDING
        internal int Height(int node) => heights[node];
    }

    /// <summary>
    /// The meter the lowering's reader charges: it admits every charge, because the validator already
    /// paid for reading these bytes, and its poll is the cancellation token.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1B5E69
    // Broiler-Human:        PENDING
    private sealed class ReadMeter(System.Threading.CancellationToken cancellationToken) : IVmBoundedAllocationMeter
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=8B6B9A
        // Broiler-Human:        PENDING
        public bool TryReserve(ulong byteCount) => true;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B71722
        // Broiler-Human:        PENDING
        public void Release(ulong byteCount)
        {
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1B7690
        // Broiler-Human:        PENDING
        public bool TryChargeWork(ulong workUnits) => true;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=BB3A6A
        // Broiler-Human:        PENDING
        public bool Poll() => !cancellationToken.IsCancellationRequested;
    }
}
