// S9-S16: fractionalSecondDigits, smallestUnit and roundingMode in every toString that takes them: PlainTime,
// PlainDateTime, Instant, ZonedDateTime and Duration. Values in range, out of range and of other types; every mode
// at a tie and off it, positive and negative.
var subjects = {
  PlainTime: function () { return Temporal.PlainTime.from('12:34:56.123456789'); },
  PlainDateTime: function () { return Temporal.PlainDateTime.from('2020-01-01T12:34:56.123456789'); },
  Instant: function () { return Temporal.Instant.from('2020-01-01T12:34:56.123456789Z'); },
  ZonedDateTime: function () { return Temporal.ZonedDateTime.from('2020-01-01T12:34:56.123456789[UTC]'); },
  Duration: function () { return Temporal.Duration.from('PT12H34M56.123456789S'); }
};
Object.keys(subjects).forEach(function (type) {
  [undefined, 'auto', 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, -1, 2.5, 9.9, -0.5, '3', 'x', NaN, Infinity, -Infinity, true, null, 'AUTO', 3n].forEach(function (f) {
    k(type + ' fractionalSecondDigits ' + (typeof f === 'bigint' ? f + 'n' : typeof f === 'string' ? JSON.stringify(f) : String(f)), function () {
      return subjects[type]().toString({ fractionalSecondDigits: f }); });
  });
  ['minute', 'second', 'millisecond', 'microsecond', 'nanosecond', 'hour', 'day', 'year', 'minutes', 'seconds', 'MINUTE', 'auto', '', 'week'].forEach(function (u) {
    k(type + ' smallestUnit ' + u, function () { return subjects[type]().toString({ smallestUnit: u }); });
  });
  k(type + ' smallestUnit second with fractionalSecondDigits 5', function () { return subjects[type]().toString({ smallestUnit: 'second', fractionalSecondDigits: 5 }); });
  k(type + ' smallestUnit millisecond with fractionalSecondDigits 0', function () { return subjects[type]().toString({ smallestUnit: 'millisecond', fractionalSecondDigits: 0 }); });
});
// Every rounding mode at a tie and off it, positive and negative values.
var modes = ['ceil', 'floor', 'expand', 'trunc', 'halfCeil', 'halfFloor', 'halfExpand', 'halfTrunc', 'halfEven', 'x', 'HALFEXPAND', 'halfexpand'];
[['PlainTime', function () { return Temporal.PlainTime.from('12:34:56.5'); }, { smallestUnit: 'second' }],
 ['PlainTime', function () { return Temporal.PlainTime.from('12:34:57.5'); }, { smallestUnit: 'second' }],
 ['PlainTime', function () { return Temporal.PlainTime.from('23:59:59.999999995'); }, { fractionalSecondDigits: 8 }],
 ['PlainTime', function () { return Temporal.PlainTime.from('23:59:30'); }, { smallestUnit: 'minute' }],
 ['PlainDateTime', function () { return Temporal.PlainDateTime.from('2020-12-31T23:59:59.5'); }, { smallestUnit: 'second' }],
 ['Instant', function () { return Temporal.Instant.from('1969-12-31T23:59:59.5Z'); }, { smallestUnit: 'second' }],
 ['Instant', function () { return Temporal.Instant.from('1969-12-31T23:59:58.5Z'); }, { smallestUnit: 'second' }],
 ['Instant', function () { return Temporal.Instant.from('1969-12-31T23:59:59.25Z'); }, { fractionalSecondDigits: 1 }],
 ['ZonedDateTime', function () { return Temporal.ZonedDateTime.from('2020-01-01T23:59:59.5-05:00[America/New_York]'); }, { smallestUnit: 'second' }],
 ['Duration', function () { return Temporal.Duration.from('PT1.5S'); }, { smallestUnit: 'second' }],
 ['Duration', function () { return Temporal.Duration.from('-PT1.5S'); }, { smallestUnit: 'second' }],
 ['Duration', function () { return Temporal.Duration.from('-PT2.5S'); }, { smallestUnit: 'second' }],
 ['Duration', function () { return Temporal.Duration.from('-PT0.000000015S'); }, { fractionalSecondDigits: 8 }]
].forEach(function (c, i) {
  modes.forEach(function (m) {
    k('#' + i + ' ' + c[0] + ' ' + c[1]().toString() + ' ' + JSON.stringify(c[2]) + ' roundingMode ' + m, function () {
      var o = { roundingMode: m }; for (var key in c[2]) o[key] = c[2][key]; return c[1]().toString(o); });
  });
});
// Rounding that carries past a limit or across a day.
k('PlainDateTime max rounded up', function () { return new Temporal.PlainDateTime(275760, 9, 13, 23, 59, 59, 999).toString({ smallestUnit: 'second', roundingMode: 'ceil' }); });
k('PlainDateTime min rounded down', function () { return new Temporal.PlainDateTime(-271821, 4, 19, 0, 0, 0, 1).toString({ smallestUnit: 'second', roundingMode: 'floor' }); });
k('Instant max rounded up', function () { return new Temporal.Instant(8640000000000000000000n).toString({ smallestUnit: 'minute', roundingMode: 'ceil' }); });
k('Instant max - 1 rounded up', function () { return new Temporal.Instant(8640000000000000000000n - 1n).toString({ smallestUnit: 'second', roundingMode: 'ceil' }); });
k('Instant min rounded down', function () { return new Temporal.Instant(-8640000000000000000000n).toString({ smallestUnit: 'minute', roundingMode: 'floor' }); });
k('Instant min + 1 rounded down', function () { return new Temporal.Instant(-8640000000000000000000n + 1n).toString({ smallestUnit: 'minute', roundingMode: 'floor' }); });
k('ZonedDateTime max rounded up', function () { return new Temporal.ZonedDateTime(8640000000000000000000n, 'UTC').toString({ smallestUnit: 'minute', roundingMode: 'ceil' }); });
k('PlainTime 23:59:59.9 rounded up to a second', function () { return Temporal.PlainTime.from('23:59:59.9').toString({ smallestUnit: 'second', roundingMode: 'ceil' }); });
k('PlainDateTime 2020-12-31T23:59:59.9 rounded up', function () { return Temporal.PlainDateTime.from('2020-12-31T23:59:59.9').toString({ smallestUnit: 'second', roundingMode: 'ceil' }); });
