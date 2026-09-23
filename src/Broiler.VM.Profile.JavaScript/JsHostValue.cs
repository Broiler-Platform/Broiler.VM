// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   55
// Annotated:        55/55
// Exempt:           78
// Human-reviewed:   0/55
// IP risk:          Low
// Security risk:    High
// Criteria:         9/9
// Resource impact:  2/10 max
// Unverified:       55
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>What a <see cref="JsHostValue"/> holds.</summary>
/// <remarks>
/// <para>
/// <b>Seven of these are the wide surface's own kinds and two are refinements of one of them.</b>
/// <c>Function</c> and <c>Array</c> are not language types - both are Object - and they are split
/// out because an embedder asks "is this callable" and "is this an array" constantly, and on a
/// surface whose values are opaque to it that question would otherwise cost a crossing. The realm
/// knows the answer when it mints the value, so it answers once, there.
/// </para>
/// <para>
/// <b><see cref="Missing"/> is zero, and it is not <c>undefined</c>.</b> It is what an argument
/// read past the end of an argument list answers, and the distinction matters to anything that
/// resolves on arity: a host method called with no argument and one called with an explicit
/// <c>undefined</c> are different calls. The wide surface numbers its own empty slot zero for the
/// same reason - <c>JsType.Empty</c>, the marker an uninitialised binding holds - so a
/// <c>default</c> value of this struct means "no value" without a constructor having run.
/// </para>
/// <para>
/// <b>BigInt is absent because the surface underneath has no BigInt</b>, not as a policy. A value
/// of that kind is unreachable rather than unhandled, which is the same statement <c>JsType</c>
/// makes one level down.
/// <i>(Amended 2026-09-21. <c>JsType</c> now has a BigInt kind (JSD-0033), admitted with card B05;
/// a program that hands one to the host surface is refused with a <c>TypeError</c> at the
/// crossing. This kind gains a member with card B06.)</i>
/// </para>
/// <para>
/// <i>(Amended 2026-09-22, JSeal B06, decision JSD-0024 section 19.)</i> <see cref="BigInt"/> is
/// the eighth language kind. It is appended rather than inserted, so every value an embedder
/// already stored keeps its number.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8B59F3
// Broiler-Human:        PENDING
public enum JsHostValueKind : byte
{
    /// <summary>No value at all: an argument that was not supplied.</summary>
    Missing = 0,

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

    /// <summary>A Symbol.</summary>
    Symbol = 6,

    /// <summary>An ordinary object: anything not callable and not an Array.</summary>
    Object = 7,

    /// <summary>A callable object.</summary>
    Function = 8,

    /// <summary>An Array exotic object.</summary>
    Array = 9,

    /// <summary>A BigInt: an exact integer, held as a <see cref="System.Numerics.BigInteger"/>.</summary>
    /// <remarks>
    /// A primitive, like a String: it carries no realm and no identity, and two BigInts with the
    /// same mathematical value are the same value. A BigInt OBJECT - what <c>Object(1n)</c> makes -
    /// is an <see cref="Object"/>.
    /// </remarks>
    BigInt = 10,
}

/// <summary>
/// The stable, opaque identity of one guest object or symbol, as an embedder sees it.
/// </summary>
/// <remarks>
/// <para>
/// <b>It has no members, and that is the whole of what it is.</b> An embedder may hold one, compare
/// two by reference, and use one as a dictionary or weak-table key; it may not read anything off
/// one, and there is nothing to read. What it is <em>for</em> is identity: the realm answers the
/// same instance for the same guest object every time, so an embedder that keys a wrapper table on
/// it finds the wrapper it stored, and a script's <c>el === el</c> and a host's
/// <c>ReferenceEquals</c> agree.
/// </para>
/// <para>
/// <b>It holds its object strongly and is held weakly, which is what makes the lifetime work with
/// no release protocol at all.</b> The realm canonicalises through a
/// <see cref="System.Runtime.CompilerServices.ConditionalWeakTable{TKey,TValue}"/> keyed on the
/// guest object, so the table's own reference to this instance is a dependent handle rather than a
/// root: an embedder holding one pins exactly the object it names, dropping it makes both
/// collectable, and nothing anywhere has to be told. A handle table with integer keys - the obvious
/// alternative - would need a release call, and a release call an embedder forgets is a leak the
/// profile cannot see.
/// </para>
/// <para>
/// <b>It carries its realm so that presenting one to a different realm is a refusal rather than a
/// confusion.</b> Two realms in one process is the ordinary case for an embedder with a worker, and
/// a value from one silently accepted by the other would corrupt both.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5A4DC0
// Broiler-Falsified-If: two reads of one guest object answer two instances, or one instance names two guest objects
// Broiler-Human:        PENDING
public sealed class JsHostRef
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=49D14F
    // Broiler-Human:        PENDING
    internal JsHostRef(JsHostRealm realm, object target)
    {
        Realm = realm;
        Target = target;
    }

    /// <summary>The realm that minted it. A ref presented elsewhere is a foreign one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=65204F
    // Broiler-Human:        PENDING
    internal JsHostRealm Realm { get; }

    /// <summary>The guest object or symbol this names, held strongly.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DD1BCD
    // Broiler-Human:        PENDING
    internal object Target { get; }
}

