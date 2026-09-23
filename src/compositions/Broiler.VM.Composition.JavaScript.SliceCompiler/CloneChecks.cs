// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The internal structured-clone graph and its ArrayBuffer transfer (JSeal cards I14, I15 and
/// I16), judged by what guest programs observe of a clone.
/// </summary>
/// <remarks>
/// <para>
/// <b>The clone is internal and these checks reach it through two unsafe accessors, and both halves
/// are deliberate.</b> Decision JSD-0032 keeps the carrier out of the public surface until its
/// supported type set is complete for the contract a public API would promise, and rule A10 forbids
/// the profile to open its internals to anything. Rule A11 then leaves a composition root as the
/// only lawful observer of a profile, and this root already carries the lowering these programs
/// need. Reflection was the first way in and rule B5 refused it - <c>MethodBase.Invoke</c> is one of
/// the dynamic entry points no shipping assembly may reach - so the two members are bound at compile
/// time with <see cref="UnsafeAccessorAttribute"/> instead: named in this file, checked by the
/// runtime when first called, preserved by the trimmer, and invoking nothing dynamically. <b>That is
/// still a door into the profile's internals from outside it</b>, and it is recorded as one in
/// JSD-0032 section 2 rather than presented as anything else; this file is its only user.
/// </para>
/// <para>
/// <b>Every answer is the guest's.</b> Each program is given a <c>cloneHost</c> object by an
/// embedder of the in-realm host surface, clones through it, and answers a string of observations
/// made with ordinary language operations - identity, prototype, brand, own keys, bytes. A clone
/// that looked right to the host and wrong to a program would fail here, which is the direction a
/// clone has to be judged from.
/// </para>
/// <para>
/// <b>The cross-realm checks dispose the source before the destination adopts.</b> A carrier that
/// still named an object of the source realm would either fail there or answer from a realm that no
/// longer exists; one that holds data answers the same either way.
/// </para>
/// </remarks>
internal static class CloneChecks
{
    /// <summary>The identity these checks present to the verifier.</summary>
    private const string Caller = "js-slice-compiler://clone";

    /// <summary>The profile type a carrier has, named for the two accessors below.</summary>
    private const string CarrierType =
        "Broiler.VM.Profile.JavaScript.JsCloneCarrier, Broiler.VM.Profile.JavaScript";

    /// <summary>The bounds the carrier itself declares, which a check passes when it is not testing them.</summary>
    private const long Unbounded = long.MaxValue;

    /// <summary>A guest helper every program shares: the refusal a value meets, or "cloned".</summary>
    private const string Prelude =
        """
        var rt = cloneHost.roundTrip;
        function refusal(v) {
            try { rt(v); return 'cloned'; }
            catch (e) { return (e instanceof TypeError && String(e.message).indexOf('DataCloneError:') === 0) ? 'refused' : 'other:' + e; }
        }
        function tag(v) { return Object.prototype.toString.call(v); }
        function tr(v, list) { return cloneHost.transfer(v, list); }
        function trefusal(v, list) {
            try { tr(v, list); return 'cloned'; }
            catch (e) { return (e instanceof TypeError && String(e.message).indexOf('DataCloneError:') === 0) ? 'refused' : 'other:' + e; }
        }
        function filled(n) { var b = new ArrayBuffer(n), u = new Uint8Array(b); for (var i = 0; i < n; i++) u[i] = i + 1; return b; }
        function intact(b, n) { return b.byteLength === n && new Uint8Array(b)[n - 1] === n; }

        """;

