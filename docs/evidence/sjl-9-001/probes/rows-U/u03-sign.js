// Row U6: the sign. Only the first unit written carries it (signDisplay "never" after), and a negative duration's
// first unit is -0 where its value is 0 (PartitionDurationFormatPattern, FormatNumericUnits, DurationSign).
function f(opts, d) {
  t('U6 en ' + JSON.stringify(opts) + ' ' + JSON.stringify(d, function (k, v) { return v === 0 && 1 / v < 0 ? '-0' : v; }), function () { return new Intl.DurationFormat('en', opts).format(d); });
}
['short', 'long', 'narrow', 'digital'].forEach(function (s) { f({ style: s }, { hours: -1, minutes: -2, seconds: -3 }); });
f({ style: 'long' }, { minutes: -5, seconds: -0 });
f({ style: 'digital' }, { minutes: -5, seconds: -0 });
f({ hoursDisplay: 'always' }, { hours: 0, minutes: -5 });
f({ style: 'digital' }, { hours: 0, minutes: -5 });
f({ daysDisplay: 'always' }, { years: -1, days: 0 });
f({ style: 'digital' }, { seconds: -1, milliseconds: -500 });
f({ style: 'short', milliseconds: 'numeric' }, { seconds: -1, milliseconds: -500 });
f({ style: 'long' }, { seconds: -0 });
f({ style: 'digital' }, { seconds: -0 });
f({ yearsDisplay: 'always' }, { seconds: -0 });
f({ yearsDisplay: 'always' }, { seconds: -1 });
f({ style: 'long', minutes: 'numeric' }, { hours: -1 });