/// <summary>
/// One JavaScript value as an embedder holds it: a kind, a double, and a reference.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is the wide surface's own layout with the reference replaced by an identity.</b>
/// <c>JsValue</c> carries the engine's own object; this carries a <see cref="JsHostRef"/> naming
/// it. The substitution is what keeps the engine's object model out of an embedder's hands while
/// leaving every question an embedder actually asks - what kind is this, is it truthy, what number
/// is it, is it the same object as that one - answerable without crossing back into the realm.
/// </para>
/// <para>
/// <b>The cheap conversions are here and the expensive ones are not.</b>
/// <see cref="AsBoolean"/>, <see cref="AsNumber"/> and <see cref="AsString"/> are decidable from
/// what the struct already holds. The two coercions that can run guest code -
/// <c>ToString</c> on an object may call a <c>toString</c> the guest wrote - are realm members
/// instead, because a host that had to enter the realm to ask whether a value is truthy would do it
/// on every branch of every callback, and because a conversion that can execute guest code should
/// not read like a property access.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BE2021
// Broiler-Human:        PENDING
public readonly struct JsHostValue : System.IEquatable<JsHostValue>
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=48AEDA
    // Broiler-Human:        PENDING
    private readonly double number;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9FC52F
    // Broiler-Human:        PENDING
    private readonly object? reference;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5CDA9D
    // Broiler-Human:        PENDING
    internal JsHostValue(JsHostValueKind kind, double value, object? handle)
    {
        Kind = kind;
        number = value;
        reference = handle;
    }

    /// <summary>No value at all: what an argument past the end of a list answers.</summary>
    /// <remarks>
    /// <b>It never reaches the guest as a value</b> (JSD-0024 section 20). A member that requires a
    /// value - an argument of <see cref="JsHostRealm.Invoke"/> or <see cref="JsHostRealm.Construct"/>,
    /// a value written or defined, an element of <see cref="JsHostRealm.NewArray"/>, a thrown value, a
    /// promise settlement, a clone root, a property target - refuses it with an
    /// <see cref="System.ArgumentException"/>; where the host answers the guest or a value is only
    /// read - a body's return, an exotic hook's answer, a receiver, a conversion - it is
    /// <c>undefined</c>.
    /// </remarks>
    public static JsHostValue Missing => default;

    /// <summary>The one <c>undefined</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=475311
    // Broiler-Human:        PENDING
    public static JsHostValue Undefined { get; } = new(JsHostValueKind.Undefined, 0, null);

    /// <summary>The one <c>null</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7C9BA8
    // Broiler-Human:        PENDING
    public static JsHostValue Null { get; } = new(JsHostValueKind.Null, 0, null);

    /// <summary>Which of the eleven kinds this is.</summary>
    public JsHostValueKind Kind { get; }

    /// <summary>A Boolean.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BA2F93
    // Broiler-Human:        PENDING
    public static JsHostValue Boolean(bool value) =>
        new(JsHostValueKind.Boolean, value ? 1 : 0, null);

    /// <summary>A Number.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=498286
    // Broiler-Human:        PENDING
    public static JsHostValue Number(double value) => new(JsHostValueKind.Number, value, null);

    /// <summary>A String.</summary>
    /// <remarks>
    /// A host may mint one without a realm, because a String is not a realm's object: nothing about
    /// it is per-realm and no identity has to be canonicalised. That is what lets an embedder build
    /// an argument list before it has decided which realm to call into.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=52C438
    // Broiler-Human:        PENDING
    public static JsHostValue String(string value) =>
        new(JsHostValueKind.String, 0, value ?? string.Empty);

    /// <summary>A BigInt of exactly <paramref name="value"/> (JSeal B06).</summary>
    /// <remarks>
    /// <para>
    /// <b>Realm-free, as a String is</b>: a BigInt is a primitive with no identity, so nothing about
    /// it is per-realm and an embedder may build one before it knows which realm it will call.
    /// </para>
    /// <para>
    /// <b>Nothing is checked here, and everything is checked at the crossing.</b> The width the realm
    /// admits - <c>2^20</c> bits, the ceiling every BigInt the language computes answers a
    /// <c>RangeError</c> past - and whether the realm's composition admitted the BigInt surface at
    /// all are properties of a realm, and this has none. A value past the ceiling reaches the realm
    /// as that <c>RangeError</c> before the realm allocates anything for it; a realm that declined
    /// the surface refuses it with <see cref="JsHostRefusal.SurfaceDeclined"/>. The crossing charges
    /// fuel per 64-bit word, in both directions.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F79C3D
    // Broiler-Human:        PENDING
    public static JsHostValue BigInt(System.Numerics.BigInteger value) =>
        new(JsHostValueKind.BigInt, 0, value);

    /// <summary>True when this is <see cref="Missing"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B12B56
    // Broiler-Human:        PENDING
    public bool IsMissing => Kind == JsHostValueKind.Missing;

    /// <summary>True when this is <c>undefined</c> or <c>null</c> or was not supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=985318
    // Broiler-Human:        PENDING
    public bool IsNullish =>
        Kind is JsHostValueKind.Missing or JsHostValueKind.Undefined or JsHostValueKind.Null;

    /// <summary>True when this names a guest object, of any of the three object kinds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=427F98
    // Broiler-Human:        PENDING
    public bool IsObject =>
        Kind is JsHostValueKind.Object or JsHostValueKind.Function or JsHostValueKind.Array;

    /// <summary>The Boolean this holds, or <see langword="false"/> for any other kind.</summary>
    /// <remarks>
    /// It is NOT <c>ToBoolean</c>. The language's truthiness test is a coercion over every kind and
    /// this reads one kind's payload; a host that wants the language's answer asks the realm, where
    /// the name says so.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=CB2DE6
    // Broiler-Human:        PENDING
    public bool AsBoolean() => Kind == JsHostValueKind.Boolean && number != 0;

    /// <summary>The Number this holds, or NaN for any other kind.</summary>
    /// <remarks>
    /// NaN rather than zero for a non-Number, because zero is a number a guest can actually produce
    /// and NaN is the answer the language's own <c>ToNumber</c> gives for the kinds that have none.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=24FDEB
    // Broiler-Human:        PENDING
    public double AsNumber() => Kind == JsHostValueKind.Number ? number : double.NaN;

    /// <summary>The String this holds, or <see langword="null"/> for any other kind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3D7EBB
    // Broiler-Human:        PENDING
    public string? AsString() => Kind == JsHostValueKind.String ? (string)reference! : null;

    /// <summary>The BigInt this holds, or <see langword="null"/> for any other kind.</summary>
    /// <remarks>
    /// Exact: the integer the guest computed, whatever its width. A Number is not a BigInt and
    /// answers <see langword="null"/> here, as a BigInt answers NaN from <see cref="AsNumber"/> -
    /// nothing on this surface converts one into the other.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D432AB
    // Broiler-Human:        PENDING
    public System.Numerics.BigInteger? AsBigInt() =>
        Kind == JsHostValueKind.BigInt ? (System.Numerics.BigInteger)reference! : null;

    /// <summary>
    /// The identity of the guest object or symbol this names, or <see langword="null"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E76337
    // Broiler-Human:        PENDING
    public JsHostRef? AsRef() => reference as JsHostRef;

    /// <summary>The double this carries, whatever the kind, for the realm's own conversion.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E9755A
    // Broiler-Human:        PENDING
    internal double RawNumber => number;

    /// <summary>The reference this carries, whatever the kind, for the realm's own conversion.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=86E09E
    // Broiler-Human:        PENDING
    internal object? RawReference => reference;

    /// <summary>
    /// Identity equality: the language's <c>===</c> for everything this surface can hold.
    /// </summary>
    /// <remarks>
    /// <b>It coincides with <c>===</c> and it is not implemented as it.</b> Two values are equal
    /// when they are the same kind and the same payload, and for an object the payload is a
    /// canonicalised <see cref="JsHostRef"/>, so reference equality on the ref IS object identity in
    /// the realm. The one place the two relations differ is NaN, which this reports equal to itself
    /// and <c>===</c> does not: a host stores these in lists - an argument vector, a listener set -
    /// and a value not equal to itself could not be found in the list it was put into. A host that
    /// wants the language's relation over a Number asks for it in the language's terms.
    /// <para>
    /// <i>(Amended 2026-09-22, JSeal B06.)</i> <b>A BigInt compares by its mathematical value</b>,
    /// because that is what <c>===</c> does and a BigInt has no identity to compare: two crossings
    /// of one guest <c>10n</c>, and a <see cref="BigInt"/> the host built from <c>10</c>, are equal
    /// and hash alike. A BigInt never equals a Number of the same magnitude, as <c>10n === 10</c>
    /// is false. <b>A String still compares by reference</b>, as it did before this amendment: two
    /// equal texts that crossed separately may be unequal here. That is a limitation of this
    /// member, recorded in JSD-0024 section 19 and not changed silently, because an embedder's
    /// existing tables hash on it; a host that needs the language's answer over two Strings compares
    /// <see cref="AsString"/>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A08401
    // Broiler-Human:        PENDING
    public bool Equals(JsHostValue other) =>
        Kind == other.Kind &&
        (Kind == JsHostValueKind.BigInt
            ? ((System.Numerics.BigInteger)reference!).Equals((System.Numerics.BigInteger)other.reference!)
            : number.Equals(other.number) && ReferenceEquals(reference, other.reference));

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is JsHostValue other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=41C3E9
    // Broiler-Human:        PENDING
    public override int GetHashCode() => Kind == JsHostValueKind.BigInt
        ? System.HashCode.Combine(Kind, ((System.Numerics.BigInteger)reference!).GetHashCode())
        : System.HashCode.Combine(
            Kind, number, reference is null ? 0 : System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(reference));

    /// <summary>Identity equality.</summary>
    public static bool operator ==(JsHostValue left, JsHostValue right) => left.Equals(right);

    /// <summary>Identity inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B1E875
    // Broiler-Human:        PENDING
    public static bool operator !=(JsHostValue left, JsHostValue right) => !left.Equals(right);
}

