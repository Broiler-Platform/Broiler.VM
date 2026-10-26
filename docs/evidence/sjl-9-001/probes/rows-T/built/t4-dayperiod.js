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
// SJL-9.1 sub-area (d), rows T13-T14: day periods. The dayPeriod option (CLDR's flexible day periods,
// pattern letter B) alone, with an hour, with an hour and minutes, and inside other formats; noon and
// midnight; the AM/PM marker (a) in en and de. ECMA-402 3d4c558 11.1.2 (Table 7's dayPeriod), and
// FormatDateTimePattern as e8cc03fc amends it ("a String value representing the day period ... depends
// upon the implementation and the effective locale").
function at(h, m) { return Date.UTC(2024, 0, 5, h, m || 0, 0); }
var hours = [[0, 0], [0, 30], [3, 0], [6, 0], [9, 15], [11, 59], [12, 0], [12, 1], [13, 0], [15, 0], [18, 0], [20, 59], [21, 0], [23, 0]];
['en', 'de'].forEach(function (l) {
  ['narrow', 'short', 'long'].forEach(function (w) {
    t(l + ' {dayPeriod:' + w + '} at ' + hours.map(function (h) { return h[0] + ':' + h[1]; }).join(' '), function () {
      var f = new Intl.DateTimeFormat(l, { dayPeriod: w, timeZone: 'UTC' }); return hours.map(function (h) { return f.format(at(h[0], h[1])); });
    });
  });
  t(l + ' {dayPeriod:short, hour:numeric}', function () { var f = new Intl.DateTimeFormat(l, { dayPeriod: 'short', hour: 'numeric', timeZone: 'UTC' }); return hours.map(function (h) { return f.format(at(h[0], h[1])); }); });
  t(l + ' {dayPeriod:long, hour:numeric, minute:numeric}', function () { var f = new Intl.DateTimeFormat(l, { dayPeriod: 'long', hour: 'numeric', minute: 'numeric', timeZone: 'UTC' }); return hours.map(function (h) { return f.format(at(h[0], h[1])); }); });
  t(l + ' {dayPeriod:narrow, hour:numeric, minute:numeric, second:numeric}', function () { var f = new Intl.DateTimeFormat(l, { dayPeriod: 'narrow', hour: 'numeric', minute: 'numeric', second: 'numeric', timeZone: 'UTC' }); return [f.format(at(12, 0)), f.format(at(15, 0))]; });
  t(l + ' {dayPeriod:short, hour:numeric, hourCycle:h23}', function () { var f = new Intl.DateTimeFormat(l, { dayPeriod: 'short', hour: 'numeric', hourCycle: 'h23', timeZone: 'UTC' }); return [f.format(at(9)), f.format(at(15)), f.resolvedOptions().hourCycle]; });
  t(l + ' {dayPeriod:short, year:numeric, month:short, day:numeric}', function () { return new Intl.DateTimeFormat(l, { dayPeriod: 'short', year: 'numeric', month: 'short', day: 'numeric', timeZone: 'UTC' }).format(at(15)); });
  t(l + ' {dayPeriod:short, weekday:long}', function () { return new Intl.DateTimeFormat(l, { dayPeriod: 'short', weekday: 'long', timeZone: 'UTC' }).format(at(15)); });
  t(l + ' {dayPeriod:long, hour:numeric} formatToParts at 15:00', function () { return parts(new Intl.DateTimeFormat(l, { dayPeriod: 'long', hour: 'numeric', timeZone: 'UTC' }).formatToParts(at(15))); });
  t(l + ' {dayPeriod:long, minute:numeric} at 15:07', function () { return new Intl.DateTimeFormat(l, { dayPeriod: 'long', minute: 'numeric', timeZone: 'UTC' }).format(at(15, 7)); });
  t(l + ' {dayPeriod:short} in Asia/Kolkata at 06:00 UTC', function () { return new Intl.DateTimeFormat(l, { dayPeriod: 'short', hour: 'numeric', timeZone: 'Asia/Kolkata' }).format(at(6)); });
  t(l + ' {hour:numeric, hour12:true} AM/PM', function () { var f = new Intl.DateTimeFormat(l, { hour: 'numeric', hour12: true, timeZone: 'UTC' }); return [f.format(at(0)), f.format(at(12)), f.format(at(23))]; });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
