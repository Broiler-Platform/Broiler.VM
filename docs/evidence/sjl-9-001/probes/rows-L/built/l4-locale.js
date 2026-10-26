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
// SJL-9.1 sub-area (a), rows L10-L13: the Intl.Locale constructor (ECMA-402 15.1.1, UpdateLanguageId 15.1.2,
// MakeLocaleRecord 15.1.3), its getters and its options' reads and checks.
function info(l) {
  return R('{toString: ' + q(l.toString()) + ', baseName: ' + q(l.baseName) + ', language: ' + q(l.language) +
    ', script: ' + show(l.script) + ', region: ' + show(l.region) + ', variants: ' + show(l.variants) +
    ', calendar: ' + show(l.calendar) + ', caseFirst: ' + show(l.caseFirst) + ', collation: ' + show(l.collation) +
    ', firstDayOfWeek: ' + show(l.firstDayOfWeek) + ', hourCycle: ' + show(l.hourCycle) +
    ', numberingSystem: ' + show(l.numberingSystem) + ', numeric: ' + show(l.numeric) + '}');
}
function L(label, tag, options) { t(label + ' ' + q(tag) + (options === undefined ? '' : ' ' + show(options)), function () { return info(new Intl.Locale(tag, options)); }); }

L('getters', 'en-latn-us-u-ca-gregory-co-phonebk-hc-h12-kf-upper-kn-nu-latn-fw-mon');
L('getters', 'en-u-kn-false');
L('getters', 'en-u-kn-true');
L('getters', 'en-u-kf');
L('getters', 'de-DE-1996-fonipa');
L('getters', 'en-x-foo');
L('getters', 'und');
L('getters', 'en-u-ca-islamicc-co-dict');
L('options', 'en-US-u-ca-gregory', { language: 'fr', script: 'cyrl', region: 'ca', variants: 'FONIPA-1996', calendar: 'Buddhist', collation: 'EMOJI', hourCycle: 'h23', caseFirst: 'lower', numeric: false, numberingSystem: 'Arab', firstDayOfWeek: '3' });
L('options', 'en', { calendar: 'islamicc' });
L('options', 'en-u-ca-gregory', { calendar: 'japanese' });
L('options', 'en-u-kn-true', { numeric: false });
L('options', 'en', { numeric: 'yes' });
L('options', 'en', { numeric: 0 });
L('options', 'en', { firstDayOfWeek: '0' });
L('options', 'en', { firstDayOfWeek: '7' });
L('options', 'en', { firstDayOfWeek: 'MON' });
L('options', 'en', { firstDayOfWeek: 'monday' });
L('options', 'en', { region: '419' });
L('options', 'cel', { variants: 'gaulish' });
L('options', 'en', { language: 'iw' });
L('options', 'en-US', { region: 'SU' });
L('options', 'en', { variants: 'hepburn-heploc' });
L('options', 'en-u-ca-gregory', { calendar: undefined });
// Option values that must be refused.
L('refused', 'en', { language: '' });
L('refused', 'en', { language: 'a' });
L('refused', 'en', { language: 'abcd' });
L('refused', 'en', { language: 'en-US' });
L('refused', 'en', { script: 'Lat' });
L('refused', 'en', { region: 'US1' });
L('refused', 'en', { variants: '' });
L('refused', 'en', { variants: 'aaaa' });
L('refused', 'en', { variants: 'fonipa-fonipa' });
L('refused', 'en', { variants: 'fonipa-' });
L('refused', 'en', { calendar: 'ab' });
L('refused', 'en', { calendar: 'gregory-' });
L('refused', 'en', { collation: 'phonebook' });
L('refused', 'en', { hourCycle: 'h25' });
L('refused', 'en', { caseFirst: 'UPPER' });
L('refused', 'en', { numberingSystem: 'latn_' });
L('refused', 'en', { firstDayOfWeek: '8' });
L('refused', 'en', { firstDayOfWeek: '' });
// Non-ASCII option values (U+212A KELVIN SIGN lowers to "k" under the platform's casing; U+0130 to "i").
L('non-ASCII', 'en', { variants: 'Kaaaa' });
L('non-ASCII', 'en', { numberingSystem: 'Khmr' });
L('non-ASCII', 'en', { calendar: 'bİddhist' });
L('non-ASCII', 'en', { collation: 'phoKebk' });
L('non-ASCII', 'en', { language: 'Ko' });
L('non-ASCII', 'en', { region: 'KR' });
L('non-ASCII', 'en', { firstDayOfWeek: 'moK' });
// The constructor's other steps.
t('called without new', function () { return Intl.Locale('en'); });
t('tag 5', function () { return new Intl.Locale(5); });
t('tag undefined', function () { return new Intl.Locale(); });
t('tag null', function () { return new Intl.Locale(null); });
t('tag a Locale, with options', function () { return info(new Intl.Locale(new Intl.Locale('en-u-ca-gregory'), { region: 'GB' })); });
t('tag an object with toString', function () { return info(new Intl.Locale({ toString: function () { return 'DE-at'; } })); });
t('options null', function () { return new Intl.Locale('en', null); });
t('options 5', function () { return info(new Intl.Locale('en', 5)); });
t('option reads, in order', function () {
  var log = [];
  new Intl.Locale('en', spy({ language: 'fr', script: 'Latn', region: 'FR', variants: '1996', calendar: 'gregory', collation: 'emoji', hourCycle: 'h23', caseFirst: 'upper', numeric: true, numberingSystem: 'latn', firstDayOfWeek: 'mon' }, log));
  return log.join(',');
});
t('option reads stop at the first bad value', function () {
  var log = [];
  try { new Intl.Locale('en', spy({ language: 'fr', script: 'L', region: 'FR' }, log)); } catch (e) { log.push(errName(e)); }
  return log.join(',');
});
t('tag read before options are coerced', function () {
  var log = [];
  try { new Intl.Locale({ toString: function () { log.push('tag'); return 'en'; } }, { get language() { log.push('language'); return undefined; } }); } catch (e) { log.push(errName(e)); }
  return log.join(',');
});
t('getters on Intl.Locale.prototype', function () {
  var d = Object.getOwnPropertyDescriptor(Intl.Locale.prototype, 'baseName');
  return [typeof d.get, d.set, d.enumerable, d.configurable, d.get.name, Intl.Locale.length, Intl.Locale.name];
});
t('Object.prototype.toString of a Locale', function () { return Object.prototype.toString.call(new Intl.Locale('en')); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
