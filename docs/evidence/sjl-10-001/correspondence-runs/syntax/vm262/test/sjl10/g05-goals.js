/*---
description: SJL-10.1 probe g05-goals
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
// G23-G30: each target type's goals, and the area design's table (section 3) of accept and refuse cases.
// The area's table:
k('PlainDate.from("2020-01-01[]")', function () { return Temporal.PlainDate.from('2020-01-01[]').toString(); });
k('PlainYearMonth.from("2020-02-30")', function () { return Temporal.PlainYearMonth.from('2020-02-30').toString(); });
k('PlainTime.from("2021-13-45T12:00")', function () { return Temporal.PlainTime.from('2021-13-45T12:00').toString(); });
// Year-month goal: AnnotatedYearMonth (basic or extended, no day) or a full date-time.
['2020-01', '202001', '+00202001', '+002020-01', '-00202001', '2020-1', '2020-001', '2020-00', '2020-13', '2020-01-01',
 '2020-01[u-ca=gregory]', '2020-01[u-ca=iso8601]', '2020-01[!u-ca=iso8601]', '2020-01[u-ca=ISO8601]', '1976-11[u-ca=ISO8601]',
 '2020-01-01[u-ca=gregory]', '2020-01-15[u-ca=hebrew]', '2020-01[UTC]', '2020-01[foo=bar]', '2020-01T12:00', '2020-01Z',
 '202001[u-ca=gregory]', '2020-02-30[u-ca=gregory]', '+275760-09', '+275760-10', '-271821-04', '-271821-03',
 '-271821-04-01[u-ca=gregory]', '2020-01[u-ca=chinese]'].forEach(function (s) { each(['PlainYearMonth'], s); });
// Month-day goal: AnnotatedMonthDay (optional "--", basic or extended) or a full date-time.
['01-01', '0101', '--01-01', '--0101', '-01-01', '---01-01', '--1-01', '02-29', '02-30', '04-31', '00-01', '13-01',
 '01-00', '01-32', '--01-01[u-ca=iso8601]', '11-18[+01:00][u-ca=ISO8601]', '01-01[u-ca=gregory]', '01-01[!u-ca=iso8601]',
 '2020-01-01[u-ca=gregory]', '2021-02-29', '2020-02-29', '01-01T12:00', '--01-01T12:00', '01-01[UTC]', '01-01Z',
 '2020-01-01[u-ca=hebrew]', '1972-02-29[u-ca=gregory]', '+275760-09-13[u-ca=gregory]', '-271821-04-19[u-ca=gregory]',
 '-271821-04-19[u-ca=hebrew]', '+275760-09-13[u-ca=hebrew]'].forEach(function (s) { each(['PlainMonthDay'], s); });
// Zoned goal: the time zone annotation is required; a date alone is the start of day.
['2020-01-01[UTC]', '2020-01-01T00:00Z[UTC]', '2020-01-01T00:00z[UTC]', '2020-01-01T00:00+00:00[UTC]',
 '2020-01-01T00:00', '2020-01-01Z[UTC]', '2020-01-01T00:00Z', '2020-01-01T00:00[!UTC]', '2020-01-01T00:00[u-ca=iso8601][UTC]',
 '2020-01-01T00:00[foo=bar][UTC]', '2020-01-01T00:00[UTC][foo=bar]', '2020-01-01T00:00[UTC][!foo=bar]',
 '2020-01-01T00:00[UTC][u-ca=hebrew]', '2020-01-01T00:00[+01:00]', '2020-01-01T00:00[+0100]', '2020-01-01T00:00[+01]',
 '2020-01-01T00:00[-00:00]', '2020-01-01T00:00[+01:00:00]', '2020-01-01T00:00[Europe/Berlin]', '2020-01-01T00:00[europe/berlin]',
 '2020-01-01T00:00[]', '2020-01-01T00:00[!]', '2020-01-01T00:00[Mars Olympus]', '2020-01-01[u-ca=iso8601][UTC]',
 '2020-01[UTC]', '01-01[UTC]', 'T00:00[UTC]', '2020-01-01T00:00+01:00[Europe/Berlin]', '2020-01-01T00:00+0100[Europe/Berlin]',
 '2020-01-01T00:00+01[Europe/Berlin]', '2020-01-01T00:00+01:00:00[Europe/Berlin]', '2020-01-01T00:00+01:00:00.000000001[Europe/Berlin]',
 '1970-01-01T00:00-00:44:30[Africa/Monrovia]', '1970-01-01T00:00-00:45[Africa/Monrovia]', '1970-01-01T00:00-00:44[Africa/Monrovia]',
 '1970-01-01T00:00-00:44:30.0[Africa/Monrovia]', '1970-01-01T00:00-0044[Africa/Monrovia]'].forEach(function (s) { each(['ZonedDateTime'], s); });
// Instant goal: a time and Z or an offset are required; annotations are read and ignored.
['2020-01-01T00:00Z', '2020-01-01T00:00', '2020-01-01', '2020-01-01Z', '2020-01-01T00Z', '2020-01-01T00+01',
 '2020-01-01T00:00Z[UTC]', '2020-01-01T00:00Z[Mars/Olympus]', '2020-01-01T00:00Z[u-ca=hebrew]', '2020-01-01T00:00Z[!u-ca=hebrew]',
 '2020-01-01T00:00Z[u-ca=unknowncal]', '2020-01-01T00:00Z[!foo=bar]', '2020-01-01 00:00Z', '20200101T0000Z',
 '-271821-04-20T00:00Z', '-271821-04-19T23:59:59.999999999Z', '-271821-04-19T00:00Z', '+275760-09-13T00:00Z',
 '+275760-09-13T00:00:00.000000001Z', '-271821-04-19T23:00-01:00', '+275760-09-13T01:00+01:00', '+275760-09-14T00:00+01:00',
 '-271821-04-18T23:00-01:00'].forEach(function (s) { each(['Instant'], s); });
// Time goal through a date-time: the date is checked, the time required.
['2020-01-01T12:00', '2020-01-01', '2020-13-01T12:00', '2020-01-01T12:00[u-ca=hebrew]', '2020-01-01T12:00[u-ca=unknowncal]',
 '2020-01-01T12:00[!u-ca=hebrew]', '2020-01-01T12:00+01:00[Europe/Berlin]'].forEach(function (s) { each(['PlainTime'], s); });
// Date goal: the calendar of the annotation, canonicalised.
['2020-01-01[u-ca=GREGORY]', '2020-01-01[u-ca=Gregory]', '2020-01-01[u-ca=ethiopic-amete-alem]', '2020-01-01[u-ca=islamicc]',
 '2020-01-01[u-ca=gregorian]', '2020-01-01[u-ca=minguo]', '2020-01-01[u-ca=islamic]', '2020-01-01[u-ca=islamic-rgsa]'
].forEach(function (s) { each(['PlainDate'], s); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
