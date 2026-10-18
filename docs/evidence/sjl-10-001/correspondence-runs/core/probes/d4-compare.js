// D4: Temporal.Duration.compare.
var nySpring = '2026-03-08T00:00-05:00[America/New_York]';
t('compare({hours: 24}, {days: 1})', function () { return T.Duration.compare({ hours: 24 }, { days: 1 }); });
t('compare({days: 1}, {hours: 25})', function () { return T.Duration.compare({ days: 1 }, { hours: 25 }); });
t('compare({months: 1}, {days: 30})', function () { return T.Duration.compare({ months: 1 }, { days: 30 }); });
t('compare({months: 1}, {days: 30}, {relativeTo: "2020-02-01"})', function () { return T.Duration.compare({ months: 1 }, { days: 30 }, { relativeTo: '2020-02-01' }); });
t('compare({months: 1}, {days: 30}, {relativeTo: "2020-01-01"})', function () { return T.Duration.compare({ months: 1 }, { days: 30 }, { relativeTo: '2020-01-01' }); });
t('compare({days: 1}, {hours: 24}, {relativeTo: NY spring day})', function () { return T.Duration.compare({ days: 1 }, { hours: 24 }, { relativeTo: nySpring }); });
t('compare({days: 1}, {hours: 23}, {relativeTo: NY spring day})', function () { return T.Duration.compare({ days: 1 }, { hours: 23 }, { relativeTo: nySpring }); });
t('compare({months: 1}, {months: 1}) (identical, no relativeTo)', function () { return T.Duration.compare({ months: 1 }, { months: 1 }); });
t('compare({months: 1}, {months: 1}, {relativeTo: "bad"})', function () { return T.Duration.compare({ months: 1 }, { months: 1 }, { relativeTo: 'bad' }); });
t('compare({years: 1, seconds: 9e15}, {years: 1}, {relativeTo: "2020-01-01"})', function () { return T.Duration.compare({ years: 1, seconds: 9e15 }, { years: 1 }, { relativeTo: '2020-01-01' }); });
t('compare({years: 1, seconds: 9e15}, {years: 1, seconds: 9e15 - 1}, {relativeTo: "2020-01-01"})', function () { return T.Duration.compare({ years: 1, seconds: 9e15 }, { years: 1, seconds: 9e15 - 1 }, { relativeTo: '2020-01-01' }); });
t('compare({years: 1, seconds: 9007199254740991 - 31622399}, {years: 1}, {relativeTo: "2020-01-01"})', function () { return T.Duration.compare({ years: 1, seconds: 9007199254740991 - 31622399 }, { years: 1 }, { relativeTo: '2020-01-01' }); });
t('compare({days: 200000000}, {days: 1}, {relativeTo: "2020-01-01"})', function () { return T.Duration.compare({ days: 200000000 }, { days: 1 }, { relativeTo: '2020-01-01' }); });
t('compare({weeks: 1}, {days: 7}, {relativeTo: "2020-01-01"})', function () { return T.Duration.compare({ weeks: 1 }, { days: 7 }, { relativeTo: '2020-01-01' }); });
t('compare({years: 1}, {days: 365}, {relativeTo: "2020-01-01[u-ca=hebrew]"})', function () { return T.Duration.compare({ years: 1 }, { days: 365 }, { relativeTo: '2020-01-01[u-ca=hebrew]' }); });
t('compare({months: 1}, {days: 29}, {relativeTo: PlainDate hebrew 5780-05-01})', function () { return T.Duration.compare({ months: 1 }, { days: 29 }, { relativeTo: T.PlainDate.from({ year: 5780, monthCode: 'M05', day: 1, calendar: 'hebrew' }) }); });
