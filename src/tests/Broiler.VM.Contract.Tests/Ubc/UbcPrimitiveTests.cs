using Broiler.VM.Ubc;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// Appendix D's reference evaluation at its edges, which no corpus artifact reaches because nothing
/// executes at UBC-1: every class of NaN, both signed zeros, the overflowing and trapping inputs, the
/// double-rounding conversions, the region bounds, and the canonicalisation flag.
/// </summary>
public sealed class UbcPrimitiveTests
{
    private const uint CanonicalNaN32 = 0x7FC0_0000;
    private const ulong CanonicalNaN64 = 0x7FF8_0000_0000_0000;

    /// <summary>A binary32 NaN of every class: quiet positive, quiet negative, signalling, and quiet with a payload.</summary>
    public static readonly TheoryData<uint> NaNs32 = new() { 0x7FC0_0000, 0xFFC0_0000, 0x7F80_0001, 0xFFA0_0005, 0x7FC1_2345 };

    /// <summary>A binary64 NaN of every class.</summary>
    public static readonly TheoryData<ulong> NaNs64 = new()
    {
        0x7FF8_0000_0000_0000, 0xFFF8_0000_0000_0000, 0x7FF0_0000_0000_0001, 0xFFF4_0000_0000_0005, 0x7FF8_1234_5678_9ABC,
    };

    private static readonly UbcPrimitive[] Arithmetic32 =
    [
        UbcPrimitive.F32Add, UbcPrimitive.F32Sub, UbcPrimitive.F32Mul, UbcPrimitive.F32Div, UbcPrimitive.F32Min, UbcPrimitive.F32Max,
        UbcPrimitive.F32Sqrt, UbcPrimitive.F32Ceil, UbcPrimitive.F32Floor, UbcPrimitive.F32Trunc, UbcPrimitive.F32Nearest,
    ];

    private static readonly UbcPrimitive[] Arithmetic64 =
    [
        UbcPrimitive.F64Add, UbcPrimitive.F64Sub, UbcPrimitive.F64Mul, UbcPrimitive.F64Div, UbcPrimitive.F64Min, UbcPrimitive.F64Max,
        UbcPrimitive.F64Sqrt, UbcPrimitive.F64Ceil, UbcPrimitive.F64Floor, UbcPrimitive.F64Trunc, UbcPrimitive.F64Nearest,
    ];

    [Theory]
    [MemberData(nameof(NaNs32))]
    public void Every_Binary32_Arithmetic_Result_Of_A_NaN_Is_A_NaN_And_Canonical_Under_The_Flag(uint nan)
    {
        foreach (var primitive in Arithmetic32)
        {
            foreach (var (a, b) in Operands(primitive, nan, 0x3F80_0000u))
            {
                var loose = UbcPrimitives.Evaluate(primitive, a, b, canonicaliseNaN: false);
                var canonical = UbcPrimitives.Evaluate(primitive, a, b, canonicaliseNaN: true);

                Assert.Equal(UbcTrapCode.None, loose.Trap);
                Assert.True(float.IsNaN(BitConverter.UInt32BitsToSingle((uint)loose.Bits)), $"{primitive} of a NaN is not a NaN");
                Assert.Equal(0UL, loose.Bits >> 32);
                Assert.Equal(CanonicalNaN32, canonical.Bits);
            }
        }
    }

    [Theory]
    [MemberData(nameof(NaNs64))]
    public void Every_Binary64_Arithmetic_Result_Of_A_NaN_Is_A_NaN_And_Canonical_Under_The_Flag(ulong nan)
    {
        const ulong one = 0x3FF0_0000_0000_0000;

        foreach (var primitive in Arithmetic64)
        {
            foreach (var (a, b) in Operands(primitive, nan, one))
            {
                var loose = UbcPrimitives.Evaluate(primitive, a, b, canonicaliseNaN: false);
                var canonical = UbcPrimitives.Evaluate(primitive, a, b, canonicaliseNaN: true);

                Assert.True(double.IsNaN(BitConverter.UInt64BitsToDouble(loose.Bits)), $"{primitive} of a NaN is not a NaN");
                Assert.Equal(CanonicalNaN64, canonical.Bits);
            }
        }
    }

