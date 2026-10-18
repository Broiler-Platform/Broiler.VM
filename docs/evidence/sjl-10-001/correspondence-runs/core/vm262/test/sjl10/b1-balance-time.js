/*---
description: SJL-10.1 probe b1-balance-time
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
// B1: BalanceTime and AddTime through PlainTime arithmetic (wrapping at midnight, the date units ignored).
t('PlainTime("23:59:59.999999999").add({nanoseconds: 1})', function () { return PT('23:59:59.999999999').add({ nanoseconds: 1 }); });
t('PlainTime("00:00").subtract({nanoseconds: 1})', function () { return PT('00:00').subtract({ nanoseconds: 1 }); });
t('PlainTime("12:00").add({hours: 1e9})', function () { return PT('12:00').add({ hours: 1e9 }); });
t('PlainTime("12:00").subtract({hours: 2501999792983, minutes: 36})', function () { return PT('12:00').subtract({ hours: 2501999792983, minutes: 36 }); });
t('PlainTime("12:00").add({nanoseconds: 9e24})', function () { return PT('12:00').add({ nanoseconds: 9e24 }); });
t('PlainTime("12:00").add({days: 3, hours: 1})', function () { return PT('12:00').add({ days: 3, hours: 1 }); });
t('PlainTime("12:00").add({years: 1, months: 1, weeks: 1})', function () { return PT('12:00').add({ years: 1, months: 1, weeks: 1 }); });
t('PlainTime("12:00").add({minutes: 90, seconds: 3600})', function () { return PT('12:00').add({ minutes: 90, seconds: 3600 }); });
t('PlainTime("01:00").subtract({minutes: 61})', function () { return PT('01:00').subtract({ minutes: 61 }); });
t('PlainTime("12:00").add("-PT36H")', function () { return PT('12:00').add('-PT36H'); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
