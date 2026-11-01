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
// O3: the tailorings - German phonebook and search, root search, and the locales E3 names (en, es, sv, th) and CJK.
function sgn(n) { return n < 0 ? '<' : n > 0 ? '>' : '='; }
function ord(c, words) {
  var w = words.slice().sort(c.compare), s = q(w[0]);
  for (var i = 1; i < w.length; i++) s += ' ' + sgn(c.compare(w[i - 1], w[i])) + ' ' + q(w[i]);
  return R(s);
}
var de = ['Ab', 'Ae', 'Äb', 'Af', 'Od', 'Öb', 'Oe', 'Ub', 'Üb', 'Ue', 'ss', 'ß', 'st', 'a', 'ä', 'ae', 'b'];
[['de'], ['de', { collation: 'phonebk' }], ['de-u-co-phonebk'], ['de', { usage: 'search' }], ['de', { usage: 'search', sensitivity: 'base' }],
 ['en'], ['en', { usage: 'search' }], ['en', { usage: 'search', sensitivity: 'base' }], ['und', { usage: 'search' }]].forEach(function (a) {
  t('o3 ' + a[0] + ' ' + show(a[1] || {}) + ' de-words', function () { var c = new Intl.Collator(a[0], a[1]); return R(c.resolvedOptions().locale + '/' + c.resolvedOptions().collation + ': ' + ord(c, de).__raw); });
});
var es = ['n', 'ñ', 'o', 'nz', 'ña', 'ch', 'cz', 'd', 'll', 'lz', 'm'];
var sv = ['z', 'å', 'ä', 'ö', 'æ', 'ø', 'v', 'w', 'y', 'ü', 'a', 'o'];
var th = ['ก', 'เก', 'ข', 'แก', 'ก่', 'กา', '', ' ', 'ฯ', 'a', 'a b', 'ab'];
var cjk = ['一', '丁', '中', '汉', '字', '阿', '啊', '가', 'あ', 'ア', 'a'];
[['es', es], ['es', es, { collation: 'trad' }], ['sv', sv], ['fi', sv], ['th', th], ['zh', cjk], ['zh', cjk, { collation: 'stroke' }], ['ja', cjk], ['ko', cjk], ['tr', ['i', 'ı', 'I', 'İ', 'h', 'j']], ['da', ['aa', 'å', 'z', 'æ', 'ø']]].forEach(function (a) {
  t('o3 ' + a[0] + ' ' + show(a[2] || {}), function () { var c = new Intl.Collator(a[0], a[2]); return R(c.resolvedOptions().locale + '/' + c.resolvedOptions().collation + '/ip=' + c.resolvedOptions().ignorePunctuation + ': ' + ord(c, a[1]).__raw); });
});
t('o3 supportedLocalesOf', function () { return Intl.Collator.supportedLocalesOf(['de', 'de-DE', 'en', 'en-US', 'en-GB', 'es', 'sv', 'th', 'ja', 'zh', 'ko', 'tr', 'und', 'fr']); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
