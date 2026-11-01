// SJL-17.1 (bundle sjl-17-001): bundle sjl-9-001's probes/prelude.js, unchanged from the next line on.
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
// O9 to O13: the objects - option reading order, refusals, resolvedOptions, the bound compare, localeCompare's routing, collations.
function sgn(n) { return n < 0 ? '<' : n > 0 ? '>' : '='; }
function ro(c) { var r = c.resolvedOptions(); return show(r); }
t('o9 read order', function () { var log = []; new Intl.Collator('en', spy({ usage: 'sort', localeMatcher: 'lookup', collation: 'phonebk', numeric: true, caseFirst: 'upper', sensitivity: 'base', ignorePunctuation: true }, log)); return log.join(','); });
t('o9 read order localeCompare', function () { var log = []; 'a'.localeCompare('b', 'en', spy({ usage: 'sort', sensitivity: 'base' }, log)); return log.join(','); });
t('o9 collation not a type', function () { return ro(new Intl.Collator('en', { collation: 'a' })); });
t('o9 collation not a type 2', function () { return ro(new Intl.Collator('en', { collation: 'abc-' })); });
t('o9 sensitivity bad', function () { return ro(new Intl.Collator('en', { sensitivity: 'x' })); });
t('o9 usage bad', function () { return ro(new Intl.Collator('en', { usage: 'x' })); });
t('o9 caseFirst bad', function () { return ro(new Intl.Collator('en', { caseFirst: 'x' })); });
t('o9 localeMatcher bad', function () { return ro(new Intl.Collator('en', { localeMatcher: 'x' })); });
t('o9 numeric string', function () { return ro(new Intl.Collator('en', { numeric: 'false' })); });
t('o10 resolved de-u-co-phonebk-kn-kf-upper', function () { return ro(new Intl.Collator('de-u-co-phonebk-kn-kf-upper')); });
t('o10 resolved en-u-co-phonebk', function () { return ro(new Intl.Collator('en-u-co-phonebk')); });
t('o10 resolved en emoji', function () { return ro(new Intl.Collator('en', { collation: 'emoji' })); });
t('o10 resolved en eor', function () { return ro(new Intl.Collator('en-u-co-eor')); });
t('o10 resolved de search co', function () { return ro(new Intl.Collator('de-u-co-search')); });
t('o10 resolved de-u-co-standard', function () { return ro(new Intl.Collator('de-u-co-standard')); });
t('o10 resolved zh pinyin', function () { return ro(new Intl.Collator('zh-u-co-pinyin')); });
t('o10 resolved de phonebk usage search', function () { return ro(new Intl.Collator('de-u-co-phonebk', { usage: 'search' })); });
t('o10 resolved xx', function () { return ro(new Intl.Collator('xx')); });
t('o10 resolved default', function () { return ro(new Intl.Collator()); });
t('o10 supportedValuesOf collation', function () { return typeof Intl.supportedValuesOf === 'function' ? Intl.supportedValuesOf('collation') : 'absent'; });
t('o10 getCollations en de und zh', function () { return typeof Intl.Locale === 'function' ? ['en', 'de', 'und', 'zh', 'sv'].map(function (l) { return l + ':' + new Intl.Locale(l).getCollations().join('|'); }) : 'absent'; });
t('o11 bound compare identity', function () { var c = new Intl.Collator('en'); return c.compare === c.compare; });
t('o11 bound compare name length', function () { var f = new Intl.Collator('en').compare; return q(f.name) + ' ' + f.length + ' ' + Object.getPrototypeOf(f).constructor.name + ' ' + ('prototype' in f); });
t('o11 compare detached', function () { var f = new Intl.Collator('en').compare; return R(sgn(f('a', 'b')) + ' ' + ['c', 'b', 'a'].sort(f).join('')); });
t('o11 compare coerces', function () { return R(sgn(new Intl.Collator('en').compare(10, 9)) + ' ' + sgn(new Intl.Collator('en').compare()) + ' ' + sgn(new Intl.Collator('en').compare(undefined, 'undefined'))); });
t('o11 compare getter on non-collator', function () { return Object.getOwnPropertyDescriptor(Intl.Collator.prototype, 'compare').get.call({}); });
t('o12 localeCompare after Intl.Collator replaced', function () { var saved = Intl.Collator; Intl.Collator = function () { throw new Error('replaced'); }; try { return R(sgn('a'.localeCompare('b')) + ' ' + sgn('ä'.localeCompare('af', 'de-u-co-phonebk'))); } finally { Intl.Collator = saved; } });
t('o12 localeCompare refusals', function () { return 'a'.localeCompare('b', 'en', { sensitivity: 'x' }); });
t('o12 localeCompare bad tag', function () { return 'a'.localeCompare('b', 'en_US'); });
t('o12 localeCompare a B', function () { return R(sgn('a'.localeCompare('B')) + ' ' + sgn('B'.localeCompare('a')) + ' ' + sgn('ä'.localeCompare('b'))); });
t('o12 Collator call without new', function () { var c = Intl.Collator('de'); return R((c instanceof Intl.Collator) + ' ' + c.resolvedOptions().locale); });
t('o13 default locale and its collation', function () { var c = new Intl.Collator(); return R(c.resolvedOptions().locale + ' ' + sgn(c.compare('a', 'B')) + ' ' + sgn(c.compare('i', 'ı')) + ' ' + sgn(c.compare('ä', 'b'))); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
