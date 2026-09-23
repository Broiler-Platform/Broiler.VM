// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER HALF PRECISION: the binary16 conversion `Math.f16round` shares with
// `DataView.prototype.getFloat16`/`setFloat16` and `Float16Array`, and the iterable argument every
// typed array constructor takes.
//
// Retained from JSeal slices F01-F03. The conversion is checked EXHAUSTIVELY rather than by
// example: all 65536 bit patterns are decoded through a DataView and compared with a decoding
// written here from the format's definition, every finite pattern is re-encoded, and every
// rounding boundary between two neighbouring finite values - the midpoint and the doubles either
// side of it - is rounded through `Math.f16round`, a `setFloat16` and a Float16Array element. The
// loops print counts, so a divergence still names one case.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function is(a, b) { return Object.is(a, b); }
function Boom() { this.name = "Boom"; }

// The format, from its definition: 1 sign bit, 5 exponent bits biased by 15, 10 fraction bits.
function decode(h) {
  var sign = (h & 0x8000) ? -1 : 1, e = (h >> 10) & 0x1f, f = h & 0x3ff;
  if (e === 0x1f) { return f === 0 ? sign * Infinity : NaN; }
  if (e === 0) { return sign * f * Math.pow(2, -24); }
  return sign * (1024 + f) * Math.pow(2, e - 25);
}
// The next double above a positive finite double, through its bits (the platform order is little-endian).
var bits64 = new Float64Array(1), words = new Uint32Array(bits64.buffer);
function nextUp(x) { bits64[0] = x; if (words[0] === 0xffffffff) { words[0] = 0; words[1]++; } else { words[0]++; } return bits64[0]; }
function nextDown(x) { bits64[0] = x; if (words[0] === 0) { words[0] = 0xffffffff; words[1]--; } else { words[0]--; } return bits64[0]; }
var dv = new DataView(new ArrayBuffer(4));
var half = new Float16Array(1);
// The three rounding paths the realm has; each must answer what the others do.
function viaView(x) { dv.setFloat16(0, x); return dv.getFloat16(0); }
function viaArray(x) { half[0] = x; return half[0]; }
function rounded(x, want) { return is(Math.f16round(x), want) && is(viaView(x), want) && is(viaArray(x), want); }

