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
// O1 and O2: root order across scripts, canonical equivalence and ignorables, under `en` (no tailoring in either engine's data).
function sgn(n) { return n < 0 ? '<' : n > 0 ? '>' : '='; }
function ord(c, words) {
  var w = words.slice().sort(c.compare), s = q(w[0]);
  for (var i = 1; i < w.length; i++) s += ' ' + sgn(c.compare(w[i - 1], w[i])) + ' ' + q(w[i]);
  return R(s);
}
var en = new Intl.Collator('en');
var scripts = ['a', 'z', 'ß', 'ss', 'æ', 'ae', 'ø', 'œ', 'þ', 'ð', 'ı', 'α', 'ω', 'а', 'я',
  'ა', 'ա', 'א', 'ת', 'ا', 'ي', 'अ', 'ह', 'ก', 'ฮ', '가', '힣', 'あ', 'ア', 'ん',
  '一', '丁', '中', '𠀀', '😀', '1', '١', '१', '½', '①', 'Ꭰ', 'ሀ', 'ཀ', 'ᠠ', 'ꀀ'];
t('o1 scripts', function () { return ord(en, scripts); });
var punct = [' ', '_', '-', ',', ';', ':', '!', '?', '.', "'", '"', '(', ')', '[', '@', '*', '/', '\\', '&', '#', '%', '`', '^', '+', '<', '=', '|', '~', '$', '€', '0', 'a'];
t('o1 punctuation and symbols', function () { return ord(en, punct); });
var pairs = [
  ['å', 'å'], ['Å', 'Å'], ['Ω', 'Ω'], ['q̣̇', 'q̣̇'], ['ḍ̇', 'ḍ̇'],
  ['ä', 'ä'], ['ab', 'a­b'], ['ab', 'a​b'], ['ab', 'a\u0000b'], ['ab', 'a﻿b'], ['a', 'á'], ['ﬁ', 'fi'],
  ['ǆ', 'dž'], ['①', '1'], ['Ａ', 'A'], ['ª', 'a'], ['เก', 'กเ'], ['가', '가'],
  ['', ' '], ['', '­'], ['a b', 'ab'], ['a-b', 'ab'], ['à֮̀̕b', 'à֮̀̕b']];
pairs.forEach(function (p) { t('o2 ' + q(p[0]) + ' vs ' + q(p[1]), function () { return R(sgn(en.compare(p[0], p[1]))); }); });
t('o1 und resolves', function () { return new Intl.Collator('und').resolvedOptions().locale; });
t('o1 root order und = en', function () { var u = new Intl.Collator('und'); return scripts.every(function (a) { return scripts.every(function (b) { return u.compare(a, b) === en.compare(a, b); }); }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
