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
// SJL-9.1 sub-area (d), rows T5-T7: resolvedOptions (its properties, their order and the values the
// selected format gives), the hour cycle (hour12, hourCycle, -u-hc-, the locale's default) and the
// default time zone. ECMA-402 3d4c558 11.3.5 with proposal-temporal@e8cc03fc's CreateDateTimeFormat.
function ro(l, o) { return new Intl.DateTimeFormat(l, o).resolvedOptions(); }
['en', 'en-US', 'de', 'de-DE', 'de-AT', 'en-GB', 'fr', 'ja', 'ko', 'zh-TW', 'ar-EG', undefined].forEach(function (l) {
  t('resolvedOptions ' + show(l) + ' {}', function () { return ro(l, { timeZone: 'UTC' }); });
});
t('en {hour:numeric, minute:numeric}', function () { return ro('en', { hour: 'numeric', minute: 'numeric', timeZone: 'UTC' }); });
t('en {minute:numeric}', function () { return ro('en', { minute: 'numeric', timeZone: 'UTC' }); });
t('en {hour:2-digit}', function () { return ro('en', { hour: '2-digit', timeZone: 'UTC' }); });
t('en {year:2-digit, month:long}', function () { return ro('en', { year: '2-digit', month: 'long', timeZone: 'UTC' }); });
t('en {month:short, day:2-digit}', function () { return ro('en', { month: 'short', day: '2-digit', timeZone: 'UTC' }); });
t('en {weekday:narrow, era:long, second:numeric, fractionalSecondDigits:1, timeZoneName:longGeneric}', function () {
  return ro('en', { weekday: 'narrow', era: 'long', second: 'numeric', fractionalSecondDigits: 1, timeZoneName: 'longGeneric', timeZone: 'UTC' });
});
t('en {dayPeriod:short}', function () { return ro('en', { dayPeriod: 'short', timeZone: 'UTC' }); });
t('en {dayPeriod:long, hour:numeric}', function () { return ro('en', { dayPeriod: 'long', hour: 'numeric', timeZone: 'UTC' }); });
t('en {dateStyle:long}', function () { return ro('en', { dateStyle: 'long', timeZone: 'UTC' }); });
t('en {timeStyle:short}', function () { return ro('en', { timeStyle: 'short', timeZone: 'UTC' }); });
t('en {dateStyle:full, timeStyle:full}', function () { return ro('en', { dateStyle: 'full', timeStyle: 'full', timeZone: 'UTC' }); });
t('de {month:numeric, day:numeric}', function () { return ro('de', { month: 'numeric', day: 'numeric', timeZone: 'UTC' }); });
t('de {year:numeric, month:short}', function () { return ro('de', { year: 'numeric', month: 'short', timeZone: 'UTC' }); });
// The hour cycle.
var cycles = [['en', {}], ['en', { hour12: true }], ['en', { hour12: false }], ['en', { hourCycle: 'h11' }], ['en', { hourCycle: 'h24' }],
  ['en', { hourCycle: 'h23', hour12: true }], ['en-u-hc-h23', {}], ['en-u-hc-h23', { hourCycle: 'h11' }], ['en-u-hc-h23', { hour12: true }],
  ['en-u-hc-h24', { hourCycle: 'h24' }], ['de', {}], ['de', { hour12: true }], ['de', { hour12: false }], ['de-u-hc-h12', {}],
  ['en-GB', {}], ['en-GB', { hour12: true }], ['ja', {}], ['ja', { hour12: true }], ['ja-JP', { hour12: true }], ['ko', {}], ['en-IN', {}], ['zh-TW', {}]];
cycles.forEach(function (c) {
  t('hour cycle ' + c[0] + ' ' + show(c[1]), function () {
    var o = { hour: 'numeric', minute: '2-digit', timeZone: 'UTC' }; for (var k in c[1]) o[k] = c[1][k];
    var f = new Intl.DateTimeFormat(c[0], o), r = f.resolvedOptions();
    return [r.locale, r.hourCycle, r.hour12, f.format(Date.UTC(2024, 0, 5, 0, 5)), f.format(Date.UTC(2024, 0, 5, 12, 5))];
  });
});
['h11', 'h12', 'h23', 'h24'].forEach(function (hc) {
  t('timeStyle short hourCycle ' + hc, function () { var f = new Intl.DateTimeFormat('en', { timeStyle: 'short', hourCycle: hc, timeZone: 'UTC' }); return [f.resolvedOptions().hourCycle, f.format(Date.UTC(2024, 0, 5, 0, 5)), f.format(Date.UTC(2024, 0, 5, 12, 5))]; });
  t('de timeStyle medium hourCycle ' + hc, function () { var f = new Intl.DateTimeFormat('de', { timeStyle: 'medium', hourCycle: hc, timeZone: 'UTC' }); return [f.resolvedOptions().hourCycle, f.format(Date.UTC(2024, 0, 5, 0, 5)), f.format(Date.UTC(2024, 0, 5, 12, 5))]; });
});
t('hourCycle without an hour field (year only)', function () { var r = ro('en', { year: 'numeric', hourCycle: 'h11', timeZone: 'UTC' }); return [r.hourCycle, r.hour12]; });
t('hourCycle with dateStyle only', function () { var r = ro('en', { dateStyle: 'short', hour12: true, timeZone: 'UTC' }); return [r.hourCycle, r.hour12]; });
// The default time zone (the realm's; the host's in an engine that takes it).
t('default timeZone', function () { return new Intl.DateTimeFormat().resolvedOptions().timeZone; });
t('default timeZone of Date#toLocaleString, as text', function () { return new Date(Date.UTC(2024, 6, 15, 9, 30)).toLocaleString('en'); });
t('default locale', function () { return new Intl.DateTimeFormat().resolvedOptions().locale; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
