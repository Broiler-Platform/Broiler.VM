// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER RegExp.escape (JSeal slice F10).
//
// The pinned ES2026 RegExp.escape takes a String and nothing else - no coercion, so a String
// wrapper or an object with a toString is a TypeError - and escapes it code point by code point:
// a leading ASCII letter or digit as \xHH, a SyntaxCharacter or "/" with a backslash, the other
// punctuators ,-=<>#&!%:;@~'`" as \xHH, the five control escapes as \t \n \v \f \r, other white
// space, line terminators and lone surrogates as \xHH or \uHHHH, and everything else unchanged.
// The later cases check the point of the leading escape: the answer, placed after \c, \0 or a
// back reference, still matches the input literally.
//
// Each case prints its own number, so a divergence names a case rather than a line. A case that
// throws prints the error's name, because a refusal is an answer.

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function literal(s, flags) { return new RegExp("^" + RegExp.escape(s) + "$", flags).test(s); }
function ascii() { var s = ""; for (var i = 0; i < 128; i++) { s += String.fromCharCode(i); } return s; }
function units(s) { var out = []; for (var i = 0; i < s.length; i++) { out.push(s.charCodeAt(i).toString(16)); } return out.join(" "); }
p(function () { return typeof RegExp.escape; });
p(function () { return RegExp.escape.length + ":" + RegExp.escape.name; });
p(function () { var d = Object.getOwnPropertyDescriptor(RegExp, "escape"); return d.writable + "," + d.enumerable + "," + d.configurable; });
p(function () { return RegExp.escape(""); });
p(function () { return RegExp.escape("foo"); });
p(function () { return RegExp.escape("1st"); });
p(function () { return RegExp.escape("_foo"); });
p(function () { return RegExp.escape("Z9"); });
p(function () { return RegExp.escape("^$\\.*+?()[]{}|/"); });
p(function () { return RegExp.escape(",-=<>#&!%:;@~'`\""); });
p(function () { return RegExp.escape("\t\n\v\f\r"); });
p(function () { return RegExp.escape(" \u00a0\u1680\u2000\u200a\u202f\u205f\u3000\ufeff"); });
p(function () { return RegExp.escape("\u2028\u2029"); });
p(function () { var r = RegExp.escape("\ud800|\udfff|\ud83d\ude00"); return r.slice(0, -2) + " " + units(r.slice(-2)); });
p(function () { return units(RegExp.escape("\u00e9\u4e2d\u200b\u180e_")); });
p(function () { return RegExp.escape("a.b*c"); });
p(function () { return RegExp.escape(1); });
p(function () { return RegExp.escape(); });
p(function () { return RegExp.escape(null); });
p(function () { return RegExp.escape(new String("a")); });
p(function () { var called = false; try { RegExp.escape({ toString: function () { called = true; return "a"; } }); } catch (e) { return e.name + ":" + called; } });
p(function () { return RegExp.escape(Symbol("x")); });
p(function () { return new RegExp.escape("a"); });
p(function () { return RegExp.escape.call(undefined, "a.b"); });
p(function () { return literal(ascii(), ""); });
p(function () { return literal(ascii(), "u"); });
p(function () { return literal("\ud800a\udc00\ud83d\ude00\u2028\u3000", ""); });
p(function () { return literal("\ud800a\udc00\ud83d\ude00\u2028\u3000", "u"); });
p(function () { return new RegExp("[" + RegExp.escape("^a-z]\\") + "]+$", "u").exec("z-a^]\\")[0]; });
p(function () { return new RegExp("\\c" + RegExp.escape("a")).test("\\ca"); });
p(function () { return new RegExp("\\0" + RegExp.escape("1")).test("\u00001"); });
p(function () { return new RegExp("(a)\\1" + RegExp.escape("2")).test("aa2"); });
p(function () { return new RegExp("(a)\\1" + RegExp.escape("2"), "u").test("aa2"); });
p(function () { return new RegExp("\\x" + RegExp.escape("41")).test("\\x41"); });
p(function () { return new RegExp("a{" + RegExp.escape("2}")).test("a{2}"); });
p(function () { var s = "a.".repeat(20000); return RegExp.escape(s).length; });
