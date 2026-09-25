// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   33
// Annotated:        33/33
// Exempt:           156
// Human-reviewed:   0/33
// IP risk:          Low
// Security risk:    Critical
// Criteria:         17/10
// Resource impact:  1/10 max
// Unverified:       33
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Immutable;

namespace Broiler.VM.Ubc;

/// <summary>
/// The closed set of machine operations an emitter may implement itself for a family row the family
/// classifies as that operation (Appendix D of <c>docs/universal-bytecode.md</c>).
/// </summary>
/// <remarks>
/// <para>
/// Every entry is a total function over word slots with a stated result for every input, or a partial
/// one whose every undefined input is a named trap. Names are the operation's, not any language's. A
/// family row names an entry, and obligation E2 holds the family's own handler to it bit for bit over a
/// retained input corpus: the handler is the meaning, and the primitive is an implementation checked
/// against it. Adding an entry is a universal bytecode format version.
/// </para>
/// <para>
/// Deliberately absent: floating-point remainder, exponentiation and transcendental functions, any
/// modular conversion a language defines, strings, objects, allocation and calls. A family that needs
/// one of those has a dynamic row, which is the correct answer rather than a gap.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=887882
// Broiler-Human:        PENDING
public enum UbcPrimitive : ushort
{
    /// <summary>Two's-complement addition, thirty-two bits.</summary>
    I32Add = 1,
    /// <summary>Two's-complement subtraction, thirty-two bits.</summary>
    I32Sub,
    /// <summary>Two's-complement multiplication, thirty-two bits.</summary>
    I32Mul,
    /// <summary>Bitwise and, thirty-two bits.</summary>
    I32And,
    /// <summary>Bitwise or, thirty-two bits.</summary>
    I32Or,
    /// <summary>Bitwise exclusive or, thirty-two bits.</summary>
    I32Xor,
    /// <summary>Left shift, the count masked to five bits.</summary>
    I32Shl,
    /// <summary>Arithmetic right shift, the count masked to five bits.</summary>
    I32ShrS,
    /// <summary>Logical right shift, the count masked to five bits.</summary>
    I32ShrU,
    /// <summary>Left rotation, the count masked to five bits.</summary>
    I32Rotl,
    /// <summary>Right rotation, the count masked to five bits.</summary>
    I32Rotr,
    /// <summary>Count of leading zero bits.</summary>
    I32Clz,
    /// <summary>Count of trailing zero bits.</summary>
    I32Ctz,
    /// <summary>Count of set bits.</summary>
    I32Popcnt,
    /// <summary>One when the word is zero, otherwise zero.</summary>
    I32Eqz,
    /// <summary>Equality, answering an i32 zero or one.</summary>
    I32Eq,
    /// <summary>Inequality.</summary>
    I32Ne,
    /// <summary>Signed less-than.</summary>
    I32LtS,
    /// <summary>Unsigned less-than.</summary>
    I32LtU,
    /// <summary>Signed greater-than.</summary>
    I32GtS,
    /// <summary>Unsigned greater-than.</summary>
    I32GtU,
    /// <summary>Signed less-than-or-equal.</summary>
    I32LeS,
    /// <summary>Unsigned less-than-or-equal.</summary>
    I32LeU,
    /// <summary>Signed greater-than-or-equal.</summary>
    I32GeS,
    /// <summary>Unsigned greater-than-or-equal.</summary>
    I32GeU,
    /// <summary>Signed division truncating toward zero; traps on a zero divisor and on the one overflowing quotient.</summary>
    I32DivS,
    /// <summary>Unsigned division; traps on a zero divisor.</summary>
    I32DivU,
    /// <summary>Signed remainder with the dividend's sign; traps on a zero divisor; the overflowing case answers zero.</summary>
    I32RemS,
    /// <summary>Unsigned remainder; traps on a zero divisor.</summary>
    I32RemU,

    /// <summary>Two's-complement addition, sixty-four bits.</summary>
    I64Add,
    /// <summary>Two's-complement subtraction, sixty-four bits.</summary>
    I64Sub,
    /// <summary>Two's-complement multiplication, sixty-four bits.</summary>
    I64Mul,
    /// <summary>Bitwise and, sixty-four bits.</summary>
    I64And,
    /// <summary>Bitwise or, sixty-four bits.</summary>
    I64Or,
    /// <summary>Bitwise exclusive or, sixty-four bits.</summary>
    I64Xor,
    /// <summary>Left shift, the count masked to six bits.</summary>
    I64Shl,
    /// <summary>Arithmetic right shift, the count masked to six bits.</summary>
    I64ShrS,
    /// <summary>Logical right shift, the count masked to six bits.</summary>
    I64ShrU,
    /// <summary>Left rotation, the count masked to six bits.</summary>
    I64Rotl,
    /// <summary>Right rotation, the count masked to six bits.</summary>
    I64Rotr,
    /// <summary>Count of leading zero bits.</summary>
    I64Clz,
    /// <summary>Count of trailing zero bits.</summary>
    I64Ctz,
    /// <summary>Count of set bits.</summary>
    I64Popcnt,
    /// <summary>One when the word is zero, otherwise zero, as an i32.</summary>
    I64Eqz,
    /// <summary>Equality, answering an i32.</summary>
    I64Eq,
    /// <summary>Inequality.</summary>
    I64Ne,
    /// <summary>Signed less-than.</summary>
    I64LtS,
    /// <summary>Unsigned less-than.</summary>
    I64LtU,
    /// <summary>Signed greater-than.</summary>
    I64GtS,
    /// <summary>Unsigned greater-than.</summary>
    I64GtU,
    /// <summary>Signed less-than-or-equal.</summary>
    I64LeS,
    /// <summary>Unsigned less-than-or-equal.</summary>
    I64LeU,
    /// <summary>Signed greater-than-or-equal.</summary>
    I64GeS,
    /// <summary>Unsigned greater-than-or-equal.</summary>
    I64GeU,
    /// <summary>Signed division; traps on a zero divisor and on the one overflowing quotient.</summary>
    I64DivS,
    /// <summary>Unsigned division; traps on a zero divisor.</summary>
    I64DivU,
    /// <summary>Signed remainder; traps on a zero divisor; the overflowing case answers zero.</summary>
    I64RemS,
    /// <summary>Unsigned remainder; traps on a zero divisor.</summary>
    I64RemU,

