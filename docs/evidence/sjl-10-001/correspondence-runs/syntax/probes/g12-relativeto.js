// G53-G55: relativeTo given as a string (ParseISODateTime with TemporalDateTimeString[+Zoned] and [~Zoned]): a date or
// date-time, a zoned one where it has a time zone annotation, Z only with an annotation; the grammar of the rest.
['2020-01-01', '2020-01-01T00:00', '2020-01-01T00:00[UTC]', '2020-01-01T00:00Z[UTC]', '2020-01-01T00:00Z', '2020-01-01T00:00+01:00',
 '2020-01-01T00:00+01:00[Europe/Berlin]', '2020-01-01T00:00+02:00[Europe/Berlin]', '2020-01-01[]', '2020-01-01[u-ca=iso8601][UTC]',
 '2020-01-01[foo=bar][UTC]', '2020-01-01[!foo=bar]', '2020-01-01[u-ca=hebrew]', '2020-01-01T00:00+25:00', '2020-01-01T00:00+01:0000',
 '2020-01', '01-01', 'T00:00', '00:00', '2020-02-30', '2020-01-01T24:00', '2020-01-01[Mars Olympus]', '2020-01-01[UTC][UTC]',
 '2020-01-01T00:00−01:00', '٢٠٢٠-٠١-٠١', '2020-01-01T12:3045', '2020-01-01T00:00+01:00:00.5[UTC]',
 '2020-01-01T00:00Z[u-ca=hebrew]', '2020-01-01T00:00[u-ca=gregory][!u-ca=japanese]'].forEach(function (s) {
  t1('Duration#round relativeTo ' + JSON.stringify(s), function () { return Temporal.Duration.from('P1M').round({ smallestUnit: 'day', relativeTo: s }).toString(); });
  k('Duration#total relativeTo ' + JSON.stringify(s), function () { return Temporal.Duration.from('P1M').total({ unit: 'day', relativeTo: s }); });
});
