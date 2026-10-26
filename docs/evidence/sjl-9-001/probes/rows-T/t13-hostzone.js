// SJL-9.1 sub-area (d), rows T21 and T28: the leftover item of Broiler.JS's bundle sjl-6-001 (README lines 356
// and 368: formatToParts with a minute and a second and no hour read them in the host's local time), run with
// the host's zone set to Asia/Kolkata (+05:30) against formats in UTC, in the default zone and in Asia/Kathmandu;
// and the timeZone option's ASCII-only case matching (Broiler.JS's sjl-7-001 README line 548 item), with
// letters whose Unicode case mapping reaches an ASCII letter (U+017F LONG S, U+212A KELVIN SIGN, U+0131
// DOTLESS I). ECMA-402 3d4c558 11.1.2 (GetAvailableNamedTimeZoneIdentifier: ASCII-case-insensitive).
var X = Date.UTC(2026, 9, 16, 12, 0, 5);
function ms(o) { var p = { minute: '2-digit', second: '2-digit' }; for (var k in o) p[k] = o[k]; return parts(new Intl.DateTimeFormat('en', p).formatToParts(X)); }
t('{minute, second} timeZone UTC', function () { return ms({ timeZone: 'UTC' }); });
t('{minute, second} no timeZone (the default zone)', function () { return ms({}); });
t('{minute, second} timeZone Asia/Kathmandu', function () { return ms({ timeZone: 'Asia/Kathmandu' }); });
t('format {minute, second} timeZone UTC', function () { return new Intl.DateTimeFormat('en', { minute: '2-digit', second: '2-digit', timeZone: 'UTC' }).format(X); });
t('default zone resolved', function () { return new Intl.DateTimeFormat('en').resolvedOptions().timeZone; });
['America/Loſ_Angeles', 'Asia/Kolkata', 'Asia/Kolkatı', 'AMERICA/LOS_ANGELES', 'america/los_angeles'].forEach(function (z) {
  t('timeZone ' + q(z), function () { return new Intl.DateTimeFormat('en', { timeZone: z }).resolvedOptions().timeZone; });
});
