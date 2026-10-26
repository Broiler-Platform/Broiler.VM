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
// Rows N and S: Broiler.VM's reference dataset program src/tests/cldr/numbers/numbers.js at cd1928d, byte for byte as a
// string, evaluated by indirect eval as its README says (the completion value is its lines).
// The prelude's helpers are kept under names of their own first: the program declares globals of its
// own (displaynames.js a variable `plain`, numbers.js functions `parts` and `show`) that replace them.
var __out = out, __errName = errName, __plain = plain;
var __text = "// SPDX-FileCopyrightText: 2026 Broiler Platform contributors\n// SPDX-License-Identifier: Apache-2.0\n//\n// German and English number formatting, the dataset JSD-0027 section 7 asks slice I2 to retain\n// (JSD-0044). Each case formats the same values with one Intl.NumberFormat, and writes each answer\n// with its parts; ranges are written the same way. The program completes with the lines; it uses no\n// host function, so Node and the profile run the same text. Non-ASCII is escaped, so the retained\n// answers are ASCII.\n\nvar values = [0, -0, 1, -1, 0.5, 1.5, 2.5, -2.5, 0.125, 1.005, 12.345, 999.995, 1000, 1234.5678,\n  -1234.5678, 99999, 100000, 123456789, 1e21, 1.23e-7, 5e-324, NaN, Infinity, -Infinity,\n  '123456789012345678901234567890.5', '-0.000', '0x1F', ' 42 ', '1e400', '1e-400', 7n, -12345678901234567890n];\n\nvar cases = [\n  ['en', {}],\n  ['en-US', { minimumFractionDigits: 2 }],\n  ['en', { maximumFractionDigits: 0 }],\n  ['en', { minimumIntegerDigits: 3 }],\n  ['en', { maximumSignificantDigits: 3 }],\n  ['en', { minimumSignificantDigits: 4 }],\n  ['en', { maximumSignificantDigits: 2, maximumFractionDigits: 2, roundingPriority: 'morePrecision' }],\n  ['en', { maximumSignificantDigits: 2, maximumFractionDigits: 2, roundingPriority: 'lessPrecision' }],\n  ['en', { useGrouping: false }],\n  ['en', { useGrouping: 'min2' }],\n  ['en', { useGrouping: 'always' }],\n  ['en', { signDisplay: 'always' }],\n  ['en', { signDisplay: 'exceptZero' }],\n  ['en', { signDisplay: 'never' }],\n  ['en', { signDisplay: 'negative' }],\n  ['en', { roundingMode: 'halfEven', maximumFractionDigits: 1 }],\n  ['en', { roundingMode: 'floor', maximumFractionDigits: 1 }],\n  ['en', { roundingMode: 'ceil', maximumFractionDigits: 1 }],\n  ['en', { roundingMode: 'trunc', maximumFractionDigits: 1 }],\n  ['en', { roundingIncrement: 25, minimumFractionDigits: 2, maximumFractionDigits: 2 }],\n  ['en', { trailingZeroDisplay: 'stripIfInteger', minimumFractionDigits: 2 }],\n  ['en', { style: 'percent' }],\n  ['en', { style: 'percent', signDisplay: 'exceptZero', maximumFractionDigits: 1 }],\n  ['en', { style: 'currency', currency: 'USD' }],\n  ['en', { style: 'currency', currency: 'EUR', currencyDisplay: 'code' }],\n  ['en', { style: 'currency', currency: 'EUR', currencyDisplay: 'name' }],\n  ['en', { style: 'currency', currency: 'JPY' }],\n  ['en', { style: 'currency', currency: 'CAD', currencyDisplay: 'narrowSymbol' }],\n  ['en', { style: 'currency', currency: 'USD', currencySign: 'accounting' }],\n  ['en', { style: 'currency', currency: 'USD', currencySign: 'accounting', signDisplay: 'always' }],\n  ['en', { style: 'currency', currency: 'USD', notation: 'compact' }],\n  ['en', { notation: 'scientific' }],\n  ['en', { notation: 'engineering', maximumFractionDigits: 2 }],\n  ['en', { notation: 'compact' }],\n  ['en', { notation: 'compact', compactDisplay: 'long' }],\n  ['en', { style: 'unit', unit: 'kilometer-per-hour' }],\n  ['en', { style: 'unit', unit: 'liter', unitDisplay: 'long' }],\n  ['en', { style: 'unit', unit: 'percent' }],\n  ['en', { style: 'unit', unit: 'megabyte', unitDisplay: 'narrow' }],\n  ['en-u-nu-thai', {}],\n  ['en', { numberingSystem: 'thai' }],\n  ['de', {}],\n  ['de-DE', { minimumFractionDigits: 2 }],\n  ['de', { useGrouping: 'min2' }],\n  ['de', { signDisplay: 'exceptZero' }],\n  ['de', { style: 'percent' }],\n  ['de', { style: 'currency', currency: 'EUR' }],\n  ['de', { style: 'currency', currency: 'USD', currencyDisplay: 'name' }],\n  ['de', { style: 'currency', currency: 'USD', currencyDisplay: 'code' }],\n  ['de', { style: 'currency', currency: 'EUR', currencySign: 'accounting' }],\n  ['de', { notation: 'scientific' }],\n  ['de', { notation: 'compact' }],\n  ['de', { notation: 'compact', compactDisplay: 'long' }],\n  ['de', { style: 'currency', currency: 'EUR', notation: 'compact' }],\n  ['de', { style: 'unit', unit: 'kilometer-per-hour', unitDisplay: 'long' }],\n  ['de', { style: 'unit', unit: 'celsius' }]\n];\n\nvar ranges = [[1, 5], [1, 1], [-5, -1], [0.9999, 1.0001], [1000, 5000], [3, 5000000], [-1, 1]];\n\nfunction escape(text) {\n  var out = '';\n  for (var i = 0; i < text.length; i++) {\n    var unit = text.charCodeAt(i);\n    out += unit < 0x20 || unit > 0x7e || text[i] === '\\\\'\n      ? '\\\\u' + ('000' + unit.toString(16)).slice(-4)\n      : text[i];\n  }\n  return out;\n}\n\nfunction parts(list) {\n  var out = '';\n  for (var i = 0; i < list.length; i++) {\n    out += (i ? ' ' : '') + list[i].type + (list[i].source ? '/' + list[i].source : '') + ':\"' + escape(list[i].value) + '\"';\n  }\n  return out;\n}\n\nfunction show(value) {\n  return typeof value === 'bigint' ? value + 'n' : typeof value === 'string' ? '\"' + value + '\"' : Object.is(value, -0) ? '-0' : String(value);\n}\n\nvar lines = [];\n\nfor (var c = 0; c < cases.length; c++) {\n  var format = new Intl.NumberFormat(cases[c][0], cases[c][1]);\n  var head = (c + 1) + ' ' + cases[c][0] + ' ' + JSON.stringify(cases[c][1]);\n\n  for (var v = 0; v < values.length; v++) {\n    lines.push(head + ' ' + show(values[v]) + ' => \"' + escape(format.format(values[v])) + '\" ' + parts(format.formatToParts(values[v])));\n  }\n\n  for (var r = 0; r < ranges.length; r++) {\n    lines.push(head + ' range ' + ranges[r][0] + '..' + ranges[r][1] + ' => \"' +\n      escape(format.formatRange(ranges[r][0], ranges[r][1])) + '\" ' + parts(format.formatRangeToParts(ranges[r][0], ranges[r][1])));\n  }\n\n  var resolved = format.resolvedOptions();\n  var options = [];\n  for (var key in resolved) {\n    options.push(key + '=' + resolved[key]);\n  }\n  lines.push(head + ' resolved ' + options.join(' '));\n}\n\nlines.push('toLocaleString ' + escape((1234.5).toLocaleString('de')) + ' ' + escape((-0.5).toLocaleString('en', { style: 'percent' })) +\n  ' ' + escape((12345678901234567890n).toLocaleString('de-DE')));\n\nlines.join('\\n');\n";
var __value;
try { __value = (0, eval)(__text); }
catch (e) { __value = "ABORT " + __errName(e) + ": " + __plain(String(e && e.message)); }
String(__value).split("\n").forEach(function (line) { __out(line); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
