// SJL-9.1 sub-area (a), rows L10-L13: the Intl.Locale constructor (ECMA-402 15.1.1, UpdateLanguageId 15.1.2,
// MakeLocaleRecord 15.1.3), its getters and its options' reads and checks.
function info(l) {
  return R('{toString: ' + q(l.toString()) + ', baseName: ' + q(l.baseName) + ', language: ' + q(l.language) +
    ', script: ' + show(l.script) + ', region: ' + show(l.region) + ', variants: ' + show(l.variants) +
    ', calendar: ' + show(l.calendar) + ', caseFirst: ' + show(l.caseFirst) + ', collation: ' + show(l.collation) +
    ', firstDayOfWeek: ' + show(l.firstDayOfWeek) + ', hourCycle: ' + show(l.hourCycle) +
    ', numberingSystem: ' + show(l.numberingSystem) + ', numeric: ' + show(l.numeric) + '}');
}
function L(label, tag, options) { t(label + ' ' + q(tag) + (options === undefined ? '' : ' ' + show(options)), function () { return info(new Intl.Locale(tag, options)); }); }

L('getters', 'en-latn-us-u-ca-gregory-co-phonebk-hc-h12-kf-upper-kn-nu-latn-fw-mon');
L('getters', 'en-u-kn-false');
L('getters', 'en-u-kn-true');
L('getters', 'en-u-kf');
L('getters', 'de-DE-1996-fonipa');
L('getters', 'en-x-foo');
L('getters', 'und');
L('getters', 'en-u-ca-islamicc-co-dict');
L('options', 'en-US-u-ca-gregory', { language: 'fr', script: 'cyrl', region: 'ca', variants: 'FONIPA-1996', calendar: 'Buddhist', collation: 'EMOJI', hourCycle: 'h23', caseFirst: 'lower', numeric: false, numberingSystem: 'Arab', firstDayOfWeek: '3' });
L('options', 'en', { calendar: 'islamicc' });
L('options', 'en-u-ca-gregory', { calendar: 'japanese' });
L('options', 'en-u-kn-true', { numeric: false });
L('options', 'en', { numeric: 'yes' });
L('options', 'en', { numeric: 0 });
L('options', 'en', { firstDayOfWeek: '0' });
L('options', 'en', { firstDayOfWeek: '7' });
L('options', 'en', { firstDayOfWeek: 'MON' });
L('options', 'en', { firstDayOfWeek: 'monday' });
L('options', 'en', { region: '419' });
L('options', 'cel', { variants: 'gaulish' });
L('options', 'en', { language: 'iw' });
L('options', 'en-US', { region: 'SU' });
L('options', 'en', { variants: 'hepburn-heploc' });
L('options', 'en-u-ca-gregory', { calendar: undefined });
// Option values that must be refused.
L('refused', 'en', { language: '' });
L('refused', 'en', { language: 'a' });
L('refused', 'en', { language: 'abcd' });
L('refused', 'en', { language: 'en-US' });
L('refused', 'en', { script: 'Lat' });
L('refused', 'en', { region: 'US1' });
L('refused', 'en', { variants: '' });
L('refused', 'en', { variants: 'aaaa' });
L('refused', 'en', { variants: 'fonipa-fonipa' });
L('refused', 'en', { variants: 'fonipa-' });
L('refused', 'en', { calendar: 'ab' });
L('refused', 'en', { calendar: 'gregory-' });
L('refused', 'en', { collation: 'phonebook' });
L('refused', 'en', { hourCycle: 'h25' });
L('refused', 'en', { caseFirst: 'UPPER' });
L('refused', 'en', { numberingSystem: 'latn_' });
L('refused', 'en', { firstDayOfWeek: '8' });
L('refused', 'en', { firstDayOfWeek: '' });
// Non-ASCII option values (U+212A KELVIN SIGN lowers to "k" under the platform's casing; U+0130 to "i").
L('non-ASCII', 'en', { variants: 'Kaaaa' });
L('non-ASCII', 'en', { numberingSystem: 'Khmr' });
L('non-ASCII', 'en', { calendar: 'bİddhist' });
L('non-ASCII', 'en', { collation: 'phoKebk' });
L('non-ASCII', 'en', { language: 'Ko' });
L('non-ASCII', 'en', { region: 'KR' });
L('non-ASCII', 'en', { firstDayOfWeek: 'moK' });
// The constructor's other steps.
t('called without new', function () { return Intl.Locale('en'); });
t('tag 5', function () { return new Intl.Locale(5); });
t('tag undefined', function () { return new Intl.Locale(); });
t('tag null', function () { return new Intl.Locale(null); });
t('tag a Locale, with options', function () { return info(new Intl.Locale(new Intl.Locale('en-u-ca-gregory'), { region: 'GB' })); });
t('tag an object with toString', function () { return info(new Intl.Locale({ toString: function () { return 'DE-at'; } })); });
t('options null', function () { return new Intl.Locale('en', null); });
t('options 5', function () { return info(new Intl.Locale('en', 5)); });
t('option reads, in order', function () {
  var log = [];
  new Intl.Locale('en', spy({ language: 'fr', script: 'Latn', region: 'FR', variants: '1996', calendar: 'gregory', collation: 'emoji', hourCycle: 'h23', caseFirst: 'upper', numeric: true, numberingSystem: 'latn', firstDayOfWeek: 'mon' }, log));
  return log.join(',');
});
t('option reads stop at the first bad value', function () {
  var log = [];
  try { new Intl.Locale('en', spy({ language: 'fr', script: 'L', region: 'FR' }, log)); } catch (e) { log.push(errName(e)); }
  return log.join(',');
});
t('tag read before options are coerced', function () {
  var log = [];
  try { new Intl.Locale({ toString: function () { log.push('tag'); return 'en'; } }, { get language() { log.push('language'); return undefined; } }); } catch (e) { log.push(errName(e)); }
  return log.join(',');
});
t('getters on Intl.Locale.prototype', function () {
  var d = Object.getOwnPropertyDescriptor(Intl.Locale.prototype, 'baseName');
  return [typeof d.get, d.set, d.enumerable, d.configurable, d.get.name, Intl.Locale.length, Intl.Locale.name];
});
t('Object.prototype.toString of a Locale', function () { return Object.prototype.toString.call(new Intl.Locale('en')); });
