// vm-fuel: 200000000000
// Row M9, the control: the same program as m9-charge-long.js but for the one long format.
var s = '0.' + '1'.repeat(1000000);
var nf = new Intl.NumberFormat('en', { maximumFractionDigits: 100 });
t('M9 string length', function () { return s.length; });
t('M9 format("0.1")', function () { return nf.format('0.1'); });
t('M9 the long format (not taken in the control)', function () { return 'not taken'; });
