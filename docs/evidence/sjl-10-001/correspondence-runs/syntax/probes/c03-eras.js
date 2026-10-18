// C10-C15: eras and era years. Every era name either engine knows, in every calendar, with eraYear 5; the era
// and era year a date answers at each era's edge; eraYear zero and negative; year against era; era alone.
var eras = ['ce', 'bce', 'ad', 'bc', 'CE', 'Ce', 'gregory', 'gregory-inverse', 'be', 'roc', 'broc', 'minguo', 'before-roc', 'reiwa',
  'heisei', 'showa', 'taisho', 'meiji', 'am', 'aa', 'AM', 'coptic', 'hebrew', 'incarnation', 'mundi', 'ethiopic', 'ethioaa', 'shaka',
  'saka', 'ap', 'persian', 'ah', 'bh', 'AH', 'islamic', 'x', ''];
['iso8601'].concat(CALS).forEach(function (cal) {
  eras.forEach(function (era) {
    k(cal + ' era ' + JSON.stringify(era) + ' eraYear 5', function () {
      var d = Temporal.PlainDate.from({ calendar: cal, era: era, eraYear: 5, monthCode: 'M01', day: 1 });
      return d.era + ' ' + d.eraYear + ' year ' + d.year + ' iso ' + d.toString({ calendarName: 'never' }); });
  });
});
// The era and era year of ISO dates at era edges, in each calendar.
['-271821-04-19', '-000001-12-31', '0000-01-01', '0000-12-31', '0001-01-01', '0284-08-29', '0284-08-30', '0008-08-27', '0008-08-28',
 '0622-07-15', '0622-07-16', '0622-07-19', '0079-03-21', '0079-03-22', '1867-12-31', '1868-01-01', '1872-12-31', '1873-01-01', '1911-12-31',
 '1912-01-01', '1912-07-29', '1912-07-30', '1926-12-24', '1926-12-25', '1989-01-07', '1989-01-08', '2019-04-30', '2019-05-01', '+275760-09-13'
].forEach(function (iso) {
  CALS.forEach(function (cal) {
    t(cal + ' ' + iso, function () { var d = Temporal.PlainDate.from(iso).withCalendar(cal); return d.era + ' ' + d.eraYear + ' / ' + d.year + ' ' + d.monthCode + ' ' + d.day; });
  });
});
// eraYear zero and negative, overspecified, and incomplete.
[['gregory', 'ce', 0], ['gregory', 'bce', 0], ['gregory', 'bce', -1], ['japanese', 'reiwa', 0], ['japanese', 'reiwa', -1], ['japanese', 'meiji', 1],
 ['japanese', 'meiji', 6], ['japanese', 'heisei', 31], ['japanese', 'heisei', 40], ['roc', 'broc', 0], ['roc', 'roc', 0], ['ethiopic', 'aa', 5500],
 ['ethiopic', 'aa', 5501], ['ethiopic', 'am', 0], ['islamic-civil', 'bh', 0], ['islamic-civil', 'ah', 0], ['coptic', 'am', -5], ['hebrew', 'am', 0],
 ['persian', 'ap', -1], ['indian', 'shaka', 0], ['buddhist', 'be', 0]].forEach(function (c) {
  k(c[0] + ' era ' + c[1] + ' eraYear ' + c[2], function () {
    var d = Temporal.PlainDate.from({ calendar: c[0], era: c[1], eraYear: c[2], monthCode: 'M01', day: 1 });
    return d.era + ' ' + d.eraYear + ' year ' + d.year + ' iso ' + d.toString({ calendarName: 'never' }); });
});
[['gregory', { year: 2020, era: 'ce', eraYear: 2019 }], ['gregory', { year: 2020, era: 'ce', eraYear: 2020 }], ['gregory', { era: 'ce' }],
 ['gregory', { eraYear: 2020 }], ['gregory', { year: 2020, era: 'ce' }], ['gregory', { year: 2020, eraYear: 2020 }], ['chinese', { era: 'ce', eraYear: 2020 }],
 ['chinese', { year: 2020, era: 'x', eraYear: 1 }], ['iso8601', { era: 'ce', eraYear: 2020 }], ['iso8601', { year: 2020, era: 'x', eraYear: 1 }],
 ['japanese', { year: 2019, era: 'heisei', eraYear: 31 }], ['japanese', { year: 2019, era: 'reiwa', eraYear: 1 }]].forEach(function (c) {
  k(c[0] + ' ' + JSON.stringify(c[1]), function () {
    var bag = { calendar: c[0], monthCode: 'M01', day: 1 }; for (var key in c[1]) bag[key] = c[1][key];
    var d = Temporal.PlainDate.from(bag); return d.era + ' ' + d.eraYear + ' year ' + d.year + ' iso ' + d.toString({ calendarName: 'never' }); });
});
