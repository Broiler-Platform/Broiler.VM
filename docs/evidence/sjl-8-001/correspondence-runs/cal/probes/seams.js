// Every ISO day in windows around each source boundary: the calendar date must advance by one day
// (same month, day + 1; or day 1 of the next month) and round-trip through from({year, monthCode, day}).
// Emits each anomaly and a count per window.
function key(c) { return c.year + '/' + c.monthCode + '/' + c.day; }
function scan(cal, startIso, endIso) {
  var d = Temporal.PlainDate.from(startIso), end = Temporal.PlainDate.from(endIso), prev = null, n = 0, bad = 0;
  while (Temporal.PlainDate.compare(d, end) <= 0) {
    var c, note = '';
    try {
      c = d.withCalendar(cal);
      var back = Temporal.PlainDate.from({ calendar: cal, year: c.year, monthCode: c.monthCode, day: c.day }, { overflow: 'reject' });
      if (!back.withCalendar('iso8601').equals(d)) note += ' round-trip ' + back.withCalendar('iso8601').toString();
      if (prev) {
        var next = prev.year === c.year && prev.month === c.month && c.day === prev.day + 1;
        var first = c.day === 1 && prev.day === prev.daysInMonth && ((prev.year === c.year && c.month === prev.month + 1) || (c.year === prev.year + 1 && c.month === 1 && prev.month === prev.monthsInYear));
        if (!next && !first) note += ' after ' + key(prev) + ' (daysInMonth ' + prev.daysInMonth + ', monthsInYear ' + prev.monthsInYear + ')';
      }
    } catch (e) { note += ' throws ' + e.name; c = null; }
    if (note) { bad++; emit(cal + ' ' + d.toString() + ' => ' + (c ? key(c) : '-') + note); }
    prev = c; n++; d = d.add({ days: 1 });
  }
  emit(cal + ' window ' + startIso + '..' + endIso + ' => ' + n + ' days, ' + bad + ' anomalies');
}
scan('persian', '0622-01-01', '0623-06-30');
scan('persian', '9999-01-01', '+010001-06-30');
scan('persian', '2123-01-01', '2124-12-31');
scan('chinese', '1899-06-01', '1902-06-30');
scan('chinese', '1911-06-01', '1912-06-30');
scan('chinese', '2100-06-01', '2104-06-30');
scan('dangi', '0917-06-01', '0918-06-30');
scan('dangi', '1899-06-01', '1902-06-30');
scan('dangi', '2050-06-01', '2052-06-30');
scan('dangi', '2100-06-01', '2104-06-30');
scan('islamic-umalqura', '1881-06-01', '1883-12-31');
scan('islamic-umalqura', '2173-06-01', '2175-06-30');
