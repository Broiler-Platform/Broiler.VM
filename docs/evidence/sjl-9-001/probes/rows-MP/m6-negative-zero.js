// Row M8: negative zero as an Intl mathematical value (ToIntlMathematicalValue's negative-zero, and
// FormatNumericToString's sign of a value that rounds to zero).
function nf(o) { return new Intl.NumberFormat('en', o); }
t('M8 format(-0), format("-0"), format(-0.0001), format("-0.0001")', function () { var f = nf(); return [f.format(-0), f.format('-0'), f.format(-0.0001), f.format('-0.0001')]; });
t('M8 formatToParts(-0)', function () { return parts(nf().formatToParts(-0)); });
t('M8 signDisplay maxFD 0 on -0.4: auto always exceptZero negative never', function () { return ['auto', 'always', 'exceptZero', 'negative', 'never'].map(function (s) { return nf({ maximumFractionDigits: 0, signDisplay: s }).format(-0.4); }); });
t('M8 signDisplay maxFD 0 on 0.4: auto always exceptZero negative never', function () { return ['auto', 'always', 'exceptZero', 'negative', 'never'].map(function (s) { return nf({ maximumFractionDigits: 0, signDisplay: s }).format(0.4); }); });
t('M8 (-0).toLocaleString(), (-0.0001).toLocaleString("de")', function () { return [(-0).toLocaleString('en'), (-0.0001).toLocaleString('de')]; });
t('M8 PluralRules en select(-0), select("-0"), select(-1), select(-1.5)', function () { var p = new Intl.PluralRules('en'); return [p.select(-0), p.select('-0'), p.select(-1), p.select(-1.5)]; });
