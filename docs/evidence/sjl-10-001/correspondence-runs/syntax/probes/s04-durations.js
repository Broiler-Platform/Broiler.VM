// S23-S27: Duration's string form: zero, signs, every field, sub-second fields beyond a second, the limits, rounding
// that carries into larger units, and precision against auto.
[{}, { years: 1 }, { years: -1 }, { weeks: 3, days: 4 }, { hours: 25 }, { minutes: 61 }, { seconds: 61 }, { milliseconds: 1500 },
 { milliseconds: 1000 }, { microseconds: 1000001 }, { nanoseconds: 1000000001 }, { seconds: 1, milliseconds: 999, microseconds: 999, nanoseconds: 1000 },
 { milliseconds: 0.0 }, { years: 4294967295 }, { months: 4294967295 }, { weeks: 4294967295 }, { days: 104249991374 }, { hours: 2501999792983 },
 { minutes: 150119987579016 }, { seconds: 9007199254740991 }, { milliseconds: 9007199254740991 }, { microseconds: 9007199254740991 },
 { nanoseconds: 9007199254740991 }, { seconds: 9007199254740991, nanoseconds: 999999999 }, { milliseconds: 9007199254740991999 },
 { days: -1, hours: -1 }, { hours: 1, nanoseconds: 1 }, { minutes: 1, seconds: 0 }, { seconds: 0.5e-9 * 0 }].forEach(function (f) {
  k('Duration ' + JSON.stringify(f), function () { return Temporal.Duration.from(f).toString(); });
  k('Duration ' + JSON.stringify(f) + ' fractionalSecondDigits 0', function () { return Temporal.Duration.from(f).toString({ fractionalSecondDigits: 0 }); });
  k('Duration ' + JSON.stringify(f) + ' fractionalSecondDigits 3', function () { return Temporal.Duration.from(f).toString({ fractionalSecondDigits: 3 }); });
});
// Rounding that carries.
[['PT59.999999999S', 'ceil', 0], ['PT59.5S', 'halfExpand', 0], ['PT59M59.5S', 'halfExpand', 0], ['PT23H59M59.5S', 'halfExpand', 0],
 ['P1DT23H59M59.5S', 'halfExpand', 0], ['P1WT23H59M59.5S', 'halfExpand', 0], ['P1YT23H59M59.5S', 'halfExpand', 0], ['PT0.5S', 'halfEven', 0],
 ['PT1.5S', 'halfEven', 0], ['-PT59.5S', 'halfExpand', 0], ['-PT59.5S', 'floor', 0], ['-PT59.5S', 'ceil', 0], ['PT1H59M59.999S', 'ceil', 2],
 ['PT2501999792983H59M59.999999999S', 'ceil', 0], ['PT9007199254740991.999999999S', 'ceil', 0], ['PT9007199254740991.5S', 'halfExpand', 0],
 ['PT9007199254740990.5S', 'halfEven', 0]].forEach(function (c) {
  k('Duration ' + c[0] + ' ' + c[1] + ' fractionalSecondDigits ' + c[2], function () {
    return Temporal.Duration.from(c[0]).toString({ roundingMode: c[1], fractionalSecondDigits: c[2] }); });
  k('Duration ' + c[0] + ' ' + c[1] + ' smallestUnit second', function () {
    return Temporal.Duration.from(c[0]).toString({ roundingMode: c[1], smallestUnit: 'second' }); });
});
