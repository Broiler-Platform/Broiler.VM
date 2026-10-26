// Row U11: fractional units (ComputeFractionalDigits, NextUnitFractional), summed exactly as a mathematical
// value and written with roundingMode "trunc" and fractionalDigits.
function f(opts, d, label) {
  t('U11 en ' + JSON.stringify(opts) + ' ' + (label || JSON.stringify(d)), function () { return new Intl.DurationFormat('en', opts).format(d); });
}
var D = { style: 'digital' };
f(D, { seconds: 4503599627370497, nanoseconds: 1 });
f(D, { seconds: 10000000, nanoseconds: 1 });
f(D, { seconds: 1, milliseconds: 2, microseconds: 3, nanoseconds: 9007199254740991 });
f(D, { milliseconds: 4503599627370497000, microseconds: 4503599627370495000000 }, '{milliseconds: 4503599627370497000, microseconds: 4503599627370495000000}');
f(D, { seconds: 1, nanoseconds: 5 });
f(D, { microseconds: 1e20 }, '{microseconds: 1e20}');
f(D, { seconds: 0, nanoseconds: 1 });
f({ style: 'digital', fractionalDigits: 0 }, { seconds: 7, milliseconds: 999, microseconds: 999, nanoseconds: 999 });
f({ style: 'digital', fractionalDigits: 3 }, { seconds: 7, milliseconds: 999, microseconds: 999, nanoseconds: 999 });
f({ style: 'digital', fractionalDigits: 9 }, { seconds: 7 });
f({ style: 'short', milliseconds: 'numeric' }, { milliseconds: 1500 });
f({ style: 'long', seconds: 'numeric', fractionalDigits: 3 }, { seconds: 1, milliseconds: 234, microseconds: 567 });
f({ style: 'narrow', microseconds: 'numeric' }, { milliseconds: 1, microseconds: 2, nanoseconds: 3 });
f({ style: 'long', milliseconds: 'numeric' }, { nanoseconds: 1 });
f({ style: 'long', milliseconds: 'numeric' }, { minutes: 1 });
f({ style: 'long', milliseconds: 'numeric', fractionalDigits: 2 }, { seconds: 2, milliseconds: 999 });
f({ style: 'short', microseconds: 'numeric' }, { seconds: 1, milliseconds: 2, microseconds: 3, nanoseconds: 4 });
f({ style: 'short', nanoseconds: 'numeric' }, { seconds: 1, milliseconds: 2, microseconds: 3, nanoseconds: 4 });
f(D, { seconds: -4503599627370497, nanoseconds: -1 });
