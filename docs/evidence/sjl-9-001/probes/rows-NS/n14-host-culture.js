// Row N14: which number-formatting answers follow the host's culture data. Run with no LANG, and (Broiler.JS)
// under DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1: a named locale's separators, percent pattern and grouping
// sizes, and the default locale's.
['de', 'fr', 'de-CH', 'en-IN', 'sv', 'ar'].forEach(function (l) {
  t(l + ' resolved locale; 1234567.5; percent 0.5; EUR 1234.5', function () {
    return [new Intl.NumberFormat(l).resolvedOptions().locale, new Intl.NumberFormat(l).format(1234567.5),
      new Intl.NumberFormat(l, { style: 'percent' }).format(0.5), new Intl.NumberFormat(l, { style: 'currency', currency: 'EUR' }).format(1234.5)];
  });
});
t('default: resolved locale; 1234567.5; percent 0.5', function () {
  return [new Intl.NumberFormat().resolvedOptions().locale, new Intl.NumberFormat().format(1234567.5), new Intl.NumberFormat(undefined, { style: 'percent' }).format(0.5)];
});
t('(1234567.5).toLocaleString()', function () { return (1234567.5).toLocaleString(); });
t('supportedLocalesOf de, fr, en', function () { return Intl.NumberFormat.supportedLocalesOf(['de', 'fr', 'en']); });
