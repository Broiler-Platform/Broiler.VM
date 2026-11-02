// SJL-17.1 (bundle sjl-17-001): bundle sjl-9-001's probes/prelude.js, unchanged from the next line on.
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
// O4 to O7: the four options - sensitivity, ignorePunctuation, numeric, caseFirst - with their keywords and defaults.
function sgn(n) { return n < 0 ? '<' : n > 0 ? '>' : '='; }
function ord(c, words) {
  var w = words.slice().sort(c.compare), s = q(w[0]);
  for (var i = 1; i < w.length; i++) s += ' ' + sgn(c.compare(w[i - 1], w[i])) + ' ' + q(w[i]);
  return R(s);
}
function ro(c) { var r = c.resolvedOptions(); return r.locale + ' ' + r.usage + ' ' + r.sensitivity + ' ip=' + r.ignorePunctuation + ' co=' + r.collation + ' kn=' + r.numeric + ' kf=' + r.caseFirst; }
var acc = ['a', 'A', 'á', 'Á', 'b', 'ä', 'Ä'];
['base', 'accent', 'case', 'variant'].forEach(function (s) {
  t('o4 sensitivity ' + s, function () { var c = new Intl.Collator('en', { sensitivity: s }); return R(ord(c, acc).__raw); });
  t('o4 sensitivity ' + s + ' search', function () { var c = new Intl.Collator('en', { sensitivity: s, usage: 'search' }); return R(ord(c, acc).__raw); });
});
t('o4 defaults sort', function () { return ro(new Intl.Collator('en')); });
t('o4 defaults search', function () { return ro(new Intl.Collator('en', { usage: 'search' })); });
t('o4 defaults de search', function () { return ro(new Intl.Collator('de', { usage: 'search' })); });
var pu = ['ab', 'a b', 'a-b', 'a_b', 'a.b', 'a b', 'aB', 'a‐b', 'a$b', 'a+b', 'a€b', '', ' ', '-'];
[false, true].forEach(function (ip) {
  t('o5 ignorePunctuation ' + ip, function () { var c = new Intl.Collator('en', { ignorePunctuation: ip }); return R(ord(c, pu).__raw); });
  t('o5 ignorePunctuation ' + ip + ' base', function () { var c = new Intl.Collator('en', { ignorePunctuation: ip, sensitivity: 'base' }); return R(ord(c, pu).__raw); });
});
t('o5 th default', function () { return ro(new Intl.Collator('th')); });
t('o5 th-u-ka-noignore', function () { return ro(new Intl.Collator('th-u-ka-noignore')); });
var nu = ['a10', 'a2', 'a02', 'a1', 'a01', 'a001', 'a9', 'a١٠', 'a٢', 'a２', '10', '9', '1.5', '1.10', 'b1', 'a'];
t('o6 numeric absent', function () { var c = new Intl.Collator('en'); return R(ro(c) + ': ' + ord(c, nu).__raw); });
t('o6 numeric true', function () { var c = new Intl.Collator('en', { numeric: true }); return R(ro(c) + ': ' + ord(c, nu).__raw); });
t('o6 -u-kn', function () { var c = new Intl.Collator('en-u-kn'); return R(ro(c) + ': ' + ord(c, nu).__raw); });
t('o6 -u-kn-false', function () { var c = new Intl.Collator('en-u-kn-false'); return R(ro(c)); });
t('o6 -u-kn with numeric false', function () { var c = new Intl.Collator('en-u-kn', { numeric: false }); return R(ro(c) + ': ' + sgn(c.compare('a2', 'a10'))); });
t('o6 a2 a10 localeCompare numeric', function () { return R(sgn('a2'.localeCompare('a10', 'en', { numeric: true }))); });
var cf = ['a', 'A', 'b', 'B', 'ab', 'Ab', 'aB', 'AB', 'ä', 'Ä'];
['false', 'upper', 'lower'].forEach(function (k) {
  t('o7 caseFirst ' + k, function () { var c = new Intl.Collator('en', { caseFirst: k }); return R(ro(c) + ': ' + ord(c, cf).__raw); });
  t('o7 caseFirst ' + k + ' case', function () { var c = new Intl.Collator('en', { caseFirst: k, sensitivity: 'case' }); return R(ord(c, cf).__raw); });
});
t('o7 -u-kf-upper', function () { var c = new Intl.Collator('en-u-kf-upper'); return R(ro(c) + ': ' + ord(c, cf).__raw); });
t('o7 -u-kf-upper caseFirst lower', function () { var c = new Intl.Collator('en-u-kf-upper', { caseFirst: 'lower' }); return R(ro(c)); });
t('o7 de phonebk upper', function () { var c = new Intl.Collator('de-u-co-phonebk-kf-upper'); return R(ro(c) + ': ' + ord(c, ['ä', 'Ä', 'ae', 'Ae', 'AE', 'af']).__raw); });
t('o7 a A localeCompare upper', function () { return R(sgn('a'.localeCompare('A', 'en', { caseFirst: 'upper' }))); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
