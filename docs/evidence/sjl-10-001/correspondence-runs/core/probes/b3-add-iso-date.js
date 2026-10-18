// B3: AddISODate (BalanceISOYearMonth, then RegulateISODate, then AddDaysToISODate) under both overflows.
var rej = { overflow: 'reject' };
t('PlainDate("2020-01-31").add({months: 1})', function () { return PD('2020-01-31').add({ months: 1 }); });
t('PlainDate("2020-01-31").add({months: 1}, reject)', function () { return PD('2020-01-31').add({ months: 1 }, rej); });
t('PlainDate("2020-02-29").add({years: 1})', function () { return PD('2020-02-29').add({ years: 1 }); });
t('PlainDate("2020-02-29").add({years: 1}, reject)', function () { return PD('2020-02-29').add({ years: 1 }, rej); });
t('PlainDate("2020-03-31").subtract({months: 1})', function () { return PD('2020-03-31').subtract({ months: 1 }); });
t('PlainDate("2020-01-31").add({months: 1, days: 1})', function () { return PD('2020-01-31').add({ months: 1, days: 1 }); });
t('PlainDate("2019-01-31").add({years: 1, months: 1})', function () { return PD('2019-01-31').add({ years: 1, months: 1 }); });
t('PlainDate("2020-01-01").add({weeks: 2, days: 3})', function () { return PD('2020-01-01').add({ weeks: 2, days: 3 }); });
t('PlainDate("2020-03-15").subtract({months: 15})', function () { return PD('2020-03-15').subtract({ months: 15 }); });
t('PlainDate("2020-01-01").add({months: 1200000})', function () { return PD('2020-01-01').add({ months: 1200000 }); });
t('PlainDate("2020-01-01").add({days: 1e11})', function () { return PD('2020-01-01').add({ days: 1e11 }); });
t('PlainDate("2020-01-01").add({years: 1e9 + 1})', function () { return PD('2020-01-01').add({ years: 1e9 + 1 }); });
t('PlainDateTime("2020-01-31T10:00").add({months: 1}, reject)', function () { return PDT('2020-01-31T10:00').add({ months: 1 }, rej); });
t('PlainDateTime("2020-01-31T10:00").add({months: 1})', function () { return PDT('2020-01-31T10:00').add({ months: 1 }); });
t('ZonedDateTime("2020-01-31T12:00[UTC]").add({months: 1}, reject)', function () { return Z('2020-01-31T12:00[UTC]').add({ months: 1 }, rej); });
t('PlainDate("2020-01-01").add({months: 1}, {overflow: "bogus"})', function () { return PD('2020-01-01').add({ months: 1 }, { overflow: 'bogus' }); });
t('PlainDate("2020-01-01").add({months: 1}, null)', function () { return PD('2020-01-01').add({ months: 1 }, null); });