    /// <summary>IEEE-754 addition, binary32, round to nearest ties to even.</summary>
    F32Add,
    /// <summary>IEEE-754 subtraction, binary32.</summary>
    F32Sub,
    /// <summary>IEEE-754 multiplication, binary32.</summary>
    F32Mul,
    /// <summary>IEEE-754 division, binary32.</summary>
    F32Div,
    /// <summary>Minimum: NaN if either operand is NaN, and negative zero below positive zero.</summary>
    F32Min,
    /// <summary>Maximum: NaN if either operand is NaN, and positive zero above negative zero.</summary>
    F32Max,
    /// <summary>Absolute value: the sign bit cleared.</summary>
    F32Abs,
    /// <summary>Negation: the sign bit flipped.</summary>
    F32Neg,
    /// <summary>Square root.</summary>
    F32Sqrt,
    /// <summary>Round toward positive infinity.</summary>
    F32Ceil,
    /// <summary>Round toward negative infinity.</summary>
    F32Floor,
    /// <summary>Round toward zero.</summary>
    F32Trunc,
    /// <summary>Round to nearest, ties to even.</summary>
    F32Nearest,
    /// <summary>The first operand's magnitude with the second's sign.</summary>
    F32Copysign,
    /// <summary>Ordered equality, answering an i32.</summary>
    F32Eq,
    /// <summary>Inequality: true when either operand is NaN.</summary>
    F32Ne,
    /// <summary>Ordered less-than.</summary>
    F32Lt,
    /// <summary>Ordered greater-than.</summary>
    F32Gt,
    /// <summary>Ordered less-than-or-equal.</summary>
    F32Le,
    /// <summary>Ordered greater-than-or-equal.</summary>
    F32Ge,

    /// <summary>IEEE-754 addition, binary64.</summary>
    F64Add,
    /// <summary>IEEE-754 subtraction, binary64.</summary>
    F64Sub,
    /// <summary>IEEE-754 multiplication, binary64.</summary>
    F64Mul,
    /// <summary>IEEE-754 division, binary64.</summary>
    F64Div,
    /// <summary>Minimum, as the binary32 entry.</summary>
    F64Min,
    /// <summary>Maximum, as the binary32 entry.</summary>
    F64Max,
    /// <summary>Absolute value.</summary>
    F64Abs,
    /// <summary>Negation.</summary>
    F64Neg,
    /// <summary>Square root.</summary>
    F64Sqrt,
    /// <summary>Round toward positive infinity.</summary>
    F64Ceil,
    /// <summary>Round toward negative infinity.</summary>
    F64Floor,
    /// <summary>Round toward zero.</summary>
    F64Trunc,
    /// <summary>Round to nearest, ties to even.</summary>
    F64Nearest,
    /// <summary>The first operand's magnitude with the second's sign.</summary>
    F64Copysign,
    /// <summary>Ordered equality.</summary>
    F64Eq,
    /// <summary>Inequality.</summary>
    F64Ne,
    /// <summary>Ordered less-than.</summary>
    F64Lt,
    /// <summary>Ordered greater-than.</summary>
    F64Gt,
    /// <summary>Ordered less-than-or-equal.</summary>
    F64Le,
    /// <summary>Ordered greater-than-or-equal.</summary>
    F64Ge,

    /// <summary>The low thirty-two bits of a sixty-four-bit word.</summary>
    I32WrapI64,
    /// <summary>Sign extension to sixty-four bits.</summary>
    I64ExtendI32S,
    /// <summary>Zero extension to sixty-four bits.</summary>
    I64ExtendI32U,
    /// <summary>Signed i32 to the nearest binary32.</summary>
    F32ConvertI32S,
    /// <summary>Unsigned i32 to the nearest binary32.</summary>
    F32ConvertI32U,
    /// <summary>Signed i64 to the nearest binary32.</summary>
    F32ConvertI64S,
    /// <summary>Unsigned i64 to the nearest binary32.</summary>
    F32ConvertI64U,
    /// <summary>Signed i32 to binary64, exactly.</summary>
    F64ConvertI32S,
    /// <summary>Unsigned i32 to binary64, exactly.</summary>
    F64ConvertI32U,
    /// <summary>Signed i64 to the nearest binary64.</summary>
    F64ConvertI64S,
    /// <summary>Unsigned i64 to the nearest binary64.</summary>
    F64ConvertI64U,
    /// <summary>Binary64 to the nearest binary32.</summary>
    F32DemoteF64,
    /// <summary>Binary32 to binary64, exactly.</summary>
    F64PromoteF32,
    /// <summary>The bits of a binary32 as an i32.</summary>
    I32ReinterpretF32,
    /// <summary>The bits of a binary64 as an i64.</summary>
    I64ReinterpretF64,
    /// <summary>The bits of an i32 as a binary32.</summary>
    F32ReinterpretI32,
    /// <summary>The bits of an i64 as a binary64.</summary>
    F64ReinterpretI64,

    /// <summary>Binary32 to signed i32 toward zero; traps on NaN and out of range.</summary>
    I32TruncF32S,
    /// <summary>Binary32 to unsigned i32 toward zero; traps on NaN and out of range.</summary>
    I32TruncF32U,
    /// <summary>Binary64 to signed i32 toward zero; traps on NaN and out of range.</summary>
    I32TruncF64S,
    /// <summary>Binary64 to unsigned i32 toward zero; traps on NaN and out of range.</summary>
    I32TruncF64U,
    /// <summary>Binary32 to signed i64 toward zero; traps on NaN and out of range.</summary>
    I64TruncF32S,
    /// <summary>Binary32 to unsigned i64 toward zero; traps on NaN and out of range.</summary>
    I64TruncF32U,
    /// <summary>Binary64 to signed i64 toward zero; traps on NaN and out of range.</summary>
    I64TruncF64S,
    /// <summary>Binary64 to unsigned i64 toward zero; traps on NaN and out of range.</summary>
    I64TruncF64U,

    /// <summary>A little-endian i32 loaded from a region.</summary>
    RegionLoadI32,
    /// <summary>A little-endian i64 loaded from a region.</summary>
    RegionLoadI64,
    /// <summary>A little-endian binary32 loaded from a region.</summary>
    RegionLoadF32,
    /// <summary>A little-endian binary64 loaded from a region.</summary>
    RegionLoadF64,
    /// <summary>A byte loaded and sign-extended to i32.</summary>
    RegionLoadI32From8S,
    /// <summary>A byte loaded and zero-extended to i32.</summary>
    RegionLoadI32From8U,
    /// <summary>Sixteen bits loaded and sign-extended to i32.</summary>
    RegionLoadI32From16S,
    /// <summary>Sixteen bits loaded and zero-extended to i32.</summary>
    RegionLoadI32From16U,
    /// <summary>A byte loaded and sign-extended to i64.</summary>
    RegionLoadI64From8S,
    /// <summary>A byte loaded and zero-extended to i64.</summary>
    RegionLoadI64From8U,
    /// <summary>Sixteen bits loaded and sign-extended to i64.</summary>
    RegionLoadI64From16S,
    /// <summary>Sixteen bits loaded and zero-extended to i64.</summary>
    RegionLoadI64From16U,
    /// <summary>Thirty-two bits loaded and sign-extended to i64.</summary>
    RegionLoadI64From32S,
    /// <summary>Thirty-two bits loaded and zero-extended to i64.</summary>
    RegionLoadI64From32U,
    /// <summary>An i32 stored little-endian.</summary>
    RegionStoreI32,
    /// <summary>An i64 stored little-endian.</summary>
    RegionStoreI64,
    /// <summary>A binary32 stored little-endian.</summary>
    RegionStoreF32,
    /// <summary>A binary64 stored little-endian.</summary>
    RegionStoreF64,
    /// <summary>The low byte of an i32 stored.</summary>
    RegionStoreI32To8,
    /// <summary>The low sixteen bits of an i32 stored.</summary>
    RegionStoreI32To16,
    /// <summary>The low byte of an i64 stored.</summary>
    RegionStoreI64To8,
    /// <summary>The low sixteen bits of an i64 stored.</summary>
    RegionStoreI64To16,
    /// <summary>The low thirty-two bits of an i64 stored.</summary>
    RegionStoreI64To32,
    /// <summary>A region's size, in the units the family's region declares.</summary>
    RegionSize,

