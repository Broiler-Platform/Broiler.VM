// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER ArraySpeciesCreate AND Symbol.isConcatSpreadable.
//
// Retained from JSeal slices V07-V09: the allocating Array methods (map, filter, slice, splice,
// concat, flat, flatMap) build their result through the receiver's species, and concat spreads what
// IsConcatSpreadable says to spread. Every case was compared against the comparison engine and the
// specification before it was written down, and each prints its own number so a divergence names a
// case rather than a line.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// A species constructor that records how it was called and hands back an ordinary object.
function recorder(log) {
  function C(n) { log.push("new:" + arguments.length + ":" + n); this.tag = "C"; }
  var a = [1, 2, 3];
  a.constructor = {};
  a.constructor[Symbol.species] = C;
  return a;
}

// --- V07: map and filter through the species
p(function () { class A extends Array {} return new A(1, 2, 3).map(function (x) { return x; }) instanceof A; });
p(function () { class A extends Array {} return new A(1, 2, 3).filter(function (x) { return x > 1; }) instanceof A; });
p(function () { class A extends Array {} var r = new A(1, 2, 3).map(function (x) { return x * 2; }); return r.length + ":" + r.join(); });
p(function () { var log = []; var a = recorder(log); var r = a.map(function (x) { return x; }); return log.join() + "|" + r.tag + "|" + r[0] + r[1] + r[2] + "|" + r.length; });
p(function () { var log = []; var a = recorder(log); var r = a.filter(function (x) { return x !== 2; }); return log.join() + "|" + r[0] + r[1] + "|" + r.length; });
p(function () { var a = [1]; a.constructor = undefined; return Object.getPrototypeOf(a.map(function (x) { return x; })) === Array.prototype; });
p(function () { var a = [1]; a.constructor = {}; a.constructor[Symbol.species] = null; return Array.isArray(a.map(function (x) { return x; })); });
p(function () { var a = [1]; a.constructor = {}; a.constructor[Symbol.species] = undefined; return Array.isArray(a.filter(function () { return true; })); });
p(function () { var a = [1]; a.constructor = 0; return a.map(function (x) { return x; }); });
p(function () { var a = [1]; a.constructor = {}; a.constructor[Symbol.species] = function () {}.bind(); return typeof a.map(function (x) { return x; }); });
p(function () { var a = [1]; a.constructor = {}; a.constructor[Symbol.species] = Math.max; return a.map(function (x) { return x; }); });
p(function () { var a = [1]; a.constructor = {}; a.constructor[Symbol.species] = 1; return a.filter(function () { return true; }); });
p(function () { var calls = 0; var a = [1]; a.constructor = {}; Object.defineProperty(a.constructor, Symbol.species, { get: function () { calls++; throw new RangeError("s"); } }); var r = t(function () { return a.map(function (x) { return x; }); }); return r + ":" + calls; });
p(function () { var calls = 0; var a = [1]; Object.defineProperty(a, "constructor", { get: function () { calls++; throw new SyntaxError("c"); } }); var r = t(function () { return a.filter(function () { return true; }); }); return r + ":" + calls; });
p(function () { var o = { length: 2, 0: "a", 1: "b", constructor: function () { throw new Error("never"); } }; var r = Array.prototype.map.call(o, function (x) { return x + x; }); return Array.isArray(r) + ":" + r.join(); });
p(function () { var r = [1, , 3].map(function (x) { return x; }); return r.length + ":" + (1 in r) + ":" + r[2]; });
p(function () { var log = []; var a = [, 5]; a.constructor = {}; a.constructor[Symbol.species] = function (n) { log.push(n); }; var r = a.map(function (x) { return x; }); return log.join() + ":" + (0 in r) + ":" + r[1] + ":" + ("length" in r); });
p(function () { var a = [1, 2]; a.constructor = {}; a.constructor[Symbol.species] = function () { return Object.freeze({}); }; return a.map(function (x) { return x; }); });
p(function () { var a = [1]; a.constructor = {}; a.constructor[Symbol.species] = function () { var o = {}; Object.defineProperty(o, "0", { set: function () { throw new Error("setter"); }, configurable: true }); return o; }; var r = a.map(function (x) { return x + 1; }); return r[0] + ":" + JSON.stringify(Object.getOwnPropertyDescriptor(r, "0")); });
p(function () { var seen = []; var a = [1]; a.constructor = {}; a.constructor[Symbol.species] = function () { return new Proxy({}, { defineProperty: function (tgt, k, d) { seen.push(k + ":" + d.value + ":" + d.writable + d.enumerable + d.configurable); return Reflect.defineProperty(tgt, k, d); } }); }; a.map(function (x) { return x * 3; }); return seen.join(); });
p(function () { var C = function () { this.fromSpecies = true; }; var b = [1, 2]; b.constructor = {}; b.constructor[Symbol.species] = C; var viaProxy = Array.prototype.map.call(new Proxy(b, {}), function (x) { return x; }); return viaProxy.fromSpecies === true; });
p(function () { var r = Array.prototype.map.call("ab", function (c) { return c.toUpperCase(); }); return Array.isArray(r) + ":" + r.join(""); });

