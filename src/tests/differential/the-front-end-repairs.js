// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER FRONT-END REPAIRS FOUND DURING JSEAL WAVES 1-2 (VM-FIX-D).
//
// A function declaration hoisted in a body sees that body's let, const and class bindings; a
// string followed by a line break is a directive only where a semicolon would be inserted; a
// tagged template's strings object is one per site and is kept where no guest can reach it;
// strict code refuses its reserved words as binding names, references and labels; a named function
// expression's own name is an immutable binding, through direct eval too; and a unary operator
// directly on the base of ** is an early error.
// Each case prints its own number so a divergence names a case rather than a line.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
var ev = eval;
function compiles(src) { try { ev("(function () {" + src + "\n})"); return "ok"; } catch (e) { return e.name; } }

// --- a hoisted function sees its body's lexical bindings (1-15)
p(function () { function outer() { const a = 1; function f() { return a; } return f(); } return outer(); });
p(function () { function outer() { let c = 0; function inc() { c++; } inc(); inc(); return c; } return outer(); });
p(function () { function outer() { function f() { return a; } try { f(); return "read"; } catch (e) { return e.name; } const a = 1; } return outer(); });
p(function () { function outer() { function f() { return new C().v; } class C { constructor() { this.v = 7; } } return f(); } return outer(); });
p(function () { function* g() { let n = 3; function f() { return n; } yield f(); } return g().next().value; });
p(function () { var arrow = () => { const k = 9; function f() { return k; } return f(); }; return arrow(); });
p(function () { function outer() { "use strict"; const a = 2; function f() { return a * 2; } return f(); } return outer(); });
p(function () { function outer() { let x = 1; { function g() { return x; } } return g(); } return outer(); });
p(function () { function outer() { "use strict"; let x = 1; { let y = 2; function g() { return x + y; } return g(); } } return outer(); });
p(function () { function outer(v) { switch (v) { case 1: let q = 5; function h() { return q; } return h(); } } return outer(1); });
p(function () { function outer() { const z = 1; function w() { try { z = 2; } catch (e) { return e.name; } return "wrote"; } return w(); } return outer(); });
p(function () { function outer(a = function () { return typeof b; }) { let b = 1; return a(); } return outer(); });
p(function () { function outer() { const base = 10; function even(n) { return n === 0 ? base : odd(n - 1); } function odd(n) { return n === 0 ? -base : even(n - 1); } return even(4); } return outer(); });
p(function () { function outer() { const a = "o"; function mid() { const b = "m"; function inner() { return a + b; } return inner(); } return mid(); } return outer(); });
p(function () { return ev("let evLex = 4; function evFn() { return evLex; } evFn()"); });

// --- a line break ends a directive only where a semicolon would be inserted (16-24)
p(function () { return ev('"a"\n+ 1'); });
p(function () { return ev('"a"\n.length'); });
p(function () { return ev('"ab"\n[1]'); });
p(function () { return ev('"use strict"\n+ 1'); });
p(function () { return ev('"a"\n? 1 : 2'); });
p(function () { return ev('"a"\n, 5'); });
p(function () { return compiles('"use strict"\nvar public = 1;'); });
p(function () { return ev('"a"\n"use strict"\n; (function () { return this === undefined; })()'); });
p(function () { return ev('(function () { "a"\n+ "b"; return typeof this; })()'); });

// --- one strings object per tagged-template site, out of the guest's reach (25-34)
function tag(s) { return s; }
globalThis.tag = tag;
var namesBefore = Object.getOwnPropertyNames(globalThis).length;
function site() { return tag`a${1}b\n`; }
p(function () { return site() === site(); });
p(function () { return Object.getOwnPropertyNames(globalThis).length === namesBefore; });
p(function () { var s = site(); return Object.isFrozen(s) + " " + Object.isFrozen(s.raw) + " " + Array.isArray(s.raw); });
p(function () { var d = Object.getOwnPropertyDescriptor(site(), "raw"); return d.writable + " " + d.enumerable + " " + d.configurable; });
p(function () { return Object.keys(site()).join(); });
p(function () { return tag`x` === tag`x`; });
p(function () { var saved = Object.freeze; Object.freeze = function () { throw new Error("called"); }; try { return Object.isFrozen(tag`z`); } finally { Object.freeze = saved; } });
p(function () { return ev("tag`q`") === ev("tag`q`"); });
p(function () { var g = ev("(function () { return tag`q`; })"); return g() === g(); });
p(function () { var s = site(); return JSON.stringify(s) + " " + JSON.stringify(s.raw) + " " + s.length; });

