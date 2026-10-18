// R3: ISO year-month limits (ISOYearMonthWithinLimits: April -271821 to September 275760).
t('new PlainYearMonth(-271821, 4)', function () { return new T.PlainYearMonth(-271821, 4); });
t('new PlainYearMonth(-271821, 3)', function () { return new T.PlainYearMonth(-271821, 3); });
t('new PlainYearMonth(275760, 9)', function () { return new T.PlainYearMonth(275760, 9); });
t('new PlainYearMonth(275760, 10)', function () { return new T.PlainYearMonth(275760, 10); });
t('new PlainYearMonth(-271821, 4, undefined, 1)', function () { return new T.PlainYearMonth(-271821, 4, undefined, 1).toString(); });
t('new PlainYearMonth(-271821, 4, undefined, 19)', function () { return new T.PlainYearMonth(-271821, 4, undefined, 19).toString(); });
t('PlainYearMonth.from("-271821-04")', function () { return PYM('-271821-04'); });
t('PlainYearMonth.from("-271821-03")', function () { return PYM('-271821-03'); });
t('PlainYearMonth.from("+275760-09")', function () { return PYM('+275760-09'); });
t('PlainYearMonth.from({year: 275760, month: 10})', function () { return PYM({ year: 275760, month: 10 }); });
t('PlainDate("-271821-04-19").toPlainYearMonth()', function () { return PD('-271821-04-19').toPlainYearMonth(); });
