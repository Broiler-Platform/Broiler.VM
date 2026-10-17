// Umm al-Qura years by Hijri year: ISO date of 1 Muharram, months, inLeapYear, days in the year and the
// twelve month lengths (l = 30, s = 29); and the same year in islamic-civil for comparison.
function year(cal, y) {
  var d = Temporal.PlainDate.from({ calendar: cal, year: y, month: 1, day: 1 }, { overflow: 'reject' });
  var lengths = '';
  for (var m = 1; m <= d.monthsInYear; m++) {
    lengths += Temporal.PlainDate.from({ calendar: cal, year: y, month: m, day: 1 }, { overflow: 'reject' }).daysInMonth === 30 ? 'l' : 's';
  }
  return d.withCalendar('iso8601').toString() + ' ' + d.monthsInYear + ' ' + d.inLeapYear + ' ' + d.daysInYear + ' ' + lengths;
}
var years = [];
for (var y = 1290; y <= 1610; y++) years.push(y);
[-280000, -100, 0, 1, 500, 1000, 2000, 5000, 280000].forEach(function (y) { years.push(y); });
years.forEach(function (y) { t('islamic-umalqura ' + y, function () { return year('islamic-umalqura', y); }); });
[1298, 1299, 1300, 1600, 1601, 1602].forEach(function (y) { t('islamic-civil ' + y, function () { return year('islamic-civil', y); }); });
// The table's first and last days and their neighbours, from ISO, and back.
['1882-11-11', '1882-11-12', '1882-11-13', '2174-11-24', '2174-11-25', '2174-11-26', '1883-10-31', '1883-11-01'].forEach(function (iso) {
  t('islamic-umalqura from ' + iso, function () {
    var c = Temporal.PlainDate.from(iso).withCalendar('islamic-umalqura');
    var back = Temporal.PlainDate.from({ calendar: 'islamic-umalqura', year: c.year, monthCode: c.monthCode, day: c.day });
    return [c.era, c.eraYear, c.year, c.monthCode, c.day, c.dayOfYear, c.daysInMonth, c.daysInYear, c.inLeapYear].join() + ' back ' + back.withCalendar('iso8601').toString();
  });
});
