// B6c: control for B6a: the same shape at 100,000 iterations, under Broiler.VM's frame ceiling.
var s = 'ab'.repeat(100000);
t('/(?:ab)*/ exec on (ab)^100000', function () { var r = /(?:ab)*/.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
