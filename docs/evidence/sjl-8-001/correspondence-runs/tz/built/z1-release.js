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
// Z1: offsets where tzdb 2025b and 2026e differ (NEWS of 2025c to 2026e), with controls.
var cases = [
  ['America/Vancouver', '2026-07-01T12:00Z'], ['America/Vancouver', '2027-01-15T12:00Z'],
  ['America/Edmonton', '2027-01-15T12:00Z'], ['America/Winnipeg', '2027-01-15T12:00Z'],
  ['America/Inuvik', '2027-01-15T12:00Z'],
  ['Africa/Casablanca', '2026-07-01T12:00Z'], ['Africa/Casablanca', '2027-01-15T12:00Z'],
  ['Europe/Chisinau', '2026-03-29T00:30Z'], ['Europe/Chisinau', '2026-10-25T00:30Z'],
  ['Europe/Dublin', '1925-09-25T12:00Z'], ['America/Bogota', '1992-05-02T12:00Z'],
];
cases.forEach(function (c) { t(c[0] + ' @ ' + c[1] + ' offset', function () { return at(c[1], c[0]).offset; }); });
t('America/Vancouver next transition after 2026-07-01', function () { return at('2026-07-01T12:00Z', 'America/Vancouver').getTimeZoneTransition('next'); });
t('Africa/Casablanca next transition after 2026-10-01', function () { return at('2026-10-01T12:00Z', 'Africa/Casablanca').getTimeZoneTransition('next'); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
