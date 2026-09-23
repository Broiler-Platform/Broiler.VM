// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE OBJECT-INTEGRITY PATHS UNDER THE LIBRARY.
//
// Retained from JSeal slice VM-FIX-F: SetIntegrityLevel through [[DefineOwnProperty]], Array
// mutators throwing through Set/DeletePropertyOrThrow, typed-array canonical numeric keys and the
// typed-array [[Set]] receiver rule, the double coercion in ArraySetLength, the constructor of
// Array.from, the tag and array replacer of JSON, and iterator records whose done or value getter
// throws. Every case was compared against the comparison engine and the specification before it
// was written down.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// --- (1) SetIntegrityLevel goes through [[DefineOwnProperty]]
p(function () { Object.freeze(new Uint8Array(2)); return "frozen"; });
p(function () { return Object.isFrozen(Object.freeze(new Uint8Array(0))); });
p(function () { var ta = Object.seal(new Uint8Array(2)); ta[0] = 7; return Object.isSealed(ta) + "/" + ta[0]; });
p(function () { var ta = new Uint8Array(2); try { Object.freeze(ta); } catch (e) {} return Object.isExtensible(ta); });
p(function () { var o = Object.freeze([1, 2]); return Object.isFrozen(o) + "/" + Object.getOwnPropertyDescriptor(o, "length").writable; });

// --- (2) Array mutators use Set/DeletePropertyOrThrow
p(function () { var a = Object.freeze([1]); a.push(2); return a.length; });
p(function () { var a = Object.freeze([1]); a.unshift(0); return a.length; });
p(function () { var a = Object.freeze([1]); a.pop(); return a.length; });
p(function () { var a = Object.freeze([1, 2]); a.shift(); return a.length; });
p(function () { var a = Object.freeze([1, 2]); a.splice(0, 1); return a.length; });
p(function () { var a = Object.freeze([1, 2]); a.reverse(); return a.join(); });
p(function () { var a = Object.freeze([1, 2]); a.fill(0); return a.join(); });
p(function () { var a = Object.freeze([2, 1]); a.sort(); return a.join(); });
p(function () { var a = Object.freeze([1, 2]); a.copyWithin(0, 1); return a.join(); });
p(function () { var a = [1]; Object.defineProperty(a, "length", {writable: false}); a.push(2); return a.length; });
p(function () { var a = [1]; Object.defineProperty(a, "length", {writable: false}); try { a.push(2); } catch (e) {} return a.length + "/" + (1 in a); });
p(function () { var a = Object.seal([1, 2]); a.pop(); return a.length; });
p(function () { var a = Object.seal([1, 2]); try { a.pop(); } catch (e) {} return a.join(); });
p(function () { var o = {length: 2, 0: 1}; Object.defineProperty(o, 1, {value: 2, configurable: false}); Array.prototype.shift.call(o); return o.length; });
p(function () { var o = {length: 2}; Object.defineProperty(o, "length", {value: 2, writable: false}); Array.prototype.push.call(o); return o.length; });
p(function () { var o = {length: 1, 0: 1}; Object.freeze(o); Array.prototype.pop.call(o); return o.length; });
p(function () { var o = {length: 0}; Object.freeze(o); Array.prototype.pop.call(o); return o.length; });
p(function () { var a = [0, 1, 2]; Object.defineProperty(a, 0, {value: 0, writable: false}); a.copyWithin(0, 1); return a.join(); });
p(function () { var a = [0, 1]; Object.defineProperty(a, 1, {value: 1, configurable: false}); a.copyWithin(1, 5); return a.join(); });
p(function () { var o = {length: 2}; Object.defineProperty(o, 0, {value: 0, configurable: false}); Array.prototype.copyWithin.call(o, 0, 1); return o[0]; });
p(function () { var a = [1, , 3]; Object.defineProperty(a, 2, {value: 3, configurable: false}); a.reverse(); return a.join(); });
p(function () { var a = [3, 1]; Object.defineProperty(a, 0, {value: 3, writable: false}); a.sort(); return a.join(); });
p(function () { var a = []; var n = 0; Object.defineProperty(Array.prototype, "0", {set: function () { n++; Object.freeze(a); }, configurable: true}); try { a.push(1); return "none"; } catch (e) { return e.name + "/" + n + "/" + a.hasOwnProperty(0) + "/" + a.length; } finally { delete Array.prototype[0]; } });
p(function () { var a = []; Object.defineProperty(Array.prototype, "0", {value: 9, writable: false, configurable: true}); try { a.push(1); return "none"; } catch (e) { return e.name + "/" + a.length; } finally { delete Array.prototype[0]; } });
p(function () { return Array.prototype.copyWithin.call(true) instanceof Boolean; });

