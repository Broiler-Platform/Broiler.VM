// SJL-13.1 task 1.1, sub-area T: the value probes' shared prelude (SJL-11.1's form,
// Broiler.VM docs/evidence/sjl-11-001/correspondence-runs/vm/probes/prelude.js, cut to what these
// probes use). Broiler.JS's shell has print; Broiler.VM's test262 realm has none, so there the lines
// are collected and handed back in one uncaught Error the conformance root prints (epilogue.js).
var __lines = [];
var __host = typeof __vmTest === 'undefined' && typeof print === 'function';
function out(s) { s = String(s); if (__host) print(s); else __lines.push(s); }
// A string is printed quoted, every code unit outside printable ASCII (and " and \) as \uXXXX.
function q(s) {
  var r = '';
  for (var i = 0; i < s.length; i++) {
    var c = s.charCodeAt(i);
    if (c < 0x20 || c > 0x7e || c === 0x22 || c === 0x5c) r += '\\u' + ('0000' + c.toString(16)).slice(-4);
    else r += s.charAt(i);
  }
  return '"' + r + '"';
}
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
  if (typeof v === 'string') return q(v);
  if (typeof v === 'number') return Object.is(v, -0) ? '-0' : String(v);
  return typeof v + ' ' + String(v);
}
function errName(e) { return (e && e.constructor && e.constructor.name) || typeof e; }
function t(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + errName(e) + ': ' + plain(String(e && e.message)); }
  out(label + ' => ' + r);
}
// Every source under test is compiled by an indirect eval (script goal, sloppy, global), so that a
// source one engine refuses cannot stop the probe file itself from compiling.
function ev(src) { return (0, eval)(src); }
// SJL-13.1 task 1.1, sub-area T: value probes. Each line names its row of rows-T.md. A source is a
// single-quoted string here; a backslash the source itself must hold is written \\, and \n, \u2028
// and the like put that character into the source.
t('T01a U+0085 between tokens', function () { return ev('1\u0085+1'); });
t('T01b U+FEFF between tokens', function () { return ev('\uFEFF1+\uFEFF1'); });
t('T01c U+180E between tokens', function () { return ev('1\u180E+1'); });
t('T01d raw U+FFFF in a string', function () { return ev('"a\uFFFFb".length'); });
t('T01e raw U+FFFF in comments and a template', function () { return ev('/*\uFFFF*/ `x\uFFFFy`.length // \uFFFF'); });
t('T01f raw U+FFFF in a regular expression', function () { return ev('/a\uFFFFb/.source.length'); });
t('T06a #! at a substitution start', function () { return ev('`${#!\n1}`'); });
t('T06b --> at a substitution start', function () { return ev('`${-->\n1}`'); });
t('T06c --> at a line start in a substitution', function () { return ev('`${\n-->\n1}`'); });
t('T07a <!-- holding } in a substitution', function () { return ev('`${1 <!-- }\n}`'); });
t('T07b --> holding } in a substitution', function () { return ev('`${1\n--> }\n}`'); });
t('T08a typeof /}/ in a substitution', function () { return ev('`${typeof /}/}`'); });
t('T08b return /`/ in a substitution', function () { return ev('`${(function () { return /`/.source; })()}`'); });
t('T11a "\\u{000000000041}"', function () { return ev('"\\u{000000000041}"'); });
t('T11b \\u{000000000061} as a binding', function () { return ev('var \\u{000000000061}1 = 5; a1'); });
t('T12a "\\u{0_41}"', function () { return ev('"\\u{0_41}"'); });
t('T12b "\\x_1"', function () { return ev('"\\x_1"'); });
t('T12c tagged `\\x_1`, cooked', function () { return ev('(function (s) { return s[0]; })`\\x_1`'); });
t('T12d untagged `\\x_1`', function () { return ev('`\\x_1`'); });
t('T13a "\\x 1"', function () { return ev('"\\x 1"'); });
t('T13b "\\u 041"', function () { return ev('"\\u 041"'); });
t('T13c "\\x1 ".length', function () { return ev('"\\x1 ".length'); });
t('T13d a\\u 062 as a binding', function () { return ev('var a\\u 062 = 7; ab'); });
t('T13e "\\x<LF>1"', function () { return ev('"\\x\n1".charCodeAt(0)'); });
t('T13f tagged `\\x 1`, cooked', function () { return ev('(function (s) { return s[0]; })`\\x 1`'); });
t('T14a legacy escapes, sloppy', function () { return ev('"\\101|\\400|\\8|\\9|\\0x|\\08"'); });
t('T14b line continuations', function () { return ev('"a\\\nb\\\r\nc\\\rd\\\u2028e\\\u2029f"'); });
t('T14c raw LS and PS in a string', function () { return ev('"a\u2028b\u2029c".length'); });
t('T17a 07.5', function () { return ev('07.5'); });
t('T17b 08.5', function () { return ev('08.5'); });
t('T18a 1.5m', function () { return String(ev('1.5m')); });
t('T19a a ?.\u0663 : 0', function () { return ev('var a = 1; a ?.\u0663 : 0'); });
t('T24a line terminator, then /b/g', function () { return ev('var a = 6, b = 2, g = 3; var r = a\n/b/g; r'); });
t('T24a2 line comment, then /b/g', function () { return ev('var a = 6, b = 2, g = 3; var r = a // c\n/b/g; r'); });
t('T24b object literal, then / 2 / 3', function () { return ev('var r = {valueOf: function () { return 6; }} / 2 / 3; r'); });
t('T24c1 for-of over /a/g.exec', function () { return ev('var n = 0; for (var x of /a/g.exec("a")) n++; n'); });
t('T24c2 extends /x/.constructor', function () { return ev('class A extends /x/.constructor {} typeof A'); });
t('T24e ++/x/.lastIndex', function () { return ev('++/x/.lastIndex'); });
t('T24f await / 2 / 1 in a script', function () { return ev('var await = 4; await / 2 / 1'); });
t('T24g1 yield /a/g in a generator', function () { return ev('function* gen() { yield /a/g; } gen().next().value.source'); });
t('T24g2 yield as a name in an arrow in a generator', function () { return ev('function* gen() { yield (function () { return (() => { var yield = 6, a = 2, g = 3; return yield /a/g; })(); })(); } gen().next().value'); });
t('T24g3 yield as a name outside a generator', function () { return ev('var yield = 6, a = 2, g = 3; yield /a/g'); });
t('T24h if head, then a regular expression', function () { return ev('if (true) /a/g.test("a")'); });
t('T26a /\\u{1F600}/u.source', function () { return ev('/\\u{1F600}/u.source'); });
t('T26b /\\u{61}/u.source', function () { return ev('/\\u{61}/u.source'); });
t('T26c /\\u{D83D}/u.source', function () { return ev('/\\u{D83D}/u.source'); });
t('T28a String.raw after }}', function () { return ev('String.raw`${1}}}x${2}}y`'); });
t('T28b cooked after }}', function () { return ev('(function (s) { return s.join("|"); })`${1}}}x${2}}y`'); });
t('T28c raw of a lone } part', function () { return ev('(function (s) { return s.raw.join("|"); })`${1}}${2}`'); });
t('T28d String.raw, first part opening with }}', function () { return ev('String.raw`}}a${1}`'); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
