// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE STATEMENT, SCOPING AND OBJECT SURFACE.
//
// Hoisting and closure capture, labelled break and continue, `switch` fallthrough, `for … in`
// order, the comma operator and evaluation order, `try`/`catch`/`finally` in every combination
// including an abrupt completion inside a finaliser, property descriptors and own-key order,
// accessors, prototypes, `this` in each of the ways it is bound, array holes, and number
// formatting at the edges.
//
// It is the probe that found the empty protected range: `try { } catch (e) { }` lowered to an
// exception region whose start equalled its end, and the verifier refused an artifact this
// lowering had just produced.
//
// Each case prints its own number, so a divergence names a case rather than a line, and the
// numbering survives a case being rewritten in place. A case that throws prints the error's name,
// because a refusal is an answer and a probe that stopped at the first one would compare nothing
// after it.

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name + (e.message ? "" : ""); } }
function p(f) { __n++; print(__n + " " + t(f)); }

// --- scoping and hoisting
p(function () { var s = ""; for (var i = 0; i < 3; i++) { s += i; } return s + i; });
p(function () { var fs = []; for (var i = 0; i < 3; i++) fs.push(function () { return i; }); return fs[0]() + "" + fs[2](); });
p(function () { var fs = []; for (let i = 0; i < 3; i++) fs.push(function () { return i; }); return fs[0]() + "" + fs[2](); });
p(function () { return typeof x; var x = 1; });
p(function () { try { return y; } catch (e) { return e.name; } let y = 1; });
p(function () { { let a = 1; } try { return a; } catch (e) { return e.name; } });
p(function () { var a = 1; { var a = 2; } return a; });
p(function () { function g() { return 1; } { function g() { return 2; } } return g(); });
p(function () { return (function () { return typeof arguments; })(); });
p(function () { return (function f(n) { return n <= 1 ? 1 : n * f(n - 1); })(5); });
p(function () { var c = 0; function inc() { c++; return c; } inc(); inc(); return c; });
p(function () { "use strict"; try { undeclared = 1; } catch (e) { return e.name; } return "no-throw"; });
p(function () { sloppyGlobal = 5; return sloppyGlobal; });

// --- control flow
p(function () { var s = ""; outer: for (var i = 0; i < 3; i++) { for (var j = 0; j < 3; j++) { if (j === 1) continue outer; s += "" + i + j; } } return s; });
p(function () { var s = ""; outer: for (var i = 0; i < 3; i++) { for (var j = 0; j < 3; j++) { if (i === 1) break outer; s += "" + i + j; } } return s; });
p(function () { var s = ""; switch (2) { case 1: s += "a"; case 2: s += "b"; case 3: s += "c"; break; default: s += "d"; } return s; });
p(function () { switch ("2") { case 2: return "loose"; default: return "strict"; } });
p(function () { var i = 0; do { i++; } while (i < 3); return i; });
p(function () { var s = ""; for (var k in {a:1,b:2}) { s += k; } return s; });
p(function () { var s = ""; for (var k in [7,8]) { s += k + ":"; } return s; });
p(function () { var s = ""; for (var k in "ab") { s += k; } return s; });
p(function () { var s = ""; for (var k in null) { s += k; } return s + "|"; });
p(function () { return (1, 2, 3); });
p(function () { var a = 1; a += (a = 2); return a; });
p(function () { var o = {n: 1}; o.n += (o.n = 5); return o.n; });
p(function () { var i = 0; var a = [0,0]; a[i++] = i; return a.join(",") + ":" + i; });
p(function () { return true ? false ? 1 : 2 : 3; });
p(function () { var x = null; return x?.y; });
p(function () { var x = null; return x ?? "d"; });
p(function () { var x = 0; return x || "d"; });
p(function () { var x = 0; return x ?? "d"; });

// --- exceptions
p(function () { try { throw 1; } catch (e) { return typeof e; } });
p(function () { try { try { throw 1; } finally { } } catch (e) { return "outer" + e; } });
p(function () { function g() { try { return "t"; } finally { return "f"; } } return g(); });
p(function () { function g() { try { throw 1; } finally { return "f"; } } return g(); });
p(function () { var s = ""; try { s += "t"; throw 1; } catch (e) { s += "c"; } finally { s += "f"; } return s; });
p(function () { function g() { for (;;) { try { break; } finally { return "f"; } } return "b"; } return g(); });
p(function () { try { throw new TypeError("m"); } catch (e) { return e instanceof TypeError && e instanceof Error; } });
p(function () { var e2; try { null.x; } catch (e) { e2 = e; } return e2 instanceof TypeError; });
p(function () { try { throw {name:"X"}; } catch (e) { return e.name; } });
p(function () { function g() { try { return 1; } finally { } } return g(); });
p(function () { var s = ""; function g() { try { s += "a"; return s; } finally { s += "b"; } } g(); return s; });
p(function () { try { } catch (e) { } return "ok"; });

