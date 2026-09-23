// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER LengthOfArrayLike, CreateDataPropertyOrThrow, THE UNMAPPED `callee` AND
// THE RECEIVER HALF OF OrdinarySet.
//
// Retained from JSeal slice VM-FIX-C: the Array methods read `length` with ToLength rather than
// ToUint32 and refuse a result past 2^53-1; an allocating method defines its result elements through
// the result's own [[DefineOwnProperty]], so an exotic species result validates them; an unmapped
// `arguments` object carries the realm's one %ThrowTypeError% as a non-configurable `callee`; and a
// write that lands on an existing receiver property hands the receiver `{ value }` and nothing
// else. Every case was compared against the comparison engine and the specification before it was
// written down.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
var AP = Array.prototype;

// --- LengthOfArrayLike: a negative length is 0 and a length past 2^32 is kept
p(function () { return AP.indexOf.call({ length: -1, 0: 1 }, 1); });
p(function () { var o = { length: -5 }; var r = AP.push.call(o, "a"); return r + ":" + o.length + ":" + o[0]; });
p(function () { var o = { length: 4294967297 }; o[4294967296] = "x"; return AP.includes.call(o, "x", 4294967295); });
p(function () { var o = { length: 4294967298 }; o[4294967297] = "y"; return AP.at.call(o, -1); });
p(function () { var o = { length: 4294967296 }; var r = AP.push.call(o, "z"); return r + ":" + o[4294967296] + ":" + o.length; });
p(function () { var o = { length: 4294967297 }; AP.fill.call(o, 7, 4294967296); return o[4294967296]; });
p(function () { var o = { length: 9007199254740993, 9007199254740990: "q" }; return AP.lastIndexOf.call(o, "q"); });
p(function () { var o = { length: 9007199254740993, 9007199254740990: "p" }; var r = AP.pop.call(o); return r + ":" + o.length; });
p(function () { var o = { length: Infinity }; AP.pop.call(o); return o.length; });
p(function () { var o = { length: "4294967296" }; return AP.push.call(o) + ":" + o.length; });

// --- the 2^53-1 ceiling: refused before anything is written
p(function () { var o = { length: 9007199254740991 }; try { AP.push.call(o, 1); } catch (e) { return e.name + ":" + o.length + ":" + (9007199254740991 in o); } });
p(function () { var o = { length: 9007199254740990 }; return AP.push.call(o, "a") + ":" + o[9007199254740990]; });
p(function () { var o = { length: 9007199254740990 }; return AP.push.call(o, "a", "b"); });
p(function () { return AP.unshift.call({ length: 9007199254740991 }, 1); });
p(function () { return AP.unshift.call({ length: 9007199254740991 }); });
p(function () { var o = { length: 9007199254740991 }; try { AP.splice.call(o, 0, 0, 1); } catch (e) { return e.name + ":" + o.length; } });
p(function () { return AP.splice.call({ length: 9007199254740991 }, 9007199254740990, 1, "a", "b"); });
p(function () { var o = { length: 9007199254740991 }; try { AP.splice.call(o, 0); } catch (e) { return e.name + ":" + o.length; } });
p(function () { return AP.toSpliced.call({ length: 9007199254740991 }, 0, 0, 1); });
p(function () { return AP.toSpliced.call({ length: 4294967296 }, 0, 0); });
p(function () { return AP.toSpliced.call({ length: 4294967297 }, 0, 1).length; });
p(function () { return AP.toReversed.call({ length: 4294967296 }); });
p(function () { return AP.toSorted.call({ length: 4294967296 }); });
p(function () { return AP.with.call({ length: 4294967296 }, 0, 1); });
p(function () { return Array.from({ length: 4294967296 }); });
p(function () { var o = { length: 9007199254740991 }; o.constructor = null; return AP.map.call(o, function (x) { return x; }); });

