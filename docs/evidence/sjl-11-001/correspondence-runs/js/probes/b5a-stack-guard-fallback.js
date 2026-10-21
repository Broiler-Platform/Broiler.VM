// B5a: a general quantifier over a long input under u: Broiler.Regex 0.1.0-preview.4's continuation
// chain grows per iteration and its stack guard throws RegexOverflowException; Broiler.JS then
// answers from its .NET translation. The specification's answer is the whole input.
var s = 'ab'.repeat(100000);
t('/(?:ab)*/u exec on (ab)^100000', function () { var r = /(?:ab)*/u.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
t('/(?:ab)*$/u test on (ab)^100000', function () { return /(?:ab)*$/u.test(s); });
