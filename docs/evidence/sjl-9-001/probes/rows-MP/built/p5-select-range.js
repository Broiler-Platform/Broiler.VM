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
// Row P5: Intl.PluralRules.prototype.selectRange (ECMA-402 17.3.4, ResolvePluralRange 17.5.4): the start's
// category when both ends format alike, otherwise PluralRuleSelectRange (implementation-dependent, or "other").
var en = new Intl.PluralRules('en'), de = new Intl.PluralRules('de');
t('P5 en (1,1) (1,1.0001) (-1,1) ("1","1.0")', function () { return [en.selectRange(1, 1), en.selectRange(1, 1.0001), en.selectRange(-1, 1), en.selectRange('1', '1.0')]; });
t('P5 de (1,1); en {minFD 1} (1,1)', function () { return [de.selectRange(1, 1), new Intl.PluralRules('en', { minimumFractionDigits: 1 }).selectRange(1, 1)]; });
t('P5 en (0,1) (1,2) (2,1) (0,0.5) (1,1.5)', function () { return [en.selectRange(0, 1), en.selectRange(1, 2), en.selectRange(2, 1), en.selectRange(0, 0.5), en.selectRange(1, 1.5)]; });
t('P5 de (0,1) (1,2) (2,1) (1,1.5)', function () { return [de.selectRange(0, 1), de.selectRange(1, 2), de.selectRange(2, 1), de.selectRange(1, 1.5)]; });
t('P5 en ordinal (1,2) (1,1) (2,3)', function () { var o = new Intl.PluralRules('en', { type: 'ordinal' }); return [o.selectRange(1, 2), o.selectRange(1, 1), o.selectRange(2, 3)]; });
t('P5 fr (0,1) (1,2)', function () { var f = new Intl.PluralRules('fr'); return [f.resolvedOptions().locale, f.selectRange(0, 1), f.selectRange(1, 2)]; });
t('P5 (Infinity, Infinity), (5, 1) no throw, (-Infinity, Infinity)', function () { return [en.selectRange(Infinity, Infinity), en.selectRange(5, 1), en.selectRange(-Infinity, Infinity)]; });
t('P5 (NaN, 1)', function () { return en.selectRange(NaN, 1); });
t('P5 (undefined, 1)', function () { return en.selectRange(undefined, 1); });
t('P5 (1) one argument', function () { return en.selectRange(1); });
t('P5 order of coercion: start then end, both before the NaN test', function () { var log = []; try { en.selectRange({ valueOf: function () { log.push('start'); return NaN; } }, { valueOf: function () { log.push('end'); return 1; } }); } catch (e) { log.push(errName(e)); } return log; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
