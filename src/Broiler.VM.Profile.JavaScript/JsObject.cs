// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   31
// Annotated:        31/31
// Exempt:           28
// Human-reviewed:   0/31
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       31
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>The attribute bits a property carries.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8BFC83
// Broiler-Human:        PENDING
[System.Flags]
internal enum JsPropertyAttributes : byte
{
    /// <summary>None of the three: a non-writable, non-enumerable, non-configurable data property.</summary>
    None = 0,

    /// <summary>The property's value may be replaced.</summary>
    Writable = 1,

    /// <summary>The property is reached by <c>for…in</c> and <c>Object.keys</c>.</summary>
    Enumerable = 2,

    /// <summary>The property may be deleted or redefined.</summary>
    Configurable = 4,

    /// <summary>The property is an accessor pair rather than a value.</summary>
    Accessor = 8,

    /// <summary>What an ordinary assignment creates.</summary>
    Default = Writable | Enumerable | Configurable,

    /// <summary>What a built-in method is defined with.</summary>
    BuiltIn = Writable | Configurable,
}

/// <summary>One own property: either a value or a getter and setter pair.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=2DD4EE
// Broiler-Human:        PENDING
internal struct JsProperty
{
    /// <summary>The value of a data property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3700CE
    // Broiler-Human:        PENDING
    internal JsValue Value;

    /// <summary>The getter of an accessor property, when it has one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7807A3
    // Broiler-Human:        PENDING
    internal JsObject? Getter;

    /// <summary>The setter of an accessor property, when it has one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A734E1
    // Broiler-Human:        PENDING
    internal JsObject? Setter;

    /// <summary>The attribute bits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9E00B4
    // Broiler-Human:        PENDING
    internal JsPropertyAttributes Attributes;

    /// <summary>Whether this is an accessor property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=145623
    // Broiler-Human:        PENDING
    internal readonly bool IsAccessor => (Attributes & JsPropertyAttributes.Accessor) != 0;

    /// <summary>Whether the value may be replaced.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F27086
    // Broiler-Human:        PENDING
    internal readonly bool Writable => (Attributes & JsPropertyAttributes.Writable) != 0;

    /// <summary>Whether the property is enumerable.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C54C1F
    // Broiler-Human:        PENDING
    internal readonly bool Enumerable => (Attributes & JsPropertyAttributes.Enumerable) != 0;

    /// <summary>Whether the property may be deleted or redefined.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=47B245
    // Broiler-Human:        PENDING
    internal readonly bool Configurable => (Attributes & JsPropertyAttributes.Configurable) != 0;

    /// <summary>A data property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6BACAB
    // Broiler-Human:        PENDING
    internal static JsProperty Data(JsValue value, JsPropertyAttributes attributes) =>
        new() { Value = value, Attributes = attributes };

    /// <summary>An accessor property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=48D7CF
    // Broiler-Human:        PENDING
    internal static JsProperty Accessor(
        JsObject? getter, JsObject? setter, JsPropertyAttributes attributes) =>
        new()
        {
            Value = JsValue.Undefined,
            Getter = getter,
            Setter = setter,
            Attributes = attributes | JsPropertyAttributes.Accessor,
        };
}

