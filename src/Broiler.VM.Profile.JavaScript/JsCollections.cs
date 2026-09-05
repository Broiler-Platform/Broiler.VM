// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   51
// Annotated:        51/51
// Exempt:           36
// Human-reviewed:   0/51
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  4/10 max
// Unverified:       51
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>SameValueZero</c> relation, in the shape a hash table can key on.
/// </summary>
/// <remarks>
/// <para>
/// <b>Three relations exist in this language and a keyed collection uses the third one.</b>
/// <c>===</c> says <c>NaN !== NaN</c>; <c>Object.is</c> says <c>+0</c> and <c>-0</c> differ;
/// <c>SameValueZero</c> - the one <c>Map</c>, <c>Set</c> and <c>Array.prototype.includes</c> use -
/// says a <c>NaN</c> key finds itself and that the two zeroes are one key. Picking either of the
/// other two here is not a near miss: keying on <c>===</c> makes <c>m.set(NaN, 1).get(NaN)</c>
/// answer <c>undefined</c> and lets a program fill a table with unreachable entries, and keying on
/// <c>Object.is</c> makes <c>-0</c> and <c>0</c> two slots that every arithmetic path in the guest
/// treats as one.
/// </para>
/// <para>
/// <b>The hash is computed on a CANONICAL form rather than on the value's own bits, and that is
/// the whole reason this type exists rather than a lambda.</b> Two values this comparer calls
/// equal must hash alike or the dictionary loses them, and both of the interesting cases -
/// every <c>NaN</c> payload, and the two signed zeroes - are pairs of DIFFERENT bit patterns that
/// must land in one bucket. <see cref="Canonical"/> folds them before <c>GetHashCode</c> ever sees
/// them, so the agreement is a property of this file and not of how the runtime happens to hash a
/// <c>double</c> this year.
/// </para>
/// <para>
/// <b>An object key hashes on its identity and never on its contents.</b>
/// <see cref="System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(object)"/> is used
/// explicitly rather than the object's own <c>GetHashCode</c>, because a future
/// <see cref="JsObject"/> subclass that overrode the latter would silently make two distinct
/// guest objects one Map key - a bug with no symptom until a program put both in a table.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8DA3AD
// Broiler-Human:        PENDING
internal sealed class JsSameValueZero : System.Collections.Generic.IEqualityComparer<JsValue>
{
    /// <summary>The one instance: the comparer holds no state and needs no second.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=688CCB
    // Broiler-Human:        PENDING
    internal static JsSameValueZero Instance { get; } = new();

    /// <summary>Whether two values are the same key.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F82E57
    // Broiler-Human:        PENDING
    public bool Equals(JsValue left, JsValue right) => left.SameValueZero(right);

    /// <summary>A hash that agrees with <see cref="Equals"/> on every pair it calls equal.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=83BB32
    // Broiler-Human:        PENDING
    public int GetHashCode(JsValue value) => value.Type switch
    {
        JsType.Number => System.HashCode.Combine(JsType.Number, Canonical(value.AsNumber())),
        JsType.String => System.HashCode.Combine(
            JsType.String, System.StringComparer.Ordinal.GetHashCode(value.AsString())),
        JsType.Boolean => System.HashCode.Combine(JsType.Boolean, value.AsBoolean()),
        JsType.Object => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(value.AsObject()),
        _ => (int)value.Type,
    };

    /// <summary>
    /// The one bit pattern every <c>SameValueZero</c>-equal Number is hashed through.
    /// </summary>
    /// <remarks>
    /// Every <c>NaN</c> becomes the quiet one and <c>-0</c> becomes <c>+0</c>, so the hash of a key
    /// is a function of the equivalence class and not of the member. Nothing else is touched: a
    /// subnormal, an infinity and an integer all hash as themselves.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=937CDE
    // Broiler-Human:        PENDING
    internal static double Canonical(double value)
    {
        if (double.IsNaN(value))
        {
            return double.NaN;
        }

        return value == 0 ? 0 : value;
    }
}