    [Theory]
    [MemberData(nameof(NaNs32))]
    public void The_Sign_Operations_Never_Canonicalise_A_Binary32_NaN(uint nan)
    {
        // abs, neg and copysign are bit operations, not arithmetic: the flag leaves the payload alone.
        Assert.Equal(nan & 0x7FFF_FFFF, UbcPrimitives.Evaluate(UbcPrimitive.F32Abs, nan, 0, canonicaliseNaN: true).Bits);
        Assert.Equal(nan ^ 0x8000_0000, UbcPrimitives.Evaluate(UbcPrimitive.F32Neg, nan, 0, canonicaliseNaN: true).Bits);
        Assert.Equal(nan | 0x8000_0000, UbcPrimitives.Evaluate(UbcPrimitive.F32Copysign, nan, 0xBF80_0000, canonicaliseNaN: true).Bits);
        Assert.Equal(nan, UbcPrimitives.Evaluate(UbcPrimitive.I32ReinterpretF32, nan, 0, canonicaliseNaN: true).Bits);
        Assert.Equal(nan, UbcPrimitives.Evaluate(UbcPrimitive.F32ReinterpretI32, nan, 0, canonicaliseNaN: true).Bits);
    }

    [Theory]
    [MemberData(nameof(NaNs64))]
    public void The_Sign_Operations_Never_Canonicalise_A_Binary64_NaN(ulong nan)
    {
        Assert.Equal(nan & 0x7FFF_FFFF_FFFF_FFFF, UbcPrimitives.Evaluate(UbcPrimitive.F64Abs, nan, 0, canonicaliseNaN: true).Bits);
        Assert.Equal(nan ^ 0x8000_0000_0000_0000, UbcPrimitives.Evaluate(UbcPrimitive.F64Neg, nan, 0, canonicaliseNaN: true).Bits);
        Assert.Equal(nan & 0x7FFF_FFFF_FFFF_FFFF, UbcPrimitives.Evaluate(UbcPrimitive.F64Copysign, nan, 0, canonicaliseNaN: true).Bits);
        Assert.Equal(nan, UbcPrimitives.Evaluate(UbcPrimitive.I64ReinterpretF64, nan, 0, canonicaliseNaN: true).Bits);
    }

    [Theory]
    [MemberData(nameof(NaNs32))]
    public void Every_Comparison_With_A_Binary32_NaN_Is_Unordered(uint nan)
    {
        const uint one = 0x3F80_0000;

        foreach (var (a, b) in new[] { (nan, one), (one, nan), (nan, nan) })
        {
            Assert.Equal(0UL, Evaluate(UbcPrimitive.F32Eq, a, b));
            Assert.Equal(1UL, Evaluate(UbcPrimitive.F32Ne, a, b));
            Assert.Equal(0UL, Evaluate(UbcPrimitive.F32Lt, a, b));
            Assert.Equal(0UL, Evaluate(UbcPrimitive.F32Gt, a, b));
            Assert.Equal(0UL, Evaluate(UbcPrimitive.F32Le, a, b));
            Assert.Equal(0UL, Evaluate(UbcPrimitive.F32Ge, a, b));
        }
    }

    [Theory]
    [MemberData(nameof(NaNs64))]
    public void Every_Comparison_With_A_Binary64_NaN_Is_Unordered_And_Every_Conversion_Of_It_Traps(ulong nan)
    {
        Assert.Equal(0UL, Evaluate(UbcPrimitive.F64Eq, nan, nan));
        Assert.Equal(1UL, Evaluate(UbcPrimitive.F64Ne, nan, nan));
        Assert.Equal(0UL, Evaluate(UbcPrimitive.F64Ge, nan, 0));

        foreach (var primitive in new[] { UbcPrimitive.I32TruncF64S, UbcPrimitive.I32TruncF64U, UbcPrimitive.I64TruncF64S, UbcPrimitive.I64TruncF64U })
        {
            Assert.Equal(UbcTrapCode.InvalidConversion, UbcPrimitives.Evaluate(primitive, nan, 0, false).Trap);
        }
    }

