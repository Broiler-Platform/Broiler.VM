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
// Row M3: ToIntlMathematicalValue of a BigInt: its exact value, in every notation.
function f(o) { return new Intl.NumberFormat('en', o); }
t('M3 default 12345678901234567890123n', function () { return f().format(12345678901234567890123n); });
t('M3 scientific maxSD 21: 123456789012345678901n', function () { return f({ notation: 'scientific', maximumSignificantDigits: 21 }).format(123456789012345678901n); });
t('M3 engineering maxSD 21: 123456789012345678901n', function () { return f({ notation: 'engineering', maximumSignificantDigits: 21 }).format(123456789012345678901n); });
t('M3 scientific default: 123456789012345678901n', function () { return f({ notation: 'scientific' }).format(123456789012345678901n); });
t('M3 maxSD 3: 12345n; -5n; 0n', function () { var g = f({ maximumSignificantDigits: 3 }); return [g.format(12345n), g.format(-5n), g.format(0n)]; });
t('M3 percent: 1n', function () { return f({ style: 'percent' }).format(1n); });
t('M3 10n**30n formatToParts', function () { return parts(f().formatToParts(10n ** 30n)); });
t('M3 10n**400n (length, first, last)', function () { var s = f({ useGrouping: false }).format(10n ** 400n); return [s.length, s.slice(0, 3), s.slice(-3)]; });
t('M3 (2n**64n+1n).toLocaleString en', function () { return (2n ** 64n + 1n).toLocaleString('en'); });
t('M3 BigInt toLocaleString maxSD 21 scientific 123456789012345678901n', function () { return (123456789012345678901n).toLocaleString('en', { notation: 'scientific', maximumSignificantDigits: 21 }); });
t('M3 Object(5n) ToPrimitive', function () { return f().format(Object(5n)); });
t('M3 10n**308n and 10n**309n and -(10n**400n) (lengths)', function () { var g = f({ useGrouping: false }); return [g.format(10n ** 308n).length, g.format(10n ** 309n).length, g.format(-(10n ** 400n)).length]; });
t('M3 PluralRules en select(10n**400n)', function () { return new Intl.PluralRules('en').select(10n ** 400n); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
