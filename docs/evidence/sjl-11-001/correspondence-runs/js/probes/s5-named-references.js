// S5: \k. With u, or with any named group in the pattern, \k must be \k<name> naming a group of
// the pattern (an early error otherwise, wherever the group is). Without u and without named
// groups, \k is Annex B's identity escape.
both('\\k<b>(?<a>x) /u (no group b)', '\\k<b>(?<a>x)', 'u', 'x');
both('\\k<b>(?<a>x) / (no group b, a named group exists)', '\\k<b>(?<a>x)', '', 'x');
both('\\k<a>(?<a>x) /u (forward reference)', '\\k<a>(?<a>x)', 'u', 'x');
both('(?<a>x)\\k<a> /u on "xx"', '(?<a>x)\\k<a>', 'u', 'xx');
both('\\k<a> / (no named groups) on "k<a>"', '\\k<a>', '', 'k<a>');
both('\\k<a> /u (no named groups)', '\\k<a>', 'u');
both('\\k / on "k"', '\\k', '', 'k');
both('(?<a>.)\\k /', '(?<a>.)\\k', '');
both('(?<a>.)\\k<a /', '(?<a>.)\\k<a', '');
both('(?<a>.)\\k<> /', '(?<a>.)\\k<>', '');
both('\\k<a>(?<b>x) / (k<a> literal? no: a named group exists)', '\\k<a>(?<b>x)', '', 'x');
both('[\\k<a>](?<a>x) / (\\k inside a class)', '[\\k<a>](?<a>x)', '', 'kx');
