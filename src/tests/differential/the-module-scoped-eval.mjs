// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER DIRECT EVAL IN MODULE CODE (JSeal V15-module).
//
// A direct eval at a module's top level, or in a function whose scope reaches a module, sees the
// module's own bindings and its imports - live, immutable, in their dead zone until the exporting
// module initialises them - and then the global scope. Eval code in a module is strict, so its
// `var`s and functions stay in the evaluation's own record, and it is Script-goal code, so
// `import`, `export`, `import.meta` and `new.target` there are SyntaxErrors. Dependencies are
// under `modules/`, so the runner does not treat them as probes. Each case prints its own number.
import { early, readLater } from "./modules/scoped-eval-early.mjs";
import { live, bumpLive, fixed } from "./modules/scoped-eval-dep.mjs";
import * as ns from "./modules/scoped-eval-dep.mjs";
import receiverless from "./modules/scoped-eval-dep.mjs";

export let later = "later";

let m = "module";
const k = 1;
let n = 0;
function show(v) { return typeof v === "string" ? JSON.stringify(v) : String(v); }
function out(r) { n++; print(n + " " + r); }
function p(f) { let r; try { r = show(f()); } catch (e) { r = e && e.name; } out(r); }
let r;

// --- the module's own bindings, from its top level
try { r = show(eval("m")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("m = 'changed'; m") + ":" + m); } catch (e) { r = e.name; } out(r);
try { r = show(eval("k = 2")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("late")); } catch (e) { r = e.name; } out(r);
let late = "late";
try { r = show(eval("late")); } catch (e) { r = e.name; } out(r);

// --- imports: live, immutable, the namespace, a default
try { r = show(eval("live")); } catch (e) { r = e.name; } out(r);
bumpLive();
try { r = show(eval("live")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("live = 5")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("ns.fixed") + ":" + (eval("ns") === ns)); } catch (e) { r = e.name; } out(r);
try { r = show(eval("receiverless()")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("typeof fixed") + ":" + eval("typeof bumpLive")); } catch (e) { r = e.name; } out(r);

// --- eval code in a module is strict: its declarations stay its own
try { r = show(eval("var v = 1; v") + ":" + typeof v); } catch (e) { r = e.name; } out(r);
try { r = show(eval("let w = 2; w") + ":" + typeof w); } catch (e) { r = e.name; } out(r);
try { r = show(eval("function g() { return 3; } g()") + ":" + typeof g); } catch (e) { r = e.name; } out(r);
try { r = show(eval("with ({}) {}")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("undeclaredInModule = 1")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("delete live")); } catch (e) { r = e.name; } out(r);

// --- eval code is Script-goal code
try { r = show(eval("this")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("new.target")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("import.meta")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("export default 1;")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("import x from './modules/scoped-eval-dep.mjs';")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("arguments")); } catch (e) { r = e.name; } out(r);
try { r = show(eval("await")); } catch (e) { r = e.name; } out(r);

// --- functions, closures, blocks and nesting in a module
p(function () { let y = 7; return eval("y + live"); });
p(function () { let c = 1; const h = eval("() => c + fixed"); c = 9; return h(); });
p(function () { return eval("eval('m + later')"); });
p(function () { return (function (live) { return eval("live"); })("param"); });
p(function () { return (() => eval("m"))(); });
{ let b = "block"; try { r = show(eval("b + ':' + m")); } catch (e) { r = e.name; } out(r); }
p(function () { class K { #p = 4; get() { return eval("this.#p + live"); } } return new K().get(); });
p(function () { bumpLive(); const h = eval("() => live"); bumpLive(); return h(); });
p(function () { return (0, eval)("typeof m"); });

// --- an import in its dead zone, met through eval by a module that ran first
p(function () { return early; });
p(function () { return readLater(); });
