// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE PUBLIC BIGINT SURFACE, retained from JSeal slice B05 (decision
// JSD-0033 section 7). The wide manifest admits BigInt from this slice on, through the optional
// surface `broiler.javascript.bigint`: literals, the `BigInt` global and its prototype, wrappers,
// conversions, equality and relational comparison with exact mixed Number/String operands, and the
// language's TypeErrors wherever a BigInt may not become a Number. BigInt64Array, BigUint64Array and
// the DataView BigInt accessors are cards B07-B08: case 60 asked about them as absences at B05 and
// answers "function" for all three since B07-B08 (probe the-bigint-typed-arrays covers them).
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : typeof v === "bigint" ? v + "n" : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// 1-6: literals, typeof, truthiness and the exact decimal text.
p(function () { return [typeof 1n, typeof Object(1n), typeof BigInt, typeof BigInt.prototype].join(","); });
p(function () { return [!!0n, !!-0n, !!1n, !!-1n, 0n ? "t" : "f", Boolean(0n)].join(","); });
p(function () { return String(9007199254740993n) + "|" + (0x1fffffffffffffn + 2n) + "|" + 0b1010n + "|" + 0o777n; });
p(function () { return `${123456789012345678901234567890n}` + "" + -5n; });
p(function () { return [1n + "", "" + -0n, [1n, 2n].join("-"), String([3n])].join("|"); });
p(function () { var o = {}; o[1n] = "a"; o[10000000000000000000000n] = "b"; return Object.keys(o).join(","); });

// 7-14: the BigInt function: conversion, parsing and refusals.
p(function () { return [BigInt(0), BigInt(-0), BigInt(42), BigInt(2 ** 60), BigInt(-1e21)].join(","); });
p(function () { return [BigInt(true), BigInt(false), BigInt(""), BigInt("  12  "), BigInt("-7"), BigInt("+8")].join(","); });
p(function () { return [BigInt("0x10"), BigInt("0o17"), BigInt("0b101"), BigInt("0XfF"), BigInt("\n\t 0 ")].join(","); });
p(function () { return ["1.5", "1e3", "0x", "-0x1", "1n", "1_0", "abc", "0b2", " - 1"].map(function (s) { return t(function () { return BigInt(s); }); }).join(","); });
p(function () { return [1.5, NaN, Infinity, -Infinity, 2 ** -1].map(function (v) { return t(function () { return BigInt(v); }); }).join(","); });
p(function () { return [undefined, null, Symbol()].map(function (v) { return t(function () { return BigInt(v); }); }).join(",") + "|" + t(function () { return BigInt(); }); });
p(function () { return t(function () { return new BigInt(1); }) + "," + t(function () { return BigInt({ valueOf: function () { return 5n; } }); }) + "," + t(function () { return BigInt({ valueOf: function () { return 6; } }); }) + "," + t(function () { return BigInt({ toString: function () { return "7"; }, valueOf: undefined }); }); });
p(function () { return [BigInt.length, BigInt.name, BigInt.asIntN.length, BigInt.asUintN.length, BigInt.prototype.toString.length, Object.getPrototypeOf(BigInt.prototype) === Object.prototype].join(","); });

// 15-19: asIntN and asUintN.
p(function () { return [BigInt.asIntN(8, 255n), BigInt.asIntN(8, 128n), BigInt.asIntN(8, -129n), BigInt.asIntN(0, 5n), BigInt.asIntN(64, 2n ** 63n)].join(","); });
p(function () { return [BigInt.asUintN(8, -1n), BigInt.asUintN(64, -1n), BigInt.asUintN(0, 9n), BigInt.asUintN(1, 3n), BigInt.asUintN(200, 5n)].join(","); });
p(function () { return [t(function () { return BigInt.asIntN(-1, 1n); }), t(function () { return BigInt.asUintN(2 ** 53, 1n); }), t(function () { return BigInt.asIntN(8, 1); }), t(function () { return BigInt.asUintN(8, "0x1ff"); })].join(","); });
p(function () { return [BigInt.asIntN("3", 7n), BigInt.asIntN(undefined, 7n), BigInt.asUintN(2.9, 7n), BigInt.asIntN(2 ** 53 - 1, -3n)].join(","); });
p(function () { var l = []; BigInt.asIntN({ valueOf: function () { l.push("bits"); return 4; } }, { valueOf: function () { l.push("big"); return 20n; } }); return l.join(","); });

