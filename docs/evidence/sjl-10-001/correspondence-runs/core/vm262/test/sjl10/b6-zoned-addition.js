/*---
description: SJL-10.1 probe b6-zoned-addition
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
// B6: AddZonedDateTime: the date part on the wall clock, then the time part on the exact timeline.
t('NY 2026-03-07T02:30 + {days: 1}', function () { return Z('2026-03-07T02:30-05:00[America/New_York]').add({ days: 1 }); });
t('NY 2026-03-07T02:30 + {hours: 24}', function () { return Z('2026-03-07T02:30-05:00[America/New_York]').add({ hours: 24 }); });
t('NY 2026-03-07T02:30 + {days: 1}, disambiguation n/a, reject', function () { return Z('2026-03-07T02:30-05:00[America/New_York]').add({ days: 1 }, { overflow: 'reject' }); });
t('NY 2026-11-01T00:30-04:00 + {days: 1}', function () { return Z('2026-11-01T00:30-04:00[America/New_York]').add({ days: 1 }); });
t('NY 2026-11-01T00:30-04:00 + {hours: 24}', function () { return Z('2026-11-01T00:30-04:00[America/New_York]').add({ hours: 24 }); });
t('NY 2026-10-31T01:30-04:00 + {days: 1}', function () { return Z('2026-10-31T01:30-04:00[America/New_York]').add({ days: 1 }); });
t('NY 2026-11-01T01:30-05:00 - {days: 1}', function () { return Z('2026-11-01T01:30-05:00[America/New_York]').subtract({ days: 1 }); });
t('NY 2026-03-08T12:00-04:00 - {days: 1, hours: 12}', function () { return Z('2026-03-08T12:00-04:00[America/New_York]').subtract({ days: 1, hours: 12 }); });
t('NY 2026-01-31T02:30-05:00 + {months: 1, weeks: 1}', function () { return Z('2026-01-31T02:30-05:00[America/New_York]').add({ months: 1, weeks: 1 }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
