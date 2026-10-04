// THE `v` FLAG (phase F2, JSC-262): set operations, nested classes, string literals and properties of
// strings, the case folding `i` applies to a class's operands, and the early errors. The values are
// the comparison engine's.
var r = [];
r.push(/^[[0-9]--\p{Emoji_Keycap_Sequence}]+$/v.test('0123'), /^[[0-9]--\p{Emoji_Keycap_Sequence}]+$/v.test('6\uFE0F\u20E3'));
r.push(/^\p{RGI_Emoji}$/v.test('\u{1F468}\u200D\u{1F469}\u200D\u{1F467}'), /^\p{RGI_Emoji}$/v.test('a'));
r.push(/^[\q{abc|d}]+$/v.test('abcdabc'), /^[\q{abc|d}]$/v.test('ab'), /[\q{a|ab}]/v.exec('ab')[0]);
r.push(/[\p{L}&&\p{Script=Greek}]/v.test('\u03B1'), /[\p{L}&&\p{Script=Greek}]/v.test('a'));
r.push(/\P{Ll}/iv.test('a'), /\P{Ll}/iu.test('a'), /[^\P{Ll}]/iv.test('A'), /[\p{Lu}&&[a-z]]/iv.test('Q'));
r.push(new RegExp('x', 'v').flags, new RegExp('x', 'dgimsvy').flags, /x/v.unicodeSets, /x/u.unicodeSets);
['[a-z&&b]', '[a&&&b]', '[^\\p{RGI_Emoji}]', '\\P{RGI_Emoji}', '[(]', '[a!!b]', '[a&&b--c]', '[\\q{ab}-c]'].forEach(function (bad) {
  try { new RegExp(bad, 'v'); r.push('admitted ' + bad); } catch (e) { r.push(e.name); }
});
try { new RegExp('x', 'uv'); } catch (e) { r.push(e.name); }
r.push('\u{1F468}\u200D\u{1F469}\u200D\u{1F467}x'.match(/\p{RGI_Emoji}/gv).length, 'a\u{1F600}b'.split(/(?:)/v).length);
r.join(' ');
