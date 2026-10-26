// SJL-9.1 sub-area (a), rows L4-L7: CanonicalizeUnicodeLocaleId's alias replacement (UTS #35 Annex C,
// CLDR's supplemental aliases and the BCP 47 data's deprecated types), as Intl.getCanonicalLocales and
// Intl.Locale show it. One line per input.
function g(label, input) { t(label + ' ' + q(input), function () { return Intl.getCanonicalLocales(input); }); }

// Language aliases: simple, with a script or region added, language and region rules, variants.
['iw', 'in', 'ji', 'jw', 'mo', 'tl', 'aar', 'ces', 'heb', 'tw', 'cmn', 'cmn-TW', 'drw', 'aju', 'aam', 'swc',
 'no', 'no-NO', 'nb', 'sh', 'sh-Cyrl', 'sh-BA', 'cnr', 'cnr-BA', 'zh-hakka', 'zh-xiang', 'zh-guoyu', 'art-lojban',
 'cel-gaulish', 'art-lojban-fonipa', 'sgn-GR', 'sgn-BE-FR', 'sgn-BE-NL', 'sgn-US', 'sgn-CH-DE', 'und-aaland',
 'sv-aaland', 'hy-arevela', 'hy-arevmda', 'el-polytoni', 'ja-Latn-hepburn-heploc', 'ja-Latn-heploc',
 'en-heploc', 'sl-rozaj-biske-1994', 'en-scouse', 'zh-cmn', 'zh-yue', 'yue-HK', 'tlh', 'sgn', 'und-hepburn-heploc',
 'ru-tarask', 'be-tarask', 'en-arevela', 'aa-saaho', 'und-Latn-saaho']
  .forEach(function (s) { g('language', s); });
// Script aliases.
['und-Qaai', 'en-Qaai', 'und-Qaai-US'].forEach(function (s) { g('script', s); });
// Region aliases: single, numeric, and lists resolved by likely subtags.
['en-BU', 'de-DD', 'fr-FX', 'en-UK', 'pt-TP', 'sr-YU', 'fr-ZR', 'en-QU', 'en-554', 'en-840', 'en-276', 'de-276',
 'es-419', 'en-062', 'en-SU', 'hy-SU', 'ru-SU', 'uz-SU', 'en-810', 'sr-CS', 'en-CS', 'en-NT', 'en-AN', 'nl-AN',
 'en-YD', 'und-172', 'und-Cyrl-172', 'und-200', 'cs-200']
  .forEach(function (s) { g('region', s); });
// -u- types: the BCP 47 data's deprecated types and their preferred forms, subdivisions, booleans.
['en-u-ca-islamicc', 'en-u-ca-ethiopic-amete-alem', 'en-u-ks-primary', 'en-u-ks-identic', 'en-u-ms-imperial',
 'en-u-tz-cnckg', 'en-u-tz-usnavajo', 'en-u-tz-eire', 'en-u-tz-utc', 'en-u-tz-gmt0', 'en-u-tz-aqams', 'en-u-tz-cnhrb',
 'en-u-tz-fiplt', 'en-u-tz-usnyc', 'en-u-sd-no23', 'en-u-rg-cn11', 'en-u-sd-frg', 'en-u-rg-cz10a', 'en-u-sd-gbzzzz',
 'en-u-rg-uszzzz', 'en-u-sd-fi01', 'en-u-sd-lud', 'en-u-kb-yes', 'en-u-kc-yes', 'en-u-kh-yes', 'en-u-kk-yes',
 'en-u-ka-yes', 'en-u-kr-yes', 'en-u-kn-yes', 'en-u-co-dict', 'en-u-co-trad', 'en-u-dx-latn', 'en-u-nu-traditio',
 'en-u-hc-h24', 'en-u-cf-account', 'en-u-em-emoji', 'en-u-mu-celsius', 'en-u-va-posix', 'en-u-ca-gregory-co-standard']
  .forEach(function (s) { g('u', s); });
// -t- language and fields.
['en-t-iw', 'en-t-sh', 'en-t-aaland', 'en-t-zh-cmn', 'und-t-m0-names', 'und-t-d0-fwidth', 'und-t-d0-hwidth',
 'und-t-d0-name', 'und-t-m0-bgn', 'und-t-k0-azerty', 'en-t-hi-h0-hybrid', 'en-t-en-sgn-us']
  .forEach(function (s) { g('t', s); });
// The same through Intl.Locale (its [[Locale]] is CanonicalizeUnicodeLocaleId's too).
['iw', 'sh', 'en-840', 'und-Qaai', 'en-u-tz-usnavajo', 'ja-Latn-hepburn-heploc', 'sgn-BE-FR', 'en-SU']
  .forEach(function (s) { t('Locale ' + q(s), function () { return new Intl.Locale(s).toString(); }); });
