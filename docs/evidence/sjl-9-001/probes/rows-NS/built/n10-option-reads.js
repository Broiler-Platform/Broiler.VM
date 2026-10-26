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
// Row N10: the unit options (style, currency, currencyDisplay, currencySign, unit, unitDisplay) are read
// once, by the constructor; nothing format or resolvedOptions does reads the options object again.
var log = [];
var nf = new Intl.NumberFormat('en', spy({ style: 'currency', currency: 'EUR', currencyDisplay: 'code', currencySign: 'accounting' }, log));
t('constructor reads (currency options)', function () { return log.slice(); });
log.length = 0;
nf.format(-1);
t('reads by format(-1)', function () { return log.slice(); });
log.length = 0;
nf.formatToParts(-1);
nf.formatRange(1, 2);
t('reads by formatToParts and formatRange', function () { return log.slice(); });
log.length = 0;
nf.resolvedOptions();
t('reads by resolvedOptions', function () { return log.slice(); });
var log2 = [];
var nu = new Intl.NumberFormat('en', spy({ style: 'unit', unit: 'liter', unitDisplay: 'long' }, log2));
log2.length = 0;
nu.format(2); nu.resolvedOptions();
t('unit style: reads by format and resolvedOptions', function () { return log2.slice(); });
var o = { style: 'currency', currency: 'EUR' };
var f = new Intl.NumberFormat('en', o);
t('before mutation: format(1), resolved style/currency', function () { var r = f.resolvedOptions(); return [f.format(1), r.style, r.currency]; });
o.currency = 'usd';
t('after o.currency = "usd": format(1), resolved currency', function () { return [f.format(1), f.resolvedOptions().currency]; });
o.style = 'percent';
t('after o.style = "percent": format(1), resolved style', function () { return [f.format(1), f.resolvedOptions().style]; });
var u = { style: 'unit', unit: 'liter' };
var fu = new Intl.NumberFormat('en', u);
u.unit = 'meter';
t('after u.unit = "meter": format(2), resolved unit', function () { return [fu.format(2), fu.resolvedOptions().unit]; });
var g = { style: 'currency', currency: 'EUR', currencyDisplay: 'symbol', currencySign: 'standard' };
var fg = new Intl.NumberFormat('en', g);
g.currencyDisplay = 'code'; g.currencySign = 'accounting';
t('after currencyDisplay/currencySign changed: format(-1), resolved', function () { var r = fg.resolvedOptions(); return [fg.format(-1), r.currencyDisplay, r.currencySign]; });
t('a getter that throws after construction: format(1)', function () {
  var n = 0;
  var b = { style: 'currency', get currency() { if (n++ > 0) throw new Error('read again'); return 'EUR'; } };
  var h = new Intl.NumberFormat('en', b);
  return h.format(1);
});
t('Number.prototype.toLocaleString reads each time (two calls)', function () {
  var l3 = [];
  var opts = spy({ style: 'percent' }, l3);
  (0.5).toLocaleString('en', opts); (0.5).toLocaleString('en', opts);
  return l3.filter(function (k) { return k === 'style'; }).length;
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
