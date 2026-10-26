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
// Rows T: Broiler.VM's reference dataset program src/tests/temporal/intl/temporal-intl.js at cd1928d, byte for byte as a
// string, evaluated by indirect eval as its README says (the completion value is its lines).
// The prelude's helpers are kept under names of their own first, in case the program declares globals.
var __out = out, __errName = errName, __plain = plain;
var __text = "// SPDX-FileCopyrightText: 2026 Broiler Platform contributors\n// SPDX-License-Identifier: Apache-2.0\n//\n// Intl.DateTimeFormat over Temporal objects, toLocaleString, and the ISO 8601 calendar (JSD-0055):\n// each line one formatting and its answer, or the name of the error it threw. It uses no host\n// function and completes with its lines, so the reference polyfill on Node and the profile run the\n// same text.\n(function () {\n  var lines = [];\n  function line(label, f) {\n    var answer;\n    try { answer = String(f()); } catch (e) { answer = 'throws ' + e.name; }\n    lines.push(label + ' => ' + answer);\n  }\n  function parts(list) {\n    return list.map(function (part) { return part.type + ':' + part.value + (part.source ? '@' + part.source : ''); }).join('|');\n  }\n\n  var values = {\n    date: new Temporal.PlainDate(2024, 3, 5),\n    dateGregory: new Temporal.PlainDate(2024, 3, 5, 'gregory'),\n    dateBce: new Temporal.PlainDate(-44, 3, 15, 'gregory'),\n    dateTime: new Temporal.PlainDateTime(2024, 3, 5, 13, 4, 5, 678),\n    time: new Temporal.PlainTime(0, 30, 45, 123),\n    yearMonth: new Temporal.PlainYearMonth(2024, 3, 'gregory', 5),\n    monthDay: new Temporal.PlainMonthDay(3, 5, 'gregory'),\n    instant: Temporal.Instant.from('2024-03-05T13:04:05.678Z'),\n    zoned: Temporal.ZonedDateTime.from('2024-03-05T13:04:05.678+01:00[Europe/Berlin]'),\n    yearMonthIso: new Temporal.PlainYearMonth(2024, 3),\n    monthDayIso: new Temporal.PlainMonthDay(3, 5)\n  };\n  var optionSets = [\n    {},\n    { dateStyle: 'full' }, { dateStyle: 'medium' }, { dateStyle: 'short' },\n    { timeStyle: 'medium' }, { timeStyle: 'short' }, { dateStyle: 'long', timeStyle: 'short' },\n    { year: 'numeric' }, { month: 'long' }, { day: 'numeric' }, { weekday: 'long' },\n    { year: '2-digit', month: 'short', day: '2-digit' }, { era: 'short' },\n    { hour: 'numeric', minute: '2-digit' }, { hour: '2-digit', hourCycle: 'h23' }, { second: 'numeric', fractionalSecondDigits: 3 },\n    { timeZoneName: 'shortOffset' }, { month: 'long', hour: 'numeric' }\n  ];\n  var locales = ['en', 'de'];\n  var calendars = [undefined, 'iso8601'];\n\n  locales.forEach(function (locale) {\n    calendars.forEach(function (calendar) {\n      optionSets.forEach(function (options) {\n        var full = Object.assign({ timeZone: 'Pacific/Apia' }, options);\n        if (calendar) full.calendar = calendar;\n        var head = locale + ' ' + JSON.stringify(full);\n        var dtf;\n        try { dtf = new Intl.DateTimeFormat(locale, full); } catch (e) { lines.push(head + ' => throws ' + e.name); return; }\n        Object.keys(values).forEach(function (name) {\n          line(head + ' ' + name, function () { return dtf.format(values[name]); });\n        });\n        line(head + ' date parts', function () { return parts(dtf.formatToParts(values.date)); });\n        line(head + ' dateTime range', function () { return dtf.formatRange(values.dateTime, values.dateTime.add({ hours: 5 })); });\n        line(head + ' date range parts', function () { return parts(dtf.formatRangeToParts(values.date, values.date.add({ days: 40 }))); });\n        line(head + ' mixed range', function () { return dtf.formatRange(values.date, values.dateTime); });\n        line(head + ' Date', function () { return dtf.format(Date.UTC(2024, 2, 5, 13, 4, 5, 678)); });\n      });\n    });\n  });\n\n  // toLocaleString, each type, with and without options.\n  [['en', undefined], ['de', undefined], ['en', { dateStyle: 'medium' }], ['de', { month: 'long', day: 'numeric' }],\n   ['en', { calendar: 'iso8601' }], ['en', { timeZone: 'Asia/Kolkata' }], ['en', { timeZoneName: 'shortOffset' }]].forEach(function (pair) {\n    var head = pair[0] + ' ' + JSON.stringify(pair[1]);\n    Object.keys(values).forEach(function (name) {\n      line('toLocaleString ' + head + ' ' + name, function () { return values[name].toLocaleString(pair[0], pair[1]); });\n    });\n  });\n  line('zoned toLocaleString UTC', function () { return Temporal.ZonedDateTime.from('2024-03-05T13:04Z[UTC]').toLocaleString('en'); });\n  line('zoned toLocaleString Kolkata', function () { return Temporal.ZonedDateTime.from('2024-03-05T13:04+05:30[Asia/Kolkata]').toLocaleString('de'); });\n  line('supportedValuesOf calendar', function () { return Intl.supportedValuesOf('calendar').filter(function (c) { return c === 'gregory' || c === 'iso8601'; }).join(); });\n  line('resolved iso8601', function () { return new Intl.DateTimeFormat('en-u-ca-iso8601').resolvedOptions().calendar; });\n\n  return lines.join('\\n');\n})();\n";
var __value;
try { __value = (0, eval)(__text); }
catch (e) { __value = "ABORT " + __errName(e) + ": " + __plain(String(e && e.message)); }
String(__value).split("\n").forEach(function (line) { __out(line); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
