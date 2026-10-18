/*---
description: SJL-10.1 probe d2-largest-unit-and-balance
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
// D2: DefaultTemporalLargestUnit and TemporalDurationFromInternal, through Duration addition.
t('Duration({minutes: 30}).add({minutes: 90})', function () { return D({ minutes: 30 }).add({ minutes: 90 }); });
t('Duration({hours: 1}).add({minutes: 90})', function () { return D({ hours: 1 }).add({ minutes: 90 }); });
t('Duration({days: 1}).add({hours: 24})', function () { return D({ days: 1 }).add({ hours: 24 }); });
t('Duration({days: 1}).add({hours: 25})', function () { return D({ days: 1 }).add({ hours: 25 }); });
t('Duration({seconds: 59}).add({milliseconds: 1001})', function () { return D({ seconds: 59 }).add({ milliseconds: 1001 }); });
t('Duration({microseconds: 1}).add({nanoseconds: 2000})', function () { return D({ microseconds: 1 }).add({ nanoseconds: 2000 }); });
t('Duration({hours: 1}).subtract({hours: 2})', function () { return D({ hours: 1 }).subtract({ hours: 2 }); });
t('Duration({days: 1, hours: -0}).subtract({days: 1})', function () { return D({ days: 1 }).subtract({ days: 1 }); });
t('Duration({seconds: 2**53 - 1}).add({nanoseconds: 999999999})', function () { return D({ seconds: Math.pow(2, 53) - 1 }).add({ nanoseconds: 999999999 }); });
t('Duration({nanoseconds: 2**53 - 1}).add({nanoseconds: 2})', function () { return D({ nanoseconds: Math.pow(2, 53) - 1 }).add({ nanoseconds: 2 }).nanoseconds; });
t('Duration({nanoseconds: 2**53 - 1}).add({nanoseconds: 4})', function () { return D({ nanoseconds: Math.pow(2, 53) - 1 }).add({ nanoseconds: 4 }).nanoseconds; });
t('Duration({milliseconds: 2**53 - 1}).add({milliseconds: 2**53 - 1}).milliseconds', function () { return D({ milliseconds: Math.pow(2, 53) - 1 }).add({ milliseconds: Math.pow(2, 53) - 1 }).milliseconds; });
t('Duration({seconds: 2**53 - 1}).add({seconds: 1})', function () { return D({ seconds: Math.pow(2, 53) - 1 }).add({ seconds: 1 }); });
t('Duration({days: 104249991374}).add({days: 1})', function () { return D({ days: 104249991374 }).add({ days: 1 }); });
t('Duration({days: 104249991374}).add({hours: 1})', function () { return D({ days: 104249991374 }).add({ hours: 1 }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
