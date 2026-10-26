// Row P2: select and selectRange read their arguments with ToIntlMathematicalValue (ECMA-402 17.3.3,
// 17.3.4), so a BigInt is a value, not a TypeError.
var p = new Intl.PluralRules('en');
t('P2 en select(1n), select(2n), select(0n), select(-1n)', function () { return [p.select(1n), p.select(2n), p.select(0n), p.select(-1n)]; });
t('P2 en ordinal select(2n), select(3n)', function () { var o = new Intl.PluralRules('en', { type: 'ordinal' }); return [o.select(2n), o.select(3n)]; });
t('P2 en select(10n**30n)', function () { return p.select(10n ** 30n); });
t('P2 en {minFD 1}.select(1n)', function () { return new Intl.PluralRules('en', { minimumFractionDigits: 1 }).select(1n); });
t('P2 en select(Object(1n))', function () { return p.select(Object(1n)); });
t('P2 de selectRange(0n, 1n); en selectRange(1n, 1n)', function () { return [new Intl.PluralRules('de').selectRange(0n, 1n), p.selectRange(1n, 1n)]; });