// --- V08: slice and splice
p(function () { class A extends Array {} return new A(1, 2, 3).slice(1) instanceof A; });
p(function () { class A extends Array {} return new A(1, 2, 3).splice(0, 1) instanceof A; });
p(function () { var log = []; var a = recorder(log); var r = a.slice(1, 3); return log.join() + "|" + r[0] + r[1] + "|" + r.length; });
p(function () { var log = []; var a = recorder(log); var r = a.splice(1, 1); return log.join() + "|" + r[0] + "|" + r.length + "|" + a.join(); });
p(function () { var log = []; var a = [1, , 3, 4]; a.constructor = {}; a.constructor[Symbol.species] = function (n) { log.push(n); }; var r = a.slice(0, 3); return log.join() + ":" + (1 in r) + ":" + r[2] + ":" + r.length; });
p(function () { var a = [1, 2, 3]; a.constructor = {}; a.constructor[Symbol.species] = function () { var o = {}; Object.defineProperty(o, "length", { set: function (v) { this.setTo = v; } }); return o; }; var r = a.splice(0, 2); return r.setTo + ":" + r[0] + r[1] + ":" + a.join(); });
p(function () { var a = [1, 2]; a.constructor = {}; a.constructor[Symbol.species] = function () { return Object.freeze({}); }; return t(function () { return a.splice(0, 1); }) + ":" + a.join(); });
p(function () { var a = [1, 2]; a.constructor = {}; a.constructor[Symbol.species] = parseInt; return a.slice(); });
p(function () { var r = Array.prototype.slice.call({ length: 3, 0: "x", 2: "z" }, 0); return Array.isArray(r) + ":" + r.length + ":" + (1 in r); });
p(function () { var order = []; var a = [1, 2, 3]; Object.defineProperty(a, "constructor", { get: function () { order.push("ctor"); return Array; } }); a.slice({ valueOf: function () { order.push("start"); return 0; } }, { valueOf: function () { order.push("end"); return 2; } }); return order.join(); });

// --- V08: concat, flat and flatMap
p(function () { class A extends Array {} return new A(1, 2).concat([3]) instanceof A; });
p(function () { class A extends Array {} return new A([1], [2]).flat() instanceof A; });
p(function () { class A extends Array {} return new A(1, 2).flatMap(function (x) { return [x, x]; }) instanceof A; });
p(function () { var log = []; var a = recorder(log); var r = a.concat(4, [5]); return log.join() + "|" + r[0] + r[3] + r[4] + "|" + r.length; });
p(function () { var log = []; var a = [[1], [2, [3]]]; a.constructor = {}; a.constructor[Symbol.species] = function (n) { log.push(n); }; var r = a.flat(); return log.join() + ":" + r[0] + r[1] + ":" + Array.isArray(r[2]) + ":" + ("length" in r); });
p(function () { var log = []; var a = [1, 2]; a.constructor = {}; a.constructor[Symbol.species] = function (n) { log.push(n); }; var r = a.flatMap(function (x) { return [x, x * 10]; }); return log.join() + ":" + r[0] + "," + r[1] + "," + r[2] + "," + r[3]; });
p(function () { var order = []; var px = new Proxy([[1]], { get: function (tgt, k, r) { if (k === "length" || k === "constructor") { order.push(k); } return Reflect.get(tgt, k, r); } }); Array.prototype.flat.call(px, { valueOf: function () { order.push("depth"); return 1; } }); return order.join(); });
p(function () { return [1, [2, [3, [4]]]].flat(Infinity).join(); });
p(function () { return [1, [2]].flat(-1).length; });
p(function () { var r = [[1, , 3]].flat(); return r.length + ":" + r.join(); });
p(function () { var a = [1]; a.constructor = {}; a.constructor[Symbol.species] = function () { return Object.freeze([]); }; return a.concat(2); });

