// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE `with` STATEMENT AND THE NAMES IT RESOLVES.
//
// Retained from the bring-up of the family it covers: every case was compared against the
// comparison engine before it was written down, and each prints its own number so a divergence
// names a case rather than a line.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// --- the object has the name, and does not
p(function () { var o = {a: 1}; with (o) { return a; } });
p(function () { var a = "outer"; var o = {}; with (o) { return a; } });
p(function () { var o = {a: 1}; var a = "outer"; with (o) { return a; } });
p(function () { var o = {a: undefined}; var a = "outer"; with (o) { return String(a); } });
p(function () { var o = {}; with (o) { return typeof nowhere; } });
p(function () { var o = {}; with (o) { return nowhere; } });

// --- the prototype chain counts
p(function () { var base = {b: 7}; var o = Object.create(base); with (o) { return b; } });
p(function () { function B() {} B.prototype.m = function () { return "proto"; }; with (new B()) { return m(); } });
p(function () { var base = {b: 7}; var o = Object.create(base); var b = 1; with (o) { return b; } });
p(function () { var o = Object.create(null); var q = "outer"; with (o) { return q; } });
p(function () { var o = {}; with (o) { return typeof toString; } });
p(function () { var o = Object.create(null); with (o) { return typeof toString; } });

// --- shadowing an outer var, let and parameter
p(function () { var v = 1; with ({v: 2}) { return v; } });
p(function () { let l = 1; with ({l: 2}) { return l; } });
p(function () { return (function (q) { with ({q: 2}) { return q; } })(1); });
p(function () { return (function (q) { with ({}) { return q; } })(1); });
p(function () { const c = 1; with ({c: 2}) { return c; } });
p(function () { let l = 1; with ({}) { return l; } });
p(function () { var v = 1; { let v = 2; with ({}) { return v; } } });
p(function () { var v = 1; { let v = 2; with ({v: 3}) { return v; } } });

// --- assignment in both directions
p(function () { var o = {a: 1}; var a = "outer"; with (o) { a = 9; } return o.a + "/" + a; });
p(function () { var o = {}; var a = "outer"; with (o) { a = 9; } return ("a" in o) + "/" + a; });
p(function () { var o = {a: 1}; with (o) { a += 1; } return o.a; });
p(function () { var a = 1; with ({}) { a += 1; } return a; });
p(function () { var o = {a: 1}; with (o) { a++; } return o.a; });
p(function () { var o = {a: 1}; var r; with (o) { r = a++; } return r + "/" + o.a; });
p(function () { var o = {a: 1}; var r; with (o) { r = ++a; } return r + "/" + o.a; });
p(function () { var a = 5; with ({}) { a--; } return a; });
p(function () { var base = {b: 1}; var o = Object.create(base); with (o) { b = 3; } return o.b + "/" + base.b + "/" + o.hasOwnProperty("b"); });
p(function () { var o = {}; Object.defineProperty(o, "g", {get: function () { return 4; }, set: function (v) { this.seen = v; }, configurable: true}); with (o) { g = 11; } return o.seen; });
p(function () { var o = {}; Object.defineProperty(o, "g", {get: function () { return 4; }, configurable: true}); with (o) { return g; } });
p(function () { var o = {a: 1}; var seen = []; with (o) { a = a + 1; } return o.a; });

// --- delete through a with
p(function () { var o = {d: 1}; var r; with (o) { r = delete d; } return r + "/" + ("d" in o); });
p(function () { var base = {d: 1}; var o = Object.create(base); var r; with (o) { r = delete d; } return r + "/" + o.d; });
p(function () { var d = 1; var r; with ({}) { r = delete d; } return String(r); });
p(function () { var o = {d: 1}; with (o) { delete d; return typeof d; } });
p(function () { var d = "outer"; var o = {d: 1}; with (o) { delete d; return d; } });

// --- nesting
p(function () { with ({a: 1}) { with ({b: 2}) { return a + b; } } });
p(function () { with ({a: 1}) { with ({a: 2}) { return a; } } });
p(function () { var a = 0; with ({a: 1}) { with ({}) { return a; } } });
p(function () { with ({}) { with ({}) { return typeof missing; } } });
p(function () { with ({a: 1}) { with ({b: 2}) { a = 5; } } return "done"; });
p(function () { var o1 = {a: 1}; var o2 = {}; with (o1) { with (o2) { a = 5; } } return o1.a + "/" + ("a" in o2); });