/// <summary>
/// The insertion-ordered, <c>SameValueZero</c>-keyed table both <c>Map</c> and <c>Set</c> are.
/// </summary>
/// <remarks>
/// <para>
/// <b>A list of entries plus a dictionary from key to slot, with a tombstone on delete.</b> The
/// language asks this structure for three things at once and no single container gives all three.
/// Insertion order must survive a delete, so the order cannot be the dictionary's. Lookup must be
/// constant time, so the order cannot be a list alone. And - the requirement that decides the
/// shape - an iteration in progress MUST see entries added after it started and MUST NOT see
/// entries deleted after it started, which the specification states as an explicit obligation on
/// <c>forEach</c> and on every iterator. A structure that compacted on delete would renumber the
/// survivors under the walking cursor and skip one; a structure that snapshotted the entries would
/// hide an append. Tombstoning in place and walking by rising slot index does both correctly, and
/// it does them without a single allocation on the iteration path.
/// </para>
/// <para>
/// <b>A deleted key leaves the dictionary immediately and the list eventually.</b> Removing the
/// key at once is what makes a re-added key take a NEW slot at the end, which is the visible
/// behaviour: <c>m.delete(k); m.set(k, v)</c> moves <c>k</c> to the back of the iteration order,
/// and an iteration already running visits it again. The tombstone in the list is what keeps every
/// slot index a survivor still holds stable while that happens.
/// </para>
/// <para>
/// <b>The tombstones ARE collected, but only where nobody is looking.</b>
/// <see cref="Compact"/> runs when no iteration is in flight and the dead outnumber the living;
/// during an iteration it does nothing at all, because compaction is exactly the renumbering the
/// cursor cannot survive. The specification's own model never compacts - it keeps an empty record
/// for ever - and an implementation that copied that literally would let
/// <c>while (true) { m.set(k, 1); m.delete(k); }</c> grow a list without bound while the table
/// stayed at one entry. The fuel charged per <c>set</c> bounds how fast that can happen and the
/// compaction bounds how far.
/// </para>
/// <para>
/// <b>What is NOT here: iterators.</b> There is no <c>keys</c>, <c>values</c>, <c>entries</c> or
/// <c>[Symbol.iterator]</c> on this table, because those are objects with a <c>next</c> method and
/// a realm to hang off, which a table has no business knowing about. The realm builds them in
/// <c>JsRealm.CollectionIterators.cs</c> over the slot walk below, so that <c>forEach</c> and
/// every iterator read the same walk rather than two that can disagree.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=720256
// Broiler-Human:        PENDING
internal sealed class JsKeyedTable
{
    /// <summary>Below this many slots the tombstones are not worth a rebuild.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=697D23
    // Broiler-Human:        PENDING
    private const int CompactionFloor = 8;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B7E975
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<Entry> entries = [];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=D23B09
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<JsValue, int> slots =
        new(JsSameValueZero.Instance);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=FB4463
    // Broiler-Human:        PENDING
    private int living;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=420E1D
    // Broiler-Human:        PENDING
    private int walking;

    /// <summary>How many entries are alive: the collection's <c>size</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=713051
    // Broiler-Human:        PENDING
    internal int Count => living;

    /// <summary>
    /// How many slots the walk must cover, tombstones included. Re-read on every step of an
    /// iteration, which is what makes an entry added during one visible to it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9DB289
    // Broiler-Human:        PENDING
    internal int SlotCount => entries.Count;

    /// <summary>
    /// The key and value at <paramref name="slot"/>, or <see langword="false"/> for a tombstone.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=833059
    // Broiler-Human:        PENDING
    internal bool TryAt(int slot, out JsValue key, out JsValue value)
    {
        var entry = entries[slot];
        key = entry.Key;
        value = entry.Value;
        return entry.Living;
    }

    /// <summary>Reads the value stored under <paramref name="key"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C7A00B
    // Broiler-Human:        PENDING
    internal bool TryGet(JsValue key, out JsValue value)
    {
        if (slots.TryGetValue(Normalise(key), out var slot))
        {
            value = entries[slot].Value;
            return true;
        }

        value = JsValue.Undefined;
        return false;
    }

    /// <summary>Whether the table holds <paramref name="key"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=61DE72
    // Broiler-Human:        PENDING
    internal bool Has(JsValue key) => slots.ContainsKey(Normalise(key));

