// Row N9: formatRange and formatRangeToParts: the separator and its spacing, which layers collapse, the
// approximately sign, the plural of a collapsed name, and the range across locales.
var cases = [
  ['en', {}], ['en', { signDisplay: 'always' }], ['en', { style: 'percent' }], ['en', { style: 'currency', currency: 'USD' }],
  ['en', { style: 'currency', currency: 'EUR', currencyDisplay: 'code' }], ['en', { style: 'currency', currency: 'EUR', currencyDisplay: 'name' }],
  ['en', { style: 'currency', currency: 'USD', currencySign: 'accounting' }], ['en', { style: 'unit', unit: 'kilometer' }],
  ['en', { style: 'unit', unit: 'kilometer', unitDisplay: 'long' }], ['en', { notation: 'compact' }], ['en', { notation: 'scientific' }],
  ['de', {}], ['de', { style: 'currency', currency: 'EUR' }], ['de', { style: 'unit', unit: 'kilometer', unitDisplay: 'long' }],
  ['de', { style: 'percent' }],
];
var ranges = [[3, 5], [1, 1], [-5, -1], [-1, 1], [1000, 5000], [1, 2], [0.9999, 1.0001], [5, 3]];
cases.forEach(function (c) {
  t(c[0] + ' ' + JSON.stringify(c[1]) + ' formatRange', function () {
    var f = new Intl.NumberFormat(c[0], c[1]);
    return ranges.map(function (r) { return f.formatRange(r[0], r[1]); });
  });
});
t('en USD formatRangeToParts(3, 5)', function () { return parts(new Intl.NumberFormat('en', { style: 'currency', currency: 'USD' }).formatRangeToParts(3, 5)); });
t('en {} formatRangeToParts(-5, -1)', function () { return parts(new Intl.NumberFormat('en').formatRangeToParts(-5, -1)); });
t('en kilometer long formatRangeToParts(1, 5)', function () { return parts(new Intl.NumberFormat('en', { style: 'unit', unit: 'kilometer', unitDisplay: 'long' }).formatRangeToParts(1, 5)); });
t('de {} formatRangeToParts(1, 1)', function () { return parts(new Intl.NumberFormat('de').formatRangeToParts(1, 1)); });
t('en maximumFractionDigits 0 formatRangeToParts(0.9999, 1.0001)', function () { return parts(new Intl.NumberFormat('en', { maximumFractionDigits: 0 }).formatRangeToParts(0.9999, 1.0001)); });
t('en signDisplay never formatRange(-1, 1)', function () { return new Intl.NumberFormat('en', { signDisplay: 'never' }).formatRange(-1, 1); });
t('en USD accounting formatRangeToParts(-5, -1)', function () { return parts(new Intl.NumberFormat('en', { style: 'currency', currency: 'USD', currencySign: 'accounting' }).formatRangeToParts(-5, -1)); });
['pt-PT', 'fr', 'ja', 'es'].forEach(function (l) {
  t(l + ' EUR formatRange(3, 5), {} formatRange(3, 5)', function () { return [new Intl.NumberFormat(l, { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).formatRange(3, 5), new Intl.NumberFormat(l).formatRange(3, 5)]; });
});
t('formatRange with BigInt and String ends', function () { var f = new Intl.NumberFormat('en'); return [f.formatRange(1n, 5n), f.formatRange('1', '5.5')]; });
