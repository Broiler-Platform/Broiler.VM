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

// --- the reflective namespace, and the one object-literal member that is not a member.
// APPENDED rather than inserted, for the reason the section above gives: a case number is how a
// divergence is named, and renumbering would move every declaration that points at one.
p(function(){ return typeof Reflect; });
p(function(){ return Object.prototype.toString.call(Reflect); });
p(function(){ return Reflect.get({a:1}, "a"); });
p(function(){ return String(Reflect.get({}, "missing")); });
p(function(){ var o = Object.create({inherited: 7}); return Reflect.get(o, "inherited"); });
p(function(){ var o = {get x(){ return this.tag; }}; return Reflect.get(o, "x", {tag: "receiver"}); });
p(function(){ var o = {}; return Reflect.set(o, "a", 1) + "," + o.a; });
p(function(){ var o = Object.freeze({a:1}); return String(Reflect.set(o, "a", 2)); });
p(function(){ return String(Reflect.has({a:1}, "a")) + String(Reflect.has({}, "a")); });
p(function(){ return String(Reflect.has({}, "toString")); });
p(function(){ var s = Symbol.iterator; return String(Reflect.has([], s)); });
p(function(){ var o = {a:1}; return String(Reflect.deleteProperty(o, "a")) + "," + String(o.a); });
p(function(){ var o = {}; Object.defineProperty(o, "x", {value:1, configurable:false}); return String(Reflect.deleteProperty(o, "x")); });
p(function(){ return Reflect.ownKeys({b:1, a:2, 2:3, 1:4}).join(","); });
p(function(){ var s = Symbol("k"); var o = {a:1}; o[s] = 2; return Reflect.ownKeys(o).length; });
p(function(){ return String(Reflect.getPrototypeOf([]) === Array.prototype); });
p(function(){ return String(Reflect.getPrototypeOf(Object.create(null))); });
p(function(){ var o = {}; Reflect.setPrototypeOf(o, Array.prototype); return String(Object.getPrototypeOf(o) === Array.prototype); });
p(function(){ var o = {}; return String(Reflect.isExtensible(o)) + String(Reflect.preventExtensions(o)) + String(Reflect.isExtensible(o)); });
p(function(){ var o = {}; Reflect.defineProperty(o, "x", {value: 5, enumerable: true}); return o.x; });
p(function(){ var o = {}; return String(Reflect.defineProperty(Object.freeze(o), "y", {value:1})); });
p(function(){ var d = Reflect.getOwnPropertyDescriptor({a:1}, "a"); return d.value + "," + d.writable + "," + d.enumerable + "," + d.configurable; });
p(function(){ return String(Reflect.getOwnPropertyDescriptor({}, "a")); });
p(function(){ return Reflect.apply(Math.max, null, [1, 5, 3]); });
p(function(){ return Reflect.apply(function(){ return this.v; }, {v: 9}, []); });
p(function(){ function C(a){ this.a = a; } return Reflect.construct(C, [4]).a; });
p(function(){ return Reflect.construct(Array, [3]).length; });
p(function(){ function C(){ } function D(){ } var o = Reflect.construct(C, [], D); return String(Object.getPrototypeOf(o) === D.prototype); });
p(function(){ try { Reflect.get(1, "a"); return "no-throw"; } catch (e) { return e.name; } });
p(function(){ try { Reflect.apply(1, null, []); return "no-throw"; } catch (e) { return e.name; } });
p(function(){ try { Reflect.construct(Math.max, []); return "no-throw"; } catch (e) { return e.name; } });
p(function(){ try { Reflect.apply(Math.max, null, 1); return "no-throw"; } catch (e) { return e.name; } });
p(function(){ return Reflect.get.length + "," + Reflect.set.length + "," + Reflect.has.length + "," + Reflect.apply.length + "," + Reflect.construct.length; });
p(function(){ return Reflect.ownKeys.length + "," + Reflect.defineProperty.length + "," + Reflect.getPrototypeOf.length; });
p(function(){ var s = Symbol("v"); var o = {}; o[s] = 1; return String(Reflect.get(o, s)); });
p(function(){ var s = Symbol("v"); var o = {}; Reflect.set(o, s, 3); return String(o[s]); });
p(function(){ var s = Symbol("v"); var o = {}; o[s] = 1; Reflect.deleteProperty(o, s); return String(o[s]); });
p(function(){ var s = Symbol("v"); var o = {}; o[s] = 1; var d = Reflect.getOwnPropertyDescriptor(o, s); return String(d.value); });
p(function(){ var s = Symbol("v"); var o = {}; Reflect.defineProperty(o, s, {value: 8}); return String(o[s]); });
p(function(){ return String(Reflect.ownKeys([1,2]).join(",")); });
p(function(){ return String(Reflect.ownKeys(function f(a,b){}).indexOf("length") >= 0); });
p(function(){ return Object.getOwnPropertySymbols({}).length; });
p(function(){ var s = Symbol("x"); var o = {}; o[s] = 1; return Object.getOwnPropertySymbols(o)[0] === s; });
p(function(){ var o = {}; o[Symbol.iterator] = 1; o[Symbol.toStringTag] = "T"; return Object.getOwnPropertySymbols(o).length; });
p(function(){ return Object.getOwnPropertySymbols.length; });
p(function(){ var o = {}; o[Symbol.toStringTag] = "Custom"; return Object.prototype.toString.call(o); });
p(function(){ return Object.prototype.toString.call(Math); });
p(function(){ return Object.prototype.toString.call(JSON); });
p(function(){ return Object.prototype.toString.call([]) + Object.prototype.toString.call(null) + Object.prototype.toString.call(undefined); });
p(function(){ function* g(){} return Object.prototype.toString.call(g()); });
p(function(){ var o = {}; o[Symbol.toStringTag] = 5; return Object.prototype.toString.call(o); });
p(function(){ return Object.prototype.toString.call(new Map()) + Object.prototype.toString.call(new Set()); });
p(function(){ return Object.prototype.toString.call(Symbol()); });
p(function(){ return Object.prototype.toString.call(new Date()); });
p(function(){ return String(Object.getOwnPropertyDescriptor(Object.prototype, "__proto__") !== undefined); });
p(function(){ var o = {}; o.__proto__ = Array.prototype; return String(Object.getPrototypeOf(o) === Array.prototype); });
p(function(){ var o = {a:1}; return String(o.__proto__ === Object.prototype); });
p(function(){ var o = Object.create(null); o.__proto__ = 5; return String(Object.getPrototypeOf(o)); });
p(function(){ var o = {__proto__: Array.prototype}; return String(Object.getPrototypeOf(o) === Array.prototype); });
p(function(){ var o = {}; o.__proto__ = null; return String(Object.getPrototypeOf(o)); });

