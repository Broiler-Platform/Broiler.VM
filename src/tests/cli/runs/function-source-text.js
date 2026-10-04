// Function.prototype.toString answers the source text the function was defined by (phase F3,
// JSC-259): every form a program can write, its comments and whitespace kept, and the native
// rendering for what no source defined.
var r = [];
function /* a */ f /* b */ (x, y) /* c */ { return x + y; }
r.push(f.toString());
r.push((async function* g() { yield 1; }).toString());
r.push((x => x * 2).toString(), (async (a, b) => { await a; }).toString());
var o = { m() { return 1; }, get "k e y"() { return 2; }, set s(v) {}, *gen() {}, ["c" + 1](a) {} };
r.push(o.m.toString(), Object.getOwnPropertyDescriptor(o, "k e y").get.toString(),
  Object.getOwnPropertyDescriptor(o, "s").set.toString(), o.gen.toString(), o.c1.toString());
class A extends Object { constructor() { super(); } static /* s */ async m() {} #p() {} q() { return this.#p; } }
r.push(A.toString(), A.m.toString(), new A().q().toString());
class B {}
r.push(B.toString(), (class {}).toString());
r.push(new Function("a", "b", "return a + b").toString());
r.push(Function("return 1").toString());
r.push(eval("(function e() { /* inside eval */ })").toString());
r.push(`${function t() { return `nested ${1}`; }}`);
r.push(Math.max.toString(), f.bind(null).toString(), (class { m() {} }).prototype.m.call.toString());
r.join(" @@ ").split("\n").join("~")
