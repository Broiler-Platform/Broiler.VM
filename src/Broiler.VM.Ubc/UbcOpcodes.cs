// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           49
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    High
// Criteria:         3/2
// Resource impact:  0/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Immutable;

namespace Broiler.VM.Ubc;

/// <summary>The common family: the instructions every emitter executes itself, with one meaning.</summary>
/// <remarks>
/// An instruction is common only if its meaning is fully stated by the universal bytecode without
/// reference to any language's specification, and if it would mean the same thing in a language nobody
/// has planned. Unprefixed opcode bytes <c>0x00</c> to <c>0xEF</c> belong to this family; the members
/// below are the ones format version 1 defines, and every other byte in that range is refused. The byte
/// values are Appendix A of <c>docs/universal-bytecode.md</c>.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=6963BB
// Broiler-Human:        PENDING
public enum UbcOpcode : byte
{
    /// <summary>Nothing. Admitted so a patched stream has a legal filler.</summary>
    Nop = 0x00,

    /// <summary>Abort: family slot and code name the fault. Slot zero code zero is the universal unreachable.</summary>
    Trap = 0x01,

    /// <summary>Continue at an absolute code offset inside the unit.</summary>
    Jump = 0x02,

    /// <summary>Pop an <c>i32</c>; jump when it is zero.</summary>
    JumpIfZero = 0x03,

    /// <summary>Pop an <c>i32</c>; jump when it is not zero.</summary>
    JumpIfNonZero = 0x04,

    /// <summary>Pop an <c>i32</c>; jump to that row of a jump table, or to its last row when out of range.</summary>
    JumpTable = 0x05,

    /// <summary>Return the unit's declared results from the top of the stack, discarding the rest.</summary>
    Return = 0x06,

    /// <summary>Call a unit of the same artifact with the signature its Units row declares.</summary>
    Call = 0x07,

    /// <summary>Discard the top slot.</summary>
    Drop = 0x08,

    /// <summary>Copy the top slot.</summary>
    Dup = 0x09,

    /// <summary>Copy the top two slots, in order.</summary>
    Dup2 = 0x0A,

    /// <summary>Exchange the top two slots.</summary>
    Swap = 0x0B,

    /// <summary>Copy the slot the operand's count of places below the top.</summary>
    Pick = 0x0C,

    /// <summary>Pop an <c>i32</c> and two slots of one type; keep the deeper when the word is not zero.</summary>
    Select = 0x0D,

    /// <summary>Keep the top <c>k</c> slots and discard the <c>n</c> beneath them.</summary>
    Squash = 0x0E,

    /// <summary>Push a local, typed by the unit's local table.</summary>
    LocalGet = 0x10,

    /// <summary>Pop into a local.</summary>
    LocalSet = 0x11,

    /// <summary>Store the top into a local and keep it.</summary>
    LocalTee = 0x12,

    /// <summary>Push a thirty-two-bit word constant.</summary>
    ConstI32 = 0x20,

    /// <summary>Push a sixty-four-bit word constant.</summary>
    ConstI64 = 0x21,

    /// <summary>Push a binary32 constant, its bits as written.</summary>
    ConstF32 = 0x22,

    /// <summary>Push a binary64 constant, its bits as written.</summary>
    ConstF64 = 0x23,
}

/// <summary>How a common row changes the stack. The walk and the interpreter dispatch on this, not on the opcode.</summary>
/// <remarks>
/// <see cref="Listed"/> rows state their typed pops and pushes in the row itself. Every other member is
/// an effect that depends on the stack's own types, the operand, the unit's local table or another
/// unit's signature, and its rule is stated once, here, beside the member.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B52EF2
// Broiler-Human:        PENDING
public enum UbcCommonEffect : byte
{
    /// <summary>The row's <see cref="UbcCommonRow.Pops"/> and <see cref="UbcCommonRow.Pushes"/>.</summary>
    Listed = 0,

    /// <summary>No successor. The operand's first field is a family slot, its second a code that slot's trap vocabulary defines.</summary>
    Trap = 1,

    /// <summary>No change to the stack; the only successor is the target.</summary>
    Jump = 2,

    /// <summary>Pop an <c>i32</c>; every row of the named table, all in this unit, is a successor.</summary>
    JumpTable = 3,

    /// <summary>The unit's results, typed and in order, on top of the stack; no successor.</summary>
    Return = 4,

    /// <summary>The callee's parameters on top, popped; its results pushed.</summary>
    Call = 5,

    /// <summary><c>[t] -&gt; []</c>.</summary>
    Drop = 6,

    /// <summary><c>[t] -&gt; [t t]</c>.</summary>
    Dup = 7,

    /// <summary><c>[t u] -&gt; [t u t u]</c>.</summary>
    Dup2 = 8,

