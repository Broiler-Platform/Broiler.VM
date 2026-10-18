// F2: PlainDate until and since, every largest unit, the month-end anchoring of the receiver.
t('2020-01-31 until 2020-03-01 month', function () { return PD('2020-01-31').until('2020-03-01', { largestUnit: 'month' }); });
t('2020-03-01 since 2020-01-31 month', function () { return PD('2020-03-01').since('2020-01-31', { largestUnit: 'month' }); });
t('2020-01-31 since 2020-03-01 month', function () { return PD('2020-01-31').since('2020-03-01', { largestUnit: 'month' }); });
t('2020-03-01 until 2020-01-31 month', function () { return PD('2020-03-01').until('2020-01-31', { largestUnit: 'month' }); });
t('2020-01-01 until 2020-03-01 week', function () { return PD('2020-01-01').until('2020-03-01', { largestUnit: 'week' }); });
t('2019-02-28 until 2020-02-29 year', function () { return PD('2019-02-28').until('2020-02-29', { largestUnit: 'year' }); });
t('2020-02-29 until 2021-02-28 year', function () { return PD('2020-02-29').until('2021-02-28', { largestUnit: 'year' }); });
t('2021-02-28 until 2020-02-29 year', function () { return PD('2021-02-28').until('2020-02-29', { largestUnit: 'year' }); });
t('2020-01-29 until 2020-02-28 month', function () { return PD('2020-01-29').until('2020-02-28', { largestUnit: 'month' }); });
t('2019-01-29 until 2019-03-01 month', function () { return PD('2019-01-29').until('2019-03-01', { largestUnit: 'month' }); });
t('-271821-04-19 until +275760-09-13 year', function () { return PD('-271821-04-19').until('+275760-09-13', { largestUnit: 'year' }); });
t('-271821-04-19 until +275760-09-13 day', function () { return PD('-271821-04-19').until('+275760-09-13'); });
t('+275760-09-13 since -271821-04-19 week', function () { return PD('+275760-09-13').since('-271821-04-19', { largestUnit: 'week' }); });
t('2020-01-01 until 2020-01-01', function () { return PD('2020-01-01').until('2020-01-01', { largestUnit: 'year' }); });
