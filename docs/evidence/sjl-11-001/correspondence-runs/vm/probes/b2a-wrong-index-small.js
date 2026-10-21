// B2a: exhaustion inside one alternative (control size): /(?:(a+)+b|a*c)/ on a^16 c; the
// specification's answer is the second alternative at index 0, length 17.
var s = 'a'.repeat(16) + 'c';
t('/(?:(a+)+b|a*c)/u on a^16c', function () { var r = /(?:(a+)+b|a*c)/u.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
t('/(?:(a+)+b|a*c)/ on a^16c', function () { var r = /(?:(a+)+b|a*c)/.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
