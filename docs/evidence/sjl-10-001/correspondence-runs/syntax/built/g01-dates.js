// Shared prelude (SJL-8.1's, with SJL-10.1's helpers added at the end): Broiler.JS's shell has print; Broiler.VM's
// test262 realm has none, so the lines are collected and handed back in one uncaught Error the
// conformance root prints.
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
// The error's type alone, for rows that compare acceptance and refusal (messages differ by design).
function k(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + ((e && e.constructor && e.constructor.name) || typeof e); }
  out(label + ' => ' + r);
}
// SJL-10.1: every from() a grammar goal reaches, each answering its value's toString().
var FROM = {
  PlainDate: function (s) { return Temporal.PlainDate.from(s).toString(); },
  PlainDateTime: function (s) { return Temporal.PlainDateTime.from(s).toString(); },
  PlainTime: function (s) { return Temporal.PlainTime.from(s).toString(); },
  PlainYearMonth: function (s) { return Temporal.PlainYearMonth.from(s).toString(); },
  PlainMonthDay: function (s) { return Temporal.PlainMonthDay.from(s).toString(); },
  ZonedDateTime: function (s) { return Temporal.ZonedDateTime.from(s).toString(); },
  Instant: function (s) { return Temporal.Instant.from(s).toString(); },
  Duration: function (s) { return Temporal.Duration.from(s).toString(); }
};
// Runs one string through the named from()s; a refusal shows the error's type only.
function each(types, s) {
  types.forEach(function (type) { k(type + '.from(' + JSON.stringify(s) + ')', function () { return FROM[type](s); }); });
}
var PLAIN = ['PlainDate', 'PlainDateTime', 'PlainTime', 'PlainYearMonth', 'PlainMonthDay'];
var ALL7 = ['PlainDate', 'PlainDateTime', 'PlainTime', 'PlainYearMonth', 'PlainMonthDay', 'ZonedDateTime', 'Instant'];
// G1-G4: the date part. Year forms (four digits, six with an ASCII sign, -000000 refused), extended and basic
// separators and their mix, month and day ranges, leap days, the representable limits, signs other than ASCII,
// decimal digits other than ASCII, surrounding space. Each string alone, then with a time, through every from().
var dates = ['2020-01-01', '20200101', '2020-0101', '202001-01', '+002020-01-01', '+0020200101', '-002020-01-01',
  '-000000-01-01', '+000000-01-01', '-0000000101', '+0020-01-01', '02020-01-01', '020-01-01', '2020-1-01', '2020-01-1',
  '2020-00-01', '2020-13-01', '2020-01-00', '2020-01-32', '2020-02-29', '2021-02-29', '2020-02-30', '2020-04-31',
  '1900-02-29', '2000-02-29', '−002020-01-01', '+999999-12-31', '-999999-01-01', '-271821-04-19', '-271821-04-18',
  '+275760-09-13', '+275760-09-14', '0000-01-01', '-000001-12-31', ' 2020-01-01', '2020-01-01 ',
  '２０２０-01-01', '٢٠٢٠-٠١-٠١', '2020-01-01\u0000', ''];
dates.forEach(function (d) {
  each(PLAIN, d);
  each(['ZonedDateTime'], d + '[UTC]');
  each(['Instant'], d + 'T00:00Z');
});
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
