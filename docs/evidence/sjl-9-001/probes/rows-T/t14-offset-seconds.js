// SJL-9.1 sub-area (d), row T25: the localized GMT format of an offset that is not a whole number of minutes
// (a local mean time's, before a zone's first standard time), the leftover item of Broiler.JS's bundle
// sjl-4-001 (README lines 267-268 and 278: GmtOffset rounding an offset to whole minutes, ties to even, so
// Monrovia's -0:44:30 before 1972 was named -0:44), in each of the four offset-bearing styles. ECMA-402 3d4c558
// FormatDateTimePattern ("timeZoneName": an implementation, locale and time-zone dependent name), UTS #35's
// localized GMT format.
var cases = [
  ['Africa/Monrovia', Date.UTC(1970, 0, 1, 12)],
  ['Africa/Monrovia', Date.UTC(1971, 11, 31, 12)],
  ['Europe/Amsterdam', Date.UTC(1900, 0, 1, 12)],
  ['America/New_York', Date.UTC(1880, 0, 1, 12)],
  ['Asia/Kolkata', Date.UTC(1900, 0, 1, 12)],
  ['Europe/Dublin', Date.UTC(1900, 0, 1, 12)],
  ['America/Sao_Paulo', Date.UTC(1900, 0, 1, 12)]
];
cases.forEach(function (c) {
  t(c[0] + ' at ' + c[1] + ' [short, long, shortOffset, longOffset]', function () {
    return ['short', 'long', 'shortOffset', 'longOffset'].map(function (s) {
      return new Intl.DateTimeFormat('en', { timeZone: c[0], timeZoneName: s, hour: 'numeric', minute: 'numeric', second: 'numeric', hourCycle: 'h23' }).format(c[1]);
    });
  });
});
t('Africa/Monrovia 1970 formatToParts timeZoneName shortOffset', function () {
  return parts(new Intl.DateTimeFormat('en', { timeZone: 'Africa/Monrovia', timeZoneName: 'shortOffset' }).formatToParts(Date.UTC(1970, 0, 1, 12)));
});
t('de Africa/Monrovia 1970 [shortOffset, longOffset]', function () {
  return ['shortOffset', 'longOffset'].map(function (s) { return new Intl.DateTimeFormat('de', { timeZone: 'Africa/Monrovia', timeZoneName: s }).format(Date.UTC(1970, 0, 1, 12)); });
});
