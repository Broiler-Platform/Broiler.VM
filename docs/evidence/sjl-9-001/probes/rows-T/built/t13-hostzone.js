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
// SJL-9.1 sub-area (d), rows T21 and T28: the leftover item of Broiler.JS's bundle sjl-6-001 (README lines 356
// and 368: formatToParts with a minute and a second and no hour read them in the host's local time), run with
// the host's zone set to Asia/Kolkata (+05:30) against formats in UTC, in the default zone and in Asia/Kathmandu;
// and the timeZone option's ASCII-only case matching (Broiler.JS's sjl-7-001 README line 548 item), with
// letters whose Unicode case mapping reaches an ASCII letter (U+017F LONG S, U+212A KELVIN SIGN, U+0131
// DOTLESS I). ECMA-402 3d4c558 11.1.2 (GetAvailableNamedTimeZoneIdentifier: ASCII-case-insensitive).
var X = Date.UTC(2026, 9, 16, 12, 0, 5);
function ms(o) { var p = { minute: '2-digit', second: '2-digit' }; for (var k in o) p[k] = o[k]; return parts(new Intl.DateTimeFormat('en', p).formatToParts(X)); }
t('{minute, second} timeZone UTC', function () { return ms({ timeZone: 'UTC' }); });
t('{minute, second} no timeZone (the default zone)', function () { return ms({}); });
t('{minute, second} timeZone Asia/Kathmandu', function () { return ms({ timeZone: 'Asia/Kathmandu' }); });
t('format {minute, second} timeZone UTC', function () { return new Intl.DateTimeFormat('en', { minute: '2-digit', second: '2-digit', timeZone: 'UTC' }).format(X); });
t('default zone resolved', function () { return new Intl.DateTimeFormat('en').resolvedOptions().timeZone; });
['America/Loſ_Angeles', 'Asia/Kolkata', 'Asia/Kolkatı', 'AMERICA/LOS_ANGELES', 'america/los_angeles'].forEach(function (z) {
  t('timeZone ' + q(z), function () { return new Intl.DateTimeFormat('en', { timeZone: z }).resolvedOptions().timeZone; });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
