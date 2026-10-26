// Row P4: the compact exponent (operands c and e of UTS #35 Part 3 5.1.1) in PluralRules with
// notation "compact" (ECMA-402 17.5.1, PluralRuleSelect takes the notation).
function s(loc, o, xs) { var p = new Intl.PluralRules(loc, o); return R(show(p.resolvedOptions().locale) + ' ' + xs.map(function (x) { return p.select(x); }).join(' ')); }
t('P4 en compact: 1 1000 1000000 1200000', function () { return s('en', { notation: 'compact' }, [1, 1000, 1000000, 1200000]); });
t('P4 de compact: 1 1000 1000000 1200000', function () { return s('de', { notation: 'compact' }, [1, 1000, 1000000, 1200000]); });
t('P4 fr standard: 1000000 1200000 2000', function () { return s('fr', {}, [1000000, 1200000, 2000]); });
t('P4 fr compact: 1000000 1200000 2000', function () { return s('fr', { notation: 'compact' }, [1000000, 1200000, 2000]); });
t('P4 es compact: 1000000 1200000', function () { return s('es', { notation: 'compact' }, [1000000, 1200000]); });
t('P4 it compact long: 1000000 1200000', function () { return s('it', { notation: 'compact', compactDisplay: 'long' }, [1000000, 1200000]); });
t('P4 en scientific: 1 1000000', function () { return s('en', { notation: 'scientific' }, [1, 1000000]); });
t('P4 fr NumberFormat compact long 1200000 (the plural pattern the formatter picks)', function () { return new Intl.NumberFormat('fr', { notation: 'compact', compactDisplay: 'long' }).format(1200000); });
