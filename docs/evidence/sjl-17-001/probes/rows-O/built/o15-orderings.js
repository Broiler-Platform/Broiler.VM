// SJL-17.1 (bundle sjl-17-001): bundle sjl-9-001's probes/prelude.js, unchanged from the next line on.
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
// O15: src/tests/cldr/orderings/orderings.js (sha256 d3c968d64f7809eb90219d1452db27ad6a022715bc96c0c4922634f371c6cab8), by indirect eval.
var program = "// SPDX-FileCopyrightText: 2026 Broiler Platform contributors\n// SPDX-License-Identifier: Apache-2.0\n//\n// German and English orderings, the dataset JSD-0027 section 7 asks slice I1 to retain (JSD-0043).\n// Each case sorts the same words with one collator and writes them in order, with `=` between two\n// the collator calls equal and `<` otherwise. The program completes with the lines; it uses no host\n// function, so Node and the profile run the same text. Non-ASCII is escaped both ways, so the\n// retained answers are ASCII.\n\nvar words = [\n  'a', 'A', '\u00e4', '\u00c4', 'ae', 'Ae', 'AE', 'af', 'Af', 'b', 'B',\n  'Apfel', '\u00c4pfel', 'Affe', 'Aepfel',\n  'o', '\u00f6', 'oe', 'Ofen', '\u00d6l', 'Oel', 'offen', '\u00f6ffnen',\n  'u', '\u00fc', 'ue', '\u00dcbel', 'Uebel', 'Ufer',\n  'ss', '\u00df', 'st', 'Stra\u00dfe', 'Strasse', 'Strasze', 'stra\u00dfe',\n  'co-op', 'coop', 'co op', 'Coop', 'co_op',\n  'cote', 'cot\u00e9', 'c\u00f4te', 'c\u00f4t\u00e9',\n  'resume', 'r\u00e9sum\u00e9', 'Resume', 'R\u00e9sum\u00e9',\n  'naive', 'na\u00efve', '\u00e9clair', 'Eclair', 'eclair',\n  '1', '2', '10', '100', 'a1', 'a2', 'a10', 'file-2', 'file10', 'file 3',\n  'Zebra', 'zebra', 'zoo', '\u00c5ngstr\u00f6m', 'Angstrom',\n  '\ufb01le', 'file', 'e\u0301clair', '\u2163', 'IV', 'iv',\n  '', ' ', '-', '!', '#hash', 'hash'\n];\n\nvar cases = [\n  ['en', {}],\n  ['en-US', {}],\n  ['en', { sensitivity: 'base' }],\n  ['en', { sensitivity: 'accent' }],\n  ['en', { sensitivity: 'case' }],\n  ['en', { ignorePunctuation: true }],\n  ['en', { numeric: true }],\n  ['en', { caseFirst: 'upper' }],\n  ['en', { caseFirst: 'lower' }],\n  ['en-u-kn-kf-upper', {}],\n  ['en', { usage: 'search' }],\n  ['en', { usage: 'search', sensitivity: 'base' }],\n  ['de', {}],\n  ['de-DE', { sensitivity: 'base' }],\n  ['de', { sensitivity: 'accent' }],\n  ['de', { caseFirst: 'upper' }],\n  ['de', { numeric: true, ignorePunctuation: true }],\n  ['de-u-co-phonebk', {}],\n  ['de-u-co-phonebk', { sensitivity: 'base' }],\n  ['de-DE-u-co-phonebk', { caseFirst: 'upper' }],\n  ['de-u-co-phonebk', { sensitivity: 'case' }],\n  ['de-u-co-phonebk', { caseFirst: 'lower' }],\n  ['de', { usage: 'search' }],\n  ['de', { usage: 'search', sensitivity: 'base' }],\n  ['de', { usage: 'search', sensitivity: 'case' }],\n  ['de', { usage: 'search', caseFirst: 'upper' }],\n  ['en', { sensitivity: 'case', caseFirst: 'upper' }]\n];\n\nfunction escape(text) {\n  var out = '';\n  for (var i = 0; i < text.length; i++) {\n    var unit = text.charCodeAt(i);\n    out += unit < 0x20 || unit > 0x7e || text[i] === '\\\\'\n      ? '\\\\u' + ('000' + unit.toString(16)).slice(-4)\n      : text[i];\n  }\n  return out;\n}\n\nfunction quote(text) {\n  return '\"' + escape(text) + '\"';\n}\n\nvar lines = [];\n\nfor (var c = 0; c < cases.length; c++) {\n  var collator = new Intl.Collator(cases[c][0], cases[c][1]);\n  var sorted = words.slice().sort(collator.compare);\n  var line = (c + 1) + ' ' + cases[c][0] + ' ' + JSON.stringify(cases[c][1]) + ' ' + quote(sorted[0]);\n\n  for (var w = 1; w < sorted.length; w++) {\n    line += (collator.compare(sorted[w - 1], sorted[w]) === 0 ? ' = ' : ' < ') + quote(sorted[w]);\n  }\n\n  lines.push(line);\n}\n\nlines.join('\\n');\n";
out((0, eval)(program));
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
