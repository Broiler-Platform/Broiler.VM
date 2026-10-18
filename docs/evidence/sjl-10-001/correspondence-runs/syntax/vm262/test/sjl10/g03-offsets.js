/*---
description: SJL-10.1 probe g03-offsets
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
// G10-G13: UTC offsets after a time, and the designator Z, in every goal. Z is refused where the goal's
// DateTimeUTCOffset is [~Z] (the plain types); an offset is optional except for Instant; hour 00-23, minute and
// second 00-59, at most nine fraction digits, consistent separators within the offset, ASCII sign and digits.
var offsets = ['Z', 'z', '+00:00', '-00:00', '-00', '-0000', '+01', '+0100', '+01:00', '+01:00:00', '+010000',
  '+01:00:00.5', '+01:00:00,5', '+010000.123456789', '+01:00:00.1234567890', '+01:0000', '+0100:00', '+24:00',
  '+23:60', '+23:59:60', '+23:59:59.999999999', '+1', '+001', '−01:00', '+٠١:٠٠', '+01:00Z',
  'Z+01:00', '+01:00:00.', '+01:00.5'];
offsets.forEach(function (o) {
  each(PLAIN, '2020-01-01T12:00' + o);
  each(['PlainTime'], '12:00' + o);
  k('ZonedDateTime.from(' + JSON.stringify('2020-01-01T12:00' + o + '[UTC]') + ', {offset: "ignore"})', function () {
    return Temporal.ZonedDateTime.from('2020-01-01T12:00' + o + '[UTC]', { offset: 'ignore' }).toString(); });
  each(['Instant'], '2020-01-01T12:00' + o);
});
// An offset with no time, and an offset on a date-only string.
each(ALL7, '2020-01-01+01:00');
each(ALL7, '2020-01-01Z');
each(ALL7, '2020-01-01+01:00[UTC]');
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
