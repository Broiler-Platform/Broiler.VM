// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The Error intrinsics: <c>Error</c>, <c>Error.prototype</c> and the six native subtypes.
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1D9793
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5161C6
    // Broiler-Human:        PENDING
    private void ErrorIntrinsicInstall(string name, JsNativeFunction baseConstructor)
    {
        var prototype = new JsObject(ErrorPrototype, "Error");

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
