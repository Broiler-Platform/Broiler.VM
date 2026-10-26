// SJL-9.1 sub-area (a), rows L14-L20: the Locale information methods (ECMA-402 15.3.10-15.3.16 and their
// abstract operations 15.5.9-15.5.17, the merged Intl Locale Info proposal): one line per method and locale.
var tags = ['en', 'en-US', 'en-GB', 'en-IN', 'de', 'de-DE', 'fr-FR', 'es-419', 'pt-BR', 'ja', 'ja-JP', 'ko-KR', 'zh-TW',
  'zh-Hans-CN', 'ar', 'ar-EG', 'ar-SA', 'he-IL', 'fa-IR', 'th-TH', 'hi-IN', 'ckb-IQ', 'und', 'und-Arab', 'und-Hebr',
  'xyz', 'en-001', 'en-150', 'en-AE', 'en-MV', 'ms-BN',
  'en-u-ca-japanese-co-emoji-hc-h23-nu-arab-fw-wed', 'en-US-u-rg-gbzzzz', 'en-u-sd-usca', 'en-u-rg-sazzzz',
  'en-u-sd-gbsct', 'de-u-co-phonebk', 'en-u-fw-sun', 'en-u-fw-xyz', 'und-u-hc-h11'];
var methods = ['getCalendars', 'getCollations', 'getHourCycles', 'getNumberingSystems', 'getTextInfo', 'getWeekInfo'];
methods.forEach(function (m) {
  tags.forEach(function (tag) {
    t(m + ' ' + q(tag), function () { return new Intl.Locale(tag)[m](); });
  });
});
// getTimeZones: the region's zones; shown as their count and their first and last few.
['en', 'en-US', 'de-DE', 'fr-FR', 'ar-EG', 'zh-TW', 'en-001', 'en-AQ', 'pt-BR', 'en-GB', 'und-150', 'ru-RU', 'en-u-rg-gbzzzz', 'xx-ZZ']
  .forEach(function (tag) {
    t('getTimeZones ' + q(tag), function () {
      var z = new Intl.Locale(tag).getTimeZones();
      if (z === undefined) return z;
      return R(z.length + ' zones: ' + show(z.slice(0, 4)) + (z.length > 4 ? ' ... ' + show(z.slice(-2)) : ''));
    });
  });
// The returned objects: own keys in order, fresh each call, not frozen.
t('getWeekInfo keys and descriptors', function () {
  var w = new Intl.Locale('en-US').getWeekInfo();
  var d = Object.getOwnPropertyDescriptor(w, 'firstDay');
  return [Object.keys(w), d.writable, d.enumerable, d.configurable, Object.getPrototypeOf(w) === Object.prototype];
});
t('getTextInfo keys', function () { return Object.keys(new Intl.Locale('ar').getTextInfo()); });
t('getCalendars returns a fresh array', function () { var l = new Intl.Locale('en'); return [l.getCalendars() !== l.getCalendars(), Array.isArray(l.getCalendars())]; });
t('methods are not getters (Locale Info as merged)', function () {
  return ['calendars', 'collations', 'hourCycles', 'numberingSystems', 'timeZones', 'textInfo', 'weekInfo'].map(function (k) {
    return k + ':' + (Object.getOwnPropertyDescriptor(Intl.Locale.prototype, k) ? 'present' : 'absent');
  });
});
t('method lengths', function () { return methods.concat(['getTimeZones']).map(function (m) { return m + '=' + Intl.Locale.prototype[m].length; }); });
t('a method on a non-Locale', function () { try { Intl.Locale.prototype.getWeekInfo.call({}); return 'no error'; } catch (e) { return R(errName(e)); } });
