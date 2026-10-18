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
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
