// `??` STANDS BESIDE `||` AND `&&` ONLY WITH PARENTHESES AROUND ONE SIDE, and with them each side is
// an ordinary operand: the grammar keeps the two families apart so no reader has to know which binds
// tighter. A chain of `??` alone needs none.
var nothing = null;
var zero = 0;

[(nothing || zero) ?? 'right', nothing ?? (zero || 'fallback'), (nothing ?? zero) && 'and',
  nothing ?? undefined ?? 'last', (zero && nothing) ?? 'coalesced'].join(' / ');
