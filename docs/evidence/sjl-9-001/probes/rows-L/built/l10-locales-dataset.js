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
// SJL-9.1 sub-area (a): Broiler.VM's locales dataset program (src/tests/cldr/locales/locales.js at cd1928d, sha256
// 86c48f27b95ef062ad986bf6189a348b78837720982a19108680c8e38529b52f), its text held as a string and evaluated by indirect
// eval, as Node's reference command and Broiler.JS's dataset driver evaluate it; one output line per line of its
// completion value. work/compare_locales_dataset.py compares the lines with the retained answer and Broiler.VM's divergences.
var program = "// SPDX-FileCopyrightText: 2026 Broiler Platform contributors\n// SPDX-License-Identifier: Apache-2.0\n//\n// Intl.Locale over tags of every shape, the dataset slice I4's Locale record keeps (JSD-0046). Each\n// case makes a Locale from a tag and options, and writes its identifier and parts, its keywords,\n// its maximal and minimal forms, and what its information methods answer. Locale reads CLDR's\n// likely subtags and aliases for every language, not only the supported ones, so the tags range\n// over scripts, regions, variants, extensions and deprecated forms. It uses no host function and\n// completes with its lines, so Node and the profile run the same text. Non-ASCII is escaped.\n\nvar cases = [\n  ['en'], ['en-US'], ['en-GB'], ['en-Latn'], ['en-Arab'], ['en-Shaw'], ['de'], ['de-AT'], ['de-CH'],\n  ['fr'], ['fr-CA'], ['es-419'], ['pt-BR'], ['ar'], ['ar-EG'], ['he'], ['fa-IR'], ['ur'], ['hi'],\n  ['zh'], ['zh-Hant'], ['zh-TW'], ['zh-Hans-SG'], ['ja'], ['ko'], ['th'], ['ru'], ['sr-Latn'], ['uz-Cyrl'],\n  ['und'], ['und-AQ'], ['und-Latn-AQ'], ['und-Cyrl-RO'], ['und-Thai'], ['und-150'], ['und-AT'],\n  ['mo'], ['iw'], ['in-ID'], ['sh'], ['no'], ['tl'], ['art-lojban'], ['zh-hakka'], ['aar-x-private'],\n  ['es-ES-preeuro'], ['sl-rozaj-biske-1994'], ['hy-SU'], ['ru-SU'], ['en-u-ca-gregory-co-phonebk-kn'],\n  ['en-u-nu-thai-hc-h23'], ['de-u-co-phonebk-ca-buddhist'], ['en-u-fw-mon'], ['en-u-rg-gbzzzz'],\n  ['en-u-sd-usca'], ['ar-u-nu-arab'], ['pt-u-attr2-attr1-ca-gregory'], ['en-t-iw'], ['x-private'.replace('x-', 'und-x-')],\n  ['en', { calendar: 'islamicc', collation: 'emoji', hourCycle: 'h11', caseFirst: 'upper', numeric: true, numberingSystem: 'arab', firstDayOfWeek: '0' }],\n  ['en-US', { language: 'fr', script: 'Cyrl', region: 'BE', variants: 'Fonipa-1996' }],\n  ['de-u-kn-false', { numeric: true }],\n  ['en', { firstDayOfWeek: 'tue' }],\n  ['ja-JP', { region: 'US' }],\n  ['he-IL', { script: 'Latn' }]\n];\n\nfunction escape(text) {\n  var out = '';\n  for (var i = 0; i < text.length; i++) {\n    var unit = text.charCodeAt(i);\n    out += unit < 0x20 || unit > 0x7e || text[i] === '\\\\'\n      ? '\\\\u' + ('000' + unit.toString(16)).slice(-4)\n      : text[i];\n  }\n  return out;\n}\n\n// THE INFORMATION IS READ IN EITHER FORM: the current draft's methods (getWeekInfo), or the\n// accessors (weekInfo) of the proposal stage Node 22 implements.\nfunction info(locale, name) {\n  var method = locale['get' + name[0].toUpperCase() + name.slice(1)];\n  return typeof method === 'function' ? method.call(locale) : locale[name];\n}\n\nfunction show(value) {\n  return value === undefined ? 'undefined' : Array.isArray(value) ? '[' + value.join(',') + ']' : String(value);\n}\n\nvar lines = [];\n\nfor (var c = 0; c < cases.length; c++) {\n  var head = (c + 1) + ' ' + cases[c][0] + (cases[c][1] ? ' ' + JSON.stringify(cases[c][1]) : '');\n\n  try {\n    var locale = new Intl.Locale(cases[c][0], cases[c][1]);\n    var week = info(locale, 'weekInfo');\n    lines.push(head + ' => ' + locale + ' base=' + locale.baseName + ' language=' + show(locale.language) +\n      ' script=' + show(locale.script) + ' region=' + show(locale.region));\n    lines.push(head + ' variants => ' + show(locale.variants));\n    lines.push(head + ' keywords => calendar=' + show(locale.calendar) + ' collation=' + show(locale.collation) +\n      ' hourCycle=' + show(locale.hourCycle) + ' caseFirst=' + show(locale.caseFirst) +\n      ' numeric=' + locale.numeric + ' numberingSystem=' + show(locale.numberingSystem));\n    lines.push(head + ' firstDayOfWeek => ' + show(locale.firstDayOfWeek));\n    lines.push(head + ' likely => max=' + locale.maximize() + ' min=' + locale.minimize());\n    lines.push(head + ' calendars => ' + show(info(locale, 'calendars')));\n    lines.push(head + ' collations => ' + show(info(locale, 'collations')));\n    lines.push(head + ' hourCycles => ' + show(info(locale, 'hourCycles')));\n    lines.push(head + ' numberingSystems => ' + show(info(locale, 'numberingSystems')));\n    lines.push(head + ' timeZones => ' + show(info(locale, 'timeZones')));\n    lines.push(head + ' direction => ' + show(info(locale, 'textInfo').direction));\n    lines.push(head + ' week => firstDay=' + week.firstDay + ' weekend=' + show(week.weekend));\n  } catch (error) {\n    lines.push(head + ' => ' + error.name);\n  }\n}\n\nlines.map(escape).join('\\n');\n";
var completion = (0, eval)(program);
String(completion).split('\n').forEach(function (line) { out(line); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
