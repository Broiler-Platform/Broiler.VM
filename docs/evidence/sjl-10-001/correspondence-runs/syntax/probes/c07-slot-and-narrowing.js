// C31-C33: the calendar a Temporal object lends as a calendar value (ToTemporalCalendarIdentifier reads its slot),
// and integer fields of a calendar property bag beyond 32 bits (the milestone's integer narrowing, where it touches
// the calendar fields a string or bag carries).
var md = Temporal.PlainMonthDay.from({ calendar: 'hebrew', monthCode: 'M05L', day: 1 });
var ym = Temporal.PlainYearMonth.from({ calendar: 'coptic', year: 1740, monthCode: 'M13' });
var zdt = Temporal.ZonedDateTime.from('2020-01-01T00:00[UTC][u-ca=japanese]');
[['a PlainMonthDay in hebrew', md], ['a PlainYearMonth in coptic', ym], ['a ZonedDateTime in japanese', zdt],
 ['a PlainDateTime in indian', Temporal.PlainDateTime.from('2020-01-01T00:00[u-ca=indian]')],
 ['a PlainTime', Temporal.PlainTime.from('12:00')], ['a Duration', Temporal.Duration.from('P1D')], ['an Instant', Temporal.Instant.from('2020-01-01T00:00Z')]
].forEach(function (c) {
  k('PlainDate.from({..., calendar: ' + c[0] + '}).calendarId', function () { return Temporal.PlainDate.from({ year: 2020, month: 1, day: 1, calendar: c[1] }).calendarId; });
  k('PlainDate#withCalendar(' + c[0] + ').calendarId', function () { return new Temporal.PlainDate(2020, 1, 1).withCalendar(c[1]).calendarId; });
  k('PlainMonthDay.from({..., calendar: ' + c[0] + '}).calendarId', function () { return Temporal.PlainMonthDay.from({ monthCode: 'M01', day: 1, calendar: c[1] }).calendarId; });
});
// Fields beyond 32 bits: 2^32 + y wraps to y under a 32-bit narrowing.
var W = 4294967296;
['iso8601', 'gregory', 'japanese', 'hebrew', 'chinese', 'persian', 'coptic', 'islamic-civil'].forEach(function (cal) {
  k(cal + ' year 2^32+' + LEAPYEAR[cal], function () { return ymd(Temporal.PlainDate.from({ calendar: cal, year: W + LEAPYEAR[cal], monthCode: 'M01', day: 1 })); });
  k(cal + ' day 2^32+1 constrain', function () { return ymd(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], monthCode: 'M01', day: W + 1 })); });
  k(cal + ' day 2^32+1 reject', function () { return ymd(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], monthCode: 'M01', day: W + 1 }, { overflow: 'reject' })); });
  k(cal + ' month 2^32+1 constrain', function () { return ymd(Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], month: W + 1, day: 1 })); });
  k(cal + ' PlainYearMonth year 2^32+' + LEAPYEAR[cal], function () { var d = Temporal.PlainYearMonth.from({ calendar: cal, year: W + LEAPYEAR[cal], monthCode: 'M01' }); return d.year + ' ' + d.toString(); });
  k(cal + ' PlainMonthDay year 2^32+' + LEAPYEAR[cal], function () { var d = Temporal.PlainMonthDay.from({ calendar: cal, year: W + LEAPYEAR[cal], monthCode: 'M01', day: 1 }); return d.toString(); });
});
['gregory', 'japanese', 'roc', 'coptic', 'ethiopic', 'islamic-civil', 'hebrew', 'persian'].forEach(function (cal) {
  var era = Temporal.PlainDate.from({ calendar: cal, year: LEAPYEAR[cal], month: 1, day: 1 }).era;
  k(cal + ' era ' + era + ' eraYear 2^32+5', function () { return ymd(Temporal.PlainDate.from({ calendar: cal, era: era, eraYear: W + 5, monthCode: 'M01', day: 1 })); });
});
