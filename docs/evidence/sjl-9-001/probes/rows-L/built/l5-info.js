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
// SJL-9.1 sub-area (a), rows L14-L20: the Locale information methods (ECMA-402 15.3.10-15.3.16 and their
// abstract operations 15.5.9-15.5.17, the merged Intl Locale Info proposal): one line per method and locale.
var tags = ['en', 'en-US', 'en-GB', 'en-IN', 'de', 'de-DE', 'fr-FR', 'es-419', 'pt-BR', 'ja', 'ja-JP', 'ko-KR', 'zh-TW',
  'zh-Hans-CN', 'ar', 'ar-EG', 'ar-SA', 'he-IL', 'fa-IR', 'th-TH', 'hi-IN', 'ckb-IQ', 'und', 'und-Arab', 'und-Hebr',
  'xyz', 'en-001', 'en-150', 'en-AE', 'en-MV', 'ms-BN',
  'en-u-ca-japanese-co-emoji-hc-h23-nu-arab-fw-wed', 'en-US-u-rg-gbzzzz', 'en-u-sd-usca', 'en-u-rg-sazzzz',
  'en-u-sd-gbsct', 'de-u-co-phonebk', 'en-u-fw-sun', 'en-u-fw-xyz', 'und-u-hc-h11'];
var methods = ['getCalendars', 'getCollations', 'getHourCycles', 'getNumberingSystems', 'getTextInfo', 'getWeekInfo'];
methods.forEach(function (m) {
  tags.forEach(function (tag) {
    t(m + ' ' + q(tag), function () { return new Intl.Locale(tag)[m](); });
  });
});
// getTimeZones: the region's zones; shown as their count and their first and last few.
['en', 'en-US', 'de-DE', 'fr-FR', 'ar-EG', 'zh-TW', 'en-001', 'en-AQ', 'pt-BR', 'en-GB', 'und-150', 'ru-RU', 'en-u-rg-gbzzzz', 'xx-ZZ']
  .forEach(function (tag) {
    t('getTimeZones ' + q(tag), function () {
      var z = new Intl.Locale(tag).getTimeZones();
      if (z === undefined) return z;
      return R(z.length + ' zones: ' + show(z.slice(0, 4)) + (z.length > 4 ? ' ... ' + show(z.slice(-2)) : ''));
    });
  });
// The returned objects: own keys in order, fresh each call, not frozen.
t('getWeekInfo keys and descriptors', function () {
  var w = new Intl.Locale('en-US').getWeekInfo();
  var d = Object.getOwnPropertyDescriptor(w, 'firstDay');
  return [Object.keys(w), d.writable, d.enumerable, d.configurable, Object.getPrototypeOf(w) === Object.prototype];
});
t('getTextInfo keys', function () { return Object.keys(new Intl.Locale('ar').getTextInfo()); });
t('getCalendars returns a fresh array', function () { var l = new Intl.Locale('en'); return [l.getCalendars() !== l.getCalendars(), Array.isArray(l.getCalendars())]; });
t('methods are not getters (Locale Info as merged)', function () {
  return ['calendars', 'collations', 'hourCycles', 'numberingSystems', 'timeZones', 'textInfo', 'weekInfo'].map(function (k) {
    return k + ':' + (Object.getOwnPropertyDescriptor(Intl.Locale.prototype, k) ? 'present' : 'absent');
  });
});
t('method lengths', function () { return methods.concat(['getTimeZones']).map(function (m) { return m + '=' + Intl.Locale.prototype[m].length; }); });
t('a method on a non-Locale', function () { try { Intl.Locale.prototype.getWeekInfo.call({}); return 'no error'; } catch (e) { return R(errName(e)); } });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
