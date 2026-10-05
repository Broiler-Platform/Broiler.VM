// Inside a `with` body a name is resolved once, before the right-hand side, and written back through
// that reference: a compound assignment or an update writes the object the read found even when its
// getter deleted the property, and a plain assignment writes the binding the name meant before the
// right-hand side ran. Until 2026-10-03 the name was resolved again at the write. The values are the
// specification's and test262's (S11.13.1_A5/A6, S11.13.2_A5/A6); the comparison engine resolves
// again, as this host did (JSC-243).
var r = [];
function deleted() { var x = 0; var scope = { get x() { delete this.x; return 2; } }; with (scope) { x *= 3; } return scope.x + ":" + x; }
r.push(deleted());
var outer = { x: 0 }, inner = { get x() { delete this.x; return 2; } };
with (outer) { with (inner) { x *= 3; } }
r.push(inner.x + ":" + outer.x);
var o2 = { get y() { delete this.y; return 1; } }; var y = 10;
with (o2) { r.push(y++); }
r.push(o2.y + ":" + y);
var q = 0; var holder = {};
with (holder) { q = (holder.q = 5, 7); }
r.push(holder.q + ":" + q);
var s = 0; var obj = { s: 1 };
with (obj) { s = (delete obj.s, 9); }
r.push(obj.s + ":" + s);
var plain = 1; with ({}) { plain += 2; plain++; }
r.push(plain);
r.join(" / ");