    /// <summary>The operand unchanged: the identity a family may name for a row that means nothing under a manifest.</summary>
    WordKeep,
}

/// <summary>The universal trap codes a primitive can raise.</summary>
/// <remarks>
/// A family table maps each one a primitive row can raise to a code of its own trap vocabulary, so the
/// fault payload a trap produces is always the family's.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=44DDB6
// Broiler-Human:        PENDING
[System.Flags]
public enum UbcTrapCode : byte
{
    /// <summary>No trap.</summary>
    None = 0,

    /// <summary>An integer division or remainder by zero.</summary>
    DivideByZero = 1,

    /// <summary>The one signed division whose quotient does not fit.</summary>
    IntegerOverflow = 2,

    /// <summary>A truncation of NaN, or of a value outside the target's range.</summary>
    InvalidConversion = 4,

    /// <summary>A region access whose effective address and width leave the region.</summary>
    OutOfBounds = 8,
}

/// <summary>The answer of one evaluation of a primitive: result bits, or a trap.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E8302B
// Broiler-Human:        PENDING
public readonly struct UbcPrimitiveResult
{
    private UbcPrimitiveResult(ulong bits, UbcTrapCode trap)
    {
        Bits = bits;
        Trap = trap;
    }

    /// <summary>The result bits, zero-extended; meaningless when <see cref="Trap"/> is not none.</summary>
    public ulong Bits { get; }

    /// <summary>The trap the evaluation raised, or <see cref="UbcTrapCode.None"/>.</summary>
    public UbcTrapCode Trap { get; }

    /// <summary>A value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A7B94B
    // Broiler-Human:        PENDING
    public static UbcPrimitiveResult Value(ulong bits) => new(bits, UbcTrapCode.None);

    /// <summary>A trap.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=022274
    // Broiler-Human:        PENDING
    public static UbcPrimitiveResult Trapped(UbcTrapCode trap) => new(0, trap);
}

