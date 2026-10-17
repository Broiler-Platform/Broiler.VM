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
// A7: the Etc/GMT zones.
['Etc/GMT+5', 'etc/gmt+5', 'ETC/GMT+5', 'Etc/GMT-14', 'Etc/GMT+12', 'Etc/GMT-0', 'Etc/GMT0', 'Etc/GMT+0', 'GMT+0', 'GMT-0', 'GMT0', 'Etc/GMT'].forEach(function (n) {
  t('ZonedDateTime ctor ' + n, function () { var z = new Temporal.ZonedDateTime(0n, n); return z.timeZoneId + ' ' + z.offset; });
  t('DateTimeFormat ' + n, function () { return new Intl.DateTimeFormat('en', { timeZone: n }).resolvedOptions().timeZone; });
});
t('DateTimeFormat Etc/GMT+5 hour at epoch', function () { return new Intl.DateTimeFormat('en-US', { timeZone: 'Etc/GMT+5', hour: 'numeric', hourCycle: 'h23' }).format(0); });
t('DateTimeFormat Etc/GMT-14 hour at epoch', function () { return new Intl.DateTimeFormat('en-US', { timeZone: 'Etc/GMT-14', hour: 'numeric', hourCycle: 'h23' }).format(0); });
t('Etc/GMT+5 equals -05:00', function () { return new Temporal.ZonedDateTime(0n, 'Etc/GMT+5').equals(new Temporal.ZonedDateTime(0n, '-05:00')); });
t('Etc/GMT+5 next transition', function () { return new Temporal.ZonedDateTime(0n, 'Etc/GMT+5').getTimeZoneTransition('next'); });
t('Etc/GMT+5 1850 offset', function () { return at('1850-01-01T00:00Z', 'Etc/GMT+5').offset; });
t('Intl.supportedValuesOf lists Etc/GMT+5', function () { return Intl.supportedValuesOf('timeZone').indexOf('Etc/GMT+5') >= 0; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
