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
// Rows S8 (the names [[Fields]] holds: naming by type, style, language display and fallback), S9 (the
// canonical code: validation and case regularization) and S10 (the constructor, resolvedOptions, supportedLocalesOf).
function dn(l, o) { return new Intl.DisplayNames(l, o); }
function both(l, o, code) { var a = dn(l, o).of(code); var b = dn(l, Object.assign({ fallback: 'none' }, o)).of(code); return [a, b]; }
['en', 'de', 'fr', 'es', 'ja', 'zh-Hant'].forEach(function (l) {
  t(l + ' language dialect: en-GB, de-AT, zh-Hant-TW, xyz, en-XY', function () { return ['en-GB', 'de-AT', 'zh-Hant-TW', 'xyz', 'en-XY'].map(function (c) { return dn(l, { type: 'language' }).of(c); }); });
  t(l + ' region: US, DE, 419, ZZ, AA', function () { return ['US', 'DE', '419', 'ZZ', 'AA'].map(function (c) { return dn(l, { type: 'region' }).of(c); }); });
  t(l + ' script, currency, calendar, dateTimeField', function () { return [dn(l, { type: 'script' }).of('Latn'), dn(l, { type: 'currency' }).of('EUR'), dn(l, { type: 'calendar' }).of('gregory'), dn(l, { type: 'dateTimeField' }).of('weekOfYear')]; });
});
t('en language standard: en-GB, zh-Hant-TW, en-Latn-US', function () { var f = dn('en', { type: 'language', languageDisplay: 'standard' }); return [f.of('en-GB'), f.of('zh-Hant-TW'), f.of('en-Latn-US')]; });
t('en language short and narrow: en-GB, en-US', function () { return [dn('en', { type: 'language', style: 'short' }).of('en-GB'), dn('en', { type: 'language', style: 'narrow' }).of('en-US')]; });
t('en region short: US, GB, HK', function () { var f = dn('en', { type: 'region', style: 'short' }); return [f.of('US'), f.of('GB'), f.of('HK')]; });
t('en fallback code/none: language xyz, region AA, script Qaaa, currency XYZ, calendar xyz', function () { return [both('en', { type: 'language' }, 'xyz'), both('en', { type: 'region' }, 'AA'), both('en', { type: 'script' }, 'Qaaa'), both('en', { type: 'currency' }, 'XYZ'), both('en', { type: 'calendar' }, 'xyz')]; });
t('en fallback none: known codes US, EUR, gregory, en', function () { var o = { fallback: 'none' }; return [dn('en', Object.assign({ type: 'region' }, o)).of('US'), dn('en', Object.assign({ type: 'currency' }, o)).of('EUR'), dn('en', Object.assign({ type: 'calendar' }, o)).of('gregory'), dn('en', Object.assign({ type: 'language' }, o)).of('en')]; });
t('en dateTimeField short and narrow: year, dayPeriod', function () { return [dn('en', { type: 'dateTimeField', style: 'short' }).of('year'), dn('en', { type: 'dateTimeField', style: 'narrow' }).of('dayPeriod')]; });
// S9: the canonical code.
t('region case: us, Us, 419', function () { var f = dn('en', { type: 'region', fallback: 'code' }); var g = dn('xyz', { type: 'region' }); return [f.of('us'), f.of('Us'), g.of('us')]; });
t('script case: latn, HANS', function () { var g = dn('xyz', { type: 'script' }); return [dn('en', { type: 'script' }).of('latn'), g.of('latn'), g.of('HANS')]; });
t('calendar case: GREGORY, Islamic-Civil', function () { var g = dn('xyz', { type: 'calendar' }); return [dn('en', { type: 'calendar' }).of('GREGORY'), g.of('GREGORY'), g.of('Islamic-Civil')]; });
t('currency case: eur', function () { return [dn('en', { type: 'currency' }).of('eur'), dn('xyz', { type: 'currency' }).of('eur')]; });
t('language canonical: EN-us, iw, in, sh, und, en-latn-us, zh-hant-tw', function () { var g = dn('xyz', { type: 'language', fallback: 'code' }); return ['EN-us', 'iw', 'in', 'sh', 'und', 'en-latn-us', 'zh-hant-tw'].map(function (c) { try { return g.of(c); } catch (e) { return e.name; } }); });
t('calendar aliases: islamicc, ethiopic-amete-alem, gregorian', function () { var g = dn('en', { type: 'calendar' }); return ['islamicc', 'ethiopic-amete-alem', 'gregorian'].map(function (c) { try { return g.of(c); } catch (e) { return e.name; } }); });
var bad = {
  region: ['١٢٣', 'US\n', 'Uİ', '1٣', 'ＵS'],
  script: ['Latn\n', 'Lаtn', 'Ｌatn'],
  calendar: ['gregory\n', 'grеgory', 'abc_def'],
  currency: ['USD\n', 'ıSD'],
  language: ['en\n', 'en-US\n', 'de-١٢٣', 'en_US', 'root', 'en-u-ca-gregory', 'en-x-priv', 'i-klingon'],
  dateTimeField: ['year\n', 'Year', 'era '],
};
Object.keys(bad).forEach(function (ty) {
  t(ty + ' invalid codes', function () {
    var f = dn('en', { type: ty });
    return bad[ty].map(function (c) { try { return q(c) + ':' + q(String(f.of(c))); } catch (e) { return q(c) + ':' + e.name; } }).join(' ');
  });
});
t('of(Symbol()), of({toString}), of(undefined)', function () {
  var f = dn('en', { type: 'region' });
  return [function () { return f.of(Symbol()); }, function () { return f.of({ toString: function () { return 'DE'; } }); }, function () { return f.of(undefined); }].map(function (g) { try { return String(g()); } catch (e) { return e.name; } });
});
// S10: the constructor.
var log = [];
var d = dn('en', spy({ localeMatcher: 'lookup', style: 'short', type: 'language', fallback: 'none', languageDisplay: 'standard' }, log));
t('constructor reads', function () { return log; });
t('resolvedOptions: language type; region type', function () { return [Object.keys(d.resolvedOptions()).join(','), Object.keys(dn('en', { type: 'region' }).resolvedOptions()).join(',')]; });
t('no options, {}, options a string, null', function () { return [undefined, {}, 'region', null].map(function (o) { try { new Intl.DisplayNames('en', o); return 'ok'; } catch (e) { return e.name; } }); });
t('DisplayNames without new; length', function () { var r; try { Intl.DisplayNames('en', { type: 'region' }); r = 'ok'; } catch (e) { r = e.name; } return [r, Intl.DisplayNames.length]; });
t('typeof Intl.DisplayNames.supportedLocalesOf; result', function () { var f = Intl.DisplayNames.supportedLocalesOf; return [typeof f, typeof f === 'function' ? f(['de-AT', 'en', 'zz']) : null]; });
t('invalid style, type, fallback, languageDisplay', function () { return ['style', 'type', 'fallback', 'languageDisplay'].map(function (k) { var o = { type: 'region' }; o[k] = 'bogus'; try { new Intl.DisplayNames('en', o); return k + ':ok'; } catch (e) { return k + ':' + e.name; } }).join(' '); });
t('resolved locale: de-CH, fr, xyz', function () { return ['de-CH', 'fr', 'xyz'].map(function (l) { return dn(l, { type: 'region' }).resolvedOptions().locale; }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
