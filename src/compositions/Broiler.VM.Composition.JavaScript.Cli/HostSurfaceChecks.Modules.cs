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
/// The module half of the host-surface lane: a host linking an in-memory module graph into a
/// realm it already holds, evaluating it, and answering a guest <c>import()</c> later.
/// </summary>
/// <remarks>
/// <para>
/// <b>The graph is a table and not a filesystem</b>, which is the shape an embedder that owns its
/// own module map has: keys, source text and the resolution of each request, all held by the
/// composition. The provider below answers module requests from that table, compiling under the
/// module goal, and REFUSES every evaluation request - so every check here also shows that module
/// loading does not borrow the permission <c>eval</c> would need.
/// </para>
/// <para>
/// <b>Each check is judged by a transcript</b>: what the guest printed and what the host recorded,
/// in the order they happened, compared line by line. A promise that settled synchronously, a body
/// that ran twice and a namespace that is a copy all change a line.
/// </para>
/// </remarks>
internal static partial class HostSurfaceChecks
{
    /// <summary>Runs the module checks, answering the failure count.</summary>
    private static int RunModules()
    {
        var failures = 0;

        failures += ModuleCheck(
            "a host links a two-module graph, evaluates it once, and reads live bindings",
            new Dictionary<string, string>
            {
                ["mem:/main"] =
                    "import { x, bump } from './dep'; export const seen = x; bump(); export const after = x; " +
                    "globalThis.mainRuns = (globalThis.mainRuns || 0) + 1;",
                ["mem:/dep"] =
                    "export let x = 1; export function bump() { x++; } " +
                    "globalThis.depRuns = (globalThis.depRuns || 0) + 1;",
            },
            new Dictionary<string, string>
            {
                ["unrun"] =
                    "try { ns.seen; print('tdz=none'); } catch (e) { print('tdz=' + e.name); } " +
                    "print('unrun=' + typeof mainRuns);",
                ["started"] =
                    "var state = 'pending'; done.then(function () { state = 'fulfilled'; }); " +
                    "print('tag=' + Object.prototype.toString.call(ns) + ':' + Object.keys(ns).join(',') + ':' + Object.isExtensible(ns)); " +
                    "print('sync=' + state + ':' + mainRuns + ':' + depRuns);",
                ["drained"] = "print('drained=' + state + ':' + ns.seen + ':' + ns.after);",
                ["dynamic"] =
                    "import('./dep').then(function (n) { print('dynamic=' + (n === depns) + ':' + n.x + ':' + depRuns); });",
                ["live"] = "depns.bump(); print('live=' + depns.x + ':' + mainRuns + ':' + depRuns);",
            },
            session =>
            {
                session.Turn(realm =>
                {
                    var main = realm.LoadModule("./main");
                    session.Record("key=" + main.Key);
                    session.Record("same-handle=" + ReferenceEquals(main, realm.LoadModule("./main")));
                    realm.DefineValue(realm.Global, "ns", main.Namespace);
                });
                session.Script("unrun");
                session.Turn(realm =>
                {
                    var main = realm.LoadModule("./main");
                    var done = realm.EvaluateModule(main);
                    session.Record("same-promise=" + (done == realm.EvaluateModule(main)));
                    realm.DefineValue(realm.Global, "done", done);
                });
                session.Script("started");
                session.Drain();
                session.Script("drained");
                session.Turn(realm =>
                {
                    var dep = realm.LoadModule("./dep");
                    session.Record("dep=" + dep.Key);
                    realm.DefineValue(realm.Global, "depns", dep.Namespace);
                });
                session.Script("dynamic");
                session.Drain();
                session.Script("live");
            },
            [
                "key=mem:/main",
                "same-handle=True",
                "tdz=ReferenceError",
                "unrun=undefined",
                "same-promise=True",
                "tag=[object Module]:after,seen:false",
                "sync=pending:1:1",
                "drained=fulfilled:1:2",
                "dep=mem:/dep",
                "dynamic=true:2:1",
                "live=3:1:1",
            ]);

        failures += ModuleCheck(
            "a host-linked cycle meets the cross-module dead zone and then the live binding",
            new Dictionary<string, string>
            {
                ["mem:/a"] =
                    "import { b, readA } from './b'; export let a = 'a0'; a = 'a1'; " +
                    "export const fromB = b; export const late = readA();",
                ["mem:/b"] =
                    "import { a } from './a'; export let b; " +
                    "try { b = 'saw:' + a; } catch (e) { b = 'tdz:' + e.name; } " +
                    "export function readA() { return a; }",
            },
            new Dictionary<string, string>
            {
                ["cycle"] =
                    "done.then(function () { print('cycle=' + ns.fromB + ':' + ns.late + ':' + bns.readA() + ':' + (bns.b === ns.fromB)); });",
            },
            session =>
            {
                session.Turn(realm =>
                {
                    var a = realm.LoadModule("./a");
                    var b = realm.LoadModule("./b");
                    session.Record("keys=" + a.Key + "," + b.Key);
                    realm.DefineValue(realm.Global, "ns", a.Namespace);
                    realm.DefineValue(realm.Global, "bns", b.Namespace);
                    realm.DefineValue(realm.Global, "done", realm.EvaluateModule(a));
                });
                session.Script("cycle");
                session.Drain();
            },
            ["keys=mem:/a,mem:/b", "cycle=tdz:ReferenceError:a1:a1:true"]);

        failures += ModuleCheck(
            "a load that fails is a guest error of the right kind, is not cached, and links nothing",
            new Dictionary<string, string>
            {
                ["mem:/bad"] = "export let = 1;",
                ["mem:/needs-missing"] = "import './nowhere'; export const never = 1;",
                ["mem:/lib"] = "export function f() { return 'f'; }",
                ["mem:/user"] = "import { f } from './lib'; export const got = f();",
            },
            new Dictionary<string, string>
            {
                ["user"] =
                    "userDone.then(function () { print('user=' + uns.got); }, function (e) { print('user-rejected=' + e.name); });",
                ["missing"] =
                    "import('./missing').then(function (n) { print('missing=' + n.v); }, function (e) { print('dynamic-missing=' + e.name); });",
            },
            session =>
            {
                session.Turn(realm =>
                {
                    session.Record("missing=" + Failure(realm, () => realm.LoadModule("./missing")));
                    session.Record("bad=" + Failure(realm, () => realm.LoadModule("./bad")));
                    session.Record("needs-missing=" + Failure(realm, () => realm.LoadModule("./needs-missing")));

                    // THE COMPOSITION DECLINES ONE RESOLUTION, and the graph must be left exactly as
                    // unlinked as if it had never been asked for: the next load links it afresh.
                    session.Lane.RefuseConfirmation = true;
                    session.Record("unconfirmed=" + Failure(realm, () => realm.LoadModule("./user")));
                    session.Lane.RefuseConfirmation = false;

                    var user = realm.LoadModule("./user");
                    realm.DefineValue(realm.Global, "uns", user.Namespace);
                    realm.DefineValue(realm.Global, "userDone", realm.EvaluateModule(user));
                });
                session.Script("user");
                session.Script("missing");
                session.Drain();
                session.Lane.Modules["mem:/missing"] = "export const v = 'present now';";
                session.Script("missing");
                session.Drain();
            },
            [
                "missing=TypeError",
                "bad=SyntaxError",
                "needs-missing=TypeError",
                "unconfirmed=TypeError",
                "user=f",
                "dynamic-missing=TypeError",
                "missing=present now",
            ]);

        failures += ModuleCheck(
            "top-level await settles a host evaluation only through the queue, and only when finished",
            new Dictionary<string, string>
            {
                ["mem:/slow"] = "export let v = 'start'; await null; await null; await null; v = 'end';",
                ["mem:/main"] = "import { v } from './slow'; export const seen = v;",
                ["mem:/slow2"] = "export let v = 'start'; await null; await null; await null; v = 'end';",
                ["mem:/throws"] = "export const before = 1; throw new RangeError('boom');",
                ["mem:/tla-throws"] = "await null; throw new TypeError('late');",
                ["mem:/tla-importer"] = "import './tla-throws'; globalThis.importerRan = true;",
            },
            new Dictionary<string, string>
            {
                ["started"] =
                    "var st = 'pending'; done.then(function () { st = 'fulfilled:' + ns.seen; }); print('sync=' + st);",
                ["drained"] = "print('drained=' + st);",
                ["guest-first"] = "import('./slow2');",
                ["host-second"] = "d2.then(function () { print('host-waited=' + ns2.v); });",
                ["rejections"] =
                    "t1.then(null, function (e) { print('rejected=' + e.name + ':' + e.message); }); " +
                    "t2.then(null, function (e) { print('tla-rejected=' + e.name + ':' + e.message + ':' + (typeof importerRan)); });",
            },
            session =>
            {
                session.Turn(realm =>
                {
                    var main = realm.LoadModule("./main");
                    realm.DefineValue(realm.Global, "ns", main.Namespace);
                    realm.DefineValue(realm.Global, "done", realm.EvaluateModule(main));
                });
                session.Script("started");
                session.Drain();
                session.Script("drained");

                // A GUEST IMPORT STARTS THE MODULE AND THE HOST ASKS FOR THE SAME EVALUATION WHILE
                // IT IS STILL AWAITING. The host's promise must wait for the module to finish.
                session.Script("guest-first");
                session.Turn(realm =>
                {
                    var slow = realm.LoadModule("./slow2");
                    realm.DefineValue(realm.Global, "ns2", slow.Namespace);
                    realm.DefineValue(realm.Global, "d2", realm.EvaluateModule(slow));
                });
                session.Script("host-second");
                session.Drain();

                session.Turn(realm =>
                {
                    var throws = realm.LoadModule("./throws");
                    var first = realm.EvaluateModule(throws);
                    session.Record("same-rejection=" + (first == realm.EvaluateModule(throws)));
                    realm.DefineValue(realm.Global, "t1", first);
                    realm.DefineValue(
                        realm.Global, "t2", realm.EvaluateModule(realm.LoadModule("./tla-importer")));
                });
                session.Script("rejections");
                session.Drain();
            },
            [
                "sync=pending",
                "drained=fulfilled:end",
                "host-waited=end",
                "same-rejection=True",
                "rejected=RangeError:boom",
                "tla-rejected=TypeError:late:undefined",
            ]);

        failures += ModuleCheck(
            "an errored module stays errored, and a cycle member waits for its awaiting root, whoever started them",
            new Dictionary<string, string>
            {
                ["mem:/boom"] = "globalThis.boomRuns = (globalThis.boomRuns || 0) + 1; throw new Error('boom');",
                ["mem:/second"] = "import './boom'; globalThis.secondRan = true;",
                ["mem:/ca"] =
                    "import './cb'; log.push('A start'); " +
                    "await new Promise(function (r) { globalThis.releaseA = r; }); log.push('A end');",
                ["mem:/cb"] = "import './ca'; (globalThis.log = globalThis.log || []).push('B run');",
            },
            new Dictionary<string, string>
            {
                ["guest-boom"] =
                    "import('./boom').then(null, function (e) { globalThis.boomErr = e; print('guest-boom=' + e.message); });",
                ["host-boom"] =
                    "hb.then(function () { print('host-boom=fulfilled'); }, function (e) { print('host-boom=' + (e === boomErr) + ':' + boomRuns); }); " +
                    "hs.then(function () { print('host-second=fulfilled'); }, function (e) { print('host-second=' + (e === boomErr) + ':' + typeof secondRan); });",
                ["guest-cycle"] = "import('./ca');",
                ["host-cycle"] =
                    "hc.then(function () { print('host-member=' + log.join(',')); }); " +
                    "print('before-release=' + log.join(',')); releaseA();",
            },
            session =>
            {
                // THE GUEST MEETS THE FAILURE FIRST, and the host's evaluation of the same module,
                // and of a module that depends on it, must reject with the identical value.
                session.Script("guest-boom");
                session.Drain();
                session.Turn(realm =>
                {
                    realm.DefineValue(realm.Global, "hb", realm.EvaluateModule(realm.LoadModule("./boom")));
                    realm.DefineValue(realm.Global, "hs", realm.EvaluateModule(realm.LoadModule("./second")));
                });
                session.Script("host-boom");
                session.Drain();

                // THE GUEST STARTS A CYCLE WHOSE ROOT AWAITS, and the host evaluates the member whose
                // own body has already run: its promise waits for the root.
                session.Script("guest-cycle");
                session.Drain();
                session.Turn(realm =>
                    realm.DefineValue(realm.Global, "hc", realm.EvaluateModule(realm.LoadModule("./cb"))));
                session.Script("host-cycle");
                session.Drain();
            },
            [
                "guest-boom=boom",
                "host-boom=true:1",
                "host-second=true:undefined",
                "before-release=B run,A start",
                "host-member=B run,A start,A end",
            ]);

        failures += ModuleCheck(
            "a deferred import stays pending until a turn completes it, once, through the queue",
            new Dictionary<string, string>
            {
                ["mem:/later"] = "export const v = 'later'; globalThis.laterRuns = (globalThis.laterRuns || 0) + 1;",
                ["mem:/dir/a"] = "export function load() { return import('./b'); }",
                ["mem:/dir/b"] = "export const where = 'dir/b';",
            },
            new Dictionary<string, string>
            {
                ["ask"] =
                    "var got = 'pending'; var twice = 'pending'; " +
                    "var p1 = import('./later'); var p2 = import('./later'); " +
                    "p1.then(function (n) { got = 'fulfilled:' + n.v; }, function (e) { got = 'rejected:' + e.name; }); " +
                    "Promise.all([p1, p2]).then(function (r) { twice = (r[0] === r[1]) + ':' + laterRuns; }); " +
                    "print('sync=' + got);",
                ["check"] = "print('check=' + got + ':' + twice);",
                ["nested"] =
                    "var gotB = 'pending'; ans.load().then(function (n) { gotB = n.where; }, function (e) { gotB = 'rejected:' + e.name; });",
                ["refused"] =
                    "var gotC = 'pending'; import('./nowhere').then(function () { gotC = 'resolved'; }, function (e) { gotC = 'rejected:' + e.name + ':' + e.message; });",
                ["results"] = "print('nested=' + gotB); print('refused=' + gotC);",
            },
            session =>
            {
                session.Lane.Defer = true;
                session.Script("ask");
                session.RecordOffers();
                session.Drain();
                session.Script("check");
                session.Turn(realm =>
                {
                    var first = session.Lane.Offered[0];
                    var second = session.Lane.Offered[1];
                    session.Record("complete=" + realm.CompleteModuleRequest(first));
                    session.Record("again=" + realm.CompleteModuleRequest(first));
                    session.Record("fail-late=" + realm.FailModuleRequest(first, JsHostErrorKind.TypeError, "late"));
                    session.Record("second=" + realm.CompleteModuleRequest(second));
                });
                session.Script("check");
                session.Drain();
                session.Script("check");

                session.Lane.Offered.Clear();
                session.Lane.Reported = 0;
                session.Lane.Defer = false;
                session.Turn(realm =>
                {
                    var a = realm.LoadModule("./dir/a");
                    realm.DefineValue(realm.Global, "ans", a.Namespace);
                    _ = realm.EvaluateModule(a);
                });
                session.Drain();
                session.Lane.Defer = true;
                session.Script("nested");
                session.Script("refused");
                session.RecordOffers();
                session.Turn(realm =>
                {
                    session.Record("nested-complete=" + realm.CompleteModuleRequest(session.Lane.Offered[0]));
                    session.Record("refused-fail=" + realm.FailModuleRequest(
                        session.Lane.Offered[1], JsHostErrorKind.TypeError, "the host has no such module"));
                });
                session.Drain();
                session.Script("results");
            },
            [
                "sync=pending",
                "offered=|./later",
                "offered=|./later",
                "check=pending:pending",
                "complete=Accepted",
                "again=AlreadyResolved",
                "fail-late=AlreadyResolved",
                "second=Accepted",
                "check=pending:pending",
                "check=fulfilled:later:true:1",
                "offered=mem:/dir/a|./b",
                "offered=|./nowhere",
                "nested-complete=Accepted",
                "refused-fail=Accepted",
                "nested=dir/b",
                "refused=rejected:TypeError:the host has no such module",
            ]);

        failures += ModuleCheck(
            "a loader may not complete the request it is offered, and a Now answer loads in the step",
            new Dictionary<string, string>
            {
                ["mem:/later"] = "export const v = 'now';",
            },
            new Dictionary<string, string>
            {
                ["ask"] =
                    "import('./later').then(function (n) { print('now=' + n.v); }, function (e) { print('rejected=' + e.name); });",
                ["eval"] = "try { eval('1'); print('eval=ran'); } catch (e) { print('eval=' + e.name); }",
            },
            session =>
            {
                session.Lane.CompleteInsideOffer = true;
                session.Script("ask");
                session.Drain();
                session.Script("eval");
            },
            ["inside=InvalidOperationException", "now=now", "eval=SyntaxError"]);

        failures += ModuleCheck(
            "a loader that throws rejects the import and settles the request, so no later turn completes it",
            new Dictionary<string, string>
            {
                ["mem:/later"] = "export const v = 'later';",
            },
            new Dictionary<string, string>
            {
                ["ask"] =
                    "var got = 'pending'; import('./later').then(function () { got = 'fulfilled'; }, " +
                    "function (e) { got = 'rejected:' + e.name + ':' + e.message; });",
                ["check"] = "print('got=' + got);",
            },
            session =>
            {
                session.Lane.ThrowOnImport = true;
                session.Script("ask");
                session.RecordOffers();
                session.Drain();
                session.Script("check");
                session.Turn(realm =>
                {
                    var request = session.Lane.Offered[0];
                    session.Record("complete=" + realm.CompleteModuleRequest(request));
                    session.Record("fail=" + realm.FailModuleRequest(request, JsHostErrorKind.TypeError, "late"));
                });
                session.Drain();
                session.Script("check");
            },
            [
                "offered=|./later",
                "got=rejected:RangeError:the loader failed",
                "complete=AlreadyResolved",
                "fail=AlreadyResolved",
                "got=rejected:RangeError:the loader failed",
            ]);

        failures += CheckModuleLifetime();

        return failures;
    }

