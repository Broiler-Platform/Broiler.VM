// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           5
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// An Array exotic object: dense elements in a list, everything else in the ordinary map.
/// </summary>
/// <remarks>
/// <para>
/// <b>The elements are a list and not map entries.</b> An Array whose indices lived in the
/// ordinary property map would allocate one map entry, one string key and one hash lookup per
/// element, and the benchmarks this profile is built to run are mostly loops over arrays. A hole
/// is <see cref="JsValue.Empty"/> in the list, which is the same marker an uninitialised binding
/// uses and is distinguishable from a stored <c>undefined</c> - a distinction <c>in</c>,
/// <c>hasOwnProperty</c> and <c>for…in</c> all observe.
/// </para>
/// <para>
/// <b>A sparse Array falls back to the map.</b> Writing index 100000 to an empty Array does not
/// allocate 100000 slots: past a growth threshold the index becomes an ordinary property, and the
/// list stays whatever length it reached. That keeps <c>a[1e9] = 1</c> from being an allocation
/// the guest chose.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1D8A60
// Broiler-Human:        PENDING
internal sealed class JsArray : JsObject
{
    /// <summary>How far past the dense end a write may reach before the Array goes sparse.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=72848B
    // Broiler-Human:        PENDING
    private const int DenseGrowthSlack = 1024;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0C926E
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<JsValue> elements = [];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7CB409
    // Broiler-Human:        PENDING
    private uint length;

    /// <summary>Whether <c>length</c> may still move.</summary>
    /// <remarks>
    /// <b>It is a field rather than an ordinary property because <c>length</c> is not an ordinary
    /// property.</b> An Array's length is derived from the elements, so there is nowhere in the
    /// property store for its attributes to live; without this, <c>defineProperty(a, "length",
    /// { writable: false })</c> was accepted, reported as accepted, and then ignored by the next
    /// <c>push</c>. A refusal that reports success is the one answer this profile refuses to give.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9667EC
    // Broiler-Human:        PENDING
    private bool lengthWritable = true;

    /// <summary>Creates an empty Array.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A4EEE4
    // Broiler-Human:        PENDING
    internal JsArray(JsObject? prototype)
        : base(prototype, "Array")
    {
    }

    /// <summary>The Array's <c>length</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F4AC04
    // Broiler-Human:        PENDING
    internal uint Length => length;

    /// <summary>Whether the length may still move.</summary>
    /// <remarks>
    /// A closed length is what makes a write past the end a refusal rather than an extension, and
    /// the engine has to be able to ask before it stores: a store that was silently dropped is a
    /// <c>TypeError</c> in strict code, and only the caller knows the mode.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=AA042B
    // Broiler-Human:        PENDING
    internal bool LengthWritable => lengthWritable;

    /// <summary>How many elements the dense part holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FB598E
    // Broiler-Human:        PENDING
    internal int DenseCount => elements.Count;

    /// <summary>Reads the dense element at <paramref name="at"/>, which may be a hole.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5DD9AB
    // Broiler-Human:        PENDING
    internal JsValue DenseAt(int at) => elements[at];

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B2758E
    // Broiler-Human:        PENDING
    internal override int OwnPropertyCount
    {
        get
        {
            var count = base.OwnPropertyCount + 1;

            foreach (var element in elements)
            {
                if (!element.IsEmpty)
                {
                    count++;
                }
            }

            return count;
        }
    }

    /// <summary>Appends one element.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=502981
    // Broiler-Human:        PENDING
    internal void Push(JsValue value)
    {
        if (length == elements.Count)
        {
            elements.Add(value);
            length++;
            return;
        }

        SetIndex(length, value);
    }

    /// <summary>Sets the Array's length, dropping or extending as the specification requires.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=65E131
    // Broiler-Human:        PENDING
    internal void SetLength(uint value) => _ = TrySetLength(value);

    /// <summary>Sets the length, and answers whether it got all the way there.</summary>
    /// <remarks>
    /// <b>A SHORTENING DELETES FROM THE TOP DOWN AND STOPS AT THE FIRST ELEMENT IT MAY NOT
    /// DELETE.</b> `Object.defineProperty(a, "1", {configurable: false})` makes index 1 permanent,
    /// and the language says a later `a.length = 0` deletes index 2, fails at index 1, and leaves
    /// the length at 2 — a partial result rather than either extreme. Truncating regardless would
    /// destroy a property the program was promised, and refusing outright would leave a length
    /// nobody could shrink at all.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=28839C
    // Broiler-Human:        PENDING
    internal bool TrySetLength(uint value)
    {
        if (value >= length)
        {
            length = value;
            return true;
        }

        var reached = value;

        for (var candidate = length; candidate > value; candidate--)
        {
            var at = candidate - 1;
            var key = JsNumberFormat.ToUintString(at);

            // THE DENSE HALF HAS NO ATTRIBUTES TO CONSULT: an element that is still in it is an
            // ordinary configurable data property, and one that was given attributes has already
            // moved into the map underneath - leaving a HOLE in the dense half at that index, which
            // is why the test is on the slot being empty rather than on the index being past the
            // end. Only the map can refuse.
            var dense = at < elements.Count && !elements[(int)at].IsEmpty;

            if (!dense && base.TryGetOwnProperty(key, out var held) && !held.Configurable)
            {
                reached = at + 1;
                break;
            }

            if (dense)
            {
                elements[(int)at] = JsValue.Empty;
            }
            else
            {
                base.DeleteOwnProperty(key);
            }
        }

        if (elements.Count > reached)
        {
            elements.RemoveRange((int)reached, elements.Count - (int)reached);
        }

        length = reached;
        return reached == value;
    }

