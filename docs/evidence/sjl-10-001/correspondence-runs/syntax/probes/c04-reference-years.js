// C16-C20: PlainMonthDay's ISO reference date for a month code and day with no year, in every calendar: the latest
// such ISO date in 1900-1972, else the earliest in 1973-2035 (else before 1900), and for chinese and dangi the
// proposal's Table 6. Under reject the day must exist; under constrain it is constrained.
function refdate(cal, code, day, overflow) {
  var md = Temporal.PlainMonthDay.from({ calendar: cal, monthCode: code, day: day }, { overflow: overflow });
  return md.monthCode + '/' + md.day + ' ref ' + md.toString({ calendarName: 'never' });
}
['iso8601'].concat(CALS).forEach(function (cal) {
  var codes = ['M01', 'M02', 'M03', 'M04', 'M05', 'M06', 'M07', 'M08', 'M09', 'M10', 'M11', 'M12', 'M13'];
  if (cal === 'chinese' || cal === 'dangi') codes = codes.concat(['M01L', 'M02L', 'M03L', 'M04L', 'M05L', 'M06L', 'M07L', 'M08L', 'M09L', 'M10L', 'M11L', 'M12L']);
  if (cal === 'hebrew') codes = codes.concat(['M05L']);
  codes.forEach(function (code) {
    [1, 29, 30, 31].forEach(function (day) {
      k(cal + ' ' + code + ' ' + day + ' reject', function () { return refdate(cal, code, day, 'reject'); });
    });
    [30, 31, 32].forEach(function (day) {
      k(cal + ' ' + code + ' ' + day + ' constrain', function () { return refdate(cal, code, day, 'constrain'); });
    });
  });
});
// Table 6's M11L split at day 10 (2033 or 2034), and a year given with the month-day (dropped after resolving).
[['chinese', 'M11L', 10], ['chinese', 'M11L', 11], ['chinese', 'M11L', 29], ['dangi', 'M11L', 10]].forEach(function (c) {
  k(c[0] + ' ' + c[1] + ' ' + c[2] + ' reject', function () { return refdate(c[0], c[1], c[2], 'reject'); }); });
[['hebrew', 5783, 'M05L', 1], ['hebrew', 5784, 'M05L', 30], ['chinese', 2023, 'M02L', 30], ['chinese', 2024, 'M02L', 1], ['coptic', 1739, 'M13', 6],
 ['coptic', 1740, 'M13', 6], ['islamic-civil', 1445, 'M12', 30], ['islamic-civil', 1444, 'M12', 30], ['persian', 1403, 'M12', 30], ['persian', 1404, 'M12', 30],
 ['indian', 1946, 'M01', 31], ['gregory', 2021, 'M02', 29], ['iso8601', 2021, 'M02', 29]].forEach(function (c) {
  ['constrain', 'reject'].forEach(function (overflow) {
    k(c[0] + ' year ' + c[1] + ' ' + c[2] + ' ' + c[3] + ' ' + overflow, function () {
      var md = Temporal.PlainMonthDay.from({ calendar: c[0], year: c[1], monthCode: c[2], day: c[3] }, { overflow: overflow });
      return md.monthCode + '/' + md.day + ' ref ' + md.toString({ calendarName: 'never' }); });
  });
});
// A month number with no year (a TypeError where the calendar is not ISO), and a year far out of range.
[['hebrew', { month: 6, day: 1 }], ['gregory', { month: 2, day: 29 }], ['iso8601', { month: 2, day: 29 }], ['chinese', { month: 1, day: 1 }],
 ['hebrew', { year: 1e10, monthCode: 'M01', day: 1 }], ['iso8601', { year: 1e10, monthCode: 'M01', day: 1 }], ['gregory', { year: 1e10, monthCode: 'M01', day: 1 }],
 ['chinese', { year: 1e10, monthCode: 'M01', day: 1 }], ['hebrew', { year: 300000, monthCode: 'M01', day: 1 }]].forEach(function (c) {
  k(c[0] + ' ' + JSON.stringify(c[1]), function () {
    var bag = { calendar: c[0] }; for (var key in c[1]) bag[key] = c[1][key];
    var md = Temporal.PlainMonthDay.from(bag); return md.monthCode + '/' + md.day + ' ref ' + md.toString({ calendarName: 'never' }); });
});
