/*---
description: SJL-10.1 probe b5-limits-in-addition
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
// B5: addition at the representable limits.
t('PlainDate("+275760-09-13").add({days: 1})', function () { return PD('+275760-09-13').add({ days: 1 }); });
t('PlainDate("-271821-04-19").subtract({days: 1})', function () { return PD('-271821-04-19').subtract({ days: 1 }); });
t('PlainDate("1970-01-01").add({days: 100000000})', function () { return PD('1970-01-01').add({ days: 100000000 }); });
t('PlainDate("1970-01-01").add({days: 100000001})', function () { return PD('1970-01-01').add({ days: 100000001 }); });
t('PlainDate("-271821-04-19").add({days: 0})', function () { return PD('-271821-04-19').add({ days: 0 }); });
t('PlainDateTime("+275760-09-13T00:00").add({hours: 23, minutes: 59})', function () { return PDT('+275760-09-13T00:00').add({ hours: 23, minutes: 59 }); });
t('PlainDateTime("+275760-09-13T00:00").add({days: 1})', function () { return PDT('+275760-09-13T00:00').add({ days: 1 }); });
t('PlainDateTime("-271821-04-19T00:00:00.000000001").subtract({nanoseconds: 1})', function () { return PDT('-271821-04-19T00:00:00.000000001').subtract({ nanoseconds: 1 }); });
t('PlainYearMonth("+275760-09").add({months: 1})', function () { return PYM('+275760-09').add({ months: 1 }); });
t('PlainYearMonth("-271821-04").add({months: 0})', function () { return PYM('-271821-04').add({ months: 0 }); });
t('PlainYearMonth("-271821-05").subtract({months: 1})', function () { return PYM('-271821-05').subtract({ months: 1 }); });
t('PlainYearMonth("+275760-08").add({months: 1})', function () { return PYM('+275760-08').add({ months: 1 }); });
t('Instant("+275760-09-13T00:00Z").add({nanoseconds: 1})', function () { return I('+275760-09-13T00:00Z').add({ nanoseconds: 1 }); });
t('ZonedDateTime(max, "UTC").add({nanoseconds: 1})', function () { return new T.ZonedDateTime(8640000000000000000000n, 'UTC').add({ nanoseconds: 1 }); });
t('ZonedDateTime(max - 1n, "UTC").add({days: 0, nanoseconds: 1})', function () { return new T.ZonedDateTime(8639999999999999999999n, 'UTC').add({ nanoseconds: 1 }); });
t('ZonedDateTime("+275760-09-12T12:00[UTC]").add({days: 1})', function () { return Z('+275760-09-12T12:00[UTC]').add({ days: 1 }); });
t('ZonedDateTime("+275760-09-12T00:00[UTC]").add({days: 1})', function () { return Z('+275760-09-12T00:00[UTC]').add({ days: 1 }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
