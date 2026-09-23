// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER `using` AND `await using` DECLARATIONS.
//
// Retained for JSeal slices F21-F22 (decision JSD-0034). The first cases check what parses and what is
// refused early, by compiling source through `Function` and indirect `eval`. The middle cases cover
// disposal order and when it happens, how completions combine, and the loop heads. The last cases
// are asynchronous, including the turn counts an `await using` owes. As in `the-disposal-stacks.js`,
// asynchronous answers are collected and printed together at the end, in case order.
var out = [];
var pending = [];
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return "throws " + e.name; } }
function p(f) { __n++; out[__n] = t(f); }
function later(n, f) { __n = n; pending.push(Promise.resolve().then(f).then(function (v) { out[n] = String(v); }, function (e) { out[n] = "rejected " + (e && e.name); })); }
function parses(src) { try { Function(src); return "ok"; } catch (e) { return e.name; } }
function parsesScript(src) { try { (0, eval)(src); return "ok"; } catch (e) { return e.name; } }
function r(log, name, fail) { return { [Symbol.dispose]: function () { log.push(name); if (fail) { throw new Error(name); } } }; }
function ar(log, name, fail) { return { [Symbol.asyncDispose]: async function () { log.push(name); if (fail) { throw new Error(name); } } }; }

// --- grammar and early errors (1-16)
p(function () { return parses("{ using a = null, b = undefined; }"); });
p(function () { return parses("{ using [a] = null; }"); });
p(function () { return parses("{ using {a} = null; }"); });
p(function () { return parses("{ using a = null, [b] = null; }"); });
p(function () { return parses("{ using a; }"); });
p(function () { return parses("switch (0) { case 0: using a = null; }"); });
p(function () { return parses("switch (0) { case 0: { using a = null; } }"); });
p(function () { return parses("if (1) using a = null;"); });
p(function () { return parses("for (using a in {}) ;"); });
p(function () { return parses("for (using a = null;;) break;"); });
p(function () { return parses("{ using let = null; }"); });
p(function () { return parses("{ using a = null; let a; }"); });
p(function () { return parses("{ await using a = null; }"); });
p(function () { return parses("async function f() { for (await using of of []) {} }"); });
p(function () { return parsesScript("using a = null; 0"); });
p(function () { var using = 7; { using
  ; } return using; });

// --- disposal order and timing (17-26)
p(function () { var log = []; { using a = r(log, "a"), b = r(log, "b"); using c = null; log.push("body"); } return log.join(); });
p(function () { var log = []; var f = function () { using a = r(log, "f"); log.push("body"); return log.join("+"); }; var v = f(); return v + " " + log.join(); });
p(function () { var log = []; for (var i = 0; i < 3; i++) { using x = r(log, "i" + i); if (i == 1) { continue; } if (i == 2) { break; } log.push("after" + i); } return log.join(); });
p(function () { var log = []; l: { using a = r(log, "lab"); break l; } return log.join(); });
p(function () { var log = []; for (using x of [r(log, "o1"), r(log, "o2")]) { log.push("iter"); } return log.join(); });
p(function () { var log = []; for (using x = r(log, "h1"), y = r(log, "h2"); log.length < 2; ) { log.push("loop"); } return log.join(); });
p(function () { var log = []; var it = (function* () { using a = r(log, "gen"); yield 1; yield 2; })(); it.next(); var v = it.return(5); return JSON.stringify(v) + " " + log.join(); });
p(function () { var log = []; var iter = { i: 0, [Symbol.iterator]() { return this; }, next() { return { done: this.i++ > 1, value: r(log, "v" + this.i) }; }, return() { log.push("close"); return {}; } };
  for (using x of iter) { break; } return log.join(); });
p(function () { var log = []; var o = { get [Symbol.dispose]() { log.push("get"); return function () { log.push("call"); }; } }; { using x = o; log.push("body"); } return log.join(); });
p(function () { var log = []; var o = r(log, "orig"); { using x = o; o[Symbol.dispose] = function () { log.push("replaced"); }; } return log.join(); });

