// A6: offset time zone identifiers.
var names = ['+05:30', '-05:30', '+0530', '+05', '-00:00', '+00:00', '-00', '+23:59', '-23:59', '+24:00', '+05:60', '+05:30:15', '+05:30:00', '+053015', '+05:30:15.5',
  '−05:00', '+٠٥:٣٠', '+05:3', '05:30', '+5:30'];
names.forEach(function (n) {
  var l = JSON.stringify(n);
  t('ZonedDateTime ctor ' + l, function () { var z = new Temporal.ZonedDateTime(0n, n); return z.timeZoneId + ' ' + z.offset; });
  t('DateTimeFormat ' + l, function () { return new Intl.DateTimeFormat('en', { timeZone: n }).resolvedOptions().timeZone; });
});
t('Instant.toString({timeZone:"-00:00"})', function () { return Temporal.Instant.from('2020-01-01T00:00Z').toString({ timeZone: '-00:00' }); });
t('ZonedDateTime.from with [+05:30:15] annotation', function () { return Temporal.ZonedDateTime.from('2020-01-01T00:00+05:30:15[+05:30:15]').toString(); });
t('ZonedDateTime.from with offset +05:30 and no annotation', function () { return Temporal.ZonedDateTime.from('2020-01-01T00:00+05:30').toString(); });
t('toZonedDateTimeISO("2020-01-01T00:00+05:30")', function () { return Temporal.Instant.from('2020-01-01T00:00Z').toZonedDateTimeISO('2020-01-01T00:00+05:30').timeZoneId; });
t('toZonedDateTimeISO("2020-01-01T00:00+05:30:15")', function () { return Temporal.Instant.from('2020-01-01T00:00Z').toZonedDateTimeISO('2020-01-01T00:00+05:30:15').timeZoneId; });
t('DateTimeFormat +05:30 hour at epoch', function () { return new Intl.DateTimeFormat('en-US', { timeZone: '+05:30', hour: 'numeric', minute: 'numeric', hourCycle: 'h23' }).format(0); });
t('+00:00 equals UTC', function () { return new Temporal.ZonedDateTime(0n, '+00:00').equals(new Temporal.ZonedDateTime(0n, 'UTC')); });
t('+05:30 equals +0530', function () { return new Temporal.ZonedDateTime(0n, '+05:30').equals(new Temporal.ZonedDateTime(0n, '+0530')); });
t('-12:00 wall 2020-01-01T00:00 to instant', function () { return Temporal.PlainDateTime.from('2020-01-01T00:00').toZonedDateTime('-12:00').toInstant().toString(); });
