// G1-G4: the date part. Year forms (four digits, six with an ASCII sign, -000000 refused), extended and basic
// separators and their mix, month and day ranges, leap days, the representable limits, signs other than ASCII,
// decimal digits other than ASCII, surrounding space. Each string alone, then with a time, through every from().
var dates = ['2020-01-01', '20200101', '2020-0101', '202001-01', '+002020-01-01', '+0020200101', '-002020-01-01',
  '-000000-01-01', '+000000-01-01', '-0000000101', '+0020-01-01', '02020-01-01', '020-01-01', '2020-1-01', '2020-01-1',
  '2020-00-01', '2020-13-01', '2020-01-00', '2020-01-32', '2020-02-29', '2021-02-29', '2020-02-30', '2020-04-31',
  '1900-02-29', '2000-02-29', '−002020-01-01', '+999999-12-31', '-999999-01-01', '-271821-04-19', '-271821-04-18',
  '+275760-09-13', '+275760-09-14', '0000-01-01', '-000001-12-31', ' 2020-01-01', '2020-01-01 ',
  '２０２０-01-01', '٢٠٢٠-٠١-٠١', '2020-01-01\u0000', ''];
dates.forEach(function (d) {
  each(PLAIN, d);
  each(['ZonedDateTime'], d + '[UTC]');
  each(['Instant'], d + 'T00:00Z');
});
