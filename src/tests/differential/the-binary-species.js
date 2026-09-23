// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE SPECIES PROTOCOL OF THE BINARY SURFACE: `%TypedArray%.prototype`
// `map`, `filter`, `slice` and `subarray`, and `ArrayBuffer.prototype.slice`.
//
// Retained from JSeal slices V10-V12: every case was checked against the specification's
// TypedArraySpeciesCreate, TypedArrayCreateFromConstructor and ArrayBuffer.prototype.slice steps
// and compared against the comparison engine before it was written down. Detachment is reached
// through `ArrayBuffer.prototype.transfer`, the one way the language itself offers.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function withSpecies(o, S) { var c = {}; c[Symbol.species] = S; o.constructor = c; return o; }
function Boom() { this.name = "Boom"; }

// --- map: the species is observed, and the default is the receiver's own kind
p(function () { class Mine extends Uint8Array {} var r = new Mine([1, 2, 3]).map(function (x) { return x * 2; }); return (r instanceof Mine) + " " + r.join(","); });
p(function () { var r = withSpecies(new Int8Array([1, 2]), Float64Array).map(function (x) { return x + 0.5; }); return r.constructor.name + " " + r.join(","); });
p(function () { var a = new Int8Array([1, 2]); a.constructor = undefined; var r = a.map(function (x) { return x; }); return Object.getPrototypeOf(r) === Int8Array.prototype; });
p(function () { var a = new Int8Array([1]); a.constructor = 1; return a.map(function (x) { return x; }); });
p(function () { var r = withSpecies(new Int8Array([7]), null).map(function (x) { return x; }); return Object.getPrototypeOf(r) === Int8Array.prototype; });
p(function () { return withSpecies(new Int8Array([1]), {}).map(function (x) { return x; }); });
p(function () { return withSpecies(new Int8Array([1]), function () { return {}; }).map(function (x) { return x; }); });
p(function () { return withSpecies(new Int8Array([1, 2, 3]), function (n) { return new Int8Array(n - 1); }).map(function (x) { return x; }); });
p(function () { var r = withSpecies(new Int8Array([1, 2]), function (n) { return new Int8Array(n + 3); }).map(function (x) { return x + 1; }); return r.join(","); });
p(function () { var a = new Int8Array([1]); Object.defineProperty(a, "constructor", { get: function () { throw new Boom(); } }); return a.map(function (x) { return x; }); });
p(function () { var a = new Int8Array([1]); var c = {}; Object.defineProperty(c, Symbol.species, { get: function () { throw new Boom(); } }); a.constructor = c; return a.map(function (x) { return x; }); });

// --- map: the order of the observable steps, and the conversion into the result's kind
p(function () { var log = []; var a = withSpecies(new Int8Array([1, 2]), function (n) { log.push("ctor(" + n + ")"); return new Int8Array(n); }); a.map(function (x) { log.push("cb" + x); return x; }); return log.join(" "); });
p(function () { var r = withSpecies(new Int8Array([1, 2]), Float64Array).map(function (x) { return x === 1 ? "3" : { valueOf: function () { return 4.25; } }; }); return r.join(","); });
p(function () { var r = withSpecies(new Uint8Array([1, 2]), Uint8ClampedArray).map(function (x) { return x === 1 ? 300 : -5; }); return r.constructor.name + " " + r.join(","); });
p(function () { var r = withSpecies(new Uint8Array([1]), Uint8Array).map(function () { return 300; }); return r.join(","); });
p(function () { var r = new Float64Array([1, 2, 3]).map(function (x, i, a) { if (i === 0) { a.buffer.transfer(); } return x; }); return r.join(","); });
p(function () { var log = []; var a = new Int8Array([1]); a.buffer.transfer(); Object.defineProperty(a, "constructor", { get: function () { log.push("ctor"); return Int8Array; } }); try { a.map(function (x) { return x; }); } catch (e) { log.push(e.name); } return log.join(" "); });
p(function () { return withSpecies(new Int8Array([1]), function (n) { var d = new Int8Array(n); d.buffer.transfer(); return d; }).map(function (x) { return x; }); });
p(function () { return withSpecies(new Int8Array([1]), function (n) { return new Proxy(new Int8Array(n), {}); }).map(function (x) { return x; }); });
p(function () { var args; withSpecies(new Int8Array([4, 5, 6]), function () { args = Array.prototype.slice.call(arguments); return new Int8Array(3); }).map(function (x) { return x; }); return args.length + " " + typeof args[0] + " " + args[0]; });

