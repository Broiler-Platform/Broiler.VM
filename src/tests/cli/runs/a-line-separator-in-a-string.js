// U+2028 and U+2029 are string characters (ES2019), and a continuation adds nothing to a cooked template.
var s = 'a b c';
[s.length, s.charCodeAt(1).toString(16), s.charCodeAt(3).toString(16), String.raw`a\
b`.length, `a\
b`.length].join(' ');
