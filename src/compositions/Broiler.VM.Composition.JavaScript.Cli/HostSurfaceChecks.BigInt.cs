// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Numerics;
using Broiler.VM.Profile.JavaScript;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>
/// The checks for a BigInt at the host surface (JSeal B06, decision JSD-0024 section 19): the
/// crossing in both directions, the conversions, equality, a thrown BigInt, and the clone.
/// </summary>
/// <remarks>
/// <para>
/// <b>The integers are judged by the guest wherever the guest can judge them.</b> The host answers a
/// digest of what it read - kind, sign, bit length and the value modulo a prime - and the guest
/// computes the same digest with its own BigInt arithmetic, so a crossing that answered the right
/// kind and a rounded value, the most plausible wrong answer, fails. A value the host built is
/// compared by the guest with <c>===</c> against a literal it wrote itself.
/// </para>
/// <para>
/// <b>The widest value is the ceiling itself</b>: a BigInt of exactly 2^20 bits crosses, and one
/// bit more is the <c>RangeError</c> the language's own operations answer past it.
/// </para>
/// </remarks>
internal static partial class HostSurfaceChecks
{
    /// <summary>The digest the guest computes, as <see cref="DemonstrationSurface.Describe"/> does.</summary>
    private const string BigIntPrelude =
        """
        function describe(v) {
            if (typeof v !== 'bigint') { return 'not a bigint'; }
            var a = v < 0n ? -v : v;
            return 'BigInt:' + (v > 0n ? 1 : v < 0n ? -1 : 0) + ':' +
                (a === 0n ? 0 : a.toString(2).length) + ':' + (a % 1000000007n);
        }
        function same(got, want) { return got === want ? 'same' : got + ' vs ' + want; }

        """;

