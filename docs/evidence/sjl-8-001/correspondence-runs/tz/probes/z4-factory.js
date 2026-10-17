// Z4: the Factory zone.
t('new Temporal.ZonedDateTime(0n, "Factory")', function () { return new Temporal.ZonedDateTime(0n, 'Factory').toString(); });
t('new Temporal.ZonedDateTime(0n, "factory").timeZoneId', function () { return new Temporal.ZonedDateTime(0n, 'factory').timeZoneId; });
t('Intl.DateTimeFormat timeZone Factory resolved', function () { return new Intl.DateTimeFormat('en', { timeZone: 'Factory' }).resolvedOptions().timeZone; });
t('Intl.supportedValuesOf("timeZone") includes Factory', function () { return Intl.supportedValuesOf('timeZone').indexOf('Factory') >= 0; });