    /// <summary>The same-realm programs: a title, the program, and the one answer it must give.</summary>
    private static readonly (string Name, string Source, string Expected)[] Programs =
    [
        (
            "clone/i14/primitives-survive-including-negative-zero-and-nan",
            """
            [rt(undefined) === undefined, rt(null) === null, rt(true), rt(false), rt(42),
             Object.is(rt(-0), -0), Number.isNaN(rt(NaN)), rt(Infinity) === Infinity,
             rt('') === '', rt('caf\u00e9 \ud83d\ude00') === 'caf\u00e9 \ud83d\ude00'].join(',');
            """,
            "true,true,true,false,42,true,true,true,true,true"),
        (
            "clone/i14/an-ordinary-object-copies-own-enumerable-string-keys-in-order",
            """
            var s = Symbol('k');
            class K { constructor() { this.b = 2; this.a = 1; } method() { return 1; } }
            var src = new K();
            src[2] = 'two'; src[1] = 'one'; src.z = { deep: [1, 2] };
            src[s] = 'symbol-keyed';
            Object.defineProperty(src, 'hidden', { value: 1, enumerable: false });
            var c = rt(src);
            [Object.keys(c).join('|'), Object.getPrototypeOf(c) === Object.prototype, c !== src,
             c.z !== src.z, c.z.deep.join('+'), Object.getOwnPropertySymbols(c).length,
             'hidden' in c, typeof c.method].join(',');
            """,
            "1|2|b|a|z,true,true,true,1+2,0,false,undefined"),
        (
            "clone/i14/getters-run-depth-first-and-a-deleted-key-is-skipped",
            """
            var order = [];
            var src = {
                get a() { order.push('a'); return { get z() { order.push('z'); return 1; } }; },
                get b() { order.push('b'); delete this.c; return 2; },
                c: 3,
                d: 4
            };
            var c = rt(src);
            [order.join(''), Object.keys(c).join(''), c.a.z, c.b,
             Object.getOwnPropertyDescriptor(c, 'b').get === undefined].join(',');
            """,
            "azb,abd,1,2,true"),
        (
            "clone/i14/a-getter-throw-propagates-as-the-same-value",
            """
            var boom = new RangeError('boom');
            var answer;
            try { rt({ first: 1, get second() { throw boom; } }); answer = 'no-throw'; }
            catch (e) { answer = String(e === boom); }
            answer;
            """,
            "true"),
        (
            "clone/i14/cycles-terminate-and-shared-references-stay-shared",
            """
            var shared = { v: 1 };
            var src = { x: shared, y: shared, list: [shared, shared] };
            src.self = src;
            var a = { name: 'a' }, b = { name: 'b', a: a }; a.b = b;
            src.pair = a;
            var c = rt(src);
            [c.self === c, c.x === c.y, c.list[0] === c.x, c.list[1] === c.y, c.x !== shared,
             c.pair.b.a === c.pair, c.pair.b !== b].join(',');
            """,
            "true,true,true,true,true,true,true"),
        (
            "clone/i14/holes-length-and-non-index-properties-of-an-array-survive",
            """
            var dense = [1, , 3];
            dense.length = 10;
            dense.label = 'L';
            var sparse = [];
            sparse[1000000000] = 'far';
            sparse[5] = 'near';
            var c = rt(dense), d = rt(sparse);
            [Array.isArray(c), c.length, 1 in c, c[2], c.label, Object.keys(c).join('|'),
             Object.getPrototypeOf(c) === Array.prototype,
             d.length, Object.keys(d).join('|'), d[1000000000]].join(',');
            """,
            "true,10,false,3,L,0|2|label,true,1000000001,5|1000000000,far"),
        (
            "clone/i14/mutation-does-not-cross-the-boundary-in-either-direction",
            """
            var src = { inner: { n: 1 }, list: [1, 2] };
            var c = rt(src);
            src.inner.n = 2; src.list.push(3); src.added = true;
            c.inner.n = 9; c.list[0] = 'x';
            [src.inner.n, src.list.join('+'), c.inner.n, c.list.join('+'), 'added' in c].join(',');
            """,
            "2,1+2+3,9,x+2,false"),
        (
            "clone/i14/functions-symbols-proxies-and-unlisted-brands-are-refused",
            """
            function* gen() { yield 1; }
            var args = (function () { return arguments; })(1, 2);
            [refusal(function () {}), refusal(() => 1), refusal(class {}), refusal(Math.max),
             refusal(function () {}.bind(null)), refusal(Symbol('s')), refusal({ nested: Symbol.iterator }),
             refusal(Object(Symbol('w'))), refusal(new Proxy({}, {})), refusal(new Proxy(function () {}, {})),
             refusal(new WeakMap()), refusal(new WeakSet()), refusal(Promise.resolve(1)), refusal(gen()),
             refusal(args), refusal([1][Symbol.iterator]()), refusal(new Map([[1, 2]]).keys()),
             refusal({ ok: 1, bad: [function () {}] })].join(',');
            """,
            "refused,refused,refused,refused,refused,refused,refused,refused,refused,refused,refused,refused,refused,refused,refused,refused,refused,refused"),
        (
            "clone/i14/a-deep-chain-clones-without-recursing-on-the-host-stack",
            """
            var head = { depth: 0 }, at = head;
            for (var i = 1; i < 200000; i++) { at.next = { depth: i }; at = at.next; }
            var c = rt(head), walked = 0, last;
            for (var n = c; n; n = n.next) { walked++; last = n.depth; }
            [walked, last].join(',');
            """,
            "200000,199999"),
        (
            "clone/i15/boolean-number-and-string-wrappers-keep-their-brand-and-value",
            """
            var b = new Boolean(false), n = new Number(-0), s = new String('ab');
            s.extra = 1;
            var cb = rt(b), cn = rt(n), cs = rt(s);
            [typeof cb, tag(cb), cb.valueOf(), cb !== b, tag(cn), Object.is(cn.valueOf(), -0),
             tag(cs), cs.valueOf(), cs.length, 'extra' in cs, Object.getPrototypeOf(cs) === String.prototype].join(',');
            """,
            "object,[object Boolean],false,true,[object Number],true,[object String],ab,2,false,true"),
        (
            "clone/i15/a-date-keeps-its-time-value-and-nothing-else",
            """
            var d = new Date(86400000); d.note = 'x';
            var c = rt(d), invalid = rt(new Date(NaN));
            d.setTime(0);
            [tag(c), c instanceof Date, c.getTime(), 'note' in c, Number.isNaN(invalid.getTime()), c !== d].join(',');
            """,
            "[object Date],true,86400000,false,true,true"),
        (
            "clone/i15/a-regexp-keeps-source-and-flags-and-resets-lastindex",
            """
            var r = /a+(b)/gy; r.lastIndex = 3; r.extra = 1;
            var c = rt(r);
            var before = c.lastIndex;
            var m = c.exec('aab');
            [tag(c), c.source, c.flags, before === 3 ? 'kept' : 'reset', 'extra' in c,
             m ? m[1] : 'none', c.lastIndex, rt(new RegExp('')).source, rt(/x/dimsu).flags, c !== r].join(',');
            """,
            "[object RegExp],a+(b),gy,reset,false,b,3,(?:),dimsu,true"),
        (
            "clone/i15/maps-and-sets-keep-order-cycles-and-repeated-keys-and-values",
            """
            var k = { key: 1 }, v = { value: 1 };
            var m = new Map();
            m.set('first', 1); m.set(k, v); m.set(v, k); m.set(-0, 'zero'); m.set(NaN, 'nan');
            m.set(m, m); m.delete('first'); m.set('last', v);
            var s = new Set([k, v, 'text']); s.add(s); m.set('set', s);
            var c = rt(m);
            var keys = Array.from(c.keys());
            var ck = keys[0], cv = c.get(ck), cs = c.get('set');
            [tag(c), c.size, keys.length, typeof keys[0], c.get(cv) === ck, c.get(c) === c,
             c.get(0), Object.is(keys[2], 0), c.get(NaN), c.get('last') === cv, c.has('first'),
             tag(cs), cs.size, cs.has(cs), cs.has(ck), cs.has(cv), ck !== k, Object.getPrototypeOf(c) === Map.prototype].join(',');
            """,
            "[object Map],7,7,object,true,true,zero,true,nan,true,false,[object Set],4,true,true,true,true,true"),
        (
            "clone/i15/errors-keep-their-listed-name-message-and-cause",
            """
            class MyErr extends RangeError {}
            var e = new TypeError('bad', { cause: { why: 'because' } }); e.extra = 1;
            var custom = new Error('c'); custom.name = 'Custom';
            var loop = new Error('loop'); loop.cause = loop;
            var accessor = new Error('x'); Object.defineProperty(accessor, 'message', { get: function () { return 'g'; } });
            var ce = rt(e), cc = rt(custom), cl = rt(loop), ca = rt(accessor), cm = rt(new MyErr('m'));
            var ag = rt(new AggregateError([1], 'agg')), bare = rt(new Error()), num = new Error('n'); num.message = 42;
            var d = Object.getOwnPropertyDescriptor(ce, 'message');
            [tag(ce), ce instanceof TypeError, ce.name, ce.message, ce.cause.why, 'extra' in ce,
             d.enumerable + '/' + d.writable + '/' + d.configurable, cc.name, cl.cause === cl,
             Object.getOwnPropertyNames(ca).indexOf('message'), cm instanceof RangeError, cm instanceof MyErr,
             ag.name, 'errors' in ag, Object.getOwnPropertyNames(bare).indexOf('message'), typeof rt(num).message,
             tag(rt(Error.prototype)), refusal(new Error('f', { cause: function () {} }))].join(',');
            """,
            "[object Error],true,TypeError,bad,because,false,false/true/true,Error,true,-1,true,false,Error,false,-1,string,[object Object],refused"),
        (
            "clone/i15/buffers-copy-their-bytes-and-views-over-one-buffer-share-one-clone",
            """
            var buf = new ArrayBuffer(8);
            var bytes = new Uint8Array(buf); for (var i = 0; i < 8; i++) bytes[i] = i + 1;
            var u8 = new Uint8Array(buf, 2, 3), dv = new DataView(buf, 1, 4), f64 = new Float64Array([-0, NaN]);
            var i16 = new Int16Array(buf, 4, 2);
            u8.extra = 1;
            var c = rt({ buf: buf, u8: u8, dv: dv, i16: i16, f64: f64 });
            bytes[2] = 99;
            c.u8[0] = 42;
            [tag(c.buf), c.buf !== buf, c.u8.buffer === c.buf, c.dv.buffer === c.buf, c.i16.buffer === c.buf,
             c.u8.byteOffset + '/' + c.u8.length, c.dv.byteOffset + '/' + c.dv.byteLength,
             c.i16.byteOffset + '/' + c.i16.length, Array.from(new Uint8Array(c.buf)).join('+'),
             tag(c.u8), tag(c.dv), tag(c.i16), 'extra' in c.u8, Object.is(c.f64[0], -0), Number.isNaN(c.f64[1]),
             bytes[3], c.dv.getUint8(1)].join(',');
            """,
            "[object ArrayBuffer],true,true,true,true,2/3,1/4,4/2,1+2+42+4+5+6+7+8,[object Uint8Array],[object DataView],[object Int16Array],false,true,true,4,42"),
        (
            // JSD-0032's typed-array row, for the two BigInt kinds (JSeal B07): named by constructor
            // in the carrier, rebuilt over the one cloned buffer, and aliasing it with any other
            // view that shared it - no element is carried as a Number.
            "clone/b07/bigint-views-clone-by-kind-and-share-one-buffer",
            """
            var buf = new ArrayBuffer(24);
            var s64 = new BigInt64Array(buf, 0, 2), u64 = new BigUint64Array(buf, 8, 2), u8 = new Uint8Array(buf);
            s64[0] = -(2n ** 63n); s64[1] = -1n; u64[1] = 2n ** 64n - 2n;
            var c = rt({ s: s64, u: u64, b: u8 });
            c.b[0] = 1;
            [tag(c.s), tag(c.u), c.s.buffer === c.u.buffer, c.u.buffer === c.b.buffer, c.u.byteOffset + '/' + c.u.length,
             String(c.s[0]), String(c.s[1]), String(c.u[0]), String(c.u[1]), typeof c.u[1], String(s64[0])].join(',');
            """,
            "[object BigInt64Array],[object BigUint64Array],true,true,8/2,-9223372036854775807,-1,18446744073709551615,18446744073709551614,bigint,-9223372036854775808"),
        (
            "clone/i15/a-detached-buffer-and-a-view-over-one-are-refused",
            """
            var buf = new ArrayBuffer(4), view = new Uint8Array(buf);
            buf.transfer();
            // A getter that detaches a buffer the walk has already recorded: the view met
            // afterwards is out of bounds, and HTML refuses it rather than cloning stale bytes.
            var late = new ArrayBuffer(4), lateU8 = new Uint8Array(late), lateDv = new DataView(late);
            var viaU8 = new ArrayBuffer(4), u8 = new Uint8Array(viaU8);
            var viaDv = new ArrayBuffer(4), dv = new DataView(viaDv);
            [refusal(buf), refusal(view), refusal({ inner: [view] }),
             refusal({ a: viaU8, get b() { viaU8.transfer(); return 1; }, c: u8 }),
             refusal({ a: viaDv, get b() { viaDv.transfer(); return 1; }, c: dv }),
             refusal({ a: late, get b() { late.transfer(); return 1; }, c: lateU8, d: lateDv })].join(',');
            """,
            "refused,refused,refused,refused,refused,refused"),
        (
            // JSD-0032's resizable row, taken as written (JSeal F06): the carrier has no place for
            // a maximum or for a length-tracking view, so a resizable buffer and every view over
            // one are refused rather than cloned as fixed-length; a fixed buffer still clones.
            "clone/f06/a-resizable-buffer-and-a-view-over-one-are-refused",
            """
            var r = new ArrayBuffer(4, { maxByteLength: 8 });
            var shrunk = new ArrayBuffer(8, { maxByteLength: 8 }), outOfBounds = new Uint8Array(shrunk, 4, 4);
            shrunk.resize(2);
            [refusal(r), refusal(new Uint8Array(r)), refusal(new Int16Array(r, 0, 1)), refusal(new DataView(r)),
             refusal({ inner: [new DataView(r, 1, 2)] }), refusal(outOfBounds),
             refusal(r.transferToFixedLength()), refusal(new ArrayBuffer(4))].join(',');
            """,
            "refused,refused,refused,refused,refused,refused,cloned,cloned"),
        (
            "clone/i15/every-brand-keeps-graph-identity-and-refuses-an-unsupported-member",
            """
            var buf = new ArrayBuffer(2);
            var values = [new Boolean(true), new Number(1), new String('s'), new Date(0), /r/, new Map(), new Set(),
                          new Error('e'), buf, new Uint8Array(buf), new DataView(buf)];
            var twice = values.map(function (v) { var c = rt({ a: v, b: v }); return c.a === c.b && c.a !== v; }).join('');
            [twice, refusal(new Map([[1, function () {}]])), refusal(new Map([[Symbol('k'), 1]])),
             refusal(new Set([new WeakMap()])), refusal(new Error('e', { cause: Symbol('c') })),
             refusal(new Map([['p', new Proxy({}, {})]]))].join(',');
            """,
            "truetruetruetruetruetruetruetruetruetruetrue,refused,refused,refused,refused,refused"),
        (
            "clone/i15/no-brand-is-flattened-into-an-ordinary-object",
            """
            var values = [new Boolean(true), new Number(1), new String('s'), new Date(0), /r/, new Map(), new Set(),
                          new Error('e'), new ArrayBuffer(1), new Uint8Array(1), new DataView(new ArrayBuffer(1)), [], {}];
            values.map(function (v) { return tag(rt(v)) === tag(v); }).join(',');
            """,
            "true,true,true,true,true,true,true,true,true,true,true,true,true"),
        (
            "clone/i16/a-transferred-buffer-moves-its-bytes-and-every-source-view-observes-detachment",
            """
            var buf = filled(8), u8 = new Uint8Array(buf), dv = new DataView(buf, 2, 4), i16 = new Int16Array(buf, 4, 2);
            var loose = filled(4);
            var c = tr({ buf: buf, u8: u8, dv: dv, i16: i16, again: buf, list: [buf, u8] }, [buf, loose]);
            var dvThrows;
            try { dv.getUint8(0); dvThrows = 'no-throw'; } catch (e) { dvThrows = e instanceof TypeError; }
            c.u8[2] = 99;
            [buf.byteLength, u8.length, u8.byteLength, String(u8[0]), i16.length, dvThrows, loose.byteLength,
             tag(c.buf), c.buf.byteLength, Array.from(new Uint8Array(c.buf)).join('+'),
             c.again === c.buf, c.list[0] === c.buf, c.list[1] === c.u8, c.u8.buffer === c.buf, c.dv.buffer === c.buf,
             c.i16.buffer === c.buf, c.dv.getUint8(0), c.i16.byteOffset + '/' + c.i16.length,
             trefusal(buf, [])].join(',');
            """,
            "0,0,0,undefined,0,true,0,[object ArrayBuffer],8,1+2+99+4+5+6+7+8,true,true,true,true,true,true,99,4/2,refused"),
        (
            "clone/i16/a-refused-transfer-list-or-graph-detaches-nothing",
            """
            var a = filled(8), b = filled(4), view = new Uint8Array(a), gone = filled(2), ran = false, boom = new RangeError('boom');
            gone.transfer();
            var outcomes = [
                trefusal(a, [a, a]),
                trefusal(a, [a, view]),
                trefusal(a, [a, {}]),
                trefusal(a, [a, 1]),
                trefusal({ get g() { ran = true; return 1; } }, [a, view]),
                ran,
                trefusal(a, [a, gone]),
                trefusal({ a: a, f: function () {} }, [a, b]),
                trefusal({ a: a, g: new WeakMap() }, [b, a]),
                trefusal({ a: a, get g() { b.transfer(); return 1; } }, [a, b]),
                (function () { try { tr({ a: a, get g() { throw boom; } }, [a]); return 'no-throw'; } catch (e) { return e === boom; } })()
            ];
            [outcomes.join(','), intact(a, 8), view.length, view[7], b.byteLength,
             intact(tr(a, [a]), 8), a.byteLength].join(',');
            """,
            "refused,refused,refused,refused,refused,false,refused,refused,refused,refused,true,true,8,8,0,true,0"),
        (
            "clone/i16/resizable-buffers-are-refused-until-f04-f06-integrate",
            """
            // RESIZABLE BUFFERS DO NOT EXIST IN THIS REALM YET: the options bag is ignored and the
            // buffer is fixed. The row is a tripwire. When they arrive, a resizable buffer must be
            // refused and left usable, not transferred as a fixed one, until the transfer and the
            // record learn its maximum length (JSD-0032 section 5).
            var r = new ArrayBuffer(4, { maxByteLength: 8 });
            var answer = [trefusal(r, [r]), refusal(r), r.byteLength].join(',');
            r.resizable !== true ? 'refused-or-absent'
                : answer === 'refused,refused,4' ? 'refused-or-absent' : 'resizable answered ' + answer;
            """,
            "refused-or-absent"),
    ];

