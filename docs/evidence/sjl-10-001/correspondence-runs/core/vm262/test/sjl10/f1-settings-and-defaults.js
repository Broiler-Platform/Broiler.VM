/*---
description: SJL-10.1 probe f1-settings-and-defaults
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
// F1: GetDifferenceSettings: each type's defaults, unit groups, disallowed units and increments.
t('PlainDate 2020-01-01 until 2021-03-15', function () { return PD('2020-01-01').until('2021-03-15'); });
t('PlainDateTime 2020-01-01T00:00 until 2020-01-03T12:30', function () { return PDT('2020-01-01T00:00').until('2020-01-03T12:30'); });
t('PlainTime 01:00 until 23:30:15', function () { return PT('01:00').until('23:30:15'); });
t('Instant until +1 day 1 h 0.5 s', function () { return I('2020-01-01T00:00Z').until('2020-01-02T01:00:00.5Z'); });
t('ZonedDateTime until +25 h', function () { return Z('2020-01-01T00:00[UTC]').until('2020-01-02T01:00[UTC]'); });
t('PlainYearMonth 2019-05 until 2021-03', function () { return PYM('2019-05').until('2021-03'); });
t('PlainDate until largestUnit hour', function () { return PD('2020-01-01').until('2020-01-02', { largestUnit: 'hour' }); });
t('PlainDate until smallestUnit hour', function () { return PD('2020-01-01').until('2020-01-02', { smallestUnit: 'hour' }); });
t('PlainTime until largestUnit day', function () { return PT('01:00').until('02:00', { largestUnit: 'day' }); });
t('Instant until smallestUnit day', function () { return I('2020-01-01T00:00Z').until('2020-01-02T00:00Z', { smallestUnit: 'day' }); });
t('Instant until largestUnit day', function () { return I('2020-01-01T00:00Z').until('2020-01-02T00:00Z', { largestUnit: 'day' }); });
t('PlainYearMonth until smallestUnit week', function () { return PYM('2019-05').until('2021-03', { smallestUnit: 'week' }); });
t('PlainYearMonth until largestUnit day', function () { return PYM('2019-05').until('2021-03', { largestUnit: 'day' }); });
t('PlainDate until smallestUnit year largestUnit month', function () { return PD('2020-01-01').until('2021-03-15', { smallestUnit: 'year', largestUnit: 'month' }); });
t('PlainTime until smallestUnit minute increment 7', function () { return PT('01:00').until('02:00', { smallestUnit: 'minute', roundingIncrement: 7 }); });
t('PlainTime until smallestUnit minute increment 60', function () { return PT('01:00').until('02:00', { smallestUnit: 'minute', roundingIncrement: 60 }); });
t('Instant until smallestUnit hour increment 24', function () { return I('2020-01-01T00:00Z').until('2020-01-03T00:00Z', { smallestUnit: 'hour', roundingIncrement: 24 }); });
t('Instant until smallestUnit hour increment 12', function () { return I('2020-01-01T00:00Z').until('2020-01-03T05:00Z', { smallestUnit: 'hour', roundingIncrement: 12, largestUnit: 'hour' }); });
t('PlainDate until smallestUnit day increment 1e9', function () { return PD('2020-01-01').until('2020-01-02', { smallestUnit: 'day', roundingIncrement: 1e9 }); });
t('PlainDate until roundingIncrement 0', function () { return PD('2020-01-01').until('2020-01-02', { roundingIncrement: 0 }); });
t('PlainDate until roundingIncrement 1e9 + 1', function () { return PD('2020-01-01').until('2020-01-02', { roundingIncrement: 1e9 + 1 }); });
t('PlainDate until roundingMode bogus', function () { return PD('2020-01-01').until('2020-01-02', { roundingMode: 'bogus' }); });
t('PlainDate until largestUnit auto smallestUnit month', function () { return PD('2020-01-01').until('2021-03-15', { largestUnit: 'auto', smallestUnit: 'month' }); });
t('ZonedDateTime until smallestUnit day (largest auto)', function () { return Z('2020-01-01T00:00[UTC]').until('2020-01-03T12:00[UTC]', { smallestUnit: 'day' }); });
t('Instant until smallestUnit minute (largest auto)', function () { return I('2020-01-01T00:00Z').until('2020-01-01T02:00Z', { smallestUnit: 'minute' }); });
t('PlainDate until options 5', function () { return PD('2020-01-01').until('2020-01-02', 5); });
t('PlainDate until other calendar', function () { return PD('2020-01-01').until(PD('2020-02-01').withCalendar('gregory')); });
t('ZonedDateTime until other zone, largestUnit day', function () { return Z('2020-01-01T00:00[UTC]').until('2020-01-02T00:00+01:00[Europe/Berlin]', { largestUnit: 'day' }); });
t('ZonedDateTime until other zone, largestUnit hour', function () { return Z('2020-01-01T00:00[UTC]').until('2020-01-02T00:00+01:00[Europe/Berlin]', { largestUnit: 'hour' }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
