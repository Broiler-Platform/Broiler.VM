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
// SJL-9.1 sub-area (d), row T27: the numbering system of a date format: the digits of each numeric
// field, the fractional-second separator, and the resolved numberingSystem. FormatDateTimePattern
// (e8cc03fc) formats every number through an Intl.NumberFormat with the format's numberingSystem and
// useGrouping false.
var A = Date.UTC(2024, 0, 5, 15, 7, 9, 123);
['arab', 'arabext', 'beng', 'deva', 'hanidec', 'thai', 'fullwide', 'latn', 'tamldec'].forEach(function (nu) {
  t('en-u-nu-' + nu + ' [numberingSystem, default, {hour,minute,second,fractionalSecondDigits:3}, dateStyle medium]', function () {
    var o = { timeZone: 'UTC' };
    var f1 = new Intl.DateTimeFormat('en-u-nu-' + nu, o);
    var f2 = new Intl.DateTimeFormat('en-u-nu-' + nu, { hour: 'numeric', minute: '2-digit', second: '2-digit', fractionalSecondDigits: 3, hourCycle: 'h23', timeZone: 'UTC' });
    var f3 = new Intl.DateTimeFormat('en-u-nu-' + nu, { dateStyle: 'medium', timeZone: 'UTC' });
    return [f1.resolvedOptions().numberingSystem, f1.format(A), f2.format(A), f3.format(A)];
  });
});
t('de {numberingSystem:arab} default and fractional second', function () { return [new Intl.DateTimeFormat('de', { numberingSystem: 'arab', timeZone: 'UTC' }).format(A), new Intl.DateTimeFormat('de', { numberingSystem: 'arab', second: 'numeric', fractionalSecondDigits: 2, timeZone: 'UTC' }).format(A)]; });
t('en {numberingSystem:hanidec} year 2024 and 10000', function () { var f = new Intl.DateTimeFormat('en', { numberingSystem: 'hanidec', year: 'numeric', timeZone: 'UTC' }); var d = new Date(0); d.setUTCFullYear(10000); return [f.format(A), f.format(d)]; });
t('en-u-nu-arab {timeZoneName:shortOffset} in Asia/Kolkata', function () { return new Intl.DateTimeFormat('en-u-nu-arab', { hour: 'numeric', timeZoneName: 'shortOffset', timeZone: 'Asia/Kolkata' }).format(A); });
t('en-u-nu-arab formatToParts', function () { return parts(new Intl.DateTimeFormat('en-u-nu-arab', { year: 'numeric', month: 'numeric', day: 'numeric', timeZone: 'UTC' }).formatToParts(A)); });
t('ar-EG default numberingSystem', function () { return new Intl.DateTimeFormat('ar-EG').resolvedOptions().numberingSystem; });
t('en {numberingSystem:"arab"} with -u-nu-latn', function () { var r = new Intl.DateTimeFormat('en-u-nu-latn', { numberingSystem: 'arab' }).resolvedOptions(); return [r.locale, r.numberingSystem]; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
