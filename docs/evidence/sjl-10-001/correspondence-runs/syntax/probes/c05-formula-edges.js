// C21-C27: the formula calendars at their edges: the ISO limits, years 0 and 1, each calendar's epoch, its new years
// around 2020, leap days, the Coptic and Ethiopic thirteenth month, the Indian leap Chaitra, the tabular Islamic
// leap year, Hebrew years of each length. Fields read, and the date rebuilt from its fields.
var F = ['gregory', 'buddhist', 'roc', 'japanese', 'coptic', 'ethiopic', 'ethioaa', 'indian', 'islamic-civil', 'islamic-tbla', 'hebrew'];
function fields(d) {
  return [d.era, d.eraYear, d.year, d.month, d.monthCode, d.day, d.dayOfYear, d.daysInMonth, d.daysInYear, d.monthsInYear, d.inLeapYear,
    d.weekOfYear, d.yearOfWeek].join(' ');
}
['-271821-04-19', '-271821-04-20', '+275760-09-13', '+275760-09-12', '0000-01-01', '0000-12-31', '0001-01-01', '-000001-12-31',
 '0284-08-29', '0008-08-27', '-005492-07-17', '0622-07-16', '0622-07-15', '0079-03-22', '-003761-10-07', '2019-09-11', '2019-09-12', '2020-09-11',
 '2020-09-12', '2023-09-11', '2023-09-12', '2024-03-20', '2024-03-21', '2024-03-22', '2024-07-07', '2024-07-08', '2024-06-06', '2024-06-07',
 '2024-10-02', '2024-10-03', '2025-09-22', '2025-09-23', '2023-03-22', '2022-03-22', '2026-09-12', '2027-10-02', '2028-09-21', '2031-09-18',
 '2029-09-10', '2010-09-09', '1900-02-28', '1900-03-01', '2100-02-28', '2100-03-01', '1582-10-14', '1582-10-15'].forEach(function (iso) {
  F.forEach(function (cal) {
    t(cal + ' ' + iso, function () { return fields(Temporal.PlainDate.from(iso).withCalendar(cal)); });
  });
});
// The round trip: fields back to the ISO date, at the ISO limits.
F.forEach(function (cal) {
  ['-271821-04-19', '+275760-09-13'].forEach(function (iso) {
    t(cal + ' round trip ' + iso, function () {
      var d = Temporal.PlainDate.from(iso).withCalendar(cal);
      return Temporal.PlainDate.from({ calendar: cal, year: d.year, monthCode: d.monthCode, day: d.day }).toString(); });
    t(cal + ' one day past ' + iso, function () {
      var d = Temporal.PlainDate.from(iso).withCalendar(cal);
      return Temporal.PlainDate.from({ calendar: cal, year: d.year, monthCode: d.monthCode, day: d.day + (iso[0] === '+' ? 1 : -1) }, { overflow: 'reject' }).toString(); });
  });
});
// Hebrew years of each of the six lengths, and the length of each month in them.
[5781, 5782, 5783, 5784, 5785, 5786, 5806, 5795].forEach(function (y) {
  t('hebrew ' + y + ' months', function () {
    var first = Temporal.PlainDate.from({ calendar: 'hebrew', year: y, month: 1, day: 1 }), r = [];
    for (var m = 1; m <= first.monthsInYear; m++) r.push(Temporal.PlainDate.from({ calendar: 'hebrew', year: y, month: m, day: 1 }).daysInMonth);
    return first.daysInYear + ' ' + first.toString({ calendarName: 'never' }) + ' ' + r.join(','); });
});