// --- what the object may be
p(function () { with ("abc") { return length + ":" + charAt(1); } });
p(function () { with (new String("abc")) { return length; } });
p(function () { with (new Number(5)) { return toFixed(2); } });
p(function () { with (new Boolean(true)) { return valueOf(); } });
p(function () { with (42) { return toFixed(1); } });
p(function () { with (true) { return String(valueOf()); } });
p(function () { with ([1, 2, 3]) { return length; } });
p(function () { with ([1, 2, 3]) { return join("-"); } });
p(function () { var arr = [1, 2]; with (arr) { push(3); } return arr.length; });
p(function () { try { with (null) { return 1; } } catch (e) { return e.name; } });
p(function () { try { with (undefined) { return 1; } } catch (e) { return e.name; } });
p(function () { var seen = 0; try { with (null) { return 1; } } catch (e) { seen = 1; } return seen; });
p(function () { with (function () { return 1; }) { return typeof call; } });
p(function () { with (Math) { return floor(PI); } });
p(function () { with (JSON) { return stringify({a: 1}); } });

// --- Symbol.unscopables
p(function () { var o = {u: 1}; o[Symbol.unscopables] = {u: true}; var u = "outer"; with (o) { return u; } });
p(function () { var o = {u: 1}; o[Symbol.unscopables] = {u: false}; var u = "outer"; with (o) { return u; } });
p(function () { var o = {u: 1}; o[Symbol.unscopables] = {u: 0}; var u = "outer"; with (o) { return u; } });
p(function () { var o = {u: 1}; o[Symbol.unscopables] = {}; var u = "outer"; with (o) { return u; } });
p(function () { var o = {u: 1}; o[Symbol.unscopables] = null; var u = "outer"; with (o) { return u; } });
p(function () { var o = {u: 1}; o[Symbol.unscopables] = {u: true}; with (o) { return typeof u; } });
p(function () { var o = {u: 1}; o[Symbol.unscopables] = {u: true}; var u = "outer"; with (o) { u = "written"; } return o.u + "/" + u; });
p(function () { var o = {u: 1, v: 2}; o[Symbol.unscopables] = {u: true}; with (o) { return v; } });
p(function () { var base = {}; base[Symbol.unscopables] = {u: true}; var o = Object.create(base); o.u = 1; var u = "outer"; with (o) { return u; } });
p(function () { var values = "outer"; with ([1, 2]) { return typeof values; } });
p(function () { var keys = "outer"; with ([1, 2]) { return keys; } });
p(function () { var join = "outer"; with ([1, 2]) { return typeof join; } });
p(function () { var o = {u: 1}; Object.defineProperty(o, Symbol.unscopables, {get: function () { throw new RangeError("blocklist"); }}); var u = "outer"; with (o) { return u; } });

// --- an accessor that throws, and one with a side effect
p(function () { var o = {}; Object.defineProperty(o, "t", {get: function () { throw new RangeError("boom"); }, configurable: true}); with (o) { return t; } });
p(function () { var o = {}; Object.defineProperty(o, "t", {get: function () { throw new RangeError("boom"); }, configurable: true}); try { with (o) { return t; } } catch (e) { return e.name; } });
p(function () { var calls = 0; var o = {}; Object.defineProperty(o, "t", {get: function () { calls++; return calls; }, configurable: true}); with (o) { t; t; } return calls; });
p(function () { var o = {}; Object.defineProperty(o, "t", {set: function (v) { throw new RangeError("nope"); }, get: function () { return 0; }, configurable: true}); try { with (o) { t = 1; } } catch (e) { return e.name; } return "no-throw"; });

