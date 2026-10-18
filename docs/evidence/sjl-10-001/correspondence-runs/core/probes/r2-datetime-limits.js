// R2: ISO date-time limits (ISODateTimeWithinLimits: strictly inside nsMinInstant - nsPerDay .. nsMaxInstant + nsPerDay).
t('new PlainDateTime(-271821, 4, 19)', function () { return new T.PlainDateTime(-271821, 4, 19); });
t('new PlainDateTime(-271821, 4, 19, 0, 0, 0, 0, 0, 1)', function () { return new T.PlainDateTime(-271821, 4, 19, 0, 0, 0, 0, 0, 1); });
t('new PlainDateTime(275760, 9, 13, 23, 59, 59, 999, 999, 999)', function () { return new T.PlainDateTime(275760, 9, 13, 23, 59, 59, 999, 999, 999); });
t('new PlainDateTime(275760, 9, 14)', function () { return new T.PlainDateTime(275760, 9, 14); });
t('PlainDateTime.from("-271821-04-19T00:00")', function () { return PDT('-271821-04-19T00:00'); });
t('PlainDateTime.from("-271821-04-19T00:00:00.000000001")', function () { return PDT('-271821-04-19T00:00:00.000000001'); });
t('PlainDate("-271821-04-19").toPlainDateTime()', function () { return PD('-271821-04-19').toPlainDateTime(); });
t('PlainDate("-271821-04-19").toPlainDateTime("00:00:00.000000001")', function () { return PD('-271821-04-19').toPlainDateTime('00:00:00.000000001'); });
t('PlainDate("+275760-09-13").toPlainDateTime("23:59:59.999999999")', function () { return PD('+275760-09-13').toPlainDateTime('23:59:59.999999999'); });
t('PlainDateTime.from({year: -271821, month: 4, day: 19})', function () { return PDT({ year: -271821, month: 4, day: 19 }); });
t('PlainDateTime.from({year: -271821, month: 4, day: 19, nanosecond: 1})', function () { return PDT({ year: -271821, month: 4, day: 19, nanosecond: 1 }); });
