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
// Rows U1-U4: GetDurationUnitOptions (ECMA-402 3d4c558 sec-getdurationunitoptions) and ValidateDurationUnitStyle
// (sec-validatedurationunitstyle): each unit's resolved style and display, and the RangeErrors.
// ro() writes resolvedOptions as "unit=style/display" for the ten units, after locale, numberingSystem and style.
var U = ['years', 'months', 'weeks', 'days', 'hours', 'minutes', 'seconds', 'milliseconds', 'microseconds', 'nanoseconds'];
function ro(df) {
  var o = df.resolvedOptions();
  var s = o.locale + ' ' + o.numberingSystem + ' ' + o.style;
  for (var i = 0; i < U.length; i++) s += ' ' + U[i] + '=' + o[U[i]] + '/' + o[U[i] + 'Display'];
  if (o.fractionalDigits !== undefined) s += ' fractionalDigits=' + o.fractionalDigits;
  return R(s);
}
var bags = [
  {}, { style: 'digital' },
  { style: 'long', minutes: 'numeric' },
  { style: 'long', hours: 'numeric' },
  { style: 'long', hours: '2-digit' },
  { hours: 'numeric' },
  { minutes: 'numeric' },
  { style: 'long', seconds: 'numeric' },
  { style: 'short', milliseconds: 'numeric' },
  { style: 'narrow', microseconds: 'numeric', fractionalDigits: 0 },
  { style: 'long', seconds: 'numeric', fractionalDigits: 3 },
  { hours: 'numeric', minutesDisplay: 'auto' },
  { style: 'digital', hoursDisplay: 'auto' },
  { style: 'digital', hours: 'long' },
  { style: 'digital', hours: '2-digit' },
  { style: 'digital', fractionalDigits: 2.9 },
  { fractionalDigits: '3' },
  { seconds: 'numeric', secondsDisplay: 'auto', milliseconds: 'numeric', millisecondsDisplay: 'auto' }
];
bags.forEach(function (b) {
  t('U1-U4 en ' + JSON.stringify(b), function () { return ro(new Intl.DurationFormat('en', b)); });
});
var bad = [
  { milliseconds: 'numeric', millisecondsDisplay: 'always' },
  { style: 'digital', millisecondsDisplay: 'always' },
  { hours: 'numeric', nanosecondsDisplay: 'always' },
  { seconds: 'numeric', microseconds: 'numeric', microsecondsDisplay: 'always' },
  { style: 'long', milliseconds: 'numeric', microsecondsDisplay: 'always' },
  { hours: 'numeric', minutes: 'long' },
  { milliseconds: 'numeric', microseconds: 'long' },
  { seconds: '2-digit', milliseconds: 'short' },
  { style: 'full' },
  { hours: 'digital' },
  { fractionalDigits: 10 },
  { fractionalDigits: -1 },
  { fractionalDigits: NaN },
  { numberingSystem: 'latn-' },
  { hoursDisplay: 'never' }
];
bad.forEach(function (b) {
  t('U3-U4 en ' + JSON.stringify(b, function (k, v) { return typeof v === 'number' && v !== v ? 'NaN' : v; }), function () { return ro(new Intl.DurationFormat('en', b)); });
});
t('U4 resolvedOptions keys, {fractionalDigits: 1}', function () { return Object.keys(new Intl.DurationFormat('en', { fractionalDigits: 1 }).resolvedOptions()).join(','); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
