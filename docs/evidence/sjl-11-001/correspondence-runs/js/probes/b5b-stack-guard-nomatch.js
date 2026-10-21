// B5b: the same shape over a v-mode class the .NET translator cannot express ([\s--\d]), so
// Broiler.JS holds no .NET engine to fall back to. The specification's answer is the whole input.
var s = ' a'.repeat(100000);
t('/(?:[\\s--\\d]a)*/v exec on (" a")^100000', function () { var r = C('(?:[\\s--\\d]a)*', 'v').exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
t('/^(?:[\\s--\\d]a)*$/v test on (" a")^100000', function () { return C('^(?:[\\s--\\d]a)*$', 'v').test(s); });