    /// <summary>
    /// Stores <paramref name="value"/> under <paramref name="key"/>, keeping an existing key where
    /// it already is.
    /// </summary>
    /// <remarks>
    /// Overwriting in place rather than re-appending is observable and the specification says so:
    /// <c>m.set("a", 1); m.set("b", 2); m.set("a", 3)</c> iterates <c>a</c> before <c>b</c>. Only a
    /// key that was never there, or one a <c>delete</c> removed, reaches the end of the list.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A24157
    // Broiler-Human:        PENDING
    internal void Set(JsValue key, JsValue value)
    {
        var normalised = Normalise(key);

        if (slots.TryGetValue(normalised, out var slot))
        {
            entries[slot] = new Entry(normalised, value, true);
            return;
        }

        slots[normalised] = entries.Count;
        entries.Add(new Entry(normalised, value, true));
        living++;
    }

    /// <summary>Removes <paramref name="key"/>, answering whether it was there.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=586DA6
    // Broiler-Human:        PENDING
    internal bool Delete(JsValue key)
    {
        var normalised = Normalise(key);

        if (!slots.Remove(normalised, out var slot))
        {
            return false;
        }

        // THE TOMBSTONE HOLDS NOTHING. Leaving the key and the value in a dead slot would keep two
        // guest objects reachable from a table that no longer claims to hold them, which is the
        // difference between "deleted" and "invisible".
        entries[slot] = new Entry(JsValue.Undefined, JsValue.Undefined, false);
        living--;
        Compact();
        return true;
    }

    /// <summary>Empties the table.</summary>
    /// <remarks>
    /// Every slot becomes a tombstone rather than the list becoming shorter, so a
    /// <c>forEach</c> whose callback called <c>clear</c> walks off the end of a table of dead slots
    /// instead of indexing past a list that shrank under it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C027A2
    // Broiler-Human:        PENDING
    internal void Clear()
    {
        for (var slot = 0; slot < entries.Count; slot++)
        {
            entries[slot] = new Entry(JsValue.Undefined, JsValue.Undefined, false);
        }

        slots.Clear();
        living = 0;
        Compact();
    }

    /// <summary>Marks an iteration as running, so nothing renumbers the slots under it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9FD035
    // Broiler-Human:        PENDING
    internal void EnterIteration() => walking++;

    /// <summary>Ends one iteration and collects the tombstones when the last one leaves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8CE06D
    // Broiler-Human:        PENDING
    internal void ExitIteration()
    {
        walking--;
        Compact();
    }

    /// <summary>
    /// The key a value is stored under: <c>-0</c> becomes <c>+0</c> and everything else is itself.
    /// </summary>
    /// <remarks>
    /// This is a STORAGE rule and not only a lookup rule, which is why it is applied on the way in
    /// as well as on the way out. <c>new Map([[-0, "a"]]).forEach(function (v, k) { ... })</c>
    /// hands the callback <c>+0</c>, so <c>1 / k</c> is <c>Infinity</c>; a table that normalised
    /// only its lookups would answer <c>-Infinity</c> and be wrong in the one place the difference
    /// can be seen.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8EF10C
    // Broiler-Human:        PENDING
    internal static JsValue Normalise(JsValue key) =>
        key.Type == JsType.Number && key.AsNumber() == 0 ? JsValue.Number(0) : key;

    /// <summary>Rebuilds the list without its tombstones, when it is safe to renumber.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B4525F
    // Broiler-Human:        PENDING
    private void Compact()
    {
        if (walking != 0 || entries.Count < CompactionFloor || living * 2 > entries.Count)
        {
            return;
        }

        var kept = new System.Collections.Generic.List<Entry>(living);

        foreach (var entry in entries)
        {
            if (entry.Living)
            {
                kept.Add(entry);
            }
        }

        entries.Clear();
        entries.AddRange(kept);
        slots.Clear();

        for (var slot = 0; slot < entries.Count; slot++)
        {
            slots[entries[slot].Key] = slot;
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=82E46D
    // Broiler-Human:        PENDING
    private readonly struct Entry(JsValue key, JsValue value, bool living)
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=E9E82A
        // Broiler-Human:        PENDING
        internal JsValue Key { get; } = key;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=E50CDA
        // Broiler-Human:        PENDING
        internal JsValue Value { get; } = value;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B94F4C
        // Broiler-Human:        PENDING
        internal bool Living { get; } = living;
    }
}

/// <summary>A <c>Map</c>: an ordered table whose entries are key and value.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=AF0E62
// Broiler-Human:        PENDING
internal sealed class JsMapObject : JsObject
{
    /// <summary>Creates an empty Map.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=E76CEA
    // Broiler-Human:        PENDING
    internal JsMapObject(JsObject? prototype)
        : base(prototype, "Map")
    {
    }

