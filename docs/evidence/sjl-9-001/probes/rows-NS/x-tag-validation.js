// Not a row of this sub-area: the witness for notes.md's point to sub-area a. Row S9 found that Broiler.JS's
// DisplayNames accepts a language code with a final line feed or non-ASCII digits; this probe asks whether the same
// tags pass Broiler.JS's general tag validation (getCanonicalLocales, Intl.Locale, a constructor's locales).
['en\n', 'en-US\n', 'de-١٢٣', 'en-١٢٣', 'de-DE\n'].forEach(function (tag) {
  t('tag ' + q(tag) + ': getCanonicalLocales; Intl.Locale; NumberFormat locales', function () {
    return [function () { return Intl.getCanonicalLocales(tag); }, function () { return new Intl.Locale(tag).toString(); },
      function () { return new Intl.NumberFormat(tag).resolvedOptions().locale; }].map(function (f) {
      try { return f(); } catch (e) { return e.name; }
    });
  });
});
