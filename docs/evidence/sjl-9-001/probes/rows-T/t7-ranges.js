// SJL-9.1 sub-area (d), rows T21-T23: formatRange and formatRangeToParts. The range patterns (CLDR's
// intervalFormats through the interval formatter), the fields compared, the collapsed range, the
// sources, the argument checks. ECMA-402 3d4c558 11.3.6-11.3.7 and PartitionDateTimeRangePattern as
// e8cc03fc intl.html amends it (Table 16's fields compared in order; a collapsed result is "shared").
function d(y, mo, da, h, mi) { return Date.UTC(y, mo, da, h || 0, mi || 0); }
var pairs = [
  ['same instant', d(2024, 0, 5, 10), d(2024, 0, 5, 10)],
  ['10:00-10:30', d(2024, 0, 5, 10), d(2024, 0, 5, 10, 30)],
  ['10:00-11:00', d(2024, 0, 5, 10), d(2024, 0, 5, 11)],
  ['10:00-15:00', d(2024, 0, 5, 10), d(2024, 0, 5, 15)],
  ['Jan 5-Jan 7', d(2024, 0, 5, 10), d(2024, 0, 7, 10)],
  ['Jan 5-Feb 7', d(2024, 0, 5, 10), d(2024, 1, 7, 10)],
  ['2024-2025', d(2024, 0, 5, 10), d(2025, 0, 5, 10)],
  ['BC-AD', -62198755200000 - 86400000 * 400, d(2024, 0, 5)],
  ['reversed (Jan 7-Jan 5)', d(2024, 0, 7, 10), d(2024, 0, 5, 10)]
];
var opts = [
  {}, { dateStyle: 'medium' }, { dateStyle: 'long' }, { dateStyle: 'short', timeStyle: 'short' }, { timeStyle: 'short' },
  { year: 'numeric', month: 'short', day: 'numeric' }, { month: 'long', day: 'numeric' }, { year: 'numeric', month: 'long' },
  { weekday: 'short' }, { weekday: 'long', month: 'short', day: 'numeric' }, { hour: 'numeric' }, { hour: 'numeric', minute: 'numeric' },
  { hour: 'numeric', minute: 'numeric', hourCycle: 'h23' }, { year: 'numeric', month: 'short', day: 'numeric', hour: 'numeric', minute: 'numeric' }, { era: 'short', year: 'numeric' }
];
['en', 'de'].forEach(function (l) {
  opts.forEach(function (o) {
    var p = { timeZone: 'UTC' }; for (var k in o) p[k] = o[k];
    t(l + ' formatRange ' + show(o), function () { var f = new Intl.DateTimeFormat(l, p); return pairs.map(function (q) { return f.formatRange(q[1], q[2]); }); });
  });
});
t('en formatRangeToParts {year, month:short, day} Jan 5-Jan 7', function () { return parts(new Intl.DateTimeFormat('en', { year: 'numeric', month: 'short', day: 'numeric', timeZone: 'UTC' }).formatRangeToParts(d(2024, 0, 5), d(2024, 0, 7))); });
t('en formatRangeToParts default, same day', function () { return parts(new Intl.DateTimeFormat('en', { timeZone: 'UTC' }).formatRangeToParts(d(2024, 0, 5, 1), d(2024, 0, 5, 9))); });
t('en formatRangeToParts {hour, minute} 10:00-15:00', function () { return parts(new Intl.DateTimeFormat('en', { hour: 'numeric', minute: 'numeric', timeZone: 'UTC' }).formatRangeToParts(d(2024, 0, 5, 10), d(2024, 0, 5, 15))); });
t('en formatRangeToParts {weekday:short} Tue-Wed', function () { return parts(new Intl.DateTimeFormat('en', { weekday: 'short', timeZone: 'UTC' }).formatRangeToParts(d(2024, 0, 2), d(2024, 0, 3))); });
t('en formatRangeToParts dateStyle long timeStyle short, two days', function () { return parts(new Intl.DateTimeFormat('en', { dateStyle: 'long', timeStyle: 'short', timeZone: 'UTC' }).formatRangeToParts(d(2024, 0, 5, 10), d(2024, 0, 6, 11))); });
t('de formatRangeToParts {year, month:long, day} Jan 5-Feb 7', function () { return parts(new Intl.DateTimeFormat('de', { year: 'numeric', month: 'long', day: 'numeric', timeZone: 'UTC' }).formatRangeToParts(d(2024, 0, 5), d(2024, 1, 7))); });
t('formatRange(undefined, 0)', function () { return new Intl.DateTimeFormat('en').formatRange(undefined, 0); });
t('formatRange(0)', function () { return new Intl.DateTimeFormat('en').formatRange(0); });
t('formatRange(0, NaN)', function () { return new Intl.DateTimeFormat('en').formatRange(0, NaN); });
t('formatRange(NaN, undefined): which error first', function () { return new Intl.DateTimeFormat('en').formatRange(NaN, undefined); });
t('formatRange coercion order', function () { var log = []; try { new Intl.DateTimeFormat('en').formatRange({ valueOf: function () { log.push('x'); return 0; } }, { valueOf: function () { log.push('y'); return NaN; } }); } catch (e) { log.push(errName(e)); } return R(log.join(',')); });
t('formatRange in America/New_York across the DST change', function () { return new Intl.DateTimeFormat('en', { hour: 'numeric', minute: 'numeric', timeZoneName: 'short', timeZone: 'America/New_York' }).formatRange(Date.UTC(2024, 2, 10, 6), Date.UTC(2024, 2, 10, 8)); });
t('formatRange dayPeriod 9:00-15:00', function () { return new Intl.DateTimeFormat('en', { hour: 'numeric', dayPeriod: 'short', timeZone: 'UTC' }).formatRange(d(2024, 0, 5, 9), d(2024, 0, 5, 15)); });
t('formatRange fractionalSecondDigits', function () { return new Intl.DateTimeFormat('en', { minute: 'numeric', second: 'numeric', fractionalSecondDigits: 2, timeZone: 'UTC' }).formatRange(d(2024, 0, 5, 9) + 100, d(2024, 0, 5, 9) + 200); });
