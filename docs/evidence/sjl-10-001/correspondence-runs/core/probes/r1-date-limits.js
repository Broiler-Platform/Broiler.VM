// R1: ISO date limits (ISODateWithinLimits: the date at noon is a representable date-time).
t('new PlainDate(-271821, 4, 19)', function () { return new T.PlainDate(-271821, 4, 19); });
t('new PlainDate(-271821, 4, 18)', function () { return new T.PlainDate(-271821, 4, 18); });
t('new PlainDate(275760, 9, 13)', function () { return new T.PlainDate(275760, 9, 13); });
t('new PlainDate(275760, 9, 14)', function () { return new T.PlainDate(275760, 9, 14); });
t('PlainDate.from("-271821-04-19")', function () { return PD('-271821-04-19'); });
t('PlainDate.from("-271821-04-18")', function () { return PD('-271821-04-18'); });
t('PlainDate.from("+275760-09-14")', function () { return PD('+275760-09-14'); });
t('PlainDate.from({year: -271821, month: 4, day: 18})', function () { return PD({ year: -271821, month: 4, day: 18 }); });
t('PlainDate.from({year: 275760, month: 9, day: 13})', function () { return PD({ year: 275760, month: 9, day: 13 }); });
t('PlainMonthDay(4, 18, undefined, -271821)', function () { return new T.PlainMonthDay(4, 18, undefined, -271821); });
t('PlainMonthDay(4, 19, undefined, -271821)', function () { return new T.PlainMonthDay(4, 19, undefined, -271821).toString(); });
t('PlainMonthDay(9, 14, undefined, 275760)', function () { return new T.PlainMonthDay(9, 14, undefined, 275760); });
