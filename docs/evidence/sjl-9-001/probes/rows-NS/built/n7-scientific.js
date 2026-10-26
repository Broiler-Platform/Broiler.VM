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
// Row N7: scientific and engineering notation: the exponent, its symbols and parts, with each style.
var vals = [123456, 0.000123, -1.5e-7, 1e21, 0, 9.9999, 999.95];
['en', 'de'].forEach(function (l) {
  ['scientific', 'engineering'].forEach(function (n) {
    t(l + ' ' + n, function () { var f = new Intl.NumberFormat(l, { notation: n }); return vals.map(function (v) { return f.format(v); }); });
  });
  t(l + ' scientific formatToParts(-0.000123)', function () { return parts(new Intl.NumberFormat(l, { notation: 'scientific' }).formatToParts(-0.000123)); });
  t(l + ' currency USD scientific 1234.5, engineering 1234567', function () { return [new Intl.NumberFormat(l, { style: 'currency', currency: 'USD', notation: 'scientific' }).format(1234.5), new Intl.NumberFormat(l, { style: 'currency', currency: 'USD', notation: 'engineering' }).format(1234567)]; });
  t(l + ' unit meter scientific 1234.5', function () { return new Intl.NumberFormat(l, { style: 'unit', unit: 'meter', notation: 'scientific' }).format(1234.5); });
  t(l + ' scientific signDisplay always 1234, exceptZero 0', function () { return [new Intl.NumberFormat(l, { notation: 'scientific', signDisplay: 'always' }).format(1234), new Intl.NumberFormat(l, { notation: 'scientific', signDisplay: 'exceptZero' }).format(0)]; });
});
t('en scientific maximumSignificantDigits 2: 123456', function () { return new Intl.NumberFormat('en', { notation: 'scientific', maximumSignificantDigits: 2 }).format(123456); });
t('en currency USD scientific formatToParts(1234.5)', function () { return parts(new Intl.NumberFormat('en', { style: 'currency', currency: 'USD', notation: 'scientific' }).formatToParts(1234.5)); });
t('sv, fa scientific -1.5e-7', function () { return [new Intl.NumberFormat('sv', { notation: 'scientific' }).format(-1.5e-7), new Intl.NumberFormat('fa', { notation: 'scientific' }).format(-1.5e-7)]; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
