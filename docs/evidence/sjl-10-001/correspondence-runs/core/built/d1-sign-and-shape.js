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
// D1: DurationSign, blank, negation and abs, and the record's zeros.
t('new Duration(0,0,0,0,0,0,0,0,0,-1).sign', function () { return new T.Duration(0, 0, 0, 0, 0, 0, 0, 0, 0, -1).sign; });
t('new Duration().sign', function () { return new T.Duration().sign; });
t('new Duration().blank', function () { return new T.Duration().blank; });
t('Duration("P1D").negated()', function () { return D('P1D').negated(); });
t('Object.is(Duration("P1D").negated().hours, 0)', function () { return Object.is(D('P1D').negated().hours, 0); });
t('Object.is(new Duration().negated().years, 0)', function () { return Object.is(new T.Duration().negated().years, 0); });
t('Duration("-P1DT1H").abs()', function () { return D('-P1DT1H').abs(); });
t('Duration("P1D").with({hours: -1})', function () { return D('P1D').with({ hours: -1 }); });
t('Duration("P1D").with({days: -1}).sign', function () { return D('P1D').with({ days: -1 }).sign; });
t('Duration("P1D").with({})', function () { return D('P1D').with({}); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
