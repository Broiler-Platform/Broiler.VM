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
// Row M6: the rounding FormatNumericToString applies (ECMA-402 16.5.3): rounding modes, increments,
// rounding priority, trailingZeroDisplay.
function nf(o) { return new Intl.NumberFormat('en', o); }
var modes = ['ceil', 'floor', 'expand', 'trunc', 'halfCeil', 'halfFloor', 'halfExpand', 'halfTrunc', 'halfEven'];
modes.forEach(function (m) {
  t('M6 ' + m + ' maxFD 0: -2.5 -1.5 -0.5 0.5 1.5 2.5 -0.4 0.6', function () { var f = nf({ maximumFractionDigits: 0, roundingMode: m }); return [-2.5, -1.5, -0.5, 0.5, 1.5, 2.5, -0.4, 0.6].map(function (x) { return f.format(x); }).join(' '); });
});
t('M6 increment 25 minFD 2 maxFD 2: 1.13, 1.12, 1.125 halfEven, 1.375 halfEven', function () { var f = nf({ roundingIncrement: 25, minimumFractionDigits: 2, maximumFractionDigits: 2 }); var g = nf({ roundingIncrement: 25, minimumFractionDigits: 2, maximumFractionDigits: 2, roundingMode: 'halfEven' }); return [f.format(1.13), f.format(1.12), g.format(1.125), g.format(1.375)]; });
t('M6 increment 5000 maxFD 0... minFD 0 maxFD 0: 12345, 7499, 7500', function () { var f = nf({ roundingIncrement: 5000, maximumFractionDigits: 0 }); return [f.format(12345), f.format(7499), f.format(7500)]; });
t('M6 morePrecision minSD 2 maxFD 0: 1.234; 123.4', function () { var f = nf({ roundingPriority: 'morePrecision', maximumSignificantDigits: 2, maximumFractionDigits: 0 }); return [f.format(1.234), f.format(123.4)]; });
t('M6 lessPrecision maxSD 2 maxFD 0: 1.234; 123.4', function () { var f = nf({ roundingPriority: 'lessPrecision', maximumSignificantDigits: 2, maximumFractionDigits: 0 }); return [f.format(1.234), f.format(123.4)]; });
t('M6 maxSD 3 minSD 3: 0, 9.995, 0.000123456', function () { var f = nf({ maximumSignificantDigits: 3, minimumSignificantDigits: 3 }); return [f.format(0), f.format(9.995), f.format(0.000123456)]; });
t('M7 trailingZeroDisplay stripIfInteger minFD 2: 5, 5.1, 4.9999, 5.001', function () { var f = nf({ minimumFractionDigits: 2, trailingZeroDisplay: 'stripIfInteger' }); return [f.format(5), f.format(5.1), f.format(4.9999), f.format(5.001)]; });
t('M7 trailingZeroDisplay stripIfInteger minSD 2 maxSD 2: 1, 1.5', function () { var f = nf({ minimumSignificantDigits: 2, maximumSignificantDigits: 2, trailingZeroDisplay: 'stripIfInteger' }); return [f.format(1), f.format(1.5)]; });
t('M7 trailingZeroDisplay stripIfInteger currency EUR: 5, 5.5', function () { var f = nf({ style: 'currency', currency: 'EUR', trailingZeroDisplay: 'stripIfInteger' }); return [f.format(5), f.format(5.5)]; });
t('M7 trailingZeroDisplay stripIfInteger formatToParts minFD 2: 5', function () { return parts(nf({ minimumFractionDigits: 2, trailingZeroDisplay: 'stripIfInteger' }).formatToParts(5)); });
t('M7 trailingZeroDisplay stripIfInteger percent minFD 1: 0.05', function () { return nf({ style: 'percent', minimumFractionDigits: 1, trailingZeroDisplay: 'stripIfInteger' }).format(0.05); });
t('M6 increment 25 with minFD 2 only (maxFD resolves to 2): 1.13, 1.12; resolvedOptions maxFD', function () { var f = nf({ roundingIncrement: 25, minimumFractionDigits: 2 }); return [f.format(1.13), f.format(1.12), f.resolvedOptions().maximumFractionDigits]; });
t('M6 increment 5 with no fraction option (both resolve to 0): 7, 12; currency USD increment 5: 1.23', function () { return [nf({ roundingIncrement: 5 }).format(7), nf({ roundingIncrement: 5 }).format(12), nf({ style: 'currency', currency: 'USD', roundingIncrement: 5 }).format(1.23)]; });
t('M6 increment 5 currency JPY (cDigits 0): 7', function () { return nf({ style: 'currency', currency: 'JPY', roundingIncrement: 5 }).format(7); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
