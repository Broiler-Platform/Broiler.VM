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
// S4: \0 and decimal digits. CharacterEscape :: 0 [lookahead not-in DecimalDigit], where
// DecimalDigit is ASCII 0-9 only; so \0 before U+0661 (ARABIC-INDIC DIGIT ONE) is NUL then U+0661.
var one = '\u0661';
both('\\0\u0661 /u on NUL+U+0661', '\\0' + one, 'u', '\u0000' + one);
both('\\0\u0661 / on NUL+U+0661', '\\0' + one, '', '\u0000' + one);
both('[\\0\u0661] /u on U+0661', '[\\0' + one + ']', 'u', one);
both('\\0\uff11 /u (FULLWIDTH DIGIT ONE) on NUL+U+FF11', '\\0\uff11', 'u', '\u0000\uff11');
both('\\01 /u', '\\01', 'u', '\u0001');
both('\\01 / (legacy octal) on U+0001', '\\01', '', '\u0001');
both('\\00 /u', '\\00', 'u');
both('[\\00] /u', '[\\00]', 'u');
both('\\0 /u on NUL', '\\0', 'u', '\u0000');
both('\\8 / on "8"', '\\8', '', '8');
both('\\8 /u', '\\8', 'u');
both('\\1(a) / on "a"', '\\1(a)', '', 'a');
both('\\2(a) / on U+0002a', '\\2(a)', '', '\u0002a');
both('\\2(a) /u', '\\2(a)', 'u');
both('\\x4 / on "x4"', '\\x4', '', 'x4');
both('\\x4 /u', '\\x4', 'u');
both('\\u12 / on "u12"', '\\u12', '', 'u12');
both('\\u12 /u', '\\u12', 'u');
both('\\u{110000} /u', '\\u{110000}', 'u');
both('\\u{3} / (u then quantifier) on "uuu"', '\\u{3}', '', 'uuu');
both('\\c / on "\\\\c"', '\\c', '', '\\c');
both('\\c /u', '\\c', 'u');
both('[\\c_] / on U+001F', '[\\c_]', '', '\u001f');
both('[\\c_] /u', '[\\c_]', 'u');
both('\\cJ /u on LF', '\\cJ', 'u', '\n');
both('\\- /u', '\\-', 'u');
both('[\\-] /u on "-"', '[\\-]', 'u', '-');
both('\\/ /u on "/"', '\\/', 'u', '/');
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
