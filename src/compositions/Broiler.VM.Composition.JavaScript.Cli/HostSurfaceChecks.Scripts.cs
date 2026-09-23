// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>
/// The checks for the host surface's script member, <c>EvaluateScript</c> (JSeal V15-host).
/// </summary>
/// <remarks>
/// <para>
/// <b>Every check is judged by what the guest printed</b>, like the rest of the lane: the guest asks
/// the host to run a script through <c>broilerHost.run(source, name, strict)</c>, which is
/// <c>JsHostRealm.EvaluateScript</c> and nothing else, and prints the completion values, the error
/// names and what a later script or the guest itself can still see. One check runs the scripts from
/// an embedder's TURN instead, with no guest frame under them, which is how an embedder that drives
/// its realm between invocations - JSeal's <c>VmRealm</c> - will call it.
/// </para>
/// <para>
/// <b>The provider is this file's own and it holds a policy</b>: it answers every script request,
/// because only the host surface can send one, and it answers or refuses every OTHER program request
/// - a guest's direct or indirect <c>eval</c> - by the policy the check names. That is the permit
/// design the member exists for: the embedder's script is authorised by the embedder, and a guest
/// evaluation inside it still meets the realm's guest-evaluation policy.
/// </para>
/// </remarks>
internal static partial class HostSurfaceChecks
{
    /// <summary>Runs every script check and answers the failure count.</summary>
    private static int RunScripts()
    {
        var failures = 0;

        failures += ScriptCheck(
            "a host script's let persists into the next one, and each answers its completion value",
            """
            var a = broilerHost.run('let kept = 41; kept + 1', 'first.js');
            var b = broilerHost.run('kept + 1', 'second.js');
            print('completion=' + a + ',' + b);
            print('lexical=' + (typeof globalThis.kept) + ':' + kept);
            broilerHost.run('var v = 1; function g() { return v; }', 'third.js');
            var d = Object.getOwnPropertyDescriptor(globalThis, 'v');
            print('var=' + d.configurable + ':' + d.enumerable + ':' + g());
            print('eval=' + broilerHost.run('eval("var viaEval = 5"); viaEval', 'fourth.js'));
            print('empty=' + broilerHost.run('', 'empty.js') + ':' + broilerHost.run('var only;', 'decl.js'));
            """,
            [
                "completion=42,42",
                "lexical=undefined:41",
                "var=false:true:1",
                "eval=5",
                "empty=undefined:undefined",
            ]);

        failures += ScriptCheck(
            "a conflicting host script is refused before it creates anything",
            """
            broilerHost.run('let x1 = 1; const c1 = 2; class k1 {}', 'a.js');
            function attempt(src) {
                try { broilerHost.run(src, 'b.js'); return 'ran'; } catch (e) { return e.name; }
            }
            function state(name) {
                try { eval(name); return 'bound'; } catch (e) { return e.name; }
            }
            print('lex-lex=' + attempt('let x1 = 2;') + ',' + attempt('var x2; const c1 = 3;') + ',' + attempt('class k1 {}'));
            print('var-lex=' + attempt('var fresh1; var x1;') + ',' + attempt('function k1() {}'));
            print('created=' + ('x2' in globalThis) + ':' + ('fresh1' in globalThis) + ':' + x1 + ':' + c1);
            Object.defineProperty(globalThis, 'pinned', { value: 1, configurable: false });
            Object.defineProperty(globalThis, 'loose', { value: 1, configurable: true });
            print('restricted=' + attempt('let fresh2; let pinned;') + ':' + state('fresh2'));
            print('configurable=' + attempt('let loose = 2;') + ':' + loose + ':' + globalThis.loose);
            print('builtin=' + attempt('let undefined;') + ',' + attempt('const NaN = 1;'));
            Object.preventExtensions(globalThis);
            print('function=' + attempt('function brandNew() {}') + ':' + ('brandNew' in globalThis));
            print('var=' + attempt('var brandNew2;') + ':' + ('brandNew2' in globalThis));
            print('existing=' + attempt('var v1; var x3 = 1;'));
            """,
            [
                "lex-lex=SyntaxError,SyntaxError,SyntaxError",
                "var-lex=SyntaxError,SyntaxError",
                "created=false:false:1:2",
                "restricted=SyntaxError:ReferenceError",
                "configurable=ran:2:1",
                "builtin=SyntaxError,SyntaxError",
                "function=TypeError:false",
                "var=TypeError:false",
                "existing=TypeError",
            ]);

        failures += ScriptCheck(
            "a guest eval inside a host script still meets the realm's guest-eval policy",
            """
            print('host=' + broilerHost.run('var h = 1 + 1; h', 'host.js'));
            print('direct=' + broilerHost.run('var r; try { r = eval("1 + 1"); } catch (e) { r = e.name; } r', 'host.js'));
            print('indirect=' + broilerHost.run('var r2; try { r2 = (0, eval)("2"); } catch (e) { r2 = e.name; } r2', 'host.js'));
            print('function=' + broilerHost.run('var r3; try { r3 = Function("return 3")(); } catch (e) { r3 = e.name; } r3', 'host.js'));
            print('guest=' + (function () { try { return eval('4'); } catch (e) { return e.name; } })());
            print('counts=' + broilerHost.counts());
            """,
            [
                "host=2",
                "direct=SyntaxError",
                "indirect=SyntaxError",
                "function=SyntaxError",
                "guest=SyntaxError",
                "counts=scripts:4,guest-answered:0,guest-refused:4",
            ],
            allowGuestEval: false);

        failures += ScriptCheck(
            "a guest cannot send the script mark, whatever its source begins with",
            """
            function tryEval(src) { try { return (0, eval)(src); } catch (e) { return e.name; } }
            print('marked=' + tryEval(String.fromCharCode(2) + String.fromCharCode(0) + '1'));
            print('function=' + (function () { try { return Function(String.fromCharCode(2))(); } catch (e) { return e.name; } })());
            print('counts=' + broilerHost.counts());
            """,
            [
                "marked=SyntaxError",
                "function=SyntaxError",
                "counts=scripts:0,guest-answered:0,guest-refused:0",
            ]);

        failures += ScriptCheck(
            "a host script that does not parse is a SyntaxError naming its source, and the provider holds the position",
            """
            try { broilerHost.run('let ok = 1;\nlet = ;', 'broken.js'); print('no-throw'); }
            catch (e) { print('error=' + e.name + ':' + (e instanceof SyntaxError) + ':' + e.message); }
            print('diagnostic=' + broilerHost.diagnostic());
            print('untouched=' + (typeof ok));
            try { broilerHost.run('(', ''); } catch (e) { print('unnamed=' + e.message); }
            """,
            [
                "error=SyntaxError:true:the host script broken.js is not a program this profile admits",
                "diagnostic=broken.js:2:7",
                "untouched=undefined",
                "unnamed=the host script is not a program this profile admits",
            ]);

        failures += ScriptCheck(
            "the embedder's strictness option makes a sloppy source strict, and only when asked",
            """
            var probe = '(function () { return this === undefined; })()';
            print('sloppy=' + broilerHost.run(probe, 's.js', false) + ',strict=' + broilerHost.run(probe, 's.js', true));
            print('directive=' + broilerHost.run('"use strict"; ' + probe, 's.js', false));
            try { broilerHost.run('undeclared = 1;', 's.js', true); print('no-throw'); } catch (e) { print('assign=' + e.name); }
            broilerHost.run('undeclared2 = 1;', 's.js');
            print('sloppy-assign=' + undeclared2);
            """,
            ["sloppy=false,strict=true", "directive=true", "assign=ReferenceError", "sloppy-assign=1"]);

        failures += ScriptCheck(
            "a host script that spends the allowance ends the run, uncatchably",
            """
            print('before');
            try { broilerHost.run('for (;;) {}', 'spin.js'); print('no-throw'); } catch (e) { print('caught=' + e); }
            print('after');
            """,
            ["before"],
            outcome: VmOutcome.ResourceExhaustion);

        failures += ScriptCheck(
            "an embedder runs scripts from a turn, and a later program sees what they declared",
            "print('first=' + (typeof fromTurn));",
            ["first=undefined", "turn=7,ReferenceError,SyntaxError", "second=turn:7"],
            turn: static (realm, printed) =>
            {
                var completion = realm.EvaluateScript("let fromTurn = 'turn'; 7", "turn.js");
                var names = new List<string> { realm.ToJsString(completion) };

                foreach (var source in (string[])["undeclared", "let fromTurn;"])
                {
                    try
                    {
                        realm.EvaluateScript(source, "turn.js");
                        names.Add("ran");
                    }
                    catch (JsHostThrowException thrown)
                    {
                        names.Add(realm.ToJsString(realm.GetProperty(thrown.Thrown, "name")));
                    }
                }

                printed.Add("turn=" + string.Join(",", names));
            },
            second: "print('second=' + fromTurn + ':' + broilerHost.run('fromTurn.length + 3', 'again.js'));");

        return failures;
    }

