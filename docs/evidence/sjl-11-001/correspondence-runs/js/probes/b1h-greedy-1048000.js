// B1h: /a*/ on a^1048000, just under Broiler.VM's frame ceiling of 2^20 (1,048,576). One case per file.
var s = 'a'.repeat(1048000);
t('/a*/ exec on a^1048000: index, length', function () { var r = /a*/.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
