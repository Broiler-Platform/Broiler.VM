// C4-C9: month codes, valid and invalid, in each calendar and in iso8601, under both overflow settings, through
// PlainDate.from with a year (the leap year LEAPYEAR names), and through PlainYearMonth.from.
var codes = ['M00', 'M00L', 'M01', 'M01L', 'M02L', 'M05L', 'M06L', 'M12', 'M12L', 'M13', 'M13L', 'M14', 'M99', 'm01', 'M1', 'M001',
  'M01l', 'L01M', 'M01 ', ''];
['iso8601'].concat(CALS).forEach(function (cal) {
  codes.forEach(function (code) {
    ['constrain', 'reject'].forEach(function (overflow) {
      k(cal + ' ' + LEAPYEAR[cal] + ' ' + JSON.stringify(code) + ' ' + overflow, function () {
        return ymd(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], monthCode: code, day: 1 }, { overflow: overflow })); });
    });
  });
});
// A leap month code in a year without that leap month: Hebrew 5783, Chinese and Korean 2024.
[['hebrew', 5783, 'M05L'], ['chinese', 2024, 'M02L'], ['dangi', 2024, 'M02L'], ['chinese', 2023, 'M03L'], ['chinese', 2023, 'M02L']
].forEach(function (c) {
  ['constrain', 'reject'].forEach(function (overflow) {
    k(c[0] + ' ' + c[1] + ' ' + c[2] + ' ' + overflow, function () {
      return ymd(Temporal.PlainDate.from({ calendar: c[0], year: c[1], monthCode: c[2], day: 1 }, { overflow: overflow })); });
    k('PlainYearMonth ' + c[0] + ' ' + c[1] + ' ' + c[2] + ' ' + overflow, function () {
      var d = Temporal.PlainYearMonth.from({ calendar: c[0], year: c[1], monthCode: c[2] }, { overflow: overflow });
      return d.year + '/' + d.monthCode + ' ' + d.toString(); });
  });
});
// month and monthCode together; a month beyond the year's months.
[['hebrew', 5784, 6, 'M05L'], ['hebrew', 5784, 7, 'M06'], ['hebrew', 5783, 6, 'M06'], ['chinese', 2023, 3, 'M02L'], ['chinese', 2023, 3, 'M03'],
 ['coptic', 1739, 13, 'M13'], ['iso8601', 2024, 1, 'M02']].forEach(function (c) {
  k(c[0] + ' ' + c[1] + ' month ' + c[2] + ' monthCode ' + c[3], function () {
    return ymd(Temporal.PlainDate.from({ calendar: c[0], year: c[1], month: c[2], monthCode: c[3], day: 1 })); });
});
CALS.concat(['iso8601']).forEach(function (cal) {
  ['constrain', 'reject'].forEach(function (overflow) {
    k(cal + ' month 14 ' + overflow, function () { return ymd(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], month: 14, day: 1 }, { overflow: overflow })); });
  });
});
// The month codes a date answers in a leap year, month by month.
['hebrew', 'chinese', 'dangi', 'coptic', 'ethiopic'].forEach(function (cal) {
  t(cal + ' ' + LEAPYEAR[cal] + ' monthCodes', function () {
    var r = [], first = Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], month: 1, day: 1 });
    for (var m = 1; m <= first.monthsInYear; m++) r.push(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], month: m, day: 1 }).monthCode);
    return r.join(' '); });
});