/// <summary>The body of a host-implemented function the guest can call.</summary>
/// <remarks>
/// <para>
/// It receives the realm rather than reading one from anywhere ambient, because a process may hold
/// several and a callback that resolved its own realm from thread state would be right until the
/// day an embedder ran two.
/// </para>
/// <para>
/// <b>An argument past the end of <paramref name="arguments"/> is not supplied</b>, and a body that
/// wants that distinction reads the span's length rather than coercing
/// <see cref="JsHostValue.Missing"/>.
/// </para>
/// <para>
/// <b>A body that answers <see cref="JsHostValue.Missing"/></b> - <c>default</c>, a body with nothing
/// to return - gives the guest <c>undefined</c> (JSD-0024 section 20); a construct body that answers
/// it gives the guest the object the realm made.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=659F51
// Broiler-Human:        PENDING
public delegate JsHostValue JsHostFunction(
    JsHostRealm realm, JsHostValue thisValue, System.ReadOnlySpan<JsHostValue> arguments);

/// <summary>The attributes a member an embedder installs carries.</summary>
/// <remarks>
/// <b>Three bits, and the value-or-accessor distinction is not one of them.</b> Which of the two a
/// member is, is decided by which member of the realm installs it, so encoding it here as well
/// would let the two disagree - a value installed with the accessor bit set is a mistake nothing
/// could catch. What is here is what an interface description actually varies: whether a member is
/// walked by an enumeration, whether it may be redefined, and whether it may be assigned to.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A85D9F
// Broiler-Human:        PENDING
[System.Flags]
public enum JsHostPropertyFlags
{
    /// <summary>Not enumerable, not configurable, not writable.</summary>
    None = 0,

    /// <summary>Walked by <c>for...in</c> and reported by <c>Object.keys</c>.</summary>
    Enumerable = 1,

    /// <summary>May be redefined or deleted.</summary>
    Configurable = 2,

    /// <summary>May be assigned to. Meaningless for an accessor, whose writability is its setter.</summary>
    Writable = 4,

    /// <summary>What an ordinary member of an interface is, and the default everywhere here.</summary>
    Default = Enumerable | Configurable | Writable,
}

/// <summary>Which constructor a host-raised error is built from.</summary>
/// <remarks>
/// The set is the realm's own intrinsic error constructors and nothing else. A host that wants an
/// error type the language does not have builds it as an ordinary object and throws that.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C957EB
// Broiler-Human:        PENDING
public enum JsHostErrorKind
{
    /// <summary><c>Error</c>.</summary>
    Error = 0,

    /// <summary><c>TypeError</c>.</summary>
    TypeError = 1,

    /// <summary><c>RangeError</c>.</summary>
    RangeError = 2,

    /// <summary><c>SyntaxError</c>.</summary>
    SyntaxError = 3,

    /// <summary><c>ReferenceError</c>.</summary>
    ReferenceError = 4,
}

