// SJL-9.1 sub-area (d), rows T24-T26: formatting in calendars other than the Gregorian one (the
// calendar's era, year, month name and numbering; the deprecated Hijri types; the resolved calendar),
// with the leftover item of bundle sjl-10-001 (the 24 Intl calendar identifiers, at the formatter's
// sites). ECMA-402 3d4c558 11.1.2 ([[ca]]), ToLocalTime ("best available information about the
// specified calendar"); the Intl era and month code proposal's CreateDateTimeFormat fallback.
var A = Date.UTC(2024, 0, 5, 15, 7, 9, 123), M = Date.UTC(2024, 4, 20, 12);
var cals = ['buddhist', 'chinese', 'coptic', 'dangi', 'ethioaa', 'ethiopic', 'gregory', 'hebrew', 'indian', 'islamic', 'islamic-civil', 'islamic-rgsa',
  'islamic-tbla', 'islamic-umalqura', 'iso8601', 'japanese', 'persian', 'roc', 'islamicc', 'ethiopic-amete-alem', 'gregorian', 'abc'];
cals.forEach(function (c) {
  t('en-u-ca-' + c + ' resolved [locale, calendar]', function () { var r = new Intl.DateTimeFormat('en-u-ca-' + c).resolvedOptions(); return [r.locale, r.calendar]; });
  t('en {calendar:' + c + '} resolved [locale, calendar]', function () { var r = new Intl.DateTimeFormat('en', { calendar: c }).resolvedOptions(); return [r.locale, r.calendar]; });
  t('en-u-ca-' + c + ' default, dateStyle long, dateStyle full, {era,year,month:long,day}', function () {
    function g(o) { var p = { timeZone: 'UTC' }; for (var k in o) p[k] = o[k]; return new Intl.DateTimeFormat('en-u-ca-' + c, p).format(A); }
    return [g({}), g({ dateStyle: 'long' }), g({ dateStyle: 'full' }), g({ era: 'short', year: 'numeric', month: 'long', day: 'numeric' })];
  });
});
['buddhist', 'chinese', 'hebrew', 'islamic-umalqura', 'japanese', 'persian', 'roc'].forEach(function (c) {
  t('de-u-ca-' + c + ' dateStyle long', function () { return new Intl.DateTimeFormat('de-u-ca-' + c, { dateStyle: 'long', timeZone: 'UTC' }).format(A); });
});
t('japanese eras: 1800, 1868-10-23, 1873, 1912-07-30, 1926-12-25, 1989-01-08, 2019-05-01', function () {
  var f = new Intl.DateTimeFormat('en-u-ca-japanese', { era: 'long', year: 'numeric', month: 'short', day: 'numeric', timeZone: 'UTC' });
  return [Date.UTC(1800, 0, 1), Date.UTC(1868, 9, 23), Date.UTC(1873, 0, 1), Date.UTC(1912, 6, 30), Date.UTC(1926, 11, 25), Date.UTC(1989, 0, 8), Date.UTC(2019, 4, 1)].map(function (x) { return f.format(x); });
});
t('chinese leap month (2023-04-20, leap 2nd month) and the next month', function () { var f = new Intl.DateTimeFormat('en-u-ca-chinese', { year: 'numeric', month: 'long', day: 'numeric', timeZone: 'UTC' }); return [f.format(Date.UTC(2023, 3, 20)), f.format(Date.UTC(2023, 4, 20))]; });
t('chinese {month:numeric} leap', function () { return new Intl.DateTimeFormat('en-u-ca-chinese', { month: 'numeric', day: 'numeric', timeZone: 'UTC' }).format(Date.UTC(2023, 3, 20)); });
t('hebrew Adar I and Adar II (5784)', function () { var f = new Intl.DateTimeFormat('en-u-ca-hebrew', { year: 'numeric', month: 'long', day: 'numeric', timeZone: 'UTC' }); return [f.format(Date.UTC(2024, 1, 15)), f.format(Date.UTC(2024, 2, 15))]; });
t('islamic-umalqura months', function () { var f = new Intl.DateTimeFormat('en-u-ca-islamic-umalqura', { month: 'long', timeZone: 'UTC' }); var r = []; for (var m = 0; m < 12; m++) r.push(f.format(Date.UTC(2024, m, 15))); return r; });
t('dangi formatToParts', function () { return parts(new Intl.DateTimeFormat('en-u-ca-dangi', { year: 'numeric', month: 'long', day: 'numeric', timeZone: 'UTC' }).formatToParts(A)); });
t('roc before 1912', function () { return new Intl.DateTimeFormat('en-u-ca-roc', { era: 'short', year: 'numeric', timeZone: 'UTC' }).format(Date.UTC(1900, 0, 1)); });
t('buddhist formatToParts', function () { return parts(new Intl.DateTimeFormat('en-u-ca-buddhist', { year: 'numeric', month: 'short', timeZone: 'UTC' }).formatToParts(A)); });
t('iso8601 dateStyle full and default', function () { return [new Intl.DateTimeFormat('en-u-ca-iso8601', { dateStyle: 'full', timeZone: 'UTC' }).format(A), new Intl.DateTimeFormat('en-u-ca-iso8601', { timeZone: 'UTC' }).format(A)]; });
t('persian at year 1 and negative', function () { var f = new Intl.DateTimeFormat('en-u-ca-persian', { era: 'short', year: 'numeric', month: 'short', day: 'numeric', timeZone: 'UTC' }); return [f.format(Date.UTC(622, 2, 22)), f.format(Date.UTC(600, 0, 1))]; });
t('coptic and ethiopic eras before their epochs', function () { return ['coptic', 'ethiopic', 'ethioaa'].map(function (c) { return new Intl.DateTimeFormat('en-u-ca-' + c, { era: 'short', year: 'numeric', timeZone: 'UTC' }).format(Date.UTC(100, 0, 1)); }); });
t('Date#toLocaleDateString en-u-ca-japanese', function () { return new Date(M).toLocaleDateString('en-u-ca-japanese', { timeZone: 'UTC' }); });