// --- V08: the change-by-copy methods ignore the species
p(function () { class A extends Array {} var a = new A(3, 1, 2); return [a.toReversed(), a.toSorted(), a.toSpliced(0, 1), a.with(0, 9)].map(function (r) { return Object.getPrototypeOf(r) === Array.prototype; }).join(); });
p(function () { var touched = 0; var a = [1, 2]; Object.defineProperty(a, "constructor", { get: function () { touched++; return Array; } }); a.toReversed(); a.toSorted(); a.toSpliced(0, 0); a.with(0, 1); return touched; });

// --- V09: Symbol.isConcatSpreadable
p(function () { var r = [].concat({ length: 1, 0: "x", [Symbol.isConcatSpreadable]: true }); return r.length + ":" + r[0]; });
p(function () { var r = [].concat({ length: 2, 0: "x", 1: "y", [Symbol.isConcatSpreadable]: true }); return r.length + ":" + r.join(); });
p(function () { var inner = [1, 2]; inner[Symbol.isConcatSpreadable] = false; var r = [0].concat(inner); return r.length + ":" + (r[1] === inner); });
p(function () { var r = [].concat({ length: 3, 1: "b", [Symbol.isConcatSpreadable]: 1 }); return r.length + ":" + (0 in r) + ":" + r[1] + ":" + (2 in r); });
p(function () { var r = [].concat({ length: 3, [Symbol.isConcatSpreadable]: 0 }); return r.length + ":" + typeof r[0]; });
p(function () { var o = { length: 1, 0: 1 }; Object.defineProperty(o, Symbol.isConcatSpreadable, { get: function () { throw new EvalError("g"); } }); return [].concat(o); });
p(function () { var order = []; function mk(name) { var o = {}; Object.defineProperty(o, Symbol.isConcatSpreadable, { get: function () { order.push(name); return false; } }); return o; } var a = []; Object.defineProperty(a, Symbol.isConcatSpreadable, { get: function () { order.push("this"); return undefined; } }); a.concat(mk("x"), mk("y")); return order.join(); });
p(function () { var proto = { [Symbol.isConcatSpreadable]: true }; var o = Object.create(proto); o.length = 2; o[0] = "p"; o[1] = "q"; return [].concat(o).join(); });
p(function () { var seen = []; var px = new Proxy({ length: 2, 0: "u", 1: "v" }, { get: function (tgt, k) { seen.push(typeof k === "symbol" ? k.description : k); return k === Symbol.isConcatSpreadable ? true : tgt[k]; } }); var r = [].concat(px); return r.join() + "|" + seen.join(); });
p(function () { var o = { length: 2, 0: 1, 1: 2, [Symbol.isConcatSpreadable]: true }; var r = Array.prototype.concat.call(o, 3); return r.length + ":" + r.join(); });
p(function () { String.prototype[Symbol.isConcatSpreadable] = true; try { var r = [].concat("ab"); return r.length + ":" + r[0]; } finally { delete String.prototype[Symbol.isConcatSpreadable]; } });
p(function () { var f = function (a, b) {}; f[Symbol.isConcatSpreadable] = true; f[0] = "f0"; var r = [].concat(f); return r.length + ":" + r[0] + ":" + (1 in r); });
p(function () { return [].concat({ length: -4294967294, 1: "A", [Symbol.isConcatSpreadable]: true }).length; });
p(function () { var o = { length: { valueOf: function () { throw new URIError("len"); } }, [Symbol.isConcatSpreadable]: true }; return [].concat(o); });
p(function () { return [].concat(new Proxy([1, , 3], {})).length + ":" + (1 in [].concat(new Proxy([1, , 3], {}))); });
p(function () { return [].concat({ length: 2, 0: 1 }).length; });
