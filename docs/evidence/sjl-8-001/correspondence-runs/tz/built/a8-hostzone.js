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
// A8: the default (host) zone and Date's local time, beside Temporal's reading of the same zone.
t('Temporal.Now.timeZoneId()', function () { return Temporal.Now.timeZoneId(); });
t('Intl.DateTimeFormat().resolvedOptions().timeZone', function () { return new Intl.DateTimeFormat().resolvedOptions().timeZone; });
t('typeof Temporal.Now.zonedDateTimeISO()', function () { return typeof Temporal.Now.zonedDateTimeISO(); });
t('new Date(2040, 6, 1, 12).getTimezoneOffset()', function () { return new Date(2040, 6, 1, 12).getTimezoneOffset(); });
t('new Date(2026, 6, 1, 12).getTimezoneOffset()', function () { return new Date(2026, 6, 1, 12).getTimezoneOffset(); });
t('America/New_York 2040-07-01 Temporal offset', function () { return at('2040-07-01T16:00Z', 'America/New_York').offset; });
t('DateTimeFormat (default zone) hour at 2040-07-01T16:00Z', function () { return new Intl.DateTimeFormat('en-US', { hour: 'numeric', hourCycle: 'h23' }).format(Date.UTC(2040, 6, 1, 16)); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
