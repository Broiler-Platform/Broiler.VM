/*---
description: SJL-11.1 probe s8-unicode-sets
flags: [raw]
---*/
var __vmTest = true;
// Shared prelude (SJL-8's and SJL-10's form, extended for SJL-11.1). Broiler.JS's shell has print;
// Broiler.VM's test262 realm has none, so there the lines are collected and handed back in one
// uncaught Error the conformance root prints (epilogue.js).
var __lines = [];
var __host = typeof __vmTest === 'undefined' && typeof print === 'function';
function out(s) { s = String(s); if (__host) print(s); else __lines.push(s); }
// A string is printed quoted, every code unit outside printable ASCII (and " and \) as \uXXXX,
// so a lone surrogate, a line terminator or U+0085 is visible and the output stays ASCII.
function q(s) {
  var r = '';
  for (var i = 0; i < s.length; i++) {
    var c = s.charCodeAt(i);
    if (c < 0x20 || c > 0x7e || c === 0x22 || c === 0x5c) r += '\\u' + ('0000' + c.toString(16)).slice(-4);
    else r += s.charAt(i);
  }
  return '"' + r + '"';
}
// An engine's error message, unquoted, with every code unit outside printable ASCII as \uXXXX, so
// a NUL or a line terminator a message echoes stays visible and the output stays ASCII.
function plain(s) {
  var r = '';
  for (var i = 0; i < s.length; i++) {
    var c = s.charCodeAt(i);
    r += c < 0x20 || c > 0x7e ? '\\u' + ('0000' + c.toString(16)).slice(-4) : s.charAt(i);
  }
  return r;
}
function show(v) {
  if (v === null) return 'null';
  if (v === undefined) return 'undefined';
  if (v && v.__raw !== undefined) return v.__raw;
  if (typeof v === 'string') return q(v);
  if (typeof v === 'number') return Object.is(v, -0) ? '-0' : String(v);
  if (typeof v === 'boolean' || typeof v === 'bigint') return String(v);
  if (Array.isArray(v)) { var a = []; for (var i = 0; i < v.length; i++) a.push(show(v[i])); return '[' + a.join(', ') + ']'; }
  if (v instanceof RegExp) return 'RegExp ' + q(String(v));
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
function t(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + errName(e) + ': ' + plain(String(e && e.message)); }
  out(label + ' => ' + r);
}
// An exec result: its index, its elements, and its groups and indices when present.
function m(re, s) {
  var r = re.exec(s);
  if (r === null) return R('null');
  var parts = ['@' + r.index, show(Array.prototype.slice.call(r))];
  if (r.groups !== undefined) parts.push('groups' + show(r.groups));
  if (r.indices !== undefined) {
    var ind = [];
    for (var i = 0; i < r.indices.length; i++) ind.push(r.indices[i] === undefined ? 'undefined' : '[' + r.indices[i][0] + ',' + r.indices[i][1] + ']');
    parts.push('indices[' + ind.join(', ') + ']');
    if (r.indices.groups !== undefined) {
      var g = [], gk = Object.keys(r.indices.groups);
      for (var j = 0; j < gk.length; j++) { var x = r.indices.groups[gk[j]]; g.push(gk[j] + ': ' + (x === undefined ? 'undefined' : '[' + x[0] + ',' + x[1] + ']')); }
      parts.push('indices.groups{' + g.join(', ') + '}');
    }
  }
  return R(parts.join(' '));
}
// The constructor path and the literal path (an indirect eval of the literal's text).
function C(p, f) { return new RegExp(p, f); }
function L(p, f) { return (0, eval)('/' + p + '/' + (f || '')); }
// The literal's earliness (13.2.7.1): its text after a statement that throws, compiled by an
// indirect eval, as test262's negative tests place a literal after $DONOTEVALUATE(). The literal
// is never evaluated: a SyntaxError here is an early error; 'not early' means the program was
// admitted and ran, so an error of the literal's, if it has one, comes only when it is evaluated.
function early(p, f) {
  try { (0, eval)('throw "not early";\n/' + p + '/' + (f || '') + ';'); }
  catch (e) { if (e === 'not early') return R('not early'); throw e; }
  return R('the statement before the literal did not throw');
}
// Whether a pattern is accepted, by each path: 'ok' or the error's constructor.
function acc(make, p, f) {
  try { make(p, f); return R('ok'); } catch (e) { return R(errName(e)); }
}
// The three paths: the constructor, the literal evaluated, and the literal never evaluated.
function both(label, p, f, subject) {
  t(label + ' new RegExp', function () { var re = C(p, f); return subject === undefined ? R('ok') : m(re, subject); });
  t(label + ' literal', function () { var re = L(p, f); return subject === undefined ? R('ok') : m(re, subject); });
  t(label + ' literal, not evaluated', function () { return early(p, f); });
}
function elapsed(fn) {
  var t0 = Date.now();
  var r = fn();
  return { r: r, ms: Date.now() - t0 };
}
// S8: the v flag's class syntax and its early errors, each by the constructor, the literal
// evaluated and the literal never evaluated.
// Broiler.JS runs this probe under the invariant culture, tr-TR and lt-LT (it holds vi rows); the
// first line shows the culture each run took.
t('host default locale (Intl)', function () { return new Intl.NumberFormat().resolvedOptions().locale; });
var cases = [
  ['[a&&&b]', 'v'], ['[a--b&&c]', 'v'], ['[(]', 'v'], ['[a-\\d]', 'v'], ['\\q{a}', 'v'], ['[\\q{a}]', 'u'],
  ['[^\\q{ab}]', 'v'], ['[^\\q{a}]', 'v', 'b'], ['[\\q{abc|d}]', 'v', 'abc'], ['[\\q{}]', 'v', ''],
  ['[[a-z]--[aeiou]]', 'v', 'e'], ['[[a-z]--[aeiou]]', 'v', 'b'], ['[\\w&&\\d]', 'v', 'a'], ['[\\w&&\\d]', 'v', '5'],
  ['[\\s--\\d]', 'v', ' '], ['[\\s&&\\S]', 'v', ' '], ['[\\p{L}--\\p{Ll}]', 'v', 'a'], ['[\\p{L}--\\p{Ll}]', 'v', 'A'],
  ['^\\p{RGI_Emoji}$', 'v', '\ud83d\udc68\u200d\ud83d\udc69\u200d\ud83d\udc67\u200d\ud83d\udc66'],
  ['^[\\p{RGI_Emoji}--\\q{\ud83d\ude00}]$', 'v', '\ud83d\ude00'],
  ['[a-z]', 'v', 'q'], ['[|]', 'v'], ['[a|]', 'v'], ['[a\\|]', 'v', '|'], ['[!!]', 'v'], ['[a!!b]', 'v'], ['[a\\!\\!b]', 'v', '!'],
  ['[\\q{KK}]', 'vi', 'kk'], ['[\\q{KK}]', 'vi', '\u212a\u212a'], ['[^[\\w]]', 'vi', '\u017f'],
];
for (var i = 0; i < cases.length; i++) {
  (function (c) {
    both(q(c[0]) + ' /' + c[1] + (c[2] !== undefined ? ' on ' + q(c[2]) : ''), c[0], c[1], c[2]);
  })(cases[i]);
}
// test262's test/built-ins/RegExp/prototype/unicodeSets/breaking-change-from-u-to-v-01.js to -28.js, in
// order: each a literal after $DONOTEVALUATE(), an early SyntaxError under v (the third line of
// each case is that path).
var breaking = [
  '[(]', '[)]', '[[]', '[{]', '[}]', '[/]', '[-]', '[|]', '[&&]', '[!!]',
  '[##]', '[$$]', '[%%]', '[**]', '[++]', '[,,]', '[..]', '[::]', '[;;]', '[<<]',
  '[==]', '[>>]', '[??]', '[@@]', '[``]', '[~~]', '[^^^]', '[_^^]',
];
for (var j = 0; j < breaking.length; j++) {
  both('breaking-change-from-u-to-v-' + ('0' + (j + 1)).slice(-2) + ' ' + q(breaking[j]) + ' /v', breaking[j], 'v');
}
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
