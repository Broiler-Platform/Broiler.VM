// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE RUNTIME HALF OF EXPLICIT RESOURCE MANAGEMENT.
//
// Retained for JSeal slices F18-F20: the two disposal Symbols, SuppressedError, DisposableStack and
// AsyncDisposableStack, and the two iterator disposers. No `using` syntax is exercised - that is F21
// and F22. The synchronous cases print as they go; the asynchronous ones are collected and printed
// by a reaction queued behind all of them, so the output order is the case order and not the order
// the jobs happened to run in.
var out = [];
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return "throws " + e.name; } }
function p(f) { __n++; out[__n] = t(f); }
function d(o, k) { var x = Object.getOwnPropertyDescriptor(o, k); return x ? [!!x.writable, !!x.enumerable, !!x.configurable].join("/") : "absent"; }

// --- the Symbols (1-6)
p(function () { return typeof Symbol.dispose + " " + typeof Symbol.asyncDispose; });
p(function () { return Symbol.dispose.description + " " + Symbol.asyncDispose.description; });
p(function () { return d(Symbol, "dispose") + " " + d(Symbol, "asyncDispose"); });
p(function () { return Symbol.dispose !== Symbol.asyncDispose && Symbol.dispose !== Symbol.for("Symbol.dispose"); });
p(function () { return Symbol.keyFor(Symbol.dispose) === undefined; });
p(function () { Symbol.dispose = 1; return typeof Symbol.dispose; });

// --- SuppressedError (7-20)
p(function () { return typeof SuppressedError + " " + SuppressedError.length + " " + SuppressedError.name; });
p(function () { return Object.getPrototypeOf(SuppressedError) === Error; });
p(function () { return Object.getPrototypeOf(SuppressedError.prototype) === Error.prototype; });
p(function () { return SuppressedError.prototype.name + "|" + SuppressedError.prototype.message + "|" + d(SuppressedError.prototype, "name"); });
p(function () { var a = {}, b = []; var e = new SuppressedError(a, b, "m"); return e.error === a && e.suppressed === b && e.message === "m"; });
p(function () { var e = new SuppressedError(1, 2, "m"); return Object.getOwnPropertyNames(e).filter(function (k) { return k !== "stack"; }).join(","); });
p(function () { var e = new SuppressedError(1, 2); return e.hasOwnProperty("message") + " " + d(e, "error") + " " + d(e, "suppressed"); });
p(function () { var e = SuppressedError(1, 2, "x"); return e instanceof SuppressedError && e instanceof Error; });
p(function () { return String(new SuppressedError(0, 0, "boom")); });
p(function () { return Object.prototype.toString.call(new SuppressedError()); });
p(function () { var e = new SuppressedError(1, 2, "m", { cause: 3 }); return e.hasOwnProperty("cause"); });
p(function () { var e = new SuppressedError(); return e.hasOwnProperty("error") + " " + e.error + " " + e.suppressed; });
p(function () { class S extends SuppressedError {} var e = new S(1, 2); return e instanceof S && e.error === 1; });
p(function () { return SuppressedError.prototype.hasOwnProperty("error") || SuppressedError.prototype.hasOwnProperty("suppressed"); });

// --- DisposableStack shape (21-30)
p(function () { return typeof DisposableStack + " " + DisposableStack.length; });
p(function () { return DisposableStack(); });
p(function () { return Object.getOwnPropertyNames(DisposableStack.prototype).sort().join(","); });
p(function () { return DisposableStack.prototype[Symbol.dispose] === DisposableStack.prototype.dispose; });
p(function () { return DisposableStack.prototype[Symbol.toStringTag] + " " + Object.prototype.toString.call(new DisposableStack()); });
p(function () { var g = Object.getOwnPropertyDescriptor(DisposableStack.prototype, "disposed"); return typeof g.get + " " + g.set + " " + g.get.name; });
p(function () { var s = DisposableStack.prototype; return [s.use.length, s.adopt.length, s.defer.length, s.move.length, s.dispose.length].join(","); });
p(function () { return DisposableStack.prototype.use.call({}, null); });
p(function () { return DisposableStack.prototype.use.call(new AsyncDisposableStack(), null); });
p(function () { return Object.getOwnPropertyDescriptor(DisposableStack.prototype, "disposed").get.call(new DisposableStack()); });

// --- DisposableStack behaviour (31-50)
p(function () { var log = []; var s = new DisposableStack();
  s.defer(function () { log.push("a"); });
  s.use({ [Symbol.dispose]() { log.push("b"); } });
  s.adopt("v", function (v) { log.push("c" + v); });
  s.use(null); s.use(undefined);
  var r = s.dispose(); return log.join("") + " " + r + " " + s.disposed; });