/// <summary>Why the realm refused a crossing.</summary>
/// <remarks>
/// Each of these is a refusal rather than an exception the embedder is expected to catch and
/// interpret: they name a programming error at the seam, and the realm raises them as
/// <see cref="JsHostSurfaceException"/> so that a provider cannot mistake one for a guest throw.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=08DA4A
// Broiler-Human:        PENDING
public enum JsHostRefusal
{
    /// <summary>A value minted by another realm was presented to this one.</summary>
    ForeignRealm = 0,

    /// <summary>The realm was touched outside a step of the instance that owns it.</summary>
    RealmNotCurrent = 1,

    /// <summary>A value of the wrong kind was presented where an object was required.</summary>
    NotAnObject = 2,

    /// <summary>A value that is not callable was presented where a function was required.</summary>
    /// <remarks>
    /// <b>There is deliberately no member here for re-entering too deeply.</b> A guest call made
    /// from host code goes through the interpreter's own call path, which charges the call-depth
    /// ceiling and raises the profile's abort when it is reached - so recursion through this seam
    /// is bounded by the bound that already governs recursion in the guest, and a second one would
    /// be a second answer to one question. A member declared here and raised nowhere would also be
    /// exactly the shape this component keeps a test against: a refusal a reader can find, plan
    /// for, and never observe.
    /// </remarks>
    NotCallable = 3,

    /// <summary>
    /// What was offered for adoption is not a clone carrier this profile build minted (card I17).
    /// </summary>
    /// <remarks>
    /// Another engine's carrier, a carrier from a second copy of this profile, and any other
    /// object all answer this: none is a graph this realm can walk.
    /// </remarks>
    ForeignCarrier = 4,

    /// <summary>
    /// A clone carrier holding transferred bytes was already claimed by an earlier adoption (card I17).
    /// </summary>
    CarrierConsumed = 5,

    /// <summary>
    /// A value of a surface the realm's composition declined was presented: a BigInt, to a realm
    /// whose composition declined <c>broiler.javascript.bigint</c> (card B06).
    /// </summary>
    /// <remarks>
    /// Such a realm holds no BigInt at all - its programs were refused one at verification - so a
    /// host that hands it one has a wiring defect, not a guest to argue with.
    /// </remarks>
    SurfaceDeclined = 6,
}

/// <summary>What a bulk read of an <c>ArrayBuffer</c> found.</summary>
/// <remarks>
/// <para>
/// <b>These are answers about the value, not refusals of the embedder</b>, which is why they are a
/// status rather than members of <see cref="JsHostRefusal"/>. Asking whether an arbitrary value is a
/// buffer is the ordinary question a <c>BufferSource</c>-shaped embedder asks, and "no" is a normal
/// answer to it. What stays a refusal is what always was: a value minted by another realm, or a
/// crossing outside a step.
/// </para>
/// <para>
/// <b>Only <see cref="Copied"/> means bytes were written</b>, and it is also the answer for an
/// empty buffer: a buffer of length zero is a buffer, and its answer is zero bytes copied. A
/// detached buffer is told apart from both. <b>A later version may add members</b> - a shared
/// buffer, when this profile has one, would need its own answer rather than being folded into an
/// existing one - so an embedder treats any value it does not recognise as "nothing was written".
/// </para>
/// <para>
/// <b>A resizable buffer answers exactly as a fixed-length one does, over the bytes it holds at
/// the moment of the call.</b> (Decided 2026-09-21 with JSeal F04-F06, which superseded this
/// remark's earlier expectation that it would need its own member; decision JSD-0024 section 12.)
/// A read is one crossing in which no guest code runs, so the buffer cannot resize between the
/// length being measured and the bytes being copied: <see cref="Copied"/> means the current length
/// was copied, and <see cref="DestinationTooSmall"/> reports the current length, which a later
/// resize may change before the embedder asks again. Nothing about the maximum or the
/// resizability is reported, and the realm keeps no reference to the destination.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F3509A
// Broiler-Human:        PENDING
public enum JsHostBufferStatus
{
    /// <summary>The value is an <c>ArrayBuffer</c>, and all of its bytes were copied.</summary>
    Copied = 0,

    /// <summary>The value is not an <c>ArrayBuffer</c>. Nothing was written.</summary>
    /// <remarks>
    /// A typed array, a <c>DataView</c>, a proxy, an object whose prototype is
    /// <c>ArrayBuffer.prototype</c> and every primitive all answer this: the test is the value's
    /// brand, not anything a guest can write.
    /// </remarks>
    NotAnArrayBuffer = 1,

    /// <summary>The value is an <c>ArrayBuffer</c> whose bytes are gone. Nothing was written.</summary>
    Detached = 2,

    /// <summary>
    /// The value is an <c>ArrayBuffer</c> larger than the destination. Nothing was written.
    /// </summary>
    DestinationTooSmall = 3,
}

/// <summary>A refusal at the host surface: the embedder used the seam wrongly.</summary>
/// <remarks>
/// It is deliberately not the same type as <see cref="JsHostThrowException"/>. A guest throw is
/// data the embedder is expected to handle; this is a defect in the embedder, and giving them one
/// type would let a <c>catch</c> written for the first silently swallow the second.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=93B6FD
// Broiler-Human:        PENDING
public sealed class JsHostSurfaceException : System.Exception
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=972CB1
    // Broiler-Human:        PENDING
    internal JsHostSurfaceException(JsHostRefusal refusal, string message)
        : base(message) => Refusal = refusal;

    /// <summary>Which refusal this is.</summary>
    public JsHostRefusal Refusal { get; }
}

/// <summary>A value the guest threw, carried out to the embedder that called into it.</summary>
/// <remarks>
/// It carries the thrown value rather than a rendering of it, because a DOM-shaped embedder
/// re-throws what it caught and a rendering could not be re-thrown. Rendering it would also run the
/// guest's own <c>toString</c>, which is a coercion the embedder may not want and certainly did not
/// ask for at the moment an exception was propagating.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A1BC05
// Broiler-Human:        PENDING
public sealed class JsHostThrowException : System.Exception
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AA36B8
    // Broiler-Human:        PENDING
    internal JsHostThrowException(JsHostValue thrown, string message)
        : base(message) => Thrown = thrown;

    /// <summary>What the guest threw.</summary>
    public JsHostValue Thrown { get; }
}

