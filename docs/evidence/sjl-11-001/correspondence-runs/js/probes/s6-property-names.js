// S6: property-escape names under u and v. ES2026 admits exactly the names and aliases of its
// tables (22.2.2.9, UnicodeMatchProperty and UnicodeMatchPropertyValue): no loose matching, no
// spaces, no blocks, no Is- prefix; Script and Script_Extensions take Katakana_Or_Hiragana (Hrkt).
var names = [
  'L', 'Letter', 'Lu', 'Uppercase_Letter', 'gc=Lu', 'General_Category=Lu', 'General_Category=Uppercase_Letter',
  'Script=Greek', 'Script=Grek', 'sc=Grek', 'scx=Grek', 'Script_Extensions=Greek',
  'Script=Katakana_Or_Hiragana', 'sc=Hrkt', 'scx=Hrkt',
  'ASCII', 'Any', 'Assigned', 'ASCII_Hex_Digit', 'AHex', 'Lowercase', 'Lower', 'Emoji',
  'letter', 'ascii', 'Ascii', 'A S C I I', 'lu', 'uppercaseletter', 'uppercase-letter', 'Uppercase-Letter',
  'general_category=Lu', 'GC=Lu', 'gc=lu', 'Script=greek', 'script=Greek', 'Script = Greek', 'ScriptExtensions=Greek',
  'scx:Greek', 'Block=Basic_Latin', 'InBasicLatin', 'Is_Lowercase', 'IsLowercase', 'General_Category', 'Script',
  'Lu=Lu', 'Alphabetic=Yes', 'ASCII=Y', '', 'L&', 'LC', 'Cased_Letter', 'Lowercase_Letter=Ll',
  'RGI_Emoji', 'rgi_emoji', 'Basic_Emoji', 'Emoji_Keycap_Sequence',
];
for (var i = 0; i < names.length; i++) {
  (function (n) {
    var p = '\\p{' + n + '}';
    t(q(p) + ' /u new RegExp', function () { return acc(C, p, 'u'); });
    t(q(p) + ' /u literal', function () { return acc(L, p, 'u'); });
    t(q(p) + ' /u literal, not evaluated', function () { return early(p, 'u'); });
    t(q(p) + ' /v new RegExp', function () { return acc(C, p, 'v'); });
    t(q('[' + p + ']') + ' /u new RegExp', function () { return acc(C, '[' + p + ']', 'u'); });
  })(names[i]);
}
both('\\P{RGI_Emoji} /v', '\\P{RGI_Emoji}', 'v');
both('[^\\p{RGI_Emoji}] /v', '[^\\p{RGI_Emoji}]', 'v');
both('\\p{L} / (no u: identity escapes) on "p{L}"', '\\p{L}', '', 'p{L}');
both('\\p{sc=Hrkt} /u on U+30A2', '\\p{sc=Hrkt}', 'u', '\u30a2');
both('\\p{scx=Hrkt} /u on U+30A2', '\\p{scx=Hrkt}', 'u', '\u30a2');
both('\\p{Script=Hiragana} /u on U+3042', '\\p{Script=Hiragana}', 'u', '\u3042');
