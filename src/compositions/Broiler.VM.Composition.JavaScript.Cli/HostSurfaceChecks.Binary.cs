// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Threading;
using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>
/// The checks for the host surface's byte members: <c>TryReadArrayBuffer</c> and
/// <c>NewArrayBuffer</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>The bytes are judged by the guest wherever the guest can judge them.</b> The host answers a
/// digest of what it read and the guest computes the same digest over the same buffer, so a read
/// that answered the right length and the wrong bytes - an array of zeros, the most plausible
/// wrong answer - fails. A buffer the host made is read back by the guest through its own
/// <c>Uint8Array</c>, never by the host that made it.
/// </para>
/// <para>
/// <b>The budget and cancellation checks are the exception, because what they are about is the
/// program NOT printing.</b> They run a host body that records what it saw and compare that, and
/// the invocation's outcome, instead.
/// </para>
/// </remarks>
internal static partial class HostSurfaceChecks
{
    /// <summary>The digest both sides compute, and the guest's own byte patterns.</summary>
    private const string Prelude =
        """
        function fnv(u8, n) {
            var h = 2166136261;
            for (var i = 0; i < n; i++) { h = Math.imul((h ^ u8[i]) >>> 0, 16777619) >>> 0; }
            return h;
        }
        function make(n) {
            var b = new ArrayBuffer(n);
            var u = new Uint8Array(b);
            for (var i = 0; i < n; i++) { u[i] = (i * 31 + 7) & 255; }
            return b;
        }
        function same(got, want) { return got === want ? 'same' : got + ' vs ' + want; }

        """;

