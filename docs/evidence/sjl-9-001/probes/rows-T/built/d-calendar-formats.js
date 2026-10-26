// Shared prelude for SJL-9.1's probes (SJL-11.1's form, sjl-11-001/correspondence-runs/*/probes/prelude.js,
// with the regular-expression helpers left out and two Intl helpers added). Broiler.JS's shell has print;
// Broiler.VM's test262 realm has none, so there the lines are collected and handed back in one uncaught
// Error the conformance root prints (epilogue.js). The command-line root has print.
var __lines = [];
var __host = typeof __vmTest === 'undefined' && typeof print === 'function';
function out(s) { s = String(s); if (__host) print(s); else __lines.push(s); }
// A string is printed quoted, every code unit outside printable ASCII (and " and \) as \uXXXX,
// so U+202F, U+00A0, a lone surrogate or a line terminator is visible and the output stays ASCII.
function q(s) {
  var r = '';
  for (var i = 0; i < s.length; i++) {
    var c = s.charCodeAt(i);
    if (c < 0x20 || c > 0x7e || c === 0x22 || c === 0x5c) r += '\\u' + ('0000' + c.toString(16)).slice(-4);
    else r += s.charAt(i);
  }
  return '"' + r + '"';
}
// An engine's error message, unquoted, with every code unit outside printable ASCII as \uXXXX.
function plain(s) {
  var r = '';
  for (var i = 0; i < s.length; i++) {
    var c = s.charCodeAt(i);
    r += c < 0x20 || c > 0x7e ? '\\u' + ('0000' + c.toString(16)).slice(-4) : s.charAt(i);
  }
  return r;
}
// Objects are shown with their own enumerable keys in the engine's order (resolvedOptions' order
// is specified, so it is shown as the engine gives it).
function show(v) {
  if (v === null) return 'null';
  if (v === undefined) return 'undefined';
  if (v && v.__raw !== undefined) return v.__raw;
  if (typeof v === 'string') return q(v);
  if (typeof v === 'number') return Object.is(v, -0) ? '-0' : String(v);
  if (typeof v === 'boolean' || typeof v === 'bigint') return String(v);
  if (typeof v === 'function') return 'function ' + (v.name || '(anonymous)');
  if (Array.isArray(v)) { var a = []; for (var i = 0; i < v.length; i++) a.push(show(v[i])); return '[' + a.join(', ') + ']'; }
  if (typeof v === 'object') {
    var k = Object.keys(v), p = [];
    for (var j = 0; j < k.length; j++) p.push(k[j] + ': ' + show(v[k[j]]));
    return '{' + p.join(', ') + '}';
  }
  return String(v);
}
function R(s) { return { __raw: s }; }
function errName(e) {
  return (e && e.constructor && e.constructor.name) || typeof e;
}
// One line per behaviour: the label, then the value or the error's constructor and message.
function t(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + errName(e) + ': ' + plain(String(e && e.message)); }
  out(label + ' => ' + r);
}
// formatToParts / formatRangeToParts as type:"value" pairs (source too, when present).
function parts(p) {
  var a = [];
  for (var i = 0; i < p.length; i++) a.push(p[i].type + (p[i].source !== undefined ? '/' + p[i].source : '') + ':' + q(String(p[i].value)));
  return R('[' + a.join(', ') + ']');
}
// An options bag that records every property read, in order, for observable-order rows.
function spy(bag, log) {
  var keys = Object.keys(bag);
  var o = {};
  keys.forEach(function (k) {
    Object.defineProperty(o, k, { enumerable: true, get: function () { log.push(k); return bag[k]; } });
  });
  return new Proxy(o, { get: function (target, k, r) { if (typeof k === 'string' && keys.indexOf(k) < 0) log.push(k); return Reflect.get(target, k, r); } });
}
// Rows T: Broiler.VM's reference dataset program src/tests/temporal/calendars-intl/calendar-formats.js at cd1928d, byte for byte as a
// string, evaluated by indirect eval as its README says (the completion value is its lines).
// The prelude's helpers are kept under names of their own first, in case the program declares globals.
var __out = out, __errName = errName, __plain = plain;
var __text = "// SPDX-FileCopyrightText: 2026 Broiler Platform contributors\n// SPDX-License-Identifier: Apache-2.0\n//\n// Intl.DateTimeFormat in the CLDR calendars (JSD-0057): each line one formatting and its answer, or\n// the name of the error it threw. It uses no host function and completes with its lines, so the\n// reference polyfill on Node and the profile run the same text.\n(function () {\n  var lines = [];\n  function line(label, f) {\n    var answer;\n    try { answer = String(f()); } catch (e) { answer = 'throws ' + e.name; }\n    lines.push(label + ' => ' + answer);\n  }\n  function parts(list) {\n    return list.map(function (part) { return part.type + ':' + part.value + (part.source ? '@' + part.source : ''); }).join('|');\n  }\n\n  var calendars = ['buddhist', 'chinese', 'coptic', 'dangi', 'ethioaa', 'ethiopic', 'hebrew', 'indian', 'islamic-civil',\n                   'islamic-tbla', 'islamic-umalqura', 'japanese', 'persian', 'roc'];\n  var locales = ['en', 'de'];\n\n  // Dates chosen for each calendar's months, leap months, eras and new years: a Chinese leap month\n  // (2020 and 2023), the Hebrew Adar I and II of 5784 and the Adar of 5785, the last day of Showa,\n  // the first of Heisei and of Reiwa, Nowruz, Muharram, the Coptic and Ethiopian thirteenth month,\n  // and a date before 1900.\n  var dates = ['2024-05-23', '2020-05-23', '2023-03-22', '2024-02-15', '2024-03-15', '2025-03-15', '1989-01-07',\n               '1989-01-08', '2019-05-01', '2024-03-20', '2023-07-19', '2023-09-08', '1912-07-30'];\n  var optionSets = [\n    {},\n    { dateStyle: 'full' }, { dateStyle: 'long' }, { dateStyle: 'medium' }, { dateStyle: 'short' },\n    { year: 'numeric', month: 'long', day: 'numeric', era: 'short' }, { era: 'long', year: 'numeric' },\n    { era: 'narrow', year: 'numeric', month: 'numeric', day: 'numeric' },\n    { year: 'numeric', month: 'short' }, { month: 'long', day: 'numeric' }, { month: 'numeric', day: 'numeric' },\n    { year: '2-digit', month: '2-digit', day: '2-digit' }, { year: 'numeric' }, { month: 'narrow' },\n    { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }\n  ];\n\n  locales.forEach(function (locale) {\n    calendars.forEach(function (calendar) {\n      optionSets.forEach(function (options) {\n        var full = Object.assign({ timeZone: 'UTC', calendar: calendar }, options);\n        var head = locale + ' ' + JSON.stringify(full);\n        var dtf;\n        try { dtf = new Intl.DateTimeFormat(locale, full); } catch (e) { lines.push(head + ' => throws ' + e.name); return; }\n        dates.forEach(function (text) {\n          line(head + ' ' + text, function () { return dtf.format(Date.parse(text + 'T12:00Z')); });\n        });\n      });\n      var head = locale + ' ' + calendar;\n      var tagged = locale + '-u-ca-' + calendar;\n      var dtf = new Intl.DateTimeFormat(locale, { timeZone: 'UTC', calendar: calendar, dateStyle: 'long' });\n      var era = new Intl.DateTimeFormat(locale, { timeZone: 'UTC', calendar: calendar, year: 'numeric', month: 'long', day: 'numeric', era: 'long' });\n      var noon = function (text) { return Date.parse(text + 'T12:00Z'); };\n      line(head + ' parts', function () { return parts(dtf.formatToParts(noon('2020-05-23'))); });\n      line(head + ' era parts', function () { return parts(era.formatToParts(noon('2023-03-22'))); });\n      line(head + ' range in a month', function () { return dtf.formatRange(noon('2024-05-23'), noon('2024-05-25')); });\n      line(head + ' range across months', function () { return dtf.formatRange(noon('2024-05-23'), noon('2024-07-02')); });\n      line(head + ' range across years', function () { return dtf.formatRange(noon('2023-05-23'), noon('2024-07-02')); });\n      line(head + ' range parts', function () { return parts(dtf.formatRangeToParts(noon('2024-05-23'), noon('2024-07-02'))); });\n      line(head + ' date and time', function () {\n        return new Intl.DateTimeFormat(locale, { timeZone: 'UTC', calendar: calendar, dateStyle: 'medium', timeStyle: 'short', hourCycle: 'h23' }).format(noon('2024-05-23'));\n      });\n      line(head + ' resolved', function () { return dtf.resolvedOptions().calendar; });\n      line(head + ' plain date', function () { return Temporal.PlainDate.from('2024-05-23').withCalendar(calendar).toLocaleString(tagged); });\n      line(head + ' plain date long', function () { return Temporal.PlainDate.from('2023-03-22').withCalendar(calendar).toLocaleString(tagged, { dateStyle: 'long' }); });\n      line(head + ' plain year-month', function () { return Temporal.PlainDate.from('2024-05-23').withCalendar(calendar).toPlainYearMonth().toLocaleString(tagged); });\n      line(head + ' plain month-day', function () { return Temporal.PlainDate.from('2024-05-23').withCalendar(calendar).toPlainMonthDay().toLocaleString(tagged); });\n      line(head + ' plain date-time', function () { return Temporal.PlainDateTime.from('2024-05-23T08:30').withCalendar(calendar).toLocaleString(tagged, { year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit', hourCycle: 'h23' }); });\n      line(head + ' zoned', function () { return Temporal.ZonedDateTime.from('2024-05-23T08:30[UTC]').withCalendar(calendar).toLocaleString(tagged, { dateStyle: 'medium' }); });\n      line(head + ' plain date in another calendar', function () { return Temporal.PlainDate.from('2024-05-23').withCalendar(calendar).toLocaleString(locale); });\n      line(head + ' plain date in gregory', function () {\n        return new Intl.DateTimeFormat(locale, { calendar: calendar }).format(Temporal.PlainDate.from('2024-05-23').withCalendar('gregory'));\n      });\n    });\n  });\n\n  // The requested calendar each locale resolves, aliases and fallbacks included, and the list.\n  ['buddhist', 'chinese', 'coptic', 'dangi', 'ethioaa', 'ethiopic', 'ethiopic-amete-alem', 'gregory', 'hebrew', 'indian',\n   'islamic', 'islamic-civil', 'islamic-rgsa', 'islamic-tbla', 'islamic-umalqura', 'islamicc', 'iso8601', 'japanese',\n   'persian', 'roc', 'julian'].forEach(function (calendar) {\n    line('resolved option ' + calendar, function () { return new Intl.DateTimeFormat('en', { calendar: calendar }).resolvedOptions().calendar; });\n    line('resolved extension ' + calendar, function () { return new Intl.DateTimeFormat('de-u-ca-' + calendar).resolvedOptions().calendar; });\n  });\n  line('supportedValuesOf calendar', function () { return Intl.supportedValuesOf('calendar').join(); });\n\n  return lines.join('\\n');\n})();\n";
var __value;
try { __value = (0, eval)(__text); }
catch (e) { __value = "ABORT " + __errName(e) + ": " + __plain(String(e && e.message)); }
String(__value).split("\n").forEach(function (line) { __out(line); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
