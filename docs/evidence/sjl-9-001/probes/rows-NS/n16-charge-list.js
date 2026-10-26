// vm-fuel: 200000000000
// Row N16: metering inside the area's formatters. A list of 20,000 Strings, formatted and formatted to parts
// by Intl.ListFormat, and 2,000 numbers formatted to parts by Intl.NumberFormat; n16-charge-control.js is the same
// program with those three calls not taken.
var a = [];
for (var i = 0; i < 20000; i++) a.push('x');
var lf = new Intl.ListFormat('en');
var nf = new Intl.NumberFormat('en');
t('N16 list length', function () { return a.length; });
t('N16 the list formatted (length)', function () { return lf.format(a).length; });
t('N16 the list formatted to parts (count)', function () { return lf.formatToParts(a).length; });
t('N16 2,000 numbers formatted to parts (parts)', function () { var n = 0; for (var j = 0; j < 2000; j++) n += nf.formatToParts(1234567.25).length; return n; });
