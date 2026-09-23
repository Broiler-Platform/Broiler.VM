// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   30
// Annotated:        30/30
// Exempt:           62
// Human-reviewed:   0/30
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  3/10 max
// Unverified:       30
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>What one record of a clone carrier reconstructs as.</summary>
/// <remarks>
/// <b>Every brand is its own kind, and there is no fallback kind.</b> A value whose brand is not
/// named here is refused when it is serialized, so a destination never receives an ordinary object
/// standing in for a Date, a Map or a buffer. The supported-type matrix in decision JSD-0032 is the
/// list this enumeration spells.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=36934C
// Broiler-Human:        PENDING
internal enum JsCloneKind : byte
{
    /// <summary>An ordinary object: own enumerable string-keyed properties, in order.</summary>
    Object = 0,

    /// <summary>An Array: its length and its own enumerable string-keyed properties.</summary>
    Array = 1,

    /// <summary>A Boolean wrapper object.</summary>
    Boolean = 2,

    /// <summary>A Number wrapper object.</summary>
    Number = 3,

    /// <summary>A String wrapper object.</summary>
    String = 4,

    /// <summary>A Date: its time value.</summary>
    Date = 5,

    /// <summary>A RegExp: its original source and flags, and nothing else.</summary>
    RegExp = 6,

    /// <summary>A Map: its entries, in order.</summary>
    Map = 7,

    /// <summary>A Set: its members, in order.</summary>
    Set = 8,

    /// <summary>An Error: a name from the fixed list, a message, and a cause when it had one.</summary>
    Error = 9,

    /// <summary>A fixed-length ArrayBuffer: a copy of its bytes.</summary>
    ArrayBuffer = 10,

    /// <summary>A typed array: its element kind, its buffer's record, offset and length.</summary>
    TypedArray = 11,

    /// <summary>A DataView: its buffer's record, offset and byte length.</summary>
    DataView = 12,

    /// <summary>
    /// A BigInt wrapper object (card B06): its integer, as the one BigInt slot in
    /// <see cref="JsCloneRecord.Values"/>.
    /// </summary>
    BigInt = 13,
}

/// <summary>What a primitive slot of a clone carrier holds.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2729FA
// Broiler-Human:        PENDING
internal enum JsCloneSlotKind : byte
{
    /// <summary><c>undefined</c>.</summary>
    Undefined = 0,

    /// <summary><c>null</c>.</summary>
    Null = 1,

    /// <summary>A Boolean, in <see cref="JsCloneSlot.Number"/> as zero or one.</summary>
    Boolean = 2,

    /// <summary>A Number, bit for bit: <c>-0</c> and NaN survive.</summary>
    Number = 3,

    /// <summary>A String.</summary>
    String = 4,

    /// <summary>An object: an index into the carrier's records.</summary>
    Reference = 5,

    /// <summary>A BigInt, exactly, in <see cref="JsCloneSlot.BigInt"/> (card B06).</summary>
    BigInt = 6,
}