    /// <summary>Writes one indexed element.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=89F663
    // Broiler-Human:        PENDING
    internal void SetIndex(uint at, JsValue value)
    {
        // THE SAME REFUSAL AS THE ONE ABOVE, on the fast path the engine reaches directly. A write
        // that would extend a length nobody may move is not a write.
        if (!lengthWritable && at >= length)
        {
            return;
        }

        if (at < elements.Count)
        {
            elements[(int)at] = value;
        }
        else if (at <= (ulong)elements.Count + DenseGrowthSlack && at < int.MaxValue)
        {
            while (elements.Count < at)
            {
                elements.Add(JsValue.Empty);
            }

            elements.Add(value);
        }
        else
        {
            base.SetOwnProperty(
                JsNumberFormat.ToUintString(at), JsProperty.Data(value, JsPropertyAttributes.Default));
        }

        if (at >= length)
        {
            length = at + 1;
        }
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=AF68DC
    // Broiler-Human:        PENDING
    internal override bool TryGetOwnProperty(string key, out JsProperty property)
    {
        if (IsArrayIndex(key, out var at))
        {
            if (at < elements.Count)
            {
                var element = elements[(int)at];

                if (!element.IsEmpty)
                {
                    property = JsProperty.Data(element, JsPropertyAttributes.Default);
                    return true;
                }

                // A HOLE FALLS THROUGH TO THE ORDINARY MAP RATHER THAN ANSWERING "ABSENT".
                // SetOwnProperty vacates a dense slot when it is handed a descriptor the slot
                // cannot express and writes the map instead, so a hole is EITHER a hole or a slot
                // that moved. Answering absent for both made a frozen element unreachable, and the
                // element was still there.
            }
        }
        else if (string.Equals(key, "length", System.StringComparison.Ordinal))
        {
            property = JsProperty.Data(
                JsValue.Number(length),
                lengthWritable ? JsPropertyAttributes.Writable : JsPropertyAttributes.None);

            return true;
        }

        return base.TryGetOwnProperty(key, out property);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B28009
    // Broiler-Human:        PENDING
    internal override void SetOwnProperty(string key, JsProperty property)
    {
        if (string.Equals(key, "length", System.StringComparison.Ordinal) && !property.IsAccessor)
        {
            var wanted = JsValue.ToUint32(property.Value.Type == JsType.Number
                ? property.Value.AsNumber()
                : 0);

            // A DEFINITION MAY CLOSE THE LENGTH AND AN ASSIGNMENT MAY NOT, and both arrive here.
            // The checked path hands down whatever attributes the descriptor asked for, so losing
            // `Writable` is how a definition says so; an ordinary assignment carries the attributes
            // the property already had, which the caller read from `TryGetOwnProperty` above.
            if (!lengthWritable && wanted != length)
            {
                return;
            }

            SetLength(wanted);
            lengthWritable = (property.Attributes & JsPropertyAttributes.Writable) != 0;
            return;
        }

        // AN ELEMENT PAST A CLOSED LENGTH IS REFUSED, because writing it would move the length that
        // was just made immovable. An element INSIDE the length is unaffected: closing the length
        // says nothing about the elements, and freezing those is what `Object.freeze` is for.
        if (!lengthWritable && IsArrayIndex(key, out var beyond) && beyond >= length)
        {
            return;
        }

        if (IsArrayIndex(key, out var at) && !property.IsAccessor &&
            property.Attributes == JsPropertyAttributes.Default)
        {
            SetIndex(at, property.Value);
            return;
        }

        if (IsArrayIndex(key, out var slot) && slot < elements.Count)
        {
            // A descriptor an element cannot express - an accessor, or an element that is not
            // writable - moves the element out of the dense part and into the ordinary map, where
            // it can carry attributes. The hole it leaves behind is what makes the two halves
            // agree afterwards.
            elements[(int)slot] = JsValue.Empty;
        }

        base.SetOwnProperty(key, property);

        if (IsArrayIndex(key, out var reached) && reached >= length)
        {
            length = reached + 1;
        }
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7DDAF3
    // Broiler-Human:        PENDING
    internal override bool DeleteOwnProperty(string key)
    {
        if (IsArrayIndex(key, out var at) && at < elements.Count)
        {
            elements[(int)at] = JsValue.Empty;
            return true;
        }

        return base.DeleteOwnProperty(key);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=271967
    // Broiler-Human:        PENDING
    internal override System.Collections.Generic.List<string> OwnPropertyNames()
    {
        var indices = new System.Collections.Generic.List<string>();
        var rest = new System.Collections.Generic.List<string>();

        for (var at = 0; at < elements.Count; at++)
        {
            if (!elements[at].IsEmpty)
            {
                indices.Add(JsNumberFormat.ToUintString((uint)at));
            }
        }

        CollectOwnNames(indices, rest);
        SortIndexKeys(indices);
        indices.Add("length");
        indices.AddRange(rest);
        return indices;
    }
}
