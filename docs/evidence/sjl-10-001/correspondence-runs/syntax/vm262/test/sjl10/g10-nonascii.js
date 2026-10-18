/*---
description: SJL-10.1 probe g10-nonascii
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
// G47-G48: decimal digits other than ASCII (Arabic-Indic U+0660-U+0669, fullwidth U+FF10-U+FF19), and the sign
// U+2212 MINUS SIGN, in each parser; the error's type and message. The grammar's digits are ASCII
// (DecimalDigit), its signs ASCIISign (U+2212 left the grammar with the proposal's ae9c7855, 2024-07-04).
var AI = function (s) { return s.replace(/[0-9]/g, function (c) { return String.fromCharCode(0x0660 + +c); }); };
var FW = function (s) { return s.replace(/[0-9]/g, function (c) { return String.fromCharCode(0xFF10 + +c); }); };
var cases = [
  ['PlainDate', '2020-01-01'], ['PlainDateTime', '2020-01-01T12:00'], ['PlainTime', '12:30'], ['PlainYearMonth', '2020-01'],
  ['PlainMonthDay', '01-01'], ['ZonedDateTime', '2020-01-01T12:00[UTC]'], ['Instant', '2020-01-01T12:00Z'], ['Duration', 'P1D']];
cases.forEach(function (c) {
  t1(c[0] + '.from(Arabic-Indic ' + JSON.stringify(AI(c[1])) + ')', function () { return FROM[c[0]](AI(c[1])); });
  t1(c[0] + '.from(fullwidth ' + JSON.stringify(FW(c[1])) + ')', function () { return FROM[c[0]](FW(c[1])); });
});
// Digits other than ASCII in one part only.
t1('PlainDate.from("2020-01-0١")', function () { return Temporal.PlainDate.from('2020-01-0١').toString(); });
t1('PlainDateTime.from("2020-01-01T12:0٠")', function () { return Temporal.PlainDateTime.from('2020-01-01T12:0٠').toString(); });
t1('PlainDateTime.from("2020-01-01T12:00:00.١")', function () { return Temporal.PlainDateTime.from('2020-01-01T12:00:00.١').toString(); });
t1('Instant.from("2020-01-01T12:00+0١:00")', function () { return Temporal.Instant.from('2020-01-01T12:00+0١:00').toString(); });
t1('ZonedDateTime.from("2020-01-01T12:00+0١:00[UTC]")', function () { return Temporal.ZonedDateTime.from('2020-01-01T12:00+0١:00[UTC]').toString(); });
t1('PlainDate.from("2020-01-01[u-ca=١]")', function () { return Temporal.PlainDate.from('2020-01-01[u-ca=١]').toString(); });
t1('PlainDate.from("2020-01-01[١=a]")', function () { return Temporal.PlainDate.from('2020-01-01[١=a]').toString(); });
t1('Duration.from("PT1.٥S")', function () { return Temporal.Duration.from('PT1.٥S').toString(); });
// U+2212 in the year, the offset and the duration sign, in each parser.
var M = '−';
[['PlainDate', M + '002020-01-01'], ['PlainDateTime', M + '002020-01-01T00:00'], ['PlainYearMonth', M + '002020-01'],
 ['PlainMonthDay', M + '002020-01-01'], ['ZonedDateTime', M + '002020-01-01T00:00[UTC]'], ['Instant', M + '002020-01-01T00:00Z'],
 ['PlainDate', '2020-01-01T00:00' + M + '01:00'], ['PlainDateTime', '2020-01-01T00:00' + M + '01:00'], ['PlainTime', '00:00' + M + '01:00'],
 ['PlainYearMonth', '2020-01-01T00:00' + M + '01:00'], ['PlainMonthDay', '2020-01-01T00:00' + M + '01:00'],
 ['ZonedDateTime', '2020-01-01T00:00' + M + '01:00[UTC]'], ['Instant', '2020-01-01T00:00' + M + '01:00'],
 ['ZonedDateTime', '2020-01-01T00:00[' + M + '01:00]'], ['Duration', M + 'P1D']
].forEach(function (c) { t1(c[0] + '.from(' + JSON.stringify(c[1]) + ')', function () { return FROM[c[0]](c[1]); }); });
t1('new ZonedDateTime(0n, "−01:00")', function () { return new Temporal.ZonedDateTime(0n, M + '01:00').toString(); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
