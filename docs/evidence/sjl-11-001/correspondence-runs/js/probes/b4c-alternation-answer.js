// B4c: an alternation whose answer shows whether quantifier-free alternation is charged:
// (?:(?:a|a) written out n times then b|a+) on a^n. The first alternative fails after 2^n paths;
// the specification's answer is the second's, index 0 and length n. A budget per start that the
// first alternative's paths exhaust at start 0 reads that start as a failure and answers a later
// index (or null); a budget they do not reach answers index 0.
function p(n) { return '(?:' + '(?:a|a)'.repeat(n) + 'b|a+)'; }
var ns = [12, 20, 24];
for (var i = 0; i < ns.length; i++) {
  (function (n) {
    t('(?:(?:a|a)x' + n + 'b|a+) /u on a^' + n, function () { var r = new RegExp(p(n), 'u').exec('a'.repeat(n)); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
    t('(?:(?:a|a)x' + n + 'b|a+) / on a^' + n, function () { var r = new RegExp(p(n), '').exec('a'.repeat(n)); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
  })(ns[i]);
}
