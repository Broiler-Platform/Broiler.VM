// `for (let k in o)` and `for (const k in o)` bind a fresh `k` each turn, which is what a closure
// in the body captures, and the head's names are in their dead zone while the object is evaluated.
// Until 2026-10-03 every closure saw the last key, and `for (let x in { x })` read the outer `x`
// (JSC-246).
var r = [];
var a = []; for (let k in { p: 1, q: 1 }) a.push(() => k); r.push(a.map(f => f()).join());
var b = []; for (const k in { p: 1, q: 1 }) { b.push(() => k); } r.push(b.map(f => f()).join());
var c = []; for (let [k] in { p: 1, q: 1 }) c.push(() => k); r.push(c.map(f => f()).join());
function inner() { var d = []; for (let k in { p: 1, q: 1 }) d.push(() => k); return d.map(f => f()).join(); }
r.push(inner());
function dead(f) { try { f(); return "read"; } catch (e) { return e.name; } }
r.push(dead(() => { let x = 1; for (let x in { x }) {} }));
r.push(dead(() => { let y = {}; for (const y in y) {} }));
var outside = "outside", probe;
for (let outside in { i: probe = function () { return typeof outside; } }) ;
r.push(dead(probe), outside);
r.join(" / ")
