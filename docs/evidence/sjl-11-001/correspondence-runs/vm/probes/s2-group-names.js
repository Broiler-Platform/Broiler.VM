// S2: group-name syntax (RegExpIdentifierName, ES2020 and later): ID_Start/ID_Continue by code
// point, $ _ ZWNJ ZWJ, and \u escapes (four digits, braced, or a surrogate pair of escapes) in
// both modes; a lone surrogate or any other character is a SyntaxError.
var cases = [
  ['(?<a>x)', ''], ['(?<$>x)', ''], ['(?<_>x)', ''], ['(?<\u00e9>x)', ''], ['(?<a\u200cb>x)', ''],
  ['(?<a b>x)', ''], ['(?<1>x)', ''], ['(?<a-b>x)', ''], ['(?<a.b>x)', ''], ['(?<>x)', ''],
  ['(?<\\u0041>x)', ''], ['(?<\\u0041>x)', 'u'],
  ['(?<\\u{41}>x)', ''], ['(?<\\u{41}>x)', 'u'],
  ['(?<\\uD835\\uDC00>x)', ''], ['(?<\\uD835\\uDC00>x)', 'u'],
  ['(?<\\u{1D400}>x)', ''], ['(?<\\u{1D400}>x)', 'u'],
  ['(?<\ud835\udc00>x)', ''], ['(?<\ud835\udc00>x)', 'u'],
  ['(?<\\uD800>x)', ''], ['(?<\\uD800>x)', 'u'], ['(?<\\u{D800}>x)', 'u'],
  ['(?<a\\u0020>x)', ''], ['(?<\\u0031>x)', ''], ['(?<\\x41>x)', ''],
  ['(?<\ud835\udfda>x)', ''], ['(?<\ud83e\udd8a>x)', ''],
];
for (var i = 0; i < cases.length; i++) {
  (function (p, f) {
    t(q(p) + ' /' + f + ' new RegExp', function () { var r = C(p, f).exec('x'); return r === null ? R('null') : R('ok groups ' + Object.keys(r.groups).map(q).join(',')); });
    t(q(p) + ' /' + f + ' literal', function () { var r = L(p, f).exec('x'); return r === null ? R('null') : R('ok groups ' + Object.keys(r.groups).map(q).join(',')); });
    t(q(p) + ' /' + f + ' literal, not evaluated', function () { return early(p, f); });
  })(cases[i][0], cases[i][1]);
}
// A reference spelled with an escape names the group spelled without one.
both('(?<A>x)\\k<\\u0041> on "xx"', '(?<A>x)\\k<\\u0041>', '', 'xx');
both('(?<\\u{1D400}>x)\\k<\ud835\udc00> on "xx"', '(?<\\u{1D400}>x)\\k<\ud835\udc00>', '', 'xx');
