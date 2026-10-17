// Z2: histories before 1970 where backzone differs from the default build.
var cases = [
  ['Europe/Amsterdam', '1850-01-01T12:00Z'], ['Europe/Amsterdam', '1937-07-01T12:00Z'], ['Europe/Amsterdam', '1939-01-15T12:00Z'],
  ['Europe/Amsterdam', '1980-07-01T12:00Z'],
  ['Atlantic/Reykjavik', '1900-01-01T12:00Z'], ['Atlantic/Reykjavik', '1960-01-15T12:00Z'], ['Atlantic/Reykjavik', '1980-01-15T12:00Z'],
  ['Europe/Stockholm', '1900-01-01T12:00Z'], ['Europe/Oslo', '1950-07-01T12:00Z'],
  ['Europe/Brussels', '1937-07-01T12:00Z'], ['Africa/Abidjan', '1960-01-15T12:00Z'],
];
cases.forEach(function (c) { t(c[0] + ' @ ' + c[1] + ' offset', function () { return at(c[1], c[0]).offset; }); });
['Europe/Amsterdam', 'Atlantic/Reykjavik', 'Europe/Brussels', 'Africa/Abidjan'].forEach(function (z) {
  t(z + ' offset changes 1800-1970', function () {
    var n = 0, cur = at('1800-01-01T00:00Z', z), end = Temporal.Instant.from('1970-01-01T00:00Z');
    while (true) { var nx = cur.getTimeZoneTransition('next'); if (nx === null || Temporal.Instant.compare(nx.toInstant(), end) >= 0) break; n++; cur = nx; }
    return n;
  });
});
