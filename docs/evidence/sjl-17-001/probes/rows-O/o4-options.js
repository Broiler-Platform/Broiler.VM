// O4 to O7: the four options - sensitivity, ignorePunctuation, numeric, caseFirst - with their keywords and defaults.
function sgn(n) { return n < 0 ? '<' : n > 0 ? '>' : '='; }
function ord(c, words) {
  var w = words.slice().sort(c.compare), s = q(w[0]);
  for (var i = 1; i < w.length; i++) s += ' ' + sgn(c.compare(w[i - 1], w[i])) + ' ' + q(w[i]);
  return R(s);
}
function ro(c) { var r = c.resolvedOptions(); return r.locale + ' ' + r.usage + ' ' + r.sensitivity + ' ip=' + r.ignorePunctuation + ' co=' + r.collation + ' kn=' + r.numeric + ' kf=' + r.caseFirst; }
var acc = ['a', 'A', 'á', 'Á', 'b', 'ä', 'Ä'];
['base', 'accent', 'case', 'variant'].forEach(function (s) {
  t('o4 sensitivity ' + s, function () { var c = new Intl.Collator('en', { sensitivity: s }); return R(ord(c, acc).__raw); });
  t('o4 sensitivity ' + s + ' search', function () { var c = new Intl.Collator('en', { sensitivity: s, usage: 'search' }); return R(ord(c, acc).__raw); });
});
t('o4 defaults sort', function () { return ro(new Intl.Collator('en')); });
t('o4 defaults search', function () { return ro(new Intl.Collator('en', { usage: 'search' })); });
t('o4 defaults de search', function () { return ro(new Intl.Collator('de', { usage: 'search' })); });
var pu = ['ab', 'a b', 'a-b', 'a_b', 'a.b', 'a b', 'aB', 'a‐b', 'a$b', 'a+b', 'a€b', '', ' ', '-'];
[false, true].forEach(function (ip) {
  t('o5 ignorePunctuation ' + ip, function () { var c = new Intl.Collator('en', { ignorePunctuation: ip }); return R(ord(c, pu).__raw); });
  t('o5 ignorePunctuation ' + ip + ' base', function () { var c = new Intl.Collator('en', { ignorePunctuation: ip, sensitivity: 'base' }); return R(ord(c, pu).__raw); });
});
t('o5 th default', function () { return ro(new Intl.Collator('th')); });
t('o5 th-u-ka-noignore', function () { return ro(new Intl.Collator('th-u-ka-noignore')); });
var nu = ['a10', 'a2', 'a02', 'a1', 'a01', 'a001', 'a9', 'a١٠', 'a٢', 'a２', '10', '9', '1.5', '1.10', 'b1', 'a'];
t('o6 numeric absent', function () { var c = new Intl.Collator('en'); return R(ro(c) + ': ' + ord(c, nu).__raw); });
t('o6 numeric true', function () { var c = new Intl.Collator('en', { numeric: true }); return R(ro(c) + ': ' + ord(c, nu).__raw); });
t('o6 -u-kn', function () { var c = new Intl.Collator('en-u-kn'); return R(ro(c) + ': ' + ord(c, nu).__raw); });
t('o6 -u-kn-false', function () { var c = new Intl.Collator('en-u-kn-false'); return R(ro(c)); });
t('o6 -u-kn with numeric false', function () { var c = new Intl.Collator('en-u-kn', { numeric: false }); return R(ro(c) + ': ' + sgn(c.compare('a2', 'a10'))); });
t('o6 a2 a10 localeCompare numeric', function () { return R(sgn('a2'.localeCompare('a10', 'en', { numeric: true }))); });
var cf = ['a', 'A', 'b', 'B', 'ab', 'Ab', 'aB', 'AB', 'ä', 'Ä'];
['false', 'upper', 'lower'].forEach(function (k) {
  t('o7 caseFirst ' + k, function () { var c = new Intl.Collator('en', { caseFirst: k }); return R(ro(c) + ': ' + ord(c, cf).__raw); });
  t('o7 caseFirst ' + k + ' case', function () { var c = new Intl.Collator('en', { caseFirst: k, sensitivity: 'case' }); return R(ord(c, cf).__raw); });
});
t('o7 -u-kf-upper', function () { var c = new Intl.Collator('en-u-kf-upper'); return R(ro(c) + ': ' + ord(c, cf).__raw); });
t('o7 -u-kf-upper caseFirst lower', function () { var c = new Intl.Collator('en-u-kf-upper', { caseFirst: 'lower' }); return R(ro(c)); });
t('o7 de phonebk upper', function () { var c = new Intl.Collator('de-u-co-phonebk-kf-upper'); return R(ro(c) + ': ' + ord(c, ['ä', 'Ä', 'ae', 'Ae', 'AE', 'af']).__raw); });
t('o7 a A localeCompare upper', function () { return R(sgn('a'.localeCompare('A', 'en', { caseFirst: 'upper' }))); });
