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
// C35-C37: the reference dates conversions write: toPlainYearMonth (the first day of the calendar month), toPlainMonthDay
// (the month-day's reference year, and its calendar), PlainMonthDay#toPlainDate and PlainYearMonth#toPlainDate (the year
// read in the calendar).
['iso8601'].concat(CALS).forEach(function (cal) {
  var d = Temporal.PlainDate.from('2023-03-15').withCalendar(cal);
  t(cal + ' PlainDate#toPlainYearMonth', function () { return d.toPlainYearMonth().toString({ calendarName: 'always' }); });
  t(cal + ' PlainDate#toPlainMonthDay', function () { var md = d.toPlainMonthDay(); return md.calendarId + ' ' + md.monthCode + '/' + md.day + ' ' + md.toString({ calendarName: 'always' }); });
  t(cal + ' PlainDateTime#toPlainDate#toPlainYearMonth', function () { return Temporal.PlainDateTime.from('2023-03-15T12:00').withCalendar(cal).toPlainDate().toPlainYearMonth().toString({ calendarName: 'always' }); });
  t(cal + ' ZonedDateTime#toPlainDate#toPlainMonthDay', function () { return Temporal.ZonedDateTime.from('2023-03-15T12:00[UTC]').withCalendar(cal).toPlainDate().toPlainMonthDay().toString({ calendarName: 'always' }); });
  t(cal + ' PlainMonthDay#toPlainDate({year: the calendar year of 2001-06-01})', function () {
    return Temporal.PlainMonthDay.from({ calendar: cal, monthCode: 'M02', day: 10 }).toPlainDate({ year: Temporal.PlainDate.from('2001-06-01').withCalendar(cal).year }).toString({ calendarName: 'always' }); });
  t(cal + ' PlainYearMonth#toPlainDate({day: 10})', function () { return d.toPlainYearMonth().toPlainDate({ day: 10 }).toString({ calendarName: 'always' }); });
  t(cal + ' PlainYearMonth.from(fields) reference day', function () { return Temporal.PlainYearMonth.from({ calendar: cal, year: d.year, monthCode: d.monthCode }).toString({ calendarName: 'always' }); });
  t(cal + ' PlainYearMonth#with({monthCode: M01})', function () { return d.toPlainYearMonth().with({ monthCode: 'M01' }).toString({ calendarName: 'always' }); });
  t(cal + ' PlainMonthDay#with({day: 1})', function () { return d.toPlainMonthDay().with({ day: 1 }).toString({ calendarName: 'always' }); });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
