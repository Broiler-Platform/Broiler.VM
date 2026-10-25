// B2b: /(?:(a+)+b|a*c)/u on a^24 c (the survey's wrong index); the specification's answer is
// index 0, length 25. One case per file: Broiler.VM may exhaust its allowance here.
var s = 'a'.repeat(24) + 'c';
t('/(?:(a+)+b|a*c)/u on a^24c', function () { var e = elapsed(function () { return /(?:(a+)+b|a*c)/u.exec(s); }); return R((e.r === null ? 'null' : '@' + e.r.index + ' length ' + e.r[0].length) + ' elapsedMs ' + e.ms); });
