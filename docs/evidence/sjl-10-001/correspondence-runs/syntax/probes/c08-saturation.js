// C34: a year beyond 32 bits in an ISO month-day, where the year decides only whether February 29 exists. The ISO
// reference year is 1972 whatever the year; the year's leap year decides the day under reject and constrain.
[2147483652, 2147483653, 4294967296, 1e10, 1e10 + 1, 1e15, -2147483652, 2147483647, 2147483648].forEach(function (y) {
  ['reject', 'constrain'].forEach(function (overflow) {
    k('PlainMonthDay.from({year: ' + y + ', month: 2, day: 29}, ' + overflow + ')', function () {
      return Temporal.PlainMonthDay.from({ year: y, month: 2, day: 29 }, { overflow: overflow }).toString(); });
  });
});
k('PlainMonthDay.from({year: 2147483652, monthCode: "M02", day: 29}) through with()', function () {
  return Temporal.PlainMonthDay.from('02-28').with({ year: 2147483652, day: 29 }, { overflow: 'reject' }).toString(); });
