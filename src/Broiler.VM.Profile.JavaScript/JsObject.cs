// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   22
// Annotated:        22/22
// Exempt:           23
// Human-reviewed:   0/22
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       22
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

    /// <summary>Creates an object with the given prototype.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=2E6A46
    // Broiler-Human:        PENDING
    internal JsObject(JsObject? prototype, string className = "Object")
    {
        Prototype = prototype;
        ClassName = className;
    }

    /// <summary>The object's prototype, or <see langword="null"/> at the end of a chain.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1A016B
    // Broiler-Human:        PENDING
    internal JsObject? Prototype { get; set; }

    /// <summary>Whether new own properties may be added.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=FB3EAD
    // Broiler-Human:        PENDING
    internal bool Extensible { get; set; } = true;

    /// <summary>
    /// The specification's <c>[[Class]]</c>, which <c>Object.prototype.toString</c> reports.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A6308F
    // Broiler-Human:        PENDING
    internal string ClassName { get; set; }

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
