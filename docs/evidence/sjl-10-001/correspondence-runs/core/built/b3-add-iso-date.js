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
// B3: AddISODate (BalanceISOYearMonth, then RegulateISODate, then AddDaysToISODate) under both overflows.
var rej = { overflow: 'reject' };
t('PlainDate("2020-01-31").add({months: 1})', function () { return PD('2020-01-31').add({ months: 1 }); });
t('PlainDate("2020-01-31").add({months: 1}, reject)', function () { return PD('2020-01-31').add({ months: 1 }, rej); });
t('PlainDate("2020-02-29").add({years: 1})', function () { return PD('2020-02-29').add({ years: 1 }); });
t('PlainDate("2020-02-29").add({years: 1}, reject)', function () { return PD('2020-02-29').add({ years: 1 }, rej); });
t('PlainDate("2020-03-31").subtract({months: 1})', function () { return PD('2020-03-31').subtract({ months: 1 }); });
t('PlainDate("2020-01-31").add({months: 1, days: 1})', function () { return PD('2020-01-31').add({ months: 1, days: 1 }); });
t('PlainDate("2019-01-31").add({years: 1, months: 1})', function () { return PD('2019-01-31').add({ years: 1, months: 1 }); });
t('PlainDate("2020-01-01").add({weeks: 2, days: 3})', function () { return PD('2020-01-01').add({ weeks: 2, days: 3 }); });
t('PlainDate("2020-03-15").subtract({months: 15})', function () { return PD('2020-03-15').subtract({ months: 15 }); });
t('PlainDate("2020-01-01").add({months: 1200000})', function () { return PD('2020-01-01').add({ months: 1200000 }); });
t('PlainDate("2020-01-01").add({days: 1e11})', function () { return PD('2020-01-01').add({ days: 1e11 }); });
t('PlainDate("2020-01-01").add({years: 1e9 + 1})', function () { return PD('2020-01-01').add({ years: 1e9 + 1 }); });
t('PlainDateTime("2020-01-31T10:00").add({months: 1}, reject)', function () { return PDT('2020-01-31T10:00').add({ months: 1 }, rej); });
t('PlainDateTime("2020-01-31T10:00").add({months: 1})', function () { return PDT('2020-01-31T10:00').add({ months: 1 }); });
t('ZonedDateTime("2020-01-31T12:00[UTC]").add({months: 1}, reject)', function () { return Z('2020-01-31T12:00[UTC]').add({ months: 1 }, rej); });
t('PlainDate("2020-01-01").add({months: 1}, {overflow: "bogus"})', function () { return PD('2020-01-01').add({ months: 1 }, { overflow: 'bogus' }); });
t('PlainDate("2020-01-01").add({months: 1}, null)', function () { return PD('2020-01-01').add({ months: 1 }, null); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
