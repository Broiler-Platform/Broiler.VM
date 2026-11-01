// O8: lone surrogates and code points around them.
function sgn(n) { return n < 0 ? '<' : n > 0 ? '>' : '='; }
var en = new Intl.Collator('en');
var p = [['\ud800', '\udc00'], ['\ud800', '�'], ['\udc00', '�'], ['\ud800', 'a'], ['\ud800', '\ud800'], ['\ud800', '\udbff'], ['a\ud800', 'a'],
  ['a\ud800', 'a\udc00'], ['𐀀', '\ud800'], ['𐀀', '�'], ['\ud800b', '\udc00a'], ['�', '￿'], ['\ud83d', '😀'],
  ['\udfff', '一'], ['\ud800', '͸'], ['ä\ud800', 'ä\ud800']];
p.forEach(function (x) { t('o8 ' + q(x[0]) + ' vs ' + q(x[1]), function () { return R(sgn(en.compare(x[0], x[1])) + ' / base ' + sgn(new Intl.Collator('en', { sensitivity: 'base' }).compare(x[0], x[1]))); }); });
t('o8 localeCompare lone', function () { return R(sgn('\ud800'.localeCompare('\udc00'))); });
