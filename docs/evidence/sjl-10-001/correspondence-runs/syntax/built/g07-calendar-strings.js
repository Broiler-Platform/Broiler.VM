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
// G37-G40: ParseTemporalCalendarString, through a property bag's calendar and withCalendar: an ISO string answers
// its u-ca annotation (iso8601 without one), anything else must be an AnnotationValue and a supported calendar.
['iso8601', 'ISO8601', 'gregory', '2020-01-01', '2020-01-01[u-ca=gregory]', '2020-01-01T00:00Z[UTC][u-ca=hebrew]',
 '2020-01-01T00:00Z[u-ca=hebrew]', 'T10:00Z', '10:00Z', 'T10:00[u-ca=hebrew]', '10:00[u-ca=hebrew]', '2020-01[u-ca=gregory]',
 '2020-01[u-ca=iso8601]', '01-01[u-ca=gregory]', '--01-01', '2020-01-01[]', '2020-01-01[u-ca=gregory][UTC]',
 '2020-01-01[UTC][u-ca=gregory]', '2020-01-01[!foo=bar]', '2020-01-01[u-ca=gregory][!u-ca=japanese]', 'gregory-x', '',
 '-', 'a_b', '2020-13-01', '2020-01-01T25:00[u-ca=gregory]', '2020-01-01T12:00+25:00[u-ca=gregory]',
 '2020-01-01T12:00+01:00:00.5[u-ca=gregory]', '2020-01-01[Mars Olympus][u-ca=gregory]', '2020-02-30[u-ca=gregory]',
 '1214[u-ca=gregory]', 'T1214[u-ca=gregory]', '2020-01-01T12:3045Z[u-ca=gregory]', 'gregorian', 'minguo', 'islamic',
 'japanese ', ' japanese', 'JAPANESE', 'ethiopic-amete-alem', 'islamicc', 'islamic-civil', 'u-ca=gregory', '[u-ca=gregory]'
].forEach(function (c) {
  k('PlainDate.from({..., calendar: ' + JSON.stringify(c) + '}).calendarId', function () {
    return Temporal.PlainDate.from({ year: 2020, monthCode: 'M01', day: 1, calendar: c }).calendarId; });
  k('PlainDate#withCalendar(' + JSON.stringify(c) + ').calendarId', function () {
    return new Temporal.PlainDate(2020, 1, 1).withCalendar(c).calendarId; });
  k('new PlainDate(2020, 1, 1, ' + JSON.stringify(c) + ').calendarId', function () {
    return new Temporal.PlainDate(2020, 1, 1, c).calendarId; });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
