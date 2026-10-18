// F5: PlainDateTime until and since: the day borrowed for the time, time largest units, rounding.
t('2020-01-01T12:00 until 2020-01-03T06:00', function () { return PDT('2020-01-01T12:00').until('2020-01-03T06:00'); });
t('2020-01-03T06:00 since 2020-01-01T12:00', function () { return PDT('2020-01-03T06:00').since('2020-01-01T12:00'); });
t('2020-01-03T06:00 until 2020-01-01T12:00', function () { return PDT('2020-01-03T06:00').until('2020-01-01T12:00'); });
t('2020-01-01T12:00 until 2020-01-03T06:00 largest hour', function () { return PDT('2020-01-01T12:00').until('2020-01-03T06:00', { largestUnit: 'hour' }); });
t('2020-01-01T12:00 until 2020-01-03T06:00 smallest day halfExpand', function () { return PDT('2020-01-01T12:00').until('2020-01-03T06:00', { smallestUnit: 'day', roundingMode: 'halfExpand' }); });
t('2020-01-31T12:00 until 2020-03-01T06:00 largest month', function () { return PDT('2020-01-31T12:00').until('2020-03-01T06:00', { largestUnit: 'month' }); });
t('2019-03-31T00:00 until 2020-02-29T12:00 month/year trunc', function () { return PDT('2019-03-31T00:00').until('2020-02-29T12:00', { smallestUnit: 'month', largestUnit: 'year' }); });
t('2019-03-31T00:00 until 2020-02-29T12:00 month/year halfExpand', function () { return PDT('2019-03-31T00:00').until('2020-02-29T12:00', { smallestUnit: 'month', largestUnit: 'year', roundingMode: 'halfExpand' }); });
t('2020-02-29T12:00 since 2019-03-31T00:00 month/year trunc', function () { return PDT('2020-02-29T12:00').since('2019-03-31T00:00', { smallestUnit: 'month', largestUnit: 'year' }); });
t('2019-01-01T00:00 until 2020-12-31T23:59:59.999999999 microsecond/year halfExpand', function () { return PDT('2019-01-01T00:00').until('2020-12-31T23:59:59.999999999', { smallestUnit: 'microsecond', largestUnit: 'year', roundingMode: 'halfExpand' }); });
t('2020-01-01T00:00 until 2020-01-01T23:59:59.5 second/day halfExpand', function () { return PDT('2020-01-01T00:00').until('2020-01-01T23:59:59.5', { smallestUnit: 'second', largestUnit: 'day', roundingMode: 'halfExpand' }); });
t('2020-01-01T00:00 until 2020-01-01T23:30 hour/day increment 3 ceil', function () { return PDT('2020-01-01T00:00').until('2020-01-01T23:30', { smallestUnit: 'hour', largestUnit: 'day', roundingIncrement: 3, roundingMode: 'ceil' }); });
t('2020-01-01T00:00 until 2020-01-08T12:00 week/week halfExpand', function () { return PDT('2020-01-01T00:00').until('2020-01-08T12:00', { smallestUnit: 'week', roundingMode: 'halfExpand' }); });
t('-271821-04-19T00:00:00.000000001 until +275760-09-13T23:59:59.999999999 largest nanosecond', function () { return PDT('-271821-04-19T00:00:00.000000001').until('+275760-09-13T23:59:59.999999999', { largestUnit: 'nanosecond' }); });
t('-271821-04-19T00:00:00.000000001 until +275760-09-13T23:59:59.999999999 largest year', function () { return PDT('-271821-04-19T00:00:00.000000001').until('+275760-09-13T23:59:59.999999999', { largestUnit: 'year' }); });
