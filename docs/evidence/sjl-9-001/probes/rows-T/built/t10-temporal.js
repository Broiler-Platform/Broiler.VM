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
// SJL-9.1 sub-area (d), rows T28-T31: Intl over Temporal. DateTimeFormat's format, formatToParts and
// formatRange of each Temporal type (the formats the constructor keeps per type, GetDateTimeFormat's
// required/defaults/inherit, AdjustDateTimeStyleFormat), the calendar checks, the ZonedDateTime refusal,
// the seven toLocaleString methods, and change 377fb80e (a plain value is written at +00:00, so no zone
// moves it). tc39/proposal-temporal@e8cc03fc spec/intl.html: CreateDateTimeFormat, GetDateTimeFormat,
// AdjustDateTimeStyleFormat, HandleDateTimeValue and the HandleDateTimeTemporal* operations,
// FormatDateTimePattern (isPlain), and the toLocaleString amendments.
var T = Temporal;
var pd = T.PlainDate.from('2024-03-10'), pt = T.PlainTime.from('02:30:15.123'), pdt = T.PlainDateTime.from('2024-03-10T02:30:15'),
  pym = T.PlainYearMonth.from('2024-03'), pmd = T.PlainMonthDay.from('03-10'), ins = T.Instant.from('2024-03-10T07:30:15Z'),
  zdt = T.ZonedDateTime.from('2024-03-10T03:30:15-04:00[America/New_York]');
var vals = [['PlainDate', pd], ['PlainTime', pt], ['PlainDateTime', pdt], ['PlainYearMonth', pym], ['PlainMonthDay', pmd], ['Instant', ins], ['ZonedDateTime', zdt]];
var bags = [{}, { year: 'numeric' }, { hour: 'numeric' }, { month: 'long', day: 'numeric' }, { minute: 'numeric' }, { era: 'short' }, { timeZoneName: 'short' },
  { dateStyle: 'medium' }, { timeStyle: 'short' }, { dateStyle: 'full', timeStyle: 'long' }, { weekday: 'long' }];
bags.forEach(function (b) {
  var o = { timeZone: 'America/New_York' }; for (var k in b) o[k] = b[k];
  t('en NY ' + show(b) + ' format of each type', function () {
    var f = new Intl.DateTimeFormat('en', o);
    return R(vals.map(function (v) { try { return v[0] + ' ' + q(f.format(v[1])); } catch (e) { return v[0] + ' THROWS ' + errName(e); } }).join('; '));
  });
});
t('en NY formatToParts of PlainDateTime (377fb80e: no zone shift in the DST gap)', function () { return parts(new Intl.DateTimeFormat('en', { timeZone: 'America/New_York', hour: 'numeric', minute: 'numeric', timeZoneName: 'short' }).formatToParts(pdt)); });
t('en Asia/Kolkata PlainTime 02:30 and Instant', function () { var f = new Intl.DateTimeFormat('en', { timeZone: 'Asia/Kolkata', timeStyle: 'long' }); return [f.format(pt), f.format(ins)]; });
t('en formatRange PlainDate-PlainDate', function () { return new Intl.DateTimeFormat('en').formatRange(pd, T.PlainDate.from('2024-03-12')); });
t('en formatRange PlainDate-PlainDateTime', function () { return new Intl.DateTimeFormat('en').formatRange(pd, pdt); });
t('en formatRange PlainDate-Number', function () { return new Intl.DateTimeFormat('en').formatRange(pd, 0); });
t('en formatRange Instant-Instant', function () { return new Intl.DateTimeFormat('en', { timeZone: 'UTC' }).formatRange(ins, ins.add({ hours: 2 })); });
t('en formatRangeToParts PlainTime-PlainTime', function () { return parts(new Intl.DateTimeFormat('en').formatRangeToParts(pt, T.PlainTime.from('05:00'))); });
// Calendars.
t('en-u-ca-japanese PlainDate (iso8601)', function () { return new Intl.DateTimeFormat('en-u-ca-japanese', { dateStyle: 'long' }).format(pd); });
t('en-u-ca-japanese PlainDate (japanese)', function () { return new Intl.DateTimeFormat('en-u-ca-japanese', { dateStyle: 'long' }).format(pd.withCalendar('japanese')); });
t('en PlainDate in hebrew calendar', function () { return new Intl.DateTimeFormat('en').format(pd.withCalendar('hebrew')); });
t('en PlainYearMonth iso8601 (gregory formatter)', function () { return new Intl.DateTimeFormat('en').format(pym); });
t('en-u-ca-iso8601 PlainYearMonth', function () { return new Intl.DateTimeFormat('en-u-ca-iso8601').format(pym); });
t('en-u-ca-iso8601 PlainMonthDay', function () { return new Intl.DateTimeFormat('en-u-ca-iso8601').format(pmd); });
t('en-u-ca-gregory PlainYearMonth in gregory', function () { return new Intl.DateTimeFormat('en-u-ca-gregory').format(T.PlainYearMonth.from({ year: 2024, month: 3, calendar: 'gregory' })); });
// toLocaleString.
vals.forEach(function (v) {
  t(v[0] + '#toLocaleString("en")', function () { return v[1].toLocaleString('en'); });
  t(v[0] + '#toLocaleString("de", {dateStyle:"long"})', function () { return v[1].toLocaleString('de', { dateStyle: 'long' }); });
  t(v[0] + '#toLocaleString("en", {timeStyle:"short"})', function () { return v[1].toLocaleString('en', { timeStyle: 'short' }); });
  t(v[0] + '#toLocaleString("en", {hour:"numeric"})', function () { return v[1].toLocaleString('en', { hour: 'numeric' }); });
  t(v[0] + '#toLocaleString("en", {year:"numeric"})', function () { return v[1].toLocaleString('en', { year: 'numeric' }); });
});
t('ZonedDateTime#toLocaleString with a timeZone option', function () { return zdt.toLocaleString('en', { timeZone: 'America/New_York' }); });
t('ZonedDateTime#toLocaleString in Asia/Calcutta', function () { return T.ZonedDateTime.from('2024-03-10T12:00[Asia/Calcutta]').toLocaleString('en', { timeZoneName: 'long' }); });
t('ZonedDateTime#toLocaleString resolvedOptions-like zone (Asia/Calcutta) in parts', function () { return T.ZonedDateTime.from('2024-03-10T12:00[Asia/Calcutta]').toLocaleString('en'); });
t('ZonedDateTime in japanese calendar, en-u-ca-japanese', function () { return zdt.withCalendar('japanese').toLocaleString('en-u-ca-japanese', { dateStyle: 'long' }); });
t('ZonedDateTime in japanese calendar, en', function () { return zdt.withCalendar('japanese').toLocaleString('en', { dateStyle: 'long' }); });
t('Instant#toLocaleString default in UTC host', function () { return ins.toLocaleString('en', { timeZone: 'UTC' }); });
t('PlainDate#toLocaleString reads', function () { var log = []; pd.toLocaleString('en', spy({ year: 'numeric', timeZone: 'UTC' }, log)); return R(log.join(',')); });
t('ZonedDateTime#toLocaleString reads', function () { var log = []; zdt.toLocaleString('en', spy({ year: 'numeric' }, log)); return R(log.join(',')); });
t('Duration#toLocaleString (DurationFormat, sub-area e)', function () { return typeof T.Duration.from('PT1H').toLocaleString('en'); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
