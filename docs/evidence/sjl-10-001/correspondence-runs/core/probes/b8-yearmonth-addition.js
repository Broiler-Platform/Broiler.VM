// B8: AddDurationToYearMonth (ISO): years and months only, from the first of the month.
t('PlainYearMonth("2020-01").subtract({months: 1})', function () { return PYM('2020-01').subtract({ months: 1 }); });
t('PlainYearMonth("2020-01").add({years: 1, months: 13})', function () { return PYM('2020-01').add({ years: 1, months: 13 }); });
t('PlainYearMonth("2020-01").add({days: 1})', function () { return PYM('2020-01').add({ days: 1 }); });
t('PlainYearMonth("2020-01").add({weeks: 1})', function () { return PYM('2020-01').add({ weeks: 1 }); });
t('PlainYearMonth("2020-01").add({hours: 1})', function () { return PYM('2020-01').add({ hours: 1 }); });
t('PlainYearMonth("2020-01").add({days: 0, months: 1})', function () { return PYM('2020-01').add({ days: 0, months: 1 }); });
t('PlainYearMonth("2020-01").add({months: 1}, {overflow: "reject"})', function () { return PYM('2020-01').add({ months: 1 }, { overflow: 'reject' }); });
t('PlainYearMonth("2020-01").add({months: -1200000})', function () { return PYM('2020-01').add({ months: -1200000 }); });
