/*---
description: SJL-10.1 probe f5-datetime-differences
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
// F5: PlainDateTime until and since: the day borrowed for the time, time largest units, rounding.
t('2020-01-01T12:00 until 2020-01-03T06:00', function () { return PDT('2020-01-01T12:00').until('2020-01-03T06:00'); });
t('2020-01-03T06:00 since 2020-01-01T12:00', function () { return PDT('2020-01-03T06:00').since('2020-01-01T12:00'); });
t('2020-01-03T06:00 until 2020-01-01T12:00', function () { return PDT('2020-01-03T06:00').until('2020-01-01T12:00'); });
t('2020-01-01T12:00 until 2020-01-03T06:00 largest hour', function () { return PDT('2020-01-01T12:00').until('2020-01-03T06:00', { largestUnit: 'hour' }); });
t('2020-01-01T12:00 until 2020-01-03T06:00 smallest day halfExpand', function () { return PDT('2020-01-01T12:00').until('2020-01-03T06:00', { smallestUnit: 'day', roundingMode: 'halfExpand' }); });
t('2020-01-31T12:00 until 2020-03-01T06:00 largest month', function () { return PDT('2020-01-31T12:00').until('2020-03-01T06:00', { largestUnit: 'month' }); });
t('2019-03-31T00:00 until 2020-02-29T12:00 month/year trunc', function () { return PDT('2019-03-31T00:00').until('2020-02-29T12:00', { smallestUnit: 'month', largestUnit: 'year' }); });
t('2019-03-31T00:00 until 2020-02-29T12:00 month/year halfExpand', function () { return PDT('2019-03-31T00:00').until('2020-02-29T12:00', { smallestUnit: 'month', largestUnit: 'year', roundingMode: 'halfExpand' }); });
t('2020-02-29T12:00 since 2019-03-31T00:00 month/year trunc', function () { return PDT('2020-02-29T12:00').since('2019-03-31T00:00', { smallestUnit: 'month', largestUnit: 'year' }); });
t('2019-01-01T00:00 until 2020-12-31T23:59:59.999999999 microsecond/year halfExpand', function () { return PDT('2019-01-01T00:00').until('2020-12-31T23:59:59.999999999', { smallestUnit: 'microsecond', largestUnit: 'year', roundingMode: 'halfExpand' }); });
t('2020-01-01T00:00 until 2020-01-01T23:59:59.5 second/day halfExpand', function () { return PDT('2020-01-01T00:00').until('2020-01-01T23:59:59.5', { smallestUnit: 'second', largestUnit: 'day', roundingMode: 'halfExpand' }); });
t('2020-01-01T00:00 until 2020-01-01T23:30 hour/day increment 3 ceil', function () { return PDT('2020-01-01T00:00').until('2020-01-01T23:30', { smallestUnit: 'hour', largestUnit: 'day', roundingIncrement: 3, roundingMode: 'ceil' }); });
t('2020-01-01T00:00 until 2020-01-08T12:00 week/week halfExpand', function () { return PDT('2020-01-01T00:00').until('2020-01-08T12:00', { smallestUnit: 'week', roundingMode: 'halfExpand' }); });
t('-271821-04-19T00:00:00.000000001 until +275760-09-13T23:59:59.999999999 largest nanosecond', function () { return PDT('-271821-04-19T00:00:00.000000001').until('+275760-09-13T23:59:59.999999999', { largestUnit: 'nanosecond' }); });
t('-271821-04-19T00:00:00.000000001 until +275760-09-13T23:59:59.999999999 largest year', function () { return PDT('-271821-04-19T00:00:00.000000001').until('+275760-09-13T23:59:59.999999999', { largestUnit: 'year' }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
