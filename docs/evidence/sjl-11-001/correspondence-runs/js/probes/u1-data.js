// U1: the Unicode data each engine's regular expressions read: \s and \w, property escapes,
// case folding and string properties, at Unicode 17.0.0 (the programme's pin, SJL-D-6).
t('host default locale (Intl)', function () { return new Intl.NumberFormat().resolvedOptions().locale; });
var cases = [
  ['\\s', '', '\ufeff'], ['\\s', '', '\u0085'], ['\\s', '', '\u180e'], ['\\s', '', '\u00a0'], ['\\s', '', '\u3000'],
  ['\\s', 'u', '\ufeff'], ['\\s', 'u', '\u0085'], ['\\S', '', '\u0085'], ['[\\s]', '', '\u2029'], ['\\s', 'v', '\u1680'],
  ['\\d', 'u', '\u0661'], ['\\d', '', '\u0661'], ['\\w', '', '\u00e9'], ['\\w', 'u', '\u0130'], ['\\w', 'i', '\u0130'],
  ['\\p{L}', 'u', '\ua7ce'], ['\\p{Lu}', 'u', '\ua7ce'], ['\\p{L}', 'u', '\ud803\udd70'], ['\\p{Cn}', 'u', '\ua7ce'],
  ['\\p{sc=Garay}', 'u', '\ud803\udd70'], ['\\p{Script=Sidetic}', 'u', '\ud802\udd40'], ['\\p{sc=Sidt}', 'u', '\ud802\udd40'],
  ['\\p{Script=Beria_Erfe}', 'u'], ['\\p{Script=Tolong_Siki}', 'u'], ['\\p{Script=Tai_Yo}', 'u'], ['\\p{Script=Chisoi}', 'u'],
  ['\\p{ID_Start}', 'u', '\ua7ce'], ['\\p{Emoji}', 'u', '\u2615'], ['\\p{Extended_Pictographic}', 'u', '\ud83e\udee9'],
  ['\\p{Assigned}', 'u', '\ua7ce'], ['\\p{Any}', 'u', '\udfff'],
  ['\ua7ce', 'i', '\ua7cf'], ['\ua7ce', 'iu', '\ua7cf'], ['[\ua7ce]', 'i', '\ua7cf'], ['[\ua7ce]', 'iu', '\ua7cf'],
  ['\u1c89', 'iu', '\u1c8a'], ['\\u{10D50}', 'iu', '\ud803\udd70'], ['\\u{16EA0}', 'iu', '\ud81b\udebb'],
  ['^\\p{RGI_Emoji}$', 'v', '\ud83e\udee9'], ['^\\p{Basic_Emoji}$', 'v', '\u231a'], ['^\\p{RGI_Emoji_Flag_Sequence}$', 'v', '\ud83c\uddfa\ud83c\uddf3'],
  ['^\\p{Emoji_Keycap_Sequence}$', 'v', '#\ufe0f\u20e3'], ['^\\p{RGI_Emoji_ZWJ_Sequence}$', 'v', '\ud83d\udc68\u200d\ud83d\udcbb'],
  ['(?<\ua7ce>x)', '', 'x'], ['(?<\ua7ce>x)', 'u', 'x'],
];
for (var i = 0; i < cases.length; i++) {
  (function (c) {
    t(q(c[0]) + ' /' + c[1] + (c[2] !== undefined ? ' test ' + q(c[2]) : ' accepted'), function () { var re = C(c[0], c[1]); return c[2] === undefined ? R('ok') : re.test(c[2]); });
  })(cases[i]);
}