    /// <summary><c>[t u] -&gt; [u t]</c>.</summary>
    Swap = 9,

    /// <summary><c>[t ...] -&gt; [t ... t]</c>, the copied slot the operand's count below the top.</summary>
    Pick = 10,

    /// <summary><c>[t t i32] -&gt; [t]</c>, both candidates of one type.</summary>
    Select = 11,

    /// <summary><c>[... n k] -&gt; [k]</c>: the operand's first field is <c>n</c>, its second <c>k</c>.</summary>
    Squash = 12,

    /// <summary><c>[] -&gt; [t]</c>, <c>t</c> the local's type.</summary>
    LocalGet = 13,

    /// <summary><c>[t] -&gt; []</c>, <c>t</c> the local's type.</summary>
    LocalSet = 14,

    /// <summary><c>[t] -&gt; [t]</c>, <c>t</c> the local's type.</summary>
    LocalTee = 15,
}

/// <summary>One row of the common family's table.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=68D540
// Broiler-Human:        PENDING
public sealed class UbcCommonRow
{
    internal UbcCommonRow(
        UbcOpcode opcode,
        string mnemonic,
        UbcOperandShape shape,
        UbcCommonEffect effect,
        ImmutableArray<UbcSlotType> pops,
        ImmutableArray<UbcSlotType> pushes,
        bool isTerminal,
        bool hasCodeTarget)
    {
        Opcode = opcode;
        Mnemonic = mnemonic;
        Shape = shape;
        Effect = effect;
        Pops = pops;
        Pushes = pushes;
        IsTerminal = isTerminal;
        HasCodeTarget = hasCodeTarget;
    }

    /// <summary>The opcode byte.</summary>
    public UbcOpcode Opcode { get; }

    /// <summary>The mnemonic a disassembly writes.</summary>
    public string Mnemonic { get; }

    /// <summary>The operand shape.</summary>
    public UbcOperandShape Shape { get; }

    /// <summary>How the row changes the stack.</summary>
    public UbcCommonEffect Effect { get; }

    /// <summary>For a <see cref="UbcCommonEffect.Listed"/> row, its pops, bottom to top; otherwise empty.</summary>
    public ImmutableArray<UbcSlotType> Pops { get; }

    /// <summary>For a <see cref="UbcCommonEffect.Listed"/> row, its pushes, bottom to top; otherwise empty.</summary>
    public ImmutableArray<UbcSlotType> Pushes { get; }

    /// <summary>
    /// True when execution never falls through to the next instruction: <c>trap</c>, <c>jump</c>,
    /// <c>jump_table</c> and <c>return</c>.
    /// </summary>
    public bool IsTerminal { get; }

    /// <summary>True when the operand is an absolute code offset inside the unit.</summary>
    public bool HasCodeTarget { get; }

    /// <summary>The fuel every common row charges before its effect: one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=70E86F
    // Broiler-Human:        PENDING
    public uint Cost => 1;

    /// <summary>The width of the whole instruction: the opcode byte and the operand.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7E6862
    // Broiler-Human:        PENDING
    public int InstructionWidth => 1 + UbcOperandShapes.Width(Shape);
}

