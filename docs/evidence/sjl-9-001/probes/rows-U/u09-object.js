// Row U18: the constructor and prototype as objects: NewTarget, branding, the tag, the option reads, and the
// options argument (GetOptionsObject).
class Sub extends Intl.DurationFormat {}
t('U18 new Sub() prototype is Sub.prototype', function () { var s = new Sub('en'); return [Object.getPrototypeOf(s) === Sub.prototype, s instanceof Sub, s.format({ hours: 1 })]; });
t('U18 Reflect.construct(Intl.DurationFormat, [], Array) prototype', function () { var o = Reflect.construct(Intl.DurationFormat, ['en'], Array); return Object.getPrototypeOf(o) === Array.prototype; });
t('U18 Intl.DurationFormat()', function () { return Intl.DurationFormat(); });
t('U18 format.call({})', function () { return Intl.DurationFormat.prototype.format.call({}, { hours: 1 }); });
t('U18 resolvedOptions.call(Intl.DurationFormat.prototype)', function () { return Intl.DurationFormat.prototype.resolvedOptions.call(Intl.DurationFormat.prototype); });
t('U18 Object.prototype.toString', function () { return Object.prototype.toString.call(new Intl.DurationFormat()); });
t('U18 lengths', function () { return [Intl.DurationFormat.length, Intl.DurationFormat.prototype.format.length, Intl.DurationFormat.prototype.formatToParts.length, Intl.DurationFormat.prototype.resolvedOptions.length, Intl.DurationFormat.supportedLocalesOf.length]; });
t('U18 new Intl.DurationFormat("en", null)', function () { return new Intl.DurationFormat('en', null); });
t('U18 new Intl.DurationFormat("en", "long")', function () { return new Intl.DurationFormat('en', 'long'); });
t('U18 new Intl.DurationFormat("en", undefined).resolvedOptions().style', function () { return new Intl.DurationFormat('en', undefined).resolvedOptions().style; });
t('U18 style read through ToString', function () { return new Intl.DurationFormat('en', { style: { toString: function () { return 'long'; } } }).resolvedOptions().style; });
t('U18 option reads, every option given', function () {
  var log = [];
  new Intl.DurationFormat('en', spy({ localeMatcher: 'lookup', numberingSystem: 'latn', style: 'long', years: 'long', yearsDisplay: 'auto', months: 'long', monthsDisplay: 'auto', weeks: 'long', weeksDisplay: 'auto', days: 'long', daysDisplay: 'auto', hours: 'numeric', hoursDisplay: 'auto', minutes: 'numeric', minutesDisplay: 'auto', seconds: 'numeric', secondsDisplay: 'auto', milliseconds: 'numeric', millisecondsDisplay: 'auto', microseconds: 'numeric', microsecondsDisplay: 'auto', nanoseconds: 'numeric', nanosecondsDisplay: 'auto', fractionalDigits: 3 }, log));
  return log.join(',');
});
t('U18 option reads, an empty bag', function () { var log = []; new Intl.DurationFormat('en', spy({}, log)); return log.join(','); });
t('U18 format is the prototype method on every instance', function () { return new Intl.DurationFormat('en').format === new Intl.DurationFormat('de').format; });
