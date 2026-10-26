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
// Rows M4 and M5: SetNumberFormatDigitOptions (ECMA-402 16.1.2): the reads, their order and count, the
// refusals, and the defaults each style hands it (16.1.1: currency digits, percent 0, else 0 and 3).
function nf(o) { return new Intl.NumberFormat('en', o); }
function ro(o) { var r = nf(o).resolvedOptions(); var keys = ['minimumIntegerDigits', 'minimumFractionDigits', 'maximumFractionDigits', 'minimumSignificantDigits', 'maximumSignificantDigits', 'roundingIncrement', 'roundingMode', 'roundingPriority', 'trailingZeroDisplay']; var a = []; keys.forEach(function (k) { if (k in r) a.push(k + '=' + show(r[k])); }); return R(a.join(' ')); }
var all = { localeMatcher: 'best fit', numberingSystem: 'latn', style: 'decimal', currency: 'EUR', currencyDisplay: 'symbol', currencySign: 'standard', unit: 'meter', unitDisplay: 'short', notation: 'standard', minimumIntegerDigits: 1, minimumFractionDigits: 0, maximumFractionDigits: 3, minimumSignificantDigits: 1, maximumSignificantDigits: 21, roundingIncrement: 1, roundingMode: 'halfExpand', roundingPriority: 'auto', trailingZeroDisplay: 'auto', compactDisplay: 'short', useGrouping: 'auto', signDisplay: 'auto' };
t('M4 NumberFormat option reads, in order', function () { var log = []; nf(spy(all, log)); return log; });
t('M4 PluralRules option reads, in order', function () { var log = []; new Intl.PluralRules('en', spy({ localeMatcher: 'best fit', type: 'cardinal', notation: 'standard', compactDisplay: 'short', minimumIntegerDigits: 1, minimumFractionDigits: 0, maximumFractionDigits: 3, minimumSignificantDigits: 1, maximumSignificantDigits: 21, roundingIncrement: 1, roundingMode: 'halfExpand', roundingPriority: 'auto', trailingZeroDisplay: 'auto' }, log)); return log; });
t('M4 NumberFormat: the reads after construction, by format then by resolvedOptions', function () { var log = []; var f = nf(spy({ maximumFractionDigits: 2, roundingMode: 'floor', trailingZeroDisplay: 'auto' }, log)); var n = log.length; f.format(1.234); var a = log.slice(n); n = log.length; f.resolvedOptions(); return [a, log.slice(n)]; });
t('M4 PluralRules: the reads after construction, by select then by resolvedOptions', function () { var log = []; var p = new Intl.PluralRules('en', spy({ maximumFractionDigits: 2, roundingMode: 'floor' }, log)); var n = log.length; p.select(1.234); var a = log.slice(n); n = log.length; p.resolvedOptions(); return [a, log.slice(n)]; });
t('M4 refusals: roundingIncrement 3', function () { return nf({ roundingIncrement: 3 }); });
t('M4 refusals: roundingIncrement 5 with maxSD 2 (TypeError)', function () { return nf({ roundingIncrement: 5, maximumSignificantDigits: 2 }); });
t('M4 refusals: roundingIncrement 5 with maxFD 2 (mnfd 0)', function () { return nf({ roundingIncrement: 5, maximumFractionDigits: 2 }); });
t('M4 refusals: roundingIncrement 5 with roundingPriority morePrecision', function () { return nf({ roundingIncrement: 5, minimumFractionDigits: 2, maximumFractionDigits: 2, roundingPriority: 'morePrecision' }); });
t('M4 refusals: minFD 3 > maxFD 2', function () { return nf({ minimumFractionDigits: 3, maximumFractionDigits: 2 }); });
t('M4 refusals: maxFD 101; minSD 0; minSD 22; minSD 5 maxSD 4; minID 0; minID 22', function () {
  var r = [];
  [{ maximumFractionDigits: 101 }, { minimumSignificantDigits: 0 }, { minimumSignificantDigits: 22 }, { minimumSignificantDigits: 5, maximumSignificantDigits: 4 }, { minimumIntegerDigits: 0 }, { minimumIntegerDigits: 22 }].forEach(function (o) { try { nf(o); r.push('ok'); } catch (e) { r.push(errName(e)); } });
  return r;
});
t('M4 refusals: roundingMode "half-even"; roundingPriority "more"; trailingZeroDisplay "strip"', function () { var r = []; [{ roundingMode: 'half-even' }, { roundingPriority: 'more' }, { trailingZeroDisplay: 'strip' }].forEach(function (o) { try { nf(o); r.push('ok'); } catch (e) { r.push(errName(e)); } }); return r; });
t('M4 maxFD 1.9 floors; minSD "2" coerces', function () { return [ro({ maximumFractionDigits: 1.9 }), ro({ minimumSignificantDigits: '2' })]; });
t('M5 defaults: {}', function () { return ro({}); });
t('M5 defaults: maxFD 1 / minFD 5', function () { return [ro({ maximumFractionDigits: 1 }), ro({ minimumFractionDigits: 5 })]; });
t('M5 defaults: percent; percent minFD 1', function () { return [ro({ style: 'percent' }), ro({ style: 'percent', minimumFractionDigits: 1 })]; });
t('M5 defaults: currency JPY, KWD, EUR maxFD 0', function () { return [ro({ style: 'currency', currency: 'JPY' }), ro({ style: 'currency', currency: 'KWD' }), ro({ style: 'currency', currency: 'EUR', maximumFractionDigits: 0 })]; });
t('M5 defaults: compact; currency EUR compact; compact maxFD 1', function () { return [ro({ notation: 'compact' }), ro({ style: 'currency', currency: 'EUR', notation: 'compact' }), ro({ notation: 'compact', maximumFractionDigits: 1 })]; });
t('M5 defaults: scientific currency EUR', function () { return ro({ style: 'currency', currency: 'EUR', notation: 'scientific' }); });
t('M5 defaults: maxSD 3; morePrecision maxSD 3; lessPrecision', function () { return [ro({ maximumSignificantDigits: 3 }), ro({ roundingPriority: 'morePrecision', maximumSignificantDigits: 3 }), ro({ roundingPriority: 'lessPrecision' })]; });
t('M5 defaults: roundingIncrement 25 (mxfd default becomes mnfd)', function () { return ro({ roundingIncrement: 25, minimumFractionDigits: 2 }); });
t('M5 percent 0.125, 0.5, 0.0049', function () { var f = nf({ style: 'percent' }); return [f.format(0.125), f.format(0.5), f.format(0.0049)]; });
t('M5 percent maxFD 2: 0.12345', function () { return nf({ style: 'percent', maximumFractionDigits: 2 }).format(0.12345); });
t('M5 (0.125).toLocaleString("en", {style: "percent"})', function () { return (0.125).toLocaleString('en', { style: 'percent' }); });
t('M5 currency JPY 1.5, KWD 1.23456, EUR 1.005', function () { return [nf({ style: 'currency', currency: 'JPY' }).format(1.5), nf({ style: 'currency', currency: 'KWD' }).format(1.23456), nf({ style: 'currency', currency: 'EUR' }).format(1.005)]; });
t('M5 minimumIntegerDigits 3: 5, 5 with maxSD 1, 1234', function () { var f = nf({ minimumIntegerDigits: 3 }); return [f.format(5), nf({ minimumIntegerDigits: 3, maximumSignificantDigits: 1 }).format(5), f.format(1234)]; });
t('M4 ToNumber of a digit option: valueOf calls by the constructor, then by format, then by resolvedOptions', function () { var n = 0; var v = { valueOf: function () { n++; return 2; } }; var f = nf({ maximumFractionDigits: v, minimumSignificantDigits: v }); var a = n; f.format(1.23456); var b = n; var r = f.resolvedOptions(); return [a, b - a, n - b, typeof r.minimumSignificantDigits]; });
t('M4 PluralRules: valueOf calls of a digit option by the constructor, select, resolvedOptions', function () { var n = 0; var v = { valueOf: function () { n++; return 2; } }; var p = new Intl.PluralRules('en', { maximumFractionDigits: v }); var a = n; p.select(1.23456); var b = n; p.resolvedOptions(); return [a, b - a, n - b]; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
