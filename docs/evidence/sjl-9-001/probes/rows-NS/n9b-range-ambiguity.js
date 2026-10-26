// Row N9b: CollapseNumberRange "must not introduce ambiguity": two different ranges must not format alike
// unless their uncollapsed forms are alike. Each line formats two ranges and says whether the texts are equal.
function pair(l, o, a, b) {
  var f = new Intl.NumberFormat(l, o);
  var x = f.formatRange(a[0], a[1]), y = f.formatRange(b[0], b[1]);
  return [x, y, x === y];
}
t('en {} (-5, -1) and (-5, 1)', function () { return pair('en', {}, [-5, -1], [-5, 1]); });
t('en {} (-1, -1.5) and (-1, 1.5)', function () { return pair('en', { maximumFractionDigits: 1 }, [-1, -1.5], [-1, 1.5]); });
t('en signDisplay always (1, 5) and (1, -5)', function () { return pair('en', { signDisplay: 'always' }, [1, 5], [1, -5]); });
t('en signDisplay always (-1, -5) and (-1, 5)', function () { return pair('en', { signDisplay: 'always' }, [-1, -5], [-1, 5]); });
t('en kilometer (-5, -1) and (-5, 1)', function () { return pair('en', { style: 'unit', unit: 'kilometer' }, [-5, -1], [-5, 1]); });
t('en percent (-0.05, -0.01) and (-0.05, 0.01)', function () { return pair('en', { style: 'percent' }, [-0.05, -0.01], [-0.05, 0.01]); });
t('de {} (-5, -1) and (-5, 1)', function () { return pair('de', {}, [-5, -1], [-5, 1]); });
t('en USD (-5, -1) and (-5, 1)', function () { return pair('en', { style: 'currency', currency: 'USD' }, [-5, -1], [-5, 1]); });
t('en USD accounting (-5, -1) and (-5, 1)', function () { return pair('en', { style: 'currency', currency: 'USD', currencySign: 'accounting' }, [-5, -1], [-5, 1]); });
