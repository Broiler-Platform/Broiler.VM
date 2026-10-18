// S17-S22: ZonedDateTime's offset and timeZoneName options and its calendar annotation; offsets of zones with
// sub-minute offsets, rounded to the minute in strings; Instant.toString's timeZone.
var z = function () { return Temporal.ZonedDateTime.from('2020-07-01T12:00:00.5+02:00[Europe/Berlin][u-ca=japanese]'); };
['auto', 'never', 'critical', undefined, 'x', 'AUTO', null].forEach(function (tz) {
  ['auto', 'never', undefined, 'x', 'critical'].forEach(function (off) {
    k('ZonedDateTime timeZoneName ' + JSON.stringify(tz) + ' offset ' + JSON.stringify(off), function () { return z().toString({ timeZoneName: tz, offset: off }); });
  });
});
['auto', 'always', 'never', 'critical'].forEach(function (cn) {
  k('ZonedDateTime iso8601 calendarName ' + cn + ' timeZoneName critical', function () {
    return Temporal.ZonedDateTime.from('2020-07-01T12:00[Europe/Berlin]').toString({ calendarName: cn, timeZoneName: 'critical' }); });
});
// Sub-minute offsets: Monrovia's -00:44:30 (1970), Amsterdam's +00:19:32 (1930) and +00:20 (1938), Dublin 1900.
[['1970-01-01T00:00Z', 'Africa/Monrovia'], ['1930-06-01T00:00Z', 'Europe/Amsterdam'], ['1938-06-01T00:00Z', 'Europe/Amsterdam'],
 ['1900-01-01T00:00Z', 'Europe/Dublin'], ['1850-01-01T00:00Z', 'America/New_York'], ['1850-01-01T00:00Z', 'Asia/Kolkata'],
 ['1900-01-01T00:00Z', 'Asia/Kolkata'], ['1850-01-01T00:00Z', 'Africa/Abidjan']].forEach(function (c) {
  t('ZonedDateTime ' + c[1] + ' at ' + c[0], function () { var d = Temporal.Instant.from(c[0]).toZonedDateTimeISO(c[1]); return d.toString() + ' offset ' + d.offset + ' ' + d.offsetNanoseconds; });
  t('Instant ' + c[0] + ' toString({timeZone: ' + c[1] + '})', function () { return Temporal.Instant.from(c[0]).toString({ timeZone: c[1] }); });
  t('round trip of ' + c[1] + ' at ' + c[0], function () { var s = Temporal.Instant.from(c[0]).toZonedDateTimeISO(c[1]).toString(); return Temporal.ZonedDateTime.from(s).epochNanoseconds; });
});
// Instant.toString's timeZone values, and its rounding with a zone.
['UTC', '+01:00', '-00:00', '+05:30', 'Asia/Kolkata', 'Europe/Berlin', 'utc', '+0530', '2020-01-01T00:00+01:00', 'Mars/Olympus', '+01:00:01', null, 0].forEach(function (zone) {
  k('Instant toString({timeZone: ' + JSON.stringify(zone) + '})', function () { return Temporal.Instant.from('2020-07-01T12:00:00.123Z').toString({ timeZone: zone }); });
});
k('Instant toString({timeZone: Asia/Kolkata, smallestUnit: hour})', function () { return Temporal.Instant.from('2020-07-01T12:00Z').toString({ timeZone: 'Asia/Kolkata', smallestUnit: 'hour' }); });
k('Instant toString({timeZone: Asia/Kolkata, smallestUnit: minute})', function () { return Temporal.Instant.from('2020-07-01T12:00:31Z').toString({ timeZone: 'Asia/Kolkata', smallestUnit: 'minute' }); });
k('ZonedDateTime toString rounding to the minute across a transition', function () {
  return Temporal.ZonedDateTime.from('2020-03-29T00:59:59.9+00:00[Europe/London]').toString({ smallestUnit: 'minute', roundingMode: 'ceil' }); });
k('ZonedDateTime toString rounding to the second in a fold', function () {
  return Temporal.ZonedDateTime.from('2020-10-25T01:59:59.9+01:00[Europe/London]').toString({ smallestUnit: 'second', roundingMode: 'ceil' }); });
k('ZonedDateTime offset getter, Monrovia 1970', function () { return Temporal.Instant.from('1970-01-01T00:00Z').toZonedDateTimeISO('Africa/Monrovia').offset; });
