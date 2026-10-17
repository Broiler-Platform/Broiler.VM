/*---
description: sjl8 calendar probe persian-leap-map
flags: [noStrict]
---*/
var __o = []; function emit(s) { __o.push(String(s)); }
// Shared by every probe: one line per operation, "label => answer" or "label => throws Name".
function show(v) { return v === undefined ? 'undefined' : String(v); }
function t(label, fn) {
  var r;
  try { r = show(fn()); } catch (e) { r = 'throws ' + (e && e.name); }
  emit(label + ' => ' + r);
}
// inLeapYear for every Persian year 1 to 9400 AP, one line of 100 years ('1' leap, '0' common, 'E' error).
for (var c = 0; c < 94; c++) {
  var s = '';
  for (var y = c * 100 + 1; y <= c * 100 + 100; y++) {
    try { s += Temporal.PlainDate.from({ calendar: 'persian', year: y, month: 1, day: 1 }).inLeapYear ? '1' : '0'; } catch (e) { s += 'E'; }
  }
  emit('persian ' + (c * 100 + 1) + '-' + (c * 100 + 100) + ' => ' + s);
}

throw new Error('@@OUT@@\n' + __o.join('\n') + '\n@@END@@');
