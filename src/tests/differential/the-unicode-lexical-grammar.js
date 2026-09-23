// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE UNICODE HALF OF THE LEXICAL GRAMMAR (JSeal slice JSD-0031-later).
//
// The tokenizer classifies identifier characters with the pinned Unicode 17.0.0 ID_Start and
// ID_Continue - literal ones by code point, supplementary ones included, and escaped ones, which
// were not classified at all before - and WhiteSpace as TAB, VT, FF, ZWNBSP and Zs rather than the
// platform's char.IsWhiteSpace. Regular-expression group names read the same two sets. A literal's
// flags are an early error, and a well-formed v is refused by name at compile time. Without u, the
// matcher's Canonicalize is UnicodeData.txt's simple upper case of one code unit (not the
// platform's Unicode 16 data); under u a lastIndex inside a surrogate pair starts the match at the
// pair.
//
// Source text holding a non-ASCII identifier is built with String.fromCodePoint, and every answer
// is ASCII, so the probe reads the same on any console code page. The last cases walk every BMP
// code unit that the ES2026 non-u Canonicalize moves - 1,169 of them, listed as runs
// [first, count, stride, delta] generated once with Node v24.17.0 (Unicode 17.0) from
// toUpperCase with the specification's two rules - and report every one /X/i or /[X]/i misses.
//
// Each case prints its own number, so a divergence names a case rather than a line. A case that
// throws prints the error's name, because a refusal is an answer.

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function run(source) { return (0, eval)(source); }
function ch(cp) { return String.fromCodePoint(cp); }
function hex(cp) { return cp.toString(16).toUpperCase(); }
function u4(c) { return "\\u" + ("000" + hex(c)).slice(-4); }
function compiles(source) { try { (0, eval)("(function () {" + source + "\n})"); return true; } catch (e) { return false; } }

// 1-18: escaped identifiers are held to ID_Start / ID_Continue like literal ones.
p(function () { return run("var \\u0021 = 1"); });
p(function () { return run("var \\u0030x = 1"); });
p(function () { return run("var \\u{1F600} = 1"); });
p(function () { return run("var x\\u{1F600} = 1"); });
p(function () { return run("var \\u{1D400} = 7; \\u{1D400}"); });
p(function () { return run("var \\uD835\\uDC00 = 1"); });
p(function () { return run("var \\uD800 = 1"); });
p(function () { return run("var \\u200D = 1"); });
p(function () { return run("var x\\u200D = 3; x\\u200D"); });
p(function () { return run("var \\u0024\\u005F = 4; $_"); });
p(function () { return run("var a\\u{1D7CE} = 8; a\\u{1D7CE}"); });
p(function () { return run("var \\u{1D7CE} = 1"); });
p(function () { return run("var \\u2E2F = 1"); });
p(function () { return run("var \\u2118 = 6; \\u2118"); });
p(function () { return run("var x\\u00B7 = 6; x\\u00B7"); });
p(function () { return run("var \\u00B7 = 6"); });
p(function () { return run("var \\uA7CE = 9; \\uA7CE"); });
p(function () { return run("var \\u{16EA0} = 1; \\u{16EA0}"); });

// 19-26: an escape does not make a reserved word a name, and does not stop a name being a key.
p(function () { return run("var v\\u0061r = 1"); });
p(function () { return run("var \\u0069f = 1"); });
p(function () { return run("({ \\u0069f: 1 }).if"); });
p(function () { return run("var o = { if: 2 }; o.\\u0069f"); });
p(function () { return run("'use strict'; var l\\u0065t = 1"); });
p(function () { return run("var l\\u0065t = 5; let"); });
p(function () { return run("\\u0074his"); });
p(function () { return run("n\\u0065w Object()"); });

// 27-30: private names: the character after `#` is held to ID_Start, escaped or not.
p(function () { return run("class C { #\\u0061 = 1; m() { return this.#a; } } new C().m()"); });
p(function () { return run("class C { #\\u0031 = 1 }"); });
p(function () { return run("class C { #1 = 1 }"); });
p(function () { return run("class C { #" + ch(0x1D400) + " = 1; m() { return this.#" + ch(0x1D400) + "; } } new C().m()"); });

