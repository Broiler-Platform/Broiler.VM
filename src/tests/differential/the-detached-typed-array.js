// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER A TYPED ARRAY WHOSE BUFFER HAS BEEN DETACHED: the integer-indexed
// exotic object's [[GetOwnProperty]], [[DefineOwnProperty]], [[Set]], [[Delete]] and
// [[OwnPropertyKeys]] empty only the NUMERIC keys, and every other string key is an ordinary
// property that keeps answering (ES2026 TypedArray exotic objects, CanonicalNumericIndexString) -
// and `fill`, `copyWithin` and the constructor, whose argument conversions can detach the buffer,
// check it again afterwards (%TypedArray%.prototype.fill and .copyWithin,
// InitializeTypedArrayFromArrayBuffer). A numeric key that is not an index ("-0", "1.5") is never
// read off the prototype chain (nor past a typed array met part-way up one), never reaches an
// inherited setter and cannot be defined; and the DataView and typed array constructors read
// `new.target.prototype` before their last detach check (OrdinaryCreateFromConstructor,
// AllocateTypedArray).
//
// Retained from the JSeal follow-up fix VM-FIX-A: every case was checked against those steps and
// compared against the comparison engine before it was written down. Detachment is reached
// through `ArrayBuffer.prototype.transfer`, the one way the language itself offers.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function detached(C) { var a = new (C || Int8Array)(4); a.buffer.transfer(); return a; }