// 20-25: BigInt.prototype.
p(function () { return [(255n).toString(16), (-255n).toString(2), (35n).toString(36), (0n).toString(7), (10n).toString(undefined)].join(","); });
p(function () { return [t(function () { return (1n).toString(1); }), t(function () { return (1n).toString(37); }), t(function () { return BigInt.prototype.toString.call(1); }), t(function () { return BigInt.prototype.valueOf.call({}); })].join(","); });
p(function () { return [BigInt.prototype.valueOf.call(Object(3n)), BigInt.prototype.toString.call(Object(-4n)), (5n).toLocaleString(), typeof (6n).valueOf()].join(","); });
p(function () { return [Object.prototype.toString.call(1n), Object.prototype.toString.call(Object(1n)), BigInt.prototype[Symbol.toStringTag]].join(","); });
p(function () { var d = Object.getOwnPropertyDescriptor(BigInt.prototype, Symbol.toStringTag); return [d.writable, d.enumerable, d.configurable].join(","); });
p(function () { return (123456789n ** 5n).toString(3).length + "," + (2n ** 200n).toString(32) + "," + (-(16n ** 20n)).toString(16); });

// 26-30: wrappers and ToPrimitive.
p(function () { var w = Object(7n); return [typeof w, w instanceof BigInt, w + 1n, w == 7n, w === 7n, w.constructor === BigInt].join(","); });
p(function () { BigInt.prototype.twice = function () { return this * 2n; }; try { return (21n).twice(); } finally { delete BigInt.prototype.twice; } });
p(function () { "use strict"; BigInt.prototype.self = function () { return typeof this; }; try { return (1n).self(); } finally { delete BigInt.prototype.self; } });
p(function () { var o = { valueOf: function () { return 2n; } }; return [o * 3n, -o, o + "", `${o}`].join(","); });
p(function () { return [t(function () { return +1n; }), t(function () { return Number(1n); }), t(function () { return 1n + 1; }), t(function () { return Math.abs(-1n); }), t(function () { return Math.max(1n, 2n); })].join(","); });

// 31-37: equality, strict and loose, with exact mixed operands.
p(function () { return [1n === 1n, 2n ** 64n === 18446744073709551616n, 1n === 1, 0n === -0n, Object.is(0n, -0n), [NaN, 1n].includes(1n), [1n].indexOf(1n)].join(","); });
p(function () { return [1n == 1, 1 == 1n, 0n == -0, 1n == 1.5, 9007199254740993n == 9007199254740992, 2n ** 53n == 2 ** 53].join(","); });
p(function () { return [1n == NaN, 1n == Infinity, -1n == -Infinity, 1n != 2, 3n != 3].join(","); });
p(function () { return [1n == "1", "1" == 1n, 1n == "0x1", 1n == "1.0", 0n == "", 1n == "1n", 10n == " 10 ", 9007199254740993n == "9007199254740993"].join(","); });
p(function () { return [1n == true, 0n == false, 2n == true, 1n == null, 0n == undefined, 1n == Symbol.iterator].join(","); });
p(function () { return [1n == Object(1n), Object(1n) == 1n, 1n == { valueOf: function () { return 1; } }, 1n == [1], Object(1n) == Object(1n)].join(","); });
p(function () { return [new Set([1n, 1n, BigInt(1), 1]).size, new Map([[2n, "a"]]).get(BigInt("2")), new Set([0n]).has(-0n)].join(","); });

