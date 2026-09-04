// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           0
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// Iteration over the keyed collections, which arrived after them and after <c>Symbol</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is a file of its own because it is a join between two things built separately.</b> The
/// collections were written when this realm had no Symbol and so could carry no
/// <c>[Symbol.iterator]</c>; the Symbol arrived afterwards. Folding these methods back into the
/// collections' own setup would hide that they depend on a surface the collections do not, and the
/// order the realm builds its intrinsics in is already load-bearing enough.
/// </para>
/// <para>
/// <b>The walk is over slots and not over living entries.</b> A collection's table keeps deleted
/// entries as tombstones while anything is iterating, precisely so that an iterator's position
/// stays meaningful across a deletion; an iterator that counted living entries would skip one every
/// time an earlier entry died. So the cursor is a slot index, the walk asks the table whether that
/// slot is alive, and an entry added during iteration is reached because the slot list grew.
/// </para>
/// <para>
/// <b>Nothing here tells the table that iteration has ended.</b> The table compacts only when
/// nothing is walking, and an iterator this realm hands to a guest may be abandoned half-finished —
/// a <c>break</c> out of a <c>for … of</c> does exactly that. Holding the table uncompactable until
/// a guest that may never come back finishes would make a collection grow without bound; not
/// holding it means a compaction can happen under an abandoned iterator, and the cursor of an
/// iterator nobody is using is nobody's business. What this costs is stated rather than hidden: an
/// iterator resumed after a compaction sees the compacted order.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>Installs the iteration members on the keyed collections' prototypes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1C14A8
    // Broiler-Human:        PENDING
    private void SetupCollectionIterators()
    {
        var mapEntries = Native("entries", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.Object(engine.Realm.CollectionIterator(
                engine, thisValue, "Map Iterator", IndexedIteratorKind.Entry, wantsMap: true));
        });

        Method(MapPrototype, "keys", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.Object(engine.Realm.CollectionIterator(
                engine, thisValue, "Map Iterator", IndexedIteratorKind.Key, wantsMap: true));
        });

        Method(MapPrototype, "values", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.Object(engine.Realm.CollectionIterator(
                engine, thisValue, "Map Iterator", IndexedIteratorKind.Value, wantsMap: true));
        });

        MapPrototype.SetOwnProperty(
            "entries", JsProperty.Data(JsValue.Object(mapEntries), JsPropertyAttributes.BuiltIn));

        MapPrototype.SetOwnSymbol(
            IteratorSymbol, JsProperty.Data(JsValue.Object(mapEntries), JsPropertyAttributes.BuiltIn));

        MapPrototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Map"), JsPropertyAttributes.Configurable));

        var setValues = Native("values", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.Object(engine.Realm.CollectionIterator(
                engine, thisValue, "Set Iterator", IndexedIteratorKind.Value, wantsMap: false));
        });

        // A SET'S KEYS AND VALUES ARE THE SAME FUNCTION OBJECT, which the language says in those
        // words: `Set.prototype.keys` IS `Set.prototype.values`. Two functions that behaved the
        // same would pass every test but one, and that one is `Set.prototype.keys === values`.
        SetPrototype.SetOwnProperty(
            "values", JsProperty.Data(JsValue.Object(setValues), JsPropertyAttributes.BuiltIn));

        SetPrototype.SetOwnProperty(
            "keys", JsProperty.Data(JsValue.Object(setValues), JsPropertyAttributes.BuiltIn));

        Method(SetPrototype, "entries", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.Object(engine.Realm.CollectionIterator(
                engine, thisValue, "Set Iterator", IndexedIteratorKind.Entry, wantsMap: false));
        });

        SetPrototype.SetOwnSymbol(
            IteratorSymbol, JsProperty.Data(JsValue.Object(setValues), JsPropertyAttributes.BuiltIn));

        SetPrototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Set"), JsPropertyAttributes.Configurable));
    }

    /// <summary>One iterator over a keyed collection's slots.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D8E690
    // Broiler-Human:        PENDING
    private JsObject CollectionIterator(
        JsEngine owner, JsValue receiver, string tag, IndexedIteratorKind kind, bool wantsMap)
    {
        var table = TableOf(owner, receiver, tag, wantsMap);

        return CreateListIterator(tag, slot =>
        {
            while (slot < table.SlotCount)
            {
                if (table.TryAt(slot, out var key, out var value))
                {
                    // THE SLOT THE WALK STOPPED AT IS REPORTED, not the one it started from. A
                    // tombstone the walk stepped over is a step the cursor has to make too, and a
                    // cursor that made one step per call re-read the entry this walk had just
                    // answered with.
                    return (
                        true,
                        kind switch
                        {
                            IndexedIteratorKind.Key => key,
                            IndexedIteratorKind.Value => wantsMap ? value : key,
                            _ => JsValue.Object(NewArray([key, wantsMap ? value : key])),
                        },
                        slot + 1);
                }

                slot++;
            }

            return (false, JsValue.Undefined, slot);
        });
    }

    /// <summary>
    /// The table behind a receiver, or a <c>TypeError</c> naming what it should have been.
    /// </summary>
    /// <remarks>
    /// The brand check the language performs. A <c>Map.prototype.keys</c> called on a Set is a
    /// <c>TypeError</c> and not an empty iterator, because an empty iterator is an answer and the
    /// receiver was not one of these at all.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=8551ED
    // Broiler-Human:        PENDING
    private static JsKeyedTable TableOf(JsEngine owner, JsValue receiver, string tag, bool wantsMap)
    {
        var held = receiver.AsObjectOrNull();

        if (wantsMap && held is JsMapObject map)
        {
            return map.Table;
        }

        if (!wantsMap && held is JsSetObject set)
        {
            return set.Table;
        }

        throw owner.Error("TypeError", tag + " called on a value that is not a " + (wantsMap ? "Map" : "Set"));
    }
}
