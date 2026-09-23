// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER A FUNCTION BODY'S SEPARATE VARIABLE ENVIRONMENT (JSeal V15-finish,
// FunctionDeclarationInstantiation step 28, JSD-0026 section 14).
//
// Once a parameter list has expressions, the specification gives the body a variable environment
// of its own: a body `var` of a parameter's name starts with the parameter's value and is then a
// different binding, and a closure the parameter list made - in its own syntax or through the
// source a parameter-list eval evaluated - sees the parameters and never the body's declarations.
// A direct eval in such a body declares into the body's environment, and one in the parameter list
// declares into the function's. Each case prints its own number so a divergence names a case.
var __n = 0;
function show(v) { return typeof v === "string" ? JSON.stringify(v) : String(v); }
function t(f) { try { return show(f()); } catch (e) { return e && e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// --- closures in the parameter list do not see the body's declarations
p(function () { function f(g = () => typeof x) { var x = 1; return g(); } return f(); });
p(function () { function f(a, g = () => a) { var a = 2; return [a, g()].join(); } return f(1); });
p(function () { function f(a, g = () => a) { var a; return [a, g()].join(); } return f(1); });
p(function () { function f(a, g = () => a) { function a() { } return [typeof a, g()].join(); } return f(1); });
p(function () { function f(a, g = () => { a = 5; }) { var a; g(); return a; } return f(1); });
p(function () { function f(a, g = () => { a = 5; }) { g(); return a; } return f(1); });
p(function () { function f(g = () => arguments) { var arguments; return [typeof arguments, g() === arguments].join(); } return f(); });
p(function () { function f(a, g = () => a) { let b = 2; return g() + b; } return f(1); });
p(function () { function f(g = () => yy) { let yy = 1; return g(); } return f(); });
p(function () { function f(C = class { m() { return typeof x; } }) { var x = 1; return new C().m(); } return f(); });
p(function () { function f(o = { m() { return typeof x; } }) { var x = 1; return o.m(); } return f(); });
p(function () { function* g(a, h = () => a) { var a = 2; yield [a, h()].join(); } return g(1).next().value; });
p(function () { var f = (a, g = () => a) => { var a = 2; return [a, g()].join(); }; return f(1); });
p(function () { return ({ m(a, g = () => a) { var a = 2; return [a, g()].join(); } }).m(1); });
p(function () { function f({ a, g = () => a }) { var a = 2; return [a, g()].join(); } return f({ a: 1 }); });
p(function () { "use strict"; function f(a, g = () => a) { var a = 2; return [a, g()].join(); } return f(1); });
p(function () { function f(n, g = () => n) { var n = n - 1; return n > 0 ? f(n) + g() : g(); } return f(3); });
p(function () { function f(a, g = () => a) { var r = []; for (let i = 0; i < 2; i++) { r.push(() => i + a); } try { throw 1; } catch (e) { var a = 10; } return [r[0](), r[1](), g()].join(); } return f(1); });
p(function () { function f(a, b = a + 1, g = () => b) { var b; return [b, g()].join(); } return f(1); });
p(function () { var o = {}; function f(a, g = () => this) { return [this === o, g() === o, arguments.length].join(); } o.f = f; return o.f(5); });
p(function () { function f(a, g = () => 0) { var v = 3; function h() { return v + a; } return h(); } return f(1); });
p(function () { function f(g = () => typeof x) { eval("var x = 1"); return [x, g()].join(); } return f(); });
p(function () { function f(a = 1, g = () => a) { a = 7; return [a, g()].join(); } return f(); });
p(function () { function f(a, g = () => a) { var a = 2; { function a() { } } return [typeof a, g()].join(); } return f(1); });

// --- a direct eval in the body of a function whose parameter list may call eval
p(function () { function f(a = eval("() => typeof x")) { eval("var x = 1"); return a(); } return f(); });
p(function () { function f(a = eval("function pz() { return typeof x; }")) { eval("var x = 1"); return pz(); } return f(); });
p(function () { function f(a = eval("1")) { eval("var q = 3"); return q; } return f(); });
p(function () { function f(a = eval("var pv = 1"), g = () => pv) { eval("var pv = 2"); return [pv, g()].join(); } return f(); });
p(function () { function f(a = eval("var pv = 1")) { return eval("pv"); } return f(); });
p(function () { function f(a, b = eval("a")) { eval("var a = 5"); return [a, b].join(); } return f(1); });
p(function () { function f(a, b = eval("0")) { eval("var a"); return a; } return f(1); });
p(function () { function f(a = 0, g = eval("() => a")) { var a = 2; eval("a = 3"); return [a, g()].join(); } return f(1); });
p(function () { function f(a = eval("var pv = 1")) { eval("var pv = 2"); delete pv; return pv; } return f(); });
p(function () { function f(a = eval("0")) { let l = 1; eval("var l"); } return f(); });
p(function () { function f(a = eval("0")) { eval("let l2 = 1"); return typeof l2; } return f(); });
p(function () { function f(a = eval("0")) { var fromArgs = eval("arguments.length"); return fromArgs; } return f(1, 2); });

// --- a parameter-list eval in a function whose body declares a parameter's name again
p(function () { function f(a, b = eval("() => a")) { var a = 2; return b(); } return f(1); });
p(function () { function f(a, b = eval("(function () { return a; })")) { var a = 2; return b(); } return f(1); });
p(function () { function f(a, b = eval("() => a")) { function a() { } return typeof b(); } return f(1); });
p(function () { function f(a, b = eval("() => arguments.length")) { var arguments; return b(); } return f(1); });
p(function () { function f(a, b = eval("() => a")) { var a; a = 3; return [a, b()].join(); } return f(1); });
p(function () { function f(a, b = eval("() => a")) { var a; function g() { a = 3; } g(); return [a, b()].join(); } return f(1); });
p(function () { function f(p = eval("var arguments = 'param'")) { function arguments() { } return typeof arguments; } return f(); });
p(function () { function f(a, b = eval("var e1 = a")) { var e1; return e1; } return f(4); });

// --- a body direct eval whose parameter list makes no closure still declares into the body's environment
p(function () { function f(a = 1) { eval("var a"); return a; } return f(); });
p(function () { function f(a = 1) { eval("var arguments"); return typeof arguments; } return f(); });
p(function () { function f(a = 1) { eval("var a = 2"); return a; } return f(); });
p(function () { function f(a = 1) { var a; eval("a = 3"); return a; } return f(); });
p(function () { function f(a = 1) { return eval("a"); } return f(); });

// --- the body's record under the rest of the language
p(function () { function f(a, g = () => a) { { function a() { } } return [typeof a, g()].join(); } return f(1); });
p(function () { function f(a, g = () => a) { var a = 2; try { return a; } finally { a = 3; } } return f(1); });
p(function () { function f(a, g = () => a) { var s = 0; for (var v of [1, 2]) s += v; return s + g(); } return f(1); });
p(function () { function* g(a, h = () => a) { var a = 2; yield a; a = 3; yield [a, h()].join(); } var it = g(1); it.next(); return it.next().value; });
p(function () { class C { x = 1; constructor(a, g = () => a) { var a = 2; this.r = [a, g(), this.x].join(); } } return new C(1).r; });
p(function () { class B0 { } class D extends B0 { constructor(a, g = () => a) { var a = 2; super(); this.r = [a, g()].join(); } } return new D(1).r; });
p(function () { function f(a, g = () => a) { arguments[0] = 9; return [a, g()].join(); } return f(1); });
p(function () { function f(a, g = () => a) { var a = 2; return (() => [a, g(), new.target === undefined].join())(); } return f(1); });
p(function () { function f(a, g = () => a) { var a = 2; with ({ a: 5 }) { return [a, g()].join(); } } return f(1); });
p(function () { function f(a, [b] = [() => a]) { var a = 2; return b(); } return f(1); });
p(function () { function f(a, { b = () => a } = {}) { var a = 2; return b(); } return f(1); });
p(function () { function f(a, g = () => a) { var a = 2; let h = () => a; return [h(), g()].join(); } return f(1); });

// --- a parameter list without expressions gives the body no environment of its own
p(function () { function f(...r) { eval("var r"); return r.length; } return f(5); });
p(function () { function f({ a }) { eval("var a"); return a; } return f({ a: 3 }); });
p(function () { function f([a], { b: [c] }) { eval("var a, c"); return [a, c].join(); } return f([1], { b: [2] }); });
p(function () { function f({ [("k")]: a }) { eval("var a"); return a; } return f({ k: 4 }); });
p(function () { function f([a = 0]) { eval("var a"); return a; } return f([6]); });
