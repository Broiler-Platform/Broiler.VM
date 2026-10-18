// F6: PlainTime and Instant until and since (DifferenceTime, DifferenceInstant, time units only).
t('PlainTime 10:00 until 08:30', function () { return PT('10:00').until('08:30'); });
t('PlainTime 10:00 since 08:29:31 minute halfExpand', function () { return PT('10:00').since('08:29:31', { smallestUnit: 'minute', roundingMode: 'halfExpand' }); });
t('PlainTime 10:00 since 08:29:31 largest minute', function () { return PT('10:00').since('08:29:31', { largestUnit: 'minute' }); });
t('PlainTime 08:29:31 since 10:00 minute floor', function () { return PT('08:29:31').since('10:00', { smallestUnit: 'minute', roundingMode: 'floor' }); });
t('PlainTime 00:00 until 23:59:59.999999999 hour increment 6 halfEven', function () { return PT('00:00').until('15:00', { smallestUnit: 'hour', roundingIncrement: 6, roundingMode: 'halfEven' }); });
t('PlainTime 00:00 until 21:00 hour increment 6 halfEven', function () { return PT('00:00').until('21:00', { smallestUnit: 'hour', roundingIncrement: 6, roundingMode: 'halfEven' }); });
t('PlainTime 00:00 until 23:59:59.999999999 largest nanosecond', function () { return PT('00:00').until('23:59:59.999999999', { largestUnit: 'nanosecond' }); });
t('Instant min until max largest hour', function () { return I('-271821-04-20T00:00Z').until('+275760-09-13T00:00Z', { largestUnit: 'hour' }); });
t('Instant min until max largest nanosecond', function () { return I('-271821-04-20T00:00Z').until('+275760-09-13T00:00Z', { largestUnit: 'nanosecond' }); });
t('Instant min until max largest microsecond', function () { return I('-271821-04-20T00:00Z').until('+275760-09-13T00:00Z', { largestUnit: 'microsecond' }).microseconds; });
t('Instant until 1 h 29 min hour halfExpand', function () { return I('2020-01-01T00:00Z').until('2020-01-01T01:29Z', { smallestUnit: 'hour', roundingMode: 'halfExpand' }); });
t('Instant since 1 h 30 min hour halfTrunc', function () { return I('2020-01-01T01:30Z').since('2020-01-01T00:00Z', { smallestUnit: 'hour', roundingMode: 'halfTrunc' }); });
t('Instant since -1 h 30 min hour halfTrunc', function () { return I('2020-01-01T00:00Z').since('2020-01-01T01:30Z', { smallestUnit: 'hour', roundingMode: 'halfTrunc' }); });
t('Instant since -1 h 30 min hour halfCeil', function () { return I('2020-01-01T00:00Z').since('2020-01-01T01:30Z', { smallestUnit: 'hour', roundingMode: 'halfCeil' }); });
t('Instant until 3 h 29 min, 30-minute increments, expand', function () { return I('2020-01-01T00:00Z').until('2020-01-01T03:29Z', { smallestUnit: 'minute', roundingIncrement: 30, roundingMode: 'expand', largestUnit: 'hour' }); });
