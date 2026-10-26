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
// SJL-9.1 sub-area (d), rows T24-T26: formatting in calendars other than the Gregorian one (the
// calendar's era, year, month name and numbering; the deprecated Hijri types; the resolved calendar),
// with the leftover item of bundle sjl-10-001 (the 24 Intl calendar identifiers, at the formatter's
// sites). ECMA-402 3d4c558 11.1.2 ([[ca]]), ToLocalTime ("best available information about the
// specified calendar"); the Intl era and month code proposal's CreateDateTimeFormat fallback.
var A = Date.UTC(2024, 0, 5, 15, 7, 9, 123), M = Date.UTC(2024, 4, 20, 12);
var cals = ['buddhist', 'chinese', 'coptic', 'dangi', 'ethioaa', 'ethiopic', 'gregory', 'hebrew', 'indian', 'islamic', 'islamic-civil', 'islamic-rgsa',
  'islamic-tbla', 'islamic-umalqura', 'iso8601', 'japanese', 'persian', 'roc', 'islamicc', 'ethiopic-amete-alem', 'gregorian', 'abc'];
cals.forEach(function (c) {
  t('en-u-ca-' + c + ' resolved [locale, calendar]', function () { var r = new Intl.DateTimeFormat('en-u-ca-' + c).resolvedOptions(); return [r.locale, r.calendar]; });
  t('en {calendar:' + c + '} resolved [locale, calendar]', function () { var r = new Intl.DateTimeFormat('en', { calendar: c }).resolvedOptions(); return [r.locale, r.calendar]; });
  t('en-u-ca-' + c + ' default, dateStyle long, dateStyle full, {era,year,month:long,day}', function () {
    function g(o) { var p = { timeZone: 'UTC' }; for (var k in o) p[k] = o[k]; return new Intl.DateTimeFormat('en-u-ca-' + c, p).format(A); }
    return [g({}), g({ dateStyle: 'long' }), g({ dateStyle: 'full' }), g({ era: 'short', year: 'numeric', month: 'long', day: 'numeric' })];
  });
});
['buddhist', 'chinese', 'hebrew', 'islamic-umalqura', 'japanese', 'persian', 'roc'].forEach(function (c) {
  t('de-u-ca-' + c + ' dateStyle long', function () { return new Intl.DateTimeFormat('de-u-ca-' + c, { dateStyle: 'long', timeZone: 'UTC' }).format(A); });
});
t('japanese eras: 1800, 1868-10-23, 1873, 1912-07-30, 1926-12-25, 1989-01-08, 2019-05-01', function () {
  var f = new Intl.DateTimeFormat('en-u-ca-japanese', { era: 'long', year: 'numeric', month: 'short', day: 'numeric', timeZone: 'UTC' });
  return [Date.UTC(1800, 0, 1), Date.UTC(1868, 9, 23), Date.UTC(1873, 0, 1), Date.UTC(1912, 6, 30), Date.UTC(1926, 11, 25), Date.UTC(1989, 0, 8), Date.UTC(2019, 4, 1)].map(function (x) { return f.format(x); });
});
t('chinese leap month (2023-04-20, leap 2nd month) and the next month', function () { var f = new Intl.DateTimeFormat('en-u-ca-chinese', { year: 'numeric', month: 'long', day: 'numeric', timeZone: 'UTC' }); return [f.format(Date.UTC(2023, 3, 20)), f.format(Date.UTC(2023, 4, 20))]; });
t('chinese {month:numeric} leap', function () { return new Intl.DateTimeFormat('en-u-ca-chinese', { month: 'numeric', day: 'numeric', timeZone: 'UTC' }).format(Date.UTC(2023, 3, 20)); });
t('hebrew Adar I and Adar II (5784)', function () { var f = new Intl.DateTimeFormat('en-u-ca-hebrew', { year: 'numeric', month: 'long', day: 'numeric', timeZone: 'UTC' }); return [f.format(Date.UTC(2024, 1, 15)), f.format(Date.UTC(2024, 2, 15))]; });
t('islamic-umalqura months', function () { var f = new Intl.DateTimeFormat('en-u-ca-islamic-umalqura', { month: 'long', timeZone: 'UTC' }); var r = []; for (var m = 0; m < 12; m++) r.push(f.format(Date.UTC(2024, m, 15))); return r; });
t('dangi formatToParts', function () { return parts(new Intl.DateTimeFormat('en-u-ca-dangi', { year: 'numeric', month: 'long', day: 'numeric', timeZone: 'UTC' }).formatToParts(A)); });
t('roc before 1912', function () { return new Intl.DateTimeFormat('en-u-ca-roc', { era: 'short', year: 'numeric', timeZone: 'UTC' }).format(Date.UTC(1900, 0, 1)); });
t('buddhist formatToParts', function () { return parts(new Intl.DateTimeFormat('en-u-ca-buddhist', { year: 'numeric', month: 'short', timeZone: 'UTC' }).formatToParts(A)); });
t('iso8601 dateStyle full and default', function () { return [new Intl.DateTimeFormat('en-u-ca-iso8601', { dateStyle: 'full', timeZone: 'UTC' }).format(A), new Intl.DateTimeFormat('en-u-ca-iso8601', { timeZone: 'UTC' }).format(A)]; });
t('persian at year 1 and negative', function () { var f = new Intl.DateTimeFormat('en-u-ca-persian', { era: 'short', year: 'numeric', month: 'short', day: 'numeric', timeZone: 'UTC' }); return [f.format(Date.UTC(622, 2, 22)), f.format(Date.UTC(600, 0, 1))]; });
t('coptic and ethiopic eras before their epochs', function () { return ['coptic', 'ethiopic', 'ethioaa'].map(function (c) { return new Intl.DateTimeFormat('en-u-ca-' + c, { era: 'short', year: 'numeric', timeZone: 'UTC' }).format(Date.UTC(100, 0, 1)); }); });
t('Date#toLocaleDateString en-u-ca-japanese', function () { return new Date(M).toLocaleDateString('en-u-ca-japanese', { timeZone: 'UTC' }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
