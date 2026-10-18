/*---
description: SJL-10.1 probe r5-time-records
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
// R5: time records (IsValidTime) and ToIntegerWithTruncation of their fields.
t('new PlainTime(23, 59, 59, 999, 999, 999)', function () { return new T.PlainTime(23, 59, 59, 999, 999, 999); });
t('new PlainTime(24)', function () { return new T.PlainTime(24); });
t('new PlainTime(0, 60)', function () { return new T.PlainTime(0, 60); });
t('new PlainTime(0, 0, 0, 1000)', function () { return new T.PlainTime(0, 0, 0, 1000); });
t('new PlainTime(-1)', function () { return new T.PlainTime(-1); });
t('new PlainTime(1.9, 2.9)', function () { return new T.PlainTime(1.9, 2.9); });
t('new PlainTime(-0.5)', function () { return new T.PlainTime(-0.5); });
t('new PlainTime(NaN)', function () { return new T.PlainTime(NaN); });
t('new PlainTime(Infinity)', function () { return new T.PlainTime(Infinity); });
t('new PlainTime(undefined, 5)', function () { return new T.PlainTime(undefined, 5); });
t('new PlainDateTime(2020, 1, 1, 24)', function () { return new T.PlainDateTime(2020, 1, 1, 24); });
t('new PlainDate(2020, 2, 30)', function () { return new T.PlainDate(2020, 2, 30); });
t('new PlainDate(2021, 2, 29)', function () { return new T.PlainDate(2021, 2, 29); });
t('new PlainDate(2020, 13, 1)', function () { return new T.PlainDate(2020, 13, 1); });
t('new PlainDate(2020, 0, 1)', function () { return new T.PlainDate(2020, 0, 1); });
t('new PlainDate(2020.7, 1.2, 1.9)', function () { return new T.PlainDate(2020.7, 1.2, 1.9); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
