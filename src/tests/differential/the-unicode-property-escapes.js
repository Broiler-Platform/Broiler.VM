// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER UNICODE PROPERTY ESCAPES (JSeal slice F09, JSD-0031 U4).
//
// Under the u flag, \p{...} and \P{...} name a Unicode 17.0.0 property by the exact names ES2026
// admits - a General_Category value or alias, one of the 53 binary properties, or
// General_Category/gc, Script/sc and Script_Extensions/scx with a value - inside and outside a
// class, complemented by \P or by [^...]. Anything else is an early SyntaxError: no loose
// matching, no other property, no properties of strings (those need the v flag, which this realm
// refuses as a flag). Without u, \p is still the identity escape Annex B makes it. Under iu the
// comparison is Canonicalize, which is simple case folding from CaseFolding.txt for literals,
// classes and property escapes alike, so /\p{Lu}/iu matches a lower-case letter.
//
// The last cases walk every \p{Lu} code point whose lower case is one code point - 1,414 pairs,
// listed below as runs [first, count, stride, delta] generated once with Node v24.17.0 (Unicode
// 17.0) from toLowerCase, which this realm cannot use for it because its case conversion still
// reads the platform's Unicode 16 data - and report every pair that /\p{Lu}/iu, /X/iu or /[X]/iu
// fails to match. The 28 Unicode 17.0 pairs of JSD-0031 section 1 are among them and are also
// checked by name.
//
// Each case prints its own number, so a divergence names a case rather than a line. A case that
// throws prints the error's name, because a refusal is an answer.

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function re(source, flags) { return new RegExp(source, flags); }
function hex(cp) { return cp.toString(16).toUpperCase(); }
function esc(cp) { return "\\u{" + hex(cp) + "}"; }
var LU_PAIRS = [
  [0x41, 0x1A, 0x1, 0x20], [0xC0, 0x17, 0x1, 0x20], [0xD8, 0x7, 0x1, 0x20],
  [0x100, 0x18, 0x2, 0x1], [0x132, 0x3, 0x2, 0x1], [0x139, 0x8, 0x2, 0x1], [0x14A, 0x17, 0x2, 0x1],
  [0x178, 0x1, 0x0, -0x79], [0x179, 0x3, 0x2, 0x1], [0x181, 0x1, 0x0, 0xD2],
  [0x182, 0x2, 0x2, 0x1], [0x186, 0x1, 0x0, 0xCE], [0x187, 0x1, 0x0, 0x1], [0x189, 0x2, 0x1, 0xCD],
  [0x18B, 0x1, 0x0, 0x1], [0x18E, 0x1, 0x0, 0x4F], [0x18F, 0x1, 0x0, 0xCA],
  [0x190, 0x1, 0x0, 0xCB], [0x191, 0x1, 0x0, 0x1], [0x193, 0x1, 0x0, 0xCD],
  [0x194, 0x1, 0x0, 0xCF], [0x196, 0x1, 0x0, 0xD3], [0x197, 0x1, 0x0, 0xD1],
  [0x198, 0x1, 0x0, 0x1], [0x19C, 0x1, 0x0, 0xD3], [0x19D, 0x1, 0x0, 0xD5],
  [0x19F, 0x1, 0x0, 0xD6], [0x1A0, 0x3, 0x2, 0x1], [0x1A6, 0x1, 0x0, 0xDA], [0x1A7, 0x1, 0x0, 0x1],
  [0x1A9, 0x1, 0x0, 0xDA], [0x1AC, 0x1, 0x0, 0x1], [0x1AE, 0x1, 0x0, 0xDA], [0x1AF, 0x1, 0x0, 0x1],
  [0x1B1, 0x2, 0x1, 0xD9], [0x1B3, 0x2, 0x2, 0x1], [0x1B7, 0x1, 0x0, 0xDB], [0x1B8, 0x2, 0x4, 0x1],
  [0x1C4, 0x3, 0x3, 0x2], [0x1CD, 0x8, 0x2, 0x1], [0x1DE, 0x9, 0x2, 0x1], [0x1F1, 0x1, 0x0, 0x2],
  [0x1F4, 0x1, 0x0, 0x1], [0x1F6, 0x1, 0x0, -0x61], [0x1F7, 0x1, 0x0, -0x38],
  [0x1F8, 0x14, 0x2, 0x1], [0x220, 0x1, 0x0, -0x82], [0x222, 0x9, 0x2, 0x1],
  [0x23A, 0x1, 0x0, 0x2A2B], [0x23B, 0x1, 0x0, 0x1], [0x23D, 0x1, 0x0, -0xA3],
  [0x23E, 0x1, 0x0, 0x2A28], [0x241, 0x1, 0x0, 0x1], [0x243, 0x1, 0x0, -0xC3],
  [0x244, 0x1, 0x0, 0x45], [0x245, 0x1, 0x0, 0x47], [0x246, 0x5, 0x2, 0x1], [0x370, 0x2, 0x2, 0x1],
  [0x376, 0x1, 0x0, 0x1], [0x37F, 0x1, 0x0, 0x74], [0x386, 0x1, 0x0, 0x26],
  [0x388, 0x3, 0x1, 0x25], [0x38C, 0x1, 0x0, 0x40], [0x38E, 0x2, 0x1, 0x3F],
  [0x391, 0x11, 0x1, 0x20], [0x3A3, 0x9, 0x1, 0x20], [0x3CF, 0x1, 0x0, 0x8],
  [0x3D8, 0xC, 0x2, 0x1], [0x3F4, 0x1, 0x0, -0x3C], [0x3F7, 0x1, 0x0, 0x1],
  [0x3F9, 0x1, 0x0, -0x7], [0x3FA, 0x1, 0x0, 0x1], [0x3FD, 0x3, 0x1, -0x82],
  [0x400, 0x10, 0x1, 0x50], [0x410, 0x20, 0x1, 0x20], [0x460, 0x11, 0x2, 0x1],
  [0x48A, 0x1B, 0x2, 0x1], [0x4C0, 0x1, 0x0, 0xF], [0x4C1, 0x7, 0x2, 0x1], [0x4D0, 0x30, 0x2, 0x1],
  [0x531, 0x26, 0x1, 0x30], [0x10A0, 0x26, 0x1, 0x1C60], [0x10C7, 0x2, 0x6, 0x1C60],
  [0x13A0, 0x50, 0x1, 0x97D0], [0x13F0, 0x6, 0x1, 0x8], [0x1C89, 0x1, 0x0, 0x1],
  [0x1C90, 0x2B, 0x1, -0xBC0], [0x1CBD, 0x3, 0x1, -0xBC0], [0x1E00, 0x4B, 0x2, 0x1],
  [0x1E9E, 0x1, 0x0, -0x1DBF], [0x1EA0, 0x30, 0x2, 0x1], [0x1F08, 0x8, 0x1, -0x8],
  [0x1F18, 0x6, 0x1, -0x8], [0x1F28, 0x8, 0x1, -0x8], [0x1F38, 0x8, 0x1, -0x8],
  [0x1F48, 0x6, 0x1, -0x8], [0x1F59, 0x4, 0x2, -0x8], [0x1F68, 0x8, 0x1, -0x8],
  [0x1FB8, 0x2, 0x1, -0x8], [0x1FBA, 0x2, 0x1, -0x4A], [0x1FC8, 0x4, 0x1, -0x56],
  [0x1FD8, 0x2, 0x1, -0x8], [0x1FDA, 0x2, 0x1, -0x64], [0x1FE8, 0x2, 0x1, -0x8],
  [0x1FEA, 0x2, 0x1, -0x70], [0x1FEC, 0x1, 0x0, -0x7], [0x1FF8, 0x2, 0x1, -0x80],
  [0x1FFA, 0x2, 0x1, -0x7E], [0x2126, 0x1, 0x0, -0x1D5D], [0x212A, 0x1, 0x0, -0x20BF],
  [0x212B, 0x1, 0x0, -0x2046], [0x2132, 0x1, 0x0, 0x1C], [0x2183, 0x1, 0x0, 0x1],
  [0x2C00, 0x30, 0x1, 0x30], [0x2C60, 0x1, 0x0, 0x1], [0x2C62, 0x1, 0x0, -0x29F7],
  [0x2C63, 0x1, 0x0, -0xEE6], [0x2C64, 0x1, 0x0, -0x29E7], [0x2C67, 0x3, 0x2, 0x1],
  [0x2C6D, 0x1, 0x0, -0x2A1C], [0x2C6E, 0x1, 0x0, -0x29FD], [0x2C6F, 0x1, 0x0, -0x2A1F],
  [0x2C70, 0x1, 0x0, -0x2A1E], [0x2C72, 0x2, 0x3, 0x1], [0x2C7E, 0x2, 0x1, -0x2A3F],
  [0x2C80, 0x32, 0x2, 0x1], [0x2CEB, 0x2, 0x2, 0x1], [0x2CF2, 0x2, 0x794E, 0x1],
  [0xA642, 0x16, 0x2, 0x1], [0xA680, 0xE, 0x2, 0x1], [0xA722, 0x7, 0x2, 0x1],
  [0xA732, 0x1F, 0x2, 0x1], [0xA779, 0x2, 0x2, 0x1], [0xA77D, 0x1, 0x0, -0x8A04],
  [0xA77E, 0x5, 0x2, 0x1], [0xA78B, 0x1, 0x0, 0x1], [0xA78D, 0x1, 0x0, -0xA528],
  [0xA790, 0x2, 0x2, 0x1], [0xA796, 0xA, 0x2, 0x1], [0xA7AA, 0x1, 0x0, -0xA544],
  [0xA7AB, 0x1, 0x0, -0xA54F], [0xA7AC, 0x1, 0x0, -0xA54B], [0xA7AD, 0x1, 0x0, -0xA541],
  [0xA7AE, 0x1, 0x0, -0xA544], [0xA7B0, 0x1, 0x0, -0xA512], [0xA7B1, 0x1, 0x0, -0xA52A],
  [0xA7B2, 0x1, 0x0, -0xA515], [0xA7B3, 0x1, 0x0, 0x3A0], [0xA7B4, 0x8, 0x2, 0x1],
  [0xA7C4, 0x1, 0x0, -0x30], [0xA7C5, 0x1, 0x0, -0xA543], [0xA7C6, 0x1, 0x0, -0x8A38],
  [0xA7C7, 0x2, 0x2, 0x1], [0xA7CB, 0x1, 0x0, -0xA567], [0xA7CC, 0x8, 0x2, 0x1],
  [0xA7DC, 0x1, 0x0, -0xA641], [0xA7F5, 0x1, 0x0, 0x1], [0xFF21, 0x1A, 0x1, 0x20],
  [0x10400, 0x28, 0x1, 0x28], [0x104B0, 0x24, 0x1, 0x28], [0x10570, 0xB, 0x1, 0x27],
  [0x1057C, 0xF, 0x1, 0x27], [0x1058C, 0x7, 0x1, 0x27], [0x10594, 0x2, 0x1, 0x27],
  [0x10C80, 0x33, 0x1, 0x40], [0x10D50, 0x16, 0x1, 0x20], [0x118A0, 0x20, 0x1, 0x20],
  [0x16E40, 0x20, 0x1, 0x20], [0x16EA0, 0x19, 0x1, 0x1B], [0x1E900, 0x22, 0x1, 0x22]
];
function eachPair(visit) {
  for (var r = 0; r < LU_PAIRS.length; r++) {
    var run = LU_PAIRS[r];
    for (var k = 0; k < run[1]; k++) { var upper = run[0] + k * run[2]; visit(upper, upper + run[3]); }
  }
}
function misses(test) {
  var found = [];
  var total = 0;
  eachPair(function (upper, lower) { total++; if (!test(upper, lower)) { found.push(hex(upper)); } });
  return total + " pairs, " + found.length + " missed" + (found.length ? ": " + found.slice(0, 12).join(" ") : "");
}
// 1-9: the lone forms, positive and negative, and the card's own example.
p(function () { return new RegExp("\\p{Letter}", "u").test("a"); });
p(function () { return /\p{L}/u.test("1"); });
p(function () { return /\P{L}/u.test("1"); });
p(function () { return /\P{L}/u.test("a"); });
p(function () { return /^\p{Lu}+$/u.test("ABC\u00c9"); });
p(function () { return /\p{Nd}/u.test("\u0663") + "," + /\p{Decimal_Number}/u.test("x"); });
p(function () { return /\p{ASCII_Hex_Digit}/u.test("f") + "," + /\p{AHex}/u.test("g"); });
p(function () { return /^\p{Any}$/u.test("\ud800") + "," + /\P{Any}/u.test("\u{10FFFF}"); });
p(function () { return /\p{Assigned}/u.test("\u0378") + "," + /\p{Cn}/u.test("\u0378") + "," + /\p{ASCII}/u.test("\u007f"); });
// 10-17: the name=value forms.
p(function () { return /^\p{Script=Greek}+$/u.test("\u03b1\u03b2\u03b3"); });
p(function () { return /\p{sc=Grek}/u.test("a") + "," + /\p{Script=Latin}/u.test("a"); });
p(function () { return /\p{scx=Deva}/u.test("\u0964") + "," + /\p{sc=Deva}/u.test("\u0964") + "," + /\p{sc=Zyyy}/u.test("\u0964"); });
p(function () { return /\p{Script_Extensions=Devanagari}/u.test("\u0966") + "," + /\p{scx=Beng}/u.test("\u0966"); });
p(function () { return /\p{gc=Nd}/u.test("7") + "," + /\p{General_Category=Decimal_Number}/u.test("7") + "," + /\p{gc=L}/u.test("7"); });
p(function () { return /\p{General_Category=LC}/u.test("a") + "," + /\p{gc=Cased_Letter}/u.test("\u01c5"); });
p(function () { return re("\\p{sc=Hrkt}", "u").test("\u30a2") + "," + re("\\p{scx=Katakana_Or_Hiragana}", "u").test("\u30a2"); });
p(function () { return /\p{Script=Unknown}/u.test("\u0378") + "," + /\p{sc=Zzzz}/u.test("a"); });
// 18-26: supplementary code points, classes, complements and negated classes.
p(function () { return /^\p{L}$/u.test("\u{1D400}") + "," + /^\p{Lu}$/u.test("\ud835\udc00") + "," + /^\P{L}$/u.test("\u{1F600}"); });
p(function () { return /^[\p{L}\d]+$/u.test("ab12\u{1D400}") + "," + /^[\p{L}\d]+$/u.test("ab-12"); });
p(function () { return /^[^\p{L}]$/u.test("\u{1F600}") + "," + /^[^\p{L}]$/u.test("\u{1D400}"); });
p(function () { return /^[\P{L}]$/u.test("1") + "," + /^[\P{L}]$/u.test("a"); });
p(function () { return /^[^\P{L}]+$/u.test("abc\u{1D400}") + "," + /^[^\P{L}]$/u.test("1"); });
p(function () { return /^[\p{Lu}\p{Nd}]+$/u.test("A1B2") + "," + /^[\p{Lu}\P{Lu}]$/u.test("\u{10FFFF}"); });
p(function () { return "a1\u{1D400}b\u{1F600}".match(/\p{L}/gu).length + "," + "a1\u{1D400}b\u{1F600}".match(/\P{L}/gu).length; });
p(function () { return "x\u{1F600}y".replace(/\P{L}/gu, "_"); });
p(function () { return /(?<=\p{Lu})\p{Ll}/u.exec("aBc")[0] + "," + /(?<!\p{Lu})\p{Ll}/u.exec("Bca")[0]; });
// 27-33: the case-insensitive forms.
p(function () { return /\p{Lu}/iu.test("a") + "," + /\p{Lu}/u.test("a"); });
p(function () { return /[\p{Lu}]/iu.test("a") + "," + /[^\p{Lu}]/iu.test("a"); });
p(function () { return /\P{Lu}/iu.test("A") + "," + /\P{Ll}/iu.test("a") + "," + /[\P{Lu}]/u.test("A"); });
p(function () { return /\p{Ll}/iu.test("\u212a") + "," + /\p{Lu}/iu.test("\u017f") + "," + /^\p{Ll}$/iu.test("\u0130"); });
p(function () { return /\u212a/iu.test("k") + "," + /\u017f/iu.test("S") + "," + /\u0130/iu.test("i") + "," + /\u0131/iu.test("I"); });
p(function () { return /\u{10400}/iu.test("\u{10428}") + "," + /[\u{10428}]/iu.test("\u{10400}") + "," + /\u{1E900}/iu.test("\u{1E922}"); });
p(function () { return /\u017f/i.test("S") + "," + /\u212a/i.test("k") + "," + /[\u212a]/i.test("K"); });
// 34-48: early SyntaxErrors: misspellings, loose matching, unknown and unsupported properties.
p(function () { return re("\\p{letter}", "u"); });
p(function () { return re("\\p{Letter }", "u"); });
p(function () { return re("\\p{ Letter}", "u"); });
p(function () { return re("\\p{Script_Extensions=greek}", "u"); });
p(function () { return re("\\p{script_extensions=Greek}", "u"); });
p(function () { return re("\\p{Scx=Greek}", "u"); });
p(function () { return re("\\p{scx:Greek}", "u"); });
p(function () { return re("\\p{IsLetter}", "u"); });
p(function () { return re("\\p{Lowercase_Letter=Y}", "u"); });
p(function () { return re("\\p{ASCII=Y}", "u"); });
p(function () { return re("\\p{Block=Basic_Latin}", "u"); });
p(function () { return re("\\p{Line_Break=Alphabetic}", "u"); });
p(function () { return re("\\p{WSpace}", "u"); });
p(function () { return re("\\p{gc=Greek}", "u"); });
p(function () { return re("\\p{sc=Lu}", "u"); });
// 49-56: the escape's own grammar, and escaping.
p(function () { return re("\\pL", "u"); });
p(function () { return re("\\p{L", "u"); });
p(function () { return re("\\p{}", "u"); });
p(function () { return re("\\P", "u"); });
p(function () { return re("[\\p{L}-z]", "u"); });
p(function () { return /\\p\{L\}/u.test("\\p{L}") + "," + /[\\p]/u.test("p") + "," + re("\\\\p", "u").test("\\p"); });
p(function () { return re("\\\\\\p{L}", "u").test("\\x") + "," + re("^" + RegExp.escape("\\p{L}") + "$", "u").test("\\p{L}"); });
p(function () { return new Function("return /\\p{letter}/u")(); });
// 57-61: outside u, \p is the identity escape; properties of strings and the v flag stay refused.
p(function () { return /\p{L}/.test("p{L}") + "," + /\P/.test("P") + "," + /[\p{L}]/.test("{"); });
p(function () { return re("\\p{RGI_Emoji}", "u"); });
p(function () { return re("\\p{Basic_Emoji}", "u"); });
p(function () { return re("[\\p{L}]", "v").test("a"); });
p(function () { return re("\\p{L}", "uv"); });
// 62-66: the 28 Unicode 17.0 case pairs of JSD-0031 section 1, by name, then every \p{Lu} pair.
p(function () {
  var named = [[0xA7CE, 0xA7CF], [0xA7D2, 0xA7D3], [0xA7D4, 0xA7D5]];
  for (var cp = 0x16EA0; cp <= 0x16EB8; cp++) { named.push([cp, cp + 0x1B]); }
  var bad = [];
  for (var i = 0; i < named.length; i++) {
    var x = String.fromCodePoint(named[i][0]), y = String.fromCodePoint(named[i][1]);
    if (!(/\p{Lu}/iu.test(y) && re(esc(named[i][0]), "iu").test(y) && re("[" + esc(named[i][0]) + "]", "iu").test(y) &&
          re(esc(named[i][1]), "iu").test(x) && /^\p{Lu}$/u.test(x) && /^\p{Ll}$/u.test(y))) { bad.push(hex(named[i][0])); }
  }
  return named.length + " pairs, " + bad.length + " missed" + (bad.length ? ": " + bad.join(" ") : "");
});
p(function () { return misses(function (upper, lower) { return /^\p{Lu}$/u.test(String.fromCodePoint(upper)); }); });
p(function () { return misses(function (upper, lower) { return /^\p{Lu}$/iu.test(String.fromCodePoint(lower)); }); });
p(function () { return misses(function (upper, lower) { return re("^" + esc(upper) + "$", "iu").test(String.fromCodePoint(lower)); }); });
p(function () { return misses(function (upper, lower) { return re("^[" + esc(upper) + "]$", "iu").test(String.fromCodePoint(lower)); }); });
// 67-74: a greedy quantifier over one class is a run that gives characters back one at a time -
// by code point under u - and over a whole code space it needs no backtrack point per character.
p(function () { return /^\p{L}+$/u.test("a".repeat(1100000)) + "," + /^[^x]+$/.test("y".repeat(1100000)); });
p(function () { return /^.*(\ud83d\ude00)$/u.exec("a\u{1F600}\u{1F600}")[1].length + "," + /^.*(\ud83d\ude00)$/u.exec("a\u{1F600}\u{1F600}").index; });
p(function () { return /^[^x]*\ud83d/u.exec("\ud83d\ude00\ud83d")[0].length + "," + /^[^x]*\ude00/.exec("\ud83d\ude00\ud83d")[0].length; });
p(function () { return /^\p{L}{2,3}\p{L}/u.exec("abcde")[0] + "," + /^\p{L}{2,3}?\p{L}/u.exec("abcde")[0] + "," + /^[a-z]{2,}c/.exec("abcabc")[0]; });
p(function () { return /(?<=^\p{L}{2,4})\d/u.exec("abc1")[0] + "," + /(?<=(\p{L}+))\d/u.exec("ab\u{1D400}1")[1].length; });
p(function () { return /^[a-z]+$/iu.test("\u212a\u017f") + "," + /^[a-z]+$/i.test("\u212a\u017f") + "," + /^[\u212a]+$/iu.test("kK"); });
p(function () { return "aa1bb22".replace(/\p{L}*(\d)/gu, "<$1>") + "," + "x\u{1F600}\u{1F600}y".split(/\P{L}+/u).join("|"); });
p(function () { return /^(?:\p{Lu}|\p{Ll})+$/u.test("AbC".repeat(1000)) + "," + /^[\p{Lu}]{3}$/iu.test("abc"); });
