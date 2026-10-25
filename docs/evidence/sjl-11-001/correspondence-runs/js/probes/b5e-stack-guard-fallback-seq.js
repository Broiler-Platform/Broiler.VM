// B5e: 50,000 general quantifiers in sequence under u, (?:ab)* x50000 then c, on (ab)^10 c: the
// continuation chain deepens per term and Broiler.Regex 0.1.0-preview.4 checks its stack guard on
// entry to each quantifier (Matcher.cs:420-421). The specification's answer is the whole input.
var p = '(?:ab)*'.repeat(50000) + 'c';
t('(?:ab)* x50000 c /u exec on (ab)^10 c', function () { var r = new RegExp(p, 'u').exec('ab'.repeat(10) + 'c'); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
