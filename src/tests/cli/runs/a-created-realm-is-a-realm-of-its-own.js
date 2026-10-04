// `$262.createRealm()` builds an ordinary new realm on the same engine and answers its `$262`
// (JSD-0030 SR-7, JSC-264): a global object and intrinsics of its own, the agent's Symbols, and
// functions that run in the realm that made them. Until 2026-10-04 it refused.
var other = $262.createRealm();
var g = other.global;
var rows = [];

rows.push("own global " + (g !== globalThis) + ", own Array " + (g.Array !== Array));
rows.push("shared Symbol.iterator " + (g.Symbol.iterator === Symbol.iterator) +
  ", shared registry " + (g.Symbol.for("k") === Symbol.for("k")));

var thrown;
try { g.eval("null.x"); } catch (e) { thrown = e; }
rows.push("its TypeError " + (thrown instanceof g.TypeError) + ", not ours " + !(thrown instanceof TypeError));

var C = new g.Function();
C.prototype = null;
rows.push("an intrinsic of the new target's realm " +
  (Object.getPrototypeOf(Reflect.construct(Array, [], C)) === g.Array.prototype));

var f = g.eval("(function () { return this; })");
rows.push("its global for a sloppy this " + (f() === g));

var A = g.eval("[1, 2]");
rows.push("another realm's Array is no species " +
  (Object.getPrototypeOf(Array.prototype.map.call(A, function (x) { return x; })) === Array.prototype));

rows.push("a realm from a realm " + (other.createRealm().global !== g));
rows.join("\n");
