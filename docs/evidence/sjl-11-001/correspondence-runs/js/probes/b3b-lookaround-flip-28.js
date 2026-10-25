// vm-fuel: 2000000000
// B3b: the critic's look-around flip, /^(?!(?:(a+)+b|a*c))/u on a^28 c; the specification's answer
// is null. Broiler.VM runs it under a fuel allowance of 2,000,000,000 (stated in the command).
var s = 'a'.repeat(28) + 'c';
t('/^(?!(?:(a+)+b|a*c))/u on a^28c', function () { var e = elapsed(function () { return /^(?!(?:(a+)+b|a*c))/u.exec(s); }); return R((e.r === null ? 'null' : '@' + e.r.index + ' length ' + e.r[0].length) + ' elapsedMs ' + e.ms); });
