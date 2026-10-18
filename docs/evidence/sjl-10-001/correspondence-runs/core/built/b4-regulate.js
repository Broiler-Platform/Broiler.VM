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
// B4: RegulateISODate and RegulateTime from property bags, constrain and reject.
var rej = { overflow: 'reject' };
t('PlainDate.from({year: 2021, month: 2, day: 31})', function () { return PD({ year: 2021, month: 2, day: 31 }); });
t('PlainDate.from({year: 2021, month: 13, day: 1})', function () { return PD({ year: 2021, month: 13, day: 1 }); });
t('PlainDate.from({year: 2021, month: 2, day: 31}, reject)', function () { return T.PlainDate.from({ year: 2021, month: 2, day: 31 }, rej); });
t('PlainDate.from({year: 2021, month: 0, day: 1})', function () { return PD({ year: 2021, month: 0, day: 1 }); });
t('PlainDate.from({year: 2021, month: 1, day: 0})', function () { return PD({ year: 2021, month: 1, day: 0 }); });
t('PlainTime.from({hour: 25, minute: 61, second: 61, millisecond: 1000})', function () { return PT({ hour: 25, minute: 61, second: 61, millisecond: 1000 }); });
t('PlainTime.from({hour: 25}, reject)', function () { return T.PlainTime.from({ hour: 25 }, rej); });
t('PlainTime.from({hour: -1, minute: -1})', function () { return PT({ hour: -1, minute: -1 }); });
t('PlainTime.from({hour: -1}, reject)', function () { return T.PlainTime.from({ hour: -1 }, rej); });
t('PlainTime.from({microsecond: 1000, nanosecond: 1000})', function () { return PT({ microsecond: 1000, nanosecond: 1000 }); });
t('PlainDateTime.from({year: 2020, month: 1, day: 1, second: 60})', function () { return PDT({ year: 2020, month: 1, day: 1, second: 60 }); });
t('PlainDateTime.from({year: 2020, month: 1, day: 1, second: 60}, reject)', function () { return T.PlainDateTime.from({ year: 2020, month: 1, day: 1, second: 60 }, rej); });
t('PlainYearMonth.from({year: 2020, month: 13})', function () { return PYM({ year: 2020, month: 13 }); });
t('PlainYearMonth.from({year: 2020, month: 13}, reject)', function () { return T.PlainYearMonth.from({ year: 2020, month: 13 }, rej); });
t('PlainDateTime("2020-02-29T12:00").with({year: 2021})', function () { return PDT('2020-02-29T12:00').with({ year: 2021 }); });
t('PlainDateTime("2020-02-29T12:00").with({year: 2021}, reject)', function () { return PDT('2020-02-29T12:00').with({ year: 2021 }, rej); });
t('PlainTime("12:00").with({minute: 75})', function () { return PT('12:00').with({ minute: 75 }); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
