// Row N7: scientific and engineering notation: the exponent, its symbols and parts, with each style.
var vals = [123456, 0.000123, -1.5e-7, 1e21, 0, 9.9999, 999.95];
['en', 'de'].forEach(function (l) {
  ['scientific', 'engineering'].forEach(function (n) {
    t(l + ' ' + n, function () { var f = new Intl.NumberFormat(l, { notation: n }); return vals.map(function (v) { return f.format(v); }); });
  });
  t(l + ' scientific formatToParts(-0.000123)', function () { return parts(new Intl.NumberFormat(l, { notation: 'scientific' }).formatToParts(-0.000123)); });
  t(l + ' currency USD scientific 1234.5, engineering 1234567', function () { return [new Intl.NumberFormat(l, { style: 'currency', currency: 'USD', notation: 'scientific' }).format(1234.5), new Intl.NumberFormat(l, { style: 'currency', currency: 'USD', notation: 'engineering' }).format(1234567)]; });
  t(l + ' unit meter scientific 1234.5', function () { return new Intl.NumberFormat(l, { style: 'unit', unit: 'meter', notation: 'scientific' }).format(1234.5); });
  t(l + ' scientific signDisplay always 1234, exceptZero 0', function () { return [new Intl.NumberFormat(l, { notation: 'scientific', signDisplay: 'always' }).format(1234), new Intl.NumberFormat(l, { notation: 'scientific', signDisplay: 'exceptZero' }).format(0)]; });
});
t('en scientific maximumSignificantDigits 2: 123456', function () { return new Intl.NumberFormat('en', { notation: 'scientific', maximumSignificantDigits: 2 }).format(123456); });
t('en currency USD scientific formatToParts(1234.5)', function () { return parts(new Intl.NumberFormat('en', { style: 'currency', currency: 'USD', notation: 'scientific' }).formatToParts(1234.5)); });
t('sv, fa scientific -1.5e-7', function () { return [new Intl.NumberFormat('sv', { notation: 'scientific' }).format(-1.5e-7), new Intl.NumberFormat('fa', { notation: 'scientific' }).format(-1.5e-7)]; });
