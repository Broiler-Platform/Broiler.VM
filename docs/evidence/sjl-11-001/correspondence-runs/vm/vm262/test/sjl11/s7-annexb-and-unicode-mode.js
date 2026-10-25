/*---
description: SJL-11.1 probe s7-annexb-and-unicode-mode
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
// S7: Annex B's relaxations without u, and their refusal with u (the survey's list); the modifiers
// (F3), each by the constructor, the literal evaluated and the literal never evaluated.
// Broiler.JS runs this probe under the invariant culture, tr-TR and lt-LT (it holds (?i:) rows);
// the first line shows the culture each run took.
t('host default locale (Intl)', function () { return new Intl.NumberFormat().resolvedOptions().locale; });
var cases = [
  ['(?=a){2}', '', 'a'], ['(?=a){2}', 'u'], ['(?<=a)*', ''], ['(?!a)+', '', 'b'],
  ['{', '', '{'], ['{', 'u'], ['a{', '', 'a{'], ['a{1', '', 'a{1'], ['a{1,', '', 'a{1,'], ['x{2,1}', ''],
  [']', '', ']'], [']', 'u'], ['}', '', '}'], ['}', 'u'],
  ['[\\d-a]', '', '-'], ['[\\d-a]', 'u'], ['[a-\\d]', '', '-'], ['[--\\d]', '', '-'],
  ['a**', ''], ['a{2147483648}', '', 'a'], ['a{2147483648,}', '', 'a'], ['a{9007199254740993}', '', 'a'],
  ['[b-a]', ''], ['(?:)', '', ''], ['()', '', ''], ['[]', '', 'a'], ['[^]', '', 'a'],
  ['(', ''], [')', ''], ['a)', ''], ['(?', ''], ['(?a)', ''], ['[', ''], ['\\', ''],
  ['(?i:a)b', '', 'Ab'], ['(?i:a)b', '', 'AB'], ['(?-i:a)b', 'i', 'aB'], ['(?-i:a)b', 'i', 'AB'],
  ['(?ii:a)', ''], ['(?i-i:a)', ''], ['(?-:a)', ''], ['(?i)a', ''], ['(?u:a)', ''], ['(?m-s:a)', '', 'a'],
  ['(?s:.)', '', '\n'], ['(?m:^b)', '', 'a\nb'], ['(?i:\\u212a)', 'u', 'k'],
  // The modifier patterns of the literal tests test/language/literals/regexp/early-err-modifiers-*.js
  // and early-err-arithmetic-modifiers-*.js that Broiler.JS fails at sjl-10-001's after run (each
  // file's literal follows $DONOTEVALUATE(), so the error must be early; the third line of each
  // case is that path).
  ['(?imsi:a)', ''], ['(?I:a)', ''], ['(?M:a)', ''], ['(?S:a)', ''],
  ['(?m-m:a)', ''], ['(?s-s:a)', ''], ['(?ims-m:a)', ''], ['(?m-ims:a)', ''],
  ['(?-ii:a)', ''], ['(?-imsi:a)', ''], ['(?ii-:a)', ''], ['(?imsi-:a)', ''],
  ['(?I-:a)', ''], ['(?M-:a)', ''], ['(?S-:a)', ''], ['(?-I:a)', ''], ['(?-M:a)', ''], ['(?-S:a)', ''],
  ['(?ms-i)', ''], ['(?-s)', ''], ['(?i-)', ''],
];
for (var i = 0; i < cases.length; i++) {
  (function (c) {
    both(q(c[0]) + ' /' + c[1] + (c[2] !== undefined ? ' on ' + q(c[2]) : ''), c[0], c[1], c[2]);
  })(cases[i]);
}
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
