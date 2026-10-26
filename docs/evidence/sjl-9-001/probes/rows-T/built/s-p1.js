// Shared prelude for SJL-9.1's probes (SJL-11.1's form, sjl-11-001/correspondence-runs/*/probes/prelude.js,
// with the regular-expression helpers left out and two Intl helpers added). Broiler.JS's shell has print;
// Broiler.VM's test262 realm has none, so there the lines are collected and handed back in one uncaught
// Error the conformance root prints (epilogue.js). The command-line root has print.
var __lines = [];
var __host = typeof __vmTest === 'undefined' && typeof print === 'function';
function out(s) { s = String(s); if (__host) print(s); else __lines.push(s); }
// A string is printed quoted, every code unit outside printable ASCII (and " and \) as \uXXXX,
// so U+202F, U+00A0, a lone surrogate or a line terminator is visible and the output stays ASCII.
function q(s) {
  var r = '';
  for (var i = 0; i < s.length; i++) {
    var c = s.charCodeAt(i);
    if (c < 0x20 || c > 0x7e || c === 0x22 || c === 0x5c) r += '\\u' + ('0000' + c.toString(16)).slice(-4);
    else r += s.charAt(i);
  }
  return '"' + r + '"';
}
// An engine's error message, unquoted, with every code unit outside printable ASCII as \uXXXX.
function plain(s) {
  var r = '';
  for (var i = 0; i < s.length; i++) {
    var c = s.charCodeAt(i);
    r += c < 0x20 || c > 0x7e ? '\\u' + ('0000' + c.toString(16)).slice(-4) : s.charAt(i);
  }
  return r;
}
// Objects are shown with their own enumerable keys in the engine's order (resolvedOptions' order
// is specified, so it is shown as the engine gives it).
function show(v) {
  if (v === null) return 'null';
  if (v === undefined) return 'undefined';
  if (v && v.__raw !== undefined) return v.__raw;
  if (typeof v === 'string') return q(v);
  if (typeof v === 'number') return Object.is(v, -0) ? '-0' : String(v);
  if (typeof v === 'boolean' || typeof v === 'bigint') return String(v);
  if (typeof v === 'function') return 'function ' + (v.name || '(anonymous)');
  if (Array.isArray(v)) { var a = []; for (var i = 0; i < v.length; i++) a.push(show(v[i])); return '[' + a.join(', ') + ']'; }
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
// One line per behaviour: the label, then the value or the error's constructor and message.
function t(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + errName(e) + ': ' + plain(String(e && e.message)); }
  out(label + ' => ' + r);
}
// formatToParts / formatRangeToParts as type:"value" pairs (source too, when present).
function parts(p) {
  var a = [];
  for (var i = 0; i < p.length; i++) a.push(p[i].type + (p[i].source !== undefined ? '/' + p[i].source : '') + ':' + q(String(p[i].value)));
  return R('[' + a.join(', ') + ']');
}
// An options bag that records every property read, in order, for observable-order rows.
function spy(bag, log) {
  var keys = Object.keys(bag);
  var o = {};
  keys.forEach(function (k) {
    Object.defineProperty(o, k, { enumerable: true, get: function () { log.push(k); return bag[k]; } });
  });
  return new Proxy(o, { get: function (target, k, r) { if (typeof k === 'string' && keys.indexOf(k) < 0) log.push(k); return Reflect.get(target, k, r); } });
}
// The JS survey's probe survey-sjl9-js/probe/p1.js, unchanged but for print, which writes through the prelude's out
// (Broiler.VM's test262 realm has no print); each line is the survey's own print call's arguments joined by spaces.
function __p() { out(Array.prototype.slice.call(arguments).join(" ")); }
__p(typeof Intl.DisplayNames.supportedLocalesOf);
var n = new Intl.DisplayNames('de', {type:'region'});
__p(n.of('US'), new Intl.DisplayNames('en',{type:'language'}).of('de-AT'), new Intl.DisplayNames('en',{type:'currency'}).of('EUR'));
__p(new Intl.NumberFormat('en',{maximumFractionDigits:1}).format(1.15), new Intl.NumberFormat('en',{maximumFractionDigits:1}).format(1.25), new Intl.NumberFormat('en',{maximumFractionDigits:2}).format(1.005));
__p(new Intl.PluralRules('en').selectRange(1,2), new Intl.PluralRules('de').selectRange(0,1), new Intl.PluralRules('en',{minimumFractionDigits:1}).select(1));
__p(new Intl.NumberFormat('en',{minimumFractionDigits:2, trailingZeroDisplay:'stripIfInteger'}).format(5));
__p(new Intl.NumberFormat('en',{style:'currency',currency:'EUR',currencyDisplay:'name'}).format(2));
__p(new Intl.DateTimeFormat('de',{month:'long'}).format(0), new Intl.DateTimeFormat('de',{weekday:'long'}).format(0));
__p(new Intl.Collator('en',{numeric:true}).compare('a2','a10'), new Intl.Collator('en',{caseFirst:'upper'}).compare('a','A'));
__p(Intl.DateTimeFormat().resolvedOptions().locale, new Intl.NumberFormat('xx').resolvedOptions().locale, new Intl.NumberFormat('fr-FR').resolvedOptions().locale);
__p(Intl.NumberFormat.supportedLocalesOf(['ja','zz','sw-KE']).join());
var nf = new Intl.NumberFormat('en'); __p(nf.format === nf.format);
var df = new Intl.DateTimeFormat('en'); __p(df.format === df.format);
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
