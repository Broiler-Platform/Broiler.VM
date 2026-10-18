// C28-C30: the table calendars at their tables' edges (SJL-8.1's rows K1 to K13 hold these calendars' years in depth;
// these confirm the arithmetic over them at the seams): chinese and dangi 1899-1901, 2050-2051, 2100-2103; Umm
// al-Qura 1299-1301 and 1599-1601 AH; Persian 1501-1502 and 2987-2988 AP. New year, months, leap month, length.
function yearInfo(cal, y) {
  var first = Temporal.PlainDate.from({ calendar: cal, year: y, month: 1, day: 1 }), r = [];
  for (var m = 1; m <= first.monthsInYear; m++) r.push(Temporal.PlainDate.from({ calendar: cal, year: y, month: m, day: 1 }).daysInMonth === 30 ? 'l' : 's');
  var leap = []; for (var n = 1; n <= first.monthsInYear; n++) { var c = Temporal.PlainDate.from({ calendar: cal, year: y, month: n, day: 1 }).monthCode; if (c.length === 4) leap.push(c + '@' + n); }
  return first.toString({ calendarName: 'never' }) + ' ' + first.daysInYear + ' ' + first.monthsInYear + ' ' + r.join('') + ' ' + leap.join(',');
}
[['chinese', [1899, 1900, 1901, 2050, 2051, 2100, 2101, 2102, 2103]], ['dangi', [1899, 1900, 1901, 2050, 2051, 2100, 2101, 2102, 2103]],
 ['islamic-umalqura', [1299, 1300, 1301, 1599, 1600, 1601]], ['persian', [1501, 1502, 2987, 2988]]].forEach(function (c) {
  c[1].forEach(function (y) { t(c[0] + ' ' + y, function () { return yearInfo(c[0], y); }); });
});
// The arithmetic across the seams: a year added, a month added, until() in months.
[['chinese', '1900-01-15', { years: 1 }], ['chinese', '2102-06-01', { years: 1 }], ['chinese', '2102-06-01', { months: 13 }],
 ['islamic-umalqura', '2173-12-01', { months: 1 }], ['islamic-umalqura', '1882-11-01', { months: 1 }], ['persian', '2123-03-01', { years: 1 }]
].forEach(function (c) {
  t(c[0] + ' ' + c[1] + ' + ' + JSON.stringify(c[2]), function () {
    var d = Temporal.PlainDate.from(c[1]).withCalendar(c[0]); var e = d.add(c[2]);
    return e.year + '/' + e.monthCode + '/' + e.day + ' ' + e.toString({ calendarName: 'never' }) + ' until months ' + d.until(e, { largestUnit: 'months' }).toString(); });
});
