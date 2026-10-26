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
// Row N5: style "unit": CLDR's unit patterns by width, compound units, the plural a pattern is chosen by.
function uf(l, u, d, o) { return new Intl.NumberFormat(l, Object.assign({ style: 'unit', unit: u, unitDisplay: d }, o || {})); }
['en', 'de'].forEach(function (l) {
  ['kilometer-per-hour', 'liter', 'megabyte', 'celsius', 'second', 'mile-per-gallon', 'liter-per-kilometer', 'byte-per-second', 'kilobit-per-stone', 'fluid-ounce', 'day'].forEach(function (u) {
    ['short', 'long', 'narrow'].forEach(function (d) {
      t(l + ' ' + u + ' ' + d + ' 1, 2, -1.5', function () { var f = uf(l, u, d); return [f.format(1), f.format(2), f.format(-1.5)]; });
    });
  });
});
t('en liter long 1 with minimumFractionDigits 1', function () { return uf('en', 'liter', 'long', { minimumFractionDigits: 1 }).format(1); });
t('en liter long 1.0001 (max 3 fraction digits)', function () { return uf('en', 'liter', 'long').format(1.0001); });
t('en liter long 0.9999 with maximumFractionDigits 0', function () { return uf('en', 'liter', 'long', { maximumFractionDigits: 0 }).format(0.9999); });
t('en day long 1 and 1n', function () { var f = uf('en', 'day', 'long'); return [f.format(1), f.format(1n)]; });
t('de kilometer-per-hour long formatToParts(-1.5)', function () { return parts(uf('de', 'kilometer-per-hour', 'long').formatToParts(-1.5)); });
t('en kilometer-per-hour short formatToParts(1000)', function () { return parts(uf('en', 'kilometer-per-hour', 'short').formatToParts(1000)); });
t('en megabyte narrow formatToParts(5)', function () { return parts(uf('en', 'megabyte', 'narrow').formatToParts(5)); });
t('en kilometer compact 1234567', function () { return uf('en', 'kilometer', 'short', { notation: 'compact' }).format(1234567); });
t('en kilometer long compact 1234567', function () { return uf('en', 'kilometer', 'long', { notation: 'compact' }).format(1234567); });
['ja', 'ko', 'zh-TW', 'fr', 'es', 'pl'].forEach(function (l) {
  t(l + ' kilometer-per-hour short -987, long 2', function () { return [uf(l, 'kilometer-per-hour', 'short').format(-987), uf(l, 'kilometer-per-hour', 'long').format(2)]; });
});
['METER', 'meter-per', 'kilometer-per-hour-per-second', 'per-meter', 'acre-foot', 'meter\n', 'kılometer'].forEach(function (u) {
  t('invalid unit ' + q(u), function () { return uf('en', u, 'short').resolvedOptions().unit; });
});
t('unit with style decimal is not resolved', function () { var r = new Intl.NumberFormat('en', { unit: 'meter' }).resolvedOptions(); return ['unit' in r, 'unitDisplay' in r]; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
