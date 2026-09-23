// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER `Array.fromAsync`.
//
// The three input shapes in the order the specification prefers them (async iterator, then sync
// iterator wrapped so every value is awaited, then array-like with every element awaited), the
// mapper awaited in order, the receiver as the constructor of the result, and the rule that every
// failure - including the ones found before the first `await` - is a REJECTION and never a throw.
// Where an iterator is abandoned part-way it is owed its `return`, and the rejection comes only
// after that `return`'s answer has been awaited.
//
// IT PRINTS AT THE END AND NOT AS IT GOES, for the reason the promise probe does: every answer is
// produced by a job, and the host's one stated drain point is what runs them.

var out = [];
function at(n, v) { out[n] = String(v); }
var pending = [];
function track(p) { pending.push(p); return p; }
function name(e) { return e && e.name ? e.name : String(e); }
function asyncOf(values, log) {
  var o = {};
  o[Symbol.asyncIterator] = function () {
    var i = 0;
    return {
      next: function () {
        if (log) { log.push("next" + arguments.length); }
        return Promise.resolve(i < values.length ? { value: values[i++], done: false } : { value: undefined, done: true });
      },
      "return": function () { if (log) { log.push("return"); } return Promise.resolve({ done: true }); }
    };
  };
  return o;
}
function syncOf(values, log) {
  var o = {};
  o[Symbol.iterator] = function () {
    var i = 0;
    return {
      next: function () { return i < values.length ? { value: values[i++], done: false } : { value: undefined, done: true }; },
      "return": function () { log.push("return"); return {}; }
    };
  };
  return o;
}

at(1, typeof Array.fromAsync);
at(2, Array.fromAsync.length + ":" + Array.fromAsync.name);
var d3 = Object.getOwnPropertyDescriptor(Array, "fromAsync");
at(3, d3.writable + "," + d3.enumerable + "," + d3.configurable);
at(4, Array.fromAsync([]) instanceof Promise);
try { new Array.fromAsync([]); at(5, "constructed"); } catch (e) { at(5, name(e)); }

var log6 = [];
track(Array.fromAsync([1]).then(function () { log6.push("then"); at(6, log6.join()); }));
log6.push("sync");

async function* gen() { yield 1; yield Promise.resolve(2); yield 3; }
track(Array.fromAsync(gen()).then(function (r) { at(7, Array.isArray(r) + ":" + r.join()); }));
track(Array.fromAsync([Promise.resolve(1), 2, Promise.resolve(3)]).then(function (r) { at(8, r.join()); }));
track(Array.fromAsync({ length: 2, 0: Promise.resolve("a"), 1: "b" }).then(function (r) { at(9, r.join() + ":" + r.length); }));
track(Array.fromAsync("ab\u{1F600}").then(function (r) { at(10, r.length); }));
track(Array.fromAsync(new Set([3, 4])).then(function (r) { at(11, r.join()); }));
track(Array.fromAsync([1, 2], function (x, i) { return x * 10 + i; }).then(function (r) { at(12, r.join()); }));
track(Array.fromAsync([1, 2], async function (x) { return x * 2; }).then(function (r) { at(13, r.join()); }));
track(Array.fromAsync([1], function () { return this.m; }, { m: "this" }).then(function (r) { at(14, r.join()); }));
track(Array.fromAsync([1], function () { "use strict"; return typeof this; }).then(function (r) { at(15, r.join()); }));

try {
  track(Array.fromAsync([], 5).then(function () { at(16, "fulfilled"); }, function (e) { at(16, "rejected " + name(e)); }));
} catch (e) { at(16, "threw " + name(e)); }
try {
  track(Array.fromAsync(null).then(function () { at(17, "fulfilled"); }, function (e) { at(17, "rejected " + name(e)); }));
} catch (e) { at(17, "threw " + name(e)); }
var bad18 = {}; bad18[Symbol.asyncIterator] = 1;
track(Array.fromAsync(bad18).then(function () { at(18, "fulfilled"); }, function (e) { at(18, "rejected " + name(e)); }));

var log19 = [];
var rej19 = {};
rej19[Symbol.asyncIterator] = function () {
  return { next: function () { return Promise.reject(new RangeError("n")); }, "return": function () { log19.push("return"); return {}; } };
};
track(Array.fromAsync(rej19).then(null, function (e) { at(19, name(e) + ":" + log19.join()); }));

var log20 = [];
track(Array.fromAsync(syncOf([1, Promise.reject(new SyntaxError("v")), 3], log20)).then(null, function (e) { at(20, name(e) + ":" + log20.join()); }));

var log21 = [];
track(Array.fromAsync(asyncOf([1, 2], log21), function (x) { if (x === 2) { throw new EvalError("m"); } return x; })
  .then(null, function (e) { at(21, name(e) + ":" + log21.join()); }));