    [Theory]
    [MemberData(nameof(NaNs32))]
    public void A_Binary32_NaN_Converts_To_A_NaN_Of_Either_Width_And_Canonically_Under_The_Flag(uint nan)
    {
        var promoted = UbcPrimitives.Evaluate(UbcPrimitive.F64PromoteF32, nan, 0, false);
        Assert.True(double.IsNaN(BitConverter.UInt64BitsToDouble(promoted.Bits)));
        Assert.Equal(CanonicalNaN64, UbcPrimitives.Evaluate(UbcPrimitive.F64PromoteF32, nan, 0, true).Bits);

        foreach (var primitive in new[] { UbcPrimitive.I32TruncF32S, UbcPrimitive.I32TruncF32U, UbcPrimitive.I64TruncF32S, UbcPrimitive.I64TruncF32U })
        {
            Assert.Equal(UbcTrapCode.InvalidConversion, UbcPrimitives.Evaluate(primitive, nan, 0, true).Trap);
        }
    }

    [Theory]
    [MemberData(nameof(NaNs64))]
    public void A_Binary64_NaN_Demotes_To_A_NaN_And_Canonically_Under_The_Flag(ulong nan)
    {
        var demoted = UbcPrimitives.Evaluate(UbcPrimitive.F32DemoteF64, nan, 0, false);
        Assert.True(float.IsNaN(BitConverter.UInt32BitsToSingle((uint)demoted.Bits)));
        Assert.Equal(0UL, demoted.Bits >> 32);
        Assert.Equal((ulong)CanonicalNaN32, UbcPrimitives.Evaluate(UbcPrimitive.F32DemoteF64, nan, 0, true).Bits);
    }

    [Fact]
    public void The_Flag_Leaves_Every_Result_That_Is_Not_A_NaN_Alone()
    {
        Assert.Equal(Bits(3.0f), UbcPrimitives.Evaluate(UbcPrimitive.F32Add, Bits(1.0f), Bits(2.0f), true).Bits);
        Assert.Equal(Bits(double.PositiveInfinity), UbcPrimitives.Evaluate(UbcPrimitive.F64Div, Bits(1.0), Bits(0.0), true).Bits);
        Assert.Equal(Bits(-0.0), UbcPrimitives.Evaluate(UbcPrimitive.F64Mul, Bits(-1.0), Bits(0.0), true).Bits);
        Assert.Equal(1UL, UbcPrimitives.Evaluate(UbcPrimitive.I32Add, 0, 1, true).Bits);
    }

    [Fact]
    public void Min_And_Max_Order_The_Signed_Zeros_Both_Ways_Round()
    {
        foreach (var (a, b) in new[] { (Bits(-0.0f), Bits(0.0f)), (Bits(0.0f), Bits(-0.0f)) })
        {
            Assert.Equal(Bits(-0.0f), Evaluate(UbcPrimitive.F32Min, a, b));
            Assert.Equal(Bits(0.0f), Evaluate(UbcPrimitive.F32Max, a, b));
        }

        foreach (var (a, b) in new[] { (Bits(-0.0), Bits(0.0)), (Bits(0.0), Bits(-0.0)) })
        {
            Assert.Equal(Bits(-0.0), Evaluate(UbcPrimitive.F64Min, a, b));
            Assert.Equal(Bits(0.0), Evaluate(UbcPrimitive.F64Max, a, b));
        }

        Assert.Equal(Bits(-0.0f), Evaluate(UbcPrimitive.F32Min, Bits(-0.0f), Bits(-0.0f)));
        Assert.Equal(Bits(0.0), Evaluate(UbcPrimitive.F64Max, Bits(0.0), Bits(0.0)));
    }

