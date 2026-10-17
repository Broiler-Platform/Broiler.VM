// inLeapYear for every Persian year 1 to 9400 AP, one line of 100 years ('1' leap, '0' common, 'E' error).
for (var c = 0; c < 94; c++) {
  var s = '';
  for (var y = c * 100 + 1; y <= c * 100 + 100; y++) {
    try { s += Temporal.PlainDate.from({ calendar: 'persian', year: y, month: 1, day: 1 }).inLeapYear ? '1' : '0'; } catch (e) { s += 'E'; }
  }
  emit('persian ' + (c * 100 + 1) + '-' + (c * 100 + 100) + ' => ' + s);
}
