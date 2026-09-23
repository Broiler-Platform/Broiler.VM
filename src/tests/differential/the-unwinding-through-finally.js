// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE CODE A JUMP RUNS ON ITS WAY OUT.
//
// Retained for JSeal slice F21 (its unmet acceptance items). A `break`, `continue` or `return`
// runs every finaliser, iterator close and resource disposal it passes; what one of those throws
// belongs to the statements still around it, so no `finally` runs twice and no `catch` of a
// statement already left sees it. The last cases are the early errors `await` has in a class
// static block. Asynchronous answers are collected and printed together at the end, in case order.
var out = [];
var pending = [];
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return "throws " + e; } }
function p(f) { __n++; out[__n] = t(f); }
function later(n, f) { __n = n; pending.push(Promise.resolve().then(f).then(function (v) { out[n] = String(v); }, function (e) { out[n] = "rejected " + e; })); }
function parses(src) { try { Function(src); return "ok"; } catch (e) { return e.name; } }
function r(log, name, fail) { return { [Symbol.dispose]: function () { log.push(name); if (fail) { throw name; } } }; }
function it(log, fail) { return { [Symbol.iterator]() { return this; }, next() { return { done: false, value: 1 }; }, return() { log.push("ret"); if (fail) { throw "c"; } return {}; } }; }
function shown(e) { return e instanceof SuppressedError ? "Suppressed(" + e.error + "," + e.suppressed + ")" : String(e); }
function run(log, f) { try { log.push("= " + f()); } catch (e) { log.push("caught " + shown(e)); } return log.join(); }

// --- a resource scope whose disposer throws after a passed finally (1-4)
p(function () { var log = []; return run(log, function () { using a = r(log, "a", true); try { return 1; } finally { log.push("fin"); } }); });
p(function () { var log = []; return run(log, function () { for (;;) { using a = r(log, "a", true); try { break; } finally { log.push("fin"); } } }); });
p(function () { var log = []; return run(log, function () { for (var i = 0; i < 2; i++) { using a = r(log, "a" + i, true); try { continue; } finally { log.push("fin" + i); } } }); });
p(function () { var log = []; return run(log, function () { for (;;) { using a = r(log, "a", true), b = r(log, "b", true); try { break; } finally { log.push("fin"); } } }); });

// --- a finaliser that throws on the way out (5-10)
p(function () { var log = []; return run(log, function () { try { return 1; } finally { log.push("fin"); throw "t"; } }); });
p(function () { var log = []; return run(log, function () { try { try { return 1; } finally { log.push("in"); throw "t"; } } finally { log.push("out"); } }); });
p(function () { var log = []; return run(log, function () { try { throw 0; } catch (e) { return 1; } finally { log.push("fin"); throw "t"; } }); });
p(function () { var log = []; for (var i = 0; i < 2; i++) { try { try { continue; } finally { log.push("fin" + i); throw "t" + i; } } catch (e) { log.push("c " + e); } } return log.join(); });
p(function () { var log = []; return run(log, function () { try { try { return 1; } finally { log.push("in"); throw "i"; } } finally { log.push("out"); throw "o"; } }); });
p(function () { var log = []; return run(log, function () { try { switch (1) { case 1: try { break; } finally { log.push("fin"); throw "t"; } } } catch (e) { log.push("c " + e); } return 2; }); });

// --- iterator closes on the way out (11-16)
p(function () { var log = []; return run(log, function () { for (var x of it(log, true)) { try { return 1; } finally { log.push("fin"); } } }); });
p(function () { var log = []; return run(log, function () { for (var x of it(log, true)) { try { return 1; } catch (e) { log.push("wrong " + e); } } }); });
p(function () { var log = []; return run(log, function () { for (var x of it(log, true)) { break; } }); });
p(function () { var log = []; return run(log, function () { for (var x of it(log, true)) { return 1; } }); });
p(function () { var log = []; return run(log, function () { outer: for (var y of it(log, false)) { for (var x of it(log, true)) { try { break outer; } finally { log.push("fin"); } } } }); });
p(function () { var log = []; return run(log, function () { for (var x of it(log, false)) { try { try { return 1; } finally { log.push("fin"); throw "t"; } } catch (e) { log.push("c " + e); return 2; } } }); });

