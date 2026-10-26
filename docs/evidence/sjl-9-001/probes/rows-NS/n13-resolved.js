// Row N13: resolvedOptions of the unit, currency and notation options: which keys, in which order, with which values.
var cases = [
  ['en', {}], ['en', { style: 'currency', currency: 'jpy' }], ['en', { style: 'currency', currency: 'EUR', currencyDisplay: 'name', currencySign: 'accounting' }],
  ['en', { currency: 'EUR' }], ['en', { style: 'unit', unit: 'kilometer-per-hour', unitDisplay: 'narrow' }], ['en', { unit: 'meter', unitDisplay: 'long' }],
  ['en', { notation: 'compact' }], ['en', { notation: 'compact', compactDisplay: 'long' }], ['en', { compactDisplay: 'long' }],
  ['en', { notation: 'scientific' }], ['en', { notation: 'engineering', useGrouping: false }], ['de', { style: 'percent', signDisplay: 'negative' }],
  ['en', { useGrouping: 'true' }], ['en', { useGrouping: true }], ['en', { useGrouping: '' }],
];
cases.forEach(function (c) {
  t(c[0] + ' ' + JSON.stringify(c[1]) + ' resolvedOptions', function () {
    var r = new Intl.NumberFormat(c[0], c[1]).resolvedOptions();
    return Object.keys(r).map(function (k) { return k + '=' + String(r[k]); }).join(' ');
  });
});
t('style "currency" without currency', function () { return new Intl.NumberFormat('en', { style: 'currency' }).format(1); });
t('style "unit" without unit', function () { return new Intl.NumberFormat('en', { style: 'unit' }).format(1); });
['US', 'USDD', 'U$D', 'ıSD', 'ＵSD', 'USD\n'].forEach(function (c) {
  t('invalid currency ' + q(c), function () { return new Intl.NumberFormat('en', { currency: c }).resolvedOptions().locale; });
});
t('invalid style, currencyDisplay, currencySign, unitDisplay, notation, compactDisplay, signDisplay', function () {
  return ['style', 'currencyDisplay', 'currencySign', 'unitDisplay', 'notation', 'compactDisplay', 'signDisplay'].map(function (k) {
    var o = {}; o[k] = 'bogus';
    try { new Intl.NumberFormat('en', o); return k + ':ok'; } catch (e) { return k + ':' + e.name; }
  }).join(' ');
});
