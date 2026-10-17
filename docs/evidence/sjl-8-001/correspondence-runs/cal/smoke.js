print(typeof Temporal, typeof Intl);
var d = Temporal.PlainDate.from({year: 2024, monthCode: 'M01', day: 1, calendar: 'chinese'});
print(d.toString(), d.year, d.monthCode, d.monthsInYear);
console.log("console works");
