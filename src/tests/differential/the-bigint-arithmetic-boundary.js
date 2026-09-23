// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE NUMERIC OPERATORS THE BIGINT ARITHMETIC CHANGED, retained from JSeal
// slices B03-B04 (decision JSD-0033). The arithmetic, bitwise, shift, unary and update operators now
// convert their operands with ToNumeric and dispatch on the result, so every Number path runs through
// new code: cases 1-20 ask that no Number answer, conversion order or error moved. The BigInt
// operators themselves are judged by the slice compiler's `--checks` rows `bigint/b03/...` and
// `bigint/b04/...` through the internal gate, which the end-user host cannot open: here a BigInt in
// any operator is still the wide manifest's named refusal (cases 21-24).
// (Amended 2026-09-21, JSeal B05: the wide manifest admits BigInt through `broiler.javascript.bigint`
// now, so cases 21-24 answer "bigint" as the comparison engine does; the public operators are probed
// in `the-bigint-public-surface.js`.)
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function o(log, name, value) { return { valueOf: function () { log.push(name); return value; } }; }

// 1-6: Number arithmetic, including the signed zero, NaN and the string concatenation of `+`.
p(function () { return [1 + 2, "1" + 2, 1 + "2", 5 - "2", "6" * "7", 7 / 2, -7 % 2, 2 ** -1].join(","); });
p(function () { return [Object.is(-0 + -0, -0), Object.is(0 - 0, 0), Object.is(-0 * 1, -0), 1 / -0].join(","); });
p(function () { return [NaN + 1, undefined - 1, null * 3, true + true, [] + [], [2] * [3], {} + 1].join("|"); });
p(function () { return [2 ** 53 + 1, 0.1 + 0.2, 1e308 * 10, -(2 ** 31), (-8) ** (1 / 3)].join(","); });
p(function () { var s = Symbol("s"); return t(function () { return s + 1; }) + t(function () { return 1 - s; }); });
p(function () { return [-"3", -{ valueOf: function () { return 4; } }, -[5], -"x", -null].join(","); });

// 7-12: bitwise and shift operators over 32-bit Numbers.
p(function () { return [5 & 3, -5 | 3, 5 ^ 3, ~5, ~-1, ~~3.7, ~"8"].join(","); });
p(function () { return [1 << 31, 1 << 32, 1 << -1, -1 >> 28, -1 >>> 28, -1 >>> 0, 2 ** 32 >> 0].join(","); });
p(function () { return ["3" << "1", 4.9 >> 1, NaN | 0, Infinity & 1, (2 ** 32 + 5) | 0, -0 | 0].join(","); });
p(function () { return [0xffffffff & 0xffffffff, 0x80000000 | 0, 0x80000000 >>> 0, 7 >>> 32].join(","); });
p(function () { var s = Symbol(); return t(function () { return s | 0; }) + t(function () { return 1 << s; }); });
p(function () { var a = 6; a &= 3; var b = 1; b <<= 4; var c = -16; c >>= 2; var d = -16; d >>>= 28; var e = 5; e ^= 1; return [a, b, c, d, e].join(","); });

// 13-16: both operands are converted, left first, before either operation runs.
p(function () { var l = []; o(l, "a", 1) - o(l, "b", 2); o(l, "c", 1) * o(l, "d", 2); o(l, "e", 1) ** o(l, "f", 2); return l.join(""); });
p(function () { var l = []; o(l, "a", 1) & o(l, "b", 2); o(l, "c", 1) << o(l, "d", 2); o(l, "e", 1) >>> o(l, "f", 2); return l.join(""); });
p(function () { var l = []; try { o(l, "a", 1) - Symbol(); } catch (e) { l.push(e.name); } try { Symbol() | o(l, "b", 1); } catch (e) { l.push(e.name); } return l.join(","); });
p(function () { var l = []; var r = o(l, "a", 1) + o(l, "b", 2); var s = { toString: function () { l.push("s"); return "x"; }, valueOf: null } + 1; return l.join("") + r + s; });

// 17-20: update expressions convert once with ToNumber and answer a Number.
p(function () { var s = "5"; var old = s++; return [typeof old, old, s, typeof s].join(","); });
p(function () { var l = []; var v = o(l, "v", 9); var old = v--; return [old, v, l.join("")].join(","); });
p(function () { var z = -0; var old = z++; var y = -0; var q = --y; return [Object.is(old, -0), z, q].join(","); });
p(function () { var obj = { n: "1" }, arr = ["2"], k = "n"; obj.n++; ++arr[0]; obj[k]--; var u; u++; return [obj.n, arr[0], u].join(","); });

// 21-24: a BigInt in an operator is still the wide manifest's named refusal, never a Number.
p(function () { return typeof eval("1n + 1n"); });
p(function () { return typeof eval("var x = 1n; x++; x"); });
p(function () { return typeof eval("~1n"); });
p(function () { return typeof eval("1n << 2n"); });
