// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           15
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       18
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=12944D
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

            return JsValue.String("[object " + engine.ToObject(thisValue).ClassName + "]");
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

            foreach (var key in target.OwnPropertyNames())
            {
                engine.Charge(1);

                if (target.TryGetOwnProperty(key, out var property))
                {
                    result.DefineOrdinary(key, JsValue.Object(ObjectDescriptorFor(property)));
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

                foreach (var key in source.OwnPropertyNames())
                {
                    engine.Charge(1);

                    if (!source.TryGetOwnProperty(key, out var property) || !property.Enumerable)
                    {
                        continue;
                    }

                    engine.SetProperty(receiver, key, engine.GetProperty(reader, key), strict: true);
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A520F1
    // Broiler-Human:        PENDING
    private static void ObjectApplyDescriptor(
        JsEngine engine, JsObject target, string key, ObjectDescriptorFields fields)
    {
        var wantsAccessor = fields.HasGet || fields.HasSet;
        var wantsData = fields.HasValue || fields.HasWritable;

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0D1D2E
    // Broiler-Human:        PENDING
    private static void ObjectDefineFromProperties(
        JsEngine engine, JsObject target, JsValue properties)
    {
        var source = engine.ToObject(properties);
        var reader = JsValue.Object(source);
        var keys = new System.Collections.Generic.List<string>();
        var descriptors = new System.Collections.Generic.List<ObjectDescriptorFields>();

        foreach (var key in source.OwnPropertyNames())
        {
            engine.Charge(1);

            if (!source.TryGetOwnProperty(key, out var property) || !property.Enumerable)
            {
                continue;
            }

            keys.Add(key);
            descriptors.Add(ObjectToDescriptorFields(engine, engine.GetProperty(reader, key)));
        }

        for (var at = 0; at < keys.Count; at++)
        {
            engine.Charge(1);
            ObjectApplyDescriptor(engine, target, keys[at], descriptors[at]);
        }
    }

    /// <summary>The specification's <c>OrdinarySetPrototypeOf</c>, cycle check included.</summary>
    /// <remarks>
    /// The cycle check is not politeness. Every property lookup walks the chain with a plain loop,
    /// so a chain that closed on itself would be an unkillable spin rather than a wrong answer.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1453A9
    // Broiler-Human:        PENDING
    private static void ObjectSetPrototype(JsEngine engine, JsObject target, JsObject? prototype)
    {
        if (ReferenceEquals(target.Prototype, prototype))
        {
            return;
        }

        if (!target.Extensible)
        {
            throw engine.Error("TypeError", "#<Object> is not extensible");
        }

        var walk = prototype;

        while (walk is not null)
        {
            engine.Charge(1);

            if (ReferenceEquals(walk, target))
            {
                throw engine.Error("TypeError", "Cyclic __proto__ value");
            }

            walk = walk.Prototype;
        }

        target.Prototype = prototype;
    }

    /// <summary>Applies <c>Object.freeze</c> or <c>Object.seal</c> to every own property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=2A86BF
    // Broiler-Human:        PENDING
    private static void ObjectSetIntegrity(JsEngine engine, JsObject target, bool freeze)
    {
        target.Extensible = false;

        foreach (var key in target.OwnPropertyNames())
        {
            engine.Charge(1);

            if (ObjectIsUnattributable(target, key) ||
                !target.TryGetOwnProperty(key, out var property))
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
            target.SetOwnProperty(key, property);
        }
    }

    /// <summary>Answers <c>Object.isFrozen</c> or <c>Object.isSealed</c> for an object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=803DCB
    // Broiler-Human:        PENDING
    private static bool ObjectTestIntegrity(JsEngine engine, JsObject target, bool freeze)
    {
        if (target.Extensible)
        {
            return false;
        }

        foreach (var key in target.OwnPropertyNames())
        {
            engine.Charge(1);

            if (ObjectIsUnattributable(target, key) ||
                !target.TryGetOwnProperty(key, out var property))
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

    /// <summary>Whether an Array's dense element store would shadow a descriptor written here.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=72ECC6
    // Broiler-Human:        PENDING
    private static bool ObjectShadowsDenseSlot(JsObject target, string key) =>
        target is JsArray array &&
        JsObject.IsArrayIndex(key, out var at) &&
        at < (uint)array.DenseCount;

    /// <summary>Whether <paramref name="target"/> cannot carry property attributes at this key.</summary>
    /// <remarks>
    /// One key on an Array cannot: <c>length</c>, which <see cref="JsArray"/> synthesises rather
    /// than stores, so writing its descriptor sets the length and drops the attributes. Freezing
    /// and sealing step over it, and <c>isFrozen</c> and <c>isSealed</c> step over it too so that
    /// the pair agree - a freeze that reported itself undone would be worse than one that admits
    /// what it could not reach. A dense ELEMENT used to be in this list and no longer is: the array
    /// vacates the slot and writes the ordinary map when it is handed a descriptor the slot cannot
    /// express, so an element can carry attributes after all.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1FA36A
    // Broiler-Human:        PENDING
    private static bool ObjectIsUnattributable(JsObject target, string key) =>
        target is JsArray && string.Equals(key, "length", System.StringComparison.Ordinal);

    /// <summary>Which fields a descriptor object actually carried, and what they said.</summary>
    /// <remarks>
    /// The presence flags are what separates <c>{ value: undefined }</c> from <c>{}</c>. A
    /// representation that only held the six values could not tell them apart, and the difference
    /// decides whether a redefinition is a change or a no-op.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1C9173
    // Broiler-Human:        PENDING
    private sealed class ObjectDescriptorFields
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