// --- objects and properties
p(function () { var o = {}; Object.defineProperty(o, "x", {value: 1}); return Object.keys(o).length; });
p(function () { var a = [1,2,3]; Object.defineProperty(a, "length", {value: 1}); return a.join(","); });
p(function () { var a = [1]; try { Object.defineProperty(a, "length", {value: 1, writable: false}); a.push(2); } catch (e) { return e.name; } return a.length; });
p(function () { var o = {a:1}; var q = Object.create(o); q.a = 2; return o.a + "," + q.a; });
p(function () { var o = Object.create({set a(v) { this.got = v; }}); o.a = 7; return o.got + "," + o.hasOwnProperty("a"); });
p(function () { var o = {}; Object.defineProperty(o, "x", {get: function () { throw new RangeError("g"); }}); try { o.x; } catch (e) { return e.name; } });
p(function () { var o = {a:1, 2:2, 1:1, b:2}; return Object.keys(o).join(","); });
p(function () { var o = {}; o[1.0] = "a"; return Object.keys(o)[0]; });
p(function () { var o = {}; o[-0] = "a"; return Object.keys(o)[0]; });
p(function () { var o = {}; o[1e21] = "a"; return Object.keys(o)[0]; });
p(function () { var o = {a:1}; return JSON.stringify(Object.getOwnPropertyDescriptor(o, "a")); });
p(function () { return "a" in {a: undefined}; });
p(function () { var a = [1,2]; delete a[0]; return a.length + ":" + (0 in a); });
p(function () { var o = {}; Object.defineProperty(o, "x", {value:1, configurable:false}); return delete o.x; });
p(function () { "use strict"; var o = {}; Object.defineProperty(o, "x", {value:1, configurable:false}); try { delete o.x; } catch (e) { return e.name; } return "no-throw"; });
p(function () { var o = {}; o.__proto__ = null; return Object.getPrototypeOf(o); });
p(function () { return ({}).__proto__ === Object.prototype; });
p(function () { var o = {a:1}; var n = 0; for (var k in o) { o.b = 2; n++; } return n; });
p(function () { return Object.getOwnPropertyNames(function (a,b) {}).sort().join(","); });
p(function () { function F() {} return F.prototype.constructor === F; });
p(function () { function F() {} var o = new F(); return o instanceof F; });
p(function () { function F() { return {a:1}; } return new F().a; });
p(function () { function F() { return 5; } return new F() instanceof F; });
p(function () { function F() { this.x = 1; } F.prototype = null; return typeof new F(); });

// --- functions and this
p(function () { var o = {f: function () { return this === o; }}; return o.f(); });
p(function () { var o = {f: function () { return this; }}; var g = o.f; return g() === globalThis; });
p(function () { "use strict"; var o = {f: function () { return this; }}; var g = o.f; return g(); });
p(function () { var o = {a: 1, f: function () { var s = function () { return this && this.a; }; return s(); }}; return String(o.f()); });
p(function () { return (function () { return typeof this; }).call(5); });
p(function () { "use strict"; return (function () { return typeof this; }).call(5); });
p(function () { function f() {} f.x = 1; return Object.keys(f).join(","); });
p(function () { return Function.prototype.call.call(function () { return this.v; }, {v:9}); });
p(function () { function f(a,b,c) {} return f.bind(null,1).length; });
p(function () { return typeof (function () {}).prototype; });
p(function () { return typeof (function () {}).bind(null).prototype; });
p(function () { var f = function () {}; return f.hasOwnProperty("prototype"); });
p(function () { return [].constructor === Array; });
p(function () { return (5).constructor.name; });
p(function () { return "".constructor.name; });

// --- getters, setters, accessors on arrays
p(function () { var a = []; a.length = 3; return a.join(","); });
p(function () { var a = [1,2,3]; a[1] = undefined; return JSON.stringify(a); });
p(function () { var a = [1,2,3]; delete a[1]; return JSON.stringify(a); });
p(function () { var a = [1,2,3]; return a.map(function (x) { return x; }).length; });
p(function () { var a = [1,,3]; var n = 0; a.forEach(function () { n++; }); return n; });
p(function () { var a = [1,,3]; return a.map(function (x) { return 1; }).join(","); });
p(function () { var a = [1,,3]; return 1 in a.map(function (x) { return 1; }); });
p(function () { var a = [1,2]; a.foo = 1; var s = ""; for (var k in a) s += k; return s; });

// --- number formatting
p(function () { return String(1/3); });
p(function () { return String(100000000000000000000); });
p(function () { return String(1e-7); });
p(function () { return String(-1e-7); });
p(function () { return String(5e-324); });
p(function () { return String(1.7976931348623157e308); });
p(function () { return (0.1).toString(3); });
p(function () { return String(0.000001); });
p(function () { return String(123456789012345678901234567890); });
p(function () { return (1e-10).toExponential(); });
p(function () { return JSON.stringify(1e-7); });
p(function () { return String(Number.MIN_VALUE); });

// --- strict mode, whose runtime half was enforced later than its grammar half. APPENDED
// rather than inserted: a case number is how a divergence is named, and renumbering would
// silently move every declaration that points at one.
p(function(){ "use strict"; return typeof hoisted; function hoisted(){} });
p(function(){ "use strict"; var o = {}; return typeof o.x; });
p(function(){ "use strict"; try { undeclaredHere = 1; return "no-throw"; } catch (e) { return e.name; } });
p(function(){ "use strict"; var q = 1; return q; });
p(function(){ "use strict"; function inner(){ return 5; } return inner(); });
p(function(){ "use strict"; return (function(){ function deep(){ return 6; } return deep(); })(); });
p(function(){ function sloppyHoisted(){ return 7; } return sloppyHoisted(); });
p(function(){ "use strict"; let l = 8; return l; });
p(function(){ "use strict"; const c = 9; return c; });
p(function(){ "use strict"; class K { m(){ return 10; } } return new K().m(); });
p(function(){ "use strict"; function* g(){ yield 11; } return g().next().value; });
p(function(){ "use strict"; var o = Object.freeze({a:1}); try { o.a = 2; return "no-throw"; } catch (e) { return e.name; } });
p(function(){ "use strict"; var o = {}; Object.defineProperty(o,"x",{value:1,configurable:false}); try { delete o.x; return "no-throw"; } catch (e) { return e.name; } });
