// Shared prelude (SJL-8's form): Broiler.JS's shell has print; Broiler.VM's test262 realm has none, so
// the lines are collected and handed back in one uncaught Error the conformance root prints.
var __lines = [];
var __host = typeof __vmTest === 'undefined' && typeof print === 'function';
function out(s) { s = String(s); if (__host) print(s); else __lines.push(s); }
function show(v) {
  if (v === null) return 'null';
  if (v === undefined) return 'undefined';
  if (typeof v === 'bigint') return v + 'n';
  if (typeof v === 'string') return v;
  if (typeof v === 'number') return Object.is(v, -0) ? '-0' : String(v);
  if (Array.isArray(v)) return '[' + v.map(show).join(', ') + ']';
  if (v && typeof v === 'object' && typeof v.toString === 'function' && v[Symbol.toStringTag]) return v[Symbol.toStringTag] + ' ' + v.toString();
  return String(v);
}
function t(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + ((e && e.constructor && e.constructor.name) || typeof e) + (e && e.name && e.constructor && e.name !== e.constructor.name ? '(' + e.name + ')' : '') + ': ' + (e && e.message); }
  out(label + ' => ' + r);
}
var T = Temporal;
function D(x) { return T.Duration.from(x); }
function PD(x) { return T.PlainDate.from(x); }
function PDT(x) { return T.PlainDateTime.from(x); }
function PT(x) { return T.PlainTime.from(x); }
function PYM(x) { return T.PlainYearMonth.from(x); }
function I(x) { return T.Instant.from(x); }
function Z(x) { return T.ZonedDateTime.from(x); }
// F8: PlainYearMonth until and since (the first of each month, years and months only).
t('2019-05 until 2021-03 largest month', function () { return PYM('2019-05').until('2021-03', { largestUnit: 'month' }); });
t('2019-05 until 2021-03 smallest year halfExpand', function () { return PYM('2019-05').until('2021-03', { smallestUnit: 'year', roundingMode: 'halfExpand' }); });
t('2021-03 since 2019-05 smallest year halfExpand', function () { return PYM('2021-03').since('2019-05', { smallestUnit: 'year', roundingMode: 'halfExpand' }); });
t('2019-05 since 2021-03 smallest year floor', function () { return PYM('2019-05').since('2021-03', { smallestUnit: 'year', roundingMode: 'floor' }); });
t('2019-05 until 2021-03 smallest month increment 5 halfExpand', function () { return PYM('2019-05').until('2021-03', { smallestUnit: 'month', roundingIncrement: 5, roundingMode: 'halfExpand' }); });
t('-271821-04 until -271821-05', function () { return PYM('-271821-04').until('-271821-05'); });
t('-271821-05 until -271821-04', function () { return PYM('-271821-05').until('-271821-04'); });
t('-271821-04 until -271821-04', function () { return PYM('-271821-04').until('-271821-04'); });
t('+275760-09 since +275760-08', function () { return PYM('+275760-09').since('+275760-08'); });
t('+275760-08 until +275760-09 smallest year ceil', function () { return PYM('+275760-08').until('+275760-09', { smallestUnit: 'year', roundingMode: 'ceil' }); });
t('2019-05 until 2021-03 other calendar', function () { return PYM('2019-05').until(PYM('2021-03').toPlainDate({ day: 1 }).withCalendar('gregory').toPlainYearMonth()); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
