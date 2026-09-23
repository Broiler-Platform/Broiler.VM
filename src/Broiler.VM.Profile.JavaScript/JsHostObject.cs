// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           4
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    High
// Criteria:         5/5
// Resource impact:  3/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// An object whose property lookup an embedder completes: a live collection, a style declaration,
/// a storage area.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is an ordinary object first and an exotic one second, and the order is the whole design.</b>
/// Every override here consults <c>base</c> before the handler, so a name the object's own storage
/// holds is answered from storage and the embedder is never asked about it. The specification's
/// named-property rule requires exactly that ranking, and getting it backwards is silently wrong
/// rather than loudly wrong: a collection that happens to contain an element named <c>item</c> would
/// begin shadowing its own <c>item()</c> method, and every ordinary use of the object would keep
/// working while that one name answered something else.
/// </para>
/// <para>
/// <b>It is a subclass rather than a proxy.</b> The realm has a complete <c>Proxy</c> with thirteen
/// traps and it would express this, at the cost of a property read on a handler object plus a call
/// for every operation - on the path a page uses most. A subclass overriding four members costs a
/// virtual call that the base already pays, and it needs no target object for a thing that has no
/// target.
/// </para>
/// <para>
/// The one existing exotic subclass in this profile, the String wrapper, consults its own slots
/// first and calls <c>base</c> last. That is correct there, because <c>length</c> and the index
/// keys genuinely are a String wrapper's own properties rather than named properties ranked beneath
/// them. The opposite order here is deliberate and is not drift.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=5BF8DC
// Broiler-Falsified-If: a handler is consulted for a name this object's own storage already holds
// Broiler-Human:        PENDING
internal sealed class JsHostObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=FB06BD
    // Broiler-Human:        PENDING
    private readonly JsHostRealm realm;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=294CC6
    // Broiler-Human:        PENDING
    private readonly IJsHostExotic handler;

    /// <summary>The handler's deletion half, where it declared one; asked once, at mint time.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6A00F9
    // Broiler-Human:        PENDING
    private readonly IJsHostExoticDeletion? deleter;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F9A3C7
    // Broiler-Human:        PENDING
    internal JsHostObject(JsObject? prototype, JsHostRealm owner, IJsHostExotic completion)
        : base(prototype)
    {
        realm = owner;
        handler = completion;
        deleter = completion as IJsHostExoticDeletion;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EF9B54
    // Broiler-Human:        PENDING
    internal override string ClassName => "Object";

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=0A2ACE
    // Broiler-Falsified-If: the handler answers a key the base found
    // Broiler-Human:        PENDING
    internal override bool TryGetOwnProperty(string key, out JsProperty property)
    {
        // BASE FIRST. Ordinary properties outrank named ones, and this line is the whole of that
        // guarantee: past it, the object's own storage did not hold the key and the embedder is the
        // only thing left that could know.
        if (base.TryGetOwnProperty(key, out property))
        {
            return true;
        }

        // A HOOK IS HOST CODE, AND WHAT IT RAISES IS TRANSLATED HERE. A handler that throws, is
        // refused at the seam, or answers a value this realm refuses - one another realm minted, a
        // BigInt the composition declined - reaches the guest as the value or the TypeError a host
        // body's would, rather than unwinding through the interpreter's frames (JSD-0024 section 19).
        // Each hook is also a charged, gated crossing, entered inside that translation; outside every
        // step - the engine rendering a value after the step closed - it is not asked (section 19.2).
        try
        {
            if (!realm.TryEnterHook())
            {
                property = default;
                return false;
            }

            return AskHandler(key, out property);
        }
        catch (System.Exception raised) when (IsHostRaised(raised))
        {
            throw realm.HookRaised(raised);
        }
    }

    /// <summary>Asks the handler about a key the object's own storage did not hold.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=979FBD
    // Broiler-Human:        PENDING
    private bool AskHandler(string key, out JsProperty property)
    {
        // AN INDEX KEY IS ASKED OF THE INDEXED HOOK AND NEVER OF THE NAMED ONE. The two are
        // different questions to an embedder - a collection's contents against its members' names -
        // and a key the engine has just formatted from a number should not have to be parsed back
        // out of a string on the other side of the boundary.
        if (IsArrayIndex(key, out var index))
        {
            if (!handler.TryGetIndex(realm, index, out var element))
            {
                property = default;
                return false;
            }

            property = JsProperty.Data(
                realm.Unwrap(element),
                JsPropertyAttributes.Enumerable | JsPropertyAttributes.Configurable);

            return true;
        }

        if (!handler.TryGetNamed(realm, key, out var answered))
        {
            property = default;
            return false;
        }

        // Enumerable and configurable, and NOT writable: a named property is the embedder's to
        // answer, so a guest writing through it would be writing into storage the embedder does not
        // read. An assignment therefore creates an ordinary own property that shadows it from then
        // on, which is the behaviour the base's own write path already produces.
        property = JsProperty.Data(
            realm.Unwrap(answered),
            JsPropertyAttributes.Enumerable | JsPropertyAttributes.Configurable);

        return true;
    }

    /// <summary>
    /// Takes a named assignment the embedder claims, and lets every other one through.
    /// </summary>
    /// <remarks>
    /// <b>The embedder is asked only about names its own storage does not already hold</b>, for the
    /// same reason the read path asks it only then: an ordinary property that exists outranks a
    /// named one, and a write that went to the embedder first would make an expando unreachable the
    /// moment the embedder happened to know its name. An index key is not offered at all - an
    /// assignment to a live collection's element is not a thing such a collection takes - and
    /// neither is an accessor definition, which is the realm installing a member rather than a
    /// guest assigning to one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=539B64
    // Broiler-Falsified-If: an assignment reaches the handler for a key this object's storage already holds
    // Broiler-Human:        PENDING
    internal override void SetOwnProperty(string key, JsProperty property)
    {
        if (!realm.Installing &&
            !base.TryGetOwnProperty(key, out _) &&
            !IsArrayIndex(key, out _) &&
            !property.IsAccessor &&
            OfferNamedWrite(key, property.Value))
        {
            return;
        }

        base.SetOwnProperty(key, property);
    }

    /// <summary>Offers one named assignment to the handler, translating what it raises.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=60F60B
    // Broiler-Falsified-If: an exception the handler raises unwinds through interpreter frames untranslated
    // Broiler-Human:        PENDING
    private bool OfferNamedWrite(string key, JsValue value)
    {
        var offered = realm.Wrap(value);

        try
        {
            return realm.TryEnterHook() && handler.TrySetNamed(realm, key, offered);
        }
        catch (System.Exception raised) when (IsHostRaised(raised))
        {
            throw realm.HookRaised(raised);
        }
    }

    /// <summary>
    /// Offers a named deletion to a handler that declared one, then deletes as any object does.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The offer comes first and the ordinary deletion runs regardless</b>, which is the order
    /// <see cref="IJsHostExoticDeletion"/> specifies: the item goes before the property mirroring
    /// it, and what <c>delete</c> answers stays the object's own answer rather than the handler's.
    /// The offer does not depend on what the object's storage holds, so a non-configurable own
    /// property is offered and then refused by the base, exactly as the JSeal contract's other
    /// provider does it.
    /// </para>
    /// <para>
    /// <b>This is the one door every deletion route reaches</b> - the <c>delete</c> operator,
    /// <c>Reflect.deleteProperty</c>, a trapless <c>Proxy</c>, and the host's own
    /// <c>DeleteProperty</c> - which is what makes "offered once" a property of this member rather
    /// than of each caller. A symbol-keyed deletion arrives at <c>DeleteOwnSymbol</c> instead and is
    /// never offered; an index key is filtered here, as the named write path filters it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=889091
    // Broiler-Falsified-If: an index key reaches the deletion hook, or the ordinary deletion is skipped after the hook returned
    // Broiler-Human:        PENDING
    internal override bool DeleteOwnProperty(string key)
    {
        if (deleter is not null && !IsArrayIndex(key, out _))
        {
            realm.OfferDeletion(deleter, key);
        }

        return base.DeleteOwnProperty(key);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=8EC6DD
    // Broiler-Human:        PENDING
    internal override System.Collections.Generic.List<string> OwnPropertyNames()
    {
        var names = base.OwnPropertyNames();
        System.Collections.Generic.IReadOnlyList<string> supported;
        uint length;

        try
        {
            if (!realm.TryEnterHook())
            {
                return names;
            }

            supported = handler.SupportedNames(realm);
            realm.TryEnterHook();
            length = handler.IndexedLength(realm);
        }
        catch (System.Exception raised) when (IsHostRaised(raised))
        {
            throw realm.HookRaised(raised);
        }

        // INDEX KEYS FIRST AND IN ORDER, because that is where the language puts them and because
        // an enumeration reporting a collection's named members before its elements would disagree
        // with every other object in the realm.
        for (var at = 0u; at < length; at++)
        {
            var key = JsNumberFormat.ToUintString(at);

            if (!names.Contains(key))
            {
                names.Insert((int)at, key);
            }
        }

        // A supported name the object's own storage already holds is NOT added again. The keys a
        // for-in or an Object.keys walks must be distinct, and a duplicate would make a spread
        // assign the same key twice and an enumeration report a length nothing else agrees with.
        for (var at = 0; at < supported.Count; at++)
        {
            var name = supported[at];

            if (!names.Contains(name))
            {
                names.Add(name);
            }
        }

        return names;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=042D25
    // Broiler-Human:        PENDING
    internal override int OwnPropertyCount => OwnPropertyNames().Count;

    /// <summary>
    /// Whether an exception is one of the three a host body may raise, which the realm translates.
    /// </summary>
    /// <remarks>
    /// A type test and nothing else, because it runs as an exception filter: a filter that threw
    /// would be taken as <c>false</c> and the exception would pass by untranslated.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=191ABE
    // Broiler-Human:        PENDING
    private static bool IsHostRaised(System.Exception raised) =>
        raised is JsHostThrowException or JsHostSurfaceException or JsHostTerminatedException;
}
