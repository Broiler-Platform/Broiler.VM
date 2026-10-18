/*---
description: SJL-10.1 probe b2-balance-datetime
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
// B2: BalanceISODateTime through PlainDateTime and Instant arithmetic.
t('PlainDateTime("2020-12-31T23:00").add({hours: 25})', function () { return PDT('2020-12-31T23:00').add({ hours: 25 }); });
t('PlainDateTime("2020-01-01T00:00").subtract({minutes: 1})', function () { return PDT('2020-01-01T00:00').subtract({ minutes: 1 }); });
t('PlainDateTime("2020-01-01T12:00").subtract({hours: 36})', function () { return PDT('2020-01-01T12:00').subtract({ hours: 36 }); });
t('PlainDateTime("2020-02-28T12:00").add({days: 1, hours: 12})', function () { return PDT('2020-02-28T12:00').add({ days: 1, hours: 12 }); });
t('PlainDateTime("2020-01-31T23:00").add({months: 1, hours: 2})', function () { return PDT('2020-01-31T23:00').add({ months: 1, hours: 2 }); });
t('PlainDateTime("2020-01-01T00:00").add({nanoseconds: 86400e9 * 366})', function () { return PDT('2020-01-01T00:00').add({ nanoseconds: 86400e9 * 366 }); });
t('PlainDateTime("2020-01-01T00:00").add({seconds: 2**53 - 1})', function () { return PDT('2020-01-01T00:00').add({ seconds: Math.pow(2, 53) - 1 }); });
t('Instant("2020-01-01T00:00Z").add({hours: 48, nanoseconds: 1})', function () { return I('2020-01-01T00:00Z').add({ hours: 48, nanoseconds: 1 }); });
t('Instant("2020-01-01T00:00Z").add({days: 1})', function () { return I('2020-01-01T00:00Z').add({ days: 1 }); });
t('Instant("2020-01-01T00:00Z").subtract({hours: 2501999792983})', function () { return I('2020-01-01T00:00Z').subtract({ hours: 2501999792983 }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
