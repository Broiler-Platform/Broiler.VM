/*---
description: SJL-8 probe z7-lookup
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
// Z7: identifier lookup: ASCII case, non-ASCII characters that invariant upper-casing folds, and junk.
var names = ['Europe/Berlin', 'europe/berlin', 'EUROPE/BERLIN', 'eUrOpE/bErLiN',
  'Europe/Berlın', 'America/Loſ_Angeles', 'Europe/Kıev', 'Europe/Berlin ', ' Europe/Berlin', 'Europe/Berlin\u0000',
  'Europe//Berlin', 'Europe/Berlin/', 'utc', 'Utc', 'etc/utc', 'gmt', 'UT', 'Z', 'z', 'İstanbul', 'Europe/İstanbul', 'Europe/Istanbul'];
names.forEach(function (n) {
  var label = JSON.stringify(n);
  t('ZonedDateTime ctor ' + label, function () { return new Temporal.ZonedDateTime(0n, n).timeZoneId; });
  t('DateTimeFormat ' + label, function () { return new Intl.DateTimeFormat('en', { timeZone: n }).resolvedOptions().timeZone; });
});
t('ZonedDateTime.from annotation [Europe/Berl\\u0131n]', function () { return Temporal.ZonedDateTime.from('2020-01-01T00:00[Europe/Berlın]').timeZoneId; });
t('ZonedDateTime.from annotation [EUROPE/BERLIN]', function () { return Temporal.ZonedDateTime.from('2020-01-01T00:00[EUROPE/BERLIN]').timeZoneId; });
t('Instant.toZonedDateTimeISO("America/Lo\\u017F_Angeles")', function () { return Temporal.Instant.from('2020-01-01T00:00Z').toZonedDateTimeISO('America/Loſ_Angeles').toString(); });
t('ISO string as Temporal zone "2020-01-01T00:00[Europe/Berlin]"', function () { return Temporal.Instant.from('2020-01-01T00:00Z').toZonedDateTimeISO('2020-01-01T00:00[Europe/Berlin]').timeZoneId; });
t('ISO string as DateTimeFormat timeZone "2020-01-01T00:00[Europe/Berlin]"', function () { return new Intl.DateTimeFormat('en', { timeZone: '2020-01-01T00:00[Europe/Berlin]' }).resolvedOptions().timeZone; });
t('ISO string as DateTimeFormat timeZone "2020-01-01T00:00Z"', function () { return new Intl.DateTimeFormat('en', { timeZone: '2020-01-01T00:00Z' }).resolvedOptions().timeZone; });
t('ISO string as ZonedDateTime ctor zone "2020-01-01T00:00[Europe/Berlin]"', function () { return new Temporal.ZonedDateTime(0n, '2020-01-01T00:00[Europe/Berlin]').timeZoneId; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
