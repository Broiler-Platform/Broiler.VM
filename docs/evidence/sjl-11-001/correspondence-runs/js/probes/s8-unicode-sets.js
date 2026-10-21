// S8: the v flag's class syntax and its early errors, each by the constructor, the literal
// evaluated and the literal never evaluated.
// Broiler.JS runs this probe under the invariant culture, tr-TR and lt-LT (it holds vi rows); the
// first line shows the culture each run took.
t('host default locale (Intl)', function () { return new Intl.NumberFormat().resolvedOptions().locale; });
var cases = [
  ['[a&&&b]', 'v'], ['[a--b&&c]', 'v'], ['[(]', 'v'], ['[a-\\d]', 'v'], ['\\q{a}', 'v'], ['[\\q{a}]', 'u'],
  ['[^\\q{ab}]', 'v'], ['[^\\q{a}]', 'v', 'b'], ['[\\q{abc|d}]', 'v', 'abc'], ['[\\q{}]', 'v', ''],
  ['[[a-z]--[aeiou]]', 'v', 'e'], ['[[a-z]--[aeiou]]', 'v', 'b'], ['[\\w&&\\d]', 'v', 'a'], ['[\\w&&\\d]', 'v', '5'],
  ['[\\s--\\d]', 'v', ' '], ['[\\s&&\\S]', 'v', ' '], ['[\\p{L}--\\p{Ll}]', 'v', 'a'], ['[\\p{L}--\\p{Ll}]', 'v', 'A'],
  ['^\\p{RGI_Emoji}$', 'v', '\ud83d\udc68\u200d\ud83d\udc69\u200d\ud83d\udc67\u200d\ud83d\udc66'],
  ['^[\\p{RGI_Emoji}--\\q{\ud83d\ude00}]$', 'v', '\ud83d\ude00'],
  ['[a-z]', 'v', 'q'], ['[|]', 'v'], ['[a|]', 'v'], ['[a\\|]', 'v', '|'], ['[!!]', 'v'], ['[a!!b]', 'v'], ['[a\\!\\!b]', 'v', '!'],
  ['[\\q{KK}]', 'vi', 'kk'], ['[\\q{KK}]', 'vi', '\u212a\u212a'], ['[^[\\w]]', 'vi', '\u017f'],
];
for (var i = 0; i < cases.length; i++) {
  (function (c) {
    both(q(c[0]) + ' /' + c[1] + (c[2] !== undefined ? ' on ' + q(c[2]) : ''), c[0], c[1], c[2]);
  })(cases[i]);
}
// test262's test/built-ins/RegExp/prototype/unicodeSets/breaking-change-from-u-to-v-01.js to -28.js, in
// order: each a literal after $DONOTEVALUATE(), an early SyntaxError under v (the third line of
// each case is that path).
var breaking = [
  '[(]', '[)]', '[[]', '[{]', '[}]', '[/]', '[-]', '[|]', '[&&]', '[!!]',
  '[##]', '[$$]', '[%%]', '[**]', '[++]', '[,,]', '[..]', '[::]', '[;;]', '[<<]',
  '[==]', '[>>]', '[??]', '[@@]', '[``]', '[~~]', '[^^^]', '[_^^]',
];
for (var j = 0; j < breaking.length; j++) {
  both('breaking-change-from-u-to-v-' + ('0' + (j + 1)).slice(-2) + ' ' + q(breaking[j]) + ' /v', breaking[j], 'v');
}
