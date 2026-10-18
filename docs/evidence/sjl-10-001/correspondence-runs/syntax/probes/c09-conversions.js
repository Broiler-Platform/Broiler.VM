// C35-C37: the reference dates conversions write: toPlainYearMonth (the first day of the calendar month), toPlainMonthDay
// (the month-day's reference year, and its calendar), PlainMonthDay#toPlainDate and PlainYearMonth#toPlainDate (the year
// read in the calendar).
['iso8601'].concat(CALS).forEach(function (cal) {
  var d = Temporal.PlainDate.from('2023-03-15').withCalendar(cal);
  t(cal + ' PlainDate#toPlainYearMonth', function () { return d.toPlainYearMonth().toString({ calendarName: 'always' }); });
  t(cal + ' PlainDate#toPlainMonthDay', function () { var md = d.toPlainMonthDay(); return md.calendarId + ' ' + md.monthCode + '/' + md.day + ' ' + md.toString({ calendarName: 'always' }); });
  t(cal + ' PlainDateTime#toPlainDate#toPlainYearMonth', function () { return Temporal.PlainDateTime.from('2023-03-15T12:00').withCalendar(cal).toPlainDate().toPlainYearMonth().toString({ calendarName: 'always' }); });
  t(cal + ' ZonedDateTime#toPlainDate#toPlainMonthDay', function () { return Temporal.ZonedDateTime.from('2023-03-15T12:00[UTC]').withCalendar(cal).toPlainDate().toPlainMonthDay().toString({ calendarName: 'always' }); });
  t(cal + ' PlainMonthDay#toPlainDate({year: the calendar year of 2001-06-01})', function () {
    return Temporal.PlainMonthDay.from({ calendar: cal, monthCode: 'M02', day: 10 }).toPlainDate({ year: Temporal.PlainDate.from('2001-06-01').withCalendar(cal).year }).toString({ calendarName: 'always' }); });
  t(cal + ' PlainYearMonth#toPlainDate({day: 10})', function () { return d.toPlainYearMonth().toPlainDate({ day: 10 }).toString({ calendarName: 'always' }); });
  t(cal + ' PlainYearMonth.from(fields) reference day', function () { return Temporal.PlainYearMonth.from({ calendar: cal, year: d.year, monthCode: d.monthCode }).toString({ calendarName: 'always' }); });
  t(cal + ' PlainYearMonth#with({monthCode: M01})', function () { return d.toPlainYearMonth().with({ monthCode: 'M01' }).toString({ calendarName: 'always' }); });
  t(cal + ' PlainMonthDay#with({day: 1})', function () { return d.toPlainMonthDay().with({ day: 1 }).toString({ calendarName: 'always' }); });
});
