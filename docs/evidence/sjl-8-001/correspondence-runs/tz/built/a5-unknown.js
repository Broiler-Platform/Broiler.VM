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
// A5: unknown zone names on every path that takes a zone.
var names = ['Mars/Olympus', 'Etc/Unknown', '', 'America/New York', 'Europe/Berlin ', 'EST5', 'Etc/GMT+13', 'Etc/GMT-15', 'Etc/GMT+05'];
names.forEach(function (n) {
  var l = JSON.stringify(n);
  t('ZonedDateTime ctor ' + l, function () { return new Temporal.ZonedDateTime(0n, n).toString(); });
  t('ZonedDateTime.from annotation ' + l, function () { return Temporal.ZonedDateTime.from('2020-01-01T00:00[' + n + ']').toString(); });
  t('Instant.toZonedDateTimeISO ' + l, function () { return Temporal.Instant.from('2020-01-01T00:00Z').toZonedDateTimeISO(n).toString(); });
  t('Instant.toString timeZone ' + l, function () { return Temporal.Instant.from('2020-01-01T00:00Z').toString({ timeZone: n }); });
  t('PlainDate.toZonedDateTime ' + l, function () { return Temporal.PlainDate.from('2020-01-01').toZonedDateTime(n).toString(); });
  t('Now.zonedDateTimeISO ' + l, function () { return typeof Temporal.Now.zonedDateTimeISO(n); });
  t('Duration.round relativeTo bag ' + l, function () { return Temporal.Duration.from({ hours: 25 }).round({ largestUnit: 'day', relativeTo: { year: 2020, month: 1, day: 1, timeZone: n } }).toString(); });
  t('DateTimeFormat ' + l, function () { return new Intl.DateTimeFormat('en', { timeZone: n }).resolvedOptions().timeZone; });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
