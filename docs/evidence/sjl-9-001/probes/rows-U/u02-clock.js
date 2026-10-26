// Rows U5 and U1: the clock (FormatNumericUnits, FormatNumericHours/Minutes/Seconds) and its place in the list
// (PartitionDurationFormatPattern, ListFormatParts), formatted in en.
function f(opts, d) {
  t('U5 en ' + JSON.stringify(opts) + ' ' + JSON.stringify(d), function () { return new Intl.DurationFormat('en', opts).format(d); });
}
var A = { style: 'long', minutes: 'numeric' };
[{ minutes: 5 }, { seconds: 0 }, { hours: 1, minutes: 2 }, { minutes: 1, seconds: 30 }, { days: 1 }, { minutes: -5, seconds: -0 }].forEach(function (d) { f(A, d); });
var B = { style: 'long', hours: 'numeric' };
[{ hours: 1 }, { hours: 1, seconds: 5 }, { minutes: 3 }, { days: 2, hours: 1 }, { hours: 0 }].forEach(function (d) { f(B, d); });
f({ hours: 'numeric' }, { hours: 2, minutes: 3 });
f({ minutes: 'numeric' }, { hours: 2, minutes: 3 });
f({ minutes: 'numeric' }, { hours: 2 });
var C = { style: 'digital' };
[{ hours: 1, minutes: 2, seconds: 3 }, { minutes: 5 }, { hours: 12345, minutes: 1 }, { days: 1, hours: 25 }, { seconds: 7, milliseconds: 8, microseconds: 9 }, { seconds: 0 }, { years: 1, hours: 1 }].forEach(function (d) { f(C, d); });
f({ style: 'digital', fractionalDigits: 2 }, { seconds: 7, milliseconds: 999 });
f({ style: 'digital', hoursDisplay: 'auto' }, { minutes: 5 });
f({ style: 'digital', hoursDisplay: 'auto' }, { seconds: 5 });
f({ style: 'short', seconds: 'numeric' }, { minutes: 1, seconds: 2 });
f({ style: 'long', hours: 'numeric', minutesDisplay: 'auto', secondsDisplay: 'auto' }, { hours: 1, seconds: 5 });
f({ style: 'digital', hours: '2-digit' }, { hours: 1 });
f({ style: 'digital', hours: 'long' }, { hours: 1, minutes: 2, seconds: 3 });
f({ style: 'long', seconds: 'numeric' }, { minutes: 1 });
