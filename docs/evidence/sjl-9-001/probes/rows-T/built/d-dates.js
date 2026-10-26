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
// Rows T: Broiler.VM's reference dataset program src/tests/cldr/dates/dates.js at cd1928d, byte for byte as a
// string, evaluated by indirect eval as its README says (the completion value is its lines).
// The prelude's helpers are kept under names of their own first, in case the program declares globals.
var __out = out, __errName = errName, __plain = plain;
var __text = "// SPDX-FileCopyrightText: 2026 Broiler Platform contributors\n// SPDX-License-Identifier: Apache-2.0\n//\n// German and English date formatting, the dataset JSD-0027 section 7 asks slice I3 to retain\n// (JSD-0045). Each case formats the same instants with one Intl.DateTimeFormat, and writes each\n// answer with its parts; ranges are written the same way, and each format's resolved options. Only\n// UTC and fixed offsets are used, the time zones JSD-0027 section 5 item 5 admits. The program\n// completes with its lines; it uses no host function, so Node and the profile run the same text.\n// Non-ASCII is escaped, so the retained answers are ASCII.\n\nvar instants = [\n  0,                      // 1970-01-01T00:00:00.000Z, a Thursday, midnight\n  1709647629123,          // 2024-03-05T14:07:09.123Z, a Tuesday afternoon\n  1735732800000,          // 2025-01-01T12:00:00.000Z, noon\n  951782400000,           // 2000-02-29T00:00:00.000Z, a leap day\n  -62198755200000,        // -000001-01-01T00:00:00.000Z, 1 January 2 BC\n  253402300799999,        // 9999-12-31T23:59:59.999Z\n  -1,                     // 1969-12-31T23:59:59.999Z\n  -62167219200000,        // 0000-01-01T00:00:00.000Z, 1 January 1 BC, the year 0\n  8.64e15,                // +275760-09-13T00:00:00.000Z, the last time value\n  -8.64e15                // -271821-04-20T00:00:00.000Z, the first time value\n];\n\nvar cases = [\n  ['en', {}],\n  ['en-US', { timeZone: 'UTC' }],\n  ['en', { dateStyle: 'full' }],\n  ['en', { dateStyle: 'long' }],\n  ['en', { dateStyle: 'medium' }],\n  ['en', { dateStyle: 'short' }],\n  ['en', { timeStyle: 'full' }],\n  ['en', { timeStyle: 'long' }],\n  ['en', { timeStyle: 'medium' }],\n  ['en', { timeStyle: 'short' }],\n  ['en', { dateStyle: 'full', timeStyle: 'full' }],\n  ['en', { dateStyle: 'long', timeStyle: 'short' }],\n  ['en', { dateStyle: 'medium', timeStyle: 'medium' }],\n  ['en', { dateStyle: 'short', timeStyle: 'long' }],\n  ['en', { year: 'numeric', month: 'long', day: 'numeric' }],\n  ['en', { year: '2-digit', month: '2-digit', day: '2-digit' }],\n  ['en', { weekday: 'long', year: 'numeric', month: 'short', day: 'numeric' }],\n  ['en', { weekday: 'short' }],\n  ['en', { weekday: 'narrow', day: 'numeric' }],\n  ['en', { month: 'narrow' }],\n  ['en', { month: 'long', year: 'numeric' }],\n  ['en', { month: 'short', day: 'numeric' }],\n  ['en', { era: 'short', year: 'numeric' }],\n  ['en', { era: 'long', year: 'numeric', month: 'long', day: 'numeric' }],\n  ['en', { hour: 'numeric' }],\n  ['en', { hour: 'numeric', minute: '2-digit' }],\n  ['en', { hour: '2-digit', minute: '2-digit', second: '2-digit' }],\n  ['en', { hour: 'numeric', minute: 'numeric', hour12: false }],\n  ['en', { hour: 'numeric', minute: 'numeric', hourCycle: 'h23' }],\n  ['en', { hour: 'numeric', minute: 'numeric', hourCycle: 'h24' }],\n  ['en', { hour: 'numeric', minute: 'numeric', hourCycle: 'h11' }],\n  ['en', { minute: '2-digit', second: '2-digit' }],\n  ['en', { hour: 'numeric', minute: 'numeric', second: 'numeric', fractionalSecondDigits: 3 }],\n  ['en', { second: 'numeric', fractionalSecondDigits: 1 }],\n  ['en', { hour: 'numeric', dayPeriod: 'short' }],\n  ['en', { dayPeriod: 'long' }],\n  ['en', { hour: 'numeric', minute: 'numeric', timeZoneName: 'short' }],\n  ['en', { hour: 'numeric', timeZoneName: 'long' }],\n  ['en', { hour: 'numeric', timeZoneName: 'shortOffset', timeZone: '+05:30' }],\n  ['en', { hour: 'numeric', minute: 'numeric', timeZoneName: 'longOffset', timeZone: '-08:00' }],\n  ['en', { dateStyle: 'medium', timeStyle: 'full', timeZone: '+01:00' }],\n  ['en', { year: 'numeric', month: 'numeric', day: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric' }],\n  ['en', { month: 'long', day: 'numeric', hour: 'numeric', minute: 'numeric' }],\n  ['en-u-hc-h23', { hour: 'numeric', minute: 'numeric' }],\n  ['en-u-nu-thai', { year: 'numeric', month: 'numeric', day: 'numeric' }],\n  ['en-u-ca-gregory', { dateStyle: 'medium' }],\n  ['de', {}],\n  ['de-DE', { dateStyle: 'full', timeStyle: 'full' }],\n  ['de', { dateStyle: 'long', timeStyle: 'medium' }],\n  ['de', { dateStyle: 'medium', timeStyle: 'short' }],\n  ['de', { dateStyle: 'short' }],\n  ['de', { year: 'numeric', month: 'long', day: 'numeric' }],\n  ['de', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }],\n  ['de', { weekday: 'short', month: 'short', day: 'numeric' }],\n  ['de', { month: 'long', year: 'numeric' }],\n  ['de', { era: 'short', year: 'numeric' }],\n  ['de', { hour: 'numeric', minute: 'numeric' }],\n  ['de', { hour: 'numeric', minute: 'numeric', hour12: true }],\n  ['de', { hour: '2-digit', minute: '2-digit', second: '2-digit', timeZoneName: 'short' }],\n  ['de', { hour: 'numeric', dayPeriod: 'long' }],\n  ['de', { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' }],\n  ['en', { timeZoneName: 'shortGeneric' }],\n  ['en', { hour: 'numeric', timeZoneName: 'longGeneric' }],\n  ['en', { hour: 'numeric', minute: 'numeric', timeZoneName: 'long', timeZone: '+00:00' }],\n  ['de', { hour: 'numeric', timeZoneName: 'short', timeZone: '+00:00' }],\n  ['en', { dateStyle: 'medium', timeStyle: 'long', timeZone: 'Etc/GMT-3' }],\n  ['en', { year: 'numeric', month: 'long', day: 'numeric', weekday: 'long', hour: '2-digit', minute: '2-digit', second: '2-digit', timeZoneName: 'shortOffset', timeZone: '-03:30' }],\n  ['de', { hour: 'numeric', minute: 'numeric', hourCycle: 'h11' }],\n  ['de', { hour: 'numeric', dayPeriod: 'short', hour12: true }],\n  ['de', { dayPeriod: 'narrow' }],\n  ['de', { weekday: 'short' }],\n  ['de', { weekday: 'narrow', month: 'narrow' }],\n  ['en', { era: 'narrow', year: 'numeric' }],\n  ['en', { fractionalSecondDigits: 2 }],\n  ['de', { minute: 'numeric', second: 'numeric', fractionalSecondDigits: 3 }],\n  ['en', { dateStyle: 'short', timeStyle: 'short', hour12: false }],\n  ['en', { timeStyle: 'medium', hourCycle: 'h23' }],\n  ['de', { timeStyle: 'short', hour12: true }],\n  ['en-u-hc-h11', { timeStyle: 'short' }],\n  ['de-u-hc-h12', { hour: 'numeric', minute: 'numeric' }],\n  ['en', { hour: 'numeric', hour12: true, hourCycle: 'h23' }],\n  ['en', { weekday: 'long', month: 'long', day: 'numeric' }],\n  ['de', { year: '2-digit', month: 'short' }],\n  ['en', { era: 'short', month: 'short' }]\n];\n\nvar ranges = [\n  [1709647629123, 1709654829123],   // two hours apart\n  [1709647629123, 1709733629123],   // a day apart\n  [1709647629123, 1712326029123],   // a month apart\n  [1709647629123, 1741183629123],   // a year apart\n  [1709647629123, 1709647629123],   // the same instant\n  [1709647629123, 1709647689123]    // a minute apart\n];\n\nfunction escape(text) {\n  var out = '';\n  for (var i = 0; i < text.length; i++) {\n    var unit = text.charCodeAt(i);\n    out += unit < 0x20 || unit > 0x7e || text[i] === '\\\\'\n      ? '\\\\u' + ('000' + unit.toString(16)).slice(-4)\n      : text[i];\n  }\n  return out;\n}\n\nfunction parts(list) {\n  var out = '';\n  for (var i = 0; i < list.length; i++) {\n    out += (i ? ' ' : '') + list[i].type + (list[i].source ? '/' + list[i].source : '') + ':\"' + escape(list[i].value) + '\"';\n  }\n  return out;\n}\n\nvar lines = [];\n\nfor (var c = 0; c < cases.length; c++) {\n  var options = cases[c][1];\n\n  if (options.timeZone === undefined) {\n    options = Object.assign({ timeZone: 'UTC' }, options);\n  }\n\n  var format = new Intl.DateTimeFormat(cases[c][0], options);\n  var head = (c + 1) + ' ' + cases[c][0] + ' ' + JSON.stringify(cases[c][1]);\n\n  for (var i = 0; i < instants.length; i++) {\n    lines.push(head + ' ' + instants[i] + ' => \"' + escape(format.format(instants[i])) + '\" ' + parts(format.formatToParts(instants[i])));\n  }\n\n  for (var r = 0; r < ranges.length; r++) {\n    lines.push(head + ' range ' + ranges[r][0] + '..' + ranges[r][1] + ' => \"' +\n      escape(format.formatRange(ranges[r][0], ranges[r][1])) + '\" ' + parts(format.formatRangeToParts(ranges[r][0], ranges[r][1])));\n  }\n\n  var resolved = format.resolvedOptions();\n  var keys = [];\n  for (var key in resolved) {\n    keys.push(key + '=' + resolved[key]);\n  }\n  lines.push(head + ' resolved ' + escape(keys.join(' ')));\n}\n\nvar date = new Date(1709647629123);\nlines.push('toLocaleString => ' + escape(date.toLocaleString('en', { timeZone: 'UTC' })) + ' | ' +\n  escape(date.toLocaleDateString('de', { timeZone: 'UTC' })) + ' | ' +\n  escape(date.toLocaleTimeString('en-US', { timeZone: 'UTC' })) + ' | ' +\n  escape(new Date(NaN).toLocaleString('en')));\nlines.push('toLocale options => ' + escape(date.toLocaleString('de', { timeZone: 'UTC' })) + ' | ' +\n  escape(date.toLocaleDateString('en', { timeZone: 'UTC', month: 'long' })) + ' | ' +\n  escape(date.toLocaleTimeString('de', { timeZone: 'UTC', hour: 'numeric' })) + ' | ' +\n  escape(date.toLocaleDateString('en', { timeZone: 'UTC', dateStyle: 'full' })));\n\nlines.join('\\n');\n";
var __value;
try { __value = (0, eval)(__text); }
catch (e) { __value = "ABORT " + __errName(e) + ": " + __plain(String(e && e.message)); }
String(__value).split("\n").forEach(function (line) { __out(line); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
