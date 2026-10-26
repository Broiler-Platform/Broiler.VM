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
// Row U18: the constructor and prototype as objects: NewTarget, branding, the tag, the option reads, and the
// options argument (GetOptionsObject).
class Sub extends Intl.DurationFormat {}
t('U18 new Sub() prototype is Sub.prototype', function () { var s = new Sub('en'); return [Object.getPrototypeOf(s) === Sub.prototype, s instanceof Sub, s.format({ hours: 1 })]; });
t('U18 Reflect.construct(Intl.DurationFormat, [], Array) prototype', function () { var o = Reflect.construct(Intl.DurationFormat, ['en'], Array); return Object.getPrototypeOf(o) === Array.prototype; });
t('U18 Intl.DurationFormat()', function () { return Intl.DurationFormat(); });
t('U18 format.call({})', function () { return Intl.DurationFormat.prototype.format.call({}, { hours: 1 }); });
t('U18 resolvedOptions.call(Intl.DurationFormat.prototype)', function () { return Intl.DurationFormat.prototype.resolvedOptions.call(Intl.DurationFormat.prototype); });
t('U18 Object.prototype.toString', function () { return Object.prototype.toString.call(new Intl.DurationFormat()); });
t('U18 lengths', function () { return [Intl.DurationFormat.length, Intl.DurationFormat.prototype.format.length, Intl.DurationFormat.prototype.formatToParts.length, Intl.DurationFormat.prototype.resolvedOptions.length, Intl.DurationFormat.supportedLocalesOf.length]; });
t('U18 new Intl.DurationFormat("en", null)', function () { return new Intl.DurationFormat('en', null); });
t('U18 new Intl.DurationFormat("en", "long")', function () { return new Intl.DurationFormat('en', 'long'); });
t('U18 new Intl.DurationFormat("en", undefined).resolvedOptions().style', function () { return new Intl.DurationFormat('en', undefined).resolvedOptions().style; });
t('U18 style read through ToString', function () { return new Intl.DurationFormat('en', { style: { toString: function () { return 'long'; } } }).resolvedOptions().style; });
t('U18 option reads, every option given', function () {
  var log = [];
  new Intl.DurationFormat('en', spy({ localeMatcher: 'lookup', numberingSystem: 'latn', style: 'long', years: 'long', yearsDisplay: 'auto', months: 'long', monthsDisplay: 'auto', weeks: 'long', weeksDisplay: 'auto', days: 'long', daysDisplay: 'auto', hours: 'numeric', hoursDisplay: 'auto', minutes: 'numeric', minutesDisplay: 'auto', seconds: 'numeric', secondsDisplay: 'auto', milliseconds: 'numeric', millisecondsDisplay: 'auto', microseconds: 'numeric', microsecondsDisplay: 'auto', nanoseconds: 'numeric', nanosecondsDisplay: 'auto', fractionalDigits: 3 }, log));
  return log.join(',');
});
t('U18 option reads, an empty bag', function () { var log = []; new Intl.DurationFormat('en', spy({}, log)); return log.join(','); });
t('U18 format is the prototype method on every instance', function () { return new Intl.DurationFormat('en').format === new Intl.DurationFormat('de').format; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
