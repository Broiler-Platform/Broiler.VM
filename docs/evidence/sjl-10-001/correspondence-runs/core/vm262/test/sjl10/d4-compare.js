/*---
description: SJL-10.1 probe d4-compare
flags: [raw]
---*/
var __vmTest = true;
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
// D4: Temporal.Duration.compare.
var nySpring = '2026-03-08T00:00-05:00[America/New_York]';
t('compare({hours: 24}, {days: 1})', function () { return T.Duration.compare({ hours: 24 }, { days: 1 }); });
t('compare({days: 1}, {hours: 25})', function () { return T.Duration.compare({ days: 1 }, { hours: 25 }); });
t('compare({months: 1}, {days: 30})', function () { return T.Duration.compare({ months: 1 }, { days: 30 }); });
t('compare({months: 1}, {days: 30}, {relativeTo: "2020-02-01"})', function () { return T.Duration.compare({ months: 1 }, { days: 30 }, { relativeTo: '2020-02-01' }); });
t('compare({months: 1}, {days: 30}, {relativeTo: "2020-01-01"})', function () { return T.Duration.compare({ months: 1 }, { days: 30 }, { relativeTo: '2020-01-01' }); });
t('compare({days: 1}, {hours: 24}, {relativeTo: NY spring day})', function () { return T.Duration.compare({ days: 1 }, { hours: 24 }, { relativeTo: nySpring }); });
t('compare({days: 1}, {hours: 23}, {relativeTo: NY spring day})', function () { return T.Duration.compare({ days: 1 }, { hours: 23 }, { relativeTo: nySpring }); });
t('compare({months: 1}, {months: 1}) (identical, no relativeTo)', function () { return T.Duration.compare({ months: 1 }, { months: 1 }); });
t('compare({months: 1}, {months: 1}, {relativeTo: "bad"})', function () { return T.Duration.compare({ months: 1 }, { months: 1 }, { relativeTo: 'bad' }); });
t('compare({years: 1, seconds: 9e15}, {years: 1}, {relativeTo: "2020-01-01"})', function () { return T.Duration.compare({ years: 1, seconds: 9e15 }, { years: 1 }, { relativeTo: '2020-01-01' }); });
t('compare({years: 1, seconds: 9e15}, {years: 1, seconds: 9e15 - 1}, {relativeTo: "2020-01-01"})', function () { return T.Duration.compare({ years: 1, seconds: 9e15 }, { years: 1, seconds: 9e15 - 1 }, { relativeTo: '2020-01-01' }); });
t('compare({years: 1, seconds: 9007199254740991 - 31622399}, {years: 1}, {relativeTo: "2020-01-01"})', function () { return T.Duration.compare({ years: 1, seconds: 9007199254740991 - 31622399 }, { years: 1 }, { relativeTo: '2020-01-01' }); });
t('compare({days: 200000000}, {days: 1}, {relativeTo: "2020-01-01"})', function () { return T.Duration.compare({ days: 200000000 }, { days: 1 }, { relativeTo: '2020-01-01' }); });
t('compare({weeks: 1}, {days: 7}, {relativeTo: "2020-01-01"})', function () { return T.Duration.compare({ weeks: 1 }, { days: 7 }, { relativeTo: '2020-01-01' }); });
t('compare({years: 1}, {days: 365}, {relativeTo: "2020-01-01[u-ca=hebrew]"})', function () { return T.Duration.compare({ years: 1 }, { days: 365 }, { relativeTo: '2020-01-01[u-ca=hebrew]' }); });
t('compare({months: 1}, {days: 29}, {relativeTo: PlainDate hebrew 5780-05-01})', function () { return T.Duration.compare({ months: 1 }, { days: 29 }, { relativeTo: T.PlainDate.from({ year: 5780, monthCode: 'M05', day: 1, calendar: 'hebrew' }) }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