/// <summary>
/// The operation ended underneath the embedder, and no host code may continue.
/// </summary>
/// <remarks>
/// <para>
/// <b>It exists because a provider's <c>catch</c> is the one thing that can silently un-enforce a
/// budget.</b> When a guest call raises the profile's own abort - a spent allowance, a cancellation,
/// a call-depth ceiling - that abort is not an error the embedder may handle: the operation is over,
/// and code that carries on is code running outside the allowance a host granted. An embedder
/// writing <c>catch (Exception)</c> around a guest call is ordinary defensive style and would
/// swallow it.
/// </para>
/// <para>
/// <b>So the abort is latched and re-raised.</b> Catching this type does not clear the latch: the
/// realm refuses every subsequent crossing with the same abort, and the profile's own frame raises
/// it again when the host body returns, so the operation still ends the way the core was told it
/// would. What an embedder gets from catching it is the chance to release its own resources, which
/// is the only thing a <c>catch</c> here can honestly be for.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=CECBE0
// Broiler-Falsified-If: an operation whose allowance was spent completes normally because host code caught this
// Broiler-Human:        PENDING
public sealed class JsHostTerminatedException : System.Exception
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5897FA
    // Broiler-Human:        PENDING
    internal JsHostTerminatedException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// What a composition registers to put host objects in a realm.
/// </summary>
/// <remarks>
/// <para>
/// <b>A composition supplies one of these and the profile calls it; there is no other door.</b> The
/// realm is handed over exactly once per instance, at instantiation, and only when the composition
/// also registered the host-surface capability - so the capability table is still the permission,
/// and a composition that registered nothing gets a realm with no host object in it and a program
/// naming one refused before it runs.
/// </para>
/// <para>
/// <b>It is called on the guest's own thread, inside the operation, with the meter live.</b> That is
/// what makes everything it does chargeable to the operation that caused it, and it is why the
/// surface is a callback rather than something an embedder invokes when it likes: a realm reached
/// from outside a step has no meter to charge and no operation to fault.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8FEF7F
// Broiler-Falsified-If: a realm is handed to a surface a composition did not register, or outside a step
// Broiler-Human:        PENDING
public interface IJsHostSurface
{
    /// <summary>Installs whatever this embedder puts in a new realm.</summary>
    /// <param name="realm">The realm, valid only for the duration of this call and later crossings.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3B74E3
    // Broiler-Human:        PENDING
    void OnRealmCreated(JsHostRealm realm);

    /// <summary>
    /// Runs whatever the embedder has been waiting to do, inside a step, on the guest's thread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the door in from outside.</b> The realm is usable only inside a step, and an
    /// embedder holding one between two invocations has no step to be inside. Rather than let it
    /// touch the realm anyway - where there is no meter to charge and no operation to fault - the
    /// profile gives it a way to ASK for a step: the host invokes the reserved entry point
    /// <see cref="JavaScriptProfile.TurnEntryPoint"/>, the profile opens the window, and this runs.
    /// </para>
    /// <para>
    /// <b>It is the embedder's own queue and the profile does not know what is in it.</b> What
    /// arrives here is a turn, not a work item; an embedder that has nothing pending does nothing
    /// and the invocation completes, which is what makes it safe for a host to ask for a turn it
    /// may not need.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A00E82
    // Broiler-Human:        PENDING
    void OnTurn(JsHostRealm realm);
}

/// <summary>
/// A host-completed property lookup, for an object whose members are not a fixed list.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is consulted only when the object's own property storage found nothing, and that order is
/// not negotiable.</b> A live collection that happens to contain an element named <c>item</c> must
/// not shadow its own <c>item()</c> method, and getting the order backwards is silently wrong
/// rather than loudly wrong - the object keeps working and answers the wrong member for exactly the
/// names a page is most likely to have.
/// </para>
/// <para>
/// The one existing exotic subclass in this profile - the String wrapper - consults its own slots
/// FIRST, because <c>length</c> and the index keys genuinely are a String wrapper's own properties.
/// The opposite order here is deliberate and is not an inconsistency: these are named properties,
/// which the specification ranks below ordinary ones.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=FADADB
// Broiler-Falsified-If: a handler is consulted for a name the object's own storage already holds
// Broiler-Human:        PENDING
public interface IJsHostExotic
{
    /// <summary>Answers a name the object's own storage did not hold.</summary>
    /// <remarks>
    /// A hook that answers <see langword="true"/> with <see cref="JsHostValue.Missing"/> gives the
    /// guest a property whose value is <c>undefined</c> (JSD-0024 section 20), as
    /// <see cref="TryGetIndex"/> does.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=430F87
    // Broiler-Human:        PENDING
    bool TryGetNamed(JsHostRealm realm, string name, out JsHostValue value);

    /// <summary>The names this object supports, for enumeration and spread.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=90AC1E
    // Broiler-Human:        PENDING
    System.Collections.Generic.IReadOnlyList<string> SupportedNames(JsHostRealm realm);

    /// <summary>Answers an integer-indexed lookup the object's own storage did not hold.</summary>
    /// <remarks>
    /// <b>Indexed and named lookup are separate members because they are separate questions.</b> A
    /// live collection answers an element read from its contents and a named read from its members'
    /// names, and the two can disagree about whether a key exists. One hook taking a string would
    /// also make every indexed read parse a number back out of a key the engine had just formatted
    /// from one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B12C9F
    // Broiler-Human:        PENDING
    bool TryGetIndex(JsHostRealm realm, uint index, out JsHostValue value);

    /// <summary>
    /// Takes an assignment to a named property, or declines it so ordinary assignment happens.
    /// </summary>
    /// <remarks>
    /// <b>Declining is the common answer and the important one.</b> An embedder's object takes the
    /// names it owns and must let an ordinary expando through untouched, because a guest assigning
    /// a name the embedder does not own is writing TO the object rather than THROUGH it. A hook
    /// that took everything would make every such assignment vanish.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=74DA1F
    // Broiler-Human:        PENDING
    bool TrySetNamed(JsHostRealm realm, string name, JsHostValue value);

    /// <summary>How many integer-indexed elements this object has right now.</summary>
    /// <remarks>
    /// Asked immediately before an enumeration rather than when the object was minted, so a live
    /// collection reports what it holds now. An embedder whose object has no indexed elements
    /// answers zero.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=CBA49F
    // Broiler-Human:        PENDING
    uint IndexedLength(JsHostRealm realm);
}

