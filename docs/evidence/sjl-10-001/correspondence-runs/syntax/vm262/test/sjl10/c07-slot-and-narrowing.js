/*---
description: SJL-10.1 probe c07-slot-and-narrowing
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
// C31-C33: the calendar a Temporal object lends as a calendar value (ToTemporalCalendarIdentifier reads its slot),
// and integer fields of a calendar property bag beyond 32 bits (the milestone's integer narrowing, where it touches
// the calendar fields a string or bag carries).
var md = Temporal.PlainMonthDay.from({ calendar: 'hebrew', monthCode: 'M05L', day: 1 });
var ym = Temporal.PlainYearMonth.from({ calendar: 'coptic', year: 1740, monthCode: 'M13' });
var zdt = Temporal.ZonedDateTime.from('2020-01-01T00:00[UTC][u-ca=japanese]');
[['a PlainMonthDay in hebrew', md], ['a PlainYearMonth in coptic', ym], ['a ZonedDateTime in japanese', zdt],
 ['a PlainDateTime in indian', Temporal.PlainDateTime.from('2020-01-01T00:00[u-ca=indian]')],
 ['a PlainTime', Temporal.PlainTime.from('12:00')], ['a Duration', Temporal.Duration.from('P1D')], ['an Instant', Temporal.Instant.from('2020-01-01T00:00Z')]
].forEach(function (c) {
  k('PlainDate.from({..., calendar: ' + c[0] + '}).calendarId', function () { return Temporal.PlainDate.from({ year: 2020, month: 1, day: 1, calendar: c[1] }).calendarId; });
  k('PlainDate#withCalendar(' + c[0] + ').calendarId', function () { return new Temporal.PlainDate(2020, 1, 1).withCalendar(c[1]).calendarId; });
  k('PlainMonthDay.from({..., calendar: ' + c[0] + '}).calendarId', function () { return Temporal.PlainMonthDay.from({ monthCode: 'M01', day: 1, calendar: c[1] }).calendarId; });
});
// Fields beyond 32 bits: 2^32 + y wraps to y under a 32-bit narrowing.
var W = 4294967296;
['iso8601', 'gregory', 'japanese', 'hebrew', 'chinese', 'persian', 'coptic', 'islamic-civil'].forEach(function (cal) {
  k(cal + ' year 2^32+' + LEAPYEAR[cal], function () { return ymd(Temporal.PlainDate.from({ calendar: cal, year: W + LEAPYEAR[cal], monthCode: 'M01', day: 1 })); });
  k(cal + ' day 2^32+1 constrain', function () { return ymd(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], monthCode: 'M01', day: W + 1 })); });
  k(cal + ' day 2^32+1 reject', function () { return ymd(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], monthCode: 'M01', day: W + 1 }, { overflow: 'reject' })); });
  k(cal + ' month 2^32+1 constrain', function () { return ymd(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], month: W + 1, day: 1 })); });
  k(cal + ' PlainYearMonth year 2^32+' + LEAPYEAR[cal], function () { var d = Temporal.PlainYearMonth.from({ calendar: cal, year: W + LEAPYEAR[cal], monthCode: 'M01' }); return d.year + ' ' + d.toString(); });
  k(cal + ' PlainMonthDay year 2^32+' + LEAPYEAR[cal], function () { var d = Temporal.PlainMonthDay.from({ calendar: cal, year: W + LEAPYEAR[cal], monthCode: 'M01', day: 1 }); return d.toString(); });
});
['gregory', 'japanese', 'roc', 'coptic', 'ethiopic', 'islamic-civil', 'hebrew', 'persian'].forEach(function (cal) {
  var era = Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], month: 1, day: 1 }).era;
  k(cal + ' era ' + era + ' eraYear 2^32+5', function () { return ymd(Temporal.PlainDate.from({ calendar: cal, era: era, eraYear: W + 5, monthCode: 'M01', day: 1 })); });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
