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
// Row P3: which languages' plural rules each engine holds. Each line: the resolved locale, then the
// categories select gives.
function c(loc, xs, o) { var p = new Intl.PluralRules(loc, o); return R(show(p.resolvedOptions().locale) + ' ' + xs.map(function (x) { return p.select(x); }).join(' ')); }
t('P3 fr 0 1 1.5 2 1000000', function () { return c('fr', [0, 1, 1.5, 2, 1000000]); });
t('P3 ru 1 3 5 21 1.5', function () { return c('ru', [1, 3, 5, 21, 1.5]); });
t('P3 pl 1 2 5 22 1.5', function () { return c('pl', [1, 2, 5, 22, 1.5]); });
t('P3 cs 1 3 5 1.5', function () { return c('cs', [1, 3, 5, 1.5]); });
t('P3 uk 1 3 5', function () { return c('uk', [1, 3, 5]); });
t('P3 hi 0 1 2', function () { return c('hi', [0, 1, 2]); });
t('P3 he 1 2 3', function () { return c('he', [1, 2, 3]); });
t('P3 ar 0 1 2 3 11 100', function () { return c('ar', [0, 1, 2, 3, 11, 100]); });
t('P3 cy 0 1 2 3 6', function () { return c('cy', [0, 1, 2, 3, 6]); });
t('P3 ja 0 1 2', function () { return c('ja', [0, 1, 2]); });
t('P3 pt 0 1 1.5; pt-PT 0 1 1.5', function () { return [c('pt', [0, 1, 1.5]), c('pt-PT', [0, 1, 1.5])]; });
t('P3 fr ordinal 1 2; sv ordinal 1 2 3', function () { return [c('fr', [1, 2], { type: 'ordinal' }), c('sv', [1, 2, 3], { type: 'ordinal' })]; });
t('P3 pluralCategories: fr, ru, ar, ja, de ordinal, en ordinal', function () { return [new Intl.PluralRules('fr').resolvedOptions().pluralCategories, new Intl.PluralRules('ru').resolvedOptions().pluralCategories, new Intl.PluralRules('ar').resolvedOptions().pluralCategories, new Intl.PluralRules('ja').resolvedOptions().pluralCategories, new Intl.PluralRules('de', { type: 'ordinal' }).resolvedOptions().pluralCategories, new Intl.PluralRules('en', { type: 'ordinal' }).resolvedOptions().pluralCategories]; });
t('P3 supportedLocalesOf fr ru de en-GB zz', function () { return Intl.PluralRules.supportedLocalesOf(['fr', 'ru', 'de', 'en-GB', 'zz']); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
