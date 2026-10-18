// G37-G40: ParseTemporalCalendarString, through a property bag's calendar and withCalendar: an ISO string answers
// its u-ca annotation (iso8601 without one), anything else must be an AnnotationValue and a supported calendar.
['iso8601', 'ISO8601', 'gregory', '2020-01-01', '2020-01-01[u-ca=gregory]', '2020-01-01T00:00Z[UTC][u-ca=hebrew]',
 '2020-01-01T00:00Z[u-ca=hebrew]', 'T10:00Z', '10:00Z', 'T10:00[u-ca=hebrew]', '10:00[u-ca=hebrew]', '2020-01[u-ca=gregory]',
 '2020-01[u-ca=iso8601]', '01-01[u-ca=gregory]', '--01-01', '2020-01-01[]', '2020-01-01[u-ca=gregory][UTC]',
 '2020-01-01[UTC][u-ca=gregory]', '2020-01-01[!foo=bar]', '2020-01-01[u-ca=gregory][!u-ca=japanese]', 'gregory-x', '',
 '-', 'a_b', '2020-13-01', '2020-01-01T25:00[u-ca=gregory]', '2020-01-01T12:00+25:00[u-ca=gregory]',
 '2020-01-01T12:00+01:00:00.5[u-ca=gregory]', '2020-01-01[Mars Olympus][u-ca=gregory]', '2020-02-30[u-ca=gregory]',
 '1214[u-ca=gregory]', 'T1214[u-ca=gregory]', '2020-01-01T12:3045Z[u-ca=gregory]', 'gregorian', 'minguo', 'islamic',
 'japanese ', ' japanese', 'JAPANESE', 'ethiopic-amete-alem', 'islamicc', 'islamic-civil', 'u-ca=gregory', '[u-ca=gregory]'
].forEach(function (c) {
  k('PlainDate.from({..., calendar: ' + JSON.stringify(c) + '}).calendarId', function () {
    return Temporal.PlainDate.from({ year: 2020, monthCode: 'M01', day: 1, calendar: c }).calendarId; });
  k('PlainDate#withCalendar(' + JSON.stringify(c) + ').calendarId', function () {
    return new Temporal.PlainDate(2020, 1, 1).withCalendar(c).calendarId; });
  k('new PlainDate(2020, 1, 1, ' + JSON.stringify(c) + ').calendarId', function () {
    return new Temporal.PlainDate(2020, 1, 1, c).calendarId; });
});