p(function () { var n = 0; var s = new DisposableStack(); s.defer(function () { n++; }); s.dispose(); s.dispose(); return n; });
p(function () { var s = new DisposableStack(); s.dispose(); return t(function () { s.use(null); }) + " " + t(function () { s.defer(function () {}); }) + " " + t(function () { s.adopt(1, function () {}); }) + " " + t(function () { s.move(); }); });
p(function () { var s = new DisposableStack(); return t(function () { s.use(1); }) + " " + t(function () { s.use({}); }) + " " + t(function () { s.defer(1); }) + " " + t(function () { s.adopt(1, 1); }); });
p(function () { var s = new DisposableStack(); var o = {}; return s.use(o) === o && s.adopt(o, function () {}) === o && s.defer(function () {}) === undefined; });
p(function () { var reads = 0; var o = {}; Object.defineProperty(o, Symbol.dispose, { get() { reads++; return function () { reads += 10; }; } });
  var s = new DisposableStack(); s.use(o); var before = reads; s.dispose(); return before + " " + reads; });
p(function () { var log = []; var o = { [Symbol.dispose]() { log.push("orig"); } }; var s = new DisposableStack(); s.use(o);
  o[Symbol.dispose] = function () { log.push("new"); }; s.dispose(); return log.join(","); });
p(function () { var who; var o = { [Symbol.dispose]() { who = this; } }; var s = new DisposableStack(); s.use(o); s.dispose(); return who === o; });
p(function () { var args; var s = new DisposableStack(); s.defer(function () { args = arguments.length + ":" + (this === undefined || this === globalThis); }); s.dispose(); return args; });
p(function () { var e1 = new Error("one"); var s = new DisposableStack(); s.defer(function () { throw e1; });
  try { s.dispose(); return "no throw"; } catch (e) { return e === e1; } });
p(function () { var s = new DisposableStack(); var log = [];
  s.defer(function () { log.push(1); throw "first"; });
  s.defer(function () { log.push(2); });
  s.defer(function () { log.push(3); throw "last"; });
  try { s.dispose(); } catch (e) { return log.join("") + " " + e.name + " " + e.error + " " + e.suppressed + " " + e.hasOwnProperty("message"); } });
p(function () { var s = new DisposableStack();
  s.defer(function () { throw "a"; }); s.defer(function () { throw "b"; }); s.defer(function () { throw "c"; });
  try { s.dispose(); } catch (e) { return e.error + "(" + e.suppressed.error + "(" + e.suppressed.suppressed + "))"; } });
p(function () { var log = []; var s = new DisposableStack(); s.defer(function () { log.push("x"); });
  var m = s.move(); return s.disposed + " " + m.disposed + " " + log.length + " " + (m instanceof DisposableStack); });
p(function () { var log = []; var s = new DisposableStack(); s.defer(function () { log.push("x"); }); var m = s.move(); s.dispose(); var a = log.length; m.dispose(); return a + " " + log.length; });
p(function () { class Sub extends DisposableStack {} var s = new Sub(); var m = s.move(); return (s instanceof Sub) + " " + (m instanceof Sub) + " " + (Object.getPrototypeOf(m) === DisposableStack.prototype); });
p(function () { var n = 0; var s = new DisposableStack(); s.defer(function () { n++; s.dispose(); }); s.defer(function () { n++; s.dispose(); }); s.dispose(); return n + " " + s.disposed; });
p(function () { var s = new DisposableStack(); var inner = "none"; s.defer(function () { inner = t(function () { s.defer(function () {}); }); }); s.dispose(); return inner; });
p(function () { var s = new DisposableStack(); return Object.isExtensible(s) + " " + (Object.getPrototypeOf(s) === DisposableStack.prototype); });
p(function () { function NT() {} NT.prototype = 3; var s = Reflect.construct(DisposableStack, [], NT); return Object.getPrototypeOf(s) === DisposableStack.prototype; });
p(function () { var log = []; var s = new DisposableStack(); for (var i = 0; i < 5; i++) { (function (k) { s.defer(function () { log.push(k); }); })(i); } s.dispose(); return log.join(""); });

// --- iterator disposers (51-53), reaching %IteratorPrototype% through a generator: three hops
function iteratorPrototype() { return Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf((function* () {})()))); }
p(function () { var IP = iteratorPrototype(); var f = IP[Symbol.dispose]; return typeof f + " " + f.name + " " + f.length + " " + d(IP, Symbol.dispose); });
p(function () { var IP = iteratorPrototype(); var it = Object.create(IP); var n = 0; it.return = function () { n = arguments.length + 1; return {}; }; return it[Symbol.dispose]() + " " + n; });
p(function () { var IP = iteratorPrototype(); return Object.create(IP)[Symbol.dispose](); });