// 31-40: literal identifiers, by code point.
p(function () { return run("var " + ch(0x1D400) + " = 7; " + ch(0x1D400)); });
p(function () { return run("var x" + ch(0x1D7CE) + " = 8; x" + ch(0x1D7CE)); });
p(function () { return run("var " + ch(0x1D7CE) + " = 8"); });
p(function () { return run("var " + ch(0x1F600) + " = 1"); });
p(function () { return run("var " + ch(0xA7CE) + " = 9; " + ch(0xA7CE)); });
p(function () { return run("var x" + ch(0x30FB) + " = 6; x" + ch(0x30FB)); });
p(function () { return run("var x" + ch(0xFF65) + " = 6; x" + ch(0xFF65)); });
p(function () { return run("var " + ch(0x16EA0) + " = 1; " + ch(0x16EA0)); });
p(function () { return run("var " + ch(0x2E2F) + " = 1"); });
p(function () { return run("3" + ch(0x1D400)); });

// 41-42: every 67th code point: a literal start or part agrees with \p{ID_Start} / \p{ID_Continue}.
// The members are declared in one program, which compiles only if every one is admitted; the
// non-members are tried one at a time from every 1999th code point, because each is a separate
// load and the host's default allowance admits 4,096 of them.
function sweep(prefix, set, first) {
  var members = [], bad = [];
  for (var cp = first; cp <= 0x10FFFF; cp += 67) {
    if ((cp < 0xD800 || cp > 0xDFFF) && set.test(ch(cp))) members.push(prefix + ch(cp));
  }
  if (!compiles("var " + members.join(", ") + ";")) bad.push("members");
  for (var cp = first; cp <= 0x10FFFF; cp += 1999) {
    if ((cp < 0xD800 || cp > 0xDFFF) && !set.test(ch(cp)) && compiles("var " + prefix + ch(cp) + ";")) bad.push(hex(cp));
  }
  return members.length + " " + bad.length + (bad.length ? " " + bad.slice(0, 8).join(" ") : "");
}
p(function () { return sweep("", /^[$_\p{ID_Start}]$/u, 0x80); });
p(function () { return sweep("a", /^[$\u200C\u200D\p{ID_Continue}]$/u, 0x81); });

// 43-52: WhiteSpace is TAB, VT, FF, ZWNBSP and Zs; U+0085 and the separators U+001C-U+001F are not.
p(function () { return run("1" + ch(0x85) + "+1"); });
p(function () { return run("1" + ch(0x1C) + "+1"); });
p(function () { return run("1" + ch(0x180E) + "+1"); });
p(function () { return run("1" + ch(0x200B) + "+1"); });
p(function () { return run("1" + ch(0x1680) + "+1"); });
p(function () { return run("1" + ch(0x3000) + "+1"); });
p(function () { return run("1" + ch(0xFEFF) + "+1"); });
p(function () { return run("1" + ch(0x202F) + "+1"); });
p(function () { return run("var a = 1" + ch(0x2028) + "a"); });
p(function () { return run("var a = 1" + ch(0x85) + "a"); });

// 53: every candidate code point that separates two tokens, as the list of those that do.
p(function () {
  var ranges = [[0, 0x20], [0x7F, 0xA0], [0x1680, 0x1680], [0x180E, 0x180E], [0x2000, 0x200F],
    [0x2028, 0x202F], [0x205F, 0x2064], [0x3000, 0x3000], [0xFEFF, 0xFEFF]];
  var found = [];
  for (var r = 0; r < ranges.length; r++) {
    for (var cp = ranges[r][0]; cp <= ranges[r][1]; cp++) {
      var answer;
      try { answer = (0, eval)("1" + ch(cp) + "+1"); } catch (e) { answer = null; }
      if (answer === 2) found.push(hex(cp));
    }
  }
  return found.join(" ");
});

// 54-61: a literal's flags are an early error; a well-formed v is refused by name.
p(function () { return run("/a/v.flags"); });
p(function () { return compiles("if (false) { /a/v }"); });
p(function () { return compiles("if (false) { /a/x }"); });
p(function () { return compiles("if (false) { /a/gg }"); });
p(function () { return compiles("if (false) { /a/uv }"); });
p(function () { return run("/a/dgimsuy.flags"); });
p(function () { return compiles("/a/g" + ch(0x1D400)); });
p(function () { return new RegExp("a", "v").flags; });

