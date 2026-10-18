/*---
description: SJL-10.1 probe c02-month-codes
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
// C4-C9: month codes, valid and invalid, in each calendar and in iso8601, under both overflow settings, through
// PlainDate.from with a year (the leap year LEAPYEAR names), and through PlainYearMonth.from.
var codes = ['M00', 'M00L', 'M01', 'M01L', 'M02L', 'M05L', 'M06L', 'M12', 'M12L', 'M13', 'M13L', 'M14', 'M99', 'm01', 'M1', 'M001',
  'M01l', 'L01M', 'M01 ', ''];
['iso8601'].concat(CALS).forEach(function (cal) {
  codes.forEach(function (code) {
    ['constrain', 'reject'].forEach(function (overflow) {
      k(cal + ' ' + LEAPYEAR[cal] + ' ' + JSON.stringify(code) + ' ' + overflow, function () {
        return ymd(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], monthCode: code, day: 1 }, { overflow: overflow })); });
    });
  });
});
// A leap month code in a year without that leap month: Hebrew 5783, Chinese and Korean 2024.
[['hebrew', 5783, 'M05L'], ['chinese', 2024, 'M02L'], ['dangi', 2024, 'M02L'], ['chinese', 2023, 'M03L'], ['chinese', 2023, 'M02L']
].forEach(function (c) {
  ['constrain', 'reject'].forEach(function (overflow) {
    k(c[0] + ' ' + c[1] + ' ' + c[2] + ' ' + overflow, function () {
      return ymd(Temporal.PlainDate.from({ calendar: c[0], year: c[1], monthCode: c[2], day: 1 }, { overflow: overflow })); });
    k('PlainYearMonth ' + c[0] + ' ' + c[1] + ' ' + c[2] + ' ' + overflow, function () {
      var d = Temporal.PlainYearMonth.from({ calendar: c[0], year: c[1], monthCode: c[2] }, { overflow: overflow });
      return d.year + '/' + d.monthCode + ' ' + d.toString(); });
  });
});
// month and monthCode together; a month beyond the year's months.
[['hebrew', 5784, 6, 'M05L'], ['hebrew', 5784, 7, 'M06'], ['hebrew', 5783, 6, 'M06'], ['chinese', 2023, 3, 'M02L'], ['chinese', 2023, 3, 'M03'],
 ['coptic', 1739, 13, 'M13'], ['iso8601', 2024, 1, 'M02']].forEach(function (c) {
  k(c[0] + ' ' + c[1] + ' month ' + c[2] + ' monthCode ' + c[3], function () {
    return ymd(Temporal.PlainDate.from({ calendar: c[0], year: c[1], month: c[2], monthCode: c[3], day: 1 })); });
});
CALS.concat(['iso8601']).forEach(function (cal) {
  ['constrain', 'reject'].forEach(function (overflow) {
    k(cal + ' month 14 ' + overflow, function () { return ymd(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], month: 14, day: 1 }, { overflow: overflow })); });
  });
});
// The month codes a date answers in a leap year, month by month.
['hebrew', 'chinese', 'dangi', 'coptic', 'ethiopic'].forEach(function (cal) {
  t(cal + ' ' + LEAPYEAR[cal] + ' monthCodes', function () {
    var r = [], first = Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], month: 1, day: 1 });
    for (var m = 1; m <= first.monthsInYear; m++) r.push(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], month: m, day: 1 }).monthCode);
    return r.join(' '); });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
