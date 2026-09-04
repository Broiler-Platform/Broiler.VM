// An assignment pattern's leaves are REFERENCES rather than names, so a member expression is a
// legal target here where it would be a syntax error in a declaration. The parenthesis around the
// object form is what keeps the statement from beginning with a block.
var left = 1;
var right = 2;
var holder = {};
var picked;
var others;

[left, right] = [right, left];
[holder.first] = [7];
({ a: picked, ...others } = { a: 8, b: 9 });

left + " " + right + " " + holder.first + " " + picked + " " + JSON.stringify(others);
