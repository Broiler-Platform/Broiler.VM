// B1d: /^a*$/u on a^10000001 (the survey's "budget no-match"); the specification's answer is true.
var s = 'a'.repeat(10000001);
t('/^a*$/u test on a^10000001', function () { return /^a*$/u.test(s); });
