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
// Row N1: the decimal pattern and symbols of a locale (separators, minus sign, grouping sizes, the minimum
// grouping digits), in the languages each engine has data for and in the ones test262 and D5 name.
var locs = ['en', 'en-US', 'de', 'de-DE', 'de-AT', 'de-CH', 'fr', 'fr-CH', 'es', 'it', 'pl', 'pt-PT', 'pt-BR', 'sv', 'nb',
  'ru', 'tr', 'nl', 'ja', 'zh', 'ko', 'en-IN', 'hi', 'ar', 'ar-EG', 'he', 'fa', 'th', 'xyz'];
var vals = [1234.5, -1234.5, 1234567.891, 1000, 10000, 0.5];
locs.forEach(function (l) {
  t(l + ' resolved locale, numberingSystem', function () { var r = new Intl.NumberFormat(l).resolvedOptions(); return [r.locale, r.numberingSystem]; });
  vals.forEach(function (v) {
    t(l + ' format(' + v + ')', function () { return new Intl.NumberFormat(l).format(v); });
  });
});
['de', 'fr', 'sv', 'de-CH', 'en-IN', 'ar', 'es'].forEach(function (l) {
  t(l + ' formatToParts(-1234567.5)', function () { return parts(new Intl.NumberFormat(l).formatToParts(-1234567.5)); });
});
t('es useGrouping auto 1234, 12345', function () { var f = new Intl.NumberFormat('es'); return [f.format(1234), f.format(12345)]; });
t('es useGrouping always 1234', function () { return new Intl.NumberFormat('es', { useGrouping: 'always' }).format(1234); });
t('pl useGrouping auto 1234, min2 1234, always 1234', function () { return [new Intl.NumberFormat('pl').format(1234), new Intl.NumberFormat('pl', { useGrouping: 'min2' }).format(1234), new Intl.NumberFormat('pl', { useGrouping: 'always' }).format(1234)]; });
t('en-IN 100000, 10000000 always', function () { var f = new Intl.NumberFormat('en-IN', { useGrouping: 'always' }); return [f.format(100000), f.format(10000000)]; });
t('Number.prototype.toLocaleString de, fr, sv', function () { return [(-1234.5).toLocaleString('de'), (-1234.5).toLocaleString('fr'), (-1234.5).toLocaleString('sv')]; });
t('Array.prototype.toLocaleString de', function () { return [1234.5, -0.5].toLocaleString('de'); });
t('NaN and Infinity in de, fr, ar, zh-TW', function () { return ['de', 'fr', 'ar', 'zh-TW'].map(function (l) { var f = new Intl.NumberFormat(l); return f.format(NaN) + ' | ' + f.format(-Infinity); }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
