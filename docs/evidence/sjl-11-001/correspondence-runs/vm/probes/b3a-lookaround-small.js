// B3a: exhaustion inside a negative look-ahead (control size): /^(?!(?:(a+)+b|a*c))/ on a^16 c;
// the body matches (a*c), so the assertion fails and the specification's answer is null.
var s = 'a'.repeat(16) + 'c';
t('/^(?!(?:(a+)+b|a*c))/u on a^16c', function () { return m(/^(?!(?:(a+)+b|a*c))/u, s); });
t('/^(?!(?:(a+)+b|a*c))/ on a^16c', function () { return m(/^(?!(?:(a+)+b|a*c))/, s); });
