// O1 and O2: root order across scripts, canonical equivalence and ignorables, under `en` (no tailoring in either engine's data).
function sgn(n) { return n < 0 ? '<' : n > 0 ? '>' : '='; }
function ord(c, words) {
  var w = words.slice().sort(c.compare), s = q(w[0]);
  for (var i = 1; i < w.length; i++) s += ' ' + sgn(c.compare(w[i - 1], w[i])) + ' ' + q(w[i]);
  return R(s);
}
var en = new Intl.Collator('en');
var scripts = ['a', 'z', 'ß', 'ss', 'æ', 'ae', 'ø', 'œ', 'þ', 'ð', 'ı', 'α', 'ω', 'а', 'я',
  'ა', 'ա', 'א', 'ת', 'ا', 'ي', 'अ', 'ह', 'ก', 'ฮ', '가', '힣', 'あ', 'ア', 'ん',
  '一', '丁', '中', '𠀀', '😀', '1', '١', '१', '½', '①', 'Ꭰ', 'ሀ', 'ཀ', 'ᠠ', 'ꀀ'];
t('o1 scripts', function () { return ord(en, scripts); });
var punct = [' ', '_', '-', ',', ';', ':', '!', '?', '.', "'", '"', '(', ')', '[', '@', '*', '/', '\\', '&', '#', '%', '`', '^', '+', '<', '=', '|', '~', '$', '€', '0', 'a'];
t('o1 punctuation and symbols', function () { return ord(en, punct); });
var pairs = [
  ['å', 'å'], ['Å', 'Å'], ['Ω', 'Ω'], ['q̣̇', 'q̣̇'], ['ḍ̇', 'ḍ̇'],
  ['ä', 'ä'], ['ab', 'a­b'], ['ab', 'a​b'], ['ab', 'a\u0000b'], ['ab', 'a﻿b'], ['a', 'á'], ['ﬁ', 'fi'],
  ['ǆ', 'dž'], ['①', '1'], ['Ａ', 'A'], ['ª', 'a'], ['เก', 'กเ'], ['가', '가'],
  ['', ' '], ['', '­'], ['a b', 'ab'], ['a-b', 'ab'], ['à֮̀̕b', 'à֮̀̕b']];
pairs.forEach(function (p) { t('o2 ' + q(p[0]) + ' vs ' + q(p[1]), function () { return R(sgn(en.compare(p[0], p[1]))); }); });
t('o1 und resolves', function () { return new Intl.Collator('und').resolvedOptions().locale; });
t('o1 root order und = en', function () { var u = new Intl.Collator('und'); return scripts.every(function (a) { return scripts.every(function (b) { return u.compare(a, b) === en.compare(a, b); }); }); });
