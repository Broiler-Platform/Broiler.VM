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
// SJL-9.1 sub-area (d), rows T1-T4: CreateDateTimeFormat's option reads (order, count, coercion,
// refusals), the required/defaults pairs of Date.prototype.toLocale*, the bound format's identity,
// the legacy constructor path (ChainDateTimeFormat, UnwrapDateTimeFormat) and the receiver checks.
// ECMA-402 3d4c558 11.1.1-11.1.2 with tc39/proposal-temporal@e8cc03fc intl.html's CreateDateTimeFormat.
var A = Date.UTC(2024, 0, 5, 15, 7, 9, 123);
var all = { localeMatcher: 'best fit', calendar: 'gregory', numberingSystem: 'latn', hour12: undefined, hourCycle: 'h23',
  timeZone: 'UTC', weekday: 'short', era: 'short', year: 'numeric', month: 'short', day: 'numeric', dayPeriod: undefined,
  hour: 'numeric', minute: 'numeric', second: 'numeric', fractionalSecondDigits: 2, timeZoneName: 'short',
  formatMatcher: 'best fit', dateStyle: undefined, timeStyle: undefined };
function reads(fn) { var log = []; try { fn(log); } catch (e) { log.push('THROWS ' + errName(e)); } return log.join(','); }
t('constructor reads, component bag', function () { return R(reads(function (log) { new Intl.DateTimeFormat('en', spy(all, log)); })); });
t('constructor reads, style bag', function () { return R(reads(function (log) { new Intl.DateTimeFormat('en', spy({ dateStyle: 'long', timeStyle: 'short' }, log)); })); });
t('constructor reads, empty bag', function () { return R(reads(function (log) { new Intl.DateTimeFormat('en', spy({}, log)); })); });
t('reads during format and resolvedOptions after construction', function () {
  return R(reads(function (log) { var f = new Intl.DateTimeFormat('en', spy({ year: 'numeric', hour: 'numeric' }, log)); log.length = 0; f.format(A); f.formatToParts(A); f.resolvedOptions(); f.formatRange(A, A + 1); }));
});
t('Date#toLocaleString reads', function () { return R(reads(function (log) { new Date(A).toLocaleString('en', spy({ year: 'numeric', hour: 'numeric', timeZone: 'UTC' }, log)); })); });
t('Date#toLocaleDateString reads', function () { return R(reads(function (log) { new Date(A).toLocaleDateString('en', spy({ month: 'long', timeZone: 'UTC' }, log)); })); });
t('Date#toLocaleTimeString reads', function () { return R(reads(function (log) { new Date(A).toLocaleTimeString('en', spy({ minute: 'numeric', timeZone: 'UTC' }, log)); })); });
t('Date#toLocaleString reads, empty bag', function () { return R(reads(function (log) { new Date(A).toLocaleString('en', spy({}, log)); })); });
// Coercion and refusals.
t('weekday: "LONG"', function () { return new Intl.DateTimeFormat('en', { weekday: 'LONG' }).resolvedOptions().weekday; });
t('month: 3', function () { return new Intl.DateTimeFormat('en', { month: 3 }).resolvedOptions().month; });
t('fractionalSecondDigits: 0', function () { return new Intl.DateTimeFormat('en', { fractionalSecondDigits: 0 }).resolvedOptions().fractionalSecondDigits; });
t('fractionalSecondDigits: 4', function () { return new Intl.DateTimeFormat('en', { fractionalSecondDigits: 4 }).resolvedOptions().fractionalSecondDigits; });
t('fractionalSecondDigits: "2.9"', function () { return new Intl.DateTimeFormat('en', { fractionalSecondDigits: '2.9', timeZone: 'UTC' }).format(A); });
t('hour12: "false" (a string is true)', function () { return new Intl.DateTimeFormat('en', { hour: 'numeric', hour12: 'false', timeZone: 'UTC' }).resolvedOptions().hourCycle; });
t('calendar: "gregory-x" (malformed)', function () { return new Intl.DateTimeFormat('en', { calendar: 'gregory-' }).resolvedOptions().calendar; });
t('calendar: "abc" (well formed, unknown)', function () { return new Intl.DateTimeFormat('en', { calendar: 'abc' }).resolvedOptions().calendar; });
t('numberingSystem: "latn!"', function () { return new Intl.DateTimeFormat('en', { numberingSystem: 'latn!' }).resolvedOptions().numberingSystem; });
t('formatMatcher: "basic"', function () { return new Intl.DateTimeFormat('en', { formatMatcher: 'basic', month: 'long', day: 'numeric', timeZone: 'UTC' }).format(A); });
t('formatMatcher: "x"', function () { return new Intl.DateTimeFormat('en', { formatMatcher: 'x' }).format(A); });
t('dateStyle with month', function () { return new Intl.DateTimeFormat('en', { dateStyle: 'short', month: 'long' }).format(A); });
t('timeStyle with timeZoneName', function () { return new Intl.DateTimeFormat('en', { timeStyle: 'short', timeZoneName: 'short' }).format(A); });
t('dateStyle with era', function () { return new Intl.DateTimeFormat('en', { dateStyle: 'short', era: 'short' }).format(A); });
t('dateStyle: "LONG"', function () { return new Intl.DateTimeFormat('en', { dateStyle: 'LONG' }).format(A); });
t('options null', function () { return new Intl.DateTimeFormat('en', null).format(A); });
t('options "abc" (a string)', function () { return new Intl.DateTimeFormat('en', 'abc').resolvedOptions().year; });
t('Date#toLocaleDateString with timeStyle', function () { return new Date(A).toLocaleDateString('en', { timeStyle: 'short', timeZone: 'UTC' }); });
t('Date#toLocaleTimeString with dateStyle', function () { return new Date(A).toLocaleTimeString('en', { dateStyle: 'short', timeZone: 'UTC' }); });
t('Date#toLocaleString with dateStyle and timeStyle', function () { return new Date(A).toLocaleString('en', { dateStyle: 'medium', timeStyle: 'short', timeZone: 'UTC' }); });
t('Date#toLocaleDateString with dateStyle', function () { return new Date(A).toLocaleDateString('en', { dateStyle: 'full', timeZone: 'UTC' }); });
t('Date#toLocaleTimeString with timeStyle', function () { return new Date(A).toLocaleTimeString('en', { timeStyle: 'full', timeZone: 'UTC' }); });
t('Date#toLocaleDateString with hour only', function () { return new Date(A).toLocaleDateString('en', { hour: 'numeric', timeZone: 'UTC' }); });
t('Date#toLocaleTimeString with year only', function () { return new Date(A).toLocaleTimeString('en', { year: 'numeric', timeZone: 'UTC' }); });
t('Date#toLocaleString of an invalid date', function () { return new Date(NaN).toLocaleString('en'); });
t('Date#toLocaleString with a string second argument "yyyy"', function () { return new Date(A).toLocaleString('en', 'yyyy'); });
t('Date#toLocaleDateString with a string second argument "yyyy-MM-dd"', function () { return new Date(A).toLocaleDateString(undefined, 'yyyy-MM-dd'); });
// The bound format, receivers and the legacy path.
t('format getter returns one function', function () { var f = new Intl.DateTimeFormat('en'); return f.format === f.format; });
t('bound format name and length', function () { var g = new Intl.DateTimeFormat('en').format; return [g.name, g.length, Object.getPrototypeOf(g) === Function.prototype]; });
t('bound format detached', function () { var g = new Intl.DateTimeFormat('en', { timeZone: 'UTC' }).format; return [A, 0].map(g); });
t('format getter on a plain object', function () { return Object.getOwnPropertyDescriptor(Intl.DateTimeFormat.prototype, 'format').get.call({}); });
t('formatToParts on a NumberFormat', function () { return Intl.DateTimeFormat.prototype.formatToParts.call(new Intl.NumberFormat(), 0); });
t('resolvedOptions on the prototype', function () { return Intl.DateTimeFormat.prototype.resolvedOptions(); });
t('called as a function', function () { return Object.prototype.toString.call(Intl.DateTimeFormat('en')); });
t('legacy: called on an object inheriting the prototype', function () {
  var o = Object.create(Intl.DateTimeFormat.prototype); var r = Intl.DateTimeFormat.call(o, 'de', { timeZone: 'UTC' });
  return [r === o, Object.getOwnPropertySymbols(o).length, typeof o.format === 'function' ? o.format(A) : 'no format', o.resolvedOptions().locale];
});
t('legacy: formatToParts on that object', function () { var o = Object.create(Intl.DateTimeFormat.prototype); Intl.DateTimeFormat.call(o, 'en'); return Intl.DateTimeFormat.prototype.formatToParts.call(o, A).length; });
t('new with a subclass', function () { class D extends Intl.DateTimeFormat {} var d = new D('en', { timeZone: 'UTC' }); return [d instanceof D, d.format(A)]; });
t('toStringTag', function () { return Object.prototype.toString.call(new Intl.DateTimeFormat()); });
t('constructor length, prototype methods', function () { var p = Intl.DateTimeFormat.prototype; return [Intl.DateTimeFormat.length, p.formatToParts.length, p.formatRange.length, p.formatRangeToParts.length, p.resolvedOptions.length]; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
