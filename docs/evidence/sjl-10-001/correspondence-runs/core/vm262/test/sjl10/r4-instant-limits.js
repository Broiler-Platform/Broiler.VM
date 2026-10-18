/*---
description: SJL-10.1 probe r4-instant-limits
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
// R4: instant limits (IsValidEpochNanoseconds: |ns| <= 8.64e21) and wall clocks at them.
t('Instant.fromEpochNanoseconds(8640000000000000000000n)', function () { return T.Instant.fromEpochNanoseconds(8640000000000000000000n); });
t('Instant.fromEpochNanoseconds(8640000000000000000001n)', function () { return T.Instant.fromEpochNanoseconds(8640000000000000000001n); });
t('Instant.fromEpochNanoseconds(-8640000000000000000000n)', function () { return T.Instant.fromEpochNanoseconds(-8640000000000000000000n); });
t('Instant.fromEpochNanoseconds(-8640000000000000000001n)', function () { return T.Instant.fromEpochNanoseconds(-8640000000000000000001n); });
t('Instant.fromEpochMilliseconds(8.64e15)', function () { return T.Instant.fromEpochMilliseconds(8.64e15); });
t('Instant.fromEpochMilliseconds(8.64e15 + 1)', function () { return T.Instant.fromEpochMilliseconds(8.64e15 + 1); });
t('Instant.from("-271821-04-20T00:00Z")', function () { return I('-271821-04-20T00:00Z'); });
t('Instant.from("-271821-04-19T23:59:59.999999999Z")', function () { return I('-271821-04-19T23:59:59.999999999Z'); });
t('Instant.from("+275760-09-13T00:00Z")', function () { return I('+275760-09-13T00:00Z'); });
t('Instant.from("+275760-09-13T00:00:00.000000001Z")', function () { return I('+275760-09-13T00:00:00.000000001Z'); });
t('Instant.from("+275760-09-13T01:00+01:00")', function () { return I('+275760-09-13T01:00+01:00'); });
t('Instant.from("-271821-04-19T23:00-01:00")', function () { return I('-271821-04-19T23:00-01:00'); });
t('new ZonedDateTime(max, "Etc/GMT-14").toPlainDateTime()', function () { return new T.ZonedDateTime(8640000000000000000000n, 'Etc/GMT-14').toPlainDateTime(); });
t('new ZonedDateTime(max, "Etc/GMT-14").toPlainDate()', function () { return new T.ZonedDateTime(8640000000000000000000n, 'Etc/GMT-14').toPlainDate(); });
t('new ZonedDateTime(min, "Etc/GMT+12").toPlainDateTime()', function () { return new T.ZonedDateTime(-8640000000000000000000n, 'Etc/GMT+12').toPlainDateTime(); });
t('new ZonedDateTime(min, "-23:59").toPlainDateTime()', function () { return new T.ZonedDateTime(-8640000000000000000000n, '-23:59').toPlainDateTime(); });
t('new ZonedDateTime(max + 1n, "UTC")', function () { return new T.ZonedDateTime(8640000000000000000001n, 'UTC'); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