    /// <summary>The BigInt rows (card B06): the carrier's BigInt slot and record.</summary>
    private static readonly (string Name, string Source, string Expected)[] BigIntPrograms =
    [
        (
            "clone/b06/a-bigint-and-a-bigint-object-clone-by-value",
            """
            var o = Object(5n), wide = -(2n ** 1000n) + 1n;
            [typeof rt(0n), rt(wide) === wide, typeof rt(o), rt(o) !== o, rt(o).valueOf() === 5n, tag(rt(o)),
             Object.getPrototypeOf(rt(o)) === BigInt.prototype, rt([1n, 2])[0] === 1n].join(',');
            """,
            "bigint,true,object,true,true,[object BigInt],true,true"),
        (
            "clone/b06/a-shared-bigint-object-stays-shared-and-keys-survive",
            """
            var o = Object(3n);
            var c = rt({ list: [o, o], m: new Map([[1n, o]]), s: new Set([2n, 2n]) });
            [c.list[0] === c.list[1], c.m.get(1n) === c.list[0], c.list[0] !== o, c.s.size, c.s.has(2n)].join(',');
            """,
            "true,true,true,1,true"),
        (
            "clone/b06/a-bigint-counts-its-words-against-the-byte-bound",
            """
            function attempt(v, entries, bytes) {
                try { cloneHost.bounded(v, entries, bytes); return 'cloned'; }
                catch (e) { return String(e.message).indexOf('DataCloneError:') === 0 ? 'refused' : 'other:' + e; }
            }
            [attempt(1n, 50, 100), attempt(2n ** 8000n, 50, 100), attempt(2n ** 8000n, 50, 2000),
             attempt(Object(2n ** 8000n), 50, 100), attempt([2n ** 700n], 50, 150),
             attempt([2n ** 700n, 2n ** 700n], 50, 150)].join(',');
            """,
            "cloned,refused,cloned,refused,cloned,refused"),
    ];