// --- CreateDataPropertyOrThrow goes through the result's own [[DefineOwnProperty]]
function speciesOf(make) { var a = [1, 2, 3]; a.constructor = {}; a.constructor[Symbol.species] = make; return a; }
p(function () { return speciesOf(function () { return new Uint8Array(1); }).map(function (x) { return x; }); });
p(function () { return speciesOf(function () { return new Uint8Array(1); }).filter(function (x) { return true; }); });
p(function () { return speciesOf(function () { return new Uint8Array(1); }).slice(0); });
p(function () { var r = speciesOf(function () { return new Uint8Array(3); }).map(function (x) { return x * 100; }); return r.constructor.name + ":" + r.join(); });
p(function () { var r = speciesOf(function () { return new Uint8Array(3); }).map(function (x) { return x * 100; }); return r[2]; });
p(function () { return speciesOf(function () { return Object.freeze(new Uint8Array(0)); }).map(function (x) { return x; }); });
p(function () { return speciesOf(function () { return new String("ab"); }).map(function (x) { return x; }); });
p(function () { return speciesOf(function () { var r = []; Object.defineProperty(r, "length", { writable: false }); return r; }).map(function (x) { return x; }); });
p(function () { var r = speciesOf(function () { return {}; }).map(function (x) { return x; }); var d = Object.getOwnPropertyDescriptor(r, "1"); return d.value + ":" + d.writable + d.enumerable + d.configurable; });
p(function () { return Object.defineProperty(new Uint8Array(2), "5", { value: 1 }); });
p(function () { return Reflect.defineProperty(new Uint8Array(2), "0", { value: 1, writable: false }); });
p(function () { return Reflect.defineProperty(new Uint8Array(2), "-0", { value: 1 }); });
p(function () { return Reflect.defineProperty(new Uint8Array(2), "1.5", { value: 1 }); });
p(function () { var ta = new Uint8Array(2); return Reflect.defineProperty(ta, "1", { value: 300 }) + ":" + ta[1]; });
p(function () { var ta = new Uint8Array(2); return Reflect.defineProperty(ta, "0", { get: function () { return 1; } }); });
p(function () { var ta = new Uint8Array(2); return Reflect.defineProperty(ta, "0", { value: 1, configurable: false }); });
p(function () { var a = []; Object.defineProperty(a, "length", { writable: false }); return Reflect.defineProperty(a, "0", { value: 1 }) + ":" + a.length; });
p(function () { var a = []; Object.defineProperty(a, "length", { writable: false }); Object.defineProperty(a, "0", { value: 1 }); return a.length; });
p(function () { var a = []; Object.defineProperty(a, "length", { writable: false }); Object.defineProperty(a, "4294967295", { value: 1 }); return a[4294967295] + ":" + a.length; });
p(function () { return Reflect.defineProperty([], "length", { value: -1 }); });

// --- the unmapped arguments object's `callee` is the realm's one %ThrowTypeError%
p(function () { "use strict"; var d = Object.getOwnPropertyDescriptor(arguments, "callee"); return d.enumerable + ":" + d.configurable + ":" + typeof d.get + ":" + (d.get === d.set); });
p(function () { "use strict"; var d = Object.getOwnPropertyDescriptor(arguments, "callee"); return d.get === Object.getOwnPropertyDescriptor(Function.prototype, "caller").get; });
p(function () { "use strict"; var d = Object.getOwnPropertyDescriptor(arguments, "callee"); return d.get === Object.getOwnPropertyDescriptor(Function.prototype, "arguments").set; });
p(function () { "use strict"; var a = (function () { return arguments; })(); var b = (function () { return arguments; })(); return Object.getOwnPropertyDescriptor(a, "callee").get === Object.getOwnPropertyDescriptor(b, "callee").get; });
p(function () { "use strict"; return arguments.callee; });
p(function () { "use strict"; arguments.callee = 1; return "wrote"; });
p(function () { "use strict"; return delete arguments.callee; });
p(function () { return (function (a = 1) { return typeof Object.getOwnPropertyDescriptor(arguments, "callee").get; })(); });
p(function () { return (function (a = 1) { return arguments.callee; })(); });
p(function () { return (function (...rest) { return delete arguments.callee; })(); });
p(function () { return (function ({ x }) { return Object.getOwnPropertyDescriptor(arguments, "callee").configurable; })({}); });
p(function () { function f() { return arguments.callee === f; } return f(); });
p(function () { "use strict"; var g = Object.getOwnPropertyDescriptor(arguments, "callee").get; return Object.isFrozen(g) + ":" + Object.isExtensible(g); });
p(function () { "use strict"; var g = Object.getOwnPropertyDescriptor(arguments, "callee").get; var d = Object.getOwnPropertyDescriptor(g, "length"); return d.value + ":" + d.writable + d.enumerable + d.configurable; });
p(function () { "use strict"; var g = Object.getOwnPropertyDescriptor(arguments, "callee").get; var d = Object.getOwnPropertyDescriptor(g, "name"); return JSON.stringify(d.value) + ":" + d.writable + d.enumerable + d.configurable; });
p(function () { "use strict"; return Object.getOwnPropertyDescriptor(arguments, "callee").get.call(); });

