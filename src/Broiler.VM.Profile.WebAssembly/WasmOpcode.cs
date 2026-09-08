// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   1
// Annotated:        1/1
// Exempt:           172
// Human-reviewed:   0/1
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       1
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The instruction bytes this profile's MVP surface names.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is a vocabulary rather than a class of constants, and that is a review decision the
/// JavaScript profile's diagnostic registry took first.</b> A closed vocabulary is one reviewable
/// thing: the declaration carries the assessment and every member is inside its fingerprint. A
/// class of a hundred and seventy <c>const byte</c> fields would be a hundred and seventy
/// separately assessed fixed values, each demanding its own annotation, which is a worse record of
/// the same fact.
/// </para>
/// <para>
/// <b>Naming an opcode here is not admitting it.</b> The decoder retains a function body as bytes
/// rather than reading it instruction by instruction; <see cref="WasmValidator"/> is the pass that
/// walks a body and <see cref="WasmInterpreter"/> the one that executes it. What this enum fixes is
/// the byte-to-name mapping both use, so that the mapping is reviewed once rather than inline in
/// each of the two places that needs it.
/// <i>(Corrected 2026-09-08. This paragraph read "Nothing in this build executes an instruction,
/// and ... validation is the next part of this work rather than a part of it. What this enum fixes
/// now is the byte-to-name mapping that pass will use". Both clauses were true when written and
/// went stale when the validator and then the interpreter landed beside this enum; the superseded
/// reading is quoted rather than deleted so the chain stays readable.)</i>
/// </para>
/// <para>
/// <b>What is deliberately absent.</b> Every instruction behind the <c>0xFC</c> prefix (bulk memory
/// and saturating truncation), the <c>0xFD</c> prefix (vectors), the <c>0xFE</c> prefix (atomics),
/// the reference instructions at <c>0xD0</c> to <c>0xD2</c> and the sign-extension operators at
/// <c>0xC0</c> to <c>0xC4</c> are outside this surface. A byte carrying one of them decodes to no
/// member here, which is how a validator will refuse it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=15689B
// Broiler-Human:        PENDING
internal enum WasmOpcode : byte
{
    /// <summary>Traps unconditionally.</summary>
    Unreachable = 0x00,

    /// <summary>Does nothing.</summary>
    Nop = 0x01,

    /// <summary>Opens a block whose label targets its end.</summary>
    Block = 0x02,

    /// <summary>Opens a block whose label targets its start.</summary>
    Loop = 0x03,

    /// <summary>Opens a conditional block.</summary>
    If = 0x04,

    /// <summary>Opens the alternative arm of a conditional block.</summary>
    Else = 0x05,

    /// <summary>Closes a block or an expression.</summary>
    End = 0x0B,

    /// <summary>Branches unconditionally to a label.</summary>
    Br = 0x0C,

    /// <summary>Branches to a label when the top of the stack is non-zero.</summary>
    BrIf = 0x0D,

    /// <summary>Branches through a guest-supplied label vector.</summary>
    BrTable = 0x0E,

    /// <summary>Returns from the enclosing function.</summary>
    Return = 0x0F,

    /// <summary>Calls a function by index.</summary>
    Call = 0x10,

    /// <summary>Calls a function through a table entry, checking its signature.</summary>
    CallIndirect = 0x11,

    /// <summary>Discards the top of the stack.</summary>
    Drop = 0x1A,

    /// <summary>Chooses between two operands.</summary>
    Select = 0x1B,

    /// <summary>Pushes a local.</summary>
    LocalGet = 0x20,

    /// <summary>Pops into a local.</summary>
    LocalSet = 0x21,

    /// <summary>Stores into a local and leaves the value on the stack.</summary>
    LocalTee = 0x22,

    /// <summary>Pushes a global.</summary>
    GlobalGet = 0x23,

    /// <summary>Pops into a mutable global.</summary>
    GlobalSet = 0x24,

    /// <summary>Loads four bytes as an i32.</summary>
    I32Load = 0x28,

    /// <summary>Loads eight bytes as an i64.</summary>
    I64Load = 0x29,

    /// <summary>Loads four bytes as an f32.</summary>
    F32Load = 0x2A,

    /// <summary>Loads eight bytes as an f64.</summary>
    F64Load = 0x2B,

    /// <summary>Loads one byte, sign-extended into an i32.</summary>
    I32Load8S = 0x2C,

    /// <summary>Loads one byte, zero-extended into an i32.</summary>
    I32Load8U = 0x2D,

    /// <summary>Loads two bytes, sign-extended into an i32.</summary>
    I32Load16S = 0x2E,

    /// <summary>Loads two bytes, zero-extended into an i32.</summary>
    I32Load16U = 0x2F,

