// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The host's half of the suite's <c>$262</c>, installed through the profile's in-realm host surface.
/// </summary>
/// <remarks>
/// <para>
/// <b>INTERPRETING.md makes <c>$262</c> the host's, and these are the three members of it this
/// harness defines: <c>detachArrayBuffer</c>, <c>evalScript</c> and <c>IsHTMLDDA</c>.</b> The suite's <c>detachArrayBuffer.js</c> reaches it through
/// <c>$DETACHBUFFER</c>, and before it existed every test including that file met the profile's
/// refusing stub, a <c>TypeError</c> - several hundred variants over <c>ArrayBuffer</c>,
/// <c>DataView</c> and the typed arrays scored as failures of the engine when what they measured was
/// that this harness had not written the host's half. The member performs the profile's own
/// <c>DetachArrayBuffer</c> through <see cref="JsHostRealm.DetachArrayBuffer"/>, which is the same act
/// <c>ArrayBuffer.prototype.transfer</c> performs, and nothing else.
/// </para>
/// <para>
/// <b><c>evalScript</c> runs its argument as a script of the same realm</b> (JSeal V15-host), through
/// <see cref="JsHostRealm.EvaluateScript"/>: <c>ScriptEvaluation</c> with its global declaration
/// checks, lexical declarations that persist for the test that follows, and the script's completion
/// value as the answer - which is what the suite's <c>global-code</c> directories measure. A source
/// that does not parse is the <c>SyntaxError</c> the harness's algorithm returns.
/// </para>
/// <para>
/// <b><c>IsHTMLDDA</c> is an object with Annex B.3.6's <c>[[IsHTMLDDA]]</c> slot</b>, made by
/// <see cref="JsHostRealm.NewHtmlDdaObject"/>: <c>typeof</c> answers <c>"undefined"</c>, it is
/// falsy, it is <c>==</c> to <c>null</c> and <c>undefined</c>, and a call answers <c>null</c>, as
/// INTERPRETING.md defines it. The profile's own <c>$262</c> has no such member, because the suite
/// says it is present only where an implementation can provide it, and the tests that use it
/// measure the annex's three changes, which only a host can switch on.
/// </para>
/// <para>
/// <b>It replaces two members, adds one, and leaves the rest of the profile's <c>$262</c> as it
/// was.</b> The
/// realm already carries that object, and every function on it refuses with a <c>TypeError</c> naming
/// what this profile does not do - <c>createRealm</c>, <c>gc</c>, the
/// <c>agent</c> API - so a test that reaches one of those fails exactly as it did before this type
/// existed. A member that answered plausibly without doing what the suite means would turn those
/// failures into passes nobody earned.
/// </para>
/// <para>
/// <b>Ordinary guests never see it.</b> The members are installed only in realms of this harness's
/// wide runs, which build their descriptor with this surface and register
/// <see cref="JavaScriptProfile.HostSurfaceCapability"/>; the end-user host and every other
/// composition do neither, so detachment stays reachable there only through <c>transfer</c>, and no
/// object there has an <c>[[IsHTMLDDA]]</c> slot.
/// </para>
/// <para>
/// <b>It holds no state</b>, so one instance serves every runtime a process creates, in parallel if
/// need be: what it installs lives in the realm it was handed and dies with it.
/// </para>
/// </remarks>
internal sealed class Test262Host : IJsHostSurface
{
    /// <summary>The one instance a run's catalog is built with.</summary>
    internal static Test262Host Instance { get; } = new();

    private Test262Host()
    {
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The profile's realm already carries a <c>$262</c> whose every function refuses, so this replaces
    /// two members of that object rather than the object: <c>global</c>, <c>agent</c>,
    /// <c>createRealm</c> and <c>gc</c> keep answering the way the profile decided they should. The
    /// replacements keep the refusing members' attributes - writable and configurable, not
    /// enumerable - which are those of every built-in method, and <c>IsHTMLDDA</c> is added with the
    /// same attributes.
    /// </remarks>
    public void OnRealmCreated(JsHostRealm realm)
    {
        var harness = realm.GetProperty(realm.Global, "$262");

        realm.DefineValue(
            harness,
            "detachArrayBuffer",
            realm.NewMethod("detachArrayBuffer", Detach, 1),
            JsHostPropertyFlags.Writable | JsHostPropertyFlags.Configurable);

        realm.DefineValue(
            harness,
            "evalScript",
            realm.NewMethod("evalScript", EvalScript, 1),
            JsHostPropertyFlags.Writable | JsHostPropertyFlags.Configurable);

        realm.DefineValue(
            harness,
            "IsHTMLDDA",
            realm.NewHtmlDdaObject(),
            JsHostPropertyFlags.Writable | JsHostPropertyFlags.Configurable);
    }

    /// <inheritdoc/>
    /// <remarks>This harness never asks for a turn, so there is nothing to do in one.</remarks>
    public void OnTurn(JsHostRealm realm)
    {
    }

    /// <summary><c>$262.evalScript(source)</c>: run it as a script of this realm, answer its completion value.</summary>
    /// <remarks>
    /// The argument is converted with <c>ToString</c>, as every caller in the suite passes a String.
    /// A guest throw - the script's own, a failed global declaration check, or the
    /// <c>SyntaxError</c> of a source that does not parse - reaches the calling test unchanged.
    /// </remarks>
    private static JsHostValue EvalScript(
        JsHostRealm realm, JsHostValue thisValue, System.ReadOnlySpan<JsHostValue> arguments) =>
        realm.EvaluateScript(
            realm.ToJsString(arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined),
            "$262.evalScript");

    /// <summary><c>$262.detachArrayBuffer(buffer)</c>: detach it, answer <c>undefined</c>.</summary>
    /// <remarks>
    /// A value that is not an <c>ArrayBuffer</c> is the guest <c>TypeError</c> the profile raises, and
    /// it propagates to the guest unchanged.
    /// </remarks>
    private static JsHostValue Detach(
        JsHostRealm realm, JsHostValue thisValue, System.ReadOnlySpan<JsHostValue> arguments)
    {
        realm.DetachArrayBuffer(arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined);
        return JsHostValue.Undefined;
    }
}