    /// <summary>Runs the BigInt checks, answering the failure count.</summary>
    private static int RunBigInt()
    {
        var failures = 0;

        failures += Check(
            "a BigInt crosses to the host and back exactly: zero, negative, wide, and the widest admitted",
            BigIntPrelude +
            """
            var values = [0n, -1n, 2n ** 64n, -(2n ** 200n) + 1n, (1n << 1048575n) + 12345n];
            for (var k = 0; k < values.length; k++) {
                var v = values[k];
                print(k + '=' + same(broilerHost.bigDescribe(v), describe(v)) + ':' + (broilerHost.bigEcho(v) === v));
            }
            print('number=' + broilerHost.bigDescribe(10) + ',object=' + broilerHost.bigDescribe(Object(10n)));
            """,
            ["0=same:true", "1=same:true", "2=same:true", "3=same:true", "4=same:true", "number=Number,object=Object"]);

        failures += Check(
            "a BigInt the host builds reaches the guest exactly, and one past the ceiling is a RangeError",
            """
            var made = broilerHost.bigMake('-123456789012345678901234567890');
            print('made=' + typeof made + ':' + (made === -123456789012345678901234567890n));
            print('widest=' + (broilerHost.bigWidest() === 1n << 1048575n));
            try { broilerHost.bigTooWide(); print('no-throw'); }
            catch (e) { print('too-wide=' + (e instanceof RangeError)); }
            var o = {};
            print('host-set=' + broilerHost.bigSetTooWide(o) + ':' + ('wide' in o));
            """,
            ["made=bigint:true", "widest=true", "too-wide=true", "host-set=RangeError:false"]);

        failures += Check(
            "ToBoolean, ToJsString and ToNumber answer the language's answers for a BigInt",
            """
            var d = broilerHost.htmlDda;
            var cases = [0n, 1n, -1n, 2n ** 70n, 0, NaN, '', 'a', null, undefined, {}, d, Object(0n)];
            var truth = [];
            for (var k = 0; k < cases.length; k++) { truth.push(broilerHost.bigTruth(cases[k])); }
            print('truth=' + truth.join(','));
            print('missing=' + broilerHost.bigTruthMissing());
            print('string=' + broilerHost.bigString(-42n) + ',' + broilerHost.bigString(2n ** 64n));
            print('number=' + broilerHost.bigNumber(1n) + ',' + broilerHost.bigNumber(Object(1n)) + ',' + broilerHost.bigNumber('7'));
            """,
            [
                "truth=false,true,true,true,false,false,false,true,false,false,true,false,true",
                "missing=false",
                "string=-42,18446744073709551616",
                "number=TypeError,TypeError,7",
            ]);

        failures += Check(
            "a BigInt equals another of the same value, however each crossed, and never a Number",
            """
            var big = 2n ** 100n, same = 2n ** 99n * 2n;
            print('value=' + broilerHost.bigEquals(10n, 5n + 5n) + ',' + broilerHost.bigEquals(big, same) + ',' +
                broilerHost.bigEquals(0n, -0n));
            print('kind=' + broilerHost.bigEquals(10n, 10) + ',' + broilerHost.bigEquals(1n, Object(1n)) + ',' +
                broilerHost.bigEquals(Object(1n), Object(1n)));
            print('host-built=' + broilerHost.bigEqualsTen(10n) + ',' + broilerHost.bigEqualsTen(11n));
            """,
            ["value=true:true,true:true,true:true", "kind=false,false,false", "host-built=true,false"]);

        failures += Check(
            "properties, calls and constructs carry a BigInt both ways",
            """
            var o = { src: -(2n ** 80n) };
            print('read=' + broilerHost.bigProps(o));
            print('written=' + (o.doubled === -(2n ** 81n)) + ',' + (o.defined === 7n) + ',' + (o[0] === 8n));
            print('call=' + broilerHost.bigInvoke(function (x) { return x * 3n; }));
            print('construct=' + broilerHost.bigConstruct(function C(x) { this.v = x + 1n; }));
            """,
            ["read=-1208925819614629174706176", "written=true,true,true", "call=21", "construct=42"]);

        failures += Check(
            "a BigInt the guest throws reaches the host carrying it, and a host re-raise throws the same value",
            """
            print('host=' + broilerHost.bigThrow(function () { throw -5n; }));
            try { broilerHost.bigRethrow(function () { throw 2n ** 70n; }); print('no-throw'); }
            catch (e) { print('guest=' + (e === 2n ** 70n)); }
            print('number=' + broilerHost.bigThrow(function () { throw 2; }));
            """,
            ["host=BigInt:-5", "guest=true", "number=Number:"]);

        failures += Check(
            "a BigInt and a BigInt object clone through DetachClone and AdoptClone",
            """
            var c = broilerHost.bigClone(2n ** 100n);
            print('primitive=' + typeof c + ':' + (c === 2n ** 100n));
            var o = Object(-7n), co = broilerHost.bigClone(o);
            print('object=' + [typeof co, co !== o, Object.getPrototypeOf(co) === BigInt.prototype,
                co.valueOf() === -7n, Object.prototype.toString.call(co)].join(','));
            var shared = Object(9n);
            var g = broilerHost.bigClone({ a: 1n, b: [shared, shared, 3n], m: new Map([[4n, 5n]]), s: new Set([6n]) });
            print('graph=' + [g.a === 1n, g.b[0] === g.b[1], g.b[0] instanceof BigInt, g.b[2] === 3n,
                g.m.get(4n) === 5n, g.s.has(6n)].join(','));
            """,
            [
                "primitive=bigint:true",
                "object=object,true,true,true,[object BigInt]",
                "graph=true,true,true,true,true,true",
            ]);

        failures += Check(
            "a BigInt a host body throws or an exotic hook answers reaches the guest as itself",
            """
            try { broilerHost.bigRaise(); print('no-throw'); } catch (e) { print('thrown=' + (e === -5n)); }
            print('exotic=' + (broilerHost.hostile('').big === 5n));
            """,
            ["thrown=true", "exotic=true"]);

        failures += CheckBigIntCloneToADecliningRealm();
        failures += CheckBigIntRoutesIntoADecliningRealm();

        return failures;
    }

