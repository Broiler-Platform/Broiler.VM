// S9: a `/` the grammar reads as division against one it reads as a regular-expression literal,
// and an invalid literal where the grammar wants a literal (an early SyntaxError). Each program
// runs through an indirect eval, so a refusal is a SyntaxError thrown by eval.
function ev(src) { return (0, eval)(src); }
// Division after an object literal's `}`, across a line break (no ASI: the division continues).
t('var a=8,gg=2; var x = {}\\n/a/gg; x', function () { return ev('var a=8,gg=2; var x = {}\n/a/gg; x'); });
t('var a=8,g=2; var x = {}\\n/a/g; x', function () { return ev('var a=8,g=2; var x = {}\n/a/g; x'); });
t('var a=8,g=2; var x = function(){}\\n/a/g; x', function () { return ev('var a=8,g=2; var x = function(){}\n/a/g; x'); });
// `yield` outside a generator is an identifier, so division.
t('var yield=6,a=2,g=3; yield /a/g', function () { return ev('var yield=6,a=2,g=3; yield /a/g'); });
// Where the grammar wants a literal, an invalid one is an early SyntaxError.
t('/a/gg (statement start)', function () { return ev('/a/gg'); });
t('x = /(?<a>.)(?<a>.)/ (duplicate in one alternative)', function () { return ev('var x = /(?<a>.)(?<a>.)/; x'); });
t('if (1) /(/ ', function () { return ev('if (1) /(/'); });
t('return-position: (function(){ return /a/uu })()', function () { return ev('(function(){ return /a/uu })()'); });
t('typeof /[b-a]/', function () { return ev('typeof /[b-a]/'); });
t('generator: yield /a/gg', function () { return ev('(function*(){ yield /a/gg })'); });
t('x = 1 ? /\\u{110000}/u : 0', function () { return ev('var x = 1 ? /\\u{110000}/u : 0; x'); });
t('x = [/a)/]', function () { return ev('var x = [/a)/]; x'); });
// An invalid literal in an array: `a)` closes a group that was never opened.
t('[/a)\\/(/].length (invalid: an unbalanced parenthesis)', function () { return ev('[/a)\\/(/].length'); });
// Valid literals in literal position, for contrast.
t('typeof /a/g', function () { return ev('typeof /a/g'); });
t('[/a\\/(b)/].length', function () { return ev('[/a\\/(b)/].length'); });
// A line terminator after a backslash in a literal (test262 staging/sm/RegExp/7.8.5-01.js): the
// literal's own grammar refuses it (RegularExpressionBackslashSequence), an early SyntaxError, by
// direct and by indirect eval; an unterminated string literal by direct eval, for contrast.
t('eval("/\\\\<LF>/") (direct)', function () { return eval('/\\\n/'); });
t('(0, eval)("/\\\\<LF>/") (indirect)', function () { return (0, eval)('/\\\n/'); });
t('eval("/ab\\\\<U+2028>/") (direct)', function () { return eval('/ab\\\u2028/'); });
t('eval("/ab[c\\\\<LF>]/") (direct)', function () { return eval('/ab[c\\\n]/'); });
t('eval("/a[bc\\\\") (direct)', function () { return eval('/a[bc\\'); });
t('eval("\\"a<LF>b\\"") (direct; a string literal, for contrast)', function () { return eval('"a\nb"'); });
