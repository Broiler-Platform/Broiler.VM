// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           1
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    High
// Criteria:         5/5
// Resource impact:  7/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The dynamic surface: <c>eval</c> and the <c>Function</c> constructor, and nothing else.
/// </summary>
/// <remarks>
/// <para>
/// <b>Neither of these compiles anything here.</b> Both turn a String into a request, hand it to
/// the mediator the core supplied for this invocation, and run whatever verified handle comes
/// back. The compiler that answers lives in the composition, behind an artifact-provider
/// capability, which is what keeps it inside a composition's declared Native AOT closure and keeps
/// this profile from reaching a filesystem, a socket, or a compiler on its own.
/// </para>
/// <para>
/// <b>The file exists at all so that a composition can decline exactly this.</b>
/// <c>broiler.javascript.dynamic</c> is a manifest identity of its own for the reason roadmap
/// section 6 gives, and the realm builds this file's contents only when the composition admitted
/// it. A composition that declined has already had every artifact naming the surface refused at
/// verification; a composition that admitted it and registered no provider gets a run-time error
/// the guest may catch. Two situations a reader experiences as the same and which are not.
/// </para>
/// <para>
/// <b>Dynamic <c>import()</c> is NOT here.</b> It belongs to the module goal, which this profile
/// does not have; the manifest's own scope names it, and admitting a form with no module records
/// behind it would be admitting a syntax rather than a surface.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=60DD8D
// Broiler-Falsified-If: anything in this file is built into a realm whose composition did not admit broiler.javascript.dynamic
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>The <c>eval</c> intrinsic, so a call site can ask whether it is calling it.</summary>
    /// <remarks>
    /// A direct <c>eval</c> is a fact about the spelling of a call site, and the executor is handed
    /// an opcode that says so — but the language is careful that the opcode is not enough: a
    /// program may assign to the global, and a call to whatever it now holds is an ordinary call
    /// however it is written. This is the identity the executor compares against.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=D093DD
    // Broiler-Falsified-If: this holds a function object the guest can reach under any other name
    // Broiler-Human:        PENDING
    internal JsObject? EvalIntrinsic { get; private set; }

    /// <summary>Whether <paramref name="value"/> is this realm's own <c>eval</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B458DE
    // Broiler-Falsified-If: it answers true for a function object this realm did not build as its own eval
    // Broiler-Human:        PENDING
    internal bool IsEvalIntrinsic(JsValue value) =>
        EvalIntrinsic is not null && value.IsObject && ReferenceEquals(value.AsObject(), EvalIntrinsic);

    /// <summary>Builds <c>eval</c> and <c>Function</c> on the global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=31E9B4
    // Broiler-Falsified-If: it installs a global that turns source into code without going through the mediator
    // Broiler-Human:        PENDING
    private void SetupDynamic()
    {
        var evaluate = Native(
            "eval",
            1,
            static (engine, thisValue, arguments) =>
            {
                _ = thisValue;

                // REACHED THROUGH THE VALUE RATHER THAN THROUGH THE SPELLING, so this is the
                // INDIRECT form by construction: the executor answers a direct call site itself and
                // never gets here. Indirect evaluation is global-scope evaluation, which is exactly
                // what an artifact compiled without knowledge of the calling frame produces.
                return engine.Evaluate(arguments, direct: false, Format.JsFormat.FunctionFlags.ProgramBody);
            });

        EvalIntrinsic = evaluate;

        GlobalObject.SetOwnProperty(
            "eval",
            JsProperty.Data(
                JsValue.Object(evaluate),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        Constructor(
            "Function",
            1,
            FunctionPrototype,
            static (engine, thisValue, arguments) => FromSource(engine, arguments),
            static (engine, thisValue, arguments) => FromSource(engine, arguments));
    }

    /// <summary>
    /// The <c>Function</c> constructor's one behaviour, which both call forms share.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is source assembly and then an indirect evaluation, which is what the language says
    /// it is.</b> The last argument is the body and every earlier one is a parameter list — several
    /// of them, comma-joined, because <c>new Function("a", "b", "return a+b")</c> and
    /// <c>new Function("a,b", "return a+b")</c> are the same function. The assembled text is then
    /// evaluated in the global scope, never in the caller's, which is the one thing about
    /// <c>Function</c> that differs from a direct <c>eval</c> and the reason it is not a scope
    /// leak.
    /// </para>
    /// <para>
    /// <b>The assembled source is parenthesised and immediately named.</b> Evaluating
    /// <c>function anonymous(...) {...}</c> as a statement would declare a global; evaluating
    /// <c>(function anonymous(...) {...})</c> answers the function as the program's completion
    /// value, which is what the constructor has to return.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=01B333
    // Broiler-Falsified-If: the assembled source is evaluated anywhere but the global scope
    // Broiler-Human:        PENDING
    private static JsValue FromSource(JsEngine engine, JsValue[] arguments)
    {
        var parameters = new System.Text.StringBuilder();

        for (var at = 0; at + 1 < arguments.Length; at++)
        {
            if (at != 0)
            {
                parameters.Append(',');
            }

            parameters.Append(engine.ToStringValue(arguments[at]));
        }

        var body = arguments.Length == 0
            ? string.Empty
            : engine.ToStringValue(arguments[arguments.Length - 1]);

        var source = new System.Text.StringBuilder()
            .Append("(function anonymous(")
            .Append(parameters)
            .Append("\n) {\n")
            .Append(body)
            .Append("\n})")
            .ToString();

        return engine.Evaluate(
            [JsValue.String(source)], direct: false, Format.JsFormat.FunctionFlags.ProgramBody);
    }
}
