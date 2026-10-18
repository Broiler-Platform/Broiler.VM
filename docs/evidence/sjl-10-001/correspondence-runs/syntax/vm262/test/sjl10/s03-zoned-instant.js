/*---
description: SJL-10.1 probe s03-zoned-instant
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
// The error's type and the first line of its message (a .NET exception's message carries its stack trace).
function t1(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + ((e && e.constructor && e.constructor.name) || typeof e) + ': ' + String(e && e.message).split('\n')[0]; }
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
// The calendars of the Intl era and month code proposal's Table 1, other than iso8601, and a year of each in the
// 2020s that is a leap year where the calendar has one (Hebrew 5784 and Chinese and Korean 2023 have a leap month).
var CALS = ['buddhist', 'chinese', 'coptic', 'dangi', 'ethioaa', 'ethiopic', 'gregory', 'hebrew', 'indian', 'islamic-civil',
  'islamic-tbla', 'islamic-umalqura', 'japanese', 'persian', 'roc'];
var LEAPYEAR = { buddhist: 2567, chinese: 2023, coptic: 1739, dangi: 2023, ethioaa: 7515, ethiopic: 2015, gregory: 2024,
  hebrew: 5784, indian: 1946, 'islamic-civil': 1445, 'islamic-tbla': 1445, 'islamic-umalqura': 1445, iso8601: 2024,
  japanese: 2024, persian: 1403, roc: 113 };
function ymd(d) { return d.year + '/' + d.monthCode + '/' + d.day + ' (' + d.month + ') iso ' + d.toString({ calendarName: 'never' }); }
// S17-S22: ZonedDateTime's offset and timeZoneName options and its calendar annotation; offsets of zones with
// sub-minute offsets, rounded to the minute in strings; Instant.toString's timeZone.
var z = function () { return Temporal.ZonedDateTime.from('2020-07-01T12:00:00.5+02:00[Europe/Berlin][u-ca=japanese]'); };
['auto', 'never', 'critical', undefined, 'x', 'AUTO', null].forEach(function (tz) {
  ['auto', 'never', undefined, 'x', 'critical'].forEach(function (off) {
    k('ZonedDateTime timeZoneName ' + JSON.stringify(tz) + ' offset ' + JSON.stringify(off), function () { return z().toString({ timeZoneName: tz, offset: off }); });
  });
});
['auto', 'always', 'never', 'critical'].forEach(function (cn) {
  k('ZonedDateTime iso8601 calendarName ' + cn + ' timeZoneName critical', function () {
    return Temporal.ZonedDateTime.from('2020-07-01T12:00[Europe/Berlin]').toString({ calendarName: cn, timeZoneName: 'critical' }); });
});
// Sub-minute offsets: Monrovia's -00:44:30 (1970), Amsterdam's +00:19:32 (1930) and +00:20 (1938), Dublin 1900.
[['1970-01-01T00:00Z', 'Africa/Monrovia'], ['1930-06-01T00:00Z', 'Europe/Amsterdam'], ['1938-06-01T00:00Z', 'Europe/Amsterdam'],
 ['1900-01-01T00:00Z', 'Europe/Dublin'], ['1850-01-01T00:00Z', 'America/New_York'], ['1850-01-01T00:00Z', 'Asia/Kolkata'],
 ['1900-01-01T00:00Z', 'Asia/Kolkata'], ['1850-01-01T00:00Z', 'Africa/Abidjan']].forEach(function (c) {
  t('ZonedDateTime ' + c[1] + ' at ' + c[0], function () { var d = Temporal.Instant.from(c[0]).toZonedDateTimeISO(c[1]); return d.toString() + ' offset ' + d.offset + ' ' + d.offsetNanoseconds; });
  t('Instant ' + c[0] + ' toString({timeZone: ' + c[1] + '})', function () { return Temporal.Instant.from(c[0]).toString({ timeZone: c[1] }); });
  t('round trip of ' + c[1] + ' at ' + c[0], function () { var s = Temporal.Instant.from(c[0]).toZonedDateTimeISO(c[1]).toString(); return Temporal.ZonedDateTime.from(s).epochNanoseconds; });
});
// Instant.toString's timeZone values, and its rounding with a zone.
['UTC', '+01:00', '-00:00', '+05:30', 'Asia/Kolkata', 'Europe/Berlin', 'utc', '+0530', '2020-01-01T00:00+01:00', 'Mars/Olympus', '+01:00:01', null, 0].forEach(function (zone) {
  k('Instant toString({timeZone: ' + JSON.stringify(zone) + '})', function () { return Temporal.Instant.from('2020-07-01T12:00:00.123Z').toString({ timeZone: zone }); });
});
k('Instant toString({timeZone: Asia/Kolkata, smallestUnit: hour})', function () { return Temporal.Instant.from('2020-07-01T12:00Z').toString({ timeZone: 'Asia/Kolkata', smallestUnit: 'hour' }); });
k('Instant toString({timeZone: Asia/Kolkata, smallestUnit: minute})', function () { return Temporal.Instant.from('2020-07-01T12:00:31Z').toString({ timeZone: 'Asia/Kolkata', smallestUnit: 'minute' }); });
k('ZonedDateTime toString rounding to the minute across a transition', function () {
  return Temporal.ZonedDateTime.from('2020-03-29T00:59:59.9+00:00[Europe/London]').toString({ smallestUnit: 'minute', roundingMode: 'ceil' }); });
k('ZonedDateTime toString rounding to the second in a fold', function () {
  return Temporal.ZonedDateTime.from('2020-10-25T01:59:59.9+01:00[Europe/London]').toString({ smallestUnit: 'second', roundingMode: 'ceil' }); });
k('ZonedDateTime offset getter, Monrovia 1970', function () { return Temporal.Instant.from('1970-01-01T00:00Z').toZonedDateTimeISO('Africa/Monrovia').offset; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