/// <summary>One value inside a clone carrier: a primitive, or the index of an object record.</summary>
/// <remarks>
/// <b>It holds no guest object and no <see cref="JsValue"/>.</b> A string is a CLR string, which is
/// immutable and belongs to no realm; a number is a double; an object is an integer naming a
/// record in the same carrier. That is what lets a carrier outlive the realm that produced it and,
/// as card I17 lets it, cross a thread. A BigInt (card B06) is a <see cref="JsBigInt"/>, which is
/// immutable, names no realm and is shared between realms exactly as a CLR string is.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=42B162
// Broiler-Human:        PENDING
internal readonly struct JsCloneSlot
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=79EB4A
    // Broiler-Human:        PENDING
    private JsCloneSlot(JsCloneSlotKind kind, double number, object? payload, int record)
    {
        Kind = kind;
        Number = number;
        this.payload = payload;
        Record = record;
    }

    /// <summary>The String or the BigInt: one reference serves both, so a slot grows by nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6B0843
    // Broiler-Human:        PENDING
    private readonly object? payload;

    /// <summary>The slot's kind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=68EDF3
    // Broiler-Human:        PENDING
    internal JsCloneSlotKind Kind { get; }

    /// <summary>The Number, or the Boolean as zero or one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BE3731
    // Broiler-Human:        PENDING
    internal double Number { get; }

    /// <summary>The String.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=91BA8A
    // Broiler-Human:        PENDING
    internal string? Text => payload as string;

    /// <summary>The BigInt.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=794698
    // Broiler-Human:        PENDING
    internal JsBigInt? BigInt => payload as JsBigInt;

    /// <summary>The record a reference names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=91F4EF
    // Broiler-Human:        PENDING
    internal int Record { get; }

    /// <summary><c>undefined</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B97DE2
    // Broiler-Human:        PENDING
    internal static JsCloneSlot Undefined => new(JsCloneSlotKind.Undefined, 0, null, -1);

    /// <summary><c>null</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6B841A
    // Broiler-Human:        PENDING
    internal static JsCloneSlot Null => new(JsCloneSlotKind.Null, 0, null, -1);

    /// <summary>A Boolean.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2B5BD8
    // Broiler-Human:        PENDING
    internal static JsCloneSlot Boolean(bool value) => new(JsCloneSlotKind.Boolean, value ? 1 : 0, null, -1);

    /// <summary>A Number.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7C5E70
    // Broiler-Human:        PENDING
    internal static JsCloneSlot NumberOf(double value) => new(JsCloneSlotKind.Number, value, null, -1);

    /// <summary>A String.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5CFBB1
    // Broiler-Human:        PENDING
    internal static JsCloneSlot StringOf(string value) => new(JsCloneSlotKind.String, 0, value, -1);

    /// <summary>A reference to record <paramref name="record"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8FA823
    // Broiler-Human:        PENDING
    internal static JsCloneSlot Reference(int record) => new(JsCloneSlotKind.Reference, 0, null, record);

    /// <summary>A BigInt.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B14D7A
    // Broiler-Human:        PENDING
    internal static JsCloneSlot BigIntOf(JsBigInt value) => new(JsCloneSlotKind.BigInt, 0, value, -1);
}

/// <summary>One object of a clone graph, as data.</summary>
/// <remarks>
/// <para>
/// <b>One shape serves every kind, and each kind reads only the fields it names.</b> The fields are
/// written while the serializer builds the record and never again: the carrier that holds the
/// record is handed out only once serialization has finished, and nothing in the profile writes a
/// record it did not just create.
/// </para>
/// <para>
/// <b>Keys and values are parallel arrays</b> for an ordinary object or an Array; a Map's
/// <see cref="Values"/> interleaves key and value; a Set's holds the members; an Error's holds the
/// cause, when <see cref="HasCause"/> says it had one.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=885418
// Broiler-Human:        PENDING
internal sealed class JsCloneRecord
{
    /// <summary>Creates an empty record of <paramref name="kind"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3EAA92
    // Broiler-Human:        PENDING
    internal JsCloneRecord(JsCloneKind kind) => Kind = kind;

    /// <summary>What the record reconstructs as.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9EBE12
    // Broiler-Human:        PENDING
    internal JsCloneKind Kind { get; }

    /// <summary>
    /// The number the kind carries: an Array's length, a Date's time value, a wrapped Number or
    /// Boolean.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E8CA0D
    // Broiler-Human:        PENDING
    internal double Number { get; set; }

    /// <summary>
    /// The text the kind carries: a wrapped String, a RegExp's source, an Error's name, a typed
    /// array's constructor name.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8DB435
    // Broiler-Human:        PENDING
    internal string? Text { get; set; }

    /// <summary>A RegExp's flags, or an Error's message; <see langword="null"/> for an Error with none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CA84D1
    // Broiler-Human:        PENDING
    internal string? Detail { get; set; }

