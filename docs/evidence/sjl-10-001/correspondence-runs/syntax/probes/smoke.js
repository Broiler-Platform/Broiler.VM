// Smoke: both engines answer.
t('typeof Temporal', function () { return typeof Temporal; });
t('PlainDate.from', function () { return Temporal.PlainDate.from('2020-01-01').toString(); });
k('PlainDate.from bad', function () { return Temporal.PlainDate.from('2020-13-01').toString(); });
