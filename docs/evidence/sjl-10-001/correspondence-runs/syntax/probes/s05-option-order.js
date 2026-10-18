// S28-S30: the order in which each toString reads its options, and when it validates them: every option a getter
// that logs its name, then a valid set, then an invalid smallestUnit with later options still read or not.
function logged(values) {
  var log = [], o = {};
  Object.keys(values).forEach(function (key) {
    Object.defineProperty(o, key, { get: function () { log.push(key); return values[key]; }, enumerable: true });
  });
  return { o: o, log: log };
}
var cases = {
  PlainDate: [Temporal.PlainDate.from('2020-01-01'), { calendarName: 'auto' }],
  PlainYearMonth: [Temporal.PlainYearMonth.from('2020-01'), { calendarName: 'auto' }],
  PlainMonthDay: [Temporal.PlainMonthDay.from('01-01'), { calendarName: 'auto' }],
  PlainTime: [Temporal.PlainTime.from('12:00'), { fractionalSecondDigits: 'auto', roundingMode: 'trunc', smallestUnit: 'second' }],
  PlainDateTime: [Temporal.PlainDateTime.from('2020-01-01T12:00'), { calendarName: 'auto', fractionalSecondDigits: 'auto', roundingMode: 'trunc', smallestUnit: 'second' }],
  Instant: [Temporal.Instant.from('2020-01-01T12:00Z'), { fractionalSecondDigits: 'auto', roundingMode: 'trunc', smallestUnit: 'second', timeZone: 'UTC' }],
  ZonedDateTime: [Temporal.ZonedDateTime.from('2020-01-01T12:00[UTC]'), { calendarName: 'auto', fractionalSecondDigits: 'auto', offset: 'auto', roundingMode: 'trunc', smallestUnit: 'second', timeZoneName: 'auto' }],
  Duration: [Temporal.Duration.from('PT1S'), { fractionalSecondDigits: 'auto', roundingMode: 'trunc', smallestUnit: 'second' }]
};
Object.keys(cases).forEach(function (type) {
  var subject = cases[type][0], values = cases[type][1];
  // The names in reverse, so that the order read is the engine's and not the object's.
  var rev = {}; Object.keys(values).reverse().forEach(function (key) { rev[key] = values[key]; });
  var l = logged(rev);
  t(type + ' reads', function () { var s = subject.toString(l.o); return l.log.join(',') + ' => ' + s; });
  ['smallestUnit', 'roundingMode', 'fractionalSecondDigits', 'calendarName', 'offset', 'timeZoneName', 'timeZone'].forEach(function (bad) {
    if (!(bad in values)) return;
    var v = {}; Object.keys(rev).forEach(function (key) { v[key] = key === bad ? (bad === 'smallestUnit' ? 'year' : bad === 'fractionalSecondDigits' ? 10 : bad === 'timeZone' ? 'Mars/Olympus' : 'bogus') : rev[key]; });
    var lb = logged(v);
    k(type + ' with an invalid ' + bad + ', reads', function () { try { subject.toString(lb.o); } finally { out('  ' + type + ' invalid ' + bad + ' read: ' + lb.log.join(',')); } });
  });
});