    /// <summary>Whether an Error carried a <c>cause</c>, which is then the one value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8B9C27
    // Broiler-Human:        PENDING
    internal bool HasCause { get; set; }

    /// <summary>The property keys of an ordinary object or an Array.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7B5074
    // Broiler-Human:        PENDING
    internal string[] Keys { get; set; } = [];

    /// <summary>The values: properties, Map entries, Set members or an Error's cause.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E11F1B
    // Broiler-Human:        PENDING
    internal JsCloneSlot[] Values { get; set; } = [];

    /// <summary>
    /// An ArrayBuffer's bytes: a copy the source realm cannot reach, or, for a transferred buffer,
    /// the source's own array, which the source let go of when it was detached.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=58AF35
    // Broiler-Human:        PENDING
    internal byte[]? Bytes { get; set; }

    /// <summary>
    /// Whether an ArrayBuffer record holds MOVED bytes: the buffer was in the transfer list, its
    /// source is detached, and the carrier is the bytes' only owner (decision JSD-0032 section 5a).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=25F2A6
    // Broiler-Human:        PENDING
    internal bool Transferred { get; set; }

    /// <summary>A view's buffer, as a record index.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=979F86
    // Broiler-Human:        PENDING
    internal int Buffer { get; set; } = -1;

    /// <summary>A view's byte offset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=63EF2A
    // Broiler-Human:        PENDING
    internal int Offset { get; set; }

    /// <summary>A typed array's element count, or a DataView's byte length.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=90D86B
    // Broiler-Human:        PENDING
    internal int Count { get; set; }
}

/// <summary>
/// A detached clone graph: records and a root, holding data and no live object of any realm.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is internal, and that is decision JSD-0032's first clause.</b> An embedder holds one only
/// inside the public <see cref="JsHostCloneCarrier"/> (card I17), which exposes none of its
/// records; the slice-compiler composition's clone checks reach the internal members directly.
/// </para>
/// <para>
/// <b>It may be adopted more than once, and every adoption builds new objects.</b> Nothing in a
/// carrier is handed to a destination by reference except CLR strings, which are immutable: a
/// buffer's bytes are copied again on every adoption, so two adoptions of one carrier share nothing
/// a guest can write. <b>The exception is a carrier that holds moved bytes</b> (card I16): bytes a
/// transfer took from a detached source can arrive in one destination only, so such a carrier is
/// claimed by its first adoption and refuses every later one (<see cref="JsCloneRefusal.Consumed"/>).
/// The public rule is the same (card I17): <see cref="JsHostCloneCarrier"/> wraps exactly one of
/// these.
/// </para>
/// <para>
/// <b>Its size is bounded when it is built.</b> <see cref="MaxEntries"/> caps records and value
/// slots together (a key travels with its slot) and <see cref="MaxBytes"/> caps buffer bytes and the
/// text of each distinct string instance, so a carrier is never larger than those two numbers however
/// the guest shaped its graph. The walk's scratch (an object's key list, a Map's or Set's snapshot) is
/// not the carrier, and is built before its entries are counted.
/// </para>
/// <para>
/// <b>The sender pays for it (card I17).</b> Serialization reports <see cref="Charged"/> to the
/// sending instance's <c>LiveBytes</c> before anything is detached, and a ceiling that refuses it
/// ends the operation. Like every retention in this profile it is never credited while that
/// instance lives, and it is given back with the rest of the instance's live bytes when the
/// instance is disposed; from then on a carrier the host still holds is host memory, bounded by the
/// two maxima and by nothing else. The receiver pays for what adoption builds.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=D07D0E
// Broiler-Falsified-If: a carrier holds a JsObject, a JsValue or any other reference into the realm that produced it
// Broiler-Human:        PENDING
internal sealed class JsCloneCarrier
{
    /// <summary>The carrier layout this build writes and reads.</summary>
    /// <remarks>
    /// Two since card B06, which added the BigInt slot and record: a reader of layout one would not
    /// know them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=268D70
    // Broiler-Human:        PENDING
    internal const int FormatVersion = 2;

