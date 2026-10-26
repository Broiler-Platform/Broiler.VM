// SJL-9.1 sub-area (d), row T27: the numbering system of a date format: the digits of each numeric
// field, the fractional-second separator, and the resolved numberingSystem. FormatDateTimePattern
// (e8cc03fc) formats every number through an Intl.NumberFormat with the format's numberingSystem and
// useGrouping false.
var A = Date.UTC(2024, 0, 5, 15, 7, 9, 123);
['arab', 'arabext', 'beng', 'deva', 'hanidec', 'thai', 'fullwide', 'latn', 'tamldec'].forEach(function (nu) {
  t('en-u-nu-' + nu + ' [numberingSystem, default, {hour,minute,second,fractionalSecondDigits:3}, dateStyle medium]', function () {
    var o = { timeZone: 'UTC' };
    var f1 = new Intl.DateTimeFormat('en-u-nu-' + nu, o);
    var f2 = new Intl.DateTimeFormat('en-u-nu-' + nu, { hour: 'numeric', minute: '2-digit', second: '2-digit', fractionalSecondDigits: 3, hourCycle: 'h23', timeZone: 'UTC' });
    var f3 = new Intl.DateTimeFormat('en-u-nu-' + nu, { dateStyle: 'medium', timeZone: 'UTC' });
    return [f1.resolvedOptions().numberingSystem, f1.format(A), f2.format(A), f3.format(A)];
  });
});
t('de {numberingSystem:arab} default and fractional second', function () { return [new Intl.DateTimeFormat('de', { numberingSystem: 'arab', timeZone: 'UTC' }).format(A), new Intl.DateTimeFormat('de', { numberingSystem: 'arab', second: 'numeric', fractionalSecondDigits: 2, timeZone: 'UTC' }).format(A)]; });
t('en {numberingSystem:hanidec} year 2024 and 10000', function () { var f = new Intl.DateTimeFormat('en', { numberingSystem: 'hanidec', year: 'numeric', timeZone: 'UTC' }); var d = new Date(0); d.setUTCFullYear(10000); return [f.format(A), f.format(d)]; });
t('en-u-nu-arab {timeZoneName:shortOffset} in Asia/Kolkata', function () { return new Intl.DateTimeFormat('en-u-nu-arab', { hour: 'numeric', timeZoneName: 'shortOffset', timeZone: 'Asia/Kolkata' }).format(A); });
t('en-u-nu-arab formatToParts', function () { return parts(new Intl.DateTimeFormat('en-u-nu-arab', { year: 'numeric', month: 'numeric', day: 'numeric', timeZone: 'UTC' }).formatToParts(A)); });
t('ar-EG default numberingSystem', function () { return new Intl.DateTimeFormat('ar-EG').resolvedOptions().numberingSystem; });
t('en {numberingSystem:"arab"} with -u-nu-latn', function () { var r = new Intl.DateTimeFormat('en-u-nu-latn', { numberingSystem: 'arab' }).resolvedOptions(); return [r.locale, r.numberingSystem]; });
