// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE BUILT-IN DEFECTS THE WAVE-6 SLICES RECORDED.
//
// Retained for JSeal VM-FIX-I. `Array.of` builds its result through a constructor receiver,
// defines each element and sets `length` strictly (1-8), as `Array.from` already did (9). The
// reviver of `JSON.parse` removes a property through the holder's own [[Delete]], so a Proxy
// holder's `deleteProperty` trap runs, its false answer is ignored and its throw propagates
// (10-13). `Error.prototype`, the native error prototypes, `Date.prototype` and
// `RegExp.prototype` are ordinary objects (14-19). A typed array built from an Array follows a
// replaced `%ArrayIteratorPrototype%.next` (20-21), and `ArrayBuffer` reads `new.target`'s
// `prototype` before it allocates (22-24).
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return "throws " + (e && e.name ? e.name : e); } }
function p(f) { __n++; print(__n + " " + t(f)); }
var ts = Object.prototype.toString;

p(function () { function C(n) { this.n = n; } var r = Array.of.call(C, "a", "b"); return (r instanceof C) + "," + r.n + "," + r.length + "," + r[1]; });
p(function () { var got; function C() { got = arguments.length + ":" + arguments[0]; } Array.of.call(C); return got; });
p(function () { var r = Array.of.call({}, 1, 2); return Array.isArray(r) + "," + r.length; });
p(function () { var r = Array.of.call(undefined, 3); return Array.isArray(r) + "," + r[0]; });
p(function () { function C() { Object.defineProperty(this, "0", { value: 0, writable: true, configurable: false }); } return Array.of.call(C, 1).length; });
p(function () { function C() { Object.defineProperty(this, "length", { value: 0, writable: false }); } return Array.of.call(C, 1); });
p(function () { class A extends Array {} var r = A.of(1, 2, 3); return (r instanceof A) + "," + r.length + "," + r.join("-"); });
p(function () { var log = []; function C() { return new Proxy({}, { defineProperty: function (t, k, d) { log.push(k); return Reflect.defineProperty(t, k, d); }, set: function (t, k, v, r) { log.push("set " + k + "=" + v); return Reflect.set(t, k, v, r); } }); } Array.of.call(C, "x", "y"); return log.join(","); });
p(function () { function C(n) { this.n = n; } var r = Array.from.call(C, { length: 2, 0: "a", 1: "b" }); return (r instanceof C) + "," + r.n + "," + r.length; });

p(function () { var log = []; var r = JSON.parse('[0, 1]', function (k, v) { if (k === "0") { this[1] = new Proxy({ a: 1, b: 2 }, { deleteProperty: function (t, k) { log.push("delete " + k); return false; } }); return v; } if (k === "a") { return undefined; } return v; }); return log.join(",") + ";" + JSON.stringify(r); });
p(function () { var log = []; var r = JSON.parse('[0, 1]', function (k, v) { if (k === "0") { this[1] = new Proxy({ a: 1, b: 2 }, { deleteProperty: function (t, k) { log.push(k); return delete t[k]; } }); return v; } if (k === "b") { return undefined; } return v; }); return log.join(",") + ";" + JSON.stringify(r); });
p(function () { return JSON.parse('[0, 1]', function (k, v) { if (k === "0") { this[1] = new Proxy({ a: 1 }, { deleteProperty: function () { throw new RangeError("trap"); } }); return v; } if (k === "a") { return undefined; } return v; }); });
p(function () { var r = JSON.parse('[0, 1]', function (k, v) { if (k === "0") { this[1] = Object.freeze({ a: 1 }); return v; } if (k === "a") { return undefined; } return v; }); return JSON.stringify(r); });

p(function () { return ts.call(Error.prototype) + "," + ts.call(TypeError.prototype) + "," + ts.call(AggregateError.prototype); });
p(function () { return ts.call(Date.prototype) + "," + ts.call(RegExp.prototype); });
p(function () { return Date.prototype.getTime(); });
p(function () { return ts.call(new Error("e")) + "," + ts.call(new RangeError("e")) + "," + ts.call(new Date(0)) + "," + ts.call(/x/); });
p(function () { return Error.prototype.toString() + "," + TypeError.prototype.toString() + "," + RegExp.prototype.toString() + "," + RegExp.prototype.source + "," + RegExp.prototype.flags; });
p(function () { return [EvalError, ReferenceError, SyntaxError, URIError].map(function (C) { return ts.call(C.prototype); }).join(","); });

p(function () { var proto = Object.getPrototypeOf([].values()); var next = proto.next; var values = [1, 2, 3, 4]; proto.next = function () { var done = values.length === 0; return { value: values.pop(), done: done }; }; try { return Array.prototype.join.call(new Float64Array([0]), ","); } finally { proto.next = next; } });
p(function () { return new Uint8Array([1, 2, 3]).join(","); });
p(function () { function D() {} var nt = function () {}.bind(null); Object.defineProperty(nt, "prototype", { get: function () { throw new D(); } }); try { Reflect.construct(ArrayBuffer, [7 * 1125899906842624], nt); return "no throw"; } catch (e) { return e instanceof D ? "D" : e.name; } });
p(function () { function N() {} N.prototype = Object.create(ArrayBuffer.prototype); var b = Reflect.construct(ArrayBuffer, [4], N); return (Object.getPrototypeOf(b) === N.prototype) + "," + b.byteLength; });
p(function () { var nt = function () {}.bind(null); Object.defineProperty(nt, "prototype", { get: function () { throw new Error("read"); } }); try { Reflect.construct(ArrayBuffer, [8, { maxByteLength: 4 }], nt); return "no throw"; } catch (e) { return e.name; } });
