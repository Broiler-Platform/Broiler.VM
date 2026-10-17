// A7: the Etc/GMT zones.
['Etc/GMT+5', 'etc/gmt+5', 'ETC/GMT+5', 'Etc/GMT-14', 'Etc/GMT+12', 'Etc/GMT-0', 'Etc/GMT0', 'Etc/GMT+0', 'GMT+0', 'GMT-0', 'GMT0', 'Etc/GMT'].forEach(function (n) {
  t('ZonedDateTime ctor ' + n, function () { var z = new Temporal.ZonedDateTime(0n, n); return z.timeZoneId + ' ' + z.offset; });
  t('DateTimeFormat ' + n, function () { return new Intl.DateTimeFormat('en', { timeZone: n }).resolvedOptions().timeZone; });
});
t('DateTimeFormat Etc/GMT+5 hour at epoch', function () { return new Intl.DateTimeFormat('en-US', { timeZone: 'Etc/GMT+5', hour: 'numeric', hourCycle: 'h23' }).format(0); });
t('DateTimeFormat Etc/GMT-14 hour at epoch', function () { return new Intl.DateTimeFormat('en-US', { timeZone: 'Etc/GMT-14', hour: 'numeric', hourCycle: 'h23' }).format(0); });
t('Etc/GMT+5 equals -05:00', function () { return new Temporal.ZonedDateTime(0n, 'Etc/GMT+5').equals(new Temporal.ZonedDateTime(0n, '-05:00')); });
t('Etc/GMT+5 next transition', function () { return new Temporal.ZonedDateTime(0n, 'Etc/GMT+5').getTimeZoneTransition('next'); });
t('Etc/GMT+5 1850 offset', function () { return at('1850-01-01T00:00Z', 'Etc/GMT+5').offset; });
t('Intl.supportedValuesOf lists Etc/GMT+5', function () { return Intl.supportedValuesOf('timeZone').indexOf('Etc/GMT+5') >= 0; });
