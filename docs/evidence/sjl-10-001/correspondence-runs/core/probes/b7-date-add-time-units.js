// B7: ToDateDurationRecordWithoutTime: a PlainDate adds the time part as whole days, truncated.
t('PlainDate("2020-01-01").add({hours: 47})', function () { return PD('2020-01-01').add({ hours: 47 }); });
t('PlainDate("2020-01-01").add({hours: 48})', function () { return PD('2020-01-01').add({ hours: 48 }); });
t('PlainDate("2020-01-01").subtract({hours: 25})', function () { return PD('2020-01-01').subtract({ hours: 25 }); });
t('PlainDate("2020-01-01").subtract({hours: 23})', function () { return PD('2020-01-01').subtract({ hours: 23 }); });
t('PlainDate("2020-01-01").add({nanoseconds: 86400e9 - 1})', function () { return PD('2020-01-01').add({ nanoseconds: 86400e9 - 1 }); });
t('PlainDate("2020-01-01").add({days: 1, hours: 24})', function () { return PD('2020-01-01').add({ days: 1, hours: 24 }); });
t('PlainDate("2020-01-31").add({months: 1, hours: 24})', function () { return PD('2020-01-31').add({ months: 1, hours: 24 }); });
t('PlainDate("2020-01-01").add({seconds: 2**53 - 1})', function () { return PD('2020-01-01').add({ seconds: Math.pow(2, 53) - 1 }); });
t('PlainDate("2020-01-01").add({hours: 2501999792983})', function () { return PD('2020-01-01').add({ hours: 2501999792983 }); });