    /// <summary>The entries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=778941
    // Broiler-Human:        PENDING
    internal JsKeyedTable Table { get; } = new();
}

/// <summary>
/// A <c>Set</c>: the same ordered table, with each member stored as its own value.
/// </summary>
/// <remarks>
/// Storing the member twice - once as the key and once as the value - is not redundancy for its own
/// sake. It is what makes <c>Set.prototype.forEach</c> and <c>Map.prototype.forEach</c> the SAME
/// walk: the specification hands a Set's callback <c>(value, value, set)</c> precisely so that a
/// function written for a Map works over a Set, and a table that stored nothing in the value would
/// need a second walk with a flag threaded through it to reproduce that.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=0126B7
// Broiler-Human:        PENDING
internal sealed class JsSetObject : JsObject
{
    /// <summary>Creates an empty Set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=D2DBB3
    // Broiler-Human:        PENDING
    internal JsSetObject(JsObject? prototype)
        : base(prototype, "Set")
    {
    }

    /// <summary>The members.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=778941
    // Broiler-Human:        PENDING
    internal JsKeyedTable Table { get; } = new();
}

/// <summary>A mutable holder, because a weak table's value must be a reference type.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=5AD396
// Broiler-Human:        PENDING
internal sealed class JsValueBox
{
    /// <summary>Creates a box over <paramref name="value"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=3FC3AF
    // Broiler-Human:        PENDING
    internal JsValueBox(JsValue value) => Value = value;

    /// <summary>The boxed value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=1A2922
    // Broiler-Human:        PENDING
    internal JsValue Value { get; set; }
}

/// <summary>
/// A <c>WeakMap</c>: object keys that the table does not keep alive.
/// </summary>
/// <remarks>
/// <para>
/// <b><see cref="System.Runtime.CompilerServices.ConditionalWeakTable{TKey,TValue}"/> and not a
/// list of <see cref="System.WeakReference{T}"/>, for one reason that decides it.</b> A WeakMap's
/// hard requirement is EPHEMERON semantics: the value must not keep its own key alive. A list of
/// weak references holding values strongly gets that exactly backwards - the overwhelmingly common
/// <c>weak.set(node, { owner: node })</c> pins <c>node</c> for the life of the realm, which is the
/// leak a WeakMap is bought to avoid, and it is invisible in every test that does not run a
/// collector. The runtime's table is a real ephemeron table and gets it right. The list would also
/// be O(n) per lookup and would need a sweep somebody has to decide when to run; the table is
/// neither.
/// </para>
/// <para>
/// <b>What this costs: there is no <c>size</c>, no <c>clear</c> and no iteration, and that is the
/// language's decision rather than the table's limitation.</b> A WeakMap that could be counted or
/// walked would let a program observe when the collector ran, which is a side channel out of the
/// deterministic execution this profile is built to give. The specification omits all three for
/// that reason and so does this.
/// </para>
/// <para>
/// <b>A primitive key is a <c>TypeError</c> on the way in and a miss on the way out.</b>
/// <c>set</c> refuses one because there is nothing to hold weakly; <c>get</c>, <c>has</c> and
/// <c>delete</c> answer <c>undefined</c>, <c>false</c> and <c>false</c> without throwing, which is
/// what the specification says and what lets a caller probe a table without guarding every call.
/// Symbols as keys - ES2023's registered-symbol carve-out - are not implemented here because this
/// realm's collection surface predates its Symbols.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=826AB6
// Broiler-Human:        PENDING
internal sealed class JsWeakMapObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C76F51
    // Broiler-Human:        PENDING
    private readonly System.Runtime.CompilerServices.ConditionalWeakTable<JsObject, JsValueBox> table =
        new();

