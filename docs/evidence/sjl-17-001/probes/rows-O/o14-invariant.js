// O14: the comparisons the invariant probe holds (part C of Broiler.JS's rule), run here in every column: each line names the resolved locale.
function sgn(n) { return n < 0 ? '<' : n > 0 ? '>' : '='; }
function ord(c, words) {
  var w = words.slice().sort(c.compare), s = q(w[0]);
  for (var i = 1; i < w.length; i++) s += ' ' + sgn(c.compare(w[i - 1], w[i])) + ' ' + q(w[i]);
  return R(s);
}
var w = ['b', 'B', 'a', 'A', 'ä', 'Ä', 'ae', 'z', 'é', 'e', 'E', 'ñ', 'n', '1', '10', '2', ' ', '-', 'a-b', 'ab'];
[['en'], ['de'], ['de-u-co-phonebk'], ['sv'], ['es'], ['th'], ['ja'], ['xx'], [undefined], ['en', { sensitivity: 'base' }], ['en', { numeric: true }], ['en', { caseFirst: 'upper' }], ['en', { ignorePunctuation: true }]].forEach(function (a) {
  t('o14 ' + String(a[0]) + ' ' + show(a[1] || {}), function () { var c = new Intl.Collator(a[0], a[1]); return R(c.resolvedOptions().locale + ': ' + ord(c, w).__raw); });
});
t('o14 localeCompare a B', function () { return R(sgn('a'.localeCompare('B')) + ' ' + sgn('a'.localeCompare('B', 'de')) + ' ' + sgn('ä'.localeCompare('b'))); });
t('o14 supportedLocalesOf', function () { return Intl.Collator.supportedLocalesOf(['de', 'en', 'en-US', 'sv', 'th', 'ja']); });