    /// <summary>Runs the byte checks, answering the failure count.</summary>
    private static int RunBinary()
    {
        var failures = 0;

        failures += Check(
            "the host reads an ArrayBuffer's exact bytes: empty, short and multi-chunk",
            Prelude +
            """
            var sizes = [0, 5, 100000];
            for (var k = 0; k < sizes.length; k++) {
                var n = sizes[k], b = make(n);
                var want = 'Copied:' + n + ':' + fnv(new Uint8Array(b), n) + (n > 0 ? ':fresh' : '');
                print(n + '=' + same(broilerHost.readBytes(b), want));
            }
            """,
            ["0=same", "5=same", "100000=same"]);

        failures += Check(
            "the host reads into a span it owns, and a short span is written nothing",
            Prelude +
            """
            var five = make(5), h = fnv(new Uint8Array(five), 5);
            print('exact=' + same(broilerHost.readInto(five, 5), 'Copied:5:' + h + ':clean'));
            print('roomy=' + same(broilerHost.readInto(five, 9), 'Copied:5:' + h + ':clean'));
            print('short=' + broilerHost.readInto(five, 4));
            print('empty=' + broilerHost.readInto(make(0), 0));
            var big = make(70000);
            print('big=' + same(broilerHost.readInto(big, 70000),
                'Copied:70000:' + fnv(new Uint8Array(big), 70000) + ':clean'));
            """,
            [
                "exact=same",
                "roomy=same",
                "short=DestinationTooSmall:5:-:clean",
                "empty=Copied:0:2166136261:clean",
                "big=same",
            ]);

        failures += Check(
            "only the realm's own ArrayBuffer brand is read; a subclass instance is one",
            Prelude +
            """
            class Sub extends ArrayBuffer {}
            var fake = { byteLength: 3 };
            var candidates = [
                new Uint8Array(4), new DataView(new ArrayBuffer(4)), fake,
                Object.create(ArrayBuffer.prototype), new Proxy(new ArrayBuffer(4), {}),
                42, 'text', null, undefined, function () {}, [1, 2, 3]
            ];
            var answers = [];
            for (var k = 0; k < candidates.length; k++) { answers.push(broilerHost.readBytes(candidates[k])); }
            print('wrong=' + answers.join(','));
            print('span=' + broilerHost.readInto(new Uint8Array(4), 8));
            var sub = new Sub(3);
            new Uint8Array(sub).set([9, 8, 7]);
            print('sub=' + same(broilerHost.readBytes(sub), 'Copied:3:' + fnv(new Uint8Array(sub), 3) + ':fresh'));
            """,
            [
                "wrong=" + string.Join(",", System.Linq.Enumerable.Repeat("NotAnArrayBuffer:0:-", 11)),
                "span=NotAnArrayBuffer:0:-:clean",
                "sub=same",
            ]);

        failures += Check(
            "a detached buffer answers Detached, and the buffer it moved to answers its bytes",
            Prelude +
            """
            var b = make(4), h = fnv(new Uint8Array(b), 4);
            var moved = b.transfer();
            print('detached=' + broilerHost.readBytes(b));
            print('span=' + broilerHost.readInto(b, 16));
            print('moved=' + same(broilerHost.readBytes(moved), 'Copied:4:' + h + ':fresh'));
            var zero = make(3).transfer(0);
            print('zero=' + broilerHost.readBytes(zero));
            """,
            [
                "detached=Detached:0:-",
                "span=Detached:0:-:clean",
                "moved=same",
                "zero=Copied:0:2166136261",
            ]);

        // A RESIZABLE BUFFER IS READ AT THE LENGTH IT HAS AT THE CALL (JSD-0024 section 12, as
        // amended with JSeal F04-F06): no status of its own, the current bytes after every resize,
        // a too-small span told the current length, and detachment answered as for any buffer.
        failures += Check(
            "a resizable buffer is read at its current length after it grows, shrinks and moves",
            Prelude +
            """
            var r = new ArrayBuffer(4, { maxByteLength: 64 });
            new Uint8Array(r).set([1, 2, 3, 4]);
            print('now=' + same(broilerHost.readBytes(r), 'Copied:4:' + fnv(new Uint8Array(r), 4) + ':fresh'));
            r.resize(10);
            new Uint8Array(r)[9] = 9;
            print('grown=' + same(broilerHost.readBytes(r), 'Copied:10:' + fnv(new Uint8Array(r), 10) + ':fresh'));
            print('short=' + broilerHost.readInto(r, 6));
            r.resize(2);
            print('shrunk=' + same(broilerHost.readInto(r, 6), 'Copied:2:' + fnv(new Uint8Array(r), 2) + ':clean'));
            var moved = r.transfer();
            print('detached=' + broilerHost.readBytes(r) + ':' + r.resizable);
            print('moved=' + moved.resizable + ':' + same(broilerHost.readBytes(moved), 'Copied:2:' + fnv(new Uint8Array(moved), 2) + ':fresh'));
            """,
            [
                "now=same",
                "grown=same",
                "short=DestinationTooSmall:10:-:clean",
                "shrunk=same",
                "detached=Detached:0:-:true",
                "moved=true:same",
            ]);

        failures += Check(
            "replaced globals, prototypes, species and join change nothing either member does",
            Prelude +
            """
            var AB = ArrayBuffer, U8 = Uint8Array;
            var TA = Object.getPrototypeOf(U8.prototype);
            var b = make(20000), want = fnv(new U8(b), 20000);
            var ran = [];
            function note(name) { return function () { ran.push(name); return 0; }; }
            Object.defineProperty(TA, 'join', { value: note('join') });
            Object.defineProperty(TA, 'subarray', { value: note('subarray') });
            Object.defineProperty(TA, 'set', { value: note('set') });
            Object.defineProperty(TA, 'length', { get: note('length') });
            Object.defineProperty(U8, Symbol.species, { get: note('u8-species') });
            Object.defineProperty(AB, Symbol.species, { get: note('ab-species') });
            Object.defineProperty(AB.prototype, 'byteLength', { get: note('byteLength') });
            Object.defineProperty(AB.prototype, 'constructor', { get: note('constructor') });
            Object.defineProperty(AB.prototype, 'slice', { value: note('slice') });
            Object.defineProperty(Object.prototype, 'buffer', { get: note('buffer'), configurable: true });
            globalThis.ArrayBuffer = note('ArrayBuffer');
            globalThis.Uint8Array = note('Uint8Array');
            globalThis.DataView = note('DataView');
            var read = broilerHost.readBytes(b);
            var into = broilerHost.readInto(b, 20000);
            var made = broilerHost.newBytes('short');
            var seen = ran.join(',');
            print('read=' + same(read, 'Copied:20000:' + want + ':fresh'));
            print('into=' + same(into, 'Copied:20000:' + want + ':clean'));
            print('proto=' + (Object.getPrototypeOf(made) === AB.prototype));
            var u = new U8(made);
            print('made=' + [u[0], u[1], u[2], u[3], u[4]].join(':'));
            print('ran=' + (seen === '' ? 'nothing' : seen));
            """,
            ["read=same", "into=same", "proto=true", "made=1:2:3:254:255", "ran=nothing"]);

        failures += Check(
            "the host makes an ArrayBuffer from its bytes: empty, short and multi-chunk, and a copy",
            Prelude +
            """
            var e = broilerHost.newBytes('empty');
            print('empty=' + (e instanceof ArrayBuffer) + ':' + e.byteLength);
            var s = broilerHost.newBytes('short');
            print('short=' + Array.prototype.join.call(new Uint8Array(s), ','));
            var m = broilerHost.newBytes('multi');
            print('multi=' + m.byteLength + ':' + same(fnv(new Uint8Array(m), m.byteLength), fnv(new Uint8Array(make(100000)), 100000)));
            print('distinct=' + (broilerHost.newBytes('short') !== broilerHost.newBytes('short')));
            print('usable=' + new Uint8Array(s.slice(1, 3)).join(',') + ':' + s.transfer().byteLength + ':' + s.byteLength);
            print('roundtrip=' + broilerHost.readBytes(broilerHost.newBytes('short')).split(':')[1]);
            """,
            ["empty=true:0", "short=1,2,3,254,255", "multi=100000:same", "distinct=true", "usable=2,3:5:0", "roundtrip=5"]);

        // A RESIZE IS HELD TO THE LIVE-BYTES CEILING PAST THE BUFFER'S HIGH-WATER MARK ONLY (JSeal
        // F04): shrinking and regrowing up to the mark again is admitted however often it happens,
        // and a growth past it that the ceiling refuses ends the run before the buffer changes -
        // nothing after it prints. (The unchanged buffer is not observable from outside: the
        // refusal ends the operation, and with it every crossing a host could read it through.)
        failures += Check(
            "a resize is charged past its high-water mark, and one the live-bytes ceiling refuses ends the run",
            """
            var r = new ArrayBuffer(1048576, { maxByteLength: 8388608 });
            for (var i = 0; i < 8; i++) { r.resize(3145728); r.resize(0); }
            r.resize(3145728);
            print('regrown=' + r.byteLength);
            r.resize(1048576);
            new Uint8Array(r)[0] = 7;
            print('before=' + r.byteLength + ':' + new Uint8Array(r)[0]);
            r.resize(7340032);
            print('continued=' + r.byteLength);
            """,
            ["regrown=3145728", "before=1048576:7"],
            outcome: VmOutcome.ResourceExhaustion,
            ceilings: new Dictionary<VmBudgetDimension, ulong> { [VmBudgetDimension.LiveBytes] = 4L * 1024 * 1024 });

        failures += CheckAbort(
            "a read the fuel allowance refuses ends the operation and copies nothing",
            "var b = new ArrayBuffer(4000000); new Uint8Array(b)[0] = 1; broilerHost.probe(b); print('continued');",
            new Dictionary<VmBudgetDimension, ulong> { [VmBudgetDimension.Fuel] = 6_000_000 },
            cancel: false,
            ProbeKind.Read,
            VmOutcome.ResourceExhaustion);

        failures += CheckAbort(
            "a cancelled read ends the operation and copies nothing",
            "var b = new ArrayBuffer(1000000); broilerHost.probe(b); print('continued');",
            null,
            cancel: true,
            ProbeKind.Read,
            VmOutcome.Cancellation);

        failures += CheckAbort(
            "a buffer the live-bytes ceiling refuses is never made, after one it admits",
            "broilerHost.probe(); print('continued');",
            new Dictionary<VmBudgetDimension, ulong> { [VmBudgetDimension.LiveBytes] = 4L * 1024 * 1024 },
            cancel: false,
            ProbeKind.NewAfterOneAdmitted,
            VmOutcome.ResourceExhaustion);

        failures += CheckAbort(
            "a buffer the fuel allowance refuses is never made",
            "broilerHost.probe(); print('continued');",
            new Dictionary<VmBudgetDimension, ulong> { [VmBudgetDimension.Fuel] = 2_000_000 },
            cancel: false,
            ProbeKind.New,
            VmOutcome.ResourceExhaustion);

        failures += CheckAbort(
            "a cancelled construction is never made",
            "broilerHost.probe(); print('continued');",
            null,
            cancel: true,
            ProbeKind.New,
            VmOutcome.Cancellation);

        return failures;
    }

