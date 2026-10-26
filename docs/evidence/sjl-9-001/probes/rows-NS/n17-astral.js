// Rows N17a (a numbering system whose digits are outside the BMP: decoding the ten digits) and N17b (the range
// collapse's lengths, counted in code points): mathbold and segment digits in format, formatToParts and formatRange,
// with an exponent (the inner layer) and with affixes (the middle layer) around astral digits.
['en-u-nu-mathbold', 'en-u-nu-segment', 'de-u-nu-mathbold'].forEach(function (l) {
  t(l + ' format 0123456789, -1234.5', function () { var f = new Intl.NumberFormat(l, { useGrouping: false }); return [f.format(1234567890), f.format(-1234.5)]; });
  t(l + ' formatToParts(-1.5)', function () { return parts(new Intl.NumberFormat(l).formatToParts(-1.5)); });
  t(l + ' formatRange 3..5, -5..-1, 1000..5000', function () { var f = new Intl.NumberFormat(l); return [f.formatRange(3, 5), f.formatRange(-5, -1), f.formatRange(1000, 5000)]; });
  t(l + ' scientific formatRange 1000..5000, 1..1', function () { var f = new Intl.NumberFormat(l, { notation: 'scientific' }); return [f.formatRange(1000, 5000), f.formatRange(1, 1)]; });
  t(l + ' EUR code formatRange 3..5; percent 0.03..0.05; compact 3000..5000', function () {
    return [new Intl.NumberFormat(l, { style: 'currency', currency: 'EUR', currencyDisplay: 'code' }).formatRange(3, 5),
      new Intl.NumberFormat(l, { style: 'percent' }).formatRange(0.03, 0.05),
      new Intl.NumberFormat(l, { notation: 'compact' }).formatRange(3000, 5000)];
  });
  t(l + ' formatRangeToParts(3, 5) EUR code', function () { return parts(new Intl.NumberFormat(l, { style: 'currency', currency: 'EUR', currencyDisplay: 'code' }).formatRangeToParts(3, 5)); });
});
