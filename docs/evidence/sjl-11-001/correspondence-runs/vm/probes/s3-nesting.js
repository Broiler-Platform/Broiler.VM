// S3: the nesting policy. The language has no bound; Broiler.VM refuses past 512 levels of
// disjunction (the pattern itself is level 1, each group or v-mode class one more).
function nest(open, close, n, atom) { return open.repeat(n) + atom + close.repeat(n); }
var depths = [510, 511, 512, 1000];
for (var i = 0; i < depths.length; i++) {
  (function (n) {
    var p = nest('(?:', ')', n, 'a');
    t('(?: x' + n + ' new RegExp', function () { return m(C(p, ''), 'a'); });
    t('(?: x' + n + ' /u new RegExp', function () { return m(C(p, 'u'), 'a'); });
    t('(?: x' + n + ' literal', function () { return m(L(p, ''), 'a'); });
    t('(?: x' + n + ' literal, not evaluated', function () { return early(p, ''); });
    var c = nest('(', ')', n, 'a');
    t('( x' + n + ' new RegExp, last group', function () { var r = C(c, '').exec('a'); return R('length ' + r.length + ' last ' + q(r[r.length - 1])); });
    var v = nest('[', ']', n, 'a');
    t('[ x' + n + ' /v new RegExp', function () { return m(C(v, 'v'), 'a'); });
    var la = nest('(?=', ')', n, 'a');
    t('(?= x' + n + ' new RegExp', function () { return m(C(la, ''), 'a'); });
  })(depths[i]);
}
