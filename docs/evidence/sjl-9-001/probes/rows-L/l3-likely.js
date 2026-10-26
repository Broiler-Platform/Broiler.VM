// SJL-9.1 sub-area (a), rows L8-L9: Intl.Locale.prototype.maximize and minimize (ECMA-402 15.3.6, 15.3.7:
// UTS #35's Add Likely Subtags and Remove Likely Subtags over CLDR's likely subtags). One line per tag.
function mx(s) { t('maximize ' + q(s), function () { return new Intl.Locale(s).maximize().toString(); }); }
function mn(s) { t('minimize ' + q(s), function () { return new Intl.Locale(s).minimize().toString(); }); }

['und', 'und-Hant', 'und-Hans', 'und-150', 'und-419', 'und-AQ', 'und-Latn-AQ', 'und-Zzzz', 'und-ZZ', 'und-Zzzz-ZZ',
 'und-Qaaa', 'und-Arab', 'und-PK', 'und-IN', 'und-Latn-IN', 'und-Cyrl-RO', 'und-Arab-IR', 'und-Thai', 'und-Brai',
 'en', 'en-GB', 'en-Shaw', 'en-150', 'zh', 'zh-TW', 'zh-Hant', 'zh-HK', 'zh-Hans-TW', 'zh-MO', 'sr', 'sr-ME',
 'sr-Latn', 'sr-RU', 'ar', 'pa', 'pa-PK', 'pa-Arab', 'hi-Latn', 'uz-AF', 'mn-Mong', 'aaa', 'xyz', 'xyz-US',
 'und-u-ca-gregory', 'en-US-x-foo', 'de-1996', 'de-u-co-phonebk', 'und-Latn-US-fonipa', 'ja', 'ko', 'ru', 'he',
 'fa', 'th', 'tr', 'el', 'und-GR', 'und-001', 'und-Zinh', 'und-Zyyy', 'zh-Hani', 'ja-Hira', 'ku', 'ckb',
 'und-SU', 'und-YU']
  .forEach(mx);
['en-Latn-US', 'en-Latn-GB', 'en-US', 'zh-Hant-TW', 'zh-Hans-CN', 'zh-Hant', 'zh-Hant-HK', 'sr-Cyrl-RS', 'sr-Latn-RS',
 'sr-Latn', 'de-Latn-DE-u-co-phonebk', 'und', 'und-Latn', 'und-US', 'und-Latn-US', 'en-GB', 'es-Latn-419', 'es-419',
 'aaa', 'xyz-Latn', 'ja-Jpan-JP', 'pa-Arab-PK', 'pa-Guru-IN', 'en-Latn-US-1996', 'en-Latn-US-x-foo', 'hi-Latn-IN',
 'uz-Arab-AF', 'und-Zzzz-ZZ', 'zh-Hani', 'ar-Arab-001', 'en-Zzzz-US', 'und-Hant-TW']
  .forEach(mn);
t('maximize keeps the extensions and returns a new Locale', function () {
  var l = new Intl.Locale('und', { calendar: 'japanese', region: 'TW' });
  var m = l.maximize();
  return [m.toString(), m !== l, m instanceof Intl.Locale, l.toString()];
});
t('maximize, minimize on a non-Locale', function () {
  var r = [];
  try { Intl.Locale.prototype.maximize.call({}); r.push('no error'); } catch (e) { r.push(errName(e)); }
  try { Intl.Locale.prototype.minimize.call('en'); r.push('no error'); } catch (e) { r.push(errName(e)); }
  return r;
});
