/*---
description: SJL-10.1 probe r1-date-limits
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
// R1: ISO date limits (ISODateWithinLimits: the date at noon is a representable date-time).
t('new PlainDate(-271821, 4, 19)', function () { return new T.PlainDate(-271821, 4, 19); });
t('new PlainDate(-271821, 4, 18)', function () { return new T.PlainDate(-271821, 4, 18); });
t('new PlainDate(275760, 9, 13)', function () { return new T.PlainDate(275760, 9, 13); });
t('new PlainDate(275760, 9, 14)', function () { return new T.PlainDate(275760, 9, 14); });
t('PlainDate.from("-271821-04-19")', function () { return PD('-271821-04-19'); });
t('PlainDate.from("-271821-04-18")', function () { return PD('-271821-04-18'); });
t('PlainDate.from("+275760-09-14")', function () { return PD('+275760-09-14'); });
t('PlainDate.from({year: -271821, month: 4, day: 18})', function () { return PD({ year: -271821, month: 4, day: 18 }); });
t('PlainDate.from({year: 275760, month: 9, day: 13})', function () { return PD({ year: 275760, month: 9, day: 13 }); });
t('PlainMonthDay(4, 18, undefined, -271821)', function () { return new T.PlainMonthDay(4, 18, undefined, -271821); });
t('PlainMonthDay(4, 19, undefined, -271821)', function () { return new T.PlainMonthDay(4, 19, undefined, -271821).toString(); });
t('PlainMonthDay(9, 14, undefined, 275760)', function () { return new T.PlainMonthDay(9, 14, undefined, 275760); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
