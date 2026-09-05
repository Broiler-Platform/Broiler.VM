// THE OTHER DIRECTION OF THE THREE RULES THE `refused/` ROWS BESIDE THIS ONE STATE, because an
// early error that fires where the language has none turns working programs into refusals, which is
// worse than the failure it was written to close.
//
// A trailing comma is legal after a rest element in a LITERAL and illegal in a pattern; a shorthand
// key that is an ordinary name is legal wherever it is written; and `eval` and `arguments` are
// ordinary destructuring targets in sloppy code, which this file is.
var out = [];

// A rest element with nothing after it, in both shapes.
var a, b, rest;
0, [a, ...rest] = [1, 2, 3];
out.push(a + ":" + rest.join("/"));

var o = {};
0, ({ p: b, ...o } = { p: 4, q: 5, r: 6 });
out.push(b + ":" + JSON.stringify(o));

// A trailing comma is what an array literal and an object literal may end with.
var literal = [7, 8,];
var entries = { s: 9, };
out.push(literal.length + ":" + entries.s);

// A rest element in a LITERAL may be followed by a comma, which is the case the pattern rule must
// not have reached.
var spread = [...literal,];
out.push(spread.join("/"));

// A shorthand entry, and a reserved word as a written-out key beside it.
var t = 10;
var shorthand = { t, default: 11, if: 12 };
out.push(shorthand.t + ":" + shorthand.default + ":" + shorthand.if);

// A shorthand entry as a pattern's target, with and without a default.
var u, v;
0, ({ u } = { u: 13 });
0, ({ v = 14 } = {});
out.push(u + ":" + v);

// And the two restricted names, which sloppy code assigns to through a pattern like any other.
var holder = {};
0, [holder.eval] = [15];
0, ({ arguments: holder.arguments } = { arguments: 16 });
out.push(holder.eval + ":" + holder.arguments);

// A nested pattern carrying a default is an ordinary target and was refused as if it were not.
var nested = [];
0, [ [nested[0]] = [17] ] = [];
0, [ {n: nested[1]} = {n: 18} ] = [];
out.push(nested.join("/"));

out.join(" ");
