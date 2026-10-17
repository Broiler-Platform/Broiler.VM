/*---
description: SJL-8 probe z4-factory
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
// Z4: the Factory zone.
t('new Temporal.ZonedDateTime(0n, "Factory")', function () { return new Temporal.ZonedDateTime(0n, 'Factory').toString(); });
t('new Temporal.ZonedDateTime(0n, "factory").timeZoneId', function () { return new Temporal.ZonedDateTime(0n, 'factory').timeZoneId; });
t('Intl.DateTimeFormat timeZone Factory resolved', function () { return new Intl.DateTimeFormat('en', { timeZone: 'Factory' }).resolvedOptions().timeZone; });
t('Intl.supportedValuesOf("timeZone") includes Factory', function () { return Intl.supportedValuesOf('timeZone').indexOf('Factory') >= 0; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
