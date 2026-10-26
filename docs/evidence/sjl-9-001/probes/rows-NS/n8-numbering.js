// Row N8: numbering systems in number formatting: the digits, the symbols written with them, the default
// system of a locale, the option and the -u-nu- keyword, and systems outside ECMA-402's table of simple digits.
var cases = [
  ['en-u-nu-arab', {}], ['en', { numberingSystem: 'arab' }], ['ar', {}], ['ar-EG', {}], ['ar-u-nu-latn', {}], ['fa', {}],
  ['en-u-nu-arabext', {}], ['hi-IN-u-nu-deva', {}], ['th-u-nu-thai', {}], ['en-u-nu-thai', {}], ['de-u-nu-thai', {}],
  ['zh-u-nu-hanidec', {}], ['en-u-nu-fullwide', {}], ['en-u-nu-mathbold', {}], ['en-u-nu-jpanfin', {}], ['en-u-nu-hant', {}],
  ['en-u-nu-roman', {}], ['en-u-nu-xyz', {}], ['en', { numberingSystem: 'xyz' }], ['de-u-nu-arab', { style: 'percent' }],
];
cases.forEach(function (c) {
  t(c[0] + ' ' + JSON.stringify(c[1]) + ' resolved, -1234.5, 0.5', function () {
    var f = new Intl.NumberFormat(c[0], c[1]);
    var r = f.resolvedOptions();
    return [r.locale, r.numberingSystem, f.format(-1234.5), f.format(0.5)];
  });
});
t('en-u-nu-arab formatToParts(-1234.5)', function () { return parts(new Intl.NumberFormat('en-u-nu-arab').formatToParts(-1234.5)); });
t('ar formatToParts(-1234.5)', function () { return parts(new Intl.NumberFormat('ar').formatToParts(-1234.5)); });
t('en-u-nu-arab scientific 1234.5', function () { return new Intl.NumberFormat('en-u-nu-arab', { notation: 'scientific' }).format(1234.5); });
t('en-u-nu-thai currency USD -1234.5', function () { return new Intl.NumberFormat('en-u-nu-thai', { style: 'currency', currency: 'USD' }).format(-1234.5); });
t('numberingSystem option invalid "latn!" and "ab"', function () { return [String((function () { try { new Intl.NumberFormat('en', { numberingSystem: 'latn!' }); return 'ok'; } catch (e) { return e.name; } })()), String((function () { try { new Intl.NumberFormat('en', { numberingSystem: 'ab' }); return 'ok'; } catch (e) { return e.name; } })())]; });
t('(1234.5).toLocaleString("en-u-nu-arab")', function () { return (1234.5).toLocaleString('en-u-nu-arab'); });