    /// <summary>Creates an empty WeakMap.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=09CEBB
    // Broiler-Human:        PENDING
    internal JsWeakMapObject(JsObject? prototype)
        : base(prototype, "WeakMap")
    {
    }

    /// <summary>Reads the value under <paramref name="key"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=EA59C8
    // Broiler-Human:        PENDING
    internal JsValue Get(JsObject key) =>
        table.TryGetValue(key, out var box) ? box.Value : JsValue.Undefined;

    /// <summary>Whether <paramref name="key"/> is present.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=DD952F
    // Broiler-Human:        PENDING
    internal bool Has(JsObject key) => table.TryGetValue(key, out _);

    /// <summary>Stores <paramref name="value"/> under <paramref name="key"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B1F28C
    // Broiler-Human:        PENDING
    internal void Set(JsObject key, JsValue value) => table.AddOrUpdate(key, new JsValueBox(value));

    /// <summary>Removes <paramref name="key"/>, answering whether it was there.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=63DB2E
    // Broiler-Human:        PENDING
    internal bool Delete(JsObject key) => table.Remove(key);
}

/// <summary>
/// A <c>WeakSet</c>: object members the set does not keep alive.
/// </summary>
/// <remarks>
/// It is the same ephemeron table as <see cref="JsWeakMapObject"/> with one shared, valueless box
/// stored against every member. Sharing one box is safe precisely because it references nothing:
/// a per-member box would be an allocation whose only purpose was to be distinct, and a box that
/// referenced the member would defeat the weakness the type exists for.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B01B53
// Broiler-Human:        PENDING
internal sealed class JsWeakSetObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=CF132B
    // Broiler-Human:        PENDING
    private static readonly JsValueBox Present = new(JsValue.Undefined);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C76F51
    // Broiler-Human:        PENDING
    private readonly System.Runtime.CompilerServices.ConditionalWeakTable<JsObject, JsValueBox> table =
        new();

    /// <summary>Creates an empty WeakSet.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C0F964
    // Broiler-Human:        PENDING
    internal JsWeakSetObject(JsObject? prototype)
        : base(prototype, "WeakSet")
    {
    }

    /// <summary>Whether <paramref name="member"/> is present.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C337B8
    // Broiler-Human:        PENDING
    internal bool Has(JsObject member) => table.TryGetValue(member, out _);

    /// <summary>Adds <paramref name="member"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=3B9E8A
    // Broiler-Human:        PENDING
    internal void Add(JsObject member) => table.AddOrUpdate(member, Present);

    /// <summary>Removes <paramref name="member"/>, answering whether it was there.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F95280
    // Broiler-Human:        PENDING
    internal bool Delete(JsObject member) => table.Remove(member);
}

/// <summary>
/// A <c>WeakRef</c>: a reference to one object that does not keep it alive - until it is read.
/// </summary>
/// <remarks>
/// <para>
/// <b>A successful <c>deref</c> promotes the reference to a strong one, permanently, and that is a
/// deliberate reading of the specification rather than a mistake.</b> The specification requires
/// <c>AddToKeptObjects</c>: a target handed out by <c>deref</c> must stay alive at least until the
/// end of the current job, so that two reads in one turn cannot disagree. This profile has no
/// "current job" reachable from inside a built-in - the queue in <see cref="JsEngine"/> is drained
/// by the host, not entered by this object - so the choice is between keeping the target for LESS
/// time than the specification demands, which is a conformance failure, and keeping it for MORE,
/// which the specification permits an implementation to do at any time and for any reason. Keeping
/// it for more is chosen.
/// </para>
/// <para>
/// <b>What that buys is monotonicity, which matters more here than in a browser.</b> Once
/// <c>deref</c> has answered an object it answers the same object for ever; it can never flip back
/// to <c>undefined</c> half-way through an expression. The alternative makes a program's OUTPUT a
/// function of when the CLR's collector happened to run, and a profile whose whole point is a
/// metered, reproducible execution should not hand the guest a primitive whose answer is a
/// property of the machine. The cost is stated plainly: a program that derefs a million targets
/// retains a million targets, and the guest could have done that with an array anyway.
/// </para>
/// <para>
/// <b>Before the first successful read it is genuinely weak.</b> A <c>WeakRef</c> that is created
/// and never read holds nothing, which is the case that matters for a cache: entries nobody asked
/// for are collectable. The point at which an unread reference starts answering <c>undefined</c>
/// is the collector's business and therefore not deterministic, and a program that branches on it
/// is branching on GC timing - which no implementation of this type can avoid.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=EF71BD
// Broiler-Human:        PENDING
internal sealed class JsWeakRefObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=ACDBC0
    // Broiler-Human:        PENDING
    private readonly System.WeakReference<JsObject> target;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A87102
    // Broiler-Human:        PENDING
    private JsObject? kept;

    /// <summary>Creates a reference to <paramref name="value"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=1E26D5
    // Broiler-Human:        PENDING
    internal JsWeakRefObject(JsObject? prototype, JsObject value)
        : base(prototype, "WeakRef") => target = new System.WeakReference<JsObject>(value);

    /// <summary>The target while it lives, or nothing once it is gone.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=D9A589
    // Broiler-Human:        PENDING
    internal JsObject? Deref()
    {
        if (kept is not null)
        {
            return kept;
        }

        if (!target.TryGetTarget(out var value))
        {
            return null;
        }

        kept = value;
        return value;
    }
}

