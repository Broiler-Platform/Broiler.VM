// R4: instant limits (IsValidEpochNanoseconds: |ns| <= 8.64e21) and wall clocks at them.
t('Instant.fromEpochNanoseconds(8640000000000000000000n)', function () { return T.Instant.fromEpochNanoseconds(8640000000000000000000n); });
t('Instant.fromEpochNanoseconds(8640000000000000000001n)', function () { return T.Instant.fromEpochNanoseconds(8640000000000000000001n); });
t('Instant.fromEpochNanoseconds(-8640000000000000000000n)', function () { return T.Instant.fromEpochNanoseconds(-8640000000000000000000n); });
t('Instant.fromEpochNanoseconds(-8640000000000000000001n)', function () { return T.Instant.fromEpochNanoseconds(-8640000000000000000001n); });
t('Instant.fromEpochMilliseconds(8.64e15)', function () { return T.Instant.fromEpochMilliseconds(8.64e15); });
t('Instant.fromEpochMilliseconds(8.64e15 + 1)', function () { return T.Instant.fromEpochMilliseconds(8.64e15 + 1); });
t('Instant.from("-271821-04-20T00:00Z")', function () { return I('-271821-04-20T00:00Z'); });
t('Instant.from("-271821-04-19T23:59:59.999999999Z")', function () { return I('-271821-04-19T23:59:59.999999999Z'); });
t('Instant.from("+275760-09-13T00:00Z")', function () { return I('+275760-09-13T00:00Z'); });
t('Instant.from("+275760-09-13T00:00:00.000000001Z")', function () { return I('+275760-09-13T00:00:00.000000001Z'); });
t('Instant.from("+275760-09-13T01:00+01:00")', function () { return I('+275760-09-13T01:00+01:00'); });
t('Instant.from("-271821-04-19T23:00-01:00")', function () { return I('-271821-04-19T23:00-01:00'); });
t('new ZonedDateTime(max, "Etc/GMT-14").toPlainDateTime()', function () { return new T.ZonedDateTime(8640000000000000000000n, 'Etc/GMT-14').toPlainDateTime(); });
t('new ZonedDateTime(max, "Etc/GMT-14").toPlainDate()', function () { return new T.ZonedDateTime(8640000000000000000000n, 'Etc/GMT-14').toPlainDate(); });
t('new ZonedDateTime(min, "Etc/GMT+12").toPlainDateTime()', function () { return new T.ZonedDateTime(-8640000000000000000000n, 'Etc/GMT+12').toPlainDateTime(); });
t('new ZonedDateTime(min, "-23:59").toPlainDateTime()', function () { return new T.ZonedDateTime(-8640000000000000000000n, '-23:59').toPlainDateTime(); });
t('new ZonedDateTime(max + 1n, "UTC")', function () { return new T.ZonedDateTime(8640000000000000000001n, 'UTC'); });
