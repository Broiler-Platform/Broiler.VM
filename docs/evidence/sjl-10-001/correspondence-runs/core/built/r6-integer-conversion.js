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
// R6: integers too large for the engines' fields, given to constructors and property bags.
t('new PlainDate(2**31, 1, 1)', function () { return new T.PlainDate(Math.pow(2, 31), 1, 1); });
t('new PlainDate(2**32 + 2020, 1, 1)', function () { return new T.PlainDate(Math.pow(2, 32) + 2020, 1, 1); });
t('new PlainDate(2**53, 1, 1)', function () { return new T.PlainDate(Math.pow(2, 53), 1, 1); });
t('new PlainDate(2020, 2**32 + 1, 1)', function () { return new T.PlainDate(2020, Math.pow(2, 32) + 1, 1); });
t('new PlainDate(2020, 1, 2**32 + 1)', function () { return new T.PlainDate(2020, 1, Math.pow(2, 32) + 1); });
t('new PlainDateTime(2020, 1, 1, 2**32)', function () { return new T.PlainDateTime(2020, 1, 1, Math.pow(2, 32)); });
t('new PlainYearMonth(1e20, 1)', function () { return new T.PlainYearMonth(1e20, 1); });
t('new PlainMonthDay(1, 1, undefined, 2**32 + 1972)', function () { return new T.PlainMonthDay(1, 1, undefined, Math.pow(2, 32) + 1972); });
t('new PlainTime(2**32)', function () { return new T.PlainTime(Math.pow(2, 32)); });
t('new PlainTime(2**32 + 1)', function () { return new T.PlainTime(Math.pow(2, 32) + 1); });
t('PlainDate.from({year: 2**32 + 2020, month: 1, day: 1})', function () { return PD({ year: Math.pow(2, 32) + 2020, month: 1, day: 1 }); });
t('PlainDate.from({year: 1e20, month: 1, day: 1})', function () { return PD({ year: 1e20, month: 1, day: 1 }); });
t('PlainDate.from({year: 2020, month: 2**32 + 1, day: 1})', function () { return PD({ year: 2020, month: Math.pow(2, 32) + 1, day: 1 }); });
t('PlainDate.from({year: 2020, month: 1, day: 2**32 + 1})', function () { return PD({ year: 2020, month: 1, day: Math.pow(2, 32) + 1 }); });
t('PlainTime.from({hour: 2**32 + 1})', function () { return PT({ hour: Math.pow(2, 32) + 1 }); });
t('PlainTime.from({hour: -(2**32) + 1})', function () { return PT({ hour: -Math.pow(2, 32) + 1 }); });
t('PlainTime.from({hour: 2**32 + 1}, reject)', function () { return T.PlainTime.from({ hour: Math.pow(2, 32) + 1 }, { overflow: 'reject' }); });
t('PlainDateTime("2020-01-01T00:00").with({year: 2**32 + 2020})', function () { return PDT('2020-01-01T00:00').with({ year: Math.pow(2, 32) + 2020 }); });
t('PlainDateTime("2020-01-01T00:00").with({hour: 2**32 + 5})', function () { return PDT('2020-01-01T00:00').with({ hour: Math.pow(2, 32) + 5 }); });
t('PlainYearMonth.from({year: 2020, month: 2**32 + 1})', function () { return PYM({ year: 2020, month: Math.pow(2, 32) + 1 }); });
t('PlainDate("2020-01-01").with({day: 1e20})', function () { return PD('2020-01-01').with({ day: 1e20 }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
