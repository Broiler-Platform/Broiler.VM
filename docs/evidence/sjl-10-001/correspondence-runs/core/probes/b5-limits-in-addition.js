// B5: addition at the representable limits.
t('PlainDate("+275760-09-13").add({days: 1})', function () { return PD('+275760-09-13').add({ days: 1 }); });
t('PlainDate("-271821-04-19").subtract({days: 1})', function () { return PD('-271821-04-19').subtract({ days: 1 }); });
t('PlainDate("1970-01-01").add({days: 100000000})', function () { return PD('1970-01-01').add({ days: 100000000 }); });
t('PlainDate("1970-01-01").add({days: 100000001})', function () { return PD('1970-01-01').add({ days: 100000001 }); });
t('PlainDate("-271821-04-19").add({days: 0})', function () { return PD('-271821-04-19').add({ days: 0 }); });
t('PlainDateTime("+275760-09-13T00:00").add({hours: 23, minutes: 59})', function () { return PDT('+275760-09-13T00:00').add({ hours: 23, minutes: 59 }); });
t('PlainDateTime("+275760-09-13T00:00").add({days: 1})', function () { return PDT('+275760-09-13T00:00').add({ days: 1 }); });
t('PlainDateTime("-271821-04-19T00:00:00.000000001").subtract({nanoseconds: 1})', function () { return PDT('-271821-04-19T00:00:00.000000001').subtract({ nanoseconds: 1 }); });
t('PlainYearMonth("+275760-09").add({months: 1})', function () { return PYM('+275760-09').add({ months: 1 }); });
t('PlainYearMonth("-271821-04").add({months: 0})', function () { return PYM('-271821-04').add({ months: 0 }); });
t('PlainYearMonth("-271821-05").subtract({months: 1})', function () { return PYM('-271821-05').subtract({ months: 1 }); });
t('PlainYearMonth("+275760-08").add({months: 1})', function () { return PYM('+275760-08').add({ months: 1 }); });
t('Instant("+275760-09-13T00:00Z").add({nanoseconds: 1})', function () { return I('+275760-09-13T00:00Z').add({ nanoseconds: 1 }); });
t('ZonedDateTime(max, "UTC").add({nanoseconds: 1})', function () { return new T.ZonedDateTime(8640000000000000000000n, 'UTC').add({ nanoseconds: 1 }); });
t('ZonedDateTime(max - 1n, "UTC").add({days: 0, nanoseconds: 1})', function () { return new T.ZonedDateTime(8639999999999999999999n, 'UTC').add({ nanoseconds: 1 }); });
t('ZonedDateTime("+275760-09-12T12:00[UTC]").add({days: 1})', function () { return Z('+275760-09-12T12:00[UTC]').add({ days: 1 }); });
t('ZonedDateTime("+275760-09-12T00:00[UTC]").add({days: 1})', function () { return Z('+275760-09-12T00:00[UTC]').add({ days: 1 }); });