    /// <summary>
    /// A carrier holding a BigInt is refused by a realm whose composition declined the surface,
    /// before it claims the carrier's moved bytes, and adopted whole by one that admits it.
    /// </summary>
    private static int CheckBigIntCloneToADecliningRealm()
    {
        const string Title =
            "a realm that declined BigInt refuses a BigInt from the host and a carrier holding one, and claims nothing";

        var lines = new List<string>();
        JsHostCloneCarrier? carrier = null;

        var failure = OnThread(() =>
        {
            using var source = CloneParty.Open(
                [("make", "var b = new ArrayBuffer(2); var msg = { n: 2n ** 70n, o: Object(-3n), b: b };")],
                out var sourceFailure);

            if (source is null)
            {
                return sourceFailure;
            }

            var made = source.Run("make")
                ?? source.Turn(realm => carrier = realm.DetachClone(
                    realm.GetProperty(realm.Global, "msg"), [realm.GetProperty(realm.Global, "b")]));

            if (made is not null || carrier is null)
            {
                return "the source: " + (made ?? "no carrier");
            }

            using (var declining = CloneParty.Open(
                [("first", "print('declining');")],
                out var decliningFailure,
                surfaces: [JavaScriptProfile.BinaryManifest]))
            {
                if (declining is null)
                {
                    return decliningFailure;
                }

                var refused = declining.Run("first")
                    ?? declining.Turn(turn =>
                    {
                        lines.Add("host-value=" + Refused(() =>
                        {
                            turn.SetProperty(turn.Global, "x", JsHostValue.BigInt(1));
                            return 0;
                        }));

                        lines.Add("carrier=" + CloneThrow(turn, () => turn.AdoptClone(carrier!)));
                        lines.Add("still-whole=" + !carrier!.IsConsumed);
                    });

                if (refused is not null)
                {
                    return refused;
                }

                lines.AddRange(declining.Printed);
            }

            using var admitting = CloneParty.Open(
                [("read", "print('adopted=' + [got.n === 2n ** 70n, got.o.valueOf() === -3n, got.b.byteLength].join(','));")],
                out var admittingFailure);

            if (admitting is null)
            {
                return admittingFailure;
            }

            return admitting.Turn(turn => turn.DefineValue(turn.Global, "got", turn.AdoptClone(carrier!)))
                ?? admitting.Run("read")
                ?? Append(lines, admitting.Printed);
        });

        if (failure is not null)
        {
            return Report(Title, failure);
        }

        return Compare(
            Title,
            lines,
            [
                "host-value=SurfaceDeclined",
                "carrier=TypeError:DataCloneError",
                "still-whole=True",
                "declining",
                "adopted=true,true,2",
            ]);
    }

    /// <summary>
    /// A BigInt that reaches a realm whose composition declined the surface is a <c>TypeError</c> the
    /// guest can catch, whichever way the host hands it over: returned from a body, thrown from one,
    /// or answered by an exotic object's hook (VM-FIX-I, JSD-0024 section 19).
    /// </summary>
    private static int CheckBigIntRoutesIntoADecliningRealm()
    {
        const string Title =
            "a BigInt a host body returns or throws, or an exotic hook answers, is a TypeError in a realm that declined BigInt";

        var lines = new List<string>();

        var failure = OnThread(() =>
        {
            using var declining = CloneParty.Open(
                [(
                    "routes",
                    """
                    function t(f) {
                        try { f(); return 'no-throw'; }
                        catch (e) { return e instanceof TypeError ? 'TypeError' : 'other:' + e; }
                    }
                    print('returned=' + t(function () { return broilerHost.bigMake('5'); }));
                    print('thrown=' + t(function () { broilerHost.bigRaise(); }));
                    print('exotic=' + t(function () { return broilerHost.hostile('').big; }));
                    print('after=' + typeof BigInt);
                    """
                )],
                out var decliningFailure,
                surfaces: [JavaScriptProfile.BinaryManifest]);

            if (declining is null)
            {
                return decliningFailure;
            }

            return declining.Run("routes") ?? Append(lines, declining.Printed);
        });

        if (failure is not null)
        {
            return Report(Title, failure);
        }

        return Compare(
            Title,
            lines,
            ["returned=TypeError", "thrown=TypeError", "exotic=TypeError", "after=undefined"]);
    }

