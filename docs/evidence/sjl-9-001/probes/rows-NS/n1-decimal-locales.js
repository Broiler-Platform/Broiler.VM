// Row N1: the decimal pattern and symbols of a locale (separators, minus sign, grouping sizes, the minimum
// grouping digits), in the languages each engine has data for and in the ones test262 and D5 name.
var locs = ['en', 'en-US', 'de', 'de-DE', 'de-AT', 'de-CH', 'fr', 'fr-CH', 'es', 'it', 'pl', 'pt-PT', 'pt-BR', 'sv', 'nb',
  'ru', 'tr', 'nl', 'ja', 'zh', 'ko', 'en-IN', 'hi', 'ar', 'ar-EG', 'he', 'fa', 'th', 'xyz'];
var vals = [1234.5, -1234.5, 1234567.891, 1000, 10000, 0.5];
locs.forEach(function (l) {
  t(l + ' resolved locale, numberingSystem', function () { var r = new Intl.NumberFormat(l).resolvedOptions(); return [r.locale, r.numberingSystem]; });
  vals.forEach(function (v) {
    t(l + ' format(' + v + ')', function () { return new Intl.NumberFormat(l).format(v); });
  });
});
['de', 'fr', 'sv', 'de-CH', 'en-IN', 'ar', 'es'].forEach(function (l) {
  t(l + ' formatToParts(-1234567.5)', function () { return parts(new Intl.NumberFormat(l).formatToParts(-1234567.5)); });
});
t('es useGrouping auto 1234, 12345', function () { var f = new Intl.NumberFormat('es'); return [f.format(1234), f.format(12345)]; });
t('es useGrouping always 1234', function () { return new Intl.NumberFormat('es', { useGrouping: 'always' }).format(1234); });
t('pl useGrouping auto 1234, min2 1234, always 1234', function () { return [new Intl.NumberFormat('pl').format(1234), new Intl.NumberFormat('pl', { useGrouping: 'min2' }).format(1234), new Intl.NumberFormat('pl', { useGrouping: 'always' }).format(1234)]; });
t('en-IN 100000, 10000000 always', function () { var f = new Intl.NumberFormat('en-IN', { useGrouping: 'always' }); return [f.format(100000), f.format(10000000)]; });
t('Number.prototype.toLocaleString de, fr, sv', function () { return [(-1234.5).toLocaleString('de'), (-1234.5).toLocaleString('fr'), (-1234.5).toLocaleString('sv')]; });
t('Array.prototype.toLocaleString de', function () { return [1234.5, -0.5].toLocaleString('de'); });
t('NaN and Infinity in de, fr, ar, zh-TW', function () { return ['de', 'fr', 'ar', 'zh-TW'].map(function (l) { var f = new Intl.NumberFormat(l); return f.format(NaN) + ' | ' + f.format(-Infinity); }); });
