// B2: BalanceISODateTime through PlainDateTime and Instant arithmetic.
t('PlainDateTime("2020-12-31T23:00").add({hours: 25})', function () { return PDT('2020-12-31T23:00').add({ hours: 25 }); });
t('PlainDateTime("2020-01-01T00:00").subtract({minutes: 1})', function () { return PDT('2020-01-01T00:00').subtract({ minutes: 1 }); });
t('PlainDateTime("2020-01-01T12:00").subtract({hours: 36})', function () { return PDT('2020-01-01T12:00').subtract({ hours: 36 }); });
t('PlainDateTime("2020-02-28T12:00").add({days: 1, hours: 12})', function () { return PDT('2020-02-28T12:00').add({ days: 1, hours: 12 }); });
t('PlainDateTime("2020-01-31T23:00").add({months: 1, hours: 2})', function () { return PDT('2020-01-31T23:00').add({ months: 1, hours: 2 }); });
t('PlainDateTime("2020-01-01T00:00").add({nanoseconds: 86400e9 * 366})', function () { return PDT('2020-01-01T00:00').add({ nanoseconds: 86400e9 * 366 }); });
t('PlainDateTime("2020-01-01T00:00").add({seconds: 2**53 - 1})', function () { return PDT('2020-01-01T00:00').add({ seconds: Math.pow(2, 53) - 1 }); });
t('Instant("2020-01-01T00:00Z").add({hours: 48, nanoseconds: 1})', function () { return I('2020-01-01T00:00Z').add({ hours: 48, nanoseconds: 1 }); });
t('Instant("2020-01-01T00:00Z").add({days: 1})', function () { return I('2020-01-01T00:00Z').add({ days: 1 }); });
t('Instant("2020-01-01T00:00Z").subtract({hours: 2501999792983})', function () { return I('2020-01-01T00:00Z').subtract({ hours: 2501999792983 }); });
