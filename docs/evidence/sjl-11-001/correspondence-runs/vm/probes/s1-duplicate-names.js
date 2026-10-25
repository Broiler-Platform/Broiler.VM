// S1: duplicate named groups (ES2025, test262 feature regexp-duplicate-named-groups): a name may
// repeat only across alternatives that cannot both participate; \k and groups read the one that did.
both('(?<a>x)|(?<a>y) on "y"', '(?<a>x)|(?<a>y)', '', 'y');
both('(?<a>x)|(?<a>y) /d on "y"', '(?<a>x)|(?<a>y)', 'd', 'y');
both('(?<a>x)|(?<a>y) /u on "y"', '(?<a>x)|(?<a>y)', 'u', 'y');
both('(?<a>x)|(?<a>y) /v on "x"', '(?<a>x)|(?<a>y)', 'v', 'x');
both('(?:(?<a>x)|(?<a>y))\\k<a> on "yy"', '(?:(?<a>x)|(?<a>y))\\k<a>', '', 'yy');
both('(?:(?<a>x)|(?<a>y))\\k<a> /u on "xx"', '(?:(?<a>x)|(?<a>y))\\k<a>', 'u', 'xx');
both('(?<a>x)(?<a>y) (same alternative)', '(?<a>x)(?<a>y)', '', 'xy');
both('(?<a>x)(?<a>y) /u (same alternative)', '(?<a>x)(?<a>y)', 'u', 'xy');
both('(?<a>x)|(?:(?<a>y)|(?<a>z)) on "z"', '(?<a>x)|(?:(?<a>y)|(?<a>z))', '', 'z');
both('((?<a>x)|(?<a>y))(?<a>z) (one alternative repeats)', '((?<a>x)|(?<a>y))(?<a>z)', '', 'xz');
// The same name spelled once with an escape: the duplicate test is on the decoded name.
both('(?<\\u0061>x)(?<a>y) (escaped duplicate, same alternative)', '(?<\\u0061>x)(?<a>y)', '', 'xy');
both('(?<\\u0061>x)|(?<a>y) (escaped duplicate, alternatives) on "y"', '(?<\\u0061>x)|(?<a>y)', '', 'y');
both('(?<\\u0061>x)(?<a>y) /u', '(?<\\u0061>x)(?<a>y)', 'u', 'xy');
t('replace $<a> over (?<a>x)|(?<a>y) /g', function () { return 'xyz'.replace(C('(?<a>x)|(?<a>y)', 'g'), '[$<a>]'); });
t('group property order (?<b>.)(?<a>.)|(?<a>.)(?<b>.)', function () { var r = C('(?<b>.)(?<a>.)|(?<a>.)(?<b>.)').exec('pq'); return R(Object.keys(r.groups).join(',') + ' a=' + r.groups.a + ' b=' + r.groups.b); });
