// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           19
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    High
// Criteria:         2/1
// Resource impact:  0/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Ubc;

/// <summary>The type of one operand-stack position or one local: four machine words and one value.</summary>
/// <remarks>
/// The four numeric types are machine words an emitter holds in the word plane, in registers or in
/// native slots. <see cref="V"/> is a language value whose representation, lifetime and meaning belong
/// to the family that declares it: emitted code never holds one and never learns its layout, and the
/// interpreter keeps it in the family's value plane. The numbering is the byte the Types and Units
/// sections carry.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D74F3B
// Broiler-Human:        PENDING
public enum UbcSlotType : byte
{
    /// <summary>A thirty-two-bit two's-complement word.</summary>
    I32 = 1,

    /// <summary>A sixty-four-bit two's-complement word.</summary>
    I64 = 2,

    /// <summary>An IEEE-754 binary32 value, held as its bits.</summary>
    F32 = 3,

    /// <summary>An IEEE-754 binary64 value, held as its bits.</summary>
    F64 = 4,

    /// <summary>A language value, owned by the family.</summary>
    V = 5,
}

/// <summary>Which of the two planes a slot lives in.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A6F07D
// Broiler-Human:        PENDING
public enum UbcPlane : byte
{
    /// <summary>The word plane: every numeric slot.</summary>
    Word = 0,

    /// <summary>The value plane: every language-value slot.</summary>
    Value = 1,
}

/// <summary>Facts about slot types that every reader of the tables needs.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=DB7129
// Broiler-Human:        PENDING
public static class UbcSlotTypes
{
    /// <summary>True when <paramref name="value"/> is a slot type format version 1 defines.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E9C237
    // Broiler-Human:        PENDING
    public static bool IsDefined(byte value) => value is >= (byte)UbcSlotType.I32 and <= (byte)UbcSlotType.V;

    /// <summary>The plane a slot of <paramref name="type"/> lives in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E42CA8
    // Broiler-Human:        PENDING
    public static UbcPlane PlaneOf(UbcSlotType type) => type == UbcSlotType.V ? UbcPlane.Value : UbcPlane.Word;

    /// <summary>The mnemonic a disassembly writes for <paramref name="type"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=None; Resources=0; Fingerprint=550D10
    // Broiler-Human:        PENDING
    public static string Name(UbcSlotType type) => type switch
    {
        UbcSlotType.I32 => "i32",
        UbcSlotType.I64 => "i64",
        UbcSlotType.F32 => "f32",
        UbcSlotType.F64 => "f64",
        UbcSlotType.V => "v",
        _ => "?",
    };
}

/// <summary>The closed set of operand shapes. Each has a fixed width, and every field is little-endian.</summary>
/// <remarks>
/// A family may use any shape and may not mint one. Because every shape has a fixed width, an
/// instruction's boundary is computable from the tables alone, and no immediate is decoded before the
/// walk has admitted it.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E95991
// Broiler-Human:        PENDING
public enum UbcOperandShape : byte
{
    /// <summary>No operand.</summary>
    None = 0,

    /// <summary>One unsigned byte.</summary>
    U8 = 1,

    /// <summary>One unsigned sixteen-bit field.</summary>
    U16 = 2,

    /// <summary>One unsigned thirty-two-bit field.</summary>
    U32 = 3,

    /// <summary>One signed thirty-two-bit field.</summary>
    I32 = 4,

    /// <summary>One signed sixty-four-bit field.</summary>
    I64 = 5,

    /// <summary>The bits of one IEEE-754 binary32 value.</summary>
    F32 = 6,

    /// <summary>The bits of one IEEE-754 binary64 value.</summary>
    F64 = 7,

    /// <summary>Two unsigned bytes.</summary>
    U8U8 = 8,

    /// <summary>An unsigned byte, then an unsigned sixteen-bit field.</summary>
    U8U16 = 9,

    /// <summary>An unsigned byte, then an unsigned thirty-two-bit field.</summary>
    U8U32 = 10,

    /// <summary>Two unsigned sixteen-bit fields.</summary>
    U16U16 = 11,
}

/// <summary>Widths and field accessors of <see cref="UbcOperandShape"/>, the one place they are stated.</summary>
/// <remarks>
/// An operand is carried as the zero-extended little-endian value of its bytes, in one
/// <see cref="ulong"/>: every shape is at most eight bytes wide. The two-field shapes read their first
/// field from the low bytes and their second from the bytes after it.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9B12BF
// Broiler-Human:        PENDING
public static class UbcOperandShapes
{
    /// <summary>True when <paramref name="shape"/> is a member of the closed set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E84369
    // Broiler-Human:        PENDING
    public static bool IsDefined(UbcOperandShape shape) => shape <= UbcOperandShape.U16U16;

    /// <summary>The width of an operand of <paramref name="shape"/>, in bytes; minus one for a shape outside the set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D53D8D
    // Broiler-Falsified-If: a shape answers a width other than the bytes its fields occupy, so a boundary is computed wrong
    // Broiler-Human:        PENDING
    public static int Width(UbcOperandShape shape) => shape switch
    {
        UbcOperandShape.None => 0,
        UbcOperandShape.U8 => 1,
        UbcOperandShape.U16 => 2,
        UbcOperandShape.U32 => 4,
        UbcOperandShape.I32 => 4,
        UbcOperandShape.I64 => 8,
        UbcOperandShape.F32 => 4,
        UbcOperandShape.F64 => 8,
        UbcOperandShape.U8U8 => 2,
        UbcOperandShape.U8U16 => 3,
        UbcOperandShape.U8U32 => 5,
        UbcOperandShape.U16U16 => 4,
        _ => -1,
    };

    /// <summary>
    /// Reads an operand of <paramref name="shape"/> from <paramref name="bytes"/>, which must be exactly
    /// its width, as a zero-extended little-endian value.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=739347
    // Broiler-Falsified-If: a byte beyond the shape's width contributes to the value, or the byte order is not little-endian
    // Broiler-Human:        PENDING
    public static ulong Read(System.ReadOnlySpan<byte> bytes)
    {
        ulong value = 0;

        for (var index = bytes.Length - 1; index >= 0; index--)
        {
            value = (value << 8) | bytes[index];
        }

        return value;
    }

    /// <summary>Writes <paramref name="operand"/> as <paramref name="width"/> little-endian bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=10E7AD
    // Broiler-Human:        PENDING
    public static void Write(System.Span<byte> destination, ulong operand, int width)
    {
        for (var index = 0; index < width; index++)
        {
            destination[index] = (byte)(operand >> (8 * index));
        }
    }

    /// <summary>The first field of a two-field operand, or the whole operand of a one-field shape.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4976FF
    // Broiler-Human:        PENDING
    public static ulong First(UbcOperandShape shape, ulong operand) => shape switch
    {
        UbcOperandShape.U8U8 or UbcOperandShape.U8U16 or UbcOperandShape.U8U32 => operand & 0xFF,
        UbcOperandShape.U16U16 => operand & 0xFFFF,
        _ => operand,
    };

    /// <summary>The second field of a two-field operand, or zero for a one-field shape.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=282869
    // Broiler-Human:        PENDING
    public static ulong Second(UbcOperandShape shape, ulong operand) => shape switch
    {
        UbcOperandShape.U8U8 => (operand >> 8) & 0xFF,
        UbcOperandShape.U8U16 => (operand >> 8) & 0xFFFF,
        UbcOperandShape.U8U32 => (operand >> 8) & 0xFFFF_FFFF,
        UbcOperandShape.U16U16 => (operand >> 16) & 0xFFFF,
        _ => 0,
    };
}
