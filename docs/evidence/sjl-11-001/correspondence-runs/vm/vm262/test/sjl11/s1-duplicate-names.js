/*---
description: SJL-11.1 probe s1-duplicate-names
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
// S1: duplicate named groups (ES2025, test262 feature regexp-duplicate-named-groups): a name may
// repeat only across alternatives that cannot both participate; \k and groups read the one that did.
both('(?<a>x)|(?<a>y) on "y"', '(?<a>x)|(?<a>y)', '', 'y');
both('(?<a>x)|(?<a>y) /d on "y"', '(?<a>x)|(?<a>y)', 'd', 'y');
both('(?<a>x)|(?<a>y) /u on "y"', '(?<a>x)|(?<a>y)', 'u', 'y');
both('(?<a>x)|(?<a>y) /v on "x"', '(?<a>x)|(?<a>y)', 'v', 'x');
both('(?:(?<a>x)|(?<a>y))\\k<a> on "yy"', '(?:(?<a>x)|(?<a>y))\\k<a>', '', 'yy');
both('(?:(?<a>x)|(?<a>y))\\k<a> /u on "xx"', '(?:(?<a>x)|(?<a>y))\\k<a>', 'u', 'xx');
both('(?<a>x)(?<a>y) (same alternative)', '(?<a>x)(?<a>y)', '', 'xy');
both('(?<a>x)(?<a>y) /u (same alternative)', '(?<a>x)(?<a>y)', 'u', 'xy');
both('(?<a>x)|(?:(?<a>y)|(?<a>z)) on "z"', '(?<a>x)|(?:(?<a>y)|(?<a>z))', '', 'z');
both('((?<a>x)|(?<a>y))(?<a>z) (one alternative repeats)', '((?<a>x)|(?<a>y))(?<a>z)', '', 'xz');
// The same name spelled once with an escape: the duplicate test is on the decoded name.
both('(?<\\u0061>x)(?<a>y) (escaped duplicate, same alternative)', '(?<\\u0061>x)(?<a>y)', '', 'xy');
both('(?<\\u0061>x)|(?<a>y) (escaped duplicate, alternatives) on "y"', '(?<\\u0061>x)|(?<a>y)', '', 'y');
both('(?<\\u0061>x)(?<a>y) /u', '(?<\\u0061>x)(?<a>y)', 'u', 'xy');
t('replace $<a> over (?<a>x)|(?<a>y) /g', function () { return 'xyz'.replace(C('(?<a>x)|(?<a>y)', 'g'), '[$<a>]'); });
t('group property order (?<b>.)(?<a>.)|(?<a>.)(?<b>.)', function () { var r = C('(?<b>.)(?<a>.)|(?<a>.)(?<b>.)').exec('pq'); return R(Object.keys(r.groups).join(',') + ' a=' + r.groups.a + ' b=' + r.groups.b); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
