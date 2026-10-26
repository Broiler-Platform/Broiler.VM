// vm-fuel: 200000000000
// Row N16, the control: the same program as n16-charge-list.js but for its three formatting calls.
var a = [];
for (var i = 0; i < 20000; i++) a.push('x');
var lf = new Intl.ListFormat('en');
var nf = new Intl.NumberFormat('en');
t('N16 list length', function () { return a.length; });
t('N16 the list formatted (length)', function () { return 'not taken'; });
t('N16 the list formatted to parts (count)', function () { return 'not taken'; });
t('N16 2,000 numbers formatted to parts (parts)', function () { var n = 0; for (var j = 0; j < 2000; j++) n += 0; return 'not taken'; });
