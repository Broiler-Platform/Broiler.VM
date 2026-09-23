// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE ITERATOR GLOBAL AND ITS HELPERS (JSeal slices F11-F15).
//
// Cases 1-24 are the prototype hierarchy and the constructor (F11): one `%Iterator.prototype%`,
// one kind-specific prototype per built-in iterator, the two accessors with the setter that
// ignores the prototype, `Iterator.from` and its wrapper. Cases 25-44 are the lazy `map` and
// `filter` over the shared helper state (F12), 45-60 `take` and `drop` (F13), 61-74 `flatMap`
// (F14) and 75-99 the terminal helpers (F15). Case 100 on is `Iterator.concat`, which the pinned
// edition places on the same constructor.
//
// Each case prints its own number, so a divergence names a case rather than a line. A case that
// throws prints the error's name, because a refusal is an answer.

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// A counting iterator over 0..n-1 that records what was asked of it.
function counter(n, log) {
  var i = 0;
  var it = Object.create(Iterator.prototype);
  it.next = function () { log && log.push("next"); return i < n ? { value: i++, done: false } : { value: undefined, done: true }; };
  it.return = function () { log && log.push("return"); return {}; };
  return it;
}
// An iterator that never ends.
function naturals() {
  var i = 0;
  return { __proto__: Iterator.prototype, next: function () { return { value: i++, done: false }; } };
}

var IteratorProto = Object.getPrototypeOf(Object.getPrototypeOf([][Symbol.iterator]()));

// ---- F11: the hierarchy ---------------------------------------------------------------------------
p(function () { return typeof Iterator + " " + (Iterator.prototype === IteratorProto); });
p(function () { return Object.getPrototypeOf(IteratorProto) === Object.prototype; });
p(function () {
  var kinds = [[][Symbol.iterator](), ""[Symbol.iterator](), new Map()[Symbol.iterator](),
    new Set()[Symbol.iterator](), /a/g[Symbol.matchAll]("a"), (function* () {})()];
  return kinds.map(function (k) { return Object.getPrototypeOf(Object.getPrototypeOf(k)) === IteratorProto || Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf(k))) === IteratorProto; }).join(",");
});
p(function () {
  var a = Object.getPrototypeOf([][Symbol.iterator]());
  var s = Object.getPrototypeOf(""[Symbol.iterator]());
  var m = Object.getPrototypeOf(new Map().keys());
  var t2 = Object.getPrototypeOf(new Set().values());
  return [a === s, a === m, m === t2, a === IteratorProto, a.hasOwnProperty("next"), IteratorProto.hasOwnProperty("next")].join(",");
});
p(function () {
  var a = Object.getPrototypeOf([][Symbol.iterator]());
  a.marker = 1;
  var seen = [""[Symbol.iterator]().marker, new Map().keys().marker, [1].values().marker];
  delete a.marker;
  return seen.join(",");
});
p(function () { return Object.getPrototypeOf([].keys()) === Object.getPrototypeOf(new Uint8Array(1).values()); });
p(function () { return [[][Symbol.iterator](), ""[Symbol.iterator](), new Map().entries(), new Set().entries(), /a/g[Symbol.matchAll]("a")].map(function (x) { return Object.prototype.toString.call(x); }).join(","); });
p(function () { var n = Object.getPrototypeOf([].keys()).next; return n.call(""[Symbol.iterator]()); });
p(function () { var n = Object.getPrototypeOf(new Map().keys()).next; return n.call(new Set().values()); });
p(function () { var it = [1, 2].values(); it.next(); it.next(); it.next(); return String(it.next().done); });
p(function () { var a = [1]; var it = a.values(); it.next(); it.next(); a.push(2); return JSON.stringify(it.next()); });
p(function () { return Iterator(); });
p(function () { return new Iterator(); });
p(function () { class My extends Iterator {} var m = new My(); return (m instanceof Iterator) + " " + (Object.getPrototypeOf(My.prototype) === IteratorProto); });
p(function () {
  var d = Object.getOwnPropertyDescriptor(Iterator.prototype, "constructor");
  var t3 = Object.getOwnPropertyDescriptor(Iterator.prototype, Symbol.toStringTag);
  return [typeof d.get, typeof d.set, d.enumerable, d.configurable, typeof t3.get, Iterator.prototype.constructor === Iterator, Iterator.prototype[Symbol.toStringTag]].join(",");
});
p(function () { var o = Object.create(Iterator.prototype); o.constructor = 5; o[Symbol.toStringTag] = "Mine"; return o.hasOwnProperty("constructor") + " " + o.constructor + " " + String(o) + " " + (Iterator.prototype.constructor === Iterator); });
p(function () { "use strict"; Iterator.prototype.constructor = 1; return "assigned"; });
p(function () { var d = Object.getOwnPropertyDescriptor(Iterator, "prototype"); return [d.writable, d.enumerable, d.configurable, Iterator.length, Iterator.name].join(","); });
p(function () { var it = [1, 2][Symbol.iterator](); return Iterator.from(it) === it; });
p(function () { return Iterator.from("ab").toArray().join(","); });
p(function () { var w = Iterator.from({ next: function () { return { value: 7, done: false }; } }); return (w instanceof Iterator) + " " + w.next().value + " " + (Object.getPrototypeOf(w) !== IteratorProto); });
p(function () { var w = Iterator.from({ next: function () { return 1; } }); return w.next(); });
p(function () { return Iterator.from(5); });
p(function () { var closed = 0; var w = Iterator.from({ next: function () { return {}; }, return: function () { closed++; return { done: true, value: 9 }; } }); var r = w.return(); var w2 = Iterator.from({ next: function () { return {}; } }); return closed + " " + r.value + " " + JSON.stringify(w2.return()); });

