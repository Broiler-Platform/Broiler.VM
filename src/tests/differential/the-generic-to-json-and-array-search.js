// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THREE SMALL GENERIC ALGORITHMS: `Date.prototype.toJSON` works on any
// receiver (ToObject, ToPrimitive with hint number, `null` for a non-finite Number, then
// Invoke(O, "toISOString") - ES2026 Date.prototype.toJSON); `Array.prototype.entries`, `keys`,
// `values` and `[Symbol.iterator]` begin with ToObject, so a nullish receiver is a TypeError and a
// primitive one is wrapped; and `indexOf`/`lastIndexOf` read their start through
// ToIntegerOrInfinity, which never answers -0, with the infinities and `lastIndexOf`'s
// `length - 1` default handled as the algorithm spells them. The last cases pin that `Array.of`
// with a BigInt typed array constructor throws where CreateDataPropertyOrThrow or the `length`
// write fails, and the array iterator reads its receiver's length through LengthOfArrayLike while
// a key iterator never reads the element.
//
// Retained from the JSeal follow-up fix VM-FIX-J: every case was checked against those steps and
// compared against the comparison engine before it was written down.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
var toJSON = Date.prototype.toJSON;

// --- Date.prototype.toJSON on a Date
p(function () { return new Date(0).toJSON(); });
p(function () { return new Date(NaN).toJSON(); });
p(function () { return toJSON.length; });
// --- on any object: its own toISOString is invoked, with no arguments
p(function () { return toJSON.call({ toISOString: function () { return "own " + arguments.length; } }); });
p(function () { var o = { toISOString: function () { return this === o; } }; return toJSON.call(o); });
// --- a non-finite Number from ToPrimitive answers null without reading toISOString
p(function () { return toJSON.call({ valueOf: function () { return Infinity; }, get toISOString() { throw new Error("read"); } }); });
p(function () { return toJSON.call({ valueOf: function () { return -Infinity; } }); });
p(function () { return toJSON.call({ valueOf: function () { return NaN; } }); });
// --- a primitive that is not a Number is not checked for finiteness
p(function () { return toJSON.call({ valueOf: function () { return "abc"; }, toISOString: function () { return "str"; } }); });
// --- ToPrimitive asks for a number, and runs before toISOString is read
p(function () { var log = []; toJSON.call({ valueOf: function () { log.push("valueOf"); return 1; }, toString: function () { log.push("toString"); return ""; }, get toISOString() { log.push("get"); return function () { log.push("call"); }; } }); return log.join(","); });
p(function () { var hint; var o = {}; o[Symbol.toPrimitive] = function (h) { hint = h; return 2; }; o.toISOString = function () { return hint; }; return toJSON.call(o); });
// --- abrupt completions pass through unchanged
p(function () { try { toJSON.call({ valueOf: function () { throw new RangeError("vo"); } }); } catch (e) { return e.name + " " + e.message; } });
p(function () { try { toJSON.call({ toISOString: function () { throw new SyntaxError("iso"); } }); } catch (e) { return e.name + " " + e.message; } });
p(function () { return toJSON.call({ toISOString: 1 }); });
p(function () { return toJSON.call({}); });
// --- the receiver goes through ToObject
p(function () { return toJSON.call(undefined); });
p(function () { return toJSON.call(null); });
p(function () { Number.prototype.toISOString = function () { return typeof this; }; try { return toJSON.call(5); } finally { delete Number.prototype.toISOString; } });
p(function () { return toJSON.call(Symbol()); });
// --- JSON.stringify reaching it through a borrowed method
p(function () { return JSON.stringify({ d: { toJSON: toJSON, toISOString: function () { return "iso"; } } }); });
p(function () { return JSON.stringify([new Date(NaN)]); });

// --- the Array iteration methods begin with ToObject
p(function () { return Array.prototype.entries.call(undefined); });
p(function () { return Array.prototype.keys.call(null); });
p(function () { return Array.prototype.values.call(undefined); });
p(function () { return Array.prototype[Symbol.iterator].call(null); });
p(function () { return Array.from(Array.prototype.values.call("ab")).join(","); });
p(function () { return Array.from(Array.prototype.keys.call(5)).length; });
p(function () { return Object.getPrototypeOf(Array.prototype.entries.call(true)) === Object.getPrototypeOf([].entries()); });
p(function () { return JSON.stringify(Array.from(Array.prototype.entries.call({ length: 2, 0: "a", 1: "b" }))); });

// --- indexOf and lastIndexOf: -0 is +0
p(function () { return 1 / [true].indexOf(true, -0); });
p(function () { return 1 / [true].lastIndexOf(true, -0); });
p(function () { return 1 / [true].indexOf(true, -0.5); });
p(function () { return 1 / [true, 0].lastIndexOf(true, -0); });
// --- the infinities
p(function () { return [1, 2, 1].indexOf(1, Infinity); });
p(function () { return [1, 2, 1].indexOf(1, -Infinity); });
p(function () { return [1, 2, 1].lastIndexOf(1, Infinity); });
p(function () { return [1, 2, 1].lastIndexOf(1, -Infinity); });
// --- lastIndexOf's default is length - 1, but an explicit undefined is 0
p(function () { return [1, 2, 1].lastIndexOf(1); });
p(function () { return [1, 2, 1].lastIndexOf(1, undefined); });
p(function () { return [2, 1, 2].lastIndexOf(2, undefined); });
// --- negative starts count from the end, and clamp
p(function () { return [1, 2, 1].indexOf(1, -1); });
p(function () { return [1, 2, 1].indexOf(1, -5); });
p(function () { return [1, 2, 1].lastIndexOf(1, -2); });
p(function () { return [1, 2, 1].lastIndexOf(1, -4); });
p(function () { return [1, 2, 1].lastIndexOf(1, 7); });
// --- an empty array converts nothing
p(function () { var n = 0; [].indexOf(1, { valueOf: function () { n++; return 0; } }); [].lastIndexOf(1, { valueOf: function () { n++; return 0; } }); return n; });
p(function () { return Array.prototype.indexOf.call({ length: 3, 2: "x" }, "x", -1) + "," + Array.prototype.lastIndexOf.call({ length: 3, 0: "x" }, "x", -3); });

// --- Array.of with a BigInt typed array as the constructed result
p(function () { return Array.of.call(function () { return new BigInt64Array(2); }, 1); });
p(function () { return Array.of.call(function () { return new BigInt64Array(2); }, 1n); });
p(function () { try { Array.of.call(function () { return new BigInt64Array(2); }, 1); } catch (e) { return e instanceof TypeError; } });

// --- the array iterator measures its receiver with LengthOfArrayLike (ToLength), and a key iterator
// never reads the element
p(function () { return [...Array.prototype.values.call({ length: 2.7, 0: "a", 1: "b", 2: "c" })].join(); });
p(function () { return [...Array.prototype.keys.call({ length: "2.5" })].join(); });
p(function () { return [...Array.prototype.entries.call({ length: -1, 0: "x" })].length; });
p(function () { var n = 0, o = { length: 2 }; Object.defineProperty(o, 0, { get: function () { n++; return "z"; } }); [...Array.prototype.keys.call(o)]; return n; });
p(function () { var n = 0, o = { length: 2 }; Object.defineProperty(o, 0, { get: function () { n++; return "z"; } }); return JSON.stringify([...Array.prototype.entries.call(o)]) + " " + n; });