// --- (3) typed-array canonical numeric non-index keys
p(function () { var ta = new Uint8Array(1); ta["-1"] = 5; return ta["-1"] + "/" + ("-1" in ta) + "/" + Object.keys(ta).length; });
p(function () { var ta = new Uint8Array(1); ta["1.5"] = 5; return ta["1.5"] + "/" + ("1.5" in ta) + "/" + Object.keys(ta).length; });
p(function () { var ta = new Uint8Array(1); ta["-0"] = 5; return ta["-0"] + "/" + ("-0" in ta) + "/" + Object.keys(ta).length; });
p(function () { var ta = new Uint8Array(1); return Reflect.set(ta, "-0", 5) + "/" + Reflect.has(ta, "-0"); });
p(function () { var ta = new Uint8Array(1); return Reflect.set(ta, "Infinity", 5) + "/" + ta.Infinity; });
p(function () { Object.prototype["-1"] = "proto"; var ta = new Uint8Array(1); var v = ta["-1"]; delete Object.prototype["-1"]; return v; });
p(function () { var ta = new Uint8Array(1); var o = Object.create(ta); o["1.5"] = 3; return o["1.5"] + "/" + Object.keys(o).length; });
p(function () { var ta = new Uint8Array(1); return Reflect.defineProperty(ta, "-0", {value: 1}) + "/" + Object.getOwnPropertyNames(ta).length; });
p(function () { var ta = new Uint8Array(1); return delete ta["-0"]; });

// --- (4) typed-array [[Set]] receiver rule
p(function () { var ta = new Uint8Array(1); var r = {}; return Reflect.set(ta, 0, 9, r) + "/" + ta[0] + "/" + r[0]; });
p(function () { var ta = new Uint8Array(1); var r = {}; return Reflect.set(ta, 5, 9, r) + "/" + (5 in r); });
p(function () { var ta = new Uint8Array(1); var o = Object.create(ta); o[0] = 4; return ta[0] + "/" + Object.keys(o).join(); });
p(function () { var ta = new Uint8Array(1); var o = Object.create(ta); o[3] = 4; return (3 in o) + "/" + Object.keys(o).length; });
p(function () { var ta = new Uint8Array(1); return Reflect.set(ta, 0, 9, ta) + "/" + ta[0]; });
p(function () { var ta = new Uint8Array(1); var r = Object.freeze({}); return Reflect.set(ta, 0, 9, r); });

// --- (5) ArraySetLength coerces twice
p(function () { var n = 0; var a = []; a.length = {valueOf: function () { n++; return 2; }}; return n + "/" + a.length; });
p(function () { var n = 0; var a = []; Object.defineProperty(a, "length", {value: {valueOf: function () { n++; return 3; }}}); return n + "/" + a.length; });
p(function () { var n = 0; var a = []; return Reflect.set(a, "length", {valueOf: function () { n++; return 1; }}) + "/" + n; });
p(function () { var n = 0; var a = []; a.length = {valueOf: function () { return n++ === 0 ? 2 : 3; }}; return a.length; });
p(function () { "use strict"; var n = 0; var a = [1]; Object.defineProperty(a, "length", {writable: false}); try { a.length = {valueOf: function () { n++; return 0; }}; } catch (e) { return e.name + "/" + n; } return "none"; });
p(function () { var a = [1, 2]; Object.defineProperty(a, "length", {writable: false}); return Reflect.defineProperty(a, "length", {value: 2, writable: true}); });

