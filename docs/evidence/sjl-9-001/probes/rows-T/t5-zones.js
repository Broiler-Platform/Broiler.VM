// SJL-9.1 sub-area (d), rows T15-T18 and Z8: the timeZone option (resolution of a name, an alias and
// an offset; row Z8's resolved identifier), the default zone, and the names a format writes for a zone
// (timeZoneName in its six forms: specific, generic with the location and partial-location forms,
// offsets), with CLDR's metazone history, in en and de. ECMA-402 3d4c558 11.1.2 steps on timeZone
// (GetAvailableNamedTimeZoneIdentifier, [[PrimaryIdentifier]]) as e8cc03fc intl.html:434 replaces it with
// [[Identifier]]; FormatDateTimePattern's timeZoneName ("depends upon the implementation and the
// effective locale").
var W = Date.UTC(2024, 0, 15, 12, 0), S = Date.UTC(2024, 6, 15, 12, 0);
function rtz(z) { return new Intl.DateTimeFormat('en', { timeZone: z }).resolvedOptions().timeZone; }
['Asia/Calcutta', 'asia/calcutta', 'Asia/Kolkata', 'ASIA/KOLKATA', 'US/Eastern', 'America/New_York', 'Europe/Kiev', 'Europe/Kyiv', 'America/Buenos_Aires',
 'UTC', 'utc', 'Etc/UTC', 'Etc/GMT', 'GMT', 'Etc/UCT', 'Universal', 'Zulu', 'Etc/GMT+5', 'etc/gmt-14', 'Etc/GMT0', 'EST', 'CET', 'Atlantic/Jan_Mayen',
 'Antarctica/South_Pole', 'Pacific/Enderbury', 'America/Godthab',
 '+05:30', '+0530', '-08', '+00:00', '-00:00', '+05:30:00', '+24:00', 'Mars/Phobos', '', 'Asia/Kolkata ', '2024-01-01T00:00+01:00[Europe/Paris]'].forEach(function (z) {
  t('resolved timeZone for ' + show(z), function () { return rtz(z); });
});
t('timeZone: undefined', function () { return new Intl.DateTimeFormat('en', { timeZone: undefined }).resolvedOptions().timeZone; });
t('timeZone: an object with toString', function () { return rtz({ toString: function () { return 'Europe/Berlin'; } }); });
t('timeZone: Symbol', function () { return rtz(Symbol('x')); });
// Formatting in a zone: the offset applies (link and primary alike).
t('Asia/Calcutta and Asia/Kolkata format alike', function () { var o = { hour: 'numeric', minute: 'numeric', hourCycle: 'h23' }; o.timeZone = 'Asia/Calcutta'; var a = new Intl.DateTimeFormat('en', o).format(W); o.timeZone = 'Asia/Kolkata'; return [a, new Intl.DateTimeFormat('en', o).format(W)]; });
// Names.
var zones = ['UTC', 'Etc/GMT', 'America/New_York', 'US/Eastern', 'America/Los_Angeles', 'America/Phoenix', 'America/Indiana/Knox', 'America/Indiana/Indianapolis',
  'Europe/Berlin', 'Europe/London', 'Europe/Dublin', 'Europe/Moscow', 'Asia/Calcutta', 'Asia/Kolkata', 'Asia/Tokyo', 'Australia/Lord_Howe', 'Pacific/Honolulu',
  'America/Sao_Paulo', 'Africa/Casablanca', 'America/Argentina/Buenos_Aires', 'Etc/GMT+5', 'Etc/GMT-14', 'Antarctica/Troll', 'Pacific/Chatham', '+05:30', '-08:00', '+00:00'];
var styles = ['short', 'long', 'shortOffset', 'longOffset', 'shortGeneric', 'longGeneric'];
['en', 'de'].forEach(function (l) {
  zones.forEach(function (z) {
    t(l + ' ' + z + ' names (winter; summer) ' + styles.join(' '), function () {
      return [W, S].map(function (when) { return styles.map(function (s) { var p = new Intl.DateTimeFormat(l, { timeZone: z, timeZoneName: s }).formatToParts(when); for (var i = 0; i < p.length; i++) if (p[i].type === 'timeZoneName') return p[i].value; return '(none)'; }).join(' | '); });
    });
  });
});
// Metazone history: a zone whose metazone changed.
[['America/Indiana/Knox', Date.UTC(1995, 0, 15)], ['America/Indiana/Knox', Date.UTC(2010, 0, 15)], ['Europe/Moscow', Date.UTC(2012, 0, 15)], ['Europe/Moscow', Date.UTC(2016, 0, 15)],
 ['America/Sao_Paulo', Date.UTC(2018, 0, 15)], ['Asia/Kolkata', Date.UTC(1942, 6, 1)], ['Europe/London', Date.UTC(1970, 0, 1)], ['Pacific/Apia', Date.UTC(2010, 0, 1)],
 ['Asia/Calcutta', -8.64e15], ['UTC', -1], ['Etc/GMT', -1], ['Etc/GMT', 0], ['Etc/GMT', 8.64e15]].forEach(function (c) {
  t('en ' + c[0] + ' at ' + c[1] + ' long, longGeneric', function () { return ['long', 'longGeneric', 'shortOffset'].map(function (s) { return new Intl.DateTimeFormat('en', { timeZone: c[0], timeZoneName: s, year: 'numeric' }).format(c[1]); }); });
});
// The format with a zone name inside a style.
t('en timeStyle full in America/New_York (summer)', function () { return new Intl.DateTimeFormat('en', { timeStyle: 'full', timeZone: 'America/New_York' }).format(S); });
t('de timeStyle full in Europe/Berlin (summer)', function () { return new Intl.DateTimeFormat('de', { timeStyle: 'full', timeZone: 'Europe/Berlin' }).format(S); });
t('en timeStyle long in Asia/Calcutta', function () { return new Intl.DateTimeFormat('en', { timeStyle: 'long', timeZone: 'Asia/Calcutta' }).format(S); });
