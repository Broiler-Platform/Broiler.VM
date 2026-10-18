// F8: PlainYearMonth until and since (the first of each month, years and months only).
t('2019-05 until 2021-03 largest month', function () { return PYM('2019-05').until('2021-03', { largestUnit: 'month' }); });
t('2019-05 until 2021-03 smallest year halfExpand', function () { return PYM('2019-05').until('2021-03', { smallestUnit: 'year', roundingMode: 'halfExpand' }); });
t('2021-03 since 2019-05 smallest year halfExpand', function () { return PYM('2021-03').since('2019-05', { smallestUnit: 'year', roundingMode: 'halfExpand' }); });
t('2019-05 since 2021-03 smallest year floor', function () { return PYM('2019-05').since('2021-03', { smallestUnit: 'year', roundingMode: 'floor' }); });
t('2019-05 until 2021-03 smallest month increment 5 halfExpand', function () { return PYM('2019-05').until('2021-03', { smallestUnit: 'month', roundingIncrement: 5, roundingMode: 'halfExpand' }); });
t('-271821-04 until -271821-05', function () { return PYM('-271821-04').until('-271821-05'); });
t('-271821-05 until -271821-04', function () { return PYM('-271821-05').until('-271821-04'); });
t('-271821-04 until -271821-04', function () { return PYM('-271821-04').until('-271821-04'); });
t('+275760-09 since +275760-08', function () { return PYM('+275760-09').since('+275760-08'); });
t('+275760-08 until +275760-09 smallest year ceil', function () { return PYM('+275760-08').until('+275760-09', { smallestUnit: 'year', roundingMode: 'ceil' }); });
t('2019-05 until 2021-03 other calendar', function () { return PYM('2019-05').until(PYM('2021-03').toPlainDate({ day: 1 }).withCalendar('gregory').toPlainYearMonth()); });