// --- loops, break, continue and labels
p(function () { var acc = ""; for (var i = 0; i < 3; i++) { with ({q: i}) { acc += q; } } return acc; });
p(function () { var acc = ""; for (var i = 0; i < 4; i++) { with ({q: i}) { if (q === 2) { continue; } acc += q; } } return acc; });
p(function () { var acc = ""; for (var i = 0; i < 4; i++) { with ({q: i}) { if (q === 2) { break; } acc += q; } } return acc; });
p(function () { var acc = ""; outer: for (var i = 0; i < 3; i++) { for (var j = 0; j < 3; j++) { with ({q: j}) { if (q === 1) { continue outer; } acc += "" + i + q; } } } return acc; });
p(function () { var acc = ""; outer: for (var i = 0; i < 3; i++) { with ({q: i}) { if (q === 2) { break outer; } acc += q; } } return acc; });
p(function () { var acc = ""; var i = 0; while (i < 3) { with ({q: i}) { acc += q; } i++; } return acc; });
p(function () { var acc = ""; done: { with ({}) { break done; } } return "left"; });
p(function () { var acc = ""; for (var k in {x: 1, y: 2}) { with ({q: k}) { acc += q; } } return acc; });
p(function () { var acc = ""; for (var e of [1, 2]) { with ({q: e}) { acc += q; } } return acc; });
p(function () { var acc = ""; do { with ({q: 1}) { acc += q; } } while (false); return acc; });
p(function () { var s = 0; for (var i = 0; i < 3; i++) { with ([i]) { s += length; } } return s; });

// --- return out of a with
p(function () { with ({r: 5}) { return r; } });
p(function () { var fin = ""; try { with ({r: 5}) { return r; } } finally { fin = "ran"; } });
p(function () { function inner() { with ({r: 5}) { return r; } } return inner(); });
p(function () { function inner() { with ({}) { return 1; } return 2; } return inner(); });
p(function () { var seen = []; function inner() { try { with ({}) { return "a"; } } finally { seen.push("f"); } } var v = inner(); return v + seen.join(""); });

// --- exceptions through a with
p(function () { try { with ({}) { throw new RangeError("x"); } } catch (e) { return e.name; } });
p(function () { try { with ({e: 1}) { throw new RangeError("x"); } } catch (e) { return typeof e; } });
p(function () { var acc = ""; try { with ({a: 1}) { acc += a; throw new RangeError("x"); } } catch (e) { acc += "c"; } finally { acc += "f"; } return acc; });
p(function () { var o = {a: 1}; try { with (o) { throw new RangeError("x"); } } catch (e) { return typeof a; } });
p(function () { try { with ({}) { with ({}) { throw new RangeError("x"); } } } catch (e) { return e.name; } return "no"; });
p(function () { var acc = ""; try { with ({}) { try { throw new RangeError("x"); } finally { acc += "i"; } } } catch (e) { acc += "o"; } return acc; });

// --- var and function declarations inside a with
p(function () { with ({}) { var v = 3; } return v; });
p(function () { var o = {v: 1}; with (o) { var v = 3; } return o.v + "/" + typeof v; });
p(function () { with ({}) { function g() { return 8; } } return g(); });
p(function () { var o = {g: 1}; with (o) { function g() { return 8; } } return typeof g; });
p(function () { with ({}) { var a = 1, b = 2; } return a + b; });
p(function () { var o = {a: 0}; with (o) { var a; } return o.a; });

// --- closures made inside a with
p(function () { var o = {c: 1}; var made; with (o) { made = function () { return c; }; } o.c = 2; return made(); });
p(function () { var o = {c: 1}; var made; with (o) { made = function () { return c; }; } delete o.c; var c = "outer"; return made(); });
p(function () { var c = "outer"; var made; with ({}) { made = function () { return c; }; } return made(); });
p(function () { var o = {c: 1}; var made; with (o) { made = function () { return typeof c; }; } delete o.c; return made(); });
p(function () { var o = {c: 1}; var made; with (o) { made = function () { c = 5; }; } made(); return o.c; });
p(function () { var o = {c: 1}; var made; with (o) { made = () => c; } o.c = 3; return made(); });

// --- typeof
p(function () { with ({tk: 3}) { return typeof tk; } });
p(function () { with ({tk: "s"}) { return typeof tk; } });
p(function () { var tk = 1; with ({tk: "s"}) { return typeof tk; } });
p(function () { with ({}) { return typeof tk2; } });
p(function () { with ({tk: undefined}) { return typeof tk; } });

// --- the object changing between two reads
p(function () { var o = {}; var g = "outer"; var acc = []; with (o) { acc.push(g); o.g = "inner"; acc.push(g); delete o.g; acc.push(g); } return acc.join(","); });
p(function () { var o = {}; var acc = []; with (o) { acc.push(typeof g2); o.g2 = 1; acc.push(typeof g2); } return acc.join(","); });
p(function () { var o = {}; var w = "outer"; with (o) { w = "first"; o.w = "planted"; w = "second"; } return w + "/" + o.w; });
p(function () { var o = {}; var f = function () { return "outer"; }; with (o) { var a = f(); o.f = function () { return "inner"; }; var b = f(); } return a + "/" + b; });

