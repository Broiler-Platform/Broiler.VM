// Persian years: ISO date of 1 Farvardin, inLeapYear, days in the year, days in Esfand (M12).
function year(y) {
  var d = Temporal.PlainDate.from({ calendar: 'persian', year: y, month: 1, day: 1 }, { overflow: 'reject' });
  var esfand = Temporal.PlainDate.from({ calendar: 'persian', year: y, monthCode: 'M12', day: 1 }, { overflow: 'reject' });
  return d.withCalendar('iso8601').toString() + ' ' + d.inLeapYear + ' ' + d.daysInYear + ' ' + esfand.daysInMonth;
}
var years = [];
for (var y = 1170; y <= 1520; y++) years.push(y);
for (var y = 2980; y <= 3010; y++) years.push(y);
[-272440, -10, -1, 0, 1, 2, 3, 100, 500, 1000, 1177, 1599, 1600, 1601, 1602, 1633, 1634, 1635, 4000, 5000, 9000, 9376, 9377, 9378, 9379, 9380, 10000, 100000, 275130].forEach(function (y) { years.push(y); });
years.forEach(function (y) { t('persian ' + y, function () { return year(y); }); });
// ISO dates at .NET's bounds (0622-03-22, 9999-12-31) and at the first correction, to the calendar and back.
['0622-03-20', '0622-03-21', '0622-03-22', '0622-03-23', '9999-12-31', '+010000-01-01', '+010000-03-20', '+010000-03-21',
 '2123-03-20', '2123-03-21', '2124-03-19', '2124-03-20', '2025-03-20', '2025-03-21', '-271821-04-19', '+275760-09-13'].forEach(function (iso) {
  t('persian from ' + iso, function () {
    var c = Temporal.PlainDate.from(iso).withCalendar('persian');
    var back = Temporal.PlainDate.from({ calendar: 'persian', year: c.year, monthCode: c.monthCode, day: c.day });
    return [c.era, c.eraYear, c.year, c.monthCode, c.day, c.dayOfYear, c.daysInMonth, c.daysInYear, c.inLeapYear].join() + ' back ' + back.withCalendar('iso8601').toString();
  });
});