    /// <summary>
    /// A deferred request across two realms and a disposal: foreign is refused, disposed is refused,
    /// and the released realm settles nothing.
    /// </summary>
    private static int CheckModuleLifetime()
    {
        const string Title =
            "a deferred request is refused in another realm and after its realm is released";

        var lines = new List<string>();
        var lane = new ModuleLane(lines) { Defer = true };
        lane.Modules["mem:/later"] = "export const v = 'later';";

        var compiled = JsCompiler.Compile(
            [
                new JsScriptUnit(
                    "ask",
                    "import('./later').then(function () { print('settled-after-release'); });",
                    SliceParseOptions.Script),
            ],
            [],
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return Report(Title, "the source was refused: " + Render(compiled));
        }

        var catalog = VmCatalog.CreateBuilder()
            .Add(JavaScriptProfile.DescriptorHostingRealms(lane))
            .Build();

        var created = VmRuntime.Create(catalog, ModuleOptions(lane));

        if (!created.TryGetRuntime(out var runtime))
        {
            return Report(Title, $"the runtime refused creation: {created.Outcome}/{created.Reason}");
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
                return Report(Title, $"verification refused: {verified.Outcome}/{verified.Reason}");
            }

            using (artifact)
            {
                var first = runtime.Instantiate(artifact, CancellationToken.None);
                var second = runtime.Instantiate(artifact, CancellationToken.None);

                if (!first.TryGetInstance(out var owner) || !second.TryGetInstance(out var other))
                {
                    return Report(Title, $"instantiation refused: {first.Outcome}/{second.Outcome}");
                }

                JsHostRealm? owning = null;

                using (other)
                {
                    using (owner)
                    {
                        lane.Pending = realm => owning = realm;

                        if (Run(owner, JavaScriptProfile.TurnEntryPoint) is { } find)
                        {
                            return Report(Title, "the first turn: " + find);
                        }

                        if (Run(owner, "ask") is { } ask)
                        {
                            return Report(Title, "the asking program: " + ask);
                        }

                        if (lane.Offered.Count != 1 || owning is null)
                        {
                            return Report(Title, $"the loader was offered {lane.Offered.Count} request(s)");
                        }

                        var request = lane.Offered[0];
                        lines.Add("offered=" + request.Referrer + "|" + request.Specifier);

                        lines.Add("outside=" + ModuleRefusal(() => owning.CompleteModuleRequest(request)));
                        lines.Add("load-outside=" + ModuleRefusal(() =>
                        {
                            _ = owning.LoadModule("./later");
                            return JsHostSettlement.Accepted;
                        }));
                        lines.Add("state-outside=" + ModuleRefusal(() =>
                        {
                            _ = owning.TryGetModuleState("mem:/later", out _);
                            return JsHostSettlement.Accepted;
                        }));

                        lane.Pending = realm => lines.Add(
                            "foreign=" + ModuleRefusal(() => realm.CompleteModuleRequest(request)));

                        if (Run(other, JavaScriptProfile.TurnEntryPoint) is { } foreign)
                        {
                            return Report(Title, "the foreign turn: " + foreign);
                        }
                    }

                    // THE INSTANCE IS RELEASED WITH THE IMPORT STILL PENDING. No step can open for it
                    // again, so the request can never be completed and the guest's reaction never runs.
                    lines.Add("released=" + ModuleRefusal(() => owning.CompleteModuleRequest(lane.Offered[0])));
                }
            }
        }

