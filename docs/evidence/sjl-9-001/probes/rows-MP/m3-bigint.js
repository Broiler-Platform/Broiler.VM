// Row M3: ToIntlMathematicalValue of a BigInt: its exact value, in every notation.
function f(o) { return new Intl.NumberFormat('en', o); }
t('M3 default 12345678901234567890123n', function () { return f().format(12345678901234567890123n); });
t('M3 scientific maxSD 21: 123456789012345678901n', function () { return f({ notation: 'scientific', maximumSignificantDigits: 21 }).format(123456789012345678901n); });
t('M3 engineering maxSD 21: 123456789012345678901n', function () { return f({ notation: 'engineering', maximumSignificantDigits: 21 }).format(123456789012345678901n); });
t('M3 scientific default: 123456789012345678901n', function () { return f({ notation: 'scientific' }).format(123456789012345678901n); });
t('M3 maxSD 3: 12345n; -5n; 0n', function () { var g = f({ maximumSignificantDigits: 3 }); return [g.format(12345n), g.format(-5n), g.format(0n)]; });
t('M3 percent: 1n', function () { return f({ style: 'percent' }).format(1n); });
t('M3 10n**30n formatToParts', function () { return parts(f().formatToParts(10n ** 30n)); });
t('M3 10n**400n (length, first, last)', function () { var s = f({ useGrouping: false }).format(10n ** 400n); return [s.length, s.slice(0, 3), s.slice(-3)]; });
t('M3 (2n**64n+1n).toLocaleString en', function () { return (2n ** 64n + 1n).toLocaleString('en'); });
t('M3 BigInt toLocaleString maxSD 21 scientific 123456789012345678901n', function () { return (123456789012345678901n).toLocaleString('en', { notation: 'scientific', maximumSignificantDigits: 21 }); });
t('M3 Object(5n) ToPrimitive', function () { return f().format(Object(5n)); });
t('M3 10n**308n and 10n**309n and -(10n**400n) (lengths)', function () { var g = f({ useGrouping: false }); return [g.format(10n ** 308n).length, g.format(10n ** 309n).length, g.format(-(10n ** 400n)).length]; });
t('M3 PluralRules en select(10n**400n)', function () { return new Intl.PluralRules('en').select(10n ** 400n); });
