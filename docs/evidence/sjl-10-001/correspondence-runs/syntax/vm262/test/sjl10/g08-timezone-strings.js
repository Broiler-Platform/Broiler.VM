/*---
description: SJL-10.1 probe g08-timezone-strings
flags: [raw]
---*/
var __vmTest = true;
// Shared prelude (SJL-8.1's, with SJL-10.1's helpers added at the end): Broiler.JS's shell has print; Broiler.VM's
// test262 realm has none, so the lines are collected and handed back in one uncaught Error the
// conformance root prints.
var __lines = [];
var __host = typeof __vmTest === 'undefined' && typeof print === 'function';
function out(s) { s = String(s); if (__host) print(s); else __lines.push(s); }
function show(v) {
  if (v === null) return 'null';
  if (v === undefined) return 'undefined';
  if (typeof v === 'bigint') return v + 'n';
  if (typeof v === 'string') return v;
  if (Array.isArray(v)) return '[' + v.map(show).join(', ') + ']';
  return String(v);
}
function t(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + ((e && e.constructor && e.constructor.name) || typeof e) + ': ' + (e && e.message); }
  out(label + ' => ' + r);
}
// The error's type alone, for rows that compare acceptance and refusal (messages differ by design).
function k(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + ((e && e.constructor && e.constructor.name) || typeof e); }
  out(label + ' => ' + r);
}
// SJL-10.1: every from() a grammar goal reaches, each answering its value's toString().
var FROM = {
  PlainDate: function (s) { return Temporal.PlainDate.from(s).toString(); },
  PlainDateTime: function (s) { return Temporal.PlainDateTime.from(s).toString(); },
  PlainTime: function (s) { return Temporal.PlainTime.from(s).toString(); },
  PlainYearMonth: function (s) { return Temporal.PlainYearMonth.from(s).toString(); },
  PlainMonthDay: function (s) { return Temporal.PlainMonthDay.from(s).toString(); },
  ZonedDateTime: function (s) { return Temporal.ZonedDateTime.from(s).toString(); },
  Instant: function (s) { return Temporal.Instant.from(s).toString(); },
  Duration: function (s) { return Temporal.Duration.from(s).toString(); }
};
// Runs one string through the named from()s; a refusal shows the error's type only.
function each(types, s) {
  types.forEach(function (type) { k(type + '.from(' + JSON.stringify(s) + ')', function () { return FROM[type](s); }); });
}
var PLAIN = ['PlainDate', 'PlainDateTime', 'PlainTime', 'PlainYearMonth', 'PlainMonthDay'];
var ALL7 = ['PlainDate', 'PlainDateTime', 'PlainTime', 'PlainYearMonth', 'PlainMonthDay', 'ZonedDateTime', 'Instant'];
// G41-G44: ParseTemporalTimeZoneString, through toZonedDateTimeISO, a property bag's timeZone and the constructor: an
// identifier, or the zone an ISO string names (its annotation, else Z as UTC, else its offset, to the minute).
['UTC', '2020-01-01T00:00Z', '2020-01-01T00:00z', '2020-01-01T00:00+01:00', '2020-01-01T00:00+01:00:01',
 '2020-01-01T00:00+01:00:00', '2020-01-01[Europe/Berlin]', '2020-01-01T00:00Z[Europe/Berlin]', 'T10:00Z', '10:00Z',
 '10:00+01:00', 'T10:00+01:00', '10:00[Europe/Berlin]', '2020-01-01', '2020-01-01T00:00', '2020-01-01T00:00[u-ca=gregory]',
 '2020-01[Europe/Berlin]', '01-01[Europe/Berlin]', '2020-01-01[]', '2020-01-01T00:00+25:00', '2020-01-01T00:00+23:60',
 '2020-01-01[u-ca=gregory][Europe/Berlin]', '2020-01-01[foo=bar][Europe/Berlin]', '+01:00', '+0100', '+01', '-00:00',
 '+01:00:00', '+01:00:00.5', '+24:00', '2020-01-01T00:00+01:00[+02:00]', '2020-01-01T00:00Z[+02:00]',
 '2020-01-01T12:3045Z', '2021-12', '1214Z', '2020-01-01T00:00Z[!Europe/Berlin]', '2020-01-01T00:00Z[u-ca=gregory][!foo=bar]',
 '2020-01-01T00:00-00:00', '2020-01-01T00:00-00'
].forEach(function (z) {
  k('Instant#toZonedDateTimeISO(' + JSON.stringify(z) + ').timeZoneId', function () {
    return Temporal.Instant.from('2020-01-01T00:00Z').toZonedDateTimeISO(z).timeZoneId; });
  k('ZonedDateTime.from({..., timeZone: ' + JSON.stringify(z) + '}).timeZoneId', function () {
    return Temporal.ZonedDateTime.from({ year: 2020, month: 1, day: 1, timeZone: z }).timeZoneId; });
  k('new ZonedDateTime(0n, ' + JSON.stringify(z) + ').timeZoneId', function () {
    return new Temporal.ZonedDateTime(0n, z).timeZoneId; });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
