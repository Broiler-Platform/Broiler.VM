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
// M5: the sticky flag. The match is attempted at lastIndex only (RegExpBuiltinExec step 13.d.i).
// Answers first; then a timing that shows whether a failure at lastIndex costs one attempt or a
// forward search (the elapsed milliseconds are data of this run, not a claim).
function at(re, s, li) {
  re.lastIndex = li;
  var r = re.exec(s);
  return R((r === null ? 'null' : '@' + r.index + ' ' + show(Array.prototype.slice.call(r))) + ' lastIndex ' + re.lastIndex);
}
t('/a/y lastIndex 0 on "ba"', function () { return at(/a/y, 'ba', 0); });
t('/a/y lastIndex 1 on "ba"', function () { return at(/a/y, 'ba', 1); });
t('/a/uy lastIndex 0 on "ba"', function () { return at(/a/uy, 'ba', 0); });
t('/b|ab/y lastIndex 0 on "ab"', function () { return at(/b|ab/y, 'ab', 0); });
t('/^b/y lastIndex 1 on "ab"', function () { return at(/^b/y, 'ab', 1); });
t('/^b/my lastIndex 2 on "a\\nb"', function () { return at(/^b/my, 'a\nb', 2); });
t('/(?<=a)b/y lastIndex 1 on "ab"', function () { return at(/(?<=a)b/y, 'ab', 1); });
t('/\\bb/y lastIndex 1 on "ab"', function () { return at(/\bb/y, 'ab', 1); });
t('"aab".search(/b/y)', function () { return 'aab'.search(/b/y); });
t('"aab".match(/a/gy)', function () { return 'aab'.match(/a/gy); });
t('"aaba".replace(/a/gy, "-")', function () { return 'aaba'.replace(/a/gy, '-'); });
t('"a,b".split(/,/y)', function () { return 'a,b'.split(/,/y); });
var s = 'y' + 'a'.repeat(22);
t('/x|(a+)+b/y lastIndex 0 on "y"+a^22 (answer)', function () { var e = elapsed(function () { return at(/x|(a+)+b/y, s, 0); }); return R(e.r.__raw + ' elapsedMs ' + e.ms); });
t('/x|(a+)+b/uy lastIndex 0 on "y"+a^22 (answer)', function () { var e = elapsed(function () { return at(/x|(a+)+b/uy, s, 0); }); return R(e.r.__raw + ' elapsedMs ' + e.ms); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
