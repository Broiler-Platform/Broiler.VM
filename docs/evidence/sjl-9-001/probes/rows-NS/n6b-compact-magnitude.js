// Row N6f: compact notation's magnitude of a Number near a power of ten, which Broiler.JS computes from the
// double with Math.Log10 and Math.Pow (JSIntl.cs CompactMagnitude, FormatCompact) and Broiler.VM from the
// Intl mathematical value's digits (ComputeExponent over JsDecimal).
var vals = [1e21, 1e22, 1e23, 9.999999999999999e22, 1e24, 123456789012345680000, 99999.99999999999, 999999.9999999999, 0.001, 0.00099999];
['short', 'long'].forEach(function (d) {
  t('en compact ' + d, function () { var f = new Intl.NumberFormat('en', { notation: 'compact', compactDisplay: d }); return vals.map(function (v) { return f.format(v); }); });
});
t('en compact formatToParts(1e23)', function () { return parts(new Intl.NumberFormat('en', { notation: 'compact' }).formatToParts(1e23)); });
t('de compact 1e23, 999999.9999999999', function () { var f = new Intl.NumberFormat('de', { notation: 'compact' }); return [f.format(1e23), f.format(999999.9999999999)]; });
t('en engineering 1e23, 9.999999999999999e22', function () { var f = new Intl.NumberFormat('en', { notation: 'engineering' }); return [f.format(1e23), f.format(9.999999999999999e22)]; });