    /// <summary>Runs every clone check.</summary>
    internal static System.Collections.Generic.List<(string Name, bool Passed, string Detail)> Run()
    {
        var results = new System.Collections.Generic.List<(string Name, bool Passed, string Detail)>();

        foreach (var (name, source, expected) in Programs)
        {
            results.Add(Judge(name, Prelude + source, expected));
        }

        foreach (var (name, source, expected) in BigIntPrograms)
        {
            results.Add(Judge(name, Prelude + source, expected));
        }

        results.Add(TheBoundRefusesAGraphPastIt());
        results.Add(AClonedGraphIsAdoptedByASecondRealmAfterTheFirstIsDisposed());
        results.Add(TransferredBytesCountAgainstTheBoundBeforeAnythingIsDetached());
        results.Add(ACarrierHoldingMovedBytesIsAdoptedOnceAfterItsSourceIsDisposed());
        results.Add(TheCloneIsChargedInProportionToTheGraph());
        return results;
    }

    /// <summary>Runs one same-realm program and compares its answer.</summary>
    private static (string, bool, string) Judge(string name, string source, string expected)
    {
        var surface = new CloneSurface(new CarrierBox());

        if (!TryRun(source, surface, fuel: null, out var answer, out var why))
        {
            return (name, false, why);
        }

        return (
            name,
            string.Equals(answer, expected, System.StringComparison.Ordinal),
            string.Equals(answer, expected, System.StringComparison.Ordinal)
                ? "the program answered " + answer
                : $"the program answered '{answer}' and '{expected}' was expected");
    }