// --- OrdinarySet onto an existing receiver property: [[DefineOwnProperty]](P, { value }) only
function traced(target, log, answer) {
  return new Proxy(target, {
    defineProperty: function (t, k, d) { log.push(String(k) + ":" + Object.keys(d).join("/")); return answer === undefined ? Reflect.defineProperty(t, k, d) : answer; },
  });
}
p(function () { var log = []; var target = { x: 1 }; var r = traced(target, log); var ok = Reflect.set({}, "x", 2, r); return ok + "|" + log.join() + "|" + target.x; });
p(function () { var log = []; var target = {}; var r = traced(target, log); var ok = Reflect.set({}, "y", 2, r); return ok + "|" + log.join() + "|" + target.y; });
p(function () { var s = Symbol("s"); var log = []; var target = {}; target[s] = 1; var r = traced(target, log); var ok = Reflect.set({}, s, 2, r); return ok + "|" + log.join() + "|" + target[s]; });
p(function () { var s = Symbol("s"); var log = []; var target = {}; var r = traced(target, log); var ok = Reflect.set({}, s, 2, r); return ok + "|" + log.join() + "|" + target[s]; });
p(function () { var log = []; return Reflect.set({}, "x", 2, traced({ x: 1 }, log, false)) + "|" + log.join(); });
p(function () { var s = Symbol("s"); var log = []; var target = {}; target[s] = 1; return Reflect.set({}, s, 2, traced(target, log, false)) + "|" + log.join(); });
p(function () { var log = []; var r = traced({ x: 1 }, log); r.x = 5; return log.join(); });
p(function () { var s = Symbol("s"); var log = []; var target = {}; target[s] = 1; var r = traced(target, log); r[s] = 5; return log.join() + "|" + target[s]; });
p(function () { var r = new Proxy({}, { defineProperty: function () { return false; } }); r.z = 1; return "z" in r; });
p(function () { "use strict"; var r = new Proxy({ x: 1 }, { defineProperty: function () { return false; } }); r.x = 2; return "wrote"; });
p(function () { var arr = [1, 2, 3]; var ok = Reflect.set({ length: 0 }, "length", 1, arr); return ok + ":" + arr.length + ":" + arr.join(); });
p(function () { var arr = [1, 2, 3]; return Reflect.set({}, "length", -1, arr); });
p(function () { var recv = {}; Object.defineProperty(recv, "x", { value: 1, writable: false, configurable: true }); return Reflect.set({}, "x", 2, recv) + ":" + recv.x; });
p(function () { var recv = { get x() { return 1; } }; return Reflect.set({}, "x", 2, recv); });
p(function () { var recv = {}; Object.defineProperty(recv, "x", { value: 1, writable: true, enumerable: false, configurable: false }); var ok = Reflect.set({}, "x", 2, recv); var d = Object.getOwnPropertyDescriptor(recv, "x"); return ok + ":" + d.value + d.enumerable + d.configurable; });
p(function () { var ta = new Uint8Array(2); return Reflect.set({}, "0", 300, ta) + ":" + ta[0]; });
p(function () { var ta = new Uint8Array(2); return Reflect.set({}, "5", 1, ta) + ":" + ta[5]; });
p(function () { var log = []; var o = new Proxy({ length: 2 }, { deleteProperty: function (t, k) { log.push(k); return Reflect.deleteProperty(t, k); } }); AP.reverse.call(o); return log.join() + "|"; });
p(function () { var log = []; var o = new Proxy({ 0: "a", length: 3 }, { deleteProperty: function (t, k) { log.push(k); return Reflect.deleteProperty(t, k); } }); AP.reverse.call(o); return log.join() + "|" + o[2]; });
