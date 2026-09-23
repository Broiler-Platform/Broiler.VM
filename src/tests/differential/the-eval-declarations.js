// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER WHAT A DIRECT OR INDIRECT EVAL DECLARES (JSeal slice V15, JSD-0026
// steps 6-8).
//
// A sloppy evaluation's `var` and function declarations belong to its caller's variable
// environment: a function's own record, extended by name with deletable bindings, or the global
// object, where they become configurable properties. Its `let`, `const` and `class` declarations,
// and every declaration of a strict evaluation, stay inside the evaluation. Nothing is introduced
// when a declaration would collide with a lexical binding between the call and the variable
// environment - that is a SyntaxError raised before anything is created - except a catch clause's
// parameter, which Annex B.3.4 exempts whatever its form. An indirect eval is global code in the
// same sense. Each case prints its own number so a divergence names a case.
var __n = 0;
function show(v) { return typeof v === "string" ? JSON.stringify(v) : String(v); }
function t(f) { try { return show(f()); } catch (e) { return e && e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function top(r) { __n++; print(__n + " " + r); }
function desc(name) {
  var d = Object.getOwnPropertyDescriptor(globalThis, name);
  return d === undefined ? "absent" : [typeof d.value, d.writable, d.enumerable, d.configurable].join("/");
}

// --- a sloppy evaluation introduces a var into the calling function (step 6)
p(function () { function f() { eval("var y = 1"); return y; } return f(); });
p(function () { function f() { eval("var y = 1"); delete y; return typeof y; } return f(); });
p(function () { function f() { eval("var y = 1"); return delete y; } return f(); });
p(function () { function f(a) { eval("var a = 2"); return a; } return f(1); });
p(function () { function f(a) { eval("var a"); return a; } return f(1); });
p(function () { function f() { eval("function g() { return 'g'; }"); return g(); } return f(); });
p(function () { function f() { eval("var y = 1"); return (() => y)(); } return f(); });
p(function () { function f() { var h = () => y; eval("var y = 2"); return h(); } return f(); });
p(function () { function f() { function inner() { return typeof y; } eval("var y = 3"); return inner(); } return f(); });
p(function () { function f() { var r = [typeof yv]; eval("var yv = 1"); r.push(typeof yv); return r.join(); } return f(); });
p(function () { function f() { eval("var y = 1"); eval("y = 5"); return y; } return f(); });
p(function () { function f() { eval("eval('var y = 7')"); return y; } return f(); });
p(function () { function f() { eval("var notGlobal = 1"); return typeof globalThis.notGlobal; } return f(); });
p(function () { function f(n) { if (n) eval("var q = n"); return typeof q; } return [f(1), f(0)].join(); });
p(function () { function f() { eval("function g() { return 1; } function g() { return 2; }"); return g(); } return f(); });
p(function () { function f() { eval("var g = 1; function g() { }"); return typeof g; } return f(); });
p(function () { function f() { var o = {}; with (o) { eval("var w = 1"); } return [w, "w" in o].join(); } return f(); });
p(function () { function f() { var o = { w: 0 }; with (o) { eval("var w = 1"); } return [w, o.w].join(); } return f(); });
p(function () { function f() { eval("function g() { 'use strict'; return this; }"); return typeof g(); } return f(); });
p(function () { function f() { eval("function g() { return this === globalThis; }"); return g(); } return f(); });
p(function () { function f(a) { eval("var fromArguments = arguments[0]"); return fromArguments; } return f(4); });
p(function () { function f(a) { eval("var arguments = 1"); return arguments; } return f(4); });
p(function () { function f() { var r = (() => { eval("var z = 1"); return z; })(); return [r, typeof z].join(); } return f(); });
p(function () { function f() { eval("var before = typeof bb; { function bb() { } }"); return before + "," + typeof bb; } return f(); });
p(function () { function f() { eval("{ function b() { return 'b'; } }"); return b(); } return f(); });
p(function () { function f() { let cc = 1; { eval("{ function cc() { } }"); } return typeof cc; } return f(); });
p(function () { function f() { eval("var s = 'kept'"); return eval("s"); } return f(); });
p(function () { function* g() { eval("var gv = 'gen'"); yield gv; } return g().next().value; });

// --- conflicts: checked before anything is created (step 7)
p(function () { function f() { let a; { eval("var a"); } } return f(); });
p(function () { function f() { let a = 1; try { eval("var a = 2"); } catch (e) { return e.name + ":" + a; } } return f(); });
p(function () { function f() { let a = 1; eval("var a"); } return f(); });
p(function () { function f() { { let b; eval("function b() { }"); } } return f(); });
p(function () { function f() { let a; try { eval("var zz1; var a"); } catch (e) { } return typeof zz1; } return f(); });
p(function () { function f() { return eval("let n = 1; eval('var n = 2')"); } return f(); });
p(function () { function f() { eval("let q = 1; { var r = q; }"); return r; } return f(); });
p(function () { function f() { const k = 1; { eval("var k"); } } return f(); });
p(function () { function f() { class K { } eval("var K"); } return f(); });

// --- a catch clause's parameter is exempt, whatever its form (Annex B.3.4)
p(function () { function f() { try { throw 1; } catch (e) { eval("var e = 2"); return e; } } return f(); });
p(function () { function f() { try { throw [1]; } catch ([e]) { eval("var e = 2"); return e; } } return f(); });
p(function () { function f() { try { throw 1; } catch (e) { eval("var e = 2"); } return typeof e; } return f(); });
p(function () { function f() { try { throw 1; } catch (e) { let k; eval("var k"); } } return f(); });
p(function () { function f() { try { throw 1; } catch (err) { eval("function err() { }"); return typeof err; } } return f(); });

// --- strict evaluations keep everything they declare
p(function () { function f() { "use strict"; eval("var v = 1"); return typeof v; } return f(); });
p(function () { function f() { eval("'use strict'; var v = 1"); return typeof v; } return f(); });
p(function () { function f() { eval("'use strict'; function sfn() { }"); return typeof sfn; } return f(); });
p(function () { function f() { "use strict"; return (0, eval)("var indirectFromStrict = 1; typeof indirectFromStrict"); } return f(); });

// --- source that does not parse, or throws after its declarations were made
p(function () { function f() { try { eval("var 1x"); } catch (e) { return e.name; } } return f(); });
p(function () { function f() { try { eval("var ok1 = 1; @"); } catch (e) { return e.name + "," + typeof ok1; } } return f(); });
p(function () { function f() { try { eval("var ok2 = 1; throw 3"); } catch (e) { return e + "," + ok2; } } return f(); });

// --- indirect eval: global code, with configurable bindings and the global checks (step 8)
p(function () { (0, eval)("var gi1 = 1"); return desc("gi1"); });
p(function () { (0, eval)("function gf1() { }"); return desc("gf1"); });
p(function () { (0, eval)("let gl1 = 1"); return typeof gl1; });
p(function () { return (0, eval)("const gc1 = 1; gc1"); });
p(function () { (0, eval)("class GC1 { }"); return typeof GC1; });
p(function () { return (0, eval)("function NaN() { }"); });
p(function () { return (0, eval)("var NaN; typeof NaN"); });
p(function () { Object.defineProperty(globalThis, "acc1", { get() { return 1; }, configurable: true }); (0, eval)("function acc1() { }"); return desc("acc1"); });
p(function () { Object.defineProperty(globalThis, "nc1", { value: 1, writable: true, enumerable: true, configurable: false }); (0, eval)("function nc1() { }"); return desc("nc1"); });
p(function () { Object.defineProperty(globalThis, "nw1", { value: 1, writable: false, enumerable: false, configurable: false }); (0, eval)("function nw1() { }"); });
p(function () { (0, eval)("'use strict'; var si1 = 1"); return typeof si1; });
p(function () { (0, eval)("{ function gab1() { return 1; } }"); return typeof gab1; });
p(function () { (0, eval)("var ord1; function ord2() { }"); return Object.keys(globalThis).filter(function (k) { return k === "ord1" || k === "ord2"; }).join(); });
p(function () { return (0, eval)("this") === globalThis; });
p(function () { (0, eval)("var del1 = 1"); return delete globalThis.del1; });
p(function () { return (0, eval)("var twice = 1; eval('var twice = 2'); twice"); });
p(function () { try { (0, eval)("var gok1; var NaN; function Infinity() { }"); } catch (e) { return e.name + "," + desc("gok1"); } });

// --- a script's own top level, where the caller's variable environment is the global one (step 8)
var r;
let topLex = 1;
try { eval("let zz = 1"); r = typeof zz; } catch (e) { r = e.name; } top(r);
try { eval("var topLex = 2"); r = "no error"; } catch (e) { r = e.name + "," + topLex; } top(r);
try { eval("let topLex = 3"); r = show(topLex); } catch (e) { r = e.name; } top(r);
try { eval("var gg = 1"); r = show(delete gg); } catch (e) { r = e.name; } top(r);
try { eval("var tl = 1"); r = show(tl); } catch (e) { r = e.name; } top(r);
try { eval("function tf() { return 'tf'; }"); r = show(tf()); } catch (e) { r = e.name; } top(r);
try { r = show(eval("var cv = 5; cv")); } catch (e) { r = e.name; } top(r);
try { { let b; eval("var declaredInABlock = 1"); r = typeof declaredInABlock; } } catch (e) { r = e.name; } top(r);
try { try { throw 1; } catch (e) { eval("var e = 2"); r = show(e); } } catch (e) { r = e.name; } top(r);
try { { let blockLex; eval("var blockLex"); } r = "no error"; } catch (e) { r = e.name; } top(r);
try { (0, eval)("var topLex = 4"); r = "no error"; } catch (e) { r = e.name + "," + topLex; } top(r);

// --- a parameter initialiser's evaluation: the parameters are between it and the variable environment (step 9)
p(function () { function f(q = eval("var arguments")) { } return f(); });
p(function () { function f(q = eval("var arguments"), arguments) { } return f(); });
p(function () { var f = (q = eval("var arguments = 'param'")) => arguments; return f(); });
p(function () { function f(a = eval("var z = 1"), b = z) { return b; } return f(); });
p(function () { function f(a = eval("var z = 1")) { return z; } return f(); });
p(function () { var x = "outer"; function f({ a: ignored = eval("var x = 'inner'") }) { return x; } return f({}); });
p(function () { function f(a = eval("var z = 1")) { var z; return z; } return f(); });
p(function () { function f(a = eval("typeof bodyVar")) { var bodyVar = 1; return a; } return f(); });
p(function () { function f(a = eval("var z = 1"), g = () => z) { var z = 2; return [z, g()].join(); } return f(); });
p(function () { function f(a = eval("var z = 1")) { return delete z; } return f(); });
p(function () { function f(a = eval("var p1 = 1"), g = () => p1) { eval("var y = 1"); return g(); } return f(); });

// --- a parameter list and its body share one record here: the shapes it cannot answer keep the refusal (step 9)
p(function () { function f(a = eval("() => typeof x")) { eval("var x = 1"); return a(); } return f(); });
p(function () { function f(a = eval("function pz() { return typeof x; }")) { eval("var x = 1"); return pz(); } return f(); });
p(function () { function f(a = eval("1")) { eval("var q = 3"); return q; } return f(); });
p(function () { function f(a, b = eval("() => a")) { var a = 2; return b(); } return f(1); });
p(function () { function f(a, b = eval("(function () { return a; })")) { var a = 2; return b(); } return f(1); });
p(function () { function f(a, b = eval("() => a")) { function a() { } return typeof b(); } return f(1); });
p(function () { function f(a, b = eval("() => arguments.length")) { var arguments; return b(); } return f(1); });
p(function () { function f(a, b = eval("a")) { var c = 2; return [b, c].join(); } return f(4); });
p(function () { function f(a, b = eval("() => a")) { var a; return b(); } return f(1); });
p(function () { function f(a, b = eval("() => a")) { var a; a = 3; return b(); } return f(1); });
p(function () { function f(a, b = eval("() => a")) { var a; function g() { a = 3; } g(); return b(); } return f(1); });

// --- a call through a locally bound name `eval` is direct when the value is %eval%, and ordinary otherwise
p(function () { function f() { var eval = globalThis.eval; eval("var sh = 1"); return typeof sh; } return f() + "," + desc("sh"); });
p(function () { function f() { var eval = globalThis.eval; var y = 5; return eval("y"); } return f(); });
p(function () { function f(eval) { return eval("1"); } return f(function (s) { return "own:" + s; }); });
p(function () { function f() { function eval(s) { return "decl:" + s; } return eval("2"); } return f(); });