/// <summary>One registration in a <see cref="JsFinalizationRegistryObject"/>.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8EA05E
// Broiler-Human:        PENDING
internal sealed class JsFinalizationRecord
{
    /// <summary>Records one <c>register</c> call.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C1B7E3
    // Broiler-Human:        PENDING
    internal JsFinalizationRecord(JsObject target, JsValue held, JsObject? token)
    {
        Target = new System.WeakReference<JsObject>(target);
        Held = held;
        Token = token is null ? null : new System.WeakReference<JsObject>(token);
    }

    /// <summary>The object whose collection would, in a realm that ran cleanups, be reported.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=2751C0
    // Broiler-Human:        PENDING
    internal System.WeakReference<JsObject> Target { get; }

    /// <summary>The value the cleanup callback would have been handed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C1BB99
    // Broiler-Human:        PENDING
    internal JsValue Held { get; }

    /// <summary>The token <c>unregister</c> matches on, when one was supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=7CF922
    // Broiler-Human:        PENDING
    internal System.WeakReference<JsObject>? Token { get; }
}

/// <summary>
/// A <c>FinalizationRegistry</c> that records registrations and NEVER runs a cleanup callback.
/// </summary>
/// <remarks>
/// <para>
/// <b>This is a declared divergence and not an unfinished feature, so read the reason before
/// filing it as a bug.</b> Guest code in this profile runs only on a guest stack, inside a metered
/// invocation, under an allowance a host granted and a meter is spending: every call goes through
/// <see cref="JsEngine.Call"/>, which charges fuel, counts depth, polls for cancellation and has
/// somewhere to put a thrown value. A CLR finalizer has none of those. It runs on the collector's
/// own thread, at a moment nobody chose, outside every invocation, with no allowance to spend and
/// no caller to report a throw to. Running a guest's cleanup callback from there would be running
/// unmetered guest code on a thread this profile does not own - the single worst thing an isolate
/// could do - and no amount of care inside the callback would fix the frame it was called on.
/// </para>
/// <para>
/// <b>So the type exists, answers, and is inert.</b> <c>register</c> validates its arguments
/// exactly as the specification says and records the registration; <c>unregister</c> removes what
/// a token names and answers truthfully whether it removed anything; <c>cleanupSome</c> accepts
/// its optional callback and does nothing. A program that uses a registry as a bookkeeping device -
/// which is most of them - behaves identically. A program that WAITS for a cleanup waits for ever,
/// and that is the observable difference, stated here rather than discovered.
/// </para>
/// <para>
/// <b>What would make this implementable is a job the host drains.</b> The queue is already there:
/// a future revision could sweep collected targets at a drain point and enqueue the callback as an
/// ordinary job, which puts it back on a metered guest stack inside an invocation the host asked
/// for. That is the shape to build, and it is deliberately not built here, because the sweep needs
/// a decision about WHEN a target counts as collected that this profile has not yet made.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=66E399
// Broiler-Falsified-If: a cleanup callback registered here is ever invoked, or any guest code runs from a CLR finalizer
// Broiler-Human:        PENDING
internal sealed class JsFinalizationRegistryObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=042973
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<JsFinalizationRecord> records = [];

    /// <summary>Creates a registry over <paramref name="cleanup"/>, which is never called.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=AB0C6D
    // Broiler-Human:        PENDING
    internal JsFinalizationRegistryObject(JsObject? prototype, JsValue cleanup)
        : base(prototype, "FinalizationRegistry") => Cleanup = cleanup;

    /// <summary>
    /// The callback the specification would call. It is held so that identity is preserved and
    /// nothing else; see this type's remarks for why it is not called.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=E2A8F8
    // Broiler-Human:        PENDING
    internal JsValue Cleanup { get; }

    /// <summary>How many registrations stand. Not reachable from the language.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=0ECBC8
    // Broiler-Human:        PENDING
    internal int Count => records.Count;

    /// <summary>Records one registration.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=655C95
    // Broiler-Human:        PENDING
    internal void Register(JsObject target, JsValue held, JsObject? token) =>
        records.Add(new JsFinalizationRecord(target, held, token));

    /// <summary>
    /// Removes every registration <paramref name="token"/> names, answering whether it removed any.
    /// </summary>
    /// <remarks>
    /// One token may name several registrations and all of them go, which is the specification's
    /// wording and the reason this is a sweep rather than a lookup. A record whose token has been
    /// collected can never be named again and is dropped on the way past, which is the only
    /// pruning this list gets.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=0EC68B
    // Broiler-Human:        PENDING
    internal bool Unregister(JsObject token)
    {
        var removed = false;

        for (var at = records.Count - 1; at >= 0; at--)
        {
            var held = records[at].Token;

            if (held is null)
            {
                continue;
            }

            if (!held.TryGetTarget(out var candidate))
            {
                records.RemoveAt(at);
                continue;
            }

            if (ReferenceEquals(candidate, token))
            {
                records.RemoveAt(at);
                removed = true;
            }
        }

        return removed;
    }
}

