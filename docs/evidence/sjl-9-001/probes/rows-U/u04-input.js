// Rows U7-U10: the argument of format and formatToParts. ECMA-402 3d4c558 reads it with ToDurationRecord
// (sec-todurationrecord); tc39/proposal-temporal@e8cc03fc's intl.html replaces that with ToTemporalDuration, which
// both engines follow (JSD-0078 D1): a property bag's ten fields are read once each, in alphabetical order, each
// converted as it is read (U7); a Temporal.Duration is read from its slots (U8); a String is parsed (U9); and the
// refusals (U10).
var df = new Intl.DurationFormat('en');
var U = ['years', 'months', 'weeks', 'days', 'hours', 'minutes', 'seconds', 'milliseconds', 'microseconds', 'nanoseconds'];
function logged(values, log) {
  var o = {};
  U.forEach(function (u) {
    Object.defineProperty(o, u, { enumerable: true, get: function () { log.push('get ' + u); return values[u]; } });
  });
  return o;
}
t('U7 the reads of format({hours: 1, minutes: 2})', function () {
  var log = []; var r = df.format(logged({ hours: 1, minutes: 2 }, log)); return log.join(', ') + ' => ' + r;
});
t('U7 the reads of formatToParts({seconds: 3})', function () {
  var log = []; var r = df.formatToParts(logged({ seconds: 3 }, log)); return log.join(', ') + ' => ' + r.length + ' parts';
});
t('U7 reads and conversions interleaved, {days, hours} as objects with valueOf', function () {
  var log = [];
  function v(name, n) { return { valueOf: function () { log.push('valueOf ' + name); return n; } }; }
  var o = logged({ days: v('days', 1), hours: v('hours', 2) }, log);
  var r = df.format(o); return log.join(', ') + ' => ' + r;
});
t('U7 valueOf calls of a field object {valueOf: 7}', function () {
  var n = 0; var r = df.format({ hours: { valueOf: function () { n++; return 7; } } }); return [n, r];
});
t('U7 a getter answering 1, then -1, then 1', function () {
  var k = 0; var o = {}; Object.defineProperty(o, 'hours', { get: function () { k++; return k === 2 ? -1 : 1; } }); var r = df.format(o); return [k, r];
});
t('U7 a getter answering 1, then 1.5', function () {
  var k = 0; var o = {}; Object.defineProperty(o, 'hours', { get: function () { k++; return k === 1 ? 1 : 1.5; } }); var r = df.format(o); return [k, r];
});
t('U7 a getter throwing on its second read', function () {
  var k = 0; var o = {}; Object.defineProperty(o, 'hours', { get: function () { k++; if (k > 1) throw new EvalError('second read'); return 1; } }); return df.format(o);
});
t('U7 format({hours: "2"})', function () { return df.format({ hours: '2' }); });
t('U7 format({hours: -0})', function () { return df.format({ hours: -0 }); });
t('U7 format({hours: undefined, minutes: 1})', function () { return df.format({ hours: undefined, minutes: 1 }); });
t('U8 format(new Temporal.Duration(0, 0, 0, 0, 1, 2))', function () { return df.format(new Temporal.Duration(0, 0, 0, 0, 1, 2)); });
var strings = ['PT1H30M', 'P1Y2M3W4DT5H6M7.008009010S', '-PT5M', '1 hour', ''];
strings.forEach(function (s) { t('U9 format(' + JSON.stringify(s) + ')', function () { return df.format(s); }); });
var big = typeof BigInt === 'function' ? BigInt(5) : 5;
var refusals = [
  ['5', 5], ['undefined', undefined], ['null', null], ['true', true], ['5n', big],
  ['{}', {}], ['{foo: 1}', { foo: 1 }], ['{hours: 1.5}', { hours: 1.5 }], ['{hours: Infinity}', { hours: Infinity }], ['{hours: NaN}', { hours: NaN }],
  ['{hours: 2n}', { hours: big }], ['{hours: 1, minutes: -1}', { hours: 1, minutes: -1 }],
  ['{years: 2**32}', { years: 4294967296 }], ['{years: 2**32 - 1}', { years: 4294967295 }], ['{weeks: -(2**32)}', { weeks: -4294967296 }],
  ['{seconds: 2**53}', { seconds: 9007199254740992 }], ['{seconds: 2**53 - 1}', { seconds: 9007199254740991 }],
  ['{days: 104249991374}', { days: 104249991374 }], ['{days: 104249991375}', { days: 104249991375 }], ['{nanoseconds: Number.MAX_VALUE}', { nanoseconds: Number.MAX_VALUE }]
];
refusals.forEach(function (p) { t('U10 format(' + p[0] + ')', function () { return df.format(p[1]); }); });
t('U10 format(a symbol)', function () { return df.format(Symbol('d')); });
// Rows U8 and U9: a Temporal.Duration and a duration string are read from slots, never through Temporal.Duration.prototype's getters.
var getterCalls = [];
U.forEach(function (u) {
  var d = Object.getOwnPropertyDescriptor(Temporal.Duration.prototype, u);
  Object.defineProperty(Temporal.Duration.prototype, u, { configurable: true, get: function () { getterCalls.push(u); return d.get.call(this); } });
});
t('U8 format(new Temporal.Duration(0, 0, 0, 0, 1, 2)), prototype getters wrapped', function () { getterCalls = []; var r = df.format(new Temporal.Duration(0, 0, 0, 0, 1, 2)); return r + ' | getters called: ' + getterCalls.join(','); });
t('U9 format("PT1H2M"), prototype getters wrapped', function () { getterCalls = []; var r = df.format('PT1H2M'); return r + ' | getters called: ' + getterCalls.join(','); });
t('U9 formatToParts("PT3S"), prototype getters wrapped', function () { getterCalls = []; var r = df.formatToParts('PT3S'); return r.length + ' parts | getters called: ' + getterCalls.join(','); });
U.forEach(function (u) {
  Object.defineProperty(Temporal.Duration.prototype, u, { configurable: true, get: function () { throw new EvalError('tainted ' + u); } });
});
t('U9 format("PT1H2M"), prototype getters throwing', function () { return df.format('PT1H2M'); });
t('U8 format(Temporal.Duration.from("PT1H2M")), prototype getters throwing', function () { return df.format(Temporal.Duration.from('PT1H2M')); });
