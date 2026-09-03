// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           30
// Human-reviewed:   0/10
// IP risk:          None
// Security risk:    High
// Criteria:         5/5
// Resource impact:  1/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The instruction set of format version 1, at the <c>broiler.javascript.slice</c> surface.
/// </summary>
/// <remarks>
/// <para>
/// Every instruction is one opcode byte followed by a fixed operand width, so an instruction
/// boundary is computable from the opcode alone and a jump target can be checked against the
/// boundary set without executing anything. A variable-width operand in the code section would
/// make boundary checking a decode, and a decode that must run before validation is a decode that
/// runs on unvalidated bytes.
/// </para>
/// <para>
/// The set is the smallest one that is still JavaScript rather than arithmetic. It carries
/// Boolean as a distinct value kind because <c>1 &lt; 2</c> is <c>true</c> and not <c>1</c>; it
/// carries <see cref="Rem"/> because JavaScript's remainder takes the sign of the dividend and is
/// not <c>fmod</c>'s neighbour by accident; and it carries the five bitwise operators because
/// they are defined through <c>ToInt32</c> and <c>ToUint32</c>, which is where a naive
/// implementation over doubles stops agreeing with the language.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=39CADD
// Broiler-Human:        PENDING
public enum JavaScriptOpcode : byte
{
    /// <summary>Push constant pool entry <c>u16</c>.</summary>
    LoadConstant = 0x10,

    /// <summary>Push local slot <c>u16</c>.</summary>
    LoadLocal = 0x11,

    /// <summary>Pop one value into local slot <c>u16</c>.</summary>
    StoreLocal = 0x12,

    /// <summary>Pop two, push their sum, under the numeric addition of the language.</summary>
    Add = 0x20,

    /// <summary>Pop two, push their difference.</summary>
    Subtract = 0x21,

    /// <summary>Pop two, push their product.</summary>
    Multiply = 0x22,

    /// <summary>Pop two, push their quotient. Division by zero is <c>Infinity</c>, not a fault.</summary>
    Divide = 0x23,

    /// <summary>Pop two, push the remainder, whose sign follows the dividend.</summary>
    Rem = 0x24,

    /// <summary>Pop one, push its numeric negation.</summary>
    Negate = 0x25,

    /// <summary>Pop one, push <c>ToNumber</c> of it. Unary plus, which is not a no-op.</summary>
    ToNumber = 0x26,

    /// <summary>Pop one, push the logical negation of <c>ToBoolean</c> of it.</summary>
    Not = 0x27,

    /// <summary>Pop two, push whether the first is less than the second.</summary>
    LessThan = 0x30,

    /// <summary>Pop two, push whether the first is less than or equal to the second.</summary>
    LessThanOrEqual = 0x31,

    /// <summary>Pop two, push whether the first is greater than the second.</summary>
    GreaterThan = 0x32,

    /// <summary>Pop two, push whether the first is greater than or equal to the second.</summary>
    GreaterThanOrEqual = 0x33,

    /// <summary>Pop two, push strict equality.</summary>
    StrictEquals = 0x34,

    /// <summary>Pop two, push strict inequality.</summary>
    StrictNotEquals = 0x35,

    /// <summary>Pop two, push the bitwise OR of their <c>ToInt32</c>.</summary>
    BitwiseOr = 0x40,

    /// <summary>Pop two, push the bitwise AND of their <c>ToInt32</c>.</summary>
    BitwiseAnd = 0x41,

    /// <summary>Pop two, push the bitwise XOR of their <c>ToInt32</c>.</summary>
    BitwiseXor = 0x42,

    /// <summary>Pop two, push the left shift, masking the shift count to five bits.</summary>
    ShiftLeft = 0x43,

    /// <summary>Pop two, push the sign-propagating right shift.</summary>
    ShiftRight = 0x44,

    /// <summary>Pop two, push the zero-filling right shift, over <c>ToUint32</c>.</summary>
    ShiftRightUnsigned = 0x45,

    /// <summary>Jump by the signed 32-bit displacement that follows.</summary>
    Jump = 0x50,

    /// <summary>Pop one; jump by the displacement when <c>ToBoolean</c> of it is false.</summary>
    JumpIfFalse = 0x51,

    /// <summary>Pop one; jump by the displacement when <c>ToBoolean</c> of it is true.</summary>
    JumpIfTrue = 0x52,

    /// <summary>Pop one and discard it.</summary>
    Pop = 0x60,

    /// <summary>Push a second copy of the top of the stack.</summary>
    Duplicate = 0x61,

    /// <summary>Pop one and finish the entry with it as the completion value.</summary>
    Return = 0x70,

