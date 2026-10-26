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
// Rows N3 (currency symbols, placement, digits and spacing) and N4 (currencyDisplay "name").
function cf(l, o) { return new Intl.NumberFormat(l, Object.assign({ style: 'currency' }, o)); }
['en', 'de'].forEach(function (l) {
  ['USD', 'EUR', 'JPY', 'GBP', 'CHF', 'KWD', 'BTN', 'XYZ'].forEach(function (c) {
    ['symbol', 'narrowSymbol', 'code', 'name'].forEach(function (d) {
      t(l + ' ' + c + ' ' + d + ' 1, -1234.5', function () { var f = cf(l, { currency: c, currencyDisplay: d }); return [f.format(1), f.format(-1234.5)]; });
    });
  });
});
t('en EUR name 0, 1, 2, 1.5', function () { var f = cf('en', { currency: 'EUR', currencyDisplay: 'name' }); return [f.format(0), f.format(1), f.format(2), f.format(1.5)]; });
t('de EUR name 1, 2', function () { var f = cf('de', { currency: 'EUR', currencyDisplay: 'name' }); return [f.format(1), f.format(2)]; });
t('en EUR name formatToParts(2)', function () { return parts(cf('en', { currency: 'EUR', currencyDisplay: 'name' }).formatToParts(2)); });
t('en EUR code formatToParts(0)', function () { return parts(cf('en', { currency: 'EUR', currencyDisplay: 'code' }).formatToParts(0)); });
t('en KWD code 1.2345', function () { return cf('en', { currency: 'KWD', currencyDisplay: 'code' }).format(1.2345); });
t('de USD code 1', function () { return cf('de', { currency: 'USD', currencyDisplay: 'code' }).format(1); });
t('en USD accounting -5, signDisplay always 5, 0, -0', function () { var f = cf('en', { currency: 'USD', currencySign: 'accounting', signDisplay: 'always' }); return [f.format(-5), f.format(5), f.format(0), f.format(-0)]; });
t('en USD accounting formatToParts(-5)', function () { return parts(cf('en', { currency: 'USD', currencySign: 'accounting' }).formatToParts(-5)); });
t('de EUR accounting -5', function () { return cf('de', { currency: 'EUR', currencySign: 'accounting' }).format(-5); });
t('en USD exceptZero -0.001, 0.001', function () { var f = cf('en', { currency: 'USD', signDisplay: 'exceptZero' }); return [f.format(-0.001), f.format(0.001)]; });
['fr', 'ja', 'es', 'pt-PT', 'ko-KR', 'zh-TW', 'en-IN', 'sv', 'ar'].forEach(function (l) {
  t(l + ' EUR symbol -1234.5, USD symbol 1', function () { return [cf(l, { currency: 'EUR' }).format(-1234.5), cf(l, { currency: 'USD' }).format(1)]; });
});
t('currency lower case eur resolved and formatted', function () { var f = cf('en', { currency: 'eur' }); return [f.resolvedOptions().currency, f.format(1)]; });
t('currency with maximumFractionDigits 0: JPY 1.5, USD 1.5', function () { return [cf('en', { currency: 'JPY' }).format(1.5), cf('en', { currency: 'USD', maximumFractionDigits: 0 }).format(1.5)]; });
t('currency with minimumFractionDigits 3: USD 1.5', function () { return cf('en', { currency: 'USD', minimumFractionDigits: 3 }).format(1.5); });
t('currency with maximumSignificantDigits 2: USD 1234.5', function () { return cf('en', { currency: 'USD', maximumSignificantDigits: 2 }).format(1234.5); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
