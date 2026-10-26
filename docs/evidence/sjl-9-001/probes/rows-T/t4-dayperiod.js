// SJL-9.1 sub-area (d), rows T13-T14: day periods. The dayPeriod option (CLDR's flexible day periods,
// pattern letter B) alone, with an hour, with an hour and minutes, and inside other formats; noon and
// midnight; the AM/PM marker (a) in en and de. ECMA-402 3d4c558 11.1.2 (Table 7's dayPeriod), and
// FormatDateTimePattern as e8cc03fc amends it ("a String value representing the day period ... depends
// upon the implementation and the effective locale").
function at(h, m) { return Date.UTC(2024, 0, 5, h, m || 0, 0); }
var hours = [[0, 0], [0, 30], [3, 0], [6, 0], [9, 15], [11, 59], [12, 0], [12, 1], [13, 0], [15, 0], [18, 0], [20, 59], [21, 0], [23, 0]];
['en', 'de'].forEach(function (l) {
  ['narrow', 'short', 'long'].forEach(function (w) {
    t(l + ' {dayPeriod:' + w + '} at ' + hours.map(function (h) { return h[0] + ':' + h[1]; }).join(' '), function () {
      var f = new Intl.DateTimeFormat(l, { dayPeriod: w, timeZone: 'UTC' }); return hours.map(function (h) { return f.format(at(h[0], h[1])); });
    });
  });
  t(l + ' {dayPeriod:short, hour:numeric}', function () { var f = new Intl.DateTimeFormat(l, { dayPeriod: 'short', hour: 'numeric', timeZone: 'UTC' }); return hours.map(function (h) { return f.format(at(h[0], h[1])); }); });
  t(l + ' {dayPeriod:long, hour:numeric, minute:numeric}', function () { var f = new Intl.DateTimeFormat(l, { dayPeriod: 'long', hour: 'numeric', minute: 'numeric', timeZone: 'UTC' }); return hours.map(function (h) { return f.format(at(h[0], h[1])); }); });
  t(l + ' {dayPeriod:narrow, hour:numeric, minute:numeric, second:numeric}', function () { var f = new Intl.DateTimeFormat(l, { dayPeriod: 'narrow', hour: 'numeric', minute: 'numeric', second: 'numeric', timeZone: 'UTC' }); return [f.format(at(12, 0)), f.format(at(15, 0))]; });
  t(l + ' {dayPeriod:short, hour:numeric, hourCycle:h23}', function () { var f = new Intl.DateTimeFormat(l, { dayPeriod: 'short', hour: 'numeric', hourCycle: 'h23', timeZone: 'UTC' }); return [f.format(at(9)), f.format(at(15)), f.resolvedOptions().hourCycle]; });
  t(l + ' {dayPeriod:short, year:numeric, month:short, day:numeric}', function () { return new Intl.DateTimeFormat(l, { dayPeriod: 'short', year: 'numeric', month: 'short', day: 'numeric', timeZone: 'UTC' }).format(at(15)); });
  t(l + ' {dayPeriod:short, weekday:long}', function () { return new Intl.DateTimeFormat(l, { dayPeriod: 'short', weekday: 'long', timeZone: 'UTC' }).format(at(15)); });
  t(l + ' {dayPeriod:long, hour:numeric} formatToParts at 15:00', function () { return parts(new Intl.DateTimeFormat(l, { dayPeriod: 'long', hour: 'numeric', timeZone: 'UTC' }).formatToParts(at(15))); });
  t(l + ' {dayPeriod:long, minute:numeric} at 15:07', function () { return new Intl.DateTimeFormat(l, { dayPeriod: 'long', minute: 'numeric', timeZone: 'UTC' }).format(at(15, 7)); });
  t(l + ' {dayPeriod:short} in Asia/Kolkata at 06:00 UTC', function () { return new Intl.DateTimeFormat(l, { dayPeriod: 'short', hour: 'numeric', timeZone: 'Asia/Kolkata' }).format(at(6)); });
  t(l + ' {hour:numeric, hour12:true} AM/PM', function () { var f = new Intl.DateTimeFormat(l, { hour: 'numeric', hour12: true, timeZone: 'UTC' }); return [f.format(at(0)), f.format(at(12)), f.format(at(23))]; });
});
