// Rows S8 (the names [[Fields]] holds: naming by type, style, language display and fallback), S9 (the
// canonical code: validation and case regularization) and S10 (the constructor, resolvedOptions, supportedLocalesOf).
function dn(l, o) { return new Intl.DisplayNames(l, o); }
function both(l, o, code) { var a = dn(l, o).of(code); var b = dn(l, Object.assign({ fallback: 'none' }, o)).of(code); return [a, b]; }
['en', 'de', 'fr', 'es', 'ja', 'zh-Hant'].forEach(function (l) {
  t(l + ' language dialect: en-GB, de-AT, zh-Hant-TW, xyz, en-XY', function () { return ['en-GB', 'de-AT', 'zh-Hant-TW', 'xyz', 'en-XY'].map(function (c) { return dn(l, { type: 'language' }).of(c); }); });
  t(l + ' region: US, DE, 419, ZZ, AA', function () { return ['US', 'DE', '419', 'ZZ', 'AA'].map(function (c) { return dn(l, { type: 'region' }).of(c); }); });
  t(l + ' script, currency, calendar, dateTimeField', function () { return [dn(l, { type: 'script' }).of('Latn'), dn(l, { type: 'currency' }).of('EUR'), dn(l, { type: 'calendar' }).of('gregory'), dn(l, { type: 'dateTimeField' }).of('weekOfYear')]; });
});
t('en language standard: en-GB, zh-Hant-TW, en-Latn-US', function () { var f = dn('en', { type: 'language', languageDisplay: 'standard' }); return [f.of('en-GB'), f.of('zh-Hant-TW'), f.of('en-Latn-US')]; });
t('en language short and narrow: en-GB, en-US', function () { return [dn('en', { type: 'language', style: 'short' }).of('en-GB'), dn('en', { type: 'language', style: 'narrow' }).of('en-US')]; });
t('en region short: US, GB, HK', function () { var f = dn('en', { type: 'region', style: 'short' }); return [f.of('US'), f.of('GB'), f.of('HK')]; });
t('en fallback code/none: language xyz, region AA, script Qaaa, currency XYZ, calendar xyz', function () { return [both('en', { type: 'language' }, 'xyz'), both('en', { type: 'region' }, 'AA'), both('en', { type: 'script' }, 'Qaaa'), both('en', { type: 'currency' }, 'XYZ'), both('en', { type: 'calendar' }, 'xyz')]; });
t('en fallback none: known codes US, EUR, gregory, en', function () { var o = { fallback: 'none' }; return [dn('en', Object.assign({ type: 'region' }, o)).of('US'), dn('en', Object.assign({ type: 'currency' }, o)).of('EUR'), dn('en', Object.assign({ type: 'calendar' }, o)).of('gregory'), dn('en', Object.assign({ type: 'language' }, o)).of('en')]; });
t('en dateTimeField short and narrow: year, dayPeriod', function () { return [dn('en', { type: 'dateTimeField', style: 'short' }).of('year'), dn('en', { type: 'dateTimeField', style: 'narrow' }).of('dayPeriod')]; });
// S9: the canonical code.
t('region case: us, Us, 419', function () { var f = dn('en', { type: 'region', fallback: 'code' }); var g = dn('xyz', { type: 'region' }); return [f.of('us'), f.of('Us'), g.of('us')]; });
t('script case: latn, HANS', function () { var g = dn('xyz', { type: 'script' }); return [dn('en', { type: 'script' }).of('latn'), g.of('latn'), g.of('HANS')]; });
t('calendar case: GREGORY, Islamic-Civil', function () { var g = dn('xyz', { type: 'calendar' }); return [dn('en', { type: 'calendar' }).of('GREGORY'), g.of('GREGORY'), g.of('Islamic-Civil')]; });
t('currency case: eur', function () { return [dn('en', { type: 'currency' }).of('eur'), dn('xyz', { type: 'currency' }).of('eur')]; });
t('language canonical: EN-us, iw, in, sh, und, en-latn-us, zh-hant-tw', function () { var g = dn('xyz', { type: 'language', fallback: 'code' }); return ['EN-us', 'iw', 'in', 'sh', 'und', 'en-latn-us', 'zh-hant-tw'].map(function (c) { try { return g.of(c); } catch (e) { return e.name; } }); });
t('calendar aliases: islamicc, ethiopic-amete-alem, gregorian', function () { var g = dn('en', { type: 'calendar' }); return ['islamicc', 'ethiopic-amete-alem', 'gregorian'].map(function (c) { try { return g.of(c); } catch (e) { return e.name; } }); });
var bad = {
  region: ['١٢٣', 'US\n', 'Uİ', '1٣', 'ＵS'],
  script: ['Latn\n', 'Lаtn', 'Ｌatn'],
  calendar: ['gregory\n', 'grеgory', 'abc_def'],
  currency: ['USD\n', 'ıSD'],
  language: ['en\n', 'en-US\n', 'de-١٢٣', 'en_US', 'root', 'en-u-ca-gregory', 'en-x-priv', 'i-klingon'],
  dateTimeField: ['year\n', 'Year', 'era '],
};
Object.keys(bad).forEach(function (ty) {
  t(ty + ' invalid codes', function () {
    var f = dn('en', { type: ty });
    return bad[ty].map(function (c) { try { return q(c) + ':' + q(String(f.of(c))); } catch (e) { return q(c) + ':' + e.name; } }).join(' ');
  });
});
t('of(Symbol()), of({toString}), of(undefined)', function () {
  var f = dn('en', { type: 'region' });
  return [function () { return f.of(Symbol()); }, function () { return f.of({ toString: function () { return 'DE'; } }); }, function () { return f.of(undefined); }].map(function (g) { try { return String(g()); } catch (e) { return e.name; } });
});
// S10: the constructor.
var log = [];
var d = dn('en', spy({ localeMatcher: 'lookup', style: 'short', type: 'language', fallback: 'none', languageDisplay: 'standard' }, log));
t('constructor reads', function () { return log; });
t('resolvedOptions: language type; region type', function () { return [Object.keys(d.resolvedOptions()).join(','), Object.keys(dn('en', { type: 'region' }).resolvedOptions()).join(',')]; });
t('no options, {}, options a string, null', function () { return [undefined, {}, 'region', null].map(function (o) { try { new Intl.DisplayNames('en', o); return 'ok'; } catch (e) { return e.name; } }); });
t('DisplayNames without new; length', function () { var r; try { Intl.DisplayNames('en', { type: 'region' }); r = 'ok'; } catch (e) { r = e.name; } return [r, Intl.DisplayNames.length]; });
t('typeof Intl.DisplayNames.supportedLocalesOf; result', function () { var f = Intl.DisplayNames.supportedLocalesOf; return [typeof f, typeof f === 'function' ? f(['de-AT', 'en', 'zz']) : null]; });
t('invalid style, type, fallback, languageDisplay', function () { return ['style', 'type', 'fallback', 'languageDisplay'].map(function (k) { var o = { type: 'region' }; o[k] = 'bogus'; try { new Intl.DisplayNames('en', o); return k + ':ok'; } catch (e) { return k + ':' + e.name; } }).join(' '); });
t('resolved locale: de-CH, fr, xyz', function () { return ['de-CH', 'fr', 'xyz'].map(function (l) { return dn(l, { type: 'region' }).resolvedOptions().locale; }); });
