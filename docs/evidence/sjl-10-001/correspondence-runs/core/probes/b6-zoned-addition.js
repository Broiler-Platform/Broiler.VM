// B6: AddZonedDateTime: the date part on the wall clock, then the time part on the exact timeline.
t('NY 2026-03-07T02:30 + {days: 1}', function () { return Z('2026-03-07T02:30-05:00[America/New_York]').add({ days: 1 }); });
t('NY 2026-03-07T02:30 + {hours: 24}', function () { return Z('2026-03-07T02:30-05:00[America/New_York]').add({ hours: 24 }); });
t('NY 2026-03-07T02:30 + {days: 1}, disambiguation n/a, reject', function () { return Z('2026-03-07T02:30-05:00[America/New_York]').add({ days: 1 }, { overflow: 'reject' }); });
t('NY 2026-11-01T00:30-04:00 + {days: 1}', function () { return Z('2026-11-01T00:30-04:00[America/New_York]').add({ days: 1 }); });
t('NY 2026-11-01T00:30-04:00 + {hours: 24}', function () { return Z('2026-11-01T00:30-04:00[America/New_York]').add({ hours: 24 }); });
t('NY 2026-10-31T01:30-04:00 + {days: 1}', function () { return Z('2026-10-31T01:30-04:00[America/New_York]').add({ days: 1 }); });
t('NY 2026-11-01T01:30-05:00 - {days: 1}', function () { return Z('2026-11-01T01:30-05:00[America/New_York]').subtract({ days: 1 }); });
t('NY 2026-03-08T12:00-04:00 - {days: 1, hours: 12}', function () { return Z('2026-03-08T12:00-04:00[America/New_York]').subtract({ days: 1, hours: 12 }); });
t('NY 2026-01-31T02:30-05:00 + {months: 1, weeks: 1}', function () { return Z('2026-01-31T02:30-05:00[America/New_York]').add({ months: 1, weeks: 1 }); });
