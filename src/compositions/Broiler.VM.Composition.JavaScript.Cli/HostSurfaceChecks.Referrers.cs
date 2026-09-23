// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using Broiler.VM.Profile.JavaScript;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>
/// The referrer a guest <c>import()</c> in eval code or in a <c>Function</c> body carries (JSeal
/// I12-upstream, decision JSD-0024 section 20).
/// </summary>
/// <remarks>
/// <b>The language resolves such an import against <c>GetActiveScriptOrModule()</c></b>: eval code
/// takes the script or module of the code that evaluated it, a function takes the one that was
/// active when it was created, and a built-in - <c>eval</c> itself, the <c>Function</c>
/// constructor, a host function - has none of its own, so the one below it on the stack answers.
/// Each row records what the embedder's loader was offered, which is the referrer the realm
/// computed. The lane's provider answers evaluation requests here, so eval code runs at all.
/// </remarks>
internal static partial class HostSurfaceChecks
{
    /// <summary>Runs the referrer checks, answering the failure count.</summary>
    private static int RunReferrers()
    {
        var failures = 0;

        failures += ModuleCheck(
            "import() in eval and Function code carries the key of the module whose code created it",
            new Dictionary<string, string>
            {
                ["mem:/dir/m"] =
                    "export function viaIndirect(s) { return (0, eval)('import(' + JSON.stringify(s) + ')'); } " +
                    "export function viaDirect(s) { return eval('import(s)'); } " +
                    "export function viaFunction(s) { return Function('s', 'return import(s)')(s); } " +
                    "export function viaArrowInEval(s) { return (0, eval)('(x => import(x))')(s); } " +
                    "export function viaFunctionInEval(s) { return (0, eval)('Function(\"s\", \"return import(s)\")')(s); } " +
                    "export function viaHost(s) { return hostEval('import(' + JSON.stringify(s) + ')'); } " +
                    "export const made = Function('s', 'return import(s)');",
                ["mem:/dir/tla"] = "await null; export const p = (0, eval)('import(\"./b\")');",
                ["mem:/dir/b"] = "export const where = 'dir/b';",
                ["mem:/b"] = "export const where = 'b';",
            },
            new Dictionary<string, string>
            {
                ["ask"] =
                    "var got = []; function keep(p) { p.then(function (n) { got.push(n.where); }, function (e) { got.push(e.name); }); } " +
                    "keep(mns.viaIndirect('./b')); keep(mns.viaDirect('./b')); keep(mns.viaFunction('./b')); " +
                    "keep(mns.viaArrowInEval('./b')); keep(mns.viaFunctionInEval('./b')); keep(mns.viaHost('./b'));",
                ["results"] = "print('got=' + got.join(','));",
            },
            session =>
            {
                session.Lane.AnswerPrograms = true;
                session.Turn(realm =>
                {
                    // A HOST FUNCTION CALLS eval: a built-in below a host body below module code, so
                    // the module is the active one.
                    realm.DefineValue(
                        realm.Global,
                        "hostEval",
                        realm.NewMethod("hostEval", (r, _, arguments) => r.Invoke(
                            r.GetProperty(r.Global, "eval"), JsHostValue.Undefined, [arguments[0]])));

                    var m = realm.LoadModule("./dir/m");
                    realm.DefineValue(realm.Global, "mns", m.Namespace);
                    _ = realm.EvaluateModule(m);
                });
                session.Drain();
                session.Lane.Defer = true;
                session.Script("ask");
                session.RecordOffers();

                // A FUNCTION MODULE CODE MADE, called by the host with no guest code below it: its
                // script or module was fixed when it was created.
                session.Turn(realm =>
                {
                    var made = realm.GetProperty(realm.GetProperty(realm.Global, "mns"), "made");
                    _ = realm.Invoke(made, JsHostValue.Undefined, [JsHostValue.String("./b")]);
                });
                session.RecordOffers();

                // A FUNCTION THE HOST MADE, with no guest code on the stack at all: the specification
                // leaves no referrer, and the loader is offered none.
                session.Turn(realm =>
                {
                    var function = realm.Construct(
                        realm.GetProperty(realm.Global, "Function"), [JsHostValue.String("return import('./b')")]);
                    _ = realm.Invoke(function, JsHostValue.Undefined);
                });
                session.RecordOffers();

                // A MODULE BODY THAT AWAITS runs its eval from an async frame.
                session.Turn(realm => _ = realm.EvaluateModule(realm.LoadModule("./dir/tla")));
                session.Drain();
                session.RecordOffers();

                session.Turn(realm =>
                {
                    for (var at = 0; at < 6; at++)
                    {
                        realm.CompleteModuleRequest(session.Lane.Offered[at]);
                    }
                });
                session.Drain();
                session.Script("results");
            },
            [
                "offered=mem:/dir/m|./b",
                "offered=mem:/dir/m|./b",
                "offered=mem:/dir/m|./b",
                "offered=mem:/dir/m|./b",
                "offered=mem:/dir/m|./b",
                "offered=mem:/dir/m|./b",
                "offered=mem:/dir/m|./b",
                "offered=|./b",
                "offered=mem:/dir/tla|./b",
                "got=dir/b,dir/b,dir/b,dir/b,dir/b,dir/b",
            ]);

        failures += ModuleCheck(
            "import() in eval and Function code inside a host script carries the script's referrer",
            new Dictionary<string, string>
            {
                ["mem:/b"] = "export const where = 'b';",
            },
            new Dictionary<string, string> { ["noop"] = "0;" },
            session =>
            {
                session.Lane.AnswerPrograms = true;
                session.Lane.Defer = true;
                session.Turn(realm => realm.EvaluateScript(
                    "import('./b'); (0, eval)('import(\"./b\")'); eval('import(\"./b\")'); " +
                    "Function('return import(\"./b\")')(); " +
                    "var later = Function('return import(\"./b\")'); function declared() { return (0, eval)('import(\"./b\")'); }",
                    "dir/page.js"));
                session.RecordOffers();
                session.Turn(realm =>
                {
                    _ = realm.Invoke(realm.GetProperty(realm.Global, "later"), JsHostValue.Undefined);
                    _ = realm.Invoke(realm.GetProperty(realm.Global, "declared"), JsHostValue.Undefined);
                });
                session.RecordOffers();
            },
            [
                "offered=script:dir/page.js|./b",
                "offered=script:dir/page.js|./b",
                "offered=script:dir/page.js|./b",
                "offered=script:dir/page.js|./b",
                "offered=script:dir/page.js|./b",
                "offered=script:dir/page.js|./b",
            ]);

        failures += ModuleCheck(
            "import() in eval and Function code a promise job runs carries the referrer that queued the job",
            new Dictionary<string, string>
            {
                ["mem:/dir/j"] =
                    "export function viaJob() { " +
                    "Promise.resolve('import(\"./b\")').then(eval); " +
                    "Promise.resolve('return import(\"./b\")').then(Function).then(f => f()); }",
                ["mem:/dir/b"] = "export const where = 'dir/b';",
            },
            new Dictionary<string, string> { ["noop"] = "0;" },
            session =>
            {
                session.Lane.AnswerPrograms = true;
                session.Turn(realm =>
                {
                    var j = realm.LoadModule("./dir/j");
                    realm.DefineValue(realm.Global, "jns", j.Namespace);
                    _ = realm.EvaluateModule(j);
                });
                session.Drain();
                session.Lane.Defer = true;

                // THE JOBS ARE QUEUED BY MODULE CODE and run later from the drain, with no guest
                // frame below the built-in eval and Function they call.
                session.Turn(realm => _ = realm.Invoke(
                    realm.GetProperty(realm.GetProperty(realm.Global, "jns"), "viaJob"), JsHostValue.Undefined));
                session.Drain();
                session.RecordOffers();

                // A HOST JOB QUEUED WITH NO GUEST CODE ON THE STACK has no script or module, and its
                // eval code is offered none.
                session.Turn(realm => realm.EnqueueJob(() => _ = realm.Invoke(
                    realm.GetProperty(realm.Global, "eval"), JsHostValue.Undefined,
                    [JsHostValue.String("import('./b')")])));
                session.Drain();
                session.RecordOffers();
            },
            [
                "offered=mem:/dir/j|./b",
                "offered=mem:/dir/j|./b",
                "offered=|./b",
            ]);

        return failures;
    }
}
