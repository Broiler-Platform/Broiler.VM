// Row P6: Intl.PluralRules.prototype.resolvedOptions (ECMA-402 17.3.2, table "Resolved Options of
// PluralRules Instances"): every property, in the table's order.
function ro(o) { return new Intl.PluralRules('en', o).resolvedOptions(); }
t('P6 en {}', function () { return ro({}); });
t('P6 en ordinal', function () { return ro({ type: 'ordinal' }); });
t('P6 en compact', function () { return ro({ notation: 'compact' }); });
t('P6 en compact long', function () { return ro({ notation: 'compact', compactDisplay: 'long' }); });
t('P6 en maxSD 3', function () { return ro({ maximumSignificantDigits: 3 }); });
t('P6 en morePrecision maxSD 3', function () { return ro({ roundingPriority: 'morePrecision', maximumSignificantDigits: 3 }); });
t('P6 en increment 5 minFD 1 maxFD 1 floor stripIfInteger', function () { return ro({ roundingIncrement: 5, minimumFractionDigits: 1, maximumFractionDigits: 1, roundingMode: 'floor', trailingZeroDisplay: 'stripIfInteger' }); });
t('P6 de {} pluralCategories; de ordinal pluralCategories', function () { return [new Intl.PluralRules('de').resolvedOptions().pluralCategories, new Intl.PluralRules('de', { type: 'ordinal' }).resolvedOptions().pluralCategories]; });
t('P6 refusals: type "bogus", notation "bogus", compactDisplay "bogus", minFD 3 maxFD 2, increment 3, increment 5 maxSD 2', function () {
  var r = [];
  [{ type: 'bogus' }, { notation: 'bogus' }, { compactDisplay: 'bogus' }, { minimumFractionDigits: 3, maximumFractionDigits: 2 }, { roundingIncrement: 3 }, { roundingIncrement: 5, maximumSignificantDigits: 2 }].forEach(function (o) { try { new Intl.PluralRules('en', o); r.push('ok'); } catch (e) { r.push(errName(e)); } });
  return r;
});
t('P6 called without new', function () { return Intl.PluralRules('en'); });
t('P6 select on a non-PluralRules this', function () { return Intl.PluralRules.prototype.select.call({}, 1); });
t('P6 resolvedOptions on a non-PluralRules this', function () { return Intl.PluralRules.prototype.resolvedOptions.call(new Intl.NumberFormat('en')); });