// --- `__proto__` in an object literal, which sets a prototype where every neighbouring spelling
// of it defines a property
p(function(){ var o = {__proto__: null}; return String(Object.getPrototypeOf(o)); });
p(function(){ var o = {__proto__: null}; return Object.keys(o).length; });
p(function(){ var o = {"__proto__": Array.prototype}; return String(Object.getPrototypeOf(o) === Array.prototype); });
p(function(){ var o = {__proto__: 5}; return String(Object.getPrototypeOf(o) === Object.prototype) + Object.keys(o).length; });
p(function(){ var o = {__proto__: undefined}; return String(Object.getPrototypeOf(o) === Object.prototype); });
p(function(){ var k = "__proto__"; var o = {[k]: Array.prototype}; return String(Object.getPrototypeOf(o) === Object.prototype) + "," + o.hasOwnProperty("__proto__"); });
p(function(){ var __proto__ = 5; var o = {__proto__}; return String(Object.getPrototypeOf(o) === Object.prototype) + "," + o.__proto__; });
p(function(){ var o = {__proto__(){ return 1; }}; return String(o.__proto__()) + "," + String(Object.getPrototypeOf(o) === Object.prototype); });
p(function(){ var o = JSON.parse('{"__proto__": 1}'); return String(Object.getPrototypeOf(o) === Object.prototype) + "," + o.hasOwnProperty("__proto__"); });
p(function(){ var src = JSON.parse('{"__proto__": 1}'); var o = {...src}; return String(o.hasOwnProperty("__proto__")); });
p(function(){ var o = {a: 1, __proto__: Array.prototype, b: 2}; return Object.keys(o).join(",") + "," + String(Object.getPrototypeOf(o) === Array.prototype); });
p(function(){ var o = {__proto__: Array.prototype}; return JSON.stringify(o); });
p(function(){ var o = {}; var d = Object.getOwnPropertyDescriptor(o, "__proto__"); return String(d); });
p(function(){ var o = {__proto__: {greet: function(){ return "hi"; }}}; return o.greet(); });
p(function(){ var o = Object.create(null); return String(o.__proto__); });
p(function(){ var got = {}; var o = {__proto__: got}; return String(Object.getPrototypeOf(o) === got); });
p(function(){ return String(Object.getPrototypeOf({__proto__: Array.prototype, __proto__x: 1}) === Array.prototype); });
p(function(){ var o = {get __proto__(){ return 1; }}; return String(o.__proto__) + "," + String(Object.getPrototypeOf(o) === Object.prototype); });
p(function(){ var o = {}; o["__proto__"] = Array.prototype; return String(Object.getPrototypeOf(o) === Array.prototype); });
p(function(){ var o = Object.create(null); o["__proto__"] = 1; return String(Object.getPrototypeOf(o)) + "," + o.__proto__; });

// --- the class BODY: fields, static blocks, private names and a generator member.
//
// The four families admitted on 2026-09-05 are here rather than in a probe of their own, because
// what each of them is FOR is an object graph the cases above already ask about from the outside: a
// field is an own property with a descriptor, a static block writes the constructor, a private
// element is the one thing on an object that none of the reflection above can see, and a generator
// member is a method with a `super`. Every case is one line, so a divergence names the rule rather
// than the class.
//
// EVERY EARLY-ERROR CASE USES INDIRECT `eval` AND NOT DIRECT. A direct `eval` inside a function is
// refused by this profile — it resolves every name at lowering — so a direct one would compare a
// refusal about `eval` against the other engine's answer about the class and find a difference that
// is not the one the case is asking about.
function thrown(f) { try { f(); return "no-throw"; } catch (e) { return e.name; } }

// A field's two times: the key with the class, the initialiser with the instance.
p(function(){ var log = []; function k(n){ log.push("k:" + n); return n; } class C { [k("a")] = log.push("i:a"); [k("b")] = 2; } new C(); return log.join(" "); });
p(function(){ class C { x; } return String(new C().x) + "," + Object.keys(new C()).join(","); });
p(function(){ class C { x = 1; } var d = Object.getOwnPropertyDescriptor(new C(), "x"); return String(d.writable) + d.enumerable + d.configurable; });
p(function(){ class C { x = this; } var c = new C(); return String(c.x === c); });
p(function(){ class C { x = 1; constructor(){ this.x += 1; } } return new C().x; });
p(function(){ class B { x = 1; } class D extends B { y = this.x + 1; } var d = new D(); return d.x + "," + d.y; });
p(function(){ class C { static x = this.name; } return C.x; });
p(function(){ class C { static x = 1; static y = C.x + 1; } return C.y; });
p(function(){ return thrown(function(){ (0, eval)("class Q1 { static [Q1.name] = 1; }"); }); });
p(function(){ class B { set x(v){ this.seen = v; } } class D extends B { x = 1; } var d = new D(); return String(d.seen) + "," + d.x + "," + d.hasOwnProperty("x"); });
p(function(){ var s = Symbol("k"); class C { [s] = 1; } return new C()[s] + "," + Object.getOwnPropertySymbols(new C()).length; });
p(function(){ class C { x = 1; } return JSON.stringify(new C()); });
p(function(){ class C { "a b" = 1; 0 = 2; } var c = new C(); return c["a b"] + "," + c[0] + "," + Object.keys(c).join("|"); });

