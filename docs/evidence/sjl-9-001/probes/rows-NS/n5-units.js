// Row N5: style "unit": CLDR's unit patterns by width, compound units, the plural a pattern is chosen by.
function uf(l, u, d, o) { return new Intl.NumberFormat(l, Object.assign({ style: 'unit', unit: u, unitDisplay: d }, o || {})); }
['en', 'de'].forEach(function (l) {
  ['kilometer-per-hour', 'liter', 'megabyte', 'celsius', 'second', 'mile-per-gallon', 'liter-per-kilometer', 'byte-per-second', 'kilobit-per-stone', 'fluid-ounce', 'day'].forEach(function (u) {
    ['short', 'long', 'narrow'].forEach(function (d) {
      t(l + ' ' + u + ' ' + d + ' 1, 2, -1.5', function () { var f = uf(l, u, d); return [f.format(1), f.format(2), f.format(-1.5)]; });
    });
  });
});
t('en liter long 1 with minimumFractionDigits 1', function () { return uf('en', 'liter', 'long', { minimumFractionDigits: 1 }).format(1); });
t('en liter long 1.0001 (max 3 fraction digits)', function () { return uf('en', 'liter', 'long').format(1.0001); });
t('en liter long 0.9999 with maximumFractionDigits 0', function () { return uf('en', 'liter', 'long', { maximumFractionDigits: 0 }).format(0.9999); });
t('en day long 1 and 1n', function () { var f = uf('en', 'day', 'long'); return [f.format(1), f.format(1n)]; });
t('de kilometer-per-hour long formatToParts(-1.5)', function () { return parts(uf('de', 'kilometer-per-hour', 'long').formatToParts(-1.5)); });
t('en kilometer-per-hour short formatToParts(1000)', function () { return parts(uf('en', 'kilometer-per-hour', 'short').formatToParts(1000)); });
t('en megabyte narrow formatToParts(5)', function () { return parts(uf('en', 'megabyte', 'narrow').formatToParts(5)); });
t('en kilometer compact 1234567', function () { return uf('en', 'kilometer', 'short', { notation: 'compact' }).format(1234567); });
t('en kilometer long compact 1234567', function () { return uf('en', 'kilometer', 'long', { notation: 'compact' }).format(1234567); });
['ja', 'ko', 'zh-TW', 'fr', 'es', 'pl'].forEach(function (l) {
  t(l + ' kilometer-per-hour short -987, long 2', function () { return [uf(l, 'kilometer-per-hour', 'short').format(-987), uf(l, 'kilometer-per-hour', 'long').format(2)]; });
});
['METER', 'meter-per', 'kilometer-per-hour-per-second', 'per-meter', 'acre-foot', 'meter\n', 'kılometer'].forEach(function (u) {
  t('invalid unit ' + q(u), function () { return uf('en', u, 'short').resolvedOptions().unit; });
});
t('unit with style decimal is not resolved', function () { var r = new Intl.NumberFormat('en', { unit: 'meter' }).resolvedOptions(); return ['unit' in r, 'unitDisplay' in r]; });
