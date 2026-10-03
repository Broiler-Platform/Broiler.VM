// In a `with` body, as in a loop's, `let` before a line break is the identifier and the line
// break ends its statement. Until 2026-10-03 the `with` body refused it as a declaration
// (JSC-250).
var r = [], let = 5, x = 0;
with ({}) let
x = 1;
r.push(x);
with ({ y: 2 }) let
{ r.push(typeof y); }
r.push(let);
r.join(" ")
