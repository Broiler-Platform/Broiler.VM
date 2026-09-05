// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   27
// Annotated:        27/27
// Exempt:           6
// Human-reviewed:   0/27
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  4/10 max
// Unverified:       27
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The keyed collections: <c>Map</c>, <c>Set</c>, <c>WeakMap</c>, <c>WeakSet</c>, <c>WeakRef</c>
/// and <c>FinalizationRegistry</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>THERE IS NO <c>Symbol.iterator</c> ON ANY OF THESE, AND THAT IS THE ONE THING TO KNOW BEFORE
/// READING FURTHER.</b> <c>keys</c>, <c>values</c>, <c>entries</c>, <c>for…of</c> over a Map, and
/// spreading a Set into an Array are all one mechanism - an iterator object reached through a
/// well-known Symbol - and this realm's collection surface was built before that mechanism was
/// wired to it. So <c>forEach</c> is how a program walks a collection here, and it is written over
/// the same slot walk the iterators will use, so that adding them later adds objects rather than
/// changing semantics. A program that reaches for <c>[...set]</c> gets a <c>TypeError</c> naming
/// the missing protocol, which is an answer it can see, rather than a wrong array.
/// </para>
/// <para>
/// <b>Construction takes an ARRAY-LIKE where the language takes an iterable, and the difference
/// cuts both ways.</b> <c>new Set([1, 2, 3])</c>, <c>new Map([[k, v]])</c> and <c>new Set("abc")</c>
/// all work, because an Array, a String and anything with a <c>length</c> can be read through the
/// ordinary property path. <c>new Set(anotherSet)</c> and <c>new Map(aGenerator)</c> do NOT, because
/// those are iterables and nothing else: the specification's Map constructor calls
/// <c>GetIterator</c>, and there is no iterator to get. The divergence is stated in both
/// directions - an object carrying a <c>length</c> is accepted here and refused by a conforming
/// engine, and an iterable that is not array-like is refused here and accepted there - and it
/// closes when Symbols reach this surface, not before.
/// </para>
/// <para>
/// <b>A String is read by CODE UNIT and not by code point.</b> <c>new Set("abc")</c> has three
/// members in every engine; <c>new Set("a😀")</c> has two in a conforming engine and
/// three here, because the iterator the specification uses walks code points and an index walk
/// walks units. That is the one place the array-like reading is visibly wrong rather than merely
/// narrow, and it is written here rather than left in a test.
/// </para>
/// <para>
/// <b>Fuel is charged per element wherever the work is per element</b> - construction, <c>clear</c>
/// and <c>forEach</c> - exactly as <c>JsRealm.Array.cs</c> charges it, and retention is reported
/// per entry stored. A table a guest fills in a loop and a table a built-in fills from an array are
/// the same amount of work and cost the same allowance; a collection that was free to build would
/// be the one hole in the meter big enough to drive a program through.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>What one stored entry is charged against the live-bytes ceiling.</summary>
    /// <remarks>
    /// A slot, a dictionary bucket and two 24-byte values, rounded up. It is an estimate and is
    /// meant to be one: the ceiling exists to stop a table from being unbounded, and being within a
    /// small factor is what that needs. An entry that cost nothing to report would make a million
    /// -entry Map free, which is the only outcome worth preventing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=1BBE3E
    // Broiler-Human:        PENDING
    private const ulong CollectionEntryBytes = 96;

    /// <summary><c>Map.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=1A31C4
    // Broiler-Human:        PENDING
    internal JsObject MapPrototype { get; private set; } = null!;

    /// <summary><c>Set.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=0006E9
    // Broiler-Human:        PENDING
    internal JsObject SetPrototype { get; private set; } = null!;

    /// <summary><c>WeakMap.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9D26EE
    // Broiler-Human:        PENDING
    internal JsObject WeakMapPrototype { get; private set; } = null!;

    /// <summary><c>WeakSet.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6B69E8
    // Broiler-Human:        PENDING
    internal JsObject WeakSetPrototype { get; private set; } = null!;

    /// <summary><c>WeakRef.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=BA54CC
    // Broiler-Human:        PENDING
    internal JsObject WeakRefPrototype { get; private set; } = null!;

    /// <summary><c>FinalizationRegistry.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=417DD2
    // Broiler-Human:        PENDING
    internal JsObject FinalizationRegistryPrototype { get; private set; } = null!;

    /// <summary>Builds every keyed collection and its prototype.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=173FAA
    // Broiler-Human:        PENDING
    private void SetupCollections()
    {
        MapPrototype = new JsObject(ObjectPrototype, "Map");
        SetPrototype = new JsObject(ObjectPrototype, "Set");
        WeakMapPrototype = new JsObject(ObjectPrototype, "WeakMap");
        WeakSetPrototype = new JsObject(ObjectPrototype, "WeakSet");
        WeakRefPrototype = new JsObject(ObjectPrototype, "WeakRef");
        FinalizationRegistryPrototype = new JsObject(ObjectPrototype, "FinalizationRegistry");

        SetupMap();
        SetupSet();
        SetupWeakMap();
        SetupWeakSet();
        SetupWeakReferences();
    }

    /// <summary>Builds <c>Map</c> and <c>Map.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=06C9C6
    // Broiler-Human:        PENDING
    private void SetupMap()
    {
        var mapConstructor = Constructor(
            "Map",
            0,
            MapPrototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor Map requires 'new'"),
            (engine, thisValue, arguments) =>
            {
                var made = new JsMapObject(MapPrototype);
                var source = ArgOfCollection(arguments, 0);

                // AN ABSENT ARGUMENT AND `null` BOTH MEAN "EMPTY", and only those two. `new Map(0)`
                // is a TypeError in every engine and is one here, because a Number is neither
                // iterable nor array-like and silently answering an empty Map would hide the call
                // that meant to pass entries.
                if (!source.IsNullish)
                {
                    // THE ENTRIES GO IN THROUGH THE OBJECT'S OWN `set`, read once, called per
                    // entry. It is not a detour: a subclass that overrides `set` sees its override
                    // used by the constructor, and a `set` that throws stops the walk - which is
                    // the only thing that ends `new Map(iterable)` over an iterator that never
                    // reports done.
                    var adder = engine.GetProperty(JsValue.Object(made), "set");

                    if (!adder.IsObject || !adder.AsObject().IsCallable)
                    {
                        return engine.ThrowTypeError("Map.prototype.set is not a function");
                    }

                    CollectionEach(engine, source, entry =>
                    {
                        if (!entry.IsObject)
                        {
                            throw engine.Error(
                                "TypeError", "Iterator value is not an entry object");
                        }

                        engine.Retain(CollectionEntryBytes);

                        engine.Call(
                            adder,
                            JsValue.Object(made),
                            [engine.GetProperty(entry, "0"), engine.GetProperty(entry, "1")]);
                    });
                }

                return JsValue.Object(made);
            });

        // A MAP AND NOT AN OBJECT, which is the difference from `Object.groupBy` beside it: the
        // key a program groups by is often not a string - a date, an object, a number that must not
        // be spelled - and a Map is the only one of the two that keeps it as itself.
        Method(mapConstructor, "groupBy", 2, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var source = ArgOfCollection(arguments, 0);
            var chooser = ArgOfCollection(arguments, 1);

            if (!chooser.IsObject || !chooser.AsObject().IsCallable)
            {
                return engine.ThrowTypeError("Map.groupBy: the callback is not a function");
            }

            var made = new JsMapObject(MapPrototype);
            var at = 0;

            foreach (var element in CollectionElements(engine, source))
            {
                engine.Charge(1);
                engine.Retain(CollectionEntryBytes);
                var key = engine.Call(chooser, JsValue.Undefined, [element, JsValue.Number(at)]);

                if (!made.Table.TryGet(key, out var held) ||
                    held.AsObjectOrNull() is not JsArray bucket)
                {
                    bucket = NewArray();
                    made.Table.Set(key, JsValue.Object(bucket));
                }

                bucket.Push(element);
                at++;
            }

            return JsValue.Object(made);
        });

        // ONE LOOKUP WHERE A PROGRAM WOULD WRITE TWO, and that is the whole of it: `has` then `set`
        // hashes the key twice and reads it twice, and a key whose hashing is expensive - a long
        // string, an object in a large table - pays for both. The pair also spells the intent, which
        // is why the language added them rather than leaving the idiom to the caller.
        SpeciesGetter(mapConstructor);

        Method(MapPrototype, "getOrInsert", 2, static (engine, thisValue, arguments) =>
        {
            var table = CollectionThisMap(engine, thisValue, "getOrInsert").Table;
            var key = ArgOfCollection(arguments, 0);
            engine.Charge(1);

            if (table.TryGet(key, out var held))
            {
                return held;
            }

            var value = ArgOfCollection(arguments, 1);
            engine.Retain(CollectionEntryBytes);
            table.Set(key, value);
            return value;
        });

        // THE CALLBACK RUNS ONLY WHEN THE KEY IS ABSENT, which is the difference from the pair
        // above and the reason both exist: a default that costs something to build - a parse, an
        // allocation, a request - should not be built for a key that is already there.
        Method(MapPrototype, "getOrInsertComputed", 2, static (engine, thisValue, arguments) =>
        {
            var table = CollectionThisMap(engine, thisValue, "getOrInsertComputed").Table;
            var key = ArgOfCollection(arguments, 0);
            var callback = ArgOfCollection(arguments, 1);

            if (!callback.IsObject || !callback.AsObject().IsCallable)
            {
                return engine.ThrowTypeError("Map.prototype.getOrInsertComputed: the callback is not a function");
            }

            engine.Charge(1);

            if (table.TryGet(key, out var held))
            {
                return held;
            }

            // THE CALLBACK IS GUEST CODE AND MAY HAVE TOUCHED THIS MAP, so the table is asked again
            // afterwards and the answer this call inserts wins: the language says the entry is set
            // after the callback returns, which overwrites whatever the callback put there.
            var value = engine.Call(callback, JsValue.Undefined, [CollectionCanonicalKey(key)]);
            engine.Retain(CollectionEntryBytes);
            table.Set(key, value);
            return value;
        });

        Method(MapPrototype, "get", 1, static (engine, thisValue, arguments) =>
            CollectionThisMap(engine, thisValue, "get").Table
                .TryGet(ArgOfCollection(arguments, 0), out var found)
                    ? found
                    : JsValue.Undefined);

        Method(MapPrototype, "has", 1, static (engine, thisValue, arguments) =>
            JsValue.Boolean(
                CollectionThisMap(engine, thisValue, "has").Table
                    .Has(ArgOfCollection(arguments, 0))));

        // `set` ANSWERS THE MAP AND NOT THE VALUE, which is what makes
        // `m.set(a, 1).set(b, 2)` the idiom it is. Returning the value would read naturally and
        // break every chained call in the wild.
        Method(MapPrototype, "set", 2, static (engine, thisValue, arguments) =>
        {
            var map = CollectionThisMap(engine, thisValue, "set");
            engine.Charge(1);
            engine.Retain(CollectionEntryBytes);
            map.Table.Set(ArgOfCollection(arguments, 0), ArgOfCollection(arguments, 1));
            return thisValue;
        });

        Method(MapPrototype, "delete", 1, static (engine, thisValue, arguments) =>
        {
            var map = CollectionThisMap(engine, thisValue, "delete");
            engine.Charge(1);
            return JsValue.Boolean(map.Table.Delete(ArgOfCollection(arguments, 0)));
        });

        Method(MapPrototype, "clear", 0, static (engine, thisValue, arguments) =>
        {
            var map = CollectionThisMap(engine, thisValue, "clear");
            engine.Charge((ulong)map.Table.SlotCount);
            map.Table.Clear();
            return JsValue.Undefined;
        });

        Method(MapPrototype, "forEach", 1, static (engine, thisValue, arguments) =>
        {
            var map = CollectionThisMap(engine, thisValue, "forEach");

            CollectionWalk(
                engine,
                map.Table,
                CollectionCallbackOf(engine, arguments, "Map.prototype.forEach"),
                ArgOfCollection(arguments, 1),
                thisValue);

            return JsValue.Undefined;
        });

        CollectionGetter(MapPrototype, "size", static (engine, thisValue, arguments) =>
            JsValue.Number(CollectionThisMap(engine, thisValue, "size").Table.Count));
    }

    /// <summary>Builds <c>Set</c> and <c>Set.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9E82CD
    // Broiler-Human:        PENDING
    private void SetupSet()
    {
        var setConstructor = Constructor(
            "Set",
            0,
            SetPrototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor Set requires 'new'"),
            (engine, thisValue, arguments) =>
            {
                var made = new JsSetObject(SetPrototype);
                var source = ArgOfCollection(arguments, 0);

                if (!source.IsNullish)
                {
                    // THROUGH THE OBJECT'S OWN `add`, for the reason the Map's constructor states.
                    var adder = engine.GetProperty(JsValue.Object(made), "add");

                    if (!adder.IsObject || !adder.AsObject().IsCallable)
                    {
                        return engine.ThrowTypeError("Set.prototype.add is not a function");
                    }

                    CollectionEach(engine, source, member =>
                    {
                        engine.Retain(CollectionEntryBytes);
                        engine.Call(adder, JsValue.Object(made), [member]);
                    });
                }

                return JsValue.Object(made);
            });

        SetupSetOperations();

        SpeciesGetter(setConstructor);

        Method(SetPrototype, "has", 1, static (engine, thisValue, arguments) =>
            JsValue.Boolean(
                CollectionThisSet(engine, thisValue, "has").Table
                    .Has(ArgOfCollection(arguments, 0))));

        // A SET MEMBER IS STORED AS BOTH KEY AND VALUE. That is what makes the Set's `forEach`
        // hand its callback `(value, value, set)` through the same walk the Map uses, which is the
        // shape the specification chose so that one callback serves both.
        Method(SetPrototype, "add", 1, static (engine, thisValue, arguments) =>
        {
            var set = CollectionThisSet(engine, thisValue, "add");
            var member = ArgOfCollection(arguments, 0);
            engine.Charge(1);
            engine.Retain(CollectionEntryBytes);
            set.Table.Set(member, member);
            return thisValue;
        });

        Method(SetPrototype, "delete", 1, static (engine, thisValue, arguments) =>
        {
            var set = CollectionThisSet(engine, thisValue, "delete");
            engine.Charge(1);
            return JsValue.Boolean(set.Table.Delete(ArgOfCollection(arguments, 0)));
        });

        Method(SetPrototype, "clear", 0, static (engine, thisValue, arguments) =>
        {
            var set = CollectionThisSet(engine, thisValue, "clear");
            engine.Charge((ulong)set.Table.SlotCount);
            set.Table.Clear();
            return JsValue.Undefined;
        });

        Method(SetPrototype, "forEach", 1, static (engine, thisValue, arguments) =>
        {
            var set = CollectionThisSet(engine, thisValue, "forEach");

            CollectionWalk(
                engine,
                set.Table,
                CollectionCallbackOf(engine, arguments, "Set.prototype.forEach"),
                ArgOfCollection(arguments, 1),
                thisValue);

            return JsValue.Undefined;
        });

        CollectionGetter(SetPrototype, "size", static (engine, thisValue, arguments) =>
            JsValue.Number(CollectionThisSet(engine, thisValue, "size").Table.Count));
    }

    /// <summary>Builds <c>WeakMap</c> and <c>WeakMap.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=978CD6
    // Broiler-Human:        PENDING
    private void SetupWeakMap()
    {
        _ = Constructor(
            "WeakMap",
            0,
            WeakMapPrototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor WeakMap requires 'new'"),
            (engine, thisValue, arguments) =>
            {
                var made = new JsWeakMapObject(WeakMapPrototype);
                var source = ArgOfCollection(arguments, 0);

                if (!source.IsNullish)
                {
                    var adder = engine.GetProperty(JsValue.Object(made), "set");

                    if (!adder.IsObject || !adder.AsObject().IsCallable)
                    {
                        return engine.ThrowTypeError("WeakMap.prototype.set is not a function");
                    }

                    CollectionEach(engine, source, entry =>
                    {
                        if (!entry.IsObject)
                        {
                            throw engine.Error(
                                "TypeError", "Iterator value is not an entry object");
                        }

                        engine.Retain(CollectionEntryBytes);

                        engine.Call(
                            adder,
                            JsValue.Object(made),
                            [engine.GetProperty(entry, "0"), engine.GetProperty(entry, "1")]);
                    });
                }

                return JsValue.Object(made);
            });

        // A PRIMITIVE KEY IS A MISS ON THE WAY OUT AND A THROW ON THE WAY IN, which is asymmetric
        // on purpose: a lookup that threw would force every caller to type-test before asking, and
        // a store that did not throw would silently drop the entry.
        Method(WeakMapPrototype, "get", 1, static (engine, thisValue, arguments) =>
        {
            var map = CollectionThisWeakMap(engine, thisValue, "get");
            var key = ArgOfCollection(arguments, 0);
            return key.AsObjectOrNull() is { } target ? map.Get(target) : JsValue.Undefined;
        });

        Method(WeakMapPrototype, "has", 1, static (engine, thisValue, arguments) =>
        {
            var map = CollectionThisWeakMap(engine, thisValue, "has");
            var key = ArgOfCollection(arguments, 0);
            return JsValue.Boolean(key.AsObjectOrNull() is { } target && map.Has(target));
        });

        Method(WeakMapPrototype, "set", 2, static (engine, thisValue, arguments) =>
        {
            var map = CollectionThisWeakMap(engine, thisValue, "set");
            var key = ArgOfCollection(arguments, 0);

            if (!key.IsObject)
            {
                throw engine.Error("TypeError", "Invalid value used as weak map key");
            }

            engine.Charge(1);
            engine.Retain(CollectionEntryBytes);
            map.Set(key.AsObject(), ArgOfCollection(arguments, 1));
            return thisValue;
        });

        Method(WeakMapPrototype, "delete", 1, static (engine, thisValue, arguments) =>
        {
            var map = CollectionThisWeakMap(engine, thisValue, "delete");
            var key = ArgOfCollection(arguments, 0);
            engine.Charge(1);
            return JsValue.Boolean(key.AsObjectOrNull() is { } target && map.Delete(target));
        });
    }

    /// <summary>Builds <c>WeakSet</c> and <c>WeakSet.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B8A16A
    // Broiler-Human:        PENDING
    private void SetupWeakSet()
    {
        _ = Constructor(
            "WeakSet",
            0,
            WeakSetPrototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor WeakSet requires 'new'"),
            (engine, thisValue, arguments) =>
            {
                var made = new JsWeakSetObject(WeakSetPrototype);
                var source = ArgOfCollection(arguments, 0);

                if (!source.IsNullish)
                {
                    var adder = engine.GetProperty(JsValue.Object(made), "add");

                    if (!adder.IsObject || !adder.AsObject().IsCallable)
                    {
                        return engine.ThrowTypeError("WeakSet.prototype.add is not a function");
                    }

                    CollectionEach(engine, source, member =>
                    {
                        engine.Retain(CollectionEntryBytes);
                        engine.Call(adder, JsValue.Object(made), [member]);
                    });
                }

                return JsValue.Object(made);
            });

        Method(WeakSetPrototype, "has", 1, static (engine, thisValue, arguments) =>
        {
            var set = CollectionThisWeakSet(engine, thisValue, "has");
            var member = ArgOfCollection(arguments, 0);
            return JsValue.Boolean(member.AsObjectOrNull() is { } target && set.Has(target));
        });

        Method(WeakSetPrototype, "add", 1, static (engine, thisValue, arguments) =>
        {
            var set = CollectionThisWeakSet(engine, thisValue, "add");
            var member = ArgOfCollection(arguments, 0);

            if (!member.IsObject)
            {
                throw engine.Error("TypeError", "Invalid value used in weak set");
            }

            engine.Charge(1);
            engine.Retain(CollectionEntryBytes);
            set.Add(member.AsObject());
            return thisValue;
        });

        Method(WeakSetPrototype, "delete", 1, static (engine, thisValue, arguments) =>
        {
            var set = CollectionThisWeakSet(engine, thisValue, "delete");
            var member = ArgOfCollection(arguments, 0);
            engine.Charge(1);
            return JsValue.Boolean(member.AsObjectOrNull() is { } target && set.Delete(target));
        });
    }

    /// <summary>Builds <c>WeakRef</c> and <c>FinalizationRegistry</c>.</summary>
    /// <remarks>
    /// The two are built together because they are one feature: a program that wants to know when
    /// something went away holds a <c>WeakRef</c> to ask and a registry to be told. Only the asking
    /// half works here - see <see cref="JsFinalizationRegistryObject"/> for why the telling half is
    /// inert, and note that a registry that never tells is exactly as useful as polling a
    /// <c>WeakRef</c>, which is what a program should do on this profile.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=D1507C
    // Broiler-Human:        PENDING
    private void SetupWeakReferences()
    {
        _ = Constructor(
            "WeakRef",
            1,
            WeakRefPrototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor WeakRef requires 'new'"),
            (engine, thisValue, arguments) =>
            {
                var target = ArgOfCollection(arguments, 0);

                if (!target.IsObject)
                {
                    throw engine.Error("TypeError", "Invalid value used as weak ref target");
                }

                engine.Charge(1);
                return JsValue.Object(new JsWeakRefObject(WeakRefPrototype, target.AsObject()));
            });

        Method(WeakRefPrototype, "deref", 0, static (engine, thisValue, arguments) =>
        {
            var reference = CollectionThisWeakRef(engine, thisValue, "deref");
            var target = reference.Deref();
            return target is null ? JsValue.Undefined : JsValue.Object(target);
        });

        _ = Constructor(
            "FinalizationRegistry",
            1,
            FinalizationRegistryPrototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor FinalizationRegistry requires 'new'"),
            (engine, thisValue, arguments) =>
            {
                var cleanup = ArgOfCollection(arguments, 0);

                // THE CALLBACK IS VALIDATED THOUGH IT IS NEVER CALLED. A registry built over a
                // non-function is a programming error the specification reports at construction,
                // and a profile that skipped the check because it would never call the value would
                // be hiding the error rather than diverging honestly about the call.
                if (!cleanup.IsObject || !cleanup.AsObject().IsCallable)
                {
                    throw engine.Error(
                        "TypeError", "FinalizationRegistry requires a cleanup callback function");
                }

                engine.Charge(1);
                return JsValue.Object(
                    new JsFinalizationRegistryObject(FinalizationRegistryPrototype, cleanup));
            });

        Method(FinalizationRegistryPrototype, "register", 2, static (engine, thisValue, arguments) =>
        {
            var registry = CollectionThisRegistry(engine, thisValue, "register");
            var target = ArgOfCollection(arguments, 0);
            var held = ArgOfCollection(arguments, 1);
            var token = ArgOfCollection(arguments, 2);

            if (!target.IsObject)
            {
                throw engine.Error(
                    "TypeError", "FinalizationRegistry.prototype.register: invalid target");
            }

            // A TARGET THAT IS ALSO ITS OWN HELD VALUE IS REFUSED, because the held value is
            // retained strongly and would keep the target alive for ever - a registration that
            // could never fire, which the specification refuses rather than accepts and drops.
            if (target.StrictlyEquals(held))
            {
                throw engine.Error(
                    "TypeError",
                    "FinalizationRegistry.prototype.register: target and holdings must not be same");
            }

            if (token.Type != JsType.Undefined && !token.IsObject)
            {
                throw engine.Error(
                    "TypeError",
                    "FinalizationRegistry.prototype.register: invalid unregisterToken");
            }

            engine.Charge(1);
            engine.Retain(CollectionEntryBytes);
            registry.Register(target.AsObject(), held, token.AsObjectOrNull());
            return JsValue.Undefined;
        });

        Method(FinalizationRegistryPrototype, "unregister", 1, static (engine, thisValue, arguments) =>
        {
            var registry = CollectionThisRegistry(engine, thisValue, "unregister");
            var token = ArgOfCollection(arguments, 0);

            if (!token.IsObject)
            {
                throw engine.Error(
                    "TypeError",
                    "FinalizationRegistry.prototype.unregister: invalid unregisterToken");
            }

            engine.Charge((ulong)registry.Count + 1);
            return JsValue.Boolean(registry.Unregister(token.AsObject()));
        });

        // `cleanupSome` IS PRESENT AND DOES NOTHING, and both halves are deliberate. It is present
        // because a program written for a host that has it should not fail to load here; it does
        // nothing because there is no sweep behind it and never calls the callback it accepts. It
        // is also a DIVERGENCE IN THE OTHER DIRECTION from everything else in this file: the
        // method is a stage-2 proposal that shipping engines do not expose by default, so
        // `typeof registry.cleanupSome` answers "function" here and "undefined" in Node.
        Method(FinalizationRegistryPrototype, "cleanupSome", 0, static (engine, thisValue, arguments) =>
        {
            var callback = ArgOfCollection(arguments, 0);

            if (callback.Type != JsType.Undefined &&
                (!callback.IsObject || !callback.AsObject().IsCallable))
            {
                throw engine.Error(
                    "TypeError",
                    "FinalizationRegistry.prototype.cleanupSome: invalid callback");
            }

            _ = CollectionThisRegistry(engine, thisValue, "cleanupSome");
            return JsValue.Undefined;
        });
    }

    /// <summary>Reads argument <paramref name="at"/>, which may not have been supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6E8CBC
    // Broiler-Human:        PENDING
    private static JsValue ArgOfCollection(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>Defines a getter-only accessor on <paramref name="host"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=248F6C
    // Broiler-Human:        PENDING
    private void CollectionGetter(JsObject host, string name, JsNativeBody body) =>
        host.SetOwnProperty(
            name,
            JsProperty.Accessor(
                Native("get " + name, 0, body), null, JsPropertyAttributes.Configurable));

    /// <summary>The receiver as a Map, or a <c>TypeError</c> naming the method.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A666F9
    // Broiler-Human:        PENDING
    private static JsMapObject CollectionThisMap(JsEngine engine, JsValue thisValue, string method) =>
        thisValue.AsObjectOrNull() as JsMapObject ??
            throw engine.Error(
                "TypeError", "Map.prototype." + method + " called on an incompatible receiver");

    /// <summary>The receiver as a Set, or a <c>TypeError</c> naming the method.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=BB679E
    // Broiler-Human:        PENDING
    private static JsSetObject CollectionThisSet(JsEngine engine, JsValue thisValue, string method) =>
        thisValue.AsObjectOrNull() as JsSetObject ??
            throw engine.Error(
                "TypeError", "Set.prototype." + method + " called on an incompatible receiver");

    /// <summary>The receiver as a WeakMap, or a <c>TypeError</c> naming the method.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=71D74D
    // Broiler-Human:        PENDING
    private static JsWeakMapObject CollectionThisWeakMap(
        JsEngine engine, JsValue thisValue, string method) =>
        thisValue.AsObjectOrNull() as JsWeakMapObject ??
            throw engine.Error(
                "TypeError", "WeakMap.prototype." + method + " called on an incompatible receiver");

    /// <summary>The receiver as a WeakSet, or a <c>TypeError</c> naming the method.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=37A587
    // Broiler-Human:        PENDING
    private static JsWeakSetObject CollectionThisWeakSet(
        JsEngine engine, JsValue thisValue, string method) =>
        thisValue.AsObjectOrNull() as JsWeakSetObject ??
            throw engine.Error(
                "TypeError", "WeakSet.prototype." + method + " called on an incompatible receiver");

    /// <summary>The receiver as a WeakRef, or a <c>TypeError</c> naming the method.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=230143
    // Broiler-Human:        PENDING
    private static JsWeakRefObject CollectionThisWeakRef(
        JsEngine engine, JsValue thisValue, string method) =>
        thisValue.AsObjectOrNull() as JsWeakRefObject ??
            throw engine.Error(
                "TypeError", "WeakRef.prototype." + method + " called on an incompatible receiver");

    /// <summary>The receiver as a FinalizationRegistry, or a <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F22CB1
    // Broiler-Human:        PENDING
    private static JsFinalizationRegistryObject CollectionThisRegistry(
        JsEngine engine, JsValue thisValue, string method) =>
        thisValue.AsObjectOrNull() as JsFinalizationRegistryObject ??
            throw engine.Error(
                "TypeError",
                "FinalizationRegistry.prototype." + method + " called on an incompatible receiver");

    /// <summary>The first argument as a callable, or a <c>TypeError</c> naming the method.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=82D649
    // Broiler-Human:        PENDING
    private static JsValue CollectionCallbackOf(
        JsEngine engine, JsValue[] arguments, string method)
    {
        var callback = ArgOfCollection(arguments, 0);

        if (!callback.IsObject || !callback.AsObject().IsCallable)
        {
            throw engine.Error("TypeError", method + " requires a callback function");
        }

        return callback;
    }

    /// <summary>
    /// Walks a table by rising slot, calling <paramref name="callback"/> for every living entry.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The bound is re-read on every step and the walk is not a snapshot.</b> A callback that
    /// adds an entry has that entry visited before the walk ends, a callback that deletes one that
    /// has not been reached yet stops it being visited, and a callback that deletes and re-adds a
    /// key visits it a second time at the back. All three are stated obligations rather than
    /// emergent behaviour, and all three fall out of walking slots rather than copying entries.
    /// </para>
    /// <para>
    /// The <c>finally</c> is what makes the table compactable again after a callback throws. Without
    /// it, one exception out of one <c>forEach</c> would leave the table believing an iteration was
    /// still running and its tombstones uncollectable for the life of the realm.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=D6C4FD
    // Broiler-Human:        PENDING
    private static void CollectionWalk(
        JsEngine engine,
        JsKeyedTable table,
        JsValue callback,
        JsValue thisArg,
        JsValue receiver)
    {
        table.EnterIteration();

        try
        {
            for (var slot = 0; slot < table.SlotCount; slot++)
            {
                engine.Charge(1);

                if (!table.TryAt(slot, out var key, out var value))
                {
                    continue;
                }

                _ = engine.Call(callback, thisArg, [value, key, receiver]);
            }
        }
        finally
        {
            table.ExitIteration();
        }
    }

    /// <summary>The seven set operations, which arrived long after <c>Set</c> itself.</summary>
    /// <remarks>
    /// <para>
    /// <b>The argument is a SET-LIKE and not a Set</b>, and that is the whole design of these seven:
    /// each reads a <c>size</c>, a <c>has</c> and a <c>keys</c> off whatever it is handed, so a Map,
    /// a custom collection, or anything else with those three participates. Requiring a real Set
    /// would have made the methods useless for exactly the objects a program most wants to combine
    /// with one.
    /// </para>
    /// <para>
    /// <b>Which side is walked depends on which is smaller, and it is OBSERVABLE.</b>
    /// <c>intersection</c>, <c>difference</c> and <c>isDisjointFrom</c> walk the receiver when it is
    /// the smaller and the argument's keys when it is not - so the result's ORDER and the calls the
    /// argument sees both differ with the sizes. That is the specification's own arrangement, not an
    /// optimisation taken here, and reproducing it is the job.
    /// </para>
    /// <para>
    /// <b>The declared size is read once and trusted for the branch</b>, which is why a set-like
    /// that lies about its size is a case the suite tests: the branch is chosen from the number it
    /// reported, and the walk that follows uses its real contents.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=DFD771
    // Broiler-Human:        PENDING
    private void SetupSetOperations()
    {
        Method(SetPrototype, "union", 1, (engine, thisValue, arguments) =>
        {
            var table = CollectionThisSet(engine, thisValue, "union").Table;
            var other = CollectionSetRecord(engine, ArgOfCollection(arguments, 0), "union");
            var made = new JsSetObject(SetPrototype);
            CollectionCopyInto(engine, table, made);

            foreach (var member in CollectionSetKeys(engine, other))
            {
                engine.Charge(1);
                engine.Retain(CollectionEntryBytes);
                var key = CollectionCanonicalKey(member);
                made.Table.Set(key, key);
            }

            return JsValue.Object(made);
        });

        Method(SetPrototype, "intersection", 1, (engine, thisValue, arguments) =>
        {
            var table = CollectionThisSet(engine, thisValue, "intersection").Table;
            var other = CollectionSetRecord(engine, ArgOfCollection(arguments, 0), "intersection");
            var made = new JsSetObject(SetPrototype);

            if (table.Count <= other.Size)
            {
                foreach (var member in CollectionMembers(table))
                {
                    engine.Charge(1);

                    if (engine.Call(other.Has, other.Receiver, [member]).ToBooleanValue())
                    {
                        made.Table.Set(member, member);
                    }
                }

                return JsValue.Object(made);
            }

            foreach (var member in CollectionSetKeys(engine, other))
            {
                engine.Charge(1);
                var key = CollectionCanonicalKey(member);

                if (table.Has(key))
                {
                    made.Table.Set(key, key);
                }
            }

            return JsValue.Object(made);
        });

        Method(SetPrototype, "difference", 1, (engine, thisValue, arguments) =>
        {
            var table = CollectionThisSet(engine, thisValue, "difference").Table;
            var other = CollectionSetRecord(engine, ArgOfCollection(arguments, 0), "difference");
            var made = new JsSetObject(SetPrototype);
            CollectionCopyInto(engine, table, made);

            if (table.Count <= other.Size)
            {
                foreach (var member in CollectionMembers(table))
                {
                    engine.Charge(1);

                    if (engine.Call(other.Has, other.Receiver, [member]).ToBooleanValue())
                    {
                        made.Table.Delete(member);
                    }
                }

                return JsValue.Object(made);
            }

            foreach (var member in CollectionSetKeys(engine, other))
            {
                engine.Charge(1);
                made.Table.Delete(CollectionCanonicalKey(member));
            }

            return JsValue.Object(made);
        });

        Method(SetPrototype, "symmetricDifference", 1, (engine, thisValue, arguments) =>
        {
            var table = CollectionThisSet(engine, thisValue, "symmetricDifference").Table;
            var other = CollectionSetRecord(
                engine, ArgOfCollection(arguments, 0), "symmetricDifference");

            var made = new JsSetObject(SetPrototype);
            CollectionCopyInto(engine, table, made);

            // THE RECEIVER IS ASKED AND THE RESULT IS CHANGED, which are two different tables here
            // for a reason: a member the argument yields twice would otherwise be removed and then
            // added back, and the language says the membership question is about the RECEIVER.
            foreach (var member in CollectionSetKeys(engine, other))
            {
                engine.Charge(1);
                var key = CollectionCanonicalKey(member);

                if (table.Has(key))
                {
                    made.Table.Delete(key);
                }
                else
                {
                    engine.Retain(CollectionEntryBytes);
                    made.Table.Set(key, key);
                }
            }

            return JsValue.Object(made);
        });

        Method(SetPrototype, "isSubsetOf", 1, (engine, thisValue, arguments) =>
        {
            var table = CollectionThisSet(engine, thisValue, "isSubsetOf").Table;
            var other = CollectionSetRecord(engine, ArgOfCollection(arguments, 0), "isSubsetOf");

            if (table.Count > other.Size)
            {
                return JsValue.False;
            }

            foreach (var member in CollectionMembers(table))
            {
                engine.Charge(1);

                if (!engine.Call(other.Has, other.Receiver, [member]).ToBooleanValue())
                {
                    return JsValue.False;
                }
            }

            return JsValue.True;
        });

        Method(SetPrototype, "isSupersetOf", 1, (engine, thisValue, arguments) =>
        {
            var table = CollectionThisSet(engine, thisValue, "isSupersetOf").Table;
            var other = CollectionSetRecord(engine, ArgOfCollection(arguments, 0), "isSupersetOf");

            if (table.Count < other.Size)
            {
                return JsValue.False;
            }

            foreach (var member in CollectionSetKeys(engine, other))
            {
                engine.Charge(1);

                if (!table.Has(CollectionCanonicalKey(member)))
                {
                    return JsValue.False;
                }
            }

            return JsValue.True;
        });

        Method(SetPrototype, "isDisjointFrom", 1, (engine, thisValue, arguments) =>
        {
            var table = CollectionThisSet(engine, thisValue, "isDisjointFrom").Table;
            var other = CollectionSetRecord(engine, ArgOfCollection(arguments, 0), "isDisjointFrom");

            if (table.Count <= other.Size)
            {
                foreach (var member in CollectionMembers(table))
                {
                    engine.Charge(1);

                    if (engine.Call(other.Has, other.Receiver, [member]).ToBooleanValue())
                    {
                        return JsValue.False;
                    }
                }

                return JsValue.True;
            }

            foreach (var member in CollectionSetKeys(engine, other))
            {
                engine.Charge(1);

                if (table.Has(CollectionCanonicalKey(member)))
                {
                    return JsValue.False;
                }
            }

            return JsValue.True;
        });
    }

    /// <summary>What one of the seven reads off the object it was handed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B18E2A
    // Broiler-Human:        PENDING
    private readonly record struct JsSetLike(JsValue Receiver, double Size, JsValue Has, JsValue Keys);

    /// <summary>The specification's <c>GetSetRecord</c>, in the order it reads its three members.</summary>
    /// <remarks>
    /// <b>The order is observable and is therefore fixed here</b>: the size is read and coerced
    /// first - so a <c>size</c> getter runs before a <c>has</c> getter - a <c>NaN</c> size is a
    /// <c>TypeError</c> where a negative one is a <c>RangeError</c>, and only then are the two
    /// methods read and tested for callability.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=181D66
    // Broiler-Human:        PENDING
    private static JsSetLike CollectionSetRecord(JsEngine engine, JsValue other, string member)
    {
        if (!other.IsObject)
        {
            throw engine.Error("TypeError", "Set.prototype." + member + ": the argument is not an object");
        }

        var declared = engine.ToNumber(engine.GetProperty(other, "size"));

        if (double.IsNaN(declared))
        {
            throw engine.Error("TypeError", "Set.prototype." + member + ": the size is not a number");
        }

        var size = JsValue.ToInteger(declared);

        if (size < 0)
        {
            throw engine.Error("RangeError", "Set.prototype." + member + ": the size is negative");
        }

        var has = engine.GetProperty(other, "has");

        if (!has.IsObject || !has.AsObject().IsCallable)
        {
            throw engine.Error("TypeError", "Set.prototype." + member + ": `has` is not a function");
        }

        var keys = engine.GetProperty(other, "keys");

        if (!keys.IsObject || !keys.AsObject().IsCallable)
        {
            throw engine.Error("TypeError", "Set.prototype." + member + ": `keys` is not a function");
        }

        return new JsSetLike(other, size, has, keys);
    }

    /// <summary>Walks the set-like's own <c>keys</c> iterator, one step at a time.</summary>
    /// <remarks>
    /// <b>LAZILY, and the difference is observable through the argument.</b> A consumer that has its
    /// answer stops asking - <c>isDisjointFrom</c> stops at the first common member - and the
    /// specification's traces are written against exactly that: draining the iterator first shows up
    /// as extra <c>next</c> calls the argument can see and count. Stopping early closes the
    /// iterator, which is what tells a generator to run its <c>finally</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=08814F
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.IEnumerable<JsValue> CollectionSetKeys(
        JsEngine engine, JsSetLike other)
    {
        var iterator = engine.Call(other.Keys, other.Receiver, System.Array.Empty<JsValue>());

        if (!iterator.IsObject)
        {
            throw engine.Error("TypeError", "`keys` did not answer an iterator");
        }

        // THE ITERATOR IS ALREADY OPEN, so the record is built around it rather than by asking the
        // object for one: `keys` answered an iterator, and asking THAT for its `Symbol.iterator`
        // would be a second protocol step the language does not take here.
        var record = new JsIteratorRecord(iterator, engine.GetProperty(iterator, "next"));
        var drained = false;

        try
        {
            while (engine.TryIterateNext(record, out var member))
            {
                engine.Charge(1);
                yield return member;
            }

            drained = true;
        }
        finally
        {
            if (!drained)
            {
                engine.CloseIteratorQuietly(record);
            }
        }
    }

    /// <summary>The receiver's members, in insertion order, as they stand WHEN EACH IS REACHED.</summary>
    /// <remarks>
    /// <b>A slot walk over the live table and not a snapshot of it.</b> The argument's <c>has</c> is
    /// guest code and may delete from the receiver, and the language says a member deleted before
    /// the walk reaches it is not visited - which a copy taken up front would visit anyway. The
    /// bound is taken once, so a member APPENDED during the walk is not visited either.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=2A70D5
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.IEnumerable<JsValue> CollectionMembers(JsKeyedTable table)
    {
        var bound = table.SlotCount;

        for (var slot = 0; slot < bound; slot++)
        {
            if (table.TryAt(slot, out var key, out _))
            {
                yield return key;
            }
        }
    }

    /// <summary>Copies one Set's members into another, in order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=7E7F9E
    // Broiler-Human:        PENDING
    private static void CollectionCopyInto(JsEngine engine, JsKeyedTable table, JsSetObject made)
    {
        for (var slot = 0; slot < table.SlotCount; slot++)
        {
            if (!table.TryAt(slot, out var key, out _))
            {
                continue;
            }

            engine.Charge(1);
            engine.Retain(CollectionEntryBytes);
            made.Table.Set(key, key);
        }
    }

    /// <summary>The key a collection stores, which is the key it was given with one exception.</summary>
    /// <remarks>
    /// <b><c>-0</c> is stored as <c>+0</c>.</b> The table keys on <c>SameValueZero</c>, under which
    /// the two are one key; the language says the stored key is the normalized one, so a callback
    /// handed the key of an entry it is about to create is handed <c>+0</c> even where the caller
    /// wrote <c>-0</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=AA04C6
    // Broiler-Human:        PENDING
    private static JsValue CollectionCanonicalKey(JsValue key) =>
        key.IsNumber && key.AsNumber() == 0 ? JsValue.Number(0) : key;

    /// <summary>Walks a source ONE element at a time, handing each to <paramref name="step"/>.</summary>
    /// <remarks>
    /// <para>
    /// <b>The constructors cannot collect first and store second, and an infinite iterator is why.</b>
    /// The language builds a Map by reading the collection's own <c>set</c> once and CALLING it per
    /// entry, so a <c>set</c> that throws stops the walk at the first entry — which is the only thing
    /// that terminates <c>new Map(iterable)</c> over an iterator that never reports done. Collecting
    /// the elements into a list first runs that iterator for ever, and this realm did exactly that
    /// until the pinned suite spent its wall clock on it.
    /// </para>
    /// <para>
    /// <b>An abrupt step closes the iterator</b>, which is the other half of the same rule: the
    /// iterator is told the consumer stopped early, so a generator's <c>finally</c> runs and a
    /// hand-written iterator's <c>return</c> is called exactly once.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=4835DC
    // Broiler-Human:        PENDING
    private static void CollectionEach(JsEngine engine, JsValue source, System.Action<JsValue> step)
    {
        if (!source.IsObject && !source.IsString)
        {
            throw engine.Error("TypeError", "the argument is not iterable");
        }

        if (engine.TryGetSymbolMethod(source, engine.Realm.IteratorSymbol, out _))
        {
            var record = engine.GetIterator(source);

            while (engine.TryIterateNext(record, out var element))
            {
                engine.Charge(1);

                try
                {
                    step(element);
                }
                catch (JsThrow)
                {
                    engine.CloseIteratorQuietly(record);
                    throw;
                }
            }

            return;
        }

        var declared = engine.GetProperty(source, "length");

        if (declared.Type == JsType.Undefined)
        {
            throw engine.Error("TypeError", "the argument is not iterable");
        }

        var length = JsValue.ToInteger(engine.ToNumber(declared));

        if (length > 4294967295.0)
        {
            throw engine.Error("RangeError", "Invalid collection length");
        }

        for (uint at = 0; at < length; at++)
        {
            engine.Charge(1);
            step(engine.GetProperty(source, JsNumberFormat.ToUintString(at)));
        }
    }

    /// <summary>
    /// Reads an array-like into a list, which is this realm's stand-in for iterating an iterable.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Everything is read through <see cref="JsEngine.GetProperty"/> and never off
    /// <see cref="JsArray"/>'s dense list, so a getter on an element, an index inherited from
    /// <c>Array.prototype</c> and a Proxy-shaped object with a <c>length</c> all behave the way the
    /// ordinary property path says they do. That also means a getter can run guest code in the
    /// middle of a construction, which is why the elements are collected into a list FIRST and
    /// stored SECOND: a getter that mutated the collection being built would otherwise be mutating
    /// it under the loop filling it.
    /// </para>
    /// <para>
    /// A value with no <c>length</c> and no <c>Symbol.iterator</c> is a <c>TypeError</c> and not an
    /// empty collection. Answering empty would turn every "I passed the wrong thing" into a
    /// silently empty Map, which is the failure mode with no symptom.
    /// </para>
    /// <para>
    /// <b>THE ITERATION PROTOCOL COMES FIRST, and this method's remarks said otherwise until
    /// 2026-09-04.</b> It described itself as this realm's stand-in for iterating an iterable and
    /// read an array-like instead, which was the only reading available while the realm had no
    /// <c>Symbol</c>. It stopped being the only reading and did not stop being what the code did:
    /// <c>new Map(generator())</c> and <c>new Set(userIterable)</c> answered a <c>TypeError</c>
    /// saying the argument is not iterable, about an argument that is. The array-like reading stays
    /// as the fallback, because the specification's own <c>AddEntriesFromIterable</c> is reached
    /// only through an iterator and an Array is one.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=4DF0AE
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<JsValue> CollectionElements(
        JsEngine engine, JsValue source)
    {
        var values = new System.Collections.Generic.List<JsValue>();

        if (!source.IsObject && !source.IsString)
        {
            throw engine.Error("TypeError", "the argument is not iterable");
        }

        if (engine.TryGetSymbolMethod(source, engine.Realm.IteratorSymbol, out _))
        {
            engine.IterateInto(source, values);
            return values;
        }

        var declared = engine.GetProperty(source, "length");

        if (declared.Type == JsType.Undefined)
        {
            throw engine.Error("TypeError", "the argument is not iterable");
        }

        var length = JsValue.ToInteger(engine.ToNumber(declared));

        if (length <= 0)
        {
            return values;
        }

        if (length > 4294967295.0)
        {
            throw engine.Error("RangeError", "Invalid collection length");
        }

        for (uint at = 0; at < length; at++)
        {
            engine.Charge(1);
            values.Add(engine.GetProperty(source, JsNumberFormat.ToUintString(at)));
        }

        return values;
    }
}
