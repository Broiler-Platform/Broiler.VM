// Row N6: compact notation: the locale's patterns and thresholds, the exponent chosen after rounding, the
// default rounding (two significant digits or whole numbers), plural compact patterns, compact currency.
var vals = [999, 999.5, 999.995, 1000, 1500, 12345, 99999, 999999, 1e6, 1.2e6, 2e6, 1e9, 1e12, 1.5e15, 0.012345, -12345];
['en', 'de', 'ja', 'ko', 'zh', 'zh-TW', 'en-IN', 'fr', 'es', 'pt', 'ru'].forEach(function (l) {
  ['short', 'long'].forEach(function (d) {
    t(l + ' compact ' + d, function () { var f = new Intl.NumberFormat(l, { notation: 'compact', compactDisplay: d }); return vals.map(function (v) { return f.format(v); }); });
  });
});
t('de compact long 1e6, 2e6, 1e9, 2e9 (plural)', function () { var f = new Intl.NumberFormat('de', { notation: 'compact', compactDisplay: 'long' }); return [f.format(1e6), f.format(2e6), f.format(1e9), f.format(2e9)]; });
t('en compact 5e-324, 0.0001234', function () { var f = new Intl.NumberFormat('en', { notation: 'compact' }); return [f.format(5e-324), f.format(0.0001234)]; });
t('en compact "123456789012345678901234567890.5"', function () { return new Intl.NumberFormat('en', { notation: 'compact' }).format('123456789012345678901234567890.5'); });
t('en compact maximumSignificantDigits 3: 12345, 1234567', function () { var f = new Intl.NumberFormat('en', { notation: 'compact', maximumSignificantDigits: 3 }); return [f.format(12345), f.format(1234567)]; });
t('en compact maximumFractionDigits 2: 12345, 1234567', function () { var f = new Intl.NumberFormat('en', { notation: 'compact', maximumFractionDigits: 2 }); return [f.format(12345), f.format(1234567)]; });
t('en compact roundingMode floor 1990', function () { return new Intl.NumberFormat('en', { notation: 'compact', roundingMode: 'floor' }).format(1990); });
t('en compact formatToParts(-1234567)', function () { return parts(new Intl.NumberFormat('en', { notation: 'compact' }).formatToParts(-1234567)); });
t('en compact long formatToParts(1234567)', function () { return parts(new Intl.NumberFormat('en', { notation: 'compact', compactDisplay: 'long' }).formatToParts(1234567)); });
t('ja compact formatToParts(987654321)', function () { return parts(new Intl.NumberFormat('ja-JP', { notation: 'compact' }).formatToParts(987654321)); });
t('en compact useGrouping default 1234 (no compact), 12345678912', function () { var f = new Intl.NumberFormat('en', { notation: 'compact' }); return [f.format(1234), f.format(1234567891234567)]; });
['en', 'de'].forEach(function (l) {
  t(l + ' currency USD compact 0, 1234.5, 1234567, -1234567', function () { var f = new Intl.NumberFormat(l, { style: 'currency', currency: 'USD', notation: 'compact' }); return [f.format(0), f.format(1234.5), f.format(1234567), f.format(-1234567)]; });
  t(l + ' currency EUR compact long 1234567', function () { return new Intl.NumberFormat(l, { style: 'currency', currency: 'EUR', notation: 'compact', compactDisplay: 'long' }).format(1234567); });
  t(l + ' currency EUR name compact 1234567', function () { return new Intl.NumberFormat(l, { style: 'currency', currency: 'EUR', currencyDisplay: 'name', notation: 'compact' }).format(1234567); });
});
t('en currency USD compact formatToParts(1234567)', function () { return parts(new Intl.NumberFormat('en', { style: 'currency', currency: 'USD', notation: 'compact' }).formatToParts(1234567)); });