    [Fact]
    public void The_Signed_Zeros_Survive_Every_Operation_That_Should_Keep_Them()
    {
        Assert.Equal(Bits(-0.0f), Evaluate(UbcPrimitive.F32Nearest, Bits(-0.5f), 0));
        Assert.Equal(Bits(-0.0f), Evaluate(UbcPrimitive.F32Ceil, Bits(-0.5f), 0));
        Assert.Equal(Bits(-0.0f), Evaluate(UbcPrimitive.F32Trunc, Bits(-0.7f), 0));
        Assert.Equal(Bits(-0.0), Evaluate(UbcPrimitive.F64Nearest, Bits(-0.5), 0));
        Assert.Equal(Bits(-0.0), Evaluate(UbcPrimitive.F64Ceil, Bits(-0.25), 0));
        Assert.Equal(Bits(-0.0), Evaluate(UbcPrimitive.F64Floor, Bits(-0.0), 0));
        Assert.Equal(Bits(-0.0), Evaluate(UbcPrimitive.F64Sqrt, Bits(-0.0), 0));
        Assert.Equal(Bits(-0.0), Evaluate(UbcPrimitive.F64Add, Bits(-0.0), Bits(-0.0)));
        Assert.Equal(Bits(0.0), Evaluate(UbcPrimitive.F64Add, Bits(-0.0), Bits(0.0)));
        Assert.Equal(Bits(0.0), Evaluate(UbcPrimitive.F64Sub, Bits(0.0), Bits(0.0)));
        Assert.Equal(Bits(-0.0f), Evaluate(UbcPrimitive.F32Neg, Bits(0.0f), 0));
        Assert.Equal(Bits(0.0f), Evaluate(UbcPrimitive.F32Abs, Bits(-0.0f), 0));
        Assert.Equal(Bits(-1.0), Evaluate(UbcPrimitive.F64Copysign, Bits(1.0), Bits(-0.0)));
        Assert.Equal(Bits(-0.0), Evaluate(UbcPrimitive.F64PromoteF32, Bits(-0.0f), 0));
        Assert.Equal(Bits(-0.0f), Evaluate(UbcPrimitive.F32DemoteF64, Bits(-0.0), 0));

        // The two zeros are equal as numbers, and ordered only by min and max.
        Assert.Equal(1UL, Evaluate(UbcPrimitive.F64Eq, Bits(-0.0), Bits(0.0)));
        Assert.Equal(0UL, Evaluate(UbcPrimitive.F32Lt, Bits(-0.0f), Bits(0.0f)));

        // A truncation of a negative zero or of a small negative value is a zero, not a trap.
        Assert.Equal(0UL, Evaluate(UbcPrimitive.I32TruncF64U, Bits(-0.9), 0));
        Assert.Equal(0UL, Evaluate(UbcPrimitive.I64TruncF32U, Bits(-0.0f), 0));
        Assert.Equal(0UL, Evaluate(UbcPrimitive.I32TruncF32S, Bits(-0.0f), 0));
    }

    [Fact]
    public void Nearest_Rounds_Ties_To_Even()
    {
        Assert.Equal(Bits(2.0), Evaluate(UbcPrimitive.F64Nearest, Bits(2.5), 0));
        Assert.Equal(Bits(4.0), Evaluate(UbcPrimitive.F64Nearest, Bits(3.5), 0));
        Assert.Equal(Bits(-2.0f), Evaluate(UbcPrimitive.F32Nearest, Bits(-2.5f), 0));
        Assert.Equal(Bits(8388609.0f), Evaluate(UbcPrimitive.F32Nearest, Bits(8388609.0f), 0));
    }

    [Fact]
    public void Integer_Division_Traps_On_Zero_And_On_The_One_Overflowing_Quotient()
    {
        Assert.Equal(UbcTrapCode.DivideByZero, Trap(UbcPrimitive.I32DivS, 1, 0));
        Assert.Equal(UbcTrapCode.DivideByZero, Trap(UbcPrimitive.I32DivU, 1, 0));
        Assert.Equal(UbcTrapCode.DivideByZero, Trap(UbcPrimitive.I32RemS, 1, 0));
        Assert.Equal(UbcTrapCode.DivideByZero, Trap(UbcPrimitive.I32RemU, 1, 0));
        Assert.Equal(UbcTrapCode.DivideByZero, Trap(UbcPrimitive.I64DivS, 1, 0));
        Assert.Equal(UbcTrapCode.DivideByZero, Trap(UbcPrimitive.I64DivU, 1, 0));
        Assert.Equal(UbcTrapCode.DivideByZero, Trap(UbcPrimitive.I64RemS, 1, 0));
        Assert.Equal(UbcTrapCode.DivideByZero, Trap(UbcPrimitive.I64RemU, 1, 0));

        // The divisor is read at its width: an i32 of zero with high bits set is still zero.
        Assert.Equal(UbcTrapCode.DivideByZero, Trap(UbcPrimitive.I32DivS, 1, 0xFFFF_FFFF_0000_0000));

        Assert.Equal(UbcTrapCode.IntegerOverflow, Trap(UbcPrimitive.I32DivS, 0x8000_0000, 0xFFFF_FFFF));
        Assert.Equal(UbcTrapCode.IntegerOverflow, Trap(UbcPrimitive.I64DivS, 0x8000_0000_0000_0000, ulong.MaxValue));

        // The overflowing remainder answers zero, and the unsigned forms of the same bits divide.
        Assert.Equal(0UL, Evaluate(UbcPrimitive.I32RemS, 0x8000_0000, 0xFFFF_FFFF));
        Assert.Equal(0UL, Evaluate(UbcPrimitive.I64RemS, 0x8000_0000_0000_0000, ulong.MaxValue));
        Assert.Equal(0UL, Evaluate(UbcPrimitive.I32DivU, 0x8000_0000, 0xFFFF_FFFF));
        Assert.Equal(0x8000_0000UL, Evaluate(UbcPrimitive.I32RemU, 0x8000_0000, 0xFFFF_FFFF));

        // Truncation toward zero, and a remainder with the dividend's sign.
        Assert.Equal(0xFFFF_FFFEUL, Evaluate(UbcPrimitive.I32DivS, unchecked((uint)-7), 3));
        Assert.Equal(0xFFFF_FFFFUL, Evaluate(UbcPrimitive.I32RemS, unchecked((uint)-7), 3));
        Assert.Equal(1UL, Evaluate(UbcPrimitive.I32RemS, 7, unchecked((uint)-3)));
    }

