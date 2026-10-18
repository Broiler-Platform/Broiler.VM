/*---
description: SJL-10.1 probe c04-reference-years
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
// C16-C20: PlainMonthDay's ISO reference date for a month code and day with no year, in every calendar: the latest
// such ISO date in 1900-1972, else the earliest in 1973-2035 (else before 1900), and for chinese and dangi the
// proposal's Table 6. Under reject the day must exist; under constrain it is constrained.
function refdate(cal, code, day, overflow) {
  var md = Temporal.PlainMonthDay.from({ calendar: cal, monthCode: code, day: day }, { overflow: overflow });
  return md.monthCode + '/' + md.day + ' ref ' + md.toString({ calendarName: 'never' });
}
['iso8601'].concat(CALS).forEach(function (cal) {
  var codes = ['M01', 'M02', 'M03', 'M04', 'M05', 'M06', 'M07', 'M08', 'M09', 'M10', 'M11', 'M12', 'M13'];
  if (cal === 'chinese' || cal === 'dangi') codes = codes.concat(['M01L', 'M02L', 'M03L', 'M04L', 'M05L', 'M06L', 'M07L', 'M08L', 'M09L', 'M10L', 'M11L', 'M12L']);
  if (cal === 'hebrew') codes = codes.concat(['M05L']);
  codes.forEach(function (code) {
    [1, 29, 30, 31].forEach(function (day) {
      k(cal + ' ' + code + ' ' + day + ' reject', function () { return refdate(cal, code, day, 'reject'); });
    });
    [30, 31, 32].forEach(function (day) {
      k(cal + ' ' + code + ' ' + day + ' constrain', function () { return refdate(cal, code, day, 'constrain'); });
    });
  });
});
// Table 6's M11L split at day 10 (2033 or 2034), and a year given with the month-day (dropped after resolving).
[['chinese', 'M11L', 10], ['chinese', 'M11L', 11], ['chinese', 'M11L', 29], ['dangi', 'M11L', 10]].forEach(function (c) {
  k(c[0] + ' ' + c[1] + ' ' + c[2] + ' reject', function () { return refdate(c[0], c[1], c[2], 'reject'); }); });
[['hebrew', 5783, 'M05L', 1], ['hebrew', 5784, 'M05L', 30], ['chinese', 2023, 'M02L', 30], ['chinese', 2024, 'M02L', 1], ['coptic', 1739, 'M13', 6],
 ['coptic', 1740, 'M13', 6], ['islamic-civil', 1445, 'M12', 30], ['islamic-civil', 1444, 'M12', 30], ['persian', 1403, 'M12', 30], ['persian', 1404, 'M12', 30],
 ['indian', 1946, 'M01', 31], ['gregory', 2021, 'M02', 29], ['iso8601', 2021, 'M02', 29]].forEach(function (c) {
  ['constrain', 'reject'].forEach(function (overflow) {
    k(c[0] + ' year ' + c[1] + ' ' + c[2] + ' ' + c[3] + ' ' + overflow, function () {
      var md = Temporal.PlainMonthDay.from({ calendar: c[0], year: c[1], monthCode: c[2], day: c[3] }, { overflow: overflow });
      return md.monthCode + '/' + md.day + ' ref ' + md.toString({ calendarName: 'never' }); });
  });
});
// A month number with no year (a TypeError where the calendar is not ISO), and a year far out of range.
[['hebrew', { month: 6, day: 1 }], ['gregory', { month: 2, day: 29 }], ['iso8601', { month: 2, day: 29 }], ['chinese', { month: 1, day: 1 }],
 ['hebrew', { year: 1e10, monthCode: 'M01', day: 1 }], ['iso8601', { year: 1e10, monthCode: 'M01', day: 1 }], ['gregory', { year: 1e10, monthCode: 'M01', day: 1 }],
 ['chinese', { year: 1e10, monthCode: 'M01', day: 1 }], ['hebrew', { year: 300000, monthCode: 'M01', day: 1 }]].forEach(function (c) {
  k(c[0] + ' ' + JSON.stringify(c[1]), function () {
    var bag = { calendar: c[0] }; for (var key in c[1]) bag[key] = c[1][key];
    var md = Temporal.PlainMonthDay.from(bag); return md.monthCode + '/' + md.day + ' ref ' + md.toString({ calendarName: 'never' }); });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
