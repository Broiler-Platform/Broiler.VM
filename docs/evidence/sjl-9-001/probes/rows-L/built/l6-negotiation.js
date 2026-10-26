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
// SJL-9.1 sub-area (a), rows L21-L28: the available locales, the default locale, ResolveLocale with its
// relevant extension keys and the options that override them, SupportedLocales and FilterLocales, and
// TransformCase's locale (ECMA-402 9.1, 9.2.2-9.2.10, 6.2.4 DefaultLocale, 19.1.2 TransformCase).
var ctors = [
  ['Collator', function (l, o) { return new Intl.Collator(l, o); }],
  ['DateTimeFormat', function (l, o) { return new Intl.DateTimeFormat(l, o); }],
  ['DisplayNames', function (l, o) { var p = { type: 'region' }; for (var k in o) p[k] = o[k]; return new Intl.DisplayNames(l, p); }],
  ['DurationFormat', function (l, o) { return new Intl.DurationFormat(l, o); }],
  ['ListFormat', function (l, o) { return new Intl.ListFormat(l, o); }],
  ['NumberFormat', function (l, o) { return new Intl.NumberFormat(l, o); }],
  ['PluralRules', function (l, o) { return new Intl.PluralRules(l, o); }],
  ['RelativeTimeFormat', function (l, o) { return new Intl.RelativeTimeFormat(l, o); }],
  ['Segmenter', function (l, o) { return new Intl.Segmenter(l, o); }]
];
// The default locale and the resolution of a few requests, per service.
var requests = [undefined, [], 'en', 'en-US', 'de', 'de-AT', 'de-CH-1996', 'en-US-x-foo', 'en-GB', 'fr', 'ja-JP', 'xx', 'zh-Hant-TW',
  ['xx', 'de'], ['zz-ZZ', 'fr-FR', 'en'], 'sr', 'en-u-attr-ca-japanese-nu-arab-co-phonebk-kn-kf-upper-hc-h23-xx-yyy'];
