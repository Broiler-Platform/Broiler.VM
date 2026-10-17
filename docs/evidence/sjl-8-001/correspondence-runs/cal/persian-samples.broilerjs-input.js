function emit(s) { print(String(s)); }
// Shared by every probe: one line per operation, "label => answer" or "label => throws Name".
function show(v) { return v === undefined ? 'undefined' : String(v); }
function t(label, fn) {
  var r;
  try { r = show(fn()); } catch (e) { r = 'throws ' + (e && e.name); }
  emit(label + ' => ' + r);
}
// Persian years where the probes found the engines differing inside 1178..3000 AP, and years either side.
[1205, 1206, 1403, 1404, 1498, 1499, 1501, 1502, 1503, 1534, 1535, 1536, 1537, 2025, 2026, 2027, 2028, 2092, 2093].forEach(function (y) {
  t('persian ' + y, function () {
    var d = Temporal.PlainDate.from({ calendar: 'persian', year: y, month: 1, day: 1 }, { overflow: 'reject' });
    return d.withCalendar('iso8601').toString() + ' ' + d.inLeapYear + ' ' + d.daysInYear;
  });
});
t('persian 1535-12-30 reject', function () { return Temporal.PlainDate.from({ calendar: 'persian', year: 1535, monthCode: 'M12', day: 30 }, { overflow: 'reject' }).withCalendar('iso8601').toString(); });
t('persian 1536-12-30 reject', function () { return Temporal.PlainDate.from({ calendar: 'persian', year: 1536, monthCode: 'M12', day: 30 }, { overflow: 'reject' }).withCalendar('iso8601').toString(); });
t('ISO 2157-03-20 to persian', function () { return Temporal.PlainDate.from('2157-03-20').withCalendar('persian').toString({ calendarName: 'never' }) + ' ' + Temporal.PlainDate.from('2157-03-20').withCalendar('persian').monthCode + '-' + Temporal.PlainDate.from('2157-03-20').withCalendar('persian').day; });
