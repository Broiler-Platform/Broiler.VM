// Row M6: the rounding FormatNumericToString applies (ECMA-402 16.5.3): rounding modes, increments,
// rounding priority, trailingZeroDisplay.
function nf(o) { return new Intl.NumberFormat('en', o); }
var modes = ['ceil', 'floor', 'expand', 'trunc', 'halfCeil', 'halfFloor', 'halfExpand', 'halfTrunc', 'halfEven'];
modes.forEach(function (m) {
  t('M6 ' + m + ' maxFD 0: -2.5 -1.5 -0.5 0.5 1.5 2.5 -0.4 0.6', function () { var f = nf({ maximumFractionDigits: 0, roundingMode: m }); return [-2.5, -1.5, -0.5, 0.5, 1.5, 2.5, -0.4, 0.6].map(function (x) { return f.format(x); }).join(' '); });
});
t('M6 increment 25 minFD 2 maxFD 2: 1.13, 1.12, 1.125 halfEven, 1.375 halfEven', function () { var f = nf({ roundingIncrement: 25, minimumFractionDigits: 2, maximumFractionDigits: 2 }); var g = nf({ roundingIncrement: 25, minimumFractionDigits: 2, maximumFractionDigits: 2, roundingMode: 'halfEven' }); return [f.format(1.13), f.format(1.12), g.format(1.125), g.format(1.375)]; });
t('M6 increment 5000 maxFD 0... minFD 0 maxFD 0: 12345, 7499, 7500', function () { var f = nf({ roundingIncrement: 5000, maximumFractionDigits: 0 }); return [f.format(12345), f.format(7499), f.format(7500)]; });
t('M6 morePrecision minSD 2 maxFD 0: 1.234; 123.4', function () { var f = nf({ roundingPriority: 'morePrecision', maximumSignificantDigits: 2, maximumFractionDigits: 0 }); return [f.format(1.234), f.format(123.4)]; });
t('M6 lessPrecision maxSD 2 maxFD 0: 1.234; 123.4', function () { var f = nf({ roundingPriority: 'lessPrecision', maximumSignificantDigits: 2, maximumFractionDigits: 0 }); return [f.format(1.234), f.format(123.4)]; });
t('M6 maxSD 3 minSD 3: 0, 9.995, 0.000123456', function () { var f = nf({ maximumSignificantDigits: 3, minimumSignificantDigits: 3 }); return [f.format(0), f.format(9.995), f.format(0.000123456)]; });
t('M7 trailingZeroDisplay stripIfInteger minFD 2: 5, 5.1, 4.9999, 5.001', function () { var f = nf({ minimumFractionDigits: 2, trailingZeroDisplay: 'stripIfInteger' }); return [f.format(5), f.format(5.1), f.format(4.9999), f.format(5.001)]; });
t('M7 trailingZeroDisplay stripIfInteger minSD 2 maxSD 2: 1, 1.5', function () { var f = nf({ minimumSignificantDigits: 2, maximumSignificantDigits: 2, trailingZeroDisplay: 'stripIfInteger' }); return [f.format(1), f.format(1.5)]; });
t('M7 trailingZeroDisplay stripIfInteger currency EUR: 5, 5.5', function () { var f = nf({ style: 'currency', currency: 'EUR', trailingZeroDisplay: 'stripIfInteger' }); return [f.format(5), f.format(5.5)]; });
t('M7 trailingZeroDisplay stripIfInteger formatToParts minFD 2: 5', function () { return parts(nf({ minimumFractionDigits: 2, trailingZeroDisplay: 'stripIfInteger' }).formatToParts(5)); });
t('M7 trailingZeroDisplay stripIfInteger percent minFD 1: 0.05', function () { return nf({ style: 'percent', minimumFractionDigits: 1, trailingZeroDisplay: 'stripIfInteger' }).format(0.05); });
t('M6 increment 25 with minFD 2 only (maxFD resolves to 2): 1.13, 1.12; resolvedOptions maxFD', function () { var f = nf({ roundingIncrement: 25, minimumFractionDigits: 2 }); return [f.format(1.13), f.format(1.12), f.resolvedOptions().maximumFractionDigits]; });
t('M6 increment 5 with no fraction option (both resolve to 0): 7, 12; currency USD increment 5: 1.23', function () { return [nf({ roundingIncrement: 5 }).format(7), nf({ roundingIncrement: 5 }).format(12), nf({ style: 'currency', currency: 'USD', roundingIncrement: 5 }).format(1.23)]; });
t('M6 increment 5 currency JPY (cDigits 0): 7', function () { return nf({ style: 'currency', currency: 'JPY', roundingIncrement: 5 }).format(7); });
