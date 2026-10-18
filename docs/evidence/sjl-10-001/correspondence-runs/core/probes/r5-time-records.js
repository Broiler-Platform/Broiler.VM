// R5: time records (IsValidTime) and ToIntegerWithTruncation of their fields.
t('new PlainTime(23, 59, 59, 999, 999, 999)', function () { return new T.PlainTime(23, 59, 59, 999, 999, 999); });
t('new PlainTime(24)', function () { return new T.PlainTime(24); });
t('new PlainTime(0, 60)', function () { return new T.PlainTime(0, 60); });
t('new PlainTime(0, 0, 0, 1000)', function () { return new T.PlainTime(0, 0, 0, 1000); });
t('new PlainTime(-1)', function () { return new T.PlainTime(-1); });
t('new PlainTime(1.9, 2.9)', function () { return new T.PlainTime(1.9, 2.9); });
t('new PlainTime(-0.5)', function () { return new T.PlainTime(-0.5); });
t('new PlainTime(NaN)', function () { return new T.PlainTime(NaN); });
t('new PlainTime(Infinity)', function () { return new T.PlainTime(Infinity); });
t('new PlainTime(undefined, 5)', function () { return new T.PlainTime(undefined, 5); });
t('new PlainDateTime(2020, 1, 1, 24)', function () { return new T.PlainDateTime(2020, 1, 1, 24); });
t('new PlainDate(2020, 2, 30)', function () { return new T.PlainDate(2020, 2, 30); });
t('new PlainDate(2021, 2, 29)', function () { return new T.PlainDate(2021, 2, 29); });
t('new PlainDate(2020, 13, 1)', function () { return new T.PlainDate(2020, 13, 1); });
t('new PlainDate(2020, 0, 1)', function () { return new T.PlainDate(2020, 0, 1); });
t('new PlainDate(2020.7, 1.2, 1.9)', function () { return new T.PlainDate(2020.7, 1.2, 1.9); });
