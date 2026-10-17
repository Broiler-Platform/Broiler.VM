// A3: start of day, where midnight is skipped.
var cases = [['America/Santiago', '2026-09-06'], ['America/Havana', '2026-03-08'], ['Asia/Beirut', '1920-03-28'], ['Asia/Beirut', '2026-03-29'],
  ['America/Toronto', '1919-03-31'], ['America/Sao_Paulo', '2018-11-04'], ['America/Asuncion', '2024-10-06'], ['Pacific/Apia', '2011-12-30'],
  ['America/New_York', '2026-03-08'],
  ['America/Santiago', '2040-09-02'], ['America/Havana', '2040-03-11'], ['Asia/Beirut', '2040-03-25']];
cases.forEach(function (c) {
  t('PlainDate ' + c[1] + ' toZonedDateTime ' + c[0], function () { return Temporal.PlainDate.from(c[1]).toZonedDateTime(c[0]).toString(); });
  t('startOfDay ' + c[1] + ' ' + c[0], function () { return Temporal.ZonedDateTime.from(c[1] + 'T12:00[' + c[0] + ']').startOfDay().toString(); });
  t('hoursInDay ' + c[1] + ' ' + c[0], function () { return Temporal.ZonedDateTime.from(c[1] + 'T12:00[' + c[0] + ']').hoursInDay; });
});
t('ZonedDateTime.from date-only string in gap America/Santiago', function () { return Temporal.ZonedDateTime.from('2026-09-06[America/Santiago]').toString(); });
t('startOfDay at the lower limit -271821-04-20 UTC', function () { return Temporal.ZonedDateTime.from('-271821-04-20T12:00[UTC]').startOfDay().toString(); });
t('startOfDay at the lower limit -271821-04-20 Etc/GMT+1', function () { return new Temporal.ZonedDateTime(-8640000000000000000000n, 'Etc/GMT+1').startOfDay().toString(); });
t('startOfDay at the lower limit -271821-04-19 Etc/GMT-1', function () { return new Temporal.ZonedDateTime(-8640000000000000000000n, 'Etc/GMT-1').startOfDay().toString(); });