// 38-44: relational comparison, exact against Number and String.
p(function () { return [1n < 2n, -3n < -2n, 2n ** 100n > 2n ** 99n, 5n <= 5n, 5n >= 6n].join(","); });
p(function () { return [1n < 1.5, 2n > 1.5, 9007199254740993n > 9007199254740992, 9007199254740992 < 9007199254740993n, -1n < -0.5, 0n <= -0].join(","); });
p(function () { return [1n < Infinity, 1n > -Infinity, 1n < NaN, 1n >= NaN, NaN > 1n, 2n ** 1100n > 1.7976931348623157e308].join(","); });
p(function () { return [1n < "2", "10" > 9n, 1n < "x", 1n >= "x", "1.5" > 1n, 1n <= "0x1", "9007199254740993" > 9007199254740992n].join(","); });
p(function () { return [1n < true, 0n < null, 1n > undefined, 1n < { valueOf: function () { return 2n; } }].join(","); });
p(function () { return t(function () { return 1n < Symbol(); }) + "," + t(function () { return Symbol() > 1n; }); });
p(function () { var l = []; var a = { valueOf: function () { l.push("a"); return 1n; } }; var b = { valueOf: function () { l.push("b"); return 2; } }; return [a > b, l.join("")].join(","); });

// 45-48: sorting, coercion order, Number conversion by the right operation only.
p(function () { return [3n, -1n, 10n, 2n].sort(function (a, b) { return a < b ? -1 : a > b ? 1 : 0; }).join(",") + "|" + [3n, -1n, 10n, 2n].sort().join(","); });
p(function () { return [3n, 1, 2n, 0.5].sort(function (a, b) { return a < b ? -1 : a > b ? 1 : 0; }).map(function (v) { return typeof v; }).join(","); });
p(function () { return [parseInt(12n), parseFloat(-3n), String(2n ** 70n), isNaN.length].join(","); });
p(function () { return [t(function () { return isNaN(1n); }), Number.isInteger(1n), Number.isFinite(5n), t(function () { return new Date(0n); }), t(function () { return "ab".at(1n); })].join(","); });

// 49-53: JSON.
p(function () { return t(function () { return JSON.stringify(1n); }) + "," + t(function () { return JSON.stringify({ a: 2n }); }) + "," + t(function () { return JSON.stringify([Object(3n)]); }); });
p(function () { BigInt.prototype.toJSON = function () { return this.toString() + "n"; }; try { return JSON.stringify({ a: 1n, b: [2n] }); } finally { delete BigInt.prototype.toJSON; } });
p(function () { return JSON.stringify({ a: 1n }, function (k, v) { return typeof v === "bigint" ? Number(v) : v; }); });
p(function () { return JSON.stringify({ a: 1n }, function (k, v) { return typeof v === "bigint" ? v.toString() : v; }); });
p(function () { return t(function () { return JSON.parse("1n"); }) + "," + typeof JSON.parse("123456789012345678901"); });

// 54-59: update expressions and compound assignment on every reference kind.
p(function () { var a = 5n; var b = a++; var c = ++a; var d = a--; var e = --a; return [a, b, c, d, e].join(","); });
p(function () { var o = { x: 1n }, k = "x"; o.x++; ++o[k]; o.x += 10n; return [o.x, typeof o.x].join(","); });
p(function () { var a = [0n]; a[0]--; a[0] -= 1n; return a[0]; });
p(function () { class C { #p = 2n; step() { this.#p++; return ++this.#p; } } return new C().step(); });
p(function () { var w = Object(9n); var old = w++; return [typeof old, old, w].join(","); });
p(function () { var s = "5", n = 1.5, u; s++; n--; u++; return [s, n, u].join(","); });

// 60: BigInt typed arrays and DataView accessors: absent at B05, present since cards B07-B08.
p(function () { return [typeof BigInt64Array, typeof BigUint64Array, typeof DataView.prototype.getBigInt64].join(","); });

// 61-62: Number(x) rounds to nearest, ties to even, and past the largest finite Number is Infinity.
p(function () { return [Number(2n ** 53n + 1n), Number(2n ** 53n + 3n), Number(-(2n ** 53n + 3n)), Number(2n ** 1024n - 2n ** 970n), Number(2n ** 1024n - 2n ** 970n - 1n), Number(123456789012345678901234567890n)].join(","); });
p(function () { var big = "1" + "0".repeat(400); return [BigInt(big) === 10n ** 400n, 10n ** 400n == big, 10n ** 400n < big + "1", big.length].join(","); });
