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
// D10: Duration.prototype.total: exact fractions rounded once, plain and zoned relativeTo.
var spring = '2026-03-08T00:00-05:00[America/New_York]', fall = '2026-11-01T00:00-04:00[America/New_York]';
t('{hours: 1, minutes: 30}.total("hour")', function () { return D({ hours: 1, minutes: 30 }).total('hour'); });
t('{days: 45}.total({unit: "month", relativeTo: "2020-01-01"})', function () { return D({ days: 45 }).total({ unit: 'month', relativeTo: '2020-01-01' }); });
t('{days: 45}.total({unit: "month", relativeTo: "2020-01-01T00:00[UTC]"})', function () { return D({ days: 45 }).total({ unit: 'month', relativeTo: '2020-01-01T00:00[UTC]' }); });
t('{nanoseconds: MAX_SAFE_INTEGER}.total("second")', function () { return D({ nanoseconds: Number.MAX_SAFE_INTEGER }).total('second'); });
t('{seconds: 2**53 - 1, nanoseconds: 999999999}.total("nanosecond")', function () { return D({ seconds: Math.pow(2, 53) - 1, nanoseconds: 999999999 }).total('nanosecond'); });
t('{seconds: 2**53 - 1, nanoseconds: 999999999}.total("day")', function () { return D({ seconds: Math.pow(2, 53) - 1, nanoseconds: 999999999 }).total('day'); });
t('{days: 1}.total({unit: "hour", relativeTo: NY spring})', function () { return D({ days: 1 }).total({ unit: 'hour', relativeTo: spring }); });
t('{hours: 12}.total({unit: "day", relativeTo: NY spring})', function () { return D({ hours: 12 }).total({ unit: 'day', relativeTo: spring }); });
t('{hours: 12}.total({unit: "day", relativeTo: NY fall})', function () { return D({ hours: 12 }).total({ unit: 'day', relativeTo: fall }); });
t('{months: 1}.total({unit: "day", relativeTo: "2020-02-01"})', function () { return D({ months: 1 }).total({ unit: 'day', relativeTo: '2020-02-01' }); });
t('{years: 1}.total({unit: "week", relativeTo: "2020-01-01"})', function () { return D({ years: 1 }).total({ unit: 'week', relativeTo: '2020-01-01' }); });
t('{months: 1}.total("day")', function () { return D({ months: 1 }).total('day'); });
t('{days: 1}.total("day")', function () { return D({ days: 1 }).total('day'); });
t('{days: 1}.total({unit: "month"})', function () { return D({ days: 1 }).total({ unit: 'month' }); });
t('{days: 1}.total({})', function () { return D({ days: 1 }).total({}); });
t('{months: 10, days: 29, hours: 12}.total({unit: "month", relativeTo: "2019-03-31"})', function () { return D({ months: 10, days: 29, hours: 12 }).total({ unit: 'month', relativeTo: '2019-03-31' }); });
t('{months: 10, days: 29, hours: 12}.total({unit: "year", relativeTo: "2019-03-31T00:00[UTC]"})', function () { return D({ months: 10, days: 29, hours: 12 }).total({ unit: 'year', relativeTo: '2019-03-31T00:00[UTC]' }); });
t('{days: -45}.total({unit: "month", relativeTo: "2020-03-15"})', function () { return D({ days: -45 }).total({ unit: 'month', relativeTo: '2020-03-15' }); });
t('{days: 1}.total({unit: "day", relativeTo: "+275760-09-12T00:00:01[UTC]"})', function () { return D({ days: 1 }).total({ unit: 'day', relativeTo: '+275760-09-12T00:00:01+00:00[UTC]' }); });
t('{hours: 1}.total({unit: "day", relativeTo: "+275760-09-12T12:00[UTC]"})', function () { return D({ hours: 1 }).total({ unit: 'day', relativeTo: '+275760-09-12T12:00[UTC]' }); });
t('{days: 1}.total({unit: "day", relativeTo: "+275760-09-12"})', function () { return D({ days: 1 }).total({ unit: 'day', relativeTo: '+275760-09-12' }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
