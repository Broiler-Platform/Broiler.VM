// THE BASE OF `**` IS NOT A UNARY EXPRESSION: `-2 ** 2` is an early SyntaxError, because the reader
// of `-2 ** 2` and the reader of `(-2) ** 2` would disagree about its value. The parenthesised form
// is a program.
var squared = -2 ** 2;
