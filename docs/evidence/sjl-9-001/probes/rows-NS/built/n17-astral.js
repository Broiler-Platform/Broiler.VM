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
// Rows N17a (a numbering system whose digits are outside the BMP: decoding the ten digits) and N17b (the range
// collapse's lengths, counted in code points): mathbold and segment digits in format, formatToParts and formatRange,
// with an exponent (the inner layer) and with affixes (the middle layer) around astral digits.
['en-u-nu-mathbold', 'en-u-nu-segment', 'de-u-nu-mathbold'].forEach(function (l) {
  t(l + ' format 0123456789, -1234.5', function () { var f = new Intl.NumberFormat(l, { useGrouping: false }); return [f.format(1234567890), f.format(-1234.5)]; });
  t(l + ' formatToParts(-1.5)', function () { return parts(new Intl.NumberFormat(l).formatToParts(-1.5)); });
  t(l + ' formatRange 3..5, -5..-1, 1000..5000', function () { var f = new Intl.NumberFormat(l); return [f.formatRange(3, 5), f.formatRange(-5, -1), f.formatRange(1000, 5000)]; });
  t(l + ' scientific formatRange 1000..5000, 1..1', function () { var f = new Intl.NumberFormat(l, { notation: 'scientific' }); return [f.formatRange(1000, 5000), f.formatRange(1, 1)]; });
  t(l + ' EUR code formatRange 3..5; percent 0.03..0.05; compact 3000..5000', function () {
    return [new Intl.NumberFormat(l, { style: 'currency', currency: 'EUR', currencyDisplay: 'code' }).formatRange(3, 5),
      new Intl.NumberFormat(l, { style: 'percent' }).formatRange(0.03, 0.05),
      new Intl.NumberFormat(l, { notation: 'compact' }).formatRange(3000, 5000)];
  });
  t(l + ' formatRangeToParts(3, 5) EUR code', function () { return parts(new Intl.NumberFormat(l, { style: 'currency', currency: 'EUR', currencyDisplay: 'code' }).formatRangeToParts(3, 5)); });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
