/*---
description: SJL-10.1 probe r8-duration-limits
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
// R8: duration record limits (IsValidDuration) and the fields' Number values.
t('new Duration(2**32 - 1)', function () { return new T.Duration(Math.pow(2, 32) - 1); });
t('new Duration(2**32)', function () { return new T.Duration(Math.pow(2, 32)); });
t('new Duration(0, 2**32)', function () { return new T.Duration(0, Math.pow(2, 32)); });
t('new Duration(0, 0, -(2**32))', function () { return new T.Duration(0, 0, -Math.pow(2, 32)); });
t('new Duration(0, 0, 0, 104249991374)', function () { return new T.Duration(0, 0, 0, 104249991374); });
t('new Duration(0, 0, 0, 104249991375)', function () { return new T.Duration(0, 0, 0, 104249991375); });
t('new Duration(0, 0, 0, 0, 0, 0, 2**53 - 1)', function () { return new T.Duration(0, 0, 0, 0, 0, 0, Math.pow(2, 53) - 1); });
t('new Duration(0, 0, 0, 0, 0, 0, 2**53)', function () { return new T.Duration(0, 0, 0, 0, 0, 0, Math.pow(2, 53)); });
t('new Duration(0,0,0,0,0,0, 2**53 - 1, 999, 999, 999)', function () { return new T.Duration(0, 0, 0, 0, 0, 0, Math.pow(2, 53) - 1, 999, 999, 999); });
t('new Duration(0,0,0,0,0,0, 2**53 - 1, 999, 999, 1000)', function () { return new T.Duration(0, 0, 0, 0, 0, 0, Math.pow(2, 53) - 1, 999, 999, 1000); });
t('new Duration(0,0,0,0,0,0,0,0,0, 9e24)', function () { return new T.Duration(0, 0, 0, 0, 0, 0, 0, 0, 0, 9e24); });
t('new Duration(0,0,0,0,0,0,0,0,0, 2**53 * 1e9)', function () { return new T.Duration(0, 0, 0, 0, 0, 0, 0, 0, 0, Math.pow(2, 53) * 1e9); });
t('new Duration(0,0,0,0, 2501999792983, 36)', function () { return new T.Duration(0, 0, 0, 0, 2501999792983, 36); });
t('new Duration(0,0,0,0, 2501999792983, 37)', function () { return new T.Duration(0, 0, 0, 0, 2501999792983, 37); });
t('new Duration(1, -1)', function () { return new T.Duration(1, -1); });
t('new Duration(0, 0, 0, 0, 0, 0, 0, 0, 0, 1.5)', function () { return new T.Duration(0, 0, 0, 0, 0, 0, 0, 0, 0, 1.5); });
t('new Duration(Infinity)', function () { return new T.Duration(Infinity); });
t('Object.is(new Duration(-0).years, 0)', function () { return Object.is(new T.Duration(-0).years, 0); });
t('Duration.from({nanoseconds: 9007199254740993}).nanoseconds', function () { return D({ nanoseconds: 9007199254740993 }).nanoseconds; });
t('Duration.from({days: 104249991374, seconds: 2**53 - 1 - 104249991374 * 86400}).days', function () { return D({ days: 104249991374, seconds: Math.pow(2, 53) - 1 - 104249991374 * 86400 }).days; });
t('Duration.from("P4294967296Y")', function () { return D('P4294967296Y'); });
t('Duration.from("PT9007199254740992S")', function () { return D('PT9007199254740992S'); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
