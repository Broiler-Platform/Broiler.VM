// SJL-9.1 sub-area (a), row L8b: a -u-sd- or -u-rg- value whose CLDR subdivision alias replaces it by a region
// code (CLDR's subdivisionAlias: fi01 -> AX, cn71 -> TW, frgp -> GP). UTS #35 writes the region as a
// subdivision value, the region lowercased with "zzzz" appended; the result must stay a structurally
// valid, canonical tag, so canonicalizing it again changes nothing.
function g(s) {
  t('getCanonicalLocales ' + q(s), function () { return Intl.getCanonicalLocales(s); });
  t('  again ' + q(s), function () { var once = Intl.getCanonicalLocales(s)[0]; return [once, Intl.getCanonicalLocales(once)[0]]; });
}
['en-u-sd-fi01', 'en-u-rg-fi01', 'en-u-rg-cn71', 'zh-u-sd-cn91', 'fr-u-sd-frgp', 'fr-u-rg-frgua', 'en-u-sd-no23', 'en-u-sd-axzzzz']
  .forEach(g);
['en-u-rg-fi01', 'zh-u-rg-cn71', 'en-u-rg-axzzzz', 'zh-u-rg-twzzzz'].forEach(function (s) {
  t('Locale ' + q(s), function () { var l = new Intl.Locale(s); return [l.toString(), l.getWeekInfo().firstDay, l.getHourCycles()]; });
});
t('Locale "en-u-sd-AX" (the unescaped form, which is a key)', function () { return new Intl.Locale('en-u-sd-AX').toString(); });
