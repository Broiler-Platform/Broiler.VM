// Chinese and Korean: dates from fields with leap month codes, ISO dates to the calendar and back.
function f(c) { return [c.year, c.month, c.monthCode, c.day, c.dayOfYear, c.daysInMonth, c.daysInYear, c.monthsInYear, c.inLeapYear].join(); }
function fromFields(cal, y, code, day) {
  t(cal + ' from ' + y + ' ' + code + ' ' + day + ' reject', function () {
    return Temporal.PlainDate.from({ calendar: cal, year: y, monthCode: code, day: day }, { overflow: 'reject' }).withCalendar('iso8601').toString();
  });
}
[['chinese', 1900, 'M08L'], ['chinese', 1987, 'M06L'], ['chinese', 2012, 'M04L'], ['dangi', 2012, 'M03L'], ['chinese', 2012, 'M03L'], ['dangi', 2012, 'M04L'],
 ['chinese', 2017, 'M06L'], ['chinese', 2020, 'M04L'], ['chinese', 2023, 'M02L'], ['dangi', 2023, 'M02L'], ['chinese', 2025, 'M06L'], ['chinese', 2028, 'M05L'],
 ['chinese', 2033, 'M11L'], ['dangi', 2033, 'M11L'], ['chinese', 2052, 'M08L'], ['dangi', 2052, 'M08L'], ['chinese', 2099, 'M02L'], ['dangi', 2099, 'M02L'], ['dangi', 2099, 'M03L'],
 ['chinese', 2101, 'M07L'], ['dangi', 2101, 'M07L'], ['chinese', 2104, 'M05L'], ['chinese', 2104, 'M06L']].forEach(function (a) { fromFields(a[0], a[1], a[2], 1); });
fromFields('chinese', 2057, 'M08', 30); fromFields('chinese', 2057, 'M09', 1); fromFields('dangi', 2057, 'M08', 30);
['chinese', 'dangi'].forEach(function (cal) {
  ['1899-12-31', '1900-01-30', '1900-01-31', '1901-02-18', '1901-02-19', '1911-12-31', '1912-02-17', '1912-02-18', '1954-01-22', '1987-08-17',
   '2026-03-24', '2027-11-27', '2029-08-01', '2033-12-22', '2051-02-10', '2051-02-11', '2057-09-28', '2061-01-21', '2061-01-22', '2101-01-28', '2101-01-29',
   '2103-02-06', '2103-02-07', '2104-01-27', '2104-01-28', '0918-02-19', '0918-02-18', '-271821-04-19', '-271000-01-01', '+275760-09-13', '+275000-01-01'].forEach(function (iso) {
    t(cal + ' ' + iso, function () {
      var c = Temporal.PlainDate.from(iso).withCalendar(cal);
      var back;
      try { back = Temporal.PlainDate.from({ calendar: cal, year: c.year, monthCode: c.monthCode, day: c.day }, { overflow: 'reject' }).withCalendar('iso8601').toString(); }
      catch (e) { back = 'throws ' + e.name; }
      return f(c) + ' back ' + back;
    });
  });
});
