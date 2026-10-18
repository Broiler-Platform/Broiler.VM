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
// D8: the nudge after a shifted window. From 2019-03-31 the destination 2020-02-29T12:00 lies past the first month
// window (2020-01-31 to the clamped 2020-02-29) and inside the second (2020-02-29 to 2020-03-31); rounded down to
// 11 months, does the nudged instant bubble into a year?
var d = { months: 10, days: 29, hours: 12 };
t('Duration round month/year trunc, relativeTo "2019-03-31"', function () { return D(d).round({ smallestUnit: 'month', largestUnit: 'year', roundingMode: 'trunc', relativeTo: '2019-03-31' }); });
t('Duration round month/year trunc, relativeTo "2019-03-31T00:00[UTC]"', function () { return D(d).round({ smallestUnit: 'month', largestUnit: 'year', roundingMode: 'trunc', relativeTo: '2019-03-31T00:00[UTC]' }); });
t('Duration round month/year trunc, relativeTo ZonedDateTime 2019-03-31[UTC]', function () { return D(d).round({ smallestUnit: 'month', largestUnit: 'year', roundingMode: 'trunc', relativeTo: Z('2019-03-31T00:00[UTC]') }); });
t('PlainDateTime 2019-03-31T00:00 until 2020-02-29T12:00 month/year trunc', function () { return PDT('2019-03-31T00:00').until('2020-02-29T12:00', { smallestUnit: 'month', largestUnit: 'year' }); });
t('ZonedDateTime 2019-03-31T00:00[UTC] until 2020-02-29T12:00[UTC] month/year trunc', function () { return Z('2019-03-31T00:00[UTC]').until('2020-02-29T12:00[UTC]', { smallestUnit: 'month', largestUnit: 'year' }); });
t('Duration total month, relativeTo "2019-03-31"', function () { return D(d).total({ unit: 'month', relativeTo: '2019-03-31' }); });
t('2019-03-31 + P1Y', function () { return PD('2019-03-31').add({ years: 1 }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