    /// <summary>
    /// A bound narrowed to a few entries refuses a graph past it and admits one within it.
    /// </summary>
    /// <remarks>
    /// The carrier's own bounds are four million entries and 256 MiB, and a check that built a graph
    /// that large would spend its time building it. The internal serializer takes a narrower pair,
    /// never a wider one, so the refusal at the bound is reached with a graph of a hundred entries.
    /// </remarks>
    private static (string, bool, string) TheBoundRefusesAGraphPastIt()
    {
        const string Name = "clone/i14/validation-and-allocation-are-bounded";

        const string Source =
            """
            var small = [1, 2, 3], large = [];
            for (var i = 0; i < 100; i++) large.push({ i: i });
            var text = 'x'.repeat(64);
            function attempt(v, entries, bytes) {
                try { cloneHost.bounded(v, entries, bytes); return 'cloned'; }
                catch (e) { return String(e.message).indexOf('DataCloneError:') === 0 ? 'refused' : 'other:' + e; }
            }
            [attempt(small, 50, 1000), attempt(large, 50, 1000), attempt(text, 50, 100),
             attempt(new ArrayBuffer(200), 50, 100), attempt([text, text, text], 50, 200)].join(',');
            """;

        const string Expected = "cloned,refused,refused,refused,cloned";

        var surface = new CloneSurface(new CarrierBox());

        if (!TryRun(Source, surface, fuel: null, out var answer, out var why))
        {
            return (Name, false, why);
        }

        return (
            Name,
            string.Equals(answer, Expected, System.StringComparison.Ordinal),
            $"answered {answer} ({Expected} expected): a hundred-entry graph and a 128-byte string are " +
            "refused under narrowed bounds, a three-element array is admitted, and one string named " +
            "three times is counted once");
    }

