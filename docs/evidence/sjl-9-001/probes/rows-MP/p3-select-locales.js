// Row P3: which languages' plural rules each engine holds. Each line: the resolved locale, then the
// categories select gives.
function c(loc, xs, o) { var p = new Intl.PluralRules(loc, o); return R(show(p.resolvedOptions().locale) + ' ' + xs.map(function (x) { return p.select(x); }).join(' ')); }
t('P3 fr 0 1 1.5 2 1000000', function () { return c('fr', [0, 1, 1.5, 2, 1000000]); });
t('P3 ru 1 3 5 21 1.5', function () { return c('ru', [1, 3, 5, 21, 1.5]); });
t('P3 pl 1 2 5 22 1.5', function () { return c('pl', [1, 2, 5, 22, 1.5]); });
t('P3 cs 1 3 5 1.5', function () { return c('cs', [1, 3, 5, 1.5]); });
t('P3 uk 1 3 5', function () { return c('uk', [1, 3, 5]); });
t('P3 hi 0 1 2', function () { return c('hi', [0, 1, 2]); });
t('P3 he 1 2 3', function () { return c('he', [1, 2, 3]); });
t('P3 ar 0 1 2 3 11 100', function () { return c('ar', [0, 1, 2, 3, 11, 100]); });
t('P3 cy 0 1 2 3 6', function () { return c('cy', [0, 1, 2, 3, 6]); });
t('P3 ja 0 1 2', function () { return c('ja', [0, 1, 2]); });
t('P3 pt 0 1 1.5; pt-PT 0 1 1.5', function () { return [c('pt', [0, 1, 1.5]), c('pt-PT', [0, 1, 1.5])]; });
t('P3 fr ordinal 1 2; sv ordinal 1 2 3', function () { return [c('fr', [1, 2], { type: 'ordinal' }), c('sv', [1, 2, 3], { type: 'ordinal' })]; });
t('P3 pluralCategories: fr, ru, ar, ja, de ordinal, en ordinal', function () { return [new Intl.PluralRules('fr').resolvedOptions().pluralCategories, new Intl.PluralRules('ru').resolvedOptions().pluralCategories, new Intl.PluralRules('ar').resolvedOptions().pluralCategories, new Intl.PluralRules('ja').resolvedOptions().pluralCategories, new Intl.PluralRules('de', { type: 'ordinal' }).resolvedOptions().pluralCategories, new Intl.PluralRules('en', { type: 'ordinal' }).resolvedOptions().pluralCategories]; });
t('P3 supportedLocalesOf fr ru de en-GB zz', function () { return Intl.PluralRules.supportedLocalesOf(['fr', 'ru', 'de', 'en-GB', 'zz']); });
