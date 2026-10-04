// A batch of evaluation-order and completion repairs. A parenthesised optional member keeps its base
// as the receiver, and `super.m?.()` is called against `this`. A computed destructuring key is
// converted where it is evaluated, before the target. A private method installed twice on one object
// throws. A `with` store asks the object whether the name still exists, in sloppy code too. And a
// `break` out of a `finally` carries the finally's own value.
var out = [];
const a = { b() { return this === a; } };
out.push([(a?.b)(), (a?.b)?.(), a.b?.()].join(","));
class Base { m() { return this; } }
class Derived extends Base { m() { return super.m?.() === this; } }
out.push(new Derived().m());

var log = [];
var key = { toString() { log.push("key"); return "p"; } };
function target() { log.push("target"); return {}; }
({ [key]: target().q } = { get p() { log.push("get"); return 1; } });
out.push(log.join(","));

class Returning { constructor(o) { return o; } }
class Twice extends Returning { #m() {} }
var shared = {};
new Twice(shared);
try { new Twice(shared); out.push("installed twice"); } catch (e) { out.push(e.name); }

var asked = [];
var env = new Proxy({ p: 0 }, {
  has(t, k) { if (typeof k === "string") { asked.push("has:" + k); } return k in t; },
  set(t, k, v) { asked.push("set:" + k); t[k] = v; return true; },
});
with (env) { p = 1; }
out.push(asked.join(","));

out.push(String(eval("99; do { -99; try { 39 } finally { 42; break; -2 } } while (false);")));
out.push(String(eval("99; do { -99; try { 39 } finally { break; -2 } } while (false);")));
print(out.join(" / "));
