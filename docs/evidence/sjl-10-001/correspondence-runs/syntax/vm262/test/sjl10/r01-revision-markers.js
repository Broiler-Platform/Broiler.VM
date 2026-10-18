/*---
description: SJL-10.1 probe r01-revision-markers
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
// R1-R4 (Facts): behaviours that changed between revisions of the Temporal proposal (tc39/proposal-temporal) and of
// the Intl era and month code proposal (tc39/proposal-intl-era-monthcode), each labelled with the commit that
// changed it. "after" is the pinned revision e8cc03fc's behaviour.
function ty(label, f) { t(label, function () { return typeof f(); }); }
// The API surface the proposal removed (2024-06-13 to 2024-09-05) or added.
ty('Temporal.Calendar (removed b8892420, 2024-09-05)', function () { return Temporal.Calendar; });
ty('Temporal.TimeZone (removed b8892420, 2024-09-05)', function () { return Temporal.TimeZone; });
ty('PlainDate.prototype.getISOFields (removed 7899d593, 2024-09-05)', function () { return Temporal.PlainDate.prototype.getISOFields; });
ty('PlainDate.prototype.getCalendar (removed 92b19abd, 2024-07-19)', function () { return Temporal.PlainDate.prototype.getCalendar; });
ty('ZonedDateTime.prototype.getTimeZone (removed 92b19abd, 2024-07-19)', function () { return Temporal.ZonedDateTime.prototype.getTimeZone; });
ty('ZonedDateTime.prototype.getTimeZoneTransition (added 133921b2, 2024-07-19)', function () { return Temporal.ZonedDateTime.prototype.getTimeZoneTransition; });
ty('PlainDateTime.prototype.withPlainDate (removed f7acbd2f, 2024-06-13)', function () { return Temporal.PlainDateTime.prototype.withPlainDate; });
ty('PlainTime.prototype.toPlainDateTime (removed f81d05fa, 2024-06-13)', function () { return Temporal.PlainTime.prototype.toPlainDateTime; });
ty('PlainTime.prototype.toZonedDateTime (removed f81d05fa, 2024-06-13)', function () { return Temporal.PlainTime.prototype.toZonedDateTime; });
ty('Instant.prototype.epochSeconds (removed a6cffb25, 2024-06-13)', function () { return Temporal.Instant.from('2020-01-01T00:00Z').epochSeconds; });
ty('Instant.prototype.epochMicroseconds (removed a6cffb25, 2024-06-13)', function () { return Temporal.Instant.from('2020-01-01T00:00Z').epochMicroseconds; });
ty('Instant.fromEpochSeconds (removed a6cffb25, 2024-06-13)', function () { return Temporal.Instant.fromEpochSeconds; });
ty('PlainDateTime.prototype.toPlainYearMonth (removed efebfa2c, 2024-06-13)', function () { return Temporal.PlainDateTime.prototype.toPlainYearMonth; });
ty('ZonedDateTime.prototype.toPlainMonthDay (removed efebfa2c, 2024-06-13)', function () { return Temporal.ZonedDateTime.prototype.toPlainMonthDay; });
ty('Temporal.Now.plainDate (removed 2ae23dde, 2024-06-13)', function () { return Temporal.Now.plainDate; });
ty('Temporal.Now.plainDateISO', function () { return Temporal.Now.plainDateISO; });
ty('Temporal.Now.zonedDateTime (removed 2ae23dde, 2024-06-13)', function () { return Temporal.Now.zonedDateTime; });
ty('PlainDate.prototype.toPlainYearMonth (kept)', function () { return Temporal.PlainDate.prototype.toPlainYearMonth; });
ty('PlainDate.prototype.yearOfWeek (added 7fa4d189, 2022-11-29)', function () { return Temporal.PlainDate.from('2020-01-01').yearOfWeek; });
t('Temporal[Symbol.toStringTag]', function () { return Temporal[Symbol.toStringTag]; });
t('Object.getOwnPropertyNames(Temporal)', function () { return Object.getOwnPropertyNames(Temporal).sort().join(' '); });
// Behaviours.
k('Duration#add with a calendar unit and relativeTo (relativeTo removed 8ae0dbd7, 2024-06-13)', function () { return Temporal.Duration.from('P1Y').add('P1M', { relativeTo: '2020-01-01' }).toString(); });
k('Duration.from({years: 2**32}) (limited 1104cad8, 2024-01-31)', function () { return Temporal.Duration.from({ years: 4294967296 }).toString(); });
k('ZonedDateTime offset with a nanosecond that does not match (strict 1117eaf8, 2025-06-02)', function () { return Temporal.ZonedDateTime.from('2020-01-01T00:00+01:00:00.000000001[Europe/Berlin]').toString(); });
k('PlainYearMonth#add({days: 1}) (refused 41faca47, 2026-01-29)', function () { return Temporal.PlainYearMonth.from('2020-01').add({ days: 1 }).toString(); });
k('PlainYearMonth#add({weeks: 1}) (refused 41faca47, 2026-01-29)', function () { return Temporal.PlainYearMonth.from('2020-01').add({ weeks: 1 }).toString(); });
k('PlainYearMonth#add({hours: 1}) (refused 41faca47, 2026-01-29)', function () { return Temporal.PlainYearMonth.from('2020-01').add({ hours: 1 }).toString(); });
k('PlainYearMonth#add({months: 1, days: 0}) (41faca47, 2026-01-29)', function () { return Temporal.PlainYearMonth.from('2020-01').add({ months: 1, days: 0 }).toString(); });
k('PlainYearMonth#subtract({days: 40})', function () { return Temporal.PlainYearMonth.from('2020-03').subtract({ days: 40 }).toString(); });
k('PlainMonthDay.from("+999999-01-01[u-ca=gregory]") (refused bd5ac121, 2025-02-21)', function () { return Temporal.PlainMonthDay.from('+999999-01-01[u-ca=gregory]').toString(); });
k('PlainMonthDay.from("+999999-01-01") (ISO, allowed bd5ac121)', function () { return Temporal.PlainMonthDay.from('+999999-01-01').toString(); });
k('PlainMonthDay.from("11-18[+01:00][u-ca=ISO8601]") (polyfill 385b479b, 2026-02-26)', function () { return Temporal.PlainMonthDay.from('11-18[+01:00][u-ca=ISO8601]').toString(); });
k('PlainYearMonth.from("1976-11[u-ca=ISO8601]") (polyfill 385b479b, 2026-02-26)', function () { return Temporal.PlainYearMonth.from('1976-11[u-ca=ISO8601]').toString(); });
k('PlainDate.from("2020-01-01[u-ca=ISO8601]").calendarId (canonicalised ba56a48a, 2024-07-01)', function () { return Temporal.PlainDate.from('2020-01-01[u-ca=ISO8601]').calendarId; });
k('PlainMonthDay chinese M11L 1 reference (1973-2035 allowed 10f83f39, 2025-09-23; Table 6 ad5028b, 2026-01-21)', function () {
  return Temporal.PlainMonthDay.from({ calendar: 'chinese', monthCode: 'M11L', day: 1 }, { overflow: 'reject' }).toString({ calendarName: 'never' }); });
k('PlainMonthDay hebrew year 1e10 (polyfill 69a60a2a, 2026-05-11)', function () { return Temporal.PlainMonthDay.from({ calendar: 'hebrew', year: 1e10, monthCode: 'M01', day: 1 }).toString(); });
k('PlainDate.from("−002020-01-01") (U+2212 reverted ae9c7855, 2024-07-04)', function () { return Temporal.PlainDate.from('−002020-01-01').toString(); });
k('new ZonedDateTime(0n, "+01:00:01") (minute offsets 70bc509f, 2023-07-18)', function () { return new Temporal.ZonedDateTime(0n, '+01:00:01').toString(); });
k('Duration#round with relativeTo "2022-08-18T17:01Z" (refused 2dc20bf6, 2022-10-11)', function () { return Temporal.Duration.from('P1D').round({ smallestUnit: 'day', relativeTo: '2022-08-18T17:01Z' }).toString(); });
// The Intl era and month code proposal's era codes: changed 08de50a (2025-06-30), aliases cut 684dd16 (2025-11-21).
[['gregory', 'gregory-inverse'], ['gregory', 'gregory'], ['roc', 'minguo'], ['roc', 'before-roc'], ['roc', 'minguo-qian'], ['roc', 'roc-inverse'],
 ['ethiopic', 'incar'], ['ethiopic', 'mundi'], ['ethiopic', 'ethiopic-amete-alem'], ['coptic', 'coptic'], ['coptic', 'coptic-inverse'], ['indian', 'saka'],
 ['persian', 'persian'], ['japanese', 'japanese'], ['japanese', 'japanese-inverse'], ['islamic-civil', 'islamicc'], ['hebrew', 'hebrew']].forEach(function (c) {
  k('era ' + c[1] + ' in ' + c[0], function () { return Temporal.PlainDate.from({ calendar: c[0], era: c[1], eraYear: 5, monthCode: 'M01', day: 1 }).era; });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
