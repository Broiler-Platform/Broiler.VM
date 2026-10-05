// What the parser reads by context, as the edition does (JSC-253): `yield`, `await` and `let` as an
// arrow's one parameter where each is a name; `yield / 2` dividing outside a generator and
// `yield /re/` a literal inside one; a numeric property key spelled as Number::toString spells
// it; `?.` before a digit as a conditional; a chain after `new C(1)`; `08` in sloppy code; a
// private name in a class heritage resolving outward.
var r = [];
var af = yield => yield + 1;
var lf = let => let * 2;
r.push(af(1), lf(2));
var yield = 8, g = 2;
r.push(yield / g / 2);
function* gen() { yield /re/g; }
r.push(String(gen().next().value));
var o = { 0.0000001: "a", 1e21: "b", 123e-20: "c" };
r.push(Object.keys(o).join("+"));
class C { get 0.0000001() { return "g"; } }
r.push(C.prototype["1e-7"]);
r.push(true ?.30 : false);
class D { constructor(v) { this.a = v; } }
r.push(new D(99)?.a);
r.push(08 + 1);
class Outer { static #p = 7; static make() { return class extends (Outer.#p, Object) { }; } }
r.push(typeof Outer.make());
r.join(" / ")
