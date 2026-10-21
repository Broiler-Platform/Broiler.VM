// B1e: /^a*$/ on a^10000001 without u; the specification's answer is true.
var s = 'a'.repeat(10000001);
t('/^a*$/ test on a^10000001', function () { return /^a*$/.test(s); });
