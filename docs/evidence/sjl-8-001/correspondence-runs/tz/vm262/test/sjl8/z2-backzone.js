/*---
description: SJL-8 probe z2-backzone
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
// Z2: histories before 1970 where backzone differs from the default build.
var cases = [
  ['Europe/Amsterdam', '1850-01-01T12:00Z'], ['Europe/Amsterdam', '1937-07-01T12:00Z'], ['Europe/Amsterdam', '1939-01-15T12:00Z'],
  ['Europe/Amsterdam', '1980-07-01T12:00Z'],
  ['Atlantic/Reykjavik', '1900-01-01T12:00Z'], ['Atlantic/Reykjavik', '1960-01-15T12:00Z'], ['Atlantic/Reykjavik', '1980-01-15T12:00Z'],
  ['Europe/Stockholm', '1900-01-01T12:00Z'], ['Europe/Oslo', '1950-07-01T12:00Z'],
  ['Europe/Brussels', '1937-07-01T12:00Z'], ['Africa/Abidjan', '1960-01-15T12:00Z'],
];
cases.forEach(function (c) { t(c[0] + ' @ ' + c[1] + ' offset', function () { return at(c[1], c[0]).offset; }); });
['Europe/Amsterdam', 'Atlantic/Reykjavik', 'Europe/Brussels', 'Africa/Abidjan'].forEach(function (z) {
  t(z + ' offset changes 1800-1970', function () {
    var n = 0, cur = at('1800-01-01T00:00Z', z), end = Temporal.Instant.from('1970-01-01T00:00Z');
    while (true) { var nx = cur.getTimeZoneTransition('next'); if (nx === null || Temporal.Instant.compare(nx.toInstant(), end) >= 0) break; n++; cur = nx; }
    return n;
  });
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
