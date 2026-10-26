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
// SJL-9.1 sub-area (a), rows L29-L34: Intl.supportedValuesOf (ECMA-402 8.3.2 and AvailableCanonical* 6.x),
// each key's list shown as its count, its first and last values, and its order and duplicates.
function summary(a) {
  var sorted = true, dup = false;
  for (var i = 1; i < a.length; i++) { if (a[i - 1] > a[i]) sorted = false; if (a[i - 1] === a[i]) dup = true; }
  return R(a.length + ' values, sorted ' + sorted + ', duplicates ' + dup + ': ' + show(a.slice(0, 6)) + ' ... ' + show(a.slice(-3)));
}
['calendar', 'collation', 'currency', 'numberingSystem', 'timeZone', 'unit'].forEach(function (k) {
  t('supportedValuesOf ' + q(k), function () { return summary(Intl.supportedValuesOf(k)); });
});
t('calendar, whole', function () { return Intl.supportedValuesOf('calendar'); });
t('collation, whole', function () { return Intl.supportedValuesOf('collation'); });
t('numberingSystem, whole', function () { return Intl.supportedValuesOf('numberingSystem'); });
t('unit, whole', function () { return Intl.supportedValuesOf('unit'); });
t('currency: EUR, USD, XXX, XTS, ZWL, VES, VEB, SLE, XCG, ANG', function () {
  var c = Intl.supportedValuesOf('currency');
  return ['EUR', 'USD', 'XXX', 'XTS', 'ZWL', 'VES', 'VEB', 'SLE', 'XCG', 'ANG'].map(function (x) { return x + ':' + (c.indexOf(x) >= 0); });
});
t('timeZone: links and primaries', function () {
  var z = Intl.supportedValuesOf('timeZone');
  return ['UTC', 'Etc/UTC', 'Etc/GMT', 'Asia/Kolkata', 'Asia/Calcutta', 'Europe/Kyiv', 'Europe/Kiev', 'America/Buenos_Aires', 'America/Argentina/Buenos_Aires', 'Antarctica/South_Pole', 'EST5EDT', 'Etc/GMT+5'].map(function (x) { return x + ':' + (z.indexOf(x) >= 0); });
});
// Each calendar and numbering system listed is accepted by the services that use it.
t('every calendar resolves in DateTimeFormat', function () {
  return Intl.supportedValuesOf('calendar').filter(function (c) { return new Intl.DateTimeFormat('en', { calendar: c }).resolvedOptions().calendar !== c; });
});
t('every numbering system resolves in NumberFormat', function () {
  return Intl.supportedValuesOf('numberingSystem').filter(function (n) { return new Intl.NumberFormat('en', { numberingSystem: n }).resolvedOptions().numberingSystem !== n; });
});
t('every collation resolves in some Collator', function () {
  return Intl.supportedValuesOf('collation').filter(function (co) {
    return ['en', 'de', 'zh', 'ja', 'ko', 'es', 'sv'].every(function (l) { return new Intl.Collator(l, { collation: co }).resolvedOptions().collation !== co; });
  });
});
t('every unit is accepted by NumberFormat', function () {
  return Intl.supportedValuesOf('unit').filter(function (u) { try { new Intl.NumberFormat('en', { style: 'unit', unit: u }); return false; } catch (e) { return true; } });
});
t('every currency is well-formed', function () { return Intl.supportedValuesOf('currency').filter(function (c) { return !/^[A-Z]{3}$/.test(c); }); });
// Keys that are refused, and the key's coercion.
['', 'calendars', 'Calendar', 'region', 'language', 'hourCycle'].forEach(function (k) {
  t('supportedValuesOf ' + q(k), function () { return Intl.supportedValuesOf(k); });
});
t('supportedValuesOf undefined', function () { return Intl.supportedValuesOf(); });
t('supportedValuesOf a Symbol', function () { return Intl.supportedValuesOf(Symbol('calendar')); });
t('supportedValuesOf {toString}', function () { return summary(Intl.supportedValuesOf({ toString: function () { return 'calendar'; } })); });
t('fresh array each call', function () { return Intl.supportedValuesOf('calendar') !== Intl.supportedValuesOf('calendar'); });
t('length, name', function () { return [Intl.supportedValuesOf.length, Intl.supportedValuesOf.name]; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