    /// <summary>
    /// Runs one guest program - and, when asked, an embedder's turn and a second program after it -
    /// against a realm with this file's provider, and compares what was printed.
    /// </summary>
    private static int ScriptCheck(
        string title,
        string source,
        string[] expected,
        bool allowGuestEval = true,
        VmOutcome outcome = VmOutcome.Normal,
        System.Action<JsHostRealm, List<string>>? turn = null,
        string? second = null)
    {
        var printed = new List<string>();
        var provider = new PolicyProvider(allowGuestEval);
        var surface = new ScriptSurface(provider);

        JsScriptUnit[] units = second is null
            ? [new JsScriptUnit("check", source, SliceParseOptions.Script, false, Caller)]
            :
            [
                new JsScriptUnit("check", source, SliceParseOptions.Script, false, Caller),
                new JsScriptUnit("second", second, SliceParseOptions.Script, false, Caller),
            ];

        var compiled = JsCompiler.Compile(units, [], new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return Report(title, "the source was refused: " + Render(compiled));
        }

        var catalog = VmCatalog.CreateBuilder()
            .Add(JavaScriptProfile.DescriptorHostingRealms(surface))
            .Build();

        var created = VmRuntime.Create(catalog, ScriptOptions(printed, provider));

        if (!created.TryGetRuntime(out var runtime))
        {
            return Report(title, $"the runtime refused creation: {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            var verified = runtime.Verify(in descriptor, compiled.Artifact, CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                return Report(title, $"verification refused: {verified.Outcome}/{verified.Reason}");
            }

            using (artifact)
            {
                var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

                if (!instantiated.TryGetInstance(out var instance))
                {
                    return Report(
                        title, $"instantiation refused: {instantiated.Outcome}/{instantiated.Reason}");
                }

                using (instance)
                {
                    var request = new VmInvocationRequest(
                        new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("check")));
                    var result = instance.Invoke(in request, CancellationToken.None);

                    if (result.Outcome != outcome)
                    {
                        return Report(
                            title,
                            $"the run answered {result.Outcome}/{result.Reason} and {outcome} was expected");
                    }

                    if (turn is not null)
                    {
                        // THE EMBEDDER'S OWN SCRIPTS, WITH NO GUEST FRAME UNDER THEM: a turn is
                        // the step an embedder asks for, and the realm is current only inside it.
                        surface.Pending = realm => turn(realm, printed);

                        if (Run(instance, JavaScriptProfile.TurnEntryPoint) is { } turnFailure)
                        {
                            return Report(title, "the turn: " + turnFailure);
                        }
                    }

                    if (second is not null && Run(instance, "second") is { } secondFailure)
                    {
                        return Report(title, "the second program: " + secondFailure);
                    }
                }
            }
        }