/// <summary>
/// The common family's one table: the only place the walk, the interpreter and every encoder read a
/// common row's shape, width, effect and target (rule U4).
/// </summary>
/// <remarks>
/// Adding a row is a universal bytecode format version. A <c>v</c>-typed shuffle, local access or
/// squash is a plane operation an emitter performs through the family's plane helper; the same rows
/// over words are register or slot moves. Nothing here says which, because nothing here knows a
/// family: the walk resolves each instruction's planes from the types it proves.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=944BD6
// Broiler-Falsified-If: a common row's width or effect is stated anywhere in this assembly outside this table
// Broiler-Human:        PENDING
public static class UbcOpcodes
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=740684
    // Broiler-Human:        PENDING
    private static readonly UbcCommonRow?[] Table = Build();

    /// <summary>Every row, in ascending opcode order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E69506
    // Broiler-Human:        PENDING
    public static ImmutableArray<UbcCommonRow> All { get; } = CollectAll();

    /// <summary>True when <paramref name="opcode"/> is an unprefixed byte format version 1 defines.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=27D489
    // Broiler-Human:        PENDING
    public static bool IsDefined(byte opcode) => Table[opcode] is not null;

    /// <summary>The row of <paramref name="opcode"/>, or false when the byte is not a common instruction.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=3E1C8B
    // Broiler-Falsified-If: a byte in the prefix range or an undefined byte answers true
    // Broiler-Human:        PENDING
    public static bool TryDescribe(byte opcode, out UbcCommonRow row)
    {
        var found = Table[opcode];
        row = found!;
        return found is not null;
    }

    /// <summary>The row of a defined opcode.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=FEED30
    // Broiler-Human:        PENDING
    public static UbcCommonRow Row(UbcOpcode opcode) =>
        Table[(byte)opcode] ?? throw new System.ArgumentOutOfRangeException(nameof(opcode), opcode, "not a common opcode");

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4C0386
    // Broiler-Falsified-If: a row here disagrees with Appendix A of the concept in its byte, shape, effect or target
    // Broiler-Human:        PENDING
    private static UbcCommonRow?[] Build()
    {
        var none = ImmutableArray<UbcSlotType>.Empty;
        var i32 = ImmutableArray.Create(UbcSlotType.I32);
        var table = new UbcCommonRow?[256];

        void Add(UbcOpcode opcode, string mnemonic, UbcOperandShape shape, UbcCommonEffect effect,
            ImmutableArray<UbcSlotType> pops, ImmutableArray<UbcSlotType> pushes, bool terminal, bool target) =>
            table[(byte)opcode] = new UbcCommonRow(opcode, mnemonic, shape, effect, pops, pushes, terminal, target);

        Add(UbcOpcode.Nop, "nop", UbcOperandShape.None, UbcCommonEffect.Listed, none, none, false, false);
        Add(UbcOpcode.Trap, "trap", UbcOperandShape.U8U16, UbcCommonEffect.Trap, none, none, true, false);
        Add(UbcOpcode.Jump, "jump", UbcOperandShape.U32, UbcCommonEffect.Jump, none, none, true, true);
        Add(UbcOpcode.JumpIfZero, "jump_if_zero", UbcOperandShape.U32, UbcCommonEffect.Listed, i32, none, false, true);
        Add(UbcOpcode.JumpIfNonZero, "jump_if_nonzero", UbcOperandShape.U32, UbcCommonEffect.Listed, i32, none, false, true);
        Add(UbcOpcode.JumpTable, "jump_table", UbcOperandShape.U16, UbcCommonEffect.JumpTable, i32, none, true, false);
        Add(UbcOpcode.Return, "return", UbcOperandShape.None, UbcCommonEffect.Return, none, none, true, false);
        Add(UbcOpcode.Call, "call", UbcOperandShape.U32, UbcCommonEffect.Call, none, none, false, false);
        Add(UbcOpcode.Drop, "drop", UbcOperandShape.None, UbcCommonEffect.Drop, none, none, false, false);
        Add(UbcOpcode.Dup, "dup", UbcOperandShape.None, UbcCommonEffect.Dup, none, none, false, false);
        Add(UbcOpcode.Dup2, "dup2", UbcOperandShape.None, UbcCommonEffect.Dup2, none, none, false, false);
        Add(UbcOpcode.Swap, "swap", UbcOperandShape.None, UbcCommonEffect.Swap, none, none, false, false);
        Add(UbcOpcode.Pick, "pick", UbcOperandShape.U8, UbcCommonEffect.Pick, none, none, false, false);
        Add(UbcOpcode.Select, "select", UbcOperandShape.None, UbcCommonEffect.Select, none, none, false, false);
        Add(UbcOpcode.Squash, "squash", UbcOperandShape.U8U8, UbcCommonEffect.Squash, none, none, false, false);
        Add(UbcOpcode.LocalGet, "local.get", UbcOperandShape.U16, UbcCommonEffect.LocalGet, none, none, false, false);
        Add(UbcOpcode.LocalSet, "local.set", UbcOperandShape.U16, UbcCommonEffect.LocalSet, none, none, false, false);
        Add(UbcOpcode.LocalTee, "local.tee", UbcOperandShape.U16, UbcCommonEffect.LocalTee, none, none, false, false);
        Add(UbcOpcode.ConstI32, "const.i32", UbcOperandShape.I32, UbcCommonEffect.Listed, none, i32, false, false);
        Add(UbcOpcode.ConstI64, "const.i64", UbcOperandShape.I64, UbcCommonEffect.Listed, none, ImmutableArray.Create(UbcSlotType.I64), false, false);
        Add(UbcOpcode.ConstF32, "const.f32", UbcOperandShape.F32, UbcCommonEffect.Listed, none, ImmutableArray.Create(UbcSlotType.F32), false, false);
        Add(UbcOpcode.ConstF64, "const.f64", UbcOperandShape.F64, UbcCommonEffect.Listed, none, ImmutableArray.Create(UbcSlotType.F64), false, false);

        return table;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1BC9DC
    // Broiler-Human:        PENDING
    private static ImmutableArray<UbcCommonRow> CollectAll()
    {
        var builder = ImmutableArray.CreateBuilder<UbcCommonRow>();

        foreach (var row in Table)
        {
            if (row is not null)
            {
                builder.Add(row);
            }
        }

        return builder.ToImmutable();
    }
}
