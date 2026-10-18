// Shared prelude (SJL-8's form): Broiler.JS's shell has print; Broiler.VM's test262 realm has none, so
// the lines are collected and handed back in one uncaught Error the conformance root prints.
var __lines = [];
var __host = typeof __vmTest === 'undefined' && typeof print === 'function';
function out(s) { s = String(s); if (__host) print(s); else __lines.push(s); }
function show(v) {
  if (v === null) return 'null';
  if (v === undefined) return 'undefined';
  if (typeof v === 'bigint') return v + 'n';
  if (typeof v === 'string') return v;
  if (typeof v === 'number') return Object.is(v, -0) ? '-0' : String(v);
  if (Array.isArray(v)) return '[' + v.map(show).join(', ') + ']';
  if (v && typeof v === 'object' && typeof v.toString === 'function' && v[Symbol.toStringTag]) return v[Symbol.toStringTag] + ' ' + v.toString();
  return String(v);
}
function t(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + ((e && e.constructor && e.constructor.name) || typeof e) + (e && e.name && e.constructor && e.name !== e.constructor.name ? '(' + e.name + ')' : '') + ': ' + (e && e.message); }
  out(label + ' => ' + r);
}
var T = Temporal;
function D(x) { return T.Duration.from(x); }
function PD(x) { return T.PlainDate.from(x); }
function PDT(x) { return T.PlainDateTime.from(x); }
function PT(x) { return T.PlainTime.from(x); }
function PYM(x) { return T.PlainYearMonth.from(x); }
function I(x) { return T.Instant.from(x); }
function Z(x) { return T.ZonedDateTime.from(x); }
// F4: differences in a calendar other than ISO 8601, with rounding options.
t('hebrew PlainDate 2020-01-01 until 2020-07-20 smallest month halfExpand', function () { return PD('2020-01-01[u-ca=hebrew]').until('2020-07-20[u-ca=hebrew]', { smallestUnit: 'month', roundingMode: 'halfExpand' }); });
t('hebrew PlainDate 2020-01-01 until 2020-07-20 largest month (no rounding)', function () { return PD('2020-01-01[u-ca=hebrew]').until('2020-07-20[u-ca=hebrew]', { largestUnit: 'month' }); });
t('hebrew PlainDate 2020-01-01 until 2021-10-20 smallest year halfExpand', function () { return PD('2020-01-01[u-ca=hebrew]').until('2021-10-20[u-ca=hebrew]', { smallestUnit: 'year', roundingMode: 'halfExpand' }); });
t('hebrew PlainDate 2020-01-01 since 2020-07-20 smallest month halfExpand', function () { return PD('2020-01-01[u-ca=hebrew]').since('2020-07-20[u-ca=hebrew]', { smallestUnit: 'month', roundingMode: 'halfExpand' }); });
t('coptic PlainDate 2020-01-01 until 2020-03-20 smallest week ceil', function () { return PD('2020-01-01[u-ca=coptic]').until('2020-03-20[u-ca=coptic]', { smallestUnit: 'week', roundingMode: 'ceil' }); });
t('islamic-civil PlainDate 2020-01-01 until 2020-03-20 smallest day increment 7', function () { return PD('2020-01-01[u-ca=islamic-civil]').until('2020-03-20[u-ca=islamic-civil]', { smallestUnit: 'day', roundingIncrement: 7, roundingMode: 'halfExpand' }); });
t('gregory PlainDate 2020-01-01 until 2020-07-20 smallest month halfExpand', function () { return PD('2020-01-01[u-ca=gregory]').until('2020-07-20[u-ca=gregory]', { smallestUnit: 'month', roundingMode: 'halfExpand' }); });
t('hebrew PlainDateTime 2020-01-01T00:00 until 2020-01-03T13:00 smallest day halfExpand', function () { return PDT('2020-01-01T00:00[u-ca=hebrew]').until('2020-01-03T13:00[u-ca=hebrew]', { smallestUnit: 'day', roundingMode: 'halfExpand' }); });
t('hebrew PlainDateTime 2020-01-01T00:00 until 2020-07-20T00:00 smallest month halfExpand', function () { return PDT('2020-01-01T00:00[u-ca=hebrew]').until('2020-07-20T00:00[u-ca=hebrew]', { smallestUnit: 'month', roundingMode: 'halfExpand' }); });
t('hebrew PlainDateTime 2020-01-01T00:00 until 2020-01-03T13:20 largest day smallest hour halfExpand', function () { return PDT('2020-01-01T00:00[u-ca=hebrew]').until('2020-01-03T13:20[u-ca=hebrew]', { largestUnit: 'day', smallestUnit: 'hour', roundingMode: 'halfExpand' }); });
t('hebrew ZonedDateTime until +2 d 13 h smallest day halfExpand', function () { return Z('2020-01-01T00:00[UTC][u-ca=hebrew]').until('2020-01-03T13:00[UTC][u-ca=hebrew]', { smallestUnit: 'day', roundingMode: 'halfExpand' }); });
t('hebrew ZonedDateTime until 2020-07-20 largest month smallest month halfExpand', function () { return Z('2020-01-01T00:00[UTC][u-ca=hebrew]').until('2020-07-20T00:00[UTC][u-ca=hebrew]', { largestUnit: 'month', smallestUnit: 'month', roundingMode: 'halfExpand' }); });
t('hebrew ZonedDateTime until +2 d 13 h 20 min largest day smallest hour halfExpand', function () { return Z('2020-01-01T00:00[UTC][u-ca=hebrew]').until('2020-01-03T13:20[UTC][u-ca=hebrew]', { largestUnit: 'day', smallestUnit: 'hour', roundingMode: 'halfExpand' }); });
t('hebrew PlainYearMonth 5780-04 until 5781-03 smallest year halfExpand', function () { return T.PlainYearMonth.from({ year: 5780, monthCode: 'M04', calendar: 'hebrew' }).until(T.PlainYearMonth.from({ year: 5781, monthCode: 'M03', calendar: 'hebrew' }), { smallestUnit: 'year', roundingMode: 'halfExpand' }); });
t('hebrew PlainYearMonth 5780-04 until 5781-03 (no rounding)', function () { return T.PlainYearMonth.from({ year: 5780, monthCode: 'M04', calendar: 'hebrew' }).until(T.PlainYearMonth.from({ year: 5781, monthCode: 'M03', calendar: 'hebrew' })); });
t('hebrew PlainDateTime 2020-01-01T00:00 until 2020-01-16T00:00 smallest month halfExpand', function () { return PDT('2020-01-01T00:00[u-ca=hebrew]').until('2020-01-16T00:00[u-ca=hebrew]', { smallestUnit: 'month', roundingMode: 'halfExpand' }); });
t('hebrew PlainDate 2020-01-01 until 2020-01-16 smallest month halfExpand', function () { return PD('2020-01-01[u-ca=hebrew]').until('2020-01-16[u-ca=hebrew]', { smallestUnit: 'month', roundingMode: 'halfExpand' }); });
t('hebrew PlainDateTime 2020-01-01T00:00 until 2020-01-16T00:00 smallest month largest year halfExpand', function () { return PDT('2020-01-01T00:00[u-ca=hebrew]').until('2020-01-16T00:00[u-ca=hebrew]', { smallestUnit: 'month', largestUnit: 'year', roundingMode: 'halfExpand' }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
