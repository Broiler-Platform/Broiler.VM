/*---
description: SJL-10.1 probe c06-table-edges
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
// C28-C30: the table calendars at their tables' edges (SJL-8.1's rows K1 to K13 hold these calendars' years in depth;
// these confirm the arithmetic over them at the seams): chinese and dangi 1899-1901, 2050-2051, 2100-2103; Umm
// al-Qura 1299-1301 and 1599-1601 AH; Persian 1501-1502 and 2987-2988 AP. New year, months, leap month, length.
function yearInfo(cal, y) {
  var first = Temporal.PlainDate.from({ calendar: cal, year: y, month: 1, day: 1 }), r = [];
  for (var m = 1; m <= first.monthsInYear; m++) r.push(Temporal.PlainDate.from({ calendar: cal, year: y, month: m, day: 1 }).daysInMonth === 30 ? 'l' : 's');
  var leap = []; for (var n = 1; n <= first.monthsInYear; n++) { var c = Temporal.PlainDate.from({ calendar: cal, year: y, month: n, day: 1 }).monthCode; if (c.length === 4) leap.push(c + '@' + n); }
  return first.toString({ calendarName: 'never' }) + ' ' + first.daysInYear + ' ' + first.monthsInYear + ' ' + r.join('') + ' ' + leap.join(',');
}
[['chinese', [1899, 1900, 1901, 2050, 2051, 2100, 2101, 2102, 2103]], ['dangi', [1899, 1900, 1901, 2050, 2051, 2100, 2101, 2102, 2103]],
 ['islamic-umalqura', [1299, 1300, 1301, 1599, 1600, 1601]], ['persian', [1501, 1502, 2987, 2988]]].forEach(function (c) {
  c[1].forEach(function (y) { t(c[0] + ' ' + y, function () { return yearInfo(c[0], y); }); });
});
// The arithmetic across the seams: a year added, a month added, until() in months.
[['chinese', '1900-01-15', { years: 1 }], ['chinese', '2102-06-01', { years: 1 }], ['chinese', '2102-06-01', { months: 13 }],
 ['islamic-umalqura', '2173-12-01', { months: 1 }], ['islamic-umalqura', '1882-11-01', { months: 1 }], ['persian', '2123-03-01', { years: 1 }]
].forEach(function (c) {
  t(c[0] + ' ' + c[1] + ' + ' + JSON.stringify(c[2]), function () {
    var d = Temporal.PlainDate.from(c[1]).withCalendar(c[0]); var e = d.add(c[2]);
    return e.year + '/' + e.monthCode + '/' + e.day + ' ' + e.toString({ calendarName: 'never' }) + ' until months ' + d.until(e, { largestUnit: 'months' }).toString(); });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
