// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   37
// Annotated:        37/37
// Exempt:           32
// Human-reviewed:   0/37
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  2/10 max
// Unverified:       37
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
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D452DA
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
    public static JsHostValue Missing => default;

    /// <summary>The one <c>undefined</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=475311
    // Broiler-Human:        PENDING
    public static JsHostValue Undefined { get; } = new(JsHostValueKind.Undefined, 0, null);

    /// <summary>The one <c>null</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7C9BA8
    // Broiler-Human:        PENDING
    public static JsHostValue Null { get; } = new(JsHostValueKind.Null, 0, null);

    /// <summary>Which of the ten kinds this is.</summary>
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
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B6680A
    // Broiler-Human:        PENDING
    public bool Equals(JsHostValue other) =>
        Kind == other.Kind &&
        number.Equals(other.number) &&
        ReferenceEquals(reference, other.reference);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is JsHostValue other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B343DB
    // Broiler-Human:        PENDING
    public override int GetHashCode() => System.HashCode.Combine(
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
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=659F51
// Broiler-Human:        PENDING
public delegate JsHostValue JsHostFunction(
    JsHostRealm realm, JsHostValue thisValue, System.ReadOnlySpan<JsHostValue> arguments);

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
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C088FA
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
