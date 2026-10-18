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
// D6: Duration.prototype.round with a plain-date relativeTo: nudging, bubbling, the window shift.
t('{days: 45} round largest month, relativeTo 2020-01-31', function () { return D({ days: 45 }).round({ largestUnit: 'month', relativeTo: '2020-01-31' }); });
t('{days: 15} round smallest month, relativeTo 2020-02-01', function () { return D({ days: 15 }).round({ smallestUnit: 'month', relativeTo: '2020-02-01' }); });
t('{days: 15} round smallest month, relativeTo 2020-01-01', function () { return D({ days: 15 }).round({ smallestUnit: 'month', relativeTo: '2020-01-01' }); });
t('{months: 11, days: 30} round smallest month largest year, relativeTo 2020-01-01', function () { return D({ months: 11, days: 30 }).round({ smallestUnit: 'month', largestUnit: 'year', relativeTo: '2020-01-01' }); });
t('{days: 40} round smallest week largest month, relativeTo 2020-01-01', function () { return D({ days: 40 }).round({ smallestUnit: 'week', largestUnit: 'month', relativeTo: '2020-01-01' }); });
t('{days: 10} round smallest week, relativeTo 2020-01-01', function () { return D({ days: 10 }).round({ smallestUnit: 'week', relativeTo: '2020-01-01' }); });
t('{months: 10, days: 29, hours: 12} round month/year trunc, relativeTo 2019-03-31', function () { return D({ months: 10, days: 29, hours: 12 }).round({ smallestUnit: 'month', largestUnit: 'year', roundingMode: 'trunc', relativeTo: '2019-03-31' }); });
t('{months: 10, days: 29, hours: 12} round month/year halfExpand, relativeTo 2019-03-31', function () { return D({ months: 10, days: 29, hours: 12 }).round({ smallestUnit: 'month', largestUnit: 'year', roundingMode: 'halfExpand', relativeTo: '2019-03-31' }); });
t('{months: 10, days: 29, hours: 12} round month/month trunc, relativeTo 2019-03-31', function () { return D({ months: 10, days: 29, hours: 12 }).round({ smallestUnit: 'month', largestUnit: 'month', roundingMode: 'trunc', relativeTo: '2019-03-31' }); });
t('{hours: 36} round largest day, relativeTo 2020-01-01', function () { return D({ hours: 36 }).round({ largestUnit: 'day', relativeTo: '2020-01-01' }); });
t('{years: 1} round largest day, relativeTo 2020-01-01', function () { return D({ years: 1 }).round({ largestUnit: 'day', relativeTo: '2020-01-01' }); });
t('{years: 1} round largest day, relativeTo 2021-01-01', function () { return D({ years: 1 }).round({ largestUnit: 'day', relativeTo: '2021-01-01' }); });
t('{months: 1} round smallest day, relativeTo 2020-01-31', function () { return D({ months: 1 }).round({ smallestUnit: 'day', relativeTo: '2020-01-31' }); });
t('{months: -1, days: -15} round smallest month, relativeTo 2020-03-31', function () { return D({ months: -1, days: -15 }).round({ smallestUnit: 'month', relativeTo: '2020-03-31' }); });
t('{weeks: 3} round largest month, relativeTo 2020-02-15', function () { return D({ weeks: 3 }).round({ largestUnit: 'month', relativeTo: '2020-02-15' }); });
t('{days: 31} round smallest month increment 2 halfExpand, relativeTo 2020-01-01', function () { return D({ days: 31 }).round({ smallestUnit: 'month', largestUnit: 'month', roundingIncrement: 2, relativeTo: '2020-01-01' }); });
t('{days: 30} round smallest month increment 2 halfExpand, relativeTo 2020-01-01', function () { return D({ days: 30 }).round({ smallestUnit: 'month', largestUnit: 'month', roundingIncrement: 2, relativeTo: '2020-01-01' }); });
t('{hours: 23, minutes: 59, seconds: 59, nanoseconds: 999999999, months: 23, days: 30} round microsecond, largest year, relativeTo 2020-01-01', function () { return D({ months: 23, days: 30, hours: 23, minutes: 59, seconds: 59, milliseconds: 999, microseconds: 999, nanoseconds: 999 }).round({ smallestUnit: 'microsecond', largestUnit: 'year', relativeTo: '2020-01-01' }); });
t('{days: -1, hours: -12} round smallest day halfExpand, relativeTo 2020-01-01', function () { return D({ days: -1, hours: -12 }).round({ smallestUnit: 'day', relativeTo: '2020-01-01' }); });
t('{years: 1, months: 6} round smallest year halfEven, relativeTo 2020-01-01', function () { return D({ years: 1, months: 6 }).round({ smallestUnit: 'year', roundingMode: 'halfEven', relativeTo: '2020-01-01' }); });
t('{days: 366} round largest year, relativeTo PlainDateTime 2020-01-01T12:00', function () { return D({ days: 366 }).round({ largestUnit: 'year', relativeTo: PDT('2020-01-01T12:00') }); });
t('{days: 1} round largest year, relativeTo -271821-04-19', function () { return D({ days: 1 }).round({ largestUnit: 'year', relativeTo: '-271821-04-19' }); });
t('{days: 1} round largest year, relativeTo -271821-04-20', function () { return D({ days: 1 }).round({ largestUnit: 'year', relativeTo: '-271821-04-20' }); });
t('{} round largest year, relativeTo -271821-04-19', function () { return new T.Duration().round({ largestUnit: 'year', relativeTo: '-271821-04-19' }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
