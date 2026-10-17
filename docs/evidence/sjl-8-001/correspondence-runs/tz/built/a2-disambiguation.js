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
// A2: possible instants and disambiguation in gaps and overlaps.
var modes = ['compatible', 'earlier', 'later', 'reject'];
function each(label, wall, zone) {
  modes.forEach(function (m) {
    t(label + ' ' + wall + ' ' + zone + ' ' + m, function () { return Temporal.PlainDateTime.from(wall).toZonedDateTime(zone, { disambiguation: m }).toString(); });
  });
}
each('gap', '2026-03-08T02:30', 'America/New_York');
each('overlap', '2026-11-01T01:30', 'America/New_York');
each('gap', '2026-03-29T02:30', 'Europe/Berlin');
each('overlap', '2026-10-25T02:30', 'Europe/Berlin');
each('overlap 30min', '2026-04-05T01:45', 'Australia/Lord_Howe');
each('gap 30min', '2026-10-04T02:15', 'Australia/Lord_Howe');
each('gap 24h', '1994-12-31T12:00', 'Pacific/Kiritimati');
each('gap 24h', '2011-12-30T12:00', 'Pacific/Apia');
each('overlap 24h', '1867-10-19T12:00', 'America/Juneau');
each('gap sub-minute', '1972-01-07T00:20:00', 'Africa/Monrovia');
each('gap after 2037', '2040-03-11T02:30', 'America/New_York');
each('overlap after 2037', '2040-11-04T01:30', 'America/New_York');
each('plain', '2026-07-01T12:00', 'America/New_York');
t('ZonedDateTime.from overlap with offset -05:00', function () { return Temporal.ZonedDateTime.from('2026-11-01T01:30-05:00[America/New_York]').toString(); });
t('ZonedDateTime.from mismatched offset (reject default)', function () { return Temporal.ZonedDateTime.from('2026-07-01T12:00+01:00[America/New_York]').toString(); });
t('ZonedDateTime.from bag gap later', function () { return Temporal.ZonedDateTime.from({ year: 2026, month: 3, day: 8, hour: 2, minute: 30, timeZone: 'America/New_York' }, { disambiguation: 'later' }).toString(); });
t('withPlainTime into gap', function () { return at('2026-03-08T12:00Z', 'America/New_York').withPlainTime('02:30').toString(); });
t('hoursInDay New York 2026-03-08', function () { return at('2026-03-08T17:00Z', 'America/New_York').hoursInDay; });
t('hoursInDay New York 2026-11-01', function () { return at('2026-11-01T17:00Z', 'America/New_York').hoursInDay; });
t('hoursInDay Apia 2011-12-29', function () { return at('2011-12-29T12:00Z', 'Pacific/Apia').hoursInDay; });
t('hoursInDay New York 2040-03-11', function () { return at('2040-03-11T17:00Z', 'America/New_York').hoursInDay; });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
