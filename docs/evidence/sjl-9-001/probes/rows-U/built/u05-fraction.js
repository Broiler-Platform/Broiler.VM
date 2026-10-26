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
// Row U11: fractional units (ComputeFractionalDigits, NextUnitFractional), summed exactly as a mathematical
// value and written with roundingMode "trunc" and fractionalDigits.
function f(opts, d, label) {
  t('U11 en ' + JSON.stringify(opts) + ' ' + (label || JSON.stringify(d)), function () { return new Intl.DurationFormat('en', opts).format(d); });
}
var D = { style: 'digital' };
f(D, { seconds: 4503599627370497, nanoseconds: 1 });
f(D, { seconds: 10000000, nanoseconds: 1 });
f(D, { seconds: 1, milliseconds: 2, microseconds: 3, nanoseconds: 9007199254740991 });
f(D, { milliseconds: 4503599627370497000, microseconds: 4503599627370495000000 }, '{milliseconds: 4503599627370497000, microseconds: 4503599627370495000000}');
f(D, { seconds: 1, nanoseconds: 5 });
f(D, { microseconds: 1e20 }, '{microseconds: 1e20}');
f(D, { seconds: 0, nanoseconds: 1 });
f({ style: 'digital', fractionalDigits: 0 }, { seconds: 7, milliseconds: 999, microseconds: 999, nanoseconds: 999 });
f({ style: 'digital', fractionalDigits: 3 }, { seconds: 7, milliseconds: 999, microseconds: 999, nanoseconds: 999 });
f({ style: 'digital', fractionalDigits: 9 }, { seconds: 7 });
f({ style: 'short', milliseconds: 'numeric' }, { milliseconds: 1500 });
f({ style: 'long', seconds: 'numeric', fractionalDigits: 3 }, { seconds: 1, milliseconds: 234, microseconds: 567 });
f({ style: 'narrow', microseconds: 'numeric' }, { milliseconds: 1, microseconds: 2, nanoseconds: 3 });
f({ style: 'long', milliseconds: 'numeric' }, { nanoseconds: 1 });
f({ style: 'long', milliseconds: 'numeric' }, { minutes: 1 });
f({ style: 'long', milliseconds: 'numeric', fractionalDigits: 2 }, { seconds: 2, milliseconds: 999 });
f({ style: 'short', microseconds: 'numeric' }, { seconds: 1, milliseconds: 2, microseconds: 3, nanoseconds: 4 });
f({ style: 'short', nanoseconds: 'numeric' }, { seconds: 1, milliseconds: 2, microseconds: 3, nanoseconds: 4 });
f(D, { seconds: -4503599627370497, nanoseconds: -1 });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