    /// <summary>Names the guest error an adoption threw and whether it is a DataCloneError.</summary>
    private static string CloneThrow(JsHostRealm realm, System.Func<JsHostValue> action)
    {
        try
        {
            _ = action();
            return "not thrown";
        }
        catch (JsHostThrowException thrown)
        {
            var message = realm.ToJsString(realm.GetProperty(thrown.Thrown, "message"));

            return realm.ToJsString(realm.GetProperty(thrown.Thrown, "name")) + ":" +
                (message.StartsWith("DataCloneError:", System.StringComparison.Ordinal) ? "DataCloneError" : message);
        }
    }

    private sealed partial class DemonstrationSurface
    {
        /// <summary>The digest the guest's <c>describe</c> computes, from the host's own reading.</summary>
        internal static string Describe(JsHostValue value)
        {
            if (value.AsBigInt() is not { } big)
            {
                return value.Kind.ToString();
            }

            var magnitude = BigInteger.Abs(big);

            return "BigInt:" + big.Sign + ":" + magnitude.GetBitLength() + ":" +
                (magnitude % 1000000007).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>Installs the BigInt members on <c>broilerHost</c>.</summary>
        private static void InstallBigInt(JsHostRealm realm, JsHostValue host)
        {
            realm.DefineValue(
                host,
                "bigDescribe",
                realm.NewMethod("bigDescribe", (_, _, arguments) => JsHostValue.String(Describe(Argument(arguments, 0))), 1));

            realm.DefineValue(
                host,
                "bigEcho",
                realm.NewMethod("bigEcho", (_, _, arguments) => Argument(arguments, 0), 1));

            realm.DefineValue(
                host,
                "bigMake",
                realm.NewMethod(
                    "bigMake",
                    (r, _, arguments) => JsHostValue.BigInt(
                        BigInteger.Parse(r.ToJsString(Argument(arguments, 0)), System.Globalization.CultureInfo.InvariantCulture)),
                    1));

            // THE CEILING: 2^20 bits is admitted and 2^20 + 1 is the RangeError, whichever way the
            // value is handed over - returned from a body, or written by the host itself.
            realm.DefineValue(
                host,
                "bigWidest",
                realm.NewMethod("bigWidest", (_, _, _) => JsHostValue.BigInt(BigInteger.One << ((1 << 20) - 1))));

            realm.DefineValue(
                host,
                "bigTooWide",
                realm.NewMethod("bigTooWide", (_, _, _) => JsHostValue.BigInt(BigInteger.One << (1 << 20))));

            realm.DefineValue(
                host,
                "bigSetTooWide",
                realm.NewMethod(
                    "bigSetTooWide",
                    (r, _, arguments) =>
                    {
                        try
                        {
                            r.SetProperty(Argument(arguments, 0), "wide", JsHostValue.BigInt(BigInteger.One << (1 << 20)));
                            return JsHostValue.String("set");
                        }
                        catch (JsHostThrowException thrown)
                        {
                            return JsHostValue.String(r.ToJsString(r.GetProperty(thrown.Thrown, "name")));
                        }
                    },
                    1));

            realm.DefineValue(
                host,
                "bigTruth",
                realm.NewMethod("bigTruth", (r, _, arguments) => JsHostValue.Boolean(r.ToBoolean(Argument(arguments, 0))), 1));

            realm.DefineValue(
                host,
                "bigTruthMissing",
                realm.NewMethod("bigTruthMissing", (r, _, _) => JsHostValue.Boolean(r.ToBoolean(JsHostValue.Missing))));

            realm.DefineValue(
                host,
                "bigString",
                realm.NewMethod("bigString", (r, _, arguments) => JsHostValue.String(r.ToJsString(Argument(arguments, 0))), 1));

            realm.DefineValue(
                host,
                "bigNumber",
                realm.NewMethod(
                    "bigNumber",
                    (r, _, arguments) =>
                    {
                        try
                        {
                            return JsHostValue.String(
                                r.ToNumber(Argument(arguments, 0)).ToString(System.Globalization.CultureInfo.InvariantCulture));
                        }
                        catch (JsHostThrowException thrown)
                        {
                            return JsHostValue.String(r.ToJsString(r.GetProperty(thrown.Thrown, "name")));
                        }
                    },
                    1));

            realm.DefineValue(
                host,
                "bigEquals",
                realm.NewMethod(
                    "bigEquals",
                    (_, _, arguments) =>
                    {
                        var left = Argument(arguments, 0);
                        var right = Argument(arguments, 1);

                        return JsHostValue.String(left.Equals(right)
                            ? "true:" + (left.GetHashCode() == right.GetHashCode()).ToString().ToLowerInvariant()
                            : "false");
                    },
                    2));

            realm.DefineValue(
                host,
                "bigEqualsTen",
                realm.NewMethod(
                    "bigEqualsTen",
                    (_, _, arguments) => JsHostValue.Boolean(Argument(arguments, 0) == JsHostValue.BigInt(10)),
                    1));

            realm.DefineValue(
                host,
                "bigProps",
                realm.NewMethod(
                    "bigProps",
                    (r, _, arguments) =>
                    {
                        var target = Argument(arguments, 0);
                        var source = r.GetProperty(target, "src").AsBigInt() ?? BigInteger.Zero;
                        r.SetProperty(target, "doubled", JsHostValue.BigInt(source * 2));
                        r.DefineValue(target, "defined", JsHostValue.BigInt(7));
                        r.DefineIndex(target, 0, JsHostValue.BigInt(8));
                        return JsHostValue.String(source.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    },
                    1));

            realm.DefineValue(
                host,
                "bigInvoke",
                realm.NewMethod(
                    "bigInvoke",
                    (r, _, arguments) =>
                    {
                        System.Span<JsHostValue> passed = [JsHostValue.BigInt(7)];
                        var answered = r.Invoke(Argument(arguments, 0), JsHostValue.Undefined, passed);
                        return JsHostValue.String(answered.AsBigInt()?.ToString() ?? "not a bigint");
                    },
                    1));

            realm.DefineValue(
                host,
                "bigConstruct",
                realm.NewMethod(
                    "bigConstruct",
                    (r, _, arguments) =>
                    {
                        System.Span<JsHostValue> passed = [JsHostValue.BigInt(41)];
                        var made = r.Construct(Argument(arguments, 0), passed);
                        return JsHostValue.String(r.GetProperty(made, "v").AsBigInt()?.ToString() ?? "not a bigint");
                    },
                    1));

            realm.DefineValue(
                host,
                "bigThrow",
                realm.NewMethod(
                    "bigThrow",
                    (r, _, arguments) =>
                    {
                        try
                        {
                            r.Invoke(Argument(arguments, 0), JsHostValue.Undefined);
                            return JsHostValue.String("returned");
                        }
                        catch (JsHostThrowException thrown)
                        {
                            return JsHostValue.String(thrown.Thrown.Kind + ":" + thrown.Thrown.AsBigInt());
                        }
                    },
                    1));

            // A BIGINT THE HOST THROWS rather than returns (VM-FIX-I): the guest catches the value
            // itself, or, in a realm that declined the surface, the TypeError a returned one is.
            realm.DefineValue(
                host,
                "bigRaise",
                realm.NewMethod("bigRaise", (r, _, _) => throw r.Throw(JsHostValue.BigInt(-5))));

            // RE-RAISED AS CAUGHT: the body's exception is the one the realm raised, so the guest
            // must see the value it threw and not a rendering of it.
            realm.DefineValue(
                host,
                "bigRethrow",
                realm.NewMethod(
                    "bigRethrow",
                    (r, _, arguments) =>
                    {
                        try
                        {
                            return r.Invoke(Argument(arguments, 0), JsHostValue.Undefined);
                        }
                        catch (JsHostThrowException)
                        {
                            throw;
                        }
                    },
                    1));

            realm.DefineValue(
                host,
                "bigClone",
                realm.NewMethod("bigClone", (r, _, arguments) => r.AdoptClone(r.DetachClone(Argument(arguments, 0))), 1));
        }

        /// <summary>The argument at <paramref name="at"/>, or <c>undefined</c> past the end.</summary>
        private static JsHostValue Argument(System.ReadOnlySpan<JsHostValue> arguments, int at) =>
            at < arguments.Length ? arguments[at] : JsHostValue.Undefined;
    }
}