// --- an outer catch around the scope sees the throw once, and the normal paths are unchanged (17-20)
p(function () { var log = []; var f = function () { try { using a = r(log, "a", true); try { return 1; } finally { log.push("fin"); } } catch (e) { log.push("c " + e); return 2; } }; return f() + " " + log.join(); });
p(function () { var log = []; return run(log, function () { try { return "r"; } finally { log.push("fin"); } }); });
p(function () { var log = []; return run(log, function () { try { log.push("body"); } finally { log.push("fin"); } return "end"; }); });
p(function () { var log = []; return run(log, function () { try { throw "x"; } finally { log.push("fin"); } }); });

// --- a generator suspended in a finaliser the return path runs (21-24)
p(function () { var log = []; var g = (function* () { try { try { return 1; } finally { log.push("in"); yield 2; log.push("in2"); } } finally { log.push("out"); } })();
  var a = g.next(); var b = g.return(9); return JSON.stringify([a, b]) + " " + log.join(); });
p(function () { var log = []; var g = (function* () { try { try { return 1; } catch (e) { log.push("wrong " + e); } finally { yield 2; } } catch (e) { log.push("outer " + e); } })();
  g.next(); var b = g.throw("x"); return JSON.stringify(b) + " " + log.join(); });
p(function () { var log = []; var g = (function* () { for (var x of it(log, false)) { try { return 1; } finally { log.push("fin"); yield 2; } } })();
  g.next(); var b = g.return(7); return JSON.stringify(b) + " " + log.join(); });
p(function () { var log = []; var g = (function* () { try { try { return 1; } finally { log.push("in"); } } finally { log.push("out"); yield 3; } })();
  var a = g.next(); var b = g.next(); return JSON.stringify([a, b]) + " " + log.join(); });

// --- await in a class static block is neither a name nor the operator (25-40)
p(function () { return parses("class C { static { using await = null; } }"); });
p(function () { return parses("class C { static { let await; } }"); });
p(function () { return parses("class C { static { const await = 1; } }"); });
p(function () { return parses("class C { static { var await; } }"); });
p(function () { return parses("class C { static { class await {} } }"); });
p(function () { return parses("class C { static { function await() {} } }"); });
p(function () { return parses("class C { static { await: ; } }"); });
p(function () { return parses("class C { static { x = await; } }"); });
p(function () { return parses("class C { static { ((x = await) => 1); } }"); });
p(function () { return parses("class C { static { try {} catch (await) {} } }"); });
p(function () { return parses("class C { static { `${await}`; } }"); });
p(function () { return parses("class C { static { (() => await); } }"); });
p(function () { return parses("class C { static { (function await() {}); } }"); });
p(function () { return parses("class C { static { class D { m(await) {} x = await; } } }"); });
p(function () { return parses("class C { static { ({ await: 1 }).await; } }"); });
p(function () { var await = 5; var seen; class C { static { seen = (() => await)(); } } return seen; });

// --- an async function: an await using scope after a passed finally (41-42)
later(41, async function () { var log = []; var f = async function () { await using a = { async [Symbol.asyncDispose]() { log.push("a"); throw "a"; } }; try { return 1; } finally { log.push("fin"); } };
  try { await f(); } catch (e) { log.push("caught " + e); } return log.join(); });
later(42, async function () { var log = []; var f = async function () { for (var x of it(log, true)) { try { await null; return 1; } finally { log.push("fin"); } } };
  try { await f(); } catch (e) { log.push("caught " + e); } return log.join(); });

// --- a generator's forced return and throw at a yield inside a for-of body (43-46)
p(function () { var log = []; var g = (function* () { for (var x of it(log, false)) { yield 1; } })(); g.next(); return JSON.stringify(g.return(5)) + " " + log.join(); });
p(function () { var log = []; var g = (function* () { for (var x of it(log, true)) { yield 1; } })(); g.next(); try { g.return(5); } catch (e) { log.push("caught " + e); } return log.join(); });
p(function () { var log = []; var g = (function* () { for (using x of [r(log, "x"), r(log, "y")]) { yield 1; } })(); g.next(); return JSON.stringify(g.return(5)) + " " + log.join(); });
p(function () { var log = []; var g = (function* () { for (var x of it(log, true)) { yield 1; } })(); g.next(); try { g.throw("e"); } catch (e) { log.push("caught " + e); } return log.join(); });

Promise.all(pending).then(function () { for (var i = 1; i < out.length; i++) { print(i + " " + out[i]); } });