/// <summary>
/// The deletion half of a host-completed lookup, for an object whose behaviour includes taking a
/// named item away: a storage area, a legacy platform object with a named deleter.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is a second interface rather than a sixth member of <see cref="IJsHostExotic"/>, so no
/// existing handler has to change.</b> A handler that does not implement it is minted and behaves
/// exactly as before: a <c>delete</c> on its object is the ordinary deletion and nothing else. The
/// object asks the question once, when it is minted, so implementing this interface IS the
/// declaration that an object deletes, and a type is a declaration the compiler keeps honest.
/// </para>
/// <para>
/// <b>Which keys arrive here is the language's line and not one this surface invents.</b> A key
/// that is a canonical array index (<c>"7"</c>, but not <c>"007"</c> or <c>"4294967295"</c>) is an
/// index and is never offered; a symbol is never offered; every other string key is a name and is
/// offered exactly once per deletion, whether the guest deleted it with <c>delete</c>, through
/// <c>Reflect.deleteProperty</c> or a <c>Proxy</c> without a trap of its own, or the host deleted
/// it with <see cref="JsHostRealm.DeleteProperty"/>. That matches the named hooks: an index key
/// reaches <see cref="IJsHostExotic.TryGetIndex"/> and is never offered to
/// <see cref="IJsHostExotic.TrySetNamed"/> either.
/// </para>
/// <para>
/// <b>The hook runs BEFORE the ordinary deletion, and the ordinary deletion runs either way.</b> A
/// named deleter has to take the item out before a property mirroring it goes, and a mirror that
/// outlived its item would answer for something the object no longer has. What <c>delete</c>
/// evaluates to is the ordinary deletion's answer, not the handler's: an absent or configurable
/// own property answers <c>true</c>; a non-configurable own property stays, answers <c>false</c>,
/// and throws a <c>TypeError</c> in strict code - after the handler was offered the name, because
/// the offer does not depend on what the object's own storage holds. That is the order the JSeal
/// contract's <c>IJsExoticDelete</c> specifies and both of its providers implement.
/// </para>
/// <para>
/// <b>A handler that throws ends the deletion there.</b> A <see cref="JsHostThrowException"/>
/// reaches the guest as the value it carries, a refusal at the seam reaches it as a
/// <c>TypeError</c>, and a termination re-raises the latched abort - the same translation every
/// host function body gets. In each case the ordinary deletion does not run, so the object's own
/// storage is left as it was.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=7EB180
// Broiler-Falsified-If: an index key or a symbol reaches the hook, or one deletion offers a name twice
// Broiler-Human:        PENDING
public interface IJsHostExoticDeletion
{
    /// <summary>
    /// Takes the deletion of a named property, removing whatever the name stands for, or declines
    /// it so that only the ordinary deletion happens.
    /// </summary>
    /// <remarks>
    /// <b>Declining is a real answer and the common one</b>: a name the object does not own is a
    /// page deleting an expando it put there itself. The answer is the handler's own record of
    /// whether it owned the name; it does not change what <c>delete</c> evaluates to, because the
    /// ordinary deletion runs after it either way.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=175929
    // Broiler-Human:        PENDING
    bool TryDeleteNamed(JsHostRealm realm, string name);
}

/// <summary>What one settlement of a host promise capability did.</summary>
/// <remarks>
/// <b>Two outcomes and no third, because the rest are refusals.</b> A settlement from outside a
/// step, from another thread, after the instance was released, or with a capability or value
/// another realm minted is a mistake at the seam, and it is raised as a
/// <see cref="JsHostSurfaceException"/> rather than answered here - an embedder that could read a
/// misuse as an ordinary outcome would carry on as if its promise had been settled.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A34841
// Broiler-Human:        PENDING
public enum JsHostSettlement
{
    /// <summary>
    /// This call resolved the promise: it is fulfilled or rejected, or it is following a thenable.
    /// </summary>
    Accepted = 0,

    /// <summary>
    /// An earlier call already resolved it, so this one did nothing: no value was read, no
    /// <c>then</c> was looked up, and no reaction was queued.
    /// </summary>
    AlreadyResolved = 1,
}

/// <summary>
/// A genuine promise of one realm, and the right to resolve or reject it, held by an embedder.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is the specification's PromiseCapability with the resolving functions kept on the host
/// side.</b> The promise is an ordinary promise of the realm, built on its own intrinsic
/// <c>Promise.prototype</c> - not through the <c>Promise</c> global, which a guest may replace -
/// and it is settled through the realm's own resolve procedure and reaction queue. The resolving
/// functions are never guest values, so nothing a guest does can settle it and nothing a guest
/// does to <c>Promise</c> changes how it is built.
/// </para>
/// <para>
/// <b>It settles only through <see cref="JsHostRealm.ResolvePromise"/> and
/// <see cref="JsHostRealm.RejectPromise"/>, inside a step, on the guest's thread</b> - which is
/// what keeps a settlement charged to an operation and keeps guest code (a <c>then</c> getter on a
/// resolution value) off an arbitrary CLR thread. An embedder whose work completes elsewhere asks
/// for a turn and settles inside it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5E7572
// Broiler-Falsified-If: a capability settles a promise twice, or settles one outside a step of its own realm
// Broiler-Human:        PENDING
public sealed class JsHostPromiseCapability
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=163E8A
    // Broiler-Human:        PENDING
    internal JsHostPromiseCapability(JsHostRealm realm, JsPromiseObject target, JsHostValue promise)
    {
        Realm = realm;
        Target = target;
        Promise = promise;
    }

    /// <summary>The promise, as the embedder hands it to the guest.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=90EC23
    // Broiler-Human:        PENDING
    public JsHostValue Promise { get; }

    /// <summary>The realm that minted it. A capability presented elsewhere is a foreign one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=65204F
    // Broiler-Human:        PENDING
    internal JsHostRealm Realm { get; }

    /// <summary>The engine's own promise object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4365E8
    // Broiler-Human:        PENDING
    internal JsPromiseObject Target { get; }

    /// <summary>
    /// The specification's <c>[[AlreadyResolved]]</c>, shared by both halves of the capability.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=331D25
    // Broiler-Human:        PENDING
    internal bool AlreadyResolved { get; set; }
}

