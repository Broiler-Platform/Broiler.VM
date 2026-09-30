// `??` CANNOT STAND BESIDE `||` OR `&&` WITHOUT PARENTHESES AROUND ONE OF THEM. The grammar keeps the
// two families apart, and until 2026-09-30 this host read the line as `(a ?? b) || c`.
var a = null;
var b = 0;
var c = 'c';
var mixed = a ?? b || c;
