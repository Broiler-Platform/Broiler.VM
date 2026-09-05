// TWO PROPERTIES AT ONCE, and the second was a defect.
//
// The first is per-iteration binding: each turn of a `for … of` with a lexical head gets its own
// environment, so a closure the body made keeps the value that turn had rather than the one the
// loop finished with.
//
// The second is that a nested function is a CONTROL-FLOW BOUNDARY. The compiler carried one stack
// of loops and finalisers across a nested unit, so this `return` was compiled as a return out of
// the ENCLOSING `for … of` - emitting a read of that loop's iterator slot against an environment
// with no such slot, which the executor answers by aborting the invocation as an internal defect.
// Reverting that repair puts this file back to exit 7.
var makers = [];

for (const value of [1, 2, 3]) {
  makers.push(function () { return value; });
}

makers.map(function (make) { return make(); }).join(",");