    /// <summary>The most records and value slots one carrier may hold, together.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=8888D4
    // Broiler-Falsified-If: a serialization completes with more entries than this
    // Broiler-Human:        PENDING
    internal const long MaxEntries = 1L << 22;

    /// <summary>The most buffer bytes and string bytes (per distinct string instance) one carrier may hold.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=7362FC
    // Broiler-Falsified-If: a serialization completes holding more bytes than this
    // Broiler-Human:        PENDING
    internal const long MaxBytes = 1L << 28;

    /// <summary>Seals a finished graph.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=72AEB6
    // Broiler-Human:        PENDING
    internal JsCloneCarrier(
        JsCloneRecord[] records,
        JsCloneSlot root,
        long entries,
        long bytes,
        int moved = 0,
        long charged = 0,
        bool holdsBigInt = false)
    {
        Records = records;
        Root = root;
        Entries = entries;
        Bytes = bytes;
        Moved = moved;
        Charged = charged;
        HoldsBigInt = holdsBigInt;
    }

    /// <summary>
    /// Whether any slot is a BigInt, so that a realm whose composition declined the BigInt surface
    /// refuses the carrier before it claims or builds anything (card B06).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FC89FE
    // Broiler-Human:        PENDING
    internal bool HoldsBigInt { get; }

    /// <summary>What one entry (a record or a value slot) is reported to <c>LiveBytes</c> as.</summary>
    /// <remarks>
    /// A slot is a kind, a double, a string reference and an index; a record is an object with a
    /// handful of fields and two arrays. Thirty-two bytes per entry is an estimate within a small
    /// factor of both, which is what a ceiling needs: an entry that cost nothing to report would
    /// make a four-million-entry carrier free.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A77C4A
    // Broiler-Human:        PENDING
    internal const long EntryBytes = 32;

    /// <summary>
    /// The live bytes a carrier of <paramref name="entries"/> entries and
    /// <paramref name="copiedBytes"/> copied buffer and string bytes is charged to its sender.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=ECD920
    // Broiler-Falsified-If: the charge does not grow with the carrier's entries and copied bytes
    // Broiler-Human:        PENDING
    internal static long ChargeFor(long entries, long copiedBytes) =>
        (entries * EntryBytes) + copiedBytes;

    /// <summary>What the sending instance's <c>LiveBytes</c> was charged for this carrier.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C3B255
    // Broiler-Human:        PENDING
    internal long Charged { get; }

    /// <summary>Whether an adoption has claimed this carrier's moved bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E1E0E6
    // Broiler-Human:        PENDING
    internal bool Claimed => System.Threading.Volatile.Read(ref claimed) != 0;

    /// <summary>Set once the one adoption a carrier with moved bytes allows has claimed them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F0E9BD
    // Broiler-Human:        PENDING
    private int claimed;

    /// <summary>The object records, in the order the serializer first met each object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=75736B
    // Broiler-Human:        PENDING
    internal JsCloneRecord[] Records { get; }

    /// <summary>The value that was cloned.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D444C8
    // Broiler-Human:        PENDING
    internal JsCloneSlot Root { get; }

    /// <summary>How many records, keys and values the carrier holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E67AD2
    // Broiler-Human:        PENDING
    internal long Entries { get; }

    /// <summary>How many buffer and string bytes the carrier holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=37EE5F
    // Broiler-Human:        PENDING
    internal long Bytes { get; }

    /// <summary>How many transferred buffers the carrier holds; nonzero makes it single-use.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E1C253
    // Broiler-Human:        PENDING
    internal int Moved { get; }