// --- filter: every callback runs before the species is read, which is handed the kept count
p(function () { var log = []; var a = new Int8Array([1, 2, 3]); Object.defineProperty(a, "constructor", { get: function () { log.push("get"); return Int8Array; } }); a.filter(function (x) { log.push("cb" + x); return x !== 2; }); return log.join(" "); });
p(function () { var args; var r = withSpecies(new Int8Array([1, 2, 3, 4]), function () { args = Array.prototype.slice.call(arguments); return new Int16Array(args[0]); }).filter(function (x) { return x % 2 === 0; }); return args.join("|") + " " + r.constructor.name + " " + r.join(","); });
p(function () { class Mine extends Float32Array {} var r = new Mine([1.5, 2, 3.5]).filter(function (x) { return x !== 2; }); return (r instanceof Mine) + " " + r.join(","); });
p(function () { return withSpecies(new Int8Array([1, 2]), function (n) { return new Int8Array(n - 1); }).filter(function () { return true; }); });
p(function () { return withSpecies(new Int8Array([1, 2]), function () { return [0, 0]; }).filter(function () { return true; }); });
p(function () { var r = new Float64Array([1, 2, 3]).filter(function (x, i, a) { if (i === 0) { a.buffer.transfer(); } return true; }); return r.join(","); });
p(function () { var r = withSpecies(new Int8Array([-1, 5]), Uint8Array).filter(function () { return true; }); return r.join(","); });

// --- slice: species with the count, byte copy for the same kind, element conversion otherwise
p(function () { var args; withSpecies(new Int8Array([1, 2, 3, 4]), function () { args = Array.prototype.slice.call(arguments); return new Int8Array(args[0]); }).slice(1, 3); return args.length + " " + args[0]; });
p(function () { class Mine extends Int16Array {} var r = new Mine([1, 2, 3]).slice(1); return (r instanceof Mine) + " " + r.join(","); });
p(function () { var r = withSpecies(new Int8Array([-1, 2]), Uint8Array).slice(); return r.constructor.name + " " + r.join(","); });
p(function () { var r = withSpecies(new Float32Array([1.5, -2.75]), Int16Array).slice(); return r.join(","); });
p(function () { var r = withSpecies(new Uint8Array([200, 5]), Int8Array).slice(); return r.join(","); });
p(function () { var r = withSpecies(new Uint8Array([1, 2]), Uint8ClampedArray).slice(); return r.constructor.name + " " + r.join(","); });
p(function () { var a = new Float64Array([1, 2, 3]); var r = a.slice(1); r[0] = 9; return a.join(",") + " " + r.join(",") + " " + (r.buffer === a.buffer); });
p(function () { var ta = new Int16Array([10, 20, 30, 40, 50, 60]); withSpecies(ta, function () { return new Int16Array(ta.buffer, 4); }); var r = ta.slice(1, 4); return Array.prototype.join.call(r, ","); });
p(function () { return withSpecies(new Int8Array([1, 2, 3]), function (n) { return new Int8Array(n - 1); }).slice(); });
p(function () { var r = withSpecies(new Int8Array([1, 2]), function (n) { return new Int8Array(n + 2); }).slice(); return r.join(","); });
p(function () { var a = new Int8Array([1, 2]); return withSpecies(a, function (n) { a.buffer.transfer(); return new Int8Array(n); }).slice(); });
p(function () { var a = new Int8Array([1, 2]); var r = withSpecies(a, function (n) { a.buffer.transfer(); return new Int8Array(n); }).slice(1, 1); return r.length; });
p(function () { var a = new Int8Array([1, 2]); a.buffer.transfer(); return a.slice(); });
p(function () { var log = []; var a = new Int8Array([1, 2]); Object.defineProperty(a, "constructor", { get: function () { log.push("get"); return Int8Array; } }); a.slice({ valueOf: function () { log.push("start"); return 0; } }, { valueOf: function () { log.push("end"); return 1; } }); return log.join(" "); });
p(function () { return withSpecies(new Int8Array([1]), function () { return new Uint8Array(new ArrayBuffer(8), 8); }).slice(); });

