// B5g: which engine answers after the stack guard. Two patterns whose .NET translation is known to
// answer otherwise than the specification (JSRegExp.Broiler.cs:16-27, problems 3, 4 and 8):
// (?<=(\d+)(\d+))$ on "1053" (captures "1" and "053") and (a?b??)* on "ab" (capture "b"); each
// prefixed with 10 and with 50,000 general quantifiers (?:ab)*, under u. One process for all.
function run(n, tail, s) {
  var re = new RegExp('(?:ab)*'.repeat(n) + tail, 'u');
  var r = re.exec(s);
  return r === null ? R('null') : R('@' + r.index + ' ' + show(Array.prototype.slice.call(r)));
}
t('(?:ab)* x10 (?<=(\\d+)(\\d+))$ /u on "1053"', function () { return run(10, '(?<=(\\d+)(\\d+))$', '1053'); });
t('(?:ab)* x50000 (?<=(\\d+)(\\d+))$ /u on "1053"', function () { return run(50000, '(?<=(\\d+)(\\d+))$', '1053'); });
t('(?:ab)* x10 (a?b??)* /u on "ab"', function () { return run(10, '(?:x)?(a?b??)*', 'ab'); });
t('(?:ab)* x50000 (a?b??)* /u on "ab"', function () { return run(50000, '(?:x)?(a?b??)*', 'ab'); });
t('control, no prefix: (a?b??)* / on "ab"', function () { return m(new RegExp('(a?b??)*', ''), 'ab'); });
