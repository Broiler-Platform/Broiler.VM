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
// F7: ZonedDateTime until and since across daylight-saving changes, and rounding to the day.
var NY = '[America/New_York]';
t('2026-03-08T00:00-05:00 until 2026-03-09T00:00-04:00', function () { return Z('2026-03-08T00:00-05:00' + NY).until('2026-03-09T00:00-04:00' + NY); });
t('2026-03-08T00:00-05:00 until 2026-03-09T00:00-04:00 largest day', function () { return Z('2026-03-08T00:00-05:00' + NY).until('2026-03-09T00:00-04:00' + NY, { largestUnit: 'day' }); });
t('2026-11-01T00:00-04:00 until 2026-11-02T00:00-05:00 largest day', function () { return Z('2026-11-01T00:00-04:00' + NY).until('2026-11-02T00:00-05:00' + NY, { largestUnit: 'day' }); });
t('2026-03-07T12:00-05:00 until 2026-03-07T23:45-05:00 smallest day halfExpand', function () { return Z('2026-03-07T12:00-05:00' + NY).until('2026-03-07T23:45-05:00' + NY, { smallestUnit: 'day', roundingMode: 'halfExpand' }); });
t('2026-03-07T23:45-05:00 since 2026-03-07T12:00-05:00 smallest day halfExpand', function () { return Z('2026-03-07T23:45-05:00' + NY).since('2026-03-07T12:00-05:00' + NY, { smallestUnit: 'day', roundingMode: 'halfExpand' }); });
t('2026-10-31T12:00-04:00 until 2026-11-01T00:15-04:00 smallest day halfExpand', function () { return Z('2026-10-31T12:00-04:00' + NY).until('2026-11-01T00:15-04:00' + NY, { smallestUnit: 'day', roundingMode: 'halfExpand' }); });
t('2026-03-07T00:00-05:00 until 2026-03-08T11:45-04:00 smallest day halfExpand', function () { return Z('2026-03-07T00:00-05:00' + NY).until('2026-03-08T11:45-04:00' + NY, { smallestUnit: 'day', roundingMode: 'halfExpand' }); });
t('2026-03-07T12:00-05:00 until 2026-03-07T23:45-05:00 smallest day largest month halfExpand', function () { return Z('2026-03-07T12:00-05:00' + NY).until('2026-03-07T23:45-05:00' + NY, { smallestUnit: 'day', largestUnit: 'month', roundingMode: 'halfExpand' }); });
t('2026-03-07T12:00-05:00 until 2026-03-08T12:30-04:00 hour/day halfExpand', function () { return Z('2026-03-07T12:00-05:00' + NY).until('2026-03-08T12:30-04:00' + NY, { largestUnit: 'day', smallestUnit: 'hour', roundingMode: 'halfExpand' }); });
t('2026-03-07T12:00-05:00 until 2026-03-08T11:30-04:00 hour/day halfExpand', function () { return Z('2026-03-07T12:00-05:00' + NY).until('2026-03-08T11:30-04:00' + NY, { largestUnit: 'day', smallestUnit: 'hour', roundingMode: 'halfExpand' }); });
t('2026-02-07T12:00-05:00 until 2026-03-07T23:45-05:00 smallest week largest month halfExpand', function () { return Z('2026-02-07T12:00-05:00' + NY).until('2026-03-14T23:45-04:00' + NY, { smallestUnit: 'week', largestUnit: 'month', roundingMode: 'halfExpand' }); });
t('2019-03-31T00:00[UTC] until 2020-02-29T12:00[UTC] month/year trunc', function () { return Z('2019-03-31T00:00[UTC]').until('2020-02-29T12:00[UTC]', { smallestUnit: 'month', largestUnit: 'year' }); });
t('2019-03-31T00:00[UTC] until 2020-02-29T12:00[UTC] year/year trunc', function () { return Z('2019-03-31T00:00[UTC]').until('2020-02-29T12:00[UTC]', { smallestUnit: 'year' }); });
t('2026-03-08T01:30-05:00 until 2026-03-08T03:30-04:00 largest day', function () { return Z('2026-03-08T01:30-05:00' + NY).until('2026-03-08T03:30-04:00' + NY, { largestUnit: 'day' }); });
t('2026-11-01T01:30-04:00 until 2026-11-01T01:30-05:00 largest day', function () { return Z('2026-11-01T01:30-04:00' + NY).until('2026-11-01T01:30-05:00' + NY, { largestUnit: 'day' }); });
t('2026-11-01T01:45-04:00 until 2026-11-02T01:15-05:00 largest day', function () { return Z('2026-11-01T01:45-04:00' + NY).until('2026-11-02T01:15-05:00' + NY, { largestUnit: 'day' }); });
t('2026-11-02T01:15-05:00 since 2026-11-01T01:45-04:00 largest day', function () { return Z('2026-11-02T01:15-05:00' + NY).since('2026-11-01T01:45-04:00' + NY, { largestUnit: 'day' }); });
t('ZonedDateTime.round smallest day, NY 2026-03-08T12:00-04:00', function () { return Z('2026-03-08T12:00-04:00' + NY).round({ smallestUnit: 'day' }); });
t('ZonedDateTime.round smallest day, NY 2026-03-08T11:29-04:00', function () { return Z('2026-03-08T11:29-04:00' + NY).round({ smallestUnit: 'day' }); });
t('ZonedDateTime.round smallest day, NY 2026-11-01T12:29-05:00', function () { return Z('2026-11-01T12:29-05:00' + NY).round({ smallestUnit: 'day' }); });
t('ZonedDateTime.hoursInDay NY 2026-03-08 and 2026-11-01', function () { return Z('2026-03-08T12:00-04:00' + NY).hoursInDay + ' ' + Z('2026-11-01T12:00-05:00' + NY).hoursInDay; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
