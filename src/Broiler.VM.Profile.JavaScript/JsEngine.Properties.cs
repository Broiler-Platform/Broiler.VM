// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   22
// Annotated:        22/22
// Exempt:           0
// Human-reviewed:   0/22
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  5/10 max
// Unverified:       22
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using Broiler.ECMAScript.Numbers;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    // ---- properties ----------------------------------------------------------------------------

    /// <summary>The prototype a primitive's property lookup starts from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=4659EB
    // Broiler-Human:        PENDING
    private JsObject? PrototypeFor(JsValue value) => value.Type switch
    {
        JsType.String => Realm.StringPrototype,
        JsType.Number => Realm.NumberPrototype,
        JsType.Boolean => Realm.BooleanPrototype,
        JsType.Symbol => Realm.SymbolPrototype,

        JsType.BigInt => BigIntPrototypeOrRefuse(),
        _ => null,
    };

    /// <summary>
    /// <c>BigInt.prototype</c>, which a realm has whenever a BigInt can reach it.
    /// </summary>
    /// <remarks>
    /// <b>A composition that declines the BigInt surface has no BigInt values</b>: every artifact
    /// holding a BigInt constant or naming the <c>BigInt</c> global is refused at its verification,
    /// and no other source makes one. So this refusal is unreachable by a program; it exists so that
    /// a value that arrived some way nobody foresaw is refused by name rather than read as an object
    /// with no chain.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=31B5DF
    // Broiler-Human:        PENDING
    private JsObject BigIntPrototypeOrRefuse() => Realm.BigIntPrototype ?? throw Error("TypeError", "this composition declined the BigInt surface, so BigInt.prototype does not exist");

    /// <summary>Reads a property off any value, walking the prototype chain.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B38348
    // Broiler-Human:        PENDING
    internal JsValue GetProperty(JsValue baseValue, string key)
    {
        if (baseValue.IsNullish)
        {
            return ThrowTypeError("Cannot read properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") + " (reading '" + key + "')");
        }

        if (baseValue.IsString)
        {
            var text = baseValue.AsString();

            if (string.Equals(key, "length", StringComparison.Ordinal))
            {
                return JsValue.Number(text.Length);
            }

            if (JsObject.IsArrayIndex(key, out var at))
            {
                return at < text.Length
                    ? JsValue.String(text[(int)at].ToString())
                    : JsValue.Undefined;
            }
        }

        var start = baseValue.IsObject ? baseValue.AsObject() : PrototypeFor(baseValue);

        // AN INTEGER-INDEXED EXOTIC OBJECT DOES NOT INHERIT ITS INDICES, and that is a property of
        // [[Get]] rather than of [[GetOwnProperty]] — so it cannot be expressed by an override on
        // the object and has to be expressed here. Without it a realm in which somebody wrote
        // `Object.prototype[9] = 42` would answer 42 for `new Int32Array(3)[9]`, where the language
        // says `undefined`: the index is out of the view, and out of the view is the end of the
        // search rather than the start of a walk.
        //
        // The same holds for a numeric key that is not an index - "-0", "1.5", "-1": it names no
        // element, so the answer is `undefined` without a walk (JsTypedArray.IsNumericKey).
        if (start is JsTypedArray view && JsTypedArray.IsNumericKey(key))
        {
            return view.TryGetOwnProperty(key, out var element) ? element.Value : JsValue.Undefined;
        }

        return start is null ? JsValue.Undefined : Lookup(start, key, baseValue);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=170ECC
    // Broiler-Human:        PENDING
    private JsValue Lookup(JsObject start, string key, JsValue receiver)
    {
        var current = start;

        while (current is not null)
        {
            // A PROXY SWALLOWS THE REST OF THE WALK RATHER THAN ANSWERING ONE LINK OF IT. `[[Get]]`
            // on a proxy is a whole operation - the `get` trap decides everything, including
            // whether a prototype is consulted at all - so continuing the loop past it would run
            // the trap for the own property and then walk the TARGET's chain behind its back. The
            // test is inside the loop because a proxy is just as likely to be somebody's
            // prototype as it is to be the object a program named.
            if (current is JsProxy proxy)
            {
                return proxy.ProxyGet(JsValue.String(key), receiver);
            }

            // A TYPED ARRAY PART-WAY UP THE CHAIN ENDS THE WALK FOR A NUMERIC KEY, for the reason
            // it does at the start: its [[Get]] answers the element or `undefined` and never asks
            // its own prototype.
            if (current is JsTypedArray passedView && JsTypedArray.IsNumericKey(key))
            {
                return passedView.TryGetOwnProperty(key, out var element)
                    ? element.Value
                    : JsValue.Undefined;
            }

            if (current.TryGetOwnProperty(key, out var property))
            {
                if (!property.IsAccessor)
                {
                    return property.Value;
                }

                return property.Getter is null
                    ? JsValue.Undefined
                    : Call(JsValue.Object(property.Getter), receiver, []);
            }

            current = current.Prototype;
        }

        return JsValue.Undefined;
    }

    /// <summary>Writes a property on any value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F25DC7
    // Broiler-Human:        PENDING
    internal void SetProperty(JsValue baseValue, string key, JsValue value, bool strict)
    {
        if (baseValue.IsNullish)
        {
            ThrowTypeError("Cannot set properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") + " (setting '" + key + "')");
            return;
        }

        var current = baseValue.IsObject ? baseValue.AsObject() : PrototypeFor(baseValue);
        var target = baseValue.AsObjectOrNull();

        // AN ARRAY'S `length` IS THE ONE PROPERTY WHOSE VALUE IS CHECKED BEFORE IT IS STORED, and
        // the check has to be here because it can THROW: `a.length = -1` is a RangeError in every
        // engine, and the object model has no engine to raise one with. What reaches the object is
        // the coerced number, so `a.length = "2"` sets two rather than nothing.
        // A `length` that is already closed refuses the write before anything is converted:
        // `OrdinarySet` stops at the non-writable own property and never reaches `ArraySetLength`,
        // so a `valueOf` on the assigned value does not run.
        if (target is JsArray sized && string.Equals(key, "length", StringComparison.Ordinal))
        {
            if (!sized.LengthWritable)
            {
                if (strict)
                {
                    ThrowTypeError("Cannot assign to read only property 'length' of object '[object Array]'");
                }

                return;
            }

            value = JsValue.Number(ArrayLengthOrRefuse(value));
        }

        // A WRITE PAST A CLOSED LENGTH IS A REFUSAL, and in strict code a refusal is a TypeError.
        // The object model drops the write silently because it cannot know the mode, so the mode's
        // half of the answer belongs here.
        if (target is JsArray fixedLength && !fixedLength.LengthWritable && JsObject.IsArrayIndex(key, out var past) && past >= fixedLength.Length)
        {
            if (strict)
            {
                ThrowTypeError("Cannot add property " + key + ", object is not extensible");
            }

            return;
        }

        // THE ELEMENT CONVERSION IS THE ENGINE'S BECAUSE IT CAN RUN `valueOf`. The object model
        // stores an element without an engine to hand, so it can convert a primitive exactly and
        // nothing else; the language says a write to an integer-indexed element is `ToNumber` of
        // whatever was assigned, and `ToNumber` of an object is a call. Doing it here is also what
        // makes the write silently discarded when the index is out of the view or the buffer is
        // detached — after the conversion has happened, which is the order the specification asks
        // for and is observable through a `valueOf` with a side effect.
        if (target is JsTypedArray view && JsTypedArray.IsNumericKey(key))
        {
            // A numeric key that is not an index is converted and then discarded the same way, and
            // it never walks the chain: no inherited setter may see it. The conversion is the one
            // the view's content type names: ToBigInt for a BigInt kind (JSeal B07).
            var element = ToElementValue(view.Kind, value);

            if (JsObject.IsArrayIndex(key, out var at))
            {
                _ = view.TryWriteAt((int)at, element);
            }

            return;
        }

        // A NAMESPACE REFUSES EVERY WRITE, and in strict code a refused write is a `TypeError`.
        // The walk below would find an export's own property, see it is writable - which it is,
        // and which a program can read off the descriptor - and let the assignment land in a copy
        // that no longer tracks the module's binding. Module code is always strict, so this is a
        // throw wherever an import is in scope; the sloppy branch is here for a namespace that
        // reached a script through a host.
        if (target is JsModuleNamespace)
        {
            if (strict)
            {
                ThrowTypeError("Cannot assign to '" + key + "' of a module namespace object");
            }

            return;
        }

        while (current is not null)
        {
            // A PROXY ANSWERS `[[Set]]` WHOLE, and the refusal it reports is turned into the
            // language's own two answers here: `false` is silent in sloppy code and a TypeError in
            // strict code, which is the rule every other refusal on this path already follows.
            if (current is JsProxy proxy)
            {
                if (!proxy.ProxySet(JsValue.String(key), value, baseValue) && strict)
                {
                    ThrowTypeError("Cannot assign to read only property '" + key + "'");
                }

                return;
            }

            // A TYPED ARRAY PART-WAY UP THE CHAIN, with a numeric key it holds no element for,
            // takes the write and discards it without converting the value: its [[Set]] answers
            // true for a receiver other than itself, and no setter behind it is reached. An element
            // it does hold is an ordinary writable data property from here on.
            if (current is JsTypedArray passedView && JsTypedArray.IsNumericKey(key) && !passedView.TryGetOwnProperty(key, out _))
            {
                return;
            }

            if (current.TryGetOwnProperty(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        if (strict)
                        {
                            ThrowTypeError("Cannot set property " + key + " which has only a getter");
                        }

                        return;
                    }

                    Call(JsValue.Object(property.Setter), baseValue, [value]);
                    return;
                }

                if (!property.Writable)
                {
                    if (strict)
                    {
                        ThrowTypeError("Cannot assign to read only property '" + key + "'");
                    }

                    return;
                }

                if (ReferenceEquals(current, target))
                {
                    property.Value = value;
                    target.SetOwnProperty(key, property);
                    return;
                }

                break;
            }

            current = current.Prototype;
        }

        if (target is null)
        {
            if (strict)
            {
                ThrowTypeError("Cannot create property '" + key + "' on a primitive");
            }

            return;
        }

        if (!target.Extensible)
        {
            if (strict)
            {
                ThrowTypeError("Cannot add property " + key + ", object is not extensible");
            }

            return;
        }

        target.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>An Array length, or the <c>RangeError</c> the language owes for anything else.</summary>
    /// <remarks>
    /// <b>The test is that the number survives the round trip</b>, which is the specification's own
    /// wording and is why <c>-1</c>, <c>1.5</c> and <c>NaN</c> are all refused while <c>"2"</c> is
    /// accepted: each of the three has a <c>ToUint32</c> that differs from its <c>ToNumber</c>, and
    /// the string does not.
    /// <para>
    /// <b>The value is converted twice, as <c>ArraySetLength</c> says</b>: once by
    /// <c>ToUint32</c> and once by <c>ToNumber</c>. An object's <c>valueOf</c> therefore runs twice
    /// and the two answers are compared, so one that answers differently the second time is a
    /// <c>RangeError</c> rather than whichever length the first call chose.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6A9832
    // Broiler-Human:        PENDING
    internal uint ArrayLengthOrRefuse(JsValue value)
    {
        var index = IntegerConversions.ToUint32(ToNumber(value));
        var number = ToNumber(value);

        if (index != number)
        {
            throw Error("RangeError", "Invalid array length");
        }

        return index;
    }

    /// <summary>Defines an own data property under a key that is not known until it is evaluated.</summary>
    /// <remarks>
    /// <b>A computed member of an object literal DEFINES and does not assign</b>, and the two differ
    /// wherever the chain has an opinion: <c>{ [k]: v }</c> with <c>k</c> of <c>"__proto__"</c> makes
    /// an own property called <c>__proto__</c>, where an assignment would have found the accessor on
    /// <c>Object.prototype</c> and moved the object's prototype instead. The same difference shows
    /// against any setter, and against a read-only property inherited from a frozen prototype.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4DBE17
    // Broiler-Human:        PENDING
    private void DefineByKey(JsObject host, JsValue key, JsValue value)
    {
        key = ToPropertyKeyValue(key);

        if (key.IsSymbol)
        {
            host.SetOwnSymbol(key.AsSymbol(), JsProperty.Data(value, JsPropertyAttributes.Default));
            return;
        }

        host.SetOwnProperty(key.AsString(), JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    // ---- the reflective forms of the two above -------------------------------------------------

    /// <summary>Reads a property off one object's chain with any accessor bound to another value.</summary>
    /// <remarks>
    /// <b>The chain that is walked and the <c>this</c> a getter sees are the same thing in every
    /// ordinary read</b>, because the base of the reference is both, and separating them is the
    /// entire reason <c>Reflect.get</c> takes a third argument. A program can run a getter it found
    /// on one object against an object that does not have it, which is how a class hierarchy reads
    /// an inherited accessor without inheriting.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B99E24
    // Broiler-Human:        PENDING
    internal JsValue GetWithReceiver(JsObject target, string key, JsValue receiver)
    {
        if (target is JsTypedArray view && JsTypedArray.IsNumericKey(key))
        {
            return view.TryGetOwnProperty(key, out var element) ? element.Value : JsValue.Undefined;
        }

        return Lookup(target, key, receiver);
    }

    /// <summary>The same read for a Symbol-keyed property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C5E6A9
    // Broiler-Human:        PENDING
    internal JsValue GetSymbolWithReceiver(JsObject target, JsSymbol key, JsValue receiver)
    {
        var current = target;

        while (current is not null)
        {
            // THE SAME WHOLE-OPERATION RULE THE STRING WALK OBEYS, and for the same reason.
            if (current is JsProxy proxy)
            {
                return proxy.ProxyGet(JsValue.Symbol(key), receiver);
            }

            if (current.TryGetOwnSymbol(key, out var property))
            {
                if (!property.IsAccessor)
                {
                    return property.Value;
                }

                return property.Getter is null ? JsValue.Undefined : Call(JsValue.Object(property.Getter), receiver, []);
            }

            current = current.Prototype;
        }

        return JsValue.Undefined;
    }

    /// <summary>Writes through one object's chain, lands the write on another, and answers whether
    /// it took.</summary>
    /// <remarks>
    /// <para>
    /// <b>The answer is the point, and it is not the same question as "what does the property read
    /// back as".</b> A setter that discards what it was handed still took the write — the language
    /// says <c>[[Set]]</c> is true whenever a setter ran — and a read-back would call it a refusal.
    /// So this walks the chain itself rather than storing and looking.
    /// </para>
    /// <para>
    /// <b>The walk is over the target and the store is on the receiver</b>, which is what makes a
    /// data property found on a prototype shadow rather than overwrite. The two coincide for every
    /// call that does not name a receiver, which is nearly all of them.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D0728E
    // Broiler-Human:        PENDING
    internal bool SetWithReceiver(JsObject target, string key, JsValue value, JsValue receiver)
    {
        // A NUMERIC KEY ON A TYPED ARRAY IS THE INTEGER-INDEXED [[Set]]. When the view is its own
        // receiver the value is converted and written, or discarded if the key names no element;
        // for any other receiver a key that names no element is a success with no conversion,
        // and only a valid element goes on to the ordinary walk, which lands it on the receiver.
        if (target is JsTypedArray view && JsTypedArray.IsNumericKey(key))
        {
            if (receiver.IsObject && ReferenceEquals(receiver.AsObject(), view))
            {
                var element = ToElementValue(view.Kind, value);

                if (JsObject.IsArrayIndex(key, out var at))
                {
                    _ = view.TryWriteAt((int)at, element);
                }

                return true;
            }

            if (!view.TryGetOwnProperty(key, out _))
            {
                return true;
            }
        }

        // A NAMESPACE REFUSES EVERY WRITE, INCLUDING ONE TO A NAME IT DOES NOT EXPORT. Its export
        // properties read back as writable - the language says so, and a program can see it in the
        // descriptor - so the walk below would find a writable data property and let the write
        // land, silently turning a live binding into a copy. The refusal is unconditional and is
        // the object's, not the property's, which is why it is decided before the walk begins.
        if (target is JsModuleNamespace)
        {
            return false;
        }

        var current = target;

        while (current is not null)
        {
            // THE SAME WHOLE-OPERATION RULE, with the boolean this member already answers in.
            if (current is JsProxy proxy)
            {
                return proxy.ProxySet(JsValue.String(key), value, receiver);
            }

            // THE SAME RULE FOR A TYPED ARRAY PART-WAY UP THE CHAIN as for one at its start.
            if (current is JsTypedArray passedView && !ReferenceEquals(current, target) &&
                JsTypedArray.IsNumericKey(key))
            {
                if (receiver.IsObject && ReferenceEquals(receiver.AsObject(), passedView))
                {
                    var element = ToElementValue(passedView.Kind, value);

                    if (JsObject.IsArrayIndex(key, out var passedAt))
                    {
                        _ = passedView.TryWriteAt((int)passedAt, element);
                    }

                    return true;
                }

                if (!passedView.TryGetOwnProperty(key, out _))
                {
                    return true;
                }
            }

            if (current.TryGetOwnProperty(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        return false;
                    }

                    Call(JsValue.Object(property.Setter), receiver, [value]);
                    return true;
                }

                if (!property.Writable)
                {
                    return false;
                }

                break;
            }

            current = current.Prototype;
        }

        return LandOnReceiver(receiver, JsValue.String(key), value);
    }

    /// <summary>The same write for a Symbol-keyed property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C15A9A
    // Broiler-Human:        PENDING
    internal bool SetSymbolWithReceiver(JsObject target, JsSymbol key, JsValue value, JsValue receiver)
    {
        var current = target;

        while (current is not null)
        {
            // THE SAME WHOLE-OPERATION RULE, with the boolean this member already answers in.
            if (current is JsProxy proxy)
            {
                return proxy.ProxySet(JsValue.Symbol(key), value, receiver);
            }

            if (current.TryGetOwnSymbol(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        return false;
                    }

                    Call(JsValue.Object(property.Setter), receiver, [value]);
                    return true;
                }

                if (!property.Writable)
                {
                    return false;
                }

                break;
            }

            current = current.Prototype;
        }

        return LandOnReceiver(receiver, JsValue.Symbol(key), value);
    }

    /// <summary>Where a reflective write ends up: an own property of the receiver, or a refusal.</summary>
    /// <remarks>
    /// <para>
    /// <b>The receiver is asked through its own internal methods and is handed exactly what the
    /// specification hands it.</b> <c>OrdinarySetWithOwnDescriptor</c> reads the receiver's own
    /// property with <c>[[GetOwnProperty]]</c>; an existing writable data property is updated with
    /// <c>[[DefineOwnProperty]](P, { [[Value]]: V })</c> - the value and nothing else, so a Proxy
    /// receiver's <c>defineProperty</c> trap sees a one-field descriptor and an Array receiver's
    /// <c>length</c> goes through the Array's own definition - and an absent one is created by
    /// <c>CreateDataProperty</c>. Restating the whole stored descriptor, as this used to, showed a
    /// trap fields the language never passes and threw where a trap's <c>false</c> is the answer.
    /// </para>
    /// <para>
    /// Both key kinds come here, so a Symbol-keyed write cannot drift from a String-keyed one.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=E7D724
    // Broiler-Human:        PENDING
    private bool LandOnReceiver(JsValue receiver, JsValue key, JsValue value)
    {
        if (!receiver.IsObject)
        {
            return false;
        }

        var holder = receiver.AsObject();

        var held = key.IsSymbol
            ? holder.TryGetOwnSymbol(key.AsSymbol(), out var existing)
            : holder.TryGetOwnProperty(key.AsString(), out existing);

        var fields = new JsRealm.ObjectDescriptorFields { HasValue = true, Value = value };

        if (held)
        {
            if (existing.IsAccessor || !existing.Writable)
            {
                return false;
            }
        }
        else
        {
            fields.HasWritable = true;
            fields.Writable = true;
            fields.HasEnumerable = true;
            fields.Enumerable = true;
            fields.HasConfigurable = true;
            fields.Configurable = true;
        }

        return JsRealm.ObjectDefineOwn(this, holder, key, fields);
    }

    /// <summary>The <c>in</c> operator's lookup: does any object in the chain have the key.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1AC05C
    // Broiler-Human:        PENDING
    internal static bool HasProperty(JsObject start, string key)
    {
        // THE SAME EXOTIC RULE [[Get]] OBEYS, AND FOR THE SAME REASON. An integer-indexed object
        // does not inherit its indices, so `9 in new Int32Array(3)` is false whatever anybody put
        // on `Object.prototype`. Expressing it here rather than on the object is forced: the walk
        // is the engine's, not the object's.
        if (start is JsTypedArray view && JsTypedArray.IsNumericKey(key))
        {
            return view.TryGetOwnProperty(key, out _);
        }

        // A NAMESPACE ANSWERS THIS FROM ITS EXPORT SET AND NEVER READS THE BINDING. The walk below
        // asks each object for the property, and a namespace answers that by reading through to the
        // module's slot - which throws for an export whose module has not run yet. `'x' in ns` is
        // true for such a name and `ns.x` is a `ReferenceError`; going through the walk would make
        // both the same throw. The Symbol table is still the base's, so `@@toStringTag in ns` is
        // answered below by the ordinary path.
        if (start is JsModuleNamespace names)
        {
            return names.Exports(key);
        }

        var current = start;

        while (current is not null)
        {
            // A PROXY ANSWERS `in` WHOLE. The `has` trap is asked about the chain as well as the
            // object, so a walk that continued past it would ask the target the question the trap
            // has already answered.
            if (current is JsProxy proxy)
            {
                return proxy.ProxyHas(JsValue.String(key));
            }

            // A typed array part-way up the chain answers a numeric key itself, and ends the walk.
            if (current is JsTypedArray passedView && JsTypedArray.IsNumericKey(key))
            {
                return passedView.TryGetOwnProperty(key, out _);
            }

            if (current.TryGetOwnProperty(key, out _))
            {
                return true;
            }

            current = current.Prototype;
        }

        return false;
    }

    /// <summary>
    /// The abstract operation <c>CopyDataProperties</c>, which is what object spread is.
    /// </summary>
    /// <remarks>
    /// <b>Own and enumerable, in the order the source yields them, and through the ordinary read
    /// path.</b> Reading through the property path is what makes a getter on the source run once
    /// and contribute its value, which is what the language says and what copying descriptors would
    /// not do. A <c>null</c> or <c>undefined</c> source contributes nothing rather than throwing -
    /// <c>{...null}</c> is an empty object.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=137468
    // Broiler-Human:        PENDING
    internal void CopyDataProperties(JsObject target, JsValue source)
    {
        if (source.IsNullish)
        {
            return;
        }

        var from = ToObject(source);

        // EVERY OWN KEY, STRING AND SYMBOL, in one `[[OwnPropertyKeys]]` (JSP-6, JSC-238): the
        // spread of an object literal and the rest of an object pattern copy an enumerable
        // Symbol-keyed property as they copy a String-keyed one, where only the Strings were
        // asked for and every Symbol was dropped. One call, so a Proxy's `ownKeys` runs once.
        foreach (var key in from.OwnKeys())
        {
            Charge(1);

            if (key.IsSymbol)
            {
                var symbol = key.AsSymbol();

                if (from.TryGetOwnSymbol(symbol, out var symbolProperty) && symbolProperty.Enumerable)
                {
                    target.SetOwnSymbol(symbol, JsProperty.Data(GetSymbol(source, symbol), JsPropertyAttributes.Default));
                }

                continue;
            }

            var name = key.AsString();

            if (!from.TryGetOwnProperty(name, out var property) || !property.Enumerable)
            {
                continue;
            }

            target.SetOwnProperty(name, JsProperty.Data(GetProperty(source, name), JsPropertyAttributes.Default));
        }
    }

    /// <summary>Reads an indexed property, with the fast path an Array element deserves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=250E86
    // Broiler-Human:        PENDING
    internal JsValue GetIndexed(JsValue target, JsValue key)
    {
        // THE BASE IS CHECKED BEFORE THE KEY IS CONVERTED, and the order is observable. The
        // language builds a Reference from `base[expr]` WITHOUT converting the key, and converts it
        // where the reference is read - so `null[{ toString(){ throw x; } }]` is the `TypeError`
        // about the base and never the exception the conversion would have raised. A key that needs
        // no user code is still named in the message, because naming it costs nothing and runs
        // nothing.
        if (target.IsNullish && !key.IsSymbol && key.Type is not JsType.String and not JsType.Number)
        {
            ThrowTypeError(
                "Cannot read properties of " +
                    (target.Type == JsType.Null ? "null" : "undefined"));
        }

        if (target.IsObject && target.AsObject() is JsArray array && key.Type == JsType.Number)
        {
            var number = key.AsNumber();
            var at = (int)number;

            if (at == number && at >= 0 && at < array.DenseCount)
            {
                var element = array.DenseAt(at);

                // A HOLE IS NOT AN ANSWER. It may be a hole, or it may be a slot the array vacated
                // when the element was given attributes it could not carry, in which case the value
                // is in the ordinary map and the general path finds it.
                if (!element.IsEmpty)
                {
                    return element;
                }
            }
        }

        if (target.IsString && key.Type == JsType.Number)
        {
            var text = target.AsString();
            var number = key.AsNumber();
            var at = (int)number;

            if (at == number && at >= 0 && at < text.Length)
            {
                return JsValue.String(text[at].ToString());
            }
        }

        // THE KEY IS CONVERTED TO A STRING OR A SYMBOL, and a Symbol wrapper converts to its
        // Symbol: `o[Object(s)]` reads `o[s]`. Until 2026-10-04 the wrapper was converted to a
        // String, which throws (JSC-256).
        var property = ToPropertyKeyValue(key);

        if (property.IsSymbol)
        {
            return GetSymbol(target, property.AsSymbol());
        }

        return GetProperty(target, property.AsString());
    }

    /// <summary>Reads a Symbol-keyed property, walking the prototype chain the same way.</summary>
    /// <remarks>
    /// It is a second walk rather than a widened one because the key type differs all the way down:
    /// the storage is a separate table, and a String key and a Symbol key can never collide, so
    /// there is nothing for the two walks to agree about.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=9D20BE
    // Broiler-Human:        PENDING
    internal JsValue GetSymbol(JsValue baseValue, JsSymbol key)
    {
        if (baseValue.IsNullish)
        {
            return ThrowTypeError(
                "Cannot read properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") +
                " (reading a Symbol-keyed property)");
        }

        var current = baseValue.IsObject ? baseValue.AsObject() : PrototypeFor(baseValue);

        while (current is not null)
        {
            // THE SAME WHOLE-OPERATION RULE THE STRING WALK OBEYS, and for the same reason.
            if (current is JsProxy proxy)
            {
                return proxy.ProxyGet(JsValue.Symbol(key), baseValue);
            }

            if (current.TryGetOwnSymbol(key, out var property))
            {
                if (!property.IsAccessor)
                {
                    return property.Value;
                }

                return property.Getter is null
                    ? JsValue.Undefined
                    : Call(JsValue.Object(property.Getter), baseValue, []);
            }

            current = current.Prototype;
        }

        return JsValue.Undefined;
    }

    /// <summary>Writes a Symbol-keyed property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=E3C0FB
    // Broiler-Human:        PENDING
    internal void SetSymbol(JsValue baseValue, JsSymbol key, JsValue value, bool strict)
    {
        if (baseValue.IsNullish)
        {
            ThrowTypeError(
                "Cannot set properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") +
                " (setting a Symbol-keyed property)");

            return;
        }

        // A PRIMITIVE BASE WALKS ITS WRAPPER'S PROTOTYPE, as the String write does: a Symbol-keyed
        // setter inherited from `Number.prototype` runs with the primitive as `this`. Only a write
        // that would have to create or change an own property on the primitive is refused.
        var target = baseValue.AsObjectOrNull();
        var current = target ?? PrototypeFor(baseValue);

        while (current is not null)
        {
            // THE SAME WHOLE-OPERATION RULE THE STRING WRITE OBEYS, refusal and mode included.
            if (current is JsProxy proxy)
            {
                if (!proxy.ProxySet(JsValue.Symbol(key), value, baseValue) && strict)
                {
                    ThrowTypeError("Cannot assign to a read only Symbol-keyed property");
                }

                return;
            }

            if (current.TryGetOwnSymbol(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        if (strict)
                        {
                            ThrowTypeError("Cannot set a Symbol-keyed property which has only a getter");
                        }

                        return;
                    }

                    Call(JsValue.Object(property.Setter), baseValue, [value]);
                    return;
                }

                if (!property.Writable)
                {
                    if (strict)
                    {
                        ThrowTypeError("Cannot assign to a read only Symbol-keyed property");
                    }

                    return;
                }

                // AN OWN WRITABLE PROPERTY KEEPS ITS ATTRIBUTES AND TAKES ONLY THE VALUE. Writing
                // the default attribute set here, as this once did, made a sealed property
                // configurable again and a non-enumerable one enumerable on the first assignment.
                if (ReferenceEquals(current, target))
                {
                    property.Value = value;
                    target.SetOwnSymbol(key, property);
                    return;
                }

                break;
            }

            current = current.Prototype;
        }

        if (target is null)
        {
            if (strict)
            {
                ThrowTypeError("Cannot create a Symbol-keyed property on a primitive");
            }

            return;
        }

        // THE SAME EXTENSIBILITY DECISION THE STRING WRITE MAKES. A Symbol key is a new own
        // property like any other, so a frozen, sealed or merely non-extensible object refuses it:
        // silently in sloppy code and as a TypeError in strict code.
        if (!target.Extensible)
        {
            if (strict)
            {
                ThrowTypeError("Cannot add a Symbol-keyed property, object is not extensible");
            }

            return;
        }

        target.SetOwnSymbol(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>Whether a Symbol-keyed property is reachable from <paramref name="start"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=529AE1
    // Broiler-Human:        PENDING
    internal static bool HasSymbol(JsObject start, JsSymbol key)
    {
        var current = start;

        while (current is not null)
        {
            // THE SAME WHOLE-OPERATION RULE THE STRING WALK OBEYS, and for the same reason.
            if (current is JsProxy proxy)
            {
                return proxy.ProxyHas(JsValue.Symbol(key));
            }

            if (current.TryGetOwnSymbol(key, out _))
            {
                return true;
            }

            current = current.Prototype;
        }

        return false;
    }

    /// <summary>Writes an indexed property, with the fast path an Array element deserves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=25A379
    // Broiler-Human:        PENDING
    internal void SetIndexed(JsValue target, JsValue key, JsValue value, bool strict)
    {
        // The same ordering the read half performs, for the same reason: a write through a
        // reference whose base is nullish is a `TypeError` about the base, decided before anything
        // the key's conversion could run.
        if (target.IsNullish && !key.IsSymbol && key.Type is not JsType.String and not JsType.Number)
        {
            ThrowTypeError(
                "Cannot set properties of " +
                    (target.Type == JsType.Null ? "null" : "undefined"));
        }

        if (target.IsObject && target.AsObject() is JsArray array && key.Type == JsType.Number)
        {
            var number = key.AsNumber();
            var at = (int)number;

            // THE FAST PATH IS FOR AN ELEMENT THAT IS STILL AN ELEMENT. A dense slot the array
            // vacated is a property with attributes living in the ordinary map, and writing the
            // slot would step straight over a `writable: false` that somebody asked for - which is
            // what made a frozen array assignable while reporting itself frozen. Appending is a
            // fast path too, and only while the array is extensible.
            if (at == number && at >= 0)
            {
                // A CLOSED LENGTH REFUSES BEFORE EITHER FAST PATH IS TAKEN, and in strict code the
                // refusal is a TypeError. The object model drops such a write silently because it
                // cannot know the mode; this is where the mode is known.
                if (!array.LengthWritable && at >= array.Length)
                {
                    if (strict)
                    {
                        ThrowTypeError("Cannot add property " + at.ToString(CultureInfo.InvariantCulture) + ", object is not extensible");
                    }

                    return;
                }

                if (at < array.DenseCount)
                {
                    if (!array.DenseAt(at).IsEmpty)
                    {
                        array.SetIndex((uint)at, value);
                        return;
                    }
                }
                else if (array.Extensible && at == array.Length &&
                         !ChainMayAnswerIndex(array.Prototype, at))
                {
                    array.SetIndex((uint)at, value);
                    return;
                }
            }
        }

        var property = ToPropertyKeyValue(key);

        if (property.IsSymbol)
        {
            SetSymbol(target, property.AsSymbol(), value, strict);
            return;
        }

        SetProperty(target, property.AsString(), value, strict);
    }

    /// <summary>
    /// Whether anything on a prototype chain could answer a write to index <paramref name="at"/>
    /// before it lands on the Array that starts the chain.
    /// </summary>
    /// <remarks>
    /// <b>An append is <c>OrdinarySet</c>, and <c>OrdinarySet</c> walks the chain.</b> An inherited
    /// setter at the index runs instead of the append, and an inherited read-only property refuses
    /// it; the append fast path stepped over both, so <c>Array.prototype</c> defining a setter at
    /// <c>"0"</c> never saw <c>[].push(1)</c>. A Proxy or a typed array on the chain answers the
    /// write itself and is always sent to the full walk; any other prototype is asked only whether
    /// it holds the key, and the ordinary chain - <c>Array.prototype</c> then
    /// <c>Object.prototype</c> - holds none.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=35F4F3
    // Broiler-Human:        PENDING
    private static bool ChainMayAnswerIndex(JsObject? prototype, int at)
    {
        string? key = null;

        for (var current = prototype; current is not null; current = current.Prototype)
        {
            if (current is JsProxy or JsTypedArray)
            {
                return true;
            }

            key ??= JsNumberFormat.ToUintString((uint)at);

            if (current.TryGetOwnProperty(key, out _))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// The specification's <c>CreateListFromArrayLike</c> narrowed to the two kinds of property key.
    /// </summary>
    /// <remarks>
    /// <b>An array-like and not an iterable</b>, which is the same choice <c>Reflect.apply</c> makes
    /// and for the same reason: the specification says so, and a handler returning a Set would be
    /// returning something with no <c>length</c>. <b>The element check is not politeness either</b> —
    /// every caller of this treats what comes back as an own key, and a Number in the list would
    /// become a key nothing on the target could ever match, so an <c>ownKeys</c> trap that answered
    /// <c>[0]</c> would silently report a property called neither <c>0</c> nor anything else.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F70F2F
    // Broiler-Human:        PENDING
    internal List<JsValue> ProxyKeyList(JsValue list)
    {
        if (!list.IsObject)
        {
            ThrowTypeError("the 'ownKeys' trap answered a value that is not an object");
        }

        var length = IntegerConversions.ToIntegerOrInfinity(ToNumber(GetProperty(list, "length")));

        if (length > JsRealm.ReflectArgumentCeiling)
        {
            ThrowRangeError("the 'ownKeys' trap answered a list longer than this profile admits");
        }

        var collected = new List<JsValue>();

        for (var at = 0; at < length; at++)
        {
            Charge(1);
            var element = GetIndexed(list, JsValue.Number(at));

            if (!element.IsSymbol && element.Type != JsType.String)
            {
                ThrowTypeError("the 'ownKeys' trap answered a key that is neither a String nor a Symbol");
            }

            collected.Add(element);
        }

        return collected;
    }
}