// --- an ordinary key written after the detach is stored and read back
p(function () { var a = detached(); a.foo = 1; return a.foo; });
p(function () { var a = detached(); a.foo = 1; return Object.prototype.hasOwnProperty.call(a, "foo"); });
p(function () { "use strict"; var a = detached(); a.foo = 2; return a.foo; });
// --- a write that shadows a prototype member is an own property, not a write through to it
p(function () { function X() {} var a = detached(); a.constructor = X; return (a.constructor === X) + " " + (Int8Array.prototype.constructor === Int8Array); });
// --- an ordinary key written BEFORE the detach survives it
p(function () { var a = new Uint8Array(2); a.tag = "kept"; a.buffer.transfer(); return a.tag; });
// --- defineProperty on an ordinary key succeeds and reports its descriptor
p(function () { var a = detached(); Object.defineProperty(a, "bar", { value: 3, writable: false }); var d = Object.getOwnPropertyDescriptor(a, "bar"); return d.value + " " + d.writable + " " + d.enumerable; });
p(function () { var a = detached(); return Reflect.defineProperty(a, "baz", { value: 4, configurable: true }) + " " + a.baz; });
// --- delete and in on an ordinary key
p(function () { var a = detached(); a.gone = 1; return delete a.gone && !("gone" in a); });
p(function () { var a = detached(); a.here = 1; return "here" in a; });
// --- the keys: the indices are gone and the ordinary keys are listed
p(function () { var a = new Int16Array(3); a.x = 1; a.buffer.transfer(); a.y = 2; return Reflect.ownKeys(a).join(","); });
p(function () { var a = new Int16Array(3); a.x = 1; a.buffer.transfer(); return Object.keys(a).join(","); });
p(function () { var a = detached(); a.s = 1; var seen = []; for (var k in a) { seen.push(k); } return seen.join(","); });
// --- the numeric keys stay empty and writes to them are still discarded
p(function () { var a = detached(); a[0] = 5; return a[0] + " " + a.hasOwnProperty(0) + " " + a.length; });
p(function () { var a = detached(); return Reflect.set(a, 0, 5) + " " + a[0]; });
p(function () { var a = detached(); return Object.getOwnPropertyDescriptor(a, "0") === undefined; });
// --- a symbol key is ordinary too
p(function () { var s = Symbol("s"); var a = detached(Float64Array); a[s] = 7; return a[s]; });
// --- preventing extensions still leaves the ordinary keys it already had writable
p(function () { var a = detached(); a.k = 1; Object.preventExtensions(a); a.k = 2; a.m = 3; return a.k + " " + a.m + " " + Object.isExtensible(a); });
// --- fill and copyWithin revalidate the receiver after converting their arguments
p(function () { var a = new Int8Array(4); a.fill({ valueOf: function () { a.buffer.transfer(); return 1; } }); return "no throw"; });
p(function () { var a = new Int8Array(4); a.fill(1, 0, { valueOf: function () { a.buffer.transfer(); return 4; } }); return "no throw"; });
p(function () { var a = new Int8Array(4); a.copyWithin(0, { valueOf: function () { a.buffer.transfer(); return 1; } }); return "no throw"; });
p(function () { var a = new Int8Array(4); a.copyWithin(0, 4, { valueOf: function () { a.buffer.transfer(); return 4; } }); return "no throw, nothing to move"; });
// --- a numeric key that is not an index is never an ordinary property, detached or not;
//     "01" and "1.0" are not numeric and are
p(function () { var a = detached(); a["1.1"] = 1; a["-0"] = 1; a["-1"] = 1; return a["1.1"] + " " + a["-0"] + " " + a["-1"]; });
p(function () { var a = new Int8Array(2); a["1.5"] = 1; a["-0"] = 1; a.Infinity = 1; a.NaN = 1; return String(a["1.5"]) + String(a["-0"]) + String(a.Infinity) + String(a.NaN) + " " + Object.keys(a).join(","); });
p(function () { var a = new Int8Array(2); a["01"] = 1; a["1.0"] = 2; return a["01"] + " " + a["1.0"] + " " + Object.keys(a).join(","); });
p(function () { var a = new Int8Array(2); return ("-0" in a) + " " + ("1.5" in a) + " " + delete a["-1"]; });
// --- the constructor converts its length before it asks whether the buffer is still there
p(function () { var b = new ArrayBuffer(8); new Int8Array(b, 0, { valueOf: function () { b.transfer(); return 1; } }); return "no throw"; });
// --- a numeric key that is not an index is never looked up on the prototype chain, never reaches
//     an inherited setter, is still converted when it is assigned, and cannot be defined
p(function () { Object.prototype["1.5"] = 9; try { var a = new Int8Array(2); return a["1.5"] + " " + ("1.5" in a) + " " + Reflect.has(a, "1.5"); } finally { delete Object.prototype["1.5"]; } });
p(function () { Object.prototype["-0"] = 9; try { var a = new Int8Array(2); return a["-0"] + " " + Reflect.get(a, "-0") + " " + ("-0" in a); } finally { delete Object.prototype["-0"]; } });
p(function () { var hit = 0; Object.defineProperty(Object.prototype, "-0", { set: function () { hit++; }, configurable: true }); try { var a = new Int8Array(2); a["-0"] = 1; Reflect.set(a, "-0", 1); return hit; } finally { delete Object.prototype["-0"]; } });
p(function () { var c = 0; var a = new Int8Array(2); a["1.5"] = { valueOf: function () { c++; return 1; } }; Reflect.set(a, "1.5", { valueOf: function () { c++; return 1; } }); return c; });
p(function () { var a = new Int8Array(2); return Reflect.defineProperty(a, "1.5", { value: 1 }) + " " + Reflect.defineProperty(a, "-0", { value: 1 }) + " " + Reflect.defineProperty(a, "2", { value: 1 }) + " " + Reflect.defineProperty(a, "1", { value: 1 }); });
p(function () { Object.defineProperty(new Int8Array(2), "-0", { value: 1 }); return "no throw"; });
p(function () { var a = detached(); return Reflect.defineProperty(a, "0", { value: 1 }); });
// --- new.target's prototype is read before the constructor's last detach check, and read once
p(function () { var b = new ArrayBuffer(8); var nt = function () {}.bind(); Object.defineProperty(nt, "prototype", { get: function () { b.transfer(); return Int8Array.prototype; } }); Reflect.construct(Int8Array, [b], nt); return "no throw"; });
p(function () { var b = new ArrayBuffer(8); var nt = function () {}.bind(); Object.defineProperty(nt, "prototype", { get: function () { b.transfer(); return DataView.prototype; } }); Reflect.construct(DataView, [b], nt); return "no throw"; });
p(function () { var reads = 0; var nt = function () {}.bind(); Object.defineProperty(nt, "prototype", { get: function () { reads++; return Object.prototype; } }); Reflect.construct(Int8Array, [4], nt); Reflect.construct(Int8Array, [[1, 2]], nt); Reflect.construct(Int8Array, [new ArrayBuffer(4)], nt); Reflect.construct(DataView, [new ArrayBuffer(4)], nt); return reads; });
p(function () { class S extends Uint8Array {} class D extends DataView {} var s = new S(2); return (Object.getPrototypeOf(s) === S.prototype) + " " + s.length + " " + (new D(new ArrayBuffer(2)) instanceof D); });
// --- a typed array part-way up a prototype chain answers a numeric key itself, and Reflect.set
//     with another receiver neither converts nor writes for a key that names no element
p(function () { var hit = 0; Object.defineProperty(Int8Array.prototype, "1.5", { get: function () { hit++; }, set: function () { hit++; }, configurable: true }); try { var ta = new Int8Array(1); var o = Object.create(ta); o["1.5"] = 1; o[1] = 1; return hit + " " + o["1.5"] + " " + ("1.5" in o) + " " + o.hasOwnProperty("1.5") + " " + o.hasOwnProperty(1); } finally { delete Int8Array.prototype["1.5"]; } });
p(function () { var ta = new Int8Array([5]); var o = Object.create(ta); o[0] = 7; return o.hasOwnProperty(0) + " " + o[0] + " " + ta[0]; });
p(function () { var c = 0; var v = { valueOf: function () { c++; return 3; } }; var ta = new Int8Array(1); var r = {}; return Reflect.set(ta, "1.5", v, r) + " " + Reflect.set(ta, 4, v, r) + " " + Reflect.set(ta, 0, v, r) + " " + c + " " + r.hasOwnProperty(4) + " " + (r[0] === v) + " " + ta[0]; });
// --- defining an element takes only a data descriptor an element can have, and converts its value
p(function () { var a = new Int8Array(2); return [Reflect.defineProperty(a, "0", { value: 1, configurable: false }), Reflect.defineProperty(a, "0", { value: 1, enumerable: false }), Reflect.defineProperty(a, "0", { get: function () {} }), Reflect.defineProperty(a, "0", { value: 1, writable: false }), Reflect.defineProperty(a, "0", { value: 2, writable: true, enumerable: true, configurable: true }), a[0]].join(","); });
p(function () { var a = new Int8Array(2); Object.defineProperty(a, "1", { value: { valueOf: function () { return 7; } } }); return a[1]; });
p(function () { var a = new Int8Array(2); return Reflect.defineProperty(a, "0", { value: { valueOf: function () { a.buffer.transfer(); return 7; } } }) + " " + a[0]; });
