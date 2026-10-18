// D2: DefaultTemporalLargestUnit and TemporalDurationFromInternal, through Duration addition.
t('Duration({minutes: 30}).add({minutes: 90})', function () { return D({ minutes: 30 }).add({ minutes: 90 }); });
t('Duration({hours: 1}).add({minutes: 90})', function () { return D({ hours: 1 }).add({ minutes: 90 }); });
t('Duration({days: 1}).add({hours: 24})', function () { return D({ days: 1 }).add({ hours: 24 }); });
t('Duration({days: 1}).add({hours: 25})', function () { return D({ days: 1 }).add({ hours: 25 }); });
t('Duration({seconds: 59}).add({milliseconds: 1001})', function () { return D({ seconds: 59 }).add({ milliseconds: 1001 }); });
t('Duration({microseconds: 1}).add({nanoseconds: 2000})', function () { return D({ microseconds: 1 }).add({ nanoseconds: 2000 }); });
t('Duration({hours: 1}).subtract({hours: 2})', function () { return D({ hours: 1 }).subtract({ hours: 2 }); });
t('Duration({days: 1, hours: -0}).subtract({days: 1})', function () { return D({ days: 1 }).subtract({ days: 1 }); });
t('Duration({seconds: 2**53 - 1}).add({nanoseconds: 999999999})', function () { return D({ seconds: Math.pow(2, 53) - 1 }).add({ nanoseconds: 999999999 }); });
t('Duration({nanoseconds: 2**53 - 1}).add({nanoseconds: 2})', function () { return D({ nanoseconds: Math.pow(2, 53) - 1 }).add({ nanoseconds: 2 }).nanoseconds; });
t('Duration({nanoseconds: 2**53 - 1}).add({nanoseconds: 4})', function () { return D({ nanoseconds: Math.pow(2, 53) - 1 }).add({ nanoseconds: 4 }).nanoseconds; });
t('Duration({milliseconds: 2**53 - 1}).add({milliseconds: 2**53 - 1}).milliseconds', function () { return D({ milliseconds: Math.pow(2, 53) - 1 }).add({ milliseconds: Math.pow(2, 53) - 1 }).milliseconds; });
t('Duration({seconds: 2**53 - 1}).add({seconds: 1})', function () { return D({ seconds: Math.pow(2, 53) - 1 }).add({ seconds: 1 }); });
t('Duration({days: 104249991374}).add({days: 1})', function () { return D({ days: 104249991374 }).add({ days: 1 }); });
t('Duration({days: 104249991374}).add({hours: 1})', function () { return D({ days: 104249991374 }).add({ hours: 1 }); });
