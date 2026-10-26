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
// SJL-9.1 sub-area (d), rows T8-T12: the formats a locale's data and the pattern generator give, for
// en and de (Broiler.VM's data locales) and for locales outside them: the default, every date and
// time style and their joins, and component sets through the format matcher; the names (months,
// weekdays, eras) the formats write. ECMA-402 3d4c558 11.1.2, 11.5.1-11.5.4 (DateTimeStyleFormat,
// BasicFormatMatcher, BestFitFormatMatcher), FormatDateTimePattern as e8cc03fc amends it.
var A = Date.UTC(2024, 0, 5, 15, 7, 9, 123);
function f(l, o) { var p = { timeZone: 'UTC' }; for (var k in o) p[k] = o[k]; return new Intl.DateTimeFormat(l, p).format(A); }
var styles = ['full', 'long', 'medium', 'short'];
['en', 'de'].forEach(function (l) {
  t(l + ' default', function () { return f(l, {}); });
  styles.forEach(function (d) { t(l + ' dateStyle ' + d, function () { return f(l, { dateStyle: d }); }); });
  styles.forEach(function (s) { t(l + ' timeStyle ' + s, function () { return f(l, { timeStyle: s }); }); });
  styles.forEach(function (d) { styles.forEach(function (s) { t(l + ' dateStyle ' + d + ' timeStyle ' + s, function () { return f(l, { dateStyle: d, timeStyle: s }); }); }); });
});
var sets = [
  { year: 'numeric' }, { year: '2-digit' }, { month: 'numeric' }, { month: '2-digit' }, { month: 'narrow' }, { month: 'short' }, { month: 'long' },
  { day: 'numeric' }, { day: '2-digit' }, { weekday: 'narrow' }, { weekday: 'short' }, { weekday: 'long' }, { era: 'short' }, { era: 'long' }, { era: 'narrow' },
  { year: 'numeric', month: 'short' }, { year: 'numeric', month: 'long' }, { year: 'numeric', month: 'numeric' }, { year: '2-digit', month: '2-digit' },
  { month: 'short', day: 'numeric' }, { month: 'long', day: 'numeric' }, { month: 'numeric', day: 'numeric' }, { month: '2-digit', day: '2-digit' },
  { year: 'numeric', month: 'short', day: 'numeric' }, { year: 'numeric', month: 'long', day: 'numeric' }, { year: 'numeric', month: '2-digit', day: '2-digit' },
  { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }, { weekday: 'short', month: 'short', day: 'numeric' }, { weekday: 'long', day: 'numeric' },
  { era: 'short', year: 'numeric' }, { era: 'long', year: 'numeric', month: 'long', day: 'numeric' }, { era: 'short', month: 'short', day: 'numeric' },
  { hour: 'numeric' }, { hour: '2-digit' }, { minute: 'numeric' }, { minute: '2-digit' }, { second: 'numeric' }, { hour: 'numeric', minute: 'numeric' },
  { hour: '2-digit', minute: '2-digit' }, { hour: 'numeric', minute: 'numeric', second: 'numeric' }, { minute: 'numeric', second: 'numeric' },
  { hour: 'numeric', minute: '2-digit', second: '2-digit', fractionalSecondDigits: 3 }, { second: 'numeric', fractionalSecondDigits: 2 }, { fractionalSecondDigits: 1 },
  { hour: 'numeric', timeZoneName: 'short' }, { hour: 'numeric', minute: 'numeric', timeZoneName: 'long' }, { timeZoneName: 'short' }, { timeZoneName: 'shortOffset' },
  { year: 'numeric', month: 'short', day: 'numeric', hour: 'numeric', minute: 'numeric' }, { weekday: 'long', hour: 'numeric' },
  { year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric', timeZoneName: 'short' },
  { weekday: 'short', year: 'numeric', month: 'numeric', day: 'numeric', hour: '2-digit', minute: '2-digit', hour12: false }
];
['en', 'de'].forEach(function (l) {
  sets.forEach(function (o) { t(l + ' ' + show(o), function () { return f(l, o); }); });
});
// Locales outside Broiler.VM's data: what each engine resolves and writes.
['fr', 'es', 'it', 'ja', 'ko', 'zh-TW', 'ar-EG', 'ru', 'en-GB', 'en-IN', 'de-AT', 'pt-BR'].forEach(function (l) {
  t(l + ' locale, default, dateStyle long, month long, weekday long, timeStyle short', function () {
    return [new Intl.DateTimeFormat(l).resolvedOptions().locale, f(l, {}), f(l, { dateStyle: 'long' }), f(l, { month: 'long' }), f(l, { weekday: 'long' }), f(l, { timeStyle: 'short' })];
  });
});
// Every month's and weekday's name in each width, en and de.
['en', 'de'].forEach(function (l) {
  ['long', 'short', 'narrow'].forEach(function (w) {
    t(l + ' months ' + w + ' (format, with a day)', function () { var r = []; for (var m = 0; m < 12; m++) r.push(new Intl.DateTimeFormat(l, { month: w, day: 'numeric', timeZone: 'UTC' }).format(Date.UTC(2024, m, 1))); return r; });
    t(l + ' months ' + w + ' (stand-alone)', function () { var r = []; for (var m = 0; m < 12; m++) r.push(new Intl.DateTimeFormat(l, { month: w, timeZone: 'UTC' }).format(Date.UTC(2024, m, 1))); return r; });
    t(l + ' weekdays ' + w, function () { var r = []; for (var d = 0; d < 7; d++) r.push(new Intl.DateTimeFormat(l, { weekday: w, timeZone: 'UTC' }).format(Date.UTC(2024, 0, 7 + d))); return r; });
  });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