// ---- F12: map and filter --------------------------------------------------------------------------
p(function () { var log = []; var h = counter(3, log).map(function (x) { return x * 2; }); return log.length + " " + Object.prototype.toString.call(h); });
p(function () { return counter(4).map(function (x, i) { return x + ":" + i; }).toArray().join(","); });
p(function () { return counter(6).filter(function (x, i) { return x % 2 === 0; }).toArray().join(","); });
p(function () { var idx = []; counter(5).filter(function (x, i) { idx.push(i); return false; }).toArray(); return idx.join(","); });
p(function () { return naturals().map(function (x) { return x * x; }).take(4).toArray().join(","); });
p(function () { var log = []; var h = counter(5, log).map(function (x) { return x; }); h.next(); h.return(); return log.join(","); });
p(function () { var log = []; var h = counter(5, log).map(function (x) { return x; }); var r = h.return(); return log.join(",") + " " + JSON.stringify(r) + " " + JSON.stringify(h.next()); });
p(function () { var log = []; var h = counter(5, log).map(function (x) { throw new RangeError("m"); }); try { h.next(); } catch (e) { log.push(e.name); } return log.join(",") + " " + JSON.stringify(h.next()); });
p(function () { var h; h = counter(5).map(function (x) { return h.next(); }); return h.next(); });
p(function () { var h; h = counter(5).map(function (x) { return h.return(); }); return h.next(); });
p(function () { var log = []; try { counter(3, log).map(5); } catch (e) { log.push(e.name); } return log.join(","); });
p(function () { return Iterator.prototype.map.call(5, function () {}); });
p(function () { var reads = 0; var it = { get next() { reads++; return function () { return { done: true }; }; } }; var h = Iterator.prototype.map.call(it, function (x) { return x; }); h.next(); h.next(); return reads; });
p(function () { var h = counter(2).map(function (x) { return x; }); return [h.next().value, h.next().value, h.next().done, h.next().done].join(","); });
p(function () { var H = Object.getPrototypeOf(counter(1).map(function () {})); return [H.hasOwnProperty("next"), H.hasOwnProperty("return"), H[Symbol.toStringTag], Object.getPrototypeOf(H) === IteratorProto].join(","); });
p(function () { var H = Object.getPrototypeOf(counter(1).map(function () {})); return H.next.call(counter(1)); });
p(function () { var h = counter(3).filter(function (x) { return x > 0; }); var r = h.next(); return r.value + " " + r.done + " " + Object.keys(r).join("|"); });
p(function () { var bad = { next: function () { return { get done() { throw new EvalError("d"); } }; }, return: function () { throw new Error("should not close"); } }; return Iterator.prototype.map.call(bad, function (x) { return x; }).next(); });
p(function () { var err = { next: function () { return { value: 1, done: false }; }, return: function () { throw new SyntaxError("close"); } }; return Iterator.prototype.filter.call(err, function () { throw new URIError("cb"); }).next(); });
p(function () { var h = counter(3).map(function (x) { return x; }); h.next(); var ret = { next: 0 }; var closedWith = h.return(); return JSON.stringify(closedWith); });

