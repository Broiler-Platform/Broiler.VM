// S1-S8: every type's string form at notable years and in calendars other than ISO, under each calendarName, and
// toJSON, which is toString with no options.
var years = [0, 1, 999, 1000, 9999, 10000, -1, -9999, -10000, 123456, -123456];
years.forEach(function (y) {
  t('PlainDate ' + y, function () { return new Temporal.PlainDate(y, 1, 2).toString(); });
  t('PlainYearMonth ' + y, function () { return new Temporal.PlainYearMonth(y, 3).toString(); });
  t('PlainDateTime ' + y, function () { return new Temporal.PlainDateTime(y, 1, 2, 3, 4, 5, 6, 7, 8).toString(); });
  t('Instant ' + y, function () { return Temporal.Instant.from(new Temporal.PlainDate(y, 1, 2).toString() + 'T03:04:05.000000006Z').toString(); });
  t('ZonedDateTime ' + y, function () { return new Temporal.PlainDateTime(y, 1, 2, 3, 4, 5).toZonedDateTime('+01:00').toString(); });
});
t('PlainDate min', function () { return new Temporal.PlainDate(-271821, 4, 19).toString(); });
t('PlainDate max', function () { return new Temporal.PlainDate(275760, 9, 13).toString(); });
t('PlainYearMonth min', function () { return new Temporal.PlainYearMonth(-271821, 4).toString(); });
t('Instant min', function () { return new Temporal.Instant(-8640000000000000000000n).toString(); });
t('Instant max', function () { return new Temporal.Instant(8640000000000000000000n).toString(); });
t('PlainMonthDay ISO with reference 1972', function () { return new Temporal.PlainMonthDay(2, 29).toString(); });
t('PlainMonthDay ISO with reference 2000', function () { return new Temporal.PlainMonthDay(2, 29, 'iso8601', 2000).toString(); });
t('PlainYearMonth ISO with reference day 15', function () { return new Temporal.PlainYearMonth(2020, 2, 'iso8601', 15).toString(); });
['auto', 'always', 'never', 'critical', undefined, 'x', 'AUTO', '', null, 1, true].forEach(function (cn) {
  var o = { calendarName: cn }, label = ' calendarName ' + JSON.stringify(cn);
  ['iso8601', 'gregory', 'hebrew', 'chinese'].forEach(function (cal) {
    k('PlainDate ' + cal + label, function () { return Temporal.PlainDate.from('2020-03-15').withCalendar(cal).toString(o); });
    k('PlainDateTime ' + cal + label, function () { return Temporal.PlainDateTime.from('2020-03-15T12:00').withCalendar(cal).toString(o); });
    k('PlainYearMonth ' + cal + label, function () { return Temporal.PlainYearMonth.from({ calendar: cal, year: Temporal.PlainDate.from('2020-03-15').withCalendar(cal).year, monthCode: 'M02' }).toString(o); });
    k('PlainMonthDay ' + cal + label, function () { return Temporal.PlainMonthDay.from({ calendar: cal, monthCode: 'M02', day: 10 }).toString(o); });
    k('ZonedDateTime ' + cal + label, function () { return Temporal.ZonedDateTime.from('2020-03-15T12:00[UTC]').withCalendar(cal).toString(o); });
  });
});
// toJSON against toString, and toString's options argument when not an object.
t('PlainDate toJSON', function () { return Temporal.PlainDate.from('2020-03-15[u-ca=hebrew]').toJSON(); });
t('PlainYearMonth toJSON', function () { return Temporal.PlainYearMonth.from('2020-03-15[u-ca=hebrew]').toJSON(); });
t('PlainMonthDay toJSON', function () { return Temporal.PlainMonthDay.from('2020-03-15[u-ca=hebrew]').toJSON(); });
t('PlainMonthDay ISO toJSON', function () { return Temporal.PlainMonthDay.from('03-15').toJSON(); });
t('PlainDateTime toJSON', function () { return Temporal.PlainDateTime.from('2020-03-15T12:00:00.5[u-ca=japanese]').toJSON(); });
t('PlainTime toJSON', function () { return Temporal.PlainTime.from('12:00:00.000001').toJSON(); });
t('ZonedDateTime toJSON', function () { return Temporal.ZonedDateTime.from('2020-03-15T12:00:00.1+05:30[Asia/Kolkata][u-ca=roc]').toJSON(); });
t('Instant toJSON', function () { return Temporal.Instant.from('2020-03-15T12:00:00.12Z').toJSON(); });
t('Duration toJSON', function () { return Temporal.Duration.from('P1Y2M3W4DT5H6M7.008009010S').toJSON(); });
['PlainDate', 'PlainTime', 'Duration', 'Instant'].forEach(function (type) {
  [null, 1, 'x', true].forEach(function (o) {
    k(type + '#toString(' + JSON.stringify(o) + ')', function () {
      var v = type === 'PlainDate' ? Temporal.PlainDate.from('2020-01-01') : type === 'PlainTime' ? Temporal.PlainTime.from('12:00')
        : type === 'Duration' ? Temporal.Duration.from('PT1S') : Temporal.Instant.from('2020-01-01T00:00Z');
      return v.toString(o); });
  });
});