    /// <summary>Loads one byte, sign-extended into an i64.</summary>
    I64Load8S = 0x30,

    /// <summary>Loads one byte, zero-extended into an i64.</summary>
    I64Load8U = 0x31,

    /// <summary>Loads two bytes, sign-extended into an i64.</summary>
    I64Load16S = 0x32,

    /// <summary>Loads two bytes, zero-extended into an i64.</summary>
    I64Load16U = 0x33,

    /// <summary>Loads four bytes, sign-extended into an i64.</summary>
    I64Load32S = 0x34,

    /// <summary>Loads four bytes, zero-extended into an i64.</summary>
    I64Load32U = 0x35,

    /// <summary>Stores an i32 as four bytes.</summary>
    I32Store = 0x36,

    /// <summary>Stores an i64 as eight bytes.</summary>
    I64Store = 0x37,

    /// <summary>Stores an f32 as four bytes.</summary>
    F32Store = 0x38,

    /// <summary>Stores an f64 as eight bytes.</summary>
    F64Store = 0x39,

    /// <summary>Stores the low byte of an i32.</summary>
    I32Store8 = 0x3A,

    /// <summary>Stores the low two bytes of an i32.</summary>
    I32Store16 = 0x3B,

    /// <summary>Stores the low byte of an i64.</summary>
    I64Store8 = 0x3C,

    /// <summary>Stores the low two bytes of an i64.</summary>
    I64Store16 = 0x3D,

    /// <summary>Stores the low four bytes of an i64.</summary>
    I64Store32 = 0x3E,

    /// <summary>Pushes the current page count of the memory.</summary>
    MemorySize = 0x3F,

    /// <summary>Grows the memory, answering the old page count or minus one.</summary>
    MemoryGrow = 0x40,

    /// <summary>Pushes a signed 32-bit constant.</summary>
    I32Const = 0x41,

    /// <summary>Pushes a signed 64-bit constant.</summary>
    I64Const = 0x42,

    /// <summary>Pushes a 32-bit float constant, encoded as four raw little-endian bytes.</summary>
    F32Const = 0x43,

    /// <summary>Pushes a 64-bit float constant, encoded as eight raw little-endian bytes.</summary>
    F64Const = 0x44,

    /// <summary>Tests an i32 for zero.</summary>
    I32Eqz = 0x45,

    /// <summary>Compares two i32 values for equality.</summary>
    I32Eq = 0x46,

    /// <summary>Compares two i32 values for inequality.</summary>
    I32Ne = 0x47,

    /// <summary>Signed i32 less-than.</summary>
    I32LtS = 0x48,

    /// <summary>Unsigned i32 less-than.</summary>
    I32LtU = 0x49,

    /// <summary>Signed i32 greater-than.</summary>
    I32GtS = 0x4A,

    /// <summary>Unsigned i32 greater-than.</summary>
    I32GtU = 0x4B,

    /// <summary>Signed i32 less-or-equal.</summary>
    I32LeS = 0x4C,

    /// <summary>Unsigned i32 less-or-equal.</summary>
    I32LeU = 0x4D,

    /// <summary>Signed i32 greater-or-equal.</summary>
    I32GeS = 0x4E,

    /// <summary>Unsigned i32 greater-or-equal.</summary>
    I32GeU = 0x4F,

    /// <summary>Tests an i64 for zero.</summary>
    I64Eqz = 0x50,

    /// <summary>Compares two i64 values for equality.</summary>
    I64Eq = 0x51,

    /// <summary>Compares two i64 values for inequality.</summary>
    I64Ne = 0x52,

    /// <summary>Signed i64 less-than.</summary>
    I64LtS = 0x53,

    /// <summary>Unsigned i64 less-than.</summary>
    I64LtU = 0x54,

    /// <summary>Signed i64 greater-than.</summary>
    I64GtS = 0x55,

    /// <summary>Unsigned i64 greater-than.</summary>
    I64GtU = 0x56,

    /// <summary>Signed i64 less-or-equal.</summary>
    I64LeS = 0x57,

    /// <summary>Unsigned i64 less-or-equal.</summary>
    I64LeU = 0x58,

    /// <summary>Signed i64 greater-or-equal.</summary>
    I64GeS = 0x59,

    /// <summary>Unsigned i64 greater-or-equal.</summary>
    I64GeU = 0x5A,

    /// <summary>f32 equality.</summary>
    F32Eq = 0x5B,

    /// <summary>f32 inequality.</summary>
    F32Ne = 0x5C,

    /// <summary>f32 less-than.</summary>
    F32Lt = 0x5D,

    /// <summary>f32 greater-than.</summary>
    F32Gt = 0x5E,

    /// <summary>f32 less-or-equal.</summary>
    F32Le = 0x5F,

