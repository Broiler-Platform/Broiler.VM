// B9: RoundTime, RoundISODateTime and RoundTemporalInstant (the rounding of points rather than durations).
var modes = ['ceil', 'floor', 'expand', 'trunc', 'halfCeil', 'halfFloor', 'halfExpand', 'halfTrunc', 'halfEven'];
modes.forEach(function (m) {
  t('PlainTime 12:30 round hour ' + m, function () { return PT('12:30').round({ smallestUnit: 'hour', roundingMode: m }); });
  t('Instant -1.5 s round second ' + m, function () { return T.Instant.fromEpochNanoseconds(-1500000000n).round({ smallestUnit: 'second', roundingMode: m }); });
});
t('PlainTime 23:59:59.5 round second halfExpand', function () { return PT('23:59:59.5').round({ smallestUnit: 'second' }); });
t('PlainTime 13:00 round hour increment 12 ceil', function () { return PT('13:00').round({ smallestUnit: 'hour', roundingIncrement: 12, roundingMode: 'ceil' }); });
t('PlainDateTime 2020-12-31T23:59:59.5 round second', function () { return PDT('2020-12-31T23:59:59.5').round('second'); });
t('PlainDateTime 2020-12-31T12:00 round day', function () { return PDT('2020-12-31T12:00').round('day'); });
t('PlainDateTime 2020-12-31T12:00 round day increment 2', function () { return PDT('2020-12-31T12:00').round({ smallestUnit: 'day', roundingIncrement: 2 }); });
t('PlainDateTime +275760-09-13T12:00 round day', function () { return PDT('+275760-09-13T12:00').round('day'); });
t('PlainDateTime -271821-04-19T00:00:00.000000001 round day floor', function () { return PDT('-271821-04-19T00:00:00.000000001').round({ smallestUnit: 'day', roundingMode: 'floor' }); });
t('PlainDateTime -271821-04-19T00:00:00.000000001 round microsecond floor', function () { return PDT('-271821-04-19T00:00:00.000000001').round({ smallestUnit: 'microsecond', roundingMode: 'floor' }); });
t('Instant max round hour ceil', function () { return T.Instant.fromEpochNanoseconds(8640000000000000000000n).round({ smallestUnit: 'hour', roundingMode: 'ceil' }); });
t('Instant max - 1 ns round day increment 1 halfExpand (hours 24)', function () { return T.Instant.fromEpochNanoseconds(8639999999999999999999n).round({ smallestUnit: 'hour', roundingIncrement: 24 }); });
t('Instant round minute increment 1440', function () { return I('2020-01-01T12:00Z').round({ smallestUnit: 'minute', roundingIncrement: 1440 }); });
t('Instant round minute increment 1441', function () { return I('2020-01-01T12:00Z').round({ smallestUnit: 'minute', roundingIncrement: 1441 }); });
t('ZonedDateTime NY 2026-03-08T01:59:59.5-05:00 round second', function () { return Z('2026-03-08T01:59:59.5-05:00[America/New_York]').round('second'); });
t('ZonedDateTime NY 2026-11-01T01:59:59.5-04:00 round second', function () { return Z('2026-11-01T01:59:59.5-04:00[America/New_York]').round('second'); });
t('ZonedDateTime NY 2026-11-01T01:30-05:00 round hour', function () { return Z('2026-11-01T01:30-05:00[America/New_York]').round('hour'); });
t('ZonedDateTime UTC +275760-09-13T00:00 round day', function () { return Z('+275760-09-13T00:00[UTC]').round('day'); });
t('ZonedDateTime UTC +275760-09-12T12:00 round day', function () { return Z('+275760-09-12T12:00[UTC]').round('day'); });
