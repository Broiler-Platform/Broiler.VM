// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   25
// Annotated:        25/25
// Exempt:           15
// Human-reviewed:   0/25
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       25
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>Object</c> intrinsic: the constructor, <c>Object.prototype</c>, and the static
/// reflection surface that reads and writes property descriptors.
/// </summary>
/// <remarks>
/// <para>
/// <b><c>defineProperty</c> is the checked form and <c>SetOwnProperty</c> is the unchecked one.</b>
/// Everything here that a program can reach goes through
/// <see cref="ObjectApplyDescriptor"/>, which compares the descriptor it was handed against the
/// property that is already there and refuses the redefinitions a non-configurable property
/// forbids. An implementation that forwarded <c>Object.defineProperty</c> straight to the object's
/// own store would let a program rewrite a frozen constant and would report <c>true</c> from
/// <c>Object.isFrozen</c> the whole time.
/// </para>
/// <para>
/// <b>A descriptor object is read with <c>HasProperty</c> and not with <c>hasOwnProperty</c>.</b>
/// The specification reads each field off the prototype chain, so a descriptor that inherits
/// <c>enumerable</c> from its prototype carries it. The difference is only ever observable through
/// a hand-built descriptor, which is exactly what a conformance suite hands it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>Which of the three shapes <c>keys</c>, <c>values</c> and <c>entries</c> yield.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BA9CBF
    // Broiler-Human:        PENDING
    private enum ObjectEntryKind
    {
        /// <summary>The property name alone.</summary>
        Key = 0,

        /// <summary>The property value alone.</summary>
        Value = 1,

        /// <summary>A two-element Array holding the name and the value.</summary>
        Pair = 2,
    }

    /// <summary>Builds <c>Object</c>, <c>Object.prototype</c> and the statics on the constructor.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EA7390
    // Broiler-Human:        PENDING
    private void SetupObject()
    {
        Method(ObjectPrototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;

            // THE TAG IS THE RECEIVER'S CLASS, and `undefined` and `null` are named before any
            // coercion is attempted - `ToObject` would throw on both, so a toString that coerced
            // first could never produce the two answers the specification names for them.
            if (thisValue.Type == JsType.Undefined)
            {
                return JsValue.String("[object Undefined]");
            }

            if (thisValue.Type == JsType.Null)
            {
                return JsValue.String("[object Null]");
            }

            var host = engine.ToObject(thisValue);

            // `Symbol.toStringTag` OVERRIDES THE CLASS AND IS WHAT MAKES THIS EXTENSIBLE. A guest
            // object carrying one reports it — which is how `[object Generator]`, `[object Map]`
            // and every tag a program sets on a class of its own are produced. A tag that is not a
            // String is ignored rather than coerced, because a tag is a name and coercing one would
            // let `[object 42]` through.
            var tagged = engine.GetSymbol(thisValue, engine.Realm.ToStringTagSymbol);

            return JsValue.String(
                "[object " + (tagged.Type == JsType.String ? tagged.AsString() : host.ClassName) + "]");
        });

        Method(ObjectPrototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var method = engine.GetProperty(thisValue, "toString");
            return engine.Call(method, thisValue, System.Array.Empty<JsValue>());
        });

        Method(ObjectPrototype, "valueOf", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            return JsValue.Object(engine.ToObject(thisValue));
        });

        // `__proto__` IS AN ACCESSOR ON THIS PROTOTYPE AND NOT A DATA PROPERTY OF EVERY OBJECT.
        //
        // It is Annex B and it is not going anywhere: `o.__proto__ = null` and
        // `({}).__proto__ === Object.prototype` are written by real programs, and an object literal
        // with a `__proto__` key is how a great deal of code sets a prototype. Without it, the
        // assignment created an ORDINARY OWN PROPERTY NAMED `__proto__` - a wrong answer that looks
        // like a right one, because every later read of `o.__proto__` returns what was stored and
        // nothing reports that the prototype never moved.
        //
        // The getter and the setter are the specification's, which is why the setter answers
        // `undefined` rather than throwing for a primitive receiver or a non-object value: the two
        // ways to be a no-op are distinguished by the receiver, not by the argument.
        ObjectPrototype.SetOwnProperty(
            "__proto__",
            JsProperty.Accessor(
                Native("get __proto__", 0, static (engine, thisValue, arguments) =>
                {
                    _ = arguments;
                    var held = engine.ToObject(thisValue).Prototype;
                    return held is null ? JsValue.Null : JsValue.Object(held);
                }),
                Native("set __proto__", 1, static (engine, thisValue, arguments) =>
                {
                    var value = ArgOfObject(arguments, 0);

                    if (!thisValue.IsObject || (!value.IsObject && value.Type != JsType.Null))
                    {
                        return JsValue.Undefined;
                    }

                    ObjectSetPrototype(
                        engine, thisValue.AsObject(), value.Type == JsType.Null ? null : value.AsObject());

                    return JsValue.Undefined;
                }),
                JsPropertyAttributes.Configurable));

        // THE FOUR ANNEX B ACCESSOR HELPERS. They are older than `Object.defineProperty` and the
        // language keeps them because the web does; a program that uses one is usually old rather
        // than wrong, and answering `undefined is not a function` for it is a refusal dressed as a
        // bug. `__defineGetter__` DEFINES rather than assigns, so it replaces a data property with
        // an accessor - which is the whole reason it exists.
        Method(ObjectPrototype, "__defineGetter__", 2, static (engine, thisValue, arguments) =>
        {
            var host = engine.ToObject(thisValue);
            var accessor = ArgOfObject(arguments, 1);

            if (!accessor.IsObject || !accessor.AsObject().IsCallable)
            {
                return engine.ThrowTypeError("Object.prototype.__defineGetter__: the getter is not callable");
            }

            ObjectDefineAccessorHalf(engine, host, ArgOfObject(arguments, 0), accessor, getter: true);
            return JsValue.Undefined;
        });

        Method(ObjectPrototype, "__defineSetter__", 2, static (engine, thisValue, arguments) =>
        {
            var host = engine.ToObject(thisValue);
            var accessor = ArgOfObject(arguments, 1);

            if (!accessor.IsObject || !accessor.AsObject().IsCallable)
            {
                return engine.ThrowTypeError("Object.prototype.__defineSetter__: the setter is not callable");
            }

            ObjectDefineAccessorHalf(engine, host, ArgOfObject(arguments, 0), accessor, getter: false);
            return JsValue.Undefined;
        });

        // THE LOOKUP PAIR WALKS THE PROTOTYPE CHAIN, which is what distinguishes them from
        // `getOwnPropertyDescriptor`: they answer about the property a read would REACH.
        Method(ObjectPrototype, "__lookupGetter__", 1, static (engine, thisValue, arguments) =>
            ObjectLookupAccessorHalf(engine, thisValue, ArgOfObject(arguments, 0), getter: true));

        Method(ObjectPrototype, "__lookupSetter__", 1, static (engine, thisValue, arguments) =>
            ObjectLookupAccessorHalf(engine, thisValue, ArgOfObject(arguments, 0), getter: false));

        Method(ObjectPrototype, "hasOwnProperty", 1, static (engine, thisValue, arguments) =>
        {
            var requested = ArgOfObject(arguments, 0);
            var host = engine.ToObject(thisValue);

            // A SYMBOL KEY IS A DIFFERENT TABLE AND SO A DIFFERENT QUESTION. Coercing it to a
            // String first would ask about a property named `Symbol(x)`, which no object has and
            // every object could.
            return JsValue.Boolean(
                requested.IsSymbol
                    ? host.TryGetOwnSymbol(requested.AsSymbol(), out _)
                    : host.HasOwnProperty(engine.ToPropertyKey(requested)));
        });

        Method(ObjectPrototype, "isPrototypeOf", 1, static (engine, thisValue, arguments) =>
        {
            var candidate = ArgOfObject(arguments, 0);

            if (!candidate.IsObject)
            {
                return JsValue.False;
            }

            var target = engine.ToObject(thisValue);
            var walk = candidate.AsObject().Prototype;

            while (walk is not null)
            {
                engine.Charge(1);

                if (ReferenceEquals(walk, target))
                {
                    return JsValue.True;
                }

                walk = walk.Prototype;
            }

            return JsValue.False;
        });

        Method(ObjectPrototype, "propertyIsEnumerable", 1, static (engine, thisValue, arguments) =>
        {
            var requested = ArgOfObject(arguments, 0);
            var target = engine.ToObject(thisValue);

            if (requested.IsSymbol)
            {
                return JsValue.Boolean(
                    target.TryGetOwnSymbol(requested.AsSymbol(), out var owned) && owned.Enumerable);
            }

            var key = engine.ToPropertyKey(requested);
            return JsValue.Boolean(target.TryGetOwnProperty(key, out var property) && property.Enumerable);
        });

        // `Object(v)` and `new Object(v)` are the same operation: a nullish argument produces a
        // fresh ordinary object and anything else produces the wrapper `ToObject` would.
        JsValue FromValue(JsEngine engine, JsValue thisValue, JsValue[] arguments)
        {
            _ = thisValue;
            var value = ArgOfObject(arguments, 0);

            return value.IsNullish
                ? JsValue.Object(new JsObject(ObjectPrototype))
                : JsValue.Object(engine.ToObject(value));
        }

        var constructor = Constructor("Object", 1, ObjectPrototype, FromValue, FromValue);

        Method(constructor, "keys", 1, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            return ObjectEnumerate(engine, ArgOfObject(arguments, 0), ObjectEntryKind.Key);
        });

        Method(constructor, "values", 1, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            return ObjectEnumerate(engine, ArgOfObject(arguments, 0), ObjectEntryKind.Value);
        });

        Method(constructor, "entries", 1, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            return ObjectEnumerate(engine, ArgOfObject(arguments, 0), ObjectEntryKind.Pair);
        });

        // THE INVERSE OF `entries`, AND IT TAKES AN ITERABLE RATHER THAN AN ARRAY. That is what
        // makes `Object.fromEntries(map)` work, which is the shape most programs use it in: a Map
        // iterates as [key, value] pairs and needs no conversion first.
        Method(constructor, "fromEntries", 1, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var source = ArgOfObject(arguments, 0);

            if (source.IsNullish)
            {
                return engine.ThrowTypeError("Object.fromEntries requires an iterable argument");
            }

            var made = new JsObject(ObjectPrototype);

            foreach (var entry in CollectionElements(engine, source))
            {
                engine.Charge(1);

                if (!entry.IsObject)
                {
                    return engine.ThrowTypeError("Iterator value " + engine.ToStringValue(entry) +
                        " is not an entry object");
                }

                var key = engine.GetIndexed(entry, JsValue.Number(0));
                var value = engine.GetIndexed(entry, JsValue.Number(1));

                // A DEFINITION AND NOT AN ASSIGNMENT, which is the same distinction a computed
                // member of an object literal makes: a key of `__proto__` becomes an own property
                // here rather than moving the object's prototype.
                if (key.IsSymbol)
                {
                    made.SetOwnSymbol(
                        key.AsSymbol(), JsProperty.Data(value, JsPropertyAttributes.Default));
                }
                else
                {
                    made.SetOwnProperty(
                        engine.ToPropertyKey(key),
                        JsProperty.Data(value, JsPropertyAttributes.Default));
                }
            }

            return JsValue.Object(made);
        });

        // THE GROUPS ARE AN OBJECT WITH A NULL PROTOTYPE, and that is the whole reason this method
        // is worth having over the four lines a program would write instead: a group key of
        // `toString` or `constructor` does not collide with anything, because there is nothing on
        // the chain to collide with.
        Method(constructor, "groupBy", 2, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var source = ArgOfObject(arguments, 0);
            var chooser = ArgOfObject(arguments, 1);

            if (!chooser.IsObject || !chooser.AsObject().IsCallable)
            {
                return engine.ThrowTypeError("Object.groupBy: the callback is not a function");
            }

            var groups = new JsObject(null);
            var at = 0;

            foreach (var element in CollectionElements(engine, source))
            {
                engine.Charge(1);
                var key = engine.ToPropertyKey(
                    engine.Call(chooser, JsValue.Undefined, [element, JsValue.Number(at)]));

                if (!groups.TryGetOwnProperty(key, out var held) || held.Value.AsObjectOrNull() is not JsArray bucket)
                {
                    bucket = NewArray();
                    groups.SetOwnProperty(
                        key, JsProperty.Data(JsValue.Object(bucket), JsPropertyAttributes.Default));
                }

                bucket.Push(element);
                at++;
            }

            return JsValue.Object(groups);
        });

        // `hasOwnProperty` WITHOUT THE RECEIVER, which is the point: `o.hasOwnProperty(k)` is a
        // method call on `o`, so it fails for an object with a null prototype and lies for one that
        // shadowed the name. The idiom that worked was
        // `Object.prototype.hasOwnProperty.call(o, k)`, and this is that idiom with a name.
        Method(constructor, "hasOwn", 2, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var host = engine.ToObject(ArgOfObject(arguments, 0));
            var requested = ArgOfObject(arguments, 1);

            return JsValue.Boolean(
                requested.IsSymbol
                    ? host.TryGetOwnSymbol(requested.AsSymbol(), out _)
                    : host.HasOwnProperty(engine.ToPropertyKey(requested)));
        });

        Method(constructor, "getOwnPropertyNames", 1, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = engine.ToObject(ArgOfObject(arguments, 0));
            var result = NewArray();

            foreach (var key in target.OwnPropertyNames())
            {
                engine.Charge(1);
                result.Push(JsValue.String(key));
            }

            return JsValue.Object(result);
        });

        // THE SYMBOL KEYS ARE A SEPARATE TABLE AND THEREFORE A SEPARATE ANSWER. `getOwnPropertyNames`
        // must not report them and this must report nothing else, which is the whole reason the
        // language has two functions where a reader might expect one filter.
        Method(constructor, "getOwnPropertySymbols", 1, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = engine.ToObject(ArgOfObject(arguments, 0));
            var result = NewArray();

            foreach (var key in target.OwnSymbolKeys())
            {
                engine.Charge(1);
                result.Push(JsValue.Symbol(key));
            }

            return JsValue.Object(result);
        });

        Method(constructor, "getPrototypeOf", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var prototype = engine.ToObject(ArgOfObject(arguments, 0)).Prototype;
            return prototype is null ? JsValue.Null : JsValue.Object(prototype);
        });

        Method(constructor, "setPrototypeOf", 2, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ArgOfObject(arguments, 0);

            if (target.IsNullish)
            {
                return engine.ThrowTypeError("Object.setPrototypeOf called on null or undefined");
            }

            var prototype = ArgOfObject(arguments, 1);

            if (!prototype.IsObject && prototype.Type != JsType.Null)
            {
                return engine.ThrowTypeError("Object prototype may only be an Object or null");
            }

            if (target.IsObject)
            {
                ObjectSetPrototype(engine, target.AsObject(), prototype.AsObjectOrNull());
            }

            return target;
        });

        Method(constructor, "create", 2, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var prototype = ArgOfObject(arguments, 0);

            if (!prototype.IsObject && prototype.Type != JsType.Null)
            {
                return engine.ThrowTypeError("Object prototype may only be an Object or null");
            }

            var created = new JsObject(prototype.AsObjectOrNull());
            var properties = ArgOfObject(arguments, 1);

            if (properties.Type != JsType.Undefined)
            {
                ObjectDefineFromProperties(engine, created, properties);
            }

            return JsValue.Object(created);
        });

        Method(constructor, "defineProperty", 3, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ArgOfObject(arguments, 0);

            if (!target.IsObject)
            {
                return engine.ThrowTypeError("Object.defineProperty called on non-object");
            }

            var requested = ArgOfObject(arguments, 1);
            var fields = ObjectToDescriptorFields(engine, ArgOfObject(arguments, 2));

            if (requested.IsSymbol)
            {
                // A PROXY IS ASKED THROUGH ITS TRAP, with the fields the caller actually wrote:
                // an unchecked `SetOwnSymbol` would reach the same trap but with a descriptor
                // completed to four keys, and the trap is entitled to see the one it was given.
                if (target.AsObject() is JsProxy proxy)
                {
                    ObjectDefinedOrThrow(engine, proxy, requested, fields);
                    return target;
                }

                // THE SYMBOL TABLE HAS NO REDEFINITION RULES OF ITS OWN YET, so this defines
                // rather than validating against a current descriptor the way the String path does.
                // A Symbol-keyed property is either absent or defined by the code that owns the
                // Symbol, and nothing in this surface can redefine one it did not create.
                target.AsObject().SetOwnSymbol(
                    requested.AsSymbol(), ObjectPropertyFromFields(engine, target.AsObject(), fields));

                return target;
            }

            ObjectApplyDescriptor(engine, target.AsObject(), engine.ToPropertyKey(requested), fields);
            return target;
        });

        Method(constructor, "defineProperties", 2, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ArgOfObject(arguments, 0);

            if (!target.IsObject)
            {
                return engine.ThrowTypeError("Object.defineProperties called on non-object");
            }

            ObjectDefineFromProperties(engine, target.AsObject(), ArgOfObject(arguments, 1));
            return target;
        });

        Method(constructor, "getOwnPropertyDescriptor", 2, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = engine.ToObject(ArgOfObject(arguments, 0));
            var requested = ArgOfObject(arguments, 1);

            if (requested.IsSymbol)
            {
                return target.TryGetOwnSymbol(requested.AsSymbol(), out var owned)
                    ? JsValue.Object(ObjectDescriptorFor(owned))
                    : JsValue.Undefined;
            }

            var key = engine.ToPropertyKey(requested);

            return target.TryGetOwnProperty(key, out var property)
                ? JsValue.Object(ObjectDescriptorFor(property))
                : JsValue.Undefined;
        });

        Method(constructor, "getOwnPropertyDescriptors", 1, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = engine.ToObject(ArgOfObject(arguments, 0));
            var result = new JsObject(ObjectPrototype);

            // BOTH KEY TABLES. The plural in the name is the whole difference from
            // `getOwnPropertyDescriptor`, and an answer that omitted the Symbol-keyed properties
            // was not the descriptors of the object but the descriptors of half of it.
            foreach (var key in target.OwnKeys())
            {
                engine.Charge(1);

                if (ObjectOwnAt(target, key, out var property))
                {
                    ObjectSetOwnAt(
                        result,
                        key,
                        JsProperty.Data(
                            JsValue.Object(ObjectDescriptorFor(property)),
                            JsPropertyAttributes.Default));
                }
            }

            return JsValue.Object(result);
        });

        Method(constructor, "assign", 2, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var receiver = JsValue.Object(engine.ToObject(ArgOfObject(arguments, 0)));

            for (var at = 1; at < arguments.Length; at++)
            {
                var next = arguments[at];

                if (next.IsNullish)
                {
                    continue;
                }

                var source = engine.ToObject(next);
                var reader = JsValue.Object(source);

                // BOTH KEY TABLES, in the specification's order: String keys and then Symbol keys.
                // Copying only the String half meant a Symbol-keyed property never survived an
                // `Object.assign`, which is how a great deal of code copies an object.
                foreach (var key in source.OwnKeys())
                {
                    engine.Charge(1);

                    if (!ObjectOwnAt(source, key, out var property) || !property.Enumerable)
                    {
                        continue;
                    }

                    if (key.IsSymbol)
                    {
                        engine.SetSymbol(
                            receiver,
                            key.AsSymbol(),
                            engine.GetSymbol(reader, key.AsSymbol()),
                            strict: true);

                        continue;
                    }

                    engine.SetProperty(
                        receiver, key.AsString(), engine.GetProperty(reader, key.AsString()), strict: true);
                }
            }

            return receiver;
        });

        Method(constructor, "freeze", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var value = ArgOfObject(arguments, 0);

            if (value.IsObject)
            {
                ObjectSetIntegrity(engine, value.AsObject(), freeze: true);
            }

            return value;
        });

        Method(constructor, "seal", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var value = ArgOfObject(arguments, 0);

            if (value.IsObject)
            {
                ObjectSetIntegrity(engine, value.AsObject(), freeze: false);
            }

            return value;
        });

        Method(constructor, "isFrozen", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var value = ArgOfObject(arguments, 0);

            // A PRIMITIVE IS VACUOUSLY FROZEN. It has no own property that could be rewritten, and
            // answering `false` would make `Object.isFrozen(1)` disagree with `Object.freeze(1)`.
            return value.IsObject
                ? JsValue.Boolean(ObjectTestIntegrity(engine, value.AsObject(), freeze: true))
                : JsValue.True;
        });

        Method(constructor, "isSealed", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var value = ArgOfObject(arguments, 0);

            return value.IsObject
                ? JsValue.Boolean(ObjectTestIntegrity(engine, value.AsObject(), freeze: false))
                : JsValue.True;
        });

        Method(constructor, "preventExtensions", 1, static (engine, thisValue, arguments) =>
        {
            _ = engine;
            _ = thisValue;
            var value = ArgOfObject(arguments, 0);

            if (value.IsObject)
            {
                value.AsObject().Extensible = false;
            }

            return value;
        });

        Method(constructor, "isExtensible", 1, static (engine, thisValue, arguments) =>
        {
            _ = engine;
            _ = thisValue;
            var value = ArgOfObject(arguments, 0);
            return JsValue.Boolean(value.IsObject && value.AsObject().Extensible);
        });

        Method(constructor, "is", 2, static (engine, thisValue, arguments) =>
        {
            _ = engine;
            _ = thisValue;

            return JsValue.Boolean(
                ObjectSameValue(ArgOfObject(arguments, 0), ArgOfObject(arguments, 1)));
        });
    }

    /// <summary>Defines one half of an accessor, leaving the other half as it stands.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=595FB1
    // Broiler-Human:        PENDING
    private static void ObjectDefineAccessorHalf(
        JsEngine engine, JsObject host, JsValue key, JsValue accessor, bool getter)
    {
        var function = accessor.AsObject();

        if (key.IsSymbol)
        {
            host.TryGetOwnSymbol(key.AsSymbol(), out var held);

            host.SetOwnSymbol(
                key.AsSymbol(),
                JsProperty.Accessor(
                    getter ? function : held.Getter,
                    getter ? held.Setter : function,
                    JsPropertyAttributes.Enumerable | JsPropertyAttributes.Configurable));

            return;
        }

        var name = engine.ToPropertyKey(key);
        host.TryGetOwnProperty(name, out var existing);

        host.SetOwnProperty(
            name,
            JsProperty.Accessor(
                getter ? function : existing.Getter,
                getter ? existing.Setter : function,
                JsPropertyAttributes.Enumerable | JsPropertyAttributes.Configurable));
    }

    /// <summary>Finds one half of the accessor a read of <paramref name="key"/> would reach.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B0B2B3
    // Broiler-Human:        PENDING
    private static JsValue ObjectLookupAccessorHalf(
        JsEngine engine, JsValue receiver, JsValue key, bool getter)
    {
        var current = engine.ToObject(receiver);
        var symbol = key.IsSymbol ? key.AsSymbol() : null;
        var name = symbol is null ? engine.ToPropertyKey(key) : string.Empty;

        while (current is not null)
        {
            engine.Charge(1);

            var found = symbol is null
                ? current.TryGetOwnProperty(name, out var property)
                : current.TryGetOwnSymbol(symbol, out property);

            if (found)
            {
                if (!property.IsAccessor)
                {
                    return JsValue.Undefined;
                }

                var half = getter ? property.Getter : property.Setter;
                return half is null ? JsValue.Undefined : JsValue.Object(half);
            }

            current = current.Prototype;
        }

        return JsValue.Undefined;
    }

    /// <summary>Reads argument <paramref name="at"/>, which the caller may not have supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=939E17
    // Broiler-Human:        PENDING
    private static JsValue ArgOfObject(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>
    /// The specification's <c>SameValue</c>, which differs from <c>===</c> at NaN and at zero.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=2CD84F
    // Broiler-Human:        PENDING
    private static bool ObjectSameValue(JsValue left, JsValue right)
    {
        if (left.Type != JsType.Number || right.Type != JsType.Number)
        {
            return left.StrictlyEquals(right);
        }

        var first = left.AsNumber();
        var second = right.AsNumber();

        if (double.IsNaN(first) || double.IsNaN(second))
        {
            return double.IsNaN(first) && double.IsNaN(second);
        }

        // `+0` AND `-0` ARE DIFFERENT VALUES HERE and the same value under `===`, so the sign has
        // to be compared explicitly - `first == second` is true for the pair.
        return first == 0 && second == 0
            ? double.IsNegative(first) == double.IsNegative(second)
            : first == second;
    }

    /// <summary>Builds the Array <c>Object.keys</c>, <c>values</c> or <c>entries</c> answers with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=87AC04
    // Broiler-Human:        PENDING
    private JsValue ObjectEnumerate(JsEngine engine, JsValue value, ObjectEntryKind kind)
    {
        var target = engine.ToObject(value);
        var reader = JsValue.Object(target);
        var result = NewArray();

        foreach (var key in target.OwnPropertyNames())
        {
            engine.Charge(1);

            if (!target.TryGetOwnProperty(key, out var property) || !property.Enumerable)
            {
                continue;
            }

            if (kind == ObjectEntryKind.Key)
            {
                result.Push(JsValue.String(key));
                continue;
            }

            var element = engine.GetProperty(reader, key);

            if (kind == ObjectEntryKind.Value)
            {
                result.Push(element);
                continue;
            }

            var pair = NewArray();
            pair.Push(JsValue.String(key));
            pair.Push(element);
            result.Push(JsValue.Object(pair));
        }

        return JsValue.Object(result);
    }

    /// <summary>
    /// Builds the descriptor object <c>Object.getOwnPropertyDescriptor</c> answers with.
    /// </summary>
    /// <remarks>
    /// The own keys are exactly four and their attributes are the ordinary ones: a descriptor is a
    /// plain object a program may edit and hand back, not a view onto the property it describes.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=12738E
    // Broiler-Human:        PENDING
    private JsObject ObjectDescriptorFor(JsProperty property)
    {
        var result = new JsObject(ObjectPrototype);

        if (property.IsAccessor)
        {
            result.DefineOrdinary(
                "get", property.Getter is null ? JsValue.Undefined : JsValue.Object(property.Getter));

            result.DefineOrdinary(
                "set", property.Setter is null ? JsValue.Undefined : JsValue.Object(property.Setter));
        }
        else
        {
            result.DefineOrdinary("value", property.Value);
            result.DefineOrdinary("writable", JsValue.Boolean(property.Writable));
        }

        result.DefineOrdinary("enumerable", JsValue.Boolean(property.Enumerable));
        result.DefineOrdinary("configurable", JsValue.Boolean(property.Configurable));
        return result;
    }

    /// <summary>Reads a descriptor object into the fields it actually carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=93FF58
    // Broiler-Human:        PENDING
    private static ObjectDescriptorFields ObjectToDescriptorFields(JsEngine engine, JsValue value)
    {
        if (!value.IsObject)
        {
            throw engine.Error("TypeError", "Property description must be an object");
        }

        var source = value.AsObject();
        var fields = new ObjectDescriptorFields();

        if (engine.HasProperty(source, "enumerable"))
        {
            fields.HasEnumerable = true;
            fields.Enumerable = engine.GetProperty(value, "enumerable").ToBooleanValue();
        }

        if (engine.HasProperty(source, "configurable"))
        {
            fields.HasConfigurable = true;
            fields.Configurable = engine.GetProperty(value, "configurable").ToBooleanValue();
        }

        if (engine.HasProperty(source, "value"))
        {
            fields.HasValue = true;
            fields.Value = engine.GetProperty(value, "value");
        }

        if (engine.HasProperty(source, "writable"))
        {
            fields.HasWritable = true;
            fields.Writable = engine.GetProperty(value, "writable").ToBooleanValue();
        }

        if (engine.HasProperty(source, "get"))
        {
            var getter = engine.GetProperty(value, "get");

            if (getter.Type != JsType.Undefined && !(getter.IsObject && getter.AsObject().IsCallable))
            {
                throw engine.Error("TypeError", "Getter must be a function");
            }

            fields.HasGet = true;
            fields.Getter = getter.AsObjectOrNull();
        }

        if (engine.HasProperty(source, "set"))
        {
            var setter = engine.GetProperty(value, "set");

            if (setter.Type != JsType.Undefined && !(setter.IsObject && setter.AsObject().IsCallable))
            {
                throw engine.Error("TypeError", "Setter must be a function");
            }

            fields.HasSet = true;
            fields.Setter = setter.AsObjectOrNull();
        }

        if ((fields.HasGet || fields.HasSet) && (fields.HasValue || fields.HasWritable))
        {
            throw engine.Error(
                "TypeError",
                "Invalid property descriptor. Cannot both specify accessors and a value or writable attribute");
        }

        return fields;
    }

    /// <summary>The property a descriptor's fields describe, with nothing to redefine.</summary>
    /// <remarks>
    /// The Symbol-keyed path uses it: there is no current descriptor to validate against, so the
    /// fields are read straight into a property. Absent attribute fields default to false, which is
    /// what <c>DefinePropertyOrThrow</c> says for a property that did not exist.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BFF4D5
    // Broiler-Human:        PENDING
    private static JsProperty ObjectPropertyFromFields(
        JsEngine engine, JsObject target, ObjectDescriptorFields fields)
    {
        _ = engine;
        _ = target;
        var attributes = JsPropertyAttributes.None;

        if (fields.HasEnumerable && fields.Enumerable)
        {
            attributes |= JsPropertyAttributes.Enumerable;
        }

        if (fields.HasConfigurable && fields.Configurable)
        {
            attributes |= JsPropertyAttributes.Configurable;
        }

        if (fields.HasGet || fields.HasSet)
        {
            return JsProperty.Accessor(fields.Getter, fields.Setter, attributes);
        }

        if (fields.HasWritable && fields.Writable)
        {
            attributes |= JsPropertyAttributes.Writable;
        }

        return JsProperty.Data(fields.HasValue ? fields.Value : JsValue.Undefined, attributes);
    }

    /// <summary>
    /// The specification's <c>ValidateAndApplyPropertyDescriptor</c>: refuse what the existing
    /// property forbids, then write the merge of the two.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7CC38E
    // Broiler-Human:        PENDING
    private static void ObjectApplyDescriptor(
        JsEngine engine, JsObject target, string key, ObjectDescriptorFields fields)
    {
        // A PROXY DEFINES THROUGH ITS TRAP AND NONE OF THIS FUNCTION APPLIES TO IT. Everything
        // below is `ValidateAndApplyPropertyDescriptor`, which is what an ORDINARY object's
        // `[[DefineOwnProperty]]` is; an exotic object may define its own, and this one does. The
        // validation would also read the proxy's current descriptor and extensibility through two
        // traps the language never says to call here, before reaching the trap that decides.
        if (target is JsProxy proxy)
        {
            ObjectDefinedOrThrow(engine, proxy, JsValue.String(key), fields);
            return;
        }

        var wantsAccessor = fields.HasGet || fields.HasSet;
        var wantsData = fields.HasValue || fields.HasWritable;

        // AN ARRAY'S `length` IS CHECKED AND COERCED BEFORE IT IS DEFINED, exactly as an assignment
        // to it is: a definition is the other way a program reaches it, and the two may not
        // disagree about which values are lengths.
        if (target is JsArray sized && fields.HasValue &&
            string.Equals(key, "length", System.StringComparison.Ordinal))
        {
            var wanted = engine.ArrayLengthOrRefuse(fields.Value);
            fields.Value = JsValue.Number(wanted);
            ObjectApplyLength(engine, sized, wanted, fields);
            return;
        }

        if (!target.TryGetOwnProperty(key, out var current))
        {
            if (!target.Extensible)
            {
                throw engine.Error(
                    "TypeError", "Cannot define property " + key + ", object is not extensible");
            }

            var fresh = JsPropertyAttributes.None;

            if (fields.HasEnumerable && fields.Enumerable)
            {
                fresh |= JsPropertyAttributes.Enumerable;
            }

            if (fields.HasConfigurable && fields.Configurable)
            {
                fresh |= JsPropertyAttributes.Configurable;
            }

            if (wantsAccessor)
            {
                ObjectWriteOwn(
                    engine, target, key, JsProperty.Accessor(fields.Getter, fields.Setter, fresh));

                return;
            }

            if (fields.HasWritable && fields.Writable)
            {
                fresh |= JsPropertyAttributes.Writable;
            }

            ObjectWriteOwn(
                engine,
                target,
                key,
                JsProperty.Data(fields.HasValue ? fields.Value : JsValue.Undefined, fresh));

            return;
        }

        if (!fields.HasEnumerable && !fields.HasConfigurable && !wantsAccessor && !wantsData)
        {
            return;
        }

        // A NON-CONFIGURABLE PROPERTY IS THE WHOLE POINT OF THE VALIDATION. Everything below is a
        // refusal the specification names; a define that skipped them would make `freeze` a
        // suggestion rather than a guarantee.
        if (!current.Configurable)
        {
            if (fields.HasConfigurable && fields.Configurable)
            {
                throw engine.Error("TypeError", "Cannot redefine property: " + key);
            }

            if (fields.HasEnumerable && fields.Enumerable != current.Enumerable)
            {
                throw engine.Error("TypeError", "Cannot redefine property: " + key);
            }

            if ((wantsAccessor && !current.IsAccessor) || (wantsData && current.IsAccessor))
            {
                throw engine.Error("TypeError", "Cannot redefine property: " + key);
            }

            if (current.IsAccessor)
            {
                if (fields.HasGet && !ReferenceEquals(fields.Getter, current.Getter))
                {
                    throw engine.Error("TypeError", "Cannot redefine property: " + key);
                }

                if (fields.HasSet && !ReferenceEquals(fields.Setter, current.Setter))
                {
                    throw engine.Error("TypeError", "Cannot redefine property: " + key);
                }
            }
            else if (!current.Writable)
            {
                if (fields.HasWritable && fields.Writable)
                {
                    throw engine.Error("TypeError", "Cannot redefine property: " + key);
                }

                if (fields.HasValue && !ObjectSameValue(fields.Value, current.Value))
                {
                    throw engine.Error("TypeError", "Cannot redefine property: " + key);
                }
            }
        }

        var attributes = current.Attributes;

        if (fields.HasEnumerable)
        {
            attributes = fields.Enumerable
                ? attributes | JsPropertyAttributes.Enumerable
                : attributes & ~JsPropertyAttributes.Enumerable;
        }

        if (fields.HasConfigurable)
        {
            attributes = fields.Configurable
                ? attributes | JsPropertyAttributes.Configurable
                : attributes & ~JsPropertyAttributes.Configurable;
        }

        if (wantsAccessor)
        {
            var getter = fields.HasGet ? fields.Getter : current.IsAccessor ? current.Getter : null;
            var setter = fields.HasSet ? fields.Setter : current.IsAccessor ? current.Setter : null;

            ObjectWriteOwn(
                engine,
                target,
                key,
                JsProperty.Accessor(
                    getter,
                    setter,
                    attributes & ~(JsPropertyAttributes.Writable | JsPropertyAttributes.Accessor)));

            return;
        }

        if (wantsData || !current.IsAccessor)
        {
            var value = fields.HasValue
                ? fields.Value
                : current.IsAccessor ? JsValue.Undefined : current.Value;

            var writable = fields.HasWritable
                ? fields.Writable
                : !current.IsAccessor && current.Writable;

            attributes &= ~(JsPropertyAttributes.Accessor | JsPropertyAttributes.Writable);

            if (writable)
            {
                attributes |= JsPropertyAttributes.Writable;
            }

            ObjectWriteOwn(engine, target, key, JsProperty.Data(value, attributes));
            return;
        }

        // An accessor whose only change was `enumerable` or `configurable`.
        ObjectWriteOwn(
            engine,
            target,
            key,
            JsProperty.Accessor(
                current.Getter, current.Setter, attributes & ~JsPropertyAttributes.Accessor));
    }

    /// <summary>Writes a validated descriptor, refusing the one shape the object model cannot hold.</summary>
    /// <remarks>
    /// <see cref="JsArray"/> stores a dense element as a bare value, so the only descriptor such a
    /// slot expresses is the ordinary one. Handed anything else it vacates the slot and writes the
    /// ordinary map instead - but a vacated slot inside the dense range shadows that map, so the
    /// property would become unreachable rather than merely un-writable. Refusing is the honest
    /// answer: a define that went through would silently delete the element it was redefining.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9379A1
    // Broiler-Human:        PENDING
    private static void ObjectWriteOwn(
        JsEngine engine, JsObject target, string key, JsProperty property)
    {
        _ = engine;
        target.SetOwnProperty(key, property);
    }

    /// <summary>
    /// Reads every own enumerable descriptor off <paramref name="properties"/> and applies them.
    /// </summary>
    /// <remarks>
    /// Both passes are the specification's: every descriptor is read and validated before any of
    /// them is written, so a malformed later descriptor leaves none of the earlier ones applied.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=58216D
    // Broiler-Human:        PENDING
    private static void ObjectDefineFromProperties(
        JsEngine engine, JsObject target, JsValue properties)
    {
        var source = engine.ToObject(properties);
        var reader = JsValue.Object(source);
        var keys = new System.Collections.Generic.List<JsValue>();
        var descriptors = new System.Collections.Generic.List<ObjectDescriptorFields>();

        // BOTH KEY TABLES. `Object.create(p, { [Symbol()]: … })` and `Object.defineProperties` with
        // a Symbol-keyed descriptor both reach here, and walking the String table alone dropped
        // those definitions silently - the call returned the object, having defined nothing.
        foreach (var key in source.OwnKeys())
        {
            engine.Charge(1);

            if (!ObjectOwnAt(source, key, out var property) || !property.Enumerable)
            {
                continue;
            }

            keys.Add(key);

            descriptors.Add(
                ObjectToDescriptorFields(
                    engine,
                    key.IsSymbol
                        ? engine.GetSymbol(reader, key.AsSymbol())
                        : engine.GetProperty(reader, key.AsString())));
        }

        for (var at = 0; at < keys.Count; at++)
        {
            engine.Charge(1);
            var key = keys[at];

            if (target is JsProxy proxy)
            {
                ObjectDefinedOrThrow(engine, proxy, key, descriptors[at]);
                continue;
            }

            if (key.IsSymbol)
            {
                target.SetOwnSymbol(
                    key.AsSymbol(), ObjectPropertyFromFields(engine, target, descriptors[at]));

                continue;
            }

            ObjectApplyDescriptor(engine, target, key.AsString(), descriptors[at]);
        }
    }

    /// <summary>Reads one own property under a key of either kind.</summary>
    /// <remarks>
    /// <b>The two key tables are a storage decision and <c>[[OwnPropertyKeys]]</c> is one
    /// operation</b>, so every static that walks an object completely needs to ask about whichever
    /// kind of key came back. Writing the branch at each of those sites is how five of them came to
    /// walk the String table only.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=82FDAC
    // Broiler-Human:        PENDING
    private static bool ObjectOwnAt(JsObject target, JsValue key, out JsProperty property) =>
        key.IsSymbol
            ? target.TryGetOwnSymbol(key.AsSymbol(), out property)
            : target.TryGetOwnProperty(key.AsString(), out property);

    /// <summary>Defines one own property under a key of either kind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8EF211
    // Broiler-Human:        PENDING
    private static void ObjectSetOwnAt(JsObject target, JsValue key, JsProperty property)
    {
        if (key.IsSymbol)
        {
            target.SetOwnSymbol(key.AsSymbol(), property);
            return;
        }

        target.SetOwnProperty(key.AsString(), property);
    }

    /// <summary>
    /// <c>OrdinaryDefineOwnProperty</c>, answering the boolean rather than throwing the refusal.
    /// </summary>
    /// <remarks>
    /// <b>It exists because a missing Proxy trap forwards the INTERNAL METHOD.</b> A proxy with no
    /// <c>defineProperty</c> trap must do to its target exactly what <c>Reflect.defineProperty</c>
    /// would — validate, and answer whether it took — and an unchecked write instead of this is how
    /// a proxy over a frozen object would have let a redefinition through.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BE8B4A
    // Broiler-Human:        PENDING
    internal static bool ObjectDefineOrdinary(
        JsEngine engine, JsObject target, JsValue key, ObjectDescriptorFields fields)
    {
        try
        {
            if (key.IsSymbol)
            {
                target.SetOwnSymbol(key.AsSymbol(), ObjectPropertyFromFields(engine, target, fields));
                return true;
            }

            ObjectApplyDescriptor(engine, target, key.AsString(), fields);
            return true;
        }
        catch (JsThrow)
        {
            return false;
        }
    }

    /// <summary><c>OrdinaryPreventExtensions</c>, which cannot fail and says so.</summary>
    /// <remarks>
    /// It is a function rather than an assignment only because <c>Reflect.preventExtensions</c> has
    /// to answer the boolean that an exotic <c>[[PreventExtensions]]</c> may make <c>false</c>, and
    /// the two branches read better as one expression than as an assignment and a constant.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5C5577
    // Broiler-Human:        PENDING
    internal static bool ObjectPreventExtensionsOrdinary(JsObject target)
    {
        target.Extensible = false;
        return true;
    }

    /// <summary>
    /// <c>OrdinarySetPrototypeOf</c>, likewise as a boolean, and likewise for two callers.
    /// </summary>
    /// <remarks>
    /// <b>Refusing a change and refusing a NO-OP are different, and the order below is the
    /// specification's.</b> Setting the prototype an object already has succeeds even where the
    /// object is sealed, because <c>[[SetPrototypeOf]]</c> asks whether the answer would change and
    /// a non-extensible object refuses only a change.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=638EFE
    // Broiler-Human:        PENDING
    internal static bool ObjectSetPrototypeOrdinary(
        JsEngine engine, JsObject target, JsObject? prototype)
    {
        if (ReferenceEquals(target.Prototype, prototype))
        {
            return true;
        }

        if (!target.Extensible)
        {
            return false;
        }

        for (var walk = prototype; walk is not null; walk = walk.Prototype)
        {
            engine.Charge(1);

            if (ReferenceEquals(walk, target))
            {
                return false;
            }

            // THE WALK STOPS AT A PROXY. Reading the next link would run a `getPrototypeOf` trap
            // the specification does not ask for at this point - guest code, in the middle of an
            // operation that has not yet decided whether it will happen - and the specification
            // ends the cycle check at the first object whose `[[GetPrototypeOf]]` is not the
            // ordinary one. A cycle through a proxy is therefore undetected here, which is the
            // language's own answer: a trap may invent a chain with no fixed shape to be cyclic
            // in, and the lookups that walk it are bounded by the fuel meter instead.
            if (walk is JsProxy)
            {
                break;
            }
        }

        target.Prototype = prototype;
        return true;
    }

    /// <summary>
    /// <c>DefinePropertyOrThrow</c> against a Proxy: the trap's refusal, as the <c>TypeError</c> the
    /// <c>Object</c> statics owe.
    /// </summary>
    /// <remarks>
    /// The pairing is the one this whole file is built around — <c>Object.defineProperty</c> throws
    /// where <c>Reflect.defineProperty</c> answers <c>false</c> — so the boolean is read HERE and
    /// not inside the proxy, which answers in the specification's own currency and lets each caller
    /// spend it the way it must.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DC325B
    // Broiler-Human:        PENDING
    private static void ObjectDefinedOrThrow(
        JsEngine engine, JsProxy proxy, JsValue key, ObjectDescriptorFields fields)
    {
        if (!proxy.ProxyDefineOwnProperty(key, fields))
        {
            throw engine.Error(
                "TypeError", "the 'defineProperty' trap refused the definition");
        }
    }

    /// <summary>The specification's <c>OrdinarySetPrototypeOf</c>, cycle check included.</summary>
    /// <remarks>
    /// The cycle check is not politeness. Every property lookup walks the chain with a plain loop,
    /// so a chain that closed on itself would be an unkillable spin rather than a wrong answer.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=91BA7B
    // Broiler-Human:        PENDING
    private static void ObjectSetPrototype(JsEngine engine, JsObject target, JsObject? prototype)
    {
        // A PROXY IS ASKED THROUGH ITS TRAP AND THROWS WHERE IT REFUSES, which is what
        // `Object.setPrototypeOf` owes and what `Reflect.setPrototypeOf` - which does not come this
        // way - must not do. None of the three ordinary tests below is an exotic object's to obey.
        if (target is JsProxy proxy)
        {
            if (!proxy.ProxySetPrototypeOf(prototype))
            {
                throw engine.Error("TypeError", "the 'setPrototypeOf' trap refused the prototype");
            }

            return;
        }

        // THE TWO REFUSALS ARE ONE ANSWER HERE AND TWO MESSAGES, which is the only reason this is
        // not simply a call. `Object.setPrototypeOf` throws for both a closed object and a cycle
        // and the language names each; the ordinary internal method answers one boolean, so the
        // reason is recovered by asking which of the two it was.
        if (ObjectSetPrototypeOrdinary(engine, target, prototype))
        {
            return;
        }

        throw engine.Error(
            "TypeError",
            target.Extensible ? "Cyclic __proto__ value" : "#<Object> is not extensible");
    }

    /// <summary>Applies <c>Object.freeze</c> or <c>Object.seal</c> to every own property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AC3A6B
    // Broiler-Human:        PENDING
    private static void ObjectSetIntegrity(JsEngine engine, JsObject target, bool freeze)
    {
        target.Extensible = false;

        // IT WALKS BOTH KEY TABLES, and it walked only the String one until 2026-09-05. A
        // Symbol-keyed property survived `Object.freeze` writable and configurable, and
        // `Object.isFrozen` agreed the object was frozen because it asked the same half-question -
        // so a class keeping state under a Symbol was never actually frozen by either.
        foreach (var key in target.OwnKeys())
        {
            engine.Charge(1);

            if (!ObjectOwnAt(target, key, out var property))
            {
                continue;
            }

            var attributes = property.Attributes & ~JsPropertyAttributes.Configurable;

            if (freeze && !property.IsAccessor)
            {
                attributes &= ~JsPropertyAttributes.Writable;
            }

            if (attributes == property.Attributes)
            {
                continue;
            }

            property.Attributes = attributes;
            ObjectSetOwnAt(target, key, property);
        }
    }

    /// <summary>Answers <c>Object.isFrozen</c> or <c>Object.isSealed</c> for an object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=526896
    // Broiler-Human:        PENDING
    private static bool ObjectTestIntegrity(JsEngine engine, JsObject target, bool freeze)
    {
        if (target.Extensible)
        {
            return false;
        }

        // BOTH KEY TABLES, for the reason `ObjectSetIntegrity` gives: the two predicates have to
        // ask the same question or one of them reports an integrity the other never applied.
        foreach (var key in target.OwnKeys())
        {
            engine.Charge(1);

            if (!ObjectOwnAt(target, key, out var property))
            {
                continue;
            }

            if (property.Configurable)
            {
                return false;
            }

            if (freeze && !property.IsAccessor && property.Writable)
            {
                return false;
            }
        }

        return true;
    }

    // TWO PREDICATES USED TO STAND HERE AND BOTH ARE GONE. `ObjectShadowsDenseSlot` had no caller
    // at all; `ObjectIsUnattributable` had two, and its whole body was `return false` - a question
    // whose answer had become "nothing" when an earlier correction let an Array's `length` carry
    // attributes after all, leaving a named predicate that read as though it still excluded
    // something. Removing them is the second half of that correction rather than a new one.

    /// <summary>Applies a definition of an Array's <c>length</c>, which may only partly succeed.</summary>
    /// <remarks>
    /// <b>A shortening that meets a non-configurable element stops there</b>, and the definition has
    /// then failed even though it did something: the length is left where the walk stopped and the
    /// <c>TypeError</c> says the definition did not take. Reporting success would tell a program the
    /// length is what it asked for when it is not.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=794CB0
    // Broiler-Human:        PENDING
    private static void ObjectApplyLength(
        JsEngine engine, JsArray target, uint wanted, ObjectDescriptorFields fields)
    {
        if (fields.HasEnumerable && fields.Enumerable)
        {
            throw engine.Error("TypeError", "Cannot redefine property: length");
        }

        if (fields.HasConfigurable && fields.Configurable)
        {
            throw engine.Error("TypeError", "Cannot redefine property: length");
        }

        if (!target.LengthWritable && wanted != target.Length)
        {
            throw engine.Error("TypeError", "Cannot assign to read only property 'length'");
        }

        var applied = target.TrySetLength(wanted);

        if (fields.HasWritable && !fields.Writable)
        {
            target.TryGetOwnProperty("length", out var held);
            held.Attributes &= ~JsPropertyAttributes.Writable;
            target.SetOwnProperty("length", held);
        }

        if (!applied)
        {
            throw engine.Error("TypeError", "Cannot redefine property: length");
        }
    }

    /// <summary>Which fields a descriptor object actually carried, and what they said.</summary>
    /// <remarks>
    /// The presence flags are what separates <c>{ value: undefined }</c> from <c>{}</c>. A
    /// representation that only held the six values could not tell them apart, and the difference
    /// decides whether a redefinition is a change or a no-op.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9BF883
    // Broiler-Human:        PENDING
    internal sealed class ObjectDescriptorFields
    {
        /// <summary>Whether the descriptor carried <c>enumerable</c>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8D9CF0
        // Broiler-Human:        PENDING
        internal bool HasEnumerable { get; set; }

        /// <summary>What <c>enumerable</c> said.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=75BAB2
        // Broiler-Human:        PENDING
        internal bool Enumerable { get; set; }

        /// <summary>Whether the descriptor carried <c>configurable</c>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BA10F9
        // Broiler-Human:        PENDING
        internal bool HasConfigurable { get; set; }

        /// <summary>What <c>configurable</c> said.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6C03F0
        // Broiler-Human:        PENDING
        internal bool Configurable { get; set; }

        /// <summary>Whether the descriptor carried <c>writable</c>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A75256
        // Broiler-Human:        PENDING
        internal bool HasWritable { get; set; }

        /// <summary>What <c>writable</c> said.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=ACB784
        // Broiler-Human:        PENDING
        internal bool Writable { get; set; }

        /// <summary>Whether the descriptor carried <c>value</c>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=993122
        // Broiler-Human:        PENDING
        internal bool HasValue { get; set; }

        /// <summary>What <c>value</c> held.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8DB3A8
        // Broiler-Human:        PENDING
        internal JsValue Value { get; set; } = JsValue.Undefined;

        /// <summary>Whether the descriptor carried <c>get</c>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D87D0F
        // Broiler-Human:        PENDING
        internal bool HasGet { get; set; }

        /// <summary>The getter, or <see langword="null"/> when <c>get</c> was <c>undefined</c>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=850836
        // Broiler-Human:        PENDING
        internal JsObject? Getter { get; set; }

        /// <summary>Whether the descriptor carried <c>set</c>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B8059B
        // Broiler-Human:        PENDING
        internal bool HasSet { get; set; }

        /// <summary>The setter, or <see langword="null"/> when <c>set</c> was <c>undefined</c>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B8AA19
        // Broiler-Human:        PENDING
        internal JsObject? Setter { get; set; }
    }
}