        if (printed.Count != expected.Length)
        {
            return Report(
                title,
                $"printed {printed.Count} line(s) and {expected.Length} were expected: "
                    + string.Join(" | ", printed));
        }

        for (var at = 0; at < expected.Length; at++)
        {
            if (!string.Equals(printed[at], expected[at], System.StringComparison.Ordinal))
            {
                return Report(
                    title, $"line {at + 1} was '{printed[at]}' and '{expected[at]}' was expected");
            }
        }

        System.Console.WriteLine("ok   " + title);
        return 0;
    }

    /// <summary>The runtime options: `print`, the host-surface permission and this file's provider.</summary>
    private static VmRuntimeCreationOptions ScriptOptions(List<string> printed, PolicyProvider provider)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var capabilities = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        capabilities.Add(VmCapabilityRegistration.Value(
            JavaScriptProfile.WriteCapability,
            (VmBytes argument, out VmOpaqueRef answer) =>
            {
                answer = default;
                printed.Add(System.Text.Encoding.UTF8.GetString(argument.Span).TrimEnd('\n'));
                return VmHostCallOutcome.Completed;
            }));

        capabilities.Add(VmCapabilityRegistration.Value(
            JavaScriptProfile.HostSurfaceCapability,
            (VmBytes argument, out VmOpaqueRef answer) =>
            {
                answer = default;
                return VmHostCallOutcome.Completed;
            }));

        capabilities.Add(VmCapabilityRegistration.ArtifactProvider(
            JavaScriptProfile.SourceProviderCapability,
            provider));

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities.ToImmutable());
    }

    /// <summary>
    /// A provider with a guest-evaluation policy: every script request is answered, every other
    /// program request is answered or refused by the policy, and the last refusal is kept.
    /// </summary>
    private sealed class PolicyProvider(bool allowGuestEval) : IVmArtifactProvider
    {
        public VmCapabilityId CapabilityId => JavaScriptProfile.SourceProviderCapability.CapabilityId;

        public int Version => JavaScriptProfile.SourceProviderCapability.Version;

        /// <summary>How many script requests - the embedder's own - were answered.</summary>
        internal int Scripts { get; private set; }

        /// <summary>How many guest program requests were answered.</summary>
        internal int GuestAnswered { get; private set; }

        /// <summary>How many guest program requests the policy refused.</summary>
        internal int GuestRefused { get; private set; }

        /// <summary>The first diagnostic of the last refused compilation, or nothing.</summary>
        internal SliceSourceDiagnostic? Diagnostic { get; private set; }

        public VmArtifactProviderAnswer Answer(scoped in VmArtifactRequest request)
        {
            if (request.RequestingProfileId != JavaScriptProfile.Id ||
                JsFormat.TryReadModuleRequest(request.RequestPayload.Span, out _, out _))
            {
                return VmArtifactProviderAnswer.NotFound(VmReason.ProviderArtifactNotFound);
            }

            // THE MARK IS THE AUTHORISATION. Only the host surface writes it, so a request that
            // carries it is the embedder's own script; every other program request is a guest's.
            var payload = request.RequestPayload.Span;
            var hostScript = payload.Length != 0 && payload[0] == JsFormat.ScriptRequestMark;

            if (!hostScript && !allowGuestEval)
            {
                GuestRefused++;
                return VmArtifactProviderAnswer.Refused(VmReason.ProviderRefused);
            }

            if (!JsCompiler.TryReadProgramRequest(payload, out var script))
            {
                return VmArtifactProviderAnswer.Refused(VmReason.MalformedEncoding);
            }

            var compiled = JsCompiler.Compile([script], [], new JsCompileRequest());

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                Diagnostic = compiled.Diagnostics.Count == 0 ? null : compiled.Diagnostics[0];
                return VmArtifactProviderAnswer.Refused(VmReason.SemanticValidationFailed);
            }

            if (hostScript)
            {
                Scripts++;
            }
            else
            {
                GuestAnswered++;
            }

            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            return VmArtifactProviderAnswer.Provided(in descriptor, compiled.Artifact);
        }
    }

    /// <summary>An embedder whose one job is to run the guest's scripts as its own.</summary>
    private sealed class ScriptSurface(PolicyProvider provider) : IJsHostSurface
    {
        /// <summary>What the embedder does when a host asks for a turn.</summary>
        internal System.Action<JsHostRealm>? Pending { get; set; }

        public void OnTurn(JsHostRealm realm)
        {
            var work = Pending;
            Pending = null;
            work?.Invoke(realm);
        }

        public void OnRealmCreated(JsHostRealm realm)
        {
            var host = realm.NewObject();

            realm.DefineValue(
                host,
                "run",
                realm.NewMethod(
                    "run",
                    static (r, _, arguments) => r.EvaluateScript(
                        r.ToJsString(arguments[0]),
                        arguments.Length > 1 ? r.ToJsString(arguments[1]) : string.Empty,
                        arguments.Length > 2 && arguments[2] == JsHostValue.Boolean(true)),
                    3));

            realm.DefineValue(
                host,
                "counts",
                realm.NewMethod(
                    "counts",
                    (r, _, _) => JsHostValue.String(
                        $"scripts:{provider.Scripts},guest-answered:{provider.GuestAnswered}," +
                        $"guest-refused:{provider.GuestRefused}")));

            realm.DefineValue(
                host,
                "diagnostic",
                realm.NewMethod(
                    "diagnostic",
                    (r, _, _) => JsHostValue.String(
                        provider.Diagnostic is { } found
                            ? $"{found.SourceName}:{found.Line}:{found.Column}"
                            : "none")));

            realm.DefineValue(realm.Global, "broilerHost", host);
        }
    }
}
