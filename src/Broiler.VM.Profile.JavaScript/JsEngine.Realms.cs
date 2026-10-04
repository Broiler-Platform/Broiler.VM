// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           4
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  3/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The engine's realms: the agent's Symbols they share, the realm that is running, the switch that
/// keeps it the running function's, and the realms a guest creates (JSD-0030 SR-1, SR-2 and SR-7).
/// </summary>
/// <remarks>
/// <para>
/// <b>One engine is one agent and may hold several realms.</b> The first is built with the engine;
/// every other is built by <c>$262.createRealm</c>, on the same engine, from the same surface set,
/// charged to the same allowance and sharing the agent's Symbols and its job queue (JSD-0030 D3, D6).
/// </para>
/// <para>
/// <b><see cref="Realm"/> is the running realm, and every frame and built-in sets it.</b> A function
/// carries the realm that was running when it was made (<see cref="JsFunction.Realm"/>); entering a
/// script function's frame, or a built-in, makes that realm the running one until it returns, however
/// it returns. So <c>engine.Realm</c> read anywhere is the specification's "current Realm Record",
/// which is the realm every intrinsic a step reaches for - an error's prototype, a wrapper's, the
/// global object of a sloppy call - has to come from.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=7BBE7E
// Broiler-Falsified-If: a function runs in a realm other than its own, or a built-in of one realm hands a guest of another its intrinsics
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{
    /// <summary>The well-known Symbols and the <c>Symbol.for</c> registry every realm here shares.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0F2665
    // Broiler-Human:        PENDING
    internal JsAgentSymbols Symbols { get; } = new();

    /// <summary>The running realm: the realm of the innermost frame or built-in that is running.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8D411A
    // Broiler-Human:        PENDING
    internal JsRealm Realm { get; private set; }

    /// <summary>The realm the engine built with itself, which an embedder's first view is of.</summary>
    /// <remarks>
    /// <b>It is read only where no frame is running</b>: when the engine is built, and when the
    /// embedder's view is first made. Anything a guest's code reaches reads <see cref="Realm"/>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=031184
    // Broiler-Human:        PENDING
    internal JsRealm FirstRealm { get; }

    /// <summary>Makes <paramref name="realm"/> the running realm, answering the one it replaces.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1D34FD
    // Broiler-Human:        PENDING
    internal JsRealm EnterRealm(JsRealm realm)
    {
        var outer = Realm;
        Realm = realm;
        return outer;
    }

    /// <summary>Makes <paramref name="outer"/>, which <see cref="EnterRealm"/> answered, running again.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A1E596
    // Broiler-Human:        PENDING
    internal void LeaveRealm(JsRealm outer) => Realm = outer;

    /// <summary>Calls a built-in of another realm, with that realm running while it runs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=905FB6
    // Broiler-Human:        PENDING
    private JsValue CallInRealm(JsNativeFunction native, JsValue thisValue, JsValue[] arguments)
    {
        var outer = EnterRealm(native.Realm!);

        try
        {
            return native.Call(this, thisValue, arguments);
        }
        finally
        {
            Realm = outer;
        }
    }

    /// <summary>Constructs through a built-in of another realm, with that realm running while it runs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=4731D8
    // Broiler-Human:        PENDING
    private JsValue ConstructInRealm(JsNativeFunction native, JsValue[] arguments, JsValue newTarget)
    {
        var outer = EnterRealm(native.Realm!);

        try
        {
            return native.Construct(this, arguments, newTarget);
        }
        finally
        {
            Realm = outer;
        }
    }

    /// <summary>
    /// The specification's <c>GetFunctionRealm</c>: a function's own realm, reached through any bound
    /// functions and proxies around it, and the running realm for anything else.
    /// </summary>
    /// <remarks>
    /// A revoked proxy has no realm and is a <c>TypeError</c> (7.3.24 step 4), which is what makes a
    /// construction whose <c>new.target</c> was revoked while its <c>prototype</c> was read throw
    /// rather than use an intrinsic (JSC-252).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=52CDF8
    // Broiler-Falsified-If: a function's realm is answered as any realm but the one running when it was made, or a revoked proxy answers a realm
    // Broiler-Human:        PENDING
    internal JsRealm FunctionRealm(JsValue constructor)
    {
        var current = constructor.AsObjectOrNull();

        while (current is not null)
        {
            Charge(1);

            switch (current)
            {
                case JsBoundFunction bound:
                    current = bound.Target;
                    break;

                case JsProxy proxy:
                    if (proxy.Handler is null)
                    {
                        ThrowTypeError("the new target is a revoked Proxy, which has no realm");
                    }

                    current = proxy.Target;
                    break;

                case JsFunction { Realm: { } realm }:
                    return realm;

                default:
                    return Realm;
            }
        }

        return Realm;
    }

    /// <summary>
    /// The specification's <c>GetPrototypeFromConstructor(newTarget, fallback)</c>, where
    /// <paramref name="fallback"/> is the running realm's intrinsic.
    /// </summary>
    /// <remarks>
    /// A <c>new.target</c> whose <c>prototype</c> is not an object answers the intrinsic of ITS
    /// realm, not of the realm running: <c>Reflect.construct(Array, [], C)</c> with <c>C</c> from
    /// another realm and <c>C.prototype = null</c> builds an Array of <c>C</c>'s realm. A
    /// <c>new.target</c> that is not an object - an internal construction - answers the fallback.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=B714F4
    // Broiler-Falsified-If: a new.target whose prototype is not an object answers an intrinsic of any realm but its own
    // Broiler-Human:        PENDING
    internal JsObject PrototypeFromConstructor(JsValue newTarget, JsObject fallback)
    {
        if (!newTarget.IsObject)
        {
            return fallback;
        }

        var wanted = GetProperty(newTarget, "prototype");

        if (wanted.IsObject)
        {
            return wanted.AsObject();
        }

        return FunctionRealm(newTarget).CounterpartOf(fallback, Realm);
    }

    /// <summary>
    /// <c>ToObject</c> as <paramref name="realm"/> would answer it: a primitive's wrapper with that
    /// realm's prototype, for a receiver bound by a sloppy function of another realm.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=885304
    // Broiler-Human:        PENDING
    private JsObject ToObjectIn(JsRealm realm, JsValue value)
    {
        if (ReferenceEquals(realm, Realm))
        {
            return ToObject(value);
        }

        var outer = EnterRealm(realm);

        try
        {
            return ToObject(value);
        }
        finally
        {
            Realm = outer;
        }
    }

    /// <summary>
    /// Builds a new realm on this engine, from its surface set, and answers it (JSD-0030 SR-7): what
    /// <c>$262.createRealm</c> does before it hands the new realm's <c>$262</c> back.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is charged what a realm was measured to cost</b>, admitted before anything is built, so a
    /// loop that creates realms meets the allowance rather than the process's memory
    /// (<see cref="RealmRetainedBytes"/>, <see cref="RealmFuel"/>).
    /// </para>
    /// <para>
    /// <b>An embedder's host surface is told</b>, through a view of the new realm that shares the
    /// engine's step window, exactly as it was told of the first; INTERPRETING.md's harness installs
    /// its <c>$262</c> members there. A surface that throws is the guest's <c>TypeError</c>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=28AC81
    // Broiler-Falsified-If: a created realm is built from a surface set other than the engine's, is not charged, or shares a global object or an intrinsic with another realm
    // Broiler-Human:        PENDING
    internal JsRealm CreateRealm()
    {
        RetainOrAbort(RealmRetainedBytes);
        Charge(RealmFuel);

        var realm = new JsRealm(this);

        if (HostSurface is { } surface && hostRealm is { } first)
        {
            new JsHostRealm(this, realm, first).AnnounceCreated(surface);
        }

        return realm;
    }

    /// <summary>The live bytes a created realm is retained at.</summary>
    /// <remarks>
    /// <b>Measured, and rounded up to a power of two</b>: on 2026-10-04, a realm built from every
    /// surface held 504,818 bytes of managed heap when created by <c>$262.createRealm</c> and 489,699
    /// as a ShadowRealm's, averaged over a hundred held at once (JSD-0040 section 5). The first charge,
    /// 262,144 - what an instantiation reports for its first realm - was half that (JSC-265).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=11F557
    // Broiler-Human:        PENDING
    internal const ulong RealmRetainedBytes = 524_288;

    /// <summary>The fuel a realm's construction is charged, in proportion to the intrinsics it builds.</summary>
    /// <remarks>
    /// <b>Measured as time</b>: a realm took about 2 ms to build on 2026-10-04, which is what the
    /// interpreter spends on about 37,000 units of fuel on the same machine; this is the power of two
    /// below it. The first charge, 4,096, was a ninth of that (JSC-265).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7A7367
    // Broiler-Human:        PENDING
    private const ulong RealmFuel = 32_768;

    /// <summary>
    /// The host surface the composition installed, told of every realm the engine creates after its
    /// first; nothing where none was installed.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1A4010
    // Broiler-Human:        PENDING
    internal IJsHostSurface? HostSurface { get; set; }
}
