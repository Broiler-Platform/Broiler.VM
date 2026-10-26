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
// SJL-9.1 sub-area (d), rows T19-T20: formatToParts' part types and the values at the time value's
// limits, TimeClip, and the leftover items of bundles sjl-6-001 (formatToParts(8.64e15); a minute and
// second without an hour in a zone off the hour). ECMA-402 3d4c558 11.3.4 formatToParts,
// FormatDateTimeToParts and HandleDateTimeOthers (e8cc03fc), Table 7.
var A = Date.UTC(2024, 0, 5, 15, 7, 9, 123);
function fp(l, o, x) { var p = { timeZone: 'UTC' }; for (var k in o) p[k] = o[k]; return parts(new Intl.DateTimeFormat(l, p).formatToParts(x)); }
t('en full parts', function () { return fp('en', { weekday: 'long', era: 'short', year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: '2-digit', second: '2-digit', fractionalSecondDigits: 3, timeZoneName: 'short' }, A); });
t('de full parts', function () { return fp('de', { weekday: 'long', era: 'short', year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: '2-digit', second: '2-digit', fractionalSecondDigits: 3, timeZoneName: 'short' }, A); });
t('en default parts', function () { return fp('en', {}, A); });
t('en dateStyle full timeStyle full parts', function () { return fp('en', { dateStyle: 'full', timeStyle: 'full' }, A); });
t('en relatedYear and yearName (chinese)', function () { return fp('en-u-ca-chinese', { year: 'numeric', month: 'long' }, A); });
t('en {year:2-digit} for 2024, 1999, 5, -1', function () { return [Date.UTC(2024, 0, 1), Date.UTC(1999, 0, 1), Date.UTC(5, 0, 1) - 0, Date.UTC(-1, 0, 1)].map(function (x) { var d = new Date(x); if (x === Date.UTC(5, 0, 1)) { d.setUTCFullYear(5); } return new Intl.DateTimeFormat('en', { year: '2-digit', timeZone: 'UTC' }).format(d); }); });
t('en {year:numeric, era:short} for year 0 and -1 (1 BC, 2 BC)', function () { return [0, -1].map(function (y) { var d = new Date(0); d.setUTCFullYear(y); return new Intl.DateTimeFormat('en', { year: 'numeric', era: 'short', timeZone: 'UTC' }).format(d); }); });
t('en {year:numeric} for year 0 and -1', function () { return [0, -1].map(function (y) { var d = new Date(0); d.setUTCFullYear(y); return new Intl.DateTimeFormat('en', { year: 'numeric', timeZone: 'UTC' }).format(d); }); });
t('8.64e15 parts', function () { return fp('en', { year: 'numeric', month: 'short', day: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric' }, 8.64e15); });
t('-8.64e15 parts', function () { return fp('en', { year: 'numeric', month: 'short', day: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric', era: 'short' }, -8.64e15); });
t('8.64e15 {minute, second}', function () { return fp('en', { minute: 'numeric', second: 'numeric' }, 8.64e15); });
t('8.64e15 + 1', function () { return fp('en', {}, 8.64e15 + 1); });
t('NaN', function () { return fp('en', {}, NaN); });
t('a string "0"', function () { return fp('en', {}, '0'); });
t('a Date object', function () { return fp('en', {}, new Date(A)); });
t('1.9 (TimeClip truncates)', function () { return fp('en', { second: 'numeric', fractionalSecondDigits: 3 }, 1.9); });
t('-0.5', function () { return fp('en', { second: 'numeric', fractionalSecondDigits: 3 }, -0.5); });
t('a BigInt', function () { return fp('en', {}, 1n); });
t('a Symbol', function () { return fp('en', {}, Symbol()); });
t('{minute, second} in Asia/Kolkata at 12:00:05Z (sjl-6-001 leftover)', function () { return fp('en', { minute: '2-digit', second: '2-digit', timeZone: 'Asia/Kolkata' }, Date.UTC(2026, 9, 16, 12, 0, 5)); });
t('{minute, second} in Asia/Kathmandu', function () { var o = { minute: '2-digit', second: '2-digit', timeZone: 'Asia/Kathmandu' }; return [new Intl.DateTimeFormat('en', o).format(Date.UTC(2026, 9, 16, 12, 0, 5))]; });
t('{minute} in Asia/Kolkata', function () { return new Intl.DateTimeFormat('en', { minute: 'numeric', timeZone: 'Asia/Kolkata' }).format(Date.UTC(2026, 9, 16, 12, 0, 5)); });
t('{hour, minute} in Asia/Kolkata at the epoch', function () { return new Intl.DateTimeFormat('en', { hour: 'numeric', minute: 'numeric', timeZone: 'Asia/Kolkata' }).format(0); });
t('Date#toLocaleTimeString({minute, second}) in Asia/Kolkata', function () { return new Date(Date.UTC(2026, 9, 16, 12, 0, 5)).toLocaleTimeString('en', { minute: '2-digit', second: '2-digit', timeZone: 'Asia/Kolkata' }); });
t('format of an Object with valueOf', function () { return new Intl.DateTimeFormat('en', { timeZone: 'UTC' }).format({ valueOf: function () { return A; } }); });
t('formatToParts property order and prototype', function () { var p = new Intl.DateTimeFormat('en').formatToParts(A)[0]; return [Object.keys(p).join(','), Object.getPrototypeOf(p) === Object.prototype]; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