// ---- F13: take and drop ---------------------------------------------------------------------------
p(function () { return counter(5).take(2).toArray().join(","); });
p(function () { return counter(5).take(0).toArray().length; });
p(function () { return counter(3).take(Infinity).toArray().join(","); });
p(function () { return counter(3).take(1.9).toArray().join(","); });
p(function () { return counter(3).take(NaN); });
p(function () { return counter(3).take(-1); });
p(function () { return counter(3).take(-0.5).toArray().length; });
p(function () { var log = []; var h = counter(5, log).take(1); h.next(); h.next(); return log.join(","); });
p(function () { var log = []; try { counter(5, log).take(-3); } catch (e) { log.push(e.name); } return log.join(","); });
p(function () { var order = []; var lim = { valueOf: function () { order.push("limit"); return 1; } }; var it = { get next() { order.push("get next"); return function () { return { done: true }; }; } }; Iterator.prototype.take.call(it, lim); return order.join(","); });
p(function () { return counter(5).drop(2).toArray().join(","); });
p(function () { return counter(5).drop(0).toArray().join(","); });
p(function () { return counter(5).drop(Infinity).toArray().length; });
p(function () { var log = []; counter(3, log).drop(10).toArray(); return log.length; });
p(function () { return counter(1e5).drop(99998).toArray().join(","); });
p(function () { return naturals().drop(3).take(3).toArray().join(","); });

// ---- F14: flatMap ---------------------------------------------------------------------------------
p(function () { return counter(3).flatMap(function (x) { return [x, x * 10]; }).toArray().join(","); });
p(function () { return counter(3).flatMap(function (x) { return []; }).toArray().length; });
p(function () { return counter(2).flatMap(function (x) { return "ab"; }).toArray(); });
p(function () { return counter(2).flatMap(function (x) { return new String("ab"); }).toArray().join(","); });
p(function () { return counter(2).flatMap(function (x) { return counter(x + 1); }).toArray().join(","); });
p(function () { return counter(2).flatMap(function (x) { return { next: function () { return { done: true }; } }; }).toArray().length; });
p(function () { return counter(2).flatMap(function (x) { return 5; }).next(); });
p(function () { return counter(2).flatMap(function (x) { return [[x]]; }).toArray().map(function (a) { return Array.isArray(a); }).join(","); });
p(function () { var log = []; var inner = { __proto__: Iterator.prototype, next: function () { return { value: "i", done: false }; }, return: function () { log.push("inner"); return {}; } }; var h = counter(3, log).flatMap(function () { return inner; }); h.next(); h.return(); return log.join(","); });
p(function () { var log = []; var inner = { next: function () { return { value: "i", done: false }; }, return: function () { log.push("inner"); throw new EvalError("x"); } }; var h = counter(3, log).flatMap(function () { return inner; }); h.next(); try { h.return(); } catch (e) { log.push(e.name); } return log.join(","); });
p(function () { var log = []; var inner = { next: function () { throw new RangeError("step"); } }; var h = counter(3, log).flatMap(function () { return inner; }); try { h.next(); } catch (e) { log.push(e.name); } return log.join(",") + " " + h.next().done; });
p(function () { var log = []; var h = counter(3, log).flatMap(function () { throw new TypeError("m"); }); try { h.next(); } catch (e) { log.push(e.name); } return log.join(","); });
p(function () { return naturals().flatMap(function (x) { return [x, -x]; }).take(5).toArray().join(","); });
p(function () { var h = counter(1).flatMap(function (x) { return [x]; }); h.next(); h.next(); return JSON.stringify(h.next()) + JSON.stringify(h.return()); });