// --- F01: every bit pattern decodes as the format defines, and every finite one re-encodes to itself
p(function () {
  var wrong = 0, nans = 0, back = 0;
  for (var h = 0; h < 65536; h++) {
    dv.setUint16(0, h);
    var v = dv.getFloat16(0);
    if (v !== v) { nans++; if (!isNaN(decode(h))) wrong++; continue; }
    if (!is(v, decode(h))) wrong++;
    dv.setUint16(0, 0); dv.setFloat16(0, v);
    if (dv.getUint16(0) !== h) back++;
    if (!is(Math.f16round(v), v)) back++;
  }
  return "wrong " + wrong + " nan " + nans + " not-round-tripped " + back;
});
p(function () {
  // The same patterns through a Float16Array aliasing a Uint16Array: the element read is the decode.
  var u = new Uint16Array(65536), f = new Float16Array(u.buffer), wrong = 0;
  for (var h = 0; h < 65536; h++) u[h] = h;
  for (h = 0; h < 65536; h++) { var v = f[h]; if (v === v ? !is(v, decode(h)) : !isNaN(decode(h))) wrong++; }
  return "wrong " + wrong + " length " + f.length + " bytes " + f.byteLength;
});
// --- F01: every rounding boundary between two neighbouring positive finite values, and its mirror,
// through Math.f16round; then a sample of them through all three paths, which must agree
p(function () {
  var wrong = 0, ties = 0, f = Math.f16round;
  for (var h = 0; h < 0x7bff; h++) {
    var lo = decode(h), hi = decode(h + 1), mid = (lo + hi) / 2, even = (h & 1) === 0 ? lo : hi;
    var below = nextDown(mid), above = nextUp(mid);
    ties++;
    if (!is(f(mid), even) || !is(f(-mid), -even) || !is(f(below), lo) || !is(f(above), hi) ||
        !is(f(-below), -lo) || !is(f(-above), -hi)) wrong++;
  }
  return "wrong " + wrong + " boundaries " + ties;
});
p(function () {
  var wrong = 0, ties = 0;
  for (var h = 0; h < 0x7bff; h += (h & 0x3ff) < 3 || (h & 0x3ff) > 0x3fc ? 1 : 61) {
    var lo = decode(h), hi = decode(h + 1), mid = (lo + hi) / 2, even = (h & 1) === 0 ? lo : hi;
    ties++;
    if (!rounded(mid, even) || !rounded(-mid, -even)) wrong++;
    if (!rounded(nextDown(mid), lo) || !rounded(nextUp(mid), hi)) wrong++;
  }
  return "wrong " + wrong + " boundaries " + ties;
});
// --- F01: overflow, the subnormal floor, signed zero, NaN, and a double that a float would round twice
p(function () { return [rounded(65520, Infinity), rounded(nextDown(65520), 65504), rounded(-65520, -Infinity), rounded(1e300, Infinity), rounded(-Infinity, -Infinity)].join(); });
p(function () { return [rounded(Math.pow(2, -25), 0), rounded(nextUp(Math.pow(2, -25)), Math.pow(2, -24)), rounded(-Math.pow(2, -25), -0), rounded(1e-300, 0), rounded(-1e-300, -0)].join(); });
p(function () { return [rounded(-0, -0), rounded(0, 0), isNaN(Math.f16round(NaN)), isNaN(viaView(NaN)), isNaN(viaArray(NaN))].join(); });
p(function () { return [rounded(1 + Math.pow(2, -11) + Math.pow(2, -40), 1 + Math.pow(2, -10)), rounded(1 + Math.pow(2, -11), 1), rounded(1.337, 1.3369140625), rounded(1 / 3, 0.333251953125)].join(); });
p(function () { return [Math.f16round("1.5"), Math.f16round(), Math.f16round(true), Math.f16round(5e-8), Math.f16round(65504.5)].join(); });

// --- F02: the accessors, their shape, both byte orders and an unaligned offset
p(function () { var g = DataView.prototype.getFloat16, s = DataView.prototype.setFloat16; return [typeof g, g.length, g.name, typeof s, s.length, s.name].join(); });
p(function () { var d = new DataView(new ArrayBuffer(5)); d.setFloat16(1, 1.5); d.setFloat16(3, -2, true); return [d.getUint8(1), d.getUint8(2), d.getUint8(3), d.getUint8(4), d.getFloat16(1), d.getFloat16(3, true), d.getFloat16(3)].join(); });
p(function () { var d = new DataView(new ArrayBuffer(2)); var out = []; [0x7c00, 0xfc00, 0x7bff, 0x0001, 0x8000, 0x0400, 0x3555].forEach(function (h) { d.setUint16(0, h); out.push(d.getFloat16(0)); }); return out.join() + " " + is(out[4], -0); });
p(function () { var d = new DataView(new ArrayBuffer(2)); d.setFloat16(0, NaN); var h = d.getUint16(0); return ((h & 0x7c00) === 0x7c00 && (h & 0x3ff) !== 0) + " " + isNaN(d.getFloat16(0)); });
p(function () { var d = new DataView(new ArrayBuffer(8), 2, 4); d.setFloat16(2, 3.140625); return d.getFloat16(2) + " " + new Uint8Array(d.buffer).join(); });
// --- F02: bounds, invalid receivers, and the order of the coercions against detachment
p(function () { var d = new DataView(new ArrayBuffer(4)); return [t(function () { return d.getFloat16(3); }), t(function () { return d.getFloat16(-1); }), t(function () { return d.setFloat16(3, 1); }), t(function () { return d.getFloat16(Infinity); }), t(function () { return d.getFloat16(2.9); })].join(); });
p(function () { var g = DataView.prototype.getFloat16; return [t(function () { return g.call({}, 0); }), t(function () { return g.call(new Uint8Array(4), 0); }), t(function () { return g.call(new ArrayBuffer(4), 0); }), t(function () { return DataView.prototype.setFloat16.call(undefined, 0, 1); })].join(); });
p(function () { var b = new ArrayBuffer(4), d = new DataView(b); b.transfer(); return [t(function () { return d.getFloat16(0); }), t(function () { return d.setFloat16(0, 1); }), t(function () { return d.getFloat16(-1); }), t(function () { return d.setFloat16(-1, 1); })].join(); });
p(function () { var log = []; var d = new DataView(new ArrayBuffer(4)); d.setFloat16({ valueOf: function () { log.push("offset"); return 0; } }, { valueOf: function () { log.push("value"); return 2; } }, { valueOf: function () { log.push("endian"); return 0; } }); return log.join(" ") + " " + d.getFloat16(0, true); });
p(function () { var d = new DataView(new ArrayBuffer(4)); return d.setFloat16(100, { valueOf: function () { throw new Boom(); } }); });
p(function () { var b = new ArrayBuffer(4), d = new DataView(b); return d.setFloat16(0, { valueOf: function () { b.transfer(); return 1; } }); });
p(function () { var d = new DataView(new ArrayBuffer(4)); return d.getFloat16({ valueOf: function () { throw new Boom(); } }); });

