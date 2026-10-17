/*---
description: SJL-8 probe z3-extrapolation
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
// Z3: offsets after the last listed transition (2037 in a fat TZif file).
var cases = [
  ['America/New_York', '2037-07-01T12:00Z'], ['America/New_York', '2040-07-01T12:00Z'], ['America/New_York', '2400-07-01T12:00Z'],
  ['Europe/Berlin', '2037-07-01T12:00Z'], ['Europe/Berlin', '2040-07-01T12:00Z'],
  ['America/Santiago', '2040-01-15T12:00Z'], ['America/Santiago', '2040-07-01T12:00Z'],
  ['Australia/Sydney', '2040-01-15T12:00Z'], ['Australia/Sydney', '2040-07-01T12:00Z'],
  ['Asia/Kolkata', '2040-07-01T12:00Z'],
];
cases.forEach(function (c) { t(c[0] + ' @ ' + c[1] + ' offset', function () { return at(c[1], c[0]).offset; }); });
t('Intl.DateTimeFormat America/New_York hour at 2040-07-01T12:00Z', function () {
  return new Intl.DateTimeFormat('en-US', { timeZone: 'America/New_York', hour: 'numeric', hourCycle: 'h23' }).format(Date.UTC(2040, 6, 1, 12));
});
t('Intl.DateTimeFormat Europe/Berlin hour at 2040-07-01T12:00Z', function () {
  return new Intl.DateTimeFormat('en-US', { timeZone: 'Europe/Berlin', hour: 'numeric', hourCycle: 'h23' }).format(Date.UTC(2040, 6, 1, 12));
});
t('America/New_York 2040-07-01T12:00 wall clock to instant', function () { return Temporal.PlainDateTime.from('2040-07-01T12:00').toZonedDateTime('America/New_York').toInstant(); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
