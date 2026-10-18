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
// F2: PlainDate until and since, every largest unit, the month-end anchoring of the receiver.
t('2020-01-31 until 2020-03-01 month', function () { return PD('2020-01-31').until('2020-03-01', { largestUnit: 'month' }); });
t('2020-03-01 since 2020-01-31 month', function () { return PD('2020-03-01').since('2020-01-31', { largestUnit: 'month' }); });
t('2020-01-31 since 2020-03-01 month', function () { return PD('2020-01-31').since('2020-03-01', { largestUnit: 'month' }); });
t('2020-03-01 until 2020-01-31 month', function () { return PD('2020-03-01').until('2020-01-31', { largestUnit: 'month' }); });
t('2020-01-01 until 2020-03-01 week', function () { return PD('2020-01-01').until('2020-03-01', { largestUnit: 'week' }); });
t('2019-02-28 until 2020-02-29 year', function () { return PD('2019-02-28').until('2020-02-29', { largestUnit: 'year' }); });
t('2020-02-29 until 2021-02-28 year', function () { return PD('2020-02-29').until('2021-02-28', { largestUnit: 'year' }); });
t('2021-02-28 until 2020-02-29 year', function () { return PD('2021-02-28').until('2020-02-29', { largestUnit: 'year' }); });
t('2020-01-29 until 2020-02-28 month', function () { return PD('2020-01-29').until('2020-02-28', { largestUnit: 'month' }); });
t('2019-01-29 until 2019-03-01 month', function () { return PD('2019-01-29').until('2019-03-01', { largestUnit: 'month' }); });
t('-271821-04-19 until +275760-09-13 year', function () { return PD('-271821-04-19').until('+275760-09-13', { largestUnit: 'year' }); });
t('-271821-04-19 until +275760-09-13 day', function () { return PD('-271821-04-19').until('+275760-09-13'); });
t('+275760-09-13 since -271821-04-19 week', function () { return PD('+275760-09-13').since('-271821-04-19', { largestUnit: 'week' }); });
t('2020-01-01 until 2020-01-01', function () { return PD('2020-01-01').until('2020-01-01', { largestUnit: 'year' }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