    /// <summary>
    /// Push nothing and fault: the binding this would have read is not yet initialised.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The temporal dead zone, and it needs an opcode because nothing else in this format can
    /// raise a language error.</b> `let x = x + 1;` and `x; let x;` must throw a runtime
    /// `ReferenceError`; before this opcode the profile answered `undefined`, because reading a
    /// slot that had not been written yet was indistinguishable from reading one holding
    /// `undefined`. Division by zero is `Infinity` here and every other instruction is total, so
    /// there was no instruction that could fail at all.
    /// </para>
    /// <para>
    /// <b>It declares a push of one although it never pushes, and that is deliberate.</b> The
    /// verifier's model is an operand-stack HEIGHT, and this instruction stands exactly where a
    /// `LoadLocal` would have stood - so declaring the height it replaces keeps every join, every
    /// bound and every reachability answer the same as the program that has no dead zone in it. At
    /// run time the frame is abandoned before the push happens, so the declared height describes a
    /// state no execution observes.
    /// </para>
    /// <para>
    /// <b>It carries no operand, so it cannot name the binding.</b> A message reading "cannot
    /// access `x` before initialisation" would need an interned name, and the constant pool's
    /// interned-name tag is reserved from version 1 and admitted by no manifest yet. The error KIND
    /// is what a conformance test matches on, and the position table is what names the line.
    /// </para>
    /// </remarks>
    ThrowUninitializedBinding = 0x71,
}

/// <summary>
/// What the verifier and the encoder both have to agree about an opcode: how wide it is and what
/// it does to the operand stack.
/// </summary>
/// <remarks>
/// This table is the one place the two halves of the format meet. Putting it in the format
/// assembly rather than in either consumer is what makes the format the pivot: the executor and
/// the lowering must agree on the bytecode and neither may depend on the other.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=7F5FC6
// Broiler-Falsified-If: an opcode's declared width differs from what the encoder writes or the executor reads, or a declared stack effect differs from what the executor performs
// Broiler-Human:        PENDING
public static class JavaScriptOpcodes
{
    /// <summary>Every opcode this format version defines, in ascending numeric order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=F45BF4
    // Broiler-Human:        PENDING
    public static readonly JavaScriptOpcode[] All =
    [
        JavaScriptOpcode.LoadConstant, JavaScriptOpcode.LoadLocal, JavaScriptOpcode.StoreLocal,
        JavaScriptOpcode.Add, JavaScriptOpcode.Subtract, JavaScriptOpcode.Multiply,
        JavaScriptOpcode.Divide, JavaScriptOpcode.Rem, JavaScriptOpcode.Negate,
        JavaScriptOpcode.ToNumber, JavaScriptOpcode.Not,
        JavaScriptOpcode.LessThan, JavaScriptOpcode.LessThanOrEqual,
        JavaScriptOpcode.GreaterThan, JavaScriptOpcode.GreaterThanOrEqual,
        JavaScriptOpcode.StrictEquals, JavaScriptOpcode.StrictNotEquals,
        JavaScriptOpcode.BitwiseOr, JavaScriptOpcode.BitwiseAnd, JavaScriptOpcode.BitwiseXor,
        JavaScriptOpcode.ShiftLeft, JavaScriptOpcode.ShiftRight, JavaScriptOpcode.ShiftRightUnsigned,
        JavaScriptOpcode.Jump, JavaScriptOpcode.JumpIfFalse, JavaScriptOpcode.JumpIfTrue,
        JavaScriptOpcode.Pop, JavaScriptOpcode.Duplicate, JavaScriptOpcode.Return,
    ];

    /// <summary>Whether <paramref name="value"/> is an opcode this format version defines.</summary>
    /// <remarks>
    /// A switch rather than a range test or a lookup into a sparse table, because the opcode
    /// numbers are grouped by family with gaps between the families and a range test would admit
    /// every gap.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=B722F8
    // Broiler-Falsified-If: a byte this returns true for has no arm in the verifier's stack-effect switch or in the executor's dispatch
    // Broiler-Human:        PENDING
    public static bool IsDefined(byte value) => value switch
    {
        (byte)JavaScriptOpcode.LoadConstant or (byte)JavaScriptOpcode.LoadLocal or
        (byte)JavaScriptOpcode.StoreLocal or (byte)JavaScriptOpcode.Add or
        (byte)JavaScriptOpcode.Subtract or (byte)JavaScriptOpcode.Multiply or
        (byte)JavaScriptOpcode.Divide or (byte)JavaScriptOpcode.Rem or
        (byte)JavaScriptOpcode.Negate or (byte)JavaScriptOpcode.ToNumber or
        (byte)JavaScriptOpcode.Not or (byte)JavaScriptOpcode.LessThan or
        (byte)JavaScriptOpcode.LessThanOrEqual or (byte)JavaScriptOpcode.GreaterThan or
        (byte)JavaScriptOpcode.GreaterThanOrEqual or (byte)JavaScriptOpcode.StrictEquals or
        (byte)JavaScriptOpcode.StrictNotEquals or (byte)JavaScriptOpcode.BitwiseOr or
        (byte)JavaScriptOpcode.BitwiseAnd or (byte)JavaScriptOpcode.BitwiseXor or
        (byte)JavaScriptOpcode.ShiftLeft or (byte)JavaScriptOpcode.ShiftRight or
        (byte)JavaScriptOpcode.ShiftRightUnsigned or (byte)JavaScriptOpcode.Jump or
        (byte)JavaScriptOpcode.JumpIfFalse or (byte)JavaScriptOpcode.JumpIfTrue or
        (byte)JavaScriptOpcode.Pop or (byte)JavaScriptOpcode.Duplicate or
        (byte)JavaScriptOpcode.Return or
        (byte)JavaScriptOpcode.ThrowUninitializedBinding => true,
        _ => false,
    };

