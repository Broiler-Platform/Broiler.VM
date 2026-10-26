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
// Row P1: Intl.PluralRules.prototype.select resolves over FormatNumericToString's string (ResolvePlural,
// ECMA-402 17.5.2), so the digit options and the rounding decide the operands.
function pr(o) { return new Intl.PluralRules('en', o); }
t('P1 en {minFD 1}.select(1); de {minFD 1}.select(1)', function () { return [pr({ minimumFractionDigits: 1 }).select(1), new Intl.PluralRules('de', { minimumFractionDigits: 1 }).select(1)]; });
t('P1 en {maxFD 0}.select(1.4); select(0.6)', function () { var p = pr({ maximumFractionDigits: 0 }); return [p.select(1.4), p.select(0.6)]; });
t('P1 en default (maxFD 3): select(1.0001), select(0.9999), select(1.0005), select(1.5)', function () { var p = pr(); return [p.select(1.0001), p.select(0.9999), p.select(1.0005), p.select(1.5)]; });
t('P1 en {maxSD 1}.select(1.5); {minSD 2}.select(1)', function () { return [pr({ maximumSignificantDigits: 1 }).select(1.5), pr({ minimumSignificantDigits: 2 }).select(1)]; });
t('P1 en {maxFD 0, roundingMode floor}.select(1.9); {maxFD 0, roundingMode ceil}.select(0.1)', function () { return [pr({ maximumFractionDigits: 0, roundingMode: 'floor' }).select(1.9), pr({ maximumFractionDigits: 0, roundingMode: 'ceil' }).select(0.1)]; });
t('P1 en {minFD 1, trailingZeroDisplay stripIfInteger}.select(1)', function () { return pr({ minimumFractionDigits: 1, trailingZeroDisplay: 'stripIfInteger' }).select(1); });
t('P1 en {minimumIntegerDigits 2}.select(1)', function () { return pr({ minimumIntegerDigits: 2 }).select(1); });
t('P1 en {roundingIncrement 5, minFD 1, maxFD 1}.select(0.98)', function () { return pr({ roundingIncrement: 5, minimumFractionDigits: 1, maximumFractionDigits: 1 }).select(0.98); });
t('P1 en select("1.0"), select(" 1 "), select("1.50"), select("abc")', function () { var p = pr(); return [p.select('1.0'), p.select(' 1 '), p.select('1.50'), p.select('abc')]; });
t('P1 en select({valueOf: "1"}), ToPrimitive count', function () { var n = 0; var r = pr().select({ valueOf: function () { n++; return '1'; } }); return [r, n]; });
t('P1 en ordinal select 1 2 3 4 11 12 13 21 22 23 101 111 1.5', function () { var p = pr({ type: 'ordinal' }); return [1, 2, 3, 4, 11, 12, 13, 21, 22, 23, 101, 111, 1.5].map(function (x) { return p.select(x); }).join(' '); });
t('P1 en ordinal {minFD 1}.select(1); {maxFD 0}.select(1.4)', function () { return [pr({ type: 'ordinal', minimumFractionDigits: 1 }).select(1), pr({ type: 'ordinal', maximumFractionDigits: 0 }).select(1.4)]; });
t('P1 de ordinal select(1); de select 0 1 2 1.5', function () { var o = new Intl.PluralRules('de', { type: 'ordinal' }); var c = new Intl.PluralRules('de'); return [o.select(1), [0, 1, 2, 1.5].map(function (x) { return c.select(x); }).join(' ')]; });
t('P1 en select NaN Infinity -Infinity 1e21 -1 undefined', function () { var p = pr(); return [p.select(NaN), p.select(Infinity), p.select(-Infinity), p.select(1e21), p.select(-1), p.select(undefined)]; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
