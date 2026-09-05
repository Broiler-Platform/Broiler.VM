// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// A Proxy: the one exotic object whose internal methods are guest code, with the invariants that
/// keep the rest of the language true anyway.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every other exotic object in this profile overrides an internal method to answer FASTER or
/// DIFFERENTLY; this one overrides them to answer ELSEWHERE.</b> <see cref="JsArray"/> keeps its
/// elements in a vector, <see cref="JsTypedArray"/> reads a buffer — both compute an answer the
/// engine could have computed itself. A Proxy calls a function the program wrote, which may throw,
/// may re-enter, may itself be a Proxy, and may lie. That last one is why over half of this file is
/// checks rather than forwarding: a Proxy that simply relayed each trap's answer would pass every
/// easy test and quietly break <c>Object.freeze</c>, <c>const</c>-like non-writability and the
/// prototype chain for everything downstream of it.
/// </para>
/// <para>
/// <b>The invariants are about the TARGET and not about the handler.</b> The rule in every case is
/// the same: whatever the target has committed to — a non-configurable property, a non-writable
/// value, a closed extensibility — the proxy may not contradict, because code that already checked
/// the target is entitled to keep believing what it read. A handler is free to invent anything the
/// target has not committed to, which is nearly everything, and free to invent nothing at all: a
/// missing trap forwards.
/// </para>
/// <para>
/// <b>Why the realm is held rather than passed.</b> Thirteen internal methods all run guest code,
/// so all thirteen need an engine — but nine of them are reached through
/// <see cref="JsObject"/> virtuals whose signatures the whole profile shares and which have no
/// engine to give. The alternative was an engine parameter on every property operation in the
/// object model for the sake of one object kind. A Proxy is always made by a realm, so it carries
/// the one it was made by; <see cref="JsRealm.Engine"/> is the engine that realm runs on.
/// </para>
/// <para>
/// <b>The four whole-chain methods are NOT here, and that is deliberate.</b> <c>[[Get]]</c>,
/// <c>[[Set]]</c>, <c>[[HasProperty]]</c>, <c>[[Call]]</c> and <c>[[Construct]]</c> are operations
/// over a whole prototype chain or a whole call, and the engine owns those walks — a proxy in the
/// middle of a chain must swallow the REST of the walk rather than answer one link of it. They are
/// invoked from <see cref="JsEngine"/> at the points where the language already puts them, and the
/// bodies they call are the <c>Proxy…</c> methods below.
/// </para>
/// <para>
/// <b>A revoked proxy keeps whether it was callable and loses everything else.</b> The
/// specification fixes the <c>[[Call]]</c> and <c>[[Construct]]</c> slots when the proxy is made,
/// so <c>typeof</c> a revoked proxy over a function is still <c>"function"</c> even though calling
/// it is a <c>TypeError</c>. Reading those two off the target would have made <c>typeof</c> throw,
/// and deriving them at revocation time is impossible because the target is gone.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
// Broiler-Human:        PENDING
internal sealed class JsProxy : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private readonly JsRealm realm;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private readonly bool callable;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private readonly bool constructor;

    /// <summary>Creates a proxy over <paramref name="target"/> answering through
    /// <paramref name="handler"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsProxy(JsRealm owner, JsObject target, JsObject handler)
        : base(null)
    {
        realm = owner;
        Target = target;
        Handler = handler;
        callable = target.IsCallable;
        constructor = target.IsConstructor;
    }

    /// <summary>The object the traps are told about, or <see langword="null"/> once revoked.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsObject? Target { get; private set; }

    /// <summary>The object the traps are read off, or <see langword="null"/> once revoked.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsObject? Handler { get; private set; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override bool IsCallable => callable;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override bool IsConstructor => constructor;

    /// <summary>The tag <c>Object.prototype.toString</c> reports, which is one of exactly three.</summary>
    /// <remarks>
    /// <para>
    /// <b>A Proxy is not a Date because its target is one.</b> The tag is derived from internal
    /// SLOTS, and a proxy has none of the ones that produce a tag — no <c>[[DateValue]]</c>, no
    /// <c>[[RegExpMatcher]]</c>, no <c>[[ErrorData]]</c> — so the language asks it only two
    /// questions: is it callable, and is it an Array. Copying the target's tag, which is what this
    /// did first, made <c>Object.prototype.toString.call(new Proxy(new Date(), {}))</c> answer
    /// <c>[object Date]</c> where every engine answers <c>[object Object]</c>.
    /// </para>
    /// <para>
    /// <b>It is derived on each read rather than fixed at creation</b>, because the Array question
    /// is <c>IsArray</c> and <c>IsArray</c> may THROW: a proxy over an already-revoked proxy has no
    /// target to look through, and the specification puts that refusal at the <c>toString</c> call
    /// rather than at the creation.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override string ClassName
    {
        get => callable ? "Function" : realm.ProxyArrayTag(this);
        set => _ = value;
    }

    /// <summary>Drops the target and the handler, which every internal method then refuses.</summary>
    /// <remarks>
    /// <b>Both are dropped and not just the handler</b>, because the target is the other half of
    /// what a revocation is for: a revoker exists so the object graph a program handed out can be
    /// released, and a proxy that kept its target alive would hold exactly the reference the
    /// revocation was meant to break.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal void Revoke()
    {
        Target = null;
        Handler = null;
    }

    /// <summary>
    /// <c>[[GetPrototypeOf]]</c> and <c>[[SetPrototypeOf]]</c>, which is what this virtual is.
    /// </summary>
    /// <remarks>
    /// <b>The setter cannot report a refusal, so it throws one.</b> The specification's
    /// <c>[[SetPrototypeOf]]</c> answers a boolean and only <c>Reflect.setPrototypeOf</c> wants it;
    /// that member reaches <see cref="ProxySetPrototypeOf"/> directly and reads the boolean, and
    /// every other route through this property is a route that owes a <c>TypeError</c> anyway.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override JsObject? Prototype
    {
        get => ProxyGetPrototypeOf();

        set
        {
            if (!ProxySetPrototypeOf(value))
            {
                Engine.ThrowTypeError("the 'setPrototypeOf' trap refused the prototype");
            }
        }
    }

    /// <summary><c>[[IsExtensible]]</c> and <c>[[PreventExtensions]]</c>.</summary>
    /// <remarks>
    /// <b>Assigning <see langword="true"/> is not an operation the language has</b> — nothing makes
    /// a non-extensible object extensible again — so the setter only ever means "prevent", and a
    /// <see langword="true"/> reaching it is the object model's own initialisation rather than a
    /// guest asking for anything.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override bool Extensible
    {
        get => ProxyIsExtensible();

        set
        {
            if (!value && !ProxyPreventExtensions())
            {
                Engine.ThrowTypeError("the 'preventExtensions' trap refused");
            }
        }
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override bool TryGetOwnProperty(string key, out JsProperty property) =>
        ProxyGetOwnProperty(JsValue.String(key), out property);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override bool TryGetOwnSymbol(JsSymbol key, out JsProperty property) =>
        ProxyGetOwnProperty(JsValue.Symbol(key), out property);

    /// <summary><c>[[DefineOwnProperty]]</c>, reached wherever the profile defines a property.</summary>
    /// <remarks>
    /// <b>It throws where the trap answers <see langword="false"/></b>, for the same reason
    /// <see cref="Prototype"/> does: this virtual returns nothing, and the two callers that want a
    /// boolean — <c>Reflect.defineProperty</c> and <c>Object.isFrozen</c>'s neighbours — already
    /// treat a refusal as a <c>JsThrow</c> to catch. Every other caller owes a <c>TypeError</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override void SetOwnProperty(string key, JsProperty property) =>
        DefineOrThrow(JsValue.String(key), property);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override void SetOwnSymbol(JsSymbol key, JsProperty property) =>
        DefineOrThrow(JsValue.Symbol(key), property);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override bool DeleteOwnProperty(string key) => ProxyDelete(JsValue.String(key));

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override bool DeleteOwnSymbol(JsSymbol key) => ProxyDelete(JsValue.Symbol(key));

    /// <summary>The String half of <c>[[OwnPropertyKeys]]</c>.</summary>
    /// <remarks>
    /// <b>The order is the trap's and is NOT re-sorted.</b> An ordinary object reports its array
    /// indices first in numeric order because that is how the specification says an ordinary object
    /// enumerates; a Proxy reports what its trap returned, in that order, which is the one place in
    /// the language where <c>Object.keys</c> can answer <c>["1", "0"]</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override System.Collections.Generic.List<string> OwnPropertyNames()
    {
        var names = new System.Collections.Generic.List<string>();

        foreach (var key in ProxyOwnKeys())
        {
            if (!key.IsSymbol)
            {
                names.Add(key.AsString());
            }
        }

        return names;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override System.Collections.Generic.List<JsSymbol> OwnSymbolKeys()
    {
        var keys = new System.Collections.Generic.List<JsSymbol>();

        foreach (var key in ProxyOwnKeys())
        {
            if (key.IsSymbol)
            {
                keys.Add(key.AsSymbol());
            }
        }

        return keys;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override System.Collections.Generic.List<JsValue> OwnKeys() => ProxyOwnKeys();

    /// <summary>
    /// <c>[[OwnPropertyKeys]]</c>: the trap's list, checked against what the target has committed to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Three separate invariants live here and they answer three different lies.</b> A duplicate
    /// key would make one property enumerate twice, which no caller of this is written to survive.
    /// A missing non-configurable key would hide a property the target promised is permanent, so
    /// <c>Object.getOwnPropertyNames</c> would disagree with a <c>getOwnPropertyDescriptor</c> that
    /// still answers. And on a non-extensible target the list must be EXACTLY the target's, in
    /// content if not in order, because a key that is not there cannot appear later and a key that
    /// is there cannot go away.
    /// </para>
    /// <para>
    /// <b>The target's keys are read once and its descriptors once each</b>, which matters because
    /// the target may itself be a Proxy: each of those reads is a trap call the specification counts.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsValue> ProxyOwnKeys()
    {
        var target = TrapTarget();

        if (!TryTrap("ownKeys", out var trap))
        {
            return target.OwnKeys();
        }

        var reported = Engine.ProxyKeyList(
            Engine.Call(trap, JsValue.Object(Handler!), [JsValue.Object(target)]));

        // THE DUPLICATE CHECK IS FIRST AND IS NOT AN OPTIMISATION. Everything below removes keys
        // from a working copy of this list, and a duplicate would let one target key satisfy two
        // removals - so a handler could hide a second, real key behind a repeat of the first.
        for (var at = 0; at < reported.Count; at++)
        {
            Engine.Charge(1);

            for (var other = at + 1; other < reported.Count; other++)
            {
                if (ProxySameKey(reported[at], reported[other]))
                {
                    Engine.ThrowTypeError("the 'ownKeys' trap reported a duplicate key");
                }
            }
        }

        var extensible = target.Extensible;
        var settled = new System.Collections.Generic.List<JsValue>();
        var free = new System.Collections.Generic.List<JsValue>();

        foreach (var key in target.OwnKeys())
        {
            Engine.Charge(1);

            var held = key.IsSymbol
                ? target.TryGetOwnSymbol(key.AsSymbol(), out var owned)
                : target.TryGetOwnProperty(key.AsString(), out owned);

            if (held && !owned.Configurable)
            {
                settled.Add(key);
            }
            else
            {
                free.Add(key);
            }
        }

        if (extensible && settled.Count == 0)
        {
            return reported;
        }

        var unaccounted = new System.Collections.Generic.List<JsValue>(reported);

        foreach (var key in settled)
        {
            Engine.Charge(1);

            if (!ProxyRemoveKey(unaccounted, key))
            {
                Engine.ThrowTypeError(
                    "the 'ownKeys' trap omitted a non-configurable own key of the target");
            }
        }

        if (extensible)
        {
            return reported;
        }

        foreach (var key in free)
        {
            Engine.Charge(1);

            if (!ProxyRemoveKey(unaccounted, key))
            {
                Engine.ThrowTypeError(
                    "the 'ownKeys' trap omitted an own key of a non-extensible target");
            }
        }

        if (unaccounted.Count != 0)
        {
            Engine.ThrowTypeError(
                "the 'ownKeys' trap reported a key a non-extensible target does not have");
        }

        return reported;
    }

    /// <summary><c>[[GetPrototypeOf]]</c>.</summary>
    /// <remarks>
    /// <b>The invariant only bites on a non-extensible target</b>, and that is the whole shape of
    /// it: while a target may still gain a prototype the proxy is free to name a different one,
    /// because nothing downstream could have relied on the answer. Once the target is closed its
    /// prototype can never change again, so an answer that differed would be a permanent lie.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsObject? ProxyGetPrototypeOf()
    {
        var target = TrapTarget();

        if (!TryTrap("getPrototypeOf", out var trap))
        {
            return target.Prototype;
        }

        var answer = Engine.Call(trap, JsValue.Object(Handler!), [JsValue.Object(target)]);

        if (!answer.IsObject && answer.Type != JsType.Null)
        {
            Engine.ThrowTypeError("the 'getPrototypeOf' trap answered neither an object nor null");
        }

        if (target.Extensible)
        {
            return answer.AsObjectOrNull();
        }

        if (!ReferenceEquals(answer.AsObjectOrNull(), target.Prototype))
        {
            Engine.ThrowTypeError(
                "the 'getPrototypeOf' trap disagreed with a non-extensible target's prototype");
        }

        return answer.AsObjectOrNull();
    }

    /// <summary><c>[[SetPrototypeOf]]</c>, with the boolean <c>Reflect.setPrototypeOf</c> wants.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal bool ProxySetPrototypeOf(JsObject? wanted)
    {
        var target = TrapTarget();

        if (!TryTrap("setPrototypeOf", out var trap))
        {
            // THE SAME FORWARDING DISTINCTION THE DEFINE BELOW MAKES: `OrdinarySetPrototypeOf`
            // REFUSES a change on a non-extensible object, which a bare assignment would let
            // through, and it answers a boolean an inner proxy can answer too.
            return target is JsProxy inner
                ? inner.ProxySetPrototypeOf(wanted)
                : JsRealm.ObjectSetPrototypeOrdinary(Engine, target, wanted);
        }

        var answered = Engine.Call(
            trap,
            JsValue.Object(Handler!),
            [JsValue.Object(target), wanted is null ? JsValue.Null : JsValue.Object(wanted)]);

        if (!answered.ToBooleanValue())
        {
            return false;
        }

        if (target.Extensible)
        {
            return true;
        }

        if (!ReferenceEquals(wanted, target.Prototype))
        {
            Engine.ThrowTypeError(
                "the 'setPrototypeOf' trap moved the prototype of a non-extensible target");
        }

        return true;
    }

    /// <summary><c>[[IsExtensible]]</c>.</summary>
    /// <remarks>
    /// <b>This is the one trap with no freedom at all.</b> Its answer must equal the target's, so
    /// the only thing a handler may do with it is observe the question — which is exactly what the
    /// specification intends, because extensibility is the hinge every other invariant here turns
    /// on and a proxy that could lie about it could lie about all of them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal bool ProxyIsExtensible()
    {
        var target = TrapTarget();

        if (!TryTrap("isExtensible", out var trap))
        {
            return target.Extensible;
        }

        var answered = Engine.Call(trap, JsValue.Object(Handler!), [JsValue.Object(target)])
            .ToBooleanValue();

        if (answered != target.Extensible)
        {
            Engine.ThrowTypeError(
                "the 'isExtensible' trap disagreed with the target's extensibility");
        }

        return answered;
    }

    /// <summary><c>[[PreventExtensions]]</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal bool ProxyPreventExtensions()
    {
        var target = TrapTarget();

        if (!TryTrap("preventExtensions", out var trap))
        {
            // AN INNER PROXY IS ASKED AND NOT ASSIGNED TO, because its own answer is a boolean and
            // the setter that would carry it has to throw instead. `OrdinaryPreventExtensions`
            // cannot fail, so the assignment is the whole of it for everything else.
            if (target is JsProxy inner)
            {
                return inner.ProxyPreventExtensions();
            }

            target.Extensible = false;
            return true;
        }

        var answered = Engine.Call(trap, JsValue.Object(Handler!), [JsValue.Object(target)])
            .ToBooleanValue();

        // A `true` HERE IS A CLAIM THE TARGET HAS TO BEAR OUT. Reporting success while leaving the
        // target extensible would let a later `Object.isExtensible` on the same target contradict
        // the `Object.preventExtensions` that just returned; a `false` claims nothing and is
        // therefore never checked.
        if (answered && target.Extensible)
        {
            Engine.ThrowTypeError(
                "the 'preventExtensions' trap reported success on a still-extensible target");
        }

        return answered;
    }

    /// <summary><c>[[GetOwnProperty]]</c>, for a String or a Symbol key alike.</summary>
    /// <remarks>
    /// <para>
    /// <b>A trap that answers <c>undefined</c> is making a claim, not declining to answer.</b> It
    /// claims the property is absent, and that claim is refused for a property the target holds
    /// non-configurably — such a property can never be deleted, so "absent" can never become true —
    /// and refused again on a non-extensible target, where a property that exists cannot go away.
    /// </para>
    /// <para>
    /// <b>The reported descriptor is COMPLETED before it is checked.</b> A handler answering
    /// <c>{ value: 1 }</c> has said <c>writable: false, enumerable: false, configurable: false</c>
    /// by omission, and the last of those is what makes the check below bite; treating an absent
    /// field as "no opinion" would let every invariant be skipped by leaving it out.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal bool ProxyGetOwnProperty(JsValue key, out JsProperty property)
    {
        property = default;
        var target = TrapTarget();

        if (!TryTrap("getOwnPropertyDescriptor", out var trap))
        {
            return key.IsSymbol
                ? target.TryGetOwnSymbol(key.AsSymbol(), out property)
                : target.TryGetOwnProperty(key.AsString(), out property);
        }

        var answered = Engine.Call(
            trap, JsValue.Object(Handler!), [JsValue.Object(target), key]);

        if (!answered.IsObject && answered.Type != JsType.Undefined)
        {
            Engine.ThrowTypeError(
                "the 'getOwnPropertyDescriptor' trap answered neither an object nor undefined");
        }

        var held = key.IsSymbol
            ? target.TryGetOwnSymbol(key.AsSymbol(), out var owned)
            : target.TryGetOwnProperty(key.AsString(), out owned);

        if (!answered.IsObject)
        {
            if (!held)
            {
                return false;
            }

            if (!owned.Configurable)
            {
                Engine.ThrowTypeError(
                    "the 'getOwnPropertyDescriptor' trap reported a non-configurable own property " +
                    "of the target as absent");
            }

            if (!target.Extensible)
            {
                Engine.ThrowTypeError(
                    "the 'getOwnPropertyDescriptor' trap reported an own property of a " +
                    "non-extensible target as absent");
            }

            return false;
        }

        var extensible = target.Extensible;
        var fields = CompletedFields(JsRealm.DescriptorFieldsOf(Engine, answered));
        var reported = CompletedProperty(fields);

        if (!ProxyCompatible(extensible, fields, held, owned))
        {
            Engine.ThrowTypeError(
                "the 'getOwnPropertyDescriptor' trap reported a descriptor the target cannot bear");
        }

        if (!reported.Configurable)
        {
            if (!held || owned.Configurable)
            {
                Engine.ThrowTypeError(
                    "the 'getOwnPropertyDescriptor' trap reported a non-configurable property the " +
                    "target does not hold non-configurably");
            }

            if (!reported.IsAccessor && !reported.Writable && !owned.IsAccessor && owned.Writable)
            {
                Engine.ThrowTypeError(
                    "the 'getOwnPropertyDescriptor' trap reported a writable target property as " +
                    "non-writable and non-configurable");
            }
        }

        property = reported;
        return true;
    }

    /// <summary><c>[[DefineOwnProperty]]</c>, answering the boolean the specification gives it.</summary>
    /// <remarks>
    /// <b>The descriptor handed to the trap is the one the CALLER wrote and not a completed one</b>,
    /// which is why <c>Object.defineProperty(p, "x", { value: 1 })</c> gives the trap an object with
    /// one key. The distinction is observable — a handler that forwards to <c>Reflect</c> would
    /// otherwise turn a partial redefinition into a total one — so the fields are carried through
    /// with their presence flags intact rather than being expanded into six.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal bool ProxyDefineOwnProperty(JsValue key, JsRealm.ObjectDescriptorFields fields)
    {
        var target = TrapTarget();

        if (!TryTrap("defineProperty", out var trap))
        {
            // A MISSING TRAP FORWARDS THE INTERNAL METHOD AND NOT AN UNCHECKED WRITE. Both halves
            // of that matter: the ordinary form VALIDATES, so a proxy over a frozen object refuses
            // a redefinition the way the object itself would; and it ANSWERS A BOOLEAN, so an inner
            // proxy's `false` reaches `Reflect.defineProperty` as `false` rather than as the
            // `TypeError` a property setter would have had to throw. The caller's partial
            // descriptor goes through intact either way.
            return target is JsProxy inner
                ? inner.ProxyDefineOwnProperty(key, fields)
                : JsRealm.ObjectDefineOrdinary(Engine, target, key, fields);
        }

        var answered = Engine.Call(
            trap,
            JsValue.Object(Handler!),
            [JsValue.Object(target), key, JsValue.Object(realm.DescriptorObjectOfFields(fields))]);

        if (!answered.ToBooleanValue())
        {
            return false;
        }

        var held = key.IsSymbol
            ? target.TryGetOwnSymbol(key.AsSymbol(), out var owned)
            : target.TryGetOwnProperty(key.AsString(), out owned);

        var extensible = target.Extensible;
        var settling = fields.HasConfigurable && !fields.Configurable;

        if (!held)
        {
            if (!extensible)
            {
                Engine.ThrowTypeError(
                    "the 'defineProperty' trap added a property to a non-extensible target");
            }

            if (settling)
            {
                Engine.ThrowTypeError(
                    "the 'defineProperty' trap reported a non-configurable property the target " +
                    "does not have");
            }

            return true;
        }

        if (!ProxyCompatible(extensible, fields, true, owned))
        {
            Engine.ThrowTypeError(
                "the 'defineProperty' trap accepted a descriptor the target cannot bear");
        }

        if (settling && owned.Configurable)
        {
            Engine.ThrowTypeError(
                "the 'defineProperty' trap made a configurable target property non-configurable");
        }

        // THE LAST CLAUSE IS THE ONE THAT LOOKS REDUNDANT AND IS NOT. A target property that is
        // non-configurable but still WRITABLE may have its value changed forever; reporting it as
        // non-writable would freeze, in the eyes of every reader of the proxy, a slot that the
        // target itself will keep letting through.
        if (!owned.IsAccessor && !owned.Configurable && owned.Writable &&
            fields.HasWritable && !fields.Writable)
        {
            Engine.ThrowTypeError(
                "the 'defineProperty' trap made a writable non-configurable target property " +
                "non-writable");
        }

        return true;
    }

    /// <summary><c>[[Delete]]</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal bool ProxyDelete(JsValue key)
    {
        var target = TrapTarget();

        if (!TryTrap("deleteProperty", out var trap))
        {
            return key.IsSymbol
                ? target.DeleteOwnSymbol(key.AsSymbol())
                : target.DeleteOwnProperty(key.AsString());
        }

        var answered = Engine.Call(
            trap, JsValue.Object(Handler!), [JsValue.Object(target), key]);

        if (!answered.ToBooleanValue())
        {
            return false;
        }

        var held = key.IsSymbol
            ? target.TryGetOwnSymbol(key.AsSymbol(), out var owned)
            : target.TryGetOwnProperty(key.AsString(), out owned);

        if (!held)
        {
            return true;
        }

        if (!owned.Configurable)
        {
            Engine.ThrowTypeError(
                "the 'deleteProperty' trap reported the deletion of a non-configurable property");
        }

        if (!target.Extensible)
        {
            Engine.ThrowTypeError(
                "the 'deleteProperty' trap reported the deletion of an own property of a " +
                "non-extensible target");
        }

        return true;
    }

    /// <summary><c>[[HasProperty]]</c>, which is the WHOLE chain and not one link of it.</summary>
    /// <remarks>
    /// <b>A <see langword="false"/> is the only answer that is checked</b>, because it is the only
    /// one that hides something: claiming a property the target does not have costs nothing, while
    /// denying one the target holds non-configurably contradicts a promise the target made.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal bool ProxyHas(JsValue key)
    {
        var target = TrapTarget();

        if (!TryTrap("has", out var trap))
        {
            return key.IsSymbol
                ? Engine.HasSymbol(target, key.AsSymbol())
                : Engine.HasProperty(target, key.AsString());
        }

        var answered = Engine.Call(
            trap, JsValue.Object(Handler!), [JsValue.Object(target), key]);

        if (answered.ToBooleanValue())
        {
            return true;
        }

        var held = key.IsSymbol
            ? target.TryGetOwnSymbol(key.AsSymbol(), out var owned)
            : target.TryGetOwnProperty(key.AsString(), out owned);

        if (!held)
        {
            return false;
        }

        if (!owned.Configurable)
        {
            Engine.ThrowTypeError("the 'has' trap denied a non-configurable own property");
        }

        if (!target.Extensible)
        {
            Engine.ThrowTypeError(
                "the 'has' trap denied an own property of a non-extensible target");
        }

        return false;
    }

    /// <summary><c>[[Get]]</c>, with the receiver the reference's base supplied.</summary>
    /// <remarks>
    /// <b>The invariant compares VALUES and not properties.</b> A target property that is neither
    /// configurable nor writable is a constant for the rest of the program's life, so a trap that
    /// answers something else has invented a second value for a slot the language promised has one.
    /// The accessor half is the same promise in the other shape: a non-configurable accessor with
    /// no getter reads <c>undefined</c> forever.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsValue ProxyGet(JsValue key, JsValue receiver)
    {
        var target = TrapTarget();

        if (!TryTrap("get", out var trap))
        {
            return key.IsSymbol
                ? Engine.GetSymbolWithReceiver(target, key.AsSymbol(), receiver)
                : Engine.GetWithReceiver(target, key.AsString(), receiver);
        }

        var answered = Engine.Call(
            trap, JsValue.Object(Handler!), [JsValue.Object(target), key, receiver]);

        var held = key.IsSymbol
            ? target.TryGetOwnSymbol(key.AsSymbol(), out var owned)
            : target.TryGetOwnProperty(key.AsString(), out owned);

        if (!held || owned.Configurable)
        {
            return answered;
        }

        if (!owned.IsAccessor && !owned.Writable && !JsRealm.SameValueOf(answered, owned.Value))
        {
            Engine.ThrowTypeError(
                "the 'get' trap answered a value other than the target's non-writable, " +
                "non-configurable property");
        }

        if (owned.IsAccessor && owned.Getter is null && answered.Type != JsType.Undefined)
        {
            Engine.ThrowTypeError(
                "the 'get' trap answered a value for a non-configurable accessor with no getter");
        }

        return answered;
    }

    /// <summary><c>[[Set]]</c>, answering whether the write took.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal bool ProxySet(JsValue key, JsValue value, JsValue receiver)
    {
        var target = TrapTarget();

        if (!TryTrap("set", out var trap))
        {
            return key.IsSymbol
                ? Engine.SetSymbolWithReceiver(target, key.AsSymbol(), value, receiver)
                : Engine.SetWithReceiver(target, key.AsString(), value, receiver);
        }

        var answered = Engine.Call(
            trap, JsValue.Object(Handler!), [JsValue.Object(target), key, value, receiver]);

        if (!answered.ToBooleanValue())
        {
            return false;
        }

        var held = key.IsSymbol
            ? target.TryGetOwnSymbol(key.AsSymbol(), out var owned)
            : target.TryGetOwnProperty(key.AsString(), out owned);

        if (!held || owned.Configurable)
        {
            return true;
        }

        if (!owned.IsAccessor && !owned.Writable && !JsRealm.SameValueOf(value, owned.Value))
        {
            Engine.ThrowTypeError(
                "the 'set' trap reported a write to the target's non-writable, non-configurable " +
                "property");
        }

        if (owned.IsAccessor && owned.Setter is null)
        {
            Engine.ThrowTypeError(
                "the 'set' trap reported a write to a non-configurable accessor with no setter");
        }

        return true;
    }

    /// <summary><c>[[Call]]</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsValue ProxyCall(JsValue thisValue, JsValue[] arguments)
    {
        var target = TrapTarget();

        if (!TryTrap("apply", out var trap))
        {
            return Engine.Call(JsValue.Object(target), thisValue, arguments);
        }

        return Engine.Call(
            trap,
            JsValue.Object(Handler!),
            [JsValue.Object(target), thisValue, JsValue.Object(realm.NewArray(arguments))]);
    }

    /// <summary><c>[[Construct]]</c>.</summary>
    /// <remarks>
    /// <b>The trap must answer an object, and that is the one invariant a construct has.</b> An
    /// ordinary constructor returning a primitive gets the instance it was given instead; a trap has
    /// no instance to fall back on, because the whole point of the trap is that none was made.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsValue ProxyConstruct(JsValue[] arguments, JsValue newTarget)
    {
        var target = TrapTarget();

        if (!TryTrap("construct", out var trap))
        {
            return Engine.Construct(JsValue.Object(target), arguments, newTarget);
        }

        var made = Engine.Call(
            trap,
            JsValue.Object(Handler!),
            [JsValue.Object(target), JsValue.Object(realm.NewArray(arguments)), newTarget]);

        if (!made.IsObject)
        {
            return Engine.ThrowTypeError("the 'construct' trap answered a value that is not an object");
        }

        return made;
    }

    /// <summary>The engine this proxy's traps run on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private JsEngine Engine => realm.Engine;

    /// <summary>The target, or the <c>TypeError</c> every internal method owes once revoked.</summary>
    /// <remarks>
    /// <b>It is the FIRST thing each internal method does</b>, before the handler is read and before
    /// any argument is coerced, because a revoked proxy has to be indistinguishable from a revoked
    /// proxy: an operation that coerced a key first would let a program tell which key it asked for.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private JsObject TrapTarget() =>
        Target ?? (JsObject)Engine.ThrowTypeError(
            "an operation was attempted on a revoked Proxy").AsObject();

    /// <summary>Reads one trap off the handler: absent, or callable, or a <c>TypeError</c>.</summary>
    /// <remarks>
    /// <b>Absent and <c>null</c> are the same thing here and everything else is not.</b> The
    /// specification's <c>GetMethod</c> treats both nullish values as "no trap" so that a handler
    /// may spell a declined trap either way, and refuses a present non-callable outright rather than
    /// silently forwarding — a handler with a misspelled function is a bug the program wants told
    /// about, not a proxy that quietly stops trapping.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private bool TryTrap(string name, out JsValue trap)
    {
        Engine.Charge(2);
        trap = Engine.GetProperty(JsValue.Object(Handler!), name);

        if (trap.IsNullish)
        {
            trap = JsValue.Undefined;
            return false;
        }

        if (!trap.IsObject || !trap.AsObject().IsCallable)
        {
            Engine.ThrowTypeError("the '" + name + "' trap is not a function");
        }

        return true;
    }

    /// <summary>Runs <c>[[DefineOwnProperty]]</c> from a virtual that cannot report a refusal.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private void DefineOrThrow(JsValue key, JsProperty property)
    {
        // THE PROPERTY IS TOTAL, so the descriptor built from it is too - which is right for every
        // caller that reaches a define through this virtual. Those are the profile's own writes:
        // `Object.freeze` re-stating a property with one bit changed, an intrinsic being built.
        // A program's PARTIAL descriptor never comes this way; `Object.defineProperty` and
        // `Reflect.defineProperty` carry their fields to `ProxyDefineOwnProperty` intact.
        var fields = new JsRealm.ObjectDescriptorFields
        {
            HasEnumerable = true,
            Enumerable = property.Enumerable,
            HasConfigurable = true,
            Configurable = property.Configurable,
        };

        if (property.IsAccessor)
        {
            fields.HasGet = true;
            fields.Getter = property.Getter;
            fields.HasSet = true;
            fields.Setter = property.Setter;
        }
        else
        {
            fields.HasValue = true;
            fields.Value = property.Value;
            fields.HasWritable = true;
            fields.Writable = property.Writable;
        }

        if (!ProxyDefineOwnProperty(key, fields))
        {
            Engine.ThrowTypeError("the 'defineProperty' trap refused the definition");
        }
    }

    /// <summary>The specification's <c>CompletePropertyDescriptor</c>, as presence flags.</summary>
    /// <remarks>
    /// <b>A field a trap left out is a field the trap SAID something about</b>, and what it said is
    /// the default: absent <c>configurable</c> means <c>false</c>. Only the descriptor a
    /// <c>getOwnPropertyDescriptor</c> trap answers with is completed this way — the one a program
    /// hands to <c>Object.defineProperty</c> is not, because there an absent field means "leave it
    /// alone". Conflating the two would make every invariant below skippable by omission.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private static JsRealm.ObjectDescriptorFields CompletedFields(
        JsRealm.ObjectDescriptorFields fields)
    {
        fields.HasEnumerable = true;
        fields.HasConfigurable = true;

        if (fields.HasGet || fields.HasSet)
        {
            fields.HasGet = true;
            fields.HasSet = true;
            return fields;
        }

        fields.HasValue = true;
        fields.HasWritable = true;
        return fields;
    }

    /// <summary>The specification's <c>CompletePropertyDescriptor</c> over the fields read.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private static JsProperty CompletedProperty(JsRealm.ObjectDescriptorFields fields)
    {
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
    /// The specification's <c>IsCompatiblePropertyDescriptor</c>: could the target hold this?
    /// </summary>
    /// <remarks>
    /// <b>It is <c>ValidateAndApplyPropertyDescriptor</c> asked as a question rather than performed
    /// as an action.</b> The neighbouring <c>ObjectApplyDescriptor</c> answers the same question by
    /// throwing where it would refuse and writing where it would not; here nothing is being written,
    /// so the refusals are enumerated and the write is not. Keeping the two in one function would
    /// have meant a validating path that has to be told not to store, which is the shape of thing
    /// that eventually stores.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private static bool ProxyCompatible(
        bool extensible, JsRealm.ObjectDescriptorFields fields, bool held, JsProperty current)
    {
        if (!held)
        {
            return extensible;
        }

        if (current.Configurable)
        {
            return true;
        }

        if (fields.HasConfigurable && fields.Configurable)
        {
            return false;
        }

        if (fields.HasEnumerable && fields.Enumerable != current.Enumerable)
        {
            return false;
        }

        var wantsAccessor = fields.HasGet || fields.HasSet;

        if (wantsAccessor != current.IsAccessor &&
            (wantsAccessor || fields.HasValue || fields.HasWritable))
        {
            return false;
        }

        if (current.IsAccessor)
        {
            return (!fields.HasGet || ReferenceEquals(fields.Getter, current.Getter)) &&
                (!fields.HasSet || ReferenceEquals(fields.Setter, current.Setter));
        }

        if (current.Writable)
        {
            return true;
        }

        return (!fields.HasWritable || !fields.Writable) &&
            (!fields.HasValue || JsRealm.SameValueOf(fields.Value, current.Value));
    }

    /// <summary>Whether two own keys are the same key, across the two key types.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private static bool ProxySameKey(JsValue left, JsValue right) =>
        left.IsSymbol
            ? right.IsSymbol && ReferenceEquals(left.AsSymbol(), right.AsSymbol())
            : !right.IsSymbol &&
                string.Equals(left.AsString(), right.AsString(), System.StringComparison.Ordinal);

    /// <summary>Removes the first occurrence of <paramref name="key"/>, and says whether it was there.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private static bool ProxyRemoveKey(
        System.Collections.Generic.List<JsValue> keys, JsValue key)
    {
        for (var at = 0; at < keys.Count; at++)
        {
            if (ProxySameKey(keys[at], key))
            {
                keys.RemoveAt(at);
                return true;
            }
        }

        return false;
    }
}
