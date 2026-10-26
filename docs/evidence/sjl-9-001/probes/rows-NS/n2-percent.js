// Row N2: the percent pattern and sign of a locale (style "percent", and unit "percent").
['en', 'de', 'de-CH', 'fr', 'sv', 'tr', 'es', 'ar', 'ja'].forEach(function (l) {
  t(l + ' percent 0.5, -0.5, 12.34', function () { var f = new Intl.NumberFormat(l, { style: 'percent' }); return [f.format(0.5), f.format(-0.5), f.format(12.34)]; });
  t(l + ' percent formatToParts(-0.5)', function () { return parts(new Intl.NumberFormat(l, { style: 'percent' }).formatToParts(-0.5)); });
});
t('en percent signDisplay always 0.5, exceptZero 0', function () { return [new Intl.NumberFormat('en', { style: 'percent', signDisplay: 'always' }).format(0.5), new Intl.NumberFormat('en', { style: 'percent', signDisplay: 'exceptZero' }).format(0)]; });
t('de unit percent 50, long 50', function () { return [new Intl.NumberFormat('de', { style: 'unit', unit: 'percent' }).format(50), new Intl.NumberFormat('de', { style: 'unit', unit: 'percent', unitDisplay: 'long' }).format(50)]; });
t('en unit percent formatToParts(50)', function () { return parts(new Intl.NumberFormat('en', { style: 'unit', unit: 'percent' }).formatToParts(50)); });
t('de percent formatToParts(0.5)', function () { return parts(new Intl.NumberFormat('de', { style: 'percent' }).formatToParts(0.5)); });
t('en percent compact 1234.5', function () { return new Intl.NumberFormat('en', { style: 'percent', notation: 'compact' }).format(1234.5); });
t('en percent scientific 1234.5', function () { return new Intl.NumberFormat('en', { style: 'percent', notation: 'scientific' }).format(1234.5); });
