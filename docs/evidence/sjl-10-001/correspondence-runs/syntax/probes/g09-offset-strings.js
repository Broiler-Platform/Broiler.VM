// G45-G46: ToOffsetString, the offset field of a property bag and of with(): a UTCOffset[+SubMinutePrecision], so no
// Z, hour 00-23, minute and second 00-59; a value other than a string is a TypeError. The area's table names with().
['Z', 'z', '+00:00', '-00:00', '+25:00', '+24:00', '+23:59', '+01:00:00.5', '+01:00:00.123456789', '+01:00:00.1234567890',
 '+0100', '+01', '-00', '+01:00:60', '+01:0000', '−01:00', '+٠١:٠٠', '', 'UTC', '+1', '+01:00Z'
].forEach(function (o) {
  k('ZonedDateTime#with({offset: ' + JSON.stringify(o) + '}, {offset: "ignore"})', function () {
    return Temporal.ZonedDateTime.from('2020-01-01T00:00[UTC]').with({ offset: o }, { offset: 'ignore' }).toString(); });
  k('ZonedDateTime.from({..., offset: ' + JSON.stringify(o) + ', timeZone: "UTC"}, {offset: "use"})', function () {
    return Temporal.ZonedDateTime.from({ year: 2020, month: 1, day: 1, offset: o, timeZone: 'UTC' }, { offset: 'use' }).toString(); });
});
[0, null, true, 1n].forEach(function (o) {
  k('ZonedDateTime#with({offset: ' + String(o) + '})', function () {
    return Temporal.ZonedDateTime.from('2020-01-01T00:00[UTC]').with({ offset: o }).toString(); });
});
