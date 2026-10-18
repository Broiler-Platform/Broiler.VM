/*---
description: SJL-10.1 probe g06-durations
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
// G31-G36: duration strings (ParseTemporalDurationString). Designators in either case, the order of parts, a fraction
// only on the last time part, the time designator with no time part, signs, and values at the limits, whose
// digits each engine reads its own way (Broiler.VM exactly, Broiler.JS by double.Parse).
['P1Y', 'p1y', 'P1Y2M3W4DT5H6M7S', 'P1Y1M1W1D', 'P1M1W', 'P1W1D', 'PT0S', 'P0D', '-P1D', '+P1D', '−P1D', 'P', 'PT',
 'P1YT', 'P1DT', 'P1Dt', 'PT1H1.5M', 'PT1.5H', 'PT1.5H30M', 'PT1.5M30S', 'PT1,5S', 'PT0.123456789S', 'PT0.1234567890S',
 'PT1.S', 'PT.5S', 'P1.5D', 'P1.5Y', 'P1.5W', 'P1D1H', 'P1H', 'PT1D', 'P1M1Y', 'P1W1M', 'PT1S1M', 'PT1M1H', 'P 1D',
 'P1D ', ' P1D', 'PT46H66M71.50040904S', 'P4294967295Y', 'P4294967296Y', 'P4294967295M', 'P4294967295W',
 'P104249991374D', 'P104249991375D', 'PT9007199254740991S', 'PT9007199254740991.999999999S', 'PT9007199254740992S',
 'PT2501999792983H', 'PT2501999792984H', 'PT150119987579016M', 'PT2501999792983.999999999H', 'P1Y1Y', 'PT1H1H',
 'P١D', 'PT١H', 'P1d', 'pt1h', 'PT1h', 'P1Dt1H', 'P1DT1H1M1.123456789S', 'PT0.000000001H', 'PT0.000000001M',
 'PT1.999999999H', '-PT1.000000001M', 'P0Y0M0W0DT0H0M0.000000000S', 'PT00000000000000000000001S',
 'P00000000000000000000001D', 'PT1.5H0M', 'PT1H0.5M', '-P1Y-1M', 'P-1D', 'P1Y2M3W4DT5H6M7.008009010S'
].forEach(function (s) { each(['Duration'], s); });
// The fields of a few, read back.
['PT46H66M71.50040904S', 'PT1.5H', 'PT0.000000001H', '-PT1.000000001M', 'PT2501999792983.999999999H'].forEach(function (s) {
  t('fields of ' + JSON.stringify(s), function () {
    var d = Temporal.Duration.from(s);
    return [d.hours, d.minutes, d.seconds, d.milliseconds, d.microseconds, d.nanoseconds].join(',');
  });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
