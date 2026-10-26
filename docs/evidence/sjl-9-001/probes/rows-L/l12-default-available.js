// SJL-9.1 sub-area (a), row L44: [[AvailableLocales]] must include the value DefaultLocale returns
// (negotiation.html, sec-internal-slots). For each service: its DefaultLocale (the resolved locale of no
// request), whether supportedLocalesOf keeps that tag, and the same for "en-US" and "en".
['Collator', 'DateTimeFormat', 'DisplayNames', 'DurationFormat', 'ListFormat', 'NumberFormat', 'PluralRules',
 'RelativeTimeFormat', 'Segmenter'].forEach(function (name) {
  var C = Intl[name];
  function make(locales) {
    if (name === 'DisplayNames') return new C(locales, { type: 'region' });
    return new C(locales);
  }
  t(name + ' DefaultLocale', function () { return make(undefined).resolvedOptions().locale; });
  t(name + ' supportedLocalesOf(DefaultLocale)', function () {
    if (typeof C.supportedLocalesOf !== 'function') return R('(no supportedLocalesOf)');
    return C.supportedLocalesOf([make(undefined).resolvedOptions().locale]);
  });
  t(name + ' supportedLocalesOf(["en-US", "en"])', function () {
    if (typeof C.supportedLocalesOf !== 'function') return R('(no supportedLocalesOf)');
    return C.supportedLocalesOf(['en-US', 'en']);
  });
  t(name + ' resolution of "en-US"', function () { return make('en-US').resolvedOptions().locale; });
});
t('Intl.getCanonicalLocales of the DefaultLocale', function () {
  return Intl.getCanonicalLocales(new Intl.NumberFormat().resolvedOptions().locale);
});
