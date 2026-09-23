// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   80
// Annotated:        80/80
// Exempt:           12
// Human-reviewed:   0/80
// IP risk:          Low
// Security risk:    High
// Criteria:         41/41
// Resource impact:  6/10 max
// Unverified:       80
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// One realm, as an embedder holds it: the door through which host objects reach guest code.
/// </summary>
/// <remarks>
/// <para>
/// <b>This is not the core's host-capability boundary and it is not trying to be.</b> A capability
/// is how a guest asks a host for something the host owns - a line of output, a ruling on a module
/// specifier - and the core carries the request because the core is what a composition registered
/// against. A host object is a different shape entirely: it is an ordinary object in the realm, its
/// methods are ordinary functions, and calling one is <c>JsEngine.Call</c> from inside a built-in's
/// body, which is the same mechanism <c>Array.prototype.map</c> uses to call the function it was
/// given. Nothing about it crosses the core, and therefore nothing about it nests a core operation,
/// re-enters a runtime, or asks a lifecycle gate for permission it was not designed to give.
/// </para>
/// <para>
/// <b>What the capability table still decides is whether any of this exists.</b> A realm is handed
/// to an embedder only where the composition registered
/// <see cref="JavaScriptProfile.HostSurfaceCapability"/> and the registration answered
/// <c>Completed</c>. Registration is still the permission and there is still no other door; what
/// this design says is that the capability is the permission and not the channel, which is a
/// distinction the plan did not previously draw and which decision JSD-0024 argues in full.
/// </para>
/// <para>
/// <b>It is charged like a host call because it is one.</b> Every crossing charges
/// <c>HostCalls</c> one unit and a declared amount of <c>Fuel</c> proportional to what it carries,
/// so an embedder cannot buy unmetered work by moving it across the seam. That is the property the
/// capability channel gave for free and the one a seam has to pay for deliberately.
/// </para>
/// <para>
/// <b>It is valid only inside a step, on the guest's own thread.</b> A realm reached from anywhere
/// else has no meter to charge and no operation to fault, so what looks like working code would be
/// work nobody is paying for. Every member refuses with
/// <see cref="JsHostRefusal.RealmNotCurrent"/> rather than doing it anyway.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=88DEB7
// Broiler-Falsified-If: a crossing runs outside a step, on another thread, or without charging HostCalls
// Broiler-Human:        PENDING
public sealed class JsHostRealm
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3283F6
    // Broiler-Human:        PENDING
    private readonly JsEngine engine;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F929E7
    // Broiler-Human:        PENDING
    private readonly System.Runtime.CompilerServices.ConditionalWeakTable<JsObject, JsHostRef> objects = new();

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C9B66B
    // Broiler-Human:        PENDING
    private readonly System.Runtime.CompilerServices.ConditionalWeakTable<JsSymbol, JsHostRef> symbols = new();

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=543148
    // Broiler-Human:        PENDING
    private JsAbort? latched;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=07A129
    // Broiler-Human:        PENDING
    private int guestThreadId;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3E11D2
    // Broiler-Human:        PENDING
    private int stepDepth;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F05674
    // Broiler-Human:        PENDING
    internal JsHostRealm(JsEngine owner) => engine = owner;

    /// <summary>The realm's global object: what a script sees as <c>globalThis</c>.</summary>
    /// <remarks>
    /// It is an ordinary mutable object, and an embedder installing a name on it is doing exactly
    /// what the standard library's own setup does. There is no separate variable scope to keep in
    /// step: on this surface a top-level <c>var</c> becomes a property of this object, which is the
    /// property a browser-shaped embedder depends on when it recovers a frame's declarations.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6222D5
    // Broiler-Human:        PENDING
    public JsHostValue Global
    {
        get
        {
            Enter(1);
            return Wrap(JsValue.Object(engine.Realm.GlobalObject));
        }
    }

    /// <summary>
    /// Whether this realm may be touched right now: inside a step, on the thread the guest runs on.
    /// </summary>
    /// <remarks>
    /// <b>An embedder needs to be able to ASK rather than to find out by being refused.</b> Every
    /// other member answers <see cref="JsHostRefusal.RealmNotCurrent"/> outside the window, which is
    /// the right answer for a mistake and the wrong one for a decision: an embedder holding this
    /// realm between two invocations is not making a mistake, it is deciding whether to do the work
    /// now or to ask for a turn first. This is what lets it decide.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BB34FA
    // Broiler-Human:        PENDING
    public bool IsCurrent =>
        stepDepth > 0 && System.Environment.CurrentManagedThreadId == guestThreadId;

    // ---- the step bracket ----------------------------------------------------------------------

    /// <summary>Opens the window in which this realm may be touched.</summary>
    /// <remarks>
    /// Called by the profile around a step, on the thread the guest runs on. It is a depth rather
    /// than a flag because a step can contain a host call that contains a guest call that contains
    /// another host call, and the window closes when the outermost one does.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F54928
    // Broiler-Human:        PENDING
    internal void BeginStep()
    {
        guestThreadId = System.Environment.CurrentManagedThreadId;
        stepDepth++;
    }

    /// <summary>Closes the window, and answers an abort host code caught and discarded.</summary>
    /// <remarks>
    /// <para>
    /// <b>The answer is the point.</b> An embedder writing <c>catch (Exception)</c> around a guest
    /// call is ordinary defensive style, and without this it would turn a spent allowance into a
    /// completed operation. The abort is latched when it is converted, so catching
    /// <see cref="JsHostTerminatedException"/> buys an embedder the chance to release its own
    /// resources and buys it nothing else.
    /// </para>
    /// <para>
    /// <b>It answers rather than throws, and that is not a detail.</b> This runs in a
    /// <c>finally</c>, and an exception raised from a <c>finally</c> replaces whatever was already
    /// propagating - so a version of this that threw would discard the very abort it was trying to
    /// preserve whenever one was already on its way out. The caller re-raises what this answers,
    /// and only where nothing else is in flight.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=EF50C7
    // Broiler-Falsified-If: an operation whose allowance was spent completes because host code caught the abort
    // Broiler-Human:        PENDING
    internal JsAbort? EndStep()
    {
        stepDepth--;

        if (stepDepth > 0)
        {
            return null;
        }

        var abort = latched;
        latched = null;
        guestThreadId = 0;
        return abort;
    }

    // ---- values --------------------------------------------------------------------------------

    /// <summary>A new ordinary object with this realm's <c>Object.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=405CC0
    // Broiler-Human:        PENDING
    public JsHostValue NewObject()
    {
        Enter(4);

        try
        {
            return Wrap(JsValue.Object(new JsObject(engine.Realm.ObjectPrototype)));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>A new Array, optionally pre-filled.</summary>
    /// <exception cref="System.ArgumentException">
    /// An element is <see cref="JsHostValue.Missing"/> (JSD-0024 section 20): this member makes no holes, and
    /// nothing is created.
    /// </exception>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=28F4B9
    // Broiler-Human:        PENDING
    public JsHostValue NewArray(System.ReadOnlySpan<JsHostValue> elements = default)
    {
        Enter(4 + (ulong)elements.Length);
        RequireValues(elements, nameof(elements));

        try
        {
            var array = new JsArray(engine.Realm.ArrayPrototype);

            for (var at = 0; at < elements.Length; at++)
            {
                array.Push(Unwrap(elements[at]));
            }

            return Wrap(JsValue.Object(array));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// A new non-constructable host function: an operation or an accessor's body.
    /// </summary>
    /// <remarks>
    /// <b>Non-constructable is the default because most host functions are.</b> A method reached off
    /// an object is not something <c>new</c> applies to, and the interface-description languages an
    /// embedder is likely to be implementing say so outright. It is also what makes a host object
    /// affordable: a constructable function mints a <c>prototype</c> object and a
    /// <c>constructor</c> back-reference on it, and an object with a hundred members would mint a
    /// hundred unreachable pairs. Anything a guest may legitimately <c>new</c> asks for
    /// <see cref="NewConstructor"/> instead.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A9AE39
    // Broiler-Human:        PENDING
    public JsHostValue NewMethod(string name, JsHostFunction body, int length = 0)
    {
        Enter(6);

        if (body is null)
        {
            throw new System.ArgumentNullException(nameof(body));
        }

        try
        {
            return Wrap(JsValue.Object(engine.Realm.Native(
                name ?? string.Empty, length, Bind(body))));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// A new object with an <c>[[IsHTMLDDA]]</c> internal slot (ECMA-262 Annex B.3.6): a
    /// non-constructable function that answers <c>null</c> to every call.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It exists for a conformance host's <c>$262.IsHTMLDDA</c></b>, which the pinned suite's
    /// INTERPRETING.md defines as an object with the slot that answers <c>null</c> when called with
    /// no argument or with <c>""</c>. The annex's three changes follow from the slot and nothing
    /// else: <c>typeof</c> answers <c>"undefined"</c>, <c>ToBoolean</c> answers <c>false</c>, and
    /// <c>==</c> with <c>null</c> or <c>undefined</c> answers <c>true</c>. Everything else - <c>??</c>,
    /// optional chaining, default initialisers, <c>GetMethod</c>, strict equality - treats it as the
    /// object it is.
    /// </para>
    /// <para>
    /// <b>No guest can make one</b>, so an ordinary program meets one only where its embedder put it.
    /// It is a legacy of <c>document.all</c> and a web page host is the only other embedder with a
    /// reason to call this (JSD-0024 section 18).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AB6038
    // Broiler-Human:        PENDING
    public JsHostValue NewHtmlDdaObject()
    {
        Enter(6);

        try
        {
            var realm = engine.Realm;

            return Wrap(JsValue.Object(new JsNativeFunction(
                realm.FunctionPrototype, string.Empty, 0, static (_, _, _) => JsValue.Null)
            {
                EmulatesUndefined = true,
            }));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>A new constructable host function, carrying a readable <c>prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1BFB83
    // Broiler-Human:        PENDING
    public JsHostValue NewConstructor(string name, JsHostFunction body, int length = 0)
    {
        Enter(8);

        if (body is null)
        {
            throw new System.ArgumentNullException(nameof(body));
        }

        try
        {
            var prototype = new JsObject(engine.Realm.ObjectPrototype);

            // TWO BOUND BODIES, BECAUSE THE RECEIVER SLOT MEANS TWO THINGS. On a call it is the
            // receiver; on a construction the language has created no object yet and the engine
            // puts the new target there instead. Binding once would make an embedder's `this` the
            // constructor on every `new`, which is a wrong answer rather than a missing one.
            var function = new JsNativeFunction(
                engine.Realm.FunctionPrototype,
                name ?? string.Empty,
                length,
                Bind(body),
                BindConstructor(body));

            function.DefineBuiltIn("prototype", JsValue.Object(prototype));
            prototype.DefineBuiltIn("constructor", JsValue.Object(function));

            return Wrap(JsValue.Object(function));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// A new object whose property lookup this embedder completes.
    /// </summary>
    /// <remarks>
    /// The handler answers only what the object's own storage did not; see
    /// <see cref="IJsHostExotic"/> for why that order is the way round it is.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=38EC4F
    // Broiler-Human:        PENDING
    public JsHostValue NewExotic(IJsHostExotic handler)
    {
        Enter(6);

        if (handler is null)
        {
            throw new System.ArgumentNullException(nameof(handler));
        }

        try
        {
            return Wrap(JsValue.Object(new JsHostObject(engine.Realm.ObjectPrototype, this, handler)));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>An <c>Error</c> of the named kind, as a value: build it, then throw it.</summary>
    /// <remarks>
    /// It answers rather than throws so that a host body reads
    /// <c>throw realm.Error(kind, message)</c>. A helper that threw would leave the C# compiler
    /// believing control continued past it, and the reader unsure whether it did.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9F20FE
    // Broiler-Human:        PENDING
    public JsHostThrowException Error(JsHostErrorKind kind, string message)
    {
        Enter(6);

        try
        {
            var text = message ?? string.Empty;
            var error = engine.Realm.CreateError(NameOf(kind), text);
            return new JsHostThrowException(Wrap(error), text);
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>The exception to throw so the guest sees <paramref name="value"/> thrown.</summary>
    /// <remarks>
    /// <b>An embedder that already holds the value it wants thrown needs this, and <see cref="Error"/>
    /// is not it.</b> That one builds an error of a named kind; this one throws what the embedder
    /// has - an object it constructed through some other constructor the realm carries, or a value
    /// it caught and is re-raising. Without it, an embedder holding a guest value could only throw a
    /// CLR exception, which unwinds through interpreter frames as something no <c>catch</c> in the
    /// guest can see.
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="value"/> is <see cref="JsHostValue.Missing"/>, which is not a value (JSD-0024
    /// section 20); nothing is written or thrown into the realm.
    /// </exception>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=08BF34
    // Broiler-Human:        PENDING
    public JsHostThrowException Throw(JsHostValue value)
    {
        Enter(1);
        RequireValue(value, nameof(value));

        return new JsHostThrowException(value, "an exception the host raised");
    }

    /// <summary>The abstract operation <c>ToString</c>. May run guest code, and may throw.</summary>
    /// <remarks><see cref="JsHostValue.Missing"/> converts as <c>undefined</c> does (JSD-0024 section 20).</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=01C147
    // Broiler-Human:        PENDING
    public string ToJsString(JsHostValue value)
    {
        Enter(2);

        try
        {
            var text = engine.ToStringValue(Unwrap(value));
            engine.ChargeText(text.Length);
            return text;
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>The abstract operation <c>ToNumber</c>. May run guest code, and may throw.</summary>
    /// <remarks><see cref="JsHostValue.Missing"/> converts as <c>undefined</c> does (JSD-0024 section 20).</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9CEBDC
    // Broiler-Human:        PENDING
    public double ToNumber(JsHostValue value)
    {
        Enter(2);

        try
        {
            return engine.ToNumber(Unwrap(value));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>The abstract operation <c>ToBoolean</c>. Runs no guest code (card B06).</summary>
    /// <remarks>
    /// <para>
    /// <b>It is a realm member although it runs nothing, because one kind of object is false</b>: an
    /// object with an <c>[[IsHTMLDDA]]</c> slot (<see cref="NewHtmlDdaObject"/>), which the value
    /// alone cannot tell apart from any other object. Every other answer is the language's from the
    /// value: <c>0n</c>, <c>0</c>, <c>-0</c>, NaN, the empty String, <c>undefined</c>, <c>null</c>
    /// and <c>false</c> are false, and so is <see cref="JsHostValue.Missing"/>.
    /// </para>
    /// <para>
    /// <b>A BigInt is answered without entering the realm's value space</b>: its truth is whether it
    /// is zero, so neither the surface check nor the width ceiling nor a per-word charge applies.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2B5052
    // Broiler-Human:        PENDING
    public bool ToBoolean(JsHostValue value)
    {
        Enter(1);

        return value.Kind == JsHostValueKind.BigInt
            ? !((System.Numerics.BigInteger)value.RawReference!).IsZero
            : Unwrap(value).ToBooleanValue();
    }

    // ---- members -------------------------------------------------------------------------------

    /// <summary>Installs a data property: writable, enumerable and configurable.</summary>
    /// <remarks>
    /// <b>Enumerable, and the check that caught this is the reason it says so out loud.</b> The
    /// profile's own built-ins are deliberately non-enumerable - a <c>for...in</c> over an object
    /// must not walk <c>Object.prototype</c>'s methods - and installing a host's members on those
    /// terms made <c>Object.keys</c> answer without them, which is not what an embedder describing
    /// an interface means. The interface-description language a DOM-shaped embedder is implementing
    /// says its members are enumerable, writable and configurable, so this is that set: the same
    /// one an ordinary guest assignment produces.
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="value"/> is <see cref="JsHostValue.Missing"/>, which is not a value (JSD-0024
    /// section 20); nothing is written or thrown into the realm.
    /// </exception>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7AEF7E
    // Broiler-Human:        PENDING
    public void DefineValue(
        JsHostValue target,
        string name,
        JsHostValue value,
        JsHostPropertyFlags flags = JsHostPropertyFlags.Default)
    {
        Enter(3);
        RequireValue(value, nameof(value));

        try
        {
            using var installing = Install();

            ObjectOf(target).SetOwnProperty(
                name ?? string.Empty, JsProperty.Data(Unwrap(value), Attributes(flags)));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// Installs an accessor property. A <see langword="null"/> setter makes it read-only.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=34375F
    // Broiler-Human:        PENDING
    public void DefineAccessor(
        JsHostValue target,
        string name,
        JsHostFunction getter,
        JsHostFunction? setter = null,
        JsHostPropertyFlags flags = JsHostPropertyFlags.Default)
    {
        Enter(5);

        if (getter is null)
        {
            throw new System.ArgumentNullException(nameof(getter));
        }

        try
        {
            using var installing = Install();
            var key = name ?? string.Empty;

            ObjectOf(target).SetOwnProperty(
                key,
                JsProperty.Accessor(
                    engine.Realm.Native("get " + key, 0, Bind(getter)),
                    setter is null ? null : engine.Realm.Native("set " + key, 1, Bind(setter)),
                    Attributes(flags)));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>Reads a property, following the prototype chain. May run a guest getter.</summary>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="target"/> is <see cref="JsHostValue.Missing"/>, which is not a value (JSD-0024
    /// section 20); nothing is written or thrown into the realm.
    /// </exception>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A7C320
    // Broiler-Human:        PENDING
    public JsHostValue GetProperty(JsHostValue target, string name)
    {
        Enter(3);
        RequireValue(target, nameof(target));

        try
        {
            return Wrap(engine.GetProperty(Unwrap(target), name ?? string.Empty));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>Writes a property. May run a guest setter.</summary>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="target"/> or <paramref name="value"/> is <see cref="JsHostValue.Missing"/> (JSD-0024 section
    /// 20); nothing is written.
    /// </exception>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=53AF3D
    // Broiler-Human:        PENDING
    public void SetProperty(JsHostValue target, string name, JsHostValue value)
    {
        Enter(3);
        RequireValue(target, nameof(target));
        RequireValue(value, nameof(value));

        try
        {
            // NOT STRICT, AND THE DIFFERENCE IS OBSERVABLE. An assignment to an accessor with no
            // setter is a silent no-op in the language's ordinary mode and a TypeError in strict
            // mode, and an embedder writing to a member it does not own is doing what an ordinary
            // assignment does rather than what a strict program does. A host that wants the refusal
            // asks whether the write took by reading the property back.
            engine.SetProperty(Unwrap(target), name ?? string.Empty, Unwrap(value), strict: false);
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>Whether the property exists, own or inherited.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=814218
    // Broiler-Human:        PENDING
    public bool HasProperty(JsHostValue target, string name)
    {
        Enter(3);

        try
        {
            return engine.HasProperty(ObjectOf(target), name ?? string.Empty);
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// The object's own enumerable string keys: integer-like keys in ascending order, then the
    /// rest in creation order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Enumerable, and the filter is here rather than in the object.</b> The engine's own key
    /// walk answers every own string key whatever its attributes, because the operations inside
    /// the realm that use it do their own filtering. Answering that list unfiltered would hand an
    /// embedder the realm's non-enumerable built-ins as if they were the object's members, and an
    /// embedder mirroring what it was told would publish them.
    /// </para>
    /// <para>
    /// <b>The order is the language's, which is not creation order.</b> Integer-like keys come
    /// first in ascending numeric order and everything else follows in the order it was created,
    /// so an object with a member named <c>2</c> reports it before a member added earlier. That is
    /// what the specification's own key order does and what a guest sees from
    /// <c>Object.keys</c>; a member that promised creation order would be promising something the
    /// realm underneath it does not do.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EA8ECC
    // Broiler-Human:        PENDING
    public System.Collections.Generic.IReadOnlyList<string> OwnPropertyNames(JsHostValue target)
    {
        Enter(4);

        try
        {
            var host = ObjectOf(target);
            var names = host.OwnPropertyNames();
            engine.ChargeText(names.Count);

            var enumerable = new System.Collections.Generic.List<string>(names.Count);

            foreach (var name in names)
            {
                if (host.TryGetOwnProperty(name, out var property) &&
                    (property.Attributes & JsPropertyAttributes.Enumerable) != 0)
                {
                    enumerable.Add(name);
                }
            }

            return enumerable;
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>Installs an integer-indexed data property.</summary>
    /// <remarks>
    /// <b>On an Array this grows <c>length</c>, and that is the whole reason it is a member of its
    /// own rather than a formatted key handed to <see cref="DefineValue"/>.</b> An index written
    /// through the ordinary string path lands in the object's map without the Array's own length
    /// bookkeeping noticing, which makes the element invisible to every generic that reads
    /// <c>length</c> to find out how far to walk.
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="value"/> is <see cref="JsHostValue.Missing"/>, which is not a value (JSD-0024
    /// section 20); nothing is written or thrown into the realm.
    /// </exception>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=19C400
    // Broiler-Falsified-If: an index defined here is not found by an operation that walks length
    // Broiler-Human:        PENDING
    public void DefineIndex(
        JsHostValue target,
        uint index,
        JsHostValue value,
        JsHostPropertyFlags flags = JsHostPropertyFlags.Default)
    {
        Enter(3);
        RequireValue(value, nameof(value));

        try
        {
            using var installing = Install();
            var host = ObjectOf(target);

            if (host is JsArray array)
            {
                array.SetIndex(index, Unwrap(value));
                return;
            }

            host.SetOwnProperty(
                JsNumberFormat.ToUintString(index),
                JsProperty.Data(Unwrap(value), Attributes(flags)));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>Reads an integer-indexed property, following the prototype chain.</summary>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="target"/> is <see cref="JsHostValue.Missing"/>, which is not a value (JSD-0024
    /// section 20); nothing is written or thrown into the realm.
    /// </exception>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BF73FD
    // Broiler-Human:        PENDING
    public JsHostValue GetIndex(JsHostValue target, uint index)
    {
        Enter(3);
        RequireValue(target, nameof(target));

        try
        {
            return Wrap(engine.GetProperty(Unwrap(target), JsNumberFormat.ToUintString(index)));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>Deletes an own property, answering whether it is now absent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=799789
    // Broiler-Human:        PENDING
    public bool DeleteProperty(JsHostValue target, string name)
    {
        Enter(3);

        try
        {
            return ObjectOf(target).DeleteOwnProperty(name ?? string.Empty);
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>Points an object's prototype chain at another object, or at nothing.</summary>
    /// <remarks>
    /// This is how an embedder links a wrapper to the interface it implements, so that asking for
    /// the object's prototype answers the interface rather than the realm's plain
    /// <c>Object.prototype</c>. Passing a null-ish value gives the object a null prototype, which is
    /// a legal and different thing from leaving it alone.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0A9DFA
    // Broiler-Human:        PENDING
    public void SetPrototype(JsHostValue target, JsHostValue prototype)
    {
        Enter(3);

        try
        {
            ObjectOf(target).Prototype = prototype.IsNullish ? null : ObjectOf(prototype);
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>The object's prototype, or <see cref="JsHostValue.Null"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D5A4E5
    // Broiler-Human:        PENDING
    public JsHostValue GetPrototype(JsHostValue target)
    {
        Enter(2);

        try
        {
            var prototype = ObjectOf(target).Prototype;

            return prototype is null ? JsHostValue.Null : Wrap(JsValue.Object(prototype));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    // ---- bytes ---------------------------------------------------------------------------------

    /// <summary>
    /// Copies the bytes of an <c>ArrayBuffer</c> into a new array the embedder owns.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The brand is the realm's own buffer type, and nothing a guest can write decides it.</b> No
    /// property is read, no function is called and no global is consulted, so a page that replaced
    /// <c>ArrayBuffer</c>, <c>Uint8Array</c>, a species, <c>join</c> or a <c>byteLength</c> getter
    /// has changed nothing this member does, and no guest code runs inside it. See
    /// <see cref="JsHostBufferStatus"/> for the answers.
    /// </para>
    /// <para>
    /// <b>The array is a copy and is the embedder's from the moment it is answered.</b> The realm's
    /// own storage is never handed out, so a guest writing to the buffer afterwards cannot change
    /// what the embedder holds, and the embedder writing to the array cannot change the buffer. An
    /// empty buffer answers <see cref="JsHostBufferStatus.Copied"/> with an empty array; every other
    /// status answers an empty array too, and only the status tells them apart.
    /// </para>
    /// <para>
    /// <b>It costs the crossing and then one unit of fuel per byte, charged before a byte is
    /// copied.</b> A charge the allowance refuses, or a cancellation observed while charging, ends
    /// the operation as every other crossing does - with <see cref="JsHostTerminatedException"/> -
    /// and <paramref name="bytes"/> is not assigned: there is no partial answer to catch.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=9302B3
    // Broiler-Falsified-If: a guest-writable property or function decides the answer, or the realm's own storage is answered
    // Broiler-Human:        PENDING
    public JsHostBufferStatus TryReadArrayBuffer(JsHostValue value, out byte[] bytes)
    {
        Enter(2);

        try
        {
            var status = BufferBytes(value, out var data);

            if (status != JsHostBufferStatus.Copied || data!.Length == 0)
            {
                bytes = System.Array.Empty<byte>();
                return status;
            }

            // CHARGED BEFORE THE COPY, SO A REFUSED CHARGE COPIES NOTHING. The charge is split at
            // the engine's poll window, which is also where a cancellation is observed.
            engine.Charge((ulong)data.Length);

            var copy = new byte[data.Length];
            System.Buffer.BlockCopy(data, 0, copy, 0, data.Length);

            bytes = copy;
            return status;
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// Copies the bytes of an <c>ArrayBuffer</c> into the front of a span the embedder supplies.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The same brand test, answers and charge as the member that allocates, for an embedder that
    /// already owns the memory. <paramref name="byteLength"/> is the number of bytes written for
    /// <see cref="JsHostBufferStatus.Copied"/>; for
    /// <see cref="JsHostBufferStatus.DestinationTooSmall"/> it is the length the buffer needs, so an
    /// embedder can size a destination and ask again; for every other status it is zero.
    /// </para>
    /// <para>
    /// <b>Nothing is written unless everything fits and everything is paid for.</b> A destination
    /// too small is answered before the charge, a refused charge or a cancellation throws before
    /// the copy, and the bytes past <paramref name="byteLength"/> are never touched. The span is
    /// only borrowed for this call; the realm keeps no reference to it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=780A25
    // Broiler-Falsified-If: a destination is written when the answer is not Copied, or is written before the charge is admitted
    // Broiler-Human:        PENDING
    public JsHostBufferStatus TryReadArrayBuffer(
        JsHostValue value, System.Span<byte> destination, out int byteLength)
    {
        Enter(2);

        try
        {
            var status = BufferBytes(value, out var data);

            if (status != JsHostBufferStatus.Copied)
            {
                byteLength = 0;
                return status;
            }

            if (data!.Length > destination.Length)
            {
                byteLength = data.Length;
                return JsHostBufferStatus.DestinationTooSmall;
            }

            if (data.Length > 0)
            {
                engine.Charge((ulong)data.Length);
                new System.ReadOnlySpan<byte>(data).CopyTo(destination);
            }

            byteLength = data.Length;
            return status;
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// A new <c>ArrayBuffer</c> in this realm holding a copy of <paramref name="bytes"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>One operation, and nothing guest-visible takes part in it.</b> The buffer is built on the
    /// realm's intrinsic <c>ArrayBuffer.prototype</c> - the one a guest cannot replace, whatever it
    /// did to the global named <c>ArrayBuffer</c> - and filled by a copy, so no constructor, species,
    /// typed array or setter runs and no guest code can observe it half built.
    /// </para>
    /// <para>
    /// <b>The span is read during this call and never again.</b> The bytes are copied before the
    /// value is answered, so an embedder mutating or releasing its memory afterwards changes nothing
    /// the guest can read. An empty span answers an empty buffer.
    /// </para>
    /// <para>
    /// <b>Everything is paid for before the buffer exists.</b> The crossing, then one unit of fuel
    /// per byte - the same charge <c>new ArrayBuffer(n)</c> makes - then the live-bytes ceiling,
    /// which is asked rather than told so that its refusal is decided now rather than at the next
    /// charge. Any of them refusing, or a cancellation observed while charging, ends the operation
    /// with <see cref="JsHostTerminatedException"/> and no buffer was made: there is no partial
    /// value to hold. A span longer than the largest array the runtime can allocate is a
    /// <c>RangeError</c>, as <c>new ArrayBuffer(n)</c> would throw.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=E9CCCF
    // Broiler-Falsified-If: a buffer is answered after its fuel or live-bytes charge was refused, or a later write to the caller's memory changes it
    // Broiler-Human:        PENDING
    public JsHostValue NewArrayBuffer(System.ReadOnlySpan<byte> bytes)
    {
        Enter(4);

        try
        {
            if (bytes.Length > System.Array.MaxLength)
            {
                throw engine.Error(
                    "RangeError",
                    "Invalid array buffer length: " + JsNumberFormat.ToJsString(bytes.Length));
            }

            engine.Charge((ulong)bytes.Length);
            engine.RetainOrAbort((ulong)bytes.Length);

            var buffer = new JsArrayBuffer(engine.Realm.ArrayBufferPrototype, bytes.Length);
            bytes.CopyTo(buffer.Data!);

            return Wrap(JsValue.Object(buffer));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>The brand test both reads share: the realm's own buffer type, and its bytes.</summary>
    /// <remarks>
    /// The storage is answered to the two members above and to nothing else, and they copy out of it
    /// before they return. It is private because handing it any further would be handing out the
    /// guest's mutable memory.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=78A87E
    // Broiler-Falsified-If: an object that is not the realm's own buffer type answers Copied
    // Broiler-Human:        PENDING
    private JsHostBufferStatus BufferBytes(JsHostValue value, out byte[]? data)
    {
        data = null;

        if (Unwrap(value).AsObjectOrNull() is not JsArrayBuffer buffer)
        {
            return JsHostBufferStatus.NotAnArrayBuffer;
        }

        data = buffer.Data;

        return data is null ? JsHostBufferStatus.Detached : JsHostBufferStatus.Copied;
    }

    // ---- binary data ---------------------------------------------------------------------------

    /// <summary>
    /// Detaches an <c>ArrayBuffer</c>: the language's <c>DetachArrayBuffer</c>, performed by the
    /// host that owns the realm.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The specification makes detachment a host operation, and this is the host's door to
    /// it.</b> The language itself detaches only through <c>ArrayBuffer.prototype.transfer</c>;
    /// everything else that empties a buffer - a structured transfer, a conformance suite's
    /// <c>$262.detachArrayBuffer</c> - is an embedder calling <c>DetachArrayBuffer</c> on a buffer
    /// it was handed. Nothing here gives a guest a new way to detach: a guest reaches this only
    /// through a function an embedder chose to install, in a composition that registered
    /// <see cref="JavaScriptProfile.HostSurfaceCapability"/>.
    /// </para>
    /// <para>
    /// <b>It performs the same act as <c>transfer</c> and no other.</b> The bytes are released and
    /// every view over the buffer answers absent for every index from that instant on, because
    /// detachment is one field becoming null; a buffer already detached stays detached, which is
    /// the specification's answer too. This profile builds no <c>SharedArrayBuffer</c> and no
    /// buffer with a detach key, so the two refusals the specification's steps can reach are not
    /// reachable here.
    /// </para>
    /// <para>
    /// <b>A value that is not an <c>ArrayBuffer</c> is a guest <c>TypeError</c></b>, raised as a
    /// <see cref="JsHostThrowException"/> an installed function can let propagate to the guest -
    /// which is what a conformance host answers for <c>$262.detachArrayBuffer(1)</c>. It is not a
    /// <see cref="JsHostSurfaceException"/>, because asking is not a wiring defect of the host.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=D9D821
    // Broiler-Falsified-If: a buffer this detaches can still be read or written through any view, or a guest reaches this without a function an embedder installed
    // Broiler-Human:        PENDING
    public void DetachArrayBuffer(JsHostValue buffer)
    {
        Enter(2);

        if (UnwrapAtCrossing(buffer).AsObjectOrNull() is not JsArrayBuffer target)
        {
            throw Error(JsHostErrorKind.TypeError, "DetachArrayBuffer requires an ArrayBuffer");
        }

        _ = target.Detach();
    }

    // ---- calling -------------------------------------------------------------------------------

    /// <summary>
    /// Calls a guest function from host code, and this is the member the whole seam exists for.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It routes through <c>JsEngine.Call</c>, which is the interpreter's own call path.</b> The
    /// call-depth ceiling is charged, the fuel is charged, cancellation is polled, and a guest
    /// <c>throw</c> unwinds through the C# frames exactly as it does when a built-in calls a
    /// comparison function. Nothing here is a second execution mechanism, which is why nothing here
    /// needs a second set of rules.
    /// </para>
    /// <para>
    /// <b>What comes out is a <see cref="JsHostThrowException"/> and never a raw guest throw.</b> A
    /// provider must be able to catch what the guest threw without catching the profile's own
    /// control flow, and the two are different types precisely so a <c>catch</c> written for one
    /// cannot absorb the other.
    /// </para>
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    /// An argument is <see cref="JsHostValue.Missing"/> (JSD-0024 section 20); the function is not called. A
    /// receiver that is <see cref="JsHostValue.Missing"/> is <c>undefined</c>, as a call with no receiver is.
    /// </exception>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=790C56
    // Broiler-Falsified-If: a guest call from host code skips the call-depth charge or lets a JsAbort escape as a guest throw
    // Broiler-Human:        PENDING
    public JsHostValue Invoke(
        JsHostValue function, JsHostValue thisValue, System.ReadOnlySpan<JsHostValue> arguments = default)
    {
        Enter(4 + (ulong)arguments.Length);
        RequireValues(arguments, nameof(arguments));

        var callee = UnwrapAtCrossing(function);

        if (!callee.IsObject || !callee.AsObject().IsCallable)
        {
            throw new JsHostSurfaceException(
                JsHostRefusal.NotCallable, "the value presented to Invoke is not callable");
        }

        try
        {
            return Wrap(engine.Call(callee, Unwrap(thisValue), UnwrapAll(arguments)));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>Calls a guest constructor with <c>new</c>.</summary>
    /// <exception cref="System.ArgumentException">
    /// An argument is <see cref="JsHostValue.Missing"/> (JSD-0024 section 20); nothing is constructed.
    /// </exception>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=BE2103
    // Broiler-Human:        PENDING
    public JsHostValue Construct(
        JsHostValue constructor, System.ReadOnlySpan<JsHostValue> arguments = default)
    {
        Enter(4 + (ulong)arguments.Length);
        RequireValues(arguments, nameof(arguments));

        var callee = UnwrapAtCrossing(constructor);

        if (!callee.IsObject)
        {
            throw new JsHostSurfaceException(
                JsHostRefusal.NotAnObject, "the value presented to Construct is not an object");
        }

        try
        {
            return Wrap(engine.Construct(callee, UnwrapAll(arguments)));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    // ---- scripts -------------------------------------------------------------------------------

    /// <summary>
    /// Runs <paramref name="source"/> as a script in this realm and answers its completion value
    /// (JSeal V15-host, JSD-0024 section 16).
    /// </summary>
    /// <param name="source">The script's source text.</param>
    /// <param name="sourceName">
    /// The name a refusal is attributed to: the provider's diagnostic carries it with the position,
    /// and the <c>SyntaxError</c> the embedder receives names it. It decides nothing else, and may not
    /// contain U+0000.
    /// </param>
    /// <param name="strict">Whether the script is strict code whatever its directive prologue says.</param>
    /// <remarks>
    /// <para>
    /// <b>This is <c>ScriptEvaluation</c>, not an <c>eval</c></b>: the script's <c>let</c>,
    /// <c>const</c> and <c>class</c> declarations persist in the realm's global lexical environment for
    /// every later script, its <c>var</c>s and functions are non-configurable global properties, and
    /// <c>GlobalDeclarationInstantiation</c>'s conflict and definability checks run before its first
    /// instruction - a failed one is the guest <c>SyntaxError</c> or <c>TypeError</c> the language
    /// names, with nothing created. A source that does not parse is a <c>SyntaxError</c>.
    /// </para>
    /// <para>
    /// <b>The embedder authorises the compilation, and only the embedder can ask for it.</b> The
    /// source travels to the composition's artifact provider under
    /// <see cref="Format.JsFormat.ScriptRequestMark"/>, which no guest-initiated load can send, so a
    /// provider may answer it under a policy that refuses guest evaluation - and a direct or indirect
    /// <c>eval</c> the script itself performs is still a guest request that policy decides.
    /// </para>
    /// <para>
    /// <b>It is a crossing and it is charged like one</b>: one <c>HostCalls</c> unit and 4 fuel here,
    /// then the source's length in fuel when the request is made, then the script's own execution
    /// under the same allowance. A guest throw reaches the embedder as a
    /// <see cref="JsHostThrowException"/>; an exhausted allowance or a cancellation as the latched
    /// <see cref="JsHostTerminatedException"/>. No provider registered, or an answer that is not a
    /// script compiled under the requested strictness, is a guest <c>EvalError</c>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=634ACD
    // Broiler-Falsified-If: a host script runs outside a step, without charging HostCalls, or lets a JsAbort escape as a guest throw
    // Broiler-Human:        PENDING
    public JsHostValue EvaluateScript(string source, string sourceName = "", bool strict = false)
    {
        Enter(4);

        System.ArgumentNullException.ThrowIfNull(source);
        System.ArgumentNullException.ThrowIfNull(sourceName);

        if (sourceName.Contains('\0'))
        {
            throw new System.ArgumentException("a source name may not contain U+0000", nameof(sourceName));
        }

        try
        {
            return Wrap(engine.EvaluateScript(source, sourceName, strict));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    // ---- jobs ----------------------------------------------------------------------------------

    /// <summary>Whether any job is queued.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6D189C
    // Broiler-Human:        PENDING
    public bool HasPendingJobs
    {
        get
        {
            Enter(1);
            return engine.HasPendingJobs;
        }
    }

    /// <summary>Queues a host callback to run at the next drain.</summary>
    /// <remarks>
    /// <b>It joins the guest's own queue rather than a second one beside it.</b> A host job and a
    /// promise reaction are both microtasks, and two queues would make their relative order a
    /// property of which queue a drain happened to read first - which is exactly the kind of
    /// ordering an embedder cannot reason about and a page depends on. The action is wrapped in an
    /// ordinary native function and enqueued where every other job goes.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=749324
    // Broiler-Falsified-If: a host job and a guest job run in an order neither queue decided
    // Broiler-Human:        PENDING
    public void EnqueueJob(System.Action job)
    {
        Enter(2);

        if (job is null)
        {
            throw new System.ArgumentNullException(nameof(job));
        }

        try
        {
            engine.EnqueueJob(
                JsValue.Object(engine.Realm.Native(
                    "hostJob",
                    0,
                    (owner, _, _) =>
                    {
                        try
                        {
                            job();
                        }
                        catch (JsHostThrowException raised)
                        {
                            throw Raised(raised);
                        }
                        catch (JsHostTerminatedException)
                        {
                            throw latched ?? new JsAbort(
                                JsAbortKind.InternalDefect,
                                "a host job reported a termination that was not latched");
                        }
                        catch (JsHostSurfaceException refusal)
                        {
                            throw owner.Error("TypeError", refusal.Message);
                        }

                        return JsValue.Undefined;
                    })),
                System.Array.Empty<JsValue>());
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// Runs queued jobs until the queue is empty or <paramref name="limit"/> have run, answering
    /// how many ran.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A job that enqueues another is followed, which is why there is a limit at all.</b> A
    /// promise chain that re-queues itself would otherwise make this loop unbounded by anything
    /// but the allowance, and an embedder that wanted to interleave its own work between turns
    /// would have no way to take one.
    /// </para>
    /// <para>
    /// <b>A job that throws stops the drain and leaves its successors queued.</b> That differs
    /// from the profile's own whole-queue drain, which folds every fault into the first and reports
    /// it at the end; the difference is who is asking. A host draining a bounded number of jobs is
    /// stepping the queue and wants to see the fault at the job that caused it, with the rest of
    /// the queue intact to drain again.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=0D75AE
    // Broiler-Falsified-If: a drain runs more than its limit, or a throwing job discards the queue behind it
    // Broiler-Human:        PENDING
    public int DrainJobs(int limit = 10_000)
    {
        Enter(2);

        var ran = 0;

        try
        {
            while (ran < limit && engine.HasPendingJobs)
            {
                if (engine.StepOneJob(out var thrown))
                {
                    ran++;
                    throw new JsThrow(thrown, engine.ToStringValue(thrown));
                }

                ran++;
            }

            return ran;
        }
        catch (JsThrow raised)
        {
            throw Thrown(raised);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    // ---- promises ------------------------------------------------------------------------------

    /// <summary>A new pending promise of this realm, and the right to settle it.</summary>
    /// <remarks>
    /// <para>
    /// <b>It is built on the realm's intrinsic <c>Promise.prototype</c>, never through the
    /// <c>Promise</c> global.</b> That binding is writable, and a host that constructed through it
    /// would build whatever a page had put there. Nothing here runs guest code at all: there is no
    /// executor to call, because the resolving functions stay on this side of the seam.
    /// </para>
    /// <para>
    /// <b>It is charged as <c>new Promise</c> is charged</b> - the realm's own constructor cost and
    /// retention for the resolving pair - on top of the crossing, so minting a capability costs the
    /// guest no less than minting a promise would.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=33371B
    // Broiler-Falsified-If: the promise is built through a binding a guest can replace
    // Broiler-Human:        PENDING
    public JsHostPromiseCapability NewPromiseCapability()
    {
        Enter(6);

        try
        {
            var promise = engine.Realm.NewHostPromise(engine);
            return new JsHostPromiseCapability(this, promise, Wrap(JsValue.Object(promise)));
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>Resolves a capability's promise with a value, adopting it if it is a thenable.</summary>
    /// <remarks>
    /// <para>
    /// <b>It is the realm's own resolve procedure</b>, so a thenable is adopted exactly as the
    /// language adopts one: its <c>then</c> is read once, now, on this thread and inside this step -
    /// which is why the member is refused anywhere else - and a callable <c>then</c> is called from a
    /// queued job. Resolving the promise with itself rejects it with a <c>TypeError</c>.
    /// </para>
    /// <para>
    /// <b>No reaction runs before this returns.</b> Settling hands every waiting reaction to the job
    /// queue, and the host decides when that queue runs.
    /// </para>
    /// <para>
    /// <b><see cref="JsHostValue.Missing"/> is refused</b> with an <see cref="System.ArgumentException"/>
    /// and leaves the capability unsettled: it is not a language value, and resolving with
    /// <c>undefined</c> is spelled <see cref="JsHostValue.Undefined"/>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=402C27
    // Broiler-Falsified-If: a second resolution changes the promise or queues a reaction
    // Broiler-Human:        PENDING
    public JsHostSettlement ResolvePromise(JsHostPromiseCapability capability, JsHostValue value)
    {
        Enter(3);

        return Settle(capability, value, rejected: false);
    }

    /// <summary>Rejects a capability's promise with a reason.</summary>
    /// <remarks>
    /// A rejection reason is never adopted, so this reads nothing off the reason and runs no guest
    /// code; it shares the capability's one resolved flag with <see cref="ResolvePromise"/>, so
    /// whichever of the two is called first wins. <see cref="JsHostValue.Missing"/> is refused with
    /// an <see cref="System.ArgumentException"/>, as for <see cref="ResolvePromise"/>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=586445
    // Broiler-Falsified-If: a rejection after a resolution changes the promise or queues a reaction
    // Broiler-Human:        PENDING
    public JsHostSettlement RejectPromise(JsHostPromiseCapability capability, JsHostValue reason)
    {
        Enter(3);

        return Settle(capability, reason, rejected: true);
    }

    /// <summary>The body both settlement members share, after their entry charge.</summary>
    /// <remarks>
    /// <para>
    /// <b>A misuse is refused before the flag is read</b>, so a foreign capability, a foreign
    /// value or <see cref="JsHostValue.Missing"/> is reported as the mistake it is even when the
    /// promise happens to be settled already; answering
    /// <see cref="JsHostSettlement.AlreadyResolved"/> there would hide it.
    /// </para>
    /// <para>
    /// <b>The flag is set before the resolve procedure runs</b>, which is the specification's order
    /// for <c>[[AlreadyResolved]]</c>: a <c>then</c> getter that re-entered the host and settled the
    /// same capability again meets a capability that is already resolved.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=531055
    // Broiler-Falsified-If: a capability another realm minted settles a promise in this one
    // Broiler-Human:        PENDING
    private JsHostSettlement Settle(JsHostPromiseCapability capability, JsHostValue value, bool rejected)
    {
        if (capability is null)
        {
            throw new System.ArgumentNullException(nameof(capability));
        }

        if (!ReferenceEquals(capability.Realm, this))
        {
            throw new JsHostSurfaceException(
                JsHostRefusal.ForeignRealm,
                "the promise capability was minted by a different realm and means nothing in this one");
        }

        // MISSING IS NOT A VALUE THE LANGUAGE HAS. It unwraps to the marker an uninitialised
        // binding holds, and a promise settled with it would hand every reaction a hole that reads
        // as a TDZ error; the embedder meant undefined or meant nothing, and either way it is told.
        if (value.IsMissing)
        {
            throw new System.ArgumentException(
                "Missing is not a settlement value; pass JsHostValue.Undefined for undefined",
                rejected ? "reason" : "value");
        }

        var argument = UnwrapAtCrossing(value);

        if (capability.AlreadyResolved)
        {
            return JsHostSettlement.AlreadyResolved;
        }

        capability.AlreadyResolved = true;

        try
        {
            // THE SAME TWO PATHS AN ASYNC FUNCTION'S OWN PROMISE SETTLES BY: a rejection settles,
            // and a resolution goes through the resolve procedure that adopts a thenable.
            engine.Realm.SettleAsyncPromise(engine, capability.Target, argument, rejected);
            return JsHostSettlement.Accepted;
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    // ---- modules -------------------------------------------------------------------------------

    /// <summary>
    /// The embedder's loader, when its surface implements one; recognised once, when the realm is
    /// handed over.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=284B76
    // Broiler-Human:        PENDING
    internal IJsHostModuleLoader? ModuleLoader { get; set; }

    /// <summary>The one handle this realm answers for each module key a host linked.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=ACBB77
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, JsHostModule> modules =
        new(System.StringComparer.Ordinal);

    /// <summary>
    /// Loads and links the module graph <paramref name="specifier"/> names from
    /// <paramref name="referrer"/> into this realm, evaluating nothing, and answers its root.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The graph comes from the composition's artifact provider and from nowhere else.</b> The
    /// pair is put to it as the same module request a guest <c>import()</c> makes, so the
    /// provider resolves, compiles under the module goal and answers; the core verifies the answer
    /// under this operation's allowance; and an answer whose entry is not a module graph is refused.
    /// A composition that holds a resolved in-memory graph - keys, source text, each module's
    /// resolutions - answers from it, and one that registered no provider answers nothing. Nothing
    /// here compiles, and no module text ever reaches the classic-script path.
    /// </para>
    /// <para>
    /// <b>Identity is the realm's module registry.</b> A module this realm already holds under its
    /// key - linked by an earlier call, statically imported, or reached by a guest
    /// <c>import()</c> - is adopted rather than rebuilt, and the same key answers the same
    /// <see cref="JsHostModule"/> and the same namespace every time.
    /// </para>
    /// <para>
    /// <b>Failures are guest errors, as for <c>import()</c>.</b> A module the provider does not have,
    /// a provider that is not registered, or a resolution the composition does not confirm throws a
    /// <see cref="JsHostThrowException"/> carrying a <c>TypeError</c>; a module the front end refused,
    /// or a graph that does not link, carries a <c>SyntaxError</c>. A refused graph leaves nothing
    /// registered, so a later call links it from the start. A spent allowance or a cancellation is
    /// <see cref="JsHostTerminatedException"/>, as everywhere.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=B6FBE2
    // Broiler-Falsified-If: a module reaches the realm without the artifact provider, a module body runs, or one key answers two handles
    // Broiler-Human:        PENDING
    public JsHostModule LoadModule(string specifier, string referrer = "")
    {
        Enter(4);

        if (specifier is null)
        {
            throw new System.ArgumentNullException(nameof(specifier));
        }

        referrer ??= string.Empty;

        // THE REQUEST SEPARATES THE TWO HALVES WITH A NUL, so a referrer holding one would be read
        // back as a different referrer and a different specifier. A module key never holds one.
        if (referrer.Contains('\0'))
        {
            throw new System.ArgumentException("a referrer cannot contain U+0000", nameof(referrer));
        }

        try
        {
            engine.ChargeText(specifier.Length + referrer.Length);

            var (program, index) = engine.LinkHostModule(referrer, specifier);
            var key = JsEngine.ModuleKey(program, index);

            if (modules.TryGetValue(key, out var known))
            {
                return known;
            }

            var handle = new JsHostModule(
                this, program, index, key, Wrap(engine.ModuleNamespace(program, index)));

            modules[key] = handle;
            return handle;
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// Evaluates a linked module graph and answers its evaluation promise: the same promise on
    /// every call.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It never blocks and it never drains.</b> The bodies that can run synchronously run now,
    /// in dependency order, each once per realm; a body that awaits suspends the walk, and the rest
    /// of the graph runs from the job queue. The promise fulfils with <c>undefined</c> when the last
    /// module has finished and rejects with what a module threw, and it settles only through the
    /// queue - so a host reads the outcome after draining, never from this call.
    /// </para>
    /// <para>
    /// <b>A module another walk started and that is still under way is waited for</b> - one that
    /// is awaiting, and a member of a cycle whose root is - whether a guest <c>import()</c> or an
    /// earlier call started it, so the promise cannot fulfil while a module of the graph is
    /// suspended. <b>A module whose evaluation already failed</b>, by whatever route, rejects the
    /// promise with the identical value, and so does every module that depends on it; no body
    /// among them runs again.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=418E05
    // Broiler-Falsified-If: two calls answer two promises, a module body runs twice, the promise fulfils while a module of the graph is still suspended, or it fulfils for a graph holding an errored module
    // Broiler-Human:        PENDING
    public JsHostValue EvaluateModule(JsHostModule module)
    {
        Enter(4);

        if (module is null)
        {
            throw new System.ArgumentNullException(nameof(module));
        }

        if (!ReferenceEquals(module.Realm, this))
        {
            throw new JsHostSurfaceException(
                JsHostRefusal.ForeignRealm,
                "the module was linked by a different realm and means nothing in this one");
        }

        if (!module.Evaluation.IsMissing)
        {
            return module.Evaluation;
        }

        try
        {
            var promise = engine.Realm.NewHostPromise(engine);

            // RECORDED BEFORE ANY BODY RUNS, so a body that reaches back into the host and asks for
            // the same evaluation is answered the promise it is already part of, rather than a
            // second walk that would find every module under way and fulfil at once.
            module.Evaluation = Wrap(JsValue.Object(promise));
            engine.EvaluateInto(module.Program, module.Index, promise, settleWithNamespace: false);

            return module.Evaluation;
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// Reads what this realm knows about the module it holds under <paramref name="moduleKey"/>: its
    /// <c>[[Status]]</c>, its <c>[[EvaluationError]]</c>, its <c>[[HasTLA]]</c> and its
    /// <c>[[CycleRoot]]</c> (JSeal I11-upstream, JSD-0024 section 20).
    /// </summary>
    /// <param name="moduleKey">A key as <see cref="JsHostModule.Key"/> reports it.</param>
    /// <param name="state">The module's state when this answers <see langword="true"/>.</param>
    /// <returns>
    /// <see langword="false"/> when this realm holds no module of that key: none was linked here, by
    /// any route, or its graph did not link - a refused graph leaves nothing registered.
    /// </returns>
    /// <remarks>
    /// <para>
    /// <b>Every module the realm holds is answered, whatever route reached it</b> - a host's
    /// <see cref="LoadModule"/>, a module another one imports statically, a guest <c>import()</c>,
    /// or an artifact's own entry graph - because the registry is the realm's, not the host's.
    /// </para>
    /// <para>
    /// <b>When each status is observable.</b> <see cref="JsHostModuleStatus.Linked"/> from the
    /// moment <see cref="LoadModule"/> returns until an evaluation enters the module, and for good
    /// after a failed evaluation that never reached it. <see cref="JsHostModuleStatus.Evaluating"/>
    /// only while an evaluation walk is running and has entered the module without completing its
    /// component - that is, from host code a module body of the same walk calls synchronously (a
    /// host function, an exotic hook, a loader's <see cref="IJsHostModuleLoader.OnImport"/>); the
    /// walk runs inside one step, so no later step sees it. <see cref="JsHostModuleStatus.EvaluatingAsync"/>
    /// between the walk that completed its component and the async completion that finishes it,
    /// which a drain delivers. <see cref="JsHostModuleStatus.Evaluated"/> from then on, with
    /// <see cref="JsHostModuleState.EvaluationError"/> set when it failed. An evaluation asked for
    /// while another is under way - an <see cref="EvaluateModule"/> from a host callback, and every
    /// guest <c>import()</c> - runs from a job, so the modules it reaches stay
    /// <see cref="JsHostModuleStatus.Linked"/> until a drain runs it.
    /// <see cref="JsHostModuleStatus.Linking"/> and <see cref="JsHostModuleStatus.Unlinked"/> are
    /// not answered.
    /// </para>
    /// <para>
    /// <b>It reads and runs nothing</b>: no guest code, no getter, and no allocation the guest can
    /// see. It is a crossing like every member - one <c>HostCalls</c> unit and one unit of fuel,
    /// refused outside a step or on another thread with <see cref="JsHostRefusal.RealmNotCurrent"/> -
    /// and an evaluation error that is a BigInt is charged per word as every BigInt crossing is.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=863A32
    // Broiler-Human:        PENDING
    public bool TryGetModuleState(
        string moduleKey,
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out JsHostModuleState? state)
    {
        Enter(1);

        if (moduleKey is null)
        {
            throw new System.ArgumentNullException(nameof(moduleKey));
        }

        state = null;

        if (engine.FindModuleInstance(moduleKey) is not { } instance)
        {
            return false;
        }

        try
        {
            var status = instance.State switch
            {
                JsModuleState.Created => JsHostModuleStatus.Linking,
                JsModuleState.Initialised => JsHostModuleStatus.Linked,
                JsModuleState.Evaluating => JsHostModuleStatus.Evaluating,
                JsModuleState.EvaluatingAsync => JsHostModuleStatus.EvaluatingAsync,
                _ => JsHostModuleStatus.Evaluated,
            };

            state = new JsHostModuleState(
                moduleKey,
                status,
                instance.HasTla,
                instance.EvaluationError is { } error ? Wrap(error) : JsHostValue.Missing,
                instance.CycleRoot is { } root ? root.Program.Modules[root.Index].Key : string.Empty);

            return true;
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// Completes a guest <c>import()</c> the embedder's loader deferred: the module is loaded now,
    /// and the import settles when its graph has finished.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is the load the import would have made, made later.</b> The request's referrer and
    /// specifier are put to the composition's artifact provider in this step, the answer is
    /// verified and linked, and the graph is evaluated; the import fulfils with the module's
    /// namespace, or rejects with a <c>TypeError</c> (no such module), a <c>SyntaxError</c> (a source
    /// or link failure) or what a module threw. Every one of those settles through the job queue,
    /// and nothing is thrown to the embedder for them.
    /// </para>
    /// <para>
    /// <b>Once, in its own realm, and not while it is being offered.</b> A second completion or
    /// failure answers <see cref="JsHostSettlement.AlreadyResolved"/> and does nothing; a request
    /// another realm offered is refused with <see cref="JsHostRefusal.ForeignRealm"/>; a request
    /// completed from inside the <see cref="IJsHostModuleLoader.OnImport"/> call that offered it is
    /// an <see cref="System.InvalidOperationException"/>; and outside a step, including after the
    /// instance was released, the refusal is <see cref="JsHostRefusal.RealmNotCurrent"/>, as for
    /// every member.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=1CDAD4
    // Broiler-Falsified-If: a request settles twice, settles outside a step of its own realm, or settles other than through the job queue
    // Broiler-Human:        PENDING
    public JsHostSettlement CompleteModuleRequest(JsHostModuleRequest request)
    {
        Enter(4);

        if (!Claim(request))
        {
            return JsHostSettlement.AlreadyResolved;
        }

        try
        {
            engine.ChargeText(request.Specifier.Length + request.Referrer.Length);
            engine.CompleteImport(request.Referrer, request.Specifier, request.Target);
            return JsHostSettlement.Accepted;
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// Rejects a guest <c>import()</c> the embedder's loader deferred, with an error of the named
    /// kind.
    /// </summary>
    /// <remarks>
    /// The same once-only, own-realm, in-a-step rules as <see cref="CompleteModuleRequest"/>, and the
    /// same settlement: through the job queue, with no reaction run before this returns. A host that
    /// could not find or would not load a module fails with <see cref="JsHostErrorKind.TypeError"/>,
    /// which is what an undeferred import of a module the provider does not have rejects with.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=882B8B
    // Broiler-Falsified-If: a failed request settles twice, or settles outside a step of its own realm
    // Broiler-Human:        PENDING
    public JsHostSettlement FailModuleRequest(
        JsHostModuleRequest request, JsHostErrorKind kind, string message)
    {
        Enter(4);

        if (!Claim(request))
        {
            return JsHostSettlement.AlreadyResolved;
        }

        try
        {
            var error = engine.Realm.CreateError(NameOf(kind), message ?? string.Empty);
            engine.Realm.SettleAsyncPromise(engine, request.Target, error, rejected: true);
            return JsHostSettlement.Accepted;
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// The checks both completions share, answering false for a request that was settled already.
    /// </summary>
    /// <remarks>
    /// A misuse is refused before the settled flag is read, so a foreign request or a completion
    /// from inside the offer is reported as the mistake it is even when the request is settled.
    /// The flag is set here, before any work, so a completion that re-enters through guest code
    /// meets a request that is already taken.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=36AF6C
    // Broiler-Falsified-If: a request another realm offered, or one still being offered, is settled here
    // Broiler-Human:        PENDING
    private bool Claim(JsHostModuleRequest request)
    {
        if (request is null)
        {
            throw new System.ArgumentNullException(nameof(request));
        }

        if (!ReferenceEquals(request.Realm, this))
        {
            throw new JsHostSurfaceException(
                JsHostRefusal.ForeignRealm,
                "the module request was offered by a different realm and means nothing in this one");
        }

        if (request.Offering)
        {
            throw new System.InvalidOperationException(
                "a module request cannot be completed from inside the call that offers it; " +
                "answer Deferred and complete it from a later step");
        }

        if (request.Settled)
        {
            return false;
        }

        request.Settled = true;
        return true;
    }

    /// <summary>
    /// Offers one guest <c>import()</c> to the embedder's loader, answering whether it deferred it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A crossing, charged and gated like one.</b> It is internal because only the engine's
    /// dynamic import calls it, from inside a guest operation that is already inside a step; a
    /// realm reached outside one offers nothing, and the import is answered synchronously as it
    /// was before any loader existed.
    /// </para>
    /// <para>
    /// <b>What the loader raises is translated as a host function body's is</b>: a guest throw
    /// rejects the import with the value it carries, a refusal at the seam rejects it with a
    /// <c>TypeError</c>, and a termination re-raises the latched abort. Every outcome but
    /// <see cref="JsHostModuleLoad.Deferred"/> - an answer of <see cref="JsHostModuleLoad.Now"/> or
    /// any exception out of the loader - marks the request settled, so the embedder cannot later
    /// complete an import the realm already answered or rejected.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=7999FB
    // Broiler-Falsified-If: a loader's JsHostThrowException unwinds through interpreter frames untranslated, or a request the realm answered can be completed again
    // Broiler-Human:        PENDING
    internal bool OfferModuleRequest(string referrer, string specifier, JsPromiseObject promise)
    {
        if (ModuleLoader is not { } loader || !IsCurrent)
        {
            return false;
        }

        var request = new JsHostModuleRequest(this, referrer, specifier, promise) { Offering = true };
        var answer = JsHostModuleLoad.Now;

        try
        {
            Enter(1);
            answer = loader.OnImport(this, request);
        }
        catch (JsHostThrowException raised)
        {
            throw Raised(raised);
        }
        catch (JsHostTerminatedException)
        {
            throw latched ?? new JsAbort(
                JsAbortKind.InternalDefect, "a module loader reported a termination that was not latched");
        }
        catch (JsHostSurfaceException refusal)
        {
            throw engine.Error("TypeError", refusal.Message);
        }
        finally
        {
            // ONLY A DEFERRED ANSWER LEAVES THE REQUEST OPEN. A loader that threw has had its import
            // rejected here (or the step ended), so a request it kept must not settle it a second
            // time from a later turn.
            request.Offering = false;
            request.Settled = answer != JsHostModuleLoad.Deferred;
        }

        return answer == JsHostModuleLoad.Deferred;
    }

    /// <summary>
    /// Offers a named deletion to an exotic object's deletion hook, translating what it raises.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is a crossing, so it is charged and gated like one</b>, and it is internal rather than
    /// public because only the host object itself calls it, from inside a guest operation that is
    /// already inside a step.
    /// </para>
    /// <para>
    /// <b>What the handler raises is translated exactly as a host function body's is</b>, and for
    /// the same reason: a guest throw is re-thrown as the guest throw it is, a refusal at the seam
    /// becomes a <c>TypeError</c>, and a termination re-raises the latched abort. The deletion ends
    /// at the throw, so the object's own storage is left as it was.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=20E4E7
    // Broiler-Falsified-If: a handler's JsHostThrowException unwinds through interpreter frames untranslated
    // Broiler-Human:        PENDING
    internal void OfferDeletion(IJsHostExoticDeletion deleter, string name)
    {
        try
        {
            Enter(1);
            _ = deleter.TryDeleteNamed(this, name);
        }
        catch (JsHostThrowException raised)
        {
            throw Raised(raised);
        }
        catch (JsHostTerminatedException)
        {
            throw latched ?? new JsAbort(
                JsAbortKind.InternalDefect, "a deletion hook reported a termination that was not latched");
        }
        catch (JsHostSurfaceException refusal)
        {
            throw engine.Error("TypeError", refusal.Message);
        }
    }

    /// <summary>
    /// The exception an exotic object's hook raised, as the guest throw or the abort it stands for.
    /// </summary>
    /// <remarks>
    /// <b>A hook is a host body the guest reaches through a property operation instead of a call</b>,
    /// so what it raises is translated exactly as <see cref="Bind"/> translates a body's: a guest
    /// throw is re-thrown as the value it carries, a refusal at the seam - including a value it
    /// answered that another realm minted or that names a BigInt this realm's composition declined -
    /// becomes a <c>TypeError</c>, and a termination re-raises the latched abort. Only those three
    /// types arrive here; the host object filters by type, and runs nothing in the filter.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=7B7F0D
    // Broiler-Falsified-If: a hook's JsHostThrowException or JsHostSurfaceException unwinds through interpreter frames untranslated
    // Broiler-Human:        PENDING
    internal System.Exception HookRaised(System.Exception raised) => raised switch
    {
        JsHostThrowException thrown => Raised(thrown),
        JsHostSurfaceException refusal => engine.Error("TypeError", refusal.Message),
        _ => latched ?? new JsAbort(
            JsAbortKind.InternalDefect, "an exotic hook reported a termination that was not latched"),
    };

    /// <summary>
    /// Opens one exotic-hook crossing, charged and gated as every other crossing is, or answers that
    /// the hook is not to be asked because no step is open.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A hook is a crossing, so it pays for one</b> (JSD-0024 section 19.2): one <c>HostCalls</c>
    /// unit and one fuel unit before each <see cref="IJsHostExotic"/> member runs, exactly what the
    /// deletion hook and the module loader already paid. It is called inside the host object's
    /// translation, so a latched abort (<see cref="JsHostTerminatedException"/>) or a step open on
    /// another thread (<see cref="JsHostSurfaceException"/>) reaches the guest as
    /// <see cref="HookRaised"/> resolves it, and a spent allowance ends the operation as a
    /// <see cref="JsAbort"/> does anywhere else.
    /// </para>
    /// <para>
    /// <b>Outside every step the answer is <c>false</c> and nothing is charged or raised.</b> Guest
    /// code runs only inside a step, so the only reads that reach a hook then are the engine's own:
    /// rendering an uncaught value or a completion value after the step has closed. Those are not
    /// the guest's crossings, there is no step to charge them to, and refusing them would turn an
    /// ordinary uncaught value into a contract violation; the host object answers as though the
    /// handler held nothing, and the handler is not asked.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=9046ED
    // Broiler-Falsified-If: an exotic hook runs without charging HostCalls, outside a step, or after the realm latched an abort
    // Broiler-Human:        PENDING
    internal bool TryEnterHook()
    {
        if (stepDepth <= 0)
        {
            return false;
        }

        Enter(1);
        return true;
    }

    /// <summary>
    /// The guest throw an embedder's <see cref="JsHostThrowException"/> stands for.
    /// </summary>
    /// <remarks>
    /// <b>It runs inside the catch clause every guest-to-host crossing ends in</b>, where a refusal
    /// <see cref="Unwrap"/> raised would escape past the sibling clause meant for it and end the
    /// invocation. So a thrown value that another realm minted, or a BigInt thrown into a realm
    /// whose composition declined the surface, reaches the guest as the <c>TypeError</c> a returned
    /// one does (JSD-0024 section 19). A BigInt past the ceiling is already the guest's
    /// <c>RangeError</c> and passes through as one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=9154C2
    // Broiler-Falsified-If: a host's thrown value that this realm refuses escapes as a JsHostSurfaceException
    // Broiler-Human:        PENDING
    private JsThrow Raised(JsHostThrowException raised)
    {
        try
        {
            return new JsThrow(Unwrap(raised.Thrown), raised.Message);
        }
        catch (JsHostSurfaceException refusal)
        {
            return engine.Error("TypeError", refusal.Message);
        }
    }

    // ---- conversion ----------------------------------------------------------------------------

    /// <summary>Names a guest value for an embedder, canonicalising an object's identity.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=5F3135
    // Broiler-Falsified-If: two calls over one guest object answer two different JsHostRef instances
    // Broiler-Human:        PENDING
    internal JsHostValue Wrap(JsValue value)
    {
        switch (value.Type)
        {
            case JsType.Empty:
                return JsHostValue.Missing;

            case JsType.Undefined:
                return JsHostValue.Undefined;

            case JsType.Null:
                return JsHostValue.Null;

            case JsType.Boolean:
                return JsHostValue.Boolean(value.AsNumber() != 0);

            case JsType.Number:
                return JsHostValue.Number(value.AsNumber());

            case JsType.String:
                return JsHostValue.String(value.AsString());

            case JsType.Symbol:
                return new JsHostValue(JsHostValueKind.Symbol, 0, RefFor(value.AsSymbol()));

            // A BIGINT CROSSES AS ITS INTEGER (card B06, JSD-0024 section 19), charged per word
            // like every other crossing whose size the guest controls. Nothing is copied: the
            // integer is immutable and the embedder's value shares its digits.
            case JsType.BigInt:
            {
                var integer = value.AsBigInt();
                engine.Charge(JsBigInt.LinearCost(integer.Words));
                return JsHostValue.BigInt(integer.Value);
            }

            default:
            {
                var target = value.AsObject();

                var kind = target is JsArray
                    ? JsHostValueKind.Array
                    : target.IsCallable ? JsHostValueKind.Function : JsHostValueKind.Object;

                return new JsHostValue(kind, 0, RefFor(target));
            }
        }
    }

    /// <summary>Resolves an embedder's value back, refusing one another realm minted.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F7AD59
    // Broiler-Falsified-If: a value minted by another realm resolves rather than being refused
    // Broiler-Human:        PENDING
    internal JsValue Unwrap(JsHostValue value)
    {
        switch (value.Kind)
        {
            // MISSING IS THE HOST'S "NO VALUE" AND NEVER THE REALM'S (JSD-0024 section 20). The
            // realm's own "no value" is the marker an uninitialised binding holds, and a guest that
            // met it where a value belongs threw a dead-zone error from a place with no binding. A
            // member that requires a value refuses Missing before it gets here (RequireValue); what
            // reaches this line is a value the host answers or only lets be read - a body's return,
            // a hook's answer, a receiver, a conversion - and there the language's absent value is
            // `undefined`.
            case JsHostValueKind.Missing:
                return JsValue.Undefined;

            case JsHostValueKind.Undefined:
                return JsValue.Undefined;

            case JsHostValueKind.Null:
                return JsValue.Null;

            case JsHostValueKind.Boolean:
                return JsValue.Boolean(value.RawNumber != 0);

            case JsHostValueKind.Number:
                return JsValue.Number(value.RawNumber);

            case JsHostValueKind.String:
                return JsValue.String((string)value.RawReference!);

            case JsHostValueKind.BigInt:
                return JsValue.BigInt(AdmitBigInt((System.Numerics.BigInteger)value.RawReference!));

            default:
            {
                if (value.RawReference is not JsHostRef handle)
                {
                    throw new JsHostSurfaceException(
                        JsHostRefusal.NotAnObject, "the value names no guest object");
                }

                if (!ReferenceEquals(handle.Realm, this))
                {
                    throw new JsHostSurfaceException(
                        JsHostRefusal.ForeignRealm,
                        "the value was minted by a different realm and means nothing in this one");
                }

                return handle.Target is JsSymbol symbol
                    ? JsValue.Symbol(symbol)
                    : JsValue.Object((JsObject)handle.Target);
            }
        }
    }

    /// <summary>
    /// An embedder's BigInt as this realm's value: refused where the composition declined the
    /// surface, a <c>RangeError</c> past the ceiling before anything is allocated, and charged per
    /// word (card B06).
    /// </summary>
    /// <remarks>
    /// <b>The refusals are the realm's own exceptions, raised where <see cref="Unwrap"/> is.</b>
    /// Inside a crossing's <c>try</c> they become the <see cref="JsHostThrowException"/> or the
    /// latched termination every crossing answers; inside a host body's return they reach the guest
    /// as the <c>RangeError</c> it can catch. The ceiling is the one the language's own operations
    /// answer past (<see cref="JsBigInt.MaximumBits"/>), measured from the integer's length without
    /// reading its digits.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=EABD9A
    // Broiler-Falsified-If: a BigInt wider than JsBigInt.MaximumBits reaches the realm, or one reaches a realm whose composition declined the surface
    // Broiler-Human:        PENDING
    private JsBigInt AdmitBigInt(System.Numerics.BigInteger value)
    {
        if (engine.Realm.BigIntPrototype is null)
        {
            throw new JsHostSurfaceException(
                JsHostRefusal.SurfaceDeclined,
                "this realm's composition declined the BigInt surface, so a BigInt means nothing in it");
        }

        var bits = value.GetBitLength();

        if (bits > JsBigInt.MaximumBits)
        {
            throw engine.Error(
                "RangeError",
                "Maximum BigInt size exceeded: the host's value is wider than " +
                JsBigInt.MaximumBits.ToString(System.Globalization.CultureInfo.InvariantCulture) + " bits");
        }

        engine.Charge(JsBigInt.LinearCost(JsBigInt.WordsOf(bits)));
        return new JsBigInt(value);
    }

    /// <summary>
    /// <see cref="Unwrap"/> for a crossing that resolves a value before its own <c>try</c>: a guest
    /// throw or an abort the conversion raises is translated as the crossing's would be.
    /// </summary>
    /// <remarks>
    /// Only a BigInt's conversion can raise either (card B06); every other kind resolves or is
    /// refused with a <see cref="JsHostSurfaceException"/>, as before.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=6B2764
    // Broiler-Falsified-If: a JsThrow or a JsAbort escapes a public crossing untranslated
    // Broiler-Human:        PENDING
    private JsValue UnwrapAtCrossing(JsHostValue value)
    {
        try
        {
            return Unwrap(value);
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// Refuses <see cref="JsHostValue.Missing"/> where a member requires a value (JSD-0024 section
    /// 20), naming the parameter.
    /// </summary>
    /// <remarks>
    /// It is an <see cref="System.ArgumentException"/>, as <see cref="DetachClone"/> and a promise
    /// settlement already answered, because presenting "no value" where one is required is the
    /// embedder's mistake and not the guest's: nothing is thrown into the realm and nothing is written.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0893AA
    // Broiler-Human:        PENDING
    private static void RequireValue(JsHostValue value, string parameter)
    {
        if (value.IsMissing)
        {
            throw new System.ArgumentException(
                "Missing is not a value; pass JsHostValue.Undefined for undefined", parameter);
        }
    }

    /// <summary><see cref="RequireValue"/> over every value of a list, before any is resolved.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=86C6FA
    // Broiler-Human:        PENDING
    private static void RequireValues(System.ReadOnlySpan<JsHostValue> values, string parameter)
    {
        foreach (var value in values)
        {
            RequireValue(value, parameter);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3DF541
    // Broiler-Human:        PENDING
    private JsValue[] UnwrapAll(System.ReadOnlySpan<JsHostValue> arguments)
    {
        if (arguments.Length == 0)
        {
            return System.Array.Empty<JsValue>();
        }

        var resolved = new JsValue[arguments.Length];

        for (var at = 0; at < arguments.Length; at++)
        {
            resolved[at] = Unwrap(arguments[at]);
        }

        return resolved;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6329D1
    // Broiler-Human:        PENDING
    private JsHostRef RefFor(JsObject target) =>
        objects.GetValue(target, key => new JsHostRef(this, key));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B72A3F
    // Broiler-Human:        PENDING
    private JsHostRef RefFor(JsSymbol target) =>
        symbols.GetValue(target, key => new JsHostRef(this, key));

    /// <summary>
    /// Whether this realm is installing a member of its own, rather than a guest assigning to one.
    /// </summary>
    /// <remarks>
    /// <b>An exotic object has to tell those two apart and cannot see the difference from the
    /// write.</b> Both arrive at the same property-storage member; one is an embedder saying what
    /// the object HAS and the other is a guest writing THROUGH it, and routing the first to the
    /// named-property handler makes an embedder unable to install a member whose name its own
    /// handler happens to claim. That is not hypothetical: an interface with an <c>item</c> method
    /// on a collection whose contents include something called <c>item</c> is the ordinary case.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=DBCA8A
    // Broiler-Falsified-If: a member the realm installs reaches an exotic handler as an assignment
    // Broiler-Human:        PENDING
    internal bool Installing { get; private set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=4F42DB
    // Broiler-Human:        PENDING
    private InstallScope Install() => new(this);

    /// <summary>Marks one install for as long as it is held.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=485CB3
    // Broiler-Human:        PENDING
    private readonly struct InstallScope : System.IDisposable
    {
        private readonly JsHostRealm realm;

        private readonly bool outer;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=484F70
        // Broiler-Human:        PENDING
        internal InstallScope(JsHostRealm owner)
        {
            realm = owner;
            outer = owner.Installing;
            owner.Installing = true;
        }

        /// <summary>Restores rather than clears, so a nested install leaves the outer one marked.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C94409
        // Broiler-Human:        PENDING
        public void Dispose() => realm.Installing = outer;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=132F2A
    // Broiler-Human:        PENDING
    private JsObject ObjectOf(JsHostValue value)
    {
        var resolved = Unwrap(value);

        return resolved.AsObjectOrNull() ?? throw new JsHostSurfaceException(
            JsHostRefusal.NotAnObject, "the value presented is not a guest object");
    }

    /// <summary>What the guest wrote after <c>new</c>, inside a host construct body.</summary>
    /// <remarks>
    /// <b>It is meaningful inside a construct body and nowhere else</b>, where it answers
    /// <see cref="JsHostValue.Missing"/> - which is the same distinction the language draws, since
    /// <c>new.target</c> is <c>undefined</c> in an ordinary call. It is a property of the realm
    /// rather than a parameter because the body signature is shared with the ordinary call path,
    /// and growing that signature would have grown it for every host function an embedder writes to
    /// serve the few that construct.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5C434A
    // Broiler-Human:        PENDING
    public JsHostValue NewTarget { get; private set; }

    /// <summary>Turns an embedder's constructor body into one the interpreter can construct.</summary>
    /// <remarks>
    /// It saves and restores the previous new target rather than clearing it, so a constructor that
    /// constructs another one leaves the outer body's answer intact when the inner returns.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=1A9B56
    // Broiler-Falsified-If: a nested construction leaves the outer body reading the inner target
    // Broiler-Human:        PENDING
    private JsNativeBody BindConstructor(JsHostFunction body)
    {
        var call = Bind(body);

        return (owner, newTarget, arguments) =>
        {
            // THE RECEIVER IS MADE HERE, WHICH IS WHAT MAKES THIS A CONSTRUCTION RATHER THAN A CALL.
            // The profile's own built-in constructors make and return their object, because each
            // knows what it is building; an embedder's does not - it is describing an interface, and
            // what the language does for a scripted constructor is create the object from the new
            // target's prototype and hand it over as `this`. Doing that here is what lets an
            // embedder write the body it would write in JavaScript.
            var prototype = newTarget.IsObject
                ? engine.GetProperty(newTarget, "prototype")
                : JsValue.Undefined;

            var instance = new JsObject(
                prototype.AsObjectOrNull() ?? engine.Realm.ObjectPrototype);

            var outer = NewTarget;
            NewTarget = Wrap(newTarget);

            try
            {
                // AN OBJECT THE BODY RETURNS WINS OVER THE ONE MADE HERE, which is the language's
                // own rule for a constructor's return value and the door an embedder needs when the
                // thing it is constructing already exists - a wrapper for a node it did not just
                // create.
                var answered = call(owner, JsValue.Object(instance), arguments);

                return answered.IsObject ? answered : JsValue.Object(instance);
            }
            finally
            {
                NewTarget = outer;
            }
        };
    }

    /// <summary>Turns an embedder's body into one the interpreter can call.</summary>
    /// <remarks>
    /// <b>An exception the embedder raises becomes a guest throw here and nowhere else.</b> A
    /// <see cref="JsHostThrowException"/> carries a value the embedder built through this realm, so
    /// it is re-thrown as the guest throw it already is; anything else is a defect on the host side
    /// and reaches the guest as a <c>TypeError</c> naming it, because a CLR exception unwinding
    /// through interpreter frames would leave the operand stack in a state no <c>catch</c> in the
    /// guest could reason about.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=9EA8DF
    // Broiler-Falsified-If: a host body's CLR exception unwinds through interpreter frames uncaught
    // Broiler-Human:        PENDING
    private JsNativeBody Bind(JsHostFunction body) =>
        (owner, thisValue, arguments) =>
        {
            var projected = arguments.Length == 0
                ? System.Array.Empty<JsHostValue>()
                : new JsHostValue[arguments.Length];

            for (var at = 0; at < arguments.Length; at++)
            {
                projected[at] = Wrap(arguments[at]);
            }

            try
            {
                return Unwrap(body(this, Wrap(thisValue), projected));
            }
            catch (JsHostThrowException raised)
            {
                throw Raised(raised);
            }
            catch (JsHostTerminatedException)
            {
                throw latched ?? new JsAbort(
                    JsAbortKind.InternalDefect, "a host body reported a termination that was not latched");
            }
            catch (JsHostSurfaceException refusal)
            {
                throw owner.Error("TypeError", refusal.Message);
            }
        };

    // ---- structured clone, internally -----------------------------------------------------------

    /// <summary>Serializes a guest value into a detached clone carrier. Internal: see JSD-0032.</summary>
    /// <remarks>
    /// <para>
    /// <b>Not public, and deliberately so.</b> The public door is <see cref="DetachClone"/> (card
    /// I17), which answers the opaque <see cref="JsHostCloneCarrier"/> at the carrier's own bounds.
    /// This member hands out the internal carrier and accepts narrower bounds; its only caller is
    /// the slice-compiler composition's clone checks, which bind it with an unsafe accessor because
    /// rule A10 forbids opening the profile's internals, rule A11 forbids a test project to
    /// reference it, and rule B5 forbids a reflective call.
    /// </para>
    /// <para>
    /// A guest throw from a getter reaches the caller as the <see cref="JsHostThrowException"/> it
    /// would be anywhere on this surface; a value the matrix refuses reaches it as a thrown guest
    /// <c>TypeError</c> whose message begins <c>DataCloneError:</c>, because this realm has no
    /// <c>DOMException</c> to throw instead.
    /// </para>
    /// <para>
    /// The two bounds may only narrow the carrier's own <see cref="JsCloneCarrier.MaxEntries"/> and
    /// <see cref="JsCloneCarrier.MaxBytes"/>, never widen them; a check passes small ones so that the
    /// refusal at the bound can be reached without building a graph of four million entries.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=BCF1B2
    // Broiler-Falsified-If: a serialization runs outside a step, or a refusal reaches host code as anything but a guest throw
    // Broiler-Human:        PENDING
    internal JsCloneCarrier CloneSerialize(JsHostValue value, long maxEntries, long maxBytes)
    {
        Enter(1);

        try
        {
            return engine.Realm.CloneSerialize(Unwrap(value), maxEntries, maxBytes);
        }
        catch (JsCloneRefusedException refused)
        {
            throw Error(JsHostErrorKind.TypeError, "DataCloneError: " + refused.Message);
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// Serializes a guest value and moves the bytes of every listed <c>ArrayBuffer</c> into the
    /// carrier. Internal: see JSD-0032 section 5a.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>All or nothing.</b> The list is checked (every entry an attached, fixed-length
    /// <c>ArrayBuffer</c>, none twice), the graph is serialized, and the listed buffers are checked
    /// again and measured against the bounds; only then is any of them detached. A refusal, a
    /// guest throw or an abort at any of those points leaves every listed buffer attached. Once the
    /// call returns, every listed buffer is detached, reachable from the value or not.
    /// </para>
    /// <para>
    /// The list is a host array because a host hands it over: turning a guest iterable into one is
    /// the caller's step, as WebIDL's sequence conversion is the caller's step in HTML.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=2B215E
    // Broiler-Falsified-If: a refused transfer leaves any listed buffer detached, or a completed one leaves any attached
    // Broiler-Human:        PENDING
    internal JsCloneCarrier CloneSerializeWithTransfer(
        JsHostValue value,
        JsHostValue[] transfer,
        long maxEntries,
        long maxBytes)
    {
        Enter(1);

        try
        {
            var list = new JsValue[transfer.Length];

            for (var at = 0; at < transfer.Length; at++)
            {
                list[at] = Unwrap(transfer[at]);
            }

            return engine.Realm.CloneSerializeWithTransfer(Unwrap(value), list, maxEntries, maxBytes);
        }
        catch (JsCloneRefusedException refused)
        {
            throw Error(JsHostErrorKind.TypeError, "DataCloneError: " + refused.Message);
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>Rebuilds a carrier's graph in this realm. Internal: see JSD-0032.</summary>
    /// <remarks>
    /// The carrier may have come from this realm or from any other realm of this profile, live or
    /// already disposed: it holds data and no object of the realm that produced it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=28EF4E
    // Broiler-Falsified-If: a deserialization runs outside a step, or answers an object built on another realm's intrinsics
    // Broiler-Human:        PENDING
    internal JsHostValue CloneDeserialize(JsCloneCarrier carrier)
    {
        Enter(1);

        try
        {
            return Wrap(engine.Realm.CloneDeserialize(carrier));
        }
        catch (JsCloneRefusedException refused)
        {
            throw Error(JsHostErrorKind.TypeError, "DataCloneError: " + refused.Message);
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    // ---- structured clone across realms and threads (card I17) --------------------------------

    /// <summary>
    /// Structured-clones <paramref name="value"/> into a carrier that belongs to no realm, moving
    /// the bytes of every <c>ArrayBuffer</c> in <paramref name="transfer"/>: the sending half of a
    /// message to a realm on another thread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It runs here, on this realm's thread, inside a step</b>, and the carrier it answers holds
    /// no object of this realm, so the host may hand it to any thread (see
    /// <see cref="JsHostCloneCarrier"/> for the lifetime, thread and compatibility rules). The walk
    /// is decision JSD-0032's: getters run, in HTML's order, and a throw from one reaches the caller
    /// as the <see cref="JsHostThrowException"/> it is anywhere on this surface. A value outside the
    /// supported matrix, a bad transfer list, or a graph past the carrier's bounds is thrown as a
    /// guest <c>TypeError</c> whose message begins <c>DataCloneError:</c>, because this realm has no
    /// <c>DOMException</c>; the sending side is where HTML puts that error.
    /// </para>
    /// <para>
    /// <b>All or nothing, and paid for by this realm.</b> The carrier's <c>LiveBytes</c> charge
    /// (<see cref="JsHostCloneCarrier.ChargedBytes"/>) is taken before any listed buffer is
    /// detached, so a refusal, a guest throw, a spent allowance or a refused ceiling leaves every
    /// listed buffer attached; once this returns, every listed buffer is detached.
    /// </para>
    /// <para>
    /// <b><see cref="JsHostValue.Missing"/> is refused</b> with an <see cref="System.ArgumentException"/>,
    /// as the value or as an entry of <paramref name="transfer"/>, before anything is serialized or
    /// detached. It is not a value the language has, and a message carrying it would arrive as
    /// <c>undefined</c> - a guess at what the embedder meant.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=37F58D
    // Broiler-Falsified-If: a carrier is made outside a step, holds an object of this realm, or leaves a listed buffer attached after returning
    // Broiler-Human:        PENDING
    public JsHostCloneCarrier DetachClone(JsHostValue value, System.ReadOnlySpan<JsHostValue> transfer = default)
    {
        Enter(1 + (ulong)transfer.Length);

        if (value.IsMissing)
        {
            throw new System.ArgumentException(
                "Missing is not a value to clone; pass JsHostValue.Undefined for undefined", nameof(value));
        }

        foreach (var entry in transfer)
        {
            if (entry.IsMissing)
            {
                throw new System.ArgumentException(
                    "Missing is not a transferable value", nameof(transfer));
            }
        }

        try
        {
            var list = new JsValue[transfer.Length];

            for (var at = 0; at < transfer.Length; at++)
            {
                list[at] = Unwrap(transfer[at]);
            }

            return new JsHostCloneCarrier(engine.Realm.CloneSerializeWithTransfer(
                Unwrap(value), list, JsCloneCarrier.MaxEntries, JsCloneCarrier.MaxBytes));
        }
        catch (JsCloneRefusedException refused)
        {
            throw Error(JsHostErrorKind.TypeError, "DataCloneError: " + refused.Message);
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    /// <summary>
    /// Builds the graph a carrier holds in this realm, out of this realm's own intrinsics: the
    /// receiving half of a message from a realm on another thread.
    /// </summary>
    /// <param name="carrier">
    /// What <see cref="DetachClone"/> answered, in this realm or any other realm of this profile
    /// build, live or disposed. It is typed as <see cref="object"/> because a host carries whatever
    /// its engines mint; anything that is not a <see cref="JsHostCloneCarrier"/> of this build is
    /// refused with <see cref="JsHostRefusal.ForeignCarrier"/>.
    /// </param>
    /// <remarks>
    /// <para>
    /// <b>It runs here, on this realm's thread, inside a step</b>, and runs no guest code: every
    /// object is new, every prototype is this realm's, and objects the source shared or linked in
    /// a cycle are shared and linked again among the new ones. This realm pays the fuel for the
    /// rebuild and <c>LiveBytes</c> for every buffer.
    /// </para>
    /// <para>
    /// <b>Refusals.</b> A foreign object is <see cref="JsHostRefusal.ForeignCarrier"/>; a
    /// single-use carrier already claimed is <see cref="JsHostRefusal.CarrierConsumed"/>. Both are
    /// the host's mistakes and are raised as <see cref="JsHostSurfaceException"/>, before anything
    /// is claimed, built or charged beyond the crossing. A crossing outside a step or on another
    /// thread is refused with <see cref="JsHostRefusal.RealmNotCurrent"/> and claims nothing.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=1EC4C5
    // Broiler-Falsified-If: an adoption answers an object on another realm's intrinsics, accepts a foreign carrier, or adopts a single-use carrier twice
    // Broiler-Human:        PENDING
    public JsHostValue AdoptClone(object carrier)
    {
        Enter(1);

        if (carrier is null)
        {
            throw new System.ArgumentNullException(nameof(carrier));
        }

        // THE COMPATIBILITY RULE. The type is this build's (a second copy of the assembly has its
        // own), and the profile and layout are asked anyway, so that a later build which admits an
        // older layout has one place to say so.
        if (carrier is not JsHostCloneCarrier held
            || held.Profile != JavaScriptProfile.Id
            || held.FormatVersion != JsHostCloneCarrier.CurrentFormatVersion)
        {
            throw new JsHostSurfaceException(
                JsHostRefusal.ForeignCarrier,
                "the carrier was not minted by a realm of this profile build");
        }

        // THE HOST'S MISTAKE, NOT THE GUEST'S: asked before the claim, so that it can be told apart
        // from the race the claim decides, which the catch below reports the same way.
        if (held.IsConsumed)
        {
            throw new JsHostSurfaceException(
                JsHostRefusal.CarrierConsumed, "the carrier's transferred buffers were already adopted");
        }

        try
        {
            return Wrap(engine.Realm.CloneDeserialize(held.Graph));
        }
        catch (JsCloneRefusedException refused) when (refused.Reason is JsCloneRefusal.Consumed)
        {
            throw new JsHostSurfaceException(JsHostRefusal.CarrierConsumed, refused.Message);
        }
        catch (JsCloneRefusedException refused)
        {
            throw Error(JsHostErrorKind.TypeError, "DataCloneError: " + refused.Message);
        }
        catch (JsThrow thrown)
        {
            throw Thrown(thrown);
        }
        catch (JsAbort abort)
        {
            throw Latch(abort);
        }
    }

    // ---- the gate ------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=48F054
    // Broiler-Falsified-If: a crossing proceeds while the realm is outside a step or on another thread
    // Broiler-Human:        PENDING
    private void Enter(ulong units)
    {
        if (latched is not null)
        {
            throw new JsHostTerminatedException(
                "the operation ended underneath this call and no host code may continue");
        }

        if (stepDepth <= 0 || System.Environment.CurrentManagedThreadId != guestThreadId)
        {
            throw new JsHostSurfaceException(
                JsHostRefusal.RealmNotCurrent,
                "the realm was touched outside a step of the instance that owns it");
        }

        engine.ChargeHostCrossing(units);
    }

    /// <remarks>
    /// <para>
    /// <b>A thrown BigInt reaches the host as itself</b> (card B06): a
    /// <see cref="JsHostThrowException"/> whose <see cref="JsHostThrowException.Thrown"/> is the
    /// BigInt. Until B06 it reached the host as a <c>TypeError</c> standing in for it (decision
    /// JSD-0033), because the surface had no kind to carry it.
    /// </para>
    /// <para>
    /// <b>The one crossing <see cref="Wrap"/> charges for is a BigInt's words</b>, and this runs
    /// inside the catch clause every crossing ends in, where an abort the charge raised would escape
    /// past the sibling clause meant for it. So a spent allowance here answers the latched
    /// termination, as it would anywhere else in the crossing.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=871C26
    // Broiler-Falsified-If: a guest throw of any value reaches host code as anything but a JsHostThrowException or the latched termination
    // Broiler-Human:        PENDING
    private System.Exception Thrown(JsThrow raised)
    {
        try
        {
            return new JsHostThrowException(Wrap(raised.Value), raised.Message);
        }
        catch (JsAbort abort)
        {
            return Latch(abort);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=F6EE30
    // Broiler-Falsified-If: an abort reaches host code as anything a catch clause can clear
    // Broiler-Human:        PENDING
    private JsHostTerminatedException Latch(JsAbort abort)
    {
        latched ??= abort;

        return new JsHostTerminatedException(
            "the operation ended underneath this call and no host code may continue");
    }

    /// <summary>The engine's attribute bits for an embedder's flags.</summary>
    /// <remarks>
    /// The two sets carry the same three questions in a different order, so this is a permutation
    /// rather than a translation - and it is written out rather than cast, because two enumerations
    /// that happen to agree today are two enumerations.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=444542
    // Broiler-Human:        PENDING
    private static JsPropertyAttributes Attributes(JsHostPropertyFlags flags)
    {
        var attributes = JsPropertyAttributes.None;

        if ((flags & JsHostPropertyFlags.Enumerable) != 0)
        {
            attributes |= JsPropertyAttributes.Enumerable;
        }

        if ((flags & JsHostPropertyFlags.Configurable) != 0)
        {
            attributes |= JsPropertyAttributes.Configurable;
        }

        if ((flags & JsHostPropertyFlags.Writable) != 0)
        {
            attributes |= JsPropertyAttributes.Writable;
        }

        return attributes;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=98F889
    // Broiler-Human:        PENDING
    private static string NameOf(JsHostErrorKind kind) => kind switch
    {
        JsHostErrorKind.TypeError => "TypeError",
        JsHostErrorKind.RangeError => "RangeError",
        JsHostErrorKind.SyntaxError => "SyntaxError",
        JsHostErrorKind.ReferenceError => "ReferenceError",
        _ => "Error",
    };
}
