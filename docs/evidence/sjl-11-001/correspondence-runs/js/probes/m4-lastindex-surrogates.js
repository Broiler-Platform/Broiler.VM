// M4: lastIndex inside a surrogate pair, and AdvanceStringIndex, under u and v (RegExpBuiltinExec
// steps 12-22: the input index is the code point holding lastIndex; "index" is lastIndex).
var pair = '\ud834\udf06';
function at(re, s, li) {
  re.lastIndex = li;
  var r = re.exec(s);
  return R((r === null ? 'null' : '@' + r.index + ' ' + show(Array.prototype.slice.call(r))) + ' lastIndex ' + re.lastIndex);
}
t('/./gu lastIndex 1 on pair', function () { return at(/./gu, pair, 1); });
t('/./gv lastIndex 1 on pair', function () { return at(/./gv, pair, 1); });
t('/./g lastIndex 1 on pair', function () { return at(/./g, pair, 1); });
t('/\\udf06/gu lastIndex 1 on pair', function () { return at(/\udf06/gu, pair, 1); });
t('/\\udf06/g lastIndex 1 on pair', function () { return at(/\udf06/g, pair, 1); });
t('/\\udf06/u lastIndex 0 on pair', function () { return at(/\udf06/u, pair, 0); });
t('/(?:)/gu lastIndex 1 on pair', function () { return at(/(?:)/gu, pair, 1); });
t('/./yu lastIndex 1 on pair', function () { return at(/./yu, pair, 1); });
t('/\\udf06/yu lastIndex 1 on pair', function () { return at(/\udf06/yu, pair, 1); });
t('/./gu lastIndex 1 on "a"+pair', function () { return at(/./gu, 'a' + pair, 1); });
t('/./gu lastIndex 2 on "a"+pair', function () { return at(/./gu, 'a' + pair, 2); });
t('/(?<=\\ud834)./gu lastIndex 1 on pair', function () { return at(/(?<=\ud834)./gu, pair, 1); });
t('pair.match(/(?:)/gu).length', function () { return pair.match(/(?:)/gu).length; });
t('pair.match(/(?:)/g).length', function () { return pair.match(/(?:)/g).length; });
t('pair.replace(/(?:)/gu, "-")', function () { return pair.replace(/(?:)/gu, '-'); });
t('pair.replace(/(?:)/gv, "-")', function () { return pair.replace(/(?:)/gv, '-'); });
t('pair.split(/(?:)/u)', function () { return pair.split(/(?:)/u); });
t('[...pair.matchAll(/(?:)/gu)].map(r=>r.index)', function () { return Array.from(pair.matchAll(/(?:)/gu)).map(function (r) { return r.index; }); });
t('pair.search(/\\udf06/u)', function () { return pair.search(/\udf06/u); });
t('pair.search(/\\udf06/)', function () { return pair.search(/\udf06/); });
