// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           1
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    Medium
// Criteria:         1/0
// Resource impact:  3/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Buffers.Binary;
using System.Security.Cryptography;
using Broiler.ECMAScript.Numbers;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>Math</c> namespace object: eight constants and thirty-five functions over doubles.
/// </summary>
/// <remarks>
/// <para>
/// <b><c>Math</c> is an ordinary object, and it is deliberately not frozen.</b> It is defined on the
/// global object as writable and configurable, and every function on it is writable and
/// configurable, because the first thing a benchmark harness does is replace <c>Math.random</c>
/// with a seeded generator of its own. An implementation that froze the namespace - which reads as
/// the safer choice - would make the whole Octane corpus unrunnable and the failure would surface
/// as a silent score difference rather than as a throw.
/// </para>
/// <para>
/// <b>Eight of these are the shared library's, not the platform's function of the same name.</b>
/// <c>Math.round</c>, <c>Math.pow</c> (and with it the <c>**</c> operator), <c>Math.hypot</c>,
/// <c>Math.cbrt</c>, <c>Math.expm1</c>, <c>Math.log1p</c>, <c>Math.f16round</c> and the sum of
/// <c>Math.sumPrecise</c> are <c>Broiler.ECMAScript.Numbers</c>' <c>MathKernels</c>,
/// <c>Float16</c> and <c>PreciseSum</c> (JSD-0069). The platform's functions differ from the
/// language: <c>System.Math.Round</c> answers <c>2</c> where the language answers <c>3</c> for
/// <c>2.5</c>, and IEEE 754's pow - and therefore <c>System.Math.Pow</c> - answers <c>1</c> for
/// <c>pow(1, &#8734;)</c> where the language answers <c>NaN</c>. What runs guest code stays here:
/// each argument's <c>ToNumber</c>, in order, and <c>sumPrecise</c>'s iteration. <c>Math.max</c> and
/// <c>Math.min</c> are written out below, because they have to distinguish <c>+0</c> from <c>-0</c>,
/// which <c>&gt;</c> cannot.
/// </para>
/// <para>
/// <b><c>Math.random</c> is a realm-local xorshift seeded per realm from the platform's entropy
/// source.</b> ES2026 21.3.2.27 requires each realm to produce a distinct sequence, and the seed is
/// the implementation's to choose, so two realms of one process and two processes draw different
/// sequences. Until 2026-09-29 the seed was a constant, and every realm of every process drew one
/// sequence. The reason given then was replay: "the same program over a fresh realm draws the same
/// sequence, which is what makes a differential corpus comparable across runs and a failing case
/// replayable". No retained program in this checkout draws from it, and a program that needs a
/// replayable sequence installs its own, as every benchmark harness does. It is still not a source
/// of unpredictability to rely on: the generator is not cryptographic, and only its seed is not a
/// constant.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>
    /// The state of the realm's own generator: drawn once per realm from the platform's entropy
    /// source, non-zero, and never reseeded from a clock.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C07584
    // Broiler-Human:        PENDING
    private ulong mathRandomState = MathRandomSeed();

    /// <summary>A seed for one realm's generator, drawn from the platform's entropy source.</summary>
    /// <remarks>
    /// A xorshift state of zero stays zero, so a zero draw is replaced by the constant that seeded
    /// every realm until 2026-09-29. The odds of that draw are one in 2^64.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=261289
    // Broiler-Human:        PENDING
    private static ulong MathRandomSeed()
    {
        Span<byte> bytes = stackalloc byte[sizeof(ulong)];
        RandomNumberGenerator.Fill(bytes);
        var seed = BinaryPrimitives.ReadUInt64LittleEndian(bytes);
        return seed != 0 ? seed : 0x2545F4914F6CDD1DUL;
    }

    /// <summary>Builds <c>Math</c> and defines it on the global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7EAAE2
    // Broiler-Human:        PENDING
    private void SetupMath()
    {
        var math = new JsObject(ObjectPrototype, "Math");

        // THE TAG A NAMESPACE OBJECT CARRIES, which is what makes `Object.prototype.toString.call(Math)`
        // answer `[object Math]` rather than `[object Object]`.
        math.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Math"), JsPropertyAttributes.Configurable));

        // Writable and configurable, NOT frozen: see the type remarks.
        GlobalObject.DefineBuiltIn("Math", JsValue.Object(math));

        math.DefineFrozen("E", JsValue.Number(Math.E));
        math.DefineFrozen("LN10", JsValue.Number(2.302585092994046));
        math.DefineFrozen("LN2", JsValue.Number(0.6931471805599453));
        math.DefineFrozen("LOG10E", JsValue.Number(0.4342944819032518));
        math.DefineFrozen("LOG2E", JsValue.Number(1.4426950408889634));
        math.DefineFrozen("PI", JsValue.Number(Math.PI));
        math.DefineFrozen("SQRT1_2", JsValue.Number(0.7071067811865476));
        math.DefineFrozen("SQRT2", JsValue.Number(1.4142135623730951));

        Method(math, "abs", 1, static (engine, _, arguments) => JsValue.Number(Math.Abs(MathNumberAt(engine, arguments, 0))));
        Method(math, "acos", 1, static (engine, _, arguments) => JsValue.Number(Math.Acos(MathNumberAt(engine, arguments, 0))));
        Method(math, "acosh", 1, static (engine, _, arguments) => JsValue.Number(Math.Acosh(MathNumberAt(engine, arguments, 0))));
        Method(math, "asin", 1, static (engine, _, arguments) => JsValue.Number(Math.Asin(MathNumberAt(engine, arguments, 0))));
        Method(math, "asinh", 1, static (engine, _, arguments) => JsValue.Number(Math.Asinh(MathNumberAt(engine, arguments, 0))));
        Method(math, "atan", 1, static (engine, _, arguments) => JsValue.Number(Math.Atan(MathNumberAt(engine, arguments, 0))));
        Method(math, "atanh", 1, static (engine, _, arguments) => JsValue.Number(Math.Atanh(MathNumberAt(engine, arguments, 0))));

        // The arguments are coerced left to right, which is observable through a valueOf.
        Method(math, "atan2", 2, static (engine, _, arguments) => JsValue.Number(Math.Atan2(MathNumberAt(engine, arguments, 0), MathNumberAt(engine, arguments, 1))));

        // CORRECTLY ROUNDED, so a perfect cube answers its own root: `Math.cbrt(27) === 3`, where
        // the platform's `3.0000000000000004` is conforming too but reads as a defect to a program
        // that compares a cube root against an integer.
        Method(math, "cbrt", 1, static (engine, _, arguments) => JsValue.Number(MathKernels.Cbrt(MathNumberAt(engine, arguments, 0))));

        Method(math, "ceil", 1, static (engine, _, arguments) => JsValue.Number(Math.Ceiling(MathNumberAt(engine, arguments, 0))));
        Method(math, "clz32", 1, static (engine, _, arguments) => JsValue.Number(MathLeadingZeros(engine.ToUint32(ArgOfMath(arguments, 0)))));
        Method(math, "cos", 1, static (engine, _, arguments) => JsValue.Number(Math.Cos(MathNumberAt(engine, arguments, 0))));
        Method(math, "cosh", 1, static (engine, _, arguments) => JsValue.Number(Math.Cosh(MathNumberAt(engine, arguments, 0))));
        Method(math, "exp", 1, static (engine, _, arguments) => JsValue.Number(Math.Exp(MathNumberAt(engine, arguments, 0))));
        Method(math, "expm1", 1, static (engine, _, arguments) => JsValue.Number(MathKernels.Expm1(MathNumberAt(engine, arguments, 0))));
        Method(math, "floor", 1, static (engine, _, arguments) => JsValue.Number(Math.Floor(MathNumberAt(engine, arguments, 0))));
        Method(math, "fround", 1, static (engine, _, arguments) => JsValue.Number((double)(float)MathNumberAt(engine, arguments, 0)));

        // HALF PRECISION, WHICH THE LANGUAGE ADDED FOR THE SAME REASON IT ADDED `fround`: a program
        // that stores numbers in a Float16Array wants to know what a value will become before it
        // stores it. So it is the SAME conversion the Float16Array and the DataView accessors
        // store through - the shared library's `Float16` - and a value this answers is exactly the
        // value a store would keep.
        Method(math, "f16round", 1, static (engine, _, arguments) => JsValue.Number(Float16.Round(MathNumberAt(engine, arguments, 0))));

        // AN EXACTLY ROUNDED SUM, which is not what a loop of additions gives: adding left to right
        // rounds at every step and the errors accumulate, so `0.1 + 0.2 + 0.3` and
        // `0.3 + 0.2 + 0.1` differ. The shared library's `PreciseSum` carries the exact value and
        // rounds it once, at the end; the iteration is this file's.
        Method(math, "sumPrecise", 1, static (engine, _, arguments) => JsValue.Number(MathSumPrecise(engine, MathArgumentAt(arguments, 0))));
        Method(math, "hypot", 2, static (engine, _, arguments) => JsValue.Number(MathKernels.Hypot(MathHypotOperands(engine, arguments))));
        Method(math, "imul", 2, static (engine, _, arguments) => JsValue.Number(MathIntegerProduct(engine, arguments)));
        Method(math, "log", 1, static (engine, _, arguments) => JsValue.Number(Math.Log(MathNumberAt(engine, arguments, 0))));
        Method(math, "log1p", 1, static (engine, _, arguments) => JsValue.Number(MathKernels.Log1p(MathNumberAt(engine, arguments, 0))));
        Method(math, "log10", 1, static (engine, _, arguments) => JsValue.Number(Math.Log10(MathNumberAt(engine, arguments, 0))));
        Method(math, "log2", 1, static (engine, _, arguments) => JsValue.Number(Math.Log2(MathNumberAt(engine, arguments, 0))));
        Method(math, "max", 2, static (engine, _, arguments) => JsValue.Number(MathLargest(engine, arguments)));
        Method(math, "min", 2, static (engine, _, arguments) => JsValue.Number(MathSmallest(engine, arguments)));
        Method(math, "pow", 2, static (engine, _, arguments) => JsValue.Number(MathKernels.Power(MathNumberAt(engine, arguments, 0), MathNumberAt(engine, arguments, 1))));

        // The only member that reads and writes realm state, so the only lambda that is not static.
        Method(math, "random", 0, (_, _, _) => JsValue.Number(MathNextRandom()));
        Method(math, "round", 1, static (engine, _, arguments) => JsValue.Number(MathKernels.Round(MathNumberAt(engine, arguments, 0))));
        Method(math, "sign", 1, static (engine, _, arguments) => JsValue.Number(MathSignOf(MathNumberAt(engine, arguments, 0))));
        Method(math, "sin", 1, static (engine, _, arguments) => JsValue.Number(Math.Sin(MathNumberAt(engine, arguments, 0))));
        Method(math, "sinh", 1, static (engine, _, arguments) => JsValue.Number(Math.Sinh(MathNumberAt(engine, arguments, 0))));
        Method(math, "sqrt", 1, static (engine, _, arguments) => JsValue.Number(Math.Sqrt(MathNumberAt(engine, arguments, 0))));
        Method(math, "tan", 1, static (engine, _, arguments) => JsValue.Number(Math.Tan(MathNumberAt(engine, arguments, 0))));
        Method(math, "tanh", 1, static (engine, _, arguments) => JsValue.Number(Math.Tanh(MathNumberAt(engine, arguments, 0))));
        Method(math, "trunc", 1, static (engine, _, arguments) => JsValue.Number(Math.Truncate(MathNumberAt(engine, arguments, 0))));
    }

    /// <summary>The argument at <paramref name="at"/>, or <c>undefined</c> when there is none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=760962
    // Broiler-Human:        PENDING
    private static JsValue ArgOfMath(JsValue[] arguments, int at) => at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>The argument at <paramref name="at"/> as a Number, which may run guest code.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F3B6A0
    // Broiler-Human:        PENDING
    private static double MathNumberAt(JsEngine engine, JsValue[] arguments, int at) =>
        engine.ToNumber(ArgOfMath(arguments, at));

    /// <summary>The specification's <c>Math.sign</c>, which answers a signed zero for a signed zero.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5E329B
    // Broiler-Human:        PENDING
    private static double MathSignOf(double value)
    {
        if (double.IsNaN(value) || value == 0)
        {
            return value;
        }

        return value < 0 ? -1 : 1;
    }

    /// <summary>
    /// The specification's <c>Math.max</c>: <c>-&#8734;</c> for no arguments, <c>NaN</c> when any
    /// argument is <c>NaN</c>, and <c>+0</c> in preference to <c>-0</c>.
    /// </summary>
    /// <remarks>
    /// Every argument is coerced even after a <c>NaN</c> has been seen, because coercion is
    /// observable through a <c>valueOf</c> and the specification coerces the whole list before it
    /// compares anything. The <c>-0</c> clause exists because <c>+0 &gt; -0</c> is false: the two
    /// compare equal and only their sign bits tell them apart.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BFD5A1
    // Broiler-Human:        PENDING
    private static double MathLargest(JsEngine engine, JsValue[] arguments)
    {
        var largest = double.NegativeInfinity;
        var sawNotANumber = false;

        for (var at = 0; at < arguments.Length; at++)
        {
            engine.Charge(1);
            var candidate = engine.ToNumber(arguments[at]);

            if (double.IsNaN(candidate))
            {
                sawNotANumber = true;
                continue;
            }

            if (candidate > largest ||
                (candidate == largest && double.IsNegative(largest) && !double.IsNegative(candidate)))
            {
                largest = candidate;
            }
        }

        return sawNotANumber ? double.NaN : largest;
    }

    /// <summary>
    /// The specification's <c>Math.min</c>: <c>+&#8734;</c> for no arguments, <c>NaN</c> when any
    /// argument is <c>NaN</c>, and <c>-0</c> in preference to <c>+0</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2E49D7
    // Broiler-Human:        PENDING
    private static double MathSmallest(JsEngine engine, JsValue[] arguments)
    {
        var smallest = double.PositiveInfinity;
        var sawNotANumber = false;

        for (var at = 0; at < arguments.Length; at++)
        {
            engine.Charge(1);
            var candidate = engine.ToNumber(arguments[at]);

            if (double.IsNaN(candidate))
            {
                sawNotANumber = true;
                continue;
            }

            if (candidate < smallest ||
                (candidate == smallest && !double.IsNegative(smallest) && double.IsNegative(candidate)))
            {
                smallest = candidate;
            }
        }

        return sawNotANumber ? double.NaN : smallest;
    }

    /// <summary>
    /// <c>Math.hypot</c>'s arguments as Numbers, each argument's <c>ToNumber</c> in order, for the
    /// shared library's <c>MathKernels.Hypot</c>.
    /// </summary>
    /// <remarks>
    /// Every argument is coerced before any is examined, because coercion is observable through a
    /// <c>valueOf</c>; the coerced values are kept rather than re-read, since re-reading would run a
    /// <c>valueOf</c> twice. <b>The charges are the engine's and are unchanged</b>: one unit per
    /// argument as it is coerced, and, when every value is finite and one is not a zero, one more per
    /// argument for the summing pass the kernel then makes.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5EBD76
    // Broiler-Human:        PENDING
    private static double[] MathHypotOperands(JsEngine engine, JsValue[] arguments)
    {
        var values = new double[arguments.Length];
        var summed = true;
        var nonZero = false;

        for (var at = 0; at < arguments.Length; at++)
        {
            engine.Charge(1);
            var value = engine.ToNumber(arguments[at]);
            values[at] = value;
            summed &= double.IsFinite(value);
            nonZero |= value != 0;
        }

        if (summed && nonZero)
        {
            for (var at = 0; at < values.Length; at++)
            {
                engine.Charge(1);
            }
        }

        return values;
    }

    /// <summary>
    /// The specification's <c>Math.imul</c>: the low 32 bits of the product of two ToInt32 values.
    /// </summary>
    /// <remarks>
    /// The wrap is the point of the operation rather than an accident of it, so the multiplication
    /// is written <c>unchecked</c> even though this assembly does not compile with overflow checks
    /// on. Someone turning them on later should not turn this into a throw.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5FD3FE
    // Broiler-Human:        PENDING
    private static double MathIntegerProduct(JsEngine engine, JsValue[] arguments)
    {
        var left = engine.ToInt32(ArgOfMath(arguments, 0));
        var right = engine.ToInt32(ArgOfMath(arguments, 1));
        return unchecked(left * right);
    }

    /// <summary>Counts the leading zero bits of a 32-bit value, answering 32 for zero.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=202216
    // Broiler-Human:        PENDING
    private static int MathLeadingZeros(uint bits) =>
        System.Numerics.BitOperations.LeadingZeroCount(bits);

    /// <summary>
    /// The next draw from the realm's generator: a double in <c>[0, 1)</c>.
    /// </summary>
    /// <remarks>
    /// A 64-bit xorshift with the (13, 7, 17) triple, whose period is 2^64-1 and whose state is
    /// therefore never zero once it starts non-zero. The top 53 bits of the state are scaled by
    /// 2^-53, which is the widest fraction a double represents exactly, so every draw is a distinct
    /// representable value and the distribution has no gaps. This is not a cryptographic generator
    /// and nothing may use it as one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E92C57
    // Broiler-Human:        PENDING
    private double MathNextRandom()
    {
        var state = mathRandomState;
        state ^= state << 13;
        state ^= state >> 7;
        state ^= state << 17;
        mathRandomState = state;
        return (state >> 11) * (1.0 / 9007199254740992.0);
    }

    /// <summary>Reads one argument, which the caller may not have supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=270320
    // Broiler-Human:        PENDING
    private static JsValue MathArgumentAt(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>The count of elements at which <c>Math.sumPrecise</c> stops: 2^53.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=55572E
    // Broiler-Human:        PENDING
    private const long MathSumPreciseLimit = 9007199254740992L;

    /// <summary>The exactly rounded sum of an iterable of Numbers (ES2026 21.3.2.34).</summary>
    /// <remarks>
    /// <para>
    /// <b>It iterates, and only iterates.</b> <c>GetIterator(items, sync)</c> refuses a nullish
    /// value and a value without a <c>Symbol.iterator</c> method - an array-like among them - with a
    /// <c>TypeError</c>, and reads that method once; a primitive whose prototype is iterable is
    /// iterated. Until SJL-4.4 an array-like was walked by its <c>length</c> and the method was read
    /// twice (JSD-0069).
    /// </para>
    /// <para>
    /// <b>Every element must already BE a Number.</b> This is the one arithmetic method in the
    /// namespace that does not coerce: a string in the list is a <c>TypeError</c>, and the iterator
    /// is CLOSED where the walk stops, so a generator handed a bad list runs its <c>finally</c> and
    /// is asked for nothing more. The 2^53rd element is a <c>RangeError</c>, closed the same way.
    /// </para>
    /// <para>
    /// <b>The sum is the shared library's <c>PreciseSum</c></b>, which is handed each element once
    /// it is known to be a Number. It decides NaN and the infinities as the clause's states - both
    /// infinities present is NaN, either alone is itself, and a NaN anywhere wins - and carries the
    /// finite values exactly, rounding once at the end.
    /// </para>
    /// <para>
    /// <b>An empty list sums to negative zero</b>, which is the identity this operation was given
    /// so that the sign of a sum of negative zeros survives.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=95BDCF
    // Broiler-Falsified-If: an array-like without Symbol.iterator is summed, Symbol.iterator is read twice, or an element that is not a Number is summed or leaves the iterator unclosed
    // Broiler-Human:        PENDING
    private static double MathSumPrecise(JsEngine engine, JsValue items)
    {
        var record = engine.GetIterator(items);
        var sum = new PreciseSum();
        var count = 0L;

        while (engine.TryIterateNext(record, out var element))
        {
            // TWO UNITS AN ELEMENT, the walk's and the sum's, as the element was charged before.
            engine.Charge(1);
            engine.Charge(1);

            if (++count >= MathSumPreciseLimit)
            {
                engine.CloseIteratorQuietly(record);
                throw engine.Error("RangeError", "Math.sumPrecise: too many elements");
            }

            if (!element.IsNumber)
            {
                engine.CloseIteratorQuietly(record);
                throw engine.Error("TypeError", "Math.sumPrecise: an element is not a number");
            }

            sum.Add(element.AsNumber());
        }

        return sum.Result();
    }
}
