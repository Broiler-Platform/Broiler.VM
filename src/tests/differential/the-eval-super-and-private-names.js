// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER `super` AND PRIVATE NAMES IN DIRECTLY EVALUATED SOURCE (JSeal
// V15-finish, JSD-0026 section 14).
//
// A direct eval in a method sees the method's [[HomeObject]], so `super.x` and `super.m()` in the
// evaluated source are the method's own; in a derived constructor `super()` is the constructor's
// call. A private name the evaluated source uses and does not declare is its caller's class's, and
// one no enclosing class declares is a SyntaxError raised before anything is evaluated or declared.
// Each case prints its own number so a divergence names a case.
var __n = 0;
function t(f) { try { return String(f()); } catch (e) { return e && e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// --- super property references and super calls
class A { m() { return "A.m"; } get g() { return "A.g:" + this.tag; } static s() { return "A.s"; } }
class B extends A { m() { return eval("super.m()"); } k() { return (() => eval("super.g"))(); } static s() { return eval("super.s()"); } }
p(() => new B().m());
p(() => { var b = new B(); b.tag = 7; return b.k(); });
p(() => B.s());
p(() => ({ __proto__: { x: 1 }, m() { return eval("super.x"); } }).m());
p(() => ({ __proto__: { x: 1 }, m() { return eval("super['x']"); } }).m());
p(() => ({ __proto__: { x: 1 }, m() { eval("super.y = 5"); return this.y; } }).m());
class C extends A { constructor() { eval("super()"); this.ok = 1; } }
p(() => new C().ok);
class D extends A { constructor() { var f = () => eval("super()"); f(); this.ok = 2; } }
p(() => new D().ok);
class E extends A { f = eval("super.m()"); }
p(() => new E().f);
class F extends A { constructor() { eval("this"); } }
p(() => new F());
class G extends A { constructor() { super(); eval("super()"); } }
p(() => new G());
p(() => ({ m() { return eval("() => super.toString === Object.prototype.toString")(); } }).m());
function plain() { return eval("super.x"); }
p(() => plain());
class H extends A { m() { return eval("eval('super.m()')"); } }
p(() => new H().m());
class I extends A { x = 1; constructor() { eval("super()"); } }
p(() => new I().x);
class J extends A { m() { return eval("(function () { return super.m(); })")(); } }
p(() => new J().m());

// --- private names of the calling class
class PA {
  #x = 1; static #s = 2; #m() { return "m"; } get #g() { return "g"; } set #g(v) { this.log = v; }
  rx() { return eval("this.#x"); }
  wx() { eval("this.#x = 5"); return this.#x; }
  rm() { return eval("this.#m()"); }
  rg() { return eval("this.#g"); }
  sg() { eval("this.#g = 9"); return this.log; }
  has(o) { return eval("#x in o"); }
  static rs() { return eval("PA.#s"); }
  bad() { return eval("this.#nope"); }
  badSide() { var r = []; try { eval("r.push(1); this.#nope"); } catch (e) { r.push(e.name); } return r.join(); }
  badVar() { try { eval("var leaked = 1; this.#nope"); } catch (e) { } return typeof leaked; }
  arrow() { return (() => eval("this.#x"))(); }
  nested() { return eval("eval('this.#x')"); }
  inner() { return eval("(() => this.#x)")(); }
  own() { return eval("(class { #y = 3; get() { return this.#y; } }).prototype.get.call(new (class { #y = 4 })())"); }
  incr() { eval("this.#x++"); return this.#x; }
  brand() { return eval("this.#x").constructor === Number; }
  other() { return eval("({}).#x"); }
}
p(() => new PA().rx());
p(() => new PA().wx());
p(() => new PA().rm());
p(() => new PA().rg());
p(() => new PA().sg());
p(() => [new PA().has(new PA()), new PA().has({})].join());
p(() => PA.rs());
p(() => new PA().bad());
p(() => new PA().badSide());
p(() => new PA().badVar());
p(() => new PA().arrow());
p(() => new PA().nested());
p(() => new PA().inner());
p(() => new PA().own());
p(() => new PA().incr());
p(() => new PA().brand());
p(() => new PA().other());
class PB { #x = 7; f = eval("this.#x"); }
p(() => new PB().f);
class Outer { #o = "o"; m() { class Inner { #i = "i"; get(o) { return eval("[this.#i, o.#o].join()"); } } return new Inner().get(this); } }
p(() => new Outer().m());
p(() => (0, eval)("class Q { m() { return this.#z; } }"));
function sloppy() { return eval("this.#x"); }
p(() => sloppy());

// --- the private-name check is an early error: it asks no `with` object around the class
var __trap = new Proxy({}, { has(t, k) { if (typeof k === "string" && k.indexOf("#") >= 0) throw new RangeError("trap ran"); return false; } });
with (__trap) { var PW = class { #other; m(x) { return eval("x.#y"); } }; }
p(() => new PW().m({}));
var __log = [];
var __spy = new Proxy({}, { has(t, k) { __log.push(String(k)); return false; }, get(t, k) { __log.push(String(k)); return undefined; } });
with (__spy) { var PL = class { #other; m(x) { try { eval("x.#y"); } catch (e) { return e.name + ":" + __log.join("|"); } } }; }
p(() => { __log.length = 0; return new PL().m({}); });
with (__trap) { var PD = class { #y = 5; m() { return eval("this.#y"); } }; }
p(() => new PD().m());
