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
// Rows S1 (list patterns by locale, type and style), S2 (the template chosen by the elements) and S3
// (StringListFromIterable, the parts, the constructor's reads).
var locs = ['en', 'en-GB', 'de', 'es', 'fr', 'it', 'ja', 'zh', 'pl', 'ru', 'he', 'ar', 'xyz'];
locs.forEach(function (l) {
  ['conjunction', 'disjunction', 'unit'].forEach(function (ty) {
    ['long', 'short', 'narrow'].forEach(function (st) {
      t(l + ' ' + ty + ' ' + st, function () {
        var f = new Intl.ListFormat(l, { type: ty, style: st });
        return [f.resolvedOptions().locale, f.format(['A', 'B']), f.format(['A', 'B', 'C', 'D'])];
      });
    });
  });
});
t('es conjunction: y before i, hi, hie, y', function () { var f = new Intl.ListFormat('es'); return [f.format(['a', 'iglesia']), f.format(['a', 'hijo']), f.format(['a', 'hielo']), f.format(['a', 'b', 'Isabel'])]; });
t('es disjunction: o before o, ho, 8, 11', function () { var f = new Intl.ListFormat('es', { type: 'disjunction' }); return [f.format(['a', 'otro']), f.format(['a', 'hora']), f.format(['7', '8']), f.format(['10', '11'])]; });
t('he conjunction: Hebrew and non-Hebrew last element', function () { var f = new Intl.ListFormat('he'); return [f.format(['א', 'ב']), f.format(['א', 'B']), f.format(['א', '1'])]; });
t('en formatToParts(["a", "", "c"])', function () { return parts(new Intl.ListFormat('en').formatToParts(['a', '', 'c'])); });
t('de formatToParts(["a", "b", "c", "d"])', function () { return parts(new Intl.ListFormat('de').formatToParts(['a', 'b', 'c', 'd'])); });
t('format of a string, a Set, undefined, []', function () { var f = new Intl.ListFormat('en'); return [f.format('abc'), f.format(new Set(['x', 'y'])), f.format(undefined), f.format([])]; });
t('format(null)', function () { return new Intl.ListFormat('en').format(null); });
t('format([1, 2])', function () { return new Intl.ListFormat('en').format([1, 2]); });
t('a non-String element closes the iterator', function () {
  var closed = 0;
  var it = { i: 0, next: function () { this.i++; return this.i === 1 ? { value: 'a', done: false } : { value: 1, done: false }; }, return: function () { closed++; return {}; } };
  var iterable = {}; iterable[Symbol.iterator] = function () { return it; };
  try { new Intl.ListFormat('en').format(iterable); return 'no error'; } catch (e) { return [e.name, closed]; }
});
t('elements with "{0}" and "{1}"', function () { return new Intl.ListFormat('en').format(['{1}', '{0}', '{0}{1}']); });
var log = [];
var lf = new Intl.ListFormat('en', spy({ localeMatcher: 'lookup', type: 'unit', style: 'narrow' }, log));
t('constructor reads', function () { return log; });
t('resolvedOptions keys', function () { return Object.keys(lf.resolvedOptions()); });
t('options a string', function () { return new Intl.ListFormat('en', 'conjunction').resolvedOptions().type; });
t('options null', function () { return new Intl.ListFormat('en', null).resolvedOptions().type; });
t('ListFormat without new', function () { return Intl.ListFormat('en'); });
t('invalid type, style', function () { return ['type', 'style'].map(function (k) { var o = {}; o[k] = 'bogus'; try { new Intl.ListFormat('en', o); return k + ':ok'; } catch (e) { return k + ':' + e.name; } }).join(' '); });
t('format and formatToParts identity and length', function () { var f = new Intl.ListFormat('en'); return [f.format === f.format, Intl.ListFormat.prototype.format.length, Intl.ListFormat.prototype.formatToParts.length]; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
