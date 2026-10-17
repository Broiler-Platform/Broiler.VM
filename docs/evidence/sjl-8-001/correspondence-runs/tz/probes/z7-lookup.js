// Z7: identifier lookup: ASCII case, non-ASCII characters that invariant upper-casing folds, and junk.
var names = ['Europe/Berlin', 'europe/berlin', 'EUROPE/BERLIN', 'eUrOpE/bErLiN',
  'Europe/Berlın', 'America/Loſ_Angeles', 'Europe/Kıev', 'Europe/Berlin ', ' Europe/Berlin', 'Europe/Berlin\u0000',
  'Europe//Berlin', 'Europe/Berlin/', 'utc', 'Utc', 'etc/utc', 'gmt', 'UT', 'Z', 'z', 'İstanbul', 'Europe/İstanbul', 'Europe/Istanbul'];
names.forEach(function (n) {
  var label = JSON.stringify(n);
  t('ZonedDateTime ctor ' + label, function () { return new Temporal.ZonedDateTime(0n, n).timeZoneId; });
  t('DateTimeFormat ' + label, function () { return new Intl.DateTimeFormat('en', { timeZone: n }).resolvedOptions().timeZone; });
});
t('ZonedDateTime.from annotation [Europe/Berl\\u0131n]', function () { return Temporal.ZonedDateTime.from('2020-01-01T00:00[Europe/Berlın]').timeZoneId; });
t('ZonedDateTime.from annotation [EUROPE/BERLIN]', function () { return Temporal.ZonedDateTime.from('2020-01-01T00:00[EUROPE/BERLIN]').timeZoneId; });
t('Instant.toZonedDateTimeISO("America/Lo\\u017F_Angeles")', function () { return Temporal.Instant.from('2020-01-01T00:00Z').toZonedDateTimeISO('America/Loſ_Angeles').toString(); });
t('ISO string as Temporal zone "2020-01-01T00:00[Europe/Berlin]"', function () { return Temporal.Instant.from('2020-01-01T00:00Z').toZonedDateTimeISO('2020-01-01T00:00[Europe/Berlin]').timeZoneId; });
t('ISO string as DateTimeFormat timeZone "2020-01-01T00:00[Europe/Berlin]"', function () { return new Intl.DateTimeFormat('en', { timeZone: '2020-01-01T00:00[Europe/Berlin]' }).resolvedOptions().timeZone; });
t('ISO string as DateTimeFormat timeZone "2020-01-01T00:00Z"', function () { return new Intl.DateTimeFormat('en', { timeZone: '2020-01-01T00:00Z' }).resolvedOptions().timeZone; });
t('ISO string as ZonedDateTime ctor zone "2020-01-01T00:00[Europe/Berlin]"', function () { return new Temporal.ZonedDateTime(0n, '2020-01-01T00:00[Europe/Berlin]').timeZoneId; });
