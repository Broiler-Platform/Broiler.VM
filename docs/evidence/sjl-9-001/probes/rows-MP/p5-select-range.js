// Row P5: Intl.PluralRules.prototype.selectRange (ECMA-402 17.3.4, ResolvePluralRange 17.5.4): the start's
// category when both ends format alike, otherwise PluralRuleSelectRange (implementation-dependent, or "other").
var en = new Intl.PluralRules('en'), de = new Intl.PluralRules('de');
t('P5 en (1,1) (1,1.0001) (-1,1) ("1","1.0")', function () { return [en.selectRange(1, 1), en.selectRange(1, 1.0001), en.selectRange(-1, 1), en.selectRange('1', '1.0')]; });
t('P5 de (1,1); en {minFD 1} (1,1)', function () { return [de.selectRange(1, 1), new Intl.PluralRules('en', { minimumFractionDigits: 1 }).selectRange(1, 1)]; });
t('P5 en (0,1) (1,2) (2,1) (0,0.5) (1,1.5)', function () { return [en.selectRange(0, 1), en.selectRange(1, 2), en.selectRange(2, 1), en.selectRange(0, 0.5), en.selectRange(1, 1.5)]; });
t('P5 de (0,1) (1,2) (2,1) (1,1.5)', function () { return [de.selectRange(0, 1), de.selectRange(1, 2), de.selectRange(2, 1), de.selectRange(1, 1.5)]; });
t('P5 en ordinal (1,2) (1,1) (2,3)', function () { var o = new Intl.PluralRules('en', { type: 'ordinal' }); return [o.selectRange(1, 2), o.selectRange(1, 1), o.selectRange(2, 3)]; });
t('P5 fr (0,1) (1,2)', function () { var f = new Intl.PluralRules('fr'); return [f.resolvedOptions().locale, f.selectRange(0, 1), f.selectRange(1, 2)]; });
t('P5 (Infinity, Infinity), (5, 1) no throw, (-Infinity, Infinity)', function () { return [en.selectRange(Infinity, Infinity), en.selectRange(5, 1), en.selectRange(-Infinity, Infinity)]; });
t('P5 (NaN, 1)', function () { return en.selectRange(NaN, 1); });
t('P5 (undefined, 1)', function () { return en.selectRange(undefined, 1); });
t('P5 (1) one argument', function () { return en.selectRange(1); });
t('P5 order of coercion: start then end, both before the NaN test', function () { var log = []; try { en.selectRange({ valueOf: function () { log.push('start'); return NaN; } }, { valueOf: function () { log.push('end'); return 1; } }); } catch (e) { log.push(errName(e)); } return log; });
