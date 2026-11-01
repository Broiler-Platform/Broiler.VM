// O3: the tailorings - German phonebook and search, root search, and the locales E3 names (en, es, sv, th) and CJK.
function sgn(n) { return n < 0 ? '<' : n > 0 ? '>' : '='; }
function ord(c, words) {
  var w = words.slice().sort(c.compare), s = q(w[0]);
  for (var i = 1; i < w.length; i++) s += ' ' + sgn(c.compare(w[i - 1], w[i])) + ' ' + q(w[i]);
  return R(s);
}
var de = ['Ab', 'Ae', 'Äb', 'Af', 'Od', 'Öb', 'Oe', 'Ub', 'Üb', 'Ue', 'ss', 'ß', 'st', 'a', 'ä', 'ae', 'b'];
[['de'], ['de', { collation: 'phonebk' }], ['de-u-co-phonebk'], ['de', { usage: 'search' }], ['de', { usage: 'search', sensitivity: 'base' }],
 ['en'], ['en', { usage: 'search' }], ['en', { usage: 'search', sensitivity: 'base' }], ['und', { usage: 'search' }]].forEach(function (a) {
  t('o3 ' + a[0] + ' ' + show(a[1] || {}) + ' de-words', function () { var c = new Intl.Collator(a[0], a[1]); return R(c.resolvedOptions().locale + '/' + c.resolvedOptions().collation + ': ' + ord(c, de).__raw); });
});
var es = ['n', 'ñ', 'o', 'nz', 'ña', 'ch', 'cz', 'd', 'll', 'lz', 'm'];
var sv = ['z', 'å', 'ä', 'ö', 'æ', 'ø', 'v', 'w', 'y', 'ü', 'a', 'o'];
var th = ['ก', 'เก', 'ข', 'แก', 'ก่', 'กา', '', ' ', 'ฯ', 'a', 'a b', 'ab'];
var cjk = ['一', '丁', '中', '汉', '字', '阿', '啊', '가', 'あ', 'ア', 'a'];
[['es', es], ['es', es, { collation: 'trad' }], ['sv', sv], ['fi', sv], ['th', th], ['zh', cjk], ['zh', cjk, { collation: 'stroke' }], ['ja', cjk], ['ko', cjk], ['tr', ['i', 'ı', 'I', 'İ', 'h', 'j']], ['da', ['aa', 'å', 'z', 'æ', 'ø']]].forEach(function (a) {
  t('o3 ' + a[0] + ' ' + show(a[2] || {}), function () { var c = new Intl.Collator(a[0], a[2]); return R(c.resolvedOptions().locale + '/' + c.resolvedOptions().collation + '/ip=' + c.resolvedOptions().ignorePunctuation + ': ' + ord(c, a[1]).__raw); });
});
t('o3 supportedLocalesOf', function () { return Intl.Collator.supportedLocalesOf(['de', 'de-DE', 'en', 'en-US', 'en-GB', 'es', 'sv', 'th', 'ja', 'zh', 'ko', 'tr', 'und', 'fr']); });
