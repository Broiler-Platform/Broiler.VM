// SJL-13.1's correspondence, sub-area P, row P16b (added 2026-10-27): the value probes' shared prelude, sub-area
// T's prelude.js beside T/value/t-values.js with only these first four lines changed. Broiler.JS's shell has print;
// Broiler.VM's test262 realm has none, so there the lines are collected and handed back in one uncaught Error the
// conformance root prints (epilogue.js).
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
// SJL-13.1's correspondence, sub-area P, row P16b (added 2026-10-27): value probes. Each line names its probe, and
// PP<nn> is the parse probe of probe-P16b.manifest that holds the same head. A source is a single-quoted string here,
// and \n puts a line feed into it.
t('PE01 PP11 var, a line break for the first semicolon', function () { return ev('for (var i = 0\ni < 3; i++) ;\ni'); });
t('PE02 PP12 let, a line break for the first semicolon', function () { return ev('var n = 0;\nfor (let i = 0\ni < 3; i++) n++;\nn'); });
t('PE03 PP13 const, a line break for the first semicolon', function () { return ev('var n = 0;\nfor (const i = 0\ni < 3; i++) n++;\nn'); });
t('PE04 PP14 an expression, a line break for the first semicolon', function () { return ev('var i;\nfor (i = 0\ni < 3; i++) ;\ni'); });
t('PE05 PP15 a line break for the second semicolon', function () { return ev('for (var i = 0; i < 3\ni++) ;\ni'); });
t('PE06 PP16 control, a line break in a for-in head', function () { return ev('var r = "";\nfor (var k\nin {a: 1}) r += k;\nr'); });
t('PE07 PP17 control, a line break in a for-of head', function () { return ev('var s = 0;\nfor (var v\nof [1, 2]) s += v;\ns'); });
t('PE08 PP18 control, PE01 with no line break', function () { return ev('for (var i = 0 i < 3; i++) ;\ni'); });
t('PE09 PP19 control, PE01 with its semicolon before the line break', function () { return ev('for (var i = 0;\ni < 3; i++) ;\ni'); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
