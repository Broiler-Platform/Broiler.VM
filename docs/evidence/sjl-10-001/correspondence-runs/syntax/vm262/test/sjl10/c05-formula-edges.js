/*---
description: SJL-10.1 probe c05-formula-edges
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
// C21-C27: the formula calendars at their edges: the ISO limits, years 0 and 1, each calendar's epoch, its new years
// around 2020, leap days, the Coptic and Ethiopic thirteenth month, the Indian leap Chaitra, the tabular Islamic
// leap year, Hebrew years of each length. Fields read, and the date rebuilt from its fields.
var F = ['gregory', 'buddhist', 'roc', 'japanese', 'coptic', 'ethiopic', 'ethioaa', 'indian', 'islamic-civil', 'islamic-tbla', 'hebrew'];
function fields(d) {
  return [d.era, d.eraYear, d.year, d.month, d.monthCode, d.day, d.dayOfYear, d.daysInMonth, d.daysInYear, d.monthsInYear, d.inLeapYear,
    d.weekOfYear, d.yearOfWeek].join(' ');
}
['-271821-04-19', '-271821-04-20', '+275760-09-13', '+275760-09-12', '0000-01-01', '0000-12-31', '0001-01-01', '-000001-12-31',
 '0284-08-29', '0008-08-27', '-005492-07-17', '0622-07-16', '0622-07-15', '0079-03-22', '-003761-10-07', '2019-09-11', '2019-09-12', '2020-09-11',
 '2020-09-12', '2023-09-11', '2023-09-12', '2024-03-20', '2024-03-21', '2024-03-22', '2024-07-07', '2024-07-08', '2024-06-06', '2024-06-07',
 '2024-10-02', '2024-10-03', '2025-09-22', '2025-09-23', '2023-03-22', '2022-03-22', '2026-09-12', '2027-10-02', '2028-09-21', '2031-09-18',
 '2029-09-10', '2010-09-09', '1900-02-28', '1900-03-01', '2100-02-28', '2100-03-01', '1582-10-14', '1582-10-15'].forEach(function (iso) {
  F.forEach(function (cal) {
    t(cal + ' ' + iso, function () { return fields(Temporal.PlainDate.from(iso).withCalendar(cal)); });
  });
});
// The round trip: fields back to the ISO date, at the ISO limits.
F.forEach(function (cal) {
  ['-271821-04-19', '+275760-09-13'].forEach(function (iso) {
    t(cal + ' round trip ' + iso, function () {
      var d = Temporal.PlainDate.from(iso).withCalendar(cal);
      return Temporal.PlainDate.from({ calendar: cal, year: d.year, monthCode: d.monthCode, day: d.day }).toString(); });
    t(cal + ' one day past ' + iso, function () {
      var d = Temporal.PlainDate.from(iso).withCalendar(cal);
      return Temporal.PlainDate.from({ calendar: cal, year: d.year, monthCode: d.monthCode, day: d.day + (iso[0] === '+' ? 1 : -1) }, { overflow: 'reject' }).toString(); });
  });
});
// Hebrew years of each of the six lengths, and the length of each month in them.
[5781, 5782, 5783, 5784, 5785, 5786, 5806, 5795].forEach(function (y) {
  t('hebrew ' + y + ' months', function () {
    var first = Temporal.PlainDate.from({ calendar: 'hebrew', year: y, month: 1, day: 1 }), r = [];
    for (var m = 1; m <= first.monthsInYear; m++) r.push(Temporal.PlainDate.from({ calendar: 'hebrew', year: y, month: m, day: 1 }).daysInMonth);
    return first.daysInYear + ' ' + first.toString({ calendarName: 'never' }) + ' ' + r.join(','); });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
