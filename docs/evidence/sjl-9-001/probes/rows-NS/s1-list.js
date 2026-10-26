// Rows S1 (list patterns by locale, type and style), S2 (the template chosen by the elements) and S3
// (StringListFromIterable, the parts, the constructor's reads).
var locs = ['en', 'en-GB', 'de', 'es', 'fr', 'it', 'ja', 'zh', 'pl', 'ru', 'he', 'ar', 'xyz'];
locs.forEach(function (l) {
  ['conjunction', 'disjunction', 'unit'].forEach(function (ty) {
    ['long', 'short', 'narrow'].forEach(function (st) {
      t(l + ' ' + ty + ' ' + st, function () {
        var f = new Intl.ListFormat(l, { type: ty, style: st });
        return [f.resolvedOptions().locale, f.format(['A', 'B']), f.format(['A', 'B', 'C', 'D'])];
      });
    });
  });
});
t('es conjunction: y before i, hi, hie, y', function () { var f = new Intl.ListFormat('es'); return [f.format(['a', 'iglesia']), f.format(['a', 'hijo']), f.format(['a', 'hielo']), f.format(['a', 'b', 'Isabel'])]; });
t('es disjunction: o before o, ho, 8, 11', function () { var f = new Intl.ListFormat('es', { type: 'disjunction' }); return [f.format(['a', 'otro']), f.format(['a', 'hora']), f.format(['7', '8']), f.format(['10', '11'])]; });
t('he conjunction: Hebrew and non-Hebrew last element', function () { var f = new Intl.ListFormat('he'); return [f.format(['א', 'ב']), f.format(['א', 'B']), f.format(['א', '1'])]; });
t('en formatToParts(["a", "", "c"])', function () { return parts(new Intl.ListFormat('en').formatToParts(['a', '', 'c'])); });
t('de formatToParts(["a", "b", "c", "d"])', function () { return parts(new Intl.ListFormat('de').formatToParts(['a', 'b', 'c', 'd'])); });
t('format of a string, a Set, undefined, []', function () { var f = new Intl.ListFormat('en'); return [f.format('abc'), f.format(new Set(['x', 'y'])), f.format(undefined), f.format([])]; });
t('format(null)', function () { return new Intl.ListFormat('en').format(null); });
t('format([1, 2])', function () { return new Intl.ListFormat('en').format([1, 2]); });
t('a non-String element closes the iterator', function () {
  var closed = 0;
  var it = { i: 0, next: function () { this.i++; return this.i === 1 ? { value: 'a', done: false } : { value: 1, done: false }; }, return: function () { closed++; return {}; } };
  var iterable = {}; iterable[Symbol.iterator] = function () { return it; };
  try { new Intl.ListFormat('en').format(iterable); return 'no error'; } catch (e) { return [e.name, closed]; }
});
t('elements with "{0}" and "{1}"', function () { return new Intl.ListFormat('en').format(['{1}', '{0}', '{0}{1}']); });
var log = [];
var lf = new Intl.ListFormat('en', spy({ localeMatcher: 'lookup', type: 'unit', style: 'narrow' }, log));
t('constructor reads', function () { return log; });
t('resolvedOptions keys', function () { return Object.keys(lf.resolvedOptions()); });
t('options a string', function () { return new Intl.ListFormat('en', 'conjunction').resolvedOptions().type; });
t('options null', function () { return new Intl.ListFormat('en', null).resolvedOptions().type; });
t('ListFormat without new', function () { return Intl.ListFormat('en'); });
t('invalid type, style', function () { return ['type', 'style'].map(function (k) { var o = {}; o[k] = 'bogus'; try { new Intl.ListFormat('en', o); return k + ':ok'; } catch (e) { return k + ':' + e.name; } }).join(' '); });
t('format and formatToParts identity and length', function () { var f = new Intl.ListFormat('en'); return [f.format === f.format, Intl.ListFormat.prototype.format.length, Intl.ListFormat.prototype.formatToParts.length]; });
