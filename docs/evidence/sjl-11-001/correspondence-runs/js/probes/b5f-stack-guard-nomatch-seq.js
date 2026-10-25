// B5f: B5e over the v-mode class the .NET translator cannot express, (?:[\s--\d]a)* x50000 then c,
// on (" a")^10 c. The specification's answer is the whole input.
var p = '(?:[\\s--\\d]a)*'.repeat(50000) + 'c';
t('(?:[\\s--\\d]a)* x50000 c /v exec on (" a")^10 c', function () { var r = new RegExp(p, 'v').exec(' a'.repeat(10) + 'c'); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