    /// <summary>
    /// A graph serialized in one runtime is adopted by a second after the first is disposed.
    /// </summary>
    private static (string, bool, string) AClonedGraphIsAdoptedByASecondRealmAfterTheFirstIsDisposed()
    {
        const string Name = "clone/i14-i15/a-carrier-outlives-its-source-realm-and-rebuilds-on-the-destination";

        const string Capture =
            """
            var shared = { tag: 'shared' };
            var m = new Map([[shared, [shared, new Date(5)]]]);
            var buf = new ArrayBuffer(4); new Uint8Array(buf)[0] = 7;
            var graph = { m: m, list: [shared, , 3], err: new SyntaxError('s', { cause: shared }), re: /q/g,
                          u8: new Uint8Array(buf), dv: new DataView(buf), set: new Set([shared]) };
            graph.self = graph;
            // THE SOURCE REALM MARKS ITS OWN PROTOTYPES BEFORE THE CAPTURE, so a carrier that took
            // anything from a prototype, or a destination object that inherited from one of the
            // source's, would show the mark.
            Object.prototype.fromSource = true; Map.prototype.fromSource = true;
            cloneHost.capture(graph);
            'captured';
            """;

        const string Adopt =
            """
            var g = cloneHost.adopt(), h = cloneHost.adopt();
            var entry = Array.from(g.m.entries())[0];
            [Object.getPrototypeOf(g) === Object.prototype, g.self === g, 'fromSource' in g, 'fromSource' in g.m,
             g.m instanceof Map, entry[0] === entry[1][0], entry[0] === g.list[0], 1 in g.list,
             entry[1][1] instanceof Date, entry[1][1].getTime(), g.err instanceof SyntaxError,
             g.err.cause === g.list[0], g.re instanceof RegExp, g.re.flags, g.u8.buffer === g.dv.buffer, g.u8[0],
             g.set.has(g.list[0]), g !== h, g.u8.buffer !== h.u8.buffer, (g.u8[0] = 9, h.u8[0])].join(',');
            """;

        const string Expected =
            "true,true,false,false,true,true,true,false,true,5,true,true,true,g,true,7,true,true,true,7";

        var box = new CarrierBox();

        if (!TryRun(Capture, new CloneSurface(box), fuel: null, out var captured, out var whyCapture))
        {
            return (Name, false, "the source realm: " + whyCapture);
        }

        // THE SOURCE RUNTIME IS GONE BY NOW: TryRun disposes the instance and the runtime before it
        // returns, and a collection is forced so that nothing only the carrier kept alive survives
        // by accident of timing.
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();

        if (box.Carrier is null)
        {
            return (Name, false, $"the source realm answered {captured} and left no carrier");
        }

        if (!TryRun(Adopt, new CloneSurface(box), fuel: null, out var answer, out var whyAdopt))
        {
            return (Name, false, "the destination realm: " + whyAdopt);
        }

        return (
            Name,
            string.Equals(answer, Expected, System.StringComparison.Ordinal),
            $"the destination answered {answer} ({Expected} expected) after the source runtime was " +
            "disposed; its objects inherit from its own intrinsics, not the source's marked ones, " +
            "and two adoptions share no object and no buffer");
    }

    /// <summary>
    /// A transfer whose moved bytes pass the byte bound is refused, and the buffer stays usable.
    /// </summary>
    /// <remarks>
    /// Moved bytes are carried, so they count against <c>MaxBytes</c> like copied ones; the refusal
    /// comes from the validation that precedes every detachment, so the source must still read.
    /// </remarks>
    private static (string, bool, string) TransferredBytesCountAgainstTheBoundBeforeAnythingIsDetached()
    {
        const string Name = "clone/i16/moved-bytes-count-against-the-bound-before-anything-is-detached";

        const string Source =
            """
            var small = filled(8), large = filled(200);
            function attempt(v, list, entries, bytes) {
                try { cloneHost.transferBounded(v, list, entries, bytes); return 'cloned'; }
                catch (e) { return String(e.message).indexOf('DataCloneError:') === 0 ? 'refused' : 'other:' + e; }
            }
            [attempt({ s: small, l: large }, [small, large], 50, 100), intact(small, 8), intact(large, 200),
             attempt(small, [small], 50, 100), small.byteLength].join(',');
            """;

        const string Expected = "refused,true,true,cloned,0";

        var surface = new CloneSurface(new CarrierBox());

        if (!TryRun(Prelude + Source, surface, fuel: null, out var answer, out var why))
        {
            return (Name, false, why);
        }

        return (
            Name,
            string.Equals(answer, Expected, System.StringComparison.Ordinal),
            $"answered {answer} ({Expected} expected): a 200-byte transfer under a 100-byte bound is " +
            "refused with both listed buffers intact, and an 8-byte one moves");
    }

