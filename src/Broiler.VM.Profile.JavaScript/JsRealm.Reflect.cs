// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           0
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>Reflect</c> namespace object: the internal methods, as functions, with their real answers.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is not a tidier <c>Object</c>, and the difference is what it answers on failure.</b>
/// <c>Object.defineProperty</c> throws when the object refuses and returns the object when it does
/// not; <c>Reflect.defineProperty</c> returns <c>false</c> and <c>true</c>. A program that wants to
/// know whether a definition took has no way to ask through <c>Object</c> without a <c>try</c>, and
/// that is the whole reason this namespace exists. Every member here is written to that rule
/// rather than delegated to its <c>Object</c> counterpart with the throw swallowed.
/// </para>
/// <para>
/// <b>Every member is generic and refuses a non-object receiver by name.</b> <c>Object.keys(5)</c>
/// coerces; <c>Reflect.ownKeys(5)</c> is a <c>TypeError</c>. The pair is deliberate in the language:
/// the <c>Object</c> statics are for programs handling values, and these are for programs handling
/// objects, so a value reaching one of these is a mistake worth reporting.
/// </para>
/// <para>
/// <b><c>Reflect.construct</c> is the only way to name a <c>new.target</c> the call site is not.</b>
/// That is what makes a subclass factory possible — <c>Reflect.construct(Base, args, Derived)</c>
/// builds an instance with the DERIVED prototype — and it is why this namespace could not exist
/// before the frame carried a <c>new.target</c> of its own.
/// </para>
/// <para>
/// <b><c>Proxy</c> is here now, and the claim this paragraph used to make — that nothing here
/// assumed an ordinary object in a way that would have to change — was false in four members.</b>
/// <c>ownKeys</c> read the two key tables separately, which is two <c>ownKeys</c> trap calls for one
/// internal method; <c>preventExtensions</c> assigned rather than asked, so it answered <c>true</c>
/// for a trap that had refused; <c>setPrototypeOf</c> ran <c>OrdinarySetPrototypeOf</c>'s three
/// tests against an object entitled to define its own <c>[[SetPrototypeOf]]</c>; and
/// <c>defineProperty</c> validated the descriptor against the proxy before reaching the trap that
/// decides. Each is repaired at the member, and
/// [JSC-150](roadmap.corrections.md#jsc-150) records what the four cost.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>Builds <c>Reflect</c> and defines it on the global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A30039
    // Broiler-Human:        PENDING
    private void SetupReflect()
    {
        var reflect = new JsObject(ObjectPrototype, "Reflect");
        GlobalObject.DefineBuiltIn("Reflect", JsValue.Object(reflect));

        reflect.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Reflect"), JsPropertyAttributes.Configurable));

        Method(reflect, "get", 2, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ReflectTarget(engine, arguments, "get");
            var key = ArgOfReflect(arguments, 1);

            // THE RECEIVER IS A THIRD ARGUMENT AND IT IS THE POINT OF THIS FUNCTION. A getter
            // inherited from a prototype runs with `this` bound to whatever the caller names, which
            // is how a program reads an accessor as though it were on another object. Defaulting it
            // to the target is what an ordinary property read does.
            var receiver = arguments.Length > 2 ? arguments[2] : JsValue.Object(target);

            return key.IsSymbol
                ? engine.GetSymbolWithReceiver(target, key.AsSymbol(), receiver)
                : engine.GetWithReceiver(target, engine.ToPropertyKey(key), receiver);
        });

        Method(reflect, "set", 3, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ReflectTarget(engine, arguments, "set");
            var key = ArgOfReflect(arguments, 1);
            var value = ArgOfReflect(arguments, 2);
            var receiver = arguments.Length > 3 ? arguments[3] : JsValue.Object(target);

            // IT ANSWERS WHETHER THE WRITE TOOK, which is a different question from what the
            // property reads back as: a setter that discards what it was handed still took the
            // write. So the store reports for itself rather than being made and then looked at.
            return JsValue.Boolean(
                key.IsSymbol
                    ? engine.SetSymbolWithReceiver(target, key.AsSymbol(), value, receiver)
                    : engine.SetWithReceiver(target, engine.ToPropertyKey(key), value, receiver));
        });

        Method(reflect, "has", 2, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ReflectTarget(engine, arguments, "has");
            var key = ArgOfReflect(arguments, 1);

            return JsValue.Boolean(
                key.IsSymbol
                    ? engine.HasSymbol(target, key.AsSymbol())
                    : engine.HasProperty(target, engine.ToPropertyKey(key)));
        });

        Method(reflect, "deleteProperty", 2, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ReflectTarget(engine, arguments, "deleteProperty");
            var key = ArgOfReflect(arguments, 1);

            return JsValue.Boolean(
                key.IsSymbol
                    ? target.DeleteOwnSymbol(key.AsSymbol())
                    : target.DeleteOwnProperty(engine.ToPropertyKey(key)));
        });

        Method(reflect, "ownKeys", 1, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ReflectTarget(engine, arguments, "ownKeys");
            var result = NewArray();

            // THE STRING KEYS COME FIRST AND THE SYMBOL KEYS AFTER, all of them, in the order the
            // object holds them. It is the one function that reports both tables, which is why a
            // program walking an object completely has to use it rather than `Object.keys`.
            //
            // IT ASKS FOR BOTH IN ONE CALL, and that is not tidiness: `[[OwnPropertyKeys]]` is ONE
            // internal method, so asking a Proxy for its String keys and then its Symbol keys
            // would run the `ownKeys` trap twice - visibly, and with nothing to make the two
            // answers agree.
            foreach (var key in target.OwnKeys())
            {
                engine.Charge(1);
                result.Push(key);
            }

            return JsValue.Object(result);
        });

        Method(reflect, "getPrototypeOf", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var held = ReflectTarget(engine, arguments, "getPrototypeOf").Prototype;
            return held is null ? JsValue.Null : JsValue.Object(held);
        });

        Method(reflect, "setPrototypeOf", 2, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ReflectTarget(engine, arguments, "setPrototypeOf");
            var wanted = ArgOfReflect(arguments, 1);

            if (!wanted.IsObject && wanted.Type != JsType.Null)
            {
                return engine.ThrowTypeError("Reflect.setPrototypeOf: the prototype is not an object or null");
            }

            var prototype = wanted.AsObjectOrNull();

            // A PROXY ANSWERS THIS ITSELF AND `OrdinarySetPrototypeOf` DOES NOT APPLY TO IT. An
            // exotic object's `[[SetPrototypeOf]]` is not obliged to be that function, and running
            // it first would read the proxy's prototype and extensibility through two traps the
            // specification never says to call here.
            //
            // A CYCLE IS A REFUSAL HERE AND A THROW EVERYWHERE ELSE, which is why the ordinary
            // form answers a boolean and `Object.setPrototypeOf` is the one that turns it into an
            // exception. This member used to carry a copy of that walk for want of a shared body.
            return JsValue.Boolean(
                target is JsProxy proxy
                    ? proxy.ProxySetPrototypeOf(prototype)
                    : ObjectSetPrototypeOrdinary(engine, target, prototype));
        });

        Method(reflect, "isExtensible", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            return JsValue.Boolean(ReflectTarget(engine, arguments, "isExtensible").Extensible);
        });

        Method(reflect, "preventExtensions", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ReflectTarget(engine, arguments, "preventExtensions");

            // `[[PreventExtensions]]` ANSWERS A BOOLEAN AND AN ORDINARY OBJECT'S IS ALWAYS TRUE,
            // which is why this read as an assignment for as long as nothing could refuse. A Proxy
            // can refuse, and this is the member whose whole purpose is to say so rather than throw.
            return JsValue.Boolean(
                target is JsProxy proxy
                    ? proxy.ProxyPreventExtensions()
                    : ObjectPreventExtensionsOrdinary(target));
        });

        Method(reflect, "defineProperty", 3, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ReflectTarget(engine, arguments, "defineProperty");
            var key = ArgOfReflect(arguments, 1);

            // THE KEY AND THE DESCRIPTOR ARE READ OUTSIDE THE `try`, and that is not tidiness. Both
            // can run guest code - a `toString` on the key, a getter on the descriptor - and an
            // exception from THAT is the program's own, not this object's refusal. Reading them
            // inside would report a `TypeError` the guest threw as `false`.
            var name = key.IsSymbol ? null : engine.ToPropertyKey(key);
            var fields = ObjectToDescriptorFields(engine, ArgOfReflect(arguments, 2));

            // A PROXY IS ASKED DIRECTLY AND ITS REFUSAL IS RETURNED RATHER THAN CAUGHT. The catch
            // below turns a refusal into `false` by turning an exception into one, which is right
            // for an ordinary object whose only way of refusing IS to throw - but a `defineProperty`
            // trap answers `false` in its own voice, and routing it through the ordinary path would
            // have validated the descriptor against the PROXY (two more trap calls the language
            // never asks for) before ever reaching the trap that decides.
            if (target is JsProxy proxy)
            {
                return JsValue.Boolean(
                    proxy.ProxyDefineOwnProperty(name is null ? key : JsValue.String(name), fields));
            }

            // THE ANSWER IS A BOOLEAN AND THE REFUSAL IS STILL A REFUSAL. `ObjectApplyDescriptor`
            // throws when the object declines, because that is what `Object.defineProperty` owes;
            // here the throw IS the answer, and turning it into `false` is the one place this file
            // catches rather than reports.
            try
            {
                if (name is null)
                {
                    target.SetOwnSymbol(key.AsSymbol(), ObjectPropertyFromFields(engine, target, fields));
                    return JsValue.True;
                }

                ObjectApplyDescriptor(engine, target, name, fields);
                return JsValue.True;
            }
            catch (JsThrow)
            {
                return JsValue.False;
            }
        });

        Method(reflect, "getOwnPropertyDescriptor", 2, (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ReflectTarget(engine, arguments, "getOwnPropertyDescriptor");
            var key = ArgOfReflect(arguments, 1);

            if (key.IsSymbol)
            {
                return target.TryGetOwnSymbol(key.AsSymbol(), out var owned)
                    ? JsValue.Object(ObjectDescriptorFor(owned))
                    : JsValue.Undefined;
            }

            return target.TryGetOwnProperty(engine.ToPropertyKey(key), out var property)
                ? JsValue.Object(ObjectDescriptorFor(property))
                : JsValue.Undefined;
        });

        Method(reflect, "apply", 3, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ArgOfReflect(arguments, 0);

            if (!target.IsObject || !target.AsObject().IsCallable)
            {
                return engine.ThrowTypeError("Reflect.apply: the target is not callable");
            }

            return engine.Call(
                target, ArgOfReflect(arguments, 1), ReflectArguments(engine, ArgOfReflect(arguments, 2)));
        });

        Method(reflect, "construct", 2, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var target = ArgOfReflect(arguments, 0);

            // CALLABLE IS NOT THE QUESTION HERE, and asking it is how an engine reports every one
            // of its own built-ins as a constructor. An arrow, a method, and every function in this
            // realm that has no `[[Construct]]` are all callable and none of them may be `new`ed;
            // the suite's own `isConstructor` is written as a call to THIS function, so a callable
            // check here makes the realm answer wrongly about itself.
            if (!target.IsObject || !target.AsObject().IsConstructor)
            {
                return engine.ThrowTypeError("Reflect.construct: the target is not a constructor");
            }

            var newTarget = arguments.Length > 2 ? arguments[2] : target;

            if (!newTarget.IsObject || !newTarget.AsObject().IsConstructor)
            {
                return engine.ThrowTypeError("Reflect.construct: the new target is not a constructor");
            }

            return engine.Construct(
                target,
                arguments.Length > 1 ? ReflectArguments(engine, arguments[1]) : [],
                newTarget);
        });
    }

    /// <summary>The first argument as an object, or the <c>TypeError</c> this namespace owes.</summary>
    /// <remarks>
    /// <b>It refuses rather than coercing</b>, which is the difference from the <c>Object</c>
    /// statics beside it: those are for programs handling values and these are for programs handling
    /// objects, so a primitive arriving here is a mistake and saying so is more useful than wrapping
    /// it and answering about the wrapper.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5709CE
    // Broiler-Human:        PENDING
    private static JsObject ReflectTarget(JsEngine engine, JsValue[] arguments, string member)
    {
        var target = ArgOfReflect(arguments, 0);

        return target.IsObject
            ? target.AsObject()
            : (JsObject)engine.ThrowTypeError(
                "Reflect." + member + " called on a value that is not an object").AsObject();
    }

    /// <summary>One argument, or <c>undefined</c> where there is none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=71D3D8
    // Broiler-Human:        PENDING
    private static JsValue ArgOfReflect(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>Reads an argument list out of an array-like, which is what these two take.</summary>
    /// <remarks>
    /// <b>An array-like and not an iterable</b>, which is what the specification says for
    /// <c>Reflect.apply</c> and <c>Reflect.construct</c> and is worth stating because the neighbours
    /// went the other way: a spread argument iterates, and these read a <c>length</c>. The two exist
    /// for different callers, and a program handing one of these a Set is handing it something with
    /// no <c>length</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=961EDF
    // Broiler-Human:        PENDING
    private static JsValue[] ReflectArguments(JsEngine engine, JsValue list)
    {
        if (!list.IsObject)
        {
            engine.ThrowTypeError("the argument list is not an object");
        }

        var length = JsValue.ToInteger(engine.ToNumber(engine.GetProperty(list, "length")));

        if (length <= 0)
        {
            return [];
        }

        if (length > ReflectArgumentCeiling)
        {
            engine.ThrowRangeError("the argument list is longer than this profile admits");
        }

        var collected = new JsValue[(int)length];

        for (var at = 0; at < collected.Length; at++)
        {
            engine.Charge(1);
            collected[at] = engine.GetIndexed(list, JsValue.Number(at));
        }

        return collected;
    }

    /// <summary>The longest argument list <c>apply</c> and <c>construct</c> will materialise.</summary>
    /// <remarks>
    /// The specification's ceiling is 2^32-1, which no implementation can honour and which this one
    /// would meet by exhausting the host's memory rather than the guest's allowance. It is a declared
    /// deviation, the same one the String methods make about a length they can name before producing
    /// it, and a list past it gets the <c>RangeError</c> the specification gives for a length it
    /// cannot represent.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=92F0AC
    // Broiler-Human:        PENDING
    internal const int ReflectArgumentCeiling = 1 << 20;
}
