// G49-G52: cases the matrices above left open: a time on a year-month or a month-day, an invalid date under a time
// goal, which of several u-ca annotations a calendar string answers, and the order the annotations are checked in.
['2020-01T12:00', '202001T1200', '2020-01 12:00', '2020-01T12:00Z', '2020-01T12:00[u-ca=iso8601]', '2020-01T25:00'].forEach(function (s) {
  each(['PlainYearMonth'], s); });
['01-01T12:00', '0101T12:00', '--01-01T12:00'].forEach(function (s) { each(['PlainMonthDay'], s); });
['2020-02-30T12:00', '2021-02-29T12:00', '2020-00-01T12:00', '2020-01-32T12:00', '-000000-01-01T12:00', '2020-0101T12:00'
].forEach(function (s) { each(['PlainTime'], s); });
// Which calendar a calendar string with several u-ca annotations answers.
['2020-01-01[u-ca=gregory][u-ca=japanese]', '2020-01-01[u-ca=japanese][u-ca=gregory]', '2020-01-01T00:00Z[UTC][u-ca=hebrew][u-ca=coptic]',
 '2020-01-01[u-ca=gregory][u-ca=bogus]', '2020-01-01[u-ca=bogus][u-ca=gregory]'].forEach(function (c) {
  t('withCalendar(' + JSON.stringify(c) + ')', function () { return new Temporal.PlainDate(2020, 1, 1).withCalendar(c).calendarId; });
  t('PlainDate.from(' + JSON.stringify(c) + ')', function () { return Temporal.PlainDate.from(c).calendarId; });
});
// Two time zone annotations or a time zone annotation after a calendar, through the time zone string parser.
['2020-01-01T00:00Z[Europe/Berlin][UTC]', '2020-01-01T00:00Z[u-ca=iso8601][Europe/Berlin]', '2020-01-01T00:00Z[Europe/Berlin][u-ca=iso8601]'
].forEach(function (z) {
  k('toZonedDateTimeISO(' + JSON.stringify(z) + ')', function () { return Temporal.Instant.from('2020-01-01T00:00Z').toZonedDateTimeISO(z).timeZoneId; }); });
// A month-day or year-month string in a calendar other than ISO, the reference date the string's date gives.
['2020-01-15[u-ca=hebrew]', '2020-03-01[u-ca=chinese]', '2023-03-22[u-ca=chinese]', '2020-01-15[u-ca=persian]', '2019-09-11[u-ca=coptic]'
].forEach(function (s) {
  t('PlainMonthDay.from(' + JSON.stringify(s) + ')', function () { return Temporal.PlainMonthDay.from(s).toString(); });
  t('PlainYearMonth.from(' + JSON.stringify(s) + ')', function () { return Temporal.PlainYearMonth.from(s).toString(); });
});