// A static block: the ordered list it shares with the static fields, and its `this`.
p(function(){ var log = []; class C { static a = log.push("f"); static { log.push("b:" + (this === C)); } static c = log.push("g"); } return log.join(" "); });
p(function(){ class C { static { this.made = 1; } } return C.made; });
p(function(){ class C { static a = 1; static { this.b = this.a + 1; } } return C.b; });
p(function(){ class C { static { C.named = 2; } } return C.named; });
p(function(){ class C { static {} } return Object.getOwnPropertyNames(C).join(","); });

// Private names: not properties, minted per class evaluation, and what each kind refuses.
p(function(){ class C { #x = 1; read(){ return this.#x; } } return new C().read(); });
p(function(){ class C { #x = 1; visible = 2; } var c = new C(); return Object.keys(c).join(",") + "," + Object.getOwnPropertyNames(c).length + "," + Reflect.ownKeys(c).length + "," + Object.getOwnPropertySymbols(c).length; });
p(function(){ class C { #x = 1; visible = 2; } return JSON.stringify(new C()); });
p(function(){ class C { #x = 1; } var seen = []; for (var k in new C()) { seen.push(k); } return seen.length; });
p(function(){ class C { #x = 1; static has(o){ return #x in o; } } return String(C.has(new C())) + "," + String(C.has({})); });
p(function(){ class C { #x = 1; static read(o){ return o.#x; } } return thrown(function(){ C.read({}); }); });
p(function(){ class C { #x = 1; static has(o){ return #x in o; } } return thrown(function(){ C.has(5); }); });
p(function(){ function make(){ return class { #x = 1; static has(o){ return #x in o; } }; } var A = make(), B = make(); return String(A.has(new A())) + "," + String(A.has(new B())); });
p(function(){ class C { #x = 1; bump(){ return ++this.#x; } } var c = new C(); c.bump(); return c.bump(); });
p(function(){ class C { #x = 1; add(){ this.#x += 4; return this.#x; } } return new C().add(); });
p(function(){ class C { #m(){ return 7; } call(){ return this.#m(); } } return new C().call(); });
p(function(){ class C { #m(){ return 7; } write(){ this.#m = 1; } } return thrown(function(){ new C().write(); }); });
p(function(){ class C { #v = 1; get #a(){ return this.#v; } set #a(n){ this.#v = n * 2; } run(){ this.#a = 3; return this.#a; } } return new C().run(); });
p(function(){ class C { get #a(){ return 1; } read(){ return this.#a; } write(){ this.#a = 1; } } var c = new C(); return c.read() + "," + thrown(function(){ c.write(); }); });
p(function(){ class C { static #s = 5; static read(){ return C.#s; } } return C.read(); });
p(function(){ class C { static #m(){ return 6; } static call(){ return C.#m(); } } return C.call(); });
p(function(){ class C { static #s = 1; static holds(o){ return #s in o; } } return String(C.holds(C)) + "," + String(C.holds(new C())); });
p(function(){ class C { #x = 1; put(o){ ({ a: o.#x } = { a: 9 }); return o.#x; } } return new C().put(new C()); });
p(function(){ class Outer { #x = 1; run(){ class Inner { #x = 2; read(o){ return o.#x; } } return new Inner().read(new Inner()) + "," + this.#x; } } return new Outer().run(); });
p(function(){ class C { #x = 1; read(){ return this.#x; } } var c = Object.freeze(new C()); return c.read() + "," + Object.isFrozen(c); });

// The class body's own early errors, each a refusal rather than a wrong answer.
p(function(){ return thrown(function(){ (0, eval)("class Q2 { x = arguments; }"); }); });
p(function(){ return thrown(function(){ (0, eval)("class Q3 { static { arguments; } }"); }); });
p(function(){ return thrown(function(){ (0, eval)("class Q4 { static { return 1; } }"); }); });
p(function(){ return thrown(function(){ (0, eval)("class Q5 { #x; #x; }"); }); });
p(function(){ return thrown(function(){ (0, eval)("class Q6 { #m(){} static #m(){} }"); }); });
p(function(){ return thrown(function(){ (0, eval)("class Q7 { #constructor; }"); }); });
p(function(){ return thrown(function(){ (0, eval)("class Q8 { get constructor(){} }"); }); });
p(function(){ return thrown(function(){ (0, eval)("class Q9 { static prototype = 1; }"); }); });
p(function(){ return thrown(function(){ (0, eval)("class QA { m(){ return this.#absent; } }"); }); });
p(function(){ return thrown(function(){ (0, eval)("var qb = {}; qb.#x;"); }); });
p(function(){ return thrown(function(){ (0, eval)("class QC { #x; m(o){ return delete o.#x; } }"); }); });
p(function(){ class C { get #a(){ return 1; } set #a(v){} read(){ return this.#a; } } return new C().read(); });

// A generator member of a class body, which is the object literal's method plus `super` and `static`.
p(function(){ class B { base(){ return 10; } } class D extends B { *g(){ yield super.base(); yield 2; } } return [...new D().g()].join(","); });
p(function(){ class C { static *g(){ yield 1; yield 2; } } return [...C.g()].join(","); });
p(function(){ class C { *g(){} } return thrown(function(){ new (new C().g)(); }); });
p(function(){ class C { get(){ return 1; } set(){ return 2; } static(){ return 3; } async(){ return 4; } } var c = new C(); return c.get() + "" + c.set() + c.static() + c.async(); });
p(function(){ class C { get = 1; } return new C().get; });

// --- a logical assignment to a property, which this front end refused by name until 2026-09-05.
// APPENDED for the reason the numbering asks.
p(function(){ var o = {x:0}; o.x ||= 5; return o.x; });
p(function(){ var o = {x:1}; o.x ||= 5; return o.x; });
p(function(){ var o = {x:1}; o.x &&= 5; return o.x; });
p(function(){ var o = {x:0}; o.x &&= 5; return o.x; });
p(function(){ var o = {x:null}; o.x ??= 5; return o.x; });
p(function(){ var o = {x:0}; o.x ??= 5; return o.x; });
p(function(){ var o = {}; return String(o.x ??= 7) + o.x; });
p(function(){ var o = {x:0}; var r = (o.x ||= 9); return r + "," + o.x; });
p(function(){ var o = {x:2}; var r = (o.x ||= 9); return r + "," + o.x; });
p(function(){ var calls = 0; function f(){ calls++; return {x:0}; } f().x ||= 1; return calls; });
p(function(){ var o = {a:{b:0}}; o.a.b ||= 3; return o.a.b; });
p(function(){ var o = {}; var k = "key"; o[k] ||= 4; return o.key; });
p(function(){ var o = {key:1}; var k = "key"; o[k] ||= 4; return o.key; });
p(function(){ var o = {key:1}; var k = "key"; return (o[k] &&= 8) + "," + o.key; });
p(function(){ var o = {key:null}; var k = "key"; return (o[k] ??= 8) + "," + o.key; });
p(function(){ var n = 0; function key(){ n++; return "k"; } var o = {k:1}; o[key()] ||= 2; return n; });
p(function(){ var log = []; var o = { get x(){ log.push("get"); return 1; }, set x(v){ log.push("set"); } }; o.x ||= 2; return log.join(); });
p(function(){ var log = []; var o = { get x(){ log.push("get"); return 0; }, set x(v){ log.push("set:"+v); } }; o.x ||= 2; return log.join(); });
p(function(){ "use strict"; var o = Object.freeze({x:0}); try { o.x ||= 1; return "no-throw"; } catch(e){ return e.name; } });
p(function(){ "use strict"; var o = Object.freeze({x:1}); try { o.x ||= 2; return "no-throw"; } catch(e){ return e.name; } });
p(function(){ var a = [0,1]; a[0] ||= 5; a[1] ||= 6; return a.join(); });
p(function(){ var o = {x:0}; o.x ||= (function(){ return 3; })(); return o.x; });
p(function(){ var o = {x:{y:0}}; o.x.y ??= 1; return o.x.y; });
p(function(){ var o = {}; o.f ||= function(){ return 1; }; return o.f.name + o.f(); });

// --- the two early errors this front end did not make: a numeric separator that is not between two
// digits, and a regular-expression literal whose pattern is not one. Each is asked through an
// INDIRECT eval - `(0, eval)(s)` - because a source file carrying one would not compile and the
// probe would answer nothing at all, and because a direct eval is a construct this manifest refuses.
p(function(){ return 1_000_000 + "," + 0x1_0 + "," + 0b1_1 + "," + 0o1_7; });
p(function(){ return 1_0.5_5e1_0; });
p(function(){ try { return (0, eval)("1__0"); } catch(e){ return e.name; } });
p(function(){ try { return (0, eval)("1_"); } catch(e){ return e.name; } });
p(function(){ try { return (0, eval)("0x_1"); } catch(e){ return e.name; } });
p(function(){ try { return (0, eval)("0_1"); } catch(e){ return e.name; } });
p(function(){ try { return (0, eval)("1e_5"); } catch(e){ return e.name; } });
p(function(){ try { return (0, eval)("/(/"); } catch(e){ return e.name; } });
p(function(){ try { return (0, eval)("if (false) { /(/; }"); } catch(e){ return e.name; } });
p(function(){ try { return String((0, eval)("/a(b)c/").test("abc")); } catch(e){ return e.name; } });

// --- the global LEXICAL environment, which is not the global object. Every case here is asked
// through an INDIRECT eval for one reason: a probe is one script, and a script-level `let` written
// in this file would be in scope for every case below it. `(0, eval)(s)` evaluates in the global
// scope, which is exactly where the question is about.
p(function(){ (0, eval)("var glvar = 1;"); return typeof Object.getOwnPropertyDescriptor(globalThis, "glvar"); });
p(function(){ (0, eval)("let gllet = 1;"); return typeof Object.getOwnPropertyDescriptor(globalThis, "gllet"); });
p(function(){ (0, eval)("const glconst = 1;"); return typeof Object.getOwnPropertyDescriptor(globalThis, "glconst"); });
p(function(){ (0, eval)("class GlClass {}"); return typeof Object.getOwnPropertyDescriptor(globalThis, "GlClass"); });
p(function(){ return (0, eval)("let gl1 = 4; gl1"); });
p(function(){ try { return (0, eval)("gl2; let gl2;"); } catch(e){ return e.name; } });
p(function(){ try { return (0, eval)("gl3 = 1; let gl3;"); } catch(e){ return e.name; } });
p(function(){ try { return (0, eval)("let gl4 = gl4 + 1;"); } catch(e){ return e.name; } });
p(function(){ try { return (0, eval)("const gl5 = 1; gl5 = 2;"); } catch(e){ return e.name; } });
p(function(){ (0, eval)("const gl6 = 1;"); try { return (0, eval)("gl6 = 2;"); } catch(e){ return e.name; } });
p(function(){ (0, eval)("const gl7 = 1;"); return (0, eval)("gl7"); });
p(function(){ (0, eval)("let gl8;"); return String((0, eval)("gl8")); });
p(function(){ (0, eval)("let gl9 = 1;"); (0, eval)("function readsGl9(){ return gl9; }"); return (0, eval)("gl9 = 2; readsGl9()"); });
p(function(){ (0, eval)("let glA = 1;"); return (0, eval)("typeof glA"); });
p(function(){ return (0, eval)("typeof notDeclaredAnywhereAtAll"); });
p(function(){ (0, eval)("const glB = 1;"); var keys = Object.getOwnPropertyNames(globalThis); return String(keys.indexOf("glB") < 0); });

// --- a logical assignment whose target is a PRIVATE member, which was the one target shape the
// operator's admission left refused. The short circuit is the case worth writing down: a private
// METHOD is not writable, and `??=` over a method that is not nullish never reaches the store.
p(function(){ class K { #x = 0; run(){ this.#x ||= 7; return this.#x; } } return new K().run(); });
p(function(){ class K { #y = 5; run(){ this.#y &&= 9; return this.#y; } } return new K().run(); });
p(function(){ class K { #n = null; run(){ this.#n ??= 3; return this.#n; } } return new K().run(); });
p(function(){ class K { #m(){ return 1; } run(){ this.#m ??= 2; return typeof this.#m; } } return new K().run(); });
p(function(){ class K { #m(){ return 1; } run(){ try { this.#m &&= 2; return "no-throw"; } catch(e){ return e.name; } } } return new K().run(); });
p(function(){ var n = 0; class K { #x = 0; base(){ n++; return this; } run(){ this.base().#x ||= 1; return n; } } return new K().run(); });
p(function(){ class A { constructor(){ this.x = 0; } } class B extends A { run(){ super.x ||= 7; return this.x + "," + (super.x === undefined); } } return new B().run(); });
p(function(){ class A {} A.prototype.v = 0; class B extends A { run(){ super.v ||= 4; return this.v + "," + A.prototype.v; } } return new B().run(); });
p(function(){ var n = 0; class A {} A.prototype.k = null; class B extends A { run(){ function key(){ n++; return "k"; } super[key()] ??= 6; return n + "," + this.k; } } return new B().run(); });
p(function(){ class A {} A.prototype.t = 1; class B extends A { run(){ super.t &&= 8; return this.t + "," + A.prototype.t; } } return new B().run(); });

// --- `delete` over a bare name, which is a REFERENCE and is never evaluated. Each case is asked
// through an indirect eval so that the name it asks about is the global scope's, which is the only
// scope where the answer is about anything but a slot.
p(function(){ (0, eval)("var delv = 1;"); return String((0, eval)("delete delv")) + "," + (0, eval)("typeof delv"); });
p(function(){ globalThis.delg = 2; return String((0, eval)("delete delg")) + "," + typeof globalThis.delg; });
p(function(){ return String((0, eval)("delete neverEverDeclared")); });
p(function(){ return (function(){ var local = 1; return String(delete local); })(); });

// --- the order a computed member reference performs its two steps in. The base is checked before
// the key is converted, so a nullish base is a TypeError about the base and the key's own `toString`
// never runs - which a program can tell apart, because that `toString` can throw something else.
p(function(){ try { var b = null; var k = { toString: function(){ throw new RangeError("key"); } }; return String(b[k]); } catch(e){ return e.name; } });
p(function(){ try { var b = undefined; var k = { toString: function(){ throw new RangeError("key"); } }; b[k] = 1; return "no-throw"; } catch(e){ return e.name; } });
p(function(){ var seen = 0; try { var b = null; var k = { toString: function(){ seen = 1; return "x"; } }; return String(b[k]); } catch(e){ return "threw:" + seen; } });

// --- WHAT AN ARRAY PATTERN OWES THE ITERATOR IT OPENED. A pattern abandoned part-way closes the
// iterator it was reading, and the completion that abandoned it decides how: a throw completion
// discards whatever `return` does, and every other one lets an error from `return` through and
// makes a non-object answer a TypeError of its own. An iterator that already said it was done, and
// one a rest element ran out, are owed nothing. Recorded with JSC-180, which closed the divergence
// this lowering used to declare.
function __counting(onReturn) {
  var s = { next: 0, ret: 0, self: null, args: -1 };
  var iterator = {
    next: function () { s.next += 1; return { done: s.next > 10 }; },
    return: function () {
      s.ret += 1; s.self = this; s.args = arguments.length;
      return onReturn ? onReturn() : {};
    }
  };
  s.iterable = {};
  s.iterable[Symbol.iterator] = function () { return iterator; };
  s.iterator = iterator;
  return s;
}
function __boom() { throw new TypeError("boom"); }

// An element's target reference is evaluated BEFORE the iterator is stepped, so a reference that
// throws never calls `next` - and the iterator it never stepped is still closed.
p(function(){ var s = __counting(); try { 0, [ {}[__boom()] ] = s.iterable; } catch(e){} return s.next + "," + s.ret; });
p(function(){ var s = __counting(); try { 0, [ {}[__boom()] , ] = s.iterable; } catch(e){} return s.next + "," + s.ret; });
p(function(){ var s = __counting(); try { 0, [ ...{}[__boom()] ] = s.iterable; } catch(e){} return s.next + "," + s.ret; });
p(function(){ var s = __counting(); var a; try { 0, [ a, ...{}[__boom()] ] = s.iterable; } catch(e){} return s.next + "," + s.ret; });
// `return` is called with no arguments, on the iterator itself.
p(function(){ var s = __counting(); try { 0, [ {}[__boom()] ] = s.iterable; } catch(e){} return (s.self === s.iterator) + "," + s.args; });
// A throw completion swallows what `return` raises: the exception already travelling is the answer.
p(function(){ var s = __counting(function(){ throw new RangeError("swallowed"); }); try { 0, [ {}[__boom()] ] = s.iterable; return "no-throw"; } catch(e){ return e.name + "," + s.ret; } });
// A binding pattern owes the same close, and a default that throws is how it gets abandoned.
p(function(){ var s = __counting(); try { var [ q = __boom() ] = s.iterable; } catch(e){} return s.next + "," + s.ret; });
// A nested pattern over a value that is not iterable abandons the outer one.
p(function(){ var s = __counting(); try { var [ [ r ] ] = s.iterable; } catch(e){} return s.next + "," + s.ret; });
// An iterator that already reported done is not asked for its `return`.
p(function(){ var c = 0; var it = {}; it[Symbol.iterator] = function(){ return { next: function(){ return { done: true }; }, return: function(){ c += 1; return {}; } }; }; try { var [ u = __boom() ] = it; } catch(e){} return c; });
// A rest element runs the iterator out, so the pattern that contains it closes nothing.
p(function(){ var c = 0; var it = {}; it[Symbol.iterator] = function(){ var i = 0; return { next: function(){ i += 1; return i > 2 ? { done: true } : { done: false, value: i }; }, return: function(){ c += 1; return {}; } }; }; var rest; 0, [ ...rest ] = it; return rest.join("") + "," + c; });
// A NORMAL completion over an iterator that is not done closes it loudly: an error from `return`
// propagates, and a `return` answering a non-object is a TypeError.
p(function(){ var s = __counting(function(){ throw new RangeError("loud"); }); var b; try { 0, [ b ] = s.iterable; return "no-throw"; } catch(e){ return e.name + "," + s.ret; } });
p(function(){ var s = __counting(function(){ return null; }); var c; try { 0, [ c ] = s.iterable; return "no-throw"; } catch(e){ return e.name + "," + s.ret; } });
// A FORCED RETURN through a pattern - `gen.return()` arriving at a `yield` inside one - closes it
// the same loud way, and the pattern's own statement never completes.
p(function(){ var s = __counting(); var hit = 0; function* g(){ var d; var o = [ d, ...{}[yield] ] = s.iterable; hit += 1; return o; } var i = g(); i.next(); var r = i.return(9); return r.value + "," + r.done + "," + hit + "," + s.next + "," + s.ret; });
p(function(){ var s = __counting(function(){ throw new RangeError("on return"); }); function* g(){ var e; return [ e, ...{}[yield] ] = s.iterable; } var i = g(); i.next(); try { i.return(1); return "no-throw"; } catch(e){ return e.name + "," + s.ret; } });
p(function(){ var s = __counting(function(){ return null; }); function* g(){ var f; return [ f, ...{}[yield] ] = s.iterable; } var i = g(); i.next(); try { i.return(1); return "no-throw"; } catch(e){ return e.name + "," + s.ret; } });
// A computed reference in a pattern evaluates its key ONCE, and stores what the iterator gave.
p(function(){ var n = 0; var sink = {}; function k(){ n += 1; return "k"; } 0, [ sink[k()] ] = [ 42 ]; return sink.k + "," + n; });
// Two live records, one nesting inside the other, each closed by its own pattern.
p(function(){ var inner = 0, outer = 0; var iv = {}; iv[Symbol.iterator] = function(){ return { next: function(){ return { done: false, value: 1 }; }, return: function(){ inner += 1; return {}; } }; }; var ov = {}; ov[Symbol.iterator] = function(){ return { next: function(){ return { done: false, value: iv }; }, return: function(){ outer += 1; return {}; } }; }; var [ [ z ] ] = ov; return z + "," + inner + "," + outer; });
// --- the early errors about DECLARED NAMES, which are what one declarative scope may hold. Every
// case is asked through an indirect eval, because a probe file that wrote one of them directly
// would not compile at all. `ok` means the source was accepted; anything else is the error's name.
p(function(){ try { (0, eval)("{ let dn1; let dn1 }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("{ let dn2; var dn2 }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("{ { var dn3; } let dn3; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("{ var dn4; var dn4 }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("{ function dn5() {} function dn5() {} }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("'use strict'; { function dn6() {} function dn6() {} }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("{ function* dn7() {} function* dn7() {} }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("{ function dn8() {} var dn8 }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("function dn9() { { let a; var a; } }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("{ function dnA() {} } var dnA;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("let dnB; let dnB;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("let dnC; var dnC;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("function dnD(){} let dnD;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var dnE; function dnE(){}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("function dnF(a) { let a; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("function dnG(a) { { let a; } }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("(a) => { let a; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("try {} catch (e) { let e; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("try {} catch (e) { var e; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("try {} catch ([e]) { var e; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("try {} catch (e) { function e(){} }"); return "ok"; } catch(e){ return e.name; } });

// --- a `switch` shares ONE block scope across every one of its clauses, which is what makes these
// cases different from the block cases above, and it has at most one `default`.
p(function(){ try { (0, eval)("switch (0) { case 1: let swA; break; case 2: let swA; break; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("switch (0) { case 1: let swB; break; case 2: var swB; break; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("switch (0) { case 1: let swC; break; case 2: swC = 1; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("switch (0) { default: break; default: break; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("switch (0) { case 1: function swD(){} default: function swD(){} }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("'use strict'; switch (0) { case 1: function swE(){} default: function swE(){} }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("switch (0) { case 1: function* swF(){} default: function* swF(){} }"); return "ok"; } catch(e){ return e.name; } });

// --- a loop head and the body it encloses are two scopes, and the one rule that spans them.
p(function(){ try { (0, eval)("for (let lpA, lpA;false;) ;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("for (let lpB; false;) { var lpB; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("for (let lpC of []) { let lpC; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("for (let [lpD, lpD] of []) ;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("for (var lpE of []) { var lpE; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("let let;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var let;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("'use strict'; var let;"); return "ok"; } catch(e){ return e.name; } });

// --- a STATEMENT position, where a declaration may not stand. The one exception is Annex B's, and
// it covers a plain function declaration in an `if` clause or under a label, in sloppy code only.
p(function(){ try { (0, eval)("if (1) let stA;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("if (1) function stB(){}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("'use strict'; if (1) function stC(){}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("if (1) function* stD(){}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("if (1) class StE {}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("while (0) function stF(){}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("stG: function stH(){}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("stI: let stJ;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("if (false) let\nstK = 1;"); return "ok"; } catch(e){ return e.name; } });

// --- a formal parameter list: when one name may appear in it twice, and the directive a list that
// is not simple may not be given.
p(function(){ try { (0, eval)("function paA(a, a) {}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("'use strict'; function paB(a, a) {}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("function paC(a, a = 1) {}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("({ m(a, a) {} })"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("(a, a) => 1"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("(function (a = 1) { 'use strict' })"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("({ get gA(x) {} })"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("({ set sA() {} })"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("({ set sB(...x) {} })"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("({ set sC([x]) {} })"); return "ok"; } catch(e){ return e.name; } });

// --- a unicode escape spells a name and never reaches a reserved word through one. The pairs are
// what make the rule visible: one word is a name where nothing reserves it and refused where
// something does, and a PROPERTY key is an IdentifierName and may spell anything at all.
p(function(){ try { (0, eval)("var \\u0061wait;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("async function esA() { var \\u0061wait; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var bre\\u0061k;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { return String((0, eval)("var esB = { \\u0069f: 1 }; esB.\\u0069f;")); } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("class l\\u0065t {}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var \\u0061sync = 1;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("function* esC() { var yi\\u0065ld; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var yi\\u0065ld;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("'use strict'; var yi\\u0065ld;"); return "ok"; } catch(e){ return e.name; } });

// --- an enumerating head declares a binding and does not initialise it, and the one exception is
// Annex B's: a `var` with a plain name, an `in` head, and sloppy code.
p(function(){ try { (0, eval)("for (let fhA = 3 in {}) ;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("for (const fhB = 3 in {}) ;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("for (var fhC = 3 in {}) ;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("'use strict'; for (var fhD = 3 in {}) ;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("for (var fhE = 3 of []) ;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("for (var [fhF] = [] in {}) ;"); return "ok"; } catch(e){ return e.name; } });

// --- the two names a class definition owns, against the members that may and may not spell them.
p(function(){ try { (0, eval)("class ClA { static constructor; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("class ClB { static 'constructor'; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("class ClC { static constructor() {} }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("class ClD { static prototype; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("class ClE { 'constructor'() {} }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("class ClF { static ['constructor']; }"); return "ok"; } catch(e){ return e.name; } });

// --- an object literal's SHORTHAND is a reference and its key is a name, so the escape rule reaches
// it too; a `let` written that way is a name in sloppy code and the same escape refuses `break`.
p(function(){ try { (0, eval)("var shA = { bre\\u0061k } = { break: 42 };"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var shB = { l\\u0065t } = { let: 42 };"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var shC = { br\\u0065ak = 1 } = {};"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var shD = { async } = { async: 1 };"); return "ok"; } catch(e){ return e.name; } });


// --- an object assignment pattern evaluates its targets before it reads what feeds them
//
// The half of the family JSC-152 left standing, closed by JSC-221. An ARRAY pattern's targets were
// already prepared ahead of the step; an OBJECT pattern's were not, so a property with a getter on
// it ran before the reference the getter's value was about to be stored through.
p(function(){ var o = {}, s = ""; function k(){ s += "k"; return "z"; } var src = { get a(){ s += "g"; return 1; } }; 0, ({ a: o[k()] } = src); return s + ":" + o.z; });
p(function(){ var o = {}, s = ""; function ka(){ s += "A"; return "p"; } function kb(){ s += "B"; return "q"; } var src = { get a(){ s += "a"; return 1; }, get b(){ s += "b"; return 2; } }; 0, ({ a: o[ka()], b: o[kb()] } = src); return s; });
// A rest target's reference is evaluated before the rest object exists to copy into.
p(function(){ var o = {}, s = ""; function k(){ s += "k"; return "z"; } var src = { get a(){ s += "g"; return 1; } }; 0, ({ ...o[k()] } = src); return s; });
// A target whose base throws means the property is never read at all.
p(function(){ var s = ""; var src = { get a(){ s += "g"; return 1; } }; try { 0, ({ a: (function(){ s += "b"; throw new TypeError("no base"); })().x } = src); } catch(e){ s += ":" + e.name; } return s; });
// The pattern's own computed key runs before the target reference, and both before the read.
p(function(){ var o = {}, s = ""; function pk(){ s += "P"; return "a"; } function tk(){ s += "T"; return "z"; } var src = { get a(){ s += "g"; return 1; } }; 0, ({ [pk()]: o[tk()] } = src); return s + ":" + o.z; });
// A computed key is still evaluated exactly once, with a target that prepares and one that does not.
p(function(){ var o = {}, n = 0; function pk(){ n += 1; return "a"; } 0, ({ [pk()]: o.z } = { a: 7 }); return o.z + "," + n; });
p(function(){ var o = {}, n = 0; function pk(){ n += 1; return "a"; } var v; 0, ({ [pk()]: v } = { a: 7 }); return v + "," + n; });
// A rest beside a computed key excludes the key it read, and reads that key's expression once.
p(function(){ var o = {}, n = 0; function pk(){ n += 1; return "a"; } 0, ({ [pk()]: o.y, ...o.z } = { a: 1, b: 2 }); return o.y + "," + JSON.stringify(o.z) + "," + n; });
// A nested pattern is not a reference and evaluates nothing ahead of the read.
p(function(){ var o = {}, s = ""; var src = { get a(){ s += "g"; return { b: 1 }; } }; 0, ({ a: { b: o.z } } = src); return s + ":" + o.z; });
// A declaration pattern has no reference to evaluate, so its order is unchanged by any of this.
p(function(){ var s = ""; var src = { get a(){ s += "g"; return 1; } }; var { a: v } = src; return s + ":" + v; });
// The value of the assignment is the source, whichever shape the target took.
p(function(){ var o = {}; var src = { a: 1 }; return (({ a: o.z } = src) === src) + "," + o.z; });

// --- a function declaration inside a block is a binding of that block
//
// JSC-240. The declaration was hoisted to the enclosing body, so its closure was built against a
// chain the block's own record was not on: a body naming a `let` of the same block reached a
// global. Annex B's `var`-scoped alias is the other half, written where the declaration stands.
p(function(){ var r; { let n = 0; function fn(){ n += 1; } fn(); r = n; } return r; });
p(function(){ { let n = 5; function fn(){ return n; } return fn(); } });
p(function(){ var s = ""; { function g(){ return 1; } s += g(); } { function g(){ return 2; } s += g(); } return s; });
p(function(){ var s = ""; { function* g(){ yield 1; } s += [...g()]; } { function* g(){ yield 2; } s += [...g()]; } return s; });
p(function(){ var r; switch (1) { case 1: { let z = 7; function k(){ return z; } r = k(); } } return r; });
p(function(){ try { let w = 9; function m(){ return w; } return m(); } catch (e) { return e.name; } });
p(function(){ var r; { let q = 3; l: function h(){ return q; } r = h(); } return r; });
p(function(){ var s = ""; { function a(){ return b(); } function b(){ return "ab"; } s = a(); } return s; });
// The alias exists before the declaration is reached and holds `undefined` until then.
p(function(){ var i = typeof f; { function f(){} } return i + "," + typeof f; });
p(function(){ if (false) { function q(){} } return typeof q; });
p(function(){ if (true) function i(){ return "i"; } return i(); });
p(function(){ var s = ""; if (true) function j(){ return 1; } else function j(){ return 2; } return "" + j(); });
p(function(){ switch (1) { case 1: function s(){ return "s"; } } return s(); });
// The alias is declined where a `var` of the name would have been an early error.
p(function(){ { let f = 1; { function f(){} } } return typeof f; });
p(function(){ for (let f; ; ) { { function f(){} } break; } return typeof f; });
p(function(){ switch (0) { default: let f; { function f(){} } } return typeof f; });
p(function(){ try { throw {}; } catch ({ d }) { { function d(){} } } return typeof d; });
// A simple catch parameter does not decline it.
p(function(){ try { throw null; } catch (c) { { function c(){ return 5; } } } return typeof c; });
// A parameter of the same name declines it.
p(function(p1){ { function p1(){} } return typeof p1; });
// A generator, an async function and an async generator get no alias at all.
p(function(){ switch (0) { default: async function a(){} } return typeof a; });
p(function(){ { function* b(){} } return typeof b; });
p(function(){ { async function* c(){} } return typeof c; });
// The block's own binding is the one the block sees, whatever the alias holds.
p(function(){ function f(){ return "outer"; } var s = ""; { function f(){ return "inner"; } s += f(); } return s + "," + f(); });

// --- a statement's completion value
//
// JSC-242. `UpdateEmpty(C, undefined)` is on `if`, on every iteration statement, on `with`, on
// `try` and on `switch`, and a `finally` that completes normally contributes none of its own.
p(function(){ return (0, eval)('1; if (true) { }'); });
p(function(){ return (0, eval)('2; if (true) { 3; }'); });
p(function(){ return (0, eval)('1; if (false) { }'); });
p(function(){ return (0, eval)('1; while (false) { }'); });
p(function(){ return (0, eval)('2; while (false) { 3; }'); });
p(function(){ return (0, eval)('1; do { break; } while (false)'); });
p(function(){ return (0, eval)('2; do { 3; break; } while (false)'); });
p(function(){ return (0, eval)('1; try { } finally { }'); });
p(function(){ return (0, eval)('2; try { 3; } finally { }'); });
p(function(){ return (0, eval)('4; try { } catch (err) { } finally { 5; }'); });
p(function(){ return (0, eval)('1; try { throw null; } catch (err) { } finally { }'); });
p(function(){ return (0, eval)('1; switch ("a") { default: case "a": }'); });
p(function(){ return (0, eval)('1; switch ("a") { case null: }'); });
p(function(){ return (0, eval)('1; { }'); });
p(function(){ return (0, eval)('1; l: { }'); });
p(function(){ return (0, eval)('1; for (var i0 = 0; i0 < 0; ++i0) { 2; }'); });
p(function(){ return (0, eval)('1; for (var i1 = 0; i1 < 1; ++i1) { 2; }'); });
p(function(){ return (0, eval)('1; for (var k0 in { }) { 2; }'); });
p(function(){ return (0, eval)('1; for (var v0 of [ ]) { 2; }'); });
p(function(){ return (0, eval)('1; with ({ }) { }'); });
p(function(){ return (0, eval)('1; do { 2; with({}) { 3; break; } 4; } while (false);'); });
p(function(){ return (0, eval)("for (var i2 = 0; i2 < 2; ++i2) { if (i2) { try { throw null; } catch (e) { break; } } 'bad completion'; }"); });

// --- the early errors an assignment PATTERN carries
//
// JSC-241. `ToPattern` is the cover grammar's reinterpretation and its rules are not the
// declaration path's: a rest element must be last with no comma after it, a shorthand's key is a
// reference and not a name, and strict code assigns to neither restricted name through a pattern.
p(function(){ try { (0, eval)("0, [...pa,] = [];"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("0, ({...pb,} = {});"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var pc = [...pd,];"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("0, ({ default } = {});"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("0, ({ extends } = {});"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("0, ({ default: pe } = {});"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("0, ({ if });"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("'use strict'; 0, [arguments] = [];"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("'use strict'; 0, ({ eval } = {});"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("'use strict'; 0, ({ a: arguments } = {});"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("'use strict'; 0, [...eval] = [];"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("0, [arguments] = [];"); return "ok"; } catch(e){ return e.name; } });
// A nested pattern with a default is an ordinary target and was refused as if it were not.
p(function(){ var a = [], r = ([ [a[0]] = [7] ] = [ ]); return a[0] + "," + (r.length === 0); });
p(function(){ var o = {}; 0, ([ {} = 1 ] = [ ]); 0, ({ p: {} = 1 } = {}); return "ok"; });
p(function(){ var b; 0, [ {a: b} = {a: 4} ] = [ ]; return b; });

// --- a suspension may not appear in a parameter list
//
// JSC-243. The default belongs to the enclosing function's own prologue, so a `yield` or an
// `await` written there lowers into a unit that carries neither flag.
p(function(){ try { (0, eval)("function* ga(x = yield) {}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var gb = { *g(x = yield) {} };"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("class GC { *g(x = yield) {} }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("async function gd(x = await 1) {}"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var ge = { async m(x = await 1) {} };"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("async function gf() { (a = await 1) => {}; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("async function gg() { (await) => {}; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("async function gh() { ([await]) => {}; }"); return "ok"; } catch(e){ return e.name; } });
// A method's parameters are `[~Yield, ~Await]` however deeply a generator or an async function
// encloses them, so the same words there are ordinary names.
p(function(){ try { (0, eval)("function* gi() { ({ m(x = yield) {} }); }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("async function gj() { ({ m(await) {} }); }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("function gk() { (await) => {}; }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("async function gl() { const q = async () => await 1; }"); return "ok"; } catch(e){ return e.name; } });

// --- a private name is not a property key, and a generator method is a method
//
// JSC-244. An object literal's key is read in one place, so the four method forms and the plain
// entry are one refusal; and the generator form alone did not record itself as a method, so its
// body had no home object and `super` in it was refused.
p(function(){ try { (0, eval)("var ha = { #m() {} };"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var hb = { get #m() {} };"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var hc = { *#m() {} };"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var hd = { async #m() {} };"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("var he = { #m: 1 };"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("class HF { field = { #m() {} } }"); return "ok"; } catch(e){ return e.name; } });
p(function(){ try { (0, eval)("class HG { #m() { return 1; } n() { return this.#m(); } } 0;"); return "ok"; } catch(e){ return e.name; } });
p(function(){ var base = { x: "base" }; var o = { *g() { return super.x; } }; Object.setPrototypeOf(o, base); return o.g().next().value; });
p(function(){ var base = { x: "base" }; var o = { g() { return super.x; } }; Object.setPrototypeOf(o, base); return o.g(); });
