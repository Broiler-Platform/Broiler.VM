// B1g: control for B1b: the same run written as a one-member class, [a]*, under u. Broiler.VM lowers
// a greedy quantifier over a class to one Run instruction and over a character to a loop.
var s = 'a'.repeat(10000001);
t('/[a]*/u exec on a^10000001: index, length', function () { var r = /[a]*/u.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
