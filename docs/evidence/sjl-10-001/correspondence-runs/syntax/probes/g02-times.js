// G5-G9: the time part. Separators and their mix, hour alone, fraction digits (1 to 9, 10 refused), comma,
// leap second, 24:00, fractions on minutes or hours (refused), designators T, t and space, and the early errors
// that refuse a designator-less time that is also a month-day or a year-month.
var bare = ['12', '1230', '12:30', '123045', '12:30:45', '12:30:45.1', '12:30:45.123456789', '12:30:45.1234567890',
  '12:30:45,5', '12:30:60', '12:30:61', '24:00', '23:59:59.999999999', '12:3045', '1230:45', '12:30:45.', '12:30.5',
  '12.5', 'T12:30', 't12:30', 'T12', ' 12:30', 'TT12:30', '2021-12', '202112', '2021-13', 'T2021-12', '1214', '0229',
  '0230', '1232', '--1214', '12:30Z', '12:30z', 'T12:30Z', '١٢:٣٠', '12:30:45.١'];
bare.forEach(function (s) { each(['PlainTime'], s); });
var withDate = ['T12', 'T1230', 't12:30', ' 12:30', 'T', 'TT12:30', 'T12:30:45.123456789', 'T12:30:45.1234567890',
  'T12:30:60', 'T24:00', 'T23:59:60', 'T12:3045', 'T1230:45', 'T12:30.5', 'T12.5', 'T12:30,5', '  12:30', 'T12:30:45.0'];
withDate.forEach(function (tail) {
  each(PLAIN, '2020-01-01' + tail);
  each(PLAIN, '20200101' + tail);
  each(['ZonedDateTime'], '2020-01-01' + tail + '[UTC]');
  each(['Instant'], '2020-01-01' + tail + 'Z');
});
