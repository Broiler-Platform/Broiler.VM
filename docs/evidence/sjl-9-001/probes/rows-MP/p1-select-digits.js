// Row P1: Intl.PluralRules.prototype.select resolves over FormatNumericToString's string (ResolvePlural,
// ECMA-402 17.5.2), so the digit options and the rounding decide the operands.
function pr(o) { return new Intl.PluralRules('en', o); }
t('P1 en {minFD 1}.select(1); de {minFD 1}.select(1)', function () { return [pr({ minimumFractionDigits: 1 }).select(1), new Intl.PluralRules('de', { minimumFractionDigits: 1 }).select(1)]; });
t('P1 en {maxFD 0}.select(1.4); select(0.6)', function () { var p = pr({ maximumFractionDigits: 0 }); return [p.select(1.4), p.select(0.6)]; });
t('P1 en default (maxFD 3): select(1.0001), select(0.9999), select(1.0005), select(1.5)', function () { var p = pr(); return [p.select(1.0001), p.select(0.9999), p.select(1.0005), p.select(1.5)]; });
t('P1 en {maxSD 1}.select(1.5); {minSD 2}.select(1)', function () { return [pr({ maximumSignificantDigits: 1 }).select(1.5), pr({ minimumSignificantDigits: 2 }).select(1)]; });
t('P1 en {maxFD 0, roundingMode floor}.select(1.9); {maxFD 0, roundingMode ceil}.select(0.1)', function () { return [pr({ maximumFractionDigits: 0, roundingMode: 'floor' }).select(1.9), pr({ maximumFractionDigits: 0, roundingMode: 'ceil' }).select(0.1)]; });
t('P1 en {minFD 1, trailingZeroDisplay stripIfInteger}.select(1)', function () { return pr({ minimumFractionDigits: 1, trailingZeroDisplay: 'stripIfInteger' }).select(1); });
t('P1 en {minimumIntegerDigits 2}.select(1)', function () { return pr({ minimumIntegerDigits: 2 }).select(1); });
t('P1 en {roundingIncrement 5, minFD 1, maxFD 1}.select(0.98)', function () { return pr({ roundingIncrement: 5, minimumFractionDigits: 1, maximumFractionDigits: 1 }).select(0.98); });
t('P1 en select("1.0"), select(" 1 "), select("1.50"), select("abc")', function () { var p = pr(); return [p.select('1.0'), p.select(' 1 '), p.select('1.50'), p.select('abc')]; });
t('P1 en select({valueOf: "1"}), ToPrimitive count', function () { var n = 0; var r = pr().select({ valueOf: function () { n++; return '1'; } }); return [r, n]; });
t('P1 en ordinal select 1 2 3 4 11 12 13 21 22 23 101 111 1.5', function () { var p = pr({ type: 'ordinal' }); return [1, 2, 3, 4, 11, 12, 13, 21, 22, 23, 101, 111, 1.5].map(function (x) { return p.select(x); }).join(' '); });
t('P1 en ordinal {minFD 1}.select(1); {maxFD 0}.select(1.4)', function () { return [pr({ type: 'ordinal', minimumFractionDigits: 1 }).select(1), pr({ type: 'ordinal', maximumFractionDigits: 0 }).select(1.4)]; });
t('P1 de ordinal select(1); de select 0 1 2 1.5', function () { var o = new Intl.PluralRules('de', { type: 'ordinal' }); var c = new Intl.PluralRules('de'); return [o.select(1), [0, 1, 2, 1.5].map(function (x) { return c.select(x); }).join(' ')]; });
t('P1 en select NaN Infinity -Infinity 1e21 -1 undefined', function () { var p = pr(); return [p.select(NaN), p.select(Infinity), p.select(-Infinity), p.select(1e21), p.select(-1), p.select(undefined)]; });
