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
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

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
/// <b>Four of these are not the platform's function of the same name.</b> <c>Math.round</c> rounds
/// halves toward +&#8734; rather than to even, so <c>System.Math.Round</c> answers <c>2</c> where the
/// language answers <c>3</c> for <c>2.5</c>; <c>Math.max</c> and <c>Math.min</c> have to distinguish
/// <c>+0</c> from <c>-0</c>, which <c>&gt;</c> cannot; and <c>Math.pow</c> answers <c>NaN</c> for
/// <c>pow(1, &#8734;)</c> where IEEE 754 - and therefore <c>System.Math.Pow</c> - answers <c>1</c>.
/// Each is written out below with the specification's cases rather than delegated. Two more,
/// <c>expm1</c> and <c>log1p</c>, are the platform's with the sign of a zero put back.
/// </para>
/// <para>
/// <b><c>Math.random</c> is a realm-local xorshift seeded from a constant.</b> A run is therefore
/// reproducible: the same program over a fresh realm draws the same sequence, which is what makes a
/// differential corpus comparable across runs and a failing case replayable. It is not a source of
/// unpredictability and nothing in this profile should treat it as one.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>The state of the realm's own generator. Non-zero, and never reseeded from a clock.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E852E9
    // Broiler-Human:        PENDING
    private ulong mathRandomState = 0x2545F4914F6CDD1DUL;

    /// <summary>Builds <c>Math</c> and defines it on the global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FBCF3
    // Broiler-Human:        PENDING
    private void SetupMath()
    {
        var math = new JsObject(ObjectPrototype, "Math");

        // Writable and configurable, NOT frozen: see the type remarks.
        GlobalObject.DefineBuiltIn("Math", JsValue.Object(math));

        math.DefineFrozen("E", JsValue.Number(System.Math.E));
        math.DefineFrozen("LN10", JsValue.Number(2.302585092994046));
        math.DefineFrozen("LN2", JsValue.Number(0.6931471805599453));
        math.DefineFrozen("LOG10E", JsValue.Number(0.4342944819032518));
        math.DefineFrozen("LOG2E", JsValue.Number(1.4426950408889634));
        math.DefineFrozen("PI", JsValue.Number(System.Math.PI));
        math.DefineFrozen("SQRT1_2", JsValue.Number(0.7071067811865476));
        math.DefineFrozen("SQRT2", JsValue.Number(1.4142135623730951));

        Method(math, "abs", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Abs(MathNumberAt(engine, arguments, 0))));

        Method(math, "acos", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Acos(MathNumberAt(engine, arguments, 0))));

        Method(math, "acosh", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Acosh(MathNumberAt(engine, arguments, 0))));

        Method(math, "asin", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Asin(MathNumberAt(engine, arguments, 0))));

        Method(math, "asinh", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Asinh(MathNumberAt(engine, arguments, 0))));

        Method(math, "atan", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Atan(MathNumberAt(engine, arguments, 0))));

        Method(math, "atanh", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Atanh(MathNumberAt(engine, arguments, 0))));

        // The arguments are coerced left to right, which is observable through a valueOf.
        Method(math, "atan2", 2, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Atan2(
                MathNumberAt(engine, arguments, 0), MathNumberAt(engine, arguments, 1))));

        Method(math, "cbrt", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Cbrt(MathNumberAt(engine, arguments, 0))));

        Method(math, "ceil", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Ceiling(MathNumberAt(engine, arguments, 0))));

        Method(math, "clz32", 1, static (engine, _, arguments) =>
            JsValue.Number(MathLeadingZeros(engine.ToUint32(ArgOfMath(arguments, 0)))));

        Method(math, "cos", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Cos(MathNumberAt(engine, arguments, 0))));

        Method(math, "cosh", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Cosh(MathNumberAt(engine, arguments, 0))));

        Method(math, "exp", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Exp(MathNumberAt(engine, arguments, 0))));

        Method(math, "expm1", 1, static (engine, _, arguments) =>
            JsValue.Number(MathExpMinusOne(MathNumberAt(engine, arguments, 0))));

        Method(math, "floor", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Floor(MathNumberAt(engine, arguments, 0))));

        Method(math, "fround", 1, static (engine, _, arguments) =>
            JsValue.Number((double)(float)MathNumberAt(engine, arguments, 0)));

        Method(math, "hypot", 2, static (engine, _, arguments) =>
            JsValue.Number(MathHypotenuse(engine, arguments)));

        Method(math, "imul", 2, static (engine, _, arguments) =>
            JsValue.Number(MathIntegerProduct(engine, arguments)));

        Method(math, "log", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Log(MathNumberAt(engine, arguments, 0))));

        Method(math, "log1p", 1, static (engine, _, arguments) =>
            JsValue.Number(MathLogOnePlus(MathNumberAt(engine, arguments, 0))));

        Method(math, "log10", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Log10(MathNumberAt(engine, arguments, 0))));

        Method(math, "log2", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Log2(MathNumberAt(engine, arguments, 0))));

        Method(math, "max", 2, static (engine, _, arguments) =>
            JsValue.Number(MathLargest(engine, arguments)));

        Method(math, "min", 2, static (engine, _, arguments) =>
            JsValue.Number(MathSmallest(engine, arguments)));

        Method(math, "pow", 2, static (engine, _, arguments) =>
            JsValue.Number(MathPower(
                MathNumberAt(engine, arguments, 0), MathNumberAt(engine, arguments, 1))));

        // The only member that reads and writes realm state, so the only lambda that is not static.
        Method(math, "random", 0, (_, _, _) => JsValue.Number(MathNextRandom()));

        Method(math, "round", 1, static (engine, _, arguments) =>
            JsValue.Number(MathRoundHalfUp(MathNumberAt(engine, arguments, 0))));

        Method(math, "sign", 1, static (engine, _, arguments) =>
            JsValue.Number(MathSignOf(MathNumberAt(engine, arguments, 0))));

        Method(math, "sin", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Sin(MathNumberAt(engine, arguments, 0))));

        Method(math, "sinh", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Sinh(MathNumberAt(engine, arguments, 0))));

        Method(math, "sqrt", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Sqrt(MathNumberAt(engine, arguments, 0))));

        Method(math, "tan", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Tan(MathNumberAt(engine, arguments, 0))));

        Method(math, "tanh", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Tanh(MathNumberAt(engine, arguments, 0))));

        Method(math, "trunc", 1, static (engine, _, arguments) =>
            JsValue.Number(System.Math.Truncate(MathNumberAt(engine, arguments, 0))));
    }

    /// <summary>The argument at <paramref name="at"/>, or <c>undefined</c> when there is none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=760962
    // Broiler-Human:        PENDING
    private static JsValue ArgOfMath(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>The argument at <paramref name="at"/> as a Number, which may run guest code.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F3B6A0
    // Broiler-Human:        PENDING
    private static double MathNumberAt(JsEngine engine, JsValue[] arguments, int at) =>
        engine.ToNumber(ArgOfMath(arguments, at));

    /// <summary>
    /// <c>exp(x) - 1</c>, computed without losing the significant digits near zero.
    /// </summary>
    /// <remarks>
    /// The operation is IEEE 754's <c>expm1</c> and it lives on <c>double</c> rather than on
    /// <c>System.Math</c>, which has no such member. The zero case is taken here rather than left
    /// to the platform because <c>double.ExpM1(-0)</c> answers <c>+0</c> and the language requires
    /// <c>-0</c> - a difference nothing but a conformance test ever sees, and one it always sees.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=305E18
    // Broiler-Human:        PENDING
    private static double MathExpMinusOne(double value) =>
        value == 0 ? value : double.ExpM1(value);

    /// <summary>
    /// <c>log(1 + x)</c>, computed without losing the significant digits near zero.
    /// </summary>
    /// <remarks>
    /// IEEE 754's <c>log1p</c>, with the same signed-zero correction as
    /// <see cref="MathExpMinusOne"/>: <c>double.LogP1(-0)</c> answers <c>+0</c> where the language
    /// requires <c>-0</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7ADF5B
    // Broiler-Human:        PENDING
    private static double MathLogOnePlus(double value) =>
        value == 0 ? value : double.LogP1(value);

    /// <summary>
    /// The specification's <c>Math.round</c>, which is not <c>System.Math.Round</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The language rounds a half toward +&#8734; and the platform rounds it to even, so the two
    /// disagree on every other half: <c>Math.round(2.5)</c> is <c>3</c> and
    /// <c>System.Math.Round(2.5)</c> is <c>2</c>. Rounding toward +&#8734; also means
    /// <c>Math.round(-0.5)</c> is <c>-0</c> rather than <c>-1</c>.
    /// </para>
    /// <para>
    /// The three guards before the <c>floor(x + 1/2)</c> are not decoration. An already-integral
    /// argument is returned unchanged, because for a magnitude at or above 2^52 the addition itself
    /// rounds and would answer <c>x + 1</c> for an odd <c>x</c>. The two half-open intervals around
    /// zero are returned as signed zeroes, because <c>floor(0.49999999999999994 + 0.5)</c> is
    /// <c>1</c> - the addition rounds up to exactly <c>1</c> - where the closest integer is
    /// <c>0</c>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3E0826
    // Broiler-Human:        PENDING
    private static double MathRoundHalfUp(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value == 0)
        {
            return value;
        }

        if (System.Math.Floor(value) == value)
        {
            return value;
        }

        if (value > 0 && value < 0.5)
        {
            return 0;
        }

        if (value < 0 && value >= -0.5)
        {
            return -0.0;
        }

        return System.Math.Floor(value + 0.5);
    }

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
    /// The specification's <c>Math.hypot</c>: <c>+0</c> for no arguments, <c>+&#8734;</c> when any
    /// argument is infinite even if another is <c>NaN</c>, and otherwise the square root of the sum
    /// of the squares.
    /// </summary>
    /// <remarks>
    /// The sum is taken over the magnitudes divided by the largest of them, so an argument near the
    /// top of the range does not overflow on being squared and one near the bottom does not flush to
    /// zero. The scaling is exact for powers of two and costs one extra pass, which is why the
    /// coerced values are kept rather than re-read - re-reading would run a <c>valueOf</c> twice.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F3A8FA
    // Broiler-Human:        PENDING
    private static double MathHypotenuse(JsEngine engine, JsValue[] arguments)
    {
        var magnitudes = new System.Collections.Generic.List<double>(arguments.Length);
        var largest = 0.0;
        var sawInfinity = false;
        var sawNotANumber = false;

        for (var at = 0; at < arguments.Length; at++)
        {
            engine.Charge(1);
            var value = engine.ToNumber(arguments[at]);

            if (double.IsInfinity(value))
            {
                sawInfinity = true;
            }
            else if (double.IsNaN(value))
            {
                sawNotANumber = true;
            }
            else
            {
                var magnitude = System.Math.Abs(value);
                magnitudes.Add(magnitude);

                if (magnitude > largest)
                {
                    largest = magnitude;
                }
            }
        }

        if (sawInfinity)
        {
            return double.PositiveInfinity;
        }

        if (sawNotANumber)
        {
            return double.NaN;
        }

        if (largest == 0)
        {
            return 0;
        }

        var sum = 0.0;

        foreach (var magnitude in magnitudes)
        {
            engine.Charge(1);
            var scaled = magnitude / largest;
            sum += scaled * scaled;
        }

        return largest * System.Math.Sqrt(sum);
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

    /// <summary>
    /// The specification's <c>Math.pow</c>, which differs from IEEE 754 in four places.
    /// </summary>
    /// <remarks>
    /// IEEE 754 - and therefore <c>System.Math.Pow</c> - answers <c>1</c> for <c>pow(1, &#8734;)</c>
    /// and <c>pow(1, NaN)</c>, on the reasoning that one raised to anything is one. The language
    /// answers <c>NaN</c> whenever the exponent is <c>NaN</c> or the base has magnitude <c>1</c> and
    /// the exponent is infinite, and answers <c>1</c> for a zero exponent even over a <c>NaN</c>
    /// base. Those four cases are taken first and everything else is the platform's.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CFC17F
    // Broiler-Human:        PENDING
    private static double MathPower(double baseValue, double exponent)
    {
        if (double.IsNaN(exponent))
        {
            return double.NaN;
        }

        if (exponent == 0)
        {
            return 1;
        }

        if (double.IsNaN(baseValue))
        {
            return double.NaN;
        }

        if (double.IsInfinity(exponent) && System.Math.Abs(baseValue) == 1)
        {
            return double.NaN;
        }

        return System.Math.Pow(baseValue, exponent);
    }

    /// <summary>Counts the leading zero bits of a 32-bit value, answering 32 for zero.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=668652
    // Broiler-Human:        PENDING
    private static int MathLeadingZeros(uint bits)
    {
        if (bits == 0)
        {
            return 32;
        }

        var count = 0;

        while ((bits & 0x80000000u) == 0)
        {
            bits <<= 1;
            count++;
        }

        return count;
    }

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
}
