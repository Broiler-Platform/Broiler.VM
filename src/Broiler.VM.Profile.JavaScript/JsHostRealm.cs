// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   32
// Annotated:        32/32
// Exempt:           6
// Human-reviewed:   0/32
// IP risk:          Low
// Security risk:    High
// Criteria:         8/8
// Resource impact:  4/10 max
// Unverified:       32
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CEA257
    // Broiler-Human:        PENDING
    public JsHostValue NewArray(System.ReadOnlySpan<JsHostValue> elements = default)
    {
        Enter(4 + (ulong)elements.Length);

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

    /// <summary>A new constructable host function, carrying a readable <c>prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7E7022
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
            var bound = Bind(body);
            var prototype = new JsObject(engine.Realm.ObjectPrototype);
            var function = new JsNativeFunction(
                engine.Realm.FunctionPrototype, name ?? string.Empty, length, bound, bound);

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

    /// <summary>The abstract operation <c>ToString</c>. May run guest code, and may throw.</summary>
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1A325F
    // Broiler-Human:        PENDING
    public void DefineValue(JsHostValue target, string name, JsHostValue value)
    {
        Enter(3);

        try
        {
            ObjectOf(target).DefineOrdinary(name ?? string.Empty, Unwrap(value));
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=184C43
    // Broiler-Human:        PENDING
    public void DefineAccessor(
        JsHostValue target, string name, JsHostFunction getter, JsHostFunction? setter = null)
    {
        Enter(5);

        if (getter is null)
        {
            throw new System.ArgumentNullException(nameof(getter));
        }

        try
        {
            var key = name ?? string.Empty;

            ObjectOf(target).SetOwnProperty(
                key,
                JsProperty.Accessor(
                    engine.Realm.Native("get " + key, 0, Bind(getter)),
                    setter is null ? null : engine.Realm.Native("set " + key, 1, Bind(setter)),
                    JsPropertyAttributes.Configurable | JsPropertyAttributes.Enumerable));
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5E8CFD
    // Broiler-Human:        PENDING
    public JsHostValue GetProperty(JsHostValue target, string name)
    {
        Enter(3);

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E0AB43
    // Broiler-Human:        PENDING
    public void SetProperty(JsHostValue target, string name, JsHostValue value)
    {
        Enter(3);

        try
        {
            engine.SetProperty(Unwrap(target), name ?? string.Empty, Unwrap(value), strict: true);
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=4C7364
    // Broiler-Falsified-If: a guest call from host code skips the call-depth charge or lets a JsAbort escape as a guest throw
    // Broiler-Human:        PENDING
    public JsHostValue Invoke(
        JsHostValue function, JsHostValue thisValue, System.ReadOnlySpan<JsHostValue> arguments = default)
    {
        Enter(4 + (ulong)arguments.Length);

        var callee = Unwrap(function);

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=22E2B9
    // Broiler-Human:        PENDING
    public JsHostValue Construct(
        JsHostValue constructor, System.ReadOnlySpan<JsHostValue> arguments = default)
    {
        Enter(4 + (ulong)arguments.Length);

        var callee = Unwrap(constructor);

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

    // ---- conversion ----------------------------------------------------------------------------

    /// <summary>Names a guest value for an embedder, canonicalising an object's identity.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=26A22A
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=2E3384
    // Broiler-Falsified-If: a value minted by another realm resolves rather than being refused
    // Broiler-Human:        PENDING
    internal JsValue Unwrap(JsHostValue value)
    {
        switch (value.Kind)
        {
            case JsHostValueKind.Missing:
                return JsValue.Empty;

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

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=132F2A
    // Broiler-Human:        PENDING
    private JsObject ObjectOf(JsHostValue value)
    {
        var resolved = Unwrap(value);

        return resolved.AsObjectOrNull() ?? throw new JsHostSurfaceException(
            JsHostRefusal.NotAnObject, "the value presented is not a guest object");
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=79C96E
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
                throw new JsThrow(Unwrap(raised.Thrown), raised.Message);
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=492FCF
    // Broiler-Human:        PENDING
    private JsHostThrowException Thrown(JsThrow raised) =>
        new(Wrap(raised.Value), raised.Message);

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=F6EE30
    // Broiler-Falsified-If: an abort reaches host code as anything a catch clause can clear
    // Broiler-Human:        PENDING
    private JsHostTerminatedException Latch(JsAbort abort)
    {
        latched ??= abort;

        return new JsHostTerminatedException(
            "the operation ended underneath this call and no host code may continue");
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