// 62-75: group names read the same ID_Start / ID_Continue.
function group(name, flags) { return new RegExp("(?<" + name + ">a)", flags).exec("a").groups[name.indexOf("\\") < 0 ? name : "x"]; }
p(function () { return group(ch(0x2118)); });
p(function () { return group(ch(0x2160)); });
p(function () { return group("x" + ch(0x200D)); });
p(function () { return group(ch(0x1D400)); });
p(function () { return group(ch(0x1D400), "u"); });
p(function () { return new RegExp("(?<\\u{1D400}>a)").exec("a").groups[ch(0x1D400)]; });
p(function () { return new RegExp("(?<\\uD835\\uDC00>a)").exec("a").groups[ch(0x1D400)]; });
p(function () { return group(ch(0xA7CE)); });
p(function () { return group(ch(0x16EA0), "u"); });
p(function () { return group(ch(0x2E2F)); });
p(function () { return group(ch(0x661)); });
p(function () { return group("x" + ch(0x661)); });
p(function () { return group(ch(0x1F600)); });
p(function () { return group("x" + ch(0x30FB)); });

// 76-87: the non-u Canonicalize.
p(function () { return /\uA7CE/i.test("\uA7CF"); });
p(function () { return /[\uA7CE]/i.test("\uA7CF"); });
p(function () { return /\uA7CF/i.test("\uA7CE"); });
p(function () { return /[\uA7D2\uA7D4]/i.test("\uA7D3") && /[\uA7D2\uA7D4]/i.test("\uA7D5"); });
p(function () { return /\u017F/i.test("S"); });
p(function () { return /\u212A/i.test("k"); });
p(function () { return /\u00B5/i.test("\u039C"); });
p(function () { return /\u0131/i.test("I"); });
p(function () { return /[\u03C3]/i.test("\u03C2"); });
p(function () { return /\u1F80/i.test("\u1F88"); });
p(function () { return /\u00DF/i.test("\u1E9E"); });
p(function () { return /[^\uA7CE]/i.test("\uA7CF"); });

