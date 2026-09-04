// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   26
// Annotated:        26/26
// Exempt:           19
// Human-reviewed:   0/26
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       26
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>The value kinds the <c>broiler.javascript.wide</c> surface has.</summary>
/// <remarks>
/// Six of the language's seven primitive types are here, and Symbol and BigInt are not: the
/// manifest admits neither, so a value of either kind is unreachable rather than unhandled.
/// <see cref="Empty"/> is not a language value at all - it is the marker a binding holds before it
/// is initialised, and reading it is what makes the temporal dead zone a throw rather than an
/// <c>undefined</c>.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7A80F2
// Broiler-Human:        PENDING
internal enum JsType : byte
{
    /// <summary>The slot holds nothing yet: an uninitialised binding.</summary>
    Empty = 0,

    /// <summary>The one value <c>undefined</c>.</summary>
    Undefined = 1,

    /// <summary>The one value <c>null</c>.</summary>
    Null = 2,

    /// <summary>A Boolean.</summary>
    Boolean = 3,

    /// <summary>A Number: IEEE 754 binary64.</summary>
    Number = 4,

    /// <summary>A String: a sequence of UTF-16 code units.</summary>
    String = 5,

    /// <summary>An object, which includes every function and every array.</summary>
    Object = 6,
}

/// <summary>
/// One JavaScript value at the wide surface: a tag, a double and a reference.
/// </summary>
/// <remarks>
/// <para>
/// <b>A tagged struct rather than a boxed object or a NaN-boxed word.</b> The slice's sixteen-byte
/// pair could not carry a reference and this one has to; a NaN-boxed 64-bit word could, and is the
/// representation a production engine reaches for, but it needs the garbage collector to be told
/// which words are pointers, and the collector here is the CLR's. What this uses instead is the
/// representation the CLR can already trace: a reference field the runtime knows about and a
/// double beside it.
/// </para>
/// <para>
/// The cost is stated rather than hidden: this is 24 bytes where a production engine uses 8, so
/// every operand-stack entry, every environment slot and every array element is three times the
/// size. That is a measurement this milestone is built to be able to take, not a design nobody
/// looked at.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6CE7AB
// Broiler-Human:        PENDING
internal readonly struct JsValue : System.IEquatable<JsValue>
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=48AEDA
    // Broiler-Human:        PENDING
    private readonly double number;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9FC52F
    // Broiler-Human:        PENDING
    private readonly object? reference;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0FC5B3
    // Broiler-Human:        PENDING
    private JsValue(JsType type, double value, object? handle)
    {
        Type = type;
        number = value;
        reference = handle;
    }

    /// <summary>The marker an uninitialised binding holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A767E1
    // Broiler-Human:        PENDING
    internal static JsValue Empty => default;

    /// <summary>The one <c>undefined</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=150FD8
    // Broiler-Human:        PENDING
    internal static JsValue Undefined { get; } = new(JsType.Undefined, 0, null);

    /// <summary>The one <c>null</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F94338
    // Broiler-Human:        PENDING
    internal static JsValue Null { get; } = new(JsType.Null, 0, null);

    /// <summary>The Boolean <c>true</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=50C74C
    // Broiler-Human:        PENDING
    internal static JsValue True { get; } = new(JsType.Boolean, 1, null);

    /// <summary>The Boolean <c>false</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EA8F77
    // Broiler-Human:        PENDING
    internal static JsValue False { get; } = new(JsType.Boolean, 0, null);

    /// <summary>Which kind this value is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5BD727
    // Broiler-Human:        PENDING
    internal JsType Type { get; }

    /// <summary>A Number.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3ABA88
    // Broiler-Human:        PENDING
    internal static JsValue Number(double value) => new(JsType.Number, value, null);

    /// <summary>A Boolean.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=59E756
    // Broiler-Human:        PENDING
    internal static JsValue Boolean(bool value) => value ? True : False;

    /// <summary>A String.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=13DE54
    // Broiler-Human:        PENDING
    internal static JsValue String(string value) => new(JsType.String, 0, value);

    /// <summary>An object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=830888
    // Broiler-Human:        PENDING
    internal static JsValue Object(JsObject value) => new(JsType.Object, 0, value);

    /// <summary>Whether this is the uninitialised-binding marker.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5A93EC
    // Broiler-Human:        PENDING
    internal bool IsEmpty => Type == JsType.Empty;

    /// <summary>Whether this is <c>undefined</c> or <c>null</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=324941
    // Broiler-Human:        PENDING
    internal bool IsNullish => Type is JsType.Undefined or JsType.Null;

    /// <summary>Whether this is an object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8F0CD9
    // Broiler-Human:        PENDING
    internal bool IsObject => Type == JsType.Object;

    /// <summary>Whether this is a String.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=89BFBA
    // Broiler-Human:        PENDING
    internal bool IsString => Type == JsType.String;

    /// <summary>Whether this is a Number.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5DC2D7
    // Broiler-Human:        PENDING
    internal bool IsNumber => Type == JsType.Number;

    /// <summary>The Number this holds. Meaningless unless <see cref="Type"/> is Number.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CEBD86
    // Broiler-Human:        PENDING
    internal double AsNumber() => number;

    /// <summary>The Boolean this holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=083D21
    // Broiler-Human:        PENDING
    internal bool AsBoolean() => number != 0;

    /// <summary>The String this holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=38AAAE
    // Broiler-Human:        PENDING
    internal string AsString() => (string)reference!;

    /// <summary>The object this holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=61AE29
    // Broiler-Human:        PENDING
    internal JsObject AsObject() => (JsObject)reference!;

    /// <summary>The object this holds, or <see langword="null"/> when it is not an object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4FBABA
    // Broiler-Human:        PENDING
    internal JsObject? AsObjectOrNull() => Type == JsType.Object ? (JsObject)reference! : null;

    /// <summary>The abstract operation <c>ToBoolean</c>, which calls nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BF17CE
    // Broiler-Human:        PENDING
    internal bool ToBooleanValue() => Type switch
    {
        JsType.Boolean => number != 0,
        JsType.Number => number != 0 && !double.IsNaN(number),
        JsType.String => ((string)reference!).Length != 0,
        JsType.Object => true,
        _ => false,
    };

    /// <summary>The <c>typeof</c> operator's answer for this value.</summary>
    /// <remarks>
    /// <c>typeof null</c> is <c>"object"</c>. It is a defect of the language that every
    /// implementation reproduces, and reproducing it is the whole job.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=ADEDDC
    // Broiler-Human:        PENDING
    internal string TypeOf() => Type switch
    {
        JsType.Undefined => "undefined",
        JsType.Null => "object",
        JsType.Boolean => "boolean",
        JsType.Number => "number",
        JsType.String => "string",
        _ => ((JsObject)reference!).IsCallable ? "function" : "object",
    };

    /// <summary>
    /// The strict equality comparison, <c>===</c>.
    /// </summary>
    /// <remarks>
    /// NaN is equal to nothing including itself; <c>+0</c> and <c>-0</c> are equal; two objects are
    /// equal when they are the same object and never otherwise; two Strings are equal when their
    /// code-unit sequences are.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=50076E
    // Broiler-Human:        PENDING
    internal bool StrictlyEquals(JsValue other)
    {
        if (Type != other.Type)
        {
            return false;
        }

        return Type switch
        {
            JsType.Undefined or JsType.Null => true,
            JsType.Boolean => number == other.number,
            JsType.Number => number == other.number,
            JsType.String => System.String.Equals((string)reference!, (string)other.reference!, System.StringComparison.Ordinal),
            JsType.Object => ReferenceEquals(reference, other.reference),
            _ => true,
        };
    }

    /// <summary>
    /// The <c>SameValueZero</c> relation, which differs from <c>===</c> only for NaN.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2346D6
    // Broiler-Human:        PENDING
    internal bool SameValueZero(JsValue other) =>
        Type == JsType.Number && other.Type == JsType.Number
            ? (double.IsNaN(number) && double.IsNaN(other.number)) || number == other.number
            : StrictlyEquals(other);

    /// <summary>Representation equality, which is not the language's <c>===</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D58C94
    // Broiler-Human:        PENDING
    internal bool Equals(JsValue other) =>
        Type == other.Type && number.Equals(other.number) && ReferenceEquals(reference, other.reference);

    bool System.IEquatable<JsValue>.Equals(JsValue other) => Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BBD027
    // Broiler-Human:        PENDING
    public override bool Equals(object? obj) => obj is JsValue other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AB7866
    // Broiler-Human:        PENDING
    public override int GetHashCode() => System.HashCode.Combine(Type, number, reference);

    /// <summary>Representation equality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B326CD
    // Broiler-Human:        PENDING
    public static bool operator ==(JsValue left, JsValue right) => left.Equals(right);

    /// <summary>Representation inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5F2FA6
    // Broiler-Human:        PENDING
    public static bool operator !=(JsValue left, JsValue right) => !left.Equals(right);

    /// <summary>The abstract operation <c>ToInt32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D7E592
    // Broiler-Human:        PENDING
    internal static int ToInt32(double value) => unchecked((int)ToUint32(value));

    /// <summary>The abstract operation <c>ToUint32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=273765
    // Broiler-Human:        PENDING
    internal static uint ToUint32(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return 0;
        }

        var truncated = System.Math.Truncate(value) % 4294967296.0;

        if (truncated < 0)
        {
            truncated += 4294967296.0;
        }

        return (uint)truncated;
    }

    /// <summary>The abstract operation <c>ToIntegerOrInfinity</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1769F6
    // Broiler-Human:        PENDING
    internal static double ToInteger(double value)
    {
        if (double.IsNaN(value))
        {
            return 0;
        }

        return double.IsInfinity(value) ? value : System.Math.Truncate(value);
    }
}
