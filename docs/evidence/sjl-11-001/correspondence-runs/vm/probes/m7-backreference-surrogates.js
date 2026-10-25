// M7: a back-reference under u compares characters, which are code points: a captured lone lead
// surrogate does not match the first half of a surrogate pair (BackreferenceMatcher over the
// input's code points; test262 staging/sm/RegExp/unicode-back-reference.js). Without u the input
// is code units, and the same subject matches.
var cases = [
  ['foo(.+)bar\\1', 'u', 'foo\ud834bar\ud834\udc00'],
  ['foo(.+)bar\\1', 'u', 'foo\ud834bar\ud834\ud834'],
  ['foo(.+)bar\\1', 'u', 'foo\ud834bar\ud834A'],
  ['foo(.+)bar\\1', 'u', 'fooAbarA\udc00'],
  ['foo(.+)bar\\1', 'u', 'foo\udc00bar\udc00\ud834'],
  ['^(.+)\\1$', 'u', '\udc00foobar\ud834\udc00foobar\ud834'],
  ['foo(.+)bar\\1', 'iu', 'foo\ud834bar\ud834\udc00'],
  ['foo(?<x>.+)bar\\k<x>', 'u', 'foo\ud834bar\ud834\udc00'],
  ['(?<=\\1(.))b', 'u', '\ud834\udc00\udc00b'], ['(?<=\\1(.))b', '', '\ud834\udc00\udc00b'],
  ['foo(.+)bar\\1', 'v', 'foo\ud834bar\ud834\udc00'],
  ['foo(.+)bar\\1', '', 'foo\ud834bar\ud834\udc00'],
];
for (var i = 0; i < cases.length; i++) {
  (function (c) {
    t(q(c[0]) + ' /' + c[1] + ' on ' + q(c[2]), function () { return m(new RegExp(c[0], c[1]), c[2]); });
  })(cases[i]);
}
