// B1b: a greedy run over 10,000,001 characters under u (Broiler.Regex 0.1.0-preview.4 caps its fast
// path at 10,000,000 steps per start: the survey's "shortened match"). The specification's answer
// is index 0, length 10,000,001. One case per file.
var s = 'a'.repeat(10000001);
t('/a*/u exec on a^10000001: index, length', function () { var r = /a*/u.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
