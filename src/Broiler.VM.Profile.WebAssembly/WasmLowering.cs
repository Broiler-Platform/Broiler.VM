// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   61
// Annotated:        61/61
// Exempt:           55
// Human-reviewed:   0/61
// IP risk:          Low
// Security risk:    Critical
// Criteria:         32/30
// Resource impact:  5/10 max
// Unverified:       61
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Abstractions;
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
/// <b>THE WALK OVER EACH BODY IS THE VALIDATOR'S, REPEATED WITH HEIGHTS INSTEAD OF CHECKS.</b> The
/// validator hands over each body's resolved block targets and nothing of its operand stack, so the
/// lowering tracks the stack again, from the function's entry, over the code the validator has
/// already admitted. Every immediate is read again with this profile's own padding-tolerant readers,
/// because a module may pad an integer within its byte budget and the core's canonical readers would
/// refuse it; nothing a body says is trusted that the validator did not first check.
/// </para>
/// <para>
/// <b>The stack it tracks is the walk's, slot for slot.</b> The universal bytecode walk makes a new
/// slot for every value an instruction pushes and takes a popped slot's predecessor back unchanged,
/// and it compares two arrivals at one instruction slot by slot, at a cost of the whole height,
/// unless they are the very same stack. The lowering gives every pushed slot an identity of its own
/// the same way, so two stacks with one top identity are one stack of the walk's, and it arranges
/// that every instruction more than one edge reaches is reached with one such stack: an arrival costs
/// nothing to compare, however tall the stack beneath it.
/// </para>
/// <para>
/// <b>Structured instructions emit nothing</b>, and a stack of control frames stands for them: each
/// frame's label, the height at its entry, the values a branch to it carries and whether any branch
/// reached it. A <c>loop</c>'s label is the offset of what follows it; every other label is marked
/// where its <c>end</c> is. An <c>if</c> is a <c>jump_if_zero</c> to its alternative or its end, and
/// an <c>else</c> a <c>jump</c> to the end when the consequent falls into it.
/// </para>
/// <para>
/// <b>Every edge into a label arrives with the frame's entry stack.</b> Beneath a frame's entry
/// nothing inside the frame pops, so dropping to the entry height reaches the entry's own stack. A
/// value a branch carries to a block would be a slot of its own on each edge, so it is carried in a
/// scratch local instead: a frame that a branch, or a consequent's jump, carries a value to stores
/// the value on every edge into its end, falls into its label with the entry stack, and loads the
/// value after it. The scratch locals - one per carried position and word type a body needs - follow
/// the body's own locals. A branch to the function's own label is a <c>return</c>, which drops
/// whatever lies under the results.
/// </para>
/// <para>
/// <b>A branch drops with <c>squash</c>, two hundred and fifty-five slots at a time, down chains the
/// branches of a body share whatever label they go to.</b> A drop within one <c>squash</c> of the
/// target's entry is made in place. A longer one names its target in a selector local and descends
/// through the frames beneath it: to the entry of the innermost frame below the stack - the floor -
/// by way of the heights that are multiples of two hundred and fifty-five, each step a trampoline
/// keyed by the floor and the identity of the slot it starts from, and at the floor a dispatch reads
/// the selector: to the target's label when the floor's entry is the target's, or on to the next
/// floor down with the selector set for that floor. So every slot a body pushes is dropped by one
/// chain however many labels lie beneath it, and the code a body's branches need grows with its
/// branches, its pushes and its frames, never with a product of two of them. A conditional branch that
/// has nothing to drop and carries nothing is one <c>jump_if_nonzero</c> to the label; any other
/// conditional branch goes to a trampoline keyed by the label and the identity of its stack: the
/// stores, the first drop, the selector and the jump, or the <c>return</c>. Trampolines follow the
/// body in the order they were first needed, and the floors' dispatches follow them.
/// </para>
/// <para>
/// <b>A <c>br_table</c> does its storing and its first drop once, under its selector</b>, down to the
/// slot its rows share, so its rows' trampolines are keyed by that slot and shared with every site
/// that reaches it, and a site costs its own rows whatever the number of its targets. When a row goes
/// to a frame entered at the very stack under the selector and another row deeper, the table keeps
/// its selector in the selector local, sends the deeper rows to one step of the site's own that drops
/// to their shared slot, and dispatches again there through a second table, shared by the sites that
/// reach that slot with those rows.
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
/// per function in function-index order, its declared locals and its scratch locals as runs, the
/// exact greatest word height its emitted code reaches, the entry flag when the function is exported;
/// the code, every unit built at its absolute offset so jump targets, jump tables and positions are
/// all absolute; the jump tables, unit by unit, one per <c>br_table</c> and per second dispatch in
/// the order they were needed, then one per floor that tells targets apart; an empty Entries
/// section, because exports are found by the family through its definitions; one Positions row per
/// emitted row that can trap, naming the function and the instruction's offset inside its body, in
/// ascending order; and the module's definitions.
/// </para>
/// <para>
/// <b>What the universal bytecode cannot hold is refused here</b>, with this profile's codes in the
/// translation band: more locals than a unit declares, scratch locals included, a stack above the
/// height a unit declares, more branch tables than an artifact names, an immediate its operand field
/// cannot hold. An artifact larger than the artifact-bytes ceiling is refused as the core would refuse
/// it, as an exhaustion of that ceiling at artifact scope, before it is allocated whole.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=5; Fingerprint=FB896C
// Broiler-Falsified-If: an artifact is written whose walk refuses it for a module the validator admitted, whose execution answers differently from the bare-module interpreter's other than for the float comparisons, whose walk compares two different stacks at one instruction, or two translations of one module differ in a byte
// Broiler-Human:        PENDING
internal sealed class WasmLowering
{
    /// <summary>The most slots one <c>squash</c> drops: its count is one byte. The drop chain's heights are its multiples.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=20709D
    // Broiler-Human:        PENDING
    private const int SquashField = 255;