    /// <summary>f32 greater-or-equal.</summary>
    F32Ge = 0x60,

    /// <summary>f64 equality.</summary>
    F64Eq = 0x61,

    /// <summary>f64 inequality.</summary>
    F64Ne = 0x62,

    /// <summary>f64 less-than.</summary>
    F64Lt = 0x63,

    /// <summary>f64 greater-than.</summary>
    F64Gt = 0x64,

    /// <summary>f64 less-or-equal.</summary>
    F64Le = 0x65,

    /// <summary>f64 greater-or-equal.</summary>
    F64Ge = 0x66,

    /// <summary>Counts leading zeros in an i32.</summary>
    I32Clz = 0x67,

    /// <summary>Counts trailing zeros in an i32.</summary>
    I32Ctz = 0x68,

    /// <summary>Counts set bits in an i32.</summary>
    I32Popcnt = 0x69,

    /// <summary>i32 addition.</summary>
    I32Add = 0x6A,

    /// <summary>i32 subtraction.</summary>
    I32Sub = 0x6B,

    /// <summary>i32 multiplication.</summary>
    I32Mul = 0x6C,

    /// <summary>Signed i32 division, which traps on zero and on the one overflow case.</summary>
    I32DivS = 0x6D,

    /// <summary>Unsigned i32 division, which traps on zero.</summary>
    I32DivU = 0x6E,

    /// <summary>Signed i32 remainder, which traps on zero.</summary>
    I32RemS = 0x6F,

    /// <summary>Unsigned i32 remainder, which traps on zero.</summary>
    I32RemU = 0x70,

    /// <summary>i32 bitwise and.</summary>
    I32And = 0x71,

    /// <summary>i32 bitwise or.</summary>
    I32Or = 0x72,

    /// <summary>i32 bitwise exclusive or.</summary>
    I32Xor = 0x73,

    /// <summary>i32 shift left.</summary>
    I32Shl = 0x74,

    /// <summary>i32 arithmetic shift right.</summary>
    I32ShrS = 0x75,

    /// <summary>i32 logical shift right.</summary>
    I32ShrU = 0x76,

    /// <summary>i32 rotate left.</summary>
    I32Rotl = 0x77,

    /// <summary>i32 rotate right.</summary>
    I32Rotr = 0x78,

    /// <summary>Counts leading zeros in an i64.</summary>
    I64Clz = 0x79,

    /// <summary>Counts trailing zeros in an i64.</summary>
    I64Ctz = 0x7A,

    /// <summary>Counts set bits in an i64.</summary>
    I64Popcnt = 0x7B,

    /// <summary>i64 addition.</summary>
    I64Add = 0x7C,

    /// <summary>i64 subtraction.</summary>
    I64Sub = 0x7D,

    /// <summary>i64 multiplication.</summary>
    I64Mul = 0x7E,

    /// <summary>Signed i64 division, which traps on zero and on the one overflow case.</summary>
    I64DivS = 0x7F,

    /// <summary>Unsigned i64 division, which traps on zero.</summary>
    I64DivU = 0x80,

    /// <summary>Signed i64 remainder, which traps on zero.</summary>
    I64RemS = 0x81,

    /// <summary>Unsigned i64 remainder, which traps on zero.</summary>
    I64RemU = 0x82,

    /// <summary>i64 bitwise and.</summary>
    I64And = 0x83,

    /// <summary>i64 bitwise or.</summary>
    I64Or = 0x84,

    /// <summary>i64 bitwise exclusive or.</summary>
    I64Xor = 0x85,

    /// <summary>i64 shift left.</summary>
    I64Shl = 0x86,

    /// <summary>i64 arithmetic shift right.</summary>
    I64ShrS = 0x87,

    /// <summary>i64 logical shift right.</summary>
    I64ShrU = 0x88,

    /// <summary>i64 rotate left.</summary>
    I64Rotl = 0x89,

    /// <summary>i64 rotate right.</summary>
    I64Rotr = 0x8A,

    /// <summary>f32 absolute value.</summary>
    F32Abs = 0x8B,

    /// <summary>f32 negation.</summary>
    F32Neg = 0x8C,

    /// <summary>f32 round towards positive infinity.</summary>
    F32Ceil = 0x8D,

    /// <summary>f32 round towards negative infinity.</summary>
    F32Floor = 0x8E,

    /// <summary>f32 round towards zero.</summary>
    F32Trunc = 0x8F,

    /// <summary>f32 round to nearest, ties to even.</summary>
    F32Nearest = 0x90,

    /// <summary>f32 square root.</summary>
    F32Sqrt = 0x91,

    /// <summary>f32 addition.</summary>
    F32Add = 0x92,

