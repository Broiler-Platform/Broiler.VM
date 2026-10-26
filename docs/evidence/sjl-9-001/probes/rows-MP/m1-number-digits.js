// Rows M1 and M1b: ToIntlMathematicalValue of a Number. ECMA-402 reads a Number through Number::toString's
// digits (the shortest round-trip decimal), never through its exact binary value.
function nf(o) { return new Intl.NumberFormat('en', o); }
t('M1 maxFD 1: 1.15, 1.25, 1.35, 1.45, -1.15', function () { var f = nf({ maximumFractionDigits: 1 }); return [f.format(1.15), f.format(1.25), f.format(1.35), f.format(1.45), f.format(-1.15)]; });
t('M1 maxFD 2: 1.005, 2.675, 1.255, 8.345, 0.615', function () { var f = nf({ maximumFractionDigits: 2 }); return [f.format(1.005), f.format(2.675), f.format(1.255), f.format(8.345), f.format(0.615)]; });
t('M1 maxFD 2 halfEven: 1.015, 1.025, 0.125', function () { var f = nf({ maximumFractionDigits: 2, roundingMode: 'halfEven' }); return [f.format(1.015), f.format(1.025), f.format(0.125)]; });
t('M1 maxFD 0 halfTrunc: 2.5, 0.5, 1e21+0.5 (no fraction)', function () { var f = nf({ maximumFractionDigits: 0, roundingMode: 'halfTrunc' }); return [f.format(2.5), f.format(0.5), f.format(1e21)]; });
t('M1 minSD 4 (maxSD 21): 1.005', function () { return nf({ minimumSignificantDigits: 4 }).format(1.005); });
t('M1 maxSD 21: 0.1 + 0.2', function () { return nf({ maximumSignificantDigits: 21 }).format(0.1 + 0.2); });
t('M1 maxFD 20: 0.1', function () { return nf({ maximumFractionDigits: 20 }).format(0.1); });
t('M1 maxSD 21: 5e-324', function () { return nf({ maximumSignificantDigits: 21 }).format(5e-324); });
t('M1 maxSD 21: 1.7976931348623157e308 (length, first 30)', function () { var s = nf({ maximumSignificantDigits: 21, useGrouping: false }).format(1.7976931348623157e308); return [s.length, s.slice(0, 30)]; });
t('M1 default: 123456789.12345679, 1e21, 9007199254740993', function () { var f = nf(); return [f.format(123456789.12345679), f.format(1e21), f.format(9007199254740993)]; });
t('M1 toLocaleString maxFD 1: (1.15), (2.675) maxFD 2', function () { return [(1.15).toLocaleString('en', { maximumFractionDigits: 1 }), (2.675).toLocaleString('en', { maximumFractionDigits: 2 })]; });
t('M1 formatToParts maxFD 1: 1.15', function () { return parts(nf({ maximumFractionDigits: 1 }).formatToParts(1.15)); });
t('M1 lessPrecision maxSD 2 maxFD 1: 1.15', function () { return nf({ maximumSignificantDigits: 2, maximumFractionDigits: 1, roundingPriority: 'lessPrecision' }).format(1.15); });
// M1b: an object whose ToPrimitive is a String takes the String path (exact), not ToNumber.
t('M1b {valueOf: "1.15"} maxFD 1', function () { return nf({ maximumFractionDigits: 1 }).format({ valueOf: function () { return '1.15'; } }); });
t('M1b {toString: "1.15"} maxFD 1', function () { return nf({ maximumFractionDigits: 1 }).format({ toString: function () { return '1.15'; } }); });
t('M1b {valueOf: "9007199254740993"} default', function () { return nf().format({ valueOf: function () { return '9007199254740993'; } }); });
t('M1b ToPrimitive count on format({valueOf})', function () { var n = 0; nf().format({ valueOf: function () { n++; return 1; } }); return n; });
