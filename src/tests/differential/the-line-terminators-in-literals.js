// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER LINE TERMINATORS INSIDE LITERALS (JSeal J17, upstream half).
//
// U+2028 and U+2029 are string characters since ES2019 (the JSON superset) and are still line
// terminators everywhere else; a backslash before any line terminator is a line continuation that
// adds nothing to a string or a cooked template and is kept by a raw one. Values are printed as
// code units so that no answer depends on the console's code page. The `String.raw` sources
// start at different columns because this build keys a template object by its source position,
// so two evaluated sources with a tagged template at the same position share one object.
// Cases 20 to 23 hold the separators and continuations literally in this file rather than
// through `eval`.

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
var ev = eval; // indirect, because a direct eval inside a function is refused by this profile
function codes(s) { var out = []; for (var i = 0; i < s.length; i++) out.push(s.charCodeAt(i)); return out.join(","); }
p(function () { return ev("('\u2028')").length; });
p(function () { return ev("('\u2029')").charCodeAt(0); });
p(function () { return ev('("\u2028\u2029")') === "\u2028\u2029"; });
p(function () { return codes(ev("('a\\\u2028b')")); });
p(function () { return codes(ev("('a\\\u2029b')")); });
p(function () { return codes(ev("String.raw`a\\\nb`")); });
p(function () { return codes(ev(" String.raw`a\\\rb`")); });
p(function () { return codes(ev("  String.raw`a\\\r\nb`")); });
p(function () { return codes(ev("   String.raw`a\\\u2028b`")); });
p(function () { return codes(ev("`a\\\nb`")); });
p(function () { return codes(ev("`a\\\u2028b`")); });
p(function () { return codes(ev("`${'a\\\nb'}`")); });
p(function () { return codes(ev("`${'a\u2028b'}`")); });
p(function () { return ev("var q = 1\u2028q"); });
p(function () { return new Function("return '\u2029'.length")(); });
p(function () { return ev("('a\nb')"); });
p(function () { return ev("('a\rb')"); });
p(function () { return ev("/*\u2028*/ 5"); });
p(function () { return ev("// c\u2028 7"); });
p(function () { return codes('x y'); });
p(function () { return codes("x y"); });
p(function () { return (function (s) { return s.raw[0].length + ':' + s[0].length; })`a\
b`; });
p(function () { return codes(`${'a b'}`); });
// A directive is an expression statement, so its string is the completion value of the program
// it begins unless a later statement yields one; cases 24 to 28 are that value through `eval`.
p(function () { return ev("'a'"); });
p(function () { return codes(ev("'\u2028'")); });
p(function () { return ev("'a'; var z = 1; function g() {}"); });
p(function () { return ev("'use strict'; 'b'"); });
p(function () { return ev("'a'; 1"); });