/// <summary>The three states a promise may be in.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B47D8E
// Broiler-Human:        PENDING
internal enum JsPromiseState : byte
{
    /// <summary>Not settled: reactions are recorded rather than scheduled.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=884D2F
    // Broiler-Human:        PENDING
    Pending = 0,

    /// <summary>Settled with a value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F3D7DF
    // Broiler-Human:        PENDING
    Fulfilled = 1,

    /// <summary>Settled with a reason.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F051B2
    // Broiler-Human:        PENDING
    Rejected = 2,
}

/// <summary>
/// One <c>then</c> that has not run yet: a handler, the promise it settles, and which side it is.
/// </summary>
/// <remarks>
/// A handler that is not callable is not an error and not a no-op - it is the PASS-THROUGH that
/// makes <c>p.then(null).then(f)</c> and <c>p.catch(g).then(f)</c> work, forwarding the value on
/// the fulfil side and re-throwing the reason on the reject side. <see cref="OnFulfil"/> is what
/// tells the two apart when the handler is missing, so it is recorded per reaction rather than
/// inferred from which list the reaction is in.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=81CF0E
// Broiler-Human:        PENDING
internal sealed class JsPromiseReaction
{
    /// <summary>Records one half of a <c>then</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F7A876
    // Broiler-Human:        PENDING
    internal JsPromiseReaction(JsPromiseObject derived, JsValue handler, bool onFulfil)
    {
        Derived = derived;
        Handler = handler;
        OnFulfil = onFulfil;
        Resolve = JsValue.Undefined;
        Reject = JsValue.Undefined;
    }

    /// <summary>Records one half of a <c>then</c> whose result is somebody else's promise.</summary>
    /// <remarks>
    /// <b>A <c>then</c> on a SUBCLASS answers an instance of the subclass</b>, built by that
    /// constructor and settled through the two functions it handed its executor - so this reaction
    /// has no promise of its own to settle and holds the pair instead. The ordinary case, where the
    /// species is the intrinsic, keeps the promise: it costs one object rather than three and needs
    /// no guest call to settle.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8A6A22
    // Broiler-Human:        PENDING
    internal JsPromiseReaction(JsValue resolve, JsValue reject, JsValue handler, bool onFulfil)
    {
        Derived = null;
        Handler = handler;
        OnFulfil = onFulfil;
        Resolve = resolve;
        Reject = reject;
    }

