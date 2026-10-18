/*---
description: SJL-10.1 probe f3-date-rounding
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
// F3: PlainDate until and since with rounding: modes, increments, the negated mode of since, bubbling.
t('2020-01-01 until 2020-07-20 smallest month (trunc)', function () { return PD('2020-01-01').until('2020-07-20', { smallestUnit: 'month' }); });
t('2020-01-01 until 2020-07-20 smallest month halfExpand', function () { return PD('2020-01-01').until('2020-07-20', { smallestUnit: 'month', roundingMode: 'halfExpand' }); });
t('2020-01-01 until 2020-07-02 smallest year halfExpand', function () { return PD('2020-01-01').until('2020-07-02', { smallestUnit: 'year', roundingMode: 'halfExpand' }); });
t('2020-01-01 until 2020-07-02 smallest year halfTrunc', function () { return PD('2020-01-01').until('2020-07-02', { smallestUnit: 'year', roundingMode: 'halfTrunc' }); });
t('2020-01-17 since 2020-01-01 smallest week floor', function () { return PD('2020-01-17').since('2020-01-01', { smallestUnit: 'week', roundingMode: 'floor' }); });
t('2020-01-01 since 2020-01-17 smallest week floor', function () { return PD('2020-01-01').since('2020-01-17', { smallestUnit: 'week', roundingMode: 'floor' }); });
t('2020-01-01 until 2020-01-17 smallest week ceil', function () { return PD('2020-01-01').until('2020-01-17', { smallestUnit: 'week', roundingMode: 'ceil' }); });
t('2020-01-17 until 2020-01-01 smallest week ceil', function () { return PD('2020-01-17').until('2020-01-01', { smallestUnit: 'week', roundingMode: 'ceil' }); });
t('2020-01-01 until 2020-03-19 month/day increment 5 halfExpand', function () { return PD('2020-01-01').until('2020-03-19', { largestUnit: 'month', smallestUnit: 'day', roundingIncrement: 5, roundingMode: 'halfExpand' }); });
t('2020-01-01 until 2020-03-30 month/day increment 5 halfExpand', function () { return PD('2020-01-01').until('2020-03-30', { largestUnit: 'month', smallestUnit: 'day', roundingIncrement: 5, roundingMode: 'halfExpand' }); });
t('2020-01-01 until 2020-04-15 smallest month increment 2 (trunc)', function () { return PD('2020-01-01').until('2020-04-15', { smallestUnit: 'month', roundingIncrement: 2 }); });
t('2020-01-01 until 2020-04-15 smallest month increment 2 halfExpand', function () { return PD('2020-01-01').until('2020-04-15', { smallestUnit: 'month', roundingIncrement: 2, roundingMode: 'halfExpand' }); });
t('2020-01-01 until 2021-12-20 smallest month largest year halfExpand', function () { return PD('2020-01-01').until('2021-12-20', { smallestUnit: 'month', largestUnit: 'year', roundingMode: 'halfExpand' }); });
t('2020-01-01 until 2020-01-29 smallest week largest month halfExpand', function () { return PD('2020-01-01').until('2020-01-29', { smallestUnit: 'week', largestUnit: 'month', roundingMode: 'halfExpand' }); });
t('2020-01-31 until 2020-02-29 smallest month', function () { return PD('2020-01-31').until('2020-02-29', { smallestUnit: 'month' }); });
t('2020-01-31 until 2020-02-29 smallest month halfExpand', function () { return PD('2020-01-31').until('2020-02-29', { smallestUnit: 'month', roundingMode: 'halfExpand' }); });
t('2020-01-01 until 2020-01-02 smallest day increment 1e9 ceil', function () { return PD('2020-01-01').until('2020-01-02', { smallestUnit: 'day', roundingIncrement: 1e9, roundingMode: 'ceil' }); });
t('+275760-09-12 until +275760-09-13 smallest month ceil', function () { return PD('+275760-09-12').until('+275760-09-13', { smallestUnit: 'month', roundingMode: 'ceil' }); });
t('-271821-04-20 since -271821-04-19 smallest year ceil', function () { return PD('-271821-04-20').since('-271821-04-19', { smallestUnit: 'year', roundingMode: 'ceil' }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
