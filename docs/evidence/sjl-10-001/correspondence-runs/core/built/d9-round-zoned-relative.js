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
// D9: Duration.prototype.round with a zoned relativeTo, across daylight-saving changes.
var spring = '2026-03-08T00:00-05:00[America/New_York]', fall = '2026-11-01T00:00-04:00[America/New_York]';
t('{hours: 23} round smallest day, NY spring', function () { return D({ hours: 23 }).round({ smallestUnit: 'day', relativeTo: spring }); });
t('{hours: 24} round largest day, NY spring', function () { return D({ hours: 24 }).round({ largestUnit: 'day', relativeTo: spring }); });
t('{days: 1} round largest hour, NY spring', function () { return D({ days: 1 }).round({ largestUnit: 'hour', relativeTo: spring }); });
t('{hours: 25} round largest day, NY fall', function () { return D({ hours: 25 }).round({ largestUnit: 'day', relativeTo: fall }); });
t('{hours: 12} round smallest day, NY spring', function () { return D({ hours: 12 }).round({ smallestUnit: 'day', relativeTo: spring }); });
t('{hours: 12} round smallest day, NY fall', function () { return D({ hours: 12 }).round({ smallestUnit: 'day', relativeTo: fall }); });
t('{hours: 11, minutes: 45} round smallest day, NY 2026-03-07T12:00', function () { return D({ hours: 11, minutes: 45 }).round({ smallestUnit: 'day', relativeTo: '2026-03-07T12:00-05:00[America/New_York]' }); });
t('{hours: 12, minutes: 15} round smallest day, NY 2026-10-31T12:00', function () { return D({ hours: 12, minutes: 15 }).round({ smallestUnit: 'day', relativeTo: '2026-10-31T12:00-04:00[America/New_York]' }); });
t('{months: 10, days: 29, hours: 12} round month/year trunc, relativeTo 2019-03-31T00:00[UTC]', function () { return D({ months: 10, days: 29, hours: 12 }).round({ smallestUnit: 'month', largestUnit: 'year', roundingMode: 'trunc', relativeTo: '2019-03-31T00:00[UTC]' }); });
t('{months: 10, days: 29, hours: 12} round month/year trunc, relativeTo ZonedDateTime 2019-03-31[UTC]', function () { return D({ months: 10, days: 29, hours: 12 }).round({ smallestUnit: 'month', largestUnit: 'year', roundingMode: 'trunc', relativeTo: Z('2019-03-31T00:00[UTC]') }); });
t('{days: 45} round largest month, relativeTo 2020-01-31T00:00[UTC]', function () { return D({ days: 45 }).round({ largestUnit: 'month', relativeTo: '2020-01-31T00:00[UTC]' }); });
t('{hours: 30} round smallest hour largest day, NY spring', function () { return D({ hours: 30 }).round({ smallestUnit: 'hour', largestUnit: 'day', relativeTo: spring }); });
t('{hours: 22, minutes: 40} round smallest hour increment 2 largest day, NY spring', function () { return D({ hours: 22, minutes: 40 }).round({ smallestUnit: 'hour', roundingIncrement: 2, largestUnit: 'day', relativeTo: spring }); });
t('{days: 1, hours: 1} round smallest day largest year, NY spring halfExpand', function () { return D({ days: 1, hours: 1 }).round({ smallestUnit: 'day', largestUnit: 'year', relativeTo: spring }); });
t('{days: 1} round smallest hour, relativeTo +275760-09-12T00:00[UTC]', function () { return D({ days: 1 }).round({ smallestUnit: 'hour', relativeTo: '+275760-09-12T00:00[UTC]' }); });
t('{hours: 1} round smallest hour largest day, relativeTo +275760-09-12T12:00[UTC]', function () { return D({ hours: 1 }).round({ smallestUnit: 'hour', largestUnit: 'day', relativeTo: '+275760-09-12T12:00[UTC]' }); });
t('{} round largest year, relativeTo +275760-09-13T00:00[UTC]', function () { return new T.Duration().round({ largestUnit: 'year', relativeTo: '+275760-09-13T00:00[UTC]' }); });
t('{hours: 1} round smallest hour largest year, relativeTo +275760-09-12T12:00[UTC]', function () { return D({ hours: 1 }).round({ smallestUnit: 'hour', largestUnit: 'year', relativeTo: '+275760-09-12T12:00[UTC]' }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
