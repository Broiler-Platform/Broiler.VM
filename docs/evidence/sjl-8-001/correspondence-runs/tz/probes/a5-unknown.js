// A5: unknown zone names on every path that takes a zone.
var names = ['Mars/Olympus', 'Etc/Unknown', '', 'America/New York', 'Europe/Berlin ', 'EST5', 'Etc/GMT+13', 'Etc/GMT-15', 'Etc/GMT+05'];
names.forEach(function (n) {
  var l = JSON.stringify(n);
  t('ZonedDateTime ctor ' + l, function () { return new Temporal.ZonedDateTime(0n, n).toString(); });
  t('ZonedDateTime.from annotation ' + l, function () { return Temporal.ZonedDateTime.from('2020-01-01T00:00[' + n + ']').toString(); });
  t('Instant.toZonedDateTimeISO ' + l, function () { return Temporal.Instant.from('2020-01-01T00:00Z').toZonedDateTimeISO(n).toString(); });
  t('Instant.toString timeZone ' + l, function () { return Temporal.Instant.from('2020-01-01T00:00Z').toString({ timeZone: n }); });
  t('PlainDate.toZonedDateTime ' + l, function () { return Temporal.PlainDate.from('2020-01-01').toZonedDateTime(n).toString(); });
  t('Now.zonedDateTimeISO ' + l, function () { return typeof Temporal.Now.zonedDateTimeISO(n); });
  t('Duration.round relativeTo bag ' + l, function () { return Temporal.Duration.from({ hours: 25 }).round({ largestUnit: 'day', relativeTo: { year: 2020, month: 1, day: 1, timeZone: n } }).toString(); });
  t('DateTimeFormat ' + l, function () { return new Intl.DateTimeFormat('en', { timeZone: n }).resolvedOptions().timeZone; });
});
