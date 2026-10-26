// Row U17: Temporal.Duration.prototype.toLocaleString, which tc39/proposal-temporal@e8cc03fc's intl.html defines as
// Construct(%Intl.DurationFormat%, « locales, options ») and PartitionDurationFormatPattern over the duration itself.
var d = Temporal.Duration.from({ hours: 1, minutes: 2, seconds: 3 });
t('U17 toLocaleString()', function () { return d.toLocaleString(); });
t('U17 toLocaleString("en", {style: "digital"})', function () { return d.toLocaleString('en', { style: 'digital' }); });
t('U17 toLocaleString("de", {style: "long"})', function () { return d.toLocaleString('de', { style: 'long' }); });
t('U17 equals new Intl.DurationFormat("de", {style: "long"}).format(d)', function () { return d.toLocaleString('de', { style: 'long' }) === new Intl.DurationFormat('de', { style: 'long' }).format(d); });
t('U17 toLocaleString("en", {style: "long", minutes: "numeric"})', function () { return d.toLocaleString('en', { style: 'long', minutes: 'numeric' }); });
t('U17 Temporal.Duration.from({hours: 2}).toLocaleString("en", {style: "long", minutes: "numeric"})', function () { return Temporal.Duration.from({ hours: 2 }).toLocaleString('en', { style: 'long', minutes: 'numeric' }); });
t('U17 negative toLocaleString("en", {style: "digital"})', function () { return Temporal.Duration.from('-PT1H0.5S').toLocaleString('en', { style: 'digital' }); });
t('U17 large toLocaleString("en", {style: "digital"})', function () { return Temporal.Duration.from({ seconds: 4503599627370497, nanoseconds: 1 }).toLocaleString('en', { style: 'digital' }); });
t('U17 toLocaleString("en", {style: "full"})', function () { return d.toLocaleString('en', { style: 'full' }); });
t('U17 toLocaleString("en", {milliseconds: "numeric", millisecondsDisplay: "always"})', function () { return d.toLocaleString('en', { milliseconds: 'numeric', millisecondsDisplay: 'always' }); });
t('U17 toLocaleString.call({hours: 1})', function () { return Temporal.Duration.prototype.toLocaleString.call({ hours: 1 }); });
t('U17 toLocaleString("en", null)', function () { return d.toLocaleString('en', null); });
t('U17 option reads of toLocaleString("en", spy)', function () { var log = []; d.toLocaleString('en', spy({ style: 'long', fractionalDigits: 2 }, log)); return log.join(','); });
['years', 'months', 'weeks', 'days', 'hours', 'minutes', 'seconds', 'milliseconds', 'microseconds', 'nanoseconds'].forEach(function (u) {
  Object.defineProperty(Temporal.Duration.prototype, u, { configurable: true, get: function () { throw new EvalError('tainted ' + u); } });
});
t('U17 toLocaleString("en"), prototype getters throwing', function () { return d.toLocaleString('en'); });
