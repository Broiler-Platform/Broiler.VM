// D11: round, total and compare relative to a date in another calendar.
var heb = '2020-01-01[u-ca=hebrew]';
t('{months: 1}.round({smallestUnit: "day", relativeTo: hebrew 2020-01-01})', function () { return D({ months: 1 }).round({ smallestUnit: 'day', relativeTo: heb }); });
t('{days: 45}.round({largestUnit: "month", relativeTo: hebrew 2020-01-01})', function () { return D({ days: 45 }).round({ largestUnit: 'month', relativeTo: heb }); });
t('{days: 45}.total({unit: "month", relativeTo: hebrew 2020-01-01})', function () { return D({ days: 45 }).total({ unit: 'month', relativeTo: heb }); });
t('{days: 45}.total({unit: "month", relativeTo: PlainDate.withCalendar("islamic-civil")})', function () { return D({ days: 45 }).total({ unit: 'month', relativeTo: PD('2020-01-01').withCalendar('islamic-civil') }); });
t('{months: 13}.round({largestUnit: "year", relativeTo: hebrew 5784-01-01 (a leap year)})', function () { return D({ months: 13 }).round({ largestUnit: 'year', relativeTo: T.PlainDate.from({ year: 5784, monthCode: 'M01', day: 1, calendar: 'hebrew' }) }); });
t('{days: 45}.round({largestUnit: "month", relativeTo: "2020-01-01T00:00[UTC][u-ca=coptic]"})', function () { return D({ days: 45 }).round({ largestUnit: 'month', relativeTo: '2020-01-01T00:00[UTC][u-ca=coptic]' }); });
t('{days: 45}.round({largestUnit: "month", relativeTo: {year: 5780, month: 4, day: 1, calendar: "hebrew"}})', function () { return D({ days: 45 }).round({ largestUnit: 'month', relativeTo: { year: 5780, month: 4, day: 1, calendar: 'hebrew' } }); });
t('{days: 45}.round({largestUnit: "month", relativeTo: "2020-01-01[u-ca=gregory]"})', function () { return D({ days: 45 }).round({ largestUnit: 'month', relativeTo: '2020-01-01[u-ca=gregory]' }); });
t('{days: 45}.round({largestUnit: "month", relativeTo: "2020-01-01[u-ca=japanese]"})', function () { return D({ days: 45 }).round({ largestUnit: 'month', relativeTo: '2020-01-01[u-ca=japanese]' }); });
