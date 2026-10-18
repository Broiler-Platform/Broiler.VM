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
// D11: round, total and compare relative to a date in another calendar.
var heb = '2020-01-01[u-ca=hebrew]';
t('{months: 1}.round({smallestUnit: "day", relativeTo: hebrew 2020-01-01})', function () { return D({ months: 1 }).round({ smallestUnit: 'day', relativeTo: heb }); });
t('{days: 45}.round({largestUnit: "month", relativeTo: hebrew 2020-01-01})', function () { return D({ days: 45 }).round({ largestUnit: 'month', relativeTo: heb }); });
t('{days: 45}.total({unit: "month", relativeTo: hebrew 2020-01-01})', function () { return D({ days: 45 }).total({ unit: 'month', relativeTo: heb }); });
t('{days: 45}.total({unit: "month", relativeTo: PlainDate.withCalendar("islamic-civil")})', function () { return D({ days: 45 }).total({ unit: 'month', relativeTo: PD('2020-01-01').withCalendar('islamic-civil') }); });
t('{months: 13}.round({largestUnit: "year", relativeTo: hebrew 5784-01-01 (a leap year)})', function () { return D({ months: 13 }).round({ largestUnit: 'year', relativeTo: T.PlainDate.from({ year: 5784, monthCode: 'M01', day: 1, calendar: 'hebrew' }) }); });
t('{days: 45}.round({largestUnit: "month", relativeTo: "2020-01-01T00:00[UTC][u-ca=coptic]"})', function () { return D({ days: 45 }).round({ largestUnit: 'month', relativeTo: '2020-01-01T00:00[UTC][u-ca=coptic]' }); });
t('{days: 45}.round({largestUnit: "month", relativeTo: {year: 5780, month: 4, day: 1, calendar: "hebrew"}})', function () { return D({ days: 45 }).round({ largestUnit: 'month', relativeTo: { year: 5780, month: 4, day: 1, calendar: 'hebrew' } }); });
t('{days: 45}.round({largestUnit: "month", relativeTo: "2020-01-01[u-ca=gregory]"})', function () { return D({ days: 45 }).round({ largestUnit: 'month', relativeTo: '2020-01-01[u-ca=gregory]' }); });
t('{days: 45}.round({largestUnit: "month", relativeTo: "2020-01-01[u-ca=japanese]"})', function () { return D({ days: 45 }).round({ largestUnit: 'month', relativeTo: '2020-01-01[u-ca=japanese]' }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
