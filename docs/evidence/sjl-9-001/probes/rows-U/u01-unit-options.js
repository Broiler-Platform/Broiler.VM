// Rows U1-U4: GetDurationUnitOptions (ECMA-402 3d4c558 sec-getdurationunitoptions) and ValidateDurationUnitStyle
// (sec-validatedurationunitstyle): each unit's resolved style and display, and the RangeErrors.
// ro() writes resolvedOptions as "unit=style/display" for the ten units, after locale, numberingSystem and style.
var U = ['years', 'months', 'weeks', 'days', 'hours', 'minutes', 'seconds', 'milliseconds', 'microseconds', 'nanoseconds'];
function ro(df) {
  var o = df.resolvedOptions();
  var s = o.locale + ' ' + o.numberingSystem + ' ' + o.style;
  for (var i = 0; i < U.length; i++) s += ' ' + U[i] + '=' + o[U[i]] + '/' + o[U[i] + 'Display'];
  if (o.fractionalDigits !== undefined) s += ' fractionalDigits=' + o.fractionalDigits;
  return R(s);
}
var bags = [
  {}, { style: 'digital' },
  { style: 'long', minutes: 'numeric' },
  { style: 'long', hours: 'numeric' },
  { style: 'long', hours: '2-digit' },
  { hours: 'numeric' },
  { minutes: 'numeric' },
  { style: 'long', seconds: 'numeric' },
  { style: 'short', milliseconds: 'numeric' },
  { style: 'narrow', microseconds: 'numeric', fractionalDigits: 0 },
  { style: 'long', seconds: 'numeric', fractionalDigits: 3 },
  { hours: 'numeric', minutesDisplay: 'auto' },
  { style: 'digital', hoursDisplay: 'auto' },
  { style: 'digital', hours: 'long' },
  { style: 'digital', hours: '2-digit' },
  { style: 'digital', fractionalDigits: 2.9 },
  { fractionalDigits: '3' },
  { seconds: 'numeric', secondsDisplay: 'auto', milliseconds: 'numeric', millisecondsDisplay: 'auto' }
];
bags.forEach(function (b) {
  t('U1-U4 en ' + JSON.stringify(b), function () { return ro(new Intl.DurationFormat('en', b)); });
});
var bad = [
  { milliseconds: 'numeric', millisecondsDisplay: 'always' },
  { style: 'digital', millisecondsDisplay: 'always' },
  { hours: 'numeric', nanosecondsDisplay: 'always' },
  { seconds: 'numeric', microseconds: 'numeric', microsecondsDisplay: 'always' },
  { style: 'long', milliseconds: 'numeric', microsecondsDisplay: 'always' },
  { hours: 'numeric', minutes: 'long' },
  { milliseconds: 'numeric', microseconds: 'long' },
  { seconds: '2-digit', milliseconds: 'short' },
  { style: 'full' },
  { hours: 'digital' },
  { fractionalDigits: 10 },
  { fractionalDigits: -1 },
  { fractionalDigits: NaN },
  { numberingSystem: 'latn-' },
  { hoursDisplay: 'never' }
];
bad.forEach(function (b) {
  t('U3-U4 en ' + JSON.stringify(b, function (k, v) { return typeof v === 'number' && v !== v ? 'NaN' : v; }), function () { return ro(new Intl.DurationFormat('en', b)); });
});
t('U4 resolvedOptions keys, {fractionalDigits: 1}', function () { return Object.keys(new Intl.DurationFormat('en', { fractionalDigits: 1 }).resolvedOptions()).join(','); });
