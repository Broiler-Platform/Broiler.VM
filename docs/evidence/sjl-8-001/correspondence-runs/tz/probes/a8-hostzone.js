// A8: the default (host) zone and Date's local time, beside Temporal's reading of the same zone.
t('Temporal.Now.timeZoneId()', function () { return Temporal.Now.timeZoneId(); });
t('Intl.DateTimeFormat().resolvedOptions().timeZone', function () { return new Intl.DateTimeFormat().resolvedOptions().timeZone; });
t('typeof Temporal.Now.zonedDateTimeISO()', function () { return typeof Temporal.Now.zonedDateTimeISO(); });
t('new Date(2040, 6, 1, 12).getTimezoneOffset()', function () { return new Date(2040, 6, 1, 12).getTimezoneOffset(); });
t('new Date(2026, 6, 1, 12).getTimezoneOffset()', function () { return new Date(2026, 6, 1, 12).getTimezoneOffset(); });
t('America/New_York 2040-07-01 Temporal offset', function () { return at('2040-07-01T16:00Z', 'America/New_York').offset; });
t('DateTimeFormat (default zone) hour at 2040-07-01T16:00Z', function () { return new Intl.DateTimeFormat('en-US', { hour: 'numeric', hourCycle: 'h23' }).format(Date.UTC(2040, 6, 1, 16)); });