// --- F03: the constructor and its prototype
p(function () { return [typeof Float16Array, Float16Array.name, Float16Array.length, Float16Array.BYTES_PER_ELEMENT, Float16Array.prototype.BYTES_PER_ELEMENT].join(); });
p(function () { return [Object.getPrototypeOf(Float16Array) === Object.getPrototypeOf(Int8Array), Object.getPrototypeOf(Float16Array.prototype) === Object.getPrototypeOf(Int8Array.prototype), Float16Array.prototype.constructor === Float16Array, Object.prototype.toString.call(new Float16Array(1))].join(); });
p(function () { return Float16Array(2); });
p(function () { var d = Object.getOwnPropertyDescriptor(Float16Array, "BYTES_PER_ELEMENT"); return [d.writable, d.enumerable, d.configurable, Object.getOwnPropertyNames(Float16Array.prototype).join("|")].join(); });
// --- F03: construction from a length, a buffer, a typed array, an array-like and an iterable
p(function () { var a = new Float16Array(3); return a.length + " " + a.byteLength + " " + a.join(); });
p(function () { var a = new Float16Array([1.1, 65520, -0, NaN, 1e-8, 65504, "2.5"]); return a.join() + " " + is(a[2], -0); });
p(function () { var b = new ArrayBuffer(8); var a = new Float16Array(b, 2, 2); a[0] = 0.5; return a.length + " " + a.byteOffset + " " + new Uint8Array(b).join(); });
p(function () { return [t(function () { return new Float16Array(new ArrayBuffer(8), 1); }), t(function () { return new Float16Array(new ArrayBuffer(5)); }), t(function () { return new Float16Array(new ArrayBuffer(8), 2, 4); }), t(function () { return new Float16Array(-1); })].join(); });
p(function () { return new Float16Array(new Float64Array([1 + Math.pow(2, -11) + Math.pow(2, -40), 70000, 1e-9])).join() + " " + new Uint8Array(new Float16Array([1.7, -1, 300])).join(); });
p(function () { return new Float16Array({ length: 3, 0: 1.5, 1: "2", 2: { valueOf: function () { return 0.1; } } }).join(); });
p(function () { return new Float16Array(new Set([0.1, 0.2, 3])).join(); });
p(function () { function* g() { yield 1; yield 2.0009765625; yield 65536; } return new Float16Array(g()).join(); });
p(function () { var o = { length: 9, 0: 7 }; o[Symbol.iterator] = function* () { yield 4; yield 5; }; return new Float16Array(o).join(); });
p(function () { var o = { length: 1, 0: 7 }; o[Symbol.iterator] = null; return new Float16Array(o).join(); });
p(function () { var o = { length: 1, 0: 7 }; o[Symbol.iterator] = 1; return new Float16Array(o); });
p(function () { var reads = 0, o = {}; Object.defineProperty(o, Symbol.iterator, { get: function () { reads++; return function () { return [1, 2][Symbol.iterator](); }; } }); var a = new Float16Array(o); return reads + " " + a.join(); });
p(function () { var o = {}; o[Symbol.iterator] = function () { return { next: function () { throw new Boom(); } }; }; return new Float16Array(o); });
p(function () { return new Float16Array([{ valueOf: function () { throw new Boom(); } }]); });
// --- the iterable argument is the constructor's, so every kind takes one
p(function () { return [Int8Array, Uint8Array, Uint8ClampedArray, Int16Array, Uint16Array, Int32Array, Uint32Array, Float32Array, Float64Array].map(function (C) { return new C(new Set([1, 300, -1.5])).join(":"); }).join(" "); });
p(function () { function* g() { yield 1; yield 2; } return new Uint8Array(g()).join() + " " + new Float64Array(new Map([[1, 2]]).keys()).join(); });
p(function () { var o = { length: 2, 0: 1, 1: 2 }; o[Symbol.iterator] = function () { var i = 0; return { next: function () { return i < 3 ? { value: 10 + i++, done: false } : { done: true }; } }; }; return new Int16Array(o).join(); });
// --- F03: indexed reads and writes, and the views that share the bytes
p(function () { var a = new Float16Array(2); a[0] = 1 / 3; a[1] = -65520; a[5] = 1; return a[0] + " " + a[1] + " " + a[5] + " " + a.length + " " + new Uint16Array(a.buffer).join(); });
p(function () { var a = new Float16Array(4); var s = a.subarray(1, 3); s[0] = 2.5; return a.join() + " " + s.byteOffset + " " + (s.buffer === a.buffer) + " " + (s instanceof Float16Array); });
p(function () { var a = new Float16Array([1, 2, 3]); var r = a.slice(1); r[0] = 9; return a.join() + " " + r.join() + " " + (r instanceof Float16Array); });
// --- F03: the generic %TypedArray% methods, over half-precision elements
p(function () { var a = new Float16Array([3, NaN, -0, 0, -1, 65504, 0.1]); a.sort(); return a.join() + " " + is(a[1], -0); });
p(function () { var a = new Float16Array([1, 2, 3, 4]); a.sort(function (x, y) { return y - x; }); return a.join() + " " + new Float16Array([0.5, 0.25]).toSorted().join(); });
p(function () { var a = new Float16Array([1, 2]); return a.map(function (x) { return x / 3; }).join() + " " + a.filter(function (x) { return x > 1; }).join() + " " + a.map(function (x) { return x; }).constructor.name; });
p(function () { var a = new Float16Array([0.1, NaN, 2]); return [a.includes(NaN), a.indexOf(NaN), a.indexOf(0.1), a.indexOf(Math.f16round(0.1)), a.at(-1), a.with(0, 1 / 3)[0], a.toReversed().join()].join(); });
p(function () { var a = new Float16Array(3); a.fill(0.1); return a.join() + " " + a.reduce(function (s, x) { return s + x; }, 0); });
p(function () { var a = new Float16Array(4); a.set(new Float64Array([1.1, 2.2]), 1); a.set([3.3], 0); a.copyWithin(3, 0, 1); return a.join(); });
p(function () { var a = new Float16Array([1.5, 2.5]); return [a.every(function (x) { return x > 1; }), a.some(function (x) { return x > 2; }), a.find(function (x) { return x > 2; }), a.findIndex(function (x) { return x > 2; }), a.findLast(function () { return true; }), a.findLastIndex(function () { return false; })].join(); });
p(function () { var a = new Float16Array([1.5, 0.1]); return [a.join("|"), String(a), a.reverse().join(), Array.from(a.keys()).join(), JSON.stringify(Array.from(a.entries()))].join(" "); });
p(function () { var a = new Float16Array([0.1, 0.2]); var out = []; for (var x of a) out.push(x); return out.join() + " " + [...a.values()].join() + " " + (a[Symbol.iterator] === a.values); });
p(function () { return Float16Array.from([1, 2], function (x) { return x / 3; }).join() + " " + Float16Array.of(0.1, 70000).join() + " " + Float16Array.from(new Set([0.3])).join(); });
// --- F03: species, into and out of the new kind
p(function () { class Mine extends Float16Array {} var r = new Mine([1.1, 2]).map(function (x) { return x * 2; }); return (r instanceof Mine) + " " + r.join(); });
p(function () { var a = new Float16Array([1.1, 2]); var c = {}; c[Symbol.species] = Float64Array; a.constructor = c; return a.slice().constructor.name + " " + a.slice().join(); });
p(function () { var a = new Float64Array([1.1, 70000]); var c = {}; c[Symbol.species] = Float16Array; a.constructor = c; return a.map(function (x) { return x; }).join() + " " + a.slice().join() + " " + a.filter(function () { return true; }).join(); });
// --- F03: detachment
p(function () { var a = new Float16Array([1, 2]); a.buffer.transfer(); return [a.length, a.byteLength, a.byteOffset, a[0], t(function () { return a.join(); }), t(function () { return new Float16Array(a); })].join(); });
p(function () { var a = new Float16Array([3, 1, 2]), once = true; a.sort(function (x, y) { if (once) { once = false; a.buffer.transfer(); } return x - y; }); return a.length + " " + a[0]; });
// --- an array-like's length is read by ToLength, so an impossible length is a RangeError and not a wrap
p(function () { return [t(function () { return new Float16Array({ length: Math.pow(2, 53) }); }), t(function () { return new Int8Array({ length: Infinity, 0: 5 }); }), new Uint8Array({ length: -5 }).length, new Float16Array({ length: 2.7, 0: 1.5, 1: 2 }).join(), new Float16Array({ length: "2", 0: 0.1 }).join()].join(" "); });
// --- a plain Array argument: the drain that stands in for Array.prototype.values keeps every observable step
p(function () { Array.prototype[1] = 7; try { return new Float16Array([1, , 3]).join(); } finally { delete Array.prototype[1]; } });
p(function () { var b = [1, 2, 3, 4]; Object.defineProperty(b, 0, { get: function () { b.length = 2; return 9; }, configurable: true }); var a = [1, 2]; Object.defineProperty(a, 1, { get: function () { if (a.length < 4) a.push(a.length + 1); return 2; } }); return new Float16Array(b).join() + " " + new Int16Array(a).join(); });
p(function () { var log = [], c = [{ valueOf: function () { log.push("v0"); return 1; } }, 2]; Object.defineProperty(c, 1, { get: function () { log.push("g1"); return { valueOf: function () { log.push("v1"); return 2; } }; } }); return new Float64Array(c).join() + " " + log.join(" "); });
p(function () { var d = [1, 2, 3]; d[Symbol.iterator] = function* () { yield 10; yield 11; }; var saved = Array.prototype[Symbol.iterator]; Array.prototype[Symbol.iterator] = function* () { yield 42; }; try { return new Uint8Array(d).join() + " " + new Float16Array([1, 2, 3]).join(); } finally { Array.prototype[Symbol.iterator] = saved; } });
p(function () { var n = 0, e = [5, 6]; Object.defineProperty(e, Symbol.iterator, { get: function () { n++; return Array.prototype.values; } }); class Mine extends Array {} return new Int8Array(e).join() + " " + n + " " + new Float16Array(Mine.from([1.5, 2.5])).join(); });
p(function () { var f = [1]; Object.defineProperty(f, 0, { get: function () { throw new Boom(); } }); return new Float32Array(f); });