// --- strict code's reserved words are not binding names (35-45)
var words = ["implements", "interface", "package", "private", "protected", "public", "static", "let", "yield", "eval", "arguments"];
p(function () { return words.map(function (w) { return compiles('"use strict"; var ' + w + ";"); }).join(); });
p(function () { return words.map(function (w) { return compiles("var " + w + " = 1;"); }).join(); });
p(function () { return compiles('function public() { "use strict"; }'); });
p(function () { return compiles('function f(static) { "use strict"; }'); });
p(function () { return compiles("class implements {}"); });
p(function () { return compiles('"use strict"; var { public } = {};'); });
p(function () { return compiles('"use strict"; try { } catch (private) { }'); });
p(function () { return compiles('"use strict"; let package = 1;'); });
p(function () { return compiles('"use strict"; ({ public: 1, static() { } }).public;'); });
p(function () { return compiles("class A { static static() { } }"); });
p(function () { try { ev('"use strict"; var interface = 1;'); return "ok"; } catch (e) { return e.name; } });

// --- a named function expression's own name is immutable (46-57)
p(function () { var f = function g() { g = 1; return typeof g; }; return f(); });
p(function () { var f = function g() { g += 1; return typeof g; }; return f(); });
p(function () { var f = function g() { g++; return typeof g; }; return f(); });
p(function () { var f = function g() { [g] = [1]; return typeof g; }; return f(); });
p(function () { var f = function g() { for (g in { a: 1 }); return typeof g; }; return f(); });
p(function () { var f = function g() { "use strict"; try { g = 1; } catch (e) { return e.constructor === TypeError; } return "wrote"; }; return f(); });
p(function () { var f = function g() { "use strict"; try { g++; } catch (e) { return e.name; } return "wrote"; }; return f(); });
p(function () { var f = function g() { var g = 5; g = 6; return g; }; return f(); });
p(function () { var f = function g() { return (function () { g = 2; return typeof g; })(); }; return f(); });
p(function () { var f = function g() { var r = (g = 3); return r; }; return f(); });
p(function () { var f = function g() { ({ a: g } = { a: 1 }); return typeof g; }; return f(); });
p(function () { var f = function g(x) { if (x) { return typeof g; } { let g = 1; g = 2; } return f(true); }; return f(); });

// --- a unary operator directly on the base of ** is an early error (58-60)
p(function () {
  return ["-2 ** 2", "(-2) ** 2", "-(2 ** 2)", "2 ** -2", "+1 ** 2", "!1 ** 2", "~1 ** 2", "typeof 1 ** 2",
    "void 1 ** 2", "delete o ** 2", "2 ** 3 ** 2", "(-2) ** 3", "1 * -2 ** 2", "-2 * 2 ** 2"].map(function (src) {
    try { return String(ev(src)); } catch (e) { return e.name; }
  }).join(" | ");
});
p(function () { return compiles("(async function () { await 1 ** 2; })"); });
p(function () { return compiles("(async function () { (await 1) ** 2; })"); });

// --- a named function expression's own name, written from direct eval code (61-65)
p(function () { var f = function g() { eval("g = 1"); return typeof g; }; return f(); });
p(function () { var f = function g() { "use strict"; try { eval("g = 1"); } catch (e) { return e.constructor === TypeError; } return "wrote"; }; return f(); });
p(function () { var f = function g() { eval("g += 1; g++; [g] = [2];"); return typeof g; }; return f(); });
p(function () { var f = function g() { return eval("(function () { g = 5; return typeof g; })()"); }; return f(); });
p(function () { var f = function g() { return eval("'use strict'; try { g = 1; 'wrote' } catch (e) { e.name }"); }; return f(); });

// --- strict code's reserved words are not identifier references or labels either (66-68)
p(function () {
  return ['"use strict"; public = 1', '"use strict"; var x = static;', '"use strict"; label: public: ;',
    '"use strict"; ({ public } = {})', '"use strict"; ({ implements = 1 } = {})', '"use strict"; let;',
    '"use strict"; l\\u0065t = 1', '"use strict"; st\\u0061tic: ;', 'class B { m() { return private; } }',
    'function g() { "use strict"; return package; }'].map(compiles).join(" | ");
});
p(function () {
  return ['public = 1; var static = 2; package: ;', 'var o = { interface: 1 }; o.interface',
    '"use strict"; ({ public: 1 }).public;', '"use strict"; class A { static public() { } }'].map(compiles).join(" | ");
});
p(function () { return ev("var implements = 3; implements"); });