// 88-89: every BMP code unit the non-u Canonicalize moves, and whether /X/i and /[X]/i reach its form.
var CANON = [
  [97,26,1,-32],[181,1,1,743],[224,23,1,-32],[248,7,1,-32],[255,1,1,121],[257,24,2,-1],
  [307,3,2,-1],[314,8,2,-1],[331,23,2,-1],[378,3,2,-1],[384,1,1,195],[387,2,2,-1],[392,2,4,-1],
  [402,1,1,-1],[405,1,1,97],[409,1,1,-1],[410,1,1,163],[411,1,1,42561],[414,1,1,130],[417,3,2,-1],
  [424,2,5,-1],[432,2,4,-1],[438,2,3,-1],[445,1,1,-1],[447,1,1,56],[453,1,1,-1],[454,1,1,-2],
  [456,1,1,-1],[457,1,1,-2],[459,1,1,-1],[460,1,1,-2],[462,8,2,-1],[477,1,1,-79],[479,9,2,-1],
  [498,1,1,-1],[499,1,1,-2],[501,2,4,-1],[507,19,2,-1],[547,9,2,-1],[572,1,1,-1],[575,2,1,10815],
  [578,2,5,-1],[585,4,2,-1],[592,1,1,10783],[593,1,1,10780],[594,1,1,10782],[595,1,1,-210],
  [596,1,1,-206],[598,2,1,-205],[601,1,1,-202],[603,1,1,-203],[604,1,1,42319],[608,1,1,-205],
  [609,1,1,42315],[611,1,1,-207],[612,1,1,42343],[613,1,1,42280],[614,1,1,42308],[616,1,1,-209],
  [617,1,1,-211],[618,1,1,42308],[619,1,1,10743],[620,1,1,42305],[623,1,1,-211],[625,1,1,10749],
  [626,1,1,-213],[629,1,1,-214],[637,1,1,10727],[640,1,1,-218],[642,1,1,42307],[643,1,1,-218],
  [647,1,1,42282],[648,1,1,-218],[649,1,1,-69],[650,2,1,-217],[652,1,1,-71],[658,1,1,-219],
  [669,1,1,42261],[670,1,1,42258],[837,1,1,84],[881,2,2,-1],[887,1,1,-1],[891,3,1,130],
  [940,1,1,-38],[941,3,1,-37],[945,17,1,-32],[962,1,1,-31],[963,9,1,-32],[972,1,1,-64],
  [973,2,1,-63],[976,1,1,-62],[977,1,1,-57],[981,1,1,-47],[982,1,1,-54],[983,1,1,-8],
  [985,12,2,-1],[1008,1,1,-86],[1009,1,1,-80],[1010,1,1,7],[1011,1,1,-116],[1013,1,1,-96],
  [1016,2,3,-1],[1072,32,1,-32],[1104,16,1,-80],[1121,17,2,-1],[1163,27,2,-1],[1218,7,2,-1],
  [1231,1,1,-15],[1233,48,2,-1],[1377,38,1,-48],[4304,43,1,3008],[4349,3,1,3008],[5112,6,1,-8],
  [7296,1,1,-6254],[7297,1,1,-6253],[7298,1,1,-6244],[7299,2,1,-6242],[7301,1,1,-6243],
  [7302,1,1,-6236],[7303,1,1,-6181],[7304,1,1,35266],[7306,1,1,-1],[7545,1,1,35332],
  [7549,1,1,3814],[7566,1,1,35384],[7681,75,2,-1],[7835,1,1,-59],[7841,48,2,-1],[7936,8,1,8],
  [7952,6,1,8],[7968,8,1,8],[7984,8,1,8],[8000,6,1,8],[8017,4,2,8],[8032,8,1,8],[8048,2,1,74],
  [8050,4,1,86],[8054,2,1,100],[8056,2,1,128],[8058,2,1,112],[8060,2,1,126],[8112,2,1,8],
  [8126,1,1,-7205],[8144,2,1,8],[8160,2,1,8],[8165,1,1,7],[8526,1,1,-28],[8560,16,1,-16],
  [8580,1,1,-1],[9424,26,1,-26],[11312,48,1,-48],[11361,1,1,-1],[11365,1,1,-10795],
  [11366,1,1,-10792],[11368,3,2,-1],[11379,2,3,-1],[11393,50,2,-1],[11500,2,2,-1],[11507,1,1,-1],
  [11520,38,1,-7264],[11559,2,6,-7264],[42561,23,2,-1],[42625,14,2,-1],[42787,7,2,-1],
  [42803,31,2,-1],[42874,2,2,-1],[42879,5,2,-1],[42892,2,5,-1],[42899,1,1,-1],[42900,1,1,48],
  [42903,10,2,-1],[42933,8,2,-1],[42952,2,2,-1],[42957,8,2,-1],[42998,1,1,-1],[43859,1,1,-928],
  [43888,80,1,-38864],[65345,26,1,-32]
];
function walk(classed) {
  var missed = [], total = 0;
  for (var r = 0; r < CANON.length; r++) {
    for (var k = 0; k < CANON[r][1]; k++) {
      var c = CANON[r][0] + k * CANON[r][2], form = c + CANON[r][3];
      var source = classed ? "[" + u4(c) + "]" : u4(c);
      total++;
      if (!new RegExp(source, "i").test(String.fromCharCode(form)) ||
          !new RegExp(classed ? "[" + u4(form) + "]" : u4(form), "i").test(String.fromCharCode(c))) missed.push(hex(c));
    }
  }
  return total + " " + missed.length + (missed.length ? " " + missed.slice(0, 8).join(" ") : "");
}
p(function () { return walk(false); });
p(function () { return walk(true); });

// 90-95: under u, lastIndex inside a surrogate pair starts the match at the pair.
function exec(re, s, last) { re.lastIndex = last; var m = re.exec(s); return m === null ? "null " + re.lastIndex : m.index + " " + m[0].length + " " + re.lastIndex; }
p(function () { return exec(/./gu, "\uD834\uDF06", 1); });
p(function () { return exec(/./yu, "\uD834\uDF06", 1); });
p(function () { return exec(/\uDF06/gu, "\uD834\uDF06", 1); });
p(function () { return exec(/(?:)/gu, "\uD834\uDF06", 1); });
p(function () { return exec(/\uD834\uDF06/gu, "a\uD834\uDF06", 2); });
p(function () { return exec(/./g, "\uD834\uDF06", 1); });
