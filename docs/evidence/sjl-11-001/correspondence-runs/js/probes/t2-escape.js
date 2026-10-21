// T2: RegExp.escape (22.2.5.1, EncodeForRegExpEscape over the pinned WhiteSpace and LineTerminator).
var inputs = [
  'abc', '1a', 'a1', '_', 'a b', '\u00e9', '\u0085', '\u00a0', '\ufeff', '\u2028', '\u2029', '\u3000', '\u180e',
  '\u1680', '\u202f', '\ud800', '\udfff', '\ud83d\ude00', 'x\ud83d', ',-=<>#&!%:;@~\'`"', '^$\\.*+?()[]{}|/',
  '\t\n\u000b\f\r', ' ', '', 'Z\u0000', '\u0080', '\u00ff', '\u0100',
];
for (var i = 0; i < inputs.length; i++) {
  (function (s) {
    t('RegExp.escape(' + q(s) + ')', function () { return RegExp.escape(s); });
  })(inputs[i]);
}
t('RegExp.escape(1)', function () { return RegExp.escape(1); });
t('RegExp.escape.length', function () { return RegExp.escape.length; });
t('new RegExp(RegExp.escape("a.b*c")).test("a.b*c")', function () { return new RegExp(RegExp.escape('a.b*c')).test('a.b*c'); });
t('new RegExp(RegExp.escape(",-"), "u").test(",-")', function () { return new RegExp(RegExp.escape(',-'), 'u').test(',-'); });
