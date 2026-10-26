// Rows M4 and M5: SetNumberFormatDigitOptions (ECMA-402 16.1.2): the reads, their order and count, the
// refusals, and the defaults each style hands it (16.1.1: currency digits, percent 0, else 0 and 3).
function nf(o) { return new Intl.NumberFormat('en', o); }
function ro(o) { var r = nf(o).resolvedOptions(); var keys = ['minimumIntegerDigits', 'minimumFractionDigits', 'maximumFractionDigits', 'minimumSignificantDigits', 'maximumSignificantDigits', 'roundingIncrement', 'roundingMode', 'roundingPriority', 'trailingZeroDisplay']; var a = []; keys.forEach(function (k) { if (k in r) a.push(k + '=' + show(r[k])); }); return R(a.join(' ')); }
var all = { localeMatcher: 'best fit', numberingSystem: 'latn', style: 'decimal', currency: 'EUR', currencyDisplay: 'symbol', currencySign: 'standard', unit: 'meter', unitDisplay: 'short', notation: 'standard', minimumIntegerDigits: 1, minimumFractionDigits: 0, maximumFractionDigits: 3, minimumSignificantDigits: 1, maximumSignificantDigits: 21, roundingIncrement: 1, roundingMode: 'halfExpand', roundingPriority: 'auto', trailingZeroDisplay: 'auto', compactDisplay: 'short', useGrouping: 'auto', signDisplay: 'auto' };
t('M4 NumberFormat option reads, in order', function () { var log = []; nf(spy(all, log)); return log; });
t('M4 PluralRules option reads, in order', function () { var log = []; new Intl.PluralRules('en', spy({ localeMatcher: 'best fit', type: 'cardinal', notation: 'standard', compactDisplay: 'short', minimumIntegerDigits: 1, minimumFractionDigits: 0, maximumFractionDigits: 3, minimumSignificantDigits: 1, maximumSignificantDigits: 21, roundingIncrement: 1, roundingMode: 'halfExpand', roundingPriority: 'auto', trailingZeroDisplay: 'auto' }, log)); return log; });
t('M4 NumberFormat: the reads after construction, by format then by resolvedOptions', function () { var log = []; var f = nf(spy({ maximumFractionDigits: 2, roundingMode: 'floor', trailingZeroDisplay: 'auto' }, log)); var n = log.length; f.format(1.234); var a = log.slice(n); n = log.length; f.resolvedOptions(); return [a, log.slice(n)]; });
t('M4 PluralRules: the reads after construction, by select then by resolvedOptions', function () { var log = []; var p = new Intl.PluralRules('en', spy({ maximumFractionDigits: 2, roundingMode: 'floor' }, log)); var n = log.length; p.select(1.234); var a = log.slice(n); n = log.length; p.resolvedOptions(); return [a, log.slice(n)]; });
t('M4 refusals: roundingIncrement 3', function () { return nf({ roundingIncrement: 3 }); });
t('M4 refusals: roundingIncrement 5 with maxSD 2 (TypeError)', function () { return nf({ roundingIncrement: 5, maximumSignificantDigits: 2 }); });
t('M4 refusals: roundingIncrement 5 with maxFD 2 (mnfd 0)', function () { return nf({ roundingIncrement: 5, maximumFractionDigits: 2 }); });
t('M4 refusals: roundingIncrement 5 with roundingPriority morePrecision', function () { return nf({ roundingIncrement: 5, minimumFractionDigits: 2, maximumFractionDigits: 2, roundingPriority: 'morePrecision' }); });
t('M4 refusals: minFD 3 > maxFD 2', function () { return nf({ minimumFractionDigits: 3, maximumFractionDigits: 2 }); });
t('M4 refusals: maxFD 101; minSD 0; minSD 22; minSD 5 maxSD 4; minID 0; minID 22', function () {
  var r = [];
  [{ maximumFractionDigits: 101 }, { minimumSignificantDigits: 0 }, { minimumSignificantDigits: 22 }, { minimumSignificantDigits: 5, maximumSignificantDigits: 4 }, { minimumIntegerDigits: 0 }, { minimumIntegerDigits: 22 }].forEach(function (o) { try { nf(o); r.push('ok'); } catch (e) { r.push(errName(e)); } });
  return r;
});
t('M4 refusals: roundingMode "half-even"; roundingPriority "more"; trailingZeroDisplay "strip"', function () { var r = []; [{ roundingMode: 'half-even' }, { roundingPriority: 'more' }, { trailingZeroDisplay: 'strip' }].forEach(function (o) { try { nf(o); r.push('ok'); } catch (e) { r.push(errName(e)); } }); return r; });
t('M4 maxFD 1.9 floors; minSD "2" coerces', function () { return [ro({ maximumFractionDigits: 1.9 }), ro({ minimumSignificantDigits: '2' })]; });
t('M5 defaults: {}', function () { return ro({}); });
t('M5 defaults: maxFD 1 / minFD 5', function () { return [ro({ maximumFractionDigits: 1 }), ro({ minimumFractionDigits: 5 })]; });
t('M5 defaults: percent; percent minFD 1', function () { return [ro({ style: 'percent' }), ro({ style: 'percent', minimumFractionDigits: 1 })]; });
t('M5 defaults: currency JPY, KWD, EUR maxFD 0', function () { return [ro({ style: 'currency', currency: 'JPY' }), ro({ style: 'currency', currency: 'KWD' }), ro({ style: 'currency', currency: 'EUR', maximumFractionDigits: 0 })]; });
t('M5 defaults: compact; currency EUR compact; compact maxFD 1', function () { return [ro({ notation: 'compact' }), ro({ style: 'currency', currency: 'EUR', notation: 'compact' }), ro({ notation: 'compact', maximumFractionDigits: 1 })]; });
t('M5 defaults: scientific currency EUR', function () { return ro({ style: 'currency', currency: 'EUR', notation: 'scientific' }); });
t('M5 defaults: maxSD 3; morePrecision maxSD 3; lessPrecision', function () { return [ro({ maximumSignificantDigits: 3 }), ro({ roundingPriority: 'morePrecision', maximumSignificantDigits: 3 }), ro({ roundingPriority: 'lessPrecision' })]; });
t('M5 defaults: roundingIncrement 25 (mxfd default becomes mnfd)', function () { return ro({ roundingIncrement: 25, minimumFractionDigits: 2 }); });
t('M5 percent 0.125, 0.5, 0.0049', function () { var f = nf({ style: 'percent' }); return [f.format(0.125), f.format(0.5), f.format(0.0049)]; });
t('M5 percent maxFD 2: 0.12345', function () { return nf({ style: 'percent', maximumFractionDigits: 2 }).format(0.12345); });
t('M5 (0.125).toLocaleString("en", {style: "percent"})', function () { return (0.125).toLocaleString('en', { style: 'percent' }); });
t('M5 currency JPY 1.5, KWD 1.23456, EUR 1.005', function () { return [nf({ style: 'currency', currency: 'JPY' }).format(1.5), nf({ style: 'currency', currency: 'KWD' }).format(1.23456), nf({ style: 'currency', currency: 'EUR' }).format(1.005)]; });
t('M5 minimumIntegerDigits 3: 5, 5 with maxSD 1, 1234', function () { var f = nf({ minimumIntegerDigits: 3 }); return [f.format(5), nf({ minimumIntegerDigits: 3, maximumSignificantDigits: 1 }).format(5), f.format(1234)]; });
t('M4 ToNumber of a digit option: valueOf calls by the constructor, then by format, then by resolvedOptions', function () { var n = 0; var v = { valueOf: function () { n++; return 2; } }; var f = nf({ maximumFractionDigits: v, minimumSignificantDigits: v }); var a = n; f.format(1.23456); var b = n; var r = f.resolvedOptions(); return [a, b - a, n - b, typeof r.minimumSignificantDigits]; });
t('M4 PluralRules: valueOf calls of a digit option by the constructor, select, resolvedOptions', function () { var n = 0; var v = { valueOf: function () { n++; return 2; } }; var p = new Intl.PluralRules('en', { maximumFractionDigits: v }); var a = n; p.select(1.23456); var b = n; p.resolvedOptions(); return [a, b - a, n - b]; });
