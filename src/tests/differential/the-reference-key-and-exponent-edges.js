// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER FOUR SMALL REPAIRS FOUND DURING JSEAL WAVE 1 (VM-FIX-B).
//
// The exponentiation operator follows Number::exponentiate rather than IEEE 754 pow; a computed
// member reference converts its key with ToPropertyKey exactly once across the read and the write
// of a compound, logical or update assignment; a super property accepts a Symbol key; and
// Array.prototype.toLocaleString is an own method that invokes each element's toLocaleString.
// Each case prints its own number so a divergence names a case rather than a line.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function counted(name, log) {
  return { toString: function () { log.push("k"); return name; } };
}
function once(run) {
  var log = []; var o = { a: 0, z: null }; var v = run(o, log); return log.join("") + ":" + v + ":" + o.a;
}

// --- Number::exponentiate: the cases IEEE 754 pow answers differently (1-12)
p(function () { return (-1) ** Infinity; });
p(function () { return (-1) ** -Infinity; });
p(function () { return 1 ** Infinity; });
p(function () { return 1 ** NaN; });
p(function () { return NaN ** 0; });
p(function () { return NaN ** -0; });
p(function () { var b = -1, e = Infinity; b **= e; return b; });
p(function () { return Object.is((-0) ** 3, -0) + " " + ((-0) ** -3) + " " + ((-0) ** -2); });
p(function () { return (-Infinity) ** 3 + " " + Object.is((-Infinity) ** -3, -0) + " " + (-Infinity) ** 2; });
p(function () { return 0.5 ** Infinity + " " + 0.5 ** -Infinity + " " + 2 ** -Infinity; });
p(function () { return (-8) ** (1 / 3); });
p(function () { return Math.pow(-1, Infinity) + " " + Math.pow(1, NaN) + " " + Math.pow(NaN, 0); });

// --- ToPropertyKey runs once for a computed member reference (13-24)
p(function () { return once(function (o, log) { var k = counted("a", log); return o[k] += 1; }); });
p(function () { return once(function (o, log) { var k = counted("a", log); return o[k] **= 2; }); });
p(function () { return once(function (o, log) { var k = counted("a", log); return o[k]++; }); });
p(function () { return once(function (o, log) { var k = counted("a", log); return --o[k]; }); });
p(function () { return once(function (o, log) { var k = counted("a", log); return o[k] ||= 7; }); });
p(function () { return once(function (o, log) { var k = counted("z", log); return o[k] ??= 7; }); });
p(function () { return once(function (o, log) { var k = counted("a", log); o.a = 1; return o[k] &&= 9; }); });
p(function () { var log = []; var k = { toString: function () { log.push("k"); return 1; } }; var b = [5, 6]; b[k] *= 2; return log.join("") + ":" + b; });
p(function () { var log = []; var k = { toString: function () { log.push("k"); return "a"; } }; try { null[k] += 1; } catch (e) { return e.name + ":" + log.join(""); } return "no throw"; });
p(function () { var log = []; var s = Symbol("s"); var k = { [Symbol.toPrimitive]: function () { log.push("k"); return s; } }; var o = {}; o[s] = 1; o[k] += 1; return log.join("") + ":" + o[s]; });
p(function () { var log = []; var k = counted("a", log); var o = { a: 1 }; o[k] -= (log.push("v"), 1); return log.join("") + ":" + o.a; });
p(function () { var a = [1, 2, 3]; for (var i = 0; i < a.length; i++) { a[i] += 10; a[i]++; } return a.join(); });

// --- a super property accepts a Symbol key (25-29)
p(function () { class RE extends RegExp { [Symbol.replace](s, r) { return "S:" + super[Symbol.replace](s, r); } } return "abc".replace(new RE("b"), "x"); });
p(function () { var s = Symbol("s"); var base = { [s]: 10 }; var d = { __proto__: base, m() { return super[s]; } }; return d.m(); });
p(function () { var s = Symbol("s"); var base = {}; var d = { __proto__: base, w() { super[s] = 3; return this[s] + ":" + base.hasOwnProperty(s); } }; return d.w(); });
p(function () { var s = Symbol("s"); var base = { [s]: 10 }; var d = { __proto__: base, c() { super[s] += 1; return this[s] + ":" + base[s]; } }; return d.c(); });
p(function () { var base = { get [Symbol.iterator]() { return "got"; } }; var d = { __proto__: base, m() { return super[Symbol.iterator]; } }; return d.m(); });

// --- Array.prototype.toLocaleString (30-36) and the typed-array one on the same algorithm (37-39)
p(function () { return Array.prototype.hasOwnProperty("toLocaleString") + " " + Array.prototype.toLocaleString.length + " " + Array.prototype.toLocaleString.name; });
p(function () { return [1, null, undefined, { toLocaleString: function () { return "L"; } }].toLocaleString(); });
p(function () { var n = 0; var obj = { toLocaleString: function () { n++; return "x"; } }; [undefined, obj, null, obj, obj].toLocaleString(); return n; });
p(function () { return Array.prototype.toLocaleString.call({ length: 2, 0: "a", 1: "b" }); });
p(function () { return [{ toLocaleString: 1 }].toLocaleString(); });
p(function () { "use strict"; var seen = []; Boolean.prototype.toLocaleString = function () { seen.push(typeof this); return "b"; }; var r = [true, false].toLocaleString(); delete Boolean.prototype.toLocaleString; return r + ":" + seen; });
p(function () { return [[1, 2], [3]].toLocaleString() + " " + [].toLocaleString(); });
p(function () { var d = Object.getOwnPropertyDescriptor(Number.prototype, "toLocaleString"); Number.prototype.toLocaleString = 5; try { return new Int8Array([1]).toLocaleString(); } finally { Object.defineProperty(Number.prototype, "toLocaleString", d); } });
p(function () { var d = Object.getOwnPropertyDescriptor(Number.prototype, "toLocaleString"); var seen = []; Number.prototype.toLocaleString = function () { seen.push(arguments.length); return "<" + this + ">"; }; try { return new Float64Array([1.5, -2, 0]).toLocaleString() + ":" + seen; } finally { Object.defineProperty(Number.prototype, "toLocaleString", d); } });
p(function () { return new Uint8Array([1, 2, 3]).toLocaleString() + "|" + new Uint8Array(0).toLocaleString() + "|" + (1).toLocaleString(); });
