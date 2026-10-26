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
// Row N6: compact notation: the locale's patterns and thresholds, the exponent chosen after rounding, the
// default rounding (two significant digits or whole numbers), plural compact patterns, compact currency.
var vals = [999, 999.5, 999.995, 1000, 1500, 12345, 99999, 999999, 1e6, 1.2e6, 2e6, 1e9, 1e12, 1.5e15, 0.012345, -12345];
['en', 'de', 'ja', 'ko', 'zh', 'zh-TW', 'en-IN', 'fr', 'es', 'pt', 'ru'].forEach(function (l) {
  ['short', 'long'].forEach(function (d) {
    t(l + ' compact ' + d, function () { var f = new Intl.NumberFormat(l, { notation: 'compact', compactDisplay: d }); return vals.map(function (v) { return f.format(v); }); });
  });
});
t('de compact long 1e6, 2e6, 1e9, 2e9 (plural)', function () { var f = new Intl.NumberFormat('de', { notation: 'compact', compactDisplay: 'long' }); return [f.format(1e6), f.format(2e6), f.format(1e9), f.format(2e9)]; });
t('en compact 5e-324, 0.0001234', function () { var f = new Intl.NumberFormat('en', { notation: 'compact' }); return [f.format(5e-324), f.format(0.0001234)]; });
t('en compact "123456789012345678901234567890.5"', function () { return new Intl.NumberFormat('en', { notation: 'compact' }).format('123456789012345678901234567890.5'); });
t('en compact maximumSignificantDigits 3: 12345, 1234567', function () { var f = new Intl.NumberFormat('en', { notation: 'compact', maximumSignificantDigits: 3 }); return [f.format(12345), f.format(1234567)]; });
t('en compact maximumFractionDigits 2: 12345, 1234567', function () { var f = new Intl.NumberFormat('en', { notation: 'compact', maximumFractionDigits: 2 }); return [f.format(12345), f.format(1234567)]; });
t('en compact roundingMode floor 1990', function () { return new Intl.NumberFormat('en', { notation: 'compact', roundingMode: 'floor' }).format(1990); });
t('en compact formatToParts(-1234567)', function () { return parts(new Intl.NumberFormat('en', { notation: 'compact' }).formatToParts(-1234567)); });
t('en compact long formatToParts(1234567)', function () { return parts(new Intl.NumberFormat('en', { notation: 'compact', compactDisplay: 'long' }).formatToParts(1234567)); });
t('ja compact formatToParts(987654321)', function () { return parts(new Intl.NumberFormat('ja-JP', { notation: 'compact' }).formatToParts(987654321)); });
t('en compact useGrouping default 1234 (no compact), 12345678912', function () { var f = new Intl.NumberFormat('en', { notation: 'compact' }); return [f.format(1234), f.format(1234567891234567)]; });
['en', 'de'].forEach(function (l) {
  t(l + ' currency USD compact 0, 1234.5, 1234567, -1234567', function () { var f = new Intl.NumberFormat(l, { style: 'currency', currency: 'USD', notation: 'compact' }); return [f.format(0), f.format(1234.5), f.format(1234567), f.format(-1234567)]; });
  t(l + ' currency EUR compact long 1234567', function () { return new Intl.NumberFormat(l, { style: 'currency', currency: 'EUR', notation: 'compact', compactDisplay: 'long' }).format(1234567); });
  t(l + ' currency EUR name compact 1234567', function () { return new Intl.NumberFormat(l, { style: 'currency', currency: 'EUR', currencyDisplay: 'name', notation: 'compact' }).format(1234567); });
});
t('en currency USD compact formatToParts(1234567)', function () { return parts(new Intl.NumberFormat('en', { style: 'currency', currency: 'USD', notation: 'compact' }).formatToParts(1234567)); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