    /// <summary>How many instructions the lowering steps between two looks at the cancellation token.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C323FC
    // Broiler-Human:        PENDING
    private const int InstructionsBetweenPolls = 4096;

    /// <summary>The values a trampoline that stores nothing stores.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1E773C
    // Broiler-Human:        PENDING
    private static readonly int[] NoStores = [];

    private readonly WasmModule module;
    private readonly ulong byteLimit;
    private readonly System.Threading.CancellationToken cancellationToken;
    private readonly List<byte> code = new();
    private readonly List<UbcUnit> units = new();
    private readonly List<UbcJumpTable?> tables = new();
    private readonly List<UbcPosition> positions = new();
    private readonly List<long> slots = new();
    private readonly List<Frame> frames = new();
    private readonly List<long> marks = new();
    private readonly Dictionary<(int Label, long Slot, bool Stores), int> exits = new();
    private readonly Dictionary<(int Floor, long Slot), int> chains = new();
    private readonly Dictionary<(long Slot, bool Loads), int> returns = new();
    private readonly List<Trampoline> trampolines = new();
    private readonly Dictionary<int, Floor> floors = new();
    private readonly List<Floor> floorOrder = new();
    private readonly Dictionary<(long Slot, string Rows), int> redispatches = new();
    private readonly List<(int Table, int[] Labels)> unitTables = new();
    private readonly List<UbcSlotType> scratch = new();
    private readonly Dictionary<(int Position, UbcSlotType Type), int> scratchLocals = new();
    private UbcCodeBuilder builder = new();
    private WasmTranslation? refusal;
    private int function;
    private int height;
    private long lastSlot;
    private int localBase;
    private int selector;
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
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=4; Fingerprint=FC0786
    // Broiler-Falsified-If: a unit declares a height other than the greatest its code reaches, locals other than the body's followed by its scratch locals, a code offset other than where its code begins, or a jump table or position that is not absolute
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
        slots.Clear();
        frames.Clear();
        marks.Clear();
        exits.Clear();
        chains.Clear();
        returns.Clear();
        trampolines.Clear();
        floors.Clear();
        floorOrder.Clear();
        redispatches.Clear();
        unitTables.Clear();
        scratch.Clear();
        scratchLocals.Clear();
        height = 0;
        lastSlot = 0;
        localBase = signature.ParameterCount + body.LocalCount;
        selector = -1;
        maxHeight = 0;
        reachable = true;

        if (!LowerCode(body.Code, signature))
        {
            return false;
        }

        // The trampolines, after the body, in the order the sites first needed them.
        foreach (var trampoline in trampolines)
        {
            Mark(trampoline.Label);

            switch (trampoline.Kind)
            {
                case TrampolineKind.Return:
                    foreach (var local in trampoline.Stores)
                    {
                        builder.Emit(UbcOpcode.LocalGet, (uint)local);
                    }

                    builder.Emit(UbcOpcode.Return);
                    break;

                case TrampolineKind.Redispatch:
                    if (trampoline.Route.Drop > 0)
                    {
                        builder.Emit(UbcOpcode.Squash, (uint)trampoline.Route.Drop);
                    }

                    builder.Emit(UbcOpcode.LocalGet, (uint)selector);
                    builder.Emit(UbcOpcode.JumpTable, (ulong)trampoline.Table);
                    break;

                default:
                    Store(trampoline.Stores);
                    Follow(trampoline.Route);
                    break;
            }

            if (builder.Offset > byteLimit)
            {
                return TooLargeInBody();
            }
        }

        // The floors' dispatches, in the order the floors were first needed: a floor with one target
        // goes there at once, and one with more tells them apart by the selector, through a jump
        // table of its own.
        foreach (var floor in floorOrder)
        {
            Mark(floor.Dispatch);

            if (floor.Entries.Count == 1)
            {
                builder.EmitTo(UbcOpcode.Jump, floor.Entries[0]);
            }
            else
            {
                if (tables.Count >= UbcFormat.MaxJumpTables)
                {
                    return Refuse(WebAssemblyDiagnosticCode.TranslationJumpTablesAboveMaximum, floor.At);
                }

                var table = tables.Count;
                tables.Add(null);
                unitTables.Add((table, floor.Entries.ToArray()));
                builder.Emit(UbcOpcode.LocalGet, (uint)selector);
                builder.Emit(UbcOpcode.JumpTable, (ulong)table);
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
            Runs(body.Locals, scratch),
            (uint)maxHeight,
            0,
            baseOffset,
            (uint)emitted.Length,
            exported ? UbcUnitFlags.Entry : UbcUnitFlags.None,
            ImmutableArray<uint>.Empty));