    /// <summary>How many operand bytes follow <paramref name="opcode"/>.</summary>
    /// <remarks>
    /// Two widths and no third: a <c>u16</c> index for the three slot-addressed instructions, a
    /// signed <c>i32</c> displacement for the three jumps, and nothing for everything else.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=5F48AB
    // Broiler-Falsified-If: the width returned here differs from the bytes the encoder emits for the same opcode
    // Broiler-Human:        PENDING
    public static int OperandWidth(JavaScriptOpcode opcode) => opcode switch
    {
        JavaScriptOpcode.LoadConstant or JavaScriptOpcode.LoadLocal or JavaScriptOpcode.StoreLocal => 2,
        JavaScriptOpcode.Jump or JavaScriptOpcode.JumpIfFalse or JavaScriptOpcode.JumpIfTrue => 4,
        _ => 0,
    };

    /// <summary>The whole width of one instruction, opcode byte included.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=3ECE8E
    // Broiler-Human:        PENDING
    public static int InstructionWidth(JavaScriptOpcode opcode) => 1 + OperandWidth(opcode);

    /// <summary>Whether <paramref name="opcode"/> carries a signed relative displacement.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=27F016
    // Broiler-Human:        PENDING
    public static bool IsJump(JavaScriptOpcode opcode) =>
        opcode is JavaScriptOpcode.Jump or JavaScriptOpcode.JumpIfFalse or JavaScriptOpcode.JumpIfTrue;

    /// <summary>Whether control can fall through <paramref name="opcode"/> to the next instruction.</summary>
    /// <remarks>
    /// Only two do not: an unconditional jump, which always transfers, and a return, which always
    /// finishes. Everything else, the conditional jumps included, may reach the following
    /// instruction, and the verifier's stack-height walk depends on this answer being exact.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=311EF6
    // Broiler-Human:        PENDING
    public static bool FallsThrough(JavaScriptOpcode opcode) =>
        opcode is not (JavaScriptOpcode.Jump or JavaScriptOpcode.Return);

    /// <summary>How many values <paramref name="opcode"/> pops.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=DA56F1
    // Broiler-Falsified-If: an opcode pops a different number of values in the executor than this reports
    // Broiler-Human:        PENDING
    public static int PopCount(JavaScriptOpcode opcode) => opcode switch
    {
        JavaScriptOpcode.LoadConstant or JavaScriptOpcode.LoadLocal or
        JavaScriptOpcode.ThrowUninitializedBinding => 0,

        JavaScriptOpcode.StoreLocal or JavaScriptOpcode.Negate or JavaScriptOpcode.ToNumber or
        JavaScriptOpcode.Not or JavaScriptOpcode.JumpIfFalse or JavaScriptOpcode.JumpIfTrue or
        JavaScriptOpcode.Pop or JavaScriptOpcode.Return => 1,

        JavaScriptOpcode.Duplicate => 1,

        JavaScriptOpcode.Jump => 0,

        _ => 2,
    };

    /// <summary>How many values <paramref name="opcode"/> pushes.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=E277A4
    // Broiler-Falsified-If: an opcode pushes a different number of values in the executor than this reports
    // Broiler-Human:        PENDING
    public static int PushCount(JavaScriptOpcode opcode) => opcode switch
    {
        JavaScriptOpcode.StoreLocal or JavaScriptOpcode.Pop or JavaScriptOpcode.Return or
        JavaScriptOpcode.Jump or JavaScriptOpcode.JumpIfFalse or JavaScriptOpcode.JumpIfTrue => 0,

        JavaScriptOpcode.Duplicate => 2,

        _ => 1,
    };
}