// --- the receiver a call through a with gets
p(function () { var o = {f: function () { return this === o; }}; with (o) { return f(); } });
p(function () { var o = {}; var f = function () { return this === undefined; }; with (o) { return f(); } });
p(function () { var o = {f: function () { return this === o; }}; with (o) { return (0, f)(); } });
p(function () { var base = {f: function () { return this; }}; var o = Object.create(base); with (o) { return f() === o; } });
p(function () { var o = {n: 2, f: function () { return this.n; }}; with (o) { return f(); } });

// --- with and the strictness boundary
// The four below compare THAT the source is refused and not what the refusal is called: this host
// maps every compile refusal reaching `eval` to an `EvalError`, whatever the construct, which is a
// property of its dynamic surface and is the same for `eval("1 +")`.
p(function () { var run = eval; try { run('"use strict"; with ({}) { }'); } catch (e) { return "refused"; } return "no-throw"; });
p(function () { var run = eval; try { run('function sf() { "use strict"; with ({}) { } }'); } catch (e) { return "refused"; } return "no-throw"; });
p(function () { var run = eval; try { run('class SC { m() { with ({}) { } } }'); } catch (e) { return "refused"; } return "no-throw"; });
p(function () { var run = eval; try { run('with ({}) let z = 1;'); } catch (e) { return "refused"; } return "no-throw"; });
p(function () { var run = eval; return run('with ({a: 1}) { a; }'); });