    /// <summary>The byte members of the demonstration embedder.</summary>
    private sealed partial class DemonstrationSurface
    {
        /// <summary>
        /// Installs <c>readBytes</c>, <c>readInto</c> and <c>newBytes</c>, each answering a string
        /// the guest compares against what it computes itself.
        /// </summary>
        private static void InstallBinary(JsHostRealm realm, JsHostValue host)
        {
            // THE ARRAY READ. It answers status, length and digest, and - for a non-empty buffer -
            // whether a second read is unchanged after the host wrote to the first array: "fresh"
            // is the only answer a copy can give, and "shared" is what handing out the realm's own
            // storage would give.
            realm.DefineValue(
                host,
                "readBytes",
                realm.NewMethod(
                    "readBytes",
                    (r, _, arguments) =>
                    {
                        var value = arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined;
                        var status = r.TryReadArrayBuffer(value, out var bytes);

                        if (status != JsHostBufferStatus.Copied)
                        {
                            return JsHostValue.String(status + ":" + bytes.Length + ":-");
                        }

                        var digest = Digest(bytes, bytes.Length);
                        var answer = status + ":" + bytes.Length + ":" + digest;

                        if (bytes.Length > 0)
                        {
                            bytes[0] ^= 0xFF;
                            r.TryReadArrayBuffer(value, out var again);
                            answer += Digest(again, again.Length) == digest ? ":fresh" : ":shared";
                        }

                        return JsHostValue.String(answer);
                    },
                    1));

            // THE SPAN READ, into memory the host owns and pre-fills, so that a byte written where
            // the contract says none may be is visible as "dirty".
            realm.DefineValue(
                host,
                "readInto",
                realm.NewMethod(
                    "readInto",
                    (r, _, arguments) =>
                    {
                        var value = arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined;
                        var capacity = arguments.Length > 1 ? (int)r.ToNumber(arguments[1]) : 0;
                        var destination = new byte[capacity];
                        System.Array.Fill(destination, (byte)0xEE);

                        var status = r.TryReadArrayBuffer(value, destination, out var length);
                        var written = status == JsHostBufferStatus.Copied ? length : 0;
                        var clean = true;

                        for (var at = written; at < destination.Length; at++)
                        {
                            clean &= destination[at] == 0xEE;
                        }

                        var digest = status == JsHostBufferStatus.Copied
                            ? Digest(destination, length).ToString(System.Globalization.CultureInfo.InvariantCulture)
                            : "-";

                        return JsHostValue.String(
                            status + ":" + length + ":" + digest + ":" + (clean ? "clean" : "dirty"));
                    },
                    2));

            // THE CONSTRUCTION. The host overwrites its own source after the call, so a buffer that
            // aliased it would read back as the overwrite rather than the bytes it was made from.
            realm.DefineValue(
                host,
                "newBytes",
                realm.NewMethod(
                    "newBytes",
                    (r, _, arguments) =>
                    {
                        var kind = arguments.Length > 0 ? r.ToJsString(arguments[0]) : "empty";
                        byte[] source = kind switch
                        {
                            "short" => [1, 2, 3, 254, 255],
                            "multi" => Pattern(100_000),
                            _ => [],
                        };

                        var buffer = r.NewArrayBuffer(source);
                        System.Array.Fill(source, (byte)0xAA);
                        return buffer;
                    },
                    1));
        }

