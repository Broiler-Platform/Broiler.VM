/*---
description: SJL-10.1 probe r7-narrowing-in-addition
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
// R7: integer narrowing inside addition (the milestone's "integer narrowing that wraps").
t('PlainDateTime("2020-06-15T12:00").add({years: 4294967295})', function () { return PDT('2020-06-15T12:00').add({ years: 4294967295 }); });
t('PlainDateTime("2020-06-15T12:00").subtract({years: 4294967295})', function () { return PDT('2020-06-15T12:00').subtract({ years: 4294967295 }); });
t('PlainDateTime("2020-06-15T12:00").add({years: 4294967295 - 2020 - 1000})', function () { return PDT('2020-06-15T12:00').add({ years: 4294967295 - 2020 - 1000 }); });
t('PlainDateTime("2020-06-15T12:00").add({months: 4294967295})', function () { return PDT('2020-06-15T12:00').add({ months: 4294967295 }); });
t('PlainYearMonth("2020-06").add({years: 4294967295})', function () { return PYM('2020-06').add({ years: 4294967295 }); });
t('PlainYearMonth("2020-06").subtract({years: 4294967295})', function () { return PYM('2020-06').subtract({ years: 4294967295 }); });
t('PlainDate("2020-06-15").add({years: 4294967295})', function () { return PD('2020-06-15').add({ years: 4294967295 }); });
t('ZonedDateTime("2020-06-15T12:00[UTC]").add({years: 4294967295})', function () { return Z('2020-06-15T12:00[UTC]').add({ years: 4294967295 }); });
t('PlainDate("2020-06-15[u-ca=hebrew]").add({years: 4294967295})', function () { return PD('2020-06-15[u-ca=hebrew]').add({ years: 4294967295 }); });
t('PlainDate("2020-06-15[u-ca=coptic]").subtract({years: 4294967295})', function () { return PD('2020-06-15[u-ca=coptic]').subtract({ years: 4294967295 }); });
t('PlainDate("2020-01-01").add({nanoseconds: 9e24})', function () { return PD('2020-01-01').add({ nanoseconds: 9e24 }); });
t('PlainDate("2020-01-01").add({microseconds: 9e21})', function () { return PD('2020-01-01').add({ microseconds: 9e21 }); });
t('PlainDate("2020-01-01").add({milliseconds: 9e18})', function () { return PD('2020-01-01').add({ milliseconds: 9e18 }); });
t('PlainDateTime("2020-01-01T00:00").add({nanoseconds: 9e24})', function () { return PDT('2020-01-01T00:00').add({ nanoseconds: 9e24 }); });
t('PlainDateTime("2020-01-01T00:00").add({microseconds: 9e21})', function () { return PDT('2020-01-01T00:00').add({ microseconds: 9e21 }); });
t('ZonedDateTime("2020-01-01T00:00[UTC]").add({nanoseconds: 9e24})', function () { return Z('2020-01-01T00:00[UTC]').add({ nanoseconds: 9e24 }); });
t('Duration({nanoseconds: 9e24}).round({smallestUnit: "second", relativeTo: "2020-01-01"})', function () { return D({ nanoseconds: 9e24 }).round({ smallestUnit: 'second', relativeTo: '2020-01-01' }); });
t('Duration({nanoseconds: 9e24}).round({largestUnit: "year", relativeTo: "2020-01-01"})', function () { return D({ nanoseconds: 9e24 }).round({ largestUnit: 'year', relativeTo: '2020-01-01' }); });
t('Duration({nanoseconds: 9e24}).total({unit: "day", relativeTo: "2020-01-01"})', function () { return D({ nanoseconds: 9e24 }).total({ unit: 'day', relativeTo: '2020-01-01' }); });
t('Duration({nanoseconds: 9e24}).total("day")', function () { return D({ nanoseconds: 9e24 }).total('day'); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