/// <summary>
/// An ordinary object: a prototype, an extensible flag and an ordered map of own properties.
/// </summary>
/// <remarks>
/// <para>
/// <b>Own-property order is the specification's and not the hash table's.</b>
/// <c>OwnPropertyNames</c> yields array-index keys in ascending numeric order first and every other
/// key in the order it was created, which is what <c>for…in</c>, <c>Object.keys</c> and
/// <c>JSON.stringify</c> all observe. A dictionary alone cannot promise that after a delete, so
/// insertion order is kept in a list beside it and a deleted entry is tombstoned rather than
/// removed.
/// </para>
/// <para>
/// <b>Getting and setting a property is not on this type.</b> Both can run guest code through an
/// accessor, and running guest code needs a frame, a fuel charge and a place to put an exception.
/// The engine owns all three, so the engine owns <c>Get</c> and <c>Set</c> and this type owns only
/// what the specification calls the ordinary internal methods over own properties.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=60E350
// Broiler-Human:        PENDING
internal class JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=71FF4C
    // Broiler-Human:        PENDING
    private System.Collections.Generic.Dictionary<string, int>? index;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=137A27
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<Entry>? entries;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=178828
    // Broiler-Human:        PENDING
    private int liveCount;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F81DF9
    // Broiler-Human:        PENDING
    private JsObject? prototype;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=08F265
    // Broiler-Human:        PENDING
    private bool extensible = true;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1CB525
    // Broiler-Human:        PENDING
    private string className;

    /// <summary>Creates an object with the given prototype.</summary>
    /// <remarks>
    /// <b>The field is written and not the property.</b> <see cref="Prototype"/> is virtual, and a
    /// virtual call from a constructor reaches an override before the derived type's own fields are
    /// assigned - which for <see cref="JsProxy"/> would be a trap call against a null realm. Every
    /// object starts with an ordinary prototype whether or not it is exotic, so the field is the
    /// right thing to write anyway.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=31356F
    // Broiler-Human:        PENDING
    internal JsObject(JsObject? prototype, string className = "Object")
    {
        this.prototype = prototype;
        this.className = className;
    }

    /// <summary>The object's prototype, or <see langword="null"/> at the end of a chain.</summary>
    /// <remarks>
    /// <b>It is virtual because <c>[[GetPrototypeOf]]</c> and <c>[[SetPrototypeOf]]</c> are internal
    /// methods and not a field</b>, which only <see cref="JsProxy"/> makes visible: a proxy answers
    /// them with guest code. The storage is a field of this class rather than an auto-property
    /// because a derived type overriding the pair still needs somewhere for <c>base</c> to keep an
    /// ordinary answer.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=53CF20
    // Broiler-Human:        PENDING
    internal virtual JsObject? Prototype
    {
        get => prototype;
        set => prototype = value;
    }

    /// <summary>Whether new own properties may be added.</summary>
    /// <remarks>
    /// Virtual for the same reason <see cref="Prototype"/> is: reading it is
    /// <c>[[IsExtensible]]</c> and clearing it is <c>[[PreventExtensions]]</c>, and a
    /// <see cref="JsProxy"/> answers both through its handler.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=971BED
    // Broiler-Human:        PENDING
    internal virtual bool Extensible
    {
        get => extensible;
        set => extensible = value;
    }

    /// <summary>
    /// The specification's <c>[[Class]]</c>, which <c>Object.prototype.toString</c> reports.
    /// </summary>
    /// <remarks>
    /// <b>It is virtual because the tag is derived from an object's internal SLOTS, and one object
    /// kind has none.</b> A <see cref="JsProxy"/> is not a Date because its target is one - it has
    /// no <c>[[DateValue]]</c> - so the language gives it one of three tags rather than its target's,
    /// and it derives them when asked rather than storing one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A9A0F6
    // Broiler-Human:        PENDING
    internal virtual string ClassName
    {
        get => className;
        set => className = value;
    }

    /// <summary>Whether this object may be called.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=72C1AD
    // Broiler-Human:        PENDING
    internal virtual bool IsCallable => false;

    /// <summary>Whether this object may be constructed with <c>new</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F70761
    // Broiler-Human:        PENDING
    internal virtual bool IsConstructor => false;

    /// <summary>How many own properties this object has, indexed elements included.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=27D959
    // Broiler-Human:        PENDING
    internal virtual int OwnPropertyCount => liveCount;

    /// <summary>Reads one own property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=2A21C8
    // Broiler-Human:        PENDING
    internal virtual bool TryGetOwnProperty(string key, out JsProperty property)
    {
        if (index is not null && index.TryGetValue(key, out var at) && entries![at].Live)
        {
            property = entries[at].Property;
            return true;
        }

        property = default;
        return false;
    }

    /// <summary>Whether this object has the named own property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=83EE39
    // Broiler-Human:        PENDING
    internal bool HasOwnProperty(string key) => TryGetOwnProperty(key, out _);

    /// <summary>
    /// The own properties keyed by a Symbol, which are a separate table on purpose.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A second dictionary rather than one keyed by "a String or a Symbol".</b> The cheap
    /// alternative — encoding a Symbol as a String nobody would write — is forgeable, because every
    /// String a program can imagine is a String a program can write; and widening the key type of
    /// the one table would change the type of every property operation in the profile for the sake
    /// of a table most objects never have. This one is allocated only when a Symbol key is first
    /// stored, so an object with no Symbol-keyed property carries a null field and nothing else.
    /// </para>
    /// <para>
    /// <b>Order is insertion order and is separate from the String keys'.</b> The language
    /// enumerates String keys and then Symbol keys, never interleaved, so two tables is also what
    /// the ordering rule wants.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7500E4
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<(JsSymbol Key, JsProperty Property)>? symbols;

    /// <summary>Reads one Symbol-keyed own property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=67E2E2
    // Broiler-Human:        PENDING
    internal virtual bool TryGetOwnSymbol(JsSymbol key, out JsProperty property)
    {
        if (symbols is not null)
        {
            foreach (var (candidate, stored) in symbols)
            {
                if (ReferenceEquals(candidate, key))
                {
                    property = stored;
                    return true;
                }
            }
        }

        property = default;
        return false;
    }

    /// <summary>Defines or redefines one Symbol-keyed own property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=96F3D0
    // Broiler-Human:        PENDING
    internal virtual void SetOwnSymbol(JsSymbol key, JsProperty property)
    {
        symbols ??= [];

        for (var at = 0; at < symbols.Count; at++)
        {
            if (ReferenceEquals(symbols[at].Key, key))
            {
                symbols[at] = (key, property);
                return;
            }
        }

        symbols.Add((key, property));
    }

    /// <summary>Removes one Symbol-keyed own property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=206E05
    // Broiler-Human:        PENDING
    internal virtual bool DeleteOwnSymbol(JsSymbol key)
    {
        if (symbols is null)
        {
            return true;
        }

        for (var at = 0; at < symbols.Count; at++)
        {
            if (!ReferenceEquals(symbols[at].Key, key))
            {
                continue;
            }

            if (!symbols[at].Property.Configurable)
            {
                return false;
            }

            symbols.RemoveAt(at);
            return true;
        }

        return true;
    }

    /// <summary>Every Symbol-keyed own property key, in the order they were defined.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=4FCD32
    // Broiler-Human:        PENDING
    internal virtual System.Collections.Generic.List<JsSymbol> OwnSymbolKeys()
    {
        var keys = new System.Collections.Generic.List<JsSymbol>();

        if (symbols is not null)
        {
            foreach (var (key, _) in symbols)
            {
                keys.Add(key);
            }
        }

        return keys;
    }

    /// <summary>
    /// Every own key of both kinds, which is the specification's <c>[[OwnPropertyKeys]]</c>.
    /// </summary>
    /// <remarks>
    /// <b>One method rather than the two above called in turn, and the difference is a trap
    /// call.</b> For an ordinary object this is exactly the concatenation and exists only for
    /// tidiness; for a <see cref="JsProxy"/> it is the operation the language actually names, and
    /// asking for the String keys and then the Symbol keys would run the <c>ownKeys</c> trap TWICE
    /// - which is observable, and which would let the two halves of one answer disagree.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=45C3E2
    // Broiler-Human:        PENDING
    internal virtual System.Collections.Generic.List<JsValue> OwnKeys()
    {
        var keys = new System.Collections.Generic.List<JsValue>();

        foreach (var key in OwnPropertyNames())
        {
            keys.Add(JsValue.String(key));
        }

        foreach (var key in OwnSymbolKeys())
        {
            keys.Add(JsValue.Symbol(key));
        }

        return keys;
    }

    /// <summary>
    /// The private elements a class installed on this object, which are NOT properties.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A THIRD table, and the reason is every reflection surface in the profile.</b>
    /// <c>Object.keys</c>, <c>Object.getOwnPropertyNames</c>,
    /// <c>Object.getOwnPropertySymbols</c>, <c>Reflect.ownKeys</c>, <c>JSON.stringify</c>,
    /// <c>for…in</c>, the spread of an object literal and <c>Object.assign</c> all walk the two
    /// tables above. Storing a private element in either of them would have made every one of those
    /// a way out for a field the class meant to keep, and closing them one at a time would have
    /// meant each new surface had to remember. There is nothing to remember when the storage is
    /// somewhere they do not look.
    /// </para>
    /// <para>
    /// <b>It is a list and not a dictionary because a private name is compared by identity and
    /// there are never many.</b> A class body declares as many private names as it has lines, and
    /// the walk is the same one the Symbol table already does for the same reason.
    /// </para>
    /// <para>
    /// <b>A <see cref="JsProperty"/> holds the element, and the attribute bits mean what they say.</b>
    /// A private field is writable; a private method is not, which is what makes <c>this.#m = 1</c>
    /// a <c>TypeError</c>; a private accessor is an accessor pair. None of them is ever enumerable
    /// or configurable, because nothing enumerates them and nothing may delete one.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=79DD4B
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<(JsSymbol Name, JsProperty Element)>? privates;

    /// <summary>Reads one private element.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=61865C
    // Broiler-Human:        PENDING
    internal bool TryGetPrivate(JsSymbol name, out JsProperty element)
    {
        if (privates is not null)
        {
            foreach (var (candidate, stored) in privates)
            {
                if (ReferenceEquals(candidate, name))
                {
                    element = stored;
                    return true;
                }
            }
        }

        element = default;
        return false;
    }

    /// <summary>Whether this object carries a private element of that name.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=689D5F
    // Broiler-Human:        PENDING
    internal bool HasPrivate(JsSymbol name) => TryGetPrivate(name, out _);

    /// <summary>Installs or replaces one private element.</summary>
    /// <remarks>
    /// <b>It never consults <see cref="Extensible"/>.</b> A private element is installed by the
    /// class's own construction, before the object reaches any code that could have frozen it, and
    /// a frozen instance still carries the fields its class gave it - which is what
    /// <c>Object.freeze(new C())</c> has to keep true for a class with private state.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=584929
    // Broiler-Human:        PENDING
    internal void SetPrivate(JsSymbol name, JsProperty element)
    {
        privates ??= [];

        for (var at = 0; at < privates.Count; at++)
        {
            if (ReferenceEquals(privates[at].Name, name))
            {
                privates[at] = (name, element);
                return;
            }
        }

        privates.Add((name, element));
    }

    /// <summary>Defines a Symbol-keyed data property with the built-in attribute set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A52222
    // Broiler-Human:        PENDING
    internal void DefineBuiltIn(JsSymbol key, JsValue value) =>
        SetOwnSymbol(key, JsProperty.Data(value, JsPropertyAttributes.BuiltIn));

    /// <summary>
    /// Defines or redefines one own property, with no regard for writability or extensibility.
    /// </summary>
    /// <remarks>
    /// This is the unchecked form the engine's own construction uses - intrinsics, literals and
    /// anything the specification defines rather than assigns. The checked form the language
    /// exposes is <c>Object.defineProperty</c>, which validates against the current descriptor
    /// first and then calls this.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5F9A87
    // Broiler-Human:        PENDING
    internal virtual void SetOwnProperty(string key, JsProperty property)
    {
        index ??= new System.Collections.Generic.Dictionary<string, int>(System.StringComparer.Ordinal);
        entries ??= [];

        if (index.TryGetValue(key, out var at))
        {
            if (!entries[at].Live)
            {
                liveCount++;
            }

            entries[at] = new Entry(key, property, true);
            return;
        }

        index[key] = entries.Count;
        entries.Add(new Entry(key, property, true));
        liveCount++;
    }

    /// <summary>Removes one own property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CC78C4
    // Broiler-Human:        PENDING
    internal virtual bool DeleteOwnProperty(string key)
    {
        if (index is null || !index.TryGetValue(key, out var at) || !entries![at].Live)
        {
            return true;
        }

        if (!entries[at].Property.Configurable)
        {
            return false;
        }

        entries[at] = new Entry(key, default, false);
        liveCount--;
        return true;
    }

    /// <summary>
    /// Every own property name, array indices first in ascending numeric order.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C87522
    // Broiler-Human:        PENDING
    internal virtual System.Collections.Generic.List<string> OwnPropertyNames()
    {
        var indices = new System.Collections.Generic.List<string>();
        var rest = new System.Collections.Generic.List<string>();
        CollectOwnNames(indices, rest);
        SortIndexKeys(indices);
        indices.AddRange(rest);
        return indices;
    }

    /// <summary>Appends this object's stored keys, splitting index keys from the rest.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5CB8DC
    // Broiler-Human:        PENDING
    private protected void CollectOwnNames(
        System.Collections.Generic.List<string> indices,
        System.Collections.Generic.List<string> rest)
    {
        if (entries is null)
        {
            return;
        }

        foreach (var entry in entries)
        {
            if (!entry.Live)
            {
                continue;
            }

            if (IsArrayIndex(entry.Key, out _))
            {
                indices.Add(entry.Key);
            }
            else
            {
                rest.Add(entry.Key);
            }
        }
    }

    /// <summary>Orders array-index keys the way the specification orders them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EA1531
    // Broiler-Human:        PENDING
    private protected static void SortIndexKeys(System.Collections.Generic.List<string> keys) =>
        keys.Sort(static (left, right) =>
        {
            IsArrayIndex(left, out var a);
            IsArrayIndex(right, out var b);
            return a.CompareTo(b);
        });

    /// <summary>Defines a data property with the built-in attribute set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=443962
    // Broiler-Human:        PENDING
    internal void DefineBuiltIn(string key, JsValue value) =>
        SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.BuiltIn));

    /// <summary>Defines a data property that nothing may change.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1E22FA
    // Broiler-Human:        PENDING
    internal void DefineFrozen(string key, JsValue value) =>
        SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.None));

    /// <summary>Defines an ordinary, assignable, enumerable property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=100E6F
    // Broiler-Human:        PENDING
    internal void DefineOrdinary(string key, JsValue value) =>
        SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));

    /// <summary>
    /// Whether <paramref name="key"/> is a canonical array index, and what it is.
    /// </summary>
    /// <remarks>
    /// Canonical is the whole of it: <c>"01"</c> and <c>"1.0"</c> and <c>"-0"</c> are ordinary
    /// property names and not indices, because the specification's round trip through
    /// <c>ToString</c> does not produce them. An implementation that parsed the digits and stopped
    /// would answer that <c>a["01"]</c> is <c>a[1]</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E3AF51
    // Broiler-Human:        PENDING
    internal static bool IsArrayIndex(string key, out uint value)
    {
        value = 0;

        if (key.Length == 0 || key.Length > 10)
        {
            return false;
        }

        if (key[0] == '0' && key.Length > 1)
        {
            return false;
        }

        ulong accumulated = 0;

        foreach (var character in key)
        {
            if (character is < '0' or > '9')
            {
                return false;
            }

            accumulated = (accumulated * 10) + (ulong)(character - '0');

            if (accumulated > 4294967294)
            {
                return false;
            }
        }

        value = (uint)accumulated;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=01F91E
    // Broiler-Human:        PENDING
    private readonly struct Entry(string key, JsProperty property, bool live)
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=71D82D
        // Broiler-Human:        PENDING
        internal string Key { get; } = key;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=129942
        // Broiler-Human:        PENDING
        internal JsProperty Property { get; } = property;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0D40F9
        // Broiler-Human:        PENDING
        internal bool Live { get; } = live;
    }
}