    [Fact]
    public void Integer_Arithmetic_Wraps_Masks_Its_Counts_And_Keeps_To_Its_Width()
    {
        Assert.Equal(0UL, Evaluate(UbcPrimitive.I32Add, 0xFFFF_FFFF, 1));
        Assert.Equal(0xFFFF_FFFFUL, Evaluate(UbcPrimitive.I32Sub, 0, 1));
        Assert.Equal(0UL, Evaluate(UbcPrimitive.I32Mul, 0x1_0000, 0x1_0000));
        Assert.Equal(0x8000_0000_0000_0000UL, Evaluate(UbcPrimitive.I64Add, long.MaxValue, 1));
        Assert.Equal(2UL, Evaluate(UbcPrimitive.I32Shl, 1, 33));
        Assert.Equal(2UL, Evaluate(UbcPrimitive.I64Shl, 1, 65));
        Assert.Equal(0xFFFF_FFFFUL, Evaluate(UbcPrimitive.I32ShrS, 0x8000_0000, 31));
        Assert.Equal(1UL, Evaluate(UbcPrimitive.I32ShrU, 0x8000_0000, 63));
        Assert.Equal(0x8000_0001UL, Evaluate(UbcPrimitive.I32Rotr, 0x0000_0003, 1));
        Assert.Equal(1UL, Evaluate(UbcPrimitive.I64Rotl, 0x8000_0000_0000_0000, 1));
        Assert.Equal(32UL, Evaluate(UbcPrimitive.I32Clz, 0, 0));
        Assert.Equal(32UL, Evaluate(UbcPrimitive.I32Ctz, 0, 0));
        Assert.Equal(64UL, Evaluate(UbcPrimitive.I64Ctz, 0, 0));
        Assert.Equal(32UL, Evaluate(UbcPrimitive.I32Popcnt, 0xFFFF_FFFF_FFFF_FFFF, 0));
        Assert.Equal(1UL, Evaluate(UbcPrimitive.I32Eqz, 0xFFFF_FFFF_0000_0000, 0));
        Assert.Equal(1UL, Evaluate(UbcPrimitive.I32LtS, 0x8000_0000, 0));
        Assert.Equal(0UL, Evaluate(UbcPrimitive.I32LtU, 0x8000_0000, 0));
        Assert.Equal(0x0000_0000_FFFF_FFFFUL, Evaluate(UbcPrimitive.I32WrapI64, ulong.MaxValue, 0));
        Assert.Equal(ulong.MaxValue, Evaluate(UbcPrimitive.I64ExtendI32S, 0xFFFF_FFFF, 0));
        Assert.Equal(0xFFFF_FFFFUL, Evaluate(UbcPrimitive.I64ExtendI32U, 0xFFFF_FFFF, 0));
    }

