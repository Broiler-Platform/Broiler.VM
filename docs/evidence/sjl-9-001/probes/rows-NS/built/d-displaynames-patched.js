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
// Rows N and S: src/tests/cldr/displaynames/displaynames.js at cd1928d with one change, the JS survey's
// (survey-sjl9-js/datasets2/): its last line calls Intl.NumberFormat.supportedLocalesOf where the program calls
// Intl.DisplayNames.supportedLocalesOf, so an engine without the latter still completes the other lines.
// The prelude's helpers are kept under names of their own first: the program declares globals of its
// own (displaynames.js a variable `plain`, numbers.js functions `parts` and `show`) that replace them.
var __out = out, __errName = errName, __plain = plain;
var __text = "// SPDX-FileCopyrightText: 2026 Broiler Platform contributors\n// SPDX-License-Identifier: Apache-2.0\n//\n// German and English display names, the dataset slice I4's DisplayNames record keeps (JSD-0052).\n// Each type, style, language display and fallback names codes of every shape in en and de: languages\n// with scripts, regions and variants, regions and areas, scripts, currencies, calendars and date-time\n// fields, and codes the data does not name; a last group answers the errors ECMA-402 states. It uses\n// no host function and completes with its lines, so Node and the profile run the same text.\n// Non-ASCII is escaped.\n\nvar codes = {\n  language: ['en', 'de', 'en-US', 'en-GB', 'en-AU', 'de-AT', 'de-CH', 'fr-CA', 'es-419', 'pt-BR', 'nl-BE',\n    'zh-Hans', 'zh-Hant', 'zh-Hant-TW', 'zh-Hans-CN', 'sr-Latn', 'sr-Cyrl-RS', 'en-Latn-US', 'de-1996',\n    'sl-rozaj-biske', 'ja-Kana', 'ar-001', 'xyz', 'xyz-DE', 'en-XY', 'und', 'zxx', 'mul', 'iw', 'in', 'sh', 'EN-us'],\n  region: ['US', 'GB', 'DE', 'AT', 'CH', '419', '001', '150', 'HK', 'MO', 'PS', 'BA', 'MM', 'CI', 'CZ', 'XK',\n    'ZZ', 'AA', '999', 'us', 'de'],\n  script: ['Latn', 'Cyrl', 'Hans', 'Hant', 'Arab', 'Zzzz', 'Zyyy', 'Zsye', 'Qaaa', 'latn', 'HANS'],\n  currency: ['USD', 'EUR', 'CHF', 'JPY', 'GBP', 'BTN', 'XYZ', 'usd', 'eur'],\n  calendar: ['gregory', 'buddhist', 'islamic-civil', 'islamic-umalqura', 'iso8601', 'ethioaa', 'roc', 'japanese',\n    'chinese', 'xyz', 'GREGORY'],\n  dateTimeField: ['era', 'year', 'quarter', 'month', 'weekOfYear', 'weekday', 'day', 'dayPeriod', 'hour', 'minute',\n    'second', 'timeZoneName']\n};\nvar invalid = {\n  language: ['', 'e', 'en-', 'en_US', 'en-u-ca-gregory', 'root', 'en-x-private', '123'],\n  region: ['', 'U', 'USA', '1234', 'u1'],\n  script: ['', 'Lat', 'Latin', 'La1n'],\n  currency: ['', 'US', 'USDD', 'U1D'],\n  calendar: ['', 'gregory-', 'gregory_', 'ab', 'abcdefghi'],\n  dateTimeField: ['', 'Year', 'years', 'week', 'zone']\n};\nvar locales = ['en', 'de'];\nvar styles = ['long', 'short', 'narrow'];\n\nfunction escape(text) {\n  var out = '';\n  for (var i = 0; i < text.length; i++) {\n    var unit = text.charCodeAt(i);\n    out += unit < 0x20 || unit > 0x7e || text[i] === '\\\\'\n      ? '\\\\u' + ('000' + unit.toString(16)).slice(-4)\n      : text[i];\n  }\n  return out;\n}\n\nfunction attempt(run) {\n  try {\n    return run();\n  } catch (error) {\n    return error.name;\n  }\n}\n\nvar lines = [];\n\nfor (var l = 0; l < locales.length; l++) {\n  for (var type in codes) {\n    var displays = type === 'language' ? ['dialect', 'standard'] : [undefined];\n\n    for (var s = 0; s < styles.length; s++) {\n      for (var d = 0; d < displays.length; d++) {\n        var options = { type: type, style: styles[s] };\n\n        if (displays[d]) {\n          options.languageDisplay = displays[d];\n        }\n\n        var names = new Intl.DisplayNames(locales[l], options);\n        var none = new Intl.DisplayNames(locales[l], Object.assign({ fallback: 'none' }, options));\n        var head = locales[l] + ' ' + JSON.stringify(options);\n\n        for (var c = 0; c < codes[type].length; c++) {\n          lines.push(head + ' ' + codes[type][c] + ' => ' + attempt(function () { return names.of(codes[type][c]); }) +\n            ' | ' + attempt(function () { return none.of(codes[type][c]); }));\n        }\n\n        lines.push(head + ' resolvedOptions => ' + JSON.stringify(names.resolvedOptions()));\n      }\n    }\n  }\n}\n\nfor (var kind in invalid) {\n  var plain = new Intl.DisplayNames('en', { type: kind });\n  lines.push(kind + ' invalid => ' + invalid[kind].map(function (code) {\n    return JSON.stringify(code) + ':' + attempt(function () { return plain.of(code); });\n  }).join(' '));\n}\n\nlines.push('no options => ' + attempt(function () { return new Intl.DisplayNames('en'); }));\nlines.push('no type => ' + attempt(function () { return new Intl.DisplayNames('en', {}); }));\nlines.push('bad type => ' + attempt(function () { return new Intl.DisplayNames('en', { type: 'unit' }); }));\nlines.push('primitive options => ' + attempt(function () { return new Intl.DisplayNames('en', 'region'); }));\nlines.push('call => ' + attempt(function () { return Intl.DisplayNames(); }));\nlines.push('symbol code => ' + attempt(function () { return new Intl.DisplayNames('en', { type: 'region' }).of(Symbol()); }));\nlines.push('supportedLocalesOf => ' + Intl.NumberFormat.supportedLocalesOf(['de-AT', 'en', 'de-DE', 'zz']).join(','));\n\nlines.map(escape).join('\\n');\n";
var __value;
try { __value = (0, eval)(__text); }
catch (e) { __value = "ABORT " + __errName(e) + ": " + __plain(String(e && e.message)); }
String(__value).split("\n").forEach(function (line) { __out(line); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
