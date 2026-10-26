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
// Rows M and P: Broiler.VM's reference dataset program src/tests/cldr/plurals/plurals.js at cd1928d, byte for
// byte as a string, evaluated by indirect eval as its README says (the completion value is its lines).
var __text = "// SPDX-FileCopyrightText: 2026 Broiler Platform contributors\n// SPDX-License-Identifier: Apache-2.0\n//\n// German and English plural categories, the dataset slice I4's PluralRules record keeps (JSD-0047).\n// Each format selects the category of every value, cardinal and ordinal, under digit options,\n// rounding modes, increments, priorities and notations, and of ranges; and writes its resolved\n// options. It uses no host function and completes with its lines, so Node and the profile run the\n// same text. Non-ASCII is escaped.\n\nvar formats = [\n  ['en'], ['en-US'], ['de'], ['de-DE'],\n  ['en', { type: 'ordinal' }], ['en-US', { type: 'ordinal' }], ['de', { type: 'ordinal' }],\n  ['en', { minimumFractionDigits: 1 }], ['de', { minimumFractionDigits: 2 }],\n  ['en', { maximumFractionDigits: 0 }], ['en', { type: 'ordinal', maximumFractionDigits: 0 }],\n  ['en', { minimumSignificantDigits: 2 }], ['en', { maximumSignificantDigits: 1 }],\n  ['de', { maximumSignificantDigits: 2, roundingMode: 'floor' }],\n  ['en', { maximumFractionDigits: 0, roundingMode: 'trunc' }], ['en', { maximumFractionDigits: 0, roundingMode: 'halfEven' }],\n  ['en', { minimumFractionDigits: 1, maximumFractionDigits: 1, roundingIncrement: 5 }],\n  ['en', { minimumFractionDigits: 2, trailingZeroDisplay: 'stripIfInteger' }],\n  ['en', { maximumFractionDigits: 1, maximumSignificantDigits: 1, roundingPriority: 'morePrecision' }],\n  ['en', { maximumFractionDigits: 1, maximumSignificantDigits: 1, roundingPriority: 'lessPrecision' }],\n  ['en', { minimumIntegerDigits: 3 }],\n  ['en', { notation: 'compact' }], ['de', { notation: 'compact', compactDisplay: 'long' }],\n  ['en', { notation: 'scientific' }], ['de', { notation: 'engineering', type: 'ordinal' }],\n  ['en-u-nu-arab'], ['de-u-co-phonebk', { localeMatcher: 'lookup' }]\n];\n\nvar values = [0, 1, 2, 3, 4, 5, 11, 12, 13, 21, 22, 23, 100, 101, 102, 103, 111, 1000, 1000000, 0.5, 1.5,\n  0.95, 0.999, 1.004, 1.05, 2.5, -1, -2, -0, NaN, Infinity, -Infinity, '1.0', '1.00000000000000000001', 1e21, 1n];\n\nvar ranges = [[1, 2], [0, 1], [1, 1], [2, 3], [1, 1.0001], [1, 5], [1, 2.5], [0.5, 1], [-1, 1], [NaN, 1], [1, undefined]];\n\nfunction escape(text) {\n  var out = '';\n  for (var i = 0; i < text.length; i++) {\n    var unit = text.charCodeAt(i);\n    out += unit < 0x20 || unit > 0x7e || text[i] === '\\\\'\n      ? '\\\\u' + ('000' + unit.toString(16)).slice(-4)\n      : text[i];\n  }\n  return out;\n}\n\nfunction show(value) {\n  return typeof value === 'string' ? \"'\" + value + \"'\" : typeof value === 'bigint' ? value + 'n' :\n    value === 0 && 1 / value < 0 ? '-0' : String(value);\n}\n\nfunction attempt(run) {\n  try {\n    return run();\n  } catch (error) {\n    return error.name;\n  }\n}\n\nvar lines = [];\n\nfor (var f = 0; f < formats.length; f++) {\n  var head = (f + 1) + ' ' + formats[f][0] + (formats[f][1] ? ' ' + JSON.stringify(formats[f][1]) : '');\n  var rules = new Intl.PluralRules(formats[f][0], formats[f][1]);\n\n  for (var v = 0; v < values.length; v++) {\n    lines.push(head + ' select ' + show(values[v]) + ' => ' + attempt(function () { return rules.select(values[v]); }));\n  }\n\n  for (var r = 0; r < ranges.length; r++) {\n    lines.push(head + ' selectRange ' + show(ranges[r][0]) + ' ' + show(ranges[r][1]) + ' => ' +\n      attempt(function () { return rules.selectRange(ranges[r][0], ranges[r][1]); }));\n  }\n\n  lines.push(head + ' resolvedOptions => ' + JSON.stringify(rules.resolvedOptions()));\n}\n\nlines.push('call => ' + attempt(function () { return Intl.PluralRules(); }));\nlines.push('type => ' + attempt(function () { return new Intl.PluralRules('en', { type: 'plural' }); }));\nlines.push('supportedLocalesOf => ' + Intl.PluralRules.supportedLocalesOf(['de-AT', 'en', 'de-DE', 'zz']).join(','));\n\nlines.map(escape).join('\\n');\n";
var __value = (0, eval)(__text);
String(__value).split('\n').forEach(function (line) { out(line); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
