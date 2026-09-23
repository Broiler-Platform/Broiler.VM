// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER DIRECT EVAL AND THE CALLER'S BINDINGS (JSeal slice V14, JSD-0026
// steps 1-5).
//
// A direct eval evaluates its source in the caller's scope: it reads and writes the caller's
// bindings, captured ones included, sees `this`, `new.target` and `arguments` of the enclosing
// function, and every activation evaluates against its own records. An indirect eval, an optional
// call, the Function constructor and a locally shadowed `eval` do none of that. A strict evaluation
// and an evaluation's lexical declarations are isolated; a sloppy `var` or function declaration is
// its caller's since JSeal V15 (the-eval-declarations.js covers them), while `super` and private
// names are refused explicitly rather than answered. Each case prints its own number so a
// divergence names a case.
globalThis.x = "global";
var __n = 0;
function show(v) { return typeof v === "string" ? JSON.stringify(v) : String(v); }
function t(f) { try { return show(f()); } catch (e) { return e && e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function top(r) { __n++; print(__n + " " + r); }

// --- the acceptance cases: local reads and writes, captures, nesting
p(function () { function f() { let x = 7; return eval("x"); } return f(); });
p(function () { function f() { var x = 1; eval("x = 2"); return x; } return f(); });
p(function () { function f() { let x = 1; const g = () => x; eval("x = 5"); return g(); } return f(); });
p(function () { function f() { let x = 1; const h = eval("() => x"); x = 9; return h(); } return f(); });
p(function () { function f() { let x = 1; return eval("let x = 2; eval('x')"); } return f(); });
p(function () { function f(n) { let x = n; return n ? eval("f(n - 1) + x") : 0; } return f(3); });
p(function () { function mk(v) { return function () { return eval("v"); }; } return [mk(1)(), mk(2)()].join(); });
p(function () { function f(v) { return eval("() => v"); } var a = f("a"), b = f("b"); return a() + b(); });
p(function () { function f() { { eval("x"); let x; } } return f(); });
p(function () { function f() { const k = 1; eval("k = 2"); } return f(); });

// --- names nobody binds, and the global scope behind the caller's
p(function () { function f() { return eval("typeof nothingHere"); } return f(); });
p(function () { function f() { return eval("nothingHere"); } return f(); });
p(function () { function f() { "use strict"; eval("undeclaredStrict = 1"); } return f(); });
p(function () { function f() { eval("createdBySloppyEval = 4"); } f(); return typeof createdBySloppyEval; });
p(function () { function f() { return eval("x"); } return f(); });
p(function () { function f() { var x = "local"; return eval("x"); } return f(); });

// --- what is not a direct eval stays global, and a shadowed eval is an ordinary call
p(function () { function f() { var x = "local"; return (0, eval)("x"); } return f(); });
p(function () { function f() { var x = "local"; return eval?.("x"); } return f(); });
p(function () { function f() { var x = "local"; var e = eval; return e("x"); } return f(); });
p(function () { function f() { var eval = function (s) { return "g:" + s; }; return eval("x"); } return f(); });
p(function () { function f() { var x = "local"; return Function("return x")(); } return f(); });

// --- the spread spelling is direct too
p(function () { function f() { var x = "local"; return eval(...["x"]); } return f(); });
p(function () { function f() { return eval(...[]); } return f(); });
p(function () { function f() { var x = "local"; return eval(...["x", "ignored"]); } return f(); });

// --- a `with` record between the call and the binding
p(function () { function f() { var x = "local"; with ({ eval }) return eval("x"); } return f(); });
p(function () { function f() { var x = "local"; var eval = function () { return "g"; }; with ({ eval: globalThis.eval }) return eval("x"); } return f(); });
p(function () { function f() { var o = { y: "fromWith" }; with (o) return eval("y"); } return f(); });
p(function () { function f() { var o = { y: 1 }; with (o) eval("y = 2"); return o.y; } return f(); });
p(function () { function f() { var o = { m() { return this === o; } }; with (o) return eval("m()"); } return f(); });

// --- `arguments`, `this` and `new.target` of the enclosing function
p(function () { function f(a) { return eval("arguments[0]"); } return f("A"); });
p(function () { function f(a) { eval("arguments[0] = 2"); return a; } return f(1); });
p(function () { function f(a) { return (() => eval("arguments[0]"))(); } return f("arrowed"); });
p(function () { var o = { m() { return eval("this") === o; } }; return o.m(); });
p(function () { function F() { this.t = eval("new.target") === F; } return new F().t; });
p(function () { class B { } class D extends B { constructor() { var r; try { eval("this"); } catch (e) { r = e.name; } super(); this.r = r; } } return new D().r; });

// --- strictness and the evaluation's own declarations
p(function () { function f() { "use strict"; eval("var v = 1"); return typeof v; } return f(); });
p(function () { function f() { eval("'use strict'; var v = 1"); return typeof v; } return f(); });
p(function () { function f() { "use strict"; eval("with ({}) {}"); } return f(); });
p(function () { function f() { eval("let y = 1"); return typeof y; } return f(); });
p(function () { function f() { return eval("let z = 3; () => z")(); } return f(); });
p(function () { function f() { eval("class K { }"); return typeof K; } return f(); });
p(function () { function f() { "use strict"; eval("function g() { return 1; }"); return typeof g; } return f(); });

// --- a sloppy declaration (admitted by V15), and what is still refused by name
p(function () { function f() { eval("var y = 1"); return y; } return f(); });
p(function () { function f() { eval("function h() { }"); return typeof h; } return f(); });
p(function () { class C { x = eval("arguments"); } return new C().x; });
p(function () { class B { m() { return "b"; } } class D extends B { m() { return eval("super.m()"); } } return new D().m(); });
p(function () { class C { #p = 1; m() { return eval("this.#p"); } } return new C().m(); });
p(function () { function f() { return eval("this.#p"); } return f(); });

// --- the operators that reach a binding through a name
p(function () { function f() { var x = 1; return eval("delete x"); } return f(); });
p(function () { function f() { return eval("delete nothingHere"); } return f(); });
p(function () { function f() { var q = 1; return eval("typeof q"); } return f(); });
p(function () { function f() { var c = 1; eval("c += 2; c++"); return c; } return f(); });

// --- what the evaluation completes with, or throws
p(function () { function f() { var a = 2; return eval("if (a) { a * 10 }"); } return f(); });
p(function () { function f() { return eval(5); } return f(); });
p(function () { function f() { return eval("1 +"); } return f(); });
p(function () { function f() { return eval("throw new RangeError()"); } return f(); });

// --- functions made by the evaluation, and evaluations inside them
p(function () { function f() { var k = "kk"; return eval("(function () { return k; })")(); } return f(); });
p(function () { function f() { var a = "A"; return eval("(function () { var b = 'B'; return eval('a + b'); })")(); } return f(); });
p(function () { function* g() { var x = "gen"; yield eval("x"); } return g().next().value; });

// --- records inside a function: catch, per-iteration `let`, switch, block
p(function () { function f() { try { throw "c"; } catch (e) { return eval("e"); } } return f(); });
p(function () { function f() { var fs = []; for (let i = 0; i < 3; i++) fs.push(eval("() => i")); return fs.map(g => g()).join(); } return f(); });
p(function () { function f() { switch (1) { case 1: let s = "sw"; return eval("s"); } } return f(); });
p(function () { function f() { var x = "outer"; { let x = "inner"; return eval("x"); } } return f(); });
p(function () { function f(a = eval("1")) { return a; } return f(); });
p(function () { function f(a, b = eval("a")) { return b; } return f(3); });

// --- a script's own body: the global path where nothing lies between, a caller's record where one does
var r;
try { r = show(eval("x")); } catch (e) { r = e.name; } top(r);
try { { r = show(eval("x")); } } catch (e) { r = e.name; } top(r);
try { { let x = "block"; r = show(eval("x")); } } catch (e) { r = e.name; } top(r);
try { for (let i = 5; i < 6; i++) r = show(eval("i")); } catch (e) { r = e.name; } top(r);
try { try { throw "caught"; } catch (e) { r = show(eval("e")); } } catch (e) { r = e.name; } top(r);
try { with ({ x: "with" }) r = show(eval("x")); } catch (e) { r = e.name; } top(r);
try { switch (1) { case 1: let s = "sw"; r = show(eval("s")); } } catch (e) { r = e.name; } top(r);
try { { let y = 1; eval("y = 2"); r = show(y); } } catch (e) { r = e.name; } top(r);
try { { let b; eval("var declaredInABlock = 1"); r = typeof declaredInABlock; } } catch (e) { r = e.name; } top(r);
try { r = show(eval(...["x"])); } catch (e) { r = e.name; } top(r);
try { r = show(eval("new.target")); } catch (e) { r = e.name; } top(r);