// ---- F15: the terminal helpers --------------------------------------------------------------------
p(function () { return counter(4).toArray().join(","); });
p(function () { return Array.isArray(counter(0).toArray()) + " " + counter(0).toArray().length; });
p(function () { var seen = []; var r = counter(3).forEach(function (x, i) { seen.push(x + "@" + i); }); return seen.join(",") + " " + r; });
p(function () { return counter(5).reduce(function (a, b) { return a + b; }); });
p(function () { return counter(5).reduce(function (a, b) { return a + b; }, 100); });
p(function () { return counter(0).reduce(function (a, b) { return a + b; }); });
p(function () { return counter(0).reduce(function (a, b) { return a + b; }, "init"); });
p(function () { var idx = []; counter(4).reduce(function (a, b, i) { idx.push(i); return a; }); return idx.join(","); });
p(function () { var idx = []; counter(3).reduce(function (a, b, i) { idx.push(i); return a; }, 0); return idx.join(","); });
p(function () { var log = []; var r = counter(5, log).some(function (x) { return x === 1; }); return r + " " + log.join(","); });
p(function () { return counter(5).some(function (x) { return x > 10; }); });
p(function () { var log = []; var r = counter(5, log).every(function (x) { return x < 1; }); return r + " " + log.join(","); });
p(function () { return counter(5).every(function (x) { return x < 10; }); });
p(function () { var log = []; var r = counter(5, log).find(function (x) { return x === 2; }); return r + " " + log.join(","); });
p(function () { return String(counter(5).find(function (x) { return x === 9; })); });
p(function () { var log = []; try { counter(5, log).forEach(function (x) { if (x === 1) throw new RangeError("cb"); }); } catch (e) { log.push(e.name); } return log.join(","); });
p(function () { var log = []; try { counter(5, log).reduce(5); } catch (e) { log.push(e.name); } return log.join(","); });
p(function () { var log = []; try { counter(5, log).some(null); } catch (e) { log.push(e.name); } return log.join(","); });
p(function () { var r = { next: function () { return { value: 1, done: false }; }, return: function () { throw new EvalError("ret"); } }; return Iterator.prototype.find.call(r, function () { return true; }); });
p(function () { var r = { next: function () { return { value: 1, done: false }; }, return: function () { return 5; } }; return Iterator.prototype.every.call(r, function () { return false; }); });
p(function () { return Iterator.prototype.toArray.call({ next: function () { return { done: true }; } }).length; });
p(function () { return Iterator.prototype.toArray.call(null); });
p(function () { var total = 0; naturals().take(20000).forEach(function (x) { total += x; }); return total; });
p(function () { return ["map", "filter", "take", "drop", "flatMap", "reduce", "toArray", "forEach", "some", "every", "find"].map(function (k) { return k + "/" + Iterator.prototype[k].length; }).join(","); });
p(function () { var d = Object.getOwnPropertyDescriptor(Iterator.prototype, "map"); var constructs; try { new Iterator.prototype.map(function () {}); constructs = "constructs"; } catch (e) { constructs = e.name; } return [d.writable, d.enumerable, d.configurable, constructs].join(","); });

// ---- Iterator.concat ------------------------------------------------------------------------------
p(function () { return Iterator.concat([1, 2], new Set([3]), "xy".split("")).toArray().join(","); });
p(function () { return Iterator.concat([1], 2); });
p(function () { return Iterator.concat().next().done; });
p(function () { var log = []; var h = Iterator.concat({ [Symbol.iterator]: function () { log.push("open"); return counter(2, log); } }); log.push("made"); h.next(); h.return(); return log.join(","); });
