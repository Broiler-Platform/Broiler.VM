/*---
description: SJL-10.1 probe g02-times
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
// G5-G9: the time part. Separators and their mix, hour alone, fraction digits (1 to 9, 10 refused), comma,
// leap second, 24:00, fractions on minutes or hours (refused), designators T, t and space, and the early errors
// that refuse a designator-less time that is also a month-day or a year-month.
var bare = ['12', '1230', '12:30', '123045', '12:30:45', '12:30:45.1', '12:30:45.123456789', '12:30:45.1234567890',
  '12:30:45,5', '12:30:60', '12:30:61', '24:00', '23:59:59.999999999', '12:3045', '1230:45', '12:30:45.', '12:30.5',
  '12.5', 'T12:30', 't12:30', 'T12', ' 12:30', 'TT12:30', '2021-12', '202112', '2021-13', 'T2021-12', '1214', '0229',
  '0230', '1232', '--1214', '12:30Z', '12:30z', 'T12:30Z', '١٢:٣٠', '12:30:45.١'];
bare.forEach(function (s) { each(['PlainTime'], s); });
var withDate = ['T12', 'T1230', 't12:30', ' 12:30', 'T', 'TT12:30', 'T12:30:45.123456789', 'T12:30:45.1234567890',
  'T12:30:60', 'T24:00', 'T23:59:60', 'T12:3045', 'T1230:45', 'T12:30.5', 'T12.5', 'T12:30,5', '  12:30', 'T12:30:45.0'];
withDate.forEach(function (tail) {
  each(PLAIN, '2020-01-01' + tail);
  each(PLAIN, '20200101' + tail);
  each(['ZonedDateTime'], '2020-01-01' + tail + '[UTC]');
  each(['Instant'], '2020-01-01' + tail + 'Z');
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
