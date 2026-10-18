/*---
description: SJL-10.1 probe d5-round-time
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
// D5: Duration.prototype.round with no relativeTo: the nine modes, increments, and the unit checks.
var modes = ['ceil', 'floor', 'expand', 'trunc', 'halfCeil', 'halfFloor', 'halfExpand', 'halfTrunc', 'halfEven'];
modes.forEach(function (m) {
  t('PT1H30M round hour ' + m, function () { return D('PT1H30M').round({ smallestUnit: 'hour', roundingMode: m }); });
  t('-PT1H30M round hour ' + m, function () { return D('-PT1H30M').round({ smallestUnit: 'hour', roundingMode: m }); });
  t('PT2H30M round hour ' + m, function () { return D('PT2H30M').round({ smallestUnit: 'hour', roundingMode: m }); });
  t('-PT1H20M round 30 minutes ' + m, function () { return D('-PT1H20M').round({ smallestUnit: 'minute', roundingIncrement: 30, roundingMode: m }); });
});
t('Duration({hours: 25, minutes: 30}).round({largestUnit: "day"})', function () { return D({ hours: 25, minutes: 30 }).round({ largestUnit: 'day' }); });
t('Duration({days: 3, hours: 12}).round({smallestUnit: "day", roundingIncrement: 2, largestUnit: "day"})', function () { return D({ days: 3, hours: 12 }).round({ smallestUnit: 'day', roundingIncrement: 2, largestUnit: 'day' }); });
t('Duration({minutes: 125}).round({smallestUnit: "minute", roundingIncrement: 15})', function () { return D({ minutes: 125 }).round({ smallestUnit: 'minute', roundingIncrement: 15 }); });
t('Duration({hours: 47}).round({largestUnit: "day"})', function () { return D({ hours: 47 }).round({ largestUnit: 'day' }); });
t('Duration({days: 2}).round({largestUnit: "hour"})', function () { return D({ days: 2 }).round({ largestUnit: 'hour' }); });
t('Duration({days: 2}).round({largestUnit: "auto", smallestUnit: "hour"})', function () { return D({ days: 2 }).round({ largestUnit: 'auto', smallestUnit: 'hour' }); });
t('Duration({hours: 1}).round("minutes")', function () { return D({ hours: 1 }).round('minutes'); });
t('Duration({hours: 1}).round({})', function () { return D({ hours: 1 }).round({}); });
t('Duration({hours: 1}).round()', function () { return D({ hours: 1 }).round(); });
t('Duration({hours: 1}).round({largestUnit: "hour", smallestUnit: "day"})', function () { return D({ hours: 1 }).round({ largestUnit: 'hour', smallestUnit: 'day' }); });
t('Duration({hours: 1}).round({smallestUnit: "hour", roundingIncrement: 5})', function () { return D({ hours: 1 }).round({ smallestUnit: 'hour', roundingIncrement: 5 }); });
t('Duration({hours: 1}).round({smallestUnit: "hour", roundingIncrement: 24})', function () { return D({ hours: 1 }).round({ smallestUnit: 'hour', roundingIncrement: 24 }); });
t('Duration({days: 1}).round({smallestUnit: "day", roundingIncrement: 2, largestUnit: "month"})', function () { return D({ days: 1 }).round({ smallestUnit: 'day', roundingIncrement: 2, largestUnit: 'month', relativeTo: '2020-01-01' }); });
t('Duration({days: 1}).round({smallestUnit: "month"})', function () { return D({ days: 1 }).round({ smallestUnit: 'month' }); });
t('Duration({years: 1}).round({smallestUnit: "hour"})', function () { return D({ years: 1 }).round({ smallestUnit: 'hour' }); });
t('Duration({weeks: 1}).round({largestUnit: "day"})', function () { return D({ weeks: 1 }).round({ largestUnit: 'day' }); });
t('Duration({days: 1}).round({largestUnit: "week"})', function () { return D({ days: 1 }).round({ largestUnit: 'week' }); });
t('Duration({seconds: 2**53 - 1}).round({smallestUnit: "hour", roundingMode: "ceil"})', function () { return D({ seconds: Math.pow(2, 53) - 1 }).round({ smallestUnit: 'hour', roundingMode: 'ceil' }); });
t('Duration({seconds: 2**53 - 1}).round({smallestUnit: "day", largestUnit: "day", roundingMode: "floor"})', function () { return D({ seconds: Math.pow(2, 53) - 1 }).round({ smallestUnit: 'day', largestUnit: 'day', roundingMode: 'floor' }); });
t('Duration({days: 1e8}).round({smallestUnit: "day", roundingIncrement: 1e9, roundingMode: "ceil", largestUnit: "day"})', function () { return D({ days: 1e8 }).round({ smallestUnit: 'day', roundingIncrement: 1e9, roundingMode: 'ceil', largestUnit: 'day' }); });
t('Duration({days: 1e8}).round({smallestUnit: "day", roundingIncrement: 2.5, largestUnit: "day"})', function () { return D({ days: 1e8 + 1 }).round({ smallestUnit: 'day', roundingIncrement: 2.5, largestUnit: 'day' }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
