// D5: Duration.prototype.round with no relativeTo: the nine modes, increments, and the unit checks.
var modes = ['ceil', 'floor', 'expand', 'trunc', 'halfCeil', 'halfFloor', 'halfExpand', 'halfTrunc', 'halfEven'];
modes.forEach(function (m) {
  t('PT1H30M round hour ' + m, function () { return D('PT1H30M').round({ smallestUnit: 'hour', roundingMode: m }); });
  t('-PT1H30M round hour ' + m, function () { return D('-PT1H30M').round({ smallestUnit: 'hour', roundingMode: m }); });
  t('PT2H30M round hour ' + m, function () { return D('PT2H30M').round({ smallestUnit: 'hour', roundingMode: m }); });
  t('-PT1H20M round 30 minutes ' + m, function () { return D('-PT1H20M').round({ smallestUnit: 'minute', roundingIncrement: 30, roundingMode: m }); });
});
t('Duration({hours: 25, minutes: 30}).round({largestUnit: "day"})', function () { return D({ hours: 25, minutes: 30 }).round({ largestUnit: 'day' }); });
t('Duration({days: 3, hours: 12}).round({smallestUnit: "day", roundingIncrement: 2, largestUnit: "day"})', function () { return D({ days: 3, hours: 12 }).round({ smallestUnit: 'day', roundingIncrement: 2, largestUnit: 'day' }); });
t('Duration({minutes: 125}).round({smallestUnit: "minute", roundingIncrement: 15})', function () { return D({ minutes: 125 }).round({ smallestUnit: 'minute', roundingIncrement: 15 }); });
t('Duration({hours: 47}).round({largestUnit: "day"})', function () { return D({ hours: 47 }).round({ largestUnit: 'day' }); });
t('Duration({days: 2}).round({largestUnit: "hour"})', function () { return D({ days: 2 }).round({ largestUnit: 'hour' }); });
t('Duration({days: 2}).round({largestUnit: "auto", smallestUnit: "hour"})', function () { return D({ days: 2 }).round({ largestUnit: 'auto', smallestUnit: 'hour' }); });
t('Duration({hours: 1}).round("minutes")', function () { return D({ hours: 1 }).round('minutes'); });
t('Duration({hours: 1}).round({})', function () { return D({ hours: 1 }).round({}); });
t('Duration({hours: 1}).round()', function () { return D({ hours: 1 }).round(); });
t('Duration({hours: 1}).round({largestUnit: "hour", smallestUnit: "day"})', function () { return D({ hours: 1 }).round({ largestUnit: 'hour', smallestUnit: 'day' }); });
t('Duration({hours: 1}).round({smallestUnit: "hour", roundingIncrement: 5})', function () { return D({ hours: 1 }).round({ smallestUnit: 'hour', roundingIncrement: 5 }); });
t('Duration({hours: 1}).round({smallestUnit: "hour", roundingIncrement: 24})', function () { return D({ hours: 1 }).round({ smallestUnit: 'hour', roundingIncrement: 24 }); });
t('Duration({days: 1}).round({smallestUnit: "day", roundingIncrement: 2, largestUnit: "month"})', function () { return D({ days: 1 }).round({ smallestUnit: 'day', roundingIncrement: 2, largestUnit: 'month', relativeTo: '2020-01-01' }); });
t('Duration({days: 1}).round({smallestUnit: "month"})', function () { return D({ days: 1 }).round({ smallestUnit: 'month' }); });
t('Duration({years: 1}).round({smallestUnit: "hour"})', function () { return D({ years: 1 }).round({ smallestUnit: 'hour' }); });
t('Duration({weeks: 1}).round({largestUnit: "day"})', function () { return D({ weeks: 1 }).round({ largestUnit: 'day' }); });
t('Duration({days: 1}).round({largestUnit: "week"})', function () { return D({ days: 1 }).round({ largestUnit: 'week' }); });
t('Duration({seconds: 2**53 - 1}).round({smallestUnit: "hour", roundingMode: "ceil"})', function () { return D({ seconds: Math.pow(2, 53) - 1 }).round({ smallestUnit: 'hour', roundingMode: 'ceil' }); });
t('Duration({seconds: 2**53 - 1}).round({smallestUnit: "day", largestUnit: "day", roundingMode: "floor"})', function () { return D({ seconds: Math.pow(2, 53) - 1 }).round({ smallestUnit: 'day', largestUnit: 'day', roundingMode: 'floor' }); });
t('Duration({days: 1e8}).round({smallestUnit: "day", roundingIncrement: 1e9, roundingMode: "ceil", largestUnit: "day"})', function () { return D({ days: 1e8 }).round({ smallestUnit: 'day', roundingIncrement: 1e9, roundingMode: 'ceil', largestUnit: 'day' }); });
t('Duration({days: 1e8}).round({smallestUnit: "day", roundingIncrement: 2.5, largestUnit: "day"})', function () { return D({ days: 1e8 + 1 }).round({ smallestUnit: 'day', roundingIncrement: 2.5, largestUnit: 'day' }); });
