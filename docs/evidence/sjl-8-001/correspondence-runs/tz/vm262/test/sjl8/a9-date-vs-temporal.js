/*---
description: SJL-8 probe a9-date-vs-temporal
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
// A9: Date's local time (the host provider) against Temporal's reading of the same zone, in Broiler.JS.
t('Temporal.Now.timeZoneId()', function () { return Temporal.Now.timeZoneId(); });
t('new Date(2027, 0, 15, 12).getTimezoneOffset()', function () { return new Date(2027, 0, 15, 12).getTimezoneOffset(); });
t('Temporal offset of the host zone at 2027-01-15T20:00Z', function () { return at('2027-01-15T20:00Z', Temporal.Now.timeZoneId()).offset; });
t('Temporal.Now.zonedDateTimeISO().offset (now)', function () { return Temporal.Now.zonedDateTimeISO().offset; });
t('new Date().getTimezoneOffset() (now)', function () { return new Date().getTimezoneOffset(); });
t('new Date(2040, 6, 1, 12).getTimezoneOffset()', function () { return new Date(2040, 6, 1, 12).getTimezoneOffset(); });
t('Temporal offset of the host zone at 2040-07-01T19:00Z', function () { return at('2040-07-01T19:00Z', Temporal.Now.timeZoneId()).offset; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