    [Fact]
    public void Truncations_Trap_Exactly_Outside_Their_Range()
    {
        // i32 from f64: the values just inside survive truncation toward zero, the ones just outside trap.
        Assert.Equal(0x7FFF_FFFFUL, Evaluate(UbcPrimitive.I32TruncF64S, Bits(2147483647.9), 0));
        Assert.Equal(0x8000_0000UL, Evaluate(UbcPrimitive.I32TruncF64S, Bits(-2147483648.9), 0));
        Assert.Equal(UbcTrapCode.InvalidConversion, Trap(UbcPrimitive.I32TruncF64S, Bits(2147483648.0), 0));
        Assert.Equal(UbcTrapCode.InvalidConversion, Trap(UbcPrimitive.I32TruncF64S, Bits(-2147483649.0), 0));
        Assert.Equal(0xFFFF_FFFFUL, Evaluate(UbcPrimitive.I32TruncF64U, Bits(4294967295.9), 0));
        Assert.Equal(UbcTrapCode.InvalidConversion, Trap(UbcPrimitive.I32TruncF64U, Bits(4294967296.0), 0));
        Assert.Equal(UbcTrapCode.InvalidConversion, Trap(UbcPrimitive.I32TruncF64U, Bits(-1.0), 0));

        // i32 from f32: 2^31 is representable and out of range; -2^31 is in range.
        Assert.Equal(UbcTrapCode.InvalidConversion, Trap(UbcPrimitive.I32TruncF32S, Bits(2147483648.0f), 0));
        Assert.Equal(0x8000_0000UL, Evaluate(UbcPrimitive.I32TruncF32S, Bits(-2147483648.0f), 0));
        Assert.Equal(0xFFFF_FF00UL, Evaluate(UbcPrimitive.I32TruncF32U, Bits(4294967040.0f), 0));
        Assert.Equal(UbcTrapCode.InvalidConversion, Trap(UbcPrimitive.I32TruncF32U, Bits(4294967296.0f), 0));

        // i64: -2^63 is in range, 2^63 is not, and the largest binary64 below 2^64 converts unsigned.
        Assert.Equal(0x8000_0000_0000_0000UL, Evaluate(UbcPrimitive.I64TruncF64S, Bits(-9223372036854775808.0), 0));
        Assert.Equal(UbcTrapCode.InvalidConversion, Trap(UbcPrimitive.I64TruncF64S, Bits(9223372036854775808.0), 0));
        Assert.Equal(0xFFFF_FFFF_FFFF_F800UL, Evaluate(UbcPrimitive.I64TruncF64U, Bits(18446744073709549568.0), 0));
        Assert.Equal(UbcTrapCode.InvalidConversion, Trap(UbcPrimitive.I64TruncF64U, Bits(18446744073709551616.0), 0));
        Assert.Equal(UbcTrapCode.InvalidConversion, Trap(UbcPrimitive.I64TruncF32S, Bits(9223372036854775808.0f), 0));

        foreach (var infinity in new[] { double.PositiveInfinity, double.NegativeInfinity })
        {
            Assert.Equal(UbcTrapCode.InvalidConversion, Trap(UbcPrimitive.I64TruncF64S, Bits(infinity), 0));
            Assert.Equal(UbcTrapCode.InvalidConversion, Trap(UbcPrimitive.I32TruncF32U, Bits((float)infinity), 0));
        }
    }

