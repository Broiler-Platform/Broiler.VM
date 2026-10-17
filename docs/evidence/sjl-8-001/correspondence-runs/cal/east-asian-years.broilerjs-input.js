function emit(s) { print(String(s)); }
// Shared by every probe: one line per operation, "label => answer" or "label => throws Name".
function show(v) { return v === undefined ? 'undefined' : String(v); }
function t(label, fn) {
  var r;
  try { r = show(fn()); } catch (e) { r = 'throws ' + (e && e.name); }
  emit(label + ' => ' + r);
}
// Chinese and Korean (dangi) years by related ISO year: the ISO date of month 1 day 1, months in the
// year, inLeapYear, the leap month's code, days in the year, and each ordinal month's length (l = 30, s = 29).
function year(cal, y) {
  var d = Temporal.PlainDate.from({ calendar: cal, year: y, month: 1, day: 1 }, { overflow: 'reject' });
  var n = d.monthsInYear, leap = '-', lengths = '';
  for (var m = 1; m <= n; m++) {
    var x = Temporal.PlainDate.from({ calendar: cal, year: y, month: m, day: 1 }, { overflow: 'reject' });
    if (x.monthCode.length === 4) leap = x.monthCode + '@' + m;
    lengths += x.daysInMonth === 30 ? 'l' : 's';
  }
  return d.withCalendar('iso8601').toString() + ' ' + n + ' ' + d.inLeapYear + ' ' + leap + ' ' + d.daysInYear + ' ' + lengths;
}
var years = [];
for (var y = 1880; y <= 2210; y++) years.push(y);
[-271820, -500, 0, 1, 500, 622, 917, 918, 919, 1000, 1500, 1582, 1600, 1700, 1800, 1850, 2300, 2500, 3000, 5000, 9999, 100000, 275759].forEach(function (y) { years.push(y); });
['chinese', 'dangi'].forEach(function (cal) {
  years.forEach(function (y) { t(cal + ' ' + y, function () { return year(cal, y); }); });
});
