// G14-G22: RFC 9557 annotations. The time zone annotation (first, at most once, its grammar), critical flags,
// unknown keys, the calendar annotation (u-ca) once or several times, key and value grammar, empty and broken
// brackets, the order of annotations.
var anns = ['[u-ca=iso8601]', '[!u-ca=iso8601]', '[u-ca=gregory][u-ca=japanese]', '[u-ca=gregory][!u-ca=japanese]',
  '[!u-ca=gregory][u-ca=japanese]', '[u-ca=iso8601][!u-ca=iso8601]', '[foo=bar]', '[!foo=bar]', '[U-CA=iso8601]',
  '[u-ca=ISO8601]', '[u-ca=iso_8601]', '[u-ca=]', '[=iso8601]', '[]', '[!]', '[u-ca=iso8601', 'u-ca=iso8601]',
  '[u-ca=iso8601]]', '[[u-ca=iso8601]', '[UTC]', '[!UTC]', '[UTC][UTC]', '[UTC][u-ca=iso8601]', '[u-ca=iso8601][UTC]',
  '[foo=bar][UTC]', '[Europe/Berlin]', '[Mars/Olympus]', '[Mars Olympus]', '[+01:00]', '[+01:00:00]', '[-07:00:01]',
  '[+0100]', '[+01]', '[+24:00]', '[a=b]', '[_x=y]', '[x-y=z]', '[1a=b]', '[-a=b]', '[a=b-c]', '[a=b--c]', '[a=-b]',
  '[a=b-]', '[a=b_c]', '[aBc=d]', '[a=b=c]', '[foo=bar][u-ca=gregory]', '[u-ca=gregory][foo=bar][!u-ca=japanese]',
  '[.]', '[..]', '[a/./b]', '[Etc/GMT+5]', '[EST5EDT]', '[a..b]', '[/a]', '[a/]', '[a//b]', '[9a]', '[a-b+c_d.e]',
  '[ UTC]', '[UTC ]', '[u-ca=gregory] ', '[u-ca=١]', '[UTC][!u-ca=iso8601][u-ca=gregory]', '[!foo=bar][u-ca=iso8601]',
  '[u-ca=chinese]', '[u-ca=unknowncal]', '[u-ca=islamic]'];
anns.forEach(function (a) {
  each(['PlainDate', 'PlainDateTime', 'PlainYearMonth', 'PlainMonthDay'], '2020-01-01T12:00' + a);
  each(['PlainTime'], '12:00' + a);
  each(['ZonedDateTime'], '2020-01-01T12:00[UTC]' + a);
  each(['Instant'], '2020-01-01T12:00Z' + a);
});
