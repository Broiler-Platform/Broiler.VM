/*---
description: SJL-11.1 probe m4-lastindex-surrogates
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
// M4: lastIndex inside a surrogate pair, and AdvanceStringIndex, under u and v (RegExpBuiltinExec
// steps 12-22: the input index is the code point holding lastIndex; "index" is lastIndex).
var pair = '\ud834\udf06';
function at(re, s, li) {
  re.lastIndex = li;
  var r = re.exec(s);
  return R((r === null ? 'null' : '@' + r.index + ' ' + show(Array.prototype.slice.call(r))) + ' lastIndex ' + re.lastIndex);
}
t('/./gu lastIndex 1 on pair', function () { return at(/./gu, pair, 1); });
t('/./gv lastIndex 1 on pair', function () { return at(/./gv, pair, 1); });
t('/./g lastIndex 1 on pair', function () { return at(/./g, pair, 1); });
t('/\\udf06/gu lastIndex 1 on pair', function () { return at(/\udf06/gu, pair, 1); });
t('/\\udf06/g lastIndex 1 on pair', function () { return at(/\udf06/g, pair, 1); });
t('/\\udf06/u lastIndex 0 on pair', function () { return at(/\udf06/u, pair, 0); });
t('/(?:)/gu lastIndex 1 on pair', function () { return at(/(?:)/gu, pair, 1); });
t('/./yu lastIndex 1 on pair', function () { return at(/./yu, pair, 1); });
t('/\\udf06/yu lastIndex 1 on pair', function () { return at(/\udf06/yu, pair, 1); });
t('/./gu lastIndex 1 on "a"+pair', function () { return at(/./gu, 'a' + pair, 1); });
t('/./gu lastIndex 2 on "a"+pair', function () { return at(/./gu, 'a' + pair, 2); });
t('/(?<=\\ud834)./gu lastIndex 1 on pair', function () { return at(/(?<=\ud834)./gu, pair, 1); });
t('pair.match(/(?:)/gu).length', function () { return pair.match(/(?:)/gu).length; });
t('pair.match(/(?:)/g).length', function () { return pair.match(/(?:)/g).length; });
t('pair.replace(/(?:)/gu, "-")', function () { return pair.replace(/(?:)/gu, '-'); });
t('pair.replace(/(?:)/gv, "-")', function () { return pair.replace(/(?:)/gv, '-'); });
t('pair.split(/(?:)/u)', function () { return pair.split(/(?:)/u); });
t('[...pair.matchAll(/(?:)/gu)].map(r=>r.index)', function () { return Array.from(pair.matchAll(/(?:)/gu)).map(function (r) { return r.index; }); });
t('pair.search(/\\udf06/u)', function () { return pair.search(/\udf06/u); });
t('pair.search(/\\udf06/)', function () { return pair.search(/\udf06/); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
