// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER BigInt64Array, BigUint64Array AND THE DataView BigInt ACCESSORS,
// retained from JSeal slices B07-B08. The two kinds exist where BigInt does (JSD-0033 section 7):
// their elements read as BigInts, every conversion into one is ToBigInt (so a Number is a
// TypeError), a write stores the value modulo 2**64, and copying between a BigInt and a Number
// typed array is a TypeError in every direction. Atomics and SharedArrayBuffer stay absent
// (JSD-0028) and are not asked about here.
var __n = 0;
function s(v) { return typeof v === "bigint" ? v + "n" : typeof v === "string" ? JSON.stringify(v) : String(v); }
function t(f) { try { var v = f(); return s(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function list(a) { return Array.prototype.map.call(a, s).join(","); }

// 1-6: the globals, their shape and their place in the hierarchy.
p(function () { return [typeof BigInt64Array, typeof BigUint64Array, BigInt64Array.name, BigUint64Array.name, BigInt64Array.length, BigUint64Array.length].join(","); });
p(function () { return [BigInt64Array.BYTES_PER_ELEMENT, BigUint64Array.BYTES_PER_ELEMENT, BigInt64Array.prototype.BYTES_PER_ELEMENT, BigUint64Array.prototype.BYTES_PER_ELEMENT].join(","); });
p(function () { var TA = Object.getPrototypeOf(Int8Array); return [Object.getPrototypeOf(BigInt64Array) === TA, Object.getPrototypeOf(BigUint64Array.prototype) === TA.prototype, BigInt64Array.from === Int8Array.from, BigUint64Array.of === TA.of].join(","); });
p(function () { return [Object.prototype.toString.call(new BigInt64Array(1)), new BigUint64Array(1)[Symbol.toStringTag], ArrayBuffer.isView(new BigInt64Array(0))].join(","); });
p(function () { return [t(function () { return BigInt64Array(1); }), t(function () { return BigUint64Array(); })].join(","); });
p(function () { var d = Object.getOwnPropertyDescriptor(BigInt64Array, "BYTES_PER_ELEMENT"); return [d.writable, d.enumerable, d.configurable].join(","); });

// 7-12: construction from a length, an array, an iterable, an array-like, a buffer.
p(function () { var a = new BigInt64Array(3); return [a.length, a.byteLength, list(a)].join("|"); });
p(function () { return list(new BigInt64Array([1n, -2n, 3n])) + "|" + list(new BigUint64Array([1n, -2n, 3n])); });
p(function () { return list(new BigInt64Array(new Set([5n, 6n]))) + "|" + list(new BigUint64Array((function* () { yield 7n; yield "8"; yield true; })())); });
p(function () { return list(new BigInt64Array({ length: 2, 0: 9n, 1: "0x10" })) + "|" + list(new BigInt64Array("2")); });
p(function () { var b = new ArrayBuffer(24); var a = new BigInt64Array(b, 8); var c = new BigUint64Array(b, 8, 1); return [a.length, a.byteOffset, c.length, t(function () { return new BigInt64Array(b, 4); }), t(function () { return new BigInt64Array(new ArrayBuffer(12)); })].join(","); });
p(function () { return [t(function () { return new BigInt64Array([1]); }), t(function () { return new BigInt64Array([1.5]); }), t(function () { return new BigUint64Array({ length: 1, 0: undefined }); }), t(function () { return new BigInt64Array(["1.5"]); }), t(function () { return new BigInt64Array([Symbol()]); })].join(","); });

// 13-17: truncation modulo 2**64 and signedness.
p(function () { return list(new BigInt64Array([2n ** 63n, 2n ** 63n - 1n, -(2n ** 63n), -(2n ** 63n) - 1n, 2n ** 64n, 2n ** 64n + 5n])); });
p(function () { return list(new BigUint64Array([-1n, 2n ** 64n, 2n ** 64n - 1n, -(2n ** 64n) - 1n, 2n ** 70n + 3n])); });
p(function () { var a = new BigInt64Array(1); a[0] = 2n ** 200n - 1n; var u = new BigUint64Array(a.buffer); return s(a[0]) + "," + s(u[0]); });
p(function () { var a = new BigInt64Array(2); a[0] = 12345678901234567890123n; a[1] = -12345678901234567890123n; return list(a); });
p(function () { var u = new BigUint64Array(1); u[0] = -1n; var b = new Uint8Array(u.buffer); return list(b) + "|" + s(new BigInt64Array(u.buffer)[0]); });

// 18-23: element writes convert with ToBigInt.
p(function () { var a = new BigInt64Array(1); return [t(function () { a[0] = 1; }), t(function () { a[0] = "12"; return a[0]; }), t(function () { a[0] = true; return a[0]; }), t(function () { a[0] = "x"; }), t(function () { a[0] = null; })].join(","); });
p(function () { var a = new BigInt64Array(1); var r = t(function () { "use strict"; a[5] = 1n; return "ok"; }); var q = t(function () { a[5] = 1; }); return r + "," + q + "," + a[5]; });
p(function () { var a = new BigInt64Array(1); var log = []; a[0] = { valueOf: function () { log.push("v"); return 7n; } }; return s(a[0]) + log.join(""); });
p(function () { var a = new BigInt64Array(1); return [Reflect.set(a, 0, 3n), s(a[0]), t(function () { return Reflect.set(a, 0, 3); }), t(function () { Object.defineProperty(a, "0", { value: 4 }); }), t(function () { Object.defineProperty(a, "0", { value: 5n }); return a[0]; })].join(","); });
p(function () { var f = new Float64Array(1); return [t(function () { f[0] = 1n; }), t(function () { new Float64Array([1n]); }), t(function () { new Int8Array(1).fill(1n); }), t(function () { Float64Array.of(1n); })].join(","); });
p(function () { var a = new BigInt64Array(2); return [t(function () { a[-0] = 1n; return a[0]; }), a["1.5"], "-0" in a, Object.keys(a).join("/"), JSON.stringify(Object.getOwnPropertyDescriptor(a, "1"), function (k, v) { return typeof v === "bigint" ? v + "n" : v; })].join(","); });

// 24-29: content types do not mix.
p(function () { return [t(function () { return new BigInt64Array(new Float64Array(1)); }), t(function () { return new Float64Array(new BigInt64Array(1)); }), list(new BigUint64Array(new BigInt64Array([-1n])))].join(","); });
p(function () { var a = new BigInt64Array(2); return [t(function () { a.set(new Int8Array(1)); }), t(function () { new Int8Array(2).set(a); }), t(function () { a.set(new BigUint64Array([-1n]), 1); return list(a); }), t(function () { a.set([1]); })].join(","); });
p(function () { var a = new BigInt64Array(2); return [t(function () { a.set(new Float64Array(1), 5); }), t(function () { a.set([5n, 6n]); return list(a); }), t(function () { a.set(["7", true], 0); return list(a); })].join(","); });
p(function () { var a = new BigInt64Array([1n, 2n]); a.constructor = { [Symbol.species]: Float64Array }; return [t(function () { return a.map(function (x) { return x; }); }), t(function () { return a.slice(); }), t(function () { return a.filter(function () { return true; }); }), t(function () { return a.subarray(0); })].join(","); });
p(function () { var a = new BigInt64Array([1n, 2n]); a.constructor = { [Symbol.species]: BigUint64Array }; var m = a.map(function (x) { return -x; }); return m.constructor.name + ":" + list(m) + "|" + list(a.slice(1)); });
p(function () { var f = new Float64Array([1]); f.constructor = { [Symbol.species]: BigInt64Array }; return [t(function () { return f.map(function (x) { return x; }); }), t(function () { return f.slice(0); })].join(","); });

// 30-36: the %TypedArray% methods over BigInt elements.
p(function () { var a = new BigInt64Array([3n, -1n, 2n]); return [s(a.at(-1)), a.includes(-1n), a.includes(-1), a.indexOf(2n), a.indexOf(2), a.lastIndexOf(3n), a.join("|"), String(a), a.toLocaleString()].join(","); });
p(function () { var a = new BigInt64Array([1n, 2n, 3n, 4n]); return [list(a.map(function (x) { return x * x; })), list(a.filter(function (x) { return x % 2n === 0n; })), s(a.reduce(function (x, y) { return x + y; })), s(a.reduceRight(function (x, y) { return x - y; }, 0n)), a.every(function (x) { return x > 0n; }), a.some(function (x) { return x > 3n; })].join("|"); });
p(function () { var a = new BigUint64Array([5n, 6n, 7n]); return [s(a.find(function (x) { return x > 5n; })), a.findIndex(function (x) { return x === 7n; }), s(a.findLast(function (x) { return x < 7n; })), a.findLastIndex(function (x) { return x === 1n; })].join(","); });
p(function () { var a = new BigInt64Array([1n, 2n, 3n, 4n, 5n]); return [list(a.slice(1, 3)), list(a.subarray(2)), list(a.copyWithin(0, 3)), list(a.reverse()), list(a.fill(-9n, 3)), t(function () { a.fill(1); })].join("|"); });
p(function () { var a = new BigInt64Array([3n, -(2n ** 63n), 2n ** 62n, 0n, -1n]); var u = new BigUint64Array([3n, 2n ** 64n - 1n, 0n, 2n ** 63n]); return list(a.sort()) + "|" + list(u.sort()) + "|" + list(u.sort(function (x, y) { return x < y ? 1 : x > y ? -1 : 0; })); });
p(function () { var a = new BigInt64Array([2n, 1n, 3n]); return [list(a.toSorted()), list(a.toReversed()), list(a.with(1, 10n)), list(a), t(function () { a.with(0, 1); }), t(function () { a.with(5, 1n); })].join("|"); });
p(function () { var a = new BigUint64Array([10n, 20n]); var out = []; a.forEach(function (x, i) { out.push(i + ":" + s(x)); }); return out.join(",") + "|" + [...a].map(s).join(",") + "|" + [...a.keys()].join(",") + "|" + [...a.entries()].map(function (e) { return e[0] + "=" + s(e[1]); }).join(","); });

// 37-40: from and of.
p(function () { return [list(BigInt64Array.from([1n, "2", true])), list(BigUint64Array.from({ length: 2, 0: -1n, 1: 1n })), list(BigInt64Array.from([1, 2], function (x) { return BigInt(x) * 3n; })), list(BigInt64Array.of(4n, -5n))].join("|"); });
p(function () { return [t(function () { return BigInt64Array.from([1]); }), t(function () { return BigInt64Array.of(1); }), t(function () { return BigUint64Array.from([1n], function (x) { return 1; }); })].join(","); });
p(function () { class B extends BigInt64Array {} var b = B.from([1n, 2n]); return [b instanceof B, b instanceof BigInt64Array, list(b), list(B.of(3n)), list(b.map(function (x) { return x + 1n; })), b.map(function (x) { return x; }).constructor === B].join(","); });
p(function () { var o = { length: 2, get 0() { return 1n; }, get 1() { throw new RangeError("x"); } }; return t(function () { return BigInt64Array.from(o); }); });

// 41-46: detachment, resizing and aliasing within one buffer.
p(function () { var a = new BigInt64Array([1n, 2n]); a.buffer.transfer(); return [a.length, a.byteLength, s(a[0]), t(function () { return a.at(0); }), t(function () { a[0] = 1n; return "ok"; }), t(function () { a[0] = 1; })].join(","); });
p(function () { var a = new BigInt64Array(2); var b = a.buffer; return t(function () { a.fill({ valueOf: function () { b.transfer(); return 1n; } }); }); });
p(function () { var rab = new ArrayBuffer(16, { maxByteLength: 40 }); var a = new BigInt64Array(rab); var f = new BigUint64Array(rab, 8, 1); a[1] = -1n; rab.resize(32); var r1 = [a.length, f.length, s(f[0])]; rab.resize(8); return r1.join(",") + "|" + [a.length, f.length, s(f[0]), t(function () { return f.at(0); })].join(","); });
p(function () { var b = new ArrayBuffer(16); var s64 = new BigInt64Array(b); var u64 = new BigUint64Array(b); var u8 = new Uint8Array(b); var dv = new DataView(b); u8[7] = 0x80; s64[1] = 258n; return [s(s64[0]), s(u64[0]), s(dv.getBigUint64(8, true)), dv.getUint16(8, true)].join(","); });
p(function () { var b = new ArrayBuffer(32); var a = new BigInt64Array(b); a.set([1n, 2n, 3n]); var sub = new BigInt64Array(b, 8, 3); sub.set(a.subarray(0, 3)); return list(a); });
p(function () { var rab = new ArrayBuffer(24, { maxByteLength: 24 }); var a = new BigInt64Array(rab); a.set([1n, 2n, 3n]); var n = a.map(function (x, i) { if (i === 0) rab.resize(8); return x; }); return list(n); });

// 47-48: structured JSON and Map/Set keys.
p(function () { return [t(function () { return JSON.stringify(new BigInt64Array(1)); }), JSON.stringify(new BigInt64Array(0)), t(function () { return Object.entries(new BigUint64Array([1n])).join(); })].join(","); });
p(function () { var a = new BigInt64Array([1n]); var m = new Map([[a[0], "one"]]); var st = new Set([a[0], 1n, BigInt(1)]); return [m.get(1n), st.size, Object.is(a[0], a[0]), a[0] === a[0]].join(","); });

// 49-60: the DataView BigInt accessors (B08).
p(function () { var P = DataView.prototype; return [typeof P.getBigInt64, P.getBigInt64.length, P.getBigUint64.length, P.setBigInt64.length, P.setBigUint64.length, P.getBigInt64.name, P.setBigUint64.name].join(","); });
p(function () { var d = new DataView(new ArrayBuffer(16)); d.setBigInt64(0, -2n); d.setBigUint64(8, 0x0102030405060708n, true); return [s(d.getBigInt64(0)), s(d.getBigUint64(0)), s(d.getBigInt64(0, true)), d.getUint8(8), d.getUint8(15), s(d.getBigUint64(8, true)), s(d.getBigUint64(8))].join(","); });
p(function () { var d = new DataView(new ArrayBuffer(17)); d.setBigInt64(1, 2n ** 63n); d.setBigUint64(9, -1n, true); return [s(d.getBigInt64(1)), s(d.getBigUint64(1)), s(d.getBigInt64(9, true)), s(d.getBigUint64(9, 1))].join(","); });
p(function () { var d = new DataView(new ArrayBuffer(8)); d.setBigInt64(0, 2n ** 64n + 3n); var a = s(d.getBigInt64(0)); d.setBigUint64(0, -(2n ** 65n) - 1n); return a + "," + s(d.getBigUint64(0)) + "," + s(d.getBigInt64(0)); });
p(function () { var d = new DataView(new ArrayBuffer(8)); return [t(function () { d.setBigInt64(0, 1); }), t(function () { d.setBigUint64(0, undefined); }), t(function () { d.setBigInt64(0, "0x1f"); return d.getBigInt64(0); }), t(function () { d.setBigInt64(0, true); return d.getBigUint64(0); }), t(function () { d.setBigInt64(0); })].join(","); });
p(function () { var d = new DataView(new ArrayBuffer(12), 2); return [t(function () { return d.getBigInt64(3); }), t(function () { return d.getBigInt64(2); }), t(function () { return d.getBigUint64(-1); }), t(function () { d.setBigInt64(3, 0n); }), t(function () { return d.getBigInt64(Infinity); })].join(","); });
p(function () { var d = new DataView(new ArrayBuffer(8)); var log = []; var r = t(function () { d.setBigInt64({ valueOf: function () { log.push("i"); return 0; } }, { valueOf: function () { log.push("v"); return 1n; } }, { valueOf: function () { log.push("e"); return 1; } }); }); return r + ":" + log.join(""); });
p(function () { var d = new DataView(new ArrayBuffer(8)); var log = []; var r = t(function () { d.setBigInt64({ valueOf: function () { log.push("i"); return 100; } }, { valueOf: function () { log.push("v"); return 1; } }); }); return r + ":" + log.join(""); });
p(function () { var b = new ArrayBuffer(8); var d = new DataView(b); return [t(function () { d.setBigInt64(0, { valueOf: function () { b.transfer(); return 1n; } }); }), t(function () { return d.getBigInt64(0); }), t(function () { return d.getBigInt64(100); })].join(","); });
p(function () { var rab = new ArrayBuffer(16, { maxByteLength: 16 }); var d = new DataView(rab, 8); return [t(function () { d.setBigUint64(0, { valueOf: function () { rab.resize(12); return 1n; } }); }), t(function () { return d.getBigUint64(0); }), d.byteLength].join(","); });
p(function () { return [t(function () { return DataView.prototype.getBigInt64.call(new BigInt64Array(1), 0); }), t(function () { return DataView.prototype.setBigUint64.call({}, 0, 0n); })].join(","); });
p(function () { var d = new DataView(new ArrayBuffer(8)); d.setFloat64(0, -0); var a = s(d.getBigUint64(0)); d.setBigUint64(0, 0x7ff8000000000000n); return a + "," + d.getFloat64(0) + "," + s(d.getBigInt64(0, false)); });

// 61-63: JSON.parse's reviver writes through CreateDataProperty, the typed array's own
// [[DefineOwnProperty]], so a revived element converts by the content type (review fix, 2026-09-22).
p(function () { function rv(T, x) { var r; JSON.parse('{"a":1,"b":{"0":1}}', function (k, v) { if (k === "a") this.b = r = new T(2); if (k === "0") return x; return v; }); return s(r[0]); } return [t(function () { return rv(BigInt64Array, 7); }), t(function () { return rv(BigInt64Array, "12"); }), t(function () { return rv(BigUint64Array, { valueOf: function () { return -1n; } }); }), t(function () { return rv(BigInt64Array, true); })].join(","); });
p(function () { function rv(T, x) { var r; JSON.parse('{"a":1,"b":{"0":1}}', function (k, v) { if (k === "a") this.b = r = new T(2); if (k === "0") return x; return v; }); return s(r[0]); } return [t(function () { return rv(Float64Array, 5n); }), t(function () { return rv(Float64Array, { valueOf: function () { return 3; } }); }), t(function () { return rv(BigInt64Array, undefined); })].join(","); });
p(function () { var r; JSON.parse('{"a":1,"b":{"0":1,"5":2}}', function (k, v) { if (k === "a") this.b = r = new BigInt64Array(2); if (k === "5") return 7n; return v; }); var f; JSON.parse('{"a":1,"b":{"x":1}}', function (k, v) { if (k === "a") this.b = f = Object.freeze({ x: 1 }); if (k === "x") return 7; return v; }); return [r.length, s(r[5]), Object.keys(r).join(), f.x].join(","); });