    /// <summary>
    /// A carrier that took a buffer's bytes outlives its source runtime and is adopted exactly once.
    /// </summary>
    private static (string, bool, string) ACarrierHoldingMovedBytesIsAdoptedOnceAfterItsSourceIsDisposed()
    {
        const string Name = "clone/i16/a-carrier-holding-moved-bytes-is-adopted-once-after-its-source-is-disposed";

        const string Capture =
            """
            var buf = filled(6), u = new Uint16Array(buf, 2, 2);
            cloneHost.captureTransfer({ buf: buf, u: u, again: u }, [buf]);
            [buf.byteLength, u.length].join(',');
            """;

        const string Adopt =
            """
            var g = cloneHost.adopt(), second;
            try { cloneHost.adopt(); second = 'adopted-twice'; }
            catch (e) { second = String(e.message) === "DataCloneError: the carrier's transferred buffers were already adopted" ? 'refused' : 'other:' + e; }
            [g.buf.byteLength, Array.from(new Uint8Array(g.buf)).join('+'), g.u.buffer === g.buf, g.again === g.u,
             g.u.byteOffset + '/' + g.u.length, Object.getPrototypeOf(g.buf) === ArrayBuffer.prototype, second].join(',');
            """;

        const string ExpectedCapture = "0,0";
        const string Expected = "6,1+2+3+4+5+6,true,true,2/2,true,refused";

        var box = new CarrierBox();

        if (!TryRun(Prelude + Capture, new CloneSurface(box), fuel: null, out var captured, out var whyCapture))
        {
            return (Name, false, "the source realm: " + whyCapture);
        }

        if (!string.Equals(captured, ExpectedCapture, System.StringComparison.Ordinal) || box.Carrier is null)
        {
            return (Name, false, $"the source realm answered {captured} ({ExpectedCapture} expected)");
        }

        // THE SOURCE RUNTIME IS GONE: the moved bytes now belong to the carrier alone.
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();

        if (!TryRun(Adopt, new CloneSurface(box), fuel: null, out var answer, out var whyAdopt))
        {
            return (Name, false, "the destination realm: " + whyAdopt);
        }

        return (
            Name,
            string.Equals(answer, Expected, System.StringComparison.Ordinal),
            $"the source saw its buffer and view detached ({captured}); the destination answered " +
            $"{answer} ({Expected} expected): the bytes arrived once and a second adoption is refused");
    }

    /// <summary>
    /// The clone's charge grows with the graph, measured by bisecting the fuel allowance.
    /// </summary>
    /// <remarks>
    /// The method is ProportionalityChecks' own: the smallest allowance a program completes under is
    /// what it costs, and a control that is the same program minus the clone isolates the clone.
    /// The declared floor is one unit per element serialized and rebuilt, which the implementation
    /// exceeds several times over; the shape is that doubling the graph at least half again the
    /// charge, which a flat charge fails.
    /// </remarks>
    private static (string, bool, string) TheCloneIsChargedInProportionToTheGraph()
    {
        const string Name = "clone/i14/the-clone-is-charged-in-proportion-to-the-graph";

        int[] magnitudes = [64, 128, 256, 512];
        var charges = new System.Collections.Generic.List<ulong>();

        foreach (var n in magnitudes)
        {
            var build = $"var a = []; for (var i = 0; i < {n}; i++) a.push({{ v: i, s: 'x' }});";
            var candidate = Cost(build + " cloneHost.roundTrip(a); 'done';");
            var control = Cost(build + " cloneHost.roundTrip(0); 'done';");

            if (candidate is null || control is null || candidate <= control)
            {
                return (Name, false, $"at n={n} the candidate cost {candidate} and the control {control}");
            }

            charges.Add(candidate.Value - control.Value);
        }

        var detail = string.Join(", ", magnitudes.Select((n, at) => $"n={n}: {charges[at]}"));

        for (var at = 0; at < magnitudes.Length; at++)
        {
            if (charges[at] < (ulong)magnitudes[at])
            {
                return (Name, false, $"below the declared floor of one unit per element: {detail}");
            }

            if (at > 0 && charges[at] * 2 < charges[at - 1] * 3)
            {
                return (Name, false, $"doubling the graph did not grow the charge by half again: {detail}");
            }
        }

        return (Name, true, "attributed fuel " + detail);
    }

    /// <summary>The smallest fuel allowance a program completes under, by bisection.</summary>
    private static ulong? Cost(string source)
    {
        const ulong Ceiling = 16_000_000;

        if (!TryRun(source, new CloneSurface(new CarrierBox()), Ceiling, out _, out _))
        {
            return null;
        }

        ulong low = 0, high = Ceiling;

        while (high - low > 1)
        {
            var middle = low + ((high - low) / 2);

            if (TryRun(source, new CloneSurface(new CarrierBox()), middle, out _, out _))
            {
                high = middle;
            }
            else
            {
                low = middle;
            }
        }

        return high;
    }

    /// <summary>Compiles, verifies, instantiates and runs one program against one surface.</summary>
    private static bool TryRun(string source, CloneSurface surface, ulong? fuel, out string answer, out string why)
    {
        answer = string.Empty;

        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", source, SliceParseOptions.Script, false, Caller)],
            [],
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            why = "the source was refused: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString());

            return false;
        }

        var catalog = VmCatalog.CreateBuilder()
            .Add(JavaScriptProfile.DescriptorHostingRealms(surface))
            .Build();

        var created = VmRuntime.Create(catalog, Options(fuel));

        if (!created.TryGetRuntime(out var runtime))
        {
            why = $"the runtime refused creation: {created.Outcome}/{created.Reason}";
            return false;
        }

