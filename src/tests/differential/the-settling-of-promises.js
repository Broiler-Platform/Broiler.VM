// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE PROMISE AND THE ORDER IT SETTLES IN.
//
// The combinators over the receiver rather than over this realm's own constructor, the capability
// every static builds its answer through, the species a `then` consults, the identity rules a
// program can see - `Promise.resolve(p) === p`, one `reject` function shared by every element of an
// `all` - and the turn a reaction runs on.
//
// IT PRINTS AT THE END AND NOT AS IT GOES, because every answer here is one a job produces: the
// cases are filled in as their promises settle and printed by a reaction queued behind all of them.
// A probe that printed as it went would be a probe about the ORDER of its own output rather than
// about the promise, and the order is case 26.

var out = [];
function at(n, v) { out[n] = String(v); }

at(1, typeof Promise.withResolvers);
at(2, typeof Promise.try);
at(3, Object.prototype.toString.call(Promise.resolve()));
at(4, Promise.prototype[Symbol.toStringTag]);
at(5, typeof Object.getOwnPropertyDescriptor(Promise, Symbol.species).get);
var pr = Promise.resolve(7);
at(6, Promise.resolve(pr) === pr);
try { Promise.all.call(null, []); at(7, "no-throw"); } catch (e) { at(7, e.name); }
try { Promise.resolve.call(5, 1); at(8, "no-throw"); } catch (e) { at(8, e.name); }
function MyPromise(exec) { this.p = new Promise(exec); }
MyPromise.prototype.then = function () { return this.p.then.apply(this.p, arguments); };
at(9, Promise.resolve.call(MyPromise, 3) instanceof MyPromise);
var withR = Promise.withResolvers();
at(10, (typeof withR.resolve) + "," + (typeof withR.reject) + "," + (withR.promise instanceof Promise));
Promise.all([1, Promise.resolve(2)]).then(function (v) { at(11, v.join()); });
Promise.allSettled([Promise.reject(1), 2]).then(function (v) { at(12, v[0].status + v[0].reason + v[1].status + v[1].value); });
Promise.race([Promise.resolve("r"), new Promise(function () {})]).then(function (v) { at(13, v); });
Promise.any([Promise.reject(1), Promise.resolve("a")]).then(function (v) { at(14, v); });
Promise.any([Promise.reject(1)]).catch(function (e) { at(15, e.name + ":" + e.errors.join() + ":" + (Object.getPrototypeOf(e) === AggregateError.prototype)); });
Promise.any([]).catch(function (e) { at(16, e.name + ":" + e.errors.length); });
if (typeof Promise.try === "function") {
  Promise.try(function () { return 5; }).then(function (v) { at(17, v); });
  Promise.try(function () { throw new Error("boom"); }).catch(function (e) { at(18, e.message); });
  Promise.try(function (a, b) { return a + b; }, 1, 2).then(function (v) { at(19, v); });
} else { at(17, "absent"); at(18, "absent"); at(19, "absent"); }
var calls = 0;
Promise.all([{ then: function (res) { calls++; res("t"); } }]).then(function (v) { at(20, v.join() + ":" + calls); });
var handlers = [];
var probe = { then: function (f, r) { handlers.push(r); f(1); } };
Promise.all([probe, probe]).then(function () { at(21, handlers[0] === handlers[1]); });
var order = [];
Promise.resolve().then(function () { order.push(1); });
Promise.resolve().then(function () { order.push(2); }).then(function () { order.push(3); });
Promise.reject(new Error("x")).catch(function (e) { at(22, e.message); });
Promise.resolve(1).finally(function () { order.push("f"); }).then(function (v) { at(23, v); });
var counted = 0;
function Sub(exec) { counted++; this.p = new Promise(exec); }
Sub.prototype.then = function () { return this.p.then.apply(this.p, arguments); };
Sub.resolve = function (v) { return new Sub(function (r) { r(v); }); };
Object.defineProperty(Sub, Symbol.species, { get: function () { return Sub; } });
at(24, typeof Sub.resolve);
var stopping = {};
stopping[Symbol.iterator] = function () { var n = 0; return { next: function () { return { value: n++, done: false }; }, "return": function () { at(25, "closed"); return {}; } }; };
var savedResolve = Promise.resolve;
Promise.resolve = function (v) { if (v > 2) { throw new Error("stop"); } return savedResolve.call(Promise, v); };
Promise.all(stopping).catch(function (e) { at(27, e.message); });
Promise.resolve = savedResolve;
function tick(n, f) { var q = Promise.resolve(); for (var i = 0; i < n; i++) { q = q.then(function () {}); } return q.then(f); }
tick(14, function () {
  at(26, order.join());
  for (var i = 1; i < out.length; i++) { print(i + " " + (out[i] === undefined ? "MISSING" : out[i])); }
});
