// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   33
// Annotated:        33/33
// Exempt:           4
// Human-reviewed:   0/33
// IP risk:          Low
// Security risk:    High
// Criteria:         14/14
// Resource impact:  3/10 max
// Unverified:       33
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Numerics;
using Broiler.ECMAScript;
using Broiler.ECMAScript.Numbers;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// One BigInt: an exact signed integer of bounded width. <b>Internal</b>; the language's BigInt,
/// built on it, is the <c>broiler.javascript.bigint</c> surface since card B05.
/// </summary>
/// <remarks>
/// <para>
/// <b>The magnitude is a <see cref="BigInteger"/>, and that is a choice between
/// allowed things rather than an assumption that a library is permitted</b> (decision JSD-0033).
/// It is part of the base class library, it is trimming- and Native-AOT-safe, it reaches no
/// reflection, dynamic loading or unmanaged code (rule B5), and it adds no reference outside the
/// framework (rule B1). A hand-written limb array was weighed and refused: it would be a second
/// arbitrary-precision implementation for this profile to get right, and arithmetic (JSeal B03) is
/// where a wrong carry would show up as a plausible wrong answer.
/// </para>
/// <para>
/// <b>A class and not the struct itself, for two reasons.</b> <see cref="JsValue"/> already carries
/// a reference field, so a BigInt costs the value layout nothing: the tag says BigInt, the double is
/// unused, and the reference is this. And a class is where the width ceiling, the size a charge is
/// computed from, and the canonical decoding live, so no caller handles a bare
/// <see cref="BigInteger"/> it could forget to bound.
/// </para>
/// <para>
/// <b>Identity means nothing.</b> Two instances holding one integer are the same BigInt: equality
/// and hashing are mathematical, which is what <c>===</c>, <c>SameValueZero</c> and a Map key
/// require of a primitive.
/// </para>
/// <para>
/// <b>The operations are the shared library's</b> (decision JSD-0069): the arithmetic, the bitwise
/// operations, the shifts, <c>asIntN</c> and <c>asUintN</c>, the text, the parse and the conversions
/// to and from a Number are <c>BigIntMath</c> and <c>BigIntText</c>, under <see cref="Policy"/> and
/// charged in this family's units to the fuel meter the engine passes. Each member below is an
/// adapter of one line: it passes the policy and the meter, and answers <see langword="null"/> for an
/// over-width status, which the engine turns into the <c>RangeError</c>.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=411082
// Broiler-Human:        PENDING
internal sealed class JsBigInt : IEquatable<JsBigInt>
{
    /// <summary>
    /// The widest magnitude a BigInt value of this realm may hold, in bits: 1,048,576 (128 KiB).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The constant bound (65,536 bits) and this value bound are two numbers with one owner. Every
    /// operation that computes a BigInt (JSeal B03-B04) answers <see langword="null"/> rather than a
    /// value past it, and the engine turns that into a <c>RangeError</c>; an operation whose result
    /// is known to be too wide from its operands' widths refuses before it allocates.
    /// </para>
    /// <para>
    /// <b>The width is <see cref="BigInteger.GetBitLength"/>'s</b>: the shortest
    /// two's-complement length without the sign bit. For a non-negative value that is the
    /// magnitude's bit count; for a negative power of two it is one less, so <c>-(2n ** 1048576n)</c>
    /// is admitted and <c>2n ** 1048576n</c> is not (decision JSD-0033, as amended for B03).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B101DC
    // Broiler-Human:        PENDING
    internal const long MaximumBits = 1L << 20;

    /// <summary>
    /// <see cref="MaximumBits"/> as the shared library's width policy: every operation below runs
    /// under it, and an embedder's BigInt is admitted by it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=CE45A3
    // Broiler-Falsified-If: an operation runs under another policy than MaximumBits, or a value the policy refuses is constructed
    // Broiler-Human:        PENDING
    internal static readonly BigIntPolicy Policy = new(MaximumBits);

