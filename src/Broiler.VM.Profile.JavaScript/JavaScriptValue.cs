// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   20
// Annotated:        20/20
// Exempt:           8
// Human-reviewed:   0/20
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  1/10 max
// Unverified:       20
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>The value kinds the <c>broiler.javascript.slice</c> surface has.</summary>
/// <remarks>
/// Three of the language's seven primitive types and none of its object types. String, Symbol,
/// BigInt, Null and every object are outside this manifest, and the verifier refuses an artifact
/// that could produce one rather than the executor discovering it.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=C65804
// Broiler-Human:        PENDING
public enum JavaScriptValueKind : byte
{
    /// <summary>The one value <c>undefined</c>.</summary>
    Undefined = 0,

    /// <summary>A Boolean.</summary>
    Boolean = 1,

    /// <summary>A Number: IEEE 754 binary64, with the language's own arithmetic over it.</summary>
    Number = 2,
}

/// <summary>
/// One JavaScript value at the slice surface, and the abstract operations the language defines
/// over it.
/// </summary>
/// <remarks>
/// <para>
/// <b>This representation is provisional and JS-4 settles it.</b> Decision JSD-0004 and roadmap
/// section 8 make the value representation a gate on entry to JS-4 rather than that milestone's
/// first task, because the standard library is typed against whatever answer it gets. What is
/// here is the smallest thing that is honestly JavaScript over three primitive types: a kind and
/// a double, sixteen bytes, no heap allocation and no boxing. It is not a claim about what the
/// full surface will use, and no fixture, figure or Native AOT probe in this milestone may be
/// read as one.
/// </para>
/// <para>
/// <b>Why a tagged value rather than a bare double.</b> A profile that represented the slice as
/// doubles alone would have to answer <c>1 &lt; 2</c> with <c>1</c>, and <c>1 === true</c> with
/// true. Both are wrong in the language, and both are the kind of wrong that a corpus of
/// arithmetic would never catch. The tag is what makes this a JavaScript profile rather than a
/// calculator with a different magic number, and every operation below is written from the
/// specification's abstract operations rather than from what a double happens to do.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EE0E99
// Broiler-Human:        PENDING
public readonly struct JavaScriptValue : System.IEquatable<JavaScriptValue>
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=48AEDA
    // Broiler-Human:        PENDING
    private readonly double number;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3A53D4
    // Broiler-Human:        PENDING
    private JavaScriptValue(JavaScriptValueKind kind, double value)
    {
        Kind = kind;
        number = value;
    }

    /// <summary>The one <c>undefined</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=156546
    // Broiler-Human:        PENDING
    public static JavaScriptValue Undefined => new(JavaScriptValueKind.Undefined, 0);

    /// <summary>The Boolean <c>true</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=10C0E1
    // Broiler-Human:        PENDING
    public static JavaScriptValue True => new(JavaScriptValueKind.Boolean, 1);

    /// <summary>The Boolean <c>false</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2428A2
    // Broiler-Human:        PENDING
    public static JavaScriptValue False => new(JavaScriptValueKind.Boolean, 0);

    /// <summary>Which of the three kinds this is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E0124A
    // Broiler-Human:        PENDING
    public JavaScriptValueKind Kind { get; }

    /// <summary>A Number.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EF5F68
    // Broiler-Human:        PENDING
    public static JavaScriptValue Number(double value) => new(JavaScriptValueKind.Number, value);

    /// <summary>A Boolean.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=822A61
    // Broiler-Human:        PENDING
    public static JavaScriptValue Boolean(bool value) => value ? True : False;

    /// <summary>
    /// The abstract operation <c>ToNumber</c>, over the three kinds this surface has.
    /// </summary>
    /// <remarks>
    /// <c>undefined</c> is NaN and not zero, which is the case a implementation that reached for a
    /// default gets wrong. <c>true</c> is 1 and <c>false</c> is +0.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9411F0
    // Broiler-Human:        PENDING
    public double ToNumber() => Kind switch
    {
        JavaScriptValueKind.Number => number,
        JavaScriptValueKind.Boolean => number,
        _ => double.NaN,
    };

    /// <summary>
    /// The abstract operation <c>ToBoolean</c>.
    /// </summary>
    /// <remarks>
    /// A Number is false when it is <c>+0</c>, <c>-0</c> or <c>NaN</c>, and true otherwise. Note
    /// that <c>-0</c> is false and that <c>number != 0</c> already answers that correctly, while
    /// a comparison written as <c>number &gt; 0 || number &lt; 0</c> would too - but a test on the
    /// bit pattern would not, and this is where an implementation that "optimised" the check would
    /// diverge from the language.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=BCB85F
    // Broiler-Human:        PENDING
    public bool ToBooleanValue() => Kind switch
    {
        JavaScriptValueKind.Undefined => false,
        JavaScriptValueKind.Boolean => number != 0,
        _ => number != 0 && !double.IsNaN(number),
    };

    /// <summary>
    /// The abstract operation <c>ToInt32</c>: truncate, reduce modulo 2^32, reinterpret as signed.
    /// </summary>
    /// <remarks>
    /// NaN and both infinities are zero rather than saturating, which is the whole difference
    /// between this and a cast. A conversion written as <c>(int)value</c> in C# saturates for a
    /// value out of range and is undefined-ish for NaN, so <c>2147483648 | 0</c> would answer
    /// <c>2147483647</c> instead of <c>-2147483648</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=61E0F7
    // Broiler-Falsified-If: ToInt32 of 2147483648 is not -2147483648, or of NaN or an infinity is not 0
    // Broiler-Human:        PENDING
    public int ToInt32() => unchecked((int)ToUint32());

    /// <summary>The abstract operation <c>ToUint32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=983293
    // Broiler-Falsified-If: ToUint32 of -1 is not 4294967295, or of a value above 2^53 disagrees with the specification's modulo
    // Broiler-Human:        PENDING
    public uint ToUint32()
    {
        var value = ToNumber();

        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return 0;
        }

        // Truncation toward zero, then a modulo that lands in [0, 2^32). The remainder operator on
        // doubles is exact for every integral magnitude a double can hold, so this is the
        // specification's arithmetic and not an approximation of it.
        var truncated = System.Math.Truncate(value) % 4294967296.0;

        if (truncated < 0)
        {
            truncated += 4294967296.0;
        }

        return (uint)truncated;
    }

    /// <summary>
    /// The strict equality comparison, <c>===</c>.
    /// </summary>
    /// <remarks>
    /// Different kinds are never strictly equal, so <c>1 === true</c> is false. Within Number,
    /// <c>NaN</c> is equal to nothing including itself, and <c>+0</c> and <c>-0</c> are equal -
    /// which is the pair of exceptions that makes this not a bit comparison.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=4BA1F7
    // Broiler-Falsified-If: NaN is strictly equal to itself, or +0 is not strictly equal to -0, or 1 is strictly equal to true
    // Broiler-Human:        PENDING
    public bool StrictlyEquals(JavaScriptValue other)
    {
        if (Kind != other.Kind)
        {
            return false;
        }

        return Kind switch
        {
            JavaScriptValueKind.Undefined => true,
            JavaScriptValueKind.Boolean => number == other.number,

            // == on doubles is already the language's rule for Number: false for NaN on either
            // side, true for +0 against -0. Writing it any other way would be writing a different
            // relation and calling it this one.
            _ => number == other.number,
        };
    }

    /// <summary>
    /// The abstract relational comparison, with <see langword="false"/> for an undefined result.
    /// </summary>
    /// <remarks>
    /// The specification's comparison answers <c>undefined</c> when either operand is NaN, and
    /// every one of the four relational operators maps that to <c>false</c> - including
    /// <c>&gt;=</c>, which is why <c>NaN &gt;= NaN</c> is false rather than true. Folding the
    /// mapping in here rather than at each operator is what keeps all four consistent.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=C91DA6
    // Broiler-Falsified-If: any relational comparison involving NaN answers true
    // Broiler-Human:        PENDING
    public static bool LessThan(JavaScriptValue left, JavaScriptValue right) =>
        left.ToNumber() < right.ToNumber();

    /// <summary>The <c>&lt;=</c> comparison.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F96D19
    // Broiler-Human:        PENDING
    public static bool LessThanOrEqual(JavaScriptValue left, JavaScriptValue right) =>
        left.ToNumber() <= right.ToNumber();

    /// <summary>The <c>&gt;</c> comparison.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6DA591
    // Broiler-Human:        PENDING
    public static bool GreaterThan(JavaScriptValue left, JavaScriptValue right) =>
        left.ToNumber() > right.ToNumber();

    /// <summary>The <c>&gt;=</c> comparison.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CD5CA3
    // Broiler-Human:        PENDING
    public static bool GreaterThanOrEqual(JavaScriptValue left, JavaScriptValue right) =>
        left.ToNumber() >= right.ToNumber();

    /// <summary>Value equality over the representation, which is not the language's <c>===</c>.</summary>
    /// <remarks>
    /// This exists because the type is a struct and the compiler asks for it. It is deliberately
    /// NOT the language relation: a caller comparing two values as JavaScript would must call
    /// <see cref="StrictlyEquals"/>, and the difference is that this one says NaN equals NaN.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=FFF533
    // Broiler-Human:        PENDING
    public bool Equals(JavaScriptValue other) =>
        Kind == other.Kind && number.Equals(other.number);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=13683B
    // Broiler-Human:        PENDING
    public override bool Equals(object? obj) => obj is JavaScriptValue other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=87AB5A
    // Broiler-Human:        PENDING
    public override int GetHashCode() => System.HashCode.Combine(Kind, number);

    /// <summary>Representation equality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B67366
    // Broiler-Human:        PENDING
    public static bool operator ==(JavaScriptValue left, JavaScriptValue right) => left.Equals(right);

    /// <summary>Representation inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=519AE0
    // Broiler-Human:        PENDING
    public static bool operator !=(JavaScriptValue left, JavaScriptValue right) => !left.Equals(right);

    /// <summary>
    /// A stable rendering, for a corpus entry and a composition's own report to compare against.
    /// </summary>
    /// <remarks>
    /// <b>This is a fixed surface, not an approximated one.</b> Roadmap section 6 requires every
    /// surface the specification leaves implementation-defined to be named and either fixed or
    /// declared varying before a corpus entry is written over it. Number-to-string at the edges is
    /// one of those surfaces, so this method is deliberately NOT the language's <c>ToString</c>:
    /// it is a round-trip rendering this profile fixes for its own evidence, it names <c>-0</c>
    /// distinctly from <c>0</c> because the distinction is observable in the language, and no
    /// caller may present it as what a JavaScript program would print.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BFF18E
    // Broiler-Human:        PENDING
    public string ToDiagnosticString() => Kind switch
    {
        JavaScriptValueKind.Undefined => "undefined",
        JavaScriptValueKind.Boolean => number != 0 ? "true" : "false",
        _ when double.IsNaN(number) => "NaN",
        _ when double.IsPositiveInfinity(number) => "Infinity",
        _ when double.IsNegativeInfinity(number) => "-Infinity",
        _ when number == 0 && double.IsNegative(number) => "-0",
        _ => number.ToString("R", System.Globalization.CultureInfo.InvariantCulture),
    };
}
