// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           0
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    Critical
// Criteria:         7/7
// Resource impact:  6/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The WebAssembly profile's own numeric, comparison and conversion arms: the reference handler of
/// the family's hundred and twenty-three numeric rows, and what the interpreter dispatches every such
/// instruction to.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE ARMS WERE MOVED HERE VERBATIM, AND THAT INCLUDES THEIR DEFECT.</b> They were the
/// interpreter's private members; they are this type's now, with the operand stack and its top handed
/// in rather than read from the interpreter's fields, and not one arm's body changed. The interpreter
/// delegates its numeric dispatch here, so the arms that produced the base run's answers are the arms
/// the universal bytecode's obligation E2 compares with the primitive table - and the routing defect
/// travels with them: <see cref="TryNumeric"/> sends 0x45 to 0x8A to the integer arm, which has no case
/// for the twelve float comparisons 0x5B to 0x66 and answers that it has no answer, and the comparison
/// arm below is never called. The negative control of milestone UBC-4 is that defect, retained failing
/// before the routing is corrected, and the correction is the lead's to make.
/// </para>
/// <para>
/// <b>Nothing here canonicalises a NaN</b>, and the two unsigned sixty-four-bit conversions are the C#
/// casts the interpreter always used. Whether those agree with the primitive table under the family's
/// NaN flag is what the comparison lane shows; this type says only what the profile's own arms answer.
/// </para>
/// <para>
/// <see cref="TryEvaluate"/> is the one entry from outside the interpreter: it runs one row's arm over
/// one or two operand words, which is how the family's handler answers a primitive row an emitter does
/// not execute itself and how <see cref="WebAssemblyProfile.TryEvaluateReference"/> hands the arms to a
/// composition root.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=2; Fingerprint=8A1B77
// Broiler-Falsified-If: an arm here answers differently from the interpreter arm it was moved from, or the reference evaluation reads an operand the row does not pop
// Broiler-Human:        PENDING
internal static class WasmReferenceNumerics
{
    /// <summary>
    /// Runs the arm of the numeric row <paramref name="opcode"/> over <paramref name="a"/>, the deeper
    /// operand, and <paramref name="b"/>, the top one, which a one-operand row does not read.
    /// </summary>
    /// <remarks>
    /// Answers true with the result's bits, or with the trap it raised in <paramref name="trap"/> and
    /// zero bits; <paramref name="trap"/> is zero, which names no trap, when none was raised. Answers
    /// false when the arms have no answer - the routing defect for 0x5B to 0x66 - and for a byte that
    /// is not a numeric row of the family's table.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=19B6AD
    // Broiler-Falsified-If: an answer is given for a byte that is no numeric row, or a row's arm is run over more operands than its effect pops
    // Broiler-Human:        PENDING
    internal static bool TryEvaluate(byte opcode, ulong a, ulong b, out ulong bits, out WasmTrapKind trap)
    {
        bits = 0;
        trap = 0;

        if (opcode is < WasmFamilyTable.FirstNumeric or > WasmFamilyTable.LastNumeric ||
            !WasmFamilyTable.Table.TryGetRow(opcode, out var row))
        {
            return false;
        }

        var arity = row.Effect.Pops.Length;
        var stack = new WasmValue[2];
        stack[0] = WasmValue.FromBits(a);
        stack[1] = WasmValue.FromBits(b);
        var top = arity;

        if (!TryNumeric(opcode, stack, ref top, out var raised))
        {
            return false;
        }

        if (raised is { } kind)
        {
            trap = kind;
            return true;
        }

        bits = stack[top - 1].Low;
        return true;
    }

