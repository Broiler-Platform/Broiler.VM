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
// S23-S27: Duration's string form: zero, signs, every field, sub-second fields beyond a second, the limits, rounding
// that carries into larger units, and precision against auto.
[{}, { years: 1 }, { years: -1 }, { weeks: 3, days: 4 }, { hours: 25 }, { minutes: 61 }, { seconds: 61 }, { milliseconds: 1500 },
 { milliseconds: 1000 }, { microseconds: 1000001 }, { nanoseconds: 1000000001 }, { seconds: 1, milliseconds: 999, microseconds: 999, nanoseconds: 1000 },
 { milliseconds: 0.0 }, { years: 4294967295 }, { months: 4294967295 }, { weeks: 4294967295 }, { days: 104249991374 }, { hours: 2501999792983 },
 { minutes: 150119987579016 }, { seconds: 9007199254740991 }, { milliseconds: 9007199254740991 }, { microseconds: 9007199254740991 },
 { nanoseconds: 9007199254740991 }, { seconds: 9007199254740991, nanoseconds: 999999999 }, { milliseconds: 9007199254740991999 },
 { days: -1, hours: -1 }, { hours: 1, nanoseconds: 1 }, { minutes: 1, seconds: 0 }, { seconds: 0.5e-9 * 0 }].forEach(function (f) {
  k('Duration ' + JSON.stringify(f), function () { return Temporal.Duration.from(f).toString(); });
  k('Duration ' + JSON.stringify(f) + ' fractionalSecondDigits 0', function () { return Temporal.Duration.from(f).toString({ fractionalSecondDigits: 0 }); });
  k('Duration ' + JSON.stringify(f) + ' fractionalSecondDigits 3', function () { return Temporal.Duration.from(f).toString({ fractionalSecondDigits: 3 }); });
});
// Rounding that carries.
[['PT59.999999999S', 'ceil', 0], ['PT59.5S', 'halfExpand', 0], ['PT59M59.5S', 'halfExpand', 0], ['PT23H59M59.5S', 'halfExpand', 0],
 ['P1DT23H59M59.5S', 'halfExpand', 0], ['P1WT23H59M59.5S', 'halfExpand', 0], ['P1YT23H59M59.5S', 'halfExpand', 0], ['PT0.5S', 'halfEven', 0],
 ['PT1.5S', 'halfEven', 0], ['-PT59.5S', 'halfExpand', 0], ['-PT59.5S', 'floor', 0], ['-PT59.5S', 'ceil', 0], ['PT1H59M59.999S', 'ceil', 2],
 ['PT2501999792983H59M59.999999999S', 'ceil', 0], ['PT9007199254740991.999999999S', 'ceil', 0], ['PT9007199254740991.5S', 'halfExpand', 0],
 ['PT9007199254740990.5S', 'halfEven', 0]].forEach(function (c) {
  k('Duration ' + c[0] + ' ' + c[1] + ' fractionalSecondDigits ' + c[2], function () {
    return Temporal.Duration.from(c[0]).toString({ roundingMode: c[1], fractionalSecondDigits: c[2] }); });
  k('Duration ' + c[0] + ' ' + c[1] + ' smallestUnit second', function () {
    return Temporal.Duration.from(c[0]).toString({ roundingMode: c[1], smallestUnit: 'second' }); });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