        return Compare(
            Title,
            lines,
            [
                "offered=|./later",
                "outside=RealmNotCurrent",
                "load-outside=RealmNotCurrent",
                "state-outside=RealmNotCurrent",
                "foreign=ForeignRealm",
                "released=RealmNotCurrent",
            ]);
    }

    /// <summary>Names the refusal a module member met, or says it was not refused.</summary>
    private static string ModuleRefusal(System.Func<JsHostSettlement> act)
    {
        try
        {
            return "not refused: " + act();
        }
        catch (JsHostSurfaceException refusal)
        {
            return refusal.Refusal.ToString();
        }
    }

    /// <summary>Names the guest error a host load met, by its <c>name</c>, or says it loaded.</summary>
    private static string Failure(JsHostRealm realm, System.Func<JsHostModule> load)
    {
        try
        {
            return "loaded " + load().Key;
        }
        catch (JsHostThrowException thrown)
        {
            return realm.ToJsString(realm.GetProperty(thrown.Thrown, "name"));
        }
    }

    /// <summary>Compares a transcript with what was expected, reporting the first difference.</summary>
    private static int Compare(string title, List<string> lines, string[] expected)
    {
        if (lines.Count != expected.Length)
        {
            return Report(
                title,
                $"recorded {lines.Count} line(s) and {expected.Length} were expected: " +
                    string.Join(" | ", lines));
        }

        for (var at = 0; at < expected.Length; at++)
        {
            if (!string.Equals(lines[at], expected[at], System.StringComparison.Ordinal))
            {
                return Report(title, $"line {at + 1} was '{lines[at]}' and '{expected[at]}' was expected");
            }
        }

        System.Console.WriteLine("ok   " + title);
        return 0;
    }

    /// <summary>
    /// Runs one module check: a realm over the named scripts, a lane over the named modules, and a
    /// sequence of turns, scripts and drains whose transcript is compared.
    /// </summary>
    private static int ModuleCheck(
        string title,
        Dictionary<string, string> modules,
        Dictionary<string, string> scripts,
        System.Action<ModuleSession> body,
        string[] expected)
    {
        var lines = new List<string>();
        var lane = new ModuleLane(lines);

        foreach (var (key, text) in modules)
        {
            lane.Modules[key] = text;
        }

        var units = new List<JsScriptUnit>();

        foreach (var (name, text) in scripts)
        {
            units.Add(new JsScriptUnit(name, text, SliceParseOptions.Script));
        }

        var compiled = JsCompiler.Compile(units, [], new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return Report(title, "the source was refused: " + Render(compiled));
        }

        var catalog = VmCatalog.CreateBuilder()
            .Add(JavaScriptProfile.DescriptorHostingRealms(lane))
            .Build();

        var created = VmRuntime.Create(catalog, ModuleOptions(lane));

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
                    var session = new ModuleSession(lane, instance, lines);

                    try
                    {
                        body(session);
                    }
                    catch (System.Exception raised)
                    {
                        return Report(title, "the host side threw " + raised.GetType().Name + ": " + raised.Message);
                    }

                    if (session.Failure is { } failure)
                    {
                        return Report(title, failure + " | so far: " + string.Join(" | ", lines));
                    }
                }
            }
        }

        return Compare(title, lines, expected);
    }

    /// <summary>
    /// The runtime options of the module lane: <c>print</c>, the host-surface permission, the
    /// resolver and the in-memory provider.
    /// </summary>
    private static VmRuntimeCreationOptions ModuleOptions(ModuleLane lane)
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
                lane.Lines.Add(System.Text.Encoding.UTF8.GetString(argument.Span).TrimEnd('\n'));
                return VmHostCallOutcome.Completed;
            }));

        capabilities.Add(VmCapabilityRegistration.Value(
            JavaScriptProfile.HostSurfaceCapability,
            (VmBytes argument, out VmOpaqueRef answer) =>
            {
                answer = default;
                return VmHostCallOutcome.Completed;
            }));

        capabilities.Add(VmCapabilityRegistration.Value(
            JavaScriptProfile.ResolveCapability,
            (VmBytes argument, out VmOpaqueRef answer) =>
            {
                answer = default;
                return lane.Confirms(argument.Span) ? VmHostCallOutcome.Completed : VmHostCallOutcome.Refused;
            }));

        capabilities.Add(VmCapabilityRegistration.ArtifactProvider(
            JavaScriptProfile.SourceProviderCapability,
            lane));

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities.ToImmutable());
    }

    /// <summary>One check's sequence of turns, scripts and drains, stopping at the first failure.</summary>
    private sealed class ModuleSession(ModuleLane lane, VmInstance instance, List<string> lines)
    {
        internal ModuleLane Lane { get; } = lane;

        internal string? Failure { get; private set; }

        internal void Record(string line) => lines.Add(line);

        internal void Turn(System.Action<JsHostRealm> work)
        {
            if (Failure is not null)
            {
                return;
            }

            Lane.Pending = work;
            Failure = Run(instance, JavaScriptProfile.TurnEntryPoint);
        }

        internal void Script(string name)
        {
            Failure ??= Run(instance, name);
        }

        internal void Drain()
        {
            Failure ??= Run(instance, JavaScriptProfile.DrainEntryPoint);
        }

        /// <summary>Records every request offered since the last call, in order.</summary>
        internal void RecordOffers()
        {
            for (var at = Lane.Reported; at < Lane.Offered.Count; at++)
            {
                lines.Add("offered=" + Lane.Offered[at].Referrer + "|" + Lane.Offered[at].Specifier);
            }

            Lane.Reported = Lane.Offered.Count;
        }
    }

    /// <summary>
    /// An embedder that owns an in-memory module map: its surface, its loader, its provider and its
    /// resolution rule, in one small object.
    /// </summary>
    /// <remarks>
    /// <b>The resolution rule is the whole of what the table adds to text</b>: <c>./name</c> is
    /// resolved against the referrer's directory, and a referrer of nothing means <c>mem:/</c>.
    /// Anything else is not found. The provider answers module requests from the table and refuses
    /// every evaluation request, so the lane runs with guest evaluation forbidden.
    /// </remarks>
    private sealed class ModuleLane(List<string> lines) : IJsHostSurface, IJsHostModuleLoader, IVmArtifactProvider
    {
        internal List<string> Lines { get; } = lines;

        internal Dictionary<string, string> Modules { get; } = new(System.StringComparer.Ordinal);

        internal List<JsHostModuleRequest> Offered { get; } = [];

        internal int Reported { get; set; }

        internal bool Defer { get; set; }

        internal bool CompleteInsideOffer { get; set; }

        internal bool ThrowOnImport { get; set; }

        internal bool RefuseConfirmation { get; set; }

        /// <summary>Whether evaluation requests and host scripts are answered rather than refused.</summary>
        internal bool AnswerPrograms { get; set; }

        internal System.Action<JsHostRealm>? Pending { get; set; }

        public VmCapabilityId CapabilityId => JavaScriptProfile.SourceProviderCapability.CapabilityId;

        public int Version => JavaScriptProfile.SourceProviderCapability.Version;

        public void OnRealmCreated(JsHostRealm realm)
        {
        }

        public void OnTurn(JsHostRealm realm)
        {
            var work = Pending;
            Pending = null;
            work?.Invoke(realm);
        }

        public JsHostModuleLoad OnImport(JsHostRealm realm, JsHostModuleRequest request)
        {
            if (CompleteInsideOffer)
            {
                try
                {
                    realm.CompleteModuleRequest(request);
                    Lines.Add("inside=completed");
                }
                catch (System.InvalidOperationException)
                {
                    Lines.Add("inside=InvalidOperationException");
                }

                return JsHostModuleLoad.Now;
            }

            // A LOADER THAT KEEPS THE REQUEST AND THEN THROWS, which rejects the import in the step.
            if (ThrowOnImport)
            {
                Offered.Add(request);
                throw realm.Error(JsHostErrorKind.RangeError, "the loader failed");
            }

            if (!Defer)
            {
                return JsHostModuleLoad.Now;
            }

            Offered.Add(request);
            return JsHostModuleLoad.Deferred;
        }

        public VmArtifactProviderAnswer Answer(scoped in VmArtifactRequest request)
        {
            if (request.RequestingProfileId != JavaScriptProfile.Id)
            {
                return VmArtifactProviderAnswer.Refused(VmReason.ProviderRefused);
            }

            if (!JsFormat.TryReadModuleRequest(request.RequestPayload.Span, out var referrer, out var specifier))
            {
                // EVERY EVALUATION IS REFUSED unless a check says otherwise: this embedder forbids
                // guest `eval`, and its modules must load all the same.
                return AnswerPrograms
                    ? AnswerProgram(request.RequestPayload.Span)
                    : VmArtifactProviderAnswer.Refused(VmReason.ProviderRefused);
            }

            var root = Resolve(referrer, specifier);

            if (root.Length == 0 || !Modules.ContainsKey(root))
            {
                return VmArtifactProviderAnswer.NotFound(VmReason.ProviderArtifactNotFound);
            }

            var units = new List<JsModuleUnit>();
            var seen = new HashSet<string>(System.StringComparer.Ordinal) { root };
            var pending = new Queue<string>();
            pending.Enqueue(root);

            while (pending.Count != 0)
            {
                var key = pending.Dequeue();
                var text = Modules[key];
                var requests = JsCompiler.Requests(text, SliceParseOptions.Module);

                if (!requests.Succeeded)
                {
                    units.Add(new JsModuleUnit(key, text, SliceParseOptions.Module));
                    continue;
                }

                var resolutions = new List<JsResolvedRequest>();

                foreach (var requested in requests.Specifiers)
                {
                    var resolved = Resolve(key, requested);

                    if (resolved.Length == 0 || !Modules.ContainsKey(resolved))
                    {
                        return VmArtifactProviderAnswer.NotFound(VmReason.ProviderArtifactNotFound);
                    }

                    resolutions.Add(new JsResolvedRequest(requested, resolved));

                    if (seen.Add(resolved))
                    {
                        pending.Enqueue(resolved);
                    }
                }

                units.Add(new JsModuleUnit(key, text, SliceParseOptions.Module, resolutions));
            }

            var compiled = JsCompiler.Compile([], units, new JsCompileRequest());

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return VmArtifactProviderAnswer.Refused(VmReason.SemanticValidationFailed);
            }

            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            return VmArtifactProviderAnswer.Provided(in descriptor, compiled.Artifact);
        }

        /// <summary>
        /// Answers an evaluation request or an embedder's script, for the checks that need eval code
        /// to run (JSeal I12-upstream). A host script is placed at <c>script:</c> and its source
        /// name, which is the referrer a dynamic <c>import()</c> in it carries.
        /// </summary>
        private static VmArtifactProviderAnswer AnswerProgram(System.ReadOnlySpan<byte> payload)
        {
            if (!JsCompiler.TryReadProgramRequest(payload, out var script))
            {
                return VmArtifactProviderAnswer.Refused(VmReason.MalformedEncoding);
            }

            if (payload.Length != 0 && payload[0] == JsFormat.ScriptRequestMark)
            {
                script = script with { Referrer = "script:" + script.SourceName };
            }

            var compiled = JsCompiler.Compile([script], [], new JsCompileRequest());

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return VmArtifactProviderAnswer.Refused(VmReason.ProviderRefused);
            }

            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            return VmArtifactProviderAnswer.Provided(in descriptor, compiled.Artifact);
        }

        /// <summary>Confirms a resolution the profile asks about, unless told to refuse.</summary>
        internal bool Confirms(System.ReadOnlySpan<byte> request)
        {
            var parts = JsFormat.DecodeText(request).Split('\0');

            return !RefuseConfirmation &&
                parts.Length == 3 &&
                string.Equals(Resolve(parts[0], parts[1]), parts[2], System.StringComparison.Ordinal);
        }

        /// <summary>This embedder's rule: <c>./name</c> against the referrer's directory.</summary>
        private static string Resolve(string referrer, string specifier)
        {
            if (!specifier.StartsWith("./", System.StringComparison.Ordinal))
            {
                return string.Empty;
            }

            var directory = referrer.Length == 0
                ? "mem:/"
                : referrer[..(referrer.LastIndexOf('/') + 1)];

            return directory + specifier[2..];
        }
    }
}
