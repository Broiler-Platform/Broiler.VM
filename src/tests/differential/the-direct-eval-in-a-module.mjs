// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER DIRECT EVAL UNDER THE MODULE GOAL (JSeal slice V14, JSD-0026 step 1).
//
// A module's own bindings are slots and its imports are not bindings a global evaluation can see.
// JSeal V14 refused a direct eval at a module's top level, and in a function whose scope chain
// reaches a module, with an explicit EvalError rather than evaluating the source globally - which
// used to answer a ReferenceError for a name the module declares; since JSeal V15-module the eval
// scope map has a module row and both evaluate against the module (the-module-scoped-eval.mjs
// covers them in depth). An indirect eval, and a direct one whose argument is not a String, are
// unaffected. Each case prints its own number.
let m = "module";
let n = 0;
function show(v) { return typeof v === "string" ? JSON.stringify(v) : String(v); }
function t(f) { try { return show(f()); } catch (e) { return e && e.name; } }
function p(f) { n++; print(n + " " + t(f)); }

let r;
try { r = show(eval("m")); } catch (e) { r = e.name; }
n++; print(n + " " + r);
p(function () { let y = 2; return eval("y"); });
p(function () { return (0, eval)("typeof m"); });
try { r = show(eval(5)); } catch (e) { r = e.name; }
n++; print(n + " " + r);
