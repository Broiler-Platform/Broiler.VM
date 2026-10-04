// Five repairs of one batch. A Symbol wrapper is a property key that converts to its Symbol, in
// `in`, a read and a write. A strict `delete` that fails converts its key once. A `super` property
// is a destructuring target. `new.target` in a default parameter reads the call's, at the top level
// of a script too. And a Set method walks the receiver live: a member deleted and added again by the
// argument's `has` is visited again.
var wrapped = Object(Symbol("w"));
var keyed = {};
keyed[wrapped] = "set";
var symbolKeys = [wrapped in keyed, keyed[wrapped], keyed[wrapped.valueOf()]].join(" ");

var conversions = 0;
var key = { toString() { conversions++; return "fixed"; } };
var strictDelete = (function () {
  "use strict";
  var frozen = Object.freeze({ fixed: 1 });
  try { delete frozen[key]; return "deleted"; } catch (e) { return e.name; }
})();

class Base {}
class Derived extends Base {
  assign() { [super.first, { second: super.second }] = [1, { second: 2 }]; return [this.first, this.second].join(","); }
}
var superTargets = new Derived().assign();

function called(expected, actual = new.target) { if (new.target) { this.read = actual === expected; } return actual === expected; }
var defaults = [new called(called).read, called(undefined)].join(",");

var seen = [];
var set = new Set([1, 2, 3]);
set.intersection({
  size: 100,
  has(v) { if (v === 2 && !seen.includes(v)) { set.delete(v); set.add(v); } seen.push(v); return true; },
  keys() { throw new Error("not called"); },
});

print([symbolKeys, strictDelete + " " + conversions, superTargets, defaults, seen.join(",")].join(" / "));
