// A9: Date's local time (the host provider) against Temporal's reading of the same zone, in Broiler.JS.
t('Temporal.Now.timeZoneId()', function () { return Temporal.Now.timeZoneId(); });
t('new Date(2027, 0, 15, 12).getTimezoneOffset()', function () { return new Date(2027, 0, 15, 12).getTimezoneOffset(); });
t('Temporal offset of the host zone at 2027-01-15T20:00Z', function () { return at('2027-01-15T20:00Z', Temporal.Now.timeZoneId()).offset; });
t('Temporal.Now.zonedDateTimeISO().offset (now)', function () { return Temporal.Now.zonedDateTimeISO().offset; });
t('new Date().getTimezoneOffset() (now)', function () { return new Date().getTimezoneOffset(); });
t('new Date(2040, 6, 1, 12).getTimezoneOffset()', function () { return new Date(2040, 6, 1, 12).getTimezoneOffset(); });
t('Temporal offset of the host zone at 2040-07-01T19:00Z', function () { return at('2040-07-01T19:00Z', Temporal.Now.timeZoneId()).offset; });
