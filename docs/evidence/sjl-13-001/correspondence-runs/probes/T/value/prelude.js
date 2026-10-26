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