// --- (7) Array.from honours its this-constructor and ToLength
p(function () { function C(n) { this.args = arguments.length + ":" + n; } var r = Array.from.call(C, {length: 2, 0: "a", 1: "b"}); return (r instanceof C) + "/" + r.args + "/" + r.length + "/" + r[1]; });
p(function () { function C() { this.k = 1; } var r = Array.from.call(C, [1, 2]); return (r instanceof C) + "/" + r.length + "/" + r.k; });
p(function () { var r = Array.from.call({}, {length: 1, 0: "x"}); return Array.isArray(r) + "/" + r[0]; });
p(function () { return Array.from({length: -3}).length; });
p(function () { return Array.from({length: 2.7, 0: 1, 1: 2}).join(); });
p(function () { return Array.from({length: "2", 0: 1, 1: 2}).join(); });
p(function () { function C() { return Object.freeze({}); } return Array.from.call(C, {length: 1, 0: 1}); });

// --- (8) %TypedArray%.prototype.set and %TypedArray%.from
p(function () { var ta = new Uint8Array(3); ta.set({length: -1, 0: 5}); return ta.join(); });
p(function () { var ta = new Uint8Array(3); ta.set({length: 1.9, 0: 5}); return ta.join(); });
p(function () { var ta = new Uint8Array(3); ta.set({length: "2", 0: 5, 1: 6}); return ta.join(); });
p(function () { return Uint8Array.from({length: 2.5, 0: 1, 1: 2}).join(); });
p(function () { return Uint8Array.from({length: -1}).length; });
p(function () { var n = 0; var r = Uint8Array.from([1, 2], function (v, k) { n += k; return v * 2; }); return r.join() + "/" + n; });
p(function () { return Uint8Array.from.call(function () {}, []); });
p(function () { return Uint8Array.from([], "x"); });
p(function () { var self = {}; var seen; Uint8Array.from([1], function () { seen = this; return 0; }, self); return seen === self; });
p(function () { return Uint8Array.from.call(Float64Array, [1.5]).constructor === Float64Array; });
p(function () { class U extends Uint8Array {} var r = U.from([1, 2]); return (r instanceof U) + "/" + r.join(); });
p(function () { return Uint8Array.from.call(function () { return new Uint8Array(0); }, [1]); });
p(function () { var ta = new Uint8Array(2); try { ta.set([1], {valueOf: function () { ta.buffer.transfer(); return 0; }}); return "none"; } catch (e) { return e.name + "/" + ta.length; } });

// --- (9) JSON[Symbol.toStringTag] and the array replacer
p(function () { return Object.prototype.toString.call(JSON); });
p(function () { var d = Object.getOwnPropertyDescriptor(JSON, Symbol.toStringTag); return d.value + d.writable + d.enumerable + d.configurable; });
p(function () { return JSON.stringify({a: 1, b: 2, c: 3}, new Proxy(["a", "c"], {})); });
p(function () { return JSON.stringify({a: 1, b: 2}, {length: 1, 0: "a"}); });
p(function () { var r = ["b"]; r.length = 1; return JSON.stringify({a: 1, b: 2}, r); });
p(function () { var log = []; var r = new Proxy(["a"], {get: function (t, k, rc) { log.push(String(k)); return Reflect.get(t, k, rc); }}); JSON.stringify({a: 1}, r); return log.join(); });
p(function () { return JSON.stringify({1: 1, a: 2}, [1, "a", 1]); });

// --- (10) an iterator whose done or value getter throws is not closed
p(function () { var closed = 0; var it = {}; it[Symbol.iterator] = function () { return {next: function () { return {get done() { throw new Error("d"); }}; }, return: function () { closed++; return {}; }}; }; try { for (var x of it) {} } catch (e) {} return closed; });
p(function () { var closed = 0; var it = {}; it[Symbol.iterator] = function () { return {next: function () { return {done: false, get value() { throw new Error("v"); }}; }, return: function () { closed++; return {}; }}; }; try { for (var x of it) {} } catch (e) {} return closed; });
p(function () { var closed = 0; var it = {}; it[Symbol.iterator] = function () { return {next: function () { return {done: false, get value() { throw new Error("v"); }}; }, return: function () { closed++; return {}; }}; }; try { var [a] = it; } catch (e) {} return closed; });
p(function () { var closed = 0; var it = {}; it[Symbol.iterator] = function () { return {next: function () { return {done: false, value: 1}; }, return: function () { closed++; return {}; }}; }; for (var x of it) { break; } return closed; });
p(function () { var closed = 0; var it = {}; it[Symbol.iterator] = function () { return {next: function () { return {get done() { throw new Error("d"); }}; }, return: function () { closed++; return {}; }}; }; try { Array.from(it); } catch (e) {} return closed; });
