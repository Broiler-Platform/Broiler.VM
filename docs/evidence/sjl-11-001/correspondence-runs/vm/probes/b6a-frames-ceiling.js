// B6a: a backtrack point per iteration over 1,100,000 iterations (Broiler.VM's frame ceiling is
// 2^20). The specification's answer is the whole input.
var s = 'ab'.repeat(1100000);
t('/(?:ab)*/ exec on (ab)^1100000', function () { var r = /(?:ab)*/.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