ctors.forEach(function (c) {
  requests.forEach(function (r) {
    t(c[0] + ' resolved locale for ' + show(r), function () { return c[1](r, {}).resolvedOptions().locale; });
  });
});
// Keywords and options (ResolveLocale step for each relevant key).
t('NumberFormat en-u-nu-arab', function () { var o = new Intl.NumberFormat('en-u-nu-arab').resolvedOptions(); return [o.locale, o.numberingSystem]; });
t('NumberFormat en-u-nu-arab, numberingSystem latn', function () { var o = new Intl.NumberFormat('en-u-nu-arab', { numberingSystem: 'latn' }).resolvedOptions(); return [o.locale, o.numberingSystem]; });
t('NumberFormat en-u-nu-latn, numberingSystem latn', function () { var o = new Intl.NumberFormat('en-u-nu-latn', { numberingSystem: 'latn' }).resolvedOptions(); return [o.locale, o.numberingSystem]; });
t('NumberFormat en-u-nu-xxxx', function () { var o = new Intl.NumberFormat('en-u-nu-xxxx').resolvedOptions(); return [o.locale, o.numberingSystem]; });
t('NumberFormat en, numberingSystem LATN', function () { var o = new Intl.NumberFormat('en', { numberingSystem: 'LATN' }).resolvedOptions(); return [o.locale, o.numberingSystem]; });
t('NumberFormat en, numberingSystem Kelvin-hmr', function () { var o = new Intl.NumberFormat('en', { numberingSystem: 'Khmr' }).resolvedOptions(); return [o.locale, o.numberingSystem]; });
t('NumberFormat de-u-nu-thai', function () { var o = new Intl.NumberFormat('de-u-nu-thai').resolvedOptions(); return [o.locale, o.numberingSystem]; });
t('Collator de-u-co-phonebk-kn-kf-upper', function () { var o = new Intl.Collator('de-u-co-phonebk-kn-kf-upper').resolvedOptions(); return [o.locale, o.collation, o.numeric, o.caseFirst]; });
t('Collator de-u-co-phonebk, collation eor', function () { var o = new Intl.Collator('de-u-co-phonebk', { collation: 'eor' }).resolvedOptions(); return [o.locale, o.collation]; });
t('Collator de-u-co-phonebk, collation phonebk', function () { var o = new Intl.Collator('de-u-co-phonebk', { collation: 'phonebk' }).resolvedOptions(); return [o.locale, o.collation]; });
t('Collator en-u-co-phonebk', function () { var o = new Intl.Collator('en-u-co-phonebk').resolvedOptions(); return [o.locale, o.collation]; });
t('Collator de-u-kn-false, numeric true', function () { var o = new Intl.Collator('de-u-kn-false', { numeric: true }).resolvedOptions(); return [o.locale, o.numeric]; });
t('Collator de-u-kn, numeric true', function () { var o = new Intl.Collator('de-u-kn', { numeric: true }).resolvedOptions(); return [o.locale, o.numeric]; });
t('DateTimeFormat en-u-ca-japanese-hc-h23-nu-latn', function () { var o = new Intl.DateTimeFormat('en-u-ca-japanese-hc-h23-nu-latn').resolvedOptions(); return [o.locale, o.calendar, o.numberingSystem]; });
t('DateTimeFormat en-u-ca-japanese, calendar gregory', function () { var o = new Intl.DateTimeFormat('en-u-ca-japanese', { calendar: 'gregory' }).resolvedOptions(); return [o.locale, o.calendar]; });
t('DateTimeFormat en-u-ca-islamicc', function () { var o = new Intl.DateTimeFormat('en-u-ca-islamicc').resolvedOptions(); return [o.locale, o.calendar]; });
t('DateTimeFormat en-u-ca-islamic', function () { var o = new Intl.DateTimeFormat('en-u-ca-islamic').resolvedOptions(); return [o.locale, o.calendar]; });
t('RelativeTimeFormat en-u-nu-arab', function () { var o = new Intl.RelativeTimeFormat('en-u-nu-arab').resolvedOptions(); return [o.locale, o.numberingSystem]; });
t('PluralRules en-u-nu-arab', function () { return new Intl.PluralRules('en-u-nu-arab').resolvedOptions().locale; });
// localeMatcher.
t('NumberFormat localeMatcher lookup', function () { return new Intl.NumberFormat('de-AT', { localeMatcher: 'lookup' }).resolvedOptions().locale; });
t('NumberFormat localeMatcher invalid', function () { return new Intl.NumberFormat('en', { localeMatcher: 'fuzzy' }); });
t('NumberFormat localeMatcher read once and first', function () {
  var log = [];
  new Intl.NumberFormat('en', spy({ localeMatcher: 'lookup', numberingSystem: 'latn', style: 'decimal' }, log));
  return log.slice(0, 3).join(',');
});
// SupportedLocales, every service.
var lists = [['ja', 'zz', 'sw-KE', 'de-CH', 'en-US-u-ca-gregory', 'de-DE-x-foo', 'sr', 'en-GB', 'de-1996'], [], 'de', undefined];
ctors.forEach(function (c) {
  lists.forEach(function (l) {
    t(c[0] + '.supportedLocalesOf ' + show(l), function () { return Intl[c[0]].supportedLocalesOf(l); });
  });
});
t('typeof Intl.DisplayNames.supportedLocalesOf', function () { return typeof Intl.DisplayNames.supportedLocalesOf; });
t('supportedLocalesOf localeMatcher invalid', function () { return Intl.NumberFormat.supportedLocalesOf('en', { localeMatcher: 'fuzzy' }); });
t('supportedLocalesOf options null', function () { return Intl.NumberFormat.supportedLocalesOf('en', null); });
t('supportedLocalesOf options 5', function () { return Intl.NumberFormat.supportedLocalesOf('en', 5); });
t('supportedLocalesOf reads', function () {
  var log = [];
  Intl.NumberFormat.supportedLocalesOf(['en'], spy({ localeMatcher: 'lookup' }, log));
  return log.join(',');
});
t('supportedLocalesOf result', function () { var r = Intl.Collator.supportedLocalesOf(['en']); return [Array.isArray(r), Object.isFrozen(r), Object.isExtensible(r)]; });
// TransformCase's locale (String.prototype.toLocaleLowerCase and toLocaleUpperCase).
[undefined, 'en', 'tr', 'TR', 'az', 'lt', 'tr-u-co-trad', 'tr-x-foo', ['tr', 'en'], ['en', 'tr'], 'xx', 'de']
  .forEach(function (l) {
    t('"Iİi".toLocaleLowerCase(' + show(l) + ')', function () { return 'Iİi'.toLocaleLowerCase(l); });
    t('"iı".toLocaleUpperCase(' + show(l) + ')', function () { return 'iı'.toLocaleUpperCase(l); });
  });
t('"İ".toLocaleLowerCase("lt")', function () { return 'İ'.toLocaleLowerCase('lt'); });
t('"i̇".toLocaleUpperCase("lt")', function () { return 'i̇'.toLocaleUpperCase('lt'); });
t('toLocaleLowerCase("tr_TR")', function () { return 'I'.toLocaleLowerCase('tr_TR'); });
t('toLocaleLowerCase(null)', function () { return 'I'.toLocaleLowerCase(null); });
t('toLocaleLowerCase([5])', function () { return 'I'.toLocaleLowerCase([5]); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
