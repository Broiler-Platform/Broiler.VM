// SJL-9.1 sub-area (a), row L27: the default-locale provider (ECMA-402 6.2.4 DefaultLocale; SJL-D-19,
// JSD-0081 decision D7) as each service and each locale-sensitive method with no locales argument shows it.
// Run with no LANG, and again under LANG=LC_ALL=tr_TR.UTF-8 and de_DE.UTF-8: one DefaultLocale should serve
// every service and TransformCase alike.
t('Intl.NumberFormat().resolvedOptions().locale', function () { return new Intl.NumberFormat().resolvedOptions().locale; });
t('Intl.Collator().resolvedOptions().locale', function () { return new Intl.Collator().resolvedOptions().locale; });
t('Intl.DateTimeFormat().resolvedOptions().locale', function () { return new Intl.DateTimeFormat().resolvedOptions().locale; });
t('Intl.PluralRules().resolvedOptions().locale', function () { return new Intl.PluralRules().resolvedOptions().locale; });
t('Intl.ListFormat().resolvedOptions().locale', function () { return new Intl.ListFormat().resolvedOptions().locale; });
t('Intl.Segmenter().resolvedOptions().locale', function () { return new Intl.Segmenter().resolvedOptions().locale; });
t('Intl.NumberFormat("xx").resolvedOptions().locale', function () { return new Intl.NumberFormat('xx').resolvedOptions().locale; });
t('"I".toLocaleLowerCase()', function () { return 'I'.toLocaleLowerCase(); });
t('"i".toLocaleUpperCase()', function () { return 'i'.toLocaleUpperCase(); });
t('(1234.5).toLocaleString()', function () { return (1234.5).toLocaleString(); });
t('(1234.5).toLocaleString(undefined, {})', function () { return (1234.5).toLocaleString(undefined, {}); });
t('new Date(0).toLocaleDateString(undefined, {timeZone: "UTC"})', function () { return new Date(0).toLocaleDateString(undefined, { timeZone: 'UTC' }); });
t('["a", 1000].toLocaleString()', function () { return ['a', 1000].toLocaleString(); });
t('Intl.Collator.supportedLocalesOf(["tr", "de-DE", "en-US"])', function () { return Intl.Collator.supportedLocalesOf(['tr', 'de-DE', 'en-US']); });
t('new Intl.Locale("und").maximize().toString()', function () { return new Intl.Locale('und').maximize().toString(); });
