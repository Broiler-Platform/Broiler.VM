// vm-fuel: 200000000000
// Row M9: the charge of ToIntlMathematicalValue's string read and of FormatNumericToString grows with
// the digits in Broiler.VM (JsDecimal.FromString, JsNumberFormatter.FormatNumericToString); Broiler.JS
// meters nothing. The control m9-charge-control.js differs only in the last line. The value is
// 0.111..., a million fraction digits, within the Number range, so it is formatted, not an infinity.
var s = '0.' + '1'.repeat(1000000);
var nf = new Intl.NumberFormat('en', { maximumFractionDigits: 100 });
t('M9 string length', function () { return s.length; });
t('M9 format("0.1")', function () { return nf.format('0.1'); });
t('M9 the long format: its length and its last characters', function () { var r = nf.format(s); return [r.length, r.slice(-3)]; });
