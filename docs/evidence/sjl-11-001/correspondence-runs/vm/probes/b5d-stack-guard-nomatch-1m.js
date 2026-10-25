// B5d: B5b at 1,000,000 iterations, over the class the .NET translator cannot express. The
// specification's answer is the whole input. One case per file.
var s = ' a'.repeat(1000000);
t('/(?:[\\s--\\d]a)*/v exec on (" a")^1000000', function () { var r = C('(?:[\\s--\\d]a)*', 'v').exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
