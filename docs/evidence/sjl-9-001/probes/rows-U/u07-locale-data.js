// Rows U13-U16: what the locale's data decides: the clock's separators and two-digit hours ([[DigitalFormat]]), the
// unit and list patterns, the numbering system's digits and symbols, and the resolved locale.
function f(row, loc, opts, d) {
  t(row + ' ' + loc + ' ' + JSON.stringify(opts) + ' ' + JSON.stringify(d), function () {
    var df = new Intl.DurationFormat(loc, opts);
    return df.resolvedOptions().locale + ' ' + df.resolvedOptions().numberingSystem + ' ' + q(df.format(d));
  });
}
['en', 'de', 'da', 'id', 'si', 'en-DK', 'fi', 'af', 'lt', 'es-CL', 'sr', 'be'].forEach(function (l) { f('U13-U14', l, { style: 'digital' }, { hours: 1, minutes: 2, seconds: 3 }); });
f('U13', 'da', { style: 'long', minutes: 'numeric' }, { hours: 1, minutes: 2, seconds: 3 });
['fr', 'ja', 'es', 'ar', 'zh-TW'].forEach(function (l) { f('U15', l, { style: 'long' }, { hours: 1, minutes: 2 }); });
f('U15', 'de', { style: 'short' }, { nanoseconds: 1 });
f('U15', 'de', { style: 'short' }, { milliseconds: 1, microseconds: 1 });
f('U15', 'de', { style: 'narrow' }, { hours: 1, milliseconds: 1 });
f('U15', 'de', { style: 'narrow' }, { hours: 2, milliseconds: 2 });
f('U16', 'en', { style: 'short', numberingSystem: 'arab' }, { hours: -1, minutes: -2, seconds: -3 });
f('U16', 'en', { style: 'short', numberingSystem: 'arab' }, { days: 1000 });
f('U16', 'en', { style: 'digital', numberingSystem: 'arab' }, { hours: 1, minutes: 2, seconds: 3, milliseconds: 500 });
f('U16', 'de', { style: 'short', numberingSystem: 'arab' }, { years: -1, months: -2 });
f('U16', 'en-u-nu-arab', { style: 'short' }, { hours: 1 });
f('U16', 'en-u-nu-arab', { style: 'short', numberingSystem: 'latn' }, { hours: 1 });
f('U16', 'en-u-ca-gregory-nu-latn', {}, { hours: 1 });
f('U16', 'ar-EG', { style: 'short' }, { hours: 1, minutes: 2 });
f('U16', 'fa', { style: 'digital' }, { hours: 1, minutes: 2, seconds: 3 });
f('U16', 'en', { style: 'short', numberingSystem: 'abcd' }, { hours: 1 });
t('U15 supportedLocalesOf([fr, de-AT, da, en])', function () { return Intl.DurationFormat.supportedLocalesOf(['fr', 'de-AT', 'da', 'en']); });
// The number formats a duration builds, alone (sub-area c's rows): a unit of ar, and en's arab digits and symbols.
t('U15 NumberFormat ar unit hour long 1, 2', function () { var nf = new Intl.NumberFormat('ar', { style: 'unit', unit: 'hour', unitDisplay: 'long' }); return [nf.resolvedOptions().locale, nf.format(1), nf.format(2)]; });
t('U15 ListFormat ar unit long ["x", "y"]', function () { return new Intl.ListFormat('ar', { type: 'unit', style: 'long' }).format(['x', 'y']); });
t('U16 NumberFormat en arab -1234.5', function () { return new Intl.NumberFormat('en', { numberingSystem: 'arab' }).format(-1234.5); });