/// <summary>
/// One module graph a host linked into a realm through <see cref="JsHostRealm.LoadModule"/>: its
/// root's key and namespace, and the handle a host evaluates it by.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is the realm's module and not a copy of it.</b> The key names the one instance the realm
/// holds for it, so a second <see cref="JsHostRealm.LoadModule"/> naming the same module, a static
/// import of it and a guest <c>import()</c> of it all reach the same environment, and
/// <see cref="Namespace"/> is the same object a guest's namespace import sees. The realm answers the
/// same handle for the same key every time.
/// </para>
/// <para>
/// <b>Holding one is not permission to touch the realm.</b> <see cref="Key"/> and
/// <see cref="Namespace"/> are plain data and readable at any time, even after the instance is
/// released; everything that does work - evaluating the graph, reading a binding through the
/// namespace - is a <see cref="JsHostRealm"/> member and is refused outside a step like every other
/// crossing.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E66A7A
// Broiler-Falsified-If: two handles, or two namespaces, exist for one module key in one realm
// Broiler-Human:        PENDING
public sealed class JsHostModule
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B864B3
    // Broiler-Human:        PENDING
    internal JsHostModule(
        JsHostRealm realm, JsProgram program, int index, string key, JsHostValue moduleNamespace)
    {
        Realm = realm;
        Program = program;
        Index = index;
        Key = key;
        Namespace = moduleNamespace;
    }

    /// <summary>The key the composition resolved the module to, exactly as the artifact carries it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7BAFB4
    // Broiler-Human:        PENDING
    public string Key { get; }

    /// <summary>
    /// The module's namespace object: sorted keys, <c>@@toStringTag</c> <c>"Module"</c>, not
    /// extensible, and read through to the live bindings.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=81F597
    // Broiler-Human:        PENDING
    public JsHostValue Namespace { get; }

    /// <summary>The realm that linked it. A handle presented elsewhere is a foreign one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=65204F
    // Broiler-Human:        PENDING
    internal JsHostRealm Realm { get; }

    /// <summary>The verified artifact the graph was linked from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9D1393
    // Broiler-Human:        PENDING
    internal JsProgram Program { get; }

    /// <summary>The root module's index in that artifact.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=82DC37
    // Broiler-Human:        PENDING
    internal int Index { get; }

    /// <summary>
    /// The evaluation promise, made by the first <see cref="JsHostRealm.EvaluateModule"/> and
    /// answered by every later one; <see cref="JsHostValue.Missing"/> until then.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=946015
    // Broiler-Human:        PENDING
    internal JsHostValue Evaluation { get; set; }
}

/// <summary>
/// A guest <c>import()</c> the realm offered an embedder's <see cref="IJsHostModuleLoader"/>, and
/// the right to complete it.
/// </summary>
/// <remarks>
/// <b>It is completed at most once, only in a step of its own realm.</b>
/// <see cref="JsHostRealm.CompleteModuleRequest"/> loads the module through the composition's
/// artifact provider at that moment and settles the import's promise when the graph has finished
/// evaluating; <see cref="JsHostRealm.FailModuleRequest"/> rejects it. Either settles through the
/// realm's job queue and runs no reaction before it returns. A request whose instance was released
/// can never be completed, because no step can open for that realm again.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F7D8EF
// Broiler-Falsified-If: a request settles its promise twice, or settles it outside a step of its own realm
// Broiler-Human:        PENDING
public sealed class JsHostModuleRequest
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=77D194
    // Broiler-Human:        PENDING
    internal JsHostModuleRequest(
        JsHostRealm realm, string referrer, string specifier, JsPromiseObject promise)
    {
        Realm = realm;
        Referrer = referrer;
        Specifier = specifier;
        Target = promise;
    }

    /// <summary>
    /// The calling module's key, or the referrer a classic script was compiled with (empty when it
    /// was compiled with none).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3FC18C
    // Broiler-Human:        PENDING
    public string Referrer { get; }

    /// <summary>The specifier, after the guest's value was converted to a String.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FEEFFE
    // Broiler-Human:        PENDING
    public string Specifier { get; }

    /// <summary>The realm that offered it. A request presented elsewhere is a foreign one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=65204F
    // Broiler-Human:        PENDING
    internal JsHostRealm Realm { get; }

    /// <summary>The import's own promise.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4365E8
    // Broiler-Human:        PENDING
    internal JsPromiseObject Target { get; }

    /// <summary>Whether the loader is still deciding, during which the request cannot be completed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4AFCBE
    // Broiler-Human:        PENDING
    internal bool Offering { get; set; }

    /// <summary>Whether it was completed or failed already.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4B8BDA
    // Broiler-Human:        PENDING
    internal bool Settled { get; set; }
}

/// <summary>
/// A linked module's <c>[[Status]]</c>, as the specification names it (JSeal I11-upstream, JSD-0024
/// section 20).
/// </summary>
/// <remarks>
/// <b>Numbered as the specification lists them</b>, with the specification's <c>~new~</c> left out:
/// a module the realm holds has at least begun linking. Which of them a host can actually observe,
/// and when, is stated on <see cref="JsHostRealm.TryGetModuleState"/>.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4208BE
// Broiler-Human:        PENDING
public enum JsHostModuleStatus
{
    /// <summary>
    /// <c>~unlinked~</c>. Never answered: a module whose graph did not link is not registered, and
    /// <see cref="JsHostRealm.TryGetModuleState"/> answers <see langword="false"/> for it.
    /// </summary>
    Unlinked = 0,

    /// <summary>
    /// <c>~linking~</c>: its environment exists and its declarations are being initialised. The
    /// realm links a graph inside one host operation that runs no guest or host code, so no call can
    /// observe this today; it is answered rather than mislabelled if one ever does.
    /// </summary>
    Linking = 1,

    /// <summary>
    /// <c>~linked~</c>: its declarations are in place and no evaluation has run its body - including
    /// a module a failed evaluation never reached.
    /// </summary>
    Linked = 2,