    /// <summary>
    /// Executes one comparison, arithmetic or conversion instruction, or says it is not one.
    /// </summary>
    /// <remarks>
    /// The three groups are split by opcode range rather than by kind because that is how the format
    /// lays them out, and a reader checking this against the specification's own instruction table
    /// reads the two in the same order.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=9E674E
    // Broiler-Falsified-If: an opcode inside these ranges answers false, or one outside them answers true
    // Broiler-Human:        PENDING
    internal static bool TryNumeric(byte opcode, WasmValue[] stack, ref int top, out WasmTrapKind? trap)
    {
        trap = null;

        return opcode switch
        {
            >= 0x45 and <= 0x8A => Integer(opcode, stack, ref top, ref trap),
            >= 0x8B and <= 0xA6 => Float(opcode, stack, ref top),
            >= 0xA7 and <= 0xBF => Convert(opcode, stack, ref top, ref trap),
            _ => false,
        };
    }

    /// <summary>The integer comparisons and the integer arithmetic.</summary>
    /// <remarks>
    /// <b>THE TWO INTEGER TRAPS ARE HERE AND THEY ARE DISTINGUISHED.</b> A zero divisor is a divide
    /// by zero; the one signed division whose result does not fit is an overflow; and the same
    /// operands under a remainder are not an overflow at all - the specification says the remainder
    /// of the most negative value by minus one is zero, which is what a naive implementation gets
    /// wrong by trapping and what the CLR gets wrong by throwing.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=6; Fingerprint=B9E1D6
    // Broiler-Falsified-If: a signed remainder by minus one traps, or a shift count is used unmasked
    // Broiler-Human:        PENDING
    private static bool Integer(byte opcode, WasmValue[] stack, ref int top, ref WasmTrapKind? trap)
    {
        switch (opcode)
        {
            case 0x45:
                stack[top - 1] = WasmValue.FromI32(stack[top - 1].I32 == 0 ? 1 : 0);
                return true;

            case 0x50:
                stack[top - 1] = WasmValue.FromI32(stack[top - 1].I64 == 0 ? 1 : 0);
                return true;

            case 0x67:
                stack[top - 1] = WasmValue.FromI32(
                    System.Numerics.BitOperations.LeadingZeroCount((uint)stack[top - 1].I32));
                return true;

            case 0x68:
                stack[top - 1] = WasmValue.FromI32(
                    System.Numerics.BitOperations.TrailingZeroCount((uint)stack[top - 1].I32));
                return true;

            case 0x69:
                stack[top - 1] = WasmValue.FromI32(
                    System.Numerics.BitOperations.PopCount((uint)stack[top - 1].I32));
                return true;

            case 0x79:
                stack[top - 1] = WasmValue.FromI64(
                    System.Numerics.BitOperations.LeadingZeroCount((ulong)stack[top - 1].I64));
                return true;

            case 0x7A:
                stack[top - 1] = WasmValue.FromI64(
                    System.Numerics.BitOperations.TrailingZeroCount((ulong)stack[top - 1].I64));
                return true;

            case 0x7B:
                stack[top - 1] = WasmValue.FromI64(
                    System.Numerics.BitOperations.PopCount((ulong)stack[top - 1].I64));
                return true;

            default:
                break;
        }

        if (opcode is (>= 0x46 and <= 0x4F) or (>= 0x6A and <= 0x78))
        {
            var right = stack[--top].I32;
            var left = stack[--top].I32;

            switch (opcode)
            {
                case 0x46: stack[top++] = WasmValue.FromI32(left == right ? 1 : 0); return true;
                case 0x47: stack[top++] = WasmValue.FromI32(left != right ? 1 : 0); return true;
                case 0x48: stack[top++] = WasmValue.FromI32(left < right ? 1 : 0); return true;
                case 0x49: stack[top++] = WasmValue.FromI32((uint)left < (uint)right ? 1 : 0); return true;
                case 0x4A: stack[top++] = WasmValue.FromI32(left > right ? 1 : 0); return true;
                case 0x4B: stack[top++] = WasmValue.FromI32((uint)left > (uint)right ? 1 : 0); return true;
                case 0x4C: stack[top++] = WasmValue.FromI32(left <= right ? 1 : 0); return true;
                case 0x4D: stack[top++] = WasmValue.FromI32((uint)left <= (uint)right ? 1 : 0); return true;
                case 0x4E: stack[top++] = WasmValue.FromI32(left >= right ? 1 : 0); return true;
                case 0x4F: stack[top++] = WasmValue.FromI32((uint)left >= (uint)right ? 1 : 0); return true;
                case 0x6A: stack[top++] = WasmValue.FromI32(unchecked(left + right)); return true;
                case 0x6B: stack[top++] = WasmValue.FromI32(unchecked(left - right)); return true;
                case 0x6C: stack[top++] = WasmValue.FromI32(unchecked(left * right)); return true;

                case 0x6D:
                    if (right == 0)
                    {
                        trap = WasmTrapKind.IntegerDivideByZero;
                        return true;
                    }

                    if (left == int.MinValue && right == -1)
                    {
                        trap = WasmTrapKind.IntegerOverflow;
                        return true;
                    }

                    stack[top++] = WasmValue.FromI32(left / right);
                    return true;

                case 0x6E:
                    if (right == 0)
                    {
                        trap = WasmTrapKind.IntegerDivideByZero;
                        return true;
                    }

                    stack[top++] = WasmValue.FromI32((int)((uint)left / (uint)right));
                    return true;

                case 0x6F:
                    if (right == 0)
                    {
                        trap = WasmTrapKind.IntegerDivideByZero;
                        return true;
                    }

                    // The specification's answer here is zero, and the CLR's is an overflow
                    // exception, so the case is written out rather than left to the operator.
                    stack[top++] = WasmValue.FromI32(right == -1 ? 0 : left % right);
                    return true;

                case 0x70:
                    if (right == 0)
                    {
                        trap = WasmTrapKind.IntegerDivideByZero;
                        return true;
                    }

                    stack[top++] = WasmValue.FromI32((int)((uint)left % (uint)right));
                    return true;

                case 0x71: stack[top++] = WasmValue.FromI32(left & right); return true;
                case 0x72: stack[top++] = WasmValue.FromI32(left | right); return true;
                case 0x73: stack[top++] = WasmValue.FromI32(left ^ right); return true;
                case 0x74: stack[top++] = WasmValue.FromI32(left << (right & 31)); return true;
                case 0x75: stack[top++] = WasmValue.FromI32(left >> (right & 31)); return true;
                case 0x76: stack[top++] = WasmValue.FromI32((int)((uint)left >> (right & 31))); return true;

                case 0x77:
                    stack[top++] = WasmValue.FromI32(
                        (int)System.Numerics.BitOperations.RotateLeft((uint)left, right & 31));

                    return true;

                default:
                    stack[top++] = WasmValue.FromI32(
                        (int)System.Numerics.BitOperations.RotateRight((uint)left, right & 31));

                    return true;
            }
        }

        if (opcode is (>= 0x51 and <= 0x5A) or (>= 0x7C and <= 0x8A))
        {
            var right = stack[--top].I64;
            var left = stack[--top].I64;

            switch (opcode)
            {
                case 0x51: stack[top++] = WasmValue.FromI32(left == right ? 1 : 0); return true;
                case 0x52: stack[top++] = WasmValue.FromI32(left != right ? 1 : 0); return true;
                case 0x53: stack[top++] = WasmValue.FromI32(left < right ? 1 : 0); return true;
                case 0x54: stack[top++] = WasmValue.FromI32((ulong)left < (ulong)right ? 1 : 0); return true;
                case 0x55: stack[top++] = WasmValue.FromI32(left > right ? 1 : 0); return true;
                case 0x56: stack[top++] = WasmValue.FromI32((ulong)left > (ulong)right ? 1 : 0); return true;
                case 0x57: stack[top++] = WasmValue.FromI32(left <= right ? 1 : 0); return true;
                case 0x58: stack[top++] = WasmValue.FromI32((ulong)left <= (ulong)right ? 1 : 0); return true;
                case 0x59: stack[top++] = WasmValue.FromI32(left >= right ? 1 : 0); return true;
                case 0x5A: stack[top++] = WasmValue.FromI32((ulong)left >= (ulong)right ? 1 : 0); return true;
                case 0x7C: stack[top++] = WasmValue.FromI64(unchecked(left + right)); return true;
                case 0x7D: stack[top++] = WasmValue.FromI64(unchecked(left - right)); return true;
                case 0x7E: stack[top++] = WasmValue.FromI64(unchecked(left * right)); return true;

                case 0x7F:
                    if (right == 0)
                    {
                        trap = WasmTrapKind.IntegerDivideByZero;
                        return true;
                    }

                    if (left == long.MinValue && right == -1)
                    {
                        trap = WasmTrapKind.IntegerOverflow;
                        return true;
                    }

                    stack[top++] = WasmValue.FromI64(left / right);
                    return true;

                case 0x80:
                    if (right == 0)
                    {
                        trap = WasmTrapKind.IntegerDivideByZero;
                        return true;
                    }

                    stack[top++] = WasmValue.FromI64((long)((ulong)left / (ulong)right));
                    return true;

                case 0x81:
                    if (right == 0)
                    {
                        trap = WasmTrapKind.IntegerDivideByZero;
                        return true;
                    }

                    stack[top++] = WasmValue.FromI64(right == -1 ? 0 : left % right);
                    return true;

                case 0x82:
                    if (right == 0)
                    {
                        trap = WasmTrapKind.IntegerDivideByZero;
                        return true;
                    }

                    stack[top++] = WasmValue.FromI64((long)((ulong)left % (ulong)right));
                    return true;

                case 0x83: stack[top++] = WasmValue.FromI64(left & right); return true;
                case 0x84: stack[top++] = WasmValue.FromI64(left | right); return true;
                case 0x85: stack[top++] = WasmValue.FromI64(left ^ right); return true;
                case 0x86: stack[top++] = WasmValue.FromI64(left << (int)(right & 63)); return true;
                case 0x87: stack[top++] = WasmValue.FromI64(left >> (int)(right & 63)); return true;
                case 0x88: stack[top++] = WasmValue.FromI64((long)((ulong)left >> (int)(right & 63))); return true;

                case 0x89:
                    stack[top++] = WasmValue.FromI64(
                        (long)System.Numerics.BitOperations.RotateLeft((ulong)left, (int)(right & 63)));

                    return true;

                default:
                    stack[top++] = WasmValue.FromI64(
                        (long)System.Numerics.BitOperations.RotateRight((ulong)left, (int)(right & 63)));

                    return true;
            }
        }

        return false;
    }

