/*---
description: SJL-10.1 probe r2-datetime-limits
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
// R2: ISO date-time limits (ISODateTimeWithinLimits: strictly inside nsMinInstant - nsPerDay .. nsMaxInstant + nsPerDay).
t('new PlainDateTime(-271821, 4, 19)', function () { return new T.PlainDateTime(-271821, 4, 19); });
t('new PlainDateTime(-271821, 4, 19, 0, 0, 0, 0, 0, 1)', function () { return new T.PlainDateTime(-271821, 4, 19, 0, 0, 0, 0, 0, 1); });
t('new PlainDateTime(275760, 9, 13, 23, 59, 59, 999, 999, 999)', function () { return new T.PlainDateTime(275760, 9, 13, 23, 59, 59, 999, 999, 999); });
t('new PlainDateTime(275760, 9, 14)', function () { return new T.PlainDateTime(275760, 9, 14); });
t('PlainDateTime.from("-271821-04-19T00:00")', function () { return PDT('-271821-04-19T00:00'); });
t('PlainDateTime.from("-271821-04-19T00:00:00.000000001")', function () { return PDT('-271821-04-19T00:00:00.000000001'); });
t('PlainDate("-271821-04-19").toPlainDateTime()', function () { return PD('-271821-04-19').toPlainDateTime(); });
t('PlainDate("-271821-04-19").toPlainDateTime("00:00:00.000000001")', function () { return PD('-271821-04-19').toPlainDateTime('00:00:00.000000001'); });
t('PlainDate("+275760-09-13").toPlainDateTime("23:59:59.999999999")', function () { return PD('+275760-09-13').toPlainDateTime('23:59:59.999999999'); });
t('PlainDateTime.from({year: -271821, month: 4, day: 19})', function () { return PDT({ year: -271821, month: 4, day: 19 }); });
t('PlainDateTime.from({year: -271821, month: 4, day: 19, nanosecond: 1})', function () { return PDT({ year: -271821, month: 4, day: 19, nanosecond: 1 }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
