// Row M2: ToIntlMathematicalValue of a String: StringIntlMV of a StringNumericLiteral, exactly, then
// RoundMVResult's test for the infinities and zeros; anything else is not-a-number.
function f(o) { return new Intl.NumberFormat('en', o); }
var d = f();
t('M2 "1.15" maxFD 1; "1.005" maxFD 2', function () { return [f({ maximumFractionDigits: 1 }).format('1.15'), f({ maximumFractionDigits: 2 }).format('1.005')]; });
t('M2 " 12345678901234567890.5 "', function () { return d.format(' 12345678901234567890.5 '); });
t('M2 "9007199254740993"', function () { return d.format('9007199254740993'); });
t('M2 "1e3", ".5", "5.", "+.5e1", "-.5E-1"', function () { return [d.format('1e3'), d.format('.5'), d.format('5.'), d.format('+.5e1'), d.format('-.5E-1')]; });
t('M2 "", "   ", "-0", "+0", "-0.0001"', function () { return [d.format(''), d.format('   '), d.format('-0'), d.format('+0'), d.format('-0.0001')]; });
t('M2 "0x1F", "0b101", "0o17", "0X1f"', function () { return [d.format('0x1F'), d.format('0b101'), d.format('0o17'), d.format('0X1f')]; });
t('M2 "-0x1F", "0x", "1_000", "1e", "abc", "1n", "--1"', function () { return [d.format('-0x1F'), d.format('0x'), d.format('1_000'), d.format('1e'), d.format('abc'), d.format('1n'), d.format('--1')]; });
t('M2 "Infinity", "+Infinity", "-Infinity", " Infinity ", "infinity"', function () { return [d.format('Infinity'), d.format('+Infinity'), d.format('-Infinity'), d.format(' Infinity '), d.format('infinity')]; });
t('M2 "1e309", "-1e309", "1e-400", "-1e-400"', function () { return [d.format('1e309'), d.format('-1e309'), d.format('1e-400'), d.format('-1e-400')]; });
t('M2 "1.7976931348623158e308" (rounds to MAX) vs "1.7976931348623159e308" (to Infinity), lengths', function () { var g = f({ useGrouping: false }); return [g.format('1.7976931348623158e308').length, g.format('1.7976931348623159e308')]; });
t('M2 "2.4703282292062328e-324" (to MIN) vs "2.4703282292062327e-324" (to 0) maxSD 2', function () { var g = f({ maximumSignificantDigits: 2 }); return [g.format('2.4703282292062328e-324').length, g.format('2.4703282292062327e-324')]; });
t('M2 white space: "\\u00a01\\ufeff", "\\u20281\\u2029", "\\u00851"', function () { return [d.format('\u00a01\ufeff'), d.format('\u20281\u2029'), d.format('\u00851')]; });
t('M2 "1.0000000000000000000001" maxFD 25', function () { return f({ maximumFractionDigits: 25 }).format('1.0000000000000000000001'); });
t('M2 "1e-7" default and maxSD 3', function () { return [d.format('1e-7'), f({ maximumSignificantDigits: 3 }).format('1e-7')]; });
t('M2 "0.5" halfEven maxFD 0; "1.5"; "2.5"; "-2.5"', function () { var g = f({ maximumFractionDigits: 0, roundingMode: 'halfEven' }); return [g.format('0.5'), g.format('1.5'), g.format('2.5'), g.format('-2.5')]; });
t('M2 Symbol throws', function () { return d.format(Symbol('x')); });
t('M2 formatToParts("-0")', function () { return parts(d.formatToParts('-0')); });
t('M2 maxSD 2: "1e-400" (length), "-1e-400" (length)', function () { var g = f({ maximumSignificantDigits: 2 }); return [g.format('1e-400').length, g.format('-1e-400').length]; });
