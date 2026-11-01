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
// O14b: Broiler.JS's bundle sjl-17-001 invariant-probe.js, each collator made for the locale Broiler.JS's policy resolves under invariant globalization (en and en-US kept, every other request en-US), as invariant-probe.expected.txt was written by the reference; print is the prelude's out.
var print = out;
var Real = Intl.Collator, realCompare = String.prototype.localeCompare;
function policy(l) { var t = l === undefined ? undefined : String(l); return t !== undefined && /^en(-|$)/.test(t) ? t : 'en-US'; }
Intl.Collator = function (l, o) { return new Real(policy(l), o); };
String.prototype.localeCompare = function (that, l, o) { return realCompare.call(this, that, policy(l), o); };
var words = ['b', 'B', 'a', 'A', 'ä', 'Ä', 'ae', 'Ae', 'af', 'z', 'é', 'e', 'E', 'ñ', 'n', 'o', 'ö', 'oe',
  'ß', 'ss', 'st', '1', '2', '10', 'a1', 'a2', 'a10', ' ', '-', '_', 'a-b', 'a b', 'ab', 'aB', 'α', 'а', 'å', 'v', 'w'];
function sgn(n) { return n < 0 ? '<' : n > 0 ? '>' : '='; }
function q(s) {
  var r = '';
  for (var i = 0; i < s.length; i++) {
    var c = s.charCodeAt(i);
    r += c < 0x20 || c > 0x7e || c === 0x22 || c === 0x5c ? '\\u' + ('0000' + c.toString(16)).slice(-4) : s.charAt(i);
  }
  return '"' + r + '"';
}
function sorted(c) {
  var w = words.slice().sort(c.compare), s = q(w[0]);
  for (var i = 1; i < w.length; i++) s += ' ' + sgn(c.compare(w[i - 1], w[i])) + ' ' + q(w[i]);
  return s;
}
function line(label, fn) {
  var r;
  try { r = fn(); } catch (e) { r = 'abort: ' + ((e && e.constructor && e.constructor.name) || typeof e); }
  print(label + ' | ' + r);
}
var cases = [
  ['en', 'en', undefined],
  ['en-US', 'en-US', undefined],
  ['de', 'de', undefined],
  ['de-u-co-phonebk', 'de-u-co-phonebk', undefined],
  ['de search', 'de', { usage: 'search' }],
  ['sv', 'sv', undefined],
  ['es', 'es', undefined],
  ['th', 'th', undefined],
  ['ja', 'ja', undefined],
  ['zh', 'zh', undefined],
  ['tr', 'tr', undefined],
  ['xx', 'xx', undefined],
  ['default', undefined, undefined],
  ['en base', 'en', { sensitivity: 'base' }],
  ['en accent', 'en', { sensitivity: 'accent' }],
  ['en case', 'en', { sensitivity: 'case' }],
  ['en ignorePunctuation', 'en', { ignorePunctuation: true }],
  ['en numeric', 'en', { numeric: true }],
  ['en caseFirst upper', 'en', { caseFirst: 'upper' }],
  ['en caseFirst lower', 'en', { caseFirst: 'lower' }],
  ['en-u-kn-kf-upper', 'en-u-kn-kf-upper', undefined],
  ['de numeric ignorePunctuation', 'de', { numeric: true, ignorePunctuation: true }]
];
for (var i = 0; i < cases.length; i++) {
  (function (k) {
    line('sort ' + k[0], function () {
      var c = new Intl.Collator(k[1], k[2]);
      return 'resolved ' + c.resolvedOptions().locale + ' | ' + sorted(c);
    });
  })(cases[i]);
}
var pairs = [['a', 'B'], ['B', 'a'], ['ä', 'b'], ['ä', 'ä'], ['Å', 'Å'], ['a­b', 'ab'], ['\ud800', '\udc00'],
  ['\ud800', 'a'], ['a2', 'a10'], ['', ' '], ['A', 'a'], ['ß', 'ss']];
for (var j = 0; j < pairs.length; j++) {
  (function (p) {
    line('localeCompare ' + q(p[0]) + ' ' + q(p[1]), function () {
      return 'resolved ' + new Intl.Collator().resolvedOptions().locale + ' | ' + sgn(p[0].localeCompare(p[1]));
    });
    line('localeCompare de ' + q(p[0]) + ' ' + q(p[1]), function () {
      return 'resolved ' + new Intl.Collator('de').resolvedOptions().locale + ' | ' + sgn(p[0].localeCompare(p[1], 'de'));
    });
  })(pairs[j]);
}
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
