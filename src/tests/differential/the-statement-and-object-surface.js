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
