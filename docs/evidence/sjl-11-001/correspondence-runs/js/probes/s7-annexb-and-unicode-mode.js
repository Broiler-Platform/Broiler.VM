// S7: Annex B's relaxations without u, and their refusal with u (the survey's list); the modifiers
// (F3), each by the constructor, the literal evaluated and the literal never evaluated.
// Broiler.JS runs this probe under the invariant culture, tr-TR and lt-LT (it holds (?i:) rows);
// the first line shows the culture each run took.
t('host default locale (Intl)', function () { return new Intl.NumberFormat().resolvedOptions().locale; });
var cases = [
  ['(?=a){2}', '', 'a'], ['(?=a){2}', 'u'], ['(?<=a)*', ''], ['(?!a)+', '', 'b'],
  ['{', '', '{'], ['{', 'u'], ['a{', '', 'a{'], ['a{1', '', 'a{1'], ['a{1,', '', 'a{1,'], ['x{2,1}', ''],
  [']', '', ']'], [']', 'u'], ['}', '', '}'], ['}', 'u'],
  ['[\\d-a]', '', '-'], ['[\\d-a]', 'u'], ['[a-\\d]', '', '-'], ['[--\\d]', '', '-'],
  ['a**', ''], ['a{2147483648}', '', 'a'], ['a{2147483648,}', '', 'a'], ['a{9007199254740993}', '', 'a'],
  ['[b-a]', ''], ['(?:)', '', ''], ['()', '', ''], ['[]', '', 'a'], ['[^]', '', 'a'],
  ['(', ''], [')', ''], ['a)', ''], ['(?', ''], ['(?a)', ''], ['[', ''], ['\\', ''],
  ['(?i:a)b', '', 'Ab'], ['(?i:a)b', '', 'AB'], ['(?-i:a)b', 'i', 'aB'], ['(?-i:a)b', 'i', 'AB'],
  ['(?ii:a)', ''], ['(?i-i:a)', ''], ['(?-:a)', ''], ['(?i)a', ''], ['(?u:a)', ''], ['(?m-s:a)', '', 'a'],
  ['(?s:.)', '', '\n'], ['(?m:^b)', '', 'a\nb'], ['(?i:\\u212a)', 'u', 'k'],
  // The modifier patterns of the literal tests test/language/literals/regexp/early-err-modifiers-*.js
  // and early-err-arithmetic-modifiers-*.js that Broiler.JS fails at sjl-10-001's after run (each
  // file's literal follows $DONOTEVALUATE(), so the error must be early; the third line of each
  // case is that path).
  ['(?imsi:a)', ''], ['(?I:a)', ''], ['(?M:a)', ''], ['(?S:a)', ''],
  ['(?m-m:a)', ''], ['(?s-s:a)', ''], ['(?ims-m:a)', ''], ['(?m-ims:a)', ''],
  ['(?-ii:a)', ''], ['(?-imsi:a)', ''], ['(?ii-:a)', ''], ['(?imsi-:a)', ''],
  ['(?I-:a)', ''], ['(?M-:a)', ''], ['(?S-:a)', ''], ['(?-I:a)', ''], ['(?-M:a)', ''], ['(?-S:a)', ''],
  ['(?ms-i)', ''], ['(?-s)', ''], ['(?i-)', ''],
];
for (var i = 0; i < cases.length; i++) {
  (function (c) {
    both(q(c[0]) + ' /' + c[1] + (c[2] !== undefined ? ' on ' + q(c[2]) : ''), c[0], c[1], c[2]);
  })(cases[i]);
}
