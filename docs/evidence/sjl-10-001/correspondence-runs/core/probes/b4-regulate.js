// B4: RegulateISODate and RegulateTime from property bags, constrain and reject.
var rej = { overflow: 'reject' };
t('PlainDate.from({year: 2021, month: 2, day: 31})', function () { return PD({ year: 2021, month: 2, day: 31 }); });
t('PlainDate.from({year: 2021, month: 13, day: 1})', function () { return PD({ year: 2021, month: 13, day: 1 }); });
t('PlainDate.from({year: 2021, month: 2, day: 31}, reject)', function () { return T.PlainDate.from({ year: 2021, month: 2, day: 31 }, rej); });
t('PlainDate.from({year: 2021, month: 0, day: 1})', function () { return PD({ year: 2021, month: 0, day: 1 }); });
t('PlainDate.from({year: 2021, month: 1, day: 0})', function () { return PD({ year: 2021, month: 1, day: 0 }); });
t('PlainTime.from({hour: 25, minute: 61, second: 61, millisecond: 1000})', function () { return PT({ hour: 25, minute: 61, second: 61, millisecond: 1000 }); });
t('PlainTime.from({hour: 25}, reject)', function () { return T.PlainTime.from({ hour: 25 }, rej); });
t('PlainTime.from({hour: -1, minute: -1})', function () { return PT({ hour: -1, minute: -1 }); });
t('PlainTime.from({hour: -1}, reject)', function () { return T.PlainTime.from({ hour: -1 }, rej); });
t('PlainTime.from({microsecond: 1000, nanosecond: 1000})', function () { return PT({ microsecond: 1000, nanosecond: 1000 }); });
t('PlainDateTime.from({year: 2020, month: 1, day: 1, second: 60})', function () { return PDT({ year: 2020, month: 1, day: 1, second: 60 }); });
t('PlainDateTime.from({year: 2020, month: 1, day: 1, second: 60}, reject)', function () { return T.PlainDateTime.from({ year: 2020, month: 1, day: 1, second: 60 }, rej); });
t('PlainYearMonth.from({year: 2020, month: 13})', function () { return PYM({ year: 2020, month: 13 }); });
t('PlainYearMonth.from({year: 2020, month: 13}, reject)', function () { return T.PlainYearMonth.from({ year: 2020, month: 13 }, rej); });
t('PlainDateTime("2020-02-29T12:00").with({year: 2021})', function () { return PDT('2020-02-29T12:00').with({ year: 2021 }); });
t('PlainDateTime("2020-02-29T12:00").with({year: 2021}, reject)', function () { return PDT('2020-02-29T12:00').with({ year: 2021 }, rej); });
t('PlainTime("12:00").with({minute: 75})', function () { return PT('12:00').with({ minute: 75 }); });
