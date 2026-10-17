/*---
description: SJL-8 probe a1-offsets
flags: [raw]
---*/
var __vmTest = true;
// Shared prelude: Broiler.JS's shell has print; Broiler.VM's test262 realm has none, so the
// lines are collected and handed back in one uncaught Error the conformance root prints.
var __lines = [];
var __host = typeof __vmTest === 'undefined' && typeof print === 'function';
function out(s) { s = String(s); if (__host) print(s); else __lines.push(s); }
function show(v) {
  if (v === null) return 'null';
  if (v === undefined) return 'undefined';
  if (typeof v === 'bigint') return v + 'n';
  if (typeof v === 'string') return v;
  if (Array.isArray(v)) return '[' + v.map(show).join(', ') + ']';
  return String(v);
}
function t(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + ((e && e.constructor && e.constructor.name) || typeof e) + ': ' + (e && e.message); }
  out(label + ' => ' + r);
}
function at(instant, zone) { return Temporal.Instant.from(instant).toZonedDateTimeISO(zone); }
// A1: the offset at an instant, inside both datasets' common range.
var cases = [
  ['America/New_York', '2026-03-08T06:59:59Z'], ['America/New_York', '2026-03-08T07:00:00Z'], ['America/New_York', '2026-11-01T05:59:59Z'], ['America/New_York', '2026-11-01T06:00:00Z'],
  ['America/New_York', '1883-11-18T16:59:59Z'], ['Asia/Kolkata', '2026-01-01T00:00Z'], ['Australia/Lord_Howe', '2026-01-01T00:00Z'], ['Australia/Lord_Howe', '2026-07-01T00:00Z'],
  ['Pacific/Kiritimati', '1994-12-30T00:00Z'], ['Pacific/Kiritimati', '1995-01-02T00:00Z'], ['Africa/Monrovia', '1970-01-01T00:00Z'], ['Europe/Paris', '1900-01-01T00:00Z'],
  ['Asia/Kathmandu', '2026-01-01T00:00Z'], ['America/St_Johns', '2026-07-01T00:00Z'], ['Pacific/Apia', '2011-12-29T12:00Z'], ['Pacific/Apia', '2011-12-31T12:00Z'],
  ['America/New_York', '-271821-04-20T00:00Z'], ['America/New_York', '+275760-09-13T00:00Z'], ['Europe/London', '1971-07-01T00:00Z'],
];
cases.forEach(function (c) {
  t(c[0] + ' @ ' + c[1], function () { var z = at(c[1], c[0]); return z.offset + ' ' + z.offsetNanoseconds; });
});
t('Instant.toString({timeZone: "Africa/Monrovia"}) at 1970', function () { return Temporal.Instant.from('1970-01-01T00:00Z').toString({ timeZone: 'Africa/Monrovia' }); });
t('ZonedDateTime Africa/Monrovia 1970 toString', function () { return at('1970-01-01T00:00Z', 'Africa/Monrovia').toString(); });
t('Date.prototype.toTemporalInstant().toZonedDateTimeISO("Asia/Kolkata")', function () { return new Date(0).toTemporalInstant().toZonedDateTimeISO('Asia/Kolkata').toString(); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