    /// <summary>The floating-point arithmetic, none of which traps.</summary>
    /// <remarks>
    /// <b>NO FLOAT OPERATION HERE CAN TRAP, AND THAT IS THE SPECIFICATION'S RULE RATHER THAN THIS
    /// BUILD'S CONVENIENCE.</b> Division by zero is an infinity, zero over zero is a NaN, and the
    /// square root of a negative number is a NaN. The only place a float reaches a trap is the
    /// conversion group, where a value has to become an integer and may not fit.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=4; Fingerprint=A815A7
    // Broiler-Falsified-If: a float operation here raises a trap, or a minimum of a negative and a positive zero answers the positive one
    // Broiler-Human:        PENDING
    private static bool Float(byte opcode, WasmValue[] stack, ref int top)
    {
        switch (opcode)
        {
            case 0x8B: stack[top - 1] = WasmValue.FromF32(System.MathF.Abs(stack[top - 1].F32)); return true;
            case 0x8C: stack[top - 1] = WasmValue.FromF32(-stack[top - 1].F32); return true;
            case 0x8D: stack[top - 1] = WasmValue.FromF32(System.MathF.Ceiling(stack[top - 1].F32)); return true;
            case 0x8E: stack[top - 1] = WasmValue.FromF32(System.MathF.Floor(stack[top - 1].F32)); return true;
            case 0x8F: stack[top - 1] = WasmValue.FromF32(System.MathF.Truncate(stack[top - 1].F32)); return true;

            case 0x90:
                stack[top - 1] = WasmValue.FromF32(
                    System.MathF.Round(stack[top - 1].F32, System.MidpointRounding.ToEven));

                return true;

            case 0x91: stack[top - 1] = WasmValue.FromF32(System.MathF.Sqrt(stack[top - 1].F32)); return true;

            case 0x99: stack[top - 1] = WasmValue.FromF64(System.Math.Abs(stack[top - 1].F64)); return true;
            case 0x9A: stack[top - 1] = WasmValue.FromF64(-stack[top - 1].F64); return true;
            case 0x9B: stack[top - 1] = WasmValue.FromF64(System.Math.Ceiling(stack[top - 1].F64)); return true;
            case 0x9C: stack[top - 1] = WasmValue.FromF64(System.Math.Floor(stack[top - 1].F64)); return true;
            case 0x9D: stack[top - 1] = WasmValue.FromF64(System.Math.Truncate(stack[top - 1].F64)); return true;

            case 0x9E:
                stack[top - 1] = WasmValue.FromF64(
                    System.Math.Round(stack[top - 1].F64, System.MidpointRounding.ToEven));

                return true;

            case 0x9F: stack[top - 1] = WasmValue.FromF64(System.Math.Sqrt(stack[top - 1].F64)); return true;

            default:
                break;
        }

        if (opcode is >= 0x92 and <= 0x98)
        {
            var right = stack[--top].F32;
            var left = stack[--top].F32;

            stack[top++] = opcode switch
            {
                0x92 => WasmValue.FromF32(left + right),
                0x93 => WasmValue.FromF32(left - right),
                0x94 => WasmValue.FromF32(left * right),
                0x95 => WasmValue.FromF32(left / right),
                0x96 => WasmValue.FromF32(System.MathF.Min(left, right)),
                0x97 => WasmValue.FromF32(System.MathF.Max(left, right)),
                _ => WasmValue.FromF32(System.MathF.CopySign(left, right)),
            };

            return true;
        }

        if (opcode is >= 0xA0 and <= 0xA6)
        {
            var right = stack[--top].F64;
            var left = stack[--top].F64;

            stack[top++] = opcode switch
            {
                0xA0 => WasmValue.FromF64(left + right),
                0xA1 => WasmValue.FromF64(left - right),
                0xA2 => WasmValue.FromF64(left * right),
                0xA3 => WasmValue.FromF64(left / right),
                0xA4 => WasmValue.FromF64(System.Math.Min(left, right)),
                0xA5 => WasmValue.FromF64(System.Math.Max(left, right)),
                _ => WasmValue.FromF64(System.Math.CopySign(left, right)),
            };

            return true;
        }

        return false;
    }

