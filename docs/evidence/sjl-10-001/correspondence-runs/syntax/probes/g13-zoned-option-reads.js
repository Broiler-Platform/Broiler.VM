// G56: when ZonedDateTime.from reads its options relative to the string's parse: an invalid string (grammar, date,
// zone, calendar) throws before any option is read; a valid one reads disambiguation, offset, overflow in that order.
function opts(log) {
  var o = {};
  ['disambiguation', 'offset', 'overflow'].forEach(function (key) {
    Object.defineProperty(o, key, { get: function () { log.push(key); return undefined; } });
  });
  return o;
}
['2020-01-01T00:00[UTC]', '2020-01-01T00:00+01:00[Europe/Berlin]', '2020-01-01T00:00+02:00[Europe/Berlin]', '2020-13-01T00:00[UTC]',
 '2020-01-01T00:00[Mars/Olympus]', '2020-01-01T00:00[UTC][u-ca=bogus]', '2020-01-01T00:00[]', '2020-01-01T00:00', '2020-01-01T00:00+25:00[UTC]',
 '2020-01-01T00:00[UTC][!foo=bar]'].forEach(function (s) {
  var log = [];
  k('ZonedDateTime.from(' + JSON.stringify(s) + ', options)', function () {
    try { return Temporal.ZonedDateTime.from(s, opts(log)).toString(); } finally { out('  reads for ' + JSON.stringify(s) + ': ' + log.join(',')); } });
});
