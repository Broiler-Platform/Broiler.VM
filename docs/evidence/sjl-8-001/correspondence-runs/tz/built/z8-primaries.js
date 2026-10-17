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
// Z8: primary identifiers: equality, Temporal's timeZoneId and DateTimeFormat's resolved timeZone.
function eq(a, b) { t(a + ' equals ' + b, function () { return new Temporal.ZonedDateTime(0n, a).equals(new Temporal.ZonedDateTime(0n, b)); }); }
eq('Asia/Calcutta', 'Asia/Kolkata'); eq('Europe/Kiev', 'Europe/Kyiv'); eq('Europe/Bratislava', 'Europe/Prague');
eq('Arctic/Longyearbyen', 'Europe/Oslo'); eq('Atlantic/Jan_Mayen', 'Arctic/Longyearbyen'); eq('Europe/Busingen', 'Europe/Zurich');
eq('Europe/Vatican', 'Europe/Rome'); eq('America/Marigot', 'America/Port_of_Spain'); eq('EST5EDT', 'America/New_York');
eq('CET', 'Europe/Brussels'); eq('Etc/UTC', 'UTC'); eq('Etc/GMT', 'UTC'); eq('GMT', 'Etc/UTC'); eq('Etc/GMT-0', 'UTC'); eq('+00:00', 'UTC');
eq('Europe/Amsterdam', 'Europe/Brussels'); eq('Atlantic/Reykjavik', 'Africa/Abidjan'); eq('US/Eastern', 'America/New_York');
['Asia/Calcutta', 'Europe/Kiev', 'Europe/Bratislava', 'Arctic/Longyearbyen', 'Etc/UTC', 'Etc/GMT', 'GMT', 'US/Eastern', 'EST5EDT', 'CET', 'Etc/GMT+5', 'asia/calcutta'].forEach(function (z) {
  t('Temporal timeZoneId ' + z, function () { return new Temporal.ZonedDateTime(0n, z).timeZoneId; });
  t('DateTimeFormat resolved timeZone ' + z, function () { return new Intl.DateTimeFormat('en', { timeZone: z }).resolvedOptions().timeZone; });
});
t('Europe/Bratislava 1940-07-01 offset', function () { return at('1940-07-01T12:00Z', 'Europe/Bratislava').offset; });
t('Europe/Prague 1940-07-01 offset', function () { return at('1940-07-01T12:00Z', 'Europe/Prague').offset; });
t('EST5EDT 1945-08-01 offset', function () { return at('1945-08-01T12:00Z', 'EST5EDT').offset; });
t('EST5EDT 1960-07-01 offset', function () { return at('1960-07-01T12:00Z', 'EST5EDT').offset; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