// --- AsyncDisposableStack shape (54-60)
p(function () { return typeof AsyncDisposableStack + " " + AsyncDisposableStack.length; });
p(function () { return AsyncDisposableStack(); });
p(function () { return Object.getOwnPropertyNames(AsyncDisposableStack.prototype).sort().join(","); });
p(function () { return AsyncDisposableStack.prototype[Symbol.asyncDispose] === AsyncDisposableStack.prototype.disposeAsync; });
p(function () { return Object.prototype.toString.call(new AsyncDisposableStack()) + " " + AsyncDisposableStack.prototype.hasOwnProperty(Symbol.dispose); });
p(function () { var s = new AsyncDisposableStack(); return t(function () { s.use(1); }) + " " + t(function () { s.use({}); }) + " " + t(function () { s.use({ [Symbol.asyncDispose]: 1 }); }) + " " + t(function () { s.defer(1); }); });
p(function () { var s = new AsyncDisposableStack(); var o = { [Symbol.dispose]() {} }; return s.use(o) === o && s.use(null) === null; });

// --- the asynchronous cases (61-72) settle later and are filled in by index
var pending = [];
function later(n, f) { __n = n; pending.push(Promise.resolve().then(f).then(function (v) { out[n] = String(v); }, function (e) { out[n] = "rejects " + (e && e.name) + " " + (e && e.message); })); }
__n = 60;
later(61, function () { var log = []; var s = new AsyncDisposableStack();
  s.defer(function () { log.push("a"); });
  s.use({ async [Symbol.asyncDispose]() { await null; log.push("b"); } });
  s.use({ [Symbol.dispose]() { log.push("c"); return { then: function () { log.push("T"); } }; } });
  s.adopt("v", async function (v) { log.push("d" + v); });
  var p1 = s.disposeAsync(); var sync = log.join("");
  return p1.then(function (r) { return sync + "|" + log.join("") + " " + r + " " + s.disposed + " " + (p1 instanceof Promise); }); });
later(62, function () { return AsyncDisposableStack.prototype.disposeAsync.call({}).then(function () { return "fulfilled"; }, function (e) { return "rejected " + e.name; }); });
later(63, function () { var s = new AsyncDisposableStack(); var n = 0; s.defer(async function () { n++; });
  var a = s.disposeAsync(), b = s.disposeAsync(); return Promise.all([a, b]).then(function () { return n + " " + (a !== b); }); });
later(64, function () { var s = new AsyncDisposableStack();
  s.defer(function () { return Promise.reject("r1"); });
  s.defer(function () { throw "t2"; });
  s.defer(async function () { throw "r3"; });
  return s.disposeAsync().then(function () { return "fulfilled"; }, function (e) { return e.name + " " + e.error + "(" + e.suppressed.error + "(" + e.suppressed.suppressed + "))"; }); });
later(65, function () { var seq = []; var s = new AsyncDisposableStack(); s.use(null);
  return Promise.all([
    Promise.resolve().then(function () { return 0; }).then(function () { seq.push("j1"); }),
    s.disposeAsync().then(function () { seq.push("dispose"); }),
    Promise.resolve().then(function () { return 0; }).then(function () { seq.push("j2"); })
  ]).then(function () { return seq.join(","); }); });
later(66, function () { var seq = []; var s = new AsyncDisposableStack();
  return Promise.all([
    Promise.resolve().then(function () { seq.push("j1"); }),
    s.disposeAsync().then(function () { seq.push("dispose"); })
  ]).then(function () { return seq.join(","); }); });
later(67, function () { var log = []; var s = new AsyncDisposableStack(); s.defer(function () { log.push("x"); });
  var m = s.move(); return s.disposeAsync().then(function () { var a = log.length; return m.disposeAsync().then(function () { return a + " " + log.length + " " + s.disposed + " " + m.disposed; }); }); });
later(68, function () { var s = new AsyncDisposableStack(); var inner; s.defer(function () { inner = s.disposeAsync(); });
  return s.disposeAsync().then(function () { return inner.then(function (v) { return "inner " + v; }); }); });
later(69, function () { var s = new AsyncDisposableStack(); s.disposeAsync(); return t(function () { s.use(null); }) + " " + t(function () { s.move(); }); });
later(70, function () { var AIP = Object.getPrototypeOf(Object.getPrototypeOf(async function* () {}.prototype)); var f = AIP[Symbol.asyncDispose];
  var it = Object.create(AIP); var n = -1; it.return = function (x) { n = arguments.length + ":" + x; return 5; };
  return it[Symbol.asyncDispose]().then(function (v) { return typeof f + " " + f.name + " " + f.length + " " + v + " " + n; }); });
later(71, function () { var AIP = Object.getPrototypeOf(Object.getPrototypeOf(async function* () {}.prototype)); var it = Object.create(AIP);
  it.return = function () { throw new RangeError("r"); }; return it[Symbol.asyncDispose]().then(function () { return "fulfilled"; }, function (e) { return "rejected " + e.name; }); });
later(72, function () { var s = new AsyncDisposableStack(); s.use({ [Symbol.dispose]() { throw new RangeError("sync"); } });
  return s.disposeAsync().then(function () { return "fulfilled"; }, function (e) { return "rejected " + e.name; }); });

Promise.all(pending).then(function () {
  for (var i = 1; i < out.length; i++) { print(i + " " + out[i]); }
});
