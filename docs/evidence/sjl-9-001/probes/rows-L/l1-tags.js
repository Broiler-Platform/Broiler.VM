// SJL-9.1 sub-area (a), rows L1-L3: IsStructurallyValidLanguageTag and CanonicalizeUnicodeLocaleId as
// Intl.getCanonicalLocales shows them (ECMA-402 6.2.1, 6.2.3, 9.2.1 CanonicalizeLocaleList), and the
// list's walk. One line per input: the canonical list, or the error.
function g(label, input) { t(label + ' ' + (typeof input === 'string' ? q(input) : show(input)), function () { return Intl.getCanonicalLocales(input); }); }

// Case and order.
g('case', 'EN-us');
g('case', 'en-latn-us');
g('case', 'zh-hant-tw');
g('case', 'SR-CYRL-rs');
g('case', 'EN-u-CA-Gregory');
g('case', 'DE-1996-1901');
g('case', 'en-Latn-US-POSIX');
// Grammar: what is not structurally valid.
g('invalid', '');
g('invalid', 'en-');
g('invalid', '-en');
g('invalid', 'en--US');
g('invalid', 'e');
g('invalid', 'english');
g('invalid', 'abcd');
g('invalid', 'en_US');
g('invalid', 'root');
g('invalid', 'Latn-en');
g('invalid', 'zh-cmn-Hans');
g('invalid', 'en-els');
g('invalid', 'x-private');
g('invalid', 'i-klingon');
g('invalid', 'zh-min-nan');
g('invalid', 'en-gb-oed');
g('invalid', 'de-1996-1996');
g('invalid', 'de-DE-1996-1996');
g('invalid', 'en-u-ca-gregory-u-nu-latn');
g('invalid', 'en-a-bbb-a-ccc');
g('invalid', 'en-u');
g('invalid', 'en-t');
g('invalid', 'en-u-1a-gregory');
g('invalid', 'en-t-root');
g('invalid', 'en-t-d0');
g('invalid', 'de-t-en-emodeng-emodeng');
g('invalid', 'en-x');
g('invalid', 'en-x-abcdefghi');
g('invalid', 'en-US-u-ca-abcdefghi');
// Non-ASCII letters that ASCII-insensitive or culture casing could fold into a tag's letters:
// U+212A KELVIN SIGN (lowers to "k"), U+017F LONG S (uppers to "S"), U+0130 and U+0131 (Turkish i).
g('non-ASCII', 'Ko');
g('non-ASCII', 'en-KE');
g('non-ASCII', 'ſr');
g('non-ASCII', 'en-u-ca-gregorK');
g('non-ASCII', 'ıt');
g('non-ASCII', 'İT');
g('non-ASCII', 'en-US\u0000');
g('non-ASCII', 'ｅｎ');
// Extensions: order, duplicates, attributes, true, other singletons, private use.
g('ext', 'en-u-nu-latn-ca-gregory');
g('ext', 'en-u-bbb-aaa-ca-gregory');
g('ext', 'en-u-aaa-aaa-ca-gregory');
g('ext', 'en-u-ca-gregory-ca-buddhist');
g('ext', 'en-u-kn-true');
g('ext', 'en-u-ca-true');
g('ext', 'en-u-kn-false');
g('ext', 'en-u-kn');
g('ext', 'en-u-ca');
g('ext', 'en-u-ca-islamic-civil');
g('ext', 'en-u-ca-gregory-a-bbb');
g('ext', 'en-z-zzz-u-ca-gregory-a-aaa-x-private');
g('ext', 'en-x-u-ca-gregory');
g('ext', 'en-x-a-b-c');
g('ext', 'en-u-attr-x-private');
g('ext', 'en-t-en-latn-us-h0-hybrid-m0-ungegn');
g('ext', 'en-t-m0-ungegn-h0-hybrid');
g('ext', 'EN-T-EN-LATN');
g('ext', 'de-t-en-emodeng-1996');
// The list: duplicates after canonicalization, a Locale, element types, non-list values.
g('list', ['en-us', 'EN-US', 'en-US']);
g('list', ['de', 'en', 'de']);
t('list [Locale(en-US), "EN"]', function () { return Intl.getCanonicalLocales([new Intl.Locale('en-US'), 'EN']); });
t('list Locale(en-u-kn-true) alone', function () { return Intl.getCanonicalLocales(new Intl.Locale('en-u-kn-true')); });
g('list', [1]);
g('list', ['en', null]);
g('list', ['en', undefined]);
g('list', [true]);
t('list [Symbol]', function () { return Intl.getCanonicalLocales([Symbol('x')]); });
t('list String object', function () { return Intl.getCanonicalLocales(new String('EN-us')); });
t('list undefined', function () { return Intl.getCanonicalLocales(undefined); });
t('list null', function () { return Intl.getCanonicalLocales(null); });
t('list 5', function () { return Intl.getCanonicalLocales(5); });
t('list true', function () { return Intl.getCanonicalLocales(true); });
t('list {length: 2, 0: "en", 1: "de"}', function () { return Intl.getCanonicalLocales({ length: 2, 0: 'en', 1: 'de' }); });
t('list {length: -1, 0: "en"}', function () { return Intl.getCanonicalLocales({ length: -1, 0: 'en' }); });
t('list holes [, "en"]', function () { return Intl.getCanonicalLocales([, 'en']); });
t('list read order', function () {
  var log = [];
  var o = new Proxy({ length: 2, 0: 'en', 1: 'de' }, {
    get: function (target, k) { log.push('get ' + String(k)); return target[k]; },
    has: function (target, k) { log.push('has ' + String(k)); return k in target; }
  });
  Intl.getCanonicalLocales(o);
  return log.join(', ');
});
t('list element toString object', function () { return Intl.getCanonicalLocales([{ toString: function () { return 'DE-at'; } }]); });
t('getCanonicalLocales.length, name', function () { return [Intl.getCanonicalLocales.length, Intl.getCanonicalLocales.name]; });
t('result is a fresh array', function () { var a = Intl.getCanonicalLocales('en'); var b = Intl.getCanonicalLocales('en'); return [a !== b, Array.isArray(a), Object.isFrozen(a)]; });
