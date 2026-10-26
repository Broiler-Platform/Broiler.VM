// Rows N3 (currency symbols, placement, digits and spacing) and N4 (currencyDisplay "name").
function cf(l, o) { return new Intl.NumberFormat(l, Object.assign({ style: 'currency' }, o)); }
['en', 'de'].forEach(function (l) {
  ['USD', 'EUR', 'JPY', 'GBP', 'CHF', 'KWD', 'BTN', 'XYZ'].forEach(function (c) {
    ['symbol', 'narrowSymbol', 'code', 'name'].forEach(function (d) {
      t(l + ' ' + c + ' ' + d + ' 1, -1234.5', function () { var f = cf(l, { currency: c, currencyDisplay: d }); return [f.format(1), f.format(-1234.5)]; });
    });
  });
});
t('en EUR name 0, 1, 2, 1.5', function () { var f = cf('en', { currency: 'EUR', currencyDisplay: 'name' }); return [f.format(0), f.format(1), f.format(2), f.format(1.5)]; });
t('de EUR name 1, 2', function () { var f = cf('de', { currency: 'EUR', currencyDisplay: 'name' }); return [f.format(1), f.format(2)]; });
t('en EUR name formatToParts(2)', function () { return parts(cf('en', { currency: 'EUR', currencyDisplay: 'name' }).formatToParts(2)); });
t('en EUR code formatToParts(0)', function () { return parts(cf('en', { currency: 'EUR', currencyDisplay: 'code' }).formatToParts(0)); });
t('en KWD code 1.2345', function () { return cf('en', { currency: 'KWD', currencyDisplay: 'code' }).format(1.2345); });
t('de USD code 1', function () { return cf('de', { currency: 'USD', currencyDisplay: 'code' }).format(1); });
t('en USD accounting -5, signDisplay always 5, 0, -0', function () { var f = cf('en', { currency: 'USD', currencySign: 'accounting', signDisplay: 'always' }); return [f.format(-5), f.format(5), f.format(0), f.format(-0)]; });
t('en USD accounting formatToParts(-5)', function () { return parts(cf('en', { currency: 'USD', currencySign: 'accounting' }).formatToParts(-5)); });
t('de EUR accounting -5', function () { return cf('de', { currency: 'EUR', currencySign: 'accounting' }).format(-5); });
t('en USD exceptZero -0.001, 0.001', function () { var f = cf('en', { currency: 'USD', signDisplay: 'exceptZero' }); return [f.format(-0.001), f.format(0.001)]; });
['fr', 'ja', 'es', 'pt-PT', 'ko-KR', 'zh-TW', 'en-IN', 'sv', 'ar'].forEach(function (l) {
  t(l + ' EUR symbol -1234.5, USD symbol 1', function () { return [cf(l, { currency: 'EUR' }).format(-1234.5), cf(l, { currency: 'USD' }).format(1)]; });
});
t('currency lower case eur resolved and formatted', function () { var f = cf('en', { currency: 'eur' }); return [f.resolvedOptions().currency, f.format(1)]; });
t('currency with maximumFractionDigits 0: JPY 1.5, USD 1.5', function () { return [cf('en', { currency: 'JPY' }).format(1.5), cf('en', { currency: 'USD', maximumFractionDigits: 0 }).format(1.5)]; });
t('currency with minimumFractionDigits 3: USD 1.5', function () { return cf('en', { currency: 'USD', minimumFractionDigits: 3 }).format(1.5); });
t('currency with maximumSignificantDigits 2: USD 1234.5', function () { return cf('en', { currency: 'USD', maximumSignificantDigits: 2 }).format(1234.5); });
