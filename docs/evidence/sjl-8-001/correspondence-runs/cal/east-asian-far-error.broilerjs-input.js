// Broiler.JS only: the Chinese calendar's fields far from the present, each access caught.
['-271821-04-19', '-250000-01-01', '-200000-01-01', '-150000-01-01', '-100000-01-01', '-050000-01-01', '-020000-01-01', '-010000-01-01', '-005000-01-01',
 '+005000-01-01', '+010000-01-01', '+020000-01-01', '+050000-01-01', '+100000-01-01', '+150000-01-01', '+200000-01-01', '+250000-01-01', '+275000-01-01', '+275760-09-13'].forEach(function (iso) {
  var out;
  try {
    var c = Temporal.PlainDate.from(iso).withCalendar('chinese'), lengths = [];
    var first = Temporal.PlainDate.from({ calendar: 'chinese', year: c.year, month: 1, day: 1 });
    for (var m = 1; m <= first.monthsInYear; m++) lengths.push(Temporal.PlainDate.from({ calendar: 'chinese', year: c.year, month: m, day: 1 }).daysInMonth);
    out = 'year ' + c.year + ' ' + c.monthCode + ' day ' + c.day + '; months ' + lengths.join(',') + ' sum ' + lengths.reduce(function (a, b) { return a + b; }, 0) + ' daysInYear ' + first.daysInYear;
  } catch (e) { out = 'throws ' + e.name + ': ' + e.message; }
  print(iso + ' => ' + out);
});