    /// <summary>
    /// Claims the moved bytes for one adoption, answering whether this caller is the first.
    /// </summary>
    /// <remarks>
    /// <b>Atomic, because carriers cross threads (card I17)</b>: two adoptions racing for the
    /// same moved bytes must not both win. A claim is final even if the adoption that made it then
    /// fails; moved bytes are never offered twice.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D7D2C4
    // Broiler-Falsified-If: two adoptions of one carrier holding moved bytes both succeed
    // Broiler-Human:        PENDING
    internal bool TryClaimMoved() =>
        System.Threading.Interlocked.Exchange(ref claimed, 1) == 0;
}

/// <summary>Why a value could not be cloned.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=48B61B
// Broiler-Human:        PENDING
internal enum JsCloneRefusal : byte
{
    /// <summary>A Symbol, as a value or wrapped in an object.</summary>
    Symbol = 0,

    /// <summary>A callable object.</summary>
    Function = 1,

    /// <summary>A Proxy, callable or not.</summary>
    Proxy = 2,

    /// <summary>An object whose brand is not in the supported-type matrix.</summary>
    UnsupportedBrand = 3,

    /// <summary>An ArrayBuffer whose bytes are gone, or a view over one.</summary>
    DetachedBuffer = 4,

    /// <summary>The graph is larger than <see cref="JsCloneCarrier.MaxEntries"/> or <see cref="JsCloneCarrier.MaxBytes"/> allow.</summary>
    TooLarge = 5,

    /// <summary>The destination cannot reconstruct a record: a kind or element type it lacks.</summary>
    Unrepresentable = 6,

    /// <summary>A transfer-list entry that is not an ArrayBuffer this carrier can move.</summary>
    NotTransferable = 7,

    /// <summary>A transfer list naming one buffer twice.</summary>
    DuplicateTransfer = 8,

    /// <summary>A carrier whose moved bytes an earlier adoption already claimed.</summary>
    Consumed = 9,
}

/// <summary>
/// The clone refusal: the profile's equivalent of HTML's <c>DataCloneError</c>.
/// </summary>
/// <remarks>
/// <b>It is a CLR exception and not a guest throw, deliberately.</b> This realm has no
/// <c>DOMException</c>, so there is no guest value that IS a <c>DataCloneError</c>; which guest
/// error a refusal becomes is a decision for whoever exposes cloning. The internal bridge on
/// <see cref="JsHostRealm"/> raises a <c>TypeError</c> whose message begins
/// <c>DataCloneError:</c>, and nothing else in the profile sees one. A getter's own throw is NOT
/// this: it propagates as the guest exception it was, exactly as HTML's <c>?</c> says.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E0AC86
// Broiler-Human:        PENDING
internal sealed class JsCloneRefusedException : System.Exception
{
    /// <summary>Creates a refusal.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BB6F92
    // Broiler-Human:        PENDING
    internal JsCloneRefusedException(JsCloneRefusal reason, string message)
        : base(message) => Reason = reason;

    /// <summary>Why the value was refused.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B01513
    // Broiler-Human:        PENDING
    internal JsCloneRefusal Reason { get; }
}