        /// <summary>The guest's own pattern, <c>(i * 31 + 7) &amp; 255</c>.</summary>
        private static byte[] Pattern(int length)
        {
            var bytes = new byte[length];

            for (var at = 0; at < length; at++)
            {
                bytes[at] = (byte)((at * 31 + 7) & 255);
            }

            return bytes;
        }

        /// <summary>FNV-1a over the first <paramref name="length"/> bytes, as the guest computes it.</summary>
        private static uint Digest(byte[] bytes, int length)
        {
            var hash = 2166136261u;

            for (var at = 0; at < length; at++)
            {
                hash = unchecked((hash ^ bytes[at]) * 16777619u);
            }

            return hash;
        }
    }

    /// <summary>What a probe's host body attempts.</summary>
    private enum ProbeKind
    {
        /// <summary>Reads the buffer it is given, into an array and then into a span.</summary>
        Read,

        /// <summary>Makes a four-megabyte buffer.</summary>
        New,

        /// <summary>Makes a three-megabyte buffer, which is admitted, then a second one.</summary>
        NewAfterOneAdmitted,
    }

    /// <summary>
    /// Runs a program whose host body must be ended underneath it, and checks that it was, that
    /// nothing was answered, and what the operation reported.
    /// </summary>
    private static int CheckAbort(
        string title,
        string source,
        IReadOnlyDictionary<VmBudgetDimension, ulong>? ceilings,
        bool cancel,
        ProbeKind kind,
        VmOutcome expected)
    {
        var printed = new List<string>();
        using var cancellation = new CancellationTokenSource();
        var surface = new ProbeSurface(kind, cancel ? cancellation : null);

        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("check", source, SliceParseOptions.Script, false, Caller)],
            [],
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return Report(title, "the source was refused: " + Render(compiled));
        }

        var catalog = VmCatalog.CreateBuilder()
            .Add(JavaScriptProfile.DescriptorHostingRealms(surface))
            .Build();

        var created = VmRuntime.Create(catalog, Options(printed, permitted: true, ceilings));

        if (!created.TryGetRuntime(out var runtime))
        {
            return Report(title, $"the runtime refused creation: {created.Outcome}/{created.Reason}");
        }

        VmOutcome outcome;
        string reason;

        using (runtime)
        {
            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
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
                    var result = instance.Invoke(in request, cancellation.Token);
                    outcome = result.Outcome;
                    reason = result.Reason.ToString();
                }
            }
        }

        if (surface.Complaint is { } complaint)
        {
            return Report(title, complaint);
        }

        if (!surface.Terminated)
        {
            return Report(title, "the host body was never ended underneath it");
        }

        if (printed.Count != 0)
        {
            return Report(title, "the program printed after the host body: " + string.Join(" | ", printed));
        }

        if (outcome != expected)
        {
            return Report(title, $"the run answered {outcome}/{reason} and {expected} was expected");
        }

        System.Console.WriteLine("ok   " + title + " (" + outcome + "/" + reason + ")");
        return 0;
    }

    /// <summary>An embedder whose one member attempts what its probe says and records the result.</summary>
    private sealed class ProbeSurface : IJsHostSurface
    {
        private const byte Sentinel = 0xEE;

        private readonly ProbeKind kind;

        private readonly CancellationTokenSource? cancel;

        internal ProbeSurface(ProbeKind kind, CancellationTokenSource? cancel)
        {
            this.kind = kind;
            this.cancel = cancel;
        }

        /// <summary>Whether the body saw <see cref="JsHostTerminatedException"/>.</summary>
        internal bool Terminated { get; private set; }

        /// <summary>What was wrong, when something was.</summary>
        internal string? Complaint { get; private set; }

        public void OnTurn(JsHostRealm realm)
        {
        }

        public void OnRealmCreated(JsHostRealm realm)
        {
            var host = realm.NewObject();
            realm.DefineValue(host, "probe", realm.NewMethod("probe", Probe, 1));
            realm.DefineValue(realm.Global, "broilerHost", host);
        }

        private JsHostValue Probe(
            JsHostRealm realm, JsHostValue thisValue, System.ReadOnlySpan<JsHostValue> arguments)
        {
            var buffer = arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined;
            JsHostTerminatedException? termination = null;
            cancel?.Cancel();

            switch (kind)
            {
                case ProbeKind.Read:
                {
                    // THE SPAN FIRST, because the array member's refusal latches the realm and
                    // every later crossing is then refused before it acts.
                    var destination = new byte[4_000_000];
                    System.Array.Fill(destination, Sentinel);
                    var length = -1;

                    try
                    {
                        realm.TryReadArrayBuffer(buffer, destination, out length);
                        Complaint = "the span read completed";
                    }
                    catch (JsHostTerminatedException ended)
                    {
                        Terminated = true;
                        termination = ended;
                    }

                    if (System.Array.Exists(destination, static b => b != Sentinel) || length != -1)
                    {
                        Complaint = "the span read wrote to its destination or its length before it ended";
                    }

                    byte[]? bytes = null;

                    try
                    {
                        realm.TryReadArrayBuffer(buffer, out bytes);
                        Complaint = "the array read completed after the realm was latched";
                    }
                    catch (JsHostTerminatedException)
                    {
                        if (bytes is not null)
                        {
                            Complaint = "the array read answered bytes although it was ended";
                        }
                    }

                    break;
                }

                case ProbeKind.New:
                case ProbeKind.NewAfterOneAdmitted:
                {
                    var answered = JsHostValue.Missing;

                    if (kind is ProbeKind.NewAfterOneAdmitted)
                    {
                        // Admitted: three megabytes under a four-megabyte ceiling.
                        answered = realm.NewArrayBuffer(new byte[3 * 1024 * 1024]);

                        if (!answered.IsObject)
                        {
                            Complaint = "the admitted buffer was not answered";
                            return JsHostValue.Undefined;
                        }

                        answered = JsHostValue.Missing;
                    }

                    try
                    {
                        answered = realm.NewArrayBuffer(new byte[kind is ProbeKind.New ? 4_000_000 : 3 * 1024 * 1024]);
                        Complaint = "the refused buffer was made";
                    }
                    catch (JsHostTerminatedException ended)
                    {
                        Terminated = true;
                        termination = ended;
                    }

                    if (!answered.IsMissing)
                    {
                        Complaint = "a value was answered although construction was ended";
                    }

                    break;
                }
            }

            // Every later crossing is refused with the same termination, so nothing more can be
            // built in this operation after the refusal.
            try
            {
                realm.NewObject();
                Complaint ??= "a crossing after the termination was answered";
            }
            catch (JsHostTerminatedException)
            {
            }

            // RE-RAISED, AS A WELL-BEHAVED EMBEDDER DOES after releasing what it holds. (A body that
            // swallowed it and returned would still see the operation end with the latched abort,
            // but only when the step ends - the guest would run on until then.)
            if (termination is not null)
            {
                throw termination;
            }

            return JsHostValue.Undefined;
        }
    }
}
