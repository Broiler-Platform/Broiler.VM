// B1a: control for B1b-B1e: the subject alone, a^10000001, built and measured, no regular expression.
var s = 'a'.repeat(10000001);
t('a^10000001 length', function () { return s.length; });
