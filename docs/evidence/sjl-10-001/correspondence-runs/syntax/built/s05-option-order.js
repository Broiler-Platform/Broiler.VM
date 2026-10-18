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
// S28-S30: the order in which each toString reads its options, and when it validates them: every option a getter
// that logs its name, then a valid set, then an invalid smallestUnit with later options still read or not.
function logged(values) {
  var log = [], o = {};
  Object.keys(values).forEach(function (key) {
    Object.defineProperty(o, key, { get: function () { log.push(key); return values[key]; }, enumerable: true });
  });
  return { o: o, log: log };
}
var cases = {
  PlainDate: [Temporal.PlainDate.from('2020-01-01'), { calendarName: 'auto' }],
  PlainYearMonth: [Temporal.PlainYearMonth.from('2020-01'), { calendarName: 'auto' }],
  PlainMonthDay: [Temporal.PlainMonthDay.from('01-01'), { calendarName: 'auto' }],
  PlainTime: [Temporal.PlainTime.from('12:00'), { fractionalSecondDigits: 'auto', roundingMode: 'trunc', smallestUnit: 'second' }],
  PlainDateTime: [Temporal.PlainDateTime.from('2020-01-01T12:00'), { calendarName: 'auto', fractionalSecondDigits: 'auto', roundingMode: 'trunc', smallestUnit: 'second' }],
  Instant: [Temporal.Instant.from('2020-01-01T12:00Z'), { fractionalSecondDigits: 'auto', roundingMode: 'trunc', smallestUnit: 'second', timeZone: 'UTC' }],
  ZonedDateTime: [Temporal.ZonedDateTime.from('2020-01-01T12:00[UTC]'), { calendarName: 'auto', fractionalSecondDigits: 'auto', offset: 'auto', roundingMode: 'trunc', smallestUnit: 'second', timeZoneName: 'auto' }],
  Duration: [Temporal.Duration.from('PT1S'), { fractionalSecondDigits: 'auto', roundingMode: 'trunc', smallestUnit: 'second' }]
};
Object.keys(cases).forEach(function (type) {
  var subject = cases[type][0], values = cases[type][1];
  // The names in reverse, so that the order read is the engine's and not the object's.
  var rev = {}; Object.keys(values).reverse().forEach(function (key) { rev[key] = values[key]; });
  var l = logged(rev);
  t(type + ' reads', function () { var s = subject.toString(l.o); return l.log.join(',') + ' => ' + s; });
  ['smallestUnit', 'roundingMode', 'fractionalSecondDigits', 'calendarName', 'offset', 'timeZoneName', 'timeZone'].forEach(function (bad) {
    if (!(bad in values)) return;
    var v = {}; Object.keys(rev).forEach(function (key) { v[key] = key === bad ? (bad === 'smallestUnit' ? 'year' : bad === 'fractionalSecondDigits' ? 10 : bad === 'timeZone' ? 'Mars/Olympus' : 'bogus') : rev[key]; });
    var lb = logged(v);
    k(type + ' with an invalid ' + bad + ', reads', function () { try { subject.toString(lb.o); } finally { out('  ' + type + ' invalid ' + bad + ' read: ' + lb.log.join(',')); } });
  });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
