// M2: case-insensitive matching (Canonicalize: simple case folding with u or v; without them the
// simple upper case of one code unit, a non-ASCII character never folding to ASCII). Broiler.JS
// runs this probe under the invariant culture, tr-TR and lt-LT; the language reads no culture.
t('host default locale (Intl)', function () { return new Intl.NumberFormat().resolvedOptions().locale; });
t('host toLocaleUpperCase of "i"', function () { return 'i'.toLocaleUpperCase(); });
var cases = [
  // F2 of bundle sjl-7-001 and its neighbours.
  ['[a-z]', 'i', '\u212a'], ['[a-z]', 'iu', '\u212a'], ['\u212a', 'i', 'k'], ['\u212a', 'iu', 'k'], ['k', 'i', '\u212a'],
  ['[a-z]', 'i', '\u017f'], ['[a-z]', 'iu', '\u017f'], ['\u017f', 'i', 's'], ['\u017f', 'iu', 'S'], ['s', 'i', '\u017f'],
  ['\\w', 'iu', '\u017f'], ['\\W', 'iu', '\u017f'], ['\\w', 'i', '\u017f'], ['[\\w]', 'iu', '\u212a'], ['\\b', 'iu', '\u017f'],
  // The dotted and dotless i: the Turkic mappings are not Canonicalize's.
  ['i', 'i', '\u0130'], ['I', 'i', '\u0131'], ['i', 'i', '\u0131'], ['I', 'i', '\u0130'],
  ['\u0130', 'i', 'i'], ['\u0130', 'i', 'I'], ['\u0131', 'i', 'i'], ['\u0131', 'i', 'I'],
  ['[a-z]', 'i', '\u0130'], ['[a-z]', 'i', '\u0131'], ['[\u0130]', 'i', 'i'], ['[\u0131]', 'i', 'I'],
  ['i', 'iu', '\u0130'], ['I', 'iu', '\u0131'], ['\u0130', 'iu', 'i'], ['\u0131', 'iu', 'I'],
  ['[a-z]', 'iu', '\u0130'], ['[a-z]', 'iu', '\u0131'], ['i', 'iv', '\u0130'], ['[a-z]', 'iv', '\u0131'],
  ['(?i:i)', '', '\u0130'], ['(?i:[a-z])', '', '\u0131'],
  // Lithuanian: i with a combining dot above, and the soft-dotted capitals.
  ['i\u0307', 'i', '\u0130'], ['\u00cc', 'i', '\u00ec'], ['\u00cc', 'i', 'i\u0307\u0300'], ['\u0128', 'i', '\u0129'],
  // Others.
  ['\u03c3', 'i', '\u03c2'], ['\u03c3', 'iu', '\u03c2'], ['\u00df', 'i', '\u1e9e'], ['\u00df', 'iu', '\u1e9e'],
  ['\u1f80', 'i', '\u1f88'], ['\u0390', 'iu', '\u1fd3'], ['\u0390', 'i', '\u1fd3'],
  ['\\u{10400}', 'iu', '\ud801\udc28'], ['\u00b5', 'i', '\u039c'], ['\u00b5', 'iu', '\u03bc'],
  ['\ua7ce', 'i', '\ua7cf'], ['\ua7ce', 'iu', '\ua7cf'], ['\u1c89', 'i', '\u1c8a'], ['\u1c89', 'iu', '\u1c8a'],
  ['\\p{Lu}', 'iu', 'a'], ['\\P{Lu}', 'iu', 'A'], ['[^\\p{Lu}]', 'iu', 'A'], ['\\p{Lowercase_Letter}', 'iv', 'A'],
  ['(a)\\1', 'i', 'aA'], ['(\u0131)\\1', 'i', '\u0131I'], ['(i)\\1', 'i', 'i\u0130'],
];
for (var i = 0; i < cases.length; i++) {
  (function (c) {
    t(q(c[0]) + ' /' + c[1] + ' test ' + q(c[2]), function () { return C(c[0], c[1]).test(c[2]); });
  })(cases[i]);
}
