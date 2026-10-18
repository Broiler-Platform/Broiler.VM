// G41-G44: ParseTemporalTimeZoneString, through toZonedDateTimeISO, a property bag's timeZone and the constructor: an
// identifier, or the zone an ISO string names (its annotation, else Z as UTC, else its offset, to the minute).
['UTC', '2020-01-01T00:00Z', '2020-01-01T00:00z', '2020-01-01T00:00+01:00', '2020-01-01T00:00+01:00:01',
 '2020-01-01T00:00+01:00:00', '2020-01-01[Europe/Berlin]', '2020-01-01T00:00Z[Europe/Berlin]', 'T10:00Z', '10:00Z',
 '10:00+01:00', 'T10:00+01:00', '10:00[Europe/Berlin]', '2020-01-01', '2020-01-01T00:00', '2020-01-01T00:00[u-ca=gregory]',
 '2020-01[Europe/Berlin]', '01-01[Europe/Berlin]', '2020-01-01[]', '2020-01-01T00:00+25:00', '2020-01-01T00:00+23:60',
 '2020-01-01[u-ca=gregory][Europe/Berlin]', '2020-01-01[foo=bar][Europe/Berlin]', '+01:00', '+0100', '+01', '-00:00',
 '+01:00:00', '+01:00:00.5', '+24:00', '2020-01-01T00:00+01:00[+02:00]', '2020-01-01T00:00Z[+02:00]',
 '2020-01-01T12:3045Z', '2021-12', '1214Z', '2020-01-01T00:00Z[!Europe/Berlin]', '2020-01-01T00:00Z[u-ca=gregory][!foo=bar]',
 '2020-01-01T00:00-00:00', '2020-01-01T00:00-00'
].forEach(function (z) {
  k('Instant#toZonedDateTimeISO(' + JSON.stringify(z) + ').timeZoneId', function () {
    return Temporal.Instant.from('2020-01-01T00:00Z').toZonedDateTimeISO(z).timeZoneId; });
  k('ZonedDateTime.from({..., timeZone: ' + JSON.stringify(z) + '}).timeZoneId', function () {
    return Temporal.ZonedDateTime.from({ year: 2020, month: 1, day: 1, timeZone: z }).timeZoneId; });
  k('new ZonedDateTime(0n, ' + JSON.stringify(z) + ').timeZoneId', function () {
    return new Temporal.ZonedDateTime(0n, z).timeZoneId; });
});
