// B1c: the same run without u (Broiler.JS's .NET translation). One case per file.
var s = 'a'.repeat(10000001);
t('/a*/ exec on a^10000001: index, length', function () { var r = /a*/.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
