// M1: matching semantics the specification's examples and notes fix (22.2.2), outside the budget.
// Broiler.JS runs this probe under the invariant culture, tr-TR and lt-LT (it holds i rows); the
// first line shows the culture each run took.
t('host default locale (Intl)', function () { return new Intl.NumberFormat().resolvedOptions().locale; });
var cases = [
  ['((a)|b)+', '', 'ab'], ['(a?b??)*', '', 'ab'], ['(z)((a+)?(b+)?(c))*', '', 'zaacbbbcac'],
  ['(?=(a+))a*b\\1', '', 'baaabac'], ['(.*?)a(?!(a+)b\\2c)\\2(.*)', '', 'baaabaac'],
  ['(a*)*b', '', 'aab'], ['(a*?)*?b', '', 'aab'], ['(?:a|ab)(?:c|bcd)(d*)', '', 'abcd'],
  ['(a)|\\1b', '', 'b'], ['\\1(a)', '', 'aa'], ['(?:(a)|b)\\1', '', 'b'], ['(a\\1)', '', 'aa'],
  ['(?<=(\\d+)(\\d+))$', '', '1053'], ['(?<=\\1(a))b', '', 'aab'], ['(?<=(?<a>\\w){3})f', '', 'abcdef'],
  ['(?<!a)b', '', 'ab'], ['(?<!a)b', '', 'cb'], ['(?<=^|,)\\w+', 'g', 'x,y'],
  ['(?=(a))a', '', 'a'], ['(?!(a))b', '', 'b'], ['(a)?(?:b)?\\1c', '', 'bc'],
  ['^.$', '', '\ud83d\ude00'], ['^.$', 'u', '\ud83d\ude00'], ['^..$', '', '\ud83d\ude00'],
  ['\\u{1F600}', 'u', '\ud83d\ude00'], ['[\ud83d\ude00]', 'u', '\ud83d\ude00'], ['[\ud83d\ude00]', '', '\ud83d\ude00'],
  ['\\uD83D', 'u', '\ud83d\ude00'], ['\\uD83D', '', '\ud83d\ude00'], ['\\uDE00', 'u', '\ud83d\ude00'],
  ['\\uD83D\\uDE00', 'u', '\ud83d\ude00'], ['\\u{D83D}\\u{DE00}', 'u', '\ud83d\ude00'], ['[^a]', 'u', '\ud83d\ude00'],
  ['\\S', 'u', '\ud83d\ude00'], ['\\W', 'u', '\ud83d\ude00'], ['\ud83d\ude00{2}', 'u', '\ud83d\ude00\ud83d\ude00'],
  ['(.)\\1', 'u', '\ud83d\ude00\ud83d\ude00'], ['(.)\\1', 'iu', 'Aa'], ['\\b', 'u', ''],
  ['a{2,3}?', '', 'aaa'], ['a{0}', '', 'a'], ['(?:a|())*', '', 'aa'], ['(a|)*', '', 'aa'],
  ['(?<a>a)|b', 'd', 'b'], ['(?<a>a)(?<b>b)?', 'd', 'a'], ['[\\d-z]+', '', '1-z'],
];
for (var i = 0; i < cases.length; i++) {
  (function (c) {
    t(q(c[0]) + ' /' + c[1] + ' on ' + q(c[2]), function () { return m(C(c[0], c[1]), c[2]); });
  })(cases[i]);
}
t('"aaa".replace(/a*?/g, "-")', function () { return 'aaa'.replace(/a*?/g, '-'); });
t('"\ud83d\ude00".split(/(?:)/u)', function () { return '\ud83d\ude00'.split(/(?:)/u); });
t('"\ud83d\ude00".split(/(?:)/)', function () { return '\ud83d\ude00'.split(/(?:)/); });
t('"a,b".split(/(,)/)', function () { return 'a,b'.split(/(,)/); });
t('"abc".split(/(?<=b)/)', function () { return 'abc'.split(/(?<=b)/); });
t('[..."a1b2".matchAll(/\\d/g)].map(r=>r.index)', function () { return Array.from('a1b2'.matchAll(/\d/g)).map(function (r) { return r.index; }); });
t('"abc".replace(/(?<x>b)/, "[$<x>|$1|$&|$`|$\'|$$]")', function () { return 'abc'.replace(/(?<x>b)/, "[$<x>|$1|$&|$`|$'|$$]"); });