        return true;
    }

    /// <summary>Walks one body's instructions, emitting what each reachable one lowers to.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=4; Fingerprint=CA09CF
    // Broiler-Falsified-If: an instruction after a terminal one is emitted before a label another instruction reaches, a body is left with a frame open, an immediate is read with a reader other than this profile's padding-tolerant one, or the code and the trampolines it has asked for pass the byte limit without a refusal
    // Broiler-Human:        PENDING
    private bool LowerCode(System.ReadOnlySpan<byte> instructions, WasmFuncType signature)
    {
        var bounds = new VmReadBounds((ulong)instructions.Length, 1, ulong.MaxValue, 1);
        var reader = new VmBoundedReader(instructions, in bounds, new ReadMeter(cancellationToken), WebAssemblyProfile.MaxUnchargedWork);

        frames.Add(new Frame(FrameKind.Function, -1, -1, 0, Slots(signature.Results), -1, instructions.Length - 1));

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

            if (!Step(ref reader, opcode, at))
            {
                return false;
            }

            // Every trampoline and every dispatch asked for is at least one byte after the body, so
            // the artifact is already past the limit when these are.
            if ((ulong)builder.Offset + (ulong)trampolines.Count + (ulong)floorOrder.Count > byteLimit)
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
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=3; Fingerprint=00A8BE
    // Broiler-Falsified-If: an instruction lowers to rows whose effect on the stack differs from the instruction's, a slot the walk makes anew keeps its old identity, or a row that can trap is emitted without its position
    // Broiler-Human:        PENDING
    private bool Step(ref VmBoundedReader reader, byte opcode, int at)
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
                return Else(at);

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
                return Pop(type.ParameterCount) && Push(type.ResultCount, at);
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
                return Pop(row.Effect.Pops.Length + type.ParameterCount) && Push(type.ResultCount, at);
            }

            case WasmOpcode.Drop:
                builder.Emit(UbcOpcode.Drop);
                return Pop(1);

            case WasmOpcode.Select:
                // The walk pops the condition and both candidates and pushes a new slot for the one
                // chosen, so the chosen slot is a new one here too.
                builder.Emit(UbcOpcode.Select);
                return Pop(3) && Push(1, at);

            case WasmOpcode.LocalGet:
            case WasmOpcode.LocalSet:
            case WasmOpcode.LocalTee:
                return Local(ref reader, (WasmOpcode)opcode, at);

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=16EF88
    // Broiler-Falsified-If: a loop's label is marked anywhere but where its body begins, a conditional's test jumps anywhere but its alternative or its end, or a frame's entry height is taken before a conditional's test is popped
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
                frames.Add(new Frame(FrameKind.Loop, label, -1, height, results, -1, endAt));
                return true;
            }

            case WasmOpcode.If:
            {
                if (!Pop(UbcOpcodes.Row(UbcOpcode.JumpIfZero).Pops.Length))
                {
                    return false;
                }

                var alternative = NewLabel();
                builder.EmitTo(UbcOpcode.JumpIfZero, alternative);
                frames.Add(new Frame(FrameKind.If, NewLabel(), alternative, height, results, elseAt, endAt));
                return true;
            }

            default:
                frames.Add(new Frame(FrameKind.Block, NewLabel(), -1, height, results, -1, endAt));
                return true;
        }
    }

    /// <summary>Ends a conditional's consequent and begins its alternative.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=9088C2
    // Broiler-Falsified-If: a consequent that falls into the else is not given a jump to the end, the jump arrives with any stack but the conditional's entry stack, or the alternative begins with any stack but that one
    // Broiler-Human:        PENDING
    private bool Else(int at)
    {
        var frame = frames[^1];

        if (frame.Kind is not FrameKind.If)
        {
            return Unreadable();
        }

        if (reachable)
        {
            // The consequent's results reach the end in the scratch locals, so the jump arrives with
            // the entry stack like every other edge into the end.
            if (frame.Results.Length > 0)
            {
                if (!Scratch(frame, at))
                {
                    return false;
                }

                Store(frame.Scratch!);

                if (!Pop(frame.Results.Length))
                {
                    return false;
                }
            }

            builder.EmitTo(UbcOpcode.Jump, frame.Label);
            frame.Branched = true;
        }

        Mark(frame.ElseLabel);
        frame.Kind = FrameKind.Else;
        height = frame.EntryHeight;
        reachable = true;
        return true;
    }

    /// <summary>Ends the innermost frame: marks its label where branches land, and decides whether what follows is reachable.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=E8EABD
    // Broiler-Falsified-If: code after an end is emitted when nothing falls into the end and nothing branches to it, an edge into a label arrives with any stack but the frame's entry stack, or a reachable end leaves any stack but the frame's entry and its results
    // Broiler-Human:        PENDING
    private bool End()
    {
        var frame = frames[^1];
        frames.RemoveAt(frames.Count - 1);

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
                // A branch to a loop lands at its start, so only falling in reaches its end, and the
                // stack goes on as it fell in.
                return true;

            case FrameKind.If:
                // With no alternative, the test's jump lands here with the entry stack, and a
                // conditional with no alternative carries nothing.
                Mark(frame.ElseLabel);
                Mark(frame.Label);
                height = frame.EntryHeight;
                reachable = true;
                return true;
        }

        if (frame.Scratch is { } scratchLocals)
        {
            // Every edge in stored the carried values: so does falling in, then the label is reached
            // with the entry stack, and the values are loaded after it.
            if (reachable)
            {
                Store(scratchLocals);

                if (!Pop(scratchLocals.Length))
                {
                    return false;
                }
            }

            Mark(frame.Label);
            height = frame.EntryHeight;
            reachable = true;

            foreach (var local in scratchLocals)
            {
                builder.Emit(UbcOpcode.LocalGet, (uint)local);

                if (!Push(1, frame.EndAt))
                {
                    return false;
                }
            }

            return true;
        }

        if (frame.Branched)
        {
            // Every edge that carries a value chose the scratch protocol first, so a frame branched to
            // without it carries nothing; anything else is this assembly disagreeing with itself.
            if (frame.LabelArity > 0)
            {
                return Unreadable();
            }

            // Nothing is carried, so every branch, and falling in, arrive with the entry stack.
            Mark(frame.Label);
            height = frame.EntryHeight;
            reachable = true;
        }

        return true;
    }

    /// <summary>An unconditional branch: the stores and the first drop in place, then the jump, or a return from the function.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=0890AF
    // Broiler-Falsified-If: a branch arrives at its label with any stack but the frame's entry stack, or a branch to the function's own label is anything but a return
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

        Reached(target);

        if (target.LabelArity > 0)
        {
            if (!Scratch(target, at))
            {
                return false;
            }

            Store(target.Scratch!);

            if (!Pop(target.LabelArity))
            {
                return false;
            }
        }

        if (!Descend(target, height, at, out var route))
        {
            return false;
        }

        Follow(route);
        return true;
    }

    /// <summary>A conditional branch: one <c>jump_if_nonzero</c>, to the label or to a trampoline.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=5AE77B
    // Broiler-Falsified-If: the branch jumps straight to a label while slots must be dropped or values stored, or to a trampoline another site reaches with another stack
    // Broiler-Human:        PENDING
    private bool BranchIf(uint depth, int at)
    {
        if (!Pop(UbcOpcodes.Row(UbcOpcode.JumpIfNonZero).Pops.Length) ||
            !BranchTarget(frames[frames.Count - 1 - (int)depth], at, out var label))
        {
            return false;
        }

        builder.EmitTo(UbcOpcode.JumpIfNonZero, label);
        return true;
    }

    /// <summary>
    /// A branch through a label vector: the carried value stored and the stack dropped once, under the
    /// selector, to a slot every target's trampolines are keyed by, then one jump table, its rows the
    /// labels in order and the default last.
    /// </summary>
    /// <remarks>
    /// Every label of one vector carries the same values, and a block carries at most one: it is
    /// swapped above the selector and stored in the scratch local every target's end loads it from, and
    /// a target that is the function's own label loads it back and returns. The drop goes to the
    /// highest target entry or to the nearest multiple of <see cref="SquashField"/> below the stack,
    /// whichever is higher - never more than one <c>squash</c> - so the targets' own descents start
    /// from a slot the sites of the same frame and height share, and a table costs its own rows and
    /// no trampoline per target of its own.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=2; Fingerprint=057521
    // Broiler-Falsified-If: a table's rows are in another order than the vector's with the default last, a table is named by an index its sixteen-bit operand cannot hold, the drop reaches below a target's entry, or a row's target is reached with any stack but the one the table leaves
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

        var targets = new Frame[depths.Length];
        Frame? block = null;
        var highest = 0;

        for (var index = 0; index < depths.Length; index++)
        {
            var target = frames[frames.Count - 1 - (int)depths[index]];
            targets[index] = target;

            if (target.Kind is not FrameKind.Function)
            {
                block = target;
                highest = System.Math.Max(highest, target.EntryHeight);
            }
        }

        var labels = new int[depths.Length];

        if (block is null)
        {
            // Every row returns, and a return drops whatever lies under the results.
            if (!Pop(UbcOpcodes.Row(UbcOpcode.JumpTable).Pops.Length))
            {
                return false;
            }

            System.Array.Fill(labels, Return(NoStores));
            return Table(labels);
        }

        var stored = NoStores;

        if (block.LabelArity > 0)
        {
            // A block carries at most one value, which the validator proved; a swap lifts it above
            // the selector.
            if (block.LabelArity != 1)
            {
                return Unreadable();
            }

            if (!Scratch(block, at))
            {
                return false;
            }

            stored = block.Scratch!;
            builder.Emit(UbcOpcode.Swap);
            Store(stored);

            // The swap makes two new slots and the store takes the carried one.
            if (!Pop(2) || !Push(1, at))
            {
                return false;
            }
        }

        // Every target but a return that stores needs the scratch protocol at its end, from the same
        // scratch local as the others, because the value is stored once for them all.
        foreach (var target in targets)
        {
            if (stored.Length > 0 && target.Kind is not FrameKind.Function && !Scratch(target, at))
            {
                return false;
            }
        }

        // The selector is on top of the stack the targets are reached from.
        var under = height - 1;

        if (highest == under && System.Array.Exists(targets, target => target.Kind is FrameKind.Function || target.EntryHeight < under))
        {
            return SplitTable(targets, stored, under, at);
        }

        var anchor = System.Math.Max(under > 0 ? (under - 1) / SquashField * SquashField : 0, highest);

        if (anchor < under)
        {
            // The selector is kept, as a new slot, over what the targets share.
            builder.Emit(UbcOpcode.Squash, (uint)(under - anchor) | (1UL << 8));

            if (!Pop(under - anchor + 1) || !Push(1, at))
            {
                return false;
            }
        }

        if (!Pop(UbcOpcodes.Row(UbcOpcode.JumpTable).Pops.Length))
        {
            return false;
        }

        for (var index = 0; index < targets.Length; index++)
        {
            var target = targets[index];

            if (target.Kind is FrameKind.Function)
            {
                labels[index] = Return(stored);
                continue;
            }

            if (!Exit(target, NoStores, at, out labels[index]))
            {
                return false;
            }
        }

        return Table(labels);
    }

    /// <summary>
    /// A <c>br_table</c> whose rows reach both a frame entered at the very stack under the selector
    /// and something deeper: the near rows jump there at once, and every other row to one step of the
    /// site's own that drops to the slot the deeper rows share and dispatches again on the selector,
    /// kept in the selector local, through a second table the sites that share that slot and those
    /// rows share.
    /// </summary>
    /// <remarks>
    /// Without it every deeper row would need a trampoline of the site's own, because the stack it
    /// leaves the table with is the site's own; with it a site costs its two tables' rows and one step,
    /// whatever the number of its targets.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=2; Fingerprint=D7BD92
    // Broiler-Falsified-If: a selector value reaches another target through the two tables than through the vector, the second table's rows are reached with any stack but the shared slot's, or the drop reaches below a deeper row's entry
    // Broiler-Human:        PENDING
    private bool SplitTable(Frame[] targets, int[] stored, int under, int at)
    {
        var deepest = 0;

        foreach (var target in targets)
        {
            if (target.Kind is not FrameKind.Function && target.EntryHeight < under)
            {
                deepest = System.Math.Max(deepest, target.EntryHeight);
            }
        }

        var shared = System.Math.Max(under > 0 ? (under - 1) / SquashField * SquashField : 0, deepest);

        if (!Selector(at))
        {
            return false;
        }

        builder.Emit(UbcOpcode.LocalTee, (uint)selector);

        if (!Pop(UbcOpcodes.Row(UbcOpcode.JumpTable).Pops.Length))
        {
            return false;
        }

        // The second table's rows, from the shared slot: a near row is never taken there, so it names
        // the first row that is.
        height = shared;
        var second = new int[targets.Length];

        for (var index = 0; index < targets.Length; index++)
        {
            var target = targets[index];

            if (target.Kind is FrameKind.Function)
            {
                second[index] = Return(stored);
            }
            else if (target.EntryHeight == under)
            {
                second[index] = -1;
            }
            else if (!Exit(target, NoStores, at, out second[index]))
            {
                return false;
            }
        }

        var taken = System.Array.Find(second, label => label >= 0);

        for (var index = 0; index < second.Length; index++)
        {
            second[index] = second[index] < 0 ? taken : second[index];
        }

        var key = (Identity(shared), string.Join(',', second));

        if (!redispatches.TryGetValue(key, out var table))
        {
            if (tables.Count >= UbcFormat.MaxJumpTables)
            {
                return Refuse(WebAssemblyDiagnosticCode.TranslationJumpTablesAboveMaximum, at);
            }

            table = tables.Count;
            tables.Add(null);
            unitTables.Add((table, second));
            redispatches.Add(key, table);
        }

        if (tables.Count >= UbcFormat.MaxJumpTables)
        {
            return Refuse(WebAssemblyDiagnosticCode.TranslationJumpTablesAboveMaximum, at);
        }

        // The first table, from the stack under the selector.
        height = under;
        var step = NewLabel();
        trampolines.Add(new Trampoline(step, NoStores, new Route(under - shared, -1, -1), TrampolineKind.Redispatch, table));
        var first = new int[targets.Length];

        for (var index = 0; index < targets.Length; index++)
        {
            var target = targets[index];

            if (target.Kind is FrameKind.Function || target.EntryHeight < under)
            {
                first[index] = step;
            }
            else if (!Exit(target, NoStores, at, out first[index]))
            {
                return false;
            }
        }

        return Table(first);
    }

    /// <summary>The selector local, declared after the scratch locals the first time a body needs it; refused when a unit could not declare it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=A43D34
    // Broiler-Falsified-If: the selector is given an index a unit cannot declare or its sixteen-bit operand cannot hold
    // Broiler-Human:        PENDING
    private bool Selector(int at)
    {
        if (selector >= 0)
        {
            return true;
        }

        if (localBase + scratch.Count >= UbcFormat.MaxLocals)
        {
            return Refuse(WebAssemblyDiagnosticCode.TranslationLocalsAboveMaximum, at);
        }

        selector = localBase + scratch.Count;
        scratch.Add(UbcSlotType.I32);
        return true;
    }

    /// <summary>Emits the jump table of a <c>br_table</c> site over <paramref name="labels"/>; what follows is unreachable.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4365CA
    // Broiler-Human:        PENDING
    private bool Table(int[] labels)
    {
        var table = tables.Count;
        tables.Add(null);
        unitTables.Add((table, labels));
        builder.Emit(UbcOpcode.JumpTable, (ulong)table);
        reachable = false;
        return true;
    }

    /// <summary>
    /// Where a conditional branch from the current stack to <paramref name="target"/> jumps: a return
    /// trampoline for the function's own label, the label itself when nothing is to be dropped or
    /// stored, otherwise the trampoline that stores the carried values and descends.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=BCF375
    // Broiler-Falsified-If: a branch to the function's own label is anything but a return, or a branch that carries a value jumps anywhere its value is not stored first
    // Broiler-Human:        PENDING
    private bool BranchTarget(Frame target, int at, out int label)
    {
        if (target.Kind is FrameKind.Function)
        {
            label = Return(NoStores);
            return true;
        }

        label = -1;
        var stored = NoStores;

        if (target.LabelArity > 0)
        {
            if (!Scratch(target, at))
            {
                return false;
            }

            stored = target.Scratch!;
        }

        return Exit(target, stored, at, out label);
    }

    /// <summary>
    /// Where a branch to <paramref name="target"/> from the current stack jumps: the label itself when
    /// nothing is to be dropped or stored, otherwise the trampoline for the target, this stack's
    /// identity and whether it stores - the stores, then the first step of the descent.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=A80BE9
    // Broiler-Falsified-If: two sites with different stacks share a trampoline, one that stores and one that does not share one, or a site jumps straight to a label it reaches with slots to drop or values to store
    // Broiler-Human:        PENDING
    private bool Exit(Frame target, int[] stored, int at, out int label)
    {
        Reached(target);

        if (stored.Length == 0 && height == target.EntryHeight)
        {
            label = target.Label;
            return true;
        }

        var key = (target.Label, Identity(height), stored.Length > 0);

        if (exits.TryGetValue(key, out label))
        {
            return true;
        }

        if (!Descend(target, height - stored.Length, at, out var route))
        {
            return false;
        }

        label = NewLabel();
        exits.Add(key, label);
        trampolines.Add(new Trampoline(label, stored, route, TrampolineKind.Jump, -1));
        return true;
    }

    /// <summary>
    /// The return trampoline for the current stack: the values in <paramref name="loads"/> loaded back,
    /// then a return, made once per stack identity and whether it loads.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=060383
    // Broiler-Falsified-If: two stacks of different identities share a return trampoline, or one that loads and one that does not share one
    // Broiler-Human:        PENDING
    private int Return(int[] loads)
    {
        var key = (Identity(height), loads.Length > 0);

        if (!returns.TryGetValue(key, out var label))
        {
            label = NewLabel();
            returns.Add(key, label);
            trampolines.Add(new Trampoline(label, loads, default, TrampolineKind.Return, -1));
        }

        return label;
    }

    /// <summary>
    /// The first step from a stack of height <paramref name="from"/> - the current stack's slots up to
    /// there - towards <paramref name="target"/>'s label: the slots to drop, the selector to set and
    /// where to jump; refused when the selector is needed and a unit could not declare it.
    /// </summary>
    /// <remarks>
    /// Within one <c>squash</c> of the target's entry the step drops to it and jumps to the label.
    /// Further, it goes down towards the floor of the stack - the innermost open frame whose entry lies
    /// below it - no further than the nearest multiple of <see cref="SquashField"/> on the way, sets
    /// the selector to the target's code at that floor, and jumps to the chain that goes on from there,
    /// or to the floor's dispatch when the floor's entry is reached.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=9B1AD5
    // Broiler-Falsified-If: a step drops more slots than one squash states or below the target's entry, pushes the selector above a height the stack already reached, or leads anywhere but the target's label once its entry is reached
    // Broiler-Human:        PENDING
    private bool Descend(Frame target, int from, int at, out Route route)
    {
        var entry = target.EntryHeight;

        if (from - entry <= SquashField)
        {
            route = new Route(from - entry, -1, target.Label);
            return true;
        }

        route = default;

        // A drop longer than one squash lies above the target's entry, so some open frame's entry,
        // the target's at the latest, lies below the stack.
        if (FloorOf(from) is not { } floor)
        {
            return Unreadable();
        }

        floor.At = floor.Entries.Count == 0 ? at : floor.At;

        if (!Code(floor, target, at, out var code))
        {
            return false;
        }

        var anchor = System.Math.Max((from - 1) / SquashField * SquashField, floor.Frame.EntryHeight);
        route = new Route(from - anchor, code, anchor == floor.Frame.EntryHeight ? floor.Dispatch : Chain(floor, anchor));
        return true;
    }

    /// <summary>
    /// The trampoline that takes a stack of height <paramref name="from"/> down to
    /// <paramref name="floor"/>'s entry and on to its dispatch, made once per floor and slot identity,
    /// whatever label the branches that come this way are going to.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=3347C1
    // Broiler-Falsified-If: two stacks of different identities share a step, a step drops below the floor's entry, or a step is made again for a floor and an identity that already have one
    // Broiler-Human:        PENDING
    private int Chain(Floor floor, int from)
    {
        var key = (floor.Frame.Label, Identity(from));

        if (chains.TryGetValue(key, out var label))
        {
            return label;
        }

        var anchor = System.Math.Max(from - SquashField, floor.Frame.EntryHeight);
        var next = anchor == floor.Frame.EntryHeight ? floor.Dispatch : Chain(floor, anchor);
        label = NewLabel();
        chains.Add(key, label);
        trampolines.Add(new Trampoline(label, NoStores, new Route(from - anchor, -1, next), TrampolineKind.Jump, -1));
        return label;
    }

    /// <summary>
    /// The floor of a stack of height <paramref name="from"/>: the innermost open frame whose entry
    /// lies below it, with the dispatch its chains end in; null only when no open frame's entry lies
    /// below, which a drop never asks for.
    /// </summary>
    /// <remarks>
    /// A slot on the stack has every frame that was open beneath it when it was pushed still open,
    /// because a frame that ends drops what lies above its entry but its results; so a slot's chains
    /// lead to the frames it lies in, and only a result a frame's end kept moves, one slot, to the next.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D1C0ED
    // Broiler-Falsified-If: the floor answered is not the innermost open frame whose entry lies below the height
    // Broiler-Human:        PENDING
    private Floor? FloorOf(int from)
    {
        for (var index = frames.Count - 1; index >= 0; index--)
        {
            var frame = frames[index];

            if (frame.EntryHeight < from)
            {
                if (!floors.TryGetValue(frame.Label, out var floor))
                {
                    floor = new Floor(frame, NewLabel());
                    floors.Add(frame.Label, floor);
                    floorOrder.Add(floor);
                }

                return floor;
            }
        }

        return null;
    }

    /// <summary>
    /// <paramref name="target"/>'s code at <paramref name="floor"/>: the selector value its dispatch
    /// sends to the target's label, when the floor's entry is the target's, or to the step that sets the
    /// target's code at the next floor down and descends to it; allocated the first time a branch to the
    /// target passes the floor, with the selector local the first time a body needs it.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=3BC26E
    // Broiler-Falsified-If: two targets at one floor share a code, a code leads anywhere but towards its own target, a code's step arrives at the next floor with any stack but that floor's entry stack, or the selector is given an index a unit cannot declare
    // Broiler-Human:        PENDING
    private bool Code(Floor floor, Frame target, int at, out int code)
    {
        if (floor.Codes.TryGetValue(target.Label, out code))
        {
            return true;
        }

        if (!Selector(at))
        {
            return false;
        }

        var entry = target.Label;

        if (target.EntryHeight < floor.Frame.EntryHeight)
        {
            // The target lies further down: on from the floor's own entry, towards the next floor.
            if (!Descend(target, floor.Frame.EntryHeight, at, out var route))
            {
                return false;
            }

            entry = NewLabel();
            trampolines.Add(new Trampoline(entry, NoStores, route, TrampolineKind.Jump, -1));
        }

        code = floor.Entries.Count;
        floor.Codes.Add(target.Label, code);
        floor.Entries.Add(entry);
        return true;
    }

    /// <summary>
    /// Emits <paramref name="route"/>: the drop, then the selector when the route names a code - pushed
    /// on the stack the drop left, so never above a height the stack already reached - then the jump.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=6FEE6F
    // Broiler-Falsified-If: the selector is set before the drop, or a route with a code is emitted without it
    // Broiler-Human:        PENDING
    private void Follow(Route route)
    {
        if (route.Drop > 0)
        {
            builder.Emit(UbcOpcode.Squash, (uint)route.Drop);
        }

        if (route.Code >= 0)
        {
            builder.Emit(UbcOpcode.ConstI32, (uint)route.Code);
            builder.Emit(UbcOpcode.LocalSet, (uint)selector);
        }

        builder.EmitTo(UbcOpcode.Jump, route.Next);
    }

    /// <summary>
    /// The scratch locals that carry <paramref name="target"/>'s values across its edges, one per
    /// carried position and word type, allocated after the body's locals the first time a body needs
    /// one; refused when a unit could not declare it.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=1AE868
    // Broiler-Falsified-If: a scratch local is given an index a unit cannot declare or its sixteen-bit operand cannot hold, a local of another type than the carried value's, or one of the body's own locals
    // Broiler-Human:        PENDING
    private bool Scratch(Frame target, int at)
    {
        if (target.Scratch is not null)
        {
            return true;
        }

        var locals = new int[target.Results.Length];

        for (var position = 0; position < locals.Length; position++)
        {
            var type = target.Results[position];

            if (!scratchLocals.TryGetValue((position, type), out var local))
            {
                local = localBase + scratch.Count;

                if (local >= UbcFormat.MaxLocals)
                {
                    return Refuse(WebAssemblyDiagnosticCode.TranslationLocalsAboveMaximum, at);
                }

                scratch.Add(type);
                scratchLocals.Add((position, type), local);
            }

            locals[position] = local;
        }

        target.Scratch = locals;
        return true;
    }

    /// <summary>Stores the carried values into <paramref name="locals"/>, the top value first.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=CCACD4
    // Broiler-Human:        PENDING
    private void Store(int[] locals)
    {
        for (var position = locals.Length - 1; position >= 0; position--)
        {
            builder.Emit(UbcOpcode.LocalSet, (uint)locals[position]);
        }
    }

    /// <summary>Records that a branch reaches <paramref name="target"/>'s label, which makes a block's end reachable.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=99A902
    // Broiler-Human:        PENDING
    private static void Reached(Frame target) => target.Branched = true;

    /// <summary>A local access: the common row, which the walk types by the parameter or the declared local it names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=06BBA8
    // Broiler-Falsified-If: an index is written past its sixteen-bit field, or a tee changes the stack's identity where the walk keeps it
    // Broiler-Human:        PENDING
    private bool Local(ref VmBoundedReader reader, WasmOpcode opcode, int at)
    {
        if (!U32(ref reader, out var index))
        {
            return false;
        }

        if (index > ushort.MaxValue)
        {
            return Refuse(WebAssemblyDiagnosticCode.TranslationOperandOutOfRange, at);
        }

        switch (opcode)
        {
            case WasmOpcode.LocalGet:
                builder.Emit(UbcOpcode.LocalGet, index);
                return Push(1, at);

            case WasmOpcode.LocalSet:
                builder.Emit(UbcOpcode.LocalSet, index);
                return Pop(1);

            default:
                // The walk keeps the teed slot as it was.
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
    /// with a position when the row maps a trap, and applied to the stack as its effect says.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=B8E56F
    // Broiler-Falsified-If: a row that maps a trap is emitted without a position, or the stack moves otherwise than the row's effect
    // Broiler-Human:        PENDING
    private bool FamilyRowAt(byte opcode, ulong operand, int at)
    {
        var row = FamilyRow(opcode);

        if (!row.Traps.IsDefaultOrEmpty)
        {
            Position(at);
        }

        builder.EmitFamily(WasmFamilyTable.Slot, opcode, row.Shape, operand);
        return Pop(row.Effect.Pops.Length) && Push(row.Effect.Pushes.Length, at);
    }

    /// <summary>A common row with a listed effect, applied to the stack as the common table states it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F58AEF
    // Broiler-Human:        PENDING
    private bool Listed(UbcOpcode opcode, int at)
    {
        var row = UbcOpcodes.Row(opcode);
        return Pop(row.Pops.Length) && Push(row.Pushes.Length, at);
    }

    /// <summary>The family table's row of <paramref name="opcode"/>; the table defines every byte this lowering emits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=31A57C
    // Broiler-Human:        PENDING
    private static UbcInstructionRow FamilyRow(byte opcode) =>
        WasmFamilyTable.Table.TryGetRow(opcode, out var row)
            ? row
            : throw new System.InvalidOperationException($"the WebAssembly family table has no row 0x{opcode:X2}");

    // ---- the stack -----------------------------------------------------------------------------

    /// <summary>
    /// Pushes <paramref name="count"/> new slots, each with an identity no other slot of the body has,
    /// keeping the greatest height, and refuses a height a unit cannot declare.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=647489
    // Broiler-Falsified-If: a height above the format's greatest is admitted, the unit's declared height is lower than a height its code reaches, or a pushed slot shares an identity with another
    // Broiler-Human:        PENDING
    private bool Push(int count, int at)
    {
        for (var index = 0; index < count; index++)
        {
            if (height >= UbcFormat.MaxHeight)
            {
                return Refuse(WebAssemblyDiagnosticCode.TranslationOperandHeightAboveMaximum, at);
            }

            lastSlot++;

            if (height < slots.Count)
            {
                slots[height] = lastSlot;
            }
            else
            {
                slots.Add(lastSlot);
            }

            height++;
        }

        maxHeight = System.Math.Max(maxHeight, height);
        return true;
    }

    /// <summary>Pops <paramref name="count"/> slots; the slots beneath keep their identities.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7F5025
    // Broiler-Human:        PENDING
    private bool Pop(int count)
    {
        if (count > height)
        {
            // The validator admitted no body that pops below its frame, so this is this assembly
            // disagreeing with itself.
            return Unreadable();
        }

        height -= count;
        return true;
    }

    /// <summary>The identity of the stack of height <paramref name="at"/>: its top slot's, or zero for the empty stack.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=54E7C0
    // Broiler-Human:        PENDING
    private long Identity(int at) => at == 0 ? 0 : slots[at - 1];

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

    /// <summary>
    /// The body's declared locals, parameters excluded as the decoder already excluded them, followed by
    /// its scratch locals, as runs of one type.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A86937
    // Broiler-Falsified-If: the runs expand to other types, in another order or in another number than the body's locals followed by its scratch locals
    // Broiler-Human:        PENDING
    private static ImmutableArray<UbcLocalRun> Runs(System.ReadOnlySpan<WasmValueType> locals, List<UbcSlotType> scratchTypes)
    {
        var runs = ImmutableArray.CreateBuilder<UbcLocalRun>();
        var count = 0u;
        var type = default(UbcSlotType);

        for (var at = 0; at < locals.Length + scratchTypes.Count; at++)
        {
            var next = at < locals.Length ? SlotOf(locals[at]) : scratchTypes[at - locals.Length];

            if (count > 0 && next != type)
            {
                runs.Add(new UbcLocalRun(count, type));
                count = 0;
            }

            type = next;
            count++;
        }

        if (count > 0)
        {
            runs.Add(new UbcLocalRun(count, type));
        }

        return runs.ToImmutable();
    }

    /// <summary>One frame of the control stack.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A5235D
    // Broiler-Human:        PENDING
    private sealed class Frame(
        FrameKind kind, int label, int elseLabel, int entryHeight, ImmutableArray<UbcSlotType> results, int elseAt, int endAt)
    {
        /// <summary>What the frame stands for; a conditional's becomes an alternative at its <c>else</c>.</summary>
        internal FrameKind Kind { get; set; } = kind;

        /// <summary>The label a branch to the frame jumps to: a loop's start, anything else's end; none for the function.</summary>
        internal int Label { get; } = label;

        /// <summary>A conditional's alternative, or its end when it has none: where its test jumps.</summary>
        internal int ElseLabel { get; } = elseLabel;

        /// <summary>The stack's height at the frame's entry, a conditional's test already taken; nothing inside pops below it.</summary>
        internal int EntryHeight { get; } = entryHeight;

        /// <summary>The values the frame leaves when it ends.</summary>
        internal ImmutableArray<UbcSlotType> Results { get; } = results;

        /// <summary>The offset of a conditional's <c>else</c> in the body, or minus one.</summary>
        internal int ElseAt { get; } = elseAt;

        /// <summary>The offset of the frame's <c>end</c> in the body.</summary>
        internal int EndAt { get; } = endAt;

        /// <summary>Whether a branch reached the frame's label, or a consequent jumped to its end.</summary>
        internal bool Branched { get; set; }

        /// <summary>The scratch locals its carried values cross its edges in, once an edge has carried one; otherwise null.</summary>
        internal int[]? Scratch { get; set; }

        /// <summary>How many values a branch to the frame carries: none to a loop, its results to anything else.</summary>
        // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=0; Fingerprint=45F18E
        // Broiler-Falsified-If: a branch to a loop is said to carry a value, or a branch to a block fewer than its results
        // Broiler-Human:        PENDING
        internal int LabelArity => Kind is FrameKind.Loop ? 0 : Results.Length;
    }

    /// <summary>What a trampoline does once it has stored or loaded its values.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1BA752
    // Broiler-Human:        PENDING
    private enum TrampolineKind : byte
    {
        Jump,
        Return,
        Redispatch,
    }

    /// <summary>
    /// A trampoline: its label, then the values it stores and the route it follows; or the values it
    /// loads, in <see cref="Stores"/>, and a return; or the drop in its route and a second dispatch on
    /// the selector through <see cref="Table"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0819F2
    // Broiler-Human:        PENDING
    private sealed record Trampoline(int Label, int[] Stores, Route Route, TrampolineKind Kind, int Table);

    /// <summary>One step of a drop: the slots to drop, the selector's value to set or minus one, and the label to jump to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=5453BF
    // Broiler-Human:        PENDING
    private readonly record struct Route(int Drop, int Code, int Next);

    /// <summary>
    /// A frame as the floor of the chains that end at its entry: the dispatch they end in, and the
    /// targets it tells apart, each code's entry in code order.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=12560A
    // Broiler-Human:        PENDING
    private sealed class Floor(Frame frame, int dispatch)
    {
        /// <summary>The frame whose entry the chains end at.</summary>
        internal Frame Frame { get; } = frame;

        /// <summary>The label of the dispatch the chains jump to.</summary>
        internal int Dispatch { get; } = dispatch;

        /// <summary>Each target's code, by the target's label.</summary>
        internal Dictionary<int, int> Codes { get; } = new();

        /// <summary>Where each code leads, in code order.</summary>
        internal List<int> Entries { get; } = new();

        /// <summary>The body offset a refusal of the floor's jump table is placed at: the branch that first needed the floor.</summary>
        internal int At { get; set; }
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