/// <summary>
/// The primitive table's signatures, trap sets and reference evaluation: the C# statement of Appendix D
/// that the bytecode emitter executes and that obligation E2 compares every family handler and every
/// native emitter against.
/// </summary>
/// <remarks>
/// <para>
/// Words travel as <see cref="ulong"/> bit patterns: an <c>i32</c> in the low thirty-two bits with the
/// high bits zero, an <c>f32</c> as its binary32 bits in the low thirty-two, an <c>i64</c> and an
/// <c>f64</c> in all sixty-four. Every result is returned in that form.
/// </para>
/// <para>
/// <b>NaN canonicalisation is a flag the caller passes</b>, because it is a property of the family's
/// table rather than of the operation: when set, every floating-point result that is a NaN is replaced by
/// the canonical quiet NaN of its width (positive sign, top fraction bit only). Comparisons,
/// reinterpretations and the sign-bit operations <c>abs</c>, <c>neg</c> and <c>copysign</c> are not
/// arithmetic and are never canonicalised.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=523F94
// Broiler-Falsified-If: an entry answers a result bit pattern IEEE-754 or two's complement does not give for some input, or fails to trap where the entry's trap set says it must
// Broiler-Human:        PENDING
public static class UbcPrimitives
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B41D53
    // Broiler-Human:        PENDING
    private const uint CanonicalNaN32 = 0x7FC0_0000;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F77825
    // Broiler-Human:        PENDING
    private const ulong CanonicalNaN64 = 0x7FF8_0000_0000_0000;

    /// <summary>True when <paramref name="primitive"/> names an entry of the table.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A21622
    // Broiler-Human:        PENDING
    public static bool IsDefined(UbcPrimitive primitive) =>
        primitive >= UbcPrimitive.I32Add && primitive <= UbcPrimitive.WordKeep;

    /// <summary>True for the region-access entries, which name a family region and read a region at run time.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F49D1D
    // Broiler-Human:        PENDING
    public static bool IsRegionAccess(UbcPrimitive primitive) =>
        primitive >= UbcPrimitive.RegionLoadI32 && primitive <= UbcPrimitive.RegionSize;

    /// <summary>True for the entries whose result is a floating-point value the NaN flag applies to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1A9551
    // Broiler-Human:        PENDING
    public static bool Canonicalises(UbcPrimitive primitive) => primitive switch
    {
        UbcPrimitive.F32Add or UbcPrimitive.F32Sub or UbcPrimitive.F32Mul or UbcPrimitive.F32Div
            or UbcPrimitive.F32Min or UbcPrimitive.F32Max or UbcPrimitive.F32Sqrt or UbcPrimitive.F32Ceil
            or UbcPrimitive.F32Floor or UbcPrimitive.F32Trunc or UbcPrimitive.F32Nearest
            or UbcPrimitive.F64Add or UbcPrimitive.F64Sub or UbcPrimitive.F64Mul or UbcPrimitive.F64Div
            or UbcPrimitive.F64Min or UbcPrimitive.F64Max or UbcPrimitive.F64Sqrt or UbcPrimitive.F64Ceil
            or UbcPrimitive.F64Floor or UbcPrimitive.F64Trunc or UbcPrimitive.F64Nearest
            or UbcPrimitive.F32DemoteF64 or UbcPrimitive.F64PromoteF32 => true,
        _ => false,
    };

    /// <summary>The universal traps <paramref name="primitive"/> can raise.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=9CB1EF
    // Broiler-Falsified-If: an entry whose evaluation can trap answers a set that omits that trap
    // Broiler-Human:        PENDING
    public static UbcTrapCode TrapsOf(UbcPrimitive primitive) => primitive switch
    {
        UbcPrimitive.I32DivS or UbcPrimitive.I64DivS => UbcTrapCode.DivideByZero | UbcTrapCode.IntegerOverflow,
        UbcPrimitive.I32DivU or UbcPrimitive.I32RemS or UbcPrimitive.I32RemU
            or UbcPrimitive.I64DivU or UbcPrimitive.I64RemS or UbcPrimitive.I64RemU => UbcTrapCode.DivideByZero,
        >= UbcPrimitive.I32TruncF32S and <= UbcPrimitive.I64TruncF64U => UbcTrapCode.InvalidConversion,
        >= UbcPrimitive.RegionLoadI32 and <= UbcPrimitive.RegionStoreI64To32 => UbcTrapCode.OutOfBounds,
        _ => UbcTrapCode.None,
    };

    /// <summary>
    /// The signature of <paramref name="primitive"/>: its operand types bottom to top, and its result
    /// types. <see cref="UbcPrimitive.WordKeep"/> has no fixed signature: it keeps any one word type, and
    /// answers false here; a table matches it against the row's own effect instead.
    /// </summary>
    /// <remarks>
    /// A region load's operand is the <c>i32</c> address; a region store's operands are the address and
    /// the value; <see cref="UbcPrimitive.RegionSize"/> takes nothing and answers an <c>i32</c>. The
    /// static offset of a region access is the second field of its row's <c>U8U32</c> operand, or the
    /// whole of a <c>U32</c> operand, and is not a stack operand.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=AB80AF
    // Broiler-Falsified-If: an entry's signature disagrees with the types its evaluation reads and writes
    // Broiler-Human:        PENDING
    public static bool TryGetSignature(
        UbcPrimitive primitive,
        out ImmutableArray<UbcSlotType> operands,
        out ImmutableArray<UbcSlotType> results)
    {
        var i32 = UbcSlotType.I32;
        var i64 = UbcSlotType.I64;
        var f32 = UbcSlotType.F32;
        var f64 = UbcSlotType.F64;

        (UbcSlotType[] Operands, UbcSlotType[] Results)? signature = primitive switch
        {
            >= UbcPrimitive.I32Add and <= UbcPrimitive.I32Rotr => ([i32, i32], [i32]),
            UbcPrimitive.I32Clz or UbcPrimitive.I32Ctz or UbcPrimitive.I32Popcnt or UbcPrimitive.I32Eqz => ([i32], [i32]),
            >= UbcPrimitive.I32Eq and <= UbcPrimitive.I32RemU => ([i32, i32], [i32]),
            >= UbcPrimitive.I64Add and <= UbcPrimitive.I64Rotr => ([i64, i64], [i64]),
            UbcPrimitive.I64Clz or UbcPrimitive.I64Ctz or UbcPrimitive.I64Popcnt => ([i64], [i64]),
            UbcPrimitive.I64Eqz => ([i64], [i32]),
            >= UbcPrimitive.I64Eq and <= UbcPrimitive.I64GeU => ([i64, i64], [i32]),
            >= UbcPrimitive.I64DivS and <= UbcPrimitive.I64RemU => ([i64, i64], [i64]),
            >= UbcPrimitive.F32Add and <= UbcPrimitive.F32Max => ([f32, f32], [f32]),
            >= UbcPrimitive.F32Abs and <= UbcPrimitive.F32Nearest => ([f32], [f32]),
            UbcPrimitive.F32Copysign => ([f32, f32], [f32]),
            >= UbcPrimitive.F32Eq and <= UbcPrimitive.F32Ge => ([f32, f32], [i32]),
            >= UbcPrimitive.F64Add and <= UbcPrimitive.F64Max => ([f64, f64], [f64]),
            >= UbcPrimitive.F64Abs and <= UbcPrimitive.F64Nearest => ([f64], [f64]),
            UbcPrimitive.F64Copysign => ([f64, f64], [f64]),
            >= UbcPrimitive.F64Eq and <= UbcPrimitive.F64Ge => ([f64, f64], [i32]),
            UbcPrimitive.I32WrapI64 => ([i64], [i32]),
            UbcPrimitive.I64ExtendI32S or UbcPrimitive.I64ExtendI32U => ([i32], [i64]),
            UbcPrimitive.F32ConvertI32S or UbcPrimitive.F32ConvertI32U => ([i32], [f32]),
            UbcPrimitive.F32ConvertI64S or UbcPrimitive.F32ConvertI64U => ([i64], [f32]),
            UbcPrimitive.F64ConvertI32S or UbcPrimitive.F64ConvertI32U => ([i32], [f64]),
            UbcPrimitive.F64ConvertI64S or UbcPrimitive.F64ConvertI64U => ([i64], [f64]),
            UbcPrimitive.F32DemoteF64 => ([f64], [f32]),
            UbcPrimitive.F64PromoteF32 => ([f32], [f64]),
            UbcPrimitive.I32ReinterpretF32 => ([f32], [i32]),
            UbcPrimitive.I64ReinterpretF64 => ([f64], [i64]),
            UbcPrimitive.F32ReinterpretI32 => ([i32], [f32]),
            UbcPrimitive.F64ReinterpretI64 => ([i64], [f64]),
            UbcPrimitive.I32TruncF32S or UbcPrimitive.I32TruncF32U => ([f32], [i32]),
            UbcPrimitive.I32TruncF64S or UbcPrimitive.I32TruncF64U => ([f64], [i32]),
            UbcPrimitive.I64TruncF32S or UbcPrimitive.I64TruncF32U => ([f32], [i64]),
            UbcPrimitive.I64TruncF64S or UbcPrimitive.I64TruncF64U => ([f64], [i64]),
            UbcPrimitive.RegionLoadI32 or UbcPrimitive.RegionLoadI32From8S or UbcPrimitive.RegionLoadI32From8U
                or UbcPrimitive.RegionLoadI32From16S or UbcPrimitive.RegionLoadI32From16U => ([i32], [i32]),
            UbcPrimitive.RegionLoadI64 or (>= UbcPrimitive.RegionLoadI64From8S and <= UbcPrimitive.RegionLoadI64From32U) => ([i32], [i64]),
            UbcPrimitive.RegionLoadF32 => ([i32], [f32]),
            UbcPrimitive.RegionLoadF64 => ([i32], [f64]),
            UbcPrimitive.RegionStoreI32 or UbcPrimitive.RegionStoreI32To8 or UbcPrimitive.RegionStoreI32To16 => ([i32, i32], []),
            UbcPrimitive.RegionStoreI64 or UbcPrimitive.RegionStoreI64To8 or UbcPrimitive.RegionStoreI64To16
                or UbcPrimitive.RegionStoreI64To32 => ([i32, i64], []),
            UbcPrimitive.RegionStoreF32 => ([i32, f32], []),
            UbcPrimitive.RegionStoreF64 => ([i32, f64], []),
            UbcPrimitive.RegionSize => ([], [i32]),
            _ => null,
        };

        if (signature is null)
        {
            operands = default;
            results = default;
            return false;
        }

        operands = ImmutableArray.Create(signature.Value.Operands);
        results = ImmutableArray.Create(signature.Value.Results);
        return true;
    }

    /// <summary>The width in bytes a region access reads or writes; zero for anything else.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=CE2261
    // Broiler-Falsified-If: an access answers a width other than the bytes it moves, so the bounds check admits a read past the region
    // Broiler-Human:        PENDING
    public static int AccessWidth(UbcPrimitive primitive) => primitive switch
    {
        UbcPrimitive.RegionLoadI32 or UbcPrimitive.RegionLoadF32 or UbcPrimitive.RegionStoreI32
            or UbcPrimitive.RegionStoreF32 or UbcPrimitive.RegionLoadI64From32S or UbcPrimitive.RegionLoadI64From32U
            or UbcPrimitive.RegionStoreI64To32 => 4,
        UbcPrimitive.RegionLoadI64 or UbcPrimitive.RegionLoadF64 or UbcPrimitive.RegionStoreI64
            or UbcPrimitive.RegionStoreF64 => 8,
        UbcPrimitive.RegionLoadI32From8S or UbcPrimitive.RegionLoadI32From8U or UbcPrimitive.RegionLoadI64From8S
            or UbcPrimitive.RegionLoadI64From8U or UbcPrimitive.RegionStoreI32To8 or UbcPrimitive.RegionStoreI64To8 => 1,
        UbcPrimitive.RegionLoadI32From16S or UbcPrimitive.RegionLoadI32From16U or UbcPrimitive.RegionLoadI64From16S
            or UbcPrimitive.RegionLoadI64From16U or UbcPrimitive.RegionStoreI32To16 or UbcPrimitive.RegionStoreI64To16 => 2,
        _ => 0,
    };

    /// <summary>
    /// Evaluates a primitive that takes no region: <paramref name="a"/> is the deeper operand and
    /// <paramref name="b"/> the top one (a one-operand entry reads <paramref name="a"/> only).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=6C38AB
    // Broiler-Falsified-If: some input yields a result that differs, in any bit, from the entry's statement in Appendix D under the NaN flag given
    // Broiler-Human:        PENDING
    public static UbcPrimitiveResult Evaluate(UbcPrimitive primitive, ulong a, ulong b, bool canonicaliseNaN)
    {
        var result = EvaluateCore(primitive, a, b);

        if (!canonicaliseNaN || result.Trap != UbcTrapCode.None || !Canonicalises(primitive))
        {
            return result;
        }

        return UbcPrimitiveResult.Value(Canonical(primitive, result.Bits));
    }

    /// <summary>
    /// Evaluates a region access over <paramref name="region"/>: a load reads the effective address
    /// <c>address + offset</c> computed without overflow and answers its value, a store writes
    /// <paramref name="value"/> there, and <see cref="UbcPrimitive.RegionSize"/> answers
    /// <paramref name="size"/>. The access traps out of bounds when <c>address + offset + width</c>
    /// exceeds the region's length. Unaligned access is admitted.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=CB4415
    // Broiler-Falsified-If: an access whose effective address plus width exceeds the region's length reads or writes instead of trapping, including through overflow of the sum
    // Broiler-Human:        PENDING
    public static UbcPrimitiveResult EvaluateRegion(
        UbcPrimitive primitive,
        System.Span<byte> region,
        uint address,
        uint offset,
        ulong value,
        uint size)
    {
        if (primitive == UbcPrimitive.RegionSize)
        {
            return UbcPrimitiveResult.Value(size);
        }

        var width = AccessWidth(primitive);

        if (width == 0)
        {
            return UbcPrimitiveResult.Trapped(UbcTrapCode.OutOfBounds);
        }

        // Sixty-four-bit arithmetic cannot overflow here: two thirty-two-bit values and a width of at
        // most eight sum to far less than the range, so the comparison below is exact.
        var effective = (ulong)address + offset;

        if (effective + (ulong)width > (ulong)region.Length)
        {
            return UbcPrimitiveResult.Trapped(UbcTrapCode.OutOfBounds);
        }

        var slice = region.Slice((int)effective, width);

        switch (primitive)
        {
            case UbcPrimitive.RegionStoreI32:
            case UbcPrimitive.RegionStoreF32:
            case UbcPrimitive.RegionStoreI64To32:
            case UbcPrimitive.RegionStoreI64:
            case UbcPrimitive.RegionStoreF64:
            case UbcPrimitive.RegionStoreI32To8:
            case UbcPrimitive.RegionStoreI64To8:
            case UbcPrimitive.RegionStoreI32To16:
            case UbcPrimitive.RegionStoreI64To16:
                UbcOperandShapes.Write(slice, value, width);
                return UbcPrimitiveResult.Value(0);
        }

        var raw = UbcOperandShapes.Read(slice);

        return primitive switch
        {
            UbcPrimitive.RegionLoadI32From8S => UbcPrimitiveResult.Value((uint)(int)(sbyte)(byte)raw),
            UbcPrimitive.RegionLoadI32From16S => UbcPrimitiveResult.Value((uint)(int)(short)(ushort)raw),
            UbcPrimitive.RegionLoadI64From8S => UbcPrimitiveResult.Value((ulong)(long)(sbyte)(byte)raw),
            UbcPrimitive.RegionLoadI64From16S => UbcPrimitiveResult.Value((ulong)(long)(short)(ushort)raw),
            UbcPrimitive.RegionLoadI64From32S => UbcPrimitiveResult.Value((ulong)(long)(int)(uint)raw),
            _ => UbcPrimitiveResult.Value(raw),
        };
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=D6AA6A
    // Broiler-Falsified-If: an integer entry's result keeps bits above its width, or a shift or rotation uses an unmasked count
    // Broiler-Human:        PENDING
    private static UbcPrimitiveResult EvaluateCore(UbcPrimitive primitive, ulong a, ulong b)
    {
        var a32 = (uint)a;
        var b32 = (uint)b;

        switch (primitive)
        {
            case UbcPrimitive.I32Add: return I32(a32 + b32);
            case UbcPrimitive.I32Sub: return I32(a32 - b32);
            case UbcPrimitive.I32Mul: return I32(a32 * b32);
            case UbcPrimitive.I32And: return I32(a32 & b32);
            case UbcPrimitive.I32Or: return I32(a32 | b32);
            case UbcPrimitive.I32Xor: return I32(a32 ^ b32);
            case UbcPrimitive.I32Shl: return I32(a32 << (int)(b32 & 31));
            case UbcPrimitive.I32ShrS: return I32((uint)((int)a32 >> (int)(b32 & 31)));
            case UbcPrimitive.I32ShrU: return I32(a32 >> (int)(b32 & 31));
            case UbcPrimitive.I32Rotl: return I32(System.Numerics.BitOperations.RotateLeft(a32, (int)(b32 & 31)));
            case UbcPrimitive.I32Rotr: return I32(System.Numerics.BitOperations.RotateRight(a32, (int)(b32 & 31)));
            case UbcPrimitive.I32Clz: return I32((uint)System.Numerics.BitOperations.LeadingZeroCount(a32));
            case UbcPrimitive.I32Ctz: return I32((uint)System.Numerics.BitOperations.TrailingZeroCount(a32));
            case UbcPrimitive.I32Popcnt: return I32((uint)System.Numerics.BitOperations.PopCount(a32));
            case UbcPrimitive.I32Eqz: return Bool(a32 == 0);
            case UbcPrimitive.I32Eq: return Bool(a32 == b32);
            case UbcPrimitive.I32Ne: return Bool(a32 != b32);
            case UbcPrimitive.I32LtS: return Bool((int)a32 < (int)b32);
            case UbcPrimitive.I32LtU: return Bool(a32 < b32);
            case UbcPrimitive.I32GtS: return Bool((int)a32 > (int)b32);
            case UbcPrimitive.I32GtU: return Bool(a32 > b32);
            case UbcPrimitive.I32LeS: return Bool((int)a32 <= (int)b32);
            case UbcPrimitive.I32LeU: return Bool(a32 <= b32);
            case UbcPrimitive.I32GeS: return Bool((int)a32 >= (int)b32);
            case UbcPrimitive.I32GeU: return Bool(a32 >= b32);
            case UbcPrimitive.I32DivS:
                if (b32 == 0) { return UbcPrimitiveResult.Trapped(UbcTrapCode.DivideByZero); }
                if ((int)a32 == int.MinValue && (int)b32 == -1) { return UbcPrimitiveResult.Trapped(UbcTrapCode.IntegerOverflow); }
                return I32((uint)((int)a32 / (int)b32));
            case UbcPrimitive.I32DivU:
                if (b32 == 0) { return UbcPrimitiveResult.Trapped(UbcTrapCode.DivideByZero); }
                return I32(a32 / b32);
            case UbcPrimitive.I32RemS:
                if (b32 == 0) { return UbcPrimitiveResult.Trapped(UbcTrapCode.DivideByZero); }
                if ((int)b32 == -1) { return I32(0); }
                return I32((uint)((int)a32 % (int)b32));
            case UbcPrimitive.I32RemU:
                if (b32 == 0) { return UbcPrimitiveResult.Trapped(UbcTrapCode.DivideByZero); }
                return I32(a32 % b32);

            case UbcPrimitive.I64Add: return UbcPrimitiveResult.Value(a + b);
            case UbcPrimitive.I64Sub: return UbcPrimitiveResult.Value(a - b);
            case UbcPrimitive.I64Mul: return UbcPrimitiveResult.Value(a * b);
            case UbcPrimitive.I64And: return UbcPrimitiveResult.Value(a & b);
            case UbcPrimitive.I64Or: return UbcPrimitiveResult.Value(a | b);
            case UbcPrimitive.I64Xor: return UbcPrimitiveResult.Value(a ^ b);
            case UbcPrimitive.I64Shl: return UbcPrimitiveResult.Value(a << (int)(b & 63));
            case UbcPrimitive.I64ShrS: return UbcPrimitiveResult.Value((ulong)((long)a >> (int)(b & 63)));
            case UbcPrimitive.I64ShrU: return UbcPrimitiveResult.Value(a >> (int)(b & 63));
            case UbcPrimitive.I64Rotl: return UbcPrimitiveResult.Value(System.Numerics.BitOperations.RotateLeft(a, (int)(b & 63)));
            case UbcPrimitive.I64Rotr: return UbcPrimitiveResult.Value(System.Numerics.BitOperations.RotateRight(a, (int)(b & 63)));
            case UbcPrimitive.I64Clz: return UbcPrimitiveResult.Value((ulong)System.Numerics.BitOperations.LeadingZeroCount(a));
            case UbcPrimitive.I64Ctz: return UbcPrimitiveResult.Value((ulong)System.Numerics.BitOperations.TrailingZeroCount(a));
            case UbcPrimitive.I64Popcnt: return UbcPrimitiveResult.Value((ulong)System.Numerics.BitOperations.PopCount(a));
            case UbcPrimitive.I64Eqz: return Bool(a == 0);
            case UbcPrimitive.I64Eq: return Bool(a == b);
            case UbcPrimitive.I64Ne: return Bool(a != b);
            case UbcPrimitive.I64LtS: return Bool((long)a < (long)b);
            case UbcPrimitive.I64LtU: return Bool(a < b);
            case UbcPrimitive.I64GtS: return Bool((long)a > (long)b);
            case UbcPrimitive.I64GtU: return Bool(a > b);
            case UbcPrimitive.I64LeS: return Bool((long)a <= (long)b);
            case UbcPrimitive.I64LeU: return Bool(a <= b);
            case UbcPrimitive.I64GeS: return Bool((long)a >= (long)b);
            case UbcPrimitive.I64GeU: return Bool(a >= b);
            case UbcPrimitive.I64DivS:
                if (b == 0) { return UbcPrimitiveResult.Trapped(UbcTrapCode.DivideByZero); }
                if ((long)a == long.MinValue && (long)b == -1) { return UbcPrimitiveResult.Trapped(UbcTrapCode.IntegerOverflow); }
                return UbcPrimitiveResult.Value((ulong)((long)a / (long)b));
            case UbcPrimitive.I64DivU:
                if (b == 0) { return UbcPrimitiveResult.Trapped(UbcTrapCode.DivideByZero); }
                return UbcPrimitiveResult.Value(a / b);
            case UbcPrimitive.I64RemS:
                if (b == 0) { return UbcPrimitiveResult.Trapped(UbcTrapCode.DivideByZero); }
                if ((long)b == -1) { return UbcPrimitiveResult.Value(0); }
                return UbcPrimitiveResult.Value((ulong)((long)a % (long)b));
            case UbcPrimitive.I64RemU:
                if (b == 0) { return UbcPrimitiveResult.Trapped(UbcTrapCode.DivideByZero); }
                return UbcPrimitiveResult.Value(a % b);
        }

        return EvaluateFloat(primitive, a, b);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=819B5A
    // Broiler-Falsified-If: a floating-point entry's result differs in any bit from IEEE-754 round-to-nearest-even, or min and max do not order the zeros and propagate NaN
    // Broiler-Human:        PENDING
    private static UbcPrimitiveResult EvaluateFloat(UbcPrimitive primitive, ulong a, ulong b)
    {
        var fa = System.BitConverter.UInt32BitsToSingle((uint)a);
        var fb = System.BitConverter.UInt32BitsToSingle((uint)b);
        var da = System.BitConverter.UInt64BitsToDouble(a);
        var db = System.BitConverter.UInt64BitsToDouble(b);

        switch (primitive)
        {
            case UbcPrimitive.F32Add: return F32(fa + fb);
            case UbcPrimitive.F32Sub: return F32(fa - fb);
            case UbcPrimitive.F32Mul: return F32(fa * fb);
            case UbcPrimitive.F32Div: return F32(fa / fb);
            case UbcPrimitive.F32Min: return F32(MinOf(fa, fb));
            case UbcPrimitive.F32Max: return F32(MaxOf(fa, fb));
            case UbcPrimitive.F32Abs: return I32((uint)a & 0x7FFF_FFFF);
            case UbcPrimitive.F32Neg: return I32((uint)a ^ 0x8000_0000);
            case UbcPrimitive.F32Sqrt: return F32(System.MathF.Sqrt(fa));
            case UbcPrimitive.F32Ceil: return F32(System.MathF.Ceiling(fa));
            case UbcPrimitive.F32Floor: return F32(System.MathF.Floor(fa));
            case UbcPrimitive.F32Trunc: return F32(System.MathF.Truncate(fa));
            case UbcPrimitive.F32Nearest: return F32(System.MathF.Round(fa, System.MidpointRounding.ToEven));
            case UbcPrimitive.F32Copysign: return I32(((uint)a & 0x7FFF_FFFF) | ((uint)b & 0x8000_0000));
            case UbcPrimitive.F32Eq: return Bool(fa == fb);
            case UbcPrimitive.F32Ne: return Bool(fa != fb);
            case UbcPrimitive.F32Lt: return Bool(fa < fb);
            case UbcPrimitive.F32Gt: return Bool(fa > fb);
            case UbcPrimitive.F32Le: return Bool(fa <= fb);
            case UbcPrimitive.F32Ge: return Bool(fa >= fb);

            case UbcPrimitive.F64Add: return F64(da + db);
            case UbcPrimitive.F64Sub: return F64(da - db);
            case UbcPrimitive.F64Mul: return F64(da * db);
            case UbcPrimitive.F64Div: return F64(da / db);
            case UbcPrimitive.F64Min: return F64(MinOf(da, db));
            case UbcPrimitive.F64Max: return F64(MaxOf(da, db));
            case UbcPrimitive.F64Abs: return UbcPrimitiveResult.Value(a & 0x7FFF_FFFF_FFFF_FFFF);
            case UbcPrimitive.F64Neg: return UbcPrimitiveResult.Value(a ^ 0x8000_0000_0000_0000);
            case UbcPrimitive.F64Sqrt: return F64(System.Math.Sqrt(da));
            case UbcPrimitive.F64Ceil: return F64(System.Math.Ceiling(da));
            case UbcPrimitive.F64Floor: return F64(System.Math.Floor(da));
            case UbcPrimitive.F64Trunc: return F64(System.Math.Truncate(da));
            case UbcPrimitive.F64Nearest: return F64(System.Math.Round(da, System.MidpointRounding.ToEven));
            case UbcPrimitive.F64Copysign: return UbcPrimitiveResult.Value((a & 0x7FFF_FFFF_FFFF_FFFF) | (b & 0x8000_0000_0000_0000));
            case UbcPrimitive.F64Eq: return Bool(da == db);
            case UbcPrimitive.F64Ne: return Bool(da != db);
            case UbcPrimitive.F64Lt: return Bool(da < db);
            case UbcPrimitive.F64Gt: return Bool(da > db);
            case UbcPrimitive.F64Le: return Bool(da <= db);
            case UbcPrimitive.F64Ge: return Bool(da >= db);

            case UbcPrimitive.I32WrapI64: return I32((uint)a);
            case UbcPrimitive.I64ExtendI32S: return UbcPrimitiveResult.Value((ulong)(long)(int)(uint)a);
            case UbcPrimitive.I64ExtendI32U: return UbcPrimitiveResult.Value((uint)a);
            case UbcPrimitive.F32ConvertI32S: return F32(SignedToSingle((int)(uint)a));
            case UbcPrimitive.F32ConvertI32U: return F32(UnsignedToSingle((uint)a));
            case UbcPrimitive.F32ConvertI64S: return F32(SignedToSingle((long)a));
            case UbcPrimitive.F32ConvertI64U: return F32(UnsignedToSingle(a));
            case UbcPrimitive.F64ConvertI32S: return F64((int)(uint)a);
            case UbcPrimitive.F64ConvertI32U: return F64((uint)a);
            case UbcPrimitive.F64ConvertI64S: return F64((long)a);
            case UbcPrimitive.F64ConvertI64U: return F64(UnsignedToDouble(a));
            case UbcPrimitive.F32DemoteF64: return F32((float)da);
            case UbcPrimitive.F64PromoteF32: return F64(fa);
            case UbcPrimitive.I32ReinterpretF32: return I32((uint)a);
            case UbcPrimitive.I64ReinterpretF64: return UbcPrimitiveResult.Value(a);
            case UbcPrimitive.F32ReinterpretI32: return I32((uint)a);
            case UbcPrimitive.F64ReinterpretI64: return UbcPrimitiveResult.Value(a);

            case UbcPrimitive.I32TruncF32S: return TruncToI32(fa, signed: true);
            case UbcPrimitive.I32TruncF32U: return TruncToI32(fa, signed: false);
            case UbcPrimitive.I32TruncF64S: return TruncToI32(da, signed: true);
            case UbcPrimitive.I32TruncF64U: return TruncToI32(da, signed: false);
            case UbcPrimitive.I64TruncF32S: return TruncToI64(fa, signed: true);
            case UbcPrimitive.I64TruncF32U: return TruncToI64(fa, signed: false);
            case UbcPrimitive.I64TruncF64S: return TruncToI64(da, signed: true);
            case UbcPrimitive.I64TruncF64U: return TruncToI64(da, signed: false);

            case UbcPrimitive.WordKeep: return UbcPrimitiveResult.Value(a);
        }

        // A region access or an undefined entry reaching here is a caller defect, and it answers a trap
        // rather than a value so that nothing downstream mistakes it for a result.
        return UbcPrimitiveResult.Trapped(UbcTrapCode.OutOfBounds);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5F2E6A
    // Broiler-Falsified-If: a NaN operand yields a number, or min of the two zeros answers positive zero
    // Broiler-Human:        PENDING
    private static float MinOf(float a, float b)
    {
        if (float.IsNaN(a) || float.IsNaN(b))
        {
            return float.NaN;
        }

        if (a == 0f && b == 0f)
        {
            return float.IsNegative(a) ? a : b;
        }

        return a < b ? a : b;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E16682
    // Broiler-Falsified-If: a NaN operand yields a number, or max of the two zeros answers negative zero
    // Broiler-Human:        PENDING
    private static float MaxOf(float a, float b)
    {
        if (float.IsNaN(a) || float.IsNaN(b))
        {
            return float.NaN;
        }

        if (a == 0f && b == 0f)
        {
            return float.IsNegative(a) ? b : a;
        }

        return a > b ? a : b;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=80BF41
    // Broiler-Falsified-If: a NaN operand yields a number, or min of the two zeros answers positive zero
    // Broiler-Human:        PENDING
    private static double MinOf(double a, double b)
    {
        if (double.IsNaN(a) || double.IsNaN(b))
        {
            return double.NaN;
        }

        if (a == 0d && b == 0d)
        {
            return double.IsNegative(a) ? a : b;
        }

        return a < b ? a : b;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3A2972
    // Broiler-Falsified-If: a NaN operand yields a number, or max of the two zeros answers negative zero
    // Broiler-Human:        PENDING
    private static double MaxOf(double a, double b)
    {
        if (double.IsNaN(a) || double.IsNaN(b))
        {
            return double.NaN;
        }

        if (a == 0d && b == 0d)
        {
            return double.IsNegative(a) ? b : a;
        }

        return a > b ? a : b;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=64E30C
    // Broiler-Falsified-If: a value whose truncation lies outside the target range converts instead of trapping, or NaN converts
    // Broiler-Human:        PENDING
    private static UbcPrimitiveResult TruncToI32(double value, bool signed)
    {
        if (double.IsNaN(value))
        {
            return UbcPrimitiveResult.Trapped(UbcTrapCode.InvalidConversion);
        }

        var truncated = System.Math.Truncate(value);

        if (signed)
        {
            if (truncated < -2147483648.0 || truncated > 2147483647.0)
            {
                return UbcPrimitiveResult.Trapped(UbcTrapCode.InvalidConversion);
            }

            return I32((uint)(int)truncated);
        }

        if (truncated < 0.0 || truncated > 4294967295.0)
        {
            return UbcPrimitiveResult.Trapped(UbcTrapCode.InvalidConversion);
        }

        return I32((uint)truncated);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=66725E
    // Broiler-Falsified-If: a value whose truncation lies outside the sixty-four-bit range converts instead of trapping, the bound itself included
    // Broiler-Human:        PENDING
    private static UbcPrimitiveResult TruncToI64(double value, bool signed)
    {
        if (double.IsNaN(value))
        {
            return UbcPrimitiveResult.Trapped(UbcTrapCode.InvalidConversion);
        }

        var truncated = System.Math.Truncate(value);

        if (signed)
        {
            // -2^63 is exactly representable and in range; +2^63 is exactly representable and is not.
            if (truncated < -9223372036854775808.0 || truncated >= 9223372036854775808.0)
            {
                return UbcPrimitiveResult.Trapped(UbcTrapCode.InvalidConversion);
            }

            return UbcPrimitiveResult.Value((ulong)(long)truncated);
        }

        if (truncated < 0.0 || truncated >= 18446744073709551616.0)
        {
            return UbcPrimitiveResult.Trapped(UbcTrapCode.InvalidConversion);
        }

        return UbcPrimitiveResult.Value((ulong)truncated);
    }

    /// <summary>
    /// An unsigned sixty-four-bit integer rounded once, to nearest with ties to even, to binary32.
    /// </summary>
    /// <remarks>
    /// Converting through binary64 rounds twice for a value of more than fifty-three significant bits,
    /// and double rounding can land one unit away from the correctly rounded result. So a wide value is
    /// first cut to fifty-three bits with every dropped bit folded into the lowest kept one as a sticky
    /// bit, which is exact in binary64 and keeps everything the one rounding to binary32 looks at.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B9DA90
    // Broiler-Falsified-If: some value above two to the fifty-third rounds to a binary32 other than the correctly rounded one
    // Broiler-Human:        PENDING
    private static float UnsignedToSingle(ulong value)
    {
        if (value < 1UL << 53)
        {
            return (float)(double)value;
        }

        var drop = 64 - System.Numerics.BitOperations.LeadingZeroCount(value) - 53;
        var kept = value >> drop;

        if ((value & ((1UL << drop) - 1)) != 0)
        {
            kept |= 1;
        }

        return (float)System.Math.ScaleB((double)kept, drop);
    }

    /// <summary>A signed sixty-four-bit integer rounded once to binary32.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=702467
    // Broiler-Falsified-If: the most negative value, or any value, rounds differently from its magnitude's rounding negated
    // Broiler-Human:        PENDING
    private static float SignedToSingle(long value)
    {
        if (value >= 0)
        {
            return UnsignedToSingle((ulong)value);
        }

        // The magnitude of the most negative value is two to the sixty-third, which the unsigned
        // conversion represents exactly; negating a binary32 is exact.
        return -UnsignedToSingle(unchecked((ulong)-value));
    }

    /// <summary>An unsigned sixty-four-bit integer rounded once to binary64.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B00027
    // Broiler-Falsified-If: some value at or above two to the sixty-third rounds to a binary64 other than the correctly rounded one
    // Broiler-Human:        PENDING
    private static double UnsignedToDouble(ulong value)
    {
        if (value < 1UL << 63)
        {
            return (long)value;
        }

        // Halve with the dropped bit kept as a sticky bit, convert the now-positive signed value in one
        // rounding, and double it back, which is exact.
        var halved = (value >> 1) | (value & 1);
        return (long)halved * 2.0;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AB7EC8
    // Broiler-Human:        PENDING
    private static ulong Canonical(UbcPrimitive primitive, ulong bits)
    {
        if (ResultIsF32(primitive))
        {
            return float.IsNaN(System.BitConverter.UInt32BitsToSingle((uint)bits)) ? CanonicalNaN32 : bits;
        }

        return double.IsNaN(System.BitConverter.UInt64BitsToDouble(bits)) ? CanonicalNaN64 : bits;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=BE2863
    // Broiler-Human:        PENDING
    private static bool ResultIsF32(UbcPrimitive primitive) =>
        primitive is (>= UbcPrimitive.F32Add and <= UbcPrimitive.F32Ge) or UbcPrimitive.F32DemoteF64;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1B59D1
    // Broiler-Human:        PENDING
    private static UbcPrimitiveResult I32(uint value) => UbcPrimitiveResult.Value(value);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=577C6F
    // Broiler-Human:        PENDING
    private static UbcPrimitiveResult Bool(bool value) => UbcPrimitiveResult.Value(value ? 1u : 0u);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0540F8
    // Broiler-Human:        PENDING
    private static UbcPrimitiveResult F32(float value) => UbcPrimitiveResult.Value(System.BitConverter.SingleToUInt32Bits(value));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=25A59B
    // Broiler-Human:        PENDING
    private static UbcPrimitiveResult F64(double value) => UbcPrimitiveResult.Value(System.BitConverter.DoubleToUInt64Bits(value));
}
