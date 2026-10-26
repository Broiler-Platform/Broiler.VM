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
// Rows S4 (relative-time patterns by locale and style, numeric "auto"), S5 (the numbering system),
// S6 (the plural the pattern is chosen by) and S7 (the sign of the number written).
var locs = ['en', 'de', 'es', 'fr', 'pl', 'ja', 'ru', 'ar', 'xyz'];
locs.forEach(function (l) {
  ['long', 'short', 'narrow'].forEach(function (st) {
    t(l + ' ' + st + ' always: -3 day, 1 hour, 1000 second, 2 quarter', function () {
      var f = new Intl.RelativeTimeFormat(l, { style: st });
      return [f.resolvedOptions().locale, f.format(-3, 'day'), f.format(1, 'hour'), f.format(1000, 'second'), f.format(2, 'quarter')];
    });
  });
  t(l + ' long auto: -2, -1, 0, -0, 1, 2 day; 0 second; -1 year', function () {
    var f = new Intl.RelativeTimeFormat(l, { numeric: 'auto' });
    return [f.format(-2, 'day'), f.format(-1, 'day'), f.format(0, 'day'), f.format(-0, 'day'), f.format(1, 'day'), f.format(2, 'day'), f.format(0, 'second'), f.format(-1, 'year')];
  });
});
t('en short auto: -1 day, 1 week, 0 month (short patterns fall back)', function () { var f = new Intl.RelativeTimeFormat('en', { numeric: 'auto', style: 'short' }); return [f.format(-1, 'day'), f.format(1, 'week'), f.format(0, 'month')]; });
t('en numberingSystem "arab": 12 day, resolved', function () { var f = new Intl.RelativeTimeFormat('en', { numberingSystem: 'arab' }); return [f.format(12, 'day'), f.resolvedOptions().numberingSystem, f.resolvedOptions().locale]; });
t('en-u-nu-arab: 12 day, resolved', function () { var f = new Intl.RelativeTimeFormat('en-u-nu-arab'); return [f.format(12, 'day'), f.resolvedOptions().numberingSystem, f.resolvedOptions().locale]; });
t('de numberingSystem "thai": -1234.5 day', function () { return new Intl.RelativeTimeFormat('de', { numberingSystem: 'thai' }).format(-1234.5, 'day'); });
t('en numberingSystem "arab" formatToParts(12, "day")', function () { return parts(new Intl.RelativeTimeFormat('en', { numberingSystem: 'arab' }).formatToParts(12, 'day')); });
t('en always: 1.0001, 1.0004, -1.0004, 0.9999, 1.5, 1.25 day', function () { var f = new Intl.RelativeTimeFormat('en'); return [f.format(1.0001, 'day'), f.format(1.0004, 'day'), f.format(-1.0004, 'day'), f.format(0.9999, 'day'), f.format(1.5, 'day'), f.format(1.25, 'day')]; });
t('de always: 1.0001 day, -1.0001 year', function () { var f = new Intl.RelativeTimeFormat('de'); return [f.format(1.0001, 'day'), f.format(-1.0001, 'year')]; });
t('en always: -1, -0, 0, -1000 second (the sign of the number)', function () { var f = new Intl.RelativeTimeFormat('en'); return [f.format(-1, 'second'), f.format(-0, 'second'), f.format(0, 'second'), f.format(-1000, 'second')]; });
t('en formatToParts(-1000.5, "day")', function () { return parts(new Intl.RelativeTimeFormat('en').formatToParts(-1000.5, 'day')); });
t('en formatToParts(1000, "seconds") units', function () { return new Intl.RelativeTimeFormat('en').formatToParts(1000, 'seconds').map(function (p) { return p.unit === undefined ? '-' : p.unit; }); });
t('en auto formatToParts(-1, "day")', function () { return parts(new Intl.RelativeTimeFormat('en', { numeric: 'auto' }).formatToParts(-1, 'day')); });
t('en 1e21 day, 123456789.123 day', function () { var f = new Intl.RelativeTimeFormat('en'); return [f.format(1e21, 'day'), f.format(123456789.123, 'day')]; });
t('format("2", "day"), format(2n, "day")', function () { var f = new Intl.RelativeTimeFormat('en'); return [f.format('2', 'day'), (function () { try { return f.format(2n, 'day'); } catch (e) { return e.name; } })()]; });
t('NaN, Infinity, unit "decade", unit "Day"', function () { var f = new Intl.RelativeTimeFormat('en'); return [[NaN, 'day'], [Infinity, 'day'], [1, 'decade'], [1, 'Day']].map(function (a) { try { return f.format(a[0], a[1]); } catch (e) { return e.name; } }); });
t('ToNumber before ToString(unit)', function () { var log = []; try { new Intl.RelativeTimeFormat('en').format({ valueOf: function () { log.push('value'); return 1; } }, { toString: function () { log.push('unit'); return 'day'; } }); } catch (e) { log.push(e.name); } return log; });
var log = [];
var rf = new Intl.RelativeTimeFormat('en', spy({ localeMatcher: 'lookup', numberingSystem: 'latn', style: 'short', numeric: 'auto' }, log));
t('constructor reads', function () { return log; });
t('resolvedOptions keys', function () { return Object.keys(rf.resolvedOptions()); });
t('RelativeTimeFormat without new', function () { return Intl.RelativeTimeFormat('en'); });
t('invalid style, numeric, numberingSystem', function () { return [['style', 'bogus'], ['numeric', 'bogus'], ['numberingSystem', 'latn!']].map(function (p) { var o = {}; o[p[0]] = p[1]; try { new Intl.RelativeTimeFormat('en', o); return p[0] + ':ok'; } catch (e) { return p[0] + ':' + e.name; } }).join(' '); });
t('options a string (coerced)', function () { return new Intl.RelativeTimeFormat('en', 'x').resolvedOptions().style; });
t('options null', function () { try { return new Intl.RelativeTimeFormat('en', null).resolvedOptions().style; } catch (e) { return e.name; } });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
