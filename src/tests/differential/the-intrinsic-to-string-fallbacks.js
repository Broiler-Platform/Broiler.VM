// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE TWO toString METHODS THE SPECIFICATION TIES TO INTRINSICS:
// `Array.prototype.toString` falls back to %Object.prototype.toString% when `join` is not callable,
// and `%TypedArray%.prototype.toString` IS %Array.prototype.toString%.
//
// Retained from JSeal slice B06, whose whole-suite Test262 run exposed the first (it was hidden
// behind the BigInt refusal until B05): the fallback read `Object.prototype.toString` at the call,
// so deleting or replacing that property changed what `String([])` answered for a receiver
// without a callable `join`. Every case was checked against ECMA-262 (Array.prototype.toString
// step 3; %TypedArray%.prototype.toString) and compared against the comparison engine before it
// was written down. The cases that delete or replace a property restore it before they return.
// Fixing the fallback exposed the next step of the same Test262 file: Object.prototype.toString
// answered an object's internal class name as its builtin tag (so a Set whose tag was deleted
// printed `[object Set]`) and read the tag before deriving it; the realm had leaned on that for six
// brands whose `Symbol.toStringTag` it never installed. Cases 18-23 cover both.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
var intrinsic = Object.prototype.toString;
function without(f) { delete Object.prototype.toString; try { return f(); } finally { Object.prototype.toString = intrinsic; } }
function replaced(f) { Object.prototype.toString = function () { return "replaced"; }; try { return f(); } finally { Object.prototype.toString = intrinsic; } }

// --- the fallback is the intrinsic, whatever Object.prototype holds now
p(function () { return Array.prototype.toString.call({ join: null }); });
p(function () { return without(function () { return Array.prototype.toString.call({ join: null }); }); });
p(function () { return without(function () { return Array.prototype.toString.call({ join: 0n }); }); });
p(function () { return without(function () { return Array.prototype.toString.call(new Map()); }); });
p(function () { return replaced(function () { return Array.prototype.toString.call({ join: 1 }); }); });
p(function () { return replaced(function () { return Array.prototype.toString.call({ join: "x", [Symbol.toStringTag]: "Tagged" }); }); });
p(function () { return without(function () { var a = [1, 2]; a.join = undefined; return Array.prototype.toString.call(a); }); });

// --- a callable join is still called, with the receiver and no arguments
p(function () { return Array.prototype.toString.call({ join: function () { return "joined:" + arguments.length; } }); });
p(function () { return without(function () { return String([1, [2, 3]]); }); });
p(function () { return Array.prototype.toString.call("ab"); });
p(function () { return Array.prototype.toString.call(null); });
p(function () { return Array.prototype.toString.call(undefined); });

// --- the typed-array method is the same function object
p(function () { return Uint8Array.prototype.toString === Array.prototype.toString; });
p(function () { return Object.getPrototypeOf(Int8Array.prototype).toString === Array.prototype.toString; });
p(function () { return String(new Uint8Array([1, 2, 3])); });
p(function () { var u = new Uint8Array([4]); u.join = null; return without(function () { return String(u); }); });
p(function () { var d = Object.getOwnPropertyDescriptor(Object.getPrototypeOf(Int8Array.prototype), "toString"); return [d.writable, d.enumerable, d.configurable].join(","); });

// --- the builtin tag: only the specification's ten kinds, derived before the tag is read
p(function () { return [new WeakMap(), new WeakSet(), new ArrayBuffer(1), new DataView(new ArrayBuffer(1)), new Int16Array(1)].map(function (v) { return Object.prototype.toString.call(v); }).join(","); });
p(function () { var d = Object.getOwnPropertyDescriptor(Set.prototype, Symbol.toStringTag); delete Set.prototype[Symbol.toStringTag]; try { return Object.prototype.toString.call(new Set()); } finally { Object.defineProperty(Set.prototype, Symbol.toStringTag, d); } });
p(function () { var d = Object.getOwnPropertyDescriptor(WeakMap.prototype, Symbol.toStringTag); delete WeakMap.prototype[Symbol.toStringTag]; try { return Array.prototype.toString.call(new WeakMap()); } finally { Object.defineProperty(WeakMap.prototype, Symbol.toStringTag, d); } });
p(function () { var g = Object.getOwnPropertyDescriptor(Object.getPrototypeOf(Int8Array.prototype), Symbol.toStringTag).get; return [g.call(3), g.call({}), g.call(new Float64Array(0)), g.name, g.length].join(","); });
p(function () { var h = Proxy.revocable([], { get: function () { h.revoke(); } }); return Object.prototype.toString.call(h.proxy); });
p(function () { return [Object.prototype.toString.call(function () {}), Object.prototype.toString.call(class {}), Object.prototype.toString.call(new Proxy(new Date(0), {})), Object.prototype.toString.call((function () { return arguments; })())].join(","); });
