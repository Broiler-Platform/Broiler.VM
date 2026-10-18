// B1: BalanceTime and AddTime through PlainTime arithmetic (wrapping at midnight, the date units ignored).
t('PlainTime("23:59:59.999999999").add({nanoseconds: 1})', function () { return PT('23:59:59.999999999').add({ nanoseconds: 1 }); });
t('PlainTime("00:00").subtract({nanoseconds: 1})', function () { return PT('00:00').subtract({ nanoseconds: 1 }); });
t('PlainTime("12:00").add({hours: 1e9})', function () { return PT('12:00').add({ hours: 1e9 }); });
t('PlainTime("12:00").subtract({hours: 2501999792983, minutes: 36})', function () { return PT('12:00').subtract({ hours: 2501999792983, minutes: 36 }); });
t('PlainTime("12:00").add({nanoseconds: 9e24})', function () { return PT('12:00').add({ nanoseconds: 9e24 }); });
t('PlainTime("12:00").add({days: 3, hours: 1})', function () { return PT('12:00').add({ days: 3, hours: 1 }); });
t('PlainTime("12:00").add({years: 1, months: 1, weeks: 1})', function () { return PT('12:00').add({ years: 1, months: 1, weeks: 1 }); });
t('PlainTime("12:00").add({minutes: 90, seconds: 3600})', function () { return PT('12:00').add({ minutes: 90, seconds: 3600 }); });
t('PlainTime("01:00").subtract({minutes: 61})', function () { return PT('01:00').subtract({ minutes: 61 }); });
t('PlainTime("12:00").add("-PT36H")', function () { return PT('12:00').add('-PT36H'); });
