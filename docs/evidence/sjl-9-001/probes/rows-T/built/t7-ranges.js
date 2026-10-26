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
// SJL-9.1 sub-area (d), rows T21-T23: formatRange and formatRangeToParts. The range patterns (CLDR's
// intervalFormats through the interval formatter), the fields compared, the collapsed range, the
// sources, the argument checks. ECMA-402 3d4c558 11.3.6-11.3.7 and PartitionDateTimeRangePattern as
// e8cc03fc intl.html amends it (Table 16's fields compared in order; a collapsed result is "shared").
function d(y, mo, da, h, mi) { return Date.UTC(y, mo, da, h || 0, mi || 0); }
var pairs = [
  ['same instant', d(2024, 0, 5, 10), d(2024, 0, 5, 10)],
  ['10:00-10:30', d(2024, 0, 5, 10), d(2024, 0, 5, 10, 30)],
  ['10:00-11:00', d(2024, 0, 5, 10), d(2024, 0, 5, 11)],
  ['10:00-15:00', d(2024, 0, 5, 10), d(2024, 0, 5, 15)],
  ['Jan 5-Jan 7', d(2024, 0, 5, 10), d(2024, 0, 7, 10)],
  ['Jan 5-Feb 7', d(2024, 0, 5, 10), d(2024, 1, 7, 10)],
  ['2024-2025', d(2024, 0, 5, 10), d(2025, 0, 5, 10)],
  ['BC-AD', -62198755200000 - 86400000 * 400, d(2024, 0, 5)],
  ['reversed (Jan 7-Jan 5)', d(2024, 0, 7, 10), d(2024, 0, 5, 10)]
];
var opts = [
  {}, { dateStyle: 'medium' }, { dateStyle: 'long' }, { dateStyle: 'short', timeStyle: 'short' }, { timeStyle: 'short' },
  { year: 'numeric', month: 'short', day: 'numeric' }, { month: 'long', day: 'numeric' }, { year: 'numeric', month: 'long' },
  { weekday: 'short' }, { weekday: 'long', month: 'short', day: 'numeric' }, { hour: 'numeric' }, { hour: 'numeric', minute: 'numeric' },
  { hour: 'numeric', minute: 'numeric', hourCycle: 'h23' }, { year: 'numeric', month: 'short', day: 'numeric', hour: 'numeric', minute: 'numeric' }, { era: 'short', year: 'numeric' }
];
['en', 'de'].forEach(function (l) {
  opts.forEach(function (o) {
    var p = { timeZone: 'UTC' }; for (var k in o) p[k] = o[k];
    t(l + ' formatRange ' + show(o), function () { var f = new Intl.DateTimeFormat(l, p); return pairs.map(function (q) { return f.formatRange(q[1], q[2]); }); });
  });
});
t('en formatRangeToParts {year, month:short, day} Jan 5-Jan 7', function () { return parts(new Intl.DateTimeFormat('en', { year: 'numeric', month: 'short', day: 'numeric', timeZone: 'UTC' }).formatRangeToParts(d(2024, 0, 5), d(2024, 0, 7))); });
t('en formatRangeToParts default, same day', function () { return parts(new Intl.DateTimeFormat('en', { timeZone: 'UTC' }).formatRangeToParts(d(2024, 0, 5, 1), d(2024, 0, 5, 9))); });
t('en formatRangeToParts {hour, minute} 10:00-15:00', function () { return parts(new Intl.DateTimeFormat('en', { hour: 'numeric', minute: 'numeric', timeZone: 'UTC' }).formatRangeToParts(d(2024, 0, 5, 10), d(2024, 0, 5, 15))); });
t('en formatRangeToParts {weekday:short} Tue-Wed', function () { return parts(new Intl.DateTimeFormat('en', { weekday: 'short', timeZone: 'UTC' }).formatRangeToParts(d(2024, 0, 2), d(2024, 0, 3))); });
t('en formatRangeToParts dateStyle long timeStyle short, two days', function () { return parts(new Intl.DateTimeFormat('en', { dateStyle: 'long', timeStyle: 'short', timeZone: 'UTC' }).formatRangeToParts(d(2024, 0, 5, 10), d(2024, 0, 6, 11))); });
t('de formatRangeToParts {year, month:long, day} Jan 5-Feb 7', function () { return parts(new Intl.DateTimeFormat('de', { year: 'numeric', month: 'long', day: 'numeric', timeZone: 'UTC' }).formatRangeToParts(d(2024, 0, 5), d(2024, 1, 7))); });
t('formatRange(undefined, 0)', function () { return new Intl.DateTimeFormat('en').formatRange(undefined, 0); });
t('formatRange(0)', function () { return new Intl.DateTimeFormat('en').formatRange(0); });
t('formatRange(0, NaN)', function () { return new Intl.DateTimeFormat('en').formatRange(0, NaN); });
t('formatRange(NaN, undefined): which error first', function () { return new Intl.DateTimeFormat('en').formatRange(NaN, undefined); });
t('formatRange coercion order', function () { var log = []; try { new Intl.DateTimeFormat('en').formatRange({ valueOf: function () { log.push('x'); return 0; } }, { valueOf: function () { log.push('y'); return NaN; } }); } catch (e) { log.push(errName(e)); } return R(log.join(',')); });
t('formatRange in America/New_York across the DST change', function () { return new Intl.DateTimeFormat('en', { hour: 'numeric', minute: 'numeric', timeZoneName: 'short', timeZone: 'America/New_York' }).formatRange(Date.UTC(2024, 2, 10, 6), Date.UTC(2024, 2, 10, 8)); });
t('formatRange dayPeriod 9:00-15:00', function () { return new Intl.DateTimeFormat('en', { hour: 'numeric', dayPeriod: 'short', timeZone: 'UTC' }).formatRange(d(2024, 0, 5, 9), d(2024, 0, 5, 15)); });
t('formatRange fractionalSecondDigits', function () { return new Intl.DateTimeFormat('en', { minute: 'numeric', second: 'numeric', fractionalSecondDigits: 2, timeZone: 'UTC' }).formatRange(d(2024, 0, 5, 9) + 100, d(2024, 0, 5, 9) + 200); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
