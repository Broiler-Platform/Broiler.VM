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
// Rows N and S: Broiler.VM's reference dataset program src/tests/cldr/lists/lists.js at cd1928d, byte for byte as a
// string, evaluated by indirect eval as its README says (the completion value is its lines).
// The prelude's helpers are kept under names of their own first: the program declares globals of its
// own (displaynames.js a variable `plain`, numbers.js functions `parts` and `show`) that replace them.
var __out = out, __errName = errName, __plain = plain;
var __text = "// SPDX-FileCopyrightText: 2026 Broiler Platform contributors\n// SPDX-License-Identifier: Apache-2.0\n//\n// German and English lists, the dataset slice I4's ListFormat record keeps (JSD-0048). Each format,\n// of every type and style, joins lists of no element to five, as a string and as parts, and writes\n// its resolved options; a last group answers the errors ECMA-402 states. It uses no host function and\n// completes with its lines, so Node and the profile run the same text. Non-ASCII is escaped.\n\nvar locales = ['en', 'en-US', 'de', 'de-DE'];\nvar types = ['conjunction', 'disjunction', 'unit'];\nvar styles = ['long', 'short', 'narrow'];\nvar lists = [[], ['A'], ['A', 'B'], ['A', 'B', 'C'], ['A', 'B', 'C', 'D'], ['Anna', '', 'Carl', 'Dora', 'Emil']];\n\nfunction escape(text) {\n  var out = '';\n  for (var i = 0; i < text.length; i++) {\n    var unit = text.charCodeAt(i);\n    out += unit < 0x20 || unit > 0x7e || text[i] === '\\\\'\n      ? '\\\\u' + ('000' + unit.toString(16)).slice(-4)\n      : text[i];\n  }\n  return out;\n}\n\nfunction attempt(run) {\n  try {\n    return run();\n  } catch (error) {\n    return error.name;\n  }\n}\n\nvar lines = [];\n\nfor (var l = 0; l < locales.length; l++) {\n  for (var t = 0; t < types.length; t++) {\n    for (var s = 0; s < styles.length; s++) {\n      var options = { type: types[t], style: styles[s] };\n      var head = locales[l] + ' ' + JSON.stringify(options);\n      var format = new Intl.ListFormat(locales[l], options);\n\n      for (var n = 0; n < lists.length; n++) {\n        var list = lists[n];\n        lines.push(head + ' format ' + JSON.stringify(list) + ' => ' + format.format(list));\n        lines.push(head + ' formatToParts ' + JSON.stringify(list) + ' => ' + format.formatToParts(list).map(function (part) {\n          return part.type + ':' + part.value;\n        }).join('|'));\n      }\n\n      lines.push(head + ' resolvedOptions => ' + JSON.stringify(format.resolvedOptions()));\n    }\n  }\n}\n\nvar plain = new Intl.ListFormat('en');\nvar closed = 0;\nvar iterable = {};\niterable[Symbol.iterator] = function () {\n  var at = 0;\n  return {\n    next: function () { at++; return at === 1 ? { value: 'x', done: false } : { value: 7, done: false }; },\n    return: function () { closed++; return {}; }\n  };\n};\n\nlines.push('default => ' + JSON.stringify(plain.resolvedOptions()));\nlines.push('undefined => [' + plain.format() + ']');\nlines.push('string => ' + plain.format('abc'));\nlines.push('set => ' + plain.format(new Set(['x', 'y', 'x'])));\nlines.push('number element => ' + attempt(function () { return plain.format(['a', 1]); }));\nlines.push('iterator closed => ' + attempt(function () { return plain.format(iterable); }) + ' ' + closed);\nlines.push('null => ' + attempt(function () { return plain.format(null); }));\nlines.push('primitive options => ' + attempt(function () { return new Intl.ListFormat('en', 'long'); }));\nlines.push('null options => ' + attempt(function () { return new Intl.ListFormat('en', null); }));\nlines.push('bad type => ' + attempt(function () { return new Intl.ListFormat('en', { type: 'and' }); }));\nlines.push('bad style => ' + attempt(function () { return new Intl.ListFormat('en', { style: 'medium' }); }));\nlines.push('call => ' + attempt(function () { return Intl.ListFormat(); }));\nlines.push('supportedLocalesOf => ' + Intl.ListFormat.supportedLocalesOf(['de-AT', 'en', 'de-DE', 'zz']).join(','));\n\nlines.map(escape).join('\\n');\n";
var __value;
try { __value = (0, eval)(__text); }
catch (e) { __value = "ABORT " + __errName(e) + ": " + __plain(String(e && e.message)); }
String(__value).split("\n").forEach(function (line) { __out(line); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
