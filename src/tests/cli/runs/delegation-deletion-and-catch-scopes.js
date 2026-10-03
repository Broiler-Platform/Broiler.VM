// Four semantics corrected on 2026-10-03 (JSC-254): a synchronous `yield*` hands the inner result
// object through without reading its `value`; `delete super.x` and `delete super[k]` throw a
// ReferenceError after evaluating the key and before converting it; a catch block's `let` is
// hoisted in a scope of its own, apart from a pattern parameter's defaults; a `var` initialiser in
// a `with` body writes the reference resolved before it ran. The last row's values are the
// specification's and test262's; the comparison engine differs on it.
var r = [];
function t(f) { try { return String(f()); } catch (e) { return e.name; } }
var reads = 0;
var inner = { [Symbol.iterator]() { return this; }, i: 0, next() { var res = { done: this.i++ >= 1 }; Object.defineProperty(res, "value", { get() { reads++; return 5; } }); return res; } };
function* g() { return yield* inner; }
var it = g(), first = it.next(), last = it.next();
r.push(first.done, reads, last.value, reads);
class A { m() { return 1; } }
class B extends A { d() { return delete super.m; } e(k) { return delete super[k()]; } }
var called = 0;
r.push(t(() => new B().d()), t(() => new B().e(() => { called++; return { toString() { throw new Error("converted"); } }; })), called);
var probeParam, probeBlock; let x = "outside";
try { throw []; } catch ([_ = probeParam = function () { return x; }]) { probeBlock = function () { return x; }; let x = "inside"; }
r.push(probeParam(), probeBlock());
var early; try { throw 0; } catch (e) { early = () => typeof later; let later = 3; }
r.push(early());
var obj = { id: 1 };
with (obj) { var id = delete obj.id; }
r.push(obj.id, typeof id);
r.join(" / ")
