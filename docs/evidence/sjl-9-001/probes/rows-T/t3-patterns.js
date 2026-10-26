// SJL-9.1 sub-area (d), rows T8-T12: the formats a locale's data and the pattern generator give, for
// en and de (Broiler.VM's data locales) and for locales outside them: the default, every date and
// time style and their joins, and component sets through the format matcher; the names (months,
// weekdays, eras) the formats write. ECMA-402 3d4c558 11.1.2, 11.5.1-11.5.4 (DateTimeStyleFormat,
// BasicFormatMatcher, BestFitFormatMatcher), FormatDateTimePattern as e8cc03fc amends it.
var A = Date.UTC(2024, 0, 5, 15, 7, 9, 123);
function f(l, o) { var p = { timeZone: 'UTC' }; for (var k in o) p[k] = o[k]; return new Intl.DateTimeFormat(l, p).format(A); }
var styles = ['full', 'long', 'medium', 'short'];
['en', 'de'].forEach(function (l) {
  t(l + ' default', function () { return f(l, {}); });
  styles.forEach(function (d) { t(l + ' dateStyle ' + d, function () { return f(l, { dateStyle: d }); }); });
  styles.forEach(function (s) { t(l + ' timeStyle ' + s, function () { return f(l, { timeStyle: s }); }); });
  styles.forEach(function (d) { styles.forEach(function (s) { t(l + ' dateStyle ' + d + ' timeStyle ' + s, function () { return f(l, { dateStyle: d, timeStyle: s }); }); }); });
});
var sets = [
  { year: 'numeric' }, { year: '2-digit' }, { month: 'numeric' }, { month: '2-digit' }, { month: 'narrow' }, { month: 'short' }, { month: 'long' },
  { day: 'numeric' }, { day: '2-digit' }, { weekday: 'narrow' }, { weekday: 'short' }, { weekday: 'long' }, { era: 'short' }, { era: 'long' }, { era: 'narrow' },
  { year: 'numeric', month: 'short' }, { year: 'numeric', month: 'long' }, { year: 'numeric', month: 'numeric' }, { year: '2-digit', month: '2-digit' },
  { month: 'short', day: 'numeric' }, { month: 'long', day: 'numeric' }, { month: 'numeric', day: 'numeric' }, { month: '2-digit', day: '2-digit' },
  { year: 'numeric', month: 'short', day: 'numeric' }, { year: 'numeric', month: 'long', day: 'numeric' }, { year: 'numeric', month: '2-digit', day: '2-digit' },
  { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }, { weekday: 'short', month: 'short', day: 'numeric' }, { weekday: 'long', day: 'numeric' },
  { era: 'short', year: 'numeric' }, { era: 'long', year: 'numeric', month: 'long', day: 'numeric' }, { era: 'short', month: 'short', day: 'numeric' },
  { hour: 'numeric' }, { hour: '2-digit' }, { minute: 'numeric' }, { minute: '2-digit' }, { second: 'numeric' }, { hour: 'numeric', minute: 'numeric' },
  { hour: '2-digit', minute: '2-digit' }, { hour: 'numeric', minute: 'numeric', second: 'numeric' }, { minute: 'numeric', second: 'numeric' },
  { hour: 'numeric', minute: '2-digit', second: '2-digit', fractionalSecondDigits: 3 }, { second: 'numeric', fractionalSecondDigits: 2 }, { fractionalSecondDigits: 1 },
  { hour: 'numeric', timeZoneName: 'short' }, { hour: 'numeric', minute: 'numeric', timeZoneName: 'long' }, { timeZoneName: 'short' }, { timeZoneName: 'shortOffset' },
  { year: 'numeric', month: 'short', day: 'numeric', hour: 'numeric', minute: 'numeric' }, { weekday: 'long', hour: 'numeric' },
  { year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric', timeZoneName: 'short' },
  { weekday: 'short', year: 'numeric', month: 'numeric', day: 'numeric', hour: '2-digit', minute: '2-digit', hour12: false }
];
['en', 'de'].forEach(function (l) {
  sets.forEach(function (o) { t(l + ' ' + show(o), function () { return f(l, o); }); });
});
// Locales outside Broiler.VM's data: what each engine resolves and writes.
['fr', 'es', 'it', 'ja', 'ko', 'zh-TW', 'ar-EG', 'ru', 'en-GB', 'en-IN', 'de-AT', 'pt-BR'].forEach(function (l) {
  t(l + ' locale, default, dateStyle long, month long, weekday long, timeStyle short', function () {
    return [new Intl.DateTimeFormat(l).resolvedOptions().locale, f(l, {}), f(l, { dateStyle: 'long' }), f(l, { month: 'long' }), f(l, { weekday: 'long' }), f(l, { timeStyle: 'short' })];
  });
});
// Every month's and weekday's name in each width, en and de.
['en', 'de'].forEach(function (l) {
  ['long', 'short', 'narrow'].forEach(function (w) {
    t(l + ' months ' + w + ' (format, with a day)', function () { var r = []; for (var m = 0; m < 12; m++) r.push(new Intl.DateTimeFormat(l, { month: w, day: 'numeric', timeZone: 'UTC' }).format(Date.UTC(2024, m, 1))); return r; });
    t(l + ' months ' + w + ' (stand-alone)', function () { var r = []; for (var m = 0; m < 12; m++) r.push(new Intl.DateTimeFormat(l, { month: w, timeZone: 'UTC' }).format(Date.UTC(2024, m, 1))); return r; });
    t(l + ' weekdays ' + w, function () { var r = []; for (var d = 0; d < 7; d++) r.push(new Intl.DateTimeFormat(l, { weekday: w, timeZone: 'UTC' }).format(Date.UTC(2024, 0, 7 + d))); return r; });
  });
});