        using (runtime)
        {
            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            var verified = runtime.Verify(in descriptor, compiled.Artifact, System.Threading.CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                why = $"verification refused: {verified.Outcome}/{verified.Reason}";
                return false;
            }

            using (artifact)
            {
                var instantiated = runtime.Instantiate(artifact, System.Threading.CancellationToken.None);

                if (!instantiated.TryGetInstance(out var instance))
                {
                    why = $"instantiation refused: {instantiated.Outcome}/{instantiated.Reason}";
                    return false;
                }

                using (instance)
                {
                    var request = new VmInvocationRequest(
                        new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));

                    var invoked = instance.Invoke(in request, System.Threading.CancellationToken.None);

                    if (!JavaScriptProfile.TryGetWideCompletion(in invoked, out var completion))
                    {
                        why = $"the run answered {invoked.Outcome}/{invoked.Reason}";
                        return false;
                    }

                    answer = completion.Value;
                    why = string.Empty;
                    return true;
                }
            }
        }
    }

    /// <summary>The runtime options: the host-surface permission, and a fuel ceiling when one is given.</summary>
    private static VmRuntimeCreationOptions Options(ulong? fuel)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.AdoptParentRemaining(dimension),
                VmBudgetDimension.Fuel when fuel is { } allowance => VmCeilingSpec.Value(dimension, allowance),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
        }

        // THE PERMISSION, AND IT CARRIES NO TRAFFIC, as in the command-line root's host-surface
        // lane: the profile asks whether it is bound, once, and nothing addresses the slot again.
        var capabilities = ImmutableArray.Create(
            VmCapabilityRegistration.Value(
                JavaScriptProfile.HostSurfaceCapability,
                (VmBytes argument, out VmOpaqueRef answer) =>
                {
                    answer = default;
                    return VmHostCallOutcome.Completed;
                }));

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities);
    }

    /// <summary><c>JsHostRealm.CloneSerialize</c>, which is internal to the profile.</summary>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "CloneSerialize")]
    [return: UnsafeAccessorType(CarrierType)]
    private static extern object Serialize(JsHostRealm realm, JsHostValue value, long maxEntries, long maxBytes);

    /// <summary><c>JsHostRealm.CloneSerializeWithTransfer</c>, which is internal to the profile.</summary>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "CloneSerializeWithTransfer")]
    [return: UnsafeAccessorType(CarrierType)]
    private static extern object SerializeWithTransfer(
        JsHostRealm realm, JsHostValue value, JsHostValue[] transfer, long maxEntries, long maxBytes);

    /// <summary><c>JsHostRealm.CloneDeserialize</c>, which is internal to the profile.</summary>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "CloneDeserialize")]
    private static extern JsHostValue Deserialize(JsHostRealm realm, [UnsafeAccessorType(CarrierType)] object carrier);

    /// <summary>Where a carrier waits between the realm that made it and the one that adopts it.</summary>
    private sealed class CarrierBox
    {
        /// <summary>The carrier, as an opaque object: its type is internal to the profile.</summary>
        internal object? Carrier { get; set; }
    }

    /// <summary>An embedder that gives a realm <c>cloneHost</c> and nothing else.</summary>
    private sealed class CloneSurface(CarrierBox box) : IJsHostSurface
    {
        public void OnTurn(JsHostRealm realm)
        {
        }

        public void OnRealmCreated(JsHostRealm realm)
        {
            var host = realm.NewObject();

            realm.DefineValue(
                host,
                "roundTrip",
                realm.NewMethod(
                    "roundTrip",
                    (r, _, arguments) =>
                    {
                        var carrier = Serialize(r, Argument(arguments, 0), Unbounded, Unbounded);
                        return Deserialize(r, carrier);
                    },
                    1));

            realm.DefineValue(
                host,
                "bounded",
                realm.NewMethod(
                    "bounded",
                    (r, _, arguments) =>
                    {
                        var entries = (long)r.ToNumber(Argument(arguments, 1));
                        var bytes = (long)r.ToNumber(Argument(arguments, 2));
                        var carrier = Serialize(r, Argument(arguments, 0), entries, bytes);
                        return Deserialize(r, carrier);
                    },
                    3));

            realm.DefineValue(
                host,
                "capture",
                realm.NewMethod(
                    "capture",
                    (r, _, arguments) =>
                    {
                        box.Carrier = Serialize(r, Argument(arguments, 0), Unbounded, Unbounded);
                        return JsHostValue.Undefined;
                    },
                    1));

            realm.DefineValue(
                host,
                "adopt",
                realm.NewMethod(
                    "adopt",
                    (r, _, _) => box.Carrier is { } carrier
                        ? Deserialize(r, carrier)
                        : throw r.Error(JsHostErrorKind.TypeError, "no carrier was captured")));

            realm.DefineValue(
                host,
                "transfer",
                realm.NewMethod(
                    "transfer",
                    (r, _, arguments) =>
                    {
                        var carrier = SerializeWithTransfer(
                            r, Argument(arguments, 0), List(r, Argument(arguments, 1)), Unbounded, Unbounded);

                        return Deserialize(r, carrier);
                    },
                    2));

            realm.DefineValue(
                host,
                "transferBounded",
                realm.NewMethod(
                    "transferBounded",
                    (r, _, arguments) =>
                    {
                        var entries = (long)r.ToNumber(Argument(arguments, 2));
                        var bytes = (long)r.ToNumber(Argument(arguments, 3));
                        var carrier = SerializeWithTransfer(
                            r, Argument(arguments, 0), List(r, Argument(arguments, 1)), entries, bytes);

                        return Deserialize(r, carrier);
                    },
                    4));

            realm.DefineValue(
                host,
                "captureTransfer",
                realm.NewMethod(
                    "captureTransfer",
                    (r, _, arguments) =>
                    {
                        box.Carrier = SerializeWithTransfer(
                            r, Argument(arguments, 0), List(r, Argument(arguments, 1)), Unbounded, Unbounded);

                        return JsHostValue.Undefined;
                    },
                    2));

            realm.DefineValue(realm.Global, "cloneHost", host);
        }

        /// <summary>A guest array's elements, read by index: the transfer list as a host hands it over.</summary>
        private static JsHostValue[] List(JsHostRealm realm, JsHostValue array)
        {
            var length = (int)realm.ToNumber(realm.GetProperty(array, "length"));
            var list = new JsHostValue[length];

            for (var at = 0; at < length; at++)
            {
                list[at] = realm.GetIndex(array, (uint)at);
            }

            return list;
        }

        private static JsHostValue Argument(System.ReadOnlySpan<JsHostValue> arguments, int at) =>
            at < arguments.Length ? arguments[at] : JsHostValue.Undefined;
    }
}
