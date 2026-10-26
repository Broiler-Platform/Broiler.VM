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
// Rows M1 and M1b: ToIntlMathematicalValue of a Number. ECMA-402 reads a Number through Number::toString's
// digits (the shortest round-trip decimal), never through its exact binary value.
function nf(o) { return new Intl.NumberFormat('en', o); }
t('M1 maxFD 1: 1.15, 1.25, 1.35, 1.45, -1.15', function () { var f = nf({ maximumFractionDigits: 1 }); return [f.format(1.15), f.format(1.25), f.format(1.35), f.format(1.45), f.format(-1.15)]; });
t('M1 maxFD 2: 1.005, 2.675, 1.255, 8.345, 0.615', function () { var f = nf({ maximumFractionDigits: 2 }); return [f.format(1.005), f.format(2.675), f.format(1.255), f.format(8.345), f.format(0.615)]; });
t('M1 maxFD 2 halfEven: 1.015, 1.025, 0.125', function () { var f = nf({ maximumFractionDigits: 2, roundingMode: 'halfEven' }); return [f.format(1.015), f.format(1.025), f.format(0.125)]; });
t('M1 maxFD 0 halfTrunc: 2.5, 0.5, 1e21+0.5 (no fraction)', function () { var f = nf({ maximumFractionDigits: 0, roundingMode: 'halfTrunc' }); return [f.format(2.5), f.format(0.5), f.format(1e21)]; });
t('M1 minSD 4 (maxSD 21): 1.005', function () { return nf({ minimumSignificantDigits: 4 }).format(1.005); });
t('M1 maxSD 21: 0.1 + 0.2', function () { return nf({ maximumSignificantDigits: 21 }).format(0.1 + 0.2); });
t('M1 maxFD 20: 0.1', function () { return nf({ maximumFractionDigits: 20 }).format(0.1); });
t('M1 maxSD 21: 5e-324', function () { return nf({ maximumSignificantDigits: 21 }).format(5e-324); });
t('M1 maxSD 21: 1.7976931348623157e308 (length, first 30)', function () { var s = nf({ maximumSignificantDigits: 21, useGrouping: false }).format(1.7976931348623157e308); return [s.length, s.slice(0, 30)]; });
t('M1 default: 123456789.12345679, 1e21, 9007199254740993', function () { var f = nf(); return [f.format(123456789.12345679), f.format(1e21), f.format(9007199254740993)]; });
t('M1 toLocaleString maxFD 1: (1.15), (2.675) maxFD 2', function () { return [(1.15).toLocaleString('en', { maximumFractionDigits: 1 }), (2.675).toLocaleString('en', { maximumFractionDigits: 2 })]; });
t('M1 formatToParts maxFD 1: 1.15', function () { return parts(nf({ maximumFractionDigits: 1 }).formatToParts(1.15)); });
t('M1 lessPrecision maxSD 2 maxFD 1: 1.15', function () { return nf({ maximumSignificantDigits: 2, maximumFractionDigits: 1, roundingPriority: 'lessPrecision' }).format(1.15); });
// M1b: an object whose ToPrimitive is a String takes the String path (exact), not ToNumber.
t('M1b {valueOf: "1.15"} maxFD 1', function () { return nf({ maximumFractionDigits: 1 }).format({ valueOf: function () { return '1.15'; } }); });
t('M1b {toString: "1.15"} maxFD 1', function () { return nf({ maximumFractionDigits: 1 }).format({ toString: function () { return '1.15'; } }); });
t('M1b {valueOf: "9007199254740993"} default', function () { return nf().format({ valueOf: function () { return '9007199254740993'; } }); });
t('M1b ToPrimitive count on format({valueOf})', function () { var n = 0; nf().format({ valueOf: function () { n++; return 1; } }); return n; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
