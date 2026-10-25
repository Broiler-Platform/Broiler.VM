// B6b: three capture writes per iteration over 400,000 iterations (Broiler.VM's trail ceiling is
// 2^21 writes). The specification's answer is the whole input, the groups the last iteration's.
var s = 'abc'.repeat(400000);
t('/(?:(a)(b)(c))*/ exec on (abc)^400000', function () { var r = /(?:(a)(b)(c))*/.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length + ' ' + show([r[1], r[2], r[3]])); });