    /// <summary>Creates a BigInt holding <paramref name="value"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BCC7BF
    // Broiler-Falsified-If: an instance holding a magnitude wider than MaximumBits is constructed
    // Broiler-Human:        PENDING
    internal JsBigInt(BigInteger value)
    {
        if (value.GetBitLength() > MaximumBits)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "a BigInt wider than the realm's ceiling was constructed");
        }

        Value = value;
    }

    /// <summary>The integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=647266
    // Broiler-Human:        PENDING
    internal BigInteger Value { get; }

    /// <summary>Whether this is <c>0n</c>, the one BigInt <c>ToBoolean</c> answers false for.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3A1C27
    // Broiler-Human:        PENDING
    internal bool IsZero => Value.IsZero;

    /// <summary>
    /// The width this value's ceiling is measured in: the shortest two's-complement length without
    /// the sign bit.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3E42AF
    // Broiler-Human:        PENDING
    internal long Bits => Value.GetBitLength();

    /// <summary>
    /// The size a charge over this value is computed from: its magnitude in 64-bit words, at least
    /// one (the shared library's <c>BigIntMath.Words</c>).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=40F59E
    // Broiler-Human:        PENDING
    internal int Words => BigIntMath.Words(Value);

    /// <summary>
    /// Decodes a constant's canonical payload: a sign and the magnitude's bytes, least significant
    /// first.
    /// </summary>
    /// <remarks>
    /// The verifier has already refused a non-canonical or over-wide payload; this is the one
    /// decoding, and it is linear in the payload.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=7FD4A2
    // Broiler-Falsified-If: a payload decodes to an integer other than the one its bytes spell
    // Broiler-Human:        PENDING
    internal static JsBigInt FromConstant(bool negative, ReadOnlySpan<byte> magnitude)
    {
        var value = new BigInteger(magnitude, isUnsigned: true, isBigEndian: false);
        return new JsBigInt(negative ? -value : value);
    }

    /// <summary>
    /// The abstract operation <c>BigInt::toString(x, 10)</c>: optional <c>-</c>, then decimal
    /// digits, and no <c>n</c>, with every step charged to <paramref name="charge"/> before it runs.
    /// </summary>
    /// <remarks>
    /// The shared library's <c>BigIntText.ToString</c>, divide and conquer: the digit bound is charged
    /// first, linearly, then each division and each leaf, so a wide value's conversion meets the
    /// cancellation poll between its steps.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=126ABC
    // Broiler-Falsified-If: a wider BigInt is converted to text for the same charge as a narrower one, or the text differs from the integer
    // Broiler-Human:        PENDING
    internal string ToDecimalString(WorkCharge charge) => BigIntText.ToString(Value, 10, charge);

    /// <summary>
    /// The abstract operation <c>BigInt::toString(x, radix)</c> for a radix from 2 to 36, which the
    /// caller has checked: optional <c>-</c>, then the digits in lower case.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=5A9097
    // Broiler-Falsified-If: a wider BigInt is converted for the same charge as a narrower one, or the digits differ from the integer in that radix
    // Broiler-Human:        PENDING
    internal string ToRadixString(int radix, WorkCharge charge) => BigIntText.ToString(Value, radix, charge);

    /// <summary>Mathematical equality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CA08E5
    // Broiler-Human:        PENDING
    public bool Equals(JsBigInt? other) => other is not null && Value.Equals(other.Value);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9C25EA
    // Broiler-Human:        PENDING
    public override bool Equals(object? obj) => obj is JsBigInt other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=777EBF
    // Broiler-Human:        PENDING
    public override int GetHashCode() => Value.GetHashCode();

    // ---- arithmetic (JSeal B03) and bitwise operations (JSeal B04) ---------------------------------
    //
    // EVERY OPERATION CHARGES BEFORE IT COMPUTES AND ANSWERS NULL RATHER THAN A VALUE PAST THE
    // CEILING. The work is the shared library's BigIntMath; the charge goes to the meter the engine
    // passes, which is also where cancellation is polled, and the engine turns a null into the
    // RangeError the language gives a result it cannot represent. Argument errors the specification
    // names - a zero divisor, a negative exponent - are the engine's to raise before it calls here.

    /// <summary><c>0n</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B561EC
    // Broiler-Human:        PENDING
    internal static readonly JsBigInt Zero = new(BigInteger.Zero);

    /// <summary><c>1n</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D8C5F1
    // Broiler-Human:        PENDING
    internal static readonly JsBigInt One = new(BigInteger.One);

    /// <summary><c>-1n</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7D8ADC
    // Broiler-Human:        PENDING
    internal static readonly JsBigInt MinusOne = new(BigInteger.MinusOne);

    /// <summary>A shared operation's result as a BigInt, or null for an over-width status.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=102039
    // Broiler-Falsified-If: a status other than Completed answers a value, or a Completed result is dropped
    // Broiler-Human:        PENDING
    private static JsBigInt? Of(ResultStatus status, BigInteger result) =>
        status == ResultStatus.Completed ? new JsBigInt(result) : null;

    /// <summary><c>BigInt::add</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F296A8
    // Broiler-Human:        PENDING
    internal static JsBigInt? Add(JsBigInt left, JsBigInt right, WorkCharge charge) =>
        Of(BigIntMath.Add(left.Value, right.Value, Policy, charge, out var result), result);

    /// <summary><c>BigInt::subtract</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=FDC19E
    // Broiler-Human:        PENDING
    internal static JsBigInt? Subtract(JsBigInt left, JsBigInt right, WorkCharge charge) =>
        Of(BigIntMath.Subtract(left.Value, right.Value, Policy, charge, out var result), result);

    /// <summary><c>BigInt::unaryMinus</c>; <c>-0n</c> is <c>0n</c>, since there is one zero.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=879E23
    // Broiler-Human:        PENDING
    internal static JsBigInt? Negate(JsBigInt value, WorkCharge charge) =>
        Of(BigIntMath.Negate(value.Value, Policy, charge, out var result), result);

    /// <summary><c>BigInt::bitwiseNOT</c>: <c>-x - 1</c>, which never widens a value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=97D8B8
    // Broiler-Human:        PENDING
    internal static JsBigInt? BitwiseNot(JsBigInt value, WorkCharge charge) =>
        Of(BigIntMath.BitwiseNot(value.Value, Policy, charge, out var result), result);

    /// <summary><c>BigInt::multiply</c>, refused before it allocates when the product must be too wide.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=7119BF
    // Broiler-Falsified-If: a product the operands' widths prove too wide is computed before it is refused
    // Broiler-Human:        PENDING
    internal static JsBigInt? Multiply(JsBigInt left, JsBigInt right, WorkCharge charge) =>
        Of(BigIntMath.Multiply(left.Value, right.Value, Policy, charge, out var result), result);

    /// <summary>
    /// <c>BigInt::divide</c>, truncating toward zero. <paramref name="right"/> is not zero: the
    /// engine raises the RangeError for that first.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8ADDD8
    // Broiler-Human:        PENDING
    internal static JsBigInt? Divide(JsBigInt left, JsBigInt right, WorkCharge charge) =>
        Of(BigIntMath.Divide(left.Value, right.Value, Policy, charge, out var result), result);

    /// <summary>
    /// <c>BigInt::remainder</c>, whose sign is the dividend's. <paramref name="right"/> is not zero.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0A9B56
    // Broiler-Human:        PENDING
    internal static JsBigInt? Remainder(JsBigInt left, JsBigInt right, WorkCharge charge) =>
        Of(BigIntMath.Remainder(left.Value, right.Value, Policy, charge, out var result), result);

    /// <summary>
    /// <c>BigInt::exponentiate</c> for a non-negative <paramref name="exponent"/> (the engine raises
    /// the RangeError for a negative one first), with every multiplication charged before it runs
    /// and a result that must pass the ceiling refused as soon as that is certain.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=049FCE
    // Broiler-Falsified-If: a power the operands prove too wide is multiplied out before it is refused
    // Broiler-Human:        PENDING
    internal static JsBigInt? Power(JsBigInt @base, JsBigInt exponent, WorkCharge charge) =>
        Of(BigIntMath.Power(@base.Value, exponent.Value, Policy, charge, out var result), result);

    /// <summary><c>BigInt::bitwiseAND</c>, on two's-complement values of unbounded width.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A514D0
    // Broiler-Human:        PENDING
    internal static JsBigInt? And(JsBigInt left, JsBigInt right, WorkCharge charge) =>
        Of(BigIntMath.BitwiseAnd(left.Value, right.Value, Policy, charge, out var result), result);

    /// <summary><c>BigInt::bitwiseOR</c>, on two's-complement values of unbounded width.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=972186
    // Broiler-Human:        PENDING
    internal static JsBigInt? Or(JsBigInt left, JsBigInt right, WorkCharge charge) =>
        Of(BigIntMath.BitwiseOr(left.Value, right.Value, Policy, charge, out var result), result);

    /// <summary><c>BigInt::bitwiseXOR</c>, on two's-complement values of unbounded width.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=92CCA0
    // Broiler-Human:        PENDING
    internal static JsBigInt? Xor(JsBigInt left, JsBigInt right, WorkCharge charge) =>
        Of(BigIntMath.BitwiseXor(left.Value, right.Value, Policy, charge, out var result), result);

    /// <summary>
    /// <c>BigInt::leftShift</c>: <c>value * 2 ** count</c>, which for a negative count is a right
    /// shift that rounds toward negative infinity.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=19840A
    // Broiler-Falsified-If: a shift the widths prove too wide allocates before it is refused
    // Broiler-Human:        PENDING
    internal static JsBigInt? ShiftLeft(JsBigInt value, JsBigInt count, WorkCharge charge) =>
        Of(BigIntMath.LeftShift(value.Value, count.Value, Policy, charge, out var result), result);

    /// <summary><c>BigInt::signedRightShift</c>: a left shift by the negated count.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=5695F9
    // Broiler-Falsified-If: a shift the widths prove too wide allocates before it is refused
    // Broiler-Human:        PENDING
    internal static JsBigInt? ShiftRight(JsBigInt value, JsBigInt count, WorkCharge charge) =>
        Of(BigIntMath.SignedRightShift(value.Value, count.Value, Policy, charge, out var result), result);

    /// <summary>
    /// The operation behind <c>BigInt.asIntN(bits, value)</c>: <paramref name="value"/> modulo
    /// <c>2 ** bits</c>, read as a signed <paramref name="bits"/>-bit integer.
    /// </summary>
    /// <remarks>
    /// <paramref name="bits"/> is the result of <c>ToIndex</c>, so it is at most <c>2 ** 53 - 1</c>.
    /// The result is never wider than the value, so this never answers null.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=ED6F08
    // Broiler-Human:        PENDING
    internal static JsBigInt? AsIntN(ulong bits, JsBigInt value, WorkCharge charge) =>
        Of(BigIntMath.AsIntN(bits, value.Value, Policy, charge, out var result), result);

    /// <summary>
    /// The operation behind <c>BigInt.asUintN(bits, value)</c>: <paramref name="value"/> modulo
    /// <c>2 ** bits</c>, or null when that is wider than the ceiling.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=769C52
    // Broiler-Falsified-If: a negative value modulo a width past the ceiling allocates before it is refused
    // Broiler-Human:        PENDING
    internal static JsBigInt? AsUintN(ulong bits, JsBigInt value, WorkCharge charge) =>
        Of(BigIntMath.AsUintN(bits, value.Value, Policy, charge, out var result), result);

    /// <summary>
    /// The abstract operation <c>NumberToBigInt</c> for an integral finite <paramref name="number"/>:
    /// the exact integer, which a double always has.
    /// </summary>
    /// <remarks>The caller has refused a fraction, a NaN and an infinity with the RangeError first.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9F18D4
    // Broiler-Human:        PENDING
    internal static JsBigInt FromIntegralNumber(double number) => new(BigIntMath.FromIntegralNumber(number));

    /// <summary>
    /// The Number nearest this integer, ties to even, and an infinity past <c>Number.MAX_VALUE</c>'s
    /// rounding range, charged as one pass over the value.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=278CFC
    // Broiler-Falsified-If: a BigInt converts to a Number other than the nearest one with ties to even, or a value at or past 2 ** 1024 - 2 ** 970 converts to a finite Number
    // Broiler-Human:        PENDING
    internal double ToNumber(WorkCharge charge) => BigIntMath.ToNumber(Value, charge);

    /// <summary>
    /// Compares this integer with a finite <paramref name="number"/> exactly: negative, zero or
    /// positive as this is below, equal to or above it. The engine answers NaN and the infinities
    /// before it calls.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=077ACE
    // Broiler-Falsified-If: a BigInt and a Number compare other than their mathematical values do, or either is rounded to compare
    // Broiler-Human:        PENDING
    internal int CompareToNumber(double number, WorkCharge charge) => BigIntMath.CompareToNumber(Value, number, charge);

    /// <summary>
    /// The abstract operation <c>StringToBigInt</c>: <see langword="false"/> when
    /// <paramref name="text"/> is not a <c>StringIntegerLiteral</c>; otherwise the value, or
    /// <see langword="null"/> with its sign in <paramref name="tooWideSign"/> when the value is wider
    /// than the ceiling.
    /// </summary>
    /// <remarks>
    /// <b>A value past the ceiling is reported, not refused</b>, because the callers need different
    /// things from it: <c>BigInt(text)</c> raises the RangeError, <c>==</c> answers <c>false</c> (no
    /// value in the realm equals it), and a relational comparison answers by its sign - each exact.
    /// The parse is the shared library's <c>BigIntText.StringToBigInt</c>, which charges the text's
    /// length before it reads it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=7D08EE
    // Broiler-Falsified-If: a text outside StringIntegerLiteral answers a value, or a value past the ceiling is answered rather than reported with its sign
    // Broiler-Human:        PENDING
    internal static bool TryParseStringInteger(string text, WorkCharge charge, out JsBigInt? value, out int tooWideSign)
    {
        switch (BigIntText.StringToBigInt(text, Policy, charge, out var parsed, out tooWideSign))
        {
            case ResultStatus.Completed:
                value = new JsBigInt(parsed);
                return true;
            case ResultStatus.LimitExceeded:
                value = null;
                return true;
            default:
                value = null;
                return false;
        }
    }
}
