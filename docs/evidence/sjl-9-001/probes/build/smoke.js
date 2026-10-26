// SJL-9.1 BUILD smoke probe: one line per sub-area (a-e), the host defaults, and one line per proposal
// the admitted copy admits. It shows each binary runs Intl; it classifies nothing.
t('default locale (NumberFormat)', function () { return new Intl.NumberFormat().resolvedOptions().locale; });
t('default time zone (DateTimeFormat)', function () { return new Intl.DateTimeFormat().resolvedOptions().timeZone; });
t('a getCanonicalLocales', function () { return Intl.getCanonicalLocales(['EN-us', 'zh-hant-tw']); });
t('a Locale maximize', function () { return new Intl.Locale('und-Hant').maximize().toString(); });
t('b PluralRules en select(1), select(2)', function () { var p = new Intl.PluralRules('en'); return [p.select(1), p.select(2)]; });
t('c NumberFormat en 1234.5', function () { return new Intl.NumberFormat('en').format(1234.5); });
t('c NumberFormat de 1234.5', function () { return new Intl.NumberFormat('de').format(1234.5); });
t('d DateTimeFormat en UTC dateStyle medium, epoch', function () { return new Intl.DateTimeFormat('en', { timeZone: 'UTC', dateStyle: 'medium' }).format(0); });
t('e DurationFormat en long', function () { return new Intl.DurationFormat('en', { style: 'long' }).format({ hours: 1, minutes: 2 }); });
t('canonical-tz: resolved zone for Asia/Calcutta', function () { return new Intl.DateTimeFormat('en', { timeZone: 'Asia/Calcutta' }).resolvedOptions().timeZone; });
t('Intl.Locale-info: typeof getWeekInfo', function () { return typeof Intl.Locale.prototype.getWeekInfo; });
t('Intl.NumberFormat-v3: formatRange en 3-5', function () { return new Intl.NumberFormat('en').formatRange(3, 5); });
