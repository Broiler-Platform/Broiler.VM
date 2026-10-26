// Row N10: the unit options (style, currency, currencyDisplay, currencySign, unit, unitDisplay) are read
// once, by the constructor; nothing format or resolvedOptions does reads the options object again.
var log = [];
var nf = new Intl.NumberFormat('en', spy({ style: 'currency', currency: 'EUR', currencyDisplay: 'code', currencySign: 'accounting' }, log));
t('constructor reads (currency options)', function () { return log.slice(); });
log.length = 0;
nf.format(-1);
t('reads by format(-1)', function () { return log.slice(); });
log.length = 0;
nf.formatToParts(-1);
nf.formatRange(1, 2);
t('reads by formatToParts and formatRange', function () { return log.slice(); });
log.length = 0;
nf.resolvedOptions();
t('reads by resolvedOptions', function () { return log.slice(); });
var log2 = [];
var nu = new Intl.NumberFormat('en', spy({ style: 'unit', unit: 'liter', unitDisplay: 'long' }, log2));
log2.length = 0;
nu.format(2); nu.resolvedOptions();
t('unit style: reads by format and resolvedOptions', function () { return log2.slice(); });
var o = { style: 'currency', currency: 'EUR' };
var f = new Intl.NumberFormat('en', o);
t('before mutation: format(1), resolved style/currency', function () { var r = f.resolvedOptions(); return [f.format(1), r.style, r.currency]; });
o.currency = 'usd';
t('after o.currency = "usd": format(1), resolved currency', function () { return [f.format(1), f.resolvedOptions().currency]; });
o.style = 'percent';
t('after o.style = "percent": format(1), resolved style', function () { return [f.format(1), f.resolvedOptions().style]; });
var u = { style: 'unit', unit: 'liter' };
var fu = new Intl.NumberFormat('en', u);
u.unit = 'meter';
t('after u.unit = "meter": format(2), resolved unit', function () { return [fu.format(2), fu.resolvedOptions().unit]; });
var g = { style: 'currency', currency: 'EUR', currencyDisplay: 'symbol', currencySign: 'standard' };
var fg = new Intl.NumberFormat('en', g);
g.currencyDisplay = 'code'; g.currencySign = 'accounting';
t('after currencyDisplay/currencySign changed: format(-1), resolved', function () { var r = fg.resolvedOptions(); return [fg.format(-1), r.currencyDisplay, r.currencySign]; });
t('a getter that throws after construction: format(1)', function () {
  var n = 0;
  var b = { style: 'currency', get currency() { if (n++ > 0) throw new Error('read again'); return 'EUR'; } };
  var h = new Intl.NumberFormat('en', b);
  return h.format(1);
});
t('Number.prototype.toLocaleString reads each time (two calls)', function () {
  var l3 = [];
  var opts = spy({ style: 'percent' }, l3);
  (0.5).toLocaleString('en', opts); (0.5).toLocaleString('en', opts);
  return l3.filter(function (k) { return k === 'style'; }).length;
});
