// D8: the nudge after a shifted window. From 2019-03-31 the destination 2020-02-29T12:00 lies past the first month
// window (2020-01-31 to the clamped 2020-02-29) and inside the second (2020-02-29 to 2020-03-31); rounded down to
// 11 months, does the nudged instant bubble into a year?
var d = { months: 10, days: 29, hours: 12 };
t('Duration round month/year trunc, relativeTo "2019-03-31"', function () { return D(d).round({ smallestUnit: 'month', largestUnit: 'year', roundingMode: 'trunc', relativeTo: '2019-03-31' }); });
t('Duration round month/year trunc, relativeTo "2019-03-31T00:00[UTC]"', function () { return D(d).round({ smallestUnit: 'month', largestUnit: 'year', roundingMode: 'trunc', relativeTo: '2019-03-31T00:00[UTC]' }); });
t('Duration round month/year trunc, relativeTo ZonedDateTime 2019-03-31[UTC]', function () { return D(d).round({ smallestUnit: 'month', largestUnit: 'year', roundingMode: 'trunc', relativeTo: Z('2019-03-31T00:00[UTC]') }); });
t('PlainDateTime 2019-03-31T00:00 until 2020-02-29T12:00 month/year trunc', function () { return PDT('2019-03-31T00:00').until('2020-02-29T12:00', { smallestUnit: 'month', largestUnit: 'year' }); });
t('ZonedDateTime 2019-03-31T00:00[UTC] until 2020-02-29T12:00[UTC] month/year trunc', function () { return Z('2019-03-31T00:00[UTC]').until('2020-02-29T12:00[UTC]', { smallestUnit: 'month', largestUnit: 'year' }); });
t('Duration total month, relativeTo "2019-03-31"', function () { return D(d).total({ unit: 'month', relativeTo: '2019-03-31' }); });
t('2019-03-31 + P1Y', function () { return PD('2019-03-31').add({ years: 1 }); });
