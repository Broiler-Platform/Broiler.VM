// S10: under u, a four-digit escape of a lead surrogate followed by a braced escape. Two escapes
// form one code point only when both are four-digit (RegExpUnicodeEscapeSequence's first
// production); a braced escape never joins, so \uD83D\u{DC38} is a lone lead surrogate followed by
// a lone trail surrogate, and \uD83D\u{3042} a lone lead followed by U+3042 (test262
// staging/sm/RegExp/unicode-braced.js, unicode-lead-trail.js, unicode-class-lead-trail.js).
var cases = [
  ['\\uD83D\\u{DC38}+', 'u', '\ud83d\udc38\udc38'],
  ['\\u{D83D}\\u{DC38}+', 'u', '\ud83d\udc38\udc38'],
  ['\\u{D83D}\\uDC38+', 'u', '\ud83d\udc38\udc38'],
  ['\\uD83D\\uDC38+', 'u', '\ud83d\udc38\udc38'],
  ['\\uD83D\\u{3042}*', 'u', '\ud83d'],
  ['\\uD83D\\u{3042}*', 'u', '\ud83d\u3042\u3042'],
  ['[\\uD83D\\u{3042}]*', 'u', '\ud83d'],
  ['[\\uD83D\\u{3042}]*', 'u', '\ud83d\u3042\u3042\ud83d'],
  ['[\\uD83D\\u{DC38}]', 'u', '\udc38'],
  ['\\uD83D\\u{DC38}', 'v', '\ud83d\udc38'],
  ['\\uD83D\\u{DC38}', '', '\ud83du{DC38}'], ['\\uD83D\\u{DC38}', '', '\ud83d\udc38'],
];
for (var i = 0; i < cases.length; i++) {
  (function (c) {
    both(q(c[0]) + ' /' + c[1] + ' on ' + q(c[2]), c[0], c[1], c[2]);
  })(cases[i]);
}