var log22 = [];
track(Array.fromAsync(syncOf([1, 2], log22), async function (x) { if (x === 1) { throw new URIError("a"); } return x; })
  .then(null, function (e) { at(22, name(e) + ":" + log22.join()); }));

var log23 = [];
var gate = [];
function later(v, n) { var p = Promise.resolve(v); for (var i = 0; i < n; i++) { p = p.then(function (x) { return x; }); } return p; }
track(Array.fromAsync([5, 1, 3], function (x) { log23.push("call" + x); return later(x, x).then(function (v) { log23.push("done" + v); return v; }); })
  .then(function (r) { at(23, r.join() + ":" + log23.join()); }));

function C24() { this.count = arguments.length; }
track(Array.fromAsync.call(C24, [7, 8]).then(function (r) { at(24, (r instanceof C24) + ":" + r.count + ":" + r.length + ":" + r[1] + ":" + Array.isArray(r)); }));
track(Array.fromAsync.call(C24, { length: 2, 0: 7, 1: 8 }).then(function (r) { at(25, (r instanceof C24) + ":" + r.count + ":" + r.length); }));
track(Array.fromAsync.call({}, [1, 2]).then(function (r) { at(26, Array.isArray(r) + ":" + r.join()); }));
track(Array.fromAsync.call(undefined, { length: 1, 0: "u" }).then(function (r) { at(27, Array.isArray(r) + ":" + r.join()); }));

track(Array.fromAsync([{ then: function (res) { res(5); } }]).then(function (r) { at(28, r.join()); }));
track(Array.fromAsync({ length: 1, 0: { then: function () { throw new TypeError("t"); } } }).then(null, function (e) { at(29, name(e) + ":" + e.message); }));

var log30 = [];
function Frozen30() { return Object.freeze({}); }
track(Array.fromAsync.call(Frozen30, asyncOf([1], log30)).then(null, function (e) { at(30, name(e) + ":" + log30.join()); }));
function BadLength31() { var o = {}; Object.defineProperty(o, "length", { set: function () { throw new RangeError("len"); } }); return o; }
track(Array.fromAsync.call(BadLength31, [1]).then(null, function (e) { at(31, name(e) + ":" + e.message); }));
var len32 = {}; Object.defineProperty(len32, "length", { get: function () { throw new ReferenceError("l"); } });
track(Array.fromAsync(len32).then(null, function (e) { at(32, name(e)); }));
track(Array.fromAsync({ length: 4294967296 }).then(function () { at(33, "fulfilled"); }, function (e) { at(33, name(e)); }));

track(Array.fromAsync(asyncOf([Promise.resolve("p")])).then(function (r) { at(34, r[0] instanceof Promise); }));
var both35 = asyncOf(["async"]);
both35[Symbol.iterator] = function () { return [ "sync" ][Symbol.iterator](); };
track(Array.fromAsync(both35).then(function (r) { at(35, r.join()); }));

var log36 = [];
var slow36 = {};
slow36[Symbol.asyncIterator] = function () {
  return {
    next: function () { return { value: 1, done: false }; },
    "return": function () { return later(0, 5).then(function () { log36.push("return-settled"); return {}; }); }
  };
};
track(Array.fromAsync(slow36, function () { throw new Error("stop"); }).then(null, function (e) { log36.push("rejected " + e.message); at(36, log36.join()); }));

var log37 = [];
track(Array.fromAsync(asyncOf([1, 2], log37), function () { return arguments.length; }).then(function (r) { at(37, r.join() + ":" + log37.join()); }));
track(Array.fromAsync({ length: 2, 1: "x" }).then(function (r) { at(38, (0 in r) + ":" + r[0] + ":" + r[1]); }));
track(Array.fromAsync(3).then(function (r) { at(39, Array.isArray(r) + ":" + r.length); }));

var calls40 = 0;
var getter40 = {};
Object.defineProperty(getter40, Symbol.iterator, { get: function () { calls40++; return function () { return [1][Symbol.iterator](); }; } });
track(Array.fromAsync(getter40).then(function (r) { at(40, r.join() + ":" + calls40); }));

var noClose41 = [];
var closer41 = {};
closer41[Symbol.asyncIterator] = function () {
  return { next: function () { return { get done() { throw new Error("done"); } }; }, "return": function () { noClose41.push("return"); return {}; } };
};
track(Array.fromAsync(closer41).then(null, function (e) { at(41, e.message + ":" + noClose41.length); }));

var log42 = [];
track(Array.fromAsync(asyncOf([1], log42), function () { return Promise.reject(new TypeError("r")); })
  .then(null, function (e) { at(42, name(e) + ":" + log42.join()); }));

Promise.allSettled(pending).then(function () { return later(0, 10); }).then(function () {
  for (var i = 1; i < out.length; i++) { print(i + " " + (out[i] === undefined ? "MISSING" : out[i])); }
});
