/*---
description: SJL-8 probe a4-transitions
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
// A4: next and previous transitions.
function tr(label, instant, zone, dir) { t(label + ' ' + zone + ' ' + dir + ' from ' + instant, function () { var r = at(instant, zone).getTimeZoneTransition(dir); return r === null ? null : r.toString(); }); }
tr('dst', '2026-07-01T12:00Z', 'America/New_York', 'next');
tr('dst', '2026-07-01T12:00Z', 'America/New_York', 'previous');
tr('at a transition', '2026-03-08T07:00:00Z', 'America/New_York', 'next');
tr('at a transition', '2026-03-08T07:00:00Z', 'America/New_York', 'previous');
tr('half a second after', '2026-03-08T07:00:00.5Z', 'America/New_York', 'previous');
tr('2037', '2037-07-01T12:00Z', 'America/New_York', 'next');
tr('after 2037', '2037-12-01T12:00Z', 'America/New_York', 'next');
tr('after 2037', '2040-07-01T12:00Z', 'America/New_York', 'previous');
tr('far', '2100-01-01T00:00Z', 'America/New_York', 'previous');
tr('after 2037', '2037-12-01T12:00Z', 'America/Santiago', 'next');
tr('after 2037', '2040-07-01T12:00Z', 'Europe/Berlin', 'next');
tr('flag only (BST as standard 1968-1971)', '1968-07-01T12:00Z', 'Europe/London', 'next');
tr('flag only', '1971-07-01T12:00Z', 'Europe/London', 'previous');
tr('none since 1947', '2000-01-01T00:00Z', 'Asia/Riyadh', 'next');
tr('none since 1947', '2000-01-01T00:00Z', 'Asia/Riyadh', 'previous');
tr('first', '1800-01-01T00:00Z', 'America/New_York', 'next');
tr('before first', '1800-01-01T00:00Z', 'America/New_York', 'previous');
tr('30 min', '2026-01-01T00:00Z', 'Australia/Lord_Howe', 'next');
tr('fixed', '2026-01-01T00:00Z', 'Etc/GMT+5', 'next');
tr('fixed', '2026-01-01T00:00Z', '+05:30', 'next');
tr('fixed', '2026-01-01T00:00Z', 'UTC', 'previous');
tr('lower limit', '-271821-04-20T00:00Z', 'America/New_York', 'previous');
tr('upper limit', '+275760-09-13T00:00Z', 'America/New_York', 'next');
tr('near upper limit', '+275760-09-01T00:00Z', 'America/New_York', 'previous');
t('getTimeZoneTransition({direction:"next"}) object form', function () { return at('2026-07-01T12:00Z', 'Europe/Berlin').getTimeZoneTransition({ direction: 'next' }).toString(); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
