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
// SJL-9.1 sub-area (a), rows L8-L9: Intl.Locale.prototype.maximize and minimize (ECMA-402 15.3.6, 15.3.7:
// UTS #35's Add Likely Subtags and Remove Likely Subtags over CLDR's likely subtags). One line per tag.
function mx(s) { t('maximize ' + q(s), function () { return new Intl.Locale(s).maximize().toString(); }); }
function mn(s) { t('minimize ' + q(s), function () { return new Intl.Locale(s).minimize().toString(); }); }

['und', 'und-Hant', 'und-Hans', 'und-150', 'und-419', 'und-AQ', 'und-Latn-AQ', 'und-Zzzz', 'und-ZZ', 'und-Zzzz-ZZ',
 'und-Qaaa', 'und-Arab', 'und-PK', 'und-IN', 'und-Latn-IN', 'und-Cyrl-RO', 'und-Arab-IR', 'und-Thai', 'und-Brai',
 'en', 'en-GB', 'en-Shaw', 'en-150', 'zh', 'zh-TW', 'zh-Hant', 'zh-HK', 'zh-Hans-TW', 'zh-MO', 'sr', 'sr-ME',
 'sr-Latn', 'sr-RU', 'ar', 'pa', 'pa-PK', 'pa-Arab', 'hi-Latn', 'uz-AF', 'mn-Mong', 'aaa', 'xyz', 'xyz-US',
 'und-u-ca-gregory', 'en-US-x-foo', 'de-1996', 'de-u-co-phonebk', 'und-Latn-US-fonipa', 'ja', 'ko', 'ru', 'he',
 'fa', 'th', 'tr', 'el', 'und-GR', 'und-001', 'und-Zinh', 'und-Zyyy', 'zh-Hani', 'ja-Hira', 'ku', 'ckb',
 'und-SU', 'und-YU']
  .forEach(mx);
['en-Latn-US', 'en-Latn-GB', 'en-US', 'zh-Hant-TW', 'zh-Hans-CN', 'zh-Hant', 'zh-Hant-HK', 'sr-Cyrl-RS', 'sr-Latn-RS',
 'sr-Latn', 'de-Latn-DE-u-co-phonebk', 'und', 'und-Latn', 'und-US', 'und-Latn-US', 'en-GB', 'es-Latn-419', 'es-419',
 'aaa', 'xyz-Latn', 'ja-Jpan-JP', 'pa-Arab-PK', 'pa-Guru-IN', 'en-Latn-US-1996', 'en-Latn-US-x-foo', 'hi-Latn-IN',
 'uz-Arab-AF', 'und-Zzzz-ZZ', 'zh-Hani', 'ar-Arab-001', 'en-Zzzz-US', 'und-Hant-TW']
  .forEach(mn);
t('maximize keeps the extensions and returns a new Locale', function () {
  var l = new Intl.Locale('und', { calendar: 'japanese', region: 'TW' });
  var m = l.maximize();
  return [m.toString(), m !== l, m instanceof Intl.Locale, l.toString()];
});
t('maximize, minimize on a non-Locale', function () {
  var r = [];
  try { Intl.Locale.prototype.maximize.call({}); r.push('no error'); } catch (e) { r.push(errName(e)); }
  try { Intl.Locale.prototype.minimize.call('en'); r.push('no error'); } catch (e) { r.push(errName(e)); }
  return r;
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
