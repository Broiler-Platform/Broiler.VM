// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER SMALL GAPS EARLIER SLICES RECORDED.
//
// Retained for JSeal VM-FIX-H. An async generator forced to return at a `yield` inside a
// `for await` body closes the loop's iterator, and awaits that close, before it finishes (1-8).
// `using` followed by `await` is a declaration whose name a class static block refuses (9-11). A
// RegExp group name is read by code point, so a name outside the basic plane, spelled literally or
// with `\u` escapes, is accepted in either mode (12-22). Asynchronous answers are collected and
// printed together at the end, in case order.
var out = [];
var pending = [];
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return "throws " + (e && e.name ? e.name : e); } }
function p(f) { __n++; out[__n] = t(f); }
function later(n, f) { __n = n; pending.push(Promise.resolve().then(f).then(function (v) { out[n] = String(v); }, function (e) { out[n] = "rejected " + e; })); }
function parses(src) { try { Function(src); return "ok"; } catch (e) { return e.name; } }
function source(log, ret) {
  return {
    [Symbol.asyncIterator]() {
      return {
        next() { log.push("next"); return Promise.resolve({ value: 1, done: false }); },
        return: ret === undefined ? undefined : function () { log.push("return"); return ret(); }
      };
    }
  };
}
function settle(promise, log) { return promise.then(function (r) { log.push("done " + r.value + "/" + r.done); }, function (e) { log.push("rejected " + (e && e.name ? e.name : e)); }); }

// --- a forced return at a yield in a `for await` body (1-8)
later(1, async function () { var log = [];
  var g = (async function* () { for await (var x of source(log, function () { return { done: true }; })) { log.push("body"); yield x; log.push("after"); } })();
  await g.next(); await settle(g.return(42), log); return log.join(); });
later(2, async function () { var log = [];
  var g = (async function* () { try { for await (var x of source(log, function () { return Promise.resolve({ done: true }); })) { yield x; } } finally { log.push("fin"); await null; log.push("fin2"); } })();
  await g.next(); await settle(g.return(7), log); return log.join(); });
later(3, async function () { var log = [];
  var g = (async function* () { for await (var x of source(log, function () { return Promise.reject("r"); })) { yield x; } })();
  await g.next(); await settle(g.return(1), log); return log.join(); });
later(4, async function () { var log = [];
  var g = (async function* () { for await (var x of source(log, function () { return 5; })) { yield x; } })();
  await g.next(); await settle(g.return(1), log); return log.join(); });
later(5, async function () { var log = [];
  var g = (async function* () { for await (var x of source(log)) { yield x; } })();
  await g.next(); await settle(g.return(3), log); return log.join(); });
later(6, async function () { var log = [];
  var sync = { [Symbol.iterator]() { return { next() { return { value: Promise.resolve(9), done: false }; }, return() { log.push("sync-return"); return {}; } }; } };
  var g = (async function* () { for await (var x of sync) { log.push("got " + x); yield x; } })();
  await g.next(); await settle(g.return(2), log); return log.join(); });
later(7, async function () { var log = [];
  var outer = source(log, function () { log.push("outer"); return {}; });
  var g = (async function* () { for await (var x of outer) { for await (var y of source(log, function () { log.push("inner"); return {}; })) { yield y; } } })();
  await g.next(); await settle(g.return(4), log); return log.join(); });
later(8, async function () { var log = [];
  var g = (async function* () { for await (var x of source(log, function () { return {}; })) { yield x; } })();
  await g.next(); await settle(g.return(6), log); await settle(g.next(), log); return log.join(); });

// --- `using await` (9-11)
p(function () { return parses("class C { static { using await = null; } }"); });
p(function () { return parses("class C { static { for (using await of []) ; } }"); });
p(function () { var seen = "unset"; { using await = null; seen = await; } return String(seen); });

// --- RegExp group names by code point (12-22)
p(function () { return /(?<\u{1d49c}>b)/u.exec("abc").groups["\u{1d49c}"]; });
p(function () { return new RegExp("(?<𝒜>b)").exec("abc").groups["\u{1d49c}"]; });
p(function () { return /(?<\u{1d49c}x>b)/.exec("b").groups["\u{1d49c}x"]; });
p(function () { return /(?<𝒜>b)/.exec("b").groups["\u{1d49c}"]; });
p(function () { return /(?<\u{1d49c}>b)/u[Symbol.replace]("abc", "d$<\u{1d49c}>$`"); });
p(function () { return new RegExp("(?<$𐒤>b)", "gu")[Symbol.replace]("abc", "$'$<$𐒤>d"); });
p(function () { return /(?<a>x)\k<a>/.test("xx") + "," + /(?<a>x)\k<\u{61}>/u.test("xx"); });
p(function () { return Object.keys(new RegExp("(?<a𝟚>x)").exec("x").groups).join(); });
p(function () { return new RegExp("(?<𝟚>x)"); });
p(function () { return new RegExp("(?<\\ud835>x)", "u"); });
p(function () { return new RegExp("(?<a>x)(?<\\u0061>y)"); });

Promise.all(pending).then(function () { for (var i = 1; i < out.length; i++) { print(i + " " + out[i]); } });
