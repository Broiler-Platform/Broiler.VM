// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using Broiler.VM.Profile.JavaScript;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>
/// A linked module's <c>[[Status]]</c>, <c>[[EvaluationError]]</c>, <c>[[HasTLA]]</c> and
/// <c>[[CycleRoot]]</c>, read by the host (JSeal I11-upstream, decision JSD-0024 section 20).
/// </summary>
/// <remarks>
/// <b>Each row is a place where an embedder that infers status from its evaluation promises has to
/// guess</b>: a walk in progress, an awaiting module and its importer, a throw inside a cycle that
/// leaves a dependency unreached, a rejection in a graph a host callback evaluated, and equal
/// primitive errors from modules that can and cannot await. The transcript records what the realm
/// answers at each moment.
/// </remarks>
internal static partial class HostSurfaceChecks
{
    /// <summary>Runs the module-state checks, answering the failure count.</summary>
    private static int RunModuleStates()
    {
        var failures = 0;

        failures += ModuleCheck(
            "a host reads each module's status and top-level await across a walk, an await and a drain",
            new Dictionary<string, string>
            {
                ["mem:/a"] = "import './b'; export const a = 1;",
                ["mem:/b"] = "export const b = 2; probe();",
                ["mem:/slow"] = "export let v = 0; await null; v = 1;",
                ["mem:/user"] = "import { v } from './slow'; export const seen = v;",
            },
            new Dictionary<string, string> { ["noop"] = "0;" },
            session =>
            {
                session.Turn(realm =>
                {
                    // A HOST FUNCTION A MODULE BODY CALLS DURING THE WALK: both modules are on it.
                    realm.DefineValue(
                        realm.Global,
                        "probe",
                        realm.NewMethod("probe", (r, _, _) =>
                        {
                            session.Record("during " + State(r, "mem:/a"));
                            session.Record("during " + State(r, "mem:/b"));
                            return JsHostValue.Undefined;
                        }));

                    var a = realm.LoadModule("./a");
                    session.Record(State(realm, "mem:/a"));
                    session.Record(State(realm, "mem:/b"));
                    session.Record(State(realm, "mem:/nowhere"));
                    _ = realm.EvaluateModule(a);
                    session.Record(State(realm, "mem:/a"));
                    session.Record(State(realm, "mem:/b"));

                    _ = realm.EvaluateModule(realm.LoadModule("./user"));
                    session.Record(State(realm, "mem:/slow"));
                    session.Record(State(realm, "mem:/user"));
                });
                session.Drain();
                session.Turn(realm =>
                {
                    session.Record(State(realm, "mem:/slow"));
                    session.Record(State(realm, "mem:/user"));
                    session.Record("null-key=" + NullKey(realm));
                });
            },
            [
                "mem:/a=Linked:tla=False:error=-:root=",
                "mem:/b=Linked:tla=False:error=-:root=",
                "mem:/nowhere=absent",
                "during mem:/a=Evaluating:tla=False:error=-:root=",
                "during mem:/b=Evaluating:tla=False:error=-:root=",
                "mem:/a=Evaluated:tla=False:error=-:root=mem:/a",
                "mem:/b=Evaluated:tla=False:error=-:root=mem:/b",
                "mem:/slow=EvaluatingAsync:tla=True:error=-:root=mem:/slow",
                "mem:/user=EvaluatingAsync:tla=False:error=-:root=mem:/user",
                "mem:/slow=Evaluated:tla=True:error=-:root=mem:/slow",
                "mem:/user=Evaluated:tla=False:error=-:root=mem:/user",
                "null-key=ArgumentNullException",
            ]);

        failures += ModuleCheck(
            "a throw inside a cycle errors the modules on the walk with one value and leaves an unreached one linked",
            new Dictionary<string, string>
            {
                ["mem:/root"] = "import './c1'; import './later';",
                ["mem:/c1"] = "import './c2'; export const c1 = 1;",
                ["mem:/c2"] = "import './c1'; throw new RangeError('c2');",
                ["mem:/later"] = "export const later = 1;",
            },
            new Dictionary<string, string>
            {
                ["catch"] = "done.then(null, function (e) { globalThis.reason = e; });",
            },
            session =>
            {
                session.Turn(realm =>
                    realm.DefineValue(realm.Global, "done", realm.EvaluateModule(realm.LoadModule("./root"))));
                session.Script("catch");
                session.Drain();
                session.Turn(realm =>
                {
                    foreach (var key in new[] { "mem:/root", "mem:/c1", "mem:/c2", "mem:/later" })
                    {
                        session.Record(State(realm, key));
                    }

                    var reason = realm.GetProperty(realm.Global, "reason");
                    var identical = true;

                    foreach (var key in new[] { "mem:/root", "mem:/c1", "mem:/c2" })
                    {
                        identical &= realm.TryGetModuleState(key, out var state) && state.EvaluationError == reason;
                    }

                    session.Record("identical=" + identical);
                });
            },
            [
                "mem:/root=Evaluated:tla=False:error=RangeError: c2:root=",
                "mem:/c1=Evaluated:tla=False:error=RangeError: c2:root=",
                "mem:/c2=Evaluated:tla=False:error=RangeError: c2:root=",
                "mem:/later=Linked:tla=False:error=-:root=",
                "identical=True",
            ]);

        failures += ModuleCheck(
            "a graph a host callback evaluates stays linked until a drain runs it, then carries its error",
            new Dictionary<string, string>
            {
                ["mem:/outer"] = "globalThis.innerDone = startInner(); export const o = 1;",
                ["mem:/bad"] = "throw new TypeError('bad');",
            },
            new Dictionary<string, string>
            {
                ["catch"] = "innerDone.then(null, function (e) { print('rejected=' + e.message); });",
            },
            session =>
            {
                session.Turn(realm =>
                {
                    realm.DefineValue(
                        realm.Global,
                        "startInner",
                        realm.NewMethod("startInner", (r, _, _) =>
                        {
                            var done = r.EvaluateModule(r.LoadModule("./bad"));
                            session.Record("inside " + State(r, "mem:/bad"));
                            return done;
                        }));

                    _ = realm.EvaluateModule(realm.LoadModule("./outer"));
                    session.Record(State(realm, "mem:/outer"));
                    session.Record(State(realm, "mem:/bad"));
                });
                session.Script("catch");
                session.Drain();
                session.Turn(realm => session.Record(State(realm, "mem:/bad")));
            },
            [
                "inside mem:/bad=Linked:tla=False:error=-:root=",
                "mem:/outer=Evaluated:tla=False:error=-:root=mem:/outer",
                "mem:/bad=Linked:tla=False:error=-:root=",
                "rejected=bad",
                "mem:/bad=Evaluated:tla=False:error=TypeError: bad:root=",
            ]);

        failures += ModuleCheck(
            "equal primitive errors are told apart by module, and a cycle member finishes before its awaiting root",
            new Dictionary<string, string>
            {
                ["mem:/both"] = "import './p1'; import './p2'; import './p3';",
                ["mem:/p1"] = "await null; throw 1;",
                ["mem:/p2"] = "throw 1;",
                ["mem:/p3"] = "await null; export const ok = 1;",
                ["mem:/ca"] = "import './cb'; await null;",
                ["mem:/cb"] = "import './ca';",
            },
            new Dictionary<string, string> { ["noop"] = "0;" },
            session =>
            {
                session.Turn(realm =>
                {
                    _ = realm.EvaluateModule(realm.LoadModule("./both"));
                    _ = realm.EvaluateModule(realm.LoadModule("./ca"));

                    foreach (var key in new[] { "mem:/both", "mem:/p1", "mem:/p2", "mem:/p3", "mem:/ca", "mem:/cb" })
                    {
                        session.Record(State(realm, key));
                    }
                });
                session.Drain();
                session.Turn(realm =>
                {
                    foreach (var key in new[] { "mem:/p1", "mem:/p3", "mem:/ca" })
                    {
                        session.Record(State(realm, key));
                    }
                });
            },
            [
                "mem:/both=Evaluated:tla=False:error=1:root=",
                "mem:/p1=EvaluatingAsync:tla=True:error=-:root=mem:/p1",
                "mem:/p2=Evaluated:tla=False:error=1:root=",
                "mem:/p3=Linked:tla=True:error=-:root=",
                "mem:/ca=EvaluatingAsync:tla=True:error=-:root=mem:/ca",
                "mem:/cb=Evaluated:tla=False:error=-:root=mem:/ca",
                "mem:/p1=Evaluated:tla=True:error=1:root=mem:/p1",
                "mem:/p3=Linked:tla=True:error=-:root=",
                "mem:/ca=Evaluated:tla=True:error=-:root=mem:/ca",
            ]);

        return failures;
    }

    /// <summary>One module's state as a transcript line.</summary>
    private static string State(JsHostRealm realm, string key)
    {
        if (!realm.TryGetModuleState(key, out var state))
        {
            return key + "=absent";
        }

        var error = state.HasEvaluationError ? realm.ToJsString(state.EvaluationError) : "-";

        return state.Key + "=" + state.Status + ":tla=" + state.HasTopLevelAwait + ":error=" + error +
            ":root=" + state.CycleRoot;
    }

    /// <summary>Names what a null key is refused with.</summary>
    private static string NullKey(JsHostRealm realm)
    {
        try
        {
            _ = realm.TryGetModuleState(null!, out _);
            return "not refused";
        }
        catch (System.ArgumentNullException)
        {
            return "ArgumentNullException";
        }
    }
}
