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
// Row M2: ToIntlMathematicalValue of a String: StringIntlMV of a StringNumericLiteral, exactly, then
// RoundMVResult's test for the infinities and zeros; anything else is not-a-number.
function f(o) { return new Intl.NumberFormat('en', o); }
var d = f();
t('M2 "1.15" maxFD 1; "1.005" maxFD 2', function () { return [f({ maximumFractionDigits: 1 }).format('1.15'), f({ maximumFractionDigits: 2 }).format('1.005')]; });
t('M2 " 12345678901234567890.5 "', function () { return d.format(' 12345678901234567890.5 '); });
t('M2 "9007199254740993"', function () { return d.format('9007199254740993'); });
t('M2 "1e3", ".5", "5.", "+.5e1", "-.5E-1"', function () { return [d.format('1e3'), d.format('.5'), d.format('5.'), d.format('+.5e1'), d.format('-.5E-1')]; });
t('M2 "", "   ", "-0", "+0", "-0.0001"', function () { return [d.format(''), d.format('   '), d.format('-0'), d.format('+0'), d.format('-0.0001')]; });
t('M2 "0x1F", "0b101", "0o17", "0X1f"', function () { return [d.format('0x1F'), d.format('0b101'), d.format('0o17'), d.format('0X1f')]; });
t('M2 "-0x1F", "0x", "1_000", "1e", "abc", "1n", "--1"', function () { return [d.format('-0x1F'), d.format('0x'), d.format('1_000'), d.format('1e'), d.format('abc'), d.format('1n'), d.format('--1')]; });
t('M2 "Infinity", "+Infinity", "-Infinity", " Infinity ", "infinity"', function () { return [d.format('Infinity'), d.format('+Infinity'), d.format('-Infinity'), d.format(' Infinity '), d.format('infinity')]; });
t('M2 "1e309", "-1e309", "1e-400", "-1e-400"', function () { return [d.format('1e309'), d.format('-1e309'), d.format('1e-400'), d.format('-1e-400')]; });
t('M2 "1.7976931348623158e308" (rounds to MAX) vs "1.7976931348623159e308" (to Infinity), lengths', function () { var g = f({ useGrouping: false }); return [g.format('1.7976931348623158e308').length, g.format('1.7976931348623159e308')]; });
t('M2 "2.4703282292062328e-324" (to MIN) vs "2.4703282292062327e-324" (to 0) maxSD 2', function () { var g = f({ maximumSignificantDigits: 2 }); return [g.format('2.4703282292062328e-324').length, g.format('2.4703282292062327e-324')]; });
t('M2 white space: "\\u00a01\\ufeff", "\\u20281\\u2029", "\\u00851"', function () { return [d.format('\u00a01\ufeff'), d.format('\u20281\u2029'), d.format('\u00851')]; });
t('M2 "1.0000000000000000000001" maxFD 25', function () { return f({ maximumFractionDigits: 25 }).format('1.0000000000000000000001'); });
t('M2 "1e-7" default and maxSD 3', function () { return [d.format('1e-7'), f({ maximumSignificantDigits: 3 }).format('1e-7')]; });
t('M2 "0.5" halfEven maxFD 0; "1.5"; "2.5"; "-2.5"', function () { var g = f({ maximumFractionDigits: 0, roundingMode: 'halfEven' }); return [g.format('0.5'), g.format('1.5'), g.format('2.5'), g.format('-2.5')]; });
t('M2 Symbol throws', function () { return d.format(Symbol('x')); });
t('M2 formatToParts("-0")', function () { return parts(d.formatToParts('-0')); });
t('M2 maxSD 2: "1e-400" (length), "-1e-400" (length)', function () { var g = f({ maximumSignificantDigits: 2 }); return [g.format('1e-400').length, g.format('-1e-400').length]; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