    /// <summary>f32 subtraction.</summary>
    F32Sub = 0x93,

    /// <summary>f32 multiplication.</summary>
    F32Mul = 0x94,

    /// <summary>f32 division.</summary>
    F32Div = 0x95,

    /// <summary>f32 minimum.</summary>
    F32Min = 0x96,

    /// <summary>f32 maximum.</summary>
    F32Max = 0x97,

    /// <summary>f32 sign transfer.</summary>
    F32Copysign = 0x98,

    /// <summary>f64 absolute value.</summary>
    F64Abs = 0x99,

    /// <summary>f64 negation.</summary>
    F64Neg = 0x9A,

    /// <summary>f64 round towards positive infinity.</summary>
    F64Ceil = 0x9B,

    /// <summary>f64 round towards negative infinity.</summary>
    F64Floor = 0x9C,

    /// <summary>f64 round towards zero.</summary>
    F64Trunc = 0x9D,

    /// <summary>f64 round to nearest, ties to even.</summary>
    F64Nearest = 0x9E,

    /// <summary>f64 square root.</summary>
    F64Sqrt = 0x9F,

    /// <summary>f64 addition.</summary>
    F64Add = 0xA0,

    /// <summary>f64 subtraction.</summary>
    F64Sub = 0xA1,

    /// <summary>f64 multiplication.</summary>
    F64Mul = 0xA2,

    /// <summary>f64 division.</summary>
    F64Div = 0xA3,

    /// <summary>f64 minimum.</summary>
    F64Min = 0xA4,

    /// <summary>f64 maximum.</summary>
    F64Max = 0xA5,

    /// <summary>f64 sign transfer.</summary>
    F64Copysign = 0xA6,

    /// <summary>Narrows an i64 to an i32.</summary>
    I32WrapI64 = 0xA7,

    /// <summary>Truncates an f32 to a signed i32, trapping on NaN and out of range.</summary>
    I32TruncF32S = 0xA8,

    /// <summary>Truncates an f32 to an unsigned i32, trapping on NaN and out of range.</summary>
    I32TruncF32U = 0xA9,

    /// <summary>Truncates an f64 to a signed i32, trapping on NaN and out of range.</summary>
    I32TruncF64S = 0xAA,

    /// <summary>Truncates an f64 to an unsigned i32, trapping on NaN and out of range.</summary>
    I32TruncF64U = 0xAB,

    /// <summary>Sign-extends an i32 to an i64.</summary>
    I64ExtendI32S = 0xAC,

    /// <summary>Zero-extends an i32 to an i64.</summary>
    I64ExtendI32U = 0xAD,

    /// <summary>Truncates an f32 to a signed i64, trapping on NaN and out of range.</summary>
    I64TruncF32S = 0xAE,

    /// <summary>Truncates an f32 to an unsigned i64, trapping on NaN and out of range.</summary>
    I64TruncF32U = 0xAF,

    /// <summary>Truncates an f64 to a signed i64, trapping on NaN and out of range.</summary>
    I64TruncF64S = 0xB0,

    /// <summary>Truncates an f64 to an unsigned i64, trapping on NaN and out of range.</summary>
    I64TruncF64U = 0xB1,

    /// <summary>Converts a signed i32 to an f32.</summary>
    F32ConvertI32S = 0xB2,

    /// <summary>Converts an unsigned i32 to an f32.</summary>
    F32ConvertI32U = 0xB3,

    /// <summary>Converts a signed i64 to an f32.</summary>
    F32ConvertI64S = 0xB4,

    /// <summary>Converts an unsigned i64 to an f32.</summary>
    F32ConvertI64U = 0xB5,

    /// <summary>Narrows an f64 to an f32.</summary>
    F32DemoteF64 = 0xB6,

    /// <summary>Converts a signed i32 to an f64.</summary>
    F64ConvertI32S = 0xB7,

    /// <summary>Converts an unsigned i32 to an f64.</summary>
    F64ConvertI32U = 0xB8,

    /// <summary>Converts a signed i64 to an f64.</summary>
    F64ConvertI64S = 0xB9,

    /// <summary>Converts an unsigned i64 to an f64.</summary>
    F64ConvertI64U = 0xBA,

    /// <summary>Widens an f32 to an f64.</summary>
    F64PromoteF32 = 0xBB,

    /// <summary>Reads an f32's bits as an i32.</summary>
    I32ReinterpretF32 = 0xBC,

    /// <summary>Reads an f64's bits as an i64.</summary>
    I64ReinterpretF64 = 0xBD,

    /// <summary>Reads an i32's bits as an f32.</summary>
    F32ReinterpretI32 = 0xBE,

    /// <summary>Reads an i64's bits as an f64.</summary>
    F64ReinterpretI64 = 0xBF,
}