    /// <summary>The float comparisons, which live below the arithmetic in the opcode table.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=C9F6CF
    // Broiler-Falsified-If: a comparison against a NaN answers anything but false, or an inequality against a NaN answers false
    // Broiler-Human:        PENDING
    private static bool FloatComparison(byte opcode, WasmValue[] stack, ref int top)
    {
        if (opcode is >= 0x5B and <= 0x60)
        {
            var right = stack[--top].F32;
            var left = stack[--top].F32;

            stack[top++] = WasmValue.FromI32(opcode switch
            {
                0x5B => left == right ? 1 : 0,
                0x5C => left != right ? 1 : 0,
                0x5D => left < right ? 1 : 0,
                0x5E => left > right ? 1 : 0,
                0x5F => left <= right ? 1 : 0,
                _ => left >= right ? 1 : 0,
            });

            return true;
        }

        if (opcode is >= 0x61 and <= 0x66)
        {
            var right = stack[--top].F64;
            var left = stack[--top].F64;

            stack[top++] = WasmValue.FromI32(opcode switch
            {
                0x61 => left == right ? 1 : 0,
                0x62 => left != right ? 1 : 0,
                0x63 => left < right ? 1 : 0,
                0x64 => left > right ? 1 : 0,
                0x65 => left <= right ? 1 : 0,
                _ => left >= right ? 1 : 0,
            });

            return true;
        }

        return false;
    }