/// <summary>
/// A structured clone that belongs to no realm, as an embedder holds it: what
/// <see cref="JsHostRealm.DetachClone"/> answers on the sending thread and
/// <see cref="JsHostRealm.AdoptClone"/> takes on the receiving one.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is opaque, and it holds data only</b> (decision JSD-0032, card I17). The graph inside is
/// records, numbers, CLR strings and byte arrays; no guest object, no <see cref="JsHostValue"/> and
/// no realm or instance handle. Nothing here exposes the graph, so a host can carry one and hand it
/// to <see cref="JsHostRealm.AdoptClone"/>, and can do nothing else with it.
/// </para>
/// <para>
/// <b>Lifetime.</b> A carrier is sealed when <see cref="JsHostRealm.DetachClone"/> returns and is
/// never written again, except for the one-way claim below. It stays valid after the sending
/// realm's instance and runtime are disposed, for as long as the host keeps a reference to it.
/// </para>
/// <para>
/// <b>Threads.</b> It may cross threads, and may be read by several at once. Serialization runs on
/// the sending realm's thread inside one of its steps; adoption runs on the receiving realm's thread
/// inside one of its steps, and builds every object there. The host hands a carrier from one thread
/// to the other through any synchronising handoff (a thread-safe queue, a task's result, a lock);
/// nothing more is required, because nothing in it changes after that handoff except the claim,
/// which is atomic.
/// </para>
/// <para>
/// <b>Single use or repeatable.</b> A carrier made without a transfer list may be adopted any number
/// of times, into one realm or several, concurrently or not; each adoption builds new objects and
/// copies the bytes again. A carrier holding transferred bytes (<see cref="IsSingleUse"/>) is
/// claimed, atomically, by the first adoption that reaches it; every later adoption is refused with
/// <see cref="JsHostRefusal.CarrierConsumed"/>, and a claim is final even when the adoption that
/// made it then fails.
/// </para>
/// <para>
/// <b>Compatibility.</b> A carrier is adoptable only by a realm of the profile build that minted it:
/// the same <see cref="Profile"/>, the same <see cref="FormatVersion"/>, and the same type, which a
/// second copy of this assembly would not share. Anything else offered to
/// <see cref="JsHostRealm.AdoptClone"/> - another engine's carrier, another build's, any other
/// object - is refused with <see cref="JsHostRefusal.ForeignCarrier"/>. There is no byte form: a
/// carrier does not outlive the process.
/// </para>
/// <para>
/// <b>Cost.</b> The sending instance's <c>LiveBytes</c> is charged <see cref="ChargedBytes"/> when
/// the carrier is made, and keeps it until that instance is disposed, as it keeps every other
/// retention; after that a carrier the host still holds is host memory, bounded per carrier and by
/// nothing else. The receiving instance pays fuel for what adoption builds and <c>LiveBytes</c> for
/// every buffer it rebuilds.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=B2EC01
// Broiler-Falsified-If: a carrier exposes its graph, holds a reference into a realm, or is adopted by a realm of another profile build
// Broiler-Human:        PENDING
public sealed class JsHostCloneCarrier
{
    /// <summary>Wraps a sealed internal carrier.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=82B44D
    // Broiler-Human:        PENDING
    internal JsHostCloneCarrier(JsCloneCarrier graph) => Graph = graph;

    /// <summary>The carrier layout this build writes, and the only one it adopts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=13CD3C
    // Broiler-Human:        PENDING
    public static int CurrentFormatVersion => JsCloneCarrier.FormatVersion;

    /// <summary>The profile whose realm minted the carrier.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=152F9D
    // Broiler-Human:        PENDING
    public VmProfileId Profile => JavaScriptProfile.Id;

    /// <summary>The carrier layout it was written in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0ED30B
    // Broiler-Human:        PENDING
    public int FormatVersion => JsCloneCarrier.FormatVersion;

    /// <summary>Whether it holds transferred bytes, and so may be adopted once only.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2886BB
    // Broiler-Human:        PENDING
    public bool IsSingleUse => Graph.Moved > 0;

    /// <summary>Whether it is single-use and an adoption has already claimed it.</summary>
    /// <remarks>
    /// A snapshot: another thread may claim the carrier the moment after this answers
    /// <see langword="false"/>, so only the refusal from <see cref="JsHostRealm.AdoptClone"/> is
    /// authoritative.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F5176A
    // Broiler-Human:        PENDING
    public bool IsConsumed => Graph.Moved > 0 && Graph.Claimed;

    /// <summary>What the sending instance's <c>LiveBytes</c> was charged for the carrier.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2A311D
    // Broiler-Human:        PENDING
    public long ChargedBytes => Graph.Charged;

    /// <summary>The graph.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=944D14
    // Broiler-Human:        PENDING
    internal JsCloneCarrier Graph { get; }
}
