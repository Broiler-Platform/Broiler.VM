// What each engine takes from the host environment: TZ and LANG/LC_ALL (the .NET current culture).
t('default locale (NumberFormat)', function () { return new Intl.NumberFormat().resolvedOptions().locale; });
t('default locale (DateTimeFormat)', function () { return new Intl.DateTimeFormat().resolvedOptions().locale; });
t('default time zone (DateTimeFormat)', function () { return new Intl.DateTimeFormat().resolvedOptions().timeZone; });
t('Date offset at epoch (minutes)', function () { return new Date(0).getTimezoneOffset(); });
t('toLocaleString of 1234.5 with no locale', function () { return (1234.5).toLocaleString(); });
