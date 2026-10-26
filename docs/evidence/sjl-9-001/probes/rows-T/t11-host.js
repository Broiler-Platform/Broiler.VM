// SJL-9.1 sub-area (d), rows T6 and T32: what a date format takes from the host. The default time
// zone (SystemTimeZoneIdentifier, decision SJL-D-19's provider), the default locale (DefaultLocale,
// row L33), and the formats and names a default-locale format writes; run under TZ=UTC and
// TZ=America/New_York, with no LANG and with LANG set.
var S = Date.UTC(2024, 6, 15, 12, 0), W = Date.UTC(2024, 0, 15, 12, 0);
t('resolvedOptions default [locale, timeZone]', function () { var r = new Intl.DateTimeFormat().resolvedOptions(); return [r.locale, r.timeZone]; });
t('format(summer), format(winter) default', function () { var f = new Intl.DateTimeFormat(undefined, { hour: 'numeric', minute: 'numeric', timeZoneName: 'short' }); return [f.format(S), f.format(W)]; });
t('Date#toLocaleString() no arguments', function () { return new Date(S).toLocaleString(); });
t('Date#toLocaleDateString() no arguments', function () { return new Date(S).toLocaleDateString(); });
t('Date#toLocaleTimeString() no arguments', function () { return new Date(S).toLocaleTimeString(); });
t('en dateStyle full timeStyle full, default zone', function () { return new Intl.DateTimeFormat('en', { dateStyle: 'full', timeStyle: 'full' }).format(S); });
t('en {timeZoneName:longGeneric}, default zone', function () { return new Intl.DateTimeFormat('en', { timeZoneName: 'longGeneric' }).format(S); });
t('Temporal.Now.timeZoneId()', function () { return typeof Temporal === 'object' ? Temporal.Now.timeZoneId() : 'no Temporal'; });
t('Temporal.Instant toLocaleString default zone', function () { return Temporal.Instant.fromEpochMilliseconds(S).toLocaleString('en'); });
