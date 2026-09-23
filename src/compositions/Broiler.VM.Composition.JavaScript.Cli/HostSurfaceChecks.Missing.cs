// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using Broiler.VM.Profile.JavaScript;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>
/// <see cref="JsHostValue.Missing"/> at every crossing that resolves a host value (JSeal
/// JSD-0024-missing, decision JSD-0024 section 20).
/// </summary>
/// <remarks>
/// <b>Missing is the host's "no value", and the realm's own "no value" is the marker an
/// uninitialised binding holds.</b> Before section 20 the one resolved to the other, so a host that
/// passed Missing where a value was required injected a binding marker into guest code, which then
/// threw "Cannot access a binding before initialisation" from a place that has no binding. Each row
/// below presents Missing at a crossing and records either the refusal the host is told or what the
/// guest sees.
/// </remarks>
internal static partial class HostSurfaceChecks
{
    /// <summary>Runs the Missing checks, answering the failure count.</summary>
    private static int RunMissing()
    {
        var failures = 0;

        failures += ModuleCheck(
            "Missing is refused where a host member requires a value, and nothing is written",
            [],
            new Dictionary<string, string>
            {
                ["prep"] =
                    "globalThis.echo = function () { return arguments.length + ':' + typeof arguments[0]; }; " +
                    "globalThis.Made = function (a) { this.seen = typeof a; }; " +
                    "globalThis.o = {}; globalThis.list = [];",
                ["check"] =
                    "print('o=' + Object.keys(o).join(',') + ':' + ('x' in o) + ':' + ('y' in o) + ':' + list.length);",
            },
            session =>
            {
                session.Script("prep");
                session.Turn(realm =>
                {
                    var echo = realm.GetProperty(realm.Global, "echo");
                    var made = realm.GetProperty(realm.Global, "Made");
                    var o = realm.GetProperty(realm.Global, "o");
                    var list = realm.GetProperty(realm.Global, "list");
                    var capability = realm.NewPromiseCapability();

                    session.Record("invoke=" + MissingRefused(() => realm.Invoke(echo, JsHostValue.Undefined, [JsHostValue.Missing])));
                    session.Record("construct=" + MissingRefused(() => realm.Construct(made, [JsHostValue.Missing])));
                    session.Record("set=" + MissingRefused(() => realm.SetProperty(o, "x", JsHostValue.Missing)));
                    session.Record("define=" + MissingRefused(() => realm.DefineValue(o, "y", JsHostValue.Missing)));
                    session.Record("index=" + MissingRefused(() => realm.DefineIndex(list, 0, JsHostValue.Missing)));
                    session.Record("array=" + MissingRefused(() => realm.NewArray([JsHostValue.Number(1), JsHostValue.Missing])));
                    session.Record("get=" + MissingRefused(() => realm.GetProperty(JsHostValue.Missing, "x")));
                    session.Record("get-index=" + MissingRefused(() => realm.GetIndex(JsHostValue.Missing, 0)));
                    session.Record("set-target=" + MissingRefused(() => realm.SetProperty(JsHostValue.Missing, "x", JsHostValue.Undefined)));
                    session.Record("throw=" + MissingRefused(() => realm.Throw(JsHostValue.Missing)));
                    session.Record("resolve=" + MissingRefused(() => realm.ResolvePromise(capability, JsHostValue.Missing)));
                    session.Record("clone=" + MissingRefused(() => realm.DetachClone(JsHostValue.Missing)));

                    // AND WHERE A VALUE IS ONLY READ, Missing is what the language calls undefined.
                    session.Record(
                        "convert=" + realm.ToJsString(JsHostValue.Missing) + "," +
                        realm.ToNumber(JsHostValue.Missing).ToString(System.Globalization.CultureInfo.InvariantCulture) + "," +
                        realm.ToBoolean(JsHostValue.Missing));
                });
                session.Script("check");
            },
            [
                "invoke=ArgumentException:arguments",
                "construct=ArgumentException:arguments",
                "set=ArgumentException:value",
                "define=ArgumentException:value",
                "index=ArgumentException:value",
                "array=ArgumentException:elements",
                "get=ArgumentException:target",
                "get-index=ArgumentException:target",
                "set-target=ArgumentException:target",
                "throw=ArgumentException:value",
                "resolve=ArgumentException:value",
                "clone=ArgumentException:value",
                "convert=undefined,NaN,False",
                "o=:false:false:0",
            ]);

        failures += ModuleCheck(
            "Missing a host answers the guest is undefined: a receiver, a body's return and a hook's answer",
            [],
            new Dictionary<string, string>
            {
                ["prep"] = "globalThis.receiver = function () { 'use strict'; return this === undefined; };",
                ["read"] =
                    "var r = nothing(); print('body=' + typeof r + ':' + (r === undefined)); " +
                    "print('named=' + typeof ex.hole + ':' + ('hole' in ex) + ':' + (ex.hole === undefined)); " +
                    "print('indexed=' + typeof ex[0] + ':' + (ex[0] === undefined)); " +
                    "print('built=' + typeof new Built().value);",
            },
            session =>
            {
                session.Script("prep");
                session.Turn(realm =>
                {
                    session.Record("receiver=" + realm.ToBoolean(realm.Invoke(
                        realm.GetProperty(realm.Global, "receiver"), JsHostValue.Missing)));

                    realm.DefineValue(
                        realm.Global, "nothing", realm.NewMethod("nothing", (_, _, _) => JsHostValue.Missing));
                    realm.DefineValue(realm.Global, "ex", realm.NewExotic(new AnswersMissing()));
                    realm.DefineValue(
                        realm.Global,
                        "Built",
                        realm.NewConstructor("Built", (r, self, _) =>
                        {
                            // A CONSTRUCT BODY THAT ANSWERS MISSING keeps the object the realm made.
                            r.DefineValue(self, "value", JsHostValue.Number(1));
                            return JsHostValue.Missing;
                        }));
                });
                session.Script("read");
            },
            [
                "receiver=True",
                "body=undefined:true",
                "named=undefined:true:true",
                "indexed=undefined:true",
                "built=number",
            ]);

        return failures;
    }

    /// <summary>Names the argument refusal a member met, or says what it answered instead.</summary>
    private static string MissingRefused(System.Action act)
    {
        try
        {
            act();
            return "not refused";
        }
        catch (System.ArgumentException refused)
        {
            return "ArgumentException:" + refused.ParamName;
        }
        catch (JsHostThrowException thrown)
        {
            return "guest throw: " + thrown.Message;
        }
        catch (JsHostSurfaceException refusal)
        {
            return "surface refusal: " + refusal.Refusal;
        }
    }

    /// <summary>An exotic object whose every named and indexed answer is Missing.</summary>
    private sealed class AnswersMissing : IJsHostExotic
    {
        public bool TryGetNamed(JsHostRealm realm, string name, out JsHostValue value)
        {
            value = JsHostValue.Missing;
            return name == "hole";
        }

        public IReadOnlyList<string> SupportedNames(JsHostRealm realm) => ["hole"];

        public bool TryGetIndex(JsHostRealm realm, uint index, out JsHostValue value)
        {
            value = JsHostValue.Missing;
            return index == 0;
        }

        public bool TrySetNamed(JsHostRealm realm, string name, JsHostValue value) => false;

        public uint IndexedLength(JsHostRealm realm) => 1;
    }
}