    /// <summary>The promise this reaction's outcome settles, when it settles one directly.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=CF536E
    // Broiler-Human:        PENDING
    internal JsPromiseObject? Derived { get; }

    /// <summary>The capability's <c>resolve</c>, when the result is not this realm's own promise.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=60D3F9
    // Broiler-Human:        PENDING
    internal JsValue Resolve { get; }

    /// <summary>The capability's <c>reject</c>, under the same condition.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=0FDCDE
    // Broiler-Human:        PENDING
    internal JsValue Reject { get; }

    /// <summary>The handler, or a non-callable value meaning "pass through".</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=428C8F
    // Broiler-Human:        PENDING
    internal JsValue Handler { get; }

    /// <summary>Whether this is the fulfil side.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B5A2D2
    // Broiler-Human:        PENDING
    internal bool OnFulfil { get; }
}

/// <summary>
/// The "already resolved" flag one <c>resolve</c>/<c>reject</c> PAIR shares.
/// </summary>
/// <remarks>
/// <b>It belongs to the pair and emphatically not to the promise.</b> A promise resolved with a
/// thenable is still <c>Pending</c> while the thenable is being adopted, so "is the promise
/// settled" cannot answer "may this function still act": the executor's own <c>reject</c>, called
/// after its <c>resolve</c> was handed a thenable, must be ignored, and a flag on the promise would
/// let it through. There is also more than one pair per promise over its lifetime - the adoption
/// job makes a second - and each must latch independently.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F734BE
// Broiler-Human:        PENDING
internal sealed class JsPromiseLatch
{
    /// <summary>Whether one of the pair has already fired.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=2BF78E
    // Broiler-Human:        PENDING
    internal bool Latched { get; set; }
}

/// <summary>
/// A <c>Promise</c>: a state, a settled value, and the reactions waiting on each side.
/// </summary>
/// <remarks>
/// <para>
/// <b>Two lists rather than one list of pairs.</b> A settlement runs one side and DROPS the other,
/// so the two are never walked together; keeping them apart means settling is a walk of exactly
/// the reactions that fire, with the losing side released in one assignment rather than filtered.
/// </para>
/// <para>
/// <b>Both lists are cleared the instant the promise settles, and that is a correctness
/// requirement and not a tidy-up.</b> A reaction list that outlived the settlement would hold every
/// closure a program ever attached to a long-lived resolved promise, which is the classic promise
/// leak. After settlement a new <c>then</c> schedules its job immediately and records nothing.
/// </para>
/// <para>
/// <b>There is no <c>[[PromiseIsHandled]]</c> flag, because nothing could read it.</b> The
/// specification tracks whether a rejected promise ever had a handler so that a HOST can report an
/// unhandled rejection; this profile has no such host hook - the core's result envelope carries a
/// completion or a fault, and a rejection nobody handled is neither. Carrying the flag anyway would
/// be carrying evidence for a report that cannot be made.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F8A9B7
// Broiler-Human:        PENDING
internal sealed class JsPromiseObject : JsObject
{
    /// <summary>Creates a pending promise.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=CCEF43
    // Broiler-Human:        PENDING
    internal JsPromiseObject(JsObject? prototype)
        : base(prototype, "Promise")
    {
    }

    /// <summary>Which state the promise is in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6135CD
    // Broiler-Human:        PENDING
    internal JsPromiseState State { get; set; } = JsPromiseState.Pending;

    /// <summary>The value it fulfilled with or the reason it rejected with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B750A3
    // Broiler-Human:        PENDING
    internal JsValue Result { get; set; } = JsValue.Undefined;

    /// <summary>The reactions a fulfilment would run.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A535AC
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsPromiseReaction> FulfilReactions { get; } = [];

    /// <summary>The reactions a rejection would run.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A5BACD
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsPromiseReaction> RejectReactions { get; } = [];
}
