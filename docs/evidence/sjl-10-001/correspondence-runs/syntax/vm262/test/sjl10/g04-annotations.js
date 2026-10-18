/*---
description: SJL-10.1 probe g04-annotations
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
// G14-G22: RFC 9557 annotations. The time zone annotation (first, at most once, its grammar), critical flags,
// unknown keys, the calendar annotation (u-ca) once or several times, key and value grammar, empty and broken
// brackets, the order of annotations.
var anns = ['[u-ca=iso8601]', '[!u-ca=iso8601]', '[u-ca=gregory][u-ca=japanese]', '[u-ca=gregory][!u-ca=japanese]',
  '[!u-ca=gregory][u-ca=japanese]', '[u-ca=iso8601][!u-ca=iso8601]', '[foo=bar]', '[!foo=bar]', '[U-CA=iso8601]',
  '[u-ca=ISO8601]', '[u-ca=iso_8601]', '[u-ca=]', '[=iso8601]', '[]', '[!]', '[u-ca=iso8601', 'u-ca=iso8601]',
  '[u-ca=iso8601]]', '[[u-ca=iso8601]', '[UTC]', '[!UTC]', '[UTC][UTC]', '[UTC][u-ca=iso8601]', '[u-ca=iso8601][UTC]',
  '[foo=bar][UTC]', '[Europe/Berlin]', '[Mars/Olympus]', '[Mars Olympus]', '[+01:00]', '[+01:00:00]', '[-07:00:01]',
  '[+0100]', '[+01]', '[+24:00]', '[a=b]', '[_x=y]', '[x-y=z]', '[1a=b]', '[-a=b]', '[a=b-c]', '[a=b--c]', '[a=-b]',
  '[a=b-]', '[a=b_c]', '[aBc=d]', '[a=b=c]', '[foo=bar][u-ca=gregory]', '[u-ca=gregory][foo=bar][!u-ca=japanese]',
  '[.]', '[..]', '[a/./b]', '[Etc/GMT+5]', '[EST5EDT]', '[a..b]', '[/a]', '[a/]', '[a//b]', '[9a]', '[a-b+c_d.e]',
  '[ UTC]', '[UTC ]', '[u-ca=gregory] ', '[u-ca=١]', '[UTC][!u-ca=iso8601][u-ca=gregory]', '[!foo=bar][u-ca=iso8601]',
  '[u-ca=chinese]', '[u-ca=unknowncal]', '[u-ca=islamic]'];
anns.forEach(function (a) {
  each(['PlainDate', 'PlainDateTime', 'PlainYearMonth', 'PlainMonthDay'], '2020-01-01T12:00' + a);
  each(['PlainTime'], '12:00' + a);
  each(['ZonedDateTime'], '2020-01-01T12:00[UTC]' + a);
  each(['Instant'], '2020-01-01T12:00Z' + a);
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
