// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           0
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  5/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Generic;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    // ---- classes -------------------------------------------------------------------------------

    /// <summary>Reads a <c>this</c> that a derived constructor may not have bound yet.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=045502
    // Broiler-Human:        PENDING
    private JsValue ThisBinding(JsCell binding)
    {
        if (binding.Value.IsEmpty)
        {
            ThrowReferenceError("Must call super constructor in derived class before accessing 'this' or returning from derived constructor");
        }

        return binding.Value;
    }

    /// <summary>
    /// Defines one method-shaped member, and gives it the home object that makes its <c>super</c>
    /// resolve.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The member's name is set here and not where the closure was made</b>, because a computed
    /// key is not known until now: <c>class C { [k]() { } }</c> has to report <c>k</c>'s value as
    /// the method's name, and an accessor reports <c>"get x"</c> rather than <c>"x"</c>. The code
    /// unit carries whatever the source spelled, which is right for the common case and empty for
    /// the computed one.
    /// </para>
    /// <para>
    /// A getter and a setter for one key are one property, so defining either keeps whichever half
    /// is already there - but only when what is already there is an accessor. A data property of
    /// the same name is replaced outright, which is what redeclaring it in a class body means.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=52B47F
    // Broiler-Human:        PENDING
    private void DefineMember(JsObject host, string key, JsValue member, byte flags)
    {
        // DEFINING A MEMBER IS DefinePropertyOrThrow AND NOT AN UNCHECKED WRITE, and the one key
        // that can already be there and refuse is a class's own `prototype`: it is not
        // configurable, so `class C { static ['prototype']() { } }` is a TypeError rather than a
        // class whose `prototype` is a method. Every other key a member can name - `name`,
        // `length`, `constructor`, anything an object literal writes - is configurable, so this
        // refuses nothing a program is entitled to do.
        if (host.TryGetOwnProperty(key, out var standing) && !standing.Configurable)
        {
            ThrowTypeError("Cannot redefine property: " + key);
        }

        var getter = (flags & JsOpcodes.MemberIsGetter) != 0;
        var setter = (flags & JsOpcodes.MemberIsSetter) != 0;

        var attributes = JsPropertyAttributes.Configurable |
            ((flags & JsOpcodes.MemberIsEnumerable) != 0
                ? JsPropertyAttributes.Enumerable
                : JsPropertyAttributes.None);

        if (member.IsObject && member.AsObject() is JsScriptFunction bodied)
        {
            // A NAMED VALUE IS NAMED AND IS NOT A METHOD: `{ [k]: function () {} }` gives the
            // function `k`'s value as its name and no home object (JSP-6, JSC-238).
            if ((flags & JsOpcodes.MemberIsNamedValue) == 0)
            {
                bodied.HomeObject = host;
            }

            var label = getter ? "get " + key : setter ? "set " + key : key;
            bodied.FunctionName = label;

            bodied.SetOwnProperty("name", JsProperty.Data(JsValue.String(label), JsPropertyAttributes.Configurable));
        }

        if (!getter && !setter)
        {
            host.SetOwnProperty(key, JsProperty.Data(member, attributes | JsPropertyAttributes.Writable));

            return;
        }

        host.TryGetOwnProperty(key, out var existing);
        var accessor = member.AsObjectOrNull();

        host.SetOwnProperty(key,
            JsProperty.Accessor(
                getter ? accessor : existing.IsAccessor ? existing.Getter : null,
                setter ? accessor : existing.IsAccessor ? existing.Setter : null,
                attributes));
    }

    /// <summary>Defines one member under a Symbol key, which a computed member may name.</summary>
    /// <remarks>
    /// <para>
    /// <b>It is a second method rather than a widened one for the same reason the reads are two
    /// walks</b>: the storage is a separate table, a String key and a Symbol key can never collide,
    /// and there is nothing for the two to agree about beyond the flags — which is why the flag
    /// decoding is the only text repeated here.
    /// </para>
    /// <para>
    /// <b>The function's name is the description in brackets</b>, which is what the language says
    /// and what makes <c>C.prototype[Symbol.iterator].name</c> answer
    /// <c>"[Symbol.iterator]"</c>. A Symbol with no description names an empty function, because
    /// there is nothing to put between the brackets.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=6632D9
    // Broiler-Human:        PENDING
    private void DefineSymbolMember(JsObject host, JsSymbol key, JsValue member, byte flags)
    {
        if (host.TryGetOwnSymbol(key, out var standing) && !standing.Configurable)
        {
            ThrowTypeError("Cannot redefine property: " + key.Rendered);
        }

        var getter = (flags & JsOpcodes.MemberIsGetter) != 0;
        var setter = (flags & JsOpcodes.MemberIsSetter) != 0;

        var attributes = JsPropertyAttributes.Configurable |
            ((flags & JsOpcodes.MemberIsEnumerable) != 0
                ? JsPropertyAttributes.Enumerable
                : JsPropertyAttributes.None);

        if (member.IsObject && member.AsObject() is JsScriptFunction bodied)
        {
            if ((flags & JsOpcodes.MemberIsNamedValue) == 0)
            {
                bodied.HomeObject = host;
            }

            var described = SymbolFunctionName(key);
            var label = getter ? "get " + described : setter ? "set " + described : described;
            bodied.FunctionName = label;

            bodied.SetOwnProperty(
                "name",
                JsProperty.Data(JsValue.String(label), JsPropertyAttributes.Configurable));
        }

        if (!getter && !setter)
        {
            host.SetOwnSymbol(
                key, JsProperty.Data(member, attributes | JsPropertyAttributes.Writable));

            return;
        }

        host.TryGetOwnSymbol(key, out var existing);
        var accessor = member.AsObjectOrNull();

        host.SetOwnSymbol(
            key,
            JsProperty.Accessor(
                getter ? accessor : existing.IsAccessor ? existing.Getter : null,
                setter ? accessor : existing.IsAccessor ? existing.Setter : null,
                attributes));
    }

    /// <summary>
    /// The name a function takes from a Symbol key: its description in brackets, or the empty
    /// String for a Symbol with none.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B988B9
    // Broiler-Human:        PENDING
    private static string SymbolFunctionName(JsSymbol key) => key.Described ? "[" + key.Description + "]" : string.Empty;

    /// <summary>Builds the object graph a class definition is.</summary>
    /// <remarks>
    /// <b>The heritage is validated before anything is built</b>, so a bad <c>extends</c> leaves no
    /// half-made class behind: a superclass that is neither <c>null</c> nor a constructor, or one
    /// whose <c>prototype</c> is a primitive, is a TypeError at the definition rather than a
    /// surprise at the first <c>new</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=42386B
    // Broiler-Human:        PENDING
    private JsValue BuildClass(JsValue constructor, bool derived, JsValue heritage)
    {
        Charge(8);
        var target = (JsScriptFunction)constructor.AsObject();
        JsObject? inherited = Realm.ObjectPrototype;
        JsObject? constructorParent = Realm.FunctionPrototype;

        if (derived && heritage.Type != JsType.Null)
        {
            if (!heritage.IsObject || !heritage.AsObject().IsConstructor)
            {
                return ThrowTypeError(
                    "Class extends value " + Describe(heritage) +
                    " is not a constructor or null");
            }

            var parentPrototype = GetProperty(heritage, "prototype");

            if (!parentPrototype.IsObject && parentPrototype.Type != JsType.Null)
            {
                return ThrowTypeError(
                    "Class extends value does not have valid prototype property " +
                    Describe(parentPrototype));
            }

            inherited = parentPrototype.AsObjectOrNull();
            constructorParent = heritage.AsObject();
        }
        else if (derived)
        {
            // `extends null` GIVES THE PROTOTYPE NO PROTOTYPE and leaves the constructor an
            // ordinary function object. The class is still DERIVED, which is why constructing one
            // fails: its `super()` has `Function.prototype` above it and that is not a constructor.
            inherited = null;
        }

        var prototype = new JsObject(inherited);

        prototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(
                constructor,
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        target.Prototype = constructorParent;
        target.HomeObject = prototype;

        target.SetOwnProperty(
            "prototype",
            JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));

        return constructor;
    }

    /// <summary>Records one class element on the constructor for later application.</summary>
    /// <remarks>
    /// <para>
    /// <b>The home object is given here and not when the element is applied</b>, because it is a
    /// property of the FUNCTION and the function is created once. A field initialiser's
    /// <c>super.x</c> reads through the class prototype for every instance, not through whatever
    /// object the initialiser happened to run against.
    /// </para>
    /// <para>
    /// <b>A second half of an accessor merges rather than appending</b>, and it merges only into an
    /// element of the same name in the same list. <c>get #a</c> and <c>set #a</c> declare one
    /// private name; two records would install two elements under one name and the second would
    /// hide the first.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=24B57B
    // Broiler-Human:        PENDING
    private void RecordClassElement(JsScriptFunction target, JsObject prototype, JsValue key, JsValue body, byte flags)
    {
        Charge(4);
        var isStatic = (flags & JsOpcodes.ElementIsStatic) != 0;

        // A FIELD'S COMPUTED KEY IS CONVERTED ONCE, WHEN THE CLASS IS DEFINED, which is where
        // `ClassFieldDefinitionEvaluation` converts it: a key whose `toString` counts is asked once
        // however many instances the class makes, and the name a computed field's function takes is
        // the key it produced (JSP-6, JSC-238). A private name is already a key, and a static
        // block has none.
        if ((flags & JsOpcodes.ElementIsBlock) == 0 && !key.IsSymbol)
        {
            key = ToPropertyKeyValue(key);
        }

        if (body.IsObject && body.AsObject() is JsScriptFunction bodied)
        {
            bodied.HomeObject = isStatic ? target : prototype;
        }

        var list = isStatic
            ? target.StaticElements ??= []
            : target.InstanceElements ??= [];

        if ((flags & JsOpcodes.ElementIsSetter) != 0)
        {
            foreach (var standing in list)
            {
                if (standing.Key.IsSymbol && key.IsSymbol &&
                    ReferenceEquals(standing.Key.AsSymbol(), key.AsSymbol()))
                {
                    standing.Setter = body;
                    standing.Flags |= JsOpcodes.ElementIsSetter;
                    return;
                }
            }

            list.Add(
                new JsClassElement { Key = key, Body = JsValue.Undefined, Setter = body, Flags = flags });

            return;
        }

        if ((flags & JsOpcodes.ElementIsGetter) != 0)
        {
            foreach (var standing in list)
            {
                if (standing.Key.IsSymbol && key.IsSymbol &&
                    ReferenceEquals(standing.Key.AsSymbol(), key.AsSymbol()))
                {
                    standing.Body = body;
                    standing.Flags |= JsOpcodes.ElementIsGetter;
                    return;
                }
            }
        }

        list.Add(new JsClassElement { Key = key, Body = body, Setter = JsValue.Undefined, Flags = flags });
    }

    /// <summary>Runs the elements a class body recorded against the constructor itself.</summary>
    /// <remarks>
    /// <b>The private methods go on in a pass of their own, before anything runs.</b> A static
    /// field's initialiser and a static block may both call a private static method, and the class
    /// body is entitled to write the method after them - <c>class C { static a = C.#m(); static
    /// #m() { return 1 } }</c> is an ordinary program. One pass in source order would have made
    /// that a <c>TypeError</c> about a method the class does have.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3723CE
    // Broiler-Human:        PENDING
    private void RunStaticElements(JsScriptFunction target)
    {
        if (target.StaticElements is not { } elements)
        {
            return;
        }

        ApplyClassElements(target, elements, methods: true);
        ApplyClassElements(target, elements, methods: false);
    }

    /// <summary>Gives one new instance the elements its class recorded.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C27CBF
    // Broiler-Human:        PENDING
    private void InitialiseInstanceElements(JsObject instance, JsScriptFunction constructor)
    {
        if (constructor.InstanceElements is not { } elements)
        {
            return;
        }

        ApplyClassElements(instance, elements, methods: true);
        ApplyClassElements(instance, elements, methods: false);
    }

    /// <summary>Applies one pass of a recorded element list to one object.</summary>
    /// <remarks>
    /// <b>A field's initialiser is CALLED and a method is not</b>, which is the whole of what the
    /// method bit decides here. The receiver of the call is the object being given the element, so
    /// <c>class C { x = this.y }</c> reads the instance and <c>class C { static x = this.name }</c>
    /// reads the constructor - one rule, two objects, decided by which list the element was in.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=963DE0
    // Broiler-Human:        PENDING
    private void ApplyClassElements(JsObject target, List<JsClassElement> elements, bool methods)
    {
        var receiver = JsValue.Object(target);
        HashSet<JsSymbol>? installed = null;

        foreach (var element in elements)
        {
            var isMethod = (element.Flags & JsOpcodes.ElementIsMethod) != 0;

            if (isMethod != methods)
            {
                continue;
            }

            Charge(4);

            if (isMethod)
            {
                var method = element.Key.AsSymbol();

                // A PRIVATE METHOD OR ACCESSOR INSTALLED TWICE ON ONE OBJECT IS A TypeError too
                // (PrivateMethodOrAccessorAdd), reached the same way a field's is. A getter and a
                // setter of one name are one element of the class and two rows here, so only the
                // first row of a name in this pass asks. Until 2026-10-04 the second construction
                // installed it again (JSC-258).
                installed ??= [];

                if (installed.Add(method) && target.HasPrivate(method))
                {
                    ThrowTypeError("Cannot initialize " + method.Description + " twice on the same object");
                }

                target.SetPrivate(method, PrivateElementOf(element));
                continue;
            }

            if ((element.Flags & JsOpcodes.ElementIsBlock) != 0)
            {
                Call(element.Body, receiver, []);
                continue;
            }

            var value = element.Body.Type == JsType.Undefined
                ? JsValue.Undefined
                : Call(element.Body, receiver, []);

            // A COMPUTED FIELD'S ANONYMOUS FUNCTION TAKES THE KEY AS ITS NAME, which the initialiser
            // cannot see: the class definition converted the key, and nothing runs between the
            // function's creation and this (JSP-6, JSC-238).
            if ((element.Flags & JsOpcodes.ElementIsNamedValue) != 0 &&
                value.IsObject && value.AsObject() is JsScriptFunction anonymous)
            {
                var label = element.Key.IsSymbol
                    ? SymbolFunctionName(element.Key.AsSymbol())
                    : element.Key.AsString();

                anonymous.FunctionName = label;

                anonymous.SetOwnProperty(
                    "name", JsProperty.Data(JsValue.String(label), JsPropertyAttributes.Configurable));
            }

            if ((element.Flags & JsOpcodes.ElementIsPrivate) != 0)
            {
                var name = element.Key.AsSymbol();

                // A FIELD DECLARED TWICE ON ONE OBJECT IS A TypeError AND NOT A SECOND WRITE. It is
                // reachable without a duplicate in the source: `class C { #x = 1 }` whose
                // constructor returns an object it has already constructed would install `#x` on it
                // twice, and the language says the second attempt fails.
                if (target.HasPrivate(name))
                {
                    ThrowTypeError(
                        "Cannot initialize " + name.Description + " twice on the same object");
                }

                target.SetPrivate(
                    name, JsProperty.Data(value, JsPropertyAttributes.Writable));

                continue;
            }

            if (element.Key.IsSymbol)
            {
                DefineOwnSymbolDataProperty(target, element.Key.AsSymbol(), value);
                continue;
            }

            // A FIELD IS CreateDataPropertyOrThrow AND NOT AN ASSIGNMENT, which is what makes a
            // field shadow an inherited SETTER of the same name rather than calling it. An
            // assignment would have run the setter and defined nothing.
            DefineOwnDataProperty(target, ToPropertyKey(element.Key), value);
        }
    }

    /// <summary>Defines one own data property, refusing where the object already refuses.</summary>
    /// <remarks>
    /// <b>It is <c>CreateDataPropertyOrThrow</c> and not <c>SetOwnProperty</c>.</b> A field lands
    /// on an instance the class body has never seen frozen, so the refusal is rare - but it is
    /// reachable: a derived constructor may return a frozen object, and a class whose fields then
    /// fail to define must say so rather than produce an instance missing them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=26D8A1
    // Broiler-Human:        PENDING
    private void DefineOwnDataProperty(JsObject target, string key, JsValue value)
    {
        // A PROXY IS ASKED ONLY ITS defineProperty TRAP. CreateDataPropertyOrThrow is one
        // [[DefineOwnProperty]] call, so reading the standing property or the extensibility first
        // would run getOwnPropertyDescriptor and isExtensible traps the language never calls. The
        // proxy's own define refuses with a TypeError when the trap does.
        if (target is JsProxy)
        {
            target.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
            return;
        }

        // A TYPED ARRAY'S NUMERIC KEY IS ITS OWN [[DefineOwnProperty]], which converts the value
        // with the engine - ToNumber, or ToBigInt for a BigInt kind - and refuses a key that names
        // no element. Storing it through SetOwnProperty would convert without the engine, which
        // cannot run a `valueOf` and cannot refuse a Number bound for a BigInt element.
        // (JSeal B07, 2026-09-22.)
        if (target is JsTypedArray && JsTypedArray.IsNumericKey(key))
        {
            var fields = new JsRealm.ObjectDescriptorFields
            {
                HasValue = true,
                Value = value,
                HasWritable = true,
                Writable = true,
                HasEnumerable = true,
                Enumerable = true,
                HasConfigurable = true,
                Configurable = true,
            };

            if (!JsRealm.ObjectDefineOwn(this, target, JsValue.String(key), fields))
            {
                ThrowTypeError("Cannot define property " + key + " on a typed array");
            }

            return;
        }

        if (target.TryGetOwnProperty(key, out var standing)
            ? !standing.Configurable
            : !target.Extensible)
        {
            ThrowTypeError("Cannot define property " + key + ", object is not extensible");
        }

        target.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>
    /// <see cref="DefineOwnDataProperty"/> for a Symbol key, with the same refusal.
    /// </summary>
    /// <remarks>
    /// A class field named by a computed Symbol is <c>CreateDataPropertyOrThrow</c> exactly as a
    /// String-named one is, so an instance a derived constructor froze refuses it the same way
    /// rather than gaining a Symbol-keyed property it could not otherwise be given.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=CEB03F
    // Broiler-Human:        PENDING
    private void DefineOwnSymbolDataProperty(JsObject target, JsSymbol key, JsValue value)
    {
        // A PROXY IS ASKED ONLY ITS defineProperty TRAP, for the reason the String twin gives.
        if (target is JsProxy)
        {
            target.SetOwnSymbol(key, JsProperty.Data(value, JsPropertyAttributes.Default));
            return;
        }

        if (target.TryGetOwnSymbol(key, out var standing)
            ? !standing.Configurable
            : !target.Extensible)
        {
            ThrowTypeError("Cannot define property " + key.Rendered + ", object is not extensible");
        }

        target.SetOwnSymbol(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>The private element one recorded method or accessor installs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=65257B
    // Broiler-Human:        PENDING
    private static JsProperty PrivateElementOf(JsClassElement element)
    {
        var accessor = element.Flags & (JsOpcodes.ElementIsGetter | JsOpcodes.ElementIsSetter);

        if (accessor == 0)
        {
            // NOT WRITABLE, which is what makes `this.#m = 1` a TypeError rather than a
            // replacement of the class's own method on one instance.
            return JsProperty.Data(element.Body, JsPropertyAttributes.None);
        }

        return JsProperty.Accessor(
            element.Body.AsObjectOrNull(),
            element.Setter.AsObjectOrNull(),
            JsPropertyAttributes.None);
    }

    /// <summary>Reads one private element, or says why there is none to read.</summary>
    /// <remarks>
    /// <b>The refusal names the private name and not the object</b>, because the object is usually
    /// the answer's subject rather than its cause: a method extracted from a class and called
    /// against something else meets this, and so does a brand check written as a read. Both are
    /// the same fact - this object was not constructed by the class that minted this name.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=801E16
    // Broiler-Human:        PENDING
    private JsValue ReadPrivate(JsValue host, JsSymbol name)
    {
        if (!host.IsObject || !host.AsObject().TryGetPrivate(name, out var element))
        {
            return ThrowTypeError(
                "Cannot read private member " + name.Description +
                " from an object whose class did not declare it");
        }

        if (!element.IsAccessor)
        {
            return element.Value;
        }

        // A WRITE-ONLY PRIVATE ACCESSOR IS A TypeError WHEN READ, and not `undefined`. `set #a` on
        // its own declares a name with no getter, and the language refuses the read rather than
        // answering the absence the way a property with no getter does.
        return element.Getter is null
            ? ThrowTypeError("'" + name.Description + "' was defined without a getter")
            : Call(JsValue.Object(element.Getter), host, []);
    }

    /// <summary>Writes one private element, or says why it cannot be written.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D2D724
    // Broiler-Human:        PENDING
    private void WritePrivate(JsValue host, JsSymbol name, JsValue value)
    {
        if (!host.IsObject || !host.AsObject().TryGetPrivate(name, out var element))
        {
            ThrowTypeError(
                "Cannot write private member " + name.Description +
                " to an object whose class did not declare it");

            return;
        }

        if (element.IsAccessor)
        {
            if (element.Setter is null)
            {
                ThrowTypeError("'" + name.Description + "' was defined without a setter");
                return;
            }

            Call(JsValue.Object(element.Setter), host, [value]);
            return;
        }

        if (!element.Writable)
        {
            ThrowTypeError("Cannot write to private method " + name.Description);
            return;
        }

        host.AsObject().SetPrivate(name, JsProperty.Data(value, JsPropertyAttributes.Writable));
    }

    /// <summary>Runs a <c>super()</c>: constructs the superclass and binds the result as <c>this</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3A0C9B
    // Broiler-Human:        PENDING
    private JsValue SuperConstruct(JsScriptFunction? active, JsCell? binding, JsValue newTarget, JsValue[] arguments)
    {
        if (active is null || binding is null)
        {
            throw new JsAbort(JsAbortKind.InternalDefect, "a super call reached a frame with no this binding");
        }

        var parent = active.Prototype;

        if (parent is null || !parent.IsConstructor)
        {
            return ThrowTypeError(
                "Super constructor of " + active.FunctionName + " is not a constructor");
        }

        var constructed = Construct(JsValue.Object(parent), arguments, newTarget);

        // CALLING `super()` TWICE IS AN ERROR AND NOT A SECOND BINDING, and the check happens
        // AFTER the superclass has run rather than before. That order is the specification's and
        // it is observable: a second `super()` still constructs the superclass, with whatever the
        // superclass constructor does, and only the attempt to bind the result fails. Checking
        // first would have been the obvious encoding and it makes the superclass's side effects
        // disappear.
        if (!binding.Value.IsEmpty)
        {
            return ThrowReferenceError("Super constructor may only be called once");
        }

        binding.Value = constructed;

        // THE DERIVED CLASS'S OWN FIELDS GO ON HERE AND NOT IN ITS CONSTRUCTOR'S PROLOGUE. The
        // object did not exist until this instant - the BASE constructor made it - so this is the
        // first point at which a derived class has something to install its fields on, and it is
        // also why a field initialiser may read `this` while the constructor's first line may not.
        if (constructed.IsObject)
        {
            InitialiseInstanceElements(constructed.AsObject(), active);
        }

        return constructed;
    }

    /// <summary>The object a <c>super</c> lookup starts from.</summary>
    /// <remarks>
    /// <para>
    /// It is the home object's PROTOTYPE, and the receiver of the lookup is <c>this</c> - the pair
    /// that makes <c>super.m()</c> reach the parent's <c>m</c> and run it against the instance.
    /// Starting at the receiver's prototype instead would find the method itself and recur
    /// forever, which is the defect this design exists to make unrepresentable.
    /// </para>
    /// <para>
    /// <b>A home object with no prototype is a TypeError and not an <c>undefined</c>.</b> The
    /// specification requires the base to be object-coercible before it reads anything through it,
    /// so <c>super.x</c> inside a method of an object whose prototype is <c>null</c> fails the way
    /// <c>null.x</c> fails rather than quietly answering nothing.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=25892B
    // Broiler-Human:        PENDING
    private JsObject SuperBase(JsScriptFunction? active)
    {
        var home = active?.HomeObject;

        if (home is null)
        {
            ThrowTypeError("'super' keyword unexpected here");
        }

        if (home!.Prototype is null)
        {
            ThrowTypeError("Cannot read properties of null (reading a 'super' property)");
        }

        return home.Prototype!;
    }

    /// <summary>Writes a property through the active method's home object.</summary>
    /// <remarks>
    /// <para>
    /// The chain the write consults starts above the home object and the write itself lands on
    /// <c>this</c>: an inherited setter runs with <c>this</c> as its receiver, an inherited
    /// non-writable data property refuses the write, and anything else creates or replaces an own
    /// property of the instance rather than touching the prototype it was found on.
    /// </para>
    /// <para>
    /// <b>It is the receiver-aware <c>[[Set]]</c> every other write uses</b>, so a proxy on the
    /// chain is asked through its <c>set</c> trap, a typed array's element rule holds, and the
    /// refusal is a strict write's <c>TypeError</c>. Until 2026-10-04 it walked the chain itself and
    /// stepped past a proxy as an ordinary object, so the trap never ran (JSC-264).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F791D3
    // Broiler-Human:        PENDING
    private void SetSuper(JsObject start, JsValue receiver, string key, JsValue value, bool strict)
    {
        if (!SetWithReceiver(start, key, value, receiver) && strict)
        {
            ThrowTypeError("Cannot assign to read only property '" + key + "'");
        }
    }
}
