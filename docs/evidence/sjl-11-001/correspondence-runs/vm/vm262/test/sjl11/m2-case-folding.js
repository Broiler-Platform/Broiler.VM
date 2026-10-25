/*---
description: SJL-11.1 probe m2-case-folding
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
// M2: case-insensitive matching (Canonicalize: simple case folding with u or v; without them the
// simple upper case of one code unit, a non-ASCII character never folding to ASCII). Broiler.JS
// runs this probe under the invariant culture, tr-TR and lt-LT; the language reads no culture.
t('host default locale (Intl)', function () { return new Intl.NumberFormat().resolvedOptions().locale; });
t('host toLocaleUpperCase of "i"', function () { return 'i'.toLocaleUpperCase(); });
var cases = [
  // F2 of bundle sjl-7-001 and its neighbours.
  ['[a-z]', 'i', '\u212a'], ['[a-z]', 'iu', '\u212a'], ['\u212a', 'i', 'k'], ['\u212a', 'iu', 'k'], ['k', 'i', '\u212a'],
  ['[a-z]', 'i', '\u017f'], ['[a-z]', 'iu', '\u017f'], ['\u017f', 'i', 's'], ['\u017f', 'iu', 'S'], ['s', 'i', '\u017f'],
  ['\\w', 'iu', '\u017f'], ['\\W', 'iu', '\u017f'], ['\\w', 'i', '\u017f'], ['[\\w]', 'iu', '\u212a'], ['\\b', 'iu', '\u017f'],
  // The dotted and dotless i: the Turkic mappings are not Canonicalize's.
  ['i', 'i', '\u0130'], ['I', 'i', '\u0131'], ['i', 'i', '\u0131'], ['I', 'i', '\u0130'],
  ['\u0130', 'i', 'i'], ['\u0130', 'i', 'I'], ['\u0131', 'i', 'i'], ['\u0131', 'i', 'I'],
  ['[a-z]', 'i', '\u0130'], ['[a-z]', 'i', '\u0131'], ['[\u0130]', 'i', 'i'], ['[\u0131]', 'i', 'I'],
  ['i', 'iu', '\u0130'], ['I', 'iu', '\u0131'], ['\u0130', 'iu', 'i'], ['\u0131', 'iu', 'I'],
  ['[a-z]', 'iu', '\u0130'], ['[a-z]', 'iu', '\u0131'], ['i', 'iv', '\u0130'], ['[a-z]', 'iv', '\u0131'],
  ['(?i:i)', '', '\u0130'], ['(?i:[a-z])', '', '\u0131'],
  // Lithuanian: i with a combining dot above, and the soft-dotted capitals.
  ['i\u0307', 'i', '\u0130'], ['\u00cc', 'i', '\u00ec'], ['\u00cc', 'i', 'i\u0307\u0300'], ['\u0128', 'i', '\u0129'],
  // Others.
  ['\u03c3', 'i', '\u03c2'], ['\u03c3', 'iu', '\u03c2'], ['\u00df', 'i', '\u1e9e'], ['\u00df', 'iu', '\u1e9e'],
  ['\u1f80', 'i', '\u1f88'], ['\u0390', 'iu', '\u1fd3'], ['\u0390', 'i', '\u1fd3'],
  ['\\u{10400}', 'iu', '\ud801\udc28'], ['\u00b5', 'i', '\u039c'], ['\u00b5', 'iu', '\u03bc'],
  ['\ua7ce', 'i', '\ua7cf'], ['\ua7ce', 'iu', '\ua7cf'], ['\u1c89', 'i', '\u1c8a'], ['\u1c89', 'iu', '\u1c8a'],
  ['\\p{Lu}', 'iu', 'a'], ['\\P{Lu}', 'iu', 'A'], ['[^\\p{Lu}]', 'iu', 'A'], ['\\p{Lowercase_Letter}', 'iv', 'A'],
  ['(a)\\1', 'i', 'aA'], ['(\u0131)\\1', 'i', '\u0131I'], ['(i)\\1', 'i', 'i\u0130'],
];
for (var i = 0; i < cases.length; i++) {
  (function (c) {
    t(q(c[0]) + ' /' + c[1] + ' test ' + q(c[2]), function () { return C(c[0], c[1]).test(c[2]); });
  })(cases[i]);
}
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
