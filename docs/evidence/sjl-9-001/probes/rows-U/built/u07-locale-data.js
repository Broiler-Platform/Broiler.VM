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
// Rows U13-U16: what the locale's data decides: the clock's separators and two-digit hours ([[DigitalFormat]]), the
// unit and list patterns, the numbering system's digits and symbols, and the resolved locale.
function f(row, loc, opts, d) {
  t(row + ' ' + loc + ' ' + JSON.stringify(opts) + ' ' + JSON.stringify(d), function () {
    var df = new Intl.DurationFormat(loc, opts);
    return df.resolvedOptions().locale + ' ' + df.resolvedOptions().numberingSystem + ' ' + q(df.format(d));
  });
}
['en', 'de', 'da', 'id', 'si', 'en-DK', 'fi', 'af', 'lt', 'es-CL', 'sr', 'be'].forEach(function (l) { f('U13-U14', l, { style: 'digital' }, { hours: 1, minutes: 2, seconds: 3 }); });
f('U13', 'da', { style: 'long', minutes: 'numeric' }, { hours: 1, minutes: 2, seconds: 3 });
['fr', 'ja', 'es', 'ar', 'zh-TW'].forEach(function (l) { f('U15', l, { style: 'long' }, { hours: 1, minutes: 2 }); });
f('U15', 'de', { style: 'short' }, { nanoseconds: 1 });
f('U15', 'de', { style: 'short' }, { milliseconds: 1, microseconds: 1 });
f('U15', 'de', { style: 'narrow' }, { hours: 1, milliseconds: 1 });
f('U15', 'de', { style: 'narrow' }, { hours: 2, milliseconds: 2 });
f('U16', 'en', { style: 'short', numberingSystem: 'arab' }, { hours: -1, minutes: -2, seconds: -3 });
f('U16', 'en', { style: 'short', numberingSystem: 'arab' }, { days: 1000 });
f('U16', 'en', { style: 'digital', numberingSystem: 'arab' }, { hours: 1, minutes: 2, seconds: 3, milliseconds: 500 });
f('U16', 'de', { style: 'short', numberingSystem: 'arab' }, { years: -1, months: -2 });
f('U16', 'en-u-nu-arab', { style: 'short' }, { hours: 1 });
f('U16', 'en-u-nu-arab', { style: 'short', numberingSystem: 'latn' }, { hours: 1 });
f('U16', 'en-u-ca-gregory-nu-latn', {}, { hours: 1 });
f('U16', 'ar-EG', { style: 'short' }, { hours: 1, minutes: 2 });
f('U16', 'fa', { style: 'digital' }, { hours: 1, minutes: 2, seconds: 3 });
f('U16', 'en', { style: 'short', numberingSystem: 'abcd' }, { hours: 1 });
t('U15 supportedLocalesOf([fr, de-AT, da, en])', function () { return Intl.DurationFormat.supportedLocalesOf(['fr', 'de-AT', 'da', 'en']); });
// The number formats a duration builds, alone (sub-area c's rows): a unit of ar, and en's arab digits and symbols.
t('U15 NumberFormat ar unit hour long 1, 2', function () { var nf = new Intl.NumberFormat('ar', { style: 'unit', unit: 'hour', unitDisplay: 'long' }); return [nf.resolvedOptions().locale, nf.format(1), nf.format(2)]; });
t('U15 ListFormat ar unit long ["x", "y"]', function () { return new Intl.ListFormat('ar', { type: 'unit', style: 'long' }).format(['x', 'y']); });
t('U16 NumberFormat en arab -1234.5', function () { return new Intl.NumberFormat('en', { numberingSystem: 'arab' }).format(-1234.5); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