// --- odds and ends
p(function () { var o = {a: 1}; with (o) { return "a" in o; } });
p(function () { var o = {a: 1}; with (o) { return this === undefined ? "undefined" : typeof this; } });
p(function () { var o = {a: 1}; var s = 0; with (o) { s = a; } with ({a: 2}) { s += a; } return s; });
p(function () { var o = {a: 1}; with (o) { } return o.a; });
p(function () { var log = []; var o = {get a() { log.push("g"); return 1; }}; with (o) { a; a; } return log.join(""); });
p(function () { var o = {a: 1}; with (o) { if (a) { return "yes"; } } return "no"; });
p(function () { var o = {a: 1}; with (o) { switch (a) { case 1: return "one"; default: return "other"; } } });
p(function () { var o = {a: 1}; with (o) { return (function () { return a; })(); } });
p(function () { var o = {a: 1}; with (o) { return [a, a].join("-"); } });
p(function () { var o = {a: 1}; with (o) { return {x: a}.x; } });
p(function () { var o = {a: 1}; with (o) { return `${a}!`; } });
p(function () { var o = {a: 1}; with (o) { return a > 0 ? "pos" : "neg"; } });
p(function () { var o = {a: 1}; with (o) { try { return a; } finally { } } });
p(function () { var counter = 0; var o = {}; with (o) { for (var i = 0; i < 3; i++) { counter += i; } } return counter; });
p(function () { var o = {Object: {keys: function () { return "shadowed"; }}}; with (o) { return Object.keys({}); } });
p(function () { with ({undefined: 1}) { return typeof undefined; } });
p(function () { with ({NaN: 1}) { return NaN; } });
// --- interaction with the constructs admitted beside it
p(function () { function* g() { with ({y: 1}) { yield y; yield y + 1; } } var it = g(); return it.next().value + "," + it.next().value + "," + it.next().done; });
p(function () { function* g() { var o = {y: 1}; with (o) { yield y; o.y = 9; yield y; } } var it = g(); return it.next().value + "," + it.next().value; });
p(function () { function* g() { try { with ({}) { yield 1; } } finally { } } var it = g(); it.next(); return String(it.return(5).value); });
p(function () { var o = {a: 1}; var f = () => { with (o) { return a; } }; return f(); });
p(function () { var a = "outer"; var f; with ({a: 1}) { f = () => a; } return f(); });
p(function () { class K { m() { return 1; } } var o = {a: 1}; with (o) { return new K().m() + a; } });
p(function () { var o = {a: 1}; with (o) { class L { n() { return a; } } return new L().n(); } });
p(function () { var o = {}; with (o) { return (function () { return arguments.length; })(1, 2); } });
p(function () { return (function () { with ({}) { return arguments[0]; } })(7); });
p(function () { return (function () { with ({arguments: "shadow"}) { return arguments; } })(7); });
p(function () { var o = {a: 1}; with (o) { return [...[a, 2]].join("-"); } });
p(function () { var o = {a: 1}; with (o) { var [x, y] = [a, a + 1]; } return x + "" + y; });
p(function () { var o = {a: 1}; with (o) { return ((b = a) => b)(); } });
p(function () { var acc = ""; var o = {}; for (var e of [1, 2, 3]) { with (o) { if (e === 2) { break; } acc += e; } } return acc; });
p(function () { var closed = []; var it = {[Symbol.iterator]: function () { var i = 0; return {next: function () { return {value: i++, done: i > 3}; }, "return": function () { closed.push("r"); return {done: true}; }}; }}; for (var e of it) { with ({}) { break; } } return closed.join(""); });
p(function () { var o = {}; with (o) { with (o) { with (o) { return typeof deep; } } } });
p(function () { var d = "outer"; var o = {}; with ({}) { with ({}) { with ({}) { return d; } } } });
p(function () { var d = "outer"; var o = {d: "inner"}; with (o) { with ({}) { with ({}) { return d; } } } });
p(function () { var o = {a: 1}; with (o) { return this === undefined ? "u" : "g"; } });
p(function () { var o = {a: 1}; return (function () { with (o) { return this === undefined ? "u" : "g"; } }).call({}); });
p(function () { var o = {a: 1}; with (o) { label: { if (a) { break label; } return "no"; } } return "left"; });
p(function () { var acc = ""; outer: for (var i = 0; i < 2; i++) { with ({}) { with ({}) { continue outer; } } } return "done"; });
p(function () { var o = {a: 1}; var s = ""; with (o) { for (var k in o) { s += k; } } return s; });
p(function () { var o = {a: 1}; try { with (o) { null.x; } } catch (e) { return e.name; } });
p(function () { var o = {a: 1}; with (o) { try { throw a; } catch (a) { return "caught" + a; } } });
p(function () { var o = {a: 1}; with (o) { try { throw 5; } catch (e) { return a + e; } } });
p(function () { var o = {a: 1}; var n = 0; with (o) { n = a; } with (o) { n += a; } return n; });
p(function () { var o = {a: 1}; return typeof (function () { with (o) { return a; } }); });
p(function () { var o = {toString: function () { return "T"; }}; with (o) { return toString(); } });
p(function () { var o = {valueOf: function () { return 3; }}; with (o) { return valueOf() + 1; } });
// top-level `with` whose body is a bare expression statement, and the program completion value
p(function () { var o = {a: 1}; var seen; with (o) seen = a; return seen; });
p(function () { var o = {a: 1}; with (o) if (a) { return "took"; } return "not"; });
p(function () { var o = {}; var s = ""; with (o) { for (var e of [1, 2]) { s += e; } } return s; });
p(function () { var o = {e: 9}; var s = ""; with (o) { for (var e of [1, 2]) { s += e; } } return s + "/" + o.e; });
p(function () { var o = {k: 9}; var s = ""; with (o) { for (var k in {x: 1}) { s += k; } } return s + "/" + o.k; });
p(function () { var o = {}; var s = ""; with (o) { for (let i = 0; i < 3; i++) { s += i; } } return s; });
p(function () { var o = {i: 9}; var s = ""; with (o) { for (var i = 0; i < 3; i++) { s += i; } } return s + "/" + o.i; });
p(function () { var o = {}; var fs = []; with (o) { for (let i = 0; i < 3; i++) { fs.push(function () { return i; }); } } return fs[0]() + "" + fs[2](); });
p(function () { var o = {a: 1}; switch (1) { case 1: with (o) { return a; } } });
p(function () { var o = {a: 1}; with (o) { var q = { get x() { return a; } }; return q.x; } });
p(function () { var o = {a: 1}; try { with (o) { return a; } } catch (e) { return "no"; } finally { } });
p(function () { var o = {a: 1}; var r = []; with (o) { do { r.push(a); } while (false); } return r.join(""); });
p(function () { var o = {a: 1}; with (o) { with ({}) { return typeof a; } } });
p(function () { var deep = "d"; function outerFn() { with ({}) { return function () { with ({}) { return deep; } }; } } return outerFn()(); });
p(function () { var o = {a: 1}; with (o) { return (function () { return function () { return a; }; })()(); } });
