// B4a: quantifier-free alternation, (?:a|a) written out n times then b, on a^n (no match: 2^n paths).
function p(n) { return '(?:a|a)'.repeat(n) + 'b'; }
var ns = [12, 16, 20, 22];
for (var i = 0; i < ns.length; i++) {
  (function (n) {
    t('(?:a|a)x' + n + 'b /u on a^' + n, function () { var re = new RegExp(p(n), 'u'), s = 'a'.repeat(n); var e = elapsed(function () { return re.exec(s); }); return R(show(e.r) + ' elapsedMs ' + e.ms); });
    t('(?:a|a)x' + n + 'b / on a^' + n, function () { var re = new RegExp(p(n), ''), s = 'a'.repeat(n); var e = elapsed(function () { return re.exec(s); }); return R(show(e.r) + ' elapsedMs ' + e.ms); });
  })(ns[i]);
}