    /// <summary>
    /// <c>~evaluating~</c>: an evaluation walk has entered it and its strongly connected component
    /// has not completed. Only observable while that walk is running, from host code a module body
    /// calls synchronously.
    /// </summary>
    Evaluating = 3,

    /// <summary>
    /// <c>~evaluating-async~</c>: its component completed, and it or a module it depends on has a
    /// top-level <c>await</c> that has not finished.
    /// </summary>
    EvaluatingAsync = 4,

    /// <summary>
    /// <c>~evaluated~</c>: finished, successfully or with the error
    /// <see cref="JsHostModuleState.EvaluationError"/> holds.
    /// </summary>
    Evaluated = 5,
}

/// <summary>
/// What a realm knows about one linked module at the moment it was asked: its status, its
/// evaluation error, whether it has a top-level <c>await</c>, and its cycle root (JSeal
/// I11-upstream, JSD-0024 section 20).
/// </summary>
/// <remarks>
/// <b>A snapshot, not a view.</b> The realm answers a new one on every
/// <see cref="JsHostRealm.TryGetModuleState"/>, and nothing here changes when the module does.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=60D80C
// Broiler-Human:        PENDING
public sealed class JsHostModuleState
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9516E1
    // Broiler-Human:        PENDING
    internal JsHostModuleState(
        string key, JsHostModuleStatus status, bool hasTopLevelAwait, JsHostValue evaluationError, string cycleRoot)
    {
        Key = key;
        Status = status;
        HasTopLevelAwait = hasTopLevelAwait;
        EvaluationError = evaluationError;
        CycleRoot = cycleRoot;
    }

    /// <summary>The module's key, exactly as the artifact carries it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7BAFB4
    // Broiler-Human:        PENDING
    public string Key { get; }

    /// <summary>The module's <c>[[Status]]</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=ED9E63
    // Broiler-Human:        PENDING
    public JsHostModuleStatus Status { get; }

    /// <summary>
    /// The module's <c>[[HasTLA]]</c>: whether its own body has a top-level <c>await</c> (a top-level
    /// <c>for await</c> or <c>await using</c> included). A module that only imports one that awaits
    /// answers <see langword="false"/>; its status says whether it is still waiting.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D13ED3
    // Broiler-Human:        PENDING
    public bool HasTopLevelAwait { get; }

    /// <summary>
    /// The module's <c>[[EvaluationError]]</c>: the identical value its evaluation threw, which every
    /// later evaluation of it rejects with; <see cref="JsHostValue.Missing"/> while it has none.
    /// </summary>
    /// <remarks>
    /// A module that threw <c>undefined</c> holds <see cref="JsHostValue.Undefined"/> here, which is
    /// why "no error" is Missing and not undefined.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=28B641
    // Broiler-Human:        PENDING
    public JsHostValue EvaluationError { get; }

    /// <summary>Whether <see cref="EvaluationError"/> holds an error.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F85621
    // Broiler-Human:        PENDING
    public bool HasEvaluationError => !EvaluationError.IsMissing;

    /// <summary>
    /// The key of the module's <c>[[CycleRoot]]</c> - the root of the strongly connected component
    /// it was evaluated in, itself when it is not in a cycle - or empty while it has none (it has not
    /// completed a walk, or failed on one).
    /// </summary>
    /// <remarks>
    /// <b>A cycle member is finished only when its root is</b>: a member whose own body ran reads
    /// <see cref="JsHostModuleStatus.Evaluated"/> while its root may still be
    /// <see cref="JsHostModuleStatus.EvaluatingAsync"/>, which is the specification's own state, and
    /// an evaluation of the member answers the root's promise.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D35777
    // Broiler-Human:        PENDING
    public string CycleRoot { get; }
}

/// <summary>What an embedder's loader answers for one guest <c>import()</c>.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=30E5EE
// Broiler-Human:        PENDING
public enum JsHostModuleLoad
{
    /// <summary>
    /// Load it now: the composition's artifact provider is asked in this step, as it would be with
    /// no loader at all.
    /// </summary>
    Now = 0,

    /// <summary>
    /// The embedder keeps the request and completes or fails it from a later step; the import's
    /// promise stays pending until then.
    /// </summary>
    Deferred = 1,
}

/// <summary>
/// An embedder's say over when a guest <c>import()</c> is loaded: now, or later from a turn.
/// </summary>
/// <remarks>
/// <para>
/// <b>Optional, and recognised once.</b> A composition's <see cref="IJsHostSurface"/> that also
/// implements this is taken as the realm's loader when the realm is handed over; one that does not
/// leaves every import answered synchronously by the artifact provider, exactly as before. Like the
/// surface itself, it exists only where the composition registered
/// <see cref="JavaScriptProfile.HostSurfaceCapability"/>.
/// </para>
/// <para>
/// <b>It is offered only what the realm cannot answer on its own.</b> A specifier the calling module
/// already requested statically names a module of the same artifact and is answered without asking.
/// Every other <c>import()</c> is offered here, inside the step the guest is running in, after the
/// specifier was converted and the options were checked; the offer is a charged crossing. The loader
/// must not complete the request it is being offered - doing so throws
/// <see cref="System.InvalidOperationException"/> - and answers <see cref="JsHostModuleLoad.Deferred"/>
/// to complete it later instead. A <see cref="JsHostThrowException"/> it raises rejects the import
/// with the value it carries, and a <see cref="JsHostSurfaceException"/> rejects it with a
/// <c>TypeError</c>.
/// </para>
/// <para>
/// <b>It decides WHEN, never WHAT.</b> Completing a request asks the composition's artifact provider
/// for the module exactly as an undeferred import would, so the provider's answer - and the core's
/// verification of it - is still the only way a module reaches the realm.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F28A83
// Broiler-Falsified-If: a deferred import reaches the realm without the artifact provider and the core's verification
// Broiler-Human:        PENDING
public interface IJsHostModuleLoader
{
    /// <summary>Answers whether one guest <c>import()</c> is loaded now or completed later.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=247610
    // Broiler-Human:        PENDING
    JsHostModuleLoad OnImport(JsHostRealm realm, JsHostModuleRequest request);
}
