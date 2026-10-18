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
// S1-S8: every type's string form at notable years and in calendars other than ISO, under each calendarName, and
// toJSON, which is toString with no options.
var years = [0, 1, 999, 1000, 9999, 10000, -1, -9999, -10000, 123456, -123456];
years.forEach(function (y) {
  t('PlainDate ' + y, function () { return new Temporal.PlainDate(y, 1, 2).toString(); });
  t('PlainYearMonth ' + y, function () { return new Temporal.PlainYearMonth(y, 3).toString(); });
  t('PlainDateTime ' + y, function () { return new Temporal.PlainDateTime(y, 1, 2, 3, 4, 5, 6, 7, 8).toString(); });
  t('Instant ' + y, function () { return Temporal.Instant.from(new Temporal.PlainDate(y, 1, 2).toString() + 'T03:04:05.000000006Z').toString(); });
  t('ZonedDateTime ' + y, function () { return new Temporal.PlainDateTime(y, 1, 2, 3, 4, 5).toZonedDateTime('+01:00').toString(); });
});
t('PlainDate min', function () { return new Temporal.PlainDate(-271821, 4, 19).toString(); });
t('PlainDate max', function () { return new Temporal.PlainDate(275760, 9, 13).toString(); });
t('PlainYearMonth min', function () { return new Temporal.PlainYearMonth(-271821, 4).toString(); });
t('Instant min', function () { return new Temporal.Instant(-8640000000000000000000n).toString(); });
t('Instant max', function () { return new Temporal.Instant(8640000000000000000000n).toString(); });
t('PlainMonthDay ISO with reference 1972', function () { return new Temporal.PlainMonthDay(2, 29).toString(); });
t('PlainMonthDay ISO with reference 2000', function () { return new Temporal.PlainMonthDay(2, 29, 'iso8601', 2000).toString(); });
t('PlainYearMonth ISO with reference day 15', function () { return new Temporal.PlainYearMonth(2020, 2, 'iso8601', 15).toString(); });
['auto', 'always', 'never', 'critical', undefined, 'x', 'AUTO', '', null, 1, true].forEach(function (cn) {
  var o = { calendarName: cn }, label = ' calendarName ' + JSON.stringify(cn);
  ['iso8601', 'gregory', 'hebrew', 'chinese'].forEach(function (cal) {
    k('PlainDate ' + cal + label, function () { return Temporal.PlainDate.from('2020-03-15').withCalendar(cal).toString(o); });
    k('PlainDateTime ' + cal + label, function () { return Temporal.PlainDateTime.from('2020-03-15T12:00').withCalendar(cal).toString(o); });
    k('PlainYearMonth ' + cal + label, function () { return Temporal.PlainYearMonth.from({ calendar: cal, year: Temporal.PlainDate.from('2020-03-15').withCalendar(cal).year, monthCode: 'M02' }).toString(o); });
    k('PlainMonthDay ' + cal + label, function () { return Temporal.PlainMonthDay.from({ calendar: cal, monthCode: 'M02', day: 10 }).toString(o); });
    k('ZonedDateTime ' + cal + label, function () { return Temporal.ZonedDateTime.from('2020-03-15T12:00[UTC]').withCalendar(cal).toString(o); });
  });
});
// toJSON against toString, and toString's options argument when not an object.
t('PlainDate toJSON', function () { return Temporal.PlainDate.from('2020-03-15[u-ca=hebrew]').toJSON(); });
t('PlainYearMonth toJSON', function () { return Temporal.PlainYearMonth.from('2020-03-15[u-ca=hebrew]').toJSON(); });
t('PlainMonthDay toJSON', function () { return Temporal.PlainMonthDay.from('2020-03-15[u-ca=hebrew]').toJSON(); });
t('PlainMonthDay ISO toJSON', function () { return Temporal.PlainMonthDay.from('03-15').toJSON(); });
t('PlainDateTime toJSON', function () { return Temporal.PlainDateTime.from('2020-03-15T12:00:00.5[u-ca=japanese]').toJSON(); });
t('PlainTime toJSON', function () { return Temporal.PlainTime.from('12:00:00.000001').toJSON(); });
t('ZonedDateTime toJSON', function () { return Temporal.ZonedDateTime.from('2020-03-15T12:00:00.1+05:30[Asia/Kolkata][u-ca=roc]').toJSON(); });
t('Instant toJSON', function () { return Temporal.Instant.from('2020-03-15T12:00:00.12Z').toJSON(); });
t('Duration toJSON', function () { return Temporal.Duration.from('P1Y2M3W4DT5H6M7.008009010S').toJSON(); });
['PlainDate', 'PlainTime', 'Duration', 'Instant'].forEach(function (type) {
  [null, 1, 'x', true].forEach(function (o) {
    k(type + '#toString(' + JSON.stringify(o) + ')', function () {
      var v = type === 'PlainDate' ? Temporal.PlainDate.from('2020-01-01') : type === 'PlainTime' ? Temporal.PlainTime.from('12:00')
        : type === 'Duration' ? Temporal.Duration.from('PT1S') : Temporal.Instant.from('2020-01-01T00:00Z');
      return v.toString(o); });
  });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