    [Fact]
    public void Wide_Integers_Convert_To_Binary32_And_Binary64_With_One_Rounding()
    {
        // 2^60 + 2^36 + 1 lies just above a binary32 midpoint. Rounding to binary64 first drops the
        // low bit and lands exactly on the midpoint, which ties to even and rounds down: the wrong
        // answer. One rounding gives 2^60 + 2^37.
        const ulong aboveMidpoint = (1UL << 60) + (1UL << 36) + 1;
        var single = BitConverter.SingleToUInt32Bits((float)((1UL << 60) + (1UL << 37)));

        Assert.Equal(single, Evaluate(UbcPrimitive.F32ConvertI64U, aboveMidpoint, 0));
        Assert.Equal(single, Evaluate(UbcPrimitive.F32ConvertI64S, aboveMidpoint, 0));
        Assert.Equal(single | 0x8000_0000, Evaluate(UbcPrimitive.F32ConvertI64S, unchecked((ulong)-(long)aboveMidpoint), 0));

        // The same trap for binary64 above 2^63, where the signed conversion cannot be used directly.
        const ulong aboveDoubleMidpoint = (1UL << 63) + (1UL << 10) + 1;
        Assert.Equal(Bits((double)((1UL << 63) + (1UL << 11))), Evaluate(UbcPrimitive.F64ConvertI64U, aboveDoubleMidpoint, 0));

        Assert.Equal(Bits(18446744073709551616.0), Evaluate(UbcPrimitive.F64ConvertI64U, ulong.MaxValue, 0));
        Assert.Equal((ulong)BitConverter.SingleToUInt32Bits(18446744073709551616.0f), Evaluate(UbcPrimitive.F32ConvertI64U, ulong.MaxValue, 0));
        Assert.Equal((ulong)BitConverter.SingleToUInt32Bits(-9223372036854775808.0f), Evaluate(UbcPrimitive.F32ConvertI64S, 0x8000_0000_0000_0000, 0));
        Assert.Equal(Bits(-9223372036854775808.0), Evaluate(UbcPrimitive.F64ConvertI64S, 0x8000_0000_0000_0000, 0));
        Assert.Equal(Bits(4294967295.0), Evaluate(UbcPrimitive.F64ConvertI32U, 0xFFFF_FFFF, 0));
        Assert.Equal(Bits(-1.0), Evaluate(UbcPrimitive.F64ConvertI32S, 0xFFFF_FFFF, 0));
        Assert.Equal((ulong)BitConverter.SingleToUInt32Bits(16777216.0f), Evaluate(UbcPrimitive.F32ConvertI32S, 16777217, 0));
    }

    [Fact]
    public void Region_Accesses_Stay_Inside_The_Region_Or_Trap()
    {
        Span<byte> region = stackalloc byte[16];

        // The last in-bounds access and the first out-of-bounds one, for every width.
        foreach (var (primitive, width) in new[]
                 {
                     (UbcPrimitive.RegionLoadI32From8U, 1), (UbcPrimitive.RegionLoadI32From16U, 2),
                     (UbcPrimitive.RegionLoadI32, 4), (UbcPrimitive.RegionLoadI64, 8),
                 })
        {
            Assert.Equal(UbcTrapCode.None, UbcPrimitives.EvaluateRegion(primitive, region, (uint)(16 - width), 0, 0, 0).Trap);
            Assert.Equal(UbcTrapCode.OutOfBounds, UbcPrimitives.EvaluateRegion(primitive, region, (uint)(16 - width), 1, 0, 0).Trap);
            Assert.Equal(UbcTrapCode.OutOfBounds, UbcPrimitives.EvaluateRegion(primitive, region, (uint)(17 - width), 0, 0, 0).Trap);
        }

        // The effective address is computed without overflow: it does not wrap back into the region.
        Assert.Equal(UbcTrapCode.OutOfBounds, UbcPrimitives.EvaluateRegion(UbcPrimitive.RegionLoadI32From8U, region, uint.MaxValue, 1, 0, 0).Trap);
        Assert.Equal(UbcTrapCode.OutOfBounds, UbcPrimitives.EvaluateRegion(UbcPrimitive.RegionStoreI64, region, uint.MaxValue, uint.MaxValue, 0, 0).Trap);

        // An empty region admits no access at all, and a size query of any region answers its size.
        Assert.Equal(UbcTrapCode.OutOfBounds, UbcPrimitives.EvaluateRegion(UbcPrimitive.RegionLoadI32From8U, Span<byte>.Empty, 0, 0, 0, 0).Trap);
        Assert.Equal(3UL, UbcPrimitives.EvaluateRegion(UbcPrimitive.RegionSize, Span<byte>.Empty, 0, 0, 0, 3).Bits);

        // A store writes little-endian, unaligned, and only its width.
        region.Fill(0xEE);
        UbcPrimitives.EvaluateRegion(UbcPrimitive.RegionStoreI64To32, region, 3, 0, 0x1122_3344_5566_7788, 0);
        Assert.Equal(new byte[] { 0xEE, 0xEE, 0xEE, 0x88, 0x77, 0x66, 0x55, 0xEE }, region[..8].ToArray());

        // Loads sign- or zero-extend from their width.
        region[0] = 0x80;
        region[1] = 0xFF;
        Assert.Equal(0xFFFF_FF80UL, UbcPrimitives.EvaluateRegion(UbcPrimitive.RegionLoadI32From8S, region, 0, 0, 0, 0).Bits);
        Assert.Equal(0x80UL, UbcPrimitives.EvaluateRegion(UbcPrimitive.RegionLoadI32From8U, region, 0, 0, 0, 0).Bits);
        Assert.Equal(0xFFFF_FFFF_FFFF_FF80UL, UbcPrimitives.EvaluateRegion(UbcPrimitive.RegionLoadI64From16S, region, 0, 0, 0, 0).Bits);
        Assert.Equal(0xFF80UL, UbcPrimitives.EvaluateRegion(UbcPrimitive.RegionLoadI64From16U, region, 0, 0, 0, 0).Bits);
        Assert.Equal(0xFFFF_FFFF_88EE_FF80UL, UbcPrimitives.EvaluateRegion(UbcPrimitive.RegionLoadI64From32S, region, 0, 0, 0, 0).Bits);
        Assert.Equal(0x88EE_FF80UL, UbcPrimitives.EvaluateRegion(UbcPrimitive.RegionLoadI64From32U, region, 0, 0, 0, 0).Bits);
    }