// --- completions (27-36)
p(function () { try { { using a = r([], "one", true); } } catch (e) { return e.name + " " + e.message; } });
p(function () { try { { using a = r([], "a", true), b = r([], "b", true); throw new Error("body"); } } catch (e) { return e.name + ":" + e.error.message + ":" + e.suppressed.name + ":" + e.suppressed.error.message + ":" + e.suppressed.suppressed.message; } });
p(function () { try { { using a = r([], "a", true); throw "raw"; } } catch (e) { return e.suppressed + " " + e.hasOwnProperty("message"); } });
p(function () { var f = function () { using a = r([], "a", true); return 1; }; try { return f(); } catch (e) { return "throws " + e.message; } });
p(function () { try { { using a = 1; } } catch (e) { return e.name; } });
p(function () { try { { using a = {}; } } catch (e) { return e.name; } });
p(function () { try { { using a = { [Symbol.dispose]: 1 }; } } catch (e) { return e.name; } });
p(function () { var log = []; try { { using a = r(log, "a"); using b = {}; } } catch (e) { return e.name + " " + log.join(); } });
p(function () { try { { using a = null; a = 1; } } catch (e) { return e.name; } });
p(function () { try { { x; using x = null; } } catch (e) { return e.name; } });

// --- asynchronous (37-44)
later(37, async function () { var log = []; { await using a = ar(log, "a"), b = r(log, "b"); using c = r(log, "c"); await using d = null; log.push("body"); } return log.join(); });
later(38, async function () { var log = []; try { await using a = ar(log, "a", true); await using b = ar(log, "b", true); throw new Error("body"); } catch (e) { return log.join() + " " + e.name + ":" + e.error.message + ":" + e.suppressed.error.message + ":" + e.suppressed.suppressed.message; } });
later(39, async function () { var log = []; for (await using x of [ar(log, "x1"), r(log, "x2"), null]) { log.push("iter"); } return log.join(); });
later(40, async function () { var log = []; var f = async function () { await using a = ar(log, "r"); return "ret"; }; var v = await f(); return v + " " + log.join(); });
later(41, async function () { var ticks = 0, done = false; (async function () { while (!done) { ticks++; await null; } })();
  { await using n = null; } var t1 = ticks; { await using n = ar([], "t"); } var t2 = ticks; { using s = r([], "s"); await using n = null; } var t3 = ticks; done = true; return t1 + "," + t2 + "," + t3; });
later(42, async function () { var log = []; var f = async function () { await using x = ar(log, 42); await using y = ar(log, 43); }; f(); return log.join(); });
later(43, async function () { var log = []; var it = (async function* () { await using a = ar(log, "gen"); yield 1; yield 2; })(); await it.next(); var v = await it.return(5); return JSON.stringify(v) + " " + log.join(); });
later(44, async function () { var log = []; try { await using a = { [Symbol.asyncDispose]: function () { return Promise.reject(new RangeError("r")); } }; } catch (e) { return e.name; } return "none"; });

// --- catch and finally bodies (45-54)
p(function () { return parses("try {} catch (e) { using x = null; }") + " " + parses("try {} finally { using x = null; }"); });
p(function () { var log = []; try { throw 1; } catch (e) { using a = r(log, "c" + e); log.push("in"); } log.push("after"); return log.join(); });
p(function () { var log = []; (function () { using o = r(log, "outer"); try { throw 1; } catch (e) { using a = r(log, "c"); log.push("in"); } log.push("after"); })(); return log.join(); });
p(function () { var log = []; try { throw { x: "px" }; } catch ({ x }) { using a = r(log, x); using b = r(log, "b"); log.push("in"); } return log.join(); });
p(function () { var log = []; var f = function () { try { throw 1; } catch (e) { using a = r(log, "c"); return "ret"; } }; var v = f(); return v + " " + log.join(); });
p(function () { try { try { throw new Error("orig"); } catch (e) { using a = r([], "d", true); } } catch (e2) { return e2.message; } });
p(function () { var log = []; try { log.push("t"); } finally { using a = r(log, "f"); log.push("fin"); } log.push("after"); return log.join(); });
p(function () { var log = []; try { try { throw new Error("x"); } finally { using a = r(log, "f"); log.push("fin"); } } catch (e) { return e.message + " " + log.join(); } });
later(53, async function () { var log = []; try { throw 1; } catch (e) { await using a = ar(log, "ac"); log.push("in"); } log.push("after"); return log.join(); });
later(54, async function () { var log = []; try { log.push("t"); } finally { await using a = ar(log, "af"); using b = r(log, "sf"); log.push("fin"); } log.push("after"); return log.join(); });

Promise.all(pending).then(function () {
  for (var i = 1; i < out.length; i++) { print(i + " " + out[i]); }
});