    /// <summary>The conversions, and the only place a float can trap.</summary>
    /// <remarks>
    /// <b>THE TRUNCATION IS PERFORMED IN DOUBLE PRECISION AND THE RANGE IS COMPARED THERE.</b> A
    /// double represents every 32-bit integer exactly and every power of two up to and including two
    /// to the sixty-fourth exactly, so the four comparisons below are exact rather than nearly right
    /// - which the obvious spelling, comparing a float against a bound the float cannot represent, is
    /// not.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=5; Fingerprint=91820A
    // Broiler-Falsified-If: a value exactly on a bound is refused, or a value one unit past it is accepted
    // Broiler-Human:        PENDING
    private static bool Convert(byte opcode, WasmValue[] stack, ref int top, ref WasmTrapKind? trap)
    {
        switch (opcode)
        {
            case 0xA7:
                stack[top - 1] = WasmValue.FromI32((int)stack[top - 1].I64);
                return true;

            case 0xAC:
                stack[top - 1] = WasmValue.FromI64(stack[top - 1].I32);
                return true;

            case 0xAD:
                stack[top - 1] = WasmValue.FromI64((uint)stack[top - 1].I32);
                return true;

            case 0xB2: stack[top - 1] = WasmValue.FromF32(stack[top - 1].I32); return true;
            case 0xB3: stack[top - 1] = WasmValue.FromF32((uint)stack[top - 1].I32); return true;
            case 0xB4: stack[top - 1] = WasmValue.FromF32(stack[top - 1].I64); return true;
            case 0xB5: stack[top - 1] = WasmValue.FromF32((ulong)stack[top - 1].I64); return true;
            case 0xB6: stack[top - 1] = WasmValue.FromF32((float)stack[top - 1].F64); return true;
            case 0xB7: stack[top - 1] = WasmValue.FromF64(stack[top - 1].I32); return true;
            case 0xB8: stack[top - 1] = WasmValue.FromF64((uint)stack[top - 1].I32); return true;
            case 0xB9: stack[top - 1] = WasmValue.FromF64(stack[top - 1].I64); return true;
            case 0xBA: stack[top - 1] = WasmValue.FromF64((ulong)stack[top - 1].I64); return true;
            case 0xBB: stack[top - 1] = WasmValue.FromF64(stack[top - 1].F32); return true;

            // The four reinterpretations move no bits at all, which is exactly what they are for.
            case 0xBC: case 0xBD: case 0xBE: case 0xBF:
                return true;

            default:
                break;
        }

        if (opcode is < 0xA8 or > 0xB1)
        {
            return false;
        }

        var value = opcode switch
        {
            0xA8 or 0xA9 or 0xAE or 0xAF => (double)stack[top - 1].F32,
            _ => stack[top - 1].F64,
        };

        if (double.IsNaN(value))
        {
            trap = WasmTrapKind.InvalidConversionToInteger;
            return true;
        }

        var truncated = System.Math.Truncate(value);

        switch (opcode)
        {
            case 0xA8:
            case 0xAA:
                if (truncated < -2147483648.0 || truncated > 2147483647.0)
                {
                    trap = WasmTrapKind.IntegerOverflow;
                    return true;
                }

                stack[top - 1] = WasmValue.FromI32((int)truncated);
                return true;

            case 0xA9:
            case 0xAB:
                if (truncated < 0.0 || truncated > 4294967295.0)
                {
                    trap = WasmTrapKind.IntegerOverflow;
                    return true;
                }

                stack[top - 1] = WasmValue.FromI32((int)(uint)truncated);
                return true;

            case 0xAE:
            case 0xB0:
                if (truncated < -9223372036854775808.0 || truncated >= 9223372036854775808.0)
                {
                    trap = WasmTrapKind.IntegerOverflow;
                    return true;
                }

                stack[top - 1] = WasmValue.FromI64((long)truncated);
                return true;

            default:
                if (truncated < 0.0 || truncated >= 18446744073709551616.0)
                {
                    trap = WasmTrapKind.IntegerOverflow;
                    return true;
                }

                stack[top - 1] = WasmValue.FromI64((long)(ulong)truncated);
                return true;
        }
    }
}