// --- subarray: species with (buffer, byteOffset, length) over the SAME buffer
p(function () { var a = new Int16Array(8); var args; withSpecies(a, function () { args = Array.prototype.slice.call(arguments); return new Int16Array(args[0], args[1], args[2]); }).subarray(2, 5); return args.length + " " + (args[0] === a.buffer) + " " + args[1] + " " + args[2]; });
p(function () { var a = new Uint8Array([1, 2, 3, 4]); var s = a.subarray(1, 3); s[0] = 9; a[2] = 8; return a.join(",") + " " + s.join(",") + " " + (s.buffer === a.buffer); });
p(function () { class Mine extends Uint8Array {} var s = new Mine([1, 2, 3]).subarray(1); return (s instanceof Mine) + " " + s.join(","); });
p(function () { var s = new Uint16Array(new ArrayBuffer(16), 2, 4).subarray(1, 3); return s.byteOffset + " " + s.length; });
p(function () { return withSpecies(new Uint8Array(4), function () { return {}; }).subarray(1); });
p(function () { var r = withSpecies(new Uint8Array([1, 2, 3, 4]), function () { return new Uint8Array(1); }).subarray(0, 3); return r.length; });
p(function () { return withSpecies(new Uint8Array(4), Uint16Array).subarray(1, 3); });
p(function () { var r = withSpecies(new Uint8Array([1, 2, 3, 4]), Uint16Array).subarray(2, 3); return r.constructor.name + " " + r.length + " " + r.byteOffset; });
p(function () { var a = new Uint8Array(4); a.buffer.transfer(); return a.subarray(0); });
p(function () { var log = []; var a = new Uint8Array(4); Object.defineProperty(a, "constructor", { get: function () { log.push("get"); return Uint8Array; } }); a.subarray({ valueOf: function () { log.push("begin"); return 0; } }, { valueOf: function () { log.push("end"); return 1; } }); return log.join(" "); });
p(function () { var buf = new Uint8Array([104, 105, 33, 0]).buffer; return new Uint8Array(buf).subarray(0, 3).join(","); });

// --- ArrayBuffer.prototype.slice: species, validation of the result, then the copy
p(function () { class Mine extends ArrayBuffer {} var b = new Mine(4).slice(1); return (b instanceof Mine) + " " + b.byteLength; });
p(function () { var b = new ArrayBuffer(4); b.constructor = null; return b.slice(); });
p(function () { return withSpecies(new ArrayBuffer(4), {}).slice(); });
p(function () { return withSpecies(new ArrayBuffer(4), false).slice(); });
p(function () { return withSpecies(new ArrayBuffer(4), function () { return {}; }).slice(); });
p(function () { return withSpecies(new ArrayBuffer(4), function (n) { return new Uint8Array(n); }).slice(); });
p(function () { var b = new ArrayBuffer(4); return withSpecies(b, function () { return b; }).slice(); });
p(function () { return withSpecies(new ArrayBuffer(4), function (n) { return new ArrayBuffer(n - 1); }).slice(); });
p(function () { var src = new ArrayBuffer(4); new Uint8Array(src).set([1, 2, 3, 4]); var r = withSpecies(src, function (n) { return new ArrayBuffer(n + 2); }).slice(1, 3); return r.byteLength + " " + new Uint8Array(r).join(","); });
p(function () { var b = new ArrayBuffer(4); return withSpecies(b, function (n) { b.transfer(); return new ArrayBuffer(n); }).slice(); });
p(function () { return withSpecies(new ArrayBuffer(4), function (n) { var d = new ArrayBuffer(n); d.transfer(); return d; }).slice(); });
p(function () { var src = new ArrayBuffer(3); new Uint8Array(src).set([1, 2, 3]); var r = src.slice(); new Uint8Array(r)[0] = 9; return new Uint8Array(src).join(",") + " " + new Uint8Array(r).join(","); });
p(function () { var args; var r = withSpecies(new ArrayBuffer(8), function () { args = Array.prototype.slice.call(arguments); return new ArrayBuffer(args[0]); }).slice(2, -1); return args.length + " " + args[0] + " " + r.byteLength; });
p(function () { var log = []; var b = new ArrayBuffer(4); Object.defineProperty(b, "constructor", { get: function () { log.push("get"); return ArrayBuffer; } }); b.slice({ valueOf: function () { log.push("start"); return 0; } }, { valueOf: function () { log.push("end"); return 1; } }); return log.join(" "); });
p(function () { var b = new ArrayBuffer(4); Object.defineProperty(b, "constructor", { get: function () { throw new Boom(); } }); return b.slice(); });
p(function () { var kept; var src = new ArrayBuffer(4); new Uint8Array(src).set([1, 2, 3, 4]); var r = t(function () { return withSpecies(src, function () { kept = new ArrayBuffer(2); new Uint8Array(kept)[0] = 7; return kept; }).slice(); }); return r + " " + new Uint8Array(kept).join(","); });
p(function () { var r = withSpecies(new ArrayBuffer(4), null).slice(1); return Object.getPrototypeOf(r) === ArrayBuffer.prototype && r.byteLength; });
