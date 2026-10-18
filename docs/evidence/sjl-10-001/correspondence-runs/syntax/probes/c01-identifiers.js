// C1-C3: the calendar identifiers each engine accepts, their canonical form, and case.
['iso8601'].concat(CALS).concat(['ethiopic-amete-alem', 'islamicc', 'gregorian', 'minguo', 'islamic', 'islamic-rgsa', 'islamic-civil-x',
  'ISO8601', 'Gregory', 'HEBREW', 'Islamic-Umalqura', 'ETHIOPIC-AMETE-ALEM', 'ISLAMICC', 'GREGORIAN', 'iso-8601', 'iso8601 ', 'chinese\u0000',
  'japanese-x', 'gregoryİ', 'İso8601', 'japаnese', 'persian-x', 'ethiopic-amete', 'islamic-tbla ', 'tbla']
).forEach(function (c) {
  k('new PlainDate(2020, 1, 1, ' + JSON.stringify(c) + ').calendarId', function () { return new Temporal.PlainDate(2020, 1, 1, c).calendarId; });
  k('PlainDate.from("2020-01-01[u-ca=' + c + ']").calendarId', function () { return Temporal.PlainDate.from('2020-01-01[u-ca=' + c + ']').calendarId; });
});
t('Intl.supportedValuesOf("calendar") (SJL-9\'s; listed for the record)', function () { return Intl.supportedValuesOf('calendar').join(' '); });
