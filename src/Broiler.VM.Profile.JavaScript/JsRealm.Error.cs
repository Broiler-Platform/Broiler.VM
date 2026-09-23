// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           1
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The Error intrinsics: <c>Error</c>, <c>Error.prototype</c>, the six native subtypes,
/// <c>AggregateError</c> and <c>SuppressedError</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every constructor is recorded in <c>ErrorConstructors</c>, and that registration is the whole
/// point of this file.</b> When the engine raises a <c>TypeError</c> of its own it calls
/// <c>CreateError("TypeError", …)</c>, which looks the kind up in that dictionary and builds the
/// error on <em>that</em> constructor's <c>prototype</c>. A subtype that is built but not registered
/// still exists as a global, and guest code that writes <c>catch (e) { e instanceof TypeError }</c>
/// around an engine-raised error gets <c>false</c> - the error came off <c>Error.prototype</c>
/// instead. The conformance harness compares <c>thrown.constructor</c> by identity, so the
/// dictionary is what makes an engine-raised error and a guest-constructed one the same kind.
/// </para>
/// <para>
/// <b><c>Error.prototype.name</c> is writable and configurable, not frozen.</b> Subclassing an error
/// in guest code is done by assigning <c>name</c> on the derived prototype, and a frozen
/// <c>name</c> silently drops that assignment in sloppy mode and throws in strict mode. The
/// specification's attribute set is the ordinary built-in one and this uses it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>Builds the Error constructor, its prototype and the six native subtypes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7C4D41
    // Broiler-Human:        PENDING
    private void SetupError()
    {
        var basePrototype = ErrorPrototype;

        basePrototype.DefineBuiltIn("name", JsValue.String("Error"));
        basePrototype.DefineBuiltIn("message", JsValue.String(string.Empty));

        Method(basePrototype, "toString", 0, static (engine, thisValue, _) =>
        {
            if (!thisValue.IsObject)
            {
                return engine.ThrowTypeError("Error.prototype.toString called on a non-object");
            }

            var namePart = engine.GetProperty(thisValue, "name");
            var messagePart = engine.GetProperty(thisValue, "message");

            var name = namePart.Type == JsType.Undefined
                ? "Error"
                : engine.ToStringValue(namePart);

            var message = messagePart.Type == JsType.Undefined
                ? string.Empty
                : engine.ToStringValue(messagePart);

            engine.Charge((ulong)(name.Length + message.Length) + 1);

            // AN EMPTY NAME YIELDS THE MESSAGE ALONE AND AN EMPTY MESSAGE THE NAME ALONE.
            // Joining unconditionally produces the trailing ": " that a bare `new Error()` would
            // otherwise render, which is the one output every reader of a stack trace notices.
            if (name.Length == 0)
            {
                return JsValue.String(message);
            }

            return JsValue.String(message.Length == 0 ? name : name + ": " + message);
        });

        JsNativeBody baseBody = (engine, _, arguments) => JsValue.Object(
            ErrorIntrinsicCreate(
                engine,
                basePrototype,
                ErrorIntrinsicArg(arguments, 0),
                ErrorIntrinsicArg(arguments, 1)));

        // `Error(message)` and `new Error(message)` build the same thing: the constructor is one of
        // the handful the specification says may be called without `new`.
        var baseConstructor = Constructor("Error", 1, basePrototype, baseBody, baseBody);

        ErrorConstructors["Error"] = baseConstructor;

        ErrorIntrinsicInstall("EvalError", baseConstructor);
        ErrorIntrinsicInstall("RangeError", baseConstructor);
        ErrorIntrinsicInstall("ReferenceError", baseConstructor);
        ErrorIntrinsicInstall("SyntaxError", baseConstructor);
        ErrorIntrinsicInstall("TypeError", baseConstructor);
        ErrorIntrinsicInstall("URIError", baseConstructor);
        ErrorIntrinsicInstallAggregate(baseConstructor);
        ErrorIntrinsicInstallSuppressed(baseConstructor);
    }

    /// <summary><c>SuppressedError.prototype</c>, which disposal builds its combined errors on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AB9227
    // Broiler-Human:        PENDING
    internal JsObject SuppressedErrorPrototype { get; private set; } = null!;

    /// <summary>Builds <c>SuppressedError</c>, the other subtype with a shape of its own.</summary>
    /// <remarks>
    /// <para>
    /// <b>Its arguments are the error, the error it suppressed, and only then the message</b>, and
    /// there is no options bag: a disposal that fails while another failure is already in flight has
    /// two reasons and no one to prefer, so both are kept as payload and the message is an
    /// afterthought. The message is converted FIRST and the two payloads are defined after it, which
    /// is the own-key order a program walking the object sees.
    /// </para>
    /// <para>
    /// <b>The payloads are stored exactly as given.</b> Neither is converted, wrapped or required to
    /// be an Error, so a thrown string or a thrown <c>undefined</c> comes back out of a disposal
    /// chain as itself.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=48386B
    // Broiler-Human:        PENDING
    private void ErrorIntrinsicInstallSuppressed(JsNativeFunction baseConstructor)
    {
        var prototype = new JsObject(ErrorPrototype);

        prototype.DefineBuiltIn("name", JsValue.String("SuppressedError"));
        prototype.DefineBuiltIn("message", JsValue.String(string.Empty));

        JsNativeBody body = (engine, _, arguments) =>
        {
            var error = ErrorIntrinsicCreate(
                engine, prototype, ErrorIntrinsicArg(arguments, 2), JsValue.Undefined);

            error.DefineBuiltIn("error", ErrorIntrinsicArg(arguments, 0));
            error.DefineBuiltIn("suppressed", ErrorIntrinsicArg(arguments, 1));
            return JsValue.Object(error);
        };

        var constructor = Constructor("SuppressedError", 3, prototype, body, body);
        constructor.Prototype = baseConstructor;
        ErrorConstructors["SuppressedError"] = constructor;
        SuppressedErrorPrototype = prototype;
    }

    /// <summary>
    /// The specification's "newly created SuppressedError object" that disposal combines two throws
    /// into: no message, the new throw as <c>error</c> and the one already in flight as
    /// <c>suppressed</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=593B85
    // Broiler-Human:        PENDING
    internal JsValue NewSuppressedError(JsEngine engine, JsValue error, JsValue suppressed)
    {
        engine.Charge(4);

        var made = new JsObject(SuppressedErrorPrototype, "Error");
        made.DefineBuiltIn("error", error);
        made.DefineBuiltIn("suppressed", suppressed);
        return JsValue.Object(made);
    }

    /// <summary>Builds <c>AggregateError</c>, which is the one subtype with a different shape.</summary>
    /// <remarks>
    /// <para>
    /// <b>Its first argument is the errors and its second is the message</b>, where every other
    /// subtype takes the message first. That is not a wart of this implementation: the type exists
    /// for <c>Promise.any</c>, which has a LIST of reasons and no one reason to report, so the list
    /// is the argument that could not be left out.
    /// </para>
    /// <para>
    /// <b>The list is read through the iteration protocol</b>, like every other list argument in the
    /// language — so a Set of errors is as good as an Array of them — and it lands as an own
    /// <c>errors</c> property that is writable and configurable and not enumerable, which is what a
    /// program that walks the object sees and what one that reassigns it may do.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A9A485
    // Broiler-Human:        PENDING
    private void ErrorIntrinsicInstallAggregate(JsNativeFunction baseConstructor)
    {
        var prototype = new JsObject(ErrorPrototype);

        prototype.DefineBuiltIn("name", JsValue.String("AggregateError"));
        prototype.DefineBuiltIn("message", JsValue.String(string.Empty));

        JsNativeBody body = (engine, _, arguments) =>
        {
            var error = ErrorIntrinsicCreate(
                engine,
                prototype,
                ErrorIntrinsicArg(arguments, 1),
                ErrorIntrinsicArg(arguments, 2));

            var collected = NewArray();

            foreach (var reason in CollectionElements(engine, ErrorIntrinsicArg(arguments, 0)))
            {
                engine.Charge(1);
                collected.Push(reason);
            }

            error.DefineBuiltIn("errors", JsValue.Object(collected));
            return JsValue.Object(error);
        };

        var constructor = Constructor("AggregateError", 2, prototype, body, body);
        constructor.Prototype = baseConstructor;
        ErrorConstructors["AggregateError"] = constructor;
    }

    /// <summary>Reads one argument, which may not have been supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4B1E16
    // Broiler-Human:        PENDING
    private static JsValue ErrorIntrinsicArg(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>
    /// Builds one error object on <paramref name="prototype"/>, the specification's
    /// <c>OrdinaryCreateFromConstructor</c> followed by the message and cause installation.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=86BA81
    // Broiler-Human:        PENDING
    private static JsObject ErrorIntrinsicCreate(
        JsEngine engine, JsObject prototype, JsValue message, JsValue options)
    {
        engine.Charge(4);

        var error = new JsObject(prototype, "Error");

        // A MISSING MESSAGE LEAVES NO OWN PROPERTY AT ALL, so `new Error().message` reads the
        // empty string off the prototype and `hasOwnProperty("message")` answers false. Defining
        // an own empty string instead is observable, and test262 looks at exactly that.
        if (message.Type != JsType.Undefined)
        {
            var text = engine.ToStringValue(message);
            engine.Charge((ulong)text.Length);
            error.DefineBuiltIn("message", JsValue.String(text));
        }

        if (options.IsObject && options.AsObject().HasOwnProperty("cause"))
        {
            error.DefineBuiltIn("cause", engine.GetProperty(options, "cause"));
        }

        return error;
    }

    /// <summary>Builds one native subtype: its prototype, its constructor and its registration.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EB3C11
    // Broiler-Human:        PENDING
    private void ErrorIntrinsicInstall(string name, JsNativeFunction baseConstructor)
    {
        var prototype = new JsObject(ErrorPrototype);

        prototype.DefineBuiltIn("name", JsValue.String(name));
        prototype.DefineBuiltIn("message", JsValue.String(string.Empty));

        JsNativeBody body = (engine, _, arguments) => JsValue.Object(
            ErrorIntrinsicCreate(
                engine,
                prototype,
                ErrorIntrinsicArg(arguments, 0),
                ErrorIntrinsicArg(arguments, 1)));

        var constructor = Constructor(name, 1, prototype, body, body);

        // A SUBTYPE CONSTRUCTOR INHERITS FROM `Error`, not straight from `Function.prototype`.
        // `Object.getPrototypeOf(TypeError) === Error` is the specification's arrangement, and it
        // is what lets anything added to `Error` later be reached through every subtype.
        constructor.Prototype = baseConstructor;

        ErrorConstructors[name] = constructor;
    }
}