    [Fact]
    public void A_Primitive_Evaluated_Through_The_Wrong_Entry_Point_Traps_Rather_Than_Answering()
    {
        Span<byte> region = stackalloc byte[8];

        Assert.Equal(UbcTrapCode.OutOfBounds, UbcPrimitives.EvaluateRegion(UbcPrimitive.I32Add, region, 0, 0, 0, 0).Trap);
        Assert.Equal(UbcTrapCode.OutOfBounds, UbcPrimitives.Evaluate(UbcPrimitive.RegionLoadI32, 0, 0, false).Trap);
        Assert.Equal(UbcTrapCode.OutOfBounds, UbcPrimitives.Evaluate((UbcPrimitive)9999, 0, 0, false).Trap);
    }

    [Fact]
    public void Every_Entry_Has_A_Signature_And_A_Trap_Set_That_Match_Its_Evaluation()
    {
        foreach (var primitive in Enum.GetValues<UbcPrimitive>())
        {
            Assert.True(UbcPrimitives.IsDefined(primitive));
            Assert.Equal(primitive != UbcPrimitive.WordKeep, UbcPrimitives.TryGetSignature(primitive, out _, out _));
            Assert.Equal(UbcPrimitives.AccessWidth(primitive) != 0, UbcPrimitives.IsRegionAccess(primitive) && primitive != UbcPrimitive.RegionSize);
            Assert.Equal(
                UbcPrimitives.AccessWidth(primitive) != 0,
                (UbcPrimitives.TrapsOf(primitive) & UbcTrapCode.OutOfBounds) != 0);
        }

        Assert.False(UbcPrimitives.IsDefined(0));
        Assert.False(UbcPrimitives.IsDefined(UbcPrimitive.WordKeep + 1));
        Assert.Equal(1234UL, Evaluate(UbcPrimitive.WordKeep, 1234, 99));
    }

    /// <summary>The operand pairs a NaN reaches a primitive through: either side of a binary one, the only side of a unary one.</summary>
    private static (ulong A, ulong B)[] Operands(UbcPrimitive primitive, ulong nan, ulong one)
    {
        UbcPrimitives.TryGetSignature(primitive, out var operands, out _);
        return operands.Length == 2 ? [(nan, one), (one, nan), (nan, nan)] : [(nan, 0)];
    }

    private static ulong Evaluate(UbcPrimitive primitive, ulong a, ulong b)
    {
        var result = UbcPrimitives.Evaluate(primitive, a, b, canonicaliseNaN: false);
        Assert.Equal(UbcTrapCode.None, result.Trap);
        return result.Bits;
    }

    private static UbcTrapCode Trap(UbcPrimitive primitive, ulong a, ulong b) =>
        UbcPrimitives.Evaluate(primitive, a, b, canonicaliseNaN: false).Trap;

    private static ulong Bits(float value) => BitConverter.SingleToUInt32Bits(value);

    private static ulong Bits(double value) => BitConverter.DoubleToUInt64Bits(value);
}
