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
// Rows U1-U16 against Broiler.VM's reference dataset program src/tests/cldr/durations/durations.js at cd1928d,
// byte for byte as a string, evaluated by indirect eval as its README says (the completion value is its lines).
var __text = "// SPDX-FileCopyrightText: 2026 Broiler Platform contributors\n// SPDX-License-Identifier: Apache-2.0\n//\n// German and English durations, the dataset slice I4's DurationFormat record keeps (JSD-0051). Each\n// format, of every style and of per-unit styles, displays, fractional digits and a numbering system,\n// writes sixteen durations, some as parts, and its resolved options; a last group answers the errors\n// ECMA-402 states. It uses no host function and completes with its lines, so Node and the profile run\n// the same text. Non-ASCII is escaped.\n\nvar locales = ['en', 'de'];\nvar formats = [\n  {}, { style: 'long' }, { style: 'short' }, { style: 'narrow' }, { style: 'digital' },\n  { style: 'digital', fractionalDigits: 2 }, { style: 'digital', hours: '2-digit' },\n  { style: 'digital', hoursDisplay: 'auto' }, { style: 'long', minutes: 'numeric' },\n  { style: 'long', seconds: 'numeric', fractionalDigits: 3 }, { style: 'short', milliseconds: 'numeric' },\n  { style: 'narrow', microseconds: 'numeric', fractionalDigits: 0 }, { style: 'long', yearsDisplay: 'always', daysDisplay: 'always' },\n  { years: 'long', months: 'short', weeks: 'narrow', days: 'long' }, { style: 'short', numberingSystem: 'arab' }\n];\nvar durations = [\n  { years: 1, months: 2, weeks: 3, days: 4, hours: 5, minutes: 6, seconds: 7, milliseconds: 8, microseconds: 9, nanoseconds: 10 },\n  { hours: 1, minutes: 2, seconds: 3 }, { hours: 1, seconds: 3 }, { minutes: 5 }, { seconds: 0 },\n  { days: 1 }, { years: 2, days: 1 }, { milliseconds: 1500 }, { seconds: 1, milliseconds: 234, microseconds: 567, nanoseconds: 891 },\n  { hours: 25, minutes: 61 }, { years: -1, months: -2 }, { hours: -1, minutes: -2, seconds: -3 }, { minutes: -5, seconds: -0 },\n  { days: 1000, hours: 12345 }, { nanoseconds: 1 }, { weeks: 1, milliseconds: 1 }\n];\n\nfunction escape(text) {\n  var out = '';\n  for (var i = 0; i < text.length; i++) {\n    var unit = text.charCodeAt(i);\n    out += unit < 0x20 || unit > 0x7e || text[i] === '\\\\'\n      ? '\\\\u' + ('000' + unit.toString(16)).slice(-4)\n      : text[i];\n  }\n  return out;\n}\n\nfunction attempt(run) {\n  try {\n    return run();\n  } catch (error) {\n    return error.name;\n  }\n}\n\nvar lines = [];\n\nfor (var l = 0; l < locales.length; l++) {\n  for (var f = 0; f < formats.length; f++) {\n    var head = locales[l] + ' ' + JSON.stringify(formats[f]);\n    var format = attempt(function () { return new Intl.DurationFormat(locales[l], formats[f]); });\n\n    if (typeof format === 'string') {\n      lines.push(head + ' => ' + format);\n      continue;\n    }\n\n    for (var d = 0; d < durations.length; d++) {\n      lines.push(head + ' ' + JSON.stringify(durations[d]) + ' => ' + attempt(function () { return format.format(durations[d]); }));\n    }\n\n    lines.push(head + ' parts => ' + format.formatToParts(durations[0]).map(function (part) {\n      return part.type + ':' + part.value + (part.unit ? ':' + part.unit : '');\n    }).join('|'));\n    lines.push(head + ' resolvedOptions => ' + JSON.stringify(format.resolvedOptions()));\n  }\n}\n\nvar plain = new Intl.DurationFormat('en');\n\nlines.push('string => ' + attempt(function () { return plain.format('PT1H'); }));\nlines.push('number => ' + attempt(function () { return plain.format(5); }));\nlines.push('empty => ' + attempt(function () { return plain.format({}); }));\nlines.push('fraction => ' + attempt(function () { return plain.format({ hours: 1.5 }); }));\nlines.push('mixed signs => ' + attempt(function () { return plain.format({ hours: 1, minutes: -1 }); }));\nlines.push('too large => ' + attempt(function () { return plain.format({ years: 4294967296 }); }));\nlines.push('bad style => ' + attempt(function () { return new Intl.DurationFormat('en', { style: 'full' }); }));\nlines.push('fractional always => ' + attempt(function () { return new Intl.DurationFormat('en', { milliseconds: 'numeric', millisecondsDisplay: 'always' }); }));\nlines.push('long after numeric => ' + attempt(function () { return new Intl.DurationFormat('en', { hours: 'numeric', minutes: 'long' }); }));\nlines.push('bad fractional digits => ' + attempt(function () { return new Intl.DurationFormat('en', { fractionalDigits: 10 }); }));\nlines.push('primitive options => ' + attempt(function () { return new Intl.DurationFormat('en', 'long'); }));\nlines.push('call => ' + attempt(function () { return Intl.DurationFormat(); }));\nlines.push('supportedLocalesOf => ' + Intl.DurationFormat.supportedLocalesOf(['de-AT', 'en', 'de-DE', 'zz']).join(','));\n\nlines.map(escape).join('\\n');\n";
var __value = (0, eval)(__text);
String(__value).split('\n').forEach(function (line) { out(line); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
