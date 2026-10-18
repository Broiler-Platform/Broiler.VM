// C38: a month code with decimal digits other than ASCII, and month codes of other shapes, in each kind of calendar.
['M٠١', 'M0١', 'M０１', 'M٠٥L', 'M01L', 'M01ʟ', 'M1L', 'M001L', 'M01LL', 'ML01', ' M01'].forEach(function (code) {
  ['iso8601', 'gregory', 'hebrew', 'chinese', 'coptic', 'persian'].forEach(function (cal) {
    k(cal + ' monthCode ' + JSON.stringify(code), function () {
      return ymd(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], monthCode: code, day: 1 })); });
  });
  k('PlainMonthDay hebrew monthCode ' + JSON.stringify(code), function () {
    return Temporal.PlainMonthDay.from({ calendar: 'hebrew', monthCode: code, day: 1 }).toString(); });
  k('PlainYearMonth iso8601 monthCode ' + JSON.stringify(code), function () {
    return Temporal.PlainYearMonth.from({ year: 2020, monthCode: code }).toString(); });
});
